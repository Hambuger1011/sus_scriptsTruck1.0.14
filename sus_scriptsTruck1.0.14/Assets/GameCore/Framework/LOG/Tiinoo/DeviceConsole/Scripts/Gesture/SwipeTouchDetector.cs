using Framework;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Tiinoo.DeviceConsole
{
	public class SwipeTouchDetector : SwipeDetector
	{
		private int m_fingerCount;
		private float m_tolerableDegreeCos;
		
		private Dictionary<int, Vector2> m_touchInitPositions = new Dictionary<int, Vector2>();	// fingerId, touchInitPosition
		private Dictionary<int, Swipe> m_swipes = new Dictionary<int, Swipe>();					// fingerId, Swipe
		
		public SwipeTouchDetector(int fingerCount = 1)
		{
			m_fingerCount = fingerCount;
			m_tolerableDegreeCos = CalculateTolerableDegreeCos();
		}
		
		public override void Update()
		{
			m_swipes.Clear();
			
			foreach (Touch touch in Input.touches)
			{
				switch (touch.phase)
				{
				case TouchPhase.Began:
					HandleTouchBegan(touch);
					break;
					
				case TouchPhase.Moved:
					break;
					
				case TouchPhase.Ended:
					HandleTouchEnded(touch);
					break;
					
				case TouchPhase.Canceled:
					HandleTouchCanceled(touch);
					break;
					
				default:
					break;
				}
			}
			
			ProcessSwipes();
		}
		
		private void HandleTouchBegan(Touch touch)
		{
			m_touchInitPositions[touch.fingerId] = touch.position;
		}
		
		private void HandleTouchEnded(Touch touch)
		{
			if (!m_touchInitPositions.ContainsKey(touch.fingerId))
			{
				return;
			}
			
			Vector2 beginPos = m_touchInitPositions[touch.fingerId];
			Vector2 endPos = touch.position;
			Vector2 offset = endPos - beginPos;
			Swipe swipe = DetectSwipe(offset, m_tolerableDegreeCos);
			m_touchInitPositions.Remove(touch.fingerId);
			
			m_swipes[touch.fingerId] = swipe;
		}
		
		private void HandleTouchCanceled(Touch touch)
		{
			m_touchInitPositions.Remove(touch.fingerId);
		}
		
		private void ProcessSwipes()
		{
			if (Input.touchCount != m_fingerCount)
			{
				return;
			}
			
			// if there is only one valid swipe, and other swipes are invalid, we succeeded to detect the swipe.
			// if there are two valid swipes, but they are not equal, we failed to detect the swipe.
			Swipe swipe = Swipe.Invalid;
			foreach (KeyValuePair<int, Swipe> pair in m_swipes)
			{
				if (pair.Value == Swipe.Invalid)
				{
					continue;
				}
				
				if (swipe == Swipe.Invalid)
				{
					swipe = pair.Value;
				}
				else
				{
					if (swipe != pair.Value)
					{
						return;
					}
				}
			}
			
			NotifySwipe(swipe);
		}
	}
}

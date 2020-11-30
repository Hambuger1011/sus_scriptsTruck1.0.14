using Framework;

using UnityEngine;
using System.Collections;
using Tiinoo.DeviceConsole;

namespace Tiinoo.DeviceConsole
{
	public class WindowDetector : MonoBehaviour 
	{
		public GestureDetector m_swipeDetector;
		public GestureDetector m_keyDownDetector;
		
		void Start() 
		{
			LogHandler.onExceptionOccur += HandleOnExceptionOccur;
			CreateDetectors();
		}
		
		void OnDestroy() 
		{
			LogHandler.onExceptionOccur -= HandleOnExceptionOccur;
		}
		
		void OnEnable()
		{
			SwipeDetector.onSwipeDown += HandleOnSwipeDown;
			KeyDownDetector.onKeyDown += HandleOnKeyDown;
		}
		
		void OnDisable()
		{
			SwipeDetector.onSwipeDown -= HandleOnSwipeDown;
			KeyDownDetector.onKeyDown -= HandleOnKeyDown;
		}
		
		void Update()
		{
			DetectInput();
		}
		
		private void CreateDetectors()
		{
			DCSettings settings = DCSettings.Instance;
			
			DCSettings.Gesture openWithTouch = settings.openWithTouch;
			switch (openWithTouch)
			{
			case DCSettings.Gesture.None:
				break;
				
			case DCSettings.Gesture.SWIPE_DOWN_WITH_ONE_FINGER:
				m_swipeDetector = new SwipeTouchDetector(1);
				break;
				
			case DCSettings.Gesture.SWIPE_DOWN_WITH_TWO_FINGERS:
				m_swipeDetector = new SwipeTouchDetector(2);
				break;
				
			case DCSettings.Gesture.SWIPE_DOWN_WITH_THREE_FINGERS:
				m_swipeDetector = new SwipeTouchDetector(3);
				break;
			}
			
			KeyCode key = settings.openWithKey;
			m_keyDownDetector = new KeyDownDetector(key);
		}
		
		private void DetectInput()
		{
			if (m_swipeDetector != null)
			{
				m_swipeDetector.Update();
			}
			
			if (m_keyDownDetector != null)
			{
				m_keyDownDetector.Update();
			}
		}
		
		private void HandleOnSwipeDown()
		{
			ShowWindowConsole();
		}
		
		private void HandleOnKeyDown(KeyCode key)
		{
			ShowWindowConsole();
		}

		private void HandleOnExceptionOccur()
		{
			DCSettings settings = DCSettings.Instance;
			if (settings.showOnException)
			{
				ShowWindowConsole();
			}
		}
		
		private void ShowWindowConsole()
		{
			if (WindowConsole.isVisible)
			{
				return;
			}

            if(UIWindowMgr.Instance == null)
            {
                LOG.Error("UIWindowMgr is not found!!!");
                return;
            }
			
			UIWindowMgr.Instance.PopUpWindow(UIWindow.Id.Console, false);
		}
	}
}

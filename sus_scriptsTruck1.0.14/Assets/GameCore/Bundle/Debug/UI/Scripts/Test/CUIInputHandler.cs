
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework
{
	public class CUIInputHandler : CUIComponent, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler
	{
		private const int c_maxTouchAmount = 5;

		protected int m_instanceID;

		private static int[] s_touchedHandlerID = new int[5];

		public override void Initialize(CUIForm formScript)
		{
			if (!base.m_isInitialized)
			{
				m_instanceID = base.GetInstanceID();
				base.Initialize(formScript);
				base.m_isInitialized = true;
			}
		}

		public virtual void OnPointerDown(PointerEventData eventData)
		{
			RecordTouchHandlerID(eventData, m_instanceID);
		}

		public virtual void OnPointerUp(PointerEventData eventData)
		{
			ClearTouchHanderID(eventData);
		}

		protected static void RecordTouchHandlerID(PointerEventData eventData, int instanceID)
		{
			int pointerId = eventData.pointerId;
			if (pointerId >= 0 && pointerId < c_maxTouchAmount)
			{
				s_touchedHandlerID[pointerId] = instanceID;
			}
		}

		protected static void ClearTouchHanderID(PointerEventData eventData)
		{
			int pointerId = eventData.pointerId;
			if (pointerId >= 0 && pointerId < c_maxTouchAmount)
			{
				s_touchedHandlerID[pointerId] = 0;
			}
		}

		protected static void RefreshTouchHanderID()
		{
			Touch[] touches = Input.touches;
			int num = 0;
			for (int i = 0; i < touches.Length; i++)
			{
				int fingerId = touches[i].fingerId;
				if (fingerId >= 0 && fingerId < c_maxTouchAmount)
				{
					num |= 1 << (fingerId & 0x1F);
				}
			}
			for (int j = 0; j < c_maxTouchAmount; j++)
			{
				if ((num & 1 << j) == 0)
				{
					s_touchedHandlerID[j] = 0;
				}
			}
		}

		protected static int GetTouchHandlerID(int touchID)
		{
			if (touchID >= 0 && touchID < c_maxTouchAmount)
			{
				return s_touchedHandlerID[touchID];
			}
			return 0;
		}

		protected static bool GetTouch(int instanceID, out Touch touch)
		{
			for (int i = 0; i < c_maxTouchAmount; i++)
			{
				if (s_touchedHandlerID[i] == instanceID)
				{
					Touch[] touches = Input.touches;
					for (int j = 0; j < touches.Length; j++)
					{
						if (touches[j].fingerId == i)
						{
							touch = touches[j];
							return true;
						}
					}
				}
			}
			touch = default(Touch);
			return false;
		}
	}
}

using Framework;

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Tiinoo.DeviceConsole;

namespace Tiinoo.DeviceConsole
{
	public class Demo : MonoBehaviour
	{
		public Text uiOperationTips;
		public Text uiStackTraceTips;

		private static string KEY_COLOR = "#f44336ff";
		private int m_idCounter = 0;
		private bool m_isWatch;

		void Start()
		{
			InitOperationTips();
			InitStackTraceTips();
		}

		private void InitOperationTips()
		{
			DCSettings settings = DCSettings.Instance;
			string tips = null;
			string operation = null;

			bool useTouch = PlatformUtil.IsMobilePlatform();

			if (useTouch)
			{
				string touch = "";
				DCSettings.Gesture gesture = settings.openWithTouch;
				switch (gesture)
				{
				case DCSettings.Gesture.SWIPE_DOWN_WITH_ONE_FINGER:
					touch = "1 finger";
					break;
					
				case DCSettings.Gesture.SWIPE_DOWN_WITH_TWO_FINGERS:
					touch = "2 fingers";
					break;
					
				case DCSettings.Gesture.SWIPE_DOWN_WITH_THREE_FINGERS:
					touch = "3 fingers";
					break;
				}
				operation = string.Format("Swipe down with <color={0}>{1}</color>", KEY_COLOR, touch);
			}
			else
			{
				KeyCode key = settings.openWithKey;
				operation = string.Format("Press key <color={0}>{1}</color>", KEY_COLOR, key.ToString());
			}
			
			if (operation != null)
			{
				tips = string.Format("{0} to open the console.", operation);
			}
			
			if (tips != null)
			{
				uiOperationTips.text = tips;
			}
		}

		private void InitStackTraceTips()
		{
			string tips = string.Format("Recommend you use a <color={0}>development build</color> to view the stack trace.", KEY_COLOR);
			uiStackTraceTips.text = tips;
		}

		public void SetWatch(bool isWatch)
		{
			m_isWatch = isWatch;
		}

		public void LogInfo()
		{
			m_idCounter++;
			string s = string.Format("id = {0} This is an info!", m_idCounter);
			if (m_isWatch)
			{
				DCWatcher.Log(s);
			}
			else
			{
				LOG.Info(s);
			}
		}
		
		public void LogWarning()
		{
			m_idCounter++;
			string s = string.Format("id = {0} This is a warning!", m_idCounter);
			if (m_isWatch)
			{
				DCWatcher.LogWarning(s);
			}
			else
			{
				LOG.Warn(s);
			}
		}
		
		public void LogError()
		{
			m_idCounter++;
			string s = string.Format("id = {0} This is an error!", m_idCounter);
			if (m_isWatch)
			{
				DCWatcher.LogError(s);
			}
			else
			{
				LOG.Error(s);
			}
		}

		public void LogException()
		{
			m_idCounter++;
			Exception ex = new Exception();
			LOG.Error(ex);	// Watcher.LogException() is not supported.
		}
		
		public void RandLog100()
		{
			for (int i = 0; i < 100; i++)
			{
				int rand = UnityEngine.Random.Range(0, 3);
				switch (rand)
				{
				case 0:
					LogInfo();
					break;
					
				case 1:
					LogWarning();
					break;
					
				default:
					LogError();
					break;
				}
			}
		}
	}
}

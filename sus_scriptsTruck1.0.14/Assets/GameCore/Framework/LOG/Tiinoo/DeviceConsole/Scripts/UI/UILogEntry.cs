using Framework;

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Tiinoo.DeviceConsole;
using UberLogger;

namespace Tiinoo.DeviceConsole
{
	public class UILogEntry : MonoBehaviour, IItemView
	{
		#region inspector
		public Image uiBg;
		public Image uiLogType;
		public Text uiLogMessage;
		#endregion
		
		private LogInfo m_log;

		public void Refresh(object item, bool evenRow, bool isSelected)
		{
            LogInfo log = item as LogInfo;
			if (log == null)
			{
				return;
			}

			m_log = log;
			uiLogType.sprite = GetLogTypeSprite(m_log.logType);
			uiLogMessage.text = m_log.GetSimpleMessage();
			uiBg.color = GetBgColor(evenRow, isSelected);
		}
		
		public Color GetBgColor(bool evenRow, bool isSelected)
		{
			WindowConsole console = WindowConsole.Instance;
			Color color;
			if (isSelected)
			{
				color = console.bgLogSelected;
			}
			else
			{
                if(m_log.isException)
                {
                    color = Color.red;
                }
                else
                {
                    color = evenRow ? console.bgLogDark : console.bgLogLight;
                }
			}
			return color;
		}
		
		public static Sprite GetLogTypeSprite(LogType logType)
		{
			Sprite sprite = null;
			
			switch (logType)
			{
			case LogType.Log:
				sprite = WindowConsole.Instance.iconInfo;
				break;
				
			case LogType.Warning:
				sprite = WindowConsole.Instance.iconWarning;
				break;
				
			case LogType.Error:
			case LogType.Assert:
				sprite = WindowConsole.Instance.iconError;
				break;
			case LogType.Exception:
                    sprite = WindowConsole.Instance.iconException;
                    break;
				
			default:
				sprite = WindowConsole.Instance.iconInfo;
				break;
			}
			
			return sprite;
		}
	}
}



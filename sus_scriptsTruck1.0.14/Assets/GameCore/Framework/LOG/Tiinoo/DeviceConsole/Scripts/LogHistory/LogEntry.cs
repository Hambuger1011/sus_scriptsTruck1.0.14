using Framework;

//using UnityEngine;
//using System.Collections;
//using System;

//namespace Tiinoo.DeviceConsole
//{
//	public class LogEntry
//	{
//		private const int SHORT_MESSAGE_MAX_LENGTH = 128;

//		public LogType logType;
//		public string message;
//		public string stackTrace;

//		public bool isWatched;
//		public string lowerMessage;		// message in lowercase
//		public string timeStamp;

//		private string m_shortMessage;
		
//		public LogEntry(LogType type, string logString, string logStackTrace)
//		{
//			logType = type;

//			bool hasWatchFlag = false;
//			message = (logString != null) ? logString : "";
//			message = DCWatcher.RemoveWatchFlagIfHas(message, ref hasWatchFlag);
//			lowerMessage = message.ToLower();

//			isWatched = hasWatchFlag;
//			stackTrace = (logStackTrace != null) ? logStackTrace : "";

//			timeStamp = string.Format("{0:HH:mm:ss.ffff}", DateTime.Now);
//		}

//		public string ShortMessage
//		{
//			get
//			{
//				if (m_shortMessage != null)
//				{
//					return m_shortMessage;
//				}

//				if (string.IsNullOrEmpty(message))
//				{
//					m_shortMessage = "";
//					return m_shortMessage;
//				}

//				string[] strs = message.Split('\n');
//				m_shortMessage = strs[0];
//				if (m_shortMessage.Length > SHORT_MESSAGE_MAX_LENGTH)
//				{
//					m_shortMessage = m_shortMessage.Substring(0, SHORT_MESSAGE_MAX_LENGTH);
//				}
//				return m_shortMessage;
//			}
//		}

//		public override string ToString ()
//		{
//			return message;
//		}
//	}
//}


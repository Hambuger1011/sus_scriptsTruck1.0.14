using Framework;

using System;
using UnityEngine;
using System.Collections;
using System.Diagnostics;
using UberLogger;

namespace Tiinoo.DeviceConsole
{
	public static class LogHandler
	{
        public static System.Action<LogInfo> onLogAdded;
		public static System.Action onExceptionOccur;

		public static void LogCallback(LogInfo logInfo)
		{
            if (onLogAdded != null)
            {
                onLogAdded(logInfo);
            }

            if (logInfo.logType == LogType.Exception)
			{
				if (onExceptionOccur != null)
				{
					onExceptionOccur();
				}
			}
		}
	}
}


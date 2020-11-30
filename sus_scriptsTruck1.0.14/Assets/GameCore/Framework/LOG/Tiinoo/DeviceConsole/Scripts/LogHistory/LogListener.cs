using Framework;

using UnityEngine;
using System.Collections;
using System;
using UberLogger;

namespace Tiinoo.DeviceConsole
{
	public class LogListener : UberLogger.ILogger
    {
        public void Log(LogInfo logInfo)
        {

            //if (!GameUtility.IsMainThread && CTimerManager.HasInstance())
            //{
            //    CTimerManager.Instance.AddTimer(0, 1, (int t) =>
            //    {
            //        LogHandler.LogCallback(logInfo);
            //    });
            //}
            //else
            {
                LogHandler.LogCallback(logInfo);
            }
        }
	}
}

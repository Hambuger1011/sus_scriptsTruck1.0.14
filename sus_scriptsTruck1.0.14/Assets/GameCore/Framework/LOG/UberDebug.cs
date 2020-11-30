#if !ENABLE_UBERLOGGING && (DEVELOPMENT_BUILD || DEBUG || UNITY_EDITOR || ENABLE_DEBUG)
#define ENABLE_UBERLOGGING
#endif

using System;
using System.Diagnostics;
using IGG.SDK.Core.Error;
using IGGUtils;
using UberLogger;

//Helper functions to make logging easier
public static class LOG
{
     [StackTraceIgnore]
    static public void Log(UnityEngine.Object context, object message, params object[] par)
    {
        #if ENABLE_UBERLOGGING
        UberLogger.Logger.Log("", context, LogSeverity.Message, message, par);
        #endif
    }

     [StackTraceIgnore]
    static public void Info( object message, params object[] par)
    {
        #if ENABLE_UBERLOGGING
        UberLogger.Logger.Log("", null, LogSeverity.Message, message, par);
        #endif
    }

     [StackTraceIgnore]
    static public void Log(object message, params object[] par)
    {
        #if ENABLE_UBERLOGGING
        UberLogger.Logger.Log("", null, LogSeverity.Message, message, par);
        #endif
    }

     [StackTraceIgnore]
    static public void LogChannel(UnityEngine.Object context, string channel, object message, params object[] par)
    {
        #if ENABLE_UBERLOGGING
        UberLogger.Logger.Log(channel, context, LogSeverity.Message, message, par);
        #endif
    }

     [StackTraceIgnore]
    static public void LogChannel(string channel, object message, params object[] par)
    {
        #if ENABLE_UBERLOGGING
        UberLogger.Logger.Log(channel, null, LogSeverity.Message, message, par);
        #endif
    }


     [StackTraceIgnore]
    static public void Warn(UnityEngine.Object context, object message, params object[] par)
    {
        #if (ENABLE_UBERLOGGING || ENABLE_UBERLOGGING_WARNINGS)
        UberLogger.Logger.Log("", context, LogSeverity.Warning, message, par);
        #endif
    }

     [StackTraceIgnore]
    static public void Warn(object message, params object[] par)
    {
        #if (ENABLE_UBERLOGGING || ENABLE_UBERLOGGING_WARNINGS)
        UberLogger.Logger.Log("", null, LogSeverity.Warning, message, par);
        #endif
    }

     [StackTraceIgnore]
    static public void WarnChannel(UnityEngine.Object context, string channel, object message, params object[] par)
    {
        #if (ENABLE_UBERLOGGING || ENABLE_UBERLOGGING_WARNINGS)
        UberLogger.Logger.Log(channel, context, LogSeverity.Warning, message, par);
        #endif
    }

     [StackTraceIgnore]
    static public void WarnChannel(string channel, object message, params object[] par)
    {
        #if (ENABLE_UBERLOGGING || ENABLE_UBERLOGGING_WARNINGS)
        UberLogger.Logger.Log(channel, null, LogSeverity.Warning, message, par);
        #endif
    }

     [StackTraceIgnore]
    static public void Error(UnityEngine.Object context, object message, params object[] par)
    {
        #if (ENABLE_UBERLOGGING || ENABLE_UBERLOGGING_ERRORS)
        UberLogger.Logger.Log("", context, LogSeverity.Error, message, par);
        #endif
    }

     [StackTraceIgnore]
    static public void Error(object message, params object[] par)
    {
        #if (ENABLE_UBERLOGGING || ENABLE_UBERLOGGING_ERRORS)
        UberLogger.Logger.Log("", null, LogSeverity.Error, message, par);
        #endif
    }

     [StackTraceIgnore]
    static public void ErrorChannel(UnityEngine.Object context, string channel, object message, params object[] par)
    {
        #if (ENABLE_UBERLOGGING || ENABLE_UBERLOGGING_ERRORS)
        UberLogger.Logger.Log(channel, context, LogSeverity.Error, message, par);
        #endif
    }

     [StackTraceIgnore]
    static public void ErrorChannel(string channel, object message, params object[] par)
    {
        #if (ENABLE_UBERLOGGING || ENABLE_UBERLOGGING_ERRORS)
        UberLogger.Logger.Log(channel, null, LogSeverity.Error, message, par);
        #endif
    }


    //Logs that will not be caught by UberLogger
    //Useful for debugging UberLogger
     [LogUnityOnly]
    static public void UnityLog(object message)
    {
        #if ENABLE_UBERLOGGING
        UnityEngine.Debug.Log(message);
        #endif
    }

     [LogUnityOnly]
    static public void UnityWarn(object message)
    {
        #if (ENABLE_UBERLOGGING || ENABLE_UBERLOGGING_WARNINGS)
        UnityEngine.Debug.LogWarning(message);
        #endif
    }

     [LogUnityOnly]
    static public void UnityLogError(object message)
    {
        #if (ENABLE_UBERLOGGING || ENABLE_UBERLOGGING_ERRORS)
        UnityEngine.Debug.LogError(message);
        #endif
    }
    
    [StackTraceIgnore, Conditional("ENABLE_DEBUG")]
    public static void Assert(bool InCondition, string InFormat, params object[] InParameters)
    {
        if (!InCondition)
        {
            try
            {
                string text = null;
                if (!string.IsNullOrEmpty(InFormat))
                {
                    try
                    {
                        if (InParameters != null)
                        {
                            text = string.Format(InFormat, InParameters);
                        }
                        else
                        {
                            text = InFormat;
                        }
                    }
                    catch (Exception ex)
                    {
                        LOG.Error(ex);
                    }
                }
                else
                {
                    text = string.Format(" no assert detail, stacktrace is :{0}", Environment.StackTrace);
                }
                if (text != null)
                {
                    string str = "Assert failed! " + text;
                    LOG.Error(str);
                }
                else
                {
                    LOG.Error("Assert failed!");
                }
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
            }
        }
    }
    

    [StackTraceIgnore, Conditional("ENABLE_DEBUG")]
    public static void Assert(bool InCondition)
    {
        if (!InCondition)
        {
            try
            {
                string text = string.Format("Assert failed! , stacktrace is :{0}", Environment.StackTrace);
                LOG.Error(text);
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
            }
        }
    }
    

    public static void ShowIGGException(IGGError error)
    {
        ShowIGGException("操作失败", error);
    }
    public static void ShowIGGException(string message, IGGError error)
    {
        Error(message + "(Error: " + error.GetCode() + ")");
    }
}

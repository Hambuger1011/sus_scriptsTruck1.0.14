namespace IGG.SDK.Utils.Common
{
    /// <summary>
    /// USDKDemo日志打印工具类。
    /// </summary>
    public class IGGLog
    {
        private static LEVEL sLevel = LEVEL.DEBUG;

        public enum LEVEL
        {
            NONE = 99,
            DEBUG = 1,
            WARNING = 2,
            ERROR = 3,
        }

        public static void SetLevel(LEVEL level)
        {
            sLevel = level;
        }

        public static void Debug(string message)
        {
            if ((int)sLevel <= (int)LEVEL.DEBUG)
            {
                UnityEngine.Debug.Log(message);
            }
        }

        public static void Debug(string message, params object[] args)
        {
            if ((int)sLevel <= (int)LEVEL.DEBUG)
            {
                UnityEngine.Debug.LogFormat(message, args);
            }
        }
        
        public static void Warning(string message)
        {
            if ((int)sLevel <= (int)LEVEL.WARNING)
            {
                UnityEngine.Debug.LogWarning(message);
            }
        }

        public static void Warning(string message, params object[] args)
        {
            if ((int)sLevel <= (int)LEVEL.WARNING)
            {
                UnityEngine.Debug.LogWarningFormat(message, args);
            }
        }

        public static void Error(string message)
        {
            if ((int)sLevel <= (int)LEVEL.ERROR)
            {
                UnityEngine.Debug.LogError(message);
            }
        }

        public static void Error(string message, params object[] args)
        {
            if ((int)sLevel >= (int)LEVEL.ERROR)
            {
                UnityEngine.Debug.LogErrorFormat(message, args);
            }
        }
    }
}


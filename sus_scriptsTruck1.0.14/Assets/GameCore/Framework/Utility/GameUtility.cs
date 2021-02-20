#define USE_WEB_ASSETBUNDLE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Net;
using System;
using System.Net.Sockets;
using AB;
using System.Reflection;

public static class GameUtility
{

#if CHANNEL_SPAIN
    public static string version = "1.1.13";
    public static int buildNum = 130;

#else
    public static string version = "1.3.1";
    public static int buildNum = 40;
    public static int resVersion = 1;
#endif


    public static string WritablePath { get; private set; }
    public static string ReadonlyPath { get; private set; }
    public static bool isEditorMode { get; private set; }

#if UNITY_STANDALONE
    public static readonly string OS = "Standalone";
#elif UNITY_IOS
    public static readonly string OS = "iOS";
#elif UNITY_ANDROID
    public static readonly string OS = "Android";
#else
    public static readonly string OS = "unkown";
#endif



#if CHANNEL_HUAWEI
    public static readonly string ChannelID = "Huawei";
#elif CHANNEL_SPAIN
    public static readonly string ChannelID = "Spain";
#else
    public static readonly string ChannelID = "Onyx";
#endif


#if USE_SERVER_DATA
    public static readonly bool useServerData = true;
#else
    public static readonly bool useServerData = false;
#endif

    public static bool LoginSuccFlag = false;
    public static float LoginConsumeTime = 0;

    static bool _isDebugMode = false;
    public static bool isDebugMode
    {
        get
        {
            return _isDebugMode;
        }
    }


    static float aspectRatio = -1.0f;
    static bool mIpadAspectRatio = false;       //类似ipad的宽高比

    //屏幕分辨比
    public static float AspectRatio()
    {
        if (aspectRatio < 0)
        {
            aspectRatio = Screen.width / (Screen.height * 1.0f);
            LOG.Info("--AspectRatio-->" + aspectRatio);
        }
        return aspectRatio;
    }

    //是否为ipad屏（6:8）这样
    public static bool IpadAspectRatio()
    {
        mIpadAspectRatio = AspectRatio() > 0.60;
        return mIpadAspectRatio;
    }

    //是否为长屏（1080:2280）
    public static bool IsLongScreen()
    {
        mIpadAspectRatio = AspectRatio() < 0.53;
        return mIpadAspectRatio;
    }


//    private static string mModelStr = "";
//    private static bool mIsIphoneXDevice = false;
//    public static int IphoneXTopH = 64;
//    public static bool IsIphoneXDevice()
//    {
//        if (string.IsNullOrEmpty(mModelStr))
//        {
//            mModelStr = SystemInfo.deviceModel;
//#if UNITY_IOS
//            // iPhoneX:"iPhone10,3","iPhone10,6"  iPhoneXR:"iPhone11,8"  iPhoneXS:"iPhone11,2"  iPhoneXS Max:"iPhone11,6"
//            mIsIphoneXDevice = mModelStr.Equals("iPhone10,3") || mModelStr.Equals("iPhone10,6") || mModelStr.Equals("iPhone11,8") || mModelStr.Equals("iPhone11,2") || mModelStr.Equals("iPhone11,6");
//#endif
//        }
//        return mIsIphoneXDevice;
//    }

    public static string GetLocalIP()
    {
        try
        {
            string HostName = Dns.GetHostName(); //得到主机名
            IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
            for (int i = 0; i < IpEntry.AddressList.Length; i++)
            {
                //从IP地址列表中筛选出IPv4类型的IP地址
                //AddressFamily.InterNetwork表示此IP为IPv4,
                //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    return IpEntry.AddressList[i].ToString();
                }
            }
            return "";
        }
        catch (Exception ex)
        {
            LOG.Error("获取本机IP出错:" + ex.Message);
            return "";
        }
    }

    public static string GetPlayerIPAdress()
    {
#if UNITY_2018_2_OR_NEWER
        //Use Unity Multiplayer and NetworkIdentity instead
        string ipAdress = GetLocalIP();
#else
        string ipAdress = UnityEngine.Network.player.ipAddress;
#endif
        LOG.Info("====Player IP Adress===>" + ipAdress);
        return ipAdress;
    }

    public static int mainThreadID
    {
        get;
        private set;
    }

    public static bool IsMainThread
    {
        get { return System.Threading.Thread.CurrentThread.ManagedThreadId == GameUtility.mainThreadID; }
    }

    static GameUtility()
    {
        Init();
    }

    [UnityEngine.RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void OnGameStartUp()
    {
        Init();
    }

    public static void Init()
    {
        if(ReadonlyPath != null)
        {
            return;
        }
        ReadonlyPath = Application.streamingAssetsPath + "/";
        GetDebugState(ref _isDebugMode);
        isEditorMode = false;
        //writablePath
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.LinuxEditor:
                {
                    isEditorMode = true;
                    if (!Directory.Exists(ReadonlyPath))
                    {
                        Directory.CreateDirectory(ReadonlyPath);
                    }

                    WritablePath = System.Environment.CurrentDirectory.Replace("\\","/") + "/.data/";
                }
                break;
            case RuntimePlatform.Android:
            case RuntimePlatform.IPhonePlayer:
                {
                    WritablePath = Application.persistentDataPath + "/";
                }
                break;
            default:
                {
                    WritablePath = Application.dataPath + "/.data/";
                }
                break;
        }
        if (!Directory.Exists(WritablePath))
        {
            Directory.CreateDirectory(WritablePath);
        }

        mainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        // UberLogger.Logger.Initialize();
    }

    public static string Platform
    {
        get
        {
#if UNITY_IOS
           return "ios";
#elif UNITY_ANDROID

#if CHANNEL_HUAWEI
            return "hwAndroid";
#endif
            return "android";
#else
            return "android";
#endif
#if 静默模式下都是pc
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return "android";
                case RuntimePlatform.IPhonePlayer:
                    return "ios";
                default:
                    return "pc";
            }
#endif
        }
    }


    public static string GetPath(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return path;
    }

    public static string NormalizerDir(string dir)
    {
        dir = Regex.Replace(dir, @"\\+|/+", Path.AltDirectorySeparatorChar.ToString());
        return dir;
    }

    [Conditional("ENABLE_DEBUG")]
    public static void GetDebugState(ref bool bEnable)
    {
        bEnable = true;
    }


    public static string FormatBytes(float size)
    {
        bool isPositive = (size >= 0);
        if (!isPositive)
        {
            size = -size;
        }
        string[] units = new string[] { "B", "KB", "MB", "GB", "TB", "PB" };
        float mod = 1024.0f;
        int i = 0;
        while (size >= mod)
        {
            size /= mod;
            i++;
        }

        if (isPositive)
        {
            return string.Format("{0:F2}{1}", size, units[i]);
        }
        else
        {
            return string.Format("-{0:F2}{1}", size, units[i]);
        }
    }
    public static void _SetActive(this GameObject obj, bool bActive)
    {
        if (obj != null && obj.activeSelf != bActive)
        {
            obj.SetActive(bActive);
        }
    }

    public static int CalculateThreadCount(enThreadCount count)
    {
#if UNITY_WEBGL
		return 0;
#else
        if (count == enThreadCount.AutomaticLowLoad || count == enThreadCount.AutomaticHighLoad)
        {
#if ASTARDEBUG
			LOG.Info(SystemInfo.systemMemorySize + " " + SystemInfo.processorCount + " " + SystemInfo.processorType);
#endif

            int logicalCores = Mathf.Max(1, SystemInfo.processorCount);
            int memory = SystemInfo.systemMemorySize;

            if (memory <= 0)
            {
                LOG.Error("Machine reporting that is has <= 0 bytes of RAM. This is definitely not true, assuming 1 GiB");
                memory = 1024;
            }

            if (logicalCores <= 1) return 0;

            if (memory <= 512) return 0;

            if (count == enThreadCount.AutomaticHighLoad)
            {
                if (memory <= 1024) logicalCores = System.Math.Min(logicalCores, 2);
            }
            else {
                //Always run at at most processorCount-1 threads (one core reserved for unity thread).
                // Many computers use hyperthreading, so dividing by two is used to remove the hyperthreading cores, pathfinding
                // doesn't scale well past the number of physical cores anyway
                logicalCores /= 2;
                logicalCores = Mathf.Max(1, logicalCores);

                if (memory <= 1024) logicalCores = System.Math.Min(logicalCores, 2);

                logicalCores = System.Math.Min(logicalCores, 6);
            }

            return logicalCores;
        }
        else {
            int val = (int)count;
            return val;
        }
#endif
    }



#region 游戏结束截图
    public static Texture2D Snapshoot(Camera camera, int w, int h)
    {
        RenderTexture temporary = RenderTexture.GetTemporary(w, h, 24);
        Texture2D texture2D = new Texture2D(w, h, TextureFormat.ARGB32, false);
        RenderTexture renderTexture = camera.targetTexture;
        camera.targetTexture = temporary;
        camera.Render();
        camera.targetTexture = renderTexture;

        // 激活这个rt, 并从中中读取像素。  
        renderTexture = RenderTexture.active;
        RenderTexture.active = temporary;
        texture2D.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        texture2D.Apply();
        RenderTexture.active = renderTexture;
        RenderTexture.ReleaseTemporary(temporary);
        return texture2D;
    }
    #endregion

    

    public static void SetGameViewScale()
    {
#if UNITY_EDITOR
        var type = Type.GetType("UnityEditor.GameView,UnityEditor.dll");

        //var gameView = (GameView)WindowLayout.FindEditorWindowOfType(typeof(GameView));
        //var p = type.GetProperty("targetSize", BindingFlags.Instance | BindingFlags.NonPublic);

        var method = type.GetMethod("GetMainGameView", BindingFlags.NonPublic | BindingFlags.Static);
        object gameView = method.Invoke(null, null);
        var fieldInfo = type.GetField("m_ZoomArea", BindingFlags.Instance | BindingFlags.NonPublic);
        var o = fieldInfo.GetValue(gameView);
        method = o.GetType().GetMethod("SetTransform", BindingFlags.Instance | BindingFlags.Public);
        method.Invoke(o, new object[] { Vector2.zero, new Vector2(0.1f,0.1f) });
#endif
    }

}

public enum enThreadCount
{
    AutomaticLowLoad = -1,
    AutomaticHighLoad = -2,
    None = 0,
    One = 1,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight
}

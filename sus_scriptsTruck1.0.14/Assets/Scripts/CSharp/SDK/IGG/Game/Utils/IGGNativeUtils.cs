//#define IGGSDK_EDITOR_ANDROID // 为了编码的方便在这边定义了该宏
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using static Constants;

/// <summary>
/// 平台相关功能（打开系统默认浏览器、弹Toast、获取Google账号的token）的Wrapper。
/// </summary>
public class IGGNativeUtils
{
    private static IGGNativeUtils _instance = null;
    public static IGGNativeUtils ShareInstance()
    {
        if (_instance == null)
        {
            _instance = new IGGNativeUtils();
        }
        return _instance;
    }
#if UNITY_ANDROID
    private static AndroidJavaClass mIGGNativeUtilsClass;
    static IGGNativeUtils()
    {
        mIGGNativeUtilsClass = new AndroidJavaClass("com.igg.sdk.unity.IGGNativeUtils");
    }
    public class TokenListener : AndroidJavaProxy
    {
        public delegate void Listener(bool succeed, string token);
        public TokenListener(Listener callback)
            : base("com.igg.sdk.unity.IGGNativeUtils$GoogleAccountTokenListener")
        {
            m_callback = callback;
        }

        void onComplete(bool succeed, string token)
        {
            if (m_callback != null)
            {
                GlobalMonoBehaviour.ShareInstance().RunInMainThread(() => {  m_callback(succeed, token); });
            }
        }
        private Listener m_callback;
    }

    /**
     * 获取Google账号的token（新接口方式）。
     */
    public void FetchGooglePlayToken(TokenListener.Listener listener)
    {
        mIGGNativeUtilsClass.CallStatic("fetchGoogleToken", new TokenListener(listener));
    }

    /**
     * 获取Google账号的token（旧接口方式）。
     */
    public void FetchGooglePlayTokenLegacy(TokenListener.Listener listener)
    {
        mIGGNativeUtilsClass.CallStatic("fetchGoogleTokenLegacy", new TokenListener(listener));
    }

    /**
     * 打开系统默认浏览器。
     */
    public void OpenBrowser(string url)
    {
        mIGGNativeUtilsClass.CallStatic("openBrowser", url);
    }

    /**
     * 显示Toast。
     */
    public static void ShowToast(string message)
    {
        Debug.Log("ShowToast:" + message);
        mIGGNativeUtilsClass.CallStatic("showToast", message);
    }
   
#elif UNITY_IOS
    public class TokenListener
    {
        public delegate void Listener(bool succeed, string token);
    }

    public void FetchGooglePlayToken(TokenListener.Listener listener)
    {
        listener(true, IGGDefault.GoogleplayToken);
    }

    public void FetchGooglePlayTokenLegacy(TokenListener.Listener listener)
    {
        listener(true, IGGDefault.GoogleplayToken);
    }

    /**
     * 显示Toast。
     */
    public static void ShowToast(string message)
    {
        Debug.Log("ShowToast:" + message);
        ShowIOSToast(message, "", "ok", null);
    }
    public class MsgBoxReturnListener
    {
        public delegate void Listener(bool bSure);
        public delegate void IOSListener(bool bSure);
        [AOT.MonoPInvokeCallback(typeof(IOSListener))]
        public static void onComplete(bool bSure)
        {
            if (m_callback != null)
            {
               GlobalMonoBehaviour.ShareInstance().RunInMainThread(() => { m_callback(bSure); });
            }
        }
        public static Listener m_callback;
    }

    [DllImport("__Internal")]
    private static extern void IGGNativeUtils_ShowToast(string message, string title, string ok, MsgBoxReturnListener.IOSListener listener);
    public static void ShowIOSToast(String message, String title, String ok, MsgBoxReturnListener.Listener listener = null)
    {
        MsgBoxReturnListener.m_callback = listener;
        IGGNativeUtils_ShowToast(message, title, ok, MsgBoxReturnListener.onComplete);
    }
    [DllImport("__Internal")]
    private static extern void IGGNativeUtils_OpenBrowser(string url);

    /**
     * 打开系统默认浏览器。
     */
    public void OpenBrowser(string url)
    {
        IGGNativeUtils_OpenBrowser(url);
    }

#else
    public class TokenListener
    {
        public delegate void Listener(bool succeed, string token);
    }

    public void FetchGooglePlayToken(TokenListener.Listener listener)
    {
        listener(true, IGGDefault.GoogleplayToken);
    }

    public void FetchGooglePlayTokenLegacy(TokenListener.Listener listener)
    {
        listener(true, IGGDefault.GoogleplayToken);
    }

    public static void ShowToast(string message)
    {
        Debug.Log(message);
    }

    public void OpenBrowser(string url)
    {
        Debug.Log("OpenBrowser:" + url);
        Application.OpenURL(url);
    }
#endif
}
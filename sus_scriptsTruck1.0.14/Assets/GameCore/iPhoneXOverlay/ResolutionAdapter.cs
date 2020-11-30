using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

public class ResolutionAdapter
{

#if UNITY_EDITOR
    private const string _SAFE_AREA_ON_KEY = "EditorSafeAreaEmulation";

    public static bool IsEditorSafeAreaEmulationOn
    {
        get
        {
            return UnityEditor.EditorPrefs.GetBool(_SAFE_AREA_ON_KEY);
        }
        private set
        {
            UnityEditor.EditorPrefs.SetBool(_SAFE_AREA_ON_KEY, value);
        }
    }

    const string Menu_Safe = "Tools/ResolutionAdapter/切换刘海";
    [UnityEditor.MenuItem(Menu_Safe)]
    private static void ToggleEditorSafeZoneMode()
    {
        IsEditorSafeAreaEmulationOn = !IsEditorSafeAreaEmulationOn;
        UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
    }

    [UnityEditor.MenuItem(Menu_Safe, true)]
    public static bool ToggleEditorSafeZoneModeValidate()
    {
        UnityEditor.Menu.SetChecked(Menu_Safe, IsEditorSafeAreaEmulationOn);
        return true;
    }

#endif
    /*
        iPhone X 横持手机方向:
        分辨率
        2436 x 1125 px
        safe area
        2172 x 1062 px
        左右边距分别
        132px
        底边距 (有Home条)
        63px
        顶边距
        0px
        */

    /// <summary>
    /// 是否有刘海
    /// </summary>
    public static bool HasUnSafeArea
    {
        get
        {
            var safeArea = GetSafeArea();
            if (IsLandscape)
            {
                return GetSafeArea().x > 0;
            }
            else
            {
                return GetSafeArea().y > 0;
            }
        }
    }

    private static Rect _safeArea = new Rect(0,0,-1,-1);
    public static Vector2 androidNotchSize = new Vector2(0, 0);
    public static bool androidisSafeArea = false;

#if UNITY_IOS || UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void GetSafeArea(out float x, out float y, out float width, out float height);

    private static Rect GetiOSSafeArea()
    {
        float x, y, width, height = 0f;
        GetSafeArea(out x, out y, out width, out height);
        Rect iosSafeArea;
        if (IsLandscape)
        {
            iosSafeArea = new Rect(x, 0, width, height);
        }
        else
        {
            iosSafeArea = new Rect(0, y, width, height);
        }
        return iosSafeArea;
    }
#endif

    /// <summary>
    /// 获取安全区域
    /// </summary>
    public static Rect GetSafeArea()
    {
#if UNITY_EDITOR
        if (IsEditorSafeAreaEmulationOn)
        {
            var size = GetScreenSize();
            const int h = 97;
            // return fake safe area rect
            if (IsLandscape)
            {
                return new Rect(h, 0, size.x - h - h, size.y);
            }
            else
            {
                return new Rect(0, h, size.x, size.y - h - h);
            }
        }else{
            return Screen.safeArea;
        }
#endif
        if(_safeArea.width <= 0)
        {
            if (GameUtility.OS == "Android")
            {
                var size = GetScreenSize();
                // return fake safe area rect
                if (IsLandscape)
                {
                    _safeArea = new Rect(androidNotchSize.x, 0, size.x - androidNotchSize.x * 2, size.y);
                }
                else
                {
                    _safeArea = new Rect(0, androidNotchSize.y, size.x, size.y - androidNotchSize.y * 2);
                }
            }
            else if (GameUtility.OS == "iOS")
            {
#if UNITY_IOS || UNITY_EDITOR
                _safeArea = GetiOSSafeArea();
#endif
            }else
            {
                _safeArea = Screen.safeArea;
            }
        }
        return _safeArea;
    }


    public static Vector2 GetScreenSize()
    {
#if UNITY_EDITOR
        string[] res = UnityEditor.UnityStats.screenRes.Split('x');
        int width = int.Parse(res[0]);
        int height = int.Parse(res[1]);

        return new Vector2(width, height);
#endif
        //return new Vector2(Display.main.systemWidth, Display.main.systemHeight);
        return new Vector2(GameSettings.DefaultScreenWidth, GameSettings.DefaultScreenHeight);
    }

    /// <summary>
    /// 是否横屏
    /// </summary>
    public static bool IsLandscape
    {
        get
        {
            var size = GetScreenSize();
            return size.x > size.y;
        }
    }
    public static void setAndroidNotchSize(Vector2 size)
    {
        // ResolutionAdapter.androidNotchSize = size;
        // ResolutionAdapter._safeArea = new Rect(0, 0, -1, -1);
    }


    private static float _keyboardHeight = -1;
    public static float GetKeyboardHeight()
    {
#if UNITY_EDITOR
        return TouchScreenKeyboard.area.y;
        // return 294;
#endif
        //if (_keyboardHeight <= 0)
        {
            if (GameUtility.OS == "Android")
            {
                //using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                //{
                //    AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").
                //        Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");

                //    using (AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect"))
                //    {
                //        View.Call("getWindowVisibleDisplayFrame", Rct);
                //        _keyboardHeight = Display.main.systemHeight - Rct.Call<int>("height");
                //    }
                //}

                //using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                //{
                //    var unityPlayer = unityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer");
                //    using (var view = unityPlayer.Call<AndroidJavaObject>("getView"))
                //    {
                //        var decorHeight = 0;
                //        //mSoftInputDialog被混淆成b了
                //        using (var softInputDialog = unityPlayer.Get<AndroidJavaObject>("b"))
                //        {
                //            if (softInputDialog != null)//includeInput
                //            {
                //                using (var decorView = softInputDialog.Call<AndroidJavaObject>("getWindow").Call<AndroidJavaObject>("getDecorView"))
                //                {
                //                    if (decorView != null)
                //                    {
                //                        decorHeight = decorView.Call<int>("getHeight");
                //                    }
                //                }
                //            }else
                //            {
                //                LOG.Error("找不到softInputDialog");
                //            }
                //        }
                //        using (var rect = new AndroidJavaObject("android.graphics.Rect"))
                //        {
                //            view.Call("getWindowVisibleDisplayFrame", rect);
                //            _keyboardHeight = Display.main.systemHeight - rect.Call<int>("height") + decorHeight;
                //        }

                //    }
                //}
                _keyboardHeight = AndroidUtils.GetKeyboardHeight();
            }
            else if (GameUtility.OS == "iOS")
            {
#if UNITY_IOS || UNITY_EDITOR
                var area = TouchScreenKeyboard.area;
                _keyboardHeight = area.y + area.height;
#endif
            }
            else
            {
                _keyboardHeight = TouchScreenKeyboard.area.y;
            }
        }
        return _keyboardHeight;
    }
}

using Framework;

namespace Framework
{
    using UnityEngine;
    using System.Collections;
    using System;
    using System.Collections.Generic;
#if S03
using SDK;
#endif
    using System.Diagnostics;

    public static class AndroidUtils
    {
        static AndroidUtils()
        {
            Init();
        }

        static bool bInit = false;
        [Conditional("UNITY_ANDROID")]
        public static void Init()
        {
            if (bInit)
            {
                return;
            }
            bInit = true;
#if UNITY_EDITOR
            bool bInEditor = true;
            if (bInEditor)
            {
                return;
            }
#endif
            //AndroidHelper.CallStaticMethod("com.game.gamelib.utils.AndroidUtils", "AddReqPermEvent", "OnReqPermCallback", new JAction.Three<int, string,int>(OnReqPermCallback));
            // AndroidHelper.CallStaticMethod("com.game.gamelib.utils.AndroidUtils", "AddRequestPermissionsResult", new JAction.Three<int, string, int>(OnRequestPermissionsResult));
            // AndroidHelper.CallStaticMethod("com.game.gamelib.utils.AndroidUtils", "AddKeyDownNotify", new JFunc.One<bool, int>(OnAddKeyDownNotify));
            // AndroidHelper.CallStaticMethod("com.game.gamelib.utils.AndroidUtils", "OnApplicationPause", new JAction.One<bool>(OnApplicationPause));
        }

        #region Android事件监听
        private static void OnRequestPermissionsResult(int requestCode, string permissions, int grantResults)
        {
            CTimerManager.Instance.AddTimer(0, 1, (_seq) =>
              {
                  LOG.Error("requestCode = " + requestCode + " permissions = " + permissions + " grantResults =" + grantResults);
              //for (int i = 0; i < permissions.Length; ++i)
              //{
              //    var p = permissions[i];
              //    var code = grantResults[i];
              //    LOG.Error(p + " " + code);
              //}
          });
        }



        /// <summary>
        /// 手机按钮回调
        /// </summary>
        private static bool OnAddKeyDownNotify(int code)
        {
            bool bUse = false;
            switch (code)
            {
                case AndroidKeyCode.KEYCODE_BACK:
                    {
                        LOG.Error("拦截android返回键");
                        CTimerManager.Instance.AddTimer(0, 1, (_seq) =>
                          {
#if S03
                          SdkMgr.Instance.Logout();
#endif
                      });
                        bUse = true;
                    }
                    break;
            }
            return bUse;
        }

        //static void OnReqPermCallback(int requestCode, string permission, int resultCode)
        //{
        //    LOG.Info(requestCode + " " + permission + " " + resultCode);
        //    CTimerManager.Instance.AddTimer(0, 1, (_seq) =>
        //    {
        //        EventRouter.Instance.BroadCastEvent<int, string, int>(EventID.ON_ANDROID_PERMISSION_REQ, requestCode, permission, resultCode);
        //    });
        //}



        private static void OnApplicationPause(bool pauseStatus)
        {
            CTimerManager.Instance.AddTimer(0, 1, (_seq) =>
            {
                LOG.Error("进入后台:" + pauseStatus);
                SdkMgr.Instance.OnApplicationPause(pauseStatus);
                if(!pauseStatus)
                {
					GameFramework.Instance.OnAndroidApplicationWillEnterForeground("");
                }
            });
        }
        #endregion

        [Conditional("UNITY_ANDROID")]
        public static void CopyAssets(string src, string dst)
        {
#if UNITY_EDITOR
            bool bInEditor = true;
            if (bInEditor)
            {
                return;
            }
#endif
            // AndroidHelper.CallStaticMethod("com.game.gamelib.io.FileUtils", "CopyAssets", new[] { src, dst });
        }

        [Conditional("UNITY_ANDROID")]
        public static void CopyAssetsSync(string src, string dst, AndroidJavaRunnable callback)
        {
#if UNITY_EDITOR
            bool bInEditor = true;
            if (bInEditor)
            {
                return;
            }
#endif
            // AndroidHelper.CallStaticMethod("com.game.gamelib.io.FileUtils", "CopyAssetsSync", new object[] { src, dst, callback });
        }


        /// <summary>
        /// 重启app
        /// </summary>
        [Conditional("UNITY_ANDROID")]
        public static void Reboot()
        {
#if UNITY_EDITOR
            bool bInEditor = true;
            if (bInEditor)
            {
                return;
            }
#endif
            // AndroidHelper.CallStaticMethod("com.game.gamelib.utils.AndroidUtils", "Reboot");
        }

        [Conditional("UNITY_ANDROID")]
        public static void Reboot2(int delayMS = 1000)
        {
            // AndroidHelper.CallStaticMethod("com.game.gamelib.utils.AndroidUtils", "Reboot2", delayMS);
        }

        [Conditional("UNITY_ANDROID")]
        public static void RequestFloatViewPermission()
        {
#if UNITY_EDITOR
            bool bInEditor = true;
            if (bInEditor)
            {
                return;
            }
#endif
            // AndroidHelper.CallStaticMethod("com.game.gamelib.utils.AndroidUtils", "RequestFloatViewPermission");
        }

        public const string READ_PHONE_STATE = "android.permission.READ_PHONE_STATE";
        public const string CAMERA = "android.permission.CAMERA";

        public static int CheckSelfPermission(string strPermissions)
        {
#if UNITY_EDITOR
            bool bInEditor = true;
            if (bInEditor)
            {
                return PackageManager.PERMISSION_GRANTED;
            }
#endif
            // int permissionCode = AndroidHelper.CallStaticMethod<int>("com.game.gamelib.utils.AndroidUtils", "CheckSelfPermission", strPermissions);
            return 0; //permissionCode;
        }

        [Conditional("UNITY_ANDROID")]
        public static void RequestPermissions(int reqPerCode, string strPermissions)
        {
#if UNITY_EDITOR
            bool bInEditor = true;
            if (bInEditor)
            {
                return;
            }
#endif
            //var permissions = "android.permission.READ_PHONE_STATE,android.permission.SYSTEM_ALERT_WINDOW";
            /*
            var res = AndroidHelper.CallStaticMethod<string>("com.game.gamelib.utils.AndroidUtils", "CheckSelfPermission", strPermissions);
            //var permissionsNeeded = res.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (res != null && res.Length > 0)
            {
                Alert.Show("亲，由于系统安全设置，有些权限需要确认", "设置", null, () =>
                   {
                       AndroidHelper.CallStaticMethod("com.game.gamelib.utils.AndroidUtils", "RequestPermissions", res, reqPerCode);
                   }, null);
            }else
            */
            {
                EventRouter.Instance.BroadCastEvent<int, string, int>(CEventID.ON_ANDROID_PERMISSION_REQ, reqPerCode, strPermissions, 0);
            }
            /*
            string READ_PHONE_STATE = "android.permission.READ_PHONE_STATE";
            string SYSTEM_ALERT_WINDOW = "android.permission.SYSTEM_ALERT_WINDOW";
            AndroidJavaObject unityIns = null;
            using (AndroidJavaClass unityPlayerCls = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                unityIns = unityPlayerCls.GetStatic<AndroidJavaObject>("currentActivity");
            }

            using (AndroidJavaClass activityCompatCls = new AndroidJavaClass("android.support.v4.app.ActivityCompat"))
            {
                List<string> permissionsNeeded = new List<string>();
                //读取手机状态权限
                var permCode = activityCompatCls.CallStatic<int>("checkSelfPermission", READ_PHONE_STATE);
                if(permCode != 0)
                {
                    permissionsNeeded.Add(READ_PHONE_STATE);
                }

                //读取悬浮窗口权限
                permCode = activityCompatCls.CallStatic<int>("checkSelfPermission", SYSTEM_ALERT_WINDOW);
                if (permCode != 0)
                {
                    permissionsNeeded.Add(SYSTEM_ALERT_WINDOW);
                }

                if(permissionsNeeded.Count > 0)
                {
                    activityCompatCls.CallStatic<int>("requestPermissions", unityIns, permissionsNeeded,0);
                }
            }
            */
        }

        [Conditional("UNITY_ANDROID")]
        public static void SaveImageToGallery(string fileName, string description, Action<string> callback)
        {
#if UNITY_EDITOR
            bool bInEditor = true;
            if (bInEditor)
            {
                return;
            }
#endif
            // AndroidHelper.CallStaticMethod("com.game.gamelib.utils.AndroidUtils", "SaveImageToGallery", fileName, description, new JAction.One<string>(callback));
        }

        [Conditional("UNITY_ANDROID")]
        public static void CloseSplash()
        {
#if UNITY_EDITOR
            bool bInEditor = true;
            if (bInEditor)
            {
                return;
            }
#endif
            // AndroidHelper.CallStaticMethod("com.game.gamelib.utils.AndroidUtils", "CloseSplash");
        }

        public static string ReadtPhoneState()
        {
            //AndroidHelper.CallStaticMethod("com.game.gamelib.utils.AndroidUtils", "AddRequestPermissionsResult", new JAction.Three<int, string[], int[]>(OnRequestPermissionsResult));
            // string json = AndroidHelper.CallStaticMethod<string>("com.game.gamelib.utils.AndroidUtils", "ReadtPhoneState");
            return "";
        }
        public class AndroidKeyCode
        {
            public const int KEYCODE_BACK = 4;
        }

        public static bool isAppActivityPause()
        {
#if UNITY_EDITOR || !UNITY_ANDROID
            return true;
#endif
            //return AndroidHelper.CallStaticMethod<bool>("com.game.gamelib.utils.AndroidUtils", "isAppActivityPause");
            return false;
        }

        [Conditional("UNITY_ANDROID")]
        public static void ShowDialog(String strTitle, String strMsg, String positiveButtonTitle, JAction.Zero onOkClick)
        {
            // AndroidHelper.CallStaticMethod("com.game.gamelib.utils.AndroidUtils", "ShowDialog"
            //     , strTitle
            //     , strMsg
            //     , positiveButtonTitle
            //     , onOkClick
            //     );
        }

        //https://github.com/smarxpan/NotchScreenTool
        [Conditional("UNITY_ANDROID")]
        public static void ShowFullScreen()
        {
#if UNITY_EDITOR
            bool bInEditor = true;
            if (bInEditor)
            {
                return;
            }
#endif
            // AndroidHelper.CallStaticMethod("com.game.gamelib.utils.AndroidUtils", "ShowFullScreen", new JAction.Three<bool,int,int>((isNotchEnable, notchWidth, notchHeight)=>
            // {
            //     ResolutionAdapter.setAndroidNotchSize(new Vector2(notchWidth, notchHeight));
            // }));
        }

        [Conditional("UNITY_ANDROID")]
        public static void HideUnitySoftKeyboard()
        {
            // AndroidHelper.CallStaticMethod("com.game.gamelib.utils.AndroidUtils", "HideUnitySoftKeyboard");
        }
        public static float GetKeyboardHeight()
        {
            // float h = AndroidHelper.CallStaticMethod<int>("com.game.gamelib.utils.AndroidUtils", "GetKeyboardHeight");
            //h = Display.main.systemHeight - h -  ResolutionAdapter.GetSafeArea().y;
            return 0f;
        }
    }
}

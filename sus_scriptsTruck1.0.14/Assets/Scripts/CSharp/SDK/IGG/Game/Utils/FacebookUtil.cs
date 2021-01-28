using System.Collections.Generic;
using Facebook.Unity;
using IGG.SDK;
using UnityEngine;

namespace IGGUtils
{
    /// <summary>
    /// 辅助获取FB账号token的工具类。
    /// </summary>
    public class FacebookUtil
    {
        private static OnSuccess successDelegate;
        private static OnFailed failedDelegate;
        
        public static void Init()
        {
            if (!FB.IsInitialized)
            {
                Debug.Log("facebook Init");
                // Initialize the Facebook SDK
                FB.Init(InitCallback, OnHideUnity);
            }
            else
            {
                // Already initialized, signal an app activation App Event
                FB.ActivateApp();
            }
        }

        private static void InitCallback()
        {
            if (FB.IsInitialized)
            {
                // Signal an app activation App Event
                FB.ActivateApp();
                Debug.Log("facebook ActivateApp");
                // Continue with Facebook SDK
            }
            else
            {
                Debug.LogError("Failed to Initialize the Facebook SDK");
            }
        }

        private static void OnHideUnity(bool isGameShown)
        {
            Debug.Log("facebook OnHideUnity");
            if (!isGameShown)
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }
        
        /// <summary>
        /// 执行FB登录
        /// </summary>
        /// <param name="success"></param>
        /// <param name="failed"></param>
        public static void Login(OnSuccess success, OnFailed failed)
        {
            successDelegate = success;
            failedDelegate = failed;
            if (FB.IsInitialized)
            {
                var perms = new List<string>() { "public_profile", "email" };
                FB.LogInWithReadPermissions(perms, AuthCallback);
                Debug.Log("facebook Login");
            }
            else
            {
                Debug.LogError("Facebook sdk is not Initialized!");
            }
        }

        /// <summary>
        /// 登录结果回调
        /// </summary>
        /// <param name="result"></param>
        private static void AuthCallback(ILoginResult result)
        {
            //IGGSDK.sharedInstance().getSDKMonoBehaviour().Add
            if (FB.IsLoggedIn)
            {
                // AccessToken class will have session details
                var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
                Debug.Log("Facebook token:" + aToken.TokenString);
                successDelegate?.Invoke(aToken.TokenString);
                FB.LogOut();
            }
            else
            {
                Debug.Log("User cancelled login");
                failedDelegate?.Invoke();
            }
        }

        public delegate void OnSuccess(string token);
        public delegate void OnFailed();
    }
}
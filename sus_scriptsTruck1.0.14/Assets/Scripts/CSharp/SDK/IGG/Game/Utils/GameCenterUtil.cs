using System.Collections.Generic;
using System.Runtime.InteropServices;
using IGG.SDK;
using IGG.SDK.Foundation.Thread;
using IGG.SDK.Framework;
using UnityEngine;

namespace IGGUtils
{
    /// <summary>
    /// 辅助获取GameCenter账号token的工具类。
    /// </summary>
    public class GameCenterUtil
    {
        private const string TAG = "GameCenterUtil";

        private static Dictionary<string, GameCenterLoginResultListener> gameCenterLoginResultListeners = new Dictionary<string, GameCenterLoginResultListener>();

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void GameCenterProxy_login(string target, GameCenterLoginResultHandler gameCenterLoginResultHandler);

        public delegate void GameCenterLoginResultHandler(string target, string token);
        [AOT.MonoPInvokeCallback(typeof(GameCenterLoginResultHandler))]
        private static void GameCenterLoginResultHandlerImpl(string target, string token)
        {
            Runnable action = delegate () {
                Debug.Log(TAG + "GameCenterLoginResultHandlerImpl target:" + target);
                Debug.Log(TAG + "GameCenterLoginResultHandlerImpl token:" + token);

                GameCenterLoginResultListener gameCenterLoginResultListener = null;
                if (gameCenterLoginResultListeners.ContainsKey(target))
                {
                    gameCenterLoginResultListener = gameCenterLoginResultListeners[target];
                    if (gameCenterLoginResultListener != null)
                    {
                        gameCenterLoginResultListener(token);
                        // 处理完回调，清除监听。
                        gameCenterLoginResultListeners.Remove(target);
                    }
                }
            };

            GlobalMonoBehaviour.ShareInstance().AddRunnable(action);
        }
#endif

        private static OnSuccess successDelegate;
        private static OnFailed failedDelegate;
     
        /// <summary>
        /// GC登录
        /// </summary>
        /// <param name="success"></param>
        /// <param name="failed"></param>
        public static void Login(OnSuccess success, OnFailed failed)
        {
            successDelegate = success;
            failedDelegate = failed;

            GameCenterLoginResultListener gameCenterLoginResultListener = delegate (string token)
            {
                if (StringHelper.IsEmpty(token))
                {
                    failedDelegate?.Invoke();
                }
                else
                {
                    successDelegate?.Invoke(token);
                }
            };

            string target = gameCenterLoginResultListener.GetHashCode().ToString();
            Debug.Log(TAG + "purchaseFlow target:" + target);

            gameCenterLoginResultListeners.Add(target, gameCenterLoginResultListener);
#if UNITY_IOS
            GameCenterProxy_login(target, GameCenterLoginResultHandlerImpl);
#endif
        }

        public delegate void GameCenterLoginResultListener(string token);
        public delegate void OnSuccess(string token);
        public delegate void OnFailed();
    }
}
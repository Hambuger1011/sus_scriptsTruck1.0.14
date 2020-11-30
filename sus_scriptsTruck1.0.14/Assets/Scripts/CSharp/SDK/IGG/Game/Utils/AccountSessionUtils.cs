using System;
using System.Collections.Generic;
using IGG.SDK;
using IGG.SDK.Service;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace IGGUtils
{
    /// <summary>
    /// 模拟在其他设备登录，检测 Session 过期服务类，研发无需调用。
    /// 登录后记录 Session 和检测 Session 是否过期功能
    /// </summary>
    public static class AccountSessionUtils
    {

        private const string TAG = "AccountSessionUtils";

        /// <summary>
        /// 模拟登录。
        /// </summary>
        /// <param name="IGGID"></param>
        /// <param name="accessKey"></param>
        /// <param name="listener"></param>
        public static void SetToken(string IGGID, string accessKey, SetTokenResultListener listener)
        {
            try
            {
                new IGGGameAccountService().SimulatorLogin(IGGID, accessKey, delegate (bool isInvalid) {
                    listener?.Invoke(isInvalid);
                });

            } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// 模拟检测当前session是否有效。
        /// </summary>
        /// <param name="IGGID"></param>
        /// <param name="accessKey"></param>
        /// <param name="listener"></param>
        public static void CheckToken(string IGGID, string accessKey, CheckTokenResultListener listener)
        {
            try
            {
                new IGGGameAccountService().SimulatorCheck(IGGID, accessKey, delegate (bool isInvalid) {
                    listener?.Invoke(isInvalid);
                });
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        public delegate void CheckTokenResultListener(bool isInvalid);

        public delegate void SetTokenResultListener(bool isInvalid);
    }
}
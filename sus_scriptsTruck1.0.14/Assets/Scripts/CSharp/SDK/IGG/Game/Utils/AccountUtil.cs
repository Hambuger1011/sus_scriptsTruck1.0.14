using IGG.SDK;
using IGG.SDK.Modules.Account;
using IGG.SDK.Modules.Account.VO;
using UnityEngine;

namespace IGGUtils
{
    /// <summary>
    /// 会员业务相关的辅助工具类。
    /// </summary>
    public class AccountUtil
    {
        public const string GUEST = "Device";
        public const string GOOGLE = "Google";
        public const string FACEBOOK = "Facebook";
        public const string IGG_ACCOUNT = "IGGAccount";
        public const string GAMECENTER = "GameCenter";
        public const string APPLE = "Apple";

        /// <summary>
        /// 将登录类型的枚举值转为字符串值（用于显示）。
        /// </summary>
        /// <param name="loginType"></param>
        /// <returns></returns>
        public static string GetLoginTypeValue(IGGLoginType loginType)
        {
            string value = "";
            if (loginType == IGGLoginType.Facebook)
            {
                value += FACEBOOK;
            }
            else if (loginType == IGGLoginType.GooglePlus)
            {
                value += GOOGLE;
            }
            else if (loginType == IGGLoginType.Guest)
            {
                value += GUEST;
            }
            else if (loginType == IGGLoginType.IGGAccount)
            {
                value += IGG_ACCOUNT;
            }
            else if (loginType == IGGLoginType.GameCenter)
            {
                value += GAMECENTER;
            }
            else if (loginType == IGGLoginType.Apple)
            {
                value += APPLE;
            }
            else
            {
                value += GUEST;
            }
            return value;
        }

        /// <summary>
        /// 获取上一次的登录类型(一般在session过期或无效的时，进行重新登录需要用到)。
        /// </summary>
        /// <returns></returns>
        public static IGGLoginType ReadLastLoginType()
        {
            return IGGSession.currentSession.GetLoginType();
        }
    }
}
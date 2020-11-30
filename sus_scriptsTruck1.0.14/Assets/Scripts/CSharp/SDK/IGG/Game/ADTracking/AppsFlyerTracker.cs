using System.Collections.Generic;

namespace ADTracking
{
    /// <summary>
    /// AppsFlyer SDK 对接文档：
    /// 中文：https://support.appsflyer.com/hc/zh-cn/articles/213766183-AppsFlyer-SDK-%E5%AF%B9%E6%8E%A5-Unity
    /// 英文：https://support.appsflyer.com/hc/en-us/articles/213766183-Unity-SDK-integration-for-developers
    ///
    /// 导入AppsFlyer SDK  的unitypackage包，会在 Assets->Plugins文件夹底下增加相应的脚本文件（Demo中用到的版本是4.21.1）。
    ///
    /// 为了能验证事件的上传情况，请研发联系技术部QA配置一个归因渠道，技术部QA会发一个连接给研发，研发直接用浏览器打开就可以，Demo这边对应的连接为
    /// 安卓： https://app.appsflyer.com/com.igg.sdk.test?pid=test&c=demo-and-en-1130&advertising_id=05fe4a7e-9252-4605-9c5d-8a3966a4a571
    /// iOS: https://app.appsflyer.com/id824749543?pid=test&c=demo-ios-en-1640&idfa=6D1EA985-11DC-432B-8884-DD4E0AFEFCFC
    ///
    /// 测试环境配置请参考：http://wiki.skyunion.net/index.php?title=%E5%B9%BF%E5%91%8A%E7%9B%B8%E5%85%B3:AF%E6%B5%8B%E8%AF%95%26FAQ
    /// </summary>
    public class AppsFlyerTracker : IADTracker
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(string customerInfo)
        {
            AppsFlyer.setAppsFlyerKey ("WEYqZmRBi6ZmFww2esj28Y");  // AppsFlyerKey由技术部提供，研发有不懂的地方可以联系技术部相关人员。
            AppsFlyer.setIsDebug (true); // 研发发正式包请去掉debug模式
            AppsFlyer.setCustomerUserID(customerInfo); // 设置自定义信息（传当前游戏的gameid），这个必传，格式请参考调用Init的地方
#if UNITY_IOS
            AppsFlyer.setAppID ("1535571424");  // 由技术部提供，研发有不懂的地方可以联系技术部相关人员。
            AppsFlyer.trackAppLaunch (); // 启动事件跟踪（这边会上传安装等一些默认事件）
#elif UNITY_ANDROID
            AppsFlyer.setAppID ("com.igg.sdk.test"); // ！！！！请研发改成游戏对应的实际包名。
            AppsFlyer.init("WEYqZmRBi6ZmFww2esj28Y", "AppsFlyerTrackerCallbacks"); // AppsFlyerKey由技术部提供，研发有不懂的地方可以联系技术部相关人员。另外第二个参数固定是"AppsFlyerTrackerCallbacks"
#endif
        }

        /// <summary>
        /// 发送事件，研发可以在相应事件的触发点调用这个方法进行上传自定义事件。
        /// </summary>
        /// <param name="name">事件明</param>
        /// <param name="extraInfo">附带信息，具体要传而外信息，请参考wiki文档</param>
        public void Track(string name, Dictionary<string, object> extraInfos)
        {
            Dictionary<string, string> extraInfosTmp = new Dictionary<string, string>();

            foreach (var extraInfo in extraInfos)
            {
                extraInfosTmp.Add(extraInfo.Key, extraInfo.Value.ToString());
            }
            
            AppsFlyer.trackRichEvent(name, extraInfosTmp);
        }
    }
}
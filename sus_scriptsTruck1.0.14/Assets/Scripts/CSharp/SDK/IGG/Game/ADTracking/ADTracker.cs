using System.Collections.Generic;

namespace ADTracking
{
    /// <summary>
    /// 该广告跟踪实现类，主要用于跟踪一类标准广告事件(SIGN_UP、DAY2_RETENTION、IGG_LAUNCH)。
    ///
    /// 研发接入前，请详细阅读各文档说明（在文档导航页都能找到，2.13 表单项都能找到，http://wiki.skyunion.net/index.php?title=%E6%B8%B8%E6%88%8F%E6%8E%A5%E5%85%A5IGG%E5%B9%B3%E5%8F%B0%E7%9A%84%E6%96%87%E6%A1%A3%E5%AF%BC%E8%88%AA）。
    /// </summary>
    public class ADTracker : IADTracker
    {
        private List<IADTracker> trackers = new List<IADTracker>();
        
        /// <summary>
        /// 初始化各第三方平台广告跟踪器
        /// </summary>
        /// <param name="customerInfo"></param>
        public void Init(string customerInfo)
        {
            trackers.Add(new FirebaseTracker());
            trackers.Add(new FacebookTracker());

            foreach (var tracker in trackers)
            {
                tracker.Init(customerInfo);
            }
        }
        
        /// <summary>
        /// 事件跟踪
        /// </summary>
        /// <param name="name"></param>
        /// <param name="extraInfos"></param>
        public void Track(string name, Dictionary<string, object> extraInfos)
        {
            foreach (var tracker in trackers)
            {
                tracker.Track(name, extraInfos);
            }
        }
    }
}
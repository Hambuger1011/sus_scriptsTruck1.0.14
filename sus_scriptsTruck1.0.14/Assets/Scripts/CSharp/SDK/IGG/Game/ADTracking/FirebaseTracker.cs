using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using UnityEngine;

namespace ADTracking
{
    /// <summary>
    /// 1、接入文档：
    /// https://firebase.google.com/products/analytics/
    ///
    /// 2、导入FirebaseAnalytics.unitypackage （Demo这边用到的版本是6.0.0，具体是用dotnet4还是dotnet2，由研发自己使用，Demo这边使用的是dotnet4）
    ///
    /// 3、导入到工程里会多出以下目录 Editor Default Resources、Firebase、Parse，
    /// 并且在Assets->Plugin->Android与Assets->Plugin->iOS底下会多出firebase的目录。
    ///
    /// 4、配置好dll兼容的各平台情况，这个请参考Demo中的配置。
    ///
    /// 5、mainTemplate中对于firebase的依赖情况请参考Demo中的配置。
    ///
    /// 6、Android的Player Settings中请在Target Architectures中选上ARM64。
    ///
    /// 7、为了能验证iOS的接入情况，请配置两个地方
    ///  XcodeBuildPostBuild（参考Demo中的XcodeBuildPostBuild 191行, 这边主要为了ADhoc包能验证接入情况）
    ///  Xcode工程配置上以下命令行参数-FIRDebugEnabled（验证Debug包）：https://firebase.google.com/docs/analytics/debugview?hl=zh-CN
    ///
    /// Note: 兼容AndroidX的会在下个版本发布
    /// </summary>
    public class FirebaseTracker : IADTracker
    {
        private bool isAvailable = false;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(string customerInfo)
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    isAvailable = true;
                    FirebaseApp app = FirebaseApp.DefaultInstance;
                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                        "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                    isAvailable = false;
                }
            });

            // if (FirebaseApp.CheckDependencies() == DependencyStatus.Available)
            // {
            //     isAvailable = true;
            // }
            // else
            // {
            //     isAvailable = false;
            // }
        }

        public void GetAnalyticsInstanceIdAsync(Action<Task<string>> continuation)
        {
            FirebaseAnalytics.GetAnalyticsInstanceIdAsync().ContinueWithOnMainThread(continuation);
        }


        /// <summary>
        /// 发送事件
        /// </summary>
        /// <param name="name">事件明</param>
        /// <param name="extraInfo">附带信息，具体要传而外信息，请参考wiki文档</param>
        public void Track(string name, Dictionary<string, object> extraInfos)
        {
            if (isAvailable)
            {
                TrackInternal(name, extraInfos);
            }
            else
            {
                Debug.LogError("请检测手机上的谷歌服务是否正常。");
            }
        }

        public void TrackInternal(string name, Dictionary<string, object> extraInfos)
        {
            Parameter[] firebaseExtraInfos = new Parameter[extraInfos.Count];

            int index = 0;
            foreach (var extraInfo in extraInfos)
            {
                firebaseExtraInfos[index] = new Parameter(extraInfo.Key, extraInfo.Value.ToString());
                index++;
            }

            Debug.LogError($"FirebaseAnalytics:{name}");
            FirebaseAnalytics.LogEvent(name, firebaseExtraInfos);
        }
    }
}
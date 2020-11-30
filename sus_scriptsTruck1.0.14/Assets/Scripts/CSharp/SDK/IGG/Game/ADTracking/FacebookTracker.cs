using System;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;

namespace ADTracking
{
    /// <summary>
    /// 对接文档：
    /// https://developers.facebook.com/docs/app-events/unity/
    ///
    /// Demo当前使用SDK版本是7.17.2（unity） 研发那边尽可能使用最新的版本。
    ///
    /// 导入后相关脚本文件会放在Assets->FacebookSDK目录底下（避免跟firebase冲突，请研发删除Plugins->Android->libs底下的android support相关库:cardview）
    ///
    /// 另外请关闭PlayServicesResolver的使用
    ///
    /// 配置：
    /// 当研发把unitypackage包导入到Unity IDE后，Unity IDE的菜单栏中会多出Facebook菜单，请研发进入Facebook菜单
    /// 配置Facebook App Id与Client Token，这两个参数值都由技术部提供。
    ///
    /// </summary>
    public class FacebookTracker : IADTracker
    {
        /// <summary>
        /// 初始化，使用FB的事件发送前，必须要先初始化。
        /// </summary>
        public void Init(string customerInfo)
        {
            if (FB.IsInitialized) {
                Debug.Log("FB.IsInitialized");
                FB.ActivateApp();
            } else {
                //Handle FB.Init
                FB.Init( () => {
                    Debug.Log("FB.Init");
                    FB.ActivateApp();
                });
            }
        }

        /// <summary>
        /// 发送事件，研发可以在相应事件的触发点调用这个方法进行上传自定义事件。
        /// </summary>
        /// <param name="name">事件明</param>
        /// <param name="extraInfo">附带信息，具体要传而外信息，请参考wiki文档</param>
        public void Track(string name, Dictionary<string, object> extraInfos)
        {
            Debug.Log("FB.Track");
            FB.LogAppEvent(name, null, extraInfos);
        }
    }
}
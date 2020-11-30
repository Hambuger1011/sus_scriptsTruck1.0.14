using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using GoogleMobileAds.Api.Mediation.UnityAds;
using GoogleMobileAds.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GoogleAdmobAds : MonoBehaviour
{
    #region 单例
    private static GoogleAdmobAds _Instance;
    public static GoogleAdmobAds Instance
    {
        get
        {
            return _Instance;
        }
    }
    public void Awake()
    {
        _Instance = this as GoogleAdmobAds;
    }
    #endregion

    //激励视频广告  【活动页面】 广告位
    public ActivityRewardedAd acitityRewardedAd;
    //激励视频广告  【章节通关界面广告】 广告位
    public ChpterRewardedAd chapterRewardedAd;

    public void Start()
    {
        //         MobileAds.SetiOSAppPauseOnBackground(true);
        //
        //         List<String> deviceIds = new List<String>() { AdRequest.TestDeviceSimulator };
        //
        //         // Add some test device IDs (replace with your own device IDs).
        //         //添加一些测试设备ID（替换为您自己的设备ID）
        // #if UNITY_IPHONE
        //         deviceIds.Add("96e23e80653bb28980d3f40beb58915c");
        // #elif UNITY_ANDROID
        //         deviceIds.Add("75EF8D155528C04DACBBA6F36F433035");
        // #endif
        //
        //         // Configure TagForChildDirectedTreatment and test device IDs.
        //         //配置 TagForChildDirectedTreatment 并测试设备ID。
        //         RequestConfiguration requestConfiguration =
        //             new RequestConfiguration.Builder()
        //             .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
        //             .SetTestDeviceIds(deviceIds).build();
        //
        //         //设置配置
        //         MobileAds.SetRequestConfiguration(requestConfiguration);

        //【初始化 google Admob聚合广告】
        // Initialize the Google Mobile Ads SDK.
        // Debug.LogError("Initialize the Google Mobile Ads SDK");
        // MobileAds.Initialize(HandleInitCompleteAction);

        this.InitSDK();


        //  UnityAds.SetGDPRConsentMetaData(true);
       
    }


    public void InitSDK()
    {
        //【初始化 google Admob聚合广告】
        // Initialize the Google Mobile Ads SDK.
        Debug.Log("Initialize the Google Mobile Ads SDK");

        MobileAds.Initialize(initStatus =>
        {
            // //初始化成功
            Debug.Log("AdmobAdsManager Initialization complete");

            //【初始化 激励视频广告】
            // this.InitRewardedAd();

        });
    }

    private void HandleInitCompleteAction(InitializationStatus initstatus)
    {
        // Callbacks from GoogleMobileAds are not guaranteed to be called on
        // main thread.
        // In this example we use MobileAdsEventExecutor to schedule these calls on
        // the next Update() loop.


    
    }

    public void InitRewardedAd()
    {
        Debug.Log("RewardedAdManager Initialization start");

        //初始化 【活动页面】 广告位
        acitityRewardedAd = new ActivityRewardedAd();
        acitityRewardedAd.Initialize();
    
        // //初始化 【活动页面】 广告位
        // chapterRewardedAd = new ChpterRewardedAd();
        // chapterRewardedAd.Initialize();
    }


}

using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using GoogleMobileAds.Api.Mediation.UnityAds;
using GoogleMobileAds.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public enum EnumAds
{
    //活动页面
    Acitity,
    //章节通关界面
    Chapter
}



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
    public MyRewardedAd acitityRewardedAd;
    //激励视频广告  【章节通关界面广告】 广告位
    public MyRewardedAd chapterRewardedAd;

    public void Start()
    {
        this.InitSDK();
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
        });
    }

    private void HandleInitCompleteAction(InitializationStatus initstatus)
    {
        // Callbacks from GoogleMobileAds are not guaranteed to be called on
        // main thread.
        // In this example we use MobileAdsEventExecutor to schedule these calls on
        // the next Update() loop.
    }

    //【初始化 激励视频广告】
    public void InitRewardedAd()
    {
        Debug.Log("RewardedAdManager Initialization start");

        //初始化 【活动页面】 广告位
        acitityRewardedAd = new MyRewardedAd(EnumAds.Acitity,"ca-app-pub-9883228183528023/4970055849", "ca-app-pub-9883228183528023/7610507536");
        acitityRewardedAd.Initialize();

        //初始化 【章节通关界面广告】 广告位
        chapterRewardedAd = new MyRewardedAd(EnumAds.Chapter,"ca-app-pub-9883228183528023/9674737555", "ca-app-pub-9883228183528023/3096547452");
        chapterRewardedAd.Initialize();

    }


}

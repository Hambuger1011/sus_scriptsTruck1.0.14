﻿using System;
using GoogleMobileAds.Api;
using UnityEngine;


/// <summary>
/// Activity 界面 广告位;
/// </summary>
public class ChpterRewardedAd
{
    //激励视频广告   【章节通关界面广告】 广告位
    private RewardedAd chapter_rewardedAd;


    //【章节通关界面广告】广告位ID
     #if UNITY_EDITOR
             string chapter_adUnitId = "unused";
     #elif UNITY_ANDROID
             string chapter_adUnitId = "ca-app-pub-9883228183528023/9674737555";
     #elif UNITY_IPHONE
             string chapter_adUnitId = "ca-app-pub-9883228183528023/3096547452";
     #else
             string chapter_adUnitId = "unexpected_platform";
     #endif

    private Action CallBack;

    /// <summary>
    /// 初始化
    /// </summary>
    public void Initialize()
    {
        //激励视频广告  创建并加载
        this.chapter_rewardedAd = this.CreateAndLoadRewardedAd(chapter_adUnitId);
    }


    /// <summary>
    /// 创建并加载一个新的激励视频广告
    /// </summary>
    public RewardedAd CreateAndLoadRewardedAd(string adUnitId)
    {
        Debug.LogError("adUnitId:" + adUnitId);

        //创建一个新的 激励视频广告（新API版）
        // create new rewarded ad instance
        RewardedAd rewardedAd = new RewardedAd(adUnitId);

        // Called when an ad request has successfully loaded. 广告请求成功加载后调用。
        rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.   广告请求加载失败时调用。
        rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.  显示广告时调用。
        rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.  广告请求显示失败时调用。
        rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.  在与广告互动应获得奖励的情况下调用。
        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.  广告关闭时调用。
        rewardedAd.OnAdClosed += HandleRewardedAdClosed;


        // AdRequest adrequest = new AdRequest.Builder()
        //     .AddTestDevice(AdRequest.TestDeviceSimulator)
        //     .AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
        //     .AddKeyword("unity-admob-sample")
        //     .TagForChildDirectedTreatment(false)
        //     .AddExtra("color_bg", "9B30FF")
        //     .Build();


        AdRequest adrequest = new AdRequest.Builder().Build();

        // Create empty ad request
        rewardedAd.LoadAd(adrequest);

        //AF事件记录* 用户请求广告加载
        AppsFlyerManager.Instance.ADS_REQUEST();

        return rewardedAd;
    }
    //重试次数
    private int trycount = 3;

    /// <summary>
    /// // Called when an ad request has successfully loaded. 广告加载完成时被调用。
    /// </summary>
    private void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        Debug.LogError("HandleRewardedAdLoaded event received");
        //重置失败重试次数
        trycount = 3;
    }

    /// <summary>
    /// Called when an ad request failed to load.   在广告加载失败时被调用。提供的 AdErrorEventArgs 的 Message 属性用于描述发生了何种类型的失败。
    /// </summary>
    private void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        Debug.LogError("HandleRewardedAdFailedToLoad event received with message: " + args.Message);

        if (trycount > 0)
        {
            trycount--;
            //【活动页面】激励视频广告  创建并加载
            this.chapter_rewardedAd = this.CreateAndLoadRewardedAd(chapter_adUnitId);
        }
        else
        {
            Debug.LogError("广告加载失败超过3次！！ ");
        }
    }

    /// <summary>
    /// Called when an ad is shown.  显示广告时调用。
    /// 在广告开始展示并铺满设备屏幕时被调用。如需暂停应用音频输出或游戏循环，则非常适合使用此方法。
    /// </summary>
    private void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        //【活动页面】激励视频广告  创建并加载
        this.chapter_rewardedAd = this.CreateAndLoadRewardedAd(chapter_adUnitId);

        //AF事件记录*  用户播放广告
        AppsFlyerManager.Instance.ADS_PLAY();

        Debug.LogError("HandleRewardedAdOpening event received");
    }

    /// <summary>
    /// Called when an ad request failed to show.  广告请求显示失败时调用。
    /// 在广告显示失败时被调用。提供的 AdErrorEventArgs 的 Message 属性用于描述发生了何种类型的失败。
    /// </summary>
    private void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {

        Debug.LogError("HandleRewardedAdFailedToShow event received with message:" + args.Message);
    }

    ////
    /// 警告：强烈建议您不要在广告加载失败时尝试使用广告请求完成块加载新广告。
    /// 如果您必须使用广告请求完成块加载广告，请限制广告加载重试次数，以免在网络连接受限等情况下广告请求连续失败。
    ///

    /// <summary>
    /// Called when the user should be rewarded for interacting with the ad.  在与广告互动应获得奖励的情况下调用。
    /// 在用户因观看视频而应获得奖励时被调用。 Reward 参数描述了要呈现给用户的奖励。
    /// </summary>
    private void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;

        if (CallBack != null)
        {
            CallBack();
        }
        Debug.LogError("HandleUserEarnedReward event received for " + amount.ToString() + " " + type);
        //XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:Open(logic.uiid.UIRatinglevelForm);");
    }


    //////======================================================= 预加载广告
    //使用 OnAdClosed 预加载下一个激励广告
    //RewardedAd 是一次性对象。这意味着，在展示激励广告后，就不能再用该对象加载另一个广告了。要请求另一个激励广告，您需要创建新的 RewardedAd 对象。
    //最佳做法是在 OnAdClosed 广告事件中加载另一个激励广告，以便在上一个激励广告关闭后，立即开始加载下一个激励广告：
    //////=======================================================预加载广告

    /// <summary>
    /// Called when the ad is closed.  广告关闭时调用。
    /// 在用户点按“关闭”图标或使用“返回”按钮关闭激励视频广告时被调用。如果应用暂停了音频输出或游戏循环，则非常适合使用此方法恢复这些活动。
    /// </summary>
    private void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        Debug.LogError("HandleRewardedAdClosed  event received");
    }


    /// <summary>
    ///【活动页面】显示播放激励视频广告
    /// </summary>
    public void ShowRewardedAd_Chapter(Action vCallBack)
    {
        CallBack = vCallBack;
        if (this.chapter_rewardedAd.IsLoaded())
        {
            this.chapter_rewardedAd.Show();
        }
        else
        {
            Debug.LogError("ShowRewardedAd_Chapter 播放视频广告，视频没有加载完毕");
        }
    }
}




#if CHANNEL_ONYX
//using AnyThinkAds.Api;
//using AnyThinkAds.ThirdParty.MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[XLua.LuaCallCSharp, XLua.Hotfix]
public class OnyxAdsSdk : AdsSdkBase//, ATRewardedVideoListener
{
#if UNITY_IOS || UNITY_IPHONE
	static string mPlacementId_rewardvideo_all = "b5ed0dec95b9b9";//"b5b44a0f115321";
#else
    static string mPlacementId_rewardvideo_all = "b5ed0debeb900a";
#endif

    public OnyxAdsSdk()
    {
#if UNITY_EDITOR
        return;
#endif

        Debug.Log("Developer init video....unitid:" + mPlacementId_rewardvideo_all);
        //ATRewardedVideo.Instance.setListener(this);
        //ATRewardedVideo.Instance.addsetting(mPlacementId_rewardvideo_all, addsetting());
        LoadVideo();
    }


    void LoadVideo()
    {
#if UNITY_EDITOR
        return;
#endif
        Dictionary<string, string> jsonmap = new Dictionary<string, string>();
        jsonmap.Add("age", "22");
        jsonmap.Add("sex", "lady");
        jsonmap.Add("rv", "1");


        //ATRewardedVideo.Instance.loadVideoAd(mPlacementId_rewardvideo_all, jsonmap);
    }


    //单独适配平台属性
    private Dictionary<string, object> addsetting()
    {
        Dictionary<string, object> appsettinglist = new Dictionary<string, object>();

        //AdmobATRewardedVideoSetting
        //Dictionary<string, object> admobATRewardedVideoSetting = new Dictionary<string, object>();
        //appsettinglist.Add(AnyThinkAds.Api.ATConst.NETWORK_TYPE.NETWORK_ADMOB + "", Json.Serialize(admobATRewardedVideoSetting));

        ////mintegralATMediationSetting
        //Dictionary<string, object> mintegralATMediationSetting = new Dictionary<string, object>();
        //appsettinglist.Add(AnyThinkAds.Api.ATConst.NETWORK_TYPE.NETWORK_MINTEGRAL + "", Json.Serialize(mintegralATMediationSetting));

        ////_applovinATMediationSetting
        //Dictionary<string, object> _applovinATMediationSetting = new Dictionary<string, object>();
        //appsettinglist.Add(AnyThinkAds.Api.ATConst.NETWORK_TYPE.NETWORK_APPLOVIN + "", Json.Serialize(_applovinATMediationSetting));



        ////_flurryATMediationSetting
        //Dictionary<string, object> flurryATMediationSetting = new Dictionary<string, object>();
        //appsettinglist.Add(AnyThinkAds.Api.ATConst.NETWORK_TYPE.NETWORK_FLURRY + "", Json.Serialize(flurryATMediationSetting));


        ////_inmobiATMediationSetting
        //Dictionary<string, object> _inmobiATMediationSetting = new Dictionary<string, object>();
        //appsettinglist.Add(AnyThinkAds.Api.ATConst.NETWORK_TYPE.NETWORK_INMOBI + "", Json.Serialize(_inmobiATMediationSetting));


        ////_mopubATMediationSetting
        //Dictionary<string, object> _mopubATMediationSetting = new Dictionary<string, object>();
        //appsettinglist.Add(AnyThinkAds.Api.ATConst.NETWORK_TYPE.NETWORK_MOPUB + "", Json.Serialize(_mopubATMediationSetting));

        ////_chartboostATMediationSetting
        //Dictionary<string, object> _chartboostATMediationSetting = new Dictionary<string, object>();
        //appsettinglist.Add(AnyThinkAds.Api.ATConst.NETWORK_TYPE.NETWORK_CHARTBOOST + "", Json.Serialize(_chartboostATMediationSetting));

        ////_tapjoyATMediationSetting
        //Dictionary<string, object> _tapjoyATMediationSetting = new Dictionary<string, object>();
        //appsettinglist.Add(AnyThinkAds.Api.ATConst.NETWORK_TYPE.NETWORK_TAPJOY + "", Json.Serialize(_tapjoyATMediationSetting));

        ////_ironsourceATMediationSetting
        //Dictionary<string, object> _ironsourceATMediationSetting = new Dictionary<string, object>();
        //appsettinglist.Add(AnyThinkAds.Api.ATConst.NETWORK_TYPE.NETWORK_IRONSOURCE + "", Json.Serialize(_ironsourceATMediationSetting));

        ////_unityAdATMediationSetting
        //Dictionary<string, object> _unityAdATMediationSetting = new Dictionary<string, object>();
        //appsettinglist.Add(AnyThinkAds.Api.ATConst.NETWORK_TYPE.NETWORK_UNITYADS + "", Json.Serialize(_unityAdATMediationSetting));

        ////vungleRewardVideoSetting
        //Dictionary<string, object> vungleRewardVideoSetting = new Dictionary<string, object>();
        //vungleRewardVideoSetting.Add("orientation", 1);//1:2  1: 表示根据设备方向自动旋转  2:视频广告以最佳方向播放
        //vungleRewardVideoSetting.Add("isSoundEnable", true);//true:false
        //vungleRewardVideoSetting.Add("isBackButtonImmediatelyEnable", false);//true:false 如果为 true，用户可以立即使用返回按钮退出广告。如果为 false，在屏幕上的关闭按钮显示之前用户不可以使用返回按钮退出广告
        //appsettinglist.Add(AnyThinkAds.Api.ATConst.NETWORK_TYPE.NETWORK_VUNGLE + "", Json.Serialize(vungleRewardVideoSetting));


        ////adColonyRewardVideoSetting
        //Dictionary<string, object> adColonyRewardVideoSetting = new Dictionary<string, object>();

        //adColonyRewardVideoSetting.Add("enableConfirmationDialog", false);//true:false
        //adColonyRewardVideoSetting.Add("enableResultsDialog", false);//true:false
        //appsettinglist.Add(AnyThinkAds.Api.ATConst.NETWORK_TYPE.NETWORK_ADCOLONY + "", Json.Serialize(adColonyRewardVideoSetting));


        ////ttATRewardedVideoSetting
        //Dictionary<string, object> ttATRewardedVideoSetting = new Dictionary<string, object>();
        //ttATRewardedVideoSetting.Add("requirePermission", true);//是否申请权限
        //ttATRewardedVideoSetting.Add("orientation", 1);//可选参数 设置期望视频播放的方向
        //ttATRewardedVideoSetting.Add("supportDeepLink", true);//可选参数 设置是否支持deeplink
        //ttATRewardedVideoSetting.Add("rewardName", "金币");//可选参数 励视频奖励的名称，针对激励视频参数
        //ttATRewardedVideoSetting.Add("rewardCount", 1);//可选参数 激励视频奖励个数

        //appsettinglist.Add(AnyThinkAds.Api.ATConst.NETWORK_TYPE.NETWORK_TOUTIAO + "", Json.Serialize(ttATRewardedVideoSetting));



        return appsettinglist;
    }


    Action<bool> _onRewardBasedViedoComplete;
    public void ShowRewardBasedVideo(string tag, Action<bool> callback)
    {
        _onRewardBasedViedoComplete = callback;
#if UNITY_EDITOR
        bool isTest = true;
        if (isTest)
        {
            CallRewardBasedVideoEvent(true);
            return;
        }
#endif
        //if (isRewardedVideoAvailable())
        //{
        //    ATRewardedVideo.Instance.showAd(mPlacementId_rewardvideo_all);
        //}
        //else
        //{
        //    UITipsMgr.Instance.PopupTips("There's no video to watch yet", false);
        //    CallRewardBasedVideoEvent(false);
        //}
    }
    public bool isRewardedVideoAvailable()
    {

        // Debug.Log ("Developer isReady ?....");
        bool b = false;//ATRewardedVideo.Instance.hasAdReady(mPlacementId_rewardvideo_all);
        return b;
    }

    void CallRewardBasedVideoEvent(bool bSuc)
    {
        LoadVideo();
        if (_onRewardBasedViedoComplete != null)
        {
            var tmp = _onRewardBasedViedoComplete;
            _onRewardBasedViedoComplete = null;
            tmp(bSuc);
        }
    }



    #region 激励视频接口(这里的回调全是ndk子线程)

    //float m_timeScale = 1;
    //protected override void RewardedVideoAdOpenedEvent()
    //{
    //    base.RewardedVideoAdOpenedEvent();
    //    m_timeScale = Time.timeScale;
    //    Time.timeScale = 0;
    //}
    //protected override void RewardedVideoAdClosedEvent()
    //{
    //    base.RewardedVideoAdClosedEvent();
    //    Time.timeScale = m_timeScale;
    //    m_timeScale = 1;
    //}

    //protected override void RewardedVideoAdRewardedEvent(IronSourcePlacement ssp)
    //{
    //    base.RewardedVideoAdRewardedEvent(ssp);
    //    Debug.Log("unity-script: I got RewardedVideoAdRewardedEvent, amount = " + ssp.getRewardAmount() + " name = " + ssp.getRewardName());
    //    this.CallRewardBasedVideoEvent(ssp.getRewardAmount() > 0);
    //}


    //protected override void RewardedVideoAdShowFailedEvent(IronSourceError error)
    //{
    //    base.RewardedVideoAdShowFailedEvent(error);
    //    Time.timeScale = m_timeScale;
    //    m_timeScale = 1;
    //    Debug.Log("unity-script: I got RewardedVideoAdShowFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
    //    this.CallRewardBasedVideoEvent(false);
    //}


    public void onRewardedVideoAdLoaded(string unitId)
    {
        CTimerManager.Instance.AddTimer(0, 1, (_) =>
        {
            LOG.Info("onRewardedVideoAdLoaded:" + unitId);
        });
    }

    public void onRewardedVideoAdLoadFail(string unitId, string code, string message)
    {
        CTimerManager.Instance.AddTimer(5000, 1, (_) =>
        {
            LOG.Error("onRewardedVideoAdLoaded:" + unitId + ",code=" + code + ",msg=" + message);
            LoadVideo();
        });
    }

    //public void onRewardedVideoAdPlayStart(string unitId, ATCallbackInfo callbackInfo)
    //{
    //    CTimerManager.Instance.AddTimer(0, 1, (_) =>
    //    {
    //        LOG.Info("onRewardedVideoAdPlayStart:" + unitId);
    //    });
    //}

    //public void onRewardedVideoAdPlayEnd(string unitId, ATCallbackInfo callbackInfo)
    //{
    //    CTimerManager.Instance.AddTimer(0, 1, (_) =>
    //    {
    //        LOG.Info("onRewardedVideoAdPlayEnd:" + unitId);
    //    });
    //}

    //public void onRewardedVideoAdPlayFail(string unitId, string code, string message)
    //{
    //    CTimerManager.Instance.AddTimer(0, 1, (_) =>
    //    {
    //        LOG.Error("onRewardedVideoAdPlayFail:" + unitId + ",code=" + code + ",msg=" + message);
    //        this.CallRewardBasedVideoEvent(false);
    //    });
    //}

    //public void onRewardedVideoAdPlayClosed(string unitId, bool isReward, ATCallbackInfo callbackInfo)
    //{
    //    CTimerManager.Instance.AddTimer(0, 1, (_) =>
    //    {
    //        LOG.Info("onRewardedVideoAdPlayClosed:" + unitId);
    //        if (!isReward)
    //        {
    //            this.CallRewardBasedVideoEvent(false);
    //        }
    //    });
    //}

    //public void onRewardedVideoAdPlayClicked(string unitId, ATCallbackInfo callbackInfo)
    //{
    //    CTimerManager.Instance.AddTimer(0, 1, (_) =>
    //    {
    //        Debug.Log("onRewardedVideoAdPlayClicked:" + unitId);
    //    });
    //}

    //public void onReward(string unitId, ATCallbackInfo callbackInfo)
    //{
    //    CTimerManager.Instance.AddTimer(0, 1, (_) =>
    //    {
    //        LOG.Info("onReward:" + unitId);
    //        this.CallRewardBasedVideoEvent(true);
    //    });
    //}
    #endregion


}






[XLua.LuaCallCSharp]
public abstract class AdsSdkBase //: ATSDKInitListener
{
#if UNITY_IOS
    public static string appid = "a5ecf546be898f";
#else
    public static string appid = "a5ecf54bc549d8";
#endif
    public static string appKey = "12079614d955ae659c3a451aba301dd7";

    public static String REWARDED_INSTANCE_ID = "0";



    public AdsSdkBase()
    {
#if UNITY_EDITOR
        bool b = true;
        if (b)
        {
            return;
        }
#endif


        ////设置渠道的信息，开发者可以通过该渠道信息在后台来区分看各个渠道的广告数据（可选配置）
        //ATSDKAPI.setChannel(GameUtility.Platform);

        ////设置子渠道的信息，开发者可以通过该渠道信息在后台来区分看各个渠道的子渠道广告数据（可选配置）
        //ATSDKAPI.setSubChannel(GameUtility.version);

        ////设置自定义的Map信息，可匹配后台配置的广告商顺序的列表（App纬度）（可选配置）
        //ATSDKAPI.initCustomMap(new Dictionary<string, string> { { "unity3d_data", "test_data" } });

        ////设置自定义的Map信息，可匹配后台配置的广告商顺序的列表（Placement纬度）（可选配置）
        ////ATSDKAPI.setCustomDataForPlacementID(new Dictionary<string, string> { { "unity3d_data_pl", "test_data_pl" } }, placementId);

        ////设置开启Debug日志
        //ATSDKAPI.setLogDebug(GameUtility.isDebugMode);

        ////SDK的初始化（必须配置）
        //ATSDKAPI.initSDK(appid, appKey, this);//Use your own app_id & app_key here

        ////建议发布GDPR地区的开发者使用以下授权代码 （可选配置）
        //if (ATSDKAPI.isEUTraffic() && ATSDKAPI.getGDPRLevel() == 2)
        //    ATSDKAPI.showGDPRAuth();
    }

    public void Start()
    {

    }

    public void initFail(string message)
    {
        Debug.LogError("ads initFail:" + message);
    }

    public void initSuccess()
    {
        LOG.Info("ads init success");
    }

    public void OnApplicationPause(bool isPaused)
    {
    }

}
#endif
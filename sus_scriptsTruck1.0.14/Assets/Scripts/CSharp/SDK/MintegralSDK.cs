using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 广告视频插件
/// </summary>
public class MintegralSDK 
{
//    #if CHANNEL_HUAWEI
//    private int GDPR_ON = 1;
//    private const string RewardVideo = "RewardVideo";
//    private const string AdUnit = "80352";
//    private Action<bool> mCompleteCallBack;

//    private string[] _adTypeList = new string[]{
//		RewardVideo
//	};
		
//#if UNITY_ANDROID
//	private Dictionary<string, string[]> _adUnitDict = new Dictionary<string, string[]> () {
//		{ RewardVideo, new string[] { AdUnit} }
//	};
//#elif UNITY_IPHONE

//	private Dictionary<string, string[]> _adUnitDict = new Dictionary<string, string[]> () {
//		{ RewardVideo, new string[] { AdUnit} }
//	};
//#endif

//    public const string MTGSDKAppIDForAndroid = "112421";
//    public const string MTGSDKApiKeyForAndroid = "9598407730725e13134a0a78976daf57";
//#endif
//    public void Init()
//    {
//#if UNITY_EDITOR
//        return;
//#endif
//#if CHANNEL_HUAWEI
//        Mintegral.setConsentStatusInfoType(GDPR_ON);

//        this.mtgLog("userPrivateInfo ConsentStatus : " + Mintegral.getConsentStatusInfoType());


//#if UNITY_ANDROID

//        Mintegral.initMTGSDK(MTGSDKAppIDForAndroid, MTGSDKApiKeyForAndroid);
//#elif UNITY_IPHONE

//			Mintegral.initMTGSDK (MTGSDKAppIDForiOS,MTGSDKApiKeyForiOS);
//#endif


//        //Reward Video
//        MintegralManager.onRewardedVideoLoadSuccessEvent += onRewardedVideoLoadSuccessEvent;
//        MintegralManager.onRewardedVideoLoadedEvent += onRewardedVideoLoadedEvent;
//        MintegralManager.onRewardedVideoFailedEvent += onRewardedVideoFailedEvent;
//        MintegralManager.onRewardedVideoShownFailedEvent += onRewardedVideoShownFailedEvent;
//        MintegralManager.onRewardedVideoShownEvent += onRewardedVideoShownEvent;
//        MintegralManager.onRewardedVideoClickedEvent += onRewardedVideoClickedEvent;
//        MintegralManager.onRewardedVideoClosedEvent += onRewardedVideoClosedEvent;


//        LoadRewardVideo();
//#endif
//    }

//    public void CheckAds()
//    {
//#if UNITY_EDITOR
//        return;
//#endif
//#if CHANNEL_HUAWEI
//        if (Mintegral.isVideoReadyToPlay(AdUnit))
//        {
//            //LOG.Info("------Check Load Video---111-->");
//            SdkMgr.Instance.MingetralAdsInit = true;
//        }
//        else
//        {
//            //LOG.Info("------Check Load Video--222--->");
//            LoadRewardVideo();
//        }
//#endif
//    }

//    /// <summary>
//    /// 显示视频
//    /// </summary>
//    /// <param name="vCallBack"></param>
//    public void ShowAds(Action<bool> vCallBack)
//    {
//#if UNITY_EDITOR
//        return;
//#endif
//#if CHANNEL_HUAWEI
//        //LOG.Info("------showAds----->");
//        CheckAds();
//        if (!SdkMgr.Instance.MingetralAdsInit)
//        {
//            UITipsMgr.Instance.PopupTips("There's no video to watch yet", false);
//        }
//        else
//        {
//            if (Mintegral.isVideoReadyToPlay(AdUnit))
//            {
//                //LOG.Error("Mintegral: " + AdUnit + "\n" + "------------------------------");
//                mCompleteCallBack = vCallBack;
//                Mintegral.showRewardedVideo(AdUnit);
//            }
//        }
//#endif
//    }

//    private void LoadRewardVideo()
//    {
//        #if CHANNEL_HUAWEI
//        string[] rewardVideoUnits = _adUnitDict.ContainsKey(RewardVideo) ? _adUnitDict[RewardVideo] : null;
//        Mintegral.loadRewardedVideoPluginsForAdUnits(new string[] { AdUnit });
//#endif
//    }

//    void mtgLog(string log)
//    {

//        //LOG.Error("Mintegral: " + log + "\n" + "------------------------------");

//    }

//    #if CHANNEL_HUAWEI
//    //广告加载成功的回调
//    void onRewardedVideoLoadSuccessEvent(string adUnitId)
//    {
//        this.mtgLog("=============onRewardedVideoLoadSuccessEvent: " + adUnitId);
//    }
//    //广告缓存完成的回调
//    void onRewardedVideoLoadedEvent(string adUnitId)
//    {
//        this.mtgLog("==============onRewardedVideoLoadedEvent: " + adUnitId);
//    }
//    //广告加载失败的回调
//    void onRewardedVideoFailedEvent(string errorMsg)
//    {
//        this.mtgLog("================onRewardedVideoFailedEvent: " + errorMsg);
//    }
//    //广告展示失败的回调
//    void onRewardedVideoShownFailedEvent(string adUnitId)
//    {
        
//        this.mtgLog("==============onRewardedVideoShownFailedEvent: " + adUnitId);
//    }
//    //广告展示成功的回调
//    void onRewardedVideoShownEvent()
//    {
//        this.mtgLog("===============onRewardedVideoShownEvent");
       
//    }
//    //广告被点击的回调
//    void onRewardedVideoClickedEvent(string errorMsg)
//    {
//        this.mtgLog("=============onRewardedVideoClickedEvent: " + errorMsg);
//    }
//    //广告关闭的回调，播放视频页面关闭以后，您需要判断MTGRewardData的对象的convert属性，来决定是否给用户奖励。    
//    void onRewardedVideoClosedEvent(MintegralManager.MTGRewardData rewardData)
//    {
//        if (rewardData.converted)
//        {
//            if (mCompleteCallBack != null)
//                mCompleteCallBack(true);
//            this.mtgLog("================onRewardedVideoClosedEvent: " + rewardData.ToString());
//        }
//        else
//        {
//            if (mCompleteCallBack != null)
//                mCompleteCallBack(false);
//            this.mtgLog("=================onRewardedVideoClosedEvent: No Reward");
//        }
//    }
//#endif
	
}

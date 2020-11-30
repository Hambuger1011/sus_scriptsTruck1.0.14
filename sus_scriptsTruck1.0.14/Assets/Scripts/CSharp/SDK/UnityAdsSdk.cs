//using System.Collections;
//using System.Collections.Generic;
//#if !CHANNEL_HUAWEI
//using UnityEngine.Monetization;
//#endif
//using UnityEngine;
//using System;

//public class UnityAdsSdk 
//{

//    public string placementId = "rewardedVideo";

//#if UNITY_IOS
//   private string gameId = "2998275";
//#elif UNITY_ANDROID
//    private string gameId = "2998274";
//#else
//    private string gameId = "2998274";
//#endif

//    private Action<bool> mCompleteCallBack;

//    public void Init()
//    {
//#if !CHANNEL_HUAWEI
//        if (Monetization.isSupported)
//        {
//            Monetization.Initialize(gameId, true);
//        }
//#endif
//    }

//    public void CheckAds()
//    {
//#if !CHANNEL_HUAWEI
//        SdkMgr.Instance.UnityAdsInit = Monetization.IsReady(placementId);
//#endif
//    }

//    /// <summary>
//    /// 显示视频
//    /// </summary>
//    /// <param name="vCallBack"></param>
//    public void ShowAds(Action<bool> vCallBack)
//    {
//#if !CHANNEL_HUAWEI
//        CheckAds();
//        if(!SdkMgr.Instance.UnityAdsInit)
//        {
//            UITipsMgr.Instance.PopupTips("There's no video to watch yet", false);
//        }
//        else
//        {
//            mCompleteCallBack = vCallBack;
//            ShowAdCallbacks options = new ShowAdCallbacks();
//            options.finishCallback = HandleShowResult;
//            ShowAdPlacementContent ad = Monetization.GetPlacementContent(placementId) as ShowAdPlacementContent;
//            ad.Show(options);
//        }
//#endif
//    }

//#if !CHANNEL_HUAWEI
//    void HandleShowResult(ShowResult result)
//    {
//        if (result == ShowResult.Finished)
//        {
//            if (mCompleteCallBack != null)
//                mCompleteCallBack(true);
//        }
//        else if (result == ShowResult.Skipped)
//        {
//            if (mCompleteCallBack != null)
//                mCompleteCallBack(false);
//        }
//        else if (result == ShowResult.Failed)
//        {
//            if (mCompleteCallBack != null)
//                mCompleteCallBack(false);
//        }

//        SdkMgr.Instance.UnityAdsInit = false;

//        mCompleteCallBack = null;
//    }
//#endif

//}

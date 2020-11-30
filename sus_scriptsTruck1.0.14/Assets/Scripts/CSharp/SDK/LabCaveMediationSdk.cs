#if CHANNEL_SPAIN
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabCaveMediationSdk : LabCaveMediationDelegate
{
    public static string appHash = "5c5d385aab019d2b81181842";
    bool isBannerLoaded;
    bool isInterLoaded;
    bool isRewardedLoaded;


    public void Start()
    {
        LabCaveMediation.InitWithAppId(appHash, this);
        LabCaveMediation.SetLogging(GameUtility.isDebugMode);
    }

    public void OnMediationLoaded(LabCaveMediation.AdTypes adType)
    {
        Debug.Log("OnMediationLoaded " + adType);
        switch (adType)
        {
            case LabCaveMediation.AdTypes.BANNER:
                isBannerLoaded = true;
                break;
            case LabCaveMediation.AdTypes.INTERSTITIAL:
                isInterLoaded = true;
                break;
            case LabCaveMediation.AdTypes.VIDEOREWARDED:
                isRewardedLoaded = true;
                break;
        }

    }

    public void OnError(string description, LabCaveMediation.AdTypes type, string zoneId)
    {
        Debug.Log(zoneId + " OnError " + description + " " + type);
    }

    public void OnClick(LabCaveMediation.AdTypes adType, string provider, string zoneId)
    {
        Debug.Log(zoneId + " OnClick");
    }

    public void OnClose(LabCaveMediation.AdTypes adType, string provider, string zoneId)
    {
        Debug.Log(zoneId + " OnClose");
    }

    public void OnShow(LabCaveMediation.AdTypes adType, string provider, string zoneId)
    {
        AudioListener.pause = true;

        Debug.Log(zoneId + " OnShow");
    }

    public void OnReward(LabCaveMediation.AdTypes adType, string provider, string zoneId)
    {
        Debug.Log(zoneId + " onReward");
        CallRewardBasedVideoEvent(true);
    }




#region 激励视频接口

    public void RequestRewardBasedVideo()
    {
#if UNITY_EDITOR
        return;
#endif
    }

    Action<bool> _onRewardBasedViedoComplete;
    public void ShowRewardBasedVideo(Action<bool> callback)
    {
        _onRewardBasedViedoComplete = callback;
#if UNITY_EDITOR
        CallRewardBasedVideoEvent(true);
        return;
#endif
        if (LabCaveMediation.isRewardedVideoReady())
        {
            LabCaveMediation.ShowVideoRewardedWithZone("rewarded");
        }
        else
        {
            RequestRewardBasedVideo();
            UITipsMgr.Instance.PopupTips("There's no video to watch yet", false);
            CallRewardBasedVideoEvent(false);
            //MonoBehaviour.print("Reward based video ad is not ready yet");
        }
    }

    void CallRewardBasedVideoEvent(bool bSuc)
    {
        if (_onRewardBasedViedoComplete != null)
        {
            var tmp = _onRewardBasedViedoComplete;
            _onRewardBasedViedoComplete = null;
            tmp(bSuc);
        }
    }
    #endregion



    public void ShowInterstitial()
    {
        if(!LabCaveMediation.isInterstitialReady())
        {
            return;
        }
        LabCaveMediation.ShowInterstitialWithZone("inter");
    }
}
#endif
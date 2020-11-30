using System;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ADSReward : BaseUIForm
{
    private GameObject UIMask, RecompensasButton, CollectButton;
    int bookID, chapterID;

    public override void OnOpen()
    {
        base.OnOpen();
        FindGame();
        AddListenButton();
    }

    public override void OnClose()
    {
        base.OnClose();
        RemoveListenButton();
    }

    private void FindGame()
    {
        UIMask = transform.Find("UIMask").gameObject;
        RecompensasButton = transform.Find("Bg/RecompensasButton").gameObject;
        CollectButton = transform.Find("Bg/CollectButton").gameObject;

    }
    public void SetData(int m_curBookID, int m_curChapterID)
    {
        this.bookID = m_curBookID;
        this.chapterID = m_curChapterID;
    }

    private void AddListenButton()
    {
        UIEventListener.AddOnClickListener(UIMask, Close);
        //UIEventListener.AddOnClickListener(RecompensasButton, RecompensasButtonOnclicke);
        UIEventListener.AddOnClickListener(CollectButton, RecompensasButtonOnclicke);
    }

    private void RemoveListenButton()
    {
        UIEventListener.RemoveOnClickListener(UIMask, Close);
        //UIEventListener.RemoveOnClickListener(RecompensasButton, RecompensasButtonOnclicke);
        UIEventListener.RemoveOnClickListener(CollectButton, RecompensasButtonOnclicke);
    }

    private void Close(PointerEventData data)
    {
        CUIManager.Instance.CloseForm(UIFormName.ADSReward);
    }
    private void RecompensasButtonOnclicke(PointerEventData data)
    {
        GameHttpNet.Instance.GetChapterAdsReward(this.bookID, this.chapterID, GetAdsRewardCallBack);
    }

  
    private void GetAdsRewardCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetAdsRewardCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    AudioManager.Instance.PlayTones(AudioTones.RewardWin);
                    UserDataManager.Instance.adsRewardResultInfo = JsonHelper.JsonToObject<HttpInfoReturn<GetAdsRewardResultInfo>>(result);
                    Vector3 startPos = new Vector3(-188, -355);
                    Vector3 targetPos = new Vector3(306, 625);
                    RewardShowData rewardShowData = new RewardShowData();
                    rewardShowData.StartPos = startPos;
                    rewardShowData.TargetPos = targetPos;
                    rewardShowData.IsInputPos = false;
                    rewardShowData.Type = 1;
                    if (UserDataManager.Instance.adsRewardResultInfo != null && UserDataManager.Instance.adsRewardResultInfo.data != null)
                    {
                        rewardShowData.KeyNum = UserDataManager.Instance.adsRewardResultInfo.data.bkey;
                        rewardShowData.DiamondNum = UserDataManager.Instance.adsRewardResultInfo.data.diamond;
                        EventDispatcher.Dispatch(EventEnum.HiddenEggRewardShow, rewardShowData);    //观看视频，但是也是触发彩蛋的特效
                        TalkingDataManager.Instance.WatchTheAds(4);
                    }

                    Close(null);
                }
                else if (jo.code == 202 || jo.code == 203 || jo.code == 204)
                {
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                    UITipsMgr.Instance.PopupTips("You've reached the limit for today!", false);
                }
                else if (jo.code == 206)
                {
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                    UITipsMgr.Instance.PopupTips("Reward already collected before.", false);
                }
                else if (jo.code == 207 || jo.code == 205)
                {
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                    UITipsMgr.Instance.PopupTips("Chapter not completed, you can't collect the rewards.", false);
                }
                else //if (jo.code == 208)
                {
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                    UITipsMgr.Instance.PopupTips(jo.msg, false);
                }
            }, null);
        }
    }

}

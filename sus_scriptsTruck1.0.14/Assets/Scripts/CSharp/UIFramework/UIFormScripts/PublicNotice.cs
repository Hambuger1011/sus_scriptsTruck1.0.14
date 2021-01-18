using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UGUI;
using pb;

public class PublicNotice : BaseUIForm
{
    public GameObject ReceiveButton, CloseButton,Mask;
    public Text CountText;
    
    private int Type;
    private int mBuyType;
    public override void OnOpen()
    {
        base.OnOpen();
        Type = 0;
    }

    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(ReceiveButton, ReceiveButtonOnclicke);
        UIEventListener.RemoveOnClickListener(CloseButton, CloseButtonOnclicke);
        UIEventListener.RemoveOnClickListener(Mask, maskClose);
    }

    public void Inite(int type,int vBuyType)
    {
        Type = type;
        mBuyType = vBuyType;
        UIEventListener.AddOnClickListener(ReceiveButton, ReceiveButtonOnclicke);
        UIEventListener.AddOnClickListener(CloseButton,CloseButtonOnclicke);
        UIEventListener.AddOnClickListener(Mask, maskClose);
        switch (type)
        {
            case 1:
                //看广告
                CountText.text = "You can get free diamonds by watching ads.Would you like to have a look?";
                break;
            default:
                break;
        }
    }

    private void maskClose(PointerEventData data)
    {
        CloseButtonOnclicke(null);
    }
    private void CloseButtonOnclicke(PointerEventData data)
    {
 

        //CUIManager.Instance.OpenForm(UIFormName.ChargeTipsForm);
        //ChargeTipsForm tipForm = CUIManager.Instance.GetForm<ChargeTipsForm>(UIFormName.ChargeTipsForm);

        CUIManager.Instance.OpenForm(UIFormName.NewChargeTips);
        NewChargeTips tipForm = CUIManager.Instance.GetForm<NewChargeTips>(UIFormName.NewChargeTips);


        if (tipForm != null)
            tipForm.Init(mBuyType, 1, 1 * 0.99f);
    }

    private void ReceiveButtonOnclicke(PointerEventData data)
    {
        switch (Type)
        {
            case 1:
                //看广告
                WatchMove();
                break;
            default:
                break;
        }
    }

    #region  看广告
    private void WatchMove()
    {      
        if(UserDataManager.Instance.lotteryDrawInfo != null && UserDataManager.Instance.selfBookInfo.data.bookadcount > 0)
        {
            TalkingDataManager.Instance.WatchTheAds(1);
            SdkMgr.Instance.ShowAds(LookVideoComplete);
        }else
        {
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(150);
            UITipsMgr.Instance.PopupTips(Localization, false);

            //UITipsMgr.Instance.PopupTips("The number of advertisements has reached the upper limit.", false);
        }
    }
    private void LookVideoComplete(bool value)
    {
        if (value)
            GameHttpNet.Instance.GetAdsReward(1, GetAdsRewardCallBack);
    }
    private void GetAdsRewardCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetOrderToSubmitCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.adsRewardResultInfo = JsonHelper.JsonToObject<HttpInfoReturn<GetAdsRewardResultInfo>>(result);

                    AudioManager.Instance.PlayTones(AudioTones.RewardWin);

                    UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.adsRewardResultInfo.data.bkey);
                    UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.adsRewardResultInfo.data.diamond);

                    //Vector3 startPos = new Vector3(-188, -355);
                    //Vector3 targetPos = new Vector3(277, 628);
                    //RewardShowData rewardShowData = new RewardShowData();
                    //rewardShowData.StartPos = startPos;
                    //rewardShowData.TargetPos = targetPos;
                    //rewardShowData.IsInputPos = false;
                    //rewardShowData.KeyNum = UserDataManager.Instance.adsRewardResultInfo.data.bkey;
                    //rewardShowData.DiamondNum = UserDataManager.Instance.adsRewardResultInfo.data.diamond;
                    //rewardShowData.Type = 1;
                    //EventDispatcher.Dispatch(EventEnum.HiddenEggRewardShow, rewardShowData);

                    //UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.adsRewardResultInfo.data.bkey);
                    //UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.adsRewardResultInfo.data.diamond);
                    TalkingDataManager.Instance.WatchTheAds(2);

                    if (UserDataManager.Instance.lotteryDrawInfo != null && UserDataManager.Instance.selfBookInfo.data.bookadcount > 0)
                    {
                        UserDataManager.Instance.selfBookInfo.data.bookadcount--;
                    }

                    CloseButtonOnclicke(null);
                }
                else if (jo.code == 202)
                {
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(191);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("You've reached the limit for today!", false);
                }
                else if (jo.code == 208)
                {
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                }
            }, null);
        }
    }
    #endregion
}

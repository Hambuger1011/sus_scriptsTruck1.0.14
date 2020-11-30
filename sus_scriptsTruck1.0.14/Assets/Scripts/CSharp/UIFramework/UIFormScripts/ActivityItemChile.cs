using pb;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ActivityItemChile : MonoBehaviour {

    public Image ItemImage, titlImage, Buttons;
    private t_Activity vdata;
    public Text Day,Number, ButtonText;

    private int MoveNumber = 0;
	public void ActivityItemChilInit(t_Activity data)
    {
       
        UIEventListener.AddOnClickListener(ItemImage.gameObject, ActivityItemButtonOnclicke);
        
        vdata = data;
        ItemImage.sprite = ResourceManager.Instance.GetUISprite("Notice/"+ data.Picture);

        if (vdata.Activityid == 1001)
        {
            //Inviting gifts
            titlImage.sprite = ResourceManager.Instance.GetUISprite("Notice/bg_font3");
            Buttons.sprite= ResourceManager.Instance.GetUISprite("Notice/btn_iap1");
            ButtonText.text = CTextManager.Instance.GetText(290);
        }
        if (vdata.Activityid==1002)
        {
            //VIP      

            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.Getvipcard(VIPCallBacke);
            EventDispatcher.AddMessageListener(EventEnum.VIPDayUp, VIPDayUp);

            titlImage.sprite = ResourceManager.Instance.GetUISprite("Notice/bg_font2");

            Buttons.sprite = ResourceManager.Instance.GetUISprite("Notice/btn_iap");
            ButtonText.text = CTextManager.Instance.GetText(291);
        }
        if (vdata.Activityid == 1003)
        {
            //Super Package
            IsPremiumGiftBag = true;
            EventDispatcher.AddMessageListener(EventEnum.RecordPremiumGiftBagBuy, RecordPremiumGiftBagBuy);

            titlImage.sprite = ResourceManager.Instance.GetUISprite("Notice/bg_font1");
            Buttons.sprite = ResourceManager.Instance.GetUISprite("Notice/btn_iap3");
            ButtonText.text = "$0.99";
        }
        if (vdata.Activityid == 1004)
        {
            //Login Rewards
            titlImage.sprite = ResourceManager.Instance.GetUISprite("Notice/bg_font4");

            Buttons.sprite = ResourceManager.Instance.GetUISprite("Notice/btn_iap2");
            ButtonText.text = CTextManager.Instance.GetText(292);
        }
        if (vdata.Activityid == 1005)
        {
            //Everything X2！
            titlImage.sprite = ResourceManager.Instance.GetUISprite("Notice/bg_font");

            Buttons.gameObject.SetActive(false);
           
        }


        if (vdata.Activityid == 1006)
        {
            //看电影的
            Number.gameObject.SetActive(true);
            UpMoveNumber();

            titlImage.sprite = ResourceManager.Instance.GetUISprite("Notice/bg_font5");
            Buttons.sprite = ResourceManager.Instance.GetUISprite("Notice/btn_iap4");
           
        }

       
       
    }

    public void VIPDayUp(Notification notfi)
    {
        if (Day.gameObject == null)
        {
            return;
        }     
        Day.text = UserDataManager.Instance.Getvipcard.data.day + " days";
    }
    public void UpMoveNumber()
    {
        //红点协议

        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.ActiveInfo(GetimpinfoCallBacke);
    }

    private void ActivityItemButtonOnclicke(PointerEventData data)
    {
      
        switch (vdata.Activityid)
        {
            case 1001://邀请好友
                InviteFriends();
                break;
            case 1002://VIP界面
                VipFunction();
                break;
            case 1003://超值礼包
                PremiumGiftBag();
                break;
            case 1004://谷歌Face登录界面
                GooldFaceUI();
                break;
            case 1005://商城钻石界面
                OpenShopUI();
                break;
            case 1006://打开看视频的弹窗
                VideoUI();
                break;
        }
    }

    #region  邀请好友
    private void InviteFriends()
    {
        CUIManager.Instance.OpenForm(UIFormName.InviteForm);
    }
    #endregion

    #region VIP
    private void VipFunction()
    {
        //LOG.Info("vip按钮被点击了");
        CUIManager.Instance.OpenForm(UIFormName.VIP);
    }
    #endregion

    #region 超级礼包

    private bool IsPremiumGiftBag = false;
    private bool PremiumGiftBagHadBuy = false;
    private void PremiumGiftBag()
    {
        if (IsPremiumGiftBag)
        {
            if (!PremiumGiftBagHadBuy)
            {
                CUIManager.Instance.OpenForm(UIFormName.FirstGigtGroup);
            }else
            {
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(133);

                UITipsMgr.Instance.PopupTips(Localization, false);
                //UITipsMgr.Instance.PopupTips("Have to buy.", false);
            }
        }
        LOG.Info("点击了超级礼包");
        
    }

    private void RecordPremiumGiftBagBuy(Notification notification)
    {
        if (IsPremiumGiftBag)
        {
            PremiumGiftBagHadBuy = true;
        }
    }
    #endregion

    #region 谷歌Face登录界面
    private void GooldFaceUI()
    {
        CUIManager.Instance.OpenForm(UIFormName.LoginForm);
        CUIManager.Instance.GetForm<LoginForm>(UIFormName.LoginForm).btnCloseToTrue();
    }
    #endregion

    #region  商城钻石界面
    private void OpenShopUI()
    {
        MainTopSprite MainTopSprite= CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop);
        
        MainTopSprite.OpenChargeMoney_Diamonds(null);
    }
    #endregion

    #region 打开看视频的弹窗
    private void VideoUI()
    {

        if (MoveNumber>0)
        {
            TalkingDataManager.Instance.WatchTheAds(1);
            SdkMgr.Instance.ShowAds(LookVideoComplete);
        }else
        {
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(134);

            UITipsMgr.Instance.PopupTips(Localization, false);
        }
       
    }

    private void LookVideoComplete(bool value)
    {
        if (value)
            GameHttpNet.Instance.GetAdsReward(3, GetAdsRewardCallBack);
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
                    AudioManager.Instance.PlayTones(AudioTones.RewardWin);
                    UserDataManager.Instance.adsRewardResultInfo = JsonHelper.JsonToObject<HttpInfoReturn<GetAdsRewardResultInfo>>(result);

                    UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.adsRewardResultInfo.data.bkey);
                    UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.adsRewardResultInfo.data.diamond);

                    //Vector3 startPos = new Vector3(-188, -355);
                    //Vector3 targetPos = new Vector3(174, 720);
                    //RewardShowData rewardShowData = new RewardShowData();
                    //rewardShowData.StartPos = startPos;
                    //rewardShowData.TargetPos = targetPos;
                    //rewardShowData.IsInputPos = false;
                    //rewardShowData.KeyNum = UserDataManager.Instance.adsRewardResultInfo.data.bkey;
                    //rewardShowData.DiamondNum = UserDataManager.Instance.adsRewardResultInfo.data.diamond;
                    //rewardShowData.TicketNum = UserDataManager.Instance.UserData.TicketNum;
                    //rewardShowData.Type = 1;
                    //EventDispatcher.Dispatch(EventEnum.GetRewardShow, rewardShowData);
                    TalkingDataManager.Instance.WatchTheAds(2);

                    //计算电影的剩余的数量
                    MoveNumber--;
                    if (MoveNumber<=0)
                    {
                        MoveNumber = 0;
                    }
                    if (MoveNumber > 20)
                    {                      
                        ButtonText.text = CTextManager.Instance.GetText(293);
                    }
                    else
                    {                       
                        ButtonText.text = CTextManager.Instance.GetText(294, MoveNumber);
                    }
                }
                else if (jo.code == 202)
                {
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(135);
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


    private void VIPCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----VIPCallBacke---->" + result);
        if (result.Equals("error"))
        {
            LOG.Info("---VIPCallBacke--这条协议错误");
            return;
        }
        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo != null)
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.Getvipcard = JsonHelper.JsonToObject<HttpInfoReturn<Getvipcard<vipinfo>>>(arg.ToString());

                    if (Day == null || Day.gameObject==null)
                    {
                        return;
                    }

                    if (UserDataManager.Instance.Getvipcard.data.day == 0)
                    {
                        Day.gameObject.SetActive(false);
                        return;
                    }
                    Day.gameObject.SetActive(true);
                    Day.text = UserDataManager.Instance.Getvipcard.data.day+ " days";
                }
            }

        }, null);
    }

    public void GetimpinfoCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetimpinfoCallBacke---->" + result);
        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo.code == 200)
            {
                UserDataManager.Instance.lotteryDrawInfo = JsonHelper.JsonToObject<HttpInfoReturn<ActiveInfo>>(arg.ToString());

                if (UserDataManager.Instance.lotteryDrawInfo != null && UserDataManager.Instance.lotteryDrawInfo.data != null)
                {
                    if (Number == null || Number.gameObject==null)
                    {
                        return;
                    }
                    //MoveNumber = UserDataManager.Instance.lotteryDrawInfo.data.activeadcount;
                    MoveNumber = UserDataManager.Instance.lotteryDrawInfo.data.shopadcount;
                    if (MoveNumber>20)
                    {
                        Number.gameObject.SetActive(false);
                        ButtonText.text = CTextManager.Instance.GetText(293);
                    }
                    else
                    {
                        //Number.text = MoveNumber.ToString();

                        ButtonText.text = CTextManager.Instance.GetText(294, MoveNumber);
                    }                   
                }
            }
        }, null);
    }

    private void OnDisable()
    {
        //UIEventListener.RemoveOnClickListener(ItemImage.gameObject, ActivityItemButtonOnclicke);
        //if (vdata !=null&& vdata.Activityid == 1002)
        //{         
        //    EventDispatcher.RemoveMessageListener(EventEnum.VIPDayUp, VIPDayUp);
        //}

        //if (vdata != null && vdata.Activityid == 1003)
        //{         
        //    EventDispatcher.RemoveMessageListener(EventEnum.RecordPremiumGiftBagBuy, RecordPremiumGiftBagBuy);
        //}
    }

    public void DisPoste()
    {
        UIEventListener.RemoveOnClickListener(ItemImage.gameObject, ActivityItemButtonOnclicke);
        if (vdata != null && vdata.Activityid == 1002)
        {
            EventDispatcher.RemoveMessageListener(EventEnum.VIPDayUp, VIPDayUp);
        }

        if (vdata != null && vdata.Activityid == 1003)
        {
            EventDispatcher.RemoveMessageListener(EventEnum.RecordPremiumGiftBagBuy, RecordPremiumGiftBagBuy);
        }

        ItemImage.sprite = null;

        Destroy(gameObject);
    }
}

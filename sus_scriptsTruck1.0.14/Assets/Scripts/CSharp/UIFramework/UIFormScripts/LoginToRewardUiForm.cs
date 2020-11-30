using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using pb;

public class LoginToRewardUiForm : BaseUIForm {

	
    public GameObject UIMask;
    public GameObject Content1;
    public Text Day,Number;
    public Image bgform, showImage;
    public GameObject ClaimButton;

    private int AwardType; //获取奖励的类型 1钥匙 2钻石
    private int AwardNumber = 0;
    public override void OnOpen()
    {
        base.OnOpen();
        UIEventListener.AddOnClickListener(UIMask, backToMainForm);
        UIEventListener.AddOnClickListener(ClaimButton, ClaimButtonOnclicke);

        SpwanLoginRewardItem();
        ShowState();
    }

    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(UIMask, backToMainForm);
        UIEventListener.RemoveOnClickListener(ClaimButton, ClaimButtonOnclicke);
    }
    private void backToMainForm(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        //Debug.Log("ssddsdfd");
        Tween t = Content1.transform.parent.parent.parent.parent.DOScale(new Vector3(0, 0, 0), 0.3f);
        t.OnComplete(CloseUi);
    }


    //这个是点击领取了奖励的物品
    private void ClaimButtonOnclicke(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
#if USE_SERVER_DATA
        if (UserDataManager.Instance.selfBookInfo != null && UserDataManager.Instance.selfBookInfo.data != null)
        {
            int status = UserDataManager.Instance.selfBookInfo.data.dayprice_status;
            if(status == 1)
            {
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(186);
                UITipsMgr.Instance.PopupTips(Localization, false);

                //UITipsMgr.Instance.PopupTips("You've already claimed this reward.", false);
            }
            else if(status == 2)
            {
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(186);
                UITipsMgr.Instance.PopupTips(Localization, false);

                //UITipsMgr.Instance.PopupTips("You've already claimed this reward.", false);
            }
            else if(status == 3)
            {
                //UINetLoadingMgr.Instance.Show();
                GameHttpNet.Instance.GetDayLogin(DayLoginCallBack);
            }
        }
#else


        if (PlayerPrefs.GetInt("nowYearDay") != PlayerPrefs.GetInt("oldYearDay"))
        {
            PlayerPrefs.SetInt("oldYearDay", PlayerPrefs.GetInt("nowYearDay"));
            PlayerPrefs.SetInt("wardNumber", PlayerPrefs.GetInt("wardNumber") + 1);
            ClaimButtonOnclickeClose();//自动关闭界面
        }

#endif
    }

    private void DayLoginCallBack(object arg)
    {
        string result = arg.ToString();
        Debug.Log("----BuyChapterCallBack---->" + result);

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
             LoomUtil.QueueOnMainThread((param) => 
             {
                 //UINetLoadingMgr.Instance.Close();
                 if (jo.code == 200)
                 {
                     UserDataManager.Instance.dayLoginInfo = JsonHelper.JsonToObject<HttpInfoReturn<DayLoginInfo>>(result);
                     if (UserDataManager.Instance.selfBookInfo != null && UserDataManager.Instance.selfBookInfo.data != null)
                     {
                         UserDataManager.Instance.selfBookInfo.data.dayprice_status = 2;
                     }
                     ClaimButtonOnclickeClose();
                 }
                 else if (jo.code == 202 || jo.code == 203)
                 {
                     var Localization = GameDataMgr.Instance.table.GetLocalizationById(186);
                     UITipsMgr.Instance.PopupTips(Localization, false);

                     //UITipsMgr.Instance.PopupTips("You've already claimed this reward.", false);
                 }
                 else if (jo.code == 208)
                 {
                     var Localization = GameDataMgr.Instance.table.GetLocalizationById(187);
                     UITipsMgr.Instance.PopupTips(Localization, false);

                     //UITipsMgr.Instance.PopupTips("Reward failed", false);
                 }
                 else if (jo.code == 277)
                 {
                     UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, jo.msg);
                     return;
                 }
             }, null);
        }
    }

 
    private void ClaimButtonOnclickeClose()
    {
        Tween t = Content1.transform.parent.parent.parent.parent.DOScale(new Vector3(0, 0, 0), 0.3f);
        t.OnComplete(ClaimButtonOnclikeCloseUi);
    }

    private void ClaimButtonOnclikeCloseUi()
    {
        CUIManager.Instance.CloseForm(UIFormName.LoginToRewardUiForm);
        SpawnAwardGame();
    }

    //点击领取了之后自动关闭界面
    private void OnclickeCloseUI()
    {
        Tween t = Content1.transform.parent.parent.parent.parent.DOScale(new Vector3(0, 0, 0), 0.3f);
        t.OnComplete(CloseUi);
       
    }

    //这个是生成奖励
    private void SpawnAwardGame()
    {
        //MainTopSprite mainTopS = GameObject.Find("Canvas_Main_top(Clone)").GetComponent<MainTopSprite>();

        MainTopSprite mainTopS = CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop);

#if USE_SERVER_DATA
        if(UserDataManager.Instance.dayLoginInfo != null)
        {
            mainTopS.AwardEvent(UserDataManager.Instance.dayLoginInfo.data.rewardType, UserDataManager.Instance.dayLoginInfo.data.rewardAmount);
            if(UserDataManager.Instance.dayLoginInfo.data.rewardType == 1)
                TalkingDataManager.Instance.OnReward(UserDataManager.Instance.dayLoginInfo.data.rewardAmount, "Day Login key");
            else
                TalkingDataManager.Instance.OnReward(UserDataManager.Instance.dayLoginInfo.data.rewardAmount, "Day Login diamond");
        }
#else
        mainTopS.AwardEvent(AwardType,AwardNumber);
#endif

    }

    private void CloseUi()
    {
        CUIManager.Instance.CloseForm(UIFormName.LoginToRewardUiForm);
    }

    private void SpwanLoginRewardItem()
    {
        for (int i = 1; i < 8; i++)
        {
            LoginRewardItemForm item = ResourceManager.Instance.LoadAssetBundleUI(UIFormName.NewLoginRewardItemForm).GetComponent<LoginRewardItemForm>();
            item.transform.SetParent(Content1.transform, false);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            item.GameInite(i);
        }   
    }

    //显示状态
    private void ShowState()
    {
        int loginDay = PlayerPrefs.GetInt("wardNumber");

#if USE_SERVER_DATA
        if (UserDataManager.Instance.selfBookInfo != null && UserDataManager.Instance.selfBookInfo.data != null)
        {
            loginDay = UserDataManager.Instance.selfBookInfo.data.loginday;
        }
#endif

        Day.text = "day " + loginDay;
        if (loginDay == 7)
        {
            string pathFront = "LoginToRewardUiForm/bg_nskme_03";
            string pathshowImage = "LoginToRewardUiForm/bg_jlame_03";

            bgform.sprite = ResourceManager.Instance.GetUISprite(pathFront);
            showImage.sprite = ResourceManager.Instance.GetUISprite(pathshowImage);
            Number.text = "x20";

            AwardType = 2;
            AwardNumber = 20;

        }
        else
        {
            string pathFront = "LoginToRewardUiForm/bg_jslem_03";
            string pathshowImage = "LoginToRewardUiForm/bg_kamex_03";

            bgform .sprite= ResourceManager.Instance.GetUISprite(pathFront);
            showImage.sprite= ResourceManager.Instance.GetUISprite(pathshowImage);

            Number.text = "x2";

            AwardType = 1;
            AwardNumber = 2;
        }
    }
}

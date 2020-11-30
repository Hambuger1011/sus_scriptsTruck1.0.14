using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UGUI;
using UnityEngine.UI;
using pb;

public class GamePlaySetting : BaseUIForm
{

	// Use this for initialization
	void Start () {
		
	}

    public GameObject UIMask;
    public Image NotificationsButton, EarnRewardsButton;
    public GameObject HelpButton, ShareButton;
    public InputField inputFieldText;

    private string OnIconPath = "GamePlaySetting/btn_on";
    private string OffIconPath = "GamePlaySetting/btn_off";

    public override void OnOpen()
    {
        base.OnOpen();

        UIEventListener.AddOnClickListener(UIMask, backToMainForm);
        UIEventListener.AddOnClickListener(NotificationsButton.gameObject, NotificationButtonOnclicke);
        UIEventListener.AddOnClickListener(EarnRewardsButton.gameObject, EarnRewardsButtonOnClicke);
        UIEventListener.AddOnClickListener(HelpButton, HelpButtonOnclicke);
        UIEventListener.AddOnClickListener(ShareButton,ShareButtonOnclicke);

        if (inputFieldText != null)
        {
            //UUIDtxt.text = "UUID:" + GameHttpNet.Instance.UUID;
            inputFieldText.text = "ID:" + GameHttpNet.Instance.UUID;
            inputFieldText.onValueChanged.AddListener(InputChangeHandler);
        }

        ResetBtnState();      
    }

    public override void OnClose()
    {
        base.OnClose();

        UIEventListener.RemoveOnClickListener(UIMask, backToMainForm);
        UIEventListener.RemoveOnClickListener(NotificationsButton.gameObject, NotificationButtonOnclicke);
        UIEventListener.RemoveOnClickListener(EarnRewardsButton.gameObject, EarnRewardsButtonOnClicke);
        UIEventListener.RemoveOnClickListener(HelpButton, HelpButtonOnclicke);
        UIEventListener.RemoveOnClickListener(ShareButton, ShareButtonOnclicke);
        if (inputFieldText != null)
            inputFieldText.onValueChanged.RemoveListener(InputChangeHandler);
    }

    private void InputChangeHandler(string vStr)
    {
        inputFieldText.text = "ID:" + GameHttpNet.Instance.UUID;
    }

    private void ResetBtnState()
    {
        if (UserDataManager.Instance.UserData.BgMusicIsOn == 1)
            NotificationsButton.sprite = ResourceManager.Instance.GetUISprite(OnIconPath);
        else
            NotificationsButton.sprite = ResourceManager.Instance.GetUISprite(OffIconPath);

        if (UserDataManager.Instance.UserData.TonesIsOn == 1)
            EarnRewardsButton.sprite = ResourceManager.Instance.GetUISprite(OnIconPath);
        else
            EarnRewardsButton.sprite = ResourceManager.Instance.GetUISprite(OffIconPath);
    }

    private void backToMainForm(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CUIManager.Instance.CloseForm(UIFormName.GamePlaySetting);
    }

    private void NotificationButtonOnclicke(PointerEventData date)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        if (UserDataManager.Instance.UserData.BgMusicIsOn == 1)
        {
            UserDataManager.Instance.UserData.BgMusicIsOn = 0;
            AudioManager.Instance.StopBGM();
        }
        else
        {
            UserDataManager.Instance.UserData.BgMusicIsOn = 1;
            AudioManager.Instance.PlayBGMAgain();
        }
           
        UserDataManager.Instance.SaveUserDataToLocal();
        ResetBtnState();
    }

    private void EarnRewardsButtonOnClicke(PointerEventData date)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        if (UserDataManager.Instance.UserData.TonesIsOn == 1)
        {
            UserDataManager.Instance.UserData.TonesIsOn = 0;
            AudioManager.Instance.ChangeTonesVolume(0);
        }
        else
        {
            UserDataManager.Instance.UserData.TonesIsOn = 1;
            AudioManager.Instance.ChangeTonesVolume(1);
        }
           

        UserDataManager.Instance.SaveUserDataToLocal();
        ResetBtnState();
    }

    private void HelpButtonOnclicke(PointerEventData date)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        //LOG.Info("这个是Feedback按钮点击了");
        CUIManager.Instance.OpenForm(UIFormName.GamePlayFeedback);
        CUIManager.Instance.CloseForm(UIFormName.GamePlaySetting);
    }

    private void ShareButtonOnclicke(PointerEventData date)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
       // LOG.Info("这个是Share按钮点击了");
        TalkingDataManager.Instance.ShareRecord(1);
        string linkUrl = "";
#if UNITY_ANDROID
        linkUrl = "https://play.google.com/store/apps/details?id=" + SdkMgr.packageName;
#endif
#if UNITY_IOS
        linkUrl = "https://www.facebook.com/Scripts-Untold-Secrets-107729237761206/";
#endif
        if (!string.IsNullOrEmpty(linkUrl))
            SdkMgr.Instance.facebook.FBShareLink(linkUrl, "Secrets of game choices", "Welcome to Secrets", "", FBShareLinkSucced, FBShareLinkFaild);
    }

    private void FBShareLinkSucced(string postId)
    {
        TalkingDataManager.Instance.ShareRecord(2);
        LOG.Info("--ShareSucc--->postId:" + postId);

        var Localization = GameDataMgr.Instance.table.GetLocalizationById(142);
        UITipsMgr.Instance.PopupTips(Localization, false);

        //UITipsMgr.Instance.PopupTips("Share Successful!", false);
    }

    private void FBShareLinkFaild(bool isCancel, string errorInfo)
    {
        LOG.Info("--ShareFail--->errorInfo:" + errorInfo + "--isCancel-->" + isCancel);

        var Localization = GameDataMgr.Instance.table.GetLocalizationById(143);
        UITipsMgr.Instance.PopupTips(Localization, false);

        //UITipsMgr.Instance.PopupTips("Share failed!", false);
    }
}

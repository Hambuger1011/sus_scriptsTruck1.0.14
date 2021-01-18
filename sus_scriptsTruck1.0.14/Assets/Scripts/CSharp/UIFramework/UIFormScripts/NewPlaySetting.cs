using pb;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewPlaySetting : BaseUIForm
{
    private GameObject Backe;
    private Image MusicButton, SoundButton;
    private Image FacebookButton;
    private Text FNameText;
    private Text PlatformTypeText;
    private Text GNameText;
    private Image GoolButton;
    private InputField UUIDText;
    private InputField SendInputField;
    private string InputString = "";
    private Image sendButton;
    private HwUserInfo hwUserInfo;
    private GameObject HelpBtn;
    private GameObject TermsBtn;
    private GameObject PrivacyBtn;
    private Slider slider;
    private Text sliderText;

    private const string OnIconPath = "SettingForm/btn_on";
    private const string OffIconPath = "SettingForm/btn_off";
    private const string FbOn = "SettingForm/btn_fackbook";
    private const string FbOff = "SettingForm/btn_fackbook1";
    private const string GlOn = "SettingForm/btn_google";
    private const string GlOff = "SettingForm/btn_google1";
    private const string HwOn = "SettingForm/btn_huawei1";
    private const string HwOff = "SettingForm/btn_huawai2";
    private const string GLFlagIconPath = "SettingForm/btn_googles";
    private const string HwFlagIconPath = "SettingForm/btn_huawei";
    private const string sendOn = "SettingForm/bg_klzd_03";
    private const string sendOff = "SettingForm/bg_jsle_03";


    private int mCurLoginChannel = 0;

    private List<float> speedList =new List<float>{-2f, -1f, -0.5f, 0f, 0.5f, 1f, 1.5f, 2f};
    public override void OnOpen()
    {
        if (GameDataMgr.Instance.InAutoPlay)
        {
            GameDataMgr.Instance.AutoPlayPause = true;
            GameDataMgr.Instance.InAutoPlay = false;
        }
        base.OnOpen();

        Backe = transform.Find("Frame/BG/SettingBG/Backe").gameObject;
        MusicButton = transform.Find("Frame/BG/SettingBG/Notifications/Switch").GetComponent<Image>();
        SoundButton = transform.Find("Frame/BG/SettingBG/EarnRewards/Switch").GetComponent<Image>();
        FacebookButton = transform.Find("Frame/BG/SettingBG/Facebook/Switch").GetComponent<Image>();
        FNameText = transform.Find("Frame/BG/SettingBG/Facebook/Switch/NameTxt").GetComponent<Text>();
        PlatformTypeText = transform.Find("Frame/BG/SettingBG/Facebook/Switch/PlatformTypeTxt").GetComponent<Text>();
        GoolButton = transform.Find("Frame/BG/SettingBG/Google/Switch").GetComponent<Image>();
        GNameText = transform.Find("Frame/BG/SettingBG/Google/Switch/GoogleText").GetComponent<Text>();
        UUIDText = transform.Find("Frame/BG/SettingBG/UUID").GetComponent<InputField>();
        SendInputField = transform.Find("Frame/BG/SettingBG/SendBg/InputField").GetComponent<InputField>();
        sendButton = transform.Find("Frame/BG/SettingBG/SendBg/SendButton").GetComponent<Image>();
        HelpBtn = transform.Find("Frame/BG/SettingBG/HelpBtn").gameObject;
        TermsBtn = transform.Find("Frame/BG/SettingBG/TermsBtn").gameObject;
        PrivacyBtn = transform.Find("Frame/BG/SettingBG/PrivacyBtn").gameObject;
        slider = transform.Find("Frame/BG/SettingBG/AutoPlaySpeed/Slider").GetComponent<Slider>();
        sliderText = transform.Find("Frame/BG/SettingBG/AutoPlaySpeed/Slider/Handle Slide Area/Handle/Text").GetComponent<Text>();

        GoolButton.gameObject.SetActive(false);
#if UNITY_ANDROID
        UIEventListener.AddOnClickListener(GoolButton.gameObject,GoogleButOn);
        GoolButton.gameObject.SetActive(true);
#endif

        if (UUIDText != null)
        {
            string UUID = GameHttpNet.Instance.UUID;

#if !UNITY_EDITOR
            if(string.IsNullOrEmpty(UUID))
                UUID = GameHttpNet.Instance.UUID;
#endif          
            UUIDText.text = "ID:" + UUID;
            UUIDText.onValueChanged.AddListener(delegate
            {
                UUIDText.text = "ID:" + UUID;
            });
        }

        addMessageListener(EventEnum.FaceBookLoginSucc, FaceBookLoginSuccHandler);
        addMessageListener(EventEnum.GoogleLoginSucc, GoogleLoginSuccHandler);
        addMessageListener(EventEnum.ThirdPartyLoginSucc, ThirdPartyLoginSuccHandler);
        addMessageListener(EventEnum.HuaweiLoginInfo, HuaweiLoginHandler);


        UIEventListener.AddOnClickListener(Backe, backToMainForm);
        UIEventListener.AddOnClickListener(MusicButton.gameObject, NotificationHandler);
        UIEventListener.AddOnClickListener(SoundButton.gameObject, EarnRewardHandler);
        UIEventListener.AddOnClickListener(FacebookButton.gameObject, OnLoginClick);
        UIEventListener.AddOnClickListener(sendButton.gameObject, SendButtonOnClicke);
        UIEventListener.AddOnClickListener(HelpBtn, HelpHandler);
        UIEventListener.AddOnClickListener(TermsBtn, TermsHandler);
        UIEventListener.AddOnClickListener(PrivacyBtn, PrivacyHandler);

        SendInputField.onValueChanged.AddListener(InputChangeHandler);
        slider.onValueChanged.AddListener(SilderChangeHandler);

        ResetBtnState();
    }


    public override void OnClose()
    {
        if (GameDataMgr.Instance.AutoPlayPause)
        {
            GameDataMgr.Instance.AutoPlayPause = false;
            GameDataMgr.Instance.InAutoPlay = true;
        }
        base.OnClose();

        UIEventListener.RemoveOnClickListener(Backe, backToMainForm);
        UIEventListener.RemoveOnClickListener(MusicButton.gameObject, NotificationHandler);
        UIEventListener.RemoveOnClickListener(SoundButton.gameObject, EarnRewardHandler);
        UIEventListener.RemoveOnClickListener(FacebookButton.gameObject, OnLoginClick);
        UIEventListener.RemoveOnClickListener(sendButton.gameObject, SendButtonOnClicke);
        UIEventListener.RemoveOnClickListener(HelpBtn, HelpHandler);
        UIEventListener.RemoveOnClickListener(TermsBtn, TermsHandler);
        UIEventListener.RemoveOnClickListener(PrivacyBtn, PrivacyHandler);

        SendInputField.onValueChanged.RemoveListener(InputChangeHandler);
        slider.onValueChanged.RemoveListener(SilderChangeHandler);
    }

    private void HelpHandler(PointerEventData data)
    {
     
    }

    private void TermsHandler(PointerEventData data)
    {
     
    }
    private void PrivacyHandler(PointerEventData data)
    {
       
    }

    private void InputChangeHandler(string vStr)
    {
        InputString = SendInputField.text;
        if (InputString.Length >= 10)
        {
            sendButton.sprite = ResourceManager.Instance.GetUISprite(sendOn);
        }
        else if (InputString.Length < 10)
        {
            sendButton.sprite = ResourceManager.Instance.GetUISprite(sendOff);
        }
    }

    private void SilderChangeHandler(float even)
    {
        int v = (int) even - 1;
        UserDataManager.Instance.UserData.AutoSpeed = speedList[v];
        sliderText.text = speedList[v].ToString();
        UserDataManager.Instance.SaveUserDataToLocal();
    }

    private void SendButtonOnClicke(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        InputString = SendInputField.text;

        if (InputString.Length < 10)
        {
            AudioManager.Instance.PlayTones(AudioTones.LoseFail);

            var Localization = GameDataMgr.Instance.table.GetLocalizationById(163);
            UITipsMgr.Instance.PopupTips(Localization, false);

            //UITipsMgr.Instance.PopupTips("Send more than 4 characters!", false);
            return;
        }

        LOG.Info("你发送的内容是：" + InputString);

        // SendEmail(UnityPath, InputString);
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.UserFeedback(4, "", InputString, UserFeedbackCallBack);
    }


    private void UserFeedbackCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----UserFeedbackCallBack---->" + result);
        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();
            JsonObject jo = JsonHelper.JsonToJObject(result);

            if (jo != null)
            {
                if (jo.code == 200)
                {
                    SendInputField.text = "";

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(164);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("success", false);
                    AudioManager.Instance.PlayTones(AudioTones.RewardWin);
                }
                else
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(165);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Error, send again!", false);
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                }
            }
            else
            {
                //UINetLoadingMgr.Instance.Close();

                var Localization = GameDataMgr.Instance.table.GetLocalizationById(165);
                UITipsMgr.Instance.PopupTips(Localization, false);

                //UITipsMgr.Instance.PopupTips("Error, send again!", false);
                AudioManager.Instance.PlayTones(AudioTones.LoseFail);
            }
        }, null);
    }

    /// <summary>
    /// 关闭游戏进行中的设置界面
    /// </summary>
    /// <param name="data"></param>
    private void backToMainForm(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        LOG.Info("backToMainForm");
        CUIManager.Instance.CloseForm(UIFormName.SettingForm);
    }

    private void NotificationHandler(PointerEventData data)
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
    private void EarnRewardHandler(PointerEventData data)
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

    private void ResetBtnState()
    {
        if (UserDataManager.Instance.UserData.BgMusicIsOn == 1)
            MusicButton.sprite = ResourceManager.Instance.GetUISprite(OnIconPath);
        else
            MusicButton.sprite = ResourceManager.Instance.GetUISprite(OffIconPath);

        if (UserDataManager.Instance.UserData.TonesIsOn == 1)
            SoundButton.sprite = ResourceManager.Instance.GetUISprite(OnIconPath);
        else
            SoundButton.sprite = ResourceManager.Instance.GetUISprite(OffIconPath);

        slider.value = speedList.IndexOf(UserDataManager.Instance.UserData.AutoSpeed)+1;
    }

    private void OnLoginClick(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        if (UserDataManager.Instance.LoginInfo != null)
        {
            if (UserDataManager.Instance.LoginInfo.LastLoginChannel == 1 && UserDataManager.Instance.LoginInfo.FaceBookLoginInfo != null)
            {
                SdkMgr.Instance.facebook.Logout();
                GameHttpNet.Instance.TOKEN = string.Empty;
                ReLoadInfo();
            }
            else if (UserDataManager.Instance.LoginInfo.LastLoginChannel == 0 || UserDataManager.Instance.LoginInfo.LastLoginChannel == 4)
            {
                mCurLoginChannel = 0;
                SdkMgr.Instance.FacebookLogin(2);
            }
        }
    }

    private void ReLoadInfo()
    {      
        GameDataMgr.Instance.userData.RemoveAllBook();

        var Localization = GameDataMgr.Instance.table.GetLocalizationById(190);
        UITipsMgr.Instance.PopupTips(Localization, false);

        //UITipsMgr.Instance.PopupTips("Log out Successful!", false);
        UserDataManager.Instance.LoginInfo.LastLoginChannel = 4;
        UserDataManager.Instance.LogOutDelLocalInfo();
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.GetUserInfo(GetUserInfoCallBack);
        LoadingButtonShow();

    }

    private void GetUserInfoCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetUserInfoCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.userInfo = JsonHelper.JsonToObject<HttpInfoReturn<UserInfoCont>>(arg.ToString());

                    if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data != null)
                    {
                        UserDataManager.Instance.UserData.UserID = UserDataManager.Instance.userInfo.data.userinfo.uid;
                        UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.userInfo.data.userinfo.bkey, false);
                        UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.userInfo.data.userinfo.diamond, false);
                        UserDataManager.Instance.UserData.bookList = new List<int>();
                        //UserDataManager.Instance.UserData.bookList.AddRange(UserDataManager.Instance.userInfo.data.userinfo.booklist);


                        GameDataMgr.Instance.SetServerTime(int.Parse(UserDataManager.Instance.userInfo.data.userinfo.current_time));
                        //【初始化评星】
                        XLuaHelper.initAppRating();
                        //同步头像框缓存
                        XLuaManager.Instance.GetLuaEnv().DoString(@"Cache.DressUpCache:ResetLogin();");
                    }

                    GameHttpNet.Instance.GetSelfBookInfo(ToLoadSelfBookInfo);
                    //GameHttpNet.Instance.GetShopList(GetShopListCallBack);
                }
                else if (jo.code == 277)
                {
                    //UINetLoadingMgr.Instance.Close();
                    UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, jo.msg);
                    return;
                }
                else
                {
                    //UINetLoadingMgr.Instance.Close();

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(175);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Your information is out of date. Please, log in again.", false);
                    LOG.Error(jo.msg);
                }
            }, null);
        }
    }

    private void ToLoadSelfBookInfo(object arg)
    {

        string result = arg.ToString();
        LOG.Info("----ToLoadSelfBookInfo---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);

        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.selfBookInfo = JsonHelper.JsonToObject<HttpInfoReturn<SelfBookInfo>>(arg.ToString());
                    UserDataManager.Instance.InitRecordServerBookData();
                    // EventDispatcher.Dispatch(EventEnum.BookJoinToShelfEvent);
                    //刷新我的书本
                    XLuaManager.Instance.GetLuaEnv().DoString(@"GameController.MainFormControl:ResetMyBookList()");
                    EventDispatcher.Dispatch(EventEnum.BookProgressUpdate);
                    DoEnter();

                }
                else if (jo.code == 277)
                {
                    UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, jo.msg);
                    return;
                }
                else
                {
                    LOG.Error(jo.msg);
                }
            }, null);

        }
    }

    private void GetShopListCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetShopListCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.shopList = JsonHelper.JsonToObject<HttpInfoReturn<ShopListCont>>(result);
                }
                else if (jo.code == 277)
                {
                    UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, jo.msg);
                    return;
                }
            }, null);
        }
    }

    private void DoEnter()
    {

        CUIManager.Instance.CloseForm(UIFormName.SettingForm);
        if (UserDataManager.Instance.userInfo.data.userinfo.firstplay == 0)
        {
          
        }
        else
        {
            // CUIManager.Instance.OpenForm(UIFormName.MainForm/*, useFormPool: true*/);
            // var mainForm = CUIManager.Instance.GetForm<MainForm>(UIFormName.MainForm);
            // if (mainForm != null)
            //     mainForm.ResetMyBookList();

            //打开主界面
            XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:Open(logic.uiid.UIMainForm);");
            // EventDispatcher.Dispatch(EventEnum.BookJoinToShelfEvent);
            //刷新我的书本
            XLuaManager.Instance.GetLuaEnv().DoString(@"GameController.MainFormControl:ResetMyBookList()");
        }

        if (UserDataManager.Instance.thirdPartyLoginInfo != null && UserDataManager.Instance.thirdPartyLoginInfo.data != null &&
            UserDataManager.Instance.thirdPartyLoginInfo.data.isfirst == 1)
        {
            UserDataManager.Instance.thirdPartyLoginInfo.data.isfirst = 0;
            UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, GameDataMgr.Instance.table.GetLocalizationById(224)/*"First login Successful! You have already received 10 diamonds."*/, AlertType.Sure, null, "OK");
        }

    }

    private void LoadingButtonShow()
    {
        if (UserDataManager.Instance.LoginInfo != null)
        {
            if (FNameText == null) return;
            if (UserDataManager.Instance.LoginInfo.LastLoginChannel == 1 && UserDataManager.Instance.LoginInfo.FaceBookLoginInfo != null)
            {
                FNameText.text = "Log out";
                PlatformTypeText.text = "F";
                GNameText.text = "Sign In";
               
                FacebookButton.sprite = ResourceManager.Instance.GetUISprite(FbOn);
                GoolButton.sprite = ResourceManager.Instance.GetUISprite(GlOff);
            }
            else if (UserDataManager.Instance.LoginInfo.LastLoginChannel == 2 && UserDataManager.Instance.LoginInfo.GoogleLoginInfo != null)
            {
                FNameText.text = "Sign In";
                GNameText.text = "Log out";
               
                FacebookButton.sprite = ResourceManager.Instance.GetUISprite(FbOff);
                GoolButton.sprite = ResourceManager.Instance.GetUISprite(GlOn);
            }
            else if (UserDataManager.Instance.LoginInfo.LastLoginChannel == 3 && UserDataManager.Instance.hwLoginInfo != null)
            {
                FNameText.text = "Sign In";
                GNameText.text = "Log out";
               
                FacebookButton.sprite = ResourceManager.Instance.GetUISprite(FbOff);
                GoolButton.sprite = ResourceManager.Instance.GetUISprite(HwOn);
            }
            else
            {
                FNameText.text = "Sign In";
                PlatformTypeText.text = "F";
                GNameText.text = "Sign In";

#if CHANNEL_HUAWEI
                FacebookButton.sprite = ResourceManager.Instance.GetUISprite(FbOn);
                GoolButton.sprite = ResourceManager.Instance.GetUISprite(HwOn);
#else
                FacebookButton.sprite = ResourceManager.Instance.GetUISprite(FbOn);
                GoolButton.sprite = ResourceManager.Instance.GetUISprite(GlOn);
#endif
            }
        }
    }

    private void GoogleButOn(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        if (UserDataManager.Instance.LoginInfo != null)
        {
            if (UserDataManager.Instance.LoginInfo.LastLoginChannel == 2 && UserDataManager.Instance.LoginInfo.GoogleLoginInfo != null)
            {
#if CHANNEL_ONYX
                SdkMgr.Instance.google.Logout();
#endif
                GameHttpNet.Instance.TOKEN = string.Empty;
                ReLoadInfo();
            }
            else if (UserDataManager.Instance.LoginInfo.LastLoginChannel == 3 && UserDataManager.Instance.hwLoginInfo != null)
            {
                GameHttpNet.Instance.TOKEN = string.Empty;
                GameDataMgr.Instance.userData.RemoveAllBook();

                var Localization = GameDataMgr.Instance.table.GetLocalizationById(190);
                UITipsMgr.Instance.PopupTips(Localization, false);

                //UITipsMgr.Instance.PopupTips("Log out Successful!", false);
                UserDataManager.Instance.LoginInfo.LastLoginChannel = 0;
                UserDataManager.Instance.LogOutDelLocalInfo();

                LoadingButtonShow();

                // CUIManager.Instance.OpenForm(UIFormName.HwLoginForm);
            }
            else if (UserDataManager.Instance.LoginInfo.LastLoginChannel == 0 || UserDataManager.Instance.LoginInfo.LastLoginChannel == 4)
            {
                mCurLoginChannel = 0;
                //UINetLoadingMgr.Instance.Show();
#if CHANNEL_HUAWEI
                SdkMgr.Instance.hwSDK.Login(2);
#else
                SdkMgr.Instance.GoogleLogin(2);
#endif
            }
        }
    }

    private void GoogleLoginSuccHandler(Notification vNot)
    {
        LoginDataInfo loginInfo = vNot.Data as LoginDataInfo;
        if (loginInfo != null)
        {
            string userId = loginInfo.UserId;
            string tokenStr = loginInfo.Token;

            if (loginInfo.OpenType != 2) return;

            UserDataManager.Instance.UserData.UserID = userId;
            UserDataManager.Instance.UserData.IdToken = tokenStr;
            UserDataManager.Instance.UserData.UserName = loginInfo.UserName;


            if (UserDataManager.Instance.LoginInfo == null)
                UserDataManager.Instance.LoginInfo = new ThirdLoginData();

            mCurLoginChannel = 2;

            //Debug.Log("gogldengluchengg");

            LoginDataInfo glInfo = new LoginDataInfo();
            glInfo.UserId = userId;
            glInfo.UserName = loginInfo.UserName;
            glInfo.Email = loginInfo.Email;
            glInfo.Token = UserDataManager.Instance.UserData.IdToken;
            glInfo.UserImageUrl = loginInfo.UserImageUrl;
            UserDataManager.Instance.LoginInfo.GoogleLoginInfo = glInfo;



            LOG.Info("--- GoogleInfo  --id-->" + userId + " userNick:" + glInfo.UserName + " --token->" + UserDataManager.Instance.UserData.IdToken + "----Email--->" + glInfo.Email);

#if UNITY_ANDROID
#if CHANNEL_HUAWEI
            GameHttpNet.Instance.LoginByThirdParty(0, loginInfo.Email, loginInfo.UserName, loginInfo.UserImageUrl, userId, UserDataManager.Instance.UserData.IdToken, "huawei", LoginByThirdPartyCallBack);
#else
            GameHttpNet.Instance.LoginByThirdParty(0, loginInfo.Email, loginInfo.UserName, loginInfo.UserImageUrl, userId, UserDataManager.Instance.UserData.IdToken, "android", LoginByThirdPartyCallBack);
#endif
#else 
            GameHttpNet.Instance.LoginByThirdParty(0, loginInfo.Email, loginInfo.UserName, loginInfo.UserImageUrl, userId, UserDataManager.Instance.UserData.IdToken,"ios",LoginByThirdPartyCallBack);
#endif
        }
    }
    private void ThirdPartyLoginSuccHandler(Notification vNot)
    {
        LoginDataInfo loginInfo = vNot.Data as LoginDataInfo;
        if (loginInfo != null)
        {
            string userId = loginInfo.UserId;
            string tokenStr = loginInfo.Token;

            if (loginInfo.OpenType != 2) return;

            UserDataManager.Instance.UserData.UserID = userId;
            UserDataManager.Instance.UserData.IdToken = tokenStr;
            UserDataManager.Instance.UserData.UserName = loginInfo.UserName;


            if (UserDataManager.Instance.LoginInfo == null)
                UserDataManager.Instance.LoginInfo = new ThirdLoginData();

            // mCurLoginChannel = 2;

            LoginDataInfo glInfo = new LoginDataInfo();
            glInfo.UserId = userId;
            glInfo.UserName = loginInfo.UserName;
            glInfo.Email = loginInfo.Email;
            glInfo.Token = UserDataManager.Instance.UserData.IdToken;
            glInfo.UserImageUrl = loginInfo.UserImageUrl;
            UserDataManager.Instance.LoginInfo.ThirdPartyLoginInfo = glInfo;

            GameHttpNet.Instance.LoginByThirdParty(0, loginInfo.Email, loginInfo.UserName, loginInfo.UserImageUrl, userId, UserDataManager.Instance.UserData.IdToken, "android", LoginByThirdPartyCallBack);
            
        }
    }
    private void FaceBookLoginSuccHandler(Notification vNot)
    {
        LoginDataInfo loginInfo = vNot.Data as LoginDataInfo;

        if (loginInfo != null)
        {
            string userId = loginInfo.UserId;
            string tokenStr = loginInfo.Token;

            if (loginInfo.OpenType != 2) return;


            UserDataManager.Instance.UserData.UserID = userId;
            UserDataManager.Instance.UserData.IdToken = tokenStr;

            LOG.Info("---FaceBookLoginSuccHandler --userId-->" + userId + "===token--->" + tokenStr);

            SdkMgr.Instance.facebook.GetMyInfo(ReturnSelfFBInfo);

        }
        else
        {

        }
    }

    private void HuaweiLoginHandler(Notification vData)
    {
#if CHANNEL_HUAWEI
        hwUserInfo = vData.Data as HwUserInfo;
        if (hwUserInfo != null && hwUserInfo.type == 2)
        {
#if UNITY_EDITOR
            mCurLoginChannel = 3;
            UserDataManager.Instance.UserData.UserID = hwUserInfo.playerId;
            UserDataManager.Instance.UserData.IdToken = hwUserInfo.gameAuthSign;
            GameHttpNet.Instance.LoginByThirdParty(2, "", hwUserInfo.displayName, "", hwUserInfo.playerId, "huawei", "huawei", LoginByThirdPartyCallBack);
            return;
#endif
            GameHttpNet.Instance.CheckLoginInfo(1, hwUserInfo.playerId, hwUserInfo.playerLevel, hwUserInfo.gameAuthSign, hwUserInfo.ts, HwLoginCallBack);
        }

#endif
    }

    private void HwLoginCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----HwLoginCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    if (hwUserInfo != null)
                    {
                        mCurLoginChannel = 3;
                        UserDataManager.Instance.UserData.UserID = hwUserInfo.playerId;
                        UserDataManager.Instance.UserData.IdToken = hwUserInfo.gameAuthSign;
                        GameHttpNet.Instance.LoginByThirdParty(2, "", hwUserInfo.displayName, "", hwUserInfo.playerId, "huawei", "huawei", LoginByThirdPartyCallBack);
                    }
                }
            }, null);
        }
    }

    private void ReturnSelfFBInfo(string vMsg)
    {
        LOG.Info("---return Self FB Info ---->" + vMsg);

        Dictionary<string, string> selfInfo = JsonHelper.JsonToObject<Dictionary<string, string>>(vMsg);
        string userId = string.Empty;
        string userNick = string.Empty;
        string email = string.Empty;
        string faceUrl = string.Empty;
        if (selfInfo != null)
        {
            if (selfInfo.ContainsKey("id"))
                userId = selfInfo["id"];
            if (selfInfo.ContainsKey("name"))
                userNick = selfInfo["name"];

            UserDataManager.Instance.UserData.UserName = userNick;

            if (UserDataManager.Instance.LoginInfo == null)
                UserDataManager.Instance.LoginInfo = new ThirdLoginData();

            mCurLoginChannel = 1;
            LoginDataInfo fbInfo = new LoginDataInfo();
            fbInfo.UserId = userId;
            fbInfo.UserName = userNick;
            fbInfo.Email = email;
            fbInfo.Token = UserDataManager.Instance.UserData.IdToken;
            fbInfo.UserImageUrl = faceUrl;
            UserDataManager.Instance.LoginInfo.FaceBookLoginInfo = fbInfo;

            LOG.Info("--- Self FB  --id-->" + userId + " userNick:" + userNick + " --token->" + UserDataManager.Instance.UserData.IdToken);

            //UINetLoadingMgr.Instance.Show();

#if UNITY_ANDROID
#if CHANNEL_HUAWEI
            GameHttpNet.Instance.LoginByThirdParty(1, email, userNick, faceUrl, userId, UserDataManager.Instance.UserData.IdToken, "huawei", LoginByThirdPartyCallBack);
#else
            GameHttpNet.Instance.LoginByThirdParty(1, email, userNick, faceUrl, userId, UserDataManager.Instance.UserData.IdToken, "android", LoginByThirdPartyCallBack);
#endif
#else 
            GameHttpNet.Instance.LoginByThirdParty(1,email,userNick,faceUrl,userId,UserDataManager.Instance.UserData.IdToken,"ios",LoginByThirdPartyCallBack);
#endif  

        }
    }
    private void LoginByThirdPartyCallBack(object arg, EnumReLogin loginType)
    {
        string result = arg.ToString();
        LOG.Info("----LoginByThirdPartyCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                UserDataManager.Instance.LoginInfo.LastLoginChannel = mCurLoginChannel;
                if (jo.code == 200)
                {
                    UserDataManager.Instance.thirdPartyLoginInfo = JsonHelper.JsonToObject<HttpInfoReturn<ThirdPartyReturnInfo>>(result);
                    if (UserDataManager.Instance.thirdPartyLoginInfo != null && UserDataManager.Instance.thirdPartyLoginInfo.data != null)
                    {
                        GameHttpNet.Instance.TOKEN = UserDataManager.Instance.thirdPartyLoginInfo.data.token;
                    }

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(172);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Login Successful!", false);

                    GameHttpNet.Instance.GetUserInfo(GetUserInfoCallBack);

                    //清除书架之前账号所保存的所有书本的数据
                    GameDataMgr.Instance.userData.MyBookListClean();
                }
                else if (jo.code == 201)
                {
                    //UINetLoadingMgr.Instance.Close();
                    LOG.Info("--LoginByThirdPartyCallBack--->参数不完整");

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(173);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("参数不完整");
                }
                else if (jo.code == 208)
                {
                    //UINetLoadingMgr.Instance.Close();
                    LOG.Info("--LoginByThirdPartyCallBack--->登录失败");

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(174);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Login failed.");
                }
                else if (jo.code == 277)
                {
                    UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, jo.msg);
                    return;
                }

                UserDataManager.Instance.SaveLoginInfo();

            }, null);
        }
    }

}

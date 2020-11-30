using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using pb;

public class SettingForm : BaseUIForm
{
    public RectTransform frameTrans;
    public GameObject UIMask;
    public Image NotificationBtn;
    public Image EarnRewardBtn;
    public Button FBLoginBtn,GoogleBtn;
    public GameObject HelpBtn;
    public GameObject TermsBtn;
    public GameObject PrivacyBtn;
    public Text PlatformTypeText;
    public Text FNameText, GNameText;
    public Image NickNameGroup;
    public Text NickNameTxt;
    public InputField inputFieldText;
    public Image FbSwitch, GlSwitch;
    public Image UseBtn;
    public InputField CodeInputField;

    private int mCurLoginChannel = 0;
    private string OnIconPath = "SettingForm/btn_on";
    private string OffIconPath = "SettingForm/btn_off";

    public InputField InputField;
    public Image sendButton,platformImage;
    private HwUserInfo hwUserInfo;

    private Image GlFlagIcon;
    private Text PlayGamesTxt;

    private string InputString = "";
    private const string sendOn = "SettingForm/bg_klzd_03";
    private const string sendOff = "SettingForm/bg_jsle_03";

    private const string FbOn = "SettingForm/btn_fackbook";
    private const string FbOff = "SettingForm/btn_fackbook1";
    private const string GlOn = "SettingForm/btn_google";
    private const string GlOff = "SettingForm/btn_google1";
    private const string HwOn = "SettingForm/btn_huawei1";
    private const string HwOff = "SettingForm/btn_huawai2";
    private const string GLFlagIconPath = "SettingForm/btn_googles";
    private const string HwFlagIconPath = "SettingForm/btn_huawei";

    private RectTransform top;
    private GameObject Close;

    private void Awake()
    {
        top = transform.Find("top").GetComponent<RectTransform>();
        Close = transform.Find("top/Close").gameObject;
    }

    public override void OnOpen()
    {
        base.OnOpen();

        //【屏幕适配】
        float offect = XLuaHelper.UnSafeAreaNotFit(this.myForm, null, 750, 120);
        var t = top.rectTransform();
        var size = t.sizeDelta;
        size.y += offect;
        t.sizeDelta = size;



        FBLoginBtn.onClick.AddListener(OnLoginClick);
        GoogleBtn.gameObject.SetActive(false);
#if UNITY_ANDROID
        GoogleBtn.onClick.AddListener(GoogleButOn);
        GoogleBtn.gameObject.SetActive(true);
#endif

        UIEventListener.AddOnClickListener(UIMask, backToMainForm);
        UIEventListener.AddOnClickListener(NotificationBtn.gameObject, NotificationHandler);
        UIEventListener.AddOnClickListener(EarnRewardBtn.gameObject, EarnRewardHandler);
        //UIEventListener.AddOnClickListener(FBLoginBtn, facebookLoginHandler);
        UIEventListener.AddOnClickListener(HelpBtn, HelpHandler);
        UIEventListener.AddOnClickListener(TermsBtn, TermsHandler);
        UIEventListener.AddOnClickListener(PrivacyBtn, PrivacyHandler);
        UIEventListener.AddOnClickListener(sendButton.gameObject, SendButtonOnClicke);

        UIEventListener.AddOnClickListener(Close, CloseButtonOn);

        if(UseBtn != null)
            UIEventListener.AddOnClickListener(UseBtn.gameObject, UseCodeHandler);

        addMessageListener(EventEnum.FaceBookLoginSucc, FaceBookLoginSuccHandler);
        addMessageListener(EventEnum.GoogleLoginSucc, GoogleLoginSuccHandler);
        addMessageListener(EventEnum.HuaweiLoginInfo, HuaweiLoginHandler);

        if (UseBtn == null && GameUtility.IpadAspectRatio() && frameTrans != null)
        {
            this.frameTrans.localScale = new Vector3(0.7f, 0, 1);
            this.frameTrans.DOScaleY(0.7f, 0.25f).SetEase(Ease.OutBack).Play();
        }
        else
        {
            this.frameTrans.localScale = new Vector3(1, 0, 1);
            this.frameTrans.DOScaleY(1, 0.25f).SetEase(Ease.OutBack).Play();
        }

        Transform glFlagIconTrans = frameTrans.Find("BG/SettingBG/Google/Image");
        if (glFlagIconTrans != null)
            GlFlagIcon = glFlagIconTrans.GetComponent<Image>();

        Transform platformNameTxtTrans = frameTrans.Find("BG/SettingBG/Google/Text");
        if (platformNameTxtTrans != null)
            PlayGamesTxt = platformNameTxtTrans.GetComponent<Text>();

#if CHANNEL_HUAWEI
        if(GlFlagIcon != null)
            GlFlagIcon.sprite = ResourceManager.Instance.GetUISprite(HwFlagIconPath);
        if (PlayGamesTxt != null)
            PlayGamesTxt.text = "Huawei";

        if(platformImage!=null)
           platformImage.sprite= ResourceManager.Instance.GetUISprite("SettingForm/btn_huawei");
        
#else
        if (GlFlagIcon != null)
            GlFlagIcon.sprite = ResourceManager.Instance.GetUISprite(GLFlagIconPath);
        if (PlayGamesTxt != null)
            PlayGamesTxt.text = "Play Games";
        if (platformImage != null)
            platformImage.sprite = ResourceManager.Instance.GetUISprite("SettingForm/btn_googles");

#endif

        ShowNickName(false);
        LoadingButtonShow();

        if (inputFieldText != null)
        {
            string UUID = GameHttpNet.Instance.UUID;

#if !UNITY_EDITOR
            //UUID = UserDataManager.Instance.UserData.JPushId;
            if(string.IsNullOrEmpty(UUID))
                UUID = GameHttpNet.Instance.UUID;
#endif

            //PhoneUUID.text = "ID:" + GameHttpNet.Instance.UUID;
            inputFieldText.text = "ID:" + UUID;
            inputFieldText.onValueChanged.AddListener(delegate
            {
                inputFieldText.text = "ID:" + UUID;
            });
        }

        ResetBtnState();


        InputField.onValueChanged.AddListener(InputChangeHandler);
    }

    public override void OnClose()
    {
        base.OnClose();

        FBLoginBtn.onClick.RemoveListener(OnLoginClick);
        GoogleBtn.onClick.RemoveListener(GoogleButOn);

        UIEventListener.RemoveOnClickListener(UIMask, backToMainForm);
        UIEventListener.RemoveOnClickListener(NotificationBtn.gameObject, NotificationHandler);
        UIEventListener.RemoveOnClickListener(EarnRewardBtn.gameObject, EarnRewardHandler);
        UIEventListener.RemoveOnClickListener(HelpBtn, HelpHandler);
        UIEventListener.RemoveOnClickListener(TermsBtn, TermsHandler);
        UIEventListener.RemoveOnClickListener(PrivacyBtn, PrivacyHandler);
        UIEventListener.RemoveOnClickListener(sendButton.gameObject, SendButtonOnClicke);
        UIEventListener.RemoveOnClickListener(Close, CloseButtonOn);


        InputField.onValueChanged.RemoveListener(InputChangeHandler);
    }

    private void CloseButtonOn(PointerEventData data)
    {
        CUIManager.Instance.CloseForm(UIFormName.SettingNav);
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
                NickNameTxt.text = UserDataManager.Instance.LoginInfo.FaceBookLoginInfo.UserName;
                ShowNickName(true);

                FbSwitch.sprite = ResourceManager.Instance.GetUISprite(FbOn);
                GlSwitch.sprite = ResourceManager.Instance.GetUISprite(GlOff);
            }
            else if (UserDataManager.Instance.LoginInfo.LastLoginChannel == 2 && UserDataManager.Instance.LoginInfo.GoogleLoginInfo != null)
            {
                FNameText.text = "Sign In";
                GNameText.text = "Log out";
                NickNameTxt.text = UserDataManager.Instance.LoginInfo.GoogleLoginInfo.UserName;
                ShowNickName(true);

                FbSwitch.sprite = ResourceManager.Instance.GetUISprite(FbOff);
                GlSwitch.sprite = ResourceManager.Instance.GetUISprite(GlOn);
            }
            else if (UserDataManager.Instance.LoginInfo.LastLoginChannel == 3 && UserDataManager.Instance.hwLoginInfo != null)
            {
                FNameText.text = "Sign In";
                GNameText.text = "Log out";
                NickNameTxt.text = UserDataManager.Instance.hwLoginInfo.displayName;
                ShowNickName(true);

                FbSwitch.sprite = ResourceManager.Instance.GetUISprite(FbOff);
                GlSwitch.sprite = ResourceManager.Instance.GetUISprite(HwOn);
            }
            else
            {
                FNameText.text = "Sign In";
                PlatformTypeText.text = "F";
                GNameText.text = "Sign In";
                NickNameTxt.text = "";

#if CHANNEL_HUAWEI
                FbSwitch.sprite = ResourceManager.Instance.GetUISprite(FbOn);
                GlSwitch.sprite = ResourceManager.Instance.GetUISprite(HwOn);
#else
                FbSwitch.sprite = ResourceManager.Instance.GetUISprite(FbOn);
                GlSwitch.sprite = ResourceManager.Instance.GetUISprite(GlOn);
#endif
            }
        }
    }

    private void InputChangeHandler(string vStr)
    {
        InputString = InputField.text;
        if (InputString.Length >= 10)
        {
            sendButton.sprite = ResourceManager.Instance.GetUISprite(sendOn);
        }
        else if (InputString.Length < 10)
        {
            sendButton.sprite = ResourceManager.Instance.GetUISprite(sendOff);
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
        //OnCloseClick();
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
            GameHttpNet.Instance.LoginByThirdParty(1,email,userNick,faceUrl,userId,UserDataManager.Instance.UserData.IdToken,"android",LoginByThirdPartyCallBack);
#endif
#else 
            GameHttpNet.Instance.LoginByThirdParty(1,email,userNick,faceUrl,userId,UserDataManager.Instance.UserData.IdToken,"ios",LoginByThirdPartyCallBack);
#endif  

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
    private void ShowNickName(bool value)
    {
       //NickNameGroup.gameObject.SetActive(value);
       //if (value)
       //    FBLoginBtn.rectTransform().anchoredPosition = new Vector2(208, 10);
       //else
       //    FBLoginBtn.rectTransform().anchoredPosition = new Vector2(208, 0);
    }

    private void ResetBtnState()
    {
        if (UserDataManager.Instance.UserData.BgMusicIsOn == 1)
            NotificationBtn.sprite = ResourceManager.Instance.GetUISprite(OnIconPath);
        else
            NotificationBtn.sprite = ResourceManager.Instance.GetUISprite(OffIconPath);

        if (UserDataManager.Instance.UserData.TonesIsOn == 1)
            EarnRewardBtn.sprite = ResourceManager.Instance.GetUISprite(OnIconPath);
        else
            EarnRewardBtn.sprite = ResourceManager.Instance.GetUISprite(OffIconPath);
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

    private void HelpHandler(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CUIManager.Instance.OpenForm(UIFormName.HelpSupportForm);
        var view = CUIManager.Instance.GetForm<HelpSupportForm>(UIFormName.HelpSupportForm);
        view.TexteShow("Help");
    }

    private void TermsHandler(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CUIManager.Instance.OpenForm(UIFormName.HelpSupportForm);
        var view = CUIManager.Instance.GetForm<HelpSupportForm>(UIFormName.HelpSupportForm);
        view.TexteShow("Terms");
    }

    private void PrivacyHandler(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CUIManager.Instance.OpenForm(UIFormName.HelpSupportForm);
        var view = CUIManager.Instance.GetForm<HelpSupportForm>(UIFormName.HelpSupportForm);
        view.TexteShow("Privacy");
    }

    private void LoginSuccHandler(string vUserID,string vToken)
    {
        LOG.Info("-----Facebook login succ-----userId:" + vUserID + " vToken:" + vToken);
        //FacebookLogic.Instance.GetMyInfo(GetMyInfoHandler);
    }

    private void GetMyInfoHandler(string vMsg)
    {
        LOG.Info("-----Facebook GetMyInfoHandler-----" + vMsg);
    }

    private void LoginErrorHandler(string vErrMsg)
    {
        LOG.Info("-----Facebook login fail-----" + vErrMsg);
    }

    private void backToMainForm(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        LOG.Info("backToMainForm");
        CUIManager.Instance.CloseForm(UIFormName.SettingForm);
    }
    
    private void OnLoginClick()
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

    private void GoogleButOn()
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
            else if(UserDataManager.Instance.LoginInfo.LastLoginChannel == 3 && UserDataManager.Instance.hwLoginInfo != null)
            {
                GameHttpNet.Instance.TOKEN = string.Empty;
                GameDataMgr.Instance.userData.RemoveAllBook();

                var Localization = GameDataMgr.Instance.table.GetLocalizationById(190);
                UITipsMgr.Instance.PopupTips(Localization, false);

                //UITipsMgr.Instance.PopupTips("Log out Successful!", false);
                UserDataManager.Instance.LoginInfo.LastLoginChannel = 0;
                UserDataManager.Instance.LogOutDelLocalInfo();

                LoadingButtonShow();
                CUIManager.Instance.CloseForm(UIFormName.SettingNav);
                CUIManager.Instance.OpenForm(UIFormName.HwLoginForm);
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
    
    private void ReLoadInfo()
    {
        ShowNickName(false); 
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
                        UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.userInfo.data.userinfo.bkey,false);
                        UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.userInfo.data.userinfo.diamond,false);
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
        CUIManager.Instance.CloseForm(UIFormName.SettingNav);
        if (UserDataManager.Instance.userInfo.data.userinfo.firstplay == 0)
        {
            CUIManager.Instance.OpenForm(UIFormName.GuideForm/*, useFormPool: true*/);
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

    private void UseCodeHandler(PointerEventData data)
    {
        if(string.IsNullOrEmpty(CodeInputField.text))
        {
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(176);
            UITipsMgr.Instance.PopupTips(Localization, false);

            //UITipsMgr.Instance.PopupTips("Place enter  invitation codes", false);
            return;
        }
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.InviteExchange(CodeInputField.text, EnterCodeHandler);
    }

    private void EnterCodeHandler(object value)
    {
        string result = value.ToString();
        LOG.Info("-----EnterCodeHandler---->" + result);
        if (result.Equals("error"))
        {
            LOG.Info("---EnterCodeHandler--这条协议错误");
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
                    UserDataManager.Instance.receiveInviteResult = JsonHelper.JsonToObject<HttpInfoReturn<ReceiveInviteResult>>(value.ToString());
                    if (UserDataManager.Instance.receiveInviteResult != null && UserDataManager.Instance.receiveInviteResult.data != null)
                    {
                        int getKey = UserDataManager.Instance.receiveInviteResult.data.bkey - UserDataManager.Instance.UserData.KeyNum;
                        int getDiamond = UserDataManager.Instance.receiveInviteResult.data.diamond - UserDataManager.Instance.UserData.DiamondNum;

                        UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.receiveInviteResult.data.bkey);
                        UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.receiveInviteResult.data.diamond);

                        string tips = "";
                        if (getKey > 0 && getDiamond > 0)
                        {
                            tips = "Congratulations on getting " + getDiamond + " diamonds and " + getKey + " keys";
                        }
                        else if (getKey > 0)
                        {
                            tips = "Congratulations on getting " + getKey + " keys";
                        }
                        else if (getDiamond > 0)
                        {
                            tips = "Congratulations on getting " + getDiamond + " diamonds";
                        }

                        if (!string.IsNullOrEmpty(tips))
                            UITipsMgr.Instance.PopupTips(tips, false);

                        int bookId = UserDataManager.Instance.receiveInviteResult.data.bookid;
                        if (bookId > 0 && UserDataManager.Instance.UserData.bookList.IndexOf(bookId) == -1)
                        {
                            UserDataManager.Instance.UserData.bookList.Add(bookId);
                            CUIManager.Instance.OpenForm(UIFormName.InviteGiftBag);
                            InvitGiftBag tem = CUIManager.Instance.GetForm<InvitGiftBag>(UIFormName.InviteGiftBag);
                            if (tem != null)
                                tem.Inite(bookId);
                        }

                        //UIAlertMgr.Instance.Show("TIPS", "Invitation Code Used Successfully，get 1 diamond");
                    }
                }
                else if (jo.code == 205)
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(177);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("You can't use your own invitation code.", false);
                }
                else if(jo.code == 203)
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(178);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("This invitation code has been used.", false);
                }
                else if (jo.code == 202)
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(179);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Maximum number of invitations used today.", false);
                }
                else if (jo.code == 201)
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(180);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("This invitation code does not exist.", false);
                }
            }

        }, null);
    }

   
    private void SendButtonOnClicke(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        InputString = InputField.text;

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
        GameHttpNet.Instance.UserFeedback(4,"",InputString, UserFeedbackCallBack);
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
                        InputField.text = "";

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
                }else
                {
                    //UINetLoadingMgr.Instance.Close();

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(165);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Error, send again!", false);
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                }              
            }, null);       
    }
}

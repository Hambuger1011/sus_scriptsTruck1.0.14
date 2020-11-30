using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UGUI;
using UnityEngine.UI;
using pb;
using System.IO;

public class Navigation : BaseUIForm {

    public RectTransform BG,Mask;
    public GameObject[] ButtonOnShow;
    public GameObject[] UIpanl;
    public GameObject PersonCenter,Email, News, Share, Setting, FAQ,FacebookBtn,HeadBg;
    private EmailForm EmailForm;
    public GameObject HelpBtn;
    public GameObject TermsBtn;
    public GameObject PrivacyBtn;
    public InputField inputFieldText, UseNameInputField;
    public GameObject EmailHit, NewHit;
    public Text EmailHitText, NewHitText,VersionStr,FacebookTxt;
    public FAQSprite FAQSprite;
    public GameObject Tou0, Tou1, Tou2;
    public Image tou0Image, tou1Image, tou2Image,touTile,VipFrameImg;
    public RectTransform TouBGmove;
    public GameObject FaceBookGo, FBFirstLoginDesc;

    private bool touMove = false, MoveCompleted=false;
    private int NowTypeNumber = -1;
    private int NowTouNumber = 0;
    private int mCurLoginChannel = 0;
    private HwUserInfo hwUserInfo;
    public Text FaceBookDescText;

    public override void OnOpen()
    {
        base.OnOpen();
        ButtonOnShowToTrueOrFalse(-1);
        UIpanlOnTrue(-1);
        NowTypeNumber = -1;

        BG = transform.Find("Frame/BG").GetComponent<RectTransform>();
        Mask = transform.Find("Frame/Mask").GetComponent<RectTransform>();
       
        //UIEventListener.AddOnClickListener(PersonCenter, PersonCenterButtonOn);
        UIEventListener.AddOnClickListener(Email, EmailButtonOn);
        UIEventListener.AddOnClickListener(News, NewsButtonOn);
        UIEventListener.AddOnClickListener(Share, ShareButtonOn);
        UIEventListener.AddOnClickListener(Setting, SettingButtonOn);
        UIEventListener.AddOnClickListener(FAQ, FAQButtonOn);
        UIEventListener.AddOnClickListener(HelpBtn, HelpHandler);
        UIEventListener.AddOnClickListener(TermsBtn, TermsHandler);
        UIEventListener.AddOnClickListener(PrivacyBtn, PrivacyHandler);
        UIEventListener.AddOnClickListener(Mask.gameObject, MaskButtonOn);
        UIEventListener.AddOnClickListener(FacebookBtn, FacebookHandler);

        //UIEventListener.AddOnClickListener(HeadBg, Tou0ButtonOnclick);
        //UIEventListener.AddOnClickListener(Tou0, Tou0ButtonOnclick);
        UIEventListener.AddOnClickListener(Tou1, Tou1ButtonOnclick);
        UIEventListener.AddOnClickListener(Tou2,Tou2ButtonOnclick);

        UseNameInputField.onEndEdit.AddListener(ChangeUserName);

        addMessageListener(EventEnum.NavigationClose, NavigationClose);
        addMessageListener(EventEnum.FaceBookLoginSucc, FaceBookLoginSuccHandler);
        addMessageListener(EventEnum.GoogleLoginSucc, GoogleLoginSuccHandler);
        addMessageListener(EventEnum.HuaweiLoginInfo, HuaweiLoginHandler);
        addMessageListener(EventEnum.HwLoginFormUpdat, HwLoginFormUpdat);


        Init();
        if (inputFieldText != null)
        {
            string UUID = UserDataManager.Instance.userInfo.data.userinfo.phoneimei;

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

        if (string.IsNullOrEmpty(UserDataManager.Instance.userInfo.data.userinfo.nickname))
        {
            UseNameInputField.text = "Guest";
        }else
        {
            UseNameInputField.text = UserDataManager.Instance.userInfo.data.userinfo.nickname;
        }

        
        NowTouNumber = UserDataManager.Instance.userInfo.data.userinfo.avatar;

        if (UserDataManager.Instance.userInfo != null)
        {
            if (tou0Image != null)
            {
                //【调用lua公共方法 加载头像】   -1代码当前装扮的头像    
                XLuaManager.Instance.CallFunction("GameHelper", "ShowAvatar", -1, tou0Image);
            }

            if (VipFrameImg != null)
            {
                //【调用lua公共方法 加载头像框】   -1代码当前装扮的头像框   
                XLuaManager.Instance.CallFunction("GameHelper", "ShowAvatarframe", -1, VipFrameImg);
            }
        }


        UpdateLoginState();

#if CHANNEL_SPAIN
        this.PersonCenter.CustomSetActive(false);
        this.News.CustomSetActive(false);
        this.Share.CustomSetActive(false);
#endif
    }

    public override void OnClose()
    {
        base.OnClose();
       
        UIEventListener.RemoveOnClickListener(Email, EmailButtonOn);
        UIEventListener.RemoveOnClickListener(News, NewsButtonOn);
        UIEventListener.RemoveOnClickListener(Share, ShareButtonOn);
        UIEventListener.RemoveOnClickListener(Setting, SettingButtonOn);
        UIEventListener.RemoveOnClickListener(FAQ, FAQButtonOn); UIEventListener.AddOnClickListener(HelpBtn, HelpHandler);
        UIEventListener.RemoveOnClickListener(HelpBtn, HelpHandler);
        UIEventListener.RemoveOnClickListener(TermsBtn, TermsHandler);
        UIEventListener.RemoveOnClickListener(PrivacyBtn, PrivacyHandler);
        UIEventListener.RemoveOnClickListener(FacebookBtn, FacebookHandler);

       
        if (MoveCompleted)
        {
            UIEventListener.RemoveOnClickListener(HeadBg, Tou0ButtonOnclick);
            UIEventListener.RemoveOnClickListener(Tou0, Tou0ButtonOnclick);
            UIEventListener.RemoveOnClickListener(PersonCenter, PersonCenterButtonOn);
        }

        UIEventListener.RemoveOnClickListener(Tou1, Tou1ButtonOnclick);
        UIEventListener.RemoveOnClickListener(Tou2, Tou2ButtonOnclick);

        UseNameInputField.onEndEdit.RemoveListener(ChangeUserName);
    }

    /// <summary>
    /// 第三方登录界面，登录的时候刷新侧边栏 的状态
    /// </summary>
    /// <param name="notification"></param>
    public void HwLoginFormUpdat(Notification notification)
    {
        UpdateLoginState();

        if (string.IsNullOrEmpty(UserDataManager.Instance.userInfo.data.userinfo.nickname))
        {
            UseNameInputField.text = "Guest";
        }
        else
        {
            UseNameInputField.text = UserDataManager.Instance.userInfo.data.userinfo.nickname;
        }
    }
    private void UpdateLoginState()
    {
        if (FBFirstLoginDesc == null) return;
        FBFirstLoginDesc.gameObject.SetActive(false);
        if (UserDataManager.Instance.LoginInfo != null)
        {
            FacebookBtn.gameObject.SetActive(true);
            if (UserDataManager.Instance.LoginInfo.LastLoginChannel == 1 && UserDataManager.Instance.LoginInfo.FaceBookLoginInfo != null)
            {
                FacebookBtn.GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("Navigation/btn_facebook");
                FacebookTxt.text = "Log  out";
            }
            else if (UserDataManager.Instance.LoginInfo.LastLoginChannel == 0 || UserDataManager.Instance.LoginInfo.LastLoginChannel == 4)
            {
#if CHANNEL_HUAWEI
                FacebookBtn.GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("Navigation/btn_huawei");
                FacebookTxt.text = "Huawei";
                FaceBookDescText.text = "Sign in with Huawei Get <color=#52d3fe>10 diamond</color> for your first login!";
                FBFirstLoginDesc.gameObject.SetActive(false);
#else
                FacebookBtn.GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("Navigation/btn_facebook");
                FacebookTxt.text = "Facebook";
                FaceBookDescText.text = "Sign in with Facebook Get <color=#52d3fe>10 diamond</color> for your first login!";
                FBFirstLoginDesc.gameObject.SetActive(true);
#endif

            }
            else if (UserDataManager.Instance.LoginInfo.LastLoginChannel == 3)
            {
                FacebookBtn.GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("Navigation/btn_huawei");
                FacebookTxt.text = "Log  out";
            }
            else if (UserDataManager.Instance.LoginInfo.LastLoginChannel == 2)
            {
                FacebookBtn.gameObject.SetActive(false);
            }

            FaceBookGo.gameObject.SetActive(UserDataManager.Instance.LoginInfo.LastLoginChannel != 2);
        }
    }

    private void ChangeUserName(string vName)
    {
        if (UserDataManager.Instance.userInfo.data.userinfo.nickname.ToString().Equals(vName))
        {
            return;
        }
        else
        {
            if (vName.Length < 1 || vName.Length > 20)
            {
                UseNameInputField.text = UserDataManager.Instance.userInfo.data.userinfo.nickname;
                return;
            }

            if (vName.Equals("Guest")) return;

            //LOG.Info("名字按下");
            GameHttpNet.Instance.SetUserLanguage(UseNameInputField.text.ToString(), 2, ChangeNameCallBackHandler);
        }
    }

    private void Tou0ButtonOnclick(PointerEventData data)
    {
        if (!CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop).AniPlayEndReturn())
        {
            return;
        }


        //CUIManager.Instance.CloseForm(UIFormName.Navigation);
        //EventDispatcher.Dispatch(EventEnum.MainFormMove, 2);
        CUIManager.Instance.OpenForm(UIFormName.ProfileForm);
        //EventDispatcher.Dispatch(EventEnum.AddOpenFormType, 9);
        return;

        if (!touMove)
        {
            //头像移出
            TouBGmove.DOAnchorPosX(-23, 0.5f);

            TouSpriteChange();

            TouMoveF(1);
        }
        else
        {
            //头像缩进
           
            TouMoveF(2);
        }
        touMove =!touMove;
    }

    private void Tou1ButtonOnclick(PointerEventData data)
    {
        string name = tou1Image.sprite.name;
        NowTouNumber = int.Parse(name);
        //Debug.Log("name:" + NowTouNumber);

        TouMoveF(2);
    }
    private void Tou2ButtonOnclick(PointerEventData data)
    {
        string name = tou2Image.sprite.name;
        NowTouNumber = int.Parse(name);
        //Debug.Log("name:" + NowTouNumber);
        TouMoveF(2);
    }

    /// <summary>
    /// 头像根据选择而变动 ***=***
    /// </summary>
    private void TouSpriteChange()
    {
        bool tou1 = false;
        bool tou2 = false;
        for (int i=0;i<3; i++)
        {           
            if (i == UserDataManager.Instance.userInfo.data.userinfo.avatar - 1)
            {
                tou0Image.sprite = ResourceManager.Instance.GetUISprite("Navigation/" + UserDataManager.Instance.userInfo.data.userinfo.avatar.ToString());
                //Debug.Log("tou0:" + i + 1);
            }
            else
            {
                if (!tou1)
                {
                    tou1 = true;
                    tou1Image.sprite = ResourceManager.Instance.GetUISprite("Navigation/" + (i + 1).ToString());
                    //Debug.Log("tou1:" + (i + 1));
                }
                else if (!tou2)
                {
                    tou2 = true;
                    tou2Image.sprite = ResourceManager.Instance.GetUISprite("Navigation/" + (i + 1).ToString());
                    //Debug.Log("tou2:" + (i + 1));
                }
            }
        }
    }

    private void TouMoveF(int moveType)
    {
        if (moveType==1)
        {
            //头像移出
            TouBGmove.DOAnchorPosX(-23, 0.5f);
        }
        else
        {

            GameHttpNet.Instance.SetUserLanguage(NowTouNumber.ToString(), 4, SetFaceidCallBackHandler);
        }
    }

    private void SetFaceidCallBackHandler(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----SetFaceidCallBackHandler---->" + result);
        if (result.Equals("error"))
        {
            LOG.Info("设置头像id失败，协议返回错误");
            return;
        }

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            if (jo.code == 200)
            {
                //头像缩进
                TouBGmove.DOAnchorPosX(-300, 0.5f).Play().OnComplete(() => {
                    touMove = false;
                   
                    UserDataManager.Instance.userInfo.data.userinfo.avatar = NowTouNumber;
                    TouSpriteChange();
                });
            }
        }
    }

    private void ChangeNameCallBackHandler(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----ChangeNameCallBackHandler---->" + result);
        if (result.Equals("error"))
        {
            LOG.Info("设置名字失败，协议返回错误");
            return;
        }

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) => 
            {
                if (jo.code == 200)
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(189);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Name changed successfully!", false);

                    UserDataManager.Instance.userInfo.data.userinfo.nickname = UseNameInputField.text.ToString();
                }
            }, null);
           
        }
    }
    public void NavigationClose(Notification notification)
    {
        LOG.Info("进入书本时有侧边栏，关闭侧边栏");
        CUIManager.Instance.CloseForm(UIFormName.Navigation);
        EventDispatcher.Dispatch(EventEnum.MainFormMove, 2);
    }

   

    private void MaskButtonOn(PointerEventData data)
    {
        if (!CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop).AniPlayEndReturn())
        {
            return;
        }

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

    private void PersonCenterButtonOn(PointerEventData data)
    {
        if (!CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop).AniPlayEndReturn())
        {
            return;
        }
        UIpanlOnTrue(0);


        //CUIManager.Instance.CloseForm(UIFormName.Navigation);
        //EventDispatcher.Dispatch(EventEnum.MainFormMove, 2);
        CUIManager.Instance.OpenForm(UIFormName.ProfileForm);
        //EventDispatcher.Dispatch(EventEnum.AddOpenFormType, 9);
    }
    private void EmailButtonOn(PointerEventData data)
    {
        if (!CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop).AniPlayEndReturn())
        {
            return;
        }
        UIpanlOnTrue(1);
        //if (!EmailForm.gameObject.activeSelf) return;
        CUIManager.Instance.OpenForm(UIFormName.EmailForm);

        ButtonOnShowToTrueOrFalse(0);
        //BgMoveOut();
        EmailForm = CUIManager.Instance.GetForm<EmailForm>(UIFormName.EmailForm);
        EmailForm.MailBoxTochAreButton(null);

        EventDispatcher.Dispatch(EventEnum.GetEmailShowHint, 5);
    }
    private void NewsButtonOn(PointerEventData data)
    {
        if (!CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop).AniPlayEndReturn())
        {
            return;
        }
        UIpanlOnTrue(2);
        //if (!EmailForm.gameObject.activeSelf) return;
        CUIManager.Instance.OpenForm(UIFormName.EmailForm);

        ButtonOnShowToTrueOrFalse(1);
        //BgMoveOut();       
        EmailForm = CUIManager.Instance.GetForm<EmailForm>(UIFormName.EmailForm);
        EmailForm.NewTochAreButton(null);
        EventDispatcher.Dispatch(EventEnum.GetEmailShowHint,6);
    }

    private void ShareButtonOn(PointerEventData data)
    {
        if (!CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop).AniPlayEndReturn())
        {
            return;
        }
        UIpanlOnTrue(3);
        ButtonOnShowToTrueOrFalse(2);     
        //EventDispatcher.Dispatch(EventEnum.GetEmailShowHint,2);
        //BgMoveOut();
       
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        TalkingDataManager.Instance.ShareRecord(1);

        string linkUrl = "";
#if UNITY_ANDROID
        linkUrl = "https://play.google.com/store/apps/details?id=" + SdkMgr.packageName;
#endif
#if UNITY_IOS
        linkUrl = "https://www.facebook.com/Scripts-Untold-Secrets-107729237761206/";
#endif
    }

    private void FBShareLinkFaild(bool isCancel, string errorInfo)
    {
        var Localization = GameDataMgr.Instance.table.GetLocalizationById(143);
        UITipsMgr.Instance.PopupTips(Localization, false);

        //UITipsMgr.Instance.PopupTips("Share Failed!", false);
    }
    private void SettingButtonOn(PointerEventData data)
    {
        if (!CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop).AniPlayEndReturn())
        {
            return;
        }
        UIpanlOnTrue(4);
        ButtonOnShowToTrueOrFalse(3);     
        EventDispatcher.Dispatch(EventEnum.GetEmailShowHint,3);
        //BgMoveOut();
        //UIpanlOnTrue(1);

        CUIManager.Instance.OpenForm(UIFormName.SettingNav);

    }
    private void FAQButtonOn(PointerEventData data)
    {
        if (!CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop).AniPlayEndReturn())
        {
            return;
        }
        UIpanlOnTrue(5);
        ButtonOnShowToTrueOrFalse(4);      
        EventDispatcher.Dispatch(EventEnum.GetEmailShowHint,4);
        //BgMoveOut();
        //UIpanlOnTrue(2);
        CUIManager.Instance.OpenForm(UIFormName.FAQ);
        //CUIManager.Instance.GetForm<FAQSprite>(UIFormName.FAQ).Init();
       
    }
    private void ButtonOnShowToTrueOrFalse(int TotrueName)
    {
        NowTypeNumber = TotrueName;
        for (int i=0;i< ButtonOnShow.Length;i++)
        {
            if (i== TotrueName)
            {
                ButtonOnShow[i].SetActive(true);
            }else
            {
                ButtonOnShow[i].SetActive(false);
            }
        }
    }

    private void UIpanlOnTrue(int namber)
    {
      
        for (int i=0;i< UIpanl.Length;i++)
        {
            if (i== namber)
            {
                UIpanl[i].SetActive(true);
            }
            else
            {
                UIpanl[i].SetActive(false);
            }
        }
    }
    public void BgMove()
    {
        float offerH = 0;
        if (ResolutionAdapter.HasUnSafeArea)
        {
            var scale = this.myForm.GetScale();
            var safeArea = ResolutionAdapter.GetSafeArea();
            var offset = this.myForm.Pixel2View(safeArea.position);
            offerH = offset.y;
        }

        BG.anchoredPosition = new Vector2(0, -offerH);

        EventDispatcher.Dispatch(EventEnum.MainFormMove, 1);

        BG.DOAnchorPosX(381, 0.5f).Play().OnComplete(()=> {
         
            EventDispatcher.Dispatch(EventEnum.GetEmailShowHint, 7);

            //等到侧边栏完全出现后才可以，点击个人中心
            UIEventListener.AddOnClickListener(PersonCenter, PersonCenterButtonOn);
            UIEventListener.AddOnClickListener(Tou0, Tou0ButtonOnclick);
            UIEventListener.AddOnClickListener(HeadBg, Tou0ButtonOnclick);
            MoveCompleted = true;
            //end

        });
       
    }
    public void BgMoveOut()
    {
        BG.DOAnchorPosX(0,0.5f).Play();
        TouMoveF(2);
    }
    public void GetimpinfoCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetimpinfoCallBacke---->" + result);
        LoomUtil.QueueOnMainThread((param) =>
        {
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo.code == 200)
            {
                UserDataManager.Instance.lotteryDrawInfo = JsonHelper.JsonToObject<HttpInfoReturn<ActiveInfo>>(arg.ToString());

                if (UserDataManager.Instance.lotteryDrawInfo != null && UserDataManager.Instance.lotteryDrawInfo.data != null)
                {

                    if (EmailHit==null)
                    {
                        return;
                    }

                    //int EmailAwarNumber = UserDataManager.Instance.selfBookInfo.data.unreceivemsgcount;
                    int Emailshu = UserDataManager.Instance.selfBookInfo.data.unreadmsgcount;
                    int Newshu = 0;// UserDataManager.Instance.lotteryDrawInfo.data.unreadnewscount;
                    int CountNumber = Emailshu;

                    if (CountNumber > 0)
                    {
                        EmailHit.SetActive(true);
                        EmailHitText.text = CountNumber.ToString();
                    }else
                    {
                        EmailHit.SetActive(false);
                    }

                    if (Newshu>0)
                    {
                        NewHit.SetActive(true);
                        NewHitText.text = Newshu.ToString();
                    }else
                    {
                        NewHit.SetActive(false);
                    }
                }
            }
        }, null);
    }
    private void Init()
    {
        //红点协议
        GameHttpNet.Instance.ActiveInfo(GetimpinfoCallBacke);

        VersionStr.text = "Version:" + SdkMgr.Instance.GameVersion()+"("+ GameUtility.buildNum + ")";
        BgMove();

        //FAQ存储表的数据入字典
        GameDataMgr.Instance.table.FAQDialogData();
    }


    #region facebook login
    private void FacebookHandler(PointerEventData data)
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
            else if(UserDataManager.Instance.LoginInfo.LastLoginChannel == 3 && UserDataManager.Instance.hwLoginInfo != null)
            {
                GameHttpNet.Instance.TOKEN = string.Empty;
                GameDataMgr.Instance.userData.RemoveAllBook();

                var Localization = GameDataMgr.Instance.table.GetLocalizationById(190);
                UITipsMgr.Instance.PopupTips(Localization, false);

                //UITipsMgr.Instance.PopupTips("Log out Successful!", false);
                UserDataManager.Instance.LoginInfo.LastLoginChannel = 0;
                UserDataManager.Instance.LogOutDelLocalInfo();
                MaskButtonOn(null);
                CUIManager.Instance.OpenForm(UIFormName.HwLoginForm);
            }
            else if (UserDataManager.Instance.LoginInfo.LastLoginChannel == 0 || UserDataManager.Instance.LoginInfo.LastLoginChannel == 4)
            {
                mCurLoginChannel = 0;
                //UINetLoadingMgr.Instance.Show();
#if CHANNEL_HUAWEI
                SdkMgr.Instance.hwSDK.Login(1);
#else
                SdkMgr.Instance.FacebookLogin(1);
#endif
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
    }

    private void GoogleLoginSuccHandler(Notification vNot)
    {
        FacebookBtn.gameObject.SetActive(false);
    }

    private void FaceBookLoginSuccHandler(Notification vNot)
    {
        LoginDataInfo loginInfo = vNot.Data as LoginDataInfo;

        if (loginInfo != null)
        {
            string userId = loginInfo.UserId;
            string tokenStr = loginInfo.Token;

            FacebookTxt.text = "Log  out";

            if (loginInfo.OpenType != 1) return;

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
            GameHttpNet.Instance.
                LoginByThirdParty(1,email,userNick,faceUrl,userId,UserDataManager.Instance.UserData.IdToken,"android",LoginByThirdPartyCallBack);
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
        if(hwUserInfo != null && hwUserInfo.type == 1)
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
                    MaskButtonOn(null);
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
                UpdateLoginState();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.thirdPartyLoginInfo = JsonHelper.JsonToObject<HttpInfoReturn<ThirdPartyReturnInfo>>(result);
                    if (UserDataManager.Instance.thirdPartyLoginInfo != null && UserDataManager.Instance.thirdPartyLoginInfo.data != null)
                    {
                        GameHttpNet.Instance.TOKEN = UserDataManager.Instance.thirdPartyLoginInfo.data.token;
                    }
                    GameHttpNet.Instance.GetUserInfo(GetUserInfoCallBack);
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
                       // UserDataManager.Instance.UserData.bookList.AddRange(UserDataManager.Instance.userInfo.data.userinfo.booklist);


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
        MaskButtonOn(null);
        if (UserDataManager.Instance.userInfo.data.userinfo.firstplay == 0)
        {
            CUIManager.Instance.OpenForm(UIFormName.GuideForm/*, useFormPool: true*/);
        }
        else
        {
            /*, useFormPool: true*/
            //CUIManager.Instance.OpenForm(UIFormName.MainForm/*, useFormPool: true*/);

            //打开主界面
            XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:Open(logic.uiid.UIMainForm);");
            // EventDispatcher.Dispatch(EventEnum.BookJoinToShelfEvent);
            //刷新我的书本
            XLuaManager.Instance.GetLuaEnv().DoString(@"GameController.MainFormControl:ResetMyBookList()");
            // var mainForm = CUIManager.Instance.GetForm<MainForm>(UIFormName.MainForm);
            // if (mainForm != null)
            //     mainForm.ResetMyBookList();
        }

        if (UserDataManager.Instance.thirdPartyLoginInfo != null && UserDataManager.Instance.thirdPartyLoginInfo.data != null &&
            UserDataManager.Instance.thirdPartyLoginInfo.data.isfirst == 1)
        {
            UserDataManager.Instance.thirdPartyLoginInfo.data.isfirst = 0;
            UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, GameDataMgr.Instance.table.GetLocalizationById(224)/*"First login Successful! You have already received 10 diamonds."*/, AlertType.Sure, null, "OK");
        }

    }
#endregion


}

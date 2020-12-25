using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UGUI;
using pb;
using DG.Tweening;
using IGG.SDK.Modules.AgreementSigning.VO;
using XLua;

/// <summary>
/// 个人中心界面
/// </summary>
public class ProfileForm : BaseUIForm
{
    public Image HeadIcon, HeadFrame, PersonalityMenuImg, CustomizeMenuImg;
    public GameObject DressUpMask, Backe;
    public GameObject PerColumnGroup;
    public Text VIPText;
    public Transform HeadContent, FrameContent;
    public GameObject HeadItemView, FrameItemView;
    public Image Rect1, Rect2, Rect3, Rect4, Rect5;
    public Text TxtRect1, TxtRect2, TxtRect3, TxtRect4, TxtRect5;
    public Text PerTitleTxt, PerContentTxt;

    public InputField UseNameInputField;
    public Button BtnOk, BtnCancel, InputButton;
    public BannedWords BannedWords;
    public GameObject InputBg;

    public Text Rect1Percentage, Rect2Percentage, Rect3Percentage, Rect4Percentage, Rect5Percentage;
    public GameObject PersonalityInterpretation, PersonContent;
    public RectTransform BookContent, DressUpMaskBg, MyBooksContent;
    public GameObject MyBooksItem;
    public GameObject Refresh;
    public GameObject MyBookEmpty;

    private InfinityGridLayoutGroup mHeadGridLayoutGroup;
    private InfinityGridLayoutGroup mFrameGridLayoutGroup;
    private InfinityGridLayoutGroup mBgGridLayoutGroup;

    private int mAmount = 20; //设定列表数据的总的数量

    private List<ProfileItemInfo> headIconInfoList;
    private List<ProfileItemInfo> headFrameInfoList;
    private List<ProfileItemInfo> bgInfoList;

    private List<ProfileHeadItemView> mHeadItemViewList;
    private List<ProfileFrameItemView> mFrameItemViewList;
    private List<ProfileBgItemView> mBgItemViewList;


    private int mOptionNum; //已经选择的次数
    private bool mShapeOpen; //当选择项，超过20的时候，图形是开放的
    private bool mDetailOpen; //当选项超过50的时候，详细性格分析是开放的
    private bool mCanRefresh = false; //判断是否够钥匙刷新性格描述
    private int mCurNeekKeys = 0; //现在刷新，需要消耗多少钥匙
    public int mCueMenuIndex = 0; //当前的导航标签索引
    private Sprite mMenuSelectSpr;
    private Sprite mMenuUnSelectSpr;
    private bool mCustomizeHadChange = false; //装扮有更新
    private int mCurHeadId; //头像
    private int mCurFrameId; //vip框
    private int mCurBgId; //背景
    private float offsetAngle = 0;
    private ContentSizeFitter FrameContentCs;
    private GridLayoutGroup FrameContentGl;

    private List<GameObject> MyBookInitsGo;

    //private GameObject mMenuGroup;

    private Image SignInButton;
    private Text SignInButtonText;
    private int mCurLoginChannel;
    private HwUserInfo hwUserInfo;
    private GameObject Setting;
    private GameObject FAQ;
    private GameObject Vip;

    private RectTransform UIMask, Frame, CatEnterBg, EnterButton;

    private string OnIconPath = "SettingForm/btn_on";
    private string OffIconPath = "SettingForm/btn_off";
    private GameObject MusicaButton, SonidosButton;
    private Image MusicaImage, SonidosImage;
    private GameObject Preguntas, Politica, Terminos, Contactanos, Email, Hit, Community;
    private GameObject Pakage, PakageHit;

    private GameObject CommunityWindow, Facebook, Twitter, Instagram, Google, Youtube, Back, AjustesBg;

    private Text compleNumber, MyReadBookNumber, VersionText, terminosText, politicaText;

    private List<IGGAgreement> IggAgreementList;

    private int TimeSequence;

    protected override void Awake()
    {
        base.Awake();
        //mMenuGroup = transform.Find("Frame/MenuGroup").gameObject ;        

        SignInButton = transform.Find("Frame/ScrollView/Viewport/Content/TopBg/SignInButton").GetComponent<Image>();
        SignInButtonText = transform.Find("Frame/ScrollView/Viewport/Content/TopBg/SignInButton/Text")
            .GetComponent<Text>();
        Setting = transform.Find("Frame/ScrollView/Viewport/Content/TopBg/Setting").gameObject;
        FAQ = transform.Find("Frame/ScrollView/Viewport/Content/TopBg/FAQ").gameObject;
        Vip = transform.Find("Frame/ScrollView/Viewport/Content/TopBg/Vip").gameObject;


        UIMask = transform.Find("UIMask").GetComponent<RectTransform>();
        Frame = transform.Find("Frame").GetComponent<RectTransform>();
        CatEnterBg = transform.Find("Frame/ScrollView/Viewport/Content/CatEnterBg").GetComponent<RectTransform>();
        EnterButton = transform.Find("Frame/ScrollView/Viewport/Content/CatEnterBg/EnterButton")
            .GetComponent<RectTransform>();

        AjustesBg = transform.Find("Frame/ScrollView/Viewport/Content/AjustesBg").gameObject;

        MusicaButton = transform.Find("Frame/ScrollView/Viewport/Content/AjustesBg/MusieBg/MusieButton").gameObject;
        MusicaImage = MusicaButton.GetComponent<Image>();

        SonidosButton = transform.Find("Frame/ScrollView/Viewport/Content/AjustesBg/Sonidos/SonidosButton").gameObject;
        SonidosImage = SonidosButton.GetComponent<Image>();

        Preguntas = transform.Find("Frame/ScrollView/Viewport/Content/AjustesBg/Preguntas").gameObject;
        Preguntas.gameObject.SetActiveEx(false);
        Politica = transform.Find("Frame/ScrollView/Viewport/Content/AjustesBg/Politica").gameObject;
        Terminos = transform.Find("Frame/ScrollView/Viewport/Content/AjustesBg/Terminos").gameObject;
        Community = transform.Find("Frame/ScrollView/Viewport/Content/AjustesBg/Community").gameObject;
        Contactanos = transform.Find("Frame/ScrollView/Viewport/Content/AjustesBg/Contactanos").gameObject;
        Email = transform.Find("Frame/ScrollView/Viewport/Content/TopBg/Email").gameObject;
        Hit = transform.Find("Frame/ScrollView/Viewport/Content/TopBg/Email/Hit").gameObject;

        Pakage = transform.Find("Frame/ScrollView/Viewport/Content/TopBg/Pakage").gameObject;
        PakageHit = transform.Find("Frame/ScrollView/Viewport/Content/TopBg/Pakage/Hit").gameObject;

        CommunityWindow = transform.Find("CommunityWindow").gameObject;
        Facebook = transform.Find("CommunityWindow/BG/Facebook/Image").gameObject;
        Twitter = transform.Find("CommunityWindow/BG/Twitter/Image").gameObject;
        Instagram = transform.Find("CommunityWindow/BG/Instagram/Image").gameObject;
        Youtube = transform.Find("CommunityWindow/BG/Youtube/Image").gameObject;
        Google = transform.Find("CommunityWindow/BG/Google/Image").gameObject;
        transform.Find("CommunityWindow/BG/Google").gameObject.SetActiveEx(false);
        Back = transform.Find("CommunityWindow/BG/Back/Back").gameObject;

        compleNumber = transform.Find("Frame/ScrollView/Viewport/Content/TopBg/comple/compleNumber")
            .GetComponent<Text>();
        MyReadBookNumber = transform.Find("Frame/ScrollView/Viewport/Content/TopBg/Libros/compleNumber")
            .GetComponent<Text>();
        VersionText = transform.Find("Frame/ScrollView/Viewport/Content/AjustesBg/VersionText").GetComponent<Text>();
        politicaText = transform.Find("Frame/ScrollView/Viewport/Content/AjustesBg/Politica/Text").GetComponent<Text>();
        terminosText = transform.Find("Frame/ScrollView/Viewport/Content/AjustesBg/Terminos/Text").GetComponent<Text>();

        Politica.gameObject.SetActiveEx(false);
        Terminos.gameObject.SetActiveEx(false);
        IggAgreementList = IGGAgreementManager.Instance.IggAgreementList;
        if (IggAgreementList != null && IggAgreementList.Count > 0)
        {
            if (IggAgreementList.Count >= 1)
            {
                politicaText.text = IggAgreementList[0].LocalizedName;
                Politica.gameObject.SetActiveEx(true);
            }

            if (IggAgreementList.Count >= 2)
            {
                terminosText.text = IggAgreementList[1].LocalizedName;
                Terminos.gameObject.SetActiveEx(true);
            }

            PluginTools.Instance.ContentSizeFitterRefresh(AjustesBg.transform);
        }
        else
        {
            TimeSequence = CTimerManager.Instance.AddTimer(1000, -1, TimeReapety);
        }
    }

    private void TimeReapety(int seq)
    {
        IggAgreementList = IGGAgreementManager.Instance.IggAgreementList;
        if (IggAgreementList == null || IggAgreementList.Count <= 0) return;
        CTimerManager.Instance.RemoveTimer(TimeSequence);
        if (IggAgreementList.Count >= 1)
        {
            if (politicaText != null && Politica != null)
            {
                politicaText.text = IggAgreementList[0].LocalizedName;
                Politica.gameObject.SetActiveEx(true);
            }
        }

        if (IggAgreementList.Count >= 2)
        {
            if (terminosText != null && Politica != null)
            {
                terminosText.text = IggAgreementList[1].LocalizedName;
                Terminos.gameObject.SetActiveEx(true);
            }
        }

        PluginTools.Instance.ContentSizeFitterRefresh(AjustesBg.transform);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public override void OnOpen()
    {
        base.OnOpen();


        if (MyBookInitsGo == null)
            MyBookInitsGo = new List<GameObject>();

        //int offerH = 0;
        //if (GameUtility.IsIphoneXDevice())
        //{
        //    offerH = GameUtility.IphoneXTopH;
        //    Frame.offsetMax = new Vector2(0, -(offerH));

        //}

        CatEnterBg.gameObject.SetActive(false);

        //BookContent.anchoredPosition = new Vector2(-400f, 0);

        UIEventListener.AddOnClickListener(Refresh, RefreshHandler);
        UIEventListener.AddOnClickListener(PersonalityMenuImg.gameObject, PerMenuClickHandler);
        UIEventListener.AddOnClickListener(CustomizeMenuImg.gameObject, CusMenuClickHandler);

        addMessageListener(EventEnum.SetGuidPos, SetGuidPos);
        addMessageListener(EventEnum.FaceBookLoginSucc, FaceBookLoginSucc);
        addMessageListener(EventEnum.GoogleLoginSucc, GoogleLoginSuccHandler);
        addMessageListener(EventEnum.ThirdPartyLoginSucc, ThirdPartyLoginSuccHandler);
        addMessageListener(EventEnum.EmailNumberShow, EmailRedshow);
        addMessageListener(EventEnum.PakageNumberShow, PakageRedshow);


        BtnOk.onClick.AddListener(BtnOkOnclick);
        BtnCancel.onClick.AddListener(BtnCancelOnclick);
        InputButton.onClick.AddListener(InputButtonOnclick);
        UseNameInputField.onValueChanged.AddListener(NameChanged);

        mHeadGridLayoutGroup = HeadContent.GetComponent<InfinityGridLayoutGroup>();
        mFrameGridLayoutGroup = FrameContent.GetComponent<InfinityGridLayoutGroup>();
        FrameContentCs = FrameContent.GetComponent<ContentSizeFitter>();
        FrameContentGl = FrameContent.GetComponent<GridLayoutGroup>();


        UIEventListener.AddOnClickListener(SignInButton.gameObject, SignInButtonOnclicke);
        UIEventListener.AddOnClickListener(Setting, SettingButton);
        UIEventListener.AddOnClickListener(FAQ, FAQButton);
        UIEventListener.AddOnClickListener(EnterButton.gameObject, EnterButtonOnclicke);

        UIEventListener.AddOnClickListener(MusicaButton, MusicaButtonOnClicke);
        UIEventListener.AddOnClickListener(SonidosButton, SonidosButtonOnClicke);
        UIEventListener.AddOnClickListener(Preguntas, PreguntasButton);
        UIEventListener.AddOnClickListener(Politica, PoliticaButton);
        UIEventListener.AddOnClickListener(Terminos, TerminosButton);
        UIEventListener.AddOnClickListener(Community, CommunityButton);
        UIEventListener.AddOnClickListener(Contactanos, ContactanosButton);
        UIEventListener.AddOnClickListener(Email, EmailButton);
        UIEventListener.AddOnClickListener(Pakage, PakageButton);

        UIEventListener.AddOnClickListener(Facebook, FacebookButton);
        UIEventListener.AddOnClickListener(Twitter, TwitterButton);
        UIEventListener.AddOnClickListener(Instagram, InstagramButton);
        UIEventListener.AddOnClickListener(Google, GoogleButton);
        UIEventListener.AddOnClickListener(Youtube, YoutubeButton);
        UIEventListener.AddOnClickListener(Back, BackButton);

        //请求红点
        XLuaManager.Instance.GetLuaEnv().DoString(@"GameController.MainFormControl:RedPointRequest()");

        OnAppear();
        RefreshUI();
    }


    private void EmailRedshow(Notification obj)
    {
        int isShow = System.Convert.ToInt32(obj.Data);
        if (isShow == 1)
        {
            Hit.SetActive(true);
        }
        else if (isShow == 2)
        {
            Hit.SetActive(false);
        }
    }

    private void PakageRedshow(Notification obj)
    {
        int isShow = System.Convert.ToInt32(obj.Data);
        if (isShow == 1)
        {
            PakageHit.SetActive(true);
        }
        else if (isShow == 2)
        {
            PakageHit.SetActive(false);
        }
    }

    private void NameChanged(string arg0)
    {
        SetBtnOkActive(true);
    }

    public override void OnAppear()
    {
        base.OnAppear();
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.GetSelfBookInfo(CompleNumberUpdat);
    }


    private void RefreshUI()
    {
        if (HeadIcon != null)
        {
            //【调用lua公共方法 加载头像】   -1代码当前装扮的头像    
            XLuaManager.Instance.CallFunction("GameHelper", "ShowAvatar", -1, HeadIcon);
        }

        if (HeadFrame != null)
        {
            //【调用lua公共方法 加载头像框】   -1代码当前装扮的头像框   
            XLuaManager.Instance.CallFunction("GameHelper", "ShowAvatarframe", -1, HeadFrame);
        }


        if (UserDataManager.Instance.userInfo.data.userinfo.is_vip == 1)
            Vip.SetActive(true);
        else
            Vip.SetActive(false);

        mCueMenuIndex = 0;

        if (string.IsNullOrEmpty(UserDataManager.Instance.userInfo.data.userinfo.nickname))
        {
            UseNameInputField.text = "Guest";
        }
        else
        {
            UseNameInputField.text = UserDataManager.Instance.userInfo.data.userinfo.nickname;
        }

        VersionText.text = "©IGG All Rights Reserved.\nversion " + SdkMgr.Instance.GameVersion();

        //UUidInputField.text = UserDataManager.Instance.userInfo.data.userinfo.phoneimei.ToString();

        //CheckCatGuide();

        ResetBtnState();

        ShowCompleNumber();

        SpwanMyBookes(); //生成书本

        SetBtnOkActive(false);
    }

    private void SetBtnOkActive(bool isActive)
    {
        BtnOk.gameObject.SetActiveEx(isActive);
        BtnCancel.gameObject.SetActiveEx(isActive);
        InputBg.gameObject.SetActiveEx(isActive);
        InputButton.gameObject.SetActiveEx(!isActive);
        Email.gameObject.SetActiveEx(!isActive);
    }

    public override void OnClose()
    {
        base.OnClose();

        UIEventListener.RemoveOnClickListener(Refresh, RefreshHandler);
        UIEventListener.RemoveOnClickListener(PersonalityMenuImg.gameObject, PerMenuClickHandler);
        UIEventListener.RemoveOnClickListener(CustomizeMenuImg.gameObject, CusMenuClickHandler);
        //UIEventListener.RemoveOnClickListener(ShareImage.gameObject, ShareClickHandler);

        UIEventListener.RemoveOnClickListener(SignInButton.gameObject, SignInButtonOnclicke);
        UIEventListener.RemoveOnClickListener(Setting, SettingButton);
        UIEventListener.RemoveOnClickListener(FAQ, FAQButton);
        UIEventListener.RemoveOnClickListener(EnterButton.gameObject, EnterButtonOnclicke);

        UIEventListener.RemoveOnClickListener(MusicaButton, MusicaButtonOnClicke);
        UIEventListener.RemoveOnClickListener(SonidosButton, SonidosButtonOnClicke);
        UIEventListener.RemoveOnClickListener(Preguntas, PreguntasButton);
        UIEventListener.RemoveOnClickListener(Politica, PoliticaButton);
        UIEventListener.RemoveOnClickListener(Terminos, TerminosButton);
        UIEventListener.RemoveOnClickListener(Community, CommunityButton);
        UIEventListener.RemoveOnClickListener(Contactanos, ContactanosButton);
        UIEventListener.RemoveOnClickListener(Email, EmailButton);

        UIEventListener.RemoveOnClickListener(Facebook, FacebookButton);
        UIEventListener.RemoveOnClickListener(Twitter, TwitterButton);
        UIEventListener.RemoveOnClickListener(Instagram, InstagramButton);
        UIEventListener.RemoveOnClickListener(Google, GoogleButton);
        UIEventListener.RemoveOnClickListener(Youtube, YoutubeButton);
        UIEventListener.RemoveOnClickListener(Back, BackButton);

        BtnOk.onClick.RemoveListener(BtnOkOnclick);
        BtnCancel.onClick.RemoveListener(BtnCancelOnclick);
        InputButton.onClick.RemoveListener(InputButtonOnclick);
        UseNameInputField.onValueChanged.RemoveListener(NameChanged);

        if (mHeadItemViewList != null)
        {
            int len = mHeadItemViewList.Count;
            for (int i = 0; i < len; i++)
            {
                ProfileHeadItemView itemView = mHeadItemViewList[i];
                if (itemView != null)
                {
                    itemView.Dispose();
                    GameObject.Destroy(itemView);
                    itemView = null;
                }
            }

            mHeadItemViewList = null;
        }

        if (mFrameItemViewList != null)
        {
            int len = mFrameItemViewList.Count;
            for (int i = 0; i < len; i++)
            {
                ProfileFrameItemView itemView = mFrameItemViewList[i];
                if (itemView != null)
                {
                    itemView.Dispose();
                    GameObject.Destroy(itemView);
                    itemView = null;
                }
            }

            mFrameItemViewList = null;
        }

        if (mBgItemViewList != null)
        {
            int len = mBgItemViewList.Count;
            for (int i = 0; i < len; i++)
            {
                ProfileBgItemView itemView = mBgItemViewList[i];
                if (itemView != null)
                {
                    itemView.Dispose();
                    GameObject.Destroy(itemView);
                    itemView = null;
                }
            }

            mBgItemViewList = null;
        }


        DestroyMyBookItems();
        SaveCustomizeInfo();
    }

    private void BtnCancelOnclick()
    {
        UseNameInputField.text = UserDataManager.Instance.userInfo.data.userinfo.nickname;
        SetBtnOkActive(false);
    }

    private void InputButtonOnclick()
    {
        UseNameInputField.text = "";
        EventSystem.current.SetSelectedGameObject(UseNameInputField.gameObject);
        SetBtnOkActive(true);
    }

    private void DestroyMyBookItems()
    {
        if (MyBookInitsGo != null)
        {
            for (int i = 0; i < MyBookInitsGo.Count; i++)
            {
                if (MyBookInitsGo[i] != null)
                {
                    MyBookInitsGo[i].GetComponent<ProfileMyBookItem>().Dispose();
                    Destroy(MyBookInitsGo[i]);
                }
            }
        }
    }

    private void ShowCompleNumber()
    {
        compleNumber.text = UserDataManager.Instance.selfBookInfo.data.read_chapter_count.ToString();
        MyReadBookNumber.text = UserDataManager.Instance.selfBookInfo.data.read_book_count.ToString();
    }

    private void CompleNumberUpdat(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----ToLoadSelfBookInfo---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);

        if (jo != null)
        {
            //UINetLoadingMgr.Instance.Close();
            if (jo.code == 200)
            {
                UserDataManager.Instance.selfBookInfo =
                    JsonHelper.JsonToObject<HttpInfoReturn<SelfBookInfo>>(arg.ToString());
                this.RefreshUI();
            }
        }
    }

    private void SettingButton(PointerEventData data)
    {
        CUIManager.Instance.OpenForm(UIFormName.SettingNav);
    }

    private void FAQButton(PointerEventData data)
    {
        CUIManager.Instance.OpenForm(UIFormName.FAQ);
        //CUIManager.Instance.GetForm<FAQSprite>(UIFormName.FAQ).Init();
    }

    private void ShareClickHandler(PointerEventData data)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            SdkMgr.Instance.shareSDK.ShareScreenShot();
#elif UNITY_IOS && !UNITY_EDITOR
        SdkMgr.Instance.shareSDK.SharePicture();
#endif
    }

    public void PerMenuClickHandler(PointerEventData data)
    {
        if (mCueMenuIndex != 0)
        {
            mCueMenuIndex = 0;
            //PersonalityMenuImg.sprite = mMenuSelectSpr;
            //CustomizeMenuImg.sprite = mMenuUnSelectSpr;

            //PersonalityContent.gameObject.SetActive(true);

            //ShareImage.gameObject.SetActive(true);
            // Backe.SetActive(false);
            //  DressUpMaskBgMove(false);
        }
        else
        {
            SaveCustomizeInfo();
        }
    }

    private void CusMenuClickHandler(PointerEventData data)
    {
        if (mCueMenuIndex == 0)
        {
            mCueMenuIndex = 1;
            //PersonalityMenuImg.sprite = mMenuUnSelectSpr;
            //CustomizeMenuImg.sprite = mMenuSelectSpr;

            //PersonalityContent.gameObject.SetActive(false);

            //ShareImage.gameObject.SetActive(false);

            //UINetLoadingMgr.Instance.Show();
            // GameHttpNet.Instance.GetUserProfileCustomizeConfig(CustomizeConfigHandler);
        }
        else
        {
            // DressUpMask.SetActive(true);
            //  Backe.SetActive(true);
            // DressUpMaskBgMove(true);
        }

        //打开装扮界面
        XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:Open(logic.uiid.UIDressUpForm);");
    }

    private void CustomizeConfigHandler(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----CustomizeConfigHandler---->" + result);
        if (result.Equals("error"))
        {
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
                    UserDataManager.Instance.profileConfigInfo =
                        JsonHelper.JsonToObject<HttpInfoReturn<ProfileConfigInfo>>(arg.ToString());
                    if (UserDataManager.Instance.profileConfigInfo != null &&
                        UserDataManager.Instance.profileConfigInfo.data != null &&
                        UserDataManager.Instance.profileConfigInfo.data.ornamentarr != null)
                    {
                        InitCustomizeItems();
                    }
                }
            }
        }, null);
    }

    private void SaveCustomizeInfo()
    {
        if (mCustomizeHadChange)
        {
            GameHttpNet.Instance.SetUserProfileCustomize(mCurHeadId, mCurBgId, mCurFrameId, SetCustomizeCallBack);
        }

        mCustomizeHadChange = false;
    }

    private void SetCustomizeCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----SetCustomizeCallBack---->" + result);
        if (result.Equals("error"))
        {
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
                    // Backe.SetActive(false);
                    // DressUpMaskBgMove(false);
                    UserDataManager.Instance.userInfo.data.userinfo.avatar = mCurHeadId;
                    UserDataManager.Instance.userInfo.data.userinfo.avatar_frame = mCurFrameId;

                    //【调用lua公共方法 加载头像】   -1代码当前装扮的头像    
                    XLuaManager.Instance.CallFunction("GameHelper", "SetDressUpCache", mCurHeadId, mCurFrameId);
                }
            }
        }, null);
    }

    private void CloseHandler(PointerEventData data)
    {
        CUIManager.Instance.CloseForm(UIFormName.InviteForm);
    }

    private void BtnOkOnclick()
    {
        if (BannedWords.haveBannedWords())
        {
            UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(440));
            return;
        }

        if (UserDataManager.Instance.userInfo.data.userinfo.nickname.ToString()
            .Equals(UseNameInputField.text.ToString()))
        {
            return;
        }
        else
        {
            if (UseNameInputField.text.Length < 1 || UseNameInputField.text.Length > 20)
            {
                UseNameInputField.text = UserDataManager.Instance.userInfo.data.userinfo.nickname;
                SetBtnOkActive(false);
                return;
            }

            SetBtnOkActive(false);
            GameHttpNet.Instance.SetUserLanguage(UseNameInputField.text.ToString(), 2, ChangeNameCallBackHandler);
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

                    //埋点*改名
                    GamePointManager.Instance.BuriedPoint(EventEnum.ChangeNickName);
                }
            }, null);
        }
    }


    //填充服务端返回的数据
    private void SetData()
    {
        if (PerColumnGroup == null)
        {
            return;
        }

        if (UserDataManager.Instance.profileData != null && UserDataManager.Instance.profileData.data != null &&
            UserDataManager.Instance.profileData.data.info != null)
        {
            ProfileDetailInfo detailInfo = UserDataManager.Instance.profileData.data.info;
            mOptionNum = detailInfo.option_num;
            mShapeOpen = mOptionNum >= UserDataManager.Instance.profileData.data.select.Select1;
            mDetailOpen = mOptionNum >= UserDataManager.Instance.profileData.data.select.Select2;

            float vShareProgress = mOptionNum / (UserDataManager.Instance.profileData.data.select.Select1 * 1.0f);
            if (vShareProgress > 1.0f)
                vShareProgress = 1.0f;

            float vDetailProgress = mOptionNum / (UserDataManager.Instance.profileData.data.select.Select1 * 1.0f);
            if (vDetailProgress > 1.0f)
                vDetailProgress = 1.0f;

            //if (!mDetailOpen)
            //    PersonalityInterpretation.SetActive(false);
            ////PerInterUnOpenTipTxt.text = "Make " + UserDataManager.Instance.profileData.data.select.Select2 + " choices to unlock(<color='#5492ec'>" + mOptionNum + "</color>/" + UserDataManager.Instance.profileData.data.select.Select2 + ")";
            //else
            //    PersonalityInterpretation.SetActive(true);
            ////PerInterUnOpenTipTxt.text = "";


            PerColumnGroup.gameObject.SetActive(true);

            SetShape(detailInfo);
            SetRefreshCost(detailInfo);
        }
    }

    // 设置性格图
    private void SetShape(ProfileDetailInfo detailInfo)
    {
        if (detailInfo != null)
        {
            TxtRect1.text = GameDataMgr.Instance.table.GetPersonalityTxtById(1);
            TxtRect2.text = GameDataMgr.Instance.table.GetPersonalityTxtById(2);
            TxtRect3.text = GameDataMgr.Instance.table.GetPersonalityTxtById(3);
            TxtRect4.text = GameDataMgr.Instance.table.GetPersonalityTxtById(4);
            TxtRect5.text = GameDataMgr.Instance.table.GetPersonalityTxtById(5);

            int maxValue = 0;
            if (detailInfo.mature > maxValue)
                maxValue = detailInfo.mature;
            if (detailInfo.rigorous > maxValue)
                maxValue = detailInfo.rigorous;
            if (detailInfo.reasonable > maxValue)
                maxValue = detailInfo.reasonable;
            if (detailInfo.curious > maxValue)
                maxValue = detailInfo.curious;
            if (detailInfo.tsundere > maxValue)
                maxValue = detailInfo.tsundere;


            if (maxValue > 0)
            {
                Rect1.fillAmount = (float) detailInfo.mature / (maxValue * 1.0f);
                Rect2.fillAmount = (float) detailInfo.rigorous / (maxValue * 1.0f);
                Rect3.fillAmount = (float) detailInfo.reasonable / (maxValue * 1.0f);
                Rect4.fillAmount = (float) detailInfo.curious / (maxValue * 1.0f);
                Rect5.fillAmount = (float) detailInfo.tsundere / (maxValue * 1.0f);

                Rect1Percentage.text = string.Format("{0}%", (int) (detailInfo.mature / (maxValue * 1.0f) * 100));
                Rect2Percentage.text = string.Format("{0}%", (int) (detailInfo.rigorous / (maxValue * 1.0f) * 100));
                Rect3Percentage.text = string.Format("{0}%", (int) (detailInfo.reasonable / (maxValue * 1.0f) * 100));
                Rect4Percentage.text = string.Format("{0}%", (int) (detailInfo.curious / (maxValue * 1.0f) * 100));
                Rect5Percentage.text = string.Format("{0}%", (int) (detailInfo.tsundere / (maxValue * 1.0f) * 100));
            }
            else
            {
                Rect1.fillAmount = 0;
                Rect2.fillAmount = 0;
                Rect3.fillAmount = 0;
                Rect4.fillAmount = 0;
                Rect5.fillAmount = 0;

                Rect1Percentage.text = "0%";
                Rect2Percentage.text = "0%";
                Rect3Percentage.text = "0%";
                Rect4Percentage.text = "0%";
                Rect5Percentage.text = "0%";
            }

            float
                allValue = maxValue /*detailInfo.mature + detailInfo.rigorous + detailInfo.reasonable + detailInfo.curious + detailInfo.tsundere*/;
            if (allValue > 0)
            {
                float percent = (float) detailInfo.mature / (allValue * 1.0f);
                offsetAngle = 0;

                percent = (float) detailInfo.rigorous / (allValue * 1.0f);

                percent = (float) detailInfo.reasonable / (allValue * 1.0f);

                percent = (float) detailInfo.curious / (allValue * 1.0f);

                percent = (float) detailInfo.tsundere / (allValue * 1.0f);
            }

            //Num1Txt.transform.localPosition = new Vector3(200 * Rect1.fillAmount - 55, 0, 0);
            //Num2Txt.transform.localPosition = new Vector3(200 * Rect2.fillAmount - 55, 0, 0);
            //Num3Txt.transform.localPosition = new Vector3(200 * Rect3.fillAmount - 55, 0, 0);
            //Num4Txt.transform.localPosition = new Vector3(200 * Rect4.fillAmount - 55, 0, 0);
            //Num5Txt.transform.localPosition = new Vector3(200 * Rect5.fillAmount - 55, 0, 0);

            if (!string.IsNullOrEmpty(detailInfo.trait_type) && !string.IsNullOrEmpty(detailInfo.trait_value))
            {
                string[] typeList = detailInfo.trait_type.Split('-');
                string[] valueList = detailInfo.trait_value.Split('-');

                if (typeList != null && typeList.Length > 2)
                {
                    int perA = int.Parse(typeList[0]);
                    int perB = int.Parse(typeList[1]);
                    int perC = int.Parse(typeList[2]);

                    int perAB = 0;
                    if (perA > perB)
                        perAB = perB * 10 + perA;
                    else
                        perAB = perA * 10 + perB;

                    t_analysisA anA = GameDataMgr.Instance.table.GetAnalysisAById(perAB);
                    if (anA != null)
                        PerTitleTxt.text = "  Your chart belongs to type " + anA.group + " personality";

                    if (valueList != null && valueList.Length > 2)
                    {
                        int valueA = int.Parse(valueList[0]);
                        int valueB = int.Parse(valueList[1]);
                        int valueC = int.Parse(valueList[2]);

                        SetInterpretation(perAB, perC, valueA, valueB, valueC);
                    }
                }
            }
        }
    }

    private void SetCicleAngle(Image vCircle, float vPercent)
    {
        //vCircle.fillAmount = vPercent;
        float curAngle = -360.0f * vPercent;

        vCircle.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, curAngle));
    }

    //设置属性描述
    private void SetInterpretation(int vPerAB, int vPerC, int vA, int vB, int vC)
    {
        string result = GameDataMgr.Instance.table.GetAnalysisATxtById(vPerAB, vA);
        result += "\n\n" + GameDataMgr.Instance.table.GetAnalysisBTxtById(vPerAB, vB);
        result += "\n\n" + GameDataMgr.Instance.table.GetAnalysisCTxtById(vPerC, vC);

        PerContentTxt.text = result;
        //PerContentTxt.rectTransform.sizeDelta = new Vector2(665, PerContentTxt.preferredHeight);
        //PersonInterpretationBg.rectTransform.sizeDelta = new Vector2(750, PerContentTxt.preferredHeight + 130);


        float detailHight = 416 + PerContentTxt.preferredHeight + 250 + 200;
        if (detailHight < 900f)
            detailHight = 900f;

        PersonContent.SetActive(false);
        Number = 0;
        InvokeRepeating("PersonInvoke", 0, 0.3f);
    }

    private int Number = 0;

    private void PersonInvoke()
    {
        Number++;
        if (Number >= 3)
        {
            CancelInvoke("PersonInvoke");
            PersonContent.SetActive(true);
        }

        PersonContent.SetActive(false);
        PersonContent.SetActive(true);

        //Debug.Log("111111");
        //Invoke("PersonInvoke2",0.3f);
    }

    private void PersonInvoke2()
    {
        CancelInvoke("PersonInvoke2");
        PersonContent.SetActive(true);
        //Debug.Log("222222");
    }

    //设置刷新的消耗
    private void SetRefreshCost(ProfileDetailInfo detailInfo)
    {
        ProfileBaseInfo baseInfo = UserDataManager.Instance.profileData.data.select;
        mCanRefresh = false;
        if (baseInfo != null)
        {
            string[] refreshCostList = baseInfo.RefreshInterpretation.Split(',');
            if (refreshCostList.Length > 0)
            {
                int curIndex = 0;
                if (refreshCostList.Length > detailInfo.refresh_count)
                    curIndex = detailInfo.refresh_count;
                else
                    curIndex = refreshCostList.Length - 1;


                mCurNeekKeys = int.Parse(refreshCostList[curIndex]);
                //KeyNumTxt.text = "X" + mCurNeekKeys;
            }
        }
    }

    /// <summary>
    /// 这个是刷新
    /// </summary>
    /// <param name="vData"></param>
    private void RefreshHandler(PointerEventData vData)
    {
        if (UserDataManager.Instance.profileData.data == null ||
            UserDataManager.Instance.profileData.data.select == null) return;
        if (UserDataManager.Instance.UserData.KeyNum >= mCurNeekKeys)
        {
            string str = "Do you want to spend " + mCurNeekKeys + " key to refresh “Personality Interpretation”?";
            UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218) /*"TIPS"*/, str,
                AlertType.SureOrCancel, AlertCallBack);
        }
        else
        {
            OpenChargeTip(1, mCurNeekKeys, 1 * 0.99f, true);
        }
    }

    private void AlertCallBack(bool value)
    {
        if (value)
        {
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.RefreshUserProfile(RefreshCallBack);
        }
    }

    private void RefreshCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----RefreshCallBack---->" + result);
        if (result.Equals("error"))
        {
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
                    UserDataManager.Instance.profileRefreshData =
                        JsonHelper.JsonToObject<HttpInfoReturn<ProfileRefreshData>>(arg.ToString());
                    if (UserDataManager.Instance.profileRefreshData != null &&
                        UserDataManager.Instance.profileRefreshData.data != null)
                    {
                        UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.profileRefreshData.data.bkey);
                        UserDataManager.Instance.ResetMoney(2,
                            UserDataManager.Instance.profileRefreshData.data.diamond);

                        UserDataManager.Instance.profileData.data.info.mature =
                            UserDataManager.Instance.profileRefreshData.data.mature;
                        UserDataManager.Instance.profileData.data.info.rigorous =
                            UserDataManager.Instance.profileRefreshData.data.rigorous;
                        UserDataManager.Instance.profileData.data.info.reasonable =
                            UserDataManager.Instance.profileRefreshData.data.reasonable;
                        UserDataManager.Instance.profileData.data.info.curious =
                            UserDataManager.Instance.profileRefreshData.data.curious;
                        UserDataManager.Instance.profileData.data.info.tsundere =
                            UserDataManager.Instance.profileRefreshData.data.tsundere;
                        UserDataManager.Instance.profileData.data.info.trait_type =
                            UserDataManager.Instance.profileRefreshData.data.trait_type;
                        UserDataManager.Instance.profileData.data.info.trait_value =
                            UserDataManager.Instance.profileRefreshData.data.trait_value;
                        UserDataManager.Instance.profileData.data.info.refresh_count =
                            UserDataManager.Instance.profileRefreshData.data.refresh_count;
                        SetData();

                        var Localization = GameDataMgr.Instance.table.GetLocalizationById(192);
                        UITipsMgr.Instance.PopupTips(Localization, false);

                        //UITipsMgr.Instance.PopupTips("Refresh success", false);
                    }
                }
            }
        }, null);
    }

    private void OpenChargeTip(int vType, int vNum, float vPrice, bool vNeedHideMainTop = false)
    {
        //if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data != null &&
        //                 UserDataManager.Instance.userInfo.data.userinfo.newpackage_status == 1)
        //{
        //    CUIManager.Instance.OpenForm(UIFormName.FirstGigtGroup);
        //}
        //else
        //{
        //    if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data != null &&
        //               UserDataManager.Instance.userInfo.data.userinfo.newpackage_status == 1)
        //    {
        //        CUIManager.Instance.OpenForm(UIFormName.FirstGigtGroup);
        //        return;
        //    }
        //CUIManager.Instance.OpenForm(UIFormName.ChargeTipsForm);
        //ChargeTipsForm tipForm = CUIManager.Instance.GetForm<ChargeTipsForm>(UIFormName.ChargeTipsForm);

        CUIManager.Instance.OpenForm(UIFormName.NewChargeTips);
        NewChargeTips tipForm = CUIManager.Instance.GetForm<NewChargeTips>(UIFormName.NewChargeTips);


        if (tipForm != null)
            tipForm.Init(vType, vNum, vPrice, vNeedHideMainTop);
        //}
    }


    private void InitCustomizeItems()
    {
        headIconInfoList = new List<ProfileItemInfo>();
        headFrameInfoList = new List<ProfileItemInfo>();
        bgInfoList = new List<ProfileItemInfo>();

        int len = UserDataManager.Instance.profileConfigInfo.data.ornamentarr.Count;
        for (int i = 0; i < len; i++)
        {
            ProfileItemInfo itemInfo = UserDataManager.Instance.profileConfigInfo.data.ornamentarr[i];
            if (itemInfo.type == 1)
            {
                headIconInfoList.Add(itemInfo);
            }
            else if (itemInfo.type == 2)
            {
                headFrameInfoList.Add(itemInfo);
            }
            else if (itemInfo.type == 3)
            {
                //if (mCurBgId == itemInfo.res)
                //    BgItemDescTxt.text = itemInfo.remark;
                bgInfoList.Add(itemInfo);
            }
        }

        //mHeadGridLayoutGroup.SetAmount(headIconInfoList.Count);
        //mHeadGridLayoutGroup.updateChildrenCallback = HeadUpdateChildrenCallback;

        //mFrameGridLayoutGroup.SetAmount(headFrameInfoList.Count);
        //mFrameGridLayoutGroup.updateChildrenCallback = FrameUpdateChildrenCallback;

        //mBgGridLayoutGroup.SetAmount(bgInfoList.Count);
        //mBgGridLayoutGroup.updateChildrenCallback = BgUpdateChildrenCallback;


        InitHeadList();
        InitFrameList();
        //InitBgList();

        FrameComponentToChange();

        // DressUpMask.SetActive(true);
        // Backe.SetActive(true);
        //  DressUpMaskBgMove(true);
    }

    private void FrameComponentToChange()
    {
        FrameContentCs.enabled = true;
        FrameContentGl.enabled = true;

        FrameContent.gameObject.SetActive(false);

        FrameContent.gameObject.SetActive(true);
    }

    private void InitHeadList()
    {
        // int len = 0;
        // if (mHeadItemViewList == null)
        // {
        //     mHeadItemViewList = new List<ProfileHeadItemView>();
        //     len = 6;
        //     for (int i = 0; i < len; i++)
        //     {
        //         GameObject go = Instantiate(HeadItemView);
        //         go.transform.SetParent(HeadContent.transform);
        //        
        //         ProfileHeadItemView itemView = go.GetComponent<ProfileHeadItemView>();
        //         if (itemView != null)
        //         {
        //             if (headIconInfoList.Count > i)
        //             {
        //                 itemView.Init(headIconInfoList[i], HeadSelectHandler);
        //                 itemView.gameObject.SetActive(true);
        //             }
        //             else
        //                 itemView.gameObject.SetActive(false);
        //             mHeadItemViewList.Add(itemView);
        //         }
        //     }
        // }
        // else
        // {
        //     len = 5;
        //     for (int i = 0; i < len; i++)
        //     {
        //         ProfileHeadItemView itemView = mHeadItemViewList[i];
        //         if (itemView != null)
        //         {
        //             if (headIconInfoList.Count > i)
        //             {
        //                 itemView.Init(headIconInfoList[i], HeadSelectHandler);
        //                 itemView.gameObject.SetActive(true);
        //             }
        //             else
        //                 itemView.gameObject.SetActive(false);
        //         }
        //     }
        // }
    }

    private void InitFrameList()
    {
        int len = 0;
        if (mFrameItemViewList == null)
        {
            mFrameItemViewList = new List<ProfileFrameItemView>();
            len = 6;
            for (int i = 0; i < len; i++)
            {
                GameObject go = Instantiate(FrameItemView);
                go.transform.SetParent(FrameContent.transform);
                ProfileFrameItemView itemView = go.GetComponent<ProfileFrameItemView>();
                if (itemView != null)
                {
                    if (headFrameInfoList.Count > i)
                    {
                        itemView.Init(headFrameInfoList[i], FrameSelectHandler);
                        itemView.gameObject.SetActive(true);
                    }
                    else
                        itemView.gameObject.SetActive(false);

                    mFrameItemViewList.Add(itemView);
                }
            }
        }
        else
        {
            len = 5;
            for (int i = 0; i < len; i++)
            {
                ProfileFrameItemView itemView = mFrameItemViewList[i];
                if (itemView != null)
                {
                    if (headFrameInfoList.Count > i)
                    {
                        itemView.Init(headFrameInfoList[i], FrameSelectHandler);
                        itemView.gameObject.SetActive(true);
                    }
                    else
                        itemView.gameObject.SetActive(false);
                }
            }
        }
    }

    //【装扮头像】
    public void ShowAvatar(int avatarId)
    {
        if (HeadIcon != null)
        {
            //【调用lua公共方法 加载头像】   -1代码当前装扮的头像    
            XLuaManager.Instance.CallFunction("GameHelper", "ShowAvatar", avatarId, HeadIcon);
        }

        mCurHeadId = avatarId;
    }

    //【装扮头像框】
    public void ShowAvatarframe(int avatarId)
    {
        if (HeadFrame != null)
        {
            //【调用lua公共方法 加载头像框】   -1代码当前装扮的头像框   
            XLuaManager.Instance.CallFunction("GameHelper", "ShowAvatarframe", avatarId, HeadFrame);
        }
    }


    private void FrameUpdateChildrenCallback(int index, Transform trans)
    {
        if (headFrameInfoList != null)
        {
            if (index < headFrameInfoList.Count)
            {
                trans.gameObject.SetActive(true);
                if (headFrameInfoList[index] != null)
                {
                    trans.GetComponent<ProfileFrameItemView>().Init(headFrameInfoList[index], FrameSelectHandler);
                }
            }
            else
            {
                trans.gameObject.SetActive(false);
            }
        }
    }

    private void FrameSelectHandler(int vId, string vDesc, bool toUser)
    {
        if (toUser)
        {
            HeadFrame.sprite = ResourceManager.Instance.GetUISprite("ProfileForm/bg_touxkuang" + vId);
            if (mCurFrameId != vId)
            {
                mCustomizeHadChange = true;
            }

            mCurFrameId = vId;
        }

        //FrameItemDescTxt.text = vDesc;
    }

    private void BgUpdateChildrenCallback(int index, Transform trans)
    {
        if (bgInfoList != null)
        {
            trans.gameObject.SetActive(true);
            if (index < bgInfoList.Count)
            {
                if (bgInfoList[index] != null)
                {
                    trans.GetComponent<ProfileBgItemView>().Init(bgInfoList[index], BgSelectHandler);
                }
            }
            else
            {
                trans.gameObject.SetActive(false);
            }
        }
    }

    private void BgSelectHandler(int vId, string vDesc, bool toUse)
    {
        //BgItemDescTxt.text = vDesc;
        if (toUse)
        {
            //BgImage.sprite = ResourceManager.Instance.GetUISprite("ProfileForm/bg_img" + vId);
            mCurBgId = vId;
            if (UserDataManager.Instance.userInfo.data != null)
            {
                mCustomizeHadChange = true;
            }
        }
    }

    private void DressUpMaskBgMove(bool Bo)
    {
        if (Bo)
        {
            DressUpMaskBg.DOAnchorPosY(0, 0.5f);
        }
        else
        {
            DressUpMaskBg.DOAnchorPosY(-600, 0.5f).OnComplete(() => { DressUpMask.SetActive(false); });
        }
    }

    private void SpwanMyBookes()
    {
        MyBookEmpty.SetActive(false);

        DestroyMyBookItems();
        List<int> myBookIDs = GameDataMgr.Instance.userData.GetMyBookIds();
        for (int i = 0; i < myBookIDs.Count; ++i)
        {
            GameObject go = Instantiate(MyBooksItem);
            go.transform.SetParent(MyBooksContent);
            go.SetActive(true);
            //go.transform.SetSiblingIndex(0);
            MyBookInitsGo.Add(go);

            ProfileMyBookItem ProfileMyBookItem = go.GetComponent<ProfileMyBookItem>();
            ProfileMyBookItem.bookTypeName = "mybookItem";
            t_BookDetails bookDetail = GameDataMgr.Instance.table.GetBookDetailsById(myBookIDs[i]);
            ProfileMyBookItem.Init(bookDetail);
        }

        if (myBookIDs.Count <= 0)
        {
            MyBookEmpty.SetActive(true);
        }
    }

    #region facebook login

    private void SignInButtonOnclicke(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        var luaenv = XLuaManager.Instance.GetLuaEnv();
        var res = luaenv.DoString("logic.UIMgr:Open(logic.uiid.AccountInfo)");
    }

    private void ReLoadInfo()
    {
        GameDataMgr.Instance.userData.RemoveAllBook();

        var Localization = GameDataMgr.Instance.table.GetLocalizationById(190);
        UITipsMgr.Instance.PopupTips(Localization, false);

        //UITipsMgr.Instance.PopupTips("Log out Successful!", false);
        UserDataManager.Instance.LoginInfo.LastLoginChannel = 4;
        UserDataManager.Instance.LogOutDelLocalInfo();

        GameHttpNet.Instance.TouristLogin(TouristLoginCallBacke);
    }

    private void TouristLoginCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----TouristLoginGetToken---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            if (jo.code == 200)
            {
                TalkingDataManager.Instance.LoginAccount(EventEnum.LoginTouristResultSucc);
                GamePointManager.Instance.BuriedPoint(EventEnum.LoginOk, "", "", "", "", "0");
                HttpInfoReturn<TouristLoginInfo> TouristLoginInfo =
                    JsonHelper.JsonToObject<HttpInfoReturn<TouristLoginInfo>>(result);
                GameHttpNet.Instance.TOKEN = TouristLoginInfo.data.token;
                //UINetLoadingMgr.Instance.Show();
                GameHttpNet.Instance.GetUserInfo(GetUserInfoCallBack);

                RefreshUI();
            }
            else
            {
                TalkingDataManager.Instance.LoginAccount(EventEnum.LoginTouristResultFail);
            }
        }
    }

    /// <summary>
    /// FaceBook登录成功回调
    /// </summary>
    /// <param name="vNot"></param>
    private void FaceBookLoginSucc(Notification vNot)
    {
        LoginDataInfo loginInfo = vNot.Data as LoginDataInfo;

        if (loginInfo != null)
        {
            string userId = loginInfo.UserId;
            string tokenStr = loginInfo.Token;

            //SignInButtonText.text = "Log  out";

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

            LOG.Info("--- Self FB  --id-->" + userId + " userNick:" + userNick + " --token->" +
                     UserDataManager.Instance.UserData.IdToken);

            //UINetLoadingMgr.Instance.Show();

#if UNITY_ANDROID
#if CHANNEL_HUAWEI
            GameHttpNet.Instance.LoginByThirdParty(1, email, userNick, faceUrl, userId, UserDataManager.Instance.UserData.IdToken, "huawei", LoginByThirdPartyCallBack);
#else
            GameHttpNet.Instance.LoginByThirdParty(1, email, userNick, faceUrl, userId,
                UserDataManager.Instance.UserData.IdToken, "android", LoginByThirdPartyCallBack);
#endif
#else
            GameHttpNet.Instance.LoginByThirdParty(1,email,userNick,faceUrl,userId,UserDataManager.Instance.UserData.IdToken,"ios",LoginByThirdPartyCallBack);
#endif
        }
    }

    private void GoogleLoginSuccHandler(Notification vNot)
    {
        LoginDataInfo loginInfo = vNot.Data as LoginDataInfo;
        if (loginInfo != null)
        {
            string userId = loginInfo.UserId;
            string tokenStr = loginInfo.Token;

            if (loginInfo.OpenType != 1) return;

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


            LOG.Info("--- GoogleInfo  --id-->" + userId + " userNick:" + glInfo.UserName + " --token->" +
                     UserDataManager.Instance.UserData.IdToken + "----Email--->" + glInfo.Email);

#if UNITY_ANDROID
#if CHANNEL_HUAWEI
            GameHttpNet.Instance.LoginByThirdParty(0, loginInfo.Email, loginInfo.UserName, loginInfo.UserImageUrl, userId, UserDataManager.Instance.UserData.IdToken, "huawei", LoginByThirdPartyCallBack);
#else
            GameHttpNet.Instance.LoginByThirdParty(0, loginInfo.Email, loginInfo.UserName, loginInfo.UserImageUrl,
                userId, UserDataManager.Instance.UserData.IdToken, "android", LoginByThirdPartyCallBack);
#endif
#else
            GameHttpNet.Instance.LoginByThirdParty(0, loginInfo.Email, loginInfo.UserName, loginInfo.UserImageUrl, userId, UserDataManager.Instance.UserData.IdToken,"ios",LoginByThirdPartyCallBack);
#endif
        }
    }

    private void ThirdPartyLoginSuccHandler(Notification vNot)
    {
        var luaenv = XLuaManager.Instance.GetLuaEnv();
        var res = luaenv.DoString("logic.UIMgr:Close(logic.uiid.PlatformQuickLogin)" +
                                  "logic.UIMgr:Close(logic.uiid.AccountInfo)");
        LoginDataInfo loginInfo = vNot.Data as LoginDataInfo;
        if (loginInfo != null)
        {
            string userId = loginInfo.UserId;
            string tokenStr = loginInfo.Token;

            if (loginInfo.OpenType != 1) return;

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

            GameHttpNet.Instance.LoginByThirdParty(0, loginInfo.Email, loginInfo.UserName, loginInfo.UserImageUrl,
                userId, UserDataManager.Instance.UserData.IdToken, "android", LoginByThirdPartyCallBack);
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
                    if (hwUserInfo != null)
                    {
                        mCurLoginChannel = 3;
                        UserDataManager.Instance.UserData.UserID = hwUserInfo.playerId;
                        UserDataManager.Instance.UserData.IdToken = hwUserInfo.gameAuthSign;
                        GameHttpNet.Instance.LoginByThirdParty(2, "", hwUserInfo.displayName, "", hwUserInfo.playerId,
                            "huawei", "huawei", LoginByThirdPartyCallBack);
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
                    UserDataManager.Instance.thirdPartyLoginInfo =
                        JsonHelper.JsonToObject<HttpInfoReturn<ThirdPartyReturnInfo>>(result);
                    if (UserDataManager.Instance.thirdPartyLoginInfo != null &&
                        UserDataManager.Instance.thirdPartyLoginInfo.data != null)
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
                    UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218) /*"TIPS"*/, jo.msg);
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
            if (jo.code == 200)
            {
                UserDataManager.Instance.userInfo =
                    JsonHelper.JsonToObject<HttpInfoReturn<UserInfoCont>>(arg.ToString());
                if (UserDataManager.Instance.userInfo.data.userinfo.firstplay == 0)
                {
                    IGGSDKManager.Instance.isNewUser = true;
                }
                else
                {
                    IGGSDKManager.Instance.isNewUser = false;
                    IGGAgreementManager.Instance.OnRequestStatusCustomClick();
                }

                if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data != null)
                {
                    UserDataManager.Instance.UserData.UserID = UserDataManager.Instance.userInfo.data.userinfo.uid;
                    UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.userInfo.data.userinfo.bkey, false);
                    UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.userInfo.data.userinfo.diamond,
                        false);
                    UserDataManager.Instance.UserData.bookList = new List<int>();
                    //UserDataManager.Instance.UserData.bookList.AddRange(UserDataManager.Instance.userInfo.data.userinfo.booklist);


                    GameDataMgr.Instance.SetServerTime(int.Parse(UserDataManager.Instance.userInfo.data.userinfo
                        .current_time));
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
                UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218) /*"TIPS"*/, jo.msg);
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

            RefreshUI();
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
                    UserDataManager.Instance.selfBookInfo =
                        JsonHelper.JsonToObject<HttpInfoReturn<SelfBookInfo>>(arg.ToString());

                    //清除书架之前账号所保存的所有书本的数据
                    GameDataMgr.Instance.userData.MyBookListClean();
                    UserDataManager.Instance.InitRecordServerBookData(); //记录书本的数据
                    // EventDispatcher.Dispatch(EventEnum.BookJoinToShelfEvent);//调用生成新的书本
                    //刷新我的书本
                    XLuaManager.Instance.GetLuaEnv().DoString(@"GameController.MainFormControl:ResetMyBookList()");
                    EventDispatcher.Dispatch(EventEnum.BookProgressUpdate);
                    DoEnter();

                    SpwanMyBookes(); //生成个人中心的我的书本

                    if (compleNumber != null)
                        compleNumber.text = UserDataManager.Instance.selfBookInfo.data.read_chapter_count.ToString();
                    if (MyReadBookNumber != null)
                        MyReadBookNumber.text =
                            UserDataManager.Instance.selfBookInfo.data.favorite_book.Count.ToString();
                }
                else if (jo.code == 277)
                {
                    UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218) /*"TIPS"*/, jo.msg);
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
                    UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218) /*"TIPS"*/, jo.msg);
                    return;
                }
            }, null);
        }
    }

    private void DoEnter()
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

        if (UserDataManager.Instance.thirdPartyLoginInfo != null &&
            UserDataManager.Instance.thirdPartyLoginInfo.data != null &&
            UserDataManager.Instance.thirdPartyLoginInfo.data.isfirst == 1)
        {
            UserDataManager.Instance.thirdPartyLoginInfo.data.isfirst = 0;
            //UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, GameDataMgr.Instance.table.GetLocalizationById(224)/*"First login Successful! You have already received 10 diamonds."*/, AlertType.Sure, null, "OK");
        }
    }

    #endregion


    private void EnterButtonOnclicke(PointerEventData data)
    {
        if (UserDataManager.Instance.GuidStupNum == (int) CatGuidEnum.PersonalCenterCatEnter)
        {
            UserDataManager.Instance.isFirstCatEnt = true;
            CUIManager.Instance.CloseForm(UIFormName.CatGuid);
            UserDataManager.Instance.GuidStupNum += 1;
            GameHttpNet.Instance.UserpetguideChange(UserDataManager.Instance.GuidStupNum, UserpetguideChangeCall);
        }

        if (UserDataManager.Instance.profileData.data.guide_id == (int) CatGuidEnum.PlaceHuangyuandianYes)
        {
            //如果是在商店放置装饰物那中断，则引导去装饰物那里
            UserDataManager.Instance.GuidStupNum = (int) CatGuidEnum.DecorationsButtonOn;
            UserDataManager.Instance.isFirstCatEnt = true;
        }

        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);

        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.PostGetSceneInfo(ProcessGetSceneInfo);
    }

    private void UserpetguideChangeCall(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetPropertyHandler---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);

        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                ////UINetLoadingMgr.Instance.Close();              
                if (jo.code == 200)
                {
                }
            }, null);
        }
    }

    private void ProcessGetSceneInfo(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetpetgiftinfoCallbacke---->" + result);

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.SceneInfo = JsonHelper.JsonToObject<HttpInfoReturn<SceneInfo>>(result);

                    GameDataMgr.Instance.table.GetCatInMapData();
                    CUIManager.Instance.OpenForm(UIFormName.CatLoading);
                }
            }, null);
        }
    }

    private void CheckCatGuide()
    {
        //添加引导的相关的判断
        //1.性格是否解锁
        //2.是否已经指引过了
        //3.是否引导的第一步

        if (UserDataManager.Instance.profileData != null)
            LOG.Info("进入猫的指引,guide_id:" + UserDataManager.Instance.profileData.data.guide_id);

        if (!UserDataManager.Instance.CheckGameFunIsOpen(2) || !UserDataManager.Instance.CheckTraitIsUnlock()) return;
        CatEnterBg.gameObject.SetActive(UserDataManager.Instance.CheckGameFunIsOpen(2));

        if (UserDataManager.Instance.profileData.data.guide_id == 2)
        {
            UserDataManager.Instance.isFirstCatEnt = true;
            UserDataManager.Instance.GuidStupNum = (int) CatGuidEnum.PersonalCenterCatEnter;
            CUIManager.Instance.OpenForm(UIFormName.CatGuid);
            EventDispatcher.Dispatch(EventEnum.CatGuidCanvasGroupOFF);
            LOG.Info("进入猫的指引,GuidStupNum:" + UserDataManager.Instance.GuidStupNum);
        }

        Invoke("SetPosInvoke", 0.3f);
    }

    private void SetPosInvoke()
    {
        CancelInvoke("SetPosInvoke");
        EventDispatcher.Dispatch(EventEnum.DoGuidStep, UserDataManager.Instance.GuidStupNum);
    }

    /// <summary>
    /// 设置引导点击的位置
    /// </summary>
    /// <param name="notice"></param>
    public void SetGuidPos(Notification notice)
    {
        if (UserDataManager.Instance.GuidStupNum == (int) CatGuidEnum.PersonalCenterCatEnter)
        {
            float SeenH = Mathf.Abs(UIMask.rect.height); //屏幕高度
            float SeenW = Mathf.Abs(UIMask.rect.width); //屏幕宽度
            float h1 = Mathf.Abs(Frame.offsetMax.y);
            float h2 = Mathf.Abs(CatEnterBg.anchoredPosition.y);
            float w1 = Mathf.Abs(CatEnterBg.anchoredPosition.x);
            float h3 = Mathf.Abs(EnterButton.anchoredPosition.y);
            float w2 = Mathf.Abs(EnterButton.anchoredPosition.x);

            float H = SeenH - h1 - h2 - h3;
            float W = w1 + w2;

            UserDataManager.Instance.GuidPos = new Vector2(W, H); /*CatMainButton.transform.localPosition;*/
            LOG.Info("h1:" + h1 + "--h2:" + h2 + "--w1:" + w1 + "--h3:" + h3 + "--w2:" + w2 + "--H:" + H + "--W：" + W);
        }
    }


    #region 新UI功能修改

    private void ResetBtnState()
    {
        if (UserDataManager.Instance.UserData.BgMusicIsOn == 1)
        {
            MusicaImage.sprite = ResourceManager.Instance.GetUISprite(OnIconPath);
        }
        else
        {
            MusicaImage.sprite = ResourceManager.Instance.GetUISprite(OffIconPath);
        }


        if (UserDataManager.Instance.UserData.TonesIsOn == 1)
        {
            SonidosImage.sprite = ResourceManager.Instance.GetUISprite(OnIconPath);
        }
        else
        {
            SonidosImage.sprite = ResourceManager.Instance.GetUISprite(OffIconPath);
        }
    }

    private void MusicaButtonOnClicke(PointerEventData data)
    {
        GamePointManager.Instance.BuriedPoint(EventEnum.SettingMusic);
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        if (UserDataManager.Instance.UserData.BgMusicIsOn == 1)
        {
            UserDataManager.Instance.UserData.BgMusicIsOn = 0;
            AudioManager.Instance.StopBGM();
        }
        else
        {
            UserDataManager.Instance.UserData.BgMusicIsOn = 1;
            //AudioManager.Instance.PlayBGMAgain();
        }


        UserDataManager.Instance.SaveUserDataToLocal();
        ResetBtnState();
    }

    private void SonidosButtonOnClicke(PointerEventData data)
    {
        GamePointManager.Instance.BuriedPoint(EventEnum.SettingSound);
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

    private void PreguntasButton(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CUIManager.Instance.OpenForm(UIFormName.FAQ);
        //CUIManager.Instance.GetForm<FAQSprite>(UIFormName.FAQ).Init();

        //埋点*FAQ
        GamePointManager.Instance.BuriedPoint(EventEnum.FaqList);
    }

    private void PoliticaButton(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        Application.OpenURL(IggAgreementList[0].URL);
        //埋点*隐私政策
        GamePointManager.Instance.BuriedPoint(EventEnum.PrivacyPolicy);
    }

    private void TerminosButton(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        Application.OpenURL(IggAgreementList[1].URL);
        //埋点*服务条款
        GamePointManager.Instance.BuriedPoint(EventEnum.mService);
    }

    private void CommunityButton(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CommunityWindow.SetActiveEx(true);
    }

    private void FacebookButton(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        Application.OpenURL("https://www.facebook.com/Scripts-Untold-Secrets-107729237761206/");
    }

    private void TwitterButton(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        Application.OpenURL("https://twitter.com/ScriptsUntold");
    }

    private void InstagramButton(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        Application.OpenURL("https://www.instagram.com/Scripts_Untold_Secrets/");
    }

    private void GoogleButton(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        Application.OpenURL("https://mail.google.com/mail/u/2/#inbox");
    }

    private void YoutubeButton(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        Application.OpenURL("https://www.youtube.com/channel/UCZuXtAPaiEPTlSJAMx-otOg?view_as=subscriber");
    }

    private void BackButton(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CommunityWindow.SetActiveEx(false);
    }

    private void ContactanosButton(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        IGGSDKManager.Instance.OpenTSH();
    }


    private void EmailButton(PointerEventData data)
    {
        XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:Open(logic.uiid.UIEmailForm);");

        //埋点*邮件
        GamePointManager.Instance.BuriedPoint(EventEnum.MailBox);
    }

    void PakageButton(PointerEventData data)
    {
        XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:Open(logic.uiid.UIPakageForm);");

        //埋点*背包
        GamePointManager.Instance.BuriedPoint(EventEnum.PakageBox);
    }
    #endregion
}
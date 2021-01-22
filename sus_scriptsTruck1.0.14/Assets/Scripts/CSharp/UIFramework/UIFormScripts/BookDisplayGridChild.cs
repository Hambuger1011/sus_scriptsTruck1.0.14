using AB;
using System.Collections;
using System.Collections.Generic;
using UIEventTriggerBase;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UGUI;
using DG.Tweening;
using pb;
using GameCore.UI;
using XLua;
#if !NOT_USE_LUA
using DialogDisplaySystem = BookReadingWrapper;
#endif

public class BookDisplayGridChild : MonoBehaviour {

    public Text BookTitle;
    public Image BookChapterBG;
    //public Button ShareButton;
    public Button PlayButton;
    public Button BackButton;
    public Text ChapterText;
    public Text ChapterDiscription;
    public Text ReleaseTimesTxt;

    public UIToggle StarToogleButton;
    public Button btnComment;
    public Text CommentNum;

    public Button returns;
    //public Button BookSGame;
    //public GameObject[] BlueGames;
    public RectTransform ReseGame, PlayButtonText,BG;
    public int ChapterId;

    public Button ResetBookBtn;
    public Button ResetChapterBtn;
    public GameObject ReturnGamesBtn;
    public GameObject ReadNumGroup;
    //public Button SubscribeBtn;
    //public Text SubscribeDescTxt;

    public GameObject PlayKeyShowImage;
    public Text PlayKeyShowImageText;
    public Text ReaderNumberText;
    public GameObject ReseGameBg;
    public GameObject PlayButtonEx;
    public Image mCloseIcon;
    public Text mUpdateTimeTxt;
    public GameObject BookFree;



    private int mBookId;
    private string BookName;
    private bool OpenReturnUi = false;

    private bool mChapterIsOpen = false;
    private bool mIsComplete = false;
    private bool mIsLock = false;

    private UIVoidPointerEvent shareButtonEvent;
    private UIVoidPointerEvent commingSoonButtonEvent;
    private UIVoidPointerEvent readCompleteEvent;
    private UIVoidPointerEvent backButtonEvent;
    private Action<int> lockEvent;
    private UIVoidPointerEvent resetBookEvent;
    private Action<int> resetChapterEvent;
    private Action<int> startReadingEvent;
    private bool mIsSubcribe = false;

    private t_BookDetails mBookDetail;

    private int OnclickType = 1;// 1 代表重置章节  2 代表重置书本
    private int mReleaseState = 0;  //0：连载中，1：完结

    public CanvasGroup mCanvasGroup;

    private GameObject NeedKey;
    private GameObject ReseMask;
    private GameObject CloseButton;

    //道具相关
    Button btnKeyProp = null;
    GameObject propObj = null;
    Image propImage = null;
    GameObject objKeyPropDeleteLine = null;
    //道具相关_reset
    Button btnKeyProp2 = null;
    Image propImage2 = null;
    GameObject objKeyPropDeleteLine2 = null;


    UnityObjectRefCount m_cacheImage;
    public UnityObjectRefCount cacheImage
    {
        get
        {
            return m_cacheImage;
        }
        set
        {
            if (m_cacheImage != null)
            {
                m_cacheImage.Release();
            }
            m_cacheImage = value;
        }
    }

    private GameObject BookFree2;

    void Awake()
    {
#if CHANNEL_SPAIN
        //this.ShareButton.gameObject.CustomSetActive(false);
        // this.btnComment.gameObject.CustomSetActive(false);
#endif
        NeedKey = transform.Find("ReseGame/Bg/BtnResetChapter/NeedKey").gameObject;
        BookFree2 = transform.Find("ReseGame/Bg/BtnResetChapter/NeedKey/BookFree").gameObject;
        ReseMask = transform.Find("ReseGame/ReseMask").gameObject;
        CloseButton = transform.Find("Close").gameObject;

        btnComment.onClick.AddListener(OnCommentClick);//书评按钮
        //BookSGame.onClick.AddListener(BookSGameOnclicke);//书本的按钮
        returns.onClick.AddListener(ReturnButtonOnclicke);//重置按钮

        btnKeyProp = transform.Find("PlayButton/Image/propObj/btnKeyProp").GetComponent<Button>();
        propObj = transform.Find("PlayButton/Image/propObj").gameObject;
        propImage = btnKeyProp.transform.Find("Image").GetComponent<Image>();
        objKeyPropDeleteLine = transform.Find("PlayButton/Image/propObj/btnKeyProp/line_delete").gameObject;
        btnKeyProp.gameObject.SetActive(false);
        objKeyPropDeleteLine.SetActive(false);
        btnKeyProp.onClick.AddListener(OnClickKeyPropBtn);

        btnKeyProp2 = transform.Find("ReseGame/Bg/BtnResetChapter/NeedKey/btnKeyProp").GetComponent<Button>();
        propImage2 = btnKeyProp2.transform.Find("Image").GetComponent<Image>();
        objKeyPropDeleteLine2 = transform.Find("ReseGame/Bg/BtnResetChapter/NeedKey/btnKeyProp/line_delete").gameObject;
        btnKeyProp2.gameObject.SetActive(false);
        objKeyPropDeleteLine2.SetActive(false);
        btnKeyProp2.onClick.AddListener(OnClickKeyPropBtn);
        
        RefreshKeyPropBtnState();
        
        EventDispatcher.AddMessageListener(EventEnum.SetPropItem, SetPropItemHandler);

        //UIEventListener.AddOnClickListener(ShareButton.gameObject, OnShareHandler);//分享按钮
        UIEventListener.AddOnClickListener(PlayButton.gameObject, OnPlayerHandler);
        UIEventListener.AddOnClickListener(BackButton.gameObject, OnBackHandler);
        UIEventListener.AddOnClickListener(ResetBookBtn.gameObject, OnResetBookHandler);
        UIEventListener.AddOnClickListener(ResetChapterBtn.gameObject, OnResetChapterHandler);
        //UIEventListener.AddOnClickListener(SubscribeBtn.gameObject, SubscribeHandler);

        //UIEventListener.AddOnClickListener(YesBu, YesBuOnclick);
        //UIEventListener.AddOnClickListener(NoBu, NoBuOnclicke);
        UIEventListener.AddOnClickListener(ReseMask, ReseMaskClose);

        //ReseGame.DOAnchorPosY(100, 0.1f);
        UIEventListener.AddOnClickListener(CloseButton, Close);
        this.StarToogleButton.onValueChanged.AddListener(BookJoinToShelf);

    }
    
    private void SetPropItemHandler(Notification vNot)
    {
        PropInfoItem propInfoItem = vNot.Data as PropInfoItem;
        RefreshKeyPropBtnState(false);
    }

    private void OnShareHandler(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        //BlueGameShowTrue(3);
        if (shareButtonEvent != null)
            shareButtonEvent(data);
    }

    private void OnPlayerHandler(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        Debug.Log($"lzh ===========> OnPlayerHandler: {mChapterIsOpen} {mIsComplete} {mIsLock}");
        if (mChapterIsOpen)
        {
            if (mIsComplete)
            {
                if (readCompleteEvent != null)
                    readCompleteEvent(data);
            }
            else
            {
                if (startReadingEvent != null)
                    startReadingEvent(ChapterId);
            }
        }
        else if (mIsLock)
        {
            if (lockEvent != null)
                lockEvent(ChapterId);
        }
        else
        {
            if (commingSoonButtonEvent != null)
                commingSoonButtonEvent(data);
        }
    }

    private void OnBackHandler(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        if (backButtonEvent != null)
            backButtonEvent(data);
    }

    private void OnResetBookHandler(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        OnclickType = 2;
        //ReseGameBg.SetActive(false);
        //YNbg.SetActive(true);

        GamePointManager.Instance.BuriedPoint(EventEnum.ResetBook,"","",mBookId.ToString());
        YesBuOnclick(null);
    }

    private void OnResetChapterHandler(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        OnclickType = 1;
        //ReseGameBg.SetActive(false);
        //YNbg.SetActive(true);

        GamePointManager.Instance.BuriedPoint(EventEnum.ResetChapter,"","",mBookId.ToString());
        YesBuOnclick(null);
    }

    private void SubscribeHandler(PointerEventData data)
    {
        if (mIsSubcribe) return;
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.SetSubscribe(mBookId, SetSubscribeCallBack);
    }

    private void SetSubscribeCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----SetSubscribeCallBack---->" + result);
        if (result.Equals("error"))
        {
            return;
        }
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    //UserDataManager.Instance.SetBookSubscribeInfo(mBookId, 1, UserDataManager.Instance.GetBookSubscribeCount(mBookId) + 1);
                    //UserDataManager.Instance.SetBookSubscribeInfo(mBookId, 2, 1);
                    CheckBookOpenState();

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(144);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Subscription completed",false);
                }
            }, null);
        }

    }
   
    void OnDestroy()
    {
        cacheImage = null;
        btnComment.onClick.RemoveListener(OnCommentClick);//书评按钮
        //BookSGame.onClick.RemoveListener(BookSGameOnclicke);//书本的按钮
        returns.onClick.RemoveListener(ReturnButtonOnclicke);//重置按钮

        //UIEventListener.RemoveOnClickListener(ShareButton.gameObject, OnShareHandler);//分享按钮
        UIEventListener.RemoveOnClickListener(PlayButton.gameObject, OnPlayerHandler);
        UIEventListener.RemoveOnClickListener(BackButton.gameObject, OnBackHandler);
        UIEventListener.RemoveOnClickListener(ResetBookBtn.gameObject, OnResetBookHandler);
        UIEventListener.RemoveOnClickListener(ResetChapterBtn.gameObject, OnResetChapterHandler);
        //UIEventListener.RemoveOnClickListener(YesBu, YesBuOnclick);
        //UIEventListener.RemoveOnClickListener(NoBu, NoBuOnclicke);
        //UIEventListener.RemoveOnClickListener(SubscribeBtn.gameObject, SubscribeHandler);
        UIEventListener.RemoveOnClickListener(ReseMask, ReseMaskClose);

        UIEventListener.RemoveOnClickListener(CloseButton, Close);
        EventDispatcher.RemoveMessageListener(EventEnum.SetPropItem, SetPropItemHandler);
    }
    

    public void Init(t_BookDetails vBookDetails, int vBookID, string bookTitle, int vChapterId, string chapterText, 
        string chapterDiscription, bool chapterIsOpen, bool isComplete,bool isLock, 
        int vReleaseState,int vCommentCount,
        string bookChapterBGSpriteName, UIVoidPointerEvent shareButtonEvent,
        Action<int> vStartReadingEvent, UIVoidPointerEvent commingSoonButtonEvent, 
        UIVoidPointerEvent readCompleteEvent, UIVoidPointerEvent backButtonEvent,
        Action<int> lockEvent,UIVoidPointerEvent resetBookEvent,
        Action<int> resetChapterEvent)
    {
        mBookDetail = vBookDetails;
        mBookId = vBookID;
        BookTitle.text = bookTitle;
        

        if (vCommentCount >= 1000000)
        {
            CommentNum.text = string.Format("{0}m", (vCommentCount / 1000000.0f).ToString("0.0"));
        }
        else if (vCommentCount >= 1000)
        {
            CommentNum.text = string.Format("{0}k", (vCommentCount / 1000.0f).ToString("0.0"));
        }
        else
        {
            CommentNum.text = vCommentCount.ToString();
        }

        BookName = bookTitle;////////
       
        mChapterIsOpen = chapterIsOpen;
        mIsComplete = isComplete;
        mIsLock = isLock;

        this.shareButtonEvent = shareButtonEvent;
        this.startReadingEvent = vStartReadingEvent;
        this.commingSoonButtonEvent = commingSoonButtonEvent;
        this.readCompleteEvent = readCompleteEvent;
        this.backButtonEvent = backButtonEvent;
        this.lockEvent = lockEvent;
        this.resetBookEvent = resetBookEvent;
        this.resetChapterEvent = resetChapterEvent;

        //if (ReleaseTimesTxt != null)
        //    ReleaseTimesTxt.text = vReleaseTimes;

        ChapterId = vChapterId;
        ChapterText.text = chapterText;
        ChapterDiscription.text = chapterDiscription;

        Invoke("BgRectSet",0.8f);
       
        ReaderNumberText.text = "";


        List<int> myBookIDs = GameDataMgr.Instance.userData.GetMyBookIds();

       
         if (myBookIDs != null)
         {
             StarToogleButton.isOn = (myBookIDs.IndexOf(vBookID) != -1);
         }else
         {
             StarToogleButton.isOn = false;

            //标记书本已经收藏

         }

        BookChapterBG.sprite = ResourceManager.Instance.GetUISprite("BookDisplayForm/main_bg_picture2");
        ABSystem.ui.DownloadChapterBG(mBookId, (id, refCount) =>
        {
            if(BookChapterBG == null)
            {
                refCount.Release();
                return;
            }
            if (mBookId != id)
            {
                refCount.Release();
                return;
            }
            cacheImage = refCount;
            BookChapterBG.DOFade(0, 0).SetEase(Ease.Flash).Play();
            BookChapterBG.sprite = refCount.Get<Sprite>();
            BookChapterBG.DOFade(1, 0.2f).SetEase(Ease.Flash).Play();
        });
        //if (bookChapterBGSprite != null) BookChapterBG.sprite = ABSystem.ui.GetUITexture(AbTag.Global, string.Concat("assets/bundle/BookPreview/banner/bg_book", mBookId, ".png"));

        if (mChapterIsOpen)
        {
            if(isComplete)
            {
                mCanvasGroup.alpha = 0;
                PlayButton.GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("BookDisplayForm/bg_manek_03");
                PlayButton.transform.GetChild(0).GetComponent<Text>().text = CTextManager.Instance.GetText(295);
                PlayButtonEx.SetActive(false);
                btnKeyProp.gameObject.SetActive(false);
                propObj.gameObject.SetActive(false);
            }
            else
            {
                mCanvasGroup.alpha = 1;
                PlayButton.transform.GetChild(0).GetComponent<Text>().text = CTextManager.Instance.GetText(296);
                MyBooksDisINSTANCE.Instance.chapterIDSet(ChapterId);//储存当前的章节
                CheckChapterNeedPay();
                propObj.gameObject.SetActive(true);
            }
        }
        else
        {
            mCanvasGroup.alpha = 1;
            if (isLock)
            {
                PlayButton.GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("BookDisplayForm/bg_manek_03");
                PlayButton.transform.GetChild(0).GetComponent<Text>().text = CTextManager.Instance.GetText(297);
                PlayButtonEx.SetActive(false);
                btnKeyProp.gameObject.SetActive(false);
                propObj.gameObject.SetActive(false);
            }
             else
             {
                 PlayButton.GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("BookDisplayForm/bg_manek_03");
                 PlayButton.transform.GetChild(0).GetComponent<Text>().text = CTextManager.Instance.GetText(298);
                PlayButtonEx.SetActive(false);
                btnKeyProp.gameObject.SetActive(false);
                propObj.gameObject.SetActive(false);
            }
        }

        UpdateReadNum();
        CheckBookOpenState();

        showCloseBtn();
    }

    private void showCloseBtn()
    {
        mCloseIcon.DOKill();
        mCloseIcon.color = new Color(1, 1, 1, 0);
        mCloseIcon.rectTransform().anchoredPosition = new Vector2(0, -240);
        mCloseIcon.rectTransform().DOAnchorPosY(-150,0.3f).SetDelay(0.2f).SetEase(Ease.OutBack).Play();
        mCloseIcon.DOFade(1, 0.3f).SetDelay(0.2f).SetEase(Ease.Flash).Play();
    }

    /// <summary>
    /// </summary>
    private void BgRectSet()
    {
        CancelInvoke("BgRectSet");
        mCanvasGroup.alpha = 1;
    }

    public void UpdateReadNum()
    {
        int readcount = 0;
        if (UserDataManager.Instance.bookDetailInfo != null && UserDataManager.Instance.bookDetailInfo.data != null)
        {
            readcount = UserDataManager.Instance.bookDetailInfo.data.read_count;

            if(!string.IsNullOrEmpty(UserDataManager.Instance.bookDetailInfo.data.book_info.releaseday))
            {
                 mUpdateTimeTxt.text = UserDataManager.Instance.bookDetailInfo.data.book_info.releaseday;
            }
        }

        if (readcount >= 1000000)
        {
            ReaderNumberText.text = string.Format("{0}m", (readcount / 1000000.0f).ToString("0.0"));
        }
        else if (readcount >= 1000)
        {
            ReaderNumberText.text = string.Format("{0}k", (readcount / 1000.0f).ToString("0.0"));
        }
        else
        {
            ReaderNumberText.text = readcount.ToString();
        }
    }

    private void CheckBookOpenState()
    {
        if(mBookDetail != null)
        {
            bool bookIsOpen = mBookDetail.IsOpen == 0;//=0是开放，反人类

#if !CHANNEL_SPAIN
            //ShareButton.gameObject.SetActive(bookIsOpen);
#endif
            StarToogleButton.gameObject.SetActive(bookIsOpen);
            ReturnGamesBtn.gameObject.SetActive(bookIsOpen);
            //ReadNumGroup.gameObject.SetActive(bookIsOpen);
            PlayButton.gameObject.SetActive(bookIsOpen);
            //SubscribeBtn.gameObject.SetActive(!bookIsOpen);
            //SubscribeDescTxt.gameObject.SetActive(!bookIsOpen);

            //mIsSubcribe = UserDataManager.Instance.CheckBookIsSubscribe(mBookId) == 1;
            //SubscribeDescTxt.text = "<color='#39bbff'>" + UserDataManager.Instance.GetBookSubscribeCount(mBookId) + "</color> <color='#434343'>Subscribe</color>";

            //if (mIsSubcribe)
            //    SubscribeBtn.GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("BookDisplayForm/bg_manek_03");
            //else
            //    SubscribeBtn.GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("BookDisplayForm/bg_jsnale_03");
        }
    }
   
    private void BookJoinToShelf(bool vIsJoin)
    {
        if(vIsJoin)
        {
            //Debug.Log("天增书本");
            GameDataMgr.Instance.userData.AddMyBookId(mBookId,true);
        }else
        {
            GameDataMgr.Instance.userData.RemoveMyBookId(mBookId);
        }
        
    }


    //这个是打开书评的界面
    private void OnCommentClick()
    {
        //UITipsMgr.Instance.PopupTips("This option is not available yet.", false);
        //return;
        //BlueGameShowTrue(1);
        
        var luaenv = XLuaManager.Instance.GetLuaEnv();
        var res = luaenv.DoString(@"
return function()
    logic.bookReadingMgr:SetSelectBookId("+mBookId.ToString()+")"+
  " logic.UIMgr:Open(logic.uiid.Comment) " +
  "end");
        using (var func = (LuaFunction)res[0])
        {
            var f = (Action)func.Cast(typeof(Action));
            f();
        }
        this.OnBackHandler(null);
        // CUIManager.Instance.OpenForm(UIFormName.Comment);
        // var form = CUIManager.Instance.GetForm<CommentForm>(UIFormName.Comment);
        // form.SetData(mBookId, ChapterId, BookTitle.text, BookChapterBG.sprite);
    }

    /// <summary>
    /// 这个是书本按钮的点击的效果
    /// </summary>
    private void BookSGameOnclicke()
    {
        //BlueGameShowTrue(0);
        //Debug.Log("书本按钮");
    }

    public bool isShow = false;
    //这是点击打开重置界面的按钮
    private void ReturnButtonOnclicke()
    {
        isShow = true;
        //这个是打开重置界面
        ReseGame.gameObject.SetActive(true);
     
        //显示是否需要扣费的钥匙
        t_BookDetails bookDetails = GameDataMgr.Instance.table.GetBookDetailsById(mBookId);
        int restartCost = int.Parse(bookDetails.CharacterPricesArray[ChapterId - 1]);
        if (restartCost > 0)
        {
            //显示钥匙
            NeedKey.SetActive(true);
            RefreshKeyPropBtnState();
            //【限时活动免费读书 显示标签】
            this.Limit_time_Free();
        }
        else
        {
            NeedKey.SetActive(false);
        }
    }

    /// <summary>
    /// 关闭书本重置界面
    /// </summary>
    /// <param name="data"></param>
    public void ReseMaskClose(PointerEventData data)
    {
        isShow = false;
        ReseGame.gameObject.SetActive(false);

    }



    /// <summary>
    /// 【限时活动免费读书 显示标签】
    /// </summary>
    public void Limit_time_Free()
    {
        if (this.BookFree != null)
        {
            //【调用lua公共方法 限时活动免费读书 显示标签】
            XLuaManager.Instance.CallFunction("GameHelper", "ShowFree", this.BookFree);
        }

        if (this.BookFree2 != null)
        {
            //【调用lua公共方法 限时活动免费读书 显示标签】
            XLuaManager.Instance.CallFunction("GameHelper", "ShowFree", this.BookFree2);
        }
    }



    /// <summary>
    /// 这个是显示阅读本章节的时候是否需要钥匙的显示
    /// </summary>
    public void CheckChapterNeedPay()
    {
        if (mChapterIsOpen && !mIsComplete)
        {
            if (mBookDetail.CharacterPricesArray.Length > 0 && mBookDetail.CharacterPricesArray.Length >= ChapterId)
            {
                int continueCost = 0;
                var curChapterPrice = GameDataMgr.Instance.table.GetChapterDivedeById(mBookId, ChapterId);
                if (curChapterPrice != null)
                    continueCost = curChapterPrice.chapterPay;
                else
                    continueCost = int.Parse(mBookDetail.CharacterPricesArray[ChapterId - 1]);

                PlayKeyShowImageText.text = continueCost.ToString();
                if (continueCost > 0&& UserDataManager.Instance.bookDetailInfo.data.cost_max_chapter< ChapterId)
                {
                    //play付费显示
                    PlayButton.GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("BookDisplayForm/bg_jsnale_03");
                    PlayButtonText.anchoredPosition = new Vector2(-32, 3f);
                    PlayKeyShowImage.SetActive(true);

                    RefreshKeyPropBtnState();

                    //【限时活动免费读书 显示标签】
                    this.Limit_time_Free();
                }
                else { 
                     PlayButton.GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("BookDisplayForm/bg_jsnale_03");
                    PlayButtonText.anchoredPosition = new Vector2(2f, 3f);
                    PlayKeyShowImage.SetActive(false);
                }

                //if (!UserDataManager.Instance.CheckBookHasBuy(mBookId) && !DialogDisplaySystem.Instance.CheckHasPayChapter(mBookId, ChapterId))
                //{
                //    int continueCost = 0;
                //    var curChapterPrice = GameDataMgr.Instance.table.GetChapterDivedeById(mBookId, ChapterId);
                //    if (curChapterPrice != null)
                //        continueCost = curChapterPrice.chapterPay;
                //    else
                //        continueCost = int.Parse(mBookDetail.CharacterPricesArray[ChapterId - 1]);

                //    PlayKeyShowImageText.text =continueCost.ToString();
                //    if (continueCost > 0)
                //    {
                //        //play付费显示

                //        PlayButton.GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("BookDisplayForm/bg_jsnale_03");
                //        PlayButtonText.anchoredPosition = new Vector2(-32, 5.6f);
                //        PlayKeyShowImage.SetActive(true);
                //    }
                //    else
                //    {
                //        PlayButton.GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("BookDisplayForm/bg_jsnale_03");
                //        PlayButtonText.anchoredPosition = new Vector2(1.7f, 5.6f);
                //        PlayKeyShowImage.SetActive(false);
                //    }
                //}
                //else
                //{
                //    PlayButton.GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("BookDisplayForm/bg_jsnale_03");
                //    PlayButtonText.anchoredPosition = new Vector2(1.7f, 5.6f);
                //    PlayKeyShowImage.SetActive(false);
                //}
            }
        }
    } 
    
    public void YesBuOnclick(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        if (OnclickType==1)
        {
            // 重置章节
            if (mChapterIsOpen)
            {
                if (resetChapterEvent != null)
                    resetChapterEvent(ChapterId);
            }
        }
        else
        {
            // 重置书本
            //if (mIsOpen && !mIsComplete)
            //{
            LOG.Info("点击了重置书本");
                ReturnButtonOnclicke();
                if (resetBookEvent != null)
                    resetBookEvent(data);
            //}
        }
        isShow = false;
        ReseGame.gameObject.SetActive(false);

        //ReseGameBg.SetActive(true);
        //YNbg.SetActive(false);
    } 

    public void NoBuOnclicke(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        ReseGameBg.SetActive(true);
        //YNbg.SetActive(false);
    }


    private void Close(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_click);
        CUIManager.Instance.CloseForm(UIFormName.BookDisplayForm);
        // EventDispatcher.Dispatch(EventEnum.BookJoinToShelfEvent);
        //刷新我的书本
        XLuaManager.Instance.GetLuaEnv().DoString(@"GameController.MainFormControl:ResetMyBookList()");
    }

    private void OnClickKeyPropBtn()
    {
        UserDataManager.Instance.is_use_prop = !UserDataManager.Instance.is_use_prop;
        RefreshKeyPropBtnState();
    }
    void RefreshKeyPropBtnState(bool needSet = true)
    {
        PropInfo info = UserDataManager.Instance.userPropInfo_Key;
        if(info==null || info.discount_list.Count == 0 || info.discount_list[0].prop_num <= 0)
        {
            btnKeyProp.gameObject.SetActive(false);
            btnKeyProp2.gameObject.SetActive(false);
            return;
        }
        btnKeyProp.gameObject.SetActive(true);
        btnKeyProp2.gameObject.SetActive(true);
        objKeyPropDeleteLine.SetActive(UserDataManager.Instance.is_use_prop);
        objKeyPropDeleteLine2.SetActive(UserDataManager.Instance.is_use_prop);
        if (UserDataManager.Instance.is_use_prop)
        {
            propImage.sprite = ResourceManager.Instance.GetUISprite("PakageForm/Props_icon_Key Coupon_1");
            propImage2.sprite = ResourceManager.Instance.GetUISprite("PakageForm/Props_icon_Key Coupon_1");
            if (needSet)
                UserDataManager.Instance.SetLuckyPropItem(true, info.discount_list[0]);
        }
        else
        {
            propImage.sprite = ResourceManager.Instance.GetUISprite("PakageForm/com_icon_kyes1");
            propImage2.sprite = ResourceManager.Instance.GetUISprite("PakageForm/com_icon_kyes1");
            if (needSet)
                UserDataManager.Instance.SetLuckyPropItem(false, null);
        }

    }
}

using BookReading;
using pb;
using System;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if !NOT_USE_LUA
using DialogDisplaySystem = BookReadingWrapper;
#endif

[XLua.LuaCallCSharp]
public class UIChapterSwitch :
#if NOT_USE_LUA
UIBookReadingElement
#else
    MonoBehaviour
#endif
{
    public Button CloseBtn;
    public Button btnRestart;
    public Button btnContinue;
    public Button BtFeedback; 
    public GameObject restartKeyIcon;
    public GameObject continueKeyIcon;
    public GameObject NextUpBookItem;
    public ScrollRect scrollview;
    public Text txtContinueTip, ContinueTitle;
    public Text txtRestartTip, PriceTitle;
    public Text txtResCostNum;
    public Text txtConCostNum,KeyNumText;
    public GameObject UIMask;

    public GameObject BookFree1;
    public GameObject BookFree2;

    //道具相关
    bool isCheckedKeyPropBtn = true;
    Button btnKeyProp = null;
    GameObject objKeyPropDeleteLine = null;

    private int m_curBookID;
    private int m_curDialogID;
    private int m_curChapterID;
    private int m_nextDialogueID;

    private int restartCost;
    private int continueCost;


    private bool BookReadComplete;

#if NOT_USE_LUA
    private BookReadingForm _form;
    public override void Bind(BookReadingForm form)
    {
        _form = form;
        form.chapterSwitch = this;
    }
    public override void ResetUI()
    {
        Hide();
    }

    public override void SetSkin() { }
#endif

#if NOT_USE_LUA
    public override void Dispose()
#else
    void OnDestroy()
#endif
    {
        btnContinue.GetComponent<Image>().sprite = null;
        btnRestart.GetComponent<Image>().sprite = null;
        BtFeedback.GetComponent<Image>().sprite = null;

        btnRestart.onClick.RemoveListener(OnRestart);
        btnContinue.onClick.RemoveListener(OnContinue);
        BtFeedback.onClick.RemoveListener(FeedbackOnclick);
        CloseBtn.onClick.RemoveListener(ReturnToSelectChapterView);

        btnKeyProp = transform.Find("Frame/BtnNextChapter/IconImage/btnKeyProp").GetComponent<Button>();
        objKeyPropDeleteLine = transform.Find("Frame/BtnNextChapter/IconImage/btnKeyProp/line_delete").gameObject;
        btnKeyProp.gameObject.SetActive(false);
        objKeyPropDeleteLine.SetActive(false);
        btnKeyProp.onClick.AddListener(OnClickKeyPropBtn);
        RefreshKeyPropBtnState();

        UIEventListener.RemoveOnClickListener(KeyButton, KeyButtonOnclicke);
        EventDispatcher.RemoveMessageListener(EventEnum.OnKeyNumChange.ToString(), OnKeyNumChange);


       // UIEventListener.RemoveOnClickListener(UIMask, MastClose);

    }
    void Awake()
    {
        btnRestart.onClick.AddListener(OnRestart);
        btnContinue.onClick.AddListener(OnContinue);
        BtFeedback.onClick.AddListener(FeedbackOnclick);
       
        CloseBtn.onClick.AddListener(ReturnToSelectChapterView);

        KeyNumText.text = UserDataManager.Instance.UserData.KeyNum.ToString();
        UIEventListener.AddOnClickListener(KeyButton,KeyButtonOnclicke);
        EventDispatcher.AddMessageListener(EventEnum.OnKeyNumChange.ToString(), OnKeyNumChange);


       // UIEventListener.AddOnClickListener(UIMask, MastClose);

        Transform bgTrans = this.gameObject.transform.Find("Frame");
        if (GameUtility.IpadAspectRatio() && bgTrans != null)
            bgTrans.localScale = Vector3.one * 0.7f;
    }


    /// <summary>
    /// 【限时活动免费读书 显示标签】
    /// </summary>
    public void Limit_time_Free(GameObject obj)
    {
        if (this.BookFree1 != null)
        {
            //【调用lua公共方法 限时活动免费读书 显示标签】
            XLuaManager.Instance.CallFunction("GameHelper", "ShowFree", obj);
        }
    }



#if NOT_USE_LUA
    public void SetData(int curDialogID, int vBookId, int vChapterId)
    {
        this.m_nextDialogueID = DialogDisplaySystem.Instance.CurrentDialogData.next;
#else
    public void SetData(int vBookId, int vChapterId, int curDialogID, int nextDialogueID)
    {
        this.m_nextDialogueID = nextDialogueID;
#endif
        m_curDialogID = curDialogID;
        m_curBookID = vBookId;
        m_curChapterID = vChapterId;

        
        
        //UINetLoadingMgr.Instance.Show();
        var bookData = BookReadingWrapper.Instance.CurrentBookData;
        GameHttpNet.Instance.SendPlayerProgress(m_curBookID,
               m_curChapterID, m_curDialogID,StartReadChapterCallBack);

        txtRestartTip.text = CTextManager.Instance.GetText(278) + m_curChapterID;
        txtContinueTip.text = CTextManager.Instance.GetText(279);

        var cfg = GameDataMgr.Instance.table.GetBookDetailsById(m_curBookID);
        int chapterId = m_curChapterID;
        if (cfg != null && cfg.CharacterPricesArray.Length > 0)
        {
            if (cfg.CharacterPricesArray.Length >= chapterId)
            {
                var curChapterPrice = GameDataMgr.Instance.table.GetChapterDivedeById(m_curBookID, chapterId);
                if (curChapterPrice != null)
                    restartCost = curChapterPrice.chapterPay;
                else
                    restartCost = int.Parse(cfg.CharacterPricesArray[chapterId - 1]);

                if (UserDataManager.Instance.CheckBookHasBuy(m_curBookID))
                    restartCost = 0;



                if (restartCost > 0)
                {
                    txtResCostNum.text = restartCost.ToString();
                    //btnRestart.GetComponent<Image>().sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/ChangeClothes/bg_btn.png");

                    btnRestart.GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("BookDisplayForm/tc-chongkan2");

                    //BtFeedback.GetComponent<Image>().sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/ChangeClothes/bg_btn2.png");
                    //txtRestartTip.rectTransform.anchoredPosition = new Vector2(0, 0);
                    restartKeyIcon.SetActive(true);

                    //【限时活动免费读书 显示标签】
                    this.Limit_time_Free(this.BookFree2);


                    txtRestartTip.gameObject.SetActive(false);
                    PriceTitle.gameObject.SetActive(true);
                    PriceTitle.text = CTextManager.Instance.GetText(278) + m_curChapterID;

                    //ResourceManager.Instance.GetUISprite("BookDisplayForm/bg_icon1_03");
                }
                else
                {
                    //btnRestart.GetComponent<Image>().sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/ChangeClothes/bg_btn2.png");

                    btnRestart.GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("BookDisplayForm/tc-chongkan");

                    //BtFeedback.GetComponent<Image>().sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/ChangeClothes/bg_btn2.png");
                    //txtRestartTip.rectTransform.anchoredPosition = new Vector2(50, 0);
                    restartKeyIcon.SetActive(false);

                    txtRestartTip.gameObject.SetActive(true);
                    PriceTitle.gameObject.SetActive(false);
                }

                int chapterOpenIndex = UserDataManager.Instance.bookDetailInfo.data.book_info.chapteropen;
                if (chapterOpenIndex == -1)
                    chapterOpenIndex = cfg.ChapterOpen;

                if (chapterId + 1 <= chapterOpenIndex)
                {
                    var nextChapterPrice = GameDataMgr.Instance.table.GetChapterDivedeById(m_curBookID, chapterId + 1);
                    if (nextChapterPrice != null)
                        continueCost = nextChapterPrice.chapterPay;
                    else
                        continueCost = int.Parse(cfg.CharacterPricesArray[chapterId]);

                    if (UserDataManager.Instance.CheckBookHasBuy(m_curBookID))
                        continueCost = 0;

                    if (continueCost > 0)
                    {
                        txtConCostNum.text = continueCost.ToString();
                        //btnContinue.GetComponent<Image>().sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/ChangeClothes/bg_btn.png");

                        btnContinue.GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("BookDisplayForm/bg_iap1");

                        //txtContinueTip.rectTransform.anchoredPosition = new Vector2(0, 0);
                        continueKeyIcon.SetActive(true);

                        //【限时活动免费读书 显示标签】
                        this.Limit_time_Free(this.BookFree1);

                        ContinueTitle.gameObject.SetActive(false);
                        txtContinueTip.gameObject.SetActive(true);

                        RefreshKeyPropBtnState();
                    }
                    else
                    {
                        //btnContinue.GetComponent<Image>().sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/ChangeClothes/bg_btn2.png");

                        btnContinue.GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("BookDisplayForm/bg_iap");

                        //txtContinueTip.rectTransform.anchoredPosition = new Vector2(50, 0);
                        continueKeyIcon.SetActive(false);

                        ContinueTitle.gameObject.SetActive(true);
                        txtContinueTip.gameObject.SetActive(false);
                    }

                    //if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data != null &&
                    //     UserDataManager.Instance.userInfo.data.userinfo.newpackage_status == 1 )
                    //{
                    //    TalkingDataManager.Instance.WatchTheAds(3);
                    //    //SdkMgr.Instance.admob.ShowRewardBasedVideo(LookVideoComplete);
                    //    SdkMgr.Instance.unityAds.ShowAds(LookVideoComplete);
                    //}
                }
                else
                {
                    //btnContinue.GetComponent<Image>().sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/ChangeClothes/bg_btn2.png");

                    btnContinue.GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("BookDisplayForm/bg_iap");

                    //txtContinueTip.rectTransform.anchoredPosition = new Vector2(50, 0);
                    continueKeyIcon.SetActive(false);

                    ContinueTitle.gameObject.SetActive(true);
                    txtContinueTip.gameObject.SetActive(false);
                }
            }
        }

    }


    public void Show()
    {
        var bookData = BookReadingWrapper.Instance.CurrentBookData;
        //TalkingDataManager.Instance.onCompleted("ReadChapterComplete_" + bookData.BookID + "_" + bookData.ChapterID);
#if CHANNEL_SPAIN
        SdkMgr.Instance.ads.ShowInterstitial("chapter-completed");
#endif
        this.gameObject.SetActiveEx(true);
        //ShowAwardGame();

        //【AF事件记录* 读完1个官方故事章节】
        AppsFlyerManager.Instance.FINISH_OFFICIAL_CHAPTER();

        //【AF事件记录* 第一次收藏书本】
        AppsFlyerManager.Instance.FIRST_FAVORITE_OFFICIAL_BOOK();

    }

    public void Hide()
    {
        this.gameObject.SetActiveEx(false);
    }

    private void ShowAwardGame()
    {
        AwardGameItem item = ResourceManager.Instance.LoadAssetBundleUI(UIFormName.AwardGame).GetComponent<AwardGameItem>();
        item.transform.SetParent(transform, false);
        item.transform.localPosition = Vector3.zero;
        item.transform.localScale = Vector3.one;
        item.IniteGame(3, 1);
    }

    private void OnRestart()
    {

        if (UserDataManager.Instance.UserData.KeyNum < restartCost && XLuaHelper.LimitTimeActivity == 0)
        {
            int type = MyBooksDisINSTANCE.Instance.GameOpenUItype();
            if (type==1)
            {
                if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data != null &&
                      UserDataManager.Instance.userInfo.data.userinfo.newpackage_status == 1)
                {
                    CUIManager.Instance.OpenForm(UIFormName.FirstGigtGroup);
                    CUIManager.Instance.GetForm<FirstGigtGroup>(UIFormName.FirstGigtGroup).GetType(1);
                    return;
                }
            }else if (type==2)
            {
                MyBooksDisINSTANCE.Instance.VideoUI(1);
                return;
            }
            else
            {
                //CUIManager.Instance.OpenForm(UIFormName.ChargeTipsForm);
                //ChargeTipsForm tipForm = CUIManager.Instance.GetForm<ChargeTipsForm>(UIFormName.ChargeTipsForm);

                CUIManager.Instance.OpenForm(UIFormName.NewChargeTips);
                NewChargeTips tipForm = CUIManager.Instance.GetForm<NewChargeTips>(UIFormName.NewChargeTips);


                if (tipForm != null)
                    tipForm.Init(1, restartCost, restartCost * 0.99f);
                return;
            }

        }

#if USE_SERVER_DATA
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.ResetChapter(m_curBookID, m_curChapterID,ResetChapterCallBack);
#else
        UserDataManager.Instance.CalculateKeyNum(-restartCost);
        DoRestartChapter();
#endif

    }

    private void ResetChapterCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----ResetChapterCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
             LoomUtil.QueueOnMainThread((param) => 
             {
                 //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.resetChapterOrBookInfo = JsonHelper.JsonToObject<HttpInfoReturn<ResetBookOrChapterResultInfo>>(result);
                    if (UserDataManager.Instance.resetChapterOrBookInfo != null && UserDataManager.Instance.resetChapterOrBookInfo.data != null)
                    {
                        int purchase = UserDataManager.Instance.UserData.KeyNum - UserDataManager.Instance.resetChapterOrBookInfo.data.bkey;
                        if (purchase > 0)
                            TalkingDataManager.Instance.OnPurchase("ResetChapter cost key", purchase, 1);

                        UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.resetChapterOrBookInfo.data.bkey);
                        UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.resetChapterOrBookInfo.data.diamond);
                    }
                    DoRestartChapter();
                     AudioManager.Instance.PlayTones(AudioTones.RewardWin);
                 }
                else if (jo.code == 202)    // 书本未读，无需重置
                {
                    DoRestartChapter();
                     AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                 }
                 else if(jo.code == 203)    //你的钥匙数量不足,无法重置
                {
                     AudioManager.Instance.PlayTones(AudioTones.LoseFail);

                     int type = MyBooksDisINSTANCE.Instance.GameOpenUItype();
                     if (type==1)
                     {
                         if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data != null &&
                                              UserDataManager.Instance.userInfo.data.userinfo.newpackage_status == 1)
                         {
                             CUIManager.Instance.OpenForm(UIFormName.FirstGigtGroup);
                             CUIManager.Instance.GetForm<FirstGigtGroup>(UIFormName.FirstGigtGroup).GetType(1);
                             return;
                         }
                     }else if (type==2)
                     {
                         MyBooksDisINSTANCE.Instance.VideoUI(1);
                         return;
                     }else
                     {
                         //CUIManager.Instance.OpenForm(UIFormName.ChargeTipsForm);
                         //ChargeTipsForm tipForm = CUIManager.Instance.GetForm<ChargeTipsForm>(UIFormName.ChargeTipsForm);

                         CUIManager.Instance.OpenForm(UIFormName.NewChargeTips);
                         NewChargeTips tipForm = CUIManager.Instance.GetForm<NewChargeTips>(UIFormName.NewChargeTips);


                         if (tipForm != null)
                             tipForm.Init(1, restartCost, restartCost * 0.99f);
                     }
                }
             }, null);
        }
    }

    private void DoRestartChapter()
    {
        if (m_curChapterID == 1)
        {
#if NOT_USE_LUA
            DialogDisplaySystem.Instance.ResetCurBookPlayerName();
#else
            var luaenv = XLuaManager.Instance.GetLuaEnv();
            var res = luaenv.DoString(@"logic.bookReadingMgr:ResetCurBookPlayerName()");
#endif
        }
        
        int beginDialogID = m_curChapterID <= 1 ? DialogDisplaySystem.Instance.BeginDialogID : DialogDisplaySystem.Instance.BeginDialogID + 1;
        DialogDisplaySystem.Instance.InitByBookID(
            m_curBookID,
            m_curChapterID,
            beginDialogID,
            DialogDisplaySystem.Instance.BeginDialogID,
            DialogDisplaySystem.Instance.EndDialogID
            );


        if (UserDataManager.Instance.bookDetailInfo != null && UserDataManager.Instance.bookDetailInfo.data != null)
            UserDataManager.Instance.bookDetailInfo.data.cost_max_chapter = m_curChapterID;

        DialogDisplaySystem.Instance.UpdatePayChapterRecordByReset(m_curBookID, m_curChapterID);

        TalkingDataManager.Instance.onStart("ReadChapterStart_" + m_curBookID + "_" + m_curChapterID);
        
        DialogDisplaySystem.Instance.PrepareReading(true);
    }

    private void OnContinue()
    {
        t_BookDetails bookDetails = GameDataMgr.Instance.table.GetBookDetailsById(m_curBookID);
        int[] chapterDivisionArray = bookDetails.ChapterDivisionArray;
        int nextChapterId = m_curChapterID + 1;
        bool readComplete = false;   //是否章节都阅读完成
        int chapterLen = chapterDivisionArray.Length;
        int chapterOpenIndex = UserDataManager.Instance.bookDetailInfo.data.book_info.chapteropen;
        if (chapterOpenIndex == -1)
            chapterOpenIndex = bookDetails.ChapterOpen;
        if (nextChapterId > chapterOpenIndex)
        {
            readComplete = true;
        }

        if (readComplete)
        {
            UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(222)/*"CHAPTER COMPLETE"*/, GameDataMgr.Instance.table.GetLocalizationById(229)/*"This chapter is not available yet. Please, stay tuned!"*/);
            //【AF事件记录*  读完1本官方故事的全部章节】
            AppsFlyerManager.Instance.FINISH_OFFICIAL_BOOK();
            return;
        }
        
        if (UserDataManager.Instance.UserData.KeyNum < continueCost && XLuaHelper.LimitTimeActivity == 0)
        {
            int type = MyBooksDisINSTANCE.Instance.GameOpenUItype();
            if (type==1)
            {

                if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data != null &&
                           UserDataManager.Instance.userInfo.data.userinfo.newpackage_status == 1)
                {
                    CUIManager.Instance.OpenForm(UIFormName.FirstGigtGroup);
                    CUIManager.Instance.GetForm<FirstGigtGroup>(UIFormName.FirstGigtGroup).GetType(1);
                    return;
                }
            }else if (type==2)
            {
                MyBooksDisINSTANCE.Instance.VideoUI(1);
                return;
            }else
            {

                //CUIManager.Instance.OpenForm(UIFormName.ChargeTipsForm);
                //ChargeTipsForm tipForm = CUIManager.Instance.GetForm<ChargeTipsForm>(UIFormName.ChargeTipsForm);

                CUIManager.Instance.OpenForm(UIFormName.NewChargeTips);
                NewChargeTips tipForm = CUIManager.Instance.GetForm<NewChargeTips>(UIFormName.NewChargeTips);


                if (tipForm != null)
                    tipForm.Init(1, continueCost, continueCost * 0.99f);
                return;
            }

        }

        //通知服务端扣费
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.BuyChapter (m_curBookID, nextChapterId, BuyChapterCallBack);
    }

    private void FeedbackOnclick()
    {
        CUIManager.Instance.OpenForm(UIFormName.FeedbackForm);

    }
    private void FeedbackCloseOnclick()
    {
        //Debug.Log("ddsd");
        CUIManager.Instance.CloseForm(UIFormName.FeedbackForm);
    }

   
    private void BuyChapterCallBack(HttpInfoReturn<BuyChapterResultInfo> buyData)
    {

        //UINetLoadingMgr.Instance.Close();
        if (buyData.code == 200)
        {
            UserDataManager.Instance.SetBuyChapterResultInfo(this.m_curBookID, buyData);
            if (UserDataManager.Instance.buyChapterResultInfo != null && UserDataManager.Instance.buyChapterResultInfo.data != null)
            {
                int purchase = UserDataManager.Instance.UserData.KeyNum - UserDataManager.Instance.buyChapterResultInfo.data.bkey;
                if (purchase > 0)
                    TalkingDataManager.Instance.OnPurchase("Read Next Chapter cost key", purchase, 1);

                UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.buyChapterResultInfo.data.bkey);
                UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.buyChapterResultInfo.data.diamond);
            }
            

            //var bookurl = jsonData["data"]["bookurl"];
            //TurnToNextChapter(bookurl != null? bookurl.ToString():"");

            TurnToNextChapter("");
        }
        else if (buyData.code == 203)//已经买过？
        {
            //var bookurl = jsonData["data"]["bookurl"];
            //TurnToNextChapter(bookurl != null ? bookurl.ToString() : "");

            TurnToNextChapter("");
        }
        else if (buyData.code == 204)
        {
            int type = MyBooksDisINSTANCE.Instance.GameOpenUItype();
            if (type == 1)
            {
                if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data != null &&
             UserDataManager.Instance.userInfo.data.userinfo.newpackage_status == 1)
                {
                    CUIManager.Instance.OpenForm(UIFormName.FirstGigtGroup);
                    CUIManager.Instance.GetForm<FirstGigtGroup>(UIFormName.FirstGigtGroup).GetType(1);
                    return;
                }
            }
            else if (type == 2)
            {
                MyBooksDisINSTANCE.Instance.VideoUI(1);
                return;
            }
            else
            {
                //CUIManager.Instance.OpenForm(UIFormName.ChargeTipsForm);
                //ChargeTipsForm tipForm = CUIManager.Instance.GetForm<ChargeTipsForm>(UIFormName.ChargeTipsForm);

                CUIManager.Instance.OpenForm(UIFormName.NewChargeTips);
                NewChargeTips tipForm = CUIManager.Instance.GetForm<NewChargeTips>(UIFormName.NewChargeTips);


                if (tipForm != null)
                    tipForm.Init(1, continueCost, continueCost * 0.99f);
            }
        }
        else if (buyData.code == 205)
        {
            int type = MyBooksDisINSTANCE.Instance.GameOpenUItype();
            if (type == 1)
            {

                if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data != null &&
                  UserDataManager.Instance.userInfo.data.userinfo.newpackage_status == 1)
                {
                    CUIManager.Instance.OpenForm(UIFormName.FirstGigtGroup);
                    CUIManager.Instance.GetForm<FirstGigtGroup>(UIFormName.FirstGigtGroup).GetType(1);
                    return;
                }
            }
            else if (type == 2)
            {
                MyBooksDisINSTANCE.Instance.VideoUI(1);
                return;
            }
            else
            {
                //CUIManager.Instance.OpenForm(UIFormName.ChargeTipsForm);
                //ChargeTipsForm tipForm = CUIManager.Instance.GetForm<ChargeTipsForm>(UIFormName.ChargeTipsForm);

                CUIManager.Instance.OpenForm(UIFormName.NewChargeTips);
                NewChargeTips tipForm = CUIManager.Instance.GetForm<NewChargeTips>(UIFormName.NewChargeTips);


                if (tipForm != null)
                    tipForm.Init(2, continueCost, continueCost * 0.99f);
            }

        }
        else if (buyData.code == 208)
        {
            LOG.Error("--BuyChapterCallBack--扣费失败");
        }
    }
    

    private void TurnToNextChapter(string bookurl)
    {
        t_BookDetails bookDetails = GameDataMgr.Instance.table.GetBookDetailsById(m_curBookID);
        int[] chapterDivisionArray = bookDetails.ChapterDivisionArray;
        int nextChapterId = m_curChapterID + 1;
        DialogDisplaySystem.Instance.ChangeBookDialogPath(m_curBookID, nextChapterId);
        DialogDisplaySystem.Instance.AddChapterzPayId(m_curBookID, nextChapterId);

        //GameHttpNet.Instance.GetCostChapterList(m_curBookID, UserOptionCostListHandler);

        int dialogueID = this.m_nextDialogueID;// DialogDisplaySystem.Instance.CurrentDialogData.next;
        int endDialogID = -1;
        int beginDialogID = m_curChapterID - 1 < 0 ? 1 : chapterDivisionArray[m_curChapterID - 1];
        if (m_curChapterID < chapterDivisionArray.Length)
        {
            endDialogID = chapterDivisionArray[m_curChapterID];
        }
        TalkingDataManager.Instance.onStart("ReadChapterStart_" + m_curBookID + "_" + nextChapterId);
        DialogDisplaySystem.Instance.InitByBookID(m_curBookID, nextChapterId, dialogueID, beginDialogID, endDialogID);
        //GameHttpNet.Instance.SendPlayerProgress(m_curBookID,
        //        m_curChapterID, m_curDialogID,
        //        0, 0, 0, 0, string.Empty, 0, 0, 0, string.Empty, 0, StartReadChapterCallBack);
        //EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice);

#if NOT_USE_LUA
        _form.setBGOnClickListenerActive(true);
        _form.ResetOperationTime();
        this.gameObject.SetActiveEx(false);
#endif
        this.Hide();
        DialogDisplaySystem.Instance.PrepareReading(true, bookurl);
    }

    private void UserOptionCostListHandler(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----UserOptionCostListHandler---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo.code == 200)
        {
            UserDataManager.Instance.bookChapterOptionCostList = JsonHelper.JsonToObject<HttpInfoReturn<BookOptionCostCont<BookOptionCostItemInfo>>>(result);
        }
    }
    


    private void StartReadChapterCallBack(object arg)
    {
        //UINetLoadingMgr.Instance.Close();
        string result = arg.ToString();
        LOG.Info("----GetDayLoginCallBack---->" + result);
        var info = JsonHelper.JsonToObject<HttpInfoReturn<SaveStep>>(result);
        switch (info.code)
        {
            case 200:
                {
                    UserDataManager.Instance.ResetMoney(1, info.data.user_key, false);
                    UserDataManager.Instance.ResetMoney(2, info.data.user_diamond, false);
                    bool adFlag = false;
                    if(info.data.chapter_end == 1)
                    {
                        if (info.data.show_ad == 1)
                        {
                            adFlag = true;
                        }
                    }

                    if (adFlag)
                    {
                        LOG.Info("需要看广告");
                        //SdkMgr.Instance.ShowAds(LookVideoComplete);

                        //【过场广告先屏蔽关闭】
                        //GoogleAdmobAds.Instance.chapterRewardedAd.ShowRewardedAd_Chapter(LookVideoComplete);
                    }
                    else
                    {
                        LOG.Info("付费用户不需要看广告");
                    }
                }
                break;
        }

    }

    private void ReturnToSelectChapterView()
    {
        //Debug.Log("+++++++===============dfdfdffaf");
        AudioManager.Instance.CleanClip();
        BookReadingWrapper.Instance.CloseForm();
        // CUIManager.Instance.OpenForm(UIFormName.MainForm/*, useFormPool: true*/);
        //打开主界面
        XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:Open(logic.uiid.UIMainForm);");
        CUIManager.Instance.OpenForm(UIFormName.BookDisplayForm);
        var view = CUIManager.Instance.GetForm<BookDisplayForm>(UIFormName.BookDisplayForm);
        view.InitByBookID(m_curBookID, true);
        this.gameObject.SetActiveEx(false);
        EventDispatcher.Dispatch(EventEnum.BookProgressUpdate);

        //是否已经评星过     0：否   1：是
        int IsRatingLevel = UserDataManager.Instance.userInfo.data.userinfo.is_store_score;
        Debug.LogError("是否已经评星过 :" + IsRatingLevel);
        if (IsRatingLevel == 0)
        {
            //打开评星
            XLuaManager.Instance.GetLuaEnv().DoString(@"GameHelper.OpenRating();");
        }
    }
    private void MastClose(PointerEventData data)
    {
        ReturnToSelectChapterView();
    }

    // private void LookVideoComplete(bool value)
    // {
    //     if(value)
    //         GameHttpNet.Instance.GetAdsReward(2,GetAdsRewardCallBack);
    // }
    //
    // private void GetAdsRewardCallBack(object arg)
    // {
    //     string result = arg.ToString();
    //     LOG.Info("----GetAdsRewardCallBack---->" + result);
    //     JsonObject jo = JsonHelper.JsonToJObject(result);
    //     if (jo != null)
    //     {
    //         LoomUtil.QueueOnMainThread((param) =>
    //         {
    //             if (jo.code == 200)
    //             {
    //                 AudioManager.Instance.PlayTones(AudioTones.RewardWin);
    //                 UserDataManager.Instance.adsRewardResultInfo = JsonHelper.JsonToObject<HttpInfoReturn<GetAdsRewardResultInfo>>(result);
    //                 Vector3 startPos = new Vector3(-188, -355);
    //                 Vector3 targetPos = new Vector3(306, 625);
    //                 RewardShowData rewardShowData = new RewardShowData();
    //                 rewardShowData.StartPos = startPos;
    //                 rewardShowData.TargetPos = targetPos;
    //                 rewardShowData.IsInputPos = false;
    //                 rewardShowData.Type = 1;
    //                 if (UserDataManager.Instance.adsRewardResultInfo != null && UserDataManager.Instance.adsRewardResultInfo.data != null)
    //                 {
    //                     rewardShowData.KeyNum = UserDataManager.Instance.adsRewardResultInfo.data.bkey;
    //                     rewardShowData.DiamondNum = UserDataManager.Instance.adsRewardResultInfo.data.diamond;
    //                     EventDispatcher.Dispatch(EventEnum.HiddenEggRewardShow, rewardShowData);    //观看视频，但是也是触发彩蛋的特效
    //                     TalkingDataManager.Instance.WatchTheAds(4);
    //                 }
    //             }
    //             else if (jo.code == 202)
    //             {
    //                 AudioManager.Instance.PlayTones(AudioTones.LoseFail);
    //                 UITipsMgr.Instance.PopupTips("You've reached the limit for today!", false);
    //             }
    //             else if (jo.code == 208)
    //             {
    //                 AudioManager.Instance.PlayTones(AudioTones.LoseFail);
    //             }
    //         }, null);
    //     }
    // }

    private void AutoDo()
    {
        CancelInvoke("AutoDo");
        OnContinue();
    }


    /// <summary>
    /// 生成推荐的书本
    /// </summary>
    private void spwnNextUpBookItem()
    {
        ArrayList bookSetList =MyBooksDisINSTANCE.Instance.returnbookRecommendList();//生成的书本数量
        int shu = bookSetList.Count;

        LOG.Info("推荐书本存储有："+shu+"书");

        for (int i=0;i<shu; i++)
        {
            if ((int)bookSetList[i]!= m_curBookID)
            {
                GameObject go = Instantiate(NextUpBookItem);
                go.SetActive(true);

                go.transform.SetParent(scrollview.content);
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                go.transform.localScale = Vector3.one;
                go.GetComponent<NextUpBookItem>().Init((int)bookSetList[i], this.gameObject);
            }
           
        }
    }

    private void OnEnable()
    {
        NextUpBookItem.SetActive(false);
        

        spwnNextUpBookItem();
    }


    public GameObject KeyButton;

    private void KeyButtonOnclicke(PointerEventData data)
    {       
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CUIManager.Instance.OpenForm(UIFormName.ChargeMoneyForm);
        CUIManager.Instance.OpenForm(UIFormName.MainFormTop);
        ChargeMoneyForm Char = CUIManager.Instance.GetForm<ChargeMoneyForm>(UIFormName.ChargeMoneyForm);
        Char.SetFormStyle(1);
        Char.GamePlayOpenKey();
        CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop).GamePlayTopOpen("UIChargeMoneyForm");
    }

#region 显示钥匙的数量
    private void OnKeyNumChange(Notification notification)
    {
        KeyNumText.text = ((int)notification.Data).ToString();

    }
#endregion

    
    public void LookVideoComplete()
    {
        LoomUtil.QueueOnMainThread((param) =>
        {
            var bookData = BookReadingWrapper.Instance.CurrentBookData;
            if (bookData != null)
            {
                UserDataManager.Instance.NowBookID = bookData.BookID;
                var uiform = CUIManager.Instance.OpenForm(UIFormName.ADSReward);
                if (uiform != null)
                {
                    var uiCtrl = uiform.GetComponent<ADSReward>();
                    uiCtrl.SetData(this.m_curBookID, this.m_curChapterID);
                }
            }
        }, null);
    }

    private void OnClickKeyPropBtn()
    {
        isCheckedKeyPropBtn = !isCheckedKeyPropBtn;
        RefreshKeyPropBtnState();
    }
    void RefreshKeyPropBtnState()
    {
        PropInfo info = UserDataManager.Instance.userPropInfo_Key;
        if (info == null || info.discount_list.Count == 0 || info.discount_list[0].prop_num <= 0)
        {
            btnKeyProp.gameObject.SetActive(false);
            return;
        }
        btnKeyProp.gameObject.SetActive(true);
        objKeyPropDeleteLine.SetActive(isCheckedKeyPropBtn);
        if (isCheckedKeyPropBtn)
        {
            UserDataManager.Instance.SetLuckyPropItem(isCheckedKeyPropBtn, info.discount_list[0]);
        }
        else
        {
            UserDataManager.Instance.SetLuckyPropItem(isCheckedKeyPropBtn, null);
        }
    }
}

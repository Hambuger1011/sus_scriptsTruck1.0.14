using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.UI;
using AB;
using pb;
using DG.Tweening;

#if !NOT_USE_LUA
using DialogDisplaySystem = BookReadingWrapper;
#endif

public class BookDisplayForm : BaseUIForm
{
    public RectTransform frameTrans;
    public Image UIMask;
    public RectTransform BookDisplayGridChildContent;
    public GameObject BookDisplayGridChildTpl;
    public ScrollViewPage SVPage;
   


    bool m_switchNext = false;

    private int mIndex;
    private int mChapterId;
    private int mCurBookId;
    private GameObject PlayShowNeedKeyGame;
    private List<BookDisplayGridChild> mDisplayItemList;

    private int mLastBookId = 0;
    private int continueCost;



    private void Awake()
    {
        SVPage.hasPrefabChild = true;
        BookDisplayGridChildTpl.SetActiveEx(false);
 
    }

    public override void OnOpen()
    {
        base.OnOpen();
        UIEventListener.AddOnClickListener(UIMask.gameObject, back);
       
        if (GameUtility.IpadAspectRatio() && frameTrans != null)
            frameTrans.localScale = Vector3.one * 0.7f;

        UIMask.color = new Color(0, 0, 0, 0);
        UIMask.DOFade(0.6f, 0.3f).SetEase(Ease.Flash).Play() ;
        SVPage.DOKill();
    }

    public override void OnClose()
    {
        
        base.OnClose();
        UIMask.DOKill();
        UIEventListener.RemoveOnClickListener(UIMask.gameObject, back);
        //CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop).displayFormClose();
        GameDataMgr.Instance.userData.RemoveCollectChange(mCurBookId);
        mLastBookId = 0;
    }
    public void InitByBookID(int bookID, bool switchNext = false)
    {
        mCurBookId = bookID;
        this.m_switchNext = switchNext;
        mDisplayItemList = new List<BookDisplayGridChild>();

        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.GetBookDetailInfo(bookID, GetBookDetailInfoCallBack);  //获得书本信息

        //if (mLastBookId == 0 || mLastBookId != bookID)
        //{
        //    mLastBookId = bookID;
        //    //UINetLoadingMgr.Instance.Show();
        //    GameHttpNet.Instance.GetBookDetailInfo(bookID, GetBookDetailInfoCallBack);  //获得书本信息

        //    //GameHttpNet.Instance.GetCostChapterList(bookID, GetCostChapterListCallBack);
        //    //GameHttpNet.Instance.GetBookOptionSelList(bookID, GetBookOptionSellistHandler);
        //}
    }

    private void UserOptionCostListHandler(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----UserOptionCostListHandler---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if(jo.code == 200)
        {
            UserDataManager.Instance.bookChapterOptionCostList = JsonHelper.JsonToObject<HttpInfoReturn<BookOptionCostCont<BookOptionCostItemInfo>>>(result);
        }
    }

    public void GetBookDetailInfoCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetBookDetailInfoCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);

        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.bookDetailInfo = JsonHelper.JsonToObject<HttpInfoReturn<BookDetailInfo>>(result);
                    //UserDataManager.Instance.RecordBookDetailInfo();
                    mChapterId = UserDataManager.Instance.bookDetailInfo.data.finish_max_chapter + 1;
                    if (mChapterId > UserDataManager.Instance.bookDetailInfo.data.book_info.chaptercount)
                        mChapterId = UserDataManager.Instance.bookDetailInfo.data.book_info.chaptercount;

                    //calculateGreaterIndex(mCurBookId);
                    initBookDisplayGridChild(mCurBookId,true);   //实例化物体

                    UpdateBookReadNum();
                }
            }, null);
        }
    }

    public void GetCostChapterListCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetCostChapterListCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if(jo != null)
        {
            LoomUtil.QueueOnMainThread((param) => 
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.bookCostChapterList = JsonHelper.JsonToObject<HttpInfoReturn<BookCostChapterListCont<BookCostChapterItemInfo>>>(result);
                    if (UserDataManager.Instance.bookCostChapterList != null && UserDataManager.Instance.bookCostChapterList.data != null)
                    {
                        List<BookCostChapterItemInfo> costList = UserDataManager.Instance.bookCostChapterList.data.costarr;
                        if (costList != null)
                        {
                            int len = costList.Count;
                            for (int i = 0; i < len; i++)
                            {
                                BookCostChapterItemInfo itemInfo = costList[i];
                                if (itemInfo != null)
                                {
                                    DialogDisplaySystem.Instance.AddChapterzPayId(itemInfo.bookid, itemInfo.chapterid);
                                }
                            }
                        }
                    }
                }
                PlayShowNeedKey();
            }, null);
        }
    }


    private void GetBookOptionSellistHandler(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetBookOptionSellistHandler---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                UserDataManager.Instance.BookOptSelInfo = JsonHelper.JsonToObject<HttpInfoReturn<BookOptionSelectInfo>>(result);
                if(UserDataManager.Instance.BookOptSelInfo.data != null && UserDataManager.Instance.BookOptSelInfo.data.options_arr != null)
                {
                    //UserDataManager.Instance.RecordBookOptionSelect(mCurBookId, UserDataManager.Instance.BookOptSelInfo.data.options_arr);
                }
            }, null);
        }
     }

    private void initBookDisplayGridChild(int bookID,bool checkTween = false)
    {
        t_BookDetails bookDetails = GameDataMgr.Instance.table.GetBookDetailsById(bookID);
        int len = 0;
        if (mDisplayItemList != null)
            len = mDisplayItemList.Count;
        int chapterCount = mChapterId; //得到开放的章节
        int chapterOpenIndex = UserDataManager.Instance.bookDetailInfo.data.book_info.chapteropen;
        if (chapterOpenIndex == -1)
            chapterOpenIndex = bookDetails.ChapterOpen;
        //chapterCount = chapterOpenIndex;//
        if (len < chapterOpenIndex)
            len = chapterOpenIndex;
        for (int i = 0; i < len; i++)
        {
            BookDisplayGridChild displayItem;
            int chapterId = i + 1;
            if (mDisplayItemList.Count > i)
            {
                displayItem = mDisplayItemList[i];
            }
            else
            {
                GameObject go = Instantiate(BookDisplayGridChildTpl, BookDisplayGridChildContent);
                displayItem = go.GetComponent<BookDisplayGridChild>();
                mDisplayItemList.Add(displayItem);
            }

            if (displayItem == null)
                return;
            displayItem.Init(bookDetails,
                bookID,
                bookDetails.BookName,
                chapterId,
                string.Format("Chapter {0} / {1}", chapterId, chapterOpenIndex),
                bookDetails.ChapterDiscriptionArray[i],
                i <= (mChapterId-1),
                i < (mChapterId -1),
                (i > (mChapterId - 1) && i < chapterOpenIndex),
                bookDetails.ChapterRelease,
                UserDataManager.Instance.bookDetailInfo.data.book_comment_count,
                "bg_book"+ bookID,
                share,
                startReading,
                commingSoonButtonEvent,
                readCompleteEvent,
                back,
                LockHandler,
                ResetBookHandler,
                ResetChapterHandler);

            displayItem.gameObject.SetActive(true);
        }

        SVPage.MoveToIndexNoTween(mChapterId - 1);
        if(checkTween && mChapterId == 1)
        {
            float posY = SVPage.transform.localPosition.y;
            SVPage.transform.localPosition = new Vector2(800, posY);
            SVPage.transform.DOLocalMoveX(0, 0.2f).SetEase(Ease.OutFlash).Play();
        }
    }

    private void ResetBookHandler(UnityEngine.EventSystems.PointerEventData data)
    {
        BookData bookData = UserDataManager.Instance.UserData.BookDataList.Find((bookdata) => bookdata.BookID == mCurBookId);
        if (bookData != null)
        {
            if (mIndex == 0 && (bookData.DialogueID <= 1))
            {
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(141);
                UITipsMgr.Instance.PopupTips(Localization, false);
                return;
            }
            else
            {
                #if USE_SERVER_DATA
                    GameHttpNet.Instance.ResetBook(mCurBookId, ResetBookCallBack);
                  //GameHttpNet.Instance.ResetChapterOrBook(0, mCurBookId, 1, ResetBookCallBack);
                #else
                    DoResetBook();
                #endif
            }
        }
        else
        {
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(141);
            UITipsMgr.Instance.PopupTips(Localization, false);
        }
    }

    /// <summary>
    /// 【限时活动免费读书 显示标签】
    /// </summary>
    public void Limit_time_Free()
    {
        if (mDisplayItemList!=null)
        {
            if (mDisplayItemList.Count > 0)
            {
                for (int i = 0; i < mDisplayItemList.Count; i++)
                {
                    mDisplayItemList[i].Limit_time_Free();
                }
            }
        }
    }


    private void ResetBookCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----ResetBookCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
             LoomUtil.QueueOnMainThread((param) => 
             {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.resetChapterOrBookInfo = JsonHelper.JsonToObject<HttpInfoReturn<ResetBookOrChapterResultInfo>>(result);

                     DoResetBook();
                 }
                 else if (jo.code == 202)    // 书本未读，无需重置
                {
                     AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                     DoResetBook();
                }
                else if (jo.code == 203)    //你的钥匙数量不足,无法重置
                {
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                    OpenChargeTip(1, 0, 1 * 0.99f, true);
                }
             }, null);
        }
    }

    private void OpenChargeTip(int vType,int vNum,float vPrice,bool vNeedHideMainTop = false)
    {


        //if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data != null &&
        //                 UserDataManager.Instance.userInfo.data.userinfo.newpackage_status == 1)
        //{
        //    CUIManager.Instance.OpenForm(UIFormName.FirstGigtGroup);            
        //}
        //else
        //{
            //if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data != null &&
            //           UserDataManager.Instance.userInfo.data.userinfo.newpackage_status == 1)
            //{
            //    CUIManager.Instance.OpenForm(UIFormName.FirstGigtGroup);
            //    return;
            //}

            //CUIManager.Instance.OpenForm(UIFormName.ChargeTipsForm);
            //ChargeTipsForm tipForm = CUIManager.Instance.GetForm<ChargeTipsForm>(UIFormName.ChargeTipsForm);

        CUIManager.Instance.OpenForm(UIFormName.NewChargeTips);
        NewChargeTips tipForm = CUIManager.Instance.GetForm<NewChargeTips>(UIFormName.NewChargeTips);

        if (tipForm != null)
            tipForm.Init(vType, vNum, vPrice, vNeedHideMainTop);
        //}
}

    private void DoResetBook()
    {
        mChapterId = 1;
        DialogDisplaySystem.Instance.UpdatePayChapterRecordByReset(mCurBookId, mChapterId);
        initBookDisplayGridChild(mCurBookId);
        BookData bookData = UserDataManager.Instance.UserData.BookDataList.Find((bookdata) => bookdata.BookID == mCurBookId);
        if(bookData  != null)
        {
            bookData.DialogueID = 1;
            bookData.ChapterID = 1;
            bookData.PlayerName = "PLAYER";
        }
        t_BookDetails bookDetails = GameDataMgr.Instance.table.GetBookDetailsById(mCurBookId);
        int endDialogId = 0;
        if (bookDetails != null && bookDetails.ChapterDivisionArray.Length > 0)
        {
            endDialogId = bookDetails.ChapterDivisionArray[0];
        }

        DialogDisplaySystem.Instance.InitByBookID(
            mCurBookId,1,1,1,endDialogId
            );
        EventDispatcher.Dispatch(EventEnum.BookProgressUpdate);

        var Localization = GameDataMgr.Instance.table.GetLocalizationById(140);
        UITipsMgr.Instance.PopupTips(Localization, false);

        //UITipsMgr.Instance.PopupTips("Reset Book Successful.", false);
    }

    private void ResetChapterHandler(int vChapterId)
    {
        
        t_BookDetails bookDetails = GameDataMgr.Instance.table.GetBookDetailsById(mCurBookId);
        mChapterId = vChapterId;
        mIndex = vChapterId - 1;

       BookData bookData = UserDataManager.Instance.UserData.BookDataList.Find((bookdata) => bookdata.BookID == mCurBookId);
       if (bookData != null && bookDetails != null)
       {
           if(mIndex == 0 && (bookData.DialogueID <=1))
           {
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(141);
                UITipsMgr.Instance.PopupTips(Localization, false);

                //UITipsMgr.Instance.PopupTips("Reset Chapter Successful.", false);
               return;
           }else if(mIndex > 0)
           {
               int tempDialogId = bookDetails.ChapterDivisionArray[mIndex - 1];
               if (tempDialogId == bookData.DialogueID || tempDialogId == (bookData.DialogueID + 1) || (tempDialogId + 1) == bookData.DialogueID)
               {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(141);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Reset Chapter Successful.", false);
                   return;
               }
           }
       }
        
        if (bookDetails != null && bookDetails.CharacterPricesArray.Length > 0)
        {
            if (bookDetails.CharacterPricesArray.Length >= mChapterId)
            {
                int restartCost = int.Parse(bookDetails.CharacterPricesArray[mChapterId - 1]);
                if (UserDataManager.Instance.CheckBookHasBuy(mCurBookId))
                    restartCost = 0;
                if (restartCost > 0)
                {
                    if (UserDataManager.Instance.UserData.KeyNum < restartCost && XLuaHelper.LimitTimeActivity == 0)
                    {
                        OpenChargeTip(1, restartCost, restartCost * 0.99f, true);
                        return;
                    }
#if USE_SERVER_DATA
                    
#else
        UserDataManager.Instance.CalculateKeyNum(-restartCost);
        DoRestartChapter();
#endif
                }

                //UINetLoadingMgr.Instance.Show();
                GameHttpNet.Instance.ResetChapter(mCurBookId, mChapterId, ResetChapterCallBack);
            }
        }
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

                    if (UserDataManager.Instance.bookDetailInfo != null && UserDataManager.Instance.bookDetailInfo.data != null)
                        UserDataManager.Instance.bookDetailInfo.data.cost_max_chapter = mChapterId;

                    DialogDisplaySystem.Instance.UpdatePayChapterRecordByReset(mCurBookId, mChapterId);
                    initBookDisplayGridChild(mCurBookId);
                    DoRestartChapter();
                }
                else if (jo.code == 202)    // 书本未读，无需重置
                {
                    DoRestartChapter();
                }
                else if (jo.code == 203)    //你的钥匙数量不足,无法重置
                {
                    OpenChargeTip(1, 0, 1 * 0.99f, true);
                }
            }, null);
        }
    }

    private void DoRestartChapter()
    {
        t_BookDetails bookDetails = GameDataMgr.Instance.table.GetBookDetailsById(mCurBookId);
        int[] chapterDivisionArray = bookDetails.ChapterDivisionArray;
        int beginDialogID = mIndex - 1 < 0 ? 1 : chapterDivisionArray[mIndex - 1];
        if (beginDialogID != 1)
            beginDialogID = beginDialogID + 1;  //代表下一个章节的第一个id
        int endDialogID = 0;
        if (mIndex < chapterDivisionArray.Length)
        {
            endDialogID = chapterDivisionArray[mIndex];
        }

        DialogDisplaySystem.Instance.InitByBookID(
            mCurBookId,
            mChapterId,
            beginDialogID,
            beginDialogID,
            endDialogID
            );
        BookData bookData = UserDataManager.Instance.UserData.BookDataList.Find((bookdata) => bookdata.BookID == mCurBookId);
        if (bookData != null)
        {
            bookData.DialogueID = DialogDisplaySystem.Instance.BeginDialogID;
            bookData.ChapterID = mChapterId;
            bookData.PlayerDetailsID = 1;
            bookData.PlayerClothes = 1;
            if (mChapterId == 1)
            {
                bookData.PlayerName = "PLAYER";
            }
            
        }
        GameHttpNet.Instance.SendPlayerProgress(mCurBookId, mChapterId, beginDialogID,StartReadChapterCallBack);
        EventDispatcher.Dispatch(EventEnum.BookProgressUpdate);

        var Localization = GameDataMgr.Instance.table.GetLocalizationById(141);
        UITipsMgr.Instance.PopupTips(Localization, false);

        //UITipsMgr.Instance.PopupTips("Reset Chapter Successful.", false);
    }


    private void StartReadChapterCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----StartReadChapterCallBack---->" + result);
    }


    private void calculateGreaterIndex(int bookID)
    {
        //mChapterId 获得最新读到的章节是什么

        BookData bookData = UserDataManager.Instance.UserData.BookDataList.Find((bookdata) => bookdata.BookID == bookID);
        t_BookDetails bookDetails = GameDataMgr.Instance.table.GetBookDetailsById(mCurBookId);
        int[] chapterDivisionArray = bookDetails.ChapterDivisionArray;
        if (bookData != null)
        {
            int saveDialogueID = bookData.DialogueID;
            bool readComplete = true;   //是否章节都阅读完成
            int chapterLen = chapterDivisionArray.Length;
            for (int i = 0; i < chapterLen; i++)
            {
                bool flag = false;
                if(m_switchNext)
                {
                    flag = chapterDivisionArray[i] - 1 > saveDialogueID;
                }
                else
                {
                    flag = chapterDivisionArray[i] > saveDialogueID;
                }
                if (flag)
                {
                    readComplete = false;
                    mChapterId = i+1;
                    break;
                }
            }

            if (readComplete)
                mChapterId = chapterLen;
        }else
        {
            mChapterId = 1;
        }
    }
    private void startReading(int vChapterId)
    {
        EventDispatcher.Dispatch(EventEnum.NavigationClose);
        EventDispatcher.Dispatch(EventEnum.NoticeClose);
        initDialogDisplaySystemData(vChapterId);
        AudioManager.Instance.CleanClip();
    }

    private void commingSoonButtonEvent(UnityEngine.EventSystems.PointerEventData data)
    {
      
        //CUIManager.Instance.OpenForm(UIFormName.ComingSoonDialogForm);
        //int index = data.pointerEnter.transform.parent.GetSiblingIndex();
        //var dialog = CUIManager.Instance.GetForm<ComingSoonDialogForm>(UIFormName.ComingSoonDialogForm);
        //dialog.SetMessage(string.Concat("CHAPTER ", " COMMING SOON"), string.Concat("This chapter is not available yet. Please, stay tuned!"));

        UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(228)/*"CHAPTER COMING SOON"*/, GameDataMgr.Instance.table.GetLocalizationById(229)/*"This chapter is not available yet. Please, stay tuned!"*/);
    }

    private void readCompleteEvent(UnityEngine.EventSystems.PointerEventData data)
    {
      
        //CUIManager.Instance.OpenForm(UIFormName.ComingSoonDialogForm);
        //int index = data.pointerEnter.transform.parent.GetSiblingIndex();
        //var dialog = CUIManager.Instance.GetForm<ComingSoonDialogForm>(UIFormName.ComingSoonDialogForm);
        //dialog.SetMessage(string.Concat("CHAPTER ", " COMPLETE"), string.Concat("This Chapter Read Complete"));

        UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(222)/*"CHAPTER COMPLETE"*/, GameDataMgr.Instance.table.GetLocalizationById(230)/*"This Chapter Read Complete"*/);
    }

    private void LockHandler(int vIndex)
    {

        //int index = vIndex;
        //CUIManager.Instance.OpenForm(UIFormName.ComingSoonDialogForm);
        //var dialog = CUIManager.Instance.GetForm<ComingSoonDialogForm>(UIFormName.ComingSoonDialogForm);
        //dialog.SetMessage(string.Concat("CHAPTER ", index, " LOCKED"), string.Concat("Complete Chapter ", index-1, " to unlock this chapter."));

        UIAlertMgr.Instance.Show(string.Concat(GameDataMgr.Instance.table.GetLocalizationById(231)+" "/*"CHAPTER "*/, vIndex, " "+GameDataMgr.Instance.table.GetLocalizationById(232)/*" LOCKED"*/), string.Concat(GameDataMgr.Instance.table.GetLocalizationById(233)+" "/*"Complete Chapter "*/, vIndex - 1, " "+GameDataMgr.Instance.table.GetLocalizationById(234) /*" to unlock this chapter."*/));
    }

    private void initDialogDisplaySystemData(int vChapterId)
    {
        int dialogueID = 1;
        DialogDisplaySystem.Instance.ChangeBookDialogPath(mCurBookId, vChapterId);
        BookData bookData = UserDataManager.Instance.UserData.BookDataList.Find((bookdata) => bookdata.BookID == mCurBookId);
        mChapterId = vChapterId;
        mIndex = vChapterId - 1;
        //检查章节是否付费
        t_BookDetails bookDetails = GameDataMgr.Instance.table.GetBookDetailsById(mCurBookId);
        if (bookDetails != null)
        {
            if (bookData != null)
            {
                int saveDialogueID = bookData.DialogueID;
                bool readComplete = false;   //是否章节都阅读完成
                int chapterOpenIndex = UserDataManager.Instance.bookDetailInfo.data.book_info.chapteropen;
                if (chapterOpenIndex == -1)
                    chapterOpenIndex = bookDetails.ChapterOpen;
                if (mChapterId > chapterOpenIndex)
                {
                    readComplete = true;
                }


                if (readComplete)
                {
                    UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(228)/*"CHAPTER COMING SOON"*/, GameDataMgr.Instance.table.GetLocalizationById(229)/*"This chapter is not available yet. Please, stay tuned!"*/);
                    return;
                }
            }

            if (bookDetails.CharacterPricesArray.Length > 0 && bookDetails.CharacterPricesArray.Length >= mChapterId)
            {
                continueCost = 0;
                var curChapterPrice = GameDataMgr.Instance.table.GetChapterDivedeById(mCurBookId, mChapterId);
                if (curChapterPrice != null)
                    continueCost = curChapterPrice.chapterPay;
                else
                    continueCost = int.Parse(bookDetails.CharacterPricesArray[mIndex]);

                if (UserDataManager.Instance.CheckBookHasBuy(mCurBookId))
                    continueCost = 0;


                if (continueCost > 0 && UserDataManager.Instance.bookDetailInfo.data.cost_max_chapter < mChapterId)
                {

                    if (UserDataManager.Instance.UserData.KeyNum < continueCost && XLuaHelper.LimitTimeActivity==0)
                    {
                        OpenChargeTip(1, continueCost, continueCost * 0.99f, true);
                        return;
                    }
#if USE_SERVER_DATA
                    //UINetLoadingMgr.Instance.Show();
                    GameHttpNet.Instance.BuyChapter(mCurBookId, mChapterId, BuyChapterCallBack);
#else
                        UserDataManager.Instance.CalculateKeyNum(-continueCost);
#endif
                }
                else
                {
                    //TurnToNextChapter();
                    GameHttpNet.Instance.BuyChapter(mCurBookId, mChapterId, BuyChapterCallBack);
                }

            }
        }

#if !USE_SERVER_DATA
        //TurnToNextChapter();
        GameHttpNet.Instance.BuyChapter(mCurBookId, mChapterId, BuyChapterCallBack);
#endif

    }

    private void BuyChapterCallBack(HttpInfoReturn<BuyChapterResultInfo> buyData)
    {
        //UINetLoadingMgr.Instance.Close();
        if (buyData.code == 200)
        {
            UserDataManager.Instance.SetBuyChapterResultInfo(this.mCurBookId, buyData);
            if (UserDataManager.Instance.buyChapterResultInfo != null && UserDataManager.Instance.buyChapterResultInfo.data != null)
            {
                var data = UserDataManager.Instance.buyChapterResultInfo.data;
                if (data.npc_detail != null && data.npc_detail.Count > 0)
                {
                    UserDataManager.Instance.RecordNpcInfo(mCurBookId, data.npc_detail[0]);
                }
                if (continueCost > 0)
                {
                    int purchase = UserDataManager.Instance.UserData.KeyNum - UserDataManager.Instance.buyChapterResultInfo.data.bkey;
                    if (purchase > 0)
                        TalkingDataManager.Instance.OnPurchase("Read Next Chapter cost key", purchase, 1);

                }

                //记录已经付费过的选项
                UserDataManager.Instance.SaveCharpterSelectHadBuy(UserDataManager.Instance.buyChapterResultInfo.data.pay_options);

                UserDataManager.Instance.ClothHadBuyClean();
                UserDataManager.Instance.SaveClothHadBuy(UserDataManager.Instance.buyChapterResultInfo.data.pay_clothes);
                
                UserDataManager.Instance.OutfitHadBuyClean();
                UserDataManager.Instance.SaveOutfitHadBuy(UserDataManager.Instance.buyChapterResultInfo.data.pay_outfit);

                UserDataManager.Instance.HairHadBuyClean();
                UserDataManager.Instance.SaveHairHadBuy(UserDataManager.Instance.buyChapterResultInfo.data.pay_hair);

                UserDataManager.Instance.CharacterHadBuyClean();
                UserDataManager.Instance.SaveCharacterHadBuy(UserDataManager.Instance.buyChapterResultInfo.data.pay_character);
                
                UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.buyChapterResultInfo.data.bkey);
                UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.buyChapterResultInfo.data.diamond);
            }

            TurnToNextChapter();
        }
        else if (buyData.code == 203)
        {
            TurnToNextChapter();
        }
        else if (buyData.code == 204)
        {
            OpenChargeTip(1, 0, 0, true);
        }
        else if (buyData.code == 205)
        {
            OpenChargeTip(2, 0, 0, true);
        }
        else if (buyData.code == 208)
        {
            LOG.Error("--BuyChapterCallBack--扣费失败");
        }
    }
    

    private void TurnToNextChapter()
    {       
        int dialogueID = 1;
        BookData bookData = UserDataManager.Instance.UserData.BookDataList.Find((bookdata) => bookdata.BookID == mCurBookId);
        t_BookDetails bookDetails = GameDataMgr.Instance.table.GetBookDetailsById(mCurBookId);
        int[] chapterDivisionArray = bookDetails.ChapterDivisionArray;
        int endDialogID = -1;
        int beginDialogID = mIndex - 1 < 0 ? 1 : chapterDivisionArray[mIndex - 1];
        if (mIndex < chapterDivisionArray.Length)
        {
            endDialogID = chapterDivisionArray[mIndex];
        }
        if (bookData != null)
        {
            int saveDialogueID = bookData.DialogueID;
            if (saveDialogueID <= beginDialogID && beginDialogID != 1)
            {
                TalkingDataManager.Instance.onStart("ReadChapterStart_" + UserDataManager.Instance.UserData.CurSelectBookID + "_" + (mIndex + 1));
                saveDialogueID = beginDialogID + 1;
            }
            if (mChapterId -1 == mIndex)
            {
                dialogueID = saveDialogueID;
            }
            else
            {
                dialogueID = beginDialogID;
            }
        }
        else
        {
            dialogueID = beginDialogID;
        }


        if(dialogueID == 1)
        {
            TalkingDataManager.Instance.onStart("ReadChapterStart_" + mCurBookId + "_" + 1);
        }

        //Debug.Log("增加====");
        UserDataManager.Instance.Nofistenter = true;
        GameDataMgr.Instance.userData.AddMyBookId(mCurBookId,false);
        DialogDisplaySystem.Instance.InitByBookID(mCurBookId, mIndex + 1, dialogueID, beginDialogID, endDialogID);
        

      
        ////UINetLoadingMgr.Instance.Show();
        //if (bookData != null && bookData.DialogueID <= beginDialogID)
        //{
        //    GameHttpNet.Instance.SendPlayerProgress(mCurBookId, mChapterId, beginDialogID,StartReadingBookHandler);
        //}else
        //{
        //    GameHttpNet.Instance.SendPlayerProgress(mCurBookId, mChapterId, dialogueID,StartReadingBookHandler);
        //}

        DialogDisplaySystem.Instance.AddChapterzPayId(mCurBookId, mChapterId);
        GameDataMgr.Instance.userData.CheckHasBookCollectChange();
        DialogDisplaySystem.Instance.PrepareReading(false);

        GetBarrageByChapter();
    }

    private void GetBarrageByChapter()
    {
        GameHttpNet.Instance.GetBookBarrageCountList(mCurBookId, mChapterId, GetBarrageByChapterCallBack);
    }

    private void GetBarrageByChapterCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetBarrageByChapterCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);

        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.BookBarrageCountList = JsonHelper.JsonToObject<HttpInfoReturn<BookBarrageCountList>>(result);
                }
            }, null);
        }
    }

    //private void StartReadingBookHandler(object arg)
    //{
    //    string result = arg.ToString();
    //    LOG.Info("----StartReadChapterCallBack---->" + result);
    //    JsonObject jo = JsonHelper.JsonToJObject(result);
    //    if (jo != null)
    //    {
    //        LoomUtil.QueueOnMainThread((param) =>
    //        {
    //            //UINetLoadingMgr.Instance.Close();
    //            if (jo.code == 200)
    //            {
    //                DialogDisplaySystem.Instance.AddChapterzPayId(mCurBookId, mChapterId);
    //                GameDataMgr.Instance.userData.CheckHasBookCollectChange();
    //                DialogDisplaySystem.Instance.PrepareReading(false);
    //            }
    //            else
    //            {
    //                if (!string.IsNullOrEmpty(jo.msg)) UITipsMgr.Instance.PopupTips(jo.msg, false);
    //            }
    //        }, null);
    //    }
    //}

    public bool ReseMaskISOpen()
    {
        bool boo = false;
        if (mDisplayItemList != null)
        {
            for (int i = 0; i < mDisplayItemList.Count; i++)
            {
                BookDisplayGridChild  displayItem = mDisplayItemList[i];
                if (displayItem != null && displayItem.isShow == true)
                {
                    boo = true;
                }
            }
        }
        return boo;
    }



    public void ReseMaskClose()
    {
        if (mDisplayItemList != null)
        {
            for (int i = 0; i < mDisplayItemList.Count; i++)
            {
                BookDisplayGridChild  displayItem = mDisplayItemList[i];
                if (displayItem != null)
                    displayItem.ReseMaskClose(null);
            }
        }
    }

    /// <summary>
    /// 这个是显示阅读这个章节需要使用钥匙不
    /// </summary>
    private void PlayShowNeedKey()
    {
        int len = 0;
        if (mDisplayItemList != null)
        {
            len = mDisplayItemList.Count;
            for (int i = 0; i < len; i++)
            {
                  BookDisplayGridChild  displayItem = mDisplayItemList[i];
                  if (displayItem != null)
                      displayItem.CheckChapterNeedPay();
            }
        }
    }

    /// <summary>
    /// 更新书本阅读数量
    /// </summary>
    private void UpdateBookReadNum()
    {
        int len = 0;
        if (mDisplayItemList != null)
        {
            len = mDisplayItemList.Count;
            for (int i = 0; i < len; i++)
            {
                BookDisplayGridChild displayItem = mDisplayItemList[i];
                if (displayItem != null)
                    displayItem.UpdateReadNum();
            }
        }
    }

    
    private void share(UnityEngine.EventSystems.PointerEventData data)
    {
        LOG.Info("这个是点击了分享的按钮");
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
        
    }
    

    private void GetShareAwardHandler(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetShareAwardHandler---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
             LoomUtil.QueueOnMainThread((param) => 
             {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.shareAward = JsonHelper.JsonToObject<HttpInfoReturn<ShareAwardInfo>>(result);
                    if(UserDataManager.Instance.shareAward != null && UserDataManager.Instance.shareAward.data != null)
                    {
                        UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.shareAward.data.bkey);
                        UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.shareAward.data.diamond);

                        AudioManager.Instance.PlayTones(AudioTones.RewardWin);
                        UserDataManager.Instance.adsRewardResultInfo = JsonHelper.JsonToObject<HttpInfoReturn<GetAdsRewardResultInfo>>(result);
                        Vector3 startPos = new Vector3(140, -300);
                        Vector3 targetPos = new Vector3(174, 720);
                        RewardShowData rewardShowData = new RewardShowData();
                        rewardShowData.StartPos = startPos;
                        rewardShowData.TargetPos = targetPos;
                        rewardShowData.IsInputPos = false;
                        rewardShowData.KeyNum = UserDataManager.Instance.shareAward.data.bkey;
                        rewardShowData.DiamondNum = UserDataManager.Instance.shareAward.data.diamond;
                        rewardShowData.TicketNum = UserDataManager.Instance.shareAward.data.ticket;
                        rewardShowData.Type = 1;
                        EventDispatcher.Dispatch(EventEnum.GetRewardShow, rewardShowData);
                    }
                }
             }, null);
        }
    }

    private void FBShareLinkFaild(bool isCancel, string errorInfo)
    {
        var Localization = GameDataMgr.Instance.table.GetLocalizationById(143);
        UITipsMgr.Instance.PopupTips(Localization, false);

        //UITipsMgr.Instance.PopupTips("Share Failed!", false);
    }

    private void back(UnityEngine.EventSystems.PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_click);
        CUIManager.Instance.CloseForm(UIFormName.BookDisplayForm);
        //EventDispatcher.Dispatch(EventEnum.BookJoinToShelfEvent);
        //刷新我的书本
        XLuaManager.Instance.GetLuaEnv().DoString(@"GameController.MainFormControl:ResetMyBookList()");
    } 
}
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
    public GameObject ItemCellPrefab;
    public ScrollViewPage SVPage;
    

    bool m_switchNext = false;

    private int mIndex;
    private int mChapterId;
    private int mCurBookId;
    private GameObject PlayShowNeedKeyGame;
    private List<ScrollViewPageCell> mCellList;
    private Stack<BookDisplayGridChild> mDisplayItemList;

    private int mLastBookId = 0;
    private int continueCost;



    int bookID;
    int openChapterCount;//开放的章节总数
    int allChapterCount;//所有的章节总数
    JDT_Book bookDetails;

    protected override void Awake()
    {
        SVPage.hasPrefabChild = true;
        //BookDisplayGridChildTpl.SetActiveEx(false);
        //ItemCellPrefab.gameObject.SetActive(false);
    }

    public override void OnOpen()
    {
        base.OnOpen();
        BookDisplayGridChildTpl.SetActiveEx(false);
        ItemCellPrefab.gameObject.SetActive(false);
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
        XLuaManager.Instance.GetLuaEnv().DoString(@"if GameController.WindowConfig.NeedShowNextWindow then
            GameController.WindowConfig.NeedShowNextWindow = false
            GameController.WindowConfig:ShowNextWindow()
        end");
    }
    public void InitByBookID(int bookID, bool switchNext = false)
    {
        mCurBookId = bookID;
        this.m_switchNext = switchNext;

        mDisplayItemList = new Stack<BookDisplayGridChild>();
        mCellList = new List<ScrollViewPageCell>();

        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.GetPropByType(new int[] { (int)PropType.Key }, GetPropByTypeCallBack_Key);
        GameHttpNet.Instance.GetPropByType(new int[] { (int)PropType.Outfit_Discount, (int)PropType.Outfit_Coupon }, GetPropByTypeCallBack_Outfit);
        GameHttpNet.Instance.GetPropByType(new int[] { (int)PropType.Choice_Discount, (int)PropType.Choice_Coupon }, GetPropByTypeCallBack_Choice);

        JDT_Version verInfo = JsonDTManager.Instance.GetJDTVersionInfo(bookID);
        if (verInfo != null)
        {
            GameHttpNet.Instance.GetBookVersionInfo(bookID,GetBookVersionInfoCallBack,verInfo.book_version,verInfo.chapter_version,verInfo.role_model_version);
        }
        else
        {
            GameHttpNet.Instance.GetBookDetailInfo(bookID, GetBookDetailInfoCallBack);  //获得书本信息
        }
    }

    private void GetBookVersionInfoCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetBookVersionInfoCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo.code == 200)
        {
            UserDataManager.Instance.bookJDTFormSever = JsonHelper.JsonToObject<HttpInfoReturn<BookJDTFormSever>>(result);
            if (UserDataManager.Instance.bookJDTFormSever != null &&
                UserDataManager.Instance.bookJDTFormSever.data != null)
            {
                BookJDTFormSever serverInfo = UserDataManager.Instance.bookJDTFormSever.data;
                
                JDT_Version verInfo = JsonDTManager.Instance.GetJDTVersionInfo(bookID);
                if (serverInfo.book_version != null)
                {
                    JsonDTManager.Instance.SaveLocalVersionBookDetail(mCurBookId,serverInfo.book_version);
                    if(serverInfo.info != null)
                        verInfo.book_version = serverInfo.info.book_version;
                }
                if (serverInfo.chapter_version != null)
                {
                    JsonDTManager.Instance.SaveLocalVersionChapter(mCurBookId,serverInfo.chapter_version);
                    if(serverInfo.info != null)
                        verInfo.chapter_version = serverInfo.info.chapter_version;
                }
                if (serverInfo.clothes_price_version != null)
                {
                    JsonDTManager.Instance.SaveLocalVersionClothesPrice(mCurBookId,serverInfo.clothes_price_version);
                    if(serverInfo.info != null)
                        verInfo.clothes_price_version = serverInfo.info.clothes_price_version;
                }
                if (serverInfo.model_price_version != null)
                {
                    JsonDTManager.Instance.SaveLocalVersionModelPrice(mCurBookId,serverInfo.model_price_version);
                    if(serverInfo.info != null)
                        verInfo.model_price_version = serverInfo.info.model_price_version;
                }
                if (serverInfo.role_model_version != null)
                {
                    JsonDTManager.Instance.SaveLocalVersionRoleModel(mCurBookId,serverInfo.role_model_version);
                    if(serverInfo.info != null)
                        verInfo.role_model_version = serverInfo.info.role_model_version;
                }
                if (serverInfo.skin_version != null)
                {
                    JsonDTManager.Instance.SaveLocalVersionSkin(mCurBookId,serverInfo.skin_version);
                    if(serverInfo.info != null)
                        verInfo.skin_version = serverInfo.info.skin_version;
                }
                
                if (verInfo != null)
                {
                    JsonDTManager.Instance.SaveLocalJDTVersionInfo(mCurBookId,verInfo);
                }
                
            }
            
            GameHttpNet.Instance.GetBookDetailInfo(mCurBookId, GetBookDetailInfoCallBack);  //获得书本信息
        }
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
    public void GetPropByTypeCallBack_Outfit(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetPropByTypeCallBack_Outfit---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);

        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    HttpInfoReturn<PropInfo> info = JsonHelper.JsonToObject<HttpInfoReturn<PropInfo>>(result);
                    UserDataManager.Instance.userPropInfo_Outfit = info.data;

                }
            }, null);
        }
    }
    public void GetPropByTypeCallBack_Choice(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetPropByTypeCallBack_Choice---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);

        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    HttpInfoReturn<PropInfo> info = JsonHelper.JsonToObject<HttpInfoReturn<PropInfo>>(result);
                    UserDataManager.Instance.userPropInfo_Choice = info.data;

                }
            }, null);
        }
    }
    public void GetPropByTypeCallBack_Key(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetPropByTypeCallBack_Key---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);

        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    HttpInfoReturn<PropInfo> info= JsonHelper.JsonToObject<HttpInfoReturn<PropInfo>>(result);
                    UserDataManager.Instance.userPropInfo_Key = info.data;

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

    BookDisplayGridChild GetPageItem()
    {
        BookDisplayGridChild item = null;
        if (mDisplayItemList.Count>0)
        {
            item = mDisplayItemList.Pop();
        }
        else
        {
            // 生成
            GameObject go = Instantiate(BookDisplayGridChildTpl, BookDisplayGridChildContent);
            item = go.GetComponent<BookDisplayGridChild>();
            item.gameObject.SetActive(false);
        }
        return item;
    }

    void initBookDisplayGridChild(int bookID, bool checkTween = false)
    {
        // 获取数据
        this.bookID = bookID;
        bookDetails = JsonDTManager.Instance.GetJDTBookDetailInfo(bookID);
        openChapterCount = bookDetails.chapteropen; //得到开放的章节
        allChapterCount = UserDataManager.Instance.bookDetailInfo.data.book_info.chapteropen;//章节总数
        if (allChapterCount == -1)
            allChapterCount = openChapterCount;
        int len = openChapterCount;

        //Debug.LogError($"lzh ===> {openChapterCount} {allChapterCount} {len} {mChapterId}");
        // 生成cell
        for (int i = 0; i < len; i++)
        {
            ScrollViewPageCell cell= null;
            if (mCellList == null || mCellList.Count - 1 < i)
            {
                GameObject go = Instantiate(ItemCellPrefab, BookDisplayGridChildContent);
                cell = go.AddComponent<ScrollViewPageCell>();
                cell.hasCell = false;
                go.SetActive(true);
                mCellList.Add(cell);
            }
            else
            {
                cell = mCellList[i];
            }

            if (cell != null)
                cell.hasCell = false;
        }

        // init scroll viwe page
        SVPage.Init(OnRefreshItemData, mChapterId - 1);

        SVPage.MoveToIndexNoTween(mChapterId - 1);
        if (checkTween && mChapterId == 1)
        {
            float posY = SVPage.transform.localPosition.y;
            SVPage.transform.localPosition = new Vector2(800, posY);
            SVPage.transform.DOLocalMoveX(0, 0.2f).SetEase(Ease.OutFlash).Play();
        }
    }
    //int curMainPageIndex, int itemCount
    void OnRefreshItemData(int curMainPageIndex, int lastMainPageIndex)
    {
        int needItemCount = 3;
        int len = openChapterCount;
        int firstIndex = 0;
        if (curMainPageIndex == 0)
        {
            firstIndex = 0;
        }
        else if(curMainPageIndex == len-1)
        {
            firstIndex = curMainPageIndex - 2;
        }
        else
        {
            firstIndex = curMainPageIndex - 1;
        }
        Debug.Log($"lzh =======> len={len} curMainPageIndex={curMainPageIndex} lastMainPageIndex={lastMainPageIndex} {firstIndex} {mDisplayItemList.Count}");
        for (int i = 0; i < len; i++)
        {
            ScrollViewPageCell cell = mCellList[i];
            if (i < firstIndex || i > firstIndex + needItemCount-1)
            {
                if (cell.hasCell)
                {
                    mDisplayItemList.Push(cell.item as BookDisplayGridChild);
                    cell.Reset();
                }
            }
        }
        for (int i = firstIndex; i < firstIndex + needItemCount; i++)
        {
            if (i >= len) break;
            BookDisplayGridChild displayItem;
            ScrollViewPageCell cell = mCellList[i];
            bool needRefresh = true;
            if(cell.hasCell && cell.item.gameObject.activeSelf)
            {
                needRefresh = false;
            }
            if (cell.hasCell)
            {
                displayItem = cell.item as BookDisplayGridChild;
            }
            else
            {
                //Debug.Log($"lzh =========> i={i} mDisplayItemList.Count = {mDisplayItemList.Count} {bookDetails.ChapterDiscriptionArray.Length}");
                displayItem = GetPageItem();
            }
            cell.MountItem(cell.transform, displayItem);

            int chapterId = i+1;
            if (needRefresh)
            {
                JDT_Chapter chapterInfo = JsonDTManager.Instance.GetJDTChapterInfo(mCurBookId, chapterId);
                if (chapterInfo != null)
                {
                    displayItem.Init(bookDetails,
                        bookID,
                        bookDetails.bookname,
                        chapterId,
                        string.Format("Chapter {0} / {1}", chapterId, allChapterCount),
                        chapterInfo.dsc,
                        i <= (mChapterId - 1),
                        i < (mChapterId - 1),
                        (i > (mChapterId - 1) && i < allChapterCount),
                        bookDetails.chapterrelease,
                        UserDataManager.Instance.bookDetailInfo.data.book_comment_count,
                        "bg_book" + bookID,
                        share,
                        startReading,
                        commingSoonButtonEvent,
                        readCompleteEvent,
                        back,
                        LockHandler,
                        ResetBookHandler,
                        ResetChapterHandler);
                }
            }
        }
    }

    //private void initBookDisplayGridChild_old(int bookID, bool checkTween = false)
    //{
    //    t_BookDetails bookDetails = JsonDTManager.Instance.GetJDTBookDetailInfo(bookID);
    //    int len = 0;
    //    if (mDisplayItemList != null)
    //        len = mDisplayItemList.Count;
    //    int chapterCount = mChapterId; //得到开放的章节
    //    int chapterOpenIndex = UserDataManager.Instance.bookDetailInfo.data.book_info.chapteropen;
    //    Debug.LogError($"lzh ===>  chapterCount:{chapterCount} chapterOpenIndex:{chapterOpenIndex} ChapterOpen:{bookDetails.ChapterOpen} ");
    //    if (chapterOpenIndex == -1)
    //        chapterOpenIndex = bookDetails.ChapterOpen;
    //    //chapterCount = chapterOpenIndex;//
    //    if (len < chapterOpenIndex)
    //        len = chapterOpenIndex;
    //    for (int i = 0; i < len; i++)
    //    {
    //        BookDisplayGridChild displayItem;
    //        int chapterId = i + 1;
    //        if (mDisplayItemList.Count > i)
    //        {
    //            displayItem = mDisplayItemList[i] as BookDisplayGridChild;
    //        }
    //        else
    //        {
    //            GameObject go = Instantiate(BookDisplayGridChildTpl, BookDisplayGridChildContent);
    //            displayItem = go.GetComponent<BookDisplayGridChild>();
    //            mDisplayItemList.Add(displayItem);
    //        }

    //        if (displayItem == null)
    //            return;
    //        displayItem.Init(bookDetails,
    //            bookID,
    //            bookDetails.BookName,
    //            chapterId,
    //            string.Format("Chapter {0} / {1}", chapterId, chapterOpenIndex),
    //            bookDetails.ChapterDiscriptionArray[i],
    //            i <= (mChapterId - 1),
    //            i < (mChapterId - 1),
    //            (i > (mChapterId - 1) && i < chapterOpenIndex),
    //            bookDetails.ChapterRelease,
    //            UserDataManager.Instance.bookDetailInfo.data.book_comment_count,
    //            "bg_book" + bookID,
    //            share,
    //            startReading,
    //            commingSoonButtonEvent,
    //            readCompleteEvent,
    //            back,
    //            LockHandler,
    //            ResetBookHandler,
    //            ResetChapterHandler);

    //        displayItem.gameObject.SetActive(true);
    //    }

    //    SVPage.MoveToIndexNoTween(mChapterId - 1);
    //    if (checkTween && mChapterId == 1)
    //    {
    //        float posY = SVPage.transform.localPosition.y;
    //        SVPage.transform.localPosition = new Vector2(800, posY);
    //        SVPage.transform.DOLocalMoveX(0, 0.2f).SetEase(Ease.OutFlash).Play();
    //    }
    //}

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
                foreach(var item in mDisplayItemList)
                {
                    (item as BookDisplayGridChild).Limit_time_Free();
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
        JDT_Book bookDetails = JsonDTManager.Instance.GetJDTBookDetailInfo(mCurBookId);
        int endDialogId = 0;
        JDT_Chapter chapterInfo = JsonDTManager.Instance.GetJDTChapterInfo(mCurBookId, 1);
        if (chapterInfo != null)
        {
            endDialogId = chapterInfo.chapterfinish;
        }

        DialogDisplaySystem.Instance.InitByBookID(
            mCurBookId,1,1,1,endDialogId
            );
        EventDispatcher.Dispatch(EventEnum.BookProgressUpdate);

        var Localization = GameDataMgr.Instance.table.GetLocalizationById(140);
        UITipsMgr.Instance.PopupTips(Localization, false);

        //UITipsMgr.Instance.PopupTips("Reset Book Successful.", false);
    }

    private int restartCost = 0;
    private void ResetChapterHandler(int vChapterId)
    {
        mChapterId = vChapterId;
        mIndex = vChapterId - 1;
       BookData bookData = UserDataManager.Instance.UserData.BookDataList.Find((bookdata) => bookdata.BookID == mCurBookId);
       if (bookData != null && bookDetails != null)
       {
           if(mIndex == 0 && (bookData.DialogueID <=1))
           {
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(141);
                UITipsMgr.Instance.PopupTips(Localization, false);
               return;
           }else if(mIndex > 0)
           {
               JDT_Chapter chapterInfo = JsonDTManager.Instance.GetJDTChapterInfo(mCurBookId, mIndex);
               int tempDialogId = chapterInfo.chapterfinish;
               if (tempDialogId == bookData.DialogueID || tempDialogId == (bookData.DialogueID + 1) || (tempDialogId + 1) == bookData.DialogueID)
               {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(141);
                    UITipsMgr.Instance.PopupTips(Localization, false);
                   return;
               }
           }
       }

       JDT_Book bookInfo = JsonDTManager.Instance.GetJDTBookDetailInfo(mCurBookId);
        
        if (bookInfo.chaptercount >= mChapterId)
        {
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.ResetChapter(mCurBookId, mChapterId, ResetChapterCallBack);
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
                else if (jo.code == 201)    //你的钥匙数量不足,无法重置
                {
                    OpenChargeTip(1, restartCost, restartCost * 0.99f, true);
                }
            }, null);
        }
    }

    private void DoRestartChapter()
    {
        JDT_Chapter nextChapterInfo = JsonDTManager.Instance.GetJDTChapterInfo(mCurBookId,mIndex+1);
        int beginDialogID = 1;
        int endDialogID = 0;
        if (nextChapterInfo != null)
        {
            beginDialogID = nextChapterInfo.chapterstart;
            endDialogID = nextChapterInfo.chapterfinish;
        }
        
        if (beginDialogID != 1)
            beginDialogID = beginDialogID + 1;  //代表下一个章节的第一个id
        
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

    private void startReading(int vChapterId)
    {
        Debug.Log($"lzh ===========> startReading({vChapterId})");
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
        JDT_Book bookDetails = JsonDTManager.Instance.GetJDTBookDetailInfo(mCurBookId);
        if (bookDetails != null)
        {
            if (bookData != null)
            {
                int saveDialogueID = bookData.DialogueID;
                bool readComplete = false;   //是否章节都阅读完成
                int chapterOpenIndex = UserDataManager.Instance.bookDetailInfo.data.book_info.chapteropen;
                if (chapterOpenIndex == -1)
                    chapterOpenIndex = bookDetails.chapteropen;
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

            if (bookDetails.chaptercount >= mChapterId)
            {
                continueCost = 0;
                JDT_Chapter chapterInfo = JsonDTManager.Instance.GetJDTChapterInfo(mCurBookId, mChapterId);
                if (chapterInfo != null)
                {
                    continueCost = chapterInfo.payamount;
                }
                
                if (UserDataManager.Instance.CheckBookHasBuy(mCurBookId))
                    continueCost = 0;


                if (continueCost > 0 && UserDataManager.Instance.bookDetailInfo.data.cost_max_chapter < mChapterId)
                {
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
            OpenChargeTip(1, continueCost, continueCost * 0.99f, true);
        }
    }
    

    private void TurnToNextChapter()
    {       
        int dialogueID = 1;
        BookData bookData = UserDataManager.Instance.UserData.BookDataList.Find((bookdata) => bookdata.BookID == mCurBookId);
        JDT_Chapter nextChapterInfo = JsonDTManager.Instance.GetJDTChapterInfo(mCurBookId,mIndex + 1);
        int endDialogID = -1;
        int beginDialogID = 1;
        if (nextChapterInfo != null)
        {
            beginDialogID = nextChapterInfo.chapterstart;
            endDialogID = nextChapterInfo.chapterfinish;
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
        GetModelAndClothesPrice();
    }

    private void GetModelAndClothesPrice()
    {
        JDT_Version verInfo = JsonDTManager.Instance.GetJDTVersionInfo(bookID);
        if (verInfo != null)
        {
            GameHttpNet.Instance.GetBookVersionInfo(bookID,GetModelAndClothesPriceCallBack,0,0,verInfo.role_model_version,
                verInfo.model_price_version,verInfo.clothes_price_version,verInfo.skin_version);
        }
    }
    
    private void GetModelAndClothesPriceCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetModelAndClothesPriceCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo.code == 200)
        {
            UserDataManager.Instance.bookJDTFormSever = JsonHelper.JsonToObject<HttpInfoReturn<BookJDTFormSever>>(result);
            if (UserDataManager.Instance.bookJDTFormSever != null &&
                UserDataManager.Instance.bookJDTFormSever.data != null)
            {
                BookJDTFormSever serverInfo = UserDataManager.Instance.bookJDTFormSever.data;
                JDT_Version verInfo = JsonDTManager.Instance.GetJDTVersionInfo(bookID);
                if (serverInfo.book_version != null)
                {
                    JsonDTManager.Instance.SaveLocalVersionBookDetail(mCurBookId,serverInfo.book_version);
                    if(serverInfo.info != null)
                        verInfo.book_version = serverInfo.info.book_version;
                }
                if (serverInfo.chapter_version != null)
                {
                    JsonDTManager.Instance.SaveLocalVersionChapter(mCurBookId,serverInfo.chapter_version);
                    if(serverInfo.info != null)
                        verInfo.chapter_version = serverInfo.info.chapter_version;
                }
                if (serverInfo.clothes_price_version != null)
                {
                    JsonDTManager.Instance.SaveLocalVersionClothesPrice(mCurBookId,serverInfo.clothes_price_version);
                    if(serverInfo.info != null)
                        verInfo.clothes_price_version = serverInfo.info.clothes_price_version;
                }
                if (serverInfo.model_price_version != null)
                {
                    JsonDTManager.Instance.SaveLocalVersionModelPrice(mCurBookId,serverInfo.model_price_version);
                    if(serverInfo.info != null)
                        verInfo.model_price_version = serverInfo.info.model_price_version;
                }
                if (serverInfo.role_model_version != null)
                {
                    JsonDTManager.Instance.SaveLocalVersionRoleModel(mCurBookId,serverInfo.role_model_version);
                    if(serverInfo.info != null)
                        verInfo.role_model_version = serverInfo.info.role_model_version;
                }
                if (serverInfo.skin_version != null)
                {
                    JsonDTManager.Instance.SaveLocalVersionSkin(mCurBookId,serverInfo.skin_version);
                    if(serverInfo.info != null)
                        verInfo.skin_version = serverInfo.info.skin_version;
                }
                
                if (verInfo != null)
                {
                    JsonDTManager.Instance.SaveLocalJDTVersionInfo(mCurBookId,verInfo);
                }
            }
        }
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
            foreach (var item in mDisplayItemList)
            {
                BookDisplayGridChild displayItem = item as BookDisplayGridChild;
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
            foreach (var item in mDisplayItemList)
            {
                BookDisplayGridChild  displayItem = item as BookDisplayGridChild;
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
        if (mDisplayItemList != null)
        {
            foreach (var item in mDisplayItemList)
            {
                  BookDisplayGridChild displayItem = item as BookDisplayGridChild;
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
        if (mDisplayItemList != null)
        {
            foreach (var item in mDisplayItemList)
            {
                BookDisplayGridChild displayItem = item as BookDisplayGridChild;
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
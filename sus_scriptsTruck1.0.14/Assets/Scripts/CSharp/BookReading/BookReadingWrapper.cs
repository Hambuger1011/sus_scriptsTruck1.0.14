using AB;
using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using XLua;
using Spine.Unity;
using pb;

public class BookReadingWrapper : CSingleton<BookReadingWrapper>
{
    public bool IsTextTween = false;
    
    public int BookID { get; private set; }
    public int ChapterID { get; private set; }
    public int BeginDialogID { get; private set; }
    public int EndDialogID { get; private set; }
    public BookData CurrentBookData { get; private set; }

    public bool IsPlaying
    {
        get
        {
            return MyBooksDisINSTANCE.Instance.GetIsPlaying();
        }
    }
    //public BaseDialogData CurrentDialogData;

    public string m_resRootPath;

    protected override void Init()
    {
        base.Init();
        EventDispatcher.AddMessageListener(UIEventMethodName.BookReadingForm_IsTweening.ToString(), BookReadingForm_IsTweening);
    }

    public void ChangeBookDialogPath(int bookID, int chapterID)
    {
    }
    private void BookReadingForm_IsTweening(Notification notification)
    {
        this.IsTextTween = (bool)notification.Data;
    }
        
    public void EnableTouchEffect()
    {
        GlobalForm.Instance.EnableTouchEffect();
    }
    
    public void DisableTouchEffect()
    {
        GlobalForm.Instance.DisableTouchEffect();
    }

    public void InitByBookID(int bookID, int chapterID, int dialogueID, int beginDialogID, int endDialogID)
    {
        this.BookID = bookID;
        this.ChapterID = chapterID;
        this.EndDialogID = endDialogID;
        this.BeginDialogID = beginDialogID;
        this.m_resRootPath = string.Concat("Assets/Bundle/Book/", bookID, "/");
        CurrentBookData = UserDataManager.Instance.UserData.BookDataList.Find((bookData) => bookData.BookID == bookID);
        if (CurrentBookData == null)
        {
            CurrentBookData = new BookData()
            {
                BookID = bookID,
                ChapterID = chapterID,
                BookName = GameDataMgr.Instance.table.GetBookDetailsById(bookID).BookName,
                DialogueID = 1,
            
                PlayerName = PlayerNameReturn(),
                PlayerDetailsID = 1,
                PlayerClothes = 1,
            };
            UserDataManager.Instance.UserData.BookDataList.Add(CurrentBookData);
        }
        else
        {
            CurrentBookData.ChapterID = chapterID;
            CurrentBookData.DialogueID = dialogueID;
            CurrentBookData.PlayerName = PlayerNameReturn();
        }
        LOG.Info("========curBookId========>" + bookID + "===chapterID===>" + chapterID +"===curDialogId===>" + dialogueID);
        //m_currentDialogData = GetBaseDialogDataByDialogueID(dialogueID);//加载对应的行的表的数据

        //mBookDetail = GameDataMgr.Instance.table.GetBookDetailsById(bookID);
    }

    private string PlayerNameReturn()
    {
        string Name = "";
        if (UserDataManager.Instance.buyChapterResultInfo!=null&& UserDataManager.Instance.buyChapterResultInfo.data.step_info.role_name!=null)
        {
            Name = UserDataManager.Instance.buyChapterResultInfo.data.step_info.role_name.ToString();
        }
        
        if (string.IsNullOrEmpty(Name))
        {
            Name = "PLAYER";
        }

        //Debug.Log("===============书本角色名字：" + Name);

        return Name;
    }

    public void PrepareReading(bool isContinue,string bookurl = null)
    {
       // CUIManager.Instance.CloseForm(UIFormName.GuideForm);
//         LOG.Info("开始阅读:bookID=" + this.BookID + ",chapterID=" + this.ChapterID);
//         this.IsTextTween = false;
//
//         var luaenv = XLuaManager.Instance.GetLuaEnv();
//         var res = luaenv.DoString(@"
// return function(isContinue, bookurl)
//     GameMain.StartReading(isContinue,url)
// end");
//         using (var func = (LuaFunction)res[0])
//         {
//             func.Action<bool,string> (isContinue, bookurl);
//         }

        //CUIManager.Instance.CloseForm(UIFormName.BookDisplayForm);
        //CUIManager.Instance.CloseForm(UIFormName.MainFormTop);
        //CUIManager.Instance.CloseForm(UIFormName.MainForm);
        ////CUIManager.Instance.CloseForm(UIFormName.BookReadingForm);
        //CUIManager.Instance.OpenForm(UIFormName.BookLoadingForm);



        ////先加cover图

        //Sprite cover = ABSystem.ui.GetUITexture(AbTag.Global, string.Concat("assets/bundle/BookPreview/banner/book_", CurrentBookData.BookID, ".png"));
        //var loadindForm = CUIManager.Instance.GetForm<BookLoadingForm>(UIFormName.BookLoadingForm);
        //loadindForm.SetCoverImage(cover);

        ////mBookFolderPaht = "Assets/Bundle/Book/" + CurrentBookData.BookID + "/";
        ////mBookCommonPath = mBookFolderPaht + GetChapterFolderId(true) + "/";

        ////var coverImagePath = mBookCommonPath + "Cover/0001.png";

        ////this.PreLoadAsset(enResType.eSprite, coverImagePath).AddCall((_) =>
        ////{
        ////    var loadindForm = CUIManager.Instance.GetForm<BookLoadingForm>(UIFormName.BookLoadingForm);
        ////    if (loadindForm != null)
        ////    {
        ////        loadindForm.SetCoverImage(_.resSprite);
        ////    }
        ////});

        ////加载配置
        //Action<string> doLoad = (url) =>
        //{
        //    DialogDisplaySystem.Instance.StartCoroutine(LoadDialogData(url, () =>
        //    {
        //        m_currentDialogData = GetBaseDialogDataByDialogueID(CurrentBookData.DialogueID);//加载对应的行的表的数据
        //                                                                                          //加载其它资源
        //        DialogDisplaySystem.Instance.PreLoadRes((bSuc) =>
        //        {
        //            if (bSuc)
        //            {
        //                CUIManager.Instance.OpenForm(UIFormName.BookReadingForm);
        //                var uiBookReadingForm = CUIManager.Instance.GetForm<BookReadingForm>(UIFormName.BookReadingForm);
        //                uiBookReadingForm.PrepareReading();
        //            }
        //        });
        //    }));
        //};
        //if (string.IsNullOrEmpty(bookurl))
        //{
        //GameHttpNet.Instance.BuyChapter(CurrentBookData.BookID, CurrentBookData.ChapterID, (arg) =>
        //{
        //    string result = arg.ToString();
        //    var jsonData = LitJson.JsonMapper.ToObject(result);
        //    var url = jsonData["data"]["bookurl"];
        //    bookurl = (url != null ? url.ToString() : "");
        //    doLoad(bookurl);
        //});
        //}
        //else
        //{
        //    doLoad(bookurl);
        //}
    }


    public void DoTurnDialog(int vDialogId,bool isBack = false)
    {
#if ENABLE_DEBUG || UNITY_EDITOR
        var luaenv = XLuaManager.Instance.GetLuaEnv();
        //var obj = luaenv.Global.Get<LuaTable>("logic");
        //obj = obj.Get<LuaTable>("bookReadingMgr");
        //LOG.Error(obj == null);
        //var func = obj.Get<Action<object, object>>("PlayById");
        //LOG.Error(func == null);
        //func(obj,vDialogId);

        string luaStr = @"
return function(id)
    logic.bookReadingMgr:GotoDialogID(id)
end";

        if(isBack)
        {
            luaStr = @"
return function(id)
    logic.bookReadingMgr:DoGoBackStep(id)
end";
        }

        var res = luaenv.DoString(luaStr);
        var func = (LuaFunction)res[0];
        func.Action<int>(vDialogId);
        func.Dispose();
#endif
    }

#if ENABLE_DEBUG
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            //DoTurnDialog(JumpToIndex);
        }
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            UserDataManager.Instance.ClearCache();
        }

        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            StartAutoTest();
        }
    }
#endif

    #region AutoTestStory

    public void StartAutoTest()
    {
#if ENABLE_DEBUG
        // GameDataMgr.Instance.InAutoPlay = !GameDataMgr.Instance.InAutoPlay;
#endif
    }
    #endregion


    

    //private void UpdateEndDialogId()
    //{
    //    int curChapterId = m_currentDialogData.chapterID;
    //    if (EndDialogID > 0 && curChapterId > 0 && mBookDetail != null
    //        && mBookDetail.ChapterDivisionArray != null &&
    //        mBookDetail.ChapterDivisionArray.Length > curChapterId - 1 &&
    //        mBookDetail.ChapterDivisionArray[curChapterId - 1] != EndDialogID)
    //    {
    //        EndDialogID = mBookDetail.ChapterDivisionArray[curChapterId - 1];
    //    }
    //}

    public void AddChapterzPayId(int vBookId, int vChapterId)
    {
        if (this.CurrentBookData != null && this.CurrentBookData.BookID == vBookId)
        {
            int index = this.CurrentBookData.ChapterPayList.IndexOf(vChapterId);
            if (index == -1)
            {
                this.CurrentBookData.ChapterPayList.Add(vChapterId);
            }
        }
    }


    public bool CheckHasPayChapter(int vBookId, int vChapterId)
    {
        if (CurrentBookData != null && CurrentBookData.BookID == vBookId)
        {
            if (CurrentBookData.ChapterPayList.IndexOf(vChapterId) != -1)
                return true;
        }

        if (UserDataManager.Instance.bookCostChapterList != null && UserDataManager.Instance.bookCostChapterList.data != null)
        {
            List<BookCostChapterItemInfo> costList = UserDataManager.Instance.bookCostChapterList.data.costarr;
            if (costList != null)
            {
                int len = costList.Count;
                for (int i = 0; i < len; i++)
                {
                    BookCostChapterItemInfo itemInfo = costList[i];
                    if (itemInfo != null && itemInfo.bookid == vBookId && itemInfo.chapterid == vChapterId)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }



    /// <summary>
    /// 重置书本或者重置章节时，清理一些额外的章节
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vChapterId"></param>
    public void UpdatePayChapterRecordByReset(int vBookId, int vChapterId)
    {

        if (CurrentBookData != null && CurrentBookData.BookID == vBookId)
        {
            List<int> payList = CurrentBookData.ChapterPayList;
            int len = payList.Count;
            for (int i = len - 1; i >= 0; i--)
            {
                if (CurrentBookData.ChapterPayList[i] > vChapterId)
                    CurrentBookData.ChapterPayList.RemoveAt(i);
            }
        }

        if (UserDataManager.Instance.bookCostChapterList != null && UserDataManager.Instance.bookCostChapterList.data != null)
        {
            List<BookCostChapterItemInfo> costList = UserDataManager.Instance.bookCostChapterList.data.costarr;
            if (costList != null)
            {
                int len = costList.Count;
                for (int i = len - 1; i >= 0; i--)
                {
                    BookCostChapterItemInfo itemInfo = costList[i];
                    if (itemInfo != null && itemInfo.bookid == vBookId && itemInfo.chapterid > vChapterId)
                    {
                        costList.RemoveAt(i);
                    }
                }
            }
        }
    }


    public LuaFunction getUITextureFunc;
    public Sprite GetUITexture(string strSpriteName, bool isCommon = true)
    {
        if(getUITextureFunc == null)
        {
            var luaenv = XLuaManager.Instance.GetLuaEnv();
            var res = luaenv.DoString(@"
return function(name, isCommon)
    return logic.bookReadingMgr.Res:GetSprite(name, isCommon)
end");
            getUITextureFunc = (LuaFunction)res[0];
        }

        return getUITextureFunc.Func<string, bool, Sprite>(strSpriteName, isCommon);
    }

    public LuaFunction getSpineFunc;
    public SkeletonDataAsset GetSkeDataAsset(string skeDataPath)
    {
        //SkeletonDataAsset skeData = ABSystem.ui.GetObject(AbTag.DialogDisplay, m_resRootPath + skeDataPath + "_skeletondata.asset") as SkeletonDataAsset;
        //return skeData;

        if (getSpineFunc == null)
        {
            var luaenv = XLuaManager.Instance.GetLuaEnv();
            var res = luaenv.DoString(@"
return function(key)
    return logic.bookReadingMgr.Res:GetSkeDataAsset(key)
end");
            getSpineFunc = (LuaFunction)res[0];
        }

        return (SkeletonDataAsset)getSpineFunc.Func<string, UnityEngine.Object>(skeDataPath);
    }



    public void ClearBookResources()
    {
        var luaenv = XLuaManager.Instance.GetLuaEnv();
        luaenv.DoString("logic.bookReadingMgr:ClearBookResources()");
    }


    public string GetRoleName(int role_id, int curSelectBookID = 0)
    {
        var luaenv = XLuaManager.Instance.GetLuaEnv();
        var res = luaenv.DoString("return logic.bookReadingMgr:GetRoleName("+ role_id + ")");
        return (string)res[0];
    }


    //List<t_BookDialog> m_bookDialogList = null;
    Dictionary<int, t_BookDialog> m_bookDialogMap = new Dictionary<int, t_BookDialog>();
    public t_BookDialog GetDialogById(int vId)
    {
        //InitDialogData();
        t_BookDialog cfg;
        if (m_bookDialogMap.TryGetValue(vId, out cfg))
        {
            return cfg;
        }
        return null;
    }

    public void LoadBookConfig(byte[] bytes)
    {
        var m_bookDialogList = XlsxData.Deserialize<List<t_BookDialog>>(bytes);
        foreach (var item in m_bookDialogList)
        {
            if (!m_bookDialogMap.ContainsKey(item.dialogID))
                m_bookDialogMap.Add(item.dialogID, item);
            else
                LOG.Error("---BookID-->" + this.CurrentBookData.BookID + "--DialogIdRepetition-->" + item.dialogID);
        }
       // LOG.Error("book data:" + m_bookDialogList.Count);
    }

    public void Reset()
    {
        m_bookDialogMap.Clear();
    }

    public void CloseForm()
    {
        var luaenv = XLuaManager.Instance.GetLuaEnv();
        luaenv.DoString("logic.UIMgr:Close(logic.uiid.BookReading)");
    }
}

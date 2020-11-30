#if NOT_USE_LUA
using AB;
using GameLogic;
using pb;
using System;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.Networking;
using Spine.Unity;

public class DialogDisplaySystem : SingletonMono<DialogDisplaySystem> {

#if ENABLE_DEBUG
#if !UNITY_EDITOR
    [System.NonSerialized]
#endif
    public int curDialogID;

#if !UNITY_EDITOR
    [System.NonSerialized]
#endif
    public int JumpToIndex;        //For Test

#if !UNITY_EDITOR
    [System.NonSerialized]
#endif
    public int AutoSelIndex = 0;        

#if !UNITY_EDITOR
    [System.NonSerialized]
#endif
    public float AutoTestSpeed = 0.6f;        //For Test

#endif

    public BaseDialogData CurrentDialogData { get { return m_currentDialogData; } }
    public BookData CurrentBookData { get { return m_currentBookData; } }
    public DialogType LastDialogType { get { return m_eLastDialogType; } set { m_eLastDialogType = value; } }
    public int LastRoleID { get { return m_iLastRoleID; } }

    [HideInInspector]
    public bool IsTextTween = false;

    private BaseDialogData m_currentDialogData;
    private BookData m_currentBookData;
    private DialogType m_eLastDialogType = DialogType.Narration;
    private int m_iLastRoleID = -1;
    private t_BookDetails mBookDetail;

    public int BeginDialogID { get; private set; }
    public int EndDialogID { get; private set; }

    public int readingBookID { get; private set; }




#region t_BookDialog
    List<t_BookDialog> m_bookDialogList = null;
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

    IEnumerator LoadDialogData(string bookUrl, Action callback)
    {
        LOG.Error("bookUrl:" + bookUrl);
        if (this.m_lastBookID == this.m_readBookID 
            && this.m_lastChapterID == this.m_readChapterID
           )
        {
            callback();
            yield break;
        }


        Action<byte[]> doInitBook = (bytes) =>
        {
            m_bookDialogMap.Clear();
            m_bookDialogList = XlsxData.Deserialize<List<t_BookDialog>>(bytes);
            foreach (var item in m_bookDialogList)
            {
                if (!m_bookDialogMap.ContainsKey(item.dialogID))
                    m_bookDialogMap.Add(item.dialogID, item);
                else
                    LOG.Error("---BookID-->" + this.CurrentBookData.BookID + "--DialogIdRepetition-->" + item.dialogID);
            }
            LOG.Error("book data:" + m_bookDialogList.Count);
            callback();
        };
        if(string.IsNullOrEmpty(bookUrl) || !ABSystem.Instance.isUseAssetBundle)//url为空或debug模式，直接加载本地
        {
            int bookID = this.m_readBookID;
            int chapterID = this.m_readChapterID;
            var filename = string.Concat(XlsxData.BOOK_DIR, bookID, "/t_BookDialog_", bookID, "_", chapterID, ".bytes");
            this.resPreLoader.PreLoad(enResType.eText, filename,(_) =>
            {
                if(_ != null)
                {
                    doInitBook(_.resTextAsset.bytes);
                }
                else
                {
                    LOG.Error("加载配置错误:" + filename);
                }
            });
        }
        else
        {
            using (var req = UnityWebRequest.Get(bookUrl))
            {
                yield return req.SendWebRequest();
                if (req.isNetworkError || req.isHttpError || !string.IsNullOrEmpty(req.error))
                {
                    LOG.Error("加载配置失败:" + req.error);
                    yield break;
                }
                var buff = req.downloadHandler.data;
                doInitBook(buff);
            }
        }
    }
    public List<t_BookDialog> GetDialogList()
    {
        return m_bookDialogList;
    }

    //当前书本
    int m_readBookID = 0;
    int m_readChapterID = 0;

    //上次书本
    int m_lastBookID = 0;
    int m_lastChapterID = 0;
    public void ChangeBookDialogPath(int bookID, int chapterID)
    {
        if (chapterID == 0)
        {
            return;
        }
        m_readBookID = bookID;
        m_readChapterID = chapterID;
    }
#endregion


    public void InitByBookID(int bookID, int chapterID,int dialogueID,int beginDialogID, int endDialogID)
    {
        this.readingBookID = bookID;
        this.EndDialogID = endDialogID;
        this.BeginDialogID = beginDialogID;
        this.m_resRootPath = string.Concat("assets/bundle/book/", bookID, "/");
        m_currentBookData = UserDataManager.Instance.UserData.BookDataList.Find((bookData) => bookData.BookID == bookID);
        if (m_currentBookData == null)
        {
            m_currentBookData = new BookData()
            {
                BookID = bookID,
                ChapterID = chapterID,
                BookName = GameDataMgr.Instance.table.GetBookDetailsById(bookID).BookName,
                DialogueID = 1,
                PlayerName = "PLAYER",
                PlayerDetailsID = 1,
                PlayerClothes = 1,
            };
            UserDataManager.Instance.UserData.BookDataList.Add(m_currentBookData);
        }else
        {
            m_currentBookData.ChapterID = chapterID;
            m_currentBookData.DialogueID = dialogueID;
        }
        LOG.Info("========curBookId========>" + bookID + "===curDialogId===>" + dialogueID);
        m_currentDialogData = GetBaseDialogDataByDialogueID(dialogueID);//加载对应的行的表的数据

        mBookDetail = GameDataMgr.Instance.table.GetBookDetailsById(bookID);
    }



    public void ShowNextDialog(int id,bool forceShow = false)
    {
#if ENABLE_DEBUG
        curDialogID = id;
#endif
        if (m_currentDialogData.next != -1)
            m_currentDialogData = GetBaseDialogDataByDialogueID(id);
        if (m_currentDialogData != null)
        {
            m_currentDialogData.ShowDialog();
        }
        else
        {
            LOG.Error("Next is Null!!!");
        }
    }

    private BaseDialogData GetBaseDialogDataByDialogueID(int dialogueID)
    {
        if (dialogueID == -1) return null;
        if (m_currentDialogData != null)
        {
            m_eLastDialogType = m_currentDialogData.dialog_type;
            m_iLastRoleID = m_currentDialogData.role_id;
        }

        //开始读表
        t_BookDialog data = this.GetDialogById(dialogueID);
        if(data == null)
        {
            LOG.Error("对话结束:" + dialogueID);
            return null;
        }
        DialogType type = (DialogType)data.dialog_type;
        switch (type)
        {
            case DialogType.Negative:
                return new DialogDataNegative(data);
            case DialogType.Narration:
                return new DialogDataNarration(data);
            case DialogType.EnterName:
                return new DialogDataEnterName(data);
            case DialogType.EnterNPCname:
                return new DialogDataEnterNpcName(data);
            case DialogType.PlayerDialogue:
                return new DialogDataPlayerDialogue(data);
            case DialogType.OtherDialogue:
                return new DialogDataOtherDialogue(data);
            case DialogType.ChangeClothes:
            case DialogType.AutoSelectClothes:
                return new DialogDataChangeClothes(data);
            case DialogType.ChangeSceneByBlack:
            case DialogType.ChangeSceneToBlack:
            case DialogType.ChangeSceneByWhite:
            case DialogType.ChangeSceneByWave:
            case DialogType.ChangeSceneByShutter:
                return new DialogDataChangeScene(data);
            case DialogType.PlayerImagineDialogue:
                return new DialogDataPlayerImagineDialogue(data);
            case DialogType.OtherImagineDialogue:
                return new DialogDataOtherImagineDialogue(data);
            case DialogType.PhoneCallDialogue:
            case DialogType.BubbleChat:
            case DialogType.BubbleNarration:
            case DialogType.BubblePlayerDialog:
            case DialogType.BubbleOtherPlayerDialog:
                return new DialogDataPhoneCallDialogue(data);
            case DialogType.SceneInteraction:
                return new DialogDataSceneInteraction(data);
            case DialogType.ManualChangeScene:
                return new DialogDataManualChangeScene(data);
            case DialogType.SceneTap:
                return new DialogDataSceneTap(data);
            case DialogType.Puzzle:
                return new DialogDataPuzzle(data);
            case DialogType.ClockChangeScene:
                return new DialogDataClockChangeScene(data);
            case DialogType.ChangeWholeClothes:
                return new DialogChangeWholeClothes(data);
            case DialogType.ChoiceRole:
                return new DialogDataChoiceRole(data);
            case DialogType.StoryItems:
                return new DialogDataStoryItems(data);
            default:
                break;
        }
        return null;
    }

    protected override void Init()
    {
        base.Init();
        EventDispatcher.AddMessageListener(EventEnum.DialogDisplaySystem_PlayerMakeChoice, DialogDisplaySystem_PlayerMakeChoice);
    }

    protected override void UnInit()
    {
        base.UnInit();
        EventDispatcher.RemoveMessageListener(EventEnum.DialogDisplaySystem_PlayerMakeChoice, DialogDisplaySystem_PlayerMakeChoice);
    }

    private void DialogDisplaySystem_PlayerMakeChoice(Notification notification)
    {
        if (EndDialogID > 0 && m_currentDialogData.dialogID == EndDialogID)
        {
            //LOG.Error("开始章节跳转");
            if (!UserDataManager.Instance.IsPayUser())
            {
                SdkMgr.Instance.ShowAds(LookVideoComplete);
            }
            var view = CUIManager.Instance.GetForm<BookReadingForm>(UIFormName.BookReadingForm);
            view.chapterSwitch.SetData(m_currentDialogData.dialogID, m_currentBookData.BookID, m_currentDialogData.chapterID);
            view.chapterSwitch.Show();
            TalkingDataManager.Instance.onCompleted("ReadChapterComplete_"+CurrentBookData.BookID+"_"+ m_currentBookData.ChapterID);
            return;
        }

        switch (m_currentDialogData.dialog_type)
        {
            case DialogType.Negative:
            case DialogType.Narration:
                ShowNextDialog(GetNextDialogId());
                break;
            case DialogType.EnterName:
            case DialogType.EnterNPCname:
                ShowNextDialog(GetNextDialogId());
                break;
            case DialogType.PlayerDialogue:
            case DialogType.OtherDialogue:
            case DialogType.PlayerImagineDialogue:
            case DialogType.OtherImagineDialogue:
            case DialogType.BubblePlayerDialog:
            case DialogType.BubbleOtherPlayerDialog:
                if (m_currentDialogData.trigger == 0 || IsEmojiTrigger())
                {
                    ShowNextDialog(GetNextDialogId());
                }
                else
                {
                    if(notification == null)
                    {
                        LOG.Error("什么鬼,null!!!");
                        return;
                    }
                    int mIndex = (int)notification.Data;
                    ShowNextDialog(getNextDialogIDBySelection(mIndex, m_currentDialogData));
                }
                break;
            case DialogType.ChangeClothes:
            case DialogType.AutoSelectClothes:
                if (notification == null)
                {
                    //如果这里返回的是null，那么说明自动选择服装，服务端没有通过,需要再次发送自动选择衣服
                    ShowNextDialog(m_currentDialogData.dialogID);
                    return;
                }
                ShowNextDialog(getNextDialogIDBySelection((int)notification.Data, m_currentDialogData));
                break;
            case DialogType.ChangeWholeClothes:
                if (m_currentDialogData.trigger == 1 || m_currentDialogData.trigger == 3)   //选主角，或者选npc的形象
                {
                    ShowNextDialog(GetNextDialogId());
                }
                else
                {
                    if (notification == null)
                    {
                        LOG.Error("什么鬼,null!!!");
                        return;
                    }
                    ShowNextDialog(getNextDialogIDBySelection((int)notification.Data, m_currentDialogData));
                }
                break;
            case DialogType.BubbleNarration:
            case DialogType.ChangeSceneByBlack:
            case DialogType.ChangeSceneToBlack:
            case DialogType.ChangeSceneByWhite:
            case DialogType.ChangeSceneByWave:
            case DialogType.ChangeSceneByShutter:
                ShowNextDialog(GetNextDialogId());
                break;
            case DialogType.SceneTap:
                if (notification == null)
                {
                    LOG.Error("什么鬼,null!!!");
                    return;
                }
                else
                {
                    int mIndex = (int)notification.Data;
                    ShowNextDialog(getNextDialogIDBySelection(mIndex, m_currentDialogData));
                }
                    
                break;
            case DialogType.PhoneCallDialogue:
            case DialogType.BubbleChat:
            case DialogType.SceneInteraction:
            case DialogType.ManualChangeScene:
            case DialogType.Puzzle:
            case DialogType.ClockChangeScene:
            case DialogType.ChoiceRole:
            case DialogType.StoryItems:
                ShowNextDialog(GetNextDialogId());
                break;
        }
        UpdateEndDialogId();
    }

    //获取跳转到对应的对话id
    private int GetNextDialogId()
    {
        int result = m_currentDialogData.next;
        if(m_currentDialogData.ConsequenceID > 0)
        {
            int recordSelIndex = UserDataManager.Instance.GetBookOptionSelectIndex(m_currentBookData.BookID, m_currentDialogData.ConsequenceID);
            if(recordSelIndex > 0)
            {
                result = getNextDialogIDBySelection(recordSelIndex - 1, m_currentDialogData);
            }
        }else if(m_currentDialogData.propertyCheck > 0 && m_currentDialogData.selection_num > 0)
        {
            for(int n = 0;n<m_currentDialogData.selection_num;n++)
            {
                string personalistStr = m_currentDialogData.GetPersonalist(n);
                if (!string.IsNullOrEmpty(personalistStr))
                {
                    personalistStr = personalistStr.Replace("##", "_");
                    string[] infoList = personalistStr.Split('_');
                    int len = infoList.Length;
                    bool conditionIsConform = false;
                    for (int i = 0; i < len; i++)
                    {
                        if (!string.IsNullOrEmpty(infoList[i]))
                        {
                            conditionIsConform = true;
                            string[] keyValue = infoList[i].Split(':');
                            if (keyValue != null && keyValue.Length >= 2)
                            {
                                int key = int.Parse(keyValue[0]);
                                int value = int.Parse(keyValue[1]);
                                if(UserDataManager.Instance.GetPropertyValueByType(m_currentBookData.BookID, key)<value)
                                {
                                    conditionIsConform = false; //条件不符合时，退出循环
                                    break;
                                }
                            }
                        }
                    }

                    if(conditionIsConform)
                    {
                        result = getNextDialogIDBySelection(n,m_currentDialogData);
                        break;
                    }
                }
            }
        }
        return result;
    }

    private int GetNextDialogIdByProperty()
    {
        int result = m_currentDialogData.next;
        if(m_currentDialogData.selection_num > 0)
        {

        }
        return result;
    }

    private void LookVideoComplete(bool value)
    {
        if (value)
            GameHttpNet.Instance.GetAdsReward(2, GetAdsRewardCallBack, m_currentBookData.BookID);
    }

    private void GetAdsRewardCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetAdsRewardCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    AudioManager.Instance.PlayTones(AudioTones.RewardWin);
                    UserDataManager.Instance.adsRewardResultInfo = JsonHelper.JsonToObject<HttpInfoReturn<GetAdsRewardResultInfo>>(result);
                    Vector3 startPos = new Vector3(-188, -355);
                    Vector3 targetPos = new Vector3(306, 625);
                    RewardShowData rewardShowData = new RewardShowData();
                    rewardShowData.StartPos = startPos;
                    rewardShowData.TargetPos = targetPos;
                    rewardShowData.IsInputPos = false;
                    rewardShowData.Type = 1;
                    if (UserDataManager.Instance.adsRewardResultInfo != null && UserDataManager.Instance.adsRewardResultInfo.data != null)
                    {
                        rewardShowData.KeyNum = UserDataManager.Instance.adsRewardResultInfo.data.bkey;
                        rewardShowData.DiamondNum = UserDataManager.Instance.adsRewardResultInfo.data.diamond;
                        EventDispatcher.Dispatch(EventEnum.HiddenEggRewardShow, rewardShowData);    //观看视频，但是也是触发彩蛋的特效
                        TalkingDataManager.Instance.WatchTheAds(4);
                    }
                }
                else if (jo.code == 202 || jo.code == 203 || jo.code == 204)
                {
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                    UITipsMgr.Instance.PopupTips("You've reached the limit for today!", false);
                }
                else if (jo.code == 206)
                {
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                    UITipsMgr.Instance.PopupTips("Reward already collected before.", false);
                }
                else if (jo.code == 207 || jo.code == 205)
                {
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                    UITipsMgr.Instance.PopupTips("Chapter not completed, you can't collect the rewards.", false);
                }
                else if (jo.code == 208)
                {
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                }
            }, null);
        }
    }

    //是否是表情的trigger
    private bool IsEmojiTrigger()
    {
        if (m_currentDialogData != null)
        {
            return (m_currentDialogData.trigger == 999 || m_currentDialogData.trigger == 998 || m_currentDialogData.trigger == 997 || m_currentDialogData.trigger == 996 || m_currentDialogData.trigger == 995);
        }
        return false;
    }

    private void UpdateEndDialogId()
    {
        int curChapterId = m_currentDialogData.chapterID;
        if (EndDialogID > 0 && curChapterId > 0 && mBookDetail != null
            && mBookDetail.ChapterDivisionArray != null &&
            mBookDetail.ChapterDivisionArray.Length > curChapterId - 1 &&
            mBookDetail.ChapterDivisionArray[curChapterId - 1] != EndDialogID)
        {
            EndDialogID = mBookDetail.ChapterDivisionArray[curChapterId - 1];
        }
    }

    private void StartReadChapterCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----StartReadChapterCallBack---->" + result);

    }

    private int getNextDialogIDBySelection(int index, BaseDialogData baseDialogData)
    {
        return baseDialogData.GetSelectionsNext()[index];
    }

#if ENABLE_DEBUG || UNITY_EDITOR
    public void DoTurnDialog(int vDialogId)
    {
        if(vDialogId > EndDialogID)
        {
            vDialogId = EndDialogID;
        }
        m_currentDialogData = GetBaseDialogDataByDialogueID(vDialogId);
        t_BookDetails bookDetails = GameDataMgr.Instance.table.GetBookDetailsById(UserDataManager.Instance.UserData.CurSelectBookID);
        int[] chapterDivisionArray = bookDetails.ChapterDivisionArray;
        int endDialogID = -1;
        int beginDialogID = m_currentDialogData.chapterID - 1 < 0 ? 1 : chapterDivisionArray[m_currentDialogData.chapterID - 1];
        if (m_currentDialogData.chapterID < chapterDivisionArray.Length)
        {
            endDialogID = chapterDivisionArray[m_currentDialogData.chapterID];
        }
        m_currentDialogData.ShowDialog();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            DoTurnDialog(JumpToIndex);
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

#region AutoTestStory

    private void StartAutoTest()
    {
        // GameDataMgr.Instance.InAutoPlay = !GameDataMgr.Instance.InAutoPlay;
    }
#endregion

#endif

#region 预加载资源
    public ResPreLoader resPreLoader = new ResPreLoader();
    Action<bool> onPreLoadCallback;
    //List<AbTask> m_preLoadData = new List<AbTask>();
    //List<AbTask> m_bookAssetData = new List<AbTask>();
    Dictionary<string, bool> mChapterResDic = new Dictionary<string, bool>();
    //Dictionary<string, AbTask> mLastChapterResDic = new Dictionary<string, AbTask>();

    //List<AbTask> m_needLoadAssetData = new List<AbTask>();
    AbBookRes curBookRes;


    string mBookFolderPaht = "";    // 书本前缀路径
    string mBookCommonPath = "";    //书本公共资源的路径
    bool mCollectLoaded = false;

    

    public void PreLoadRes(Action<bool> callback)
    {
        onPreLoadCallback = callback;
        //bool theSameBook = true;
        //recordDic.Clear();

        mBookFolderPaht = "Assets/Bundle/Book/" + m_currentBookData.BookID + "/";
        mBookCommonPath = mBookFolderPaht + GetChapterFolderId(true) + "/";

		Action doSetCoverImage = () => {

			var coverImagePath = mBookCommonPath + "Cover/0001.png";

			this.resPreLoader.PreLoad (enResType.eSprite, coverImagePath, (_) => {
                if(_.abConfigItem != null)
                {
                    string result = _.abConfigItem.abFilePath + "#" + _.abConfigItem.crc;
                    UserDataManager.Instance.RecordBoookCover(m_currentBookData.BookID, result);
                }
				var loadindForm = CUIManager.Instance.GetForm<BookLoadingForm> (UIFormName.BookLoadingForm);
				if (loadindForm != null) {
					loadindForm.SetCoverImage (_.resSprite);
				}
			});
		};

        CheckNeedLoadRes();
        if (ReleasePreLoadData())
        {

			doSetCoverImage ();
			
            //m_needLoadAssetData.Clear();
            //theSameBook = false;
            //m_bookAssetData = new List<AbTask>();

            var bookResPfb = string.Concat("assets/bundle/book/", m_currentBookData.BookID,".prefab");
            
			mCollectLoaded = false;
            this.resPreLoader.PreLoad(enResType.ePrefab, bookResPfb, (_) =>
            {
                mCollectLoaded = true;
                this.resPreLoader.PreLoad(enResType.ePrefab, UIFormName.BookReadingForm);//加载ui
                var resList = _.Get<AbBookRes>();
                curBookRes = resList;

                foreach (var res in curBookRes.objs)
                {
                    //LOG.Info("------res--->" + res);
                    //加载公用资源
                    if (res.IndexOf(mBookCommonPath) != -1)
                    {
                        this.resPreLoader.PreLoad(enResType.eObject, res);
                    }
                    else
                    {
                        //加载当前章节需要的内容
                        int dotIndex = res.LastIndexOf(".") + 1;
                        string noSuffixPath = res.Substring(0, dotIndex);
                        if (mChapterResDic.ContainsKey(noSuffixPath))
                        {
                            this.resPreLoader.PreLoad(enResType.eObject, res);
                        }
                    }
                }
            });
        }
        else
        {
			if (ReleasePreChapterData ()) {
				
				doSetCoverImage ();

				this.m_lastChapterID = m_currentBookData.ChapterID;
				//m_needLoadAssetData.Clear();
				foreach (var res in curBookRes.objs) {
					//LOG.Info("------res--->" + res);
					//加载公用资源
					if (res.IndexOf (mBookCommonPath) != -1) {
						this.resPreLoader.PreLoad (enResType.eObject, res);
					} else {
						//加载当前章节需要的内容
						int dotIndex = res.LastIndexOf (".") + 1;
						string noSuffixPath = res.Substring (0, dotIndex);
						if (mChapterResDic.ContainsKey (noSuffixPath))
							this.resPreLoader.PreLoad (enResType.eObject, res);
					}
				}
			} else {
				doSetCoverImage ();
			}
		}
        this.resPreLoader.BeginPreLoad();
    }

    /// <summary>
    /// 检查章节需要加载的内容
    /// </summary>
    private void CheckNeedLoadRes()
    {
        List<t_BookDialog> dialogList = this.GetDialogList();
        List<int> bgmList = new List<int>();        //背景音乐
        List<int> sceneBgList = new List<int>();    //场景
        List<int> particleList = new List<int>();   //特效
        List<int> roleList = new List<int>();       //角色
        List<int> npcList = new List<int>();        //npc头像
        List<int> clothsList = new List<int>();     //服装
        List<int> roleHeadList = new List<int>();   //玩家头像
        List<int> goodItemsList = new List<int>();  //故事里面获得的物品
        mChapterResDic.Clear();
        bool addNewPlayerCloths = false;
        bool hasPhoneCall = false;

#region 收集要加载的内容
        int len = dialogList.Count;
        t_BookDialog dialogItem;
        for(int i = 0 ;i<len;i++)
        {
            dialogItem = dialogList[i];
            if(dialogItem != null && m_currentBookData.ChapterID == dialogItem.chapterID)
            {
                CheckIsInList(ref bgmList, dialogItem.BGMID);
                CheckIsInList(ref sceneBgList, dialogItem.sceneID,false);

                int parLen = dialogItem.SceneParticalsArray.Length;
                for (int j = 0; j < parLen; j++ )
                {
                    int parItemId = dialogItem.SceneParticalsArray[j];
                    CheckIsInList(ref particleList, parItemId);
                }

                if (dialogItem.dialog_type == (int)DialogType.StoryItems)
                {
                    CheckIsInList(ref goodItemsList, int.Parse(dialogItem.selection_1));
                }

                if (dialogItem.dialog_type == (int)DialogType.PhoneCallDialogue && !hasPhoneCall)
                    hasPhoneCall = true;

                if ((dialogItem.dialog_type == (int)DialogType.EnterName && dialogItem.trigger == 2)
                    || (dialogItem.dialog_type == (int)DialogType.ChangeWholeClothes && dialogItem.trigger == 1))  //新手选角色时，要统计角色
                {
                    int selNum = dialogItem.selection_num;
                    if (selNum > 0)
                        CheckIsInList(ref roleList, int.Parse(dialogItem.selection_1));
                    if (selNum > 1)
                        CheckIsInList(ref roleList, int.Parse(dialogItem.selection_2));
                    if (selNum > 2)
                        CheckIsInList(ref roleList, int.Parse(dialogItem.selection_3));
                    if (selNum > 3)
                        CheckIsInList(ref roleList, int.Parse(dialogItem.selection_4));
                }

                int detailId = 0;
                if (dialogItem.dialog_type == (int)DialogType.ChangeClothes || (dialogItem.dialog_type == (int)DialogType.ChangeWholeClothes && dialogItem.trigger == 2))
                {
                    int roleLen = roleList.Count;
                    if(roleLen > 0)
                    {
                        for(int j = 0;j<roleLen;j++)
                        {
                            int tempRoleId = roleList[j];
                            if(!addNewPlayerCloths) //添加选角色是的新手服装
                            {
                                detailId =  GetRoleAppearanceId(tempRoleId, 0);
                                AddClothsAndRoleHead(ref clothsList,ref roleHeadList, detailId);
                            }
                            //统计选项
                            List<int> mSelectList = GetSelectionsList(dialogItem);
                            if(mSelectList != null && mSelectList.Count > 0)
                            {
                                int selNum = mSelectList.Count;
                                for (int n = 0; n < selNum;n++ )
                                {
                                    if (mSelectList[n] > 0)
                                    {
                                        detailId = GetRoleAppearanceId(tempRoleId, mSelectList[n]);
                                        AddClothsAndRoleHead(ref clothsList, ref roleHeadList, detailId);
                                    }
                                }   
                            }
                        }
                        addNewPlayerCloths = true;
                    }
                    else if (DialogDisplaySystem.Instance.CurrentBookData.PlayerDetailsID > 0)
                    {
                        int tempRoleId = 1;
                        //统计选项
                        List<int> mSelectList = GetSelectionsList(dialogItem);
                        if (mSelectList != null && mSelectList.Count > 0)
                        {
                            int selNum = mSelectList.Count;
                            for (int n = 0; n < selNum; n++)
                            {
                                if (mSelectList[n] > 0)
                                {
                                    detailId = GetRoleAppearanceId(tempRoleId, mSelectList[n]);
                                    AddClothsAndRoleHead(ref clothsList, ref roleHeadList, detailId);
                                }
                            }
                        }
                    }
                }

                if( dialogItem.role_id != 0)
                {
                    if(dialogItem.role_id == 1)
                    {
                        if (DialogDisplaySystem.Instance.CurrentBookData.PlayerClothes > 0)
                        {
                            detailId = GetRoleAppearanceId(1, DialogDisplaySystem.Instance.CurrentBookData.PlayerClothes);
                            AddClothsAndRoleHead(ref clothsList, ref roleHeadList, detailId);

                            if (dialogItem.icon == 0)
                                AddClothsAndRoleHead(ref clothsList, ref roleHeadList, GetRoleAppearanceId(1, dialogItem.icon));
                        }
                        else
                        {
                            CheckIsInList(ref roleHeadList, GetRoleAppearanceId(1, dialogItem.icon));
                        }
                    }
                    else
                    {
                        int npcDetailId = 0;
                        if (dialogItem.role_id == DialogDisplaySystem.Instance.CurrentBookData.NpcId)
                            npcDetailId = DialogDisplaySystem.Instance.CurrentBookData.NpcDetailId;
                        if (dialogItem.dialog_type == (int)DialogType.EnterNPCname || (dialogItem.dialog_type == (int)DialogType.ChangeWholeClothes && dialogItem.trigger == 3))
                        {
                            int selNum = dialogItem.selection_num;
                            
                            if (selNum > 0)
                                AddClothsAndRoleHead(ref clothsList, ref npcList, GetNpcAppearanceId(dialogItem.role_id, dialogItem.icon, int.Parse(dialogItem.selection_1)));
                            if (selNum > 1)
                                AddClothsAndRoleHead(ref clothsList, ref npcList, GetNpcAppearanceId(dialogItem.role_id, dialogItem.icon, int.Parse(dialogItem.selection_2)));
                            if (selNum > 2)
                                AddClothsAndRoleHead(ref clothsList, ref npcList, GetNpcAppearanceId(dialogItem.role_id, dialogItem.icon, int.Parse(dialogItem.selection_3)));
                            if (selNum > 3)
                                AddClothsAndRoleHead(ref clothsList, ref npcList, GetNpcAppearanceId(dialogItem.role_id, dialogItem.icon, int.Parse(dialogItem.selection_4)));
                        }
                        else
                        {
                            CheckIsInList(ref npcList, GetNpcAppearanceId(dialogItem.role_id, dialogItem.icon, npcDetailId));
                        }
                    }
                }
            }
        }
#endregion

#region 统计章节内容的加载路径

        string typeResPath = string.Empty;
        len = bgmList.Count;
        for(int i = 0;i<len;i++)
        {
            typeResPath = mBookFolderPaht + "Music/BGM/" + bgmList[i] + ".";
            AddToChapterResDic(typeResPath);
        }
        if (hasPhoneCall)
            AddToChapterResDic(mBookFolderPaht + "Music/BGM/TelephoneRing_didi.");

        len = sceneBgList.Count;
        for (int i = 0; i < len; i++)
        {
            typeResPath = mBookFolderPaht + "SceneBG/" + sceneBgList[i] + ".";
            AddToChapterResDic(typeResPath);
        }

        len = particleList.Count;
        for (int i = 0; i < len; i++)
        {
            typeResPath = mBookFolderPaht + "UIParticle/" + particleList[i] + ".";
            AddToChapterResDic(typeResPath);
        }

#if false
        len = roleHeadList.Count;
        for (int i = 0; i < len; i++)
        {
            typeResPath = mBookFolderPaht + "RoleHead/" + roleHeadList[i] + ".";
            AddToChapterResDic(typeResPath);
        }

        len = clothsList.Count;
        for (int i = 0; i < len; i++)
        {
            typeResPath = mBookFolderPaht + "RoleClothes/" + clothsList[i] + ".";
            AddToChapterResDic(typeResPath);
        }

        len = npcList.Count;
        for (int i = 0; i < len; i++)
        {
            typeResPath = mBookFolderPaht + "RoleHead/" + npcList[i] + ".";
            AddToChapterResDic(typeResPath);
        }
#else
        Action<int> AddSpineRes = (value) =>
        {
            //typeResPath = mBookFolderPaht + "Role/" + value + ".";
            //AddToChapterResDic(typeResPath);
            //typeResPath = mBookFolderPaht + "Role/" + value + ".atlas.";
            //AddToChapterResDic(typeResPath);
            //typeResPath = mBookFolderPaht + "Role/" + value + ".skel.";
            //AddToChapterResDic(typeResPath);
            //typeResPath = mBookFolderPaht + "Role/" + value + "_Atlas.";
            //AddToChapterResDic(typeResPath);
            //typeResPath = mBookFolderPaht + "Role/" + value + "_Material.";
            //AddToChapterResDic(typeResPath);
            typeResPath = mBookFolderPaht + "Role/" + value + "_SkeletonData.";
            AddToChapterResDic(typeResPath);
        };

        len = roleHeadList.Count;
        for (int i = 0; i < len; i++)
        {
            AddSpineRes(roleHeadList[i]);
        }

        len = clothsList.Count;
        for (int i = 0; i < len; i++)
        {
            AddSpineRes(clothsList[i]);
        }

        len = npcList.Count;
        for (int i = 0; i < len; i++)
        {
            AddSpineRes(npcList[i]);
        }
#endif

        len = goodItemsList.Count;
        for (int i = 0; i < len; i++)
        {
            typeResPath = mBookFolderPaht + "StoryItems/icon" + goodItemsList[i] + ".";
            AddToChapterResDic(typeResPath);
        }
#endregion
    }

    private List<int> mSelectList = new List<int>();
    public List<int> GetSelectionsList(t_BookDialog vDialogItem)
    {
        mSelectList.Clear();
        mSelectList.Add((string.IsNullOrEmpty(vDialogItem.selection_1)) ? 0 : int.Parse(vDialogItem.selection_1));
        mSelectList.Add((string.IsNullOrEmpty(vDialogItem.selection_2)) ? 0 : int.Parse(vDialogItem.selection_2));
        mSelectList.Add((string.IsNullOrEmpty(vDialogItem.selection_3)) ? 0 : int.Parse(vDialogItem.selection_3));
        mSelectList.Add((string.IsNullOrEmpty(vDialogItem.selection_4)) ? 0 : int.Parse(vDialogItem.selection_4));
        return mSelectList;
    }
    private void AddClothsAndRoleHead(ref List<int> vClothsList,ref List<int> vRoleHeadList,int vId)
    {
        CheckIsInList(ref vClothsList, vId);
        CheckIsInList(ref vRoleHeadList, vId);
    }

    private void AddToChapterResDic(string value)
    {
        if(!mChapterResDic.ContainsKey(value))
        {
            mChapterResDic.Add(value, true);
        }
    }

    private int GetRoleAppearanceId(int vRoleId, int vClothId)
    {
#if false
        int appearanceID = (100000 + (vRoleId * 10000) + (1 * 100) + vClothId) * 10000;
#else
        if (vClothId == 0)
            vClothId = 1;
        int clothGroupId = Mathf.CeilToInt(vClothId / (4 * 1.0f));
        int appearanceID = (100000 + (vRoleId * 10000) + clothGroupId) * 10000;
#endif
        return appearanceID;
    }
    private int GetNpcAppearanceId(int vRoleId,int vClothId,int vNpcDetail)
    {
#if false
        int appearanceID = (100000 + (vRoleId * 100) + vClothId) * 10000 + vNpcDetail;
#else
        if (vClothId == 0)
            vClothId = 1;
        int clothGroupId = Mathf.CeilToInt(vClothId / (4 * 1.0f));
        int sex = Mathf.CeilToInt(vNpcDetail / (3 * 1.0f));
        int appearanceID = (100000 + (vRoleId * 100) + vClothId) * 10000+sex;
#endif
        return appearanceID;
    }

    /// <summary>
    /// 判断并添加进列表
    /// </summary>
    /// <param name="vList"></param>
    /// <param name="value"></param>
    /// <param name="vIsCheckZero"></param>
    private void CheckIsInList(ref List<int> vList, int value, bool vIsCheckZero = true)
    {
        if(vList != null)
        {
            if ( !vIsCheckZero ||  (vIsCheckZero && value != 0))
            {
                if (vList.IndexOf(value) == -1)
                    vList.Add(value);
            }
        }
    }


    //获取章节对应的文件夹id
    private string GetChapterFolderId(bool isCommon = false)
    {
        string result = "common";
        if (isCommon) 
            result = "common"; 
        else
            result = "Chapter_" + m_currentBookData.ChapterID;
        return result;
    }

    bool ReleasePreLoadData()
    {
        if (m_currentBookData.BookID == this.m_lastBookID)
        {
            return false;
        }
        LOG.Info("不同书本，清除所有资源");
        this.ClearBookRes();
        return true;
    }

    //不同书本的时候，不用检查章节是否相同，都清理掉
    //相同的书本，检查章节是否相同，如果不同，则清理资源
    bool ReleasePreChapterData()
    {
        if (m_currentBookData.ChapterID == this.m_lastChapterID)
        {
            return false;
        }

        LOG.Info("同书本，不同章节");
        var lstTask = this.resPreLoader.lstTask;
        List<string> lstRemove = new List<string>(lstTask.Count);

        foreach(var itr in lstTask)
        {
            var path = itr.Key;
            int dotIndex = path.LastIndexOf(".") + 1;
            string noSuffixPath = path.Substring(0, dotIndex);
            if (noSuffixPath.IndexOf(mBookFolderPaht) != -1
               && path.IndexOf(mBookCommonPath) == -1 && !mChapterResDic.ContainsKey(noSuffixPath))
            {
                lstRemove.Add(path);
            }
        }

        if(lstRemove.Count > 0)
        {
            for (int i = 0; i < lstRemove.Count; ++i)
            {
                var key = lstRemove[i];
                lstTask[key].Release(this.resPreLoader);
                lstTask.Remove(key);
            }
            ABMgr.Instance.GC();
        }
        
        return true;
    }

    //private void CheckRemoveAssetByNeedLoad(AbTask vAsset)
    //{
    //    if(m_needLoadAssetData == null)return;
    //    int index = m_needLoadAssetData.IndexOf(vAsset);
    //    if (index != -1)
    //        m_needLoadAssetData.RemoveAt(index);
    //}
    

    public float GetPreLoadProgress()
    {
        return this.resPreLoader.GetPreLoadProgress();
    }

    public void ClearBookRes()
    {
        ABSystem.Instance.ClearAssetTag(AbTag.DialogDisplay);
        this.resPreLoader.Clean();
        ABSystem.Instance.GC();
    }

    //private Dictionary<string, bool> recordDic = new Dictionary<string, bool>();
    public bool IsPreLoadDone()
    {
        return this.resPreLoader.IsPreLoadDone();
        //if (m_needLoadAssetData.Count == 0 || !mCollectLoaded)
        //{
        //    return false;
        //}
        //bool isDone = true;

        //foreach (var d in m_needLoadAssetData)
        //{

        //    if (!d.IsDone())
        //    {
        //        isDone = false;
        //        break;
        //    }
        //    else
        //    {
        //        if((d is CAsset) && !recordDic.ContainsKey(((CAsset)d).assetName))
        //        {
        //            recordDic.Add(((CAsset)d).assetName,true);
        //            //Debug.Log("====isDone--->" + ((CAsset)d).assetName +"---len-->"+m_needLoadAssetData.Count);
        //        }
        //    }
        //}
        //return isDone;
    }

    //========================================
    [System.NonSerialized]
    public string m_resRootPath;

    public GameObject GetPrefab(string path)
    {
        var assetName = string.Concat(m_resRootPath, GetChapterFolderId(true) + "/", path, ".prefab");
        GameObject prefab = ABSystem.ui.GetGameObject(AbTag.DialogDisplay,assetName);
        if (prefab == null)
        {
            return null;
        }
        return prefab;
    }

    public AbAtlas GetAtlas(string strAtlasName)
    {
        var assetName = string.Concat(m_resRootPath, "atlas/", strAtlasName, ".prefab");
        var asset = ABSystem.ui.bundle.LoadImme(AbTag.DialogDisplay, enResType.eAtlas, assetName);
        if (asset == null)
        {
            LOG.Error("预加载图集失败:" + strAtlasName);
            return null;
        }
        var atlas = asset.resPrefab.GetComponent<AbAtlas>();
        return atlas;
    }

    public GameObject Load(string path, Transform parents = null, bool needInstant = true)
    {
        var assetName = string.Concat(m_resRootPath, path, ".prefab");
        GameObject prefab = ABSystem.ui.GetGameObject(AbTag.DialogDisplay, assetName);
        if (prefab == null)
        {
            return null;
        }
        GameObject go = GameObject.Instantiate(prefab, parents);
        return go;
    }

    public Sprite GetUITexture(string strSpriteName, bool isCommon = true)
    {
        if(isCommon)
            return ABSystem.ui.GetUITexture(AbTag.DialogDisplay, string.Concat(m_resRootPath, GetChapterFolderId(isCommon), "/", strSpriteName, ".png"));

        return ABSystem.ui.GetUITexture(AbTag.DialogDisplay, string.Concat(m_resRootPath, strSpriteName, ".png"));
    }

    public SkeletonDataAsset GetSkeDataAsset(string skeDataPath)
    {
        SkeletonDataAsset skeData = ABSystem.ui.GetObject(AbTag.DialogDisplay, m_resRootPath + skeDataPath + "_skeletondata.asset") as SkeletonDataAsset;
        return skeData;
    }

    /// <summary>
    /// 这是新手的资源加载
    /// </summary>
    /// <param name="strSpriteName"></param>
    /// <returns></returns>
    public Sprite NewFirstGetUiTexture(string strSpriteName)
    {
        var asset = ABSystem.ui.bundle.LoadImme(AbTag.DialogDisplay, enResType.eSprite, "assets/bundle/book/0/" + strSpriteName + ".png");
        if (asset == null)
        {
            return null;
        }
        return asset.resSprite;
    }

    public Sprite GetUISprite(string strSpriteName)
    {
        return ABSystem.ui.GetAtlasSprite(strSpriteName);
    }

    public Sprite LoadSprite(string strSpriteName)
    {
        var asset = ABSystem.ui.bundle.LoadImme(AbTag.DialogDisplay, enResType.eSprite, m_resRootPath + GetChapterFolderId(true) + "/" + strSpriteName);
        if (asset == null)
        {
            return null;
        }
        return asset.resSprite;
    }

    public Sprite GetCoverImage()
    {
        return LoadSprite("Cover/0001.png");
    }

#region Audio
    public void StopBGM()
    {
        AudioManager.Instance.StopBGM();
    }
    public void PlayTones(AudioTones audioTones)
    {
        AudioManager.Instance.PlayTones(audioTones);
    }

    public void StopTones(AudioTones audioTones)
    {
        AudioManager.Instance.StopTones(audioTones);
    }

    public void PlayBGM(int bookID, string BGMID, bool isCommon = false)
    {
        //ResourceManager.Instance.GetAudioBGM(bookID + "/" + BGMID)
        //Debug.Log("-----bookID---" + bookID + "---BGM ID--" + BGMID);
        if (string.IsNullOrEmpty(BGMID))
        {
            AudioManager.Instance.PlayBGM(null);
        }
        else
        {
            if (UserDataManager.Instance.UserData.BgMusicIsOn == 0) return;
            var asset = ABSystem.ui.bundle.LoadImme(AbTag.DialogDisplay, enResType.eAudio, string.Concat(m_resRootPath,"music/bgm/", BGMID, ".mp3"));
            AudioManager.Instance.PlayBGM(asset.resAudioClip);
        }
    }
#endregion

#endregion

    public void PrepareReading(string bookurl = null)
    {
        LOG.Info("开始阅读:bookID="+ m_currentBookData.BookID+",chapterID="+ m_currentBookData.ChapterID);
        CUIManager.Instance.CloseForm(UIFormName.BookDisplayForm);
        CUIManager.Instance.CloseForm(UIFormName.MainFormTop);
        //CUIManager.Instance.CloseForm(UIFormName.MainForm);
        //关闭主界面
        XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:Close(logic.uiid.UIMainForm);");
        //CUIManager.Instance.CloseForm(UIFormName.BookReadingForm);
        CUIManager.Instance.OpenForm(UIFormName.BookLoadingForm);



        //先加cover图
        //Sprite cover = ABSystem.ui.GetUITexture(AbTag.Global,string.Concat("assets/bundle/BookPreview/banner/book_", m_currentBookData.BookID, ".png"));
        //var loadindForm = CUIManager.Instance.GetForm<BookLoadingForm>(UIFormName.BookLoadingForm);
        //loadindForm.SetCoverImage(cover);

        mBookFolderPaht = "Assets/Bundle/Book/" + m_currentBookData.BookID + "/";
        mBookCommonPath = mBookFolderPaht + GetChapterFolderId(true) + "/";


        //加载配置
        Action<string> doLoad = (url) =>
        {
            DialogDisplaySystem.Instance.StartCoroutine(LoadDialogData(url, () =>
            {
                m_currentDialogData = GetBaseDialogDataByDialogueID(m_currentBookData.DialogueID);//加载对应的行的表的数据
                                                                                                  //加载其它资源
                DialogDisplaySystem.Instance.PreLoadRes((bSuc) =>
                {
                    if (bSuc)
                    {
                        CUIManager.Instance.OpenForm(UIFormName.BookReadingForm);
                        var uiBookReadingForm = CUIManager.Instance.GetForm<BookReadingForm>(UIFormName.BookReadingForm);
                        uiBookReadingForm.PrepareReading();
                    }
                });
            }));
        };
        if(string.IsNullOrEmpty(bookurl))
        {
            GameHttpNet.Instance.BuyChapter(m_currentBookData.BookID, m_currentBookData.ChapterID, (arg) =>
            {
                string result = arg.ToString();
                var jsonData = LitJson.JsonMapper.ToObject(result);
                var url = jsonData["data"]["bookurl"];
                bookurl = (url != null ? url.ToString() : "");
                doLoad(bookurl);
            });
        }
        else
        {
            doLoad(bookurl);
        }
    }

    public void StartReading()
    {
        this.m_lastBookID = this.m_readBookID;
        this.m_lastChapterID = this.m_readChapterID;

        this.resPreLoader.EndPreLoad();
        if (onPreLoadCallback != null)
        {
            onPreLoadCallback(true);
            onPreLoadCallback = null;
        }
        LOG.Error(m_currentBookData.BookID+":加载书本资源数量:"+this.resPreLoader.lstTask.Count);
    }

    public void AddClothes(int vClothId)
    {
        if (m_currentBookData != null)
        {
            int index = m_currentBookData.ClothesList.IndexOf(vClothId);
            if (index == -1)
            {
                m_currentBookData.ClothesList.Add(vClothId);
            }
        }
    }

    public bool CheckHasThisCloth(int vClothId)
    {
        if (m_currentBookData != null)
        {
            return m_currentBookData.ClothesList.IndexOf(vClothId) != -1;
        }
        return false;
    }

    public void AddChapterzPayId(int vBookId, int vChapterId)
    {
        if (m_currentBookData != null && m_currentBookData.BookID == vBookId)
        {
            int index = m_currentBookData.ChapterPayList.IndexOf(vChapterId);
            if (index == -1)
            {
                m_currentBookData.ChapterPayList.Add(vChapterId);
            }
        }
    }

    public bool CheckHasPayChapter(int vBookId, int vChapterId)
    {
        if (m_currentBookData != null && m_currentBookData.BookID == vBookId)
        {
            if (m_currentBookData.ChapterPayList.IndexOf(vChapterId) != -1)
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

    public string GetRoleName(int roleId,int bookId)
    {
        if (roleId == 1)
            return DialogDisplaySystem.Instance.CurrentBookData.PlayerName;

        if (roleId == DialogDisplaySystem.Instance.CurrentBookData.NpcId && !string.IsNullOrEmpty(DialogDisplaySystem.Instance.CurrentBookData.NpcName))
            return DialogDisplaySystem.Instance.CurrentBookData.NpcName;

        var cfg = GameDataMgr.Instance.table.GetBookDetailsById(bookId);
        if (cfg.BookCharacterArray.Length == 0 || roleId - 1 >= cfg.BookCharacterArray.Length)
        {
            LOG.Error("角色id错误:roleID = " + roleId + ",role count = " + cfg.BookCharacterArray.Length);
            return "Error";
        }
        else
        {
            return cfg.BookCharacterArray[roleId - 1];
        }
    }

    /// <summary>
    /// 重置书本或者重置章节时，清理一些额外的章节
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vChapterId"></param>
    public void UpdatePayChapterRecordByReset(int vBookId,int vChapterId)
    {
        if (m_currentBookData != null && m_currentBookData.BookID == vBookId)
        {
            List<int> payList = m_currentBookData.ChapterPayList;
            int len = payList.Count;
            for(int i = len - 1;i >= 0;i--)
            {
                if (m_currentBookData.ChapterPayList[i] > vChapterId)
                    m_currentBookData.ChapterPayList.RemoveAt(i);
            }
        }

        if (UserDataManager.Instance.bookCostChapterList != null && UserDataManager.Instance.bookCostChapterList.data != null)
        {
            List<BookCostChapterItemInfo> costList = UserDataManager.Instance.bookCostChapterList.data.costarr;
            if (costList != null)
            {
                int len = costList.Count;
                for (int i = len - 1; i >=0; i--)
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

    public void ResetCurBookPlayerName()
    {
        if(CurrentBookData != null)
        {
            CurrentBookData.PlayerName = "PLAYER";
        }
    }

#if ENABLE_DEBUG
    public void ToShowDialogId(int vId)
    {
        curDialogID = vId;
    }
#endif

}
#endif
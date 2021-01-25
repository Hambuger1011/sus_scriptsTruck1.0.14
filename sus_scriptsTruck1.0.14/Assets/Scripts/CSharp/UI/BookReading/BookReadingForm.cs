//#define AUTO_PLAY


using BookReading;
using DG.Tweening;
using pb;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using AB;
using System;
using System.Collections.Generic;

public class BookReadingForm : BaseUIForm
{
    #region Element
    [System.NonSerialized]
    public UINarration narration;

    [System.NonSerialized]
    public UIPlayerDialogue playerDialogue;

    [System.NonSerialized]
    public UIOthersDialogue othersDialogue;

    [System.NonSerialized]
    public UIPlayerImagineDialogue playerImagineDialogue;

    [System.NonSerialized]
    public UIOthersImagineDialogue othersImagineDialogue;



    [System.NonSerialized]
    public UIChangeClothes changeClothes;

    [System.NonSerialized]
    public UIChangeWholeClothes changeWholeClothes;

    [System.NonSerialized]
    public UIEnterName enterName;

    [System.NonSerialized]
    public UISceneInteraction sceneInteraction;

    [System.NonSerialized]
    public UIPhoneCallDialogue phoneCallDialogue;

    [System.NonSerialized]
    public UITipsImage tipsImage;

    [System.NonSerialized]
    public UIChoiceCharacter choiceCharacter;

    [System.NonSerialized]
    public UIChoiceButtonGroup choiceButtonGroup;

    [System.NonSerialized]
    public UIChapterSwitch chapterSwitch;

    [System.NonSerialized]
    public UIChoiceRole choiceRole;
    #endregion

    private GameObject SceneBG;
    public GameObject[] SceneBGgame;
    public Image ChangeClothesImage;
    public InputField InputField;

    public Image ChangeSceneMask;
    public GameObject BackButton;
    public Text DiamondNum;
    public BookReadingFormTopBarController Topbar;
    public GameObject OperationTipsGo;
    public GameObject listenerImage;
    public InputController inputController;
    public Watch watch;
    public GameObject ClockChangeListener;
    public Canvas LayerCanvas;
    public GraphicRaycaster BookGraphicRay;
    public GameObject BarrageOpenClosegame;

    private float curOptionTime;    //当前的操作时间
    private float operationIntervalTime = 15;
    private bool showOperationHand = false, ClockEfferToFalse = false;
    private int nowUseSceneGame = 0, noUseSceneGame = 0;//记录当前正在使用的场景
    private int dialogDataTrigger = 0;//记录移动方向的类型；
    private BaseDialogData baseDialogData;//记录当前的BaseDialogData
    private float ClockAlpha = 0;//时钟场景的Alpha值
    private float mCurClickOperationTime = 0f;
    private float mAutoTestDelay = 0f;
    private int mAutoSelectId = 0;


    private bool isTweening = false;
    private float m_fScreenAdapterScale;
    private UIBookReadingElement[] elements;
    private bool mStopCalOpertionTime = false;  //停止计算操作手势提示的时间
    private Transform mNormalDialogTrans;

    int mAdsTimerSeq;
    int mAdsTimeTick;

    private bool mIsDoubleClick = false;
    private bool mInBubble = false;
    private float mLastClickCollectTime = 0;
    private float passValue = 0.0f;
    public Image PassImage;

    public GameObject InputFieldBg, SendButton, closinputButton;
    public InputField InputFieldSend;
    public BookReadingBottomBar BottomBar;
    private string SendCont;


    public GameObject barrageTrue, barrageFalse;

    private int nowBarrage, lastBarrage, page = 1;
    private List<commentlist> tem;

    public GameObject BroadScripte;
    public CanvasGroup canvaGroup;




#if NOT_USE_LUA
    public bool IsTweening
    {
        get { return isTweening; }
        set
        {
            isTweening = value;
            DialogDisplaySystem.Instance.IsTextTween = isTweening;
        }
    }
    public int BookID
    {
        get
        {
            return DialogDisplaySystem.Instance.CurrentBookData.BookID;
        }
    }

    protected override void Awake()
    {
        base.Awake();
#if AUTO_PLAY
        this.CancelInvoke("AutoPlay");
        this.InvokeRepeating("AutoPlay", 0, 1f);
#endif
        elements = this.GetComponentsInChildren<UIBookReadingElement>(true);
        foreach (var e in elements)
        {
            e.Bind(this);
        }

        MyBooksDisINSTANCE.Instance.BarrageInit();
        Topbar.Init();

        //添加按钮事件
        UIEventListener.AddOnClickListener(BackButton, backToMainForm);
        UIEventListener.AddOnClickListener(listenerImage, ChoicesButtonListener);
        UIEventListener.AddOnClickListener(BarrageOpenClosegame, BarrageOpenClose);
        UIEventListener.AddOnClickListener(SendButton, SendButtonOnclick);
        UIEventListener.AddOnClickListener(closinputButton, closinputButtonOnclick);
        InputFieldSend.onValueChanged.AddListener(InputChangeHandler);
        InputField.onEndEdit.AddListener(ReplyButtonOnclicke);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (Topbar != null)
            Topbar.Dispose();

        UIEventListener.RemoveOnClickListener(BackButton, backToMainForm);
        UIEventListener.RemoveOnClickListener(listenerImage, ChoicesButtonListener);
        UIEventListener.RemoveOnClickListener(BarrageOpenClosegame, BarrageOpenClose);
        UIEventListener.RemoveOnClickListener(SendButton, SendButtonOnclick);
        UIEventListener.RemoveOnClickListener(closinputButton, closinputButtonOnclick);

        InputFieldSend.onValueChanged.RemoveListener(InputChangeHandler);
        InputField.onEndEdit.RemoveListener(ReplyButtonOnclicke);
    }

#if AUTO_PLAY
    void AutoPlay()
    {
        var data = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute<IPointerClickHandler>(SceneBG, data, ExecuteEvents.pointerClickHandler);//模拟点击事件
    }
#endif

    //只有界面打开时调用一次
    public override void OnOpen()
    {
        base.OnOpen();
    }

    public void PrepareReading()
    {
        CancelInvoke("SpwonBarragePrefb");
        MyBooksDisINSTANCE.Instance.GetAllGameInite().Clear();
        MyBooksDisINSTANCE.Instance.GetNowUseGame().Clear();
        SceneBGgameChose();

        if (elements != null)
        {
            foreach (var e in elements)
            {
                e.SetSkin();
            }
        }
        if (LayerCanvas != null)
            LayerCanvas.enabled = true;
        if (BookGraphicRay != null)
            BookGraphicRay.enabled = true;

        if (!GameDataMgr.Instance.BookReadingFormIsOn)
        {
            addMessageListener(EventEnum.OnDiamondNumChange, OnDiamondNumChange);
            addMessageListener(EventEnum.ChangeBookReadingBgEnable, ChangeSceneBgEnableHandler);
            addMessageListener(UIEventMethodName.BookReadingForm_ShowDialogueType.ToString(), BookReadingForm_ShowDialogueType);
            addMessageListener(UIEventMethodName.BookReadingForm_IsTweening.ToString(), BookReadingForm_IsTweening);
            addMessageListener(UIEventMethodName.ClickHeadToChangeFaceExpression.ToString(), ClickHeadToChangeFaceExpressionHandler);
            addMessageListener(EventEnum.AddBarrageOnclike, AddBarrageOnclike);
        }

        resizeRectByScreenSize();
        StartReading();
        //GlobalForm.Instance.EnableTouchEffect();
        NewBookTipsChange();//刷新书本是否为最新的内容的状态
        MyBooksDisINSTANCE.Instance.SetIsPlaying(true);

        GameDataMgr.Instance.BookReadingFormIsOn = true;
        mLastClickCollectTime = Time.time;

        //Debug.Log("--------BookReadingForm   OnOpen---------1111->");

        mNormalDialogTrans = LayerCanvas.transform.Find("NormalDialogue");
        if (mNormalDialogTrans != null)
        {
            if (GameUtility.IpadAspectRatio())
            {
                mNormalDialogTrans.transform.localPosition = new Vector3(0, 90, 0);
            }
            else
            {
                mNormalDialogTrans.transform.localPosition = Vector3.zero;
            }
        }


        if (MyBooksDisINSTANCE.Instance.GetisCloseBarrage())
        {
            //关闭弹幕
            //barrageTrue.SetActive(false);
            canvaGroup.alpha = 0;
            BroadScripte.SetActive(true);
        }
        else
        {
            //打开弹幕
            //barrageTrue.SetActive(true);
            canvaGroup.alpha = 1;
            BroadScripte.SetActive(false);
        }


    }

    public override void OnClose()
    {
        base.OnClose();

        CTimerManager.Instance.RemoveTimer(mAdsTimerSeq);
        //Debug.Log("--------BookReadingForm   onClose---------->");

        ClearImageResource();
        this.gameObject.SetActive(false);
        if (LayerCanvas != null)
            LayerCanvas.enabled = false;
        if (BookGraphicRay != null)
            BookGraphicRay.enabled = false;



        DialogDisplaySystem.Instance.StopBGM();
        GlobalForm.Instance.DisableTouchEffect();

        GameDataMgr.Instance.BookReadingFormIsOn = false;
        ABMgr.Instance.GC();

        DestroyAllbarrage();
    }

    private void ClearImageResource()
    {
        if (elements != null)
        {
            foreach (var e in elements)
            {
                e.Dispose();
            }
        }
    }
    public void StartReading()
    {
        changeSceneWithoutTween(DialogDisplaySystem.Instance.CurrentDialogData);
#if ENABLE_DEBUG
        DialogDisplaySystem.Instance.ToShowDialogId(DialogDisplaySystem.Instance.CurrentDialogData.dialogID);
#endif
        OnDiamondNumChange(new Notification(UserDataManager.Instance.UserData.DiamondNum));
        //TalkingDataManager.Instance.RecordChapterFirstDialog(DialogDisplaySystem.Instance.CurrentBookData.BookID);

        resetUIForm();
        ResetOperationTime();
        readDataInitDialgue();
        DialogDisplaySystem.Instance.CurrentDialogData.ShowDialog();                 //First Show


#if CHANNEL_SPAIN
        mAdsTimeTick = 0;
        CTimerManager.Instance.RemoveTimer(mAdsTimerSeq);
        mAdsTimerSeq = CTimerManager.Instance.AddTimer(1000, -1, OnAdsTimer);
#endif
    }

#if CHANNEL_SPAIN
    private void OnAdsTimer(int timerSequence)
    {
        mAdsTimeTick += 1;
        if(mAdsTimeTick >= 60*4)
        {
            if (!LabCaveMediation.isInterstitialReady())
            {
                return;
            }
            mAdsTimeTick = 0;
            SdkMgr.Instance.ads.ShowInterstitial();
        }
    }
#endif

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void CustomUpdate()
    {
        base.CustomUpdate();

        if (curOptionTime < Time.time - operationIntervalTime)
        {
            if (!showOperationHand && !mStopCalOpertionTime)
            {
                showOperationHand = true;
                OperationTipsGo.SetActive(true);
            }
        }
        else
        {
            if (showOperationHand)
            {
                showOperationHand = false;
                OperationTipsGo.SetActive(false);
            }
        }

#if ENABLE_DEBUG
        if (GameDataMgr.Instance.InAutoPlay)
        {
            mAutoTestDelay += Time.deltaTime;
            if (mAutoTestDelay >= DialogDisplaySystem.Instance.AutoTestSpeed)
            {
                mAutoTestDelay = 0;
                if (listenerImage.GetComponent<UIEventTriggerBase.UIEventTrigger>().enabled)
                {
                    ChoicesButtonListener(null);
                }
            }
        }
#endif
    }


    #region UIFormMethods
    private void resizeRectByScreenSize()
    {
        float screenX = System.Convert.ToSingle(Screen.width);
        float screenY = System.Convert.ToSingle(Screen.height);
        //Debug.Log("screenX:"+ screenX+ "--screenY:"+ screenY+"--x:"+x+"--y:"+y);

        //得到现实屏幕跟固定屏幕的比例
        m_fScreenAdapterScale = screenY / (screenX / 750f * 1334f);
        //Debug.Log("--------BookReadingForm   OnOpen---------1111->");

    }

    public void resetUIForm()
    {
        foreach (var e in elements)
        {
            e.ResetUI();
        }

        bookDetail = JsonDTManager.Instance.GetJDTBookDetailInfo(UserDataManager.Instance.UserData.CurSelectBookID);

        Color returnColor = Color.white;
        ColorUtility.TryParseHtmlString("#" + bookDetail.DescriptionColor, out returnColor);
        narration.NarrationText.color = returnColor;

        ColorUtility.TryParseHtmlString("#" + bookDetail.DialogColor, out returnColor);
        playerDialogue.PlayerText.color = returnColor;

        othersDialogue.OtherText.color = returnColor;
        playerImagineDialogue.PlayerImagineText.color = returnColor;
        othersImagineDialogue.OtherImagineText.color = returnColor;
    }

    private void readDataInitDialgue()
    {
        BookData bookData = DialogDisplaySystem.Instance.CurrentBookData;

        t_BookDialog lastDialog = DialogDisplaySystem.Instance.GetDialogById(bookData.DialogueID - 1);

        DialogDisplaySystem.Instance.LastDialogType = lastDialog == null ? DialogType.Negative : (DialogType)lastDialog.dialog_type;

        bool cfgInPhoneCall = true;
        bool cfgInBubble = true;

        t_BookDialog curDialog = DialogDisplaySystem.Instance.GetDialogById(bookData.DialogueID);

        if (curDialog != null)
        {
            cfgInPhoneCall = curDialog.PhoneCall == 1;
            cfgInBubble = curDialog.PhoneCall == 2;

            //如果当前是气泡聊天模式,并且不是刚切进，或者切出气泡聊天类型
            if (cfgInBubble && curDialog.dialog_type != (int)DialogType.BubbleChat)
            {
                OpenBubbleForm();
            }
        }

        if (bookData.IsPhoneCallMode && cfgInPhoneCall)
        {
            phoneCallDialogue.InitData(bookData);
        }
    }
    #endregion


    /// <summary>
    /// 开场进入播放剧情
    /// </summary>
    private void BookReadingForm_ShowDialogueType(Notification notification)
    {

        BaseDialogData dialogData = notification.Data as BaseDialogData;

        DialogDisplaySystem.Instance.CurrentBookData.ChapterID = dialogData.chapterID;
        DialogDisplaySystem.Instance.CurrentBookData.DialogueID = dialogData.dialogID;

        BookPassShow(notification);//显示当前书本的阅读进度

        //TalkingDataManager.Instance.ReadAppointDialogID(DialogDisplaySystem.Instance.CurrentBookData.BookID, dialogData.chapterID, dialogData.id);
        changeBGM(dialogData);

        ShowMSstait();//显示音乐音效状态
        ResetOperationTime();


        ifDialogueTypeChangeTween(DialogDisplaySystem.Instance.LastDialogType, dialogData.dialog_type);
        string showText = StringUtils.ReplaceChar(dialogData.dialog);

        BubbleForm bubbleForm = null;
        t_BookDialog nextDialog = null;

        DialogType type = (DialogType)dialogData.dialog_type;
        switch (dialogData.dialog_type)
        {
            case DialogType.Narration:
                if (!phoneCallDialogue.m_bIsPhoneCallMode)
                {
                    showNarrationDialogueDetails(
                        phoneCallDialogue.m_bIsPhoneCallMode,
                        showText,
                        dialogData,
                        narration.Narration,
                        narration.NarrationText,
                        narration.NarrationDialogBox
                        );
                }
                else
                {
                    showNarrationDialogueDetails(
                        phoneCallDialogue.m_bIsPhoneCallMode,
                        showText,
                        dialogData,
                        phoneCallDialogue.PhoneCallNarrationBox.gameObject,
                        phoneCallDialogue.PhoneCallNarrationText,
                        phoneCallDialogue.PhoneCallNarrationBox
                        );
                }
                break;
            case DialogType.EnterName:
                setBGOnClickListenerActive(false);
                if (dialogData.trigger == 1)
                {
                    enterName.Show(dialogData);
                }
                else if (dialogData.trigger == 2)
                {
                    choiceCharacter.Show(dialogData);
                }
                else
                {
                    LOG.Error("未知trigger:" + dialogData.trigger);
                }
                break;
            case DialogType.EnterNPCname:
                setBGOnClickListenerActive(false);
                if (dialogData.trigger == 1)
                {
                    enterName.Show(dialogData, 1);
                }
                else if (dialogData.trigger == 2)
                {
                    choiceCharacter.Show(dialogData, 1);
                }
                else
                {
                    LOG.Error("未知trigger:" + dialogData.trigger);
                }
                break;
            case DialogType.PlayerDialogue:
            case DialogType.PlayerImagineDialogue:
                if (!phoneCallDialogue.m_bIsPhoneCallMode)
                {
                    playerDialogue.ChangeDialogMode((dialogData.dialog_type == DialogType.PlayerDialogue) ? 0 : 1);
                    showPlayerDialogueDetails(
                        phoneCallDialogue.m_bIsPhoneCallMode,
                        showText,
                        dialogData,
                        playerDialogue.PlayerDialogue,
                        playerDialogue.PlayerText,
                        playerDialogue.PlayerCharacterDisplay,
                        playerDialogue.PlayerName,
                        playerDialogue.PlayerDialogBox,
                        playerDialogue.DialogBoxContent
                        );
                }
                else
                {
                    showPlayerDialogueDetails(
                        phoneCallDialogue.m_bIsPhoneCallMode,
                        showText,
                        dialogData,
                        phoneCallDialogue.PhoneCallPlayerDialogue,
                        phoneCallDialogue.PhoneCallPlayerDialogueText,
                        playerDialogue.PlayerCharacterDisplay,
                        phoneCallDialogue.PhoneCallPlayerDialogueName,
                        phoneCallDialogue.PhoneCallPlayerDialogueBox,
                        phoneCallDialogue.DialogBoxContent
                        );

                    GameHttpNet.Instance.SendPlayerProgress(UserDataManager.Instance.UserData.CurSelectBookID,
                    DialogDisplaySystem.Instance.CurrentBookData.ChapterID, dialogData.dialogID, 0,
                    0, 0, 0, string.Empty, 0, 0, 0, string.Empty, 0, RecordPhoneCallCallBack);
                }
                break;
            case DialogType.OtherDialogue:
            case DialogType.OtherImagineDialogue:
                if (!phoneCallDialogue.m_bIsPhoneCallMode)
                {
                    othersDialogue.ChangeDialogMode((dialogData.dialog_type == DialogType.OtherDialogue) ? 0 : 1);
                    showOtherDialogueDetails(
                        phoneCallDialogue.m_bIsPhoneCallMode,
                        showText,
                        dialogData,
                        othersDialogue.OtherDialogue,
                        othersDialogue.OtherText,
                        othersDialogue.OtherCharacterDisplay,
                        othersDialogue.OtherName,
                        othersDialogue.OtherDialogBox,
                        othersDialogue.DialogBoxContent
                        );
                }
                else
                {
                    showOtherDialogueDetails(
                        phoneCallDialogue.m_bIsPhoneCallMode,
                        showText,
                        dialogData,
                        phoneCallDialogue.PhoneCallOtherDialogue,
                        phoneCallDialogue.PhoneCallOtherDialogueText,
                        othersDialogue.OtherCharacterDisplay,
                        phoneCallDialogue.PhoneCallOtherDialogueName,
                        phoneCallDialogue.PhoneCallOtherDialogueBox,
                        phoneCallDialogue.DialogBoxContent
                        );

                    GameHttpNet.Instance.SendPlayerProgress(UserDataManager.Instance.UserData.CurSelectBookID,
                    DialogDisplaySystem.Instance.CurrentBookData.ChapterID, dialogData.dialogID, 0,
                    0, 0, 0, string.Empty, 0, 0, 0, string.Empty, 0, RecordPhoneCallCallBack);
                }
                break;
            case DialogType.ChangeClothes:
                mStopCalOpertionTime = true;
                changeClothes.Show(dialogData);
                break;
            case DialogType.ChangeWholeClothes:
                mStopCalOpertionTime = true;
                changeWholeClothes.Show(dialogData);
                break;
            case DialogType.AutoSelectClothes:
                mAutoSelectId = int.Parse(dialogData.selection_1);
                if (dialogData.selection_num >= 1 && mAutoSelectId > 0)
                {
                    int tempIndex = 0;//即选择第一个
                    GameHttpNet.Instance.SendPlayerProgress(UserDataManager.Instance.UserData.CurSelectBookID,
                    DialogDisplaySystem.Instance.CurrentBookData.ChapterID, DialogDisplaySystem.Instance.CurrentBookData.DialogueID,
                    tempIndex, 0, 0, mAutoSelectId, string.Empty, 0, 0, 0, string.Empty, 0, AutoSelectClothesHandler);
                }
                break;
            case DialogType.ChangeSceneByBlack:
                changeSceneMask_by_Black(dialogData);
                break;
            case DialogType.ChangeSceneToBlack:
                changeSceneMask_to_Black(dialogData);
                break;

            case DialogType.ChangeSceneByWhite:
                changeSceneMask_by_White(dialogData);
                break;

            case DialogType.ChangeSceneByWave:
                changeSceneMask_by_Wave(dialogData);
                break;
            case DialogType.ChangeSceneByShutter:
                changeSceneMask_by_Shutter(dialogData);
                break;

            case DialogType.ManualChangeScene:
                mStopCalOpertionTime = true;
                ManualChangeSceneTweem(dialogData);
                break;
            case DialogType.ClockChangeScene:
                mStopCalOpertionTime = true;
                ClockEfferToFalse = true;//这个是把场景特效删除掉
                ClockChangeSceneTween(dialogData);
                break;
            //case DialogType.PlayerImagineDialogue:
            //    showPlayerDialogueDetails(
            //        phoneCallDialogue.m_bIsPhoneCallMode, 
            //        showText, 
            //        dialogData, 
            //        playerImagineDialogue.PlayerImagineDialogue, 
            //        playerImagineDialogue.PlayerImagineText, 
            //        playerImagineDialogue.PlayerImagineCharacterDisplay, 
            //        playerImagineDialogue.PlayerImagineName, 
            //        playerImagineDialogue.PlayerImagineDialogBox,
            //        playerImagineDialogue.DialogBoxContent
            //        );
            //    break;
            //case DialogType.OtherImagineDialogue:
            //    showOtherDialogueDetails(
            //        phoneCallDialogue.m_bIsPhoneCallMode, 
            //        showText, 
            //        dialogData, 
            //        othersImagineDialogue.OtherImagineDialogue, 
            //        othersImagineDialogue.OtherImagineText, 
            //        othersImagineDialogue.OtherImagineCharacterDisplay, 
            //        othersImagineDialogue.OtherImagineName, 
            //        othersImagineDialogue.OtherImagineDialogBox,
            //        othersImagineDialogue.DialogBoxContent
            //        );
            //    break;
            case DialogType.PhoneCallDialogue:
                nextDialog = DialogDisplaySystem.Instance.GetDialogById(dialogData.next);
                bool phoneState = phoneCallDialogue.m_bIsPhoneCallMode;
                if (nextDialog != null && nextDialog.PhoneCall != 1)
                {
                    //如果下一个id，就不是电话剧情，那么这里要把状态设置成现在已经在电话剧情中，主要是用户初始化在剧情的最后一个id
                    phoneState = true;
                }
                phoneCallDialogue.getPhoneCallMessage(
                    phoneState,
                    dialogData
                    );
                break;
            case DialogType.SceneInteraction:
                setBGOnClickListenerActive(false);
                sceneInteraction.SetPos(dialogData.sceneActionX, dialogData.sceneActionY);
                sceneInteraction.Show();
                break;
            case DialogType.SceneTap:
                mStopCalOpertionTime = true;
                setBGOnClickListenerActive(false);
                CUIManager.Instance.OpenForm(UIFormName.SceneTapForm);
                SceneTapForm tapForm = CUIManager.Instance.GetForm<SceneTapForm>(UIFormName.SceneTapForm);
                if (tapForm != null)
                    tapForm.Init(dialogData.trigger);
                break;
            case DialogType.Puzzle:
                mStopCalOpertionTime = true;
                setBGOnClickListenerActive(false);
                CUIManager.Instance.OpenForm(UIFormName.PuzzleForm);
                PuzzleForm puzzForm = CUIManager.Instance.GetForm<PuzzleForm>(UIFormName.PuzzleForm);
                if (puzzForm != null)
                    puzzForm.Init(dialogData.trigger);
                break;
            case DialogType.ChoiceRole:
                mStopCalOpertionTime = true;
                this.choiceRole.Show(dialogData);
                break;
            case DialogType.StoryItems:
                {
                    mStopCalOpertionTime = true;
                    var uiform = CUIManager.Instance.OpenForm(UIFormName.StoryItems);
                    var uiCtrl = uiform.GetComponent<UIStoryItems>();
                    uiCtrl.Show(dialogData);
                }
                break;
            case DialogType.BubbleChat:
                nextDialog = DialogDisplaySystem.Instance.GetDialogById(dialogData.next);
                if (!mInBubble && nextDialog.PhoneCall == 2)
                {
                    OpenBubbleForm();
                }
                else
                {
                    bubbleForm = CUIManager.Instance.GetForm<BubbleForm>(UIFormName.BubbleForm);
                    if (bubbleForm != null)
                        bubbleForm.ShowOrHideView(false);
                    mInBubble = false;
                    bubbleForm = null;
                }
                break;
            case DialogType.BubbleNarration:
            case DialogType.BubblePlayerDialog:
            case DialogType.BubbleOtherPlayerDialog:
                bubbleForm = CUIManager.Instance.GetForm<BubbleForm>(UIFormName.BubbleForm);
                if (bubbleForm != null)
                    bubbleForm.DialogNextStep(dialogData);
                break;
            default:
                LOG.Error("未知类型:" + dialogData.dialog_type);
                break;
        }

        if (IsEmojiTrigger(dialogData) &&
            (dialogData.dialog_type == DialogType.Narration || dialogData.dialog_type == DialogType.PlayerDialogue ||
            dialogData.dialog_type == DialogType.OtherDialogue || dialogData.dialog_type == DialogType.PlayerImagineDialogue ||
            dialogData.dialog_type == DialogType.OtherImagineDialogue))
        {
            setBGOnClickListenerActive(false);
            CUIManager.Instance.OpenForm(UIFormName.EmojiMsgForm);
            EmojiMsgForm emojiForm = CUIManager.Instance.GetForm<EmojiMsgForm>(UIFormName.EmojiMsgForm);
            if (emojiForm != null)
                emojiForm.Init(dialogData.trigger);
        }

        TalkingDataInfo();
    }

    private void AutoSelectClothesHandler(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----Record ClothChange ---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    if (mAutoSelectId > 0)
                    {
                        DialogDisplaySystem.Instance.CurrentBookData.PlayerClothes = mAutoSelectId;
                        int tempIndex = 0; //默认选择第一个
                        EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, tempIndex);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(jo.msg)) UITipsMgr.Instance.PopupTips(jo.msg, false);
                }
                mAutoSelectId = 0;
            }, null);
        }
    }

    private void OpenBubbleForm()
    {
        mInBubble = true;
        mStopCalOpertionTime = true;
        setBGOnClickListenerActive(false);
        CUIManager.Instance.OpenForm(UIFormName.BubbleForm);
        BubbleForm bubbleForm = CUIManager.Instance.GetForm<BubbleForm>(UIFormName.BubbleForm);
        if (bubbleForm)
            bubbleForm.ShowOrHideView(true);
    }

    //是否是表情的trigger
    private bool IsEmojiTrigger(BaseDialogData dialogData)
    {
        if (dialogData != null)
        {
            return (dialogData.trigger == 999 || dialogData.trigger == 998 || dialogData.trigger == 997 || dialogData.trigger == 996 || dialogData.trigger == 995);
        }
        return false;
    }

    private void RecordPhoneCallCallBack(object arg) { }

    //根据音乐和音效的状态，对音乐和音效状态进行设置
    private void ShowMSstait()
    {
        if (UserDataManager.Instance.UserData.BgMusicIsOn == 0)
        {
            AudioManager.Instance.StopBGM();
        }

        if (UserDataManager.Instance.UserData.TonesIsOn == 0)
        {
            AudioManager.Instance.ChangeTonesVolume(0);
        }
        else
        {
            AudioManager.Instance.ChangeTonesVolume(1);
        }
    }


    
    /// <summary>
    /// 这个是显示当前书本阅读的进度
    /// </summary>
    private void BookPassShow(Notification notification)
    {

        BaseDialogData dialogData = notification.Data as BaseDialogData;

        //Debug.Log("dialogData.chapterID:" + dialogData.chapterID);
        //Debug.Log("dialogData.dialogID:" + dialogData.dialogID);

        t_BookDetails bookDetails = JsonDTManager.Instance.GetJDTBookDetailInfo(UserDataManager.Instance.UserData.CurSelectBookID);
        int chapterDivisionArray = bookDetails.ChapterDivisionArray[0];

        //Debug.Log("章节结束：" + chapterDivisionArray);

        if (dialogData.chapterID <= 1)
        {
            int nowDialogID = dialogData.dialogID;

            int allDialogID = bookDetails.ChapterDivisionArray[0];
            passValue = (float)nowDialogID / (allDialogID * 1.0f);

            //Debug.Log("本章节目标：" + allDialogID);

        }
        else
        {

            float nowDialogID = dialogData.dialogID - bookDetails.ChapterDivisionArray[dialogData.chapterID - 2];
            float allDialogID = bookDetails.ChapterDivisionArray[dialogData.chapterID - 1] - bookDetails.ChapterDivisionArray[dialogData.chapterID - 2];
            passValue = (float)nowDialogID / (allDialogID * 1.0f);

            //Debug.Log("本章节目标：" + allDialogID);
        }

        //TO DO 添加进度条
        //Debug.LogError("此书当前的进度是：" + passValue);
        if (PassImage != null)
            PassImage.fillAmount = passValue;

        if (MyBooksDisINSTANCE.Instance.GetisCloseBarrage())
        {
            //关闭弹幕

        }
        else
        {
            //打开弹幕
            BarragePopup(passValue);
        }

    }

    //刷新这本书是否是新内容的状态
    private void NewBookTipsChange()
    {
        t_BookDetails cfg = JsonDTManager.Instance.GetJDTBookDetailInfo(UserDataManager.Instance.UserData.CurSelectBookID);
        int bookid = cfg.id;
        int bookCount = cfg.ChapterCount;

        LOG.Info("bookid:" + bookid);

        //记录当前阅读的书本，在返回书架时给这本书排在第一的位置
        MyBooksDisINSTANCE.Instance.setMyBooksFirstId(bookid);

        PlayerPrefs.SetInt("BookChapterCount" + bookid, bookCount);

        //Debug.Log("--------BookReadingForm   OnOpen---------4444->");
    }

    private void ifDialogueTypeChangeTween(DialogType lastDialogType, DialogType nowDialogType)
    {
        if (IsChangeDialogType(lastDialogType, nowDialogType))
        {
            switch (lastDialogType)
            {
                case DialogType.Negative:
                case DialogType.EnterName:
                case DialogType.EnterNPCname:
                case DialogType.ChangeSceneByBlack:
                case DialogType.ChangeSceneToBlack:
                case DialogType.ChangeSceneByWhite:
                case DialogType.ChangeSceneByWave:
                case DialogType.ChangeSceneByShutter:
                case DialogType.ChoiceCharacter:
                case DialogType.PhoneCallDialogue:
                case DialogType.BubbleChat:
                case DialogType.BubbleNarration:
                case DialogType.BubblePlayerDialog:
                case DialogType.BubbleOtherPlayerDialog:
                case DialogType.SceneInteraction:
                case DialogType.SceneTap:
                case DialogType.ChoiceRole:
                case DialogType.StoryItems:
                    break;
                case DialogType.Narration:
                    if (phoneCallDialogue.m_bIsPhoneCallMode) dialogOutTween(DialogType.Narration, phoneCallDialogue.PhoneCallNarration, phoneCallDialogue.PhoneCallPlayerDialogueBox.gameObject);
                    else dialogOutTween(DialogType.Narration, narration.Narration, narration.NarrationDialogBox.gameObject);
                    break;
                case DialogType.PlayerDialogue:
                case DialogType.PlayerImagineDialogue:
                    if (phoneCallDialogue.m_bIsPhoneCallMode) dialogOutTween(DialogType.PlayerDialogue, phoneCallDialogue.PhoneCallPlayerDialogue, phoneCallDialogue.PhoneCallPlayerDialogueBox.gameObject);
                    else dialogOutTween(DialogType.PlayerDialogue, playerDialogue.PlayerDialogue, playerDialogue.DialogBoxContent);
                    break;
                case DialogType.OtherDialogue:
                case DialogType.OtherImagineDialogue:
                    if (phoneCallDialogue.m_bIsPhoneCallMode) dialogOutTween(DialogType.OtherDialogue, phoneCallDialogue.PhoneCallOtherDialogue, phoneCallDialogue.PhoneCallPlayerDialogueBox.gameObject);
                    else dialogOutTween(DialogType.OtherDialogue, othersDialogue.OtherDialogue, othersDialogue.DialogBoxContent);
                    break;
                case DialogType.ChangeClothes:
                case DialogType.AutoSelectClothes:
                    break;
                //case DialogType.PlayerImagineDialogue:
                //    dialogOutTween(DialogType.PlayerImagineDialogue, playerImagineDialogue.PlayerImagineDialogue);
                //    break;
                //case DialogType.OtherImagineDialogue:
                //    dialogOutTween(DialogType.OtherImagineDialogue, othersImagineDialogue.OtherImagineDialogue);
                //    break;
                default:
                    break;
            }
        }
    }

    public void ResetOperationTime()
    {
        curOptionTime = Time.time;
    }

    #region UIListener
    private void BookReadingForm_IsTweening(Notification notification)
    {
        IsTweening = (bool)notification.Data;
        //Debug.Log("====================>" + IsTweening);
    }

    private void OnDiamondNumChange(Notification notification)
    {
        //DiamondNum.text = notification.Data.ToString();
    }
    private void ChangeSceneBgEnableHandler(Notification notifition)
    {
        setBGOnClickListenerActive(((int)notifition.Data) == 1);
        mStopCalOpertionTime = false;
        ResetOperationTime();
    }

    private void ChoicesButtonListener(PointerEventData data)
    {
        CollectClickTimes();
        if (Time.time - mCurClickOperationTime < 0.2) return;
        if (DialogDisplaySystem.Instance.CurrentDialogData != null)
        {
            if (IsTweening)
            {
                switch (DialogDisplaySystem.Instance.CurrentDialogData.dialog_type)
                {
                    case DialogType.Narration:
                        narration.NarrationText.StopTyperTween();
                        break;
                    case DialogType.PlayerDialogue:
                    case DialogType.PlayerImagineDialogue:
                        playerDialogue.PlayerText.StopTyperTween();
                        break;
                    case DialogType.OtherDialogue:
                    case DialogType.OtherImagineDialogue:
                        othersDialogue.OtherText.StopTyperTween();
                        break;
                        //case DialogType.PlayerImagineDialogue:
                        //    playerImagineDialogue.PlayerImagineText.StopTyperTween();
                        //    break;
                        //case DialogType.OtherImagineDialogue:
                        //    othersImagineDialogue.OtherImagineText.StopTyperTween();
                        //    break;
                }

                AudioManager.Instance.PlayTones(AudioTones.dialog_click);
            }
            else
            {
                if (
                    (DialogDisplaySystem.Instance.CurrentDialogData.dialog_type == DialogType.PlayerDialogue && DialogDisplaySystem.Instance.CurrentDialogData.trigger != 0)
                    ||
                    (DialogDisplaySystem.Instance.CurrentDialogData.dialog_type == DialogType.PlayerImagineDialogue && DialogDisplaySystem.Instance.CurrentDialogData.trigger != 0)
                    )
                {
                    return;
                }


                //事件派发(跳到下一个对话)
                EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, null);
                AudioManager.Instance.PlayTones(AudioTones.dialog_click);
            }
        }

        mCurClickOperationTime = Time.time;
        ResetOperationTime();
    }

    
    private void CollectClickTimes()
    {
        if (mLastClickCollectTime > 0)
        {
            if (Time.time - mLastClickCollectTime < 0.2f)
            {
                mIsDoubleClick = true;
            }
        }
        mLastClickCollectTime = Time.time;
    }

    private void backToMainForm(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        GameHttpNet.Instance.SendPlayerProgress(UserDataManager.Instance.UserData.CurSelectBookID,
                    DialogDisplaySystem.Instance.CurrentBookData.ChapterID, DialogDisplaySystem.Instance.CurrentBookData.DialogueID,
                    0, 0, 0, 0, string.Empty, 0, 0, 0, string.Empty, 0, RecordChapterProgress);

        // TalkingDataManager.Instance.exitChapter(DialogDisplaySystem.Instance.CurrentBookData.DialogueID, DialogDisplaySystem.Instance.CurrentBookData.ChapterID);

        CUIManager.Instance.CloseForm(UIFormName.BookReadingForm);

        //打开主界面
        XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:Open(logic.uiid.UIMainForm);");


        EventDispatcher.Dispatch(EventEnum.BookProgressUpdate);
        AudioManager.Instance.CleanClip();
    }

    private void RecordChapterProgress(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----StartReadChapterCallBack---->" + result);

    }

    #endregion




    #region DialogueDetails

    #region ShowDialogueDetails
    private void showNarrationDialogueDetails(bool isPhoneCallMode, string showText, BaseDialogData dialogData, GameObject narrationDialogue, TextTyperAnimation textTyperAnimation, RectTransform narrationDialogBox)
    {
        IsTweening = true;
        sceneBGMove(dialogData.Scenes_X, () =>
        {
            narrationDialogue.SetActive(true);
            //textTyperAnimation.text = showText;
            textDialogTween(narrationDialogBox, dialogData, textTyperAnimation, 0, showText, () => { IsTweening = false; });
            tipsImage.ShowTips(dialogData.tips);
            if (IsChangeDialogType(DialogDisplaySystem.Instance.LastDialogType, dialogData.dialog_type))
                dialogEnterTween(dialogData.dialog_type, narrationDialogue);
        });

    }

    private void showPlayerDialogueDetails(bool isPhoneCallMode, string showText, BaseDialogData dialogData, GameObject playerDialogue, TextTyperAnimation textTyperAnimation,
        CharacterFaceExpressionChange expressionChange, Text playerName, RectTransform playerDialogBox, GameObject vDialogContent)
    {
        IsTweening = true;
        playerName.text = DialogDisplaySystem.Instance.GetRoleName(dialogData.role_id, UserDataManager.Instance.UserData.CurSelectBookID);

        float mScale = 1f;
        if (GameUtility.IpadAspectRatio()) mScale = 0.7f;
        Vector3 targetScale = Vector3.one * mScale;

        sceneBGMove(dialogData.Scenes_X, () =>
        {
            if (IsChangeDialogType(DialogDisplaySystem.Instance.LastDialogType, dialogData.dialog_type))
                ResetDialogPos(dialogData.dialog_type, playerDialogue);
            playerDialogue.SetActive(true);
            playerDialogue.transform.localScale = targetScale;
            if (vDialogContent != null) vDialogContent.gameObject.SetActive(false);
            if (!isPhoneCallMode)
            {

                if (IsChangeDialogType(DialogDisplaySystem.Instance.LastDialogType, dialogData.dialog_type))
                {
                    characterShowTween(dialogData.role_id, dialogData.icon, dialogData.phiz_id, dialogData.icon_bg, 0, dialogData.Orientation, expressionChange);
                    dialogEnterTween(dialogData.dialog_type, playerDialogue, () =>
                    {
                        if (vDialogContent != null) vDialogContent.gameObject.SetActive(true);
                        //textTyperAnimation.text = showText;
                        textDialogTween(playerDialogBox, dialogData, textTyperAnimation, 0, showText, () =>
                        {
                            if (dialogData.trigger == 1)
                            {
                                choiceButtonGroup.choicesDialogInit(dialogData);
                            }
                            IsTweening = false;
                        });
                    });
                }
                else
                {
                    characterShowTween(dialogData.role_id, dialogData.icon, dialogData.phiz_id, dialogData.icon_bg, 0, dialogData.Orientation, expressionChange);
                    if (vDialogContent != null) vDialogContent.gameObject.SetActive(true);
                    //textTyperAnimation.text = showText;
                    textDialogTween(playerDialogBox, dialogData, textTyperAnimation, 0, showText, () =>
                    {
                        if (dialogData.trigger == 1)
                        {
                            choiceButtonGroup.choicesDialogInit(dialogData);
                        }
                        IsTweening = false;
                    });
                }

                characterShake(expressionChange, dialogData.is_tingle == 1);
                tipsImage.ShowTips(dialogData.tips);
            }
            else
            {
                if (vDialogContent != null) vDialogContent.gameObject.SetActive(true);

                if (IsChangeDialogType(DialogDisplaySystem.Instance.LastDialogType, dialogData.dialog_type))
                {
                    textTyperAnimation.text = "";
                    dialogEnterTween(dialogData.dialog_type, playerDialogue, () =>
                    {
                        textDialogTween(playerDialogBox, dialogData, textTyperAnimation, 0, showText, () =>
                        {
                            if (dialogData.trigger == 1)
                            {
                                choiceButtonGroup.choicesDialogInit(dialogData, false, isPhoneCallMode);
                            }
                            IsTweening = false;
                        });
                    });
                }
                else
                {
                    textTyperAnimation.text = "";
                    textDialogTween(playerDialogBox, dialogData, textTyperAnimation, 0, showText, () =>
                    {
                        if (dialogData.trigger == 1)
                        {
                            choiceButtonGroup.choicesDialogInit(dialogData, false, isPhoneCallMode);
                        }
                        IsTweening = false;
                    });

                }
            }

        });
    }

    private void showOtherDialogueDetails(bool isPhoneCallMode, string showText, BaseDialogData dialogData, GameObject otherDialogue, TextTyperAnimation textTyperAnimation,
        CharacterFaceExpressionChange expressionChange, Text otherName, RectTransform otherDialogBox, GameObject vDialogContent)
    {
        IsTweening = true;
        textTyperAnimation.gameObject.SetActive(false);
        otherName.text = DialogDisplaySystem.Instance.GetRoleName(dialogData.role_id, UserDataManager.Instance.UserData.CurSelectBookID);

        float mScale = 1f;
        if (GameUtility.IpadAspectRatio()) mScale = 0.7f;
        Vector3 targetScale = Vector3.one * mScale;

        sceneBGMove(dialogData.Scenes_X, () =>
        {
            if (IsChangeDialogType(DialogDisplaySystem.Instance.LastDialogType, dialogData.dialog_type))
                ResetDialogPos(dialogData.dialog_type, otherDialogue);
            otherDialogue.SetActive(true);
            otherDialogue.transform.localScale = targetScale;
            if (vDialogContent != null) vDialogContent.gameObject.SetActive(false);
            if (!isPhoneCallMode)
            {
                if (IsChangeDialogType(DialogDisplaySystem.Instance.LastDialogType, dialogData.dialog_type))
                {
                    characterShowTween(dialogData.role_id, dialogData.icon, dialogData.phiz_id, dialogData.icon_bg, 1, dialogData.Orientation, expressionChange);
                    dialogEnterTween(dialogData.dialog_type, otherDialogue, () =>
                    {
                        if (vDialogContent != null) vDialogContent.gameObject.SetActive(true);
                        //textTyperAnimation.text = showText;
                        textDialogTween(otherDialogBox, dialogData, textTyperAnimation, 0, showText, () =>
                        {
                            if (dialogData.trigger == 1)
                                choiceButtonGroup.choicesDialogInit(dialogData, false);

                            IsTweening = false;
                        });
                    });
                }
                else
                {
                    characterShowTween(dialogData.role_id, dialogData.icon, dialogData.phiz_id, dialogData.icon_bg, 1, dialogData.Orientation, expressionChange);
                    if (vDialogContent != null) vDialogContent.gameObject.SetActive(true);
                    //textTyperAnimation.text = showText;
                    textDialogTween(otherDialogBox, dialogData, textTyperAnimation, 0, showText, () =>
                    {
                        if (dialogData.trigger == 1)
                            choiceButtonGroup.choicesDialogInit(dialogData, false);

                        IsTweening = false;
                    });
                }

                characterShake(expressionChange, dialogData.is_tingle == 1);
                tipsImage.ShowTips(dialogData.tips);
            }
            else
            {
                if (vDialogContent != null) vDialogContent.gameObject.SetActive(true);
                if (IsChangeDialogType(DialogDisplaySystem.Instance.LastDialogType, dialogData.dialog_type))
                {
                    textTyperAnimation.text = "";
                    dialogEnterTween(dialogData.dialog_type, otherDialogue, () =>
                    {
                        //textTyperAnimation.text = showText;
                        textDialogTween(otherDialogBox, dialogData, textTyperAnimation, 0, showText, () =>
                        {
                            if (dialogData.trigger == 1)
                                choiceButtonGroup.choicesDialogInit(dialogData, false, isPhoneCallMode);

                            IsTweening = false;
                        });
                    });

                }
                else
                {
                    textTyperAnimation.text = "";
                    textDialogTween(otherDialogBox, dialogData, textTyperAnimation, 0, showText, () =>
                    {
                        if (dialogData.trigger == 1)
                            choiceButtonGroup.choicesDialogInit(dialogData, false, isPhoneCallMode);

                        IsTweening = false;
                    });

                }
            }
        });

    }

    private bool IsChangeDialogType(DialogType lastType, DialogType curType)
    {
        if (lastType != curType)
        {
            if ((lastType == DialogType.PlayerDialogue && curType == DialogType.PlayerImagineDialogue) ||
               (lastType == DialogType.PlayerImagineDialogue && curType == DialogType.PlayerDialogue) ||
               (lastType == DialogType.OtherDialogue && curType == DialogType.OtherImagineDialogue) ||
               (lastType == DialogType.OtherImagineDialogue && curType == DialogType.OtherDialogue))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    #endregion

    private void dialogEnterTween(DialogType dialogType, GameObject dialogBox, Action vCallBack = null)
    {
        RectTransform rectTransform = dialogBox.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = dialogBox.AddMissingComponent<CanvasGroup>();
        canvasGroup.DOKill();
        rectTransform.DOKill();
        float duration = 0.18f;
        float mScale = 1f;
        int targetPosX = 18;
        if (GameUtility.IpadAspectRatio())
        {
            mScale = 0.7f;
            targetPosX = 120;
        }
        Vector3 targetScale = Vector3.one * mScale;

        switch (dialogType)
        {
            case DialogType.Narration:
                canvasGroup.alpha = 0f;
                canvasGroup.DOFade(1, duration);
                //rectTransform.anchoredPosition = new Vector2(0, 100);
                rectTransform.localScale = targetScale;
                rectTransform.DOAnchorPos(new Vector2(0, 100), duration).SetEase(Ease.Flash).OnComplete(() => { if (vCallBack != null) vCallBack(); });
                break;
            case DialogType.PlayerDialogue:
                canvasGroup.alpha = 0f;
                canvasGroup.DOFade(1, duration);
                rectTransform.anchoredPosition = new Vector2(-200, -150);
                rectTransform.localScale = targetScale;
                rectTransform.DOAnchorPos(new Vector2(-1 * targetPosX, -70), duration).SetEase(Ease.Flash).OnComplete(() => { if (vCallBack != null) vCallBack(); });
                break;
            case DialogType.OtherDialogue:
                canvasGroup.alpha = 0f;
                canvasGroup.DOFade(1, duration);
                rectTransform.anchoredPosition = new Vector2(200, -150);
                rectTransform.localScale = targetScale;
                rectTransform.DOAnchorPos(new Vector2(targetPosX, -70), duration).SetEase(Ease.Flash).OnComplete(() => { if (vCallBack != null) vCallBack(); });
                break;
            case DialogType.PlayerImagineDialogue:
                canvasGroup.alpha = 0f;
                canvasGroup.DOFade(1, duration);
                rectTransform.anchoredPosition = new Vector2(-200, -150);
                rectTransform.localScale = targetScale;
                rectTransform.DOAnchorPos(new Vector2(-1 * targetPosX, -70), duration).SetEase(Ease.Flash).OnComplete(() => { if (vCallBack != null) vCallBack(); });
                break;
            case DialogType.OtherImagineDialogue:
                canvasGroup.alpha = 0f;
                canvasGroup.DOFade(1, duration);
                rectTransform.anchoredPosition = new Vector2(200, -150);
                rectTransform.localScale = targetScale;
                rectTransform.DOAnchorPos(new Vector2(targetPosX, -70), duration).SetEase(Ease.Flash).OnComplete(() => { if (vCallBack != null) vCallBack(); });
                break;
        }
    }

    private void ResetDialogPos(DialogType dialogType, GameObject dialogBox)
    {
        RectTransform rectTransform = dialogBox.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = dialogBox.AddMissingComponent<CanvasGroup>();
        canvasGroup.DOKill();
        rectTransform.DOKill();
        float mScale = 1f;
        if (GameUtility.IpadAspectRatio()) mScale = 0.7f;
        Vector3 targetScale = Vector3.one * mScale;
        switch (dialogType)
        {
            case DialogType.Narration:
                break;
            case DialogType.PlayerDialogue:
                rectTransform.anchoredPosition = new Vector2(-200, -150);
                rectTransform.localScale = targetScale;
                break;
            case DialogType.OtherDialogue:
                rectTransform.anchoredPosition = new Vector2(200, -150);
                rectTransform.localScale = targetScale;
                break;
            case DialogType.PlayerImagineDialogue:
                rectTransform.anchoredPosition = new Vector2(-200, -150);
                rectTransform.localScale = targetScale;
                break;
            case DialogType.OtherImagineDialogue:
                rectTransform.anchoredPosition = new Vector2(200, -150);
                rectTransform.localScale = targetScale;
                break;
        }

    }

    private void dialogOutTween(DialogType dialogType, GameObject dialogBox, GameObject dialogContent)
    {
        RectTransform rectTransform = dialogBox.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = dialogBox.GetComponent<CanvasGroup>();
        float duration = 0.2f;
        float scale = 0.6f;
        switch (dialogType)
        {
            case DialogType.Narration:
                canvasGroup.DOFade(0, duration);
                rectTransform.DOAnchorPos(new Vector2(0, 100), duration).SetEase(Ease.Flash).OnComplete(() => dialogBox.SetActive(false));
                break;
            case DialogType.PlayerDialogue:
                //canvasGroup.DOFade(0, duration);
                rectTransform.DOAnchorPos(new Vector2(-350, -100), duration).SetEase(Ease.Flash).OnComplete(() => dialogBox.SetActive(false));
                //dialogContent.gameObject.SetActive(false);
                rectTransform.DOScale(new Vector3(scale, scale, 1), duration).SetEase(Ease.Flash);
                break;
            case DialogType.OtherDialogue:
                //canvasGroup.DOFade(0, duration);
                rectTransform.DOAnchorPos(new Vector2(350, -100), duration).SetEase(Ease.Flash).OnComplete(() => dialogBox.SetActive(false));
                //dialogContent.gameObject.SetActive(false);
                rectTransform.DOScale(new Vector3(scale, scale, 1), duration).SetEase(Ease.Flash);
                break;
            case DialogType.PlayerImagineDialogue:
                //canvasGroup.DOFade(0, duration);
                rectTransform.DOAnchorPos(new Vector2(-350, -100), duration).SetEase(Ease.Flash).OnComplete(() => dialogBox.SetActive(false));
                //dialogContent.gameObject.SetActive(false);
                rectTransform.DOScale(new Vector3(scale, scale, 1), duration).SetEase(Ease.Flash);
                break;
            case DialogType.OtherImagineDialogue:
                //canvasGroup.DOFade(0, duration);
                rectTransform.DOAnchorPos(new Vector2(350, -100), duration).SetEase(Ease.Flash).OnComplete(() => dialogBox.SetActive(false));
                //dialogContent.gameObject.SetActive(false);
                rectTransform.DOScale(new Vector3(scale, scale, 1), duration).SetEase(Ease.Flash);
                break;
        }
    }
    private void textDialogTween(RectTransform rectTransform, BaseDialogData dialogData, TextTyperAnimation text, int offsetH, string vMsg, Action vCallBack = null)
    {
        float duration = 0.2f;
        float basePadding = 120;
        if (bookDetail != null)
            basePadding += bookDetail.DialogFrameHeight;
        text.text = vMsg;
        float rectHeight = text.preferredHeight + basePadding + offsetH;
        if (rectHeight < 180)
            rectHeight = 180;
        float currentHeight = 160;
        text.text = "";
        rectTransform.sizeDelta = new Vector2(650, currentHeight);
        bool tempDoubleClick = mIsDoubleClick;
        //显示对话窗口(先放大再缩小)
        DOTween.To(() => rectTransform.sizeDelta.y, (height) => rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height), rectHeight * 1.05f, duration).SetEase(Ease.Flash)
            .OnComplete(() =>
            {
                //缩放到原始尺寸
                DOTween.To(() => rectTransform.sizeDelta.y, (height) => rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height), rectHeight, duration * 0.5f).SetEase(Ease.Flash);
                text.gameObject.SetActive(true);
                text.text = vMsg;
                if (!tempDoubleClick)
                {
                    text.DoTyperTween();
                }
                else
                {
                    text.Progress = 1;
                }
                if (dialogData.is_tingle == 2)//文字抖动
                {
                    text.DoShake();
                }
                if (vCallBack != null)
                {
                    vCallBack();
                }
            });
        mIsDoubleClick = false;

    }
    private void characterShowTween(int role_id, int clothesID, int phiz_id, int iconBGId, int vSideType, int vOrientation, CharacterFaceExpressionChange characterFaceExpressionChange, Action vCallBack = null)
    {

        int appearanceID = 0;
        int facialExpressionID = 0;
        float duration = 0.3f;
        RectTransform rectTransform = characterFaceExpressionChange.GetComponent<RectTransform>();
        rectTransform.DOKill();
        float mScale = 1f;
        //if (GameUtility.IpadAspectRatio()) mScale = 0.7f;
        Vector3 targetScale = Vector3.one * mScale;
        int clothGroupId = 0;
        if (role_id == 1)
        {
            int playerClothsId = clothesID;
            if (clothesID != 0)
                clothesID = DialogDisplaySystem.Instance.CurrentBookData.PlayerClothes;
#if false
            appearanceID = (100000 + (DialogDisplaySystem.Instance.CurrentBookData.PlayerDetailsID * 10000) + (role_id * 100) + clothesID)*10000;
#else
            clothGroupId = Mathf.CeilToInt(DialogDisplaySystem.Instance.CurrentBookData.PlayerClothes / (4 * 1.0f));
            appearanceID = (100000 + (1 * 10000) + clothGroupId) * 10000;
#endif
            facialExpressionID = (100000 + (1 * 10000) + (role_id * 100) + phiz_id) * 10000;
            rectTransform.localScale = targetScale;
        }
        else
        {
            int npcDetailId = 0;
#if false
            if (role_id == DialogDisplaySystem.Instance.CurrentBookData.NpcId)
                npcDetailId = DialogDisplaySystem.Instance.CurrentBookData.NpcDetailId;
            appearanceID = (100000 + (role_id * 100) + clothesID)*10000 + npcDetailId;
#else
            if (role_id == DialogDisplaySystem.Instance.CurrentBookData.NpcId)
            {
                npcDetailId = Mathf.CeilToInt(DialogDisplaySystem.Instance.CurrentBookData.NpcDetailId / (3 * 1.0f));
            }

            if (clothesID != 0)
                clothGroupId = Mathf.CeilToInt(clothesID / (4 * 1.0f));
            else
                clothGroupId = 1;
            appearanceID = (100000 + (role_id * 100) + clothGroupId) * 10000 + npcDetailId;
#endif
            facialExpressionID = (100000 + (role_id * 100) + phiz_id) * 10000 + npcDetailId;
            rectTransform.localScale = targetScale;
        }

        if (DialogDisplaySystem.Instance.LastRoleID != role_id)
        {
#if false
            if(vSideType == 0)
                rectTransform.anchoredPosition = new Vector2(-300, -100);
            else
                rectTransform.anchoredPosition = new Vector2(300, -100);
            rectTransform.localScale = new Vector3(0.6f, 0.6f);
            rectTransform.DOAnchorPos(new Vector2(0, 21.7f), duration).SetEase(Ease.Flash);
            rectTransform.DOScale(targetScale, duration).SetEase(Ease.Flash).OnComplete(() =>
            {
                if (vCallBack != null)
                    vCallBack();
                rectTransform.localScale = targetScale;
            }).Play();
#else
            if (vSideType == 0)
                rectTransform.anchoredPosition = new Vector2(-300, 22);
            else
                rectTransform.anchoredPosition = new Vector2(300, 22);
            rectTransform.localScale = targetScale;
            rectTransform.DOAnchorPos(new Vector2(0, 22f), duration).SetEase(Ease.Flash).OnComplete(() =>
            {
                if (vCallBack != null)
                    vCallBack();
                rectTransform.localScale = targetScale;
            }).Play();
#endif
        }
        else
        {
            rectTransform.anchoredPosition = new Vector2(0, 22f);
            rectTransform.localScale = targetScale;
        }

        //characterFaceExpressionChange.Change(appearanceID, facialExpressionID,phiz_id, iconBGId,vOrientation);
        characterFaceExpressionChange.Change(role_id, appearanceID, clothesID, phiz_id, iconBGId, vOrientation);
    }

    private void ClickHeadToChangeFaceExpressionHandler(Notification notification)
    {
        if (DialogDisplaySystem.Instance.CurrentDialogData != null)
        {
            BaseDialogData dialogData = DialogDisplaySystem.Instance.CurrentDialogData;
            if (dialogData.phiz_id == 0) return;
            if (!phoneCallDialogue.m_bIsPhoneCallMode)
            {
                CharacterFaceExpressionChange characterFaceExpressionChange = null;
                switch (dialogData.dialog_type)
                {
                    case DialogType.PlayerDialogue:
                    case DialogType.PlayerImagineDialogue:
                        characterFaceExpressionChange = playerDialogue.PlayerCharacterDisplay;
                        break;
                    case DialogType.OtherDialogue:
                    case DialogType.OtherImagineDialogue:
                        characterFaceExpressionChange = othersDialogue.OtherCharacterDisplay;
                        break;
                    //case DialogType.PlayerImagineDialogue:
                    //    characterFaceExpressionChange = playerImagineDialogue.PlayerImagineCharacterDisplay;
                    //    break;
                    //case DialogType.OtherImagineDialogue:
                    //    characterFaceExpressionChange = othersImagineDialogue.OtherImagineCharacterDisplay;
                    //    break;
                    default:
                        LOG.Error("未知类型:" + dialogData.dialog_type);
                        break;
                }
                if (characterFaceExpressionChange != null)
                {
                    CharacterToChangeFaceExpression(dialogData.role_id, dialogData.icon, dialogData.phiz_id, 4, dialogData.icon_bg, characterFaceExpressionChange);
                }
            }
        }
    }
    private void CharacterToChangeFaceExpression(int role_id, int clothesID, int phiz_id, int vTurnPhizId, int iconBGId, CharacterFaceExpressionChange characterFaceExpressionChange)
    {
        int appearanceID = 0;
        int facialExpressionID = 0;
        int turnFaceExpreId = 0;
        if (role_id == 1)
        {
            int playerClothsId = clothesID;
            if (clothesID != 0)
                clothesID = DialogDisplaySystem.Instance.CurrentBookData.PlayerClothes;
            appearanceID = (100000 + (1 * 10000) + (role_id * 100) + clothesID) * 10000;
            facialExpressionID = (100000 + (1 * 10000) + (role_id * 100) + phiz_id) * 10000;
            turnFaceExpreId = (100000 + (1 * 10000) + (role_id * 100) + vTurnPhizId) * 10000;
        }
        else
        {
            appearanceID = (100000 + (role_id * 100) + clothesID) * 10000;
            facialExpressionID = (100000 + (role_id * 100) + phiz_id) * 10000;
            turnFaceExpreId = (100000 + (role_id * 100) + vTurnPhizId) * 10000;
        }
        //characterFaceExpressionChange.Change(appearanceID, facialExpressionID,phiz_id, iconBGId, turnFaceExpreId);
    }


    private void characterShake(CharacterFaceExpressionChange characterFaceExpressionChange, bool isShake)
    {
        RectTransform rectTransform = characterFaceExpressionChange.GetComponent<RectTransform>();
        if (isShake)
        {
            rectTransform.DOShakeRotation(0.3f, new Vector3(0, 0, 3), 200).SetDelay(0.5f).OnStart(() =>
            {
                Handheld.Vibrate();
            }).SetEase(Ease.Flash);

        }
    }


    //private void changeDialogueSayingType(Image image,bool isSaying)
    //{
    //    if(isSaying) image.sprite = DialogDisplaySystem.Instance.GetUISprite("BookReadingForm/bg_chat_2");
    //    else image.sprite = DialogDisplaySystem.Instance.GetUISprite("BookReadingForm/bg_think");
    //}

    #endregion

    #region SceneBG
    public void setBGOnClickListenerActive(bool boolean)
    {
        var evtTrigger = listenerImage.GetComponent<UIEventTriggerBase.UIEventTrigger>();
        if (evtTrigger != null)
            evtTrigger.enabled = boolean;
    }

    private void sceneBGMove(float position_x, Action vCallBack = null)
    {
        float duration = 0.4f;
        RectTransform transform = SceneBG.GetComponent<RectTransform>();
        transform.DOKill();
        if (transform.anchoredPosition.x == position_x)
        {
            if (vCallBack != null)
                vCallBack();
        }
        else
        {
            transform.DOAnchorPos(new Vector2(position_x, 0), duration).SetEase(Ease.Flash).OnUpdate(() => IsTweening = true).OnComplete(() =>
            {
                if (vCallBack != null)
                    vCallBack();
                else
                {
                    IsTweening = false;
                }
            });
        }
    }

    /// <summary>
    /// 黑色切换场景(带场景移动)
    /// </summary>
    private void changeSceneMask_by_Black(BaseDialogData dialogData)
    {
        //LOG.Info("多次场景切换");
        ChangeSceneMask.color = new Color(0, 0, 0, 0);
        SceneBGgameExchange();
        SceneBGgame[nowUseSceneGame].GetComponent<CanvasGroup>().alpha = 1;
        SceneBGgame[noUseSceneGame].GetComponent<CanvasGroup>().alpha = 1; ;

        EfferDestroy(SceneBGgame[noUseSceneGame]);


        ChangeSceneMask.DOFade(1, 0.6f).SetEase(Ease.Flash).OnUpdate(() => IsTweening = true).OnComplete(() =>
        {
            Image bg = SceneBG.GetComponent<Image>();
            bg.sprite = DialogDisplaySystem.Instance.GetUITexture("SceneBG/" + dialogData.sceneID, false);
            bg.rectTransform.anchoredPosition = Vector2.zero;
            bg.color = StringUtils.HexToColor(dialogData.sceneColor);

            SceneBG.transform.ClearAllChild();
            int[] sceneParticalArray = dialogData.SceneParticalsArray;
            for (int i = 0, iCount = sceneParticalArray.Length; i < iCount; i++)
            {
                GameObject go = DialogDisplaySystem.Instance.Load("UIParticle/" + sceneParticalArray[i], SceneBG.transform);
                Transform rect = go.GetComponent<Transform>();
                rect.ResetTransform();
                rect.localScale = new Vector3(m_fScreenAdapterScale, m_fScreenAdapterScale, m_fScreenAdapterScale);
            }
            SceneBG.GetComponent<UIDepth>().ResetOrder();

            changeBGSize(bg, m_fScreenAdapterScale);
            ChangeSceneMask.DOFade(0, 0.6f).SetEase(Ease.Flash).SetDelay(0.3f)
            .OnUpdate(() => IsTweening = true)
            .OnComplete(() =>
            {
                //IsTweening = false;
                //EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, null);
                bg.rectTransform.DOAnchorPos(new Vector2(375f - bg.rectTransform.rect.width / 2, 0), 2.5f)
                .OnUpdate(() => IsTweening = true)
                .OnComplete(() =>
                {
                    mIsDoubleClick = false;
                    IsTweening = false;
                    EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, null);
                });
            });
        });
    }

    /// <summary>
    /// 黑色切换场景（不带场景移动）
    /// </summary>
    private void changeSceneMask_to_Black(BaseDialogData dialogData)
    {
        ChangeSceneMask.color = new Color(0, 0, 0, 0);
        SceneBGgameExchange();
        SceneBGgame[nowUseSceneGame].GetComponent<CanvasGroup>().alpha = 1;
        SceneBGgame[noUseSceneGame].GetComponent<CanvasGroup>().alpha = 1; ;

        EfferDestroy(SceneBGgame[noUseSceneGame]);

        ChangeSceneMask.DOFade(1, 0.6f).SetEase(Ease.Flash).OnUpdate(() => IsTweening = true).OnComplete(() =>
        {
            Image bg = SceneBG.GetComponent<Image>();
            bg.sprite = DialogDisplaySystem.Instance.GetUITexture("SceneBG/" + dialogData.sceneID, false);
            bg.rectTransform.anchoredPosition = Vector2.zero;
            bg.color = StringUtils.HexToColor(dialogData.sceneColor);

            SceneBG.transform.ClearAllChild();
            int[] sceneParticalArray = dialogData.SceneParticalsArray;
            for (int i = 0, iCount = sceneParticalArray.Length; i < iCount; i++)
            {
                GameObject go = DialogDisplaySystem.Instance.Load("UIParticle/" + sceneParticalArray[i], SceneBG.transform);
                Transform rect = go.GetComponent<Transform>();
                rect.ResetTransform();
                rect.localScale = new Vector3(m_fScreenAdapterScale, m_fScreenAdapterScale, m_fScreenAdapterScale);
            }
            SceneBG.GetComponent<UIDepth>().ResetOrder();

            changeBGSize(bg, m_fScreenAdapterScale);
            ChangeSceneMask.DOFade(0, 0.6f).SetEase(Ease.Flash).SetDelay(0.3f)
            .OnUpdate(() => IsTweening = true)
            .OnComplete(() =>
            {
                mIsDoubleClick = false;
                IsTweening = false;
                EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, null);
            });
        });
    }



    /// <summary>
    /// 白色切换场景
    /// </summary>
    private void changeSceneMask_by_White(BaseDialogData dialogData)
    {
        //LOG.Info("多次场景切换");
        ChangeSceneMask.color = new Color(1, 1, 1, 0);
        SceneBGgameExchange();
        SceneBGgame[nowUseSceneGame].GetComponent<Image>().color = Color.white;
        SceneBGgame[nowUseSceneGame].GetComponent<CanvasGroup>().alpha = 1;
        SceneBGgame[noUseSceneGame].GetComponent<CanvasGroup>().alpha = 1; ;

        EfferDestroy(SceneBGgame[noUseSceneGame]);


        ChangeSceneMask.DOFade(1, 0.6f).SetEase(Ease.InCirc).OnUpdate(() => IsTweening = true).OnComplete(() =>
        {
            Image bg = SceneBG.GetComponent<Image>();
            bg.sprite = DialogDisplaySystem.Instance.GetUITexture("SceneBG/" + dialogData.sceneID, false);
            bg.rectTransform.anchoredPosition = Vector2.zero;
            bg.color = StringUtils.HexToColor(dialogData.sceneColor);

            SceneBG.transform.ClearAllChild();
            int[] sceneParticalArray = dialogData.SceneParticalsArray;
            for (int i = 0, iCount = sceneParticalArray.Length; i < iCount; i++)
            {
                GameObject go = DialogDisplaySystem.Instance.Load("UIParticle/" + sceneParticalArray[i], SceneBG.transform);
                Transform rect = go.GetComponent<Transform>();
                rect.ResetTransform();
                rect.localScale = new Vector3(m_fScreenAdapterScale, m_fScreenAdapterScale, m_fScreenAdapterScale);
            }
            SceneBG.GetComponent<UIDepth>().ResetOrder();

            changeBGSize(bg, m_fScreenAdapterScale);
            ChangeSceneMask.DOFade(0, 0.6f).SetEase(Ease.Flash).SetDelay(0.3f)
            .OnUpdate(() => IsTweening = true)
            .OnComplete(() =>
            {
                //IsTweening = false;
                //EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, null);
                bg.rectTransform.DOAnchorPos(new Vector2(375f - bg.rectTransform.rect.width / 2, 0), 2.5f)
                .OnUpdate(() => IsTweening = true)
                .OnComplete(() =>
                {
                    mIsDoubleClick = false;
                    IsTweening = false;
                    EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, null);
                });
            });
        });
    }


    /// <summary>
    /// 水波纹切换场景
    /// </summary>
    private void changeSceneMask_by_Wave(BaseDialogData dialogData)
    {
        //copy a bg
        var _bgGo = GameObject.Instantiate(SceneBG);
        _bgGo.name = "_bg";
        var _bgTrans = _bgGo.transform;
        _bgTrans.SetParent(SceneBG.transform.parent, false);
        var _bgCG = _bgGo.GetComponent<CanvasGroup>();
        var bgCG = SceneBG.GetComponent<CanvasGroup>();



        //LOG.Info("多次场景切换");
        ChangeSceneMask.color = new Color(0, 0, 0, 0);
        SceneBGgameExchange();
        SceneBGgame[nowUseSceneGame].GetComponent<CanvasGroup>().alpha = 1;
        SceneBGgame[noUseSceneGame].GetComponent<CanvasGroup>().alpha = 1; ;

        EfferDestroy(SceneBGgame[noUseSceneGame]);

        //遮挡屏幕
        //ChangeSceneMask.DOFade(1, 0.6f).SetEase(Ease.Flash).OnUpdate(() => IsTweening = true).OnComplete(() =>
        //{
        Image bg = SceneBG.GetComponent<Image>();
        bg.sprite = DialogDisplaySystem.Instance.GetUITexture("SceneBG/" + dialogData.sceneID, false);
        bg.rectTransform.anchoredPosition = Vector2.zero;
        bg.color = StringUtils.HexToColor(dialogData.sceneColor);

        SceneBG.transform.ClearAllChild();
        int[] sceneParticalArray = dialogData.SceneParticalsArray;
        for (int i = 0, iCount = sceneParticalArray.Length; i < iCount; i++)
        {
            GameObject go = DialogDisplaySystem.Instance.Load("UIParticle/" + sceneParticalArray[i], SceneBG.transform);
            Transform rect = go.GetComponent<Transform>();
            rect.ResetTransform();
            rect.localScale = new Vector3(m_fScreenAdapterScale, m_fScreenAdapterScale, m_fScreenAdapterScale);
        }

        SceneBG.GetComponent<UIDepth>().ResetOrder();

        //显示屏幕
        changeBGSize(bg, m_fScreenAdapterScale);
        //ChangeSceneMask.DOFade(0, 0.6f).SetEase(Ease.Flash).SetDelay(0.3f)

        var effect = this.myForm.GetCamera().gameObject.AddComponent<WaveEffect>();
        effect.Play(Mathf.Max(Screen.width, Screen.height) / 1200f);
        IsTweening = true;
        bgCG.alpha = 0;
        _bgCG.alpha = 1;
        _bgCG.DOFade(0, 1.2f).SetEase(Ease.Flash).SetDelay(0.3f)
        .OnUpdate(() =>
        {
            bgCG.alpha = 1 - _bgCG.alpha;
        })
        .OnComplete(() =>
        {
            GameObject.Destroy(_bgGo);
            GameObject.Destroy(effect);
            //IsTweening = false;
            //EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, null);
            bg.rectTransform.DOAnchorPos(new Vector2(375f - bg.rectTransform.rect.width / 2, 0), 1.5f)
        .OnUpdate(() => IsTweening = true)
        .OnComplete(() =>
        {
            mIsDoubleClick = false;
            IsTweening = false;
            EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, null);
        });
        });
        //});
    }



    /// <summary>
    /// 叶窗切换场景
    /// </summary>
    private void changeSceneMask_by_Shutter(BaseDialogData dialogData)
    {

        //copy a bg
        var _bgGo = GameObject.Instantiate(SceneBG);
        _bgGo.name = "_bg";
        var _bgTrans = _bgGo.transform;
        _bgTrans.SetParent(SceneBG.transform.parent, false);
        _bgTrans.SetAsFirstSibling();
        //var _bgCG = _bgGo.GetComponent<CanvasGroup>();
        //var bgCG = SceneBG.GetComponent<CanvasGroup>();



        //LOG.Info("多次场景切换");
        //ChangeSceneMask.color = new Color(0, 0, 0, 0);
        SceneBGgameExchange();
        SceneBGgame[nowUseSceneGame].GetComponent<CanvasGroup>().alpha = 1;
        SceneBGgame[noUseSceneGame].GetComponent<CanvasGroup>().alpha = 1; ;

        EfferDestroy(SceneBGgame[noUseSceneGame]);

        //遮挡屏幕
        //ChangeSceneMask.DOFade(1, 0.6f).SetEase(Ease.Flash).OnUpdate(() => IsTweening = true).OnComplete(() =>
        //{
        Image bg = SceneBG.GetComponent<Image>();
        bg.sprite = DialogDisplaySystem.Instance.GetUITexture("SceneBG/" + dialogData.sceneID, false);
        bg.rectTransform.anchoredPosition = Vector2.zero;
        bg.color = StringUtils.HexToColor(dialogData.sceneColor);

        var oldMaterial = bg.material;
        var tmpMaterial = bg.material = new Material(Shader.Find("Game/UI/Effect/Shutter"));

        SceneBG.transform.ClearAllChild();
        int[] sceneParticalArray = dialogData.SceneParticalsArray;
        for (int i = 0, iCount = sceneParticalArray.Length; i < iCount; i++)
        {
            GameObject go = DialogDisplaySystem.Instance.Load("UIParticle/" + sceneParticalArray[i], SceneBG.transform);
            Transform rect = go.GetComponent<Transform>();
            rect.ResetTransform();
            rect.localScale = new Vector3(m_fScreenAdapterScale, m_fScreenAdapterScale, m_fScreenAdapterScale);
        }

        SceneBG.GetComponent<UIDepth>().ResetOrder();
        //显示屏幕
        changeBGSize(bg, m_fScreenAdapterScale);
        IsTweening = true;
        tmpMaterial.SetFloat("_Count", 10);
        tmpMaterial.SetFloat("_Range", 0);
        tmpMaterial.DOFloat(1, "_Range", 1.2f).SetEase(Ease.Flash).SetDelay(0.3f)
        .OnComplete(() =>
        {
            GameObject.Destroy(_bgGo);
            bg.material = oldMaterial;
            GameObject.Destroy(tmpMaterial);
            //IsTweening = false;
            //EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, null);
            bg.rectTransform.DOAnchorPos(new Vector2(375f - bg.rectTransform.rect.width / 2, 0), 1.5f)
        .OnUpdate(() => IsTweening = true)
        .OnComplete(() =>
        {
            mIsDoubleClick = false;
            IsTweening = false;
            EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, null);
        });
        });
        //});
    }

    private void MoveSceneToShowHandler(BaseDialogData dialogData)
    {
        SceneBG.transform.rectTransform().DOAnchorPos(new Vector2(375f - SceneBG.transform.rectTransform().rect.width / 2, 0), 1.5f)
        .OnUpdate(() => IsTweening = true)
        .OnComplete(() =>
        {
            mIsDoubleClick = false;
            IsTweening = false;
            EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, null);
        });
    }


    /// <summary>
    /// 这个是手动切换场景
    /// </summary>
    /// <param name="dialogData"></param>
    private void ManualChangeSceneTweem(BaseDialogData dialogData)
    {
        baseDialogData = dialogData;
        inputController.showdirection(dialogData.trigger);
        inputController.gameObject.SetActive(true);

        ManualChangeSceneExchange();
        SceneBGgame[nowUseSceneGame].GetComponent<CanvasGroup>().alpha = 1;
        SceneBGgame[noUseSceneGame].GetComponent<CanvasGroup>().alpha = 1;

        //给背景设置图片
        Image bg = SceneBG.GetComponent<Image>();
        bg.sprite = DialogDisplaySystem.Instance.GetUITexture("SceneBG/" + dialogData.sceneID, false);
        bg.SetNativeSize();
        bg.color = StringUtils.HexToColor(dialogData.sceneColor);

        //这个是设置图片的大小与屏幕高度一致
        RectTransform rec = bg.gameObject.GetComponent<RectTransform>();
        float Pw = rec.rect.width;
        float Ph = rec.rect.height;
        float w = Screen.width;
        float h = Screen.height;
        //LOG.Info("屏幕宽度：" + w + "--屏幕高度：" + h + "--图片的宽度:" + Pw + "--图片的高度：" + Ph);
        bg.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Pw * h / Ph * 1.0f, h);

        //得到图片变化后的宽度
        float TW = Pw * h / Ph * 1.0f;
        //得到背景左右移动时，初始放置的位置
        float LRpos = (w + TW) / 2 * 1.0f;
        //得到背景上下移动时，初始放置的位置
        float UDpos = h * 1.0f;
        //测试
        //int shu = Random.Range(0, 2);
        //if (shu == 0)
        //{

        //    bg.rectTransform.anchoredPosition = new Vector3(LRpos, 0, 0);
        //    dialogDataTrigger = 1;
        //    inputController.showdirection(dialogDataTrigger);
        //}
        //else
        //{
        //    bg.rectTransform.anchoredPosition = new Vector3(baseDialogData.Scenes_X, -UDpos, 0);
        //    dialogDataTrigger = 2;
        //    inputController.showdirection(dialogDataTrigger);
        //}
        //return;
        //end

        if (dialogData.trigger == 1)
        {
            //这个是左边滑动场景

            bg.rectTransform.anchoredPosition = new Vector3(LRpos, 0, 0);

        }
        else if (dialogData.trigger == 2)
        {
            //这个是上边滑动场景

            bg.rectTransform.anchoredPosition = new Vector3(baseDialogData.Scenes_X, -UDpos, 0);

        }
        else if (dialogData.trigger == 3)
        {
            //这个是右滑动

            bg.rectTransform.anchoredPosition = new Vector3(-LRpos, 0, 0);
        }
        else if (dialogData.trigger == 4)
        {
            //这个是下滑动
            bg.rectTransform.anchoredPosition = new Vector3(baseDialogData.Scenes_X, UDpos, 0);
        }
        dialogDataTrigger = dialogData.trigger;

        mIsDoubleClick = false;

#if ENABLE_DEBUG
        if (GameDataMgr.Instance.InAutoPlay)
        {
            ManualChangeSceneMove(1);
        }
#endif
    }

    /// <summary>
    /// 这个是时钟旋转切换界面
    /// </summary>
    /// <param name="dialogData"></param>
    private void ClockChangeSceneTween(BaseDialogData dialogData)
    {
        ClockAlpha = 0;
        baseDialogData = dialogData;
        ManualChangeSceneExchange();
        ClockChangeListener.SetActive(true);
        SceneBGgame[nowUseSceneGame].GetComponent<CanvasGroup>().alpha = 0;
        SceneBGgame[noUseSceneGame].GetComponent<CanvasGroup>().alpha = 1; ;
        watch.GetTurns(5);//赋值，指定这个时钟需要转多少圈数

        //删除特效
        if (ClockEfferToFalse)
        {
            ClockEfferToFalse = false;
            EfferDestroy(SceneBGgame[noUseSceneGame]);
        }


        //给背景设置图片
        Image bg = SceneBG.GetComponent<Image>();
        bg.sprite = DialogDisplaySystem.Instance.GetUITexture("SceneBG/" + dialogData.sceneID, false);
        bg.rectTransform.anchoredPosition = Vector2.zero;
        bg.SetNativeSize();
        bg.color = StringUtils.HexToColor(dialogData.sceneColor);

        //这个是设置图片的大小与屏幕高度一致
        RectTransform rec = bg.gameObject.GetComponent<RectTransform>();
        float Pw = rec.rect.width;
        float Ph = rec.rect.height;
        float w = Screen.width;
        float h = Screen.height;
        //LOG.Info("屏幕宽度：" + w + "--屏幕高度：" + h + "--图片的宽度:" + Pw + "--图片的高度：" + Ph);
        bg.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Pw * h / Ph * 1.0f, h);

        SceneBG.transform.ClearAllChild();

        mIsDoubleClick = false;

#if ENABLE_DEBUG
        if (GameDataMgr.Instance.InAutoPlay)
        {
            ClockChangeSceneAlpha(2);
        }
#endif

    }

    /// <summary>
    ///这个是时钟切换场景的时候，当不断的转动时钟，场景的Alpha的值不断的改变
    /// </summary>
    /// <param name="value"></param>
    public void ClockChangeSceneAlpha(float value)
    {
        //手指提示不出现     
        OperationTipsGo.SetActive(false);

        ClockAlpha = value;


        if (ClockAlpha < 0)
        {
            ClockAlpha = 0;
        }

        if (ClockAlpha >= 1)
        {
            SceneBGgame[nowUseSceneGame].GetComponent<CanvasGroup>().alpha = 1;
            SceneBGgame[noUseSceneGame].GetComponent<CanvasGroup>().alpha = 0;
            SceneBGgame[noUseSceneGame].SetActive(false);

            Image SceneBGgameImage = SceneBGgame[noUseSceneGame].GetComponent<Image>();
            SceneBGgameImage.color = Color.black;
            SceneBGgameImage.sprite = null;


            Image bg = SceneBG.GetComponent<Image>();

            int[] sceneParticalArray = baseDialogData.SceneParticalsArray;
            for (int i = 0, iCount = sceneParticalArray.Length; i < iCount; i++)
            {
                GameObject go = DialogDisplaySystem.Instance.Load("UIParticle/" + sceneParticalArray[i], SceneBG.transform);
                Transform rect = go.GetComponent<Transform>();
                rect.ResetTransform();
                rect.localScale = new Vector3(m_fScreenAdapterScale, m_fScreenAdapterScale, m_fScreenAdapterScale);
            }

#if false
            ParticleSystem[] particles = SceneBG.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0 ,iCount = particles.Length; i < iCount; i++)
            {
#if false
                particles[i].GetComponent<Renderer>().sortingLayerID = UIManager.Instance.CurrentSortLayerID;
                particles[i].GetComponent<Renderer>().sortingOrder = 10;
#else
                //particles[i].GetComponent<Renderer>().sortingOrder = UIManager.Instance.CurrentSortLayerID;
#endif
                particles[i].Play();
            }
#else
            SceneBG.GetComponent<UIDepth>().ResetOrder();
#endif

            float orignalW = bg.sprite.rect.width / bg.pixelsPerUnit;
            float orignalH = bg.sprite.rect.height / bg.pixelsPerUnit;
            bg.rectTransform.anchorMax = bg.rectTransform.anchorMin;
            bg.rectTransform.sizeDelta = new Vector2(orignalW, orignalH) * m_fScreenAdapterScale;


            mIsDoubleClick = false;

            EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, null);


            return;
        }
        else
        {
            SceneBGgame[nowUseSceneGame].GetComponent<CanvasGroup>().alpha = ClockAlpha;
            SceneBGgame[noUseSceneGame].GetComponent<CanvasGroup>().alpha = 1 - ClockAlpha;
        }
    }

    /// <summary>
    /// 游戏刚开始的时候设置场景（调用一次）
    /// </summary>
    /// <param name="dialogData"></param>
    private void changeSceneWithoutTween(BaseDialogData dialogData)
    {
        //LOG.Info("初始场景切换");
        Image bg = SceneBG.GetComponent<Image>();
        bg.sprite = DialogDisplaySystem.Instance.GetUITexture("SceneBG/" + dialogData.sceneID, false);
        bg.color = StringUtils.HexToColor(dialogData.sceneColor);
        ChangeSceneMask.materialForRendering.SetFloat("_CircleRadio", 750f * m_fScreenAdapterScale);
        SceneBG.transform.ClearAllChild();
        int[] sceneParticalArray = dialogData.SceneParticalsArray;
        for (int i = 0, iCount = sceneParticalArray.Length; i < iCount; i++)
        {
            GameObject go = DialogDisplaySystem.Instance.Load("UIParticle/" + sceneParticalArray[i], SceneBG.transform);
            Transform rect = go.GetComponent<Transform>();
            rect.ResetTransform();
            rect.localScale = new Vector3(m_fScreenAdapterScale, m_fScreenAdapterScale, m_fScreenAdapterScale);
        }
        SceneBG.GetComponent<UIDepth>().ResetOrder();
        changeBGSize(bg, m_fScreenAdapterScale);
        changeMaskTweenSize(ChangeSceneMask, m_fScreenAdapterScale);
        sceneBGMove(dialogData.Scenes_X);
    }
    private void changeBGSize(Image bg, float scale)
    {
        float orignalW = bg.sprite.rect.width / bg.pixelsPerUnit;
        float orignalH = bg.sprite.rect.height / bg.pixelsPerUnit;
        bg.rectTransform.anchorMax = bg.rectTransform.anchorMin;
        bg.rectTransform.sizeDelta = new Vector2(orignalW, orignalH) * scale;
        bg.rectTransform.anchoredPosition = new Vector2(orignalW / 2 - 375f, 0);
        //left : new Vector2(orignalW - 375f, 0); right : new Vector2(375f - orignalW / 2, 0);
    }
    private void changeMaskTweenSize(Image bg, float scale)
    {
        bg.rectTransform.anchorMax = bg.rectTransform.anchorMin;
        bg.rectTransform.sizeDelta = new Vector2(750f, 1334f * scale);
    }
    #endregion


    #region TalkingData
    private void TalkingDataInfo()
    {
        // int firstSelectBook 
        int isStartReakBook = UserDataManager.Instance.UserData.IsStartReadBook;
        if (isStartReakBook == 0)
        {
            UserDataManager.Instance.UserData.IsStartReadBook = 1;
            //TalkingDataManager.Instance.NewPlayerRecord(4, DialogDisplaySystem.Instance.CurrentBookData.BookID.ToString());
        }
    }
    #endregion


    #region BGMDetails
    [System.NonSerialized]
    public int currentBGMID = -1;
    public void changeBGM(BaseDialogData dialogData)
    {
        if (dialogData.BGMID != currentBGMID)
        {
            DialogDisplaySystem.Instance.PlayBGM(UserDataManager.Instance.UserData.CurSelectBookID, dialogData.BGMID.ToString());
            currentBGMID = dialogData.BGMID;
        }
    }

    #endregion



    /// <summary>
    /// 初始的时候设置场景
    /// </summary>
    /// <returns></returns>
    private GameObject SceneBGgameChose()
    {
        inputController.gameObject.SetActive(false);
        ClockChangeListener.SetActive(false);

        for (int i = 0; i < SceneBGgame.Length; i++)
        {
            if (i == 0)
            {
                SceneBG = SceneBGgame[i];
                SceneBG.SetActive(true);
                nowUseSceneGame = i;//记录当前使用的场景
            }
            else
            {
                SceneBGgame[i].SetActive(false);
                noUseSceneGame = i;//没有使用的到的场景

                Image SceneBGgameImage = SceneBGgame[i].GetComponent<Image>();
                SceneBGgameImage.sprite = null;
                SceneBGgameImage.color = Color.black;

            }
        }
        //UIEventListener.AddOnClickListener(SceneBG, ChoicesButtonListener);
        return SceneBG;
    }

    /// <summary>
    /// 这个是自动场景调换时候的场景更换
    /// </summary>
    private GameObject SceneBGgameExchange()
    {
        ClockChangeListener.SetActive(false);
        SceneBGgame[nowUseSceneGame].SetActive(false);

        Image SceneBGgameImage = SceneBGgame[nowUseSceneGame].GetComponent<Image>();
        SceneBGgameImage.sprite = null;
        SceneBGgameImage.color = Color.black;


        SceneBGgame[noUseSceneGame].SetActive(true);
        SceneBG = SceneBGgame[noUseSceneGame];

        //更换标志位
        int tem;
        tem = nowUseSceneGame;
        nowUseSceneGame = noUseSceneGame;
        noUseSceneGame = tem;

        //UIEventListener.AddOnClickListener(SceneBG, ChoicesButtonListener);
        return SceneBG;
    }

    /// <summary>
    /// 这个是手动场景切换的时候场景的设置
    /// </summary>
    /// <returns></returns>
    private GameObject ManualChangeSceneExchange()
    {
        ClockChangeListener.SetActive(false);
        SceneBGgame[nowUseSceneGame].SetActive(true);
        SceneBGgame[noUseSceneGame].SetActive(true);
        SceneBG = SceneBGgame[noUseSceneGame];

        //把当前使用的场景显示在最上一层
        SceneBG.transform.SetSiblingIndex(1);

        //更换标志位
        int tem;
        tem = nowUseSceneGame;
        nowUseSceneGame = noUseSceneGame;
        noUseSceneGame = tem;

        //UIEventListener.AddOnClickListener(SceneBG, ChoicesButtonListener);
        return SceneBG;
    }

    /// <summary>
    /// 这个是手动滑动场景，滑动的方向正确后，场景移动
    /// </summary>
    /// <param name="moveTpye"></param>
    public void ManualChangeSceneMove(int moveTpye)
    {
        EfferDestroy(SceneBGgame[noUseSceneGame]);
        bool directionyes = false;//检查滑动的方向是否和要求滑动的方向一致
        if (dialogDataTrigger == 1)
        {
            //左滑动
            if (dialogDataTrigger == moveTpye)
            {
                directionyes = true;
            }

        }
        else if (dialogDataTrigger == 2)
        {
            //上滑动
            if (dialogDataTrigger == moveTpye)
            {
                directionyes = true;
            }

        }
        else if (dialogDataTrigger == 3)
        {
            //右滑动
            if (dialogDataTrigger == moveTpye)
            {
                directionyes = true;
            }

        }
        else if (dialogDataTrigger == 4)
        {
            //下滑动
            if (dialogDataTrigger == moveTpye)
            {
                directionyes = true;
            }

        }

        if (!directionyes)
        {
            //LOG.Info("滑动方向与要求不符合，返回");
            return;
        }


        SceneBG.GetComponent<RectTransform>().DOAnchorPos(new Vector2(baseDialogData.Scenes_X, 0), 1f).OnComplete(() =>
        {
            SceneBG.transform.ClearAllChild();
            Image bg = SceneBG.GetComponent<Image>();
            int[] sceneParticalArray = baseDialogData.SceneParticalsArray;
            for (int i = 0, iCount = sceneParticalArray.Length; i < iCount; i++)
            {
                GameObject go = DialogDisplaySystem.Instance.Load("UIParticle/" + sceneParticalArray[i], SceneBG.transform);
                Transform rect = go.GetComponent<Transform>();
                rect.ResetTransform();
                rect.localScale = new Vector3(m_fScreenAdapterScale, m_fScreenAdapterScale, m_fScreenAdapterScale);
            }

#if false
            ParticleSystem[] particles = SceneBG.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0 ,iCount = particles.Length; i < iCount; i++)
            {
#if false
                particles[i].GetComponent<Renderer>().sortingLayerID = UIManager.Instance.CurrentSortLayerID;
                particles[i].GetComponent<Renderer>().sortingOrder = 10;
#else
                //particles[i].GetComponent<Renderer>().sortingOrder = UIManager.Instance.CurrentSortLayerID;
#endif
                particles[i].Play();
            }
#else
            SceneBG.GetComponent<UIDepth>().ResetOrder();
#endif

            //changeBGSize(bg, m_fScreenAdapterScale);

            float orignalW = bg.sprite.rect.width / bg.pixelsPerUnit;
            float orignalH = bg.sprite.rect.height / bg.pixelsPerUnit;
            bg.rectTransform.anchorMax = bg.rectTransform.anchorMin;
            bg.rectTransform.sizeDelta = new Vector2(orignalW, orignalH) * m_fScreenAdapterScale;
            mIsDoubleClick = false;
            EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, null);
            //手动场景切换完毕后，把不是当前使用的场景关闭
            SceneBGgame[nowUseSceneGame].SetActive(true);
            SceneBGgame[noUseSceneGame].SetActive(false);

            Image SceneBGgameImage = SceneBGgame[noUseSceneGame].GetComponent<Image>();
            SceneBGgameImage.sprite = null;
            SceneBGgameImage.color = Color.black;

            inputController.gameObject.SetActive(false);
        });
    }

    public void BgAddBlurEffect(bool value)
    {
        if (SceneBG != null)
        {
            Image bg = SceneBG.GetComponent<Image>();
            if (bg != null)
                bg.material = (value) ? ShaderUtil.BlurEffevtMaterial() : null;
        }
    }

    private void EfferDestroy(GameObject go)
    {
        int count = go.transform.childCount;
        //LOG.Info("背景下的特效个数：" + count);
        if (count > 0)
        {

            go.transform.ClearAllChild();
        }

    }

    #region  发送弹幕逻辑
    private void closinputButtonOnclick(PointerEventData data)
    {
        InputFieldBg.SetActive(false);
    }

    public void AddBarrageOnclike(Notification notification)
    {
        //按键盘enter调用
        string st = notification.Data.ToString();
        SendCont = UserDataManager.Instance.CheckBannedWords(st);
        InputField.text = st;

        GetSendCont(st);
    }

    private void ReplyButtonOnclicke(string vSt)
    {
        //按键盘enter调用
        string st = InputField.text;
        SendCont = UserDataManager.Instance.CheckBannedWords(st);
        GetSendCont(st);
    }

    /// <summary>
    /// 这个是键盘输入发送内容后，显示出发送的按钮
    /// </summary>
    private void GetSendCont(string vSt)
    {
        iscanRemove = true;

        InputFieldBg.SetActive(true);
        InputFieldSend.text = SendCont;

        InputField.text = "";
        BottomBar.BottomTouch(null);
    }
    private bool iscanRemove = false;
    /// <summary>
    /// 如果发送框没有字数就消失
    /// </summary>
    /// <param name="vSt"></param>
    private void InputChangeHandler(string vSt)
    {

        if (string.IsNullOrEmpty(vSt) && iscanRemove)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                iscanRemove = false;
                InputField.text = "";
                InputFieldSend.text = "";
                SendCont = "";

                if (InputFieldBg == null)
                {
                    return;
                }

                InputFieldBg.SetActive(false);
            }, null);


        }
    }
    private void SendButtonOnclick(PointerEventData data)
    {

        SendCont = InputFieldSend.text;
        string st = SendCont;
        // 去空格 
        st = st.Trim();
        // 获得评论长度
        int str_len = st.Length;
        LOG.Info("发送的内容：" + st + " len:" + str_len);
        // 长度范围判断 
        if (str_len < 4 || str_len > 70)
        {
            // 直接弹窗提示 
            UITipsMgr.Instance.PopupTips("Can't send empty messages.", false);
            return;
        }

        //书本ID 
        int bookid = bookDetail.id;
        // 评论类型  弹幕 默认为2 
        int com_type = 2;
        // 评论状态 弹幕只有直接评论  值为1
        int com_reply = 1;

        BookData bookData = DialogDisplaySystem.Instance.CurrentBookData;
        t_BookDialog curDialog = DialogDisplaySystem.Instance.GetDialogById(bookData.DialogueID);

        // 书本区间标识
        int book_section = curDialog.Barrage;

        // 发送协议
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.Setcomment(com_reply, com_type, bookid, st, book_section, SetcommentCallBack);

    }


    private void SetcommentCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----SetcommentreportCallback---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    CancelInvoke("SpwonBarragePrefb");

                    UITipsMgr.Instance.PopupTips("Reply success!", false);

                    //实例化出弹幕的预制体
                    //Invoke("spwanGo", 0.5f);

                    spwanGo();
                }

            }, null);
        }
    }

    private void spwanGo()
    {
        //CancelInvoke("spwanGo");
        GameObject go = MyBooksDisINSTANCE.Instance.GetPrefb();
        go.GetComponent<BarrageItemForm>().sendBarrage(SendCont);
        go.SetActive(true);
        go.transform.SetParent(barrageTrue.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        InputField.text = "";
        InputFieldSend.text = "";
        SendCont = "";
        InputFieldBg.SetActive(false);
        InvokeRepeating("SpwonBarragePrefb", 0.5f, 0.8f);
    }
    #endregion

    #region 弹幕弹出逻辑
    /// <summary>
    /// 这个是弹幕弹出，的方法调用
    /// </summary>
    private void BarragePopup(float passe)
    {
        BookData bookData = DialogDisplaySystem.Instance.CurrentBookData;
        t_BookDialog curDialog = DialogDisplaySystem.Instance.GetDialogById(bookData.DialogueID);
        nowBarrage = curDialog.Barrage;
        //Debug.Log("区间："+ curDialog.Barrage);

        if (passe >= 1)
        {
            //这本书，这个章节读完了
            if (tem == null)
            {
                //清除上个区间残留在列表中的数据
                tem = new List<commentlist>();
                tem.Clear();
            }
        }
        else
        {
            if (nowBarrage != lastBarrage)
            {
                //跳到了下一个区间
                if (tem == null)
                {
                    //清除上个区间残留在列表中的数据
                    tem = new List<commentlist>();
                    tem.Clear();
                }
                page = 1;
                lastBarrage = nowBarrage;
                GameHttpNet.Instance.Getcomment(2, bookDetail.id, nowBarrage, page, GetcommentCallBacke);
            }
            else
            {
                //还是在这个区间里面

            }
        }
    }

    private void GetcommentCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetcommentCallBacke---->" + result);
        if (result.Equals("error"))
        {
            LOG.Info("---GetcommentCallBacke--这条协议错误");
            return;
        }
        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();
            JsonObject jo = JsonHelper.JsonToJObject(result);

            if (jo.code == 200)
            {
                UserDataManager.Instance.Getcomment = JsonHelper.JsonToObject<HttpInfoReturn<Getcomment>>(result);

                if (UserDataManager.Instance.Getcomment == null)
                {
                    return;
                }
                if (tem == null)
                {
                    tem = new List<commentlist>();
                }

                tem = UserDataManager.Instance.Getcomment.data.commentlist;
                if (tem.Count <= 0)
                {
                    LOG.Info("这个区间没有弹幕");
                    return;
                }

                InvokeRepeating("SpwonBarragePrefb", 0, 0.9f);
            }
        }, null);
    }

    /// <summary>
    /// 生成弹幕的预制体
    /// </summary>
    private void SpwonBarragePrefb()
    {
        if (tem != null)
        {
            if (tem.Count <= 0)
            {
                CancelInvoke("SpwonBarragePrefb");
                //这个区间的这一页数据已经，显示完了，加载下一页的数据

                List<GameObject> lise = null;

                if (UserDataManager.Instance.Getcomment == null)
                {
                    lise = MyBooksDisINSTANCE.Instance.GetNowUseGame();
                    for (int i = 0; i < lise.Count; i++)
                    {
                        lise[i].GetComponent<BarrageItemForm>().gameMoveFalse();
                    }
                    return;
                }
                page++;
                if (page > UserDataManager.Instance.Getcomment.data.pages_total)
                {
                    //这是代表页数已经加载完了

                    lise = MyBooksDisINSTANCE.Instance.GetNowUseGame();

                    //得到现在正在活跃的物体

                    if (lise.Count <= 0) return;
                    for (int i = 0; i < lise.Count; i++)
                    {
                        lise[i].GetComponent<BarrageItemForm>().gameMoveFalse();
                    }

                    return;
                }
                GameHttpNet.Instance.Getcomment(2, bookDetail.id, nowBarrage, page, GetcommentCallBacke);
            }
            else
            {
                //实例化出弹幕的预制体
                GameObject go = MyBooksDisINSTANCE.Instance.GetPrefb();

                //Debug.Log("go:"+go.name+ "--tem[0]:"+ tem[0].content+ "--barrageFalse:"+ barrageFalse.name);

                go.SetActive(true);
                go.transform.SetParent(barrageTrue.transform);
                go.transform.localPosition = Vector3.zero;
                go.GetComponent<BarrageItemForm>().Inite(tem[0], barrageFalse);
                //go.transform.localScale = Vector3.one;
                tem.RemoveAt(0);
            }
        }

    }
    /// <summary>
    /// 删除所以实例出来的弹幕
    /// </summary>
    private void DestroyAllbarrage()
    {
        List<GameObject> Allbarrage = MyBooksDisINSTANCE.Instance.GetAllGameInite();

        for (int i = 0; i < Allbarrage.Count; i++)
        {
            Allbarrage[i].GetComponent<BarrageItemForm>().DestroySelf();
        }
    }


    #endregion

    #region 关闭与打开弹幕
    private void BarrageOpenClose(PointerEventData data)
    {
        bool isCloseBarrage = MyBooksDisINSTANCE.Instance.SetisCloseBarrage();

        if (isCloseBarrage)
        {
            //关闭弹幕
            //barrageTrue.SetActive(false);
            canvaGroup.alpha = 0;
            BroadScripte.SetActive(true);
        }
        else
        {
            //打开弹幕
            //barrageTrue.SetActive(true);
            canvaGroup.alpha = 1;
            BroadScripte.SetActive(false);
        }
    }
    #endregion
#endif
}


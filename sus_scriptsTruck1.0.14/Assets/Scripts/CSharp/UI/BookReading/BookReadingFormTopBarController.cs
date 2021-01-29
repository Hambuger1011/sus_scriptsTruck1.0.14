using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UGUI;
using pb;

[XLua.LuaCallCSharp]
public class BookReadingFormTopBarController : CUIComponent
{

    public GameObject TouchArea;
    public GameObject ScreenShotBtn;
    public RectTransform LeftBar;
    public RectTransform MiddleBar;
    public RectTransform RightBar;
    public GameObject AutoPlay;
    public GameObject AutoClose;
    public Text ChapterTxt;
    public Text DiamondTxt;

    public RectTransform RewardEffect;
    public RectTransform TopGB, TopRect;

    public float MaxRestTime = 8f;
    public float TweenDuration = 1f;
    private bool isLock = false;

    public bool IsBarActive { get { return IsBarActive; } set { isBarActive = value; } }
    public float Range
    {
        get { return m_fRange; }
        set
        {
            if (m_fRange != value)
            {
                m_fRange = value;
                onRangeValueChange();
            }
        }
    }
    private float m_fRange = 1f;

    private float leftBarWidth;
    private float middleBarWidth;
    private float rightBarWidth;

    private bool isBarActive = false;
    private float restingTime = 0f;
    private int starDiamond = 0, offerH = 0;
    private bool mInResident = false;   //常驻的方式
    private bool firsEnter = false;
    private InputField mTurnInputField;
    private Button mTurnBtn;
    private Vector3 rightBarPos;


    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventDispatcher.RemoveMessageListener(EventEnum.HiddenEggRewardShow, GetRewardHandler);
        EventDispatcher.RemoveMessageListener(EventEnum.OnDiamondNumChange, OnDiamondNumChange);
        EventDispatcher.RemoveMessageListener(EventEnum.ResidentMoneyInfo, residentMonyHandler);
    }

    public void Init()
    {
        leftBarWidth = LeftBar.rect.width;
        middleBarWidth = MiddleBar.rect.width;
        rightBarWidth = RightBar.rect.width;
        rightBarPos = RightBar.transform.localPosition;

        isBarActive = true;
        restingTime = 0;
        firsEnter = true;

        starDiamond = UserDataManager.Instance.UserData.DiamondNum;//记录开始的时候所拥有的钻石

        LOG.Info("starDiamond:" + starDiamond);

        RewardEffect.gameObject.SetActive(false);
        ResetInfo();
        UIEventListener.AddOnClickListener(TouchArea, touchAreaOnClick);
        UIEventListener.AddOnClickListener(ScreenShotBtn, ClickScreenShotHandler);
        UIEventListener.AddOnClickListener(MiddleBar.gameObject, GamePlayTopDiamondOnclicke);
        UIEventListener.AddOnClickListener(RightBar.gameObject, AutoPlayOnclicke);

        EventDispatcher.AddMessageListener(EventEnum.HiddenEggRewardShow, GetRewardHandler);
        EventDispatcher.AddMessageListener(EventEnum.OnDiamondNumChange, OnDiamondNumChange);
        EventDispatcher.AddMessageListener(EventEnum.ResidentMoneyInfo, residentMonyHandler);

        if (bookReadingBottomBar != null)
            bookReadingBottomBar.Init();

#if ENABLE_DEBUG|| UNITY_EDITOR
        Transform inputTrans = this.transform.Find("DebugTurnInputField");
        if (inputTrans != null)
        {
            mTurnInputField = inputTrans.GetComponent<InputField>();
#if ENABLE_DEBUG
            mTurnInputField.gameObject.SetActive(true);
#else
            mTurnInputField.gameObject.SetActive(false);
#endif
        }
        Transform btnTrans = inputTrans.Find("DebugTurnButton");
        if (btnTrans != null)
        {
            mTurnBtn = btnTrans.GetComponent<Button>();
            mTurnBtn.onClick.AddListener(TurnToHandler);
        }
#endif


        //【屏幕适配】
        offectY = XLuaHelper.UnSafeAreaNotFit(this.myForm, null, 750, 81);
        this.onRangeValueChange();

    }

    private void TurnToHandler()
    {
        if (mTurnInputField != null)
        {
            string txtValue = mTurnInputField.text;
            if (string.IsNullOrEmpty(txtValue))
            {
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(144);
                UITipsMgr.Instance.PopupTips(Localization, false);

                //UITipsMgr.Instance.PopupTips("Enter your dialogID", false);
                return;
            }
            int tempDialogId = int.Parse(txtValue);
            if (tempDialogId <= 0)
            {
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(145);
                UITipsMgr.Instance.PopupTips(Localization, false);

                //UITipsMgr.Instance.PopupTips("Enter your dialogID", false);
                return;
            }
#if ENABLE_DEBUG || UNITY_EDITOR
#if !NOT_USE_LUA
            if ( BookReadingWrapper.Instance.CurrentBookData != null)
            {
                int tempChapterId = BookReadingWrapper.Instance.CurrentBookData.ChapterID;
                if (BookReadingWrapper.Instance.CurrentBookData.ChapterID > 0)
                    tempChapterId = BookReadingWrapper.Instance.CurrentBookData.ChapterID - 1;

                if(tempChapterId - 1 > -1)
                {
                    JDT_Chapter chapterInfo = JsonDTManager.Instance.GetJDTChapterInfo(UserDataManager.Instance.UserData.CurSelectBookID,tempChapterId);
                   if( chapterInfo != null &&  tempDialogId < chapterInfo.chapterstart)
                    {
                        BookReadingWrapper.Instance.DoTurnDialog(tempDialogId,true);
                        return;
                    }
                }
                
            }

            BookReadingWrapper.Instance.DoTurnDialog(tempDialogId);
#else
            DialogDisplaySystem.Instance.DoTurnDialog(tempDialogId);
#endif

#endif
        }
    }

    /// <summary>
    /// 这是实现顶部钻石滚动改变功能
    /// </summary>
    /// <param name="end"></param>
    private void DiamondTxtChange(int end, bool showDiamondAnim = true)
    {
        LOG.Info("starDiamond:" + starDiamond + "---end:" + end);
        touchAreaOnClick(null);

        if (!isLock)
        {
            isLock = true;
            if (starDiamond <= end)//钻石增加
            {
                int oldNum = starDiamond;
                
                //DiamondTxt.text = UserDataManager.Instance.UserData.DiamondNum.ToString();
                int newNum = end;
                if(starDiamond != end)
                {
                    starDiamond = end;

                    if (!CUIManager.Instance.GetForm(UIFormName.ChargeMoneyForm))
                    {
                        var icon = this.MiddleBar.Find("DimanondBg/DimanondImage").GetComponent<Image>();
                        UITween.AddDiamond(icon, this.DiamondTxt, oldNum, newNum);
                    }else
                    {
                        this.DiamondTxt.text = newNum.ToString();
                    }
                        
                }               
                isLock = false;

            }
          
            else
            {
                DOTween.To(() => starDiamond, (value) => { DiamondTxt.text = value.ToString(); }, end, 2).OnComplete(() =>
                {
                    DiamondTxt.text = UserDataManager.Instance.UserData.DiamondNum.ToString();
                    starDiamond = end;
                    isLock = false;
                });

              
            }
        }
     
    }

    private void OnDiamondNumChange(Notification notification)
    {
        LOG.Info("钻石改变了");
        //播放钻石数值滚动变化
        DiamondTxtChange(UserDataManager.Instance.UserData.DiamondNum, string.IsNullOrEmpty(notification.Type));

    }

    //是否钻石选项常驻
    private void residentMonyHandler(Notification notification)
    {
        var val = System.Convert.ToInt32(notification.Data);
        mInResident = (val == 1);
        if (mInResident)
            isBarActive = true;
    }

    public void Dispose()
    {
        UIEventListener.RemoveOnClickListener(TouchArea, touchAreaOnClick);
        UIEventListener.RemoveOnClickListener(ScreenShotBtn, ClickScreenShotHandler);
        UIEventListener.RemoveOnClickListener(MiddleBar.gameObject, GamePlayTopDiamondOnclicke);
        UIEventListener.RemoveOnClickListener(RightBar.gameObject, AutoPlayOnclicke);

        EventDispatcher.RemoveMessageListener(EventEnum.HiddenEggRewardShow, GetRewardHandler);
        EventDispatcher.RemoveMessageListener(EventEnum.OnDiamondNumChange, OnDiamondNumChange);
        EventDispatcher.RemoveMessageListener(EventEnum.ResidentMoneyInfo, residentMonyHandler);

        if (bookReadingBottomBar != null)
            bookReadingBottomBar.Dispose();
    }

    private void Update()
    {
        if (isBarActive)
        {
            restingTime += Time.unscaledDeltaTime;
            Range = Mathf.Lerp(Range, 1, 0.1f);
            if (restingTime > MaxRestTime)
            {
                isBarActive = false;
                restingTime = 0f;
            }
        }
        else
        {
            if (!mInResident)
                Range = Mathf.Lerp(Range, 0, 0.1f);
        }
    }

    private void ClickScreenShotHandler(UnityEngine.EventSystems.PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        HideBar();
        bookReadingBottomBar.HideBar();
        //CUIManager.Instance.OpenForm(UIFormName.ScreenShotForm);
    }

    private void touchAreaOnClick(UnityEngine.EventSystems.PointerEventData eventData)
    {
        RewardEffect.gameObject.SetActive(false);
        isBarActive = true;
        ResetInfo();
        bookReadingBottomBar.BottomTouchOnTopBottom();
    }
    public void touchAreaOnclick()
    {
        RewardEffect.gameObject.SetActive(false);
        isBarActive = true;
        ResetInfo();
        bookReadingBottomBar.BottomTouchOnTopBottom();
    }

    public BookReadingBottomBar bookReadingBottomBar;
    public void TouchAreaOnClickOnBottom()
    {
        RewardEffect.gameObject.SetActive(false);
        isBarActive = true;
        ResetInfo();
    }

    public void HideBar()
    {
        isBarActive = false;
    }

    private void ResetInfo()
    {
        if (ChapterTxt != null)
        {
#if !NOT_USE_LUA
            ChapterTxt.text = "Chapter " + BookReadingWrapper.Instance.CurrentBookData.ChapterID.ToString();
#else
            ChapterTxt.text = "Chapter " + DialogDisplaySystem.Instance.CurrentBookData.ChapterID.ToString();
#endif
        }
        if (DiamondTxt != null&& firsEnter)
        {
            DiamondTxt.text = UserDataManager.Instance.UserData.DiamondNum.ToString();
            firsEnter = false;
            Debug.Log("钻石固定增减");
        }
          
    }

    private float offectY = 0;
    private void onRangeValueChange()
    {
        TopGB.anchoredPosition = new Vector3(0/*LeftBar.anchoredPosition.y*/, Mathf.Lerp(230, -offectY, m_fRange));
    }

    private enum TopBarState
    {
        isEntering,
        isOuting,
    }

    private void GetRewardHandler(Notification notification)
    {
        //isLock = true;

        RewardEffect.gameObject.SetActive(false);
        isBarActive = true;
        RewardShowData showData = notification.Data as RewardShowData;
        if (showData != null)
        {
           
            if (showData.DiamondNum > 0)
            {
               
                LOG.Info("获得彩蛋奖励");
                UserDataManager.Instance.ResetMoney(2, showData.DiamondNum, false);
            }
            
        }
    }

    private void GamePlayTopDiamondOnclicke(UnityEngine.EventSystems.PointerEventData date)
    {
        //Debug.Log("这个是钻石的点击");
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);

        CUIManager.Instance.OpenForm(UIFormName.ChargeMoneyForm);
        CUIManager.Instance.OpenForm(UIFormName.MainFormTop);

        //游戏顶框点击砖石按钮，打开砖石充值界面
        ChargeMoneyForm Char = CUIManager.Instance.GetForm<ChargeMoneyForm>(UIFormName.ChargeMoneyForm);
        Char.SetFormStyle(2);
        Char.GamePlayOpenDiamond();
        //end

        CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop).GamePlayTopOpen("UIChargeMoneyForm");
    }

    private void AutoPlayOnclicke(UnityEngine.EventSystems.PointerEventData date)
    {
        GameDataMgr.Instance.InAutoPlay = !GameDataMgr.Instance.InAutoPlay;
        AutoPlay.SetActiveEx(!GameDataMgr.Instance.InAutoPlay);
        AutoClose.SetActiveEx(GameDataMgr.Instance.InAutoPlay);
        RightBar.transform.DOKill();
        RightBar.transform.DOLocalMoveX(rightBarPos.x - 134, 0.3f).Play();
        RightBar.transform.DOLocalMoveX(rightBarPos.x, 0.3f).SetDelay(3f).OnComplete(() => { }).Play();
        if (GameDataMgr.Instance.InAutoPlay && GameDataMgr.Instance.AutoPlayOpen != null)
            GameDataMgr.Instance.AutoPlayOpen.Call();
        if (!GameDataMgr.Instance.InAutoPlay && GameDataMgr.Instance.AutoPlayClose != null)
            GameDataMgr.Instance.AutoPlayClose.Call();
    }


}
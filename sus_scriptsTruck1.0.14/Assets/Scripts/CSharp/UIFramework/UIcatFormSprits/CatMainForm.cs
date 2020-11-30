using CatMainFormClasse;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UGUI;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CatMainForm : CatBaseForm {

   #region  需要绑定的脚本和调用的变量
    //[System.NonSerialized]
    //public CatMainScenes catScren;

    [System.NonSerialized]
    public CatSceneView catSceneView;

    [System.NonSerialized]
    public CatMainTop CatMainTop;

    [System.NonSerialized]
    public CatRobotSprite CatRobotSpr;

    [System.NonSerialized]
    public float ScreneWidth, ScreneHeight;//等到屏幕的宽度和高度

    [System.NonSerialized]
    public MainTopSprite MainTopSprite;
    [System.NonSerialized]
    public CatMainDown CatMainDown;
    #endregion 

    private CatTopMain catTopMain;
    private  RectTransform UiMask;

    public bool isopenDiamondAdd = false;
    private int UiFormNumber = 0;//记录当前打开界面的Id次序
    private CatClase[] CatClase;
    private GameObject BackBtn;
    private Text tilText;

    private int mStayTimeQueue = -1;

    [System.NonSerialized]
    public GameObject NewPosPrompt, GiftFromAnimalButton, ButtonListe;

    private RectTransform Top;
    private ScrollRect ScenesView;
    private bool GuidStupNumCanAdd = false;
    private void Awake()
    {
        UiMask = transform.Find("UIMask").GetComponent<RectTransform>();
        BackBtn = transform.Find("Top/BG/BackButton").gameObject;
        UIEventListener.AddOnClickListener(BackBtn, OnBackBtnClick);

        NewPosPrompt = transform.Find("Top/NewPosPrompt").gameObject;
        GiftFromAnimalButton = transform.Find("ButtonListe/GiftFromAnimalButton").gameObject;
        ButtonListe = transform.Find("ButtonListe").gameObject;

        Top = transform.Find("Top").GetComponent<RectTransform>();
        ScenesView = transform.Find("ScenesView").GetComponent<ScrollRect>();
    }

    private void OnBackBtnClick(PointerEventData eventData)
    {
        catSceneView.ResetdecorationPosPrefab();
        SetBackBtnState(false);
    }

    public override void OnOpen()
    {
        base.OnOpen();
     
        UiFormIndex = (int)CatFormEnum.CAT_MAIN;
       
        UserDataManager.Instance.UiNumberInit();
        UserDataManager.Instance.IsCatUiOpent = true;//标记猫的界面开启

        if (UiMask!=null)
        {
            ScreneWidth = UiMask.rect.width;
            ScreneHeight = UiMask.rect.height;
        }

        //tilText.text = GameDataMgr.Instance.table.GetLocalizationById(92).text.ToString() + " " + MyBooksDisINSTANCE.Instance.GetName();

        EventDispatcher.AddMessageListener(EventEnum.CloseUiFormDist.ToString(), CloseUiFormDist);

        MainTopSprite = CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop);
        MainTopSprite.gameObject.SetActive(false);
        CatClase = gameObject.GetComponentsInChildren<CatClase>();
        foreach (var e in CatClase)
        {
            e.Bind(this);
        }

        addMessageListener(EventEnum.isopenDiamondAddDith.ToString(),isopenDiamondAddDith);
        addMessageListener(EventEnum.CacularCatFormStayTime, CacularCatFormStayTimeHandler);
        addMessageListener(EventEnum.CatRobotInfoUpdate, CatRobotUpdateHandler);
        addMessageListener(EventEnum.ChangeCatFormBackBtnState, ChangeBackBtnStateHandler);
        addMessageListener(EventEnum.CatGuidUiClose, CatGuidUiClose);
        addMessageListener(EventEnum.OpenCatGuid, OpenCatGuid);
        addMessageListener(EventEnum.SetGuidPos, SetGuidPos);



        SetBackBtnState(false);

        EventDispatcher.Dispatch(EventEnum.CacularCatFormStayTime, true);

        //判断是否有故事          
        GameHttpNet.Instance.Getpetmsg(GetpetmsgCallBacke);


        if (ResolutionAdapter.HasUnSafeArea)
        {
            var scale = this.myForm.GetScale();
            var safeArea = ResolutionAdapter.GetSafeArea();
            var offset = this.myForm.Pixel2View(safeArea.position);
            var offerH = offset.y;

            Top.anchoredPosition = new Vector2(0, -50-offerH);
        }

        //OpenCatGuid(null);

        CUIManager.Instance.CloseForm(UIFormName.MainDown);

    }

    private void CatRobotUpdateHandler(Notification noti)
    {
        CatRobotSpr.UpdateRobotInfo();
    }

    private void ChangeBackBtnStateHandler(Notification noti)
    {
        bool state = (bool)noti.Data;
        SetBackBtnState(state);
    }

    private void CacularCatFormStayTimeHandler(Notification vNoti)
    {
        bool isStart = (bool)vNoti.Data;
        //LOG.Error("=========CacularCatFormStayTimeHandler=========>" + isStart);
        CatRobotSpr.gameObject.SetActive(isStart && UserDataManager.Instance.SceneInfo.data.robot != null && UserDataManager.Instance.SceneInfo.data.robot.Count > 0);
        if (isStart)
        {
            if (mStayTimeQueue == -1)
            {
                int cacularTime = 10 * 60 * 1000;
                mStayTimeQueue = CTimerManager.Instance.AddTimer(cacularTime, -1, StayTimeInfoUpdateHandler);
            }
        }else
        {
            if (mStayTimeQueue != -1)
                CTimerManager.Instance.RemoveTimer(mStayTimeQueue);

            mStayTimeQueue = -1;
        }
    }

    private void StayTimeInfoUpdateHandler(int vQueue)
    {

        if (UserDataManager.Instance.isFirstCatEnt)
        {
            //在引导没有完成的时候，10分钟计时完成不给刷新界面
            return;
        }

        //LOG.Error("--------------------->" + vQueue);
        CTimerManager.Instance.RemoveTimer(mStayTimeQueue);
        int closeFormId = UserDataManager.Instance.GetCatFormListTop();
        if (closeFormId == (int)CatFormEnum.CAT_MAIN)
        {
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.PostGetSceneInfo(ProcessGetSceneInfo);
        }
    }

    private void ProcessGetSceneInfo(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----ProcessGetSceneInfo---->" + result);

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {

                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    int closeFormId = UserDataManager.Instance.GetCatFormListTop();
                    if (closeFormId > 0)
                        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), closeFormId);

                    UserDataManager.Instance.SceneInfo = JsonHelper.JsonToObject<HttpInfoReturn<SceneInfo>>(result);
                    GameDataMgr.Instance.table.GetCatInMapData();
                    CUIManager.Instance.OpenForm(UIFormName.CatLoading);
                }
            }, null);
        }
    }

    private void isopenDiamondAddDith(Notification notification)
    {
        int type = (int)notification.Data;

        if (type==1)
        {
            isopenDiamondAdd = true;
        }
        else
        {
            isopenDiamondAdd = false;
        }
    }

    public override void CustomUpdate()
    {
        base.CustomUpdate();

#if UNITY_ANDROID
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape)&& !isopenDiamondAdd)
#else
        if (Input.GetKeyDown(KeyCode.End)&& !isopenDiamondAdd)
#endif
        {
            int closeFormId = UserDataManager.Instance.GetCatFormListTop();
            if(closeFormId > 0)
                EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), closeFormId);
        }
#endif
    }

    public void SetBackBtnState(bool v)
    {
        BackBtn.gameObject.SetActive(v);
    }

    private void GushiCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GushiCallBack---->" + result);

    }
    private void CloseUiFormDist(Notification notification)
    {
        
        if (1 == UiFormIndex)
        {
            //每次来到主界面的时候，都要打开底下的选项

            //Debug.Log("dddddd");
            CatMainDown.DwonButtonShow();

            //判断是否有故事          
            GameHttpNet.Instance.Getpetmsg(GetpetmsgCallBacke);
        }
    }

    private void GetpetmsgCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetpetmsgCallBacke---->" + result);

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.Getpetmsg = JsonHelper.JsonToObject<HttpInfoReturn<Getpetmsg>>(result);

                    int num = UserDataManager.Instance.Getpetmsg.data.ispetstory;
                    CatMainDown.IsHadStory(num);
                }
            }, null);
        }
    }

   
    public override void OnClose()
    {
        base.OnClose();

        CUIManager.Instance.OpenForm(UIFormName.MainDown);

        MainTopSprite.gameObject.SetActive(true);
        foreach (var e in CatClase)
        {
            e.CloseUi();
        }

        if (mStayTimeQueue != -1)
            CTimerManager.Instance.RemoveTimer(mStayTimeQueue);

        UIEventListener.RemoveOnClickListener(BackBtn, OnBackBtnClick);

        EventDispatcher.RemoveMessageListener(EventEnum.CloseUiFormDist.ToString(), CloseUiFormDist);

        UserDataManager.Instance.IsCatUiOpent = false;//标记猫的界面开启

    }

    #region 猫的引导界面的关闭与打开

    /// <summary>
    /// 打开猫的引导界面
    /// </summary>
    public void OpenCatGuid(Notification notice)
    {
        LOG.Info("当前引导步骤是：" + UserDataManager.Instance.GuidStupNum);
        if (UserDataManager.Instance.GuidStupNum > 0 && UserDataManager.Instance.GuidStupNum < (int)CatGuidEnum.CatGuidEnd)
        {            
            //LOG.Info("打开引导界面，步骤是："+ UserDataManager.Instance.GuidStupNum);
            if (CUIManager.Instance.GetForm<CatGuid>(UIFormName.CatGuid)==null)
            {
                CUIManager.Instance.OpenForm(UIFormName.CatGuid);
                LOG.Info("打开引导界面");

                
            }

            if (UserDataManager.Instance.isFirstCatEnt)
            {
                ScenesView.enabled = false;
            }

            GuidStupNumCanAdd = false;

            EventDispatcher.Dispatch(EventEnum.DoGuidStep, UserDataManager.Instance.GuidStupNum);
        }
        else
        {
            //Debug.Log("猫引导结束");
        }

    }
    /// <summary>
    /// 这个是关闭猫引导，派发事件调用这里处理相关的引导逻辑
    /// </summary>
    /// <param name="notice"></param>
    public void CatGuidUiClose(Notification notice)
    {
        EventDispatcher.Dispatch(EventEnum.CatGuidCanvasGroupOFF);

        if (!GuidStupNumCanAdd)
        {
            GuidStupNumCanAdd = true;
            UserDataManager.Instance.GuidStupNum += 1;
        }
        
        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.DecorationsButtonOn)
        {
            UserDataManager.Instance.GuidStupNum = (int)CatGuidEnum.PlaceHuangyuandian;
        }
        if (UserDataManager.Instance.GuidStupNum== (int)CatGuidEnum.CatGuidEnd)
        {
            UserDataManager.Instance.GuidStupNum = 98;
        }
        GameHttpNet.Instance.UserpetguideChange(UserDataManager.Instance.GuidStupNum, UserpetguideChangeCall);
    }

    private void OpenCatGuidInvoke()
    {
        CancelInvoke("OpenCatGuidInvoke");
        OpenCatGuid(null);
    }

    /// <summary>
    /// 设置引导点击的位置
    /// </summary>
    /// <param name="notice"></param>
    public void SetGuidPos(Notification notice)
    {
        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.GetGiftGuid)
        {
            //这里获得礼物按钮的位置

            RectTransform ButtonListeRect = ButtonListe.GetComponent<RectTransform>();
            RectTransform GiftFromAnimalButtonRect = GiftFromAnimalButton.GetComponent<RectTransform>();

            float H1 = ButtonListeRect.anchoredPosition.y;
            float H2 = GiftFromAnimalButtonRect.anchoredPosition.y;
            float W1= ButtonListeRect.anchoredPosition.x;
            float W2= GiftFromAnimalButtonRect.anchoredPosition.x;

            float Posx = ScreneWidth-17-50 /*+ W1 - W2*/;
            float Posy = ScreneHeight-115-184.5f /*+ H1 + H2*/;

            UserDataManager.Instance.GuidPos = new Vector3(Posx, Posy, 1);
           
        }
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

                    LOG.Info("保存猫引导步骤成功，步骤是："+UserDataManager.Instance.GuidStupNum);

                    
                    if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.CatGetFoodTips || UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.CatFoodOnGuid || UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.PlaceDecorationsTips || UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.SpwanCat)
                    {
                        //这是点击了文字提示后，进入指引适合
                        Invoke("OpenCatGuidInvoke", 0.3f);
                    }
                    else if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.CatCountdown|| UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.FeedbackTips)
                    {
                       
                        Invoke("OpenCatGuidInvoke", 0.3f);
                    }else if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.GetGiftGuid)
                    {
                        GiftFromAnimalButton.SetActive(true);

                        Invoke("OpenCatGuidInvoke", 0.3f);
                    }
                    else if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.CatGuidEnd|| UserDataManager.Instance.GuidStupNum ==98)
                    {
                        LOG.Info("猫引导结束");
                        CUIManager.Instance.CloseForm(UIFormName.CatGuid);

                        UserDataManager.Instance.GuidFirstStupNum = (int)CatGuidEnum.CatGuidEnd;
                        UserDataManager.Instance.GuidStupNum = (int)CatGuidEnum.CatGuidEnd;

                        UserDataManager.Instance.isFirstCatEnt = false;

                        ScenesView.enabled = true;
                    }
                }

            }, null);
        }
    }

    #endregion

}

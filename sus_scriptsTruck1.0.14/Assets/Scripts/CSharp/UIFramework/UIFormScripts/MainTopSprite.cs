﻿using System.Collections;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;


public class MainTopSprite : BaseUIForm
{
    public GameObject AddkeyLight, AddDiamondLight;
    public GameObject KeyAddButton;
    public Text KeyNumText;
    public GameObject DiamondAddButton;
    public Text DiamondNumText;
    public RectTransform RewardEffect;
    public GameObject AwardPos, keyMovePos, DiamondPos;
    public Text La_NumberHeart; //显示倒计时时间文字的文本  
    public GameObject TimeButton;
    public Image imgDiamond;
    public Image imgKey;
    public Image LogoIcon;
    public Button KeyDetailBtn, DiamondDetailBtn;
    public GameObject KeyDetail, DiamondDetail;
    public Button DetailMask;
    public GameObject RedImg;
    public GameObject CloseIcon, Log;

    private int TimeSequence = 0;
    private RectTransform TopBar;
    public override void OnOpen()
    {
        base.OnOpen();

        CtrlIconShow(false);
        Log.SetActive(true);
        UIEventListener.AddOnClickListener(KeyAddButton, OpenChargeMoney_Keys);
        UIEventListener.AddOnClickListener(DiamondAddButton, OpenChargeMoney_Diamonds);
        KeyDetailBtn.onClick.AddListener(ShowKeyDetail);
        DiamondDetailBtn.onClick.AddListener(ShowDiamondDetail);
        DetailMask.onClick.AddListener(DetailMaskOnClick);
        EventDispatcher.AddMessageListener(EventEnum.GotoMall, OpenMall);

        UIEventListener.AddOnClickListener(CloseIcon, CloseIconOnclicke);
        UIEventListener.AddOnClickListener(LogoIcon.gameObject, OnLogoIconClick);


        addMessageListener(EventEnum.OnKeyNumChange.ToString(), OnKeyNumChange);
        addMessageListener(EventEnum.OnDiamondNumChange, OnDiamondNumChange);
        addMessageListener(EventEnum.GetRewardShow, GetRewardHandler);
     

        KeyNumText.text = UserDataManager.Instance.UserData.KeyNum.ToString();
        DiamondNumText.text = UserDataManager.Instance.UserData.DiamondNum.ToString();

        RewardEffect.gameObject.SetActive(false);
     

        TopBar = transform.Find("Canvas/TopBar").GetComponent<RectTransform>();

       

        TimeButton.SetActive(false);

        float FirstHeight = TopBar.rect.height;

        //【屏幕适配】
        XLuaHelper.UnSafeAreaNotFit(this.myForm, TopBar,750, FirstHeight);


        GetTopHight();

        //判断是否有倒计时，如果有，先移除
        StopCountDown();

        //检查免费钥匙的状态
        CheckFreeKeyState();
    }

    private void OnLogoIconClick(PointerEventData eventData)
    {
        XLuaManager.Instance.GetLuaEnv().DoString(@"GameHelper.OpenMainDownToggle(MainDown.ProfileToggle);");
    }


    private void OpenMall(Notification obj)
    {
        OpenChargeMoney_Diamonds(null);
    }

    private void GetTopHight()
    {
        float Height=TopBar.rect.height;
        UserDataManager.Instance.TopHight = Height;
      
    }


    //当前界面
    private string CurUI = "";

    /// <summary>
    /// 这个是游戏过程之中，点击了顶部的钻石按钮，调用
    /// </summary>
    public void GamePlayTopOpen(string _CurUI)
    {
        CtrlIconShow(true);
        CurUI = _CurUI;
    }
    public void SetRedImage(bool show)
    {
        RedImg.SetActiveEx(show);
    }
    public void CtrlIconShow(bool value)
    {
        CurUI = "";
        CloseIcon.SetActiveEx(value);
        LogoIcon.gameObject.SetActiveEx(!value);
        //LogoIcon.gameObject.SetActiveEx(false);
        if (!value)
        {
            LogoIcon.DOKill();

            LogoIcon.DOFade(0, 0).SetEase(Ease.Flash).Play();
            LogoIcon.DOFade(1, 0.3f).SetEase(Ease.Flash).Play();
        }
    }


    public void CloseIconOnclicke(PointerEventData data)
    {

        if (CurUI == "UIChargeMoneyForm")
        {
            CtrlIconShow(false);
            CUIManager.Instance.CloseForm(UIFormName.ChargeMoneyForm);
            CUIManager.Instance.CloseForm(UIFormName.MainFormTop);
        }
        else if (CurUI == "UIRankForm")
        {
            CtrlIconShow(false);
            XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:Close(logic.uiid.UIRankForm);");
        }
        else if (CurUI == "UILotteryForm")
        {
            CtrlIconShow(false);
            XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:Close(logic.uiid.UILotteryForm);");
        }
        else if (CurUI == "UIDressUpForm")
        {
            CtrlIconShow(false);
            XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:Close(logic.uiid.UIDressUpForm);");
        }
        else if (CurUI == "UIMasForm")
        {
            CtrlIconShow(false);
            XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:Close(logic.uiid.UIMasForm);");
        }
        else if (CurUI == "UICommunityForm")
        {
            CtrlIconShow(false);
            XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:Close(logic.uiid.UICommunityForm);");
			CloseAllUI();
        }
        else if (CurUI == "UIChargeMoneyForm1")
        {
            CtrlIconShow(false);
            CUIManager.Instance.CloseForm(UIFormName.ChargeMoneyForm);
        }
        else
        {
            CtrlIconShow(false);
        }
    }


    private void CloseAllUI()
    {
        XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:Close(logic.uiid.UIMasForm);");
        XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:Close(logic.uiid.UIRankForm);");
        XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:Close(logic.uiid.UIDressUpForm);");
        XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:Close(logic.uiid.UICommunityForm);");
        XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:Close(logic.uiid.UILotteryForm);");
    }

    /// <summary>
    /// 顶部栏显示出来，
    /// </summary>
    public void MainTopTrue()
    {      
        //gameObject.SetActive(true);
    }
    /// <summary>
    /// 顶部栏隐藏
    /// </summary>
    public void MainTopFalse()
    {      
        //gameObject.SetActive(false);
    }


    //检查免费钥匙的状态
    private void CheckFreeKeyState()
    {
        if (UserDataManager.Instance.userInfo != null)
        {
            int end_time = UserDataManager.Instance.userInfo.data.userinfo.end_time;
            if (end_time > 0)    //表示免费钥匙倒计时未到，显示倒计时
            {
                if (GameDataMgr.Instance.GetCurrentUTCTime() < end_time)
                    StartCountDown();
                else
                    GetFreeKey();
            }
            else
            {
                //检查当前用户的钥匙数是否小于2个
                CheckKeysLessThan2();
            }
        }
    }
    
    //获取免费钥匙
    private void GetFreeKey()
    {
        GameHttpNet.Instance.GetFreeKey(GetFreeKeCallyBack);
    }

    //领取免费钥匙回调
    private void GetFreeKeCallyBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetFreeKeCallyBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);

        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    if (UserDataManager.Instance.FreeKeyApplyInfo != null
                        &&UserDataManager.Instance.FreeKeyApplyInfo.data != null)
                    {
                        UserDataManager.Instance.userInfo.data.userinfo.free_key -= 1;
                    }
                    UserDataManager.Instance.freeKeyInfo = JsonHelper.JsonToObject<HttpInfoReturn<FreeKeyInfo>>(result);
                    int award = UserDataManager.Instance.freeKeyInfo.data.bKey - UserDataManager.Instance.UserData.KeyNum;
                    if (award > 0)
                        TalkingDataManager.Instance.OnReward(award, "Free key");

                    UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.freeKeyInfo.data.bKey);
                    UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.freeKeyInfo.data.diamond);

                    if(UserDataManager.Instance.userInfo.data != null)
                        UserDataManager.Instance.userInfo.data.userinfo.end_time = -1;

                    CheckKeysLessThan2();
                }
                else if(jo.code == 204)
                {
                    UserDataManager.Instance.freeKeyInfo = JsonHelper.JsonToObject<HttpInfoReturn<FreeKeyInfo>>(result);
                    if(UserDataManager.Instance.freeKeyInfo != null)
                    {
                        UserDataManager.Instance.userInfo.data.userinfo.end_time = UserDataManager.Instance.freeKeyInfo.data.end_time;
                        CheckFreeKeyState();
                    }
                }

            }, null);

        }
    }


    //开启定时器
    private void StartCountDown()
    {
        TimeSequence = CTimerManager.Instance.AddTimer(1000, 0, ShowFreeKeyCountDown);
    }

    //计算剩余时间，并显示倒计时
    private void ShowFreeKeyCountDown(int timeSeq)
    {
        if (UserDataManager.Instance.userInfo.data != null && UserDataManager.Instance.userInfo.data.userinfo != null)
        {
            int leftTime = UserDataManager.Instance.userInfo.data.userinfo.end_time - GameDataMgr.Instance.GetCurrentUTCTime();
            if (leftTime >= 0)
            {
                TimeDownText(leftTime);
            }
            else
            {
                StopCountDown();
                GetFreeKey();
            }
        }
    }

    //停止倒计时定时器
    private void StopCountDown()
    {
        if (TimeSequence > 0)
        {
            CTimerManager.Instance.RemoveTimer(TimeSequence);
            TimeSequence = -1;
        }

        if (TimeButton.activeSelf)
            TimeButton.SetActive(false);
    }

    private void TimeDownText(int endTime)
    {
        if (KeyAddButton==null|| DiamondAddButton==null)
        {
            return;
        }

        if (KeyAddButton.activeSelf && DiamondAddButton.activeSelf)
        {
            if(!TimeButton.activeSelf)
                TimeButton.SetActive(true);
            if (endTime<=0 && TimeButton.activeSelf)
            {
                La_NumberHeart.text ="00:00:00";
                TimeButton.SetActive(false);
                return;
            }
            if (TimeButton.activeSelf)
                La_NumberHeart.text = string.Format("{0:d2}:{1:d2}:{2:d2}", endTime / 3600, (endTime / 60) % 60, endTime % 60);
        }
        else
        {
            if (TimeButton.activeSelf)
                TimeButton.SetActive(false);
        }
    }


    //检查当前用户的钥匙数是否小于2个
    private void CheckKeysLessThan2()
    {
        if (UserDataManager.Instance.userInfo != null)
        {
            //如果用户的钥匙小于2个，且没有下个钥匙的目标时间的话，则向服务器申请2小时免费钥匙
            if (UserDataManager.Instance.UserData.KeyNum < 2 &&
               UserDataManager.Instance.userInfo.data.userinfo.end_time <= 0
               && UserDataManager.Instance.userInfo.data.userinfo.free_key > 0)
            {
                //申请2小时免费钥匙
                GameHttpNet.Instance.FreeKeyApply(FreeKeyApplyCallback);
            }
            else
            {
                StopCountDown();
            }
        }
    }
    //免费钥匙申请回调
    public void FreeKeyApplyCallback(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----FreeKeyApplyCallback---->" + result);
        LoomUtil.QueueOnMainThread((param) =>
        {
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo.code == 200)
            {
                UserDataManager.Instance.FreeKeyApplyInfo = JsonHelper.JsonToObject<HttpInfoReturn<FreeKeyApply>>(result);
                if(UserDataManager.Instance.FreeKeyApplyInfo != null && UserDataManager.Instance.FreeKeyApplyInfo.data !=null)
                    UserDataManager.Instance.userInfo.data.userinfo.end_time = UserDataManager.Instance.FreeKeyApplyInfo.data.end_time;

                CheckFreeKeyState();
            }
            else if (jo.code == 202)
            {
                UserDataManager.Instance.userInfo.data.userinfo.end_time = -1;
            }
        }, null);

    }



    public void GetimpinfoTochange()
    {
        //红点协议
        GameHttpNet.Instance.ActiveInfo(GetimpinfoCallBacke);
    }

    public void GetimpinfoCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetimpinfoCallBacke---->" + result);
        LoomUtil.QueueOnMainThread((param) =>
        {
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo.code == 200)
            {
                UserDataManager.Instance.lotteryDrawInfo = JsonHelper.JsonToObject<HttpInfoReturn<ActiveInfo>>(arg.ToString());

                if (UserDataManager.Instance.lotteryDrawInfo != null && UserDataManager.Instance.lotteryDrawInfo.data != null)
                {
                    //int EmailAwarNumber = UserDataManager.Instance.selfBookInfo.data.unreceivemsgcount;
                    int Emailshu = UserDataManager.Instance.selfBookInfo.data.unreadmsgcount;
                    int Newshu = 0;// UserDataManager.Instance.lotteryDrawInfo.data.unreadnewscount;
                    int CountNumber = Emailshu;
                    EventDispatcher.Dispatch(EventEnum.EmailNumberShow.ToString(), CountNumber);                                 
                }
            }
        }, null);
    }
    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(KeyAddButton, OpenChargeMoney_Keys);
        UIEventListener.RemoveOnClickListener(DiamondAddButton, OpenChargeMoney_Diamonds);
        KeyDetailBtn.onClick.RemoveListener(ShowKeyDetail);
        DiamondDetailBtn.onClick.RemoveListener(ShowDiamondDetail);

        UIEventListener.RemoveOnClickListener(CloseIcon, CloseIconOnclicke);
        UIEventListener.RemoveOnClickListener(LogoIcon.gameObject, OnLogoIconClick);
        LogoIcon.DOKill();

    }

    /// <summary>
    /// 倒计时结束的时候刷新显示
    /// </summary>
    public void CountDownShowNume()
    {
        KeyNumText.text = UserDataManager.Instance.UserData.KeyNum.ToString();
        DiamondNumText.text = UserDataManager.Instance.UserData.DiamondNum.ToString();
    }
    
    /// <summary>
    /// mask
    /// </summary>
    public void DetailMaskOnClick()
    {
        KeyDetail.SetActiveEx(false);
        DiamondDetail.SetActiveEx(false);
        DetailMask.gameObject.SetActiveEx(false);
    }
    
    /// <summary>
    /// 展示钥匙详情
    /// </summary>
    public void ShowKeyDetail()
    {
        var active = !KeyDetail.activeInHierarchy;
        KeyDetail.SetActiveEx(active);
        DetailMask.gameObject.SetActiveEx(active);
        DiamondDetail.SetActiveEx(false);
    }
    
    /// <summary>
    /// 展示钻石详情
    /// </summary>
    public void ShowDiamondDetail()
    {
        var active = !DiamondDetail.activeInHierarchy;
        DiamondDetail.SetActiveEx(active);
        DetailMask.gameObject.SetActiveEx(active);
        KeyDetail.SetActiveEx(false);
    }
    
    /// <summary>
    /// 这个是主角界面点击充值弹出充值界面，需要key按钮和钻石按钮同时存在，而且该界面此时没有在游戏进行中打开
    /// </summary>
    /// <param name="data"></param>
    public void OpenChargeMoney_Keys(PointerEventData data)
    {
        EventDispatcher.Dispatch(EventEnum.HideWebView);
        AddkeyLight.SetActive(true);
        Invoke("FXkeyAndDiondFalse", 0.3f);
      
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);

        if(CUIManager.Instance.GetForm<ChargeMoneyForm>(UIFormName.ChargeMoneyForm)==null)
           CUIManager.Instance.OpenForm(UIFormName.ChargeMoneyForm);

        CUIManager.Instance.GetForm<ChargeMoneyForm>(UIFormName.ChargeMoneyForm).SetFormStyle(1);

        Log.SetActive(false);

        if (CurUI != "UIChargeMoneyForm")
        {
            GamePlayTopOpen("UIChargeMoneyForm1");
        }

        CheckFreeKeyState();
        //关闭之前界面（禁止套娃）
        this.CloseAllUI();
    }


    /// <summary>
    /// 这个是主角界面点击充值弹出充值界面，需要key按钮和钻石按钮同时存在，而且该界面此时没有在游戏进行中打开
    /// </summary>
    /// <param name="data"></param>
    public void OpenChargeMoney_Diamonds(PointerEventData data)
    {
        EventDispatcher.Dispatch(EventEnum.HideWebView);
        AddDiamondLight.SetActive(true);
        Invoke("FXkeyAndDiondFalse", 0.3f);
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);

        if (CUIManager.Instance.GetForm<ChargeMoneyForm>(UIFormName.ChargeMoneyForm) == null)
            CUIManager.Instance.OpenForm(UIFormName.ChargeMoneyForm);

        CUIManager.Instance.GetForm<ChargeMoneyForm>(UIFormName.ChargeMoneyForm).SetFormStyle(2);

        Log.SetActive(false);
        if (CurUI != "UIChargeMoneyForm")
        {
            GamePlayTopOpen("UIChargeMoneyForm1");
        }
        //关闭之前界面（禁止套娃）
        this.CloseAllUI();
    }

  
    private void OnKeyNumChange(Notification notification)
    {
        //KeyNumText.text = ((int)notification.Data).ToString();
        //Debug.Log("钥匙刷新执行了");

        int newNum = (int)notification.Data;
        int oldNum = int.Parse(KeyNumText.text);

        //var chargeUI = CUIManager.Instance.GetForm<ChargeMoneyForm>(UIFormName.ChargeMoneyForm);
        if (string.IsNullOrEmpty(notification.Type))
        {
            if(newNum >= oldNum)
            {
                UITween.AddDiamond(imgKey, KeyNumText, oldNum, newNum);
            }
            else
            {
                //KeyNumText.text = (newNum).ToString();

                DOTween.To(() => oldNum, (value) => { KeyNumText.text = value.ToString(); }, newNum, 2).OnComplete(() => { });
            }
        }else
        {
            //KeyNumText.text = (newNum).ToString();

            DOTween.To(() => oldNum, (value) => { KeyNumText.text = value.ToString(); }, newNum, 2).OnComplete(() => { });
        }
    }

    private void OnDiamondNumChange(Notification notification)
    {
        //Debug.Log("钻石刷新执行了-1");
        if (UserDataManager.Instance.IsCatUiOpent)
        {
            //猫的ui开启中，不能执行这里
            int Num = (int)notification.Data;
            DiamondNumText.text = (Num).ToString();

            return;
        }

        int newNum = (int)notification.Data;
        int oldNum = int.Parse(DiamondNumText.text);

        //var chargeUI = CUIManager.Instance.GetForm<ChargeMoneyForm>(UIFormName.ChargeMoneyForm);
        if (string.IsNullOrEmpty(notification.Type))
        {
            //Debug.Log("钻石刷新执行了0");
            if (newNum >= oldNum)
            {
                UITween.AddDiamond(imgDiamond, DiamondNumText, oldNum, newNum);
            }
            else
            {
                //DiamondNumText.text = (newNum).ToString();

                DOTween.To(() => oldNum, (value) => { DiamondNumText.text = value.ToString(); }, newNum, 2).OnComplete(() => {

                });
            }
        }
        else
        {
            //Debug.Log("钻石刷新执行了1");

            DOTween.To(() => oldNum, (value) => { DiamondNumText.text = value.ToString(); }, newNum, 2).OnComplete(() => {

            });
        }
        //Debug.Log("钻石刷新执行了2");
    }

  


    //private int OpenTheGameType = 0;
    //这个是在游戏中打开了充值界面的时候，对Top条框进行设置
    public void GamePlayOnpenDiamond(int OpenType)
    {
        GameOpenDiamondInit();
    }

    //这个是在游戏中打开了充值Key界面的时候，对top条框进行设置
    public void GamePlayOnpenKey(int OpenType)
    {
        GameOpenKeyInit();
    }


    //这个是游戏打开Key充值界面，对Top条框的初始化
    private void GameOpenKeyInit()
    {
        DiamondAddButton.SetActive(false);
        KeyAddButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(-305, 0, 0);
       
    }

    //这个是游戏打开钻石/key充值界面，对top条框的初始化
    private void GameOpenDiamondInit()
    {
        KeyAddButton.SetActive(false);
        DiamondAddButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(-305, 0, 0);
       
    }


    private void GetRewardHandler(Notification notification)
    {
        RewardEffect.gameObject.SetActive(false);
        RewardShowData showData = notification.Data as RewardShowData;
        if (showData != null)
        {
            Vector3 startPos = Camera.main.ScreenToWorldPoint(showData.StartPos);
            if (showData.IsInputPos)
            {
                startPos.x -= Screen.width / 2;
                startPos.y -= Screen.height / 2;
            }
            else
            {
                startPos = showData.StartPos;
            }


            RewardEffect.localPosition = startPos;
            RewardEffect.gameObject.SetActive(true);
            RewardEffect.DOAnchorPos(showData.TargetPos, 1f).OnComplete(() =>
            {
                RewardEffect.gameObject.SetActive(false);
                RewardEffect.gameObject.SetActive(true);

                LoomUtil.QueueOnMainThread((param) =>
                {
                    int award = showData.DiamondNum - UserDataManager.Instance.UserData.DiamondNum;
                    if (award > 0)
                        TalkingDataManager.Instance.OnReward(award, "Watch the ads diamond");

                    UserDataManager.Instance.ResetMoney(1, showData.KeyNum);
                    UserDataManager.Instance.ResetMoney(2, showData.DiamondNum);
                    UserDataManager.Instance.ResetMoney(3, showData.TicketNum);
                }, null);

            }).SetDelay(0.3f).Play();
        }
    }

   
  
}

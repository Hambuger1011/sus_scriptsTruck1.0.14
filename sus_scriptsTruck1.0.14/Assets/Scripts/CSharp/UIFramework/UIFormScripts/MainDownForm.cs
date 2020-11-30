using System;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class MainDownForm : BaseUIForm
{
    private enum MainDownButtonType
    {
        Library,
        Category,
        Brief,
        Inbox,
        Gift,

        Maxt
    }

    private TabItem[] tabItems;
    //private GameObject Library, Category, Brief, Inbox, Gift;
    private GameObject Hit;
    private Text EmailNumber;
    private RectTransform BG;

    public class TabItem
    {
        public Transform transform;
        public GameObject gameObject;
        public string uiid;
        public CUIForm uiform;

        private GameObject off, on;
        private Transform On_text;
        private bool m_lastOn = false;

        public TabItem(Transform transform, string uiid)
        {
            this.transform = transform;
            this.uiid = uiid;
            this.gameObject = transform.gameObject;
            this.off = this.transform.Find("Off").gameObject;
            this.on = this.transform.Find("ON").gameObject;
            this.On_text = this.transform.Find("ON/Text");

            this.on.SetActiveEx(false);
            this.off.SetActiveEx(true);
            this.On_text.transform.localPosition = new Vector3(0, -46, 0);
            if (uiid != UIFormName.Ofertas)
            {
                uiform = CUIManager.Instance.GetForm(uiid);
                if(uiform != null)
                {
                    uiform.Hide();
                }
            }
        }

        public void SetOn(bool isOn)
        {
            if(this.m_lastOn == isOn)
            {
                return;
            }
            this.on.SetActiveEx(isOn);
            this.off.SetActiveEx(!isOn);

            if (isOn)
            {
                if (uiid != UIFormName.Ofertas)
                {
                    if (uiform == null)
                    {
                        uiform = CUIManager.Instance.OpenForm(uiid);
                    }
                    uiform.Appear();
                }
                else
                {
                    var luaenv = XLuaManager.Instance.GetLuaEnv();
                    var res = luaenv.DoString(@"uiLuaFile:Creat()");
                }
            }
            else
            {
                if (uiid != UIFormName.Ofertas)
                {
                    if (uiform != null)
                    {
                        uiform.Hide();
                    }
                }
                else
                {
                    var luaenv = XLuaManager.Instance.GetLuaEnv();
                    var res = luaenv.DoString(@"uiLuaFile:Destroy()");
                }
            }

            if (m_lastOn &&  !isOn)
            {
                DOTween.Kill(this.off);
                this.off.transform.localPosition = new Vector3(0, 12, 0);
                this.off.transform.DOLocalMoveY(2, 0.3f).SetEase(Ease.Flash).Play();
            }
            if (!m_lastOn && isOn)
            {
                DOTween.Kill(this.On_text);
                this.On_text.localPosition = new Vector3(0, -75, 0);
                this.On_text.transform.DOLocalMoveY(-46, 0.2f).SetEase(Ease.Flash).SetId(this.On_text);
            }
            this.m_lastOn = isOn;
        }

        public void Close()
        {
            if(this.uiform != null)
            {
                this.uiform.Close();
            }
        }
    }

    public override void OnOpen()
    {
        base.OnOpen();

        if(tabItems == null)
        {
            // tabItems = new TabItem[(int)MainDownButtonType.Maxt];
            // tabItems[0] = new TabItem(transform.Find("Canvas/BG/Library"), UIFormName.MainForm);
            // tabItems[1] = new TabItem(transform.Find("Canvas/BG/Category"), UIFormName.TypeSelection);
            // tabItems[2] = new TabItem(transform.Find("Canvas/BG/Brief"), UIFormName.ProfileForm);
            // tabItems[3] = new TabItem(transform.Find("Canvas/BG/Inbox"), UIFormName.Comuniada);
            // tabItems[4] = new TabItem(transform.Find("Canvas/BG/Gift"), UIFormName.Ofertas);
            // //新签到界面的Lua入口
            // var luaenv = XLuaManager.Instance.GetLuaEnv();
            // var res = luaenv.DoString(@"uiLuaFile = logic.UIMgr:Creat('Logic/UI/UI_Activity/UIActivityAndNews')");
            //
            // UIEventListener.AddOnClickListener(tabItems[0].gameObject, LibraryButtonOn);
            // UIEventListener.AddOnClickListener(tabItems[1].gameObject, CategoryButtonOn);
            // UIEventListener.AddOnClickListener(tabItems[2].gameObject, BriefButtonOn);
            // UIEventListener.AddOnClickListener(tabItems[3].gameObject, InboxButtonOn);
            // UIEventListener.AddOnClickListener(tabItems[4].gameObject, GiftButtonOn);
            // EventDispatcher.AddMessageListener(EventEnum.GotoRead, OpenRead);
        }
       
        //ON[(int)MainDownButtonType.Library] = transform.Find("Canvas/BG/Library/ON").gameObject;
        //ON[(int)MainDownButtonType.Category] = transform.Find("Canvas/BG/Category/ON").gameObject;
        //ON[(int)MainDownButtonType.Brief] = transform.Find("Canvas/BG/Brief/ON").gameObject;
        //ON[(int)MainDownButtonType.Inbox] = transform.Find("Canvas/BG/Inbox/ON").gameObject;
        //ON[(int)MainDownButtonType.Gift] = transform.Find("Canvas/BG/Gift/ON").gameObject;

        //OFF[(int)MainDownButtonType.Library] = transform.Find("Canvas/BG/Library/Off").gameObject;
        //OFF[(int)MainDownButtonType.Category] = transform.Find("Canvas/BG/Category/Off").gameObject;
        //OFF[(int)MainDownButtonType.Brief] = transform.Find("Canvas/BG/Brief/Off").gameObject;
        //OFF[(int)MainDownButtonType.Inbox] = transform.Find("Canvas/BG/Inbox/Off").gameObject;
        //OFF[(int)MainDownButtonType.Gift] = transform.Find("Canvas/BG/Gift/Off").gameObject;


        //Library = transform.Find("Canvas/BG/Library").gameObject;
        //Category = transform.Find("Canvas/BG/Category").gameObject;
        //Brief = transform.Find("Canvas/BG/Brief").gameObject;
        //Inbox = transform.Find("Canvas/BG/Inbox").gameObject;
        //Gift = transform.Find("Canvas/BG/Gift").gameObject;

        Hit = transform.Find("Canvas/BG/Inbox/Off/Hit").gameObject;
        EmailNumber = transform.Find("Canvas/BG/Inbox/Off/Hit/EmailNuber").GetComponent<Text>();
        BG = transform.Find("Canvas/BG").GetComponent<RectTransform>();
        Hit.SetActive(false);


        addMessageListener(EventEnum.EmailNumberShow.ToString(), EmailNumberShow);
        addMessageListener(EventEnum.GotoActivity.ToString(), GotoActivity);

        ShowButtonType(0);

        //测试lua
        //Debug.Log("lua测试========");
        //var luaenv = XLuaManager.Instance.GetLuaEnv();
        //var res = luaenv.DoString(@"GameMain.MYmani()");
        //end
        
        if (ResolutionAdapter.HasUnSafeArea)
        {
            var scale = this.myForm.GetScale();
            var safeArea = ResolutionAdapter.GetSafeArea();
            var offset = this.myForm.Pixel2View(safeArea.position);
            BG.sizeDelta = new Vector2(750, offset.y + 100);

        }

        BG.DOKill();
        BG.anchoredPosition = new Vector2(0, -150);
        BG.DOAnchorPosY(0, 1f).SetEase(Ease.InOutFlash).Play();

        //GameHttpNet.Instance.GetSelfBookInfo(ToLoadSelfBookInfo)
    }

    private void OpenRead(Notification obj)
    {
        LibraryButtonOn(null);
    }

    public override void OnClose()
    {
        base.OnClose();

        removeMessageListeners();
        
        for(int i = 0; i < this.tabItems.Length; ++i)
        {
            this.tabItems[i].Close();
        }
        EventDispatcher.RemoveMessageListener(EventEnum.GotoRead, OpenRead);
    }

    private void EmailNumberShow(Notification notification)
    {
        int Number = (int)notification.Data;

        if (Number>0)
        {
            //Hit.SetActive(true);
            EmailNumber.text = Number.ToString();
        }else
        {
            //Hit.SetActive(false);
        }
    }
    
    private void GotoActivity(Notification notification)
    {
        ShowButtonType((int)MainDownButtonType.Gift);
    }
    private void LibraryButtonOn(PointerEventData data)
    {
        ShowButtonType((int)MainDownButtonType.Library);
    }

    private void CategoryButtonOn(PointerEventData data)
    {
        ShowButtonType((int)MainDownButtonType.Category);
    }

    private void BriefButtonOn(PointerEventData data)
    {
        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.CatButtonGuid)
        {
            EventDispatcher.Dispatch(EventEnum.CatGuidCanvasGroupOFF);  //隐藏猫引导界面
            UserDataManager.Instance.GuidStupNum += 1;

            GameHttpNet.Instance.UserpetguideChange(UserDataManager.Instance.GuidStupNum, UserpetguideChangeCall);
        }
        ShowButtonType((int)MainDownButtonType.Brief);

       
    }

    private void InboxButtonOn(PointerEventData data)
    {
        ShowButtonType((int)MainDownButtonType.Inbox);
    }

    private void GiftButtonOn(PointerEventData data)
    {
        ShowButtonType((int)MainDownButtonType.Gift);

        //这个是打开活动界面
        //MainTopSprite.UIOpent(7) 
        //UIOpenOrClose(UIFormName.Activity);
       
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
                 
                }

            }, null);
        }
    }

    /// <summary>
    /// 显示按钮的状态
    /// </summary>
    private void ShowButtonType(int Indx)
    {
        for (int i = 0; i < this.tabItems.Length; i++)
        {
            this.tabItems[i].SetOn(i == Indx);
        }
    }
    
}

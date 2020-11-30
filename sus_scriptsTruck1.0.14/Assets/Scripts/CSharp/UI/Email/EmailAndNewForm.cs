using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using pb;
using DG.Tweening;
using UGUI;

public class EmailAndNewForm : BaseUIForm
{
    //private Text EmailNumber, newNumber;
    //private GameObject EmailNumBg, newNumBg;
    //private GameObject EmailButton, NewButton;
    //private GameObject EmailOff, EmailOn, NewOff, NewOn;
    private GameObject ContactUsBtn;
    private List<GameObject> OffList, OnList;

    [System.NonSerialized]
    public ScrollRect EmailScrollView, NewScrollView;

    private GameObject EmailItem, NewItem;
    private GameObject CollectAll;
    private int EmailPage,NewPage;
   
    private List<NewEmailItemSprite> NewEmailItemSpriteList;
    private List<NewnewsItemSprite> NewnewsItemSpriteList;

    private Scrollbar EmailScrollbarVertical;
    private Scrollbar NewScrollbarVertical;
    private bool CloseUI = false;
    private int EmailItemNumber = 0;//这个是生成邮件物体的计数编号
    private float EmailItemHight = 280;//这个是conten下邮件物体的高度

    private GameObject CloseButton;
    private RectTransform TopTile;

    public override void OnOpen()
    {
        base.OnOpen();
        EmailItemHight = 280;
        CloseUI = false;
        EmailPage = 1;
        NewPage = 1;
        EmailPageTrun = true;
        NewPageTrun = true;

        if (OffList == null)
            OffList = new List<GameObject>();
        if (OnList == null)
            OnList = new List<GameObject>();

        //EmailNumber = transform.Find("Canvas/TopTile/EmailButton/NumBg/EmailNumber").GetComponent<Text>();
        //EmailNumBg = transform.Find("Canvas/TopTile/EmailButton/NumBg").gameObject;
        //newNumber = transform.Find("Canvas/TopTile/NewButton/NumBg/Text").GetComponent<Text>();
        //newNumBg = transform.Find("Canvas/TopTile/NewButton/NumBg").gameObject;
        ContactUsBtn = transform.Find("Canvas/TopTile/ContactUsButton").gameObject;
        //EmailButton = transform.Find("Canvas/TopTile/EmailButton").gameObject;
        //NewButton = transform.Find("Canvas/TopTile/NewButton").gameObject;

        CollectAll = transform.Find("Canvas/EmailScrollView/CollectAll").gameObject;

        //EmailOff = transform.Find("Canvas/TopTile/EmailButton/Off").gameObject;
        //OffList.Add(EmailOff);
        //NewOff = transform.Find("Canvas/TopTile/NewButton/Off").gameObject;
        //OffList.Add(NewOff);

        //EmailOn = transform.Find("Canvas/TopTile/EmailButton/ON").gameObject;
        //OnList.Add(EmailOn);
        //NewOn = transform.Find("Canvas/TopTile/NewButton/ON").gameObject;
        //OnList.Add(NewOn);

        EmailScrollView = transform.Find("Canvas/EmailScrollView").GetComponent<ScrollRect>();
        EmailItem = transform.Find("Canvas/EmailScrollView/Viewport/Content/EmailItem").gameObject;
        EmailScrollbarVertical = transform.Find("Canvas/EmailScrollView/Scrollbar Vertical").GetComponent<Scrollbar>();
        EmailItem.SetActive(false);

        NewScrollView = transform.Find("Canvas/NewScrollView").GetComponent<ScrollRect>();
        NewItem = transform.Find("Canvas/NewScrollView/Viewport/Content/NewItem").gameObject;
        NewScrollbarVertical= transform.Find("Canvas/NewScrollView/Scrollbar Vertical").GetComponent<Scrollbar>();
        NewItem.SetActive(false);

        CloseButton = transform.Find("Canvas/TopTile/CloseButton").gameObject;
        TopTile = transform.Find("Canvas/TopTile").GetComponent<RectTransform>();

        //UIEventListener.AddOnClickListener(EmailButton,EmailButtonOn);
        UIEventListener.AddOnClickListener(ContactUsBtn, ContectUsHandler);
        UIEventListener.AddOnClickListener(CollectAll, CollectAllOnclicke);

        UIEventListener.AddOnClickListener(CloseButton,CloseButtonButton);

        addMessageListener(EventEnum.ContenPostRest.ToString(), ContenPostRest);

        EmailScrollbarVertical.onValueChanged.AddListener(EmailPageturning);
        NewScrollbarVertical.onValueChanged.AddListener(NewPageturning);

        //红点协议
        ////UINetLoadingMgr.Instance.Show();
        GetimpinfoCallBacke();

        if (ResolutionAdapter.HasUnSafeArea)
        {
            var scale = this.myForm.GetScale();
            var safeArea = ResolutionAdapter.GetSafeArea();
            var offset = this.myForm.Pixel2View(safeArea.position);
            var offerH = offset.y;

            TopTile.sizeDelta = new Vector2(750, UserDataManager.Instance.TopHight);

            RectTransform EmailScrollViewRect = EmailScrollView.GetComponent<RectTransform>();

            EmailScrollViewRect.offsetMax = new Vector2(0, -(UserDataManager.Instance.TopHight));

        }

    }

    public override void OnClose()
    {
        base.OnClose();
        CloseUI = true;
        //红点协议     
        //GameHttpNet.Instance.Getimpinfo(GetimpinfoCallBacke);

        //UIEventListener.RemoveOnClickListener(EmailButton,EmailButtonOn);
        //UIEventListener.RemoveOnClickListener(NewButton,NewButtonOn);
        UIEventListener.RemoveOnClickListener(CollectAll, CollectAllOnclicke);
        UIEventListener.RemoveOnClickListener(CloseButton, CloseButtonButton);

        EmailScrollbarVertical.onValueChanged.RemoveListener(EmailPageturning);
        NewScrollbarVertical.onValueChanged.RemoveListener(NewPageturning);


        if (NewEmailItemSpriteList != null)
        {
            for (int i=0;i< NewEmailItemSpriteList.Count;i++)
            {
                if(NewEmailItemSpriteList[i]!=null)
                   NewEmailItemSpriteList[i].DestroyGame();//销毁生成的子物体
            }
        }

        if (NewnewsItemSpriteList != null)
        {
            for (int i = 0; i < NewnewsItemSpriteList.Count; i++)
            {
                if (NewnewsItemSpriteList[i] != null)
                    NewnewsItemSpriteList[i].DestroyGame();//销毁生成的子物体
            }
        }

        //未领取的邮件奖励个数
        //int EmailAwarNumber = UserDataManager.Instance.selfBookInfo.data.unreceivemsgcount;
        int Emailshu = UserDataManager.Instance.selfBookInfo.data.unreadmsgcount ;
        int Newshu = 0;// UserDataManager.Instance.lotteryDrawInfo.data.unreadnewscount;
        int CountNumber = Emailshu;
        EventDispatcher.Dispatch(EventEnum.EmailNumberShow, CountNumber);
    }

    private void CloseButtonButton(PointerEventData data)
    {
        CUIManager.Instance.CloseForm(UIFormName.EmailAndNew);
    }

    private void ContectUsHandler(PointerEventData data)
    {
        CUIManager.Instance.OpenForm(UIFormName.FAQFeedBack);
    }

    /// <summary>
    /// 这个是邮件的一键领取功能
    /// </summary>
    private void CollectAllOnclicke(PointerEventData data)
    {
        LOG.Info("一件领取被点击了");
        GameHttpNet.Instance.Achieveallmsgprice(AchieveallmsgpriceCallBack);
    }

    private void AchieveallmsgpriceCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----AchieveallmsgpriceCallBack---->" + result);
        if (result.Equals("error"))
        {
            LOG.Info("---AchieveallmsgpriceCallBack--这条协议错误");
            return;
        }
        LoomUtil.QueueOnMainThread((param) =>
        {
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo != null)
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.achieveallmsgprice = JsonHelper.JsonToObject<HttpInfoReturn<Achieveallmsgprice>>(arg.ToString());

                    EventDispatcher.Dispatch(EventEnum.Achieveallmsgprice);

                    UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.achieveallmsgprice.data.bkey);
                    UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.achieveallmsgprice.data.diamond);

                    CollectAll.SetActive(false);

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(158);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Received successful!", false);
                }
            }

        }, null);
    }

    private void EmailButtonOn(PointerEventData data)
    {
        TopButtonShow(0);
        EmailScrollView.gameObject.SetActive(true);
        NewScrollView.gameObject.SetActive(false);


        if (EmailScrollView.content.childCount<=1)
        {
            //如果没有生成邮件物体，就生成
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.GetEmail(EmailPage, GetEmailCallBacke);
        }
    }
    private void NewButtonOn(PointerEventData data)
    {
        TopButtonShow(1);
        EmailScrollView.gameObject.SetActive(false);
        NewScrollView.gameObject.SetActive(true);

        if (NewScrollView.content.childCount <= 1)
        {
            //UINetLoadingMgr.Instance.Show();
            //如果没有生成新闻物体，就生成
            GameHttpNet.Instance.GetNewInfo(NewPage, NewInfoCallBacke);
        }
    }

    private void TopButtonShow(int index)
    {
        for (int i=0;i<OnList.Count;i++)
        {
            if (i==index)
            {
                OnList[i].SetActive(true);
            }else
            {
                OnList[i].SetActive(false);
            }
        }

        for (int i=0;i<OffList.Count;i++)
        {
            if (i == index)
            {
                OffList[i].SetActive(false);
            }
            else
            {
                OffList[i].SetActive(true);
            }
        }
    }

    public void GetimpinfoCallBacke()
    {
        //int EmailAwarNumber = UserDataManager.Instance.selfBookInfo.data.unreceivemsgcount;
        int Emailshu = UserDataManager.Instance.selfBookInfo.data.unreadmsgcount;
        int Newshu = 0;// UserDataManager.Instance.lotteryDrawInfo.data.unreadnewscount;
        int CountNumber = Emailshu;

        if (CloseUI)
        {
            EventDispatcher.Dispatch(EventEnum.EmailNumberShow.ToString(), CountNumber);
            return;
        }

        //if (EmailNumber == null || newNumber == null)
        //    return;

        //if (CountNumber > 0)
        //{
        //    CollectAll.SetActive(true);
        //}
        //else
        //{
        //    CollectAll.SetActive(false);
        //}

        //if (Emailshu > 0)
        //{
        //    if (EmailNumber == null) return;
        //    if (EmailNumBg == null) return;

        //    EmailNumber.text = Emailshu.ToString();
        //    EmailNumBg.SetActive(true);
        //}
        //else
        //{
        //    if (EmailNumBg == null) return;

        //    EmailNumBg.SetActive(false);
        //}

        //if (Newshu > 0)
        //{
        //    if (newNumber == null) return;
        //    if (newNumBg == null) return;

        //    newNumber.text = Newshu.ToString();
        //    newNumBg.SetActive(true);
        //}
        //else
        //{
        //    if (newNumBg == null) return;

        //    newNumBg.SetActive(false);
        //}

        EmailButtonOn(null);
    }


    /// <summary>
    /// 邮件数量减少
    /// </summary>
    public void EmailNumberLower()
    {
        int Emailshu = UserDataManager.Instance.selfBookInfo.data.unreadmsgcount-1;
        UserDataManager.Instance.selfBookInfo.data.unreadmsgcount = Emailshu;
        //EmailNumber.text = Emailshu.ToString();
    }

    /// <summary>
    /// 新闻数量减少
    /// </summary>
    public void newNumberLower()
    {
        int Newshu = 0;// UserDataManager.Instance.lotteryDrawInfo.data.unreadnewscount-1;
        //UserDataManager.Instance.lotteryDrawInfo.data.unreadnewscount = Newshu;
        //newNumber.text = Newshu.ToString();      
    }

    /// <summary>
    /// 这个是content中的Y值设置
    /// </summary>
    private void ContenPostRest(Notification notification)
    {
        int Number = (int)notification.Data - 1;
        RectTransform rect = EmailScrollView.content;
        float x = rect.anchoredPosition.x;
        float LastY = rect.anchoredPosition.y;
       
        //详情信息打开
        DOTween.To(() => LastY, (value) => {
     
            rect.anchoredPosition = new Vector2(x, value);

        }, EmailItemHight * Number, 0.3f);//times秒，从 LastY 变到 EmailItemHight * Number

        LOG.Info("这个邮件上面有几个邮件物体："+ Number);
    }

    #region 生成新闻
    private void NewInfoCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----NewInfoCallBacke---->" + result);
        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo.code == 200)
            {
                UserDataManager.Instance.NewInfoCont = JsonHelper.JsonToObject<HttpInfoReturn<NewInfoCont>>(result);

                //NewPage++;
                //if (NewPage >= UserDataManager.Instance.NewInfoCont.data.pages_total)
                //    NewPage = UserDataManager.Instance.NewInfoCont.data.pages_total;


                List<NewInfoList> item = UserDataManager.Instance.NewInfoCont.data.newlist;

                for (int i=0;i<item.Count;i++)
                {
                    GameObject go = Instantiate(NewItem);
                    go.transform.SetParent(NewScrollView.content);
                    go.SetActive(true);

                    NewnewsItemSprite NewnewsItemSprite = go.GetComponent<NewnewsItemSprite>();
                    NewnewsItemSprite.Inite(item[i], this);

                    if (NewnewsItemSpriteList == null)
                        NewnewsItemSpriteList = new List<NewnewsItemSprite>();
                    NewnewsItemSpriteList.Add(NewnewsItemSprite);
                }
                NewPageTrun = false;
            }
        }, null);
    }
    #endregion

    #region 生成邮件
    public void GetEmailCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetEmailCallBacke---->" + result);
        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo.code == 200)
            {
                UserDataManager.Instance.EmailList = JsonHelper.JsonToObject<HttpInfoReturn<EmailListCont>>(result);
                if (UserDataManager.Instance.EmailList != null)
                {
                    List<EmailItemInfo> Emailistchil = UserDataManager.Instance.EmailList.data.sysarr;
                   
                    //EmailPage++;
                    //if (EmailPage >= UserDataManager.Instance.EmailList.data.pages_total)
                    //    EmailPage = UserDataManager.Instance.EmailList.data.pages_total;

                    if (Emailistchil != null)
                    {                     
                        for (int i=0;i< Emailistchil.Count;i++)
                        {
                            EmailItemNumber++;
                            GameObject go = Instantiate(EmailItem);
                            go.transform.SetParent(EmailScrollView.content);
                            go.SetActive(true);
                           
                            NewEmailItemSprite NewEmailItemSprite = go.GetComponent<NewEmailItemSprite>();
                            NewEmailItemSprite.Inite(Emailistchil[i],this, EmailItemNumber);

                            if (NewEmailItemSpriteList == null)
                                NewEmailItemSpriteList = new List<NewEmailItemSprite>();
                            NewEmailItemSpriteList.Add(NewEmailItemSprite);
                        }
                    }

                    EmailPageTrun = false;
                }
            }
        }, null);
    }
    #endregion

    #region 邮件翻页
    private bool EmailPageTrun = false;
    private void EmailPageturning(float ve)
    {
        if (ve <= 0&& !EmailPageTrun)
        {
            EmailPageTrun = true;
            EmailPage++;
            Debug.Log("邮件翻页");
            if (EmailPage> UserDataManager.Instance.EmailList.data.pages_total)
            {
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(159);
                //UITipsMgr.Instance.PopupTips(Localization, false);
                return;
            }

            //生成邮件物体
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.GetEmail(EmailPage, GetEmailCallBacke);

        }
    }
    #endregion

    #region 新闻翻页
    private bool NewPageTrun = false;
    private void NewPageturning(float ve)
    {
        if (ve <= 0 && !NewPageTrun)
        {
            NewPageTrun = true;
            NewPage++;
            Debug.Log("新闻翻页:"+ NewPage);
            if (NewPage > UserDataManager.Instance.NewInfoCont.data.pages_total)
            {
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(160);
                UITipsMgr.Instance.PopupTips(Localization, false);
                return;
            }

            //如果没有生成新闻物体，就生成
            //UINetLoadingMgr.Instance.Show();          
            GameHttpNet.Instance.GetNewInfo(NewPage, NewInfoCallBacke);
        }
    }
    #endregion


}

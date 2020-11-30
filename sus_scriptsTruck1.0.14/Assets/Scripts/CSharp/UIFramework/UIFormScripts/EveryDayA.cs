using pb;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class EveryDayA : BaseUIForm {

    public GameObject Mask,DayActivtyItem, BoxOpenGame, Box, receiveButton;
    public ScrollRect scrollView;
    public Image DownPross, receiveButtonImage;
    public RectTransform RewardEffect, keypos, dimpos;

    private int HadStart, AllNeedStart;
    private List<EveryDayBox> EveryDayBox;
    private List<DayActivtyItem> DayActivtyItemList;

    private Image mTopBg;
    private Text mTopTitle,DailyrefreshText;
    private RectTransform mBackBtn;

    void Awake()
    {
        mTopBg = transform.Find("Canvas/BG/top").GetComponent<Image>();
        mTopTitle = mTopBg.transform.Find("Text").GetComponent<Text>();
        mBackBtn = mTopBg.transform.Find("Image").GetComponent<RectTransform>();
        DailyrefreshText = transform.Find("Canvas/BG/DailyrefreshText").GetComponent<Text>();
    }

    public override void OnOpen()
    {
        base.OnOpen();

        //【屏幕适配】
        float offect = XLuaHelper.UnSafeAreaNotFit(this.myForm, mTopBg.rectTransform(), 750, 92);
        mTopTitle.rectTransform().anchoredPosition = new Vector2(0, 5 - offect / 2);
        mBackBtn.transform.rectTransform().anchoredPosition = new Vector2(55, -44 - offect);
        Mask.transform.rectTransform().anchoredPosition = new Vector2(-321, 3 - offect / 2);
        scrollView.transform.rectTransform().offsetMax = new Vector2(-20, -(526 + offect));
        DailyrefreshText.rectTransform().anchoredPosition = new Vector2(-5, -490 - offect);
        BoxOpenGame.transform.rectTransform().anchoredPosition = new Vector2(1, -232 - offect);



        DayActivtyItemList = new List<DayActivtyItem>();
        DayActivtyItemList.Clear();

        if (EveryDayBox != null)
        {
            EveryDayBox.Clear();
        }
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.Getusertask(GetusertaskCallBacke);
     
        UIEventListener.AddOnClickListener(Mask, ClostButton);

        UIEventListener.AddOnClickListener(receiveButton,receiveButtonOnclicke);

        addMessageListener(EventEnum.DayActivtyItemReviec, DayActivtyItemReviec);
        addMessageListener(EventEnum.receiveButtonImageChange, receiveButtonImageChange);

        DayActivtyItem.SetActive(false);
        Box.SetActive(false);

        //Transform bgTrans = this.gameObject.transform.Find("Canvas/BG");
        //if (GameUtility.IpadAspectRatio() && bgTrans != null)
        //    bgTrans.localScale = Vector3.one * 0.7f;

       

    }

    private void receiveButtonOnclicke(PointerEventData data)
    {
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.Achieveboxprice(0, AchieveboxpriceCallBack);
    }

    private void AchieveboxpriceCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----AchieveboxpriceCallBack---->" + result);
        if (result.Equals("error"))
        {
            LOG.Info("---AchieveboxpriceCallBack--这条协议错误");
            return;
        }
        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo != null)
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.Achievetaskprice = JsonHelper.JsonToObject<HttpInfoReturn<Achievetaskprice>>(arg.ToString());

                    UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.Achievetaskprice.data.bkey);
                    UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.Achievetaskprice.data.diamond);

                    for (int i=0;i< EveryDayBox.Count;i++)
                    {
                        EveryDayBox[i].AllBoxRevice();
                    }

                    EventDispatcher.Dispatch(EventEnum.receiveButtonImageChange, 2);
                }
                else
                {
                    LOG.Info("宝箱领取失败");
                    //var Localization = GameDataMgr.Instance.table.GetLocalizationById(138);
                    //UITipsMgr.Instance.PopupTips(Localization, false);
                }
            }
        }, null);
    }
    public override void OnClose()
    {
        base.OnClose();

        CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop).GetimpinfoTochange();
        UIEventListener.RemoveOnClickListener(Mask, ClostButton);

        UIEventListener.RemoveOnClickListener(receiveButton, receiveButtonOnclicke);


        if (DayActivtyItemList!=null)
        {
            for (int i=0;i< DayActivtyItemList.Count;i++)
            {
                DayActivtyItemList[i].DisPoste();
            }
        }

    }

    private void ClostButton(PointerEventData data)
    {
        CUIManager.Instance.CloseForm(UIFormName.EvreyDayActivty);
    }

    //这个是表示宝箱领取按钮的状态改变
    private void receiveButtonImageChange(Notification notification)
    {
        int type = (int)notification.Data;
        if (type==1)
        {
            //有宝箱可以领取
            receiveButtonImage.sprite = ResourceManager.Instance.GetUISprite("Notice/btn_focus3");
        }
        else
        {
            //没有宝箱可以领取
            receiveButtonImage.sprite = ResourceManager.Instance.GetUISprite("Notice/btn_focus2");
        }

    }

    public void GetboxlistCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetboxlistCallBacke---->" + result);
        if (result.Equals("error"))
        {
            LOG.Info("---GetboxlistCallBacke--这条协议错误");
            return;
        }
        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo != null)
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.Getboxlist = JsonHelper.JsonToObject<HttpInfoReturn<Getboxlist>>(arg.ToString());

                    List<boxarr> star_require = UserDataManager.Instance.Getboxlist.data.boxarr;
                    if (star_require.Count > 0)
                    {
                        for (int i = 0; i < star_require.Count; i++)
                        {
                            GameObject go = Instantiate(Box, BoxOpenGame.transform);
                            go.SetActive(true);
                            EveryDayBox EveryDayBoxs = go.GetComponent<EveryDayBox>();

                            if (EveryDayBox == null)
                            {
                                EveryDayBox = new List<EveryDayBox>();
                            }
                           
                            EveryDayBox.Add(EveryDayBoxs);
                            EveryDayBoxs.EveryDayBoxInit(star_require[i].box_id, HadStart, star_require[i].star_count, AllNeedStart);
                        }
                    }
                }
            }

        }, null);
    }
    private void GetusertaskCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetusertaskCallBacke---->" + result);
        if (result.Equals("error"))
        {
            LOG.Info("---GetusertaskCallBacke--这条协议错误");
            return;
        }
        LoomUtil.QueueOnMainThread((param) =>
        {
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo != null)
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.Getusertask = JsonHelper.JsonToObject<HttpInfoReturn<Getusertask>>(arg.ToString());

                    List<taskarr> tas = UserDataManager.Instance.Getusertask.data.taskarr;

                    if (UserDataManager.Instance.Getusertask.data.box_status > 0)
                    {
                        //有宝箱可以领取receiveButtonImageChange

                        EventDispatcher.Dispatch(EventEnum.receiveButtonImageChange, 1);
                    }
                    else
                    {
                        //没有宝箱可以领取
                        EventDispatcher.Dispatch(EventEnum.receiveButtonImageChange, 2);
                    }


                    if (tas.Count<=0)
                    {
                        LOG.Info("没有任务");
                        return;
                    }

                    for (int i=0;i<tas.Count;i++)
                    {
                        GameObject go = Instantiate(DayActivtyItem, scrollView.content.transform);
                        go.SetActive(true);
                        //go.GetComponent<DayActivtyItem>().DayActivtyItemInit(tas[i], RewardEffect,  keypos,  dimpos);

                        DayActivtyItem tem = go.GetComponent<DayActivtyItem>();
                        tem.DayActivtyItemInit(tas[i], RewardEffect, keypos, dimpos);
                        DayActivtyItemList.Add(tem);
                    }

                    string userstar = UserDataManager.Instance.Getusertask.data.userstar;
                    string[] ImageMane =userstar.Split('/');
                    HadStart = int.Parse(ImageMane[0]);//当前宝箱完成的星星
                    AllNeedStart = int.Parse(ImageMane[1]);//进度条满格需要的星星数
                    DownPross.fillAmount = HadStart * 1.0f / AllNeedStart;

                    if (UserDataManager.Instance.Getboxlist==null)
                    {
                        GameHttpNet.Instance.Getboxlist(GetboxlistCallBacke);
                    }
                    else
                    {
                        //UINetLoadingMgr.Instance.Close();

                        List<boxarr> star_require = UserDataManager.Instance.Getboxlist.data.boxarr;
                        if (star_require.Count > 0)
                        {
                            
                            for (int i = 0; i < star_require.Count; i++)
                            {
                                GameObject go = Instantiate(Box, BoxOpenGame.transform);
                                go.SetActive(true);
                                EveryDayBox EveryDayBoxs = go.GetComponent<EveryDayBox>();

                                if (EveryDayBox == null)
                                {
                                    EveryDayBox = new List<EveryDayBox>();
                                }                              
                                EveryDayBox.Add(EveryDayBoxs);                               
                                EveryDayBoxs.EveryDayBoxInit(star_require[i].box_id, HadStart, star_require[i].star_count, AllNeedStart);
                            }
                        }
                    }
                    
                }
            }

        }, null);
    }


    public void DayActivtyItemReviec(Notification notfi)
    {
       
        int StarAdd = (int)notfi.Data;
        HadStart += StarAdd;
        DownPross.fillAmount = HadStart * 1.0f / AllNeedStart;
        
        for (int i=0;i< EveryDayBox.Count;i++)
        {
            EveryDayBox[i].ReviecDayActivtyInit(i,HadStart);
        }
    }
}

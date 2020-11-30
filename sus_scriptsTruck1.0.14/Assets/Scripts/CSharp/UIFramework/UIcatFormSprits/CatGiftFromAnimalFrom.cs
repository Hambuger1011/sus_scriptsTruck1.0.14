using System;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CatGiftFromAnimalFrom : CatBaseForm {

    private GameObject BackButton;
    private GameObject CatAnimalGiftItem;
    private GameObject BtnAccepetAll;
    private ScrollRect ScrollView;
    private Transform Content;
    InfinityGridLayoutGroup infinityGridLayoutGroup;
    int page = 1;
    List<GameObject> catAnimalgifttemList;
    List<feedback> feedbackarr;
    private int GiftReturnNumber;
    private RectTransform BG;
    public float ScrollViewH,ScreenH;

    private CatAnimalGiftItem CatAnimalGiftItemForm;
    private Text BtnAccepetAllText;

    private void Awake()
    {
        BackButton = transform.Find("Bg/Top/BG/BackButton").gameObject;
        BtnAccepetAll = transform.Find("Bg/AcceptAll").gameObject;
        BtnAccepetAllText= transform.Find("Bg/AcceptAll/Text").GetComponent<Text>();
        BtnAccepetAllText.text = GameDataMgr.Instance.table.GetLocalizationById(273);

        infinityGridLayoutGroup = transform.Find("Bg/Scroll View/Viewport/Content").GetComponent<InfinityGridLayoutGroup>();
        ScrollView = transform.Find("Bg/Scroll View").GetComponent<ScrollRect>();
        Content = transform.Find("Bg/Scroll View/Viewport/Content");
        CatAnimalGiftItem = transform.Find("Bg/Item/CatGiftFromAnimalItem").gameObject;
        catAnimalgifttemList = new List<GameObject>();
        bar.onValueChanged.AddListener(ScrollbarChange);

        BG = transform.Find("Bg/Top/BG").GetComponent<RectTransform>();

        ScrollViewH = ScrollView.GetComponent<RectTransform>().anchoredPosition.y;
        ScreenH =transform.Find("UIMask").GetComponent<RectTransform>().rect.height;

    }
    public override void OnOpen()
    {
        base.OnOpen();
        UiFormIndex = (int)CatFormEnum.CAT_GIFT_FROM_ANIM;
        GiftReturnNumber = 0;
        //end
        //UINetLoadingMgr.Instance.Show();
       
        UIEventListener.AddOnClickListener(BackButton, CloseUi);
        UIEventListener.AddOnClickListener(BtnAccepetAll, AcceptAll);
        Content.gameObject.SetActive(true);
        //RefreshDiamondAndHeart(UserDataManager.Instance.SceneInfo.data.usermoney.diamond.ToString(), UserDataManager.Instance.SceneInfo.data.usermoney.love.ToString());

        EventDispatcher.AddMessageListener(EventEnum.OnGiftReturnNumberStatistics, OnGiftReturnNumberStatistics);

        EventDispatcher.AddMessageListener(EventEnum.SetGuidPos, SetGuidPos);


        if (ResolutionAdapter.HasUnSafeArea)
        {
            var scale = this.myForm.GetScale();
            var safeArea = ResolutionAdapter.GetSafeArea();
            var offset = this.myForm.Pixel2View(safeArea.position);
            var offerH = offset.y;

            BG.sizeDelta = new Vector2(750, 120 + offerH);
            ScrollView.rectTransform().offsetMax = new Vector2(54, -(180 + offerH));
        }

        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.GetGiftGuidYes)
        {
            //这个是猫新手引导的时候，模拟一条数据          
            GuidGiftInfoSimulation();

            ScrollView.enabled = false;
        }
        else
        {
            GameHttpNet.Instance.PostpetGiftinfo(page, GetpetGiftInfoCallback);
        }

    }

    private void AcceptAll(PointerEventData eventData)
    {

        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.GetGiftGuidYes)
        {
            CatAnimalGiftItemForm.OnOkClick();
        }
        else
        {

            GameHttpNet.Instance.PostGetpetGift(0, 1, 1, GetAllPetGiftCallback);
        }
    }
    /// <summary>
    /// 一键领取的回调
    /// </summary>
    /// <param name="arg"></param>
    private void GetAllPetGiftCallback(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetpetgiftinfoCallbacke---->" + result);

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.usermoney = JsonHelper.JsonToObject<HttpInfoReturn<usermoney>>(result);
                    UserDataManager.Instance.UpdateCatGoodsCount(UserDataManager.Instance.usermoney.data.diamond, UserDataManager.Instance.usermoney.data.love);
                    var tmpForm = CUIManager.Instance.GetForm<CatTopMain>(UIFormName.CatTop);
                    if (tmpForm != null)
                    {
                        //string dia = UserDataManager.Instance.SceneInfo.data.usermoney.diamond.ToString();
                        //string love = UserDataManager.Instance.SceneInfo.data.usermoney.love.ToString();
                        //string food = UserDataManager.Instance.SceneInfo.data.usermoney.otherfood.ToString();
                        //tmpForm.RefreshDiamondAndHeart(dia, love, food);
                        
                        tmpForm.RefreshDiamond(1, UserDataManager.Instance.SceneInfo.data.usermoney.love);
                        //tmpForm.RefreshDiamond(2, UserDataManager.Instance.SceneInfo.data.usermoney.diamond);
                        UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.SceneInfo.data.usermoney.diamond);
                    }
                    RefreshData();

                    EventDispatcher.Dispatch(EventEnum.OnGiftReturnNumberStatistics, UserDataManager.Instance.Getpetgiftinfo.data.feedback_count);

                    //BtnAccepetAll.SetActive(false);

                    CloseUi(null);//一键领取成功后，关闭这个界面
                }
                else
                {
                    UITipsMgr.Instance.PopupTips(jo.msg, false);
                }


            }, null);
        }
    }

    private void RefreshData()
    {
        if (Content)
        {
            Content.gameObject.SetActive(false);
        }
        foreach (var item in UserDataManager.Instance.Getpetgiftinfo.data.feedback)
        {
            item.isprice = 1;
        }
    }
    /// <summary>
    /// 领取单个
    /// </summary>
    /// <param name="arg"></param>
    private void GetpetGiftInfoCallback(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetpetgiftinfoCallbacke---->" + result);

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.Getpetgiftinfo = JsonHelper.JsonToObject<HttpInfoReturn<Getpetgiftinfo>>(result);
                    feedbackarr = UserDataManager.Instance.Getpetgiftinfo.data.feedback;



                    if (feedbackarr != null && feedbackarr.Count>0)
                    {
                        BtnAccepetAll.SetActive(true);
                        SpwanCatAnimalGiftItem(feedbackarr);
                    }
                    else
                    {
                        //BtnAccepetAll.SetActive(false);
                        CloseUi(null);//领取成功后，关闭这个界面
                    }

                }
            }, null);
        }
    }


    private void SpwanCatAnimalGiftItem(List<feedback> tem)
    {
        

        int TemCount = tem.Count;//获得列表中总共有多少猫
        //int ListPoolNum = 8; //规定超过多少个才使用复用列表


        for (int i = 0; i < TemCount; i++)
        {
            GameObject go = Instantiate(CatAnimalGiftItem);
            go.transform.SetParent(ScrollView.content.transform);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.SetActive(true);
            CatAnimalGiftItemForm = go.GetComponent<CatAnimalGiftItem>();
            CatAnimalGiftItemForm.Init(feedbackarr[i], i, i,this);
            catAnimalgifttemList.Add(go);

        }

        //if (TemCount <= ListPoolNum)
        //{
        //    //这里不使用复用列表


        //    for (int i = 0;i< TemCount;i++)
        //    {
        //        GameObject go = Instantiate(CatAnimalGiftItem);
        //        go.transform.SetParent(ScrollView.content.transform);
        //        go.transform.localScale = Vector3.one;
        //        go.transform.localPosition = Vector3.zero;
        //        go.SetActive(true);
        //        go.GetComponent<CatAnimalGiftItem>().Init(feedbackarr[i], i, i);
        //        catAnimalgifttemList.Add(go);
                
        //    }
           
        //}
        //else
        //{
        //    for (int i = 0; i < ListPoolNum; i++)
        //    {
        //        GameObject go = Instantiate(CatAnimalGiftItem);
        //        go.transform.SetParent(ScrollView.content.transform);
        //        go.transform.localScale = Vector3.one;
        //        go.transform.localPosition = Vector3.zero;
        //        go.SetActive(true);
        //        catAnimalgifttemList.Add(go);
        //    }

        //    infinityGridLayoutGroup.SetAmount(TemCount);
        //    infinityGridLayoutGroup.updateChildrenCallback = UpdateChildrenCallback;
        //}
    }

    public void RemoveGame(GameObject go)
    {
        catAnimalgifttemList.Remove(go);

        Destroy(go);
    }

    void UpdateChildrenCallback(int index, Transform trans)
    {
        if (feedbackarr != null)
            trans.GetComponent<CatAnimalGiftItem>().Init(feedbackarr[index], index + 1,index, this);
    }

    public override void OnClose()
    {
        base.OnClose();

        EventDispatcher.RemoveMessageListener(EventEnum.SetGuidPos, SetGuidPos);

        page = 1;
        if (catAnimalgifttemList.Count>0)
        {
            for (int i = 0; i < catAnimalgifttemList.Count; i++)
            {
                //调用这个方法移除所有实例出来的物体
                catAnimalgifttemList[i].GetComponent<CatAnimalGiftItem>().Dispose();
            }
        }

    }
    public Scrollbar bar;
    private void ScrollbarChange(float ve)
    {
         //Debug.Log("值是：" + ScrollView.verticalNormalizedPosition);

        if (ScrollView.verticalNormalizedPosition < 0)
        {
            UpdatePageData();
        }

    }

    private void UpdatePageData()
    {
        //LOG.Info("滑动到了底部了，需要更新数据");
       
        //if (UserDataManager.Instance.Getpetinfo != null)
        //{
            int count = UserDataManager.Instance.Getpetgiftinfo.data.pages_total;
            //LOG.Info("最大页数是：" + count);
            page++;
            if (page > count)
            {
            
                UITipsMgr.Instance.PopupTips(GameDataMgr.Instance.table.GetLocalizationById(240)/*"没有更多的回馈了"*/, false);
                return;
            }
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.PostpetGiftinfo(page, GetGiftNewPageDataCallback);
        //}
    }

    private void GetGiftNewPageDataCallback(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetpetgiftinfoCallbacke---->" + result);

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    HttpInfoReturn<Getpetgiftinfo> Getpetgiftinfo = JsonHelper.JsonToObject<HttpInfoReturn<Getpetgiftinfo>>(result);
                    UserDataManager.Instance.Getpetgiftinfo = Getpetgiftinfo;
                    if (Getpetgiftinfo!=null)
                    {
                        feedbackarr = UserDataManager.Instance.Getpetgiftinfo.data.feedback;
                        int TemCount = feedbackarr.Count;//获得列表中总共有多少猫
                        for (int i = 0; i < TemCount; i++)
                        {
                            GameObject go = Instantiate(CatAnimalGiftItem);
                            go.transform.SetParent(ScrollView.content.transform);
                            go.transform.localScale = Vector3.one;
                            go.transform.localPosition = Vector3.zero;
                            go.SetActive(true);
                            go.GetComponent<CatAnimalGiftItem>().Init(feedbackarr[i], i, i, this);
                            catAnimalgifttemList.Add(go);

                        }
                    }
                    
                   
                }
            }, null);
        }
    }

    private void CloseUi(PointerEventData data)
    {
        //派发事件，关闭这个界面
        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), UiFormIndex);
        UIEventListener.RemoveOnClickListener(BtnAccepetAll, AcceptAll);
        bar.onValueChanged.AddListener(ScrollbarChange);

        EventDispatcher.RemoveMessageListener(EventEnum.OnGiftReturnNumberStatistics, OnGiftReturnNumberStatistics);

    }


    /// <summary>
    /// 这个是宠物回赠数量剩余的统计
    /// </summary>
    /// <param name="notification"></param>
    private void OnGiftReturnNumberStatistics(Notification notification)
    {
        int AddNum = (int)notification.Data;

        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.GetGiftGuidYes)
        {
            //关闭引导界面
            CloseUi(null);//关闭这个界面
        }

        if (UserDataManager.Instance.Getpetgiftinfo != null)
        {
            GiftReturnNumber += AddNum;

            LOG.Info("回赠领取次数计数：" + GiftReturnNumber);
            if (GiftReturnNumber >= UserDataManager.Instance.Getpetgiftinfo.data.feedback_count)
            {
                //当领取回赠数量大于等于回赠数量的时候，判断回赠领取完了，把主界面的回赠按钮隐藏

                //没有宠物馈赠
                //BtnAccepetAll.SetActive(false);
                CloseUi(null);//关闭这个界面
            }
        }

    }

    private void GuidGiftInfoSimulation()
    {
        object arg = "{ 'code':200,'msg':'Collected!','data':{ 'feedback':[{'id':17869,'pid':'8','pet_name':'Amnesia Cat','love_qty':1,'diamond_qty':0,'create_time':1568025602,'used_time':18,'plan_time':'1 minutes ago','isprice':0,'decorations_id':1,'shop_id':6,'remark':'Sleeping is always pleasant, no matter the posture.'}],'feedback_count':14,'pages_total':1}}";
    
        string result = arg.ToString();
        LOG.Info("----GetpetgiftinfoCallbacke---->" + result);

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.Getpetgiftinfo = JsonHelper.JsonToObject<HttpInfoReturn<Getpetgiftinfo>>(result);
                    feedbackarr = UserDataManager.Instance.Getpetgiftinfo.data.feedback;

                    //BtnAccepetAll.SetActive(false);

                    if (feedbackarr != null && feedbackarr.Count > 0)
                    {                       
                        SpwanCatAnimalGiftItem(feedbackarr);
                    }
                    else
                    {                      
                        CloseUi(null);//领取成功后，关闭这个界面
                    }

                }
            }, null);
        }
    }

    /// <summary>
    /// 设置引导点击的位置
    /// </summary>
    /// <param name="notice"></param>
    public void SetGuidPos(Notification notice)
    {
        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.GetGiftGuidYes)
        {
            RectTransform Rect= BtnAccepetAll.GetComponent<RectTransform>();

            float pox = Rect.anchoredPosition.x;
            float poy = Rect.anchoredPosition.y;

            
            UserDataManager.Instance.GuidPos = new Vector3(pox, poy, 0);
            //LOG.Info("得到食碗的坐标");
        }
    }



}

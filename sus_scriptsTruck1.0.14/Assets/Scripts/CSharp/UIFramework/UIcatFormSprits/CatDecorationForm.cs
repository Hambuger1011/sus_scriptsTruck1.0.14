using pb;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class CatDecorationForm : CatBaseForm {

    private Transform BackButton;
    private GameObject CatDecorationItems;
    private ScrollRect ScrollView;
    InfinityGridLayoutGroup infinityGridLayoutGroup;
    List<TransitionItem> shopInfo = null;
    int page;
    private RectTransform BG;
    public  float ScrollViewH, UIMaskH, UIMaskW;
    private void Awake()
    {
        BackButton = transform.Find("Bg/Top/BG/BackButton");
        CatDecorationItems = transform.Find("Bg/CatDecorationItems").gameObject;
        ScrollView = transform.Find("Bg/Scroll View").GetComponent<ScrollRect>();
        infinityGridLayoutGroup = transform.Find("Bg/Scroll View/Viewport/Content").GetComponent<InfinityGridLayoutGroup>();
        BG = transform.Find("Bg/Top/BG").GetComponent<RectTransform>();

        RectTransform UIMaskRect = transform.Find("UIMask").GetComponent<RectTransform>();
        
        UIMaskH= UIMaskRect.rect.height;
        UIMaskW = UIMaskRect.rect.width;
    }
    public override void OnOpen()
    {
        base.OnOpen();
        UiFormIndex = (int)CatFormEnum.CAT_DECORATION;

        addMessageListener(EventEnum.UpdateCatGoodList, UpdateCatGoodListHandler);

        if (BackButton != null)
            UIEventListener.AddOnClickListener(BackButton.gameObject, CloseUi);


        if (ResolutionAdapter.HasUnSafeArea)
        {
            var scale = this.myForm.GetScale();
            var safeArea = ResolutionAdapter.GetSafeArea();
            var offset = this.myForm.Pixel2View(safeArea.position);
            var offerH = offset.y;

            BG.sizeDelta = new Vector2(750, 120 + offerH);
            ScrollView.rectTransform().offsetMax = new Vector2(54, -(180 + offerH));
        }

        ScrollViewH = ScrollView.transform.GetComponent<RectTransform>().anchoredPosition.y;

        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.Getpetpackinfo(ProcessGetPackInfo);

        if (UserDataManager.Instance.isFirstCatEnt)
        {
            //正在进行猫的引导
            ScrollView.enabled = false;
        }

    }

    private void UpdateCatGoodListHandler(Notification noti)
    {
        //GameHttpNet.Instance.Getpetpackinfo(ProcessGetPackInfo);
        if (UserDataManager.Instance.BagInfoContent != null)
        {
            Transiton(UserDataManager.Instance.BagInfoContent.data);
            SpwanDecorations();

            infinityGridLayoutGroup.SetAmount(shopInfo.Count);
            infinityGridLayoutGroup.updateChildrenCallback = UpdateChildrenCallback;
        }
    }
    
    private void ProcessGetPackInfo(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----ProcessGetPackInfo---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.BagInfoContent = JsonHelper.JsonToObject<HttpInfoReturn<BagInfo>>(result);
                    if (UserDataManager.Instance.BagInfoContent != null)
                    {
                        Transiton(UserDataManager.Instance.BagInfoContent.data);
                        SpwanDecorations();

                        infinityGridLayoutGroup.SetAmount(shopInfo.Count);
                        infinityGridLayoutGroup.updateChildrenCallback = UpdateChildrenCallback;

                      
                    }
                }
            }, null);
        }

    }
    public Scrollbar bar;
    private void ScrollbarChange(float ve)
    {
        if (ScrollView.verticalNormalizedPosition < 0)
        {
            UpdatePageData();
        }

    }

    private void UpdatePageData()
    {
        LOG.Info("滑动到了底部了，需要更新数据");
        page++;
        if (UserDataManager.Instance.Getpetfoodinfo != null)
        {
            int count = UserDataManager.Instance.Getpetfoodinfo.data.pages_total;
            if (page > count)
            {
                return;
            }
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.GetpetFoodinfo(page, GetDecorationNewPageDataCallback);
        }
    }

    private void GetDecorationNewPageDataCallback(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----ProcessGetPackInfo---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    HttpInfoReturn<Getpetinfo> addInfo = JsonHelper.JsonToObject<HttpInfoReturn<Getpetinfo>>(result);
                    if (addInfo != null)
                    {
                        UserDataManager.Instance.Getpetinfo.data.petarr.AddRange(addInfo.data.petarr);
                    }
                }

            }, null);
        }
    }

    private void SpwanDecorations()
    {
        //获得表中有几个物体
        int shopListCount = shopInfo.Count;

        if (DecorationsList == null)
        {
            DecorationsList = new List<CatDecorationItem>();

            int len = 10;
            for (int i = 0; i < len; i++)
            {
                GameObject go = Instantiate(CatDecorationItems);
                go.transform.SetParent(ScrollView.content.transform);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
                CatDecorationItem catDecItem = go.GetComponent<CatDecorationItem>();
                if(shopListCount > i)
                {
                    t_shop shopData = GameDataMgr.Instance.table.GetcatShopId(shopInfo[i].shopid);
                    catDecItem.Init(shopData, shopInfo[i],this);
                }
                catDecItem.gameObject.SetActive(true);
                DecorationsList.Add(catDecItem);
            }
        }
        else
        {
            int len = 8;
            for (int i = 0; i < len; i++)
            {
                CatDecorationItem catDecItem = DecorationsList[i];
                if (shopListCount > i)
                {
                    t_shop shopData = GameDataMgr.Instance.table.GetcatShopId(shopInfo[i].shopid);
                    catDecItem.Init(shopData, shopInfo[i],this);
                    catDecItem.gameObject.SetActive(true);
                }
                else
                {
                    catDecItem.gameObject.SetActive(false);
                }
            }
        }
    }

    private void Transiton(BagInfo data)
    {
        if (shopInfo == null)
        {
            shopInfo = new List<TransitionItem>();
        }
        shopInfo.Clear();


        if (UserDataManager.Instance.isFirstCatEnt)
        {
            //正在进行新手引导的时候，发装饰放前面，不然放后面

            for (int j = 0; j < data.decorationspack.Count; j++)
            {

                TransitionItem tmp = new TransitionItem();
                tmp.id = data.decorationspack[j].id;
                tmp.link_id = data.decorationspack[j].link_id;
                tmp.isUsed = data.decorationspack[j].isUsed;
                tmp.option_name = data.decorationspack[j].option_name;
                tmp.place = data.decorationspack[j].place;
                tmp.remark = data.decorationspack[j].remark;
                tmp.goods_type = 2;
                tmp.shopid = data.decorationspack[j].shop_id;
                shopInfo.Add(tmp);
            }

            for (int i = 0; i < data.foodpack.Count; i++)
            {

                TransitionItem tmp = new TransitionItem();
                tmp.id = data.foodpack[i].id;
                tmp.link_id = data.foodpack[i].link_id;
                tmp.isUsed = data.foodpack[i].isUsed;
                tmp.option_name = data.foodpack[i].option_name;
                tmp.count = data.foodpack[i].count;
                tmp.weight = data.foodpack[i].weight;
                tmp.remark = data.foodpack[i].remark;
                tmp.goods_type = 1;
                tmp.shopid = data.foodpack[i].shop_id;
                shopInfo.Add(tmp);
            }
        }else
        {          
            for (int i = 0; i < data.foodpack.Count; i++)
            {
                TransitionItem tmp = new TransitionItem();
                tmp.id = data.foodpack[i].id;
                tmp.link_id = data.foodpack[i].link_id;
                tmp.isUsed = data.foodpack[i].isUsed;
                tmp.option_name = data.foodpack[i].option_name;
                tmp.count = data.foodpack[i].count;
                tmp.weight = data.foodpack[i].weight;
                tmp.remark = data.foodpack[i].remark;
                tmp.goods_type = 1;
                tmp.shopid = data.foodpack[i].shop_id;
                shopInfo.Add(tmp);
            }
            for (int j = 0; j < data.decorationspack.Count; j++)
            {
                TransitionItem tmp = new TransitionItem();
                tmp.id = data.decorationspack[j].id;
                tmp.link_id = data.decorationspack[j].link_id;
                tmp.isUsed = data.decorationspack[j].isUsed;
                tmp.option_name = data.decorationspack[j].option_name;
                tmp.place = data.decorationspack[j].place;
                tmp.remark = data.decorationspack[j].remark;
                tmp.goods_type = 2;
                tmp.shopid = data.decorationspack[j].shop_id;
                shopInfo.Add(tmp);
            }
        }
    }


    private void CloseUi(PointerEventData data)
    {

        //派发事件，关闭这个界面
        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), UiFormIndex);

    }

    public override void OnClose()
    {
        base.OnClose();

        if (BackButton != null)
            UIEventListener.RemoveOnClickListener(BackButton.gameObject, CloseUi);
        if (DecorationsList!= null)
        {
            for (int i = 0; i < DecorationsList.Count; i++)
            {
                CatDecorationItem catDecItem = DecorationsList[i];
                if (catDecItem != null) catDecItem.Dispose();
                catDecItem = null;
            }
        }
        DecorationsList = null;
    }

    #region 生成装饰物

    List<CatDecorationItem> DecorationsList;


    void UpdateChildrenCallback(int index, Transform trans)
    {
        if (shopInfo.Count > index)
        {
            trans.gameObject.SetActive(true);
            t_shop shopData = GameDataMgr.Instance.table.GetcatShopId(shopInfo[index].shopid);
            if (shopData != null)
                trans.GetComponent<CatDecorationItem>().Init(shopData, shopInfo[index],this);
        }
        else
        {
            trans.gameObject.SetActive(false);
        }
    }
    #endregion

    public class TransitionItem
    {
        public int id;
        public int link_id;//:食物ID;
        public string option_name;//食物名称
        public int count;//数量
        public int weight;
        public int isUsed;//是否使用 1是  0否
        public string remark;
        public string place; //位置
        public int goods_type;//1 食物 2 装饰物
        public int shopid;

        public TransitionItem()
        {
            id = -1;
            link_id = -1;
            option_name = null;
            count = -1;
            weight = -1;
            isUsed = -1;
            remark = null;
            place = null;
            goods_type = 0;
            shopid = 0;
        }
    }

   
}

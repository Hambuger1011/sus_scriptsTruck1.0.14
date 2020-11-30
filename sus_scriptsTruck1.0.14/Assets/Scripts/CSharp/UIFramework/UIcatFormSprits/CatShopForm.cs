using pb;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class CatShopForm : CatBaseForm {

    private Transform BackButton;
    private GameObject catshopItem;
    private ScrollRect ScrollView;
    List<shoparr> shopinfo;
    InfinityGridLayoutGroup infinityGridLayoutGroup;
    private RectTransform BG;
    private int page = 1;

    [System.NonSerialized]
    public float UIMaskW, UIMaskH, ScrollViewTop;

    private RectTransform UIMask;
    private void Awake()
    {
        BackButton = transform.Find("Bg/Top/BG/BackButton");
        catshopItem = transform.Find("Bg/Item/catshopItem").gameObject;
        ScrollView = transform.Find("Bg/Scroll View").GetComponent<ScrollRect>();
        BG = transform.Find("Bg/Top/BG").GetComponent<RectTransform>();

        infinityGridLayoutGroup = transform.Find("Bg/Scroll View/Viewport/Content").GetComponent<InfinityGridLayoutGroup>();
        bar.onValueChanged.AddListener(ScrollbarChange);

        UIMask = transform.Find("UIMask").GetComponent<RectTransform>();
    }
    public override void OnOpen()
    {
        base.OnOpen();
        UiFormIndex = (int)CatFormEnum.CAT_SHOP;
      
        if (BackButton!=null)       
            UIEventListener.AddOnClickListener(BackButton.gameObject, CloseUi);

        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.GetpetFoodinfo(page, GetpetFoodinfoCallback);

        if (ResolutionAdapter.HasUnSafeArea)
        {
            var scale = this.myForm.GetScale();
            var safeArea = ResolutionAdapter.GetSafeArea();
            var offset = this.myForm.Pixel2View(safeArea.position);
            var offerH = offset.y;

            BG.sizeDelta = new Vector2(750, 120 + offerH);
            ScrollView.rectTransform().offsetMax = new Vector2(54, -(180 + offerH));          
        }


        UIMaskW = UIMask.rect.width;
        UIMaskH= UIMask.rect.height;
        ScrollViewTop = ScrollView.rectTransform().offsetMax.y;

        Debug.Log("UIMaskW:"+ UIMaskW+ "--UIMaskH:"+ UIMaskH+ "--ScrollViewTop:"+ ScrollViewTop);


        if (UserDataManager.Instance.isFirstCatEnt)
        {
            //正在进行猫的引导
            ScrollView.enabled = false;
        }
    }

    private void GetpetFoodinfoCallback(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetpetFoodinfoCallback---->" + result);

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.Getpetfoodinfo = JsonHelper.JsonToObject<HttpInfoReturn<Getpetfoodinfo>>(result);

                    if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.ShopBuyHuangyuandian)
                    {
                        //引导处于引导黄圆垫的时候，把黄圆垫的数据放在第一条

                        shopinfo = UserDataManager.Instance.Getpetfoodinfo.data.shoparr;

                        if (UserDataManager.Instance.isFirstCatEnt)
                        {
                            //如果现在正处于猫的引导期间，就把荒原垫的数据插到前面
                            for (int i = 0; i < shopinfo.Count; i++)
                            {
                                if (shopinfo[i].id == 6)
                                {
                                    //得到黄圆垫的数据后，把它插到最前面
                                    shopinfo.Insert(0, shopinfo[i]);

                                    shopinfo.RemoveAt(i + 1);//移除掉多余的那一条数据
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        shopinfo = UserDataManager.Instance.Getpetfoodinfo.data.shoparr;

                    }


                    if (shopinfo != null && shopinfo.Count > 0)
                    {
                        SpwanShopItem(shopinfo);
                        infinityGridLayoutGroup.SetAmount(shopinfo.Count);
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
                //UITipsMgr.Instance.PopupTips("There's no more mail", false);
                return;
            }
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.GetpetFoodinfo(page, GetFoodNewPageDataCallback);
        }
    }

    private void GetFoodNewPageDataCallback(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetpetFoodinfoCallback---->" + result);

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    HttpInfoReturn<Getpetfoodinfo> NewPageFoodinfo = JsonHelper.JsonToObject<HttpInfoReturn<Getpetfoodinfo>>(result);
                    if (NewPageFoodinfo != null)
                    {
                        UserDataManager.Instance.Getpetfoodinfo.data.shoparr.AddRange(NewPageFoodinfo.data.shoparr);
                        shopinfo = UserDataManager.Instance.Getpetfoodinfo.data.shoparr;
                    }
                    infinityGridLayoutGroup.SetCount(shopinfo.Count);
                }
            }, null);
        }
    }

    private void CloseUi(PointerEventData data)
    {
        //派发事件，关闭这个界面
        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), UiFormIndex);
        bar.onValueChanged.RemoveListener(ScrollbarChange);
    }


    public override void OnClose()
    {
        base.OnClose();
        infinityGridLayoutGroup.updateChildrenCallback = null;
        if (BackButton != null)
            UIEventListener.RemoveOnClickListener(BackButton.gameObject, CloseUi);


        for (int i=0;i< catshopItemList.Count;i++)
        {
            //调用这个方法移除所有实例出来的物体
            catshopItemList[i].GetComponent<CatShopItemFrom>().Dispose();
        }
    }


    #region 生成商店的商品物体
    List<CatShopItemFrom> catshopItemList;

    private void SpwanShopItem(List<shoparr> shopinfo)
    {
       
        int shopListCount = shopinfo.Count;
        if (catshopItemList==null)
        {
            catshopItemList = new List<CatShopItemFrom>();
            int len = 15;
            for (int i = 0; i < len; i++)
            {
                GameObject go = Instantiate(catshopItem);
                go.transform.SetParent(ScrollView.content.transform);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
                CatShopItemFrom catShopItem = go.GetComponent<CatShopItemFrom>();
                if (shopListCount > i)
                {                   
                    t_shop shop = GameDataMgr.Instance.table.GetcatShopId(shopinfo[i].id);
                    catShopItem.Init(shop,this, shopinfo[i].id);
                }
                catShopItem.gameObject.SetActive(true);
                catshopItemList.Add(catShopItem);
            } 
        }else
        {
            int len = 8;
            for (int i = 0; i < len; i++)
            {
                CatShopItemFrom catShopItem = catshopItemList[i];
                if (shopListCount > i)
                {
                    t_shop shop = GameDataMgr.Instance.table.GetcatShopId(shopinfo[i].id);
                    catShopItem.Init(shop,this, shopinfo[i].id);
                    catShopItem.gameObject.SetActive(true);
                }
                else
                {
                    catShopItem.gameObject.SetActive(false);
                }
            } 
        }
    }

    void UpdateChildrenCallback(int index, Transform trans)
    {
        if(shopinfo.Count > index)
        {
            trans.gameObject.SetActive(true);
            t_shop shopData = GameDataMgr.Instance.table.GetcatShopId(shopinfo[index].id);
            if (shopData != null)
                trans.GetComponent<CatShopItemFrom>().Init(shopData,this, shopinfo[index].id);
        }else
        {
            trans.gameObject.SetActive(false);
        }
    }
    #endregion

}

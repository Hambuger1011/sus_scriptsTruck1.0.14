using pb;
using System;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CatAnimalFrom : CatBaseForm
{

    private GameObject BackButton;
    private GameObject CatAnimalItem;
    private ScrollRect ScrollView;
    InfinityGridLayoutGroup infinityGridLayoutGroup;
    private RectTransform BG;
    int page = 1;
    private void Awake()
    {
        BackButton = transform.Find("Bg/Top/BG/BackButton").gameObject;
        CatAnimalItem = transform.Find("Bg/Item/CatAnimalItem").gameObject;
        ScrollView = transform.Find("Bg/Scroll View").GetComponent<ScrollRect>();
        infinityGridLayoutGroup = transform.Find("Bg/Scroll View/Viewport/Content").GetComponent<InfinityGridLayoutGroup>();
        BG = transform.Find("Bg/Top/BG").GetComponent<RectTransform>();
    }
    public override void OnOpen()
    {
        base.OnOpen();
        UiFormIndex = (int)CatFormEnum.CAT_ANIMAL;

        UIEventListener.AddOnClickListener(BackButton, CloseUi);
        
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.Getpetinfo(page,GetpetinfoCallbacke);

        if (ResolutionAdapter.HasUnSafeArea)
        {
            var scale = this.myForm.GetScale();
            var safeArea = ResolutionAdapter.GetSafeArea();
            var offset = this.myForm.Pixel2View(safeArea.position);
            var offerH = offset.y;

            BG.sizeDelta = new Vector2(750, 120 + offerH);
            ScrollView.rectTransform().offsetMax = new Vector2(54, -(183+offerH));
        }


    }


    public override void OnClose()
    {
        base.OnClose();
        page = 1;
        UIEventListener.RemoveOnClickListener(BackButton, CloseUi);
        bar.onValueChanged.RemoveListener(ScrollbarChange);
        infinityGridLayoutGroup.updateChildrenCallback = null;
       
        for (int i = 0; i < catAnimaltemList.Count; i++)
        {
            //调用这个方法移除所有实例出来的物体
            catAnimaltemList[i].GetComponent<CatAnimalItem>().Dispote();
        }
    }

    private void CloseUi(PointerEventData data)
    {
        //派发事件，关闭这个界面
        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), UiFormIndex);
    }

    #region 获得动物图鉴的数据
    List<GameObject> catAnimaltemList;

    private List<petarr> tem;
    public void GetpetinfoCallbacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetpetinfoCallbacke---->" + result);

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
            if (jo.code == 200)
            {
                UserDataManager.Instance.Getpetinfo = JsonHelper.JsonToObject<HttpInfoReturn<Getpetinfo>>(result);

                tem = UserDataManager.Instance.Getpetinfo.data.petarr;
  
                if(tem.Count>0)
                  SpwanCatAnimalItem(tem);

                    bar.onValueChanged.AddListener(ScrollbarChange);
                }
            }, null);
        }
    }

    public Scrollbar bar;
    private void ScrollbarChange(float ve)
    {
        //Debug.Log("值是：" + ScrollView.verticalNormalizedPosition);

        if (ScrollView.verticalNormalizedPosition < 0)
        {
            //Debug.Log("值是：" + ScrollView.verticalNormalizedPosition);
            UpdatePageData();
        }

    }

    private void UpdatePageData()
    {
        LOG.Info("滑动到了底部了，需要更新数据");
        page++;
        if (UserDataManager.Instance.Getpetinfo != null)
        {
            int count = UserDataManager.Instance.Getpetinfo.data.pages_total;
            //LOG.Info("最大页数是：" + shu);
            if (page > count)
            {
                //LOG.Info("没有更多的动物图鉴了");

                var Localization = GameDataMgr.Instance.table.GetLocalizationById(124);


                UITipsMgr.Instance.PopupTips(Localization, false);
                return;
            }
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.Getpetinfo(page,GetAnimalNewPageDataCallback);
        }
    }

    private void GetAnimalNewPageDataCallback(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetpetinfoCallbacke---->" + result);

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    HttpInfoReturn<Getpetinfo> addInfo = JsonHelper.JsonToObject<HttpInfoReturn<Getpetinfo>>(result);
                    if (addInfo !=null)
                    {
                        UserDataManager.Instance.Getpetinfo.data.petarr.AddRange(addInfo.data.petarr);
                        tem = UserDataManager.Instance.Getpetinfo.data.petarr;
                    }
                    infinityGridLayoutGroup.SetCount(tem.Count);
                }
            }, null);
        }
    }

    private void SpwanCatAnimalItem(List<petarr> tem)
    {
        if (catAnimaltemList == null)
        {
            catAnimaltemList = new List<GameObject>();
        }
        catAnimaltemList.Clear();

        int TemCount = tem.Count;//获得列表中总共有多少猫
        int ListPoolNum = 15; //规定超过多少个才使用复用列表

        if (TemCount<ListPoolNum)
        {
            //这里不使用复用列表
            for (int i = 0; i < TemCount; i++)
            {
                GameObject go = Instantiate(CatAnimalItem);
                go.transform.SetParent(ScrollView.content.transform);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
                go.SetActive(true);
                catAnimaltemList.Add(go);
            }             
        }
        else
        {
            for (int i = 0; i < ListPoolNum; i++)
            {
                GameObject go = Instantiate(CatAnimalItem);
                go.transform.SetParent(ScrollView.content.transform);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
                go.SetActive(true);
                catAnimaltemList.Add(go);
            }

            infinityGridLayoutGroup.SetAmount(TemCount);
            infinityGridLayoutGroup.updateChildrenCallback = UpdateChildrenCallback;
        }
    }

    void UpdateChildrenCallback(int index, Transform trans)
    {
        if (tem != null)
            trans.GetComponent<CatAnimalItem>().Inite(tem[index],index+1);
    }
    #endregion

}

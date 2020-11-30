using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CatStoryForm : CatBaseForm {

    private GameObject BackButton;
    private GameObject StoryItemBg;
    private ScrollRect ScrollView;
    private List<storypetarr> storyItem;
    InfinityGridLayoutGroup infinityGridLayoutGroup;
    private RectTransform BG;
    private void Awake()
    {
        BackButton = transform.Find("Bg/Top/BG/BackButton").gameObject;
        StoryItemBg = transform.Find("Bg/StoryItemBg").gameObject;
        infinityGridLayoutGroup = transform.Find("Bg/ScrollView/Viewport/Content").GetComponent<InfinityGridLayoutGroup>();
        ScrollView = transform.Find("Bg/ScrollView").GetComponent<ScrollRect>();
        BG = transform.Find("Bg/Top/BG").GetComponent<RectTransform>();

    }
    public override void OnOpen()
    {
        base.OnOpen();
        UiFormIndex = (int)CatFormEnum.CAT_STORY;

        UIEventListener.AddOnClickListener(BackButton, CloseUi);

        if (ResolutionAdapter.HasUnSafeArea)
        {
            var scale = this.myForm.GetScale();
            var safeArea = ResolutionAdapter.GetSafeArea();
            var offset = this.myForm.Pixel2View(safeArea.position);
            var offerH = offset.y;

            BG.sizeDelta = new Vector2(750, 120 + offerH);
            ScrollView.rectTransform().offsetMax = new Vector2(54, -(180 + offerH));
        }

        if (catStoryItemList == null)
        {
            catStoryItemList = new List<GameObject>();
        }
        catStoryItemList.Clear();

        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.Getpetstory(GetpetstoryCallBacke);
    }


    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(BackButton, CloseUi);

        for (int i=0;i< catStoryItemList.Count;i++)
        {
            catStoryItemList[i].GetComponent<CatStoryItem>().Disposte();
        }
    }


    private void CloseUi(PointerEventData data)
    {
        //派发事件，关闭这个界面
        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), UiFormIndex);
    }

    private void GetpetstoryCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetpetstoryCallBacke---->" + result);

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.Getpetstory = JsonHelper.JsonToObject<HttpInfoReturn<Getpetstory>>(result);

                    //UserDataManager.Instance.Getpetstory.data.storypetarr.AddRange(UserDataManager.Instance.Getpetstory.data.storypetarr);
                    storyItem = UserDataManager.Instance.Getpetstory.data.storypetarr;
                    SpwanStoryItem(storyItem);
                }
               
            }, null);
        }
    }

    #region 生成故事物体
    List<GameObject> catStoryItemList;

    private void SpwanStoryItem(List<storypetarr> storyItem)
    {
       

        //Debug.Log("数量："+ storyItem.Count);
        if (storyItem.Count>0)
        {
            ////获得表中有几个物体
            int ShopListCount = storyItem.Count;


            if (ShopListCount < 6)
            {
                //个数6个以内。不需要使用复用列表

                for (int i = 0; i < ShopListCount; i++)
                {
                    GameObject go = Instantiate(StoryItemBg);
                    go.transform.SetParent(ScrollView.content.transform);
                    go.transform.localScale = Vector3.one;
                    go.transform.localPosition = Vector3.zero;
                    go.SetActive(true);
                    catStoryItemList.Add(go);

                    CatStoryItem CatStoryItem = go.GetComponent<CatStoryItem>();
                    if (CatStoryItem != null)
                        CatStoryItem.Inite(storyItem[i]);
                }
            }
            else
            {
                //个数超过了6个。使用复用列表
                for (int i = 0; i < 6; i++)
                {
                    GameObject go = Instantiate(StoryItemBg);
                    go.transform.SetParent(ScrollView.content.transform);
                    go.transform.localScale = Vector3.one;
                    go.transform.localPosition = Vector3.zero;
                    go.SetActive(true);
                    catStoryItemList.Add(go);
                }

                infinityGridLayoutGroup.SetAmount(ShopListCount); //ShopListCount 总的数据数量
                infinityGridLayoutGroup.updateChildrenCallback = UpdateChildrenCallback;
            }
        }
       
    }

    void UpdateChildrenCallback(int index, Transform trans)
    {
        //Debug.Log("UpdateChildrenCallback: index=" + index + " name:" + trans.name);
        //Text text = trans.Find("Text").GetComponent<Text>();
        //text.text = index.ToString();

        //t_shop shopData = GameDataMgr.Instance.table.GetcatShopId(shopinfo[index].id);
        CatStoryItem CatStoryItem = trans.GetComponent<CatStoryItem>();
        if (CatStoryItem != null)
            CatStoryItem.Inite(storyItem[index]);
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UGUI;
using pb;

public class Activity : BaseUIForm {

    public GameObject ActivityItem,BackButton;
    public ScrollRect ScrollRect;

    private List<ActivityItemChile> ActivityItemList;
    private GameObject DownImageInit;
    private MainTopSprite MainTopSprite;

    private Image mTopBg;
    private Text mTopTitle;

    void Awake()
    {
        mTopBg = transform.Find("Canvas/BG/TopImage").GetComponent<Image>();
        mTopTitle = mTopBg.transform.Find("Text").GetComponent<Text>();
    }

    public override void OnOpen()
    {
        base.OnOpen();


        //【屏幕适配】
        float offect = XLuaHelper.UnSafeAreaNotFit(this.myForm, mTopBg.rectTransform(), 750, 92);
        mTopTitle.rectTransform().anchoredPosition = new Vector2(0, 5 - offect / 2);
        BackButton.transform.rectTransform().anchoredPosition = new Vector2(-318, 8 - offect / 2);
        ScrollRect.transform.rectTransform().offsetMax = new Vector2(22, -(119 + offect));






        MainTopSprite = CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop);
        //MainTopSprite.MainTopFalse();

        if(ActivityItemList==null)
           ActivityItemList = new List<ActivityItemChile>();
        
        ActivityItem.SetActive(false);
     
        SpawnActivityItem();

        UIEventListener.AddOnClickListener(BackButton, BackButtonOnClice);

        addMessageListener(EventEnum.TopMainFT, TopMainFT);

    }
    private void TopMainFT(Notification notification)
    {    
        MainTopSprite.MainTopFalse();
    }
    private void BackButtonOnClice(PointerEventData data)
    {
        CUIManager.Instance.CloseForm(UIFormName.Activity);

        MainTopSprite.MainTopTrue();
    }
    private void SpawnActivityItem()
    {
        GameDataMgr.Instance.table.ActivityData();//储存表的所有数据
        List<t_Activity> ActivityStateList = GameDataMgr.Instance.table.ActivityStateListReturnCount();

       
        if (ActivityStateList.Count== 0)
        {
            LOG.Info("活动表中没有数据");
            return;
        }
        LOG.Info("开启的活动个数是：" + ActivityStateList.Count);

        for (int i=0;i< ActivityStateList.Count; i++)
        {

            t_Activity data1 = ActivityStateList[i];

            GameObject go1 = Instantiate(ActivityItem, ScrollRect.content);
            go1.SetActive(true);
            
            ActivityItemChile item = go1.GetComponent<ActivityItemChile>();
            item.ActivityItemChilInit(data1);

            ActivityItemList.Add(item);
        }

        //if (tableCount%2==0)
        //{
        //    //表中的数据是偶数
        //    int Count = tableCount/2;
        //    for (int i = 0; i < Count; i++)
        //    {
        //        GameObject go = Instantiate(ActivityItem, ScrollRect.content.transform);
        //        go.name = (i).ToString();
        //        go.SetActive(true);
        //        //go.GetComponent<ActivityItem>().ActivityItemInit();

        //        ActivityItem item = go.GetComponent<ActivityItem>();
        //        item.ActivityItemInit();
        //        ActivityItemList.Add(item);

        //        //if (i== Count-1)
        //        //{
        //        //    DownImageInit = Instantiate(DownImage, ScrollRect.content.transform);
        //        //    DownImageInit.SetActive(true);
        //        //}
        //    }
        //}
        //else
        //{
        //    //表中的数据是奇数
        //    int Count = tableCount / 2+1;
        //    for (int i = 0; i < Count; i++)
        //    {
        //        GameObject go = Instantiate(ActivityItem, ScrollRect.content.transform);
        //        go.name = (i).ToString();
        //        go.SetActive(true);
        //        //go.GetComponent<ActivityItem>().ActivityItemInit();

        //        ActivityItem item = go.GetComponent<ActivityItem>();
        //        item.ActivityItemInit();
        //        ActivityItemList.Add(item);

        //        //if (i == Count - 1)
        //        //{
        //        //    DownImageInit = Instantiate(DownImage, ScrollRect.content.transform);
        //        //    DownImageInit.SetActive(true);
        //        //}
        //    }
        //}
    }

    public override void OnClose()
    {
        base.OnClose();

        if (ActivityItemList!=null)
        {
            for (int i=0;i< ActivityItemList.Count;i++)
            {
                ActivityItemList[i].DisPoste();
            }
        }

        //Destroy(DownImageInit);

        UIEventListener.AddOnClickListener(BackButton, BackButtonOnClice);
    }
}

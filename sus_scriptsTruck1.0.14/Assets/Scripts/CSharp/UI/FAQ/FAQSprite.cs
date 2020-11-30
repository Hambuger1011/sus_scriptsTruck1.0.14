using pb;
using System;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FAQSprite : BaseUIForm {

    public ScrollRect scroRect;
    private GameObject Item,Image;
    private int Numbers = 0;
    private int IDnumber = 0;
    private List<FAQItemSprite> ItemList;

    private RectTransform top, Frame;
    private GameObject Close;


    public override void OnOpen()
    {
        base.OnOpen();
        //FAQ存储表的数据入字典
        GameDataMgr.Instance.table.FAQDialogData();

        FindGameObject();
        Init();

        //【屏幕适配】
        float offect = XLuaHelper.UnSafeAreaNotFit(this.myForm, top, 0, 100);
        Frame.offsetMax = new Vector2(0, -offect);


    }

    private void FindGameObject()
    {
        top = transform.Find("top").GetComponent<RectTransform>();
        Close = transform.Find("top/Close").gameObject;
        Item = transform.Find("Prefb/Item").gameObject;
        Frame = transform.Find("Frame").GetComponent<RectTransform>();

        UIEventListener.AddOnClickListener(Close, CloseButtonOn);

    }

    public void Init()
    {
       
        if (ItemList==null)
        {
            ItemList = new List<FAQItemSprite>();          
        }
        IDnumber++;    
        t_BookFAQ data = GameDataMgr.Instance.table.GetFAQDialogById(IDnumber);

        //Debug.Log("IDnumber:"+ IDnumber+ "--data:"+ data);

        if (data != null)
        {
            GameObject go = Instantiate(Item);
            go.transform.SetParent(scroRect.content.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.SetActive(true);
            FAQItemSprite Faq = go.GetComponent<FAQItemSprite>();
            Faq.InitFAQItem(data);
            ItemList.Add(Faq);

            Init();
        }
    }

    private void CloseButtonOn(PointerEventData data)
    {
        CUIManager.Instance.CloseForm(UIFormName.FAQ);
    }
   
    public override void OnClose()
    {
        base.OnClose();

        UIEventListener.RemoveOnClickListener(Close, CloseButtonOn);


        if (ItemList!=null)
        {
            for (int i=0;i< ItemList.Count;i++)
            {
                ItemList[i].Disposte();
            }
        }
    }
}

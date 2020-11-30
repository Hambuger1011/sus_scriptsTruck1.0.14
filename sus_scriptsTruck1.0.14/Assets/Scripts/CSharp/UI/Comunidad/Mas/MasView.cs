using SuperScrollView;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasView
{
    private MasCtrl MasCtrl;
    private Transform transform;

    private ScrollRect ScrollView;
    private LoopGridView mLoopGridView;
    private List<MasBookSelf> MasBookSelf;

    public MasView(MasCtrl MasCtrl)
    {
        this.MasCtrl = MasCtrl;
        this.transform = MasCtrl.transform;

        if (MasBookSelf==null)
        {
            MasBookSelf = new List<MasBookSelf>();
        }

        FindGameObject();
    }

    public void FindGameObject()
    {
        ScrollView = DisplayUtil.GetChildComponent<ScrollRect>(transform.gameObject, "ScrollView");
        mLoopGridView = DisplayUtil.GetChildComponent<LoopGridView>(transform.gameObject, "ScrollView");
        mLoopGridView.ScrollRect.onValueChanged.AddListener(OnBookScrollChanged);
        mLoopGridView.InitGridView(0, OnGetItemByRowColumn);
    }

    public void Close()
    {

    }


    LoopGridViewItem OnGetItemByRowColumn(LoopGridView gridView, int itemIndex, int row, int column)
    {
        /*
            get a new item. Every item can use a different prefab, 
            the parameter of the NewListViewItem is the prefab’name. 
            And all the prefabs should be listed in ItemPrefabList in LoopGridView Inspector Setting
            */
        LoopGridViewItem item = gridView.NewListViewItem("BookIconPfb");

        MasBookSelf itemScript = item.GetComponent<MasBookSelf>();//get your own component
                                                                // IsInitHandlerCalled is false means this item is new created but not fetched from pool.
        if (item.IsInitHandlerCalled == false)
        {
            item.IsInitHandlerCalled = true;
            //itemScript.Init();// here to init the item, such as add button click event listener.
        }
        //update the item’s content for showing, such as image,text.
        itemScript.SetItemData(MasCtrl.MasDB.WriterBookList()[itemIndex], itemIndex, row, column);
        return item;


        ////Debug.Log("index:"+ index+"--总数:"+ MasCtrl.MasDB.WriterBookList().Count);
        //if (trans != null)
        //{
        //    MasBookSelf masBookSelf = trans.GetComponent<MasBookSelf>();

        //    if(index< MasCtrl.MasDB.WriterBookList().Count)
        //      masBookSelf.Init(MasCtrl.MasDB.WriterBookList()[index],index);          
        //}
    }

    public void SpwanBook(int page = 0)
    {
        if (page > 0)
        {
            this.m_page = page;
            m_waitBookRefresh = false;
        }

        var count = MasCtrl.MasDB.WriterBookList().Count;
        mLoopGridView.SetListItemCount(count);// UserDataManager.Instance.GetWriterHotBookList.data.total);
        
    }
    


    bool m_waitUiRefresh = false;//等待ui刷新
    bool m_waitBookRefresh = false;//等待消息返回
    int m_page = 0;
    private void OnBookScrollChanged(Vector2 value)
    {
        if (m_waitBookRefresh)
        {
            return;
        }
        if (m_waitUiRefresh)
        {
            if (value.y >= 0.1f)
            {
                m_waitUiRefresh = false;
            }
        }
        else
        {
            if (value.y < 0.1f)
            {
                m_waitUiRefresh = true;//等待ui刷新
                m_waitBookRefresh = true;//等待消息返回
                LOG.Info("RequestBook:" + (m_page + 1));
                this.MasCtrl.MasLogic.RequestBook(m_page + 1);
            }
        }

    }

}

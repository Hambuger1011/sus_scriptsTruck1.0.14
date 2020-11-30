using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameCore.UI;
using System;

public class BusquedaView
{
    private BusquedaCtrl BusquedaCtrl;
    private UIVirtualList InfinityGridLayoutGroup;
    private RectTransform topTrans;
    InputField inputSearch;


    public BusquedaView(BusquedaCtrl BusquedaCtrl)
    {
        this.BusquedaCtrl = BusquedaCtrl;



        FindGameObject();
        AddListen();

        
        if (ResolutionAdapter.HasUnSafeArea)
        {
            var scale = BusquedaCtrl.myForm.GetScale();
            var safeArea = ResolutionAdapter.GetSafeArea();
            var offset = BusquedaCtrl.myForm.Pixel2View(safeArea.position);
            var size = topTrans.sizeDelta;
            size.y += offset.y;
            topTrans.sizeDelta = size;
        }
        

    }

    private void FindGameObject()
    {
        topTrans = (RectTransform)BusquedaCtrl.transform.Find("Bg/Top").transform;
        InfinityGridLayoutGroup = BusquedaCtrl.transform.Find("Bg/BGScrollView/Viewport/Content/BookGroup").GetComponent<UIVirtualList>();
        inputSearch = BusquedaCtrl.transform.Find("Bg/Top/InputField").GetComponent<InputField>();
        InfinityGridLayoutGroup.scrollRect.onValueChanged.AddListener(OnBookScrollChanged);
        inputSearch.onEndEdit.AddListener(OnSearchBookName);
    }

    private void OnSearchBookName(string value)
    {
        BusquedaCtrl.BusquedaLogic.SetSearchData(BusquedaCtrl.BusquedaLogic.tagFlag, value);
        BusquedaCtrl.BusquedaLogic.RequestBook(1);
    }

    private void AddListen()
    {
        InfinityGridLayoutGroup.onItemRefresh = UpdateChildrenCallback;
        
    }

    bool m_waitUiRefresh = false;
    bool m_waitBookRefresh = false;
    int m_page = 0;
    /// <summary>
    /// 生成书本搜索选项物体
    /// </summary>

    public void SpwanGenresPrefb(int page = 0)
    {
        if(page > 0)
        {
            this.m_page = page;
            m_waitBookRefresh = false;
        }

        var keyword = inputSearch.text.Trim();
        List<BusquedaBookInfo> BookList = BusquedaCtrl.BusquedaModle.allBookList;
        BusquedaCtrl.BusquedaModle.MyPickBook = BookList;

        Debug.Log("书本数量：" + BusquedaCtrl.BusquedaModle.MyPickBook.Count);
        InfinityGridLayoutGroup.SetItemCount(BusquedaCtrl.BusquedaModle.MyPickBook.Count);
    }



    public void Close()
    {
        InfinityGridLayoutGroup.onItemRefresh -= UpdateChildrenCallback;
    }

    public void UpdateChildrenCallback(UIVirtualList.Row row)
    {
        int index = row.itemIndex;
        Transform trans = row.rect;
        //Debug.Log("index:"+ index+"--name:"+trans.name);
        if (trans != null)
        {
            BusquedaBookSelf TypeSelectionItem = trans.GetComponent<BusquedaBookSelf>();
            //t_BookDetails BookDetails = BookItemManage.Instance.GetBookDetails(BusquedaCtrl.BusquedaModle.MyPickBook[index].id);
            BusquedaBookInfo Info = BusquedaCtrl.BusquedaModle.MyPickBook[index];
            //t_BookDetails BookDetails = BookItemManage.Instance.GetBookDetails(Info.id);
            TypeSelectionItem.init(Info);
        }

    }

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
                this.BusquedaCtrl.BusquedaLogic.RequestBook(m_page + 1);
            }
        }

    }

}

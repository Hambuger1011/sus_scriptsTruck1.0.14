using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using iTween;
using DG.Tweening;
using GameCore.UI;

public class TypeSelectionFormView
{
    private TypeSelectionForm TypeSelectionForm;
    //private ScrollRect ScrollView, NoUseScrollView,CententScrollRect;
    //private GameObject BookItem;
    //private List<TypeSelectionItem> TypeSelectionItem, NoTypeSelectionItem;
    private UIVirtualList InfinityGridLayoutGroup;
    //private RectTransform CententRect;
    //private RectTransform ScrollViewRect, NoUseScrollViewRect,CententRect, BookGroupRect;

    //private bool mCanScroll = false;//是否可以拖动（条件就是，列表内容高度超过视窗的高度）

    private void FindGameObject()
    {
        var root = TypeSelectionForm.transform;
        //BookItem = root.Find("BG/Prefb/Item").gameObject;
        InfinityGridLayoutGroup = root.Find("BG/VerticalScroll/Viewport/VLayout/BookGroup").GetComponent<UIVirtualList>();
        //InfinityGridLayoutGroup = root.Find("BG/VerticalScroll/Viewport/VLayout/BookGroup").GetComponent<InfinityGridLayoutGroup>();

        //CententScrollRect = TypeSelectionForm.transform.Find("BG").GetComponent<ScrollRect>();
        //CententRect = CententScrollRect.transform.Find("VerticalScroll").GetComponent<RectTransform>();

        //BookGroupRect = CententScrollRect.transform.Find("VerticalScroll/BookGroup").GetComponent<RectTransform>();
        //ScrollView = BookGroupRect.Find("ScrollView").GetComponent<ScrollRect>();
        //ScrollViewRect = ScrollView.GetComponent<RectTransform>();
        //NoUseScrollView = BookGroupRect.Find("NoUseScrollView").GetComponent<ScrollRect>();
        //NoUseScrollViewRect = NoUseScrollView.GetComponent<RectTransform>();
    }


    public TypeSelectionFormView(TypeSelectionForm TypeSelectionForm)
    {
        this.TypeSelectionForm = TypeSelectionForm;
        FindGameObject();
        AddListen();
        

        //CententRect.sizeDelta = new Vector2(750, cententOfferH);
        //BookGroupRect.sizeDelta = new Vector2(750, boookOfferH);


        //ScrollView.enabled = false;
        //NoUseScrollView.enabled = false;

        //if (TypeSelectionItem == null)
        //    TypeSelectionItem = new List<TypeSelectionItem>();
        //if (NoTypeSelectionItem == null)
        //    NoTypeSelectionItem = new List<TypeSelectionItem>();
    }

    private void AddListen()
    {
        InfinityGridLayoutGroup.onItemRefresh = TypeSelectionUpdateChildrenCallback;

        //CententScrollRect.onValueChanged.AddListener(CententScrollRectUpdateHandler);
        //ScrollView.onValueChanged.AddListener(ScrollViewUpdateHandler);
        //NoUseScrollView.onValueChanged.AddListener(NoUseScrollViewHandler);
    }

    //private void CententScrollRectUpdateHandler(Vector2 rect)
    //{
    //    if (CententRect.anchoredPosition.y>= 540 && mCanScroll)
    //    {
    //        if (ScrollView.gameObject.activeSelf && !ScrollView.enabled)
    //        {
    //            ScrollView.enabled = true;
    //            ScrollView.velocity = CententScrollRect.velocity;
    //        }

    //        if (NoUseScrollView.gameObject.activeSelf && !NoUseScrollView.enabled)
    //        {
    //            NoUseScrollView.enabled = true;
    //            NoUseScrollView.velocity = CententScrollRect.velocity;
    //        }

    //        if (CententScrollRect.enabled)
    //            CententScrollRect.enabled = false;
    //    }
    //    else
    //    {
    //        if (ScrollView.gameObject.activeSelf && ScrollView.enabled)
    //            ScrollView.enabled = false;

    //        if (NoUseScrollView.gameObject.activeSelf && NoUseScrollView.enabled)
    //            NoUseScrollView.enabled = false;
    //    }
    //}

    //private void ScrollViewUpdateHandler(Vector2 rect)
    //{
    //    if (rect.y > 1.00f && ScrollView.enabled && ScrollView.velocity.y < 0f)
    //    {
    //        ScrollView.verticalNormalizedPosition = 1.0f;
    //        ScrollView.enabled = false;
    //        CententRect.anchoredPosition = new Vector2(CententRect.anchoredPosition.x, 539);
    //        CententScrollRect.enabled = true;
    //        CententScrollRect.velocity = ScrollView.velocity;
    //    }     
    //    //Debug.Log("------ScrollView.velocity.y------>>"+ NoUseScrollView.preferredHeight + "<===???===>" + rect.y+  "<------->" + ScrollView.velocity.y);
    //}

    //private void NoUseScrollViewHandler(Vector2 rect)
    //{
    //    if (rect.y > 1.00f && NoUseScrollView.enabled && NoUseScrollView.velocity.y < 0f)
    //    {
            
    //        NoUseScrollView.verticalNormalizedPosition = 1.0f;
    //        NoUseScrollView.enabled = false;
    //        CententRect.anchoredPosition = new Vector2(CententRect.anchoredPosition.x, 539);
    //        CententScrollRect.enabled = true;
    //        CententScrollRect.velocity = NoUseScrollView.velocity;
    //    }

    //    //Debug.Log("------ScrollView.velocity.y---+++++++++++++--->>"+ NoUseScrollView.preferredHeight + "<===???===>" + rect.y +"<---->" + NoUseScrollView.velocity.y);
    //}

    public void SpwanItem()
    {

        //if (TypeSelectionForm.TypeSelectionDB.SearchMyBook.Count >= 8)
        {
            //使用复用列表
            //ScrollView.gameObject.SetActive(true);
            //NoUseScrollView.gameObject.SetActive(false);

            //CententScrollRect.verticalNormalizedPosition = 1.0f;
            //CententScrollRect.enabled = true;
            //ScrollView.enabled = false;

            //if (TypeSelectionItem != null&& TypeSelectionItem.Count==0)
            //{
            //    for (int i = 0; i < 8; i++)
            //    {
            //        GameObject go = GameObject.Instantiate(BookItem);
            //        go.SetActive(true);
            //        go.transform.SetParent(InfinityGridLayoutGroup.transform);
            //        go.transform.localPosition = Vector3.zero;
            //        go.transform.localScale = Vector3.one;


            //        TypeSelectionItem TypeSelectionItemgo = go.transform.GetComponent<TypeSelectionItem>();
            //        TypeSelectionItem.Add(TypeSelectionItemgo);
            //    }
            //}

            //mCanScroll = true;


            InfinityGridLayoutGroup.SetItemCount(TypeSelectionForm.TypeSelectionDB.SearchMyBook.Count);
#if ENABLE_DEBUG
            HashSet<int> set = new HashSet<int>();
            foreach(var itr in TypeSelectionForm.TypeSelectionDB.SearchMyBook)
            {
                if (!set.Add(itr.id))
                {
                    UIAlertMgr.Instance.Show("fuck", "数据里书本重复:" + itr);
                }
            }
#endif

        }
        //else
        //{
        //    //ScrollView.gameObject.SetActive(false);
        //    //NoUseScrollView.gameObject.SetActive(true);

           
        //    //CententScrollRect.verticalNormalizedPosition = 1.0f;
        //    //CententScrollRect.enabled = true;

        //    //NoUseScrollView.enabled = false;

        //    DestroyItem();
        //    Debug.Log("生成书本数量：" + TypeSelectionForm.TypeSelectionDB.SearchMyBook.Count);

        //    for (int i = 0; i < TypeSelectionForm.TypeSelectionDB.SearchMyBook.Count; i++)
        //    {
        //        GameObject go = GameObject.Instantiate(BookItem);
        //        go.SetActive(true);
        //        go.transform.SetParent(NoUseScrollView.content.transform);
        //        go.transform.localPosition = Vector3.zero;
        //        go.transform.localScale = Vector3.one;

        //        BookNotUserInfo bookInfo = TypeSelectionForm.TypeSelectionDB.SearchMyBook[i];
        //        t_BookDetails BookDetails = BookItemManage.Instance.GetBookDetails(bookInfo.id);
        //        TypeSelectionItem TypeSelectionItemgo = go.transform.GetComponent<TypeSelectionItem>();
        //        TypeSelectionItemgo.init(BookDetails, bookInfo.read_count);
        //        NoTypeSelectionItem.Add(TypeSelectionItemgo);
        //    }

        //    //mCanScroll = NoUseScrollView.content.rect.height > NoUseScrollView.viewport.rect.height;
        //}


        //Debug.Log(" +++++++++++++--->> " + NoUseScrollView.content.rect + "+++++++++++++--->>"+ NoUseScrollView.viewport.rect);

    }

    //private void DestroyItem()
    //{
    //    if (NoTypeSelectionItem != null)
    //    {
    //        for (int i=0;i< NoTypeSelectionItem.Count;i++)
    //        {
    //            NoTypeSelectionItem[i].DestroyGameObject();
    //        }

    //        NoTypeSelectionItem.Clear();
    //    }
    //}

    public void Close()
    {
        //if (TypeSelectionItem != null)
        //{
        //    for (int i = 0; i < TypeSelectionItem.Count; i++)
        //    {
        //        TypeSelectionItem[i].DestroyGameObject();
        //    }

        //    TypeSelectionItem.Clear();
        //}

        //if (NoTypeSelectionItem != null)
        //{
        //    for (int i = 0; i < NoTypeSelectionItem.Count; i++)
        //    {
        //        NoTypeSelectionItem[i].DestroyGameObject();
        //    }

        //    NoTypeSelectionItem.Clear();
        //}

        InfinityGridLayoutGroup.onItemRefresh -= TypeSelectionUpdateChildrenCallback;

    }

    public void TypeSelectionUpdateChildrenCallback(UIVirtualList.Row row)
    {
        var trans = row.rect;
        var index = row.itemIndex;
        //Debug.Log("index:"+ index+"--name:"+trans.name);
        if (trans != null&& index< TypeSelectionForm.TypeSelectionDB.SearchMyBook.Count)
        {
            BookNotUserInfo bookInfo = TypeSelectionForm.TypeSelectionDB.SearchMyBook[index];
            t_BookDetails BookDetails = BookItemManage.Instance.GetBookDetails(bookInfo.id);
            TypeSelectionItem TypeSelectionItemgo = trans.GetComponent<TypeSelectionItem>();
            TypeSelectionItemgo.init(BookDetails, bookInfo.read_count);
        }
           
    }
    
}

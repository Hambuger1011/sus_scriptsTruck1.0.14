using pb;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class BusquedaLogic
{
    private BusquedaCtrl BusquedaCtrl;
    private List<GeneroPrefb> GeneroPrefbList;
   
    public BusquedaLogic(BusquedaCtrl BusquedaCtrl)
    {
        this.BusquedaCtrl = BusquedaCtrl;
        
    }

    public void Init()
    {

        m_curPage = 0;
        m_maxPage = 1;
        if (GeneroPrefbList == null)
            GeneroPrefbList = new List<GeneroPrefb>();

        SpwanGenresPrefb();
        RequestBook(1);
    }

    int m_curPage = 0;
    int m_maxPage = 1;
    public void RequestBook(int page)
    {
        if(m_curPage >= page)//已经请求过
        {
            LOG.Warn("已经请求过：m_curPage=" + m_curPage);
            return;
        }
        if(page > m_maxPage)//超过最大页码数
        {
            LOG.Warn("超过最大页码数：m_maxPage=" + m_maxPage);
            return;
        }
        m_curPage = page;
        GetBusquedaLogicData(page);
    }

    public long tagFlag { private set; get; }
    public string title { private set; get; }
    public void SetSearchData(long tagFlag,string title)
    {
        if(tagFlag == this.tagFlag && this.title == title)
        {
            return;
        }
        this.tagFlag = tagFlag;
        this.title = title;
        this.m_curPage = 0;
    }

    int m_requestDataSeq = 0;
    private void GetBusquedaLogicData(int page)
    {
        ////UINetLoadingMgr.Instance.Show();
        var tags = BitUtils.Join(",", tagFlag);

        int seq = ++m_requestDataSeq;
        GameHttpNet.Instance.GetWriterBookList(m_curPage, tags, title, (arg)=>
        {
            if(seq != m_requestDataSeq)
            {
                return;
            }
            this.GetBookNotUserCallBacke(page, arg);
        });
    }
    private void GetBookNotUserCallBacke(int page, object arg)
    {
        if(BusquedaCtrl == null)
        {
            return;
        }

        ////UINetLoadingMgr.Instance.Close();
        string result = arg.ToString();
        LOG.Info("----api_getBookNotUser---->" + result);
        if (result.Equals("error"))
        {
            return;
        }
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            if (jo.code == 200)
            {
                var info = JsonHelper.JsonToObject<HttpInfoReturn<BusquedaBookData>>(result);
                var data = info.data;
                if(data.current_page != page)//页码不匹配
                {
                    return;
                }
                if (data.per_page <= 0)
                {
                    data.per_page = 1;
                }
                this.m_maxPage = (data.total + data.per_page) / data.per_page;
                if (page > 1)
                {
                    BusquedaCtrl.BusquedaModle.allBookList.AddRange(info.data.data);
                }
                else
                {
                    BusquedaCtrl.BusquedaModle.allBookList = info.data.data;
                }
                BusquedaCtrl.BusquedaView.SpwanGenresPrefb(page);
            }
        }
    }
    /// <summary>
    /// 生成书本搜索选项物体
    /// </summary>

    private void SpwanGenresPrefb()
    {
        int MaxCount = (int)BusquedaType.End;

        string[] Namelist = BusquedaCtrl.BusquedaModle.GeneroPrefbName;

        for (int i=0;i<MaxCount;i++)
        {
            GameObject go = GameObject.Instantiate(BusquedaCtrl.GenresPrefb);
            go.SetActive(true);
            go.transform.SetParent(BusquedaCtrl.GenresText.transform);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;

            GeneroPrefb GeneroPrefb = go.GetComponent<GeneroPrefb>();
            GeneroPrefb.Init(Namelist[i], BusquedaCtrl,i);
            GeneroPrefbList.Add(GeneroPrefb);

        }

    }
    public void CloseOnButton(PointerEventData data)
    {
        CUIManager.Instance.CloseForm(UIFormName.Busqueda);
    }

    public void GetBusquedaBooke(int tagIndex)
    {
        var tag = BitUtils.SetBit64Mask(tagFlag, tagIndex, true);
        this.SetSearchData(tag, this.title);
        this.RequestBook(1);
    }


    public void RemoveBookId(int tagIndex)
    {
        var tag = BitUtils.SetBit64Mask(tagFlag, tagIndex, false);
        this.SetSearchData(tag, this.title);
        this.RequestBook(1);
    }


    public void DestryGameObject()
    {
        if (GeneroPrefbList!=null)
        {
            for (int i=0;i< GeneroPrefbList.Count;i++)
            {
                GeneroPrefbList[i].DestryGameObject();
            }
        }
    }
}

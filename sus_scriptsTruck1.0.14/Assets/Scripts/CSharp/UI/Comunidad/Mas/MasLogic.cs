using System;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MasLogic
{
    private MasCtrl MasCtrl;
    private int TypeEnd = 0;
    
    private Scrollbar ScrollbarVertical;
    private Transform transform;
    private ScrollRect ScrollView;
    private Text TitleTxt;





    public MasLogic(MasCtrl MasCtrl)
    {
        transform = MasCtrl.transform;
        this.MasCtrl = MasCtrl;

        TypeEnd = UserDataManager.Instance.MasOpenType;
        ClenaData();
        FindGameObject();

        if (TypeEnd == 1)
        {
            TitleTxt.text = CTextManager.Instance.GetText(283);
        }
        else
        {
            TitleTxt.text = CTextManager.Instance.GetText(284);
        }
      }

    public void Init()
    {
        RequestBook(1);
    }

    private void FindGameObject()
    {
        ScrollbarVertical = DisplayUtil.GetChildComponent<Scrollbar>(transform.gameObject, "ScrollbarVertical");
        ScrollView = DisplayUtil.GetChildComponent<ScrollRect>(transform.gameObject, "ScrollView");
        TitleTxt = DisplayUtil.GetChildComponent<Text>(transform.gameObject, "Title");
    }

    public void Close()
    {
    }

    private void ClenaData()
    {
        if (UserDataManager.Instance.GetWriterHotBookList != null)
        {
            UserDataManager.Instance.GetWriterHotBookList.data.data.Clear();
        }
    }

    public void CloseButtonOnclicke(PointerEventData data)
    {
        Debug.Log("MastClose被点击");
        CUIManager.Instance.CloseForm(UIFormName.ComuniadaMas);
    }
    

    int m_curPage = 0;
    int m_maxPage = 1;
    public void RequestBook(int page)
    {
        if (m_curPage >= page)//已经请求过
        {
            return;
        }
        if (page > m_maxPage)//超过最大页码数
        {
            return;
        }
        m_curPage = page;
        RequestBookdata(page);
    }
    private void RequestBookdata(int page)
    {
        ////UINetLoadingMgr.Instance.Show();
        if (TypeEnd == 1)
        {

            //热门书本数据
            GameHttpNet.Instance.GetWriterHotBookList(page, (arg) =>
            {
                this.GetWriterHotBookListCallBacke(page, arg);
            });

        }
        else
        {
            //最新书本数据
            GameHttpNet.Instance.GetWriterNewBookList(page, (arg) =>
            {
                this.GetWriterHotBookListCallBacke(page, arg);
            });
        }
    }
    private void GetWriterHotBookListCallBacke(int page, object arg)
    {
        if(transform == null)
        {
            return;
        }
        ////UINetLoadingMgr.Instance.Close();
        string result = arg.ToString();
        LOG.Info("----GetWriterIndexCallBacke---->" + result);
        if (result.Equals("error"))
        {
            return;
        }
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            if (jo.code == 200)
            {
                var info = JsonHelper.JsonToObject<HttpInfoReturn<GetWriterHotBookList>>(arg.ToString());
                var data = info.data;
                if (data.current_page != page)//页码不匹配
                {
                    return;
                }

                UserDataManager.Instance.GetWriterHotBookList = info;
                var list = MasCtrl.MasDB.WriterBookList();
                if (page > 1)
                {
                    list.AddRange(info.data.data);
                }
                else
                {
                    if(data.per_page <= 0)
                    {
                        data.per_page = 1;
                    }
                    this.m_maxPage = (data.total + data.per_page) / data.per_page;
                    list.Clear();
                    list.AddRange(info.data.data);
                }

                Debug.Log("书本数量：" + MasCtrl.MasDB.WriterBookList().Count);
                MasCtrl.MasView.SpwanBook(page);
            }
        }
    }
}

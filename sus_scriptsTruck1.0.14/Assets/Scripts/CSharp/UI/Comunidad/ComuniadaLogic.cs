using System;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using XLua;

public class ComuniadaLogic
{
    private ComuniadaCtrl ComuniadaCtrl;
    private bool ComunidadOpen = false;
    private List<ComuniadaBookSelf> ComuniadaBookSelfList;
  public ComuniadaLogic(ComuniadaCtrl ComuniadaCtrl)
    {
        this.ComuniadaCtrl = ComuniadaCtrl;

        if (ComuniadaBookSelfList == null)
            ComuniadaBookSelfList = new List<ComuniadaBookSelf>();
    }

    public void Init()
    {
        ComuniadaCtrl.ComuniadaModle.TopButtonOff.Add(ComuniadaCtrl.ComunidadButtonOFF);
        ComuniadaCtrl.ComuniadaModle.TopButtonOn.Add(ComuniadaCtrl.ComunidadButtonON);
        ComuniadaCtrl.ComuniadaModle.TopButtonOff.Add(ComuniadaCtrl.FavoritosButtonOFF);
        ComuniadaCtrl.ComuniadaModle.TopButtonOn.Add(ComuniadaCtrl.FavoritosButtonON);

        ComunidadButtonOnClicke(null);          
    }

    public void Close()
    {
        if (ComuniadaBookSelfList!=null)
        {
            for (int i=0;i< ComuniadaBookSelfList.Count;i++)
            {
                ComuniadaBookSelfList[i].DestroyGameObject();
            }
        }
    }
    /// <summary>
    /// 点击综合性书籍的按钮（顶部左边）
    /// </summary>
    /// <param name="data"></param>
    public void ComunidadButtonOnClicke(PointerEventData data)
    {
        if (ComuniadaCtrl.ComuniadaModle.TopButtonOn == null) return;
        if (ComuniadaCtrl.ComuniadaModle.TopButtonOff == null) return;

        ComuniadaCtrl.ComuniadaModle.TopButtonOn[0].SetActive(true);
        ComuniadaCtrl.ComuniadaModle.TopButtonOff[0].SetActive(false);
        ComuniadaCtrl.ComuniadaModle.TopButtonOn[1].SetActive(false);
        ComuniadaCtrl.ComuniadaModle.TopButtonOff[1].SetActive(true);

        ComuniadaCtrl.ComunidadScrollView.SetActive(true);
        ComuniadaCtrl.BusquedaButton.SetActive(true);
        OpenFavoritosScrollView(false);

        if (!ComunidadOpen)
        {
            ComunidadOpen = true;

            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.GetWriterIndex(GetWriterIndexCallBacke);

           
        }
    }

    private void GetWriterIndexCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetWriterIndexCallBacke---->" + result);
        if (result.Equals("error"))
        {
            return;
        }
        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo != null)
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.WriterIndexInfo = JsonHelper.JsonToObject<HttpInfoReturn<WriterIndexInfo>>(arg.ToString());
                    SetHistoriasBookShelfData();
                    SetNuevasBookShelfData();

                }
            }
        }, null);
    }


    void OpenFavoritosScrollView(bool isOn)
    {
        var luaenv = XLuaManager.Instance.GetLuaEnv();
        var res = luaenv.DoString(@"
return function(gameObject, isOn)
    logic.StoryEditorMgr:OpenFavoritosScrollView(gameObject, isOn)
end");
        using (var func = (LuaFunction)res[0])
        {
            func.Action<GameObject,bool>(ComuniadaCtrl.FavoritosScrollView, isOn);
        }
    }
    /// <summary>
    /// 点击喜欢书籍的按钮（顶部右边）
    /// </summary>
    /// <param name="data"></param>
    public void FavoritosButtonOnClicke(PointerEventData data)
    {
        if (ComuniadaCtrl.ComuniadaModle.TopButtonOn == null) return;
        if (ComuniadaCtrl.ComuniadaModle.TopButtonOff == null) return;

        ComuniadaCtrl.ComuniadaModle.TopButtonOn[0].SetActive(false);
        ComuniadaCtrl.ComuniadaModle.TopButtonOff[0].SetActive(true);
        ComuniadaCtrl.ComuniadaModle.TopButtonOn[1].SetActive(true);
        ComuniadaCtrl.ComuniadaModle.TopButtonOff[1].SetActive(false);

        ComuniadaCtrl.ComunidadScrollView.SetActive(false);
        ComuniadaCtrl.BusquedaButton.SetActive(false);
        OpenFavoritosScrollView(true);

      
    }

    /// <summary>
    /// 搜索点击
    /// </summary>
    /// <param name="data"></param>
    public void BusquedaButtonOnclicke(PointerEventData data)
    {
        CUIManager.Instance.OpenForm(UIFormName.Busqueda);
        //埋点*点击搜索
        GamePointManager.Instance.BuriedPoint(EventEnum.UgcSearch);
    }

    /// <summary>
    /// 生成综合性热门书籍
    /// </summary>
    public void SetHistoriasBookShelfData()
    {

        SpwanHistoriasItem();
       
    }

    private void SpwanHistoriasItem()
    {
        if (ComuniadaCtrl.BookPrefb == null) return;

        if (UserDataManager.Instance.WriterIndexInfo!=null)
        {
            if (UserDataManager.Instance.WriterIndexInfo.data.hotList.Count>5)
            {
                //使用复用列表
                Debug.Log("需要生成热门书籍："+ UserDataManager.Instance.WriterIndexInfo.data.hotList.Count);
                for (int i = 0; i < 5; i++)
                {
                    GameObject go = GameObject.Instantiate(ComuniadaCtrl.BookPrefb);
                    go.SetActive(true);
                    go.transform.SetParent(ComuniadaCtrl.HistoriasBookSekfScrollRect.content.transform);
                    go.transform.localScale = Vector3.one;
                    go.transform.localPosition = Vector3.zero;

                    ComuniadaBookSelf ComuniadaBookSelf = go.GetComponent<ComuniadaBookSelf>();
                    ComuniadaBookSelf.bookTypeName = "Historias";
                    ComuniadaBookSelfList.Add(ComuniadaBookSelf);

                }
                ComuniadaCtrl.HistoriasinfinityGridLayoutGroup.SetAmount(UserDataManager.Instance.WriterIndexInfo.data.hotList.Count);
            }
            else
            {
                for (int i = 0; i < UserDataManager.Instance.WriterIndexInfo.data.hotList.Count; i++)
                {
                    GameObject go = GameObject.Instantiate(ComuniadaCtrl.BookPrefb);
                    go.SetActive(true);
                    go.transform.SetParent(ComuniadaCtrl.HistoriasBookSekfScrollRect.content.transform);
                    go.transform.localScale = Vector3.one;
                    go.transform.localPosition = Vector3.zero;

                    ComuniadaBookSelf ComuniadaBookSelf = go.GetComponent<ComuniadaBookSelf>();
                    ComuniadaBookSelf.bookTypeName = "Historias";
                    ComuniadaBookSelf.Init(UserDataManager.Instance.WriterIndexInfo.data.hotList[i]);
                    ComuniadaBookSelfList.Add(ComuniadaBookSelf);

                }
            }
        }

       
    }

    /// <summary>
    /// 生成综合性最新书籍
    /// </summary>
    public void SetNuevasBookShelfData()
    {

        SpwanNuevasItem();
       
    }

    private void SpwanNuevasItem()
    {
        if (ComuniadaCtrl.BookPrefb == null) return;

        if (UserDataManager.Instance.WriterIndexInfo != null)
        {
            if (UserDataManager.Instance.WriterIndexInfo.data.newList.Count>5)
            {
                Debug.Log("需要生成最新书籍：" + UserDataManager.Instance.WriterIndexInfo.data.newList.Count);
                //使用复用列表
                for (int i=0;i<5;i++)
                {
                    GameObject go = GameObject.Instantiate(ComuniadaCtrl.BookPrefb);
                    go.SetActive(true);
                    go.transform.SetParent(ComuniadaCtrl.NuevasBookSekfScrollRect.content.transform);
                    go.transform.localScale = Vector3.one;
                    go.transform.localPosition = Vector3.zero;

                    ComuniadaBookSelf ComuniadaBookSelf = go.GetComponent<ComuniadaBookSelf>();
                    ComuniadaBookSelf.bookTypeName = "Nuevas";
                    ComuniadaBookSelfList.Add(ComuniadaBookSelf);
                }

                ComuniadaCtrl.NuevasinfinityGridLayoutGroup.SetAmount(UserDataManager.Instance.WriterIndexInfo.data.newList.Count);
            }
            else
            {
                for (int i = 0; i < UserDataManager.Instance.WriterIndexInfo.data.newList.Count; i++)
                {
                    GameObject go = GameObject.Instantiate(ComuniadaCtrl.BookPrefb);
                    go.SetActive(true);
                    go.transform.SetParent(ComuniadaCtrl.NuevasBookSekfScrollRect.content.transform);
                    go.transform.localScale = Vector3.one;
                    go.transform.localPosition = Vector3.zero;

                    ComuniadaBookSelf ComuniadaBookSelf = go.GetComponent<ComuniadaBookSelf>();
                    ComuniadaBookSelf.bookTypeName = "Nuevas";
                    ComuniadaBookSelf.Init(UserDataManager.Instance.WriterIndexInfo.data.newList[i]);
                    ComuniadaBookSelfList.Add(ComuniadaBookSelf);
                }
            }
        }
        
       
    }

   
    /// <summary>
    /// 综合性热门书本滑动时候，复用列表数据更新
    /// </summary>
    /// <param name="index"></param>
    /// <param name="trans"></param>

    public  void HistoriasHistoriasUpdateChildrenCallback(int index, Transform trans)
    {
       
        //Debug.Log("index:"+ index+"--name:"+trans.name);
        if (trans != null)
        {         
            trans.GetComponent<ComuniadaBookSelf>().Init(UserDataManager.Instance.WriterIndexInfo.data.hotList[index]);
        }
            
    }

    /// <summary>
    /// 综合性最新书本滑动时候，复用列表数据更新
    /// </summary>
    /// <param name="index"></param>
    /// <param name="trans"></param>
    public void NuevasHistoriasUpdateChildrenCallback(int index, Transform trans)
    {
        //Debug.Log("index:" + index + "--name:" + trans.name);
        if (trans != null)
        {         
            trans.GetComponent<ComuniadaBookSelf>().Init(UserDataManager.Instance.WriterIndexInfo.data.newList[index]);
        }
            
    }

    public void HistoriasMasButtonOnclicke(PointerEventData data)
    {
        UserDataManager.Instance.MasOpenType = 1;
        CUIManager.Instance.OpenForm(UIFormName.ComuniadaMas);
        //埋点*打开最受欢迎more
        GamePointManager.Instance.BuriedPoint(EventEnum.UgcMoreHotBook);
    }

    public void NuevasMasButtonOnclicke(PointerEventData data)
    {
        UserDataManager.Instance.MasOpenType = 2;
        CUIManager.Instance.OpenForm(UIFormName.ComuniadaMas);
        //埋点*打开最新书本more
        GamePointManager.Instance.BuriedPoint(EventEnum.UgcMoreNewBook);
    }


   

}

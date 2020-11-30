using System;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.UI;

public class ComuniadaCtrl :BaseUIForm
{
    [System.NonSerialized]
    public ComuniadaLogic ComuniadaLogic;
    [System.NonSerialized]
    public ComuniadaModle ComuniadaModle;

    [System.NonSerialized]
    public GameObject ComunidadButton, FavoritosButton, BusquedaButton;
    [System.NonSerialized]
    public GameObject ComunidadButtonOFF, ComunidadButtonON, FavoritosButtonOFF, FavoritosButtonON;
    [System.NonSerialized]
    public GameObject BookPrefb, HistoriasMasButton, NuevasMasButton;
    [System.NonSerialized]
    public InfinityGridLayoutGroup HistoriasinfinityGridLayoutGroup,NuevasinfinityGridLayoutGroup;
    [System.NonSerialized]
    public ScrollRect HistoriasBookSekfScrollRect, NuevasBookSekfScrollRect, SeguirBookSekfScrollRect;
    [System.NonSerialized]
    public GameObject ComunidadScrollView, FavoritosScrollView;
    private RectTransform BgRect;

    public override void OnOpen()
    {
        base.OnOpen();

        FindGameObject();
       
        ComuniadaLogic = new ComuniadaLogic(this);
        ComuniadaModle = new ComuniadaModle(this);

        EventLisentBingding();

        ComuniadaLogic.Init();

        //int offerH = 0;
        //if (GameUtility.IsIphoneXDevice())
        //{
        //    offerH = GameUtility.IphoneXTopH;
        //    BgRect.offsetMax = new Vector2(0, -(offerH));

        //}
    }

    public override void OnClose()
    {
        base.OnClose();

        EvenLisentMove();

        ComuniadaLogic.Close();
    }


    private void FindGameObject()
    {
        BgRect = transform.Find("BG").GetComponent<RectTransform>();

        ComunidadButton = transform.Find("BG/Top/ComunidadButton").gameObject;
        FavoritosButton = transform.Find("BG/Top/FavoritosButton").gameObject;
        BusquedaButton = transform.Find("BG/Top/BusquedaButton").gameObject;

        ComunidadButtonOFF = transform.Find("BG/Top/ComunidadButton/off").gameObject;
      
        ComunidadButtonON = transform.Find("BG/Top/ComunidadButton/on").gameObject;
     
        FavoritosButtonOFF = transform.Find("BG/Top/FavoritosButton/off").gameObject;
       
        FavoritosButtonON = transform.Find("BG/Top/FavoritosButton/on").gameObject;

        HistoriasinfinityGridLayoutGroup = transform.Find("BG/ComunidadScrollView/Viewport/Content/Historias/ScrollView/Viewport/Content").GetComponent<InfinityGridLayoutGroup>();
        BookPrefb = transform.Find("BG/Pref/BookItem").gameObject;
        HistoriasBookSekfScrollRect = transform.Find("BG/ComunidadScrollView/Viewport/Content/Historias/ScrollView").GetComponent<ScrollRect>();

        NuevasinfinityGridLayoutGroup = transform.Find("BG/ComunidadScrollView/Viewport/Content/Nuevas/ScrollView/Viewport/Content").GetComponent<InfinityGridLayoutGroup>();
        NuevasBookSekfScrollRect= transform.Find("BG/ComunidadScrollView/Viewport/Content/Nuevas/ScrollView").GetComponent<ScrollRect>();

        HistoriasMasButton = transform.Find("BG/ComunidadScrollView/Viewport/Content/Historias/MasButton").gameObject;
        NuevasMasButton = transform.Find("BG/ComunidadScrollView/Viewport/Content/Nuevas/MasButton").gameObject;


        ComunidadScrollView = transform.Find("BG/ComunidadScrollView").gameObject;
        FavoritosScrollView = transform.Find("BG/FavoritosScrollView").gameObject;

        ComunidadScrollView.SetActive(false);
        FavoritosScrollView.SetActive(false);
    }

   

    private void EventLisentBingding()
    {
        EventDispatcher.AddMessageListener(EventEnum.SwitchComuniada, OnSwitchTab);
        UIEventListener.AddOnClickListener(ComunidadButton, ComuniadaLogic.ComunidadButtonOnClicke);
        UIEventListener.AddOnClickListener(FavoritosButton, ComuniadaLogic.FavoritosButtonOnClicke);
        UIEventListener.AddOnClickListener(BusquedaButton,ComuniadaLogic.BusquedaButtonOnclicke);

        HistoriasinfinityGridLayoutGroup.updateChildrenCallback =ComuniadaLogic.HistoriasHistoriasUpdateChildrenCallback;
        NuevasinfinityGridLayoutGroup.updateChildrenCallback = ComuniadaLogic.NuevasHistoriasUpdateChildrenCallback;


        UIEventListener.AddOnClickListener(HistoriasMasButton,ComuniadaLogic.HistoriasMasButtonOnclicke);
        UIEventListener.AddOnClickListener(NuevasMasButton,ComuniadaLogic.NuevasMasButtonOnclicke);
    }

    private void OnSwitchTab(Notification notify)
    {
        var idx = Convert.ToInt32(notify.Data);
        if(idx == 0)
        {
            ComuniadaLogic.ComunidadButtonOnClicke(null);
        }
        else
        {
            ComuniadaLogic.FavoritosButtonOnClicke(null);
        }

        //埋点*点击快捷创作
        GamePointManager.Instance.BuriedPoint(EventEnum.UgcWriteBook);
    }

    private void EvenLisentMove()
    {
        UIEventListener.RemoveOnClickListener(ComunidadButton, ComuniadaLogic.ComunidadButtonOnClicke);
        UIEventListener.RemoveOnClickListener(FavoritosButton, ComuniadaLogic.FavoritosButtonOnClicke);
        UIEventListener.RemoveOnClickListener(BusquedaButton, ComuniadaLogic.BusquedaButtonOnclicke);
        EventDispatcher.RemoveMessageListener(EventEnum.SwitchComuniada, OnSwitchTab);

        HistoriasinfinityGridLayoutGroup.updateChildrenCallback =null;
        NuevasinfinityGridLayoutGroup.updateChildrenCallback = null;

        UIEventListener.RemoveOnClickListener(HistoriasMasButton, ComuniadaLogic.HistoriasMasButtonOnclicke);
        UIEventListener.RemoveOnClickListener(NuevasMasButton, ComuniadaLogic.NuevasMasButtonOnclicke);
    }
}

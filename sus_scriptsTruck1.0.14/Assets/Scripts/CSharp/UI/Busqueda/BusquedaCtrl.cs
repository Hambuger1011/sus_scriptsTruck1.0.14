using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BusquedaCtrl : BaseUIForm
{
    [System.NonSerialized]
    public GameObject CloseButton,BusqueBooke, GenresPrefb, GenresText;

    [System.NonSerialized]
    public BusquedaLogic BusquedaLogic;
    [System.NonSerialized]
    public BusquedaModle BusquedaModle;
    [System.NonSerialized]
    public BusquedaView BusquedaView;
    [System.NonSerialized]
    public ScrollRect ScrollView;

    public override void OnOpen()
    {
        base.OnOpen();

        FindGameObject();
        BusquedaLogic = new BusquedaLogic(this);
        BusquedaModle = new BusquedaModle(this);
        BusquedaView = new BusquedaView(this);

        AddListen();

        BusquedaLogic.Init();
    }

    public override void OnClose()
    {
        base.OnClose();

        BusquedaLogic.DestryGameObject();

        RemoveListen();

        BusquedaView.Close();
    }


    private void FindGameObject()
    {
        CloseButton = transform.Find("Bg/Close").gameObject;          
        GenresPrefb = transform.Find("Bg/Prefb/GeneroPrefb").gameObject;
        GenresText = transform.Find("Bg/BGScrollView/Viewport/Content/TileBg/GenresText").gameObject;
    }

    private void AddListen()
    {
       

        UIEventListener.AddOnClickListener(CloseButton, BusquedaLogic.CloseOnButton);
    }

    private void RemoveListen()
    {
        UIEventListener.RemoveOnClickListener(CloseButton, BusquedaLogic.CloseOnButton);


    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OfertesCtrl : BaseUIForm
{
    [System.NonSerialized]
    public GameObject Button, GifBg;

    [System.NonSerialized]
    public Text GifDiamanText, GifKeyText, DiscountPrice,price;

    [System.NonSerialized]
    public  OfertesLogic OfertesLogic;
    [System.NonSerialized]
    public OfertesView OfertesView;
    [System.NonSerialized]
    public OfertesDB OfertesDB;
    private RectTransform BgRect;
    public override void OnOpen()
    {
        base.OnOpen();
        OfertesDB = new OfertesDB();
        OfertesLogic = new OfertesLogic(this);
        OfertesView = new OfertesView(this);
       
        FindGameObject();
        AddListen();

        //int offerH = 0;
        //if (GameUtility.IsIphoneXDevice())
        //{
        //    offerH = GameUtility.IphoneXTopH;
        //    BgRect.offsetMax = new Vector2(0, -(offerH));

        //}
    }

    public override void OnAppear()
    {
        base.OnAppear();
        this.OfertesLogic.GetADSInfo();
    }


    private void FindGameObject()
    {
        BgRect = transform.Find("BG").GetComponent<RectTransform>();

        Button = transform.Find("BG/ScrollView/Viewport/Content/BG/GifBg/Button").gameObject;
        GifDiamanText = transform.Find("BG/ScrollView/Viewport/Content/BG/GifBg/DiamantImage/Text").GetComponent<Text>();
        GifKeyText = transform.Find("BG/ScrollView/Viewport/Content/BG/GifBg/KeyImage/Text").GetComponent<Text>();
        DiscountPrice = transform.Find("BG/ScrollView/Viewport/Content/BG/GifBg/DiscountPrice").GetComponent<Text>();
        price = transform.Find("BG/ScrollView/Viewport/Content/BG/GifBg/Button/Text").GetComponent<Text>();
        GifBg = transform.Find("BG/ScrollView/Viewport/Content/BG/GifBg").gameObject;
    }

    private void AddListen()
    {
        UIEventListener.AddOnClickListener(Button,OfertesLogic.ButtonOnclick);
    }


    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(Button, OfertesLogic.ButtonOnclick);

        OfertesView.Close();
    }
}

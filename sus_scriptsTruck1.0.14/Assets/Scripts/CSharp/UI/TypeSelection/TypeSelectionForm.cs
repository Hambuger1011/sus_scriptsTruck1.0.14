using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
public enum TypeSlectNumber
{
    Nuevo,
    Popular,
    Otros,

    Button1,
    Button2,
    Button3,
    Button4,
    Button5,
    Button6,
    Button7,
    Button8,
    Button9,
    Button10,
    Button11,
    Button12,

    end
}

public class TypeSelectionForm : BaseUIForm
{
    [NonSerialized]
    public string[] GeneroButtonName;

    [NonSerialized]
    public TypeSelectionFormLogic TypeSelectionFormLogic;

    [NonSerialized]
    public TypeSelectionFormView TypeSelectionFormView;

    [NonSerialized]
    public TypeSelectionDB TypeSelectionDB;

    [NonSerialized]
    public GameObject GeneroButton, GenresList;

    [NonSerialized]
    public List<GameObject> On, Off;

    [NonSerialized]
    public GameObject NuevoOff, NuevoOn, PopularOff, PopularOn, OtrosOff, OtrosOn;
    

    private RectTransform BgRect;
    public override void OnOpen()
    {
        base.OnOpen();

        if (On == null)
            On = new List<GameObject>();

        if (Off == null)
            Off = new List<GameObject>();

        GeneroButtonName = new string[(int)TypeSlectNumber.end-3];
        FindGameObject();

        TypeSelectionFormLogic = new TypeSelectionFormLogic(this);
        TypeSelectionFormView = new TypeSelectionFormView(this);
        TypeSelectionDB = new TypeSelectionDB();
        

        if (ResolutionAdapter.HasUnSafeArea)
        {
            var scale = this.myForm.GetScale();
            var safeArea = ResolutionAdapter.GetSafeArea();
            var offset = this.myForm.Pixel2View(safeArea.position);
            var offerH = offset.y;
            BgRect.offsetMax = new Vector2(0, -(offerH));

        }
    }

    public override void OnClose()
    {
        base.OnClose();

        TypeSelectionFormLogic.Close();
        TypeSelectionFormView.Close();
    }

    private void FindGameObject()
    {
        GeneroButton = transform.Find("BG/VerticalScroll/Viewport/VLayout/TypeGroup/Prefb/GeneroButton").gameObject;
        GenresList = transform.Find("BG/VerticalScroll/Viewport/VLayout/TypeGroup/GenresList").gameObject;

        NuevoOn = transform.Find("BG/VerticalScroll/Viewport/VLayout/TypeGroup/Ordenar/Nuevo/ON").gameObject;
        On.Add(NuevoOn);
        PopularOn = transform.Find("BG/VerticalScroll/Viewport/VLayout/TypeGroup/Ordenar/Popular/ON").gameObject;
        On.Add(PopularOn);
        OtrosOn = transform.Find("BG/VerticalScroll/Viewport/VLayout/TypeGroup/Ordenar/Otros/ON").gameObject;
        On.Add(OtrosOn);

        NuevoOff = transform.Find("BG/VerticalScroll/Viewport/VLayout/TypeGroup/Ordenar/Nuevo/Off").gameObject;
        Off.Add(NuevoOff);
        PopularOff = transform.Find("BG/VerticalScroll/Viewport/VLayout/TypeGroup/Ordenar/Popular/Off").gameObject;
        Off.Add(PopularOff);
        OtrosOff = transform.Find("BG/VerticalScroll/Viewport/VLayout/TypeGroup/Ordenar/Otros/Off").gameObject;
        Off.Add(OtrosOff);

        BgRect = transform.Find("BG").GetComponent<RectTransform>();

    }
}

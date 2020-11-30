using System;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CatSetForm : CatBaseForm {

    private Text ContentText;

    private GameObject BackButton, Setbutton;
    private Action<string> SetButtonCall;  
    private void Awake()
    {
        BackButton = transform.Find("UIMask").gameObject;      
        ContentText = transform.Find("Bg/ContentBg/ContentText").GetComponent<Text>();
      
      
        Setbutton = transform.Find("Bg/Setbutton").gameObject;
    }
    public override void OnOpen()
    {
        base.OnOpen();
        UiFormIndex = (int)CatFormEnum.CAT_SET;

        UIEventListener.AddOnClickListener(BackButton, CloseUi);    
        UIEventListener.AddOnClickListener(Setbutton, SetButtonOnclicke);
    }


    public override void OnClose()
    {
        base.OnClose();

        UIEventListener.RemoveOnClickListener(BackButton, CloseUi);  
        UIEventListener.RemoveOnClickListener(Setbutton, SetButtonOnclicke);
    }


    private void CloseUi(PointerEventData data)
    {
        //派发事件，关闭这个界面
        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), UiFormIndex);
    }

    public void Inite(string content,Action<string> SetButtonCall)
    {      
        ContentText.text = content.ToString();   
        this.SetButtonCall = SetButtonCall;
    }

   
    private void SetButtonOnclicke(PointerEventData data)
    {
        CloseUi(null);
        SetButtonCall(null);
    }
}

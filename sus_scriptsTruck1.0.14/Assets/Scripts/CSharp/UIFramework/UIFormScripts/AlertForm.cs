using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UGUI;
using DG.Tweening;
using System;
using XLua;

[LuaCallCSharp]
public enum AlertType
{
    Sure=0,      //确认(一个按钮)
    SureOrCancel=1,    //确认或取消(两个按钮)
}

/// <summary>
/// 弹窗确认框
/// </summary>

[LuaCallCSharp]
[ReflectionUse]
public class AlertForm : BaseUIForm
{
    public RectTransform FrameTrans;
    public Button BtnOK;
    public Button BtnOK2;
    public Button BtnCancel;

    public Text BtnOk1Text;
    public Text BtnOk2Text;
    public Text CancelText;
    public Text TxtTitle;
    public Text TxtContent;

    private Action<bool> mCallBack;
    public override void OnOpen()
    {
        base.OnOpen();
        this.BtnOK.onClick.AddListener(OnOkClick);
        this.BtnOK2.onClick.AddListener(OnOkClick);
        this.BtnCancel.onClick.AddListener(OnCancelClick);
        this.FrameTrans.localScale = new Vector3(1, 0, 1);
        this.FrameTrans.DOScaleY(1, 0.25f).SetEase(Ease.OutBack).Play();
    }

    public override void OnClose()
    {
        base.OnClose();
        this.BtnOK.onClick.RemoveListener(OnOkClick);
        this.BtnOK2.onClick.RemoveListener(OnOkClick);
        this.BtnCancel.onClick.RemoveListener(OnCancelClick);
    }

    private void OnOkClick()
    {
        DoResult(true);
    }

    private void OnCancelClick()
    {
        DoResult(false);
    }

    private void DoResult(bool value)
    {
        if (mCallBack != null)
            mCallBack(value);
        CUIManager.Instance.CloseForm(UIFormName.AlertUiForm);
    }

    public void SetMessage(string strTitle, string strContent, AlertType vType = AlertType.Sure, Action<bool> callBack = null, string _BtnOk1Text = "OK", string _BtnOk2Text = "OK", string CancelText = "CANCEL")
    {
        strContent = strContent.Replace("\\n", "\n");
        this.TxtTitle.text = strTitle;
        this.TxtContent.text = strContent;

        this.BtnOk1Text.text = _BtnOk1Text;
        this.BtnOk2Text.text = _BtnOk2Text;
        this.CancelText.text = CancelText;

        mCallBack = callBack;

        BtnCancel.gameObject.SetActive(vType == AlertType.SureOrCancel);

        if (vType == AlertType.SureOrCancel)
        {
            BtnOK.gameObject.SetActiveEx(true);
            BtnCancel.gameObject.SetActiveEx(true);
            BtnOK2.gameObject.SetActiveEx(false);
        }
        else
        {
            BtnOK.gameObject.SetActiveEx(false);
            BtnCancel.gameObject.SetActiveEx(false);
            BtnOK2.gameObject.SetActiveEx(true);
        }
    }
}

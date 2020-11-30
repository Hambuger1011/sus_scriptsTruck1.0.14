using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using UGUI;

public class ComingSoonDialogForm : BaseUIForm
{
    public RectTransform frameTrans;
    public Button btnOK;
    public Text txtTitle;
    public Text txtContent;

    public override void OnOpen()
    {
        base.OnOpen();
        this.btnOK.onClick.AddListener(OnOkClick);
        this.frameTrans.localScale = new Vector3(1,0,1);
        this.frameTrans.DOScaleY(1, 0.25f).SetEase(Ease.OutBack).Play();
    }

    public override void OnClose()
    {
        base.OnClose();

        this.btnOK.onClick.RemoveListener(OnOkClick);
    }

    private void OnOkClick()
    {
        CUIManager.Instance.CloseForm(UIFormName.ComingSoonDialogForm);
    }

    public void SetMessage(string strTitle, string strContent)
    {
        this.txtTitle.text = strTitle;
        this.txtContent.text = strContent;
    }
}

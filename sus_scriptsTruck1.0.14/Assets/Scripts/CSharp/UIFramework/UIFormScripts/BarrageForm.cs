using pb;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BarrageForm : BaseUIForm {

    public GameObject FirstBg, SureUI,Mask;
    public GameObject AddButtonImage, DislikeButtonImage;
    public GameObject SureButton, CancelButton;
    public Text Contern;

    private commentlist commentlist;
    public override void OnOpen()
    {
        base.OnOpen();

        UIEventListener.AddOnClickListener(AddButtonImage, AddButtonOncklicke);
        UIEventListener.AddOnClickListener(DislikeButtonImage,DislikeOnclicke);
        UIEventListener.AddOnClickListener(SureButton, SureButtonOnclicke);
        UIEventListener.AddOnClickListener(CancelButton,CancelButtonOnclicke);
        UIEventListener.AddOnClickListener(Mask, MaskClose);

        FirstBg.SetActive(true);
        SureUI.SetActive(false);
    }
    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(AddButtonImage, AddButtonOncklicke);
        UIEventListener.RemoveOnClickListener(DislikeButtonImage, DislikeOnclicke);
        UIEventListener.RemoveOnClickListener(SureButton, SureButtonOnclicke);
        UIEventListener.RemoveOnClickListener(CancelButton, CancelButtonOnclicke);
        UIEventListener.RemoveOnClickListener(Mask, MaskClose);
    }
    private void MaskClose(PointerEventData data)
    {
        CloseUi();
    }
    private void CloseUi()
    {
        CUIManager.Instance.CloseForm(UIFormName.BarrageForm);//
    }

    public void Inite(commentlist commentlist)
    {
        this.commentlist = commentlist;
        Contern.text = commentlist.content.ToString();
    }
    
    private void AddButtonOncklicke(PointerEventData data)
    {
        EventDispatcher.Dispatch(EventEnum.AddBarrageOnclike, commentlist.content.ToString());
        CloseUi();
    }

    private void DislikeOnclicke(PointerEventData data)
    {
        FirstBg.SetActive(false);
        SureUI.SetActive(true);
    }

    private void SureButtonOnclicke(PointerEventData data)
    {
        //弹幕页举报
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.Setcommentreport(2, 1, commentlist.commentid, SetcommentreportCallback);
    }
    private void CancelButtonOnclicke(PointerEventData data)
    {
        FirstBg.SetActive(true);
        SureUI.SetActive(false);
    }


    private void SetcommentreportCallback(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----SetcommentreportCallback---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(136);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Report success!", false);
                    CloseUi();
                }

            }, null);
        }
    }
}

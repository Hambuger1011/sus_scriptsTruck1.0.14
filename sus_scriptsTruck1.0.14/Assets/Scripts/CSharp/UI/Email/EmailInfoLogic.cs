using pb;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class EmailInfoLogic
{
    private GameObject CloseButton;
    private EmailInfoForm EmailInfoForm;
    private Transform transform;
    private GameObject GitButton;
    private EmailItemInfo EmailItemInfo;
    public EmailInfoLogic(EmailInfoForm EmailInfoForm)
    {
        this.EmailInfoForm = EmailInfoForm;
        this.EmailItemInfo = EmailInfoForm.EmailItemInfo;
        transform = EmailInfoForm.transform;

        FindGameObject();
        AddListenButton();
    }

    private void FindGameObject()
    {
        CloseButton = transform.Find("Bg/TopBg/Close").gameObject;
        GitButton = transform.Find("Bg/GitBg/Button").gameObject;

    }

    private void AddListenButton()
    {
        UIEventListener.AddOnClickListener(CloseButton, CloseButtonOn);
        UIEventListener.AddOnClickListener(GitButton, GitButtonOnclicke);
    }

    public void Close()
    {
        UIEventListener.RemoveOnClickListener(CloseButton, CloseButtonOn);
        UIEventListener.RemoveOnClickListener(GitButton, GitButtonOnclicke);
    }

    private void CloseButtonOn(PointerEventData data)
    {
        CUIManager.Instance.CloseForm(UIFormName.EmailInfo);
    }

    private void GitButtonOnclicke(PointerEventData data)
    {
        LOG.Info("Collect点击");
        if (EmailItemInfo.sign_url != null && !string.IsNullOrEmpty(EmailItemInfo.sign_url.ToString()))
        {
            //网页跳转
            Application.OpenURL(EmailItemInfo.sign_url);
        }

        if (EmailItemInfo != null && EmailItemInfo.msg_type == 4)
        {
            LOG.Info("--------emailItemInfo.msg_type-------->" + EmailItemInfo.msg_type);
            SdkMgr.Instance.OpenWebView(EmailItemInfo.email_url, WebViewCallBack);
        }
        else
        {
            if (EmailItemInfo.price_status == 0)
            {
                //奖励没有领取的时候点击按钮领取
                //UINetLoadingMgr.Instance.Show();
                GameHttpNet.Instance.GetEmailAward(EmailItemInfo.msgid, GetEmailAwardCallBack);
            }
            else
            {
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(169);
                UITipsMgr.Instance.PopupTips(Localization, false);
            }
        }
    }

    private void WebViewCallBack(string vMsg)
    {
        if (!string.IsNullOrEmpty(vMsg) && vMsg.Equals("2a338a7aa119c2d5433e9f738086560612"))
        {
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.GetEmailAward(EmailItemInfo.msgid, GetEmailAwardCallBack);

            SdkMgr.Instance.CloseWebView(WebViewCallBack);
        }
    }

    private void GetEmailAwardCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetEmailAwardCallBack---->" + result);
        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();
            JsonObject jo = JsonHelper.JsonToJObject(result);

            if (jo.code == 200)
            {
                UserDataManager.Instance.emailGetAwardInfo = JsonHelper.JsonToObject<HttpInfoReturn<EmailGetAwardInfo>>(result);

                            
                UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.emailGetAwardInfo.data.bkey);
                UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.emailGetAwardInfo.data.diamond);
                UserDataManager.Instance.ResetMoney(3, UserDataManager.Instance.emailGetAwardInfo.data.ticket);

                var Localization = GameDataMgr.Instance.table.GetLocalizationById(158);
                UITipsMgr.Instance.PopupTips(Localization, false);


                EmailItemInfo.price_status = 1;
                EmailInfoForm.EmailInfoView.ShowGift();

            }
            else
            {
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(169);
                UITipsMgr.Instance.PopupTips(Localization, false);
                //UITipsMgr.Instance.PopupTips("Already received!", false);
            }

        }, null);
    }
  
}

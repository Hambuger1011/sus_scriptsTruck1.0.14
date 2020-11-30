using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using System;
using UGUI;

/// <summary>
/// 确认框，管理类
/// </summary>
public class UIAlertMgr : CSingleton<UIAlertMgr>
{
    public bool IsLock = false;
    protected override void Init()
    {
        base.Init();
    }

    /// <summary>
    /// 弹窗提示
    /// </summary>
    /// <param name="strTitle"></param>
    /// <param name="strContent"></param>
    /// <param name="vType"></param>
    /// <param name="callBack"></param>
    public void Show(string strTitle, string strContent, AlertType vType = AlertType.Sure, Action<bool> callBack = null , string _BtnOk1Text = "OK", string _BtnOk2Text = "OK", string CancelText = "CANCEL")
    {
        CUIManager.Instance.OpenForm(UIFormName.AlertUiForm, true);
        var dialog = CUIManager.Instance.GetForm<AlertForm>(UIFormName.AlertUiForm);
        if (dialog != null)
        {
            dialog.SetMessage(strTitle, strContent, vType, callBack, _BtnOk1Text, _BtnOk2Text, CancelText);
        }
    }

    /// <summary>
    /// 广播提示
    /// </summary>
    public void BroadcastShow()
    {
        if (GameDataMgr.HasInstance() && GameDataMgr.Instance.BaseResLoadFinish)
        {
            CUIManager.Instance.OpenForm(UIFormName.BroadcastTipForm, true);
            var broadcastTip = CUIManager.Instance.GetForm<BroadcastTipForm>(UIFormName.BroadcastTipForm);
            if (broadcastTip != null)
                broadcastTip.ShowUI();
        }
    }
    public void ShowNetworkAlert(long responseCode, Action callback)
    {
        CUIManager.Instance.OpenForm(UIFormName.NetworkAlert);
        var networkForm = CUIManager.Instance.GetForm<NetworkAlertForm>(UIFormName.NetworkAlert);
        networkForm.ShowNetworkAlert(responseCode, callback);
    }

}

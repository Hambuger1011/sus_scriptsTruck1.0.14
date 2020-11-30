using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UGUI;
using DG.Tweening;
using System;
using GameCore.UGUI;

public class NetworkAlertForm : BaseUIForm
{
    public RectTransform networkAlertTF;
    public UITweenButton btnConnectNetwork;
    public UITweenButton btnFeedback;
    public Text txtTips;

    Action mConnectNetwork;

    protected override void Awake()
    {
        base.Awake();
        networkAlertTF.gameObject.SetActiveEx(false);

        btnConnectNetwork.onClick.AddListener(OnConnectNetworkClick);
        btnFeedback.onClick.AddListener(OnFeedbackClick);
    }

    private void OnFeedbackClick()
    {
        CUIManager.Instance.OpenForm(UIFormName.LoadingFeedBack);
    }

    void OnConnectNetworkClick()
    {
        networkAlertTF.gameObject.SetActiveEx(false);
        this.myForm.Close();
        if (mConnectNetwork != null)
        {
            var tmp = mConnectNetwork;
            mConnectNetwork = null;
            tmp();
        }
    }

    public void ShowNetworkAlert(long responseCode, Action callback)
    {
        mConnectNetwork += callback;
        networkAlertTF.gameObject.SetActiveEx(true);
        DOTween.Kill(networkAlertTF);
        networkAlertTF.localScale = new Vector3(1, 0, 1);
        networkAlertTF.DOScaleY(1, 0.25f).SetEase(Ease.OutBack).SetId(networkAlertTF).Play();
        switch (responseCode)
        {
            case 0:
                this.txtTips.text = "Network link failed";
                break;
            default:
                this.txtTips.text = "protocol error:" + responseCode;
                break;
        }
    }
}

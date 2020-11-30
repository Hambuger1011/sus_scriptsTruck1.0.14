using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using UGUI;
using UnityEngine;
using UnityEngine.UI;
public class SystemNoticePanel : MonoBehaviour
{
    public GameObject BgObj;
    private Text Title;
    private Text ContentText;
    private Button BtnOk;

    private void Awake()
    {
        this.BgObj = DisplayUtil.GetChild(this.gameObject, "BgObj");
        this.Title = DisplayUtil.GetChildComponent<Text>(this.gameObject,"Title");
        this.ContentText = DisplayUtil.GetChildComponent<Text>(this.gameObject, "ContentText");
        this.BtnOk = DisplayUtil.GetChildComponent<Button>(this.gameObject, "BtnOk");


        this.BtnOk.onClick.AddListener(OnBtnOKClick);
    }

    public void SetData(string content)
    {
        this.ContentText.text = content;
    }


    private void OnBtnOKClick()
    {
        //再次获取Appconfig   重新开始走流程
        Dispatcher.dispatchEvent(EventEnum.AgainGetAppConfIsMaintin);
    }

    public void onClose()
    {
        this.BgObj.SetActiveEx(false);
    }

}

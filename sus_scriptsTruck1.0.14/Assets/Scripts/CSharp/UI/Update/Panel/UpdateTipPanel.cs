using System;
using System.Collections;
using System.Collections.Generic;
using AB;
using Framework;
using UGUI;
using UnityEngine;
using UnityEngine.UI;

public class UpdateTipPanel : MonoBehaviour
{
    public GameObject BgObj;
    private Text Title;
    private Text ContentText;
    private Button BtnCancel;
    private Button BtnUpdate;

    private Text CancelText;


    private void Awake()
    {
        this.BgObj = DisplayUtil.GetChild(this.gameObject, "BgObj");
        this.Title = DisplayUtil.GetChildComponent<Text>(this.gameObject, "Title");
        this.ContentText = DisplayUtil.GetChildComponent<Text>(this.gameObject, "ContentText");
        this.BtnCancel = DisplayUtil.GetChildComponent<Button>(this.gameObject, "BtnCancel");
        this.BtnUpdate = DisplayUtil.GetChildComponent<Button>(this.gameObject, "BtnUpdate");

        this.CancelText = DisplayUtil.GetChildComponent<Text>(this.BtnCancel.gameObject, "Text");

        this.BtnCancel.onClick.AddListener(OnBtnCancelClick);
        this.BtnUpdate.onClick.AddListener(OnBtnUpdateClick);
    }

    private int isforceUpdate = -1;
    public void SetData(string content,int _isforceUpdate)
    {
        this.ContentText.text = content;
        isforceUpdate = _isforceUpdate;

        if (isforceUpdate == 1)
        {
            //属于强更   //跳转强更
            this.CancelText.text = "EXIT";
        }
    }


    private void OnBtnCancelClick()
    {
        if (isforceUpdate == 1)
        {
            //属于强更   //跳转强更
            Application.Quit();
        }
        else if (isforceUpdate == 0)
        {
            //开始热更新
            XLuaHelper.isHotUpdate = false;

            this.BgObj.SetActiveEx(false);


            //判断是否要进入热更新  强制提示面板
            if (GameFrameworkImpl.Instance.isHotfixRes() == true)
            {
                //打开系统更新界面
                CUIManager.Instance.OpenForm(UIFormName.UIUpdateModule);
                //打开 系统更新公告面板  //传入【游戏维护】公告信息
                CUIManager.Instance.GetForm<UIUpdateModule>(UIFormName.UIUpdateModule).SetPanel(EnumUpdate.UpdateHotfixTipPanel);
            }
            else
            {
                //【==========================进入资源更新加载场景==========================】
                CSingleton<CGameStateMgr>.GetInstance().GotoState<CLoadingState>();
                AndroidUtils.CloseSplash();
                CUIManager.Instance.CloseForm(UIFormName.UIUpdateModule);
            }
        }

      
    }

    private void OnBtnUpdateClick()
    {
        if (isforceUpdate == 1)
        {
            //属于强更   //跳转强更
            GameFrameworkImpl.Instance.GoToUpdate();
        }
        else if(isforceUpdate == 0)
        {
            // //开始热更新
            // XLuaHelper.isHotUpdate = true;
            //
            // // //跳转资源更新加载场景
            // CGameStateMgr.Instance.GotoState<CLoadingState>();
            // AndroidUtils.CloseSplash();


            //属于提更   //跳转强更  跳转平台下载更新
            GameFrameworkImpl.Instance.GoToUpdate();
        }
    }

    public void onClose()
    {
        this.BgObj.SetActiveEx(false);
    }

    
}

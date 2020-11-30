using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;
using UGUI;

/// <summary>
/// 控制网络的loading条
/// </summary>
[XLua.Hotfix, XLua.LuaCallCSharp]
public class UINetLoadingMgr : CMonoSingleton<UINetLoadingMgr>
{
    //延时打开
    private float mDelayShow = 0;
    //延时关闭
    private float mDelayClose = 0;
    //是否在显示
    private bool mIsShow;
    private void Update()
    {
        if (count > 0)
        {
            if (!mIsShow)
            {
                mDelayShow += Time.deltaTime;
                if (mDelayShow > 1f)
                {
                    mDelayShow = 0;

                    //真正打开
                    OpenShow();
                    mIsShow = true;
                }
            }
            else
            {
                mDelayClose += Time.deltaTime;

                if (mDelayClose > 5)
                {
                    count = 0;
                    mDelayClose = 0;
                    //真正隐藏
                    CloseShow();
                }
            }
        }
    }

    /// <summary>
    /// 延时两秒打开 loadUI界面 转圈
    /// </summary>
    int count = 0;
    public void Show()
    {
        count += 1;
        if (count == 1)
        {
            mIsShow = false;
            this.OpenShowMask();
        }
        // Debug.LogError("Show count: " + count);
    }

    public void Close()
    {
        count -= 1;
        if (count <= 0)
        {
            count = 0;
            if (!mLock)
            {
                CloseShow();
            }
        }
        // Debug.LogError("Close count: " + count);
    }

    private void OpenShowMask()
    {
        if (mLock) { return; }

        NetLoadingForm uiForm = CUIManager.Instance.GetForm<NetLoadingForm>(UIFormName.NetLoadingForm);
        if (uiForm != null)
        {
            if (uiForm.mask.activeSelf == false)
            {
                uiForm.showMask();
            }
        }
        else
        {
            CUIManager.Instance.OpenForm(UIFormName.NetLoadingForm);
            NetLoadingForm uiForm2 = CUIManager.Instance.GetForm<NetLoadingForm>(UIFormName.NetLoadingForm);
            if (uiForm2 != null)
            {
                uiForm2.showMask();
            }
        }
    }
    private void OpenShow()
    {
        if (mLock) { return; }

        NetLoadingForm uiForm = CUIManager.Instance.GetForm<NetLoadingForm>(UIFormName.NetLoadingForm);
        if (uiForm != null)
        {
            if (uiForm.anima.gameObject.activeSelf == false)
            {
                uiForm.show();
            }
        }
        else
        {
            CUIManager.Instance.OpenForm(UIFormName.NetLoadingForm);
            NetLoadingForm uiForm2 = CUIManager.Instance.GetForm<NetLoadingForm>(UIFormName.NetLoadingForm);
            if (uiForm2 != null)
            {
                uiForm.show();
            }
        }
    }

    private void CloseShow()
    {
        if (mLock) { return; }

        NetLoadingForm uiForm = CUIManager.Instance.GetForm<NetLoadingForm>(UIFormName.NetLoadingForm);
        if (uiForm != null)
        {
            uiForm.hide();
        }
        else
        {
            CUIManager.Instance.CloseForm(UIFormName.NetLoadingForm);
        }
        mIsShow = false;
        mDelayShow = 0;
        mDelayClose = 0;

        // Debug.LogError("CloseShow count: " + count);
    }


    private bool mLock;
    public void Lock()
    {
        mLock = true;
        //var uiform = CUIManager.Instance.OpenForm(UIFormName.NetLoadingForm);
    }

    public void UnLock()
    {
        mLock = false;
    }

    /// <summary>
    /// 直接打开 转圈界面
    /// </summary>
    int count2 = 0;
    public void Show2()
    {
        count2 += 1;
        if (count2 == 1)
        {
            this.OpenShowMask();
            //真正打开
            OpenShow();
        }
    }
    /// <summary>
    /// 直接关闭
    /// </summary>
    public void Close2()
    {
        count2 -= 1;
        if (count2 <= 0)
        {
            count2 = 0;

            //真正隐藏
            CloseShow();
        }
    }
}

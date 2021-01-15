using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UGUI;
using DG.Tweening;
using System.Text.RegularExpressions;

/// <summary>
/// 广播类组件
/// </summary>
public class BroadcastTipForm : BaseUIForm
{
    public RectTransform FrameTrans;
    public Text TxtContent;

    private int mEnterCount = 0;
    private bool mBroadcastIsRun = false;

    public override void OnOpen()
    {
        base.OnOpen();

        CheckPos();
    }

    private void CheckPos()
    {
        float offect = 0;
#if !UNITY_EDITOR && UNITY_ANDROID
        if (ResolutionAdapter.androidisSafeArea == true)
        {
            offect=60;
        }
#endif
#if UNITY_EDITOR || UNITY_IOS
        if (ResolutionAdapter.HasUnSafeArea)
        {
            var safeArea = ResolutionAdapter.GetSafeArea();
            var _offset = myForm.Pixel2View(safeArea.position);
            offect = _offset.y;
        }
#endif

        if (MyBooksDisINSTANCE.Instance.GetIsPlaying())
        {
            //在书本阅读中
            FrameTrans.anchoredPosition = new Vector2(0, 0 - offect);
        }
        else
        {
            //不在书本阅读中
            FrameTrans.anchoredPosition = new Vector2(0, -103 - offect);
        }  
    }

    /// <summary>
    /// 显示广播效果
    /// </summary>
    /// <param name="vIniformation">广播内容</param>
    public void ShowUI()
    {
        if (!mBroadcastIsRun)
        {
            mBroadcastIsRun = true;
            checkBroadcastLeft();
        }

    }
    //开始执行跑马灯效果
    private void doRun(string vIniformation)
    {
        CheckPos();
        TxtContent.text = vIniformation;
        Regex reg = new Regex(@"</?\w+[^>]*>");
        string resultStr = reg.Replace(vIniformation, "");
        int strLen = resultStr.Length;
        int oriLen = 50;
        float runTime = 7f + (((strLen - oriLen) > 0) ? (strLen - oriLen) * 0.13f : 0);
        float targetX = -(strLen * 15.7f) - (Screen.width / 2f);
        //LOG.Info("---TargetX----->" + targetX + "-----runTims----->" + runTime + "------len------->" + resultStr.Length + "----resultStr--->" + resultStr);
        TxtContent.rectTransform.anchoredPosition = new Vector2(400,0);
        TxtContent.DOKill();
        TxtContent.rectTransform.DOAnchorPosX(targetX, runTime).OnComplete(() => { checkBroadcastLeft(); }).SetEase(Ease.Linear).Play();
    }

    //检查是否还有广播需要播
    private void checkBroadcastLeft()
    {
        if ((UserDataManager.Instance.GMBroadcastQueue == null || UserDataManager.Instance.GMBroadcastQueue.Count <= 0) &&
            (UserDataManager.Instance.BroadcastQueue == null || UserDataManager.Instance.BroadcastQueue.Count <= 0))
        {
            BroadCastIsOver();
            mBroadcastIsRun = false;
            return;
        }
        
        string chatMsg = GetMsg();
        if (string.IsNullOrEmpty(chatMsg))
        {
            checkBroadcastLeft();
            return;
        }
        doRun(chatMsg);
    }

    private string GetMsg()
    {
        string chatMsg = String.Empty;
        if (UserDataManager.Instance.GMBroadcastQueue.Count > 0)
        {
            chatMsg = UserDataManager.Instance.GMBroadcastQueue.Peek();
            UserDataManager.Instance.GMBroadcastQueue.Dequeue();
        }
        else if (UserDataManager.Instance.BroadcastQueue.Count > 0)
        {
            chatMsg = UserDataManager.Instance.BroadcastQueue.Peek();
            UserDataManager.Instance.BroadcastQueue.Dequeue();
        }
        return chatMsg;
    }

    //所有广播，已经播放完
    private void BroadCastIsOver()
    {
        CUIManager.Instance.CloseForm(UIFormName.BroadcastTipForm);
    }

    void OnDisable()
    {
        mBroadcastIsRun = false;
    }

}

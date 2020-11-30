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

        if (MyBooksDisINSTANCE.Instance.GetIsPlaying())
        {
            //在游戏中
            FrameTrans.anchoredPosition = new Vector2(0, 0);
        }
        else
        {
            //不在游戏中
            FrameTrans.anchoredPosition = new Vector2(0, -103);
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
        if (UserDataManager.Instance.BroadcastQueue == null || UserDataManager.Instance.BroadcastQueue.Count <= 0)
        {
            BroadCastIsOver();
            mBroadcastIsRun = false;
            return;
        }
        string chatMsg = UserDataManager.Instance.BroadcastQueue.Peek();
        if (string.IsNullOrEmpty(chatMsg))
        {
            UserDataManager.Instance.BroadcastQueue.Dequeue();
            checkBroadcastLeft();
            return;
        }
        doRun(chatMsg);
        UserDataManager.Instance.BroadcastQueue.Dequeue();
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

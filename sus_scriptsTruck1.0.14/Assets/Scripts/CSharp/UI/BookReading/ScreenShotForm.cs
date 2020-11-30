using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using UGUI;

#if !NOT_USE_LUA
using DialogDisplaySystem = BookReadingWrapper;
#endif

/// <summary>
/// 屏幕截图
/// </summary>
public class ScreenShotForm : BaseUIForm
{
    public GameObject Mask;
    public CanvasGroup CanvasGr;
    public Image LogoIcon;
    public Text BookName;

    private bool mIsFinishScrShot;
    private bool mInOpen;
    public override void OnOpen()
    {
        base.OnOpen();

        Init();

        UIEventListener.AddOnClickListener(Mask, ToScrShotAndCloe);

    }

    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(Mask, ToScrShotAndCloe);
    }

    private  void Init()
    {
        mInOpen = true;
        mIsFinishScrShot = false;

        if (CanvasGr != null)
            CanvasGr.alpha = 0;

        if(CanvasGr != null)
        {
            CanvasGr.DOFade(1, 0.5f).SetDelay(0.2f).OnComplete(() => { DoScreenShot(); }).Play();
        }

        if(DialogDisplaySystem.Instance.CurrentBookData != null)
        {
            BookName.text = "<color='#278ecf'><size='36'>" + DialogDisplaySystem.Instance.CurrentBookData.BookName + "</size></color>" + "<color='#434343'><size='23'>  Chapter " + DialogDisplaySystem.Instance.CurrentBookData.ChapterID + "</size></color>";
            //TalkingDataManager.Instance.ScreenShot("ScreenShot_BookId " + DialogDisplaySystem.Instance.CurrentBookData.BookID + "_Chapter" + DialogDisplaySystem.Instance.CurrentBookData.ChapterID + "_DialogId" + DialogDisplaySystem.Instance.CurrentBookData.DialogueID);
        }

    }

    private void DoScreenShot()
    {
        mInOpen = false;
        if(mIsFinishScrShot)
        {
            return;
        }
        ScreenShotManager.Instance.CaptureScreenshot();
        if (CanvasGr != null)
        {
            CanvasGr.DOFade(0, 0.3f).SetDelay(0.2f).OnComplete(() => { ToClose(); }).Play();
        }
        mIsFinishScrShot = true;
    }

    private void ToClose()
    {
        CUIManager.Instance.CloseForm(UIFormName.ScreenShotForm);
    }

    private void ToScrShotAndCloe(PointerEventData data)
    {
        if(!mIsFinishScrShot && !mInOpen)
            DoScreenShot();
    }
}

using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.UI;

public class CatLoading : BaseUIForm {

    public float LoadDuration;
    private Image LoadingBG, LogoImage;
    private ProgressBar progressBar;
    private Text Percentage;
    private string orignalString;
    private CanvasGroup BGCanvasGroup;
    Action<bool> onComplete;

    bool m_isComplete = false;

    float m_progress = 0;

    private bool mCoverImageIsLoaded = false;

    private bool isPreLoadResComplete = false;//需要加载的资源是否已经全部加入字典中

    public override void OnOpen()
    {
        base.OnOpen();

        LogoImage = transform.Find("Canvas/BG").GetComponent<Image>();
        //LoadingBG = transform.Find("Canvas/IconBG").GetComponent<Image>();
        progressBar = transform.Find("Canvas/ProgressBar").GetComponent<ProgressBar>();
        Percentage = transform.Find("Canvas/ProgressBar/Percentage").GetComponent<Text>();
        BGCanvasGroup = transform.Find("Canvas/BG").GetComponent<CanvasGroup>();

        if (myForm != null && myForm.canvasScaler != null)
            myForm.canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;

        UiTween();
        //LoadingBG.gameObject.SetActive(false);
        mCoverImageIsLoaded = false;
        SetProgress(0, true);

        CatResourcesSystem.Instance.NewCatPreLoadRes(isPreLoadResCompleteF);

        //loadFinish();
    }

    private void isPreLoadResCompleteF(bool BOOL)
    {
        Debug.Log("资源已经全部加入字典中");
        isPreLoadResComplete = BOOL;
    }

    public override void OnClose()
    {
        base.OnClose();

        //Debug.Log("--------BookLoadingForm   onClose---------->");

        progressBar.gameObject.transform.rectTransform().DOKill();
    }

    /// <summary>
    /// 界面tween动画
    /// </summary>
    private void UiTween()
    {
        progressBar.gameObject.transform.rectTransform().DOAnchorPosY(123, 0.6f).SetEase(Ease.InOutBack).Play();
    }

    private void HideProgressBar()
    {
        progressBar.gameObject.transform.rectTransform().DOAnchorPosY(-100, 0.3f).SetEase(Ease.InOutBack).Play().OnComplete(() =>
        {
           
            CUIManager.Instance.CloseForm(UIFormName.CatLoading);
            CUIManager.Instance.OpenForm(UIFormName.CatMain);
            CUIManager.Instance.OpenForm(UIFormName.CatTop);
           
        });
    }
  
    Tweener tweener;
    public override void CustomUpdate()
    {
        base.CustomUpdate();
      
        if (isPreLoadResComplete)
        {
            float p = CatResourcesSystem.Instance.GetPreLoadProgress();
            if (CatResourcesSystem.Instance.IsCatPreLoadDone())
            {
                m_isComplete = true;
                isPreLoadResComplete = false;
                p = 1;
            }
            DoProgress(p);
        }
    }


    float m_toProgress = 0;
    void DoProgress(float p)
    {
        if (!m_isComplete && m_toProgress == p)
        {
            return;
        }
        m_toProgress = p;
        if (tweener != null)
        {
            //DOTween.Kill(this);
            tweener.Kill();
            tweener = null;
        }
        tweener = DOTween.To(
            () =>
            {
                return m_progress;
            },
            (value) =>
            {
                SetProgress(value);
            }
            , m_toProgress
            , this.LoadDuration)
            .SetEase(Ease.Flash)
            .SetId(this);
        if (m_isComplete)
        {
            tweener.OnComplete(() =>
            {
                SetProgress(1);
                CTimerManager.Instance.AddTimer(0, 1, (_) =>
                {
                    loadFinish();
                });
            });
        }
    }

    void SetProgress(float p, bool bForce = false)
    {
        if (bForce)
        {
            m_progress = p;
        }
        else
        {
            m_progress = Mathf.Max(m_progress, p);
        }
        progressBar.Value = p;
        Percentage.text = string.Concat(orignalString, (System.Math.Round(p, 3) * 100), " %");
    }

    private void loadFinish()
    {
        HideProgressBar();
    }
}

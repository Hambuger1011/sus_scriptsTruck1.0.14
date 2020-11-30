
using DG.Tweening;
using AB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UGUI;
using System;

public class BookLoadingForm : BaseUIForm {

    public float LoadDuration;
    public Image LogoImage;
    public Image LoadingBG;
    public ProgressBar progressBar;
    public Text Percentage;
    public string orignalString;
    public CanvasGroup BGCanvasGroup;
    Action<bool> onComplete;
   
    bool m_isComplete = false;
    float m_progress = 0;

// #if NOT_USE_LUA
//
//     public override void OnOpen()
//     {
//         base.OnOpen();
//
//         if (myForm != null && myForm.canvasScaler != null)
//             myForm.canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
//
//         UiTween();
//         LogoImage.gameObject.SetActive(false);
//
//         SetProgress(0,true);
//
//         if (LoadingBG != null)
//         {
//             if (GameUtility.IsLongScreen())
//             {
//                 LoadingBG.rectTransform().anchorMax = new Vector2(0.5f, 1);
//                 LoadingBG.rectTransform().anchorMin = new Vector2(0.5f, 0);
//                 LoadingBG.rectTransform().offsetMax = new Vector2(LoadingBG.rectTransform().offsetMax.x, 0);
//                 LoadingBG.rectTransform().offsetMin = new Vector2(LoadingBG.rectTransform().offsetMin.x, 0);
//             }
//             else
//             {
//                 LoadingBG.rectTransform().anchorMax = new Vector2(0.5f, 0.5f);
//                 LoadingBG.rectTransform().anchorMin = new Vector2(0.5f, 0.5f);
//             }
//         }
//     }
//
//     public override void OnClose()
//     {
//         base.OnClose();
//
//         //Debug.Log("--------BookLoadingForm   onClose---------->");
//
//         progressBar.gameObject.transform.rectTransform().DOKill();
//     }
//
//     /// <summary>
//     /// 界面tween动画
//     /// </summary>
//     private void UiTween()
//     {
//         progressBar.gameObject.transform.rectTransform().DOAnchorPosY(120, 0.6f).SetEase(Ease.InOutBack).Play();
//     }
//
//     private void HideProgressBar()
//     {
//         progressBar.gameObject.transform.rectTransform().DOAnchorPosY(-100, 0.3f).SetEase(Ease.InOutBack).Play().OnComplete(() =>
//         {
//             CUIManager.Instance.CloseForm(UIFormName.BookLoadingForm);
//             DialogDisplaySystem.Instance.StartReading();
//         });
//     }
//     public void SetCoverImage(Sprite spt)
//     {
//         LogoImage.gameObject.SetActive(false);
//         LoadingBG.sprite = spt;
//     }
//
//     Tweener tweener;
//     public override void CustomUpdate()
//     {
//         base.CustomUpdate();
//
// 		if(m_isComplete)
// 		{
// 			return;
// 		}
//         
// 		float p = DialogDisplaySystem.Instance.GetPreLoadProgress();
// 		if (DialogDisplaySystem.Instance.IsPreLoadDone())
// 		{
// 			m_isComplete = true;
//         }
//         DoProgress(p);
//     }
//
//
//     float m_toProgress = 0;
//     void DoProgress(float p)
//     {
//         if (!m_isComplete && m_toProgress == p)
//         {
//             return;
//         }
//         m_toProgress = p;
// #if true
//         if (tweener != null)
//         {
//             //DOTween.Kill(this);
//             tweener.Kill();
//             tweener = null;
//         }
//         tweener = DOTween.To(
//             () =>
//             {
//                 return m_progress;
//             },
//             (value) =>
//             {
//                 SetProgress(value);
//             }
//             , m_toProgress
//             , this.LoadDuration)
//             .SetEase(Ease.Flash)
//             .SetId(this);
//         if (m_isComplete)
//         {
//             tweener.OnComplete(() =>
//             {
//                 SetProgress(1);
//                 CTimerManager.Instance.AddTimer(0, 1, (_) =>
//                 {
//                     loadFinish();                  
//                 });
//             });
//         }
// #else
//         if (m_isComplete)
//         {
//             SetProgress(1);
//             CTimerManager.Instance.AddTimer(0, 1, (_) =>
//             {
//                 loadFinish();
//             });
//         }else
//         {
//             SetProgress(p);
//         }
// #endif
//     }
//
//     void SetProgress(float p,bool bForce = false)
//     {
//         if(bForce)
//         {
//             m_progress = p;
//         }else
//         {
//             m_progress = Mathf.Max(m_progress, p);
//         }
//         progressBar.Value = m_progress;
//         Percentage.text = string.Concat(orignalString, (System.Math.Round(m_progress, 3) * 100), " %");
//     }
//
//     private void loadFinish()
//     {
//         HideProgressBar();
//     }
// #endif
}
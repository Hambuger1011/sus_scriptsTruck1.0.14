using System;
using AB;
using DG.Tweening;
using Framework;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpdateRunningPanel : MonoBehaviour
{
    public GameObject BgObj;
    private Text Title;
    private Text ContentText;
    private Image ProgressBarSlider;
    private Text ProgressBarText;
    private Text SizeText;

    bool m_isComplete = true;

    private void Awake()
    {
        this.BgObj = DisplayUtil.GetChild(this.gameObject, "BgObj");
        this.Title = DisplayUtil.GetChildComponent<Text>(this.gameObject, "Title");
        this.ContentText = DisplayUtil.GetChildComponent<Text>(this.gameObject, "ContentText");
        this.ProgressBarSlider = DisplayUtil.GetChildComponent<Image>(this.gameObject, "ProgressBarSlider");
        this.ProgressBarText = DisplayUtil.GetChildComponent<Text>(this.gameObject, "ProgressBarText");

        this.SizeText = DisplayUtil.GetChildComponent<Text>(this.gameObject, "SizeText");

      //  UIEventListener.AddOnClickListener(FeedbackButton, FeedbackButtonOnClick);

    }

    public Action<bool> onComplete = null;

    public void StartLoading2(Action<bool> callback)
    {
        m_isComplete = false;
        onComplete = callback;
        ABSystem.ui.PreLoad();
    }

    public void CustomUpdate()
    {
        if (m_isComplete)
        {
            return;
        }

        float p = ABSystem.ui.GetPreLoadProgress();
        int allsize = ABSystem.ui.AllSize;
        //Debug.Log("进度：" + p);
        if (!m_isComplete && ABSystem.ui.IsPreLoadDone())
        {
            m_isComplete = true;
            DoProgress(1, allsize, true);
        }
        else
        {
            DoProgress(p, allsize, false);
        }

    }

    public float LoadDuration;
    float m_progress = 0;
    Tweener tweener, tweeners;
    float m_toProgress = 0;
    void DoProgress(float p, int _allsize, bool isComplete = false)
    {
        if (!m_isComplete && m_toProgress == p)
        {
            return;
        }
        if (p > m_toProgress)
        {
            m_toProgress = p;
        }

#if true
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
                SetProgress(value, _allsize);
            }
            , m_toProgress
            , this.LoadDuration)
            .SetEase(Ease.Flash)
            .SetId(this);
        if (m_isComplete)
        {
            tweener.OnComplete(() =>
            {
                SetProgress(1, _allsize,true);
                CTimerManager.Instance.AddTimer(500, 1, (_) =>
                {
                    loadFinish();
                });
            });
        }
#else

        if (isComplete)
        {
            Debug.LogError("load complete!!!");
            SetProgress(1);
            CTimerManager.Instance.AddTimer(0, 1, (_) =>
            {
                loadFinish();
            });
        }
        else
        {
            SetProgress(p);
        }
#endif
    }

    internal void SetData()
    {
        //【更新】展示公告信息
        string updateContent = UserDataManager.Instance.appconfinfo.messagesInfo.content.update;

        if (updateContent != null && updateContent != "")
        {
            this.ContentText.text=UserDataManager.Instance.appconfinfo.messagesInfo.content.update;
        }

       
    }
    private bool showed = false;
    private float mPersent = 1f;
    public void SetProgress(float p, int _allSize, bool bForce = false)
    {
         if (bForce)
         {
             m_progress = p;
         }
         else
         {
             m_progress = Mathf.Max(m_progress, p);
         }

        float num = m_progress * mPersent;

        this.ProgressBarSlider.fillAmount = num;
        this.ProgressBarText.text = (System.Math.Round(m_progress * mPersent, 3) * 100).ToString("f2") + "%";

        if (m_progress == 1)
        {
            this.ProgressBarText.text = "100%";
        }


        if (_allSize > 0 && showed == false)
        {
            showed = true;
            SizeText.gameObject.SetActive(true);
        }

        if (_allSize > 0)
        {
            float cursizeM = ((_allSize * num) / 1024f / 1024);
            float allsizeM = (_allSize / 1024f / 1024);

            this.SizeText.text = cursizeM.ToString("f2") + "mb/" + allsizeM.ToString("f2") + "mb";
        }
    }


    private void loadFinish()
    {
        //this.ProgressBarSlider.gameObject.SetActiveEx(false);


        if (!m_isComplete)
        {
            m_isComplete = true;

            string version = UserDataManager.Instance.appconfinfo.loginboxInfo.version;
            if (!string.IsNullOrEmpty(version))
            {
                //更新过  存当前版本号
                PlayerPrefs.SetString("updateVersion", version);
            }

            UIUpdateModule updateUI = CUIManager.Instance.GetForm<UIUpdateModule>(UIFormName.UIUpdateModule);
            updateUI.SetPanel(EnumUpdate.UpdateEndPanel);
        }

      
    }

    private void FeedbackButtonOnClick(PointerEventData data)
    {
        //CUIManager.Instance.OpenForm(UIFormName.LoadingFeedBack);
        var uiform = CUIManager.Instance.OpenForm(UIFormName.FAQFeedBack);
        if (uiform != null)
        {
            uiform.GetComponent<FAQFeedBack>().ShowTypeProblem(1);
        }
    }

    public void onClose()
    {
        this.BgObj.SetActiveEx(false);
    }

  
}

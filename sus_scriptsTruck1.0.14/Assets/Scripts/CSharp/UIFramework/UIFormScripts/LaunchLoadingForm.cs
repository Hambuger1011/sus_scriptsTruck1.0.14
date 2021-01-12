using DG.Tweening;
using AB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UGUI;
using System;
using UnityEngine.EventSystems;

using Random = UnityEngine.Random;
using UnityEngine.Networking;
using System.IO;
using IGG.SDK.Core.Error;
using Script.Game.Helpers;
#if !NOT_USE_LUA
using DialogDisplaySystem = BookReadingWrapper;
#endif

public class LaunchLoadingForm : BaseUIForm
{
    public GameObject FeedbackButton;
    public float LoadDuration;
    public Image LoadingBG, IconBG;
    public ProgressBar progressBar;
    public Text Percentage;
    public string orignalString;
    public Text CurLoadContInfoTxt;
    public Text VersionInfoTxt;
    public CanvasGroup BGCanvasGroup;
    public GameObject MiniBarGroup;
    public RectTransform MiniBarRect;
    public Image MiniBarCurProgress;
    public Text ResSizeText;


    //是否为新用户，如果是新用户则需要
    public bool IsNewPlayer = false;

    private GameObject skeGraGo;
    private int TimeSequence = 0;
    bool m_isComplete = true;
    bool mGuideResComplete = false;
    Action<bool> onComplete;

    private float mPersent = 0.7f;

    public override void OnOpen()
    {
        base.OnOpen();
        this.LoadingBG.gameObject.SetActive(false);

        skeGraGo = this.transform.Find("Canvas/BG/LoadingBgAnim").gameObject;

        UIEventListener.AddOnClickListener(FeedbackButton, FeedbackButtonOnClick);

        MiniBarRect = MiniBarGroup.GetComponent<RectTransform>();

        if (myForm != null && myForm.canvasScaler != null)
            myForm.canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

        SetMiniProgressStage(true);
        SetConfigProgress(0f);
        UiTween();
        SetProgress(0, 0, true);


        if (LoadingBG != null)
        {
            //if (GameUtility.IsLongScreen())
            //{
            //    LoadingBG.rectTransform().anchorMax = new Vector2(0.5f, 1);
            //    LoadingBG.rectTransform().anchorMin = new Vector2(0.5f, 0);
            //    LoadingBG.rectTransform().offsetMax = new Vector2(LoadingBG.rectTransform().offsetMax.x, 0);
            //    LoadingBG.rectTransform().offsetMin = new Vector2(LoadingBG.rectTransform().offsetMin.x, 0);
            //}
            //else
            //{
            //    LoadingBG.rectTransform().anchorMax = new Vector2(0.5f, 0.5f);
            //    LoadingBG.rectTransform().anchorMin = new Vector2(0.5f, 0.5f);
            //}
        }
        this.StartCoroutine(LoadCoverImage());

        //IconBG.gameObject.SetActiveEx(false);
        // FeedbackButton.SetActive(false);
        // CTimerManager.Instance.AddTimer(15000, 1, ShowFeedbackTimeHandler);
    }
    public override void OnClose()
    {
        base.OnClose();

        UIEventListener.RemoveOnClickListener(FeedbackButton, FeedbackButtonOnClick);

        //CUIManager.Instance.CloseForm(UIFormName.LoadingFeedBack);
    }

    // private void ShowFeedbackTimeHandler(int value)
    // {
    //     if(FeedbackButton == null)
    //     {
    //         return;
    //     }
    //     //Debug.Log("------------ShowFeedbackTimeHandler----------->>");
    //     FeedbackButton.SetActive(true);
    // }


    public void SetMiniProgressStage(bool value)
    {
        if (MiniBarGroup != null)
            MiniBarGroup.SetActive(value);
    }

    public void SetConfigProgress(float value)
    {
        if (MiniBarCurProgress != null)
            MiniBarCurProgress.fillAmount = value;
    }
    private void FeedbackButtonOnClick(PointerEventData data)
    {
        //CUIManager.Instance.OpenForm(UIFormName.LoadingFeedBack);
        // var uiform = CUIManager.Instance.OpenForm(UIFormName.FAQFeedBack);
        // if(uiform != null)
        // {
        //     uiform.GetComponent<FAQFeedBack>().ShowTypeProblem(1);
        // }
        //IGGSDKManager.Instance.TshInit();
        // var bundle = KungfuInstance.Get().GetPreparedURLBundle();
        // bundle.LivechatURL(HandleOpenURLFromURLBundle);
        IGGSDKManager.Instance.OpenTSH();
    }
    
    private static void HandleOpenURLFromURLBundle(IGGError error, string url)
    {
        LOG.Info("--------->>>"+url);
        IGGNativeUtils.ShareInstance().OpenBrowser(url);
    }
    
    /// <summary>
    /// 界面tween动画
    /// </summary>
    private void UiTween()
    {
        float targetPosY = MiniBarRect.anchoredPosition.y - 50;

        BGCanvasGroup.alpha = 1;
        progressBar.gameObject.transform.rectTransform().DOAnchorPosY(targetPosY, 1f).SetDelay(0.4f).SetEase(Ease.InOutBack).Play();
        IconBG.transform.rectTransform().anchoredPosition = new Vector3(0, 430, 0);
        IconBG.color = new Color(1, 1, 1, 0);
        IconBG.DOColor(new Color(1, 1, 1, 1), 1f).SetDelay(0.4f).Play();
        IconBG.gameObject.transform.rectTransform().DOAnchorPosY(375, 1.5f).SetDelay(0.6f).SetEase(Ease.InOutBack);
    }

    private void HideProgressBar()
    {
        progressBar.gameObject.transform.rectTransform().DOAnchorPosY(-100, 0.3f).SetEase(Ease.InOutBack).OnComplete(() =>
        {
            progressBar.gameObject.SetActiveEx(false);
            //存储所有需要到的文本，内容
            GameDataMgr.Instance.table.GetLocalizationData();

            SdkMgr.Instance.Start();
            onComplete(true);
        }).Play();
    }

    public void HideLogo()
    {
        IconBG.transform.localScale = Vector3.one;
        IconBG.rectTransform().DOAnchorPos(new Vector3(216, 1248), 1).SetDelay(0.3f).SetEase(Ease.InBack).Play();
        IconBG.transform.DOScale(0.3f, 1).SetEase(Ease.InOutBack).OnComplete(() =>
        {
            IconBG.transform.DOScale(0.4f, 0.4f).SetEase(Ease.OutBack).Play();
        }).Play();
    }

    float m_progress = 0;
    Tweener tweener, tweeners;
    public override void CustomUpdate()
    {
        base.CustomUpdate();
        if (m_isComplete)
        {
            return;
        }

        float p = ABSystem.ui.GetPreLoadProgress();
        int allsize = ABSystem.ui.AllSize;
        // int cursize = ABSystem.ui.GetCurSize();
        //
        // float p = (float)cursize / (float)allsize;
        // if (allsize == 0) { p = 0; }
        // if (allsize == -1) { p = 1; }

        // Debug.Log("进度：" +cursize);
        // Debug.Log("进度1：" + allsize);
        // Debug.Log("进度p：" + p);

        if (!m_isComplete && ABSystem.ui.IsPreLoadDone() && ABSystem.ui.BannerLoadOk)
        {
            m_isComplete = true;
            DoProgress(1, allsize, true);
        }
        else
        {
            DoProgress(p, allsize, false);
        }
    }

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
                SetProgress(1, _allsize);
                CTimerManager.Instance.AddTimer(0, 1, (_) =>
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

    private bool showed = false;
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
        progressBar.Value = num;
        Percentage.text = (System.Math.Round(m_progress * mPersent, 3) * 100).ToString("f2") + "%";

        if (_allSize > 0 && showed == false)
        {
            showed = true;
            ResSizeText.gameObject.SetActive(true);
        }

        if (_allSize > 0)
        {
            float cursizeM = ((_allSize * num) / 1024f / 1024);
            float allsizeM = (_allSize / 1024f / 1024);

            this.ResSizeText.text = cursizeM.ToString("f2") + "mb/" + allsizeM.ToString("f2") + "mb";
        }

    }

    //private void startLoading()
    //{
    //    progressBar.Value = 0;
    //    DOTween.To(() => 0f, (value) => { progressBar.Value = value; Percentage.text = orignalString + (System.Math.Round(value, 3) * 100).ToString() + " %"; }, 1f, LoadDuration).SetEase(Ease.Flash)
    //        .OnComplete(() => loadFinish());
    //}

    private void loadFinish()
    {
        //if (IsNewPlayer) //如果是新手，则加载指引的内容
        //{
        //    loadNewFirstRes();
        //}
        //else
        //{
        LoadAllComplete();
        //}
    }

    public void StartLoading(Action<bool> callback)
    {
        //UiTween();
        if (IsNewPlayer)
            mPersent = 0.7f;
        else
            mPersent = 1f;
        onComplete = callback;
        m_isComplete = false;
        mGuideResComplete = false;
        ABSystem.ui.PreLoad();
    }

    private void LoadAllComplete()
    {
        HideProgressBar();

    }


    private void DownloadNewCallBack(int i, UnityObjectRefCount unityObjectRefCount, string version)
    {
        if (i != 0)
        {
            unityObjectRefCount.Release();
            return;
        }
        LOG.Info("下载了新图 version="+version);
        PlayerPrefs.SetString("LoadImageVersion", version);
    }
    private void DownloadOldCallBack(int i, UnityObjectRefCount unityObjectRefCount, string version)
    {
        if (i != 0)
        {
            unityObjectRefCount.Release();
            return;
        }
        this.LoadingBG.sprite = unityObjectRefCount.GetObject() as Sprite;
        this.LoadingBG.gameObject.SetActive(true);
        LOG.Info("下载了旧图 version="+version);
        if (!version.Equals(UserDataManager.Instance.ResVersion))
        {
            DownloadMgr.Instance.DownloadLoadImg(UserDataManager.Instance.ResVersion,DownloadNewCallBack);
        }
    }

    IEnumerator LoadCoverImage()
    {
        var imgVersion = PlayerPrefs.GetString("LoadImageVersion");
        
        if (!string.IsNullOrEmpty(imgVersion))
        {
            DownloadMgr.Instance.DownloadLoadImg(imgVersion,DownloadOldCallBack);
        }
        else
        {
            this.LoadingBG.gameObject.SetActive(true);
            DownloadMgr.Instance.DownloadLoadImg(UserDataManager.Instance.ResVersion,DownloadNewCallBack);
        }
        string url = null;
        uint version = 0;

        var dict = UserDataManager.Instance.GetLoadedCoverImage();
        var urls = dict.Keys;
        int targetIdx = Random.Range(0, urls.Count);
        int idx = -1;
        foreach (var itr in urls)
        {
            ++idx;
            if (idx == targetIdx)
            {
                url = itr;
                version = dict[itr];
                break;
            }
        }
        if (url == null)
        {
            yield break;
        }

        LOG.Info("cache cover image:" + url);
#if UNITY_EDITOR
        if (!ABMgr.Instance.isUseAssetBundle)//编辑器模式
        {
            if (!url.StartsWith("http"))
            {
                var spt = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(url);
                if (spt != null)
                {
                    this.LoadingBG.sprite = spt;
                }
                yield break;
            }
        }
#endif

        switch (AbUtility.loadType)
        {
            #region load by unityWeb
            case enLoadType.eWebUnity:
                {
                    if (!Caching.IsVersionCached(url, new Hash128(0, 0, 0, version)))
                    {
                        yield break;
                    }
#if UNITY_2018_1_OR_NEWER
                    using (var req = UnityWebRequestAssetBundle.GetAssetBundle(url, version, 0))
#else
        using (var req = UnityWebRequest.GetAssetBundle(url, version, 0))
#endif
                    {
                        req.SendWebRequest();
                        while (!req.isDone)
                        {
                            yield return null;
                        }
                        var bundle = DownloadHandlerAssetBundle.GetContent(req);
                        if (bundle != null)
                        {
                            if (skeGraGo != null) skeGraGo.SetActive(false);
                            if (IconBG != null) IconBG.gameObject.SetActive(false);

                            var allSprite = bundle.LoadAllAssets<Sprite>();
                            bundle.Unload(false);
                            this.LoadingBG.sprite = allSprite[0];
                        }
                    }
                }
                break;
            #endregion
            case enLoadType.eFile:
            case enLoadType.eWebClient:
                {
                    if (File.Exists(url))
                    {
                        var ab = AssetBundle.LoadFromFile(url);
                        if (ab != null)
                        {
                            try
                            {
                                if (skeGraGo != null) skeGraGo.SetActive(false);
                               // if (IconBG != null) IconBG.gameObject.SetActive(false);

                                var allSprite = ab.LoadAllAssets<Sprite>();
                                this.LoadingBG.sprite = allSprite[0];
                            }
                            catch (Exception e)
                            {
                                Debug.LogError("加载loading失败:" + e);
                            }
                            finally
                            {
                                ab.Unload(false);
                            }
                        }
                    }
                }
                break;
        }
    }
}

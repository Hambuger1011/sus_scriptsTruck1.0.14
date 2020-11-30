using System;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AB;

public class CommentForm : BaseUIForm
{
    public Button btnBack;

    public RectTransform uiHotPanel;
    public RectTransform uiNewPanel;

   
    public GameObject uiTpl, InputMske;

    public Text txtChapterTitle, repliesChapterTitle;

    public Image chapterIcon;

    public InputField InputField;
    public GameObject SubmitBtn;
    public GameObject RepliesBG,Contentgame;
    public Scrollbar bar;
    public GameObject HotCommentList, NewCommentList;
    public ContentSizeFitter ContentSizeFitter;
    public RepliesBGsprite RepliesBGsprite;
    public GameObject writeButton, InputGame;


    private List<ChapterCommentItemInfo> hotCommentList;
    private List<ChapterCommentItemInfo> newCommentList;

    private int mBookId;
    private int mChapterId;
    private int newpage = 1, pages_total, msgarr_count;

    private string mLastComment;
    private List<UICommentItem> UICommentItemList;

    private Image mTopBg;
    private Text mTopTitle, DailyrefreshText;
    private RectTransform mBackBtn;


    UnityObjectRefCount m_cacheImage;
    public UnityObjectRefCount cacheImage
    {
        get
        {
            return m_cacheImage;
        }
        set
        {
            if (m_cacheImage != null)
            {
                m_cacheImage.Release();
            }
            m_cacheImage = value;
        }
    }


    private void Awake()
    {
        uiTpl.transform.parent.gameObject.SetActiveEx(false);

        mTopBg = transform.Find("Frame/TopBar/BG").GetComponent<Image>();
    }

    public override void OnOpen()
    {
        base.OnOpen();


        float offerH = 0;
        if (ResolutionAdapter.HasUnSafeArea)
        {
            var scale = this.myForm.GetScale();
            var safeArea = ResolutionAdapter.GetSafeArea();
            var offset = this.myForm.Pixel2View(safeArea.position);
            offerH = offset.y;
        }
        mTopBg.rectTransform().sizeDelta = new Vector2(Screen.width, 92 + offerH);
        txtChapterTitle.rectTransform().anchoredPosition = new Vector2(0, - offerH);
        btnBack.transform.rectTransform().anchoredPosition = new Vector2(55,  - offerH);
        Contentgame.transform.rectTransform().offsetMax = new Vector2(22, -(487 + offerH));
        chapterIcon.rectTransform().anchoredPosition = new Vector2(0, -283 - offerH);


        btnBack.onClick.AddListener(OnBackClick);
        UIEventListener.AddOnClickListener(SubmitBtn, InputFieldOKButtonOnclick);

        UIEventListener.AddOnClickListener(writeButton, writeButtonOncklice);
        UIEventListener.AddOnClickListener(InputMske, InputMskeOnclicke);

        newpage = 1;

        UICommentItemList = new List<UICommentItem>();
        UICommentItemList.Clear();

        RepliesBGsprite.RepliesOpen();

        writeButton.SetActive(true);
        InputGame.SetActive(false);
    }

    private void InputMskeOnclicke(PointerEventData data)
    {
        writeButton.SetActive(true);
        InputGame.SetActive(false);
    }

    private void ChapterCommentCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----ChapterCommentCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.chapterCommentList = JsonHelper.JsonToObject<HttpInfoReturn<ChapterCommentCont<ChapterCommentItemInfo>>>(result);
                    if (UserDataManager.Instance.chapterCommentList != null && UserDataManager.Instance.chapterCommentList.data != null)
                    {
                        List<ChapterCommentItemInfo> tempList = UserDataManager.Instance.chapterCommentList.data.msgarr;

                        msgarr_count = UserDataManager.Instance.chapterCommentList.data.msgarr_count;
                        pages_total= UserDataManager.Instance.chapterCommentList.data.pages_total;
                        //LOG.Info("tempList数量：" + tempList.Count+ "--msgarr_count："+ msgarr_count+ "__pages_total:"+ pages_total);

                        if (newpage==1)
                        {
                            //这个是刚开始进来的时候加载物体

                            for (int i=0;i< tempList.Count; i++)
                            {
                               
                                    GameObject go = Instantiate(uiTpl);
                                    go.transform.SetParent(NewCommentList.transform);
                                    //go.GetComponent<UICommentItem>().SetData(tempList[i]);

                                UICommentItem tem = go.GetComponent<UICommentItem>();
                                tem.SetData(tempList[i]);
                                UICommentItemList.Add(tem);

                            }
                        }else
                        {
                            for (int i = 0; i < tempList.Count; i++)
                            {                                
                                    GameObject go = Instantiate(uiTpl);
                                    go.transform.SetParent(NewCommentList.transform);
                                    //go.GetComponent<UICommentItem>().SetData(tempList[i]);

                                UICommentItem tem = go.GetComponent<UICommentItem>();
                                tem.SetData(tempList[i]);
                                UICommentItemList.Add(tem);
                            }
                        }
                        //if (tempList.Count > 3)
                        //{
                        //    hotCommentList = tempList.GetRange(0, 3);
                        //    newCommentList = tempList.GetRange(3, tempList.Count - 3);
                        //}
                        //else
                        //{
                        //    hotCommentList = tempList;
                        //    newCommentList = tempList;
                        //}

                        //int hotLen = hotCommentList.Count;
                        //int newLen = newCommentList.Count;

                        //uiHotCommentList.SetCount(hotLen);
                        //uiNewCommentList.SetCount(newLen);
                    }

                    
                }
                Contentgame.SetActive(true);

                Invoke("ContentSizeFitterFalse",0.2f);
            }, null);
        }
    }

    private void ContentSizeFitterFalse()
    {
        CancelInvoke();
        ContentSizeFitter.enabled = false;
        ContentSizeFitter.enabled = true;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        //uiHotCommentList.onInitElement -= OnInitHotElement;
        //uiNewCommentList.onInitElement -= OnInitNewElement;

        //uiHotCommentList.onResizeContent -= OnHotResizeContent;
        //uiNewCommentList.onResizeContent -= OnNewResizeContent;
        cacheImage = null;
    }

    

    public override void OnClose()
    {
        base.OnClose();

        RepliesBGsprite.RepliceClose();

        btnBack.onClick.RemoveListener(OnBackClick);
        UIEventListener.RemoveOnClickListener(SubmitBtn, InputFieldOKButtonOnclick);
        UIEventListener.RemoveOnClickListener(writeButton, writeButtonOncklice);
        UIEventListener.RemoveOnClickListener(InputMske, InputMskeOnclicke);


        if (UICommentItemList!=null)
        {
            for (int i=0;i< UICommentItemList.Count;i++)
            {
                UICommentItemList[i].Disposte();
            }

            UICommentItemList = null;
        }
    }


    private void writeButtonOncklice(PointerEventData data)
    {
        writeButton.SetActive(false);
        InputGame.SetActive(true);
    }


    public void SetData(int mBookId, int chapterId, string chapterText, Sprite bookChapterBGSprite)
    {
        Contentgame.SetActive(false);
        this.mBookId = mBookId;
        this.mChapterId = chapterId;

        txtChapterTitle.text = chapterText;
        repliesChapterTitle.text = chapterText;
        chapterIcon.sprite = ResourceManager.Instance.GetUISprite("BookDisplayForm/bg_picture"); ;
        ABSystem.ui.DownloadBanner(mBookId, (id,refCount) =>
        {
            if(chapterIcon== null)
            {
                refCount.Release();
                return;
            }
            if(mBookId != id)
            {
                refCount.Release();
                return;
            }
            cacheImage = refCount;
           chapterIcon.sprite = cacheImage.Get<Sprite>();
        });

        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.ViewChapterMessages(mBookId, chapterId,1, ChapterCommentCallBack);
    }

    private void OnBackClick()
    {
        this.myForm.Close();
    }

    private void OnInitNewElement(CUIListView list, int index, CUIListCell element)
    {
        if (newCommentList == null || index >= newCommentList.Count)
        {
            return;
        }
        UICommentItem item = (UICommentItem)element;
        item.SetData(newCommentList[index]);
    }

    private void OnInitHotElement(CUIListView list, int index, CUIListCell element)
    {
        if (hotCommentList == null || index >= hotCommentList.Count)
        {
            return;
        }
        UICommentItem item = (UICommentItem)element;
        item.SetData(hotCommentList[index]);
    }

    private void OnHotResizeContent(Vector2 size)
    {
        uiHotPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y + 58);
    }

    private void OnNewResizeContent(Vector2 size)
    {
        uiNewPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y + 58 - 1042);
    }

    private void InputFieldOKButtonOnclick(PointerEventData data)
    {
        mLastComment = InputField.text;
        if (string.IsNullOrEmpty(mLastComment))
        {
            AudioManager.Instance.PlayTones(AudioTones.LoseFail);
            UITipsMgr.Instance.PopupTips("Place enter your Comment", false);
            return;
        }
        if (mLastComment.Length < 4)
        {
            AudioManager.Instance.PlayTones(AudioTones.LoseFail);
            UITipsMgr.Instance.PopupTips("Write more than 4 characters", false);
            return;
        }
                 
            InputField.text = "";
            mLastComment = UserDataManager.Instance.CheckBannedWords(mLastComment);
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.ChapterComments(mBookId, mChapterId, mLastComment, SendCommentCallBack);
    }

    private void SendCommentCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----SendCommentCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    AudioManager.Instance.PlayTones(AudioTones.RewardWin);
                    UserDataManager.Instance.sendCommentResultInfo = JsonHelper.JsonToObject<HttpInfoReturn<SendCommentResultCont>>(result);
                    if(UserDataManager.Instance.sendCommentResultInfo != null && UserDataManager.Instance.sendCommentResultInfo.data != null)
                    {
                        //UITipsMgr.Instance.PopupTips("Comment on success", false);
                        UITipsMgr.Instance.PopupTips("Comment successful!", false);
                        if (newCommentList == null)
                            newCommentList = new List<ChapterCommentItemInfo>();
                        ChapterCommentItemInfo itemInfo = UserDataManager.Instance.sendCommentResultInfo.data.msgarr;
                        if (itemInfo != null)
                            newCommentList.Insert(0, itemInfo);
                            //uiNewCommentList.SetCount(newCommentList.Count);
                    }

                    //加载一条信息出来
                    GameObject go = Instantiate(uiTpl);
                    go.transform.SetParent(NewCommentList.transform);
                    go.transform.SetSiblingIndex(0);
                    //go.GetComponent<UICommentItem>().MySentInite(mLastComment);

                    UICommentItem tem = go.GetComponent<UICommentItem>();
                    tem.MySentInite(mLastComment);
                    UICommentItemList.Add(tem);

                    writeButton.SetActive(true);
                    InputGame.SetActive(false);
                }
                else if(jo.code == 201)
                {
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                    if (mLastComment.Length < 4)
                    {
                        UITipsMgr.Instance.PopupTips("Write more than 4 characters.", false);
                        return;
                    }
                }
                else if(jo.code == 202)
                {
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                }
                else if(jo.code == 208)
                {
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                }

                Invoke("ContentSizeFitterFalse", 0.2f);
            }, null);
        }
    }

    /// <summary>
    /// 滑动的时候监测
    /// </summary>
    /// <param name="eventData"></param>
    public void scrollbarDown()
    {
        //LOG.Info("scrollbar的值是：" + bar.value);

        if (bar.value<=0)
        {
            //LOG.Info("滑动到底部了");
            newpage++;

            if (newpage> pages_total)
            {
                //LOG.Info("我是有底线的");
                newpage = pages_total;

               
                UITipsMgr.Instance.PopupTips("No more comments.", false);
                return;
            }
            else
            {
                //UINetLoadingMgr.Instance.Show();
                GameHttpNet.Instance.ViewChapterMessages(mBookId, this.mChapterId, newpage, ChapterCommentCallBack);
            }
        }
    }
}

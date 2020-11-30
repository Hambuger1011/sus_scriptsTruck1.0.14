using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using pb;

public class RepliesBGsprite : MonoBehaviour
{
    public RectTransform repliesItemBg;
    public GameObject closeButton;
    public GameObject themeItemBg, ContenterItem, Content;
    public GameObject InputGame, SubmitBtn;
    public InputField InputField;
    public GameObject writeButton, RepliesBGInputGame, RepliesInputMask;

    private string mLastComment;
    private ChapterCommentItemInfo ItemInfo;
    private UICommentItem UICommentItem;
    public ContentSizeFitter contentSizeFitter;

    private int newpage = 1, pages_total=1, type=-1;

    private List<CommenContenterItem> CommenContenterItemList;
    private List<CommentThemeItem> CommentThemeItemList;

    /// <summary>
    /// 这个是repliesItemBg的移动
    /// </summary>
    /// <param name="type"></param>
    private void repliesItemBgMover(int type)
    {
        if (type==1)
        {
            //这个是移进界面
            repliesItemBg.DOAnchorPosY(434f, 0.5f);
        }
        else
        {
            //这个是移出界面
            repliesItemBg.DOAnchorPosY(-434f, 0.5f).OnComplete(()=> {

                transform.gameObject.SetActive(false);

            });
        }
    }
   
    public void RepliesOpen()
    {
        CommenContenterItemList = new List<CommenContenterItem>();
        CommenContenterItemList.Clear();

        //Debug.Log("打开");
        UIEventListener.AddOnClickListener(SubmitBtn, InputFieldOKButtonOnclick);
        UIEventListener.AddOnClickListener(closeButton, closeButtonOnclicke);

        UIEventListener.AddOnClickListener(writeButton,writeButtonOnclicke);
        UIEventListener.AddOnClickListener(RepliesInputMask,RepliesInputMaskOnclike);

        writeButton.SetActive(true);
        RepliesBGInputGame.SetActive(false);
    }

    public void RepliceClose()
    {
        //Debug.Log("关闭");
        UIEventListener.RemoveOnClickListener(SubmitBtn, InputFieldOKButtonOnclick);
        UIEventListener.RemoveOnClickListener(closeButton, closeButtonOnclicke);

        UIEventListener.RemoveOnClickListener(writeButton, writeButtonOnclicke);
        UIEventListener.RemoveOnClickListener(RepliesInputMask, RepliesInputMaskOnclike);

    }

    private void RepliesInputMaskOnclike(PointerEventData data)
    {
        writeButton.SetActive(true);
        RepliesBGInputGame.SetActive(false);
    }

    private void writeButtonOnclicke(PointerEventData data)
    {
        writeButton.SetActive(false);
        RepliesBGInputGame.SetActive(true);
    }
    /// <summary>
    /// 这个是给评论回复的界面初始化
    /// </summary>
    /// <param name="vItemInfo"></param>
    public void Inis(ChapterCommentItemInfo vItemInfo,GameObject UICommentItem,int liketype)
    {
        CommenContenterItemList = new List<CommenContenterItem>();
        CommenContenterItemList.Clear();

        CommentThemeItemList = new List<CommentThemeItem>();
        CommentThemeItemList.Clear();

        // LOG.Info("生成");
        ItemInfo = vItemInfo;

        type = -1;
        GameObject go = Instantiate(themeItemBg);
        go.SetActive(true);
        go.transform.SetParent(Content.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;

        //go.GetComponent<CommentThemeItem>().Init(vItemInfo, liketype);

        CommentThemeItem tem = go.GetComponent<CommentThemeItem>();
        tem.Init(vItemInfo, liketype);
        CommentThemeItemList.Add(tem);

        this.UICommentItem = UICommentItem.GetComponent<UICommentItem>();
        //Debug.Log("discussid:" + ItemInfo.discussid);
        GameHttpNet.Instance.ChapterCommenBack(ItemInfo.discussid,1, ChapterCommenBackCallback);
    }

    public void CommentThemeItemLikeChange(int type)
    {
        this.type = type;
    }

    /// <summary>
    /// 评论回复信息
    /// </summary>
    /// <param name="arg"></param>
    private void ChapterCommenBackCallback(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----ChapterCommenBackCallback---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.chapterCommenBackInfo = JsonHelper.JsonToObject<HttpInfoReturn<ChapterCommentCont<CommentBackInfo>>>(result);
                    if (UserDataManager.Instance.chapterCommenBackInfo != null && UserDataManager.Instance.chapterCommenBackInfo.data != null)
                    {
                        pages_total = UserDataManager.Instance.chapterCommenBackInfo.data.pages_total;
                        List<CommentBackInfo> tempList = UserDataManager.Instance.chapterCommenBackInfo.data.msgarr;

                        if (tempList.Count>0)
                        {
                            for (int i=0;i<tempList.Count;i++)
                            {
                                GameObject co = Instantiate(ContenterItem);
                                co.SetActive(true);
                                co.transform.SetParent(Content.transform);
                                co.transform.localPosition = Vector3.zero;
                                co.transform.localScale = Vector3.one;
                                                           
                                //co.GetComponent<CommenContenterItem>().Init(tempList[i]);

                                CommenContenterItem tem = co.GetComponent<CommenContenterItem>();
                                tem.Init(tempList[i]);
                                CommenContenterItemList.Add(tem);
                            }
                        }
                    }
                }

                Invoke("contentSizeFitterFalse", 0.2f);

            }, null);
        }
    }

    private void InputFieldOKButtonOnclick(PointerEventData data)
    {
        mLastComment = InputField.text;
        if (string.IsNullOrEmpty(mLastComment))
        {
            AudioManager.Instance.PlayTones(AudioTones.LoseFail);

            var Localization = GameDataMgr.Instance.table.GetLocalizationById(193);
            UITipsMgr.Instance.PopupTips(Localization, false);

            //UITipsMgr.Instance.PopupTips("Place enter your Comment", false);
            return;
        }
        if (mLastComment.Length < 4)
        {
            AudioManager.Instance.PlayTones(AudioTones.LoseFail);

            var Localization = GameDataMgr.Instance.table.GetLocalizationById(167);
            UITipsMgr.Instance.PopupTips(Localization, false);

            //UITipsMgr.Instance.PopupTips("Write more than 4 characters", false);
            return;
        }

        int discussid = ItemInfo.discussid;
        //UINetLoadingMgr.Instance.Show();

        //LOG.Info("discussid:" + discussid + "--mLastComment:" + mLastComment);
        mLastComment = UserDataManager.Instance.CheckBannedWords(mLastComment);

        GameHttpNet.Instance.ChapterCommentSanswering(discussid, mLastComment, ChapterCommentSansweringCallBacke);
    }

    private void ChapterCommentSansweringCallBacke(object arg)
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
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(194);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Comment successful!", false);
                    AudioManager.Instance.PlayTones(AudioTones.RewardWin);
                    //留言成功
                    InputField.text = "";

                    GameObject co = Instantiate(ContenterItem);
                    co.SetActive(true);
                    co.transform.SetParent(Content.transform);
                    co.transform.localPosition = Vector3.zero;
                    co.transform.localScale = Vector3.one;


                    co.transform.SetSiblingIndex(1);
                    //co.GetComponent<CommenContenterItem>().SpawnInit(mLastComment);

                    CommenContenterItem tem = co.GetComponent<CommenContenterItem>();
                    tem.SpawnInit(mLastComment);
                    CommenContenterItemList.Add(tem);

                    writeButton.SetActive(true);
                    RepliesBGInputGame.SetActive(false);
                }
                else if (jo.code == 201)
                {
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                    if (mLastComment.Length < 4)
                    {
                        var Localization = GameDataMgr.Instance.table.GetLocalizationById(167);
                        UITipsMgr.Instance.PopupTips(Localization, false);

                        //UITipsMgr.Instance.PopupTips("Write more than 4 characters.", false);
                        return;
                    }
                }
                else if (jo.code == 202)
                {
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                }
                else if (jo.code == 208)
                {
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                }
                Invoke("contentSizeFitterFalse", 0.2f);
            }, null);

           
        }
    }

    private void closeButtonOnclicke(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        repliesItemBgMover(2);
        //Content.transform.ClearAllChild();

        if (CommenContenterItemList != null)
        {
            for (int i = 0; i < CommenContenterItemList.Count; i++)
            {
                CommenContenterItemList[i].DisPoste();
            }
        }

        if (CommentThemeItemList!=null)
        {
            for (int i=0;i< CommentThemeItemList.Count;i++)
            {
                CommentThemeItemList[i].DisPoste();
            }
        }

        //InputGame.SetActive(true);

        if (this.UICommentItem!=null)
        {
            this.UICommentItem.LikeSprSpriteChange(this.type);
        }
    }

    private void contentSizeFitterFalse()
    {
        CancelInvoke();
        contentSizeFitter.enabled = false;
        contentSizeFitter.enabled = true;
    }

    public void ChapterCommenBackscrollbarDown()
    {
        //Debug.Log("评论回复滑动到底部了，需要刷新消息");

        newpage++;
        if (newpage > pages_total)
        {
            LOG.Info("----我是有底线的----");
            newpage = pages_total;

            var Localization = GameDataMgr.Instance.table.GetLocalizationById(195);
            UITipsMgr.Instance.PopupTips(Localization, false);

            //UITipsMgr.Instance.PopupTips("No more comments.", false);
            return;
        }
        else
        {
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.ChapterCommenBack(ItemInfo.discussid, newpage, ChapterCommenBackCallback);

        }
    }
    private void OnEnable()
    {
        newpage = 1;
        InputGame.SetActive(false);
      
        repliesItemBg.anchoredPosition = new Vector2(0, -434);
        repliesItemBgMover(1);
    }  
}

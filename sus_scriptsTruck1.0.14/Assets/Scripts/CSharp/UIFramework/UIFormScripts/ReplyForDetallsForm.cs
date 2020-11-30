using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UGUI;
using System;
using pb;

public class ReplyForDetallsForm : BaseUIForm
{
    public RectTransform Bg;
    public InputField inputtext;

    private GameObject CloseButton;

    private List<ReplyCommentItem> CommentItemList;
    public override void OnOpen()
    {
        base.OnOpen();

        CloseButton = transform.Find("Canvas/BG/CloseButton").gameObject;

        inputtext.onEndEdit.AddListener(ReplyButtonOnclicke);

        CommentItemList = new List<ReplyCommentItem>();

        //【屏幕适配】
        float offect = XLuaHelper.UnSafeAreaNotFit(this.myForm, null, 750, 120);
        Bg.offsetMin = new Vector2(0, 0);
        Bg.offsetMax = new Vector2(0, -offect);



        UIEventListener.AddOnClickListener(CloseButton, CloseButtonOn);

    }

    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(FeeBack, FeeBackButtonOn);
        UIEventListener.RemoveOnClickListener(CloseButton, CloseButtonOn);
        inputtext.onEndEdit.RemoveListener(ReplyButtonOnclicke);

        if (CommentItemList!=null)
        {
            for (int i=0;i< CommentItemList.Count;i++)
            {
                CommentItemList[i].DisPoste();
            }

            CommentItemList = null;
        }
    }

    public Text Name, ContenText, HardNumberText,time;
    public Image CommentHard,HardBG;
    public GameObject CommentItem, FeeBack;
    public ScrollRect ScrollRect;

    private bool HardHadOnclick = false;
    private commentlist commentlist;
    public void Init(commentlist commentlist)
    {
        UIEventListener.AddOnClickListener(FeeBack, FeeBackButtonOn);
        this.commentlist = commentlist;
        string name = commentlist.username.ToString();
        string ST = "<color=#28A3EAFF>" + name + "</color>";
        Name.text = ST;
        ContenText.text = commentlist.content.ToString();
        HardNumberText.text = commentlist.bestests.ToString();
        time.text = commentlist.ctime.ToString();
        if (commentlist.is_praise == 1)
        {
            //已经点赞
            CommentHard.sprite = ResourceManager.Instance.GetUISprite("EmailForm/bg_icond_03");
        }
        else
        {
            CommentHard.sprite = ResourceManager.Instance.GetUISprite("EmailForm/bg_iconc_03");
            UIEventListener.AddOnClickListener(HardBG.gameObject, HardBgButton);
        }

        //默认是1页
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.Getcommentreplay(commentlist.commentid,1, GetcommentreplayCallBacke);
    }

    private void CloseButtonOn(PointerEventData data)
    {
        CUIManager.Instance.CloseForm(UIFormName.ReplyForDetalls);
    }

    private void FeeBackButtonOn(PointerEventData data)
    {
        CUIManager.Instance.OpenForm(UIFormName.newRCRform);
        NewRCRform NewRCRform = CUIManager.Instance.GetForm<NewRCRform>(UIFormName.newRCRform);
        NewRCRform.NewReplyInit(commentlist);
        NewRCRform.ReplyNewItemInit(gameObject.GetComponent<ReplyForDetallsForm>());
    }

    #region Reply按钮回复功能
    private bool isReplySend = false;
    private string SendString;
    /// <summary>
    /// 这个是打开举报界面进行评论调用的方法
    /// </summary>
    /// <param name="vSt"></param>
    public void ReplySend(string vSt)
    {
        isReplySend = true;
        SendString = vSt;
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.Setcomment(1, 1, commentlist.commentid, vSt,0, SetcommentCallBack);
    }
    
    private void ReplyButtonOnclicke(string vSt)
    {      
        //新闻页举报
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.Setcomment(1, 1, commentlist.commentid, inputtext.text,0, SetcommentCallBack);    
    }
    private void SetcommentCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----SetcommentreportCallback---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(154);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Reply success!", false);

                    GameObject go = Instantiate(CommentItem);
                    go.transform.SetParent(ScrollRect.content.transform);
                    go.SetActive(true);
                    go.transform.SetSiblingIndex(2);

                    string MyName= UserDataManager.Instance.userInfo.data.userinfo.nickname;
                    string ReplyName= commentlist.username.ToString();
                    string Conten = "";
                    if (isReplySend)
                    {
                        isReplySend = false;
                        Conten = SendString;
                    }
                    else
                    {
                        Conten = inputtext.text;
                    }

                    int month= DateTime.Now.Month;
                    int day = DateTime.Now.Day;
                    int hour = DateTime.Now.Hour;
                    int minute = DateTime.Now.Minute;
                    string time = string.Format("{0:D2}-{1:D2}"+" "+ "{2:D2}:{3:D2}", month, day, hour, minute);

                    //go.GetComponent<ReplyCommentItem>().SendInit(MyName, ReplyName, Conten, time);

                    ReplyCommentItem tem = go.GetComponent<ReplyCommentItem>();
                    tem.SendInit(MyName, ReplyName, Conten, time);
                    CommentItemList.Add(tem);

                    inputtext.text = "";
                }

            }, null);
        }
    }
    #endregion

    #region 评论点赞功能
    private void HardBgButton(PointerEventData data)
    {
        if (!HardHadOnclick && commentlist.is_praise == 0)
        {
            //没有点过赞的
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.Setcommentpraise(1, 1, commentlist.commentid, SetcommentpraiseCallBack);
        }
    }

    private void SetcommentpraiseCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----SetcommentpraiseCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    HardHadOnclick = true;
                    commentlist.is_praise = 1;
                    CommentHard.sprite = ResourceManager.Instance.GetUISprite("EmailForm/bg_icond_03");
                    HardNumberText.text = commentlist.bestests + 1 + "";
                }

            }, null);
        }
    }
    #endregion

    #region 获取取评论列表
    private void GetcommentreplayCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetcommentreplayCallBacke---->" + result);
        if (result.Equals("error"))
        {
            LOG.Info("---GetcommentreplayCallBacke--这条协议错误");
            return;
        }
        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();
            JsonObject jo = JsonHelper.JsonToJObject(result);

            if (jo.code == 200)
            {
                UserDataManager.Instance.Getcommentreplay = JsonHelper.JsonToObject<HttpInfoReturn<Getcommentreplay>>(result);

                if (UserDataManager.Instance.Getcommentreplay == null)
                {
                    return;
                }

                List<commentlists> tem = UserDataManager.Instance.Getcommentreplay.data.commentlist;
                if (tem.Count <= 0)
                {
                    LOG.Info("这条评论没有评论");
                    return;
                }

                for (int i = 0; i < tem.Count; i++)
                {
                    GameObject go = Instantiate(CommentItem);
                    go.transform.SetParent(ScrollRect.content.transform);
                    go.SetActive(true);
                    //go.GetComponent<ReplyCommentItem>().Init(tem[i]);

                    ReplyCommentItem rep = go.GetComponent<ReplyCommentItem>();
                    rep.Init(tem[i]);
                    CommentItemList.Add(rep);
                }
            }
        }, null);
    }
    #endregion

    #region 加载下一页的评论列表，和实例化出来
    private int page = 1;
    public void NexPag()
    {
        if (UserDataManager.Instance.Getcommentreplay==null)
        {
            return;
        }

        page++;
        if (page> UserDataManager.Instance.Getcommentreplay.data.pages_total)
        {
            LOG.Info("没有下一页了");
            return;
        }
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.Getcommentreplay(commentlist.commentid, page, GetcommentreplayCallBacke);
    }
    #endregion
}

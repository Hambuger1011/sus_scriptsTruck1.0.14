using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UGUI;
using pb;

public class NewRCRform : BaseUIForm {

    public GameObject mask,Reply,Copy,Report;
    public Text ConterText;
    public InputField inputtext;

    private int com_type;
    private string ConterSt;
    private commentlist commentlist;
    private commentlists commentlists;
    private bool isNews = false;

    public override void OnOpen()
    {
        base.OnOpen();
        UIEventListener.AddOnClickListener(mask, CloseUI);
        //UIEventListener.AddOnClickListener(Reply, ReplyButtonOnclicke);
        UIEventListener.AddOnClickListener(Copy,CopeButtonOnclicke);
        UIEventListener.AddOnClickListener(Report, ReportButtonOnclicke);
        inputtext.onEndEdit.AddListener(ReplyButtonOnclicke);
    }

    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(mask, CloseUI);
        //UIEventListener.RemoveOnClickListener(Reply, ReplyButtonOnclicke);
        UIEventListener.RemoveOnClickListener(Copy, CopeButtonOnclicke);
        UIEventListener.RemoveOnClickListener(Report, ReportButtonOnclicke);
        inputtext.onEndEdit.RemoveListener(ReplyButtonOnclicke);
    }

    private void CloseUI(PointerEventData data)
    {
        CUIManager.Instance.CloseForm(UIFormName.newRCRform);
    }

    #region 新闻界面打开时候的初始化

    /// <summary>
    /// 这个是在新闻界面，点击打开这个界面时候的实例化
    /// </summary>
    public void NewReplyInit(commentlist commentlist)
    {
        isNews =true;
        this.commentlist = commentlist;
        string name = commentlist.username.ToString() + ": ";
        string ST = "<color=#28A3EAFF>" + name + "</color>" + commentlist.content;
        ConterText.text = ST;
        ConterSt= name+commentlist.content;//文本框中的内容
    }
    #endregion

    #region 评论界面里面的，评论详细信息，打开这个界面的时候初始化
    public void ReplyForInit(commentlists commentlists)
    {
        isNews =false;
        this.commentlists = commentlists;
        string touid = commentlists.to_uid + "";
        if (string.IsNullOrEmpty(touid))
        {
            //没有被回复用户
            string name = commentlists.username.ToString() + ": ";
            string ST = "<color=#28A3EAFF>" + name + "</color>" + commentlists.content;
            ConterText.text = ST;
            ConterSt = name + commentlists.content;//文本框中的内容
        }
        else
        {
            //有被回复用户
            string name = commentlists.username.ToString();
            string replyName= commentlists.to_username.ToString();
            string ST = "<color=#28A3EAFF>" + name + "</color>" +" reply "+ "<color=#28A3EAFF>"+ replyName+ "</color>"+" : " + commentlists.content;
            ConterText.text = ST;
            ConterSt = name + commentlists.content;//文本框中的内容
        }       
    }
    #endregion

    #region 评论界面的回复界面打开，举报界面时，传入需要的参数 

    private string ConterStext;
    private int isReply = 0;
    private ReplyForDetallsForm ReplyForDetallsForm;
    private ReplyCommentItem ReplyCommentItem;
    /// <summary>
    /// 这个是评论界面的回复界面，新闻信息打开界面时候
    /// </summary>
    public void ReplyNewItemInit(ReplyForDetallsForm ReplyForDetallsForm)
    {
        isReply = 1;
        this.ReplyForDetallsForm = ReplyForDetallsForm;

    }
    public void ReplyCommentItemInit(ReplyCommentItem ReplyCommentItem)
    {
        isReply = 2;
        this.ReplyCommentItem = ReplyCommentItem;

    }
    #endregion

    #region Reply按钮回复功能
    private void ReplyButtonOnclicke(string vSt)
    {
        
        if (isNews)
        {
            //新闻页举报
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.Setcomment(2, 1, commentlist.commentid, inputtext.text,0, SetcommentCallBack);
        }
        else
        {
            //评论页举报
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.Setcomment(3, 1, commentlists.replyid, inputtext.text,0, SetcommentCallBack);
        }
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
                    ConterStext = inputtext.text;
                    inputtext.text = "";

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(154);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Reply success!", false);

                    if (isReply==1)
                    {
                        ReplyForDetallsForm.ReplySend(ConterStext);
                        isReply = 0;
                        return;
                    }else if (isReply == 2)
                    {
                        ReplyCommentItem.ReplyCommentItemSend(ConterStext);
                        isReply = 0;
                        return;
                    }
                }

            }, null);
        }
    }
    #endregion

    #region Cope按钮功能
    private void CopeButtonOnclicke(PointerEventData data)
    {
        LOG.Info("Copy");
#if UNITY_IOS
            SdkMgr.Instance.shareSDK.CopyToClipboard(ConterSt);
#elif UNITY_EDITOR
        TextEditor t = new TextEditor();
        t.content = new GUIContent(ConterSt);
        t.OnFocus();
        t.Copy();
#elif UNITY_ANDROID
            SdkMgr.Instance.shareSDK.CopyToClipboard(ConterSt);
#endif

        var Localization = GameDataMgr.Instance.table.GetLocalizationById(181);
        UITipsMgr.Instance.PopupTips(Localization, false);

        //UITipsMgr.Instance.PopupTips("Copy success", false);

    }
    #endregion

    #region Report按钮举报功能
    private void ReportButtonOnclicke(PointerEventData data)
    {
        LOG.Info("Report");

        if (isNews)
        {
            //新闻页举报
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.Setcommentreport(1,1, commentlist.commentid, SetcommentreportCallback);
        }else
        {
            //评论页举报
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.Setcommentreport(1, 2, commentlists.replyid, SetcommentreportCallback);
        }
    }

    private void SetcommentreportCallback(object arg)
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
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(136);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Report success!", false);
                }

            }, null);
        }
    }
    #endregion


}

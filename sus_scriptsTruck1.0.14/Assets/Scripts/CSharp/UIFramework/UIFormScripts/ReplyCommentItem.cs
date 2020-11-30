using pb;
using System;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReplyCommentItem : MonoBehaviour {
    public Text Name, ContenText, HardNumberText, Time;
    public Image CommentHard;
    public GameObject HardBG,FeeBack, CommentItem;
    public InputField inputtext;
    public ScrollRect ScrollRect;

    private bool HardHadOnclick = false;
    private commentlists commentlists;

    private bool IsInit = false;
    private void Start()
    {
        if (gameObject.name.Equals("ReplyCommentItem"))
        {
            Invoke("ChangeHeight", 0.3f);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public void SendInit(string MyName,string ReplyName,string Conten,string time)
    {
        string ST = "<color=#28A3EAFF>" + MyName + "</color>" + " reply " + "<color=#28A3EAFF>" + ReplyName + "</color>";
        Name.text = ST;
        ContenText.text = Conten.ToString();
        HardNumberText.text ="0";
        CommentHard.sprite = ResourceManager.Instance.GetUISprite("EmailForm/bg_iconc_03");
        Time.text = time.ToString();
        //ChangeHeight();

        Invoke("ChangeHeight", 0.3f);
    }
    public void Init(commentlists commentlists)
    {
        IsInit = true;
        UIEventListener.AddOnClickListener(FeeBack, FeeBackButtonOn);
        inputtext.onEndEdit.AddListener(ReplyButtonOnclicke);

        this.commentlists = commentlists;
        Time.text = commentlists.ctime;
        if (string.IsNullOrEmpty(commentlists.to_uid.ToString()))
        {
            //没有被回复用户
            string name = commentlists.username.ToString() + ": ";
            string ST = "<color=#28A3EAFF>" + name + "</color>";
            Name.text = ST;          
        }
        else
        {
            //有被回复用户
            string name = commentlists.username.ToString();
            string replyName = commentlists.to_username.ToString();          
            string ST = "<color=#28A3EAFF>" + name + "</color>" + " reply " + "<color=#28A3EAFF>" + replyName + "</color>";
            Name.text = ST;            
        }
       
        ContenText.text = commentlists.content.ToString();
        HardNumberText.text = commentlists.bestests.ToString();

        if (commentlists.is_praise == 1)
        {
            //已经点赞
            CommentHard.sprite = ResourceManager.Instance.GetUISprite("EmailForm/bg_icond_03");
        }
        else
        {
            CommentHard.sprite = ResourceManager.Instance.GetUISprite("EmailForm/bg_iconc_03");
            UIEventListener.AddOnClickListener(HardBG.gameObject, HardBgButton);
        }

        //ChangeHeight();
        Invoke("ChangeHeight",0.3f);
    }

    private void FeeBackButtonOn(PointerEventData data)
    {
        CUIManager.Instance.OpenForm(UIFormName.newRCRform);
        if (IsInit)
        {
            NewRCRform NewRCRform = CUIManager.Instance.GetForm<NewRCRform>(UIFormName.newRCRform);
            NewRCRform.ReplyForInit(commentlists);
            NewRCRform.ReplyCommentItemInit(gameObject.GetComponent<ReplyCommentItem>());
        }   
    }
    #region Reply按钮回复功能
    private bool isReplyCommentItemSend = false;
    private string SendString;
    /// <summary>
    /// 这个是回复打开举报界面，举报界面回复，调用
    /// </summary>
    /// <param name="vStr"></param>
    public void ReplyCommentItemSend(string vStr)
    {
        isReplyCommentItemSend = true;
        SendString = vStr;
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.Setcomment(3, 1, commentlists.replyid, vStr,0, SetcommentCallBack);
    }
    private void ReplyButtonOnclicke(string vSt)
    {
        //新闻页举报
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.Setcomment(3, 1, commentlists.replyid, inputtext.text,0, SetcommentCallBack);
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
                    go.transform.localScale = Vector3.one;
                    go.transform.localPosition = Vector3.zero;

                    string MyName = UserDataManager.Instance.userInfo.data.userinfo.nickname;
                    string ReplyName = commentlists.username.ToString();


                    string Conten ="";
                    if (isReplyCommentItemSend)
                    {
                        isReplyCommentItemSend = false;
                        Conten = SendString;
                    }
                    else
                    {
                        Conten = inputtext.text;
                    }

                    int month = DateTime.Now.Month;
                    int day = DateTime.Now.Day;
                    int hour = DateTime.Now.Hour;
                    int minute = DateTime.Now.Minute;
                    string time = string.Format("{0:D2}-{1:D2}" +" "+ "{2:D2}:{3:D2}", month, day, hour, minute);

                    go.GetComponent<ReplyCommentItem>().SendInit(MyName, ReplyName, Conten, time);
                    inputtext.text = "";

                   
                }

            }, null);
        }
    }
    #endregion
    #region 物体的高度根据文本的高度自动适应变化
    private void ChangeHeight()
    {
        CancelInvoke("ChangeHeight");
        float ContenTextHeight = ContenText.rectTransform.rect.height;
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(750, ContenTextHeight + 145);
    }
    #endregion

    #region 评论点赞功能
    private void HardBgButton(PointerEventData data)
    {
        if (!HardHadOnclick && commentlists.is_praise == 0)
        {
            //没有点过赞的
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.Setcommentpraise(1, 2, commentlists.replyid, SetcommentpraiseCallBack);
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
                    commentlists.is_praise = 1;
                    CommentHard.sprite = ResourceManager.Instance.GetUISprite("EmailForm/bg_icond_03");
                    HardNumberText.text = commentlists.bestests + 1 + "";
                }

            }, null);
        }
    }
    #endregion

    //private void OnDestroy()
    //{
    //    UIEventListener.RemoveOnClickListener(FeeBack, FeeBackButtonOn);
    //    inputtext.onEndEdit.RemoveListener(ReplyButtonOnclicke);
    //}

    public void DisPoste()
    {
        UIEventListener.RemoveOnClickListener(FeeBack, FeeBackButtonOn);
        inputtext.onEndEdit.RemoveListener(ReplyButtonOnclicke);

        CommentHard.sprite = null;
        Destroy(gameObject);

    }
}

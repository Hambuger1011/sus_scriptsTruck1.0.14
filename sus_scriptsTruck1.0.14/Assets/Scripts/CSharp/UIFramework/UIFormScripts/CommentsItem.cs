using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UGUI;
using System;
using pb;

public class CommentsItem : MonoBehaviour {

    public Text CommentName, ContenText, ReadMoreContenText;
    public Text TimeText, CommenNuber, hardNuber, ReplyText, ReplyText2, totalReplyText;
    public GameObject ReadMoreButton, ReplyBG;
    public Image CommentHard;
    public GameObject HardBG,TotalReply, ScrollView, feedBackButton;
    public InputField inputtext;

    private commentlist commentlist;
    private bool HardHadOnclick = false;
    private bool SendComment = false;
    private float height = 0f;
    public void Inite(commentlist commentlist)
    {
        ScrollView.SetActive(false);
        this.commentlist = commentlist;
        CommentName.text = commentlist.username.ToString();

        ContenText.text = "";
        height = ContenText.preferredHeight-1;
        ContenText.text = commentlist.content.ToString();

        int line = (int)(ContenText.preferredHeight / height);
        //Debug.Log("preferredHeight:" + ContenText.preferredHeight+ "--height:"+ height);

        RectTransform rectT=ContenText.gameObject.GetComponent<RectTransform>();

        //Debug.Log("line:"+ line);
        if (line<=3)
        {
            rectT.sizeDelta =new Vector2(530, line*47);
        }
        else
        {
            rectT.sizeDelta = new Vector2(530,141);
        }

        TimeText.text = commentlist.ctime.ToString();
        CommenNuber.text = commentlist.replies.ToString();
        hardNuber.text = commentlist.bestests.ToString();

        ReadMoreContenText.text= commentlist.content.ToString();
        ShowReply();

        inputtext.onEndEdit.AddListener(ReplyButtonOnclicke);
        UIEventListener.AddOnClickListener(feedBackButton, newRCRformButtonOn);
        if (commentlist.is_praise==1)
        {
            //已经点赞
            CommentHard.sprite = ResourceManager.Instance.GetUISprite("EmailForm/bg_icond_03");

        }
        else
        {
            CommentHard.sprite = ResourceManager.Instance.GetUISprite("EmailForm/bg_iconc_03");
            UIEventListener.AddOnClickListener(HardBG, HardBgButton);
        }

        if (commentlist.replies>=2)
        {
            //评论超过两条显示出更多，按钮
            TotalReply.SetActive(true);

            if (commentlist.replies >= 3)
            {
                UIEventListener.AddOnClickListener(TotalReply, MoreReplyOnButton);
                totalReplyText.text = "A total of " + commentlist.replies + " responses >>";
            }else
            {
                TotalReply.SetActive(false);

            }

        }
        else
        {
            TotalReply.SetActive(false);
        }

        if (string.IsNullOrEmpty(commentlist.username))
        {
            CommentName.text = "Gust";
        }

        Invoke("ReadMoreTrue",0.2f);
    }

    public void SendInit(string Str)
    {
        ScrollView.SetActive(false);
        ContenText.gameObject.SetActive(false);
        ReplyBG.SetActive(false);
        ReadMoreButton.SetActive(false);
        SendComment = true;
        CommentName.text = UserDataManager.Instance.userInfo.data.userinfo.nickname;
        ReadMoreContenText.text = Str.ToString();

        int month = DateTime.Now.Month;
        int day = DateTime.Now.Day;
        int hour = DateTime.Now.Hour;
        int minute = DateTime.Now.Minute;
        string time = string.Format("{0:D2}-{1:D2}" + " " + "{2:D2}:{3:D2}", month, day, hour, minute);

        TimeText.text = time;

        CommentHard.sprite = ResourceManager.Instance.GetUISprite("EmailForm/bg_iconc_03");
        ShowReply();
    }

    /// <summary>
    /// 显示评论回复
    /// </summary>
    private void ShowReply()
    {
        ScrollView.SetActive(true);

        if (SendComment)
        {
            //SendComment = false;
            return;
        }
        if (commentlist.replay==null)
        {
            ReplyBG.SetActive(false);
            //LOG.Info("没有评论回复");
            return;
        }     
        string name = commentlist.replay.username.ToString() + ": ";

        List<content> con = commentlist.replay.content;

        if (con.Count>=2)
        {
            ReplyText2.gameObject.SetActive(true);

            string ST = "<color=#28A3EAFF>" + name + "</color>" + con[0].detail.ToString();
            ReplyText.text = ST;

            string ST2 = "<color=#28A3EAFF>" + name + "</color>" + con[1].detail.ToString();
            ReplyText2.text = ST2;
        }
        else if (con.Count <= 1)
        {
            ReplyText2.gameObject.SetActive(false);
            string ST = "<color=#28A3EAFF>" + name + "</color>" + con[0].detail.ToString();
            ReplyText.text = ST;
        }
       
    }
    private void ReadMoreTrue()
    {
        CancelInvoke("ReadMoreTrue");
        ReadMoreContenText.gameObject.SetActive(false);
        float height = ReadMoreContenText.GetComponent<RectTransform>().rect.height;

        if (height>140)
        {
            //这里文本的数量超过了四行了，需要显示ReadMore按钮
            ReadMoreButton.SetActive(true);
            UIEventListener.AddOnClickListener(ReadMoreButton, ReadMoreButtonOnclick);
        }
        else
        {
            ReadMoreButton.SetActive(false);
        }
    }

    private void ReadMoreButtonOnclick(PointerEventData data)
    {
        //LOG.Info("点开查看更多内容按钮");
        ReadMoreButton.SetActive(false);
        ReadMoreContenText.gameObject.SetActive(true);
        ContenText.gameObject.SetActive(false);
        ScrollView.SetActive(false);
        Invoke("ScrollViewToTrue",0.3f);
    }
    private void ScrollViewToTrue()
    {
        CancelInvoke("ScrollViewToTrue");
        ScrollView.SetActive(true);
    }

    #region 评论按钮的评论功能
    private void ReplyButtonOnclicke(string vSt)
    {
        //新闻页举报
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.Setcomment(2, 1, commentlist.commentid, inputtext.text,0, SetcommentCallBack);

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
                    inputtext.text = "";

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(154);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Reply success!", false);
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
            GameHttpNet.Instance.Setcommentpraise(1,1, commentlist.commentid, SetcommentpraiseCallBack);
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
                    hardNuber.text = commentlist.bestests+1+"";
                }

            }, null);
        }
    }
    #endregion

    #region 查看更多评论

    private void MoreReplyOnButton(PointerEventData data)
    {
        LOG.Info("点击了查看更多评论");
        CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop).UIOpent(8);
        CUIManager.Instance.OpenForm(UIFormName.ReplyForDetalls);
        CUIManager.Instance.GetForm<ReplyForDetallsForm>(UIFormName.ReplyForDetalls).Init(commentlist);
    }
    #endregion

    #region 打开举报界面

    private void newRCRformButtonOn(PointerEventData data)
    {
        CUIManager.Instance.OpenForm(UIFormName.newRCRform);
        CUIManager.Instance.GetForm<NewRCRform>(UIFormName.newRCRform).NewReplyInit(commentlist);
    }
    #endregion
  
    /// <summary>
    /// 移除物体释放资源
    /// </summary>
    public void Disposte()
    {
        inputtext.onEndEdit.RemoveListener(ReplyButtonOnclicke);
        UIEventListener.RemoveOnClickListener(HardBG, HardBgButton);
        UIEventListener.RemoveOnClickListener(TotalReply, MoreReplyOnButton);
        UIEventListener.RemoveOnClickListener(feedBackButton, newRCRformButtonOn);

        CommentHard.sprite = null;

        Destroy(gameObject);
    }
}

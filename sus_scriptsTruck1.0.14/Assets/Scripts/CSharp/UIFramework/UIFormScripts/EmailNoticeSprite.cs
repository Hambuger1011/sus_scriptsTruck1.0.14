using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UGUI;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System.IO;
using System;
using pb;

public class EmailNoticeSprite : BaseUIForm
{

    public RectTransform bg, gifsNoticebg;
    public RectTransform mash;
    public GameObject GiftsNotice, newNotice;
    public Text GiftTime, GiftCont, tiltext, ClaimBtnText, newClaimButtonText;
    public GameObject GameClaimButton;
    public ScrollRect ScrollRectContent, NoticeScrollRect, newScrollRect;
    public Image ClaimImage, GitImage, newImage;
    public EmailGiftsItem[] EmailGiftsItem;
    public GameObject SendBg;
    public InputField InputField;
    public Image sendButton;
    public Text sendText;

    private string pathNotice;
    private int type = 0;//  1 是邮件 2是新闻
    private string url = "";
    private string path;
    private Image NowUserImage;
    private int price_status, msgid, newprice_status;
    private EmailItemSprite emailItemSprite;
    private ReadEmaillList emailItemInfo;
    private const string ClaimImageOn = "EmailForm/btn_focus";
    private const string ClaimImageOff = "EmailForm/btn_focus2";
    private string NoticeNewName = "email1.png";

    public Text newTitle, newTime, newConten;
    public Image love, unlove;
    public GameObject greatButton, OhButton, newClaimButton, newClaimButtonParent, WriteButton, InputFieldBG;
    public GameObject InputMask;
    private string InputString = "";
    private NewnewsItemSprite newItemSprite;
    private NewInfoList NewInfoList;
    private bool isnocanLove = false;
    private const string loveON = "EmailForm/bg_icond_03";
    private const string loveOff = "EmailForm/bg_iconc_03";
    private const string unloveON = "EmailForm/bg_iconc_05";
    private const string unloveOff = "EmailForm/bg_icond_05";
    private const string sendOn = "EmailForm/bg_jsnale_03";
    private const string sendOff = "EmailForm/bg_manek_03";

    private GameObject CloseButton;

    public override void OnOpen()
    {
        base.OnOpen();

        CloseButton = transform.Find("Canvas/GameObject/BG/CloseButton").gameObject;

        UIEventListener.AddOnClickListener(CloseButton, CloseFrom);
        UIEventListener.AddOnClickListener(mash.gameObject, GameObjectButton);
        UIEventListener.AddOnClickListener(ClaimImage.gameObject, ClaimButtonOnclicke);
        UIEventListener.AddOnClickListener(greatButton, GreadButtonOnclicke);
        UIEventListener.AddOnClickListener(OhButton, OhButtonOnclicke);
        UIEventListener.AddOnClickListener(newClaimButton, newClaimButtonOnclicke);
        UIEventListener.AddOnClickListener(GitImage.gameObject, NowImageBotton);
        UIEventListener.AddOnClickListener(sendButton.gameObject, SendButtonOnClicke);
        UIEventListener.AddOnClickListener(NewCommentSendButton, NewCommentSendButtonOnclick);

        UIEventListener.AddOnClickListener(WriteButton,WriteButtonOnclicke);
        UIEventListener.AddOnClickListener(InputMask, InputMaskOnclicke);

        addMessageListener(EventEnum.CloseEamailNotice, CloseEamailNotice);
        addMessageListener(EventEnum.NoticeClose, NoticeClose);


        //bg.localScale = new Vector3(0,0,0);
        bg.anchoredPosition = new Vector2(0, 1334);
        BGmove(0, 1);

        EventDispatcher.Dispatch(EventEnum.EmailAwartShowClose);
        InputField.onValueChanged.AddListener(InputChangeHandler);

        if (ResolutionAdapter.HasUnSafeArea)
        {
            var scale = this.myForm.GetScale();
            var safeArea = ResolutionAdapter.GetSafeArea();
            var offset = this.myForm.Pixel2View(safeArea.position);
            var offerH = offset.y;

            bg.offsetMin = new Vector2(0, 0);
            bg.offsetMax = new Vector2(0, -(0 + offerH));
        }

       

    }

    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(CloseButton, CloseFrom);
        UIEventListener.RemoveOnClickListener(mash.gameObject, GameObjectButton);
        UIEventListener.RemoveOnClickListener(ClaimImage.gameObject, ClaimButtonOnclicke);
        UIEventListener.RemoveOnClickListener(greatButton, GreadButtonOnclicke);
        UIEventListener.RemoveOnClickListener(OhButton, OhButtonOnclicke);
        UIEventListener.RemoveOnClickListener(newClaimButton, newClaimButtonOnclicke);
        UIEventListener.RemoveOnClickListener(GitImage.gameObject, NowImageBotton);
        UIEventListener.RemoveOnClickListener(sendButton.gameObject, SendButtonOnClicke);
        UIEventListener.RemoveOnClickListener(NewCommentSendButton, NewCommentSendButtonOnclick);

        UIEventListener.RemoveOnClickListener(WriteButton, WriteButtonOnclicke);
        UIEventListener.RemoveOnClickListener(InputMask, InputMaskOnclicke);


        InputField.onValueChanged.RemoveListener(InputChangeHandler);

        if (CommentsItemList!=null)
        {
            for (int i=0;i< CommentsItemList.Count;i++)
            {
                CommentsItemList[i].Disposte();
            }

            CommentsItemList = null;
        }
    }

    /// <summary>
    /// 关闭界面
    /// </summary>
    /// <param name="data"></param>
    private void CloseFrom(PointerEventData data)
    {
        CUIManager.Instance.CloseForm(UIFormName.EmailNotice);
    }

    private void InputMaskOnclicke(PointerEventData data)
    {
        InputFieldBG.SetActive(false);
        WriteButton.SetActive(true);
    }

    /// <summary>
    /// 这个是新闻详情界面，右下叫的编辑按钮响应事件
    /// </summary>
    /// <param name="data"></param>
    private void WriteButtonOnclicke(PointerEventData data)
    {
        WriteButton.SetActive(false);
        InputFieldBG.SetActive(true);
    }
    public void NoticeClose(Notification notification)
    {
        LOG.Info("进入书本时详细新闻和邮件信息界面时，关闭");
        CUIManager.Instance.CloseForm(UIFormName.EmailNotice);

    }

    private void InputChangeHandler(string vStr)
    {
        InputString = InputField.text;
        if (InputString.Length >= 10)
        {
            sendButton.sprite = ResourceManager.Instance.GetUISprite(sendOn);
            sendText.text = "SEND";
        }
        else if (InputString.Length < 10)
        {
            sendButton.sprite = ResourceManager.Instance.GetUISprite(sendOff);
            sendText.text = "REPLY";
        }
    }
    private void SendButtonOnClicke(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        InputString = InputField.text;

        if (InputString.Length < 10)
        {
            AudioManager.Instance.PlayTones(AudioTones.LoseFail);

            var Localization = GameDataMgr.Instance.table.GetLocalizationById(163);
            UITipsMgr.Instance.PopupTips(Localization, false);

            //UITipsMgr.Instance.PopupTips("Send more than 4 characters!", false);
            return;
        }

        LOG.Info("你发送的内容是：" + InputString);

        // SendEmail(UnityPath, InputString);
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.UserFeedback(3,"",InputString, UserFeedbackCallBack);
    }
    private void UserFeedbackCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----UserFeedbackCallBack---->" + result);
        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();
            JsonObject jo = JsonHelper.JsonToJObject(result);

            if (jo != null)
            {
                if (jo.code == 200)
                {
                    InputField.text = "";

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(164);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("success", false);
                    AudioManager.Instance.PlayTones(AudioTones.RewardWin);

                 
                }
                else
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(165);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Error, send again!", false);
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                }
            }
            else
            {
                //UINetLoadingMgr.Instance.Close();

                var Localization = GameDataMgr.Instance.table.GetLocalizationById(165);
                UITipsMgr.Instance.PopupTips(Localization, false);

                //UITipsMgr.Instance.PopupTips("Error, send again!", false);
                AudioManager.Instance.PlayTones(AudioTones.LoseFail);
            }
        }, null);
    }
    public void GameObjectButton(PointerEventData data)
    {
        
    }

    private void CloseEamailNotice(Notification notification)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        BGmove(1334, 2);

        if (emailItemSprite != null)
        {
            emailItemSprite.gethadImageChange(newprice_status);
        }

    }
    /// <summary>
    /// 这个是界面出现的时候显示的动画效果
    /// </summary>
    /// <param name="Yp"></param>
    /// <param name="type"></param>

    public void BGmove(float Yp, int type)
    {
        if (type == 1)
        {
            //这个是打开界面执行的事件
            bg.DOAnchorPosY(Yp, 1f).SetEase(Ease.InOutBack);
        }
        else
        {
            //这个是关闭界面执行的事件
            CUIManager.Instance.CloseForm(UIFormName.EmailNotice);
        }
    }

    #region 打开新闻详细信息实例化评论物体

    private int page = 0;
    public GameObject CommentsItem, CommentItemPos;

    private List<CommentsItem> CommentsItemList;
    /// <summary>
    /// 获取新闻评论的初始
    /// </summary>
    private void NewCommentInit()
    {
        CommentsItemList = new List<CommentsItem>();
        CommentsItemList.Clear();

        CommentsItem.SetActive(false);
        if (this.NewInfoList == null)
        {
            return;
        }
        page = 1;//默认页数是1
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.Getcomment(1, NewInfoList.newid, 0,page, GetcommentCallBacke);
    }

    public void ShowNextPage()
    {
        //显示下一页的评论

        if (UserDataManager.Instance.Getcomment == null)
        {
            return;
        }
        page++;
        if (page > UserDataManager.Instance.Getcomment.data.pages_total)
        {
            LOG.Info("没有更多的内容了");
            return;
        }
        LOG.Info("新闻滑动到底部了，加载下一页");
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.Getcomment(1, NewInfoList.newid, 0,page, GetcommentCallBacke);
    }
    private void GetcommentCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetcommentCallBacke---->" + result);
        if (result.Equals("error"))
        {
            LOG.Info("---GetcommentCallBacke--这条协议错误");
            return;
        }
        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();
            JsonObject jo = JsonHelper.JsonToJObject(result);

            if (jo.code == 200)
            {
                UserDataManager.Instance.Getcomment = JsonHelper.JsonToObject<HttpInfoReturn<Getcomment>>(result);

                if (UserDataManager.Instance.Getcomment == null)
                {
                    return;
                }

                List<commentlist> tem = UserDataManager.Instance.Getcomment.data.commentlist;
                if (tem.Count <= 0)
                {
                    LOG.Info("这条新闻没有评论");
                    return;
                }

                for (int i = 0; i < tem.Count; i++)
                {
                    GameObject go = Instantiate(CommentsItem);
                    go.transform.SetParent(CommentItemPos.transform);
                    go.SetActive(true);
                    
                    CommentsItem com = go.GetComponent<CommentsItem>();
                    com.Inite(tem[i]);
                    CommentsItemList.Add(com);
                   
                }

               
                //newScrollRect.content.gameObject.SetActive(false);
                Invoke("newScrollRectToTrue", 0.5f);

            }
        }, null);
    }



    #endregion

    #region 对新闻进行评价
    public InputField NewCommentInputField;
    public GameObject NewCommentSendButton;
    private string NewSendConter;
    private void NewCommentSendButtonOnclick(PointerEventData data)
    {
        NewSendConter = NewCommentInputField.text;
        if (string.IsNullOrEmpty(NewSendConter))
        {
            AudioManager.Instance.PlayTones(AudioTones.LoseFail);

            var Localization = GameDataMgr.Instance.table.GetLocalizationById(166);
            UITipsMgr.Instance.PopupTips(Localization, false);

            //UITipsMgr.Instance.PopupTips("Place enter your Comment", false);
            return;
        }
        if (NewSendConter.Length < 4)
        {
            AudioManager.Instance.PlayTones(AudioTones.LoseFail);

            var Localization = GameDataMgr.Instance.table.GetLocalizationById(167);
            UITipsMgr.Instance.PopupTips(Localization, false);

            //UITipsMgr.Instance.PopupTips("Write more than 4 characters", false);
            return;
        }
        if (NewSendConter.Length > 1000)
        {
            AudioManager.Instance.PlayTones(AudioTones.LoseFail);

            var Localization = GameDataMgr.Instance.table.GetLocalizationById(168);
            UITipsMgr.Instance.PopupTips(Localization, false);

            //UITipsMgr.Instance.PopupTips("Write Less than 1000 characters", false);
            return;
        }

        NewSendConter = UserDataManager.Instance.CheckBannedWords(NewSendConter);
        //LOG.Info("====NewCommentSendButtonOnclick===>" + result + "---old--->" + NewSendConter);
        
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.Setcomment(1, 1, NewInfoList.newid, NewSendConter, 0, SetcommentCallBacke);
    }

    private void SetcommentCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----SetcommentCallBacke---->" + result);
        if (result.Equals("error"))
        {
            LOG.Info("---SetcommentCallBacke--这条协议错误");
            return;
        }
        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();
            JsonObject jo = JsonHelper.JsonToJObject(result);

            if (jo.code == 200)
            {
                GameObject go = Instantiate(CommentsItem);
                go.transform.SetParent(CommentItemPos.transform);
                go.SetActive(true);
                go.transform.SetSiblingIndex(0);
                //go.GetComponent<CommentsItem>().SendInit(NewSendConter);

                CommentsItem com = go.GetComponent<CommentsItem>();
                com.SendInit(NewSendConter);
                CommentsItemList.Add(com);

                NewSendConter = "";
                NewCommentInputField.text = "";

                nume = 0;
                CommentItemPos.SetActive(false);
                Invoke("CommentItemPosInitv",0.3f);

                WriteButton.SetActive(true);
                InputFieldBG.SetActive(false);
            }
        }, null);
    }
    private int nume = 0;
    private void CommentItemPosInitv()
    {
        nume++;
        CommentItemPos.SetActive(false);
        CancelInvoke("CommentItemPosInitv");
        CommentItemPos.SetActive(true);

        if (nume > 3)
        {
            return;
        }
        Invoke("CommentItemPosInitv", 0.2f);
    }
    #endregion
    /// <summary>
    /// 这里是新闻信息的初始化
    /// </summary>
    /// <param name="newItem"></param>
    /// <param name="newInfoList"></param>
    public void NewInit(NewnewsItemSprite newItem, NewInfoList newInfoList)
    {
        type = 2;

        InputFieldBG.SetActive(false);
        GiftsNotice.SetActive(false);
        newNotice.SetActive(true);

        this.newItemSprite = newItem;
        this.NewInfoList = newInfoList;

        newTitle.text = newInfoList.title.ToString();
        newTime.text = newInfoList.createtime.ToString();
        newConten.text = newInfoList.content.ToString();

        showLoveImage(newInfoList.is_bestests);
        NewCommentInit();

        if (NewInfoList.story_type == 1)
        {
            newClaimButtonText.text = "Go!";
            //LOG.Info("新闻链接按钮点击,应该跳转到故事界面,跳转的书本的值是：");

        }
        else if (NewInfoList.story_type == 2)
        {
            newClaimButtonText.text = "Claim!";
            //LOG.Info("新闻链接按钮点击，应该跳转到钻石界面，值是：");         
        }
        else
        {
            newClaimButtonText.text = "Go!";
            //LOG.Info("新闻链接按钮点击，应该打开网页，地址是：");         
        }

        if (string.IsNullOrEmpty(NewInfoList.story_value) && string.IsNullOrEmpty(NewInfoList.sign_url))
        {
            newClaimButton.gameObject.SetActive(false);
            newClaimButtonParent.SetActive(false);
        }
        //获得图片的下载地址
        // string sss = newInfoList.thumb_details;
        if (string.IsNullOrEmpty(newInfoList.thumb_details))
        {
            //url = "http://148.153.55.2:30996/static/assets/img/emailpic/email3.png";
            newImage.gameObject.SetActive(false);
        }
        else
        {

            url = newInfoList.thumb_details;
            newImage.gameObject.SetActive(false);

            string[] ImageMane = url.Split('/');
            //LOG.Info("最后的字符串：" + ImageMane[ImageMane.Length - 1] + "--路径：" + url);
            string fileName = ImageMane[ImageMane.Length - 1];
            NoticeNewName = fileName;
        }

        // LOG.Info("新闻详细的图片：" + url);
        ChaperImageDown();//检查图片是否已经下载好了

        //newScrollRect.content.gameObject.SetActive(false);
        //Invoke("newScrollRectToTrue", 0.5f);

        if (!string.IsNullOrEmpty(NewInfoList.button_name))
        {
            newClaimButtonText.text = NewInfoList.button_name.ToString();
        }
    }

    private void newScrollRectToTrue()
    {
        CancelInvoke("newScrollRectToTrue");

        //Debug.Log("ddd");
        //newScrollRect.content.gameObject.SetActive(true);
        for (int i = 0; i < 4; i++)
        {
            newScrollRect.gameObject.SetActive(false);
            newScrollRect.gameObject.SetActive(true);
        }
    }
    private void ChaperImageDown()
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("pathNotice")))
        {
            //开始下载图片
            StartCoroutine(UploadPNG(NoticeNewName, "emailpic"));
        }
        else
        {
            if (Directory.Exists(PlayerPrefs.GetString("pathNotice")))
            {
                string[] ImageMane = url.Split('/');
                // LOG.Info("最后的字符串：" + ImageMane[ImageMane.Length - 1] + "--路径：" + url);
                string fileName = ImageMane[ImageMane.Length - 1];


                DirectoryInfo direction = new DirectoryInfo(PlayerPrefs.GetString("pathNotice"));
                FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

                // LOG.Info("文件夹里有：" + files.Length + "--张图片");

                if (files.Length == 0)
                {
                    //LOG.Info("文件夹里没有图片的时候下载");
                    StartCoroutine(UploadPNG(NoticeNewName, "emailpic"));
                    return;
                }
                //当图片的张数多的时候清除             
                for (int i = 0; i < files.Length; i++)
                {
                    int shu = 0;

                    if (files[i].Name.EndsWith(".meta"))
                    {
                        continue;
                    }
                    // LOG.Info("图片的名字是:" + files[i].Name);  //打印出来这个文件架下的所有文件
                    if (fileName.Equals(files[i].Name))
                    {
                        //  LOG.Info("图片已经下好了，直接取来用");
                        shu++;
                        if (type == 1)
                        {
                            //邮件
                            GitImage.gameObject.SetActive(true);
                        }
                        else
                        {
                            //新闻
                            newImage.gameObject.SetActive(true);
                        }
                        path = PlayerPrefs.GetString("pathNotice") + "/" + NoticeNewName;
                        // LOG.Info("Path:" + path);
                        ShowNativeTexture();
                        return;
                    }
                    else if (i == files.Length - 1 && shu == 0)
                    {
                        // LOG.Info("开始下载图片了AAAA");
                        //开始下载图片
                        StartCoroutine(UploadPNG(NoticeNewName, "emailpic"));
                    }
                }

            }
            else
            {
                //开始下载图片
                StartCoroutine(UploadPNG(NoticeNewName, "emailpic"));
            }
        }
    }

    /// <summary>
    /// 显示爱心的状态
    /// </summary>
    public void showLoveImage(int type)
    {
        if (type == 0)
        {
            //还未做出评论
            love.sprite = ResourceManager.Instance.GetUISprite(loveOff);
            unlove.sprite = ResourceManager.Instance.GetUISprite(unloveOff);
        }
        else if (type == 1)
        {
            //点了赞
            love.sprite = ResourceManager.Instance.GetUISprite(loveON);
            unlove.sprite = ResourceManager.Instance.GetUISprite(unloveOff);

        }
        else if (type == 2)
        {
            //被踩了
            love.sprite = ResourceManager.Instance.GetUISprite(loveOff);
            unlove.sprite = ResourceManager.Instance.GetUISprite(unloveON);

        }
    }

    private void GreadButtonOnclicke(PointerEventData data)
    {

        this.newItemSprite.GreatButtonOnclicke(null);

        if (!isnocanLove && NewInfoList.is_bestests == 0)
        {
            // LOG.Info("新闻详细信息点赞");
            isnocanLove = true;
            showLoveImage(1);
        }

    }
    private void OhButtonOnclicke(PointerEventData data)
    {

        this.newItemSprite.OhButtonOnclicke(null);

        if (!isnocanLove && NewInfoList.is_bestests == 0)
        {
            //LOG.Info("新闻详细信息被踩了");

            isnocanLove = true;
            showLoveImage(2);
        }
    }

    private void newClaimButtonOnclicke(PointerEventData data)
    {
        LOG.Info("story_type:" + NewInfoList.story_type);
        if (!string.IsNullOrEmpty(NewInfoList.sign_url))
        {
            //签名跳转不为空，跳转应的网址
            Application.OpenURL(NewInfoList.sign_url);
        }

        if (NewInfoList.story_type == 1)
        {
            if (string.IsNullOrEmpty(NewInfoList.story_value))
            {
                // LOG.Info("没有指定跳转的书本");
                return;
            }
            int bookID = int.Parse(NewInfoList.story_value);
            LOG.Info("新闻链接按钮点击,应该跳转到故事界面,跳转的书本的值是：" + bookID);

            if (bookID == 0)
            {
                LOG.Info("不存在这本书");
                return;
            }
            //GameDataMgr.Instance.table.ChangeBookDialogPath(bookID);
            UserDataManager.Instance.UserData.CurSelectBookID = bookID;

            AudioManager.Instance.PlayTones(AudioTones.book_click);
            CUIManager.Instance.OpenForm(UIFormName.BookDisplayForm);
            var view = CUIManager.Instance.GetForm<BookDisplayForm>(UIFormName.BookDisplayForm);
            view.InitByBookID(UserDataManager.Instance.UserData.CurSelectBookID);

          

        }
        else if (NewInfoList.story_type == 2)
        {
            if (string.IsNullOrEmpty(NewInfoList.story_value))
            {
                LOG.Info("没有指定");
                return;
            }
            int value = int.Parse(NewInfoList.story_value);
            LOG.Info("新闻链接按钮点击，应该跳转到钻石界面，值是：" + value);

            MainTopSprite TopSprite = CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop);
            if (TopSprite != null)
            {
                TopSprite.OpenChargeMoney_Diamonds(null);
            }
        }
        else
        {
            if (string.IsNullOrEmpty(NewInfoList.story_value))
            {
                LOG.Info("没有指定");
                return;
            }

            string value = NewInfoList.story_value;

            LOG.Info("value:" + value);
            // LOG.Info("新闻链接按钮点击，应该打开网页，地址是："+value);
            Application.OpenURL(value);
        }
    }

    private string[] ImageMane;
    public void Inite(EmailItemSprite tem, ReadEmaillList emailItemInfo)
    {
        type = 1;
        GiftsNotice.SetActive(true);
        newNotice.SetActive(false);

        this.emailItemInfo = emailItemInfo;
        emailItemSprite = tem;
        this.price_status = emailItemInfo.price_status;
        newprice_status = emailItemInfo.price_status;
        this.msgid = emailItemInfo.msgid;
        tiltext.text = emailItemInfo.title.ToString();

        Invoke("GiftNoticeBgHeightChange", 0.2f);

        //带有图片的
        if (string.IsNullOrEmpty(emailItemInfo.email_pic)/*emailItemInfo.email_pic.Equals("")*/)
        {
            // LOG.Info("email_pic为空");
            GitImage.gameObject.SetActive(false);
        }
        else
        {
            GitImage.gameObject.SetActive(false);
            // LOG.Info("email_pic值是：" + emailItemInfo.email_pic);            
            //获得图片的下载地址
            url = GameHttpNet.Instance.Email_picUrl(emailItemInfo.email_pic);

            string[] ImageMane = url.Split('/');
            // LOG.Info("最后的字符串：" + ImageMane[ImageMane.Length - 1] + "--路径：" + url);
            string fileName = ImageMane[ImageMane.Length - 1];
            NoticeNewName = fileName;


            //开始下载图片
            //StartCoroutine(UploadPNG(NoticeNewName, "emailpic"));
            ChaperImageDown();
        }

        //带有奖励的
        if (emailItemInfo.isprice == 1)
        {
            ScrollRectContent.gameObject.SetActive(true);
            GameClaimButton.SetActive(true);
            GiftTime.text = emailItemInfo.createtime.ToString();
            GiftCont.text = emailItemInfo.content.ToString();
            if (emailItemInfo.msg_type == 4)
                ClaimBtnText.text = "GO";
            else
                ClaimBtnText.text = "COLLECT";

            if (emailItemInfo.price_bkey != 0)
            {
                //实例化出钥匙奖励物体             
                EmailGiftsItem[0].Init(1, emailItemInfo.price_bkey);
                EmailGiftsItem[0].gameObject.SetActive(true);
            }

            if (emailItemInfo.price_diamond != 0)
            {
                //实例化出钻石奖励物体             
                EmailGiftsItem[1].Init(2, emailItemInfo.price_diamond);
                EmailGiftsItem[1].gameObject.SetActive(true);
            }

            if (emailItemInfo.price_ticket != 0)
            {
                //实例化出钻石奖励物体              
                EmailGiftsItem[2].Init(3, emailItemInfo.price_ticket);
                EmailGiftsItem[2].gameObject.SetActive(true);
            }

            //LOG.Info("price_status:" + emailItemInfo.price_status);
            if (emailItemInfo.price_status == 0)
            {
                //奖励没有领取
                ClaimImage.sprite = ResourceManager.Instance.GetUISprite(ClaimImageOn);
            }
            else
            {
                //奖励已经领取
                ClaimImage.sprite = ResourceManager.Instance.GetUISprite(ClaimImageOff);
            }
        }
        else
        {
            ScrollRectContent.gameObject.SetActive(false);
            GameClaimButton.SetActive(false);
            GiftTime.text = emailItemInfo.createtime.ToString();
            GiftCont.text = emailItemInfo.content.ToString();
        }

        if (!string.IsNullOrEmpty(emailItemInfo.button_name))
        {
            ClaimBtnText.text = emailItemInfo.button_name.ToString();
        }

        if (!string.IsNullOrEmpty(emailItemInfo.sign_url))
        {
            //如果有跳转信息，跳转按钮必须显示出来
            GameClaimButton.SetActive(true);
        }

        if (emailItemInfo.msg_type == 5)
        {
            //这个是邮件回复类型

            SendBg.SetActive(true);
            GameClaimButton.SetActive(false);
            ScrollRectContent.gameObject.SetActive(false);
        }

        Invoke("ContenChange", 0.2f);
    }

    /// <summary>
    /// 这个，文本随着内容的大小而自动适应
    /// </summary>
    private void GiftNoticeBgHeightChange()
    {

        CancelInvoke("GiftNoticeBgHeightChange");
        float GitImageH = 0;
        if (GitImage.gameObject.activeSelf)
        {
            GitImageH = GitImage.rectTransform.rect.height;
        }
        float vTextH = GiftCont.rectTransform.rect.height;
        float TextH = vTextH + GitImageH;
        float bgWith = gifsNoticebg.rect.width;

        RectTransform NoticeScrollRectRT = NoticeScrollRect.GetComponent<RectTransform>();

        // LOG.Info("文本的高度是" + TextH+"--屏幕宽度是："+ bgWith);

        if (emailItemInfo.msg_type == 5)
        {
            //邮件回复类型

            if (TextH < 575)
            {
                gifsNoticebg.sizeDelta = new Vector2(bgWith, TextH + 595);
                NoticeScrollRectRT.sizeDelta = new Vector2(642, TextH);
            }
            else
            {
                gifsNoticebg.sizeDelta = new Vector2(bgWith, 1100);
                NoticeScrollRectRT.sizeDelta = new Vector2(642, 575);
            }
            return;
        }

        if (emailItemInfo.isprice == 1)
        {
            //带奖励的
            //NoticeScrollRectRT.sizeDelta = new Vector2(642, 231);

            if (TextH < 639)
            {
                gifsNoticebg.sizeDelta = new Vector2(bgWith, TextH + 531);
                NoticeScrollRectRT.sizeDelta = new Vector2(642, TextH);
            }
            else
            {
                gifsNoticebg.sizeDelta = new Vector2(bgWith, 1100);
                NoticeScrollRectRT.sizeDelta = new Vector2(642, 580);
            }
            return;
        }

        if (!string.IsNullOrEmpty(emailItemInfo.sign_url))
        {
            if (TextH < 767)
            {
                gifsNoticebg.sizeDelta = new Vector2(bgWith, TextH + 403);
                NoticeScrollRectRT.sizeDelta = new Vector2(642, TextH);
            }
            else
            {
                gifsNoticebg.sizeDelta = new Vector2(bgWith, 1100);
                NoticeScrollRectRT.sizeDelta = new Vector2(642, 680);
            }
            return;

        }

        if (TextH < 863)
        {
            gifsNoticebg.sizeDelta = new Vector2(bgWith, TextH + 307);
            NoticeScrollRectRT.sizeDelta = new Vector2(642, TextH);
        }
        else
        {
            gifsNoticebg.sizeDelta = new Vector2(bgWith, 1100);
            NoticeScrollRectRT.sizeDelta = new Vector2(642, 840);
        }
    }


    private void ContenChange()
    {
        CancelInvoke("ContenChange");
        GameObject go = NoticeScrollRect.content.gameObject;
        go.SetActive(false);
        go.SetActive(true);

    }
    /// <summary>
    /// 这里是点击Claim按钮领取奖励物体
    /// </summary>
    /// <param name="data"></param>
    private void ClaimButtonOnclicke(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);

        if (!string.IsNullOrEmpty(emailItemInfo.sign_url))
        {
            //网页跳转
            Application.OpenURL(emailItemInfo.sign_url);
        }

        if (emailItemInfo != null && emailItemInfo.msg_type == 4)
        {
            LOG.Info("--------emailItemInfo.msg_type-------->" + emailItemInfo.msg_type);
            SdkMgr.Instance.OpenWebView(emailItemInfo.email_url, WebViewCallBack);
        }
        else
        {
            if (price_status == 0)
            {
                //奖励没有领取的时候点击按钮领取

                //UINetLoadingMgr.Instance.Show();
                GameHttpNet.Instance.GetEmailAward(msgid, GetEmailAwardCallBack);
            }
            else
            {
                newprice_status = 1;

                var Localization = GameDataMgr.Instance.table.GetLocalizationById(169);
                UITipsMgr.Instance.PopupTips(Localization, false);

                //UITipsMgr.Instance.PopupTips("Already received!", false);

            }
        }


    }

    private void WebViewCallBack(string vMsg)
    {
        if (!string.IsNullOrEmpty(vMsg) && vMsg.Equals("2a338a7aa119c2d5433e9f738086560612"))
        {
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.GetEmailAward(msgid, GetEmailAwardCallBack);

            SdkMgr.Instance.CloseWebView(WebViewCallBack);
        }
    }

    private void GetEmailAwardCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetEmailAwardCallBack---->" + result);
        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();
            JsonObject jo = JsonHelper.JsonToJObject(result);

            if (jo.code == 200)
            {
                UserDataManager.Instance.emailGetAwardInfo = JsonHelper.JsonToObject<HttpInfoReturn<EmailGetAwardInfo>>(result);

                ClaimImage.sprite = ResourceManager.Instance.GetUISprite(ClaimImageOff);

                UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.emailGetAwardInfo.data.bkey);
                UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.emailGetAwardInfo.data.diamond);
                UserDataManager.Instance.ResetMoney(3, UserDataManager.Instance.emailGetAwardInfo.data.ticket);

                var Localization = GameDataMgr.Instance.table.GetLocalizationById(158);
                UITipsMgr.Instance.PopupTips(Localization, false);

                //UITipsMgr.Instance.PopupTips("Received successful!", false);
                //LOG.Info("邮箱领取后：钥匙："+ UserDataManager.Instance.emailGetAwardInfo.data.bkey+"--钻石："+ UserDataManager.Instance.emailGetAwardInfo.data.diamond);
                newprice_status = 1;
            }
            else
            {
                newprice_status = 1;

                var Localization = GameDataMgr.Instance.table.GetLocalizationById(169);
                UITipsMgr.Instance.PopupTips(Localization, false);

                //UITipsMgr.Instance.PopupTips("Already received!", false);
            }

        }, null);
    }

    private void NowImageBotton(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        // LOG.Info("这个是图片链接按钮点击了");
        //string OpenURL=email
        Application.OpenURL(emailItemInfo.email_url);
    }


    /// <summary>
    /// 下载图片
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private IEnumerator UploadPNG(string fileName, string dic)
    {
        if (string.IsNullOrEmpty(url))
        {
            LOG.Info("网址是空的");
        }
        else
        {
            WWW www = new WWW(url);
            yield return www;


            if (www.isDone)
            {
                if (type == 1)
                {
                    //邮件
                    GitImage.gameObject.SetActive(true);


                }
                else
                {
                    //新闻
                    newImage.gameObject.SetActive(true);
                }

                Debug.Log("url:" + url);
                byte[] bytes = www.texture.EncodeToPNG();
                path = PathForFile(fileName, dic);//平台的判断

                // LOG.Info("下载完成，文件" + path);
                SaveNativeFile(bytes, path);//保存图片到本地        


                //显示图片
                ShowNativeTexture();
            }
        }
    }

    /// <summary>
    /// 判断平台
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public string PathForFile(string filename, string dic)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            string path = Application.persistentDataPath.Substring(0, Application.persistentDataPath.Length - 5);
            path = path.Substring(0, path.LastIndexOf('/'));
            path = Path.Combine(path, "Documents");
            path = Path.Combine(path, dic);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return Path.Combine(path, filename);
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            string path = Application.persistentDataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            path = Path.Combine(path, dic);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return Path.Combine(path, filename);
        }
        else
        {
            string path = Application.dataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            path = Path.Combine(path, dic);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            pathNotice = path;
            PlayerPrefs.SetString("pathNotice", pathNotice);
            return Path.Combine(path, filename);
        }
    }
    /// <summary>
    /// 在本地保存文件
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="path"></param>
    public void SaveNativeFile(byte[] bytes, string path)
    {
        FileStream fs = new FileStream(path, FileMode.Create);
        fs.Write(bytes, 0, bytes.Length);
        fs.Flush();
        fs.Close();
    }


    /// <summary>
    /// 显示图片
    /// </summary>
    public void ShowNativeTexture()
    {
        // LOG.Info("图片path:" + path);
        Texture2D texture = GetNativeFile(path);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        Image Pice = null;

        if (type == 1)
        {
            //邮件
            Pice = GitImage;
        }
        else
        {
            //新闻图片
            Pice = newImage;
        }

        Pice.sprite = sprite;
        Pice.SetNativeSize();

        RectTransform rec = Pice.gameObject.GetComponent<RectTransform>();

        float Pw = rec.rect.width;
        float Ph = rec.rect.height;
        float w =700 /*Screen.width*/;
        float h = Screen.height;
        // LOG.Info("屏幕宽度：" + w + "--屏幕高度：" + h + "--图片的宽度:" + Pw + "--图片的高度：" + Ph);
        Pice.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(w, w * Ph / Pw * 1.0f);

    }
    /// <summary>
    /// 获取到本地的图片
    /// </summary>
    /// <param name="path"></param>
    public Texture2D GetNativeFile(string path)
    {
        try
        {
            var pathName = path;
            var bytes = ReadFile(pathName);
            int width = Screen.width;
            int height = Screen.height;
            var texture = new Texture2D(width, height);
            texture.LoadImage(bytes);
            return texture;
        }
        catch (Exception c)
        {

        }
        return null;
    }
    public byte[] ReadFile(string filePath)
    {
        var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        fs.Seek(0, SeekOrigin.Begin);
        var binary = new byte[fs.Length];
        fs.Read(binary, 0, binary.Length);
        fs.Close();
        return binary;
    }

    private void OnDisable()
    {

        //获取指定路径下面的所有资源文件  
        if (Directory.Exists(pathNotice))
        {
            DirectoryInfo direction = new DirectoryInfo(pathNotice);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

            // LOG.Info("文件夹里有：" + files.Length + "--张图片");

            //当图片的张数多的时候清除
            if (files.Length > 10)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".meta"))
                    {
                        continue;
                    }
                    // LOG.Info("图片的名字是:" + files[i].Name);  //打印出来这个文件架下的所有文件

                    files[i].Delete();
                    //LOG.Info( "FullName:" + files[i].FullName );  
                    //LOG.Info( "DirectoryName:" + files[i].DirectoryName );  
                }
                // LOG.Info("图片大于规定数量，清除");              
            }
        }
    }
}

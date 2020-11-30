using pb;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EmailTextNoticeForm : BaseUIForm {

    public RectTransform mash, gifsNoticebg;
    public Text tiltext;
    public Text GiftTime, GiftCont, ClaimBtnText;
    public Image GitImage, ClaimImage, sendButton;
    public ScrollRect ScrollRectContent, NoticeScrollRect;
    public GameObject GameClaimButton;
    public GameObject SendBg;
    public EmailGiftsItem[] EmailGiftsItem;
    public InputField InputField;
    public Text sendText;

    private string pathNotice;
    private string path;
    private int type = 0;//  1 是邮件 2是新闻
    private ReadEmaillList emailItemInfo;
    private EmailItemSprite emailItemSprite;
    private int price_status, msgid, newprice_status;
    private string url = "";
    private string InputString = "";
    private string NoticeNewName = "email1.png";
    private const string ClaimImageOn = "EmailForm/btn_focus";
    private const string ClaimImageOff = "EmailForm/btn_focus2";
    private const string sendOn = "EmailForm/bg_jsnale_03";
    private const string sendOff = "EmailForm/bg_manek_03";


    public override void OnOpen()
    {
        base.OnOpen();

        UIEventListener.AddOnClickListener(mash.gameObject, GameObjectButton);
        UIEventListener.AddOnClickListener(ClaimImage.gameObject, ClaimButtonOnclicke);
        UIEventListener.AddOnClickListener(sendButton.gameObject, SendButtonOnClicke);
        InputField.onValueChanged.AddListener(InputChangeHandler);
    }

    public override void OnClose()
    {
        base.OnClose();

        UIEventListener.RemoveOnClickListener(mash.gameObject, GameObjectButton);
        UIEventListener.RemoveOnClickListener(ClaimImage.gameObject, ClaimButtonOnclicke);
        UIEventListener.RemoveOnClickListener(sendButton.gameObject, SendButtonOnClicke);
        InputField.onValueChanged.RemoveListener(InputChangeHandler);
    }

    public void GameObjectButton(PointerEventData data)
    {
       
        //if (CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop) != null)
        //{
        //    CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop).CloseIconButton(null);
        //}

        CUIManager.Instance.CloseForm(UIFormName.EmailTextNotice);
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


                if (emailItemSprite != null)
                {
                    emailItemSprite.gethadImageChange(newprice_status);
                }
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

    public void Inite(EmailItemSprite tem, ReadEmaillList emailItemInfo)
    {
        type = 1;
       
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
                //NoticeScrollRectRT.sizeDelta = new Vector2(642, TextH-50);
            }
            else
            {
                gifsNoticebg.sizeDelta = new Vector2(bgWith, 1100);
                //NoticeScrollRectRT.sizeDelta = new Vector2(642, 575);
            }
            return;
        }

        if (emailItemInfo.isprice == 1)
        {
            //带奖励的
            //NoticeScrollRectRT.sizeDelta = new Vector2(642, 231);

            if (TextH < 639)
            {
                gifsNoticebg.sizeDelta = new Vector2(bgWith, TextH + 571);
                //NoticeScrollRectRT.sizeDelta = new Vector2(642, TextH-50);
            }
            else
            {
                gifsNoticebg.sizeDelta = new Vector2(bgWith, 1100);
                //NoticeScrollRectRT.sizeDelta = new Vector2(642, 580);
            }
            return;
        }

        if (!string.IsNullOrEmpty(emailItemInfo.sign_url))
        {
            if (TextH < 767)
            {
                gifsNoticebg.sizeDelta = new Vector2(bgWith, TextH + 403);
                //NoticeScrollRectRT.sizeDelta = new Vector2(642, TextH-50);
            }
            else
            {
                gifsNoticebg.sizeDelta = new Vector2(bgWith, 1100);
                //NoticeScrollRectRT.sizeDelta = new Vector2(642, 680);
            }
            return;

        }

        if (TextH < 863)
        {
            gifsNoticebg.sizeDelta = new Vector2(bgWith, TextH + 307);
            //NoticeScrollRectRT.sizeDelta = new Vector2(642, TextH-50);
        }
        else
        {
            gifsNoticebg.sizeDelta = new Vector2(bgWith, 1100);
            //NoticeScrollRectRT.sizeDelta = new Vector2(642, 840);
        }
    }

    private void ContenChange()
    {
        CancelInvoke("ContenChange");
        GameObject go = NoticeScrollRect.content.gameObject;
        go.SetActive(false);
        go.SetActive(true);

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
        

        Pice.sprite = sprite;
        Pice.SetNativeSize();

        RectTransform rec = Pice.gameObject.GetComponent<RectTransform>();

        float Pw = rec.rect.width;
        float Ph = rec.rect.height;
        float w = 700 /*Screen.width*/;
        float h = Screen.height;
        // LOG.Info("屏幕宽度：" + w + "--屏幕高度：" + h + "--图片的宽度:" + Pw + "--图片的高度：" + Ph);
        Pice.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(w, w * Ph / Pw * 1.0f);

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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System;
using UGUI;
using pb;

public class NewnewsItemSprite : MonoBehaviour
{
    private Text Time;
    private RectTransform Frame;
    private Text Tilte;
    private Image Picture;
    private Text Content,DetailContent;
    private Text CommentNumber;
    private GameObject Great, Oh;
    private Image love, NoLove;
    private EmailAndNewForm EmailAndNewForm;
    private NewInfoList NewInfoList;
    private string path;
    private RectTransform SelRectTrans;
    private GameObject CommentsBg;
    private float height = 0f;
    private GameObject ReadMoreButton, Hit;
    private int LikeType = 0;
    private int line;
   
    private const string loveON = "EmailForm/bg_icond_03";
    private const string loveOff = "EmailForm/bg_iconc_03";
    private const string unloveON = "EmailForm/bg_iconc_05";
    private const string unloveOff = "EmailForm/bg_icond_05";

    public void Inite(NewInfoList NewInfoList,EmailAndNewForm EmailAndNewForm)
    {
        this.NewInfoList = NewInfoList;
        this.EmailAndNewForm = EmailAndNewForm;
        Time = transform.Find("Time").GetComponent<Text>();
        Frame = transform.Find("Mask/Frame").GetComponent<RectTransform>();
        Tilte = transform.Find("Mask/Frame/Tilte").GetComponent<Text>();
        Picture = transform.Find("Mask/Frame/Picture").GetComponent<Image>();
        Content = transform.Find("Mask/Frame/Content").GetComponent<Text>();
        DetailContent = transform.Find("Mask/Frame/DetailContent").GetComponent<Text>();
        CommentNumber = transform.Find("Mask/Frame/CommentsBg/Comment/CommentNumber").GetComponent<Text>();
        CommentsBg = transform.Find("Mask/Frame/CommentsBg").gameObject;
        Great = transform.Find("Mask/Frame/CommentsBg/Great").gameObject;
        love = transform.Find("Mask/Frame/CommentsBg/Great/love").GetComponent<Image>();
        Oh = transform.Find("Mask/Frame/CommentsBg/Oh").gameObject;
        NoLove = transform.Find("Mask/Frame/CommentsBg/Oh/NoLove").GetComponent<Image>();
        ReadMoreButton = transform.Find("Mask/Frame/ReadMoreBg/ReadMoreButton").gameObject;
        Hit = transform.Find("Mask/Frame/Tilte/Hit").gameObject;
       
        SelRectTrans = transform.GetComponent<RectTransform>();

        ShowNewInfo();



        UIEventListener.AddOnClickListener(gameObject, GameOnclicke);

        UIEventListener.AddOnClickListener(Great, GreatButtonOnclicke);
        UIEventListener.AddOnClickListener(Oh, OhButtonOnclicke);


        //事件派发注册
        EventDispatcher.AddMessageListener(EventEnum.NewItemOpen, NewItemOpen);

    }


    /// <summary>
    /// 显示新闻的简略信息
    /// </summary>
    private void ShowNewInfo()
    {
        path = "";
        Content.text = "";
        height = Content.preferredHeight;

        Tilte.text = NewInfoList.title.ToString();
        Content.text = NewInfoList.content.ToString();
        DetailContent.text = NewInfoList.content.ToString();
        DetailContent.gameObject.SetActive(false);
        CommentNumber.text = NewInfoList.com_total.ToString();

        if (NewInfoList.send_type==1)
        {
            //新闻类
            CommentsBg.SetActive(true);
        }
        else
        {
            //公告类
            CommentsBg.SetActive(false);
        }

        if (NewInfoList.is_read==1)
        {
            //已经读取了
            Hit.SetActive(false);
        }else
        {
            //没有读取了
            Hit.SetActive(true);
        }

        if (NewInfoList.thumb ==null|| string.IsNullOrEmpty(NewInfoList.thumb))
        {
            Picture.gameObject.SetActive(false);
        }else
        {
            Picture.gameObject.SetActive(true);
            LoadImage(NewInfoList.thumb);
        }

        RectTransform ContentRect = Content.GetComponent<RectTransform>();

        line = GetLineCount();//计算出这个text文本所占的行数

        if (line>3)
        {
            ContentRect.sizeDelta = new Vector2(642,117);
            ReadMoreButton.SetActive(true);
           
        }
        else
        {
            ContentRect.sizeDelta = new Vector2(642, 39* line);
            ReadMoreButton.SetActive(false);
          
        }

        showLoveImage(NewInfoList.is_bestests);
        //Debug.Log("总共的行数为："+line);

        Invoke("InvokeHeight", 0.3f);
    }

    /// <summary>
    /// 这个是计算文本的行数
    /// </summary>
    /// <returns></returns>
    private int GetLineCount()
    {
        return (int)(Content.preferredHeight / height);
    }
    private void InvokeHeight()
    {
        float FrameH = Frame.rect.height;
        SelRectTrans.sizeDelta = new Vector2(727,FrameH);
       
    }

    /// <summary>
    /// 打开新闻的详细信息
    /// </summary>
    /// <param name="data"></param>
    private void GameOnclicke(PointerEventData data)
    {
        LOG.Info("新闻被点击了");

        EventDispatcher.Dispatch(EventEnum.NewItemOpen, NewInfoList.newid);
    }

    /// <summary>
    /// 新闻物体展开
    /// </summary>
    /// <param name="notification"></param>
    public void NewItemOpen(Notification notification)
    {
        if ((int)notification.Data== NewInfoList.newid)
        {
            //展开
            if (NewInfoList.send_type == 2)
            {
                //公告
                if (NewInfoList.is_read == 1)
                {
                    //这个是已经读取的
                    AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
                    NewTypyNotice();
                }
                else
                {
                    AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
                    //UINetLoadingMgr.Instance.Show();
                    GameHttpNet.Instance.NewReading(NewInfoList.newid, NewReadingCallBacke);
                }              
            }
            else
            {
                //可以点赞的新闻

                if (NewInfoList.is_read == 1)
                {
                    //这个是已经读取的
                    AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
                    OpenNewNotice();
                }                
                else
                {
                    AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
                    //UINetLoadingMgr.Instance.Show();
                    GameHttpNet.Instance.NewReading(NewInfoList.newid, NewReadingCallBacke);
                }
            }
        }
        else
        {
            //收起
            NewItemPutAway();
        }
    }

    private void NewTypyNotice()
    {
        if (NewInfoList.thumb_details == null || string.IsNullOrEmpty(NewInfoList.thumb_details))
        {
            Picture.gameObject.SetActive(false);
        }
        else
        {
            Picture.sprite = null;
            Picture.gameObject.SetActive(true);
            LoadImage(NewInfoList.thumb_details);
        }

        Content.gameObject.SetActive(false);
        DetailContent.gameObject.SetActive(true);
        NewInfoList.is_read = 1;
        Hit.SetActive(false);
        ReadMoreButton.SetActive(false);
     
        Invoke("InvokeHeight", 0.3f);
    }

    /// <summary>
    /// 新闻物体收起
    /// </summary>
    private void NewItemPutAway()
    {
        if (NewInfoList.thumb == null || string.IsNullOrEmpty(NewInfoList.thumb))
        {
            Picture.gameObject.SetActive(false);
        }
        else
        {
            Picture.sprite = null;
            Picture.gameObject.SetActive(true);
            LoadImage(NewInfoList.thumb);
        }

        if (line > 3)
        {
            ReadMoreButton.SetActive(true);
          
        }
        else
        {          
            ReadMoreButton.SetActive(false);
           
        }

        Content.gameObject.SetActive(true);
        DetailContent.gameObject.SetActive(false);
        Invoke("InvokeHeight", 0.3f);
    }

    private void OpenNewNotice()
    {

        CUIManager.Instance.OpenForm(UIFormName.EmailNotice);
        //newItemSprite tem = this.gameObject.GetComponent<newItemSprite>();
        CUIManager.Instance.GetForm<EmailNoticeSprite>(UIFormName.EmailNotice).NewInit(this, this.NewInfoList);
    }
    public void NewReadingCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----NewReadingCallBacke---->" + result);

        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();
            JsonObject jo = JsonHelper.JsonToJObject(result);

            if (jo.code == 200)
            {
                NewInfoList.is_read = 1;
                Hit.SetActive(false);             
                EmailAndNewForm.newNumberLower();

                if (NewInfoList.send_type == 2)
                {
                    NewTypyNotice();
                }
                else
                {
                    OpenNewNotice();
                }
            }
            else
            {

            }
        }, null);
    }


    /// <summary>
    /// 下载图片并且显示出来
    /// </summary>
    private void LoadImage(string thumb)
    {
        //有图片

        string[] ImageMane =thumb.ToString().Split('/');
        string fileName = ImageMane[ImageMane.Length - 1];//获得图片的名称

        path = PathForFile(fileName, "NewpicFile");//平台的判断
        if (GetNativeFile(path) != null)
        {
            //图片已经下载好了。直接调用

            LOG.Info("图片已经下载好了，直接使用");
            ShowNativeTexture();
        }
        else
        {
            //图片没下载下要下载
            LOG.Info("图片没下载，需要下载使用");

            StartCoroutine(UploadPNG(NewInfoList.thumb.ToString(), fileName, "NewpicFile"));

        }
    }

    /// <summary>
    /// 显示图片
    /// </summary>
    public void ShowNativeTexture()
    {
        Texture2D texture = GetNativeFile(path);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        if (Picture == null) return;

        Picture.sprite = sprite;

        Picture.SetNativeSize();

        RectTransform rec = Picture.gameObject.GetComponent<RectTransform>();

        float Pw = rec.rect.width;
        float Ph = rec.rect.height;
        float w = 720 /*Screen.width*/;
        float h = Screen.height;
        // LOG.Info("屏幕宽度：" + w + "--屏幕高度：" + h + "--图片的宽度:" + Pw + "--图片的高度：" + Ph);
        Picture.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(w, w * Ph / Pw * 1.0f);

    }

    #region 下载图片的逻辑
    /// <summary>
    /// 下载图片
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private IEnumerator UploadPNG(string url, string fileName, string dic)
    {
        if (string.IsNullOrEmpty(url))
        {
            LOG.Info("网址为空");
        }
        else
        {
            WWW www = new WWW(url);
            yield return www;
            if (www.isDone)
            {
                byte[] bytes = www.texture.EncodeToPNG();
                path = PathForFile(fileName, dic);//平台的判断

                //LOG.Info("下载完成，文件" + path);
                SaveNativeFile(bytes, path);//保存图片到本地        

                ShowNativeTexture();//显示图片

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


            MyBooksDisINSTANCE.Instance.SetnewfullPath(path);//保存文件夹的路径
            //LOG.Info("保存图片的文件路径是：" + path);
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
    #endregion

    #region 从文件中取得下载的图片逻辑

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
    #endregion


    #region 新闻被评论逻辑

    #region 新闻点赞
    /// <summary>
    /// 点赞
    /// </summary>
    /// <param name="data"></param>
    public void GreatButtonOnclicke(PointerEventData data)
    {

        if (NewInfoList.is_bestests == 0)
        {
            //LOG.Info("点赞了");
            LikeType = 1;
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.NewLike(NewInfoList.newid, LikeType, NewLikeCallBacke);
        }
        else
        {
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(153);
            UITipsMgr.Instance.PopupTips(Localization, false);

            //UITipsMgr.Instance.PopupTips("You've already commented.", false);
        }
    }


    #endregion

    #region 新闻被踩了

    /// <summary>
    /// 踩
    /// </summary>
    /// <param name="data"></param>
    public void OhButtonOnclicke(PointerEventData data)
    {

        if (NewInfoList.is_bestests == 0)
        {
            //LOG.Info("被踩了");
            LikeType = 2;
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.NewLike(NewInfoList.newid, LikeType, NewLikeCallBacke);
        }
        else
        {
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(153);
            UITipsMgr.Instance.PopupTips(Localization, false);

            //UITipsMgr.Instance.PopupTips("You've already commented.", false);
        }
    }
    #endregion

    public void NewLikeCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----NewLikeCallBacke---->" + result);

        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();
            JsonObject jo = JsonHelper.JsonToJObject(result);

            if (jo.code == 200)
            {
                NewInfoList.is_bestests = 1;
                showLoveImage(LikeType);
            }
            else
            {
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(153);
                UITipsMgr.Instance.PopupTips(Localization, false);

                //UITipsMgr.Instance.PopupTips("You've already commented.", false);
            }
        }, null);
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
            NoLove.sprite = ResourceManager.Instance.GetUISprite(unloveOff);
        }
        else if (type == 1)
        {
            //点了赞
            love.sprite = ResourceManager.Instance.GetUISprite(loveON);
            NoLove.sprite = ResourceManager.Instance.GetUISprite(unloveOff);
        }
        else if (type == 2)
        {
            //被踩了
            love.sprite = ResourceManager.Instance.GetUISprite(loveOff);
            NoLove.sprite = ResourceManager.Instance.GetUISprite(unloveON);
        }
    }
    #endregion

    /// <summary>
    /// 销毁这个物体是时候做的逻辑
    /// </summary>
    public void DestroyGame()
    {
        UIEventListener.RemoveOnClickListener(gameObject, GameOnclicke);
        UIEventListener.RemoveOnClickListener(Great, GreatButtonOnclicke);
        UIEventListener.RemoveOnClickListener(Oh, OhButtonOnclicke);


        EventDispatcher.RemoveMessageListener(EventEnum.NewItemOpen, NewItemOpen);
       

        Destroy(gameObject);
        LOG.Info("销毁新闻");
    }
}

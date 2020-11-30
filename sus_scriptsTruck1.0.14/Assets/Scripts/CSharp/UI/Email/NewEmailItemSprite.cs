using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System;
using DG.Tweening;
using pb;
using UGUI;

public class NewEmailItemSprite : MonoBehaviour
{
    private Text Time;
    private Text Tilte;
    private GameObject Delet;
    private Text Content;
    private GameObject ReadMoreButton;
    private GameObject Gift;
    private GameObject Details;
    private GameObject Item;
    private GameObject BGButton;
    private Image Hit;

    private Text DetailsTilte;
    private Text DetailsContent;
    private Image picture;
    private GameObject RewardBg, KeyItem, DiamonItem;
    private Text KeyItemText, DiamonItemText;

    private EmailItemInfo EmailItemInfo;
    private ScrollRect ProbablyScrollView;
    private bool DownLoging;
    private string path;
    private float Picturw = 700;//规定图片宽度为700，可以修改

    private int NumberId;//这个邮件的编号id
    private EmailAndNewForm EmailAndNewForm;

    private GameObject CollectBg;
    private Image CollectButton;
    private Text CollectButtonText;
    private string fileName;
    private const string ClaimImageOn = "EmailForm/btn_focus";
    private const string ClaimImageOff = "EmailForm/btn_focus2";
    private RectTransform GameRect;
    private float DetailsHight = 0;//这个是详情信息的高度
    private float ProbablyHight = 0;//简略信息的高度
    private CanvasGroup DetailsCanvasGroup;
    private int EmailContenNumber = 0; //这个物体在Conten中是第几个
    private int lastMsgid, NowMsgid; //记录点击的ID，看是否是在同一个物体内操作的
    private Scrollbar ScrollbarHorizontal;
    private float HorizontalLastValu = 0;
    private bool HorizontalValuChange = false;

    public void Inite(EmailItemInfo EmailItemInfo, EmailAndNewForm EmailAndNewForm,int EmailContenNumber)
    {
        HorizontalValuChange = false;
        lastMsgid = 0;
        NowMsgid = EmailItemInfo.msgid;
        this.EmailContenNumber = EmailContenNumber;
        ProbablyHight = 232;
        GameRect = transform.GetComponent<RectTransform>();
        this.EmailAndNewForm = EmailAndNewForm;
        DownLoging = false;
        this.NumberId = EmailItemInfo.msgid;
        this.EmailItemInfo = EmailItemInfo;
        ProbablyScrollView = transform.Find("ProbablyScrollView").GetComponent<ScrollRect>();
        Time = transform.Find("ProbablyScrollView/Viewport/Content/Item/TimeBg/Time").GetComponent<Text>();
        Tilte = transform.Find("ProbablyScrollView/Viewport/Content/Item/Tilte").GetComponent<Text>();
        Delet = transform.Find("ProbablyScrollView/Viewport/Content/Item/Delet").gameObject;
        Content = transform.Find("ProbablyScrollView/Viewport/Content/Item/Content").GetComponent<Text>();
        ReadMoreButton = transform.Find("ProbablyScrollView/Viewport/Content/Item/ReadMoreButton").gameObject;
        Gift = transform.Find("ProbablyScrollView/Viewport/Content/Item/Gift").gameObject;
        Details = transform.Find("Details").gameObject;
        DetailsCanvasGroup = Details.GetComponent<CanvasGroup>();

        Item = transform.Find("ProbablyScrollView/Viewport/Content/Item").gameObject;
        BGButton = transform.Find("ProbablyScrollView/Viewport/Content/Item/BGButton").gameObject;
        Hit = transform.Find("ProbablyScrollView/Viewport/Content/Item/Hit").GetComponent<Image>();

        DetailsTilte = transform.Find("Details/TiltGame/DetailsTilte").GetComponent<Text>();
        DetailsContent = transform.Find("Details/DetailsContent").GetComponent<Text>();
        picture = transform.Find("Details/picture").GetComponent<Image>();
        RewardBg = transform.Find("Details/RewardBg").gameObject;
        KeyItem = transform.Find("Details/RewardBg/KeyItem").gameObject;
        DiamonItem = transform.Find("Details/RewardBg/DiamonItem").gameObject;
        KeyItemText = transform.Find("Details/RewardBg/KeyItem/RewarNumber").GetComponent<Text>();
        DiamonItemText = transform.Find("Details/RewardBg/DiamonItem/RewarNumber").GetComponent<Text>();

        CollectBg = transform.Find("Details/CollectBg").gameObject;
        CollectButton = transform.Find("Details/CollectBg/CollectButton").GetComponent<Image>();
        CollectButtonText = transform.Find("Details/CollectBg/CollectButton/CollectButtonText").GetComponent<Text>();
        ScrollbarHorizontal = transform.Find("ProbablyScrollView/Scrollbar Horizontal").GetComponent<Scrollbar>();

        ScrollbarHorizontal.onValueChanged.AddListener(ScrollbarHorizontalValue);
        
        UIEventListener.AddOnClickListener(BGButton, BGButtonOnclicke);
        UIEventListener.AddOnClickListener(Delet, DeletOnButton);
        UIEventListener.AddOnClickListener(CollectButton.gameObject, CollectButtonOnclicke);

        //事件派发注册
        EventDispatcher.AddMessageListener(EventEnum.EmailExpand, EmailExpand);
        EventDispatcher.AddMessageListener(EventEnum.Achieveallmsgprice, Achieveallmsgprice);

        DetailsCanvasGroup.alpha = 0;
        ShowItemInfo();//邮件简略信息显示
        ShowItemDetailsInfo();

        ScrollbarHorizontal.value =0.01f;
        HorizontalLastValu = 0;

    }

    /// <summary>
    /// 点击一键领取成功后，调用的方法
    /// </summary>
    /// <param name="notification"></param>
    private void Achieveallmsgprice(Notification notification)
    {
        //LOG.Info("一键领取派发成功");

        Gift.SetActive(false);
        EmailItemInfo.price_status = 1;//表示奖励已经领取
        ShowItemDetailsInfo();
    }

    /// <summary>
    /// 获得详情信息的高度
    /// </summary>
    private void GetDetailsHight()
    {
        CancelInvoke("GetDetailsHight");
        DetailsHight = Details.GetComponent<RectTransform>().rect.height;

        GameBgHightChange(true);
    }

    /// <summary>
    /// 显示邮件的简略信息
    /// </summary>
    private void ShowItemInfo()
    {
        if (EmailItemInfo != null)
        {
            Time.text = EmailItemInfo.createtime.ToString();
            Tilte.text = EmailItemInfo.title.ToString();
            Content.text = EmailItemInfo.content.ToString();

            if (EmailItemInfo.isprice == 1 && EmailItemInfo.price_status == 0)
            {
                //是奖励而且还是未领取状态
                Gift.SetActive(true);
            }
            else
            {
                Gift.SetActive(false);
            }

            if (EmailItemInfo.status == 0)
            {
                //这个是未读取的状态
                Hit.sprite=ResourceManager.Instance.GetUISprite("EmailForm/bg_sng1");      
                
            }
            else
            {
                Hit.sprite = ResourceManager.Instance.GetUISprite("EmailForm/bg_msg_1");                
            }
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// 显示邮件的详细信息
    /// </summary>
    private void ShowItemDetailsInfo()
    {
        DetailsTilte.text= EmailItemInfo.title.ToString();
        DetailsContent.text = EmailItemInfo.content.ToString();

        if (EmailItemInfo.isprice == 1 )
        {
            //Debug.Log("dffafaf");
            //是奖励类型
            RewardBg.SetActive(true);
            KeyItem.SetActive(false);
            DiamonItem.SetActive(false);
            CollectBg.SetActive(true);
            CollectButtonText.text = CTextManager.Instance.GetText(285);

            if (EmailItemInfo.price_bkey!=0)
            {
                //有钥匙奖励
                KeyItem.SetActive(true);
                KeyItemText.text = EmailItemInfo.price_bkey.ToString();               
            }

            if (EmailItemInfo.price_diamond!=0)
            {
                //有钻石奖励             
                DiamonItem.SetActive(true);
                DiamonItemText.text = EmailItemInfo.price_diamond.ToString();
            }

            if (EmailItemInfo.price_status==0)
            {
                //奖励还有领取
                CollectButton.sprite= ResourceManager.Instance.GetUISprite("EmailForm/bg_iap");
            }
            else
            {
                //奖励已经领取
                CollectButton.sprite = ResourceManager.Instance.GetUISprite("EmailForm/btn_focus2");
            }
        }
        else
        {
            RewardBg.SetActive(false);
            CollectBg.SetActive(false);
        }
     
        if (EmailItemInfo.sign_url !=null&& !string.IsNullOrEmpty(EmailItemInfo.sign_url.ToString()))
        {
            CollectBg.SetActive(true);
        }

    }

    private void BGButtonOnclicke(PointerEventData data)
    {
        Debug.Log("邮件点击了");
        //lastMsgid = 0;
        //NowMsgid = EmailItemInfo.msgid;
        //AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        if (EmailItemInfo.status == 0)
        {
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.ReadingUserEmail(EmailItemInfo.msgid, ReadingUserEmailCallBack);
        }else
        {

            CUIManager.Instance.OpenForm(UIFormName.EmailInfo);
            CUIManager.Instance.GetForm<EmailInfoForm>(UIFormName.EmailInfo).Init(EmailItemInfo);
        }
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
      
    }
    public void ReadingUserEmailCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----ReadingUserEmailCallBack---->" + result);

        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();

            if (EmailItemInfo.status == 0)
            {
                EmailItemInfo.status = 1;//标记已经读取
                UserDataManager.Instance.selfBookInfo.data.unreadmsgcount -= 1;
            }
            ShowItemInfo();

             JsonObject jo = JsonHelper.JsonToJObject(result);

            if (jo.code == 202 || jo.code == 208)
            {
                return;
            }


            UserDataManager.Instance.ReadEmaillList = JsonHelper.JsonToObject<HttpInfoReturn<ReadEmaillListCont<ReadEmaillList>>>(result);
            if (UserDataManager.Instance.ReadEmaillList != null)
            {
                CUIManager.Instance.OpenForm(UIFormName.EmailInfo);
                CUIManager.Instance.GetForm<EmailInfoForm>(UIFormName.EmailInfo).Init(EmailItemInfo);

            }


        }, null);
    }

    /// <summary>
    /// 邮件展开
    /// </summary>
    private void EmailExpand(Notification notification)
    {
        if ((int)notification.Data==NumberId)
        {
            //展开邮件
            EmailExpandOn();
        }
        else
        {
            //收起邮件           
            EmailPutAway();
        }
    }

    /// <summary>
    /// 邮件展开执行的事件逻辑
    /// </summary>
    private void EmailExpandOn()
    {
        if(EmailItemInfo.status == 0)
        {
            //没有读取过
            EmailItemInfo.status = 1;
            EmailAndNewForm.EmailNumberLower();
            //Hit.sprite = ResourceManager.Instance.GetUISprite("EmailForm/bg_sng3");
        }
        LOG.Info("邮件展开");
        ProbablyScrollView.gameObject.SetActive(false);
        DetailsCanvasGroup.alpha = 1;
        Details.SetActive(true);       
        LoadImage();

       
    }

    /// <summary>
    /// 邮件收起
    /// </summary>
    private void EmailPutAway()
    {
        ScrollbarHorizontal.value = 0;
        //LOG.Info("邮件收起");
        ProbablyScrollView.gameObject.SetActive(true);
        Details.SetActive(false);
        GameBgHightChange(false);
    }
    /// <summary>
    /// 这个是当简略信息或者详情信息打开时，背景的高度随着改变
    /// </summary>
    private void GameBgHightChange(bool bo)
    {
        if (bo)
        {
            if (lastMsgid == NowMsgid) return;  //这里是防止当图片没有下载完的时候，又打开了另外的消息，则在下载完图片时候就不需要再打开详细信息了
           
            float startHight = ProbablyHight + 48;
            //详情信息打开
            DOTween.To(() => startHight, (value) => {

                GameRect.sizeDelta = new Vector2(750, value);

            }, DetailsHight+48, 0.3f).OnComplete(()=>{

                EventDispatcher.Dispatch(EventEnum.ContenPostRest.ToString(), EmailContenNumber);

            });//times秒，从Height变到0
        }
        else
        {
            lastMsgid = NowMsgid;
            //简略信息打开
            float startHight = ProbablyHight + 48;
            //详情信息打开
            DOTween.To(() => DetailsHight + 48, (value) => { GameRect.sizeDelta = new Vector2(750, value); }, startHight, 0.3f);//times秒，从Height变到0
        }
    }

    /// <summary>
    /// 下载图片并且显示出来
    /// </summary>
    private void LoadImage()
    {
        if (string.IsNullOrEmpty(EmailItemInfo.email_pic))
        {
            //没有图片显示
            picture.gameObject.SetActive(false);
            //gameObject.SetActive(false);
            Invoke("GetDetailsHight", 0.2f);

        }
        else
        {
            //有图片

            string[] ImageMane = EmailItemInfo.email_pic.ToString().Split('/');
            fileName = ImageMane[ImageMane.Length - 1];//获得图片的名称

            path = PathForFile(fileName, "NewpicFile");//平台的判断
            if (GetNativeFile(path)!=null)
            {
                //图片已经下载好了。直接调用

                LOG.Info("图片已经下载好了，直接使用");               
                ShowNativeTexture();
            }
            else
            {
                //图片没下载下要下载
                LOG.Info("图片没下载，需要下载使用");


                if (!DownLoging)
                {
                    DownLoging = true;
                    //LOG.Info("资源需要下载");
                    StartCoroutine(UploadPNG(EmailItemInfo.email_pic.ToString(), fileName, "NewpicFile"));
                    
                }
            }
        }
    }

    /// <summary>
    /// 点击领取奖励物
    /// </summary>
    /// <param name="data"></param>
    private void CollectButtonOnclicke(PointerEventData data)
    {
        LOG.Info("Collect点击");
        if (EmailItemInfo.sign_url != null && !string.IsNullOrEmpty(EmailItemInfo.sign_url.ToString()))
        {
            //网页跳转
            Application.OpenURL(EmailItemInfo.sign_url);         
        }

        if (EmailItemInfo != null && EmailItemInfo.msg_type == 4)
        {
            LOG.Info("--------emailItemInfo.msg_type-------->" + EmailItemInfo.msg_type);
            SdkMgr.Instance.OpenWebView(EmailItemInfo.email_url, WebViewCallBack);
        }
        else
        {
            if (EmailItemInfo.price_status == 0)
            {
                //奖励没有领取的时候点击按钮领取
                //UINetLoadingMgr.Instance.Show();
                GameHttpNet.Instance.GetEmailAward(EmailItemInfo.msgid, GetEmailAwardCallBack);
            }
            else
            {            
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(169);
                UITipsMgr.Instance.PopupTips(Localization, false);               
            }
        }
    }

    private void WebViewCallBack(string vMsg)
    {
        if (!string.IsNullOrEmpty(vMsg) && vMsg.Equals("2a338a7aa119c2d5433e9f738086560612"))
        {
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.GetEmailAward(EmailItemInfo.msgid, GetEmailAwardCallBack);

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

                CollectButton.sprite = ResourceManager.Instance.GetUISprite(ClaimImageOff);

                UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.emailGetAwardInfo.data.bkey);
                UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.emailGetAwardInfo.data.diamond);
                UserDataManager.Instance.ResetMoney(3, UserDataManager.Instance.emailGetAwardInfo.data.ticket);

                var Localization = GameDataMgr.Instance.table.GetLocalizationById(158);
                UITipsMgr.Instance.PopupTips(Localization, false);

              
                EmailItemInfo.price_status = 1;
                Gift.SetActive(false);

            }
            else
            {             
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(169);
                UITipsMgr.Instance.PopupTips(Localization, false);
                //UITipsMgr.Instance.PopupTips("Already received!", false);
            }

        }, null);
    }

    /// <summary>
    /// 点击删除按钮，删除这个邮件
    /// </summary>
    private void DeletOnButton(PointerEventData data)
    {
        DestroyGame();
    }

    /// <summary>
    /// 显示图片
    /// </summary>
    public void ShowNativeTexture()
    {       
        Texture2D texture = GetNativeFile(path);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        if (picture == null) return;

        picture.sprite = sprite;

        picture.SetNativeSize();
        RectTransform rec = picture.gameObject.GetComponent<RectTransform>();
        float Pw = rec.rect.width;
        float Ph = rec.rect.height;      
        float Hight = Picturw * Ph / Pw * 1.0f;
        rec.sizeDelta = new Vector2(Picturw, Hight);
      
        Invoke("GetDetailsHight", 0.2f);
    }

    private List<float> PxList;
    /// <summary>
    /// 监测ScrollbarHorizontal的变化值
    /// </summary>
    /// <param name="v"></param>
    private void ScrollbarHorizontalValue(float v)
    {
       
        //if (PxList == null) PxList = new List<float>();
        //if (PxList.Count>=10)
        //{
        //    if (PxList[5]> PxList[9])
        //    {
        //        //左移动
        //        ProbablyScrollView.content.anchoredPosition = new Vector2(-124, 0);
        //        PxList.Clear();
        //        Debug.Log("====左边");
        //    }
        //    else
        //    {
        //        //右移动
        //        ProbablyScrollView.content.anchoredPosition = new Vector2(0, 0);
        //        PxList.Clear();
        //        Debug.Log("====右边");
        //    }

        //}else
        //{
        //    float x = ProbablyScrollView.content.anchoredPosition.x;
        //    PxList.Add(x);
        //}

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

    public void DestroyGame()
    {
        UIEventListener.RemoveOnClickListener(BGButton,BGButtonOnclicke);
        UIEventListener.RemoveOnClickListener(Delet,DeletOnButton);
        UIEventListener.RemoveOnClickListener(CollectButton.gameObject, CollectButtonOnclicke);


        EventDispatcher.RemoveMessageListener(EventEnum.EmailExpand, EmailExpand);
        EventDispatcher.RemoveMessageListener(EventEnum.Achieveallmsgprice, Achieveallmsgprice);

        ScrollbarHorizontal.onValueChanged.RemoveListener(ScrollbarHorizontalValue);


        if (picture != null)
            picture.sprite = null;
        if(EmailItemInfo != null && !string.IsNullOrEmpty(EmailItemInfo.email_pic))
            StopCoroutine(UploadPNG(EmailItemInfo.email_pic.ToString(), fileName, "NewpicFile"));
        if(gameObject!= null)
            Destroy(gameObject);

        //LOG.Info("销毁邮件："+NumberId);
    }
}

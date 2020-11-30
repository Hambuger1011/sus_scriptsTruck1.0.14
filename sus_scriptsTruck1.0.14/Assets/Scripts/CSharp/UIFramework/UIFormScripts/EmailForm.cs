using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UGUI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.IO;
using ThisisGame;
using pb;

public class EmailForm : BaseUIForm
{
    public RectTransform BG;
    public GameObject EmailItemGame, NewItemGame;
    public GameObject Content;
    public GameObject NoMessagesGame;
    public GameObject[] topgameTF;
    public GameObject newTochAre, MailBoxTochAre, EmailScrollView;
    public ScrollRect NewScrollView;
    public GameObject CollectAll;

    private string path;
    private string url;
    private int page = 1, newPage = 1;
    private bool isEmailUI = false;

    private GameObject go;
    private ArrayList DownPiceListDic;  //保存需要下载的图片队列
    //private Dictionary<string, string> HadNewPiceList;//保存已经下载好的的图片
    private bool DownLoging = false;//现在处于正在下载的状态
    private bool NewHadDown = false;
    private bool NewInfoContNull = false;

    private List<EmailItemSprite> EmailItemGameList;
    private List<newItemSprite> NewItemGameList;
    public override void OnOpen()
    {
        DownLoging = false;
        DownPiceListDic = null;
        NewHadDown = false;
        NewInfoContNull = false;

        float offerH = 0;
        if (ResolutionAdapter.HasUnSafeArea)
        {
            var scale = this.myForm.GetScale();
            var safeArea = ResolutionAdapter.GetSafeArea();
            var offset = this.myForm.Pixel2View(safeArea.position);
            offerH = offset.y;
        }

        NewScrollView.transform.rectTransform().offsetMax = new Vector2(26, -(88 + offerH));
        EmailScrollView.transform.rectTransform().offsetMax = new Vector2(21, -(108 + offerH));

        MyBooksDisINSTANCE.Instance.returNewGoList().Clear();
        MyBooksDisINSTANCE.Instance.returnHadNewPiceList().Clear();

        //LOG.Info("数组中的数据：" + MyBooksDisINSTANCE.Instance.returNewGoList().Count);
        BG.anchoredPosition = new Vector2(0, 0);

        EmailItemGame.SetActive(false);
        //SpwanEamilItem();

        //TopGameFTChange(1);
        NewScrollView.gameObject.SetActive(false);
        //EmailScrollView.SetActive(true);

        //if (UserDataManager.Instance.NewInfoCont != null)
        //{
        //    //UINetLoadingMgr.Instance.Show();
        //    GameHttpNet.Instance.GetNewInfo(1, NewInfoCallBacke);
        //}

        bar.onValueChanged.AddListener(ScrollbarChange);

        EmailItemGameList = new List<EmailItemSprite>();
        //EmailItemGameList.Clear();

        NewItemGameList = new List<newItemSprite>();
        //NewItemGameList.Clear();
    }

    private void CollectALLShow()
    {
        //邮件一键领取的按钮显示
        if (EmailScrollView.activeSelf && UserDataManager.Instance.lotteryDrawInfo != null && UserDataManager.Instance.lotteryDrawInfo.data != null)
        {

            int Emailshu = UserDataManager.Instance.selfBookInfo.data.unreceivemsgcount;//未领取

            if (CollectAll == null)
            {
                return;
            }
            
            if (Emailshu > 0)
            {
                CollectAll.SetActive(true);
                UIEventListener.AddOnClickListener(CollectAll, CollectAllButtonOn);
            }
            else
            {
                CollectAll.SetActive(false);
            }
        }
    }
    private void CollectAllButtonOn(PointerEventData data)
    {
        LOG.Info("一件领取被点击了");
        GameHttpNet.Instance.Achieveallmsgprice(AchieveallmsgpriceCallBack);
    }

    private void AchieveallmsgpriceCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----AchieveallmsgpriceCallBack---->" + result);
        if (result.Equals("error"))
        {
            LOG.Info("---AchieveallmsgpriceCallBack--这条协议错误");
            return;
        }
        LoomUtil.QueueOnMainThread((param) =>
        {
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo != null)
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.achieveallmsgprice = JsonHelper.JsonToObject<HttpInfoReturn<Achieveallmsgprice>>(arg.ToString());
                 
                    EventDispatcher.Dispatch(EventEnum.Achieveallmsgprice);

                    UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.achieveallmsgprice.data.bkey);
                    UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.achieveallmsgprice.data.diamond);

                    CollectAll.SetActive(false);

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(158);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Received successful!", false);
                }
            }

        }, null);
    }
   
    public void NewTochAreButton(PointerEventData data)
    {    
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        newTochAre.SetActive(true);
        MailBoxTochAre.SetActive(false);
        //TopGameFTChange(2);
        //LOG.Info("新闻");
        NewScrollView.gameObject.SetActive(true);//功能为开放
        EmailScrollView.SetActive(false);
        CollectALLShow();
        if (!NewHadDown)
        {
            NewHadDown = true;
            NewInfoOntNull();//开始加载是第一页
            //LOG.Info("新闻初始");
        }
        else
        {
            //LOG.Info("新闻二次以后加载的信息");
        }

        //UITipsMgr.Instance.PopupTips("The feature is not yet open!", false);
    }

    private void NewInfoCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----NewInfoCallBacke---->" + result);
        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo.code == 200)
            {
                UserDataManager.Instance.NewInfoCont = JsonHelper.JsonToObject<HttpInfoReturn<NewInfoCont>>(result);

                if (NewInfoContNull)
                {
                    NewInfoContNull = false;
                    NewInfoOntNull();
                    return;
                }

                if (newPage>1)
                {
                    NewInfoOntNull();
                }
            }

        }, null);
    }

    private void NewInfoOntNull()
    {
        if (UserDataManager.Instance.NewInfoCont != null)
        {
            List<NewInfoList> item = UserDataManager.Instance.NewInfoCont.data.newlist;

            //LOG.Info("这一页新闻的数量是：" + item.Count);

            for (int i = 0; i < item.Count; i++)
            {

                //储存目前所加载的所有新闻
                MyBooksDisINSTANCE.Instance.returNewGoList().Add(item[i]);

            }
            for (int i = 0; i < item.Count; i++)
            {
                go = Instantiate(NewItemGame);
                go.SetActive(true);
                go.transform.SetParent(NewScrollView.content.transform);

                newItemSprite tem = go.GetComponent<newItemSprite>();
                tem.Init(item[i]);
                NewItemGameList.Add(tem);


                if (DownPiceListDic == null)
                {
                    DownPiceListDic = new ArrayList();
                }


                if (item[i].thumb == null)
                {
                    //LOG.Info("没有封面的下载路径");
                }
                else
                {
                    this.url = item[i].thumb;

                    //测试
                    //this.url = "http://3.16.92.249:30996//uploads//news//20181112//731511d9e9cb9709298f32af276b40f9.png";
                    //end

                    string[] ImageMane = url.Split('/');
                    //LOG.Info("最后的字符串：" + ImageMane[ImageMane.Length - 1] + "--路径：" + url);
                    string fileName = ImageMane[ImageMane.Length - 1];

                    if (MyBooksDisINSTANCE.Instance.returnHadNewPiceList().ContainsValue(fileName))
                    {
                        //图片已经在文件夹里了，不需要下载
                        //LOG.Info("封面已经在资源里了，可以直接用了fileName:" + fileName);

                        EventDispatcher.Dispatch(EventEnum.ShowNewIcon);
                    }
                    else
                    {
                        //下载

                        //保存需要下载的图片
                        DownPiceListDic.Add(this.url);


                        if (!DownLoging)
                        {
                            DownLoging = true;

                            //LOG.Info("资源需要下载");
                            StartCoroutine(UploadPNG((string)DownPiceListDic[0], fileName, "NewpicFile"));
                        }
                    }
                }
            }
        }else
        {
            NewInfoContNull = true;
            GameHttpNet.Instance.GetNewInfo(1, NewInfoCallBacke);
        }
    }

    /// <summary>
    /// 把队列加载完成的从队列中删除掉；
    /// </summary>
    private void DowLogImageReture()
    {
        DownPiceListDic.RemoveAt(0);
        if (DownPiceListDic.Count >= 1)
        {
            //还有带着加载图片的物体
            string urll = (string)DownPiceListDic[0];
            string[] ImageMane = urll.Split('/');
            //LOG.Info("最后的字符串：" + ImageMane[ImageMane.Length - 1] + "--路径：" + url);
            string fileName = ImageMane[ImageMane.Length - 1];

            StartCoroutine(UploadPNG((string)DownPiceListDic[0], fileName, "NewpicFile"));
        }
        else
        {
            DownLoging = false;//下载完成了
            //LOG.Info("所有需要要下载的图片已经下载完成了");
        }
    }

    public void MailBoxTochAreButton(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        //TopGameFTChange(1);
        newTochAre.SetActive(false);
        MailBoxTochAre.SetActive(true);
        //LOG.Info("邮箱");
        NewScrollView.gameObject.SetActive(false);
        EmailScrollView.SetActive(true);

        SpwanEamilItem();

        CollectALLShow();
    }

    #region 翻页实现
    public Scrollbar bar;
    private bool iscanBar = false;
    private void ScrollbarChange(float ve)
    {   
        if (ve<=0&& iscanBar)
        {
            //Debug.Log("值是：" + ve);
            EmailPagUpdate();
        }
        iscanBar = true;
    }
    #endregion
    /// <summary>
    /// 当邮件滑动到底部的时候加载新的邮件
    /// </summary>
    public void EmailPagUpdate()
    {
        //LOG.Info("滑动到了底部了，需要更新邮件");
        page++;
        if (UserDataManager.Instance.EmailList != null)
        {
            int shu = UserDataManager.Instance.EmailList.data.pages_total;
            //LOG.Info("最大页数是：" + shu);
            if (page > shu)
            {
                //LOG.Info("没有更多的邮件啦");
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(159);
                UITipsMgr.Instance.PopupTips(Localization, false);

                //UITipsMgr.Instance.PopupTips("There's no more mail", false);
                return;
            }
            //UINetLoadingMgr.Instance.Show();
            SpwanEamilItem();
        }
    }

    public void NewPagUpdate()
    {
        //LOG.Info("滑动到底部了，更新下一页的新闻");
        newPage++;

        if (UserDataManager.Instance.NewInfoCont != null)
        {
            int shu = UserDataManager.Instance.NewInfoCont.data.pages_total;
            //LOG.Info("最大页数是：" + shu);
            if (newPage > shu)
            {
                //LOG.Info("没有更多的新闻啦");
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(160);
                UITipsMgr.Instance.PopupTips(Localization, false);

                //UITipsMgr.Instance.PopupTips("There's no more news", false);
                return;
            }
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.GetNewInfo(newPage, NewInfoCallBacke);//开始加载是第一页
        }
    }
    /// <summary>
    /// 这里是生成邮件
    /// </summary>
    private void SpwanEamilItem()
    {
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.GetEmail(page, GetEmailCallBacke);
    }

    public void GetEmailCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetEmailCallBacke---->" + result);
        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo.code == 200)
            {          
                UserDataManager.Instance.EmailList = JsonHelper.JsonToObject<HttpInfoReturn<EmailListCont>>(result);

                if (UserDataManager.Instance.EmailList != null)
                {
                    List<EmailItemInfo> Emailistchil = UserDataManager.Instance.EmailList.data.sysarr;
                    int sh = UserDataManager.Instance.EmailList.data.sysarr_count;
                    //LOG.Info("邮箱总数量:" + sh);

                    if (Emailistchil != null)
                    {
                        int len = Emailistchil.Count;
                        //LOG.Info("总共有的邮件数量是：" + len);

                        if (len <= 0)
                        {
                            NoMessagesGame.SetActive(true);
                            return;
                        }
                        else
                        {
                            //LOG.Info("生成邮件的当前页数是：" + page);

                            for (int i = 0; i < len; i++)
                            {
                                EmailItemInfo item = Emailistchil[i];
                                int msgid = item.msgid;
                                string title = item.title;
                                string content = item.content;
                                string createtime = item.createtime;
                                int status = item.status;
                                int msg_type = item.msg_type;
                                int price_bkey = item.price_bkey;
                                int price_diamond = item.price_diamond;
                                int price_status = item.price_status;

                                //LOG.Info("msgid:" + msgid + "--title:" + title + "--content:" + content + "--createtime:" + createtime + "--status:" + status+"--msg_type:"+msg_type+"--price_bkey:"+price_bkey+"--price_diamond:"+price_diamond+"--price_status:"+price_status);

                                GameObject go = Instantiate(EmailItemGame);
                                go.SetActive(true);
                                go.transform.SetParent(Content.transform);

                                EmailItemSprite  tem= go.GetComponent<EmailItemSprite>();
                                tem.Init(item);
                                EmailItemGameList.Add(tem);
                            }
                        }
                    }
                }
            }
        }, null);
    }


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
        }else
        {
            WWW www = new WWW(url);
            yield return www;
            if (www.isDone)
            {
                byte[] bytes = www.texture.EncodeToPNG();
                path = PathForFile(fileName, dic);//平台的判断

                //LOG.Info("下载完成，文件" + path);
                SaveNativeFile(bytes, path);//保存图片到本地        


                if (MyBooksDisINSTANCE.Instance.returnHadNewPiceList().ContainsValue(fileName))
                {
                    //图片已经在文件夹里了，不需要下载
                }
                else
                {
                    MyBooksDisINSTANCE.Instance.returnHadNewPiceList().Add(path, fileName);//保存已经下载好的文件路径和图片名称
                }

                EventDispatcher.Dispatch(EventEnum.ShowNewIcon);

                DowLogImageReture();//删除最前面的物体，开始下载后面的
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


    /// <summary>
    /// 显示图片
    /// </summary>
    public void ShowNativeTexture()
    {
        //go.GetComponent<MeshRenderer>().material.mainTexture = GetNativeFile(path);

        Texture2D texture = GetNativeFile(path);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
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

    private void SetImageInitFile()
    {
        if (PlayerPrefs.GetString("FilePath") != null)
        {
            //LOG.Info("图片保存路径不为空");

            //获取指定路径下面的所有资源文件  
            if (Directory.Exists(MyBooksDisINSTANCE.Instance.returnNewFullPath()))
            {
                DirectoryInfo direction = new DirectoryInfo(MyBooksDisINSTANCE.Instance.returnNewFullPath());
                FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

                //LOG.Info("文件夹里有：" + files.Length + "--张图片");

                if (files.Length == 0)
                {
                    return;
                }

                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".meta"))
                    {
                        continue;
                    }
                    //LOG.Info("图片的名字是:" + files[i].Name);  //打印出来这个文件架下的所有文件

                    string fileName = files[i].Name;
                    string pathName = PlayerPrefs.GetString("FilePath");


                    if (MyBooksDisINSTANCE.Instance.returnHadNewPiceList().ContainsValue(fileName))
                    {
                        //图片已经在文件夹里了，不需要下载
                    }
                    else
                    {
                        string paths = Path.Combine(pathName, fileName);

                        //LOG.Info("初始化的时候保存的路径是：" + paths);
                        MyBooksDisINSTANCE.Instance.returnHadNewPiceList().Add(paths, fileName);//保存已经下载好的文件路径和图片名称
                    }
                    //LOG.Info( "FullName:" + files[i].FullName );  
                    //LOG.Info( "DirectoryName:" + files[i].DirectoryName );  
                }
            }
        }
    }

    private void OnEnable()
    {

        UserDataManager.Instance.NewInfoCont = null;

        SetImageInitFile();

      
    }

    public override void OnClose()
    {
        base.OnClose();
        bar.onValueChanged.RemoveListener(ScrollbarChange);

        if (EmailItemGameList!=null)
        {
            for (int i=0;i< EmailItemGameList.Count;i++)
            {
                if(EmailItemGameList[i]!=null)
                   EmailItemGameList[i].Disposte();
            }

            EmailItemGameList = null;
        }

        if (NewItemGameList!=null)
        {
            for (int i = 0; i < NewItemGameList.Count; i++)
            {
                NewItemGameList[i].Disposte();
            }

            NewItemGameList = null;
        }
    }
   
    private void OnDisable()
    {
        DownPiceListDic = null;
        MyBooksDisINSTANCE.Instance.returNewGoList().Clear();

        //获取指定路径下面的所有资源文件  
        if (Directory.Exists(MyBooksDisINSTANCE.Instance.returnNewFullPath()))
        {
            DirectoryInfo direction = new DirectoryInfo(MyBooksDisINSTANCE.Instance.returnNewFullPath());
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

            //LOG.Info("文件夹里有：" + files.Length + "--张图片");

            //当图片的张数多的时候清除
            if (files.Length > 10)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".meta"))
                    {
                        continue;
                    }
                    //LOG.Info("图片的名字是:" + files[i].Name);  //打印出来这个文件架下的所有文件

                    files[i].Delete();
                    //LOG.Info( "FullName:" + files[i].FullName );  
                    //LOG.Info( "DirectoryName:" + files[i].DirectoryName );  
                }
                //LOG.Info("图片大于规定数量，清除");
                Dictionary<string, string> dic = MyBooksDisINSTANCE.Instance.returnHadNewPiceList();
                dic.Clear();
                //dic = null;

            }
        }
    }
}

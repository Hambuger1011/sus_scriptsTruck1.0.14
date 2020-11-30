using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class EmailInfoForm : BaseUIForm
{
    [System.NonSerialized]
    public EmailInfoView EmailInfoView;
    [System.NonSerialized]
    public EmailInfoLogic EmailInfoLogic;
    [System.NonSerialized]
    public EmailItemInfo EmailItemInfo;

    private string fileName;
    private string path;
    private bool DownLoging;
    private Image picture;
    private float Picturw = 700;//规定图片宽度为700，可以修改

    public override void OnClose()
    {
        base.OnClose();

        EmailInfoLogic.Close();

    }


    public void Init(EmailItemInfo EmailItemInfo)
    {
        DownLoging = false;
        this.EmailItemInfo = EmailItemInfo;
        EmailInfoView = new EmailInfoView(this);
        EmailInfoLogic = new EmailInfoLogic(this);

        picture = transform.Find("Bg/ScrollView/Viewport/Content/picture").GetComponent<Image>();
        LoadImage();
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
           
        }
        else
        {
            //有图片

            string[] ImageMane = EmailItemInfo.email_pic.ToString().Split('/');
            fileName = ImageMane[ImageMane.Length - 1];//获得图片的名称

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
    /// 显示图片
    /// </summary>
    public void ShowNativeTexture()
    {
        Texture2D texture = GetNativeFile(path);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        if (picture == null) return;
        picture.gameObject.SetActive(true);
        picture.sprite = sprite;

        picture.SetNativeSize();
        RectTransform rec = picture.gameObject.GetComponent<RectTransform>();
        float Pw = rec.rect.width;
        float Ph = rec.rect.height;
        float Hight = Picturw * Ph / Pw * 1.0f;
        rec.sizeDelta = new Vector2(Picturw, Hight);

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


}

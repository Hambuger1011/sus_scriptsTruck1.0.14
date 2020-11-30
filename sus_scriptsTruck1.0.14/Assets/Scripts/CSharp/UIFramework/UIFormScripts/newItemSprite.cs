using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UGUI;
using System.IO;
using System;
using pb;

public class newItemSprite : MonoBehaviour
{

    public Text title, times, content,comTotal;
    public Image love, unlove,titlImage,loding;
    public GameObject NewItembg,greatButton,ohButton, IsHadRead;
    public RectTransform BgRectTransform;

    private NewInfoList newinfolist;
    private bool hadLove = false, is_read=false,isnofirst=false;
    private int LikeType = 0;
    private string path;
    private string url;
    private float height = 0f;

    private const string loveON = "EmailForm/bg_icond_03";
    private const string loveOff = "EmailForm/bg_iconc_03";
    private const string unloveON = "EmailForm/bg_iconc_05";
    private const string unloveOff = "EmailForm/bg_icond_05";

   
    /// <summary>
    /// 这里是对新闻的列表的信息初始化
    /// </summary>
    /// <param name="newinfolist"></param>
    public void Init(NewInfoList newinfolist)
    {
        //isnofirst = false;
        if (!isnofirst)
        {
            isnofirst = true;
            UIEventListener.AddOnClickListener(NewItembg, NewItemButtonOnclick);
            UIEventListener.AddOnClickListener(greatButton, GreatButtonOnclicke);
            UIEventListener.AddOnClickListener(ohButton, OhButtonOnclicke);
            //addMessageListener(EventEnum.ShowNewIcon, ShowNewIcon);
            EventDispatcher.AddMessageListener(EventEnum.ShowNewIcon, ShowNewIcon);
        }
       
        if (newinfolist.thumb==null)
        {
            loding.gameObject.SetActive(false);
        }

        content.text = "";
        height = content.preferredHeight;


        this.newinfolist = newinfolist;


        title.text = newinfolist.title.ToString();
        times.text = newinfolist.createtime.ToString();
        content.text = newinfolist.content.ToString();
        comTotal.text = newinfolist.com_total.ToString();//新闻评论的总数

      
        int line = GetLineCount();//计算出这个text文本所占的行数

        //Debug.Log("行数有："+line);

        if (line>=2)
        {
            //超过两行，显示出两行的数量
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(539, 85);
            BgRectTransform.sizeDelta = new Vector2(692,636);

        }else
        {
            BgRectTransform.sizeDelta = new Vector2(692, 289);

        }


        hadReading(newinfolist.is_read);
        showLoveImage(newinfolist.is_bestests);
        ShowNewIcon(null);//显示新闻的icon

    }

    /// <summary>
    /// 这个是计算文本的行数
    /// </summary>
    /// <returns></returns>
    private int GetLineCount()
    {
        return (int)(content.preferredHeight / height);
    }
    /// <summary>
    /// 此新闻是否已经阅读了
    /// </summary>
    private void hadReading(int is_reads)
    {
        if (is_reads == 1 || is_read)
        {
            //已读
            IsHadRead.SetActive(false);
        }
        else
        {
            //未读
            IsHadRead.SetActive(true);
        }

        
    }
    /// <summary>
    /// 显示爱心的状态
    /// </summary>
    public void showLoveImage(int type)
    {
        if (type==0)
        {
            //还未做出评论
            love.sprite = ResourceManager.Instance.GetUISprite(loveOff);
            unlove.sprite = ResourceManager.Instance.GetUISprite(unloveOff);
        }
        else if (type==1)
        {
            //点了赞
            love.sprite = ResourceManager.Instance.GetUISprite(loveON);
            unlove.sprite = ResourceManager.Instance.GetUISprite(unloveOff);
            is_read = true;
        }
        else if (type==2)
        {
            //被踩了
            love.sprite = ResourceManager.Instance.GetUISprite(loveOff);
            unlove.sprite = ResourceManager.Instance.GetUISprite(unloveON);
            is_read = true;
        }
    }

    public void NewItemButtonOnclick(PointerEventData data)
    {
        //LOG.Info("新闻事件点击了");
        IsHadRead.SetActive(false);
        if (newinfolist.is_read==1)
        {
            //这个是已经读取的
            AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
            OpenNewNotice();
        }else if(is_read)
        {
            //已经读了
            //这个是已经读取的
            AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
            OpenNewNotice();
        }
        else
        {
            AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
            if (!hadLove)
            {
                //UINetLoadingMgr.Instance.Show();
                GameHttpNet.Instance.NewReading(newinfolist.newid, NewReadingCallBacke);
            }          
        }
        EventDispatcher.Dispatch(EventEnum.GetEmailShowHint, 1);
    }

    private void OpenNewNotice()
    {
          
        //CUIManager.Instance.OpenForm(UIFormName.EmailNotice);
        //newItemSprite tem = this.gameObject.GetComponent<newItemSprite>();
        //CUIManager.Instance.GetForm<EmailNoticeSprite>(UIFormName.EmailNotice).NewInit(tem, this.newinfolist);
    }
   
    /// <summary>
    /// 点赞
    /// </summary>
    /// <param name="data"></param>
    public void GreatButtonOnclicke(PointerEventData data)
    {
        
        if (!hadLove && newinfolist.is_bestests == 0)
        {
            //LOG.Info("点赞了");
            hadLove = true;
            LikeType = 1;
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.NewLike(newinfolist.newid, LikeType, NewLikeCallBacke);
        }else
        {
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(153);
            UITipsMgr.Instance.PopupTips(Localization, false);

            //UITipsMgr.Instance.PopupTips("You've already commented.", false);
        }
    }

    /// <summary>
    /// 踩
    /// </summary>
    /// <param name="data"></param>
    public void OhButtonOnclicke(PointerEventData data)
    {
       
        if (!hadLove && newinfolist.is_bestests == 0)
        {
            //LOG.Info("被踩了");
            hadLove = true;
            LikeType = 2;
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.NewLike(newinfolist.newid, LikeType, NewLikeCallBacke);
        }
        else
        {
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(153);
            UITipsMgr.Instance.PopupTips(Localization, false);

            //UITipsMgr.Instance.PopupTips("You've already commented.", false);
        }
    }

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
                is_read = true;
                OpenNewNotice();
            }
            else
            {
                               
            }
        }, null);
    }

    /// <summary>
    /// 这里显示新闻的icon
    /// </summary>
    /// <param name="notification"></param>
    private void ShowNewIcon(Notification notification)
    {
        //测试
        //string url = "http://3.16.92.249:30996//uploads//news//20181112//731511d9e9cb9709298f32af276b40f9.png";
        //end

        if (newinfolist==null)
        {
            //LOG.Info("空");
            return;
        }
        if (string.IsNullOrEmpty(newinfolist.thumb))
        {
            //LOG.Info("网址为空");
            url = "http://3.16.92.249:30996//uploads//news//20181112//731511d9e9cb9709298f32af276b40f9.png";

            titlImage.gameObject.SetActive(false);
            BgRectTransform.sizeDelta = new Vector2(692, 289);
        }
        else
        {
            url = newinfolist.thumb;//正式

            BgRectTransform.sizeDelta = new Vector2(692, 607);
        }
       


        string[] ImageMane = url.Split('/');     
        string fileName = ImageMane[ImageMane.Length - 1];

        
        if (MyBooksDisINSTANCE.Instance.returnHadNewPiceList().ContainsValue(fileName))
        {
            //图片已经加载完成了

            if (loding == null) return;
            loding.gameObject.SetActive(false);

            foreach (KeyValuePair<string, string> kvp in MyBooksDisINSTANCE.Instance.returnHadNewPiceList())
            {
                if (kvp.Value.Equals(fileName))
                {
                    path = kvp.Key;
                }
            }

            Texture2D texture = GetNativeFile(path);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            titlImage.sprite = sprite;
        }else
        {
            //LOG.Info("newinfolist.thumb:" + newinfolist.thumb + "--fileName:" + fileName);
        }
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
   
    public void Disposte()
    {
        UIEventListener.RemoveOnClickListener(NewItembg, NewItemButtonOnclick);
        UIEventListener.RemoveOnClickListener(greatButton, GreatButtonOnclicke);
        UIEventListener.RemoveOnClickListener(ohButton, OhButtonOnclicke);
        EventDispatcher.RemoveMessageListener(EventEnum.ShowNewIcon, ShowNewIcon);
        titlImage.sprite = null;
        love.sprite = null;
        unlove.sprite = null;


        Destroy(gameObject);
    }
}

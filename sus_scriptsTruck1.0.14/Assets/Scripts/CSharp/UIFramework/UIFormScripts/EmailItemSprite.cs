using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UGUI;
using UnityEngine.UI;
using System;
using pb;

public class EmailItemSprite : MonoBehaviour
{

    public GameObject bg,destroy;
    public Text TimeText,NoticeText,DwonText;
    public Image icon,gethad;

    private int msgid;
    private string title;
    private string content;
    private string createtime;
    private int status;
    private int msg_type;
    private int price_bkey;
    private int price_diamond;
    private int price_status;
    private EmailItemInfo emailItemInfo;
    private ReadEmaillList ReadEmaillListInfo;

    private const string EmailOff = "EmailForm/bg_jzke_03";
    private const string EmailOn = "EmailForm/bg_zklea";
    private const string getOff = "EmailForm/bg_lzke_03";
    private const string getOn = "EmailForm/bg_icon_msg";

    public void Init(EmailItemInfo item)
    {
        emailItemInfo = item;
        this.msgid = item.msgid;
        this.title = item.title;
        this.content = item.content;
        this.createtime = item.createtime;
        this.status = item.status;
        this.msg_type = item.msg_type;
        this.price_bkey = item.price_bkey;
        this.price_diamond = item.price_diamond;
        this.price_status = item.price_status;

        UIEventListener.AddOnClickListener(bg, GameObjectButton);
        UIEventListener.AddOnClickListener(destroy, destroyEmailButton);
        EventDispatcher.AddMessageListener(EventEnum.Achieveallmsgprice, Achieveallmsgprice);

        EmailItemInfo();       
    }

    /// <summary>
    /// 点击一键领取成功后，调用的方法
    /// </summary>
    /// <param name="notification"></param>
    private void Achieveallmsgprice(Notification notification)
    {
        //LOG.Info("一键领取派发成功");

        this.status = 1;//表示已读      
        this.price_status = 1;//表示奖励已经领取
        emailItemInfo.price_status = 1;//表示奖励已经领取
        EmailItemInfo();
    }

    /// <summary>
    /// 销毁物体，释放内存
    /// </summary>
    public void Disposte()
    {
        UIEventListener.RemoveOnClickListener(bg, GameObjectButton);
        UIEventListener.RemoveOnClickListener(destroy, destroyEmailButton);
        EventDispatcher.RemoveMessageListener(EventEnum.Achieveallmsgprice, Achieveallmsgprice);

        icon.sprite = null;
        gethad.sprite = null;

        Destroy(gameObject);
    }
   
    private void EmailItemInfo()
    {
        TimeText.text = createtime.ToString();
        NoticeText.text = title.ToString();
        DwonText.text = content.ToString();

        if (icon==null)
        {
            return;
        }
        if (status==0)
        {
            //这个是未读取的状态
            //icon.sprite=ResourceManager.Instance.GetUISprite(EmailOff);
            icon.gameObject.SetActive(true);
        }
        else
        {
            //icon.sprite = ResourceManager.Instance.GetUISprite(EmailOn);
            icon.gameObject.SetActive(false);
        }
     
        if (emailItemInfo.isprice==1)
        {
            //这个邮件带有奖励
            gethad.gameObject.SetActive(true);

            if (emailItemInfo.price_status == 0)
            {
                //奖励未领取
                //gethad.sprite= ResourceManager.Instance.GetUISprite(getOn);

                gethad.gameObject.SetActive(true);
            }
            else if (emailItemInfo.price_status == 1)
            {
                //奖励已经领取
                //gethad.sprite = ResourceManager.Instance.GetUISprite(getOff);

                gethad.gameObject.SetActive(false);
            }
        }
        else
        {
            gethad.gameObject.SetActive(false);
        }

       
    }

    public void gethadImageChange(int price_status)
    {
        if (emailItemInfo.isprice == 1)
        {
            //邮件带有奖励

            if (price_status == 0)
            {
                //奖励未领取
                //gethad.sprite = ResourceManager.Instance.GetUISprite(getOn);

                gethad.gameObject.SetActive(true);
            }
            else if (price_status == 1)
            {
                //奖励已经领取
                //gethad.sprite = ResourceManager.Instance.GetUISprite(getOff);

                gethad.gameObject.SetActive(false);
            }
        }
           
    }

    /// <summary>
    /// 打开邮件阅读界面
    /// </summary>
    /// <param name="data"></param>
    public void GameObjectButton(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        //UINetLoadingMgr.Instance.Show();
        //LOG.Info("msgid:" + msgid+ "--emailItemInfo.msgid:"+ msgid);
        GameHttpNet.Instance.ReadingUserEmail(msgid, ReadingUserEmailCallBack);

        EventDispatcher.Dispatch(EventEnum.GetEmailShowHint, 0);
    }

    private void destroyEmailButton(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
       // LOG.Info("删除邮件");
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.DeletUserEmail(emailItemInfo.msgid, DeleUserEmailCallBack);
    }
    public void ReadingUserEmailCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----ReadingUserEmailCallBack---->" + result);

        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();
            if (status == 0)
            {
                //这里是当点击了这封邮件，刷新这封邮件的状态
                status = 1;
                EmailItemInfo();
            }

            JsonObject jo = JsonHelper.JsonToJObject(result);

            if (jo.code == 202||jo.code==208)
            {
                return;
            }
           
            UserDataManager.Instance.ReadEmaillList = JsonHelper.JsonToObject<HttpInfoReturn<ReadEmaillListCont<ReadEmaillList>>>(result);
            if (UserDataManager.Instance.ReadEmaillList != null)
            {
                ReadEmaillListInfo = UserDataManager.Instance.ReadEmaillList.data.sysarr;

                this.price_status = ReadEmaillListInfo.price_status;
            }

            //LOG.Info("点击打开邮箱阅读");
            CUIManager.Instance.OpenForm(UIFormName.EmailTextNotice);
            EmailItemSprite tem = this.gameObject.GetComponent<EmailItemSprite>();
            CUIManager.Instance.GetForm<EmailTextNoticeForm>(UIFormName.EmailTextNotice).Inite(tem, ReadEmaillListInfo);

        }, null);

    }

    private void DeleUserEmailCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----DeleUserEmailCallBack---->" + result);


        LoomUtil.QueueOnMainThread((param) => {
            //UINetLoadingMgr.Instance.Close();
           
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo.code == 200)
            {
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(161);
                UITipsMgr.Instance.PopupTips(Localization, false);

                //UITipsMgr.Instance.PopupTips("Deleted successfully", false);
                Destroy(gameObject);
            }
            else
            {
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(162);
                UITipsMgr.Instance.PopupTips(Localization, false);

                //UITipsMgr.Instance.PopupTips("Delete failed", false);
            }
          
        }, null);

    }
}

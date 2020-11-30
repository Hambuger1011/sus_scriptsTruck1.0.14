using pb;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamePlayFeedback : BaseUIForm
{

    public GameObject UIMask;
    public Image sendButton;
    public InputField InputField;
    public GameObject sendgame, okgame,okButton;

    private string InputString="";
    private float times=1;

    private const string sendOn = "EmailForm/bg_jsnale_03";
    private const string sendOff = "EmailForm/bg_manek_03";
    //默认文件目录
    private string UnityPath = @"C:/Users/Administrator/Desktop/1.xlsx";

    public override void OnOpen()
    {
        base.OnOpen();

        sendgame.SetActive(true);
        okgame.SetActive(false);

        UIEventListener.AddOnClickListener(UIMask, UIMaskButtonClose);
        UIEventListener.AddOnClickListener(sendButton.gameObject, SendButtonOnClicke);
        UIEventListener.AddOnClickListener(okButton, OKButtonOnclicke);

        InputField.onValueChanged.AddListener(InputChangeHandler);
    }

    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(UIMask, UIMaskButtonClose);
        UIEventListener.RemoveOnClickListener(sendButton.gameObject, SendButtonOnClicke);
        UIEventListener.RemoveOnClickListener(okButton, OKButtonOnclicke);

        InputField.onValueChanged.RemoveListener(InputChangeHandler);
    }

    private void InputChangeHandler(string vStr)
    {
        InputString = InputField.text;
        if (InputString.Length == 10)
        {
            sendButton.sprite = ResourceManager.Instance.GetUISprite(sendOn);
        }
        else if (InputString.Length < 10)
        {
            sendButton.sprite = ResourceManager.Instance.GetUISprite(sendOff);
        }
    }
  
    public void UIMaskButtonClose(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CUIManager.Instance.CloseForm(UIFormName.GamePlayFeedback);

    }

    private void SendButtonOnClicke(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        InputString = InputField.text;

        if (InputString.Length<10)
        {
            AudioManager.Instance.PlayTones(AudioTones.LoseFail);

            var Localization = GameDataMgr.Instance.table.GetLocalizationById(163);
            UITipsMgr.Instance.PopupTips(Localization, false);

            //UITipsMgr.Instance.PopupTips("Send more than 4 characters!", false);
            return;
        }

       // LOG.Info("你发送的内容是：" + InputString);

        // SendEmail(UnityPath, InputString);
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.UserFeedback(2,"",InputString,UserFeedbackCallBack);
    }

    private void OKButtonOnclicke(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CUIManager.Instance.CloseForm(UIFormName.GamePlayFeedback);
    }   


    private void UserFeedbackCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----UserFeedbackCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo!=null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    InputField.text = "";
                    sendgame.SetActive(false);
                    okgame.SetActive(true);
                    AudioManager.Instance.PlayTones(AudioTones.RewardWin);
                }
                else
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(165);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Error, send again!", false);
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                }
            }, null);
        }
    }


    /// <summary>
    /// 邮件发送
    /// </summary>
    private void SendEmail(string UnityPath,string sendString)
    {
        MailMessage mail = new MailMessage();
        //发送邮箱的地址
        mail.From = new MailAddress("send@mail.onyx.fun");
        //收件人邮箱地址 如需发送多个人添加多个Add即可
        mail.To.Add("382109448@qq.com");
        //标题
        mail.Subject = "Feedback";
        //正文
        mail.Body = sendString;
        //添加一个本地附件 
        //mail.Attachments.Add(new Attachment(UnityPath));

        //所使用邮箱的SMTP服务器
        SmtpClient smtpServer = new SmtpClient("smtp.exmail.qq.com");
        //SMTP端口
        smtpServer.Port = 587;
        //账号密码 一般邮箱会提供一串字符来代替密码
        smtpServer.Credentials = new System.Net.NetworkCredential("send@mail.onyx.fun", "OnyxGames00") as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };

        smtpServer.Send(mail);
        //Debug.Log("success");
    }


}

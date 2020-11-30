using pb;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadingFeedBackForm : BaseUIForm {

    private GameObject Mask,SendButton, SendButtOn, SendButtOff;
    private InputField EmailInput, MessageInput,uuidInput;
    private Dropdown dropdown;

    private string SendSt,EmailSt,StandardSt,UUid, deviceModelSt;
    private Image sendButtonImage;

#if UNITY_IOS
    private string platform = "iOS";
#elif UNITY_ANDROID
    private string platform = "Android";
#else
    private string platform = "Android";
#endif

    public override void OnOpen()
    {
        base.OnOpen();

        Mask = transform.Find("Canvas/Mask").gameObject;
        EmailInput = transform.Find("Canvas/BG/WriteText/InputField").GetComponent<InputField>();
        dropdown = transform.Find("Canvas/BG/StandardText/Dropdown").GetComponent<Dropdown>();
        MessageInput = transform.Find("Canvas/BG/MessageText/InputField").GetComponent<InputField>();
        uuidInput = transform.Find("Canvas/BG/UUID").GetComponent<InputField>();
        SendButton = transform.Find("Canvas/BG/MessageText/send").gameObject;
        SendButtOn = transform.Find("Canvas/BG/MessageText/send/sendOn").gameObject;
        SendButtOff = transform.Find("Canvas/BG/MessageText/send/sendOff").gameObject;
       
        sendButtonImage = SendButton.GetComponent<Image>();

        MessageInput.onValueChanged.AddListener(MessageInputChange);

        UIEventListener.AddOnClickListener(Mask,MaskOnclicke);
        UIEventListener.AddOnClickListener(SendButton, SendButtonOnclicke);

        //UUID
        UUid = GameHttpNet.Instance.UUID.ToString();
        uuidInput.text ="ID:"+ UUid;
        //型号
        deviceModelSt = SystemInfo.deviceModel.ToString();

        SendButtOn.SetActive(false);
        SendButtOff.SetActive(true);

        //List<string> sti = new List<string>();
        //sti.Add("XING");
        //dropdown.AddOptions(sti);
    }



    public override void OnClose()
    {
        base.OnClose();

        UIEventListener.RemoveOnClickListener(Mask, MaskOnclicke);
        UIEventListener.RemoveOnClickListener(SendButton, SendButtonOnclicke);

        MessageInput.onValueChanged.RemoveListener(MessageInputChange);


    }

    private void MaskOnclicke(PointerEventData data)
    {
        CUIManager.Instance.CloseForm(UIFormName.LoadingFeedBack);
    }

    private void MessageInputChange(string vStr)
    {
        string InputString = MessageInput.text;
        if (InputString.Length >= 10)
        {
            SendButtOn.SetActive(true);
            SendButtOff.SetActive(false);
        }
        else if (InputString.Length < 10)
        {
            SendButtOn.SetActive(false);
            SendButtOff.SetActive(true);
        }
    }
    private void SendButtonOnclicke(PointerEventData data)
    {
        float value = dropdown.value;

       
        if (value==0)
        {
            StandardSt = "Standard questions:other\n";
        }
        else if (value == 1)
        {
            StandardSt = "Standard questions:The game is stuck at the loading page.\n";
        }
        else if (value == 2)
        {
            StandardSt = "Standard questions:I can't log in to Facebook/Google\n";
        }

        SendSt = "Message:" + MessageInput.text.ToString()+"\n";
        EmailSt = "email:" + EmailInput.text.ToString() + "\n";
        //UUID
        UUid = "UUid:" + GameHttpNet.Instance.UUID.ToString()+"\n";
        //型号
        deviceModelSt = "deviceModelSt:"+ SystemInfo.deviceModel.ToString() + "\n";
        string platformSt = "platform:" + platform + "\n";

        //发送的内容是
        string SendCon = UUid + platformSt + deviceModelSt + EmailSt + SendSt;

        //Debug.Log("SendCon："+ SendCon);
        if (MessageInput.text.ToString().Length>=10)
        {
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.UserFeedback(1,"",SendCon, UserFeedbackCallBack);

        }else
        {
            //var Localization = GameDataMgr.Instance.table.GetLocalizationById(241);
            UITipsMgr.Instance.PopupTips("Send more than 10 characters!", false);

        }


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
                    UIAlertMgr.Instance.Show("TIPS", "Thank you for your support. We'll check the problem and give you an answer as soon as possible.");
                    MaskOnclicke(null);
                }
                else
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(241);
                    UITipsMgr.Instance.PopupTips("Send more than 10 characters!", false);

                }
            }          
        }, null);
    }

   
}

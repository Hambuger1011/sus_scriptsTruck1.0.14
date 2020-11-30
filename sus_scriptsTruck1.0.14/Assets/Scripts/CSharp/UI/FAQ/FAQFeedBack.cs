using pb;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FAQFeedBack : BaseUIForm
{
    private GameObject Mask, SendButton, SendButtOn, SendButtOff;
    private InputField  MessageInput, uuidInput;
    private Dropdown dropdown,subDropdown;

    private string SendSt, StandardSt, UUid, deviceModelSt;
    private Image sendButtonImage;

    private List<FeedbackItemInfo> questList;

    private int mQuestionType = -1;


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

        Mask = transform.Find("Canvas/BG/Top/close").gameObject;
       
        dropdown = transform.Find("Canvas/BG/StandardText/Dropdown").GetComponent<Dropdown>();
        subDropdown = transform.Find("Canvas/BG/SubQuestionText/Dropdown").GetComponent<Dropdown>();
        MessageInput = transform.Find("Canvas/BG/MessageText/InputField").GetComponent<InputField>();
        uuidInput = transform.Find("Canvas/BG/UUID").GetComponent<InputField>();
        SendButton = transform.Find("Canvas/BG/MessageText/send").gameObject;
        SendButtOn = transform.Find("Canvas/BG/MessageText/send/sendOn").gameObject;
        SendButtOff = transform.Find("Canvas/BG/MessageText/send/sendOff").gameObject;

        sendButtonImage = SendButton.GetComponent<Image>();

        MessageInput.onValueChanged.AddListener(MessageInputChange);

        UIEventListener.AddOnClickListener(Mask, MaskOnclicke);
        UIEventListener.AddOnClickListener(SendButton, SendButtonOnclicke);
        dropdown.onValueChanged.AddListener(ChangeTypeQuestionHandler);
        subDropdown.onValueChanged.AddListener(ChangeSubQuestionHandler);

        //UUID
        UUid = GameHttpNet.Instance.UUID.ToString();
        uuidInput.text = "ID:" + UUid;
        //型号
        deviceModelSt = SystemInfo.deviceModel.ToString();

        SendButtOn.SetActive(false);
        SendButtOff.SetActive(true);

        InitQuestion();
    }


    private void InitQuestion()
    {
        questList = new List<FeedbackItemInfo>();

        questList.Add(SplitItem("Purchasing Issues", "I charged my diamonds but i did not get them.\nI charged my keys but i did not get them."));
        questList.Add(SplitItem("Loading Issues", "The game is stuck at the login loading page.\n The game stuck at book loading page.\n The game stuck at entering the next chapter.\nI can not login my Facebook/Google account.\nThe game stuck at finishing the chapter."));
        questList.Add(SplitItem("Glitches reporting", "My progress is lost.\nMy MC has changed.\nMy costume is wrong. \nMy free keys does not count even they are less than 2\nMy game is frozen.\n I watch the AD videos but i did not get diamond."));
        questList.Add(SplitItem("About Books", "This book is awesome! I love it.\n When will this book update the new chapter? Will this book have a sequel?\nI think this part of the book is not making any senses."));
        questList.Add(SplitItem("Suggestions & Ideas ", "I want to give you some suggestions\n I have interest in writing a story.\n I have interest in writing a sequel."));
        questList.Add(SplitItem("About language", "I want to play this game in Spanish.\nI want to play this game in Portuguese.\nI want to play this game in Russian.\nI want to play this game in French."));
        questList.Add(SplitItem("Others", "How can i get free diamonds and keys?\nHow to join your social media accounts?\nHow can i read the book again?\nHow can i read this chapter again?"));

        dropdown.ClearOptions();
        subDropdown.ClearOptions();

        mQuestionType = -1;

        for (int i = 0;i<questList.Count;i++)
        {
            dropdown.options.Add(new Dropdown.OptionData(questList[i].typeQuestion));
        }

        ShowTypeProblem(0);
    }

    private FeedbackItemInfo SplitItem(string vTypeQuest,string vSubList)
    {
        if (string.IsNullOrEmpty(vTypeQuest)) return null;
        FeedbackItemInfo itemInfo = new FeedbackItemInfo();
        itemInfo.typeQuestion = vTypeQuest;
        itemInfo.subQuesList = vSubList.Split('\n');
        return itemInfo;
    }

    public void ShowTypeProblem(int vIndex)
    {
        dropdown.value = vIndex;
        ShowQuestion(vIndex);
    }

    private void ChangeTypeQuestionHandler(int vIndex)
    {
        ShowQuestion(vIndex);
    }

    /// <summary>
    /// 显示问题的细节内容
    /// </summary>
    private void ShowQuestion(int vIndex)
    {
        if(subDropdown.options.Count > 0)
            subDropdown.value = subDropdown.options.Count-1;

        mQuestionType = vIndex;

        subDropdown.ClearOptions();
        string[] subQuests = questList[vIndex].subQuesList;
        if (subQuests != null)
        {
            int len = subQuests.Length;
            for (int i = 0; i < len; i++)
            {
                subDropdown.options.Add(new Dropdown.OptionData(subQuests[i]));
            }
            subDropdown.value = 0;
            MessageInput.text = subQuests[0];
        }
    }

    public void ChangeSubQuestionHandler(int vIndex)
    {
        MessageInput.text = questList[mQuestionType].subQuesList[vIndex];
    }


    public override void OnClose()
    {
        base.OnClose();

        UIEventListener.RemoveOnClickListener(Mask, MaskOnclicke);
        UIEventListener.RemoveOnClickListener(SendButton, SendButtonOnclicke);

        MessageInput.onValueChanged.RemoveListener(MessageInputChange);

        dropdown.onValueChanged.RemoveListener(ChangeTypeQuestionHandler);
        subDropdown.onValueChanged.RemoveListener(ChangeSubQuestionHandler);


    }

    private void MaskOnclicke(PointerEventData data)
    {
        CUIManager.Instance.CloseForm(UIFormName.FAQFeedBack);
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
        if(mQuestionType == -1)
        {
            UITipsMgr.Instance.PopupTips("Please choose your questions.", false);
            return;
        }


        float value = dropdown.value;

        StandardSt = "";

        FeedbackItemInfo tempItemInfo = questList[mQuestionType];
        if(tempItemInfo != null)
        {
            StandardSt = "Standard questions:" + tempItemInfo.typeQuestion + "\nsub question:"+ tempItemInfo.subQuesList[subDropdown.value]+ "\n";
        }

        SendSt = "Message:" + MessageInput.text.ToString() + "\n";
      
        //UUID
        UUid = "UUid:" + GameHttpNet.Instance.UUID.ToString() + "\n";
        //型号
        deviceModelSt = "deviceModelSt:" + SystemInfo.deviceModel.ToString() + "\n";
        string platformSt = "platform:" + platform + "\n";
        string version = "version:" + SdkMgr.Instance.GameVersion() + "\n";

        //发送的内容是
        string SendCon = UUid + platformSt + deviceModelSt + StandardSt + SendSt + version;

        Debug.Log("SendCon："+ SendCon);
        if (MessageInput.text.ToString().Length >= 10)
        {
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.UserFeedback(mQuestionType+1, "", SendCon, UserFeedbackCallBack);

        }
        else
        {
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

public class FeedbackItemInfo
{
    public string typeQuestion;
    public string[] subQuesList;
}

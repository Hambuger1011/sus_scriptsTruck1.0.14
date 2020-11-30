using pb;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.UI;

#if !NOT_USE_LUA
using DialogDisplaySystem = BookReadingWrapper;
#endif

public class FeedbackForm : BaseUIForm
{  
    public Button FeedbackClose;
    public Button FeedbackSend;
    public InputField InputField;
    private string InputString;

    private void FeedbackCloseOnclick()
    {
        //Debug.Log("ddsd");
        CUIManager.Instance.CloseForm(UIFormName.FeedbackForm);
    }

    private void FeedbackSendOnclick()
    {
        InputString = InputField.text;

        if (InputString.Length < 10)
        {
            AudioManager.Instance.PlayTones(AudioTones.LoseFail);

            var Localization = GameDataMgr.Instance.table.GetLocalizationById(163);
            UITipsMgr.Instance.PopupTips(Localization, false);

            //UITipsMgr.Instance.PopupTips("Send more than 4 characters!", false);
            return;
        }
        if(DialogDisplaySystem.Instance.CurrentBookData != null)
        {
            InputString += " BookID:" + DialogDisplaySystem.Instance.CurrentBookData.BookID + " ChapterID:" + DialogDisplaySystem.Instance.CurrentBookData.ChapterID + " DialogID:" + DialogDisplaySystem.Instance.CurrentBookData.DialogueID;
        }
        //LOG.Info("你发送的内容是：" + InputString);

        // SendEmail(UnityPath, InputString);
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.UserFeedback(2,"",InputString, UserFeedbackCallBack);
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

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(170);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("send successful!", false);
                    CUIManager.Instance.CloseForm(UIFormName.FeedbackForm);
                    AudioManager.Instance.PlayTones(AudioTones.RewardWin);
                }
                else
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(171);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Error, send again!", false);
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                }
            }
            else
            {
                //UINetLoadingMgr.Instance.Close();

                var Localization = GameDataMgr.Instance.table.GetLocalizationById(171);
                UITipsMgr.Instance.PopupTips(Localization, false);

                //UITipsMgr.Instance.PopupTips("Error, send again!", false);
                AudioManager.Instance.PlayTones(AudioTones.LoseFail);
            }
        }, null);
    }

    public override void OnOpen()
    {
        base.OnOpen();

        FeedbackClose.onClick.AddListener(FeedbackCloseOnclick);
        FeedbackSend.onClick.AddListener(FeedbackSendOnclick);
    }

    public override void OnClose()
    {
        base.OnClose();

        FeedbackClose.onClick.RemoveListener(FeedbackCloseOnclick);
        FeedbackSend.onClick.RemoveListener(FeedbackSendOnclick);
    }
}

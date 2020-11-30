using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using System;
using UGUI;

/// <summary>
/// 聊天框
/// </summary>
public class ChatForm : BaseUIForm
{
    public Button CloseBtn;
    public Button SendBtn;
    public InputField InputField;
    public Text RecordMsgTxt;

    private string mRecrodMsg = "";
    private string mInputStr;

    private Dictionary<string, object> mTypeDic = new Dictionary<string, object>();
    private Dictionary<string, string> mSendDataDic = new Dictionary<string, string>();

    public override void OnOpen()
    {
        base.OnOpen();
        CloseBtn.onClick.AddListener(FormCloseOnclick);
        SendBtn.onClick.AddListener(SendOnclick);

        WebSocketManager.Instance.SetMsgCallBack(ReceiveMsgHandler);
    }


    public override void OnClose()
    {
        base.OnClose();

        CloseBtn.onClick.RemoveListener(FormCloseOnclick);
        SendBtn.onClick.RemoveListener(SendOnclick);
    }

    private void FormCloseOnclick()
    {
        CUIManager.Instance.CloseForm(UIFormName.ChatForm);
    }

    private void SendOnclick()
    {
        mInputStr = InputField.text;
        if (mInputStr.Length == 0) return;

        mSendDataDic.Clear();
        mSendDataDic.Add("touid", "247");
        mSendDataDic.Add("msg", mInputStr);

        mTypeDic.Clear();
        mTypeDic.Add("type", "chat_single");
        mTypeDic.Add("data", mSendDataDic);

        WebSocketManager.Instance.SendMsg(JsonHelper.ObjectToJson(mTypeDic));

        InputField.text = "";
        
    }

    private void ReceiveMsgHandler(string vMsg)
    {
        UTF8Encoding utf8 = new UTF8Encoding();
        vMsg = Regex.Unescape(vMsg);
        //vMsg =  vMsg));
        mRecrodMsg = vMsg+"\n"+ mRecrodMsg;
        RecordMsgTxt.text = mRecrodMsg;
    }

}

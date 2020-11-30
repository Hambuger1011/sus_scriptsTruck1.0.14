using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Framework;

/// <summary>
/// WebSocket
/// </summary>
public class WebSocketManager : CMonoSingleton<WebSocketManager>
{
    #region socketBase

    public WebSocket mSocket;

    public string SocketPath { get { return mSocketPath; } set { mSocketPath = value;} }
    private string mSocketPath;

    private Queue<string> mMsgQueue;
    private bool mIsBind = false;
    private Action<string> mMsgCallBack;

    protected override void Init()
    {
        base.Init();
        mMsgQueue = new Queue<string>();
    }

    public void OnStart()
    {
        ReConnect();
    }

    private void ReConnect()
    {
        StopCoroutine("SocketConnectAndReceive");

        SocketPath = "ws://192.168.0.142:8282?uid=" + UserDataManager.Instance.socketInfo.data.uid + "&phoneimei=" + GameHttpNet.Instance.UUID;
        mSocket = new WebSocket(new Uri(SocketPath));
        mIsBind = false;
        StartCoroutine("SocketConnectAndReceive");
    }

    IEnumerator SocketConnectAndReceive()
    {
        yield return StartCoroutine(mSocket.Connect());
        while (true)
        {
            //if (!mIsBind)
            //    ToBind();
            //if(mMsgQueue.Count>0)
            //    mSocket.SendString(mMsgQueue.Dequeue());

            string reply = mSocket.RecvString();
            if (!string.IsNullOrEmpty(reply))
            {
                LOG.Info("Received=================>>>>>>: " + reply);
                MsgRecive(reply);

                if (!mIsBind)
                    ToBind();
            }

            if (mSocket.error != null)
            {
                Debug.LogError("Error: " + mSocket.error);
                break;
            }
            yield return 0;
        }
    }

    //接收到信息返回
    private void MsgRecive(string vMsg)
    {
        if (mMsgCallBack != null)
            mMsgCallBack(vMsg);
    }

    //绑定用户信息
    private void ToBind()
    {
        mIsBind = true;
        HttpInfoReturn<SocketInfo> socketInfo = UserDataManager.Instance.socketInfo;
        if(socketInfo != null &&  socketInfo.data != null)
        {
            Dictionary<string, string> info = new Dictionary<string, string>();
            info.Add("type", "login");
            info.Add("uid", socketInfo.data.uid.ToString());
            SendMsg(JsonHelper.ObjectToJson(info));
        }
    }

    #endregion


    /// <summary>
    /// 设置聊天的回调
    /// </summary>
    /// <param name="vCallBack"></param>
    public void SetMsgCallBack(Action<string> vCallBack)
    {
        mMsgCallBack = vCallBack;
    }

    /// <summary>
    /// 发送信息
    /// </summary>
    /// <param name="vMsg"></param>
    public void SendMsg(string vMsg)
    {
        Debug.Log("------SendMsg----->" + vMsg);
        if (mSocket != null && mSocket.IsConnect)
        {
            mSocket.SendString(vMsg);
        }
        else
        {
            mMsgQueue.Enqueue(vMsg);
        }
    }

}

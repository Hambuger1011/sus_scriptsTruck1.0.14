using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// socket 返回的数据
/// </summary>

public class WebSocketData 
{
}

/// <summary>
/// 绑定当前用户信息
/// </summary>
public class WSBindInfo
{
    public int status;      //状态 1成功 0失败
    public int type;        //返回类型
}

/// <summary>
/// 接收到的聊天信息
/// </summary>
public class WSReceiveMsgInfo
{
    public int status;          //状态 1成功 0失败
    public int type;            //返回类型

    public int msg;             //聊天内容
    public string senderinfo;   //发送者信息
    public int uid;             //发送者ID
    public int sex;             //发送者性别
    public string nickname;     //发送者呢称
    public string face;         // 发送者头像
}

/// <summary>
/// 聊天记录列表
/// </summary>
public class WSChatRecorList
{
    public int status;          //状态 1成功 0失败
    public int type;            //返回类型

    public List<WSChatRecordItemInfo> recordList;   //记录列表
}

/// <summary>
/// 聊天记录项
/// </summary>
public class WSChatRecordItemInfo
{
    public int uid;         //发送者ID (当前用户)
    public int touid;       //接收方ID
    public string text;     //聊天内容
    public string ctime;    //聊天时间
    public int is_read;     //是否已读 1=已读
}

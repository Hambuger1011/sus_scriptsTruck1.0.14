using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UGUI;
using UnityEngine;

[XLua.LuaCallCSharp]
public class GamePointManager : Singleton<GamePointManager>
{
    /// <summary>
    /// 埋点
    /// </summary>
    /// <param name="event_name">事件名称</param>
    public void BuriedPoint(string event_name, string area_name = "", string area_value = "", string bookid = "",
        string dialogid = "", string option_value = "", EventHandler vCallBackHandler = null)
    {
        int LogStatus = 0;

        if (UserDataManager.Instance.versionInfo != null && UserDataManager.Instance.versionInfo.data != null)
        {
            if (UserDataManager.Instance.versionInfo.data.log_status==1)
            {
                LogStatus = 1;
            }
        }

        if (LogStatus == 0) { return; }


        LOG.Info("******BuriedPoint---->" + event_name);
        string apiName = "api_log";
        Dictionary<string, string> parameters = new Dictionary<string, string>();

        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);

        string userid = GameHttpNet.Instance.UUID;
        ; //设备ID，即PhoneImei

        string version = SdkMgr.Instance.GameVersion(); //app 版本号    

        string device = SystemInfo.deviceModel; //设备
        string system = SystemInfo.operatingSystem; //系统版本号
        string platform = "Editor"; //unity编辑器

#if !UNITY_EDITOR && UNITY_ANDROID
     platform = "Android";   //安卓
#endif

#if !UNITY_EDITOR && UNITY_IOS
     platform = "IOS";     //IOS
#endif

        parameters.Add("userid", userid); //设备ID，即PhoneImei
        parameters.Add("version", version); //app 版本号
        parameters.Add("device", device); //设备
        parameters.Add("system", system); //手机系统版本
        parameters.Add("platform", platform); //平台


        parameters.Add("event_name", event_name); //*事件名称
        if (area_name != "")
        {
            parameters.Add("area_name", area_name);
        } //区域名称, 非必须

        if (area_value != "")
        {
            parameters.Add("area_value", area_value);
        } //区域值, 非必须

        if (bookid != "")
        {
            parameters.Add("bookid", bookid);
        } //书本id, 非必须

        if (dialogid != "")
        {
            parameters.Add("dialogid", dialogid);
        } //对话id, 非必须

        if (option_value != "")
        {
            parameters.Add("option_value", option_value);
        } //对话id, 非必须



        var url = GameHttpNet.Instance.GameUrlHead + "/" + apiName;
#if ENABLE_DEBUG
        var sendSeq = GameHttpNet.Instance.getSendSeq();
        var postJson = JsonHelper.ObjectToJson(parameters, Newtonsoft.Json.Formatting.Indented);
        string sendInfo = string.Format("<color=cyan>[CS][send]POST:[{0}]{1}\n{2}</color>", sendSeq, url, postJson);
        LOG.Info(sendInfo);
#endif
        UniHttp.Instance.Post(url, parameters, null, 10000, 3, false);
    }

}

using System;
using System.Collections.Generic;
using ADTracking;
using Helper.Login;
using Newtonsoft.Json;
using UnityEngine;



[XLua.LuaCallCSharp]
public class AppsFlyerManager : Singleton<AppsFlyerManager>
{
    public string _IGGid = "";
    // //AF事件记录*安装时间记录Install
    // AppsFlyerManager.Instance.OnInstall();

    /// <summary>
    /// 安装事件 √
    /// </summary>
    public void OnInstall()
    {
        int isIntalled = PlayerPrefs.GetInt("isIntalled");
        if (isIntalled == 0 || isIntalled == 1)
        {
            PlayerPrefs.SetInt("isIntalled", 2);
        }
        else if (isIntalled == 2)
        {
            return;
        }

        LOG.Log("AppsFlyerManager  AD-Install");
        Dictionary<string, object> dic = new Dictionary<string, object>();
        Send("INSTALL", dic);
    }


    // 目的：统计的符合 iOS 系统要求的用户。
    // [IOS系统版本号] 的来源：系统对应的版本号，默认要求安卓最低 Android 6.0 以上， iOS 最低 10 以上，如果有的项目要求更高的系统，则根据实际情况提高事件的版本号。有特殊需求先于广告部商议。
    // 何时触发：一次性事件，在未触发事件时每次登录后对系统版本号进行检测，当满足系统版本号时触发上传。
    // 限制：不区分用户，比如A用户已经发过这个消息了，同设备又用B用户登录了，则不需要再次发送。
    // 特殊情况：游戏可能与运营或广告商议后需要统计多个不同版本，在提测时需要特别向 QA 同学说明，提供事件的名称、定义以及触发条件。
    // 例如，游戏 A 要求统计的版本为IOS 10.0(包含10.0)以上的用户，则事件名为IOS_10_AND_ABOVE，未触发事件时在每次登录后对系统版本号进行检测，若系统版本号低于IOS10.0 不触发事件，如系统版本号为IOS10.0及以上触发事件，之后登录后不再检测。
    // 事件名中的IOS系统版本号，只需填写大版本号，比如IOS 10.0 和IOS 10.1 事件名都为 IOS_10_AND_ABOVE。
    /// <summary>
    /// IOS_[IOS系统版本号]_IOS_ABOVE 事件 √
    /// </summary>
    public void IOSVersion()
    {
#if !UNITY_EDITOR && UNITY_IOS

        int _IOSVersion = PlayerPrefs.GetInt("_IOSVersion");
        if (_IOSVersion == 0 || _IOSVersion == 1)
        {

        }
        else if (_IOSVersion == 2)
        {
            return;
        }

        string iosVersion = SystemInfo.operatingSystem;
        //string iosVersion = "iOS 8.1";
        try
        {
            if (!string.IsNullOrEmpty(iosVersion))
            {
                string[] sArray = iosVersion.Split(new string[] { "OS" }, StringSplitOptions.RemoveEmptyEntries);

                if (sArray != null)
                {
                    string sysVersion = sArray[1].Substring(1);

                    int _System_index = sysVersion.IndexOf(".");   //字符串里获取 “.”  下标
                    if (_System_index > -1)
                    {
                        //[8.1]  [10.3] [12.2]    等等
                        string[] verArray = sysVersion.Split('.');

                        if (verArray != null && verArray.Length > 0)
                        {
                            //7.1.1  获取7
                            int HighVersion = int.Parse(verArray[0]);
                            if (HighVersion > 10)
                            {
                                LOG.Log("AppsFlyerManager  " + iosVersion);
                                PlayerPrefs.SetInt("_IOSVersion", 2);
                                Dictionary<string, object> dic = new Dictionary<string, object>();
                                dic.Add("userid", _IGGid);
                                Send("IOS_10_AND_ABOVE", dic);

                                 LOG.Log("iosVersion" + HighVersion);
                                //Debug.LogError("HighVersion " + HighVersion);
                            }

                            LOG.Log("iosVersion" + iosVersion);
                            //Debug.LogError("iosVersion" + iosVersion);
                        }
                    }
                    else
                    {
                        //[11]  [12] 
                        //Debug.LogError("andVersion.IndexOf" + _index + "     S" + CurSystemVersion + "S");
                        int HighVersion = int.Parse(sysVersion);
                        if (HighVersion > 10)
                        {
                            LOG.Log("AppsFlyerManager  " + iosVersion);
                            PlayerPrefs.SetInt("_IOSVersion", 2);
                            Dictionary<string, object> dic = new Dictionary<string, object>();
                            dic.Add("userid", _IGGid);
                            Send("IOS_10_AND_ABOVE", dic);
                            LOG.Log("iosVersion" + iosVersion);
                            LOG.Log("HighVersion" + HighVersion);
                            //Debug.LogError("HighVersion " + HighVersion);
                        }
                    }

                    LOG.Log("iosVersion" + iosVersion);
                    //Debug.LogError("iosVersion" + iosVersion);
                }
                else
                {
                    return;
                }
            }

        }
        catch (Exception e)
        {
            Debug.LogError("andVersion" + "ANDROID_[6]_AND_ABOVE" + e);
            throw;
        }
#endif
    }

    /// <summary>
    /// ANDROID_[Android系统版本号]_AND_ABOVE √
    /// </summary>
    public void AndroidVersion()
    {
#if !UNITY_EDITOR && UNITY_ANDROID

        int _AndroidVersion = PlayerPrefs.GetInt("_AndroidVersion");
        if (_AndroidVersion == 0 || _AndroidVersion == 1)
        {

        }
        else if (_AndroidVersion == 2)
        {
            return;
        }

        //Android OS 11 / API-30
        //Android OS 10 / API-29
        //Android OS 9.0 / API-28
        //Android OS 8.1 / API-27
        //Android OS 8.0 / API-26
        //Android OS 7.1.2 / API-25
        //Android OS 7.1.1 / API-25

        //string andVersion = SystemInfo.operatingSystem;
        string andVersion = "Android OS 7.1.1 / API-25";
        try
        {
            if (!string.IsNullOrEmpty(andVersion))
            {
                string[] sArray = andVersion.Split(new string[] { "API-" }, StringSplitOptions.RemoveEmptyEntries);

                if (sArray != null)
                {
                    string newStr = sArray[1].Substring(0, 2);
                    int APILevel = int.Parse(newStr);

                    if (APILevel >= 23)
                    {
                        LOG.Log("AppsFlyerManager  " + andVersion);
                        PlayerPrefs.SetInt("_AndroidVersion", 2);
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic.Add("userid", _IGGid);
                        Send("ANDROID_6_AND_ABOVE", dic);
                    }

                    LOG.Log("andVersion" + andVersion);
                    Debug.LogError("andVersion" + andVersion);
                }
                else
                {
                    return;
                }
            }

        }
        catch (Exception e)
        {
            Debug.LogError("andVersion" + "ANDROID_[6]_AND_ABOVE" + e);
            throw;
        }


     
#endif
    }


    /// <summary>
    /// IOS OS ABOVE事件 √
    /// </summary>
    public void IOS_OS()
    {
#if !UNITY_EDITOR && UNITY_IOS

        int _IOS_OS = PlayerPrefs.GetInt("_IOS_OS");
        if (_IOS_OS == 0 || _IOS_OS == 1)
        {

        }
        else if (_IOS_OS == 2)
        {
            return;
        }

        string iosVersion = SystemInfo.operatingSystem;
        //string iosVersion = "iOS 8.1";
        try
        {
            if (!string.IsNullOrEmpty(iosVersion))
            {
                string[] sArray = iosVersion.Split(new string[] { "OS" }, StringSplitOptions.RemoveEmptyEntries);

                if (sArray != null)
                {
                    string sysVersion = sArray[1].Substring(1);

                    int _System_index = sysVersion.IndexOf(".");   //字符串里获取 “.”  下标
                    if (_System_index > -1)
                    {
                        //[8.1]  [10.3] [12.2]    等等
                        string[] verArray = sysVersion.Split('.');

                        if (verArray != null && verArray.Length > 0)
                        {
                            //7.1.1  获取7
                            int HighVersion = int.Parse(verArray[0]);
                            if (HighVersion > 10)
                            {
                                this.IOS_OS_AND_ABOVE(verArray[0]);
                            }
                            PlayerPrefs.SetInt("_IOS_OS", 2);
                            LOG.Log("IOS_OS  HighVersion" + HighVersion);
                            // Debug.LogError("IOS_OS  HighVersion" + HighVersion);
                        }
                    }
                    else
                    {
                        //[11]  [12] 
                        //Debug.LogError("andVersion.IndexOf" + _index + "     S" + CurSystemVersion + "S");
                        int HighVersion = int.Parse(sysVersion);
                        if (HighVersion > 10)
                        {
                           this.IOS_OS_AND_ABOVE(sysVersion);
                        }
                        LOG.Log("IOS_OS  HighVersion" + HighVersion);
                        // Debug.LogError("IOS_OS  HighVersion" + HighVersion);
                        PlayerPrefs.SetInt("_IOS_OS", 2);
                    }
                }
                else
                {
                    return;
                }
            }

        }
        catch (Exception e)
        {
            Debug.LogError("iosVersion" + e);
            throw;
        }
#endif
    }

    /// <summary>
    /// Android OS ABOVE事件 √
    /// </summary>
    public void ANDROID_OS()
    {
#if !UNITY_EDITOR && UNITY_ANDROID


        int _ANDROID_OS = PlayerPrefs.GetInt("_ANDROID_OS");
        if (_ANDROID_OS == 0 || _ANDROID_OS == 1)
        {

        }
        else if (_ANDROID_OS == 2)
        {
            return;
        }

        try
        {
            //string andVersion = SystemInfo.operatingSystem;
            string andVersion = "Android OS 7.1.1 / API-25";
            int _index = andVersion.IndexOf("/");   //字符串里获取 “/”  下标
            if (_index > -1)
            {
                int len = _index - 12;
                string CurSystemVersion = andVersion.Substring(11, len);    //截取系统版本

                int _System_index = CurSystemVersion.IndexOf(".");   //字符串里获取 “.”  下标
                if (_System_index > -1)
                {
                    //[7.1.1]  [8.1]    等等
                    string[] verArray = CurSystemVersion.Split('.');

                    if (verArray != null && verArray.Length > 0)
                    {
                        //7.1.1  获取7
                        int HighVersion = int.Parse(verArray[0]);
                        if (HighVersion > 6)
                        {
                            this.ANDROID_OS_AND_ABOVE(verArray[0]);
                            LOG.Log("_ANDROID_OS  HighVersion" + HighVersion);
                            // Debug.LogError("_ANDROID_OS  HighVersion" + HighVersion);
                        }

                        PlayerPrefs.SetInt("_ANDROID_OS", 2);

                    }
                }
                else
                {
                    //[10]  [11] 
                    //Debug.LogError("andVersion.IndexOf" + _index + "     S" + CurSystemVersion + "S");
                    int HighVersion = int.Parse(CurSystemVersion);
                    if (HighVersion > 6)
                    {
                        this.ANDROID_OS_AND_ABOVE(CurSystemVersion);
                        LOG.Log("_ANDROID_OS  HighVersion" + HighVersion);
                        // Debug.LogError("_ANDROID_OS  HighVersion" + HighVersion);
                    }

                    PlayerPrefs.SetInt("_ANDROID_OS", 2);
                }
            }
            else
            {
                Debug.LogError("andVersion.IndexOf() is Null ！！");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("ANDROID_OS  _System_index  " + e);
            throw;
        }
#endif

    }

    /// <summary>
    /// IOS OS ABOVE事件 √
    /// </summary>
    public void IOS_OS_AND_ABOVE(string _version)
    {
        if (string.IsNullOrEmpty(_version)) { Debug.LogError("IOS_OS_AND_ABOVE _version is null"); return; }

        LOG.Log("AppsFlyerManager"+ "IOS_OS_" + _version + "_AND_ABOVE");
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("userid", _IGGid);
        string eventname = "IOS_OS_" + _version + "_AND_ABOVE";
        Send(eventname, dic);
    }


    /// <summary>
    /// ANDROID OS ABOVE事件 √
    /// </summary>
    public void ANDROID_OS_AND_ABOVE(string _version)
    {
        if (string.IsNullOrEmpty(_version)) { Debug.LogError("ANDROID_OS_AND_ABOVE _version is null"); return; }
        LOG.Log("AppsFlyerManager" + "ANDROID_OS_" + _version + "_AND_ABOVE");
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("userid", _IGGid);
        string eventname = "ANDROID_OS_" + _version + "_AND_ABOVE";
        Send(eventname, dic);
    }


    /// <summary>
    /// 第一次购买
    /// </summary>
    public void FIRST_BUY(int afcode,string price)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("userid", _IGGid);
        LOG.Log("AppsFlyerManager FIRST_BUY _IGGid:" + _IGGid);
        if (afcode > 0)
        {
            if (this.GetFirstStringByIndex(afcode) == false)
            {
                //第一次购买0.99  1.99  4.99  9.99   19.99  39.99  99.99 
                Send("FIRST_"+ price+"_PRUCHASE", dic);
                this.SaveFirstActionLog(afcode);
            }
        }
    }

    /// <summary>
    /// 用户请求广告加载 √
    /// </summary>
    public void ADS_REQUEST()
    {
        LOG.Log("AppsFlyerManager ADS_REQUEST");
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("userid", _IGGid);
        Send("ADS_REQUEST", dic);
    }

    /// <summary>
    /// 用户播放广告 √
    /// </summary>
    public void ADS_PLAY()
    {
        LOG.Log("AppsFlyerManager ADS_PLAY");
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("userid", _IGGid);
        Send("ADS_PLAY", dic);
    }


    /// <summary>
    /// 用户点击广告进行商店跳转
    /// </summary>
    public void ADS_CLICK()
    {
        LOG.Log("AppsFlyerManager ADS_CLICK");
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("userid", _IGGid);
        Send("ADS_CLICK", dic);
    }


    /// <summary>
    /// 首次充值  
    /// </summary>
    public void FIRST_PRUCHASE()
    {
        if (this.GetFirstStringByIndex(8) == false)
        {
            LOG.Log("AppsFlyerManager FIRST_PRUCHASE");
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("userid", _IGGid);
            Send("FIRST_PRUCHASE", dic);
            this.SaveFirstActionLog(8);
        }
    }


    /// <summary>
    /// 读完1个官方故事章节 √
    /// </summary>
    public void FINISH_OFFICIAL_CHAPTER()
    {
        LOG.Log("AppsFlyerManager FINISH_OFFICIAL_CHAPTER");
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("userid", _IGGid);
        Send("FINISH_OFFICIAL_CHAPTER", dic);
    }


    /// <summary>
    /// 签到1次 √
    /// </summary>
    public void SIGN()
    {
        LOG.Log("AppsFlyerManager SIGN");
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("userid", _IGGid);
        Send("SIGN", dic);
    }

    /// <summary>
    /// 第一次选择书本类型
    /// </summary>
    public void FIRST_SELECT_OFFICIAL_BOOK_TYPE()
    {
        if (this.GetFirstStringByIndex(12) == false)
        {
            LOG.Log("AppsFlyerManager FIRST_SELECT_OFFICIAL_BOOK_TYPE");
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("userid", _IGGid);
            Send("FIRST_SELECT_OFFICIAL_BOOK_TYPE", dic);
            this.SaveFirstActionLog(12);
        }
    }


    /// <summary>
    /// 第一次收藏书本
    /// </summary>
    public void FIRST_FAVORITE_OFFICIAL_BOOK()
    {
        if (this.GetFirstStringByIndex(12) == false)
        {
            LOG.Log("AppsFlyerManager FIRST_FAVORITE_OFFICIAL_BOOK");
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("userid", _IGGid);
            Send("FIRST_FAVORITE_OFFICIAL_BOOK", dic);
            this.SaveFirstActionLog(12);
        }
    }


    /// <summary>
    /// 读完1个UGC章节 √
    /// </summary>
    public void FINISH_UGC_CHAPTER()
    {
        LOG.Log("AppsFlyerManager FINISH_UGC_CHAPTER");
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("userid", _IGGid);
        Send("FINISH_UGC_CHAPTER", dic);
    }


    /// <summary>
    /// 选择1次钻石选项 √
    /// </summary>
    public void CHOOSE_DIAMOND_SELECTION()
    {
        LOG.Log("AppsFlyerManager CHOOSE_DIAMOND_SELECTION");
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("userid", _IGGid);
        Send("CHOOSE_DIAMOND_SELECTION", dic);
    }


    /// <summary>
    /// 读完1本官方故事的全部章节 √
    /// </summary>
    public void FINISH_OFFICIAL_BOOK()
    {
        LOG.Log("AppsFlyerManager FINISH_OFFICIAL_BOOK");
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("userid", _IGGid);
        Send("FINISH_OFFICIAL_BOOK", dic);
    }

    /// <summary>
    /// 完成1个UGC章节创作 √
    /// </summary>
    public void CREATE_UGC_CHAPTER()
    {
        LOG.Log("AppsFlyerManager CREATE_UGC_CHAPTER");
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("userid", _IGGid);
        Send("CREATE_UGC_CHAPTER", dic);
    }

    /// <summary>
    /// 完成1次分享 √
    /// </summary>
    public void SHARE()
    {
        LOG.Log("AppsFlyerManager SHARE");
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("userid", _IGGid);
        Send("SHARE", dic);
    }

    /// <summary>
    /// 领取一次7日签到奖励
    /// </summary>
    public void RECEIVE_7DAY_SIGN_REWARD()
    {
        LOG.Log("AppsFlyerManager RECEIVE_7DAY_SIGN_REWARD");
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("userid", _IGGid);
        Send("RECEIVE_7DAY_SIGN_REWARD", dic);
    }

    private AppsFlyerTracker AFtracker;
    protected override void Init()
    {
        AFtracker = new AppsFlyerTracker();
    }


    public void InitAFtracker(string customerInfo)
    {
        if (AFtracker != null)
        {
            AFtracker.Init(customerInfo);
        }
    }


    public void Send(string actionId, Dictionary<string, object> parameters)
    {
        try
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (AFtracker != null)
                {
                    AFtracker.Track(actionId, parameters);
                }
            }, null);
        }
        catch (System.Exception ex)
        {
            LOG.Error(actionId + "\n" + ex);
        }
    }

    private string FirstString = "";
    public void GetFirstActionLog()
    {
        GameHttpNet.Instance.GetFirstActionLog((arg) =>
        {
            string result = arg.ToString();
            Debug.LogError("----GetFirstActionLog---->" + result);
            if (result.Equals("error")) { return; }
            LoomUtil.QueueOnMainThread((param) =>
            {
                JsonObject jo = JsonHelper.JsonToJObject(result);
                if (jo != null)
                {
                    if (jo.code == 200)
                    {
                        Dictionary<string,object> _result = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                        string _datastr= JSONUtil.readString(_result, "data");

                        Dictionary<string, object> _data = JsonConvert.DeserializeObject<Dictionary<string, object>>(_datastr);
                        this.FirstString = JSONUtil.readString(_data, "action_list");
                    }
                }
                TalkingDataManager.Instance.OpenApp(EventEnum.GetBookShelfResultFail);
            }, null);
        });
    }

    public bool GetFirstStringByIndex(int index)
    {
        if (!string.IsNullOrEmpty(this.FirstString))
        {
            string[] strArr = this.FirstString.Split(',');
            if (strArr!=null && strArr.Length > 0)
            {
                for (int i = 0; i < strArr.Length; i++)
                {
                    if (int.Parse(strArr[i]) == index)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }


    public void SaveFirstActionLog(int _index)
    {
        GameHttpNet.Instance.SaveFirstActionLog(_index,(arg) =>
        {
            string result = arg.ToString();
            Debug.LogError("----SaveFirstActionLog---->" + result);
            if (result.Equals("error")) { return; }
            LoomUtil.QueueOnMainThread((param) =>
            {
                JsonObject jo = JsonHelper.JsonToJObject(result);
                if (jo != null)
                {
                    if (jo.code == 200)
                    {
                        Dictionary<string, object> _result = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                        string _datastr = JSONUtil.readString(_result, "data");

                        Dictionary<string, object> _data = JsonConvert.DeserializeObject<Dictionary<string, object>>(_datastr);
                        this.FirstString = JSONUtil.readString(_data, "action_list");
                    }
                }
                TalkingDataManager.Instance.OpenApp(EventEnum.GetBookShelfResultFail);
            }, null);
        });
    }



}

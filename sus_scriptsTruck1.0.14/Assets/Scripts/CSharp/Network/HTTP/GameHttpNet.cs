using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Framework;
using System.Diagnostics;
using UGUI;
using AB;
using WebSocketSharp;

/// <summary>
/// 游戏HTTP 服务器，接口管理类
/// </summary>
public class GameHttpNet : CSingleton<GameHttpNet>
{
    public static readonly string UUID_LOCAL_FLAG = "OnyxGames_UUID";
    public static readonly string TOKEN_GUEST_LOCAL_FLAG = "OnyxGames_GUEST_TOKEN";
    public static readonly string TOKEN_FACEBOOK_TOKEN_LOCAL_FLAG = "OnyxGames_FACEBOOK_TOKEN";
    public static readonly string TOKEN_GOOGLE_TOKEN_LOCAL_FLAG = "OnyxGames_GOOGLE_TOKEN";
    public static readonly string TOKEN_HUAWEI_TOKEN_LOCAL_FLAG = "OnyxGames_HUAWEI_TOKEN";
    public static readonly string LANG_LOCAL_FLAG = "OnyxGames_Lang";

    private string m_UUID = "5aded6e602283179eef24b98b668e1f9";
    private string m_TOKEN = "";
    private string m_Lang = "";
    private string PayFinishKey = "onyxKVeZerip93fNG2A0t0qZQAI8";
    private int system_type = 1;

#if ENABLE_DEBUG
    private int mAuditsStatus = 0; //当前的审核状态 1=审核通过 0未通过
#else
    private int mAuditsStatus = 1;        //当前的审核状态 1=审核通过 0未通过
#endif

    public string FaceBookTestID = "FBTest234891230481dshfasdkf001";
    public bool showErrorTips = true;

    private string _assetBundleAddress = "";
    private string _serverAddress = "";


    public string GetBookABUrl()
    {
        string _abUri = null;

        switch (GameDataMgr.Instance.ResourceType)
        {
#if ENABLE_DEBUG
            case 0:
                _abUri = GetResourcesUrl();
                break;
#endif
            default:
                _abUri = GetResourcesUrl();
                _abUri = string.Format("{0}book/{1}/", _abUri, GameUtility.Platform);
                break;
        }

        return _abUri;
    }

    public string GetABUrlCommon()
    {
        string _abUri = null;

        switch (GameDataMgr.Instance.ResourceType)
        {
#if ENABLE_DEBUG
            case 0:
                _abUri = GetResourcesUrl();
                break;
#endif
            default:
                _abUri = GetResourcesUrl();
                string[] versionArr = UserDataManager.Instance.ResVersion.Split('@');
                _abUri = string.Format("{0}common/{1}/{2}/{3}/", _abUri, GameUtility.Platform, versionArr[0],versionArr[1]);
                break;
        }

        return _abUri;
    }
    public string GetABUrlDatatable()
    {
        string _abUri = null;

        switch (GameDataMgr.Instance.ResourceType)
        {
#if ENABLE_DEBUG
            case 0:
                _abUri = GetResourcesUrl();
                break;
#endif
            default:
                _abUri = GetResourcesUrl();
                string[] versionArr = UserDataManager.Instance.DataTableVersion.Split('@');
                _abUri = string.Format("{0}datatable/{1}/{2}/{3}/", _abUri, GameUtility.Platform, versionArr[0], versionArr[1]);
                break;
        }

        return _abUri;
    }


    public string GetResourcesUrl()
    {
        string _abUri = null;

        switch (GameDataMgr.Instance.ResourceType)
        {
#if ENABLE_DEBUG
            case 0:
#if UNITY_ANDROID && !UNITY_EDITOR
                            _abUri = AbUtility.abReadonlyPath;
#else
                _abUri = "file://" + AbUtility.abReadonlyPath;
#endif
                break;
            case 1: //开发
                _abUri = "http://dev.sus.com/resources/";
                break;
            case 2: //tecent开发
                _abUri = "http://193.112.66.252:8082/resources/";
                break;
#endif
            case 3: //测试
                _abUri = "http://d2zbbkt89do1w1.cloudfront.net:8080/resources/";
                break;
            case 4: //正式
                _abUri = "http://d3da55fv9mg213.cloudfront.net:8080/resources/";
                break;
            case 5:
                _abUri = string.Format("{0}/resources/", PlayerPrefs.GetString("mCustomRes"));
                break;
        }
#if ENABLE_DEBUG
        if (GameDataMgr.Instance.UserLocalAddress)
        {
            return _abUri;
        }
#endif
        if (!string.IsNullOrEmpty(AssetBundleAddress))
            _abUri = string.Format("{0}/resources/", AssetBundleAddress);
        return _abUri;
    }


    private string mGameUrlHead = "http://193.112.66.252:8080";

    public string GameUrlHead
    {
        get
        {
            mGameUrlHead = "http://193.112.66.252:8080";
#if ENABLE_DEBUG
            switch (GameDataMgr.Instance.ServiceType)
            {
                case 1: //开发
                    mGameUrlHead = "http://dev.sus.com";
                    break;
                case 2: //tencent开发
                    mGameUrlHead = "http://193.112.66.252:8082";
                    break;
                case 3: //测试
                    mGameUrlHead = "http://193.112.66.252:8080";
                    break;
                case 4: //正式
                    mGameUrlHead = "https://sus-game.igg.com/";
                    break;
                case 5:
                    mGameUrlHead = "http://192.168.0.10";
                    break;
                case 6:
                    mGameUrlHead = PlayerPrefs.GetString("mCustomSvr");
                    break;
            }
#else
            if (AuditStatus == 1)
                mGameUrlHead = "https://sus-game.igg.com/";
            else
                mGameUrlHead = "http://193.112.66.252:8082";
            //UnityEngine.Debug.LogError(AuditStatus + "  " + mGameUrlHead);
#endif
            if (!string.IsNullOrEmpty(ServerAddress))
                mGameUrlHead = ServerAddress;
            return mGameUrlHead;
        }
    }

    protected override void Init()
    {
        base.Init();
        //UUID = TalkingDataManager.Instance.GetDeviceId();
#if !UNITY_EDITOR
        UUID = TalkingDataManager.Instance.GetDeviceId();
        //UUID = AndroidHelper.CallStaticMethod<string>("com.game.gamelib.utils.AndroidUtils", "getDeviceUuid");
        LOG.Info("----Phone UUID----->" + UUID +" talkingData--deviceId-->"+TalkingDataManager.Instance.GetDeviceId());
#endif
    }
    
    /// <summary>
    /// 手机唯一码
    /// </summary>
    public string UUID
    {
        get { return m_UUID; }
        set { m_UUID = value; }
    }

    /// <summary>
    /// 手机系统类型
    /// </summary>
    public int SYSTEMTYPE
    {
        get { return system_type; }
        set { system_type = value; }
    }

    /// <summary>
    /// 验证标示
    /// </summary>
    public string TOKEN
    {
        get
        {
            if (UserDataManager.Instance.LoginInfo.LastLoginChannel == 0 ||
                UserDataManager.Instance.LoginInfo.LastLoginChannel == 4)
            {
                //游客的token
                m_TOKEN = PlayerPrefs.GetString(GameHttpNet.TOKEN_GUEST_LOCAL_FLAG);
            }
            else if (UserDataManager.Instance.LoginInfo.LastLoginChannel == 1)
            {
                //facebook 的token
                m_TOKEN = PlayerPrefs.GetString(GameHttpNet.TOKEN_FACEBOOK_TOKEN_LOCAL_FLAG);
            }
            else if (UserDataManager.Instance.LoginInfo.LastLoginChannel == 2)
            {
                //google 的token
                m_TOKEN = PlayerPrefs.GetString(GameHttpNet.TOKEN_GOOGLE_TOKEN_LOCAL_FLAG);
            }
            else if (UserDataManager.Instance.LoginInfo.LastLoginChannel == 3)
            {
                //华为的token
                m_TOKEN = PlayerPrefs.GetString(GameHttpNet.TOKEN_HUAWEI_TOKEN_LOCAL_FLAG);
            }

            return m_TOKEN;
        }
        set
        {
            //token 不过期的时候是不会赋予新的token 的，token过期后会在返回280处先把token清空

            if ((UserDataManager.Instance.LoginInfo.LastLoginChannel == 0 ||
                 UserDataManager.Instance.LoginInfo.LastLoginChannel == 4))
            {
                //游客的token             
                PlayerPrefs.SetString(GameHttpNet.TOKEN_GUEST_LOCAL_FLAG, value);
            }
            else if (UserDataManager.Instance.LoginInfo.LastLoginChannel == 1)
            {
                //facebook 的token            
                PlayerPrefs.SetString(GameHttpNet.TOKEN_FACEBOOK_TOKEN_LOCAL_FLAG, value);
            }
            else if (UserDataManager.Instance.LoginInfo.LastLoginChannel == 2)
            {
                //google 的token             
                PlayerPrefs.SetString(GameHttpNet.TOKEN_GOOGLE_TOKEN_LOCAL_FLAG, value);
            }
            else if (UserDataManager.Instance.LoginInfo.LastLoginChannel == 3)
            {
                //华为的token             
                PlayerPrefs.SetString(GameHttpNet.TOKEN_HUAWEI_TOKEN_LOCAL_FLAG, value);
            }
        }
    }

    /// <summary>
    /// 国家标识，最终会决定语言标识
    /// </summary>
    public string LANG
    {
        get
        {
            if (string.IsNullOrEmpty(m_Lang))
                m_Lang = PlayerPrefs.GetString(GameHttpNet.LANG_LOCAL_FLAG);

            if (string.IsNullOrEmpty(m_Lang))
                m_Lang = "EN";

            //LOG.Error("=======lang=======>>>" + m_Lang);

            return m_Lang;
        }
        set
        {
            m_Lang = value;
            PlayerPrefs.SetString(LANG_LOCAL_FLAG, m_Lang);
        }
    }

    /// <summary>
    /// 审核状态
    /// 1=审核通过 0未通过
    /// </summary>
    public int AuditStatus
    {
        get { return mAuditsStatus; }
        set { mAuditsStatus = value; }
    }

    int _sendSeq = 0;

    public int SendSeq
    {
        get { return _sendSeq; }
        set { _sendSeq = value; }
    }
    
    public int getSendSeq()
    {
        _sendSeq = _sendSeq + 1;
        return _sendSeq;
    }

    public string AssetBundleAddress
    {
        get { return _assetBundleAddress; }
        set { _assetBundleAddress = value; }
    }

    public string ServerAddress
    {
        get { return _serverAddress; }
        set { _serverAddress = value; }
    }

    /// <summary>
    ///  http post
    /// </summary>
    /// <param name="url"></param>
    /// <param name="param"></param>
    /// <param name="callback"></param>
    /// <param name="timeoutMS"></param>
    /// <param name="tryCount"></param>
    /// <param name="required">如果true表示这协办必须发送成功，失败后需要重连</param>
    /// <param name="isFullUrl">apiName 是否是完整的http url</param>
    private void Post(string apiName, Dictionary<string, string> param, Action<long, string> callback,
        int timeoutMS = 20, int tryCount = 3, bool required = true, bool isFullUrl = false, bool isShowLoadUI = true)
    {
        var url = apiName;
        if (!isFullUrl)
        {
            url = this.GameUrlHead + "/" + apiName;
        }
#if ENABLE_DEBUG
        var sendSeq = getSendSeq();
        var postJson = JsonHelper.ObjectToJson(param, Newtonsoft.Json.Formatting.Indented);
        string sendInfo = string.Format("<color=#009000>[CS][send]POST:[{0}]{1}\n{2}</color>", sendSeq, url, postJson);
        LOG.Info(sendInfo);

        Stopwatch sw = new Stopwatch();
        sw.Start();
#endif
        UniHttp.Instance.Post(url, param, (HttpObject obj, long responseCode, string result) =>
        {
            if (obj.isMask)
            {
                obj.isMask = false;
                UINetLoadingMgr.Instance.Close();
            }
#if ENABLE_DEBUG
            sw.Stop();
            LOG.Info(string.Format("<color=#ee00ee>[CS][recv]POST:耗时{0:F3}s,[{1}]{2},result:\n{3}</color>",
                sw.ElapsedMilliseconds * 0.001, sendSeq, url, result));
#endif

            if (responseCode != 200)
            {
                if (required && showErrorTips)
                {
                    if (responseCode != 0)
                    {
                        param["result"] = result;
                        //TalkingDataManager.Instance.RecordProtocolError(apiName, param);
                    }

                    UIAlertMgr.Instance.ShowNetworkAlert(responseCode, () =>
                    {
#if ENABLE_DEBUG
                        LOG.Info(sendInfo);
#endif
                        obj.tryCount = 0;
                        UniHttp.Instance.DoWebPost(obj);
                    });
                }

                return;
            }

            //如果result 为Null或""
            if (string.IsNullOrEmpty(result))
            {
                UIAlertMgr.Instance.Show("Tips", "Request return data error! Failed to unpack result![Post]", AlertType.Sure,
                    (value) =>
                    {
                        this.Post(apiName, param, callback, timeoutMS, tryCount, required, isFullUrl, isShowLoadUI);
                    });
                UnityEngine.Debug.LogError("请求返回数据result解析错误[Post] result：" + result + "apiName:" + apiName);
                return;
            }

            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo.code == 280)
            {
                UIAlertMgr.Instance.Show("TIPS", jo.msg, AlertType.Sure, (isOK) =>
                {
                    CUIManager.Instance.OpenForm(UIFormName.LoginForm);
                    CUIManager.Instance.GetForm<LoginForm>(UIFormName.LoginForm).IsTimeOutOpenFanc();
                });
            }
            else if (jo.code == 281)
            {
                UIAlertMgr.Instance.Show("TIPS", jo.msg, AlertType.Sure,
                    (isOK) => { IGGSDKManager.Instance.AutoLogin(); });
            }
            else if (jo.code == 282)
            {
                UIAlertMgr.Instance.Show("TIPS", jo.msg, AlertType.Sure, (isOK) =>
                {
                    CUIManager.Instance.OpenForm(UIFormName.LoginForm);
                    CUIManager.Instance.GetForm<LoginForm>(UIFormName.LoginForm).IsTimeOutOpenFanc();
                });
            }
            else if (jo.code == 910)
            {
                UIAlertMgr.Instance.Show("TIPS", jo.msg, AlertType.Sure, (isOK) =>
                {
                    CUIManager.Instance.OpenForm(UIFormName.LoginForm);
                    CUIManager.Instance.GetForm<LoginForm>(UIFormName.LoginForm).IsTimeOutOpenFanc();
                });
            }
            else if (jo.code == 277)
            {
                UIAlertMgr.Instance.Show("Tips", jo.msg, AlertType.Sure,
                    (value) => { PluginTools.Instance.KillRunningProcess(); });
            }
            else if (jo.code == 377)
            {
                ShowMovePanel();
            }
            else if (jo.code == 378)
            {
                ShowMoveWaitPanel(apiName, param, callback, timeoutMS, tryCount, required, isFullUrl, isShowLoadUI);
            }
            else if (callback != null)
            {
                callback(responseCode, result);
            }
        }, timeoutMS, tryCount, isShowLoadUI);
    }

    private Action<long, string> moveWaitCallBack;

    private void ShowMoveWaitPanel(string apiName, Dictionary<string, string> param, Action<long, string> callback,
        int timeoutMS = 20, int tryCount = 3, bool required = true, bool isFullUrl = false, bool isShowLoadUI = true)
    {
        param.Add("is_move", "1");
        moveWaitCallBack = callback;
        XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:Open(logic.uiid.MoveWait)");
        Post(apiName, param, moveWait, 9999, tryCount, required, isFullUrl, false);
    }

    private void moveWait(long l, string s)
    {
        XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:Close(logic.uiid.MoveWait)");
        moveWaitCallBack(l, s);
    }

    private void Get(string apiName, Action<long, string> callback, int timeoutMS = 20, int tryCount = 3,
        bool required = true, bool isFullUrl = false, bool isShowLoadUI = true)
    {
        var url = apiName;
        if (!isFullUrl)
        {
            url = this.GameUrlHead + "/" + apiName;
        }
#if ENABLE_DEBUG


        var sendSeq = getSendSeq();
        string sendInfo = string.Format("<color=#009000>[CS][send]GET:[{0}]{1} </color>", sendSeq, url);
        LOG.Info(sendInfo);

        Stopwatch sw = new Stopwatch();
        sw.Start();
#endif

        UniHttp.Instance.Get(url, (HttpObject obj, long responseCode, string result) =>
        {
            if (obj.isMask)
            {
                obj.isMask = false;
                UINetLoadingMgr.Instance.Close();
            }
#if ENABLE_DEBUG
            sw.Stop();
            LOG.Info(string.Format("<color=#ee00ee>[CS][recv]Get:耗时{0:F3}s,[{1}]{2},result:\n{3}</color>",
                sw.ElapsedMilliseconds * 0.001, sendSeq, url, result));
#endif

            if (responseCode != 200)
            {
                if (required && showErrorTips)
                {
                    if (responseCode != 0)
                    {
                        var dict = new Dictionary<string, string>();
                        dict["result"] = result;
                        //TalkingDataManager.Instance.RecordProtocolError(apiName, dict);
                    }

                    UIAlertMgr.Instance.ShowNetworkAlert(responseCode, () =>
                    {
#if ENABLE_DEBUG
                        LOG.Info(sendInfo);
#endif
                        obj.tryCount = 0;
                        UniHttp.Instance.DoWebGet(obj);
                    });
                }

                return;
            }

            //如果result 为Null或""
            if (string.IsNullOrEmpty(result))
            {
                UIAlertMgr.Instance.Show("Tips", "Request return data error! Failed to unpack result![Get]", AlertType.Sure,
                    (value) =>
                    {
                        this.Get(apiName, callback, timeoutMS, tryCount, required, isFullUrl, isShowLoadUI);
                    });
                UnityEngine.Debug.LogError("请求返回数据result解析错误[Get] result：" + result + "apiName:" + apiName);
                return;
            }


            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo.code == 280)
            {
                UIAlertMgr.Instance.Show("TIPS", jo.msg, AlertType.Sure, (isOK) =>
                {
                    CUIManager.Instance.OpenForm(UIFormName.LoginForm);
                    CUIManager.Instance.GetForm<LoginForm>(UIFormName.LoginForm).IsTimeOutOpenFanc();
                });
            }
            else if (jo.code == 281)
            {
                UIAlertMgr.Instance.Show("TIPS", jo.msg, AlertType.Sure,
                    (isOK) => { IGGSDKManager.Instance.AutoLogin(); });
            }
            else if (jo.code == 282)
            {
                UIAlertMgr.Instance.Show("TIPS", jo.msg, AlertType.Sure, (isOK) =>
                {
                    CUIManager.Instance.OpenForm(UIFormName.LoginForm);
                    CUIManager.Instance.GetForm<LoginForm>(UIFormName.LoginForm).IsTimeOutOpenFanc();
                });
            }
            else if (jo.code == 910)
            {
                UIAlertMgr.Instance.Show("TIPS", jo.msg, AlertType.Sure, (isOK) =>
                {
                    CUIManager.Instance.OpenForm(UIFormName.LoginForm);
                    CUIManager.Instance.GetForm<LoginForm>(UIFormName.LoginForm).IsTimeOutOpenFanc();
                });
            }
            else if (jo.code == 277)
            {
                UIAlertMgr.Instance.Show("Tips", jo.msg, AlertType.Sure,
                    (value) => { PluginTools.Instance.KillRunningProcess(); });
            }
            else if (jo.code == 377)
            {
                ShowMovePanel();
            }
            else if (callback != null)
            {
                callback(responseCode, result);
            }
        }, timeoutMS, tryCount, isShowLoadUI);
    }

    private void TouristLoginCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----TouristLoginGetToken---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            if (jo.code == 200)
            {
                TalkingDataManager.Instance.LoginAccount(EventEnum.LoginTouristResultSucc);
                GamePointManager.Instance.BuriedPoint(EventEnum.LoginOk, "", "", "", "", "0");
                HttpInfoReturn<TouristLoginInfo> TouristLoginInfo =
                    JsonHelper.JsonToObject<HttpInfoReturn<TouristLoginInfo>>(result);
                GameHttpNet.Instance.TOKEN = TouristLoginInfo.data.token;
                AccountExpired.Instance.DoAction();
            }
            else
            {
                TalkingDataManager.Instance.LoginAccount(EventEnum.LoginTouristResultFail);
            }
        }
    }


    public void CleanToken()
    {
    }

    /// <summary>
    /// 获取版本信息-新
    /// </summary>
    public void GetVersion(Action<long, string> vCallBackHandler)
    {
        int type = 1;
#if UNITY_IOS
        type = 2;
#endif
        string url = "api_getVersion?type=" + type + "&version=" + SdkMgr.Instance.GameVersion();

        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getVersion");
                TalkingDataManager.Instance.RecordProtocolError("api_getVersion", dic);
                return;
            }

            vCallBackHandler(responseCode, result);
        });
    }

    /// <summary>
    /// 获取用户迁移code信息
    /// </summary>
    /// <param name="vToken"></param>
    /// <param name="vCallBackHandler"></param>
    public void GetMoveCode(EventHandler vCallBackHandler)
    {
        string url = "api_getMoveCode";
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getMoveCode");
                TalkingDataManager.Instance.RecordProtocolError("api_getMoveCode", dic);
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 设置用户心跳的协议
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void SetUserHeartBeat(Action<long, string> vCallBackHandler)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        this.Post("api_setUserHeartBeat", parameters, vCallBackHandler, required: false);
    }


    /// <summary>
    /// 获取用户的基本信息
    /// </summary>
    /// <param name="vToken"></param>
    public void GetUserInfo(EventHandler vCallBackHandler)
    {
        int flag = 1;
#if UNITY_ANDROID
#if CHANNEL_HUAWEI
        flag = 3;
#else
        flag = 1;
#endif
#else
                flag = 2;
#endif
        string url = "api_getUserInfo?jpushid=" + UserDataManager.Instance.UserData.JPushId + "&system_type=" + flag;

        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getUserInfo");
                dic.Add("jpushid", UserDataManager.Instance.UserData.JPushId);
                TalkingDataManager.Instance.RecordProtocolError("api_getUserInfo", dic);
            }

            vCallBackHandler(result);
        });
    }


    /// <summary>
    /// 第三方登陆
    /// </summary>
    /// <param name="vType">0:google 1facebook</param>
    /// <param name="vEmail"></param>
    /// <param name="vName">用户名</param>
    /// <param name="vFaceUrl">头像地址</param>
    /// <param name="vId">google ID  or facebook ID</param>
    /// <param name="vToken"></param>
    /// <param name="vChannel">运营渠道；iso/android</param>
    /// <param name="vCallBackHandler"></param>
    public void LoginByThirdParty(int vType, string vEmail, string vName, string vFaceUrl, string vId, string vToken,
        string vChannel, Action<string,EnumReLogin> vCallBackHandler, int vIsSwitch = 1,EnumReLogin loginType=EnumReLogin.None)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("system_type", SYSTEMTYPE.ToString());
        parameters.Add("iggid", vId);
        parameters.Add("access_token", vToken);
        parameters.Add("is_switch", vIsSwitch.ToString());

        if (UserDataManager.Instance.InviteCode.Length > 0)
        {
            parameters.Add("invite_code", UserDataManager.Instance.InviteCode);
            UserDataManager.Instance.InviteCode = "";
        }
        this.Post("api_login", parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            TOKEN = JsonHelper.JsonToObject<HttpInfoReturn<ThirdPartyReturnInfo>>(result).data.token;
            vCallBackHandler(result,loginType);
        }, 10);
    }

    /// <summary>
    /// 登入验证
    /// </summary>
    /// <param name="vType"> 1华为 </param>
    /// <param name="vPlayerId"></param>
    /// <param name="vPlayerLevel"></param>
    /// <param name="vPlayerSign">签名</param>
    /// <param name="ts">时间戳</param>
    /// <param name="vCallBackHandler"></param>
    public void CheckLoginInfo(int vType, string vPlayerId, int vPlayerLevel, string vPlayerSign, string ts,
        EventHandler vCallBackHandler)
    {
        //LOG.Info("---CheckLoginInfo--type-->" + vType + "---vPlayerId-->" + vPlayerId + "---vPlayerLevel-->" + vPlayerLevel + "---vPlayerSign-->" + vPlayerSign + "---ts-->" + ts);

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("type", vType.ToString());

        Dictionary<string, string> otherParameters = new Dictionary<string, string>();

        otherParameters.Add("playerId", vPlayerId.ToString());
        otherParameters.Add("playerLevel", vPlayerLevel.ToString());
        string tokenResult = vPlayerSign;
        otherParameters.Add("playerSSign", tokenResult);
        otherParameters.Add("ts", ts.ToString());
        string otherParamerStr = JsonHelper.ObjectToJson(otherParameters);
        parameters.Add("parameter", otherParamerStr);
        this.Post("api_checkLoginInfo", parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }


    /// <summary>
    /// 进入主页 获取自己书架的书本
    /// </summary>
    /// <param name="vToken"></param>
    /// <param name="vCallBackHandler"></param>
    public void GetSelfBookInfo(EventHandler vCallBackHandler)
    {
        string url = "api_getIndexData";
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getIndexData");
                dic.Add("UUID", UUID);
                dic.Add("TOKEN", TOKEN);
                dic.Add("jpushid", UserDataManager.Instance.UserData.JPushId);
                TalkingDataManager.Instance.RecordProtocolError("api_getIndexData", dic);
            }

            vCallBackHandler(result);
        });
    }


    /// <summary>
    /// 获取书架的书本数据 ,获取所有书本信息
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void GetBookVersion(Action<long, string> vCallBackHandler)
    {
        string url = "api_getAllBook";
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getAllBook");
                dic.Add("UUID", UUID);
                dic.Add("TOKEN", TOKEN);
                TalkingDataManager.Instance.RecordProtocolError("api_getAllBook", dic);
            }
            vCallBackHandler(responseCode, result);
        });


        //string url = "api_getallbook";
        //Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);

        //this.Post(url, parameters, (responseCode, result) =>
        //{
        //    if (responseCode != 200)
        //    {
        //        TalkingDataManager.Instance.RecordProtocolError("api_getallbook", parameters);
        //        return;
        //    }
        //    vCallBackHandler(result);
        //});
    }


    public void GetAllBook2(Action<long, string> vCallBackHandler)
    {
        string url = "api_getAllBook2";
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getAllBook2");
                dic.Add("UUID", UUID);
                dic.Add("TOKEN", TOKEN);
            }
           // vCallBackHandler(responseCode, result);
        });
    }



    /// <summary>
    /// 获取书本章节
    /// </summary>
    /// <param name="vBookId">书本ID</param>
    /// <param name="vChapterId">章节ID</param>
    public void GetBookChapterInfo(int vBookId, int vChapterId, EventHandler vCallBackHandler)
    {
        string url = "api_getbookchapter?bookid=" + vBookId + "&chapterid=" + vChapterId;
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getbookchapter");
                dic.Add("vBookId", vBookId.ToString());
                dic.Add("vChapterId", vChapterId.ToString());
                TalkingDataManager.Instance.RecordProtocolError("api_getbookchapter", dic);
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 发送游戏进度给服务端
    /// </summary>
    /// <param name="vBookId">书本Id</param>
    /// <param name="vChapterId">章节ID</param>
    /// <param name="vDialogId">DialogId</param>
    /// <param name="vCallBackHandler"></param>
    /// <param name="vOption"> 选项id</param>
    /// <param name="role_name">角色名字</param>
    /// <param name="npc_id">npc 的id</param>
    /// <param name="npc_name">npc 的名字</param>
    /// <param name="npc_sex">npc 的性别</param>
    public void SendPlayerProgress(int vBookId, int vChapterId, int vDialogId, EventHandler vCallBackHandler,
        int vOption = 0, string role_name = "", int npc_id = 0, string npc_name = "", int npc_sex = 0)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("bookid", vBookId.ToString());
        parameters.Add("chapterid", vChapterId.ToString());
        parameters.Add("dialogid", vDialogId.ToString());
        if (vOption > 0)
        {
            parameters.Add("option", vOption.ToString());
        }
        //parameters.Add("roidid", vRoldId.ToString());
        //parameters.Add("skinid", vSkinId.ToString());
        //parameters.Add("garmentid", vClothId.ToString());

        if (!string.IsNullOrEmpty(role_name))
            parameters.Add("role_name", role_name);

        //parameters.Add("phonesceneid", vInPhoneMode.ToString());
        //parameters.Add("phoneroleid", vPhoneRoleId.ToString());

        if (npc_id != 0)
            parameters.Add("npc_id", npc_id.ToString());

        if (!string.IsNullOrEmpty(npc_name))
            parameters.Add("npc_name", npc_name.ToString());

        if (npc_sex != 0)
            parameters.Add("npc_sex", npc_sex.ToString());

        bool is_use_prop = UserDataManager.Instance.is_use_prop;//是否使用道具: 1.使用道具 0.不使用（非必传，默认不使用）
        int use_prop = is_use_prop ? 1 : 0;
        if (use_prop == 1)
        {
            if (UserDataManager.Instance.propInfoItem == null) LOG.Warn("[send]使用道具信息异常"); 
            else parameters.Add("discount", UserDataManager.Instance.propInfoItem.discount.ToString());
        }
        parameters.Add("is_use_prop", use_prop.ToString());

        this.Post("api_saveStep", parameters, (responseCode, result) =>
        {
            ProgressCallBackHandler(result);
            vCallBackHandler(result);

        });
    }

    private void ProgressCallBackHandler(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----ProgressCallBackHandler---->" + result);
        UserDataManager.Instance.setProgressResultInfo =
            JsonHelper.JsonToObject<HttpInfoReturn<SetProgressResultInfo>>(result);
        if (UserDataManager.Instance.setProgressResultInfo != null &&
            UserDataManager.Instance.setProgressResultInfo.data != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (UserDataManager.Instance.setProgressResultInfo.data.rewardamount > 0)
                {
                    UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.setProgressResultInfo.data.bkey);
                    UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.setProgressResultInfo.data.diamond);
                }
                if (UserDataManager.Instance.setProgressResultInfo.code == 200)
                {
                    UserDataManager.Instance.UpdatePropItemWhenServerCallback();
                }
            }, null);
        }
    }


    /// <summary>
    /// 获取免费钥匙
    /// </summary>
    /// <param name="vToken"></param>
    public void GetFreeKey(EventHandler vCallBackHandler)
    {
        string url = "api_userFirstGift";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_userFirstGift");
                dic.Add("UUID", UUID.ToString());
                dic.Add("TOKEN", TOKEN.ToString());
                TalkingDataManager.Instance.RecordProtocolError("api_userFirstGift", dic);
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 非正式服通知服务端下发某个商品货币
    /// </summary>
    /// <param name="vToken"></param>
    public void DevFinishOrder(string sn, string product_id, string iggid, EventHandler vCallBackHandler)
    {
        string url = "api_devFinishOrder";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("sn", sn);
        parameters.Add("product_id", product_id);
        parameters.Add("iggid", iggid);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }
            vCallBackHandler(result);
        });
    }


    /// <summary>
    /// 获取7天登陆奖励
    /// </summary>
    /// <param name="vToken"></param>
    public void GetDayLogin(EventHandler vCallBackHandler)
    {
        string url = "api_userdayloginprice";
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_userdayloginprice");
                dic.Add("UUID", UUID.ToString());
                dic.Add("TOKEN", TOKEN.ToString());
                TalkingDataManager.Instance.RecordProtocolError("api_userdayloginprice", dic);
                return;
            }

            vCallBackHandler(result);
        });
    }


    /// <summary>
    /// 购买章节,用户章节扣费
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vChapterId"></param>
    public void BuyChapter(int vBookId, int vChapterId, Action<HttpInfoReturn<BuyChapterResultInfo>> vCallBackHandler)
    {
        //string url = "api_chaptercost?phoneimei=" + UUID + "&token=" + TOKEN + "&bookid=" + vBookId + "&chapterid=" + vChapterId;

        string url = "api_enterChapter";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("bookid", vBookId.ToString());
        parameters.Add("chapterid", vChapterId.ToString());
        bool is_use_prop = UserDataManager.Instance.is_use_prop;//是否使用道具: 1.使用道具 0.不使用（非必传，默认不使用）
        int use_prop = is_use_prop ? 1 : 0;
        parameters.Add("is_use_prop", use_prop.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            var buyData = JsonHelper.JsonToObject<HttpInfoReturn<BuyChapterResultInfo>>(result);
            if (buyData.code == 200)
            {
                UserDataManager.Instance.SetBuyChapterResultInfo(vBookId, buyData);
                BookData bookData =
                    UserDataManager.Instance.UserData.BookDataList.Find((bookdata) => bookdata.BookID == vBookId);
                if (bookData == null)
                {
                    bookData = new BookData();
                    UserDataManager.Instance.UserData.BookDataList.Add(bookData);
                }

                stepinfo tempStepInfo = buyData.data.step_info;
                bookData.BookID = tempStepInfo.bookid;
                bookData.DialogueID = tempStepInfo.dialogid;
                bookData.PlayerDetailsID = tempStepInfo.roleid;
                bookData.PlayerClothes = tempStepInfo.clothid;
                bookData.ChapterID = tempStepInfo.chapterid;

                bookData.character_id = tempStepInfo.character_id;
                bookData.outfit_id = tempStepInfo.outfit_id;
                bookData.hair_id = tempStepInfo.hair_id;

                if (bookData.DialogueID == 0)
                    bookData.DialogueID = 1;
                if (bookData.ChapterID == 0)
                    bookData.ChapterID = 1;
                if (bookData.PlayerDetailsID == 0)
                    bookData.PlayerDetailsID = 1;
                if (bookData.PlayerClothes == 0)
                    bookData.PlayerClothes = 1;

                if (!string.IsNullOrEmpty(tempStepInfo.dialogid_step_option))
                {
                    var arr = tempStepInfo.dialogid_step_option.Split(',');
                    UserDataManager.Instance.RecordBookOptionSelect(bookData.BookID, arr);
                }

                if (!string.IsNullOrEmpty(buyData.data.pay_clothes))
                {
                    UserDataManager.Instance.SaveClothHadBuy(buyData.data.pay_clothes);
                }

                if (!string.IsNullOrEmpty(buyData.data.pay_options))
                {
                    UserDataManager.Instance.SaveCharpterSelectHadBuy(buyData.data.pay_options);
                }

                if (!string.IsNullOrEmpty(buyData.data.pay_character))
                {
                    UserDataManager.Instance.SaveCharacterHadBuy(buyData.data.pay_character);
                }

                if (!string.IsNullOrEmpty(buyData.data.pay_hair))
                {
                    UserDataManager.Instance.SaveHairHadBuy(buyData.data.pay_hair);
                }

                if (!string.IsNullOrEmpty(buyData.data.pay_outfit))
                {
                    UserDataManager.Instance.SaveOutfitHadBuy(buyData.data.pay_outfit);
                }
                UserDataManager.Instance.UpdatePropItemWhenServerCallback();
            }

            vCallBackHandler(buyData);


            
        });
    }


    /// <summary>
    /// 获取用户指定书本信息
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vChapterId"></param>
    /// <param name="vDialogId"></param>
    /// <param name="vOptionId">选项</param>
    /// <param name="vCallBackHandler"></param>
    public void GetBookDetailInfo(int vBookId, EventHandler vCallBackHandler)
    {
        string url = "api_getBookInfo?bookid=" + vBookId;
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getBookInfo");
                dic.Add("UUID", UUID.ToString());
                dic.Add("TOKEN", TOKEN.ToString());
                dic.Add("vBookId", vBookId.ToString());
                TalkingDataManager.Instance.RecordProtocolError("api_getBookInfo", dic);
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 获取书本的弹幕留言个数
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vChaptersId"></param>
    /// <param name="vCallBackHandler"></param>
    public void GetBookBarrageCountList(int vBookId, int vChaptersId, EventHandler vCallBackHandler)
    {
        string url = "api_getBookBarrageCountList?book_id=" + vBookId + "&chapter_id=" + vChaptersId;
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getBookBarrageCountList");
                dic.Add("UUID", UUID.ToString());
                dic.Add("TOKEN", TOKEN.ToString());
                dic.Add("vBookId", vBookId.ToString());
                dic.Add("vChaptersId", vChaptersId.ToString());
                TalkingDataManager.Instance.RecordProtocolError("api_getBookBarrageCountList", dic);
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 获取对话对应的详细弹幕列表
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vDialogId"></param>
    /// <param name="vPage"></param>
    /// <param name="vCallBackHandler"></param>
    public void GetBookBarrageInfoList(int vBookId, int vDialogId, int vPage, EventHandler vCallBackHandler)
    {
        string url = "api_getBookBarrageList?book_id=" + vBookId + "&dialog_id=" + vDialogId + "&page=" + vPage +
                     "&sort_type=0";
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getBookBarrageCountList");
                dic.Add("UUID", UUID.ToString());
                dic.Add("TOKEN", TOKEN.ToString());
                dic.Add("vBookId", vBookId.ToString());
                dic.Add("vPage", vPage.ToString());
                dic.Add("vDialogId", vDialogId.ToString());
                TalkingDataManager.Instance.RecordProtocolError("api_getBookBarrageList", dic);
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 发送弹幕留言
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vDialogId"></param>
    /// <param name="vContent"></param>
    /// <param name="vCallBackHandler"></param>
    public void CreateBookBarrage(int vBookId, int vDialogId, string vContent, EventHandler vCallBackHandler)
    {
        string url = "api_createBookBarrage";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("book_id", vBookId.ToString());
        parameters.Add("content", vContent);
        parameters.Add("dialog_id", vDialogId.ToString());

        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }


    /// <summary>
    /// 重置书本
    /// </summary>
    /// <param name="vBookId"> 书本ID</param>
    /// <param name="vCallBackHandler"></param>
    public void ResetBook(int vBookId, EventHandler vCallBackHandler)
    {
        string url = "api_resetBook";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("bookid", vBookId.ToString());

        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 重置章节
    /// </summary>
    public void ResetChapter(int vBookId, int vChapterId, EventHandler vCallBackHandler)
    {
        string url = "api_resetChapter";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("bookid", vBookId.ToString());
        parameters.Add("chapterid", vChapterId.ToString());
        bool is_use_prop = UserDataManager.Instance.is_use_prop;//是否使用道具: 1.使用道具 0.不使用（非必传，默认不使用）
        int use_prop = is_use_prop ? 1 : 0;
        parameters.Add("is_use_prop", use_prop.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);

            UserDataManager.Instance.UpdatePropItemWhenServerCallback();
        });
    }

    /// <summary>
    /// 获取用户指定书本已扣费的章节
    /// </summary>
    /// <param name="vBookId"></param>
    public void GetCostChapterList(int vBookId, EventHandler vCallBackHandler)
    {
        string url = "api_getuserchaptercost";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("bookid", vBookId.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 获取商城价格配置
    /// </summary>
    public void GetShopList(EventHandler vCallBackHandler)
    {
        string url = "api_getProductList";
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 获取商城免费钻石状态
    /// </summary>
    public void GetMallAwardStatus(EventHandler vCallBackHandler)
    {
        string url = "api_getMallAwardStatus";
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }
            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 获取商城免费钻石奖励
    /// </summary>
    /// <param name="vBookId"></param>
    public void ReceiveMallAward(EventHandler vCallBackHandler)
    {
        string url = "api_receiveMallAward";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }
            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 生成用户充值订单
    /// </summary>
    public void GetOrderFormInfo(int vShopId, string vPaymentName, string vRefreshToken, int vsource,
        Action<string> vCallBackHandler)
    {
        string url = "api_createOrder";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("mallid", vShopId.ToString());
        string paymentType = "google";
#if CHANNEL_HUAWEI
        paymentType = "huawei";
#endif
#if UNITY_IOS
        paymentType = "ios";
#endif
        parameters.Add("payment_name", paymentType);
        parameters.Add("refresh_token", vRefreshToken.ToString());
        parameters.Add("source", vsource.ToString());
        //parameters.Add("deviceModel", SystemInfo.deviceModel);
        //parameters.Add("deviceName", SystemInfo.deviceName);
        //parameters.Add("deviceUniqueIdentifier", SystemInfo.deviceUniqueIdentifier);
        //parameters.Add("operatingSystem", SystemInfo.operatingSystem);
        parameters.Add("productNo", vShopId.ToString());
        this.Post(url, parameters, (responseCode, result) => { vCallBackHandler(result); });
    }

    /// <summary>
    /// 充值订单完成
    /// </summary>
    public void GetOrderToSubmitForAndroid(string recharge_no, string vOrderId, string vOrderToken, string vProductid,
        string vPackagename, string vDatasignature, string vPurchasetime, string vPurchaseState, string vTestToken,
        Action<string, string> vCallBackHandler)
    {
        string url = "api_finishOrder";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("recharge_no", recharge_no.ToString());
        parameters.Add("productid", vProductid.ToString());
        parameters.Add("packagename", vPackagename.ToString());
        parameters.Add("payment_type", "1");

        parameters.Add("datasignature", vDatasignature.ToString());
        parameters.Add("purchasetime", vPurchasetime.ToString());
        parameters.Add("purchaseState", vPurchaseState.ToString());
        parameters.Add("google_orderid", vOrderId.ToString());
        parameters.Add("order_token", vOrderToken.ToString());

        parameters.Add("secretword", PayFinishKey);
        parameters.Add("test_token", vTestToken);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }
            else
            {
                JsonObject jo = JsonHelper.JsonToJObject(result.ToString());
                if (jo != null && jo.code == 200)
                {
                    UserDataManager.Instance.SetIsPayState(1);
                }
            }

            vCallBackHandler(vOrderId, result);
        });
    }

    /// <summary>
    /// 充值订单完成 ios 的订单提交
    /// </summary>
    public void GetOrderToSubmitForIos(string vOrderId, string vOrderToken, string vProductid, string vTransactionId,
        int vIsSandbox, Action<string, string> vCallBackHandler)
    {
        string url = "api_finishOrder";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("recharge_no", vOrderId.ToString());
        parameters.Add("productid", vProductid.ToString());
        parameters.Add("packagename", SdkMgr.packageName);
        parameters.Add("payment_type", "2"); //1=google / 2=ios
        string tokenResult = vOrderToken;
        parameters.Add("datasignature", tokenResult.ToString());
        parameters.Add("ios_orderid", vTransactionId.ToString());
#if ENABLE_DEBUG
        vIsSandbox = 1;
#endif
        parameters.Add("is_sandbox", vIsSandbox.ToString());
        parameters.Add("secretword", PayFinishKey);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }
            else
            {
                JsonObject jo = JsonHelper.JsonToJObject(result.ToString());
                if (jo != null && jo.code == 200)
                {
                    UserDataManager.Instance.SetIsPayState(1);
                }
            }

            vCallBackHandler(vTransactionId, result);
        });
    }

    /// <summary>
    /// 用户取消充值订单
    /// 用户订单失败的类型：2 订单异常，3：用户取消付款
    /// </summary>
    public void UserOrderCancel(string recharge_no, string vMsg, int vStatus, EventHandler vCallBackHandler)
    {
        string url = "api_cancelOrder";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("recharge_no", recharge_no);
        parameters.Add("remark", vMsg);
        //parameters.Add("order_status", vStatus.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }


    public void Recoverorder(string vOrderId, string vOrderToken, EventHandler vCallBackHandler)
    {
        string url = "api_recoverOrder";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("order_id", vOrderId.ToString());

        string tokenResult = vOrderToken;
        parameters.Add("datasignature", tokenResult.ToString());
        int vIsSandbox = 0;
#if ENABLE_DEBUG
        vIsSandbox = 1;
#endif
        parameters.Add("is_sandbox", vIsSandbox.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }
            else
            {
                JsonObject jo = JsonHelper.JsonToJObject(result.ToString());
                if (jo != null && jo.code == 200)
                {
                    UserDataManager.Instance.SetIsPayState(1);
                }
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    ////// <summary>
    ///订单支付验证
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void SubmitChargeOrder(string vRechargeNo, EventHandler vCallBackHandler)
    {
        string url = "api_checkPayInfo";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("recharge_no", vRechargeNo);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    ///获取用户当天的抽奖状态
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void GetUserTicket(EventHandler vCallBackHandler)
    {
        string url = "api_getuserticket";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    ///用户抽奖
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void LuckyDraw(EventHandler vCallBackHandler)
    {
        string url = "api_luckydraw";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 抽奖选项
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void GetTickOptions(EventHandler vCallBackHandler)
    {
        string url = "api_getticketoption";
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getticketoption");
                TalkingDataManager.Instance.RecordProtocolError("api_getticketoption", dic);
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 点击广告领取钻石
    /// <param name="vType">1普通广告  2书本广告,3.活动广告</param>
    /// </summary>
    public void GetAdsReward(int vType, EventHandler vCallBackHandler, int bookId = 0)
    {
        string url = "api_adReward";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("adtype", vType.ToString());
        parameters.Add("bookid", bookId.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }


    public void GetAdsReward(int vType, int rewardType, EventHandler vCallBackHandler)
    {
        string url = "api_adReward";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("adtype", vType.ToString());
        parameters.Add("reward_type", rewardType.ToString());

        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }


    public void GetChapterAdsReward(int bookid, int chapterid, EventHandler vCallBackHandler)
    {
        string url = "api_chapterEndAdReward";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("bookid", bookid.ToString());
        parameters.Add("chapterid", chapterid.ToString());

        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }


    /// <summary>
    /// 清除用户游戏记录
    /// </summary>
    public void ClearUserRecord(EventHandler vCallBackHandler)
    {
        string url = "api_userlogDel";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 获取推荐书本信息,这个是推荐书本
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void GetRecommendABook(float vTime, EventHandler vCallBackHandler)
    {
        string url = "api_getNewUserRecommendBook?logintime=" + vTime;
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getNewUserRecommendBook");
                TalkingDataManager.Instance.RecordProtocolError("api_getNewUserRecommendBook", dic);
                return;
            }

            vCallBackHandler(result);
        });
    }


    /// <summary>
    /// 用户章节留言
    /// </summary>
    /// <param name="bookid"></param>
    /// <param name="chapterid"></param>
    /// <param name="content"></param>
    /// <param name="vCallBackHandler"></param>
    public void ChapterComments(int bookid, int chapterid, string content, EventHandler vCallBackHandler)
    {
        string url = "api_bookleavemsg";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("bookid", bookid.ToString());
        parameters.Add("chapterid", chapterid.ToString());
        parameters.Add("content", content.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 用户章节留言回复
    /// </summary>
    /// <param name="discussid"></param>
    /// <param name="vCallBackHandler"></param>
    public void ChapterCommentSanswering(int discussid, string content, EventHandler vCallBackHandler)
    {
        string url = "api_bookmsgreply";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("discussid", discussid.ToString());
        parameters.Add("content", content.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 查看用户章节留言
    /// </summary>
    /// <param name="bookid"></param>
    /// <param name="chapterid"></param>
    /// <param name="vCallBackHandler"></param>
    public void ViewChapterMessages(int bookid, int chapterid, int page, EventHandler vCallBackHandler)
    {
        string url = "api_getbookleavemsg";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("bookid", bookid.ToString());
        parameters.Add("chapterid", chapterid.ToString());
        parameters.Add("page", page.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 评论回复信息
    /// </summary>
    /// <param name="discussid"></param>
    /// <param name="page"></param>
    /// <param name="vCallBackHandler"></param>
    public void ChapterCommenBack(int discussid, int page, EventHandler vCallBackHandler)
    {
        //LOG.Info("回复id:"+ discussid);
        string url = "api_getbookreply";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("discussid", discussid.ToString());
        parameters.Add("page", page.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 章节评论点赞设置
    /// </summary>
    /// <param name="discussId">评论ID</param>
    /// <param name="discussType">评论类型  1点赞 0不认同</param>
    /// <param name="vCallBackHandler"></param>
    public void DiscussComment(int discussId, int discussType, EventHandler vCallBackHandler)
    {
        string url = "api_bookdiscussbest";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("discussid", discussId.ToString());
        parameters.Add("discusstype", discussType.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 书本收藏设置
    /// </summary>
    /// <param name="bookid"></param>
    /// <param name="isfav">0:取消收藏，1:收藏</param>
    /// <param name="vCallBackHandler"></param>
    public void BookCollectionSettings(int bookid, int isfav, EventHandler vCallBackHandler)
    {
        string url = "api_userBookFav";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("bookid", bookid.ToString());
        parameters.Add("isfav", isfav.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 章节回复点赞
    /// </summary>
    /// <param name="replyid"></param>
    /// <param name="discussType"></param>
    /// <param name="vCallBackHandler"></param>
    public void DiscussCommentback(int replyid, int discussType, EventHandler vCallBackHandler)
    {
        string url = "api_bookreplybest";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("replyid", replyid.ToString());
        parameters.Add("discusstype", discussType.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }


    /// <summary>
    /// 这个是获取用户的邮箱信息
    /// </summary>
    /// <param name="page"></param>
    /// <param name="vCallBackHandler"></param>
    public void GetEmail(int page, EventHandler vCallBackHandler)
    {
        string url = "api_getSystemMsg";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("page", page.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 这个是读取用户的邮件
    /// </summary>
    /// <param name="msgid"></param>
    /// <param name="vCallBackHandler"></param>
    public void ReadingUserEmail(int msgid, EventHandler vCallBackHandler)
    {
        string url = "api_readSystemMsg";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("msgid", msgid.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 获得邮件的奖励
    /// </summary>
    public void GetEmailAward(int msgid, EventHandler vCallBackHandler)
    {
        string url = "api_achieveMsgPrice";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("msgid", msgid.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    public void UserFeedback(int msgtype, string email, string content, EventHandler vCallBackHandler)
    {
        string url = "api_getUserSuggest";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("msgtype", msgtype.ToString());
        parameters.Add("email", email.ToString());
        parameters.Add("content", content.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 这个是2小时免费钥匙申请
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void FreeKeyApply(EventHandler vCallBackHandler)
    {
        string url = "api_userFirstGiftApply";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 获取指定对话的表情列表
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void GetGameEmojiMsgList(int vBookId, int vDialogId, EventHandler vCallBackHandler)
    {
        string url = "api_getBookPhiz?bookid=" + vBookId + "&dialogid=" + vDialogId;
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getBookPhiz");
                dic.Add("vBookId", vBookId.ToString());
                dic.Add("vDialogId", vDialogId.ToString());
                TalkingDataManager.Instance.RecordProtocolError("api_getBookPhiz", dic);
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    ///增加指定对话的表情
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vBookId"></param>
    /// <param name="vOptionId">选项id (如1,2,3,4）</param>
    /// <param name="vCallBackHandler"></param>
    public void SendEmojiMsgItem(int vBookId, int vDialogId, int vOptionId, EventHandler vCallBackHandler)
    {
        string url = "api_addBookPhiz";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("bookid", vBookId.ToString());
        parameters.Add("dialogid", vDialogId.ToString());
        parameters.Add("option", vOptionId.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    ///这个是删除用户指定的邮件
    /// </summary>
    /// <param name="msgid"></param>
    /// <param name="vCallBackHandler"></param>
    public void DeletUserEmail(int msgid, EventHandler vCallBackHandler)
    {
        string url = "api_systemMsgDel";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("msgid", msgid.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    public string Email_picUrl(string st)
    {
        return st;
    }


    /// <summary>
    /// 设置用户默认语言
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    /// <param name="vChangeType">设置类型 1语言 2昵称 默认是1</param>
    public void SetUserLanguage(string vLanguage, int vChangeType, EventHandler vCallBackHandler)
    {
        string url = "api_setUserInfo";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("value", vLanguage);
        parameters.Add("change_type", vChangeType.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 设置用户关闭评分
    /// </summary>
    /// <param name="vChangeType">设置类型3 是否已经提示去商店评分 (1是  0否) </param>
    public void SetUserRating(int value, EventHandler vCallBackHandler)
    {
        string url = "api_setUserInfo";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("value", value.ToString());
        parameters.Add("change_type", "3");
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }


    /// <summary>
    ///获得tcp端口
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void GetUserSocketInfo(EventHandler vCallBackHandler)
    {
        //string url = "api_getusersocker";
        string url = "http://192.168.0.142/api_getusersocker";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 获取新闻列表信息
    /// </summary>
    /// <param name="page"></param>
    /// <param name="vCallBackHandler"></param>
    public void GetNewInfo(int page, EventHandler vCallBackHandler)
    {
        string url = "api_getsystemnew";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("page", page.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 设置新闻是否已读
    /// </summary>
    /// <param name="newid"></param>
    /// <param name="vCallBackHandler"></param>
    public void NewReading(int newid, EventHandler vCallBackHandler)
    {
        string url = "api_getsystemnewread";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("newid", newid.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 设置新闻的点赞
    /// </summary>
    /// <param name="newid"></param>
    /// <param name="bestests"></param>
    /// <param name="vCallBackHandler"></param>
    public void NewLike(int newid, int bestests, EventHandler vCallBackHandler)
    {
        string url = "api_getsystemnewbestests";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("newid", newid.ToString());
        parameters.Add("bestests", bestests.ToString());
        LOG.Info("TOKEN:" + TOKEN + "--uuid:" + UUID + "--newid:" + newid + "--bestests:" + bestests);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }


    /// <summary>
    ///设置用户书本比率
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void Userbookrate(int type, int dialogid, int option, EventHandler vCallBackHandler)
    {
        string url = "api_userbookrate";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("type", type.ToString());
        parameters.Add("dialogid", dialogid.ToString());
        parameters.Add("option", option.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 书本列表
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void Getbookgiftbag(EventHandler vCallBackHandler)
    {
        return;
        string url = "api_getbookgiftbag";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 购买书本礼包
    /// </summary>
    /// <param name="giftid"></param>
    /// <param name="vCallBackHandler"></param>
    public void Buybookgiftbag(int giftid, EventHandler vCallBackHandler)
    {
        string url = "api_buybookgiftbag";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("giftid", giftid.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 红点协议
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void ActiveInfo(EventHandler vCallBackHandler)
    {
        string url = "api_getActiveInfo";
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getimpinfo");
                dic.Add("UUID", UUID.ToString());
                dic.Add("TOKEN", TOKEN.ToString());
                TalkingDataManager.Instance.RecordProtocolError("api_getimpinfo", dic);
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 获取用户商城礼包
    /// </summary>
    /// <param name="paytype"></param>
    /// <param name="priceplan"></param>
    /// <param name="vCallBackHandler"></param>
    public void Getuserpackage(int paytype, int priceplan, EventHandler vCallBackHandler)
    {
        string url = "api_getPackageList?paytype=" + paytype;
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 客户端请求错误
    /// </summary>
    /// <param name="vUrl"></param>
    /// <param name="infoJson"></param>
    /// <param name="vCallBackHandler"></param>
    public void SetClientError(string vUrl, string infoJson, EventHandler vCallBackHandler)
    {
        string url = "api_uploadErrorLog";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("phoneimei", UUID);
        parameters.Add("api", vUrl.ToString());
        parameters.Add("content", infoJson.ToString());
        parameters.Add("remark", "备注");
        UniHttp.Instance.Post(url, parameters, null, 15, 3, false);
    }

    /// <summary>
    /// 获得用户已购买的商城配置ID
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void Getuserpaymallid(EventHandler vCallBackHandler)
    {
        string url = "api_getOwnProductIDS";
        this.Get(url, (responseCode, result) =>
            {
                if (responseCode == 200)
                {
                }
                else
                {
                    result = "error";
                }

                vCallBackHandler(result);
            }, 1000 * 10, 3, true, false, false);
    }

    /// <summary>
    /// 公告栏
    /// </summary>
    /// <param name="day"></param>
    /// <param name="vCallBackHandler"></param>
    public void GetBulletinBoard(int day, float vTime, EventHandler vCallBackHandler)
    {
        //LOG.Info("day:" + day + "--TOKEN:" + TOKEN + "--phoneimei:" + UUID);
        string url = "api_getBulletinBoard";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("day", day.ToString());
        parameters.Add("logintime", vTime.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode == 200)
            {
            }
            else
            {
                result = "error";
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 获得VIP的信息
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void Getvipcard(EventHandler vCallBackHandler)
    {
        string url = "api_getvipcard";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode == 200)
            {
            }
            else
            {
                result = "error";
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 每日领取VIP卡奖励
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void Getvipcardreceive(EventHandler vCallBackHandler)
    {
        string url = "api_getvipcardreceive";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode == 200)
            {
            }
            else
            {
                result = "error";
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 获得邀请奖励列表
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void GetInviteRewardList(EventHandler vCallBackHandler)
    {
        string url = "api_getInviteList";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode == 200)
            {
            }
            else
            {
                result = "error";
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 领取邀请奖励
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void ReceiveInviteReward(int vInviteId, EventHandler vCallBackHandler)
    {
        string url = "api_inviteReceive";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("invite_id", vInviteId.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode == 200)
            {
            }
            else
            {
                result = "error";
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 互换邀请码
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void InviteExchange(string vCode, EventHandler vCallBackHandler)
    {
        string url = "api_inviteExchange";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("code", vCode);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode == 200)
            {
            }
            else
            {
                result = "error";
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 用户每日任务列表
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void Getusertask(EventHandler vCallBackHandler)
    {
        string url = "api_getusertask";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode == 200)
            {
            }
            else
            {
                result = "error";
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 用户领取每日任务奖励
    /// </summary>
    /// <param name="task_id"></param>
    /// <param name="vCallBackHandler"></param>
    public void Achievetaskprice(int task_id, EventHandler vCallBackHandler)
    {
        string url = "api_achievetaskprice";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("task_id", task_id.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode == 200)
            {
            }
            else
            {
                result = "error";
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 设置任务双倍奖励的标示
    /// </summary>
    /// <param name="task_id"></param>
    /// <param name="vCallBackHandler"></param>
    public void SetDoubleRewardDayTask(int task_id, EventHandler vCallBackHandler)
    {
        string url = "api_taskadprice";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("task_id", task_id.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode == 200)
            {
            }
            else
            {
                result = "error";
            }

            vCallBackHandler(result);
        });
    }

    public void Achieveboxprice(int box_id, EventHandler vCallBackHandler)
    {
        string url = "api_achieveboxprice";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("box_id", box_id.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode == 200)
            {
            }
            else
            {
                result = "error";
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 获取宝箱奖励的列表
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void Getboxlist(EventHandler vCallBackHandler)
    {
        string url = "api_getboxlist";
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getboxlist");
                dic.Add("UUID", UUID.ToString());
                dic.Add("TOKEN", TOKEN.ToString());
                TalkingDataManager.Instance.RecordProtocolError("api_getboxlist", dic);
                return;
            }

            vCallBackHandler(result);
        });
    }


    /// <summary>
    /// 用户性格
    /// </summary>
    public void RefreshUserProfile(EventHandler vCallBackHandler)
    {
        string url = "api_setUserTrait";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 获取用户个人中心 的装扮配置
    /// </summary>
    public void GetUserProfileCustomizeConfig(EventHandler vCallBackHandler)
    {
        string url = "api_getOrnament";
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getOrnament");
                dic.Add("UUID", UUID.ToString());
                dic.Add("TOKEN", TOKEN.ToString());
                TalkingDataManager.Instance.RecordProtocolError("api_getOrnament", dic);
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 设置用户个人中心 的装扮信息
    /// </summary>
    public void SetUserProfileCustomize(int vFaceIcon, int vBgId, int vFrameIcon, EventHandler vCallBackHandler)
    {
        string url = "api_setUserAvatar";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("faceicon", vFaceIcon.ToString());
        parameters.Add("facecircle", vFrameIcon.ToString());
        parameters.Add("background", vBgId.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 设置预约
    /// </summary>
    public void SetSubscribe(int vBookId, EventHandler vCallBackHandler)
    {
        string url = "api_setSubscribe";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("bookid", vBookId.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 这个是一件领取功能
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void Achieveallmsgprice(EventHandler vCallBackHandler)
    {
        string url = "api_achieveAllMsgPrice";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);

        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 新闻评论列表
    /// </summary>
    /// <param name="com_type"></param>
    /// <param name="com_value"></param>
    /// <param name="page"></param>
    /// <param name="vCallBackHandler"></param>
    public void Getcomment(int com_type, int com_value, int com_extend, int page, EventHandler vCallBackHandler)
    {
        //LOG.Info("com_type:"+ com_type+ "com_value:"+ com_value);
        string url = "api_getcomment";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("com_type", com_type.ToString());
        parameters.Add("com_value", com_value.ToString());
        parameters.Add("com_extend", com_extend.ToString());
        parameters.Add("page", page.ToString());

        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 设置评论点赞
    /// </summary>
    /// <param name="com_type"></param>
    /// <param name="com_reply"></param>
    /// <param name="com_value"></param>
    /// <param name="vCallBackHandler"></param>
    public void Setcommentpraise(int com_type, int com_reply, int com_value, EventHandler vCallBackHandler)
    {
        //LOG.Info(com_type+"---"+com_reply + "---" + com_value);
        string url = "api_setcommentpraise";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("com_type", com_type.ToString());
        parameters.Add("com_reply", com_reply.ToString());
        parameters.Add("com_value", com_value.ToString());
        parameters.Add("praise", "1");

        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 设置评论
    /// </summary>
    /// <param name="com_reply"></param>
    /// <param name="com_type"></param>
    /// <param name="com_value"></param>
    /// <param name="content"></param>
    /// <param name="vCallBackHandler"></param>
    public void Setcomment(int com_reply, int com_type, int com_value, string content, int com_extend,
        EventHandler vCallBackHandler)
    {
        LOG.Info("com_reply:" + com_reply + "com_type:" + com_type + "com_value:" + com_value + "content:" + content);
        string url = "api_setcomment";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("com_type", com_type.ToString());
        parameters.Add("com_reply", com_reply.ToString());
        parameters.Add("com_value", com_value.ToString());
        parameters.Add("com_extend", com_extend.ToString());
        parameters.Add("content", content.ToString());

        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 评论回复列表
    /// </summary>
    /// <param name="commentid"></param>
    /// <param name="page"></param>
    /// <param name="vCallBackHandler"></param>
    public void Getcommentreplay(int commentid, int page, EventHandler vCallBackHandler)
    {
        string url = "api_getcommentreplay";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("commentid", commentid.ToString());
        parameters.Add("page", page.ToString());

        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 设置评论举报
    /// </summary>
    /// <param name="com_type"></param>
    /// <param name="com_reply"></param>
    /// <param name="com_value"></param>
    /// <param name="vCallBackHandler"></param>
    public void Setcommentreport(int com_type, int com_reply, int com_value, EventHandler vCallBackHandler)
    {
        string url = "api_setcommentreport";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("com_type", com_type.ToString());
        parameters.Add("com_reply", com_reply.ToString());
        parameters.Add("com_value", com_value.ToString());

        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }


    /// <summary>
    /// 获得宠物信息
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void Getpetinfo(int page, EventHandler vCallBackHandler)
    {
        string url = "api_getpetinfo?page=" + page;
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getimpinfo");
                dic.Add("UUID", UUID.ToString());
                dic.Add("TOKEN", TOKEN.ToString());
                dic.Add("page", page.ToString());
                TalkingDataManager.Instance.RecordProtocolError("api_getpetinfo", dic);
                return;
            }

            vCallBackHandler(result);
        });
    }

    public void GetpetFoodinfo(int page, EventHandler vCallBackHandler)
    {
        string url = "api_getpetfood?page=" + page;
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getimpinfo");
                TalkingDataManager.Instance.RecordProtocolError("api_getpetfood", dic);
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 获取宠物回馈礼物
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void PostpetGiftinfo(int page, EventHandler vCallBackHandler, int type = 1)
    {
        string url = "api_getfeedback";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("visit_type", type.ToString());
        parameters.Add("page", page.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 领取回馈
    /// </summary>
    /// <param name="id"></param>
    /// <param name="type"></param>
    /// <param name="vCallBackHandler"></param>
    public void PostGetpetGift(int id, int type, int visit, EventHandler vCallBackHandler)
    {
        string url = "api_achievefeedback";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("fid", id.ToString());
        parameters.Add("achieve_type", type.ToString());
        parameters.Add("visit_type", visit.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 購買商品
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void PostpetBuyItem(string id, string count, EventHandler vCallBackHandler)
    {
        string url = "api_userpayfood";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("mallid", id);
        parameters.Add("count", count);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">位置</param>
    /// <param name="shopid"></param>
    /// <param name="vCallBackHandler"></param>
    public void PostPlaceItem(string id, string shopid, EventHandler vCallBackHandler)
    {
        string url = "api_placement";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("shopid", shopid);
        parameters.Add("place", id);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    public void PostGetSceneInfo(EventHandler vCallBackHandler)
    {
        string url = "api_getambient";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 擴建
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void PostUpgradeYard(EventHandler vCallBackHandler)
    {
        string url = "api_yardupgrade";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    public void PostGetYardInfo(EventHandler vCallBackHandler)
    {
        string url = "api_getyard";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }


    /// <summary>
    /// 获得宠物背包信息
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void Getpetpackinfo(EventHandler vCallBackHandler)
    {
        string url = "api_getuserbackpack";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 获取用户已收养的宠物
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void Getadoptpet(EventHandler vCallBackHandler)
    {
        string url = "api_getadoptpet";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    public void Getuserhomepet(int pid, EventHandler vCallBackHandler)
    {
        string url = "api_getuserhomepet";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("pid", pid.ToString());

        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 收养宠物
    /// </summary>
    /// <param name="pid"></param>
    /// <param name="vCallBackHandler"></param>
    public void PostPetadopt(int pid, EventHandler vCallBackHandler)
    {
        string url = "api_petadopt";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("pid", pid.ToString());

        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 放弃已经收养的宠物
    /// </summary>
    /// <param name="pid"></param>
    /// <param name="vCallBackHandler"></param>
    public void Postpetbackout(int adoptid, EventHandler vCallBackHandler)
    {
        string url = "api_petbackout";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("adoptid", adoptid.ToString());

        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 阅读宠物的故事
    /// </summary>
    /// <param name="pid"></param>
    /// <param name="story_id"></param>
    /// <param name="vCallBackHandler"></param>
    public void Rendpetstory(int pid, int story_id, EventHandler vCallBackHandler)
    {
        string url = "api_readpetstory";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("pid", pid.ToString());
        parameters.Add("story_id", story_id.ToString());

        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    public void Getpetstory(EventHandler vCallBackHandler)
    {
        string url = "api_getpetstory";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);

        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    public void Readpetstory(int pid, int story_id, EventHandler vCallBackHandler)
    {
        string url = "api_readpetstory";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("pid", pid.ToString());
        parameters.Add("story_id", story_id.ToString());

        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 宠物的红点功能
    /// </summary>
    /// <param name="page"></param>
    /// <param name="vCallBackHandler"></param>
    public void Getpetmsg(EventHandler vCallBackHandler)
    {
        string url = "api_getpetmsg";
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getpetmsg");
                TalkingDataManager.Instance.RecordProtocolError("api_getpetmsg", dic);
                return;
            }

            vCallBackHandler(result);
        });
    }


    /// <summary>
    /// 获得商城分档推荐
    /// </summary>
    /// <param name="product_type"></param>
    /// <param name="vCallBackHandler"></param>
    public void Getrecommandmall(int product_type, EventHandler vCallBackHandler)
    {
        string url = "api_getRecommendProductList?product_type=" + product_type;

        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 获取用户ip的相关信息
    /// </summary>
    /// <param name="page"></param>
    public void GetPlayerIpAdressInfo(string vIp, EventHandler vCallBackHandler)
    {
        //string url = "http://ip.taobao.com/service/getIpInfo.php?ip=" + vIp;
        //string url = "http://ip-api.com/json/" + vIp;
        string url = "api_getIpInfo";
        this.Get(url, (responseCode, result) => { vCallBackHandler(result); });
    }


    /// <summary>
    /// 更新宠物引导步骤
    /// </summary>
    public void UserpetguideChange(int guideId, EventHandler vCallBackHandler)
    {
        string url = "api_userpetguide_change";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);
        parameters.Add("guide_id", guideId.ToString());

        this.Post(url, parameters, (responseCode, result) =>
        {
            if (result.ToString().Equals("error"))
            {
                TalkingDataManager.Instance.RecordProtocolError("api_userpetguide_change", parameters);
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 完成宠物引导获得奖励
    /// </summary>
    public void AchieveguidePrice(EventHandler vCallBackHandler)
    {
        string url = "api_achieveguide_price";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters.Add("token", TOKEN);
        //parameters.Add("phoneimei", UUID);

        this.Post(url, parameters, (responseCode, result) =>
        {
            if (result.ToString().Equals("error"))
            {
                TalkingDataManager.Instance.RecordProtocolError("api_achieveguide_price", parameters);
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 获取功能开启状态
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void GetGameFunState(EventHandler vCallBackHandler)
    {
        int type = 1;
#if UNITY_IOS
        type = 2;
#elif UNITY_ANDROID
        type = 1;
#endif
        string url = "api_getdisjunctor?platform=" + type;
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getpetdisjunctor");
                TalkingDataManager.Instance.RecordProtocolError("api_getpetdisjunctor", dic);
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 这个是游客登录的时候获得游客登录的Toten
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void TouristLogin(EventHandler vCallBackHandler)
    {
        TalkingDataManager.Instance.LoginAccount(EventEnum.LoginTouristStart);
        GamePointManager.Instance.BuriedPoint(EventEnum.Login);
        string url = "api_touristLogin";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("phoneimei", UUID);
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (result.ToString().Equals("error"))
            {
                TalkingDataManager.Instance.RecordProtocolError("api_touristLogin", parameters);
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 每周更新的书本信息
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void GetbooksUpdatedWeekly(EventHandler vCallBackHandler)
    {
        string url = "api_booksUpdatedWeekly";
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_booksUpdatedWeekly");
                TalkingDataManager.Instance.RecordProtocolError("api_booksUpdatedWeekly", dic);
                return;
            }

            vCallBackHandler(result);
        });
    }




    /// <summary>
    /// 获取书本涉及到的相关的版本信息
    /// </summary>
    /// <param name="vCallBackHandler"></param>
    public void GetBookVersionInfo(int vBookId, EventHandler vCallBackHandler, int vBookVersion = 0, int vChapterVersion = 0,int vRoleModelVersion = 0, int vModelPrice = 0, int vClothesPrice = 0, int vSkinVersion =0)
    {
        string url = "api_getBookVersion?book_id=" + vBookId;
        if (vBookVersion > 0)
            url += "&book_version=" + vBookVersion;
        if (vChapterVersion > 0)
            url += "&chapter_version=" + vChapterVersion;
        if (vRoleModelVersion > 0)
            url += "&role_model_version=" + vRoleModelVersion;
        if (vModelPrice > 0)
            url += "&model_price_version=" + vModelPrice;
        if (vClothesPrice > 0)
            url += "&clothes_price_version=" + vClothesPrice;
        if (vSkinVersion > 0)
            url += "&skin_version=" + vSkinVersion;
            
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getBookVersion");
                TalkingDataManager.Instance.RecordProtocolError("api_getBookVersion", dic);
                return;
            }

            vCallBackHandler(result);
        });
    }
    
    /// <summary>
    /// 获取对应版本的书本详细信息
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vCallBackHandler"></param>
    public void GetVersionBookDetailInfo(int vBookId,EventHandler vCallBackHandler)
    {
        string url = "api_getVersionBookDetailInfo?book_id="+vBookId;
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getVersionBookDetailInfo");
                TalkingDataManager.Instance.RecordProtocolError("api_getVersionBookDetailInfo", dic);
                return;
            }
            vCallBackHandler(result);
        });
    }
    
    /// <summary>
    /// 获取对应版本的书本详细信息
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vCallBackHandler"></param>
    public void GetVersionChapterInfo(int vBookId,int vChapterId,EventHandler vCallBackHandler)
    {
        string url = "api_getVersionChapterInfo?book_id="+vBookId+"&chapter_id="+vChapterId;
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getVersionChapterInfo");
                TalkingDataManager.Instance.RecordProtocolError("api_getVersionChapterInfo", dic);
                return;
            }
            vCallBackHandler(result);
        });
    }
    
    /// <summary>
    /// 获取书本版本-章节列表
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vChapterId"></param>
    /// <param name="vCallBackHandler"></param>
    public void GetVersionChapterList(int vBookId,int vChapterId,EventHandler vCallBackHandler)
    {
        string url = "api_getVersionChapterList?book_id="+vBookId+"&chapter_id="+vChapterId;
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getVersionChapterList");
                TalkingDataManager.Instance.RecordProtocolError("api_getVersionChapterList", dic);
                return;
            }
            vCallBackHandler(result);
        });
    }
    
    /// <summary>
    /// 获取书本版本-对话列表
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vChapterId"></param>
    /// <param name="vCallBackHandler"></param>
    public void GetVersionDialogList(int vBookId,int vChapterId,EventHandler vCallBackHandler)
    {
        string url = "api_getVersionDialogList?book_id="+vBookId+"&chapter_id="+vChapterId;
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getVersionDialogList");
                TalkingDataManager.Instance.RecordProtocolError("api_getVersionDialogList", dic);
                return;
            }
            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 获取皮肤列表
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vCallBackHandler"></param>
    public void GetVersionSkinList(int vBookId,EventHandler vCallBackHandler)
    {
        string url = "api_getVersionSkinList?book_id="+vBookId;
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getVersionSkinList");
                TalkingDataManager.Instance.RecordProtocolError("api_getVersionSkinList", dic);
                return;
            }
            vCallBackHandler(result);
        });
    }
    
    /// <summary>
    /// 获取服装价格列表
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vCallBackHandler"></param>
    public void GetVersionClothesPriceList(int vBookId,EventHandler vCallBackHandler)
    {
        string url = "api_getVersionClothesPriceList?book_id="+vBookId;
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getVersionClothesPriceList");
                TalkingDataManager.Instance.RecordProtocolError("api_getVersionClothesPriceList", dic);
                return;
            }
            vCallBackHandler(result);
        });
    }
    
    /// <summary>
    /// 获取角色形象配置列表信息
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vCallBackHandler"></param>
    public void GetVersionRoleModelList(int vBookId,EventHandler vCallBackHandler)
    {
        string url = "api_getVersionRoleModelList?book_id="+vBookId;
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getVersionRoleModelList");
                TalkingDataManager.Instance.RecordProtocolError("api_getVersionRoleModelList", dic);
                return;
            }
            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 获取形象价格说明列表信息
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vCallBackHandler"></param>
    public void GetVersionModelPriceList(int vBookId,EventHandler vCallBackHandler)
    {
        string url = "api_getVersionModelPriceList?book_id="+vBookId;
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getVersionModelPriceList");
                TalkingDataManager.Instance.RecordProtocolError("api_getVersionModelPriceList", dic);
                return;
            }
            vCallBackHandler(result);
        });
    }

    public void GetBookNotUser(int type, EventHandler vCallBackHandler)
    {
        string url = "api_getBookNotUser?type=" + type;
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getBookNotUser");
                TalkingDataManager.Instance.RecordProtocolError("api_getBookNotUser", dic);
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 获得的创作综合书本的信息
    /// </summary>
    /// <param name="type"></param>
    /// <param name="vCallBackHandler"></param>
    public void GetWriterIndex(EventHandler vCallBackHandler)
    {
        string url = "api_writerIndex";
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_writerIndex");
                TalkingDataManager.Instance.RecordProtocolError("api_writerIndex", dic);
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 获得创作更多界面里面的热门书本
    /// </summary>
    /// <param name="page"></param>
    /// <param name="vCallBackHandler"></param>
    public void GetWriterHotBookList(int page, EventHandler vCallBackHandler)
    {
        string url = "api_getWriterHotBookList?page=" + page;
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getWriterHotBookList");
                TalkingDataManager.Instance.RecordProtocolError("api_getWriterHotBookList", dic);
                return;
            }

            vCallBackHandler(result);
        });
    }

    public void GetWriterNewBookList(int page, EventHandler vCallBackHandler)
    {
        string url = "api_getWriterNewBookList?page=" + page;
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getWriterNewBookList");
                TalkingDataManager.Instance.RecordProtocolError("api_getWriterNewBookList", dic);
                return;
            }

            vCallBackHandler(result);
        });
    }

    public void GetWriterBookList(int page, string tag, string title, EventHandler vCallBackHandler)
    {
        string url = "api_getWriterBookList?page=" + page + "&tag=" + tag + "&title=" + title;
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("ProtocolError", "api_getWriterNewBookList");
                TalkingDataManager.Instance.RecordProtocolError("api_getWriterNewBookList", dic);
                return;
            }

            vCallBackHandler(result);
        });
    }

    public void ShowMovePanel()
    {
        GetMoveCode(GetMoveCodeCallBack);
    }

    private void GetMoveCodeCallBack(object arg)
    {
        string result = arg.ToString();
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            if (jo.code == 200)
            {
                UserDataManager.Instance.moveCodeInfo =
                    JsonHelper.JsonToObject<HttpInfoReturn<MoveCodeInfoCont>>(arg.ToString());
                CUIManager.Instance.OpenForm(UIFormName.MigrationAccountForm);
            }
        }
    }



    /// <summary>
    /// 更新首页执行记录
    /// </summary>
    public void SaveFirstActionLog(int action_id, EventHandler vCallBackHandler)
    {
        string url = "api_saveFirstActionLog";
        Dictionary<string, string> parameters = new Dictionary<string, string>();

        parameters.Add("action_id", action_id.ToString());
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }


    /// <summary>
    /// 用户登出操作
    /// </summary>
    public void Logout(EventHandler vCallBackHandler)
    {
        string url = "api_logout";
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        this.Post(url, parameters, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }

            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 获取首页执行记录
    /// </summary>
    public void GetFirstActionLog(EventHandler vCallBackHandler)
    {
        string url = "api_getFirstActionLog";
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }
            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 通过类型获取可用道具(折扣列表)
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vChapterId"></param>
    /// <param name="vDialogId"></param>
    /// <param name="vOptionId">选项</param>
    /// <param name="vCallBackHandler"></param>
    public void GetPropByType(int[] types, EventHandler vCallBackHandler)
    {
        string type = "";
        for (int i = 0;i< types.Length; i++) {
            type += types[i].ToString();
            if(i< types.Length - 1)
            {
                type += ",";
            }
        }
        string url = "api_getPropByType?type=" + type;
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }
            vCallBackHandler(result);
        });
    }

    /// <summary>
    /// 获取用户可用的背包道具
    /// </summary>
    /// <param name="types"></param>
    /// <param name="vCallBackHandler"></param>
    public void GetBackpackProp(EventHandler vCallBackHandler)
    {
        string url = "api_getBackpackProp";
        this.Get(url, (responseCode, result) =>
        {
            if (responseCode != 200)
            {
                return;
            }
            vCallBackHandler(result);
        });
    }
}
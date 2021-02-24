using FSM;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Framework;
using AB;
using Helper.Login;
using IGG.SDK.Framework.Listener;
using IGG.SDK.Modules.AppConf;
using IGG.SDK.Modules.AppConf.VO;
using Newtonsoft.Json;
using Script.Game.Helpers;
using UniGameLib;
using UGUI;

[CGameStateMgr.GameState]
public class CLaunchState : GameStateBase
{
    public override void OnStateEnter()
    {
        //EventRouter.Instance.AddEventHandler(CEventID.ON_CONFIG_LOAD_FNISHI, OnConfigLoadFinish);
        UberLogger.Logger.ForwardMessages = GameFrameworkImpl.Instance.debugForwardToUnity;
        ABMgr.Instance.Init();


#if ENABLE_DEBUG
#if UNITY_2017_1_OR_NEWER
        UnityEngine.Debug.unityLogger.logEnabled = true;
#else
        UnityEngine.Debug.logger.logEnabled = true;
#endif

#else
            //UnityEngine.Debug.logger.logEnabled = false;

#if UNITY_2017_1_OR_NEWER
            UnityEngine.Debug.unityLogger.filterLogType = LogType.Warning;
#else
            UnityEngine.Debug.logger.filterLogType = LogType.Warning;
#endif
#endif
        //LOG.Error("Log.Init()");
        switch (GameSettings.RenderQuality)
        {
            case Framework.enGameRenderQuality.eLow:
                QualitySettings.antiAliasing = 0;
                break;
            case Framework.enGameRenderQuality.eMedium:
                QualitySettings.antiAliasing = 2;
                break;
            case Framework.enGameRenderQuality.eHigh:
                QualitySettings.antiAliasing = 4;
                break;
        }
        //CUIManager.Instance.OpenFormSync(CUIID.Canvas_DebugShader, true, true);
        //GameData.Instance.LoadConfData();


        OnConfigLoadFinish();
        //PreLoad();
    }



    public override void OnStateLeave()
    {
        //EventRouter.Instance.RemoveEventHandler(CEventID.ON_CONFIG_LOAD_FNISHI, OnConfigLoadFinish);

        Dispatcher.removeEventListener(EventEnum.AgainGetAppConf, AgainGetAppConf);
        Dispatcher.removeEventListener(EventEnum.AgainGetAppConfIsMaintin, AgainGetAppConfIsMaintin);
        Dispatcher.removeEventListener<string>(EventEnum.MaintinWhileList, OnMaintinWhileList);
    }


    private void OnConfigLoadFinish()
    {
        //CUIManager.Instance.OpenFormImme(CUIID.Canvas_Ads);
        //CUIManager.Instance.OpenForm(CUIID.Canvas_Main, true);
        //AudioMgr.Instance.PlayBGM();


        // //跳转资源更新加载场景
        // CGameStateMgr.Instance.GotoState<CLoadingState>();
        // AndroidUtils.CloseSplash();
    

        LOG.Log("【启动流程】编号：1.------->开始向服务器获取 版本信息");
        //开始向服务器获取 版本信息
        this.GetVersionStart();

    }

    #region 获取版本信息

    /// <summary>
    /// 开始向游戏服务器获取 版本信息
    /// </summary>
    private void GetVersionStart()
    {
        Dispatcher.addEventListener(EventEnum.AgainGetAppConf, AgainGetAppConf);
        Dispatcher.addEventListener(EventEnum.AgainGetAppConfIsMaintin, AgainGetAppConfIsMaintin);
        Dispatcher.addEventListener<string>(EventEnum.MaintinWhileList, OnMaintinWhileList);

        TalkingDataManager.Instance.OpenApp(EventEnum.GetVersionStart);
#if UNITY_ANDROID
        GameHttpNet.Instance.SYSTEMTYPE = 1;
#elif UNITY_IOS
        GameHttpNet.Instance.SYSTEMTYPE = 2;
#endif
        Debug.Log("CLaunchState  GetVersion  curtime：" + DateTime.Now);
        GameHttpNet.Instance.GetVersion(GetVersionCallBack);
        UserDataManager.Instance.GetBookVersionInfo();


    }


    private void GetVersionCallBack(long responseCode, string result)
    {
        Debug.Log("【启动流程】编号：2.------->GetVersionCallBack获取versoin信息成功  responseCode" + responseCode + "result"+ result);
        LOG.Info("----GetVersionCallBack---->" + responseCode + "," + result);
        if (responseCode != 200)
        {
            TalkingDataManager.Instance.OpenApp(EventEnum.GetVersionResultFail);
            return;
        }
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            if (jo.code == 200)
            {

                //报告 获取版本信息成功
                TalkingDataManager.Instance.OpenApp(EventEnum.GetVersionResultSucc);

                UserDataManager.Instance.versionInfo = JsonHelper.JsonToObject<HttpInfoReturn<VersionInfo>>(result);
                if (UserDataManager.Instance.versionInfo != null && UserDataManager.Instance.versionInfo.data != null)
                {

                    //string curTime = UserDataManager.Instance.versionInfo.data.time;

                    Debug.Log("【启动流程】编号：3.------->进入IGG更新维护流程！");


                   //游戏版本号/资源版本号
                    string versionStr = UserDataManager.Instance.versionInfo.data.resource_version;
                    string dataversionStr = UserDataManager.Instance.versionInfo.data.database_version;
                    LOG.Info("客户端版本号为：" + SdkMgr.Instance.GameVersion());


#if ENABLE_DEBUG
                    if (GameDataMgr.Instance.UserLocalVersion)
                    {
                        UserDataManager.Instance.ResVersion = string.Format("{0}@{1}@{2}",SdkMgr.Instance.GameVersion(),GameDataMgr.Instance.LocalVersion,System.Environment.TickCount.ToString());
                    }
                    else
                    {
                        UserDataManager.Instance.ResVersion = string.Format("{0}@{1}@{2}",SdkMgr.Instance.GameVersion(),versionStr,System.Environment.TickCount.ToString());
                    }
                    if (GameDataMgr.Instance.UserLocalDataVersion)
                    {
                        UserDataManager.Instance.DataTableVersion = string.Format("{0}@{1}@{2}",SdkMgr.Instance.GameVersion(),GameDataMgr.Instance.LocalDataVersion,System.Environment.TickCount.ToString());
                    }
                    else
                    {
                        UserDataManager.Instance.DataTableVersion = string.Format("{0}@{1}@{2}",SdkMgr.Instance.GameVersion(),dataversionStr,System.Environment.TickCount.ToString());
                    }
#else
                        UserDataManager.Instance.ResVersion = string.Format("{0}@{1}",SdkMgr.Instance.GameVersion(), versionStr);
                        UserDataManager.Instance.DataTableVersion = string.Format("{0}@{1}",SdkMgr.Instance.GameVersion(), dataversionStr);
#endif
                    string[] addressArr = UserDataManager.Instance.versionInfo.data.resource_url.Split('@');
                    
                    //api地址
                    GameHttpNet.Instance.ServerAddress = addressArr[0];
                    //资源请求地址
                    GameHttpNet.Instance.AssetBundleAddress = addressArr[1];
                    
                    //进入IGG更新维护流程
                    this.IggUpateVersion(GameFrameworkImpl.Instance.severCurTime);
                }
            }
            else if (jo.code == 277)
            {
                int type = 1;
#if UNITY_IOS
        type = 2;
#elif UNITY_ANDROID
                type = 1;
#endif
               // GameHttpNet.Instance.SetClientError("api_getVersion", "param:type:" + type, (msg) => { });

                UIAlertMgr.Instance.Show("TIPS", jo.msg);
                return;
            }
        }
    }

    #endregion



    #region IGG更新维护流程  处理AppConf后台数据

    private bool isenter = false;

    //IGG更新维护流程
    private void IggUpateVersion(DateTime curtime)
    {
        isenter = true;

        //维护弹窗UI各功能具体要求要求如下：
        //1、维护面板是否挂出，需要同时读取appconfig中的【是否维护】、【维护开始时间】以及【维护结束时间】三个参数，     √
        //不能只读取【是否维护】或者只读取【维护开始 / 结束时间】。

        //2、【维护公告文字区】需要支持读取【维护开始 / 结束时间】并根据设备的时区，转化为设备本地时间，
        //比如，运营后台填写的是GMT - 5时间，当填写04 / 14 00:00时候，如果此时设备时区GMT + 8时，则需要相应显示为04 / 14 13:00。

        //3、【倒计时】结束以后，需要再从appconfig取一次【维护结束时间】，如果已经结束，则玩家自动进入游戏，
        //如果没有结束，则继续挂着维护面板，倒计时需要重新计算显示。

        //4、【维护公告文字区】需要支持读取appconfig中的维护补偿数量的参数，避免维护的时候，需要每一个gameid的维护补偿数量都要手动改一次。

        //5、维护面板结束的时间节点，需要与appconfig提供的时间做参照，不允许与设备本地时间做参照，否则，玩家可以通过调整设备时间，绕过维护面板。

        //6、Facebook粉丝墙，地址也可以配置在appconfig中，便于后续变更地址。

        //7、四大面板的优先级为：维护面板→强更面板→提更面板→热更→异常登录公告面板


        //是否维护 是否展示维护界面
        //0 不  1 是
        int maintainState = UserDataManager.Instance.appconfinfo.updateInfo.maintainInfo.state;
        //维护开始时间  "2019-08-08 05:00:00"
        string startAt = UserDataManager.Instance.appconfinfo.updateInfo.maintainInfo.startAt;
        //维护结束时间  "2019-09-04 07:35"
        string endAt = UserDataManager.Instance.appconfinfo.updateInfo.maintainInfo.endAt;
        //【游戏维护】公告信息
        string maintain = UserDataManager.Instance.appconfinfo.messagesInfo.content.maintain;


        //如果是维护状态
        if (maintainState == 1) // 【是维护状态】
        {
            //维护开始时间
            //字符串  "2019-08-08 05:00" 转成 Detetime格式
            DateTime mt_startAt = DateUtil.ConvertToDateTime(startAt + ":00");

            //维护结束时间
            //字符串  "2019-09-04 07:35" 转成 Detetime格式
            DateTime mt_endAt = DateUtil.ConvertToDateTime(endAt + ":00");

            //当前时间
            // DateTime curtime = DateTime.Now;
            //
            // Debug.LogError("mt_startAt（维护开始时间）:" + mt_startAt);
            // Debug.LogError("mt_endAt（维护结束时间）:" + mt_endAt);
            // Debug.LogError("curtime（当前时间）:" + curtime);

            if (DateTime.Compare(mt_startAt, curtime) >= 0)
            {
                //mt_startAt(维护开始时间) 比 curtime（当前时间）晚   【维护未开始】

                LOG.Log("【启动流程】编号：4.------->判断维护【维护未开始】！");


                //维护前  是否展示   系统公告
                 this.LoginBoxShow();

                //进入版本号校验
                // EnterCheckVersion();
            }
            else
            {
                //mt_startAt(维护开始时间) 比 curtime（当前时间）早    【已经维护】
                LOG.Log("【启动流程】编号：5.------->判断维护【已经维护】！");

                if (DateTime.Compare(mt_endAt, curtime) >= 0)
                {
                    //mt_endAt(维护结束时间) 比 curtime（当前时间）晚     【维护中】

                    if (!string.IsNullOrEmpty(InitFlowHelper._IGGId))
                    {
                        //获取白名单列表
                        UserDataManager.Instance.appconfinfo.loginboxInfo.UpdateWhiteList();
                        whiteList = UserDataManager.Instance.appconfinfo.loginboxInfo.MyWhiteList;
                        if (whiteList != null && whiteList.Count > 0)
                        {
                            for (int i = 0; i < whiteList.Count; i++)
                            {
                                if (whiteList[i] == InitFlowHelper._IGGId)   //白名单校验成功
                                {
                                    //进入版本号校验
                                    EnterCheckVersion();
                                    return;
                                }
                            }
                        }
                    }

                    //维护中，距离维护时间结束  剩余时间 (秒)
                    long laveTime = DateUtil.StampToDateTime2(mt_endAt, curtime);
                    //Debug.LogError("距离维护时间结束,剩余时间(秒):" + laveTime);
                    LOG.Log("【启动流程】编号：6.------->判断维护【维护中】！距离维护时间结束,剩余时间(秒):" + laveTime);

                 
                    //打开 系统维护中界面  //传入【游戏维护】公告信息
                    CUIManager.Instance.GetForm<UIUpdateModule>(UIFormName.UIUpdateModule)
                        .SetPanel(EnumUpdate.MaintenancePanel, maintain, laveTime.ToString());
                }
                else
                {
                    //mt_endAt(维护结束时间) 比 curtime（当前时间）早    【维护结束】
                    LOG.Log("【启动流程】编号：7.------->判断维护【维护结束】！进入版本号校验！");
                    //进入版本号校验
                    EnterCheckVersion();
                }
            }
        }
        else
        {
            //进入版本号校验
            EnterCheckVersion();
        }
    }

    #endregion



    #region 进入版本号校验


    /// <summary>
    /// 提更版本号
    /// </summary>
    public static string updateVersion = "";

    /// <summary>
    /// 进入版本号校验
    /// </summary>
    private void EnterCheckVersion()
    {

        Debug.Log("【启动流程】编号：7.7.------->进入版本号校验！");

        updateVersion = PlayerPrefs.GetString("updateVersion");
        if (updateVersion == null || updateVersion == "")
        {
            updateVersion = SdkMgr.Instance.GameVersion();
        }
        else
        {
        }


        //【强更】展示公告信息
        string forceUpdate = UserDataManager.Instance.appconfinfo.messagesInfo.content.forceUpdate;


        //强更版本  校验
        //强更版本号 Appconf配置
        string forceVersion = UserDataManager.Instance.appconfinfo.loginboxInfo.forceVersion;
        //判断是否要下载新版本
        bool isforceUpdate = this.CheckNeedDownNewVersion(forceVersion);


        // Debug.LogError("客户端版本号： " + SdkMgr.Instance.GameVersion());
        // Debug.LogError("强更版本号 Appconf配置 " + forceVersion);

        if (isforceUpdate == true)
        {
            LOG.Log("【启动流程】编号：8.------->判断维护【需要强更】！");
           // Debug.LogError("6............ 【需要强更】");

            //【需要强更】
            ////打开 维护完成 需要更新 界面 
            //CUIManager.Instance.OpenForm(UIFormName.UIUpdateModule);
            //打开 维护完成 需要更新 界面 
            CUIManager.Instance.GetForm<UIUpdateModule>(UIFormName.UIUpdateModule)
                .SetPanel(EnumUpdate.UpdateTipPanel, forceUpdate, "",1);


            TalkingDataManager.Instance.OpenApp(EventEnum.GoToStoreUpdate);
        }
        else
        {
            //【不需要强更】
            //资源热更并加载 进入游戏
            LOG.Log("【启动流程】编号：9.------->判断维护【不需要强更】！");

            //提更版本 校验
            //提更版本号 Appconf配置
            string version = UserDataManager.Instance.appconfinfo.loginboxInfo.version;
            // Debug.LogError("提更版本号  Appconf配置 " + version);
            //
            // Debug.LogError("自己的版本号：" + updateVersion);
            //【更新】展示公告信息
            string updateContent = UserDataManager.Instance.appconfinfo.messagesInfo.content.update;

            if (!isNewVersion(updateVersion,version))
            {
                LOG.Log("【启动流程】编号：10.------->判断维护【提更版本-需要更新】！");

                //提更版本【需要更新】

                ////打开 维护完成 需要更新 界面 
                //CUIManager.Instance.OpenForm(UIFormName.UIUpdateModule);

                //打开 维护完成 需要更新 界面 
                CUIManager.Instance.GetForm<UIUpdateModule>(UIFormName.UIUpdateModule)
                    .SetPanel(EnumUpdate.UpdateTipPanel, updateContent, "", 0);  //0【热更】  1强更
            }
            else
            {
                //【不需要提更    不需要更新】

                //我客户端 
                LOG.Log("【启动流程】编号：11.------->判断维护【不需要提更-不需要更新】！");
                // Debug.LogError("9............ 不需要提更    不需要更新");

                //是否展示 系统公告
                this.LoginBoxShow();
            }
        }


    }

    #endregion



    #region 是否显示登入公告

    //已经展示过公告吗？
    private bool isShow = false;
    public void LoginBoxShow()
    {
        //是否显示登入广告
        int showLoginPop = UserDataManager.Instance.appconfinfo.loginboxInfo.showLoginPop;
        //登录框 开始时间  "2019-08-01 17:55:54"
        string startTime = UserDataManager.Instance.appconfinfo.loginboxInfo.startTime;
        //登录框 结束时间  "2019-09-15 22:00:00"
        string endTime = UserDataManager.Instance.appconfinfo.loginboxInfo.endTime;


        //登录框 开始时间  
        //字符串  "2019-08-01 17:55:54" 转成 Detetime格式
        DateTime mt_startTime = DateUtil.ConvertToDateTime(startTime);

        //登录框 结束时间
        //字符串  "2019-09-15 22:00:00" 转成 Detetime格式
        DateTime mt_endTime = DateUtil.ConvertToDateTime(endTime);

        //当前时间
        // DateTime curtime = DateUtil.ConvertToDateTime(_curtime);
        DateTime curtime = GameFrameworkImpl.Instance.severCurTime;

        // Debug.LogError("mt_startTime"+ startTime);
        // Debug.LogError("mt_endTime" + endTime);
        // Debug.LogError("curtime" + curtime);

        

        if (showLoginPop == 1)
        {
            if (DateTime.Compare(mt_startTime, curtime) >= 0)
            {
                LOG.Log("【启动流程】编号：12.------->系统公告【未到展示时间】！");

                //Debug.LogError("mt_startTime【未到展示时间】");
                //mt_startTime(登录框 开始时间) 比 curtime（当前时间）晚   登录框系统公告【未到展示时间】


                //判断是否要进入热更新  强制提示面板  或者  直接进入资源加载
                this.HotfixTipOrEndterLoading();
            }
            else
            {
                //mt_startTime(登录框 开始时间) 比 curtime（当前时间）早   登录框系统公告【展示时间后】
                if (DateTime.Compare(mt_endTime, curtime) >= 0)
                {
                    //----------------【展示中】

                    LOG.Log("【启动流程】编号：13.------->系统公告【展示中】！");
                    //mt_endTime(登录框 结束时间) 比 curtime（当前时间）晚  【展示中】


                    if (isShow)
                    {
                        //如果已经展示过了 直接进入游戏

                        //判断是否要进入热更新  强制提示面板  或者  直接进入资源加载
                        this.HotfixTipOrEndterLoading();
                    }
                    else
                    {
                        isShow = true;

                        //【登录】异常提示内容 公告信息
                        string login = UserDataManager.Instance.appconfinfo.messagesInfo.content.login;

                        ////打开系统更新界面
                        //CUIManager.Instance.OpenForm(UIFormName.UIUpdateModule);

                        //打开 系统更新公告面板  //传入【游戏维护】公告信息
                        CUIManager.Instance.GetForm<UIUpdateModule>(UIFormName.UIUpdateModule)
                            .SetPanel(EnumUpdate.SystemNoticePanel, login);
                    }
                }
                else
                {
                    LOG.Log("【启动流程】编号：14.------->系统公告【展示结束】！");
                    //mt_endTime(登录框 结束时间) 比 curtime（当前时间）早  【展示结束】

                    //判断是否要进入热更新  强制提示面板  或者  直接进入资源加载
                    this.HotfixTipOrEndterLoading();
                }
            }
        }
        else
        {
            //判断是否要进入热更新  强制提示面板  或者  直接进入资源加载
            this.HotfixTipOrEndterLoading();
        }
    }


    #endregion



    #region 进入热更新 强制弹窗提示   或者直接进入加载资源

    private void HotfixTipOrEndterLoading()
    {
        //判断是否要进入热更新  强制提示面板
        if (GameFrameworkImpl.Instance.isHotfixRes() == true)
        {
            ////打开系统更新界面
            //CUIManager.Instance.OpenForm(UIFormName.UIUpdateModule);
            //打开 系统更新公告面板  //传入【游戏维护】公告信息
            CUIManager.Instance.GetForm<UIUpdateModule>(UIFormName.UIUpdateModule).SetPanel(EnumUpdate.UpdateHotfixTipPanel);
        }
        else
        {
            //【==========================进入资源更新加载场景==========================】
            CUIManager.Instance.CloseForm(UIFormName.UIUpdateModule);
            CSingleton<CGameStateMgr>.GetInstance().GotoState<CLoadingState>();
            AndroidUtils.CloseSplash();

            Debug.Log("CLaunchState  进入资源更新加载场景  curtime：" + DateTime.Now);
        }
    }

    #endregion



    #region 提更版本  是否是新版本


    /// <summary>
    /// 客户端是否是新版本
    /// </summary>
    /// <param name="curVersion"></param>
    /// <returns></returns>
    private bool isNewVersion(string ServerVersion)
    {
        string[] severVersion = ServerVersion.Split('.');
        int first = int.Parse(severVersion[0]);
        int second = int.Parse(severVersion[1]);
        int three = int.Parse(severVersion[2]);

        string[] clientVersion = SdkMgr.Instance.GameVersion().Split('.');

        bool _isNewVersion = true; //本地客户端 是否是新版本
        if (clientVersion.Length > 2)
        {
            if (first > int.Parse(clientVersion[0]))
                _isNewVersion = false;
            else if (first == int.Parse(clientVersion[0]) && second > int.Parse(clientVersion[1]))
                _isNewVersion = false;
            else if (first == int.Parse(clientVersion[0]) && second == int.Parse(clientVersion[1]) && three > int.Parse(clientVersion[2]))
                _isNewVersion = false;
        }

        return _isNewVersion;
    }



    /// <summary>
    /// 提更版本  客户端是否是新版本
    /// </summary>
    /// <param name="curVersion"></param>
    /// <returns></returns>
    private bool isNewVersion(string ClientVersion,string ServerVersion)
    {
        string[] severVersion = ServerVersion.Split('.');
        int first= int.Parse(severVersion[0]);
        int second = int.Parse(severVersion[1]);
        int three = int.Parse(severVersion[2]);

        string[] clientVersion = ClientVersion.Split('.');

        bool _isNewVersion = true; //本地客户端 是否是新版本
        if (clientVersion.Length > 2)
        {
            if (first > int.Parse(clientVersion[0]))
                _isNewVersion = false;
            else if (first == int.Parse(clientVersion[0]) && second > int.Parse(clientVersion[1]))
                _isNewVersion = false;
            else if (first == int.Parse(clientVersion[0]) && second == int.Parse(clientVersion[1]) && three > int.Parse(clientVersion[2]))
                _isNewVersion = false;
        }

        return _isNewVersion;
    }

    #endregion



    #region 强更版本 是否是新版本

    /// <summary>
    /// 判断是否要下载新版本
    /// </summary>
    /// <returns></returns>
    private bool CheckNeedDownNewVersion(string curVersion)
    {
        if (!string.IsNullOrEmpty(curVersion))
        {
            string[] versionArr = curVersion.Split('.');
            if (versionArr.Length > 1)
            {
                int firstFlag = int.Parse(versionArr[0]);
                int secondFlag = int.Parse(versionArr[1]);
                // int threeFlag = int.Parse(versionArr[2].Substring(0, 2));
                int threeFlag = int.Parse(versionArr[2]);

                string[] versionIndexList = SdkMgr.Instance.GameVersion().Split('.');
                if (versionIndexList.Length > 2)
                {
                    if (firstFlag > int.Parse(versionIndexList[0])) return true;
                    if (firstFlag == int.Parse(versionIndexList[0]) &&
                        secondFlag > int.Parse(versionIndexList[1]))
                        return true;
                    if (firstFlag == int.Parse(versionIndexList[0]) &&
                        secondFlag == int.Parse(versionIndexList[1]) &&
                        threeFlag > int.Parse(versionIndexList[2]))
                        return true;
                }
            }
        }
        return false;
    }

    #endregion



    #region 再次获取AppConf

    //是否正在获取
    private bool isGeting = false;

    public void AgainGetAppConf()
    {
        IGGAppConf conf = null;

        KungfuInstance.Get().GetPreparedAppConfLoader().Load(InitFlowHelper.appconf_name, (AppConfDelegate)((appconf, easternStandardTime) => {
            conf = appconf;

            LOG.Log("【启动流程】编号：20.------->维护倒计时end【再次请求获取AppConf】！curtime："+ easternStandardTime.StringValue);

            //Debug.LogError("AgainGetAppConf" + easternStandardTime.StringValue);

            //获取Conf数据
            string js = appconf.GetRawString();
            //解析json
            //缓存AppConf
            GameFrameworkImpl.Instance.CacheAppConf(js);

            //0 不  1 是
            int maintainState = UserDataManager.Instance.appconfinfo.updateInfo.maintainInfo.state;
           
            //【游戏维护】公告信息
            string maintain = UserDataManager.Instance.appconfinfo.messagesInfo.content.maintain;

            if (maintainState == 1) // 【是维护状态】
            {

                //维护结束时间  "2019-09-04 07:35"
                string endAt = UserDataManager.Instance.appconfinfo.updateInfo.maintainInfo.endAt;
                //字符串  "2019-09-04 07:35" 转成 Detetime格式
                DateTime mt_endAt = DateUtil.ConvertToDateTime(endAt + ":00");

                //当前时间  获得GMT-5时间
                DateTime curtime = DateUtil.ConvertToDateTime(easternStandardTime.StringValue);
                GameFrameworkImpl.Instance.severCurTime = curtime;
                // //当前时间
                // //获得当地时区 时间戳
                // long timeshap = easternStandardTime.Timestamp;
                // DateTime curtime = DateUtil.TimpstampToDateTime(timeshap);


                if (DateTime.Compare(mt_endAt, curtime) >= 0)
                {
                    //mt_endAt(维护结束时间) 比 curtime（当前时间）晚     【维护中】

                    //Debug.LogError("5............ 【维护中】");
            
                    //维护中，距离维护时间结束  剩余时间 (秒)
                    long laveTime = DateUtil.StampToDateTime2(mt_endAt, curtime);
                    //Debug.LogError("距离维护时间结束,剩余时间(秒):" + laveTime);
                    LOG.Log("【启动流程】编号：21.------->【维护中】！距离维护时间结束,剩余时间(秒):"+laveTime);


                    ////打开系统更新界面
                    //CUIManager.Instance.OpenForm(UIFormName.UIUpdateModule);

                    // //打开 系统维护中界面  //传入【游戏维护】公告信息
                    // CUIManager.Instance.GetForm<UIUpdateModule>(UIFormName.UIUpdateModule)
                    //     .SetPanel(EnumUpdate.MaintenancePanel, maintain, laveTime.ToString());

                    //打开 系统维护中界面  //传入【游戏维护】公告信息
                    UIUpdateModule uiForm = CUIManager.Instance.GetForm<UIUpdateModule>(UIFormName.UIUpdateModule);
                    if (uiForm != null)
                    {
                        uiForm.UpdateMaintinTime((int)laveTime);
                    }

                }
                else
                {
                    //mt_endAt(维护结束时间) 比 curtime（当前时间）早    【维护结束】

                    //进入版本号校验
                    EnterCheckVersion();
                }
            }
            else
            {
                //进入版本号校验
                EnterCheckVersion();
            }

            //返回成功回调
        }), (BackupAppConfDelegate)((appConfBackup, easternStandardTime, error) =>
        {
            //返回失败回调
            conf = appConfBackup?.appconf;
            //当前时间  获得GMT-5时间
            DateTime curtime = DateUtil.ConvertToDateTime(easternStandardTime.StringValue);
            string _error = error.GetSuggestion();

            LOG.Error("【启动流程】编号：22.------->维护倒计时end【再次请求获取AppConf】！请求失败：curtime：" + easternStandardTime.StringValue + " _error" + _error);
            //Debug.LogError("AgainGetAppConf" + conf + "  curtime" + curtime + " _error" + _error);
        }));

    }

    #endregion



    #region 再次获取Appconf 是否开始维护

    public void AgainGetAppConfIsMaintin()
    {
        IGGAppConf conf = null;
        KungfuInstance.Get().GetPreparedAppConfLoader().Load(InitFlowHelper.appconf_name, (AppConfDelegate)((appconf, easternStandardTime) => {

            LOG.Log("【启动流程】编号：30.------->系统公告OK按钮【再次请求获取AppConf 是否开始维护】！");

            //获取Conf数据
            string js = appconf.GetRawString();
            //解析json
            //缓存AppConf
            GameFrameworkImpl.Instance.CacheAppConf(js);


            //当前时间  获得GMT-5时间
            DateTime curtime = DateUtil.ConvertToDateTime(easternStandardTime.StringValue);
            GameFrameworkImpl.Instance.severCurTime = curtime;
            //重新走流程
            IggUpateVersion(curtime);


            //返回成功回调
        }), (BackupAppConfDelegate)((appConfBackup, easternStandardTime, error) =>
        {
            //返回失败回调
            string _error = error.GetSuggestion();
            LOG.Error("【启动流程】编号：62.------->维护倒计时end【再次请求获取AppConf】！请求失败：curtime：" + easternStandardTime.StringValue + " _error" + _error);


            //当前时间  获得GMT-5时间
            DateTime curtime = DateUtil.ConvertToDateTime(easternStandardTime.StringValue);
            GameFrameworkImpl.Instance.severCurTime = curtime;
            //重新走流程
            IggUpateVersion(curtime);

        }));




    }

    #endregion



    #region 维护白名单

    private List<string> whiteList;
    public void OnMaintinWhileList(string IGGId)
    {
        if (InitFlowHelper._IGGId == "" || isenter == false)
        {
            InitFlowHelper._IGGId = IGGId;
            return;
        }

        KungfuInstance.Get().GetPreparedAppConfLoader().Load(InitFlowHelper.appconf_name, (AppConfDelegate)((appconf, easternStandardTime) => {
            LOG.Log("【启动流程】编号：60.------->维护倒计时end【再次请求获取AppConf】！curtime：" + easternStandardTime.StringValue);
            //获取Conf数据
            string js = appconf.GetRawString();
            //解析json
            //缓存AppConf
            GameFrameworkImpl.Instance.CacheAppConf(js);
            //0 不  1 是
            int maintainState = UserDataManager.Instance.appconfinfo.updateInfo.maintainInfo.state;

            if (maintainState == 1) // 【是维护状态】
            {
                //维护结束时间  "2019-09-04 07:35"
                string endAt = UserDataManager.Instance.appconfinfo.updateInfo.maintainInfo.endAt;
                //字符串  "2019-09-04 07:35" 转成 Detetime格式
                DateTime mt_endAt = DateUtil.ConvertToDateTime(endAt + ":00");
                //当前时间  获得GMT-5时间
                DateTime curtime = DateUtil.ConvertToDateTime(easternStandardTime.StringValue);
                GameFrameworkImpl.Instance.severCurTime = curtime;

                if (DateTime.Compare(mt_endAt, curtime) >= 0)
                {
                    //mt_endAt(维护结束时间) 比 curtime（当前时间）晚     【维护中】

                    //获取白名单列表
                    UserDataManager.Instance.appconfinfo.loginboxInfo.UpdateWhiteList();
                    whiteList = UserDataManager.Instance.appconfinfo.loginboxInfo.MyWhiteList;
                    if (whiteList != null && whiteList.Count > 0)
                    {
                        for (int i = 0; i < whiteList.Count; i++)
                        {
                            if (whiteList[i] == InitFlowHelper._IGGId)   //白名单校验成功
                            {
                                //进入版本号校验
                                EnterCheckVersion();
                                return;
                            }
                        }
                    }
                }
             
            }
            //返回成功回调
        }), (BackupAppConfDelegate)((appConfBackup, easternStandardTime, error) =>
        {
            //返回失败回调
            string _error = error.GetSuggestion();
            LOG.Error("【启动流程】编号：68.------->维护白名单【再次请求获取AppConf】！请求失败：curtime：" + easternStandardTime.StringValue + " _error" + _error);
        }));
    }


    #endregion



}

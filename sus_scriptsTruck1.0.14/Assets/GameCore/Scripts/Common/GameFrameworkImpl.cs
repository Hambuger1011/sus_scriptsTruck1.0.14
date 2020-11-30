using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using IGG.SDK.Framework.VO;
using IGG.SDK.Modules.AppConf.VO;

namespace Framework
{
    using AB;
    using Framework;
    using Framework;
    using System.Collections;
    using System.Collections.Generic;
    using UniGameLib;
    using UnityEngine;
    using UGUI;
    //using Jing.ULiteWebView;

    public class GameFrameworkImpl : GameFramework
    {
        //public ULiteWebView WebView;
        [Header("日志输出到unity")]
        public bool debugForwardToUnity = true;
        public bool isShowDebugPanel = false;

        public int DefSerType = 1;
        public int DefResType = 1;
        
        public static new GameFrameworkImpl Instance { get; private set; }
        public static GameFrameworkImpl getInstance()
        {
            return GameFrameworkImpl.Instance;
        }

        protected override void Init()
        {
#if !UNITY_EDITOR
            debugForwardToUnity = false;
#endif
            Instance = this;
            base.Init();
        }

        protected override void PrepareGameSystem()
        {
            Initialize();
            SetSystemPreparedDone();
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            TearDown();

#if !UNITY_EDITOR && UNITY_IOS
            //用户登出操作 请求日志记录
            GameHttpNet.Instance.Logout((arg) =>
            {
                string result = arg.ToString();
                LOG.Info("----api_Logout---->" + result);
            });    
#endif
        }

        public void SetSystemPreparedDone()
        {
            this.m_isSystemPrepared = true;
            this.gameObject.AddComponent<UINetLoadingMgr>();
            GameDataMgr.Instance.DoLoginGames = false;
            CheckIsNewInstall();

#if ENABLE_DEBUG
            UnityEngine.Debug.unityLogger.logEnabled = true;
            CUIManager.Instance.OpenForm(UIFormName.ServiceSelectForm);
          
#else
          
#endif
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            CUIManager.Instance.Update();
            CTimerManager.Instance.Update();
            CGameObjectPool.Instance.Update();

#if CHANNEL_ONYX|| CHANNEL_SPAIN
            if (SdkMgr.Instance != null && SdkMgr.Instance.appsFlyer != null)
            {
                SdkMgr.Instance.appsFlyer.OnUpdate();
            }
#endif

            //if (Input.GetKeyDown(KeyCode.Keypad1))
            //{
            //    UserDataManager.Instance.CalculateKeyNum(10);
            //}
            //if (Input.GetKeyDown(KeyCode.Keypad2))
            //{
            //    UserDataManager.Instance.CalculateDiamondNum(10);
            //    GameDataMgr.Instance.table.ChangeBookDialogPath(2);
            //}
        }

        protected override void OnLateUpdate()
        {
            base.OnLateUpdate();
            CUIManager.Instance.LateUpdate();
        }

        public PhoneUtil.EnumPhoneLevel level = PhoneUtil.EnumPhoneLevel.Low;
        public void Initialize()
        {
            level=PhoneUtil.Instance.GetPhoneLevel();

            //IGGLog的开关
            IGG.SDK.Foundation.Config.ENABLE_LOG = false;
            
            TalkingDataManager.Instance.InitSdk();

            //=========================Init System Start=========================//
            GameHttpNet.CreateInstance();
            CTextManager.CreateInstance();
            //CSingleton<BuglySdkMgr>.CreateInstance();
            CSingleton<EventRouter>.CreateInstance();
            CSingleton<CTimerManager>.CreateInstance();
            CSingleton<AbAtlasMgr>.CreateInstance();
           
            CUIManager.CreateInstance();
            CGameStateMgr.CreateInstance();
            CGameObjectPool.CreateInstance();
            UnitySceneMgr.CreateInstance();
            GameDataMgr.CreateInstance();
            SdkMgr.CreateInstance();
            UIAlertMgr.CreateInstance();
            PurchaseRecordManager.CreateInstance();
            BookReadingWrapper.CreateInstance();

            //XLuaManager.Instance.StartGame();
            //=========================Init System End=========================//

            TalkingDataManager.Instance.SetAccountId(GameHttpNet.Instance.UUID);

#if CHANNEL_ONYX
            SdkMgr.Instance.firebase.Init();
            SdkMgr.Instance.firebase.SetUserId(GameHttpNet.Instance.UUID);



            //注册 IGG AppConf 主配置加载成功
            Dispatcher.addEventListener<IGGAppConf, IGGEasternStandardTime>(EventEnum.AppConfComplete, onAppConfComplete);

            //注册 IGG AppConf 主配置加载失败
            Dispatcher.addEventListener<IGGAppConf, IGGEasternStandardTime>(EventEnum.AppConfFail, onAppConfFail);

            Debug.Log("GameFrameworkImpl IGG初始化 curtime：" + DateTime.Now);

            IGGSDKManager.Instance.Init();

          

#endif
            //SdkMgr.Instance.SetJPushSDK(this.gameObject.GetComponent<JPushSdk>());
            SdkMgr.Instance.SetShareSDK(this.gameObject.GetComponent<ShareSdk>());
            //SdkMgr.Instance.SetWebView(WebView);

#if CHANNEL_HUAWEI
            SdkMgr.Instance.hwSDK.CheckUpdate();
#endif

            GameUtility.LoginSuccFlag = false;
            GameUtility.LoginConsumeTime = Time.time;
            //LOG.Error("----LoginConsumeTime---->"+GameUtility.LoginConsumeTime);

            //BTConfig.Init();
            //AndroidUtils.Init();
#if UNITY_IOS || UNITY_EDITOR
            IOSUtils.Init();
#endif

            //<------------解决https证书异常问题和https链接数量------------>
            //https解析的设置
            System.Net.ServicePointManager.DefaultConnectionLimit = 100;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3
                                                              | SecurityProtocolType.Tls
                                                              | SecurityProtocolType.Tls11
                                                              | SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
            Debug.Log("GameFrameworkImpl 初始化完成 curtime：" + DateTime.Now);

        }

        /// <summary>
        /// //<------------解决https证书异常问题和https链接数量------------>
        /// </summary>
        private bool RemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain, look at each error to determine the cause.
            // if (sslPolicyErrors != SslPolicyErrors.None)
            // {
            //     for (int i = 0; i < chain.ChainStatus.Length; i++)
            //     {
            //         if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
            //         {
            //             chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
            //             chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            //             chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
            //             chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
            //             bool chainIsValid = chain.Build((X509Certificate2)certificate);
            //             if (!chainIsValid)
            //             {
            //                 isOk = false;
            //             }
            //         }
            //     }
            // }
            return isOk;
        }

        public void TearDown()
        {
            TalkingDataManager.Instance.Dispose();
        
                RecordCurReadProgress();

                LOG.Error("InitGameSystem TearDown");
            //=========================Init System Start=========================//
            //Singleton<GameData>.DestroyInstance();
            CSingleton<EventRouter>.DestroyInstance();
            CSingleton<CTimerManager>.DestroyInstance();
            CSingleton<AbAtlasMgr>.DestroyInstance();
            CGameStateMgr.DestroyInstance();
            CUIManager.DestroyInstance();
           
            CGameObjectPool.DestroyInstance();
            UnitySceneMgr.DestroyInstance();
            GameDataMgr.DestroyInstance();
            //CSingleton<BuglySdkMgr>.DestroyInstance();
            SdkMgr.DestroyInstance();
            CTextManager.DestroyInstance();
            GameHttpNet.DestroyInstance();
            UIAlertMgr.DestroyInstance();
            BackgroundWorker.DestroyInstance();
            //=========================Init System End=========================//
            
        }

        private void RecordCurReadProgress()
        {
            //if (UserDataManager.Instance.UserData != null && DialogDisplaySystem.Instance.CurrentBookData != null)
            //{              
            //    GameHttpNet.Instance.SendPlayerProgress(UserDataManager.Instance.UserData.CurSelectBookID,
            //    DialogDisplaySystem.Instance.CurrentBookData.ChapterID, DialogDisplaySystem.Instance.CurrentBookData.DialogueID, 0
            //   , DialogDisplaySystem.Instance.CurrentBookData.PlayerDetailsID, 0, 0, string.Empty, 0, 0, 0, string.Empty, 0, RecordCurReadProgressHandler);
            //}
        }

        private void RecordCurReadProgressHandler(object arg)
        {
            string result = arg.ToString();
            Debug.Log("----RecordCurReadProgressHandler---->" + result);
        }

        private void CheckIsNewInstall()
        {

#if ENABLE_DEBUG
            GameDataMgr.Instance.ServiceType = DefSerType;
            GameDataMgr.Instance.ResourceType = DefResType;
#endif

            //TalkingDataManager.Instance.OpenApp("InitApp");
            int firstEnter = UserDataManager.Instance.UserData.IsFirstEnter;
           // Debug.Log("-----FirstEnter----->" + firstEnter);
            if(firstEnter == 0)
            {
                //UserDataManager.Instance.UserData.IsFirstEnter = 1;
                //UserDataManager.Instance.SaveUserDataToLocal();
                //TalkingDataManager.Instance.NewPlayerRecord(1);
            }
        }

        protected override void OnApplicationPause(bool pauseStatus)
        {
            base.OnApplicationPause(pauseStatus);
            if (SdkMgr.HasInstance())
            {
                SdkMgr.Instance.OnApplicationPause(pauseStatus);
            }
        }


        #region IGG AppConf 加载成功

        public  DateTime severCurTime;
        public bool isFirst = false;
        /// <summary>
        /// IGG AppConf 加载成功
        /// </summary>
        /// <param name="obj"></param>
        private void onAppConfComplete(IGGAppConf appInfo, IGGEasternStandardTime severTime)
        {
            if (isFirst == true) return;

            string js = appInfo.GetRawString();

            //缓存AppConf
            this.CacheAppConf(js);

            Debug.Log("GameFrameworkImpl IGG AppConf 加载成功 curtime：" + DateTime.Now);

            Debug.Log("severTime.StringValue:" + severTime.StringValue);

            severCurTime = DateUtil.ConvertToDateTime(severTime.StringValue);


            // //获得当地时区 时间戳
            // long timeshap = severTime.Timestamp;
            // severCurTime = DateUtil.TimpstampToDateTime(timeshap);
            // Debug.LogError("severTime.Timestamp:" + severCurTime);
            appConfigIsDone = true;
            GotoState();
            isFirst = true;
        }

        #endregion


        #region IGG AppConf 加载失败

        private void onAppConfFail(IGGAppConf appInfo, IGGEasternStandardTime severTime)
        {
            if (isFirst == true) return;
            Debug.Log("GameFrameworkImpl IGG AppConf 加载失败 curtime：" + DateTime.Now);
            
            if (severTime!=null)
            {
                Debug.LogError("severTime.StringValue:" + severTime.StringValue);
                severCurTime = DateUtil.ConvertToDateTime(severTime.StringValue);
                if (severCurTime == null)
                {
                    Debug.LogError("onAppConfFail severTime.severCurTime is Null");
                    UIAlertMgr.Instance.Show("Error", "AppConf loading Fail ;severTime.severCurTime is Null", AlertType.Sure, (isOK) =>
                    {
                        Application.Quit();
                    });
                }
            }
            else
            {
                Debug.LogError("onAppConfFail severTime.StringValue is Null");

            }

            if (appInfo == null)
            {
                //加载默认Appconf 配置
                UserDataManager.Instance.appconfinfo.DefaultInfo();
                appConfigIsDone = true;
                GotoState();
            }
            else
            {
                string js = appInfo.GetRawString();
                //解析json
                //缓存AppConf
                this.CacheAppConf(js);
                appConfigIsDone = true;
                GotoState();
            }
            isFirst = true;
        }

        #endregion
        
        #region 进入场景

        public bool appConfigIsDone = false;
        public bool autoLoginIsDone = false;

        public void GotoState()
        {
            //打开系统更新界面
            CUIManager.Instance.OpenForm(UIFormName.UIUpdateModule);

            if (!appConfigIsDone||!autoLoginIsDone)return;
#if ENABLE_DEBUG
            ServiceSelectForm selectForm = CUIManager.Instance.GetForm<ServiceSelectForm>(UIFormName.ServiceSelectForm);
            if (selectForm != null)
            {

                selectForm.SetCallBack(() =>
                {
                    //进入初始场景
                    CSingleton<CGameStateMgr>.GetInstance().GotoState<CLaunchState>();
                });
            }
            else
            {
                //进入初始场景
                CSingleton<CGameStateMgr>.GetInstance().GotoState<CLaunchState>();
            }
#else
            //进入初始场景
            CSingleton<CGameStateMgr>.GetInstance().GotoState<CLaunchState>();
#endif
        }
        #endregion


        #region 强更跳转下载

        /// <summary>
        /// //跳转下载
        /// </summary>
        public void GoToUpdate()
        {
            string linkUrl = "";
#if CHANNEL_HUAWEI
                                SdkMgr.Instance.hwSDK.CheckUpdate();
                                linkUrl = "https://appgallery.cloud.huawei.com/marketshare/app/C"+ HuaweiSdk.HUAWEI_APP_ID;
#else

#if UNITY_ANDROID
            linkUrl = "https://play.google.com/store/apps/details?id=" + SdkMgr.packageName;
#endif
#if UNITY_IOS
                                linkUrl = "itms-apps://itunes.apple.com/cn/app/id" + SdkMgr.IosAppId;
#endif
            Application.OpenURL(linkUrl);
#endif
        }

        #endregion


        #region 是否热更新

        public bool isHotfixRes()
        {
            bool isHotfixUpdate = false;

            //当前版本号；
            string curResVersion = PlayerPrefs.GetString("CurResVersion");

            if (string.IsNullOrEmpty(curResVersion))
            {
                curResVersion = "1.0.0";
            }
            else
            {
                string oldResVersion = PlayerPrefs.GetString("OldResVersion");
                if (string.IsNullOrEmpty(oldResVersion))
                {

                }
                else
                {
                    string[] curResVersionArr = curResVersion.Split('.');
                    int first = int.Parse(curResVersionArr[0]);
                    int second = int.Parse(curResVersionArr[1]);
                    int three = int.Parse(curResVersionArr[2]);

                    string[] OldResVersionArr = oldResVersion.Split('.');

                    if (curResVersionArr.Length > 2)
                    {
                        if (first > int.Parse(OldResVersionArr[0]))
                            isHotfixUpdate = true;
                        else if (first == int.Parse(OldResVersionArr[0]) && second > int.Parse(OldResVersionArr[1]))
                            isHotfixUpdate = true;
                        else if (first == int.Parse(OldResVersionArr[0]) && second == int.Parse(OldResVersionArr[1]) && three > int.Parse(OldResVersionArr[2]))
                            isHotfixUpdate = true;
                    }
                }
            }

            if (isHotfixUpdate == false)
            {
                if (!string.IsNullOrEmpty(curResVersion))
                {
                    PlayerPrefs.SetString("OldResVersion", curResVersion);
                }
            }
            return isHotfixUpdate;
        }

        #endregion


        #region 切入切出游戏

        protected override void OnApplicationFocus(bool isFocus)
        {
            base.OnApplicationFocus(isFocus);

            if (isFocus == false)
            {
                LOG.Info("切出游戏时......................");

                if (XLuaManager.Instance != null && XLuaManager.Instance.GetLuaEnv() != null)
                {
                    XLuaManager.Instance.GetLuaEnv().DoString(@"GameHelper.OnGameleave()");
                }

#if UNITY_ANDROID || UNITY_IOS

#endif

            }
            else
            {
                LOG.Info("切回游戏时......................");
                if (XLuaManager.Instance != null && XLuaManager.Instance.GetLuaEnv() != null)
                {
                    XLuaManager.Instance.GetLuaEnv().DoString(@"GameHelper.OnGameback()");
                }


            }

        }

        #endregion


        #region 缓存AppConf

        public void CacheAppConf(string js)
        {
            //解析json
            UserDataManager.Instance.appconfinfo = JsonHelper.JsonToObject<AppconfInfo>(js);
            //内容配置 如果为null
            if (UserDataManager.Instance.appconfinfo.messagesInfo == null)
            {
                UserDataManager.Instance.appconfinfo.messagesInfo.DefaultInfo();
            }
            //维护白名单
            UserDataManager.Instance.appconfinfo.loginboxInfo.UpdateWhiteList();
        }

        #endregion




    }
}

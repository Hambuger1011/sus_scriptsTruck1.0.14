using System.Collections.Generic;
using ADTracking;
using Framework;
using Helper.Account;
using Helpers;
using IGG.SDK;
using IGG.SDK.Core.Configuration;
using IGG.SDK.Core.Configuration.Enumeration;
using IGG.SDK.Core.Error;
using IGG.SDK.Core.Unity;
using IGG.SDK.Core.VO;
using IGG.SDK.Framework;
using IGG.SDK.Framework.Listener;
using IGG.SDK.Framework.VO;
using IGG.SDK.Modules.Account;
using IGG.SDK.Modules.Account.Guideline;
using IGG.SDK.Modules.Account.Guideline.VO;
using IGG.SDK.Modules.AppConf;
using IGG.SDK.Modules.AppConf.VO;
using IGG.SDK.Modules.Payment.VO;
using IGGUtils;
using Newtonsoft.Json.Linq;
using Script.Game.Helpers;
using UnityEngine;

namespace Helper.Login
{
    /// <summary>
    /// 游戏启动流程的封装（初始化IGGSDK->加载AppConf->自动登录->初始化支付->获取当前账号信息）
    /// </summary>
    public class InitFlowHelper : IGGGameDelegate, IGGInitializationListener, IGGSessionErrorListener, IGGQuickLoginListener, IGGLoadResultListener, IGGAdvertisingListener
    {
        private static InitFlowHelper instance;
        
        private OnInitFlowListener listener;
        private PaymentHelper.OnPayOrSubscribeToListener payOrSubscribeToListener;

        
        private Dictionary<int,IGGGameItem> shopListMap = new Dictionary<int,IGGGameItem>();

#if ENABLE_DEBUG
        public static string appconf_name = "test_config";
#else
        public static string appconf_name = "server_config";
#endif



        private InitFlowHelper()
        {
            // tracker = new ADTracker();
        }

        // //初始化流程（初始化 USKD、Appconf、登录、支付初始化）
        public void InitFlow(string gameId, OnInitFlowListener listener, PaymentHelper.OnPayOrSubscribeToListener payOrSubscribeToListener)
        {
            this.listener = listener;
            this.payOrSubscribeToListener = payOrSubscribeToListener;
            
            IGGMonoBehaviour iGGSDKMonoBehaviour = IGGSDKMain.gameObject;
            if (null == iGGSDKMonoBehaviour)
            {
                // 因为IGGSDK依赖到IGGSDKMonoBehaviour这个MonoBehaviour，所以需要将其挂载到一个gameobject上，具体请看IGGSDKMain.Init。
                Debug.LogAssertion("请先初始化预制件 IGGSDK 或将 IGGSDKMonoBehaviour 脚本绑定在 gameobject 中");
                return;
            }
            Debug.Log("start InitFlow.");
            
            // 初始化IGGSDK
            InitIGGSDK(gameId);
        }

        //1.初始化 IGGSDK
        public void InitIGGSDK(string gameId)
        {
            Debug.Log("1.初始化 IGGSDK");   
            KungfuInstance.Reset(gameId); // 先对KungfuInstance反初始化，Reset接口内部自己会判断是不是已经初始化过了
            IGGConfigurationProvider provider = new DemoConfigurationProvider(gameId, IGGSDKMain.gameObject, this); // USDK配置提供器，研发请配置好各参数
            KungfuInstance.SetConfigurationProvider(provider);
            // 设置会话错误监听（session过期、无效、封号等错误）。
            KungfuInstance.Get().SetSessionErrorListener(this);
            // 设置广告事件触发监听
            KungfuInstance.Get().SetAdvertisingListener(this);
            
            // 通知IGGSDK初始化成功（配置完成），让UI层做出相应变更。
            listener?.onStageSuccess(Stage.INIT, null);
            // // 三星支付，进入沙盒模式
            // if (provider.PaymentServiceEnumeration() == IGGPaymentServiceEnumeration.Samsung)
            // {
            //     SamsungInAppPurchaseDebugHelper.DebugMode();
            // }

            // 这边的初始化会完成appconfig加载，研发请跟技术部确认，游戏的主配置名称是不是server_config（一般情况下都是这个）
            KungfuInstance.Get().Initialize(appconf_name, this);
        }

        /// <summary>
        ///  主配置加载成功
        /// </summary>
        /// <param name="primaryConfig"></param>
        /// <param name="serverTime">服务端时间（UTC -5），是从appconf中取得</param>
        public void OnInitialized(IGGAppConf primaryConfig, IGGEasternStandardTime serverTime)
        {
            string curTime = DateUtil.getCurrentDateTime();
            Debug.Log("IGGSDK AppConf主配置加载成功; 时间：" + curTime);
            //派发事件
            Dispatcher.dispatchEvent<IGGAppConf,IGGEasternStandardTime>(EventEnum.AppConfComplete, primaryConfig,serverTime);

            NotifyAppConfLoadedAndQuickLogin(primaryConfig);
        }

        /// <summary>
        /// 主配置加载失败
        /// <param name="primaryConfig">缓存的主配置，有可能为空，当不为空的时候
        /// ，研发可以根据IGGAppConfBackup.backupsTimeStamp时间（上次缓存时间戳）来决定要不要使用该缓存的主配置进游戏</param>
        ///
        /// <param name="clientDatetime">设备本地时间（UTC -5），因为这边返回的appconf是缓存的，所以没办法返回服务端时间，只能返回当前设备的时间</param>
        /// </summary>
        public void OnFailback(IGGAppConfBackup primaryConfig, IGGEasternStandardTime clientDatetime)
        {
            if (primaryConfig != null && primaryConfig.appconf != null)
            {
                NotifyAppConfLoadedAndQuickLogin(primaryConfig.appconf);

                //AppConf加载失败 派发事件
                Dispatcher.dispatchEvent<IGGAppConf, IGGEasternStandardTime>(EventEnum.AppConfFail, primaryConfig.appconf, clientDatetime);
            }
            else
            {
                //AppConf加载失败 派发事件
                Dispatcher.dispatchEvent<IGGAppConf, IGGEasternStandardTime>(EventEnum.AppConfFail, null, clientDatetime);

                listener?.onFailed?.Invoke(Stage.APPCONF, IGGError.Error("-1"));
                // 从IGGSDK那边取AppConf失败，请游戏那边返回游戏默认配置的AppConf。
                listener?.onLoadGameDefaultConfig?.Invoke(); 
                // 游戏获取到游戏默认配置的AppConf之后请继续执行登录的操作
                KungfuInstance.Get().QuickLogin(this);
            }
        }

        /// <summary>
        /// appconfig加载成功之后，执行QuickLogin（自动登录）
        /// </summary>
        /// <param name="primaryConfig"></param>
        private void NotifyAppConfLoadedAndQuickLogin(IGGAppConf primaryConfig)
        {
            // 从IGGSDK那边拿到AppConf（服务端）。
            listener?.onStageSuccess(Stage.APPCONF, primaryConfig);
            // 拿到AppConf之后执行登录的操作
            KungfuInstance.Get().QuickLogin(this);
        }
        //915725792
        public static string _IGGId = "";

        /// <summary>
        /// 自动登录成功
        /// </summary>
        /// <param name="session"></param>
        public void OnLoggedIn(IGGSession session)
        {
            //登录成功
            Debug.Log("当前用户 IGG ID:" + session.GetIGGId());
            // ！！！重要： 模拟向游戏服务器验证token是否有效，游戏那边请向游戏服务端验证session是否有效，不要复制这个逻辑。
            // AccountSessionUtils.SetTokenResultListener setTokenResultListener = delegate (bool isInvalid)
            // {
            //     if (isInvalid)
            //     {
            //         GameServerNotifySessionExpired();
            //     }
            // };
            //
            // AccountSessionUtils.SetToken(session.GetIGGId(), session.GetAccesskey(), setTokenResultListener);
            //

            //获取到IGGID 然后进入维护白名单校验
            _IGGId = session.GetIGGId();

            Dispatcher.dispatchEvent(EventEnum.MaintinWhileList, session.GetIGGId());

            //主动登录成功回调
            listener?.onStageSuccess(Stage.LOGIN, session);
            
            // 自动登录成功后，执行Startup(会初始化推送与获取点卡列表)
            KungfuInstance.Get().Startup(session, this);

            //AF事件 设置id
            AppsFlyerManager.Instance._IGGid = session.GetIGGId();
        }

        /// <summary>
        /// session过期的处理
        /// </summary>
        /// <param name="error"></param>
        /// <param name="previousSession"></param>
        public void OnSessionInvalidated(IGGError error, IGGAccountManagementGuideline guideline, IGGSession previousSession)
        {
            listener?.onSessionExpired.Invoke(previousSession);
        }

        /// <summary>
        /// 账号禁止操作的处理（如：封号）
        /// </summary>
        /// <param name="error"></param>
        /// <param name="previousSession"></param>
        public void OnBanned(IGGAccountManagementGuideline guideline, IGGSession previousSession)
        {
            listener?.onAccountNotAllowOperation?.Invoke();
        }

        /// <summary>
        /// 自动登录出错
        /// </summary>
        /// <param name="error"></param>
        public void OnError(IGGError error)
        {
            //登录失败
            listener?.onFailed?.Invoke(Stage.LOGIN, error);
        }
        
        /// <summary>
        /// 点卡第一次加载失败回调（这是内部还会重试，直到重试结束之后才会回调OnProductsLoaded），为什么要触发这个回调，因为怕内部重试时间太久，
        /// 导致研发那边一直等待，所以希望先告知研发点卡第一次已经加载失败了，研发自己可以根据这个回调来对UI层面做不同的处理。
        /// </summary>
        /// <param name="error"></param>
        public void OnProductsInitialLoadingFailed(IGGError error)
        {
           LOG.Error("点卡第一次加载失败回调:errorCode="+error.GetCode());
        }

        /// <summary>
        /// 点卡加载完成(可能成功，可能失败，看products是不是空值)
        /// </summary>
        /// <param name="products"></param>
        public void OnProductsLoaded(List<IGGGameItem> products)
        {
            LOG.Info("点卡加载完成:products="+products);
            PaymentHelper.SetGameItems(products);
            InitPayment();
        }
        
        

        /// <summary>
        ///Startup步骤完成之后，执行初始化支付
        /// </summary>
        private void InitPayment()
        {
            PaymentHelper.InitPayment(delegate(IGGError error)
            {
                Debug.Log("：" + error.GetCode());
                if (error.IsOccurred())
                {
                    //初始化失败 从 IGG 服务器获取价格信息，但是无法购买
                    listener?.onFailed?.Invoke(Stage.PAYMENT, error);
                }
                else
                {
                    listener?.onStageSuccess(Stage.PAYMENT, null);
                    listener.onSuccess();
                }
            },  payOrSubscribeToListener);
            
            EventDispatcher.Dispatch(EventEnum.IGGSdkInit);

        
        }
        
        public IGGGameCharacter GetGameCharacter()
        {
            var character = new IGGGameCharacter();
            character.SetCharId("mock charId"); // 当前玩家的角色ID。
            character.SetCharName("mock charName"); // 当前玩家的角色名称。
            character.SetLevel("mock charlevel"); // 当前玩家的角色等级。
            return character;
        }

        public Dictionary<string, string> GetCustomInfo()
        {
            return new Dictionary<string, string>(); // 游戏那边自定义参数，支付模块会用到。
        }

        public IGGGameServer GetGameServer()
        {
            var serverInfo = new IGGGameServer();
            serverInfo.SetLineId("mock lineId"); // 游戏服务器线路ID。
            serverInfo.SetServerId(0); // 游戏服务器ID。
            return serverInfo;
        }

        //！！！重要：game server 在使用 session 与 IGG 服务进行通信时出现 session 失效/过期情况时，需通知 game client, game client 调用此接口将本地 session 无效，避免之后使用无效的 session
        public void GameServerNotifySessionExpired()
        {
            // 游戏服务端告知当前seesion无效的时候，请调用以下接口无效本地的session。
            IGGSession.currentSession.InvalidateCurrentSession();
            //跟服务端验证发现服务端会话已失效，启动选择登录类型界面重新登录
            // ExpiredAccountLoginBehaviour.ShowUI();
        }
        
        //加载用户的账号安全信息
        public void LoadUserAccountSafety(OnLoadUserAccountSafetyListener listener)
        {
            Debug.Log("加载用户安全等级");
            
            // 获取IGGAccountManagementGuideline实例，因为IGGAccountManagementGuideline实例没有需要初始化的地方，所以
            // 可以直接调用Kungfu实例的GetPreparedAccountManagementGuideline方法获取IGGAccountManagementGuideline实例。
            var accountManagementGuideline = KungfuInstance.Get().GetPreparedAccountManagementGuideline();
            
            // 这个必须设置，对于这个CompatProxy的具体作用请参考IGGAccountManagerGuidelineMockCompatProxy类说明
            accountManagementGuideline.SetCompatProxy(new IGGAccountManagerGuidelineMockCompatProxy());
            
            //加载用户的账号安全信息
            accountManagementGuideline.LoadUser(delegate (IGGError error, IGGUserProfile userProfile)
            {
                if (error.IsNone())  // 成功
                {
                    listener?.onSuccess?.Invoke(userProfile); 
                }
                else // 失败
                {
                    listener?.onFailed?.Invoke(error);
                }
            });
        }

        public static InitFlowHelper SharedInstance()
        {
            if (instance == null)
            {
                instance = new InitFlowHelper();
            }

            return instance;
        }

        /// <summary>
        /// 在这个回调里初始化ADTracker
        /// </summary>
        public void ShouldInitialize()
        {
            Debug.Log("AD-ShouldInitialize");
            var customInfo = new JObject();
            customInfo.Add("g_id", IGGSDKManager.Instance.GetGameId());
            //tracker.Init(customInfo.ToString());

            //初始化AF
            AppsFlyerManager.Instance.InitAFtracker(customInfo.ToString());
        }

        /// <summary>
        /// 触发发送IGG_LAUNCH事件
        /// </summary>
        public void OnLaunch(IGGSession session)
        {
            Debug.Log("AD-OnLaunch");
            Dictionary<string, object> trackedParams = new Dictionary<string, object>();
            trackedParams.Add("userid", session.GetIGGId());
            // tracker.Track("IGG_LAUNCH", trackedParams);
           
            //AF事件 设置id
            AppsFlyerManager.Instance._IGGid = session.GetIGGId();

            //【AF事件记录*发送 每日登录事件】
            AppsFlyerManager.Instance.Send("IGG_LAUNCH", trackedParams);


            //绑定有礼红点  每日首次红点
            PlayerPrefs.SetInt("BindRedPoint", 1);
            //关注社媒有礼红点  每日首次红点
            PlayerPrefs.SetInt("FollowRedPoint", 1);
            //全书免费红点  每日首次红点
            PlayerPrefs.SetInt("FreeRedPoint", 1);

            //News标签
            PlayerPrefs.SetInt("NewsPanel", 1);
        }

        /// <summary>
        /// 安装事件各广告跟踪平台会自动收集，研发可以不关心该事件。
        /// </summary>
        /// <returns></returns>
        public bool OnInstall()
        {
            Debug.Log("AD-OnInstall");
            Dictionary<string, object> trackedParams = new Dictionary<string, object>();
            trackedParams.Add("userid", AppsFlyerManager.Instance._IGGid);

            // //AF事件记录*安装时间记录Install
            // AppsFlyerManager.Instance.Send("INSTALL", trackedParams);
            return true;
        }

        /// <summary>
        /// 发送SIGN_UP事件的时机
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public bool OnSignUp(IGGSession session)
        {
            Debug.Log("AD-OnSignUp");
            Dictionary<string, object> trackedParams = new Dictionary<string, object>();
            trackedParams.Add("userid", session.GetIGGId());
            //tracker.Track("SIGN_UP", trackedParams);

            //AF事件 设置id
            AppsFlyerManager.Instance._IGGid = session.GetIGGId();

            //AF事件记录*发送 SIGN_UP 事件  注册事件
            AppsFlyerManager.Instance.Send("SIGN_UP", trackedParams);


            //AF事件记录*发送 
            AppsFlyerManager.Instance.AndroidVersion();

            //AF事件记录*发送
            AppsFlyerManager.Instance.IOSVersion();

            //AF事件记录*发送 
            AppsFlyerManager.Instance.IOS_OS();

            //AF事件记录*发送 
            AppsFlyerManager.Instance.ANDROID_OS();
            return true;
        }

        /// <summary>
        /// 发送DAY2_RETENTION事件的时机
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public bool OnDayTwoRetension(IGGSession session)
        {
            Debug.Log("AD-OnDayTwoRetension");
            Dictionary<string, object> trackedParams = new Dictionary<string, object>();
            trackedParams.Add("userid", session.GetIGGId());
            //tracker.Track("DAY2_RETENTION", trackedParams);

            //AF事件记录*发送 次日登录事件
            AppsFlyerManager.Instance.Send("DAY2_RETENTION", trackedParams);
            return true; 
        }
    }

    public enum Stage
    {
        INIT,
        APPCONF,
        LOGIN,
        PAYMENT
    }

    /**
     *
     *  初始化流程每个节点的结果回调
     */
    public class OnInitFlowListener
    {
        public delegate void OnStageSuccess(Stage stage, object data);  //某个流程成功完成
        public delegate void OnSuccess(); //整个流程成功完成
        public delegate void OnFailed(Stage stage, IGGError error); //流程初始化失败

        public delegate void OnLoadGameDefaultConfig(); //返回默认AppConf
        public delegate void OnSessionExpired(IGGSession expiredSession);  //会话过期
        public delegate void OnAccountNotAllowOperation(); //当前账号不允许操作（被封号等）

        public OnStageSuccess onStageSuccess;
        public OnSuccess onSuccess;
        public OnFailed onFailed;
        //加载在线 AppConf 失败，并且 USDK 本地没有缓存 AppConf 时（首次进入游戏时出现）触发，游戏需要有一份默认配置，在 AppConf 加载失败时继续流程（登录、初始支付等流程）
        public OnLoadGameDefaultConfig onLoadGameDefaultConfig;
        //本地会话失效，启动选择登录类型界面重新登录
        public OnSessionExpired onSessionExpired;
        public OnAccountNotAllowOperation onAccountNotAllowOperation;

        public OnInitFlowListener(OnStageSuccess onStageSuccess, OnSuccess onSuccess, OnFailed onFailed, OnLoadGameDefaultConfig onLoadGameDefaultConfig, OnSessionExpired onSessionExpired, OnAccountNotAllowOperation onAccountNotAllowOperation)
        {
            this.onStageSuccess = onStageSuccess;
            this.onSuccess = onSuccess;
            this.onFailed = onFailed;
            this.onLoadGameDefaultConfig = onLoadGameDefaultConfig;
            this.onSessionExpired = onSessionExpired;
            this.onAccountNotAllowOperation = onAccountNotAllowOperation;
        }
    }

    /**
     *
     *  加载用户信息结果回调
     */
    public class OnLoadUserAccountSafetyListener
    {
        public delegate void OnSuccess(IGGUserProfile userProfile); //加载成功
        public delegate void OnFailed(IGGError error); //加载失败
        public OnSuccess onSuccess;
        public OnFailed onFailed;

        public OnLoadUserAccountSafetyListener(OnSuccess onSuccess, OnFailed onFailed)
        {
            this.onSuccess = onSuccess;
            this.onFailed = onFailed;
        }
    }

    public class DemoConfigurationProvider : IGGConfigurationProvider
    {
        private IGGMonoBehaviour monoBehaviour;
        private IGGGameDelegate gameDelegate;
        
        public DemoConfigurationProvider(string gameId, IGGMonoBehaviour monoBehaviour, IGGGameDelegate gameDelegate) : base(gameId)
        {
            this.monoBehaviour = monoBehaviour;
            this.gameDelegate = gameDelegate;
        }

        public override IGGMonoBehaviour MonoBehaviour()
        {
            return monoBehaviour;
        }

        public override IGGGameDelegate GameDelegate()
        {
            return gameDelegate;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Facebook.Unity;
using Framework;
using Helper.Account;
using Helper.EventCollection;
using Helper.Login;
using Helpers;
using IGG.SDK;
using IGG.SDK.Core.Configuration;
using IGG.SDK.Core.Error;
using IGG.SDK.Modules.Account;
using IGG.SDK.Modules.Account.Delegate;
using IGG.SDK.Modules.Account.Guideline.LoginScene;
using IGG.SDK.Modules.Account.Guideline.VO;
using IGG.SDK.Modules.Account.VO;
using IGG.SDK.Modules.AppConf;
using IGG.SDK.Modules.AppConf.VO;
using IGG.SDK.Modules.Compliance;
using IGG.SDK.Modules.Compliance.VO;
using IGG.SDK.Modules.EventCollection.VO;
using IGG.SDK.Modules.GeneralPayment.VO;
using IGG.SDK.Modules.Payment.Primary.VO;
using IGG.SDK.Modules.Payment.VO;
using IGG.SDK.Modules.RealNameVerification.VO;
using IGG.SDK.Modules.TSH.Wrapper;
using IGGUtils;
using Newtonsoft.Json.Linq;
using Script.Game.Helpers;
using UGUI;
using UnityEngine;
using XLua;
using Error = IGG.SDK.Modules.Account.Error;

[XLua.LuaCallCSharp]
public class IGGSDKManager : Singleton<IGGSDKManager>
{
    private InitFlowHelper initFlowHelper;
    private AccountHelper accountHelper;
    private string GameId = "11050102021";
    private OnLoadUserListener onLoadUserListener;
    private bool bindSuccess = false;
    private string SSOToken;
    private IGGAppConf _appconfig = new IGGAppConf();
    public LuaFunction bindCallBack;
    public Dictionary<string, string> UserInfo = new Dictionary<string, string>();
    public bool isNewUser = false;
    public bool isTokenExpired = false;
    public bool isSessionExpired = false;

    public void Init()
    {
        accountHelper = new AccountHelper(); // 简化账号业务实现的工具类。
        // 监听用户信息（第三方账号绑定情况与实名认证状态等，是否要加载实名认证相关信息，可以通过conf.json中的in_mainland_china属性进行配置）加载结果。
        onLoadUserListener =
            new OnLoadUserListener(OnLoadUserSuccess, OnLoadUserFailed, OnGuestBindState, OnBindInfo, OnUnbound);
#if UNITY_ANDROID
        GameId = "11050102021";
#else
        GameId = "11050103031";
#endif
        IGGSDKMain.Init(delegate()
        {
            initFlowHelper = InitFlowHelper.SharedInstance();
            Debug.Log("Application.dataPath:" + Application.dataPath);
            Debug.Log("Application.streamingAssetsPath:" + Application.streamingAssetsPath);
            Debug.Log("Application.temporaryCachePath:" + Application.temporaryCachePath);
            Debug.Log("Application.persistentDataPath:" + Application.persistentDataPath);
            initFlowHelper.InitFlow(GameId,
                new OnInitFlowListener(OnStageSuccess, OnSuccess, OnFailed, OnLoadGameDefaultConfig, OnSessionExpired,
                    OnAccountNotAllowOperation),
                new PaymentHelper.OnPayOrSubscribeToListener(OnPayOrSubscribeToSuccess, OnPayOrSubscribeToFailed,
                    OnPayOrSubscribeToStartingFailed
                    , OnPayOrSubscribeToFailedForHasSubscribe, OnPayOrSubscribeToCancel,
                    OnIGGSubscriptionShouldMakeRecurringPaymentsInsteadFail));
        });
    }

    void UpdateUserInfo(IGGUserProfile userProfile)
    {
        SetUserInfo("IGGID", userProfile.GetIGGId());
        SetUserInfo("LoginType", AccountUtil.GetLoginTypeValue(userProfile.GetLoginType()));

        var isAccountSafe = accountHelper.IsAccountSafety(userProfile); //当当前IGGID有绑定某一个第三方平台账号，都代表当前的IGGID是安全的。
        if (isAccountSafe)
        {
            SetUserInfo("isAccountSafe", "1");
        }
        else
        {
            SetUserInfo("isAccountSafe", "0");
        }

        IGGUserBindingProfile facebookProfile =
            accountHelper.GetBindMessage(userProfile.GetBindingProfiles(), IGGLoginType.Facebook); // 获取 FB 绑定信息。
        IGGUserBindingProfile iggAccountProfile =
            accountHelper.GetBindMessage(userProfile.GetBindingProfiles(), IGGLoginType.IGGAccount); // 获取通行证绑定信息。
        IGGUserBindingProfile gameCenterProfile =
            accountHelper.GetBindMessage(userProfile.GetBindingProfiles(),
                IGGLoginType.GameCenter); // 获取GameCenter绑定信息。
        IGGUserBindingProfile appleProfile =
            accountHelper.GetBindMessage(userProfile.GetBindingProfiles(), IGGLoginType.Apple); // 获取Apple 绑定信息。

        if (gameCenterProfile != null) //显示有绑定的情况。
        {
            SetUserInfo("GameCenterIsBind", "1");
            SetUserInfo("GameCenterBindInfo", gameCenterProfile.GetDisplayName());
        }
        else //显示未绑定的情况。
        {
            SetUserInfo("GameCenterIsBind", "0");
            SetUserInfo("GameCenterBindInfo", null);
        }

        if (appleProfile != null) //显示有绑定的情况。
        {
            SetUserInfo("AppleIsBind", "1");
            SetUserInfo("AppleBindInfo", appleProfile.GetDisplayName());
        }
        else //显示未绑定的情况。
        {
            SetUserInfo("AppleIsBind", "0");
            SetUserInfo("AppleBindInfo", null);
        }

        IGGUserBindingProfile googleProfile =
            accountHelper.GetBindMessage(userProfile.GetBindingProfiles(), IGGLoginType.GooglePlus); // 获取Google账号绑定信息。
        if (googleProfile != null) //显示有绑定的情况。
        {
            SetUserInfo("GoogleIsBind", "1");
            SetUserInfo("GoogleBindInfo", googleProfile.GetDisplayName());
        }
        else //显示未绑定的情况。
        {
            SetUserInfo("GoogleIsBind", "0");
            SetUserInfo("GoogleBindInfo", null);
        }

        if (facebookProfile != null) //显示有绑定的情况。
        {
            SetUserInfo("FBIsBind", "1");
            SetUserInfo("FBBindInfo", facebookProfile.GetDisplayName());
        }
        else //显示未绑定的情况。
        {
            SetUserInfo("FBIsBind", "0");
            SetUserInfo("FBBindInfo", null);
        }

        if (iggAccountProfile != null) //显示有绑定的情况。
        {
            SetUserInfo("IGGIsBind", "1");
            SetUserInfo("IGGBindInfo", iggAccountProfile.GetDisplayName());
        }
        else //显示未绑定的情况。
        {
            SetUserInfo("IGGIsBind", "0");
            SetUserInfo("IGGBindInfo", null);
        }

        if (bindSuccess)
        {
            bindSuccess = false;
            if (bindCallBack != null)
            {
                bindCallBack.Call();
                bindCallBack = null;
            }
        }
    }

    void SetUserInfo(string key, string value)
    {
        try
        {
            if (UserInfo.ContainsKey(key)) UserInfo.Remove(key);
            UserInfo.Add(key, value);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// 用户信息加载失败。
    /// </summary>
    /// <param name="error"></param>
    void OnLoadUserFailed(IGGError error)
    {
        LOG.Error("用户信息加载失败:errorCode=" + error.GetCode());
        UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(389));
    }

    /// <summary>
    /// 渲染设备绑定情况。
    /// </summary>
    /// <param name="guestBindState"></param>
    void OnGuestBindState(GuestBindState guestBindState)
    {
        switch (guestBindState)
        {
            case GuestBindState.NONE:
                //未绑定
                LOG.Info("未绑定");
                SetUserInfo("GuestIsBind", "0");
                SetUserInfo("GuestBindInfo", null);
                break;
            case GuestBindState.BIND_NO_CURRENT_DEVICE:
                //已绑定，但不是当前设备
                LOG.Info("已绑定（非当前设备）");
                SetUserInfo("GuestIsBind", "1");
                SetUserInfo("GuestBindInfo", "427");
                break;
            case GuestBindState.BIND_CURRENT_DEVICE:
                //已绑定，绑定当前设备
                LOG.Info("已绑定（当前设备）");
                SetUserInfo("GuestIsBind", "1");
                SetUserInfo("GuestBindInfo", "426");
                break;
            default: break;
        }
    }

    /// <summary>
    /// 渲染当前玩家已绑定第三方平台账号的视图（作为加载用户信息的一个结果回调处理）。
    /// </summary>
    /// <param name="loginType"></param>
    /// <param name="profile"></param>
    private void OnBindInfo(IGGLoginType loginType, IGGUserBindingProfile profile)
    {
        if (IGGLoginType.Facebook == loginType) // 已绑定FB
        {
            LOG.Info("已绑定FB" + profile);
        }
        else if (IGGLoginType.IGGAccount == loginType) // 已绑定IGG通行证
        {
            LOG.Info("已绑定IGG通行证" + profile);
        }
        else if (IGGLoginType.GameCenter == loginType) // 已绑定 GAMECENTER
        {
            LOG.Info("已绑定 GAMECENTER" + profile);
        }
        else if (IGGLoginType.Apple == loginType) // 已绑定 Apple Account
        {
            LOG.Info("已绑定 Apple Account" + profile);
        }
        else if (IGGLoginType.GooglePlus == loginType) // 已绑定Google账号
        {
            LOG.Info("已绑定Google账号" + profile);
        }
    }

    /// <summary>
    /// 渲染当前玩家未绑定第三方平台账号的视图（作为加载用户信息的一个结果回调处理）。
    /// </summary>
    /// <param name="loginType"></param>
    private void OnUnbound(IGGLoginType loginType)
    {
        if (IGGLoginType.Facebook == loginType) // 未绑定FB
        {
            LOG.Info("未绑定FB");
        }
        else if (IGGLoginType.IGGAccount == loginType) // 未绑定IGG通行证
        {
            LOG.Info("未绑定IGG通行证");
        }
        else if (IGGLoginType.GameCenter == loginType) // 未绑定GAMECENTER
        {
            LOG.Info("未绑定GAMECENTER");
        }
        else if (IGGLoginType.Apple == loginType) // 未绑定 Apple
        {
            LOG.Info("未绑定 Apple");
        }
        else if (IGGLoginType.GooglePlus == loginType) // 未绑定GAMECENTER
        {
            LOG.Info("未绑定GAMECENTER");
        }
    }

    /// <summary>
    /// 加载完成玩家用户信息的处理。
    /// </summary>
    /// <param name="userProfile"></param>
    void OnLoadUserSuccess(IGGUserProfile userProfile)
    {
        LOG.Info("加载完成玩家用户信息的处理");
        UpdateUserInfo(userProfile);
    }

    /// <summary>
    /// 该商品在这个谷歌（苹果）账号上以为其他IGGID订阅，请换成消耗类商品订阅
    /// </summary>
    /// <param name="message"></param>
    public void OnIGGSubscriptionShouldMakeRecurringPaymentsInsteadFail(string message)
    {
        //关闭转菊花
        UINetLoadingMgr.Instance.Close2();

        Debug.Log(message);
    }

    /// <summary>
    /// 支付或订阅的操作被取消
    /// </summary>
    /// <param name="purchase"></param>
    public void OnPayOrSubscribeToCancel(IGGIAPPurchase purchase)
    {
        //关闭转菊花
        UINetLoadingMgr.Instance.Close2();

        Debug.Log("取消购买。");
    }

    /// <summary>
    /// 已订阅某商品
    /// </summary>
    public void OnPayOrSubscribeToFailedForHasSubscribe()
    {
        //关闭转菊花
        UINetLoadingMgr.Instance.Close2();
        Debug.Log("购买失败，已经订阅该商品。");
    }

    /// <summary>
    /// 发起支付或订阅前期准备失败（一般时谷歌或Appstore支付服务有问题）
    /// </summary>
    /// <param name="error"></param>
    public void OnPayOrSubscribeToStartingFailed(IGGError error)
    {
        //关闭转菊花
        UINetLoadingMgr.Instance.Close2();

        if (StringHelper.IsEquals(error.GetCode(), "120316"))
        {
            IGGPurchaseRestriction purchaseRestriction = null;
            if (null != IGGRealnameVerificationConfig.SharedInstance().GetRestrictions())
            {
                Debug.LogError("限制配置不为空");
                purchaseRestriction =
                    IGGRealnameVerificationConfig.SharedInstance().GetRestrictions().PurchaseRestriction;
            }

            string tip = "购买失败。" + error.GetReadableUniqueCode();
            if (StringHelper.IsEquals(error.GetBaseErrorCode(),
                IGGPayResultCode.IGGPaymentErrorPurchaseLimitation + ""))
            {
                tip = purchaseRestriction != null ? purchaseRestriction.Tips : "限制消费（超出月额度）";
            }

            if (StringHelper.IsEquals(error.GetBaseErrorCode(),
                IGGPayResultCode.IGGPaymentErrorPurchaseLimitationForRunOutOfQuota + ""))
            {
                tip = purchaseRestriction != null ? purchaseRestriction.Tips : "限制消费（超出单笔额度）";
            }

            Debug.LogError(tip);
        }
        else if (StringHelper.IsEquals(error.GetCode(), "120315"))
        {
            Debug.LogError("检测限购情况失败，不能购买，请重试。");
        }
        else
        {
            Debug.LogError("购买失败。errorCode=" + error.GetCode());
        }
    }

    /// <summary>
    /// 支付或订阅失败
    /// </summary>
    /// <param name="error"></param>
    /// <param name="type"></param>
    /// <param name="purchase"></param>
    public void OnPayOrSubscribeToFailed(IGGError error, IGGPurchaseFailureType type, IGGIAPPurchase purchase)
    {
        Debug.LogError("购买失败。errorCode=" + error.GetCode());
        // ViewUtil.HideLoading();
        // ViewUtil.ShowIGGException("购买失败。", error);

        //关闭转菊花
        UINetLoadingMgr.Instance.Close2();

        //SdkMgr.Instance.google.CallPayEvent(false, error.GetCode());
        UIAlertMgr.Instance.Show("Tips", "购买失败。errorCode=" + error.GetCode());
    }

    /// <summary>
    /// 支付或订阅成功
    /// </summary>
    /// <param name="purchase"></param>
    /// <param name="result"></param>
    public void OnPayOrSubscribeToSuccess(IGGIAPPurchase purchase, IGGPurchaseResult result)
    {
        //关闭转菊花
        // UINetLoadingMgr.Instance.Close2();

        Debug.Log("OnPayOrSubscribeToSuccess 商品购买成功。");
        // ViewUtil.HideLoading();
        // Debug.LogError("购买成功。");
        Debug.LogError("购买成功。订单号：" + purchase.orderId + "    ItemID:" + result.GetItem().GetId());

#if ENABLE_DEBUG
        GameHttpNet.Instance.DevFinishOrder(purchase.orderId, result.GetItem().GetId(), "", DevFinishOrderCallBack);
#endif

        //SdkMgr.Instance.google.CallPayEvent(false, purchase.GetOrderId());
        //UIAlertMgr.Instance.Show("Tips","购买成功。"+ result.GetItem().GetId());

        //回调购买成功事件  像服务器请求 订单状态；获取刷新物品
        SdkMgr.Instance.OnPaySuccess(purchase.orderId, result.GetItem().GetId());

        //AF事件记录*首次充值 
        AppsFlyerManager.Instance.FIRST_PRUCHASE();
    }
    
    private void DevFinishOrderCallBack(object arg)
    {
        string result = arg.ToString();
        Debug.Log("----DevFinishOrder---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) => 
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.adsRewardResultInfo = JsonHelper.JsonToObject<HttpInfoReturn<GetAdsRewardResultInfo>>(result);
                    UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.adsRewardResultInfo.data.bkey);
                    UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.adsRewardResultInfo.data.diamond);
                }
            }, null);
        }
    }

    /// <summary>
    /// 判断是否已经拿到平台返回的商品信息
    /// </summary>
    /// <returns></returns>
    public bool HasProductList()
    {
        return PaymentHelper.GetShopListCount() > 0;
    }

    /// <summary>
    /// 支付调用接口
    /// </summary>
    /// <param name="productID"></param>
    public void PayItem(string productID)
    {
        IGGGameItem tempItem = PaymentHelper.GetGameItem(productID);
        if (tempItem != null)
        {
            //开启转菊花
            UINetLoadingMgr.Instance.Show2();

            Debug.LogError(
                "==BuyItem==>" + productID + "  itemId:" + tempItem.GetId() + " Title:" + tempItem.GetTitle());
            PaymentHelper.PayOrSubscribeTo(tempItem);
        }
    }

    /// <summary>
    /// 获取当前商品价格，显示
    /// </summary>
    /// <param name="productID"></param>
    /// <returns></returns>
    public string GetItemPrice(string productID)
    {
        IGGGameItem tempItem = PaymentHelper.GetGameItem(productID);
        if (tempItem != null)
        {
            LOG.Info("-----GetFormattedPrice--->>>>" + tempItem.GetPurchase().GetFormattedPrice());
            LOG.Info("-----GetCurrency--->>>>" + tempItem.GetPurchase().GetCurrency());
            LOG.Info("-----GetCurrencyDisplay--->>>>" + tempItem.GetPurchase().GetCurrencyDisplay());
            LOG.Info("-----GetPlatformPriceCurrencyCode--->>>>" +
                     tempItem.GetPurchase().GetPlatformPriceCurrencyCode());
            LOG.Info("-----GetUsdPrice--->>>>" + tempItem.GetPurchase().GetUsdPrice());
            LOG.Info("-----GetPlatformCurrencyPrice--->>>>" + tempItem.GetPurchase().GetPlatformCurrencyPrice());
            LOG.Info("-----GetPriceByCurrency--->>>>" +
                     tempItem.GetPurchase().GetPriceByCurrency(tempItem.GetPurchase().GetCurrency()));
            LOG.Info("-----GetPlatformOriginalCurrencyPrice--->>>>" +
                     tempItem.GetPurchase().GetPlatformOriginalCurrencyPrice());

#if UNITY_IOS
return tempItem.GetPurchase().GetPlatformPriceCurrencyCode() + tempItem.GetPurchase().GetPlatformCurrencyPrice();
#endif


#if UNITY_ANDROID
            return tempItem.GetPurchase().GetPlatformOriginalCurrencyPrice();
#endif


            return tempItem.GetPurchase().GetPlatformCurrencyPrice();
        }

        return "";
    }


    /// <summary>
    /// TshInit
    /// </summary>
    public void TshInit()
    {
        IGGImageLoaderHelper.InitImageLoader();
    }

    /// <summary>
    /// 打开TSH
    /// </summary>
    public void OpenTSH()
    {
        //埋点*联系我们
        GamePointManager.Instance.BuriedPoint(EventEnum.ContactUs);
        KungfuInstance.Get().GetPreparedTSHybrid().ShowPanel(delegate(IGGError error)
        {
            if (error.IsOccurred())
            {
                UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218),
                    "error:{" + error.GetCode() + "}");
            }
        });
    }

    /// <summary>
    /// 账号不允许操作（封号等）
    /// </summary>
    void OnAccountNotAllowOperation()
    {
        // ViewUtil.HideLoading();
        // //！！！重要:一般在游戏账号被封或者玩家IP被封时会触发这个回调，游戏要引导玩家去客服那边反馈这个问题。
        // ViewUtil.DialogConfig config = new ViewUtil.DialogConfig();
        // config.contentText = string.Format("无法登录该账号，请联系客服。");
        // config.showCancelBtn = false;
        // config.showConfirmBtn = true;
        // config.confirmClick = delegate (DialogBehaviour dialog)
        // {
        //     ViewUtil.HideDialog();
        // };
        // ViewUtil.ShowDialog(config);
        Debug.LogError("游戏账号被封或者玩家IP被封OnAccountNotAllowOperation");
    }

    /// <summary>
    /// session过期或无效
    /// </summary>
    /// <param name="expiredSession"></param>
    void OnSessionExpired(IGGSession expiredSession)
    {
        //本地会话失效，启动选择登录类型界面重新登录
        Debug.LogError("本地会话失效");
        isSessionExpired = true;
        // UIAlertMgr.Instance.Show("TIPS", CTextManager.Instance.GetText(327)
        //     , AlertType.Sure, (isOK) =>
        // {
        //     CUIManager.Instance.OpenForm(UIFormName.LoginForm);
        //     CUIManager.Instance.GetForm<LoginForm>(UIFormName.LoginForm).IsTimeOutOpenFanc();
        // }
        //     );
        // OnLastLoginTypeClick();客户端暂不自动进行登录操作，待服务端返回910再弹出重新登录界面
        //！！！重要:需要另行处理服务器端会话失效情况。game server 在使用 session 与 IGG 服务进行通信时出现 session 失效/过期情况时，
        // 需通知 game client, game client 调用此接口将本地 session 无效，避免之后使用无效的 session。
        // 服务器端会话失效时，要调用 InitFlowHelper.GameServerNotifySessionExpired()
        //KungfuInstance.Get().de
    }

    /// <summary>
    /// 对于首次加载appconf失败的时候，研发要自己从游戏那边获取一份默认的游戏配置（如：游戏服务端的ip跟端口配置信息），用于进游戏。
    /// </summary>
    void OnLoadGameDefaultConfig()
    {
        //Tips：加载游戏的默认配置（如：游戏服务端的ip跟端口配置信息）
    }

    private bool isFirstLogin = true;

    /// <summary>
    /// 在流程执行过程遇到会中断流程的错误会触发这个方法
    /// </summary>
    /// <param name="stage"></param>
    /// <param name="error"></param>
    void OnFailed(Stage stage, IGGError error)
    {
        //初始化流程中出现异常，显示错误、提供帮助 
        if (stage == Stage.APPCONF)
        {
            //Appconf 加载失败，使用游戏默认配置进入游戏
            Debug.LogError("主配置加载失败，游戏将以默认配置进入！errorCode=" + error.GetCode());
        }
        else if (stage == Stage.PAYMENT)
        {
            Debug.LogError("支付初始化失败。errorCode=" + error.GetCode());
        }
        else if (stage == Stage.LOGIN)
        {
            Debug.LogError("自动登录失败。errorCode=" + error.GetCode());
            if (isFirstLogin)
            {
                isFirstLogin = false;
                GameFrameworkImpl.Instance.autoLoginIsDone = true;
                GameFrameworkImpl.Instance.GotoState();
            }
        }
    }

    void OnStageSuccess(Stage stage, object data)
    {
        switch (stage)
        {
            case Stage.INIT: // SDK初始化完成
                OnInitIGGSDKComplete();
                break;
            case Stage.APPCONF: // APPCONF加载成功
                OnLoadAppConfSuccess((IGGAppConf) data);
                TshInit();
                break;
            case Stage.LOGIN: // 自动登录成功
                OnLoginSuccess((IGGSession) data);
                break;
            case Stage.PAYMENT: // 支付初始化成功
                OnInitPaymentSuccess();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// SDK初始化完成
    /// </summary>
    void OnInitIGGSDKComplete()
    {
        Debug.Log("IGGSDK 初始化完成");
    }

    /// <summary>
    /// APPCONF加载成功
    /// </summary>
    /// <param name="appconfig"></param>
    void OnLoadAppConfSuccess(IGGAppConf appconfig)
    {
        // 内容
        Debug.Log("AppConf " + appconfig.GetRawString());
        // config id
        Debug.Log("AppConf " + appconfig.GetId() + "");
        // 客户端的外网 IP
        Debug.Log("AppConf " + appconfig.GetClientIp());
        // 协议号
        Debug.Log("AppConf " + appconfig.GetProtocolNumber());
        // config 来源节点
        Debug.Log("AppConf " + appconfig.GetNode());
        // config 来源
        Debug.Log("AppConf " + appconfig.GetSource());
        // config 更新时间
        Debug.Log("AppConf " + appconfig.GetUpdateAt());
        //初始化 EC 模块
        EventCollectionHelper.SharedInstance().Init();

        DemoEvent<FlowData> demoEvent = new DemoEvent<FlowData>();
        demoEvent.SetName(EventBus.FLOW_EVENT);
        FlowData flowData = new FlowData();
        flowData.SetFlowType(FlowData.FlowType.LOAD_APPCONFIG);
        flowData.IsDone(true);
        demoEvent.SetData(flowData);
        Debug.Log("APPCONF加载成功");
        EventBus.Post(demoEvent);

        _appconfig = appconfig;
    }

    /// <summary>
    /// 获取评星状态
    /// </summary>
    public IGGAppRatingStatus GetRatingStatus()
    {
        if (_appconfig != null)
        {
            return _appconfig.GetAppRatingStatus();
        }

        return null;
    }


    /// <summary>
    /// 设置成标准模式
    /// </summary>
    public void SetRatingStatusStandard()
    {
        //IGG评星系统   获取评星系统的模式状态
        IGGAppRatingStatus rating = new IGGAppRatingStatus(IGGAppRatingMode.Standard);
        _appconfig.SetAppRatingStatus(rating);
        Debug.Log("APPCONF，IGGAppRatingMode ：" + rating.GetMode());
    }

    /// <summary>
    /// 设置成关闭模式
    /// </summary>
    public void SetRatingStatusDisable()
    {
        //IGG评星系统   获取评星系统的模式状态
        IGGAppRatingStatus rating = new IGGAppRatingStatus(IGGAppRatingMode.Disable);
        _appconfig.SetAppRatingStatus(rating);
        Debug.Log("APPCONF，IGGAppRatingMode ：" + rating.GetMode());
    }


    /// <summary>
    /// 自动登录成功
    /// </summary>
    /// <param name="session"></param>
    void OnLoginSuccess(IGGSession session)
    {
        session.DumpToLogCat();
        // IGGEvent eventValue = new IGGEvent("account.login", null);
        // EventCollectionHelper.SharedInstance().Push(eventValue, IGGEventEscalation.Level1);
        session.RequestSSOTokenForWeb(SetSSOToken);
        DemoEvent<FlowData> demoEvent = new DemoEvent<FlowData>();
        demoEvent.SetName(EventBus.FLOW_EVENT);
        FlowData flowData = new FlowData();
        flowData.SetFlowType(FlowData.FlowType.LOGIN);
        flowData.IsDone(true);
        demoEvent.SetData(flowData);
        Debug.Log("自动登录成功");
        SetUserInfo("IGGID", session.GetIGGId());
        SetUserInfo("LoginType", AccountUtil.GetLoginTypeValue(session.GetLoginType()));
        EventBus.Post(demoEvent);
        accountHelper.LoadUser(onLoadUserListener);

        UserDataManager.Instance.IGGid = session.GetIGGId();
        UserDataManager.Instance.Accesskey = session.GetAccesskey();

        if (isFirstLogin)
        {
            isFirstLogin = false;
            GameFrameworkImpl.Instance.autoLoginIsDone = true;
            GameFrameworkImpl.Instance.GotoState();
        }
    }

    /// <summary>
    /// 切换账号成功。
    /// </summary>
    /// <param name="session"></param>
    void OnSwitchLoginSuccess(IGGSession session)
    {
        //重新进入游戏
        Debug.Log("切换账号成功:IGGid=" + session.GetIGGId() + "LoginType=" + session.GetLoginType() + "Accesskey=" +
                  session.GetAccesskey());
        // IGGEvent eventValue = new IGGEvent("account.login", null);
        // EventCollectionHelper.SharedInstance().Push(eventValue, IGGEventEscalation.Level1);
        UserInfo.Clear();
        Init();
        DispatchLoginSuccMsg(session);
    }

    void GetFacebookInfo(LoginDataInfo loginInfo)
    {
        FB.API("me?fields=id,name,email", HttpMethod.GET, (result) =>
        {
            Dictionary<string, string> selfInfo = JsonHelper.JsonToObject<Dictionary<string, string>>(result.RawResult);
            string userNick = string.Empty;
            string email = string.Empty;
            string faceUrl = string.Empty;
            if (selfInfo != null)
            {
                if (selfInfo.ContainsKey("name"))
                    userNick = selfInfo["name"];
                loginInfo.UserName = userNick;
                loginInfo.Email = email;
                loginInfo.UserImageUrl = faceUrl;
                LOG.Warn("--- Self FB  --id-->" + loginInfo.UserId + " userNick:" + userNick + " email:" + email +
                         " faceUrl:" + faceUrl + " --token->" + UserDataManager.Instance.UserData.IdToken);
            }

            EventDispatcher.Dispatch(EventEnum.ThirdPartyLoginSucc, loginInfo);
        });
    }

    /// <summary>
    /// 自动登录。
    /// </summary>
    /// <param name="session"></param>
    public void AutoLogin()
    {
        //首次登陆 APPconf 加载成功 【锁】 
        GameFrameworkImpl.Instance.isFirst = false;

        UserInfo.Clear();
        Init();
        LoginDataInfo loginInfo = new LoginDataInfo();
        loginInfo.UserId = UserDataManager.Instance.IGGid;
        loginInfo.Token = UserDataManager.Instance.Accesskey;
        loginInfo.Email = "";
        loginInfo.UserName = "";
        loginInfo.UserImageUrl = "";
        loginInfo.OpenType = 1;
        isTokenExpired = false;
        ThirdPartyLoginSuccHandler(loginInfo,EnumReLogin.KickMaintain);
    }

    /// <summary>
    /// 发送登录成功消息。
    /// </summary>
    /// <param name="session"></param>
    void DispatchLoginSuccMsg(IGGSession session)
    {
        LoginDataInfo loginInfo = new LoginDataInfo();
        loginInfo.UserId = session.GetIGGId();
        loginInfo.Token = session.GetAccesskey();
        loginInfo.Email = "";
        loginInfo.UserName = "";
        loginInfo.UserImageUrl = "";
        loginInfo.OpenType = 1;

        // if (loginType == IGGLoginType.Facebook)
        // {
        //     GetFacebookInfo(loginInfo);
        // }
        // else
        // {
        if (isTokenExpired)
        {
            isTokenExpired = false;
            ThirdPartyLoginSuccHandler(loginInfo,EnumReLogin.SwitchAccount);
            return;
        }

        EventDispatcher.Dispatch(EventEnum.ThirdPartyLoginSucc, loginInfo);
        UserDataManager.Instance.SigningIn = false;
        // }
    }

    private void ThirdPartyLoginSuccHandler(LoginDataInfo loginInfo, EnumReLogin loginType)
    {
        if (loginInfo != null)
        {
            string userId = loginInfo.UserId;
            string tokenStr = loginInfo.Token;

            if (loginInfo.OpenType != 1) return;

            UserDataManager.Instance.UserData.UserID = userId;
            UserDataManager.Instance.UserData.IdToken = tokenStr;
            UserDataManager.Instance.UserData.UserName = loginInfo.UserName;


            if (UserDataManager.Instance.LoginInfo == null)
                UserDataManager.Instance.LoginInfo = new ThirdLoginData();

            // mCurLoginChannel = 2;

            LoginDataInfo glInfo = new LoginDataInfo();
            glInfo.UserId = userId;
            glInfo.UserName = loginInfo.UserName;
            glInfo.Email = loginInfo.Email;
            glInfo.Token = UserDataManager.Instance.UserData.IdToken;
            glInfo.UserImageUrl = loginInfo.UserImageUrl;
            UserDataManager.Instance.LoginInfo.ThirdPartyLoginInfo = glInfo;

            GameHttpNet.Instance.LoginByThirdParty(0, loginInfo.Email, loginInfo.UserName, loginInfo.UserImageUrl,
                userId, UserDataManager.Instance.UserData.IdToken, "android", LoginByThirdPartyCallBack, 1, loginType);
        }
    }

    private void LoginByThirdPartyCallBack(object arg, EnumReLogin loginType)
    {
        string result = arg.ToString();
        LOG.Info("----LoginByThirdPartyCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.thirdPartyLoginInfo =
                        JsonHelper.JsonToObject<HttpInfoReturn<ThirdPartyReturnInfo>>(result);
                    if (UserDataManager.Instance.thirdPartyLoginInfo != null &&
                        UserDataManager.Instance.thirdPartyLoginInfo.data != null)
                    {
                        UserDataManager.Instance.SigningIn = false;
                        GameHttpNet.Instance.TOKEN = UserDataManager.Instance.thirdPartyLoginInfo.data.token;
                        UserDataManager.Instance.SaveLoginInfo();
                        XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:CloseAll();");  //关闭所有Lua界面
                        CUIManager.Instance.CloseAllForm(); //关闭所有界面               

                        XLuaManager.Instance.mIsStartup = false;

                        switch (loginType)
                        {
                            case EnumReLogin.KickMaintain:
                                break;
                            case EnumReLogin.LoginForm:
                                XLuaManager.Instance.Startup();
                                break;
                            case EnumReLogin.SwitchAccount:
                                XLuaManager.Instance.Startup();
                                break;
                        }
                    }
                }
                else
                {
                    LOG.Error("登录失败重新登录");
                    OnLastLoginTypeClick();
                    return;
                }
            }, null);
        }
    }

    private void GetUserInfoCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetUserInfoCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            if (jo.code == 200)
            {
                UserDataManager.Instance.userInfo =
                    JsonHelper.JsonToObject<HttpInfoReturn<UserInfoCont>>(arg.ToString());

                if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data != null)
                {
                    if (UserDataManager.Instance.userInfo.data.userinfo.status != 0)
                    {
                        string msg = string.Empty;
                        if (UserDataManager.Instance.userInfo.data.userinfo.status == 1)
                        {
                            msg =
                                "Your account is being investigated due to unusual gameplay patterns.Please contact customer service for more information.";
                        }
                        else if (UserDataManager.Instance.userInfo.data.userinfo.status == 2)
                        {
                            msg =
                                "Your account has been flagged as illegal. You've been banned from the servers in Secrets.Please contact with customer support for more information.";
                        }

                        if (!string.IsNullOrEmpty(msg))
                        {
                            UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218) /*"TIPS"*/,
                                msg, AlertType.Sure, (value) =>
                                {
#if UNITY_EDITOR
                                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                                });
                            return;
                        }
                    }
                    else
                    {
                        UIAlertMgr.Instance.Show("TIPS", "Login successful");
                    }
                }
            }
        }
    }

    /// <summary>
    /// 支付初始化成功
    /// </summary>
    private void OnInitPaymentSuccess()
    {
        IGGSDKMain.gameObject.RunInMainThread(delegate()
        {
            DemoEvent<FlowData> demoEvent = new DemoEvent<FlowData>();
            demoEvent.SetName(EventBus.FLOW_EVENT);
            FlowData flowData = new FlowData();
            flowData.SetFlowType(FlowData.FlowType.INIT_PAY);
            flowData.IsDone(true);
            demoEvent.SetData(flowData);
            Debug.Log("支付初始化成功");
            EventBus.Post(demoEvent);
        });
    }

    /// <summary>
    /// 整个流程成功完成
    /// </summary>
    void OnSuccess()
    {
        // TODO: 本次QA那边暂时不测试防沉迷2.0
        if (false) // 这个根据游戏的gameid来决定要不要开启实名认证校验，研发那边请根据实际情况做配置。
        {
            IGGCompliance compliance = KungfuInstance.Get().GetPreparedCompliance();
            compliance.Check(new DemoComplianceCheckResultListener(OnPostponing, OnGuest, OnAdult, OnMinor, OnFail));
        }
        else
        {
            EnterGame();
        }
    }

    /// <summary>
    /// 防沉迷配置获取失败
    /// </summary>
    /// <param name="error"></param>
    private void OnFail(IGGError error)
    {
        LOG.Error("实名认证/反沉迷检查失败：errorCode=" + error.GetCode());
        //Demo 为演示功能，直接进入
        EnterGame();
    }

    /// <summary>
    /// （防沉迷总开关打开，检测到该IGGID是未成年人）
    /// </summary>
    /// <param name="status"></param>
    /// <param name="restrictions"></param>
    private void OnMinor(IGGComplianceStatus status, IGGComplianceRestrictions restrictions)
    {
        Debug.Log("OnMinor");
        PrintComplianceStatusInfo(status);
        PrintComplianceRestrictionsInfo(restrictions);
        //设置 Demo 中使用的数据（触发防沉迷的开关）。
        IGGRealnameVerificationConfig.SharedInstance().SetMinorData(status, restrictions);

        if (status.IsMinorsRestrictEnable && status.CheckViolation(restrictions.TimeRestriction))
        {
            //未成年用户由于时间段限制，禁止进入

            LOG.Error(restrictions.TimeRestriction.Tips);
            //Demo 为演示功能，直接进入
            EnterGame();
            return;
        }

        //（用于触发实际防沉迷的行为，比如超额不能购买、超时不能玩）。
        IGGRealnameVerificationConfig.SharedInstance().SetRestrictions(restrictions);

        //restrictions.getPurchase() 用于未成年人支付使用
        //restrictions.getTime() 用于未成年人游戏时间段
        //restrictions.getDuration() 用于未成年人游戏时长

        //Demo 为演示功能，直接进入
        EnterGame();
    }

    /// <summary>
    /// （防沉迷总开关打开，检测到该IGGID是成年人）
    /// </summary>
    /// <param name="status"></param>
    private void OnAdult(IGGComplianceStatus status) // 成年直接进游戏
    {
        Debug.Log("OnAdult");
        PrintComplianceStatusInfo(status);
        //设置 Demo 中使用的数据（触发防沉迷的开关）。
        IGGRealnameVerificationConfig.SharedInstance().SetAdultData(status);

        //允许进入游戏
        EnterGame();
    }

    /// <summary>
    /// （防沉迷总开关打开，检测到该IGGID是访客（未实名认证））
    /// </summary>
    /// <param name="status"></param>
    /// <param name="restrictions"></param>
    private void OnGuest(IGGComplianceStatus status, IGGComplianceRestrictions restrictions) // 已开启防沉迷，但未实名认证
    {
        Debug.Log("OnGuest");

        PrintComplianceStatusInfo(status);
        PrintComplianceRestrictionsInfo(restrictions);

        //设置 Demo 中使用的数据（触发防沉迷的开关）。
        IGGRealnameVerificationConfig.SharedInstance().SetGuestData(status, restrictions);
        //根据实名认证的状态进行相应的提醒
        if (status.IsRealNameVerificationEnable)
        {
            //根据实名认证 mode 执行实名认证指导
            ShowRealnameModeTips(status);
        }
        else
        {
            LOG.Error("未实名认证的用户，实名认证已关闭");
            //Demo 为演示功能，直接进入
            EnterGame();
        }
    }

    /// <summary>
    /// 打印ComplianceRestrictions，用于查看解析是否正确。
    /// </summary>
    /// <param name="restrictions"></param>
    private void PrintComplianceRestrictionsInfo(IGGComplianceRestrictions restrictions)
    {
        if (restrictions == null)
        {
            return;
        }

        try
        {
            StringBuilder sb = new StringBuilder("");
            if (restrictions.DurationRestriction != null)
            {
                sb.Append($"DurationRestriction.Duration:{restrictions.DurationRestriction.Duration},");
                sb.Append($"DurationRestriction.Rotation:{restrictions.DurationRestriction.Rotation},");
                sb.Append($"DurationRestriction.Tips:{restrictions.DurationRestriction.Tips},");
            }

            if (restrictions.TimeRestriction != null)
            {
                sb.Append($"TimeRestriction.End:{restrictions.TimeRestriction.End},");
                sb.Append($"TimeRestriction.Start:{restrictions.TimeRestriction.Start},");
                sb.Append($"TimeRestriction.Tips:{restrictions.TimeRestriction.Tips},");
            }

            if (restrictions.PurchaseRestriction != null)
            {
                sb.Append($"PurchaseRestriction.Monthly:{restrictions.PurchaseRestriction.Monthly},");
                sb.Append($"PurchaseRestriction.Single:{restrictions.PurchaseRestriction.Single},");
                sb.Append($"PurchaseRestriction.Tips:{restrictions.PurchaseRestriction.Tips},");
            }

            Debug.Log(sb.ToString());
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    /// <summary>
    /// 防沉迷相关（防沉迷总开关关闭，不限制）
    /// </summary>
    /// <param name="status"></param>
    private void OnPostponing(IGGComplianceStatus status)
    {
        Debug.Log("OnPostponing");

        PrintComplianceStatusInfo(status);

        //设置 Demo 中使用的数据（触发防沉迷的开关）。
        IGGRealnameVerificationConfig.SharedInstance().SetPostponingData(status);
        //根据实名认证的状态进行相应的提醒
        if (status.IsRealNameVerificationEnable)
        {
            //实名认证开启
            if (status.RealNameVerificationResult.State != IGGRealNameVerificationState.Authorized)
            {
                //游客身份-根据实名认证 mode 执行实名认证指导
                ShowRealnameModeTips(status);
                return;
            }
        }

        //Demo 为演示功能，直接进入
        EnterGame();
    }

    private void ShowRealnameModeTips(IGGComplianceStatus status)
    {
        //设置 Demo 中使用的数据（触发防沉迷的开关）。
        StringBuilder message = new StringBuilder("未实名认证的用户，请尽快实名认证!");
        //实名认证开启
        if (IGGRealNameMode.Gracefully == status.RealNameMode)
        {
            //不强制认证，只是提醒玩家进行认证
            message.Append("(非强制模式)");
        }
        else
        {
            //强制用户认证，试玩时间到达后无法继续游戏
            message.Append("(强制模式，试玩时间到达后无法继续游戏)");
        }

        LOG.Error(message.ToString());
        EnterGame();
    }

    /// <summary>
    /// 打印ComplianceStatus，用于查看解析是否正确。
    /// </summary>
    private void PrintComplianceStatusInfo(IGGComplianceStatus status)
    {
        if (status == null)
        {
            return;
        }

        StringBuilder sb = new StringBuilder("");
        sb.Append($"IsRealNameVerificationEnable:{status.IsRealNameVerificationEnable},");
        sb.Append($"RealNameMode:{status.RealNameMode.ToString()},");
        sb.Append($"IsGuestRestrictEnable:{status.IsGuestRestrictEnable},");
        sb.Append($"IsMinorsRestrictEnable:{status.IsMinorsRestrictEnable},");
        sb.Append($"ServerTime:{status.ServerTime},");
        sb.Append($"IGGRealNameVerificationResult.State:{status.RealNameVerificationResult.State.ToString()},");
        sb.Append($"IGGRealNameVerificationResult.IsMinor:{status.RealNameVerificationResult.IsMinor},");
        sb.Append($"IGGRealNameVerificationResult.Name:{status.RealNameVerificationResult.Name},");
        sb.Append($"IGGRealNameVerificationResult.IDCard:{status.RealNameVerificationResult.IDCard},");
        sb.Append(
            $"IGGRealNameVerificationResult.JuvenilesLevel:{status.RealNameVerificationResult.JuvenilesLevel.ToString()},");
        sb.Append($"IGGRealNameVerificationResult.IsLegalHoliday:{status.RealNameVerificationResult.IsLegalHoliday},");

        Debug.Log(sb.ToString());
    }

    /// <summary>
    /// 获取用户信息成功（在流程的最后一步会去获取该信息）
    /// </summary>
    /// <param name="userProfile"></param>
    void OnLoadUserAccountSafetySuccess(IGGUserProfile userProfile)
    {
        LOG.Info("获取用户信息成功");
        UpdateUserInfo(userProfile);
        DemoEvent<FlowData> demoEvent = new DemoEvent<FlowData>();
        demoEvent.SetName(EventBus.FLOW_EVENT);
        FlowData flowData = new FlowData();
        flowData.SetFlowType(FlowData.FlowType.TASK_ACCOUNT_SAFE_LV);
        flowData.IsDone(true);
        demoEvent.SetData(flowData);
        EventBus.Post(demoEvent);
    }

    /// <summary>
    /// 账号过期使用设备重新登录
    /// </summary>
    public void ExpiredDeviceLogin()
    {
        // 设备登录（登录前会检测当前设备是否已经绑定了IGGID，根据绑定情况会做相应登录操作）。
        accountHelper.CheckCandidateByGuest(
            new OnCheckCandidateByGuestListenr(OnUnbindByGuest, OnSwitchLoginSuccess, OnSwitchLoginFailed));
    }

    /// <summary>
    /// 账号过期使用FB重新登录
    /// </summary>
    public void ExpiredFacebookLogin()
    {
        FacebookUtil.Login(delegate(string token)
            {
                // FB登录（登录前会检测当前FB账号是否已经绑定了IGGID，根据绑定情况会做相应登录操作）。
                accountHelper.CheckCandidateByFacebookAccount(token,
                    new OnCheckCandidateByThirdAccountListenr(OnUnbindByThirdAccount, OnSwitchLoginSuccess,
                        OnSwitchLoginByThirdAccountFailed));
            },
            delegate() { UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(377)); OnCancel(); });
    }

    /// <summary>
    /// 账号过期使用IGG通行证重新登录
    /// </summary>
    public void ExpiredIGGAccountLogin()
    {
        // IGG通行证登录（登录前会检测当前IGG通行证账号是否已经绑定了IGGID，根据绑定情况会做相应登录操作）。
        accountHelper.CheckCandidateByIGGAccount(new OnCheckCandidateByIGGAccountListenr(OnUnbindByIGGAccount,
            OnSwitchLoginSuccess, OnSwitchLoginByIGGAccountFailed));
    }

    /// <summary>
    /// 账号过期使用GameCenter重新登录
    /// </summary>
    public void ExpiredGameCenterLogin()
    {
        GameCenterUtil.Login(delegate(string token)
            {
                // GameCenter登录（登录前会检测当前GameCenter账号是否已经绑定了IGGID，根据绑定情况会做相应登录操作）。
                accountHelper.CheckCandidateByGameCenter(token,
                    new OnCheckCandidateByThirdAccountListenr(OnUnbindByThirdAccount, OnSwitchLoginSuccess,
                        OnSwitchLoginByThirdAccountFailed));
            },
            delegate() { UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(379)); OnCancel();  });
    }

    /// <summary>
    /// 账号过期使用Apple重新登录
    /// </summary>
    public void ExpiredAppleLogin()
    {
        //Apple登录（登录前会检测当前Apple账号是否已经绑定了IGGID，根据绑定情况会做相应登录操作）。
        accountHelper.CheckCandidateByApple(new OnCheckCandidateByAppleListenr(OnUnbindByApple, OnSwitchLoginSuccess,
            OnSwitchLoginByAppleFailed));
    }

    /// <summary>
    /// 账号过期使用Google重新登录
    /// </summary>
    public void ExpiredGoogleLogin()
    {
        IGGNativeUtils.ShareInstance().FetchGooglePlayToken(((bool var1, string token) =>
        {
            if (var1)
            {
                // Google账号登录（登录前会检测当前Google账号是否已经绑定了IGGID，根据绑定情况会做相应登录操作）。
                accountHelper.CheckCandidateByGoogleAccount(token, IGGGoogleAccountTokenType.IdToken,
                    new OnCheckCandidateByThirdAccountListenr(OnUnbindByThirdAccount, OnSwitchLoginSuccess,
                        OnSwitchLoginByThirdAccountFailed));
            }
            else
            {
                UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(378)); OnCancel(); 
            }
        }));
    }

    /// <summary>
    /// 通过FB账号切换。
    /// </summary>
    public void FacebookLogin()
    {
        FacebookUtil.Login(delegate(string token)
            {
                // FB账号切换。
                accountHelper.SwitchLoginByFacebookAccount(token, new OnSwitchLoginByThirdAccountListener(
                    OnIGGIDSameAsNowByThirdAccount, OnSwitchLoginByThirdAccountUnbind,
                    OnSwitchLoginByThirdAccountBindDifIGGID, OnSwitchLoginByThirdAccountBindDifLoginType,
                    OnSwitchLoginByThirdAccountFailed));
            },
            delegate() { UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(377)); OnCancel();  });
    }

    /// <summary>
    /// Google 跟 iOS 平台等
    /// </summary>
    public void PrimaryLogin()
    {
#if UNITY_IOS
        GameCenterLogin();
#elif UNITY_ANDROID && !UNITY_EDITOR || IGGSDK_EDITOR_ANDROID
        GoogleLogin();
#endif
    }

    /// <summary>
    /// 绑定Google 跟 iOS 平台等
    /// </summary>
    public void PrimaryBind()
    {
#if UNITY_IOS
        BindGameCenter();
#elif UNITY_ANDROID
        BindGooglePlay();
#endif
    }

    /// <summary>
    /// 通过Google账号切换。
    /// </summary>
    public void GoogleLogin()
    {
        IGGNativeUtils.ShareInstance().FetchGooglePlayToken(((bool var1, string token) =>
        {
            if (var1)
            {
                // Google账号切换。
                accountHelper.SwitchLoginByGoogleAccount(token, IGGGoogleAccountTokenType.IdToken,
                    new OnSwitchLoginByThirdAccountListener(OnIGGIDSameAsNowByThirdAccount,
                        OnSwitchLoginByThirdAccountUnbind, OnSwitchLoginByThirdAccountBindDifIGGID,
                        OnSwitchLoginByThirdAccountBindDifLoginType, OnSwitchLoginByThirdAccountFailed));
            }
            else
            {
                UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(378)); OnCancel(); 
            }
        }));
    }

    /// <summary>
    /// 通过GameCenter账号切换。
    /// </summary>
    public void GameCenterLogin()
    {
        GameCenterUtil.Login(delegate(string token)
            {
                // GameCenter账号切换。
                accountHelper.SwitchLoginByGameCenter(token, new OnSwitchLoginByThirdAccountListener(
                    OnIGGIDSameAsNowByThirdAccount, OnSwitchLoginByThirdAccountUnbind,
                    OnSwitchLoginByThirdAccountBindDifIGGID, OnSwitchLoginByThirdAccountBindDifLoginType,
                    OnSwitchLoginByThirdAccountFailed));
            },
            delegate() { UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(379)); OnCancel();  });
    }

    /// <summary>
    /// 通过设备切换。
    /// </summary>
    public void DeviceLogin()
    {
        accountHelper.SwitchLoginByGuest(new OnSwitchGuestLoginListener(OnSwitchGuestLoginIGGIDSameAsNow,
            OnSwitchGuestLoginUnbind, OnSwitchGuestLoginBindDifIGGID, OnSwitchGuestLoginBindDifLoginType,
            OnSwitchLoginFailed));
    }

    /// <summary>
    /// 通过IGG通行证账号切换。
    /// </summary>
    public void IGGAccountLogin()
    {
        // IGG通行证账号切换。
        accountHelper.SwitchLoginByIGGAccount(new OnSwitchLoginByIGGAccountListener(OnIGGIDSameAsNowByIGGAccount,
            OnSwitchLoginByIGGAccountUnbind, OnSwitchLoginByIGGAccountBindDifIGGID,
            OnSwitchLoginByIGGAccountBindDifLoginType,
            OnSwitchLoginByIGGAccountFailed));
    }

    /// <summary>
    /// 通过Apple账号切换。
    /// </summary>
    public void AppleLogin()
    {
        // IGG通行证账号切换。
        accountHelper.SwitchLoginByApple(new OnSwitchLoginByIGGAppleListener(OnIGGIDSameAsNowByApple,
            OnSwitchLoginByAppleUnbind, OnSwitchLoginByAppleBindDifIGGID, OnSwitchLoginByAppleBindDifLoginType,
            OnSwitchLoginByAppleFailed));
    }

    /// <summary>
    /// 想切换的账号就是当前的账号（IGGID一致当时登录方式不一致），请游戏那边做相应提示，具体参考以下Demo。
    /// </summary>
    /// <param name="profile"></param>
    /// <param name="IGGID"></param>
    void OnSwitchLoginByThirdAccountBindDifLoginType(IGGThirdPartyAuthorizationProfile profile, string IGGID)
    {
        UIAlertMgr.Instance.Show("Tips", CTextManager.Instance.GetText(392) + $"{IGGID} " + "?", AlertType.SureOrCancel,
            (value) =>
            {
                //当前第三方平台账号已绑定到某个IGGID，将执行登录的操作。
                if (value)
                    accountHelper.LoginByThirdAccount(profile,
                        new OnLoginListener(OnSwitchLoginSuccess, OnSwitchLoginFailed));
                else
                    OnCancel();
            });
    }

    /// <summary>
    /// 想切换的账号就是当前的账号（登录方式跟IGGID一致），请游戏那边做相应提示，具体参考以下Demo。
    /// </summary>
    /// <param name="context"></param>
    /// <param name="IGGID"></param>
    void OnIGGIDSameAsNowByIGGAccount(IGGAccountLoginContext context, string IGGID)
    {
        UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(380));
    }


    /// <summary>
    /// 当通过IGG通行证账号切换游戏账号的时候，如果检测到当前IGG通行证账号还未绑定到某个IGGID，请游戏那边做相应的弹窗提示，具体可以参考以下Demo。
    /// </summary>
    /// <param name="context"></param>
    void OnSwitchLoginByIGGAccountUnbind(IGGAccountLoginContext context)
    {
        UIAlertMgr.Instance.Show("Tips", CTextManager.Instance.GetText(394), AlertType.SureOrCancel, (value) =>
        {
            //当前IGG通行证账号还未绑定到某个IGGID，将执行创建并登录的操作（自动绑定）。
            if (value)
                accountHelper.CreateAndLoginByIGGAccount(context,
                    new OnLoginListener(OnSwitchLoginSuccess, OnSwitchLoginFailed));
            else
                OnCancel();
        });
    }

    /// <summary>
    /// 当通过IGG通行证账号切换游戏账号的时候，如果检测到当前IGG通行证账号已绑定到某个IGGID（IGGID跟当前登录的IGGID不一致，请游戏那边做相应的弹窗提示，具体可以参考以下Demo。
    /// </summary>
    /// <param name="context"></param>
    /// <param name="IGGID"></param>
    void OnSwitchLoginByIGGAccountBindDifIGGID(IGGAccountLoginContext context, string IGGID)
    {
        UIAlertMgr.Instance.Show("Tips", CTextManager.Instance.GetText(392) + $"{IGGID} " + "?", AlertType.SureOrCancel,
            (value) =>
            {
                //当前IGG通行证账号已绑定到某个IGGID，将执行登录的操作。
                if (value)
                    accountHelper.LoginByIGGAccount(context,
                        new OnLoginListener(OnSwitchLoginSuccess, OnSwitchLoginFailed));
                else
                    OnCancel();
            });
    }

    /// <summary>
    /// 想切换的账号就是当前的账号（IGGID一致当时登录方式不一致），请游戏那边做相应提示，具体参考以下Demo。
    /// </summary>
    /// <param name="context"></param>
    /// <param name="IGGID"></param>
    void OnSwitchLoginByIGGAccountBindDifLoginType(IGGAccountLoginContext context, string IGGID)
    {
        UIAlertMgr.Instance.Show("Tips", CTextManager.Instance.GetText(392) + $"{IGGID} " + "?", AlertType.SureOrCancel,
            (value) =>
            {
                //当前IGG通行证账号已绑定到某个IGGID，将执行登录的操作。
                if (value)
                    accountHelper.LoginByIGGAccount(context,
                        new OnLoginListener(OnSwitchLoginSuccess, OnSwitchLoginFailed));
                else
                    OnCancel();
            });
    }

    /// <summary>
    /// 切换账号失败。
    /// </summary>
    /// <param name="error"></param>
    void OnSwitchLoginByIGGAccountFailed(IGGError error)
    {
        if (StringHelper.IsEquals(error?.GetCode(), Error.IGG_PASSPORT_AUTH_USER_CANCELED))
        {
            UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(313));
            OnCancel();
        }
        else
        {
            LOG.Error("切换账号失败errorCode=" + error.GetCode());
            UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(381));
            OnCancel();
        }
    }

    /// <summary>
    /// 想切换的账号就是当前的账号（登录方式跟IGGID一致），请游戏那边做相应提示，具体参考以下Demo。
    /// </summary>
    /// <param name="context"></param>
    /// <param name="IGGID"></param>
    void OnIGGIDSameAsNowByApple(IGGAppleLoginContext context, string IGGID)
    {
        UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(380));
        OnCancel();
    }

    /// <summary>
    /// 当通过Apple账号切换游戏账号的时候，如果检测到当前Apple账号还未绑定到某个IGGID，请游戏那边做相应的弹窗提示，具体可以参考以下Demo。
    /// </summary>
    /// <param name="context"></param>
    void OnSwitchLoginByAppleUnbind(IGGAppleLoginContext context)
    {
        UIAlertMgr.Instance.Show("Tips", CTextManager.Instance.GetText(395), AlertType.SureOrCancel, (value) =>
        {
            //当前Apple账号还未绑定到某个IGGID，将执行创建并登录的操作（自动绑定）。
            if (value)
                accountHelper.CreateAndLoginByApple(context,
                    new OnLoginListener(OnSwitchLoginSuccess, OnSwitchLoginFailed));
            else
                OnCancel();
        });
    }

    /// <summary>
    /// 当通过Apple账号切换游戏账号的时候，如果检测到当前Apple账号已绑定到某个IGGID（IGGID跟当前登录的IGGID不一致，请游戏那边做相应的弹窗提示，具体可以参考以下Demo。
    /// </summary>
    /// <param name="context"></param>
    /// <param name="IGGID"></param>
    void OnSwitchLoginByAppleBindDifIGGID(IGGAppleLoginContext context, string IGGID)
    {
        UIAlertMgr.Instance.Show("Tips", CTextManager.Instance.GetText(392) + $"{IGGID} " + "?", AlertType.SureOrCancel,
            (value) =>
            {
                //当前Apple账号已绑定到某个IGGID，将执行登录的操作。
                if (value)
                    accountHelper.LoginByApple(context, new OnLoginListener(OnSwitchLoginSuccess, OnSwitchLoginFailed));
                else
                    OnCancel();
            });
    }

    /// <summary>
    /// 想切换的账号就是当前的账号（IGGID一致当时登录方式不一致），请游戏那边做相应提示，具体参考以下Demo。
    /// </summary>
    /// <param name="context"></param>
    /// <param name="IGGID"></param>
    void OnSwitchLoginByAppleBindDifLoginType(IGGAppleLoginContext context, string IGGID)
    {
        UIAlertMgr.Instance.Show("Tips", CTextManager.Instance.GetText(392) + $"{IGGID} " + "?", AlertType.SureOrCancel,
            (value) =>
            {
                //当前Apple账号已绑定到某个IGGID，将执行登录的操作。
                if (value)
                    accountHelper.LoginByApple(context, new OnLoginListener(OnSwitchLoginSuccess, OnSwitchLoginFailed));
                else
                    OnCancel();
            });
    }

    /// <summary>
    /// 切换账号失败。
    /// </summary>
    /// <param name="error"></param>
    void OnSwitchLoginByAppleFailed(IGGError error)
    {
        if (StringHelper.IsEquals(error?.GetCode(), Error.APPLE_ACCOUNT_AUTH_USER_CANCELED))
        {
            UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(313));
        }
        else
        {
            LOG.Error("切换账号失败errorCode=" + error.GetCode());
            UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(381));
        }
    }

    /// <summary>
    /// 想切换的账号就是当前的账号（登录方式跟IGGID一致），请游戏那边做相应提示，具体参考以下Demo。
    /// </summary>
    /// <param name="IGGID"></param>
    private void OnSwitchGuestLoginIGGIDSameAsNow(string IGGID)
    {
        UITipsMgr.Instance.ShowTips(string.Format("IGG ID:{0} ", IGGID) + CTextManager.Instance.GetText(388));
    }

    /// <summary>
    /// 当通过设备切换游戏账号的时候，如果检测到当前设备还未绑定到某个IGGID，请游戏那边做相应的弹窗提示，具体可以参考以下Demo。
    /// </summary>
    private void OnSwitchGuestLoginUnbind()
    {
        UIAlertMgr.Instance.Show("Tips", CTextManager.Instance.GetText(390), AlertType.SureOrCancel, (value) =>
        {
            //当前设备还未绑定到某个IGGID，将执行创建并登录的操作（自动绑定）。
            if (value)
                accountHelper.CreateAndLoginByGuest(new OnLoginListener(OnSwitchLoginSuccess, OnSwitchLoginFailed));
            else
                OnCancel();
        });
    }

    /// <summary>
    /// 当通过设备切换游戏账号的时候，如果检测到当前设备已绑定到某个IGGID（IGGID跟当前登录的IGGID不一致），请游戏那边做相应的弹窗提示，具体可以参考以下Demo。
    /// </summary>
    /// <param name="IGGID"></param>
    private void OnSwitchGuestLoginBindDifIGGID(string IGGID)
    {
        UIAlertMgr.Instance.Show("Tips", CTextManager.Instance.GetText(392) + $"{IGGID} " + "?", AlertType.SureOrCancel,
            (value) =>
            {
                //当前设备已绑定到某个IGGID，将执行登录的操作。
                if (value) accountHelper.LoginByGuest(new OnLoginListener(OnSwitchLoginSuccess, OnSwitchLoginFailed));
                else
                    OnCancel();
            });
    }

    /// <summary>
    /// 想切换的账号就是当前的账号（IGGID一致但登录方式不一致），请游戏那边做相应提示，具体参考以下Demo。
    /// </summary>
    /// <param name="IGGID"></param>
    private void OnSwitchGuestLoginBindDifLoginType(string IGGID)
    {
        UIAlertMgr.Instance.Show("Tips", CTextManager.Instance.GetText(392) + $"{IGGID} " + "?", AlertType.SureOrCancel,
            (value) =>
            {
                //当前设备已绑定到某个IGGID，将执行登录的操作
                if (value) accountHelper.LoginByGuest(new OnLoginListener(OnSwitchLoginSuccess, OnSwitchLoginFailed));
                else
                    OnCancel();
            });
    }

    /// <summary>
    /// 切换第三方账号失败。
    /// </summary>
    /// <param name="error"></param>
    private void OnSwitchLoginByThirdAccountFailed(IGGError error)
    {
        LOG.Error("切换第三方账号失败errorCode=" + error.GetCode());
        UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(382));
    }

    /// <summary>
    /// 当通过第三方平台账号切换游戏账号的时候，如果检测到当前第三方平台账号已绑定到某个IGGID（IGGID跟当前登录的IGGID不一致，请游戏那边做相应的弹窗提示，具体可以参考以下Demo。
    /// </summary>
    /// <param name="profile"></param>
    /// <param name="IGGID"></param>
    private void OnSwitchLoginByThirdAccountBindDifIGGID(IGGThirdPartyAuthorizationProfile profile, string IGGID)
    {
        UIAlertMgr.Instance.Show("Tips", CTextManager.Instance.GetText(392) + $"{IGGID} " + "?", AlertType.SureOrCancel,
            (value) =>
            {
                //当前第三方平台账号已绑定到某个IGGID，将执行登录的操作。
                if (value)
                    accountHelper.LoginByThirdAccount(profile,
                        new OnLoginListener(OnSwitchLoginSuccess, OnSwitchLoginFailed));
                else
                    OnCancel();
            });
    }

    /// <summary>
    /// 想切换的账号就是当前的账号（登录方式跟IGGID一致），请游戏那边做相应提示，具体参考以下Demo。
    /// </summary>
    /// <param name="profile"></param>
    /// <param name="IGGID"></param>
    private void OnIGGIDSameAsNowByThirdAccount(IGGThirdPartyAuthorizationProfile profile, string IGGID)
    {
        UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(380));
    }

    /// <summary>
    /// 当通过第三方平台账号切换游戏账号的时候，如果检测到当前第三方平台账号还未绑定到某个IGGID，请游戏那边做相应的弹窗提示，具体可以参考以下Demo。
    /// </summary>
    /// <param name="profile"></param>
    private void OnSwitchLoginByThirdAccountUnbind(IGGThirdPartyAuthorizationProfile profile)
    {
        UIAlertMgr.Instance.Show("Tips", CTextManager.Instance.GetText(391), AlertType.SureOrCancel, (value) =>
        {
            //当前第三方平台账号还未绑定到某个IGGID，将执行创建并登录的操作（自动绑定）。
            if (value)
                accountHelper.CreateAndLoginByThirdAccount(profile,
                    new OnLoginListener(OnSwitchLoginSuccess, OnSwitchLoginFailed));
            else
                OnCancel();
        });
    }

    /**
     * 切换账号失败。
     */
    private void OnSwitchLoginFailed(IGGError error)
    {
        LOG.Error("切换账号失败errorCode=" + error.GetCode());
        UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(381));
        EventDispatcher.Dispatch(EventEnum.SwitchLoginFailed, error);
    }

    /**
     * 取消。
     */
    private void OnCancel()
    {
        LOG.Error("取消");
        EventDispatcher.Dispatch(EventEnum.SwitchLoginFailed);
    }

    /// <summary>
    /// 绑定FB操作。
    /// </summary>
    public void BindFacebook()
    {
        Debug.Log("Click Facebook bind");
        FacebookUtil.Login(delegate(string token) // 通过FB SDK拿到FB账号的token
            {
                // 通过FB账号的token，完成FB账号跟IGGID的绑定。
                accountHelper.BindByFacebookAccount(token, new OnBindListener(OnBindSuccess, OnBindFailed, OnBound),
                    onLoadUserListener);
            },
            delegate() // 通过FB SDK获取FB账号token失败
            {
                UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(377));
            });
        // SdkMgr.Instance.FacebookLogin(1);
    }

    /// <summary>
    /// 绑定IGG通行证操作。
    /// </summary>
    public void BindIGGAccount()
    {
        Debug.Log("Click IGGAccount bind");
        accountHelper.BindByIGGAccount(new OnBindListener(OnBindSuccess, OnBindFailed, OnBound), onLoadUserListener);
    }

    /// <summary>
    /// 绑定GameCenter操作。
    /// </summary>
    /// <param name="name"></param>
    /// <param name="btnTextString"></param>
    /// <param name="click"></param>
    /// <returns></returns>
    public void BindGameCenter()
    {
        Debug.Log("Click GameCenter bind");
        GameCenterUtil.Login(delegate(string token) // 通过GameCenter SDK拿到GameCenter账号的token
            {
                // 通过GameCenter账号的token，完成GameCenter账号跟IGGID的绑定。
                accountHelper.BindByGameCenter(token, new OnBindListener(OnBindSuccess, OnBindFailed, OnBound),
                    onLoadUserListener);
            },
            delegate() // 通过GameCenter SDK获取GameCenter账号token失败
            {
                UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(379));
            });
    }

    public void BindApple()
    {
        Debug.Log("Click SIA bind");
        accountHelper.BindByApple(new OnBindListener(OnBindSuccess, OnBindFailed, OnBound), onLoadUserListener);
    }

    /// <summary>
    /// 绑定Google账号操作。
    /// </summary>
    public void BindGooglePlay()
    {
        Debug.Log("Click Google bind");
        IGGNativeUtils.ShareInstance().FetchGooglePlayToken(((bool var1, string token) =>
        {
            if (var1) // 通过Google SDK拿到Google账号的token
            {
                // 通过Google账号的token，完成Google账号跟IGGID的绑定。
                accountHelper.BindByGoogleAccount(token, IGGGoogleAccountTokenType.IdToken,
                    new OnBindListener(OnBindSuccess, OnBindFailed, OnBound), onLoadUserListener);
            }
            else // 通过Google SDK获取Google账号token失败
            {
                UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(378));
            }
        }));
    }

    /// <summary>
    /// 第三方平台账号绑定成功提示。
    /// </summary>
    void OnBindSuccess()
    {
        bindSuccess = true;
        UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(383));
    }

    /// <summary>
    /// 第三方平台账号已绑定其他IGGID提示。
    /// </summary>
    /// <param name="IGGID"></param>
    void OnBound(string IGGID)
    {
        UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(387) + IGGID);
    }

    /// <summary>
    /// 第三方平台账号绑定失败提示。
    /// </summary>
    /// <param name="error"></param>
    void OnBindFailed(IGGError error)
    {
        if (StringHelper.IsEquals(error?.GetCode(), IGG.SDK.Modules.Account.Error.IGG_PASSPORT_AUTH_USER_CANCELED) ||
            StringHelper.IsEquals(error?.GetCode(), IGG.SDK.Modules.Account.Error.APPLE_ACCOUNT_AUTH_USER_CANCELED))
        {
            UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(313));
        }
        else
        {
            LOG.Error("绑定失败。" + error);
            UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(384));
            CUIManager.Instance.OpenForm(UIFormName.LoginForm);
            CUIManager.Instance.GetForm<LoginForm>(UIFormName.LoginForm).IsTimeOutOpenFanc();
        }
    }

    /// <summary>
    /// 获取用户信息失败（在流程的最后一步会去获取该信息）
    /// </summary>
    /// <param name="error"></param>
    void OnLoadUserAccountSafetyFailed(IGGError error)
    {
        LOG.Error("获取用户信息失败:errorCode=" + error.GetCode());
        // UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(385));
    }

    /// <summary>
    /// 进入游戏
    /// </summary>
    private void EnterGame()
    {
        Debug.Log("初始化流程成功。");
        //游戏逻辑处理（初始化、跳转场景等）
        //加载用户安全等级
        // initFlowHelper.LoadUserAccountSafety(
        //     new OnLoadUserAccountSafetyListener(OnLoadUserAccountSafetySuccess, OnLoadUserAccountSafetyFailed));

        // Demo用于模拟游戏被踢账号的场景，研发那边不能参考Demo这个实现，不清楚怎么实现的，可以问问其他游戏项目
        //     timer.Schedule(new CheckSessionTask(delegate(bool isInvalid)
        //     {
        //         Debug.Log("CheckSessionTask：" + isInvalid);
        //         if (isInvalid)
        //         {
        //             helper.GameServerNotifySessionExpired();
        //         }
        //     }), 20);
    }

    /// <summary>
    /// 登录成功。
    /// </summary>
    /// <param name="session"></param>
    private void OnLastLoginSuccess(IGGSession session)
    {
        //进入游戏
        LOG.Info("登录成功,可以进入游戏" + session);
        // IGGEvent eventValue = new IGGEvent("account.login", null);
        // EventCollectionHelper.SharedInstance().Push(eventValue, IGGEventEscalation.Level1);
        UserInfo.Clear();
        Init();
        session.RequestSSOTokenForWeb(SetSSOToken);
        accountHelper.LoadUser(onLoadUserListener);
        IGGLoginType loginType = session.GetLoginType();
        LoginDataInfo loginInfo = new LoginDataInfo();
        loginInfo.UserId = session.GetIGGId();
        loginInfo.Token = session.GetAccesskey();
        loginInfo.Email = "";
        loginInfo.UserName = "";
        loginInfo.UserImageUrl = "";
        loginInfo.OpenType = 1;

        ThirdPartyLoginSuccHandler(loginInfo,EnumReLogin.LoginForm);
    }

    /// <summary>
    /// 登录失败。
    /// </summary>
    /// <param name="error"></param>
    private void OnLastLoginFailed(IGGError error)
    {
        LOG.Error("OnLastLoginFailed登录失败errorCode=" + error.GetCode());
        UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(386));
        OnCancel();
    }

    /// <summary>
    /// 在执行登录过程中，如果检测到当前设备还未绑定到某个IGGID，请进行相应的弹窗提示，具体弹窗内容请参考该Demo。
    /// </summary>
    private void OnUnbindByGuest()
    {
        UIAlertMgr.Instance.Show("Tips", CTextManager.Instance.GetText(390), AlertType.SureOrCancel, (value) =>
        {
            //当前设备还未绑定到某个IGGID，将执行创建并登录的操作（自动绑定）。
            if (value)
                accountHelper.CreateAndLoginByGuest(new OnLoginListener(OnSwitchLoginSuccess, OnSwitchLoginFailed));
            else
                OnCancel();
        });
    }

    /// <summary>
    /// 在执行登录过程中，如果检测到当前第三方平台账号还未绑定到某个IGGID，请进行相应的弹窗提示，具体弹窗内容请参考该Demo。
    /// </summary>
    /// <param name="profile"></param>
    private void OnUnbindByThirdAccount(IGGThirdPartyAuthorizationProfile profile)
    {
        UIAlertMgr.Instance.Show("Tips", CTextManager.Instance.GetText(391), AlertType.SureOrCancel, (value) =>
        {
            //当前第三方平台账号还未绑定到某个IGGID，将执行创建并登录的操作（自动绑定）。
            if (value)
                accountHelper.CreateAndLoginByThirdAccount(profile,
                    new OnLoginListener(OnSwitchLoginSuccess, OnSwitchLoginFailed));
            else
                OnCancel();
        });
    }

    /// <summary>
    /// 在执行登录过程中，如果检测到当前IGG通行证账号还未绑定到某个IGGID，请进行相应的弹窗提示，具体弹窗内容请参考该Demo。
    /// </summary>
    /// <param name="context"></param>
    private void OnUnbindByIGGAccount(IGGAccountLoginContext context)
    {
        UIAlertMgr.Instance.Show("Tips", CTextManager.Instance.GetText(394), AlertType.SureOrCancel, (value) =>
        {
            //当前IGG通行证账号还未绑定到某个IGGID，将执行创建并登录的操作（自动绑定）。
            if (value)
                accountHelper.CreateAndLoginByIGGAccount(context,
                    new OnLoginListener(OnSwitchLoginSuccess, OnSwitchLoginFailed));
            else
                OnCancel();
        });
    }

    /// <summary>
    /// 在执行登录过程中，如果检测到当前Apple账号还未绑定到某个IGGID，请进行相应的弹窗提示，具体弹窗内容请参考该Demo。
    /// </summary>
    /// <param name="context"></param>
    private void OnUnbindByApple(IGGAppleLoginContext context)
    {
        UIAlertMgr.Instance.Show("Tips", CTextManager.Instance.GetText(395), AlertType.SureOrCancel, (value) =>
        {
            //当前IGG通行证账号还未绑定到某个IGGID，将执行创建并登录的操作（自动绑定）。
            if (value)
                accountHelper.CreateAndLoginByApple(context,
                    new OnLoginListener(OnSwitchLoginSuccess, OnSwitchLoginFailed));
            else
                OnCancel();
        });
    }

    /// <summary>
    /// 用上一次的登录方式重新登录游戏，登录时一般会触发三个场景：1、登录成功，重新进游戏。
    /// 2、登录失败，提示玩家再次试试。3、用的第三方账号还未绑定IGGID,提示玩家是否要新建个账号进入游戏。
    /// 具体请参考以下Demo的逻辑
    /// </summary>
    public void OnLastLoginTypeClick()
    {
        var lastLoginType = AccountUtil.ReadLastLoginType(); // 获取上次登录方式（本接口不是USDK提供，由游戏自行实现，USDKDemo仅供参考）
        if (lastLoginType == IGGLoginType.Guest) // 上次的登录方式是设备登录。
        {
            // 设备登录（登录前会检测当前设备是否已经绑定了IGGID，根据绑定情况会做相应登录操作）。
            accountHelper.CheckCandidateByGuest(
                new OnCheckCandidateByGuestListenr(OnUnbindByGuest, OnLastLoginSuccess, OnLastLoginFailed));
            return;
        }

        if (lastLoginType == IGGLoginType.GooglePlus) // 上次的登录方式是Google账号登录。
        {
            // 通过Google SDK获取谷歌账号的token。
            IGGNativeUtils.ShareInstance().FetchGooglePlayToken(((bool var1, string token) =>
            {
                if (var1) // 获取token成功
                {
                    // Google账号登录（登录前会检测当前Google账号是否已经绑定了IGGID，根据绑定情况会做相应登录操作）。
                    accountHelper.CheckCandidateByGoogleAccount(token, IGGGoogleAccountTokenType.IdToken,
                        new OnCheckCandidateByThirdAccountListenr(OnUnbindByThirdAccount, OnLastLoginSuccess,
                            OnLastLoginFailed));
                }
                else // 获取token失败
                {
                    UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(378));
                }
            }));
            return;
        }

        if (lastLoginType == IGGLoginType.Facebook) // 上次的登录方式是FB登录。
        {
            // 通过FB SDK获取FB账号的token。
            FacebookUtil.Login(delegate(string token) // 获取token成功
                {
                    // FB登录（登录前会检测当前FB账号是否已经绑定了IGGID，根据绑定情况会做相应登录操作）。
                    accountHelper.CheckCandidateByFacebookAccount(token,
                        new OnCheckCandidateByThirdAccountListenr(OnUnbindByThirdAccount, OnLastLoginSuccess,
                            OnLastLoginFailed));
                },
                delegate() // 获取token失败
                {
                    UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(377)); OnCancel(); 
                });
            return;
        }

        if (lastLoginType == IGGLoginType.GameCenter) // 上次的登录方式是GAMECENTER登录。
        {
            // 通过GameCenter SDK获取GameCenter账号的token。
            GameCenterUtil.Login(delegate(string token) // 获取token成功
                {
                    // GameCenter登录（登录前会检测当前GameCenter账号是否已经绑定了IGGID，根据绑定情况会做相应登录操作）。
                    accountHelper.CheckCandidateByGameCenter(token,
                        new OnCheckCandidateByThirdAccountListenr(OnUnbindByThirdAccount, OnLastLoginSuccess,
                            OnLastLoginFailed));
                },
                delegate() // 获取token失败
                {
                    UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(379)); OnCancel(); 
                });
            return;
        }

        if (lastLoginType == IGGLoginType.Apple) // 上次的登录方式是Apple登录。
        {
            // Apple登录（登录前会检测当前Apple账号是否已经绑定了IGGID，根据绑定情况会做相应登录操作）。
            accountHelper.CheckCandidateByApple(
                new OnCheckCandidateByAppleListenr(OnUnbindByApple, OnLastLoginSuccess, OnLastLoginFailed));
            return;
        }

        if (lastLoginType == IGGLoginType.IGGAccount) // 上次的登录方式是IGG通行证登录。
        {
            // IGG通行证登录（登录前会检测当前IGG通行证账号是否已经绑定了IGGID，根据绑定情况会做相应登录操作）。
            accountHelper.CheckCandidateByIGGAccount(
                new OnCheckCandidateByIGGAccountListenr(OnUnbindByIGGAccount, OnLastLoginSuccess, OnLastLoginFailed));
            return;
        }
    }

    public class DemoComplianceCheckResultListener : IIGGComplianceClearListener
    {
        public delegate void OnPostponing(IGGComplianceStatus status);

        public delegate void OnGuest(IGGComplianceStatus status, IGGComplianceRestrictions restrictions);

        public delegate void OnAdult(IGGComplianceStatus status);

        public delegate void OnMinor(IGGComplianceStatus status, IGGComplianceRestrictions restrictions);

        public delegate void OnFail(IGGError error);

        private OnPostponing onPostponing;
        private OnGuest onGuest;
        private OnAdult onAdult;
        private OnMinor onMinor;
        private OnFail onFail;

        public DemoComplianceCheckResultListener(OnPostponing onPostponing, OnGuest onGuest, OnAdult onAdult,
            OnMinor onMinor, OnFail onFail)
        {
            this.onPostponing = onPostponing;
            this.onGuest = onGuest;
            this.onAdult = onAdult;
            this.onMinor = onMinor;
            this.onFail = onFail;
        }

        void IIGGComplianceClearListener.OnPostponing(IGGComplianceStatus status)
        {
            onPostponing?.Invoke(status);
        }

        void IIGGComplianceClearListener.OnGuest(IGGComplianceStatus status, IGGComplianceRestrictions restrictions)
        {
            onGuest?.Invoke(status, restrictions);
        }

        void IIGGComplianceClearListener.OnAdult(IGGComplianceStatus status)
        {
            onAdult?.Invoke(status);
        }

        void IIGGComplianceClearListener.OnMinor(IGGComplianceStatus status, IGGComplianceRestrictions restrictions)
        {
            onMinor?.Invoke(status, restrictions);
        }

        void IIGGComplianceClearListener.OnFail(IGGError error)
        {
            onFail?.Invoke(error);
        }
    }

    /// <summary>
    /// 获取GameId
    /// </summary>
    public string GetGameId()
    {
        return GameId;
    }

    /// <summary>
    /// 获取SSOToken
    /// </summary>
    public string GetSSOToken()
    {
        return SSOToken;
    }

    /// <summary>
    /// 更新SSOToken
    /// </summary>
    private void SetSSOToken(IGGError error, string webssotoken)
    {
        SSOToken = webssotoken;
    }
}
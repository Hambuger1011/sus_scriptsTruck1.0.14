using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using LitJson;
using UGUI;
using Debug = UnityEngine.Debug;

//using Jing.ULiteWebView;

[XLua.LuaCallCSharp, XLua.Hotfix]
public class SdkMgr : CSingleton<SdkMgr>
{
#if CHANNEL_HUAWEI
    public static readonly string packageName = "com.OnyxGames.Secrets.huawei";
#elif CHANNEL_SPAIN
    public static readonly string packageName = "com.my.high.school.days";
#else
    #if UNITY_ANDROID
    public static readonly string packageName = "com.igg.android.scriptsuntoldsecrets";
    #else
    public static readonly string packageName = "com.igg.ios.scriptsuntoldsecretsen";
    #endif
#endif
    public static int IosAppId = 1535571424;

#if CHANNEL_SPAIN
    public static string GAME_NAME = "Scripts: Untold Secrets";
#else
        public static string GAME_NAME = "Scripts: Untold Secrets";
#endif

    public bool BillingSysInit = false;     //付费是否启动
        
    public bool AdmobSysInit = false;       //激励视频是否启动

    public bool UnityAdsInit = false;

    public bool MingetralAdsInit = false;

    public string VersionStr = "";


    //public JPushSdk jpushSDK { get; private set; }
    public ShareSdk shareSDK { get; private set; }
    public FirebaseSdk firebaseSdk { get; private set; }
    public FacebookSdk facebook { get; private set; }

#if CHANNEL_SPAIN
    public LabCaveMediationSdk ads { get; private set; }
#endif

#if CHANNEL_ONYX || CHANNEL_SPAIN

    public GoogleSdk google { get; private set; }

    public AppsFlyerSdk appsFlyer { get; private set; }
#endif

#if CHANNEL_ONYX
    public OnyxAdsSdk ads { get; private set; }

    public FirebaseSdk firebase { get; private set; }


    //public UnityAdsSdk unityAds{ get; private set; }

#endif
#if CHANNEL_HUAWEI

    public HuaweiSdk hwSDK { get; private set; }

    public MintegralSDK mintegralAds { get; private set; }

    //public ULiteWebView WebView;
#endif

    public bool _Test_PayOn = true;

    protected override void Init()
    {
        base.Init();
        BillingSysInit = false;
        AdmobSysInit = false;
        facebook = new FacebookSdk();
        facebook.Init();

#if CHANNEL_SPAIN
        ads = new LabCaveMediationSdk();
#endif


#if CHANNEL_ONYX || CHANNEL_SPAIN
        google = new GoogleSdk();
        appsFlyer = new AppsFlyerSdk();
#endif

#if CHANNEL_ONYX
        ads = new OnyxAdsSdk();
        firebase = new FirebaseSdk();
        //unityAds = new UnityAdsSdk();
#endif
#if CHANNEL_HUAWEI
        hwSDK = new HuaweiSdk();
        mintegralAds = new MintegralSDK();
#endif

        VersionStr = Application.version;

        LOG.Info("=============version===============>>>" + VersionStr);
    }


    public void Start()
    {
#if CHANNEL_ONYX || CHANNEL_SPAIN
        google.Start();
        appsFlyer.Start();
        ads.Start();
#endif
#if CHANNEL_HUAWEI
        hwSDK.Init();
        mintegralAds.Init();
#endif
    }


    //public void SetJPushSDK(JPushSdk vJPush)
    //{
    //    jpushSDK = vJPush;
    //}

    public void SetShareSDK(ShareSdk vShareSdk)
    {
        shareSDK = vShareSdk;
    }

    public void SetFirebaseSdk(FirebaseSdk vFirebaseSdk)
    {
        firebaseSdk = vFirebaseSdk;
    }


    /// <summary>
    /// 获取游戏版本号
    /// </summary>
    /// <returns></returns>
    public string GameVersion()
    {
        return VersionStr;
    }




    /// <summary>
    /// app前后台切换
    /// </summary>
    public void OnApplicationPause(bool pauseStatus)
    {
#if CHANNEL_ONYX
        if(this.ads != null)
        {
            this.ads.OnApplicationPause(pauseStatus);
        }
#endif
    }

    /// <summary>
    /// 设置webView
    /// </summary>
    /// <param name="vWebView"></param>
    //public void SetWebView(ULiteWebView vWebView)
    //{
    //    WebView = vWebView;
    //}

    /// <summary>
    /// 查看是从哪里发起的登陆
    /// 0:三个登陆按钮的界面（hw初始化登陆界面） 1：侧边栏的快捷登陆，2：:侧边栏设置界面、3：读书里面的设置界面
    /// </summary>
    /// <param name="vType"></param>
    public void GoogleLogin(int vType)
    {
#if UNITY_EDITOR

        LoginDataInfo loginInfo = new LoginDataInfo();
        loginInfo.UserId = "jsdifoajdso2341234dsfasdf12305";
        loginInfo.Token = "dnfaisondo2cvcxvzna42225";
        loginInfo.Email = "liuwei312@163.com";
        loginInfo.UserName = "googleTest";
        loginInfo.UserImageUrl = "";
        loginInfo.OpenType = vType;

        EventDispatcher.Dispatch(EventEnum.GoogleLoginSucc, loginInfo);

        return;
#endif
#if CHANNEL_ONYX || CHANNEL_SPAIN
        TalkingDataManager.Instance.LoginAccount(EventEnum.Login3rdStart,2);
        GamePointManager.Instance.BuriedPoint(EventEnum.Login);
        google.Login(vType);
#endif
    }

    /// <summary>
    /// 查看是从哪里发起的登陆
    /// 0:三个登陆按钮的界面（hw初始化登陆界面） 1：侧边栏的快捷登陆，2：:侧边栏设置界面、3：读书里面的设置界面
    /// </summary>
    /// <param name="vType"></param>
    public void FacebookLogin(int vType)
    {
#if UNITY_EDITOR
        LoginDataInfo loginInfo = new LoginDataInfo();
        loginInfo.UserId = "gjgfdqefqcwe4131efqw341dfasdfbvdc05";
        loginInfo.Token = "d189fa1sdfdfavvvv341fsad23e1w05";
        loginInfo.Email = "liuwei312@163.com";
        loginInfo.UserName = "facebookTest";
        loginInfo.UserImageUrl = "";
        loginInfo.OpenType = vType;

        EventDispatcher.Dispatch(EventEnum.FaceBookLoginSucc, loginInfo);
        return;
#endif
#if CHANNEL_ONYX || CHANNEL_HUAWEI || CHANNEL_SPAIN
        LOG.Info("==========FacebookLogin=======>>");
        TalkingDataManager.Instance.LoginAccount(EventEnum.Login3rdStart,1);
        GamePointManager.Instance.BuriedPoint(EventEnum.Login);
        facebook.Login(vType);
#endif
    }

    public void CheckAdsIsInit()
    {
#if CHANNEL_HUAWEI
#if UNITY_EDITOR
            return;
#endif
            mintegralAds.CheckAds();
#else
        
#if UNITY_EDITOR
            return;
#endif
           //unityAds.CheckAds();
#endif
    }

    public void ShowAds(Action<bool> vCallBack)
    {
        //AF事件记录* 用户请求广告加载
        AppsFlyerManager.Instance.ADS_REQUEST();

        //AF事件记录*  用户播放广告
        AppsFlyerManager.Instance.ADS_PLAY();

#if CHANNEL_ONYX
        //unityAds.ShowAds(vCallBack);
        ads.ShowRewardBasedVideo("SdkMgr", vCallBack);
#endif

#if CHANNEL_HUAWEI

            mintegralAds.ShowAds(vCallBack);
#endif

#if CHANNEL_SPAIN
        ads.ShowRewardBasedVideo(vCallBack);
#endif
    }

    private int mCurLoginChannel = 0;
    private Action loadCallBack = null;
    
    public void LoadFaceBookUserInfo(Notification vNot,Action callBack)
    {
        loadCallBack = callBack;
        LoginDataInfo loginInfo = vNot.Data as LoginDataInfo;

        if (loginInfo != null)
        {
            string userId = loginInfo.UserId;
            string tokenStr = loginInfo.Token;
            if (loginInfo.OpenType != 1) return;
            UserDataManager.Instance.UserData.UserID = userId;
            UserDataManager.Instance.UserData.IdToken = tokenStr;
            LOG.Info("---FaceBookLoginSuccHandler --userId-->" + userId + "===token--->" + tokenStr);
            SdkMgr.Instance.facebook.GetMyInfo(ReturnSelfFBInfo);
        }
    }
    
    private void ReturnSelfFBInfo(string vMsg)
    {
        LOG.Info("---return Self FB Info ---->" + vMsg);

        Dictionary<string, string> selfInfo = JsonHelper.JsonToObject<Dictionary<string, string>>(vMsg);
        string userId = string.Empty;
        string userNick = string.Empty;
        string email = string.Empty;
        string faceUrl = string.Empty;
        if (selfInfo != null)
        {
            if (selfInfo.ContainsKey("id"))
                userId = selfInfo["id"];
            if (selfInfo.ContainsKey("name"))
                userNick = selfInfo["name"];

            UserDataManager.Instance.UserData.UserName = userNick;

            if (UserDataManager.Instance.LoginInfo == null)
                UserDataManager.Instance.LoginInfo = new ThirdLoginData();

            mCurLoginChannel = 1;
            LoginDataInfo fbInfo = new LoginDataInfo();
            fbInfo.UserId = userId;
            fbInfo.UserName = userNick;
            fbInfo.Email = email;
            fbInfo.Token = UserDataManager.Instance.UserData.IdToken;
            fbInfo.UserImageUrl = faceUrl;
            UserDataManager.Instance.LoginInfo.FaceBookLoginInfo = fbInfo;

            LOG.Info("--- Self FB  --id-->" + userId + " userNick:" + userNick + " --token->" + UserDataManager.Instance.UserData.IdToken);


#if UNITY_ANDROID
#if CHANNEL_HUAWEI
            GameHttpNet.Instance.LoginByThirdParty(1, email, userNick, faceUrl, userId, UserDataManager.Instance.UserData.IdToken, "huawei", LoginByThirdPartyCallBack);
#else
            GameHttpNet.Instance.LoginByThirdParty(1, email, userNick, faceUrl, userId, UserDataManager.Instance.UserData.IdToken, "android", LoginByThirdPartyCallBack);
#endif
#else 
            GameHttpNet.Instance.LoginByThirdParty(1,email,userNick,faceUrl,userId,UserDataManager.Instance.UserData.IdToken,"ios",LoginByThirdPartyCallBack);
#endif

        }
    }
    
    public void LoadGoogleUserInfo(Notification vNot,Action callBack )
    {
        loadCallBack = callBack;
        LoginDataInfo loginInfo = vNot.Data as LoginDataInfo;
        if (loginInfo != null)
        {
            string userId = loginInfo.UserId;
            string tokenStr = loginInfo.Token;
            UserDataManager.Instance.UserData.UserID = userId;
            UserDataManager.Instance.UserData.IdToken = tokenStr;
            UserDataManager.Instance.UserData.UserName = loginInfo.UserName;
            if (UserDataManager.Instance.LoginInfo == null)
                UserDataManager.Instance.LoginInfo = new ThirdLoginData();
            mCurLoginChannel = 2;
            LoginDataInfo glInfo = new LoginDataInfo();
            glInfo.UserId = userId;
            glInfo.UserName = loginInfo.UserName;
            glInfo.Email = loginInfo.Email;
            glInfo.Token = UserDataManager.Instance.UserData.IdToken;
            glInfo.UserImageUrl = loginInfo.UserImageUrl;
            UserDataManager.Instance.LoginInfo.GoogleLoginInfo = glInfo;
            LOG.Info("--- GoogleInfo  --id-->" + userId + " userNick:" + glInfo.UserName + " --token->" + UserDataManager.Instance.UserData.IdToken + "----Email--->" + glInfo.Email);
#if UNITY_ANDROID
#if CHANNEL_HUAWEI
            GameHttpNet.Instance.LoginByThirdParty(0, loginInfo.Email, loginInfo.UserName, loginInfo.UserImageUrl, userId, UserDataManager.Instance.UserData.IdToken, "huawei", LoginByThirdPartyCallBack);
#else
            GameHttpNet.Instance.LoginByThirdParty(0, loginInfo.Email, loginInfo.UserName, loginInfo.UserImageUrl, userId, UserDataManager.Instance.UserData.IdToken, "android", LoginByThirdPartyCallBack);
#endif
#else 
            GameHttpNet.Instance.LoginByThirdParty(0, loginInfo.Email, loginInfo.UserName, loginInfo.UserImageUrl, userId, UserDataManager.Instance.UserData.IdToken,"ios",LoginByThirdPartyCallBack);
#endif
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
                UserDataManager.Instance.LoginInfo.LastLoginChannel = mCurLoginChannel;
                if (jo.code == 200)
                {
                    UserDataManager.Instance.thirdPartyLoginInfo = JsonHelper.JsonToObject<HttpInfoReturn<ThirdPartyReturnInfo>>(result);
                    if (UserDataManager.Instance.thirdPartyLoginInfo != null && UserDataManager.Instance.thirdPartyLoginInfo.data != null)
                    {
                        GameHttpNet.Instance.TOKEN = UserDataManager.Instance.thirdPartyLoginInfo.data.token;
                    }

                    loadCallBack();
                }
                else if (jo.code == 201)
                {
                    //UINetLoadingMgr.Instance.Close();
                    LOG.Info("--LoginByThirdPartyCallBack--->参数不完整");

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(173);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("参数不完整");
                }
                else if (jo.code == 208)
                {
                    //UINetLoadingMgr.Instance.Close();
                    LOG.Info("--LoginByThirdPartyCallBack--->登录失败");

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(174);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Login failed.");
                }
                else if (jo.code == 277)
                {
                    UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, jo.msg);
                    return;
                }

                UserDataManager.Instance.SaveLoginInfo();

            }, null);
        }
    }
    

    /// <summary>
    /// 打开webview
    /// </summary>
    /// <param name="vUrl"></param>
    public void OpenWebView(string vUrl,Action<string> vCallBack)
    {
        LOG.Info("=============OpenWebView Start==============>"+vUrl);
        //if(WebView != null && !string.IsNullOrEmpty(vUrl))
        //{
        //    WebView.RegistJsInterfaceAction("JsCallUnity", vCallBack);
        //    WebView.Show();
        //    WebView.LoadUrl(vUrl);
        //    LOG.Info("=============OpenWebView Doing and Register==============>");
        //}
    }

    /// <summary>
    /// 关闭网页
    /// </summary>
    /// <param name="vCallBack"></param>
    public void CloseWebView(Action<string> vCallBack)
    {
        //if (WebView != null)
        //{
        //    WebView.UnregistJsInterfaceAction("JsCallUnity", vCallBack);
        //    WebView.Close();
        //}
    }


    #region 支付
    public void Pay(int vShopId, string vPaymentName, int vsource, string price, Action<bool,string> callback)
    {
        callback += (isOK, result) =>
        {
            UINetLoadingMgr.Instance.Close2();
        };

        TalkingDataManager.Instance.GameCharge(vPaymentName);
        TalkingDataManager.Instance.DoCharge(EventEnum.GetOrderStart);
        //1.获取订单号
        GameHttpNet.Instance.GetOrderFormInfo(vShopId, vPaymentName, "", vsource, (result)=>
        {
#if UNITY_IOS
            bool isAndroid = false;
#else
            bool isAndroid = true;
#endif
            LOG.Info("----GetOrderFormCallBack---->" + result);
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if(jo == null)
            {
                callback(false,result);
                return;
            }
            switch (jo.code)
            {
                case 200:
                    {
                        UserDataManager.Instance.orderFormInfo = JsonHelper.JsonToObject<HttpInfoReturn<OrderFormInfo>>(result);
                        if (UserDataManager.Instance.orderFormInfo != null)
                        {
                            //UINetLoadingMgr.Instance.Show();
                            string orderId = UserDataManager.Instance.orderFormInfo.data.recharge_no;
                            string notifyUrl = UserDataManager.Instance.orderFormInfo.data.notify_url;

                            if (PurchaseRecordManager.Instance.HashUnCompleteOrder(orderId))
                            {
                                return;
                            }

                            TalkingDataManager.Instance.DoCharge(EventEnum.GetOrderResultSucc,orderId);

                            //2.拉起sdk支付
                            UINetLoadingMgr.Instance.Show2();
                            if (isAndroid)
                            {
                                TalkingDataManager.Instance.DoCharge(EventEnum.PlatformPayStart, orderId);
                                string sign = UserDataManager.Instance.orderFormInfo.data.sign;
                                SdkMgr.Instance.Pay_Android(vPaymentName, orderId, callback, notifyUrl, sign);
                            }
                            else
                            {
#if UNITY_IOS || UNITY_EDITOR
                                PurchaseManager.Instance.Purchase(vPaymentName, orderId, callback);
#endif
                            }
                            //TalkingDataManager.Instance.OnChargeRequest(orderId, vPaymentName, float.Parse(price), "USD", 1, "PT");
                        }
                    }
                    break;
                case 202:
                    {

                        //UINetLoadingMgr.Instance.Close();
                        LOG.Info("---GetOrderFormCallBack-->商城配置ID未找到");

                        var Localization = GameDataMgr.Instance.table.GetLocalizationById(146);
                        UITipsMgr.Instance.PopupTips(Localization, false);

                        //UITipsMgr.Instance.PopupTips("We couldn't find this Product ID.", false);
                        callback(false, result);
                    }
                    break;
                case 208:
                    {
                        TalkingDataManager.Instance.DoCharge(EventEnum.GetOrderResultFail);

                        //UINetLoadingMgr.Instance.Close();
                        LOG.Info("---GetOrderFormCallBack-->订单创建失败");

                        var Localization = GameDataMgr.Instance.table.GetLocalizationById(147);
                        UITipsMgr.Instance.PopupTips(Localization, false);

                        //UITipsMgr.Instance.PopupTips("Error while processing your order.", false);
                        callback(false, result);
                    }
                    break;
                case 277:
                default:
                    {
                        UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, jo.msg);
                        callback(false, result);
                    }
                    break;
            }
        });
    }




    public static void TestCommitPurchase(string recharge_no, string productId, Action<string, string> vCallBackHandler)
    {
#if UNITY_EDITOR || PAY_TEST
        string orderId = "GPA." + UnityEngine.Random.Range(1000, 9999) + "-" + UnityEngine.Random.Range(1000, 9999) + "-" + UnityEngine.Random.Range(1000, 9999) + "-" + UnityEngine.Random.Range(10000, 99999);
        string purchaseToken = "aaaaabbbbbcccccddddddTESTaaaaabbbbbcccccddddddTEST";
        //string productId = "com.onyx.diamond1";
        string packageName = SdkMgr.packageName;
        string dataSignature = "vEyBgASu10aKJqzdoKhkuklGRLv1IzjoNB5lxxcrOF U/uAUsMlxEABj 7W8bkEGHIrSiGH7 aR5rgRdOxU24vnxucP8b7huJGELCX ";
        string purchaseTime = "1539679992305";
        string purchaseState = "0";
        string test_token = "astar";
        LOG.Info("------TestCommitPurchase----->" + orderId);
        PurchaseRecordManager.Instance.AddPurchaseRecord(recharge_no, orderId, purchaseToken, productId, packageName, dataSignature, purchaseTime, purchaseState, 0);
        GameHttpNet.Instance.GetOrderToSubmitForAndroid(recharge_no, orderId, purchaseToken, productId, packageName, dataSignature, purchaseTime, purchaseState, test_token, vCallBackHandler);
#endif
    }

    void Pay_Android(string productId, string orderFormId, Action<bool, string> callback, string vNotifyUrl = "", string vSign = "")
    {
        #region 提交订单
        Action<string, string> GetOrderToSubmitCallBack = (string orderID, string result) =>
        {
            //UINetLoadingMgr.Instance.Close();
            LOG.Info("----GetOrderToSubmitCallBack---->" + result);
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo == null)
            {
                callback(false, result);
                TalkingDataManager.Instance.DoCharge(EventEnum.SubmitOrderResultFail, orderID);
                return;
            }

            bool isOK = false;

            switch (jo.code)
            {
                case 200:
                    {
                        UserDataManager.Instance.SetPayUser();
                        AudioManager.Instance.PlayTones(AudioTones.RewardWin);
                        UserDataManager.Instance.orderFormSubmitResultInfo = JsonHelper.JsonToObject<HttpInfoReturn<OrderFormSubmitResultInfo>>(result);
                        if (UserDataManager.Instance.orderFormSubmitResultInfo != null && UserDataManager.Instance.orderFormSubmitResultInfo.data != null)
                        {
                            UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.orderFormSubmitResultInfo.data.bkey);
                            UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.orderFormSubmitResultInfo.data.diamond);
                            UserDataManager.Instance.ResetMoney(3, UserDataManager.Instance.orderFormSubmitResultInfo.data.ticket);

                            //UITipsMgr.Instance.PopupTips("Payment Successful!", false);

                            //string vOrderId = UserDataManager.Instance.orderFormSubmitResultInfo.data.google_orderid;
                            //if (!string.IsNullOrEmpty(vOrderId))

                            TalkingDataManager.Instance.DoCharge(EventEnum.SubmitOrderResultSucc, orderID);

                            PurchaseRecordManager.Instance.SendRecordToAppsFlyer(orderID);
                            //TalkingDataManager.Instance.OnChargeSuccess(orderID);
                            isOK = true;
                        }
                    }
                    break;
                case 202://订单已支付，无需重复支付
                    {
                        //AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                        //LOG.Info("---订单已支付，无需重复支付-->");

                        //var Localization = GameDataMgr.Instance.table.GetLocalizationById(149);
                        //UITipsMgr.Instance.PopupTips(Localization, false);

                        //UITipsMgr.Instance.PopupTips("Payment completed, no need to pay again.", false);
                    }
                    break;
                case 201://充值失败
                    {

                        TalkingDataManager.Instance.DoCharge(EventEnum.SubmitOrderResultFail, orderID);
                        AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                        LOG.Info("---充值失败-->");

                        var Localization = GameDataMgr.Instance.table.GetLocalizationById(148);
                        UITipsMgr.Instance.PopupTips(Localization, false);

                        //UITipsMgr.Instance.PopupTips("Payment Failed!", false);
                    }
                    break;
                default:
                    UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, jo.msg);
                    break;
            }

            if (isOK)
            {
                PurchaseRecordManager.Instance.RemovePurchaseRecord(orderID);
            }
            callback(isOK, result);
        };
        #endregion


#if UNITY_EDITOR || PAY_TEST
        TestCommitPurchase(orderFormId, productId, GetOrderToSubmitCallBack);
        var isTest = true;
        if (isTest)
        {
            return;
        }
#endif

#if CHANNEL_HUAWEI
        if(hwSDK != null)
        {
            hwSDK.ProductPay(vSign, productId, orderFormId, vNotifyUrl);
        }
#endif

#if CHANNEL_ONYX || CHANNEL_SPAIN

        if (google != null)
        {
            google.Pay(productId, orderFormId, (bool isOK, string json) =>
            {
                if (json.StartsWith("Exception"))
                {
                    callback(false, json);
                    return;
                }

                Purchase purchase = null;
                var jsonData = JsonMapper.ToObject(json);
                var result = JsonMapper.ToObject<IabResult>((string)jsonData["result"]);
                //LOG.Error(IabResult.getResponseDesc(result.response) + "\n" + result.message);
                if (jsonData.Contains("purchase"))
                {
                    purchase = new Purchase((string)jsonData["purchase"]);
                }

                if (!isOK || purchase == null)
                {
                    if (result.response == IabResult.IABHELPER_USER_CANCELLED || (purchase != null && purchase.getPurchaseState() == IabResult.BILLING_RESPONSE_RESULT_USER_CANCELED))
                    {
                        TalkingDataManager.Instance.DoCharge(EventEnum.PlatformPayCancel, orderFormId);
                        //TalkingDataManager.Instance.PlayerCancelCharge(orderFormId);
                        GameHttpNet.Instance.UserOrderCancel(orderFormId, json, 3, OrderCancelCallBack);
                    }
                    else
                    {
                        TalkingDataManager.Instance.DoCharge(EventEnum.PlatformPayResultFail, orderFormId);
                        AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                        var Localization = GameDataMgr.Instance.table.GetLocalizationById(148);
                        UITipsMgr.Instance.PopupTips(Localization, false);
                    }
                    callback(false, json);
                    return;
                }

                //if (purchase != null && purchase.getPurchaseState() == IabResult.BILLING_RESPONSE_RESULT_OK)
                {
                    var dataSignature = purchase.getSignature();
                    //string productId = purchaseData["productId"].ToString();
                    string orderId = purchase.getOrderId();
                    string purchaseToken = purchase.getToken();
                    string developerPayload = orderFormId;// purchase.getDeveloperPayload();//已经弃用，永远为空
                    string packageName = purchase.getPackageName();
                    string purchaseTime = purchase.getPurchaseTime().ToString();
                    string purchaseState = purchase.getPurchaseState().ToString();

                    //var BuyItemDic = new Dictionary<string, string>();
                    //BuyItemDic.Add("developerPayload", developerPayload);
                    //BuyItemDic.Add("orderId", orderId);
                    //BuyItemDic.Add("purchaseToken", purchaseToken);
                    //BuyItemDic.Add("productId", productId);
                    //BuyItemDic.Add("packageName", packageName);
                    //BuyItemDic.Add("dataSignature", dataSignature);
                    //BuyItemDic.Add("purchaseTime", purchaseTime);
                    //BuyItemDic.Add("purchaseState", purchaseState);

                    //SdkMgr.Instance.facebook.LogPurchase(Price);
                    //SdkMgr.Instance.appsFlyer.Purchase(Price, productId);
                    PlayerPrefs.SetString(orderId, orderFormId);

                    LOG.Info("---订单支付成功-->developerPayload:" + developerPayload + " productId:" + productId + " orderId:" + orderId + " purchaseToken:" + purchaseToken);

                    PurchaseRecordManager.Instance.AddPurchaseRecord(developerPayload, orderId, purchaseToken, productId, packageName, dataSignature, purchaseTime, purchaseState);

#if ENABLE_DEBUG
                    if (!_Test_PayOn)
                    {
                        UIAlertMgr.Instance.Show("[Test]支付失败", "请重启游戏补单^_^", AlertType.Sure);
                        callback(false, "支付成功，测试补单");
                        return;
                    }
#endif

                    TalkingDataManager.Instance.DoCharge(EventEnum.PlatformPayResultSucc, orderFormId);
                    TalkingDataManager.Instance.DoCharge(EventEnum.SubmitOrderStart, orderFormId);
                    //UINetLoadingMgr.Instance.Show();
                    GameHttpNet.Instance.GetOrderToSubmitForAndroid(developerPayload, orderId, purchaseToken, productId, packageName, dataSignature, purchaseTime, purchaseState, "", (_orderId, _result) =>
                    {
                        PlayerPrefs.DeleteKey(orderId);
                        //UINetLoadingMgr.Instance.Close();
                        if (GetOrderToSubmitCallBack != null)
                        {
                            GetOrderToSubmitCallBack(orderId, _result);
                            GetOrderToSubmitCallBack = null;
                        }
                        PurchaseRecordManager.Instance.RemovePurchaseRecord(orderId);
                    });
                    AudioManager.Instance.PlayTones(AudioTones.RewardWin);

                }

            });
        }
#endif
    }




    private void OrderCancelCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----OrderCancelCallBack---->" + result);
    }


    private ShopItemInfo shopinfo = null;

    public void CloseDelayTimer()
    {
        if (DelayTimer >= 0)
        {
            CTimerManager.Instance.RemoveTimer(DelayTimer);
        }
    }

    public void CloseRequestTimer()
    {
        if (QueryOrderTimer >= 0)
        {
            CTimerManager.Instance.RemoveTimer(QueryOrderTimer);
        }
    }

    //延迟定时器
    private int DelayTimer = -1;
    private int QueryOrderTimer = -1;
    /// <summary>
    /// 商品购买成功
    /// </summary>
    public void OnPaySuccess(string _orderId, string ItemId)
    {
        CloseDelayTimer();

        //延时4秒请求  第一次请求订单
        DelayTimer = CTimerManager.Instance.AddTimer(4000, 1, (_) =>
        {
            //删除定时器 (本身定时器)
            CloseDelayTimer();
            //即时请求
            this.RequestQueryOrder(_orderId, ItemId);
            Debug.LogError("RequestQueryOrder1;");
        });

        //10000  10秒
        //定时触发请求获取订单
        QueryOrderTimer = CTimerManager.Instance.AddTimer(10000, -1, (_) =>
        {
            this.RequestQueryOrder(_orderId, ItemId);
            Debug.LogError("RequestQueryOrder2 ;");
        });
    }

    /// <summary>
    ///  //查询订单结果 请求
    /// </summary>
    private void RequestQueryOrder(string _orderId, string ItemId)
    {
        string url = "api_queryOrder" + "&order_id=" + _orderId;
        url = GameHttpNet.Instance.GameUrlHead + "/" + url;

        var sendSeq = getSendSeq();
        string sendInfo = string.Format("<color=#009000>[CS][send]GET:[{0}]{1} </color>", sendSeq, url);
        LOG.Info(sendInfo);

        UniHttp.Instance.Get(url, (HttpObject obj, long responseCode, string result) =>
        {
            if (obj.isMask)
            {
                obj.isMask = false;
            }

            JsonObject jo = JsonHelper.JsonToJObject(result);

            Debug.LogError("----api_queryOrder---->" + result);
            if (jo.code == 200)
            {
                UINetLoadingMgr.Instance.Close2();
                //删除定时器
                this.CloseRequestTimer();
                GetQueryOrderCallBack(_orderId, ItemId, result);
            }
            else
            {
                return;
            }
        }, 1000 * 10, 3);
    }

    /// <summary>
    ///  //查询订单结果 回调
    /// </summary>
    private void GetQueryOrderCallBack(string orderId, string ItemId, string _result)
    {
        //删除本地缓存  
        PlayerPrefs.DeleteKey(orderId);
        //关闭转菊花
        UINetLoadingMgr.Instance.Close2();

        //是否为付费玩家
        UserDataManager.Instance.SetPayUser();
        //支付成功音效
        AudioManager.Instance.PlayTones(AudioTones.RewardWin);

        //保存缓存 购买物品的数量刷新
        UserDataManager.Instance.orderFormSubmitResultInfo = JsonHelper.JsonToObject<HttpInfoReturn<OrderFormSubmitResultInfo>>(_result);
        if (UserDataManager.Instance.orderFormSubmitResultInfo != null && UserDataManager.Instance.orderFormSubmitResultInfo.data != null)
        {
            UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.orderFormSubmitResultInfo.data.bkey);
            UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.orderFormSubmitResultInfo.data.diamond);
            UserDataManager.Instance.ResetMoney(3, UserDataManager.Instance.orderFormSubmitResultInfo.data.ticket);

            //打点？
            TalkingDataManager.Instance.DoCharge(EventEnum.SubmitOrderResultSucc, orderId);

            //PurchaseRecordManager.Instance.SendRecordToAppsFlyer(orderId);
        }

        //弹提示 购买成功
        var Localization = GameDataMgr.Instance.table.GetLocalizationById(152);
        UITipsMgr.Instance.PopupTips(Localization, false);

       

        //获取商品信息
        shopinfo = UserDataManager.Instance.shopList.data.GetTypeByProduct_id(ItemId);


        if (shopinfo != null)
        {
            TalkingDataManager.Instance.ShopBuy(shopinfo.type, shopinfo.price);

            //AF事件
            AppsFlyerManager.Instance.FIRST_BUY(shopinfo.af_code, shopinfo.price);
        }

        //调用这个来刷新，商品的首充显示信息
        this.GetOwnProductIDS();

        LOG.Info("恭喜你，商品购买成功了");
    }

    /// <summary>
    /// 调用这个来刷新，商品的首充显示信息
    /// </summary>
    public void GetOwnProductIDS()
    {
        GameHttpNet.Instance.Getuserpaymallid(GetuserpaymallidCallback);
    }



    private void GetuserpaymallidCallback(object arg)
    {
        if (shopinfo == null)
        {
            Debug.LogError("商品购买成功，找不到此商品的信息；");
            // return;
        }

        string result = arg.ToString();
        LOG.Info("----GetuserpaymallidCallback---->" + result);

        LoomUtil.QueueOnMainThread((param) =>
        {

            JsonObject jo = JsonHelper.JsonToJObject(result);

            if (jo.code == 200)
            {
                UserDataManager.Instance.Getuserpaymallid = JsonHelper.JsonToObject<HttpInfoReturn<Getuserpaymallid>>(result);

                //商品购买成功，派发事件，刷新物品的首充显示信息
                EventDispatcher.Dispatch(EventEnum.GetuserpaymallidStatChack, shopinfo.type);
            }
        }, null);
    }

    int _sendSeq = 0;
    int getSendSeq()
    {
        _sendSeq = _sendSeq + 1;
        return _sendSeq;
    }

    #endregion
}

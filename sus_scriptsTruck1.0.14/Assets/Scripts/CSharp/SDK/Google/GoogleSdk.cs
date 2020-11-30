#if CHANNEL_ONYX || CHANNEL_SPAIN
#define LOGIN_BY_JAVA

#if UNITY_ANDROID && !LOGIN_BY_JAVA
//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
#endif

using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using Firebase.Crashlytics;
using LitJson;

public class GoogleSdk
{
    /*
     * 游戏服务->关联的应用
     * https://www.cnblogs.com/zhaoyanjun/p/5337442.html
     * https://console.developers.google.com/projectselector/apis/credentials
     * https://blog.csdn.net/silence_1990/article/details/82898809
     */

    /*
     * 我的应用->开发工具->服务和API
     */
#if CHANNEL_SPAIN
    string clientID = "492365670749-q0qi2metii3vbkedspqfrkkt6dcrl1qe.apps.googleusercontent.com";
    string base64EncodedPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoS2SmzVF4+ZnKP5lLWyjnPt3tU/BriPCSPES2JnK71N38HY2BB9hB3LYDB7KLHAKmCqgPqBX7XQnyieOp3HXg5rz5AbfgN5OzRiMWa69MRuKfw3pLMJXIMQgOoDt+db/6T+RWC2sGWlOyv6x4GkMGIcCU10+OdK8HI9nnnD0taxg/t0mpwlsLmjmi9bVQTVEb/+YMeut47V4pHJDj9X6d4cFWZZ5D5WIjT4mZhWBO9GsTW62HMi+B61HIqE2IT9qY4njXI37lO/C5vc1gLj6eJj6bbnXlfzn8tSzR3bfFEUWp1ymNba6YKJdh0QoTAGcjPmQlK7cYpXXndF/x70DfQIDAQAB";
#else
    // string clientID = "658840815205-3l3radaoisfrblg7a3okmd6nv56r88nq.apps.googleusercontent.com";
    // string base64EncodedPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAvNcp1CEvHiHT9d1468jtcCjZTJyO7TM6vKfH3IuMtA17weEeBnTuK9s8aqknH0d42G/7iYevVJwEYDn/SAcCQmuXBzOQQQ2pdhpfexWmtRyHDJyaHIpGiGH9zreOoiX5VXWxnDfrPnIoOQ0hNEO/RyqKg/lVDGxFKIQcNHKwVXc7rjjX7J0HIvmQs9hmIE5uFFlHULuljGu5vD9872D7eJeaT7vMXFhill+0TE5jzITV0isIjqLH7rEutSfWRfy6NOWMiz1t23+n2De6zYFbfjp33CXveuNIUYqD2GGEN+5zUASmmnLbmNXW+j4HE8Bq42VgC+AJoRO16uoqkjCH9QIDAQAB";
    
    string clientID = "888213571477-nfta17decvoip81u9c6qcu3agobnj436.apps.googleusercontent.com";
    string base64EncodedPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAibd23dFdusYRFcq7pOPvdiYLvmmumRfCM1uiWKsBbXfFAovd5YJ0zRs70IgjFAOBfwZpZ6EDUlnaKucHBILdT251n3VJcK3p3OOyhVQt5dFTRPzPNt+/fiaaAYP3mRhXvwReCPkSMw/5dRM7nDr40nfJYO0N3xxfSkf3IS27DaB9gDhVmsS+0Wrr/uYFE2hDk1Nu0NIVLQITtzmjkBjLnWea3esgLmvxyjF1x6OySE+UFWG7U5LNqJsHDFqo/8mBLsLZoW7wMGFcuG2fYVB8P8wXrL3LMY5WTexcaF250xJ3bCz1PcvqzW/ialeqLqSezPbY+qZmUzNG0mrSwIZ+lwIDAQAB";
#endif

    static readonly string CLASS_NAME = "com.google.GoogleSdk";
    Dictionary<string,SkuDetails> skuDetailsMap = new Dictionary<string, SkuDetails>();



    #region 登录
    public int mType;
    public bool m_isLogin = false;
    public bool isGooglePlayServicesAvailable = true;

    public void Start()
    {

        //AndroidHelper.CallStaticMethod(CLASS_NAME, "InitGame", clientID , new JAction.Three<bool, string, string>(OnLoginNotify) );
        //支付有点坑，需要良好的vpn，最好在真机上测试
#if UNITY_ANDROID
        InitPay();
#endif

#if UNITY_EDITOR
        return;
#endif

#if UNITY_ANDROID && !LOGIN_BY_JAVA
        //var builder = new PlayGamesClientConfiguration.Builder();

        //LOG.Error("clientID:" + clientID);
        //builder.RequestIdToken();
        //builder.RequestServerAuthCode(false);
        //builder.RequestEmail();

        //PlayGamesPlatform.InitializeInstance(builder.Build());
        //PlayGamesPlatform.DebugLogEnabled = true;
        //PlayGamesPlatform.Activate();
#endif

        //Crashlytics.SetCustomKey()
        //Crashlytics.SetCustomKey();
        //Crashlytics.IsCrashlyticsCollectionEnabled = true;
        //Crashlytics.SetUserId("OnyxTest123456");

    }

    public static bool m_initLock = false;
    public void InitPay()
    {
        if (m_initLock)
        {
            return;
        }
#if UNITY_ANDROID
        //if (!isGooglePlayServicesAvailable)
        //{
        //    return;
        //}
        //isGooglePlayServicesAvailable = AndroidHelper.CallStaticMethod<bool>(CLASS_NAME, "CheckPlayServices");
        //if (isGooglePlayServicesAvailable)
        //{
        //    m_initLock = true;
        //    LOG.Error("[+]初始化google服务");
        //    AndroidHelper.CallStaticMethod(CLASS_NAME, "InitPay", base64EncodedPublicKey, GameUtility.isDebugMode);
        //}
        //else
        //{
        //    LOG.Error("该设备没有google服务");
        //}
#endif

    }

    /*
        原生登录https://github.com/playgameservices/play-games-plugin-for-unity
    */
    public void Login(int vType)
    {
        mType = vType;

#if UNITY_EDITOR
        return;
#endif

//#if UNITY_ANDROID
//#if LOGIN_BY_JAVA
//        AndroidHelper.CallStaticMethod(CLASS_NAME, "Login", clientID);
//#else
//        var platform = ((PlayGamesPlatform)Social.Active);
//        //if (platform.IsAuthenticated())
//        //{
//        //    return;
//        //}
//        platform.Authenticate((success) =>
//        {
//            if (success)
//            {
//                string idToken = platform.GetIdToken();
//                string email = platform.GetUserEmail();
//                string userName = platform.GetUserDisplayName();
//                string userID = platform.GetUserId();
//                string userImageUrl = platform.GetUserImageUrl();

                
//                var msg = string.Format("id:{0}\nname:{1}\nmail:{2}\ntoken:{3}",
//                    userID,
//                    userName,
//                    email,
//                    idToken
//                    );
//                LOG.Error(msg);
//                //AndroidUtils.ShowDialog("登录成功", msg,"",new JAction.Zero(() =>{ }));

//                LoginDataInfo loginInfo = new LoginDataInfo();
//                loginInfo.UserId = userID;
//                loginInfo.Token = idToken;
//                loginInfo.Email = email;
//                loginInfo.UserName = userName;
//                loginInfo.UserImageUrl = userImageUrl;
//                loginInfo.OpenType = vType;

//                EventDispatcher.Dispatch(EventEnum.GoogleLoginSucc, loginInfo);
//                TalkingDataManager.Instance.LoginAccount(EventEnum.Login3rdResultSucc,2);
//            }
//            else
//            {
//                //AndroidUtils.ShowDialog("登录失败", "登录失败", "", new JAction.Zero(() => { }));
//                TalkingDataManager.Instance.LoginAccount(EventEnum.Login3rdResultFail,2);
//            }
//        });

//#endif
//#endif

    }

    public bool IsLogin()
    {

//#if UNITY_EDITOR
//        return false;
//#endif

//#if UNITY_ANDROID
//#if LOGIN_BY_JAVA
//        return m_isLogin;
//#else
//        var platform = ((PlayGamesPlatform)Social.Active);
//        return platform.IsAuthenticated();
//#endif
//#endif

        return false;
    }

    public void Logout()
    {

#if UNITY_EDITOR
        return;
#endif

//#if UNITY_ANDROID
//#if LOGIN_BY_JAVA
//        AndroidHelper.CallStaticMethod(CLASS_NAME, "Logout");
//        m_isLogin = false;
//#else
//        var platform = ((PlayGamesPlatform)Social.Active);
//        if(!platform.IsAuthenticated())
//        {
//            return;
//        }
//        platform.SignOut();
//#endif
//#endif

    }
    
#endregion



    #region 支付


    /// <summary>
    /// 购买商品
    /// </summary>
    /// <param name="productName"></param>
    /// <param name="orderFormId"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public string Pay(string productName, string orderFormId, Action<bool,string> callback)
    {
        LOG.Info("Pay:productName=" + productName + ",orderFormId=" + orderFormId);
        _onPayComplete = callback;

#if UNITY_EDITOR || PAY_TEST
        CallPayEvent(true, null);
        var isTest = true;
        if (isTest)
        {
            return productName;
        }
#endif
        
        //LOG.Warn("调用支付:productName=" + productName + ",orderFormId=" + orderFormId);

        //SkuDetails skuDetails;
        //if (skuDetailsMap.TryGetValue(productName, out skuDetails))
        //{
        //    LOG.Warn("skuDetails=" + skuDetails.getOriginalJson());
        //}
        //else
        //{
        //    LOG.Error("商品列表找不到productName:" + productName);
        //}
        //AndroidHelper.CallStaticMethod(CLASS_NAME, "BuyItem", productName, orderFormId);
        return productName;
    }
    

    private void OrderCancelCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----OrderCancelCallBack---->" + result);
    }

    Action<bool, string> _onPayComplete;
    public void CallPayEvent(bool isOK, string result)
    {
        if (_onPayComplete != null)
        {
            var tmp = _onPayComplete;
            _onPayComplete = null;
            tmp(isOK, result);
        }
        //Debug.LogError("-----Purchase---result-->" + vInfo);
    }


    string cacheProductList;

    public void QuerySkuDetails(string productList = null)
	{
		LOG.Info("<color=red>RoductList:"+productList+"</color>");
        //if (!isGooglePlayServicesAvailable)
        //{
        //    return;
        //}

        //if (!SdkMgr.Instance.BillingSysInit)
        //{
        //    return;
        //}

        //if (productList == null)
        //{
        //    productList = cacheProductList;
        //}
        //else
        //{
        //    cacheProductList = productList;
        //}

        //if (productList == null)
        //{
        //    return;
        //}

        //AndroidHelper.CallStaticMethod(CLASS_NAME, "QuerySkuDetails", productList, new JAction.Two<bool, string>((bool flag, string json) =>
        //{
        //    CTimerManager.Instance.AddTimer(0, 1, (_) =>
        //    {
        //        if (!flag)
        //        {
        //            LOG.Error("QuerySkuDetails失败:"+ json);
        //            //CTimerManager.Instance.AddTimer(5000, 1, (__) =>
        //            //{
        //            //    QuerySkuDetails();
        //            //});
        //            return;
        //        }
               
        //        LOG.Info("QuerySkuDetails:\n" + json);
        //        var jsonArray = JsonHelper.JsonToObject<string[]>(json);
        //        string str = "";
        //        foreach (string itr in jsonArray)
        //        {
        //            SkuDetails skuDetails = new SkuDetails(itr);
        //            this.skuDetailsMap[skuDetails.getSku()] = skuDetails;

        //            if (str != "")
        //            {
        //                str += "#";
        //            }
        //            str += skuDetails.getSku() + "^" + skuDetails.getPriceCurrencyCode() + "^" + (skuDetails.getPriceAmountMicros() * 10e-7) + "^" + skuDetails.getPrice();
        //            //str += skuDetails.productId+"^"+ skuDetails.price_currency_code +"^"+(skuDetails.price_amount_micros * 10e-7)+"^"+ skuDetails.price;
        //        }
        //        PurchaseManager.Instance.ReturnProductList(str);
        //        cacheProductList = null;

        //        QueryPurchase();
        //    });
        //}));

    }

    bool flagQueryPurchase = false;
    /// <summary>
    /// 查询历史订单
    /// </summary>
    public void QueryPurchase()
    {
        //if (flagQueryPurchase)
        //{
        //    return;
        //}
        //flagQueryPurchase = true;
        //LOG.Info("<color=red>QueryPurchase</color>");
        //AndroidHelper.CallStaticMethod(CLASS_NAME, "QueryPurchase");
    }

    Queue<Purchase> unCompletePurchases = new Queue<Purchase>();
    public void OnQueryPurchase(string json)
    {
        //LOG.Info("<color=cyan>OnQueryPurchase:" + json+"</color>");
        //if (json.StartsWith("Exception"))
        //{
        //    CTimerManager.Instance.AddTimer(5000, 1, (__) =>
        //    {
        //        this.QueryPurchase();
        //    });
        //    return;
        //}
        //var result = JsonHelper.JsonToObject<IabResult>(json);
        //if(result.response != IabResult.BILLING_RESPONSE_RESULT_OK)
        //{
        //    CTimerManager.Instance.AddTimer(5000, 1, (__) =>
        //    {
        //        this.QueryPurchase();
        //    });
        //    return;
        //}
        //var purchaseList = JsonHelper.JsonToObject<string[]>(result.other);
        //foreach (var jsonPurchase in purchaseList)
        //{
        //    var purchase = new Purchase(jsonPurchase);
        //    var dataSignature = purchase.getSignature();
        //    unCompletePurchases.Enqueue(purchase);
        //}
        //LOG.Error("未完消耗单数量:" + unCompletePurchases.Count);
        //IssueOrders();
    }

    /// <summary>
    /// 补单(只能查询未消耗成功的)
    /// </summary>
    void IssueOrders()
    {
        //if(unCompletePurchases.Count == 0)
        //{
        //    return;
        //}
        //var purchase = unCompletePurchases.Peek();
        //ConsumePurchase(purchase, (isOK,json)=>
        //{
        //    if (!isOK)
        //    {
        //        return;
        //    }
        //    if (purchase.getPurchaseState() == IabResult.BILLING_RESPONSE_RESULT_OK)
        //    {
        //        var dataSignature = purchase.getSignature();

        //        if (dataSignature.Length == 0)
        //        {
        //            //TalkingDataManager.Instance.PlayerCancelCharge(purchase.getOrderId());
        //            TalkingDataManager.Instance.DoCharge(EventEnum.PlatformPayCancel, purchase.getOrderId());
        //            var Localization = GameDataMgr.Instance.table.GetLocalizationById(148);
        //            UITipsMgr.Instance.PopupTips(Localization, false);


        //            //UITipsMgr.Instance.PopupTips("Payment failed!", false);
        //            return;
        //        }

        //        string productId = purchase.getSku();
        //        string orderId = purchase.getOrderId();
        //        string orderFormId = purchase.getDeveloperPayload();
        //        if (string.IsNullOrEmpty(orderFormId))
        //        {
        //            orderFormId = PlayerPrefs.GetString(orderId, string.Empty);
        //        }
        //        string purchaseToken = purchase.getToken();
        //        string developerPayload = orderFormId;// purchase.getDeveloperPayload();//已经弃用，永远为空
        //        string packageName = purchase.getPackageName();
        //        string purchaseTime = purchase.getPurchaseTime().ToString();
        //        string purchaseState = purchase.getPurchaseState().ToString();// ? "0" : "1";
                
        //        //SdkMgr.Instance.facebook.LogPurchase(Price);
        //        //SdkMgr.Instance.appsFlyer.Purchase(Price, productId);

        //        LOG.Info("---补单消耗成功-->developerPayload:" + developerPayload + " productId:" + productId + " orderId:" + orderId + " purchaseToken:" + purchaseToken);

        //        PurchaseRecordManager.Instance.AddPurchaseRecord(developerPayload, orderId, purchaseToken, productId, packageName, dataSignature, purchaseTime, purchaseState);

                
        //        //UINetLoadingMgr.Instance.Show();
        //        GameHttpNet.Instance.GetOrderToSubmitForAndroid(developerPayload, orderId, purchaseToken, productId, packageName, dataSignature, purchaseTime, purchaseState, "", (orderID, result) =>
        //        {
        //            AudioManager.Instance.PlayTones(AudioTones.RewardWin);
        //            //UINetLoadingMgr.Instance.Close();
        //            PurchaseRecordManager.Instance.GetOrderToSubmitCallBack(orderId, result);

        //            var data = JsonHelper.JsonToObject<HttpInfoReturn<OrderFormSubmitResultInfo>>(result);
        //            if (data != null)
        //            {
        //                UserDataManager.Instance.orderFormSubmitResultInfo = data;
        //                UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.orderFormSubmitResultInfo.data.bkey);
        //                UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.orderFormSubmitResultInfo.data.diamond);
        //                UserDataManager.Instance.ResetMoney(3, UserDataManager.Instance.orderFormSubmitResultInfo.data.ticket);
        //            }

        //            switch (data.code)
        //            {
        //                case 200:
        //                    {
        //                        UserDataManager.Instance.SetPayUser();
        //                    }
        //                    break;
        //            }

        //            //if (UserDataManager.Instance.orderFormSubmitResultInfo != null && UserDataManager.Instance.orderFormSubmitResultInfo.data != null)
        //            {
        //                PurchaseRecordManager.Instance.RemovePurchaseRecord(orderId);
        //                PlayerPrefs.DeleteKey(orderId);
        //                unCompletePurchases.Dequeue();//移除完成的订单
        //                IssueOrders();//补下一个单
        //            }
        //        });

        //    }
        //    else
        //    {
        //        TalkingDataManager.Instance.DoCharge(EventEnum.PlatformPayCancel, purchase.getOrderId());
        //        //TalkingDataManager.Instance.PlayerCancelCharge(purchase.getOrderId());
        //        AudioManager.Instance.PlayTones(AudioTones.LoseFail);

        //        var Localization = GameDataMgr.Instance.table.GetLocalizationById(148);
        //        UITipsMgr.Instance.PopupTips(Localization, false);

        //        //UITipsMgr.Instance.PopupTips("Payment failed!", false);
        //    }
        //});
    }

    public void ConsumePurchase(Purchase purchase, Action<bool, string> callback)
    {
        _onPayComplete = callback;
        //AndroidHelper.CallStaticMethod(CLASS_NAME, "ConsumePurchase", purchase.getOriginalJson(), purchase.getSignature());
    }
    #endregion
}
#endif
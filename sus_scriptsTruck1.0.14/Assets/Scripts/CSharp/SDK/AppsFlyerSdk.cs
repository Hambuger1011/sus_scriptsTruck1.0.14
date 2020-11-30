#if CHANNEL_ONYX || CHANNEL_SPAIN
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class AppsFlyerSdk
{
    public bool tokenSent;
    public AppsFlyerSdk()
    {
#if !CHANNEL_HUAWEI
        AppsFlyer.setIsDebug(true);

        string afId = AppsFlyer.getAppsFlyerId();


#if UNITY_IOS 

		AppsFlyer.setAppsFlyerKey ("WEYqZmRBi6ZmFww2esj28Y");
        AppsFlyer.setAppID (SdkMgr.IosAppId.ToString());
		AppsFlyer.setIsDebug (true);
		AppsFlyer.getConversionData ();
		AppsFlyer.trackAppLaunch ();

		// register to push notifications for iOS uninstall
		UnityEngine.iOS.NotificationServices.RegisterForNotifications (UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.Sound);
		Screen.orientation = ScreenOrientation.Portrait;

#elif UNITY_ANDROID


        AppsFlyer.setAppID(SdkMgr.packageName);

        //AppsFlyer.init("9vPuhfPj8FrzdSxksrYjMb");
        AppsFlyer.init("WEYqZmRBi6ZmFww2esj28Y");

        // for getting the conversion data
        AppsFlyer.loadConversionData("StartUp");


#endif
        Debug.Log("------AppFlyerId------>" + afId);

#endif

    }


    public void Start()
    {

    }

    public void OnUpdate()
    {
#if !CHANNEL_HUAWEI
#if UNITY_IOS 
		if (!tokenSent) { 
			byte[] token = UnityEngine.iOS.NotificationServices.deviceToken;           
			if (token != null) {     
			//For iOS uninstall
				AppsFlyer.registerUninstall (token);
				tokenSent = true;
			}
		}    
#endif
#endif
    }
    /// <summary>
    /// 设置用户id
    /// </summary>
    /// <param name="vUserId"></param>
    public void SetCustomerUserId(string vUserId)
    {
#if !CHANNEL_HUAWEI
        //AppsFlyer.setCustomerUserID(vUserId);
#endif
    }

    /// <summary>
    /// 记录付费的事件
    /// </summary>
    /// <param name="priceAmount"></param>
    /// <param name="priceCurrency"></param>
    public void Purchase(float priceAmount, string productId, string priceCurrency = "USD")
    {
#if !CHANNEL_HUAWEI && !ENABLE_DEBUG && !UNITY_EDITOR
        Dictionary<string, string> eventValue = new Dictionary<string, string>();
        eventValue.Add("af_revenue", priceAmount.ToString());
        eventValue.Add("af_productId", productId);
        eventValue.Add("af_currency", priceCurrency);
        AppsFlyer.trackRichEvent("af_purchase", eventValue);
#endif
    }

    /// <summary>
    /// 记录App相关的内容
    /// </summary>
    /// <param name="logEvent"></param>
    /// <param name="parameters"></param>
    public void LogAppEvent(string logEvent, Dictionary<string, string> parameters)
    {
        LOG.Info("---AppsFlyer----LogAppEvent---->" + logEvent);

#if !CHANNEL_HUAWEI
#if !UNITY_EDITOR
        AppsFlyer.trackRichEvent(logEvent, parameters);
#endif
#endif
    }

    public void PurchaseStart(string productId)
    {
#if !CHANNEL_HUAWEI
#if !UNITY_EDITOR
        Dictionary<string, string> eventValue = new Dictionary<string, string>();
        eventValue.Add("af_productId", productId);
        AppsFlyer.trackRichEvent("af_StartPurchase", eventValue);
#endif
#endif
    }
}
#endif
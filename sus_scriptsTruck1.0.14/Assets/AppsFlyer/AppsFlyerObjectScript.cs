using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppsFlyerSDK;

// This class is intended to be used the the AppsFlyerObject.prefab

public class AppsFlyerObjectScript : MonoBehaviour , IAppsFlyerConversionData
{

    // These fields are set from the editor so do not modify!
    //******************************//
    public string devKey;
    public string appID;
    public bool isDebug;
    public bool getConversionData;
    //******************************//

    void Start()
    {
    }

    public void Init(string customerInfo)
    {
        // These fields are set from the editor so do not modify!
        //******************************//
        AppsFlyer.setIsDebug(isDebug);
        AppsFlyer.setCustomerUserId(customerInfo); // 设置自定义信息（传当前游戏的gameid），这个必传，格式请参考调用Init的地方
        AppsFlyer.initSDK(devKey, appID, getConversionData ? this : null);
        //******************************//
        AppsFlyer.startSDK();
    }

    // Mark AppsFlyer CallBacks
    public void onConversionDataSuccess(string conversionData)
    {
        AppsFlyer.AFLog("didReceiveConversionData", conversionData);
        Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
        // add deferred deeplink logic here
    }

    public void onConversionDataFail(string error)
    {
        AppsFlyer.AFLog("didReceiveConversionDataWithError", error);
    }

    public void onAppOpenAttribution(string attributionData)
    {
        AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
        Dictionary<string, object> attributionDataDictionary = AppsFlyer.CallbackStringToDictionary(attributionData);
        // add direct deeplink logic here
    }

    public void onAppOpenAttributionFailure(string error)
    {
        AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
    }

    /// <summary>
    /// 发送事件，研发可以在相应事件的触发点调用这个方法进行上传自定义事件。
    /// </summary>
    /// <param name="name">事件明</param>
    /// <param name="extraInfo">附带信息，具体要传而外信息，请参考wiki文档</param>
    public void Track(string name, Dictionary<string, object> extraInfos)
    {
        Dictionary<string, string> extraInfosTmp = new Dictionary<string, string>();
        foreach (var extraInfo in extraInfos)
        {
            extraInfosTmp.Add(extraInfo.Key, extraInfo.Value.ToString());
        }
        AppsFlyer.sendEvent(name, extraInfosTmp);
    }
}

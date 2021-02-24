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
        AppsFlyer.setCustomerUserId(customerInfo); // �����Զ�����Ϣ������ǰ��Ϸ��gameid��������ش�����ʽ��ο�����Init�ĵط�
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
    /// �����¼����з���������Ӧ�¼��Ĵ��������������������ϴ��Զ����¼���
    /// </summary>
    /// <param name="name">�¼���</param>
    /// <param name="extraInfo">������Ϣ������Ҫ��������Ϣ����ο�wiki�ĵ�</param>
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

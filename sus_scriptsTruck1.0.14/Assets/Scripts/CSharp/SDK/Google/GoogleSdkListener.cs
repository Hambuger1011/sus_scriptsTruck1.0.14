#if CHANNEL_ONYX || CHANNEL_SPAIN
//using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.DynamicLinks;
using Helpers;
using UGUI;
using UnityEngine;

public class GoogleSdkListener : MonoBehaviour,ILisenter
{

    // Use this for initialization
    void Awake()
    {
        this.name = "GoogleSdk";
        GameObject.DontDestroyOnLoad(this.gameObject);
        //Debug.LogError("----AddMockPaymentMonoBehaviour--->>>");
    }

    private void Start()
    {
        //PaymentHelper.AddMockPaymentMonoBehaviour(ModuleMockPanel);
    }

    private void InitHandler(Notification vNot)
    {
        //Debug.LogError("----SetMockPaymentMonoBehaviour--->>>");
    }

    public void OnPayInitFail(string msg)
    {
        GoogleSdk.m_initLock = false;
        TalkingDataManager.Instance.GameBillingInitState(false);
        LOG.Error(msg);
    }

    public void OnPayInitSuc(string msg)
    {
        //GoogleSdk.m_initLock = false;
        SdkMgr.Instance.BillingSysInit = true;
        TalkingDataManager.Instance.GameBillingInitState(true);
        LOG.Warn("<color=green>-- Andriod In App --></color>" + msg);
    }

    public void OnPayFail(string msg)
    {
        Debug.LogError("OnPayFail:" + msg);
        SdkMgr.Instance.google.CallPayEvent(false, msg);
    }

    public void OnPayOK(string resultJson)
    {
        LOG.Error("== OnPayOK ==>" + resultJson);
        SdkMgr.Instance.google.CallPayEvent(true, resultJson);

        //var jsonData = JsonMapper.ToObject(resultJson);
        //var result = JsonMapper.ToObject<IabResult>((string)jsonData["result"]);
        //Purchase purchase = new Purchase((string)jsonData["purchase"]);
        //SdkMgr.Instance.google.CallPayEvent(true, purchase);
        ////if (result.responseCode == BillingResponseCode.OK)
        ////{
        ////}
        ////else
        ////{

        ////}
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(400, 100f, 200f, 200f), "测试"))
        {
            DynamicLinkComponents components = new Firebase.DynamicLinks.DynamicLinkComponents(
                // The base Link.
                new System.Uri("https://www.baidu.com/"),
                // The dynamic link URI prefix.
                "https://scriptsuntoldsecrets.page.link/invite") {
                IOSParameters = new Firebase.DynamicLinks.IOSParameters("com.igg.android.scriptsuntoldsecrets"),
                AndroidParameters = new Firebase.DynamicLinks.AndroidParameters("com.igg.android.scriptsuntoldsecrets"),
            };

            // Uri componentsLongDynamicLink = components.LongDynamicLink;
            // do something with: components.LongDynamicLink
            
            
            
            var options = new Firebase.DynamicLinks.DynamicLinkOptions {
                PathLength = DynamicLinkPathLength.Unguessable
            };
            
            Firebase.DynamicLinks.DynamicLinks.GetShortLinkAsync(components, options).ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("GetShortLinkAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    Debug.LogError("GetShortLinkAsync encountered an error: " + task.Exception);
                    return;
                }
            
                // Short Link has been created.
                Firebase.DynamicLinks.ShortDynamicLink link = task.Result;
                Debug.LogFormat("Generated short link {0}", link.Url);
            
                var warnings = new System.Collections.Generic.List<string>(link.Warnings);
                if (warnings.Count > 0) {
                    // Debug logging for warnings generating the short link.
                }
            });
        }
    }

    public void OnQueryPurchase(string json)
    {
        SdkMgr.Instance.google.OnQueryPurchase(json);
    }



    public void OnLoginSuc(string json)
    {
        LOG.Error("OnLoginSuc:" + json);
        var account = JsonHelper.JsonToObject<GoogleSignInAccount>(json);

        string idToken = account.tokenId;
        string email = account.email;
        string userName = account.displayName;
        string userID = account.id;
        string userImageUrl = account.photoUrl;


        var msg = string.Format("id:{0}\nname:{1}\nmail:{2}\ntoken:{3}",
            userID,
            userName,
            email,
            idToken
            );
        LOG.Error(msg);
        //AndroidUtils.ShowDialog("登录成功", msg,"",new JAction.Zero(() =>{ }));

        LoginDataInfo loginInfo = new LoginDataInfo();
        loginInfo.UserId = userID;
        loginInfo.Token = idToken;
        loginInfo.Email = email;
        loginInfo.UserName = userName;
        loginInfo.UserImageUrl = userImageUrl;
        loginInfo.OpenType = SdkMgr.Instance.google.mType;

        SdkMgr.Instance.google.m_isLogin = true;
        EventDispatcher.Dispatch(EventEnum.GoogleLoginSucc, loginInfo);
    }

    public void OnLoginFail(string msg)
    {
        Debug.LogError("OnLoginFail:" + msg);
    }

    public void ReceiveTSHMsg(string msg)
    {
        Debug.LogError("ReceiveTSHMsg:" + msg);
    }
    
       
    public void OnKeyBoardBack(string msg)
    {
        XLuaManager.Instance.GetLuaEnv().DoString(@"GameHelper.OnKeyBoardBack()");
    }


    public void OnScreenAdapation(string msg)
    {
        Debug.LogError("OnScreenAdapationOnScreenAdapation" + msg);
        if (msg == "刘海屏")
        {
            ResolutionAdapter.androidisSafeArea = true;
            ResolutionAdapter.androidNotchSize = new Vector2(55,55);
        }
        else if (msg == "该设备不是刘海屏")
        {
            ResolutionAdapter.androidNotchSize = new Vector2(0, 0);
        }
    }

    
}


public interface ILisenter
{
    //支付初始化失败
    void OnPayInitFail(string msg);

    //支付初始成功
    void OnPayInitSuc(string msg);


    //本地支付成功
    void OnPayOK(string json);

    //支付失败
    void OnPayFail(string msg);


    void OnLoginSuc(string msg);

    void OnLoginFail(string msg);
}
#endif
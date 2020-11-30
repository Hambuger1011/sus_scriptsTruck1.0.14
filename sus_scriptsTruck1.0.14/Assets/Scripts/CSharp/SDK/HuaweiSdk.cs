using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if CHANNEL_HUAWEI
using HuaweiMobileService;
#endif

using Framework;
using UGUI;


public class HuaweiSdk
{

#if CHANNEL_HUAWEI
    static readonly string HUAWEI_CLS_NAME = "com.game.gamelib.huawei.HuaweiInfoMgr";
    static readonly string HUAWEI_CP_ID = "890086000102139941";
    public static readonly string HUAWEI_APP_ID = "100644607";
    static readonly string HUAWEI_MERCHANT_ID = "890086000102139941";
    static readonly string HUAWEI_KEY = "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC9fTslC5x0kKyDTfLepuhbHsiCABsmpW6htbzaq9uDeRm4KEM0guO8tPN3eNMfZqkAxhBZ8IdOuDBVNkYwLZ/BUekCu1HwA5FXIDqAXW8lLQ6sVR6Pqf4ckoBqGh9vadeoovKg1Xjs3QgEpTwfkqSj9pQixlXC1/9akjXR79Qex/VKtO+3PWfZkDHNwPpmEy/Oi3OAMLqD8ZTRidGaR7rSh55wpE";
    static readonly string HUAWEI_SIGN = "vgOJE3QvXJQ7bh0aMiEtIiOSGv62gPNjmzckQIncK2R5w6ROzwpmkHjbUgXUSmnFmU6IPFDGstLDWfzdYEIwy1sbO7AgMBAAECggEAFMvAKHYoQxC3pidxXqcxvAJaXg4V7L1eMWgpJFSVIE4zmObuTT6KMOUpDTUY8orJRHj4RW6k4upK/6cXt+Th8g0DxhV5zh4OHuI+GWVns8xInngvBPRSm58mcb6RMaCLlwnfF/Clt1UgsqXRveNRp18B7fkXY21iLHX8ayBnhsMZ25pGz4PJ1YgpfmBjm/bU8fPoZ/3xSIVBADeUGWtm5piuMcrlo/OiPt3tJi7BjQyPelP3i63EuRBr+J+wGUpGK+kgZvNN49Qo4O/bG5TVosd9Fvx9sL1vt6VFJoNWCUzMKImarxZfdaTmhCUm7SpwkW0j7oYfGV0Mla4vqXwfnQKBgQDeAb79KRlPV2aNT09WUIN084Uh4iiQ9R7Aj4RujxYraIr9OygThg9k8gHTC3X2nFc0N0hQLFNJG0nii0vFUC9qcRV+IEiv4DBjth9rWhwpYbxGkidhaOMInk2cKVFR5B09BQjGIPs1i2ZizxcXJLfLTxt+LqRp+m4fMopa3s7dHwKBgQDagNqrN5yeJbqOmmNnh1FNvHlAoBoFIVJNqlcYp7rdfIul+rNdidYV4HAtLoSDILV3CNK797ZrS1k1Bqn1tdbzDSyer0XrgZ6EfFnDqbOJbijfBSlNPYu+2x/XQgpBjuC3iwab4dGRHFQBLfgULJ9aF3P7nfKtAFNY+HjGEVY55QKBgQCwMpaQeoP58pYT5TREfxY2RegN4l7r3X+QuNcUoD+qKAjXvpGqHSYuqdxE0b3IAoE7kfy0lNltFllnkcKLUX6wbhvuxFSsMbR2Dt+U5imftlAlem4C46n89xcFr804v2FtQoeaKCb4cjDKjy32UlAE7j4VwZMAAaBQ5l3kjR2FZQKBgAQAOADKk+ORbUkPKqLKp4J04QN96vwNECS76bxZ1eIYVbZEUOR1kaX05hjtsR91id0UHEe00XJFfuypopMNMVJh+18sdmtJhE2IuVwLz23ExprOzWLMrbJg8MRQQT/SDetOcGCKUfwGOMOMw+4aoxBTPZxt1/Oy/dOzOQLnblzBAoGAErwHdEAVswZlt1Ed01n8XY07NZDThe6WtUrrvWzwE/pN+Gi6WGDWiAQFHYzpdnnFp06nbG+WOrgZxe7BsvJLuArDMVH6Maym4udlDs8WCUMH2bIIv02gI/3LT1zIR8KR77oBS6PcHyX2XE9zzF8BJ12jcPPc1NIuPsrlay36+Lc=";




    private LoginHandlerImpl mHwLoginHandler;
    private ProductPayImpl mHwProductPayHandler;
    private CheckUpdateCallbackImpl mCheckUpdate;

    private int mType;

#endif

    public void Init()
    {
#if CHANNEL_HUAWEI
        mHwLoginHandler = new LoginHandlerImpl();
        mHwProductPayHandler = new ProductPayImpl();
        mCheckUpdate = new CheckUpdateCallbackImpl();
#endif
    }

    public void showFloatWindow()
    {
        // AndroidHelper.CallStaticMethod("com.sdk.HwSdk", "showFloatWindow");
    }

    /**
     * 游戏隐藏浮标示例 | Game Hidden Floating Indicator example
     */
    public void hideFloatWindow()
    {
        // AndroidHelper.CallStaticMethod("com.sdk.HwSdk", "hideFloatWindow");
    }

    public void Logout()
    {
        hideFloatWindow();
    }

    public void Login(int vType)
    {
#if CHANNEL_HUAWEI
        mType = vType;
#if UNITY_EDITOR
        HwUserInfo userData = new HwUserInfo();
        userData.displayName = "OnyxTest" + Time.time;
        userData.playerId = "OnyxTest" + Time.time;
        userData.playerLevel = 1;
        userData.gameAuthSign = "n823b4nndifoan12894132ni034182930nds0ifaj102n34n9sdjf0n1023n401";
        userData.isAuth = 1;
        userData.ts = Time.time.ToString();
        userData.type = vType;

        UserDataManager.Instance.hwLoginInfo = userData;
        EventDispatcher.Dispatch(EventEnum.HuaweiLoginInfo, userData);
        return;
#endif
        if (mHwLoginHandler != null) mHwLoginHandler.Type = mType;
        int isForceLogin = 1;
        GameAgent.NewInstance().Login(isForceLogin, mHwLoginHandler);
#endif
    }

    #if CHANNEL_HUAWEI
    /// <summary>
    /// 来自0:三个登陆按钮的界面（hw初始化登陆界面） 1：侧边栏的快捷登陆，2：:侧边栏设置界面、3：读书里面的设置界面
    /// </summary>
    /// <param name="vType"></param>
   


    public void ProductPay(string vSign ,string vProductId,string vOrderId,string vNotifyUrl)
    {
        string resultSign = vSign.Replace("Z", "5");
        //LOG.Info("--vSign-->" + vSign +"--result-->"+ resultSign +"--productId-->" + vProductId + "--vOrderId-->" + vOrderId + "--url-->" + vNotifyUrl);
        string vProductName =  "上海锦族网络科技有限公司";
        ProductPayRequest request = new ProductPayRequestBuild().Info(vProductName, vProductId, vOrderId).OptServiceCatalog("X6").OptUrl(vNotifyUrl).RsaSign(string.Concat(HUAWEI_KEY, resultSign, HUAWEI_SIGN)).Build();
        Purchasing.ProductPay(request, mHwProductPayHandler);
    }

    /// <summary>
    /// 检查更新状态
    /// </summary>
    public void CheckUpdate()
    {
        Purchasing.CheckUpdate(mCheckUpdate);
    }

#endif

}

 #if CHANNEL_HUAWEI
public class LoginHandlerImpl:ILoginHandler
{

    private HwUserInfo mUserData;
    private int mType;

    public void OnChange()
    {

    }

    public int Type { 
        set { mType = value; }
        get { return mType; }
    }

    public void OnResult(int resultCode, GameUserData response)
    {
        CTimerManager.Instance.AddTimer(0, 1, (_) =>
          {

              if (resultCode == 0)
              {
                  LOG.Info("---hwLogin---resultCode--->" + resultCode + "==response isAuth==>" + response.isAuth + "--displayName-->" + response.displayName + "--playerId-->" + response.playerId
                        + "--playerLevel-->" + response.playerLevel + "--playerSign-->" + response.gameAuthSign + "--ts-->" + response.ts);

                  if (response.isAuth == 1)
                  {
                      mUserData = new HwUserInfo();
                      mUserData.displayName = response.displayName;
                      mUserData.playerId = response.playerId;
                      mUserData.playerLevel = response.playerLevel;
                      mUserData.gameAuthSign = response.gameAuthSign;
                      mUserData.isAuth = response.isAuth;
                      mUserData.ts = response.ts;
                      mUserData.type = Type;

                      UserDataManager.Instance.hwLoginInfo = mUserData;
                      EventDispatcher.Dispatch(EventEnum.HuaweiLoginInfo, mUserData);
                      SdkMgr.Instance.hwSDK.showFloatWindow();
                  }
              }
              else
              {
                  mUserData = null;
                  UITipsMgr.Instance.PopupTips("Login Fail " + resultCode, false);
              }
          });
    }
}

public class ProductPayImpl:IProductPayHandler
{
    private string mRequestId;
    public void OnResult(int resultCode, ProductPayResponse response)
    {
        CTimerManager.Instance.AddTimer(0, 1, (_) =>
        {
            if (resultCode == 0)
            {
                LOG.Info("----hwPay Result--->" + resultCode + "--country-->" + response.country + "--currency-->" + response.currency + "--errMsg-->" + response.errMsg + "--merchantId-->" + response.merchantId + "--microsAmount-->" + response.microsAmount
                + "--orderID-->" + response.orderID + "--productNo-->" + response.productNo + "--requestId-->" + response.requestId + "--returnCode-->" + response.returnCode + "--sign-->" + response.sign + "--time-->" + response.time);

                mRequestId = response.requestId;

                GameHttpNet.Instance.SubmitChargeOrder(mRequestId, submitHandler);

            }
            else
            {
                LOG.Info("----hwPay Result--->" + resultCode);
                if (response != null && !string.IsNullOrEmpty(response.requestId))
                {
                    LOG.Info("----hwPay Result--->" + resultCode + "---requestId---->" + response.requestId);
                    if (resultCode == 30000)
                        GameHttpNet.Instance.UserOrderCancel(response.requestId, resultCode.ToString(), 3, OrderCancelCallBack);
                    else
                        GameHttpNet.Instance.UserOrderCancel(response.requestId, resultCode.ToString(), 3, OrderCancelCallBack);
                }
            }
        });
    }

    private void OrderCancelCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----OrderCancelCallBack---->" + result);
    }

    private void submitHandler(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----hw submitHandler---->" + result);
        if (result.Equals("error"))
        {
            return;
        }

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    AudioManager.Instance.PlayTones(AudioTones.RewardWin);

                    UserDataManager.Instance.hwBuyResultInfo = JsonHelper.JsonToObject<HttpInfoReturn<HwBuyResultInfo>>(result);
                    if (UserDataManager.Instance.hwBuyResultInfo != null && UserDataManager.Instance.hwBuyResultInfo.data != null)
                    {
                        UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.hwBuyResultInfo.data.bkey);
                        UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.hwBuyResultInfo.data.diamond);

                        UITipsMgr.Instance.PopupTips("Payment Successful!", false);

                        if (!string.IsNullOrEmpty(mRequestId))
                        {
                            PurchaseRecordManager.Instance.SendRecordToAppsFlyer(mRequestId);
                            TalkingDataManager.Instance.OnChargeSuccess(mRequestId);
                        }
                            
                    }
                }
                else if (jo.code == 201)     //充值失败
                {
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                    LOG.Info("---充值失败-->");
                    UITipsMgr.Instance.PopupTips("Payment Failed!", false);
                }
            }, null);
        }
    }
}

public class CheckUpdateCallbackImpl : ICheckUpdateHandler
{
    public void OnResult(int resultCode)
    {
        CTimerManager.Instance.AddTimer(0, 1, (_) =>
        {
            LOG.Info("--Huawei has new version-->" + resultCode);
        });
    }
}

#endif
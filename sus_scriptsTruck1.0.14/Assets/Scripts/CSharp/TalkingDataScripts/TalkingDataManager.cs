using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// TalkingData 数据记录平台
/// </summary>
[XLua.LuaCallCSharp]
public class TalkingDataManager : Singleton<TalkingDataManager>
{
    private static string TalkingDataAppID = "27CA4F3489B04944AD70A64FDD7BBA71";


    public Dictionary<string, ChapterFirstRecord> ChapterFirRecordDic;

    //private TDGAAccount account;
    public bool enableReportError = true;


    public void InitSdk()
    {

#if UNITY_IPHONE
#if UNITY_5 || UNITY_5_6_OR_NEWER
		UnityEngine.iOS.NotificationServices.RegisterForNotifications(
			UnityEngine.iOS.NotificationType.Alert |
			UnityEngine.iOS.NotificationType.Badge |
			UnityEngine.iOS.NotificationType.Sound);
#else
		NotificationServices.RegisterForRemoteNotificationTypes(
			RemoteNotificationType.Alert |
			RemoteNotificationType.Badge |
			RemoteNotificationType.Sound);
#endif
#endif

        string channel = "IOS";
#if UNITY_IOS
        channel = "IOS";
#else
        channel = "Andriod";
#endif

        //TalkingDataGA.OnStart(TalkingDataAppID, "Onyx_" + channel);
    }

    /// <summary>
    /// 获取设备ID
    /// </summary>
    /// <returns></returns>
    public string GetDeviceId()
    {
        return "oihfaosdiohou2ohfoihsiodh2ofosdhof";//TalkingDataGA.GetDeviceId();
    }

    /// <summary>
    /// 设置用户id
    /// </summary>
    /// <param name="vAccount"></param>
    public void SetAccountId(string vAccount)
    {
//        account = TDGAAccount.SetAccount(vAccount);
//#if CHANNEL_ONYX
//        SdkMgr.Instance.appsFlyer.SetCustomerUserId(vAccount);
//#endif
    }

    /// <summary>
    /// 设置用户昵称
    /// </summary>
    /// <param name="vName"></param>
    public void SetAccountName(string vName)
    {
        //if (account != null)
            //account.SetAccountName(vName);
    }

    /// <summary>
    /// 设置账户的类型
    /// </summary>
    /// <param name="vType"></param>
    public void SetAccountName()
    {
        //if (account != null)
            //account.SetAccountType(AccountType.REGISTERED);
    }


    /// <summary>
    /// 充值请求
    /// </summary>
    /// <param name="orderId">订单号</param>
    /// <param name="iapId"> 商品名称 </param>
    /// <param name="currencyAmount"> 价格 </param>
    /// <param name="currencyType"> 人民币 CNY；美元 USD；欧元 EUR </param>
    /// <param name="virtualCurrencyAmount"> 虚拟币金额 </param>
    /// <param name="paymentType"> 支付的途径 </param>
    public void OnChargeRequest(string orderId, string iapId, double currencyAmount,
            string currencyType, double virtualCurrencyAmount, string paymentType)
    {
        //TDGAVirtualCurrency.OnChargeRequest(orderId, iapId, currencyAmount, currencyType, virtualCurrencyAmount, paymentType);
        //OnCharageStart(orderId);
    }

    /// <summary>
    /// 充值成功
    /// </summary>
    /// <param name="orderId">  订单号 </param>
    public void OnChargeSuccess(string orderId)
    {
        //TDGAVirtualCurrency.OnChargeSuccess(orderId);
        //OnCharageFinish(orderId);
    }

    //开始付费
    private void OnCharageStart(string orderId)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("PlayerStartCharge_", orderId);
        //OnEvent("PlayerStartCharge_" + SdkMgr.Instance.GameVersion(), dic, true);
    }

    //付费完成（即：提交订单）
    private void OnCharageFinish(string orderId)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("PlayerFinishCharge_", orderId);
        //OnEvent("PlayerFinishCharge_" + SdkMgr.Instance.GameVersion(), dic, true);
    }

    /// <summary>
    /// 取消充值
    /// </summary>
    /// <param name="orderId">  订单号 </param>
    public void PlayerCancelCharge(string orderId)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("PlayerCancelCharge_", orderId);
        //OnEvent("PlayerCancelCharge_" + SdkMgr.Instance.GameVersion(), dic, true);
    }

    /// <summary>
    /// 赠予虚拟币
    /// </summary>
    /// <param name="virtualCurrencyAmount"> 虚拟币金额。 </param>
    /// <param name="reason"> 赠送虚拟币原因/类型。 </param>
    public void OnReward(double virtualCurrencyAmount, string reason)
    {
        //TDGAVirtualCurrency.OnReward(virtualCurrencyAmount, reason);
    }


    /// <summary>
    /// 记录付费点
    /// </summary>
    /// <param name="item">某个消费点的编号</param>
    /// <param name="itemNumber">消费数量</param>
    /// <param name="priceInVirtualCurrency"> 虚拟币单价 </param>
    public void OnPurchase(string item, int itemNumber, double priceInVirtualCurrency)
    {
        //TDGAItem.OnPurchase(item, itemNumber, priceInVirtualCurrency);
    }


    /// <summary>
    /// 消耗物品或服务等
    /// </summary>
    /// <param name="item">某个消费点的编号</param>
    /// <param name="itemNumber">消费数量</param>
    public void OnUse(string item, int itemNumber)
    {
        //TDGAItem.OnUse(item, itemNumber);
    }


    /// <summary>
    /// 接受或进入任务
    /// </summary>
    /// <param name="missionId">任务的编号</param>
    public void OnBegin(string missionId)
    {
        //TDGAMission.OnBegin(missionId);
    }

    /// <summary>
    /// 开始读故事
    /// </summary>
    /// <param name="missionId"></param>
    public void onStart(string missionId)
    {
        //TDGAMission.OnCompleted(missionId);

        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("ReadChapterStart", missionId);
        //OnEvent(missionId, dic, true);
    }

    /// <summary>
    /// 任务完成
    /// </summary>
    /// <param name="missionId">任务的编号</param>
    public void onCompleted(string missionId)
    {
        //TDGAMission.OnCompleted(missionId);

        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("ReadChapterComplete", missionId);
        //OnEvent(missionId, dic, true);
    }

    /// <summary>
    /// 拍照
    /// </summary>
    public void ScreenShot(string missionId)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("Screenshot", missionId);
        //OnEvent(missionId, dic, true);
    }

    /// <summary>
    /// 打开App
    /// </summary>
    public void OpenApp(string vStepName)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("OpenApp", GameHttpNet.Instance.UUID);
        string resultStr = "";
#if UNITY_IOS
                resultStr = vStepName.Replace("##", "1_ios");
#else
        resultStr = vStepName.Replace("##", "2_android");
#endif
        OnEvent(resultStr, dic, true);
    }

    /// <summary>
    /// 执行登录
    /// </summary>
    /// <param name="vInfo">信息</param>
    /// <param name="vType"> 0 guest ,1 facebook , 2 google</param>
    public void LoginAccount(string vInfo,int vType = 0)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("imme", GameHttpNet.Instance.UUID);
        string resultStr = "";
#if UNITY_IOS
        resultStr = vInfo.Replace("##", "1_ios");
#else
        resultStr = vInfo.Replace("##", "2_android");
#endif
        if(vType ==1)
            resultStr = resultStr + "_Facebook";
        else if(vType == 2)
            resultStr = resultStr + "_GooglePlay";

        OnEvent(resultStr, dic, true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vInfo"></param>
    /// <param name="vType">1 IosPay ,2 GooglePay</param>
    public void DoCharge(string vInfo,string vOrderId="")
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("imme", GameHttpNet.Instance.UUID);
        if (!string.IsNullOrEmpty(vOrderId))
            dic.Add("orderId", vOrderId);
        string resultStr = "";
#if UNITY_IOS
        resultStr = vInfo.Replace("##", "1_ios");
#else
        resultStr = vInfo.Replace("##", "2_android");
#endif
        OnEvent(resultStr, dic, true);
    }
   

    // 在游戏程序的event事件中加入下面的代码，也就成功的添加了一个简单的事件到您的游戏程序中
    private void OnEvent(string actionId, Dictionary<string, object> parameters, bool vSendToAppsflyer = false)
    {
        try{

        //TalkingDataGA.OnEvent(actionId, parameters);

            //if (SdkMgr.Instance.facebook != null)
            //SdkMgr.Instance.facebook.LogAppEvent(actionId, parameters);


            Dictionary<string, string> appFlyerDic = new Dictionary<string, string>();
            foreach (var kv in parameters)
            {
                appFlyerDic.Add(kv.Key, kv.Value.ToString());
            }

#if CHANNEL_ONYX
        if (SdkMgr.Instance.appsFlyer != null && vSendToAppsflyer)
            SdkMgr.Instance.appsFlyer.LogAppEvent(actionId, appFlyerDic);
#endif
            
        }catch(System.Exception ex){
            LOG.Error(actionId+"\n"+ex);
        }
    }


    /// <summary>
    /// 选择某个选项
    /// </summary>
    /// <param name="vDialogID"></param>
    /// <param name="vIndex"></param>
    public void SelectOptions(int vBookId, int vDialogID, int vIndex, int vNeedPay = 0, int vPrice = 0)
    {
        string payMsg = vNeedPay == 1 ? "_PAY_" + vPrice : "";
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("BookId", vBookId);
        //OnEvent("SelectOptions_" + vBookId + "_" + vDialogID + "_" + vIndex + payMsg, dic);
        //OnEvent("SelectOptions_" + vBookId + "_" + vDialogID + "_Total", dic);
    }


    /// <summary>
    /// 新手选择哪个故事
    /// </summary>
    /// <param name="vIndex"></param>
    public void SelectBooksInEnter(int vBookId)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("BookId", vBookId);
        //OnEvent("NewPlayerBook_" + vBookId, dic);
    }

    /// <summary>
    /// 商店购买了哪个商品
    /// </summary>
    /// <param name="vTypeId"></param>
    /// <param name="price"></param>
    public void ShopBuy(int vTypeId, string price)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("TypeId", vTypeId);
        //OnEvent("ShopBuy_" + vTypeId + "_" + price, dic);
    }

    /// <summary>
    /// 商品推荐购买了哪个商品
    /// </summary>
    /// <param name="vTypeId"></param>
    /// <param name="price"></param>
    public void Parameters(int vTypeId, string price)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("TypeId", vTypeId);
        //OnEvent("Parameters_" + vTypeId + "_" + price, dic);
    }
    /// <summary>
    /// 选择哪个主角
    /// </summary>
    /// <param name="vIndex"></param>
    public void SelectPlayer(int vBookId, int vIndex)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("BookId", vBookId);
        //OnEvent("SelectPlayer_" + vBookId + "_" + vIndex, dic);
    }

    /// <summary>
    /// 选择哪个npc
    /// </summary>
    /// <param name="vIndex"></param>
    public void SelectNpc(int vBookId, int vIndex)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("BookId", vBookId);
        //OnEvent("SelectNpc_" + vBookId + "_" + vIndex, dic);
    }

    /// <summary>
    /// 选择什么衣服
    /// </summary>
    /// <param name="vDialogID"></param>
    /// <param name="vIndex"></param>
    public void SelectCloths(int vBookId, int vDialogID, int vIndex, int vNeedPay = 0, int vPrice = 0)
    {
        string payMsg = vNeedPay == 1 ? "_PAY_" + vPrice : "";
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("BookId", vBookId);
        //OnEvent("SelectCloths_" + vBookId + "_" + vDialogID + "_" + vIndex + payMsg, dic);
    }

    /// <summary>
    /// 记录玩家手机语言
    /// </summary>
    /// <param name="vLanguge"></param>
    public void RecordPlayerLanguage(string vLanguge)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("Language", vLanguge);
        //OnEvent("Language_" + vLanguge, dic);
    }

    /// <summary>
    /// 记录协议错误
    /// </summary>
    /// <param name="vLanguge"></param>
    public void RecordProtocolError(string vProtocol, Dictionary<string, string> parameters)
    {
        //OnEvent("ProtocolError_" + vProtocol + "_version:" + SdkMgr.Instance.GameVersion(), DicStrToDicObj(parameters));

//         if (!enableReportError)
//         {
//             return;
//         }
//         if (vProtocol.EndsWith("api_setClientError"))
//         {
//             UnityEngine.Debug.LogError("api_setClientError loop!!!");
//             return;
//         }
//         vProtocol = GameHttpNet.Instance.GameUrlHead + "/" + vProtocol;
//         var json = JsonHelper.ObjectToJson(parameters);
//         UnityEngine.Debug.LogError("上报api_name:" + vProtocol + "\n" + json);
// #if UNITY_EDITOR
//         return;
// #endif
        //GameHttpNet.Instance.SetClientError(vProtocol, json, (msg) => { });
    }

    public void RecordProtocolError(string vProtocol, XLua.LuaTable luaTable)
    {

        using (luaTable)
        {
            Dictionary<string, string> param = luaTable.Cast<Dictionary<string, string>>();
            RecordProtocolError(vProtocol, param);
        }
    }

    /// <summary>
    /// 记录协议超时
    /// </summary>
    /// <param name="vMsg"></param>
    public void ProtocolOverTime(string vProtocol, Dictionary<string, string> parameters)
    {
        //OnEvent("ProtocolOverTime" + vProtocol + "_version:" + SdkMgr.Instance.GameVersion(), DicStrToDicObj(parameters));
    }

    /// <summary>
    /// 游戏付费启动结果
    /// </summary>
    /// <param name="value"></param>
    public void GameBillingInitState(bool value)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("GameBillingInitState", value.ToString());
        //OnEvent("GameBillingInitState_" + value.ToString() + "_" + SdkMgr.Instance.GameVersion(), dic);
    }

    /// <summary>
    /// 游戏付费启动结果
    /// </summary>
    /// <param name="value"></param>
    public void GameAdmobInitState(bool value)
    {

        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("GameAdmobAdsInitState", value.ToString());
        //OnEvent("GameAdmobAdsInitState_" + value.ToString() + "_" + SdkMgr.Instance.GameVersion(), dic);
    }

    /// <summary>
    /// 看广告
    /// 1:点击观看广告
    /// 2:完成观看广告
    /// </summary>
    /// <param name="vType"></param>
    public void WatchTheAds(int vType)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        switch (vType)
        {
            case 1://点击观看广告
                dic.Add("WatchTheAdsStart", "Start");
                //OnEvent("WatchTheAdsStart", dic, true);
                break;
            case 2://完成观看广告
                dic.Add("WatchTheAdsComplete", "Complete");
                //OnEvent("WatchTheAdsComplete", dic, true);
                break;
            case 3://非付费玩家看完章节弹广告
                dic.Add("ChapterCompleteWatchAdsStart", "Start");
               // OnEvent("ChapterCompleteWatchAdsStart", dic, true);
                break;
            case 4://非付费玩家看完章节完成广告观看
                dic.Add("ChapterCompleteWatchAdsComplete", "Complete");
                //OnEvent("ChapterCompleteWatchAdsComplete", dic, true);
                break;
        }
    }

    /// <summary>
    /// 游戏收费相关
    /// </summary>
    public void GameCharge(string vProductName)
    {
#if CHANNEL_ONYX
        if (SdkMgr.Instance.appsFlyer != null)
            SdkMgr.Instance.appsFlyer.PurchaseStart(vProductName);
#endif
    }

    /// <summary>
    /// 游戏分享相关
    /// </summary>
    public void ShareRecord(int vType)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        switch (vType)
        {
            case 1://开始分享
                dic.Add("FBShareStart", "Start");
                //OnEvent("FBShareStart", dic, true);
                break;
            case 2://分享成功
                dic.Add("FBShareComplete", "Complete");
                //OnEvent("FBShareComplete", dic, true);
                break;
        }
    }

    /// <summary>
    /// 幸运转盘
    /// </summary>
    /// <param name="vIndex">转到哪个</param>
    /// <param name="vDiamond">获得的钻石</param>
    /// <param name="vKey">获得的钥匙</param>
    /// <param name="vTick">获得的票券</param>
    public void LuckRollerRecord(int vIndex, int vType, int vNum)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("Index", vIndex);
        dic.Add("Type", vType);
        dic.Add("Num", vNum);
        switch (vType)
        {
            case 1:
                //OnEvent("LuckRoller_Index_" + vIndex + "_Key_" + vNum, dic);
                break;
            case 2:
                //OnEvent("LuckRoller_Index_" + vIndex + "_Diamond_" + vNum, dic);
                break;
            case 3:
                //OnEvent("LuckRoller_Index_" + vIndex + "_Tick_" + vNum, dic);
                break;
            case 0:
                //OnEvent("LuckRoller_Index_" + vIndex + "_nothing", dic);
                break;
        }
    }

    /// <summary>
    /// 转化string to object
    /// </summary>
    /// <param name="vDic"></param>
    /// <returns></returns>
    public Dictionary<string, object> DicStrToDicObj(IDictionary<string, string> vDic)
    {
        Dictionary<string, object> result = new Dictionary<string, object>();
        if (vDic == null) return result;
        foreach (var item in vDic)
        {
            result.Add(item.Key, item.Value);
        }
        return result;
    }


    /// <summary>
    /// 记录 下订单
    /// </summary>
    /// <param name="vOrderId">订单id</param>
    /// <param name="vTotal">总费用</param>
    /// <param name="vCurrencyType">货币类型</param>
    public void PlaceOrder(string vOrderId, int vTotal, string vCurrencyType)
    {
        //TalkingDataOrder order = TalkingDataOrder.CreateOrder(vOrderId.ToString(), vTotal, vCurrencyType);
        //TalkingDataPlugin.OnPlaceOrder("", order);
    }

    /// <summary>
    /// 记录 消费成功
    /// </summary>
    /// <param name="vOrderId">订单id</param>
    /// <param name="vTotal">总费用</param>
    /// <param name="vCurrencyType">货币类型</param>
    /// <param name="vPayType">支付类型</param>
    public void OrderPaySucc(string vOrderId, int vTotal, string vCurrencyType, string vPayType)
    {
        //TalkingDataOrder order = TalkingDataOrder.CreateOrder(vOrderId.ToString(), vTotal, vCurrencyType);
        //TalkingDataPlugin.OnOrderPaySucc("", vPayType, order);
    }

    public void Dispose()
    {
        //TalkingDataPlugin.SessionStoped();
    }
}

/// <summary>
/// 章节首记录
/// </summary>
public class ChapterFirstRecord
{
    public int BookID;
    public int ChapterID;
    public int DialogID;
}

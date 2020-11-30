using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 平台购买管理
/// </summary>

[XLua.Hotfix,XLua.LuaCallCSharp,XLua.CSharpCallLua]
public class PurchaseManager : MonoBehaviour
{
    private string RequestProductListStr = string.Empty;

    private int _myOrderId = 1;
    private int _buyProductReceiptIndex = 1;
    //是否在loading的时候请求了平台商品列表
    [HideInInspector]
    public bool HasToReqProductListInLoading = false;
    //是否有从平台请求回来商品列表
    [HideInInspector]
    public bool HasProductList = false;
    //购买成功后，发送给服务端，服务端是否确认返回了
    [HideInInspector]
    public bool BuySuccSendServerReturn = false;
    //是否已经发送没有校验成功的token给服务器，在登录到主城的时候
    [HideInInspector]
    public bool hasSendUnVerifyTokenToServer = false;
    //当前服务器时间
    public int CurServerTime = 0;
    //当前购买返回后，发送给服务器端的orderId
    public int CurSendServerOrderId = -1;
    //从平台请求回来的商品列表
    public List<PlatformProductItem> PlatformProductList = new List<PlatformProductItem>();
    //从平台请求回来的商品字典（方便获取价格）
    public Dictionary<string, PlatformProductItem> PlatformProductDic = new Dictionary<string, PlatformProductItem>();
    //已经购买成功了的产品信息
    public Dictionary<string, HasBuyProductItem> HasBuySuccProductDic = new Dictionary<string, HasBuyProductItem>();
    //购买间隔时间
    private int _buyIntervalTime = 2;

    private Action<bool, string> mPayCallBack;
    private string mOrderId;//向服务端申请的订单号

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private static PurchaseManager _PurchaseManager = null;

    public static PurchaseManager Instance
    {
        get
        {
            if (_PurchaseManager == null)
            {
                _PurchaseManager = (new GameObject("PurchaseManager")).AddComponent<PurchaseManager>();
            }

            return _PurchaseManager;
        }
    }


#if UNITY_IOS || UNITY_EDITOR
    // [DllImport("__Internal")]
    // private static extern void GetProductList( string vIdList );
    // [DllImport("__Internal")]
    // private static extern void DoBuy(string vProductID);
    
    // 付费
    public void Purchase( string productId,string orderFormId,Action<bool,string> callback) {
        if (CheckCanBuy())
        {
            mOrderId = orderFormId;
            mPayCallBack = callback;


#if UNITY_EDITOR || PAY_TEST
            SdkMgr.TestCommitPurchase(orderFormId, productId, GetOrderToSubmitCallBack);
            var isTest = true;
            if (isTest)
            {
                return;
            }
#endif

            TalkingDataManager.Instance.DoCharge(EventEnum.PlatformPayStart, mOrderId);
            // DoBuy(productId);
        }
    }
    //获取根据商品id列表获取对应平台商品价格列表
    public void RequestProductList(string vIdList)
    {
        // GetProductList(vIdList);
    }
#endif


    /// <summary>
    /// 获得当前购买物品时的订单索引
    /// </summary>
    public int BuyProdectReceiptIndex
    {
        set { _buyProductReceiptIndex = value; }
        get { return _buyProductReceiptIndex++; }
    }


    /// <summary>
    /// 根据平台请求商品列表
    /// </summary>
    public void ToRequestProductList()
    {
        RequestProductListStr = NeedRequestProductListStr();
        if(!string.IsNullOrEmpty(RequestProductListStr))
        {
            //获取ios平台的商品列表
#if UNITY_IOS && !UNITY_EDITOR
            RequestProductList(RequestProductListStr);
#endif
            
#if CHANNEL_ONYX || CHANNEL_SPAIN
            SdkMgr.Instance.google.QuerySkuDetails(RequestProductListStr);
#endif

        }
    }

    /// <summary>
    /// 需要请求的商品ID列表
    /// </summary>
    /// <returns></returns>
    public string NeedRequestProductListStr()
    {
        if (string.IsNullOrEmpty(RequestProductListStr))
        {
            if(UserDataManager.Instance.shopList !=null && UserDataManager.Instance.shopList.data != null)
            {
                List<ShopItemInfo> bkeyarr = UserDataManager.Instance.shopList.data.key_list;
                int len = 0;
                string result = string.Empty;
                if(bkeyarr != null)
                {
                    len = bkeyarr.Count;
                    for(int i = 0;i<len;i++)
                    {
                        ShopItemInfo itemInfo = bkeyarr[i];
                        if (itemInfo != null)
                        {
                            result += itemInfo.product_id;
                            result += ",";
                        }
                    }
                }

                List<ShopItemInfo> diamondarr = UserDataManager.Instance.shopList.data.diamond_list;
                if (diamondarr != null)
                {
                    len = diamondarr.Count;
                    for (int i = 0; i < len; i++)
                    {
                        ShopItemInfo itemInfo = diamondarr[i];
                        if (itemInfo != null)
                        {
                            result += itemInfo.product_id;
                            result += ",";
                        }
                    }
                }

                List<ShopItemInfo> packagearr = UserDataManager.Instance.shopList.data.package_list;
                if (packagearr != null)
                {
                    len = packagearr.Count;
                    for (int i = 0; i < len; i++)
                    {
                        ShopItemInfo itemInfo = packagearr[i];
                        if (itemInfo != null)
                        {
                            result += itemInfo.product_id;
                            result += ",";
                        }
                    }
                }

                RequestProductListStr = result;
                LOG.Info("--需要请求的商品ID列表-->" + RequestProductListStr);
            }
        }
       return RequestProductListStr;
    }

    public string CheckPriceByProduct(string vProductName)
    {
        if (UserDataManager.Instance.shopList != null && UserDataManager.Instance.shopList.data != null)
        {
            List<ShopItemInfo> bkeyarr = UserDataManager.Instance.shopList.data.key_list;
            int len = 0;
            if (bkeyarr != null)
            {
                len = bkeyarr.Count;
                for (int i = 0; i < len; i++)
                {
                    ShopItemInfo itemInfo = bkeyarr[i];
                    if (itemInfo != null && itemInfo.product_id == vProductName)
                    {
                        return itemInfo.price;
                    }
                }
            }

            List<ShopItemInfo> diamondarr = UserDataManager.Instance.shopList.data.diamond_list;
            if (diamondarr != null)
            {
                len = diamondarr.Count;
                for (int i = 0; i < len; i++)
                {
                    ShopItemInfo itemInfo = diamondarr[i];
                    if (itemInfo != null && itemInfo.product_id == vProductName)
                    {
                        return itemInfo.price;
                    }
                }
            }

            List<ShopItemInfo> packagearr = UserDataManager.Instance.shopList.data.package_list;
            if (packagearr != null)
            {
                len = packagearr.Count;
                for (int i = 0; i < len; i++)
                {
                    ShopItemInfo itemInfo = packagearr[i];
                    if (itemInfo != null && itemInfo.product_id == vProductName)
                    {
                        return itemInfo.price;
                    }
                }
            }
            //List<ShopItemInfo> ticketarr = UserDataManager.Instance.shopList.data.ticketarr;
            //if (ticketarr != null)
            //{
            //    len = ticketarr.Count;
            //    for (int i = 0; i < len; i++)
            //    {
            //        ShopItemInfo itemInfo = ticketarr[i];
            //        if (itemInfo != null && itemInfo.productName == vProductName)
            //        {
            //            return itemInfo.price;
            //        }
            //    }
            //}
        }
        return "";
    }

    /// <summary>
    /// 商品列表返回
    ///  格式： ID^Price # ID^Price 
    /// </summary>
    /// <param name="vList"></param>
    public void ReturnProductList(string vInfo)
    {
        LOG.Info("--ReturnProductList--->" + vInfo);
        HasProductList = false;
        PlatformProductList = new List<PlatformProductItem>();
        PlatformProductDic = new Dictionary<string, PlatformProductItem>();
        string[] list = vInfo.Split('#');
        int len = list.Length;
        PlatformProductItem item;
        for(int i = 0;i<len;i++)
        {
            string[] itemList = list[i].Split('^');
            int itemLen = itemList.Length;
            if(itemLen > 3)
            {
                item = new PlatformProductItem();
                item.productID = itemList[0];
                item.currency = itemList[1];
                item.price = itemList[2];
                item.unitPrice = itemList[3];
                PlatformProductList.Add(item);
                PlatformProductDic.Add(item.productID, item);
                if (!HasProductList)
                    HasProductList = true;

                LOG.Info("==>item.productId:" + item.productID + " item.currency:" + item.currency + " item.price:" + item.price + " item.unitPrice:" + item.unitPrice);
            }
        }
    }

    /// <summary>
    /// 获取对应商品的信息
    /// </summary>
    /// <param name="vProdectID"></param>
    /// <returns></returns>
    public PlatformProductItem GetProductInfo(string vProdectID)
    {
        PlatformProductItem productItem = null;
        if (PlatformProductDic == null)
            return null;
        if(PlatformProductDic.TryGetValue(vProdectID ,out productItem))
        {
            return productItem;
        }
        return productItem;
    }


    /// <summary>
    /// ios拉起的订单恢复回调
    /// </summary>
    /// <param name="vInfo"></param>
    public void TransactionRestored(string vInfo)
    {
        LOG.Info("=== 购买成功 ===ReturnInfo====>" + vInfo);
        string[] infoList = vInfo.Split('#');
        if (infoList.Length > 1)
        {
            HasBuyProductItem hasBuyItem = new HasBuyProductItem();
            if(string.IsNullOrEmpty(mOrderId))
            {
                mOrderId = PlayerPrefs.GetString(infoList[0]);
                if (string.IsNullOrEmpty(mOrderId))
                {
                    Debug.LogError("订单号丢失:" + infoList[0]);
                }
            }
            hasBuyItem.myOrderId = mOrderId;
            hasBuyItem.productId = infoList[0];
            hasBuyItem.token = infoList[1];
            hasBuyItem.transactionId = infoList[2];
            PlatformProductItem productItem = null;
            if (PlatformProductDic != null)
            {
                if (PlatformProductDic.TryGetValue(hasBuyItem.productId, out productItem))
                {
                    hasBuyItem.currency = productItem.currency;
                    hasBuyItem.money = productItem.price;
                    hasBuyItem.storeCommodityId = GetStoreCommodityByProductId(hasBuyItem.productId);
                }
            }
            PurchaseRecordManager.Instance.DoTransactionRestored(hasBuyItem);
        }
    }


    //购买数据通知了服务端并返回了结果
    public void BuySuccSendServerDataReturnHandler()
    {
        BuySuccSendServerReturn = true;
    }

   
    //根据产品id，获得配置对应的商品id
    private int GetStoreCommodityByProductId(string vProductId)
    {
        int storeCommodityID = 0;
        if (UserDataManager.Instance.shopList != null && UserDataManager.Instance.shopList.data != null)
        {
            List<ShopItemInfo> bkeyarr = UserDataManager.Instance.shopList.data.key_list;
            int len = 0;
            string result = string.Empty;
            if (bkeyarr != null)
            {
                len = bkeyarr.Count;
                for (int i = 0; i < len; i++)
                {
                    ShopItemInfo itemInfo = bkeyarr[i];
                    if (itemInfo != null && itemInfo.product_id == vProductId)
                    {
                        return itemInfo.id;
                    }
                }
            }

            List<ShopItemInfo> diamondarr = UserDataManager.Instance.shopList.data.diamond_list;
            if (diamondarr != null)
            {
                len = diamondarr.Count;
                for (int i = 0; i < len; i++)
                {
                    ShopItemInfo itemInfo = diamondarr[i];
                    if (itemInfo != null && itemInfo.product_id == vProductId)
                    {
                        return itemInfo.id;
                    }
                }
            }

            List<ShopItemInfo> packagearr = UserDataManager.Instance.shopList.data.package_list;
            if (packagearr != null)
            {
                len = packagearr.Count;
                for (int i = 0; i < len; i++)
                {
                    ShopItemInfo itemInfo = packagearr[i];
                    if (itemInfo != null && itemInfo.product_id == vProductId)
                    {
                        return itemInfo.id;
                    }
                }
            }
        }
        return storeCommodityID;
    }

    /// <summary>
    /// 获取商品列表失败
    /// </summary>
    /// <param name="vInfo">V info.</param>
    public void GetProductListFail(string vInfo)
    {
        LOG.Info("===失败===GetProductListFail====>" + vInfo);
    }

    public void PurchseLog(string vInfo)
    {
        LOG.Info("===Purchase LogInfo====>" + vInfo);

    }

    //判断是否过了购买间隔时间
    public bool CheckCanBuy()
    {
        if (this.mPayCallBack != null)//上一个购买未完成
        {
            LOG.Error("上一个购买未完成");
            return false;
        }
        if (GameDataMgr.Instance.GetCurrentUTCTime() - CurServerTime >= _buyIntervalTime)
        {
            CurServerTime = GameDataMgr.Instance.GetCurrentUTCTime();
            return true;
        }
        LOG.Error("上一个购买间隔时间太短");
        return false;
    }

    //存储购买商品的token 到本地
    public void DoSaveTokenInClient(string vOrderId, HasBuyProductItem vHasBuyItem)
    {
        //if(vHasBuyItem != null)
        //{
        //    HasBuySuccProductDic.Add(vOrderId, vHasBuyItem);
        //    //GameSettingPurchase.Instance.SaveHasBuyProductItem(vHasBuyItem);
        //}
    }

    //校验完成移除本地 token
    public void DoRemoveTokenInClient(string vOrderId)
    {
        //if(HasBuySuccProductDic != null)
        //{
        //    HasBuyProductItem hasBuyItem = null;
        //    if (HasBuySuccProductDic.TryGetValue(vOrderId, out hasBuyItem))
        //    {
        //        LOG.Info("--remove token in client -- orderId -->" + vOrderId + " productId:" + hasBuyItem.productId);
        //        HasBuySuccProductDic.Remove(vOrderId);
        //    }
        //}
    }

    /// <summary>
    /// 获取自增的id
    /// </summary>
    /// <returns></returns>
    private int GetMyOrderId()
    {
        return _myOrderId++;
    }

#region 支付
    /// <summary>
    /// IOS购买商品成功的回调
    /// </summary>
    /// <param name="vInfo"></param>
    public void BuyProductSucc(string vInfo)
    {
        LOG.Info("=== 购买成功 ===ReturnInfo====>" + vInfo);
        string[] infoList = vInfo.Split('#');
        if (infoList.Length > 1)
        {
            HasBuyProductItem hasBuyItem = new HasBuyProductItem();

            if (string.IsNullOrEmpty(mOrderId))
            {
                mOrderId = PlayerPrefs.GetString(infoList[0]);
                if (string.IsNullOrEmpty(mOrderId))
                {
                    Debug.LogError("订单号丢失:" + infoList[0]);
                }
            }

            hasBuyItem.myOrderId = mOrderId;
            hasBuyItem.productId = infoList[0];
            hasBuyItem.token = infoList[1];
            hasBuyItem.transactionId = infoList[2];
            PlatformProductItem productItem = null;
            if (PlatformProductDic != null)
            {
                if (PlatformProductDic.TryGetValue(hasBuyItem.productId, out productItem))
                {
                    hasBuyItem.currency = productItem.currency;
                    hasBuyItem.money = productItem.price;
                    hasBuyItem.storeCommodityId = GetStoreCommodityByProductId(hasBuyItem.productId);
                }
            }
            //DoSaveTokenInClient(hasBuyItem.myOrderId, hasBuyItem);
            //记录当前发送给服务端的orderId；
            BuySuccSendServerReturn = false;
            if (!string.IsNullOrEmpty(hasBuyItem.myOrderId) && !hasBuyItem.myOrderId.Equals("RequestList"))
            {
                OnSdkPayCallback(true, hasBuyItem);
            }
            else
            {
                //恢复订单，如果在这个接口返回，则也通知服务端，进行补单
                PurchaseRecordManager.Instance.DoTransactionRestored(hasBuyItem);
            }
        }
    }

    /// <summary>
    /// 购买商品失败
    /// </summary>
    /// <param name="vInfo"></param>
    public void BuyProductFail(string vInfo)
    {
        LOG.Info("===失败===FailInfo====>" + vInfo);
        HasBuyProductItem hasBuyItem = new HasBuyProductItem();
        hasBuyItem.myOrderId = mOrderId;
        hasBuyItem.msg = vInfo;
        OnSdkPayCallback(false, hasBuyItem);

        //FSMService.Instance.UpdateFSMTopUI((int)GameDefine.UIUpdateType.PURCHASE_CURRENCY_FAIL);
    }


    void PayComplete(bool isOK,string result)
    {
        if(this.mPayCallBack == null)
        {
            return;
        }
        var tmp = this.mPayCallBack;
        this.mPayCallBack = null;
        tmp(isOK, result);
    }


    private void OrderCancelCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----OrderCancelCallBack---->" + result);
    }



    void OnSdkPayCallback(bool isOK, HasBuyProductItem vProductItem)
    {
        //UINetLoadingMgr.Instance.Close();
        if (vProductItem != null)
            Debug.Log("----vProduceId:---->" + vProductItem.productId + "----token--->" + vProductItem.token);
        if (!isOK)
        {
            TalkingDataManager.Instance.DoCharge(EventEnum.PlatformPayCancel, vProductItem.myOrderId);
            //TalkingDataManager.Instance.PlayerCancelCharge(vProductItem.myOrderId);
            GameHttpNet.Instance.UserOrderCancel(vProductItem.myOrderId, vProductItem.msg, 3, OrderCancelCallBack);

            var Localization = GameDataMgr.Instance.table.GetLocalizationById(148);
            UITipsMgr.Instance.PopupTips(Localization, false);

            //UITipsMgr.Instance.PopupTips("Payment failed!", false);
            PayComplete(isOK, vProductItem.ToString());
            return;
        }

        LOG.Info("---ios订单支付成功-->" + " orderId:" + vProductItem.myOrderId + " productId:" + vProductItem.productId);

        PurchaseRecordManager.Instance.AddPurchaseRecord(vProductItem.myOrderId, vProductItem.transactionId, vProductItem.token, vProductItem.productId, SdkMgr.IosAppId.ToString(), "", "", "", 2);


#if ENABLE_DEBUG
        if (!SdkMgr.Instance._Test_PayOn)
        {
            UIAlertMgr.Instance.Show("[Test]支付失败", "请重启游戏补单^_^", AlertType.Sure);
            PayComplete(false, "支付成功，测试补单");
            return;
        }
#endif
        //UINetLoadingMgr.Instance.Show();

        TalkingDataManager.Instance.DoCharge(EventEnum.PlatformPayResultSucc, vProductItem.myOrderId);
        TalkingDataManager.Instance.DoCharge(EventEnum.SubmitOrderStart, vProductItem.myOrderId);

        AudioManager.Instance.PlayTones(AudioTones.RewardWin);
        GameHttpNet.Instance.GetOrderToSubmitForIos(vProductItem.myOrderId, vProductItem.token, vProductItem.productId, vProductItem.transactionId, 0, GetOrderToSubmitCallBack);

    }

    private void GetOrderToSubmitCallBack(string orderID, string result)
    {
        //UINetLoadingMgr.Instance.Close();
        LOG.Info("----GetOrderToSubmitCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        bool isOK = false;
        if (jo != null)
        {
            UserDataManager.Instance.orderFormSubmitResultInfo = JsonHelper.JsonToObject<HttpInfoReturn<OrderFormSubmitResultInfo>>(result);
            switch (jo.code)
            {
                case 200:
                    {
                        UserDataManager.Instance.SetPayUser();
                        //AudioManager.Instance.PlayTones(AudioTones.RewardWin);
                        if (UserDataManager.Instance.orderFormSubmitResultInfo != null && UserDataManager.Instance.orderFormSubmitResultInfo.data != null)
                        {
                            UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.orderFormSubmitResultInfo.data.bkey);
                            UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.orderFormSubmitResultInfo.data.diamond);
                            UserDataManager.Instance.ResetMoney(3, UserDataManager.Instance.orderFormSubmitResultInfo.data.ticket);

                            //UITipsMgr.Instance.PopupTips("Payment Successful!", false);

                            //string vOrderId = UserDataManager.Instance.orderFormSubmitResultInfo.data.google_orderid;
                            PurchaseRecordManager.Instance.SendRecordToAppsFlyer(orderID);
                            //TalkingDataManager.Instance.OnChargeSuccess(orderID);

                            TalkingDataManager.Instance.DoCharge(EventEnum.SubmitOrderResultSucc, orderID);
                        }

                        if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data != null)
                        {
                            //UserDataManager.Instance.userInfo.data.userinfo.newpackage_status = 2;
                        }
                        isOK = true;
                    }
                    break;
                case 201://订单异常
                    {
                        TalkingDataManager.Instance.DoCharge(EventEnum.SubmitOrderResultFail, orderID);
                        //AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                        LOG.Info("---订单已支付，无需重复支付-->");
                        //UITipsMgr.Instance.PopupTips("Payment completed, no need to pay again.", false);
                    }
                    break;
                case 202: //订单已取消
                    {
                        //AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                        LOG.Info("---充值失败-->");
                        //UITipsMgr.Instance.PopupTips("Payment Failed!", false);
                    }
                    break;
                case 277:
                case 211:
                    UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, jo.msg);
                    //return;
                    break;
                default:
                    UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, jo.msg);
                    break;
            }
        }



        if (isOK)
        {
            LOG.Error("移除的订单:" + orderID);
            PurchaseRecordManager.Instance.RemovePurchaseRecord(orderID);
            PurchaseRecordManager.Instance.CheckLeftRecord();
        }
        this.PayComplete(isOK, result);
    }

#endregion
}

/// <summary>
/// 平台单项商品信息
/// 物品ID 、 价格
/// </summary>
public class PlatformProductItem
{
    public string productID;    //商品id
    public string currency;      //货币单位
    public string price;        //物品价格
    public string unitPrice;    //物品单价（带货币的）
}

/// <summary>
/// 已经购买回来的产品信息
/// </summary>
public class HasBuyProductItem
{
    public string myOrderId;           //客户端自己管理的，自增id，对应购买回来的成功的商品token
    public int storeCommodityId;    //配置里对应的商品id
    public string productId = string.Empty;        //对应的商场的商品productId
    public string currency = string.Empty;         //货币单位
    public string money = string.Empty;            //价格
    public string token = string.Empty;            //购买成功app返回回来的token
    public string developPayload = string.Empty;   //
    public string googleOrderId = string.Empty;       //谷歌订单id,exampleSku
    public long purchaseTime;       //购买成功后,Google向客户端返回的purchaseTime
    public long userId;             //用户的uid
    public string transactionId;    //苹果付费订单id
    public string msg;              //付款信息

    public override string ToString()
    {
        return myOrderId+","+ storeCommodityId+","+ productId+","+ msg ;
    }
}
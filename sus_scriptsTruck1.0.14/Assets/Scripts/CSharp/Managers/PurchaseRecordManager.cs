using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using System;

[XLua.Hotfix, XLua.LuaCallCSharp, XLua.CSharpCallLua]
public class PurchaseRecordManager : CSingleton<PurchaseRecordManager>
{

    private string SecretsPurchaseFlag = "SrcretsPurchaseRecordInfo";
    private string SecretsRestoredFlag = "SecretsRestoredRecordInfo";

    private Dictionary<string, string> mPurchaseRecordDic ; //正常交易流程，记录在本地的交易记录
    private Dictionary<string, string> mRestoredRecoredDic; //恢复订单，记录在本地的交易记录


    protected override void Init()
    {
        base.Init();
        mPurchaseRecordDic = new Dictionary<string, string>();
        mRestoredRecoredDic = new Dictionary<string, string>();
        //CheckRecordByLocal();
    }


    private void SavePurchaseRecordDic()
    {
        var json = JsonHelper.ObjectToJson(mPurchaseRecordDic);
        PlayerPrefs.SetString(SecretsPurchaseFlag, json);
        PlayerPrefs.Save();
        LOG.Error("[+]SavePurchaseRecordDic:" + json);
    }

    private void SaveRestoredRecoredDic()
    {
        var json = JsonHelper.ObjectToJson(mRestoredRecoredDic);
        PlayerPrefs.SetString(SecretsRestoredFlag,json);
        PlayerPrefs.Save();
        LOG.Error("[+]SaveRestoredRecoredDic:" + json);
    }

    /// <summary>
    /// 检查本地充值记录(call by lua)
    /// </summary>
    public void CheckRecordByLocal()
    {
        string result = PlayerPrefs.GetString(SecretsPurchaseFlag);
        if(!string.IsNullOrEmpty(result))
        {
            LOG.Error(SecretsPurchaseFlag + ":" + result);
            var dict = JsonHelper.JsonToObject<Dictionary<string, string>>(result);
            if (dict != null)
            {
                mPurchaseRecordDic = dict;
            }
        }

        result = PlayerPrefs.GetString(SecretsRestoredFlag);
        if (!string.IsNullOrEmpty(result))
        {
            LOG.Error(SecretsRestoredFlag + ":" + result);
            var dict = JsonHelper.JsonToObject<Dictionary<string, string>>(result);
            if(dict != null)
            {
                mRestoredRecoredDic = dict;
            }
        }

        CheckLeftRecord();
    }


    public void CheckLeftRecord()
    {
        if (mPurchaseRecordDic.Count > 0)   //正常交易流程的订单
        {
            //LOG.Error("----CheckLeftRecord---->" + mPurchaseRecordDic.Count);
            foreach (var item in mPurchaseRecordDic)
            {
                if(!string.IsNullOrEmpty(item.Value))
                {
                    SendRecordToServer(item.Value);
                    break;
                }
            }
        }
        else if(mRestoredRecoredDic.Count > 0) // 平台恢复订单
        {
            foreach (var item in mRestoredRecoredDic)
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    string[] listStr = item.Value.Split(',');
                    if (listStr != null && listStr.Length > 1)
                    {
                        SendRestoredToServer(listStr[0], listStr[1]);
                        break;
                    }
                }
            }
        }
    }

    private void SendRecordToServer(string vRecord)
    {
        string[] listStr = vRecord.Split(',');
        if(listStr != null && listStr.Length > 9)
        {
#if UNITY_IOS
            GameHttpNet.Instance.GetOrderToSubmitForIos(listStr[0], listStr[2], listStr[3], listStr[1], 0, GetOrderToSubmitCallBack);
#else
            GameHttpNet.Instance.GetOrderToSubmitForAndroid(listStr[0], listStr[1], listStr[2], listStr[3], listStr[4], listStr[5], listStr[6], listStr[7],"", GetOrderToSubmitCallBack);
#endif
        }
    }

    public void GetOrderToSubmitCallBack(string orderID,string result)
    {
        LOG.Error("补单结果:" + result);
        //LOG.Info("----GetOrderToSubmitCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            //UINetLoadingMgr.Instance.Close();
            switch (jo.code)
            {
                case 200:
                    {

                        AudioManager.Instance.PlayTones(AudioTones.RewardWin);
                        UserDataManager.Instance.orderFormSubmitResultInfo = JsonHelper.JsonToObject<HttpInfoReturn<OrderFormSubmitResultInfo>>(result);
                        if (UserDataManager.Instance.orderFormSubmitResultInfo != null && UserDataManager.Instance.orderFormSubmitResultInfo.data != null)
                        {
                            UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.orderFormSubmitResultInfo.data.bkey);
                            UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.orderFormSubmitResultInfo.data.diamond);
                            UserDataManager.Instance.ResetMoney(3, UserDataManager.Instance.orderFormSubmitResultInfo.data.ticket);

                            //UITipsMgr.Instance.PopupTips("Payment Successful!", false);

                            //string vOrderId = UserDataManager.Instance.orderFormSubmitResultInfo.data.google_orderid;
                            SendRecordToAppsFlyer(orderID);
                            //TalkingDataManager.Instance.OnChargeSuccess(orderID);
                        }

                        if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data != null)
                        {
                            //UserDataManager.Instance.userInfo.data.userinfo.newpackage_status = 2;
                        }
                    }
                    break;
                case 202://订单已支付，无需重复支付
                    {
                        //AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                        LOG.Info("---订单已支付，无需重复支付-->");
                        //UITipsMgr.Instance.PopupTips("Payment completed, no need to pay again.", false);
                    }
                    break;
                case 208: //充值失败
                    {
                        //AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                        LOG.Info("---充值失败-->");
                        //UITipsMgr.Instance.PopupTips("Payment Failed!", false);
                    }
                    break;
                case 277:
                case 211:
                    UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, jo.msg);
                    return;
                default:
                    UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, jo.msg);
                    break;
            }

            LOG.Error("移除的订单:" + orderID);
            PurchaseRecordManager.Instance.RemovePurchaseRecord(orderID);
            CheckLeftRecord();
        }
    }

    public void SendRecordToAppsFlyer(string vOrderId)
    {
#if !ENABLE_DEBUG
        if (mPurchaseRecordDic.ContainsKey(vOrderId))
        {
            string[] listStr = mPurchaseRecordDic[vOrderId].Split(',');
            if (listStr != null && listStr.Length > 9)
            {
#if CHANNEL_ONYX || CHANNEL_SPAIN
                SdkMgr.Instance.facebook.LogPurchase(float.Parse(listStr[8]));
              //  SdkMgr.Instance.appsFlyer.Purchase(float.Parse(listStr[8]), listStr[3]);
#endif
            }
        }
#endif
    }

    /// <summary>
    /// 保存付费记录
    /// </summary>
    /// <param name="recharge_no"></param>
    /// <param name="vOrderId"></param>
    /// <param name="vOrderToken"></param>
    /// <param name="vProductid"></param>
    /// <param name="vPackagename"></param>
    /// <param name="vDatasignature"></param>
    /// <param name="vPurchasetime"></param>
    /// <param name="vPurchaseState"></param>
    /// <param name="vPaymentType">付费的形式 1：安卓  2:ios  </param>
    public void AddPurchaseRecord(string recharge_no, string vOrderId, string vOrderToken, string vProductid, string vPackagename, string vDatasignature, string vPurchasetime, string vPurchaseState,int vPaymentType = 1)
    {
        string vPrice = PurchaseManager.Instance.CheckPriceByProduct(vProductid);
        string result = recharge_no + "," + vOrderId + "," + vOrderToken + "," + vProductid + "," + vPackagename + "," + vDatasignature + "," + vPurchasetime + "," + vPurchaseState + "," + vPrice + "," + vPaymentType;
        if(!mPurchaseRecordDic.ContainsKey(vOrderId))
        {
            mPurchaseRecordDic.Add(vOrderId, result);
            PlayerPrefs.SetString(vOrderId, result);
            this.SavePurchaseRecordDic();
        }
    }

    /// <summary>
    /// 移除付费记录
    /// </summary>
    /// <param name="vKey"></param>
    public void RemovePurchaseRecord(string vKey)
    {
        if(string.IsNullOrEmpty(vKey)){
            return;
        }
        if(mPurchaseRecordDic.ContainsKey(vKey))
        {
            mPurchaseRecordDic.Remove(vKey);
            this.SavePurchaseRecordDic();
        }

        PlayerPrefs.SetString(vKey, string.Empty);
    }

    //记录恢复订单信息到本地，并发送给服务器
    public void DoTransactionRestored(HasBuyProductItem vProductionItem)
    {
        if (vProductionItem == null) return;
        string vTransactionId = vProductionItem.transactionId;
        string vSignatureToken = vProductionItem.token;
        LOG.Error("[*]DoTransactionRestored:" + vTransactionId + "," + vSignatureToken);
        if (!string.IsNullOrEmpty(vSignatureToken))
        {
            mRestoredRecoredDic[vTransactionId] = vSignatureToken;
        }
        SaveRestoredRecoredDic();

        SendRestoredToServer(vTransactionId, vSignatureToken);
    }


    //发送恢复订单给服务器
    private void SendRestoredToServer(string vTransactionId, string vSignatureToken)
    {
        //UINetLoadingMgr.Instance.Show();
        TalkingDataManager.Instance.DoCharge(EventEnum.SubmitRecoverStart, vTransactionId);
        GameHttpNet.Instance.Recoverorder(vTransactionId, vSignatureToken,(arg)=>
        {
            string result = arg.ToString();
            LOG.Info("----SendRestoredToServer---->" + result);
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo != null)
            {
                //UINetLoadingMgr.Instance.Close();
                bool needRemove = false;
                switch (jo.code)
                {
                    case 200:
                        needRemove = true;
                        var data = JsonHelper.JsonToObject<HttpInfoReturn<OrderFormSubmitResultInfo>>(result);
                        if(data != null)
                        {
                            UserDataManager.Instance.orderFormSubmitResultInfo = data;
                            UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.orderFormSubmitResultInfo.data.bkey);
                            UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.orderFormSubmitResultInfo.data.diamond);
                            UserDataManager.Instance.ResetMoney(3, UserDataManager.Instance.orderFormSubmitResultInfo.data.ticket);

                            TalkingDataManager.Instance.DoCharge(EventEnum.SubmitRecoverResultSucc, vTransactionId);
                        }
                        break;
                    case 201://异常
                        UIAlertMgr.Instance.Show("Error", jo.msg, AlertType.Sure, (b) =>
                        {
                            //SendRestoredToServer();
                        });
                        break;
                    case 202://异常次数达到上限
                        TalkingDataManager.Instance.DoCharge(EventEnum.SubmitRecoverResultFail, vTransactionId);
                        needRemove = true;
                        UIAlertMgr.Instance.Show("Error", jo.msg, AlertType.Sure, (b) =>
                         {
                             //SendRestoredToServer();
                         });
                        break;
                    default:
                        UIAlertMgr.Instance.Show("TIPS", jo.msg, AlertType.Sure);
                        break;
                }

                if (needRemove && mRestoredRecoredDic.ContainsKey(vTransactionId))
                {
                    mRestoredRecoredDic.Remove(vTransactionId);
                    SaveRestoredRecoredDic();
                }

            }
        });
    }

    /// <summary>
    /// 订单号是否在为完成记录中
    /// </summary>
    public bool HashUnCompleteOrder(string orderId)
    {
        foreach(var itr in mPurchaseRecordDic)
        {
            if (itr.Value.Contains(orderId))
            {
                CheckLeftRecord();
                return true;
            }
        }

        foreach(var itr in mRestoredRecoredDic)
        {
            if (itr.Value.Contains(orderId))
            {
                CheckLeftRecord();
                return true;
            }
        }
        return false;
    }

}

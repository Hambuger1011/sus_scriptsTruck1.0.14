﻿using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Purchase
{
    string mItemType;  // ITEM_TYPE_INAPP or ITEM_TYPE_SUBS
    string mOrderId;
    string mPackageName;
    string mSku;
    long mPurchaseTime;
    int mPurchaseState;
    string mDeveloperPayload;
    string mToken;
    string mOriginalJson;
    string mSignature;
    bool mIsAutoRenewing;

    public Purchase(string jsonPurchaseInfo)
    {
        mOriginalJson = jsonPurchaseInfo;
        var o = JsonMapper.ToObject(mOriginalJson);
        mOrderId = o.optString("orderId");
        mPackageName = o.optString("packageName");
        mSku = o.optString("productId");
        mPurchaseTime = o.optLong("purchaseTime");
        mPurchaseState = o.optInt("purchaseState");
        mDeveloperPayload = o.optString("developerPayload");
        mToken = o.optString("token", o.optString("purchaseToken"));
        mIsAutoRenewing = o.optBoolean("autoRenewing");

        mSignature = o.optString("signature");
        mItemType = o.optString("itemType");
    }

    public string getItemType() { return mItemType; }
    public string getOrderId() { return mOrderId; }
    public string getPackageName() { return mPackageName; }
    public string getSku() { return mSku; }
    public long getPurchaseTime() { return mPurchaseTime; }
    public int getPurchaseState() { return mPurchaseState; }
    public string getDeveloperPayload() { return mDeveloperPayload; }
    public string getToken() { return mToken; }
    public string getOriginalJson() { return mOriginalJson; }
    public string getSignature() { return mSignature; }
    public bool isAutoRenewing() { return mIsAutoRenewing; }

    public override string ToString() { return "PurchaseInfo(type:" + mItemType + "):" + mOriginalJson; }
}




public class IabResult
{
    public int response;
    public string message;
    public string other;


    // Billing response codes
    public const int BILLING_RESPONSE_RESULT_OK = 0;
    public const int BILLING_RESPONSE_RESULT_USER_CANCELED = 1;
    public const int BILLING_RESPONSE_RESULT_SERVICE_UNAVAILABLE = 2;
    public const int BILLING_RESPONSE_RESULT_BILLING_UNAVAILABLE = 3;
    public const int BILLING_RESPONSE_RESULT_ITEM_UNAVAILABLE = 4;
    public const int BILLING_RESPONSE_RESULT_DEVELOPER_ERROR = 5;
    public const int BILLING_RESPONSE_RESULT_ERROR = 6;
    public const int BILLING_RESPONSE_RESULT_ITEM_ALREADY_OWNED = 7;
    public const int BILLING_RESPONSE_RESULT_ITEM_NOT_OWNED = 8;

    // IAB Helper error codes

    public const int IABHELPER_ERROR_BASE = -1000;
    public const int IABHELPER_REMOTE_EXCEPTION = -1001;
    public const int IABHELPER_BAD_RESPONSE = -1002;
    public const int IABHELPER_VERIFICATION_FAILED = -1003;
    public const int IABHELPER_SEND_INTENT_FAILED = -1004;
    public const int IABHELPER_USER_CANCELLED = -1005;
    public const int IABHELPER_UNKNOWN_PURCHASE_RESPONSE = -1006;
    public const int IABHELPER_MISSING_TOKEN = -1007;
    public const int IABHELPER_UNKNOWN_ERROR = -1008;
    public const int IABHELPER_SUBSCRIPTIONS_NOT_AVAILABLE = -1009;
    public const int IABHELPER_INVALID_CONSUMPTION = -1010;
    public const int IABHELPER_SUBSCRIPTION_UPDATE_NOT_AVAILABLE = -1011;

    public static string getResponseDesc(int code)
    {
        string[] iab_msgs = ("0:OK/1:User Canceled/2:Unknown/" +
                "3:Billing Unavailable/4:Item unavailable/" +
                "5:Developer Error/6:Error/7:Item Already Owned/" +
                "8:Item not owned").Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
        string[] iabhelper_msgs = ("0:OK/-1001:Remote exception during initialization/" +
                                   "-1002:Bad response received/" +
                                   "-1003:Purchase signature verification failed/" +
                                   "-1004:Send intent failed/" +
                                   "-1005:User cancelled/" +
                                   "-1006:Unknown purchase response/" +
                                   "-1007:Missing token/" +
                                   "-1008:Unknown error/" +
                                   "-1009:Subscriptions not available/" +
                                   "-1010:Invalid consumption attempt").Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

        if (code <= IABHELPER_ERROR_BASE)
        {
            int index = IABHELPER_ERROR_BASE - code;
            if (index >= 0 && index < iabhelper_msgs.Length) return iabhelper_msgs[index];
            else return code + ":Unknown IAB Helper Error";
        }
        else if (code < 0 || code >= iab_msgs.Length)
            return code + ":Unknown";
        else
            return iab_msgs[code];
    }
}

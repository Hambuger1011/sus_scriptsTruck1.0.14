using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;

public class PurchaseHistoryRecord
{
    private string mOriginalJson;
    private string mSignature;
    private JsonData mParsedJson;

    public PurchaseHistoryRecord(string jsonPurchaseInfo)
    {
        mOriginalJson = jsonPurchaseInfo;
        mParsedJson = JsonMapper.ToObject(mOriginalJson);
        mSignature = (string)mParsedJson["signature"];
    }

    /** Returns the product Id. */
    public string getSku()
    {
        return (string)mParsedJson["productId"];
    }

    /** Returns the time the product was purchased, in milliseconds since the epoch (Jan 1, 1970). */
    public long getPurchaseTime()
    {
        return long.Parse(mParsedJson["purchaseTime"].ToString());
    }

    /** Returns a token that uniquely identifies a purchase for a given item and user pair. */
    public string getPurchaseToken()
    {
        if (mParsedJson.Contains("token"))
        {
            return (string)mParsedJson["token"];
        }
        return (string)mParsedJson["purchaseToken"];
    }


    public string getDeveloperPayload()
    {
        return (string)mParsedJson["developerPayload"];
    }

    /** Returns a string in JSON format that contains details about the purchase order. */
    public string getOriginalJson()
    {
        return mOriginalJson;
    }

    /**
     * Returns string containing the signature of the purchase data that was signed with the private
     * key of the developer. The data signature uses the RSASSA-PKCS1-v1_5 scheme.
     */
    public string getSignature()
    {
        return mSignature;
    }


    public string toString()
    {
        return "PurchaseHistoryRecord. Json: " + mOriginalJson;
    }


}

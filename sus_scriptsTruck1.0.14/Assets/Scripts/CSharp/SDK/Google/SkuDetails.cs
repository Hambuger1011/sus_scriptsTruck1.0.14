using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class SkuDetails
{
    private string mItemType;
    private string mSku;
    private string mType;
    private string mPrice;
    private long mPriceAmountMicros;
    private string mPriceCurrencyCode;
    private string mTitle;
    private string mDescription;
    private string mJson;

    public SkuDetails(string jsonSkuDetails)
    {
        mJson = jsonSkuDetails;
        var o = JsonMapper.ToObject(mJson);
        mSku = o.optString("productId");
        mType = o.optString("type");
        mPrice = o.optString("price");
        mPriceAmountMicros = o.optLong("price_amount_micros");
        mPriceCurrencyCode = o.optString("price_currency_code");
        mTitle = o.optString("title");
        mDescription = o.optString("description");
    }

    public string getSku() { return mSku; }
    public string getType() { return mType; }
    public string getPrice() { return mPrice; }
    public long getPriceAmountMicros() { return mPriceAmountMicros; }
    public string getPriceCurrencyCode() { return mPriceCurrencyCode; }
    public string getTitle() { return mTitle; }
    public string getDescription() { return mDescription; }

    public override string ToString()
    {
        return "SkuDetails:" + mJson;
    }

    public string getOriginalJson() { return mJson; }
}


static class JavaJsonAdapter
{
    public static string optString(this JsonData jsonData, string key,string defaultValue = "")
    {
        if (!jsonData.Contains(key))
        {
            return defaultValue;
        }
        var o = jsonData[key];
        if (o.IsString)
        {
            return (string)o;
        }
        return o.ToString();
    }
    public static long optLong(this JsonData jsonData, string key)
    {
        if (!jsonData.Contains(key))
        {
            return 0;
        }
        var o = jsonData[key];
        if (o.IsLong)
        {
            return (long)o;
        }
        return long.Parse(o.ToString());
    }
    public static int optInt(this JsonData jsonData, string key)
    {
        if (!jsonData.Contains(key))
        {
            return 0;
        }
        var o = jsonData[key];
        if (o.IsInt)
        {
            return (int)o;
        }
        return int.Parse(o.ToString());
    }

    public static bool optBoolean(this JsonData jsonData, string key)
    {
        if (!jsonData.Contains(key))
        {
            return false;
        }
        var o = jsonData[key];
        if (o.IsBoolean)
        {
            return (bool)o;
        }
        return false;
    }
}
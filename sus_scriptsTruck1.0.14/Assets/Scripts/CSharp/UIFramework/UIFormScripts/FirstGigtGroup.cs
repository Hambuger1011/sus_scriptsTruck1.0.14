using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UGUI;
using pb;

public class FirstGigtGroup : BaseUIForm {

    public Text KeyNumText, PriceText,oldPriceText;
    public GameObject BuyButton, CloseButton,mask;
    private int keyNum, diamondNum;

    private package vpackage;
    private int Type = 0;//1，在游戏中打开 2，在主界面中打开
    public override void OnOpen()
    {
        base.OnOpen();

        UIEventListener.AddOnClickListener(BuyButton, BuyButtonOnclick);
        UIEventListener.AddOnClickListener(CloseButton,CloseButtonOnclick);
        UIEventListener.AddOnClickListener(mask, maskClose);

        this.vpackage = null;
        //获取新手礼包信息
        GameHttpNet.Instance.Getuserpackage(1, 1, GetuserpackageCallBack);

        Transform bgTrans = this.gameObject.transform.Find("Frame/FirstGigtGroup");
        if (GameUtility.IpadAspectRatio() && bgTrans != null)
            bgTrans.localScale = Vector3.one * 0.7f;
    }

    public void GetType(int type)
    {
        Type = type;
    }
    public override void OnClose()
    {
        base.OnClose();

        UIEventListener.RemoveOnClickListener(BuyButton, BuyButtonOnclick);
        UIEventListener.RemoveOnClickListener(CloseButton, CloseButtonOnclick);
        UIEventListener.RemoveOnClickListener(mask, maskClose);
    }

    private void maskClose(PointerEventData data)
    {
        CloseButtonOnclick(null);
    }
    private void Init(package package)
    {
        if (KeyNumText == null)
            return;

        KeyNumText.text = package.bkey_qty.ToString();
        //DiamondNumText.text = package.diamond_qty.ToString();

        keyNum = package.bkey_qty;
        diamondNum= package.diamond_qty;

        KeyNumText.text = keyNum + " Key + "+ diamondNum + " Diamonds";

        PriceText.text ="$ "+package.price.ToString();
        oldPriceText.text = "$ " + package.old_price.ToString();

    }
    public void CloseButtonOnclick(PointerEventData data)
    {
        CUIManager.Instance.CloseForm(UIFormName.FirstGigtGroup);
    }
    private void GetuserpackageCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetuserpackageCallBack---->" + result);
        if (result.Equals("error"))
        {
            LOG.Info("----GetuserpackageCallBack----协议返回错误");
            return;
        }
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.Getuserpackage = JsonHelper.JsonToObject<HttpInfoReturn<Getuserpackage>>(result);

                    //这个是从游戏中返回书架
                    if (UserDataManager.Instance.Getuserpackage != null)
                    {
                        if (UserDataManager.Instance.Getuserpackage.data.package.Count == 0)
                        {
                            LOG.Info("新手礼包里面没有数据，关闭新手界面");
                            CloseButtonOnclick(null);
                            return;
                        }

                        List<package> Package = UserDataManager.Instance.Getuserpackage.data.package;
                        if (Package.Count<=0)
                        {
                            return;
                        }
                        vpackage = Package[0];
                        Init(vpackage);
                    }
                }
            }, null);
        }
    }
    public void BuyButtonOnclick(PointerEventData data)
    {
        if (UserDataManager.Instance.Getuserpackage != null)
        {
            //点击充值
            //UINetLoadingMgr.Instance.Show();
            //TalkingDataManager.Instance.GameCharge(vpackage.productName);

            SdkMgr.Instance.Pay(vpackage.id, vpackage.productName, 3, vpackage.price, OnPayCallback);
        }
    }


    void OnPayCallback(bool isOK, string result)
    {
        if (!isOK)
        {
            return;
        }

        AudioManager.Instance.PlayTones(AudioTones.RewardWin);
        if (UserDataManager.Instance.orderFormSubmitResultInfo != null && UserDataManager.Instance.orderFormSubmitResultInfo.data != null)
        {
            UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.orderFormSubmitResultInfo.data.bkey);
            UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.orderFormSubmitResultInfo.data.diamond);
            UserDataManager.Instance.ResetMoney(3, UserDataManager.Instance.orderFormSubmitResultInfo.data.ticket);
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(152);
            UITipsMgr.Instance.PopupTips(Localization, false);

            //UITipsMgr.Instance.PopupTips("Payment Successful!", false);

            MyBooksDisINSTANCE.Instance.FirstGigtHadBuyF();

            CloseButtonOnclick(null);
        }

        //给新手礼包发消息，记录新手礼包已经购买过了
        EventDispatcher.Dispatch(EventEnum.RecordPremiumGiftBagBuy, null);
        
    }
}

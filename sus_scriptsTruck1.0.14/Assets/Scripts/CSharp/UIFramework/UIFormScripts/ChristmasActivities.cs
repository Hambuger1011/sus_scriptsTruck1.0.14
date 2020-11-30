using pb;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChristmasActivities : BaseUIForm
{
    public GameObject UIMask,BuyButton;
    public Text DiamondsText, KeyText, TicketsText,PriceText;

    private float Price;
    private package mItemInfo;

    public override void OnOpen()
    {
        base.OnOpen();
        UIEventListener.AddOnClickListener(UIMask, CloseUi);
        UIEventListener.AddOnClickListener(BuyButton, BuyButtonOnclicke);
        Init();
    }

    private void Init()
    {
        if (UserDataManager.Instance.Getuserpackage != null)
        {
            List<package> tem = UserDataManager.Instance.Getuserpackage.data.package;
            mItemInfo = tem[0];
            DiamondsText.text = tem[0].diamond_qty.ToString()+" Diamonds";
            KeyText.text = tem[0].bkey_qty.ToString() + " Key";
            TicketsText.text = tem[0].tickey_qty.ToString() + " Tickets";
            PriceText.text = tem[0].price.ToString() + "$";
            Price = float.Parse(mItemInfo.price);
        }
        else
        {
            CloseUi(null);
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(UIMask, CloseUi);
        UIEventListener.RemoveOnClickListener(BuyButton, BuyButtonOnclicke);
    }

    private void BuyButtonOnclicke(PointerEventData data)
    {
       // LOG.Info("圣诞大礼包购买按钮被点击了");
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        if (mItemInfo != null)
        {
            //LOG.Info("购买：" + mItemInfo.id + "__价格是：" + mItemInfo.price);
            if (mItemInfo.pricetype == 0)    //购买
            {
                //点击充值
                //UINetLoadingMgr.Instance.Show();
                //TalkingDataManager.Instance.GameCharge(mItemInfo.productName);

                SdkMgr.Instance.Pay(mItemInfo.id, mItemInfo.productName, 3, mItemInfo.price, OnPayCallback);
            }          
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
            
            CloseUi(null);//关闭界面
            
        }
    }
    

    private void getbookgiftbagBackCall(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----getbookgiftbagBackCall---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.GetBookGiftBag = JsonHelper.JsonToObject<HttpInfoReturn<GetBookGiftBag>>(result);

                    //这个是从游戏中返回书架
                    if (UserDataManager.Instance.GetBookGiftBag != null)
                    {
                        if (UserDataManager.Instance.GetBookGiftBag.data.giftlist.Count == 0)
                        {                        
                            LOG.Info("没有书本礼包列表");
                            return;
                        }

                        //打开书本活动界面
                        CUIManager.Instance.OpenForm(UIFormName.BookActivities);
                    }
                }
            }, null);
        }
    }

    private void CloseUi(PointerEventData data)
    {
        CUIManager.Instance.CloseForm(UIFormName.ChristmasActivities);

        //获取礼包信息
        GameHttpNet.Instance.Getbookgiftbag(getbookgiftbagBackCall);
    }
}

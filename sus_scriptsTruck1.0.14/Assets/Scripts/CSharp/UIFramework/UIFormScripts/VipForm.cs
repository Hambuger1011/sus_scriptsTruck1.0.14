using pb;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VipForm : BaseUIForm {

    public GameObject CloseButton,BuyButton,Mask;
    public Text GetKeyNum, DayKeyNum,Price;
    

    private int GetKeyNumS, GetDiamNums, DayKeyNums, DayDiamNums;
    private vipinfo vpackage;
    private string productName;
    private string gamePriceName;
    public override void OnOpen()
    {
        base.OnOpen();
        UIEventListener.AddOnClickListener(CloseButton, CloseButtonOnclick);
        UIEventListener.AddOnClickListener(BuyButton,BuyButtonOnclicke);
        UIEventListener.AddOnClickListener(Mask, MaskClose);

        if (UserDataManager.Instance.Getvipcard!=null)
        {
            vpackage = UserDataManager.Instance.Getvipcard.data.vipinfo;
            if (UserDataManager.Instance.Getvipcard.data.is_buy == 0)
            {
                //没有购买过VIP
                Price.text = "$" + UserDataManager.Instance.Getvipcard.data.vipinfo.account;
            }
            else
            {
                Price.text = "Recharge";
            }
            if (UserDataManager.Instance.Getvipcard.data.day<=0)
            {
                Price.text = "$" + UserDataManager.Instance.Getvipcard.data.vipinfo.account;
            }
        
            GetKeyNumS= UserDataManager.Instance.Getvipcard.data.vipinfo.get_bkey_qty;         
            GetDiamNums = UserDataManager.Instance.Getvipcard.data.vipinfo.get_diamond_qty;
            GetKeyNum.text = "Get "+ GetKeyNumS+" key and "+ GetDiamNums+ " diamond immediately";
         
            DayKeyNums = UserDataManager.Instance.Getvipcard.data.vipinfo.day_bkey_qty;          
            DayDiamNums = UserDataManager.Instance.Getvipcard.data.vipinfo.day_diamond_qty;
            DayKeyNum.text = "Get "+ DayKeyNums + " key and "+ DayDiamNums + " diamond per day";
        }
        else
        {

            GameHttpNet.Instance.Getvipcard(VIPCallBacke);
        }

        Transform bgTrans = this.gameObject.transform.Find("Canvas/BG");
        if (GameUtility.IpadAspectRatio() && bgTrans != null)
            bgTrans.localScale = Vector3.one * 0.7f;
    }

    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(CloseButton, CloseButtonOnclick);
        UIEventListener.RemoveOnClickListener(BuyButton, BuyButtonOnclicke);
        UIEventListener.RemoveOnClickListener(Mask, MaskClose);
    }

    private void MaskClose(PointerEventData data)
    {
        CloseButtonOnclick(null);
    }
    private void CloseButtonOnclick(PointerEventData data)
    {
        CUIManager.Instance.CloseForm(UIFormName.VIP);
    }

    private void VIPCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----VIPCallBacke---->" + result);
        if (result.Equals("error"))
        {
            LOG.Info("---VIPCallBacke--这条协议错误");
            return;
        }
        LoomUtil.QueueOnMainThread((param) =>
        {
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo != null)
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.Getvipcard = JsonHelper.JsonToObject<HttpInfoReturn<Getvipcard<vipinfo>>>(arg.ToString());
         
                    vpackage = UserDataManager.Instance.Getvipcard.data.vipinfo;

                    if (Price.gameObject==null)
                    {
                        return;
                    }

                    if (UserDataManager.Instance.Getvipcard.data.is_buy==0)
                    {
                        //没有购买过VIP
                        Price.text = "$" + UserDataManager.Instance.Getvipcard.data.vipinfo.account;
                    }
                    else
                    {
                        Price.text = "Recharge";
                    }

                    if (UserDataManager.Instance.Getvipcard.data.day <= 0)
                    {
                        Price.text = "$" + UserDataManager.Instance.Getvipcard.data.vipinfo.account;
                    }

                    //GetKeyNum.text = UserDataManager.Instance.Getvipcard.data.vipinfo.get_bkey_qty.ToString();
                    //GetDionNum.text= UserDataManager.Instance.Getvipcard.data.vipinfo.get_diamond_qty.ToString();
                    //DayKeyNum.text= UserDataManager.Instance.Getvipcard.data.vipinfo.day_bkey_qty.ToString();
                    //DayDionNum.text= UserDataManager.Instance.Getvipcard.data.vipinfo.day_diamond_qty.ToString(); ;

                    GetKeyNumS = UserDataManager.Instance.Getvipcard.data.vipinfo.get_bkey_qty;
                    GetDiamNums = UserDataManager.Instance.Getvipcard.data.vipinfo.get_diamond_qty;
                    GetKeyNum.text = "Get " + GetKeyNumS + " key and " + GetDiamNums + " diamond immediately";

                    DayKeyNums = UserDataManager.Instance.Getvipcard.data.vipinfo.day_bkey_qty;
                    DayDiamNums = UserDataManager.Instance.Getvipcard.data.vipinfo.day_diamond_qty;
                    DayKeyNum.text = "Get " + DayKeyNums + " key and " + DayDiamNums + " diamond per day";
                }
            }

        }, null);
    }

    #region  购买按钮
    private void BuyButtonOnclicke(PointerEventData data)
    {
        if (UserDataManager.Instance.Getvipcard != null)
        {

            if (UserDataManager.Instance.shopList.data.package_list != null)
            {
                for (int i = 0; i < UserDataManager.Instance.shopList.data.package_list.Count; i++)
                {
                    float Price = float.Parse(UserDataManager.Instance.shopList.data.package_list[i].price);
                    if (Price == float.Parse(UserDataManager.Instance.Getvipcard.data.vipinfo.account))
                    {
                        productName = UserDataManager.Instance.shopList.data.package_list[i].product_id;
                        gamePriceName = UserDataManager.Instance.shopList.data.package_list[i].product_name;
                    }
                }
            }

            //点击充值
            //UINetLoadingMgr.Instance.Show();
            SdkMgr.Instance.Pay(vpackage.mallid, productName, 4, vpackage.account, OnPayCallback);
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
            if (UserDataManager.Instance.userInfo != null)
                UserDataManager.Instance.userInfo.data.userinfo.is_vip = 1;

            UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.orderFormSubmitResultInfo.data.bkey);
            UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.orderFormSubmitResultInfo.data.diamond);
            UserDataManager.Instance.ResetMoney(3, UserDataManager.Instance.orderFormSubmitResultInfo.data.ticket);

            //Vector3 startPos = new Vector3(-188, -355);
            //Vector3 targetPos = new Vector3(174, 720);
            //RewardShowData rewardShowData = new RewardShowData();
            //rewardShowData.StartPos = startPos;
            //rewardShowData.TargetPos = targetPos;
            //rewardShowData.IsInputPos = false;
            //rewardShowData.KeyNum = UserDataManager.Instance.orderFormSubmitResultInfo.data.bkey;
            //rewardShowData.DiamondNum = UserDataManager.Instance.orderFormSubmitResultInfo.data.diamond;
            //rewardShowData.Type = 1;
            //EventDispatcher.Dispatch(EventEnum.GetRewardShow, rewardShowData);

            var Localization = GameDataMgr.Instance.table.GetLocalizationById(152);
            UITipsMgr.Instance.PopupTips(Localization, false);

            UserDataManager.Instance.Getvipcard.data.day += UserDataManager.Instance.Getvipcard.data.vipinfo.day;
            EventDispatcher.Dispatch(EventEnum.VIPDayUp);

            CloseButtonOnclick(null);
        }

    }
    #endregion
}

using pb;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class OfertesLogic
{
    private OfertesCtrl OfertesCtrl;
    private package vpackage;
    private bool hadBuy = false;
    public OfertesLogic(OfertesCtrl OfertesCtrl)
    {
        this.OfertesCtrl = OfertesCtrl;       
        Init();
        GetADSInfo();

        
    }

    public void Init()
    {
        this.vpackage = null;

        //获取新手礼包信息
        GameHttpNet.Instance.Getuserpackage(1, 1, GetuserpackageCallBack);
    }

    public void GetADSInfo()
    {
        //红点协议
        GameHttpNet.Instance.ActiveInfo(GetimpinfoCallBacke);
    }

    public void GetimpinfoCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetimpinfoCallBacke---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo.code == 200)
        {
            UserDataManager.Instance.lotteryDrawInfo = JsonHelper.JsonToObject<HttpInfoReturn<ActiveInfo>>(arg.ToString());

            SaveWeekData();
            OfertesCtrl.OfertesView.OnActiveInfoRefresh();

        }
    }

    private void SaveWeekData()
    {
        OfertesCtrl.OfertesDB.GetADSInfoList().Clear();
        if (UserDataManager.Instance.lotteryDrawInfo!=null)
        {
            OfertesCtrl.OfertesDB.HadReadADSNumber = 0;
            ADSInfo oneInfo1 = new ADSInfo();       
            SaveWeekDataToList(1,oneInfo1, UserDataManager.Instance.lotteryDrawInfo.data.activeadcount.one);
            ADSInfo oneInfo2 = new ADSInfo();
            SaveWeekDataToList(2,oneInfo2, UserDataManager.Instance.lotteryDrawInfo.data.activeadcount.two);
            ADSInfo oneInfo3 = new ADSInfo();
            SaveWeekDataToList(3,oneInfo3, UserDataManager.Instance.lotteryDrawInfo.data.activeadcount.three);
            ADSInfo oneInfo4 = new ADSInfo();
            SaveWeekDataToList(4,oneInfo4, UserDataManager.Instance.lotteryDrawInfo.data.activeadcount.four);
            ADSInfo oneInfo5= new ADSInfo();
            SaveWeekDataToList(5,oneInfo5, UserDataManager.Instance.lotteryDrawInfo.data.activeadcount.five);

        }
    }

    private void SaveWeekDataToList(int ADSNumber,ADSInfo oneInfo, activeadcountInfoBasse activeadcountInfoBasse)
    {
        oneInfo.Number = ADSNumber;
        oneInfo.is_receive = activeadcountInfoBasse.is_receive;
        oneInfo.bkey = activeadcountInfoBasse.bkey;
        oneInfo.diamond = activeadcountInfoBasse.diamond;
        OfertesCtrl.OfertesDB.GetADSInfoList().Add(oneInfo);
        if (oneInfo.is_receive == 1)
            OfertesCtrl.OfertesDB.HadReadADSNumber = ADSNumber;

        //Debug.Log("is_receive:" + activeadcountInfoBasse.is_receive + "--bkey:" + activeadcountInfoBasse.bkey + "--diamond:" + activeadcountInfoBasse.diamond);
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
                            hadBuy = true;
                            OfertesCtrl.OfertesView.GifBgShow(false);
                            return;
                        }

                        List<package> Package = UserDataManager.Instance.Getuserpackage.data.package;
                        if (Package.Count <= 0)
                        {
                            return;
                        }
                        vpackage = Package[0];

                        OfertesCtrl.OfertesView.GifBgShow(true);
                        OfertesCtrl.OfertesView.GifDiamanTextShow(vpackage.diamond_qty.ToString());
                        OfertesCtrl.OfertesView.GifKeyTextShow(vpackage.bkey_qty.ToString());

                        float price = float.Parse(vpackage.price);
                        OfertesCtrl.OfertesView.GifPriceShow(price.ToString("f2"));
                        float oldPrice = float.Parse(vpackage.old_price);
                        OfertesCtrl.OfertesView.GifOldPrice(oldPrice.ToString("f2"));

                    }
                }
            }, null);
        }
    }



    public void ButtonOnclick(PointerEventData data)
    {
        if (UserDataManager.Instance.Getuserpackage != null&& !hadBuy)
        {
            //点击充值
            //UINetLoadingMgr.Instance.Show();
            //TalkingDataManager.Instance.GameCharge(vpackage.productName);

            SdkMgr.Instance.Pay(vpackage.id, vpackage.productName, 3, vpackage.price, OnPayCallback);
        }else
        {
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(133);
            UITipsMgr.Instance.PopupTips(Localization, false);
            //UITipsMgr.Instance.PopupTips("Have to buy.", false);
        }
        LOG.Info("点击了超级礼包");

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

            hadBuy = true;
            OfertesCtrl.OfertesView.GifBgShow(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VipEveryDay : BaseUIForm {

    public Text CountText,dayText;
    public GameObject Close, ReceiveButton, Mask;
    public override void OnOpen()
    {
        base.OnOpen();

        UIEventListener.AddOnClickListener(Close, CloseButtonOnClicke);
        UIEventListener.AddOnClickListener(ReceiveButton, ReceiveButtonOnclick);
        UIEventListener.AddOnClickListener(Mask, MaskClose);

        if (UserDataManager.Instance.Getvipcard != null)
        {
            CountText.text = UserDataManager.Instance.Getvipcard.data.vipinfo.day_bkey_qty.ToString() + " keys and " + UserDataManager.Instance.Getvipcard.data.vipinfo.day_diamond_qty.ToString() + " diamonds";
            dayText.text =UserDataManager.Instance.Getvipcard.data.day + " days remaining";
        }
        else
        {

            GameHttpNet.Instance.Getvipcard(VIPCallBacke);
        }
    }


    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(Close, CloseButtonOnClicke);
        UIEventListener.RemoveOnClickListener(ReceiveButton, ReceiveButtonOnclick);
        UIEventListener.RemoveOnClickListener(Mask, MaskClose);
    }

    private void MaskClose(PointerEventData data)
    {
        CloseButtonOnClicke(null);
    }
    private void CloseButtonOnClicke(PointerEventData data)
    {
        CUIManager.Instance.CloseForm(UIFormName.VIPDay);
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

                    if (CountText.gameObject==null)
                    {
                        return;
                    }

                    CountText.text = UserDataManager.Instance.Getvipcard.data.vipinfo.day_bkey_qty.ToString() + " keys and " + UserDataManager.Instance.Getvipcard.data.vipinfo.day_diamond_qty.ToString() + " diamonds";
                    dayText.text =UserDataManager.Instance.Getvipcard.data.day + " days remaining";
                }
            }

        }, null);
    }

    private void ReceiveButtonOnclick(PointerEventData data)
    {
        GameHttpNet.Instance.Getvipcardreceive(GetvipcardreceiveCallBack);
    }

    private void GetvipcardreceiveCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetvipcardreceiveCallBack---->" + result);
        if (result.Equals("error"))
        {
            LOG.Info("---GetvipcardreceiveCallBack--这条协议错误");
            return;
        }
        LoomUtil.QueueOnMainThread((param) =>
        {
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo != null)
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.selfBookInfo.data.is_receive = 1;

                    UserDataManager.Instance.Getvipcardreceive = JsonHelper.JsonToObject<HttpInfoReturn<Getvipcardreceive>>(arg.ToString());

                    //Vector3 startPos = new Vector3(-188, -355);
                    //Vector3 targetPos = new Vector3(174, 720);
                    //RewardShowData rewardShowData = new RewardShowData();
                    //rewardShowData.StartPos = startPos;
                    //rewardShowData.TargetPos = targetPos;
                    //rewardShowData.IsInputPos = false;
                    //rewardShowData.KeyNum = UserDataManager.Instance.Getvipcardreceive.data.bkey;
                    //rewardShowData.DiamondNum = UserDataManager.Instance.Getvipcardreceive.data.diamond;
                    //rewardShowData.Type = 1;
                    //EventDispatcher.Dispatch(EventEnum.GetRewardShow, rewardShowData);


                    UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.Getvipcardreceive.data.bkey);
                    UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.Getvipcardreceive.data.diamond);

                    CloseButtonOnClicke(null);
                }
            }

        }, null);
    }
}

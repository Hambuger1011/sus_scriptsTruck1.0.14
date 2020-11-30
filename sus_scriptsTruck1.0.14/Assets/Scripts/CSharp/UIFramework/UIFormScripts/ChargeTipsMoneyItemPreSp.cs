using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeTipsMoneyItemPreSp : MonoBehaviour {
    public Image MoneyIconBg, MoneyIcon,TypeImage;
    public Text BuyNumTxt, PriceTxt,Tile,x2valueText,x2valuePrice,vipText,vipPrivceText;

    public GameObject Bg_iap_dimon_ef, Bg_iap_key_ef, MoneyBgImage,X2Value,vipvalue;
    private ShopItemInfo ItemInfo;
    /// <summary>
    /// type 1 代表key 2 代表钻石
    /// </summary>
    /// <param name="type"></param>
    /// <param name="vItemInfo"></param>
    public void SetGameInit(int type, ShopItemInfo vItemInfo)
    {
        ItemInfo = vItemInfo;
        EventDispatcher.AddMessageListener(EventEnum.ChargeTipsMoneyX2Value, ChargeTipsMoneyX2Value);
        if (type==1)
        {
            MoneyIconBg.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/bg_sjan_03");
            MoneyIcon.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/bg_icon_" + vItemInfo.icon_id + "_03");
            TypeImage.sprite= ResourceManager.Instance.GetUISprite("ChargeMoneyForm/bg_icons_03");

            Bg_iap_dimon_ef.SetActive(false);
            Bg_iap_key_ef.SetActive(true);
        }
        else
        {
            MoneyIconBg.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/bg_picture2");
            MoneyIcon.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/bg_icon" + vItemInfo.icon_id + "_03");
            TypeImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/icon_dimonType");

            Bg_iap_dimon_ef.SetActive(true);
            Bg_iap_key_ef.SetActive(false);
        }

        Tile.text = vItemInfo.product_name.ToString();
        MoneyBgImage.transform.localScale = Vector3.one;
   
        PriceTxt.text = "$" + vItemInfo.price;
        ShowInit();
    }

    private void ShowInit()
    {

        if (UserDataManager.Instance.Getuserpaymallid != null)
        {
            if (UserDataManager.Instance.Getuserpaymallid.data.ids.IndexOf(ItemInfo.id) == -1)
            {
                //这个商品没有购买过，显示翻倍优惠
               
                if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data.userinfo.is_vip == 1)
                {
                    //这个是VIP首充
                    if (UserDataManager.Instance.Getvipcard != null)
                    {
                        X2Value.SetActive(true);
                        BuyNumTxt.gameObject.SetActive(false);
                        x2valueText.text = ItemInfo.total_count.ToString();
                        vipvalue.SetActive(false);

                        //这个是商品加成比例
                        float rat = UserDataManager.Instance.Getvipcard.data.vipinfo.add_rate / 100.0f;
                        x2valuePrice.text = (int)(ItemInfo.total_count*2 + 2*ItemInfo.total_count * rat) + "";
                    }
                    else
                    {
                        GameHttpNet.Instance.Getvipcard(VIPCallBacke);
                    }
                }
                else
                {
                    //不是vip的首充
                    X2Value.SetActive(true);
                    BuyNumTxt.gameObject.SetActive(false);
                    x2valueText.text = ItemInfo.total_count.ToString();
                    vipvalue.SetActive(false);

                    x2valuePrice.text = ItemInfo.total_count * 2 + "";
                }
            }
            else
            {
                //这个商品已经购买过了，不显示翻倍优惠             
                if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data.userinfo.is_vip == 1)
                {
                    //这个是VIP非首充
                    if (UserDataManager.Instance.Getvipcard != null)
                    {
                        X2Value.SetActive(false);
                        BuyNumTxt.gameObject.SetActive(false);
                        vipText.text = ItemInfo.total_count.ToString();
                        vipvalue.SetActive(true);

                        //这个是商品加成比例
                        float rat = UserDataManager.Instance.Getvipcard.data.vipinfo.add_rate / 100.0f;
                        vipPrivceText.text = (int)(ItemInfo.total_count + ItemInfo.total_count * rat) + "";
                    }
                    else
                    {
                        GameHttpNet.Instance.Getvipcard(VIPCallBacke);
                    }
                }
                else
                {
                    //不是vip的非首充
                    X2Value.SetActive(false);
                    BuyNumTxt.gameObject.SetActive(true);                 
                    vipvalue.SetActive(false);
                    BuyNumTxt.text = ItemInfo.total_count+"";
                }
            }
        }
        else
        {
            GameHttpNet.Instance.Getuserpaymallid(GetuserpaymallidCallback);
        }
    }

    #region VIP信息
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

                    if (x2valuePrice.gameObject==null)
                    {
                        return;
                    }
                    ShowInit();
                }
            }

        }, null);
    }
    #endregion

    private void GetuserpaymallidCallback(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetuserpaymallidCallback---->" + result);

        LoomUtil.QueueOnMainThread((param) =>
        {

            JsonObject jo = JsonHelper.JsonToJObject(result);

            if (jo.code == 200)
            {
                UserDataManager.Instance.Getuserpaymallid = JsonHelper.JsonToObject<HttpInfoReturn<Getuserpaymallid>>(result);
                ShowInit();
            }
        }, null);
    }
    public void ChargeTipsMoneyX2Value(Notification notification)
    {
        float DataPrice= float.Parse(notification.Data.ToString());
        float Price = float.Parse(ItemInfo.price);

        if (DataPrice== Price)
        {
            X2Value.SetActive(false);
            BuyNumTxt.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        EventDispatcher.RemoveMessageListener(EventEnum.ChargeTipsMoneyX2Value, ChargeTipsMoneyX2Value);
    }
}

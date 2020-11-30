using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AB;
using pb;

public class BookActivities : BaseUIForm
{
    public GameObject UIMask, ItemBookMax, Pref, BuyButton;
    public Text OriginalPrice, BuyPrice;

    private UserData m_userData;
    private giftlist giftlist;
    public override void OnOpen()
    {
        base.OnOpen();

        //获取礼包信息
        getbookgiftbagBackCall(null);

        UIEventListener.AddOnClickListener(UIMask, CloseUi);
        UIEventListener.AddOnClickListener(BuyButton, BuyButtonOnclick);
    }

    public override void OnClose()
    {
        base.OnClose();        
    }

    public void BuyButtonOnclick(PointerEventData data)
    {
        //LOG.Info("书本礼包购买按钮被点击了,giftid是：" + giftlist.giftid);
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.Buybookgiftbag(giftlist.giftid, BuyHandle);
    }

    private void Init(giftlist giftlist)
    {
        this.giftlist = giftlist;
        //原价钻石
        OriginalPrice.text = giftlist.original_diamond.ToString();
        //优惠价钻石
        BuyPrice.text = giftlist.diamond_qty.ToString();

        List<booklist> items = giftlist.booklist;
        for (int i = 0; i < items.Count; i++)
        {
            GameObject go = Instantiate(Pref);
            go.SetActive(true);
            go.transform.SetParent(ItemBookMax.transform);

            Image imag = go.transform.GetComponentInChildren<Image>();
            imag.sprite = ABSystem.ui.GetUITexture(AbTag.Global,string.Concat("Assets/Bundle/BookPreview/Icon/", items[i].id, ".png"));

        }
    }

    private void getbookgiftbagBackCall(object arg)
    {
        if (UserDataManager.Instance.GetBookGiftBag != null)
        {
            List<giftlist> item = UserDataManager.Instance.GetBookGiftBag.data.giftlist;
           
            Init(item[0]);         
        }
    }

private void BuyHandle(object arg)
{
    string result = arg.ToString();
    LOG.Info("----BuyHandle---->" + result);

    LoomUtil.QueueOnMainThread((param) =>
    {
        //UINetLoadingMgr.Instance.Close();
        JsonObject jo = JsonHelper.JsonToJObject(result);

        if (jo.code == 200)
        {
            //LOG.Info("购买成功");
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(137);
            UITipsMgr.Instance.PopupTips(Localization, false);


            //UITipsMgr.Instance.PopupTips("Purchase completed!", false);


            List<booklist> items = this.giftlist.booklist;
            for (int i = 0; i < items.Count; i++)
            {
                //把书本礼包添加进列表
                if(UserDataManager.Instance.UserData.bookList.IndexOf(items[i].id)==-1)
                {
                    UserDataManager.Instance.UserData.bookList.Add(items[i].id);
                    //LOG.Info("书本礼包购买成功添加列表的书本有：" + items[i].id);
                }else
                {
                    //LOG.Info("书本礼包购买成功,列表中已经有这本书，无需添加有：");
                }
                
            }

            CloseUi(null);
        }
        else if (jo.code == 201)
        {
            LOG.Info("购买失败");

            var Localization = GameDataMgr.Instance.table.GetLocalizationById(138);
            UITipsMgr.Instance.PopupTips(Localization, false);

            //UITipsMgr.Instance.PopupTips("Purchase failed!", false);
        }
        else if (jo.code == 205)
        {
            LOG.Info("钥匙不足");
            MainTopSprite TopSprite = CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop);
            if (TopSprite != null)
            {
                TopSprite.OpenChargeMoney_Diamonds(null);
            }
        }
        else if (jo.code == 206)
        {
            MainTopSprite TopSprite = CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop);
            if (TopSprite != null)
            {
                TopSprite.OpenChargeMoney_Diamonds(null);
            }
            LOG.Info("砖石不足");
        }
        else if (jo.code == 207)
        {
            LOG.Info("已经购买过了");

            var Localization = GameDataMgr.Instance.table.GetLocalizationById(138);
            UITipsMgr.Instance.PopupTips(Localization, false);

            //UITipsMgr.Instance.PopupTips("Purchase failed!", false);
        }
        else
        {
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(139);
            UITipsMgr.Instance.PopupTips(Localization, false);

            //UITipsMgr.Instance.PopupTips("You've already purchase.", false);
        }
    }, null);
}

private void CloseUi(PointerEventData data)
{
    CUIManager.Instance.CloseForm(UIFormName.BookActivities);
}
}

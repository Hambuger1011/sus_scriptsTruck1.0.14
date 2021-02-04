using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using pb;
using System.Linq;

public class NewChargeTips : BaseUIForm {

    private GameObject valuegame,OriginalParice,Maske, BuyButton;
    private Image typeImage, Buytype;
    private Text DiscountedPrices, Number, Price;
    private List<mallarrInfo> mallarrInfoList;
    private mallarrInfo mItemInfo;
    private int NeedNumber, Numbering;
    private int buyType;
    private bool needHideMainTop;
    private GameObject fx_glow_key, fx_glow_zuanshi;
    private Image OriginalPrice;
    private Text OriginalPriceText;
    private Image BG;
    //private Outline DiscountedPrices, Number, OriginalPriceText;

    private Dictionary<string, string> BuyItemDic;

    public override void OnOpen()
    {
        base.OnOpen();

        valuegame = transform.Find("Canvas/BG/2value").gameObject;
        OriginalParice = transform.Find("Canvas/BG/DiscountedPrices").gameObject;

        typeImage = transform.Find("Canvas/BG/type").GetComponent<Image>();
        Buytype = transform.Find("Canvas/BG/DiscountedPrices/Buytype").GetComponent<Image>();
        DiscountedPrices = transform.Find("Canvas/BG/DiscountedPrices").GetComponent<Text>();
        Number = transform.Find("Canvas/BG/DiscountedPrices/Number").GetComponent<Text>();
        Price = transform.Find("Canvas/BG/BuyButton/Price").GetComponent<Text>();
        Maske = transform.Find("Canvas/UIMask").gameObject;
        BuyButton = transform.Find("Canvas/BG/BuyButton").gameObject;

        OriginalPrice = transform.Find("Canvas/BG/OriginalPrice").GetComponent<Image>();
        OriginalPriceText = transform.Find("Canvas/BG/OriginalPrice/OriginalPriceText").GetComponent<Text>();
        BG = transform.Find("Canvas/BG").GetComponent<Image>();

        fx_glow_key = transform.Find("Canvas/BG/fx_glow_key").gameObject;
        fx_glow_zuanshi = transform.Find("Canvas/BG/fx_glow_zuanshi").gameObject;

        //fx_key_beiguang = transform.Find("Canvas/fx_key_beiguang").gameObject;
        //fx_zuanshi_beiguang = transform.Find("Canvas/fx_zuanshi_beiguang").gameObject;


        UIEventListener.AddOnClickListener(Maske,CloseUI);
        UIEventListener.AddOnClickListener(BuyButton, BuyButtonOn);
        
        if (!IGGSDKManager.Instance.HasProductList())
        {
            UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218),GameDataMgr.Instance.table.GetLocalizationById(300),AlertType.Sure, (value) =>
            {
                CUIManager.Instance.CloseForm(UIFormName.ChargeMoneyForm);
            });
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(Maske, CloseUI);
        UIEventListener.RemoveOnClickListener(BuyButton, BuyButtonOn);
    }

    private void CloseUI(PointerEventData data)
    {
        CUIManager.Instance.CloseForm(UIFormName.NewChargeTips);
    }

    private void BuyButtonOn(PointerEventData data)
    {
        Debug.Log("购买按钮点击");

        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        if (UserDataManager.Instance.shopList != null && UserDataManager.Instance.shopList.data != null)
        {
           
            if (mItemInfo != null)
            {
                //TalkingDataManager.Instance.GameCharge(mItemInfo.productName);
                //UINetLoadingMgr.Instance.Show();

                AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
                IGGSDKManager.Instance.PayItem(mItemInfo.product_id);

                // SdkMgr.Instance.Pay(mItemInfo.id, mItemInfo.product_id, 2, mItemInfo.price, OnPayCallback);
            }
            else
            {
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(151);
                UITipsMgr.Instance.PopupTips(Localization, false);

                //UITipsMgr.Instance.PopupTips("There're no items for sale right now.");
            }

        }
        else
        {
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(151);
            UITipsMgr.Instance.PopupTips(Localization, false);

            //UITipsMgr.Instance.PopupTips("There're no items for sale right now.");
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


            TalkingDataManager.Instance.Parameters(buyType, mItemInfo.price);

            UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.orderFormSubmitResultInfo.data.bkey);
            UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.orderFormSubmitResultInfo.data.diamond);
            UserDataManager.Instance.ResetMoney(3, UserDataManager.Instance.orderFormSubmitResultInfo.data.ticket);

            var Localization = GameDataMgr.Instance.table.GetLocalizationById(152);
            UITipsMgr.Instance.PopupTips(Localization, false);
            

            EventDispatcher.Dispatch(EventEnum.ChargeTipsMoneyX2Value, mItemInfo.price);

            //EventDispatcher.Dispatch(EventEnum.OnDiamondNumChange, UserDataManager.Instance.orderFormSubmitResultInfo.data.diamond);
        }

        backToMainForm(null);
    }
    private void backToMainForm(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CUIManager.Instance.CloseForm(UIFormName.NewChargeTips);
        if (needHideMainTop)
        {
            CUIManager.Instance.OpenForm(UIFormName.MainFormTop);
        }
    }
    public void CatTopMainChange()
    {
      
    }
    public void Init(int type,int needNum, float vPrice, bool vNeedHideMainTop = false)
    {
        if (needHideMainTop)
        {
            CUIManager.Instance.CloseForm(UIFormName.MainFormTop);
        }
        if (type==1)
        {
            //钥匙类型
            //typeImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/icon_key1");
            Buytype.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/icon_keyType");
            OriginalPrice.sprite= ResourceManager.Instance.GetUISprite("ChargeMoneyForm/icon_keyType");
            //fx_glow_key.SetActive(true);
           
            BG.sprite= ResourceManager.Instance.GetUISprite("ChargeMoneyForm/bg_picture2");
            BG.SetNativeSize();

            float R = 122;
            float G = 43;
            float B = 151;
            //DiscountedPrices.GetComponent<Outline>().effectColor = new Color(R / 255.0f, G / 255.0f, B / 255.0f);
            //Number.GetComponent<Outline>().effectColor = new Color(R / 255.0f, G / 255.0f, 221 / B);
            //OriginalPriceText.GetComponent<Outline>().effectColor = new Color(R / 255.0f, G / 255.0f, B / 255.0f);
        }
        else
        {
            //钻石类型
            //typeImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/icon_dimon");
            Buytype.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/icon_dimonType");
            OriginalPrice.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/icon_dimonType");
            //fx_glow_zuanshi.SetActive(true);
           
            BG.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/bg_picture1");
            BG.SetNativeSize();

            //设置边框颜色
            float R = 18;
            float G = 95;
            float B = 221;
            //DiscountedPrices.GetComponent<Outline>().effectColor = new Color(R / 255.0f, G / 255.0f, B / 255.0f);
            //Number.GetComponent<Outline>().effectColor = new Color(R / 255.0f, G / 255.0f, 221 / B);
            //OriginalPriceText.GetComponent<Outline>().effectColor = new Color(R / 255.0f, G / 255.0f, B / 255.0f);
        }
        buyType = type;
        NeedNumber = needNum;
        Numbering = 0;

        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.Getrecommandmall(type, GetrecommandmallCallBack);
    }

    private void ItemInit()
    {
        LOG.Info("分档推荐推荐的档数是:" + (Numbering+1));

        mItemInfo = mallarrInfoList[Numbering];
        if (mItemInfo.is_double==2)
        {
            //这个是双倍状态
            valuegame.SetActive(true);
        }
        else
        {
            //不是双倍状态
            valuegame.SetActive(false);
        }

        if (mItemInfo.total_count== mItemInfo.count)
        {
            //没有优惠
            DiscountedPrices.gameObject.SetActive(false);
            OriginalPrice.gameObject.SetActive(true);
            OriginalPriceText.text=mItemInfo.count.ToString();
        }
        else
        {
            DiscountedPrices.gameObject.SetActive(true);
            OriginalPrice.gameObject.SetActive(false);
        }

        DiscountedPrices.text =mItemInfo.total_count.ToString();
        Number.text =mItemInfo.count.ToString();

        if (IGGSDKManager.Instance.HasProductList())
        {
            Price.text = IGGSDKManager.Instance.GetItemPrice(mItemInfo.product_id);
        }
        else
        {
            Price.text = "";
        }
    }

    private void GetrecommandmallCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetrecommandmallCallBack---->" + result);
        if (result.Equals("error"))
        {
            LOG.Info("---GetrecommandmallCallBack--这条协议错误");
            return;
        }
        LoomUtil.QueueOnMainThread((param) =>
        {
            JsonObject jo = JsonHelper.JsonToJObject(result);

            //UINetLoadingMgr.Instance.Close();
            if (jo != null)
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.recommandmallInfo = JsonHelper.JsonToObject<HttpInfoReturn<Getrecommandmall>>(result);

                    mallarrInfoList = UserDataManager.Instance.recommandmallInfo.data.data_list;
                    mallarrInfoList.Sort(CompareCount);

                        for (int i=0;i< mallarrInfoList.Count;i++)
                    {
                        if (mallarrInfoList[i].count> NeedNumber)
                        {

                            if (buyType == 2)
                            {
                                Numbering = i;
                                //类型是钻石的，第一档符合条件，ios 我们硬性推荐第二档
#if UNITY_IOS
        Numbering = i+1;
#endif
                            }
                            else
                            {

                                Numbering = i;
                            }
                            ItemInit();
                            return;
                        }else if(i== mallarrInfoList.Count-1)
                        {
                            Numbering = mallarrInfoList.Count-1;
                            ItemInit();
                        }
                    }
                }
            }

        }, null);
    }

    private int CompareCount(mallarrInfo item1, mallarrInfo item2)
    {
        if (item1.total_count > item2.total_count)
            return 1;
        else if (item1.total_count < item2.total_count)
            return -1;

        return 0;
    }
    
}

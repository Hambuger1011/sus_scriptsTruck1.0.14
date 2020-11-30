using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UGUI;
using AB;
using DG.Tweening;
using pb;
/// <summary>
/// 快捷购买
/// </summary>
public class ChargeTipsForm :   BaseUIForm 
{
    public RectTransform TopBarTrans;
    public GameObject CloseBtn;
    public GameObject DiamondBtnGroup;
    public Image TopDiamondIcon;
    public Text AllDiamondNumTxt;
    public RectTransform TipsGroup;
    public Text TipsTxt;
    public Image MoneyIconBg;
    public Image MoneyIcon;
    public Text BuyNumTxt;
    public Text PriceTxt;
    public GameObject BuyBtn;
    public Image PayIcon;
    public Text BtnDescTxt;
    public Text PayNumTxt;
    public GameObject KeyEffect;
    public GameObject DiamondEffect;
    public GameObject Content, ChargeTipsMoneyItemPre;

    private GameObject AddButton;
    public ScrollViewListener ScrollView;

    private int buyType;
    private int buyNums;
    private bool needHideMainTop;
    private ShopItemInfo mItemInfo;
    private List<ShopItemInfo> diamondList;


    private bool DiamondAddisFalse = false;
    private CatMainForm catMainform;

    public override void OnOpen()
    {
        base.OnOpen();
      
        EventDispatcher.Dispatch(EventEnum.isopenDiamondAddDith,1);

        AddButton = transform.Find("Canvas/Frame/TopBar/DiamondButtonGroup/AddButton").gameObject;
        DiamondAddisFalse = false;

        UIEventListener.AddOnClickListener(CloseBtn, backToMainForm);
        UIEventListener.AddOnClickListener(BuyBtn.gameObject, toBuyHandler);
        //UIEventListener.AddOnClickListener(DiamondBtnGroup, OpenChargeMoney_Diamonds);

        addMessageListener(EventEnum.OnKeyNumChange.ToString(), OnKeyNumChange);
        addMessageListener(EventEnum.OnDiamondNumChange, OnDiamondNumChange);
        

        //TopBarTrans.localScale = new Vector3(1, 0, 1);
        BuyBtn.transform.localPosition = new Vector3(300, -800, 0);
        MoneyIconBg.rectTransform.localScale = new Vector3(0, 0, 0);
        TipsGroup.localScale = new Vector3(1, 0, 1);

        //TopBarTrans.DOScaleY(1, 0.25f).SetEase(Ease.OutBack).Play();
        TipsGroup.DOScaleY(1, 0.25f).SetEase(Ease.Linear).Play();
        MoneyIconBg.rectTransform.DOScale(1, 0.25f).SetDelay(0.25f).SetEase(Ease.OutBack).Play();
        BuyBtn.GetComponent<RectTransform>().DOAnchorPos(new Vector2(300, -282), 0.25f).SetEase(Ease.Linear).Play();


        //【屏幕适配】
        float offect = XLuaHelper.UnSafeAreaNotFit(this.myForm, TopBarTrans, 750, 110);
    

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

        EventDispatcher.Dispatch(EventEnum.isopenDiamondAddDith, 0);


        UIEventListener.RemoveOnClickListener(CloseBtn, backToMainForm);
        UIEventListener.RemoveOnClickListener(BuyBtn.gameObject, toBuyHandler);
        //UIEventListener.RemoveOnClickListener(DiamondBtnGroup, OpenChargeMoney_Diamonds);
    }

    private void OnKeyNumChange(Notification notification)
    {
        AllDiamondNumTxt.text = ((int)notification.Data).ToString();
    }
    private void OnDiamondNumChange(Notification notification)
    {
        AllDiamondNumTxt.text = ((int)notification.Data).ToString();
    }

    /// <summary>
    /// 返回商品付费类型
    /// </summary>
    /// <returns></returns>
    public int returnbuyType()
    {
        return buyType;
    }
    public void CatTopMainChange()
    {
        AddButton.SetActive(false);
        DiamondAddisFalse = true;     
    }
    /// <summary>
    /// 付费弹窗
    /// </summary>
    /// <param name="vType">1;钥匙，2;钻石</param>
    /// <param name="vNum"></param>
    /// <param name="vPrice"></param>
    /// <param name="vNeedHideMainTop"></param>
    public void Init(int vType,int vNum,float vPrice,bool vNeedHideMainTop = false)
    {
        buyType = vType;
        buyNums = vNum;
        needHideMainTop = vNeedHideMainTop;
        if(needHideMainTop)
        {
            CUIManager.Instance.CloseForm(UIFormName.MainFormTop);
        }

        GameHttpNet.Instance.Getuserpaymallid(GetuserpaymallidCallback);
    }
    private void GetuserpaymallidCallback(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetuserpaymallidCallback---->" + result);
        if (result.Equals("error"))
        {
            LOG.Info("----GetuserpaymallidCallback---->返回错误");
            InitCallBack();
            return;
        }
        LoomUtil.QueueOnMainThread((param) =>
        {

            JsonObject jo = JsonHelper.JsonToJObject(result);

            if (jo.code == 200)
            {
                UserDataManager.Instance.Getuserpaymallid = JsonHelper.JsonToObject<HttpInfoReturn<Getuserpaymallid>>(result);

                InitCallBack();
            }
        }, null);
    }
    private void InitCallBack()
    {
        mItemInfo = null;

        //清空保存付费价格的列表
        MyBooksDisINSTANCE.Instance.paytotalTypeListGet().Clear();
        if (buyType == 1)
        {
            TipsTxt.text = "You don't have enough Keys!\nBuy more now!";
            List<ShopItemInfo> keyList = UserDataManager.Instance.shopList.data.key_list;
            if (keyList.Count != 0)
            {
                int len = keyList.Count;
                int childNum = 0;
                for (int i = 0; i < len; i++)
                {
                    ShopItemInfo tempItemInfo = keyList[i];
                    if (tempItemInfo != null)
                    {

                        float Price = float.Parse(tempItemInfo.price);
                        if (Price != 0)
                        {
                            GameObject go = Instantiate(ChargeTipsMoneyItemPre);
                            go.SetActive(true);
                            go.transform.SetParent(Content.transform);
                            go.transform.localScale = Vector3.one;
                            go.GetComponent<ChargeTipsMoneyItemPreSp>().SetGameInit(1, tempItemInfo);
                            childNum++;
                        }
                        MyBooksDisINSTANCE.Instance.paytotalTypeListSet(Price);
                    }
                }
                ScrollView.InitInfo(childNum);
            }

            if (mItemInfo == null)
                mItemInfo = UserDataManager.Instance.shopList.data.key_list[0];


            //MoneyIconBg.sprite = ResourceManager.Instance.GetUISprite("ChargeTipsForm/bg_iap_key");
            //MoneyIcon.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/bg_icon_"+mItemInfo.IconID+"_03");
            PayIcon.sprite = ResourceManager.Instance.GetUISprite("MainForm/icon_key");
            TopDiamondIcon.sprite = ResourceManager.Instance.GetUISprite("MainForm/icon_key");
            AllDiamondNumTxt.text = UserDataManager.Instance.UserData.KeyNum.ToString();



        }
        else
        {
            //把钻石商品的价格大于0的添加进列表中
            AdddiamondarrList();

            TipsTxt.text = "You don't have enough Diamonds!\nBuy more now!";
            List<ShopItemInfo> diamondList = UserDataManager.Instance.shopList.data.diamond_list;
            if (diamondList.Count != 0)
            {
                int childNum = 0;
                int len = diamondList.Count;
                for (int i = 0; i < len; i++)
                {
                    ShopItemInfo tempItemInfo = diamondList[i];
                    if (tempItemInfo != null)
                    {

                        float Price = float.Parse(tempItemInfo.price);

                        if (Price != 0)
                        {
                            GameObject go = Instantiate(ChargeTipsMoneyItemPre);
                            go.SetActive(true);
                            go.transform.SetParent(Content.transform);
                            go.transform.localScale = Vector3.one;
                            go.GetComponent<ChargeTipsMoneyItemPreSp>().SetGameInit(2, tempItemInfo);
                            childNum++;
                        }

                        MyBooksDisINSTANCE.Instance.paytotalTypeListSet(Price);
                    }
                }
                ScrollView.InitInfo(childNum);
            }

            if (mItemInfo == null)
                mItemInfo = UserDataManager.Instance.shopList.data.diamond_list[0];

            //MoneyIconBg.sprite = ResourceManager.Instance.GetUISprite("ChargeTipsForm/bg_iap_dimon");
            //MoneyIcon.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/bg_icon"+mItemInfo.IconID+"_03");

            PayIcon.sprite = ResourceManager.Instance.GetUISprite("MainForm/icon_dimon");
            TopDiamondIcon.sprite = ResourceManager.Instance.GetUISprite("MainForm/icon_dimon");
            AllDiamondNumTxt.text = UserDataManager.Instance.UserData.DiamondNum.ToString();

        }

        if (mItemInfo != null)
        {
            //BuyNumTxt.text = "X " + mItemInfo.count;
            //PriceTxt.text = "$" + mItemInfo.price;
            PayNumTxt.text ="" /*mItemInfo.count.ToString()*/;
            buyNums = mItemInfo.total_count;

            if (UserDataManager.Instance.Getuserpaymallid != null)
            {
                if (UserDataManager.Instance.Getuserpaymallid.data.ids.IndexOf(mItemInfo.id) == -1)
                {
                    //这个商品没有购买过，显示翻倍优惠  
                    if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data.userinfo.is_vip == 1)
                    {
                        //vip的首充
                        if (UserDataManager.Instance.Getvipcard != null)
                        {
                            
                            //这个是商品加成比例
                            float rat = UserDataManager.Instance.Getvipcard.data.vipinfo.add_rate / 100.0f;
                            PayNumTxt.text = (int)(mItemInfo.total_count * 2 + 2*mItemInfo.total_count * rat) + "";
                        }
                    }else
                    {
                        //不是vip的首充
                        PayNumTxt.text = mItemInfo.total_count * 2 + "";
                    }
                }
                else
                {
                    //这个商品购买过
                    if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data.userinfo.is_vip == 1)
                    {
                        //vip的非首充
                        if (UserDataManager.Instance.Getvipcard != null)
                        {

                            //这个是商品加成比例
                            float rat = UserDataManager.Instance.Getvipcard.data.vipinfo.add_rate / 100.0f;
                            PayNumTxt.text = (int)(mItemInfo.total_count+ mItemInfo.total_count * rat) + "";
                        }
                    }
                    else
                    {
                        //不是vip的非首充
                        PayNumTxt.text = mItemInfo.total_count+ "";
                    }
                }
            }

            
        }

        PayIcon.SetNativeSize();
        TopDiamondIcon.SetNativeSize();
    }

    private void OpenChargeMoney_Diamonds(PointerEventData data)
    {
        if (DiamondAddisFalse) return;

        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        DoOpenChargeKeyOrMoney(0);
    }

    private void DoOpenChargeKeyOrMoney(int vOpenFrom = 0)
    {
        if (buyType == 1)//这个是Key的购买
        {
            CUIManager.Instance.OpenForm(UIFormName.ChargeMoneyForm);
            ChargeMoneyForm Char = CUIManager.Instance.GetForm<ChargeMoneyForm>(UIFormName.ChargeMoneyForm);
            Char.SetFormStyle(1,vOpenFrom);
            Char.GamePlayOpenKey();

            CUIManager.Instance.OpenForm(UIFormName.MainFormTop);
            CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop).GamePlayOnpenKey(1);
        }
        else
        {
            CUIManager.Instance.OpenForm(UIFormName.ChargeMoneyForm);
            ChargeMoneyForm Char = CUIManager.Instance.GetForm<ChargeMoneyForm>(UIFormName.ChargeMoneyForm);
            Char.SetFormStyle(2, vOpenFrom);
            Char.GamePlayOpenDiamond();


            CUIManager.Instance.OpenForm(UIFormName.MainFormTop);
            CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop).GamePlayOnpenDiamond(1);
        }
    }

    private void backToMainForm(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CUIManager.Instance.CloseForm(UIFormName.ChargeTipsForm);
        if(needHideMainTop)
        {
            CUIManager.Instance.OpenForm(UIFormName.MainFormTop);
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
        }
        if (UserDataManager.Instance.orderFormSubmitResultInfo != null && UserDataManager.Instance.orderFormSubmitResultInfo.data != null)
        {
            if (buyType == 1)
            {
                //保存当前最新购买Key的价格
                UserDataManager.Instance.userInfo.data.userinfo.paybkeytotal = mItemInfo.price;

            }
            else
            {
                //保存当前最新购买钻石的价格
                UserDataManager.Instance.userInfo.data.userinfo.paydiamondtotal = mItemInfo.price;

            }

            TalkingDataManager.Instance.Parameters(buyType, mItemInfo.price);

            UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.orderFormSubmitResultInfo.data.bkey);
            UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.orderFormSubmitResultInfo.data.diamond);
            UserDataManager.Instance.ResetMoney(3, UserDataManager.Instance.orderFormSubmitResultInfo.data.ticket);

            var Localization = GameDataMgr.Instance.table.GetLocalizationById(152);
            UITipsMgr.Instance.PopupTips(Localization, false);
            

            EventDispatcher.Dispatch(EventEnum.ChargeTipsMoneyX2Value, mItemInfo.price);
        }
        backToMainForm(null);
    }

    private void toBuyHandler(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        if(UserDataManager.Instance.shopList != null && UserDataManager.Instance.shopList.data != null)
        {
            //mItemInfo = null;
            //if(buyType == 1)
            //{
            //    if (UserDataManager.Instance.shopList.data.bkeyarr != null)
            //        mItemInfo = UserDataManager.Instance.shopList.data.bkeyarr[0];
            //}else
            //{
            //    if (UserDataManager.Instance.shopList.data.diamondarr != null)
            //        mItemInfo = UserDataManager.Instance.shopList.data.diamondarr[0];
            //}

            if (mItemInfo != null)
            {
                //TalkingDataManager.Instance.GameCharge(mItemInfo.productName);
                //UINetLoadingMgr.Instance.Show();

                SdkMgr.Instance.Pay(mItemInfo.id, mItemInfo.product_id, 2, mItemInfo.price, OnPayCallback);
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
    

    private void BuyKeyReturnHandler(bool value)
    {
        backToMainForm(null);
    }

    /// <summary>
    /// OK按钮的信息更新
    /// </summary>
    public void BtnOKInfoChange(int indext)
    {
        if (buyType==1)
        {
            // key
            PayIcon.sprite = ResourceManager.Instance.GetUISprite("MainForm/icon_key");

            List<ShopItemInfo> keyList = UserDataManager.Instance.shopList.data.key_list;
            if (indext >= keyList.Count)
                indext = keyList.Count - 1;
             mItemInfo = keyList[indext];
            PayNumTxt.text ="" /*mItemInfo.count.ToString()*/;

            if (UserDataManager.Instance.Getuserpaymallid != null)
            {
                if (UserDataManager.Instance.Getuserpaymallid.data.ids.IndexOf(mItemInfo.id) == -1)
                {
                    //这个商品没有购买过，显示翻倍优惠                      
                    if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data.userinfo.is_vip == 1)
                    {
                        //vip的首充
                        if (UserDataManager.Instance.Getvipcard != null)
                        {

                            //这个是商品加成比例
                            float rat = UserDataManager.Instance.Getvipcard.data.vipinfo.add_rate / 100.0f;
                            PayNumTxt.text = (int)(mItemInfo.total_count * 2 + 2*mItemInfo.total_count * rat) + "";
                        }
                    }
                    else
                    {
                        //不是vip的首充
                        PayNumTxt.text = mItemInfo.total_count * 2 + "";
                    }
                }
                else
                {
                    //这个商品购买过
                    if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data.userinfo.is_vip == 1)
                    {
                        //vip的非首充
                        if (UserDataManager.Instance.Getvipcard != null)
                        {

                            //这个是商品加成比例
                            float rat = UserDataManager.Instance.Getvipcard.data.vipinfo.add_rate / 100.0f;
                            PayNumTxt.text = (int)(mItemInfo.total_count + mItemInfo.total_count * rat) + "";
                        }
                    }
                    else
                    {
                        //不是vip的非首充
                        PayNumTxt.text = mItemInfo.total_count.ToString();
                    }
                }
            }
        }
        else
        {
            PayIcon.sprite = ResourceManager.Instance.GetUISprite("MainForm/icon_dimon");
            //Debug.Log("2222");
            //List<ShopItemInfo> diamondList = UserDataManager.Instance.shopList.data.diamondarr;
            if (indext >= diamondList.Count)
                indext = diamondList.Count - 1;
            mItemInfo = diamondList[indext];
            PayNumTxt.text = mItemInfo.total_count.ToString();

            if (UserDataManager.Instance.Getuserpaymallid != null)
            {
                if (UserDataManager.Instance.Getuserpaymallid.data.ids.IndexOf(mItemInfo.id) == -1)
                {
                    //这个商品没有购买过，显示翻倍优惠  
                    PayNumTxt.text = mItemInfo.total_count * 2 + "";
                }
            }
        }
    }

    private void AdddiamondarrList()
    {
        //Debug.Log("1111");
        if (diamondList==null)
        {
            diamondList = new List<ShopItemInfo>();
        }
        diamondList.Clear();

        if (UserDataManager.Instance.shopList.data.diamond_list!=null)
        {
            for (int i=0;i< UserDataManager.Instance.shopList.data.diamond_list.Count;i++)
            {
                float Price =float.Parse(UserDataManager.Instance.shopList.data.diamond_list[i].price);
                if (Price>0)
                {
                    diamondList.Add(UserDataManager.Instance.shopList.data.diamond_list[i]);

                    //Debug.Log("添加的价钱是："+ Price+"--i:"+i);
                }
            }
        }
    }
}

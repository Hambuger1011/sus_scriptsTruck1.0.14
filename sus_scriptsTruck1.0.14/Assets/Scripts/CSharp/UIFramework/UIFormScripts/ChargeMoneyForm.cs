using AB;
using DG.Tweening;
using pb;
using System.Collections;
using System.Collections.Generic;
using IGG.SDK.Core.Error;
using Script.Game.Helpers;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChargeMoneyForm : BaseUIForm
{

    public RectTransform frameTrans;
    //public GameObject Content;
    public Image DispalyBgImage, DiamondMoneyImage;
    //public Image KeysSelectButton;
    //public Image DiamondsSelectButton;
    public GameObject KeyEffect;
    public GameObject DiamondEffect;
    //public GameObject SelectBtnGroup;
    public GameObject FirstGiftGo;
    public GameObject BuyBtnGo;

    //public GameObject DiamondOff, DiamondOn, KeyOn, KeyOff;
    public GameObject ScrollViewKey, ScrollViewKeyContent, ScrollViewDiamond, ScrollViewDiamondContent, ScrollViewTicket, ScrollViewTicketContent;
    public Text numbertext, prices, dwonbgText, BonusText;
    public GameObject RecommendBottuon;

    public GameObject x2value;
    public Image priceImage;
    public Text originalPrice;

    public GameObject dKeysprites, kDiamondsprites;

    private RectTransform mGroupGo;


    private int mMoneyType;     // 1 钥匙，2 钻石 ，3 票券
    private int mOpenFrom = 0;      //从哪里打开的这个商场界面（0：表示从mainTop里面打开，1：表示还未付费的玩家，从快捷购买那里打开）
    private Sprite backSprite;
    private Sprite frontSprite;
    private Sprite keyBGSprite;
    private Sprite DiamondBGSprite;

    private List<ChargeItemForm> itemList;
    private List<ChargeMoneyItemScripte> ChargeMoneyItemScripteList;

    private CanvasGroup DispalyImageCanvasGroup;

    private float RecommendPrice = 0;//记录推荐价格
    private ChargeMoneyItemScripte RecommendGame, RecommendGamekey, RecommendGameDiam;//这个是记录推荐的商品价格相应的子物体
    private ShopItemInfo tmepItemInfo, tmepDItemInfo, tmepKItemInfo;

    
    private float alphaMin = 0.8f;
    private Outline AndenjoyText;

    private ScrollRect ScrollView;
    //private Scrollbar Scrollbar;
    private GameObject DiamanList, KeyList, TopDiamanButton, TopKiamanButton,LiveSupportGo;
    private Text TopDPrice, TopKPrice,TopDNumber,TopKNumber;


    private GameObject mDiamanFirst;
    private GameObject mKeyFirst;
    private void Awake()
    {
        mGroupGo = transform.Find("Frame/GameObject").GetComponent<RectTransform>();
        AndenjoyText = transform.Find("Frame/GameObject/AndenjoyText").GetComponent<Outline>();

        ScrollView = transform.Find("Frame/ScrollView").GetComponent<ScrollRect>();
        //Scrollbar = transform.Find("Frame/ScrollView/Scrollbar Vertical").GetComponent<Scrollbar>();
        DiamanList = transform.Find("Frame/ScrollView/Viewport/Content/DiamanList").gameObject;
        KeyList= transform.Find("Frame/ScrollView/Viewport/Content/KeyList").gameObject;

        TopDiamanButton = transform.Find("Frame/ScrollView/Viewport/Content/DiamanFirst/TopDiamanButton").gameObject;
        TopKiamanButton = transform.Find("Frame/ScrollView/Viewport/Content/KeyFirst/TopDiamanButton").gameObject;
        TopDPrice = transform.Find("Frame/ScrollView/Viewport/Content/DiamanFirst/TopDiamanButton/Price").GetComponent<Text>();
        TopKPrice= transform.Find("Frame/ScrollView/Viewport/Content/KeyFirst/TopDiamanButton/Price").GetComponent<Text>();
        TopDNumber = transform.Find("Frame/ScrollView/Viewport/Content/DiamanFirst/DiamantText").GetComponent<Text>();
        TopKNumber = transform.Find("Frame/ScrollView/Viewport/Content/KeyFirst/DiamantText").GetComponent<Text>();
        
        LiveSupportGo = transform.Find("Frame/ScrollView/Viewport/Content/DiamondDis/LiveSupBg").gameObject;

        mDiamanFirst = DisplayUtil.GetChild(ScrollView.gameObject, "DiamanFirst");
        mKeyFirst = DisplayUtil.GetChild(ScrollView.gameObject, "KeyFirst");

        mDiamanFirst.SetActive(false);
        mKeyFirst.SetActive(false);

    }

    public override void OnOpen()
    {
        if (GameDataMgr.Instance.InAutoPlay)
        {
            GameDataMgr.Instance.AutoPlayPause = true;
            GameDataMgr.Instance.InAutoPlay = false;
        }
        base.OnOpen();


#if !UNITY_EDITOR && UNITY_ANDROID
        if (ResolutionAdapter.androidisSafeArea == true)
        {
             RectTransform ScrollViewRect = ScrollView.GetComponent<RectTransform>();
             ScrollViewRect.offsetMax = new Vector2(0, -(UserDataManager.Instance.TopHight - 10));
        }
#endif
#if UNITY_EDITOR || UNITY_IOS
        if (ResolutionAdapter.HasUnSafeArea)
        {
            RectTransform ScrollViewRect = ScrollView.GetComponent<RectTransform>();
            ScrollViewRect.offsetMax = new Vector2(0, -(UserDataManager.Instance.TopHight - 10));
        }
#endif




        alphaMin = 0.5f;
        //UIEventListener.AddOnClickListener(UIMask, backToMainForm);
        //UIEventListener.AddOnClickListener(KeysSelectButton.gameObject, KeySelectButton);
        //UIEventListener.AddOnClickListener(DiamondsSelectButton.gameObject, DiamondSelectButton);

        UIEventListener.AddOnClickListener(dKeysprites, dKeyspritesButtonOn);
        UIEventListener.AddOnClickListener(kDiamondsprites, kDiamondspritesButtonOn);

        UIEventListener.AddOnClickListener(TopDiamanButton, TopDItemOnClicke);
        UIEventListener.AddOnClickListener(TopKiamanButton, TopkItemOnClicke);
        UIEventListener.AddOnClickListener(LiveSupportGo, ClickLiveSupportHandler);

        addMessageListener(EventEnum.GetuserpaymallidStatChack, GetuserpaymallidStatChack);

        UIEventListener.AddOnClickListener(RecommendBottuon, TopgameItemOnClicke);
        if (BuyBtnGo != null)
            UIEventListener.AddOnClickListener(BuyBtnGo.gameObject, BuyGiftHandler);

        mMoneyType = 1;
        mOpenFrom = 0;

        //this.frameTrans.localScale = new Vector3(1, 0, 1);
        //this.frameTrans.DOScaleY(1, 0.25f).SetEase(Ease.OutBack).Play();

        if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data != null)
        {
            //if (FirstGiftGo != null)
            //    FirstGiftGo.SetActive(UserDataManager.Instance.userInfo.data.userinfo.newpackage_status == 1);
        }

        ChargeMoneyItemScripteList = new List<ChargeMoneyItemScripte>();

        if (!IGGSDKManager.Instance.HasProductList())
        {
            UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218),GameDataMgr.Instance.table.GetLocalizationById(300),AlertType.Sure, (value) =>
            {
                CUIManager.Instance.CloseForm(UIFormName.ChargeMoneyForm);
            });
        }
    }

    private void ClickLiveSupportHandler(PointerEventData eventdata)
    {
        //IGGSDKManager.Instance.TshInit();
        // var bundle = KungfuInstance.Get().GetPreparedURLBundle();
        // bundle.PaymentLivechatURL(HandleOpenURLFromURLBundle);
        IGGSDKManager.Instance.OpenTSH();
    }
    private static void HandleOpenURLFromURLBundle(IGGError error, string url)
    {
        IGGNativeUtils.ShareInstance().OpenBrowser(url);
    }

    private void AndenjoyTextOutLineChange(int type)
    {
        if (AndenjoyText == null) return;
        if (type==1)
        {
            //钥匙
            AndenjoyText.effectColor = new Color(181/255.0f,79/255.0f,233/255.0f);
        }
        else
        {
            //钻石
            AndenjoyText.effectColor = new Color(198 / 255.0f, 71 / 255.0f, 176 / 255.0f);
        }
    }

    /// <summary>
    /// 点击跳转到钥匙购买界面
    /// </summary>
    /// <param name="data"></param>
    private void dKeyspritesButtonOn(PointerEventData data)
    {
        Debug.Log("点击跳转到钥匙购买界面");
        CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop).OpenChargeMoney_Keys(null);
    }

    /// <summary>
    /// 点击跳转到钻石购买界面
    /// </summary>
    /// <param name="data"></param>
    private void kDiamondspritesButtonOn(PointerEventData data)
    {
        Debug.Log("点击跳转到钻石购买界面");
        CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop).OpenChargeMoney_Diamonds(null);
    }
    private void GetuserpaymallidCallback(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetuserpaymallidCallback---->" + result);
        if (result.Equals("error"))
        {
            //ShowstateCallBacke();
            SpwanKeyAndD();
            LOG.Info("----GetuserpaymallidCallback---->返回错误");
            return;
        }

        JsonObject jo = JsonHelper.JsonToJObject(result);

        if (jo.code == 200)
        {
            UserDataManager.Instance.Getuserpaymallid = JsonHelper.JsonToObject<HttpInfoReturn<Getuserpaymallid>>(result);
            //ShowstateCallBacke();
            SpwanKeyAndD();
            //GetuserpaymallidStatChack(null);
        }
    }

    public void GetuserpaymallidStatChack(Notification notfi)
    {
        if (notfi != null&& notfi.Data!=null)
        {
            mMoneyType = (int)notfi.Data;
            //LOG.Info("商品是否处于首充翻倍状态的检测:" + mMoneyType);

            if (mMoneyType==1)
            {
                tmepItemInfo = tmepKItemInfo;
            }else
            {
                tmepItemInfo = tmepKItemInfo;
            }
        }

        if (UserDataManager.Instance.Getuserpaymallid != null)
        {           
            if (UserDataManager.Instance.Getuserpaymallid.data.ids.IndexOf(tmepItemInfo.id) == -1)
            {
                if (mMoneyType == 1)
                {
                    //钥匙
                    if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data.userinfo.is_vip == 1)
                    {
                        //翻倍首充vip
                        TopKNumber.text = "66 Keys";

                        //DiamondMoneyImage.sprite= ResourceManager.Instance.GetUISprite("ChargeMoneyForm/Key_66");
                        //DiamondMoneyImage.SetNativeSize();
                    }
                    else
                    {
                        //翻倍首充不是vip
                        TopKNumber.text = "60 Keys";


                        //DiamondMoneyImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/Key_60");
                        //DiamondMoneyImage.SetNativeSize();

                        if (UserDataManager.Instance.Getuserpaymallid.data.ids.IndexOf(20) != -1)
                        {
                           
                            //如果购买的商品里面，有第一个推荐的钥匙，不是显示双倍的
                            //DiamondMoneyImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/Key_30");
                            //DiamondMoneyImage.SetNativeSize();

                            TopKNumber.text = "30 Keys";

                        }
                    }

                   
                    //priceImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/img_shuzi2");
                    originalPrice.gameObject.SetActive(false);
                }
                else if (mMoneyType == 2)
                {
                    //钻石
                    if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data.userinfo.is_vip == 1)
                    {
                        //翻倍首充vip
                        //numbertext.text = "550 Diamonds";

                        //DiamondMoneyImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/Diamonds_550");
                        //DiamondMoneyImage.SetNativeSize();

                        TopDNumber.text = "550 Diamonds";
                    }
                    else
                    {
                        //翻倍首充不是vip
                        //numbertext.text = "500 Diamonds";

                        //DiamondMoneyImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/Diamonds_500");
                        //DiamondMoneyImage.SetNativeSize();

                        TopDNumber.text = "500 Diamonds";
                        if (UserDataManager.Instance.Getuserpaymallid.data.ids.IndexOf(24) != -1)
                        {
                           
                            //如果购买的商品里面，有第一个推荐的钻石，不是显示双倍的
                            //DiamondMoneyImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/Diamonds_250");
                            //DiamondMoneyImage.SetNativeSize();

                            TopDNumber.text = "250 Diamonds";
                        }
                    }
                    //priceImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/img_shuzi");
                    originalPrice.gameObject.SetActive(false);
                }
                else
                {
                    //票卷
                }
                //priceImage.SetNativeSize();
                //priceImage.gameObject.SetActive(true);
                x2value.SetActive(true);
                //numbertext.gameObject.SetActive(false);
                //LOG.Info("首页这个是商品还没用购买过，显示首充翻倍,商品id:" + tmepItemInfo.id);

            }
            else
            {
                if (mMoneyType == 1)
                {
                    //钥匙
                    if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data.userinfo.is_vip == 1)
                    {
                        //vip
                        //numbertext.text = "33 Key";

                        //DiamondMoneyImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/Key_33");
                        //DiamondMoneyImage.SetNativeSize();

                        TopKNumber.text = "33 Keys";
                    }
                    else
                    {
                        //不是vip
                        //numbertext.text = "30 Key";

                        //DiamondMoneyImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/Key_30");
                        //DiamondMoneyImage.SetNativeSize();

                        TopKNumber.text = "30 Keys";
                    }

                    if (UserDataManager.Instance.Getuserpaymallid.data.ids.IndexOf(20) == -1)
                    {
                        
                        //如果购买的商品里面，没有第一个推荐的钥匙，还是显示双倍的
                        //DiamondMoneyImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/Key_60");
                        //DiamondMoneyImage.SetNativeSize();

                        TopKNumber.text = "60 Keys";
                    }
                    //priceImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/img_shuzi3");
                    originalPrice.gameObject.SetActive(false);
                    originalPrice.text = "25 Keys";
                   
                }
                else if (mMoneyType == 2)
                {
                    //钻石
                    if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data.userinfo.is_vip == 1)
                    {
                        //vip
                        //numbertext.text = "275 Diamonds";

                        //DiamondMoneyImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/Diamonds_275");
                        //DiamondMoneyImage.SetNativeSize();

                        TopDNumber.text = "275 Diamonds";
                    }
                    else
                    {
                        //不是vip
                        //numbertext.text = "250 Diamonds";

                        //DiamondMoneyImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/Diamonds_250");
                        //DiamondMoneyImage.SetNativeSize();

                        TopDNumber.text = "250 Diamonds";
                    }

                    if (UserDataManager.Instance.Getuserpaymallid.data.ids.IndexOf(24) == -1)
                    {
                       
                        //如果购买的商品里面，没有第一个推荐的钻石，还是显示双倍的
                        //DiamondMoneyImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/Diamonds_500");
                        //DiamondMoneyImage.SetNativeSize();

                        TopDNumber.text = "500 Diamonds";
                    }
                    //priceImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/img_shuzi1");
                    originalPrice.gameObject.SetActive(false);
                    originalPrice.text = "200 Diamonds";
                 
                }
                else
                {
                    //票卷
                }
                //priceImage.SetNativeSize();
                //priceImage.gameObject.SetActive(false);
                x2value.SetActive(false);
                //numbertext.gameObject.SetActive(true);
                //LOG.Info("首页这个商品已经购买过了，不显示首充翻倍优惠,商品id:" + tmepItemInfo.id);              
            }
        }
    }

    public override void OnClose()
    {
        if (GameDataMgr.Instance.AutoPlayPause)
        {
            GameDataMgr.Instance.AutoPlayPause = false;
            GameDataMgr.Instance.InAutoPlay = true;
        }
        base.OnClose();

        //UIEventListener.RemoveOnClickListener(KeysSelectButton.gameObject, KeySelectButton);
        //UIEventListener.RemoveOnClickListener(DiamondsSelectButton.gameObject, DiamondSelectButton);

        UIEventListener.RemoveOnClickListener(dKeysprites, dKeyspritesButtonOn);
        UIEventListener.RemoveOnClickListener(kDiamondsprites, kDiamondspritesButtonOn);

        UIEventListener.RemoveOnClickListener(TopDiamanButton, TopDItemOnClicke);
        UIEventListener.RemoveOnClickListener(TopKiamanButton, TopkItemOnClicke);


        UIEventListener.RemoveOnClickListener(RecommendBottuon, TopgameItemOnClicke);
        UIEventListener.RemoveOnClickListener(LiveSupportGo, ClickLiveSupportHandler);
        if (BuyBtnGo != null)
            UIEventListener.RemoveOnClickListener(BuyBtnGo.gameObject, BuyGiftHandler);

        if (mOpenFrom == 1)
        {
            CUIManager.Instance.CloseForm(UIFormName.ChargeTipsForm);
        }

        if (ChargeMoneyItemScripteList!=null)
        {
            for (int i=0;i< ChargeMoneyItemScripteList.Count;i++)
            {
                ChargeMoneyItemScripteList[i].Disposal();
            }

            ChargeMoneyItemScripteList = null;
        }

        EventDispatcher.Dispatch(EventEnum.TopMainFT.ToString());
    }

    private void GetShopListCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetShopListCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.shopList = JsonHelper.JsonToObject<HttpInfoReturn<ShopListCont>>(result);
                    if (UserDataManager.Instance.shopList != null)
                        Showstate(mMoneyType);
                }
                else if (jo.code == 277)
                {
                    UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, jo.msg);
                    return;
                }
            }, null);
        }
    }

    private void ItemClickHandler(PointerEventData data)
    {
        ChargeItemForm itemForm = data.pointerEnter.GetComponent<ChargeItemForm>();
        if (itemForm != null)
        {
            int index = itemForm.Index;
            int buyNum = index * 5;
            if (mMoneyType == 1)
                UserDataManager.Instance.CalculateKeyNum(buyNum);
            else if (mMoneyType == 2)
                UserDataManager.Instance.CalculateDiamondNum(buyNum);
            //else

        }
    }

    private bool firstEnd = false;
    public void SetFormStyle(int vMonetyType, int vOpenFrom = 0)
    {
        mMoneyType = vMonetyType;
        mOpenFrom = vOpenFrom;
       
        if (vMonetyType == 1)
        {
            //Scrollbar.value = 0;
            ScrollView.DOVerticalNormalizedPos(0, 0.5f);
        }
        else if (vMonetyType == 2)
        {
            //Scrollbar.value = 1;
            //Scrollbar.DOGoto(1);
            ScrollView.DOVerticalNormalizedPos(1, 0.5f);
            //DOTween.To(() => Scrollbar.value, (value) => Scrollbar.value = value, 1, 0.5f);
        }

        if (UserDataManager.Instance.shopList == null)
        {
            firstEnd = true;
            GameHttpNet.Instance.GetShopList(GetShopListCallBack);
        }
        else
        {
            if (!firstEnd)
            {
                firstEnd = true;
                Showstate(mMoneyType);

            }
        }

    }


    private void backToMainForm(PointerEventData data)
    {
        //CUIManager.Instance.CloseForm(UIFormName.ChargeMoneyForm);
        //DisplayImageParentScaleChange();
    }

    public void DisplayImageParentScaleChange(bool isplayGameClose)
    {
        this.isPlayGameClose = isplayGameClose;
        //Tween t = frameTrans.DOAnchorPos(new Vector3(0, 1334, 0), 0.8f);
        //t.OnComplete(CloseFormToUi);

        CloseFormToUi();

    }

    private bool isPlayGameClose = false;
    private void CloseFormToUi()
    {
        if (isPlayGameClose)
        {
            isPlayGameClose = false;
            CUIManager.Instance.CloseForm(UIFormName.MainFormTop);
        }
        CUIManager.Instance.CloseForm(UIFormName.ChargeMoneyForm);
    }
    private void KeySelectButton(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        mMoneyType = 1;
        SelectButtonSpriteExchange();

    }
    private void DiamondSelectButton(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        mMoneyType = 2;
        SelectButtonSpriteExchange();
    }

    private void SelectButtonSpriteExchange()
    {

        Showstate(mMoneyType);

        KeyEffect.SetActive(mMoneyType == 1 || mMoneyType == 3);
        DiamondEffect.SetActive(mMoneyType == 2);
    }

    private void DispalyImageDoFade()
    {
        //DispalyImageCanvasGroup = DispalyBgImage.transform.GetComponent<CanvasGroup>();
        //DispalyImageCanvasGroup.alpha = alphaMin;
        frameTrans.DOAnchorPos(new Vector3(0, 0, 0), 0.8f).SetEase(Ease.OutCubic);//-670
    }


    //这个是用来显示选择界面的状态的
    private void Showstate(int vType)
    {
        ScrollViewKey.SetActive(vType == 1);
        ScrollViewDiamond.SetActive(vType == 2);
        ScrollViewTicket.SetActive(vType == 3);
        //SelectBtnGroup.SetActive(vType != 3);

        //DispalyBgImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/BannerBg_0" + vType);
        AndenjoyTextOutLineChange(vType);
        //DiamondMoneyImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/BannerMoney_0" + vType);
        //DiamondMoneyImage.SetNativeSize();
        DispalyBgImage.GetComponent<CanvasGroup>().alpha = alphaMin;
        DispalyBgImage.GetComponent<CanvasGroup>().DOFade(1, 0.5f);

        GameHttpNet.Instance.Getuserpaymallid(GetuserpaymallidCallback);
    }

    private void ShowPrice()
    {
        if (mMoneyType == 1)//这个是key界面选中
        {         
            if (UserDataManager.Instance.shopList != null)
            {
                List<ShopItemInfo> keyList = UserDataManager.Instance.shopList.data.key_list;
                if (keyList.Count > 0)
                {
                    int len = keyList.Count;

                    for (int i = 0; i < len; i++)
                    {
                        ShopItemInfo itemInfo = keyList[i];
                      
                        if (itemInfo != null&& keyList[i].is_recommend == 1)
                        {
                            //这个商品与推荐的商品相符合，记录下
                            RecommendGame = RecommendGamekey;
                            tmepItemInfo = itemInfo;
                        }
                    }
                }
            }

            if (tmepItemInfo == null) { return; }
 
            prices.text = "$" + tmepItemInfo.price;

        }
        else if (mMoneyType == 2)  //这个是钻石选中界面
        {
          
            if (UserDataManager.Instance.shopList != null)
            {
                List<ShopItemInfo> diamondList = ReSortList(UserDataManager.Instance.shopList.data.diamond_list);

                if (diamondList != null)
                {
                    int len = diamondList.Count;
                    for (int i = 0; i < len; i++)
                    {
                        ShopItemInfo itemInfo = diamondList[i];
                        if (itemInfo != null&& diamondList[i].is_recommend == 1)
                        {
                            //这个商品与推荐的商品相符合，记录下
                            RecommendGame = RecommendGameDiam;
                            tmepItemInfo = itemInfo;
                        }
                    }
                }
            }

            if (tmepItemInfo == null) { return; }          
            prices.text = "$" + tmepItemInfo.price;          
        }
    }
    private void ShowstateCallBacke()
    {

        tmepItemInfo = null;

        
        if (mMoneyType == 1)//这个是key界面选中
        {
           
            if (UserDataManager.Instance.shopList != null)
            {
                List<ShopItemInfo> keyList = UserDataManager.Instance.shopList.data.key_list;
                if (keyList.Count >0)
                {
                    int len = keyList.Count;


                    for (int i = 0; i < len; i++)
                    {
                        ShopItemInfo itemInfo = keyList[i];

                        if (itemInfo != null)
                        {
                            ChargeMoneyItemScripte item = ResourceManager.Instance.LoadAssetBundleUI(UIFormName.ChargeMoneyKeyItem).GetComponent<ChargeMoneyItemScripte>();
                            item.transform.SetParent(ScrollViewKeyContent.transform, false);
                            item.rectTransform().localPosition = Vector3.zero;
                            item.rectTransform().localScale = Vector3.one;
                            item.SetGameInit(itemInfo, i + 1, mMoneyType);

                            if (keyList[i].is_recommend == 1)
                            {
                                //这个商品与推荐的商品相符合，记录下
                                RecommendGame = item;
                                RecommendGamekey = item;
                                tmepItemInfo = itemInfo;
                            }
                            ChargeMoneyItemScripteList.Add(item);
                        }
                    }
                }
            }

            if (tmepItemInfo == null) { return; }

            //BonusText.text = tmepItemInfo.DiscountName;
            //numbertext.text = tmepItemInfo.count + " Keys";
            //nametext.text = "Keys";
            prices.text = "$" + tmepItemInfo.price;
            dwonbgText.text = "Get the best deal of Keys right now!";
        }
        else if (mMoneyType == 2)  //这个是钻石选中界面
        {
           
            if (UserDataManager.Instance.shopList != null)
            {
                List<ShopItemInfo> diamondList = ReSortList(UserDataManager.Instance.shopList.data.diamond_list);

                if (diamondList != null)
                {
                    int len = diamondList.Count;
                    for (int i = 0; i < len; i++)
                    {
                        ShopItemInfo itemInfo = diamondList[i];
                        if (itemInfo != null)
                        {
                            ChargeMoneyItemScripte item = ResourceManager.Instance.LoadAssetBundleUI(UIFormName.ChargeMoneyDiamondItem).GetComponent<ChargeMoneyItemScripte>();
                            item.transform.SetParent(ScrollViewDiamondContent.transform, false);
                            item.rectTransform().localPosition = Vector3.zero;
                            item.rectTransform().localScale = Vector3.one;
                            item.SetGameInit(itemInfo, i + 1, mMoneyType);

                            if (diamondList[i].is_recommend == 1)
                            {
                                //这个商品与推荐的商品相符合，记录下
                                RecommendGame = item;
                                RecommendGameDiam = item;
                                tmepItemInfo = itemInfo;
                            }

                            ChargeMoneyItemScripteList.Add(item);
                        }
                    }
                }
            }

            if (tmepItemInfo == null) { return; }

            //BonusText.text = tmepItemInfo.DiscountName;
            //numbertext.text = tmepItemInfo.count + " Diamonds";
            //nametext.text = "Diamonds";
            prices.text = "$" + tmepItemInfo.price;
            dwonbgText.text = "Get the best deal of Diamonds right now!";
        }
    }

    /// <summary>
    /// 这个是顶端商品推荐的商品点击
    /// </summary>
    /// <param name="data"></param>
    public void TopgameItemOnClicke(PointerEventData data)
    {
        //推荐商品点击购买
        if (RecommendGame != null)
        {
            RecommendGame.BuyItem(null);
        }
    }

    private List<ShopItemInfo> ReSortList(List<ShopItemInfo> vList)
    {
        List<ShopItemInfo> freeList = new List<ShopItemInfo>();
        List<ShopItemInfo> noFreeList = new List<ShopItemInfo>();

        if (vList != null)
        {
            int len = vList.Count;
            for (int i = 0; i < len; i++)
            {
                ShopItemInfo itemInfo = vList[i];
                if (itemInfo != null)
                {
                    noFreeList.Add(itemInfo);
                }
            }
            len = noFreeList.Count;
            for (int i = 0; i < len; i++)
            {
                freeList.Add(noFreeList[i]);
            }
        }

        return freeList;
    }

    private string mGiftName = "com.onyx.package1";
    private ShopItemInfo packageInfo;
    private void BuyGiftHandler(PointerEventData data)
    {
        if (UserDataManager.Instance.shopList != null && UserDataManager.Instance.shopList.data != null
            && UserDataManager.Instance.shopList.data.package_list != null
            && UserDataManager.Instance.shopList.data.package_list.Count > 0)
        {
            packageInfo = UserDataManager.Instance.shopList.data.package_list[0];
        }
        if (packageInfo != null)
        {
            //点击充值
            //UINetLoadingMgr.Instance.Show();
            //TalkingDataManager.Instance.GameCharge(packageInfo.productName);
            SdkMgr.Instance.Pay(packageInfo.id, packageInfo.product_id, 1, packageInfo.price, OnPayCallback);
        }
    }
    

    //这个是游戏中打开钻石充值界面的时候，这个界面做设置
    public void GamePlayOpenDiamond()
    {
        //KeysSelectButton.gameObject.SetActive(false);
    }

    public void GamePlayOpenKey()
    {
        //DiamondsSelectButton.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        frameTrans.anchoredPosition = new Vector3(0, 1334, 0);
        DispalyImageDoFade();
    }

    void OnPayCallback(bool isOK, string result)
    {
        if (!isOK)
        {
            return;
        }
        if (UserDataManager.Instance.orderFormSubmitResultInfo != null && UserDataManager.Instance.orderFormSubmitResultInfo.data != null)
        {
            UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.orderFormSubmitResultInfo.data.bkey);
            UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.orderFormSubmitResultInfo.data.diamond);
            UserDataManager.Instance.ResetMoney(3, UserDataManager.Instance.orderFormSubmitResultInfo.data.ticket);
        }

        AudioManager.Instance.PlayTones(AudioTones.RewardWin);
        var Localization = GameDataMgr.Instance.table.GetLocalizationById(148);
        UITipsMgr.Instance.PopupTips(Localization, false);

    }

    #region 新UI功能


    private void SpwanKeyAndD()
    {
        if(frameTrans == null)
        {
            return;
        }
        if (UserDataManager.Instance.shopList != null)
        {
            List<ShopItemInfo> diamondList = ReSortList(UserDataManager.Instance.shopList.data.diamond_list);

            if (diamondList != null)
            {
                int len = diamondList.Count;
                for (int i = 0; i < len; i++)
                {
                    ShopItemInfo itemInfo = diamondList[i];
                    if (itemInfo != null)
                    {
                        ChargeMoneyItemScripte item = ResourceManager.Instance.LoadAssetBundleUI(UIFormName.ChargeMoneyDiamondItem).GetComponent<ChargeMoneyItemScripte>();
                        item.transform.SetParent(DiamanList.transform, false);
                        item.rectTransform().localPosition = Vector3.zero;
                        item.rectTransform().localScale = Vector3.one;
                        item.SetGameInit(itemInfo, i + 1, 2);

                        if (diamondList[i].is_recommend == 1)
                        {
                            //这个商品与推荐的商品相符合，记录下
                            RecommendGameD = item;
                            RecommendGamekey = item;
                            tmepDItemInfo = itemInfo;
                            tmepItemInfo = itemInfo;
                            //TopDPrice.text = "$" + itemInfo.price;
                            if (IGGSDKManager.Instance.HasProductList())
                            {
                                TopDPrice.text = IGGSDKManager.Instance.GetItemPrice(itemInfo.product_id);
                            }
                            else
                            {
                                TopDPrice.text = "$" + itemInfo.price;
                            }

                            mMoneyType = 2;
                            GetuserpaymallidStatChack(null);
                        }

                        ChargeMoneyItemScripteList.Add(item);
                    }
                }
            }
        }

        if (UserDataManager.Instance.shopList != null)
        {
            List<ShopItemInfo> keyList = UserDataManager.Instance.shopList.data.key_list;
            if (keyList.Count > 0)
            {
                int len = keyList.Count;
        
        
                for (int i = 0; i < len; i++)
                {
                    ShopItemInfo itemInfo = keyList[i];
        
                    if (itemInfo != null)
                    {
                        ChargeMoneyItemScripte item = ResourceManager.Instance.LoadAssetBundleUI(UIFormName.ChargeMoneyKeyItem).GetComponent<ChargeMoneyItemScripte>();
                        item.transform.SetParent(KeyList.transform, false);
                        item.rectTransform().localPosition = Vector3.zero;
                        item.rectTransform().localScale = Vector3.one;
                        item.SetGameInit(itemInfo, i + 1, 1);
        
                        if (keyList[i].is_recommend == 1)
                        {
                            //这个商品与推荐的商品相符合，记录下
                            RecommendGameK = item;
                            RecommendGamekey = item;
                            tmepKItemInfo = itemInfo;
                            tmepItemInfo = itemInfo;
                            //TopKPrice.text = "$" + itemInfo.price;
                            if (IGGSDKManager.Instance.HasProductList())
                                                         {
                                                             TopKPrice.text = IGGSDKManager.Instance.GetItemPrice(itemInfo.product_id);
                                                         }
                                                         else
                                                         {
                                                             TopKPrice.text = "$" + itemInfo.price;
                                                         }
        
                            mMoneyType = 1;
                            GetuserpaymallidStatChack(null);
                        }
                        ChargeMoneyItemScripteList.Add(item);
                    }
                }
            }
        }
    }

    private void ShowTopDKnumber()
    {

    }

    private ChargeMoneyItemScripte RecommendGameD, RecommendGameK;
    public void TopDItemOnClicke(PointerEventData data)
    {
        //推荐商品点击购买
        if (RecommendGameD != null)
        {
            RecommendGameD.BuyItem(null);
        }
    }

    public void TopkItemOnClicke(PointerEventData data)
    {
        //推荐商品点击购买
        if (RecommendGameK != null)
        {
            RecommendGameK.BuyItem(null);
        }
    }

    #endregion

}

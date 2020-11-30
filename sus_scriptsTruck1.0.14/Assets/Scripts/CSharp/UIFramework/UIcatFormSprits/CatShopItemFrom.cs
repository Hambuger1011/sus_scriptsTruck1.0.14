using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UGUI;
using System;

public class CatShopItemFrom : MonoBehaviour {

    private Text ItemName,LeftTimeTxt;
    private Image ItemSprite;
    private GameObject BuyButton,LeftTimeGroup;
    private Image TypeSprite;
    private Text price;
    public GameObject SizeL,SizeS;

    private bool First = false;
    private t_shop ShopT;
    private CatShopForm CatShopForm;
    private int ID;
    /// <summary>
    /// 初始化
    /// </summary>
    /// 
    public void Init(t_shop t_shop,CatShopForm CatShopForm,int id)
    {       
        this.CatShopForm = CatShopForm;
        ID = id;

        EventDispatcher.AddMessageListener(EventEnum.SetGuidPos, SetGuidPos);

        //EventDispatcher.AddMessageListener(EventEnum.CatGuidRepair, CatGuidRepair);


        if (!First)
        {
            First = true;

            //只是寻找一次
            ItemName = transform.Find("ItemName").GetComponent<Text>();
            ItemSprite = transform.Find("ItemSprite").GetComponent<Image>();
            BuyButton = transform.Find("BuyButton").gameObject;
            TypeSprite = BuyButton.transform.Find("TypeSprite").GetComponent<Image>();
            price = BuyButton.transform.Find("price").GetComponent<Text>();
            LeftTimeGroup = transform.Find("LeftTimeBg").gameObject;
            LeftTimeTxt = transform.Find("LeftTimeBg/LeftTimeTxt").GetComponent<Text>();

            UIEventListener.AddOnClickListener(BuyButton,BuyButtonOnclicke);
        }
        ShopT = t_shop;
        ItemName.text = t_shop.name.ToString();
        ItemSprite.sprite= ResourceManager.Instance.GetUISprite("CatDecFoodIcon/" + t_shop.res.ToString());
        SizeS.SetActive(false);
        SizeL.SetActive(false);

        ChangeItemShowInfo();

        if (t_shop.love == 0)
        {
            //花费钻石
            TypeSprite.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/icon1");
            price.text = t_shop.diamond.ToString();
            TypeSprite.SetNativeSize();
        }
        else
        {
            //花费爱心
            TypeSprite.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/shop_icon1");
            TypeSprite.SetNativeSize();
            price.text = t_shop.love.ToString();
        }

        SizeS.SetActive(t_shop.size == 1);
        SizeL.SetActive(t_shop.size == 2);

        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.ShopBuyHuangyuandian && ID == 6)
        {
            EventDispatcher.Dispatch(EventEnum.OpenCatGuid);//打开引导界面
        }

        CatGuidRepair(null);
    }

    private void ChangeItemShowInfo()
    {
        if (BuyButton == null) return;
        int robotId = 0;
        if (UserDataManager.Instance.SceneInfo != null && UserDataManager.Instance.SceneInfo.data != null && UserDataManager.Instance.SceneInfo.data.robot != null)
        {
            int tempLen = UserDataManager.Instance.SceneInfo.data.robot.Count;
            for (int i = 0; i < tempLen; i++)
            {
                robotId = UserDataManager.Instance.SceneInfo.data.robot[i].shop_id;
            }
            if (robotId > 0 && ShopT.size == 4)
            {
                BuyButton.SetActive(ShopT.shopid == robotId);
                LeftTimeGroup.SetActive(ShopT.shopid == robotId);
                if(ShopT.shopid == robotId)
                {
                    CatRobotInfo mRobotInfo = null;
                    int len = UserDataManager.Instance.SceneInfo.data.robot.Count;
                    for (int i = 0; i < len; i++)
                    {
                        mRobotInfo = UserDataManager.Instance.SceneInfo.data.robot[i];
                    }
                    if(mRobotInfo != null)
                    {
                        int mRobotEndTime = mRobotInfo.endtime - GameDataMgr.Instance.GetCurrentUTCTime()-1;
                        int day = (mRobotEndTime / 3600) / 24;
                        if (day >= 1)
                            LeftTimeTxt.text = string.Format(GameDataMgr.Instance.table.GetLocalizationById(236), day, (mRobotEndTime / 3600) % 24);
                        else                        
                            LeftTimeTxt.text = string.Format(GameDataMgr.Instance.table.GetLocalizationById(237), mRobotEndTime / 3600, (mRobotEndTime / 60) % 60);
    
                    }
                }
                
            }
            else
            {
                BuyButton.SetActive(true);
                LeftTimeGroup.SetActive(false);
            }
        }else
        {
            BuyButton.SetActive(true);
            LeftTimeGroup.SetActive(false);
        }
    }

    private void BuyButtonOnclicke(PointerEventData data)
    {
        LOG.Info("猫商店按钮点击了");
        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.ShopBuyHuangyuandian)
        {
            EventDispatcher.Dispatch(EventEnum.CatGuidUiClose);//关闭引导界面
        }

        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CUIManager.Instance.OpenForm(UIFormName.CatPublicForm);
        string str = "";
        if (ShopT.size == 1 || ShopT.size == 2)
        {
            str = "Do you want to buy this decoration？";
        }
        else if(ShopT.size == 3)
        {
            str = "Do you want to buy this food？";
        }
        else if (ShopT.size == 4)
        {
            if(UserDataManager.Instance.SceneInfo.data.robot == null || UserDataManager.Instance.SceneInfo.data.robot.Count == 0)
                str = "Do you want to hire a Caretaker?";
            else
                str = "Do you want to renew your Caretaker?";
        }
        else if (ShopT.size == 5)
        {
            str = "Do you want to exchange 70 hearts for one diamond？";
        }

        WindowInfo tmpWin = new WindowInfo(ShopT.name.ToString(), ShopT.describe,str , "Yes",
                           PublicNoButtonCallBacke, PublicYesButtonCallback, 1, null, null, null);
        CUIManager.Instance.GetForm<CatPublicForm>(UIFormName.CatPublicForm).Inite(tmpWin);

        //Set共用界面调用
        //CUIManager.Instance.OpenForm(UIFormName.CatSetForm);
        //CUIManager.Instance.GetForm<CatSetForm>(UIFormName.CatSetForm).Inite(ShopT.name.ToString(), PublicYesButtonCallback);

    }

    private void PublicYesButtonCallback(string ST)
    {
        //Debug.Log("共用界面YES按钮回调");
        //Debug.Log("ID："+ ShopT.shopid);
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.PostpetBuyItem(ShopT.shopid.ToString(), "1", ProcessBuyItem);
       
    }
    /// <summary>
    /// 放置装饰物回调
    /// </summary>
    /// <param name="arg"></param>
    private void ProcessBuyItem(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetpetgiftinfoCallbacke---->" + result);

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.HuangyuandianYesOnclick )
                    {
                        EventDispatcher.Dispatch(EventEnum.CatGuidUiClose);//关闭引导界面
                    }

                    var tmpForm = CUIManager.Instance.GetForm<CatMainForm>(UIFormName.CatMain);
                    if (tmpForm != null)
                    {
                        UserDataManager.Instance.BuyItemResult = JsonHelper.JsonToObject<HttpInfoReturn<BuyItemResult>>(result);

                        
                        //刷新钻石爱心数
                        UserDataManager.Instance.UpdateCatGoodsCount(UserDataManager.Instance.BuyItemResult.data.diamond, UserDataManager.Instance.BuyItemResult.data.love);
                        var tmpTopForm = CUIManager.Instance.GetForm<CatTopMain>(UIFormName.CatTop);
                        string dia = UserDataManager.Instance.SceneInfo.data.usermoney.diamond.ToString();
                        string love = UserDataManager.Instance.SceneInfo.data.usermoney.love.ToString();
                        string food = UserDataManager.Instance.SceneInfo.data.usermoney.otherfood.ToString();
                        tmpTopForm.RefreshDiamondAndHeart(dia, love, "");//钻石，在下面就已经派发更新了，这里就不用更新钻石了，否则就重复了

                        //刷新主界面钻石资源显示
                        UserDataManager.Instance.CatResetMoney(UserDataManager.Instance.SceneInfo.data.usermoney.diamond);

                        //UINetLoadingMgr.Instance.Show();
                        GameHttpNet.Instance.Getpetpackinfo(ProcessGetPackInfo);
                        if (ShopT.size == 1 || ShopT.size == 2)//裝飾物
                        {
                            //EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), (int)CatFormEnum.CAT_SHOP);
                            //tmpForm.catScren.SpawnItemPos(ShopT);
                            UITipsMgr.Instance.PopupTips(jo.msg, false);
                            Invoke("DelayToShowSetTips", 0.5f);
                        }
                        else
                        {
                            //TODO;提示框
                            // CUIManager.Instance.OpenForm(UIFormName.CatSetForm);
                            // CUIManager.Instance.GetForm<CatSetForm>(UIFormName.CatSetForm).Inite("", SetBtnCallBack);
                            UITipsMgr.Instance.PopupTips(jo.msg, false);
                        }
                    }
                }
                else if (jo.code == 204)
                {
                    CUIManager.Instance.OpenForm(UIFormName.ChargeTipsForm);
                    ChargeTipsForm tipForm = CUIManager.Instance.GetForm<ChargeTipsForm>(UIFormName.ChargeTipsForm);

                    //CUIManager.Instance.OpenForm(UIFormName.NewChargeTips);
                    //NewChargeTips tipForm = CUIManager.Instance.GetForm<NewChargeTips>(UIFormName.NewChargeTips);


                    if (tipForm != null)
                    {
                        tipForm.Init(2, ShopT.diamond, 1 * 0.99f);
                        tipForm.CatTopMainChange();
                    }
                }
                else
                {
                    UITipsMgr.Instance.PopupTips(jo.msg, false);
                }
            }, null);
        }
    }

    private void ProcessGetPackInfo(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----ProcessGetPackInfo---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.BagInfoContent = JsonHelper.JsonToObject<HttpInfoReturn<BagInfo>>(result);
                    if (UserDataManager.Instance.BagInfoContent != null)
                    {
                        EventDispatcher.Dispatch(EventEnum.UpdateCatGoodList);
                        if (UserDataManager.Instance.SceneInfo != null && UserDataManager.Instance.SceneInfo.data != null)
                        {
                            UserDataManager.Instance.SceneInfo.data.robot = UserDataManager.Instance.BagInfoContent.data.robot;
                            ChangeItemShowInfo();
                            EventDispatcher.Dispatch(EventEnum.CatRobotInfoUpdate);
                        }
                    }
                }
            }, null);
        }
    }

    private void DelayToShowSetTips()
    {
        CUIManager.Instance.OpenForm(UIFormName.CatPublicForm);
        WindowInfo tmpWin = new WindowInfo(ShopT.name.ToString(), ShopT.describe, "Do you want to place this decoration?", "Set", PublicNoButtonCallBacke, PublicSetYesButtonCallback, 1, "", "", "");
        CUIManager.Instance.GetForm<CatPublicForm>(UIFormName.CatPublicForm).Inite(tmpWin);
    }

    private void PublicSetYesButtonCallback(string vStr)
    {
       
        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), (int)CatFormEnum.CAT_PUBLIC);
        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), (int)CatFormEnum.CAT_SHOP);
        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), (int)CatFormEnum.CAT_DECORATION);
        var tmpForm = CUIManager.Instance.GetForm<CatMainForm>(UIFormName.CatMain);
        CUIManager.Instance.OpenForm(UIFormName.CatMain);
        tmpForm.catSceneView.SpawnItemPos(ShopT);

        
    }

    private void PublicNoButtonCallBacke(string ST)
    {
        //Debug.Log("共用界面No按钮回调");
    }

    /// <summary>
    /// 调用这个方法移除物体，和释放内存
    /// </summary>
    public void Dispose()
    {
        EventDispatcher.RemoveMessageListener(EventEnum.SetGuidPos, SetGuidPos);
        //EventDispatcher.RemoveMessageListener(EventEnum.CatGuidRepair, CatGuidRepair);


        if (First)
        {
            ItemSprite.sprite = null;
            TypeSprite.sprite = null;
            UIEventListener.RemoveOnClickListener(BuyButton, BuyButtonOnclicke);
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// 设置引导点击的位置
    /// </summary>
    /// <param name="notice"></param>
    public void SetGuidPos(Notification notice)
    {
        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.ShopBuyHuangyuandian&&ID==6)
        {
            RectTransform SelRect = transform.GetComponent<RectTransform>();
            RectTransform BuyButtonrect = BuyButton.transform.GetComponent<RectTransform>();

            //Debug.Log("Name:"+ ShopT.name);



            float UIMaskW = CatShopForm.UIMaskW;
            float UIMaskH = CatShopForm.UIMaskH;
            float ScrollViewTop = CatShopForm.ScrollViewTop;
            float SelW = SelRect.anchoredPosition.x;
            float SelH = SelRect.anchoredPosition.y;
            float BuyButtonW = BuyButtonrect.anchoredPosition.x;
            float BuyButtonH = BuyButtonrect.anchoredPosition.y;

            float Posx = SelW+ BuyButtonW;
            float Posy = UIMaskH+ ScrollViewTop+ SelH+ BuyButtonH;

            UserDataManager.Instance.GuidPos = new Vector3(Posx, Posy, 1);
            //LOG.Info("得到食物放置确定按钮的坐标");
        }
    }

    private void CatGuidRepair(Notification notification)
    {
        if (UserDataManager.Instance.HuangyuandianYesOnclickRepair && UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.HuangyuandianYesOnclick && ID == 6)
        {
            BuyButtonOnclicke(null);
        }
    }

}

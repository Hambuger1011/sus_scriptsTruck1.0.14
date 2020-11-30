using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Helpers;
using pb;

public class ChargeMoneyItemScripte : MonoBehaviour
{

    public bool isBestDeal = false;
    public int MoneyType = 1;
    public int Number;
    public float Price;
    public int gameNumber = 0;//改物体的编号
    public string BonusNumber = "";
    public Image ShowImage, bgImage;

    public Text NumberText, PriceText, TopText;
    public GameObject  BuyButton;

    //public GameObject BonusGame;
    public RectTransform rectTransform;


    public GameObject x2value,vipalue;
    public Text priceText,vippriceText,vipbonusPrice;
    public Text BonusPrice;
    public GameObject keyNumber;

    private bool PlayShopSucce = false;
    private string TopName;
    private ShopItemInfo mItemInfo;


    // Use this for initialization
    void Start()
    {
        UIEventListener.AddOnClickListener(BuyButton, BuyItem);
        EventDispatcher.AddMessageListener(EventEnum.GetuserpaymallidStatChack, GetuserpaymallidStatChack);
    }

    private void Property()
    {
        keyNumber.SetActive(true);
        vipalue.SetActive(false);
        x2value.SetActive(false);
        NumberText.text =Number.ToString();

        //TopText.text = TopName;

        if (Price <= 0)
        {
            PriceText.text = "FREE";
        }
        else
        {
            if (IGGSDKManager.Instance.HasProductList())
            {
                PriceText.text = IGGSDKManager.Instance.GetItemPrice(mItemInfo.product_id);
            }
            else
            {
                PriceText.text = "$" + Price;
            }
        }

        if (MoneyType == 1)
        {
            //显示Key的图标
            string ImagePath = "ChargeMoneyForm/bg_icon_" + gameNumber + "_03";
            ShowImage.sprite = ResourceManager.Instance.GetUISprite(ImagePath);

            bgImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/bg_img");
            string Path = "UI/Resident/FX_UI/fx_key_0" + gameNumber;
            SpwanEffect(Path);
        }
        else if (MoneyType == 2)
        {
            string ImagePath = "ChargeMoneyForm/bg_icon" + gameNumber + "_03";
            ShowImage.sprite = ResourceManager.Instance.GetUISprite(ImagePath);

            bgImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/bg_img");
            if (Price <= 0)
            {
                //string bgImageImage = "ChargeMoneyForm/bg_sjoam_03";
                //bgImage.sprite = ResourceManager.Instance.GetUISprite(bgImageImage);
            }

            int PathNum;
            switch (gameNumber)
            {
                case 6:
                    PathNum = 1;
                    break;
                default:
                    PathNum = gameNumber+1;
                    break;
            }
            string Path = "UI/Resident/FX_UI/fx_zuanshi_0"+ PathNum;
            SpwanEffect(Path);

        }
        else if (MoneyType == 3)
        {
            // string ImagePath = "ChargeMoneyForm/tick_icon_0" + gameNumber;
            // ShowImage.sprite = ResourceManager.Instance.GetUISprite(ImagePath);

        }

        if (!string.IsNullOrEmpty(BonusNumber))
        {
            //这个显示优惠
            //BonusGame.SetActive(true);
            //BonusText.text = BonusNumber;
        }
        else
        {
            //BonusGame.SetActive(false);
        }
    }

    /// <summary>
    /// 生成特效
    /// </summary>
    /// <param name="Path"></param>
    private void SpwanEffect(string Path)
    {
        //GameObject go=ResourceManager.Instance.LoadAssetBundleUI(Path);
        //go.transform.SetParent(ShowImage.transform, false);
       
    }


    public void BuyItem(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        
        IGGSDKManager.Instance.PayItem(mItemInfo.product_id);
        
        // if (mItemInfo != null)
        // {
        //     LOG.Info("购买：" + mItemInfo.id + "__价格是：" + mItemInfo.price);
        //     if (mItemInfo.pricetype == 0)    //购买
        //     {
        //         //点击充值
        //         //UINetLoadingMgr.Instance.Show();
        //         //TalkingDataManager.Instance.GameCharge(mItemInfo.productName);
        //         
        //         SdkMgr.Instance.Pay(mItemInfo.id, mItemInfo.productName, 1, mItemInfo.price, OnPayCallback);
        //     }
        //     else if (mItemInfo.pricetype == 1)  //看奖励视频
        //     {
        //         if(UserDataManager.Instance.lotteryDrawInfo != null && UserDataManager.Instance.lotteryDrawInfo.data.shopadcount > 0)
        //         {
        //             TalkingDataManager.Instance.WatchTheAds(1);
        //             //SdkMgr.Instance.admob.ShowRewardBasedVideo(LookVideoComplete);
        //             SdkMgr.Instance.ShowAds(LookVideoComplete);
        //         }
        //         else
        //         {
        //             var Localization = GameDataMgr.Instance.table.GetLocalizationById(150);
        //             UITipsMgr.Instance.PopupTips(Localization, false);
        //
        //             //UITipsMgr.Instance.PopupTips("The number of advertisements has reached the upper limit.", false);
        //         }
        //     }
        // }
    }
    
    
    private void LookVideoComplete(bool value)
    {
        if(value)
            GameHttpNet.Instance.GetAdsReward(1, GetAdsRewardCallBack);
    }

    private void GetAdsRewardCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetOrderToSubmitCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    AudioManager.Instance.PlayTones(AudioTones.RewardWin);
                    UserDataManager.Instance.adsRewardResultInfo = JsonHelper.JsonToObject<HttpInfoReturn<GetAdsRewardResultInfo>>(result);

                    UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.adsRewardResultInfo.data.bkey);
                    UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.adsRewardResultInfo.data.diamond);

                    //Vector3 startPos = new Vector3(-188, -355);
                    //Vector3 targetPos = new Vector3(174, 720);
                    //RewardShowData rewardShowData = new RewardShowData();
                    //rewardShowData.StartPos = startPos;
                    //rewardShowData.TargetPos = targetPos;
                    //rewardShowData.IsInputPos = false;
                    //rewardShowData.KeyNum = UserDataManager.Instance.adsRewardResultInfo.data.bkey;
                    //rewardShowData.DiamondNum = UserDataManager.Instance.adsRewardResultInfo.data.diamond;
                    //rewardShowData.Type = 1;
                    //EventDispatcher.Dispatch(EventEnum.GetRewardShow, rewardShowData);
                    TalkingDataManager.Instance.WatchTheAds(2);

                    if (UserDataManager.Instance.lotteryDrawInfo != null && UserDataManager.Instance.lotteryDrawInfo.data.shopadcount > 0)
                    {
                        UserDataManager.Instance.lotteryDrawInfo.data.shopadcount--;
                    }
                }
                else if (jo.code == 202)
                {
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(135);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("You've reached the limit for today!", false);
                }
                else if (jo.code == 208)
                {
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                }
            }, null);
        }
    }

    //void OnDisable()
    //{
    //    UIEventListener.RemoveOnClickListener(BuyButton, BuyItem);
    //    EventDispatcher.RemoveMessageListener(EventEnum.GetuserpaymallidStatChack, GetuserpaymallidStatChack);
    //    Destroy(gameObject);
    //}

    /// <summary>
    /// 清理物体
    /// </summary>
    public void Disposal()
    {
        ShowImage.sprite = null;
        bgImage.sprite = null;

        UIEventListener.RemoveOnClickListener(BuyButton, BuyItem);
        EventDispatcher.RemoveMessageListener(EventEnum.GetuserpaymallidStatChack, GetuserpaymallidStatChack);
        Destroy(gameObject);
    }

    void MoveToPosit()
    {

        rectTransform.DOAnchorPosX(0, 0.5f).SetEase(Ease.OutCubic);
        CancelInvoke();
    }

    /// <summary>
    /// 显示商品是否为首充翻倍的状态商品
    /// </summary>
    private void Change2ValueType()
    {
        if (UserDataManager.Instance.Getuserpaymallid != null)
        {
            GetuserpaymallidStatChack(null);
        }
        else
        {
            SdkMgr.Instance.GetOwnProductIDS();
        }
    }
    public void GetuserpaymallidStatChack(Notification notfi)
    {
        
        if (UserDataManager.Instance.Getuserpaymallid != null)
        {

            int _itemId = int.Parse(mItemInfo.product_id);

            if (UserDataManager.Instance.Getuserpaymallid.data.ids.IndexOf(_itemId) == -1)
            {
                if (MoneyType == 1 || MoneyType == 2)
                {
                    //key和D 是首充翻倍状态

                    float prict =float.Parse(mItemInfo.price);
                    if (prict>0)
                    {                       
                        if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data.userinfo.is_vip == 1)
                        {
                            //这个是首充下的VIP
                            if (UserDataManager.Instance.Getvipcard != null)
                            {
                                vipalue.SetActive(false);
                                x2value.SetActive(true);
                                keyNumber.SetActive(false);
                                priceText.text =mItemInfo.total_count.ToString();

                                //这个是商品加成比例
                                float rat = UserDataManager.Instance.Getvipcard.data.vipinfo.add_rate / 100.0f;
                                BonusPrice.text =(int) (2 * mItemInfo.total_count +  2*mItemInfo.total_count * rat) + "";

                            }
                            else
                            {
                                //UINetLoadingMgr.Instance.Show();
                                GameHttpNet.Instance.Getvipcard(VIPCallBacke);
                            }                          
                        }
                        else
                        {
                            //没有vip的首充
                            vipalue.SetActive(false);
                            x2value.SetActive(true);
                            keyNumber.SetActive(false);
                            priceText.text =mItemInfo.total_count.ToString();
                            BonusPrice.text =2 * mItemInfo.total_count + "";
                        }                      
                    }else
                    {
                        x2value.SetActive(false);
                        keyNumber.SetActive(true);
                        vipalue.SetActive(false);
                    }
                   
                }
                else
                {
                    //票卷
                }            
            }
            else
            {
                //不是首充翻倍
                if (MoneyType == 1 || MoneyType == 2)
                {
                    //key和D
                    if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data.userinfo.is_vip == 1)
                    {
                        //这个是VIP下的非首充
                        if (UserDataManager.Instance.Getvipcard != null)
                        {
                            vipalue.SetActive(true);
                            x2value.SetActive(false);
                            keyNumber.SetActive(false);
                          
                            //这个是商品加成比例
                            float rat = UserDataManager.Instance.Getvipcard.data.vipinfo.add_rate / 100.0f;
                            
                            vippriceText.text =Number.ToString();
                            vipbonusPrice.text = (int)(Number + Number * rat) + "";
                        }
                        else
                        {
                            //UINetLoadingMgr.Instance.Show();
                            GameHttpNet.Instance.Getvipcard(VIPCallBacke);
                        }
                    }
                    else
                    {
                        //没有vip
                        vipalue.SetActive(false);
                        x2value.SetActive(false);
                        keyNumber.SetActive(true);
                        NumberText.text =Number.ToString();
                    }
                }
                else
                {
                    //票卷
                }
                //LOG.Info("这个商品已经购买过了，不显示首充翻倍优惠,商品id:" + mItemInfo.id);     
            }
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
            //UINetLoadingMgr.Instance.Close();
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo != null)
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.Getvipcard = JsonHelper.JsonToObject<HttpInfoReturn<Getvipcard<vipinfo>>>(arg.ToString());

                    if (BonusPrice==null)
                    {
                        return;
                    }
                    GetuserpaymallidStatChack(null);
                }
            }
        }, null);
    }
    #endregion
    public void SetGameInit(ShopItemInfo vItemInfo, int vIndex, int vMoneyType)
    {
        mItemInfo = vItemInfo;
        MoneyType = vMoneyType;//设置这个物体的类别
        Number = vItemInfo.total_count;
        Price = float.Parse(vItemInfo.price);
        gameNumber = vItemInfo.icon_id;
        BonusNumber = vItemInfo.discount_desc;//优惠幅度
        TopName = vItemInfo.product_name;
        isBestDeal = false;

        rectTransform.anchoredPosition = new Vector3(800, 0, 0);
        Invoke("MoveToPosit", vIndex * 0.1f);
        Property();
        Change2ValueType();
    }

}

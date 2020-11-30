using System;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.UI;

public class CatAnimalGiftItem : MonoBehaviour {
    private Text NumberText, NameText, TimeText;
    private Image AnimalItemSprite;
    private Image DecorationSprite,Icon;
    private Button GetButton;
    private bool isFirs = false;
    private int id = -1;
    private int number;//记录这条数据的编号
    private CatGiftFromAnimalFrom CatGiftFromAnimalFrom;
    private Text AcceptText;
    void Awake()
    {
    }
    public void FalseInite()
    {
        gameObject.SetActive(false);
    }
    public void Init(feedback gift, int index, int number, CatGiftFromAnimalFrom CatGiftFromAnimalFrom)
    {
        this.number = number;
        this.CatGiftFromAnimalFrom = CatGiftFromAnimalFrom;
        if (!isFirs)
        {
            isFirs = true;
            GetButton = transform.Find("AcceptBg").GetComponent<Button>();
            AcceptText = transform.Find("AcceptBg/AcceptText").GetComponent<Text>();
            AcceptText.text = GameDataMgr.Instance.table.GetLocalizationById(207);

            NumberText = transform.Find("HardBg/HardNumber").GetComponent<Text>();
            NameText = transform.Find("NameText").GetComponent<Text>();
            TimeText = transform.Find("TimeBg/TimeText").GetComponent<Text>();
            AnimalItemSprite = transform.Find("AnimalBg/AnimalSprite").GetComponent<Image>();
            DecorationSprite = transform.Find("DecorationBg/DecorationSprite").GetComponent<Image>();
            Icon = transform.Find("HardBg/Hard").GetComponent<Image>();
            this.GetButton.onClick.AddListener(OnOkClick);
           
        }
        id = gift.id;
        pb.t_shop tmp = GameDataMgr.Instance.table.GetcatShopId(gift.shop_id);
        string tmpstr;
        if (tmp == null)
        {
            tmpstr = "bg_img4";

        }
        else
        {
            tmpstr = tmp.res;
        }
        if (gift.love_qty == 0)
        {
            Icon.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/icon1");//TDOD
            Icon.SetNativeSize();
            NumberText.text = gift.diamond_qty.ToString();
        }
        else
        {
            Icon.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/icon");//TDOD
            Icon.SetNativeSize();
            NumberText.text = gift.love_qty.ToString();
        }
        AnimalItemSprite.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/cat" + gift.pid);//TDOD
        DecorationSprite.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/" + tmpstr);

        
        NameText.text = gift.pet_name.ToString();
        TimeText.text = gift.plan_time.ToString();
        SetBtnStatus(gift.isprice);


      
        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.GetGiftGuidYes)
        {
            //打开模拟
            EventDispatcher.Dispatch(EventEnum.OpenCatGuid);
        }
    }

    public void OnOkClick()
    {
        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.GetGiftGuidYes)
        {
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.AchieveguidePrice(GiftGetGuid);
        }
        else
        {
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.PostGetpetGift(id, 0, 1, GetpetGiftCallback);
        }          
    }

    private void GetpetGiftCallback(object arg)
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
                    UserDataManager.Instance.usermoney = JsonHelper.JsonToObject<HttpInfoReturn<usermoney>>(result);
                    UserDataManager.Instance.UpdateCatGoodsCount(UserDataManager.Instance.usermoney.data.diamond, UserDataManager.Instance.usermoney.data.love);
                    var tmpForm = CUIManager.Instance.GetForm<CatTopMain>(UIFormName.CatTop);
                    if (tmpForm != null)
                    {
                        //string dia = UserDataManager.Instance.SceneInfo.data.usermoney.diamond.ToString();
                        //string love = UserDataManager.Instance.SceneInfo.data.usermoney.love.ToString();
                        //string food = UserDataManager.Instance.SceneInfo.data.usermoney.otherfood.ToString();
                        //tmpForm.RefreshDiamondAndHeart(dia, love, food);

                        tmpForm.RefreshDiamond(1, UserDataManager.Instance.SceneInfo.data.usermoney.love);
                        tmpForm.RefreshDiamond(2, UserDataManager.Instance.SceneInfo.data.usermoney.diamond);
                        UserDataManager.Instance.CatResetMoney(UserDataManager.Instance.SceneInfo.data.usermoney.diamond);
                    }
                    SetBtnStatus(1);

                    //派发，统计回赠数量
                    EventDispatcher.Dispatch(EventEnum.OnGiftReturnNumberStatistics, 1);


                    CatGiftFromAnimalFrom.RemoveGame(this.gameObject);
                }
                else
                {
                    UITipsMgr.Instance.PopupTips(jo.msg, false);
                }
                                    
                
            }, null);
        }

    }

    /// <summary>
    /// 猫的新手引导中领取礼物
    /// </summary>
    private void GiftGetGuid(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GiftGetGuid---->" + result);

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.AchieveguidePrice = JsonHelper.JsonToObject<HttpInfoReturn<AchieveguidePrice>>(result);
                    UserDataManager.Instance.UpdateCatGoodsCount(UserDataManager.Instance.AchieveguidePrice.data.diamond, UserDataManager.Instance.AchieveguidePrice.data.love);
                    var tmpForm = CUIManager.Instance.GetForm<CatTopMain>(UIFormName.CatTop);
                    if (tmpForm != null)
                    {
                      
                        tmpForm.RefreshDiamond(1, UserDataManager.Instance.AchieveguidePrice.data.love);
                        tmpForm.RefreshDiamond(2, UserDataManager.Instance.AchieveguidePrice.data.diamond);
                        
                    }
                    SetBtnStatus(1);
                    CatGiftFromAnimalFrom.RemoveGame(this.gameObject);


                    //派发，统计回赠数量
                    EventDispatcher.Dispatch(EventEnum.OnGiftReturnNumberStatistics, 1);


                    if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.GetGiftGuidYes)
                    {
                        //关闭引导界面
                        EventDispatcher.Dispatch(EventEnum.CatGuidUiClose);
                    }
                }
                else
                {
                    UITipsMgr.Instance.PopupTips(jo.msg, false);
                }


            }, null);
        }
    }

    public void SetBtnStatus(int status)
    {
        //this.GetButton.gameObject.SetActive(status != 1);
    }

    public void Dispose()
    {
      
        AnimalItemSprite.sprite = null;
        DecorationSprite.sprite = null;
        this.GetButton.onClick.RemoveListener(OnOkClick);
        Destroy(gameObject);
    }

  
}

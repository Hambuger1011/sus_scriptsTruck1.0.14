using pb;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CatDiamondExchange : CatBaseForm
{
    private GameObject Mask, LessButton,AddButton,exchangeButton;
    private Text HeardNumber, DiamonNumber, needHeardNumber;
    private int exchangeDiamondNum = 0, MaxDia = 0;

    private void Awake()
    {
        Mask = transform.Find("Canvas/Mask").gameObject;
        HeardNumber = transform.Find("Canvas/BG/AllHeard/HeardNumber").GetComponent<Text>();
        DiamonNumber = transform.Find("Canvas/BG/exchangeText/DiamonNumber").GetComponent<Text>();
        needHeardNumber= transform.Find("Canvas/BG/needText/needHeardNumber").GetComponent<Text>();
        LessButton = transform.Find("Canvas/BG/LessButton").gameObject;
        AddButton = transform.Find("Canvas/BG/AddButton").gameObject;
        exchangeButton = transform.Find("Canvas/BG/exchangeButton").gameObject;
    }
    public override void OnOpen()
    {
        base.OnOpen();
        UiFormIndex = (int)CatFormEnum.CAT_DIAMONDEXCHANGE;

        UIEventListener.AddOnItemClickListener(Mask, CloseUi);
        UIEventListener.AddOnClickListener(LessButton, LessButtonOn);
        UIEventListener.AddOnClickListener(AddButton,AddButtonOn);
        UIEventListener.AddOnItemClickListener(exchangeButton,ExchangeButton);


        HeardNumber.text ="Available: "+ UserDataManager.Instance.SceneInfo.data.usermoney.love.ToString();

        exchangeDiamondNum = UserDataManager.Instance.SceneInfo.data.usermoney.love/70;
        MaxDia = exchangeDiamondNum;
        DiamonNumber.text = exchangeDiamondNum.ToString();
        needHeardNumber.text = "Needed: " + exchangeDiamondNum*70+"";
        
    }


    public override void OnClose()
    {
        base.OnClose();

        UIEventListener.RemoveOnClickListener(Mask, CloseUi);
        UIEventListener.RemoveOnClickListener(LessButton, LessButtonOn);
        UIEventListener.RemoveOnClickListener(AddButton, AddButtonOn);
        UIEventListener.RemoveOnClickListener(exchangeButton, ExchangeButton);
    }


    private void CloseUi(PointerEventData data)
    {
        //派发事件，关闭这个界面
        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), UiFormIndex);
    }

    private void LessButtonOn(PointerEventData data)
    {
        exchangeDiamondNum--;
        if (exchangeDiamondNum <= 0)
        {
            exchangeDiamondNum = 0;

            //var Localization = GameDataMgr.Instance.table.GetLocalizationById(250);
            //UITipsMgr.Instance.PopupTips(Localization, false);

        }

        DiamonNumber.text = exchangeDiamondNum.ToString();
        needHeardNumber.text = "Needed: " + exchangeDiamondNum * 70 + "";
    }
    private void AddButtonOn(PointerEventData data)
    {
        exchangeDiamondNum++;

        if (exchangeDiamondNum>=MaxDia)
        {
            exchangeDiamondNum = MaxDia;

            //var Localization = GameDataMgr.Instance.table.GetLocalizationById(249);
            //UITipsMgr.Instance.PopupTips(Localization, false);

        }
        DiamonNumber.text = exchangeDiamondNum.ToString();
        needHeardNumber.text = "Needed: " + exchangeDiamondNum *70+"";
    }
    private void ExchangeButton(PointerEventData data)
    {
        GameHttpNet.Instance.PostpetBuyItem("43", exchangeDiamondNum.ToString(), ProcessBuyItem);

    }

    /// <summary>
    /// 兑换钻石回调
    /// </summary>
    /// <param name="arg"></param>
    private void ProcessBuyItem(object arg)
    {
        if (exchangeDiamondNum <= 0)
        {
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(112);
            UITipsMgr.Instance.PopupTips(Localization, false);
            return;
        }

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
                    var tmpTopForm = CUIManager.Instance.GetForm<CatTopMain>(UIFormName.CatTop);
                    if (tmpTopForm != null)
                    {
                        UserDataManager.Instance.BuyItemResult = JsonHelper.JsonToObject<HttpInfoReturn<BuyItemResult>>(result);


                        //刷新钻石爱心数
                        UserDataManager.Instance.UpdateCatGoodsCount(UserDataManager.Instance.BuyItemResult.data.diamond, UserDataManager.Instance.BuyItemResult.data.love);
                       
                        string dia = UserDataManager.Instance.SceneInfo.data.usermoney.diamond.ToString();
                        string love = UserDataManager.Instance.SceneInfo.data.usermoney.love.ToString();
                        string food = UserDataManager.Instance.SceneInfo.data.usermoney.otherfood.ToString();
                        tmpTopForm.RefreshDiamondAndHeart(dia, love, "");//钻石，在下面就已经派发更新了，这里就不用更新钻石了，否则就重复了

                        //刷新主界面钻石资源显示
                        UserDataManager.Instance.CatResetMoney(UserDataManager.Instance.SceneInfo.data.usermoney.diamond);

                        CloseUi(null);
                    }
                }              
                else
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(112);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                }
            }, null);
        }
    }

}

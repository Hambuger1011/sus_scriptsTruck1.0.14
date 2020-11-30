using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using pb;

public class DayActivtyItem : MonoBehaviour
{

    public Text TopNumberText, ActivtyNameText, ActivtyCountText, StartNumAddText, ProssText;
    public Text CollectText;
    public Image TypeImage, Pross;
    public Image CollectButton;
    private RectTransform RewardEffect, keypos, dimpos;

    private taskarr taskarr;
    private RectTransform rect;


    private int type;
    public void DayActivtyItemInit(taskarr taskar, RectTransform RewardEffect, RectTransform keypos, RectTransform dimpos)
    {
        this.RewardEffect = RewardEffect;
        this.keypos = keypos;
        this.dimpos = dimpos;

        rect = gameObject.GetComponent<RectTransform>();


        taskarr = taskar;
        UIEventListener.AddOnClickListener(CollectButton.gameObject, CollectButtonOnclicke);

        if (taskar.bkey_qty > 0 && taskar.diamond_qty > 0)
        {
            type = 0;
            LOG.Info("该任务钻石和钥匙数目都大于0");
        }
        else if (taskar.bkey_qty > 0)
        {
            type = 1;
            //这个是钥匙类型
            TopNumberText.text =taskar.bkey_qty+ "        key";

            TypeImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/icon_3");
        }
        else if (taskar.diamond_qty > 0)
        {
            type = 2;
            //这个是钻石类型
            TopNumberText.text =taskar.diamond_qty+ "        diamonds";
            TypeImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/icon_1");
        }
        CollectButtonSprit();

        ActivtyNameText.text = taskar.task_name;
        ActivtyCountText.text = taskar.remark;
        StartNumAddText.text ="+"+ taskar.star_qty.ToString();
        ProssText.text = taskar.finished_times + "/" + taskar.gold;

        Pross.fillAmount = taskar.finished_times * 1.0f / taskar.gold;
    }

    private void CollectButtonSprit()
    {

        if (taskarr.task_status == 0)
        {
            //任务已经完成
            CollectButton.sprite = ResourceManager.Instance.GetUISprite("Notice/btn_focus");
            CollectText.text = "COLLECT";

        }
        else if (taskarr.task_status == 1)
        {
            //任务未完成
            CollectButton.sprite = ResourceManager.Instance.GetUISprite("Notice/btn_focus1");
            CollectText.text = "COLLECT";
        }
        else
        {
            //已经领取过了
            CollectText.text = "COMPLETED";
            CollectButton.sprite = ResourceManager.Instance.GetUISprite("Notice/btn_focus1");
        }
    }
    private void CollectButtonOnclicke(PointerEventData data)
    {


        //LOG.Info("每日任务被点击了");
        if (taskarr.task_status == 0)
        {
            //任务已经完成了
            UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, GameDataMgr.Instance.table.GetLocalizationById(235) /*"You can now watch a video to collect 2X Rewards."*/, AlertType.SureOrCancel, (value) =>
            {
                //UINetLoadingMgr.Instance.Show();
                if (value)
                {
                    GameHttpNet.Instance.SetDoubleRewardDayTask(taskarr.id, SetDoubleRewardDayTaskHandler); //设置广告双倍，顺便看广告
                }
                else
                {
                    GameHttpNet.Instance.Achievetaskprice(taskarr.id, AchievetaskpriceCallBacke);   //直接领奖励，不翻倍
                }
            }, "Watch AD");
        }
        else if (taskarr.task_status == 1)
        {
            //任务还没有完成

        }
        else
        {
            //任务已经领取了

        }
    }


    private void SetDoubleRewardDayTaskHandler(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----AchievetaskpriceCallBacke---->" + result);
        if (result.Equals("error"))
        {
            LOG.Info("---AchievetaskpriceCallBacke--这条协议错误");
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
                    SdkMgr.Instance.ShowAds(ShowAdsCompleteHandler);
                }
                else if (jo.code == 202)
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(155);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Task unfinished.", false);
                }
                else if (jo.code == 203)
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(156);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Reward already collected before.", false);
                }
                else if (jo.code == 208)
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(157);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Error when setting this task to give double rewards.", false);
                }
            }
        }, null);
    }

    private void ShowAdsCompleteHandler(bool value)
    {
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.Achievetaskprice(taskarr.id, AchievetaskpriceCallBacke);
    }

    private void AchievetaskpriceCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----AchievetaskpriceCallBacke---->" + result);
        if (result.Equals("error"))
        {
            LOG.Info("---AchievetaskpriceCallBacke--这条协议错误");
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
                    UserDataManager.Instance.Achievetaskprice = JsonHelper.JsonToObject<HttpInfoReturn<Achievetaskprice>>(arg.ToString());

                    UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.Achievetaskprice.data.bkey);
                    UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.Achievetaskprice.data.diamond);

                    taskarr.task_status = 2;

                    CollectText.text = "Completed";
                    CollectButtonSprit();
                    HeightToFalse();

                    EventDispatcher.Dispatch(EventEnum.DayActivtyItemReviec, taskarr.star_qty);
                }
            }
        }, null);
    }

    private void HeightToFalse()
    {
        //float times = 0.5f;
        //float Height = rect.rect.height;      
        //DOTween.To(() => Height, (value) => { rect.sizeDelta = new Vector2(rect.rect.width, value); },0, times);
        //rect.DOScaleY(0, times).OnComplete(()=> {
        //    gameObject.SetActive(false);
        //});

        int count = transform.parent.childCount;
        transform.SetSiblingIndex(count - 1);

    }
    
    /// <summary>
    /// 这个是删除物体释放资源
    /// </summary>
    public void DisPoste()
    {
        TypeImage.sprite = null;
        Pross.sprite = null;
        CollectButton.sprite = null;

        UIEventListener.RemoveOnClickListener(CollectButton.gameObject, CollectButtonOnclicke);

        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using pb;

public class ADIteam : MonoBehaviour
{
    private OfertesCtrl OfertesCtrl;
    private ADSInfo ADSInfo;
    private int Number;       //记录这个广告的编号
    private GameObject last, activation, future;
    private GameObject DtypeImage, ktypeImage;
    private Text NumberTxt, activationText;
    private bool IsCanOnclicke = false;
    public void Init(ADSInfo ADSInfo,OfertesCtrl OfertesCtrl)
    {
        this.ADSInfo = ADSInfo;
        Number = ADSInfo.Number;
        this.OfertesCtrl = OfertesCtrl;

        if (ADSInfo.is_receive == 1)
        {
            //记录已经领取到的最新广告的编号
            OfertesCtrl.OfertesDB.HadReadADSNumber = Number;
        }
        

        FindGameObject();
        ShowStep();
        ShowIU();
        BingButton();
    }

    private void FindGameObject()
    {
        last = transform.Find("last").gameObject;
        activation = transform.Find("activation").gameObject;
        activationText = transform.Find("activation/Button/Text").GetComponent<Text>();
        future = transform.Find("future").gameObject;
        DtypeImage = transform.Find("DtypeImage").gameObject;
        ktypeImage = transform.Find("ktypeImage").gameObject;
        NumberTxt = transform.Find("Number").GetComponent<Text>();
    }
    private void BingButton()
    {
        UIEventListener.AddOnClickListener(gameObject, AdsButtonOnclicke);
    }

    /// <summary>
    /// 这里显示的是广告的状态
    /// </summary>
    public void ShowStep()
    {
        last.SetActive(false);
        activation.SetActive(false);
        future.SetActive(false);
        IsCanOnclicke = false;
        int HadReadNumber = OfertesCtrl.OfertesDB.HadReadADSNumber + 1;
        if (HadReadNumber== Number)
        {
            //当前这个广告可以领取
            activation.SetActive(true);
            IsCanOnclicke = true;
        }
        else if (HadReadNumber < Number)
        {
            future.SetActive(true);
        }else
        {
            last.SetActive(true);
        }

        if (Number==1)
        {
            activationText.text = CTextManager.Instance.GetText(289);
        }
    }

    private void ShowIU()
    {
        DtypeImage.SetActive(false);
        ktypeImage.SetActive(false);

        if (ADSInfo.bkey>0)
        {
            ktypeImage.SetActive(true);
            NumberTxt.text = "X" + ADSInfo.bkey;
        }
        else if (ADSInfo.diamond>0)
        {
            DtypeImage.SetActive(true);
            NumberTxt.text = "X" + ADSInfo.diamond;
        }
    }

    private void AdsButtonOnclicke(PointerEventData data)
    {
        if (IsCanOnclicke)
        {
            IsCanOnclicke = false;
            LOG.Info("广告按钮点击了");

            if (Number==1)
            {
                GameHttpNet.Instance.GetAdsReward(3, Number, GetAdsRewardCallBack);
            }
            else
            {
                SdkMgr.Instance.ShowAds(LookVideoComplete);
            }

          
        }
        else
        {
            LOG.Info("这个按钮的点击事件未开放");
        }
    }

    private void LookVideoComplete(bool isOK)
    {
        IsCanOnclicke = true;
        if (isOK)
        {
            GameHttpNet.Instance.GetAdsReward(3, Number, GetAdsRewardCallBack);
        }
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
                    TalkingDataManager.Instance.WatchTheAds(3);

                    OfertesCtrl.OfertesDB.HadReadADSNumber = Number;//记录这个广告已经看过了
                    OfertesCtrl.OfertesView.ShowADSItemStep();
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

    public void DestroyGameObject()
    {
        UIEventListener.RemoveOnClickListener(gameObject, AdsButtonOnclicke);
        if (gameObject!=null)
        {
            Destroy(gameObject);
        }
    }
}

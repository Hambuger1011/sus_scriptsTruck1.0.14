using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Helpers;
using pb;

public class ChargeMoneyFreeItemScripte : MonoBehaviour
{
    public Image ShowImage, bgImage;
    public Text ButtonText;
    public Text DiamondText;
    public GameObject  CollectButton;
    public RectTransform rectTransform;
    public GameObject Free;
    private bool PlayShopSucce = false;
    private ShopItemInfo mItemInfo;
    private int Timecount = 0;
    private bool CanReceive = false;

    void Start()
    {
        UIEventListener.AddOnClickListener(CollectButton, BuyItem);
    }
    public void Counter()
    {
    }

    private int TimeSequence = 0;

    private void Property()
    {
        // string ImagePath = "ChargeMoneyForm/shop_icon_dimand";
        // ShowImage.sprite = ResourceManager.Instance.GetUISprite(ImagePath);
        // bgImage.sprite = ResourceManager.Instance.GetUISprite("ChargeMoneyForm/bg_img");

        var mallAwardStatusData = UserDataManager.Instance.mallAwardStatus.data;
        SetData(mallAwardStatusData.diamond,mallAwardStatusData.finish,mallAwardStatusData.countdown);
    }

    private void SetData(int diamond , int finish , int countdown)
    {
        
        DiamondText.text = diamond.ToString();
        if (finish == 1)
        {
            ButtonText.text = "00:00 Refresh";
            CanReceive = false;
        }
        else
        {
            if (countdown == 0)
            {
                ButtonText.text = "FREE";
                CanReceive = true;
            }
            else
            {
                Timecount = countdown;
                TimeSequence = CTimerManager.Instance.AddTimer(1000, 0, ShowFreeKeyCountDown);
                CanReceive = false;
            }
        }
    }

    //计算剩余时间，并显示倒计时
    private void ShowFreeKeyCountDown(int timeSeq)
    {
        if (Timecount > 0) {
            ButtonText.text = string.Format("{0:d2}:{1:d2}:{2:d2}", Timecount / 3600, (Timecount / 60) % 60, Timecount % 60);
            Timecount -= 1;
        }
        else
        {
            ButtonText.text = "FREE";
            StopCountDown();
        }
    }
    
    //停止倒计时定时器
    private void StopCountDown()
    {
        if (TimeSequence > 0)
        {
            CTimerManager.Instance.RemoveTimer(TimeSequence);
            TimeSequence = -1;
        }
    }

    public void BuyItem(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        if (CanReceive)
        {
            GameHttpNet.Instance.ReceiveMallAward((resultObj) =>
            {
                string result = resultObj.ToString();
                LOG.Info("----ReceiveMallAward---->" + result);
                JsonObject jo = JsonHelper.JsonToJObject(result);
                if (jo != null)
                {
                    LoomUtil.QueueOnMainThread((param) =>
                    {
                        if (jo.code == 200)
                        {
                            HttpInfoReturn<MallAward> mallAwardData = JsonHelper.JsonToObject<HttpInfoReturn<MallAward>>(result);
                            SetData(mallAwardData.data.diamond,mallAwardData.data.finish,mallAwardData.data.countdown);
                        }
                    }, null);
                } });
        }
    }

    /// <summary>
    /// 清理物体
    /// </summary>
    public void Disposal()
    {
        StopCountDown();
        ShowImage.sprite = null;
        bgImage.sprite = null;
        UIEventListener.RemoveOnClickListener(CollectButton, BuyItem);
        Destroy(gameObject);
    }

    void MoveToPosit()
    {
        rectTransform.DOAnchorPosX(0, 0.5f).SetEase(Ease.OutCubic);
        CancelInvoke();
    }

    public void SetGameInit(int vIndex)
    {
        rectTransform.anchoredPosition = new Vector3(800, 0, 0);
        Invoke("MoveToPosit", vIndex * 0.1f);
        Property();
    }

}

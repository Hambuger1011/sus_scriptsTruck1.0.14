using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class EveryDayBox : MonoBehaviour {

    public GameObject BoxButton,BoxOpenFX, FxBoxImage,FxBoxBg;
    public Slider BoxSlider;
    public Image Start, BoxImage,FxBoxImage1, FxBoxImage2;
    public Text StarText,keyText,diamText;
    public CanvasGroup Tist;
    public RectTransform RewardEffect, keypos, dimpos;

    private bool TistF = false;
    private Sprite BoxImageOn, BoxImageOff;
    private int Number,needStar;
    private List<int> boxarr;
    private List<int> open_boxarr;
    public void EveryDayBoxInit(int Number,int nowStar,int needStar,int AllStar)
    {
        //UIEventListener.AddOnClickListener(BoxButton, BoxButtonOnclicke);
        //Tist.alpha = 0;

        this.Number = Number;
        this.needStar = needStar;
        BoxSlider.value = needStar * 1.0f / AllStar;
        StarText.text = "" + needStar;

        diamText.text =UserDataManager.Instance.Getboxlist.data.boxarr[Number - 1].diamond_qty+" Diamonds";
        keyText.text =UserDataManager.Instance.Getboxlist.data.boxarr[Number - 1].bkey_qty+" Key";
        if (Number==1)
        {
            BoxImageOn= ResourceManager.Instance.GetUISprite("Notice/icon1");
            BoxImageOff = ResourceManager.Instance.GetUISprite("Notice/icon4");

        }
        else if (Number == 2)
        {
            BoxImageOn = ResourceManager.Instance.GetUISprite("Notice/icon2");
            BoxImageOff = ResourceManager.Instance.GetUISprite("Notice/icon5");
        }else
        {
            BoxImageOn = ResourceManager.Instance.GetUISprite("Notice/icon3");
            BoxImageOff = ResourceManager.Instance.GetUISprite("Notice/icon6");
        }

       boxarr= UserDataManager.Instance.Getusertask.data.boxarr;
       open_boxarr = UserDataManager.Instance.Getusertask.data.open_boxarr;

        if (boxarr.IndexOf(Number)!=-1)
        {
            //满足条件可以开启的宝箱
            if (open_boxarr.IndexOf(Number ) != -1)
            {
                //这个宝箱已经开启
                BoxImage.sprite = BoxImageOff;
                Start.sprite = ResourceManager.Instance.GetUISprite("Notice/bg_smg");

                FxBoxImage.SetActive(false);
                FxBoxBg.SetActive(false);
               
            }
            else
            {
                //这宝箱还没开启
                BoxImage.sprite = BoxImageOn;

                FxBoxImage.SetActive(true);
                FxBoxBg.SetActive(true);
                FxBoxImage1.sprite = BoxImageOn;
                FxBoxImage2.sprite = BoxImageOn;


                //这个宝箱打开可以开启的条件，播放提示特效
                BoxOpenFX.SetActive(true);

                Start.sprite = ResourceManager.Instance.GetUISprite("Notice/bg_smg");
            }
        }
        else
        {
            //不满足条件不可以开启的宝箱
            BoxImage.sprite = BoxImageOff;
            Start.sprite = ResourceManager.Instance.GetUISprite("Notice/bg_smg3");

            FxBoxImage.SetActive(false);
            FxBoxBg.SetActive(false);
        }
    }

    /// <summary>
    /// 这个是领取了每日任务后，宝箱做出的变化
    /// </summary>
    public void ReviecDayActivtyInit(int num,int HadStart)
    {
        
        if (HadStart>= needStar)
        {
            
            //星星满足了开启的条件
            if (open_boxarr.IndexOf(Number) != -1)
            {
               
                //这个宝箱已经开启了
                BoxImage.sprite = BoxImageOff;
                Start.sprite = ResourceManager.Instance.GetUISprite("Notice/bg_smg3");

                FxBoxImage.SetActive(false);
                FxBoxBg.SetActive(false);
            }
            else
            {
                
                //这个宝箱还没开启过
                BoxImage.sprite = BoxImageOn;
                Start.sprite = ResourceManager.Instance.GetUISprite("Notice/bg_smg");

                FxBoxImage.SetActive(true);
                FxBoxBg.SetActive(true);
                FxBoxImage1.sprite = BoxImageOn;
                FxBoxImage2.sprite = BoxImageOn;

                //这个宝箱打开可以开启的条件，播放提示特效
                BoxOpenFX.SetActive(true);

                //这个宝箱符合开启的条件
                if (boxarr.IndexOf(num + 1)==-1)
                {
                    boxarr.Add(num + 1);

                    EventDispatcher.Dispatch(EventEnum.receiveButtonImageChange, 1);
                }              
            }
        }
        else
        {
            
            BoxImage.sprite = BoxImageOff;
            Start.sprite = ResourceManager.Instance.GetUISprite("Notice/bg_smg3");

            FxBoxImage.SetActive(false);
            FxBoxBg.SetActive(false);
        }
    }

    private void BoxButtonOnclicke(PointerEventData data)
    {


        if (boxarr.IndexOf(Number) != -1&& open_boxarr.IndexOf(Number) == -1)
        {
            //满足开启条件而且还没开启过的宝箱，可以开启
            //UINetLoadingMgr.Instance.Show();

            
            GameHttpNet.Instance.Achieveboxprice(Number, AchieveboxpriceCallBack);
        }else
        {

            //if (!TistF)
            //{
            //    TistF = true;
            //    Tist.DOFade(1, 0.8f).OnComplete(()=> {
            //        Tist.DOFade(0, 2).OnComplete(() => {
            //            TistF = false;
            //        });
            //    });
            //}
        }
    }

    private void AchieveboxpriceCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----AchieveboxpriceCallBack---->" + result);
        if (result.Equals("error"))
        {
            LOG.Info("---AchieveboxpriceCallBack--这条协议错误");
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

                    //#region 领取特效出现
                    ////领取特效出现
                    //RewardEffect.transform.position = BoxImage.transform.position;
                    //RewardEffect.gameObject.SetActive(true);
                    //Vector3 targetPos = Vector3.zero;

                    //targetPos = keypos.transform.localPosition;

                    

                    //RewardEffect.transform.DOLocalMove(targetPos, 1.0f).OnComplete(() => {
                    //    RewardEffect.gameObject.SetActive(false);
                    //    RewardShowData rewardShowData = new RewardShowData();
                    //    rewardShowData.KeyNum = UserDataManager.Instance.Achievetaskprice.data.bkey;
                    //    rewardShowData.DiamondNum = UserDataManager.Instance.Achievetaskprice.data.diamond;
                        
                    //});
                    ////end
                    //#endregion
                  
                    //记录下已经打开的宝箱的id
                    open_boxarr.Add(Number);

                    BoxImage.sprite = BoxImageOff;
                    Start.sprite = ResourceManager.Instance.GetUISprite("Notice/bg_smg");

                    FxBoxImage.SetActive(false);
                    FxBoxBg.SetActive(false);
                }
                else
                {
                    LOG.Info("宝箱领取失败");
                }
            }
        }, null);
    }

    public void AllBoxRevice()
    {
        BoxImage.sprite = BoxImageOff;

        FxBoxImage.SetActive(false);
        FxBoxBg.SetActive(false);
    }
    private void OnDisable()
    {
        //LOG.Info("宝箱消除");
        //UIEventListener.RemoveOnClickListener(BoxButton, BoxButtonOnclicke);
    }
}

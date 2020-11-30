using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;
using AB;
using UGUI;
using pb;

/// <summary>
/// 幸运转盘
/// </summary>
public class LuckRollerForm : BaseUIForm
{

    public GameObject UIMask,AwardTistUIMask;
    public RectTransform Content;
    public GameObject ItemFather;
    public GameObject ItemChild;
    public RectTransform BannerBg;
    public GameObject RollerBg;
    public GameObject StartBtn;
    public GameObject TicketBtn;
    public Text TickNumTxt;
    public Text DescTxt;
    public Text StateText,  FreeText;
    public CanvasGroup FrameCG;
    public GameObject AwardTist, AwardTistRoot;
    public Image Awardtypeimage;
    public Text AwardText,tiTtext;

    private int mItemLen;
    private int mCurFinishNum;

    private int initSpeed;
    public float changeSpeed = 1f;
    private bool mIsStart = false;
    private int AwardNumber = 0;

    private List<LuckRollerItemForm> itemViewList;

    public override void OnOpen()
    {
        base.OnOpen();

        mItemLen = 0;
        mCurFinishNum = 0;
        StateText.text = "WIN BIG UNLOCK SECRETS!";

        UIEventListener.AddOnClickListener(StartBtn, StartHandler);
        UIEventListener.AddOnClickListener(TicketBtn, OpenTicketShopHandler);
        UIEventListener.AddOnClickListener(AwardTistUIMask, AwardTistMove);

        addMessageListener(EventEnum.OnTicketNumChange.ToString(), OnTicketNumChange);

        Content.anchoredPosition = new Vector2(0, 1334);
        Content.DOAnchorPosY(0, 1f).SetEase(Ease.InOutBack).Play();
        
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.GetTickOptions(GetTickListHandler);

        GameHttpNet.Instance.GetUserTicket(GetUserTickHandler);

        AwardTistRoot.SetActive(false);
    }

    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(StartBtn, StartHandler);
        UIEventListener.RemoveOnClickListener(TicketBtn, OpenTicketShopHandler);
        UIEventListener.RemoveOnClickListener(AwardTistUIMask, AwardTistMove);
    }

    private void tiTAplaToChange()
    {
        if (UserDataManager.Instance.userTickInfo.data.isfreeticket == 1&& UserDataManager.Instance.UserData.TicketNum == 0)
        {
            FreeText.text = "Free Ticket: 0";
            NoFreeTitTexToCall(18);
        }
        else
        if (UserDataManager.Instance.userTickInfo.data.isfreeticket == 0)
        {
            //Debug.Log("ddddddddddddd");
            FreeText.text = "Free Ticket: 1";
            //第一次免费
            tiTtext.text = "You have a free ticket to spin the wheel.";
        }
        else
        {
            FreeText.text = "Free Ticket: 0";
            NoFreeTitTexToCall(UnityEngine.Random.Range(1, 5));
        }

    }

    private void NoFreeTitTexToCall(int type)
    {      
        string ST = "";

        //1-4 是闲置且无免费次数时候出现的对话
        switch (type)
        {
            //闲置无免费
            case 1:
                ST = "Click the GO button on the wheel to win the biggest prize!";
                break;
            case 2:
                ST = "You can’t win, if you don’t press GO…";
                break;
            case 3:
                ST = "Do you feel lucky?";
                break;
            case 4:
                ST = "Get a deal! Go the Prize Wheel!";
                break;
                //end

                // 正在转
            case 5:
                ST = "Let’s see if Lady Luck smiles on you today!";
                break;
            case 6:
                ST = "I can’t hold back my excitement to see what you’ll win!";
                break;
            case 7:
                ST = "Dream big ... Win Big!";
                break;
            //end

            //开奖
            case 8:
                ST = "You’re shining brighter than a diamond!";
                break;
            case 9:
                ST = "3 keys can still open plenty of doors!";
                break;
            case 10:
                ST = "Lady Luck is giving you a second chance!";
                break;
            case 11:
                ST = "Wow! Don’t spend it all in one place!";
                break;
            case 12:
                ST = "We have a winner!";
                break;
            case 13:
                ST = "One key can unlock a woman’s heart!";
                break;
            case 14:
                ST = "Get some nice choices with that!";
                break;
            case 15:
                ST = "Forget making a deal, that was a steal.";
                break;
            case 16:
                ST = "Don’t give up, you’ll have better luck next time!";
                break;
            case 17:
                ST = "Call the clerk, you just cashed out!";
                break;
            //end

            //没有票
            case 18:
                ST = "You can go to the ticket shop to buy more tickets.";
                break;
            //end

        }

        tiTtext.text = ST.ToString() ;
    }
    public void Close(Action vCallBack)
    {
        //AudioManager.Instance.StopTones(AudioTones.RouletteSpin);
        Content.DOAnchorPosY(1334, 1f).SetEase(Ease.InOutBack).OnComplete(() =>
        {
            if (mIsStart)
            {
                mIsStart = false;
                if (UserDataManager.Instance.luckyDrawInfo != null && UserDataManager.Instance.luckyDrawInfo.data != null)
                {
                    LuckyDrawInfo luckInfo = UserDataManager.Instance.luckyDrawInfo.data;
                    UserDataManager.Instance.ResetMoney(1, luckInfo.bkey);
                    UserDataManager.Instance.ResetMoney(2, luckInfo.diamond);
                    UserDataManager.Instance.ResetMoney(3, luckInfo.ticket);
                }
            }
              CUIManager.Instance.CloseForm(UIFormName.LuckRollerForm);
              if (vCallBack != null)
                  vCallBack();
        });
    }

    private void GetUserTickHandler(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetUserTickHandler---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.userTickInfo = JsonHelper.JsonToObject<HttpInfoReturn<GetUserTickInfo>>(result);
                    if (UserDataManager.Instance.userTickInfo != null && UserDataManager.Instance.userTickInfo.data != null)
                    {
                       
                        UserDataManager.Instance.ResetMoney(3, UserDataManager.Instance.userTickInfo.data.user_ticket);
                        TickNumTxt.text = UserDataManager.Instance.userTickInfo.data.user_ticket.ToString();
                        ChangeCostTickState();
                        tiTAplaToChange();
                    }
                }
            }, null);
        }
    }

    private void OnTicketNumChange(Notification notific)
    {
        TickNumTxt.text = UserDataManager.Instance.UserData.TicketNum.ToString();
    }

    //更新消耗票券的状态
    private void ChangeCostTickState()
    {
         if (UserDataManager.Instance.userTickInfo != null && UserDataManager.Instance.userTickInfo.data != null)
         {
                 
             if (UserDataManager.Instance.userTickInfo.data.isfreeticket == 1)
             {
                 DescTxt.text = "Free Ticket:Used \n1 Spin = 1 Ticket";
             }
             else
             {
                 DescTxt.text = "Free Ticket:Available\n1 Spin = 1 Ticket";
             }
         }
    }

    private void GetTickListHandler(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetTickListHandler---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.tickListInfo = JsonHelper.JsonToObject<HttpInfoReturn<TicketCont<TicketItemInfo>>>(result);
                    if(UserDataManager.Instance.tickListInfo != null && UserDataManager.Instance.tickListInfo.data != null)
                    {
                        InitRollerItem();
                    }
                }
            }, null);
        }
    }
    private void OpenTicketShopHandler(PointerEventData eventData)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        EventDispatcher.Dispatch(EventEnum.ShowMoneyTicketForm);
    }

    private void InitRollerItem()
    {
        itemViewList = new List<LuckRollerItemForm>();
        for (int i = 0; i < 10; i++)
        {
            LuckRollerItemForm item = GetItem();
            if (item != null)
            {
                item.Init(UserDataManager.Instance.tickListInfo.data.ticketarr[i]);
                item.transform.rotation = Quaternion.Euler(0, 0, i * -36);
                itemViewList.Add(item);
            }
        }
        mItemLen = itemViewList.Count;
    }

    private LuckRollerItemForm GetItem()
    {
        GameObject go = GameObject.Instantiate(ItemChild, ItemFather.transform);
        var t = go.transform;
        t.localPosition = Vector3.zero;
        t.localScale = Vector3.one;
        t.localRotation = Quaternion.identity;
        LuckRollerItemForm item = go.GetComponent<LuckRollerItemForm>();
        return item;
    }


    private int targetAngle = 0;
    private void StartHandler(PointerEventData eventData)
    {
        if (mIsStart) return;
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);

        GameHttpNet.Instance.LuckyDraw(GetLuckDrawResultHandler);

        FreeText.text = "Free Ticket: 0";
       
    }

    private void GetLuckDrawResultHandler(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetLuckDrawResultHandler---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {


                    UserDataManager.Instance.luckyDrawInfo = JsonHelper.JsonToObject<HttpInfoReturn<LuckyDrawInfo>>(result);
                    if (UserDataManager.Instance.luckyDrawInfo != null && UserDataManager.Instance.luckyDrawInfo.data != null)
                    {
                        LuckyDrawInfo luckInfo = UserDataManager.Instance.luckyDrawInfo.data;
                        int index =luckInfo .option - 1;
                        LOG.Info("---LuckDraw Index --->" + index);
                        int round = UnityEngine.Random.Range(4, 6);
                        if(luckInfo.price_type == 3)
                            TickNumTxt.text = (luckInfo.ticket - luckInfo.price_count).ToString();
                        else
                            TickNumTxt.text = luckInfo.ticket .ToString();
                        targetAngle = index * 36;
                        RollerBg.transform.rotation = Quaternion.Euler(0, 0, 0);
                        initSpeed = 360 * round + targetAngle;

                        mIsStart = true;
                        AwardNumber = luckInfo.price_count;
                        TalkingDataManager.Instance.LuckRollerRecord(index, luckInfo.price_type, luckInfo.price_count);

                       // AudioManager.Instance.PlayTones(AudioTones.RouletteSpin);

                        NoFreeTitTexToCall(UnityEngine.Random.Range(5, 8));
                    }
                }
                else if(jo.code == 202)
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(188);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Not enough Ticket", false);
                }
                else if (jo.code == 203)
                {

                }
            }, null);
        }
    }
    void Update()
    {
        if(mIsStart)
        {
            if(initSpeed < 50)
            {
                RollerBg.transform.Rotate(new Vector3(0, 0, 1), -1);
                if(Mathf.Abs(RollerBg.transform.rotation.eulerAngles.z - targetAngle)<=2)
                {
                    mIsStart = false;
                    if (UserDataManager.Instance.userTickInfo != null && UserDataManager.Instance.userTickInfo.data != null)
                    {
                        UserDataManager.Instance.userTickInfo.data.isfreeticket = 1;
                        ChangeCostTickState();
                        ShowAward();
                    }
                }
            }
            else
            {
                RollerBg.transform.Rotate(new Vector3(0, 0, -1) * initSpeed * Time.deltaTime);
                //让转动的速度缓缓降低
                initSpeed -= 3;

                //当转动的速度为0时转盘停止转动
                if (initSpeed <= 0)
                {
                    //转动停止
                    mIsStart = false;
                }
            }
        }
    }
   
    private void ShowAward()
    {
        if (UserDataManager.Instance.luckyDrawInfo != null && UserDataManager.Instance.luckyDrawInfo.data != null)
        {
           // AudioManager.Instance.StopTones(AudioTones.RouletteSpin);
            LuckyDrawInfo luckInfo = UserDataManager.Instance.luckyDrawInfo.data;
            if (luckInfo.price_type == 0) return;
            Vector3 targetPos = new Vector3(174, 615);
         
            if (luckInfo.price_type == 1)    //钥匙
            {
                targetPos = new Vector3(-35, 720);
                AwardTip(1);
            }
            else if(luckInfo.price_type == 2)   //钻石
            {
                targetPos = new Vector3(174, 720);
                AwardTip(2);
            }
            else if (luckInfo.price_type == 3)  //票券
            {
                targetPos = new Vector3(-234, -320);
                AwardTip(3);
            }

            Vector3 startPos = Vector3.zero;
            RewardShowData rewardShowData = new RewardShowData();
            rewardShowData.StartPos = startPos;
            rewardShowData.TargetPos = targetPos;
            rewardShowData.IsInputPos = false;
            rewardShowData.KeyNum = luckInfo.bkey;
            rewardShowData.DiamondNum = luckInfo.diamond;
            rewardShowData.TicketNum = luckInfo.ticket;
            rewardShowData.Type = 2;
            EventDispatcher.Dispatch(EventEnum.GetRewardShow, rewardShowData);

        }
        
    }

    private void AwardTip(int type)
    {
        AwardTistRoot.SetActive(true);
        AwardTist.transform.localPosition = new Vector3(0, 75);
        if (type==1)
        {
            Awardtypeimage.sprite = ResourceManager.Instance.GetUISprite("LuckRollerForm/bg_icona_01");

            if (AwardNumber==3)
            {
                NoFreeTitTexToCall(9);
            }else if (AwardNumber == 1)
            {
                NoFreeTitTexToCall(13);
            }else if (AwardNumber==30)
            {
                NoFreeTitTexToCall(15);
            }else if (AwardNumber==50)
            {
                NoFreeTitTexToCall(17);
            }
        } else if (type==2)
        {
            Awardtypeimage.sprite = ResourceManager.Instance.GetUISprite("LuckRollerForm/bg_icona_04");

            if (AwardNumber==250)
            {
                NoFreeTitTexToCall(8);
            }else if (AwardNumber==100)
            {
                NoFreeTitTexToCall(11);
            }else if (AwardNumber==10)
            {
                NoFreeTitTexToCall(12);
            }
            else if (AwardNumber == 60)
            {
                NoFreeTitTexToCall(14);
            }
        }
        else if (type == 3)
        {
            Awardtypeimage.sprite = ResourceManager.Instance.GetUISprite("LuckRollerForm/bg_icona_07");

            if (AwardNumber==1)
            {
                NoFreeTitTexToCall(10);
            }
        }else if (type==0)
        {
            //没有中奖
            NoFreeTitTexToCall(16);
        }
        Invoke("CallCangeAwardAffter",3);
        AwardText.text = "x" + AwardNumber;
        
    }

    private void CallCangeAwardAffter()
    {
        CancelInvoke("CallCangeAwardAffter");

        if (UserDataManager.Instance.UserData.TicketNum==0)
        {
            NoFreeTitTexToCall(18);
        }else if (UserDataManager.Instance.userTickInfo.data.isfreeticket == 1)
        {
            NoFreeTitTexToCall(UnityEngine.Random.Range(1, 5));
        }
        else if (UserDataManager.Instance.userTickInfo.data.isfreeticket == 0)
        {
            FreeText.text = "Free Ticket: 1";
            tiTtext.text = "You have a free ticket to spin the wheel.";
        }
    }
    public void AwardTistMove(PointerEventData data)
    {
        AwardTist.transform.DOLocalMoveY(900, 0.8f).OnComplete(()=> {         
            AwardTistRoot.SetActive(false);
            AwardTist.transform.localPosition = new Vector3(0, 75);
        });
    }
}

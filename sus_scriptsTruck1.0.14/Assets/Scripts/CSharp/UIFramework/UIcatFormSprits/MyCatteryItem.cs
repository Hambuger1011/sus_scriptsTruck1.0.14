using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UGUI;
using pb;

public class MyCatteryItem : MonoBehaviour {

    private Text Number;
    private Text CatName;
    private Image CatSprite;
    private Image progress;
    private Image Button;
    private Image ButtonTouch;
    private Text timetexte;
    private GameObject buyshow;
    private Image buytype;
    private Text price;
    private GameObject Lock;
    private GameObject LockImage;
    private Text LockText;

    private int CatNum;
    private int Type;
    private int adoptStatus;
    private List<Getadoptpetpetarr> tem;
    private int CatId;
    private int AllTime = 0,StatiAllTime=0;//倒计时需要的总共时间
    public bool HadGiveUp = false;//是否已经放弃收养

    public void Inite(int CatNum,int Type)
    {
        this.CatNum = CatNum;
        this.Type = Type;

        Number = transform.Find("Number").GetComponent<Text>();
        CatName = transform.Find("CatName").GetComponent<Text>();
        CatSprite = transform.Find("CatSprite").GetComponent<Image>();
        progress = transform.Find("progressbg/progress").GetComponent<Image>();
        Button = transform.Find("Button").GetComponent<Image>();
        ButtonTouch = transform.Find("Button/ButtonTouch").GetComponent<Image>();
        timetexte = transform.Find("Button/timetexte").GetComponent<Text>();
        buyshow = transform.Find("Button/buyshow").gameObject;
        buytype = transform.Find("Button/buyshow/buytype").GetComponent<Image>();
        price = transform.Find("Button/buyshow/price").GetComponent<Text>();
        Lock = transform.Find("lock").gameObject;
        LockImage = transform.Find("lock/LockImage").gameObject;
        LockText = transform.Find("lock/LockText").GetComponent<Text>();

        Number.text = CatNum.ToString();

        UIEventListener.AddOnClickListener(gameObject, GameOnclicke);
        UIEventListener.AddOnClickListener(ButtonTouch.gameObject, ButtonOnclicke);

        if (Type==1)
        {
            if (UserDataManager.Instance.Getadoptpet != null)
            {
                tem = UserDataManager.Instance.Getadoptpet.data.petarr;

                Lock.SetActive(false);
                CatName.text = tem[CatNum - 1].pet_name.ToString();

                CatId = tem[CatNum - 1].id;

                int CatSpriteNumbers = CatSpriteNumber(CatId);

               
                string SpriteName = "";
                if (CatSpriteNumbers > 1)
                {
                    int Number = Random.Range(1, CatSpriteNumbers + 1);
                    SpriteName = RuerntNumber(CatId) + RuerntNumber(Number);
                }
                else
                {
                    SpriteName = RuerntNumber(CatId) + RuerntNumber(CatSpriteNumbers);
                }
                //SpriteName = RuerntNumber(CatId) + RuerntNumber(1);

                LOG.Info("猫的id:" + CatId + "--猫的图片数量：" + CatSpriteNumbers+"--当前用的图片："+ SpriteName);

                CatSprite.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/" + SpriteName);
                CatSprite.SetNativeSize();

                adoptStatus = tem[CatNum - 1].adopt_status;

                //已经收养的猫
                CatHadAdoptInit();
            }             
        }
        else if (Type==2)
        {
            //没有收养的猫
            CatNoAdopt();
        }
        else
        {
            //没有解锁的猫
            CatLock();
        }
    }
    public void LockTextShow(string st)
    {
        LockText.text = st.ToString();
    }

    private int CatSpriteNumber(int CatId)
    {
        int Number=0;
        int ForNumber = 1;
        

        for (int i=0;i< ForNumber;i++)
        {
            string SpriteName = RuerntNumber(CatId) + RuerntNumber(i + 1);
            if (ResourceManager.Instance.GetUISprite("CatDecFoodIcon/"+ SpriteName)!=null)
            {
                Number++;
                ForNumber++;
            }
            else
            {
                return Number;
            }
        }

        return Number;
    }

    private string RuerntNumber(int num)
    {
        string ST;
        if (num<10)
        {
            ST = "0" + num;
        }
        else
        {
            ST = "" + num;
        }
        return ST;
    }

    /// <summary>
    /// 已经收养了猫初始化
    /// </summary>
    private void CatHadAdoptInit()
    {
        if (UserDataManager.Instance.Getadoptpet!=null)
        {
           
            if (adoptStatus==1)
            {
                //没有倒计时
              
                Button.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/catteryDh");
                progress.fillAmount = 0;
                timetexte.text = "00:00:00";
                buyshow.SetActive(false);

            }
            else if (adoptStatus==2)
            {
                //显示领养回馈
                if (tem[CatNum - 1].love > 0)
                {
                    //这个是爱心类型
                    Button.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/catteryH");                  
                    buytype.sprite= ResourceManager.Instance.GetUISprite("CatDecFoodIcon/icon");
                    price.text ="X"+ tem[CatNum - 1].love.ToString();
                   
                }
                else if (tem[CatNum - 1].diamond > 0)
                {
                    //这个是钻石类型
                    Button.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/catteryD");                 
                    buytype.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/icon1");
                    price.text ="X"+ tem[CatNum - 1].diamond.ToString();
                   
                }else
                {
                    price.text = "0";
                    buytype.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/icon1");
                    Button.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/catteryD");
                }
                buyshow.SetActive(true);
                progress.fillAmount = 1;
                timetexte.gameObject.SetActive(false);
            }
            else if (adoptStatus==3)
            {
                //显示倒计时
                if (tem[CatNum - 1].love > 0)
                {
                    //这个是爱心类型
                    Button.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/catteryHh");
                }
                else if (tem[CatNum - 1].diamond > 0)
                {
                    //这个是钻石类型
                    Button.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/catteryDh");

                }
                else
                {
                    Button.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/catteryDh");
                }
                progress.gameObject.SetActive(true);
                progress.fillAmount = 0.5f;
               
                buyshow.SetActive(false);

                int datetime = GameDataMgr.Instance.GetCurrentUTCTime();
                int endtime = tem[CatNum - 1].end_time;               
                AllTime = endtime - datetime;//得到计时总共的秒数
                StatiAllTime = tem[CatNum - 1].stay_time;

                if (AllTime<=0)
                {
                    //倒计时完成了
                    timetexte.text = "00:00:00";
                    progress.fillAmount = 0;
                    CancelInvoke("CountDown");
                }
                else
                {

                    if (!UserDataManager.Instance.CatTimeDwonOpen)
                    {
                        //显示倒计时
                        InvokeRepeating("CountDown", 0, 1);
                    }else
                    {
                        timetexte.text = "00:00:00";
                        progress.fillAmount = 0;
                        CancelInvoke("CountDown");
                    }
                  
                }
            }
        }
       
    }

    /// <summary>
    /// 没有收养的猫的初始化
    /// </summary>
    private void CatNoAdopt()
    {
        Lock.SetActive(false);
        CatName.text = "???";
        Button.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/catteryDh");
        //img
        CatSprite.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/img");
        CatSprite.SetNativeSize();
        CatSprite.gameObject.SetActive(true);

        progress.fillAmount = 0;
        timetexte.text = "00:00:00";
        //buyshow.SetActive(false);
        LockImage.gameObject.SetActive(false);
        LockText.gameObject.SetActive(false);
    }

    /// <summary>
    /// 没有解锁的猫的初始化
    /// </summary>
    private void CatLock()
    {
        Lock.SetActive(true);
        CatName.text = "???";
        Button.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/catteryDh");
        CatSprite.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/img");
        CatSprite.SetNativeSize();
        CatSprite.gameObject.SetActive(true);

        progress.fillAmount = 0;
        timetexte.text = "00:00:00";
        buyshow.SetActive(false);

        Button.gameObject.SetActive(false);
    }

    /// <summary>
    /// 倒计时
    /// </summary>
    private void CountDown()
    {
        AllTime--;
        if (AllTime<=0)
        {
            //倒计时完成
            CancelInvoke("CountDown");
            adoptStatus = 2;
            CatHadAdoptInit();
        }
        else
        {
            progress.fillAmount =(StatiAllTime- AllTime)*1.0f /StatiAllTime;
            
            timetexte.text = string.Format("{0:d2}:{1:d2}:{2:d2}", AllTime / 3600, (AllTime / 60) % 60, AllTime % 60);
        }
    }

    private void ButtonOnclicke(PointerEventData data)
    {
        //LOG.Info("按钮被点击了");
        if (Type==1)
        {
            //已经收养的猫

            if (adoptStatus==1)
            {
                //没有倒计时
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(129);

                UITipsMgr.Instance.PopupTips(Localization, false);             
            }
            else if (adoptStatus==2)
            {
                //可以领取领养回馈
                //UINetLoadingMgr.Instance.Show();
                GameHttpNet.Instance.PostGetpetGift(tem[CatNum - 1].fid, 0,2, GetpetGiftCallback);
            }
            else
            {
                //正在显示倒计时
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(129);

                UITipsMgr.Instance.PopupTips(Localization, false);
            }
        }
        else if (Type==2)
        {
            //没有收养的猫
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(130);

            UITipsMgr.Instance.PopupTips(Localization, false);
        }
        else
        {
            //没有解锁的猫
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(131);

            UITipsMgr.Instance.PopupTips(Localization, false);
        }
    }

    /// <summary>
    /// 这个是回馈按钮领取后，状态的刷新
    /// </summary>
    private void ButtonOnclickeAffterInite()
    {
        GameHttpNet.Instance.Getadoptpet(CallBacketadoptpet);
    }


    private void CallBacketadoptpet(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----getadoptpet-CallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.Getadoptpet = JsonHelper.JsonToObject<HttpInfoReturn<Getadoptpet>>(result);
                    if (UserDataManager.Instance.Getadoptpet != null)
                    {
                        tem = UserDataManager.Instance.Getadoptpet.data.petarr;
                        adoptStatus = 3;

                        buyshow.SetActive(false);
                        progress.fillAmount = 0;

                        if (timetexte == null)
                            return;
                        timetexte.gameObject.SetActive(true);
                        Button.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/catteryDh");

                        CatHadAdoptInit();

                    }

                }

            }, null);
        }
    }

    private void GameOnclicke(PointerEventData data)
    {
        //LOG.Info("按钮被点击了");
        if (Type == 1)
        {

            if (HadGiveUp)
            {
                //已经放弃收养了

                var Localization = GameDataMgr.Instance.table.GetLocalizationById(126);
                UITipsMgr.Instance.PopupTips(Localization, false);

                return;
            }

            //已经收养的猫

            //派发事件，关闭这个界面
           // EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), (int)CatFormEnum.CAT_COLLECT);

            //显示锚的属性
            AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
            CUIManager.Instance.OpenForm(UIFormName.CatDetails);
            CUIManager.Instance.GetForm<CatDetailsForm>(UIFormName.CatDetails).Inite(CatId,1,this);



        }
        else if (Type == 2)
        {
            //没有收养的猫
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(130);

            UITipsMgr.Instance.PopupTips(Localization, false);
            //UITipsMgr.Instance.PopupTips("这只猫没有被收养", false);
        }
        else
        {
            //没有解锁的猫
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(131);

            UITipsMgr.Instance.PopupTips(Localization, false);
            //UITipsMgr.Instance.PopupTips("这只猫没有解锁", false);
        }
    }
    private void CatCall(string st)
    {

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
                        string dia = UserDataManager.Instance.SceneInfo.data.usermoney.diamond.ToString();
                        string love = UserDataManager.Instance.SceneInfo.data.usermoney.love.ToString();
                        string food = UserDataManager.Instance.SceneInfo.data.usermoney.otherfood.ToString();
                        tmpForm.RefreshDiamondAndHeart(dia, love,"");
                    }
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(132);

                    UITipsMgr.Instance.PopupTips(Localization, false);
                    //UITipsMgr.Instance.PopupTips("领取成功", false);

                    //显示倒计时
                    if (tem[CatNum - 1].love > 0)
                    {
                        //这个是爱心类型
                        Button.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/catteryHh");
                    }
                    else if (tem[CatNum - 1].diamond > 0)
                    {
                        //这个是钻石类型
                        Button.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/catteryDh");
                    }
                    else
                    {
                        Button.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/catteryDh");
                    }

                    ButtonOnclickeAffterInite();
                }
                else
                {
                    //UITipsMgr.Instance.PopupTips(jo.msg, false);
                }


            }, null);
        }

    }
    /// <summary>
    /// 调用这个方法移除物体和释放资源
    /// </summary>
    public void Dispote()
    {
        UIEventListener.RemoveOnClickListener(ButtonTouch.gameObject, ButtonOnclicke);
        CatSprite.sprite = null;

        CancelInvoke("CountDown");
        //LOG.Info("销毁领养的猫");
        Destroy(gameObject);
    }
}

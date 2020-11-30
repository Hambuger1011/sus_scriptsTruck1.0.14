using System;
using UnityEngine;
using UnityEngine.UI;

public class MaintenancePanel : MonoBehaviour
{
    public GameObject BgObj;
    private Text Title;
    private Text ContentText;
    private Button BtnFackbook;
    private Button BtnRefresh;

    private Text Timetext1;
    private Text Timetext2;
    private Text Timetext3;
    private Text Timetext4;
    private Text Timetext5;
    private Text Timetext6;
    private Text Timetext7;

    public GameObject img7;

    private GameObject Timer;
    private GameObject TipText;
    private void Awake()
    {
        this.BgObj = DisplayUtil.GetChild(this.gameObject, "BgObj");
        this.Title = DisplayUtil.GetChildComponent<Text>(this.gameObject, "Title");
        this.ContentText = DisplayUtil.GetChildComponent<Text>(this.gameObject, "ContentText");
        this.BtnFackbook = DisplayUtil.GetChildComponent<Button>(this.gameObject, "BtnFackbook");
        this.BtnRefresh = DisplayUtil.GetChildComponent<Button>(this.gameObject, "BtnRefresh");

        this.Timetext1 = DisplayUtil.GetChildComponent<Text>(this.gameObject, "Timetext1");
        this.Timetext2 = DisplayUtil.GetChildComponent<Text>(this.gameObject, "Timetext2");
        this.Timetext3 = DisplayUtil.GetChildComponent<Text>(this.gameObject, "Timetext3");
        this.Timetext4 = DisplayUtil.GetChildComponent<Text>(this.gameObject, "Timetext4");
        this.Timetext5 = DisplayUtil.GetChildComponent<Text>(this.gameObject, "Timetext5");
        this.Timetext6 = DisplayUtil.GetChildComponent<Text>(this.gameObject, "Timetext6");
        this.Timetext7 = DisplayUtil.GetChildComponent<Text>(this.gameObject, "Timetext7");

        this.img7 = DisplayUtil.GetChild(this.gameObject, "img7");

        this.Timer = DisplayUtil.GetChild(this.gameObject, "Timer");
        this.TipText = DisplayUtil.GetChild(this.gameObject, "TipText");

        this.BtnFackbook.onClick.AddListener(OnBtnFackbookClick);
        this.BtnRefresh.onClick.AddListener(OnBtnRefreshClick);
    }

    /// <summary>
    /// 剩余时间
    /// </summary>
    private int laveTime = -1;

    private int TimeSequence = 0;
    public void SetData(string content,string _time)
    {

        this.Timer.SetActive(true);
        this.TipText.SetActive(false);
    
        //维护开始时间  "2019-08-08 05:00:00"
        string startAt = UserDataManager.Instance.appconfinfo.updateInfo.maintainInfo.startAt;
        //维护结束时间  "2019-09-04 07:35"
        string endAt = UserDataManager.Instance.appconfinfo.updateInfo.maintainInfo.endAt;
        //维护奖励数量 
        int maintainItemNum = UserDataManager.Instance.appconfinfo.updateInfo.maintainItemNum;

        this.ContentText.text = string.Format(content, startAt, endAt, maintainItemNum.ToString());
        laveTime = int.Parse(_time);

        int hour = laveTime / 3600;
        if (hour >= 100)
        {
            this.img7.SetActive(true);
        }

        if (laveTime > 0)
        {
            TimeSequence = CTimerManager.Instance.AddTimer(1000, 0, CountDown);
        }

    }


    //倒计时
    private void CountDown(int timerSequence)
    {
        int hour = laveTime / 3600;
        int minute = (laveTime / 60) % 60;
        int second = laveTime % 60;

        string time = String.Format("{0:00}:{1:00}:{2:00}", hour, minute, second);
        int len = time.Length;

        int num7 = 0;
        if (hour > 99)
        {
            num7 = hour / 100;
        }
        int num6 = (hour % 100) / 10;
        int num5 = hour % 10;

        int num4 = minute / 10;
        int num3 = minute % 10;

        int num2 = second / 10;
        int num1 = second % 10;

        this.Timetext7.text = num7.ToString();
        this.Timetext6.text = num6.ToString();
        this.Timetext5.text = num5.ToString();
        this.Timetext4.text = num4.ToString();
        this.Timetext3.text = num3.ToString();
        this.Timetext2.text = num2.ToString();
        this.Timetext1.text = num1.ToString();

        laveTime -= 1;
        if (laveTime <= 0)
        {
            CTimerManager.Instance.RemoveTimer(TimeSequence);


            this.Timer.SetActive(false);
            this.TipText.SetActive(true);

           // isTimeEnd = true;
        }

    }


    private bool isTimeEnd=false;

    /// <summary>
    /// 进入下一步
    /// </summary>
    /// <param name="collision"></param>
    private void EnterNext(Collision collision)
    {
        //倒计时结束


    }


    public void onClose()
    {
        this.BgObj.SetActiveEx(false);
        CTimerManager.Instance.RemoveTimer(TimeSequence);
    }

    private void OnBtnFackbookClick()
    {
        Application.OpenURL("https://www.facebook.com/Scripts-Untold-Secrets-107729237761206/");
    }

    private DateTime _oldTime;

    private void OnBtnRefreshClick()
    {
        //当前时间 分钟
        DateTime curTime = DateTime.Now;
        //时间差 大于60秒
        long _sunTime = DateUtil.StampToDateTime2(curTime, _oldTime);
        if (_sunTime >=5)
        {
            //再次请求AppConf
            Dispatcher.dispatchEvent(EventEnum.AgainGetAppConf);
            _oldTime = curTime;
        }
        else
        {
           // UITipsMgr.Instance.PopupTips(string.Format("Refresh too frequently, please slow down and wait {0} seconds !", _sunTime), false);
        }
       
        // if (isTimeEnd == true)
        // {
        //     
        // }
        // else
        // {
        //     //刷新时间
        //     Dispatcher.dispatchEvent(EventEnum.UpdateMaintinTime);
        // }
    }

    /// <summary>
    /// 刷新维护时间
    /// </summary>
    public void UpdateMaintinTime(int _time)
    {
        if (laveTime > 0)
        {
            laveTime = _time;
        }
        else
        {
            laveTime = 0;
        }

        //维护内容
        string _content = UserDataManager.Instance.appconfinfo.messagesInfo.content.maintain;
        //维护开始时间  "2019-08-08 05:00:00"
        string startAt = UserDataManager.Instance.appconfinfo.updateInfo.maintainInfo.startAt;
        //维护结束时间  "2019-09-04 07:35"
        string endAt = UserDataManager.Instance.appconfinfo.updateInfo.maintainInfo.endAt;
        //维护奖励数量 
        int maintainItemNum = UserDataManager.Instance.appconfinfo.updateInfo.maintainItemNum;

        this.ContentText.text = string.Format(_content, startAt, endAt, maintainItemNum.ToString());


    }
}

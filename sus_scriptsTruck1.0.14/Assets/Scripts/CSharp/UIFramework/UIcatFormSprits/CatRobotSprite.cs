using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using CatMainFormClasse;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using pb;

public class CatRobotSprite : CatClase
{
    private GameObject RobotIcon;
    private CatMainForm catForm;
    private Text NextUpdateTimeTxt, EndTimeTxt;
    private Image TimeDetailBg;
    private CanvasGroup TimeCanGroup;
    private Text leve;

    private CatRobotInfo mRobotInfo;
    private int mRobotEndTime, mRobotNextUpdateTime;

    private bool mIsShowDetail = false;
    private bool mInTweening = false;
    private int mTimeQue = 0;

    private string mEndTimeStr1;
    private string mEndTimeStr2;
    private string mUpdateStr1;
    private string mUpdateStr2;

    public override void Bind(CatMainForm catmainform)
    {
        catmainform.CatRobotSpr = this;
        catForm = catmainform;

        RobotIcon = transform.Find("RobotGroup/RobotIcon").gameObject;
        TimeDetailBg = transform.Find("RobotGroup/TimeDetailBg").gameObject.GetComponent<Image>();
        TimeCanGroup = transform.Find("RobotGroup/TimeInfoGroup").gameObject.GetComponent<CanvasGroup>();
        NextUpdateTimeTxt = transform.Find("RobotGroup/TimeInfoGroup/NextUpdateTimeTxt").gameObject.GetComponent<Text>();
        EndTimeTxt = transform.Find("RobotGroup/TimeInfoGroup/EndTimeTxt").gameObject.GetComponent<Text>();
        leve = transform.Find("RobotGroup/RobotIcon/leve").GetComponent<Text>();

        TimeDetailBg.fillAmount = 0;
        TimeCanGroup.alpha = 0;
        mIsShowDetail = false;

        UIEventListener.AddOnClickListener(RobotIcon, RobotIconClickHandler);
        string LocalInfo;
        LocalInfo = GameDataMgr.Instance.table.GetLocalizationById(236);
        if (LocalInfo != null) mEndTimeStr1 = LocalInfo;
        LocalInfo = GameDataMgr.Instance.table.GetLocalizationById(237);
        if (LocalInfo != null) mEndTimeStr2 = LocalInfo;
        LocalInfo = GameDataMgr.Instance.table.GetLocalizationById(238);
        if (LocalInfo != null) mUpdateStr1 = LocalInfo;
        LocalInfo = GameDataMgr.Instance.table.GetLocalizationById(239);
        if (LocalInfo != null) mUpdateStr2 = LocalInfo;

        SetRobotInfo();
        mTimeQue = CTimerManager.Instance.AddTimer(1000, -1, OnTimeHandler);
    }


    public void UpdateRobotInfo()
    {
        SetRobotInfo();
    }

    private void HomeButtonOnclicke(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);

        //派发事件，关闭猫的主界面
        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), (int)CatFormEnum.CAT_MAIN);
    }

    public override void CloseUi()
    {
        UIEventListener.RemoveOnClickListener(RobotIcon, RobotIconClickHandler);
        CTimerManager.Instance.RemoveTimer(mTimeQue);
    }

    private void RobotIconClickHandler(PointerEventData data)
    {
        if (mInTweening) return;
        if (!mIsShowDetail)
        {
            mInTweening = true;
            TimeDetailBg.DOFillAmount(1, 0.2f).SetEase(Ease.Flash).OnComplete(() =>
            {
                TimeCanGroup.DOFade(1, 0.2f).SetEase(Ease.Flash).OnComplete(() =>
                {
                    mInTweening = false;
                    mIsShowDetail = true;
                }).Play();
            }).Play();
        }
        else
        {
            mInTweening = true;
            TimeCanGroup.DOFade(0, 0.2f).SetEase(Ease.Flash).OnComplete(() =>
            {
                TimeDetailBg.DOFillAmount(0, 0.2f).SetEase(Ease.Flash).OnComplete(() =>
                {
                    mInTweening = false;
                    mIsShowDetail = false;
                }).Play();
            }).Play();
        }
    }

    //机器人的信息 
    private void SetRobotInfo()
    {
        if (UserDataManager.Instance.SceneInfo.data.robot != null)
        {
            int len = UserDataManager.Instance.SceneInfo.data.robot.Count;
            for (int i = 0; i < len; i++)
            {
                mRobotInfo = UserDataManager.Instance.SceneInfo.data.robot[i];
            }

            if (mRobotInfo!=null)
            {
                string Leve ="";
                if (mRobotInfo.shop_id == 42)
                    Leve = "C";
                else if (mRobotInfo.shop_id == 41)
                    Leve = "B";
                else if (mRobotInfo.shop_id == 40)
                    Leve = "A";

                Debug.Log("等级：" + Leve);
                leve.text = Leve.ToString();
            }
            
            ResetRobotTime();
        }
    }

    private void OnTimeHandler(int vQue)
    {
        ResetRobotTime();
    }

    private void ResetRobotTime()
    {
        if (mRobotInfo != null)
        {
            
            if (mRobotInfo.endtime > GameDataMgr.Instance.GetCurrentUTCTime())
            {
                mRobotEndTime = mRobotInfo.endtime - GameDataMgr.Instance.GetCurrentUTCTime();
                if (mRobotInfo.utime > GameDataMgr.Instance.GetCurrentUTCTime())
                {
                    mRobotNextUpdateTime = mRobotInfo.utime - GameDataMgr.Instance.GetCurrentUTCTime();
                }
                else
                {
                    int hour = 0;
                    if (mRobotInfo.shop_id == 42)
                        hour = 1;
                    else if (mRobotInfo.shop_id == 41)
                        hour = 2;
                    else if (mRobotInfo.shop_id == 40)
                        hour = 4;

                    mRobotNextUpdateTime = mRobotInfo.utime + hour * 3600 - GameDataMgr.Instance.GetCurrentUTCTime();

                }



                //Debug.Log("-----curServerTime---->" + GameDataMgr.Instance.GetCurrentUTCTime() + "======endTime------>" + mRobotInfo.endtime + "------catTime-------->" + mRobotInfo.utime);

                int day = (mRobotEndTime / 3600) / 24;
                if (day >= 1)
                    EndTimeTxt.text = string.Format(mEndTimeStr1, day, (mRobotEndTime / 3600) % 24);
                else
                    EndTimeTxt.text = string.Format(mEndTimeStr2, mRobotEndTime / 3600, (mRobotEndTime / 60) % 60);


                if (mRobotNextUpdateTime > 3600)
                    NextUpdateTimeTxt.text = string.Format(mUpdateStr1, mRobotNextUpdateTime / 3600, (mRobotNextUpdateTime / 60) % 60, mRobotNextUpdateTime % 60);
                else
                {
                    //Debug.Log("mUpdateStr2:"+ mUpdateStr2);// Clean up cattery：{0:d2}:{1:d2}:{2:d2}
                    mUpdateStr2 = "Clean up cattery：{0:d2}:{1:d2}";
                    if (mRobotNextUpdateTime>0)
                       NextUpdateTimeTxt.text = string.Format(mUpdateStr2, (mRobotNextUpdateTime / 60) % 60, mRobotNextUpdateTime % 60);

                }

                if (!RobotIcon.gameObject.activeSelf) RobotIcon.SetActive(true);
            }
            else
            {
                if (RobotIcon.gameObject.activeSelf) RobotIcon.SetActive(false);
            }
        }
    }
}

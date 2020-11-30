using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UGUI;
using UnityEngine.UI;

public class BookReadingBottomBar : MonoBehaviour {


    public RectTransform SetingParent;
    public GameObject SettingButton;

    public CanvasGroup DanmakuNumGo;
    public Text DanmakuNumTxt;


    private bool isBarActive = false;
    private bool inHideDanmaku = false;
    private float restingTime = 0f, MaxRestime = 8f;

    public void Init()
    {
        DanmakuNumGo.DOKill();
        isBarActive = true;
        inHideDanmaku = false;
        UIEventListener.AddOnClickListener(transform.gameObject, BottomTouch);
        UIEventListener.AddOnClickListener(SettingButton, SettingButtomOnclice);
        UIEventListener.AddOnClickListener(DanmakuNumGo.gameObject, ShowwDanmakuFormHandler);

        EventDispatcher.AddMessageListener(EventEnum.DanmakuFlagTrigger, DanmakuFlagTriggerHandler);
        EventDispatcher.AddMessageListener(EventEnum.CreateBookBarrageSuccess, CreateBookBarrageSuccess);
    }

    public void Dispose()
        {
            //DanmakuNumGo.DOKill();
            isBarActive = false;
            UIEventListener.RemoveOnClickListener(transform.gameObject, BottomTouch);
            UIEventListener.RemoveOnClickListener(SettingButton, SettingButtomOnclice);
            UIEventListener.RemoveOnClickListener(DanmakuNumGo.gameObject, ShowwDanmakuFormHandler);

            EventDispatcher.RemoveMessageListener(EventEnum.DanmakuFlagTrigger, DanmakuFlagTriggerHandler);
            EventDispatcher.RemoveMessageListener(EventEnum.CreateBookBarrageSuccess, CreateBookBarrageSuccess);
        }
    
    private void DanmakuFlagTriggerHandler(Notification vNot)
    {
        int tempDialogId = System.Convert.ToInt32(vNot.Data);
        int resultNum = 0;
        if (tempDialogId > 0 && UserDataManager.Instance.bookDetailInfo.data != null && UserDataManager.Instance.bookDetailInfo.data.book_barrage_status == 1)
        {
            if (UserDataManager.Instance.BookBarrageCountList.data != null && UserDataManager.Instance.BookBarrageCountList.data.data_list != null)
            {
                int len = UserDataManager.Instance.BookBarrageCountList.data.data_list.Count;
                for (int i = 0; i < len; i++)
                {
                    BookBarrageCountItem countItem = UserDataManager.Instance.BookBarrageCountList.data.data_list[i];
                    if (countItem != null && countItem.dialog_id == tempDialogId)
                    {
                        resultNum = countItem.barrage_count;
                        break;
                    }
                }
            }
            ShowDanmakuNumIcon(true);
            DanmakuNumTxt.text = resultNum + "";
        }
        else
            ShowDanmakuNumIcon(false);
    }

    private void CreateBookBarrageSuccess(Notification obj)
    {
        DanmakuNumTxt.text = (int.Parse(DanmakuNumTxt.text) + 1).ToString();
    }
    
    private void ShowDanmakuNumIcon(bool value)
    {
        if(DanmakuNumGo != null)
        {
            if (value && !DanmakuNumGo.gameObject.activeSelf)
            {
                DanmakuNumGo.alpha = 0;
                DanmakuNumGo.gameObject.SetActive(true);
                DanmakuNumGo.DOFade(1, 0.2f).Play();
            }
            else if (!value && DanmakuNumGo.gameObject.activeSelf)
            {
                if (inHideDanmaku) return;
                inHideDanmaku = true;
                DanmakuNumGo.alpha = 1;
                DanmakuNumGo.DOFade(0, 0.2f).OnComplete(() =>
                {
                    inHideDanmaku = false;
                    DanmakuNumGo.gameObject.SetActive(false);
                }).Play();
            }
        }
    }
    
    // Update is called once per frame
    void Update () {

        if (isBarActive)
        {
            restingTime += Time.unscaledDeltaTime;

            if (restingTime>=MaxRestime)
            {
                restingTime = 0;
                //这里倒计时结束，把低部设置按钮移出去
                HideBar();
            }
        }
		
	}

    public void BottomTouch(PointerEventData data)
    {

        isBarActive = true;
        SetingGameMove(22, 0.8f);
        BookReadingFormTopBarCon.TouchAreaOnClickOnBottom();
    }

    public BookReadingFormTopBarController BookReadingFormTopBarCon;
    public void BottomTouchOnTopBottom()
    {
        isBarActive = true;
        SetingGameMove(22, 0.8f);
    }

    private void SettingButtomOnclice(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        //Debug.Log("这个是设置界面的按钮点击事件");

        //CUIManager.Instance.OpenForm(UIFormName.GamePlaySetting);

        CUIManager.Instance.OpenForm(UIFormName.SettingForm);
    }

    private void ShowwDanmakuFormHandler(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        GamePointManager.Instance.BuriedPoint(EventEnum.ViewBarrage,"","",BookReadingWrapper.Instance.CurrentBookData.BookID.ToString(),BookReadingWrapper.Instance.CurrentBookData.DialogueID.ToString());
        CUIManager.Instance.OpenForm(UIFormName.DanmakuForm);

        ShowDanmakuNumIcon(false);
    }

    public void HideBar()
    {
        isBarActive = false;
        SetingGameMove(-180, 0.8f);
    }



    private void SetingGameMove(float PosY,float time)
    {
        SetingParent.DOLocalMoveY(PosY, time);
    }

}

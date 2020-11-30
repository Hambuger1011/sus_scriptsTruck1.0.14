using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;
using UGUI;
using pb;

#if !NOT_USE_LUA
using DialogDisplaySystem = BookReadingWrapper;
#endif

/// <summary>
/// 屏幕点击
/// </summary>
[XLua.LuaCallCSharp, XLua.Hotfix]
public class SceneTapForm : BaseUIForm
{

    public GameObject UIMask;
    public RectTransform Content;
    public Image RoleImage;
    public Image NpcImage;
    public Image MouthImage;

    public GameObject TouchBg;
    public Text DescTxt;
    public Image ClickTimesProgress;
    public ProgressBar TimeCDProgress;

    public ParticleSystem partSystem;

    private bool mStartCountDown = false;
    private bool mIsFinish = false;
    private int mClickTimes = 0;
    private int mLeftTime = 0;
    private int mTotalTime = 10;
    private int mFinishTotalClick = 50;

    private float mDetailTime = 0;
    private MetaGameData mMetaGameData;

    private BaseDialogData mDialogData;
    private int mSelectLen = 0;
    private int mResultIndex = 0;
    private bool mHasGetEggHidden = false;


#if !NOT_USE_LUA
    private Action<int> onComplete;
#endif

    public override void OnOpen()
    {
        base.OnOpen();

        mStartCountDown = false;
        mIsFinish = false;
        mHasGetEggHidden = false;
        mClickTimes = 0;
        mLeftTime = mTotalTime;
        partSystem.gameObject.SetActive(false);
        UIEventListener.AddOnClickListener(TouchBg, TouchBgHandler);
        setBGOnClickListenerActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(TouchBg, TouchBgHandler);
    }

    private void setBGOnClickListenerActive(bool boolean)
    {
        var evtTrigger = TouchBg.GetComponent<UIEventTriggerBase.UIEventTrigger>();
        if (evtTrigger != null)
            evtTrigger.enabled = boolean;
    }

#if !NOT_USE_LUA
    public void Init(int vMetaGameId, BaseDialogData data, Action<int> callback)
    {
        mDialogData = data;
        onComplete = callback;
#else
    public void Init(int vMetaGameId)
    {
        mDialogData = DialogDisplaySystem.Instance.CurrentDialogData;
#endif

        mSelectLen = mDialogData.selection_num;

        t_MetaGameDetails metaGameCfg = GameDataMgr.Instance.table.GetMetaGameById(vMetaGameId);
        if (metaGameCfg != null)
        {
            mMetaGameData = new MetaGameData(metaGameCfg);
            if (mMetaGameData != null)
                InitSceneTap();
        }
        else
        {
            LOG.Error("小游戏配置有错--->" + vMetaGameId);
        }

    }

    private void InitSceneTap()
    {
        InitRole();

        int vType = mMetaGameData.ShapeDetails;
        DescTxt.text = mMetaGameData.Description;
        ClickTimesProgress.fillAmount = 0;
        TimeCDProgress.Value = 1;

        
        ShowRole();

#if ENABLE_DEBUG
        if (GameDataMgr.Instance.InAutoPlay)
        {
            Invoke("AutoDo",1.5f);
        }
#endif
    }

    private void AutoDo()
    {
        CancelInvoke("AutoDo");
        mLeftTime = 0;
        OnTimeHandler();
    }



    private void InitRole()
    {
        int clothesID = 0;
        if (mMetaGameData.CharacterA == 0)
            clothesID = 0;
        else if (mMetaGameData.CharacterA > 0)
            clothesID = DialogDisplaySystem.Instance.CurrentBookData.PlayerClothes;

        if (mMetaGameData.CharacterA >= 0)
        {
            int roleImageId = (100000 + (1 * 10000) + (1 * 100) + clothesID) * 10000;
            RoleImage.sprite = DialogDisplaySystem.Instance.GetUITexture("RoleHead/" + roleImageId, false);
            RoleImage.transform.localScale = new Vector3(-1, 1, 1);
        }

        if (mMetaGameData.CharacterB > 0)
        {
            int npcDetailId = 0;
            if (mMetaGameData.CharacterB == DialogDisplaySystem.Instance.CurrentBookData.NpcId)
                npcDetailId = DialogDisplaySystem.Instance.CurrentBookData.NpcDetailId;

            int npcImageId = (100000 + (mMetaGameData.CharacterB * 100) + mMetaGameData.CharacterBClothes) * 10000 + npcDetailId;
            NpcImage.sprite = DialogDisplaySystem.Instance.GetUITexture("RoleHead/" + npcImageId, false);
        }
    }

    private void ShowRole()
    {
        RoleImage.rectTransform.anchoredPosition = new Vector2(-600, 860);
        NpcImage.rectTransform.anchoredPosition = new Vector2(600, 860);

        if (mMetaGameData.CharacterA >= 0)
            RoleImage.rectTransform.DOAnchorPosX(-160, 0.4f).SetDelay(1f).Play();

        if (mMetaGameData.CharacterB > 0)
            NpcImage.rectTransform.DOAnchorPosX(160, 0.4f).SetDelay(0.8f).Play();

        Content.anchoredPosition = new Vector2(0, -1334);
        Content.DOAnchorPosY(253, 0.8f).SetEase(Ease.InOutBack).Play();
    }

    private void HideRole()
    {
        if (mMetaGameData.CharacterA >= 0)
        {
            RoleImage.rectTransform.anchoredPosition = new Vector2(-160, 860);
            RoleImage.rectTransform.DOAnchorPosX(-600, 0.4f).Play();
        }

        if (mMetaGameData.CharacterB > 0)
        {
            NpcImage.rectTransform.anchoredPosition = new Vector2(160, 860);
            NpcImage.rectTransform.DOAnchorPosX(600, 0.4f).SetDelay(0.2f).Play();
        }

        Content.anchoredPosition = new Vector2(0, 253);
        Content.DOAnchorPosY(-1334, 1f).SetDelay(0.6f).OnComplete(() =>
        {
            EventDispatcher.Dispatch(EventEnum.ChangeBookReadingBgEnable, 1);
            EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, mResultIndex);
            CUIManager.Instance.CloseForm(UIFormName.SceneTapForm);

#if !NOT_USE_LUA
            onComplete(mResultIndex);
#endif
        }).SetEase(Ease.InOutBack).Play();
    }

    private Vector2 ConvertPosVector2(string vPosStr)
    {
        if (!string.IsNullOrEmpty(vPosStr))
        {
            string[] tempList = vPosStr.Split(',');
            if (tempList.Length > 1)
                return new Vector2(float.Parse(tempList[0]), float.Parse(tempList[1]));
            else
                LOG.Error(" 小游戏 坐标配置有错 gameId:" + mMetaGameData.ID + "==str==>" + vPosStr);
        }
        return Vector2.zero;
    }


    void Update()
    {
        if(mStartCountDown && !mIsFinish)
        {
            mDetailTime += Time.deltaTime;
            if(mDetailTime >= 1)
            {
                mDetailTime = 0;
                mLeftTime--;

                OnTimeHandler();
            }
        }
    }

    private void TouchBgHandler(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        if(!mStartCountDown)
            mStartCountDown = true;

        
        if(!mIsFinish)
        {
            partSystem.gameObject.SetActive(true);
            partSystem.Play();
            mClickTimes++;
            TimesChange();
        }
    }

    private void OnTimeHandler()
    {
        TimeCDProgress.Value = (float)((mLeftTime * 1.0) /mTotalTime);

        if(mLeftTime <=0)
        {
            mIsFinish = true;
            setBGOnClickListenerActive(false);
            MouthImage.GetComponent<CanvasGroup>().DOFade(0, 0.8f).SetDelay(0.8f).OnComplete(() => { }).Play();
            MouthImage.rectTransform.DOAnchorPos(new Vector2(0, 600), 0.8f).SetDelay(0.3f).Play();
            MouthImage.rectTransform.DOScale(2f, 0.8f).SetDelay(0.3f).OnComplete(() =>
            {
                partSystem.Stop();
                HideRole();
            }).Play();

            CheckGetEgghidden();
        }
    }

    private void TimesChange()
    {
        ClickTimesProgress.fillAmount = (float)((mClickTimes * 1.0f) / mFinishTotalClick);

        for (int i = 0; i < mSelectLen; i++)
        {
            if (int.Parse(mDialogData.GetSelectionsText()[i]) >= mClickTimes)
            {
                mResultIndex = i;
                break;
            }
        }
        DescTxt.text = mMetaGameData.GetPostList()[mResultIndex];

        if(ClickTimesProgress.fillAmount>=0.99f)
        {
            CheckGetEgghidden();
        }
    }

    private void CheckGetEgghidden()
    {
        if (mHasGetEggHidden) return;
        mHasGetEggHidden = true;
        int hiddentEgg = mDialogData.GetSelectionsHiddenEgg()[mResultIndex];
        if (hiddentEgg > 0)
        {
            if (hiddentEgg == 1)
            {
#if USE_SERVER_DATA
                GameHttpNet.Instance.SendPlayerProgress(UserDataManager.Instance.UserData.CurSelectBookID, DialogDisplaySystem.Instance.CurrentBookData.ChapterID
                , mDialogData.dialogID, GetHiddenEggCallBack, mResultIndex + 1);
#else
                DoGetHiddenEgg();
#endif

            }
        }
    }
    private void DoGetHiddenEgg()
    {
        Vector3 mHiddenEggStartPos = new Vector3(45973.4f, 26616.4f, 10.0f);
        Vector3 targetPos = new Vector3(306, 625);
        RewardShowData rewardShowData = new RewardShowData();
        rewardShowData.StartPos = mHiddenEggStartPos;
        rewardShowData.TargetPos = targetPos;
        if (UserDataManager.Instance.SaveStepInfo != null && UserDataManager.Instance.SaveStepInfo.data != null)
        {
            rewardShowData.KeyNum = UserDataManager.Instance.SaveStepInfo.data.user_key;
            rewardShowData.DiamondNum = UserDataManager.Instance.SaveStepInfo.data.user_diamond;
            EventDispatcher.Dispatch(EventEnum.HiddenEggRewardShow, rewardShowData);
        }
    }

    private void GetHiddenEggCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----BookDialogOptionCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.SaveStepInfo = JsonHelper.JsonToObject<HttpInfoReturn<SaveStep>>(result);
                    DoGetHiddenEgg();
                }
                else if (jo.code == 203)
                {
                    LOG.Error("--GetHiddenEggCallBack--此对话没有彩蛋 DialogId" + DialogDisplaySystem.Instance.CurrentBookData.DialogueID);
                }
            }, null);
        }
    }

    
}

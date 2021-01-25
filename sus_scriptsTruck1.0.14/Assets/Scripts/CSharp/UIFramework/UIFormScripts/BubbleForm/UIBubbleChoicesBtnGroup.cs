using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;
using UGUI;

#if !NOT_USE_LUA
using DialogDisplaySystem = BookReadingWrapper;
#endif

/// <summary>
/// 气泡聊天 选项
/// </summary>

[XLua.Hotfix, XLua.CSharpCallLua, XLua.LuaCallCSharp]
public class UIBubbleChoicesBtnGroup : MonoBehaviour
{
    public GameObject ContentGroup;
    private GameObject ChoiceButtonPrefab;
    private List<UIBubbleSelectionItem> m_uiObjectList = new List<UIBubbleSelectionItem>();

    private int mOptionCost;
    private int mIndex;
    private Vector3 mHiddenEggStartPos;
    private int mMaxNum;
    private bool mNeedPay = false;

    private BaseDialogData mDialogData;
    private GridLayoutGroup m_gridLayoutGroup;
    private CanvasGroup m_canvasGroup;

    private Vector2 m_vCellSize;
    private Vector2 m_vSpacing;

    
    public void Init()
    {
        ChoiceButtonPrefab = AB.ABSystem.ui.GetGameObject(AbTag.DialogDisplay, UIFormName.UIBubbleSelectionItem);
        int count = 4;
        for (int i = 0; i < count; i++)
        {
            RectTransform go = Instantiate(ChoiceButtonPrefab, this.gameObject.transform).GetComponent<RectTransform>();
            go.name = ChoiceButtonPrefab.name + "_" + i;
            go.ResetRectTransform();
            m_uiObjectList.Add(go.GetComponent<UIBubbleSelectionItem>());
            go.gameObject.SetActive(false);
        }

        m_canvasGroup = gameObject.AddMissingComponent<CanvasGroup>();
        m_gridLayoutGroup = gameObject.AddMissingComponent<GridLayoutGroup>();

        m_vCellSize = new Vector2(590, 98);
        m_vSpacing = new Vector2(0, 3);
        m_gridLayoutGroup.cellSize = m_vCellSize;
        m_gridLayoutGroup.spacing = m_vSpacing;
        m_gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
        m_gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
        m_gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
        m_gridLayoutGroup.constraint = GridLayoutGroup.Constraint.Flexible;

        AddOnClickListener();
    }

    public void AddOnClickListener()
    {
        int iCount = m_uiObjectList.Count;
        for (int i = 0; i < iCount; i++)
        {
            UIEventListener.AddOnClickListener(m_uiObjectList[i].gameObject, itemClickHandler);
        }
    }

    public void RemoveOnClickListener()
    {
        if (m_uiObjectList != null)
        {
            int iCount = m_uiObjectList.Count;
            for (int i = 0; i < iCount; i++)
            {
                m_uiObjectList[i].Dispose();
                UIEventListener.RemoveOnClickListener(m_uiObjectList[i].gameObject, itemClickHandler);
                GameObject.Destroy(m_uiObjectList[i].gameObject);
                m_uiObjectList[i] = null;
            }
        }
        m_uiObjectList = null;
    }

    private void itemClickHandler(PointerEventData data)
    {
        UIBubbleSelectionItem selectItem = data.pointerEnter.GetComponent<UIBubbleSelectionItem>();
        if(selectItem != null)
        {
            mIndex = selectItem.Index;
            DoSelectThings();
        }
    }

    public void choicesDialogInit(BaseDialogData dialogData, bool vLeft = true)
    {
        mDialogData = dialogData;
        mMaxNum = dialogData.selection_num;
        
        ChangeDialogDetails(dialogData.selection_num, dialogData.GetSelectionsText(), dialogData.GetSelectionsCost(), dialogData.GetSelectionsHiddenEgg());
        int yNum = -350;
        switch(dialogData.selection_num)
        {
            case 1:
                yNum = -400;
                break;
            case 2:
                yNum = -320;
                break;
            case 3:
                yNum = -250;
                break;
            case 4:
                yNum = -230;
                break;
        }
        ButtonGroupTween(yNum, 0.6f);

        CheckNeedPay();

#if ENABLE_DEBUG
        if (GameDataMgr.Instance.InAutoPlay)
        {
            Invoke("AutoDo", 1f);
        }
#endif
    }

    private void ButtonGroupTween(float anchoredPosition_y, float duration)
    {
        this.m_canvasGroup.transform.localPosition = new Vector3(0, anchoredPosition_y, 0);
        DOTween.To(() => 0f, (alpha) => m_canvasGroup.alpha = alpha, 1f, duration * 0.8f).SetEase(Ease.Flash);
        DOTween.To(() => -100, (spacing_y) => m_gridLayoutGroup.spacing = new Vector2(m_vSpacing.x, spacing_y), m_vSpacing.y, duration).SetEase(Ease.Flash);
    }

    public void ChangeDialogDetails(int count, string[] selections, int[] cost, int[] hiddenEgg)
    {
        int iCount = m_uiObjectList.Count;
        for (int i = 0; i < iCount; i++)
        {
            if (i < count)
            {
                m_uiObjectList[i].Init(selections[i], cost[i], hiddenEgg[i], i);
                m_uiObjectList[i].gameObject.SetActive(true);
            }
            else m_uiObjectList[i].gameObject.SetActive(false);
        }
    }

    private void CheckNeedPay()
    {
        int len = mDialogData.selection_num;
        mNeedPay = false;
        for (int i = 0; i < len; i++)
        {
            mOptionCost = mDialogData.GetSelectionsCost()[i];

            if (mOptionCost > 0)
            {
                if (UserDataManager.Instance.CheckDialogOptionHadCost(UserDataManager.Instance.UserData.CurSelectBookID,
                DialogDisplaySystem.Instance.CurrentBookData.DialogueID, i + 1))
                {
                    mOptionCost = 0;
                }

                if (UserDataManager.Instance.CheckBookHasBuy(UserDataManager.Instance.UserData.CurSelectBookID))
                    mOptionCost = 0;

                if (mOptionCost > 0)
                {
                    mNeedPay = true;
                    break;
                }
            }
        }

        if (mNeedPay)
            EventDispatcher.Dispatch(EventEnum.ResidentMoneyInfo, 1);
    }

#if ENABLE_DEBUG
    private void AutoDo()
    {
        CancelInvoke("AutoDo");
        mIndex = 1;// DialogDisplaySystem.Instance.AutoSelIndex;
        if (mIndex >= mMaxNum)
            mIndex = 0;

        DoSelectThings();
    }
#endif
    private void choiceEvent(PointerEventData data)
    {
        ButtonGroup buttonGroup = data.pointerEnter.GetComponentInParent<ButtonGroup>();
        ButtonGroupSelection buttonGroupSelection = data.pointerEnter.GetComponent<ButtonGroupSelection>();
        mIndex = buttonGroupSelection.Index;

        DoSelectThings();
    }

    private void DoSelectThings()
    {
        mOptionCost = mDialogData.GetSelectionsCost()[mIndex];

        if (mOptionCost > 0)
        {
            if (UserDataManager.Instance.CheckDialogOptionHadCost(UserDataManager.Instance.UserData.CurSelectBookID,
            DialogDisplaySystem.Instance.CurrentBookData.DialogueID, mIndex + 1))
            {
                mOptionCost = 0;
            }

            if (UserDataManager.Instance.CheckBookHasBuy(UserDataManager.Instance.UserData.CurSelectBookID))
                mOptionCost = 0;
        }


        if (UserDataManager.Instance.UserData.DiamondNum < mOptionCost)
        {
            AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
            int type = MyBooksDisINSTANCE.Instance.GameOpenUItype();
            if (type == 1)
            {
            
            }
            else if (type == 2)
            {
                MyBooksDisINSTANCE.Instance.VideoUI();
                return;
            }
            else
            {
                CUIManager.Instance.OpenForm(UIFormName.NewChargeTips);
                NewChargeTips tipForm = CUIManager.Instance.GetForm<NewChargeTips>(UIFormName.NewChargeTips);
                
                if (tipForm != null)
                    tipForm.Init(2, mOptionCost, mOptionCost * 0.99f);

                return;
            }
        }





        if (mOptionCost > 0 && !UserDataManager.Instance.CheckDialogOptionHadCost(UserDataManager.Instance.UserData.CurSelectBookID, mDialogData.dialogID, mIndex + 1))
        {
            AudioManager.Instance.PlayTones(AudioTones.RewardWin);
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.SendPlayerProgress(UserDataManager.Instance.UserData.CurSelectBookID, DialogDisplaySystem.Instance.CurrentBookData.ChapterID
            , mDialogData.dialogID, BookDialogOptionCallBack, mIndex + 1);
        }
        else
        {
            AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
            TurnToOption();
        }
    }

    private void GetNewUserEggRewardHandler(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetNewUserEggRewardHandler---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                int isEnd = 0;
                if (jo.code == 200)
                {
                    UserDataManager.Instance.newUserEggInfo = JsonHelper.JsonToObject<HttpInfoReturn<NewUserEggInfo>>(result);
                    if (UserDataManager.Instance.newUserEggInfo != null)
                    {
                        DoGetHiddenEgg(UserDataManager.Instance.newUserEggInfo.data.bkey, UserDataManager.Instance.newUserEggInfo.data.diamond);
                        isEnd = UserDataManager.Instance.newUserEggInfo.data.is_end;
                    }
                }
                else if (jo.code == 202)
                {
                    isEnd = 1;
                }

                

            }, null);
        }
    }


    private void DoGetHiddenEgg(int vKeyNum, int vDiamondNum)
    {
        mHiddenEggStartPos = Camera.main.WorldToScreenPoint(Input.mousePosition);
        Vector3 targetPos = new Vector3(306, 625);
        RewardShowData rewardShowData = new RewardShowData();
        rewardShowData.StartPos = mHiddenEggStartPos;
        rewardShowData.TargetPos = targetPos;
        rewardShowData.KeyNum = vKeyNum;
        rewardShowData.DiamondNum = vDiamondNum;
        EventDispatcher.Dispatch(EventEnum.HiddenEggRewardShow, rewardShowData);

    }

    //private void GetHiddenEggCallBack(object arg)
    //{
    //    string result = arg.ToString();
    //    LOG.Info("----BookDialogOptionCallBack---->" + result);
    //    JsonObject jo = JsonHelper.JsonToJObject(result);
    //    if (jo != null)
    //    {
    //        LoomUtil.QueueOnMainThread((param) =>
    //        {
    //            if (jo.code == 200)
    //            {
    //                UserDataManager.Instance.SaveStepInfo = JsonHelper.JsonToObject<HttpInfoReturn<SaveStep>>(result);
    //                if (UserDataManager.Instance.SaveStepInfo != null && UserDataManager.Instance.SaveStepInfo.data != null)
    //                {
    //                    DoGetHiddenEgg(UserDataManager.Instance.SaveStepInfo.data.user_key, UserDataManager.Instance.SaveStepInfo.data.user_diamond);
    //                }
    //            }
    //            else if (jo.code == 203)
    //            {
    //                LOG.Error("--GetHiddenEggCallBack--此对话没有彩蛋 DialogId" + DialogDisplaySystem.Instance.CurrentBookData.DialogueID);
    //            }
    //        }, null);
    //    }
    //}

    private void TurnToOption()
    {
        //UINetLoadingMgr.Instance.Show();
        var bookData = BookReadingWrapper.Instance.CurrentBookData;
        GameHttpNet.Instance.SendPlayerProgress(UserDataManager.Instance.UserData.CurSelectBookID,
        DialogDisplaySystem.Instance.CurrentBookData.ChapterID, mDialogData.dialogID,SetProgressHandler);
    }

    private void SetProgressHandler(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----SetProgressHandler---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                UserDataManager.Instance.SaveStepInfo = JsonHelper.JsonToObject<HttpInfoReturn<SaveStep>>(result);
                if (jo.code == 200)
                {
                    //获得普通选项彩蛋
                    int hiddentEgg = mDialogData.GetSelectionsHiddenEgg()[mIndex];
                    if (hiddentEgg > 0)
                    {
                        DoGetHiddenEgg(UserDataManager.Instance.SaveStepInfo.data.user_key, UserDataManager.Instance.SaveStepInfo.data.user_diamond);

                        //GameHttpNet.Instance.SendPlayerProgress(UserDataManager.Instance.UserData.CurSelectBookID, DialogDisplaySystem.Instance.CurrentBookData.ChapterID
                        //, mDialogData.dialogID, GetHiddenEggCallBack, mIndex + 1);
                    }

                    //判断是否，获得新手彩蛋的奖励
                    if (mDialogData.TutorialEgg == 1 /*&& UserDataManager.Instance.newUserEggState != null && UserDataManager.Instance.newUserEggState.data.is_end != 1*/)
                    {
                        DoGetHiddenEgg(UserDataManager.Instance.SaveStepInfo.data.user_key, UserDataManager.Instance.SaveStepInfo.data.user_diamond);

                        //GameHttpNet.Instance.GetDiamondByNewUserEgg(UserDataManager.Instance.UserData.CurSelectBookID, mDialogData.dialogID, GetNewUserEggRewardHandler);
                    }


                    if (mOptionCost > 0 && !UserDataManager.Instance.CheckDialogOptionHadCost(UserDataManager.Instance.UserData.CurSelectBookID, mDialogData.dialogID, mIndex + 1))
                    {
                        UserDataManager.Instance.AddDialogOptionHadCost(UserDataManager.Instance.UserData.CurSelectBookID, mDialogData.dialogID, mIndex + 1);
                    }

                    UserDataManager.Instance.RecordBookOptionSelect(UserDataManager.Instance.UserData.CurSelectBookID, mDialogData.dialogID, mIndex + 1);

                    CheckAddPersonalist();

                    if (mNeedPay)
                        EventDispatcher.Dispatch(EventEnum.ResidentMoneyInfo, 0);
                    EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, mIndex);

                    this.gameObject.SetActive(false);
                }
                else
                {
                    if (!string.IsNullOrEmpty(jo.msg)) UITipsMgr.Instance.PopupTips(jo.msg, false);
                }
            }, null);
        }
    }

    private void CheckAddPersonalist()
    {
        if (mDialogData != null)
        {
            string personalistStr = mDialogData.GetPersonalist(mIndex);
            if (!string.IsNullOrEmpty(personalistStr))
            {
                personalistStr = personalistStr.Replace("##", "_");
                string[] infoList = personalistStr.Split('_');
                int len = infoList.Length;
                for (int i = 0; i < len; i++)
                {
                    if (!string.IsNullOrEmpty(infoList[i]))
                    {
                        string[] keyValue = infoList[i].Split(':');
                        if (keyValue != null && keyValue.Length >= 2)
                        {
                            int key = int.Parse(keyValue[0]);
                            int value = int.Parse(keyValue[1]);
                            int resultValue = 0;
                            string msg = string.Empty;
                            // switch (key)
                            // {
                            //     case 1:
                            //         resultValue = (int)Mathf.Round(value / 0.34f);
                            //         msg = GameDataMgr.Instance.table.GetPersonalityTxtById(key) + " <color='#41b1ff'>+" + resultValue + "</color>";
                            //         break;
                            //     case 2:
                            //         resultValue = (int)Mathf.Round(value / 0.21f);
                            //         msg = GameDataMgr.Instance.table.GetPersonalityTxtById(key) + " <color='#54e778'>+" + resultValue + "</color>";
                            //         break;
                            //     case 3:
                            //         resultValue = (int)Mathf.Round(value / 0.05f);
                            //         msg = GameDataMgr.Instance.table.GetPersonalityTxtById(key) + " <color='#f75acd'>+" + resultValue + "</color>";
                            //         break;
                            //     case 4:
                            //         resultValue = (int)Mathf.Round(value / 0.25f);
                            //         msg = GameDataMgr.Instance.table.GetPersonalityTxtById(key) + " <color='#b555ff'>+" + resultValue + "</color>";
                            //         break;
                            //     case 5:
                            //         resultValue = (int)Mathf.Round(value / 0.15f);
                            //         msg = GameDataMgr.Instance.table.GetPersonalityTxtById(key) + " <color='#f78686'>+" + resultValue + "</color>";
                            //         break;
                            //     default:
                            //         if (key > 5)
                            //         {
                            //             UserDataManager.Instance.SetBookPropertyValue(UserDataManager.Instance.UserData.CurSelectBookID, key, value);
                            //         }
                            //         break;
                            //
                            // }
                            UITipsMgr.Instance.ShowPopTips(msg, Input.mousePosition);//mousePosition是屏幕坐标
                        }
                    }
                }
            }
        }
    }

    private void BookDialogOptionCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----BookDialogOptionCallBack---->" + result);

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.SaveStepInfo = JsonHelper.JsonToObject<HttpInfoReturn<SaveStep>>(result);
                    if (UserDataManager.Instance.SaveStepInfo != null && UserDataManager.Instance.SaveStepInfo.data != null)
                    {
                        int purchase = UserDataManager.Instance.UserData.DiamondNum - UserDataManager.Instance.SaveStepInfo.data.user_diamond;
                        if (purchase > 0)
                            TalkingDataManager.Instance.OnPurchase("Choices Options cost diamond", purchase, 1);
                    }
                    UserDataManager.Instance.OptionCostResultMoneyReset();

                    #region 彩蛋相关
                    var bookData = BookReadingWrapper.Instance.CurrentBookData;
                    //获得普通选项彩蛋
                    int hiddentEgg = mDialogData.GetSelectionsHiddenEgg()[mIndex];
                    if (hiddentEgg > 0)
                    {
                        DoGetHiddenEgg(UserDataManager.Instance.SaveStepInfo.data.user_key, UserDataManager.Instance.SaveStepInfo.data.user_diamond);

                        //GameHttpNet.Instance.BookGetHiddenEgg(UserDataManager.Instance.UserData.CurSelectBookID, DialogDisplaySystem.Instance.CurrentBookData.ChapterID
                        //, mDialogData.dialogID, mIndex + 1, GetHiddenEggCallBack);
                    }

                    //判断是否，获得新手彩蛋的奖励
                    if (mDialogData.TutorialEgg == 1 /*&& UserDataManager.Instance.newUserEggState != null && UserDataManager.Instance.newUserEggState.data.is_end != 1*/)
                    {
                        DoGetHiddenEgg(UserDataManager.Instance.SaveStepInfo.data.user_key, UserDataManager.Instance.SaveStepInfo.data.user_diamond);

                        //GameHttpNet.Instance.GetDiamondByNewUserEgg(UserDataManager.Instance.UserData.CurSelectBookID, mDialogData.dialogID, GetNewUserEggRewardHandler);
                    }


                    if (mOptionCost > 0 && !UserDataManager.Instance.CheckDialogOptionHadCost(UserDataManager.Instance.UserData.CurSelectBookID, mDialogData.dialogID, mIndex + 1))
                    {
                        UserDataManager.Instance.AddDialogOptionHadCost(UserDataManager.Instance.UserData.CurSelectBookID, mDialogData.dialogID, mIndex + 1);
                    }

                    UserDataManager.Instance.RecordBookOptionSelect(UserDataManager.Instance.UserData.CurSelectBookID, mDialogData.dialogID, mIndex + 1);

                    CheckAddPersonalist();

                    if (mNeedPay)
                        EventDispatcher.Dispatch(EventEnum.ResidentMoneyInfo, 0);
                    EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, mIndex);

                    this.gameObject.SetActive(false);

                    #endregion

                }
                else if (jo.code == 202)    //你的钻石不足，请先充值
                {
                    int type = MyBooksDisINSTANCE.Instance.GameOpenUItype();
                    if (type == 1)
                    {

                   
                    }
                    else if (type == 2)
                    {
                        MyBooksDisINSTANCE.Instance.VideoUI();
                        return;
                    }
                    else
                    {
                        CUIManager.Instance.OpenForm(UIFormName.NewChargeTips);
                        NewChargeTips tipForm = CUIManager.Instance.GetForm<NewChargeTips>(UIFormName.NewChargeTips);


                        if (tipForm != null)
                            tipForm.Init(2, mOptionCost, mOptionCost * 0.99f);
                    }
                }else
                {
                    if (!string.IsNullOrEmpty(jo.msg)) UITipsMgr.Instance.PopupTips(jo.msg, false);
                }
               
            }, null);
        }

    }
    
    public void Dispose()
    {
        RemoveOnClickListener();
        ChoiceButtonPrefab = null;
    }

}
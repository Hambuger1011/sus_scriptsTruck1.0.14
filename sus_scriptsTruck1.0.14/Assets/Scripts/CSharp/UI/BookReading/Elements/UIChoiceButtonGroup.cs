
namespace BookReading
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UGUI;

    public class UIChoiceButtonGroup : UIBookReadingElement
    {
        public GameObject ChoiceButtonGroup;

        [System.NonSerialized]
        public GameObject ChoiceButtonPrefab;


        private BookReadingForm _form;

        private int mOptionCost;
        private int mIndex;
        private Vector3 mHiddenEggStartPos;
        private int mMaxNum;
        private bool mNeedPay = false;

        private BaseDialogData mDialogData;
#if NOT_USE_LUA
        public override void Bind(BookReadingForm form)
        {
            _form = form;
            form.choiceButtonGroup = this;
            ChoiceButtonPrefab = AB.ABSystem.ui.GetGameObject(AbTag.DialogDisplay, UIFormName.BookReadingBtnSelectItem);

            this.ChoiceButtonGroup.AddMissingComponent<ButtonGroup>().Init(this.ChoiceButtonPrefab, 4, new Vector2(590, 98), new Vector2(0, 3));
            this.ChoiceButtonGroup.GetComponent<ButtonGroup>().AddOnClickListener(choiceEvent);

            float mScale = 1f;
            if (GameUtility.IpadAspectRatio())
                mScale = 0.7f;

            ChoiceButtonGroup.transform.localScale = Vector3.one * mScale;
        }

        public override void ResetUI()
        {
            this.ChoiceButtonGroup.SetActive(false);
        }

        public override void SetSkin() { }

#region ChoiceButtonGroup
        public void choicesDialogInit(BaseDialogData dialogData, bool vLeft = true,bool inPhone=false)
        {
            mDialogData = dialogData;
            mMaxNum = dialogData.selection_num;
            _form.setBGOnClickListenerActive(false);
            ButtonGroup buttonGroup = this.ChoiceButtonGroup.GetComponent<ButtonGroup>();
            buttonGroup.ChangeDialogDetails(dialogData.selection_num, dialogData.GetSelectionsText(), dialogData.GetSelectionsCost(),dialogData.GetSelectionsHiddenEgg());
            buttonGroup.gameObject.SetActive(true);
            buttonGroup.KillTween();
            buttonGroup.ButtonGroupTween(-220, 0.6f);
            //buttonGroup.gameObject.SetActive(true);
            int yNum = -275;
            int xNum = 50;

            if (inPhone)
                yNum -= 125;

            if (GameUtility.IpadAspectRatio())
            {
                yNum = -130;
                xNum = 150;
            }

            if (vLeft)
                this.ChoiceButtonGroup.transform.localPosition = new Vector3(-1 * xNum, yNum, 0);
            else
                this.ChoiceButtonGroup.transform.localPosition = new Vector3(xNum, yNum, 0);

            CheckNeedPay();

#if ENABLE_DEBUG
            if (GameDataMgr.Instance.InAutoPlay)
            {
                Invoke("AutoDo", 1f);
            }
#endif
        }

        private void CheckNeedPay()
        {
            int len = mDialogData.selection_num;
            mNeedPay = false;
            for (int i = 0; i < len;i++ )
            {
                mOptionCost = DialogDisplaySystem.Instance.CurrentDialogData.GetSelectionsCost()[i];

                if (mOptionCost > 0)
                {
                    if (UserDataManager.Instance.CheckDialogOptionHadCost(UserDataManager.Instance.UserData.CurSelectBookID,
                    DialogDisplaySystem.Instance.CurrentBookData.DialogueID, i + 1))
                    {
                        mOptionCost = 0;
                    }

                    if (UserDataManager.Instance.CheckBookHasBuy(UserDataManager.Instance.UserData.CurSelectBookID))
                        mOptionCost = 0;

                    if(mOptionCost > 0)
                    {
                        mNeedPay = true;
                        break;
                    }
                }
            }
            
            if(mNeedPay)
                EventDispatcher.Dispatch(EventEnum.ResidentMoneyInfo, 1);
        }

#if ENABLE_DEBUG
        private void AutoDo()
        {
            CancelInvoke("AutoDo");
            mIndex = DialogDisplaySystem.Instance.AutoSelIndex;
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
            mOptionCost = DialogDisplaySystem.Instance.CurrentDialogData.GetSelectionsCost()[mIndex];

            if (mOptionCost > 0)
            {
                if (UserDataManager.Instance.CheckDialogOptionHadCost(UserDataManager.Instance.UserData.CurSelectBookID,
                DialogDisplaySystem.Instance.CurrentBookData.DialogueID, mIndex + 1))
                {
                    mOptionCost = 0;
                }

                if(UserDataManager.Instance.CheckBookHasBuy(UserDataManager.Instance.UserData.CurSelectBookID))
                    mOptionCost = 0;
            }


            if (UserDataManager.Instance.UserData.DiamondNum < mOptionCost)
            {
                AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
                int type = MyBooksDisINSTANCE.Instance.GameOpenUItype();
                if (type==1)
                {
                    if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data != null &&
                     UserDataManager.Instance.userInfo.data.userinfo.newpackage_status == 1)
                    {
                        CUIManager.Instance.OpenForm(UIFormName.FirstGigtGroup);
                        CUIManager.Instance.GetForm<FirstGigtGroup>(UIFormName.FirstGigtGroup).GetType(1);
                        return;
                    }
                }else if (type==2)
                {
                    MyBooksDisINSTANCE.Instance.VideoUI();
                    return;
                }else
                {

                    //CUIManager.Instance.OpenForm(UIFormName.ChargeTipsForm);
                    //ChargeTipsForm tipForm = CUIManager.Instance.GetForm<ChargeTipsForm>(UIFormName.ChargeTipsForm);

                    CUIManager.Instance.OpenForm(UIFormName.NewChargeTips);
                    NewChargeTips tipForm = CUIManager.Instance.GetForm<NewChargeTips>(UIFormName.NewChargeTips);


                    if (tipForm != null)
                        tipForm.Init(2, mOptionCost, mOptionCost * 0.99f);

                    return;
                }
            }



            

            if (mOptionCost > 0 && !UserDataManager.Instance.CheckDialogOptionHadCost(UserDataManager.Instance.UserData.CurSelectBookID, DialogDisplaySystem.Instance.CurrentDialogData.dialogID, mIndex + 1))
            {
                AudioManager.Instance.PlayTones(AudioTones.RewardWin);
                //UINetLoadingMgr.Instance.Show();
                GameHttpNet.Instance.BookDialogOptionCost(UserDataManager.Instance.UserData.CurSelectBookID, DialogDisplaySystem.Instance.CurrentBookData.ChapterID
                , DialogDisplaySystem.Instance.CurrentDialogData.dialogID, mIndex + 1, BookDialogOptionCallBack);
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
                        if(UserDataManager.Instance.newUserEggInfo != null)
                        {
                            DoGetHiddenEgg(UserDataManager.Instance.newUserEggInfo.data.bkey, UserDataManager.Instance.newUserEggInfo.data.diamond);
                            isEnd = UserDataManager.Instance.newUserEggInfo.data.is_end;
                        }
                    }
                    else if (jo.code == 202)
                    {
                        isEnd = 1;
                    }

                    if (UserDataManager.Instance.newUserEggState != null)
                        UserDataManager.Instance.newUserEggState.data.is_end = isEnd;

                }, null);
            }
        }
        

        private void DoGetHiddenEgg(int vKeyNum,int vDiamondNum)
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
                         UserDataManager.Instance.hiddenEggInfo = JsonHelper.JsonToObject<HttpInfoReturn<HiddenEggInfo>>(result);
                         if (UserDataManager.Instance.hiddenEggInfo != null && UserDataManager.Instance.hiddenEggInfo.data != null)
                         {
                             DoGetHiddenEgg(UserDataManager.Instance.hiddenEggInfo.data.bkey, UserDataManager.Instance.hiddenEggInfo.data.diamond);
                         }
                     }
                     else if (jo.code == 203)
                     {
                         LOG.Error("--GetHiddenEggCallBack--此对话没有彩蛋 DialogId" + DialogDisplaySystem.Instance.CurrentBookData.DialogueID);
                     }
                 }, null);
            }
        }

        private void TurnToOption()
        {
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.SendPlayerProgress(UserDataManager.Instance.UserData.CurSelectBookID,
            DialogDisplaySystem.Instance.CurrentBookData.ChapterID, DialogDisplaySystem.Instance.CurrentDialogData.dialogID, mIndex + 1,
            0, 0, 0, string.Empty, 0, 0, 0, string.Empty, 0, SetProgressHandler);
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
                    if (jo.code == 200)
                    {
                        //获得普通选项彩蛋
                        int hiddentEgg = DialogDisplaySystem.Instance.CurrentDialogData.GetSelectionsHiddenEgg()[mIndex];
                        if (hiddentEgg > 0)
                        {
                            //if (hiddentEgg == 1)
                            //{
                                GameHttpNet.Instance.BookGetHiddenEgg(UserDataManager.Instance.UserData.CurSelectBookID, DialogDisplaySystem.Instance.CurrentBookData.ChapterID
                                , DialogDisplaySystem.Instance.CurrentDialogData.dialogID, mIndex + 1, GetHiddenEggCallBack);
                            //}
                        }

                        //判断是否，获得新手彩蛋的奖励
                        if (mDialogData.TutorialEgg == 1 && UserDataManager.Instance.newUserEggState != null && UserDataManager.Instance.newUserEggState.data.is_end != 1)
                        {
                            GameHttpNet.Instance.GetDiamondByNewUserEgg(UserDataManager.Instance.UserData.CurSelectBookID, DialogDisplaySystem.Instance.CurrentDialogData.dialogID, GetNewUserEggRewardHandler);
                        }


                        this.ChoiceButtonGroup.SetActive(false);
                        if (mOptionCost > 0 && !UserDataManager.Instance.CheckDialogOptionHadCost(UserDataManager.Instance.UserData.CurSelectBookID, DialogDisplaySystem.Instance.CurrentDialogData.dialogID, mIndex + 1))
                        {
                            UserDataManager.Instance.AddDialogOptionHadCost(UserDataManager.Instance.UserData.CurSelectBookID, DialogDisplaySystem.Instance.CurrentDialogData.dialogID, mIndex + 1);
                        }

                        UserDataManager.Instance.RecordBookOptionSelect(UserDataManager.Instance.UserData.CurSelectBookID, DialogDisplaySystem.Instance.CurrentDialogData.dialogID, mIndex + 1);

                        CheckAddPersonalist();
                        _form.setBGOnClickListenerActive(true);

                        if (mNeedPay)
                            EventDispatcher.Dispatch(EventEnum.ResidentMoneyInfo, 0);
                        EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, mIndex);
                        _form.ResetOperationTime();
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
            if(mDialogData != null)
            {
                string personalistStr = mDialogData.GetPersonalist(mIndex);
                if(!string.IsNullOrEmpty(personalistStr))
                {
                    personalistStr =personalistStr.Replace("##","_");
                    string[] infoList = personalistStr.Split('_');
                    int len = infoList.Length;
                    for(int i = 0;i<len;i++)
                    {
                        if(!string.IsNullOrEmpty(infoList[i]))
                        {
                            string[] keyValue = infoList[i].Split(':');
                            if(keyValue !=null && keyValue.Length>=2)
                            {
                                int key = int.Parse(keyValue[0]);
                                int value = int.Parse(keyValue[1]);
                                int resultValue = 0;
                                string msg = string.Empty;
                                switch(key)
                                {
                                    case 1:
                                        resultValue = (int)Mathf.Round(value / 0.34f);
                                        msg = GameDataMgr.Instance.table.GetPersonalityTxtById(key) + " <color='#41b1ff'>+" + resultValue + "</color>";
                                        break;
                                    case 2:
                                        resultValue = (int)Mathf.Round(value / 0.21f);
                                        msg = GameDataMgr.Instance.table.GetPersonalityTxtById(key) + " <color='#54e778'>+" + resultValue + "</color>";
                                        break;
                                    case 3:
                                        resultValue = (int)Mathf.Round(value / 0.05f);
                                        msg = GameDataMgr.Instance.table.GetPersonalityTxtById(key) + " <color='#f75acd'>+" + resultValue + "</color>";
                                        break;
                                    case 4:
                                        resultValue = (int)Mathf.Round(value / 0.25f);
                                        msg = GameDataMgr.Instance.table.GetPersonalityTxtById(key) + " <color='#b555ff'>+" + resultValue + "</color>";
                                        break;
                                    case 5:
                                        resultValue = (int)Mathf.Round(value / 0.15f);
                                        msg = GameDataMgr.Instance.table.GetPersonalityTxtById(key) + " <color='#f78686'>+" + resultValue + "</color>";
                                        break;
                                    default:
                                        if(key > 5)
                                        {
                                            UserDataManager.Instance.SetBookPropertyValue(UserDataManager.Instance.UserData.CurSelectBookID, key, value);
                                        }
                                        break;
                                    
                                }
                                if(!string.IsNullOrEmpty(msg))
                                {
                                    UITipsMgr.Instance.ShowPopTips(msg, Input.mousePosition);//mousePosition是屏幕坐标
                                    if (UserDataManager.Instance.profileData != null && UserDataManager.Instance.profileData.data != null && UserDataManager.Instance.profileData.data.info != null)//客户端自己统计性格选项的个数
                                        UserDataManager.Instance.profileData.data.info.option_num += 1;
                                }
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
            if(jo != null)
            {
                LoomUtil.QueueOnMainThread((param) =>
                {
                    //UINetLoadingMgr.Instance.Close();
                    if(jo.code == 200)
                    {
                        UserDataManager.Instance.userOptionCostResultInfo = JsonHelper.JsonToObject<HttpInfoReturn<UserOptionCostResultInfo>>(result);
                        if (UserDataManager.Instance.userOptionCostResultInfo != null && UserDataManager.Instance.userOptionCostResultInfo.data != null)
                        {
                            int purchase = UserDataManager.Instance.UserData.DiamondNum - UserDataManager.Instance.userOptionCostResultInfo.data.diamond;
                            if (purchase > 0)
                                TalkingDataManager.Instance.OnPurchase("Choices Options cost diamond", purchase, 1);
                        }
                        UserDataManager.Instance.OptionCostResultMoneyReset();
                        TurnToOption();
                    }
                    else if (jo.code == 202)    //你的选项上一次已扣费
                    {
                        TurnToOption();
                    }
                    else if (jo.code == 203)    //章节免费
                    {
                        TurnToOption();
                    }
                    else if (jo.code == 204)    //对话不存在
                    {
                        TurnToOption();
                    }
                    else if (jo.code == 205)    //你的钻石不足，请先充值
                    {
                        int type = MyBooksDisINSTANCE.Instance.GameOpenUItype();
                        if (type==1)
                        {

                            if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data != null &&
                           UserDataManager.Instance.userInfo.data.userinfo.newpackage_status == 1)
                            {
                                CUIManager.Instance.OpenForm(UIFormName.FirstGigtGroup);
                                CUIManager.Instance.GetForm<FirstGigtGroup>(UIFormName.FirstGigtGroup).GetType(1);
                                return;
                            }
                        }else if (type==2)
                        {
                            MyBooksDisINSTANCE.Instance.VideoUI();

                            return;
                        }else
                        {
                            //CUIManager.Instance.OpenForm(UIFormName.ChargeTipsForm);
                            //ChargeTipsForm tipForm = CUIManager.Instance.GetForm<ChargeTipsForm>(UIFormName.ChargeTipsForm);

                            CUIManager.Instance.OpenForm(UIFormName.NewChargeTips);
                            NewChargeTips tipForm = CUIManager.Instance.GetForm<NewChargeTips>(UIFormName.NewChargeTips);


                            if (tipForm != null)
                                tipForm.Init(2, mOptionCost, mOptionCost * 0.99f);
                        }                   
                    }
                    else if (jo.code == 208)    //扣费失败
                    {
                        LOG.Error("--BookDialogOptionCallBack--扣费失败,BookId:" + UserDataManager.Instance.UserData.CurSelectBookID + 
                            " DialogId:" + DialogDisplaySystem.Instance.CurrentDialogData.dialogID);
                    }
                 }, null);
            }

        }

#endregion

        public override void Dispose()
        {
            if (ChoiceButtonGroup != null)
            {
                this.ChoiceButtonGroup.GetComponent<ButtonGroup>().RemoveOnClickListener(choiceEvent);
                GameObject.Destroy(ChoiceButtonGroup);
            }
            ChoiceButtonGroup = null;
            ChoiceButtonPrefab = null;
        }
#endif
    }
}
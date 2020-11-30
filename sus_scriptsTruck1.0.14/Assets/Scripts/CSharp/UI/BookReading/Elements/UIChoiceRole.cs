
namespace BookReading
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using DG.Tweening;
    using pb;
    using UGUI;

    public class UIChoiceRole : UIBookReadingElement
    {
        public GameObject pfbItem;
        public RectTransform selectRoot;
        public RectTransform unSelectRoot;
        public Button btnComfirm;

        private BookReadingForm _form;

        private int mOptionCost;
        private int mIndex;
        private Vector3 mHiddenEggStartPos;
        private int mMaxNum;
        private bool mNeedPay = false;

        private BaseDialogData mDialogData;

        Vector2[][] itemPosArr;
#if NOT_USE_LUA
        public override void Bind(BookReadingForm form)
        {
            _form = form;
            form.choiceRole = this;
            itemPosArr = new Vector2[3][];
            //2个
            itemPosArr[0] = new Vector2[] {
                new Vector3(-170,280),
                new Vector3(145,0),
            };
            //3个
            itemPosArr[1] = new Vector2[] {
                new Vector3(-180,100),
                new Vector3(135,355),
                new Vector3(160,-90),
            };
            //4个
            itemPosArr[2] = new Vector2[] {
                new Vector3(-165,380),
                new Vector3(180,250),
                new Vector3(-200,-45),
                new Vector3(155,-175),
            };

            btnComfirm.onClick.AddListener(this.DoSelectThings);
            pfbItem.SetActiveEx(false);
        }

        public override void ResetUI()
        {
            this.gameObject.SetActiveEx(false);
        }

        public override void SetSkin() { }

        public override void Dispose()
        {
            //if (ChoiceButtonGroup != null)
            //{
            //    this.ChoiceButtonGroup.GetComponent<ButtonGroup>().RemoveOnClickListener(choiceEvent);
            //    GameObject.Destroy(ChoiceButtonGroup);
            //}
            //ChoiceButtonGroup = null;
            //ChoiceButtonPrefab = null;
        }

#region ChoiceButtonGroup
        public void Show(BaseDialogData dialogData)
        {
            _form.setBGOnClickListenerActive(false);
            this.gameObject.SetActiveEx(true);
            mDialogData = dialogData;
            mMaxNum = dialogData.selection_num;
            _form.setBGOnClickListenerActive(false);
            for(int i= itemList.Count; i< mMaxNum; ++i)
            {
                this.CreateItem(i);
            }
            var headIDs = dialogData.GetSelectionsText();
            for (int i = 0; i < itemList.Count; ++i)
            {
                bool isOn = (i < mMaxNum);
                itemList[i].trans.gameObject.SetActiveEx(isOn);
                if(isOn)
                {
                    itemList[i].SetData(i, this.itemPosArr[mMaxNum - 2][i], headIDs[i]);
                }
            }
            OnChoiceEvent(0);
            //CheckNeedPay();

#if ENABLE_DEBUG
            if (GameDataMgr.Instance.InAutoPlay)
            {
                Invoke("AutoDo", 1f);
            }
#endif
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


        private void OnChoiceEvent(int idx)
        {
            mIndex = idx;
            if (mIndex >= mMaxNum)
                mIndex = 0;

            for(int i=0;i<this.itemList.Count;++i)
            {
                this.itemList[i].Select(i == mIndex);
            }
        }

#endregion

#region 选择事件

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

                if (UserDataManager.Instance.CheckBookHasBuy(UserDataManager.Instance.UserData.CurSelectBookID))
                    mOptionCost = 0;
            }


            if (UserDataManager.Instance.UserData.DiamondNum < mOptionCost)
            {
                AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
                int type = MyBooksDisINSTANCE.Instance.GameOpenUItype();
                if (type == 1)
                {
                    if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data != null &&
                     UserDataManager.Instance.userInfo.data.userinfo.newpackage_status == 1)
                    {
                        CUIManager.Instance.OpenForm(UIFormName.FirstGigtGroup);
                        CUIManager.Instance.GetForm<FirstGigtGroup>(UIFormName.FirstGigtGroup).GetType(1);
                        return;
                    }
                }
                else if (type == 2)
                {
                    MyBooksDisINSTANCE.Instance.VideoUI();
                    return;
                }
                else
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



            int hiddentEgg = DialogDisplaySystem.Instance.CurrentDialogData.GetSelectionsHiddenEgg()[mIndex];
            if (hiddentEgg > 0)
            {
                if (hiddentEgg == 1)
                {
                    GameHttpNet.Instance.BookGetHiddenEgg(UserDataManager.Instance.UserData.CurSelectBookID, DialogDisplaySystem.Instance.CurrentBookData.ChapterID
                    , DialogDisplaySystem.Instance.CurrentDialogData.dialogID, mIndex + 1, GetHiddenEggCallBack);
                }
            }


            //判断是否，获得新手彩蛋的奖励
            if (mDialogData.TutorialEgg == 1 && UserDataManager.Instance.newUserEggState != null && UserDataManager.Instance.newUserEggState.data.is_end != 1)
            {
                GameHttpNet.Instance.GetDiamondByNewUserEgg(UserDataManager.Instance.UserData.CurSelectBookID, DialogDisplaySystem.Instance.CurrentDialogData.dialogID, GetNewUserEggRewardHandler);
            }

            if (mOptionCost > 0 && !UserDataManager.Instance.CheckDialogOptionHadCost(UserDataManager.Instance.UserData.CurSelectBookID, DialogDisplaySystem.Instance.CurrentDialogData.dialogID, mIndex + 1))
            {
                AudioManager.Instance.PlayTones(AudioTones.RewardWin);
#if USE_SERVER_DATA
                //UINetLoadingMgr.Instance.Show();
                GameHttpNet.Instance.BookDialogOptionCost(UserDataManager.Instance.UserData.CurSelectBookID, DialogDisplaySystem.Instance.CurrentBookData.ChapterID
                , DialogDisplaySystem.Instance.CurrentDialogData.dialogID, mIndex + 1, BookDialogOptionCallBack);
#else
                UserDataManager.Instance.CalculateDiamondNum(-mOptionCost);
                TurnToOption();
#endif
            }
            else
            {
                AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
                TurnToOption();
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
            this.gameObject.SetActiveEx(false);

            int needPay = 0;
            if (mOptionCost > 0 && !UserDataManager.Instance.CheckDialogOptionHadCost(UserDataManager.Instance.UserData.CurSelectBookID, DialogDisplaySystem.Instance.CurrentDialogData.dialogID, mIndex + 1))
            {
                needPay = 1;
                UserDataManager.Instance.AddDialogOptionHadCost(UserDataManager.Instance.UserData.CurSelectBookID, DialogDisplaySystem.Instance.CurrentDialogData.dialogID, mIndex + 1);
            }

            CheckAddPersonalist();

            _form.setBGOnClickListenerActive(true);

            //TalkingDataManager.Instance.SelectOptions(DialogDisplaySystem.Instance.CurrentBookData.BookID, DialogDisplaySystem.Instance.CurrentBookData.DialogueID, mIndex + 1, needPay, mOptionCost);
            if (mNeedPay)
                EventDispatcher.Dispatch(EventEnum.ResidentMoneyInfo, 0);
            EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, mIndex);


            _form.ResetOperationTime();
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

                    if (UserDataManager.Instance.newUserEggState != null)
                        UserDataManager.Instance.newUserEggState.data.is_end = isEnd;

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
                                switch (key)
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
                                }
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
                        if (type == 1)
                        {

                            if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data != null &&
                           UserDataManager.Instance.userInfo.data.userinfo.newpackage_status == 1)
                            {
                                CUIManager.Instance.OpenForm(UIFormName.FirstGigtGroup);
                                CUIManager.Instance.GetForm<FirstGigtGroup>(UIFormName.FirstGigtGroup).GetType(1);
                                return;
                            }
                        }
                        else if (type == 2)
                        {
                            MyBooksDisINSTANCE.Instance.VideoUI();

                            return;
                        }
                        else
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



        List<Item> itemList = new List<Item>();
        void CreateItem(int idx)
        {
            var go = GameObject.Instantiate(this.pfbItem, this.unSelectRoot);
            var trans = go.transform as RectTransform;
            var item = new Item(trans, selectRoot, unSelectRoot);
            itemList.Add(item);
            go.GetComponent<Button>().onClick.AddListener(()=>OnChoiceEvent(item.index));
        }


        public class Item
        {
            public RectTransform trans;
            public RectTransform selectRoot;
            public RectTransform unSelectRoot;
            public Image imgRole;

            public bool isOn = false;
            public int index = 0;

            public Item(RectTransform root, RectTransform selectRoot, RectTransform unSelectRoot)
            {
                this.trans = root;
                this.selectRoot = selectRoot;
                this.unSelectRoot = unSelectRoot;
                this.imgRole = this.trans.Find("imgRole").GetComponent<Image>();
                this.isOn = false;
                UpdateUI();
            }

            public void Select(bool isOn)
            {
                if(this.isOn == isOn)
                {
                    return;
                }
                this.isOn = isOn;
                UpdateUI();
            }

            void UpdateUI()
            {

                if (this.isOn)
                {
                    this.trans.SetParent(this.selectRoot, false);
                }
                else
                {
                    this.trans.SetParent(this.unSelectRoot, false);
                }
            }

            public void SetData(int idx, Vector2 pos, string strRoleID)
            {
                index = idx;
                this.trans.anchoredPosition = pos;

                var roleID = int.Parse(strRoleID);
                var clothesID = 1;
                int appearanceID = (100000 + (roleID * 100) + clothesID) * 10000 + 0;
                var sptHead = DialogDisplaySystem.Instance.GetUITexture("RoleHead/" + appearanceID, false);
                this.imgRole.sprite = sptHead;
            }
        }
#endif
    }

}
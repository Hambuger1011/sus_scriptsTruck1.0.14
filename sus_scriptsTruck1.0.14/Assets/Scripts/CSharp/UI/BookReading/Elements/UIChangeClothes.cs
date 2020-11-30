
namespace BookReading
{
    using DG.Tweening;
    using pb;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using UGUI;

    public class UIChangeClothes : UIBookReadingElement
    {
        public enum E_BuyType
        {
            Free,
            Diamonds,
            Video,
        }

        public GameObject ChangeClothes;
        public GameObject ChangeClothesGroup;
        
        public Button ChangeClothesComfirmButton;
        public GameObject ChangeClothesDiamondGroup;
        public Text ChangeClothesComfirmButtonCost;
        public Text ChangeClothesComfirmDescTxt;
        public Button ChangeClothesDiamondAddButton;
        public Text ChangeClothesDetails;
        public GameObject SelectClothsGo;
        public GameObject ComfirmEffectGo;

        [Header("-------------skin-------------")]
        public Image imgClotheDetails;
        public Image btnBg;
        public GameObject ChangeClothesLeftButton;
        public GameObject ChangeClothesRightButton;

        private BookReadingForm _form;

        private E_BuyType buyType;
        private int cost;
        private int mChoicesClothId;
        private int mNeedPay;
#if NOT_USE_LUA
        public override void Bind(BookReadingForm form)
        {
            _form = form;
            form.changeClothes = this;
            UIEventListener.AddOnClickListener(this.ChangeClothesLeftButton, changeClothesMoveButton);
            UIEventListener.AddOnClickListener(this.ChangeClothesRightButton, changeClothesMoveButton);
            UIEventListener.AddOnClickListener(this.ChangeClothesComfirmButton.gameObject, changeClothesComfirmButton);

            if (GameUtility.IpadAspectRatio() && ChangeClothes != null)
                ChangeClothes.transform.localScale = Vector3.one * 0.7f;

        }
        public override void SetSkin()
        {
            imgClotheDetails.sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/ChangeClothes/bg.png");
            Image leftBtn = ChangeClothesLeftButton.GetComponent<Image>();
            Image rightBtn =  ChangeClothesRightButton.GetComponent<Image>();
            leftBtn.sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/ChangeClothes/btn_last.png");
            rightBtn.sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/ChangeClothes/btn_next.png");
            leftBtn.SetNativeSize();
            rightBtn.SetNativeSize();

            bool isNotFree = (cost > 0 && buyType != E_BuyType.Free);
            ComfirmEffectGo.SetActive(isNotFree);
            ChangeClothesDiamondGroup.SetActive(isNotFree);
            if(isNotFree)
            {
                btnBg.sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/ChangeClothes/bg_btn.png");
                ChangeClothesComfirmDescTxt.rectTransform.anchoredPosition = new Vector2(-52, 4);
                switch(buyType)
                {
                    case E_BuyType.Video:
                        AB.ABSystem.ui.SetAtlasSprite(ChangeClothesDiamondGroup.GetComponent<Image>(), "BookReadingForm", "icon_video");
                        break;
                    default:
                        AB.ABSystem.ui.SetAtlasSprite(ChangeClothesDiamondGroup.GetComponent<Image>(), "BookReadingForm", "icon_dimon");
                        break;
                }
            }else
            {
                btnBg.sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/ChangeClothes/bg_btn2.png");
                ChangeClothesComfirmDescTxt.rectTransform.anchoredPosition = new Vector2(0, 4);
            }
            ChangeClothesComfirmButtonCost.text = cost.ToString();
        }

        public override void ResetUI()
        {
            this.ChangeClothes.SetActive(false);
            //_form.ChangeClothesImage.gameObject.SetActive(false);
            this.SelectClothsGo.SetActive(false);
        }

        
        public void Show(BaseDialogData dialogData)
        {
            this.SelectClothsGo.SetActive(false);
            this.ChangeClothes.SetActive(true);
            _form.BgAddBlurEffect(true);
            //_form.ChangeClothesImage.gameObject.SetActive(true);
            //_form.ChangeClothesImage.color = new Color(1, 1, 1, 0);
            //_form.ChangeClothesImage.DOFade(1, 0.3f).SetEase(Ease.Linear).OnStart(() => { _form.setBGOnClickListenerActive(false); }).Play();
            setChangeClothesDetails(dialogData);
            _form.setBGOnClickListenerActive(false);
        }

#region ChangeClothesMethods
        private int changeClothesIndex = -1;
        private int changeClothesCount = -1;
        private void changeClothesMoveButton(PointerEventData data)
        {
            if (_form.IsTweening) return;
            AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
            int lastIndex = changeClothesIndex;
            if (data.pointerPress.gameObject.name.Equals("LeftButton"))
            {
                changeClothesIndex = --changeClothesIndex % changeClothesCount;
                if (changeClothesIndex < 0) changeClothesIndex = changeClothesCount - 1;
            }
            else
            {

                changeClothesIndex = ++changeClothesIndex % changeClothesCount;
            }
            {
                float duration = 0.6f;
                Image[] images = this.ChangeClothesGroup.GetComponentsInChildren<Image>(true);
                images[lastIndex].color = new Color(1, 1, 1, 1);
                images[changeClothesIndex].color = new Color(1, 1, 1, 0);
                images[lastIndex].DOFade(0, duration).SetEase(Ease.Flash).OnStart(() => _form.IsTweening = true);
                images[changeClothesIndex].DOFade(1, duration).SetEase(Ease.Flash)
                    .OnStart(() => { images[changeClothesIndex].gameObject.SetActive(true); })
                    .OnComplete(() => { _form.IsTweening = false; images[lastIndex].gameObject.SetActive(false); });
                updateSelectCloths();
                this.SelectClothsGo.SetActive(false);
                this.SelectClothsGo.SetActive(true);
            }
            _form.ResetOperationTime();
        }

        private void updateSelectCloths()
        {
            buyType = E_BuyType.Free;
            mChoicesClothId = int.Parse(DialogDisplaySystem.Instance.CurrentDialogData.GetSelectionsText()[changeClothesIndex]);
            int appearanceID = (100000 + (1 * 10000) + 100 + mChoicesClothId) * 10000;
            t_Skin skin = GameDataMgr.Instance.table.GetSkinById(DialogDisplaySystem.Instance.CurrentBookData.BookID, appearanceID);
            if (skin != null) this.ChangeClothesDetails.text = skin.dec;
#if USE_SERVER_DATA
            if (!UserDataManager.Instance.CheckClothHadCost(UserDataManager.Instance.UserData.CurSelectBookID, mChoicesClothId))
#else
            if (!DialogDisplaySystem.Instance.CheckHasThisCloth(appearanceID))
#endif
            {
                //LOG.Info("BookId:"+ DialogDisplaySystem.Instance.CurrentBookData.BookID+"--ClotheId:"+ mChoicesClothId);
                var cfg = GameDataMgr.Instance.table.GetClothePriceById(DialogDisplaySystem.Instance.CurrentBookData.BookID, mChoicesClothId);
                if (cfg != null)
                {
                    buyType = (E_BuyType)cfg.PriceType;
                    cost = cfg.ClothePrice;
                    if(buyType == E_BuyType.Video)
                    {
                        int seeVideoNum = UserDataManager.Instance.GetSeeVideoNumOfClothes(DialogDisplaySystem.Instance.CurrentBookData.BookID, mChoicesClothId);
                        cost -= seeVideoNum;
                        if(cost < 0)
                        {
                            cost = 0;
                        }
                    }
                }else
                {
                    buyType = E_BuyType.Free;
                    cost = 0;
                }

                if (UserDataManager.Instance.CheckBookHasBuy(DialogDisplaySystem.Instance.CurrentBookData.BookID))
                {
                    cost = 0;
                }
                //cost = DialogDisplaySystem.Instance.CurrentDialogData.GetSelectionsCost()[changeClothesIndex];
            }
            else
            {
                buyType = E_BuyType.Free;
                cost = 0;
            }
            SetSkin();
        }

        /// <summary>
        /// 点击购买按钮
        /// </summary>
        private void changeClothesComfirmButton(PointerEventData data)
        {
            mChoicesClothId = int.Parse(DialogDisplaySystem.Instance.CurrentDialogData.GetSelectionsText()[changeClothesIndex]);
            int appearanceID = (100000 + (1 * 10000) + 100 + mChoicesClothId) * 10000;

            mNeedPay = 0;

            switch (buyType)
            {
                case E_BuyType.Video:
                    {
#if CHANNEL_SPAIN
                        int seeVideoNum = UserDataManager.Instance.GetSeeVideoNumOfClothes(DialogDisplaySystem.Instance.CurrentBookData.BookID, mChoicesClothId);
                        if (cost > 0 )
                        {
                            SdkMgr.Instance.ads.ShowRewardBasedVideo((bSuc) =>
                            {
                                if(!bSuc)
                                {
                                    return;
                                }
                                seeVideoNum += 1;
                                UserDataManager.Instance.SetSeeVideoNumOfClothes(DialogDisplaySystem.Instance.CurrentBookData.BookID, mChoicesClothId, seeVideoNum);
                                updateSelectCloths();
                            });
                        }else
                        {
                            if (!UserDataManager.Instance.CheckClothHadCost(UserDataManager.Instance.UserData.CurSelectBookID, mChoicesClothId))
                            {
                                mNeedPay = 1;
                                //UINetLoadingMgr.Instance.Show();
                                GameHttpNet.Instance.BookChoicesClothCost(UserDataManager.Instance.UserData.CurSelectBookID, 1, mChoicesClothId, ChoicesClothCostCallBack);
                            }
                            else
                            {
                                AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
                                DoChoicesCloth();
                            }
                        }
#else
                        MyBooksDisINSTANCE.Instance.VideoUI();
#endif
                    }
                    break;
                case E_BuyType.Diamonds:
                    {

                        if (UserDataManager.Instance.UserData.DiamondNum < cost)//检测钻石是否充足
                        {
                            AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);

                            int Type = MyBooksDisINSTANCE.Instance.GameOpenUItype();

                            if (Type == 1)
                            {
                                if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data != null &&
                                     UserDataManager.Instance.userInfo.data.userinfo.newpackage_status == 1)
                                {
                                    CUIManager.Instance.OpenForm(UIFormName.FirstGigtGroup);
                                    CUIManager.Instance.GetForm<FirstGigtGroup>(UIFormName.FirstGigtGroup).GetType(1);
                                    return;
                                }
                            }
                            else if (Type == 2)
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
                                    tipForm.Init(2, cost, cost * 0.99f);
                                return;
                            }
                        }
#if USE_SERVER_DATA
                        if (cost > 0 && !UserDataManager.Instance.CheckClothHadCost(UserDataManager.Instance.UserData.CurSelectBookID, mChoicesClothId))
                        {
                            mNeedPay = 1;
                            //UINetLoadingMgr.Instance.Show();
                            GameHttpNet.Instance.BookChoicesClothCost(UserDataManager.Instance.UserData.CurSelectBookID, 1, mChoicesClothId, ChoicesClothCostCallBack);
                        }
                        else
                        {
                            AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
                            DoChoicesCloth();
                        }
#else
            UserDataManager.Instance.CalculateDiamondNum(-cost);
            DoChoicesCloth();
#endif
                    }
                    break;
                default:
                    {
                        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
                        DoChoicesCloth();
                    }
                    break;
            }

 
            
        }

       



        private void ChoicesClothCostCallBack(object arg)
        {
            string result = arg.ToString();
            LOG.Info("----ChoicesClothCostCallBack---->" + result);
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo != null)
            {
                 LoomUtil.QueueOnMainThread((param) => {
                     //UINetLoadingMgr.Instance.Close();
                    if (jo.code == 200)
                    {
                        AudioManager.Instance.PlayTones(AudioTones.RewardWin);

                        UserDataManager.Instance.choicesClothResultInfo = JsonHelper.JsonToObject<HttpInfoReturn<ChoicesClothResultInfo>>(result);

                        if (UserDataManager.Instance.choicesClothResultInfo != null && UserDataManager.Instance.choicesClothResultInfo.data != null)
                        {
                            int purchase = UserDataManager.Instance.UserData.DiamondNum - UserDataManager.Instance.choicesClothResultInfo.data.diamond;
                            if (purchase > 0)
                                TalkingDataManager.Instance.OnPurchase("ChoicesCloth cost diamond", purchase, 1);

                            UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.choicesClothResultInfo.data.bkey);
                            UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.choicesClothResultInfo.data.diamond);
                        }


                        DialogDisplaySystem.Instance.CurrentBookData.PlayerClothes = mChoicesClothId;
#if USE_SERVER_DATA
                        int appearanceID = (100000 + (1 * 10000) + 100 + mChoicesClothId) * 10000;
                        UserDataManager.Instance.AddClothAfterPay(UserDataManager.Instance.UserData.CurSelectBookID, mChoicesClothId);
#endif
                        DoChoicesCloth();
                    }
                    else if (jo.code == 202 || jo.code == 203 || jo.code == 204)
                    {
                        AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                        DoChoicesCloth();
                    }
                    else if (jo.code == 206)
                    {
                        AudioManager.Instance.PlayTones(AudioTones.LoseFail);

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
                                 tipForm.Init(2, cost, cost * 0.99f);
                             return;
                         }                     
                    }
                }, null);
            }
        }


        private void DoChoicesCloth()
        {
            LOG.Info("穿衣服");
            GameHttpNet.Instance.SendPlayerProgress(UserDataManager.Instance.UserData.CurSelectBookID,
                DialogDisplaySystem.Instance.CurrentBookData.ChapterID, DialogDisplaySystem.Instance.CurrentBookData.DialogueID,
                changeClothesIndex + 1, 0, 0, mChoicesClothId, string.Empty, 0, 0, 0, string.Empty, 0, SetProgressHandler);
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
                        DialogDisplaySystem.Instance.CurrentBookData.PlayerClothes = mChoicesClothId;
                        this.ChangeClothes.SetActive(false);
                        UserDataManager.Instance.RecordBookOptionSelect(UserDataManager.Instance.UserData.CurSelectBookID, DialogDisplaySystem.Instance.CurrentBookData.DialogueID, changeClothesIndex + 1);
                        _form.BgAddBlurEffect(false);
                        _form.setBGOnClickListenerActive(true);
                        EventDispatcher.Dispatch(EventEnum.ResidentMoneyInfo, 0);
                        EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, changeClothesIndex);
                        _form.ResetOperationTime();
                        TalkingDataManager.Instance.SelectCloths(UserDataManager.Instance.UserData.CurSelectBookID, DialogDisplaySystem.Instance.CurrentBookData.DialogueID, mChoicesClothId, mNeedPay, cost);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(jo.msg)) UITipsMgr.Instance.PopupTips(jo.msg, false);
                    }
                }, null);
            }

        }

        private void setChangeClothesDetails(BaseDialogData dialogData)
        {
            EventDispatcher.Dispatch(EventEnum.ResidentMoneyInfo, 1);
            changeClothesIndex = 0;
            changeClothesCount = dialogData.selection_num;
            ChangeClothesLeftButton.SetActive(changeClothesCount > 1);
            ChangeClothesRightButton.SetActive(changeClothesCount > 1);
            this.ChangeClothesGroup.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            setClothesDetails(dialogData.selection_num, dialogData.selection_1, dialogData.selection_2, dialogData.selection_3, dialogData.selection_4);
            updateSelectCloths();

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
            changeClothesIndex = DialogDisplaySystem.Instance.AutoSelIndex;
            if (changeClothesIndex >= changeClothesCount)
                changeClothesIndex = 0;

            updateSelectCloths();
            changeClothesComfirmButton(null);
        }
#endif
        private void setClothesDetails(int count, params string[] ClothesIDs)
        {
            Image[] imageGroup = this.ChangeClothesGroup.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < ClothesIDs.Length; i++)
            {
                if (i < count)
                {
                    int appearanceID = (100000 + (1 * 10000) + 100 + int.Parse(ClothesIDs[i])) * 10000;
                    imageGroup[i].sprite = DialogDisplaySystem.Instance.GetUITexture("RoleClothes/" + appearanceID, false);
                }
                imageGroup[i].color = Color.white;
                imageGroup[i].gameObject.SetActive(false);
            }
            imageGroup[0].gameObject.SetActive(true);
        }
#endregion

        public override void Dispose()
        {
            UIEventListener.RemoveOnClickListener(this.ChangeClothesLeftButton, changeClothesMoveButton);
            UIEventListener.RemoveOnClickListener(this.ChangeClothesRightButton, changeClothesMoveButton);
            UIEventListener.RemoveOnClickListener(this.ChangeClothesComfirmButton.gameObject, changeClothesComfirmButton);


            Image[] imageGroup = this.ChangeClothesGroup.GetComponentsInChildren<Image>(true);
            if(imageGroup != null)
            {
                int len = imageGroup.Length;
                for (int i = 0; i < len; i++)
                {
                    imageGroup[i].sprite = null;
                }
            }
            imgClotheDetails.sprite = null;
            Image leftBtn = ChangeClothesLeftButton.GetComponent<Image>();
            Image rightBtn = ChangeClothesRightButton.GetComponent<Image>();
            leftBtn.sprite = null;
            rightBtn.sprite = null;
            btnBg.sprite = null;
        }
#endif

    }
}

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
    using Spine.Unity;
    /// <summary>
    /// 换装、换全身
    /// </summary>
    public class UIChangeWholeClothes : UIBookReadingElement
    {
        public enum E_BuyType
        {
            Free,
            Diamonds,
            Video,
        }

        public GameObject ChangeClothes;
        public GameObject ChangeClothesGroup;

        public Image ClothesImage;
        public Button ChangeClothesComfirmButton;
        public GameObject ChangeClothesDiamondGroup;
        public Text ChangeClothesComfirmButtonCost;
        public Text ChangeClothesComfirmDescTxt;
        public Button ChangeClothesDiamondAddButton;
        public Text ChangeClothesDetails;
        public GameObject SelectClothsGo;
        public GameObject ComfirmEffectGo;
        public Image ScreenFlashImage;
        public Image DownIcon;
        public GameObject CharageEffectGo;

        [Header("-------------skin-------------")]
        public Image imgClotheDetails;
        public Image btnBg;
        public GameObject ChangeClothesLeftButton;
        public GameObject ChangeClothesRightButton;

        [Header("-------------SheletonGraphic-------------")]
        public SkeletonGraphic RoleSkeGraphic;
        public BoneFollowerGraphic HeadBoneFolGraphic;
        public SkeletonGraphic ExpressionSkeGraphic;
        public SkeletonGraphic HairSkeGraphic;

        private BookReadingForm _form;

        private Image mLeftBtn;
        private Image mRightBtn;
        private List<Sprite> mSpriteList;
        private List<int> mClothsList;

        private E_BuyType buyType;
        private int cost;
        private int mChoicesClothId;
        private int mNeedPay;
        private int mTriggerId;     //1：表示选主角的形象，2：表示选主角的服装，3：表示选NPC的形象
        private int mSceneWidth;
        private bool mIsSelected = false;
        private bool mInHideState = false;
        private float mRoleScale = 1f;
        private float mScale = 1f;
        private Vector3 mDownPosUp = Vector3.zero;
        private Vector3 mDownPowDo = Vector3.zero;

        private BaseDialogData mDialogData;
#if NOT_USE_LUA
        private bool isSetPos = false;
        private int appearanceIDCache = 0;
        private int clothIdCache = -1;

        public override void Bind(BookReadingForm form)
        {
            _form = form;
            form.changeWholeClothes = this;
            UIEventListener.AddOnClickListener(this.ChangeClothesLeftButton, TurnLeftHandler);
            UIEventListener.AddOnClickListener(this.ChangeClothesRightButton, TurnRightHandler);
            UIEventListener.AddOnClickListener(this.DownIcon.gameObject, HideDetailInfoHandler);
            UIEventListener.AddOnClickListener(this.ScreenFlashImage.gameObject, ShowDetailInfoHandler);
            UIEventListener.AddOnClickListener(this.ChangeClothesComfirmButton.gameObject, changeClothesComfirmButton);

            mLeftBtn = ChangeClothesLeftButton.GetComponent<Image>();
            mRightBtn = ChangeClothesRightButton.GetComponent<Image>();

            if (GameUtility.IpadAspectRatio() && ChangeClothes != null)
            {
                mScale = 0.7f;
                mDownPosUp = new Vector3(192, 230);
                mDownPowDo = new Vector3(338, 100);
            }
            else 
            {
                mScale = 1f;
                mDownPosUp = new Vector3(270, 295);
                mDownPowDo = new Vector3(332, 115);
            }
            mRoleScale = mScale;
            if (GameUtility.IsLongScreen())
                mRoleScale = 1.1f;

            ClothesImage.transform.localScale = Vector3.one * mRoleScale;
            imgClotheDetails.transform.localScale = Vector3.one * mScale;
        }

        private void HideDetailInfoHandler(PointerEventData data)
        {
            MoveLeftOrRightBtn();
            DownIcon.raycastTarget = false;
            DownIcon.rectTransform().anchoredPosition = mDownPosUp;
            DownIcon.rectTransform().DOAnchorPos(mDownPowDo, 0.4f).SetEase(Ease.InQuart).OnComplete(() => 
            {
                DownIcon.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
            }).Play();
            imgClotheDetails.transform.DOScaleY(0, 0.4f).SetEase(Ease.InQuart).OnComplete(() =>
            {
                ScreenFlashImage.color = new Color(1, 1, 1, 0);
                ScreenFlashImage.gameObject.SetActive(true);
                ScreenFlashImage.raycastTarget = true;
                mInHideState = true;
            }).Play();
        }

        private void ShowDetailInfoHandler(PointerEventData data)
        {
            if(mInHideState)
            {
                MoveLeftOrRightBtn(false);
                MoveDetailFrame(true, false);
                ScreenFlashImage.raycastTarget = false;
                ScreenFlashImage.gameObject.SetActive(false); 
                DownIcon.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                DownIcon.rectTransform().DOAnchorPos(mDownPosUp, 0.4f).SetEase(Ease.OutQuint).Play();
                DownIcon.raycastTarget = true;
            }
        }

        private void TurnLeftHandler(PointerEventData data)
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
            DoTurnMove(false);
        }

        private void TurnRightHandler(PointerEventData data)
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
            DoTurnMove(true);
        }

        private void DoTurnMove(bool isRight = true)
        {
           
            this.SelectClothsGo.SetActive(false);
            if (CharageEffectGo != null)
                CharageEffectGo.SetActive(false);
            int moveTargetX = (isRight) ? -mSceneWidth : mSceneWidth;
            MoveLeftOrRightBtn();
            MoveDetailFrame(isRight);
            ClothesImage.transform.DOLocalMoveX(moveTargetX, 0.6f).OnComplete(() => 
            {
                //ClothesImage.sprite = mSpriteList[changeClothesIndex];
                switch(mTriggerId) //1：表示选主角的形象，2：表示选主角的服装，3：表示选NPC的形象
                {
                    case 1:
                        SetSkinName(changeClothesIndex + 1);
                        break;
                    case 2:
                        UpdateAtlas(changeClothesIndex);
                        break;
                    case 3:
                        int appearanceId = mClothsList[changeClothesIndex];
                        if(appearanceId == appearanceIDCache)
                        {
                            int clothId = int.Parse(mDialogData.GetSelectionsText()[changeClothesIndex]);
                            int skin = clothId % 3;
                            SetSkinName(skin);
                        }else
                        {
                            UpdateAtlas(changeClothesIndex);
                            isSetPos = false;
                            ResetPos();
                        }
                        break;
                }
                int startX = (isRight) ?  mSceneWidth:-mSceneWidth;
                ClothesImage.transform.localPosition = new Vector3(startX, ClothesImage.transform.localPosition.y, 0);
                ClothesImage.transform.DOLocalMoveX(0, 0.6f).SetEase(Ease.OutQuint).OnComplete(() => 
                {
                    MoveDetailFrame(isRight, false);
                }).Play();
                MoveLeftOrRightBtn(false);
                updateSelectCloths();
            }).SetEase(Ease.InQuad).Play();

            _form.ResetOperationTime();
        }

        private void MoveLeftOrRightBtn(bool isOut = true)
        {
            if(isOut)
            {
                mLeftBtn.transform.DOLocalMoveX(-mSceneWidth, 0.3f).SetEase(Ease.InQuint).Play();
                mRightBtn.transform.DOLocalMoveX(mSceneWidth, 0.3f).SetEase(Ease.InQuint).Play();
            }else
            {
                mLeftBtn.transform.localScale = Vector3.zero;
                mRightBtn.transform.localScale = Vector3.zero;
                mLeftBtn.transform.localPosition = new Vector3(-300, -15, 0);
                mRightBtn.transform.localPosition = new Vector3(300, -14, 0);
                mLeftBtn.transform.DOLocalMoveX(-320, 0.3f).SetDelay(0.15f).SetEase(Ease.OutBack).Play();
                mRightBtn.transform.DOLocalMoveX(320, 0.3f).SetDelay(0.15f).SetEase(Ease.OutBack).Play();
                mLeftBtn.transform.DOScale(1, 0.3f).Play();
                mRightBtn.transform.DOScale(1, 0.3f).Play();
            }
        }

        private void MoveDetailFrame(bool isRight = true, bool isOut = true)
        {
            if (isOut)
            {
                int moveTargetX = (isRight) ? -mSceneWidth : mSceneWidth;
                imgClotheDetails.transform.DOLocalMoveX(moveTargetX, 0.6f).OnComplete(() => { imgClotheDetails.gameObject.SetActive(false); }).Play();
                DownIcon.rectTransform().anchoredPosition = mDownPosUp;
                DownIcon.transform.DOLocalMoveX(moveTargetX, 0.6f).OnComplete(() =>
                {
                    DownIcon.gameObject.SetActive(false);
                }).Play();
            }
            else
            {
                imgClotheDetails.transform.localPosition = new Vector3(0, imgClotheDetails.transform.localPosition.y, 0);
                imgClotheDetails.transform.localScale = new Vector3(1f * mScale, 0, 1 * mScale);
                DownIcon.transform.localScale = new Vector3(1 * mScale, 0, 1 * mScale);
                ChangeClothesDetails.color = new Color(0.2f, 0.2f, 0.2f, 0);
                btnBg.transform.localScale = Vector3.zero;
                imgClotheDetails.gameObject.SetActive(true);
                imgClotheDetails.transform.DOScaleY(1 * mScale, 0.4f).SetEase(Ease.OutBack).OnComplete(() => 
                {
                    DownIcon.rectTransform().anchoredPosition = mDownPosUp;
                    DownIcon.gameObject.SetActive(true);
                    DownIcon.transform.DOScaleY(1 * mScale, 0.4f).Play();
                    ChangeClothesDetails.DOColor(new Color(0.2f, 0.2f, 0.2f, 1), 0.3f).Play();
                    btnBg.transform.DOScale(1, 0.3f).SetEase(Ease.OutQuint).Play();
                }).Play();
                bool isNotFree = (cost > 0 && buyType != E_BuyType.Free);
                if (CharageEffectGo != null)
                    CharageEffectGo.SetActive(isNotFree);
            }
        }

        public override void SetSkin()
        {
            imgClotheDetails.sprite = ResourceManager.Instance.GetUISprite("BookReadingForm/bg_smg");
            mLeftBtn.sprite = ResourceManager.Instance.GetUISprite("BookReadingForm/btn_left");
            mRightBtn.sprite = ResourceManager.Instance.GetUISprite("BookReadingForm/btn_right");
            DownIcon.sprite = ResourceManager.Instance.GetUISprite("BookReadingForm/btn_back_n");
            mLeftBtn.SetNativeSize();
            mRightBtn.SetNativeSize();
            DownIcon.SetNativeSize();

            bool isNotFree = (cost > 0 && buyType != E_BuyType.Free);
            ComfirmEffectGo.SetActive(isNotFree);
            ChangeClothesDiamondGroup.SetActive(isNotFree);
            if (isNotFree)
            {
                btnBg.sprite = ResourceManager.Instance.GetUISprite("BookReadingForm/btn_me_s1");
                ChangeClothesComfirmDescTxt.rectTransform.anchoredPosition = new Vector2(-44, 4);
                switch (buyType)
                {
                    case E_BuyType.Video:
                        AB.ABSystem.ui.SetAtlasSprite(ChangeClothesDiamondGroup.GetComponent<Image>(), "BookReadingForm", "icon_video");
                        break;
                    default:
                        AB.ABSystem.ui.SetAtlasSprite(ChangeClothesDiamondGroup.GetComponent<Image>(), "BookReadingForm", "icon_dimon");
                        break;
                }
            }
            else
            {
                btnBg.sprite = ResourceManager.Instance.GetUISprite("BookReadingForm/btn_me_s");
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
            mDialogData = dialogData;
            mTriggerId = dialogData.trigger;
            mIsSelected = false;
            mSceneWidth = Screen.width;
            this.SelectClothsGo.SetActive(false);
            this.ChangeClothes.SetActive(true);
            ScreenFlashImage.gameObject.SetActive(false);
            imgClotheDetails.gameObject.SetActive(false);
            _form.BgAddBlurEffect(true);
            //_form.ChangeClothesImage.gameObject.SetActive(true);
            DownIcon.transform.localScale = Vector3.zero * mScale;
            DownIcon.rectTransform().anchoredPosition = mDownPosUp;
            //_form.ChangeClothesImage.color = new Color(1, 1, 1, 0);
            ScreenFlashImage.color = new Color(1, 1, 1, 0);
            //_form.ChangeClothesImage.DOFade(1, 0.3f).SetEase(Ease.Linear).OnStart(() => { _form.setBGOnClickListenerActive(false); }).Play();
            setChangeClothesDetails(dialogData);
            _form.setBGOnClickListenerActive(false);
        }

#region ChangeClothesMethods
        private int changeClothesIndex = -1;
        private int changeClothesCount = -1;
        private void updateSelectCloths()
        {
            buyType = E_BuyType.Free;
            if (mTriggerId == 2)//选服装
            {
                mChoicesClothId = int.Parse(DialogDisplaySystem.Instance.CurrentDialogData.GetSelectionsText()[changeClothesIndex]);
                int appearanceID = (100000 + (1 * 10000) + 100 + mChoicesClothId) * 10000;
                t_Skin skin = GameDataMgr.Instance.table.GetSkinById(DialogDisplaySystem.Instance.CurrentBookData.BookID, appearanceID);
                if (skin != null) this.ChangeClothesDetails.text = skin.dec;

                if (!UserDataManager.Instance.CheckClothHadCost(UserDataManager.Instance.UserData.CurSelectBookID, mChoicesClothId))
                {
                    var cfg = GameDataMgr.Instance.table.GetClothePriceById(DialogDisplaySystem.Instance.CurrentBookData.BookID, mChoicesClothId);
                    if (cfg != null)
                    {
                        cost = cfg.ClothePrice;
                        buyType = (E_BuyType)cfg.PriceType;
                        if (buyType == E_BuyType.Video)
                        {
                            int seeVideoNum = UserDataManager.Instance.GetSeeVideoNumOfClothes(DialogDisplaySystem.Instance.CurrentBookData.BookID, mChoicesClothId);
                            cost -= seeVideoNum;
                            if (cost < 0)
                            {
                                cost = 0;
                            }
                        }
                    }
                    else
                    {

                        buyType = E_BuyType.Free;
                        cost = 0;
                    }

                    if (UserDataManager.Instance.CheckBookHasBuy(DialogDisplaySystem.Instance.CurrentBookData.BookID))
                    {

                        cost = 0;
                    }
                }
                else
                {

                    cost = 0;
                }
            }else
            {
                if(mTriggerId == 1)
                {

                    this.ChangeClothesDetails.text = "Tap left and right to choose your appereance.";
                }
                else
                {

                    this.ChangeClothesDetails.text = "Tap left and right to choose your love interest.";
                }
                cost = 0;
            }

            SetSkin();
        }

        /// <summary>
        /// 点击购买按钮
        /// </summary>
        private void changeClothesComfirmButton(PointerEventData data)
        {
            mNeedPay = 0;

            switch (buyType)
            {
                case E_BuyType.Video:
                    {
#if CHANNEL_SPAIN
                        int seeVideoNum = UserDataManager.Instance.GetSeeVideoNumOfClothes(DialogDisplaySystem.Instance.CurrentBookData.BookID, mChoicesClothId);
                        if (cost > 0)
                        {
                            SdkMgr.Instance.ads.ShowRewardBasedVideo((bSuc) =>
                            {
                                if (!bSuc)
                                {
                                    return;
                                }
                                seeVideoNum += 1;
                                UserDataManager.Instance.SetSeeVideoNumOfClothes(DialogDisplaySystem.Instance.CurrentBookData.BookID, mChoicesClothId, seeVideoNum);
                                updateSelectCloths();
                            });
                        }
                        else
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
                                DoChoices();
                            }
                        }
#else
                        MyBooksDisINSTANCE.Instance.VideoUI();
#endif
                    }
                    break;
                case E_BuyType.Diamonds:
                    {
                        if (UserDataManager.Instance.UserData.DiamondNum < cost)
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

                        if (mTriggerId == 2) //主角选服装
                        {
                            mChoicesClothId = int.Parse(DialogDisplaySystem.Instance.CurrentDialogData.GetSelectionsText()[changeClothesIndex]);
                            int appearanceID = (100000 + (1 * 10000) + 100 + mChoicesClothId) * 10000;

                            mNeedPay = 0;

                            if (cost > 0 && !UserDataManager.Instance.CheckClothHadCost(UserDataManager.Instance.UserData.CurSelectBookID, mChoicesClothId))
                            {
                                mNeedPay = 1;
                                //UINetLoadingMgr.Instance.Show();
                                GameHttpNet.Instance.BookChoicesClothCost(UserDataManager.Instance.UserData.CurSelectBookID, 1, mChoicesClothId, ChoicesClothCostCallBack);
                            }
                            else
                            {
                                AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
                                DoChoices();
                            }
                        }
                        else
                        {
                            //选形象
                            DoChoices();
                        }
                    }
                    break;
                default:
                    {
                        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
                        DoChoices();
                    }
                    break;
            }
        }

        private void DoChoices()
        {
            int npcId = 0;
            int npcCharacterId = 0;
            if (mTriggerId == 2)
            {
                GameHttpNet.Instance.SendPlayerProgress(UserDataManager.Instance.UserData.CurSelectBookID,
                    DialogDisplaySystem.Instance.CurrentBookData.ChapterID, DialogDisplaySystem.Instance.CurrentBookData.DialogueID,
                    changeClothesIndex + 1, 0, 0, mChoicesClothId, string.Empty, 0, 0, 0, string.Empty, 0, StartReadChapterCallBack);
            }
            else
            {
                if (mTriggerId == 1)
                {
                    DialogDisplaySystem.Instance.CurrentBookData.PlayerDetailsID = changeClothesIndex + 1;
                }
                else if (mTriggerId == 3)
                {
                    npcId = mDialogData.role_id;
                    npcCharacterId = int.Parse(mDialogData.GetSelectionsText()[changeClothesIndex]);
                    DialogDisplaySystem.Instance.CurrentBookData.NpcId = npcId;
                    DialogDisplaySystem.Instance.CurrentBookData.NpcDetailId = npcCharacterId;
                }

                GameHttpNet.Instance.SendPlayerProgress(UserDataManager.Instance.UserData.CurSelectBookID,
                DialogDisplaySystem.Instance.CurrentBookData.ChapterID, DialogDisplaySystem.Instance.CurrentBookData.DialogueID, changeClothesIndex + 1
               , DialogDisplaySystem.Instance.CurrentBookData.PlayerDetailsID, 0, 0, string.Empty, 0, 0, npcId, string.Empty, npcCharacterId, StartReadChapterCallBack);
            }

            UserDataManager.Instance.RecordBookOptionSelect(UserDataManager.Instance.UserData.CurSelectBookID, DialogDisplaySystem.Instance.CurrentBookData.DialogueID, changeClothesIndex + 1);

        }
        private void SelectAndHideView()
        {
            if (mIsSelected) return;
            mIsSelected = true;

            //关闭左右按钮可以点击的功能
            mLeftBtn.raycastTarget = false;
            mRightBtn.raycastTarget = false;

            SelectClothsGo.gameObject.SetActive(true);
            ScreenFlashImage.gameObject.SetActive(true);
            ScreenFlashImage.DOColor(new Color(1, 1, 1, 1), 0.05f).OnComplete(() =>
            {
                ScreenFlashImage.DOColor(new Color(1, 1, 1, 0), 0.1f).OnComplete(() =>
                {
                    ScreenFlashImage.DOColor(new Color(0, 0, 0, 0.8f), 0.4f).SetDelay(1.5f).OnComplete(() =>
                    {
                        //开启左右按钮可以点击的功能
                        mLeftBtn.raycastTarget = true;
                        mRightBtn.raycastTarget = true;

                        int npcId = 0;
                        int npcCharacterId = 0;
                        if (mTriggerId == 2)
                        {
                            DialogDisplaySystem.Instance.CurrentBookData.PlayerClothes = mChoicesClothId;
                            TalkingDataManager.Instance.SelectCloths(UserDataManager.Instance.UserData.CurSelectBookID, DialogDisplaySystem.Instance.CurrentBookData.DialogueID, mChoicesClothId, mNeedPay, cost);
                        }
                        else
                        {
                            if (mTriggerId == 1)
                            {
                                DialogDisplaySystem.Instance.CurrentBookData.PlayerDetailsID = changeClothesIndex + 1;
                                TalkingDataManager.Instance.SelectPlayer(UserDataManager.Instance.UserData.CurSelectBookID, DialogDisplaySystem.Instance.CurrentBookData.PlayerDetailsID);
                            }
                            else if (mTriggerId == 3)
                            {
                                npcId = mDialogData.role_id;
                                npcCharacterId = int.Parse(mDialogData.GetSelectionsText()[changeClothesIndex]);
                                DialogDisplaySystem.Instance.CurrentBookData.NpcId = npcId;
                                DialogDisplaySystem.Instance.CurrentBookData.NpcDetailId = npcCharacterId;
                            }
                            TalkingDataManager.Instance.SelectNpc(UserDataManager.Instance.UserData.CurSelectBookID, npcCharacterId);
                        }

                        this.ChangeClothes.SetActive(false);
                        _form.BgAddBlurEffect(false);
                        EventDispatcher.Dispatch(EventEnum.ResidentMoneyInfo, 0);
                        EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, changeClothesIndex);
                        EventDispatcher.Dispatch(EventEnum.ChangeBookReadingBgEnable, 1);
                        _form.ResetOperationTime();

                       

                    }).Play();
                }).Play();
            }).Play();
        }

        private void StartReadChapterCallBack(object arg)
        {
            string result = arg.ToString();
            LOG.Info("----Record ClothChange ---->" + result);

            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo != null)
            {
                LoomUtil.QueueOnMainThread((param) =>
                {
                    //UINetLoadingMgr.Instance.Close();
                    if (jo.code == 200)
                    {
                        SelectAndHideView();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(jo.msg)) UITipsMgr.Instance.PopupTips(jo.msg, false);
                    }
                }, null);
            }

        }


        private void ChoicesClothCostCallBack(object arg)
        {
            string result = arg.ToString();
            LOG.Info("----ChoicesClothCostCallBack---->" + result);
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo != null)
            {
                LoomUtil.QueueOnMainThread((param) =>
                {
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
                        DoChoices();
                    }
                    else if (jo.code == 202 || jo.code == 203 || jo.code == 204)
                    {
                        AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                        DoChoices();
                    }
                    else if (jo.code == 206)
                    {
                        AudioManager.Instance.PlayTones(AudioTones.LoseFail);

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
                                tipForm.Init(2, cost, cost * 0.99f);
                            return;
                        }
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
            bool isNotFree = (cost > 0 && buyType != E_BuyType.Free);
            if (CharageEffectGo != null)
                CharageEffectGo.SetActive(isNotFree);

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
            mClothsList = new List<int>();
            mSpriteList = new List<Sprite>();
            for (int i = 0; i < count; i++)
            {
                int targetID = (100000 + (int.Parse(ClothesIDs[i]) * 10000) + 100) * 10000;
#if false
                if(mTriggerId == 1)     //1：表示选主角的形象
                    targetID = (100000 + (int.Parse(ClothesIDs[i]) * 10000) + 100) * 10000;
                else if(mTriggerId == 2)    //2：表示选主角的服装
                    targetID = (100000 + (DialogDisplaySystem.Instance.CurrentBookData.PlayerDetailsID * 10000) + 100 + int.Parse(ClothesIDs[i])) * 10000;
                else if(mTriggerId == 3)    //3：表示选NPC的形象
                    targetID = (100000 + (mDialogData.role_id * 100) + mDialogData.icon) * 10000 + int.Parse(ClothesIDs[i]);
                mSpriteList.Add(DialogDisplaySystem.Instance.GetUITexture("RoleClothes/" + targetID, false));
#else
                int clothGroupId = 0;
                if (mTriggerId == 1)     //1：表示选主角的形象
                {
                    clothGroupId = 1;
                    targetID = (100000 + (1 * 10000) + clothGroupId) * 10000;
                }
                else if (mTriggerId == 2)    //2：表示选主角的服装
                {
                    clothGroupId = Mathf.CeilToInt(int.Parse(ClothesIDs[i]) / (4 * 1.0f));
                    targetID = (100000 + (1 * 10000) + clothGroupId) * 10000;

                }
                else if (mTriggerId == 3)    //3：表示选NPC的形象
                {
                    clothGroupId = Mathf.CeilToInt(mDialogData.icon / (4 * 1.0f));
                    int sex = Mathf.CeilToInt(int.Parse(ClothesIDs[i]) / (3 * 1.0f));
                    targetID = (100000 + (mDialogData.role_id * 100) + clothGroupId) * 10000 + sex;
                }
                mClothsList.Add(targetID);
#endif
            }

            if (count > 0)
            {
#if false
                ClothesImage.sprite = mSpriteList[0];
#else
                appearanceIDCache = 0;
                clothIdCache = 0;
                isSetPos = false;
                UpdateAtlas(0);
                ResetPos();
#endif
                //ClothesImage.color = new Color(1, 1, 1, 0);
                //ClothesImage.DOColor(new Color(1, 1, 1, 1), 0.5f).OnComplete(() => 
                //{
                    MoveDetailFrame(true, false);
                //}).Play();
                MoveLeftOrRightBtn(false);
            }
        }
#endregion


        private void UpdateAtlas(int vIndex)
        {
            int clothId = int.Parse(mDialogData.GetSelectionsText()[vIndex]);
            int appearanceId = mClothsList[vIndex];

            if(isChangeCharacter(appearanceId))
            {
                SkeletonDataAsset skeData = DialogDisplaySystem.Instance.GetSkeDataAsset("Role/" + appearanceId);
                if (skeData == null)
                {
                    LOG.Error("---角色Spine SkeData 有误-->" + appearanceId);
                    return;
                }
                RoleSkeGraphic.skeletonDataAsset = skeData;
                ExpressionSkeGraphic.skeletonDataAsset = skeData;
                HairSkeGraphic.skeletonDataAsset = skeData;

                HeadBoneFolGraphic.SkeletonGraphic = RoleSkeGraphic;

                if(mTriggerId == 1 || mTriggerId == 2)
                {
                    SetClothesIndex(clothId);
                }
                else
                {
                    SetClothesIndex(1);
                }
                ResetAvatar();
            }
            else
            {
                if (isChangeClothes(clothId))
                    SetClothesIndex(clothId);

                SetExpression(1);
            }
        }

        private void ResetPos()
        {
            if (isSetPos) return;
            isSetPos = true;
            ResetAvatar();
            StartCoroutine(DoUpdatePos());
        }

        private void ResetAvatar()
        {
            int skinIndex = 1;
            switch (mTriggerId) //1：表示选主角的形象，2：表示选主角的服装，3：表示选NPC的形象
            {
                case 1:
                    skinIndex = 1;
                    break;
                case 2:
                    skinIndex = DialogDisplaySystem.Instance.CurrentBookData.PlayerDetailsID;
                    break;
                case 3:
                    int clothId = int.Parse(mDialogData.GetSelectionsText()[changeClothesIndex]);
                    skinIndex = clothId % 3 ;
                    break;
            }
            SetSkinName(skinIndex);
            SetExpression(1);
            SetHair(1);
        }

        IEnumerator DoUpdatePos()
        {
            yield return new WaitForEndOfFrame();
            Vector3 targetPos = new Vector3(-HeadBoneFolGraphic.transform.localPosition.x, -HeadBoneFolGraphic.transform.localPosition.y);
            ExpressionSkeGraphic.transform.localPosition = targetPos;
            HairSkeGraphic.transform.localPosition = targetPos;
        }

        private void SetSkinName(int vSkinIndex)
        {
            string skinName ="skin" + vSkinIndex;
            RoleSkeGraphic.initialSkinName = skinName;
            ExpressionSkeGraphic.initialSkinName = skinName;
            HairSkeGraphic.initialSkinName = skinName;

            RoleSkeGraphic.Initialize(true);
            ExpressionSkeGraphic.Initialize(true);
            HairSkeGraphic.Initialize(true);

            ResetFloower();
        }

        private void SetClothesIndex(int vIndex)
        {
            string animName = (vIndex >= 10) ? "clothes" + vIndex : "clothes" + vIndex;
            Debug.Log("===Index---->" + vIndex + "---animName---" + animName);
            RoleSkeGraphic.startingAnimation = animName;
            RoleSkeGraphic.startingLoop = true;
            RoleSkeGraphic.Initialize(true);
            clothIdCache = vIndex;
            ResetFloower();
        }

        private void SetExpression(int vIndex)
        {
            string animName = "expression" + vIndex;
            Debug.Log("===Index---->" + vIndex + "---animName---" + animName);
            ExpressionSkeGraphic.startingAnimation = animName;
            ExpressionSkeGraphic.startingLoop = false;
            ExpressionSkeGraphic.Initialize(true);

            ResetFloower();
        }

        private void SetHair(int vIndex)
        {
            string animName = "hair" + vIndex;
            Debug.Log("===Index---->" + vIndex + "---Hair---" + animName);
            HairSkeGraphic.startingAnimation = animName;
            HairSkeGraphic.startingLoop = false;
            HairSkeGraphic.Initialize(true);

            ResetFloower();
        }

        private void ResetFloower()
        {
            HeadBoneFolGraphic.boneName = "tou";
            HeadBoneFolGraphic.Initialize();
        }

        //是否角色有变化
        private bool isChangeCharacter(int appearanceID)
        {
            if (appearanceIDCache == appearanceID)
            {
                return false;
            }

            appearanceIDCache = appearanceID;
            return true;
        }

        //服装是否有变化
        private bool isChangeClothes(int curClothId)
        {
            if (clothIdCache == curClothId)
            {
                return false;
            }

            clothIdCache = curClothId;
            return true;
        }


        public override void Dispose()
        {
            UIEventListener.RemoveOnClickListener(this.ChangeClothesLeftButton, TurnLeftHandler);
            UIEventListener.RemoveOnClickListener(this.ChangeClothesRightButton, TurnRightHandler);
            UIEventListener.RemoveOnClickListener(this.DownIcon.gameObject, HideDetailInfoHandler);
            UIEventListener.RemoveOnClickListener(this.ScreenFlashImage.gameObject, ShowDetailInfoHandler);
            UIEventListener.RemoveOnClickListener(this.ChangeClothesComfirmButton.gameObject, changeClothesComfirmButton);


            if (mSpriteList != null)
            {
                int len = mSpriteList.Count;
                for (int i = 0; i < len; i++)
                {
                    mSpriteList[i] = null;
                }
            }
            ClothesImage.sprite = null;
            imgClotheDetails.sprite = null;
            mLeftBtn.sprite = null;
            mRightBtn.sprite = null;
            btnBg.sprite = null;
        }
#endif
    }
}
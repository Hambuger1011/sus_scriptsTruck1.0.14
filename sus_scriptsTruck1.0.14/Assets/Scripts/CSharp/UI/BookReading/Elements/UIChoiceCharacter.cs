
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

    public class UIChoiceCharacter : UIBookReadingElement
    {

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

        private int cost;


        private BaseDialogData mDialogData;
        private int mType;  //0：表示选择主角的外面，1：:表示选择NPC的外貌
#if NOT_USE_LUA
        public override void Bind(BookReadingForm form)
        {
            _form = form;
            form.choiceCharacter = this;
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
            Image rightBtn = ChangeClothesRightButton.GetComponent<Image>();
            leftBtn.sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/ChangeClothes/btn_last.png");
            rightBtn.sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/ChangeClothes/btn_next.png");
            leftBtn.SetNativeSize();
            rightBtn.SetNativeSize();

            ComfirmEffectGo.SetActive(cost > 0);
            ChangeClothesDiamondGroup.SetActive(cost > 0);
            if(cost > 0)
            {
                btnBg.sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/ChangeClothes/bg_btn.png");
                ChangeClothesComfirmDescTxt.rectTransform.anchoredPosition = new Vector2(-52, 4);
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

        public void Show(BaseDialogData dialogData,int vType = 0)
        {
            mDialogData = dialogData;
            mType = vType;
            this.SelectClothsGo.SetActive(false);
            this.ChangeClothes.SetActive(true);
            _form.BgAddBlurEffect(true);
            //_form.ChangeClothesImage.gameObject.SetActive(true);
            //_form.ChangeClothesImage.color = new Color(1, 1, 1, 0);
            //_form.ChangeClothesImage.DOFade(1, 0.3f).SetEase(Ease.Linear).OnStart(() => { _form.setBGOnClickListenerActive(false); }).Play();
            setChangeClothesDetails(dialogData);
            _form.setBGOnClickListenerActive(false);

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
            //DialogDisplaySystem.Instance.CurrentBookData.PlayerClothes = int.Parse(DialogDisplaySystem.Instance.CurrentDialogData.GetSelectionsText()[changeClothesIndex]);
            //int appearanceID = 100000 + (DialogDisplaySystem.Instance.CurrentBookData.PlayerDetailsID * 10000) + 100 + DialogDisplaySystem.Instance.CurrentBookData.PlayerClothes;
            //t_Skin skin = GameDataMgr.Instance.table.GetSkinById(DialogDisplaySystem.Instance.CurrentBookData.BookID, appearanceID);
            //if (skin != null) this.ChangeClothesDetails.text = skin.dec;
            //if (!DialogDisplaySystem.Instance.CheckHasThisCloth(appearanceID))
            //    cost = DialogDisplaySystem.Instance.CurrentDialogData.GetSelectionsCost()[changeClothesIndex];
            //else
                cost = 0;
                SetSkin();
        }

        private void changeClothesComfirmButton(PointerEventData data)
        {
            AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
            if(UserDataManager.Instance.UserData.DiamondNum < cost)
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
                        tipForm.Init(2, cost, cost * 0.99f);
                    return;
                }            
            }

            UserDataManager.Instance.CalculateDiamondNum(-cost);
            //DialogDisplaySystem.Instance.CurrentBookData.PlayerClothes = int.Parse(DialogDisplaySystem.Instance.CurrentDialogData.GetSelectionsText()[changeClothesIndex]);
            //int appearanceID = 100000 + (DialogDisplaySystem.Instance.CurrentBookData.PlayerDetailsID * 10000) + 100 + DialogDisplaySystem.Instance.CurrentBookData.PlayerClothes;
            //if (!DialogDisplaySystem.Instance.CheckHasThisCloth(appearanceID))
            //{
            //    DialogDisplaySystem.Instance.AddClothes(appearanceID);
            //}
            int npcId = 0;
            int npcCharacterId = 0;
            if(mType == 0)
                DialogDisplaySystem.Instance.CurrentBookData.PlayerDetailsID = changeClothesIndex + 1;
            else
            {
                npcId = mDialogData.role_id;
                npcCharacterId = int.Parse(mDialogData.GetSelectionsText()[changeClothesIndex]);
                DialogDisplaySystem.Instance.CurrentBookData.NpcId = npcId;
                DialogDisplaySystem.Instance.CurrentBookData.NpcDetailId = npcCharacterId;
            }

            this.ChangeClothes.SetActive(false);
            _form.BgAddBlurEffect(false);
            //_form.ChangeClothesImage.gameObject.SetActive(false);

            GameHttpNet.Instance.SendPlayerProgress(UserDataManager.Instance.UserData.CurSelectBookID,
                DialogDisplaySystem.Instance.CurrentBookData.ChapterID, DialogDisplaySystem.Instance.CurrentBookData.DialogueID,0
               , DialogDisplaySystem.Instance.CurrentBookData.PlayerDetailsID, 0, 0, string.Empty, 0, 0, npcId, string.Empty, npcCharacterId, StartReadChapterCallBack);

            UserDataManager.Instance.RecordBookOptionSelect(UserDataManager.Instance.UserData.CurSelectBookID, DialogDisplaySystem.Instance.CurrentBookData.DialogueID, changeClothesIndex + 1);

            _form.setBGOnClickListenerActive(true);
            EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, changeClothesIndex);
            _form.ResetOperationTime();
            if(mType == 0)
                TalkingDataManager.Instance.SelectPlayer(UserDataManager.Instance.UserData.CurSelectBookID, DialogDisplaySystem.Instance.CurrentBookData.PlayerDetailsID);
            else
                TalkingDataManager.Instance.SelectNpc(UserDataManager.Instance.UserData.CurSelectBookID, npcCharacterId);
        }

        private void StartReadChapterCallBack(object arg)
        {
            string result = arg.ToString();
            LOG.Info("----GetDayLoginCallBack---->" + result);

        }

        private void setChangeClothesDetails(BaseDialogData dialogData)
        {
            changeClothesIndex = 0;
            changeClothesCount = dialogData.selection_num;
            ChangeClothesLeftButton.SetActive(changeClothesCount > 1);
            ChangeClothesRightButton.SetActive(changeClothesCount > 1);
            this.ChangeClothesGroup.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            setClothesDetails(dialogData.selection_num, int.Parse(dialogData.selection_1), int.Parse(dialogData.selection_2), int.Parse(dialogData.selection_3), (string.IsNullOrEmpty(dialogData.selection_4))?0:(int.Parse(dialogData.selection_4)));
            updateSelectCloths();
        }
        private void setClothesDetails(int count, params int[] CharacterID)
        {
            Image[] imageGroup = this.ChangeClothesGroup.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < CharacterID.Length; i++)
            {
                if (i < count)
                {
                    //int appearanceID = 100000 + (DialogDisplaySystem.Instance.CurrentBookData.PlayerDetailsID * 10000) + 100 + int.Parse(ClothesIDs[i]);
                    int appearanceID = (100000 + (CharacterID[i] * 10000) + 100) * 10000;
                    if (mType == 1)
                        appearanceID = (100000 + (mDialogData.role_id * 100) + mDialogData.icon) * 10000 + CharacterID[i];

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
            if (imageGroup != null)
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
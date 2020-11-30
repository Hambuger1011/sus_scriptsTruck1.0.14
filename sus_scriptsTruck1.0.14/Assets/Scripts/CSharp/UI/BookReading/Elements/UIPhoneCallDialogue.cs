
namespace BookReading
{
    using DG.Tweening;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class UIPhoneCallDialogue : UIBookReadingElement
    {

        public GameObject PhoneCallDialogue;
        public RectTransform PhoneCallBG;
        public Image PhoneCallHeadIcon;
        public Text PhoneCallState;
        public Text PhoneCallRoleName;
        public Button PhoneCallButton;
        public GameObject PhoneCallNarration;
        public RectTransform PhoneCallNarrationBox;
        public TextTyperAnimation PhoneCallNarrationText;
        public GameObject PhoneCallPlayerDialogue;
        public RectTransform PhoneCallPlayerDialogueBox;
        public TextTyperAnimation PhoneCallPlayerDialogueText;
        public Text PhoneCallPlayerDialogueName;
        public GameObject PhoneCallOtherDialogue;
        public RectTransform PhoneCallOtherDialogueBox;
        public TextTyperAnimation PhoneCallOtherDialogueText;
        public Text PhoneCallOtherDialogueName;
        public GameObject DialogBoxContent;

        private BookReadingForm _form;
#if NOT_USE_LUA
        public override void Bind(BookReadingForm form)
        {
            _form = form;
            form.phoneCallDialogue = this;
            UIEventListener.AddOnClickListener(this.PhoneCallButton.gameObject, phoneCallComfirmButton);

            if (GameUtility.IpadAspectRatio() && PhoneCallDialogue != null)
                PhoneCallDialogue.transform.localScale = Vector3.one * 0.7f;
        }

        public override void ResetUI()
        {
            this.PhoneCallDialogue.SetActive(false);
        }

        public override void SetSkin() { }

        #region PhoneCallMode
        
        public bool m_bIsPhoneCallMode { get; private set; }


        public void InitData(BookData bookData)
        {
            m_bIsPhoneCallMode = bookData.IsPhoneCallMode;
            if (m_bIsPhoneCallMode)
            {
                this.PhoneCallDialogue.SetActive(true);
                this.PhoneCallButton.gameObject.SetActive(false);
                this.PhoneCallState.text = "in the call";
                this.PhoneCallRoleName.text = DialogDisplaySystem.Instance.GetRoleName(DialogDisplaySystem.Instance.CurrentBookData.PhoneRoleID, UserDataManager.Instance.UserData.CurSelectBookID);
                this.PhoneCallHeadIcon.sprite = DialogDisplaySystem.Instance.GetUITexture("UI/PhoneCallHeadIcon/" + DialogDisplaySystem.Instance.CurrentBookData.PhoneRoleID);
            }
        }

        public void getPhoneCallMessage(bool isExitPhoneCallMode, BaseDialogData dialogData)
        {
            if (!isExitPhoneCallMode)
            {
                _form.setBGOnClickListenerActive(false);
                this.PhoneCallButton.gameObject.SetActive(true);
                this.PhoneCallState.text = "Calling";
                this.PhoneCallRoleName.text = DialogDisplaySystem.Instance.GetRoleName(dialogData.role_id, UserDataManager.Instance.UserData.CurSelectBookID);
                this.PhoneCallHeadIcon.sprite = DialogDisplaySystem.Instance.GetUITexture("UI/PhoneCallHeadIcon/" + dialogData.role_id);
                DialogDisplaySystem.Instance.CurrentBookData.PhoneRoleID = dialogData.role_id;

                //GameHttpNet.Instance.SendPlayerProgress(UserDataManager.Instance.UserData.CurSelectBookID,
                //DialogDisplaySystem.Instance.CurrentBookData.ChapterID, DialogDisplaySystem.Instance.CurrentBookData.DialogueID,
                //0, 0, 0, DialogDisplaySystem.Instance.CurrentBookData.PlayerClothes, string.Empty, 1, dialogData.role_id, RecordPhoneCallCallBack);
            }

            int targetPosY = 0;
            if (GameUtility.IpadAspectRatio())
                targetPosY = -167;

            Vector2 startPos = isExitPhoneCallMode ? new Vector2(0, targetPosY) : new Vector2(-750, -857);
            Vector2 endPos = isExitPhoneCallMode ? new Vector2(-750, -857) : new Vector2(0, targetPosY);
            this.PhoneCallBG.anchoredPosition = startPos;
            this.PhoneCallBG.DOAnchorPos(endPos, 0.6f)
                .OnStart(() => { if (!isExitPhoneCallMode) this.PhoneCallDialogue.SetActive(true); _form.IsTweening = true; })
                .OnComplete(() =>
                {
                    _form.IsTweening = false;
                    if (isExitPhoneCallMode)
                    {
                        m_bIsPhoneCallMode = false;
                        DialogDisplaySystem.Instance.CurrentBookData.IsPhoneCallMode = m_bIsPhoneCallMode;

                        GameHttpNet.Instance.SendPlayerProgress(UserDataManager.Instance.UserData.CurSelectBookID,
                        DialogDisplaySystem.Instance.CurrentBookData.ChapterID, DialogDisplaySystem.Instance.CurrentBookData.DialogueID,
                        0, 0, 0, DialogDisplaySystem.Instance.CurrentBookData.PlayerClothes, string.Empty, 2, 0, 0, string.Empty, 0, RecordPhoneCallCallBack);

                        this.PhoneCallDialogue.SetActive(false);
                        EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, null);
                    }
                    else
                    {
                        this.PhoneCallDialogue.transform.DOShakeRotation(3f, new Vector3(0, 0, 3), 200).SetEase(Ease.Flash);
                        AudioManager.Instance.StopBGMQuick();
                        DialogDisplaySystem.Instance.PlayBGM(DialogDisplaySystem.Instance.CurrentBookData.BookID, "TelephoneRing_didi");
                    }
                });

#if ENABLE_DEBUG
            if (GameDataMgr.Instance.InAutoPlay)
            {
                Invoke("AutoDo", 1.5f);
            }
#endif
        }

        private void AutoDo()
        {
            CancelInvoke("AutoDo");
            phoneCallComfirmButton(null);
        }

        private void RecordPhoneCallCallBack(object arg)
        {

        }

        private void phoneCallComfirmButton(PointerEventData pointerEventData)
        {
            AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
            if (!_form.IsTweening)
            {
                m_bIsPhoneCallMode = true;
                this.PhoneCallState.text = "in the call";
                DialogDisplaySystem.Instance.CurrentBookData.IsPhoneCallMode = m_bIsPhoneCallMode;

                GameHttpNet.Instance.SendPlayerProgress(UserDataManager.Instance.UserData.CurSelectBookID,
                DialogDisplaySystem.Instance.CurrentBookData.ChapterID, DialogDisplaySystem.Instance.CurrentBookData.DialogueID,
                0, 0, 0, DialogDisplaySystem.Instance.CurrentBookData.PlayerClothes, string.Empty, 1, DialogDisplaySystem.Instance.CurrentDialogData.role_id,
                0, string.Empty, 0, RecordPhoneCallCallBack);

                _form.setBGOnClickListenerActive(true);
                this.PhoneCallButton.gameObject.SetActive(false);
                EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, null);
                this.PhoneCallDialogue.transform.rectTransform().rotation = Quaternion.identity;
                this.PhoneCallDialogue.transform.DOKill();
                AudioManager.Instance.StopBGMQuick();
                _form.currentBGMID = -1;
                _form.changeBGM(DialogDisplaySystem.Instance.CurrentDialogData);
            }
            _form.ResetOperationTime();
        }
        #endregion


        public override void Dispose()
        {
            UIEventListener.RemoveOnClickListener(this.PhoneCallButton.gameObject, phoneCallComfirmButton);

            this.PhoneCallHeadIcon.sprite = null;
        }
#endif
    }
}

namespace BookReading
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    public class UIPlayerDialogue : UIBookReadingElement
    {
        public GameObject PlayerDialogue;
        public RectTransform PlayerDialogBox;
        public Text PlayerName;
        public TextTyperAnimation PlayerText;
        public CharacterFaceExpressionChange PlayerCharacterDisplay;
        public Image ColorBg1;
        public Image ColorBg2;
        public GameObject DialogBoxContent;

        [Header("-------------skin-------------")]
        public Image imgBG;
        public Image imgBG2;


#if NOT_USE_LUA
        private void ClickHeadHandler(PointerEventData data)
        {
            AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
            EventDispatcher.Dispatch(UIEventMethodName.ClickHeadToChangeFaceExpression.ToString());
        }

        public override void Bind(BookReadingForm form)
        {
            form.playerDialogue = this;
            UIEventListener.AddOnClickListener(ColorBg1.gameObject, ClickHeadHandler);
            UIEventListener.AddOnClickListener(ColorBg2.gameObject, ClickHeadHandler);
        }
        public override void SetSkin()
        {
            imgBG.sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/Dialog/bg_chat_left.png");
            //imgBG2.sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/Dialog/bg_chat_2.png");
        }

        /// <summary>
        /// 切换对话模式（0：对话，1：思考）
        /// </summary>
        /// <param name="type"></param>
        public void ChangeDialogMode(int type = 0)
        {
            if (type == 0)
                imgBG2.sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/Dialog/bg_chat_2.png");
            else
                imgBG2.sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/Dialog/bg_think.png");
        }

        public override void ResetUI()
        {
            this.PlayerDialogue.SetActive(false);
        }

        public override void Dispose()
        {
            UIEventListener.RemoveOnClickListener(ColorBg1.gameObject, ClickHeadHandler);
            UIEventListener.RemoveOnClickListener(ColorBg2.gameObject, ClickHeadHandler);

            imgBG.sprite = null;
            imgBG2.sprite = null;
            PlayerCharacterDisplay.Dispose();
        }
#endif
    }
}
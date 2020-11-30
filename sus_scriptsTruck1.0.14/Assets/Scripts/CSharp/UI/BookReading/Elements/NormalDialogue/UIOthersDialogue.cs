
namespace BookReading
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIOthersDialogue : UIBookReadingElement
    {
        public GameObject OtherDialogue;
        public RectTransform OtherDialogBox;
        public Text OtherName;
        public TextTyperAnimation OtherText;
        public CharacterFaceExpressionChange OtherCharacterDisplay;
        public GameObject DialogBoxContent;

        [Header("-------------skin-------------")]
        public Image imgBG;
        public Image imgBG2;
#if NOT_USE_LUA
        public override void Bind(BookReadingForm form)
        {
            form.othersDialogue = this;
        }
        public override void SetSkin()
        {
            imgBG.sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/Dialog/bg_chat_right.png");
            imgBG2.sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/Dialog/bg_chat_2.png");
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
            this.OtherDialogue.SetActive(false);
        }

        public override void Dispose()
        {
            imgBG.sprite = null;
            imgBG2.sprite = null;
            OtherCharacterDisplay.Dispose();
        }
#endif
    }
}
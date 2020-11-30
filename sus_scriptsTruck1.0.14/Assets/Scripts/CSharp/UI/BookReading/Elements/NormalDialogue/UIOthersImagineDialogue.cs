
namespace BookReading
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIOthersImagineDialogue : UIBookReadingElement
    {

        public GameObject OtherImagineDialogue;
        public RectTransform OtherImagineDialogBox;
        public Text OtherImagineName;
        public TextTyperAnimation OtherImagineText;
        public CharacterFaceExpressionChange OtherImagineCharacterDisplay;
        public GameObject DialogBoxContent;

        [Header("-------------skin-------------")]
        public Image imgBG;
        public Image imgBG2;
#if NOT_USE_LUA
        public override void Bind(BookReadingForm form)
        {
            form.othersImagineDialogue = this;
        }
        public override void SetSkin()
        {
            imgBG.sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/Dialog/bg_chat_right.png");
            imgBG2.sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/Dialog/bg_think.png");
        }

        public override void ResetUI()
        {
            this.OtherImagineDialogue.SetActive(false);
        }

        public override void Dispose()
        {
            imgBG.sprite = null;
            imgBG2.sprite = null;
            OtherImagineCharacterDisplay.Dispose();
        }
#endif
    }
}
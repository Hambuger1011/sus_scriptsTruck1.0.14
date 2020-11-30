
namespace BookReading
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIPlayerImagineDialogue : UIBookReadingElement
    {
        public GameObject PlayerImagineDialogue;
        public RectTransform PlayerImagineDialogBox;
        public Text PlayerImagineName;
        public TextTyperAnimation PlayerImagineText;
        public CharacterFaceExpressionChange PlayerImagineCharacterDisplay;
        public GameObject DialogBoxContent;

        [Header("-------------skin-------------")]
        public Image imgBG;
        public Image imgBG2;
#if NOT_USE_LUA
        public override void Bind(BookReadingForm form)
        {
            form.playerImagineDialogue = this;
        }
        public override void SetSkin()
        {
            imgBG.sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/Dialog/bg_chat_left.png");
            imgBG2.sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/Dialog/bg_think.png");
        }

        public override void ResetUI()
        {
            this.PlayerImagineDialogue.SetActive(false);
        }

        public override void Dispose()
        {
            imgBG.sprite = null;
            imgBG2.sprite = null;
            PlayerImagineCharacterDisplay.Dispose();
        }
#endif
    }
}
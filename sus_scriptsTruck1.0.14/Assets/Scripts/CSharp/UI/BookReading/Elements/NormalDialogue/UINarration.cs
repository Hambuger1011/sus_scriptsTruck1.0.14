
namespace BookReading
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UINarration : UIBookReadingElement
    {
        public GameObject Narration;
        public RectTransform NarrationDialogBox;
        public TextTyperAnimation NarrationText;

        [Header("-------------skin-------------")]
        public Image imgBG;
#if NOT_USE_LUA
        public override void Bind(BookReadingForm form)
        {
            form.narration = this;
        }
        public override void SetSkin()
        {
            imgBG.sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/Dialog/bg_chat.png");
        }

        public override void ResetUI()
        {
            this.Narration.SetActive(false);
        }

        public override void Dispose()
        {
            imgBG.sprite = null;
        }
#endif
    }
}

namespace UI
{
    using Candlelight.UI;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using GameCore.UGUI;
    using UnityEngine.UI;


    [XLua.Hotfix, XLua.CSharpCallLua, XLua.LuaCallCSharp]
    public class UIBubbleBox_Image : UIBubbleBox
    {
        UITweenButton btnImage;
        public Image imgPicture;
        public override Vector2 size
        {
            get
            {
                var spt = imgPicture.sprite;
                if(spt != null)
                {
                    return spt.bounds.size * spt.pixelsPerUnit;
                }
                return new Vector2(750, 250);
            }
        }

        public UIBubbleBox_Image(UIBubbleItem item,Transform root) : base(item,root)
        {
            imgPicture = root.Find("picture").GetComponent<Image>();
            btnImage = root.GetComponent<UITweenButton>();
            btnImage.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            this.item.OnItemClick(this.item);
        }

        public override void RefreshUI()
        {
        }
    }
}

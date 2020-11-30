namespace UI
{
    using Candlelight.UI;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using GameCore.UGUI;

    [XLua.Hotfix, XLua.CSharpCallLua,XLua.LuaCallCSharp]
    public class UIBubbleBox_Voice : UIBubbleBox
    {
        UITweenButton btnImage;

        public override Vector2 size
        {
            get
            {
                return new Vector2(200, 80);
            }
        }

        public UIBubbleBox_Voice(UIBubbleItem item, Transform root) : base(item,root)
        {

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
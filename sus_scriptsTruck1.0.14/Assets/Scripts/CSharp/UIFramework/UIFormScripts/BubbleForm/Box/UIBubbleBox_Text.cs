
namespace UI
{
    using Candlelight.UI;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using UnityEngine.UI;

    [XLua.Hotfix, XLua.CSharpCallLua, XLua.LuaCallCSharp]
    public class UIBubbleBox_Text : UIBubbleBox
    {
        private float lableWidth = 0;
        private HyperText hypertext;
        private Text nameText;
        private Vector2 extents;
        private Vector2 _size;
        private Vector2 labelSize;

        public override Vector2 size
        {
            get
            {
                return _size;
            }
        }

        public UIBubbleBox_Text(UIBubbleItem item, Transform root):base(item,root)
        {
            hypertext = this.transform.Find("hypertext").GetComponent<HyperText>();
            var boxSize = this.transform.rect.size;
            labelSize = hypertext.rectTransform.rect.size;
            lableWidth = labelSize.x;
            if(lableWidth < 0)
            {
                Debug.LogError(hypertext.rectTransform.rect+" "+ hypertext.rectTransform.offsetMin);
                Debug.LogError(hypertext.rectTransform.offsetMin + " " + hypertext.rectTransform.offsetMax);
                Debug.LogError(hypertext.rectTransform.anchorMin + " " + hypertext.rectTransform.anchorMax);
                lableWidth = 100;
            }
            this.extents = boxSize - labelSize;
            //var pos = this.transform.anchoredPosition;
            //this.extents.x += Mathf.Abs(pos.x);
            //this.extents.y += Mathf.Abs(pos.y);
        }

        public override void RefreshUI()
        {
            if(this.item.data.cfgType == 1 && this.item.data.bookCfg != null)
            {
                this.hypertext.text = StringUtils.ReplaceChar(this.item.data.bookCfg.dialog);
            }
            else 
            if(this.item.data.cfgType == 2 && this.item.data.bookCfg != null)
            {
                this.hypertext.text = StringUtils.ReplaceChar(this.item.data.bookCfg.dialog);
            }

            var txtPreferredSize = new Vector2(Mathf.Min(hypertext.preferredWidth, lableWidth), hypertext.GetPreferredHeight(labelSize.x));
            _size = new Vector2(txtPreferredSize.x + extents.x, txtPreferredSize.y + extents.y);
            //Debug.LogError(this.item.index + " " + txtPreferredSize + " "+ _size + "-" + hypertext.text);
            //return  new Vector2(hypertext.preferredWidth, hypertext.preferredHeight);
        }

        public override string ToString()
        {
            if(this.hypertext == null)
            {
                return base.ToString();
            }
            return this.hypertext.text;
        }
    }
}

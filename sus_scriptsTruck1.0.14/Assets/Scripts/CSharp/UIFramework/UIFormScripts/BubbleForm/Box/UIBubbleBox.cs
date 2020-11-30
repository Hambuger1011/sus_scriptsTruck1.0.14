
namespace UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    [XLua.Hotfix, XLua.CSharpCallLua, XLua.LuaCallCSharp]
    public abstract class UIBubbleBox
    {
        public abstract Vector2 size { get; }
        public EBubbleBoxType type;

        public GameObject gameObject;
        public RectTransform transform;
        public UIBubbleItem item;
        public UIBubbleBox(UIBubbleItem item,Transform root)
        {
            this.item = item;
            this.transform = (RectTransform)root;
            this.gameObject = root.gameObject;
        }

        public virtual void SetActive(bool isOn)
        {
            this.gameObject.SetActiveEx(isOn);
        }

        public abstract void RefreshUI();

        public void SetSize()
        {
            //Debug.LogError(this.item.index + " " + size + " " + this);
            this.transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            this.transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        }
    }
}
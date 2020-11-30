using System.Collections;
using System.Collections.Generic;
namespace UI
{
    using UnityEngine;


    [XLua.Hotfix, XLua.CSharpCallLua, XLua.LuaCallCSharp]
    public class UIBubbleItem_Middle : UIBubbleItem
    {
        private Vector2 extents;
        private Vector2 _size;
        public override Vector2 size
        {
            get
            {
                return _size;
            }
        }

        
        public UIBubbleItem_Middle(UIBubbleList list, GameObject pfb) : base(EBubbleType.Middle, list, pfb)
        {
        }
        public override void SetData(UIBubbleData data)
        {
            base.SetData(data);

            var pos = this.curBox.transform.anchoredPosition;
            this.extents = new Vector2(Mathf.Abs(pos.x), Mathf.Abs(pos.y));
            _size = new Vector2(this.width, this.curBox.size.y + this.extents.y);
        }
    }
}
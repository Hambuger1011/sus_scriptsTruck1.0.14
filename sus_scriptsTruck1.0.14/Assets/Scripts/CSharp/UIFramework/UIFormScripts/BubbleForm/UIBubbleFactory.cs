namespace UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    [XLua.Hotfix, XLua.CSharpCallLua, XLua.LuaCallCSharp]
    public static class UIBubbleFactory
    {
        public static GameObject[] itemPfbs;
        
        public static UIBubbleItem CreateItem(UIBubbleList list,EBubbleType type)
        {
            UIBubbleItem item = null;
            switch (type)
            {
                case EBubbleType.Middle:
                    item = new UIBubbleItem_Middle(list,itemPfbs[(int)type]);
                    break;
                case EBubbleType.Left:
                    item = new UIBubbleItem_Left(list,itemPfbs[(int)type]);
                    break;
                case EBubbleType.Right:
                    item = new UIBubbleItem_Right(list,itemPfbs[(int)type]);
                    break;
            }
            return item;
        }
    }
}
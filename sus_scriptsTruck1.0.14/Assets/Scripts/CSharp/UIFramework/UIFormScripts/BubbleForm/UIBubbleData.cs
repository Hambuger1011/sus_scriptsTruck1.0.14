namespace UI
{
    using System.Collections;
    using System.Collections.Generic;
    using pb;
    using UnityEngine;


    [XLua.Hotfix, XLua.CSharpCallLua, XLua.LuaCallCSharp]
    public class UIBubbleData
    {
        public bool isNew = true;
        public EBubbleType type1;
        public EBubbleBoxType type2;

        public Vector2 leftTop;
        public Vector2 size;
        public UIBubbleItem ui;
        public t_BookChat_1 cfg;
        public BaseDialogData bookCfg;
        public int cfgType = 1;

        public UIBubbleData(EBubbleType type1, EBubbleBoxType type2)
        {
            this.type1 = type1;
            this.type2 = type2;
        }

        public UIBubbleData(t_BookChat_1 cfg)
        {
            cfgType = 1;
            this.cfg = cfg;
            this.type1 = (EBubbleType)cfg.type1;
            this.type2 = (EBubbleBoxType)cfg.type2;
        }

        public UIBubbleData(BaseDialogData cfg)
        {
            cfgType = 2;
            bookCfg = cfg;
            switch((int)cfg.dialog_type)
            {
                case 26:
                    type1 = EBubbleType.Middle;
                    break;
                case 27:
                    type1 = EBubbleType.Left ;
                    break;
                case 28:
                    type1 = EBubbleType.Right;
                    break;
            }

            this.type2 = (EBubbleBoxType)cfg.bubbleType;
        }
    }
}
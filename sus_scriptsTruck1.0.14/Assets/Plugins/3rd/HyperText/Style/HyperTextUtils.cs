using UnityEngine;
using System.Collections;
using System;
using Candlelight.UI;
using System.Text.RegularExpressions;

namespace Candlelight
{
    public static class HyperTextUtils
    {
        public delegate HyperTextStyles.Quad FuncLoadQuad(string className);
        static event FuncLoadQuad mFuncLoadQuad;

        public static void RegisterLoadQuad(FuncLoadQuad func)
        {
            mFuncLoadQuad -= func;
            mFuncLoadQuad += func;
        }

        public static void UnRegisterLoadQuad(FuncLoadQuad func)
        {
            mFuncLoadQuad -= func;
        }

        public static HyperTextStyles.Quad LoadStyleQuad(string className)
        {
            if (mFuncLoadQuad != null)
            {
                HyperTextStyles.Quad quad = mFuncLoadQuad(className);
                if (quad.Sprite == null)
                {
                    quad.Sprite = Resources.Load<Sprite>("Sprite/unknown");
                }
                return quad;
            }

            //Debug.Log("加载quad:" + className);
            Sprite spt = Resources.Load<Sprite>("Sprite/unknown");
            HyperTextStyles.Quad defaultQuad = new HyperTextStyles.Quad(spt, className);
            return defaultQuad;
        }
    }
}

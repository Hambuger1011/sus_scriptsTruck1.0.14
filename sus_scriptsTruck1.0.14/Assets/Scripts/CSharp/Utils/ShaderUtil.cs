using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[XLua.LuaCallCSharp,XLua.Hotfix]
public class ShaderUtil 
{
    private static Material mGrayMat;
    private static Material mBlurMat;
    private static Material mGaussianBlurMat;

    /// <summary>
    /// 灰度的shader
    /// </summary>
    /// <returns></returns>
    public static Material GrayMaterial()
    {
         if(mGrayMat == null)
         {
             mGrayMat = new Material(Shader.Find("Unlit/UIGrayScale"));
         }
        return mGrayMat;
    }

    /// <summary>
    /// 模糊的效果
    /// </summary>
    /// <returns></returns>
    public static Material BlurEffevtMaterial()
    {
        if (mBlurMat == null)
        {
            mBlurMat = new Material(Shader.Find("Custom/UISimpleBlurEffect"));
        }
        return mBlurMat;
    }

    /// <summary>
    /// 高斯模糊的效果
    /// </summary>
    /// <returns></returns>
    public static Material GaussianBlurEffevtMaterial()
    {
        if (mGaussianBlurMat == null)
        {
            mGaussianBlurMat = new Material(Shader.Find("Custom/GaussianBlur"));
        }
        return mGaussianBlurMat;
    }
}

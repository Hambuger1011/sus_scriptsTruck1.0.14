using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FreeImageAPI;
using UnityEngine.UI;

public class ImageTest : MonoBehaviour
{
    public Texture2D srcText;
    public Image image;

#if UNITY_EDITOR
    // Use this for initialization
    void Start ()
	{
        var texturePath = UnityEditor.AssetDatabase.GetAssetPath(srcText);
        FIBITMAP fib = FreeImage.LoadEx(texturePath);
        Texture2D t2d = new Texture2D((int)FreeImage.GetWidth(fib), (int)FreeImage.GetHeight(fib));

        for(uint i = 0; i < t2d.width; ++i)
        {
            for(uint j= 0; j<t2d.height;++j)
            {
                RGBQUAD value;
                FreeImage.GetPixelColor(fib, i, j, out value);
                var c = value.Color;
                t2d.SetPixel((int)i, (int)j, new UnityEngine.Color(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f, c.A/255.0f));
            }
        }
        t2d.Apply();
        Debug.Log(t2d.mipmapCount);
        FreeImage.SaveEx(fib,"ssad");
        //t2d.SetPixels(FreeImage.);
        Sprite sp=Sprite.Create(t2d,new Rect(0,0, (int)FreeImage.GetWidth(fib), (int)FreeImage.GetHeight(fib)), new Vector2(0.5f, 0.5f));

        image.sprite = sp;
        


	}
#endif
}

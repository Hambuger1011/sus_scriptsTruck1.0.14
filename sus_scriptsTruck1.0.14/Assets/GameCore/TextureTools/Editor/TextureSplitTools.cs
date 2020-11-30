#if TP
using GameCore;

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TextureSplitTools
{


    /// <summary>
    /// 创建
    /// </summary>
    [MenuItem("Assets/GameTools/切割小图")]
    static void Texture2dSplit()
    {
        string file = AssetDatabase.GetAssetPath(Selection.activeObject);
        TextureCompressMenu.Begin();
        BackgroundWorker.Instance.AddBackgroudOperation(() =>
        {
            int nTexSize;
            /*
            Color32[] pixels = null;
            bool enalbeAlpha;

            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(file);
            TextureAlphaMaskTools.GetPixels(file, out nTexSize, ref pixels);
            Sprite[] sprites = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(file).OfType<Sprite>().ToArray();
            foreach(var spt in  sprites)
            {
                //ui.uvTL = spt.uv[0];//0,1
                //ui.uvTR = spt.uv[1];//1,1
                //ui.uvBL = spt.uv[2];//0,0
                //ui.uvBR = spt.uv[3];//1,0

                int w = (int)spt.rect.size.x;
                int h = (int)spt.rect.size.y;

                int offsetx = (int)(spt.uv[2].x * tex.width);
                int offsety = (int)(spt.uv[2].y * tex.height);
                Debug.Log(spt.name+" "+offsetx + " " + offsety);

                Color32[] _pixels = new Color32[w * h];
                for(int i=0;i<w;++i)
                {
                    for (int j = 0; j < h; ++j)
                    {
                        int idx = i + w * (h - j - 1);
                        int offsetIdx = (offsetx + i) + tex.width * (offsety - j - 1);
                        _pixels[idx] = pixels[offsetIdx];
                    }
                }
                Texture2D tex2d = new Texture2D(w,h, TextureFormat.RGBA32, false);
                tex2d.SetPixels32(_pixels);
                var data = tex2d.EncodeToPNG();
                CFileManager.WriteFile("c://users/" + spt.name + ".png", data);
            }
            */
            bool enalbeAlpha;
            Color32[] pixels = null;
            CTimerManager.Instance.AddTimer(0, 1, (_seq) =>
            {

                TextureCompressTools.CheckSpritesAlpha(file, out nTexSize, ref pixels, out enalbeAlpha, (spt, _pixels) =>
                {
                    int w = (int)spt.rect.size.x;
                    int h = (int)spt.rect.size.y;
                    Texture2D tex2d = new Texture2D(w, h, TextureFormat.RGBA32, false);
                    tex2d.SetPixels32(_pixels);
                    var data = tex2d.EncodeToPNG();
                    CFileManager.WriteFile(GameUtility.WritablePath + spt.name + ".png", data);
                });
                Debug.LogError("*alpha通道：" + enalbeAlpha);
                Application.OpenURL(GameUtility.WritablePath);
                TextureCompressMenu.End();
            });
        });
    }

    /*
    C#的像素排列：从左到右,再从下到上
    unity的像素排列：从左到右,再从上到下
    */

//     [MenuItem("Temp/切割小图")]
//     static void Texture2dSplit_0()
//     {
//         string strPngFileName = "c://a.png";
//         var bmp = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile(strPngFileName, false);
//         bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
//         var pixels = new Color32[bmp.Width * bmp.Height];
//         for (int j = bmp.Height - 1; j >= 0; --j)
//         {
//             for (int i = 0; i < bmp.Width; ++i)
//             {
//                 var c = bmp.GetPixel(i, bmp.Height - j - 1);
//                 pixels[bmp.Width * j + i] = new Color32(c.R, c.G, c.B, c.A);
//             }
//         }
//         {
//             Texture2D tex2d = new Texture2D(bmp.Width, bmp.Height, TextureFormat.RGBA32, false);
//             tex2d.SetPixels32(pixels);
//             var data = tex2d.EncodeToPNG();
//             string savefilename = "c://users/test.png";
//             CFileManager.WriteFile(savefilename, data);
//         }

//         int row = 4;//4行
//         int col = 3;//3列
//         var w = bmp.Width/col;
//         var h = bmp.Height/row;

//         int pngSeq = 0;
//         var _pixels = new Color32[w * h];
//         Bitmap _bmp = new Bitmap(w, h);
//         for (int i = 0; i < col;++i)
//         {
//             for (int j = 0; j < row; ++j)
//             {
//                 int offsetx = i * w;
//                 int offsety = j * h;
//                 for (int y = h - 1; y >=0; --y)
//                 {
//                     for (int x = 0; x < w; ++x)
//                     {
//                         int idx = w * y + x;
//                         int offsetIdx = bmp.Width * (offsety + y) + (offsetx + x);
//                         _pixels[idx] = pixels[offsetIdx];
//                     }
//                 }

//                 int outW;
//                 int outH;
//                 Color32[] outPixels;
//                 ClipAlpha(w,h, _pixels,out outW,out outH,out outPixels);
// #if true
//                 Texture2D tex2d = new Texture2D(outW, outH, TextureFormat.RGBA32, false);
//                 tex2d.SetPixels32(outPixels);
//                 var data = tex2d.EncodeToPNG();
//                 string savefilename = "c://users/" + (++pngSeq) + ".png";
//                 CFileManager.WriteFile(savefilename, data);
// #else
//                 _bmp.Save("c://users/" + i + ".png");
// #endif
//                 LOG.Error("save:" + savefilename);
//             }
//         }

//     }

    static void ClipAlpha(int inW,int inH, Color32[] inPixels,out int outW,out int outH, out Color32[] outPixels)
    {
        int minX = 0;
        int minY = 0;
        int maxX = inW - 1;
        int maxY = inH - 1;
        bool flag = true;
        //clip left
        for (flag = true; minX < inW && flag; ++minX)
        {
            for(int i=minY; i <= maxY; ++i)
            {
                var p = inPixels[minX + inW * i];
                if(p.a > 0)
                {
                    flag = false;
                    break;
                }
            }
        }

        //clip right
        for (flag = true; maxX >= 0 && flag; --maxX)
        {
            for (int i = minY; i <= maxY; ++i)
            {
                var p = inPixels[maxX + inW * i];
                if (p.a > 0)
                {
                    flag = false;
                    break;
                }
            }
        }

        //clip top
        for (flag = true; maxY >= 0 && flag; --maxY)
        {
            for (int i = minX; i <= maxX; ++i)
            {
                var p = inPixels[i + inW * maxY];
                if (p.a > 0)
                {
                    flag = false;
                    break;
                }
            }
        }
        //clip bottom
        for (flag = true; minY < inH && flag; ++minY)
        {
            for (int i = minX; i <= maxX; ++i)
            {
                var p = inPixels[i + inW * minY];
                if (p.a > 0)
                {
                    flag = false;
                    break;
                }
            }
        }

        outH = maxY - minY + 1;
        outW = maxX - minX + 1;
        outPixels = new Color32[outW * outH];

        int offsetx = minX;
        int offsety = minY;
        for (int y = outH - 1; y >= 0; --y)
        {
            for (int x = 0; x < outW; ++x)
            {
                int idx = outW * y + x;
                int offsetIdx = inW * (offsety + y) + (offsetx + x);
                outPixels[idx] = inPixels[offsetIdx];
            }
        }
    }
}
#endif
using FreeImageAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ImageHelper
{
    public const int MIN_ALHPA = 220;
    public static readonly int[] s_texture2d_size = { 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192 };
    public class ImageData
    {
        public int nTexSize;
        public Color32[] pixels;
    }

    static Dictionary<string, ImageData> m_dataDict = new Dictionary<string, ImageData>();

    public static void Clear()
    {
        m_dataDict.Clear();
    }
    public static void GetPixels(string strPngFileName, out int size, ref Color32[] pixels)
    {
        ImageData data;
        lock (m_dataDict)
        {
            if (m_dataDict.TryGetValue(strPngFileName, out data))
            {
                size = data.nTexSize;
                pixels = data.pixels;
                return;
            }
        }
        size = 32;
        try
        {
#if USE_DRAWING
            var bmp = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile(strPngFileName, false);
            foreach (var s in s_texture2d_size)
            {
                if (s >= bmp.Width && s >= bmp.Height)
                {
                    size = s;
                    break;
                }
            }

            pixels = new Color32[bmp.Width * bmp.Height];
            for (int i = 0; i < bmp.Width; ++i)
            {
                for (int j = 0; j < bmp.Height; ++j)
                {
                    var c = bmp.GetPixel(i, j);
                    pixels[i + bmp.Width * (bmp.Height - j - 1)] = new Color32(c.R, c.G, c.B, c.A);
                }
            }
            lock (m_dataDict)
            {
                if (!m_dataDict.ContainsKey(strPngFileName))
                {
                    data = new ImageData();
                    m_dataDict.Add(strPngFileName, data);
                    data.nTexSize = size;
                    data.pixels = pixels;
                }
            }
#else
            FIBITMAP fib = FreeImage.LoadEx(strPngFileName);
            var width = (int)FreeImage.GetWidth(fib);
            var height = (int)FreeImage.GetHeight(fib);
            foreach (var s in s_texture2d_size)
            {
                if (s >= width && s >= height)
                {
                    size = s;
                    break;
                }
            }

            pixels = new Color32[width * height];
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    RGBQUAD value;
                    FreeImage.GetPixelColor(fib, (uint)i, (uint)j, out value);
                    var c = value.Color;
                    pixels[i + width * (height - j - 1)] = new Color32(c.R, c.G, c.B, c.A);
                }
            }
            lock (m_dataDict)
            {
                if (!m_dataDict.ContainsKey(strPngFileName))
                {
                    data = new ImageData();
                    m_dataDict.Add(strPngFileName, data);
                    data.nTexSize = size;
                    data.pixels = pixels;
                }
            }
#endif
        }
        catch (System.Exception ex)
        {
            LOG.Error(ex);
        }
    }


    public static bool CheckPngAlpha(string strPngFileName, out int size)
    {
        size = 32;
        try
        {
            FIBITMAP fib = FreeImage.LoadEx(strPngFileName);
            var width = (int)FreeImage.GetWidth(fib);
            var height = (int)FreeImage.GetHeight(fib);
            int t = Mathf.Max(width, height);
            foreach (var s in s_texture2d_size)
            {
                size = s;
                if (t > 512)
                {
                    if (s * 1.5f > t)//超过0.5倍是才扩大
                    {
                        break;
                    }
                }
                else if (s >= t)
                {
                    break;
                }
            }

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    RGBQUAD value;
                    FreeImage.GetPixelColor(fib, (uint)i, (uint)j, out value);
                    var c = value.Color;
                    if (c.A <= ImageHelper.MIN_ALHPA)
                    {
                        return true;
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            LOG.Error(ex);
        }
        return false;
    }
}

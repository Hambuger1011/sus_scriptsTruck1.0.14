using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class BitUtils
{
    public static int GetInt32Mask(int x, int y)
    {
        int ret = (x << 15 | y);//高16位存储x，低16位存储y
        return ret;
    }


    public static void PaseBit32Mask(int i, out int x, out int y)
    {
        x = i >> 15;
        y = (x << 15) ^ i;
    }



    #region 32
    public static int SetBit32Mask(int value, int index, bool on)
    {
        if (on)
        {
            value |= (1 << index);
        }
        else
        {
            value &= ~(1 << index);
        }
        return value;
    }

    public static bool GetBit32Mask(int value, int index)
    {
        return (value & (1 << index)) > 0;
    }

    public static string GetBit32MaskString(int value)
    {
        StringBuilder sb = new StringBuilder();
        int i = 0;
        while (value > 0)
        {
            if (GetBit32Mask(value, 0))
            {
                if (i != 0)
                {
                    sb.Append(",");
                }
                sb.Append(i + 1);
            }
            ++i;
            value >>= 1;
        }
        return sb.ToString();
    }


    public static int GetBit32Count(int value)
    {
        int k = 0;
        int i = 0;
        while (value > 0)
        {
            if (GetBit32Mask(value, 0))
            {
                ++k;
            }
            ++i;
            value >>= 1;
        }
        return k;
    }
    #endregion


    #region 64
    public static long SetBit64Mask(long value, int index, bool on)
    {
        if (on)
        {
            value |= (1L << index);
        }
        else
        {
            value &= ~(1L << index);
        }
        return value;
    }

    public static bool GetBit64Mask(long value, int index)
    {
        return (value & (1L << index)) > 0;
    }

    public static int GetBit64Count(long value)
    {
        int k = 0;
        int i = 0;
        while (value > 0)
        {
            if (GetBit64Mask(value, 0))
            {
                ++k;
            }
            ++i;
            value >>= 1;
        }
        return k;
    }
    public static string Join(string separator, long value)
    {
        StringBuilder sb = new StringBuilder();
        bool isFirst = true;
        int i = 0;
        while (value > 0)
        {
            if (GetBit64Mask(value, 0))
            {
                if (isFirst)
                {
                    isFirst = false;
                }else
                {
                    sb.Append(separator);
                }
                sb.Append(i + 1);
            }
            ++i;
            value >>= 1;
        }
        return sb.ToString();
    }
    #endregion
}

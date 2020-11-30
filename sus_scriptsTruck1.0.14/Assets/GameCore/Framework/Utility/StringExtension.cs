using System;
using UnityEngine;

public static class StringExtension
{
    public static readonly string asset_str = "Assets/";

    public static string RemoveExtension(this string s)
    {
        if (s == null)
        {
            return null;
        }
        int findIdx = s.LastIndexOf('.');
        if (findIdx == -1)
        {
            return s;
        }
        return s.Substring(0, findIdx);
    }

    public static string FullPathToAssetPath(this string s)
    {
        if (s == null)
        {
            return null;
        }
        string text = StringExtension.asset_str + s.Substring(Application.dataPath.Length + 1);
        return text.Replace('\\', '/');
    }

    public static string AssetPathToFullPath(this string s)
    {
        if (s == null)
        {
            return null;
        }
        if (!s.StartsWith(StringExtension.asset_str))
        {
            return null;
        }
        string text = Application.dataPath;
        text += "/";
        return text + s.Remove(0, StringExtension.asset_str.Length);
    }

    public static string GetFileExtension(this string s)
    {
        int findIdx = s.LastIndexOf('.');
        if (findIdx == -1)
        {
            return null;
        }
        return s.Substring(findIdx + 1);
    }

    public static string GetFileExtensionUpper(this string s)
    {
        string fileExtension = s.GetFileExtension();
        if (fileExtension == null)
        {
            return null;
        }
        return fileExtension.ToUpper();
    }

    public static string GetHierarchyName(this GameObject go)
    {
        if (go == null)
        {
            return "<null>";
        }
        string text = string.Empty;
        while (go != null)
        {
            if (string.IsNullOrEmpty(text))
            {
                text = go.name;
            }
            else
            {
                text = go.name + "." + text;
            }
            Transform parent = go.transform.parent;
            go = ((!(parent != null)) ? null : parent.gameObject);
        }
        return text;
    }

    public static int JavaHashCode(this string s)
    {
        int val = 0;
        int length = s.Length;
        if(length >= 30)
        {
            LOG.Error("字符串长度超出范围:" + s);
        }
        if (length > 0)
        {
            int num2 = 0;
            for (int i = 0; i < length; i++)
            {
                char c = s[num2++];
                val = 31 * val + (int)c;
            }
        }
        return val;
    }

    public static int JavaHashCodeIgnoreCase(this string s)
    {
        int val = 0;
        int length = s.Length;
        if (length > 0)
        {
            int num2 = 0;
            for (int i = 0; i < length; i++)
            {
                char c = s[num2++];
                if (c >= 'A' && c <= 'Z')
                {
                    c += ' ';
                }
                val = 31 * val + (int)c;
            }
        }
        return val;
    }
}

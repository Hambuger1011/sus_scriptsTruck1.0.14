using Object = UnityEngine.Object;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Framework;

public static class Utility
{
    private static readonly int CHINESE_CHAR_START = Convert.ToInt32("4e00", 16);
    private static readonly int CHINESE_CHAR_END = Convert.ToInt32("9fff", 16);
    
    public static bool IsChineseChar(char key)
    {
        int num = Convert.ToInt32(key);
        if (num >= CHINESE_CHAR_START && num <= CHINESE_CHAR_END)
        {
            return true;
        }
        return false;
    }

    public static bool IsSpecialChar(char key)
    {
        if (!IsChineseChar(key) && !char.IsLetter(key) && !char.IsNumber(key))
        {
            return true;
        }
        return false;
    }

    public static bool IsValidText(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            if (IsSpecialChar(text[i]))
            {
                return false;
            }
        }
        return true;
    }

    public static int GetByteCount(string inputStr)
    {
        int num = 0;
        for (int i = 0; i < inputStr.Length; i++)
        {
            num = ((!IsQuanjiaoChar(inputStr.Substring(i, 1))) ? (num + 1) : (num + 2));
        }
        return num;
    }

    public static bool IsQuanjiaoChar(string inputStr)
    {
        return Encoding.Default.GetByteCount(inputStr) > 1;
    }




    public static void VibrateHelper()
    {
        //if (GameSettings.EnableVibrate)
        {
            Handheld.Vibrate();
        }
    }

    public static T DeepCopyByReflection<T>(T obj)
    {
        if (obj == null)
        {
            return default(T);
        }
        Type type = obj.GetType();
        if (!(((object)obj) is string) && !type.IsValueType)
        {
            if (type.IsArray)
            {
                Type type2 = Type.GetType(type.FullName.Replace("[]", string.Empty));
                Array array = obj as Array;
                Array array2 = Array.CreateInstance(type2, array.Length);
                for (int i = 0; i < array.Length; i++)
                {
                    array2.SetValue(DeepCopyByReflection(array.GetValue(i)), i);
                }
                return (T)Convert.ChangeType(array2, obj.GetType());
            }
            object obj2 = Activator.CreateInstance(obj.GetType());
            PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            PropertyInfo[] array3 = properties;
            foreach (PropertyInfo propertyInfo in array3)
            {
                object value = propertyInfo.GetValue(obj, null);
                if (value != null)
                {
                    propertyInfo.SetValue(obj2, DeepCopyByReflection(value), null);
                }
            }
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            FieldInfo[] array4 = fields;
            foreach (FieldInfo fieldInfo in array4)
            {
                try
                {
                    fieldInfo.SetValue(obj2, DeepCopyByReflection(fieldInfo.GetValue(obj)));
                }
                catch
                {
                }
            }
            return (T)obj2;
        }
        return obj;
    }

    public static T DeepCopyBySerialization<T>(T obj)
    {
        object obj2 = default(object);
        using (MemoryStream memoryStream = new MemoryStream())
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, obj);
            memoryStream.Seek(0L, SeekOrigin.Begin);
            obj2 = binaryFormatter.Deserialize(memoryStream);
            memoryStream.Close();
        }
        return (T)obj2;
    }


    public static void SetActiveEx(this GameObject obj, bool bActive)
    {
        if (obj != null && obj.activeSelf != bActive)
        {
            obj.SetActive(bActive);
        }
    }

    public static void CustomSetActive(this GameObject obj, bool bActive)
    {
        if (obj != null && obj.activeSelf != bActive)
        {
            obj.SetActive(bActive);
        }
    }

}

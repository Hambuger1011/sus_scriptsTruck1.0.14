using Framework;

using System;
using System.Text;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public static class StringHelper
{
	public static StringBuilder Formater = new StringBuilder(1024);

	public static void ClearFormater()
	{
		Formater.Remove(0, Formater.Length);
	}

	public static string GetBase64Str(byte[] data)
	{
		string text = string.Empty;
		try
		{
			text = BytesToString(data);
			byte[] bytes = Convert.FromBase64String(text);
			text = Encoding.UTF8.GetString(bytes);
			if (text == null)
			{
				text = string.Empty;
				return text;
			}
			return text;
		}
		catch (Exception ex)
		{
			Debug.LogError("Get base 64 string wrong " + ex.ToString() + ((data == null) ? (-1) : data.Length));
			return text;
		}
	}

	public static byte[] ToBase64StrBytes(string str)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(str);
		string s = Convert.ToBase64String(bytes);
		return Encoding.UTF8.GetBytes(s);
	}

	public static string BytesToString(byte[] bytes)
	{
		return Encoding.UTF8.GetString(bytes).TrimEnd(default(char));
	}
	
	public static bool IsEmpty(string data)
	{
		if (null != data && "" != data)
		{
			return false;
		}
		else
		{
			return true;
		}
	}
	
	public static bool IsEquals(string txtLeft, string txtRight)
	{
		if (txtLeft != null)
		{
			return txtLeft.Equals(txtRight);
		}

		if (txtRight != null)
		{
			return txtRight.Equals(txtLeft);
		}

		return true;
	}

	public static string BytesToString_FindFristZero(byte[] bytes)
	{
		if (bytes == null)
		{
			return string.Empty;
		}
		int i;
		for (i = 0; i < bytes.Length && bytes[i] != 0; i++)
		{
		}
		return Encoding.UTF8.GetString(bytes, 0, i);
	}

	public static string GetCutString(string orgStr, int newLength, bool isPreCut = true)
	{
		if (newLength <= 0)
		{
			return orgStr;
		}
		string text = orgStr;
		if (text.Length > newLength)
		{
			if (isPreCut)
			{
				text = text.Substring(text.Length - newLength, newLength);
				text = "..." + text;
			}
			else
			{
				text = text.Substring(0, newLength);
				text += "...";
			}
		}
		return text;
	}

	public static string BytesToString(string str)
	{
		return str;
	}

	public static string UTF8BytesToString(ref byte[] str)
	{
		try
		{
			return (str == null) ? null : Encoding.UTF8.GetString(str).TrimEnd(default(char));
			IL_002a:
			string result;
			return result;
		}
		catch (Exception)
		{
			return null;
			IL_0037:
			string result;
			return result;
		}
	}

	public static string ASCIIBytesToString(byte[] data)
	{
		if (data == null)
		{
			return null;
		}
		try
		{
			return Encoding.ASCII.GetString(data).TrimEnd(default(char));
			IL_0024:
			string result;
			return result;
		}
		catch (Exception)
		{
			return null;
			IL_0031:
			string result;
			return result;
		}
	}

	public static string UTF8BytesToString(ref string str)
	{
		return str;
	}

	public static string GetRandomStr(string orgStr, int length)
	{
		if (orgStr != null && length >= 0 && length <= orgStr.Length)
		{
			int num = length;
			string text = string.Empty;
			while (num > 0)
			{
				int index = UnityEngine.Random.Range(0, orgStr.Length);
				text += orgStr[index];
				num--;
			}
			return text;
		}
		return null;
	}

	public static void StringToUTF8Bytes(string str, ref byte[] buffer)
	{
		if (str != null && buffer != null)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			if (bytes.Length >= buffer.Length)
			{
				FillErrorCodeToBuf(ref buffer);
			}
			else
			{
				Buffer.BlockCopy(bytes, 0, buffer, 0, bytes.Length);
				buffer[bytes.Length] = 0;
			}
		}
	}

	private static void FillErrorCodeToBuf(ref byte[] buffer)
	{
		try
		{
			buffer[0] = 79;
			buffer[1] = 86;
			buffer[2] = 69;
			buffer[3] = 82;
			buffer[4] = 70;
			buffer[5] = 76;
			buffer[6] = 79;
			buffer[7] = 87;
			buffer[8] = 48;
			buffer[9] = 88;
			buffer[10] = 67;
			buffer[11] = 67;
			buffer[12] = 67;
			buffer[13] = 67;
			buffer[14] = 67;
			buffer[15] = 67;
			buffer[16] = 0;
		}
		catch (Exception)
		{
		}
	}

	private static void FillErrorCodeToSBuf(ref sbyte[] buffer)
	{
		try
		{
			buffer[0] = 79;
			buffer[1] = 86;
			buffer[2] = 69;
			buffer[3] = 82;
			buffer[4] = 70;
			buffer[5] = 76;
			buffer[6] = 79;
			buffer[7] = 87;
			buffer[8] = 48;
			buffer[9] = 88;
			buffer[10] = 67;
			buffer[11] = 67;
			buffer[12] = 67;
			buffer[13] = 67;
			buffer[14] = 67;
			buffer[15] = 67;
			buffer[16] = 0;
		}
		catch (Exception)
		{
		}
	}

	public static bool IsAvailableString(string str)
	{
		int num = 0;
		int i = 0;
		char c = '\0';
		bool flag = false;
		int length = str.Length;
		while (i < length)
		{
			c = str[i];
			if (flag)
			{
				if (c >= '\udc00' && c <= '\udfff')
				{
					num += 4;
					flag = false;
					goto IL_0102;
				}
				Debug.Log(string.Format("invalid utf-16 sequence at {0} (missing surrogate tail)", i));
				return false;
			}
			if (c < '\u0080')
			{
				for (; i < length && str[i] < '\u0080'; i++)
				{
					num++;
				}
				continue;
			}
			if (c < 'ࠀ')
			{
				num += 2;
			}
			else if (c >= '\ud800' && c <= '\udbff')
			{
				flag = true;
			}
			else
			{
				if (c >= '\udc00' && c <= '\udfff')
				{
					Debug.Log(string.Format("invalid utf-16 sequence at {0} (missing surrogate head)", i));
					return false;
				}
				num += 3;
			}
			goto IL_0102;
			IL_0102:
			i++;
		}
		return true;
	}

	public static void StringToUTF8Bytes(string str, ref sbyte[] buffer)
	{
		if (str != null && buffer != null)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			if (bytes.Length >= buffer.Length)
			{
				FillErrorCodeToSBuf(ref buffer);
			}
			else
			{
				Buffer.BlockCopy(bytes, 0, buffer, 0, bytes.Length);
				buffer[bytes.Length] = 0;
			}
		}
	}

    //https://www.cnblogs.com/chenggg/p/12533168.html
    public unsafe static void ToLower(string str)
    {
        fixed (char* c = str)
        {
            int length = str.Length;
            for (int i = 0; i < length; ++i)
            {
                c[i] = char.ToLower(c[i]);
            }
        }
    }

    public unsafe static int Split(string str, char split, string[] toFill)
    {
        if (str.Length == 0)
        {
            toFill[0] = string.Empty;
            return 1;
        }
        var length = str.Length;
        int ret = 0;
        fixed (char* p = str)
        {
            var start = 0;
            for (int i = 0; i < length; ++i)
            {
                if (p[i] == split)
                {
                    toFill[ret++] = (new string(p, start, i - start));
                    start = i + 1;
                    if (i == length - 1)
                        toFill[ret++] = string.Empty;
                }
            }
            if (start < length)
            {
                toFill[ret++] = (new string(p, start, length - start));
            }
        }
        return ret;
    }

    public unsafe static void Substring(string str, int start, int length = 0)
    {
        if (length <= 0)
        {
            length = str.Length - start;
        }
        if (length > str.Length - start)
        {
            throw new IndexOutOfRangeException("{length} > {str.Length} - {start}");
        }
        fixed (char* c = str)
        {
            UnsafeUtility.MemMove(c, c + start, sizeof(char) * length);
        }
        SetLength(str, length);
    }
    public unsafe static void SetLength(this string str, int length)
    {
        fixed (char* s = str)
        {
            int* ptr = (int*)s;
            ptr[-1] = length;
            s[length] = '\0';
        }
    }
}

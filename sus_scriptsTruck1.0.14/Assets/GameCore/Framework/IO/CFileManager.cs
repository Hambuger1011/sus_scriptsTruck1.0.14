using Framework;

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

[XLua.LuaCallCSharp]
public class CFileManager
{
    public enum enFileOperation
    {
        ReadFile,
        WriteFile,
        DeleteFile,
        CreateDirectory,
        DeleteDirectory
    }

    public delegate void DelegateOnOperateFileFail(string fullPath, string fileInfo, enFileOperation fileOperation, Exception ex);

	public const uint c_fileBufferSize = 4194304u;

	private static string s_cachePath = null;

	public static string s_ifsExtractFolder = "Resources";

	private static string s_ifsExtractPath = null;

	private static MD5CryptoServiceProvider s_md5Provider = new MD5CryptoServiceProvider();

	//public static DelegateOnOperateFileFail s_delegateOnOperateFileFail = delegate
	//{
	//};

	private static byte[] s_fileBuffer = new byte[4194304];

	private static bool s_isFileBufferLocked = false;

	public static bool IsFileExist(string filePath)
	{
		return File.Exists(filePath);
	}

	public static bool IsDirectoryExist(string directory)
	{
		return Directory.Exists(directory);
	}

	public static bool CreateDirectory(string directory)
	{
		if (IsDirectoryExist(directory))
		{
			return true;
		}
		int num = 0;
		while (true)
		{
			try
			{
				Directory.CreateDirectory(directory);
				return true;
			}
			catch (Exception ex)
			{
				num++;
				if (num >= 3)
                {
                    Debug.LogError("创建目录失败:" + directory + "\n" + ex);
                    return false;
                }
			}
		}
	}

	public static bool DeleteDirectory(string directory)
	{
		if (!IsDirectoryExist(directory))
		{
			return true;
		}
		int num = 0;
		while (true)
		{
			try
			{
				Directory.Delete(directory, true);
				return true;
			}
			catch (Exception ex)
			{
				num++;
				if (num >= 3)
                {
                    Debug.LogError("删除目录失败:" + directory + "\n" + ex);
                    return false;
                }
			}
		}
	}

	public static int GetFileLength(string filePath)
	{
		if (!IsFileExist(filePath))
		{
			return 0;
		}
		int num = 0;
		while (true)
		{
			try
			{
				FileInfo fileInfo = new FileInfo(filePath);
				return (int)fileInfo.Length;
				IL_0023:;
			}
			catch (Exception ex)
			{
				num++;
				if (num >= 3)
                {
                    Debug.Log("Get FileLength of " + filePath + " Error! Exception = " + ex.ToString());
                    return 0;
                }
			}
		}
	}

    public static byte[] ReadRawFile(string filePath)
    {
        //if (!CFileManager.IsFileExist(filePath))
        //{
        //    return null;
        //}

        byte[] buff = null;
        if (Application.platform == RuntimePlatform.Android)
        {
            buff = AndroidHelper.ReadAssetFileBytes(filePath);
        }
        else
        {
            //***/data/app/com.xxx.xxx-1.apk!assets/yusong.unity3d
            //string path = Application.dataPath + "!assets/AnysdkConfig.txt";
            string path = Path.Combine(GameUtility.ReadonlyPath, filePath);
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                buff = new byte[fs.Length];
                fs.Read(buff, 0, buff.Length);
            }
        }
        return buff;
    }

    public static string ReadFileString(string filePath)
    {
        if (!IsFileExist(filePath))
        {
            return "";
        }
        return File.ReadAllText(filePath);
    }

    public static void WriteFileString(string filePath,string content)
    {
        File.WriteAllText(filePath,content);
    }

    public static byte[] ReadFile(string filePath)
	{
		if (!IsFileExist(filePath))
		{
			return null;
		}
		byte[] array = null;
		int num = 0;
		while (true)
		{
			Exception ex = null;
			try
			{
				array = File.ReadAllBytes(filePath);
			}
			catch (Exception ex2)
			{
				array = null;
				ex = ex2;
			}
			if (array != null && array.Length > 0)
			{
				break;
			}
			num++;
			if (num < 3)
			{
				continue;
			}
			Debug.LogError("读取文件失败:"+ filePath+"\n"+ex);
			return null;
		}
		return array;
	}

	public static uint ReadFile(string filePath, byte[] buffer, uint bufferSize)
	{
		if (!IsFileExist(filePath))
		{
			return 0u;
		}
		uint num = 0u;
		int num2 = 0;
		while (true)
		{
			Exception ex = null;
			FileStream fileStream = null;
			try
			{
				fileStream = File.OpenRead(filePath);
				LOG.Assert(bufferSize > (uint)fileStream.Length, string.Format("FileLength is larger than buffer!!! FileLength = {0}, BufferSize = {1}", fileStream.Length, bufferSize));
				num = (uint)fileStream.Read(buffer, 0, (int)fileStream.Length);
			}
			catch (Exception ex2)
			{
				num = 0u;
				ex = ex2;
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Close();
					fileStream.Dispose();
				}
			}
			if (num != 0)
			{
				break;
			}
			num2++;
			if (num2 < 3)
			{
				continue;
            }
            Debug.LogError("读取文件失败:" + filePath + "\n" + ex);
            return 0u;
		}
		return num;
	}

	public static bool WriteFile(string filePath, byte[] data)
	{
		DeleteFile(filePath);
		int num = 0;
		while (true)
		{
			try
			{
				File.WriteAllBytes(filePath, data);
				return true;
			}
			catch (Exception ex)
			{
				num++;
				if (num >= 3)
                {
                    DeleteFile(filePath);
                    Debug.LogError("写入文件失败:" + filePath + "\n" + ex);
                    return false;
                }
			}
		}
	}

	public static bool WriteFile(string filePath, byte[] data, int offset, int length)
	{
		DeleteFile(filePath);
		FileStream fileStream = null;
		int num = 0;
		while (true)
		{
			try
			{
				fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
				fileStream.Write(data, offset, length);
				fileStream.Close();
				fileStream.Dispose();
				return true;
			}
			catch (Exception ex)
			{
				if (fileStream != null)
				{
					fileStream.Close();
					fileStream.Dispose();
				}
				num++;
				if (num >= 3)
                {
                    DeleteFile(filePath);
                    Debug.LogError("写入文件失败:" + filePath + "\n" + ex);
                    return false;
                }
			}
		}
	}

	public static bool DeleteFile(string filePath)
	{
		if (!IsFileExist(filePath))
		{
			return true;
		}
		int num = 0;
		while (true)
		{
			try
			{
				File.Delete(filePath);
				return true;
			}
			catch (Exception ex)
			{
				num++;
				if (num >= 3)
                {
                    Debug.LogError("删除文件失败:" + filePath + "\n" + ex);
                    return false;
                }
			}
		}
	}

	public static void CopyFile(string srcFile, string dstFile)
	{
		File.Copy(srcFile, dstFile, true);
	}

	public static string GetFileMd5(string filePath)
	{
        try
        {
            var buffer = ReadFile(filePath);
            if(buffer == null)
            {
                //Debug.LogError("[-]GetFileMd5:加载文件失败" + filePath);
                return string.Empty;
            }
            var md5 = s_md5Provider.ComputeHash(buffer);
            return BitConverter.ToString(md5).Replace("-", string.Empty);

        }
        catch(Exception ex)
        {
            LOG.Error("fuck:"+filePath+"\n"+ex);
        }
        return string.Empty;
	}

	public static string GetMd5(byte[] data)
	{
        var res = BitConverter.ToString(s_md5Provider.ComputeHash(data)).Replace("-", string.Empty);
        //Debug.LogError(Encoding.UTF8.GetString(data) + "\n" + res);
        return res;
	}

	public static string GetStringMd5(string str)
	{
		var res = BitConverter.ToString(s_md5Provider.ComputeHash(Encoding.UTF8.GetBytes(str))).Replace("-", string.Empty);
        return res;
    }

	public static string CombinePath(string path1, string path2)
	{
		if (path1.LastIndexOf('/') != path1.Length - 1)
		{
			path1 += "/";
		}
		if (path2.IndexOf('/') == 0)
		{
			path2 = path2.Substring(1);
		}
		return path1 + path2;
	}

	public static string CombinePaths(params string[] values)
	{
		if (values.Length <= 0)
		{
			return string.Empty;
		}
		if (values.Length == 1)
		{
			return CombinePath(values[0], string.Empty);
		}
		if (values.Length > 1)
		{
			string text = CombinePath(values[0], values[1]);
			for (int i = 2; i < values.Length; i++)
			{
				text = CombinePath(text, values[i]);
			}
			return text;
		}
		return string.Empty;
	}

	public static string RelativeToAbsolutePath(string relativePath)
	{
		List<string> list = new List<string>();
		relativePath = relativePath.Replace('\\', '/');
		string[] array = relativePath.Split('/');
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (!(text == "."))
			{
				if (text == "..")
				{
					if (list.Count > 0)
					{
						list.RemoveAt(list.Count - 1);
					}
					else
					{
						list.Add(text);
					}
				}
				else
				{
					list.Add(text);
				}
			}
		}
		return string.Join("/", list.ToArray());
	}

	public static string GetStreamingAssetsPathWithHeader(string fileName)
	{
		return Path.Combine(Application.streamingAssetsPath, fileName);
	}

	public static string GetCachePath()
	{
		if (s_cachePath == null)
		{
			s_cachePath = Application.persistentDataPath;
		}
		return s_cachePath;
	}

	public static string GetCachePath(string fileName)
	{
		return CombinePath(GetCachePath(), fileName);
	}

	public static string GetCachePathWithHeader(string fileName)
	{
		return GetLocalPathHeader() + GetCachePath(fileName);
	}

	public static string GetIFSExtractPath()
	{
		if (s_ifsExtractPath == null)
		{
			s_ifsExtractPath = CombinePath(GetCachePath(), s_ifsExtractFolder);
		}
		return s_ifsExtractPath;
	}

	public static string GetFullName(string fullPath)
	{
		if (fullPath == null)
		{
			return null;
		}
		int num = fullPath.LastIndexOf("/");
		if (num > 0)
		{
			return fullPath.Substring(num + 1, fullPath.Length - num - 1);
		}
		return fullPath;
	}

	public static string EraseExtension(string fullName)
	{
		if (fullName == null)
		{
			return null;
		}
		int num = fullName.LastIndexOf('.');
		if (num > 0)
		{
			return fullName.Substring(0, num);
		}
		return fullName;
	}

	public static string GetExtension(string fullName)
	{
		int num = fullName.LastIndexOf('.');
		if (num > 0 && num + 1 < fullName.Length)
		{
			return fullName.Substring(num);
		}
		return string.Empty;
	}

	public static string GetFullDirectory(string fullPath)
	{
		return Path.GetDirectoryName(fullPath).Replace('\\', '/');
	}

	public static bool ClearDirectory(string fullPath)
	{
		try
		{
			string[] files = Directory.GetFiles(fullPath);
			for (int i = 0; i < files.Length; i++)
			{
				File.Delete(files[i]);
			}
			string[] directories = Directory.GetDirectories(fullPath);
			for (int j = 0; j < directories.Length; j++)
			{
				Directory.Delete(directories[j], true);
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool ClearDirectory(string fullPath, string[] fileExtensionFilter, string[] folderFilter)
	{
		try
		{
			if (fileExtensionFilter != null)
			{
				string[] files = Directory.GetFiles(fullPath);
				for (int i = 0; i < files.Length; i++)
				{
					if (fileExtensionFilter != null && fileExtensionFilter.Length > 0)
					{
						int num = 0;
						while (num < fileExtensionFilter.Length)
						{
							if (!files[i].Contains(fileExtensionFilter[num]))
							{
								num++;
								continue;
							}
							DeleteFile(files[i]);
							break;
						}
					}
				}
			}
			if (folderFilter != null)
			{
				string[] directories = Directory.GetDirectories(fullPath);
				for (int j = 0; j < directories.Length; j++)
				{
					if (folderFilter != null && folderFilter.Length > 0)
					{
						int num2 = 0;
						while (num2 < folderFilter.Length)
						{
							if (!directories[j].Contains(folderFilter[num2]))
							{
								num2++;
								continue;
							}
							DeleteDirectory(directories[j]);
							break;
						}
					}
				}
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static string GetFullPathInResources(string fileFullPath)
	{
		fileFullPath = fileFullPath.Replace("\\", "/");
		string text = "Assets/Resources/";
		int num = fileFullPath.IndexOf(text);
		if (num >= 0)
		{
			return fileFullPath.Substring(num + text.Length);
		}
		return string.Empty;
	}

	public static string GetFullPathInProject(string fileFullPath)
	{
		fileFullPath = fileFullPath.Replace("\\", "/");
		string text = "Project/";
		int num = fileFullPath.IndexOf(text);
		if (num >= 0)
		{
			return fileFullPath.Substring(num + text.Length);
		}
		return string.Empty;
	}

	public static string GetLocalPathHeader()
	{
		return "file://";
	}

	public static byte[] LockFileBuffer()
	{
		LOG.Assert(!s_isFileBufferLocked, "Buffer has been locked!!!");
		s_isFileBufferLocked = true;
		return s_fileBuffer;
	}

	public static void UnLockFileBuffer()
	{
		s_isFileBufferLocked = false;
	}
}

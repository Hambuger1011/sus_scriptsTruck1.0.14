using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System;
using pb;
using UnityEngine.UI;

[XLua.LuaCallCSharp, XLua.Hotfix]
public class PluginTools : Singleton<PluginTools>
{
    // Use this for initialization
    private static string outputPath = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";

    // Use this for initialization
    void TailTest()
    {
        //ListAllAppliction();
        UnityEngine.Debug.Log("当前应用：" + Process.GetCurrentProcess().ProcessName + " 进程ID: " +
                              Process.GetCurrentProcess().Id);

        //打开外部应用
        if (CheckProcess("chrome"))
            return;
        else
            StartProcess(outputPath);

        //杀死应用进程
        KillProcess("chrome");
    }

    /// <summary>
    /// 开启应用
    /// </summary>
    /// <param name="ApplicationPath"></param>
    public void StartProcess(string ApplicationPath)
    {
        UnityEngine.Debug.Log("打开本地应用");
        Process foo = new Process();
        foo.StartInfo.FileName = ApplicationPath;
        foo.Start();
    }

    /// <summary>
    /// 检查应用是否正在运行
    /// </summary>
    public bool CheckProcess(string processName)
    {
        bool isRunning = false;
        Process[] processes = Process.GetProcesses();
        int i = 0;
        foreach (Process process in processes)
        {
            try
            {
                i++;
                if (!process.HasExited)
                {
                    if (process.ProcessName.Contains(processName))
                    {
                        UnityEngine.Debug.Log(processName + "正在运行");
                        isRunning = true;
                        continue;
                    }
                    else if (!process.ProcessName.Contains(processName) && i > processes.Length)
                    {
                        UnityEngine.Debug.Log(processName + "没有运行");
                        isRunning = false;
                    }
                }
            }
            catch (Exception ep)
            {
            }
        }

        return isRunning;
    }

    /// <summary>
    /// 列出已开启的应用
    /// </summary>
    public void ListAllAppliction()
    {
        Process[] processes = Process.GetProcesses();
        int i = 0;
        foreach (Process process in processes)
        {
            try
            {
                if (!process.HasExited)
                {
                    UnityEngine.Debug.Log("应用ID:" + process.Id + "应用名:" + process.ProcessName);
                }
            }
            catch (Exception ep)
            {
            }
        }
    }

    /// <summary>
    /// 杀死进程
    /// </summary>
    /// <param name="processName">应用程序名</param>
    public void KillProcess(string processName)
    {
        Process[] processes = Process.GetProcesses();
        foreach (Process process in processes)
        {
            try
            {
                if (!process.HasExited)
                {
                    if (process.ProcessName == processName)
                    {
                        process.Kill();
                        UnityEngine.Debug.Log("已杀死进程");
                    }
                }
            }
            catch (System.InvalidOperationException)
            {
                //UnityEngine.Debug.Log("Holy batman we've got an exception!");
            }
        }
    }

    /// <summary>
    /// 杀死当前进程
    /// </summary>
    /// <param name="processName">应用程序名</param>
    public void KillRunningProcess()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        return;
#endif
        string processName = Process.GetCurrentProcess().ProcessName;
        KillProcess(processName);
    }


    public void ContentSizeFitterRefresh(Transform trans = null)
    {
        if (trans != null)
        {
            while (trans != null)
            {
                ContentSizeFitter csf = trans.GetComponent<ContentSizeFitter>();
                if (csf)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(trans.GetComponent<RectTransform>());
                }

                trans = trans.parent;
            }
        }
    }

    private List<t_Banned_Words> BanWords;

    public string ReplaceBannedWords(string str)
    {
        if (BanWords == null)
            BanWords = GameDataMgr.Instance.table.GetBannedWordsList();

        List<int> indexList = new List<int>();
        // str = Regex.Replace(str, word, "x",RegexOptions.IgnoreCase);
        foreach (t_Banned_Words banWord in BanWords)
        {
            string word = banWord.BannedWord.ToLower();
            string strLower = str.ToLower();
            while (true)
            {
                int index = strLower.LastIndexOf(word, StringComparison.Ordinal);
                if (index >= 0)
                {
                    indexList.Add(index);
                    strLower = strLower.Remove(index, word.Length);
                }
                else
                    break;
            }
            if (indexList.Count>0)
            {
                foreach (var index in indexList)
                {
                    str = str.Remove(index, word.Length);
                }
                indexList.Clear();
                if (str.Trim() == string.Empty)
                {
                    str = "Good";
                    break;
                }
            }
        }

        return str;
    }
}
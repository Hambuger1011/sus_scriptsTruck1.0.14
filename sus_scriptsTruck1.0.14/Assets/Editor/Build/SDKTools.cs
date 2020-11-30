using Facebook.Unity.Editor;
using Facebook.Unity.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

public static class SDKTools
{

    public static string SDK_ROOT = "Assets/Plugins/SDK/Dynamic/";

    public static void CopyDirectory(string src, string dst)
    {
        if (!Directory.Exists(src))
        {
            return;
        }
        dst += "/" + Path.GetFileName(src);
        Directory.CreateDirectory(dst);
        var dires = Directory.GetDirectories(src, "*", SearchOption.TopDirectoryOnly);
        foreach (var dir in dires)
        {
            CopyDirectory(dir, dst);
        }
        var files = Directory.GetFiles(src, "*", SearchOption.TopDirectoryOnly);
        foreach (var f in files)
        {
            File.Copy(f, dst + "/" + Path.GetFileName(f), true);
        }
    }

    static void ClearSDKRoot()
    {
        if (Directory.Exists(SDK_ROOT))
        {
            Directory.Delete(SDK_ROOT, true);
        }
        AssetDatabase.Refresh();
        Directory.CreateDirectory(SDK_ROOT);
    }

    public static void SetChannel_Onyx()
    {
        ClearSDKRoot();
        int p = 0;
        try
        {

            // var requireSDK = Directory.GetDirectories("Z_Work/SDK/Onyx", "*.*", SearchOption.TopDirectoryOnly);
            // foreach (var sdk in requireSDK)
            // {
            //     ++p;
            //     if (EditorUtility.DisplayCancelableProgressBar(string.Format("复制文件{0}({1}/{2})", sdk, p, requireSDK.Length), sdk, requireSDK.Length))
            //     {
            //         throw new Exception("用户停止");
            //     }
            //     CopyDirectory(sdk, SDK_ROOT);
            // }



            // FacebookSettings.AppIds[0] = "549205552159748";
            // FacebookSettings.ClientTokens[0] = "PM8XwC6Z7XBsAgMpx6WCCXJ7a6Q=";
            EditorUtility.SetDirty(FacebookSettings.Instance);
            ManifestMod.GenerateManifest();
        }
        finally
        {
            Thread.Sleep(1000);
            EditorUtility.ClearProgressBar();
        }
        AssetDatabase.Refresh();
    }

    
    public static void SetChannel_Huawei()
    {
        ClearSDKRoot();
        int p = 0;
        try
        {
            // var requireSDK = Directory.GetDirectories("Z_Work/SDK/Huawei", "*.*", SearchOption.TopDirectoryOnly);
            // foreach (var sdk in requireSDK)
            // {
            //     ++p;
            //     if (EditorUtility.DisplayCancelableProgressBar(string.Format("复制文件{0}({1}/{2})", sdk, p, requireSDK.Length), sdk, requireSDK.Length))
            //     {
            //         throw new Exception("用户停止");
            //     }
            //     CopyDirectory(sdk, SDK_ROOT);
            // }


            //FacebookSettings.AppIds[0] = "2354451384842070";
            //FacebookSettings.ClientTokens[0] = "f5e9f97c0526941c90995f7d33f3ebed";
            //EditorUtility.SetDirty(FacebookSettings.Instance);
            //ManifestMod.GenerateManifest();
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
        AssetDatabase.Refresh();
    }

    public static void SetChannel_Spain()
    {
        ClearSDKRoot();

        int p = 0;
        try
        {
            // var requireSDK = Directory.GetDirectories("Z_Work/SDK/Spain","*.*", SearchOption.TopDirectoryOnly);
            // foreach (var sdk in requireSDK)
            // {
            //     ++p;
            //     if (EditorUtility.DisplayCancelableProgressBar(string.Format("复制文件{0}({1}/{2})", sdk, p, requireSDK.Length), sdk, requireSDK.Length))
            //     {
            //         throw new Exception("用户停止");
            //     }
            //     CopyDirectory(sdk, SDK_ROOT);
            // }


            //FacebookSettings.AppIds[0] = "2478065609079956";
            //FacebookSettings.ClientTokens[0] = "";
            //EditorUtility.SetDirty(FacebookSettings.Instance);
            //ManifestMod.GenerateManifest();
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
        AssetDatabase.Refresh();
    }
}

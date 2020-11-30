#define USE_MINIGAME_SDK
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class SdkHooker
{
    private static string DebugKeyStorePath
    {
        get
        {
            return Application.dataPath + "/Editor/OnyxGames.keystore";
        }
    }


//    private static string TestDebugKeyStorePath
//    {
//        get
//        {
//#if false
//            if (Application.platform != RuntimePlatform.WindowsEditor)
//            {
//                Debug.LogError(Application.platform);
//                return Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/.android/debug.keystore";
//            }
//            return Environment.GetEnvironmentVariable("HOMEDRIVE") + Environment.GetEnvironmentVariable("HOMEPATH") + "\\.android\\debug.keystore";
//#else
//            string _DebugKeyStorePath = null;
//            if (_DebugKeyStorePath == null)
//            {
//                Type t = typeof(Facebook.Unity.Editor.FacebookAndroidUtil);
//                var p = t.GetProperty("DebugKeyStorePath", BindingFlags.Static | BindingFlags.NonPublic);
//                foreach(var i in p.GetAccessors()) { Debug.LogError(i); }
//                if(p != null)
//                {
//                    _DebugKeyStorePath = (string)p.GetValue(null, null);
//                }else
//                {
//                    Debug.LogError("DebugKeyStorePath not find!!!");
//                    _DebugKeyStorePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/.android/debug.keystore";
//                }
//            }
//            return _DebugKeyStorePath;
//#endif
//        }
//    }

//    [MenuItem("Facebook/DebugKeyStorePath")]
//    static void PrintDebugKeyStorePath()
//    {

//        Debug.LogError("debug.keystore:" + TestDebugKeyStorePath);
//    }


//    [DidReloadScripts]
//    static void InstallHook()
//    {
//        try
//        {

//            Debug.Log("Hook Install");
//            //Hook_Facebook();
//            Hook_Google_Resolver();

//        }
//        catch(Exception e)
//        {
//            Debug.LogError(e);
//        }
//    }

//    private static MethodHooker s_fb_hooker_postbuild;
//    static void Hook_Facebook()
//    {
//#if USE_MINIGAME_SDK
//        if (s_fb_hooker_postbuild != null)
//        {
//            return;
//        }
//        {
//            Type srcType = typeof(Facebook.Unity.Editor.XCodePostProcess);//Type.GetType("Facebook.Unity.Editor.XCodePostProcess，Facebook.Unity.Editor.dll");
//            if (srcType == null)
//            {
//                return;
//            }
//            MethodInfo srcFunc = srcType.GetMethod("OnPostProcessBuild", BindingFlags.Static | BindingFlags.Public);

//            if (srcFunc == null)
//            {
//                return;
//            }
//            var dstType = typeof(SdkHooker);
//            MethodInfo dstFunc = dstType.GetMethod("OnFacebookPostProcessBuild", BindingFlags.Static | BindingFlags.NonPublic);
//            MethodInfo miProxy = null;// type.GetMethod("ProxyClearLog", BindingFlags.Static | BindingFlags.NonPublic);

//            s_fb_hooker_postbuild = new MethodHooker(srcFunc, dstFunc, miProxy);
//            s_fb_hooker_postbuild.Install();
//        }

//        {
//            Type srcType = typeof(Facebook.Unity.Editor.FacebookAndroidUtil);
//            var f = srcType.GetField("debugKeyHash", BindingFlags.NonPublic | BindingFlags.Static);
//            f.SetValue(null, DebugKeyStorePath);
//            /*
//            var srcFunc = srcType.GetMethod("HasAndroidKeystoreFile", BindingFlags.Static | BindingFlags.Public);

//            var dstType = typeof(SdkHooker);
//            var dstFunc = dstType.GetMethod("HasAndroidKeystoreFile", BindingFlags.Static | BindingFlags.NonPublic);
//            MethodInfo miProxy = null;

//            var hooker = new MethodHooker(srcFunc, dstFunc, miProxy);
//            hooker.Install();
//            */

//        }

//        {
//            //n0HVZZ3RXdM4KzR+FILGAc0Ak8c=
//            Debug.LogError("fb hash:" + Facebook.Unity.Editor.FacebookAndroidUtil.DebugKeyHash);
//            //Debug.LogError("debug.keystore:" + TestDebugKeyStorePath);
//        }
//#endif
//    }

//    private static bool HasAndroidKeystoreFile()
//    {
//        return File.Exists(DebugKeyStorePath);
//    }

//    private static void OnFacebookPostProcessBuild(BuildTarget target, string path)
//    {
//        Debug.Log(target + ":" + path);
//    }

//    private static MethodHooker s_google_hooker_resolver;
//    static void Hook_Google_Resolver()
//    {
//#if USE_MINIGAME_SDK
//        if (s_google_hooker_resolver != null)
//        {
//            return;
//        }
//        Type type = typeof(GooglePlayServices.PlayServicesResolver);//Type.GetType("Facebook.Unity.Editor.XCodePostProcess，Facebook.Unity.Editor.dll");
                
//        if(type == null)
//        {
//            return;
//        }
//        MethodInfo miTarget = type.GetMethod("Resolve", BindingFlags.Static | BindingFlags.Public);
                
//        if(miTarget == null)
//        {
//            return;
//        }
//        type = typeof(SdkHooker);
//        MethodInfo miReplacement = type.GetMethod("Resolve", BindingFlags.Static | BindingFlags.NonPublic);
//        MethodInfo miProxy = null;// type.GetMethod("ProxyClearLog", BindingFlags.Static | BindingFlags.NonPublic);

//        s_google_hooker_resolver = new MethodHooker(miTarget, miReplacement, miProxy);
//        s_google_hooker_resolver.Install();
//#endif
//    }

//    private static void Resolve(Action resolutionComplete = null, bool forceResolution = false, Action<bool> resolutionCompleteWithResult = null)
//    {
//        if (resolutionComplete != null)
//        {
//            resolutionComplete();
//        }

//        if (resolutionCompleteWithResult != null)
//        {
//            resolutionCompleteWithResult(true);
//        }
//        Debug.LogError("fuck google");
//    }
}

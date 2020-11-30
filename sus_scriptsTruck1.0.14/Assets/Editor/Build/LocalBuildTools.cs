using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class LocalBuildTools
{
    const string OUT_PATH = "APP_OUT/";



    [InitializeOnLoadMethod]
    static void Init()
    {
        var symbolArr = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';');
        HashSet<string> symbols = new HashSet<string>(symbolArr);
        symbols.Remove("NOT_USE_LUA");
        symbols.Add("HOTFIX_ENABLE");

        if (PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup) 
            == UnityEditor.ApiCompatibilityLevel.NET_4_6)
        {
            symbols.Add("NET_4_6");
            PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Legacy;
            PlayerSettings.SetApiCompatibilityLevel(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                ApiCompatibilityLevel.NET_Standard_2_0
                );
        }
        else
        {
            //PlayerSettings.SetApiCompatibilityLevel(
            //    EditorUserBuildSettings.selectedBuildTargetGroup,
            //    ApiCompatibilityLevel.NET_4_6
            //    );
            symbols.Remove("NET_4_6");
        }
        bool hasChannel = false;
        foreach(var itr in symbols)
        {
            if (itr.StartsWith("CHANNEL_"))
            {
                hasChannel = true;
                break;
            }
        }
        var buildDefines = string.Join(";", symbols.ToArray());
        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, buildDefines);
        //Debug.Log("DefineSymbols:" + buildDefines);
        if (!hasChannel)
        {
            //SetChannel_Onyx();
        }
    }


    #region 调试

    const string MENU_DEBUG_MODE = "GameTools/打包/调试模式";
    const string MENU_PAY_TEST = "GameTools/打包/测试付费模式";

    [MenuItem(MENU_DEBUG_MODE, true)]
    static bool Check_DebugMode()
    {
#if ENABLE_DEBUG
        Menu.SetChecked(MENU_DEBUG_MODE, true);
#else
        Menu.SetChecked(MENU_DEBUG_MODE, false);
#endif
        return true;
    }

    [MenuItem(MENU_DEBUG_MODE, false, MenuPriority.BuildApp + 200)]
    static void SwitchMode()
    {
        var symbolArr = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';');
        HashSet<string> symbols = new HashSet<string>(symbolArr);
#if ENABLE_DEBUG
        symbols.Remove("ENABLE_DEBUG");

#else
        symbols.Add("ENABLE_DEBUG");
#endif
        var buildDefines = string.Join(";", symbols.ToArray());
        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, buildDefines);
        Debug.Log("DefineSymbols:" + buildDefines);
    }



    [MenuItem(MENU_PAY_TEST, true)]
    static bool Check_PayTest()
    {
#if PAY_TEST
        Menu.SetChecked(MENU_PAY_TEST, true);
#else
        Menu.SetChecked(MENU_PAY_TEST, false);
#endif
        return true;
    }


    [MenuItem(MENU_PAY_TEST, false, MenuPriority.BuildApp + 201)]
    static void SwitchPayMode()
    {

        var symbolArr = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';');
        HashSet<string> symbols = new HashSet<string>(symbolArr);
#if PAY_TEST
        symbols.Remove("PAY_TEST");

#else
        symbols.Add("PAY_TEST");
#endif
        var buildDefines = string.Join(";", symbols.ToArray());
        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, buildDefines);
        Debug.Log("DefineSymbols:" + buildDefines);
    }

    #endregion





    #region 渠道


    const string MENU_CHANNEL_ONYX = "GameTools/渠道/Onyx";
    const string MENU_CHANNEL_HUAWEI = "GameTools/渠道/华为";
    const string MENU_CHANNEL_SPAIN = "GameTools/渠道/西班牙";
    
    
    [MenuItem(MENU_CHANNEL_ONYX, true)]
    static bool ChnnelCheck_Onyx()
    {
#if CHANNEL_ONYX
        Menu.SetChecked(MENU_CHANNEL_ONYX, true);
#else
        Menu.SetChecked(MENU_CHANNEL_ONYX, false);
#endif
        return true;
    }


    [MenuItem(MENU_CHANNEL_HUAWEI, true)]
    static bool ChnnelCheck_HW()
    {
#if CHANNEL_HUAWEI
        Menu.SetChecked(MENU_CHANNEL_HUAWEI, true);
#else
        Menu.SetChecked(MENU_CHANNEL_HUAWEI, false);
#endif
        return true;
    }



    [MenuItem(MENU_CHANNEL_SPAIN, true)]
    static bool ChnnelCheck_Spain()
    {
#if CHANNEL_SPAIN
        Menu.SetChecked(MENU_CHANNEL_SPAIN, true);
#else
        Menu.SetChecked(MENU_CHANNEL_SPAIN, false);
#endif
        return true;
    }


    [MenuItem(MENU_CHANNEL_ONYX, false, MenuPriority.BuildApp + 100)]
    public static void SetChannel_Onyx()
    {
        ProjectTools.cmdLineArgs = new string[]
        {
            "-channel","Onyx",
        };
        ProjectTools.HandleChannel(EditorUserBuildSettings.activeBuildTarget);
    }


    [MenuItem(MENU_CHANNEL_HUAWEI, false, MenuPriority.BuildApp + 200)]
    public static void SetChannel_Huawei()
    {
        ProjectTools.cmdLineArgs = new string[]
        {
            "-channel","Huawei",
        };
        ProjectTools.HandleChannel(EditorUserBuildSettings.activeBuildTarget);
    }


    [MenuItem(MENU_CHANNEL_SPAIN, false, MenuPriority.BuildApp + 300)]
    public static void SetChannel_Spain()
    {
        ProjectTools.cmdLineArgs = new string[]
        {
            "-channel","Spain",
        };
        ProjectTools.HandleChannel(EditorUserBuildSettings.activeBuildTarget);
    }

    #endregion


        public static int BuildSeq
        {
            get
            {
                var seq = EditorPrefs.GetInt("BuildSeq",1);
                BuildSeq = seq + 1;
                return seq;
            }
            set
            {
                EditorPrefs.SetInt("BuildSeq",value);
            }
        }
        
    #region Android
#if UNITY_ANDROID

#if CHANNEL_ONYX
    [MenuItem("GameTools/打包/Android/Onyx正式包", false, MenuPriority.BuildApp + 100)]
    static void BuildOnyxApkFinal()
    {
        ProjectTools.auto_build = false;
        ProjectTools.cmdLineArgs = new string[]
        {
            "-version",GameUtility.version,
            "-buildNumber", GameUtility.buildNum+"",
            "-buildResource","false",
            "-outpath",string.Concat(OUT_PATH, BuildSeq, "-",  ProjectTools.GAME_NAME, GameUtility.version+"("+GameUtility.buildNum+")", "-OnyxRelease.apk"),
            "-enableDebug","false",
            "-il2cpp","true",
            "-paytest","false",
        };
        ProjectTools.BuildApk();
        if (ProjectTools.bBuildSuc)
        {
            EditorUtility.OpenWithDefaultApp(OUT_PATH);
        }
    }


    [MenuItem("GameTools/打包/Android/Onyx调试包", false, MenuPriority.BuildApp + 200)]
    static void BuildOnyxApkTest()
    {
        ProjectTools.auto_build = false;
        ProjectTools.cmdLineArgs = new string[]
        {
            "-version",GameUtility.version,
            "-buildNumber",GameUtility.buildNum+"",
            "-buildResource","false",
            "-outpath",string.Concat(OUT_PATH, BuildSeq, "-",  ProjectTools.GAME_NAME, GameUtility.version+"("+GameUtility.buildNum+")","-OnyxDebug.apk"),
            "-enableDebug","true",
            "-il2cpp","false",
        };
        ProjectTools.BuildApk();
        if (ProjectTools.bBuildSuc)
        {
            EditorUtility.OpenWithDefaultApp(OUT_PATH);
        }
    }
#endif

#if CHANNEL_HUAWEI
    [MenuItem("GameTools/打包/Android/Huawei调试包", false, MenuPriority.BuildApp + 200)]
    static void BuildHwApkTest()
    {
        ProjectTools.auto_build = false;
        ProjectTools.cmdLineArgs = new string[]
        {
            "-version",GameUtility.version,
            "-buildNumber",GameUtility.buildNum+"",
            "-buildResource","false",
            "-outpath",string.Concat(OUT_PATH,BuildSeq, "-",  ProjectTools.GAME_NAME,GameUtility.version+"("+GameUtility.buildNum+")","-HwDebug.apk"),
            "-enableDebug","true",
            "-il2cpp","false",
        };
        ProjectTools.BuildApk();
        if (ProjectTools.bBuildSuc)
        {
            EditorUtility.OpenWithDefaultApp(OUT_PATH);
        }
    }
    [MenuItem("GameTools/打包/Android/Huawei正式包", false, MenuPriority.BuildApp + 100)]
    static void BuildHwApkFinal()
    {
        ProjectTools.auto_build = false;
        ProjectTools.cmdLineArgs = new string[]
        {
            "-version",GameUtility.version,
            "-buildNumber",GameUtility.buildNum+"",
            "-buildResource","false",
            "-outpath",string.Concat(OUT_PATH,BuildSeq, "-",  ProjectTools.GAME_NAME,GameUtility.version+"("+GameUtility.buildNum+")","-HwRelease.apk"),
            "-enableDebug","false",
            "-il2cpp","true",
            "-paytest","false",
        };
        ProjectTools.BuildApk();
        if (ProjectTools.bBuildSuc)
        {
            EditorUtility.OpenWithDefaultApp(OUT_PATH);
        }
    }

#endif

#endif
    #endregion



    #region IOS
#if UNITY_IOS
    [MenuItem("GameTools/打包/IOS/正式包", false, MenuPriority.BuildApp + 100)]
    static void BuildIosFinal()
    {
        ProjectTools.auto_build = false;
        ProjectTools.cmdLineArgs = new string[]
        {
            "-version",GameUtility.version,
            "-buildNumber",GameUtility.buildNum+"",
            "-buildResource","false",
            "-outpath",string.Concat(OUT_PATH, BuildSeq, "-",  ProjectTools.GAME_NAME,GameUtility.version+"("+GameUtility.buildNum+")","-release"),
            "-enableDebug","false",
            "-paytest","false",
        };
        ProjectTools.BuildIPA();
        if (ProjectTools.bBuildSuc)
        {
            EditorUtility.OpenWithDefaultApp(OUT_PATH);
        }
    }

    [MenuItem("GameTools/打包/IOS/调试包", false, MenuPriority.BuildApp + 200)]
    static void BuildIosTest()
    {
        ProjectTools.auto_build = false;
        ProjectTools.cmdLineArgs = new string[]
        {
            "-version",GameUtility.version,
            "-buildNumber",GameUtility.buildNum+"",
            "-buildResource","false",
            "-outpath",string.Concat(OUT_PATH,BuildSeq, "-",  ProjectTools.GAME_NAME,GameUtility.version+"("+GameUtility.buildNum+")","-debug"),
            "-enableDebug","true",
        };
        ProjectTools.BuildIPA();
        if (ProjectTools.bBuildSuc)
        {
            EditorUtility.OpenWithDefaultApp(OUT_PATH);
        }
    }
#endif
    #endregion
}

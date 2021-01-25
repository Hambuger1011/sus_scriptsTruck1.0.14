/*
Android、IOS自动构建脚本
*/
using Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

using UnityEditor.Callbacks;
using System.Linq;
using Object = UnityEngine.Object;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using AB;
using System.Reflection;

public class ProjectTools
{

    public static bool auto_build = true;
#if CHANNEL_SPAIN
    public static string GAME_NAME = "My High School Sumer Party";
#else
    public static string GAME_NAME = "Scripts";
#endif
    //
    static string m_androidAppClassName = "com.game.gamelib.DefaulApplication";
    static string m_androidLaunchMode = "singleTask";
    public static bool isDebug = false;


    #region 自动化
    public static void OnJinkens_HandleChannel()
    {
        cmdLineArgs = System.Environment.GetCommandLineArgs();//获取命令行参数
        var os = GetCmdArg("-OS");
        if (os == "Android")
        {
            HandleChannel(BuildTarget.Android);
        }
        else if (os == "iOS")
        {
            HandleChannel(BuildTarget.iOS);
        }
        else
        {
            Debug.LogError("未知平台:" + os);
        }
    }
    /// <summary>
    /// 该函数为jinkens调用
    /// </summary>
    public static void OnJinkens_AutoBuild()
    {
        Debug.LogError("#############Jinkens AutoBuild Start#############");
        //Debug.LogError(System.Environment.CommandLine);
        Debug.LogError("***PATH:" + Environment.GetEnvironmentVariable("PATH"));
        Debug.LogError("***JAVA_HOME:" + Environment.GetEnvironmentVariable("JAVA_HOME"));
        ProjectTools.auto_build = true;
        cmdLineArgs = System.Environment.GetCommandLineArgs();//获取命令行参数

        //for(int i=0;i<cmdLineArgs.Length;++i)
        //{
        //    Debug.LogError(i + "." + cmdLineArgs[i]);
        //}

        //EditorUserBuildSettings.activeBuildTarget
        //var relativePath = Path.GetFullPath(System.Environment.CurrentDirectory);
        //Debug.LogError(System.Environment.CurrentDirectory);
        //Debug.LogError(relativePath);

        var os = GetCmdArg("-OS");
        if (os == "Android")
        {
            BuildApk();
        }
        else if (os == "iOS")
        {
            BuildIPA();
        }
        else
        {
            Debug.LogError("未知平台:" + os);
        }
        Debug.LogError("#############Jinkens AutoBuild End#############");
    }

    public static void OnJinkens_UploadResource()
    {
        cmdLineArgs = System.Environment.GetCommandLineArgs();//获取命令行参数
        var isUpdateSVN = GetCmdArg("-isUpdateSVN");
        var upload2Svr = GetCmdArg("-upload2Svr");
        Debug.LogError("isUpdateSVN:" + isUpdateSVN);
        Debug.LogError("upload2Svr:" + upload2Svr);
        if(isUpdateSVN == "true")
        {
            MyEditor.UnitySVN.SVN_Update();//更新svn
            //ABEditor.OnKeyGen(); //一键生成ab
        }else
        {
            //ABEditor.Build(); //一键生成ab
        }

    }
    #endregion

    /// <summary>
    /// 打IOS包
    /// </summary>

    public static void BuildIPA()
    {
        PlayerSettings.iOS.targetOSVersionString = "8.0";
        //PlayerSettings.iOS.appleDeveloperTeamID = "T9DUCZ5UX6";
        PlayerSettings.stripEngineCode = false;
        BuildApp(BuildTarget.iOS);
    }

    /// <summary>
    /// 打Android包
    /// </summary>
    public static void BuildApk()
    {
        Time.timeScale = 1;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        //if(EditorUserBuildSettings.androidBuildSubtarget != MobileTextureSubtarget.ETC2)
        //{
        //    EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ETC2;
        //}

        PlayerSettings.Android.androidIsGame = true;
        PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;

        PlayerSettings.Android.useAPKExpansionFiles = false;//是否使用obb数据
#if CHANNEL_HUAWEI
        PlayerSettings.Android.keystoreName = Application.dataPath + "/Editor/secretsHuawei.keystore";
        PlayerSettings.Android.keyaliasName = "secretsHuawei";
#elif CHANNEL_SPAIN
        PlayerSettings.Android.keystoreName = Application.dataPath + "/Editor/CH_SPAIN.keystore";
        PlayerSettings.Android.keyaliasName = "CH_SPAIN";
#else
        // PlayerSettings.Android.keystoreName = Application.dataPath + "/Editor/OnyxGames.keystore";
        // PlayerSettings.Android.keyaliasName = "onyx";
        
        PlayerSettings.Android.keystoreName = Application.dataPath + "/Editor/OnyxGames.keystore";
        PlayerSettings.Android.keyaliasName = "onyx";
#endif

        PlayerSettings.Android.keystorePass = "onyxgame";
        PlayerSettings.Android.keyaliasPass = "onyxgame";
        BuildApp(BuildTarget.Android);
    }

    static void BuildApp(BuildTarget buildTarget)
    {

        BuildTargetGroup buildTargetGroup = GetBuildTargetGroup(buildTarget);
        Debug.Log("参数:count = " + cmdLineArgs.Length);
        PlayerSettings.stripEngineCode = false;

        #region 版本号
        string buildVersion = GetCmdArg("-version");
        string buildNumber = GetCmdArg("-buildNumber");
        if (string.IsNullOrEmpty(buildNumber))
        {
            buildNumber = "1";
        }

        Debug.Log("version=" + buildVersion + " buildNumber=" + buildNumber);
        PlayerSettings.bundleVersion = buildVersion;
        switch (buildTarget)
        {
            case BuildTarget.Android:
                PlayerSettings.Android.bundleVersionCode = Convert.ToInt32(buildNumber);
                break;
            case BuildTarget.iOS:
                PlayerSettings.iOS.buildNumber = buildNumber;
                break;
        }
        #endregion


        #region app名
        var appName = GetCmdArg("-app_name"); //GAME_NAME;//游戏名
        if (string.IsNullOrEmpty(appName))
        {
            PlayerSettings.productName = GAME_NAME;
        }
        else
        {
            PlayerSettings.productName = appName;
        }
        #endregion

        #region 是否打资源
        string buildResource = GetCmdArg("-buildResource");
        if (buildResource.Equals("true"))
        {
            //ABEditor.OnKeyGen();
        }
        File.Copy("Assets/Bundle/Data/Common/t_Localization.bytes", "Assets/Resources/t_Localization.bytes", true);
        AssetDatabase.Refresh();
        #endregion

        #region 输出路径
        string outpath = GetCmdArg("-outpath");
        if (string.IsNullOrEmpty(outpath))
        {
            outpath = new DirectoryInfo("apk").FullName;
            if (!Directory.Exists(outpath))
            {
                Directory.CreateDirectory(outpath);
            }
            outpath = string.Format("{0}/{1}.apk", outpath, "S03");
        }
        Debug.Log("app outpath:" + outpath);
        #endregion

        #region 编译宏
        string enable_debug = GetCmdArg("-enableDebug");
        string pay_test = GetCmdArg("-paytest");
        string channel = GetCmdArg("-channel");
        string il2cppFlag = GetCmdArg("-il2cpp");
        string sdkPlatForm = GetCmdArg("-platformSDK");
        //TODO:添加宏
        HashSet<string> symbols = new HashSet<string>();
        var symbolArr = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup).Split(';');
        foreach (var symbol in symbolArr)
        {
            symbols.Add(symbol);
        }

        if (enable_debug.Equals("true"))
        {
            symbols.Add("ENABLE_DEBUG");
        }
        else
        {
            symbols.Remove("ENABLE_DEBUG");
        }

        if (!string.IsNullOrEmpty(pay_test))
        {
            if (pay_test.Equals("true"))
            {
                symbols.Add("PAY_TEST");
            }
            else
            {
                symbols.Remove("PAY_TEST");
            }
        }

        var buildDefines = string.Join(";", symbols.ToArray());
        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, buildDefines);
        Debug.Log("DefineSymbols:" + buildDefines);
        #endregion


        PlayerSettings.MTRendering = true;
        PlayerSettings.applicationIdentifier = SdkMgr.packageName;
#if UNITY_2017_1_OR_NEWER
        PlayerSettings.SetMobileMTRendering(buildTargetGroup, true);
#else
        PlayerSettings.mobileMTRendering = true;
#endif

        PrepareBuild();

        var scenes = GetBuildScenes();
        Debug.Log("scenes count:" + scenes.Length);

        BuildOptions buildOptions = /*BuildOptions.AcceptExternalModificationsToPlayer |*/ BuildOptions.SymlinkLibraries | BuildOptions.StrictMode;
        if (isDebug)
        {
            //buildOptions |= BuildOptions.Development | BuildOptions.AllowDebugging;
        }

        switch (buildTarget)
        {
            case BuildTarget.iOS:
                {
                    IOSPostProcessBuild.s_isEnableDebug = isDebug;
                    //buildOptions |= BuildOptions.Il2CPP;
                    PlayerSettings.stripEngineCode = false;
                    PlayerSettings.SetIncrementalIl2CppBuild(buildTargetGroup, true);
                    PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.IL2CPP);
                }
                break;
            default:
                {
                    // if (il2cppFlag.Equals("true"))
                    // {
                        PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.IL2CPP);
                        PlayerSettings.SetIncrementalIl2CppBuild(buildTargetGroup, true);
#if UNITY_2018_1_OR_NEWER
                        PlayerSettings.SetIl2CppCompilerConfiguration(buildTargetGroup, Il2CppCompilerConfiguration.Release);
                        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
#else
                        PlayerSettings.Android.targetDevice = AndroidTargetDevice.ARMv7;
#endif
                    // }
//                     else
//                     {
//                         PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.Mono2x);
// #if UNITY_2018_1_OR_NEWER
//                         PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
// #else
//                     PlayerSettings.Android.targetDevice = AndroidTargetDevice.ARMv7;
// #endif
//                     }
//                     PlayerSettings.strippingLevel = StrippingLevel.Disabled;

                }
                break;
        }

        //         BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        //         buildPlayerOptions.assetBundleManifestPath = AbUtility.AbBuildPath;
        //         buildPlayerOptions.locationPathName = outpath;
        //         buildPlayerOptions.options = buildOptions;
        //         buildPlayerOptions.scenes = scenes;
        //         buildPlayerOptions.target = buildTarget;


        Debug.Log("***************开始Build Player***************");
#if UNITY_2018_1_OR_NEWER
        var res = BuildPipeline.BuildPlayer(scenes, outpath, buildTarget, buildOptions);
        if (res.summary.totalErrors == 0)
        {
            Debug.Log("构建app成功");
            bBuildSuc = true;
        }
        else
        {

            Debug.LogError("!!!!!!!构建app失败:" + res.summary.result);
            bBuildSuc = false;
        }
#else
        var res = BuildPipeline.BuildPlayer(scenes, outpath, buildTarget, buildOptions);
        if (string.IsNullOrEmpty(res))
        {
            Debug.Log("构建app成功");
			bBuildSuc = true;
        }
        else
        {

			Debug.LogError("!!!!!!!构建app失败:" + res);
			bBuildSuc = false;
        }
#endif
        //恢复宏
        //PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, oldBuildDefines);
        //PlayerSettings.SetPropertyInt("ScriptingBackend", (int)ScriptingImplementation.Mono2x, BuildTarget.Android);
    }

	public static bool bBuildSuc = false;




    public static string[] cmdLineArgs;
    /// <summary>
    /// 获取命令行参数
    /// </summary>

    private static string GetCmdArg(string name)
    {
        string[] args = cmdLineArgs;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].ToLower() == name.ToLower())
            {
                if (args.Length > i + 1)
                {
                    return args[i + 1];
                }
            }
        }

        return "";
    }

    private static bool HasCommandLineArg(string name)
    {
        string[] args = cmdLineArgs;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].ToLower() == name.ToLower())
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 获取场景
    /// </summary>
    /// <returns></returns>
    static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();

        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null)
                continue;
            if (e.enabled)
                names.Add(e.path);
        }
        return names.ToArray();
    }

    static void PrepareBuild()
    {


    }

    public static BuildTargetGroup GetBuildTargetGroup(BuildTarget buildTarget)
    {
        BuildTargetGroup buildTargetGroup = BuildTargetGroup.Unknown;
        switch(buildTarget)
        {
            case BuildTarget.Android:
                buildTargetGroup = BuildTargetGroup.Android;
                break;
            case BuildTarget.iOS:
                buildTargetGroup = BuildTargetGroup.iOS;
                break;
        }
        return buildTargetGroup;
    }

    public static void HandleChannel(BuildTarget buildTarget)
    {
        BuildTargetGroup buildTargetGroup = GetBuildTargetGroup(buildTarget);
        string enable_debug = GetCmdArg("-enableDebug");
        string pay_test = GetCmdArg("-paytest");
        string channel = GetCmdArg("-channel");
        string il2cppFlag = GetCmdArg("-il2cpp");
        string sdkPlatForm = GetCmdArg("-platformSDK");

        //TODO:添加宏
        HashSet<string> symbols = new HashSet<string>();
        var symbolArr = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup).Split(';');
        foreach(var symbol in symbolArr)
        {
            if(symbol.StartsWith("CHANNEL_"))
            {
                continue;
            }
            symbols.Add(symbol);
        }

        switch(channel)
        {
            case "Huawei":
                symbols.Add("CHANNEL_HUAWEI");
                SDKTools.SetChannel_Huawei();
                break;
            case "Onyx":
                symbols.Add("CHANNEL_ONYX");
                SDKTools.SetChannel_Onyx();
                break;
            case "Spain":
                symbols.Add("CHANNEL_SPAIN");
                SDKTools.SetChannel_Spain();
                break;
        }
        var buildDefines = string.Join(";", symbols.ToArray());
        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, buildDefines);
        Debug.Log("DefineSymbols:" + buildDefines);
    }
}

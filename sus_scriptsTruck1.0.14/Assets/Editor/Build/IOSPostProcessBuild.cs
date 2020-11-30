using System;
using Framework;

using UnityEngine;
using System.Collections;
using UnityEditor.Callbacks;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEditor.iOS.Xcode.Custom;
using UnityEditor.iOS.Xcode.Custom.Extensions;


public static class IOSPostProcessBuild {

    public static bool s_isEnableDebug = true;
    public static string strMacro = string.Empty;

    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string projRootPath)
    {
        if (buildTarget != BuildTarget.iOS)
        {
            return;
        }

        string projPath = PBXProject.GetPBXProjectPath(projRootPath);
        PBXProject proj = new PBXProject();

        proj.ReadFromString(File.ReadAllText(projPath));
        string target = proj.TargetGuidByName(PBXProject.GetUnityTargetName());


        #region 加载需要的系统Framework
        proj.AddFrameworkToProject(target, "AdSupport.framework", true);//获取idfa用到
        proj.AddFrameworkToProject(target, "CoreMotion.framework", false);//传感器(加速计、陀螺仪、摇一摇等等)
        proj.AddFrameworkToProject(target, "Security.framework", false);//辅助存储设备标识
        proj.AddFrameworkToProject(target, "StoreKit.framework", false);//app支付
        proj.AddFrameworkToProject(target, "CoreTelephony.framework", false);//获取运营商标识
        proj.AddFrameworkToProject(target, "CoreGraphics.framework", false);//绘图框架
        proj.AddFrameworkToProject(target, "UIKit.framework", false);//用户界面
        proj.AddFrameworkToProject(target, "UserNotifications.framework", false);//用户界面

        proj.AddFrameworkToProject(target, "libz.tbd", false);//数据压缩
        proj.AddFrameworkToProject(target, "libresolv.tbd", false);//数据压缩
        proj.AddFrameworkToProject(target, "libc++.dylib", false);//支持最新的c++11标准

        proj.AddFrameworkToProject(target, "CFNetwork.framework", false);//网络
        proj.AddFrameworkToProject(target, "CoreFoundation.framework", false);//C语言接口
        proj.AddFrameworkToProject(target, "Foundation.framework", false);//C语言接口
        proj.AddFrameworkToProject(target, "SystemConfiguration.framework", false);//包含用于处理设备网络配置的接口,检测网络状况
        proj.AddFrameworkToProject(target, "AssetsLibrary.framework", false);//保存图片到手机
        #endregion

        #region 修改工程配置文件project.pbxproj
        string other_ldflags = "-weak_framework CoreMotion -weak-lSystem -lz -ObjC";//-all_load和-ObjC 只能加其中一个？

        //其他link参数
        //proj.SetBuildProperty(target, "OTHER_LDFLAGS", other_ldflags);

        //proj.SetBuildProperty(target, "DEVELOPMENT_TEAM", "UC48NCY6V6");
        //proj.SetBuildProperty(target, "PRODUCT_BUNDLE_IDENTIFIER", "S03Tianmingzhanyu");
        //proj.SetBuildProperty()

        //Frameworks搜索路径
        string[] dirs = new string[]
        {
            // "Plugins/iOS",
            // "Plugins/BuglySDK/iOS",
            //"Plugins/SDK_Google/FacebookSDK/Plugins/iOS"
        };
        //proj.SetBuildProperty(target, "FRAMEWORK_SEARCH_PATHS", "$(inherited)");
        proj.AddBuildProperty(target, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Frameworks");
        proj.AddBuildProperty(target, "HEADER_SEARCH_PATHS", "$(SRCROOT)/Libraries/");//头文件搜索路径
        foreach (var d in dirs)
        {
            proj.AddBuildProperty(target, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Frameworks/"+d);//包搜索路径
            proj.AddBuildProperty(target, "HEADER_SEARCH_PATHS", "$(SRCROOT)/Libraries/"+d);//头文件搜索路径
        }

        //禁用bitcode
        proj.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

        if (!s_isEnableDebug)
        {
            proj.AddCapability(target, PBXCapabilityType.InAppPurchase, null, true);
        }
        proj.AddCapability(target, PBXCapabilityType.PushNotifications, null, true);
        //
        File.WriteAllText(projPath, proj.WriteToString());
        #endregion



        #region 修改plist 
        // 修改plist  
        string plistPath = projRootPath + "/Info.plist";
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));
        PlistElementDict rootDict = plist.root;
        rootDict.values.Remove("UIApplicationExitsOnSuspend");


        rootDict.values.Remove("UIApplicationExitsOnSuspend");

        // 用到的权限所需要的声明，如果不声明，一但调用程序就会崩溃
        rootDict.SetString("NSLocationAlwaysUsageDescription", "是否允许此游戏使用地理位置？");
        rootDict.SetString("NSContactsUsageDescription", "是否允许此游戏使用通讯录？");
        rootDict.SetString("NSCameraUsageDescription", "是否允许此游戏使用相机？");
        rootDict.SetString("NSPhotoLibraryUsageDescription", "是否允许此游戏使用相册？");
        rootDict.SetString("NSMicrophoneUsageDescription", "是否允许此游戏使用语言？");
        rootDict.SetString("NSLocationWhenInUseUsageDescription", "是否允许此游戏运行时使用地理位置？");
        rootDict.SetString("NSPhotoLibraryAddUsageDescription", "请允许此游戏保存图片到相册");


        //Splus_GameAnalysis_SDK - 因XCODE7默认使用了HTTPS上报协议，而Splus的这个版本暂时没有采用该上报协议，因此需要开发者在Info.plist文件中配置以下参数
        //PlistElementDict dict = rootDict.CreateDict("App Transport Security Settings");
        //dict.SetBoolean("Allow Arbitrary Loads", true);


        //后台运行模式
        var arr = rootDict.CreateArray("Required background modes");
        arr.AddString("App downloads content from the network");
        arr.AddString("App downloads content in response to push notifications");

        if (s_isEnableDebug)
        {
            rootDict.SetBoolean("UIFileSharingEnabled", true);//显示Document
        }
        if (rootDict.values.ContainsKey("NSAppTransportSecurity"))
        {
	        try
	        {
		        var dict = rootDict["NSAppTransportSecurity"].AsDict();
		        var dict2 = dict.CreateDict("NSExceptionDomains");
		        var dict3 = dict2.CreateDict("igg.com");
		        dict3.CreateDict("NSExceptionAllowsInsecureHTTPLoads");
		        dict3.CreateDict("NSIncludesSubdomains");
		        dict3.SetBoolean("NSExceptionAllowsInsecureHTTPLoads",true);
		        dict3.SetBoolean("NSIncludesSubdomains",true);
		        if (dict.values.ContainsKey("NSAllowsArbitraryLoads"))
			        dict.SetBoolean("NSAllowsArbitraryLoads", true);
		        if (dict.values.ContainsKey("NSAllowsArbitraryLoadsInWebContent"))
			        dict.SetBoolean("NSAllowsArbitraryLoads", true);
	        }
	        catch (InvalidCastException arg)
	        {
		        Debug.LogError("自动配置IGGHTTP权限失败");
		        Debug.LogWarning((object)string.Format("Unable to parse NSAppTransportSecurity as a dictionary. ({0})\n{1}\nTo fix this issue make sure NSAppTransportSecurity is a dictionary in your Info.plist", arg, "Unable to apply NSURLSession workaround. If NSAllowsArbitraryLoads is set to a different value than NSAllowsArbitraryLoadsInWebContent in your Info.plist network operations will randomly fail on some versions of iOS"));
		        return;
	        }
        }
        
        File.WriteAllText(plistPath, plist.WriteToString());
        #endregion

        #region 修改代码
        //添加头文件声明
        //XcodeProjectMod.AddIncludes(projRootPath + "/Classes/UnityAppController.mm",
        //    "#import \"IOSPlugin.h\""
        ////"#import <WLSDK/WLSDKManager.h>"
        //);

        List<XcodeProjectMod.ReplaceFileProp> unityAppCtrlModifyList = new List<XcodeProjectMod.ReplaceFileProp>();

        if (s_isEnableDebug)
        {
            //XcodeProjectMod.ModifyXcodeSource_recvMemWarning(unityAppCtrlModifyList);
        }

        XcodeProjectMod.ModifyXcodeSource_iPhoneX(unityAppCtrlModifyList);
        XcodeProjectMod.ReplaceFileContent(projRootPath + "/Classes/UnityAppController.mm", unityAppCtrlModifyList);
        #endregion
    }
}





#region 代码修改
public static class XcodeProjectMod
{
    //[MenuItem("Test/XcodeProjectMod")]
    //static void Test()
    //{
    //    string file = "G://UnityAppController.mm";
    //    List<XcodeProjectMod.ReplaceFileProp> unityAppCtrlModifyList = new List<XcodeProjectMod.ReplaceFileProp>();
    //    XcodeProjectMod.ModifyXcodeSource_iPhoneX(unityAppCtrlModifyList);
    //    XcodeProjectMod.ReplaceFileContent(file, unityAppCtrlModifyList);
    //}

    public class ReplaceFileProp
    {
        public string matchLine;
        public string replaceContent;
        public ReplaceFileProp()
        {

        }
        public ReplaceFileProp(string str1,string str2)
        {
            matchLine = str1;
            replaceContent = str2;
        }
    }
    

#region 接收低内存警告
    public static void ModifyXcodeSource_recvMemWarning(List<ReplaceFileProp> l)
    {
        ReplaceFileProp p = new ReplaceFileProp();
        p.matchLine = "\t::printf(\"WARNING -> applicationDidReceiveMemoryWarning()\\n\");"; //::printf("WARNING -> applicationDidReceiveMemoryWarning()\n");
        p.replaceContent = "\t::printf(\"WARNING -> applicationDidReceiveMemoryWarning()\\n\");\n\tShowMemoryWarning(self);\n";
        //p.replaceContent = "\t::printf(\"WARNING -> applicationDidReceiveMemoryWarning()\\n\");\n\tShowMemoryWarning(application);\n";
        l.Add(p);
    }
    #endregion

#region iPhoneX适配
    public static void ModifyXcodeSource_iPhoneX(List<ReplaceFileProp> l)
    {
        ReplaceFileProp p = new ReplaceFileProp();
        p.matchLine = "_window			= [[UIWindow alloc] initWithFrame:[UIScreen mainScreen].bounds];";
        p.replaceContent =
@"
    //_window			= [[UIWindow alloc] initWithFrame:[UIScreen mainScreen].bounds];
    CGRect winSize = [UIScreen mainScreen].bounds;
    if(winSize.size.width/winSize.size.height > 2)
    {
        winSize.size.width-=148;
        winSize.origin.x = 74;
    }
    _window= [[UIWindow alloc] initWithFrame:winSize];
";
        l.Add(p);
    }
#endregion
    /// <summary>
    /// 添加头文件
    /// </summary>
    public static void AddIncludes(string filePath, params string[] strIncludes)
    {
		string strContent = "";
		foreach (var strC in strIncludes) 
		{
			strContent += strC + System.Environment.NewLine;
		}

		string[] lines = File.ReadAllLines(filePath);
		lines [0] = lines [0] + System.Environment.NewLine + strContent;

		File.WriteAllLines(filePath, lines);

    }

	/// <summary>
	/// 追加内容
	/// </summary>
	public static void AppendContent(string filePath, params string[] strContentArr)
	{
		string strContent = "";
		foreach (var strC in strContentArr) 
		{
			strContent += strC + System.Environment.NewLine;
		}

		string[] lines = File.ReadAllLines(filePath);

		for (int iLine = lines.Length - 1; iLine >= 0;--iLine)
		{
			string line = lines[iLine];

			if(line.Trim() == "@end")
			{
				line = strContent + line;
				lines[iLine] = line;
				break;
			}
		}

		File.WriteAllLines(filePath, lines);
	}

    public static void ReplaceFileContent(string filePath, List<ReplaceFileProp> props)
    {
        string[] lines = File.ReadAllLines(filePath);

        for (int iLine = 0; iLine < lines.Length; iLine++)
        {
            string line = lines[iLine].Trim();

            for (int iProp = 0; iProp < props.Count; iProp++)
            {
                string matchLine = props[iProp].matchLine.Trim();
                if (line == matchLine)
                {
                    lines[iLine] = props[iProp].replaceContent;
                }
            }
        }

        File.WriteAllLines(filePath, lines);
    }

	/// <summary>
	/// 追加内容
	/// </summary>
	public static void InitAppDelegate(string filePath)
	{
		bool flag = false;
		string[] lines = File.ReadAllLines(filePath);
		for(int i=0;i<lines.Length;++i)
		{
			var line = lines[i];
			if(line.Trim() == "- (id)init")
			{
				flag = true;
			}
			if(flag && line.Trim() == "return self;")
			{
				line = "    g_appDelegate = self;" + System.Environment.NewLine + line;
				lines [i] = line;
				break;
			}
		}
		File.WriteAllLines(filePath, lines);
	}

	public static void  AddProtocol(string filePath, params string[] strContentArr)
	{		
		string strContent = "";
		foreach (var strC in strContentArr) 
		{
			strContent += strC + System.Environment.NewLine;
		}

		string[] lines = File.ReadAllLines(filePath);
		for(int i=0;i<lines.Length;++i)
		{
			var line = lines[i];
			if(line.Trim() == "@interface UnityAppController : NSObject<UIApplicationDelegate>")
			{
				line = "@interface UnityAppController : NSObject<UIApplicationDelegate,"+strContent+">";
				lines [i] = line;
				break;
			}
		}
		File.WriteAllLines(filePath, lines);
	}



    public static void AddCode2Method_InEnd(string filename, string methodName,string code)
    {
        bool isFind = false;
        int count = 0;
        string[] lines = File.ReadAllLines(filename);
        for (int i = 0; i < lines.Length; ++i)
        {
            var line = lines[i].Trim();
            if(isFind)
            {
                if(line == "{")
                {
                    ++count;
                }
                else if(line == "}")
                {
                    --count;
                    if(count == 0)
                    {
                        lines[i] = code + System.Environment.NewLine + lines[i];
                        LOG.Info("find:" + methodName);
                        break;
                    }
                }else if(line.StartsWith("return "))
                {
                    lines[i] = code + System.Environment.NewLine + lines[i];
                    LOG.Info("find:" + methodName);
                    break;
                }

            }else if(line.Equals(methodName))
            {
                isFind = true;
                continue;
            }
        }
        File.WriteAllLines(filename, lines);
    }
}
#endregion

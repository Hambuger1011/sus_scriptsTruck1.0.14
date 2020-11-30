using UnityEngine;
using System.Collections;
using UnityEditor.Callbacks;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEditor.iOS.Xcode.Custom;
using UnityEditor.iOS.Xcode.Custom.Extensions;

public static class Xcode_JPush
{
    
//#if CHANNEL_SPAIN
//    public static string app_key = "045df6e33f46acfcd06a79d5";
//#else
//    public static string app_key = "dcfadfc03725d0bc25c18096";
//#endif

//    [PostProcessBuild(10000 + 1)]
//    public static void OnPostprocessBuild(BuildTarget buildTarget, string projRootPath)
//    {
//        if (buildTarget != BuildTarget.iOS)
//        {
//            return;
//        }

//        string projPath = PBXProject.GetPBXProjectPath(projRootPath);
//        ChangePbProj(projPath);
//        ChangeXcodePlist(projRootPath);
//        ChangeSource(projRootPath);




//    }


//    static void ChangePbProj(string projPath)
//    {
//        PBXProject proj = new PBXProject();
//        proj.ReadFromString(File.ReadAllText(projPath));
//        string targetGuid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());


//        #region 加载需要的系统Framework
//        /*
//        proj.AddFrameworkToProject(targetGuid, "CoreFoundation.framework", false);
//        proj.AddFrameworkToProject(targetGuid, "CFNetwork.framework", false);
//        proj.AddFrameworkToProject(targetGuid, "SystemConfiguration.framework", false);
//        proj.AddFrameworkToProject(targetGuid, "CoreTelephony.framework", false);
//        proj.AddFrameworkToProject(targetGuid, "CoreGraphics.framework", false);
//        proj.AddFrameworkToProject(targetGuid, "Foundation.framework", false);
//        proj.AddFrameworkToProject(targetGuid, "UIKit.framework", false);
//        proj.AddFrameworkToProject(targetGuid, "Security.framework", false);
//        //proj.AddFrameworkToProject(targetGuid, "AdSupport.framework", true);
//        proj.AddFrameworkToProject(targetGuid, "libresolv.framework", false);
//        proj.AddFrameworkToProject(targetGuid, "UserNotifications.framework", false);
//        proj.AddFrameworkToProject(targetGuid, "libz.tbd", true);
//        //proj.AddFrameworkToProject(targetGuid, "libz.dylib", true);
//        */
//        #endregion


//        #region 修改工程配置文件project.pbxproj

//        //Frameworks搜索路径
//        string[] dirs = new string[]
//        {
//            "Plugins/SDK/Static/JPush",//遍历/**
//        };
//        //proj.SetBuildProperty(target, "FRAMEWORK_SEARCH_PATHS", "$(inherited)");
//        //proj.AddBuildProperty(target, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Frameworks");
//        //proj.AddBuildProperty(target, "HEADER_SEARCH_PATHS", "$(SRCROOT)/Libraries/");//头文件搜索路径
//        foreach (var d in dirs)
//        {
//            proj.AddBuildProperty(targetGuid, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Frameworks/" + d);//包搜索路径
//            proj.AddBuildProperty(targetGuid, "HEADER_SEARCH_PATHS", "$(SRCROOT)/Libraries/" + d);//头文件搜索路径
//        }
//        proj.AddBuildProperty(targetGuid, "LIBRARY_SEARCH_PATHS", "$(inherited)");
//        proj.AddBuildProperty(targetGuid, "USER_HEADER_SEARCH_PATHS","$(inherited)");
//        #endregion

//        File.WriteAllText(projPath, proj.WriteToString());
//    }

//    static void ChangeXcodePlist(string projRootPath)
//    {

//    }

//    static void ChangeSource(string projRootPath)
//    {
//        //添加头文件声明
//        XcodeProjectMod.AddIncludes(projRootPath + "/Classes/UnityAppController.mm",
//            "#import \"JPUSHService.h\"",
//            "#import \"JPushEventCache.h\"",
//            "#import <UserNotifications/UserNotifications.h>"
//        );


//        #region 初始化
//        XcodeProjectMod.AddCode2Method_InEnd(
//            projRootPath + "/Classes/UnityAppController.mm",
//            "- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions",
//            @"
//    [[JPushEventCache sharedInstance] handFinishLaunchOption:launchOptions];
//    /*
//    不使用 IDFA 启动 SDK。
//    参数说明：
//        appKey: 极光官网控制台应用标识。
//        channel: 频道，暂无可填任意。
//        apsForProduction: YES: 发布环境；NO: 开发环境。
//    */
//    [JPUSHService setupWithOption:launchOptions appKey:@""" + app_key + @""" channel:@"""" apsForProduction:NO];

//    /*
//    使用 IDFA 启动 SDK（不能与上述方法同时使用）。
//    参数说明：
//        appKey: 极光官网控制台应用标识。
//        channel: 频道，暂无可填任意。
//        apsForProduction: YES: 发布环境；NO: 开发环境。
//        advertisingIdentifier: IDFA广告标识符。根据自身情况选择是否带有 IDFA 的启动方法，并注释另外一个启动方法。
//    */
//    //  NSString *advertisingId = [[[ASIdentifierManager sharedManager] advertisingIdentifier] UUIDString];
//    //  [JPUSHService setupWithOption:launchOptions appKey:@""替换成你自己的 Appkey"" channel:@"""" apsForProduction:NO SadvertisingIdentifier:advertisingId];

//"
//            );
//        #endregion


//        #region 注册token
//        XcodeProjectMod.AddCode2Method_InEnd(
//            projRootPath + "/Classes/UnityAppController.mm",
//            "- (void)application:(UIApplication*)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData*)deviceToken",
//            @"
//    [JPUSHService registerDeviceToken:deviceToken];
//"
//            );
//        #endregion

//        #region 远程消息
//        XcodeProjectMod.AddCode2Method_InEnd(
//    projRootPath + "/Classes/UnityAppController.mm",
//            "- (void)application:(UIApplication*)application didReceiveRemoteNotification:(NSDictionary*)userInfo",
//    @"
//    [[JPushEventCache sharedInstance] sendEvent:userInfo withKey:@""JPushPluginReceiveNotification""];
//    [JPUSHService handleRemoteNotification: userInfo];
//"
//    );
//        #endregion
//        #region 收到消息
//        XcodeProjectMod.AddCode2Method_InEnd(
//    projRootPath + "/Classes/UnityAppController.mm",
//    "- (void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo fetchCompletionHandler:(void (^)(UIBackgroundFetchResult result))handler",
//    @"
//    [[JPushEventCache sharedInstance] sendEvent:userInfo withKey:@""JPushPluginReceiveNotification""];
//"
//    );
//        #endregion

//        #region 清除角标
//        //https://blog.csdn.net/JennyHermes/article/details/78622328
//        XcodeProjectMod.AddCode2Method_InEnd(
//projRootPath + "/Classes/UnityAppController.mm",
//"- (void)applicationDidBecomeActive:(UIApplication*)application",
//@"
//    [application setApplicationIconBadgeNumber:0];
//    [application cancelAllLocalNotifications];
//");
//        #endregion


//        #region Home键切换回来
//        XcodeProjectMod.AddCode2Method_InEnd(
//        projRootPath + "/Classes/UnityAppController.mm",
//        "- (void)applicationWillEnterForeground:(UIApplication*)application",
//@"
//    if (_unityAppReady)
//    {
//        UnitySendMessage(""_GameMain"", ""OnIOSApplicationWillEnterForeground"", @"""".UTF8String);
//    }
//");
    //    #endregion
    //}

}

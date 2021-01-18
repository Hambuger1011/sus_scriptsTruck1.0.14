#if (UNITY_IOS)

using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode.Custom;
using UnityEngine;

namespace GameFramework
{
    public static class XcodeBuildPostBuild
    {
        public static void SetCapabilities(string pathToBuildProject)
        {
            string projPath = pathToBuildProject + "/Unity-iPhone.xcodeproj/project.pbxproj"; //项目路径，这个路径在mac上默认是不显示的，需要右键->显示包内容才能看到。unity到处的名字就是这个。
            PBXProject _pbxProj = new PBXProject();//创建xcode project类

            _pbxProj.ReadFromString(File.ReadAllText(projPath));//xcode project读入

            var capManager = new ProjectCapabilityManager(projPath, "usdk.entitlements", PBXProject.GetUnityTargetName());//创建设置Capability类
            capManager.AddInAppPurchase();
            capManager.AddAssociatedDomains(new string[] { "applinks:bo09.t4m.cn" });
            capManager.AddPushNotifications(false);//设置Capability
            capManager.AddGameCenter();
            capManager.AddSignInWithApple();
            capManager.WriteToFile();//写入文件保存
            
//            string _targetGuid = _pbxProj.TargetGuidByName("Unity-iPhone");//获得Target名
//            _pbxProj.AddFrameworkToProject(_targetGuid, "Storekit.framework", true);
//            _pbxProj.AddFrameworkToProject(_targetGuid, "AuthenticationServices.framework", true); 
//            File.WriteAllText(projPath, _pbxProj.WriteToString());
//            
//            try
//            {
//                Debug.Log("AddCapability InAppPurchase");
//                _pbxProj.AddCapability(xcodeTarget, PBXCapabilityType.InAppPurchase);
//            }
//            catch (Exception e)
//            {
//                Debug.LogException(e);
//                Console.WriteLine(e);
//            }
//            
//            File.WriteAllText(projPath, _pbxProj.WriteToString());
//            _pbxProj.AddFrameworkToProject(xcodeTarget, "AuthenticationServices.framework", true);

/*
            try
            {
                Debug.Log("AddCapability GameCenter");
                _pbxProj.AddCapability(xcodeTarget, PBXCapabilityType.GameCenter);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Console.WriteLine(e);
            }
            
            try
            {
                Debug.Log("AddCapability InAppPurchase");
                _pbxProj.AddCapability(xcodeTarget, PBXCapabilityType.InAppPurchase);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Console.WriteLine(e);
            }
            
            try
            {
                Debug.Log("AddCapability PushNotifications");
                _pbxProj.AddCapability(xcodeTarget, PBXCapabilityType.PushNotifications);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Console.WriteLine(e);
            }
            */

//            File.WriteAllText(projPath, _pbxProj.WriteToString());
        }
        
        public static void SetInfo(string pathToBuildProject)
        {
            string _plistPath = pathToBuildProject + "/Info.plist";
            PlistDocument _plist = new PlistDocument();

            _plist.ReadFromString(File.ReadAllText(_plistPath));
            PlistElementDict _rootDic = _plist.root;

            _rootDic.SetString("NSContactsUsageDescription", "通讯录");
            _rootDic.SetString("NSMicrophoneUsageDescription", "麦克风");
            _rootDic.SetString("NSPhotoLibraryUsageDescription", "相册");
            _rootDic.SetString("NSCameraUsageDescription", "相机");
            _rootDic.SetString("NSLocationAlwaysUsageDescription", "地理位置");

            File.WriteAllText(_plistPath, _plist.WriteToString());
        }
        
        public static void SetBuildSettings(string pathToBuildProject)
        {
            string projPath = pathToBuildProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
            PBXProject _pbxProj = new PBXProject();

            _pbxProj.ReadFromString(File.ReadAllText(projPath));
            string _targetGuid = _pbxProj.TargetGuidByName("Unity-iPhone");
            Debug.Log("_targetGuid:" + _targetGuid);


            _pbxProj.AddBuildProperty(_targetGuid, "ProvisioningStyle", "Manual");
            string debugConfig = _pbxProj.BuildConfigByName(_targetGuid, "Debug");
            Debug.Log("debugConfig:" + debugConfig);
            string releaseConfig = _pbxProj.BuildConfigByName(_targetGuid, "Release");
            Debug.Log("releaseConfig:" + releaseConfig);
            string releaseForProfilingConfig = _pbxProj.BuildConfigByName(_targetGuid, "ReleaseForProfiling");
            Debug.Log("releaseForProfilingConfig:" + releaseForProfilingConfig);
            string releaseForRunningConfig = _pbxProj.BuildConfigByName(_targetGuid, "ReleaseForRunning");
            Debug.Log("releaseForRunningConfig:" + releaseForRunningConfig);

            _pbxProj.SetBuildPropertyForConfig(debugConfig, "PROVISIONING_PROFILE_SPECIFIER", "SUS_DIs");
            _pbxProj.SetBuildPropertyForConfig(debugConfig, "ProvisioningStyle", "Manual");
            _pbxProj.SetBuildPropertyForConfig(debugConfig, "CODE_SIGN_STYLE", "Manual");
            _pbxProj.SetBuildPropertyForConfig(debugConfig, "ENABLE_BITCODE", "NO");
            
            _pbxProj.SetBuildPropertyForConfig(releaseConfig, "PROVISIONING_PROFILE_SPECIFIER", "SUS_DIs");
            _pbxProj.SetBuildPropertyForConfig(releaseConfig, "ProvisioningStyle", "Manual");
            _pbxProj.SetBuildPropertyForConfig(releaseConfig, "CODE_SIGN_STYLE", "Manual");
            _pbxProj.SetBuildPropertyForConfig(releaseConfig, "ENABLE_BITCODE", "NO");
            
            _pbxProj.SetBuildPropertyForConfig(releaseForRunningConfig, "PROVISIONING_PROFILE_SPECIFIER", "SUS_DIs");
            _pbxProj.SetBuildPropertyForConfig(releaseForRunningConfig, "ProvisioningStyle", "Manual");
            _pbxProj.SetBuildPropertyForConfig(releaseForRunningConfig, "CODE_SIGN_STYLE", "Manual");
            _pbxProj.SetBuildPropertyForConfig(releaseForRunningConfig, "ENABLE_BITCODE", "NO");
            
            _pbxProj.SetBuildPropertyForConfig(releaseForProfilingConfig, "PROVISIONING_PROFILE_SPECIFIER", "SUS_DIs");
            _pbxProj.SetBuildPropertyForConfig(releaseForProfilingConfig, "ProvisioningStyle", "Manual");
            _pbxProj.SetBuildPropertyForConfig(releaseForProfilingConfig, "CODE_SIGN_STYLE", "Manual");
            _pbxProj.SetBuildPropertyForConfig(releaseForProfilingConfig, "ENABLE_BITCODE", "NO");

            _pbxProj.SetTeamId(_targetGuid, "QCHAAV6HH8");

            File.WriteAllText(projPath, _pbxProj.WriteToString());
        }
        
        public static void AddFramework(string pathToBuildProject)
        {
            string xcodeProjectPath = pathToBuildProject + "/Unity-iPhone.xcodeproj/project.pbxproj";

            PBXProject xcodeProject = new PBXProject();
            xcodeProject.ReadFromFile(xcodeProjectPath);

            string xcodeTarget = xcodeProject.TargetGuidByName("Unity-iPhone");

            xcodeProject.AddFrameworkToProject(xcodeTarget, "SafariServices.framework", true);
            xcodeProject.AddFrameworkToProject(xcodeTarget, "WebKit.framework", true);
            xcodeProject.AddFrameworkToProject(xcodeTarget, "GameKit.framework", true);
            
            xcodeProject.AddCapability(xcodeTarget, PBXCapabilityType.InAppPurchase);

// Save the changes to Xcode project file.
            xcodeProject.WriteToFile(xcodeProjectPath);

        }
        
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuildProject)
        {
            
            if (target == BuildTarget.iOS) {
                //
                // // Get plist
                // string plistPath = pathToBuildProject + "/Info.plist";
                // PlistDocument plist = new PlistDocument();
                // plist.ReadFromString(File.ReadAllText(plistPath));
                //
                // // Get root
                // PlistElementDict rootDict = plist.root;
                //
                // // Change value of CFBundleVersion in Xcode plist
                // var buildKey = "NSPhotoLibraryUsageDescription";
                // rootDict.SetString(buildKey,"Test");
                //
                // // Write to file
                // File.WriteAllText(plistPath, plist.WriteToString());
            }
            
            // 以下代码研发情仔细阅读一下，以下有包含接入USDK的配置
            if (target == BuildTarget.iOS)
            {
                {
                    // get plist
                    string AppCon = pathToBuildProject + "/Classes/UnityAppController.mm";
                    string str = File.ReadAllText(AppCon);

                    int index = -1;
                    // 添加IGGSDK头文件
                    index = str.IndexOf("USDK");
                    if (index == -1)
                    {
                        str = str.Replace("#include <sys/sysctl.h>", "#include <sys/sysctl.h>\n\n#include <USDKWrapper/IGGPushNotification.h>\n#include <IGGTSHybrid/IGGTSHybrid.h>\n");
                    }

                    index = str.IndexOf("[[IGGPushNotification sharedInstance] onApplication:application didRegisterForRemoteNotificationsWithDeviceToken:deviceToken];");
                    if (index == -1)
                    {
                        string strFirst = "didRegisterForRemoteNotificationsWithDeviceToken:(NSData*)deviceToken\n{\n";
                        str = str.Replace(strFirst, strFirst + "\t::printf(\"didRegisterForRemoteNotificationsWithDeviceToken\");\n" 
                            + "\t[[IGGPushNotification sharedInstance] onApplication:application didRegisterForRemoteNotificationsWithDeviceToken:deviceToken];\n");
                    }

                    index = str.IndexOf("[[IGGPushNotification sharedInstance] onApplication:application didReceiveRemoteNotification:userInfo];");
                    if (index == -1)
                    {
                        string strFirst = "didReceiveRemoteNotification:(NSDictionary*)userInfo\n{\n";
                        str = str.Replace(strFirst, strFirst + "\t::printf(\"didReceiveRemoteNotification:(NSDictionary*)userInfo\");\n" 
                            + "\t[[IGGPushNotification sharedInstance] onApplication:application didReceiveRemoteNotification:userInfo];\n");
                    }
                    
                    index = str.IndexOf("[IGGTSHybrid.sharedInstance didReceiveRemoteNotifications:userInfo];");
                    if (index == -1)
                    {
                        string strFirst = "didReceiveRemoteNotification:(NSDictionary*)userInfo\n{\n";
                        str = str.Replace(strFirst, strFirst 
                                                             + "\t::printf(\"didReceiveRemoteNotification:(NSDictionary*)userInfo\");\n" 
                                                             +"\tif(userInfo != nil){\n" 
                                                             +"\tif([userInfo objectForKey:@\"broadcast\"] ){\n" 
                                                             +"\tNSString * tempstr = (NSString *)[userInfo objectForKey:@\"broadcast\"];\n" 
                                                             +"\tNSLog(@\"broadcast info：%@\",tempstr);\n" 
                                                             +"\t//NSString * userinfoStr =(NSString *)userInfo;\n" 
                                                             +"\t//NSLog(@\"broadcast userinfoStr：%@\",userinfoStr);\n" 
                                                             +"\t//UnitySendMessage( \"GoogleSdk\", \"ReceiveTSHMsg\", [userinfoStr UTF8String]  );\n" 
                                                             +"\tUnitySendMessage( \"GoogleSdk\", \"ReceiveBroadcast\", [tempstr UTF8String]  );\n" 
                                                             +"\t}\n" 
                                                             +"\t}\n" 
                                                             + "\tif ([IGGTSHybrid.sharedInstance didReceiveRemoteNotifications:userInfo]) {\n"
                                                             + "\t\tUIAlertController *alert = [UIAlertController alertControllerWithTitle:@\"客服\" message:@\"收到一条客服回复，是否现在去查看(游戏由运营决定可选接入、是否弹窗)\"\n"
                                                             + "\t\t\tpreferredStyle:UIAlertControllerStyleAlert];\n"
                                                             + "\t\t[alert addAction:[UIAlertAction actionWithTitle:@\"去查看\" style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {\n"
                                                             + "\t\t\t[IGGTSHybrid.sharedInstance showPanel:userInfo onComplete:^(IGGError *error) {\n"
                                                             + "\n"
                                                             + "\t\t\t}];"
                                                             + "\t\t}]];\n"
                                                             + "\t\t[alert addAction:[UIAlertAction actionWithTitle:@\"Cancel\" style:UIAlertActionStyleCancel handler:nil]];\n"
                                                             + "\t\t[self.window.rootViewController presentViewController:alert animated:true completion:nil];\n"
                                                             + "\t}\n"); 
                    }

                    // index = str.IndexOf("[[IGGPushNotification sharedInstance] application:application didReceiveRemoteNotification:userInfo fetchCompletionHandler:handler];");
                    // if (index == -1)
                    // {
                    //     string strFirst = "didReceiveRemoteNotification:(NSDictionary *)userInfo fetchCompletionHandler:(void (^)(UIBackgroundFetchResult result))handler\n{\n";
                    //     str = str.Replace(strFirst, strFirst + "\t::printf(\"didReceiveRemoteNotification: (NSDictionary*)userInfo fetchCompletionHandler\");\n"
                    //         + "\t[[IGGPushNotification sharedInstance] application:application didReceiveRemoteNotification:userInfo fetchCompletionHandler:handler];\n");
                    // }
                    
                    string annotationCode = "- (void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo fetchCompletionHandler:(void (^)(UIBackgroundFetchResult result))handler\n"
                                            + "{\n"
                                            + "    AppController_SendNotificationWithArg(kUnityDidReceiveRemoteNotification, userInfo);\n"
                                            + "    UnitySendRemoteNotification(userInfo);\n"
                                            + "    if (handler)\n"
                                            + "    {\n"
                                            + "        handler(UIBackgroundFetchResultNoData);\n"
                                            + "    }\n"
                                            + "}\n";

                    index = str.IndexOf(annotationCode);
                    Debug.Log("annotationCode:" + index);
                    if (index != -1)
                    {
                        str = str.Replace(annotationCode, "/*  unuse.\n" + annotationCode + "*/\n");
                    }

                    index = str.IndexOf("[[IGGPushNotification sharedInstance] onApplication:application willFinishLaunchingWithOptions:launchOptions];");
                    if (index == -1)
                    {
                        string strFirst = "willFinishLaunchingWithOptions:(NSDictionary*)launchOptions\n{\n";
                        str = str.Replace(strFirst, strFirst + "\t::printf(\"willFinishLaunchingWithOptions\");\n" 
                            + "\t[[IGGPushNotification sharedInstance] onApplication:application willFinishLaunchingWithOptions:launchOptions];\n");
                    }

                    index = str.IndexOf("[[IGGPushNotification sharedInstance] requestRemoteNotificationPermission];");
                    if (index == -1)
                    {
                        string strFirst = "didFinishLaunchingWithOptions:(NSDictionary*)launchOptions\n{\n";
                        string str2 = "\t[[IGGPushNotification sharedInstance] requestRemoteNotificationPermission];\n";
                        str = str.Replace(strFirst, strFirst+str2);
                    } 
                    
//                    index = str.IndexOf("[IGGTSHybrid.sharedInstance didFinishLaunchingWithOptions:launchOptions];");
//                    if (index == -1)
//                    {
//                        string strFirst = "didFinishLaunchingWithOptions:(NSDictionary*)launchOptions\n{\n";
//                        string str2 = "\t[IGGTSHybrid.sharedInstance didFinishLaunchingWithOptions:launchOptions];\n";
//                        str = str.Replace(strFirst, strFirst+str2);
//                    } 

                    index = str.IndexOf("[NSMutableArray arrayWithArray:[[NSProcessInfo processInfo] arguments]]");
                    if (index == -1)
                    {
                        string strFirst = "didFinishLaunchingWithOptions:(NSDictionary*)launchOptions\n{\n";
                        string str1 = "\tNSMutableArray *newArguments = [NSMutableArray arrayWithArray:[[NSProcessInfo processInfo] arguments]];\n";
                        string str2 = "\t[newArguments addObject:@\"-FIRDebugEnabled\"];\n";
                        string str3 = "\t[[NSProcessInfo processInfo] setValue:[newArguments copy] forKey:@\"arguments\"];\n";
                        str = str.Replace(strFirst, strFirst+str1+str2+str3);
                    } 
                    
                    File.WriteAllText(AppCon, str);
                }
                {
                    string preprocessorPath = pathToBuildProject + "/Classes/Preprocessor.h";
                    string text = File.ReadAllText(preprocessorPath);
                    text = text.Replace("UNITY_USES_REMOTE_NOTIFICATIONS 0", "UNITY_USES_REMOTE_NOTIFICATIONS 1");
                    File.WriteAllText(preprocessorPath, text);
                }

                
                SetBuildSettings(pathToBuildProject);
                
                SetCapabilities(pathToBuildProject);
            }
        }
    }
}
#endif
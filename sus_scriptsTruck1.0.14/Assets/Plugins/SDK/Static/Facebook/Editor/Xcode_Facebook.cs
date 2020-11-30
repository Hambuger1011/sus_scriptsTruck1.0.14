using UnityEngine;
using System.Collections;
using UnityEditor.Callbacks;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEditor.iOS.Xcode.Custom;
using UnityEditor.iOS.Xcode.Custom.Extensions;

public static class Xcode_Facebook
{

    [PostProcessBuild(10000 + 1)]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string projRootPath)
    {
        if (buildTarget != BuildTarget.iOS)
        {
            return;
        }

        string projPath = PBXProject.GetPBXProjectPath(projRootPath);
        ChangePbProj(projPath);
        ChangeXcodePlist(projRootPath);
        ChangeSource(projPath);




    }


    static void ChangePbProj(string projPath)
    {
        PBXProject proj = new PBXProject();
        proj.ReadFromString(File.ReadAllText(projPath));
        string targetGuid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());


        #region 修改工程配置文件project.pbxproj
        
        //Frameworks搜索路径
        string[] dirs = new string[]
        {
            // "Plugins/SDK/Static/Facebook/FacebookSDK/Plugins/iOS/**",//遍历/**
        };
        //proj.SetBuildProperty(target, "FRAMEWORK_SEARCH_PATHS", "$(inherited)");
        //proj.AddBuildProperty(target, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Frameworks");
        //proj.AddBuildProperty(target, "HEADER_SEARCH_PATHS", "$(SRCROOT)/Libraries/");//头文件搜索路径
        foreach (var d in dirs)
        {
            proj.AddBuildProperty(targetGuid, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Frameworks/" + d);//包搜索路径
            proj.AddBuildProperty(targetGuid, "HEADER_SEARCH_PATHS", "$(SRCROOT)/Libraries/" + d);//头文件搜索路径
        }
        proj.AddBuildProperty(targetGuid, "LIBRARY_SEARCH_PATHS", "$(inherited)");
        proj.AddBuildProperty(targetGuid, "USER_HEADER_SEARCH_PATHS","$(inherited)");
        #endregion

        File.WriteAllText(projPath, proj.WriteToString());
    }

    static void ChangeXcodePlist(string projRootPath)
    {

    }

    static void ChangeSource(string projPath)
    {

    }

}

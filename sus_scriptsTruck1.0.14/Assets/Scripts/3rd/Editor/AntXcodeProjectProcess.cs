#if UNITY_IOS
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using UnityEditor.iOS.Xcode;
using System.IO;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

public class AntXcodeProjectProcess : MonoBehaviour
{

 
    [PostProcessBuild(100)]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            //project
            string projPath = PBXProject.GetPBXProjectPath(path);
            PBXProject proj = new PBXProject();
            proj.ReadFromString(File.ReadAllText(projPath));
    //------------------------------------------------------------添加代码
            //读取UnityViewControllerBaseiOS.mm文件
            XClass UnityViewControllerBaseiOS = new XClass(path + "/Classes/UI/UnityViewControllerBase+iOS.mm");
            UnityViewControllerBaseiOS.Replace("NSAssert(UnityShouldAutorotate(),", "//NSAssert(UnityShouldAutorotate(),");

            File.WriteAllText(projPath, proj.WriteToString());

        }
    }




}
#endif
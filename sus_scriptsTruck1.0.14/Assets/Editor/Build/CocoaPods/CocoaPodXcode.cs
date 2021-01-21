using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode.Custom;

namespace GoogleMobileAds
{
    public class CocoaPodXcode
    {
        [PostProcessBuildAttribute(9996)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string projRootPath)
        {
            if (buildTarget != BuildTarget.iOS)
            {
                return;
            }
            RunPodUpdate(projRootPath);
        }

        public static void RunPodUpdate(string projRootPath)
        {
#if !UNITY_CLOUD_BUILD
            // Copy the podfile into the project.
            string podfile = "Assets/Editor/Build/CocoaPods/Podfile";
            string destPodfile = projRootPath + "/Podfile";

            if (!System.IO.File.Exists(podfile))
            {
                UnityEngine.Debug.LogError(@"Could not locate Podfile in:\n" + podfile);
                return;
            }

            UnityEngine.Debug.Log(@"locate Podfile in:\n"+podfile);
            File.Copy(podfile, destPodfile, true);
#endif
        }
    }
}

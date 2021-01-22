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
        [PostProcessBuild(1)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string projRootPath)
        {
            UnityEngine.Debug.Log(@"1111111111111111111111111111111111111111111");
#if UNITY_IOS
            // Copy the podfile into the project.
            string podfile = "Assets/Editor/Build/CocoaPods/Podfile";
            string destPodfile = projRootPath + "/Podfile";

            if (!System.IO.File.Exists(podfile))
            {
                UnityEngine.Debug.LogError(@"Could not locate Podfile in:\n" + podfile);
                return;
            }

            UnityEngine.Debug.Log(@"locate Podfile in:\n" + podfile);
            File.Copy(podfile, destPodfile, true);
#endif
        }
    }
}

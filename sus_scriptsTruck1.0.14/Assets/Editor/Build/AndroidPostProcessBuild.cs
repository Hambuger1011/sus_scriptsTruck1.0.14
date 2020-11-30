using Framework;

#if UNITY_ANDROID || UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.IO;
using System.Xml;

public class AndroidPostProcessBuild
{
//#if ANDROID_SDK_WEILAN
    public static class WLSDKParameter
    {
        public static string APPID = "20";
        public static string CLIENTID = "20";
        public static string CLIENTKEY = "7b8c5b004dff130f7ef45d185eb2035b";
        public static string AGENT = "tmzy_20_157";
    }

    public static bool wlSDK = false;
//#endif
    
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget != BuildTarget.Android)
        {
            return;
        }

        if(!Directory.Exists(path))
        {
            return;
        }

        string strAndroidManifestPath = null;
        var files = Directory.GetFiles(path, "AndroidManifest.xml", SearchOption.AllDirectories);
        foreach(var f in files)
        {
            string filename = Path.GetFileName(f);
            if(filename == "AndroidManifest.xml")
            {
                strAndroidManifestPath = f;
                break;
            }
        }

        if(strAndroidManifestPath == null)
        {
            Debug.LogError("找不到AndroidManifest.xml");
            return;
        }

        System.Text.UTF8Encoding utf8 = new System.Text.UTF8Encoding(false);
        string strXml = File.ReadAllText(strAndroidManifestPath, utf8);
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(strXml);


        var manifestRoot = xmlDoc.DocumentElement;
        var applicationRoot = manifestRoot.SelectSingleNode("//application");
        
        if (wlSDK)
            WLSDKSetting(applicationRoot);

        xmlDoc.Save(strAndroidManifestPath);
    }


    static void WLSDKSetting(XmlNode applicationRoot)
    {
        string beginFlag = "WLSDK START";
        string endFlag = "WLSDK START";
        bool bStart = false;
        foreach (XmlNode node in applicationRoot.ChildNodes)
        {
            //Debug.Log(node.Name);
            if (node.Name == "#comment")
            {
                if (node.InnerText.Trim() == beginFlag)
                {
                    bStart = true;
                }
                else if (bStart && node.InnerText.Trim() == endFlag)
                {
                    break;
                }
            }

            if (!bStart)
            {
                continue;
            }
            if (node.Name.Trim() == "meta-data")
            {
                //Debug.Log(node["name"]);
                XmlAttributeCollection attrs = node.Attributes as XmlAttributeCollection;
                if (attrs.Count < 2)
                {
                    continue;
                }

                var keyAttr = attrs[0];
                var valAttr = attrs[1];
                switch (keyAttr.Value)
                {
                    case "APPID":
                        valAttr.Value = WLSDKParameter.APPID;
                        break;
                    case "CLIENTID":
                        valAttr.Value = WLSDKParameter.CLIENTID;
                        break;
                    case "CLIENTKEY":
                        valAttr.Value = WLSDKParameter.CLIENTKEY;
                        break;
                    case "AGENT":
                        valAttr.Value = WLSDKParameter.AGENT;
                        break;
                }
            }
        }

    }



    public static void SetAndroidManifest(string appClassName,string launchMode)
    {
        return;
        string strAndroidManifestPath = Application.dataPath + "/Plugins/Android/AndroidManifest.xml";
        System.Text.UTF8Encoding utf8 = new System.Text.UTF8Encoding(false);
        string strXml = File.ReadAllText(strAndroidManifestPath, utf8);
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(strXml);


        var manifestRoot = xmlDoc.DocumentElement;
        var applicationRoot = manifestRoot.SelectSingleNode("application");

        var fuckNode = applicationRoot["Fuck"];
        var length = fuckNode.Attributes["length"];
        Debug.LogError(fuckNode.Name+" "+length.Value);
        //set application name
        XmlAttributeCollection attrs = applicationRoot.Attributes;
        Debug.Log(attrs["android:name"].Value);
        attrs["android:name"].Value = appClassName;
        attrs["android:debuggable"].Value = ProjectTools.isDebug ? "true" : "false";

        //set main activity launchmode
        foreach (XmlNode node in applicationRoot.ChildNodes)
        {
            if (node.Name.Trim() != "activity")
            {
                continue;
            }
            var tagNode = node.SelectSingleNode("intent-filter/action");
            if(tagNode == null)
            {
                continue;
            }
            attrs = tagNode.Attributes as XmlAttributeCollection;
            if(attrs["android:name"].Value == "android.intent.action.MAIN")
            {
                attrs = node.Attributes as XmlAttributeCollection;
                attrs["android:launchMode"].Value = launchMode;
                Debug.Log("main activity:" + attrs["android:name"].Value);
                break;
            }
        }
            
        xmlDoc.Save(strAndroidManifestPath);
        AssetDatabase.Refresh();
    }

    //[MenuItem("AAA/a")]
    //static void ParseXml()
    //{
    //    SetAndroidManifest("com.a.a", "singTop");
    //}
#if _DEBUG_
    [MenuItem("AAA/a")]
    static void ParseXml()
    {
        string strXmlPath = @"H:\S03_Game_Android\Assets\Plugins\Android\AndroidManifest.xml";
        System.Text.UTF8Encoding utf8 = new System.Text.UTF8Encoding(false);
        string strXml = File.ReadAllText(strXmlPath, utf8);
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(strXml);
        
        var manifestRoot = xmlDoc.DocumentElement;
        var applicationRoot = manifestRoot.SelectSingleNode("//application");

        string beginFlag = "WLSDK START";
        string endFlag = "WLSDK START";
        bool bStart = false;
        foreach (XmlNode node in applicationRoot.ChildNodes)
        {
            //Debug.Log(node.Name);
            if(node.Name == "#comment")
            {
                if(node.InnerText.Trim() == beginFlag)
                {
                    bStart = true;
                }else if(bStart && node.InnerText.Trim() == endFlag)
                {
                    break;
                }
            }

            if(!bStart)
            {
                continue;
            }
            if(node.Name.Trim() == "meta-data")
            {
                //Debug.Log(node["name"]);
                XmlAttributeCollection attrs = node.Attributes as XmlAttributeCollection;
                if(attrs.Count < 2)
                {
                    continue;
                }
                switch(attrs[0].Value)
                {
                    case "APPID":
                        attrs[0].Value = WLSDKParameter.APPID;
                        break;
                    case "CLIENTID":
                        attrs[0].Value = WLSDKParameter.CLIENTID;
                        break;
                    case "CLIENTKEY":
                        attrs[0].Value = WLSDKParameter.CLIENTKEY;
                        break;
                    case "AGENT":
                        attrs[0].Value = WLSDKParameter.AGENT;
                        break;
                }
            }
        }

        xmlDoc.Save(strXmlPath);
    }
#endif
}
#endif

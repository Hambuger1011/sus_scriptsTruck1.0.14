//#define NEW_BUILD
namespace AB
{
    using Spine.Unity;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class AbOptions_DataTable : AbstractAbOptions
    {

        public override string SystemName
        {
            get
            {
                return ABBuildWindow.SystemName_DataTable;
            }
        }

        public override string ConfigAssetName
        {
            get
            {
                return "";
            }
        }

        public override bool PackShaders()
        {
            return true;
        }

        public override void OnBegin()
        {

        }

        public override void ScanResources()
        {
            var analyzer = this.builder.analyzer;

            analyzer.AddAsset("assets/bundle/data/pb_define.txt");
            analyzer.ScanToSingle<TextAsset>("Assets/Bundle/Data/Common");//打包配置表
            analyzer.Scan<Object>("Assets/Bundle/ImageWallSpine");// UGUI
            //analyzer.Scan<TextAsset>("Assets/Bundle/Data/BookDialog");//打包书本配置表
            ScanBookIcon();
            // ScanImageWallSpine();

            Debug.Log("asset bundle ouput:" + AbUtility.AbBuildPath);
        }


        public override void UpdateConfig(string output, AssetBundleManifest assetBundleManifest, AbResConfig editorAbConfig)
        {
            Debug.Log("PlayerSettings.bundleVersion:" + PlayerSettings.bundleVersion);
        }
        
        void ScanBookIcon()
        {

            var analyzer = this.builder.analyzer;
            var savePath = "Assets/Bundle/BookPreview/";
            var data = AbBookResEditor.MakeBook("icon", "Assets/Bundle/BookPreview/icon", savePath);
            var gofileName = AbUtility.NormalizerAbName(savePath + "icon.prefab");
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(gofileName);
            {
                analyzer.AddAsset(gofileName, gofileName);//把这个保存好的 AbPackage 保存进list
            }

            int p = 0;
            foreach (var obj in data.objs)// AbBookRes表中的objs的所有物体
            {
                ++p;
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("扫描({0}/{1})", p, data.objs.Length), obj, (float)p / data.objs.Length))
                {
                    throw new Exception("用户停止");
                }
                var fileName = AbUtility.NormalizerAbName(obj);
                if (!analyzer.IsValidAsset(fileName))
                {
                    continue;
                }
                analyzer.AddAsset(fileName, fileName);//把这个保存好的 AbPackage 保存进list
            }
        }
        
        void ScanImageWallSpine()
        {

            var analyzer = this.builder.analyzer;
            var savePath = "Assets/Bundle/ImageWallSpine/";
            var data = AbBookResEditor.MakeBook("imagewallspine", "Assets/Bundle/ImageWallSpine", savePath);
            var gofileName = AbUtility.NormalizerAbName(savePath + "imagewallspine.prefab");
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(gofileName);
            {
                analyzer.AddAsset(gofileName, gofileName);//把这个保存好的 AbPackage 保存进list
            }

            int p = 0;
            foreach (var obj in data.objs)// AbBookRes表中的objs的所有物体
            {
                ++p;
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("扫描({0}/{1})", p, data.objs.Length), obj, (float)p / data.objs.Length))
                {
                    throw new Exception("用户停止");
                }
                var fileName = AbUtility.NormalizerAbName(obj);
                if (!analyzer.IsValidAsset(fileName))
                {
                    continue;
                }
                analyzer.AddAsset(fileName, fileName);//把这个保存好的 AbPackage 保存进list
            }
        }




    }
}
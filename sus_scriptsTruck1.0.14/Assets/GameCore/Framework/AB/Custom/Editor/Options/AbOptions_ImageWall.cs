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

    public class AbOptions_ImageWall : AbstractAbOptions
    {

        public override string SystemName
        {
            get
            {
                return ABBuildWindow.SystemName_ImageWall;
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
            return false;
        }

        public override void OnBegin()
        {

        }

        public override void ScanResources()
        {
            var analyzer = this.builder.analyzer;
            analyzer.Scan<Object>("Assets/Bundle/ImageWall", enResType.eObject);
        }


        public override void UpdateConfig(string output, AssetBundleManifest assetBundleManifest, AbResConfig editorAbConfig)
        {
        }

    }
}
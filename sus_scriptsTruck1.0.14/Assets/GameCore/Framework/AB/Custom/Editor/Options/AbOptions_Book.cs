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

    public class AbOptions_Book : AbstractAbOptions
    {
        int bookID;
        string path;

        public override string SystemName
        {
            get
            {
                return "Book_" + bookID;
            }
        }

        public override string ConfigAssetName
        {
            get
            {
                return "assets/bundle/AbConfig/" + SystemName + ".asset";
            }
        }
        public AbOptions_Book(int id, string path)
        {
            this.bookID = id;
            this.path = path;
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
            analyzer.Scan<Object>(path, enResType.eObject);// UGUI
        }

        public override void UpdateConfig(string output, AssetBundleManifest assetBundleManifest, AbResConfig editorAbConfig)
        {
        }
    }
}
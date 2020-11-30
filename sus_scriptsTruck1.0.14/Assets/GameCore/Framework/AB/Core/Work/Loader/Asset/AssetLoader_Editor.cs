using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AB
{
    class AssetLoader_Editor : AssetLoader
    {
        public AssetLoader_Editor(CAsset asset) : base(asset) { }
        public override float GetProgress()
        {
            return 0;
        }

        public override int GetAllSize()
        {
            return 0;
        }

        public override int GetCurSize()
        {
            return 0;
        }

        public override bool Load(enResType resType, string assetName, bool isAsync,ref Object obj)
        {
            var t = AssetLoader.resType[(int)resType - 1];
            obj = ABMgr.Instance.LoadAssetInEditor(assetName, t);
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AB
{
    public class AssetLoader_AB : AssetLoader
    {
        AssetBundleRequest loadRequest = null;
        public AssetLoader_AB(CAsset asset) : base(asset) { }

        public override int GetAllSize()
        {
            int size = 0;
            if (this.asset != null && this.asset.abConfigItem != null)
            {
                size = this.asset.abConfigItem.size;
            }
            return size;
        }

        public override int GetCurSize()
        {
            int size = 0;
            if (this.asset != null && this.asset.abConfigItem != null)
            {
                float pro = this.asset.abConfigItem.size * loadRequest.progress;
                size = (int)pro;
            }
            return size;
        }

        public override float GetProgress()
        {
            if (loadRequest == null)
            {
                return 0;
            }
            return loadRequest.progress;
        }

        public override bool Load(enResType resType, string assetName, bool isAsync, ref Object obj)
        {
            if (isAsync)
            {
                if (loadRequest == null)
                {
                    var bundle = this.asset.bundle;
                    if (!bundle.IsDone())
                    {
                        return false;
                    }
                    if (bundle.assetbundle == null)
                    {
                        this.asset.strError = "加载bundle失败";
                        return true;
                    }

#if false
                    if (AbTask.cur_asset_async_num < AbTask.MAX_ASSET_ASYNC_NUM)
                    {
                        ++AbTask.cur_asset_async_num;

                        var t = AssetLoader.resType[(int)resType - 1];
                        var assetbundle = this.asset.bundle.assetbundle;
                        loadRequest = assetbundle.LoadAssetAsync(assetName, t);
                    }
#else

                    var t = AssetLoader.resType[(int)resType - 1];
                    var assetbundle = bundle.assetbundle;
                    obj = assetbundle.LoadAsset(assetName, t);
                    return true;
#endif
                }
                else if (loadRequest.isDone)
                {
                    --AbTask.cur_asset_async_num;

                    obj = loadRequest.asset;
                    return true;
                }

            }
            else
            {

                var bundle = this.asset.bundle;
                if (!bundle.IsDone())
                {
                    this.asset.strError = "ab异步加载中，不支持即时加载asset";
                    return true;
                }
                if (bundle.assetbundle == null)
                {
                    this.asset.strError = "加载bundle失败";
                    return true;
                }
                var t = AssetLoader.resType[(int)resType - 1];
                obj = bundle.assetbundle.LoadAsset(assetName, t);
                return true;
            }
            return false;
        }

    }
}

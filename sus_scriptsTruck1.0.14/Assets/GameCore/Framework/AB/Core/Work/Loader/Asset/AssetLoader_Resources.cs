using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AB
{
    public class AssetLoader_Resources : AssetLoader
    {
        ResourceRequest loadRequest;
        public AssetLoader_Resources(CAsset asset) : base(asset) { }

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
            if(loadRequest == null)
            {
                return 0;
            }
            return loadRequest.progress;
        }

        public override bool Load(enResType resType, string assetName, bool isAsync,ref Object obj)
        {
            if(isAsync)
            {
                if (loadRequest == null)
                {
                    if (AbTask.cur_asset_async_num < AbTask.MAX_ASSET_ASYNC_NUM)
                    {
                        ++AbTask.cur_asset_async_num;
                        var t = AssetLoader.resType[(int)resType - 1];
                        loadRequest = Resources.LoadAsync(assetName, t);
                    }
                }else if(loadRequest.isDone)
                {
                    --AbTask.cur_asset_async_num;

                    obj = loadRequest.asset;
                    return true;
                }

            }
            else
            {
                var t = AssetLoader.resType[(int)resType - 1];
                obj = Resources.Load(assetName, t);
                return true;
            }
            return false;
        }
    }
}

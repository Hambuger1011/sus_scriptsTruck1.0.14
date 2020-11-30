namespace AB
{
    using GameCore;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using UnityEngine;
    using UnityEngine.Networking;
    using XLua;

    public class AbWork
    {
        public AbResConfig resConfig { get; protected set; }

        public static Dictionary<string, CBundle> bundleCache = new Dictionary<string, CBundle>();//加载的bundle
        public Dictionary<string, CAsset> assetCache = new Dictionary<string, CAsset>();//加载的CAsset


        [Header("无用bundle")]
        [NonSerialized]
        public List<AbResItem> removeBundleKeys = new List<AbResItem>();


        [Header("无用Asset")]
        [NonSerialized]
        public List<string> removeAssetKeys = new List<string>();

        public string rootPath { get; private set; }
        public AbWork(string rootPath)
        {
            if(rootPath.EndsWith("/"))
            {
                this.rootPath = rootPath;
            }
            else
            {
                this.rootPath = rootPath + "/";
            }
            //Directory.CreateDirectory(AbUtility.abWritablePath + "/ab_cache/"+ this.rootPath);
        }

        public virtual CAsset LoadAsync(string refTag, enResType resType, string strAssetName, Action<CAsset> finishFunc = null, bool isAbRes = true)
        {
            string assetName = string.Empty;
            string key = string.Empty;
            using (CString.Block())
            {
                CString sb = CString.Alloc(256);
                sb.Append(strAssetName);
                sb.ToLower();
                isAbRes = sb.StartsWith("assets/");
                assetName = sb.ToString();

                sb.Insert(0, "_");
                sb.Insert(0, resType);
                key = sb.ToString();
            }

            isAbRes = assetName.StartsWith("assets/");
            CAsset asset = null;
            if (!assetCache.TryGetValue(key, out asset))
            {
                AbResItem abConfigItem = null;
                if (isAbRes && ABMgr.Instance.isUseAssetBundle)
                {
                    abConfigItem = resConfig.GetConfigItemByAssetName(assetName);
                    if (abConfigItem == null)
                    {
                        Debug.LogError("asset资源未打进包:" + assetName);
                        if (finishFunc != null)
                        {
                            finishFunc(null);
                        }
                        return asset;
                    }
                }
                asset = new CAssetAsync(key, resType, assetName, this, resConfig, abConfigItem, isAbRes);
            }

            ABMgr.Instance.AddAssetTag(refTag, asset);
            asset.AddCall(finishFunc);
            return asset;
        }
        
        public virtual CAsset LoadImme(string refTag, enResType resType, string strAssetName, bool isAbRes = true)
        {
            string assetName = string.Empty;
            string key = string.Empty;
            using (CString.Block())
            {
                CString sb = CString.Alloc(256);
                sb.Append(strAssetName);
                sb.ToLower();
                isAbRes = sb.StartsWith("assets/");
                assetName = sb.ToString();

                sb.Insert(0, "_");
                sb.Insert(0, resType);
                key = sb.ToString();
            }
            
            isAbRes = assetName.StartsWith("assets/");
            CAsset asset = null;
            if (!assetCache.TryGetValue(key, out asset))
            {
                AbResItem abConfigItem = null;
                if (isAbRes && ABMgr.Instance.isUseAssetBundle)
                {
                    abConfigItem = resConfig.GetConfigItemByAssetName(assetName);
                    if (abConfigItem == null)
                    {
                        Debug.LogError("asset资源未打进包:" + assetName);
                        return asset;
                    }
                }
                asset = new CAssetImmediate(key, resType, assetName, this, resConfig, abConfigItem, isAbRes);
            }

            if (asset != null)
            {
                if (!asset.IsDone())
                {
                    LOG.Error("同步和异步冲突:" + asset);
                }
            }

            //t = System.Environment.TickCount - t;
            //if(t > 20)
            //{
            //    LOG.Error(string.Concat("加载时间过长:", path, ",cost ", t, "ms"));
            //}
            ABMgr.Instance.AddAssetTag(refTag, asset);
            return asset;
        }


        public void FixedUpdate()
        {
            #region Asset
#if EDITOR
            using (var s = new ProfilerSample("Asset处理"))
#endif
            {
                //remove cache
                if (removeAssetKeys.Count > 0)
                {
                    LOG.Info("释放资源:"+removeAssetKeys.Count);
                    for (int i = 0; i < removeAssetKeys.Count; ++i)
                    {
                        var key = removeAssetKeys[i];
                        CAsset asset;
                        if (assetCache.TryGetValue(key, out asset))
                        {
                            if (asset.IsDone() && asset.retainCount <= 0)
                            {
                                //LOG.Info("移除asset资源:" + key);
                                asset.Dispose();
                            }
                        }
                    }
                    removeAssetKeys.Clear();
                }
            }
            #endregion

            #region 处理Bundle
            if (removeBundleKeys.Count > 0)
            {
                for (int i = 0; i < removeBundleKeys.Count; ++i)
                {
                    var item = removeBundleKeys[i];
                    if (item == null)
                    {
                        continue;
                    }
                    CBundle bundle;
                    if (AbWork.bundleCache.TryGetValue(item.fileHashName, out bundle))
                    {
                        if (bundle.CanRelease())
                        {
                            AbWork.bundleCache.Remove(item.fileHashName);
                            bundle.Unload(false);
                        }
                    }
                }
                removeBundleKeys.Clear();
            }
            #endregion
        }
    }
}
namespace AB
{
    using Framework;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UGUI;
    using UnityEngine;
    using UnityEngine.Networking;

    public class AbResBundle_Common : AbResBundle
    {
        public AbResBundle_Common(string rootPath) : base(rootPath)
        {
        }

        #region 下载配置
        public override IEnumerator DoLoadBundleConfig(string strAssetName, Action<bool> callback,Action<float> progressCallBack)
        {
            if (!ABMgr.Instance.isUseAssetBundle)
            {
                callback(true);
                yield break;
            }
            yield return DoDownload(strAssetName, UserDataManager.Instance.ResVersion, (bSuc, filename) =>
            {
                if (bSuc)
                {
                    var buff = File.ReadAllBytes(filename);
                    try
                    {

                        resConfig = new AbResConfig();
                        resConfig.bundleContext = this;
                        resConfig.Read(buff);
                        resConfig.Init();
                        LOG.Info("加载配置成功:" + resConfig.lastModify+",count="+resConfig.assetsMap.Count);
                    }
                    catch(Exception ex)
                    {
                        Debug.LogError(ex);
                        LOG.Error("加载配置失败");
                        CFileManager.WriteFileString(filename + ".txt", "");
                    }
                    //Debug.LogError("resConfig:" + resConfig.abConfItems.Count);
                }
                Init();
                if (callback != null)
                {
                    callback(bSuc);
                }
            }, progressCallBack);
        }

        void Init()
        {
            var abFileName = AbUtility.NormalizerAbName("assets/bundle/shaders.ab");
            var abConfig = this.resConfig;
            var abConfigItem = abConfig.GetConfigItemByAbName(abFileName);
            var bundle = CBundle.Get(this, abConfigItem, false);
            if (bundle != null)
            {
                bundle.Retain(this);
            }

            
            //this.LoadAsync("Shader", enResType.eObject, "assets/bundle/shaders/ShaderVariants.shaderVariants", (asset) =>
            //{
            //    if(asset.resObject == null)
            //    {
            //        return;
            //    }
            //    var t = System.Environment.TickCount;
            //    t = System.Environment.TickCount;
            //    ShaderVariantCollection svc = asset.resObject as ShaderVariantCollection;
            //     svc.WarmUp();
            //     t = System.Environment.TickCount - t;
            //     LOG.Info("shader WarmUp:" + t + "ms");
            // });
        }

        public override IEnumerator DoDownload(string name, string strVer, Action<bool, string> callback, Action<float> progressCallBack)
        {
#if ENABLE_DEBUG

            switch (AbUtility.loadType)
            {
                case enLoadType.eFile:
                    {
                        if (progressCallBack != null)
                            progressCallBack(1);
                        //var url = Path.Combine(AbUtility.abReadonlyPath, "/common/config");
                        string[] versionArr = UserDataManager.Instance.ResVersion.Split('@');
                        var url =string.Format("{0}ab/{1}/{2}/{3}/{4}/{5}", GameUtility.WritablePath,"common", GameUtility.Platform, versionArr[0], versionArr[1],"config");
                        callback(true, url);
                        yield break;
                    }
                    break;
                default:
                    break;
            }
#endif
            var filename = string.Concat(AbUtility.abWritablePath, "/ab_cache/common/config");
            var task = DownloadMgr.Instance.Download(GetConfigUrl(strVer), filename, strVer, 0, false);

            while (!task.isDone)
            {
                if (progressCallBack != null)
                    progressCallBack(task.Progress());
                yield return null;
            }

            if (progressCallBack != null)
                progressCallBack(1);
            callback(true, filename);
            yield break;
        }

        string GetConfigUrl(string strVer)
        {
            string _abFilePath;
            switch (AbUtility.loadType)
            {
                case enLoadType.eFile:
                    _abFilePath = string.Concat(AbUtility.abReadonlyPath, "config");
                    break;
                default:
                    _abFilePath = string.Concat(AbUtility.abUrlCommon, "config");
                    break;
            }
            return _abFilePath;
        }
        #endregion
    }
}
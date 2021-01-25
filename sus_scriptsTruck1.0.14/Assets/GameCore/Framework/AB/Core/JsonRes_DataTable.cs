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

    public class JsonRes_DataTable : AbResBundle
    {
        private string jsonfilename = "datatable.json";
        public JsonRes_DataTable(string rootPath) : base(rootPath)
        {
        }

        #region 下载配置
        public override IEnumerator DoLoadBundleConfig(string strAssetName, Action<bool> callback,Action<float> progressCallBack)
        {
            if (!ABMgr.Instance.isUseAssetBundle)
            {
                string filePath = Application.streamingAssetsPath + "/data/" + jsonfilename;
                var buff = File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(buff))
                {
                    JsonDTManager.Instance.SaveJDTToLocal(buff);
                }
                callback(true);
                yield break;
            }
            yield return DoDownload(strAssetName, UserDataManager.Instance.DataTableVersion, (bSuc, filename) =>
            {
                if (bSuc)
                {
                    var buff = File.ReadAllText(filename);
                    try
                    {
                        JsonDTManager.Instance.SaveJDTToLocal(buff);
                    }
                    catch(Exception ex)
                    {
                        Debug.LogError(ex);
                        LOG.Error("加载配置失败");
                        CFileManager.WriteFileString(filename + ".txt", "");
                    }
                }
                if (callback != null)
                {
                    callback(bSuc);
                }
            }, progressCallBack);
        }

        public override IEnumerator DoDownload(string name, string strVer, Action<bool, string> callback, Action<float> progressCallBack)
        {
            //string.Concat(AbUtility.abWritablePath, "/ab_cache/datatable/config");
            var filename = string.Concat(AbUtility.abWritablePath, "/ab_cache/"+jsonfilename);
            var task = DownloadMgr.Instance.Download(GameHttpNet.Instance.GetResourcesUrl()+jsonfilename, filename, strVer, 0, false);

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

        #endregion
    }
}

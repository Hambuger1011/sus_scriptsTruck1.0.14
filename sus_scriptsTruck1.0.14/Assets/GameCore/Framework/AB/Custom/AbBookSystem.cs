namespace AB
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    [XLua.LuaCallCSharp, XLua.Hotfix]
    public class AbBookSystem : AbWork, System.IDisposable
    {
        public int bookID;
        public AbBookSystem(int bookID) : base("book_"+ bookID)
        {
            this.bookID = bookID;
        }

        static AbBookSystem m_instance;
        public static AbBookSystem Instance
        {
            get
            {
                return m_instance;
            }
        }
        public static AbBookSystem Create(int bookID)
        {
            if(m_instance != null)
            {
                if (m_instance.bookID == bookID)
                {
                    return m_instance;
                }else
                {
                    m_instance.Dispose();
                }
            }
            m_instance = new AbBookSystem(bookID);
            return m_instance;
        }


        public void Dispose()
        {
            foreach (var itr in this.assetCache)
            {
                var asset = itr.Value;
                asset.ClearRef();
            }
            assetCache.Clear();
            foreach (var itr in bundleCache)
            {
                var bundle = itr.Value;
                if(bundle.woker == this)
                {
                    bundle.ClearRef();
                }
            }

            this.FixedUpdate();
            if (m_instance == this)
            {
                m_instance = null;
            }
        }

        string GetConfigUrl(string strVer)
        {
            string _abFilePath;
            switch (AbUtility.loadType)
            {
                case enLoadType.eFile:
                    _abFilePath = string.Concat(AbUtility.abReadonlyPath, this.rootPath, "config");
                    break;
                default:
                    _abFilePath = string.Concat(AbUtility.bookUri, this.rootPath, "config?", strVer);
                    break;
            }
            return _abFilePath;
        }

        public void InitSys(Action callback)
        {
            if(resConfig != null || !ABMgr.Instance.isUseAssetBundle)
            {
                callback();
                return;
            }
            var version = UserDataManager.Instance.GetBookVersion(this.bookID);
            if(AbUtility.loadType == enLoadType.eFile)
            {
                LoadConfig(GetConfigUrl(version));
                callback();
                return;
            }
            var filename = string.Concat(AbUtility.abWritablePath, "/ab_cache/",this.rootPath,"config");
            var task = DownloadMgr.Instance.Download(GetConfigUrl(version), filename, null, 0, false);
            task.AddComplete(()=>
            {
                if(m_instance != this)
                {
                    return;
                }
                LoadConfig(filename);
                callback();
            });
        }

        void LoadConfig(string filename)
        {
            var buff = File.ReadAllBytes(filename);
            try
            {

                resConfig = new AbResConfig();
                resConfig.bundleContext = this;
                resConfig.Read(buff);
                resConfig.Init();
                LOG.Info("加载配置成功:" + resConfig.lastModify + ",count=" + resConfig.assetsMap.Count);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                LOG.Error("加载配置失败");
                CFileManager.WriteFileString(filename + ".txt", "");
            }
        }
    }
}
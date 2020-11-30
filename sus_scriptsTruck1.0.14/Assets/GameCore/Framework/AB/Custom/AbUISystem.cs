namespace AB
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using AB;
    using System;
    using UnityEngine.UI;
    using Object = UnityEngine.Object;

    public class AbUISystem
    {
        public readonly AbResBundle bundle = new AbResBundle("common/");
        Dictionary<string, AbAtlas> m_atlasDict = new Dictionary<string, AbAtlas>();

        [NonSerialized]
        public List<AbTask> m_preLoadData = new List<AbTask>();


        [NonSerialized]
        public int hadLoaded = 0;
        

        public void LoadBundleConfig(Action<bool> callback,Action<float> progressCallBack)
        {
#if ENABLE_DEBUG

            switch (GameDataMgr.Instance.ResourceType)
            {
                case 0:
                    AbUtility.loadType = enLoadType.eFile;
                    break;
                default:
                    break;
            }
#endif
            ABMgr.Instance.StartCoroutine(bundle.DoLoadBundleConfig("config", callback, progressCallBack));
        }

        #region 预加载
        int needDownloadNum;
        public void PreLoad()
        {
            needDownloadNum = 1;
            hadLoaded = 0;
            m_preLoadData.Clear();

            //下载prefab，添加资源到队列
            PreLoadBookBanner();

        }

        /// <summary>
        /// 把所有资源添加完在计算进度
        /// </summary>
        void PreLoadOther()
        {
            if(needDownloadNum > 0)
            {
                return;
            }
#if ENABLE_DEBUG
            PreLoadAsset(enResType.ePrefab, CUIID.Canvas_Debug);
            PreLoadAsset(enResType.ePrefab, CUIID.Canvas_Log);
            PreLoadAsset(enResType.ePrefab, CUIID.Canvas_Test);
#endif

            PreLoadAsset(enResType.eText, "assets/bundle/data/pb_define.txt");
            PreLoadBundle("assets/bundle/data/common.ab");
            //PreLoadBundle("assets/bundle/resident.ab");
            //PreLoadBundle("assets/bundle/CatPreview.ab");
            PreLoadAsset(enResType.eAudio, "assets/bundle/music/audiotones/book_click.mp3");
            //PreLoadBundle("Assets/StoryEditor/Res.ab");
            //
            // if (AllSize == -1)
            // {
            //     AllSize = 0;
            // }

            //资源总大小
            GetAllSize();

        }

        public float abAllSize;

        private void PreLoadBookBanner()
        {
            var path = string.Concat("assets/bundle/BookPreview/icon.prefab");
            var asset = ABSystem.ui.bundle.LoadAsync(AbTag.Global, enResType.ePrefab, path, (_) =>
            {
                var resList = _.Get<AbBookRes>();
                if(_.abConfigItem != null)
                {
                    LOG.Error("res count:" + resList.objs.Length + ":" + _.abConfigItem.fileHashName + ",resVer=" + UserDataManager.Instance.ResVersion);
                }
                else
                {
                    LOG.Error("res count:" + resList.objs.Length + ",resVer=" + UserDataManager.Instance.ResVersion);
                }
                foreach (var res in resList.objs)
                {
                    //LOG.Warn("加载:" + res);
                    PreLoadAsset(enResType.eObject, res);
                }
                --needDownloadNum;
                PreLoadOther();
            });
        }

        public CAsset PreLoadAtlas(string strAtlasName)
        {
            var asset = bundle.LoadAsync(AbTag.Global, enResType.eAtlas, string.Concat("assets/bundle/atlas/", strAtlasName, ".prefab"));
            if (asset == null)
            {
                Debug.LogError("预加载图集失败:" + strAtlasName);
                return null;
            }
            asset.Retain(this);
            asset.AddCall((_) =>
            {
                var atlas = _.resPrefab.GetComponent<AbAtlas>();
                m_atlasDict.Add(strAtlasName, atlas);
            });
            m_preLoadData.Add(asset);
            return asset;
        }

        public CBundle PreLoadBundle(string strBundleName)
        {
            if (!ABMgr.Instance.isUseAssetBundle)
            {
                return null;
            }
            var abFileName = AbUtility.NormalizerAbName(strBundleName);
            var context = ABSystem.ui.bundle;
            var abConfig = context.resConfig;
            var abConfigItem = abConfig.GetConfigItemByAbName(abFileName);
            var bundle = CBundle.Get(context, abConfigItem, false);
            if (bundle == null)
            {
                Debug.LogError("预加载失败:bundle=" + strBundleName);
                return null;
            }
            bundle.Retain(this);
            m_preLoadData.Add(bundle);
            return bundle;
        }

        public void PreLoadAsset(enResType resType, string strAssetName, Action<CAsset> finishFunc = null)
        {
            var asset = bundle.LoadAsync(AbTag.Global, resType, strAssetName, finishFunc);
            if (asset == null)
            {
                Debug.LogError("预加载失败:type=" + resType + "," + strAssetName);
                return;
            }
            asset.Retain(this);
            m_preLoadData.Add(asset);
        }


        public float GetPreLoadProgress()
        {
            if (m_preLoadData.Count == 0 || needDownloadNum > 0)
            {
                return 0;
            }

            //Debug.Log("m_preLoadData---:" + m_preLoadData.Count);

            float p = 0;
            foreach (var d in m_preLoadData)
            {
                p += d.Progress();
            }
            foreach (var d in BannerLoadList)
            {
                if (d.isDone)
                {
                    p += 1;
                }
            }
            p /= m_preLoadData.Count + BannerLoadList.Count + 1;

            return p;
        }

        public int AllSize = 0;
        public int GetAllSize()
        {
            if (AllSize > 0)
            {
                return AllSize;
            }
            if (m_preLoadData.Count == 0)
            {
                return 0;
            }
            int size = 0;

            foreach (var d in m_preLoadData)
            {
                size += d.GetAllSize();
            }
            size += BannerResAllSize;
            AllSize = size;
            return size;
        }

        public int GetCurSize()
        {
            if (m_preLoadData.Count == 0 || needDownloadNum > 0)
            {
                return 0;
            }
            int size = 0;

            foreach (var d in m_preLoadData)
            {
                size += d.GetCurSize();
            }
            foreach (var d in BannerLoadList)
            {
                if (d.IsNeedDownload())
                {
                    size += d.downloadedBytes;
                }
            }
            return size;
        }


        public bool IsPreLoadDone()
        {
            if (needDownloadNum > 0 || m_preLoadData.Count == 0)
            {
                return false;
            }
            hadLoaded = 0;
            bool isDone = true;
            foreach (var d in m_preLoadData)
            {
                if (!d.IsDone())
                {
                    isDone = false;
                    break;
                }
                d.DoneCallback();
                ++hadLoaded;
            }
            return isDone;
        }
        #endregion


        public void FixedUpdate()
        {
            bundle.FixedUpdate();
        }

        public Sprite GetAtlasSprite(string s)
        {
            var arr = s.Split('/');
            string strAtlasName = arr[0];
            string strSpriteName = arr[1];

            AbAtlas cacheAbAtlas;
            if (m_atlasDict.TryGetValue(strAtlasName, out cacheAbAtlas))
            {
                if (cacheAbAtlas != null)
                    return cacheAbAtlas[strSpriteName];
            }
            //string assetName = string.Concat("assets/UIAtlsaCache/", strAtlasName, "/", strSpriteName, ".png");
            //var asset = bundle.LoadImme(enResType.eAtlas, string.Concat("assets/bundle/atlas/", strAtlasName, ".prefab"));
            var asset = bundle.LoadImme(AbTag.Global, enResType.eAtlas, string.Concat("UI/Atlas/", strAtlasName));
            asset.Retain(this);
            asset.AddCall((_) =>
            {
                var atlas = _.resPrefab.GetComponent<AbAtlas>();
                if (atlas == null)
                {
                    LOG.Error("加载图集失败:" + strAtlasName);
                    return;
                }
                m_atlasDict.Add(strAtlasName, atlas);
            });

            if (m_atlasDict.TryGetValue(strAtlasName, out cacheAbAtlas))
            {
                if (cacheAbAtlas != null)
                    return cacheAbAtlas[strSpriteName];
            }
            return null;
        }

        public Sprite GetAtlasSprite(string strAtlasName, string strSpriteName)
        {
            AbAtlas cacheAbAtlas;
            if (m_atlasDict.TryGetValue(strAtlasName, out cacheAbAtlas))
            {
                if (cacheAbAtlas == null)
                {
                    LOG.Error("加载图集失败:" + strAtlasName);
                    return null;
                }
                return cacheAbAtlas[strSpriteName];
            }
            LOG.Error("图集未加载:" + strAtlasName);
            return null;
        }

        public void SetAtlasSprite(Image uiImg, string strAtlasName, string strSpriteName)
        {
            AbAtlas cacheAbAtlas;
            if (m_atlasDict.TryGetValue(strAtlasName, out cacheAbAtlas))
            {
                uiImg.sprite = cacheAbAtlas[strSpriteName];
                return;
            }
            //string assetName = string.Concat("assets/UIAtlsaCache/", strAtlasName, "/", strSpriteName, ".png");
            //var asset = bundle.LoadImme(enResType.eAtlas, string.Concat("assets/bundle/atlas/", strAtlasName, ".prefab"));
            var asset = bundle.LoadImme(AbTag.Global, enResType.eAtlas, string.Concat("UI/Atlas/", strAtlasName));
            asset.Retain(this);
            asset.AddCall((_) =>
            {
                var atlas = _.resPrefab.GetComponent<AbAtlas>();
                if (atlas == null)
                {
                    LOG.Error("加载图集失败:" + strAtlasName);
                    return;
                }
                m_atlasDict.Add(strAtlasName, atlas);
                uiImg.sprite = atlas[strSpriteName];
            });
        }

        public GameObject GetGameObject(string tag, string assetName)
        {
            var asset = bundle.LoadImme(tag, enResType.ePrefab, assetName);
            if (asset == null || asset.resPrefab == null)
            {
                LOG.Error("缓存中不存在资源:" + assetName);
                return null;
            }
            return asset.resPrefab;
        }


        public Object GetObject(string tag, string assetName)
        {
            var asset = bundle.LoadImme(tag, enResType.eObject, assetName);
            if (asset == null || asset.resObject == null)
            {
                LOG.Error("缓存中不存在资源:" + assetName);
                return null;
            }
            return asset.resObject;
        }


        public Sprite GetUITexture(string tag, string assetName)
        {
            var asset = bundle.LoadImme(tag, enResType.eSprite, assetName);
            if (asset == null || asset.resSprite == null)
            {
                //Debug.LogError("资源不存在:" + assetName);
                LOG.Error("资源不存在:" + assetName);
                return null;
            }
            return asset.resSprite;
        }


        public DownloadMgr.Task DownloadImage(string url, Action<string,UnityObjectRefCount> callback)
        {
            var task = DownloadMgr.Instance.DownloadImage(url);
            task.AddComplete(() =>
            {
                var sprite = XLuaHelper.LoadSprite(task.filename);
                UnityObjectRefCount refCount = SpriteRefCount.Create(sprite);
                callback(url,refCount);
            });
            return task;
        }
        
        private List<DownloadMgr.Task> BannerLoadList = new List<DownloadMgr.Task>();
        public bool BannerLoadOk = false;

        public int BannerResAllSize = 0;
        public void BannerLoadListClear()
        {
            BannerResAllSize = 0;
            BannerLoadList.Clear();
        }

        public void AddBannerLoadList(int bookId,Action<int,UnityObjectRefCount> callback = null)
        {
            var task = DownloadBanner(bookId,callback);
            BannerLoadList.Add(task);
            setSize();
        }


        private void setSize()
        {
            BannerResAllSize = 0;

            for (int i = 0; i < BannerLoadList.Count; i++)
            {
                BannerResAllSize += BannerLoadList[i].size;
            }
        }


        public DownloadMgr.Task DownloadBanner(int bookId, Action<int,UnityObjectRefCount> callback)
        {
            var task = DownloadMgr.Instance.DownloadBanner(bookId);
            task.AddComplete(() =>
            {
                var sprite = XLuaHelper.LoadSprite(task.filename);
                UnityObjectRefCount refCount = SpriteRefCount.Create(sprite);
                callback(bookId,refCount);
            });
            return task;
        }
        public DownloadMgr.Task DownloadChapterBG(int bookId, Action<int, UnityObjectRefCount> callback)
        {
            var task = DownloadMgr.Instance.DownloadChapterBG(bookId);
            task.AddComplete(() =>
            {
                var sprite = XLuaHelper.LoadSprite(task.filename);
                UnityObjectRefCount refCount = SpriteRefCount.Create(sprite);
                callback(bookId, refCount);
            });
            return task;
        }


        public DownloadMgr.Task DownloadBookCover(int bookId, Action<int, UnityObjectRefCount> callback)
        {
            var task = DownloadMgr.Instance.DownloadBookCover(bookId);
            task.AddComplete(() =>
            {
                var sprite = XLuaHelper.LoadSprite(task.filename);
                UnityObjectRefCount refCount = SpriteRefCount.Create(sprite);
                callback(bookId, refCount);
            });
            return task;
        }
        public DownloadMgr.Task DownloadBookSceneBG(int bookId, string name, Action<int, UnityObjectRefCount> callback)
        {
            var task = DownloadMgr.Instance.DownloadBookSceneBG(bookId,name);
            task.AddComplete(() =>
            {
                var sprite = XLuaHelper.LoadSprite(task.filename);
                UnityObjectRefCount refCount = SpriteRefCount.Create(sprite);
                callback(bookId, refCount);
            });
            return task;
        }
    }
}
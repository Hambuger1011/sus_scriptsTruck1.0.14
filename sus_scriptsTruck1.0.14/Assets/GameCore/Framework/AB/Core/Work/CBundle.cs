namespace AB
{
    using UnityEngine;
    using System.Collections;
    using System;
    using System.Collections.Generic;
    using UnityEngine.Profiling;
    using Object = UnityEngine.Object;
    using UnityEngine.Networking;
    using System.Diagnostics;
    using Debug = UnityEngine.Debug;
    using GameCore;

    public abstract class CBundle : AbTask
    {
        public enum enLoadtype
        {
            eFile = 0,
            eByte,
            eWeb,
        }
        public static enLoadtype loadType = enLoadtype.eFile;
        #region base
        public bool IsLoadImme { get; protected set; }
        public AssetBundle assetbundle { get; protected set; }

        public List<CBundle> dependencyList = new List<CBundle>();


        public override string ToString()
        {
            return string.Format("({0:F2})[{1}]{2},async = {3}", this.Progress() * 100, "B", this.abConfigItem.filename, this.IsLoadImme);
        }

        protected override void Clear()
        {
            base.Clear();
            onDoneCallback = null;
            IsLoadImme = false;
            assetbundle = null;
            dependencyList.Clear();
            mReferenceSet.Clear();
        }


        #region 回调
        /// <summary>
        /// 完成回调
        /// </summary>
        public event Action<CBundle> onDoneCallback;
        public void AddCall(Action<CBundle> callback)
        {
            this.onDoneCallback += callback;
            if (this.mIsDone)
            {
                DoneCallback();
            }
        }

        public override void DoneCallback()
        {
            if (onDoneCallback != null)
            {
                var callback = onDoneCallback;
                onDoneCallback = null;
                callback(this);
            }
        }

        #endregion

        #region 引用计数
        HashSet<object> mReferenceSet = new HashSet<object>();

        public object[] GetReferenceSet()
        {
            object[] set = new object[mReferenceSet.Count];
            mReferenceSet.CopyTo(set);
            return set;
        }

        public int retainCount
        {
            get
            {
                return mReferenceSet.Count;
            }
        }

        public override void Release(object referencerHashCode = null, bool bRecursively = true)
        {
            mReferenceSet.Remove(referencerHashCode);

            if (this.woker == null)
            {
                return;
            }

            if (mReferenceSet.Count <= 0)
            {
                if (bRecursively)
                {
                    foreach (var data in this.dependencyList)
                    {
                        data.Release(this.abConfigItem.filename, false);
                    }
                }
                if (abConfigItem != null)
                {
                    //if(this.abConfigItem.name.Equals("assets/bundle/prefabs/ui/canvas_drive.prefab.ab"))
                    //{
                    //    LOG.Error("11111111111111");
                    //}
                    CheckRelease();
                }
            }
        }

        public void ClearRef(bool bRecursively = true)
        {
            if (bRecursively)
            {
                foreach (var data in this.dependencyList)
                {
                    data.ClearRef(false);
                }
            }

            mReferenceSet.Clear();
            CheckRelease();
        }

        private void CheckRelease()
        {
            if (!CanRelease())
            {
                return;
            }
            this.woker.removeBundleKeys.Add(this.abConfigItem);
        }

        public override void Retain(object referencerHashCode)
        {
            //if (referenceSet.Contains(referencerHashCode))
            //{
            //    return;
            //}
            CheckRefObj(referencerHashCode);
            mReferenceSet.Add(referencerHashCode);
        }


        [Conditional("UNITY_EDITOR")]
        void CheckRefObj(object refObj)
        {
            if (object.ReferenceEquals(refObj, this))
            {
                throw new Exception(string.Format("{0}不允许引用自己:{1}", this.abConfigItem.filename, this));
            }
        }
        #endregion


        public static CBundle Get(AbWork context, AbResItem abConfigItem, bool isLoadImme, bool isDependency = false)
        {
            if (abConfigItem == null)
            {
                return null;
            }
            CBundle data;
            if (!AbWork.bundleCache.TryGetValue(abConfigItem.fileHashName, out data))
            {
                if (isLoadImme)
                {
                    if (AbUtility.loadType == enLoadType.eFile)
                    {
                        data = new CBundleImmediate(context, context.resConfig, abConfigItem, isDependency);
                    }
                    else
                    {
                        throw new Exception("Web AssetBundle不支持即时加载,缺少预加载资源:" + abConfigItem.filename);
                    }
                }
                else
                {
                    data = new CBundleAsync(context, context.resConfig, abConfigItem, isDependency);
                }
            }
            return data;
        }


        public CBundle(AbWork context, AbResConfig abConfig, AbResItem abConfigItem, bool isLoadImme, bool isEncrypt = false)
        {
            this.IsLoadImme = isLoadImme;
            this.woker = context;
            this.abConfig = abConfig;
            this.abConfigItem = abConfigItem;
            AbWork.bundleCache.Add(abConfigItem.fileHashName, this);
            this.loadState = enAbLoadState.eNonLoad;
        }

        /// <summary>
        /// 加载依赖
        /// </summary>
        public void LoadDependency(AbWork context, AbResConfig abConfig, AbResItem abConfigItem, bool isLoadImme, bool isDependency)
        {
            //加载所有依赖
            var needLoadDependencyNum = abConfigItem.dependencyList.Length;
            for (int i = 0, iMax = needLoadDependencyNum; i < iMax; ++i)
            {
                string depAssetName = abConfigItem.dependencyList[i];
                var depConfigItem = abConfig.GetConfigItemByAbName(depAssetName);
                //LoadDependencyImme(depConfigItem, isLoadImme);
                CBundle data = Get(context, depConfigItem, isLoadImme, true);
                if (!isDependency)
                {
                    data.Retain(this.abConfigItem.filename);
                }
                dependencyList.Add(data);
            }
        }


        public void Unload(bool unloadAllLoadedObjects)
        {
            if (assetbundle != null)
            {
                //LOG.Error("释放bundle:" + abConfigItem.name);
                assetbundle.Unload(unloadAllLoadedObjects);
                assetbundle = null;

            }
            else
            {
                if (this.mIsDone)
                {
                    LOG.Error("ab已经释放过:" + abConfigItem.filename);
                }
                else if(!this.IsLoadImme)
                {
                    throw new Exception("未加载完释放:" + abConfigItem.filename);
                }
            }
        }


        #endregion

        protected void OnLoadFinish()
        {
            if (this.assetbundle == null)
            {
                LOG.Error("加载bundle失败:(" + strError + ")\n" + abConfigItem.abFilePath + "|" + abConfigItem.filename);
                this.mIsDone = true;
            }
            CheckRelease();
        }

        public override bool IsLoaded()
        {
            if (this.mIsDone)
            {
                return true;
            }
            return this.assetbundle != null;
        }

        public override bool IsDone()
        {
            if (this.IsDispose || this.mIsDone)
            {
                return true;
            }

            if (!this.IsLoaded())
            {
                return false;
            }

            foreach (var task in this.dependencyList)
            {
                if (!task.IsLoaded())
                {
                    return false;
                }
            }
            this.mIsDone = true;
            return mIsDone;
        }
        public bool CanRelease()
        {
            if (this.IsDispose)
            {
                return true;
            }
            if (this.retainCount > 0)
            {
                return false;
            }
            if (!this.IsLoadImme)
            {
                return this.IsDone();
            }
            return true;
        }

        public override int GetAllSize()
        {
            return 0;
        }

        public override int GetCurSize()
        {
            return 0;
        }

    }




    public class CBundleImmediate : CBundle
    {
        private AbResItem _abConfigItem = null;
        public CBundleImmediate(AbWork context, AbResConfig abConfig, AbResItem abConfigItem, bool isDependency) : base(context, context.resConfig, abConfigItem,true)
        {
            _abConfigItem = abConfigItem;
#if ENABLE_DEBUG
            int s = System.Environment.TickCount;
#endif
            LoadDependency(this.woker, abConfig, abConfigItem, true, isDependency);//加载依赖
            this.assetbundle = AssetBundle.LoadFromFile(abConfigItem.abFilePath);
            OnLoadFinish();

#if ENABLE_DEBUG
            int e = System.Environment.TickCount - s;
            if (e > 500)
            {
                LOG.Error("加载ab文件时间过长:" + e + "ms,filename=" + abConfigItem.filename);
            }
#endif
        }
        protected override IEnumerator DoUpdate()
        {
            yield break;
        }



        public override float Progress(bool bRecursively = true)
        {
            if (this.mIsDone)
            {
                return 1;
            }
            return 0;
        }

        public override int GetAllSize()
        {
            int size = 0;
            if (_abConfigItem != null)
            {
                size = _abConfigItem.size;
            }
            return size;
        }

        public override int GetCurSize()
        {
            if (this.mIsDone)
            {
                return GetAllSize();
            }
            return 0;
        }

    }


    public class CBundleAsync : CBundle
    {

        public ABLoader loader;
        public CBundleAsync(AbWork context, AbResConfig abConfig, AbResItem abConfigItem, bool isDependency) : base(context, context.resConfig, abConfigItem,false)
        {
            ABMgr.Instance.asyncTaskList.Add(this);
            LoadDependency(this.woker, abConfig, abConfigItem, false, isDependency);//加载依赖

            if (AbUtility.loadType == enLoadType.eFile)
            {
                this.loader = new ABLoader_File(abConfigItem);
            }
            else if (AbUtility.loadType == enLoadType.eWebClient)
            {
                this.loader = new ABLoader_WebClient(context, abConfigItem);
            }
            else if (AbUtility.loadType == enLoadType.eWebUnity)
            {
                this.loader = new ABLoader_WebUnity(abConfigItem);
            }
        }

        public override float Progress(bool bRecursively = true)
        {
            if (this.mIsDone)
            {
                return 1;
            }
            else if (loader == null)
            {
                return 0;
            }
            else
            {
                float p = loader.GetProgress();
                if (bRecursively)
                {
                    foreach (AbTask data in dependencyList)
                    {
                        p += data.Progress(false);
                    }
                    p /= (dependencyList.Count + 1);
                }
                return p;
            }
        }

        public override int GetAllSize()
        {
            int size = 0;
            if (loader != null)
            {
                size = loader.GetAllSize();
            }
            return size;
        }

        public override int GetCurSize()
        {
            int size = 0;
            if (loader != null)
            {
                size = loader.GetCurSize();
            }
            return size;
        }

        protected override IEnumerator DoUpdate()
        {
            while (CBundle.cur_bundle_async_num > CBundle.MAX_BUNDLE_ASYNC_NUM)
            {
                yield return null;
            }
            CBundle.cur_bundle_async_num += 1;
            try
            {
                this.loadState = enAbLoadState.eLoading;
                var itr = this.loader.DoUpdate();
                while (itr.MoveNext())
                {
                    yield return null;
                }

                string error = null;
                assetbundle = this.loader.GetAssetBundle(ref error);
                this.strError = error;
                this.loader = null;
                OnLoadFinish();
            }
            finally
            {
                CBundle.cur_bundle_async_num -= 1;
            }
        }

    }
}




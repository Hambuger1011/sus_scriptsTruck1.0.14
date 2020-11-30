namespace AB
{
    using Object = UnityEngine.Object;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Diagnostics;
    using Debug = UnityEngine.Debug;
    using XLua;

    public abstract class CAsset : AbTask
    {
        #region base

        protected override void Clear()
        {
            base.Clear();
            loader = null;
            key = null;
            mIsDisposed = false;
            isAbRes = false;
            assetName = null;
            bundle = null;
            mReferenceSet.Clear();
            onDoneCallback = null;

            #region asset obj
            resObject = null;
            #endregion
        }

        public override bool IsLoaded()
        {
            return this.resObject != null;
        }

        public override bool IsDone()
        {
            if (this.IsDispose || this.mIsDone)
            {
                return true;
            }
            if (strError != null)
            {
                return true;
            }
            if (!this.IsLoaded())
            {
                return false;
            }
            return mIsDone;
        }

        #endregion

        public override int GetAllSize()
        {
            int size = 0;
            if (this.abConfigItem != null)
            {
                size = this.abConfigItem.size;
            }
            return size;
        }



        public enResType m_resType { get; protected set; }
        #region asset object
        public Object resObject { get; protected set; }
        public GameObject resPrefab { get { return resObject as GameObject; } }
        public AudioClip resAudioClip { get { return resObject as AudioClip; } }
        public Shader resShader { get { return resObject as Shader; } }
        public Font resFont { get { return resObject as Font; } }
        public Material resMaterial { get { return resObject as Material; } }
        public TextAsset resTextAsset { get { return resObject as TextAsset; } }
        public Texture2D resTexture2D { get { return resObject as Texture2D; } }
        public Sprite resSprite { get { return resObject as Sprite; } }
        public ScriptableObject resScriptableObject { get { return resObject as ScriptableObject; } }
        #endregion

        public bool IsLoadImme { get; private set; }
        bool mIsDisposed = false;
        public bool isAbRes;
        public string assetName { get; protected set; }
        public CBundle bundle { get; protected set; }

        public string key;
        protected AssetLoader loader;

        public override string ToString()
        {
            return string.Format("({0:F2})[{1}]{2},async = {3}", this.Progress() * 100, "A", key,this.IsLoadImme);
        }

        protected CAsset(string key, enResType resType, string assetName, AbWork context, AbResConfig abConfig, AbResItem abConfigItem, bool isAbRes, bool isLoadImme)
        {
            this.key = key;
            this.woker = context;
            this.m_resType = resType;
            this.IsLoadImme = isLoadImme;
            this.abConfig = abConfig;
            this.abConfigItem = abConfigItem;
            this.assetName = assetName;
            this.isAbRes = isAbRes;
            Add2AssetMap();

            if (!isAbRes)
            {
                this.loader = new AssetLoader_Resources(this);
            }
            else if (ABMgr.Instance.isUseAssetBundle)
            {
                this.loader = new AssetLoader_AB(this);
            }
            else
            {
                this.loader = new AssetLoader_Editor(this);
            }
            //LOG.Info("load asset:" + assetName + " isLoadImme" + isLoadImme + " " + bundle);
        }

        public void Dispose()
        {
            if (mIsDisposed)
            {
                return;
            }
            mIsDisposed = true;


            if (!this.woker.assetCache.Remove(this.key))
            {
                LOG.Error("重复释放asset:" + assetName);
            }

            //if (!this.isAbRes)
            //{
            //    Resources.UnloadAsset(this.resObject);
            //}
        }


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

        public void ClearRef(bool bRecursively = true)
        {
            if (bundle != null)
            {
                //LOG.Error("release asset:" + assetName + " bundle:" + bundle.abConfigItem.name);
                bundle.ClearRef(bRecursively);
            }
            mReferenceSet.Clear();
            this.woker.removeAssetKeys.Add(this.key);
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
                this.woker.removeAssetKeys.Add(this.key);

                if (bundle != null)
                {
                    //LOG.Error("release asset:" + assetName + " bundle:" + bundle.abConfigItem.name);
                    bundle.Release(this.key);
                }
            }
        }

        public override void Retain(object referencerHashCode)
        {
            //if (referenceSet.Contains(referencerHashCode))
            //{
            //    return;
            //}
            CheckRefObj(referencerHashCode);
            mReferenceSet.Add(referencerHashCode);
            //if (referencerHashCode is enAssetTag && (enAssetTag)referencerHashCode == enAssetTag.eLobbyScene)
            //{
            //    LOG.Error("eLobbyScene:" + this);
            //}
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
        

        protected void OnLoadFinish()
        {
            if (this.m_resType != enResType.eScene && this.resObject == null)
            {
                LOG.Error("加载资源失败:"+this.strError
                    +"\n[" + this.assetName + "] type:" + this.m_resType + " isAB:" + this.isAbRes+" isAsync:"+ !this.IsLoadImme 
                    + "\nbundle:" + this.bundle);
            }
            this.mIsDone = true;
            this.DoneCallback();
        }

        bool Set<T>(ref T refObj, Object val) where T : Object
        {
            refObj = val as T;
            return refObj != null;
        }

        protected void Add2AssetMap()
        {
            string key = null;
            using (CString.Block())
            {
                CString sb = CString.Alloc(256);
                sb.Append("_");
                sb.Insert(0, this.m_resType);
                sb.Append(this.assetName);
                key = sb.ToString();
            }
            if (GameUtility.isDebugMode && this.woker.assetCache.ContainsKey(key))
            {
                throw new Exception("资源重复:" + key);
            }
            else
            {
                this.woker.assetCache.Add(key, this);
            }
        }
        
        public GameObject Instantiate()
        {
            var go = GameObject.Instantiate(resPrefab);
            return go;
        }


        public GameObject Instantiate(Vector3 pos, Quaternion identity)
        {
            var go = GameObject.Instantiate(resPrefab, pos, identity);
            return go;
        }


        /// <summary>
        /// 完成回调
        /// </summary>
        public event Action<CAsset> onDoneCallback;
        public void AddCall(Action<CAsset> callback)
        {
            if(callback == null)
            {
                return;
            }

            if (this.IsDone())
            {
                callback(this);
            }
            else
            {
                this.onDoneCallback += callback;
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

        public T Get<T>() where T : Component
        {
            return resPrefab.GetComponent<T>();
        }

        public T Cast<T>() where T : Object
        {
            return (T)this.resObject;
        }
    }


    public class CAssetImmediate : CAsset
    {
        public CAssetImmediate(string key, enResType resType, string assetName, AbWork context, AbResConfig abConfig, AbResItem abConfigItem, bool isAbRes)
            :base(key, resType, assetName, context, abConfig, abConfigItem, isAbRes, true)
        {
            if (isAbRes && ABMgr.Instance.isUseAssetBundle)
            {
                bundle = CBundle.Get(context, abConfigItem, true);
                if (bundle == null)
                {
                    LOG.Error("获取bundle失败：" + abConfigItem.filename);
                }
                bundle.Retain(this.key);
            }

            Object obj = null;
            if (this.loader.Load(resType, assetName, false, ref obj))
            {
                this.loader = null;
                this.resObject = obj;
            }
            OnLoadFinish();
        }

        public override int GetAllSize()
        {
            return 0;
        }

        public override int GetCurSize()
        {
            return 0;
        }

        public override float Progress(bool bRecursively = true)
        {
            if (strError != null)
            {
                return 1;
            }
            if (resObject != null)
            {
                return 1;
            }
            return 0;
        }

        protected override IEnumerator DoUpdate()
        {
            yield break;
        }
    }

    public class CAssetAsync : CAsset
    {

        public CAssetAsync(string key, enResType resType, string assetName, AbWork context, AbResConfig abConfig, AbResItem abConfigItem, bool isAB)
            : base(key, resType, assetName, context, abConfig, abConfigItem, isAB, false)
        {
            ABMgr.Instance.asyncTaskList.Add(this);
            if (isAB && ABMgr.Instance.isUseAssetBundle)
            {
                bundle = CBundle.Get(context, abConfigItem, false);
                if (bundle == null)
                {
                    LOG.Error("获取bundle失败：" + abConfigItem.filename);
                }
                bundle.Retain(this.key);
            }
        }

        public override float Progress(bool bRecursively = true)
        {
            if (IsDone())
            {
                return 1;
            }
            else if (loader == null)
            {
                return 0;
            }

            float p = 0;
            if (this.bundle != null)
            {
                p = (loader.GetProgress() + this.bundle.Progress()) * 0.5f;
            }
            else
            {
                p = loader.GetProgress();
            }
            return p;
        }

        /// <summary>
        /// 获取总大小
        /// </summary>
        /// <returns></returns>
        public override int GetAllSize()
        {
            int size = 0;
            if (this.bundle != null)
            {
                size = this.bundle.GetAllSize();
            }
            else if (this.loader != null)
            {
                size = loader.GetAllSize();
            }
            return size;
        }


        /// <summary>
        /// 获取当前大小
        /// </summary>
        /// <returns></returns>
        public override int GetCurSize()
        {
            int size = 0;
            if (this.bundle != null)
            {
                size = this.bundle.GetCurSize();
            }
            else if (this.loader != null)
            {
                size = loader.GetCurSize();
            }
            return size;
        }


        protected override IEnumerator DoUpdate()
        {
            if (this.isAbRes && ABMgr.Instance.isUseAssetBundle)
            {
                while (!this.bundle.IsDone())
                {
                    yield return null;
                }
            }
            Object obj = null;
            if (this.loader.Load(m_resType, assetName, true, ref obj))
            {
                this.loader = null;
                this.resObject = obj;
                OnLoadFinish();
            }
            yield break;
        }
    }
}
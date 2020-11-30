namespace AB
{
    using UnityEngine;
    using System.Collections;
    using System;
    using System.IO;
    using System.Collections.Generic;
    using UnityEngine.Profiling;

    using Object = UnityEngine.Object;

    [System.Serializable]
    public abstract class AbTask
    {
        public static int MAX_BUNDLE_ASYNC_NUM = 1;
        public static int cur_bundle_async_num = 0;

        public static int MAX_ASSET_ASYNC_NUM = 1;
        public static int cur_asset_async_num = 0;


        [System.NonSerialized]
        public AbWork woker;

        protected bool mIsDone = false;
        
        public AbResConfig abConfig
        {
            get;
            protected set;
        }

        public AbResItem abConfigItem { get; protected set; }
        public bool IsDispose { get; protected set; }

        public string strError { get; set; }

        public enAbLoadState loadState { get; protected set; }


        abstract public bool IsLoaded();
        abstract public bool IsDone();

        abstract public float Progress(bool bRecursively = true);
        abstract public int GetAllSize();
        abstract public int GetCurSize();
        abstract public void Retain(object obj);
        abstract public void Release(object obj, bool bRecursively = true);
        abstract protected IEnumerator DoUpdate();

        IEnumerator _doUpdate;
        public bool Update()
        {
            if(_doUpdate == null)
            {
                return false;
            }
            if (!_doUpdate.MoveNext())
            {
                _doUpdate = null;
                return false;
            }
            return true;
        }


        abstract public void DoneCallback();
        
        protected virtual void Clear()
        {
            strError = null;
            mIsDone = false;
            abConfigItem = null;
            woker = null;
            loadState = enAbLoadState.eNonLoad;
        }

        public AbTask()
        {
            this._doUpdate = DoUpdate();
        }
    }



    public enum enAbLoadState
    {
        eNonLoad = 0,
        eLoading,
        eLoadSuc,
        eLoadFail,
    }
}
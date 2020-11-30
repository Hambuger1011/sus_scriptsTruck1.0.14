namespace AB
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using UnityEngine;
    using Debug = UnityEngine.Debug;

    public delegate void OnLoadException(AbTask data, Exception ex);
    public abstract class ABMgr : MonoBehaviour
    {
        Stopwatch sw = new Stopwatch();
        public static ABMgr Instance { get; private set; }

        [Header("是否bundle模式")]
        public bool isUseAssetBundle = false;
        public bool isDebug = false;
        /// <summary>
        /// 加载列表
        /// </summary>
        public List<AbTask> asyncTaskList = new List<AbTask>(512);
        //protected Dictionary<string, Shader> shaders = new Dictionary<string, Shader>(128);
        public Dictionary<string, HashSet<CAsset>> assetTagMap = new Dictionary<string, HashSet<CAsset>>(512);
        public static string const_extension = ".ab";
        public event OnLoadException onLoadException;

        protected virtual void Awake()
        {
            Instance = this;
            if (!GameUtility.isEditorMode)
            {
                isUseAssetBundle = true;
            }
            RefCountMgr.CreateInstance();
            Init();
        }

        protected virtual void OnDestroy()
        {
            RefCountMgr.DestroyInstance();
        }

        public abstract void Init();

        int i = 0;
        protected virtual void Update()
        {
            sw.Reset();
            sw.Start();
            AbTask data = null;
            try
            {
                for (i = (i >= asyncTaskList.Count ? 0 : i); i < asyncTaskList.Count; ++i)
                {
                    data = asyncTaskList[i];
                    if (data.IsDispose)
                    {
#if ENABLE_DEBUG
                        Debug.LogError("资源未加载完被释放:" + data);
#endif
                        asyncTaskList.RemoveAt(i);
                        --i;
                        continue;
                    }
                    if (!data.Update())
                    {
                        asyncTaskList.RemoveAt(i);
                        data.DoneCallback();
                    }
                    if (sw.ElapsedMilliseconds > 1000)
                    {
                        LOG.Error("加载时间过长:index=" + i + " use time=" + sw.ElapsedMilliseconds + "ms,op = " + data);
                        return;
                    }
                }
                sw.Stop();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                if (onLoadException != null)
                {
                    onLoadException(data, ex);
                }
            }
        }


        protected virtual void FixedUpdate()
        {
            RefCountMgr.Instance.Update();
            //ProcessGC();
        }


        [ContextMenu("GC")]
        public abstract void GC();


        public void ClearAssetTag(string tag)
        {
            HashSet<CAsset> assetSet;
            if (assetTagMap.TryGetValue(tag, out assetSet))
            {
                using (var itr = assetSet.GetEnumerator())
                {
                    while (itr.MoveNext())
                    {
                        if (itr.Current != null)
                        {
                            itr.Current.Release(tag);
                        }
                        else
                        {
                            LOG.Error("CAsset为空:" + tag);
                        }
                    }
                }
                assetTagMap.Remove(tag);
            }
        }

        public void AddAssetTag(string tag, CAsset asset)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return;
            }
            HashSet<CAsset> assetSet;
            if (!assetTagMap.TryGetValue(tag, out assetSet))
            {
                assetSet = new HashSet<CAsset>();
                assetTagMap.Add(tag, assetSet);
            }

            if (assetSet.Add(asset))
            {
                asset.Retain(tag);
            }
        }

        public void RemoveAssetTag(string tag, CAsset asset)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return;
            }
            HashSet<CAsset> assetSet;
            if (assetTagMap.TryGetValue(tag, out assetSet))
            {
                if (assetSet.Remove(asset))
                {
                    asset.Release(tag);
                }
            }
        }

        #region Debug
        [Conditional("UNITY_EDITOR")]
        protected virtual void EditorAssertLoadAb(string assetName)
        {
            if (assetName.Contains("\\"))
            {
                throw new Exception("asset name路径分隔符必须为[/]");
            }
        }
        

        public abstract UnityEngine.Object LoadAssetInEditor(string assetName, Type type);
        #endregion

        public static Shader FindShader(string name)
        {
            Shader shader = shader = Shader.Find(name);
            return shader;
        }

        public abstract CAsset LoadAsync(string refTag, enResType resType, string strAssetName, Action<CAsset> finishFunc = null, bool isAbRes = true);
        public abstract CAsset LoadImme(string refTag, enResType resType, string strAssetName, bool isAbRes = true);



        

    }
}
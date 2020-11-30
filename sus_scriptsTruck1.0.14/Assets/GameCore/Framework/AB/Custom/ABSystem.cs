
using Framework;

namespace AB
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using UnityEngine;
    using Debug = UnityEngine.Debug;
    using Object = UnityEngine.Object;
    using XLua;
    using System.Net;

    public class ABSystem : ABMgr
    {
        float lastProcessTime = 0;
#if UNITY_EDITOR
        [Header("========================================")]
        [Header("加载中的ab")]
        public List<string> asyncBundleList = new List<string>();
        [Header("加载中的Asset")]
        public List<string> asyncAssetList = new List<string>();

        //[Header("bundle数量")]
        //public int bundleCount = 0;

        //[Header("asset数量")]
        //public int assetCount = 0;

        //[Header("----------------------------------------")]
        //[Header("cbundle数量")]
        //public int cbundleCount = 0;

        //[Header("casset数量")]
        //public int cassetCount = 0;

        [Header("加载数量")]
        public int loadCount = 0;
        //[Header("========================================")]
#endif
        public static AbUISystem ui = new AbUISystem();

        public static new ABSystem Instance
        {
            get; private set;
        }

        protected override void Awake()
        {
            base.Awake();
            Instance = this;
#if UNITY_EDITOR
            //AbUtility.buildTarget = UnityEditor.EditorUserBuildSettings.activeBuildTarget;
            //LOG.Info("当前平台:" + AbUtility.buildTarget + " isUseAssetBundle = " + isUseAssetBundle);
#endif
        }

        public override void Init()
        {
            if (this.isUseAssetBundle)
            {
                //var num = SystemInfo.processorCount;
                //num = Mathf.Clamp(num * 1, 1, 4);
                //num = 128;

                var level = GameFrameworkImpl.Instance.level;

                if (level == PhoneUtil.EnumPhoneLevel.High)
                {
                    //高端机
                    AbTask.MAX_BUNDLE_ASYNC_NUM = 4;
                }
                else if (level == PhoneUtil.EnumPhoneLevel.Medium)
                {
                    //中端机
                    AbTask.MAX_BUNDLE_ASYNC_NUM = 2;
                }
                else
                {
                    //低端机
                    AbTask.MAX_BUNDLE_ASYNC_NUM = 1;
                }
              
                AbTask.MAX_ASSET_ASYNC_NUM = 1;

                UnityEngine.Debug.LogError("cpu核数:" + SystemInfo.processorCount + "---,--->"+AbTask.MAX_BUNDLE_ASYNC_NUM);
            }
;
            //HttpWebRequest.DefaultMaximumResponseHeadersLength = 1024 * 1024 * 8;
            //HttpWebRequest.DefaultMaximumErrorResponseLength = 1;
            //HttpWebRequest.DefaultCachePolicy = null;
            //HttpWebRequest.DefaultWebProxy = null;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                return true;
            };
        }

        protected override void Update()
        {
            //if (Time.frameCount % 6 == 0)
            //{
            //    return;
            //}
#if UNITY_EDITOR
            //assetCount = assetCache.Count;
            //bundleCount = bundleCache.Count;

            //cbundleCount = ClassPool<CBundle>.GetPoolCount();
            //cassetCount = ClassPool<CAsset>.GetPoolCount();
            loadCount = asyncTaskList.Count;
#endif
            base.Update();
#if UNITY_EDITOR

            asyncBundleList.Clear();
            asyncAssetList.Clear();
            AbTask data = null;
            for (int i = 0; i < asyncTaskList.Count; ++i)
            {
                data = asyncTaskList[i];
                if (data.loadState == enAbLoadState.eLoading)
                {
                    if (data is CBundle)
                    {
                        asyncBundleList.Add(data.ToString());
                    }
                    else
                    {
                        asyncAssetList.Add(data.ToString());
                    }
                }
            }
#endif
        }

        protected override void FixedUpdate()
        {
            if (Time.realtimeSinceStartup - lastProcessTime < 5)
            {
                return;
            }
            lastProcessTime = Time.realtimeSinceStartup;
            base.FixedUpdate();
            ui.FixedUpdate();
            if(AbBookSystem.Instance != null)
            {
                AbBookSystem.Instance.FixedUpdate();
            }
        }

        [DoNotGen]
        public override Object LoadAssetInEditor(string assetName, Type type)
        {
            Object obj = null;
#if UNITY_EDITOR
            obj = UnityEditor.AssetDatabase.LoadAssetAtPath(assetName, type);
#endif
            return obj;
        }

        public override CAsset LoadAsync(string refTag, enResType resType, string strAssetName, Action<CAsset> finishFunc = null, bool isAbRes = true)
        {
            return ui.bundle.LoadAsync(refTag, resType, strAssetName, finishFunc, isAbRes);
        }

        public override CAsset LoadImme(string refTag, enResType resType, string strAssetName, bool isAbRes = true)
        {
            return ui.bundle.LoadImme(refTag, resType, strAssetName, isAbRes);
        }

        public override void GC()
        {
            lastProcessTime = 0;
            this.FixedUpdate();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            //System.GC.WaitForPendingFinalizers();
        }
    }
}
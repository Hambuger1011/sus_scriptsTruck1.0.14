using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Framework;
using System.IO;
using AB;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UniGameLib
{
    public class UnitySceneMgr : Framework.CSingleton<UnitySceneMgr>
    {
        public const string LaunchScene = "LaunchScene";
        public const string EmptyScene = "EmptyScene";
        public const string EmptySceneWithCamera = "EmptySceneWithCamera";
        //public const string StartScene = "startscene";

        public const string SplashScene = "assets/Z_Unity/bundle/Scenes/SplashScene.unity";
        public const string LoginScene = "assets/Z_Unity/bundle/Scenes/LoginScene.unity";
        public const string LobbyScene = "assets/Z_Unity/bundle/Scenes/LobbyScene.unity";
        public const string Battle5v5Scene = "assets/Z_Unity/bundle/Scenes/5v5.unity";
        public const string S03Map02 = "assets/z_s03/scenes/sc_map02.unity";

        event UnityAction<Scene, Scene> activeSceneChanged;
        event UnityAction<Scene, LoadSceneMode> sceneLoaded;
        event UnityAction<Scene> sceneUnloaded;

        SceneLoader mLoader = new SceneLoader();

        public SceneLoader loader
        {
            get
            {
                return mLoader;
            }
        }

        protected override void Init()
        {
            base.Init();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        protected override void UnInit()
        {
            base.UnInit();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            LOG.Info("加载场景成功:" + scene.name+ " sceneLoaded=" + (sceneLoaded == null?"null":sceneLoaded.ToString()));
            //loader.Reset();
            if (sceneLoaded != null)
            {
                var callback = sceneLoaded;
                sceneLoaded = null;
                callback(scene, mode);
            }
        }

        #region 非bundle加载
        public void LoadSceneImme(string sceneName, UnityAction<Scene, LoadSceneMode> finishDelegate = null)
        {
            LOG.Info("load scene:" + sceneName);
            if (finishDelegate != null)
            {
                sceneLoaded += finishDelegate;
            }
            SceneManager.LoadScene(sceneName);
        }

        public void LoadSceneSync(string sceneName, UnityAction<Scene, LoadSceneMode> finishDelegate = null)
        {
            LOG.Info("load scene:" + sceneName);
            if (finishDelegate != null)
            {
                sceneLoaded += finishDelegate;
            }
            mLoader.Load(string.Empty, sceneName);
        }
        #endregion

#region bundle加载
        public void LoadBundleSceneSync(string tag, string sceneName, UnityAction<Scene, LoadSceneMode> finishDelegate = null)
        {
            //LOG.Info("scene count:" + SceneManager.GetAllScenes().Length);
            if (finishDelegate != null)
            {
                sceneLoaded += finishDelegate;
            }
            CBundle bundle = null;
            if (ABMgr.Instance.isUseAssetBundle)
            {
                //var abConfigItem = ABConfiguration.Instance.GetConfigItemByAssetName(sceneName.JavaHashCodeIgnoreCase());
                //bundle = CBundle.Get(abConfigItem,false);
                //bundle.Retain(tag);
            }
            mLoader.Load(tag, sceneName, bundle);
        }

        CBundle m_lastSceneBundle = null;
        public void LoadBundleSceneImme(string tag, string sceneName, UnityAction<Scene, LoadSceneMode> finishDelegate = null)
        {
            //LOG.Info("scene count:" + SceneManager.GetAllScenes().Length);
#if UNITY_EDITOR
            if (!ABMgr.Instance.isUseAssetBundle)
            {
                if (finishDelegate != null)
                {
                    sceneLoaded += finishDelegate;
                }
                EditorApplication.LoadLevelInPlayMode(sceneName);
            }else
#endif
            {
                //if(m_lastSceneBundle != null)
                //{
                //    ABMgr.Instance.removeBundleKeys.Add(m_lastSceneBundle.abConfigItem);
                //}
                //AbConfigItem abConfigItem = ABConfiguration.Instance.GetConfigItemByAssetName(sceneName.JavaHashCodeIgnoreCase());
                //var bundle = CBundle.Get(abConfigItem);
                //bundle.Retain(tag);
                //m_lastSceneBundle = bundle;
                //ABManager.Instance.LoadImme(tag, sceneName);
#if UNITY_STANDLONA
                LoadSceneImme(/*Path.GetFileNameWithoutExtension(sceneName)*/bundle.assetbundle.name, finishDelegate);
#else
                //LoadSceneImme(abConfigItem.strSceneName, finishDelegate);
#endif
            }
        }
#endregion
    }



    public class SceneLoader
    {
        bool mIsLoadAbFinish = false;
        AsyncOperation mAsync;//异步对象
        int nCurProgress;
        public string levelName;
        Coroutine mCoroutine = null;

        public int nProgress
        {
            get
            {
                return nCurProgress;
            }
        }

#region bundle加载
        public string m_curScenePath;
        CBundle mAbLoadData;

        public AbTask sceneLoadData
        {
            get { return mAbLoadData; }
        }


        public void Reset()
        {
            LOG.Info("Scene Mgr Loader Reset");
            //mAbLoadData = null;
            nCurProgress = 0;
            mIsLoadAbFinish = false;
            this.m_curScenePath = null;
            this.levelName = null;
            this.mAbLoadData = null;
            if (mCoroutine != null)
            {
                GameFramework.Instance.StopCoroutine(mCoroutine);
                mCoroutine = null;
            }
            if (mAsync != null)
            {
                mAsync = null;
            }
        }

        public void Load(string tag, string strScenePath, CBundle abLoadData = null)
        {
            if (strScenePath.Equals(m_curScenePath))
            {
                LOG.Warn("场景已经在加载中:" + strScenePath);
                return;
            }
            Reset();
            this.levelName = Path.GetFileNameWithoutExtension(strScenePath);//大小写敏感
            this.m_curScenePath = strScenePath;
            mAbLoadData = abLoadData;
            if (mAbLoadData != null)
            {
                abLoadData.AddCall((bundle) =>
                {
                    LOG.Info("加载场景assetbundle完成:" + levelName);
                    mIsLoadAbFinish = true;
                });
            }
            else
            {
                LOG.Info("加载场景assetbundle完成:" + levelName);
                mIsLoadAbFinish = true;
            }
            mCoroutine = GameFramework.Instance.StartCoroutine(LoadAssetBundleProcess(mAbLoadData));
        }


        //public void CheckScene()
        //{
        //    if(mAbLoadData == null)
        //    {
        //        LOG.Error("当前场景为空");
        //    }
        //    LOG.Info("当前场景:" + mAbLoadData.config.name+" 依赖:"+ mAbLoadData.dependency.Count);
        //    foreach(var d in mAbLoadData.dependency)
        //    {
        //        if(d == null || d.assetBundle == null)
        //        {
        //            LOG.Error("==========================================");
        //        }else
        //        {
        //            LOG.Info("依赖:" +d.config.name);
        //        }
        //    }
        //}


        /*
        //progress 的取值范围在0.1 - 1之间， 但是它不会等于1
        //也就是说progress可能是0.9的时候就直接进入新场景了
        //为了显示100%先禁止自动进入场景，达到90%时就不会再加载了，必须
        //allowSceneActivation = true才会加载到100%
        if (m_async.progress >= 0.9f)
        {
            m_slider.value = 1;
            m_async.allowSceneActivation = true;
        }
        else
        {
            m_slider.value = m_async.progress;
        }
        */

        IEnumerator LoadAssetBundleProcess(AbTask abLoadData)
        {
            LOG.Info("load scene res is start!!!");
            int p = 0;
            if (abLoadData != null)
            {
                while (!abLoadData.IsDone())
                {
                    int toPro = (int)(abLoadData.Progress() * 100);
#if false
                //LOG.Info(toPro, LOG.Color.cyan);
                while (p < toPro)
                {
                    ++p;
                    mCurProgress = p / 2;
                    yield return 0;
                }//while
#else
                    nCurProgress = toPro / 2;
#endif
                    yield return 0;
                }//while
            }

#if false
        while (p < 100)
        {
            ++p;
            mCurProgress = p / 2;
            yield return 0;
        }//while
#endif
            nCurProgress = 50;
            while (!mIsLoadAbFinish)
            {
                LOG.Info("mIsLoadAbFinish is false");
                yield return 0;
            }
            LOG.Info("场景bundle加载完成:" + nCurProgress);
            mCoroutine = GameFramework.Instance.StartCoroutine(LoadSceneProcess());
            yield return mCoroutine;
        }


        IEnumerator LoadSceneProcess()
        {
            //yield return new WaitForEndOfFrame();
            //异步读取
            if (ABMgr.Instance.isUseAssetBundle)
            {
                //var ab = mAbLoadData.assetBundle;
                //sab.LoadAllAssets();
                mAsync = SceneManager.LoadSceneAsync(levelName);
            }
#if UNITY_EDITOR
            else
            {
                mAsync = EditorApplication.LoadLevelAsyncInPlayMode(m_curScenePath);
            }
#endif
            if (mAsync == null)
            {
                //Debug.Log(mAbLoadData.config.name + " " + mAbLoadData.assetBundle.isStreamedSceneAssetBundle);
                LOG.Error("mAsync is null:" + (mAsync == null) + ",levelName:" + levelName);
            }
            //mAsync.allowSceneActivation = false;
            //        int toPro = 0;
            //        int p = 0;
            //		while (mAsync.progress < 0.9)
            //        {
            //            toPro = (int)(mAsync.progress * 100);
            //            //Debug.Log(toPro);
            //#if false
            //            while (p < toPro)
            //            {
            //                ++p;
            //                mCurProgress = 50 + p / 2;
            //                yield return 0;
            //            }//while
            //#else
            //            mCurProgress = 50 + toPro / 2;
            //			LOG.Info("mCurProgress="+mCurProgress);
            //#endif
            //            //Debug.Log("-->"+m_async.progress);
            //            yield return 0;
            //        }//while
            //
            //        //yield return mAsync;
            //
            //#if false
            //        toPro = 100;
            //        while (p < toPro)
            //        {
            //            ++p;
            //            mCurProgress = 50 + p / 2;
            //            yield return 0;
            //        }//while
            //#else
            //        mCurProgress = 100;
            //#endif
            //		LOG.Info( ("mAsync is done:" + mAsync.isDone);
            //mAsync.allowSceneActivation = true;//自动进入场景
            while (mAsync != null && !mAsync.isDone)
            {
                int p = (int)(mAsync.progress * 100);
                nCurProgress = 50 + p / 2;
                yield return 0;
            }
            nCurProgress = 100;
            OnLoadFinish();
        }

        void OnLoadFinish()
        {
            //Reset();
        }

#endregion
    }
}




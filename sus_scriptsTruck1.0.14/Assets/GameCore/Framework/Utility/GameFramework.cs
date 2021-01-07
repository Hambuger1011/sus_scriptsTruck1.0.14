using Framework;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using System.Reflection;
using ADTracking;

namespace Framework
{
    public abstract class GameFramework : CMonoSingleton<GameFramework>
    {
        protected bool m_isSystemPrepared = false;


        private bool lockFPS_SGame = true;
        public bool LockFPS
        {
            get
            {
                return this.lockFPS_SGame;
            }
            set
            {
                this.lockFPS_SGame = value;
                this.setTargetFrameRate();
            }
        }

        public static int unityTargetFrameRate
        {
            get
            {
                return GameSettings.c_renderFPS;
                //return 60;
            }
        }


        protected override void Init()
        {
            DontDestroyOnLoad(this.gameObject);
        }


        protected override void UnInit()
        {
            base.UnInit();
            //Singleton<BugLocateLogSys>.DestroyInstance();
        }
        public FirebaseTracker firebaseTracker = new FirebaseTracker();

        public virtual void Start()
        {
            if (firebaseTracker != null)
            {
                //firebase初始化
                firebaseTracker.Init("");
            }

            GameUtility.SetGameViewScale();
            //GL.Clear(false, true, Color.black);//清除显示缓存,防止花屏
            Application.runInBackground = true;
            //GameFramework.AppVersion = Application.version;//CVersion.GetAppVersion();

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            var bLandscape = true;
#if !_横屏
            bLandscape = false;
#endif
            //横屏
            Screen.autorotateToLandscapeLeft = bLandscape;//HOME键在右
            Screen.autorotateToLandscapeRight = bLandscape;//HOME键在左

            //竖屏
            Screen.autorotateToPortrait = !bLandscape;//HOME键在下
            Screen.autorotateToPortraitUpsideDown = !bLandscape;//HOME键在上

            Screen.orientation = ScreenOrientation.AutoRotation;
            AndroidUtils.ShowFullScreen();
            GameSettings.Init();
            PrepareGameSystem();
            LockFPS = true;
        }


        protected abstract void PrepareGameSystem();



        protected virtual void OnUpdate() { }
        protected virtual void OnLateUpdate() { }

        private void Update()
        {
            //if (Singleton<BattleLogic>.HasInstance() && Singleton<BattleLogic>.GetInstance().isFighting)
            //{
            //	FPSStatistic.Update();
            //}
            try
            {
                if (this.m_isSystemPrepared)
                {
                    OnUpdate();
                }
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
            }
        }

        private void LateUpdate()
        {
            try
            {
                if (m_isSystemPrepared)
                {
                    OnLateUpdate();
                }
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
            }
        }

        public int appPauseCount { get; private set; }
        public int appFocusCount { get; private set; }

        protected virtual void OnApplicationPause(bool pauseStatus)
        {
            //LOG.Info("OnApplicationPause:" + pauseStatus + ",FocusCount=" + appFocusCount + ",PauseCount=" + appPauseCount);
            if (pauseStatus)
            {
                ++appPauseCount;
            }
            else
            {
                --appPauseCount;
            }

        }

        protected virtual void OnApplicationFocus(bool focus)
        {
#if !UNITY_EDITOR
            //LOG.Info("OnApplicationFocus:" + focus + ",FocusCount="+appFocusCount+",PauseCount="+appPauseCount);
#endif
            if (focus)
            {
                ++appFocusCount;
            }
            else
            {
                --appFocusCount;
            }

#if UNITY_ANDROID || UNITY_EDITOR
            if (appFocusCount <= 0 && appPauseCount < 0 && SdkMgr.HasInstance())
#elif UNITY_IOS || UNITY_EDITOR
            //if (appFocusCount <= 1 && appPauseCount == 1 && SdkMgr.HasInstance())
            if(false)
#else
            if(false)
#endif
            {
#if UNITY_EDITOR
                //return;
#endif
            }
        }

        protected virtual void OnApplicationQuit()
        {
            LOG.Info("OnApplicationQuit:" + DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd-HH_mm_ss"));
//#if UNITY_2017_1_OR_NEWER
//            UnityEngine.Debug.unityLogger.logEnabled = false;
//#else
//            UnityEngine.Debug.logger.logEnabled = false;
//#endif
        }

        public void setTargetFrameRate()
        {
            if (this.lockFPS_SGame)
            {
                //this.StopCoroutine("SGame_WaitForTargetFrameRate");
                Application.targetFrameRate = GameSettings.c_renderFPS;
                LOG.Info("setTargetFrameRate:" + GameFramework.unityTargetFrameRate);
            }
            else
            {
                LOG.Info("setTargetFrameRate:" + GameFramework.unityTargetFrameRate);
                Application.targetFrameRate = GameFramework.unityTargetFrameRate;
                //this.StartCoroutine("SGame_WaitForTargetFrameRate");
            }
        }


        public void OnIOSApplicationWillEnterForeground(string msg)
        {
            if(appFocusCount <= 0 || appPauseCount > 0)
            {
                return;
            }
#if CHANNEL_SPAIN
            LOG.Error("播放后台切回广告");
            SdkMgr.Instance.ads.ShowInterstitial(DisjunctorType.Ads_Splash,"home-back");
#endif
        }




        public void OnAndroidApplicationWillEnterForeground(string msg)
	    {
#if CHANNEL_SPAIN
		    LOG.Error("播放后台切回广告");
		    SdkMgr.Instance.ads.ShowInterstitial(DisjunctorType.Ads_Splash, "home-back");
#endif
        }


    }
}

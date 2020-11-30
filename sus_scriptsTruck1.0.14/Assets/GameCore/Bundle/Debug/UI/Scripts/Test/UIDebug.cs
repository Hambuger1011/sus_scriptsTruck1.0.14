#if UNITY_EDITOR_OSX || (!UNITY_EDITOR && UNITY_IOS)
#define MAC
#endif

#if true
#if ENABLE_IL2CPP && !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
#define USE_IL2CPP
#else
#define USE_MONO
#endif
#endif

namespace Framework
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using GameCore;
    using UnityEngine.UI;
    using System.Runtime.InteropServices;
    using System.Diagnostics;
    using System.Text;
    using GameCore;
#if UNITY_5_5_OR_NEWER
    using UnityEngine.Profiling;
#endif
    using UnityEngine.EventSystems;
    using Framework;
    using AB;
    using global::UGUI;

    public class UIDebug : CUIEventScript
    {
        public CanvasGroup canvasGroup;
        public Text txtFpsCur;
        public Text txtFpsMax;

        public Text txtAssetMenUsed;
        public Text txtAssetMenTotal;

        public Text txtMonoMenUsed;
        public Text txtMonoMenTotal;

        public Text txtConfigTimeStamp;

        public Canvas uiCanvas;
        public Button btnFPS;
        public Text txtInfo;
        EFPSCounter fps = EFPSCounter.Create(0.5f);

        public const int N = 1024 * 1024;
#if UNITY_EDITOR
        IGCTools gcTools = new GCProfiler();
#elif USING_NATIVE_METHOD
#if USE_IL2CPP
                IGCTools gcTools = new GCIL2CPP();
#else
                IGCTools gcTools = new GCMono();
#endif
#else
        IGCTools gcTools = new GCProfiler();
#endif


        private void Awake()
        {
            //btnFPS.GetComponent<UIDragAndDrop>().onFixedPosition = OnFpsFixedPosition;

            btnFPS.onClick.AddListener(OnFPSClick);

        }

        public override void Initialize(CUIForm formScript)
        {
            base.Initialize(formScript);

            var trans = btnFPS.transform as RectTransform;
            Vector2 pos = trans.anchoredPosition;
            OnFpsFixedPosition(trans, ref pos);
            trans.anchoredPosition = pos;
        }

        string strConfigTimeStamp = "";
        StringBuilder sb = new StringBuilder();
        public override void CustomUpdate()
        {
            base.CustomUpdate();

            using (var sample = new ProfilerSample("Set Text"))
            {

                if (Time.time - m_lastTouchTime > 3)
                {
                    this.canvasGroup.alpha = 0.3f;
                }
                else
                {
                    this.canvasGroup.alpha = 1;
                }

                fps.Update();
                int maxfps = Application.targetFrameRate;
                if (maxfps <= 0)
                {
                    maxfps = 60;
                }

                txtFpsCur.text = Mathf.Min(fps.currentFPS, maxfps).ToString();
                txtFpsMax.text = maxfps.ToString();

                if (Time.frameCount % maxfps != 0)
                {
                    return;
                }

#if UNITY_2017_1_OR_NEWER
                var unityTotalReservedMemory = (int)Profiler.GetTotalReservedMemoryLong() / N;//资源总内存
                var unityUnusedReservedMemory = (int)Profiler.GetTotalUnusedReservedMemoryLong() / N;//空闲
                var unityTotalAllocatedMem = (int)Profiler.GetTotalAllocatedMemoryLong() / N;//已用
#else
                var unityTotalReservedMemory = (int)Profiler.GetTotalReservedMemory() / N;//资源总内存
                var unityUnusedReservedMemory = (int)Profiler.GetTotalUnusedReservedMemory() / N;//空闲
                var unityTotalAllocatedMem = (int)Profiler.GetTotalAllocatedMemory() / N;//已用
#endif


                long heapsize = gcTools.get_heap_size();//Mono堆总内存
                long usedsize = gcTools.get_used_size();//已用,GameUtility.FormatBytes(System.GC.GetTotalMemory(false))
                long reservedsize = heapsize - usedsize;//空闲

                txtAssetMenUsed.text = unityTotalAllocatedMem.ToString();
                txtAssetMenTotal.text = unityTotalReservedMemory.ToString();

                txtMonoMenUsed.text = usedsize.ToString();
                txtMonoMenTotal.text = heapsize.ToString();

                if (strConfigTimeStamp == "" && !string.IsNullOrEmpty(UserDataManager.Instance.ResVersion))
                {
                    //if (GameData.HasInstance() && GameData.Instance.confData.allConf != null)
                    //{
                    //    string time_snap = GameData.Instance.confData.allConf.gamebase.time_snap;
                    //    strConfigTimeStamp = time_snap.Substring(5);
                    //}
                    strConfigTimeStamp = UserDataManager.Instance.ResVersion;
                }
                txtConfigTimeStamp.text = strConfigTimeStamp;
            }
        }



        private void OnFPSLongPress(GameObject go)
        {
        }

        private void OnFPSClick()
        {
            ABSystem.Instance.LoadImme("AbTag.Debug", enResType.ePrefab, CUIID.Canvas_Test);
            CUIManager.Instance.OpenForm(CUIID.Canvas_Test, true, false);
        }

        float m_lastTouchTime = 0;

        #region Drag Event
        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            m_lastTouchTime = Time.time;
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            SetPosition(eventData);
            m_lastTouchTime = Time.time;
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            m_lastTouchTime = Time.time;
        }

        public void SetPosition(PointerEventData pointerEventData)
        {
            RectTransform rectTransform = this.transform as RectTransform;
            var trans = btnFPS.transform as RectTransform;
            Vector2 pos = CUIUtility.Screen_To_UGUI_LocalPoint(this.myForm.GetCamera(), pointerEventData.position, rectTransform);
            OnFpsFixedPosition(trans, ref pos);
            trans.anchoredPosition = pos;
            /*
            Vector3 position;
            if (rectTransform != null && RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, pointerEventData.position, pointerEventData.pressEventCamera, out position))
            {
                var trans = btnFPS.transform as RectTransform;
                Vector2 pos = position;
                OnFpsFixedPosition(trans, ref pos);
                trans.anchoredPosition = position;
            }
            */
        }

        void OnFpsFixedPosition(RectTransform trans, ref Vector2 pos)
        {
            var halfResolution = this.myForm.GetViewSize() * 0.5f;
            var halfSize = trans.rect.size * 0.5f;
            //width
            if (pos.x < halfSize.x - halfResolution.x)
            {
                pos.x = halfSize.x - halfResolution.x;
            }
            if (pos.x > halfResolution.x - halfSize.x)
            {
                pos.x = halfResolution.x - halfSize.x;
            }
            //height
            if (pos.y < halfSize.y - halfResolution.y)
            {
                pos.y = halfSize.y - halfResolution.y;
            }
            if (pos.y > halfResolution.y - halfSize.y)
            {
                pos.y = halfResolution.y - halfSize.y;
            }
        }
        #endregion
    }



    public class EFPSCounter
    {
        private int FPSAccumulator;
        private float FPSNextPeriod = 0;

        public float FPSMeasurePeriod = 1f;
        public int currentFPS = 60;
        public float deltaTime;

        public static EFPSCounter Create(float period)
        {
            EFPSCounter instance = new EFPSCounter(period);
            return instance;
        }

        protected EFPSCounter(float period)
        {
            FPSMeasurePeriod = period;
        }

        // has to be called
        float lastUpdateTime = 0;
        public void Update()
        {
            FPSAccumulator++;
            deltaTime = (Time.realtimeSinceStartup - lastUpdateTime) * 1000;
            lastUpdateTime = Time.realtimeSinceStartup;

            if (Time.realtimeSinceStartup > FPSNextPeriod)
            {
                currentFPS = (int)(FPSAccumulator / FPSMeasurePeriod);
                FPSAccumulator = 0;
                FPSNextPeriod += FPSMeasurePeriod;
            }
        }
    }


    public interface IGCTools
    {
        long get_used_size();
        long get_heap_size();
    }


#if USE_IL2CPP
    public class GCIL2CPP : IGCTools
    {
#if MAC
        const string DLL_IMPORT_SYMBOL = "__Internal";
#else
        const string DLL_IMPORT_SYMBOL = "il2cpp";
#endif
        [DllImport(DLL_IMPORT_SYMBOL)]
        public static extern long il2cpp_gc_get_used_size();

        [DllImport(DLL_IMPORT_SYMBOL)]
        public static extern long il2cpp_gc_get_heap_size();
        public long get_heap_size()
        {
            return il2cpp_gc_get_heap_size() / UIDebug.N;
        }

        public long get_used_size()
        {
            return il2cpp_gc_get_used_size() / UIDebug.N;
        }
    }
#elif USE_MONO
    public class GCMono : IGCTools
    {
#if MAC
        const string DLL_IMPORT_SYMBOL = "__Internal";
#else
#if NET_4_6
        const string DLL_IMPORT_SYMBOL = "mono-2.0-bdwgc";
#else
        const string DLL_IMPORT_SYMBOL = "mono";
#endif
#endif
        [DllImport(DLL_IMPORT_SYMBOL, EntryPoint = "mono_gc_get_used_size")]
        public static extern long mono_gc_get_used_size();
        [DllImport(DLL_IMPORT_SYMBOL, EntryPoint = "mono_gc_get_heap_size")]
        public static extern long mono_gc_get_heap_size();

        public long get_heap_size()
        {
            return mono_gc_get_heap_size() / UIDebug.N;
        }

        public long get_used_size()
        {
            return mono_gc_get_used_size() / UIDebug.N;
        }
    }
#endif

    public class GCProfiler : IGCTools
    {
        public long get_heap_size()
        {
#if UNITY_2017_1_OR_NEWER
            return Profiler.GetMonoHeapSizeLong() / UIDebug.N;
#else
            return Profiler.GetMonoHeapSize() / UIDebug.N;
#endif
        }

        public long get_used_size()
        {
#if UNITY_2017_1_OR_NEWER
            return Profiler.GetMonoUsedSizeLong() / UIDebug.N;
#else
            return Profiler.GetMonoUsedSize() / UIDebug.N;
#endif
        }
    }

    
    public struct ProfilerSample : IDisposable
    {
        public ProfilerSample(string strMsg)
        {
            Profiler.BeginSample(strMsg);
        }
        public void Dispose()
        {
            Profiler.EndSample();
        }
    }
}

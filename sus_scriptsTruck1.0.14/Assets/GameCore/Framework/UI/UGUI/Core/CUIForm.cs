using Object = UnityEngine.Object;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework;
using Common;
using AB;
using System.Diagnostics;

namespace UGUI
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [ExecuteInEditMode]
	public class CUIForm : MonoBehaviour, IComparable<CUIForm>
	{

		private const int c_openOrderMask = 10;

		private const int c_priorityOrderMask = 1000;

		private const int c_overlayOrderMask = 10000;//overlay模型最低层级

        private const int c_renderModeRevertCounter = 30;

        #region Inspector参数
        [Header("分辨率")]
        public Vector2 m_referenceResolution = CUIManager.s_resolution;

        [Header("是否只有一个实例")]
        public bool m_isSingleton = true;

        [Header("是否模态窗口")]
        public bool m_isModal = true;

        [Header("Canvas层级")]
        public enFormPriority m_priority = enFormPriority.Priority3;

        [Header("加载时关闭的ui组")]
        public int m_group;

		//public bool m_fullScreenBG;

        [Header("禁用点击事件")]
        public bool m_disableInput;
        
        [Header("是否隐藏下层的Canvas")]
        public bool m_hideUnderForms;

        [Header("保持可见")]
        public bool m_alwaysKeepVisible;
        #endregion

        [System.NonSerialized]
        public string m_formPath;

        private enFormPriority m_defaultPriority;

        private bool m_bLateUpdateUIComponents;

		private bool m_bUpdateUIComponents;
        
		public bool m_enableMultiClickedEvent = true;

		public bool m_isExceptionOfRaycastSorting;



        [Header("淡入淡出动画")]
		public enFormFadeAnimationType m_formFadeInAnimationType;

		public string m_formFadeInAnimationName = string.Empty;

		//public bool m_isPlayFadeInWithAppear;

		//public GameObject m_mapViewTmpl;

		private CUIComponent m_formFadeInAnimationScript;

		[HideInInspector]
		public enFormFadeAnimationType m_formFadeOutAnimationType;

		[HideInInspector]
		public string m_formFadeOutAnimationName = string.Empty;

		private CUIComponent m_formFadeOutAnimationScript;

		[HideInInspector]
		public int m_clickedEventDispatchedCounter;

		[HideInInspector]
		public bool m_useFormPool;

		private bool m_isNeedClose;

		private bool m_isClosed;

		private bool m_isInFadeIn;

		private bool m_isInFadeOut;

		[HideInInspector]
		public Canvas canvas;

		[HideInInspector]
		public CanvasScaler canvasScaler;

		private GraphicRaycaster graphicRaycaster;

		[NonSerialized]
		[HideInInspector]
		public SGameGraphicRaycaster sgameGraphicRaycaster;

		private int m_openOrder;

		private int m_sortingOrder;

		private int m_sequence;

		private bool m_isHided;

		private int m_hideFlags;

		private int m_renderFrameStamp;

		private List<CInitWidgetPosition> m_initWidgetPositions;

		private bool m_isInitialized;

		private List<CUIComponent> m_uiComponents;

		private List<CUIComponent> m_customUpdateUIComponents;

		private List<CUIComponent> m_customLateUpdateUIComponents;

		[HideInInspector]
		private List<GameObject> m_relatedScenes;

		[HideInInspector]
		private List<List<Camera>> m_relatedSceneCamera;

		[HideInInspector]
		private bool m_isCanChangeCameraTypeInBattle = true;

		[HideInInspector]
		public bool m_isAlwaysExit;

		[NonSerialized]
		[HideInInspector]
		public GameObject m_root;

		private List<CASyncLoadedImage> m_asyncLoadedImages;

		private Dictionary<string, GameObject> m_loadedSpriteDictionary;

		private int m_requestSGameGraphicRaycasterUpdateTilesFrameStamp;

		private int m_renderModeRevertCounter;

        public EnumUIStyle uistyle = EnumUIStyle.CommonUI;

        public bool isLua = false;
        
		CAsset _asset;

		public CAsset asset
        {
            get
            {
                return _asset;
            }

            set
            {
                if (_asset != null)
                {
                    _asset.Release(this);
                    //if (_asset.retainCount > 0)
                    //{
                    //    LOG.Error("引用大于0:" + m_formPath);
                    //}
                }
                _asset = value;
                if (_asset != null)
                {
                    _asset.Retain(this);
                }
            }
        }


        public static CUIForm LoadRes(string formPrefabPath)
        {
            var asset = ABSystem.ui.bundle.LoadImme(AbTag.Null, enResType.ePrefab, formPrefabPath);
            if (asset == null)
            {
                LOG.Error("未找到UI Prefab:" + formPrefabPath);
                return null;
            }

            var go = asset.Instantiate();
            var uiForm = go.GetComponent<CUIForm>();
            uiForm.asset = asset;
            return uiForm;
        }


        public void SetCanChangeCameraTypeInBattle(bool canChange)
		{
			m_isCanChangeCameraTypeInBattle = canChange;
		}

		public bool GetCanChangeCameraTypeInBattle()
		{
			return m_isCanChangeCameraTypeInBattle;
		}

		private void Awake()
		{
			m_uiComponents = new List<CUIComponent>();
			m_customUpdateUIComponents = new List<CUIComponent>();
			m_customLateUpdateUIComponents = new List<CUIComponent>();
			m_relatedScenes = new List<GameObject>();
			m_relatedSceneCamera = new List<List<Camera>>();
			InitializeCanvas();
		}

		private void OnDestroy()
		{
			UnInitialize();
            if(asset != null)
            {
                asset.Release();//释放引用
                asset = null;
            }
        }

		public void CustomUpdate()
		{
			UpdateFadeIn();
			UpdateFadeOut();
			CustomUpdateUIComponents();
		}

		public void CustomLateUpdate()
		{
			if (m_initWidgetPositions != null)
			{
				int idx = 0;
				while (idx < m_initWidgetPositions.Count)
				{
					CInitWidgetPosition cInitWidgetPosition = m_initWidgetPositions[idx];
					if (m_renderFrameStamp - cInitWidgetPosition.m_renderFrameStamp <= 1)
					{
						if (cInitWidgetPosition.m_widget != null)
						{
							cInitWidgetPosition.m_widget.transform.position = cInitWidgetPosition.m_worldPosition;
						}
						idx++;
					}
					else
					{
						m_initWidgetPositions.RemoveAt(idx);
					}
				}
			}

			UpdateSGameGraphicRaycasterUpdateTiles();
			UpdateRenderModeRevert();
			CustomLateUpdateUIComponents();
			m_clickedEventDispatchedCounter = 0;
			m_renderFrameStamp++;
		}

		private void CustomUpdateUIComponents()
		{
			if (m_customUpdateUIComponents != null)
			{
				int count = m_customUpdateUIComponents.Count;
				if (count > 0)
				{
					for (int i = 0; i < count; i++)
					{
						if (m_customUpdateUIComponents[i] == null && !m_bUpdateUIComponents)
						{
							m_bUpdateUIComponents = true;
							////BuglyAgent.ReportException(new CustomLateUpdateUIComponentsException("update" + m_formPath), "update" + m_formPath);
						}
						if (m_customUpdateUIComponents[i] != null && m_customUpdateUIComponents[i].gameObject.activeInHierarchy)
						{
							m_customUpdateUIComponents[i].CustomUpdate();
						}
					}
				}
			}
		}

		private void CustomLateUpdateUIComponents()
		{
			if (m_customLateUpdateUIComponents != null)
			{
				int count = m_customLateUpdateUIComponents.Count;
				if (count > 0)
				{
					for (int i = 0; i < count; i++)
					{
						if (m_customLateUpdateUIComponents[i] == null && !m_bLateUpdateUIComponents)
						{
							m_bLateUpdateUIComponents = true;
							//BuglyAgent.ReportException(new CustomLateUpdateUIComponentsException("lateUpdate" + m_formPath), "lateUpdate" + m_formPath);
						}
						if (m_customLateUpdateUIComponents[i] != null && m_customLateUpdateUIComponents[i].gameObject.activeInHierarchy)
						{
							m_customLateUpdateUIComponents[i].CustomLateUpdate();
						}
					}
				}
			}
		}

        public void SetCanvasEnable(bool bEnable)
        {
            this.canvasScaler.enabled = bEnable;
            this.canvas.enabled = bEnable;
        }

        public int GetSequence()
		{
			return m_sequence;
		}


        #region 初始化

        public void InitializeWidgetPosition(GameObject widget, Vector3 worldPosition)
        {
            if (m_initWidgetPositions == null)
            {
                m_initWidgetPositions = new List<CInitWidgetPosition>();
            }
            CInitWidgetPosition cInitWidgetPosition = new CInitWidgetPosition();
            cInitWidgetPosition.m_renderFrameStamp = m_renderFrameStamp;
            cInitWidgetPosition.m_widget = widget;
            cInitWidgetPosition.m_worldPosition = worldPosition;
            m_initWidgetPositions.Add(cInitWidgetPosition);
        }


        public void Initialize()
        {
            if (!m_isInitialized)
            {
                m_defaultPriority = m_priority;
                InitializeComponent(base.gameObject);
                m_isInitialized = true;
            }
        }

        public void UnInitialize()
        {
            if (m_isInitialized)
            {
                UnInitializeComponent();
                m_formFadeInAnimationName = null;
                m_formFadeInAnimationScript = null;
                m_formFadeOutAnimationName = null;
                m_formFadeOutAnimationScript = null;
                m_formPath = null;
                canvas = null;
                canvasScaler = null;
                graphicRaycaster = null;
                sgameGraphicRaycaster = null;
                if (m_initWidgetPositions != null)
                {
                    m_initWidgetPositions.Clear();
                    m_initWidgetPositions = null;
                }
                if (m_uiComponents != null)
                {
                    m_uiComponents.Clear();
                    m_uiComponents = null;
                }
                if (m_customUpdateUIComponents != null)
                {
                    m_customUpdateUIComponents.Clear();
                    m_customUpdateUIComponents = null;
                }
                if (m_customLateUpdateUIComponents != null)
                {
                    m_customLateUpdateUIComponents.Clear();
                    m_customLateUpdateUIComponents = null;
                }
                if (m_relatedScenes != null)
                {
                    m_relatedScenes.Clear();
                    m_relatedScenes = null;
                }
                if (m_relatedSceneCamera != null)
                {
                    for (int i = 0; i < m_relatedSceneCamera.Count; i++)
                    {
                        m_relatedSceneCamera[i].Clear();
                    }
                    m_relatedSceneCamera.Clear();
                    m_relatedSceneCamera = null;
                }
                if (m_asyncLoadedImages != null)
                {
                    m_asyncLoadedImages.Clear();
                    m_asyncLoadedImages = null;
                }
                if (m_loadedSpriteDictionary != null)
                {
                    m_loadedSpriteDictionary.Clear();
                    m_loadedSpriteDictionary = null;
                }
                m_isInitialized = false;
            }
        }
        #endregion

        #region 设置层级
        public void SetDisplayOrder(int openOrder)
		{
			LOG.Assert(openOrder > 0, "openOrder = {0}, 该值必须大于0", openOrder);
			m_openOrder = openOrder;
			if (canvas != null)
			{
				m_sortingOrder = CalculateSortingOrder(m_priority, m_openOrder);
				canvas.sortingOrder = m_sortingOrder;
				try
				{
					if (canvas.enabled)
					{
						canvas.enabled = false;
						canvas.enabled = true;
					}
				}
				catch (Exception ex)
				{
					LOG.Assert(false, "Error form {0}: message: {1}, callstack: {2}", base.name, ex.Message, ex.StackTrace);
				}
			}
			SetComponentSortingOrder(m_sortingOrder);
		}
        #endregion


        #region 打开操作
        public void Open(string formPath, Camera camera, int sequence, bool exist, int openOrder)
		{
			m_formPath = formPath;
			if (canvas != null)
			{
				canvas.worldCamera = camera;
				if (camera == null)
				{
					if (canvas.renderMode != 0)
					{
						canvas.renderMode = RenderMode.ScreenSpaceOverlay;
					}
				}
				else if (canvas.renderMode != RenderMode.ScreenSpaceCamera)
				{
					canvas.renderMode = RenderMode.ScreenSpaceCamera;
				}
                //this.m_canvas.pixelPerfect = true;
                this.canvas.planeDistance = 500;
            }
			RefreshCanvasScaler();
			Open(sequence, exist, openOrder);
		}

		public void Open(int sequence, bool exist, int openOrder)
		{
			m_bLateUpdateUIComponents = false;
			m_bUpdateUIComponents = false;
			m_isNeedClose = false;
			m_isClosed = false;
			m_isInFadeIn = false;
			m_isInFadeOut = false;
			m_clickedEventDispatchedCounter = 0;
			m_sequence = sequence;
			SetDisplayOrder(openOrder);
			m_renderFrameStamp = 0;
			if (!exist)
			{
				Initialize();
                this.OpenComponent();
                if ((bool)graphicRaycaster)
				{
					graphicRaycaster.enabled = !m_disableInput;
				}

				//TODL:播放音效
				if (IsNeedFadeIn())
				{
					StartFadeIn();
				}
			}
		}
        #endregion

        #region 关闭操作
        public void Close()
		{
			m_bLateUpdateUIComponents = false;
			m_bUpdateUIComponents = false;
			if (!m_isAlwaysExit && !m_isNeedClose && !m_isClosed)
			{
				m_isNeedClose = true;
				//TODO:关闭音效
				CloseComponent();
			}
		}

        /// <summary>
        /// 是否需要关闭
        /// </summary>
        public bool IsNeedClose()
		{
			return m_isNeedClose;
		}

        /// <summary>
        /// 转向关闭
        /// </summary>
        public bool TurnToClosed(bool ignoreFadeOut)
		{
			m_isNeedClose = false;
			m_isClosed = true;
			CSingleton<EventRouter>.GetInstance().BroadCastEvent(UIEventID.UI_FORM_CLOSED, m_formPath);
			if (!ignoreFadeOut && IsNeedFadeOut())
			{
				StartFadeOut();
				return false;
			}
			return true;
		}

		public bool IsClosed()
		{
			return m_isClosed || m_isNeedClose;
		}
        #endregion

        public bool IsCanvasEnabled()
		{
			if (canvas == null)
			{
				return false;
			}
			return canvas.enabled;
		}
        


        public void SetPriority(enFormPriority priority)
		{
			if (m_priority != priority)
			{
				m_priority = priority;
				SetDisplayOrder(m_openOrder);
			}
		}

		public void RestorePriority()
		{
			SetPriority(m_defaultPriority);
		}

		public void SetActive(bool active)
		{
			base.gameObject.SetActiveEx(active);
			if (active)
			{
				Appear(enFormHideFlag.HideByCustom, true);
			}
			else
			{
				Hide(enFormHideFlag.HideByCustom, true);
			}
		}

		public void Hide(enFormHideFlag hideFlag = enFormHideFlag.HideByCustom, bool dispatchVisibleChangedEvent = true)
		{
			if (!m_alwaysKeepVisible)
			{
				m_hideFlags |= (int)hideFlag;
				if (m_hideFlags != 0 && !m_isHided)
				{
					m_isHided = true;
					if (canvas != null)
                    {
                        SetCanvasEnable(false);
                    }
					TryEnableInput(false);
					for (int i = 0; i < m_relatedScenes.Count; i++)
					{
						CUIUtility.SetGameObjectLayer(m_relatedScenes[i], 31);
						SetSceneCameraEnable(i, false);
					}
					HideComponent();
                    CUIManager.Instance.ResetAllFormHideOrShowState();
                }
			}
		}

        public void SetSceneCameraEnable(int index, bool bEnable)
		{
			if (index >= 0 && index < m_relatedSceneCamera.Count && m_relatedSceneCamera[index] != null)
			{
				for (int i = 0; i < m_relatedSceneCamera[index].Count; i++)
				{
					if (m_relatedSceneCamera[index][i] != null)
					{
						m_relatedSceneCamera[index][i].enabled = bEnable;
					}
				}
			}
		}

		public bool IsHided()
		{
			return m_isHided;
		}

		public void Appear(enFormHideFlag hideFlag = enFormHideFlag.HideByCustom, bool dispatchVisibleChangedEvent = true)
		{
			if (!m_alwaysKeepVisible)
			{
				m_hideFlags &= (int)(~hideFlag);
				if (m_hideFlags == 0 && m_isHided)
				{
					m_isHided = false;
					if (canvas != null)
                    {
                        SetCanvasEnable(true);
                        canvas.sortingOrder = m_sortingOrder;
					}
					TryEnableInput(true);
					for (int i = 0; i < m_relatedScenes.Count; i++)
					{
						CUIUtility.SetGameObjectLayer(m_relatedScenes[i], 18);
						SetSceneCameraEnable(i, true);
					}
					AppearComponent();
                    CUIManager.Instance.ResetAllFormHideOrShowState();
                }
			}
		}

		public void TryEnableInput(bool isEnable)
		{
			if (!(graphicRaycaster == null))
			{
				switch (isEnable)
				{
				case false:
					graphicRaycaster.enabled = false;
					break;
				default:
					if (!m_disableInput)
					{
						graphicRaycaster.enabled = true;
					}
					break;
				}
			}
		}


        #region canvas排序
        public int CompareTo(CUIForm uiForm)
        {
            //从小到大排序
			if (m_sortingOrder > uiForm.m_sortingOrder)
			{
				return 1;
			}
			if (m_sortingOrder == uiForm.m_sortingOrder)
			{
				return 0;
			}
			return -1;
		}
        #endregion

        [ContextMenu("InitializeCanvas")]
        public void InitializeCanvas()
		{
			canvas = base.gameObject.GetComponent<Canvas>();
			canvasScaler = base.gameObject.GetComponent<CanvasScaler>();
			graphicRaycaster = base.GetComponent<GraphicRaycaster>();
			sgameGraphicRaycaster = (graphicRaycaster as SGameGraphicRaycaster);
			MatchScreen();
		}

        [Conditional("UNITY_EDITOR")]
		public void MatchScreen()
		{
            if (canvasScaler == null)
            {
                return;
            }

            canvasScaler.referenceResolution = m_referenceResolution;
#if UNITY_EDITOR
            canvasScaler.runInEditMode = true;
#endif
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            //canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
#if false
            m_canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

            float screenWidth = (float)Screen.width;
            float screenHeight = (float)Screen.height;
            Vector2 referenceResolution = m_canvasScaler.referenceResolution;
            float w = screenWidth / referenceResolution.x;
            float h = screenHeight / referenceResolution.y;
            if (w <= h)
            {
                if (m_fullScreenBG)
                {
                    m_canvasScaler.matchWidthOrHeight = 0f;
                }
                else
                {
                    m_canvasScaler.matchWidthOrHeight = 1f;
                }
            }else
            {
                if (m_fullScreenBG)
                {
                    m_canvasScaler.matchWidthOrHeight = 1f;
                }
                else
                {
                    m_canvasScaler.matchWidthOrHeight = 0f;
                }
            }
#endif
        }

        public GraphicRaycaster GetGraphicRaycaster()
		{
			return graphicRaycaster;
		}

		public Camera GetCamera()
		{
			if (canvas != null && canvas.renderMode != 0)
			{
				return canvas.worldCamera;
			}
			return null;
		}

		public Vector2 GetReferenceResolution()
		{
			return (!(canvasScaler == null)) ? canvasScaler.referenceResolution : Vector2.zero;
		}

		public int GetSortingOrder()
		{
			return m_sortingOrder;
		}

		public void AddUIComponent(CUIComponent uiComponent)
		{
			if (m_uiComponents != null && !m_uiComponents.Contains(uiComponent))
			{
				m_uiComponents.Add(uiComponent);
			}
			if (uiComponent.HasCustomUpdateFlag(enCustomUpdateFlag.eUpdate) && m_customUpdateUIComponents != null && !m_customUpdateUIComponents.Contains(uiComponent))
			{
				m_customUpdateUIComponents.Add(uiComponent);
			}
			if (uiComponent.HasCustomUpdateFlag(enCustomUpdateFlag.eLateUpdate) && m_customLateUpdateUIComponents != null && !m_customLateUpdateUIComponents.Contains(uiComponent))
			{
				m_customLateUpdateUIComponents.Add(uiComponent);
			}
		}

		public void RemoveUIComponent(CUIComponent uiComponent)
		{
			if (m_uiComponents != null)
			{
				m_uiComponents.Remove(uiComponent);
			}
			if (uiComponent.HasCustomUpdateFlag(enCustomUpdateFlag.eUpdate) && m_customUpdateUIComponents != null)
			{
				m_customUpdateUIComponents.Remove(uiComponent);
			}
			if (uiComponent.HasCustomUpdateFlag(enCustomUpdateFlag.eLateUpdate) && m_customLateUpdateUIComponents != null)
			{
				m_customLateUpdateUIComponents.Remove(uiComponent);
			}
		}

		public bool IsRelatedSceneExist(string sceneName)
		{
			for (int i = 0; i < m_relatedScenes.Count; i++)
			{
				if (string.Equals(sceneName, m_relatedScenes[i].name))
				{
					return true;
				}
			}
			return false;
		}

		public void AddRelatedScene(GameObject scene, string sceneName)
		{
			scene.name = sceneName;
			scene.transform.SetParent(m_root.transform);
			scene.transform.localPosition = Vector3.zero;
			scene.transform.localRotation = Quaternion.identity;
			m_relatedScenes.Add(scene);
			m_relatedSceneCamera.Add(new List<Camera>());
			AddRelatedSceneCamera(m_relatedSceneCamera.Count - 1, scene);
		}

		public void AddRelatedSceneCamera(int index, GameObject go)
		{
			if (index >= 0 && index < m_relatedSceneCamera.Count && !(go == null))
			{
				Camera component = go.GetComponent<Camera>();
				if (component != null)
				{
					m_relatedSceneCamera[index].Add(component);
				}
				for (int i = 0; i < go.transform.childCount; i++)
				{
					AddRelatedSceneCamera(index, go.transform.GetChild(i).gameObject);
				}
			}
		}

		public void AddASyncLoadedImage(Image image, string prefabPath, bool needCached, bool unloadBelongedAssetBundleAfterLoaded, bool isShowSpecMatrial = false)
		{
			if (m_asyncLoadedImages == null)
			{
				m_asyncLoadedImages = new List<CASyncLoadedImage>();
			}
			if (m_loadedSpriteDictionary == null)
			{
				m_loadedSpriteDictionary = new Dictionary<string, GameObject>();
			}
			for (int i = 0; i < m_asyncLoadedImages.Count; i++)
			{
				if (m_asyncLoadedImages[i].m_image == image)
				{
					m_asyncLoadedImages[i].m_prefabPath = prefabPath;
					m_asyncLoadedImages[i].m_needCached = needCached;
					m_asyncLoadedImages[i].m_isShowSpecMatrial = isShowSpecMatrial;
					return;
				}
			}
			CASyncLoadedImage item = new CASyncLoadedImage(image, prefabPath, needCached, unloadBelongedAssetBundleAfterLoaded, isShowSpecMatrial);
			m_asyncLoadedImages.Add(item);
			Color color = image.color;
			float r = color.r;
			Color color2 = image.color;
			float g = color2.g;
			Color color3 = image.color;
			image.color = new Color(r, g, color3.b, 0f);
		}

        public bool IsNeedFadeIn()
		{
            return false;//GameSettings.RenderQuality != SGameRenderQuality.Low && m_formFadeInAnimationType != 0 && !string.IsNullOrEmpty(m_formFadeInAnimationName);
		}

		public bool IsNeedFadeOut()
		{
			return false;
		}

		public void RePlayFadIn()
		{
			StartFadeIn();
		}

		private void RefreshCanvasScaler()
		{
			try
			{
				if (canvasScaler != null)
				{
					canvasScaler.enabled = false;
					canvasScaler.enabled = true;
				}
			}
			catch (Exception ex)
			{
				LOG.Assert(false, "Error form {0}: message: {1}, callstack: {2}", base.name, ex.Message, ex.StackTrace);
			}
		}

        

		private bool IsOverlay()
		{
			if (canvas == null)
			{
				return false;
			}
			return canvas.renderMode == RenderMode.ScreenSpaceOverlay || canvas.worldCamera == null;
		}

		private int CalculateSortingOrder(enFormPriority priority, int openOrder)
		{
			if (openOrder * 10 >= 1000)
			{
				openOrder %= 100;
			}
			return (IsOverlay() ? 10000 : 0) + (int)priority * 1000 + openOrder * 10;
		}

		public void InitializeComponent(GameObject root)
		{
			CUIComponent[] componentsInChildren = root.GetComponentsInChildren<CUIComponent>(true);
			if (componentsInChildren != null && componentsInChildren.Length > 0)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].Initialize(this);
				}
			}
		}

		public void UnInitializeComponent(GameObject root)
		{
			CUIComponent[] componentsInChildren = root.GetComponentsInChildren<CUIComponent>(true);
			if (componentsInChildren != null && componentsInChildren.Length > 0)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].UnInitialize();
				}
			}
		}
        

		private void CloseComponent()
		{
			for (int i = 0; i < m_uiComponents.Count; i++)
			{
                if (m_uiComponents[i] != null)
				    m_uiComponents[i].OnClose();
			}
		}

        private void OpenComponent()
        {
            for (int i = 0; i < this.m_uiComponents.Count; i++)
            {
                var c = this.m_uiComponents[i];
                if (c == null)
                {
                    continue;
                }
                c.OnOpen();
            }
        }

        private void HideComponent()
		{
			for (int i = 0; i < m_uiComponents.Count; i++)
			{
                if (m_uiComponents[i] != null)
				    m_uiComponents[i].OnHide();
			}
		}

		private void AppearComponent()
		{
			for (int i = 0; i < m_uiComponents.Count; i++)
			{
                if (m_uiComponents[i] != null)
				    m_uiComponents[i].OnAppear();
			}
		}

		private void SetComponentSortingOrder(int sortingOrder)
		{
			for (int i = 0; i < m_uiComponents.Count; i++)
			{
                if (m_uiComponents[i] != null)
				    m_uiComponents[i].SetSortingOrder(sortingOrder);
			}
		}

		private void UnInitializeComponent()
		{
			for (int i = 0; i < m_uiComponents.Count; i++)
			{
                if (m_uiComponents[i] != null)
				    m_uiComponents[i].UnInitialize();
			}
		}

		private void StartFadeIn()
		{
		}

		private void StartFadeOut()
		{
		}

		private void UpdateFadeIn()
		{

		}

		private void UpdateFadeOut()
		{

		}

		public bool IsInFadeIn()
		{
			return m_isInFadeIn;
		}

		public bool IsInFadeOut()
		{
			return m_isInFadeOut;
		}

		public void SetHideUnderForm(bool isHideUnderForm)
		{
			m_hideUnderForms = isHideUnderForm;
			CUIManager.Instance.ResetAllFormHideOrShowState();
		}
        

		public void ChangeRenderMode(RenderMode targetMode)
		{
			if (canvas != null && canvas.renderMode != targetMode)
			{
				canvas.renderMode = targetMode;
				if (IsUseSGameGraphicRaycaster() && (targetMode == RenderMode.ScreenSpaceOverlay || targetMode == RenderMode.ScreenSpaceCamera))
				{
					RequestSGameGraphicRaycasterUpdateTiles();
				}
			}
		}

		public void OnScreenResolutionChanged()
		{
			if (!(canvas == null))
			{
				if (canvas.renderMode == RenderMode.WorldSpace)
				{
					ChangeRenderMode(RenderMode.ScreenSpaceCamera);
					m_renderModeRevertCounter = 30;
				}
				else
				{
					if (IsUseSGameGraphicRaycaster())
					{
						RequestSGameGraphicRaycasterUpdateTiles();
					}
					if (m_renderModeRevertCounter > 0)
					{
						m_renderModeRevertCounter = 30;
					}
				}
			}
		}

		public RenderMode GetRenderMode()
		{
			return (canvas != null) ? canvas.renderMode : RenderMode.ScreenSpaceOverlay;
		}

		private bool IsUseSGameGraphicRaycaster()
		{
			return sgameGraphicRaycaster != null && sgameGraphicRaycaster.IsInitialized() && sgameGraphicRaycaster.IsUseSGameMode();
		}

		private void RequestSGameGraphicRaycasterUpdateTiles()
		{
			m_requestSGameGraphicRaycasterUpdateTilesFrameStamp = m_renderFrameStamp;
		}

		private void UpdateSGameGraphicRaycasterUpdateTiles()
		{
			if (m_requestSGameGraphicRaycasterUpdateTilesFrameStamp > 0 && m_renderFrameStamp - m_requestSGameGraphicRaycasterUpdateTilesFrameStamp >= 2)
			{
				if (IsUseSGameGraphicRaycaster())
				{
					sgameGraphicRaycaster.UpdateTiles();
				}
				m_requestSGameGraphicRaycasterUpdateTilesFrameStamp = 0;
			}
		}

		private void UpdateRenderModeRevert()
		{
			if (m_renderModeRevertCounter > 0 && --m_renderModeRevertCounter <= 0)
			{
				ChangeRenderMode(RenderMode.WorldSpace);
				m_renderModeRevertCounter = 0;
			}
		}



        private class CInitWidgetPosition
        {
            public int m_renderFrameStamp;

            public GameObject m_widget;

            public Vector3 m_worldPosition;
        }

        private class CASyncLoadedImage
        {
            public Image m_image;

            public string m_prefabPath;

            public bool m_needCached;

            public bool m_unloadBelongedAssetBundleAfterLoaded;

            public bool m_isShowSpecMatrial;

            public CASyncLoadedImage(Image image, string prefabPath, bool needCached, bool unloadBelongedAssetBundleAfterLoaded, bool isShowSpecMatrial = false)
            {
                m_image = image;
                m_prefabPath = prefabPath;
                m_needCached = needCached;
                m_unloadBelongedAssetBundleAfterLoaded = unloadBelongedAssetBundleAfterLoaded;
                m_isShowSpecMatrial = isShowSpecMatrial;
            }
        }


        /// <summary>
        /// UI设计时的大小
        /// </summary>
        public Vector2 GetDesignSize()
        {
            return this.m_referenceResolution;
        }

        /// <summary>
        /// UI缩放后的大小
        /// </summary>
        public Vector2 GetViewSize()
        {
            var screenSize = ResolutionAdapter.GetScreenSize();
            float width = screenSize.x;
            float height = screenSize.y;
            var design = this.m_referenceResolution;
            var size = new Vector2(1, 1);
            switch (this.canvasScaler.uiScaleMode)
            {
                case CanvasScaler.ScaleMode.ScaleWithScreenSize:
                    {
                        switch (this.canvasScaler.screenMatchMode)
                        {
                            case CanvasScaler.ScreenMatchMode.MatchWidthOrHeight:
                                {

                                    float f = this.canvasScaler.matchWidthOrHeight;

                                    if (f == 0f)
                                    {
                                        size = new Vector2(design.x, (design.x * height) / width);
                                    }
                                    else if (f == 1f)
                                    {
                                        size = new Vector2((design.y * width) / height, design.y);
                                    }
                                    else
                                    {
                                        float minScale = width / design.x;
                                        float maxScale = height / design.y;

                                        float k = Mathf.Lerp(minScale, maxScale, f);
                                        size = new Vector2(width / k, height / k);
                                    }
                                }
                                break;
                            case CanvasScaler.ScreenMatchMode.Expand:
                                {
                                    var fh = width / design.x;
                                    var fv = height / design.y;
                                    if (fh < fv)
                                    {
                                        size = new Vector2(design.x, height / width * design.x);
                                    }
                                    else
                                    {
                                        size = new Vector2(width / height * design.y, design.y);
                                    }
                                }
                                break;
                            case CanvasScaler.ScreenMatchMode.Shrink:
                                {
                                    var fh = width / design.x;
                                    var fv = height / design.y;
                                    if (fh > fv)
                                    {
                                        size = new Vector2(design.x, height / width * design.x);
                                    }
                                    else
                                    {
                                        size = new Vector2(width / height * design.y, design.y);
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
            return size;
        }

        /// <summary>
        /// 设置分辨率 * scale = 实际分辨率
        /// </summary>
		public Vector2 GetScale()
        {
            var s = this.GetViewSize();
            var r = this.m_referenceResolution;
            return new Vector2(s.x / r.x, s.y / r.y);
        }

        public Vector2 Pixel2View(Vector2 vec)
        {
            var viewSize = this.GetViewSize();
            var screenSize = ResolutionAdapter.GetScreenSize();
            vec.x = vec.x / screenSize.x * viewSize.x;
            vec.y = vec.y / screenSize.y * viewSize.y;
            return vec;
        }
        public float yPixel2View(float y)
        {
            var viewSize = this.GetViewSize();
            var screenSize = ResolutionAdapter.GetScreenSize();
            y = y / screenSize.y * viewSize.y;
            return y;
        }
        public float xPixel2View(float x)
        {
            var viewSize = this.GetViewSize();
            var screenSize = ResolutionAdapter.GetScreenSize();
            x = x / screenSize.x * viewSize.x;
            return x;
        }
    }
    public enum enFormPriority
    {
        Priority0,
        Priority1,
        Priority2,
        Priority3,
        Priority4,
        Priority5,
        Priority6,
        Priority7,
        Priority8,
        Priority9,
        Loading
    }


    public enum enFormFadeAnimationType
    {
        None,
        Animation,
        Animator
    }
    public enum enFormEventType
    {
        Open,
        Close
    }
    public enum enFormHideFlag
    {
        HideByCustom = 1,
        HideByOtherForm
    }

}

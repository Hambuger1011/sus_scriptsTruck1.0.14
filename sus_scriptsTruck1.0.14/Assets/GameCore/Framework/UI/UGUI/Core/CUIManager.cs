using Object= UnityEngine.Object;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GameLogic;
using Framework;


namespace UGUI
{
	public class CUIManager : Framework.CSingleton<CUIManager>
    {
        /// <summary>
        /// 分辨率
        /// </summary>
        public static Vector2 s_resolution = new Vector2(750, 1334); //new Vector2(720,1280);

        public static float c_renderFPS = 60;
        private static readonly int s_formCameraDepth = 10;
        private static readonly int s_orthographicSize = 5;

        
        private static readonly string s_uiCameraName = "UICamera";

        public delegate void OnFormSorted(List<CUIForm> inForms);

		private const int c_formCameraDepth = 10;
        /// <summary>
        /// form列表
        /// </summary>
        private List<CUIForm> m_forms;
        /// <summary>
        /// form缓存池
        /// </summary>
        private List<CUIForm> m_pooledForms;

		private int m_formSequence;
        /// <summary>
        /// 当前form序列列表
        /// </summary>
        private List<int> m_existFormSequences;

		private GameObject m_uiRoot;
        /// <summary>
        /// ui层级变化
        /// </summary>
        public OnFormSorted onFormSorted;

		public static int s_uiSystemRenderFrameCounter;

		private EventSystem m_uiInputEventSystem;

		private Camera m_uiCamera;
        /// <summary>
        /// 是否需要重新计算ui层级
        /// </summary>
        private bool m_needSortForms;
        /// <summary>
        /// 是否需要更新ui raycaster和隐藏
        /// </summary>
        private bool m_needUpdateRaycasterAndHide;

		private int _curMaxExchangeCount = 20;
        

		private ushort _curValidIndex;

		private uint _price;

		private uint _totalPirce;

		private uint m_searchHandlerCMD;

		private uint m_recommendHandlerCMD;

		private GameObject m_searchResultGo;

		private GameObject m_searchRecommendGo;

		private string m_searchEvtCallBack;

		private string m_recommendEvtCallBack;

		private string m_recommendEvtCallBackSingleEnable;

		private Vector2 m_searchBoxOrgSizeDetla;

		private float m_deltaSearchResultHeight;
        

		public Rect m_centerScreenRect = new Rect(0f, 0f, 1f, 1f);

	
		public Camera uiCamera
		{
			get
			{
				return m_uiCamera;
			}
		}

        #region 初始化
        protected override void Init()
		{
			m_forms = new List<CUIForm>();
			m_pooledForms = new List<CUIForm>();
			m_formSequence = 0;
			m_existFormSequences = new List<int>();
			s_uiSystemRenderFrameCounter = 0;
			CreateUIRoot();
			CreateEventSystem();
			CreateCamera();
			SetCenterScreenRect();
			CuiFormsList=new List<CUIForm>();
		}

        protected override void UnInit()
        {
            base.UnInit();
        }

        private void CreateUIRoot()
        {
            this.m_uiRoot = new GameObject("UIRoot");
            GameObject.DontDestroyOnLoad(this.m_uiRoot);
            this.m_uiRoot.transform.localPosition = Vector3.one * 1000;
        }

        private void CreateEventSystem()
        {
            m_uiInputEventSystem = EventSystem.current;//Object.FindObjectOfType<EventSystem>();
            if (m_uiInputEventSystem == null)
            {
                GameObject gameObject = new GameObject("EventSystem");
                m_uiInputEventSystem = gameObject.AddComponent<EventSystem>();
                gameObject.AddComponent<StandaloneInputModule>();
            }
            m_uiInputEventSystem.gameObject.transform.parent = m_uiRoot.transform;
        }

        private void CreateCamera()
        {
            GameObject go = new GameObject(s_uiCameraName);
            go.transform.SetParent(m_uiRoot.transform, true);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            Camera camera = go.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = s_orthographicSize;
            camera.clearFlags = CameraClearFlags.Depth;//空白部分不渲染任何颜色
            camera.cullingMask = 32;//UI(6)
            camera.depth = s_formCameraDepth;
            m_uiCamera = camera;
        }



        private void SetCenterScreenRect()
        {
            int width = Screen.width;
            int height = Screen.height;
            int doubleHeight = height * 2;
            if (width > doubleHeight)//分辨率奇葩，两边留黑边
            {
                float tan = (float)doubleHeight / (float)width;
                float ratio = (1f - tan) / 2f;
                m_centerScreenRect.x = ratio * (float)width;
                m_centerScreenRect.y = 0f;
                m_centerScreenRect.width = (float)doubleHeight;
                m_centerScreenRect.height = (float)height;
            }
            else
            {
                m_uiCamera.rect = new Rect(0f, 0f, 1f, 1f);
                m_centerScreenRect.x = 0f;
                m_centerScreenRect.y = 0f;
                m_centerScreenRect.width = (float)width;
                m_centerScreenRect.height = (float)height;
            }
        }
        #endregion

        public void Update()
		{
			int idx = 0;
			while (idx < m_forms.Count)
			{
				m_forms[idx].CustomUpdate();
				if (m_forms[idx].IsNeedClose())
				{
					if (m_forms[idx].TurnToClosed(false))//如果不播放动画直接关闭
                    {
						RecycleForm(idx);
						m_needSortForms = true;
						continue;
					}
				}
				else if (m_forms[idx].IsClosed() && !m_forms[idx].IsInFadeOut())//等待关闭动画结束
                {
					RecycleForm(idx);
					m_needSortForms = true;
					continue;
				}
				idx++;
			}
			if (m_needSortForms)
			{
				ProcessFormList(true, true);
			}
			else if (m_needUpdateRaycasterAndHide)
			{
				ProcessFormList(false, true);
			}
			m_needSortForms = false;
			m_needUpdateRaycasterAndHide = false;
		}

		public void LateUpdate()
		{
			for (int i = 0; i < m_forms.Count; i++)
			{
				m_forms[i].CustomLateUpdate();
			}
			s_uiSystemRenderFrameCounter++;
		}


        #region 加载UI
        public CUIForm OpenForm(string formPath, bool useFormPool = false, bool useCameraRenderMode = true, bool bReopen = false)
		{
			LOG.Info("OpenForm----"+formPath);
            CUIForm uiForm = this.OpenUnCloseForm(formPath, bReopen);
            if(uiForm == null)
            {
                uiForm = this.CreateForm(formPath, useFormPool);
                OnCreateNewForm(uiForm, formPath, useFormPool, useCameraRenderMode);
            }
            uiForm.m_useFormPool |= useFormPool;
            return uiForm;
        }

        void OnCreateNewForm(CUIForm uiForm, string formPath, bool useFormPool = false, bool useCameraRenderMode = true)
        {
            uiForm.gameObject.SetActiveEx(true);
            //string formName = this.GetFormName(formPath);
            //gameObject.name = formName;
            if (uiForm.transform.parent != this.m_uiRoot.transform)
            {
                uiForm.transform.SetParent(this.m_uiRoot.transform, false);
            }

            this.m_formSequence++;
            this.AddToExistFormSequenceList(this.m_formSequence);
            int formOpenOrder = this.GetFormOpenOrder(this.m_formSequence);
            this.m_forms.Add(uiForm);
            this.CuiFormsList.Add(uiForm);
            uiForm.Open(formPath, (!useCameraRenderMode) ? null : this.m_uiCamera, this.m_formSequence, false, formOpenOrder);
            if (uiForm.m_group > 0)
            {
                CloseGroupForm(uiForm.m_group);
            }
            this.m_needSortForms = true;
            uiForm.Appear(enFormHideFlag.HideByCustom, true);
        }

        CUIForm OpenUnCloseForm(string formPath, bool bReopen)
        {
            CUIForm uiForm = this.GetUnClosedForm(formPath);
            if (bReopen && uiForm != null && uiForm.m_isSingleton)
            {
                this.RemoveFromExistFormSequenceList(uiForm.GetSequence());
                this.m_formSequence++;
                this.AddToExistFormSequenceList(this.m_formSequence);
                int formOpenOrder = this.GetFormOpenOrder(this.m_formSequence);
                uiForm.Open(this.m_formSequence, true, formOpenOrder);
                this.m_needSortForms = true;
                return uiForm;
            }
            return uiForm;
        }

        private CUIForm CreateForm(string formPrefabPath, bool useFormPool)
        {
            CUIForm uiForm = GetFormPool(formPrefabPath);
            if(uiForm == null)
            {
                uiForm = CUIForm.LoadRes(formPrefabPath);
                uiForm.m_useFormPool = useFormPool;
            }
            return uiForm;
        }

        CUIForm GetFormPool(string formPrefabPath)
        {
            CUIForm uiForm = null;
            int idx = 0;
            while (idx < m_pooledForms.Count)
            {
                if (!string.Equals(formPrefabPath, m_pooledForms[idx].m_formPath, StringComparison.OrdinalIgnoreCase))
                {
                    idx++;
                    continue;
                }
                uiForm = m_pooledForms[idx];
                m_pooledForms.RemoveAt(idx);
                break;
            }
            return uiForm;
        }

        private string GetFormName(string formPath)
        {
            return CFileManager.EraseExtension(CFileManager.GetFullName(formPath));
        }
        #endregion


        #region 关闭UI
        public void CloseForm(string formPath)
		{
			for (int i = 0; i < m_forms.Count; i++)
			{
				if (string.Equals(m_forms[i].m_formPath, formPath))
				{
					m_forms[i].Close();
				}
			}
		}

		public void CloseForm(CUIForm formScript)
		{
			for (int i = 0; i < m_forms.Count; i++)
			{
				if (m_forms[i] == formScript)
				{
					m_forms[i].Close();
				}
			}
		}

		public void CloseForm(int formSequence)
		{
			for (int i = 0; i < m_forms.Count; i++)
			{
				if (m_forms[i].GetSequence() == formSequence)
				{
					m_forms[i].Close();
				}
			}
		}

		

		public void CloseAllForm(string[] exceptFormNames = null, bool closeImmediately = true, bool clearFormPool = true)
		{
			for (int i = 0; i < m_forms.Count; i++)
			{
                if (string.Equals(m_forms[i].m_formPath, UIFormName.PopupTipsForm))
                {
                    continue;
                }

                if (string.Equals(m_forms[i].m_formPath, UIFormName.UIUpdateModule))
                {
                    continue;
                }

                bool flag = true;
				if (exceptFormNames != null)
				{
					int num = 0;
					while (num < exceptFormNames.Length)
					{
						if (!string.Equals(m_forms[i].m_formPath, exceptFormNames[num]))
						{
							num++;
							continue;
						}
						flag = false;
						break;
					}
				}
				if (flag)
				{
					m_forms[i].Close();
				}
			}
			if (closeImmediately)
			{
				int num2 = 0;
				while (num2 < m_forms.Count)
				{
					if (m_forms[num2].IsNeedClose() || m_forms[num2].IsClosed())
					{
						if (m_forms[num2].IsNeedClose())
						{
							m_forms[num2].TurnToClosed(true);
						}
						RecycleForm(num2);
					}
					else
					{
						num2++;
					}
				}
				if (exceptFormNames != null)
				{
					ProcessFormList(true, true);
				}
			}
			if (clearFormPool)
			{
				ClearFormPool();
			}
		}


        public void CloseGroupForm(int group)
        {
            if (group != 0)
            {
                for (int i = 0; i < m_forms.Count; i++)
                {
                    if (m_forms[i].m_group == group)
                    {
                        m_forms[i].Close();
                    }
                }
            }
        }

        private void RecycleForm(int formIndex)
        {
            var uiForm = m_forms[formIndex];
            RemoveFromExistFormSequenceList(uiForm.GetSequence());
			RecycleForm(uiForm);
			m_forms.RemoveAt(formIndex);
		}

		public void AddToExistFormSequenceList(int formSequence)
		{
			if (m_existFormSequences != null)
			{
				m_existFormSequences.Add(formSequence);
			}
		}

		public void RemoveFromExistFormSequenceList(int formSequence)
		{
			if (m_existFormSequences != null)
			{
				m_existFormSequences.Remove(formSequence);
			}
		}


        public void ClearFormPool()
        {
            for (int i = 0; i < m_pooledForms.Count; i++)
            {
                m_pooledForms[i].UnInitialize();
            }
            m_pooledForms.Clear();
        }

        private void RecycleForm(CUIForm formScript)
        {
            if (formScript == null)
            {
                return;
            }

            if (formScript.m_useFormPool)
            {
                formScript.Hide(enFormHideFlag.HideByCustom, true);
                m_pooledForms.Add(formScript);
            }
            else
            {
                try
                {
                    if (formScript.canvasScaler != null)
                    {
                        formScript.canvasScaler.enabled = false;
                    }
                    formScript.SetCanvasEnable(false);
                    formScript.UnInitialize();
                    GameObject.Destroy(formScript.gameObject);
                }
                catch (Exception ex)
                {
                    LOG.Assert(false, "Error destroy {0} formScript gameObject: message: {1}, callstack: {2}", formScript.name, ex.Message, ex.StackTrace);
                }
            }
        }
        #endregion


        #region 获取UI
        public int GetFormOpenOrder(int formSequence)
		{
			int findIdx = m_existFormSequences.IndexOf(formSequence);
			return (findIdx >= 0) ? (findIdx + 1) : 0;
		}

		public bool HasForm()
		{
			return m_forms.Count > 0;
		}

		public CUIForm GetForm(string formPath)
		{
			for (int i = 0; i < m_forms.Count; i++)
			{
				if (m_forms[i].m_formPath.Equals(formPath) && !m_forms[i].IsNeedClose() && !m_forms[i].IsClosed())
				{
					return m_forms[i];
				}
			}
			return null;
		}
        
        public T GetForm<T>(string formPath)
        {
            var uiForm = GetForm(formPath);
            if(uiForm == null)
            {
                return default(T);
            }
            return uiForm.GetComponent<T>();
        }

        public CUIForm GetForm(int formSequence)
		{
			for (int i = 0; i < m_forms.Count; i++)
			{
				if (m_forms[i].GetSequence() == formSequence && !m_forms[i].IsNeedClose() && !m_forms[i].IsClosed())
				{
					return m_forms[i];
				}
			}
			return null;
		}

		public CUIForm GetFormByInstanceID(int instID)
		{
			for (int i = 0; i < m_forms.Count; i++)
			{
				if (m_forms[i].gameObject.GetInstanceID() == instID && !m_forms[i].IsNeedClose() && !m_forms[i].IsClosed())
				{
					return m_forms[i];
				}
			}
			return null;
		}

        public CUIForm GetTopForm(bool containNonRaycaster = true)
        {
            CUIForm cUIFormScript = null;
            for (int i = 0; i < m_forms.Count; i++)
            {
                if (!(m_forms[i] == null))
                {
                    GraphicRaycaster component = ((Component)m_forms[i]).GetComponent<GraphicRaycaster>();
                    bool flag = component != null && component.IsActive();
                    if (cUIFormScript == null)
                    {
                        cUIFormScript = m_forms[i];
                    }
                    else if (m_forms[i].GetSortingOrder() > cUIFormScript.GetSortingOrder())
                    {
                        if (containNonRaycaster)
                        {
                            cUIFormScript = m_forms[i];
                        }
                        else if (!flag)
                        {
                            if (m_forms[i].m_isExceptionOfRaycastSorting)
                            {
                                cUIFormScript = m_forms[i];
                            }
                        }
                        else if (!m_forms[i].m_isExceptionOfRaycastSorting)
                        {
                            cUIFormScript = m_forms[i];
                        }
                    }
                }
            }
            return cUIFormScript;
        }

        public List<CUIForm> GetForms()
        {
            return m_forms;
        }
        public List<CUIForm> CuiFormsList;
        private CUIForm _UIFormScript = null;
        public CUIForm GetBackUIForm()
        {
            if (_UIFormScript != null)
            {
                return _UIFormScript;
            }

            int len = this.CuiFormsList.Count;
            if (len > 0)
            {
                for (int i = len - 1; i >= 0; i--)
                {
                    if (!this.m_forms.Contains(this.CuiFormsList[i]))
                    {
                        this.CuiFormsList.Remove(this.CuiFormsList[i]);
                    }
                }
                len = this.CuiFormsList.Count;

                for (int j = len - 1; j >= 0; j--)
                {
                    _UIFormScript = this.CuiFormsList[j];

                    if (_UIFormScript.uistyle == EnumUIStyle.Null || _UIFormScript.uistyle == EnumUIStyle.None)
                    {
                        this.CuiFormsList.Remove(_UIFormScript);
                    }
                }

                int nlen = this.CuiFormsList.Count;
                for (int n = nlen - 1; n >= 0; n--)
                {
                    _UIFormScript = this.CuiFormsList[n];
                    if (_UIFormScript.uistyle == EnumUIStyle.PopWindowUI)
                    {
                        _UIFormScript = this.CuiFormsList[n];
                        return _UIFormScript;
                    }
                }
                for (int m = nlen - 1; m >= 0; m--)
                {
                    _UIFormScript = this.CuiFormsList[m];
                    if (_UIFormScript.uistyle == EnumUIStyle.TwoUI)
                    {
                        _UIFormScript = this.CuiFormsList[m];
                        return _UIFormScript;
                    }
                }

                for (int v = nlen - 1; v >= 0; v--)
                {
                    _UIFormScript = this.CuiFormsList[v];
                    if (_UIFormScript.uistyle == EnumUIStyle.Special)
                    {
                        _UIFormScript = this.CuiFormsList[v];
                        return _UIFormScript;
                    }
                }

            }
            return _UIFormScript;
        }
        public void ClearBackUIForm()
        {
            if (this.CuiFormsList.Contains(_UIFormScript))
            {
                if (_UIFormScript.uistyle == EnumUIStyle.TwoUI || _UIFormScript.uistyle == EnumUIStyle.PopWindowUI)
                {
                    this.CuiFormsList.Remove(_UIFormScript);
                }
            }
            _UIFormScript = null;
        }

        private CUIForm GetUnClosedForm(string formPath)
        {
            for (int i = 0; i < m_forms.Count; i++)
            {
                var form = m_forms[i];
                if (form.m_formPath.Equals(formPath) && !form.IsClosed())
                {
                    return form;
                }
            }
            return null;
        }
        #endregion



        #region 更新维护UI
        private void ProcessFormList(bool sort, bool handleInputAndHide)
		{
			if (sort)
			{
				m_forms.Sort();
				for (int i = 0; i < m_forms.Count; i++)
                {
                    var uiForm = m_forms[i];
                    int formOpenOrder = GetFormOpenOrder(uiForm.GetSequence());
                    uiForm.SetDisplayOrder(formOpenOrder);
#if UNITY_EDITOR
                    uiForm.transform.SetSiblingIndex(i);
#endif
                }
            }
			if (handleInputAndHide)
			{
				UpdateFormHided();
				UpdateFormRaycaster();
			}
			if (onFormSorted != null)
			{
				onFormSorted(m_forms);
			}
		}


        private void UpdateFormHided()
        {
            bool isHide = false;
            for (int idx = m_forms.Count - 1; idx >= 0; idx--)
            {
                var uiForm = m_forms[idx];
                if (isHide)
                {
                    uiForm.Hide(enFormHideFlag.HideByOtherForm, false);
                }
                else
                {
                    uiForm.Appear(enFormHideFlag.HideByOtherForm, false);
                }
                if (!isHide && !uiForm.IsHided() && uiForm.m_hideUnderForms)
                {
                    isHide = true;
                }
            }
        }


        private void UpdateFormRaycaster()
        {
            bool flag = true;
            for (int idx = m_forms.Count - 1; idx >= 0; idx--)
            {
                var uiForm = m_forms[idx];
                if (!uiForm.m_disableInput && !uiForm.IsHided())
                {
                    GraphicRaycaster graphicRaycaster = uiForm.GetGraphicRaycaster();
                    if (graphicRaycaster != null)
                    {
                        graphicRaycaster.enabled = flag;
                    }
                    if (uiForm.m_isModal && flag)
                    {
                        flag = false;
                    }
                }
            }
        }
        #endregion





        public void DisableInput()
        {
            if (m_uiInputEventSystem != null)
            {
                m_uiInputEventSystem.gameObject.SetActiveEx(false);
            }
        }

        public void EnableInput()
        {
            if (m_uiInputEventSystem != null)
            {
                m_uiInputEventSystem.gameObject.SetActiveEx(true);
            }
        }

        /// <summary>
        /// 清除所有ui图像数据
        /// </summary>
        public void ClearEventGraphicsData()
        {
            System.Reflection.MemberInfo[] member = typeof(GraphicRaycaster).GetMember("s_SortedGraphics", BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.NonPublic);
            if (member != null && member.Length == 1)
            {
                System.Reflection.MemberInfo memberInfo = member[0];
                if (memberInfo != null && memberInfo.MemberType == MemberTypes.Field)
                {
                    FieldInfo fieldInfo = memberInfo as FieldInfo;
                    if (fieldInfo != null)
                    {
                        List<Graphic> list = fieldInfo.GetValue(null) as List<Graphic>;
                        if (list != null)
                        {
                            list.Clear();
                        }
                    }
                }
            }
        }


        public EventSystem GetEventSystem()
        {
            return m_uiInputEventSystem;
        }

        public void ResetAllFormHideOrShowState()
        {
            m_needUpdateRaycasterAndHide = true;
        }

        /// <summary>
        /// 获取一个ui在屏幕上的rect
        /// </summary>
		public Rect GetRect(GameObject go)
		{
			Rect result = new Rect();
			RectTransform rectTrans = go.GetComponent<RectTransform>();
			if (rectTrans != null)
			{
				Vector3[] worldCorners = new Vector3[4];
				rectTrans.GetWorldCorners(worldCorners);//0左下、1左上、2右上、3右下
                Camera formCamera = uiCamera;
				Vector2 tr = CUIUtility.WorldToScreenPoint(formCamera, worldCorners[2]);
				Vector2 bl = CUIUtility.WorldToScreenPoint(formCamera, worldCorners[0]);
				result.width = Math.Abs(tr.x - bl.x);
				result.height = Math.Abs(tr.y - bl.y);

				Vector2 tl = CUIUtility.WorldToScreenPoint(formCamera, worldCorners[1]);
				result.x = tl.x;
				result.y = (float)Screen.height - tl.y;

				Vector3 localScale = go.transform.localScale;
				if (localScale.x < 0f)
				{
					result.x -= result.width;
				}
				if (localScale.y < 0f)
				{
					result.y -= result.height;
				}
			}
			return result;
		}
        
        

		private void OnScreenResolutionChanged()
		{
			for (int i = 0; i < m_forms.Count; i++)
			{
				m_forms[i].OnScreenResolutionChanged();
			}
			for (int j = 0; j < m_pooledForms.Count; j++)
			{
				m_pooledForms[j].OnScreenResolutionChanged();
			}
			SetCenterScreenRect();
		}
	}
}

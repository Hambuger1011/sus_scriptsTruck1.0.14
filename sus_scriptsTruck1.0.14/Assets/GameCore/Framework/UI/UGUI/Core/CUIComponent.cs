using Framework;

using UnityEngine;

namespace UGUI
{
    public enum enCustomUpdateFlag
    {
        eUpdate = 1,
        eLateUpdate = 2,
        eAll = 3
    }

	public class CUIComponent : MonoBehaviour
	{
		[HideInInspector]
		public CUIForm myForm;

		[HideInInspector]
		public CUIList m_belongedListScript;

		[HideInInspector]
		public int m_indexInlist;

		protected bool m_isInitialized;

		protected enCustomUpdateFlag m_customUpdateFlags;

        protected virtual void Awake()
        {
            if(!m_isInitialized)
            {
                this.myForm = this.GetComponentInParent<CUIForm>();
                Initialize(myForm);
            }
        }
        protected virtual void OnDestroy()
        {
            UnInitialize();
        }

        public virtual void Initialize(CUIForm formScript)
		{
			if (!m_isInitialized)
			{
				myForm = formScript;
				if (myForm != null)
				{
					myForm.AddUIComponent(this);
					SetSortingOrder(myForm.GetSortingOrder());
				}
				m_isInitialized = true;
			}
		}

		public virtual void UnInitialize()
		{
			if (m_isInitialized)
			{
				if (myForm != null)
				{
					myForm.RemoveUIComponent(this);
				}
				myForm = null;
				m_belongedListScript = null;
				m_isInitialized = false;
			}
		}



        public virtual void OnOpen()
        {

        }

        public virtual void OnClose()
		{
		}

		public virtual void OnHide()
		{
		}

		public virtual void OnAppear()
		{
		}

		public virtual void SetSortingOrder(int sortingOrder)
		{
		}

        /// <summary>
        /// 如果需要调用该函数,m_customUpdateFlags |= 1;
        /// </summary>
		public virtual void CustomUpdate()
		{
		}

        /// <summary>
        /// 如果需要调用该函数,m_customUpdateFlags |= 3;
        /// </summary>
        public virtual void CustomLateUpdate()
		{
		}

#if UNITY_EDITOR
        [System.Obsolete("使用CustomUpdate", true)]
        public void Update()
        {
        }

        [System.Obsolete("使用LateUpdate", true)]
        public void LateUpdate()
        {
        }
#endif

        public void SetBelongedList(CUIList belongedListScript, int index)
		{
			m_belongedListScript = belongedListScript;
			m_indexInlist = index;
		}

		protected T GetComponentInChildren<T>(GameObject go) where T : Component
		{
			T component = go.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			for (int i = 0; i < go.transform.childCount; i++)
			{
				component = GetComponentInChildren<T>(go.transform.GetChild(i).gameObject);
				if (component != null)
				{
					return component;
				}
			}
			return (T)null;
		}

		protected GameObject Instantiate(GameObject srcGameObject)
		{
			GameObject gameObject = Object.Instantiate(srcGameObject) as GameObject;
			gameObject.transform.SetParent(srcGameObject.transform.parent);
			RectTransform rectTransform = srcGameObject.transform as RectTransform;
			RectTransform rectTransform2 = gameObject.transform as RectTransform;
			if (rectTransform != null && rectTransform2 != null)
			{
				rectTransform2.pivot = rectTransform.pivot;
				rectTransform2.anchorMin = rectTransform.anchorMin;
				rectTransform2.anchorMax = rectTransform.anchorMax;
				rectTransform2.offsetMin = rectTransform.offsetMin;
				rectTransform2.offsetMax = rectTransform.offsetMax;
				rectTransform2.localPosition = rectTransform.localPosition;
				rectTransform2.localRotation = rectTransform.localRotation;
				rectTransform2.localScale = rectTransform.localScale;
			}
			return gameObject;
		}

		public static GameObject DuplicateGO(GameObject srcGameObject)
		{
			GameObject gameObject = Object.Instantiate(srcGameObject) as GameObject;
			gameObject.transform.SetParent(srcGameObject.transform.parent);
			RectTransform rectTransform = srcGameObject.transform as RectTransform;
			RectTransform rectTransform2 = gameObject.transform as RectTransform;
			if (rectTransform != null && rectTransform2 != null)
			{
				rectTransform2.pivot = rectTransform.pivot;
				rectTransform2.anchorMin = rectTransform.anchorMin;
				rectTransform2.anchorMax = rectTransform.anchorMax;
				rectTransform2.offsetMin = rectTransform.offsetMin;
				rectTransform2.offsetMax = rectTransform.offsetMax;
				rectTransform2.localPosition = rectTransform.localPosition;
				rectTransform2.localRotation = rectTransform.localRotation;
				rectTransform2.localScale = rectTransform.localScale;
			}
			return gameObject;
		}

		protected void InitializeComponent(GameObject root)
		{
			CUIComponent[] componentsInChildren = root.GetComponentsInChildren<CUIComponent>(true);
			if (componentsInChildren != null && componentsInChildren.Length > 0)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].Initialize(myForm);
				}
			}
		}

		public bool HasCustomUpdateFlag(enCustomUpdateFlag customUpdateFlag)
		{
			return (m_customUpdateFlags & customUpdateFlag) != 0;
		}
	}
}

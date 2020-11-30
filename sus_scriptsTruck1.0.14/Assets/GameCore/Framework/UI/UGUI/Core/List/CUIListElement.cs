using Framework;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
namespace UGUI
{

    public class CUIListElement : CUIComponent
	{
		public delegate void OnSelectedDelegate();
		[HideInInspector]
		public Vector2 m_defaultSize;

		[HideInInspector]
		public int m_index;

		[HideInInspector]
		public enPivotType m_pivotType = enPivotType.LeftTop;

		public stRect m_rect;

        [Header("隐藏时是否直接Deactive")]
		public bool m_useSetActiveForDisplay = true;

		public bool m_autoAddUIEventScript = false;
        

		public OnSelectedDelegate onSelected;
        
        private CanvasGroup m_canvasGroup;

        public override void Initialize(CUIForm formScript)
		{
			if (!this.m_isInitialized)
			{
                base.Initialize(formScript);
				if (m_autoAddUIEventScript)
				{
					CUIEventScript component = base.gameObject.GetComponent<CUIEventScript>();
					if (component == null)
					{
						component = base.gameObject.AddComponent<CUIEventScript>();
						component.Initialize(formScript);
					}
				}
				if (!m_useSetActiveForDisplay)
				{
					m_canvasGroup = base.gameObject.GetComponent<CanvasGroup>();
					if (m_canvasGroup == null)
					{
						m_canvasGroup = base.gameObject.AddComponent<CanvasGroup>();
					}
				}
				m_defaultSize = GetDefaultSize();
				InitRectTransform();
				base.m_isInitialized = true;
			}
		}

		public override void UnInitialize()
		{
			if (base.m_isInitialized)
			{
				onSelected = null;
				m_canvasGroup = null;
				base.UnInitialize();
				base.m_isInitialized = false;
			}
		}

		protected override void OnDestroy()
		{
			UnInitialize();
			base.OnDestroy();
		}

		protected virtual Vector2 GetDefaultSize()
		{
			return new Vector2(((RectTransform)base.gameObject.transform).rect.width, ((RectTransform)base.gameObject.transform).rect.height);
		}

        
		public void Enable(CUIList belongedList, int index, string name, ref stRect rect, bool selected)
		{
			base.m_belongedListScript = belongedList;
			m_index = index;

           
            base.gameObject.name = name + "_" + index.ToString();

            //base.gameObject.transform.localScale = Vector3.zero;

            if (m_useSetActiveForDisplay)
			{
				base.gameObject.SetActiveEx(true);
			}
			else
			{
				m_canvasGroup.alpha = 1f;
				m_canvasGroup.blocksRaycasts = true;
			}
			SetComponentBelongedList(base.gameObject);
			SetRect(ref rect);
			ChangeDisplay(selected);
		}
       
        public void Disable()
		{
			if (m_useSetActiveForDisplay)
			{
                base.gameObject.SetActiveEx(false);

                //if (INSTANCE.Instance.returnto())
                //{
                //    //Debug.Log("True");
                    
                //    Tween t = base.gameObject.transform.DOScale(new Vector3(0, 0, 0), 0.5f);
                //    t.OnComplete(CustomGameToFalse);
                //}else
                //{
                //    //Debug.Log("false");
                //    base.gameObject.SetActiveEx(false);
                //}
               
            }
			else
			{
				m_canvasGroup.alpha = 0f;
				m_canvasGroup.blocksRaycasts = false;
			}
		}

        private void CustomGameToFalse()
        {
            base.gameObject.SetActiveEx(false);
        }
		public void OnSelected(BaseEventData baseEventData)
		{
			base.m_belongedListScript.SelectElement(m_index, true);
		}

		public virtual void ChangeDisplay(bool selected)
		{
		}

		public void SetComponentBelongedList(GameObject gameObject)
		{
			CUIComponent[] components = gameObject.GetComponents<CUIComponent>();
			if (components != null && components.Length > 0)
			{
				for (int i = 0; i < components.Length; i++)
				{
					components[i].SetBelongedList(base.m_belongedListScript, m_index);
				}
			}
			for (int j = 0; j < gameObject.transform.childCount; j++)
			{
				SetComponentBelongedList(gameObject.transform.GetChild(j).gameObject);
			}
		}

		public void SetRect(ref stRect rect)
		{
			m_rect = rect;
			RectTransform t = base.gameObject.transform as RectTransform;
			t.sizeDelta = new Vector2((float)m_rect.m_width, (float)m_rect.m_height);
			if (m_pivotType == enPivotType.Centre)
			{
				t.anchoredPosition = rect.m_center;
			}
			else
			{
				t.anchoredPosition = new Vector2((float)rect.m_left, (float)rect.m_top);
			}
		}

		private void InitRectTransform()
		{
			RectTransform rectTransform = base.gameObject.transform as RectTransform;
			rectTransform.anchorMin = new Vector2(0f, 1f);
			rectTransform.anchorMax = new Vector2(0f, 1f);
			rectTransform.pivot = ((m_pivotType != 0) ? new Vector2(0f, 1f) : new Vector2(0.5f, 0.5f));
			rectTransform.sizeDelta = m_defaultSize;
			rectTransform.localRotation = Quaternion.identity;
			rectTransform.localScale = new Vector3(1f, 1f, 1f);
		}
    }
    public enum enPivotType
    {
        Centre,
        LeftTop
    }
}

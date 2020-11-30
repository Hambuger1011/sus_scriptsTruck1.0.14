using Framework;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.DemiLib;
using System;

namespace UGUI
{
    public class CUIList : CUIComponent
    {
        public ScrollRect m_scrollRect;
        public Scrollbar m_scrollBar;
        public GameObject m_content;

        [Header("列表为空时显示的对象")]
        public GameObject m_ZeroTipsObj;

        /// <summary>
        /// 元素prefab
        /// </summary>
		public GameObject m_elementTemplate;

        [Header("List类型")]
		public enUIListType m_listType;

        [Header("元素数量")]
		public int m_elementAmount;

        [Header("元素间距")]
        public Vector2 m_elementSpacing;

        [Header("元素偏移")]
        public float m_elementLayoutOffset;

        protected int m_selectedElementIndex = -1;

		protected int m_lastSelectedElementIndex = -1;
        
		protected List<CUIListElement> m_usedElementList;

		protected List<CUIListElement> m_unUsedElementList;


		protected string m_elementName;

		protected Vector2 m_elementDefaultSize;

        /// <summary>
        /// 元素大小
        /// </summary>
		protected List<Vector2> m_elementsSize;

        /// <summary>
        /// 计算得到的元素rect
        /// </summary>
		protected List<stRect> m_elementsRect;
        

        [HideInInspector]
		public Vector2 m_scrollAreaSize;

		protected RectTransform m_contentRectTransform;

		protected Vector2 m_contentSize;


		protected Vector2 m_lastContentPosition;


		private int m_currentDraggedElementIndex = -1;

		private Vector2 m_contentLastAnchoredPosition = new Vector2(16777215f, 16777215f);
        
        [Header("是否使用循环列表")]
        public bool m_useOptimized;

        [Header("是否派发scroll消息")]
        public bool m_scrollDispathEvent;

		public bool m_isDisableScrollRect;

		public bool m_alwaysDispatchSelectedChangeEvent;

        [Header("元素是否居中")]
		public bool m_autoCenteredElements;

		public bool m_autoCenteredBothSides;

		public float m_fSpeed = 0.2f;

        public delegate void OnInitElement(CUIList list,int index, CUIListElement element);
        public event OnInitElement onInitElement;

        public event Action<Vector2> onResizeContent;

		public override void Initialize(CUIForm formScript)
		{
			if (!this.m_isInitialized)
			{
				this.m_customUpdateFlags |= enCustomUpdateFlag.eUpdate;
				base.Initialize(formScript);
				m_selectedElementIndex = -1;
				m_lastSelectedElementIndex = -1;
				m_currentDraggedElementIndex = -1;
				m_contentLastAnchoredPosition = new Vector2(16777215f, 16777215f);


                #region ScrollView
                if(m_scrollRect == null)
                {
                    m_scrollRect = base.GetComponentInChildren<ScrollRect>(base.gameObject);
                }
				if (m_scrollRect != null)
				{
					RectTransform t = m_scrollRect.transform as RectTransform;
                    m_scrollAreaSize = new Vector2(t.rect.width, t.rect.height);
					m_content = m_scrollRect.content.gameObject;
				}
                #endregion
                #region ScrollBar
                if(m_scrollBar == null)
                {
                    m_scrollBar = base.GetComponentInChildren<Scrollbar>(base.gameObject);
                }
				if (m_listType == enUIListType.Vertical || m_listType == enUIListType.VerticalGrid)
				{
					if (m_scrollBar != null)
					{
						m_scrollBar.direction = Scrollbar.Direction.BottomToTop;
					}
					
					if (m_scrollRect != null)
					{
						m_scrollRect.horizontal = false;
						m_scrollRect.vertical = true;
						m_scrollRect.horizontalScrollbar = null;
						m_scrollRect.verticalScrollbar = m_scrollBar;
					}
				}
				else if (m_listType == enUIListType.Horizontal || m_listType == enUIListType.HorizontalGrid)
				{
					if (m_scrollBar != null)
					{
						m_scrollBar.direction = Scrollbar.Direction.LeftToRight;
					}
					LOG.Assert(m_scrollRect != null);
					if (m_scrollRect != null)
					{
						m_scrollRect.horizontal = true;
						m_scrollRect.vertical = false;
						m_scrollRect.horizontalScrollbar = m_scrollBar;
						m_scrollRect.verticalScrollbar = null;
					}
				}
                #endregion
                m_usedElementList = new List<CUIListElement>();
				m_unUsedElementList = new List<CUIListElement>();
				if (m_useOptimized && m_elementsRect == null)
				{
					m_elementsRect = new List<stRect>();
				}
				if (m_content != null)
				{
					m_contentRectTransform = (m_content.transform as RectTransform);
					m_contentRectTransform.pivot = new Vector2(0f, 1f);
					m_contentRectTransform.anchorMin = new Vector2(0f, 1f);
					m_contentRectTransform.anchorMax = new Vector2(0f, 1f);
					m_contentRectTransform.anchoredPosition = Vector2.zero;
					m_contentRectTransform.localRotation = Quaternion.identity;
					m_contentRectTransform.localScale = new Vector3(1f, 1f, 1f);
					m_lastContentPosition = m_contentRectTransform.anchoredPosition;
				}
                SetPrefab(m_elementTemplate);
                base.m_isInitialized = true;
                this.SetCount(this.m_elementAmount, m_elementsSize);
			}
		}

		public override void UnInitialize()
		{
			if (base.m_isInitialized)
			{
#if true
                DOTween.Kill(this);
#else
                if (LeanTween.IsInitialised())
				{
					LeanTween.cancel(base.gameObject);
				}
#endif
                m_ZeroTipsObj = null;

				if (m_usedElementList != null)
				{
					m_usedElementList.Clear();
					m_usedElementList = null;
				}
				if (m_unUsedElementList != null)
				{
					m_unUsedElementList.Clear();
					m_unUsedElementList = null;
				}
				m_elementTemplate = null;
				m_elementName = null;
				if (m_elementsSize != null)
				{
					m_elementsSize.Clear();
					m_elementsSize = null;
				}
				if (m_elementsRect != null)
				{
					m_elementsRect.Clear();
					m_elementsRect = null;
				}
				m_scrollRect = null;
				m_content = null;
				m_contentRectTransform = null;
				m_scrollBar = null;
				base.UnInitialize();
				base.m_isInitialized = false;
			}
		}

		protected override void OnDestroy()
		{
			UnInitialize();
			base.OnDestroy();
		}

		public override void CustomUpdate()
		{
			if (base.myForm != null && base.myForm.IsClosed())
			{
				return;
			}
			if (m_useOptimized)
			{
				UpdateElementsScroll();
			}
			if (m_scrollRect != null && m_scrollDispathEvent && !m_isDisableScrollRect)
			{
				DetectScroll();
			}
		}

        #region 循环列表

        protected bool UpdateElementsScroll()
        {
            Vector2 offset = m_contentRectTransform.anchoredPosition;
            if (m_contentLastAnchoredPosition == offset)
            {
                return false;
            }
            m_contentLastAnchoredPosition = offset;

            #region 处理不可见的元素
            int idx = 0;
            while (idx < m_usedElementList.Count)
            {
                if (
                    !IsRectInScrollArea(ref m_usedElementList[idx].m_rect)
                    && m_usedElementList[idx].m_indexInlist != m_currentDraggedElementIndex
                    )
                {
                    RecycleElement(m_usedElementList[idx], true);
                }
                else
                {
                    idx++;
                }
            }
            #endregion

            bool result = false;
            #region 处理可见的元素
            for (int i = 0; i < m_elementAmount; i++)
            {
                stRect stRect = m_elementsRect[i];
                if (!IsRectInScrollArea(ref stRect))
                {
                    continue;
                }
                bool isFind = false;
                int itemIdx = 0;
                while (itemIdx < m_usedElementList.Count)
                {
                    if (m_usedElementList[itemIdx].m_index != i)
                    {
                        itemIdx++;
                        continue;
                    }
                    isFind = true;
                    break;
                }
                if (!isFind)//不在已经用列表中
                {
                    CreateElement(i, ref stRect);
                    result = true;
                }
            }
            #endregion
            return result;
        }
        protected bool IsRectInScrollArea(ref stRect rect)
        {
            Vector2 pos = Vector2.zero;
            Vector2 offset = m_contentRectTransform.anchoredPosition;
            pos.x = offset.x + rect.m_left;
            pos.y = offset.y + rect.m_top;
            if (pos.x + rect.m_width >= 0f
                && pos.x <= m_scrollAreaSize.x
                && pos.y - rect.m_height <= 0f
                && pos.y >= -m_scrollAreaSize.y
                )
            {
                return true;
            }
            return false;
        }

        private string rootName;
        CUIListElement c = null;
        protected CUIListElement CreateElement(int index, ref stRect rect)
        {
            //Debug.Log("Index:" + index);
            rootName = transform.parent.name;

            if (rootName.Equals("MyBooks"))
            {
            }
            else
            {
                //CUIListElement c = null;
                c = null;
                if (m_unUsedElementList.Count > 0)
                {
                    c = m_unUsedElementList[0];
                    m_unUsedElementList.RemoveAt(0);
                }
                else if (m_elementTemplate != null)
                {
                    GameObject go = GameObject.Instantiate(m_elementTemplate, m_content.transform);
                    //go.transform.SetParent(m_content.transform);
                    var t = go.transform;
                    t.localPosition = Vector3.zero;
                    t.localScale = Vector3.one;
                    t.localRotation = Quaternion.identity;
                    base.InitializeComponent(go);
                    c = go.GetComponent<CUIListElement>();

                }
                if (c != null)
                {
                    c.Enable(this, index, m_elementName, ref rect, IsSelectedIndex(index));
                    m_usedElementList.Add(c);
                }

                onInitElement(this, index, c);

            }



            return c;

        }

        protected void RecycleElement(bool disableElement)
        {
            while (m_usedElementList.Count > 0)
            {
                CUIListElement c = m_usedElementList[0];
                m_usedElementList.RemoveAt(0);
                if (disableElement)
                {
                    c.Disable();
                }
                m_unUsedElementList.Add(c);
            }
        }

        protected void RecycleElement(CUIListElement elementScript, bool disableElement)
        {
            if (disableElement)
            {
                elementScript.Disable();
            }
            m_usedElementList.Remove(elementScript);
            m_unUsedElementList.Add(elementScript);
        }

        #endregion

        protected void DetectScroll()
		{
			if (m_contentRectTransform.anchoredPosition != m_lastContentPosition)
			{
				m_lastContentPosition = m_contentRectTransform.anchoredPosition;
				//DispatchScrollChangedEvent();
			}
		}


        #region 元素选择事件

		public virtual void SelectElement(int index, bool isDispatchSelectedChangeEvent = true)
		{
			m_lastSelectedElementIndex = m_selectedElementIndex;
			m_selectedElementIndex = index;
			if (isDispatchSelectedChangeEvent)
			{
				base.m_indexInlist = index;
			}
			if (m_lastSelectedElementIndex == m_selectedElementIndex)
			{
				if (m_alwaysDispatchSelectedChangeEvent && isDispatchSelectedChangeEvent)
				{
					//DispatchElementSelectChangedEvent();
				}
			}
			else
			{
				if (m_lastSelectedElementIndex >= 0)
				{
					CUIListElement elemenet = GetElemenet(m_lastSelectedElementIndex);
					if (elemenet != null)
					{
						elemenet.ChangeDisplay(false);
					}
				}
				if (m_selectedElementIndex >= 0)
				{
					CUIListElement elemenet2 = GetElemenet(m_selectedElementIndex);
					if (elemenet2 != null)
					{
						elemenet2.ChangeDisplay(true);
						if (elemenet2.onSelected != null)
						{
							elemenet2.onSelected();
						}
					}
				}
				if (isDispatchSelectedChangeEvent)
				{
					//DispatchElementSelectChangedEvent();
				}
			}
		}
        #endregion

        public int GetElementAmount()
		{
			return m_elementAmount;
		}

		public CUIListElement GetElemenet(int index)
		{
			if (index >= 0 && index < m_elementAmount)
			{
				if (m_useOptimized)
				{
					for (int i = 0; i < m_usedElementList.Count; i++)
					{
						if (m_usedElementList[i].m_index == index)
						{
							return m_usedElementList[i];
						}
					}
					return null;
				}
				return m_usedElementList[index];
			}
			return null;
		}

		public CUIListElement GetSelectedElement()
		{
			return GetElemenet(m_selectedElementIndex);
		}

		public CUIListElement GetLastSelectedElement()
		{
			return GetElemenet(m_lastSelectedElementIndex);
		}

		public Vector2 GetEelementDefaultSize()
		{
			return m_elementDefaultSize;
		}

		public int GetSelectedIndex()
		{
			return m_selectedElementIndex;
		}

		public int GetLastSelectedIndex()
		{
			return m_lastSelectedElementIndex;
		}

		public bool IsElementInScrollArea(int index)
		{
			if (index >= 0 && index < m_elementAmount)
			{
				stRect stRect = (!m_useOptimized) ? m_usedElementList[index].m_rect : m_elementsRect[index];
				return IsRectInScrollArea(ref stRect);
			}
			return false;
		}

		public int GetMaxIndexInScrollArea()
		{
			int num = -1;
			if (m_elementAmount == 0)
			{
				return num;
			}
			int count = m_usedElementList.Count;
			for (int i = 0; i < count; i++)
			{
				stRect rect = m_usedElementList[i].m_rect;
				if (IsRectInScrollArea(ref rect) && m_usedElementList[i].m_index > num)
				{
					num = m_usedElementList[i].m_index;
				}
			}
			return num;
		}

		public Vector2 GetContentSize()
		{
			return m_contentSize;
		}

		public Vector2 GetScrollAreaSize()
		{
			return m_scrollAreaSize;
		}

		public Vector2 GetContentPosition()
		{
			return m_lastContentPosition;
		}

		public bool IsNeedScroll()
		{
			return m_contentSize.x > m_scrollAreaSize.x || m_contentSize.y > m_scrollAreaSize.y;
		}

		public void ResetContentPosition()
		{
#if true
            DOTween.Kill(this);
#else
            if (LeanTween.IsInitialised())
            {
                LeanTween.cancel(base.gameObject);
            }
#endif
            if (m_contentRectTransform != null)
			{
				m_contentRectTransform.anchoredPosition = Vector2.zero;
			}
		}

		public void MoveElementInScrollArea(int index, bool moveImmediately)
		{
			if (index >= 0 && index < m_elementAmount)
			{
				Vector2 delta = Vector2.zero;
				Vector2 bord = Vector2.zero;
				stRect stRect = (!m_useOptimized) ? m_usedElementList[index].m_rect : m_elementsRect[index];
				Vector2 offset = m_contentRectTransform.anchoredPosition;
				bord.x = offset.x + stRect.m_left;
				bord.y = offset.y + stRect.m_top;
				if (bord.x < 0f)
				{
					delta.x = 0f - bord.x;
				}
				else if (bord.x + stRect.m_width > m_scrollAreaSize.x)
				{
					delta.x = m_scrollAreaSize.x - (bord.x + stRect.m_width);
				}
				if (bord.y > 0f)
				{
					delta.y = 0f - bord.y;
				}
				else if (bord.y - stRect.m_height < 0f - m_scrollAreaSize.y)
				{
					delta.y = 0f - m_scrollAreaSize.y - (bord.y - stRect.m_height);
				}
				if (moveImmediately)
				{
					RectTransform contentRectTransform = m_contentRectTransform;
					contentRectTransform.anchoredPosition += delta;
				}
				else
				{
#if true
                    Vector2 to = offset + delta;
                    this.m_contentRectTransform.DOAnchorPos(to, m_fSpeed).OnComplete(()=>
                    {
                        m_contentRectTransform.anchoredPosition = to;
                    }).SetId(this);
#else
                    LeanTween.value(base.gameObject, delegate (Vector2 pos)
                    {
                        m_contentRectTransform.anchoredPosition = pos;
                    }, offset, to, m_fSpeed);
#endif
                }
            }
		}

		public virtual bool IsSelectedIndex(int index)
		{
			return m_selectedElementIndex == index;
		}
        

		public void MoveContent(Vector2 offset)
		{
			if (m_contentRectTransform != null)
			{
				Vector2 vector = (m_content.transform as RectTransform).anchoredPosition;
				vector += offset;
				if (offset.x != 0f)
				{
					if (vector.x > 0f)
					{
						vector.x = 0f;
					}
					else if (vector.x + m_contentSize.x < m_scrollAreaSize.x)
					{
						vector.x = m_scrollAreaSize.x - m_contentSize.x;
					}
				}
				if (offset.y != 0f)
				{
					if (vector.y < 0f)
					{
						vector.y = 0f;
					}
					else if (m_contentSize.y - vector.y < m_scrollAreaSize.y)
					{
						vector.y = m_contentSize.y - m_scrollAreaSize.y;
					}
				}
				m_contentRectTransform.anchoredPosition = vector;
			}
		}

		public void SetCurrentDraggedElementIndex(int elementIndexInList)
		{
			m_currentDraggedElementIndex = elementIndexInList;
		}

		


        /// <summary>
        /// ScrollView size发生变化
        /// </summary>
        public void RefreshContentSize()
		{
            if(m_scrollRect == null)
            {
                return;
            }
            RectTransform rectTransform = m_scrollRect.transform as RectTransform;
            m_scrollAreaSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
        }


#region 设置element
        /// <summary>
        /// 设置element
        /// </summary>
        public void SetPrefab(GameObject gameObject)
		{
            if(gameObject == null)
            {
                return;
            }
			CUIListElement c = gameObject.GetComponent<CUIListElement>();
            if (c != null && gameObject != null)
			{
				c.Initialize(this.myForm);
				m_elementTemplate = gameObject;
				m_elementName = gameObject.name;
				m_elementDefaultSize = c.m_defaultSize;
			}
		}

        public void SetCount(int amount)
        {
            SetCount(amount, null);
        }


        public virtual void SetCount(int amount, List<Vector2> elementsSize)
        {
            if (amount < 0)
            {
                amount = 0;
            }
            if (elementsSize != null && amount != elementsSize.Count)
            {
                return;
            }
            m_elementAmount = amount;
            m_elementsSize = elementsSize;
            if (!m_isInitialized)
            {
                return;
            }
            RecycleElement(false);
            ProcessElements();//生成element
            ProcessUnUsedElement();
            if (m_ZeroTipsObj != null)
            {
                if (amount <= 0)
                {
                    m_ZeroTipsObj.SetActive(true);
                   
                }
                else
                {
                    m_ZeroTipsObj.SetActive(false);
                }
            }

        }

        
        private GameObject needDestroyGame;
        private void DestryGmaeobje()
        {
            Destroy(needDestroyGame);
        }
        protected virtual void ProcessElements()
        {
            m_contentSize = Vector2.zero;
            Vector2 zero = Vector2.zero;
            if (m_listType == enUIListType.Vertical || m_listType == enUIListType.VerticalGrid)
            {
                zero.y += m_elementLayoutOffset;
            }
            else
            {
                zero.x += m_elementLayoutOffset;
            }
            for (int i = 0; i < m_elementAmount; i++)
            {
                stRect stRect = LayoutElement(i, ref m_contentSize, ref zero);
                if (m_useOptimized)
                {
                    if (i < m_elementsRect.Count)
                    {
                        m_elementsRect[i] = stRect;
                    }
                    else
                    {
                        m_elementsRect.Add(stRect);
                    }
                }
                if (!m_useOptimized || IsRectInScrollArea(ref stRect))
                {
                    CreateElement(i, ref stRect);
                }
            }
            ResizeContent(ref m_contentSize, false);
        }



        protected stRect LayoutElement(int index, ref Vector2 contentSize, ref Vector2 offset)
        {
            stRect result = default(stRect);
            float x;
            if (m_elementsSize == null
                || m_listType == enUIListType.Vertical
                || m_listType == enUIListType.VerticalGrid
                || m_listType == enUIListType.HorizontalGrid
                )
            {
                x = m_elementDefaultSize.x;
            }
            else
            {
                Vector2 vec = m_elementsSize[index];
                x = vec.x;
            }
            result.m_width = (int)x;
            float y;
            if (m_elementsSize == null
                || m_listType == enUIListType.Horizontal
                || m_listType == enUIListType.VerticalGrid
                || m_listType == enUIListType.HorizontalGrid
                )
            {
                y = m_elementDefaultSize.y;
            }
            else
            {
                Vector2 vec = m_elementsSize[index];
                y = vec.y;
            }
            result.m_height = (int)y;
            result.m_left = (int)offset.x;
            result.m_top = (int)offset.y;
            result.m_right = result.m_left + result.m_width;
            result.m_bottom = result.m_top - result.m_height;
            result.m_center = new Vector2(result.m_left + result.m_width * 0.5f, result.m_top - result.m_height * 0.5f);

            if (result.m_right > contentSize.x)
            {
                contentSize.x = result.m_right;
            }
            if (-result.m_bottom > contentSize.y)
            {
                contentSize.y = (-result.m_bottom);
            }

            switch (m_listType)
            {
                case enUIListType.Vertical:
                    offset.y -= result.m_height + m_elementSpacing.y;
                    break;
                case enUIListType.Horizontal:
                    offset.x += result.m_width + m_elementSpacing.x;
                    break;
                case enUIListType.VerticalGrid:
                    offset.x += result.m_width + m_elementSpacing.x;
                    if (offset.x + result.m_width > m_scrollAreaSize.x)
                    {
                        offset.x = 0f;
                        offset.y -= result.m_height + m_elementSpacing.y;
                    }
                    break;
                case enUIListType.HorizontalGrid:
                    offset.y -= result.m_height + m_elementSpacing.y;
                    if (0f - offset.y + result.m_height > m_scrollAreaSize.y)
                    {
                        offset.y = 0f;
                        offset.x += result.m_width + m_elementSpacing.x;
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// 设置content大小
        /// </summary>
		protected virtual void ResizeContent(ref Vector2 size, bool resetPosition)
        {
            float centerX = 0f;
            float centerY = 0f;

            if (m_autoCenteredElements)
            {
                if (m_listType == enUIListType.Vertical && size.y < m_scrollAreaSize.y)
                {
                    centerY = (size.y - m_scrollAreaSize.y) / 2f;
                    if (m_autoCenteredBothSides)
                    {
                        centerX = (m_scrollAreaSize.x - size.x) / 2f;
                    }
                }
                else if (m_listType == enUIListType.Horizontal && size.x < m_scrollAreaSize.x)
                {
                    centerX = (m_scrollAreaSize.x - size.x) / 2f;
                    if (m_autoCenteredBothSides)
                    {
                        centerY = (size.y - m_scrollAreaSize.y) / 2f;
                    }
                }
                else if (m_listType == enUIListType.VerticalGrid && size.x < m_scrollAreaSize.x)
                {
                    float x = size.x;
                    float w = x + m_elementSpacing.x;
                    while (true)
                    {
                        x = w + m_elementDefaultSize.x;
                        if (x <= m_scrollAreaSize.x)
                        {
                            size.x = x;
                            w = x + m_elementSpacing.x;
                            continue;
                        }
                        break;
                    }
                    centerX = (m_scrollAreaSize.x - size.x) / 2f;
                }
                else if (m_listType == enUIListType.HorizontalGrid && size.y < m_scrollAreaSize.y)
                {
                    float y = size.y;
                    float h = y + m_elementSpacing.y;
                    while (true)
                    {
                        y = h + m_elementDefaultSize.y;
                        if (y <= m_scrollAreaSize.y)
                        {
                            size.y = y;
                            h = y + m_elementSpacing.y;
                            continue;
                        }
                        break;
                    }
                    centerY = (size.y - m_scrollAreaSize.y) / 2f;
                }
            }

            if (size.x < m_scrollAreaSize.x)
            {
                size.x = m_scrollAreaSize.x;
            }
            if (size.y < m_scrollAreaSize.y)
            {
                size.y = m_scrollAreaSize.y;
            }

            if (m_contentRectTransform != null)
            {
                m_contentRectTransform.sizeDelta = size;
                if (resetPosition)
                {
                    m_contentRectTransform.anchoredPosition = Vector2.zero;
                }
                if (m_autoCenteredElements)
                {
                    if (centerX != 0f)
                    {
                        Vector2 p = m_contentRectTransform.anchoredPosition;
                        p.x = centerX;
                        m_contentRectTransform.anchoredPosition = p;
                    }
                    if (centerY != 0f)
                    {
                        Vector2 p = m_contentRectTransform.anchoredPosition;
                        p.y = centerY;
                        m_contentRectTransform.anchoredPosition = p;
                    }
                }
            }

            if(onResizeContent != null)
            {
                onResizeContent(size);
            }
        }


        /// <summary>
        /// 回收不需要的element
        /// </summary>
        protected void ProcessUnUsedElement()
        {
            if (m_unUsedElementList != null && m_unUsedElementList.Count > 0)
            {
                for (int i = 0; i < m_unUsedElementList.Count; i++)
                {
                    m_unUsedElementList[i].Disable();
                }
            }
        }
#endregion

#if UNITY_EDITOR
        [ContextMenu("测试")]
        void Test()
        {
            if(this.myForm == null)
            {
                var form = this.GetComponentInParent<CUIForm>();
                Initialize(form);
            }
            SetPrefab(m_elementTemplate);
            SetCount(m_elementAmount);
        }
#endif
    }
    public enum enUIListType
    {
        Vertical,
        Horizontal,
        VerticalGrid,
        HorizontalGrid
    }

}

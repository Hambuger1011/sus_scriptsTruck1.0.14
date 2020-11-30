using Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.DemiLib;
using System;
using UGUI;

namespace UGUI
{
    public class CUIListView : CUIComponent
    {
        public enum enUIListType
        {
            Vertical,
            Horizontal
        }

        [Header("可选")]
        public ScrollRect m_scrollRect;
        public Scrollbar m_scrollBar;

        [Header("可见区域")]
        public RectTransform m_viewMask;

        [Header("Item挂载区")]
        public RectTransform m_content;

        /// <summary>
        /// 元素prefab
        /// </summary>
        public GameObject m_cellTemplate;

        [Header("列表pivot点")]
        public RectTransform m_pivot;


        [Header("List类型")]
        public enUIListType m_listType;

        [Header("元素数量")]
        public int m_count;

        [Header("元素间距")]
        public Vector2 m_spacing;

        [Header("item开始位置偏移")]
        public float m_layoutOffset;

        protected int m_selectedCellIndex = -1;

        protected int m_lastSelectedCellIndex = -1;


        protected List<CUIListCell> m_usedCellList;

        protected Stack<CUIListCell> m_unUsedCellList;

        protected Vector2 m_cellDefaultSize;

        /// <summary>
        /// 元素大小
        /// </summary>
        protected List<Vector2> m_cellSizeList;

        /// <summary>
        /// 计算得到的元素rect
        /// </summary>
        protected List<stRect> m_cellRectList;

        protected Vector2 m_contentSize;
        protected Vector2 m_lastContentPosition;

        private int m_currentDraggedElementIndex = -1;

        private Vector2 m_contentLastAnchoredPosition = new Vector2(16777215f, 16777215f);
        private Vector2 m_scrollAreaSize;

        [Header("是否使用循环列表")]
        public bool m_useOptimized;

        [Header("元素是否居中")]
        public bool m_autoCenteredElements;

        public bool m_autoCenteredBothSides;

        public float m_fSpeed = 0.2f;

        public delegate void OnInitElement(CUIListView list, int index, CUIListCell element);
        public event OnInitElement onInitElement;

        public event Action<Vector2> onResizeContent;

        public override void Initialize(CUIForm formScript)
        {
            if (!this.m_isInitialized)
            {
                this.m_customUpdateFlags |= enCustomUpdateFlag.eUpdate;
                base.Initialize(formScript);
                m_selectedCellIndex = -1;
                m_lastSelectedCellIndex = -1;
                m_currentDraggedElementIndex = -1;
                m_contentLastAnchoredPosition = new Vector2(16777215f, 16777215f);


                #region ScrollView
                if (m_scrollRect == null)
                {
                    m_scrollRect = base.GetComponentInChildren<ScrollRect>(base.gameObject);
                }
                #endregion
                #region ScrollBar
                if (m_scrollBar == null)
                {
                    m_scrollBar = base.GetComponentInChildren<Scrollbar>(base.gameObject);
                }
                if (m_listType == enUIListType.Vertical)
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
                else if (m_listType == enUIListType.Horizontal)
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

                m_usedCellList = new List<CUIListCell>(8);
                m_unUsedCellList = new Stack<CUIListCell>(8);
                if (m_useOptimized && m_cellRectList == null)
                {
                    m_cellRectList = new List<stRect>();
                }
                var size = this.m_viewMask.rect.size;
                this.m_scrollAreaSize = new Vector2(size.x, size.y);

                if (m_content != null)
                {
                    m_content.pivot = new Vector2(0f, 1f);
                    m_content.anchorMin = new Vector2(0f, 1f);
                    m_content.anchorMax = new Vector2(0f, 1f);
                    m_lastContentPosition = m_pivot.anchoredPosition;
                }
                SetPrefab(m_cellTemplate);
                base.m_isInitialized = true;
                this.SetCount(this.m_count, m_cellSizeList);
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

                if (m_usedCellList != null)
                {
                    m_usedCellList.Clear();
                    m_usedCellList = null;
                }
                if (m_unUsedCellList != null)
                {
                    m_unUsedCellList.Clear();
                    m_unUsedCellList = null;
                }
                m_cellTemplate = null;
                if (m_cellSizeList != null)
                {
                    m_cellSizeList.Clear();
                    m_cellSizeList = null;
                }
                if (m_cellRectList != null)
                {
                    m_cellRectList.Clear();
                    m_cellRectList = null;
                }
                m_scrollRect = null;
                m_content = null;
                m_viewMask = null;
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
        }



        #region 元素选择事件

        public virtual void SelectElement(int index, bool isDispatchSelectedChangeEvent = true)
        {
            m_lastSelectedCellIndex = m_selectedCellIndex;
            m_selectedCellIndex = index;
            if (isDispatchSelectedChangeEvent)
            {
                base.m_indexInlist = index;
            }
            if (m_lastSelectedCellIndex == m_selectedCellIndex)
            {
                if (isDispatchSelectedChangeEvent)
                {
                    //DispatchElementSelectChangedEvent();
                }
            }
            else
            {
                if (m_lastSelectedCellIndex >= 0)
                {
                    CUIListCell elemenet = GetElemenet(m_lastSelectedCellIndex);
                    if (elemenet != null)
                    {
                        elemenet.ChangeDisplay(false);
                    }
                }
                if (m_selectedCellIndex >= 0)
                {
                    CUIListCell elemenet2 = GetElemenet(m_selectedCellIndex);
                    if (elemenet2 != null)
                    {
                        elemenet2.ChangeDisplay(true);
                        if (elemenet2.onSelected != null)
                        {
                            elemenet2.onSelected();
                        }
                    }
                }
            }
        }
        #endregion

        public int GetElementAmount()
        {
            return m_count;
        }

        public CUIListCell GetElemenet(int index)
        {
            if (index >= 0 && index < m_count)
            {
                if (m_useOptimized)
                {
                    for (int i = 0; i < m_usedCellList.Count; i++)
                    {
                        if (m_usedCellList[i].m_index == index)
                        {
                            return m_usedCellList[i];
                        }
                    }
                    return null;
                }
                return m_usedCellList[index];
            }
            return null;
        }

        public CUIListCell GetSelectedElement()
        {
            return GetElemenet(m_selectedCellIndex);
        }

        public CUIListCell GetLastSelectedElement()
        {
            return GetElemenet(m_lastSelectedCellIndex);
        }

        public Vector2 GetEelementDefaultSize()
        {
            return m_cellDefaultSize;
        }

        public int GetSelectedIndex()
        {
            return m_selectedCellIndex;
        }

        public int GetLastSelectedIndex()
        {
            return m_lastSelectedCellIndex;
        }

        public bool IsElementInScrollArea(int index)
        {
            if (index >= 0 && index < m_count)
            {
                stRect stRect = (!m_useOptimized) ? m_usedCellList[index].m_rect : m_cellRectList[index];
                return IsRectInScrollArea(ref stRect);
            }
            return false;
        }

        public int GetMaxIndexInScrollArea()
        {
            int num = -1;
            if (m_count == 0)
            {
                return num;
            }
            int count = m_usedCellList.Count;
            for (int i = 0; i < count; i++)
            {
                stRect rect = m_usedCellList[i].m_rect;
                if (IsRectInScrollArea(ref rect) && m_usedCellList[i].m_index > num)
                {
                    num = m_usedCellList[i].m_index;
                }
            }
            return num;
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
            if (m_content != null)
            {
                m_pivot.anchoredPosition = Vector2.zero;
            }
        }

        public void MoveElementInScrollArea(int index, bool moveImmediately)
        {
            if (index >= 0 && index < m_count)
            {
                Vector2 delta = Vector2.zero;
                Vector2 bord = Vector2.zero;
                stRect stRect = (!m_useOptimized) ? m_usedCellList[index].m_rect : m_cellRectList[index];
                Vector2 offset = m_pivot.anchoredPosition;
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
                    m_pivot.anchoredPosition += delta;
                }
                else
                {
#if true
                    Vector2 to = offset + delta;
                    this.m_content.DOAnchorPos(to, m_fSpeed).OnComplete(() =>
                    {
                        m_pivot.anchoredPosition = to;
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
            return m_selectedCellIndex == index;
        }


        public void MoveContent(Vector2 offset)
        {
            if (m_content != null)
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
                m_pivot.anchoredPosition = vector;
            }
        }

        public void SetCurrentDraggedElementIndex(int elementIndexInList)
        {
            m_currentDraggedElementIndex = elementIndexInList;
        }


        #region 设置element
        /// <summary>
        /// 设置element
        /// </summary>
        public void SetPrefab(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }
            CUIListCell c = gameObject.GetComponent<CUIListCell>();
            if (c != null && gameObject != null)
            {
                c.Initialize(this.myForm);
                m_cellTemplate = gameObject;
                m_cellDefaultSize = c.m_defaultSize;
            }
        }

        public void SetCount(int amount)
        {
            SetCount(amount, null);
        }


        public virtual void SetCount(int amount, List<Vector2> sizeList)
        {
            if (amount < 0)
            {
                amount = 0;
            }
            if (sizeList != null && amount != sizeList.Count)
            {
                amount = sizeList.Count;
                LOG.Error("count不一致");
            }
            m_count = amount;
            m_cellSizeList = sizeList;
            if (!m_isInitialized)
            {
                return;
            }
            RecycleElement();
            ProcessElements();//生成element
        }


        protected virtual void ProcessElements()
        {
            m_contentSize = Vector2.zero;
            Vector2 offset = Vector2.zero;
            if (m_listType == enUIListType.Vertical)
            {
                offset.y += m_layoutOffset;
            }
            else
            {
                offset.x += m_layoutOffset;
            }
            for (int i = 0; i < m_count; i++)
            {
                stRect stRect = LayoutElement(i, ref m_contentSize, ref offset);
                if (m_useOptimized)
                {
                    if (i < m_cellRectList.Count)
                    {
                        m_cellRectList[i] = stRect;
                    }
                    else
                    {
                        m_cellRectList.Add(stRect);
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
            if (m_cellSizeList == null)
            {
                x = m_cellDefaultSize.x;
            }
            else
            {
                Vector2 vec = m_cellSizeList[index];
                x = vec.x;
            }
            result.m_width = (int)x;

            float y;
            if (m_cellSizeList == null)
            {
                y = m_cellDefaultSize.y;
            }
            else
            {
                Vector2 vec = m_cellSizeList[index];
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
                    offset.y -= result.m_height + m_spacing.y;
                    break;
                case enUIListType.Horizontal:
                    offset.x += result.m_width + m_spacing.x;
                    break;
                    //case enUIListType.VerticalGrid:
                    //    offset.x += result.m_width + m_elementSpacing.x;
                    //    if (offset.x + result.m_width > m_scrollAreaSize.x)
                    //    {
                    //        offset.x = 0f;
                    //        offset.y -= result.m_height + m_elementSpacing.y;
                    //    }
                    //    break;
                    //case enUIListType.HorizontalGrid:
                    //    offset.y -= result.m_height + m_elementSpacing.y;
                    //    if (0f - offset.y + result.m_height > m_scrollAreaSize.y)
                    //    {
                    //        offset.y = 0f;
                    //        offset.x += result.m_width + m_elementSpacing.x;
                    //    }
                    //    break;
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
                //else if (m_listType == enUIListType.VerticalGrid && size.x < m_scrollAreaSize.x)
                //{
                //    float x = size.x;
                //    float w = x + m_elementSpacing.x;
                //    while (true)
                //    {
                //        x = w + m_elementDefaultSize.x;
                //        if (x <= m_scrollAreaSize.x)
                //        {
                //            size.x = x;
                //            w = x + m_elementSpacing.x;
                //            continue;
                //        }
                //        break;
                //    }
                //    centerX = (m_scrollAreaSize.x - size.x) / 2f;
                //}
                //else if (m_listType == enUIListType.HorizontalGrid && size.y < m_scrollAreaSize.y)
                //{
                //    float y = size.y;
                //    float h = y + m_elementSpacing.y;
                //    while (true)
                //    {
                //        y = h + m_elementDefaultSize.y;
                //        if (y <= m_scrollAreaSize.y)
                //        {
                //            size.y = y;
                //            h = y + m_elementSpacing.y;
                //            continue;
                //        }
                //        break;
                //    }
                //    centerY = (size.y - m_scrollAreaSize.y) / 2f;
                //}
            }

            //if (size.x < m_scrollAreaSize.x)
            //{
            //    size.x = m_scrollAreaSize.x;
            //}
            //if (size.y < m_scrollAreaSize.y)
            //{
            //    size.y = m_scrollAreaSize.y;
            //}

            if (m_content != null)
            {
                m_content.sizeDelta = size;
                if (resetPosition)
                {
                    m_pivot.anchoredPosition = Vector2.zero;
                }
                if (m_autoCenteredElements)
                {
                    if (centerX != 0f)
                    {
                        Vector2 p = m_pivot.anchoredPosition;
                        p.x = centerX;
                        m_pivot.anchoredPosition = p;
                    }
                    if (centerY != 0f)
                    {
                        Vector2 p = m_pivot.anchoredPosition;
                        p.y = centerY;
                        m_pivot.anchoredPosition = p;
                    }
                }
            }

            if (onResizeContent != null)
            {
                onResizeContent(size);
            }
        }
        #endregion


        #region 循环列表

        protected bool UpdateElementsScroll()
        {
            Vector2 offset = this.m_pivot.anchoredPosition;
            if (m_contentLastAnchoredPosition == offset)
            {
                return false;
            }
            m_contentLastAnchoredPosition = offset;

            #region 处理不可见的元素
            for(int idx = 0; idx < m_usedCellList.Count; )
            {
                if (
                    !IsRectInScrollArea(ref m_usedCellList[idx].m_rect)
                    && m_usedCellList[idx].m_indexInlist != m_currentDraggedElementIndex
                    )
                {
                    RecycleElement(m_usedCellList[idx], true);
                }
                else
                {
                    idx++;
                }
            }
            #endregion

            bool result = false;
            #region 处理可见的元素
            for (int i = 0; i < m_count; i++)
            {
                stRect stRect = m_cellRectList[i];
                if (!IsRectInScrollArea(ref stRect))
                {
                    continue;
                }
                bool isFind = false;
                int itemIdx = 0;
                while (itemIdx < m_usedCellList.Count)
                {
                    if (m_usedCellList[itemIdx].m_index != i)
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
            Vector2 offset = this.m_pivot.anchoredPosition;
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


        protected CUIListCell CreateElement(int index, ref stRect rect)
        {
            CUIListCell c = null;
            if (m_unUsedCellList.Count > 0)
            {
                c = m_unUsedCellList.Pop();
            }
            else if (m_cellTemplate != null)
            {
                GameObject go = GameObject.Instantiate(m_cellTemplate, m_content.transform);
                //go.transform.SetParent(m_content.transform);
                var t = go.transform;
                t.localPosition = Vector3.zero;
                t.localScale = Vector3.one;
                t.localRotation = Quaternion.identity;
                base.InitializeComponent(go);
                c = go.GetComponent<CUIListCell>();
            }
            if (c != null)
            {
                c.Enable(this, index, "cell", ref rect, IsSelectedIndex(index));
                m_usedCellList.Add(c);
            }
            onInitElement(this, index, c);
            return c;
        }

        protected void RecycleElement()
        {
            while (m_usedCellList.Count > 0)
            {
                CUIListCell c = m_usedCellList[0];
                m_usedCellList.RemoveAt(0);
                c.Disable();
                m_unUsedCellList.Push(c);
            }
        }

        protected void RecycleElement(CUIListCell elementScript, bool disableElement)
        {
            if (disableElement)
            {
                elementScript.Disable();
            }
            m_usedCellList.Remove(elementScript);
            m_unUsedCellList.Push(elementScript);
        }

        #endregion

#if UNITY_EDITOR
        [ContextMenu("测试")]
        void Test()
        {
            if (this.myForm == null)
            {
                var form = this.GetComponentInParent<CUIForm>();
                Initialize(form);
            }
            SetPrefab(m_cellTemplate);
            SetCount(m_count);
        }
#endif
    }
}
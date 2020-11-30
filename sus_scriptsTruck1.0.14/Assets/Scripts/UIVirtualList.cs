namespace GameCore.UI
{

    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using System.Collections.Generic;

    public class UIVirtualList : LayoutGroup
    {
        #region inspector
        public ScrollRect scrollRect;
        public GameObject itemPrefab;
        public int spacing = 0;
        #endregion

        private const float SCROLL_DISTANCE_TOLERANCE = 0.0001f;
        private const int EXTRA_ROW_COUNT = 1;

        public event System.Action<object> onItemSelected;  // item

        //private List<object> m_itemList = new List<object>();   // contains all items added by AddItem()
        private float m_itemHeight = -1;

        private List<Row> m_poolRows = new List<Row>();         // rows in pool
        private List<Row> m_visibleRows = new List<Row>();      // rows visible
        private int m_visibleRowCount;

        private object m_selectedItem;              // current selected item
        private int m_selectedItemIndex = -1;       // index of current selected item in m_itemList

        private bool m_isDirty = false;

        private bool m_shouldScrollToBottom = false;

        public RectTransform rectTransform;

        [SerializeField]
        int _itemCount;
        public int itemCount
        {
            get
            {
                return _itemCount;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            rectTransform = (RectTransform)this.transform;
            if (scrollRect != null)
            {
                scrollRect.onValueChanged.AddListener(OnScrollRectValueChanged);
            }

            if (!IsChildAlignToTop() && !IsChildAlignToBottom())
            {
                childAlignment = TextAnchor.UpperLeft;
            }
            itemPrefab.SetActive(false);
            for(int i = 0; i < this.rectTransform.childCount; ++i)
            {
                var ch = this.rectTransform.GetChild(i);
                Row row = new Row();
                row.rect = (RectTransform)ch;
                this.m_poolRows.Add(row);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            MarkDirty();
        }

        private void Update()
        {
            if (m_isDirty)
            {
                Refresh();
                m_isDirty = false;
            }
        }

        private void Refresh()
        {
            var layoutTrans = this.scrollRect.content;
            float beginY = layoutTrans.anchoredPosition.y;
            if(layoutTrans != this.rectTransform)
            {
                beginY += rectTransform.anchoredPosition.y;
            }
            float viewHeight = ((RectTransform)(scrollRect.transform)).rect.height;

            // Calculate range of items which should be visible. [beginItemIndex, endItemIndex)
            int beginItemIndex = Mathf.FloorToInt(beginY / RowHeight) - EXTRA_ROW_COUNT;
            int endItemIndex = Mathf.CeilToInt((beginY + viewHeight) / RowHeight) + EXTRA_ROW_COUNT;

            if (beginItemIndex < 0)
            {
                beginItemIndex = 0;
            }
            if (endItemIndex > itemCount)
            {
                endItemIndex = itemCount;
            }

            bool isDirty = false;

            // Visible items which are not in range should be invisible.
            for (int i = m_visibleRows.Count - 1; i >= 0; i--)
            {
                Row row = m_visibleRows[i];
                if ((row.itemIndex >= beginItemIndex) && (row.itemIndex <= endItemIndex))
                {
                    continue;
                }
                //可见的变不可见
                m_poolRows.Add(row);
                m_visibleRows.RemoveAt(i);
                isDirty = true;
            }

            // Invisible items which are in range now should be visible.
            for (int i = beginItemIndex; i < endItemIndex; i++)
            {
                if (IsVisibleItem(i))
                {
                    continue;
                }

                Row row = GetRow(i);
                m_visibleRows.Add(row);
                isDirty = true;
            }

            // Layout
            if (isDirty
                || (m_visibleRowCount != m_visibleRows.Count))
            {
                LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            }

            m_visibleRowCount = m_visibleRows.Count;
        }

        private void MarkDirty()
        {
            if (!IsActive())
            {
                return;
            }

            m_isDirty = true;
        }

        private bool IsChildAlignToTop()
        {
            bool alignToTop = false;
            if (childAlignment == TextAnchor.UpperLeft
                || childAlignment == TextAnchor.UpperCenter
                || childAlignment == TextAnchor.UpperRight)
            {
                alignToTop = true;
            }
            return alignToTop;
        }

        private bool IsChildAlignToBottom()
        {
            bool alignToBottom = false;
            if (childAlignment == TextAnchor.LowerRight
                || childAlignment == TextAnchor.LowerCenter
                || childAlignment == TextAnchor.LowerLeft)
            {
                alignToBottom = true;
            }
            return alignToBottom;
        }

        private float ItemHeight
        {
            get
            {
                if (m_itemHeight <= 0)
                {
                    RectTransform rectTrans = itemPrefab.GetComponent<RectTransform>() as RectTransform;
                    m_itemHeight = rectTrans.rect.height;

                    if (m_itemHeight < 1)
                    {
                        LOG.Warn("ItemPrefab's height should >= 1");
                        m_itemHeight = 20;
                    }
                }

                return m_itemHeight;
            }
        }

        private float RowHeight
        {
            get { return ItemHeight + spacing; }
        }

        public override float minHeight
        {
            get
            {
                float height = RowHeight * itemCount + (padding.top + padding.bottom);
                return height;
            }
        }

        public override void SetLayoutHorizontal()
        {
            float width = rectTransform.rect.width - (padding.left + padding.right);

            float pos1 = rectTransform.rect.width + 20;
            for (int i = 0; i < m_poolRows.Count; i++)//设置不可见item
            {
                SetChildAlongAxis(m_poolRows[i].rect, 0, pos1, width);
#if UNITY_EDITOR
                m_poolRows[i].rect.name = "--"+ m_poolRows[i].itemIndex;
#endif
            }

            float pos2 = padding.left;
            for (int i = 0; i < m_visibleRows.Count; i++)
            {
                SetChildAlongAxis(m_visibleRows[i].rect, 0, pos2, width);
#if UNITY_EDITOR
                m_visibleRows[i].rect.name = "+" + m_visibleRows[i].itemIndex;
#endif
            }
        }

        public override void SetLayoutVertical()
        {
            for (int i = 0; i < m_visibleRows.Count; i++)//设置可见item
            {
                Row row = m_visibleRows[i];
                float pos = RowHeight * row.itemIndex + padding.top;
                SetChildAlongAxis(row.rect, 1, pos, ItemHeight);
            }
        }

        public override void CalculateLayoutInputVertical()
        {
            SetLayoutInputForAxis(minHeight, minHeight, 0, 1);
        }

#region items

        public void SetItemCount(int count)
        {
            this._itemCount = count;
            this.ClearItems();
            //MarkDirty();
            this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, minHeight);
        }

        //public void RemoveItem(object item)
        //{
        //    int itemIndex = m_itemList.IndexOf(item);
        //    if (itemIndex < 0)
        //    {
        //        return;
        //    }

        //    InvalidateItem(itemIndex);
        //    m_itemList.RemoveAt(itemIndex);

        //    RefreshVisibleRowsItemIndex();

        //    MarkDirty();
        //}

        //private void RefreshVisibleRowsItemIndex()
        //{
        //    for (int i = 0; i < m_visibleRows.Count; i++)
        //    {
        //        int itemIndex = i;// m_itemList.IndexOf(m_visibleRows[i].item);
        //        m_visibleRows[i].itemIndex = itemIndex;
        //    }
        //}


        private void InvalidateItem(int itemIndex)
        {
            for (int i = 0; i < m_visibleRows.Count; i++)
            {
                if (m_visibleRows[i].itemIndex == itemIndex)
                {
                    m_poolRows.Add(m_visibleRows[i]);
                    m_visibleRows.RemoveAt(i);
                    break;
                }
            }
        }

        private bool IsVisibleItem(int itemIndex)
        {
            for (int i = 0; i < m_visibleRows.Count; i++)
            {
                if (m_visibleRows[i].itemIndex == itemIndex)
                {
                    return true;
                }
            }
            return false;
        }

#endregion

#region rows
        public Action<Row> onItemRefresh;

        public class Row
        {
            //public object item;
            public int itemIndex;

            public RectTransform rect;
        }

        private Row CreateRow()
        {
            Row row = new Row();

            GameObject go = GameObject.Instantiate(itemPrefab) as GameObject;
            row.rect = go.GetComponent(typeof(RectTransform)) as RectTransform;
            //row.view = go.GetComponent(typeof(IItemView)) as IItemView;
            go.transform.SetParent(rectTransform, false);
            go.SetActive(true);
            return row;
        }

        private Row GetRow(int itemIndex)
        {
            // If there are no rows in the pool, just create one.
            if (m_poolRows.Count == 0)
            {
                Row newRow = CreateRow();
                SetRow(newRow, itemIndex);
                return newRow;
            }

            //object item = m_itemList[itemIndex];
            Row row = null;

            // Try to find out a row which has this item. If find, use it.
            for (int i = 0; i < m_poolRows.Count; i++)
            {
                if (m_poolRows[i].itemIndex == itemIndex)
                {
                    row = m_poolRows[i];
                    m_poolRows.RemoveAt(i);
                    SetRow(row, itemIndex);
                    break;
                }
            }

            // If not find, use the end item in the pool.
            if (row == null)
            {
                row = PopEnd(m_poolRows);
                SetRow(row, itemIndex);
            }

            return row;
        }

        private void SetRow(Row row, int itemIndex)
        {
            row.itemIndex = itemIndex;
            //row.item = m_itemList[itemIndex];
            if(onItemRefresh != null)
            {
                onItemRefresh(row);
            }
        }

        private Row PopEnd(List<Row> rows)
        {
            if (rows == null || rows.Count == 0)
            {
                return null;
            }

            int index = rows.Count - 1;
            Row r = rows[index];
            rows.RemoveAt(index);
            return r;
        }
#endregion

#region scroll to bottom

        private void OnScrollRectValueChanged(Vector2 size)
        {
            if (size.y >= 0 && size.y <= 1)
            {
                //scrollRect.verticalNormalizedPosition = Mathf.Clamp01(size.y);
                MarkDirty();
            }

        }

#endregion


        [ContextMenu("RefreshLayout")]
        public void RefreshLayout()
        {
            m_isDirty = true;
            this.Refresh();
            m_isDirty = false;
        }


        [ContextMenu("ClearItems")]
        public void ClearItems()
        {
            for (int i = m_visibleRows.Count - 1; i >= 0; i--)
            {
                InvalidateItem(m_visibleRows[i].itemIndex);
            }
            //this._itemCount = 0;
            MarkDirty();
        }
    }
}
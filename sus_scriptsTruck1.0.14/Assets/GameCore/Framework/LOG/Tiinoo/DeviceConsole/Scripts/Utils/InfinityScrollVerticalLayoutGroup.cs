using Framework;

using Object= UnityEngine.Object;

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Tiinoo.DeviceConsole
{
	public class InfinityScrollVerticalLayoutGroup : LayoutGroup, IPointerClickHandler
	{
		#region inspector
		public ScrollRect scrollRect;
		public GameObject itemPrefab;
		public int spacing = 0;
		public bool stickToBottom = true;
		public bool itemSelectable = true;
		#endregion

		private const float SCROLL_DISTANCE_TOLERANCE = 0.0001f;
		private const int EXTRA_ROW_COUNT = 2;
		
		public event System.Action<object> onItemSelected;	// item
		
		private List<object> m_itemList = new List<object>();	// contains all items added by AddItem()
		private float m_itemHeight = -1;
		
		private List<Row> m_poolRows = new List<Row>();			// rows in pool
		private List<Row> m_visibleRows = new List<Row>();		// rows visible
		private int m_visibleRowCount;
		
		private object m_selectedItem;				// current selected item
		private int m_selectedItemIndex = -1;		// index of current selected item in m_itemList
		
		private bool m_isDirty = false;
		
		private bool m_shouldScrollToBottom = false;
		
		protected override void Awake()
		{
			base.Awake();
			scrollRect.onValueChanged.AddListener(OnScrollRectValueChanged);

			if (!IsChildAlignToTop() && !IsChildAlignToBottom())
			{
				childAlignment = TextAnchor.UpperLeft;
			}
            itemPrefab.SetActive(false);
		}
		
		protected override void OnEnable()
		{
			base.OnEnable();
			MarkDirty();
		}

		private void Update()
		{
			CheckSelectedItem();
			
			ScrollToBottom();
			
			if (m_isDirty)
			{
				CheckShouldScrollToBottom();
				Refresh();
				m_isDirty = false;
			}
		}

		private void Refresh()
		{
			float beginY = rectTransform.anchoredPosition.y;
			float viewHeight = ((RectTransform)(scrollRect.transform)).rect.height;
			
			// Calculate range of items which should be visible. [beginItemIndex, endItemIndex)
			int beginItemIndex = Mathf.FloorToInt(beginY / RowHeight) - EXTRA_ROW_COUNT;
			int endItemIndex = Mathf.CeilToInt((beginY + viewHeight) / RowHeight) + EXTRA_ROW_COUNT;

			if (beginItemIndex < 0)
			{
				beginItemIndex = 0;
			}
			if (endItemIndex > m_itemList.Count)
			{
				endItemIndex = m_itemList.Count;
			}
			
			bool isDirty = false;
			
			// Visible items which are not in range should be invisible.
			for (int i = m_visibleRows.Count-1; i >= 0; i--)
			{
				Row row = m_visibleRows[i];
				if ((row.itemIndex >= beginItemIndex) && (row.itemIndex <= endItemIndex))
				{
					continue;
				}
				
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
			    ||  childAlignment == TextAnchor.LowerLeft)
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
					ILayoutElement element = itemPrefab.GetComponent(typeof(ILayoutElement)) as ILayoutElement;
					if (element != null)
					{
						m_itemHeight = element.preferredHeight;
					}
					else
					{
						RectTransform rectTrans = itemPrefab.GetComponent(typeof(RectTransform)) as RectTransform;
						m_itemHeight = rectTrans.rect.height;
					}
					
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
			get {return ItemHeight + spacing;}
		}
		
		public override float minHeight
		{
			get 
			{ 
				float height = RowHeight * m_itemList.Count + (padding.top + padding.bottom);
				return height;
			}
		}
		
		public override void SetLayoutHorizontal()
		{
			float width = rectTransform.rect.width - (padding.left + padding.right);
			
			float pos1 = rectTransform.rect.width + 20;
			for (int i = 0; i < m_poolRows.Count; i++)
			{
				SetChildAlongAxis(m_poolRows[i].rect, 0, pos1, width);
			}
			
			float pos2 = padding.left;
			for (int i = 0; i < m_visibleRows.Count; i++)
			{
				SetChildAlongAxis(m_visibleRows[i].rect, 0, pos2, width);
			}
		}
		
		public override void SetLayoutVertical()
		{
			for (int i = 0; i < m_visibleRows.Count; i++)
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
		
		public void AddItem(object item)
		{
			m_itemList.Add(item);
			MarkDirty();
		}
		
		public void RemoveItem(object item)
		{
			if (SelectedItem == item)
			{
				SelectedItem = null;
			}
			
			int itemIndex = m_itemList.IndexOf(item);
			if (itemIndex < 0)
			{
				return;
			}
			
			InvalidateItem(itemIndex);
			m_itemList.RemoveAt(itemIndex);
			
			RefreshVisibleRowsItemIndex();
			
			MarkDirty();
		}
		
		public void ClearItems()
		{
			SelectedItem = null;
			for (int i = m_visibleRows.Count-1; i >= 0; i--)
			{
				InvalidateItem(m_visibleRows[i].itemIndex);
			}
			m_itemList.Clear();
			MarkDirty();
		}
		
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
		
		private void RefreshVisibleRowsItemIndex()
		{
			for (int i = 0; i < m_visibleRows.Count; i++)
			{
				int itemIndex = m_itemList.IndexOf(m_visibleRows[i].item);
				m_visibleRows[i].itemIndex = itemIndex;
			}
		}
		#endregion
		
		#region rows

		private class Row
		{
			public object item;
			public int itemIndex;
			
			public RectTransform rect;
			public IItemView view;
		}

		private Row CreateRow()
		{
			Row row = new Row();
			
			GameObject go = GameObject.Instantiate(itemPrefab) as GameObject;
			row.rect = go.GetComponent(typeof(RectTransform)) as RectTransform;
			row.view = go.GetComponent(typeof(IItemView)) as IItemView;
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
			
			object item = m_itemList[itemIndex];
			Row row = null;
			
			// Try to find out a row which has this item. If find, use it.
			for (int i = 0; i < m_poolRows.Count; i++)
			{
				if (m_poolRows[i].item == item)
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
			row.item = m_itemList[itemIndex];
			
			bool evenRow = (itemIndex % 2 == 0) ? true : false;
			
			bool isSelected = false;
			if ((SelectedItem != null) && (row.item == SelectedItem))
			{
				isSelected = true;
			}
			
			row.view.Refresh(row.item, evenRow, isSelected);
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
			if (size.y < 0 || size.y > 1)
			{
				scrollRect.verticalNormalizedPosition = Mathf.Clamp01(size.y);
			}
			
			MarkDirty();
		}

		private void CheckShouldScrollToBottom()
		{
			if (!stickToBottom)
			{
				return;
			}
			
			if (scrollRect.verticalNormalizedPosition < SCROLL_DISTANCE_TOLERANCE)
			{
				m_shouldScrollToBottom = true;
			}
		}
		
		private void ScrollToBottom()
		{
			if (!stickToBottom)
			{
				return;
			}
			
			if (m_shouldScrollToBottom)
			{
				scrollRect.verticalNormalizedPosition = 0f;
				m_shouldScrollToBottom = false;
			}
		}
		#endregion

		#region select item
		public object SelectedItem
		{
			get 
			{ 
				return m_selectedItem; 
			}
			set
			{
				if (!itemSelectable)
				{
					return;
				}
				
				if (m_selectedItem == value)
				{
					return;
				}
				
//				object oldItem = m_selectedItem;
				
				if (m_selectedItem != null)
				{
					InvalidateItem(m_selectedItemIndex);
				}
				
				int newSelectedItemIndex = (value != null) ? m_itemList.IndexOf(value) : -1;
				m_selectedItem = value;
				m_selectedItemIndex = newSelectedItemIndex;
				
				if (m_selectedItem != null)
				{
					InvalidateItem(m_selectedItemIndex);
				}
				
				MarkDirty();

//				Debug.Log(string.Format("select {0} -> {1}", oldItem, m_selectedItem));

				if (onItemSelected != null)
				{
					onItemSelected(m_selectedItem);
				}
			}
		}

		private void CheckSelectedItem()
		{
			if (SelectedItem != null)
			{
				if (!m_itemList.Contains(SelectedItem))
				{
					SelectedItem = null;
				}
			}
		}
		
		public void OnPointerClick(PointerEventData eventData)
		{
			if (!itemSelectable)
			{
				return;
			}
			
			GameObject clickObj = eventData.pointerPressRaycast.gameObject;
			if (clickObj == null)
			{
				return;
			}
			
			Vector3 clickPos = clickObj.transform.position;
			Vector3 localPos = rectTransform.InverseTransformPoint(clickPos);
			
			float indexF = (Mathf.Abs(localPos.y) - padding.top) / RowHeight;
			int index = (int)(indexF);
			if (index < 0 || index >= m_itemList.Count)
			{
				SelectedItem = null;
			}
			else
			{
				SelectedItem = m_itemList[index];
			}
		}
		#endregion
	}
}


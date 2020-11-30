namespace UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;
    using UnityEngine.UI;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]


    [XLua.Hotfix, XLua.CSharpCallLua, XLua.LuaCallCSharp]
    public class UIBubbleList : UIBehaviour, ILayoutElement, ILayoutGroup
    {
        const int axis_x = 0;
        const int axis_y = 1;
        [SerializeField] protected RectOffset m_Padding = new RectOffset();
        public RectOffset padding { get { return m_Padding; } set { SetProperty(ref m_Padding, value); } }

        [SerializeField] protected TextAnchor m_ChildAlignment = TextAnchor.UpperLeft;
        public TextAnchor childAlignment { get { return m_ChildAlignment; } set { SetProperty(ref m_ChildAlignment, value); } }

        [System.NonSerialized] private RectTransform m_Rect;
        protected RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        protected DrivenRectTransformTracker m_Tracker;
        private Vector2 m_TotalMinSize = Vector2.zero;
        private Vector2 m_TotalPreferredSize = Vector2.zero;
        private Vector2 m_TotalFlexibleSize = Vector2.zero;

     




        [SerializeField]
        protected float m_Spacing = 0;
        public float spacing { get { return m_Spacing; } set { SetProperty(ref m_Spacing, value); } }

        public ScrollRect scrollView;

        [Header("是否使用循环列表")]
        public bool m_useOptimized = true;


        // Implementation

        protected UIBubbleList()
        {
            if (m_Padding == null)
                m_Padding = new RectOffset();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if(scrollView == null)
            {
                scrollView = this.GetComponentInParent<ScrollRect>();
            }
            SetDirty();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

#region ILayoutElement

        public virtual float flexibleHeight { get { return GetTotalFlexibleSize(1); } }
        public virtual float flexibleWidth { get { return GetTotalFlexibleSize(0); } }
        public virtual int layoutPriority { get { return 0; } }
        public virtual float minHeight { get { return GetTotalMinSize(1); } }
        public virtual float minWidth { get { return GetTotalMinSize(0); } }
        public virtual float preferredWidth { get { return GetTotalPreferredSize(0); } }
        public virtual float preferredHeight { get { return GetTotalPreferredSize(1); } }


        protected float GetTotalMinSize(int axis)
        {
            return m_TotalMinSize[axis];
        }

        protected float GetTotalPreferredSize(int axis)
        {
            return m_TotalPreferredSize[axis];
        }

        protected float GetTotalFlexibleSize(int axis)
        {
            return m_TotalFlexibleSize[axis];
        }

        // ILayoutElement Interface
        /// <summary>
        /// 计算面板width
        /// </summary>
        public virtual void CalculateLayoutInputHorizontal()
        {
            m_Tracker.Clear();
            //CalcAlongAxis(0, true);
        }

        /// <summary>
        /// 计算面板height
        /// </summary>
        public void CalculateLayoutInputVertical()
        {
           // CalcAlongAxis(1, true);
        }


        protected void CalcAlongAxis(int axis, bool isVertical)
        {
            float combinedPadding = (axis == 0 ? padding.horizontal : padding.vertical);

            float totalMin = combinedPadding;
            float totalPreferred = combinedPadding;
            float totalFlexible = 0;

            bool alongOtherAxis = (isVertical ^ (axis == 1));
            if(this.items.Count > 0)
            {
                totalPreferred -= spacing;
                for (int i = 0, iMax = this.items.Count; i < iMax; ++i)
                {
                    var item = items[i];
                    if(item.isNew)
                    {
                        item.isNew = false;
                        item.size = item.ui.size;
                    }
                    totalPreferred += item.size[axis] + spacing;
                }
            }
            totalPreferred = Mathf.Max(totalMin, totalPreferred);
            SetLayoutInputForAxis(totalMin, totalPreferred, totalFlexible, axis);
        }

#endregion


#region ILayoutController
        public void SetLayoutHorizontal()
        {
            //SetChildrenAlongAxis(0, true);
        }
        public void SetLayoutVertical()
        {
        }

        protected void SetChildrenAlongAxis(int axis, bool isVertical)
        {
            float size = rectTransform.rect.size[axis];
            float alignmentOnAxis = GetAlignmentOnAxis(axis);

            bool alongOtherAxis = (isVertical ^ (axis == 1));
            if (alongOtherAxis)
            {
                //float innerSize = size - (axis == 0 ? padding.horizontal : padding.vertical);
                //for (int i = 0, iMax = this.items.Count; i < iMax; ++i)
                //{
                //    var ite = this.items[i];
                //}
            }
            else
            {
                float pos = (axis == 0 ? padding.left : padding.top);
                if (GetTotalFlexibleSize(axis) == 0 && GetTotalPreferredSize(axis) < size)
                    pos = GetStartOffset(axis, GetTotalPreferredSize(axis) - (axis == 0 ? padding.horizontal : padding.vertical));

                float minMaxLerp = 0;
                if (GetTotalMinSize(axis) != GetTotalPreferredSize(axis))
                    minMaxLerp = Mathf.Clamp01((size - GetTotalMinSize(axis)) / (GetTotalPreferredSize(axis) - GetTotalMinSize(axis)));

                float itemFlexibleMultiplier = 0;
                if (size > GetTotalPreferredSize(axis))
                {
                    if (GetTotalFlexibleSize(axis) > 0)
                        itemFlexibleMultiplier = (size - GetTotalPreferredSize(axis)) / GetTotalFlexibleSize(axis);
                }

                for (int i = 0, iMax = this.items.Count; i < iMax; ++i)
                {
                    var item = this.items[i];

                    float min = 0;
                    float preferred = item.size[axis];
                    float flexible = 1;
                    float childSize = Mathf.Lerp(min, preferred, minMaxLerp);
                    childSize += flexible * itemFlexibleMultiplier;
                    float offsetInCell = 0;// (childSize - item.transform.sizeDelta[axis]) * alignmentOnAxis;
                    SetChildAlongAxis(item, axis, pos + offsetInCell, childSize);
                    pos += childSize + spacing;
                }
            }
        }
        protected void SetChildAlongAxis(UIBubbleData item, int axis, float pos, float childSize)
        {
            item.leftTop[axis] = pos;
            if (item.ui != null)
            {
                item.ui.SetSize();
                m_Tracker.Add(this, item.ui.transform,
                    DrivenTransformProperties.Anchors |
                    (axis == 0 ? DrivenTransformProperties.AnchoredPositionX : DrivenTransformProperties.AnchoredPositionY));

                item.ui.transform.SetInsetAndSizeFromParentEdge(axis == 0 ? RectTransform.Edge.Left : RectTransform.Edge.Top, pos, childSize);
            }
        }



        private void HandleSelfFittingAlongAxis(int axis)
        {
            m_Tracker.Add(this, rectTransform, (axis == 0 ? DrivenTransformProperties.SizeDeltaX : DrivenTransformProperties.SizeDeltaY));

            // Set size to min or preferred size
            //if (fitting == FitMode.MinSize)
            //    rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, LayoutUtility.GetMinSize(m_Rect, axis));
            //else
            rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, this.GetTotalPreferredSize(axis));
        }

#endregion

#region Layout

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }

#endif
        /// <summary>
        /// Callback for when properties have been changed by animation.
        /// </summary>
        protected override void OnDidApplyAnimationProperties()
        {
            SetDirty();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (isRootLayoutGroup)
                SetDirty();
        }

        protected virtual void OnTransformChildrenChanged()
        {
            SetDirty();
        }

        /// <summary>
        /// Mark the LayoutGroup as dirty.
        /// </summary>
        protected void SetDirty()
        {
            if (!IsActive())
                return;

            if (!CanvasUpdateRegistry.IsRebuildingLayout())
                LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            else
                StartCoroutine(DelayedSetDirty(rectTransform));
        }

        IEnumerator DelayedSetDirty(RectTransform rectTransform)
        {
            yield return null;
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

#endregion

#region 获取pivot
        /// <summary>
        /// Returns the calculated position of the first child layout element along the given axis.
        /// </summary>
        protected float GetStartOffset(int axis, float requiredSpaceWithoutPadding)
        {
            float requiredSpace = requiredSpaceWithoutPadding + (axis == 0 ? padding.horizontal : padding.vertical);
            float availableSpace = rectTransform.rect.size[axis];
            float surplusSpace = availableSpace - requiredSpace;
            float alignmentOnAxis = GetAlignmentOnAxis(axis);
            return (axis == 0 ? padding.left : padding.top) + surplusSpace * alignmentOnAxis;
        }

        /// <summary>
        /// Returns the alignment on the specified axis as a fraction where 
        /// 0 is left/top, 
        /// 0.5 is middle, 
        /// 1 is right/bottom.
        /// </summary>
        protected float GetAlignmentOnAxis(int axis)
        {
            if (axis == 0)
                return ((int)childAlignment % 3) * 0.5f;
            else
                return ((int)childAlignment / 3) * 0.5f;
        }
#endregion

        /// <summary>
        /// Used to set the calculated layout properties for the given axis.
        /// </summary>
        protected void SetLayoutInputForAxis(float totalMin, float totalPreferred, float totalFlexible, int axis)
        {
            m_TotalMinSize[axis] = totalMin;
            m_TotalPreferredSize[axis] = totalPreferred;
            m_TotalFlexibleSize[axis] = totalFlexible;
        }
        

        private bool isRootLayoutGroup
        {
            get
            {
                Transform parent = transform.parent;
                if (parent == null)
                    return true;
                return transform.parent.GetComponent(typeof(ILayoutGroup)) == null;
            }
        }


        /// <summary>
        /// Helper method used to set a given property if it has changed.
        /// </summary>
        /// <param name="currentValue">A reference to the member value.</param>
        /// <param name="newValue">The new value.</param>
        protected void SetProperty<T>(ref T currentValue, T newValue)
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return;
            currentValue = newValue;
            SetDirty();
        }


        
        // ILayoutController Interface
        private void Update()
        {
            UpdateItem();
        }

        List<UIBubbleItem> m_activeItems = new List<UIBubbleItem>();
        List<UIBubbleItem> m_deactiveItems = new List<UIBubbleItem>();
        public Action<UIBubbleItem> OnItemClick;

        Vector2 m_lastContentOffset;
        private void UpdateItem()
        {
            if(!m_useOptimized)
            {
                return;
            }
            Vector2 offset = this.rectTransform.anchoredPosition;
            if (m_lastContentOffset == offset)
            {
                return;
            }
            m_lastContentOffset = offset;

            for(int idx = 0; idx < this.m_activeItems.Count;)
            {
                var item = this.m_activeItems[idx];
                if(IsVisiable(item.data))
                {
                    ++idx;
                    continue;
                }
                this.m_activeItems.RemoveAt(idx);
                RecycleItem(item);
            }

            for(int i=0,iMax=this.items.Count;i<iMax;++i)
            {
                var data = this.items[i];
                if(!IsVisiable(data))
                {
                    continue;
                }
                bool isFind = false;
                foreach(var item2 in this.m_activeItems)
                {
                    if(item2.data == data)
                    {
                        isFind = true;
                        break;
                    }
                }
                if(isFind)
                {
                    continue;
                }
                var item = CreateItem(i, data);
            }

            for (int i = 0, iMax = this.items.Count; i < iMax; ++i)
            {
                var data = this.items[i];
                if(data.ui == null)
                {
                    continue;
                }
                data.ui.transform.SetAsLastSibling();
            }
        }
        
        private void RecycleItem(UIBubbleItem item)
        {
            item.DOFade(0);
            item.gameObject.name = "-";
            item.OnDeactive();
            this.m_deactiveItems.Add(item);
    }

    private UIBubbleItem CreateItem(int index, UIBubbleData data, bool reset = false)
        {
            bool isNew = false;
            UIBubbleItem item = null;
            for (int i = this.m_deactiveItems.Count - 1;i >= 0; --i)
            {
                var tmp = this.m_deactiveItems[i];
                if(tmp.type == data.type1)
                {
                    item = tmp;
                    this.m_deactiveItems.RemoveAt(i);
                    item.DOFade(1);
                    break;
                }
            }
            if(item == null)
            {
                isNew = true;
                item = UIBubbleFactory.CreateItem(this,data.type1);
                item.OnItemClick = OnItemClick;
                item.DOFade(0);
            }
            item.SetList(this);
            item.SetIndex(index);
            item.SetData(data);
            this.m_activeItems.Add(item);

            if(reset)
            {
                CalcAlongAxis(1, true);
                HandleSelfFittingAlongAxis(1);
                SetChildrenAlongAxis(1, true);
            }
            else
            {
                int axis = 1;
                float size = rectTransform.rect.size[axis];
                float minMaxLerp = 0;
                if (GetTotalMinSize(axis) != GetTotalPreferredSize(axis))
                    minMaxLerp = Mathf.Clamp01((size - GetTotalMinSize(axis)) / (GetTotalPreferredSize(axis) - GetTotalMinSize(axis)));

                float itemFlexibleMultiplier = 0;
                if (size > GetTotalPreferredSize(axis))
                {
                    if (GetTotalFlexibleSize(axis) > 0)
                        itemFlexibleMultiplier = (size - GetTotalPreferredSize(axis)) / GetTotalFlexibleSize(axis);
                }

                float min = 0;
                float preferred = item.size[1];
                float flexible = 1;
                float childSize = Mathf.Lerp(min, preferred, minMaxLerp);
                childSize += flexible * itemFlexibleMultiplier;
                SetChildAlongAxis(data, 1, data.leftTop.y, childSize);
            }

#if UNITY_EDITOR
            item.gameObject.name = string.Format("[{0}]-({1}-{2})", index, data.leftTop.y, data.leftTop.y + data.size.y);
#endif

            return item;
        }

        private bool IsVisiable(UIBubbleData layoutData)
        {
            if(layoutData.ui.isTween)
            {
                return true;
            }
            Vector2 offset = this.rectTransform.anchoredPosition;
            Vector2 viewSize = this.scrollView.viewport.rect.size;
            var leftTop = layoutData.leftTop;
            leftTop.y = offset.y - leftTop.y;
            var rightBottom = leftTop;
            //rightBottom.x += layoutData.size.x;
            rightBottom.y -= layoutData.size.y;

            if (rightBottom.y <= 0f && leftTop.y >= -viewSize.y)
            {
                return true;
            }
            return false;
        }

        List<UIBubbleData> items = new List<UIBubbleData>();
        public List<UIBubbleData> Items
        {
            get
            {
                return items;
            }
        }


        public void AddItem(UIBubbleData data)
        {
            items.Add(data);
            var item = CreateItem(items.Count-1, data, true);
        }
    }
}
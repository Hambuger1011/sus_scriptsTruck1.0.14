/*
 * 由LoopListView2.cs简化
 */
namespace StoryEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using SuperScrollView;


    [XLua.LuaCallCSharp, XLua.Hotfix]
    public class UIBubbleLayout : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {

        class SnapData
        {
            public SnapStatus mSnapStatus = SnapStatus.NoTargetSet;
            public int mSnapTargetIndex = 0;
            public float mTargetSnapVal = 0;
            public float mCurSnapVal = 0;
            public bool mIsForceSnapTo = false;
            public bool mIsTempTarget = false;
            public int mTempTargetIndex = -1;
            public float mMoveMaxAbsVec = -1;
            public void Clear()
            {
                mSnapStatus = SnapStatus.NoTargetSet;
                mTempTargetIndex = -1;
                mIsForceSnapTo = false;
                mMoveMaxAbsVec = -1;
            }
        }

        [SerializeField, Header("item prefab")]
        List<ItemPrefabConfData> mItemPrefabDataList = new List<ItemPrefabConfData>();
        //[SerializeField,Header("滚动方向")]
        //private ListItemArrangeType mArrangeType = ListItemArrangeType.TopToBottom;
        [SerializeField]
        bool mSupportScrollBar = true;
        [SerializeField]
        bool mItemSnapEnable = false;

        [SerializeField]
        Vector2 mViewPortSnapPivot = Vector2.zero;
        [SerializeField]
        Vector2 mItemSnapPivot = Vector2.zero;
        
        [SerializeField,Header("边间隔")]
        protected RectOffset m_Padding = new RectOffset();
        public RectOffset padding { get { return m_Padding; } set { SetProperty(ref m_Padding, value); } }

        /// <summary>
        /// 子物体对齐方式
        /// </summary>
        [SerializeField, Header("子物体对齐方式")]
        protected TextAnchor m_ChildAlignment = TextAnchor.UpperLeft;
        public TextAnchor childAlignment { get { return m_ChildAlignment; } set { SetProperty(ref m_ChildAlignment, value); } }


        [SerializeField,Header("间隔")]
        protected float m_Spacing = 0;
        public float spacing { get { return m_Spacing; } set { SetProperty(ref m_Spacing, value); } }

        protected virtual void SetProperty<T>(ref T currentValue, T newValue)
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return;
            currentValue = newValue;
            SetDirty();
        }
        protected virtual void SetDirty()
        {
            //if (!IsActive())
            //    return;

            //LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

        ScrollRect mScrollRect = null;
        Dictionary<int, ItemPool> mItemPoolDict = new Dictionary<int, ItemPool>();
        List<ItemPool> mItemPoolList = new List<ItemPool>();

        //public ListItemArrangeType ArrangeType { get { return mArrangeType; } set { mArrangeType = value; } }

        List<UIBubbleItem> mItemList = new List<UIBubbleItem>();//可见的item
        RectTransform mContainerTrans;
        RectTransform mScrollRectTransform = null;
        RectTransform mViewPortRectTransform = null;
        float mItemDefaultWithPaddingSize = 20;
        int mItemTotalCount = 0;
        //bool mIsVertList = false;
        System.Func<UIBubbleLayout, int, UIBubbleItem> mOnGetItemByIndex;
        Vector3[] mItemWorldCorners = new Vector3[4];
        Vector3[] mViewPortRectLocalCorners = new Vector3[4];
        int mCurReadyMinItemIndex = 0;
        int mCurReadyMaxItemIndex = 0;
        bool mNeedCheckNextMinItem = true;
        bool mNeedCheckNextMaxItem = true;
        ItemPosMgr mItemPosMgr = null;
        float mDistanceForRecycle0 = 300;
        float mDistanceForNew0 = 200;
        float mDistanceForRecycle1 = 300;
        float mDistanceForNew1 = 200;
        bool mIsDraging = false;
        PointerEventData mPointerEventData = null;
        public System.Action mOnBeginDragAction = null;
        public System.Action mOnDragingAction = null;
        public System.Action mOnEndDragAction = null;
        int mLastItemIndex = 0;
        float mLastItemPadding = 0;
        float mSmoothDumpVel = 0;
        float mSmoothDumpRate = 0.3f;
        float mSnapFinishThreshold = 0.1f;
        float mSnapVecThreshold = 145;
        float mSnapMoveDefaultMaxAbsVec = 3400f;


        Vector3 mLastFrameContainerPos = Vector3.zero;
        public System.Action<UIBubbleLayout, UIBubbleItem> mOnSnapItemFinished = null;
        public System.Action<UIBubbleLayout, UIBubbleItem> mOnSnapNearestChanged = null;
        int mCurSnapNearestItemIndex = -1;
        Vector2 mAdjustedVec;
        bool mNeedAdjustVec = false;
        int mLeftSnapUpdateExtraCount = 1;
        ClickEventListener mScrollBarClickEventListener = null;
        SnapData mCurSnapData = new SnapData();
        Vector3 mLastSnapCheckPos = Vector3.zero;
        bool mListViewInited = false;
        int mListUpdateCheckFrameCount = 0;

        public List<ItemPrefabConfData> ItemPrefabDataList
        {
            get
            {
                return mItemPrefabDataList;
            }
        }

        public List<UIBubbleItem> ItemList
        {
            get
            {
                return mItemList;
            }
        }

        //public bool IsVertList
        //{
        //    get
        //    {
        //        return mIsVertList;
        //    }
        //}
        public int ItemTotalCount
        {
            get
            {
                return mItemTotalCount;
            }
        }

        public RectTransform ContainerTrans
        {
            get
            {
                return mContainerTrans;
            }
        }

        public ScrollRect ScrollRect
        {
            get
            {
                return mScrollRect;
            }
        }

        public bool IsDraging
        {
            get
            {
                return mIsDraging;
            }
        }

        public bool ItemSnapEnable
        {
            get { return mItemSnapEnable; }
            set { mItemSnapEnable = value; }
        }

        public bool SupportScrollBar
        {
            get { return mSupportScrollBar; }
            set { mSupportScrollBar = value; }
        }

        public float SnapMoveDefaultMaxAbsVec
        {
            get { return mSnapMoveDefaultMaxAbsVec; }
            set { mSnapMoveDefaultMaxAbsVec = value; }
        }

        public ItemPrefabConfData GetItemPrefabConfData(int prefabName)
        {
            return mItemPrefabDataList[prefabName];
        }

        public void OnItemPrefabChanged(int prefabName)
        {
            ItemPrefabConfData data = GetItemPrefabConfData(prefabName);
            if (data == null)
            {
                return;
            }
            ItemPool pool = null;
            if (mItemPoolDict.TryGetValue(prefabName, out pool) == false)
            {
                return;
            }
            int firstItemIndex = -1;
            Vector3 pos = Vector3.zero;
            if (mItemList.Count > 0)
            {
                firstItemIndex = mItemList[0].ItemIndex;
                pos = mItemList[0].CachedRectTransform.anchoredPosition3D;
            }
            RecycleAllItem();
            ClearAllTmpRecycledItem();
            pool.DestroyAllItem();
            pool.Init(data.mItemPrefab, data.mInitCreateCount, mContainerTrans);
            if (firstItemIndex >= 0)
            {
                RefreshAllShownItemWithFirstIndexAndPos(firstItemIndex, pos);
            }
        }

        /*
        InitListView method is to initiate the LoopListView2 component. There are 3 parameters:
        itemTotalCount: the total item count in the listview. If this parameter is set -1, then means there are infinite items, and scrollbar would not be supported, and the ItemIndex can be from –MaxInt to +MaxInt. If this parameter is set a value >=0 , then the ItemIndex can only be from 0 to itemTotalCount -1.
        onGetItemByIndex: when a item is getting in the scrollrect viewport, and this Action will be called with the item’ index as a parameter, to let you create the item and update its content.
        */
        public void InitListView(int itemTotalCount,
            System.Func<UIBubbleLayout, int, UIBubbleItem> onGetItemByIndex,
            LoopListViewInitParam initParam = null)
        {
            if (initParam != null)
            {
                mDistanceForRecycle0 = initParam.mDistanceForRecycle0;
                mDistanceForNew0 = initParam.mDistanceForNew0;
                mDistanceForRecycle1 = initParam.mDistanceForRecycle1;
                mDistanceForNew1 = initParam.mDistanceForNew1;
                mSmoothDumpRate = initParam.mSmoothDumpRate;
                mSnapFinishThreshold = initParam.mSnapFinishThreshold;
                mSnapVecThreshold = initParam.mSnapVecThreshold;
                mItemDefaultWithPaddingSize = initParam.mItemDefaultWithPaddingSize;
            }

            mScrollRect = gameObject.GetComponent<ScrollRect>();
            if (mScrollRect == null)
            {
                Debug.LogError("ListView Init Failed! ScrollRect component not found!");
                return;
            }
            if (mDistanceForRecycle0 <= mDistanceForNew0)
            {
                Debug.LogError("mDistanceForRecycle0 should be bigger than mDistanceForNew0");
            }
            if (mDistanceForRecycle1 <= mDistanceForNew1)
            {
                Debug.LogError("mDistanceForRecycle1 should be bigger than mDistanceForNew1");
            }
            mCurSnapData.Clear();
            mItemPosMgr = new ItemPosMgr(mItemDefaultWithPaddingSize);
            mScrollRectTransform = mScrollRect.GetComponent<RectTransform>();
            mContainerTrans = mScrollRect.content;
            mViewPortRectTransform = mScrollRect.viewport;
            if (mViewPortRectTransform == null)
            {
                mViewPortRectTransform = mScrollRectTransform;
            }
            if (mScrollRect.horizontalScrollbarVisibility == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport && mScrollRect.horizontalScrollbar != null)
            {
                Debug.LogError("ScrollRect.horizontalScrollbarVisibility cannot be set to AutoHideAndExpandViewport");
            }
            if (mScrollRect.verticalScrollbarVisibility == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport && mScrollRect.verticalScrollbar != null)
            {
                Debug.LogError("ScrollRect.verticalScrollbarVisibility cannot be set to AutoHideAndExpandViewport");
            }
            //mIsVertList = (mArrangeType == ListItemArrangeType.TopToBottom || mArrangeType == ListItemArrangeType.BottomToTop);
            mScrollRect.horizontal = false;// !mIsVertList;
            mScrollRect.vertical = true;// mIsVertList;
            SetScrollbarListener();
            AdjustPivot(mViewPortRectTransform);
            AdjustAnchor(mContainerTrans);
            AdjustContainerPivot(mContainerTrans);
            InitItemPool();
            mOnGetItemByIndex = onGetItemByIndex;
            if (mListViewInited == true)
            {
                Debug.LogError("LoopListView2.InitListView method can be called only once.");
            }
            mListViewInited = true;
            ResetListView();
            //SetListItemCount(itemTotalCount, true);
            mCurSnapData.Clear();
            mItemTotalCount = itemTotalCount;
            if (mItemTotalCount < 0)
            {
                mSupportScrollBar = false;
            }
            if (mSupportScrollBar)
            {
                mItemPosMgr.SetItemMaxCount(mItemTotalCount, this.padding);
            }
            else
            {
                mItemPosMgr.SetItemMaxCount(0, this.padding);
            }
            mCurReadyMaxItemIndex = 0;
            mCurReadyMinItemIndex = 0;
            mLeftSnapUpdateExtraCount = 1;
            mNeedCheckNextMaxItem = true;
            mNeedCheckNextMinItem = true;
            UpdateContentSize();
        }

        void SetScrollbarListener()
        {
            mScrollBarClickEventListener = null;
            Scrollbar curScrollBar = null;
            //if (mIsVertList && mScrollRect.verticalScrollbar != null)
            //{
                curScrollBar = mScrollRect.verticalScrollbar;

            //}
            //if (!mIsVertList && mScrollRect.horizontalScrollbar != null)
            //{
            //    curScrollBar = mScrollRect.horizontalScrollbar;
            //}
            if (curScrollBar == null)
            {
                return;
            }
            ClickEventListener listener = ClickEventListener.Get(curScrollBar.gameObject);
            mScrollBarClickEventListener = listener;
            listener.SetPointerUpHandler(OnPointerUpInScrollBar);
            listener.SetPointerDownHandler(OnPointerDownInScrollBar);
        }

        void OnPointerDownInScrollBar(GameObject obj)
        {
            mCurSnapData.Clear();
        }

        void OnPointerUpInScrollBar(GameObject obj)
        {
            ForceSnapUpdateCheck();
        }

        public void ResetListView(bool resetPos = true)
        {
            mViewPortRectTransform.GetLocalCorners(mViewPortRectLocalCorners);
            if (resetPos)
            {
                SetContentPos(Vector3.zero);
            }
            ForceSnapUpdateCheck();
        }


        /*
        This method may use to set the item total count of the scrollview at runtime. 
        If this parameter is set -1, then means there are infinite items,
        and scrollbar would not be supported, and the ItemIndex can be from –MaxInt to +MaxInt. 
        If this parameter is set a value >=0 , then the ItemIndex can only be from 0 to itemTotalCount -1.  
        If resetPos is set false, then the scrollrect’s content position will not changed after this method finished.
        */
        public void SetListItemCount(int itemCount, bool resetPos = true)
        {
            if (itemCount == mItemTotalCount)
            {
                return;
            }
            mCurSnapData.Clear();
            mItemTotalCount = itemCount;
            if (mItemTotalCount < 0)
            {
                mSupportScrollBar = false;
            }
            if (mSupportScrollBar)
            {
                mItemPosMgr.SetItemMaxCount(mItemTotalCount, this.padding);
            }
            else
            {
                mItemPosMgr.SetItemMaxCount(0, this.padding);
            }
            if (mItemTotalCount == 0)
            {
                mCurReadyMaxItemIndex = 0;
                mCurReadyMinItemIndex = 0;
                mNeedCheckNextMaxItem = false;
                mNeedCheckNextMinItem = false;
                RecycleAllItem();
                ClearAllTmpRecycledItem();
                UpdateContentSize();
                return;
            }
            if (mCurReadyMaxItemIndex >= mItemTotalCount)
            {
                mCurReadyMaxItemIndex = mItemTotalCount - 1;
            }
            mLeftSnapUpdateExtraCount = 1;
            mNeedCheckNextMaxItem = true;
            mNeedCheckNextMinItem = true;
            if (resetPos)
            {
                MovePanelToItemIndex(0, 0);
                return;
            }
            if (mItemList.Count == 0)
            {
                MovePanelToItemIndex(0, 0);
                return;
            }
            int maxItemIndex = mItemTotalCount - 1;
            int lastItemIndex = mItemList[mItemList.Count - 1].ItemIndex;
            if (lastItemIndex <= maxItemIndex)
            {
                UpdateContentSize();
                UpdateAllShownItemsPos();
                return;
            }
            MovePanelToItemIndex(maxItemIndex, 0);

        }

        //To get the visible item by itemIndex. If the item is not visible, then this method return null.
        public UIBubbleItem GetShownItemByItemIndex(int itemIndex)
        {
            int count = mItemList.Count;
            if (count == 0)
            {
                return null;
            }
            if (itemIndex < mItemList[0].ItemIndex || itemIndex > mItemList[count - 1].ItemIndex)
            {
                return null;
            }
            int i = itemIndex - mItemList[0].ItemIndex;
            return mItemList[i];
        }


        public UIBubbleItem GetShownItemNearestItemIndex(int itemIndex)
        {
            int count = mItemList.Count;
            if (count == 0)
            {
                return null;
            }
            if (itemIndex < mItemList[0].ItemIndex)
            {
                return mItemList[0];
            }
            if (itemIndex > mItemList[count - 1].ItemIndex)
            {
                return mItemList[count - 1];
            }
            int i = itemIndex - mItemList[0].ItemIndex;
            return mItemList[i];
        }

        public int ShownItemCount
        {
            get
            {
                return mItemList.Count;
            }
        }

        public float ViewPortSize
        {
            get
            {
                //if (mIsVertList)
                {
                    return mViewPortRectTransform.rect.height;
                }
                //else
                //{
                //    return mViewPortRectTransform.rect.width;
                //}
            }
        }

        public float ViewPortWidth
        {
            get { return mViewPortRectTransform.rect.width; }
        }
        public float ViewPortHeight
        {
            get { return mViewPortRectTransform.rect.height; }
        }


        /*
         All visible items is stored in a List<LoopListViewItem2> , which is named mItemList;
         this method is to get the visible item by the index in visible items list. The parameter index is from 0 to mItemList.Count.
        */
        public UIBubbleItem GetShownItemByIndex(int index)
        {
            int count = mItemList.Count;
            if (index < 0 || index >= count)
            {
                return null;
            }
            return mItemList[index];
        }

        public UIBubbleItem GetShownItemByIndexWithoutCheck(int index)
        {
            return mItemList[index];
        }

        public int GetIndexInShownItemList(UIBubbleItem item)
        {
            if (item == null)
            {
                return -1;
            }
            int count = mItemList.Count;
            if (count == 0)
            {
                return -1;
            }
            for (int i = 0; i < count; ++i)
            {
                if (mItemList[i] == item)
                {
                    return i;
                }
            }
            return -1;
        }


        public void DoActionForEachShownItem(System.Action<UIBubbleItem, object> action, object param)
        {
            if (action == null)
            {
                return;
            }
            int count = mItemList.Count;
            if (count == 0)
            {
                return;
            }
            for (int i = 0; i < count; ++i)
            {
                action(mItemList[i], param);
            }
        }


        public UIBubbleItem NewListViewItem(int itemPrefabName)
        {
            ItemPool pool = null;
            if (mItemPoolDict.TryGetValue(itemPrefabName, out pool) == false)
            {
                return null;
            }
            UIBubbleItem item = pool.GetItem();
            RectTransform rf = item.GetComponent<RectTransform>();
            rf.SetParent(mContainerTrans);
            rf.localScale = Vector3.one;
            rf.anchoredPosition3D = Vector3.zero;
            rf.localEulerAngles = Vector3.zero;
            item.ParentListView = this;
            return item;
        }

        /*
        For a vertical scrollrect, when a visible item’s height changed at runtime, then this method should be called to let the LoopListView2 component reposition all visible items’ position.
        For a horizontal scrollrect, when a visible item’s width changed at runtime, then this method should be called to let the LoopListView2 component reposition all visible items’ position.
        */
        public void OnItemSizeChanged(int itemIndex)
        {
            UIBubbleItem item = GetShownItemByItemIndex(itemIndex);
            if (item == null)
            {
                return;
            }
            if (mSupportScrollBar)
            {
                //if (mIsVertList)
                {
                    SetItemSize(itemIndex, item.GetHeight(), this.spacing);
                }
                //else
                //{
                //    SetItemSize(itemIndex, item.GetWidth(), this.spacing);
                //}
            }
            UpdateContentSize();
            UpdateAllShownItemsPos();
        }


        /*
        To update a item by itemIndex.if the itemIndex-th item is not visible, then this method will do nothing.
        Otherwise this method will first call onGetItemByIndex(itemIndex) to get a updated item and then reposition all visible items'position. 
        */
        public void RefreshItemByItemIndex(int itemIndex)
        {
            int count = mItemList.Count;
            if (count == 0)
            {
                return;
            }
            if (itemIndex < mItemList[0].ItemIndex || itemIndex > mItemList[count - 1].ItemIndex)
            {
                return;
            }
            int firstItemIndex = mItemList[0].ItemIndex;
            int i = itemIndex - firstItemIndex;
            UIBubbleItem curItem = mItemList[i];
            Vector3 pos = curItem.CachedRectTransform.anchoredPosition3D;
            RecycleItemTmp(curItem);
            UIBubbleItem newItem = GetNewItemByIndex(itemIndex);
            if (newItem == null)
            {
                RefreshAllShownItemWithFirstIndex(firstItemIndex);
                return;
            }
            mItemList[i] = newItem;
            ////if (mIsVertList)
            //{
            //    pos.x = this.padding.left;
            //}
            ////else
            //{
            //    pos.y = this.padding.top;
            //}
            newItem.CachedRectTransform.anchoredPosition3D = pos;//refresh item
            OnItemSizeChanged(itemIndex);
            ClearAllTmpRecycledItem();
        }

        //snap move will finish at once.
        public void FinishSnapImmediately()
        {
            UpdateSnapMove(true);
        }

        /*
        This method will move the scrollrect content’s position to ( the positon of itemIndex-th item + offset ),
        and offset is from 0 to scrollrect viewport size. 
        */
        public void MovePanelToItemIndex(int itemIndex, float offset = 0)
        {
            mScrollRect.StopMovement();
            mCurSnapData.Clear();
            if (mItemTotalCount == 0)
            {
                return;
            }
            if (itemIndex < 0 && mItemTotalCount > 0)
            {
                return;
            }
            if (mItemTotalCount > 0 && itemIndex >= mItemTotalCount)
            {
                itemIndex = mItemTotalCount - 1;
            }
            if (offset < 0)
            {
                offset = 0;
            }
            Vector3 pos = Vector3.zero;
            float viewPortSize = ViewPortSize;
            if (offset > viewPortSize)
            {
                offset = viewPortSize;
            }
            //if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                float containerPos = mContainerTrans.anchoredPosition3D.y;
                if (containerPos < 0)
                {
                    containerPos = 0;
                }
                pos.y = pos.y -containerPos - offset;
            }

            RecycleAllItem();
            UIBubbleItem newItem = GetNewItemByIndex(itemIndex);
            if (newItem == null)
            {
                ClearAllTmpRecycledItem();
                return;
            }

            newItem.CachedRectTransform.anchoredPosition3D = pos;//move to pos
            if (mSupportScrollBar)
            {
                SetItemSize(itemIndex, newItem.GetHeight(), this.spacing);
            }
            mItemList.Add(newItem);
            UpdateContentSize();
            UpdateListView(viewPortSize + 100, viewPortSize + 100, viewPortSize, viewPortSize);
            AdjustPanelPos();
            ClearAllTmpRecycledItem();
            ForceSnapUpdateCheck();
            UpdateSnapMove(false, true);
        }

        //update all visible items.
        public void RefreshAllShownItem()
        {
            int count = mItemList.Count;
            if (count == 0)
            {
                return;
            }
            RefreshAllShownItemWithFirstIndex(mItemList[0].ItemIndex);
        }


        public void RefreshAllShownItemWithFirstIndex(int firstItemIndex)
        {
            int count = mItemList.Count;
            if (count == 0)
            {
                return;
            }
            UIBubbleItem firstItem = mItemList[0];
            Vector3 pos = firstItem.CachedRectTransform.anchoredPosition3D;
            RecycleAllItem();
            for (int i = 0; i < count; ++i)
            {
                int curIndex = firstItemIndex + i;
                UIBubbleItem newItem = GetNewItemByIndex(curIndex);
                if (newItem == null)
                {
                    break;
                }
                ////if (mIsVertList)
                //{
                //    pos.x = this.padding.left;
                //}
                ////else
                //{
                //    pos.y = this.padding.top;
                //}
                newItem.CachedRectTransform.anchoredPosition3D = pos;
                if (mSupportScrollBar)
                {
                    //if (mIsVertList)
                    {
                        SetItemSize(curIndex, newItem.GetHeight(), this.spacing);
                    }
                    //else
                    //{
                    //    SetItemSize(curIndex, newItem.GetWidth(), this.spacing);
                    //}
                }

                mItemList.Add(newItem);
            }
            UpdateContentSize();
            UpdateAllShownItemsPos();
            ClearAllTmpRecycledItem();
        }


        public void RefreshAllShownItemWithFirstIndexAndPos(int firstItemIndex, Vector3 pos)
        {
            RecycleAllItem();
            UIBubbleItem newItem = GetNewItemByIndex(firstItemIndex);
            if (newItem == null)
            {
                return;
            }
            ////if (mIsVertList)
            //{
            //    pos.x = this.padding.left;
            //}
            ////else
            //{
            //    pos.y = this.padding.top;
            //}
            newItem.CachedRectTransform.anchoredPosition3D = pos;
            if (mSupportScrollBar)
            {
                //if (mIsVertList)
                {
                    SetItemSize(firstItemIndex, newItem.GetHeight(), this.spacing);
                }
                //else
                //{
                //    SetItemSize(firstItemIndex, newItem.GetWidth(), this.spacing);
                //}
            }
            mItemList.Add(newItem);
            UpdateContentSize();
            UpdateAllShownItemsPos();
            UpdateListView(mDistanceForRecycle0, mDistanceForRecycle1, mDistanceForNew0, mDistanceForNew1);
            ClearAllTmpRecycledItem();
        }


        void RecycleItemTmp(UIBubbleItem item)
        {
            if (item == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(item.ItemPrefabName))
            {
                return;
            }
            ItemPool pool = null;
            if (mItemPoolDict.TryGetValue(item.prefabId, out pool) == false)
            {
                return;
            }
            pool.RecycleItem(item);

        }


        void ClearAllTmpRecycledItem()
        {
            int count = mItemPoolList.Count;
            for (int i = 0; i < count; ++i)
            {
                mItemPoolList[i].ClearTmpRecycledItem();
            }
        }


        void RecycleAllItem()
        {
            foreach (UIBubbleItem item in mItemList)
            {
                RecycleItemTmp(item);
            }
            mItemList.Clear();
        }


        void AdjustContainerPivot(RectTransform rtf)
        {
            Vector2 pivot = rtf.pivot;
            //if (mArrangeType == ListItemArrangeType.BottomToTop)
            //{
            //    pivot.y = 0;
            //}
            //else if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                pivot.y = 1;
            }
            //else if (mArrangeType == ListItemArrangeType.LeftToRight)
            //{
            //    pivot.x = 0;
            //}
            //else if (mArrangeType == ListItemArrangeType.RightToLeft)
            //{
            //    pivot.x = 1;
            //}
            rtf.pivot = pivot;
        }


        void AdjustPivot(RectTransform rtf)
        {
            Vector2 pivot = rtf.pivot;

            //if (mArrangeType == ListItemArrangeType.BottomToTop)
            //{
            //    pivot.y = 0;
            //}
            //else if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                pivot.y = 1;
            }
            //else if (mArrangeType == ListItemArrangeType.LeftToRight)
            //{
            //    pivot.x = 0;
            //}
            //else if (mArrangeType == ListItemArrangeType.RightToLeft)
            //{
            //    pivot.x = 1;
            //}
            rtf.pivot = pivot;
        }

        void AdjustContainerAnchor(RectTransform rtf)
        {
            Vector2 anchorMin = rtf.anchorMin;
            Vector2 anchorMax = rtf.anchorMax;
            //if (mArrangeType == ListItemArrangeType.BottomToTop)
            //{
            //    anchorMin.y = 0;
            //    anchorMax.y = 0;
            //}
            //else if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                anchorMin.y = 1;
                anchorMax.y = 1;
            }
            //else if (mArrangeType == ListItemArrangeType.LeftToRight)
            //{
            //    anchorMin.x = 0;
            //    anchorMax.x = 0;
            //}
            //else if (mArrangeType == ListItemArrangeType.RightToLeft)
            //{
            //    anchorMin.x = 1;
            //    anchorMax.x = 1;
            //}
            rtf.anchorMin = anchorMin;
            rtf.anchorMax = anchorMax;
        }


        void AdjustAnchor(RectTransform rtf)
        {
            Vector2 anchorMin = rtf.anchorMin;
            Vector2 anchorMax = rtf.anchorMax;
            //if (mArrangeType == ListItemArrangeType.BottomToTop)
            //{
            //    anchorMin.y = 0;
            //    anchorMax.y = 0;
            //}
            //else if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                anchorMin.y = 1;
                anchorMax.y = 1;
            }
            //else if (mArrangeType == ListItemArrangeType.LeftToRight)
            //{
            //    anchorMin.x = 0;
            //    anchorMax.x = 0;
            //}
            //else if (mArrangeType == ListItemArrangeType.RightToLeft)
            //{
            //    anchorMin.x = 1;
            //    anchorMax.x = 1;
            //}
            rtf.anchorMin = anchorMin;
            rtf.anchorMax = anchorMax;
        }

        void InitItemPool()
        {
            for (int i = 0; i < mItemPrefabDataList.Count; ++i)
            {
                ItemPrefabConfData data = mItemPrefabDataList[i];
                if (data.mItemPrefab == null)
                {
                    Debug.LogError("A item prefab is null ");
                    continue;
                }

                RectTransform rtf = data.mItemPrefab.GetComponent<RectTransform>();
                if (rtf == null)
                {
                    Debug.LogError("RectTransform component is not found in the prefab: " + i);
                    continue;
                }
                AdjustAnchor(rtf);
                AdjustPivot(rtf);
                UIBubbleItem tItem = data.mItemPrefab.GetComponent<UIBubbleItem>();
                if (tItem == null)
                {
                    tItem = data.mItemPrefab.AddComponent<UIBubbleItem>();
                }
                tItem.prefabId = i;
                ItemPool pool = new ItemPool();
                pool.Init(data.mItemPrefab, data.mInitCreateCount, mContainerTrans);
                mItemPoolDict.Add(i, pool);
                mItemPoolList.Add(pool);
            }
        }



        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            mIsDraging = true;
            CacheDragPointerEventData(eventData);
            mCurSnapData.Clear();
            if (mOnBeginDragAction != null)
            {
                mOnBeginDragAction();
            }
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            mIsDraging = false;
            mPointerEventData = null;
            if (mOnEndDragAction != null)
            {
                mOnEndDragAction();
            }
            ForceSnapUpdateCheck();
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            CacheDragPointerEventData(eventData);
            if (mOnDragingAction != null)
            {
                mOnDragingAction();
            }
        }

        void CacheDragPointerEventData(PointerEventData eventData)
        {
            if (mPointerEventData == null)
            {
                mPointerEventData = new PointerEventData(EventSystem.current);
            }
            mPointerEventData.button = eventData.button;
            mPointerEventData.position = eventData.position;
            mPointerEventData.pointerPressRaycast = eventData.pointerPressRaycast;
            mPointerEventData.pointerCurrentRaycast = eventData.pointerCurrentRaycast;
        }

        UIBubbleItem GetNewItemByIndex(int index)
        {
            if (mSupportScrollBar && index < 0)
            {
                return null;
            }
            if (mItemTotalCount > 0 && index >= mItemTotalCount)
            {
                return null;
            }
            UIBubbleItem newItem = mOnGetItemByIndex(this, index);
            if (newItem == null)
            {
                return null;
            }
            newItem.ItemIndex = index;
            newItem.ItemCreatedCheckFrameCount = mListUpdateCheckFrameCount;
            return newItem;
        }


        void SetItemSize(int itemIndex, float itemSize, float padding)
        {
            mItemPosMgr.SetItemSize(itemIndex, itemSize + padding);
            if (itemIndex >= mLastItemIndex)
            {
                mLastItemIndex = itemIndex;
                mLastItemPadding = padding;
            }
        }

        bool GetPlusItemIndexAndPosAtGivenPos(float pos, ref int index, ref float itemPos)
        {
            return mItemPosMgr.GetItemIndexAndPosAtGivenPos(pos, ref index, ref itemPos);
        }


        float GetItemPos(int itemIndex)
        {
            return mItemPosMgr.GetItemPos(itemIndex);
        }


        public Vector3 GetItemCornerPosInViewPort(UIBubbleItem item, ItemCornerEnum corner = ItemCornerEnum.LeftBottom)
        {
            item.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
            return mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[(int)corner]);
        }


        void AdjustPanelPos()
        {
            int count = mItemList.Count;
            if (count == 0)
            {
                return;
            }
            UpdateAllShownItemsPos();
            float viewPortSize = ViewPortSize;
            float contentSize = GetContentPanelSize();

            if (contentSize <= viewPortSize)
            {
                Vector3 pos = mContainerTrans.anchoredPosition3D;
                pos.y = 0;
                SetContentPos(pos);
                mItemList[0].CachedRectTransform.anchoredPosition3D = new Vector3(0, 0, 0);//adjust
                UpdateAllShownItemsPos();
                return;
            }
            UIBubbleItem tViewItem0 = mItemList[0];
            tViewItem0.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
            Vector3 topPos0 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]);
            if (topPos0.y < mViewPortRectLocalCorners[1].y)
            {
                Vector3 pos = mContainerTrans.anchoredPosition3D;
                pos.y = 0;
                SetContentPos(pos);
                mItemList[0].CachedRectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
                UpdateAllShownItemsPos();
                return;
            }
            UIBubbleItem tViewItem1 = mItemList[mItemList.Count - 1];
            tViewItem1.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
            Vector3 downPos1 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[0]);
            downPos1.y -= this.padding.bottom;
            float d = downPos1.y - mViewPortRectLocalCorners[0].y;
            if (d > 0)
            {
                Vector3 pos = mItemList[0].CachedRectTransform.anchoredPosition3D;
                pos.y = pos.y - d;
                mItemList[0].CachedRectTransform.anchoredPosition3D = pos;
                UpdateAllShownItemsPos();
                return;
            }
        }


        void Update()
        {
            if (mListViewInited == false)
            {
                return;
            }
            if (mNeedAdjustVec)
            {
                mNeedAdjustVec = false;
                //if (mIsVertList)
                {
                    if (mScrollRect.velocity.y * mAdjustedVec.y > 0)
                    {
                        mScrollRect.velocity = mAdjustedVec;
                    }
                }
                //else
                //{
                //    if (mScrollRect.velocity.x * mAdjustedVec.x > 0)
                //    {
                //        mScrollRect.velocity = mAdjustedVec;
                //    }
                //}

            }
            if (mSupportScrollBar)
            {
                mItemPosMgr.Update(false);
            }
            UpdateSnapMove();
            UpdateListView(mDistanceForRecycle0, mDistanceForRecycle1, mDistanceForNew0, mDistanceForNew1);
            ClearAllTmpRecycledItem();
            mLastFrameContainerPos = mContainerTrans.anchoredPosition3D;
        }

        //update snap move. if immediate is set true, then the snap move will finish at once.
        void UpdateSnapMove(bool immediate = false, bool forceSendEvent = false)
        {
            if (mItemSnapEnable == false)
            {
                return;
            }
            //if (mIsVertList)
            {
                UpdateSnapVertical(immediate, forceSendEvent);
            }
            //else
            //{
            //    UpdateSnapHorizontal(immediate, forceSendEvent);
            //}
        }



        public void UpdateAllShownItemSnapData()
        {
            if (mItemSnapEnable == false)
            {
                return;
            }
            int count = mItemList.Count;
            if (count == 0)
            {
                return;
            }
            Vector3 pos = mContainerTrans.anchoredPosition3D;
            UIBubbleItem tViewItem0 = mItemList[0];
            tViewItem0.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
            float start = 0;
            float end = 0;
            float itemSnapCenter = 0;
            float snapCenter = 0;
            //if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                snapCenter = -(1 - mViewPortSnapPivot.y) * mViewPortRectTransform.rect.height;
                Vector3 topPos1 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]);
                start = topPos1.y;
                end = start - tViewItem0.ItemSizeWithPadding;
                itemSnapCenter = start - tViewItem0.ItemSize * (1 - mItemSnapPivot.y);
                for (int i = 0; i < count; ++i)
                {
                    mItemList[i].DistanceWithViewPortSnapCenter = snapCenter - itemSnapCenter;
                    if ((i + 1) < count)
                    {
                        start = end;
                        end = end - mItemList[i + 1].ItemSizeWithPadding;
                        itemSnapCenter = start - mItemList[i + 1].ItemSize * (1 - mItemSnapPivot.y);
                    }
                }
            }
            //else if (mArrangeType == ListItemArrangeType.BottomToTop)
            //{
            //    snapCenter = mViewPortSnapPivot.y * mViewPortRectTransform.rect.height;
            //    Vector3 bottomPos1 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[0]);
            //    start = bottomPos1.y;
            //    end = start + tViewItem0.ItemSizeWithPadding;
            //    itemSnapCenter = start + tViewItem0.ItemSize * mItemSnapPivot.y;
            //    for (int i = 0; i < count; ++i)
            //    {
            //        mItemList[i].DistanceWithViewPortSnapCenter = snapCenter - itemSnapCenter;
            //        if ((i + 1) < count)
            //        {
            //            start = end;
            //            end = end + mItemList[i + 1].ItemSizeWithPadding;
            //            itemSnapCenter = start + mItemList[i + 1].ItemSize * mItemSnapPivot.y;
            //        }
            //    }
            //}
            //else if (mArrangeType == ListItemArrangeType.RightToLeft)
            //{
            //    snapCenter = -(1 - mViewPortSnapPivot.x) * mViewPortRectTransform.rect.width;
            //    Vector3 rightPos1 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[2]);
            //    start = rightPos1.x;
            //    end = start - tViewItem0.ItemSizeWithPadding;
            //    itemSnapCenter = start - tViewItem0.ItemSize * (1 - mItemSnapPivot.x);
            //    for (int i = 0; i < count; ++i)
            //    {
            //        mItemList[i].DistanceWithViewPortSnapCenter = snapCenter - itemSnapCenter;
            //        if ((i + 1) < count)
            //        {
            //            start = end;
            //            end = end - mItemList[i + 1].ItemSizeWithPadding;
            //            itemSnapCenter = start - mItemList[i + 1].ItemSize * (1 - mItemSnapPivot.x);
            //        }
            //    }
            //}
            //else if (mArrangeType == ListItemArrangeType.LeftToRight)
            //{
            //    snapCenter = mViewPortSnapPivot.x * mViewPortRectTransform.rect.width;
            //    Vector3 leftPos1 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]);
            //    start = leftPos1.x;
            //    end = start + tViewItem0.ItemSizeWithPadding;
            //    itemSnapCenter = start + tViewItem0.ItemSize * mItemSnapPivot.x;
            //    for (int i = 0; i < count; ++i)
            //    {
            //        mItemList[i].DistanceWithViewPortSnapCenter = snapCenter - itemSnapCenter;
            //        if ((i + 1) < count)
            //        {
            //            start = end;
            //            end = end + mItemList[i + 1].ItemSizeWithPadding;
            //            itemSnapCenter = start + mItemList[i + 1].ItemSize * mItemSnapPivot.x;
            //        }
            //    }
            //}
        }



        void UpdateSnapVertical(bool immediate = false, bool forceSendEvent = false)
        {
            if (mItemSnapEnable == false)
            {
                return;
            }
            int count = mItemList.Count;
            if (count == 0)
            {
                return;
            }
            Vector3 pos = mContainerTrans.anchoredPosition3D;
            bool needCheck = (pos.y != mLastSnapCheckPos.y);
            mLastSnapCheckPos = pos;
            if (!needCheck)
            {
                if (mLeftSnapUpdateExtraCount > 0)
                {
                    mLeftSnapUpdateExtraCount--;
                    needCheck = true;
                }
            }
            if (needCheck)
            {
                UIBubbleItem tViewItem0 = mItemList[0];
                tViewItem0.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
                int curIndex = -1;
                float start = 0;
                float end = 0;
                float itemSnapCenter = 0;
                float curMinDist = float.MaxValue;
                float curDist = 0;
                float curDistAbs = 0;
                float snapCenter = 0;
                //if (mArrangeType == ListItemArrangeType.TopToBottom)
                {
                    snapCenter = -(1 - mViewPortSnapPivot.y) * mViewPortRectTransform.rect.height;
                    Vector3 topPos1 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]);
                    start = topPos1.y;
                    end = start - tViewItem0.ItemSizeWithPadding;
                    itemSnapCenter = start - tViewItem0.ItemSize * (1 - mItemSnapPivot.y);
                    for (int i = 0; i < count; ++i)
                    {
                        curDist = snapCenter - itemSnapCenter;
                        curDistAbs = Mathf.Abs(curDist);
                        if (curDistAbs < curMinDist)
                        {
                            curMinDist = curDistAbs;
                            curIndex = i;
                        }
                        else
                        {
                            break;
                        }

                        if ((i + 1) < count)
                        {
                            start = end;
                            end = end - mItemList[i + 1].ItemSizeWithPadding;
                            itemSnapCenter = start - mItemList[i + 1].ItemSize * (1 - mItemSnapPivot.y);
                        }
                    }
                }
                //else if (mArrangeType == ListItemArrangeType.BottomToTop)
                //{
                //    snapCenter = mViewPortSnapPivot.y * mViewPortRectTransform.rect.height;
                //    Vector3 bottomPos1 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[0]);
                //    start = bottomPos1.y;
                //    end = start + tViewItem0.ItemSizeWithPadding;
                //    itemSnapCenter = start + tViewItem0.ItemSize * mItemSnapPivot.y;
                //    for (int i = 0; i < count; ++i)
                //    {
                //        curDist = snapCenter - itemSnapCenter;
                //        curDistAbs = Mathf.Abs(curDist);
                //        if (curDistAbs < curMinDist)
                //        {
                //            curMinDist = curDistAbs;
                //            curIndex = i;
                //        }
                //        else
                //        {
                //            break;
                //        }

                //        if ((i + 1) < count)
                //        {
                //            start = end;
                //            end = end + mItemList[i + 1].ItemSizeWithPadding;
                //            itemSnapCenter = start + mItemList[i + 1].ItemSize * mItemSnapPivot.y;
                //        }
                //    }
                //}

                if (curIndex >= 0)
                {
                    int oldNearestItemIndex = mCurSnapNearestItemIndex;
                    mCurSnapNearestItemIndex = mItemList[curIndex].ItemIndex;
                    if (forceSendEvent || mItemList[curIndex].ItemIndex != oldNearestItemIndex)
                    {
                        if (mOnSnapNearestChanged != null)
                        {
                            mOnSnapNearestChanged(this, mItemList[curIndex]);
                        }
                    }
                }
                else
                {
                    mCurSnapNearestItemIndex = -1;
                }
            }
            if (CanSnap() == false)
            {
                ClearSnapData();
                return;
            }
            float v = Mathf.Abs(mScrollRect.velocity.y);
            UpdateCurSnapData();
            if (mCurSnapData.mSnapStatus != SnapStatus.SnapMoving)
            {
                return;
            }
            if (v > 0)
            {
                mScrollRect.StopMovement();
            }
            float old = mCurSnapData.mCurSnapVal;
            if (mCurSnapData.mIsTempTarget == false)
            {
                if (mSmoothDumpVel * mCurSnapData.mTargetSnapVal < 0)
                {
                    mSmoothDumpVel = 0;
                }
                mCurSnapData.mCurSnapVal = Mathf.SmoothDamp(mCurSnapData.mCurSnapVal, mCurSnapData.mTargetSnapVal, ref mSmoothDumpVel, mSmoothDumpRate);
            }
            else
            {
                float maxAbsVec = mCurSnapData.mMoveMaxAbsVec;
                if (maxAbsVec <= 0)
                {
                    maxAbsVec = mSnapMoveDefaultMaxAbsVec;
                }
                mSmoothDumpVel = maxAbsVec * Mathf.Sign(mCurSnapData.mTargetSnapVal);
                mCurSnapData.mCurSnapVal = Mathf.MoveTowards(mCurSnapData.mCurSnapVal, mCurSnapData.mTargetSnapVal, maxAbsVec * Time.deltaTime);
            }
            float dt = mCurSnapData.mCurSnapVal - old;

            if (immediate || Mathf.Abs(mCurSnapData.mTargetSnapVal - mCurSnapData.mCurSnapVal) < mSnapFinishThreshold)
            {
                pos.y = pos.y + mCurSnapData.mTargetSnapVal - old;
                mCurSnapData.mSnapStatus = SnapStatus.SnapMoveFinish;
                if (mOnSnapItemFinished != null)
                {
                    UIBubbleItem targetItem = GetShownItemByItemIndex(mCurSnapNearestItemIndex);
                    if (targetItem != null)
                    {
                        mOnSnapItemFinished(this, targetItem);
                    }
                }
            }
            else
            {
                pos.y = pos.y + dt;
            }

            //if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                float maxY = mViewPortRectLocalCorners[0].y + mContainerTrans.rect.height;
                pos.y = Mathf.Clamp(pos.y, 0, maxY);
                SetContentPos(pos);
            }
            //else if (mArrangeType == ListItemArrangeType.BottomToTop)
            //{
            //    float minY = mViewPortRectLocalCorners[1].y - mContainerTrans.rect.height;
            //    pos.y = Mathf.Clamp(pos.y, minY, 0);
            //    mContainerTrans.anchoredPosition3D = pos;
            //}

        }

        void UpdateCurSnapData()
        {
            int count = mItemList.Count;
            if (count == 0)
            {
                mCurSnapData.Clear();
                return;
            }

            if (mCurSnapData.mSnapStatus == SnapStatus.SnapMoveFinish)
            {
                if (mCurSnapData.mSnapTargetIndex == mCurSnapNearestItemIndex)
                {
                    return;
                }
                mCurSnapData.mSnapStatus = SnapStatus.NoTargetSet;
            }
            if (mCurSnapData.mSnapStatus == SnapStatus.SnapMoving)
            {
                if (mCurSnapData.mIsForceSnapTo)
                {
                    if (mCurSnapData.mIsTempTarget == true)
                    {
                        UIBubbleItem targetItem = GetShownItemNearestItemIndex(mCurSnapData.mSnapTargetIndex);
                        if (targetItem == null)
                        {
                            mCurSnapData.Clear();
                            return;
                        }
                        if (targetItem.ItemIndex == mCurSnapData.mSnapTargetIndex)
                        {
                            UpdateAllShownItemSnapData();
                            mCurSnapData.mTargetSnapVal = targetItem.DistanceWithViewPortSnapCenter;
                            mCurSnapData.mCurSnapVal = 0;
                            mCurSnapData.mIsTempTarget = false;
                            mCurSnapData.mSnapStatus = SnapStatus.SnapMoving;
                            return;
                        }
                        if (mCurSnapData.mTempTargetIndex != targetItem.ItemIndex)
                        {
                            UpdateAllShownItemSnapData();
                            mCurSnapData.mTargetSnapVal = targetItem.DistanceWithViewPortSnapCenter;
                            mCurSnapData.mCurSnapVal = 0;
                            mCurSnapData.mSnapStatus = SnapStatus.SnapMoving;
                            mCurSnapData.mIsTempTarget = true;
                            mCurSnapData.mTempTargetIndex = targetItem.ItemIndex;
                            return;
                        }
                    }
                    return;
                }
                if ((mCurSnapData.mSnapTargetIndex == mCurSnapNearestItemIndex))
                {
                    return;
                }
                mCurSnapData.mSnapStatus = SnapStatus.NoTargetSet;
            }
            if (mCurSnapData.mSnapStatus == SnapStatus.NoTargetSet)
            {
                UIBubbleItem nearestItem = GetShownItemByItemIndex(mCurSnapNearestItemIndex);
                if (nearestItem == null)
                {
                    return;
                }
                mCurSnapData.mSnapTargetIndex = mCurSnapNearestItemIndex;
                mCurSnapData.mSnapStatus = SnapStatus.TargetHasSet;
                mCurSnapData.mIsForceSnapTo = false;
            }
            if (mCurSnapData.mSnapStatus == SnapStatus.TargetHasSet)
            {
                UIBubbleItem targetItem = GetShownItemNearestItemIndex(mCurSnapData.mSnapTargetIndex);
                if (targetItem == null)
                {
                    mCurSnapData.Clear();
                    return;
                }
                if (targetItem.ItemIndex == mCurSnapData.mSnapTargetIndex)
                {
                    UpdateAllShownItemSnapData();
                    mCurSnapData.mTargetSnapVal = targetItem.DistanceWithViewPortSnapCenter;
                    mCurSnapData.mCurSnapVal = 0;
                    mCurSnapData.mIsTempTarget = false;
                    mCurSnapData.mSnapStatus = SnapStatus.SnapMoving;
                }
                else
                {
                    UpdateAllShownItemSnapData();
                    mCurSnapData.mTargetSnapVal = targetItem.DistanceWithViewPortSnapCenter;
                    mCurSnapData.mCurSnapVal = 0;
                    mCurSnapData.mSnapStatus = SnapStatus.SnapMoving;
                    mCurSnapData.mIsTempTarget = true;
                    mCurSnapData.mTempTargetIndex = targetItem.ItemIndex;
                }

            }

        }
        //Clear current snap target and then the LoopScrollView2 will auto snap to the CurSnapNearestItemIndex.
        public void ClearSnapData()
        {
            mCurSnapData.Clear();
        }

        //moveMaxAbsVec param is the max abs snap move speed, if the value <= 0 then LoopListView2 would use SnapMoveDefaultMaxAbsVec
        public void SetSnapTargetItemIndex(int itemIndex, float moveMaxAbsVec = -1)
        {
            if (mItemTotalCount > 0)
            {
                if (itemIndex >= mItemTotalCount)
                {
                    itemIndex = mItemTotalCount - 1;
                }
                if (itemIndex < 0)
                {
                    itemIndex = 0;
                }
            }
            mScrollRect.StopMovement();
            mCurSnapData.mSnapTargetIndex = itemIndex;
            mCurSnapData.mSnapStatus = SnapStatus.TargetHasSet;
            mCurSnapData.mIsForceSnapTo = true;
            mCurSnapData.mMoveMaxAbsVec = moveMaxAbsVec;
        }

        //Get the nearest item index with the viewport snap point.
        public int CurSnapNearestItemIndex
        {
            get { return mCurSnapNearestItemIndex; }
        }

        public void ForceSnapUpdateCheck()
        {
            if (mLeftSnapUpdateExtraCount <= 0)
            {
                mLeftSnapUpdateExtraCount = 1;
            }
        }
        
        bool CanSnap()
        {
            if (mIsDraging)
            {
                return false;
            }
            if (mScrollBarClickEventListener != null)
            {
                if (mScrollBarClickEventListener.IsPressd)
                {
                    return false;
                }
            }

            //if (mIsVertList)
            {
                if (mContainerTrans.rect.height <= ViewPortHeight)
                {
                    return false;
                }
            }
            //else
            //{
            //    if (mContainerTrans.rect.width <= ViewPortWidth)
            //    {
            //        return false;
            //    }
            //}

            float v = 0;
            //if (mIsVertList)
            {
                v = Mathf.Abs(mScrollRect.velocity.y);
            }
            //else
            //{
            //    v = Mathf.Abs(mScrollRect.velocity.x);
            //}
            if (v > mSnapVecThreshold)
            {
                return false;
            }
            float diff = 3;
            Vector3 pos = mContainerTrans.anchoredPosition3D;
            //if (mArrangeType == ListItemArrangeType.LeftToRight)
            //{
            //    float minX = mViewPortRectLocalCorners[2].x - mContainerTrans.rect.width;
            //    if (pos.x < (minX - diff) || pos.x > diff)
            //    {
            //        return false;
            //    }
            //}
            //else if (mArrangeType == ListItemArrangeType.RightToLeft)
            //{
            //    float maxX = mViewPortRectLocalCorners[1].x + mContainerTrans.rect.width;
            //    if (pos.x > (maxX + diff) || pos.x < -diff)
            //    {
            //        return false;
            //    }
            //}
            //else if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                float maxY = mViewPortRectLocalCorners[0].y + mContainerTrans.rect.height;
                if (pos.y > (maxY + diff) || pos.y < -diff)
                {
                    return false;
                }
            }
            //else if (mArrangeType == ListItemArrangeType.BottomToTop)
            //{
            //    float minY = mViewPortRectLocalCorners[1].y - mContainerTrans.rect.height;
            //    if (pos.y < (minY - diff) || pos.y > diff)
            //    {
            //        return false;
            //    }
            //}
            return true;
        }



        public void UpdateListView(float distanceForRecycle0, float distanceForRecycle1, float distanceForNew0, float distanceForNew1)
        {
            mListUpdateCheckFrameCount++;
            //if (mIsVertList)
            {
                bool needContinueCheck = true;
                int checkCount = 0;
                int maxCount = 9999;
                while (needContinueCheck)
                {
                    checkCount++;
                    if (checkCount >= maxCount)
                    {
                        Debug.LogError("UpdateListView Vertical while loop " + checkCount + " times! something is wrong!");
                        break;
                    }
                    needContinueCheck = UpdateForVertList(distanceForRecycle0, distanceForRecycle1, distanceForNew0, distanceForNew1);
                }
            }
            //else
            //{
            //    bool needContinueCheck = true;
            //    int checkCount = 0;
            //    int maxCount = 9999;
            //    while (needContinueCheck)
            //    {
            //        checkCount++;
            //        if (checkCount >= maxCount)
            //        {
            //            Debug.LogError("UpdateListView  Horizontal while loop " + checkCount + " times! something is wrong!");
            //            break;
            //        }
            //        needContinueCheck = UpdateForHorizontalList(distanceForRecycle0, distanceForRecycle1, distanceForNew0, distanceForNew1);
            //    }
            //}

        }



        bool UpdateForVertList(float distanceForRecycle0, float distanceForRecycle1, float distanceForNew0, float distanceForNew1)
        {
            if (mItemTotalCount == 0)
            {
                if (mItemList.Count > 0)
                {
                    RecycleAllItem();
                }
                return false;
            }
            //if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                int itemListCount = mItemList.Count;
                if (itemListCount == 0)
                {
                    float curY = mContainerTrans.anchoredPosition3D.y;
                    if (curY < 0)
                    {
                        curY = 0;
                    }
                    int index = 0;
                    float pos = -curY;
                    if (mSupportScrollBar)
                    {
                        if (GetPlusItemIndexAndPosAtGivenPos(curY, ref index, ref pos) == false)
                        {
                            return false;
                        }
                        pos = -pos;
                    }
                    UIBubbleItem newItem = GetNewItemByIndex(index);
                    if (newItem == null)
                    {
                        return false;
                    }
                    if (mSupportScrollBar)
                    {
                        SetItemSize(index, newItem.GetHeight(), this.spacing);
                    }
                    mItemList.Add(newItem);
                    newItem.CachedRectTransform.anchoredPosition3D = new Vector3(this.padding.left, pos, 0);//update
                    UpdateContentSize();
                    return true;
                }
                UIBubbleItem tViewItem0 = mItemList[0];
                tViewItem0.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
                Vector3 topPos0 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]);
                Vector3 downPos0 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[0]);

                if (!mIsDraging && tViewItem0.ItemCreatedCheckFrameCount != mListUpdateCheckFrameCount
                    && downPos0.y - mViewPortRectLocalCorners[1].y > distanceForRecycle0)
                {
                    mItemList.RemoveAt(0);
                    RecycleItemTmp(tViewItem0);
                    if (!mSupportScrollBar)
                    {
                        UpdateContentSize();
                        CheckIfNeedUpdataItemPos();
                    }
                    return true;
                }

                UIBubbleItem tViewItem1 = mItemList[mItemList.Count - 1];
                tViewItem1.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
                Vector3 topPos1 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]);
                Vector3 downPos1 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[0]);
                if (!mIsDraging && tViewItem1.ItemCreatedCheckFrameCount != mListUpdateCheckFrameCount
                    && mViewPortRectLocalCorners[0].y - topPos1.y > distanceForRecycle1)
                {
                    mItemList.RemoveAt(mItemList.Count - 1);
                    RecycleItemTmp(tViewItem1);
                    if (!mSupportScrollBar)
                    {
                        UpdateContentSize();
                        CheckIfNeedUpdataItemPos();
                    }
                    return true;
                }



                if (mViewPortRectLocalCorners[0].y - downPos1.y < distanceForNew1)
                {
                    if (tViewItem1.ItemIndex > mCurReadyMaxItemIndex)
                    {
                        mCurReadyMaxItemIndex = tViewItem1.ItemIndex;
                        mNeedCheckNextMaxItem = true;
                    }
                    int nIndex = tViewItem1.ItemIndex + 1;
                    if (nIndex <= mCurReadyMaxItemIndex || mNeedCheckNextMaxItem)
                    {
                        UIBubbleItem newItem = GetNewItemByIndex(nIndex);
                        if (newItem == null)
                        {
                            mCurReadyMaxItemIndex = tViewItem1.ItemIndex;
                            mNeedCheckNextMaxItem = false;
                            CheckIfNeedUpdataItemPos();
                        }
                        else
                        {
                            if (mSupportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.GetHeight(), this.spacing);
                            }
                            mItemList.Add(newItem);
                            float y = tViewItem1.CachedRectTransform.anchoredPosition3D.y - tViewItem1.GetHeight() - this.spacing - this.padding.top;
                            newItem.CachedRectTransform.anchoredPosition3D = new Vector3(this.padding.left, y, 0);
                            UpdateContentSize();
                            CheckIfNeedUpdataItemPos();

                            if (nIndex > mCurReadyMaxItemIndex)
                            {
                                mCurReadyMaxItemIndex = nIndex;
                            }
                            return true;
                        }

                    }

                }

                if (topPos0.y - mViewPortRectLocalCorners[1].y < distanceForNew0)
                {
                    if (tViewItem0.ItemIndex < mCurReadyMinItemIndex)
                    {
                        mCurReadyMinItemIndex = tViewItem0.ItemIndex;
                        mNeedCheckNextMinItem = true;
                    }
                    int nIndex = tViewItem0.ItemIndex - 1;
                    if (nIndex >= mCurReadyMinItemIndex || mNeedCheckNextMinItem)
                    {
                        UIBubbleItem newItem = GetNewItemByIndex(nIndex);
                        if (newItem == null)
                        {
                            mCurReadyMinItemIndex = tViewItem0.ItemIndex;
                            mNeedCheckNextMinItem = false;
                        }
                        else
                        {
                            if (mSupportScrollBar)
                            {
                                SetItemSize(nIndex, newItem.GetHeight(), this.spacing);
                            }
                            mItemList.Insert(0, newItem);
                            float y = tViewItem0.CachedRectTransform.anchoredPosition3D.y + newItem.GetHeight() + this.spacing;
                            newItem.CachedRectTransform.anchoredPosition3D = new Vector3(this.padding.left, y, 0);
                            UpdateContentSize();
                            CheckIfNeedUpdataItemPos();
                            if (nIndex < mCurReadyMinItemIndex)
                            {
                                mCurReadyMinItemIndex = nIndex;
                            }
                            return true;
                        }

                    }

                }

            }
            

            return false;

        }



        





        float GetContentPanelSize()
        {
            if (mSupportScrollBar)
            {
                float tTotalSize = mItemPosMgr.TotalSize > 0 ? (mItemPosMgr.TotalSize - mLastItemPadding) : 0;
                if (tTotalSize < 0)
                {
                    tTotalSize = 0;
                }
                return tTotalSize;
            }
            int count = mItemList.Count;
            if (count == 0)
            {
                return 0;
            }
            if (count == 1)
            {
                return mItemList[0].ItemSize;
            }
            if (count == 2)
            {
                return mItemList[0].ItemSizeWithPadding + mItemList[1].ItemSize;
            }
            float s = 0;
            for (int i = 0; i < count - 1; ++i)
            {
                s += mItemList[i].ItemSizeWithPadding;
            }
            s += mItemList[count - 1].ItemSize;
            return s;
        }


        void CheckIfNeedUpdataItemPos()
        {
            int count = mItemList.Count;
            if (count == 0)
            {
                return;
            }
            //if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                UIBubbleItem firstItem = mItemList[0];
                UIBubbleItem lastItem = mItemList[mItemList.Count - 1];
                float viewMaxY = GetContentPanelSize();
                if (firstItem.TopY > 0 || (firstItem.ItemIndex == mCurReadyMinItemIndex && firstItem.TopY != 0))
                {
                    UpdateAllShownItemsPos();
                    return;
                }
                if ((-lastItem.BottomY) > viewMaxY || (lastItem.ItemIndex == mCurReadyMaxItemIndex && (-lastItem.BottomY) != viewMaxY))
                {
                    UpdateAllShownItemsPos();
                    return;
                }

            }
        }


        void UpdateAllShownItemsPos()
        {
            int count = mItemList.Count;
            if (count == 0)
            {
                return;
            }

            mAdjustedVec = (mContainerTrans.anchoredPosition3D - mLastFrameContainerPos) / Time.deltaTime;

            //if (mArrangeType == ListItemArrangeType.TopToBottom)
            {
                float item0Pos = 0;
                if (mSupportScrollBar)
                {
                    item0Pos = -GetItemPos(mItemList[0].ItemIndex);
                }
                float item0LastPos = mItemList[0].CachedRectTransform.anchoredPosition3D.y;
                float diff = item0Pos - item0LastPos;
                float curY = item0Pos;
                for (int i = 0; i < count; ++i)
                {
                    UIBubbleItem item = mItemList[i];
                    item.CachedRectTransform.anchoredPosition3D = new Vector3(this.padding.left, curY, 0);
                    curY = curY - item.GetHeight() - this.spacing;
                }
                if (diff != 0)
                {
                    Vector2 p = mContainerTrans.anchoredPosition3D;
                    p.y = p.y - diff;
                    SetContentPos(p);
                }

            }
            if (mIsDraging)
            {
                mScrollRect.OnBeginDrag(mPointerEventData);
                mScrollRect.Rebuild(CanvasUpdate.PostLayout);
                mScrollRect.velocity = mAdjustedVec;
                mNeedAdjustVec = true;
            }
        }
        void UpdateContentSize()
        {
            float size = GetContentPanelSize();
            //if (mIsVertList)
            {
                if (mContainerTrans.rect.height != size)
                {
                    mContainerTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
                }
            }
            //else
            //{
            //    if (mContainerTrans.rect.width != size)
            //    {
            //        mContainerTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
            //    }
            //}
        }

        public void SetContentPos(Vector3 pos)
        {
            float viewPortSize = ViewPortSize;
            float contentSize = GetContentPanelSize();

            if (contentSize <= viewPortSize)
            {
                if(Math.Abs(pos.y) > 1)
                {
                    //LOG.Error("pos:" + pos);
                }
                pos.y = 0;
            }
            mContainerTrans.anchoredPosition3D = pos;
        }
    }

}

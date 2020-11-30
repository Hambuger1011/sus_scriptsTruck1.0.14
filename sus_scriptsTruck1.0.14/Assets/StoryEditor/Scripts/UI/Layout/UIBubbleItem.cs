namespace StoryEditor
{
    using SuperScrollView;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    [System.Serializable]
    public class ItemPrefabConfData
    {
        public GameObject mItemPrefab = null;
   
        [Header("初始化数量")]
        public int mInitCreateCount = 0;
    }


    public class LoopListViewInitParam
    {
        // all the default values
        public float mDistanceForRecycle0 = 300; //mDistanceForRecycle0 should be larger than mDistanceForNew0
        public float mDistanceForNew0 = 200;
        public float mDistanceForRecycle1 = 300;//mDistanceForRecycle1 should be larger than mDistanceForNew1
        public float mDistanceForNew1 = 200;
        public float mSmoothDumpRate = 0.3f;
        public float mSnapFinishThreshold = 0.01f;
        public float mSnapVecThreshold = 145;
        public float mItemDefaultWithPaddingSize = 20;//item's default size (with padding)

        public static LoopListViewInitParam CopyDefaultInitParam()
        {
            return new LoopListViewInitParam();
        }
    }

    //[System.Serializable]
    //public struct RectOffset
    //{
    //    public float left;
    //    public float right;
    //    public float top;
    //    public float bottom;
    //    public float horizontal;
    //    public int vertical;
    //}

    public class UIBubbleItem : MonoBehaviour
    {
        public int prefabId;
        // indicates the item’s index in the list
        //if itemTotalCount is set -1, then the mItemIndex can be from –MaxInt to +MaxInt.
        //If itemTotalCount is set a value >=0 , then the mItemIndex can only be from 0 to itemTotalCount -1.
        int mItemIndex = -1;

        //ndicates the item’s id. 
        //This property is set when the item is created or fetched from pool, 
        //and will no longer change until the item is recycled back to pool.
        int mItemId = -1;

        UIBubbleLayout mParentListView = null;
        bool mIsInitHandlerCalled = false;
        string mItemPrefabName;
        RectTransform mCachedRectTransform;
  
        float mDistanceWithViewPortSnapCenter = 0;
        int mItemCreatedCheckFrameCount = 0;

        object mUserObjectData = null;
        int mUserIntData1 = 0;
        int mUserIntData2 = 0;
        string mUserStringData1 = null;
        string mUserStringData2 = null;

        public object UserObjectData
        {
            get { return mUserObjectData; }
            set { mUserObjectData = value; }
        }
        public int UserIntData1
        {
            get { return mUserIntData1; }
            set { mUserIntData1 = value; }
        }
        public int UserIntData2
        {
            get { return mUserIntData2; }
            set { mUserIntData2 = value; }
        }
        public string UserStringData1
        {
            get { return mUserStringData1; }
            set { mUserStringData1 = value; }
        }
        public string UserStringData2
        {
            get { return mUserStringData2; }
            set { mUserStringData2 = value; }
        }

        public float DistanceWithViewPortSnapCenter
        {
            get { return mDistanceWithViewPortSnapCenter; }
            set { mDistanceWithViewPortSnapCenter = value; }
        }

        public int ItemCreatedCheckFrameCount
        {
            get { return mItemCreatedCheckFrameCount; }
            set { mItemCreatedCheckFrameCount = value; }
        }
        

        public RectTransform CachedRectTransform
        {
            get
            {
                if (mCachedRectTransform == null)
                {
                    mCachedRectTransform = gameObject.GetComponent<RectTransform>();
                }
                return mCachedRectTransform;
            }
        }

        public System.Func<Vector2> _onGetSize;

        public float GetHeight()
        {
            if(_onGetSize != null)
            {
                return _onGetSize().y;
            }
            return this.CachedRectTransform.rect.height;
        }

        public float GetWidth()
        {
            if (_onGetSize != null)
            {
                return _onGetSize().x;
            }
            return this.CachedRectTransform.rect.width;
        }

        public string ItemPrefabName
        {
            get
            {
                return mItemPrefabName;
            }
            set
            {
                mItemPrefabName = value;
            }
        }

        public int ItemIndex
        {
            get
            {
                return mItemIndex;
            }
            set
            {
                mItemIndex = value;
            }
        }
        public int ItemId
        {
            get
            {
                return mItemId;
            }
            set
            {
                mItemId = value;
            }
        }


        public bool IsInitHandlerCalled
        {
            get
            {
                return mIsInitHandlerCalled;
            }
            set
            {
                mIsInitHandlerCalled = value;
            }
        }

        public UIBubbleLayout ParentListView
        {
            get
            {
                return mParentListView;
            }
            set
            {
                mParentListView = value;
            }
        }

        public float TopY
        {
            get
            {
                //ListItemArrangeType arrageType = ParentListView.ArrangeType;
                //if (arrageType == ListItemArrangeType.TopToBottom)
                {
                    return CachedRectTransform.anchoredPosition3D.y;
                }
                //else if(arrageType == ListItemArrangeType.BottomToTop)
                //{
                //    return CachedRectTransform.anchoredPosition3D.y + CachedRectTransform.rect.height;
                //}
                //return 0;
            }
        }

        public float BottomY
        {
            get
            {
                //ListItemArrangeType arrageType = ParentListView.ArrangeType;
                //if (arrageType == ListItemArrangeType.TopToBottom)
                {
                    return CachedRectTransform.anchoredPosition3D.y - CachedRectTransform.rect.height;
                }
                //else if (arrageType == ListItemArrangeType.BottomToTop)
                //{
                //    return CachedRectTransform.anchoredPosition3D.y;
                //}
                //return 0;
            }
        }


        public float LeftX
        {
            get
            {
                //ListItemArrangeType arrageType = ParentListView.ArrangeType;
                //if (arrageType == ListItemArrangeType.LeftToRight)
                //{
                //    return CachedRectTransform.anchoredPosition3D.x;
                //}
                //else if (arrageType == ListItemArrangeType.RightToLeft)
                //{
                //    return CachedRectTransform.anchoredPosition3D.x - CachedRectTransform.rect.width;
                //}
                return 0;
            }
        }

        public float RightX
        {
            get
            {
                //ListItemArrangeType arrageType = ParentListView.ArrangeType;
                //if (arrageType == ListItemArrangeType.LeftToRight)
                //{
                //    return CachedRectTransform.anchoredPosition3D.x + CachedRectTransform.rect.width;
                //}
                //else if (arrageType == ListItemArrangeType.RightToLeft)
                //{
                //    return CachedRectTransform.anchoredPosition3D.x;
                //}
                return 0;
            }
        }

        public float ItemSize
        {
            get
            {
                //if (ParentListView.IsVertList)
                //{
                //    return  CachedRectTransform.rect.height;
                //}
                //else
                //{
                //    return CachedRectTransform.rect.width;
                //}
                return GetHeight();
            }
        }

        public float ItemSizeWithPadding
        {
            get
            {
                return ItemSize;
            }
        }

    }
}

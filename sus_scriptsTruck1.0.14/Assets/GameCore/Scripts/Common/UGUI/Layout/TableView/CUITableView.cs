/*
适合于固定width*height的grid布局
*/

using Framework;
using Object= UnityEngine.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UGUI/Layout/CUITableView", 152)]
public class CUITableView : CLayoutGroup
{

    public enum Corner { UpperLeft = 0, UpperRight = 1, LowerLeft = 2, LowerRight = 3 }
    public enum Axis { Horizontal = 0, Vertical = 1 }
    public enum Constraint { Flexible = 0, FixedColumnCount = 1, FixedRowCount = 2 }


    [SerializeField]
    protected ScrollRect m_ScrollView = null;
    public ScrollRect ScrollView { get { return m_ScrollView; } set { SetProperty(ref m_ScrollView, value); } }


    [SerializeField]
    protected Corner m_StartCorner = Corner.UpperLeft;
    public Corner startCorner { get { return m_StartCorner; } set { SetProperty(ref m_StartCorner, value); } }

    [SerializeField]
    protected Axis m_StartAxis = Axis.Horizontal;
    public Axis startAxis { get { return m_StartAxis; } set { SetProperty(ref m_StartAxis, value); } }

    [SerializeField]
    protected Vector2 m_CellSize = new Vector2(100, 100);
    public Vector2 cellSize { get { return m_CellSize; } set { SetProperty(ref m_CellSize, value); } }

    [SerializeField]
    protected Vector2 m_Spacing = Vector2.zero;
    public Vector2 spacing { get { return m_Spacing; } set { SetProperty(ref m_Spacing, value); } }

    [SerializeField]
    protected Constraint m_Constraint = Constraint.Flexible;
    public Constraint constraint { get { return m_Constraint; } set { SetProperty(ref m_Constraint, value); } }

    [SerializeField]
    protected int m_ConstraintCount = 2;
    public int constraintCount { get { return m_ConstraintCount; } set { SetProperty(ref m_ConstraintCount, Mathf.Max(1, value)); } }


    [SerializeField]
    protected int m_CachaeSize = 100;
    public int CachaeSize { get { return m_CachaeSize; } set { SetProperty(ref m_CachaeSize, Mathf.Max(0, value)); } }

    protected CUITableView()
    { }

//#if UNITY_EDITOR
//    protected override void OnValidate()
//    {
//        base.OnValidate();
//        constraintCount = constraintCount;
//    }

//#endif

    protected override void Awake()
    {
        base.Awake();
        if(m_ScrollView == null)
        {
            m_ScrollView = this.GetComponentInParent<ScrollRect>();
        }
        m_ScrollView.onValueChanged.AddListener(OnScrollChanged);
    }


    #region ILayoutElement Interface
    /*
     CalculateLayoutInputHorizontal->SetLayoutHorizontal->CalculateLayoutInputVertical->SetLayoutVertical
    */
    public override void CalculateLayoutInputHorizontal()
    {
        m_Tracker.Clear();
        //CalculateHorizontalRowColumns();
    }

    /// <summary>
    /// 计算垂直回调
    /// </summary>
    public override void CalculateLayoutInputVertical()
    {
        //CalculateVerticalRowColumns();
    }

    public override void SetLayoutHorizontal()
    {
        //SetCellsAlongAxis(0);
    }

    public override void SetLayoutVertical()
    {
        //SetCellsAlongAxis(1);
    }

    //public void CalculateHorizontalRowColumns()
    //{
    //    int minColumns = 0;//最小列数
    //    int preferredColumns = 0;
    //    if (m_Constraint == Constraint.FixedColumnCount)//固定列数
    //    {
    //        minColumns = preferredColumns = m_ConstraintCount;
    //    }
    //    else if (m_Constraint == Constraint.FixedRowCount)//固定行数
    //    {
    //        minColumns = preferredColumns = Mathf.CeilToInt(this.m_itemCount / (float)m_ConstraintCount - 0.001f);
    //    }
    //    else//动态计算行列数
    //    {
    //        minColumns = 1;
    //        preferredColumns = Mathf.CeilToInt(Mathf.Sqrt(this.m_itemCount));
    //    }

    //    SetLayoutInputForAxis(
    //        padding.horizontal + (cellSize.x + spacing.x) * minColumns - spacing.x,//totalMin
    //        padding.horizontal + (cellSize.x + spacing.x) * preferredColumns - spacing.x,//totalPreferred
    //        -1, 0);
    //}

    //private void CalculateVerticalRowColumns()
    //{
    //    int minRows = 0;
    //    if (m_Constraint == Constraint.FixedColumnCount)//固定列数
    //    {
    //        minRows = Mathf.CeilToInt(this.m_itemCount / (float)m_ConstraintCount - 0.001f);
    //    }
    //    else if (m_Constraint == Constraint.FixedRowCount)//固定行数
    //    {
    //        minRows = m_ConstraintCount;
    //    }
    //    else
    //    {
    //        float width = rectTransform.rect.size.x;
    //        int cellCountX = Mathf.Max(1, Mathf.FloorToInt((width - padding.horizontal + spacing.x + 0.001f) / (cellSize.x + spacing.x)));
    //        minRows = Mathf.CeilToInt(this.m_itemCount / (float)cellCountX);
    //    }

    //    float minSpace = padding.vertical + (cellSize.y + spacing.y) * minRows - spacing.y;
    //    SetLayoutInputForAxis(minSpace, minSpace, -1, 1);
    //}

    //private void SetCellsAlongAxis(int axis)
    //{
    //    // Normally a Layout Controller should only set horizontal values when invoked for the horizontal axis
    //    // and only vertical values when invoked for the vertical axis.
    //    // However, in this case we set both the horizontal and vertical position when invoked for the vertical axis.
    //    // Since we only set the horizontal position and not the size, it shouldn't affect children's layout,
    //    // and thus shouldn't break the rule that all horizontal layout must be calculated before all vertical layout.

    //    if (axis == 0)
    //    {
    //        // Only set the sizes when invoked for horizontal axis, not the positions.
    //        for (int i = 0; i < rectChildren.Count; i++)
    //        {
    //            RectTransform rect = rectChildren[i];

    //            m_Tracker.Add(this, rect,
    //                DrivenTransformProperties.Anchors |
    //                DrivenTransformProperties.AnchoredPosition |
    //                DrivenTransformProperties.SizeDelta);

    //            rect.anchorMin = Vector2.up;
    //            rect.anchorMax = Vector2.up;
    //            rect.sizeDelta = cellSize;
    //        }
    //        return;
    //    }

    //    float width = rectTransform.rect.size.x;
    //    float height = rectTransform.rect.size.y;

    //    int cellCountX = 1;
    //    int cellCountY = 1;
    //    if (m_Constraint == Constraint.FixedColumnCount)//固定列数
    //    {
    //        cellCountX = m_ConstraintCount;
    //        cellCountY = Mathf.CeilToInt(this.m_itemCount / (float)cellCountX - 0.001f);
    //    }
    //    else if (m_Constraint == Constraint.FixedRowCount)//固定行数
    //    {
    //        cellCountY = m_ConstraintCount;
    //        cellCountX = Mathf.CeilToInt(this.m_itemCount / (float)cellCountY - 0.001f);
    //    }
    //    else
    //    {
    //        //列
    //        if (cellSize.x + spacing.x <= 0)
    //        {
    //            cellCountX = int.MaxValue;
    //        }
    //        else
    //        {
    //            cellCountX = Mathf.Max(1, Mathf.FloorToInt((width - padding.horizontal + spacing.x + 0.001f) / (cellSize.x + spacing.x)));
    //        }

    //        //行
    //        if (cellSize.y + spacing.y <= 0)
    //        {
    //            cellCountY = int.MaxValue;
    //        }
    //        else
    //        {
    //            cellCountY = Mathf.Max(1, Mathf.FloorToInt((height - padding.vertical + spacing.y + 0.001f) / (cellSize.y + spacing.y)));
    //        }
    //    }


    //    int cellsPerMainAxis;
    //    int actualCellCountX;
    //    int actualCellCountY;
    //    if (startAxis == Axis.Horizontal)
    //    {
    //        cellsPerMainAxis = cellCountX;
    //        actualCellCountX = Mathf.Clamp(cellCountX, 1, this.m_itemCount);
    //        actualCellCountY = Mathf.Clamp(cellCountY, 1, Mathf.CeilToInt(this.m_itemCount / (float)cellsPerMainAxis));
    //    }
    //    else
    //    {
    //        cellsPerMainAxis = cellCountY;
    //        actualCellCountY = Mathf.Clamp(cellCountY, 1, this.m_itemCount);
    //        actualCellCountX = Mathf.Clamp(cellCountX, 1, Mathf.CeilToInt(this.m_itemCount / (float)cellsPerMainAxis));
    //    }

    //    //cell占用的空间
    //    Vector2 requiredSpace = new Vector2(
    //            actualCellCountX * cellSize.x + (actualCellCountX - 1) * spacing.x,
    //            actualCellCountY * cellSize.y + (actualCellCountY - 1) * spacing.y
    //            );
    //    Vector2 startOffset = new Vector2(
    //            GetStartOffset(0, requiredSpace.x),
    //            GetStartOffset(1, requiredSpace.y)
    //            );


    //    //UpperLeft = 0, UpperRight = 1, LowerLeft = 2, LowerRight = 3
    //    int cornerX = (int)startCorner % 2;
    //    int cornerY = (int)startCorner / 2;
    //    for (int idx = 0; idx < rectChildren.Count; idx++)
    //    {
    //        int positionX;
    //        int positionY;
    //        if (startAxis == Axis.Horizontal)
    //        {
    //            positionX = idx % cellsPerMainAxis;
    //            positionY = idx / cellsPerMainAxis;
    //        }
    //        else
    //        {
    //            positionX = idx / cellsPerMainAxis;
    //            positionY = idx % cellsPerMainAxis;
    //        }

    //        if (cornerX == 1)//right
    //        {
    //            positionX = actualCellCountX - 1 - positionX;
    //        }
    //        if (cornerY == 1)//lower
    //        {
    //            positionY = actualCellCountY - 1 - positionY;
    //        }

    //        float x = startOffset.x + (cellSize[0] + spacing[0]) * positionX;
    //        float y = startOffset.y + (cellSize[1] + spacing[1]) * positionY;
    //        SetChildAlongAxis(rectChildren[idx], 0, x, cellSize[0]);//x
    //        SetChildAlongAxis(rectChildren[idx], 1, y, cellSize[1]);//y
    //    }
    //}


    #endregion

    //public OnGetItemData onGetItemData;
    //public OnCreateCell onCreateCell;
    public delegate void OnInitItem(ref CTableCell cell, int index,int cellNum);
    public Func<int> onCount;
    public OnInitItem onInitItem;
    Vector2 m_lastPosition = new Vector2(-9999, -9999);
    int m_lastMinIndex, m_lastMaxIndex;
    List<CTableCell> m_useItemTrans = new List<CTableCell>();
    Stack<CTableCell> m_unUseItemTrans = new Stack<CTableCell>();
    bool m_needUpdateItem = true;


    int m_itemCount = 0;
    public int itemCount
    {
        get
        {
            return m_itemCount;
        }
    }

    RectTransform m_scrollRectTrans;
    RectTransform scrollRectTrans
    {
        get
        {
            if(m_scrollRectTrans == null)
            {
                m_scrollRectTrans = m_ScrollView.GetComponent<RectTransform>();
            }
            return m_scrollRectTrans;
        }
    }

    public List<CTableCell> GetItems()
    {
        return m_useItemTrans;
    }

    public void SetItemCount(int _count)
    {
        m_itemCount = _count;
        UpdateItems();
    }

    public void TriggerComplete(Action callback)
    {
    }

    private void OnScrollChanged(Vector2 normalizedPosition)
    {
        var pos = rectTransform.anchoredPosition;
        var scrollOffset = pos - m_lastPosition;
        if(Math.Abs(scrollOffset.x) < m_CachaeSize && Math.Abs(scrollOffset.y) < m_CachaeSize)
        {
            //return;
        }
        m_lastPosition = pos;
        //Debug.Log("OnScrollChanged");
        UpdateLayout();
    }

    public void DoLayout()
    {
#if ENABLE_DEBUG
        using (var s = new ProfilerSample("DoReLayout()"))
#endif
        {
            DoCalculateLayout();
            if (startAxis == Axis.Horizontal)//水平方向
            {
                if (this.rectTransform.rect.size.x == 0)
                {
                    return;
                }
            }
            else//垂直方向
            {
                if (this.rectTransform.rect.size.y == 0)
                {
                    return;
                }
            }
            //int tick = System.Environment.TickCount;
            var scrollViewRect = scrollRectTrans.rect;
            Vector2 scrollViewSize = scrollViewRect.size;

            int minIndex, maxIndex;
            

            if(this.m_itemCount > 0)
            {
                Vector3 offset = rectTransform.anchoredPosition;
                //var scrollViewBounds = new Bounds(scrollViewRect.center, scrollViewSize);
                if (startAxis == Axis.Horizontal)//水平方向
                {
                    //contentSize.x = this.cellSize;
                    float hiddenLength = m_contentSize.y - scrollViewSize.y;//没有显示出来的总共长度
                    if (hiddenLength <= 0)
                    {
                        offset.y = 0;
                    }
                    else
                    {
                        offset.y = hiddenLength * (1 - m_ScrollView.verticalNormalizedPosition);
                        //offset.y = Mathf.Abs(offset.y);
                    }

                    int minPosY = Math.Max(0, Mathf.RoundToInt((offset.y - m_CachaeSize - m_cellStartOffset.y) / (cellSize[1] + spacing[1])));
                    int maxPosY = Math.Min(m_numOfY, Mathf.RoundToInt((offset.y + scrollViewSize.y + m_CachaeSize - m_cellStartOffset.y) / (cellSize[1] + spacing[1])));

                    minIndex = m_numOfX * minPosY;
                    maxIndex = m_numOfX * maxPosY;


                }
                else
                {
                    float hiddenLength = m_contentSize.x - scrollViewSize.x;//没有显示出来的总共长度
                    if (hiddenLength <= 0)
                    {
                        offset.x = 0;
                    }
                    else
                    {
                        offset.x = hiddenLength * m_ScrollView.horizontalNormalizedPosition;
                    }


                    int minPosX = Math.Max(0, Mathf.FloorToInt((offset.x - m_CachaeSize - m_cellStartOffset.x) / (cellSize[0] + spacing[0])));
                    int maxPosX = Math.Min(m_numOfX, Mathf.CeilToInt((offset.x + scrollViewSize.x + m_CachaeSize + m_cellStartOffset.x) / (cellSize[0] + spacing[0])));

                    minIndex = m_numOfY * minPosX;
                    maxIndex = m_numOfY * maxPosX;
                }

                minIndex = Mathf.Clamp(minIndex, 0, this.m_itemCount - 1);
                maxIndex = Mathf.Clamp(maxIndex, minIndex + 1, this.m_itemCount);
            }
            else
            {
                minIndex = maxIndex = 0;
            }

            //Debug.Log("min=" + m_lastMinIndex + " max=" + m_lastMaxIndex);

            for (int i = m_useItemTrans.Count - 1; i >= 0;  --i)
            {
                var item = m_useItemTrans[i];
                var idx = item.index;
                if (idx >= minIndex && idx < maxIndex)
                {
                    if (m_needUpdateItem)
                    {
                        if (onInitItem != null)
                        {
                            onInitItem(ref item, idx, cellsPerMainAxis);
                        }
                    }
                    continue;
                }
                m_unUseItemTrans.Push(item);
                item.Deactive();
                m_useItemTrans.RemoveAt(i);
                //item.FindChild("Text").GetComponent<Text>().text = "--";
            }



            //UpperLeft = 0, UpperRight = 1, LowerLeft = 2, LowerRight = 3
            int cornerX = (int)startCorner % 2;
            int cornerY = (int)startCorner / 2;
            for (int idx = minIndex; idx < maxIndex; ++idx)
            {
                if (idx >= m_lastMinIndex && idx < m_lastMaxIndex)
                {
                    continue;
                }
                int positionX;
                int positionY;
                if (startAxis == Axis.Horizontal)
                {
                    positionX = idx % cellsPerMainAxis;
                    positionY = idx / cellsPerMainAxis;
                }
                else
                {
                    positionX = idx / cellsPerMainAxis;
                    positionY = idx % cellsPerMainAxis;
                }

                if (cornerX == 1)//right
                {
                    positionX = m_numOfX - 1 - positionX;
                }
                if (cornerY == 1)//lower
                {
                    positionY = m_numOfY - 1 - positionY;
                }

                float x = m_cellStartOffset.x + (cellSize[0] + spacing[0]) * positionX;
                float y = m_cellStartOffset.y + (cellSize[1] + spacing[1]) * positionY;
                var cell = GetCellItem(idx, cellsPerMainAxis);
                if(onInitItem != null)
                {
                    onInitItem(ref cell, idx, cellsPerMainAxis);
                }
                if(cell == null)
                {
                    LOG.Error("没有创建item接口");
                    continue;
                }
                cell.Active();
                cell.index = idx;
                var itemTrans = cell.trans;
                m_useItemTrans.Add(cell);
                SetChildAlongAxis(itemTrans, 0, x, cellSize[0]);//x
                SetChildAlongAxis(itemTrans, 1, y, cellSize[1]);//y
            }

#if UNITY_EDITOR
            for(int idx = 0; idx < m_useItemTrans.Count;++idx)
            {
                var cell = m_useItemTrans[idx];
                cell.trans.SetSiblingIndex(cell.index);
                cell.trans.name = "item_" + cell.index;
            }
#endif

            m_lastMinIndex = minIndex;
            m_lastMaxIndex = maxIndex;
            m_needUpdateItem = false;

            //             tick = System.Environment.TickCount - tick;
            //             if(tick > 0)
            //             {
            //                 //LOG.Info(string.Concat("do layout use:", tick, "ms"));
            //             }
            m_isDirty = false;
        }//end  profile
    }

    CTableCell GetCellItem(int idx, int cellNum)
    {
        CTableCell cell = null;
        if(m_unUseItemTrans.Count > 0)
        {
            cell = m_unUseItemTrans.Pop();
        }
        return cell;
    }

    Vector2 m_contentSize;
    Vector2 m_cellStartOffset;
    int m_numOfX;
    int m_numOfY;
    int cellsPerMainAxis;
    void DoCalculateLayout()
    {
        var scrollViewRect = scrollRectTrans.rect;
        Vector2 scrollViewSize = scrollViewRect.size;
        m_contentSize = scrollViewSize;
        if(cellSize.x <= 0)
        {
            m_CellSize.x = scrollViewSize.x;
        }
        if (m_CellSize.y <= 0)
        {
            m_CellSize.y = scrollViewSize.y;
        }

        int cellCountX = 1;
        int cellCountY = 1;
        if (m_Constraint == Constraint.FixedColumnCount)//固定列数
        {
            cellCountX = m_ConstraintCount;//行数
            cellCountY = Mathf.CeilToInt(this.m_itemCount / (float)cellCountX - 0.001f);//列数
        }
        else if (m_Constraint == Constraint.FixedRowCount)//固定行数
        {
            cellCountY = m_ConstraintCount;//列数
            cellCountX = Mathf.CeilToInt(this.m_itemCount / (float)cellCountY - 0.001f);//行数
        }
        else
        {
            cellCountX = Mathf.Max(1, Mathf.FloorToInt((m_contentSize.x - padding.horizontal + spacing.x + 0.001f) / (cellSize.x + spacing.x))); 
            cellCountY = Mathf.Max(1, Mathf.FloorToInt((m_contentSize.y - padding.vertical + spacing.y + 0.001f) / (cellSize.y + spacing.y)));
        }


        int actualCellCountX;
        int actualCellCountY;
        if (startAxis == Axis.Horizontal)//水平方向
        {
            cellsPerMainAxis = cellCountX;
            actualCellCountX = Mathf.Clamp(cellCountX, 1, this.m_itemCount);
            actualCellCountY = Mathf.CeilToInt(this.m_itemCount / (float)cellsPerMainAxis);
        }
        else//垂直方向
        {
            cellsPerMainAxis = cellCountY;
            actualCellCountY = Mathf.Clamp(cellCountY, 1, this.m_itemCount);
            actualCellCountX = Mathf.CeilToInt(this.m_itemCount / (float)cellsPerMainAxis);
        }

        //cell占用的空间
        Vector2 requiredSpace = new Vector2(
                actualCellCountX * cellSize.x + (actualCellCountX - 1) * spacing.x,
                actualCellCountY * cellSize.y + (actualCellCountY - 1) * spacing.y
                );
        m_numOfX = actualCellCountX;
        m_numOfY = actualCellCountY;
        if (startAxis == Axis.Horizontal)//水平方向
        {
            m_contentSize.y = Mathf.Max(scrollViewSize.y, requiredSpace.y + this.padding.vertical);
            this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_contentSize.x);//默认
            this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_contentSize.y);
        }
        else//垂直方向
        {
            m_contentSize.x = Mathf.Max(scrollViewSize.x, requiredSpace.x + this.padding.horizontal);
            this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_contentSize.x);
            this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_contentSize.y);//默认
        }
        //LOG.Error("size = "+this.rectTransform.rect.size);

        SetLayoutInputForAxis(
            m_contentSize.x,//min
            m_contentSize.x,//preferred
            1, //flexible
            0);
        SetLayoutInputForAxis(
            m_contentSize.y,//min
            m_contentSize.y,//preferred
            1, //flexible
            1);
        m_cellStartOffset = new Vector2(
                GetStartOffset(0, requiredSpace.x),
                GetStartOffset(1, requiredSpace.y)
                );
        m_isDirty = true;
    }

    bool m_isDirty = false;
    bool m_isDoLayouting = false;
    void Update()
    {
        if(!m_isDirty)
        {
            return;
        }
        if(m_isDoLayouting)
        {
            return;
        }
        m_isDoLayouting = true;
        DoLayout();
        m_isDoLayouting = false;
    }

    protected override void SetProperty<T>(ref T currentValue, T newValue)
    {
        base.SetProperty<T>(ref currentValue, newValue);
        UpdateLayout();
    }

    public void UpdateLayout()
    {
        m_isDirty = true;
        this.DoCalculateLayout();
        /*
        CalculateHorizontalRowColumns();
        SetCellsAlongAxis(0);
        CalculateVerticalRowColumns();
        SetCellsAlongAxis(1);
        */
    }

    public void UpdateItems()
    {
        m_needUpdateItem = true;
        UpdateLayout();
    }
}
public class CTableCell
{
    public int index;
    public RectTransform trans;

    public CTableCell(int index, RectTransform trans)
    {
        this.index = index;
        this.trans = trans;
    }

    internal void Active()
    {
        //trans.gameObject.SetActiveEx(true);
        //trans.anchoredPosition = Vector2.zero;
    }

    internal void Deactive()
    {
        trans.anchoredPosition = Vector2.one * 10000;
#if UNITY_EDITOR
        trans.name = "--";
#endif
    }
}

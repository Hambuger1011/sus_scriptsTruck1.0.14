using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollViewPageCell:MonoBehaviour
{
    public bool hasCell = false;
    public MonoBehaviour item = null;

    public void MountItem(Transform parent, MonoBehaviour item)
    {
        hasCell = true;
        this.item = item;

        item.transform.SetParent(parent);
        item.transform.localPosition = Vector3.zero;
        item.transform.localScale = Vector3.one;
        item.gameObject.SetActive(true);
    }
    public void Reset()
    {
        hasCell = false;
        if (item)
        {
            item.gameObject.SetActive(false);
        }
        item = null;
    }
}
public class ScrollViewPage : ScrollRect
{
    [System.NonSerialized]
    public bool hasPrefabChild = false;

    public bool canAbsorb = true;
    private float closestHorizontalNormalPosition = 0;
    private int m_index;
    public event Action<int> onPageIndexChange;


    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="initDataIndex"></param>
    public void Init(Action<int,int> onRefreshItemDataHandler, int initDataIndex=0)
    {
        OnRefreshItemDataHandler = onRefreshItemDataHandler;
        ToRefreshItemData(initDataIndex, GetCount());
    }



    private int Index
    {
        get
        {
            return m_index;
        }
        set
        {
            var newValue = Mathf.Clamp(value, 0, GetCount() - 1);
            bool isDirty = (m_index != newValue);
            m_index = newValue;
            if(isDirty && onPageIndexChange != null)
            {
                onPageIndexChange(m_index);
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        this.canAbsorb = true;
    }

    //public override void OnInitializePotentialDrag(PointerEventData eventData)
    //{
    //    base.OnInitializePotentialDrag(eventData);
    //    canAbsorb = false;
    //}

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        canAbsorb = false;
        m_scrollDelta = eventData.delta;
        lastScrollTime = Time.time;
    }

    Vector2 m_scrollDelta;
    float lastScrollTime = 0;
    public override void OnEndDrag(PointerEventData eventData)
    {
        if(canAbsorb)
        {
            //Debug.Log(eventData.delta);
            return;
        }
        base.OnEndDrag(eventData);
        velocity = Vector2.zero;
        //Debug.Log(eventData.dragging+" "+ eventData.useDragThreshold +" "+ eventData.IsPointerMoving());

        var scrollDelta = eventData.delta;
        if(scrollDelta.magnitude < 0.1f && Time.time - lastScrollTime < 0.25f)
        {
            scrollDelta = m_scrollDelta;
        }

        if (scrollDelta.magnitude > 0.1f)
        {
            float angle = Vector2.Angle(scrollDelta, Vector2.up);
            if (angle > 45f && angle < 135f)
            {
                var normal = Vector3.Cross(scrollDelta, Vector2.up);
                //Debug.Log(scrollDelta.magnitude + " " + normal.z);
                
                if (normal.z > 0)//右滑
                {
                    Index -= 1;
                }
                else
                {
                    Index += 1;
                }
                float perNormalPositionLength = CalcPerNormalPositionLength();
                closestHorizontalNormalPosition = Mathf.Clamp01(Index * perNormalPositionLength);
            }
            else
            {
                //Debug.LogError(scrollDelta.magnitude + " " + Time.deltaTime);
                moveGridChildToMiddle();
            }
        }
        else
        {
            //Debug.LogError(scrollDelta.magnitude +" "+ Time.deltaTime);
            moveGridChildToMiddle();
        }
        canAbsorb = true;
    }

    public int GetCount()
    {
        //if(hasPrefabChild)
        //{
        //    return content.childCount - 1;
        //}
        //return content.childCount;

        int count = 0;
        for (int i = 0; i < content.childCount; i++)
        {
            if (content.GetChild(i).gameObject.activeSelf)
            {
                count++;
            }
        }
        //Debug.LogError($"lzh ===> count = {count}");
        return count;

    }

    private void moveGridChildToMiddle()
    {
        int contentChildCount = GetCount();
        if (contentChildCount == 0) return;

        closestHorizontalNormalPosition = 1;
        float perNormalPositionLength = CalcPerNormalPositionLength();
        for (int i = 0; i < contentChildCount; i++)
        {
            float nowPos = i * perNormalPositionLength;
            var x = Mathf.Abs(horizontalNormalizedPosition - nowPos);
            var y = Mathf.Abs(horizontalNormalizedPosition - closestHorizontalNormalPosition);
            if (x < y)
            {
                Index = i;
                closestHorizontalNormalPosition = nowPos;
            }
        }
    }

    float CalcPerNormalPositionLength()
    {
        int contentChildCount = GetCount();
        if (contentChildCount == 0) return 0;
        float perNormalPositionLength = 0;
        if (contentChildCount > 1)
        {
            perNormalPositionLength = 1f / (contentChildCount - 1);
        }
        else
        {
            perNormalPositionLength = 1;
        }
        int index = Mathf.RoundToInt(horizontalNormalizedPosition / perNormalPositionLength);
        index = Mathf.Clamp(index, 0, contentChildCount);
        //Debug.LogError($"lzh ===> contentChildCount={contentChildCount} {index} perNormalPositionLength = {perNormalPositionLength} {horizontalNormalizedPosition / perNormalPositionLength}");
        return perNormalPositionLength;
    }


    float lastHorizontalNormalizedPosition = -1;
    protected override void LateUpdate()
    {
        base.LateUpdate();
        int childCount = GetCount();
        if (childCount > 1 && canAbsorb && Mathf.Abs(horizontalNormalizedPosition - closestHorizontalNormalPosition) > 0.01f)
        {
            horizontalNormalizedPosition = Mathf.Lerp(horizontalNormalizedPosition, closestHorizontalNormalPosition, 0.35f);
            if (Mathf.Abs(horizontalNormalizedPosition - closestHorizontalNormalPosition) <= 0.01f)
            {
                horizontalNormalizedPosition = closestHorizontalNormalPosition;
            }
        }

        // 计算刷新item数据的临界点
        if (childCount > 3 && lastHorizontalNormalizedPosition != horizontalNormalizedPosition)
        {
            int curMainPageIndex = ClacCurrentMainPage(childCount);
            if (curMainPageIndex != lastMainPageIndex)
            {
                ToRefreshItemData(curMainPageIndex,childCount);
            }
            lastHorizontalNormalizedPosition = horizontalNormalizedPosition;
        }

    }

    int lastMainPageIndex =-1;
    int ClacCurrentMainPage(int childCount)
    {
        if (childCount == 0) return 0;
        float perNormalPositionLength = 0;
        if (childCount > 1)
        {
            perNormalPositionLength = 1f / (childCount - 1);
        }
        else
        {
            perNormalPositionLength = 1;
        }
        int index = Mathf.RoundToInt(horizontalNormalizedPosition / perNormalPositionLength);
        index = Mathf.Clamp(index, 0, childCount);
        //Debug.LogError($"lzh===> index={index}");
        return index;
    }

    Action<int,int> OnRefreshItemDataHandler = null;

    void ToRefreshItemData(int curMainPageIndex, int itemCount)
    {
        if (curMainPageIndex == lastMainPageIndex)
        {
            return;
        }

        if (OnRefreshItemDataHandler != null)
        {
            OnRefreshItemDataHandler.Invoke(curMainPageIndex, lastMainPageIndex);
        }
        lastMainPageIndex = curMainPageIndex;

    }

    public void MoveToNext()
    {
        if(Index + 1 < GetCount())
        {
            Index += 1;
        }else
        {
            Index = 0;
        }
        //m_index = Mathf.Clamp(m_index, 0, GetCount() - 1);
        float perNormalPositionLength = CalcPerNormalPositionLength();
        closestHorizontalNormalPosition = Mathf.Clamp01(Index * perNormalPositionLength);
    }

    public void MoveToL()
    {
        Index -= 1;
        if (Index <0)
        {
            Index = GetCount()-1;
            //Debug.Log("GetCount():" + GetCount());
        }
        //Debug.Log("sh:" + Index);
        //m_index = Mathf.Clamp(m_index, 0, GetCount() - 1);
        float perNormalPositionLength = CalcPerNormalPositionLength();
        closestHorizontalNormalPosition = Mathf.Clamp01(Index * perNormalPositionLength);
    }

    public void MoveToIndex(int vIndex)
    {
        if (vIndex >= GetCount())
        {
            vIndex = GetCount() - 1;
        }

        Index = vIndex;

        float perNormalPositionLength = CalcPerNormalPositionLength();
        closestHorizontalNormalPosition = Mathf.Clamp01(vIndex * perNormalPositionLength);
    }

    //没有补间动画的移动
    public void MoveToIndexNoTween(int vIndex)
    {
        if (vIndex >= GetCount())
        {
            vIndex = GetCount() - 1;
        }
        Index = vIndex;
        float perNormalPositionLength = CalcPerNormalPositionLength();
        closestHorizontalNormalPosition =  Mathf.Clamp01(vIndex * perNormalPositionLength);
    }
}

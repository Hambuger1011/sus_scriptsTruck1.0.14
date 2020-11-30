using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDoubleScrollRect : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ScrollRect hScroll;
    public ScrollRect vScroll;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(IsValid(hScroll.gameObject))
        {
            hScroll.OnBeginDrag(eventData);
        }

        if (IsValid(vScroll.gameObject))
        {
            vScroll.OnBeginDrag(eventData);
        }
    }

    bool IsValid(GameObject go)
    {
        return true;
        //return this.gameObject != go;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float angle = Vector2.Angle(eventData.delta, Vector2.up);
        //判断拖动方向，防止水平与垂直方向同时响应导致的拖动时整个界面都会动
        if (angle > 45f && angle < 135f)
        {
            hScroll.enabled = true;
            vScroll.enabled = false;
        }
        else
        {
            hScroll.enabled = false;
            vScroll.enabled = true;
        }

        if (IsValid(hScroll.gameObject))
        {
            hScroll.OnDrag(eventData);
        }

        if (IsValid(vScroll.gameObject))
        {
            vScroll.OnDrag(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsValid(hScroll.gameObject))
        {
            hScroll.OnEndDrag(eventData);
        }

        if (IsValid(vScroll.gameObject))
        {
            vScroll.OnEndDrag(eventData);
        }


        hScroll.enabled = true;
        vScroll.enabled = true;
    }
}

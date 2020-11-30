using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDrag : MonoBehaviour, 
    IPointerDownHandler,
    IPointerUpHandler,
    IBeginDragHandler,
    IEndDragHandler,
    IDragHandler
{
    public Action<PointerEventData> onDown;
    public Action<PointerEventData> onUp;
    public Action<PointerEventData> onDrag;
    public Action<PointerEventData> onBeginDrag;
    public Action<PointerEventData> onEndDrag;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (onDown != null) onDown(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (onUp != null) onUp(eventData);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null) onDrag(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (onBeginDrag != null) onBeginDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (onEndDrag != null) onEndDrag(eventData);
    }

    public void Clear()
    {
        onDown = null;
        onUp = null;
        onDrag = null;
        onBeginDrag = null;
        onEndDrag = null;
    }
}

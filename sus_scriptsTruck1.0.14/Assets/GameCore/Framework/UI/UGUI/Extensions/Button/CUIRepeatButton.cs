using Framework;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CUIRepeatButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler
{
    bool b_isDown = false;
    public Action onButtonDown;
    public Action onButtonUp;
    public Action onButtonExit;
    public Action onButtonClick;

    public Action onButtonBegin;
    public Action onButtonEnd;
    public Action<bool> onButtonUpdate;

    public RectTransform trans
    {
        get
        {
            return this.transform as RectTransform;
        }
    }

    /// <summary>
    /// 按下事件
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        b_isDown = true;
        if (onButtonDown != null)
        {
            onButtonDown();
        }
        if (onButtonBegin != null)
        {
            onButtonBegin();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        b_isDown = false;
        if (onButtonExit != null)
        {
            onButtonExit();
        }
        if (onButtonEnd != null)
        {
            onButtonEnd();
        }
    }

    /// <summary>
    /// 移出事件
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        b_isDown = false;
        if (onButtonUp != null)
        {
            onButtonUp();
        }

        if (onButtonEnd != null)
        {
            onButtonEnd();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(onButtonClick != null)
        {
            onButtonClick();
        }
    }

    void Update()
    {
        if(onButtonUpdate != null)
        {
            onButtonUpdate(b_isDown);
        }
    }
}

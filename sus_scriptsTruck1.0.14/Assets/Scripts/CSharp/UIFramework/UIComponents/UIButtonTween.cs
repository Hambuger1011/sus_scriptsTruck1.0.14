using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class UIButtonTween : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform target;
    public Vector2 tweenScale = new Vector2(0.9f,0.9f);

    private Vector3 _originalScale;
    
    public RectTransform tweenTrans
    {
        get
        {
            if(target != null)
            {
                return target;
            }
            return this.rectTransform();
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        _originalScale = tweenTrans.localScale;
        Vector3 s = tweenScale;
        s.z = 1;
        tweenTrans.localScale = s;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
           // INSTANCE.Instance.WriteBookeNameToDic();
        
        tweenTrans.localScale = _originalScale;
    }
}

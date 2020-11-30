using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewMainScrollViewBar : BaseUIForm, IEndDragHandler
{
    public Scrollbar bar;
    public CommentForm CommentForm;
    public void OnEndDrag(PointerEventData eventData)
    {    
        if (bar.value <= 0)
        {          
            CommentForm.scrollbarDown();
        }
    }
}

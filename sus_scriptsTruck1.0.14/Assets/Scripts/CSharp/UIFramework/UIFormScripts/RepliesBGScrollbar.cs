using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RepliesBGScrollbar : BaseUIForm, IEndDragHandler
{

    public Scrollbar bar;
    public RepliesBGsprite RepliesBGsprite;
    public void OnEndDrag(PointerEventData eventData)
    {
        if (bar.value <= 0)
        {
            RepliesBGsprite.ChapterCommenBackscrollbarDown();
        }
    }
}

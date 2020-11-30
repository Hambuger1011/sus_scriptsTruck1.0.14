using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EmailNoticeScrollView : MonoBehaviour, IEndDragHandler
{
    public Scrollbar bar;
    public EmailForm emailForm;
    public int type;

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("barValue:" + bar.value);
        if (type == 1)
        {
            //这个是检测邮箱
            if (bar.value <= 0)//到达底部
            {
                emailForm.EmailPagUpdate();
            }
        }
        else
        {
            //这个是检测新闻
            if (bar.value <= 0)//到达底部
            {
                emailForm.NewPagUpdate();
            }
        }
    }
}

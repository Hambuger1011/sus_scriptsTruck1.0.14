using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollbarVerticalContronller : BaseUIForm, IEndDragHandler
{
    public Scrollbar bar;
    public EmailForm emailForm;
    public EmailNoticeSprite EmailNoticeSprite;
    public ReplyForDetallsForm ReplyForDetallsForm;
    public int type;
   

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("barValue:" + bar.value);
        if (type==1)
        {
            //这个是检测邮箱
            //if (bar.value <= 0)//到达底部
            //{
            //    emailForm.EmailPagUpdate();
            //}
        }else if (type == 2)
        {
            //这个是新闻评论检测
            if (bar.value <= 0)//到达底部
            {
                EmailNoticeSprite.ShowNextPage();
            }
        }else if (type == 3)
        {
            //这个是新闻评论的评论检测
            if (bar.value <= 0)//到达底部
            {
                ReplyForDetallsForm.NexPag();
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

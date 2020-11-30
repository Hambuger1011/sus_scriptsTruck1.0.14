using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UGUI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using AB;

public class ShareUIForm : BaseUIForm
{
    public RectTransform BG;
    public GameObject mask;
    public Image topicon;
    public override void OnOpen()
    {
        base.OnOpen();
        BG.anchoredPosition = new Vector2(0, 1334);

        BGmove(0,1);
        UIEventListener.AddOnClickListener(mask, MaskOnClicke);
    }

    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(mask, MaskOnClicke);
    }

    public void BGmove(float Yp, int type)
    {
        BG.DOAnchorPosY(Yp, 1f).SetEase(Ease.InOutBack).OnComplete(() =>
        {
            if (type == 1)
            {
                //这个是打开界面执行的事件
            }
            else
            {
                //这个是关闭界面执行的事件
                CUIManager.Instance.CloseForm(UIFormName.ShareForm);            
            }
        });
    }

    private void MaskOnClicke(PointerEventData data)
    {
        BGmove(1334, 2);
    }

    public void ImageIconChange(int bookid)
    {
        
        topicon.sprite = ABSystem.ui.GetUITexture(AbTag.Global, "Assets/Bundle/BookPreview/Icon/share_" + bookid + ".png");
    }
}

using pb;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Notice : BaseUIForm
{
    public ScrollViewPage pageView;
    public ScrollViewPageBar pageBar;
    public GameObject itemTpl,LButton,RButton,CloseButtonGa,TestScrollView,Mask;
    public Text TestCont;

    private t_BulletinBoard data;

    public override void OnOpen()
    {
        base.OnOpen();

        pageView.hasPrefabChild = true;
        itemTpl.SetActiveEx(false);
       
        UIEventListener.AddOnClickListener(LButton, MoveToL);
        UIEventListener.AddOnClickListener(RButton, MoveToR);
        UIEventListener.AddOnClickListener(CloseButtonGa, CloseButton);
        UIEventListener.AddOnClickListener(Mask, MaskClose);

        data = GameDataMgr.Instance.table.GetBulletinBoardById(1);        
        TestCont.text = data.text.ToString();
        TestCont.gameObject.SetActive(true);

       
        BookRecommendReturn();

        Transform bgTrans = this.gameObject.transform.Find("Canvas/BG");
        if (GameUtility.IpadAspectRatio() && bgTrans != null)
            bgTrans.localScale = Vector3.one * 0.7f;
      
    }

   

    public override void OnClose()
    {
        base.OnClose();

        UIEventListener.RemoveOnClickListener(LButton, MoveToL);
        UIEventListener.RemoveOnClickListener(RButton, MoveToR);
        UIEventListener.RemoveOnClickListener(CloseButtonGa, CloseButton);
        UIEventListener.RemoveOnClickListener(Mask, MaskClose);
    }


    private void MaskClose(PointerEventData data)
    {
        CloseButton(null);
    }
    private void CloseButton(PointerEventData data)
    {
        CUIManager.Instance.CloseForm(UIFormName.Notice);

        //公告关闭后弹出其他的礼包

    }
    public void MoveToR(PointerEventData data)
    {
        pageView.MoveToNext();
    }

    public void MoveToL(PointerEventData data)
    {
        pageView.MoveToL();
    }
   
    public void BookRecommendReturn()
    {
        int count = data.num;
        for (int i = 0; i < count; i++)
        {
            var go = GameObject.Instantiate(itemTpl, pageView.content);
            go.SetActiveEx(true);
            var t = go.transform;
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;
            t.localRotation = Quaternion.identity;

            go.GetComponent<NoticeTempletSprite>().Inite(data,i+1);
        }

        if (count>1)
        {
            pageBar.SetTipsCount(count);
        }
    }
}

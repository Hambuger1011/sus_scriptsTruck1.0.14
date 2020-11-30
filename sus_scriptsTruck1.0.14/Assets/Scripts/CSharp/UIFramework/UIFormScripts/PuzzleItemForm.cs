using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using AB;

/// <summary>
/// 拼图的子块
/// </summary>
public class PuzzleItemForm : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Image mItemImg;
    private RectTransform rt;
    //记录鼠标位置.
    private Vector3 newPosition;
    private Vector2 mOriPos;

    private Vector2 range = new Vector2(50, 50);
    private bool isPlay = false;
    private Action mFinishCallBack;

    public void Init(Vector2 vOriPos,int vType,int vIndex,Action vFiniCallBack)
    {
        mItemImg = this.gameObject.GetComponent<Image>();
        rt = this.GetComponent<RectTransform>();
        mOriPos = vOriPos;
        mFinishCallBack = vFiniCallBack;
        isPlay = true;
        mItemImg.sprite = ABSystem.ui.GetUITexture(AbTag.Global,string.Concat("Assets/Bundle/Puzzle/" + vType + "_", vIndex, ".png"));
        mItemImg.SetNativeSize();
    }

	public void OnBeginDrag(PointerEventData eventData)
    {
       
    }

    /// <summary>
    /// Raises the drag event.
    /// </summary>
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        //推拽是图片跟随鼠标移动.
        if (isPlay)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, Input.mousePosition, eventData.enterEventCamera, out newPosition);
            transform.position = newPosition;
        }
    }

    /// <summary>
    /// Raises the end drag event.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        if(isPlay)
            CheckIsFinish();
    }

    private void CheckIsFinish()
    {
       // Debug.Log("--localX-" + this.transform.localPosition.x + "--localY-->" + this.transform.localPosition.y + "--oriX-->" + mOriPos.x + "--oriY-->" + mOriPos.y);
        if (this.transform.localPosition.x > mOriPos.x - range.x && this.transform.localPosition.x < mOriPos.x + range.x
            && this.transform.localPosition.y > mOriPos.y - range.y && this.transform.localPosition.y < mOriPos.y + range.y)
        {
            isPlay = false;
            this.transform.localPosition = mOriPos;
            if (mFinishCallBack != null)
                mFinishCallBack();
        }
    }
}

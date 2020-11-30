using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class EmailItemInputController : BaseUIForm, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private float fingerActionSensitivity = Screen.width * 0.05f; //手指动作的敏感度，这里设定为 二十分之一的屏幕宽度.

    private float fingerBeginX;
    private float fingerBeginY;
    private float fingerCurrentX;
    private float fingerCurrentY;
    private float fingerSegmentX;
    private float fingerSegmentY;

    private int fingerTouchState;

    //三种状态
    private int state_null = 0;
    private int state_touch = 1;
    private int state_add = 2;
    private float fingerDistance;
    private bool open = false;

    public GameObject ShowIconGame,destroygame, changImage;

    private void Start()
    {
        fingerBeginX = 0;
        fingerBeginY = 0;
        fingerCurrentX = 0;
        fingerCurrentY = 0;
        fingerSegmentX = 0;
        fingerSegmentY = 0;

        fingerTouchState = state_null;

        
    }


    public override void OnOpen()
    {
        base.OnOpen();

        UIEventListener.AddOnClickListener(changImage, changButton);
    }

    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(changImage, changButton);
    }
   
    public void OnPointerDown(PointerEventData eventData)
    {
        
        if (fingerTouchState == state_null)
        {
            fingerTouchState = state_touch;
            fingerBeginX = Input.mousePosition.x;
            fingerBeginY = Input.mousePosition.y;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
       
        if (fingerTouchState != state_touch) return;

        fingerCurrentX = Input.mousePosition.x;
        fingerCurrentY = Input.mousePosition.y;
        fingerSegmentX = fingerCurrentX - fingerBeginX; //计算左右拖动的长度
        fingerSegmentY = fingerCurrentY - fingerBeginY; //计算上下拖动的长度

        //这边计算你需要拖动的范围才算拖动了
        fingerDistance = fingerSegmentX * fingerSegmentX + fingerSegmentY * fingerSegmentY;

        if (fingerDistance > (fingerActionSensitivity * fingerActionSensitivity))
        {
            ToAddFingerAction();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
        fingerTouchState = state_null;
       
    }

    void ToAddFingerAction()
    {
        fingerTouchState = state_add;

        //判断是左右还是上下，通过设置对立的参数为0进行判断，
        if (Mathf.Abs(fingerSegmentX) > Mathf.Abs(fingerSegmentY))
        {
            fingerSegmentY = 0;
        }
        else
        {
            fingerSegmentX = 0;
        }

        // fingerSegmentX=0 则是上下拖动
        if (fingerSegmentX == 0)
        {
            if (fingerSegmentY > 0)
            {
                //LOG.Info("up");
            }
            else
            {
                //LOG.Info("down");
            }
        }
        else if (fingerSegmentY == 0)
        {
            if (fingerSegmentX > 0)
            {
                //LOG.Info("right");
                ShowIconGame.transform.DOLocalMoveX(0, 0.2f).OnComplete(()=>{
                    destroygame.SetActive(false);
                });
                
            }
            else
            {
                //LOG.Info("left");
                ShowIconGame.transform.DOLocalMoveX(-153, 0.2f);
                destroygame.SetActive(true);
            }
        }

        fingerDistance = 0;
    }

    private void changButton(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        open = !open;

        if (open)
        {
            ShowIconGame.transform.DOLocalMoveX(-153, 0.2f);
            destroygame.SetActive(true);
        }else
        {
            ShowIconGame.transform.DOLocalMoveX(0, 0.2f).OnComplete(() => {
                destroygame.SetActive(false);
            });
        }
    }
}

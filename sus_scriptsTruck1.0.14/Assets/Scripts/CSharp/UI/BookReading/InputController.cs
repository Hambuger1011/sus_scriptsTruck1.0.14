
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputController : BaseUIForm
#if NOT_USE_LUA
                                , IPointerDownHandler, IDragHandler, IPointerUpHandler
#endif
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
    private int nowType = 0;

    public BookReadingForm bookReadingForm;
    public GameObject[] directiongan;
#if NOT_USE_LUA
    void Start()
    {
        fingerBeginX = 0;
        fingerBeginY = 0;
        fingerCurrentX = 0;
        fingerCurrentY = 0;
        fingerSegmentX = 0;
        fingerSegmentY = 0;

        fingerTouchState = state_null;
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
        //if (fingerDistance > (fingerActionSensitivity * fingerActionSensitivity))
        //{
        //    ToAddFingerAction();
        //}
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
                //Debug.Log("up");
                bookReadingForm.ManualChangeSceneMove(2);
                if (nowType==2)
                {
                    //textTile.text = "";
                    directiongan[1].SetActive(false);
                }
            }
            else
            {
                //Debug.Log("down");
                bookReadingForm.ManualChangeSceneMove(4);
                if (nowType == 4)
                {
                    //textTile.text = "";
                    directiongan[3].SetActive(false);
                }
            }
        }
        else if (fingerSegmentY == 0)
        {
            if (fingerSegmentX > 0)
            {
                //Debug.Log("right");
                bookReadingForm.ManualChangeSceneMove(3);
                if (nowType == 3)
                {
                    //textTile.text = "";
                    directiongan[2].SetActive(false);
                }
            }
            else
            {
                //Debug.Log("left");
                bookReadingForm.ManualChangeSceneMove(1);
                if (nowType == 1)
                {
                    //textTile.text = "";
                    directiongan[0].SetActive(false);
                }
            }
        }

        fingerDistance = 0;
    }

    /// <summary>
    /// 显示需要滑动的方向 1 左滑 ；2 上滑；3右滑；4下滑；
    /// </summary>
    /// <param name="tpty"></param>
    public void showdirection(int tpty)
    {
        nowType = tpty;
        if (tpty==1)
        {
            //textTile.text = "Please swipe left";
            directionGame(0);
        }
        else if (tpty==2)
        {
            //textTile.text = "Please slide up";
            directionGame(1);
        }
        else if (tpty==3)
        {
            //textTile.text = "Please slide to the right";
            directionGame(2);
        }
        else if (tpty==4)
        {
            // textTile.text = "Please slide down";
            directionGame(3);
        }
    }
    /// <summary>
    /// 1,Left 2.up 3.right 4.down
    /// </summary>
    /// <param name="NUMBER"></param>
    private void directionGame(int NUMBER)
    {
        for (int i=0;i< directiongan.Length;i++)
        {
            if (i==NUMBER)
            {
                directiongan[i].SetActive(true);
            }else
            {
                directiongan[i].SetActive(false);
            }
        }
    }
#endif
}
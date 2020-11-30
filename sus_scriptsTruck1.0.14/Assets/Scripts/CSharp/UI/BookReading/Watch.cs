using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;


[XLua.LuaCallCSharp]
public class Watch : BaseUIForm, IDragHandler
{

    public float speed;
    public Transform pointerM;
    public Transform pointerH;

#if !NOT_USE_LUA
    Action<float> onClockChangeSceneAlpha;
#else
    public BookReadingForm bookReadingForm;
#endif
    public RectTransform ClockGame;

    private float angleM, timesTurns = 0, hadrunAngleM;// timesTurns 旋转了几圈
    private Vector3 oldVector_M;
    private bool canDrugEvent = false, canNoRetrogress = false;
    private int NumberOfTurns = 0;

    public void DrugEvent()
    {
        oldVector_M = pointerM.localEulerAngles;

        //通过鼠标位置决定角度（因为Vector3.Angle不会大于180）
        if (Input.mousePosition.x >= 720/*Screen.width*/ / 2)
        {
            angleM = 360 - Vector3.Angle(new Vector3(0, 1, 0), new Vector3(Input.mousePosition.x - /*Screen.width*/720 / 2, Input.mousePosition.y - /*Screen.height*/475 / 2, 0));

        }
        else
        {
            angleM = Vector3.Angle(new Vector3(0, 1, 0), new Vector3(Input.mousePosition.x - /*Screen.width*/720 / 2, Input.mousePosition.y - /*Screen.height*/475 / 2, 0));

        }


        if (hadrunAngleM > 0)
        {
            pointerM.localEulerAngles = new Vector3(0, 0, angleM);
            //LOG.Info("角度：" + pointerM.eulerAngles.z+ "--angleM:"+ angleM);
            if (Mathf.Abs(pointerM.localEulerAngles.z - oldVector_M.z) < 180)//判断是否经过12
            {
                pointerH.localEulerAngles += new Vector3(0, 0, (pointerM.localEulerAngles.z - oldVector_M.z) / 12);
            }
            else
            {
                if (Input.mousePosition.x > /*Screen.width*/720 / 2)//顺时针经过
                {
                    pointerH.localEulerAngles += new Vector3(0, 0, (pointerM.localEulerAngles.z - oldVector_M.z - 360) / 12);

                    //LOG.Info("顺时针");
                    //bookReadingForm.ClockChangeSceneAlpha(1.0f/(NumberOfTurns));

                    if (hadrunAngleM >= 180)
                    {
                        timesTurns++;
                    }


                }
                else//逆时针经过
                {
                    pointerH.localEulerAngles += new Vector3(0, 0, (pointerM.localEulerAngles.z - oldVector_M.z + 360) / 12);

                    //LOG.Info("逆时针");
                    //bookReadingForm.ClockChangeSceneAlpha(-1.0f / (NumberOfTurns));
                    if (timesTurns >= 0)
                    {
                        timesTurns--;
                        if (timesTurns <= -1)
                        {
                            timesTurns = -1;
                        }
                    }
                }
            }
        }
        else
        {


            if (Input.mousePosition.x > /*Screen.width*/720 / 2)//顺时针经过
            {
                if (timesTurns <= -1 && canNoRetrogress)
                {
                    timesTurns = 0;
                }

                //LOG.Info("顺时针");
            }
            else//逆时针经过
            {
                //LOG.Info("逆时针");
            }


        }

        hadrunAngleM = timesTurns * 360 + (360 - angleM);
        //LOG.Info("hadrunAngleM:" + hadrunAngleM + "--timesTurns:" + timesTurns+ "--angleM:"+ angleM);
        float shu = (hadrunAngleM) / (NumberOfTurns * 360);

        if (hadrunAngleM >= -40)
        {
            canNoRetrogress = true;
        }
        else
        {
            canNoRetrogress = false;
        }

        if (canDrugEvent && shu >= 1)
        {
            canDrugEvent = false;
            ClockMove(2);
        }
        else
        {
#if !NOT_USE_LUA
            onClockChangeSceneAlpha(shu);
#else
            bookReadingForm.ClockChangeSceneAlpha(shu);
#endif
        }


        //LOG.Info("Alpa值是:" + shu);
    }

    private void OnEnable()
    {
        timesTurns = 0;
        canDrugEvent = false;
        ClockMove(1);
        pointerM.localEulerAngles = new Vector3(0, 0, 0);
        pointerH.localEulerAngles = new Vector3(0, 0, 295);

        //LOG.Info("Mp:"+ pointerM.localEulerAngles);
        //LOG.Info("Hp:" + pointerH.localEulerAngles);
    }

    public void OnDrag(PointerEventData eventData)
    {

        if (Input.mousePosition.y <= 560 && canDrugEvent)
        {
            DrugEvent();
        }

    }


#if !NOT_USE_LUA
    public void GetTurns(int numbers, Action<float> onClockChangeSceneAlpha)
    {
        NumberOfTurns = numbers;
        this.onClockChangeSceneAlpha = onClockChangeSceneAlpha;
    }
#else
    public void GetTurns(int numbers)
    {
        NumberOfTurns = numbers;
    }
#endif


    private void ClockMove(int type)
    {
        if (type == 1)
        {
            //这个是时钟移进

#if CHANNEL_SPAIN
            ClockGame.DOAnchorPosY(406, 0.8f).SetEase(Ease.OutBack).OnComplete(() =>
#else
            ClockGame.DOAnchorPosY(256, 0.8f).SetEase(Ease.OutBack).OnComplete(() =>
#endif
            {
                canDrugEvent = true;
            });
        }
        else if (type == 2)
        {
            //这个是时钟移出

#if CHANNEL_SPAIN
            ClockGame.DOAnchorPosY(406f, 1f).OnComplete(() =>
#else
            ClockGame.DOAnchorPosY(256f, 1f).OnComplete(() =>
#endif
            {
                ClockGame.DOAnchorPosY(-450, 1f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    gameObject.SetActive(false);

#if !NOT_USE_LUA
                    onClockChangeSceneAlpha(1);
#else
                    bookReadingForm.ClockChangeSceneAlpha(1);
#endif
                });
            });

        }
    }
}
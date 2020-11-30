using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using UGUI;

public class CatGuid : BaseUIForm {

    private RectTransform TouchArea;
    private RectTransform TopL, TopC, TopR, DownL, DownC, DownR, CenterL, CenterR;
    private Text ShowText, TouchAreaText;
    private Image Mask;
    private GameObject Content;


    private float TouchAreaW = 0;//可以点击的范围的宽度
    private float TouchAreaH = 0;//可以点击的范围的高度

    private float SceneW = 0;//屏幕的宽度
    private float SceneH = 0;//屏幕的高度

    private float TopDistance = 0;//获得触碰区域上部边缘到屏幕顶部的距离
    private float DownDistance = 0;//获得触碰区域底部边缘到屏幕低部的距离
    private float LeftDistance = 0;//获得触碰区域左部边缘到屏幕左部的距离
    private float RightDistance = 0;//获得触碰区域右部边缘到屏幕右部的距离
    
    public float SetTouchAreaW = 200;//设置点击区域的宽度
    public float SetTouchAreaH = 200;//设置点击区域的高度

    private string Conter;

    private GameObject Clock;
    private Text ClockText;
    private bool ClockCountDown = false;
    private Image finger;
    private GameObject arrow;
    private GameObject TextPrompt;
    private Text TextPromptCont;

    private CanvasGroup MaskCG, ContentCG, TouchAreaCG, ClockCG, TextPromptCG;
    private float Speed =0.03f;
    private bool lastNotShowTipsUi = false;
    private RectTransform fingerRect, TouchAreaTextRect, TextPromptRect, TextPromptContRect;

    private TextTyperAnimation TouchAreaAnimation, ShowAnimation, TextPromptContAnimation;
    private RectTransform ShouzhiRect;
    private GameObject TouchAreaGo;
    private Image TouchAreaImage;
    private float LastVlue;
    private int LastStepNum;//记录上次执行的步骤
    private bool isTweening,isTw0Tweening;

    public override void OnOpen()
    {
        base.OnOpen();

        isTweening = false;
        isTw0Tweening = false;
        LastVlue = 0;
        LastStepNum = 0;
        Content = transform.Find("Content").gameObject;
        ContentCG = Content.GetComponent<CanvasGroup>();

        TouchAreaGo = transform.Find("TouchArea").gameObject;
        TouchArea = TouchAreaGo.GetComponent<RectTransform>();
        TouchAreaCG= TouchAreaGo.GetComponent<CanvasGroup>();
        TouchAreaImage = TouchAreaGo.GetComponent<Image>();


        ShowText = transform.Find("Content/ShowText").GetComponent<Text>();
        Mask = transform.Find("Mask").GetComponent<Image>();
        MaskCG = transform.Find("Mask").GetComponent<CanvasGroup>();

        TopL = transform.Find("TouchArea/TopL").GetComponent<RectTransform>();
        TopC = transform.Find("TouchArea/TopC").GetComponent<RectTransform>();
        TopR = transform.Find("TouchArea/TopR").GetComponent<RectTransform>();
        DownL = transform.Find("TouchArea/DownL").GetComponent<RectTransform>();
        DownC = transform.Find("TouchArea/DownC").GetComponent<RectTransform>();
        DownR = transform.Find("TouchArea/DownR").GetComponent<RectTransform>();
        CenterL = transform.Find("TouchArea/CenterL").GetComponent<RectTransform>();
        CenterR = transform.Find("TouchArea/CenterR").GetComponent<RectTransform>();

        TouchAreaText = transform.Find("TouchArea/finger/TouchAreaText").GetComponent<Text>();

        Clock = transform.Find("Clock").gameObject;
        ClockCG = Clock.GetComponent<CanvasGroup>();

        ClockText = transform.Find("Clock/ClockText").GetComponent<Text>();

        finger = transform.Find("TouchArea/finger").GetComponent<Image>();
        arrow = transform.Find("TouchArea/finger/arrow").gameObject;
        TextPrompt = transform.Find("TextPrompt").gameObject;
        TextPromptCG = TextPrompt.GetComponent<CanvasGroup>();

        TextPromptCont = transform.Find("TextPrompt/Text").GetComponent<Text>();

        ShouzhiRect = transform.Find("TouchArea/Shouzhi").GetComponent<RectTransform>();

        fingerRect = finger.GetComponent<RectTransform>();
        TouchAreaTextRect = TouchAreaText.GetComponent<RectTransform>();

        TextPromptRect = TextPrompt.GetComponent<RectTransform>();
        TextPromptContRect = TextPromptCont.GetComponent<RectTransform>();

        //获取点击区域的宽度和高度
        TouchAreaW = TouchArea.rect.width;
        TouchAreaH= TouchArea.rect.height;

        //获取屏幕的分辨率
        SceneW = Mask.rectTransform.rect.width;
        SceneH = Mask.rectTransform.rect.height;

        addMessageListener(EventEnum.DoGuidStep, DoGuidStep);
        UIEventListener.AddOnClickListener(Mask.gameObject, MaskButtonOn);

        addMessageListener(EventEnum.ClockTime, ClockTime);
        addMessageListener(EventEnum.CatGuidCanvasGroupOFF, CatGuidCanvasGroupOFF);
        addMessageListener(EventEnum.TouchAreaCanTouch, TouchAreaCanTouch);


        TouchAreaAnimation = TouchAreaText.GetComponent<TextTyperAnimation>();
        ShowAnimation = ShowText.GetComponent<TextTyperAnimation>();
        TextPromptContAnimation = TextPromptCont.GetComponent<TextTyperAnimation>();

    }

    public override void OnClose()
    {
        base.OnClose();

        UIEventListener.RemoveOnClickListener(Mask.gameObject, MaskButtonOn);

    }

    /// <summary>
    /// 派发事件执行，引导的步骤
    /// </summary>
    public void DoGuidStep(Notification notification)
    {
       
        ShowText.text = "";
        TouchAreaText.text = "";
        TextPromptCont.text = "";

        TouchAreaAnimation.Progress = 0;
        ShowAnimation.Progress = 0;
        TextPromptContAnimation.Progress = 0;

        int StepNum = (int)notification.Data;

       
        if (LastStepNum != StepNum)
        {
            //Debug.Log("dfd设置为0");
            TextPromptRect.sizeDelta = new Vector2(348, 0);
            fingerRect.sizeDelta = new Vector2(348, 0);

            isTw0Tweening = false;
            isTweening = false;
        }
        else
        {
            if (isTweening)
            {
                isTw0Tweening = true;
            }          
        }
        LastStepNum = StepNum;
        CatGuidStep(StepNum);

        LOG.Info("DoGuidStep:" + StepNum);
    }


    private void CatGuidStep(int Num)
    {
        Content.gameObject.SetActive(false);
        TouchArea.gameObject.SetActive(false);      
        Mask.gameObject.SetActive(false);
        Clock.SetActive(false);
        arrow.SetActive(false);
        TextPrompt.SetActive(false);
        CanvasGroupON();

        UserDataManager.Instance.GuidStupNum = Num;

        ModuleTransfer Mod = new ModuleTransfer();
        Mod.CatGuidEnum = Num;
        Mod.GoToModule();

        int Type = Mod.GetUiType();
        Vector3 vect = Mod.GetVector3();
        int SetTouchAreaW = Mod.GetTouchAreaW();
        int SetTouchAreaH = Mod.GetTouchAreaH();
        Conter = Mod.GetContern();

        Debug.Log("UItype:"+Type+"--VECT:"+vect);

       
        switch (Type)
        {
            case (int)CatGuidUiType.ShowTipsUi:

                UserDataManager.Instance.CatGuidIsCanTouch = false;

                //这是文字UI显示
                ShowTipsUi();
                break;
            case (int)CatGuidUiType.OnClickeUi:

                TouchAreaImage.raycastTarget = true;
                lastNotShowTipsUi = false;
                //这个是只能中心图片点击
                ShowCatDownButton(SetTouchAreaW, SetTouchAreaH, true, vect.x, vect.y);

                Invoke("fingerImagSet",0.3f);               
                break;
            case (int)CatGuidUiType.ClockUi:

                lastNotShowTipsUi = false;
                //这个是显示倒计时的界面
                ClockCountDown = true;
                ClockText.text = "10";
                Mask.gameObject.SetActive(true);

                Clock.transform.GetComponent<RectTransform>().anchoredPosition3D = UserDataManager.Instance.GuidFingerPos;
                break;

            case (int)CatGuidUiType.TextPromptUi:

                UserDataManager.Instance.CatGuidIsCanTouch = false;

                lastNotShowTipsUi = false;
                TextPromptOn();
                break;
        }      
    }
 
    private void fingerImagSet()
    {
        CancelInvoke("fingerImagSet");

        float TextHeight = TouchAreaTextRect.rect.height;

        float Difference = 0; //差值

        LOG.Info("手指文本的高度：" + TextHeight);

        finger.GetComponent<RectTransform>().anchoredPosition3D = UserDataManager.Instance.GuidFingerPos;

        TouchAreaText.rectTransform.anchoredPosition3D = new Vector3(2,57, 0);

        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.CatButtonGuid|| UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.PersonalCenterCatEnter)
        {
            //猫按钮提示图标
            Difference = 84;
            finger.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/bg_window3");

        }else if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.PlaceHuangyuandian)
        {
            //中间角箭头
            finger.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/bg_window2");
            arrow.SetActive(true);

            Difference = 84;
           
        }
        else if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.GetGiftGuid)
        {
            Difference = 36;
            //右上角箭头
            finger.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/bg_window1");

            TouchAreaText.rectTransform.anchoredPosition3D = new Vector3(0,8,0);
            //finger.GetComponent<RectTransform>().anchoredPosition3D =new Vector3(UserDataManager.Instance.GuidFingerPos.x, UserDataManager.Instance.GuidFingerPos.y-40, UserDataManager.Instance.GuidFingerPos.z);

        }
        else
        {
            Difference = 84;
            finger.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/bg_window2");

        }


        if (isTw0Tweening) return;

            DOTween.To(() => 0, (value) => {
                isTweening = true;
            fingerRect.sizeDelta = new Vector2(348, value);
           
        }, TextHeight + Difference, 0.5f).OnComplete(() => {
           
            //TextTyperAnimation TextTyperAnimation = TouchAreaText.GetComponent<TextTyperAnimation>();
            TouchAreaAnimation.NeedTime = Speed;
            TouchAreaAnimation.CatDoTyperTween();

            
        });
        
        //finger.SetNativeSize();
       
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="SetTouchAreaW">触碰区域的宽度</param>
    /// <param name="SetTouchAreaH">触碰区域的高度</param>
    /// <param name="NoUsePos">是否是根据位置来生触碰区域的位置</param>
    /// <param name="x">不根据位置而是根据anchored 来生成触碰区域的位置  x 的坐标</param>
    /// <param name="y">不根据位置而是根据anchored 来生成触碰区域的位置  y 的坐标</param>
    private void ShowCatDownButton(float SetTouchAreaW=200, float SetTouchAreaH = 200,bool NoUsePos=false, float x = 200, float y = 200)
    {
        if (Conter.Equals("")) return;
        TouchAreaText.text = Conter; //显示文字
       
      
        //设置点击的范围大小
        TouchArea.sizeDelta = new Vector2(SetTouchAreaW, SetTouchAreaH);

        TouchArea.gameObject.SetActive(true);
        float PosX =0;
        float PosY =0;
        if (NoUsePos)
        {
            TouchArea.anchoredPosition = new Vector2(x,y);
            PosX = x;
            PosY = y;
        }
        else
        {
            TouchArea.transform.position =UserDataManager.Instance.GuidPos;
            PosX = TouchArea.anchoredPosition.x;
            PosY = TouchArea.anchoredPosition.y;

        }

        //手指特效的位置
        ShouzhiRect.anchoredPosition3D= UserDataManager.Instance.GuidFingerEFPos;

        //Debug.Log("GuidPos:"+ UserDataManager.Instance.GuidPos);              
        //Debug.Log("x:"+ PosX + "--y:"+ PosX);

        TopDistance = SceneH - PosY - SetTouchAreaH / 2.0f;//获得触碰区域上部边缘到屏幕顶部的距离
        DownDistance = PosY- SetTouchAreaH / 2.0f;//获得触碰区域底部边缘到屏幕低部的距离
        LeftDistance = PosX - SetTouchAreaW / 2.0f;//获得触碰区域左部边缘到屏幕左部的距离
        RightDistance = SceneW - PosX - SetTouchAreaW / 2.0f;//获得触碰区域右部边缘到屏幕右部的距离


    //Debug.Log("TopDistance:" + TopDistance + "--DownDistance:" + DownDistance + "==LeftDistance:" + LeftDistance + "--RightDistance:" + RightDistance);

        //设置对应的图片填充的大小
        if (LeftDistance <= 0)
        {
            //左边已经贴边，不需要设置
        }
        else
        {
            TopL.sizeDelta = new Vector2(LeftDistance, TopDistance);
        }      
        TopC.sizeDelta = new Vector2(SetTouchAreaW, TopDistance);

        if (RightDistance<=0)
        {
            //右边已经贴边，不需要设置
        }
        else
        {
            TopR.sizeDelta = new Vector2(RightDistance, TopDistance);
        }

        if (DownDistance<=0)
        {
            //底部左边已经贴边，不需要设置
        }
        else
        {
            DownL.sizeDelta = new Vector2(LeftDistance, DownDistance);
        }      
        DownC.sizeDelta = new Vector2(SetTouchAreaW, DownDistance);

        if (RightDistance<=0)
        {
            //底部右边已经贴边，不需要设置
        }
        else
        {
            DownR.sizeDelta = new Vector2(RightDistance, DownDistance);
        }

        if (LeftDistance<=0)
        {
            //左边已经贴边，不需要设置
        }
        else
        {
            CenterL.sizeDelta = new Vector2(LeftDistance, SetTouchAreaH);
        }

        if (RightDistance<=0)
        {
            //右边已经贴边，不需要设置
        }
        else
        {
            CenterR.sizeDelta = new Vector2(RightDistance,SetTouchAreaH);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    private void ShowTipsUi()
    {
        Mask.gameObject.SetActive(true);
        Content.SetActive(true);

      
        //TextTyperAnimation TextTyperAnimation = ShowText.GetComponent<TextTyperAnimation>();
        if (!lastNotShowTipsUi)
        {
            lastNotShowTipsUi = true;
            Content.transform.localScale = new Vector3(1, 0, 1);
            Content.transform.DOScaleY(1, 0.3f).SetEase(Ease.OutBack).OnComplete(() => {

                ShowText.text = Conter;

                ShowAnimation.NeedTime = Speed;
                ShowAnimation.CatDoTyperTween();

            });
        }else
        {
            Content.transform.localScale = new Vector3(1,1, 1);
            ShowText.text = Conter;

            ShowAnimation.NeedTime = Speed;
            ShowAnimation.CatDoTyperTween();
        }
       
    }

    private void TextPromptOn()
    {
        Mask.gameObject.SetActive(true);
        TextPrompt.SetActive(true);
        TextPromptCont.text = Conter;

        TextPrompt.transform.GetComponent<RectTransform>().anchoredPosition3D= UserDataManager.Instance.GuidFingerPos;

        Invoke("TextPromptAdaptation",0.3f);
    }

    private void TextPromptAdaptation()
    {
        CancelInvoke("TextPromptAdaptation");

        float TextH = TextPromptContRect.rect.height;

       
        DOTween.To(() => 0, (value) => {
            TextPromptRect.sizeDelta = new Vector2(348, value);
           
        }, TextH + 84, 0.5f).OnComplete(()=> {

            //TextTyperAnimation TextTyperAnimation = TextPromptCont.GetComponent<TextTyperAnimation>();
            TextPromptContAnimation.NeedTime = Speed;
            TextPromptContAnimation.CatDoTyperTween();
        });

    }

    /// <summary>
    /// 显示倒计时
    /// </summary>
    private void ClockTime(Notification notification)
    {
        int Times = (int)notification.Data;
        Clock.SetActive(true);
        ClockText.text = Times.ToString();

        if (Times<=0)
        {
            //倒计时结束，关闭界面
            ClockCountDown = false;
            EventDispatcher.Dispatch(EventEnum.CatGuidUiClose);
        }

        Clock.transform.GetComponent<RectTransform>().anchoredPosition3D = UserDataManager.Instance.GuidFingerPos;
    }

    /// <summary>
    /// 界面设置为可见
    /// </summary>
    private void CanvasGroupON()
    {
        MaskCG.alpha = 1;
        ContentCG.alpha = 1;
        TouchAreaCG.alpha = 1;
        ClockCG.alpha = 1;
        TextPromptCG.alpha = 1;
    }

    /// <summary>
    /// 界面设置为不可见
    /// </summary>
    private void CatGuidCanvasGroupOFF(Notification notification)
    {
        if (lastNotShowTipsUi)
        {
            //使用同样的显示文本，第二次不隐藏
        }else
        {
            TouchAreaImage.raycastTarget = true;
            MaskCG.alpha = 0;
            ContentCG.alpha = 0;
            TouchAreaCG.alpha = 0;
            ClockCG.alpha = 0;
            TextPromptCG.alpha = 0;
        }
       
    }

    public void MaskButtonOn(PointerEventData data)
    {
        if (ClockCountDown|| !UserDataManager.Instance.CatGuidIsCanTouch) return;


        LOG.Info("Mask被点击了");
        EventDispatcher.Dispatch(EventEnum.CatGuidUiClose);
    }

    public void TouchAreaCanTouch(Notification notice)
    {
        TouchAreaImage.raycastTarget = false;
    }

}

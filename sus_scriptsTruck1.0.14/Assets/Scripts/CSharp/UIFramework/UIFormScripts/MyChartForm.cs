using System;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class MyChartForm : CatBaseForm {

    
    private Text MatureText, RigorousText, ElegantText, CuriousText, AffectionateText;
    private GameObject UiMask;

    private ArrayList SliderNum;
    private ProfileDetailInfo detailInfo;
    private int maxValue = 0;
    private Text Tile, MatureSliderText, RigorousSliderText, ElegantSliderText, CuriousSliderText, AffectionateSliderText;

    private Image MatureSliderPress, RigorousSliderPress, ElegantSliderPress, CuriousSliderPress, AffectionateSliderPress;
    private Image MatureSliderPressD, RigorousSliderPressD, ElegantSliderPressD, CuriousSliderPressD, AffectionateSliderPressD;
    private Text MatureSliderPercentage, RigorousSliderPercentage, ElegantSliderPercentage, CuriousSliderPercentage, AffectionateSliderPercentage;

    private void Awake()
    {
       
        MatureSliderPress= transform.Find("Bg/ChartSlider/MatureSlider/RingBg/Press").GetComponent<Image>();
        MatureSliderPressD = transform.Find("Bg/ChartSlider/MatureSlider/RingBg/PressD").GetComponent<Image>();
        MatureSliderPercentage= transform.Find("Bg/ChartSlider/MatureSlider/percentage").GetComponent<Text>();

        RigorousSliderPress = transform.Find("Bg/ChartSlider/RigorousSlider/RingBg/Press").GetComponent<Image>();
        RigorousSliderPressD = transform.Find("Bg/ChartSlider/RigorousSlider/RingBg/PressD").GetComponent<Image>();
        RigorousSliderPercentage = transform.Find("Bg/ChartSlider/RigorousSlider/percentage").GetComponent<Text>();


        ElegantSliderPress = transform.Find("Bg/ChartSlider/ElegantSlider/RingBg/Press").GetComponent<Image>();
        ElegantSliderPressD = transform.Find("Bg/ChartSlider/ElegantSlider/RingBg/PressD").GetComponent<Image>();
        ElegantSliderPercentage = transform.Find("Bg/ChartSlider/ElegantSlider/percentage").GetComponent<Text>();


        CuriousSliderPress = transform.Find("Bg/ChartSlider/CuriousSlider/RingBg/Press").GetComponent<Image>();
        CuriousSliderPressD = transform.Find("Bg/ChartSlider/CuriousSlider/RingBg/PressD").GetComponent<Image>();
        CuriousSliderPercentage = transform.Find("Bg/ChartSlider/CuriousSlider/percentage").GetComponent<Text>();


        AffectionateSliderPress = transform.Find("Bg/ChartSlider/AffectionateSlider/RingBg/Press").GetComponent<Image>();
        AffectionateSliderPressD = transform.Find("Bg/ChartSlider/AffectionateSlider/RingBg/PressD").GetComponent<Image>();
        AffectionateSliderPercentage = transform.Find("Bg/ChartSlider/AffectionateSlider/percentage").GetComponent<Text>();
        

        Tile = transform.Find("Bg/Tile").GetComponent<Text>();
        Tile.text = GameDataMgr.Instance.table.GetLocalizationById(104);

        MatureText = transform.Find("Bg/ChartSlider/MatureSlider/MatureText").GetComponent<Text>();
        MatureSliderText = transform.Find("Bg/ChartSlider/MatureSlider/Text").GetComponent<Text>();
        MatureSliderText.text = GameDataMgr.Instance.table.GetLocalizationById(22);

        RigorousText = transform.Find("Bg/ChartSlider/RigorousSlider/RigorousText").GetComponent<Text>();
        RigorousSliderText = transform.Find("Bg/ChartSlider/RigorousSlider/Text").GetComponent<Text>();
        RigorousSliderText.text= GameDataMgr.Instance.table.GetLocalizationById(23);

        ElegantText = transform.Find("Bg/ChartSlider/ElegantSlider/ElegantText").GetComponent<Text>();
        ElegantSliderText = transform.Find("Bg/ChartSlider/ElegantSlider/Text").GetComponent<Text>();
        ElegantSliderText.text = GameDataMgr.Instance.table.GetLocalizationById(24);

        CuriousText = transform.Find("Bg/ChartSlider/CuriousSlider/CuriousText").GetComponent<Text>();
        CuriousSliderText = transform.Find("Bg/ChartSlider/CuriousSlider/Text").GetComponent<Text>();
        CuriousSliderText.text= GameDataMgr.Instance.table.GetLocalizationById(25);

        AffectionateText = transform.Find("Bg/ChartSlider/AffectionateSlider/AffectionateText").GetComponent<Text>();
        AffectionateSliderText = transform.Find("Bg/ChartSlider/AffectionateSlider/Text").GetComponent<Text>();
        AffectionateSliderText.text= GameDataMgr.Instance.table.GetLocalizationById(26);

        UiMask = transform.Find("UIMask").gameObject;
    }
    public override void OnOpen()
    {
        base.OnOpen();
        UiFormIndex = (int)CatFormEnum.CAT_MY_CHART;

        UIEventListener.AddOnClickListener(UiMask, CloseUi);

        init();
        StartShow();
    }

    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(UiMask, CloseUi);
    }

    private void CloseUi(PointerEventData data)
    {  
        //派发事件，关闭这个界面
        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), UiFormIndex);     
    }

    /// <summary>
    /// 界面刚出现的效果显示
    /// </summary>
    private void StartShow()
    {
        Transform BG = transform.Find("Bg");
        BG.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        BG.DOScale(new Vector3(1, 1, 1), 0.4f).SetEase(Ease.OutBack);


        Transform CloseButton = transform.Find("CloseButton");
        CloseButton.GetComponent<RectTransform>().DOAnchorPosY(-384, 0.4f).SetEase(Ease.OutBack);
    }

    private void init()
    {
        if (SliderNum == null)
        {
            SliderNum = new ArrayList();
        }
        SliderNum.Clear();
        maxValue = 0;

        if (UserDataManager.Instance.profileData == null)
        {
            //UINetLoadingMgr.Instance.Show();

        }else
        {
            ShowSliderValue();
        }


    }

    private void ShowSliderValue()
    {
        if (UserDataManager.Instance.profileData.data.type==1)
        {
            //未解锁性格图
            MatureSliderPress.fillAmount = 0;
            RigorousSliderPress.fillAmount = 0;
            ElegantSliderPress.fillAmount = 0;
            CuriousSliderPress.fillAmount = 0;
            AffectionateSliderPress.fillAmount = 0;

            MatureText.text = "0";
            RigorousText.text = "0";
            ElegantText.text = "0";
            CuriousText.text = "0";
            AffectionateText.text = "0";
            return;
        }
        detailInfo = UserDataManager.Instance.profileData.data.info;
        SliderNum.Add(detailInfo.mature);
        SliderNum.Add(detailInfo.rigorous);
        SliderNum.Add(detailInfo.reasonable);
        SliderNum.Add(detailInfo.curious);
        SliderNum.Add(detailInfo.tsundere);

        SliderNum.Sort();//升序排列
        maxValue = (int)SliderNum[SliderNum.Count - 1];

        float percent;
        percent = (float)detailInfo.mature / (maxValue * 1.0f);
        MatureSliderPress.fillAmount = percent;
        SetCicleAngle(MatureSliderPressD, percent);
        MatureSliderPercentage.text = string.Format("{0}%", (int)(percent * 100));

        percent = (float)detailInfo.rigorous / (maxValue * 1.0f);
        RigorousSliderPress.fillAmount = percent;
        SetCicleAngle(RigorousSliderPressD, percent);
        RigorousSliderPercentage.text = string.Format("{0}%", (int)(percent * 100));


        percent = (float)detailInfo.reasonable / (maxValue * 1.0f);
        ElegantSliderPress.fillAmount = percent;
        SetCicleAngle(ElegantSliderPressD, percent);
        ElegantSliderPercentage.text = string.Format("{0}%", (int)(percent * 100));

        percent = (float)detailInfo.curious / (maxValue * 1.0f);
        CuriousSliderPress.fillAmount = percent;
        SetCicleAngle(CuriousSliderPressD, percent);
        CuriousSliderPercentage.text = string.Format("{0}%", (int)(percent * 100));


        percent = (float)detailInfo.tsundere / (maxValue * 1.0f);
        AffectionateSliderPress.fillAmount = percent;
        SetCicleAngle(AffectionateSliderPressD, percent);
        AffectionateSliderPercentage.text = string.Format("{0}%", (int)(percent * 100));


        MatureText.text = detailInfo.mature.ToString()+"/"+ maxValue;
        RigorousText.text = detailInfo.rigorous.ToString() + "/" + maxValue;
        ElegantText.text = detailInfo.reasonable.ToString() + "/" + maxValue;
        CuriousText.text = detailInfo.curious.ToString() + "/" + maxValue;
        AffectionateText.text = detailInfo.tsundere.ToString() + "/" + maxValue;
    }

    private void SetCicleAngle(Image vCircle, float vPercent)
    {
        //vCircle.fillAmount = vPercent;
        float curAngle = -360.0f * vPercent;

        vCircle.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, curAngle));

    }
    
}

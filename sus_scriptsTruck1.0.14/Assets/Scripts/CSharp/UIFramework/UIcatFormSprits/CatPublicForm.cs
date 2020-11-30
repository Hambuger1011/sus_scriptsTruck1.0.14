using System;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class CatPublicForm : CatBaseForm {

    private Text FoodName, ContentText, TileText, BtnText;

    private GameObject BackButton,nobutton,yesbutton;
    private  Action<string> YesButtonCall;
    private Action<string> NoButtonCall;
    public GameObject Type1, Type2;
    public Text AnimalText, SpaceText, extendStr;

    private GameObject Type3;
    private Text Type3ContentText;
    private Image Type3priceType;
    private Text Type3Price;
    private RectTransform BGRect;

    private GameObject NotTip, chooseYes, chooseNo;
    private bool isNotTip = false;
    private int WindowType;
    private Text nobuttonText;

    private void Awake()
    {
        BackButton = transform.Find("UIMask").gameObject;
        FoodName = transform.Find("Bg/FoodName").GetComponent<Text>();
        ContentText = transform.Find("Bg/Type1/ContentBg/ContentText").GetComponent<Text>();
        TileText = transform.Find("Bg/Type1/TileText").GetComponent<Text>();
        BtnText = transform.Find("Bg/yesbutton/Text").GetComponent<Text>();
        BtnText.text = GameDataMgr.Instance.table.GetLocalizationById(271);

        nobutton = transform.Find("Bg/nobutton").gameObject;
        nobuttonText = transform.Find("Bg/nobutton/Text").GetComponent<Text>();
        nobuttonText.text= GameDataMgr.Instance.table.GetLocalizationById(272);

        yesbutton = transform.Find("Bg/yesbutton").gameObject;

        Type3 = transform.Find("Bg/Type3").gameObject;
        Type3ContentText= transform.Find("Bg/Type3/ContentBg/ContentText").GetComponent<Text>();
        Type3priceType= transform.Find("Bg/Type3/ContentBg/priceType").GetComponent<Image>();
        Type3Price = transform.Find("Bg/Type3/ContentBg/priceType/Price").GetComponent<Text>();

        NotTip = transform.Find("Bg/NotTip").gameObject;
        chooseYes= transform.Find("Bg/NotTip/chooseYes").gameObject;
        chooseNo = transform.Find("Bg/NotTip/chooseNo").gameObject;
    }
    public override void OnOpen()
    {
        base.OnOpen();
        UiFormIndex = (int)CatFormEnum.CAT_PUBLIC;

        UIEventListener.AddOnClickListener(BackButton, CloseUi);
        UIEventListener.AddOnClickListener(nobutton,nobuttonOnclicke);
        UIEventListener.AddOnClickListener(yesbutton,YesButtonOnclicke);

        UIEventListener.AddOnClickListener(NotTip, NotTipOnclice);

        addMessageListener(EventEnum.SetGuidPos, SetGuidPos);


        NotTip.SetActive(false);

        StartShow();

        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.HuangyuandianYesOnclick|| UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.PlaceHuangyuandianYes|| UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.PlaceDecorationsYes)
        {
            //Debug.Log("----------dfdddf");
            Invoke("OpenGuid",0.3f);
        }

    }

    private void OpenGuid()
    {
        EventDispatcher.Dispatch(EventEnum.OpenCatGuid);//打开引导界面
    }

    public override void OnClose()
    {
        base.OnClose();

        UIEventListener.RemoveOnClickListener(BackButton, CloseUi);
        UIEventListener.RemoveOnClickListener(nobutton, nobuttonOnclicke);
        UIEventListener.RemoveOnClickListener(yesbutton, YesButtonOnclicke);

        UIEventListener.RemoveOnClickListener(NotTip, NotTipOnclice);

    }


    private void CloseUi(PointerEventData data)
    {
        //派发事件，关闭这个界面
        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), UiFormIndex);
    }

    private void NotTipOnclice(PointerEventData data)
    {
        isNotTip = !isNotTip;

        if (isNotTip)
        {
            chooseYes.SetActive(true);
            chooseNo.SetActive(false);
        }else
        {
            chooseYes.SetActive(false);
            chooseNo.SetActive(true);
        }
    }

    /// <summary>
    /// 界面刚出现的效果显示
    /// </summary>
    private void StartShow()
    {
        Transform BG = transform.Find("Bg");
        BG.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        BG.DOScale(new Vector3(1, 1, 1), 0.4f).SetEase(Ease.OutBack);
        BGRect = BG.GetComponent<RectTransform>();

        //Transform CloseButton = transform.Find("Bg/CloseButton");
        //CloseButton.GetComponent<RectTransform>().DOAnchorPosY(-378, 0.4f).SetEase(Ease.OutBack);
    }

    public void Inite(WindowInfo info)
    {

        if (info == null )
        {
            LOG.Error("No WindowInfo");
            return;
        }
        WindowType = info.WindowType;
        this.FoodName.text = info.titleNme.ToString();
        this.NoButtonCall = info.noButtonCall;
        this.YesButtonCall = info.yesButtonCall;
        if (info.WindowType == 2)
        {
            Type1.SetActive(false);
            Type2.SetActive(true);
            extendStr.text = info.extendDes;
            SpaceText.text = info.SpaceCountText;
            AnimalText.text = info.animalCountText;

            BGRect.sizeDelta = new Vector2(620,717);
        }
        else if (info.WindowType == 3)
        {
            //这个是收养弹框
            Type3.SetActive(true);

            string Qcontss = info.content.ToString();
            string St1 = Qcontss.Replace("\\n", "\n");
            string St= St1.Replace("/n", "\n");
            Type3ContentText.text = St;
            //Type3ContentText.text= info.content.ToString();

            //Type3priceType.sprite=
            Type3Price.text = info.tileText.ToString();

            BGRect.sizeDelta = new Vector2(620, 560);
        }
        else if (info.WindowType == 4)
        {
            //这个是放养猫弹框
            //这个是收养弹框
            Type3.SetActive(true);

            string Qcontss = info.content.ToString();
            string St1 = Qcontss.Replace("\\n", "\n");
            string St = St1.Replace("/n", "\n");
            Type3ContentText.text = St;
            //Type3ContentText.text = info.content.ToString();

            Type3priceType.gameObject.SetActive(false);
            //Type3Price.text = info.tileText.ToString();

            BGRect.sizeDelta = new Vector2(620, 560);
        }else if (info.WindowType == 5)
        {
            //这个是出现选择不提示的界面

            Type1.SetActive(true);
            Type2.SetActive(false);
            NotTip.SetActive(true);


            string Qcontss = info.content.ToString();
            string St1 = Qcontss.Replace("\\n", "\n");
            string St = St1.Replace("/n", "\n");
            ContentText.text = St;
            this.TileText.text = info.tileText.ToString();

            BGRect.sizeDelta = new Vector2(620, 722);

            if (isNotTip)
            {
                chooseYes.SetActive(true);
                chooseNo.SetActive(false);
            }
            else
            {
                chooseYes.SetActive(false);
                chooseNo.SetActive(true);
            }

        }
        else
        {
            Type1.SetActive(true);
            Type2.SetActive(false);


            string Qcontss = info.content.ToString();
            string St1 = Qcontss.Replace("\\n", "\n");
            string St = St1.Replace("/n", "\n");
            ContentText.text = St;
            this.TileText.text = info.tileText.ToString();

            BGRect.sizeDelta = new Vector2(620, 610);
        }
      
    }

    private void nobuttonOnclicke(PointerEventData data)
    {
        CloseUi(null);

        if (this.WindowType == 5)
        {
            if (isNotTip)
            {
                NoButtonCall("Yes");
            }else
            {
                NoButtonCall("No");
            }
                
        }
        else
        {
            NoButtonCall(null);
        }
           
    }

   private void YesButtonOnclicke(PointerEventData data)
    {
        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.PlaceHuangyuandianYes)
        {
            EventDispatcher.Dispatch(EventEnum.CatGuidUiClose);//关闭引导界面
        }

        CloseUi(null);

        if (this.WindowType == 5)
        {
            if (isNotTip)
            {
                YesButtonCall("Yes");
            }
            else
            {
                YesButtonCall("No");
            }

        }else 
        {
            YesButtonCall(null);
        }

       
    }


    /// <summary>
    /// 设置引导点击的位置
    /// </summary>
    /// <param name="notice"></param>
    public void SetGuidPos(Notification notice)
    {
        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.HuangyuandianYesOnclick|| UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.PlaceHuangyuandianYes || UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.PlaceDecorationsYes)
        {
            RectTransform BackButtonRect = BackButton.GetComponent<RectTransform>();
            RectTransform BgRect = transform.Find("Bg").GetComponent<RectTransform>();
            RectTransform yesbuttonRect = yesbutton.GetComponent<RectTransform>();

            float Hhalf = BackButtonRect.rect.height / 2.0f;
            float Whalf= BackButtonRect.rect.width / 2.0f;
            float BgRectHhalf = BgRect.rect.height / 2.0f;
            float BgRectWhalf= BgRect.rect.width / 2.0f;


            float Posx = Whalf+ yesbuttonRect.anchoredPosition.x;
            float Posy = Hhalf- BgRectHhalf+ yesbuttonRect.anchoredPosition.y;

            UserDataManager.Instance.GuidPos = new Vector3(Posx, Posy, 1);
            //LOG.Info("得到食物放置确定按钮的坐标");
        }
    }

}
public class WindowInfo
{
    public string titleNme;
    public string content;
    public string tileText;
    public string BtnText;
    public Action<string> noButtonCall;
    public Action<string> yesButtonCall;
    public int WindowType;
    public string animalCountText;
    public string SpaceCountText;
    public string Cost;
    public string extendDes;


    public WindowInfo(string titleNme, string content, string tileText, string BtnText, Action<string> noButtonCall,
       Action<string> yesButtonCall, int WindowType, string animalCountText, string SpaceCountText, string Cost,string extendDes = null)
    {
        this.titleNme = titleNme;
        this.content = content;
        this.tileText = tileText;
        this.BtnText = BtnText;
        this.noButtonCall = noButtonCall;
        this.yesButtonCall = yesButtonCall;
        this.WindowType = WindowType;
        this.animalCountText = animalCountText;
        this.SpaceCountText = SpaceCountText;
        this.Cost = Cost;
        this.extendDes = extendDes;
    }

}

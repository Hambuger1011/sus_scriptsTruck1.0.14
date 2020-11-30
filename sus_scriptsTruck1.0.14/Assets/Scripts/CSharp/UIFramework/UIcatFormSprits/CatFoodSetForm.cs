using System;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using pb;
using DG.Tweening;

public class CatFoodSetForm : CatBaseForm
{

    private Text ContentText,NameText,Count;

    private GameObject BackButton, Yesbutton, NextBtn,PreviousBtn,NoBtn;
    private Image FoodImg = null;
    private Transform BG, CloseButton;

    int index = 0;
    string to_id;
    private int type = 0;
    private Text YesbuttonText, NoBtnText;
    private void Awake()
    {
        BackButton = transform.Find("UIMask").gameObject;
        ContentText = transform.Find("Bg/ContentBg/ContentText").GetComponent<Text>();
        Count = transform.Find("Bg/Count").GetComponent<Text>();
        NameText = transform.Find("Bg/NameText").GetComponent<Text>();
        FoodImg  = transform.Find("Bg/FoodObj/FoodItem/FoodImg").gameObject.GetComponent<Image>();

        Yesbutton = transform.Find("Bg/Yesbutton").gameObject;
        YesbuttonText = transform.Find("Bg/Yesbutton/Text").GetComponent<Text>();
        YesbuttonText.text = GameDataMgr.Instance.table.GetLocalizationById(271);

        NoBtn = transform.Find("Bg/Nobutton").gameObject;
        NoBtnText = transform.Find("Bg/Nobutton/Text").GetComponent<Text>();
        NoBtnText.text = GameDataMgr.Instance.table.GetLocalizationById(272);

        PreviousBtn = transform.Find("Bg/PreviousBtn").gameObject;
        NextBtn = transform.Find("Bg/NextBtn").gameObject;

        BG= transform.Find("Bg");
        CloseButton = transform.Find("CloseButton");

    }
    public override void OnOpen()
    {
        base.OnOpen();
        UiFormIndex = (int)CatFormEnum.CAT_FOODSET;


        UIEventListener.AddOnClickListener(BackButton, CloseUi);
        UIEventListener.AddOnClickListener(Yesbutton, SetButtonOnclicke);
        UIEventListener.AddOnClickListener(NextBtn, OnBtnNextClicked);//右
        UIEventListener.AddOnClickListener(PreviousBtn, OnBtnPreviousClicked);//左
        UIEventListener.AddOnClickListener(NoBtn, CloseUi);

        UIEventListener.AddOnClickListener(FoodImg.gameObject, FoodOnclicke);

      
        addMessageListener(EventEnum.SetGuidPos, SetGuidPos);
        StartShow();

        //打开引导界面
        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.PlaceFoodYes)
        {
            EventDispatcher.Dispatch(EventEnum.OpenCatGuid);//打开引导界面
        }

    }


    /// <summary>
    /// 界面刚出现的效果显示
    /// </summary>
    private void StartShow()
    {      
        BG.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        BG.DOScale(new Vector3(1, 1, 1), 0.4f).SetEase(Ease.OutBack);

        CloseButton.GetComponent<RectTransform>().DOAnchorPosY(-400, 0.4f).SetEase(Ease.OutBack);
    }

    public void SetFood(List<FoodBagItem> foodpack)
    {
        if (foodpack == null || foodpack.Count == 0 || index > foodpack.Count)
        {
            LOG.Error("没有猫粮数据");
            return;
        }
        int id = foodpack[index].shop_id;
        FoodImg.sprite = ResourceManager.Instance.GetUISprite("CatSceneDecorations/" + id);
        if (foodpack[index].count == 0)
        {
            FoodImg.material = ShaderUtil.GrayMaterial();
        }
        else
        {
            FoodImg.material = null;
        }
        FoodImg.SetNativeSize();
        t_shop shopData = GameDataMgr.Instance.table.GetcatShopId(id);
        if(shopData != null)
        {
            string Qcontss = shopData.describe.ToString();
            ContentText.text = Qcontss.Replace("\\n", "\n");
            //ContentText.text = shopData.describe;
        }
           
        NameText.text = foodpack[index].option_name;
        if (foodpack[index].link_id>1)
        {
            Count.gameObject.SetActive(true);
            Count.text = "x" + foodpack[index].count;
        }
        else
        {
            Count.gameObject.SetActive(false);
        }
        
    }

    

    private void OnBtnPreviousClicked(PointerEventData eventData)
    {
        index -= 1;
        if (index < 0)
       {
            index = UserDataManager.Instance.BagInfoContent.data.foodpack.Count - 1 /*0*/;
       }
        
        SetFood(UserDataManager.Instance.BagInfoContent.data.foodpack);

        //Debug.Log("左："+index);
    }

    private void OnBtnNextClicked(PointerEventData eventData)
    {
        index += 1;
        if (index > UserDataManager.Instance.BagInfoContent.data.foodpack.Count-1)
        {
            index =0 /*UserDataManager.Instance.BagInfoContent.data.foodpack.Count - 1*/;
        }
        SetFood(UserDataManager.Instance.BagInfoContent.data.foodpack);
        //Debug.Log("右：" + index);
    }

    public override void OnClose()
    {
        base.OnClose();

        UIEventListener.RemoveOnClickListener(BackButton, CloseUi);
        UIEventListener.RemoveOnClickListener(Yesbutton, SetButtonOnclicke);
        UIEventListener.RemoveOnClickListener(NextBtn, OnBtnNextClicked);
        UIEventListener.RemoveOnClickListener(PreviousBtn, OnBtnPreviousClicked);
        UIEventListener.RemoveOnClickListener(NoBtn, CloseUi);
        UIEventListener.RemoveOnClickListener(FoodImg.gameObject, FoodOnclicke);
    }


    private void CloseUi(PointerEventData data)
    {
        //派发事件，关闭这个界面
        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), UiFormIndex);

        if (FoodImg != null)
        {
            FoodImg.gameObject.SetActive(false);
            FoodImg.sprite = null;
        }
       
        index = 0;
    }


    private void FoodOnclicke(PointerEventData vdata)
    {
        type = 1;
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.PostPlaceItem("100", UserDataManager.Instance.BagInfoContent.data.foodpack[index].shop_id.ToString(), ProcessPlaceItem);
    }


    private void SetButtonOnclicke(PointerEventData vdata)
    {

        //这个是在引导点击确定食盒放置的步骤中，点击完食盒放置后进行下一步引导
        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.PlaceFoodYes)
        {
            EventDispatcher.Dispatch(EventEnum.CatGuidUiClose);
        }

        type = 2;
        bool isFreeFood = true; //是否为免费食物  
        var pack = UserDataManager.Instance.SceneInfo.data.packarr;
        foreach (var item in pack)
        {
            if (item.pay_type == 1 && item.weight > 0)
            {
                t_shop shopItem = GameDataMgr.Instance.table.GetcatShopId(item.shop_id);
                if (shopItem != null)
                    isFreeFood = shopItem.pay == 0;
                break;
            }
        }

        if (UserDataManager.Instance.FoodNum>0&& !isFreeFood && UserDataManager.Instance.BagInfoContent.data.foodpack[index].count > 0)
        {
            if (UserDataManager.Instance.AlertFormClosTip == DateTime.Now.DayOfYear)
            {
                //点击了不提示按钮，直接替换，
                GameHttpNet.Instance.PostPlaceItem("100", UserDataManager.Instance.BagInfoContent.data.foodpack[index].shop_id.ToString(), ProcessPlaceItem);
                return;
            }

            CUIManager.Instance.OpenForm(UIFormName.CatPublicForm);
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(114);
            string cont = Localization;//"确定要替换这个装饰物吗？上面的猫咪会被赶走哦！"   慎重考虑！
            WindowInfo tmpWin = new WindowInfo(GameDataMgr.Instance.table.GetLocalizationById(218), cont, GameDataMgr.Instance.table.GetLocalizationById(245), "Yes",
                      PublicNoButtonCallBacke, PublicYesButtonCallback, 5, null, null, null);
            CUIManager.Instance.GetForm<CatPublicForm>(UIFormName.CatPublicForm).Inite(tmpWin);
         
        }
        else
        {
            GameHttpNet.Instance.PostPlaceItem("100", UserDataManager.Instance.BagInfoContent.data.foodpack[index].shop_id.ToString(), ProcessPlaceItem);
        }
    }

    private void PublicYesButtonCallback(string ST)
    {
        if (ST.Equals("Yes"))
        {
            //勾选了不出现提示
            UserDataManager.Instance.AlertFormClosTip = DateTime.Now.DayOfYear;
        }

        //Debug.Log("共用界面YES按钮回调");
        GameHttpNet.Instance.PostPlaceItem("100", UserDataManager.Instance.BagInfoContent.data.foodpack[index].shop_id.ToString(), ProcessPlaceItem);
    }
    private void PublicNoButtonCallBacke(string ST)
    {
        //Debug.Log("共用界面No按钮回调");

        if (ST.Equals("Yes"))
        {
            //勾选了不出现提示
            UserDataManager.Instance.AlertFormClosTip = DateTime.Now.DayOfYear;
        }
    }

    private void ProcessPlaceItem(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----ProcessPlaceFood---->" + result);

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {

                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                   
                    var tmpForm = CUIManager.Instance.GetForm<CatMainForm>(UIFormName.CatMain);
                    //CUIManager.Instance.OpenForm(UIFormName.CatMain);
                    tmpForm.catSceneView.RefreshFoodImg(UserDataManager.Instance.BagInfoContent.data.foodpack[index].shop_id.ToString());
                    UserDataManager.Instance.BagInfoContent.data.foodpack[index].count = 1;
                    var tmpTopForm = CUIManager.Instance.GetForm<CatTopMain>(UIFormName.CatTop);
                    if (tmpTopForm)
                    {
                        tmpTopForm.RefreshDiamondAndHeart(null, null, "100");
                    }
                    CloseUi(null);

                }
                else if (jo.code == 203)
                {
                    //这个食物消耗完了
                    EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), UiFormIndex);

                    AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
                    CUIManager.Instance.OpenForm(UIFormName.CatShop);

                }
                else
                {
                    //UITipsMgr.Instance.PopupTips(jo.msg, false);
                }
            }, null);
        }   
    }

    /// <summary>
    /// 设置引导点击的位置
    /// </summary>
    /// <param name="notice"></param>
    public void SetGuidPos(Notification notice)
    {
        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.PlaceFoodYes)
        {
            PreviousBtn.gameObject.SetActive(false);
            NextBtn.gameObject.SetActive(false);
            NoBtn.gameObject.SetActive(false);

            RectTransform YesbuttonRect = Yesbutton.transform.GetComponent<RectTransform>();
            YesbuttonRect.anchoredPosition3D = new Vector3(0, -276.58f,0);

            RectTransform maskRect = BackButton.transform.GetComponent<RectTransform>();
            float Hight = maskRect.rect.height / 2.0f;
            float Wight = maskRect.rect.width / 2.0f;


            float Posx = Wight + YesbuttonRect.anchoredPosition.x;
            float Posy= Hight+ YesbuttonRect.anchoredPosition.y+36;

            UserDataManager.Instance.GuidPos = new Vector3(Posx, Posy, 1);
            LOG.Info("得到食物放置确定按钮的坐标");
        }
    }
}

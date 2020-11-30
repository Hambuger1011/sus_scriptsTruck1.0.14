using CatMainFormClasse;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UGUI;
using DG.Tweening;
using UnityEngine.UI;

public class CatMainDown : CatClase
{
    private GameObject AnimalButton, ShopButton, DecorationsButton, StoryButton, MyChartButton;
    private GameObject DownTuchAra, GiftFromAnimalButton;
    private Transform Buttons;

    private CatMainForm catForm;
    private bool isMoving = false;
    private int GiftReturnNumber;
    private Text AnimalText, ShopText, DecorationsText, StoryText, MyChartText, GiftFromAnimalText;
    private bool mShapeOpen;
    public override void Bind(CatMainForm catmainform)
    {
        catmainform.CatMainDown = this;
        catForm = catmainform;

        GiftReturnNumber = 0;

        AnimalButton = transform.Find("Buttons/ButtonsGrourd/Animal").gameObject;
        AnimalText = transform.Find("Buttons/ButtonsGrourd/Animal/Text").GetComponent<Text>();
        AnimalText.text = GameDataMgr.Instance.table.GetLocalizationById(94);

        ShopButton = transform.Find("Buttons/ButtonsGrourd/Shop").gameObject;
        ShopText = transform.Find("Buttons/ButtonsGrourd/Shop/Text").GetComponent<Text>();
        ShopText.text = GameDataMgr.Instance.table.GetLocalizationById(95);

        DecorationsButton = transform.Find("Buttons/ButtonsGrourd/Decorations").gameObject;
        DecorationsText = transform.Find("Buttons/ButtonsGrourd/Decorations/Text").GetComponent<Text>();
        DecorationsText.text = GameDataMgr.Instance.table.GetLocalizationById(96);

        StoryButton = transform.Find("Buttons/ButtonsGrourd/Story").gameObject;
        StoryText = transform.Find("Buttons/ButtonsGrourd/Story/Text").GetComponent<Text>();
        StoryText.text = GameDataMgr.Instance.table.GetLocalizationById(97);

        MyChartButton = transform.Find("Buttons/ButtonsGrourd/MyChart").gameObject;
        MyChartText = transform.Find("Buttons/ButtonsGrourd/MyChart/Text").GetComponent<Text>();
        MyChartText.text = GameDataMgr.Instance.table.GetLocalizationById(98);

        DownTuchAra = transform.Find("DownTuchAra").gameObject;
        Buttons = transform.Find("Buttons");

        GiftFromAnimalButton = catmainform.GiftFromAnimalButton;
        GiftFromAnimalText = GiftFromAnimalButton.transform.Find("Text").GetComponent<Text>();
        //GiftFromAnimalText.text = GameDataMgr.Instance.table.GetLocalizationById(93);

      

        UIEventListener.AddOnClickListener(AnimalButton, AnimalButtonOnclicke);
        UIEventListener.AddOnClickListener(ShopButton, ShopButtonOnclicke);
        UIEventListener.AddOnClickListener(DecorationsButton, DecorationsButtonOnclicke);
        UIEventListener.AddOnClickListener(StoryButton, StoryButtonOnclicke);
        UIEventListener.AddOnClickListener(MyChartButton, MyChartButtonOnclicke);
        UIEventListener.AddOnClickListener(DownTuchAra, DownTuchAraButtonOnclicke);
        UIEventListener.AddOnClickListener(GiftFromAnimalButton,GiftFromAnimalButtonOnclicke);

        EventDispatcher.AddMessageListener(EventEnum.OnGiftReturnNumberStatistics,OnGiftReturnNumberStatistics);

        EventDispatcher.AddMessageListener(EventEnum.SetGuidPos, SetGuidPos);

        EventDispatcher.AddMessageListener(EventEnum.CatGuidRepair, CatGuidRepair);

        isMoving = false;

        ClockeTip();
        DwonButtonShow();

    }

    private void ClockeTip()
    {
        //UINetLoadingMgr.Instance.Show();
    }
    public void DwonButtonShow()
    {
        if (UserDataManager.Instance.SceneInfo != null)
        {
            isMoving = false;
            //Debug.Log("dd:"+ UserDataManager.Instance.SceneInfo.data.usermoney.usergift_status);
            if (UserDataManager.Instance.SceneInfo.data.usermoney.usergift_status == 1)
            {
                //有宠物馈赠
                
                //GiftFromAnimalButton.SetActive(false);
                //GiftFromAnimalButton.transform.DOLocalMoveX(600, 0.1f);

                GiftFromAnimalButton.SetActive(true);
                //GiftFromAnimalButton.transform.DOLocalMoveX(298, 0.5f).OnComplete(()=> {
                    
                //});
               
                //Buttons.DOLocalMoveY(-200, 0.1f).SetEase(Ease.OutCirc);
            }
            else
            {
                //没有宠物馈赠
                GiftFromAnimalButton.SetActive(false);
                //GiftFromAnimalButton.transform.DOLocalMoveX(600,0.2f);
              
            }

            Buttons.DOLocalMoveY(-200, 0.1f);
            DownTuchAraButtonOnclicke(null);
        }
    }

    /// <summary>
    /// 有故事就把故事按钮显示出来，没有就隐藏，1有 0没有
    /// </summary>
    /// <param name="num"></param>
    public void IsHadStory(int num)
    {
        if (StoryButton == null) return;

        if (num==1)
        {
            StoryButton.SetActive(true);
        }
        else
        {
            StoryButton.SetActive(false);
        }
    }
    /// <summary>
    /// 这个是宠物回赠数量剩余的统计
    /// </summary>
    /// <param name="notification"></param>
    private void OnGiftReturnNumberStatistics(Notification notification)
    {
        int AddNum = (int)notification.Data;
        if (UserDataManager.Instance.Getpetgiftinfo!=null)
        {
            GiftReturnNumber+= AddNum;

            LOG.Info("回赠领取次数计数："+GiftReturnNumber);
            if (GiftReturnNumber>= UserDataManager.Instance.Getpetgiftinfo.data.feedback_count)
            {
                //当领取回赠数量大于等于回赠数量的时候，判断回赠领取完了，把主界面的回赠按钮隐藏
                UserDataManager.Instance.SceneInfo.data.usermoney.usergift_status = 0;
               
                //没有宠物馈赠
                if (GiftFromAnimalButton != null)
                {
                    GiftFromAnimalButton.SetActive(false);
                    //GiftFromAnimalButton.transform.DOLocalMoveX(600, 0.2f);
                    DownTuchAraButtonOnclicke(null);
                }
            }
        }

    }

    private void DownTuchAraButtonOnclicke(PointerEventData data)
    {

        if (UserDataManager.Instance.isFirstCatEnt&& UserDataManager.Instance.GuidStupNum < (int)CatGuidEnum.ShopOnClicke|| UserDataManager.Instance.GuidFirstStupNum == (int)CatGuidEnum.GetGiftGuid)
        {
            //正在进行猫的引导 ,而且没有道引导商店的时候，下面按钮栏不出现
            return;

        }
        if (UserDataManager.Instance.InPlaceCatThings != 0) return;
        if (!isMoving)
        {
            isMoving = true;         
            //GiftFromAnimalButton.SetActive(false);
            //GiftFromAnimalButton.transform.DOLocalMoveX(600, 0.2f);
          
            Buttons.DOLocalMoveY(0, 0.8f).SetEase(Ease.OutCirc).OnComplete(()=> {

                if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.ShopOnClicke)
                {
                    //引导点击商店，出现引导界面
                    EventDispatcher.Dispatch(EventEnum.OpenCatGuid);//打开引导界面
                }else if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.DecorationsButtonOn)
                {

                }
                else
                {
                    Invoke("ButtonsMoveDown", 4);
                }               
            });
        }

       
    }

    private void ButtonsMoveDown()
    {
        CancelInvoke("ButtonsMoveDown");      
        Buttons.DOLocalMoveY(-200, 0.8f).SetEase(Ease.OutCirc).OnComplete(() => {
            isMoving = false;
       
            if (UserDataManager.Instance.SceneInfo != null)
            {
                if (UserDataManager.Instance.SceneInfo.data.usermoney.usergift_status == 1)
                {
                    //有宠物馈赠
                    GiftFromAnimalButton.SetActive(true);
                    //GiftFromAnimalButton.transform.DOLocalMoveX(298, 0.5f).OnComplete(() => {
                        
                    //});
                }
                else
                {
                    //没有宠物馈赠
                    GiftFromAnimalButton.SetActive(false);
                    //GiftFromAnimalButton.transform.DOLocalMoveX(600, 0.2f);
                }
            }           
        });
    }

    private void GiftFromAnimalButtonOnclicke(PointerEventData data)
    {
        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.GetGiftGuid)
        {
            EventDispatcher.Dispatch(EventEnum.CatGuidUiClose);//关闭引导界面
        }
        if (UserDataManager.Instance.InPlaceCatThings != 0) return;
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CUIManager.Instance.OpenForm(UIFormName.CatGiftFromAnimalForm);

    }

    public void AnimalButtonOnclicke(PointerEventData data)
    {
        if (UserDataManager.Instance.InPlaceCatThings != 0) return;
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CUIManager.Instance.OpenForm(UIFormName.CatAnimal);

    }

    public void ShopButtonOnclicke(PointerEventData data)
    {
        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.ShopOnClicke)
        {            
            EventDispatcher.Dispatch(EventEnum.CatGuidUiClose);//关闭引导界面
        }

        if (UserDataManager.Instance.InPlaceCatThings != 0) return;
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CUIManager.Instance.OpenForm(UIFormName.CatShop);

    }

    private void CatGuidRepair(Notification notification)
    {
        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.ShopBuyHuangyuandian|| UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.HuangyuandianYesOnclick)
        {
            if (UserDataManager.Instance.InPlaceCatThings != 0) return;
            AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
            CUIManager.Instance.OpenForm(UIFormName.CatShop);
        }else if (UserDataManager.Instance.GuidFirstStupNum == (int)CatGuidEnum.GetGiftGuid)
        {
            GiftFromAnimalButton.SetActive(true);
        }
    }

    public void DecorationsButtonOnclicke(PointerEventData data)
    {
        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.DecorationsButtonOn)
        {
            EventDispatcher.Dispatch(EventEnum.CatGuidUiClose);//关闭引导界面
        }

        if (UserDataManager.Instance.InPlaceCatThings != 0) return;
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CUIManager.Instance.OpenForm(UIFormName.CatDecorations);

    }
    public void StoryButtonOnclicke(PointerEventData data)
    {
        if (UserDataManager.Instance.InPlaceCatThings != 0) return;
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CUIManager.Instance.OpenForm(UIFormName.CatStory);

    }
    public void MyChartButtonOnclicke(PointerEventData data)
    {
        if (UserDataManager.Instance.InPlaceCatThings != 0) return;
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CUIManager.Instance.OpenForm(UIFormName.MyChart);
    }

    public override void CloseUi()
    {
        UIEventListener.RemoveOnClickListener(AnimalButton, AnimalButtonOnclicke);
        UIEventListener.RemoveOnClickListener(ShopButton, ShopButtonOnclicke);
        UIEventListener.RemoveOnClickListener(DecorationsButton, DecorationsButtonOnclicke);
        UIEventListener.RemoveOnClickListener(StoryButton, StoryButtonOnclicke);
        UIEventListener.RemoveOnClickListener(MyChartButton, MyChartButtonOnclicke);
        UIEventListener.RemoveOnClickListener(DownTuchAra, DownTuchAraButtonOnclicke);
        UIEventListener.RemoveOnClickListener(GiftFromAnimalButton, GiftFromAnimalButtonOnclicke);

        EventDispatcher.RemoveMessageListener(EventEnum.OnGiftReturnNumberStatistics, OnGiftReturnNumberStatistics);

        EventDispatcher.RemoveMessageListener(EventEnum.SetGuidPos, SetGuidPos);

        EventDispatcher.RemoveMessageListener(EventEnum.CatGuidRepair, CatGuidRepair);

    }


    /// <summary>
    /// 设置引导点击的位置
    /// </summary>
    /// <param name="notice"></param>
    public void SetGuidPos(Notification notice)
    {
        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.ShopOnClicke)
        {
            RectTransform ShopButtonRect = ShopButton.GetComponent<RectTransform>();

            float Posx = ShopButtonRect.anchoredPosition.x;
            float Posy = ShopButtonRect.anchoredPosition.y;
            UserDataManager.Instance.GuidPos = new Vector3(Posx, -Posy+34, 1);
            //LOG.Info("得到食物放置确定按钮的坐标");
        }

        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.DecorationsButtonOn)
        {
            RectTransform DecorationsButtonRect = DecorationsButton.GetComponent<RectTransform>();

            float Posx = DecorationsButtonRect.anchoredPosition.x;
            float Posy = DecorationsButtonRect.anchoredPosition.y;
            UserDataManager.Instance.GuidPos = new Vector3(Posx, -Posy + 34, 1);
            LOG.Info("得到装饰物按钮的坐标");
        }
    }

}

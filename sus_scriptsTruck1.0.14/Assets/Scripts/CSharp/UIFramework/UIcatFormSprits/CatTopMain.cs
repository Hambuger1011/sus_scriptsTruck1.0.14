using CatMainFormClasse;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UGUI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CatTopMain : BaseUIForm
{

    private Text DiamondNumber_text, HeartNumber_text, tilText;
    private GameObject Diamonds, hardButton;
    private Image HardImage, DiamondImage;
    private RectTransform BG;
    

    private void Awake()
    {
        hardButton = transform.Find("Top/BG/hardButton/Button").gameObject;
        HeartNumber_text = transform.Find("Top/BG/hardButton/HardNumber").gameObject.GetComponent<Text>();
        DiamondNumber_text = transform.Find("Top/BG/Diamonds/HardNumber").gameObject.GetComponent<Text>();
        
        tilText = transform.Find("Top/BG/tilText").gameObject.GetComponent<Text>();
        Diamonds = transform.Find("Top/BG/Diamonds/Button").gameObject;
       
        HardImage = transform.Find("Top/BG/hardButton/HardImage").GetComponent<Image>();
        DiamondImage = transform.Find("Top/BG/Diamonds/DiamondImage").GetComponent<Image>();
        BG= transform.Find("Top/BG").GetComponent<RectTransform>();
    }
    public override void OnOpen()
    {
        base.OnOpen();
       
        string dia = UserDataManager.Instance.SceneInfo.data.usermoney.diamond.ToString();
        string love = UserDataManager.Instance.SceneInfo.data.usermoney.love.ToString();
        string food = UserDataManager.Instance.SceneInfo.data.usermoney.otherfood.ToString();

        
        DiamondNumber_text.text = dia.ToString();
        HeartNumber_text.text = love.ToString();

        

        RefreshDiamondAndHeart(dia,love,food);

        addMessageListener(EventEnum.OnDiamondNumChange, OnDiamondNumChange);
        UIEventListener.AddOnClickListener(Diamonds, DiamondsButtonOnclicke);
      
        UIEventListener.AddOnClickListener(hardButton, HardButtonOnclicke);

        if (ResolutionAdapter.HasUnSafeArea)
        {
            var scale = this.myForm.GetScale();
            var safeArea = ResolutionAdapter.GetSafeArea();
            var offset = this.myForm.Pixel2View(safeArea.position);
            var offerH = offset.y;

            BG.sizeDelta = new Vector2(750, -59 + offerH);
          
        }

    }
    
    public void RefreshTitle(string title)
    {
        if (!string.IsNullOrEmpty(title))
        {
            tilText.text = title;
        }
    }
    public void RefreshDiamondAndHeart(string dia, string heart, string food)
    {
        //Debug.Log("111");
        if (!string.IsNullOrEmpty(dia))
        {
            //DiamondNumber_text.text = dia;

            int newNum = int.Parse(dia);
            int oldNum = int.Parse(DiamondNumber_text.text);
            if (newNum >= oldNum)
            {
                UITween.AddDiamond(DiamondImage, DiamondNumber_text, oldNum, newNum);
            }
            else
            {
                //DiamondNumber_text.text = (newNum).ToString();

                DOTween.To(() => oldNum, (value) => { DiamondNumber_text.text = value.ToString(); }, newNum, 2).OnComplete(() => {

                });
            }


        }
        if (!string.IsNullOrEmpty(heart))
        {
            //HeartNumber_text.text = heart;

            int newNum = int.Parse(heart);
            int oldNum = int.Parse(HeartNumber_text.text);
            if (newNum >= oldNum)
            {
                UITween.AddDiamond(HardImage, HeartNumber_text, oldNum, newNum);
            }
            else
            {
                //HeartNumber_text.text = (newNum).ToString();

                DOTween.To(() => oldNum, (value) => { HeartNumber_text.text = value.ToString(); }, newNum, 2).OnComplete(() => {

                });
            }
        }
        if (!string.IsNullOrEmpty(food))
        {
            


            int FoodNumber = int.Parse(food);
            EventDispatcher.Dispatch(EventEnum.FoodNumberShow.ToString(), FoodNumber);

           
            int num = int.Parse(food);
            if (num <= 0)
            {
                UserDataManager.Instance.CatTimeDwonOpen = true;
            }
            else
            {
                UserDataManager.Instance.CatTimeDwonOpen = false;
            }
        }
    }
    public void RefreshDiamond(int type, int Number)
    {
        //Debug.Log("type+"+ type+ "--Number:"+ Number);
        if (type == 1)
        {
            if (!string.IsNullOrEmpty(Number.ToString()))
            {
                //HeartNumber_text.text = Number.ToString();

                int newNum = Number;
                int oldNum = int.Parse(HeartNumber_text.text);
                if (newNum >= oldNum)
                {
                    UITween.AddDiamond(HardImage, HeartNumber_text, oldNum, newNum);
                }
                else
                {
                    //HeartNumber_text.text = (newNum).ToString();

                    DOTween.To(() => oldNum, (value) => { HeartNumber_text.text = value.ToString(); }, newNum, 2).OnComplete(() => {
                        
                    });

                }
            }
        }
        else if (type == 2)
        {
            if (!string.IsNullOrEmpty(Number.ToString()))
            {
                //DiamondNumber_text.text = Number.ToString();
               
                int newNum = Number;
                int oldNum = int.Parse(DiamondNumber_text.text);
                if (newNum >= oldNum)
                {
                    UITween.AddDiamond(DiamondImage, DiamondNumber_text, oldNum, newNum);
                }
                else
                {
                    //DiamondNumber_text.text = (newNum).ToString();

                    DOTween.To(() => oldNum, (value) => { DiamondNumber_text.text = value.ToString(); }, newNum, 2).OnComplete(() => {

                    });
                }

                CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop).CatDiamondNumChange(Number);
                //UserDataManager.Instance.ResetMoney(2, Number);

            }
        }
        else
        {
            if (!string.IsNullOrEmpty(Number.ToString()))
            {
               
                EventDispatcher.Dispatch(EventEnum.FoodNumberShow.ToString(), Number);

               
                if (Number<=0)
                {
                    UserDataManager.Instance.CatTimeDwonOpen = true;
                }else
                {
                    UserDataManager.Instance.CatTimeDwonOpen = false;
                }
            }
        }
    }
    public override void OnClose()
    {
        base.OnClose();
       
        UIEventListener.RemoveOnClickListener(Diamonds, DiamondsButtonOnclicke);
      
        UIEventListener.RemoveOnClickListener(hardButton, HardButtonOnclicke);
    }

    private void HardButtonOnclicke(PointerEventData data)
    {
        CUIManager.Instance.OpenForm(UIFormName.CatDiamondExchange);
    }
    private void DiamondsButtonOnclicke(PointerEventData data)
    {
        LOG.Info("打开分档推荐界面");
        CUIManager.Instance.OpenForm(UIFormName.ChargeTipsForm);
        ChargeTipsForm tipForm = CUIManager.Instance.GetForm<ChargeTipsForm>(UIFormName.ChargeTipsForm);

        //CUIManager.Instance.OpenForm(UIFormName.NewChargeTips);
        //NewChargeTips tipForm = CUIManager.Instance.GetForm<NewChargeTips>(UIFormName.NewChargeTips);


        if (tipForm != null)
        {
            tipForm.Init(2, 1, 1 * 0.99f);
            //tipForm.CatTopMainChange();
        }         
        return;
    }
  
    private void OnDiamondNumChange(Notification notification)
    {
        //DiamondNumber_text.text = ((int)notification.Data).ToString();

        int newNum = (int)notification.Data;
        int oldNum = int.Parse(DiamondNumber_text.text);
        if (newNum >= oldNum)
        {
            UITween.AddDiamond(DiamondImage, DiamondNumber_text, oldNum, newNum);
        }
        else
        {
            DiamondNumber_text.text = (newNum).ToString();
        }
    }
}

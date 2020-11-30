using CatMainFormClasse;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UGUI;
using UnityEngine;
using UnityEngine.UI;

public class CatTop : BaseUIForm {

    private Text DiamondNumber_text, HeartNumber_text, FoodNum, tilText;
  
    private void Awake()
    {
        HeartNumber_text = transform.Find("Top/BG/hardButton/HardNumber").gameObject.GetComponent<Text>();
        DiamondNumber_text = transform.Find("Top/BG/Diamonds/HardNumber").gameObject.GetComponent<Text>();
        FoodNum = transform.Find("Top/BG/CatFood/CatFoodNumber").gameObject.GetComponent<Text>();
        tilText = transform.Find("Top/BG/tilText").gameObject.GetComponent<Text>();
    }
    public override void OnOpen()
    {
        base.OnOpen();         
    }

    public void RefreshTitle(string title)
    {
        if (!string.IsNullOrEmpty(title))
        {
            tilText.text = title;
        }
    }
    public void RefreshDiamondAndHeart(string dia, string heart,string food)
    {
        if (!string.IsNullOrEmpty(dia))
        {
            DiamondNumber_text.GetComponent<Text>().text = dia;
        }
        if (!string.IsNullOrEmpty(heart))
        {
            HeartNumber_text.GetComponent<Text>().text = heart;
        }
        if (!string.IsNullOrEmpty(food))
        {
            FoodNum.GetComponent<Text>().text = food;

            UserDataManager.Instance.FoodNum =int.Parse(food);
        }
    }

  
    public override void OnClose()
    {
        base.OnClose();
       
    }

}

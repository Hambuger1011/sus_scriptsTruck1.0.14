using AB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 付费子项
/// </summary>
public class ChargeItemForm : MonoBehaviour 
{
    public Image ItemBg;
    public Image ItemIcon;
    public Text NumText;
    public Text PriceText;

    public int Index;


    public void InitInfo(int vType,int vIndex)
    {
        ItemBg.gameObject.transform.localPosition = new Vector3(700, 0, 0);
        Index = vIndex;

        string itemIconPath = string.Empty;
        string itemBgPath = string.Empty;

        if (vType  == 1)
        {
            itemIconPath = "ChargeMoneyForm/key";
            itemBgPath = "ChargeMoneyForm/bg_iap_key";
        }
        else if(vType == 2)
        {
            itemIconPath = "ChargeMoneyForm/dimon";
            itemBgPath = "ChargeMoneyForm/bg_iap_dimon";
        }

        itemIconPath = itemIconPath + vIndex;

        ItemBg.sprite = ResourceManager.Instance.GetUISprite(itemBgPath);
        ItemIcon.sprite = ResourceManager.Instance.GetUISprite(itemIconPath);
        ItemIcon.SetNativeSize();
        NumText.text = "X "+(5 * vIndex).ToString();
        PriceText.text = "$"+(0.99 * vIndex).ToString();
        GameMove();
    }

    public int Nubers = 0;
    void OnEnable()
    {
       
        ItemBg.gameObject.transform.localPosition=new Vector3(700,0,0);
    }

    private float times = 0.2f;
    public void GetNubers(int value)
    {
        Nubers = value;
        GameMove();
    }

    private void GameMove()
    {
        ItemBg.gameObject.transform.DOLocalMoveX(0, times * (Nubers + 1));
    }
}

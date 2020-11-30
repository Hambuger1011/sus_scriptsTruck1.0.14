using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmailGiftsItem : MonoBehaviour {

    public Image GiftsIcon;
    public Text Number;
    public GameObject ItemImage;

    private const string keyIcon = "EmailForm/bg_knea_03";
    private const string DiamondIcon = "EmailForm/bg_msje_03";
    private const string Ticket= "EmailForm/bg_iconb_25";
    /// <summary>
    /// 1 代表钥匙 2 代表钻石
    /// </summary>
    /// <param name="type"></param>
    /// <param name="price"></param>
    public void Init(int type,int price)
    {
        ItemImage.SetActive(true);
        if (type==1)
        {
            GiftsIcon.sprite=ResourceManager.Instance.GetUISprite(keyIcon);
            Number.text = "x" + price.ToString();       
        }
        else if(type==2)
        {
            GiftsIcon.sprite = ResourceManager.Instance.GetUISprite(DiamondIcon);
            Number.text = "x" + price.ToString();
        }
        else if(type == 3)
        {
            GiftsIcon.sprite = ResourceManager.Instance.GetUISprite(Ticket);
            Number.text = "x" + price.ToString();
        }else
        {
            ItemImage.SetActive(false);
        }    
    }
}

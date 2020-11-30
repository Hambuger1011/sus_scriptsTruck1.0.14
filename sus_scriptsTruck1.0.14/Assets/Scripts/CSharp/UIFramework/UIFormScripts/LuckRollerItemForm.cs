using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 转盘子项
/// </summary>
public class LuckRollerItemForm : MonoBehaviour 
{
    public Image AwardIcon;
    public Text DescText;


    private TicketItemInfo mItemInfo;

    public void Init(TicketItemInfo vItemInfo)
    {
        mItemInfo = vItemInfo;
        //Debug.Log(mItemInfo.pid);
        DescText.text = mItemInfo.pname;
        AwardIcon.gameObject.SetActive(false);
        string ImagePath = string.Empty;
        if(mItemInfo.price_type == 1)
        {
            if(mItemInfo.price_count <= 2)
                ImagePath = "LuckRollerForm/bg_icona_01";
            else if (mItemInfo.price_count > 2 && mItemInfo.price_count <= 3)
                ImagePath = "LuckRollerForm/bg_icona_08";
            else if (mItemInfo.price_count > 3 && mItemInfo.price_count <= 30)
                ImagePath = "LuckRollerForm/bg_icona_09";
            else
                ImagePath = "LuckRollerForm/bg_icona_02";
        }
        else if (mItemInfo.price_type == 2)
        {
            if (mItemInfo.price_count <= 10)
                ImagePath = "LuckRollerForm/bg_icona_03";
            else if (mItemInfo.price_count > 10 && mItemInfo.price_count <= 60)
                ImagePath = "LuckRollerForm/bg_icona_04";
            else if (mItemInfo.price_count > 60 && mItemInfo.price_count <= 100)
                ImagePath = "LuckRollerForm/bg_icona_05";
            else
                ImagePath = "LuckRollerForm/bg_icona_06";
        }
        else if (mItemInfo.price_type == 3)
        {
            ImagePath = "LuckRollerForm/bg_icona_07";
        }

        if (!string.IsNullOrEmpty(ImagePath))
        {
            AwardIcon.sprite = ResourceManager.Instance.GetUISprite(ImagePath);
            AwardIcon.gameObject.SetActive(true);
        }

    }
	
}

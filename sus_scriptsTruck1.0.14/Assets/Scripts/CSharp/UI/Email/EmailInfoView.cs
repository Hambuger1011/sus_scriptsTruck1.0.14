using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmailInfoView
{
    private EmailInfoForm EmailInfoForm;
    private Text Tilte, Time,Conten;
    private Transform transform;
    private EmailItemInfo EmailItemInfo;
    private GameObject GitBg;
    private GameObject KeyItem, DimandItem;
    private Text KeyItemText, DimandItemText;
    private Image CollectButton;
    private ScrollRect SRTransform;


    public EmailInfoView(EmailInfoForm EmailInfoForm)
    {
        this.EmailInfoForm = EmailInfoForm;
        transform = EmailInfoForm.transform;
        this.EmailItemInfo = EmailInfoForm.EmailItemInfo;
        FindGameObject();

        ShowTile();
        ShowTime();
        ShowConten();
        ShowGift();
    }

    private void FindGameObject()
    {
        Tilte = transform.Find("Bg/ScrollView/Viewport/Content/TileBg/Tilte").GetComponent<Text>();
        Time = transform.Find("Bg/ScrollView/Viewport/Content/TileBg/Time").GetComponent<Text>();
        Conten= transform.Find("Bg/ScrollView/Viewport/Content/Content").GetComponent<Text>();
        GitBg = transform.Find("Bg/GitBg").gameObject;
        KeyItem = transform.Find("Bg/GitBg/GitItemBg/KeyItem").gameObject;
        DimandItem = transform.Find("Bg/GitBg/GitItemBg/DimandItem").gameObject;
        KeyItemText = transform.Find("Bg/GitBg/GitItemBg/KeyItem/Text").GetComponent<Text>();
        DimandItemText = transform.Find("Bg/GitBg/GitItemBg/DimandItem/Text").GetComponent<Text>();
        CollectButton= transform.Find("Bg/GitBg/Button").GetComponent<Image>();
        SRTransform = transform.Find("Bg/ScrollView").GetComponent<ScrollRect>();
    }


    public void ShowTile()
    {
        if (Tilte != null)
            Tilte.text = EmailItemInfo.title.ToString();
    }
    public void ShowTime()
    {
        if (Time != null)
            Time.text =EmailItemInfo.createtime.ToString();
    }

    public void ShowConten()
    {
        if (Conten != null)
        {
            string contentStr = EmailItemInfo.content.Replace("\\n", "\n");
            Conten.text = contentStr;
        }
    }

    public void ShowGift()
    {
        if (GitBg!=null)
        {
            KeyItem.SetActive(false);
            DimandItem.SetActive(false);

            //offsetMin   是vector2(left,bottom);
            //offsetMax 是vector2(right, top);

            if (EmailItemInfo.isprice == 1)
            {
                //是奖励而且还是未领取状态
                GitBg.SetActive(true);
                SRTransform.rectTransform().offsetMin = new Vector2(0, 240);
            }
            else
            {
                GitBg.SetActive(false);
                SRTransform.rectTransform().offsetMin = new Vector2(0, 0);
            }

            if (EmailItemInfo.price_bkey != 0)
            {
                //有钥匙奖励
                KeyItem.SetActive(true);
                KeyItemText.text = "x"+EmailItemInfo.price_bkey.ToString();
            }

            if (EmailItemInfo.price_diamond != 0)
            {
                //有钻石奖励             
                DimandItem.SetActive(true);
                DimandItemText.text = "x" + EmailItemInfo.price_diamond.ToString();
            }

            if (EmailItemInfo.price_status == 0)
            {
                //奖励还没有领取
                CollectButton.sprite = ResourceManager.Instance.GetUISprite("EmailForm/btn_default");
            }
            else
            {
                //奖励已经领取
                CollectButton.sprite = ResourceManager.Instance.GetUISprite("EmailForm/btn_focus2");
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatWelcomeBackeItem : MonoBehaviour {

    private bool First = false;
    private Image top;
    private Text IntimacyFirst;
    private Text IntimacyLast;
    private Text ContenText;
    private GameObject Award;
    private Image TypeSpriet;
    private Text NumText;

    public void Init(adoptchange adoptchange)
    {
        if (!First)
        {
            First = true;

            top = transform.Find("top").GetComponent<Image>();
            IntimacyFirst = transform.Find("IntimacyPr/IntimacyFirst").GetComponent<Text>();
            IntimacyLast = transform.Find("IntimacyPr/IntimacyLast").GetComponent<Text>();
            ContenText = transform.Find("StatePr/ContenText").GetComponent<Text>();
            Award = transform.Find("StatePr/Award").gameObject;
            TypeSpriet = transform.Find("StatePr/Award/TypeSpriet").GetComponent<Image>();
            NumText = transform.Find("StatePr/Award/NumText").GetComponent<Text>();
        }

        string CatNameNumber = "cat" + adoptchange.pid;
        top.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/" + CatNameNumber);
        IntimacyFirst.text = adoptchange.intimacy.ToString();
        IntimacyLast.text = adoptchange.intimacy_new.ToString();

        if (adoptchange.change_status==3)
        {
            //有收益
            ContenText.gameObject.SetActive(false);
            Award.SetActive(true);

            if (adoptchange.love>0)
            {
                TypeSpriet.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/icon");
                NumText.text = adoptchange.love.ToString();
            }

            if (adoptchange.diamond > 0)
            {
                TypeSpriet.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/icon1");
                NumText.text = adoptchange.diamond.ToString();
            }
        }
        else
        {
            ContenText.gameObject.SetActive(true);
            Award.SetActive(false);
            if (adoptchange.change_status == 1)
            {
                //宠物丢失
                ContenText.text = GameDataMgr.Instance.table.GetLocalizationById(246);
            }
            else if (adoptchange.change_status == 2)
            {
                //新故事
                ContenText.text = GameDataMgr.Instance.table.GetLocalizationById(247); /*"新故事";*/
            }
            else if (adoptchange.change_status == 4)
            {
                //饥饿的
                ContenText.text = GameDataMgr.Instance.table.GetLocalizationById(248); /*"饥饿的";*/
            }
            else
            {
                //其他
                ContenText.text = "";
            }
        }
    }

    public void DisPost()
    {
        top.sprite = null;
        TypeSpriet.sprite = null;

        Destroy(gameObject);
    }
}

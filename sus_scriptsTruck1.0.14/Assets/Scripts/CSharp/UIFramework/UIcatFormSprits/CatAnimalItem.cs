using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UGUI;

public class CatAnimalItem : MonoBehaviour {

    private Text NumberText, Name, TimeText;
    private Image AnimalItemSprite;
    private Transform StarPos,Start;

    private List<GameObject> StartList;
    private bool isFirs = false;
    private petarr tem;
    private GameObject RedTip;

    public void Inite(petarr tem,int Num)
    {
        if (!isFirs)
        {
            
            isFirs = true;
            NumberText = transform.Find("NumberText").GetComponent<Text>();
            Name = transform.Find("Name").GetComponent<Text>();
            TimeText = transform.Find("TimeText").GetComponent<Text>();
            AnimalItemSprite = transform.Find("AnimalItemSprite").GetComponent<Image>();
            StarPos = transform.Find("StartPos");
            Start= transform.Find("StartPos/Start");
            //RedTip = transform.Find("RedTip").gameObject;

            UIEventListener.AddOnClickListener(gameObject, GameButtonOnclicke);

            if (StartList == null)
            {
                StartList = new List<GameObject>();
            }
            StartList.Clear();

            for (int i = 0; i < 5; i++)
            {
                GameObject go = Instantiate(Start.gameObject);
                go.transform.SetParent(StarPos);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
                go.SetActive(true);
                StartList.Add(go);             
            }
        }
        //RedTip.SetActive(false);
        this.tem = tem;

        NumberText.text = Num.ToString();
        Name.text = tem.pet_name.ToString();
        TimeText.text = tem.lasttime.ToString();
        if (tem.lockstatus==0)
        {
            
            //这个动物还没有解锁
            AnimalItemSprite.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/catbase");
            Name.text = "???";
            for (int i=0;i< StartList.Count; i++)
            {
                StartList[i].GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/icon_smg3");               
            }
        }
        else
        {
            //这个动物已经解锁了

            if (tem.isfit==1)
            {
                //已经达到收养的条件
                //RedTip.SetActive(true);
            }
            
            AnimalItemSprite.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/cat" + tem.id);

           
            for (int i = 0; i < StartList.Count; i++)
            {            
                if (tem.level > i)
                {
                    StartList[i].GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/icon_smg2");
                }
                else
                {
                    StartList[i].GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/icon_smg1_09");
                }
            }
        }

        AnimalItemSprite.gameObject.SetActive(true);
    }

    public void GameButtonOnclicke(PointerEventData data)
    {
        if (tem.lockstatus == 1)
        {
            //显示锚的属性
            AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
            CUIManager.Instance.OpenForm(UIFormName.CatAnimalAttribute);
            CUIManager.Instance.GetForm<CatAnimalAttributeForm>(UIFormName.CatAnimalAttribute).Inite(tem.id, 2);

        }

    }
    private void CatCall(string ST)
    {

    }
    private void DestroyStart()
    {
        if (StartList.Count>0)
        {
            for (int i = 0; i < StartList.Count; i++)
            {
                StartList[i].GetComponent<Image>().sprite = null;
                Destroy(StartList[i]);
            }
        }
    }
    /// <summary>
    /// 调用这个方法移除物体和释放内存
    /// </summary>
    public void Dispote()
    {
        AnimalItemSprite.sprite = null;

        if (tem.lockstatus == 1)
        {
            UIEventListener.RemoveOnClickListener(gameObject, GameButtonOnclicke);

        }

        DestroyStart();

        Destroy(gameObject);
    }
}

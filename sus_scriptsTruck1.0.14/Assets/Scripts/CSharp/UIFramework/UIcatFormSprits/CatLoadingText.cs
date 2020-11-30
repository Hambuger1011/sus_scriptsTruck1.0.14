using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatLoadingText : MonoBehaviour {

    private ArrayList myArraylist;
    private int shu = 0, LastNumber = -1;
    private Text MyText;
    private string sts;
    private bool isfirst = false;
    void OnEnable()
    {
        isfirst = true;
        MyText = transform.GetComponent<Text>();

        MyText.text = "";
        AddMyText();
        InvokeRepeating("InPutTexte", 0, 4);
    }

    void OnDisable()
    {
        //Debug.Log("清理");
        CancelInvoke("InPutTexte");
    }
    private void InPutTexte()
    {
        shu = Random.Range(0, myArraylist.Count);

        if (shu == LastNumber)
        {
            InPutTexte();
        }
        else
        {
            LastNumber = shu;
            sts = (string)myArraylist[shu];

            if (isfirst)
            {
                isfirst = false;
                MyText.color = new Color(1, 1, 1, 1);
            }
            else
            {
                MyText.color = new Color(1, 1, 1, 1);
            }

            //MyText.DOColor(new Color(1, 1, 1, 0), 1).OnComplete(TexteToTrue);
            //Debug.Log("输出的文字是" + sts);

            TexteToTrue();
        }
    }

    private void TexteToTrue()
    {
        //MyText.color = new Color(1, 1, 1, 0);
        //MyText.DOColor(new Color(1, 1, 1, 1), 1f);
        MyText.text = sts;

    }


    private void AddMyText()
    {
        myArraylist = new ArrayList();
        myArraylist.Add(GameDataMgr.Instance.table.GetLocalizationById(210)/*"亲密度达到10/40/80时，会开启猫咪故事"*/);
        myArraylist.Add(GameDataMgr.Instance.table.GetLocalizationById(211)/*"拜访次数达到15可尝试收养猫咪"*/);
        myArraylist.Add(GameDataMgr.Instance.table.GetLocalizationById(212)/*"猫咪会更容易被性格相仿的主人吸引"*/);
        myArraylist.Add(GameDataMgr.Instance.table.GetLocalizationById(213)/*"性格指数会决定你收养不同猫咪的成功率"*/);
        myArraylist.Add(GameDataMgr.Instance.table.GetLocalizationById(214)/*"品质越好的食物会吸引到更稀有的猫咪"*/);
        myArraylist.Add(GameDataMgr.Instance.table.GetLocalizationById(215)/*"只要食物充足收养的猫咪会一直为你带来爱心和钻石"*/);
        myArraylist.Add(GameDataMgr.Instance.table.GetLocalizationById(216)/*"不同的装饰物吸引到的猫咪也会不一样"*/);
        myArraylist.Add(GameDataMgr.Instance.table.GetLocalizationById(217)/*"只有5星的猫才会产出钻石"*/);

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UGUI;

public class CatWelcomBackStory : MonoBehaviour {

    private bool First = false;
    private Image CatTop;
    private Text Num;
    private GameObject Mask;
    private adoptchange adoptchange;

    public void Inite(adoptchange adoptchange)
    {
        if (!First)
        {
            First = true;
            CatTop = transform.Find("CatBg/CatTop").GetComponent<Image>();
            Num = transform.Find("Num").GetComponent<Text>();
            Mask = transform.Find("Mask").gameObject;
            UIEventListener.AddOnClickListener(Mask, MaskOnclicke);
        }
        this.adoptchange = adoptchange;
        string CatNameNumber = "cat" + adoptchange.pid;
        CatTop.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/" + CatNameNumber);
        Num.text = adoptchange.story_new.ToString();
    }

    private void MaskOnclicke(PointerEventData data)
    {
        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), (int)CatFormEnum.CAT_WELCOMEBACK);

        CUIManager.Instance.OpenForm(UIFormName.CatStoryDetails);
        CatStoryDetailsFrom catStoryDetailsFrom = CUIManager.Instance.GetForm<CatStoryDetailsFrom>(UIFormName.CatStoryDetails);
        if (catStoryDetailsFrom != null)
            catStoryDetailsFrom.Inite(adoptchange.pid, adoptchange.story_new, 3);
    }

    public void Disposte()
    {
        UIEventListener.RemoveOnClickListener(Mask, MaskOnclicke);
        CatTop.sprite = null;

        Destroy(gameObject);
    }
}

using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UGUI;

public class FAQItemSprite : MonoBehaviour {

   
    private Text QcontText;
    private string AcontTring;
    private t_BookFAQ t_BookFAQ;

    public void InitFAQItem(t_BookFAQ t_BookFAQ)
    {
        this.t_BookFAQ = t_BookFAQ;
        FindGameObject();

        string Qcontss = t_BookFAQ.Question.ToString();
        QcontText.text = Qcontss.Replace("\\n", "\n");
     
        string Acontss = t_BookFAQ.Answer.ToString(); 
        AcontTring = Acontss.Replace("\\n", "\n");

    }

    private void FindGameObject()
    {
        QcontText = transform.Find("Text").GetComponent<Text>();
        UIEventListener.AddOnClickListener(gameObject, SelfButtonOn);
    }

    private void SelfButtonOn(PointerEventData data)
    {
        Debug.Log("FAQ详细按钮点击了");

        CUIManager.Instance.OpenForm(UIFormName.FAQConten);
        CUIManager.Instance.GetForm<FAQConten>(UIFormName.FAQConten).Init(t_BookFAQ);
    }

    public void Disposte()
    {
        UIEventListener.RemoveOnClickListener(gameObject, SelfButtonOn);
        Destroy(gameObject);
    }
}

using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UGUI;

public class FAQConten : CUIComponent
{
    private Text Tilte, Conten;
    private t_BookFAQ t_BookFAQ;
    private GameObject Close;
    private GameObject ContactanButton;
    private RectTransform TopBg, ScrollViewRect;
    public void Init(t_BookFAQ t_BookFAQ)
    {
        this.t_BookFAQ = t_BookFAQ;
        FindGameObject();
        Addlistent();
        ShowContent();

        //【屏幕适配】
        float offect = XLuaHelper.UnSafeAreaNotFit(this.myForm, null, 750, 120);
        var size = TopBg.sizeDelta;
        size.y += offect;
        TopBg.sizeDelta = size;
        ScrollViewRect.offsetMax = new Vector2(0, -UserDataManager.Instance.TopHight);

    }

    private void FindGameObject()
    {
        Tilte = transform.Find("ScrollView/Viewport/Content/Tilte").GetComponent<Text>();
        Conten = transform.Find("ScrollView/Viewport/Content/Conten").GetComponent<Text>();
        Close = transform.Find("TopBg/Close").gameObject;
        ContactanButton = transform.Find("TopBg/ContactanButton").gameObject;
        TopBg = transform.Find("TopBg").GetComponent<RectTransform>();
        ScrollViewRect = transform.Find("ScrollView").GetComponent<RectTransform>();
    }
    private void Addlistent()
    {
        UIEventListener.AddOnClickListener(Close,CloseOnclicke);
        UIEventListener.AddOnClickListener(ContactanButton, ContactanButtonOnclicke);
    }
    

    private void CloseOnclicke(PointerEventData data)
    {
        UIEventListener.RemoveOnClickListener(Close, CloseOnclicke);
        CUIManager.Instance.CloseForm(UIFormName.FAQConten);
        UIEventListener.RemoveOnClickListener(ContactanButton, ContactanButtonOnclicke);
    }

    private void ShowContent()
    {
        string Qcontss = t_BookFAQ.Question.ToString();
        Tilte.text = Qcontss.Replace("\\n", "\n");
        string Acontss = t_BookFAQ.Answer.ToString();
        Conten.text = Acontss.Replace("\\n", "\n");
    }

    /// <summary>
    /// 点击后打开反馈意见界面
    /// </summary>
    /// <param name="data"></param>
    private void ContactanButtonOnclicke(PointerEventData data)
    {
        // CUIManager.Instance.OpenForm(UIFormName.FAQFeedBack);
        IGGSDKManager.Instance.OpenTSH();
    }
}

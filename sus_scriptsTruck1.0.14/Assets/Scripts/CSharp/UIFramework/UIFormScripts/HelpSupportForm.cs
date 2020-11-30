using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UGUI;
using DG.Tweening;

public class HelpSupportForm : BaseUIForm
{
    public GameObject UIMask;
    //public RectTransform ClostBtn;
    public RectTransform frameTrans;
    public Text TitleTxt;
    public Text ContentTxt;
    public GameObject FeedbackBtn;

    public GameObject SocialGroup;
    public GameObject FacebookIconGo,InstagramIconGo,TwitterIconGo,DiscordIconGo;

    public GameObject Content;
    public Text textPre;
    public RectTransform TopRect, FrameRect;
    //[Header("--表示以多少行的字节做分割--")]
    private int LndexNo = 70;//这个是表示分割(这里是文本的每70行就做成一部分)
    private int LineNumber = 0, Remainder = 0;
    private string Str;

    public override void OnOpen()
    {
        base.OnOpen();

        //【屏幕适配】
        float offect = XLuaHelper.UnSafeAreaNotFit(this.myForm, null, 750, 120);
        var h = TopRect.rect.size.y;
        h += offect;
        TopRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
        var offsetMax = this.FrameRect.offsetMax;
        offsetMax.y -= offect;
        this.FrameRect.offsetMax = offsetMax;



        UIEventListener.AddOnClickListener(UIMask.gameObject, ClostCurFormHandler);
        UIEventListener.AddOnClickListener(FeedbackBtn.gameObject, FeedbackHandler);

        UIEventListener.AddOnClickListener(FacebookIconGo, FaceBookLinkHandler);
        UIEventListener.AddOnClickListener(InstagramIconGo, InstagramLinkHandler);
        UIEventListener.AddOnClickListener(TwitterIconGo, TwitterLinkHandler);
        UIEventListener.AddOnClickListener(DiscordIconGo, DiscordLinkHandler);

        //ClostBtn.gameObject.SetActive(false);
        //this.frameTrans.localScale = new Vector3(1, 0, 1);
        //ClostBtn.anchoredPosition = new Vector2(0, 0);
        //this.frameTrans.DOScaleY(1, 0.25f).SetEase(Ease.OutBack).OnComplete(() => {
        //    //ClostBtn.gameObject.SetActive(true);
        //    //ClostBtn.DOAnchorPos(new Vector2(0, -565), 0.3f).SetEase(Ease.OutBack).Play();
        //}).Play();
    }

    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(UIMask.gameObject, ClostCurFormHandler);
        UIEventListener.RemoveOnClickListener(FeedbackBtn.gameObject, FeedbackHandler);

        UIEventListener.RemoveOnClickListener(FacebookIconGo, FaceBookLinkHandler);
        UIEventListener.RemoveOnClickListener(InstagramIconGo, InstagramLinkHandler);
        UIEventListener.RemoveOnClickListener(TwitterIconGo, TwitterLinkHandler);
        UIEventListener.RemoveOnClickListener(DiscordIconGo, DiscordLinkHandler);
    }
    
    public void SetInfo(string vTitle,string vContent)
    {
       
    }
   
    //这个是terms文本的读取；
    public void TexteShow(string TextName)
    {
        if (TextName.Equals("Help"))
        {
            TitleTxt.text = "Contact Us";
            SocialGroup.SetActiveEx(true);
        }
        else
        {
            TitleTxt.text = TextName;
            SocialGroup.SetActiveEx(false);
        }

        DestroyConterntChild();    
        TextAsset binAsset = Resources.Load(TextName, typeof(TextAsset)) as TextAsset;

        //Debug.Log("文本内容：" + binAsset.text);

        //ContentTxt.text = binAsset.text;

        string[] strs = binAsset.text.Split('\n');
        //Debug.Log("文本行数：" + strs.Length);
       
        Remainder = strs.Length % LndexNo;
        LineNumber = (strs.Length - strs.Length % LndexNo) / LndexNo;
        //Debug.Log("文本有多少个" + LndexNo + "行：" + LineNumber + "--剩余几行：" + Remainder);

        //这个是整除行处理
        if (LineNumber>0)
        {
            for (int i = 0; i < LineNumber; i++)
            {
                for (int j = 0; j < LndexNo; j++)
                {
                    Str = Str + " " + strs[LndexNo * i + j] + '\n';

                    if (j == (LndexNo - 1))
                    {
                        InstanticText(Str);
                        //Debug.Log("序列：" + (i + 1) + "---文本：" + Str);
                        //ContentTxt.text = Str.ToString();
                        Str = null;
                    }
                }
            }
        }
       
        //这个是余数行处理
        for (int i= LineNumber* LndexNo;i< strs.Length;i++)
        {

            Str = Str + strs[i] + '\n';

            if (i== strs.Length-1)
            {
                InstanticText(Str);
               
                Str = null;
            }
        }
    }

    private void InstanticText(string st)
    {
        //textPre.text = st;
        GameObject go = Instantiate(textPre.gameObject, Content.transform.position, Quaternion.identity);
       
        go.transform.SetParent(Content.transform);
        go.transform.localScale = Vector3.one;
        go.GetComponent<Text>().text = st;
       
    }

    private void DestroyConterntChild()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void ClostCurFormHandler(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CUIManager.Instance.CloseForm(UIFormName.HelpSupportForm);
    }

    private void FeedbackHandler(PointerEventData data)
    {
        var uiform = CUIManager.Instance.OpenForm(UIFormName.FAQFeedBack);
        if (uiform != null)
        {
            uiform.GetComponent<FAQFeedBack>().ShowTypeProblem(4);
        }
    }

    private void FaceBookLinkHandler(PointerEventData data)
    {
        Application.OpenURL("https://www.facebook.com/Scripts-Untold-Secrets-107729237761206/");
    }

    private void InstagramLinkHandler(PointerEventData data)
    {
        Application.OpenURL("https://instagram.com/secretsgame/");
    }

    private void TwitterLinkHandler(PointerEventData data)
    {
        Application.OpenURL("https://twitter.com/ScriptsUntold");
    }

    private void DiscordLinkHandler(PointerEventData data)
    {
        Application.OpenURL("https://discord.gg/tGXxYdB");
    }
}

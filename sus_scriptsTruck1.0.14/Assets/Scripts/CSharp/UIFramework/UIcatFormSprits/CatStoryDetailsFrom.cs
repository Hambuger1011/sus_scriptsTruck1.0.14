using AB;
using pb;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CatStoryDetailsFrom : CatBaseForm {

    private GameObject Mask;
    private Text Tile;
    private Image CatSprite;
    private Text Pag;
    private Text ContenText;
    private GameObject Section1, Section2, Section3;
    private List<GameObject> SectionList,OpenSectionList;
    private List<Text> SectionTextList;
    private t_story StoryInfo;
    private Text Section1Text, Section2Text, Section3Text;
    private int storys;
    private int CatId;
    private RectTransform BG, StoryBg;

    private void Awake()
    {
        Mask = transform.Find("Bg/Top/BG/BackButton").gameObject;
        Tile = transform.Find("Bg/StoryBg/Tile").GetComponent<Text>();
        CatSprite = transform.Find("Bg/StoryBg/ScrollView/Viewport/Content/CatSprit").GetComponent<Image>();
        Pag = transform.Find("Bg/StoryBg/ScrollView/Viewport/Content/pag").GetComponent<Text>();
        ContenText = transform.Find("Bg/StoryBg/ScrollView/Viewport/Content/ContenText").GetComponent<Text>();

        SectionList = new List<GameObject>();
        OpenSectionList = new List<GameObject>();
        SectionTextList = new List<Text>();
      
        Section1Text = transform.Find("Bg/StoryBg/Section1/Section1Text").GetComponent<Text>();
        SectionTextList.Add(Section1Text);
        Section2Text = transform.Find("Bg/StoryBg/Section2/Section2Text").GetComponent<Text>();
        SectionTextList.Add(Section2Text);
        Section3Text = transform.Find("Bg/StoryBg/Section3/Section3Text").GetComponent<Text>();
        SectionTextList.Add(Section3Text);

        Section1 = transform.Find("Bg/StoryBg/Section1").gameObject;     
        SectionList.Add(Section1);
        Section2 = transform.Find("Bg/StoryBg/Section2").gameObject;
        SectionList.Add(Section2);
        Section3 = transform.Find("Bg/StoryBg/Section3").gameObject;
        SectionList.Add(Section3);

        BG = transform.Find("Bg/Top/BG").GetComponent<RectTransform>();
        StoryBg = transform.Find("Bg/StoryBg").GetComponent<RectTransform>();
    }
    public override void OnOpen()
    {
        base.OnOpen();
        UiFormIndex = (int)CatFormEnum.CAT_STORY_DETAIL;
        
        UIEventListener.AddOnClickListener(Mask, CloseUi);
        UIEventListener.AddOnClickListener(Section1,Section1ButtonOnclicke);
        UIEventListener.AddOnClickListener(Section2,Section2ButtonOnclicke);
        UIEventListener.AddOnClickListener(Section3,Section3ButtonOnclicke);

        if (ResolutionAdapter.HasUnSafeArea)
        {
            var scale = this.myForm.GetScale();
            var safeArea = ResolutionAdapter.GetSafeArea();
            var offset = this.myForm.Pixel2View(safeArea.position);
            var offerH = offset.y;

            BG.sizeDelta = new Vector2(750, 120 + offerH);
            StoryBg.offsetMax = new Vector2(54, -(150 + offerH));
        }
    }


    public override void OnClose()
    {
        base.OnClose();

        UIEventListener.RemoveOnClickListener(Mask, CloseUi);

        UIEventListener.RemoveOnClickListener(Section1,Section1ButtonOnclicke);
        UIEventListener.RemoveOnClickListener(Section2,Section2ButtonOnclicke);
        UIEventListener.RemoveOnClickListener(Section3,Section3ButtonOnclicke);

        SectionList = null;
        OpenSectionList = null;
        SectionTextList = null;

    }


    private void CloseUi(PointerEventData data)
    {
        //派发事件，关闭这个界面
        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), UiFormIndex);
    }


   /// <summary>
   /// 
   /// </summary>
   /// <param name="CatID">猫的id</param>
   /// <param name="storys">已经开启的故事数</param>
   /// <param name="max_story">故事最大数</param>
    public void Inite(int CatID,int storys,int max_story)
    {
        CatId = CatID;
        this.storys = storys;
        StoryInfo = GameDataMgr.Instance.table.GetCatStoryById(CatID);
        Tile.text = StoryInfo.title.ToString();

        ContenText.gameObject.SetActive(false);

        CatSprite.sprite= ABSystem.ui.GetUITexture(AbTag.Global, "assets/Bundle/CatPreview/Screnes/Story" + CatID + ".png");
        CatSprite.SetNativeSize();

        for (int i=0;i< SectionList.Count; i++)
        {
            if (storys>i)
            {
                SectionList[i].SetActive(true);

                OpenSectionList.Add(SectionList[i]);
            }
            else
            {
                SectionList[i].SetActive(true);

                SectionList[i].GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("CatStory/bg_kams3");
                SectionList[i].GetComponent<RectTransform>().sizeDelta = new Vector2(184,50);
            }
        }
        if (storys==1)
        {
            Section1ButtonOnclicke(null);

        }else if (storys == 2)
        {
            Section2ButtonOnclicke(null);

        }else if (storys == 3)
        {
            Section3ButtonOnclicke(null);

        }

        Invoke("ContenTextInite",0.2f);
    }

    private void ContenTextInite()
    {
        CancelInvoke("ContenTextInite");
        ContenText.gameObject.SetActive(true);
    }

    private void Section1ButtonOnclicke(PointerEventData data)
    {
        ContenText.gameObject.SetActive(false);
        Invoke("ContenTextInite", 0.2f);

        if (storys >= 1)
        {
            SectionButtonShow(1);
            Pag.text = "1/3";
            string Qcontss = StoryInfo.story1.ToString();
            ContenText.text = Qcontss.Replace("/n", "\n");

            //ContenText.text = StoryInfo.story1.ToString();
            //ContenText.gameObject.SetActive(true);
            //CatSprite.sprite=

            GameHttpNet.Instance.Readpetstory(CatId, 1, ReadpetstoryCallBacke);
        }
        else
        {
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(127);

            UITipsMgr.Instance.PopupTips(Localization, false);
        }

       
    }

    private void Section2ButtonOnclicke(PointerEventData data)
    {
        ContenText.gameObject.SetActive(false);
        Invoke("ContenTextInite", 0.2f);

        if (storys >= 2)
        {
            SectionButtonShow(2);
            Pag.text = "2/3";

            string Qcontss = StoryInfo.story2.ToString();
            ContenText.text = Qcontss.Replace("/n", "\n");

            //ContenText.text = StoryInfo.story2.ToString();
            //ContenText.gameObject.SetActive(true);
            //CatSprite.sprite=

            GameHttpNet.Instance.Readpetstory(CatId, 2, ReadpetstoryCallBacke);
        }
        else
        {
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(127);

            UITipsMgr.Instance.PopupTips(Localization, false);
        }

    }
    private void Section3ButtonOnclicke(PointerEventData data)
    {
        ContenText.gameObject.SetActive(false);
        Invoke("ContenTextInite", 0.2f);

        if (storys >= 3)
        {
            SectionButtonShow(3);
            Pag.text = "3/3";

            string Qcontss = StoryInfo.story3.ToString();
            ContenText.text = Qcontss.Replace("/n", "\n");


            //ContenText.text = StoryInfo.story3.ToString();
            //ContenText.gameObject.SetActive(true);
            //CatSprite.sprite=
            GameHttpNet.Instance.Readpetstory(CatId, 3, ReadpetstoryCallBacke);
        }
        else
        {
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(127);

            UITipsMgr.Instance.PopupTips(Localization, false);
        }

    }

    private void SectionButtonShow(int num)
    {
        for (int i=0;i< OpenSectionList.Count;i++)
        {
            if (i+1==num)
            {
                //正在阅读的故事

                OpenSectionList[i].GetComponent<Image>().sprite= ResourceManager.Instance.GetUISprite("CatStory/bg_kams1");
                string Section = /*"Section"*/ GameDataMgr.Instance.table.GetLocalizationById(209) + (i + 1);
                SectionTextList[i].text = "<color=#5D2E13FF>" + Section+"</color>";

                OpenSectionList[i].GetComponent<RectTransform>().sizeDelta = new Vector2(184, 38);
            } else
            {
                OpenSectionList[i].GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("CatStory/bg_kams2");
                string Section = /*"Section"*/ GameDataMgr.Instance.table.GetLocalizationById(209) + (i + 1);
                SectionTextList[i].text = "<color=#FAFAFAFF>" + Section + "</color>";
               
                OpenSectionList[i].GetComponent<RectTransform>().sizeDelta = new Vector2(184, 50);
            }
        }
    }

    private void ReadpetstoryCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----ReadpetstoryCallBacke---->" + result);

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    //UITipsMgr.Instance.PopupTips("故事阅读成功", false);
                }
                else
                {
                    //UITipsMgr.Instance.PopupTips("故事阅读失败", false);
                }

            }, null);
        }
    }
}

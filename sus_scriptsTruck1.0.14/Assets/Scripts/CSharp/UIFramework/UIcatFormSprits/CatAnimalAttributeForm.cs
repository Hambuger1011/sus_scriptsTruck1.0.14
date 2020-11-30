using DG.Tweening;
using pb;
using System;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using AB;

public class CatAnimalAttributeForm : CatBaseForm
{

    private GameObject Mask;
    private Text Name;
    private Image CatSprite;
    private Text ContenText;
    private GameObject PersonalityButton;
    private Text PersonalityButtonText, Personality;
   
    private GameObject AppearanceButton;
    private Text AppearanceButtonText;
    private Text AppearanceText;

    private GameObject Star, StarOff;
    private GameObject TopStat;
    private homepetpetarr homepetpetarr;
   
    private List<GameObject> StarList;

    private RectTransform frameTrans;
    private Text Level;

    private int CatId;


    private string Price;
    private void Awake()
    {
        Mask = transform.Find("Mask").gameObject;
        Name = transform.Find("BG/Name").GetComponent<Text>();
        CatSprite = transform.Find("BG/CatSprite").GetComponent<Image>();
        ContenText = transform.Find("BG/ContenBg/ContenText").GetComponent<Text>();
        PersonalityButton = transform.Find("BG/PersonalityPren/PersonalityButton").gameObject;

        Personality = transform.Find("BG/PersonalityPren/Personality").GetComponent<Text>();
        Personality.text = GameDataMgr.Instance.table.GetLocalizationById(105);

        PersonalityButtonText = transform.Find("BG/PersonalityPren/PersonalityButton/PersonalityButtonText").GetComponent<Text>();
       
        AppearanceButton = transform.Find("BG/AppearancePren/AppearanceButton").gameObject;
        AppearanceButtonText = transform.Find("BG/AppearancePren/AppearanceButton/AppearanceButtonText").GetComponent<Text>();
        AppearanceText = transform.Find("BG/AppearancePren/AppearanceText").GetComponent<Text>();

        AppearanceText.text = GameDataMgr.Instance.table.GetLocalizationById(107);

        Level = transform.Find("BG/Level").GetComponent<Text>();
        Level.text = GameDataMgr.Instance.table.GetLocalizationById(251);

        Star = transform.Find("BG/TopStat/Star").gameObject;
        StarOff = transform.Find("BG/TopStat/StarOff").gameObject;
        TopStat = transform.Find("BG/TopStat").gameObject;

        frameTrans = transform.Find("BG").GetComponent<RectTransform>();

        Star.SetActive(false);
    }
    public override void OnOpen()
    {
        base.OnOpen();
        UiFormIndex = (int)CatFormEnum.CAT_ANIMALATTRIBUTE;

        UIEventListener.AddOnClickListener(Mask, CloseUi);
       
       
        if (GameUtility.IpadAspectRatio())
        {
            this.frameTrans.localScale = new Vector3(0.7f, 0, 1);
            this.frameTrans.DOScaleY(0.7f, 0.25f).SetEase(Ease.OutBack).Play();
        }
        else
        {
            StartShow();
        }

        Transform CloseButton = transform.Find("CloseButton");
        CloseButton.GetComponent<RectTransform>().DOAnchorPosY(-516, 0.4f).SetEase(Ease.OutBack);
    }


    public override void OnClose()
    {
        base.OnClose();
        CatSprite.sprite = null;

        UIEventListener.RemoveOnClickListener(Mask, CloseUi);
     
        if (StarList != null)
        {
            for (int i = 0; i < StarList.Count; i++)
            {
                Destroy(StarList[i]);
            }

            StarList.Clear();
        }
    }


    private void CloseUi(PointerEventData data)
    {
        //派发事件，关闭这个界面
        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), UiFormIndex);
    }

    /// <summary>
    /// 界面刚出现的效果显示
    /// </summary>
    private void StartShow()
    {
        frameTrans.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        frameTrans.DOScale(new Vector3(1, 1, 1), 0.4f).SetEase(Ease.OutBack);  
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pid"></param>
    /// <param name="type">1 默认 2 动物图鉴打开的界面，把按钮置灰  </param>
    public void Inite(int pid, int type)
    {
        CatId = pid;      
        string CatNameNumber = "cat" + pid;
        CatSprite.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/" + CatNameNumber);

        Price = GameDataMgr.Instance.table.GetCatAttributeById(pid).diamond_qty.ToString();

        GameHttpNet.Instance.Getuserhomepet(pid, GetuserhomepetCallBacke);

        PlayCatTones();
    }

    private void PlayCatTones()
    {
        var asset = ABSystem.ui.bundle.LoadImme(AbTag.Cat, enResType.eAudio, string.Concat("assets/Bundle/CatPreview/CatSounds/", CatId, ".mp3"));
        AudioManager.Instance.PlayCatTones(asset.resAudioClip);
    }

    private void GetuserhomepetCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetuserhomepetCallBacke---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.Getuserhomepet = JsonHelper.JsonToObject<HttpInfoReturn<Getuserhomepet>>(result);
                    if (UserDataManager.Instance.Getuserhomepet != null)
                    {

                        //List<petarr> tem = UserDataManager.Instance.Getuserhomepet.data.homepetpetarr;                  
                        string naem = UserDataManager.Instance.Getuserhomepet.data.petarr.pet_name;

                        this.homepetpetarr = UserDataManager.Instance.Getuserhomepet.data.petarr;
                        CatInitInfo();
                    }
                }
                else if (jo.code == 201)
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(122);
                    UITipsMgr.Instance.PopupTips(Localization, false);
                }

            }, null);
        }
    }

    private void CatInitInfo()
    {
        if (Name == null)
        {
            return;
        }

        Name.text = homepetpetarr.pet_name.ToString();
        PersonalityButtonText.text = homepetpetarr.personality.ToString();

        StarList = new List<GameObject>();
        
        for (int i = 0; i < 5; i++)
        {
            GameObject go = null;
            if (i<homepetpetarr.level)
            {
                go = Instantiate(Star);
                go.transform.SetParent(TopStat.transform);
                go.transform.localScale = Vector3.one;
                go.SetActive(true);
                StarList.Add(go);
            }else
            {
                go = Instantiate(StarOff);
                go.transform.SetParent(TopStat.transform);
                go.transform.localScale = Vector3.one;
                go.SetActive(true);
                StarList.Add(go);
            }          
        }

        AppearanceButtonText.text = homepetpetarr.appearance.ToString();
        ContenText.text = homepetpetarr.remark.ToString();

       
       
    } 
}

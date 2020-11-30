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

public class CatDetailsForm : CatBaseForm {

    private GameObject Mask;
    private Text Name;
    private Image CatSprite;
    private Text ContenText;
    private GameObject PersonalityButton;
    private Text PersonalityButtonText,Personality;
    private GameObject lntimacyButton;
    private Text lntimacy;
    private Text lntimacyButtonText;
    private GameObject AppearanceButton;
    private Text AppearanceButtonText;
    private Text AppearanceText;
    private GameObject DifficultyPren;
    private Image[] DifficultyImageList;
    private GameObject oboption;
    private Text oboptionText;
    private GameObject ReleasePren;
    private GameObject Release;
    private Text ReleaseText;
    private GameObject Star;
    private GameObject TopStat;
    private homepetpetarr homepetpetarr;
    private int ButtonEvenType = 0;
    private Text level;

    private List<GameObject> StarList;
    private int Type = 0;
    private GameObject newtag;

    private int CatId;

    private RectTransform frameTrans;

    private string Price;
    private int PriceNum;
    private MyCatteryItem MyCatteryItem;
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
        lntimacyButton = transform.Find("BG/lntimacyPren/lntimacyButton").gameObject;
        lntimacyButtonText = transform.Find("BG/lntimacyPren/lntimacyButton/lntimacyButtonText").GetComponent<Text>();  
        
        lntimacy = transform.Find("BG/lntimacyPren/lntimacy").GetComponent<Text>();
        lntimacy.text= GameDataMgr.Instance.table.GetLocalizationById(106);

        AppearanceButton = transform.Find("BG/AppearancePren/AppearanceButton").gameObject;
        AppearanceButtonText = transform.Find("BG/AppearancePren/AppearanceButton/AppearanceButtonText").GetComponent<Text>();
        AppearanceText = transform.Find("BG/AppearancePren/AppearanceText").GetComponent<Text>();

        AppearanceText.text= GameDataMgr.Instance.table.GetLocalizationById(107);

        level = transform.Find("BG/Level").GetComponent<Text>();
        level.text = GameDataMgr.Instance.table.GetLocalizationById(251);

        DifficultyPren = transform.Find("BG/DifficultyPren").gameObject;
        DifficultyImageList = transform.Find("BG/DifficultyPren/DifficultyBg").GetComponentsInChildren<Image>();//在false状态下不能找到
        oboption = transform.Find("BG/OboptionPren/oboption").gameObject;
        oboptionText = transform.Find("BG/OboptionPren/oboptionText").GetComponent<Text>();
        newtag = transform.Find("BG/OboptionPren/newtag").gameObject;
        ReleasePren = transform.Find("BG/ReleasePren").gameObject;
        Release = transform.Find("BG/ReleasePren/Release").gameObject;
        ReleaseText = transform.Find("BG/ReleasePren/ReleaseText").GetComponent<Text>();
        Star = transform.Find("BG/TopStat/Star").gameObject;
        TopStat = transform.Find("BG/TopStat").gameObject;

        frameTrans = transform.Find("BG").GetComponent<RectTransform>();
        Star.SetActive(false);
    }
    public override void OnOpen()
    {
        base.OnOpen();
        UiFormIndex = (int)CatFormEnum.CAT_DETAIL;
      
        UIEventListener.AddOnClickListener(Mask, CloseUi);
        UIEventListener.AddOnClickListener(Release,ReleaseButtonOnclicke);
        UIEventListener.AddOnClickListener(oboption.gameObject, OboptionButtonOnclicke);
      
        if (GameUtility.IpadAspectRatio())
        {
            this.frameTrans.localScale = new Vector3(0.7f, 0, 1);
            this.frameTrans.DOScaleY(0.7f, 0.25f).SetEase(Ease.OutBack).Play();
        }
        else
        {
            StartShow();
        }
        Transform CloseButton = transform.Find("BG/CloseButton");
        CloseButton.GetComponent<RectTransform>().DOAnchorPosY(-590, 0.4f).SetEase(Ease.OutBack);
    }
    

    public override void OnClose()
    {
        base.OnClose();

        CatSprite.gameObject.SetActive(false);
        CatSprite.sprite = null;

        UIEventListener.RemoveOnClickListener(Mask, CloseUi);
        UIEventListener.RemoveOnClickListener(oboption.gameObject, OboptionButtonOnclicke);
        UIEventListener.RemoveOnClickListener(Release, ReleaseButtonOnclicke);

        oboption.GetComponent<Image>().sprite = null;

        if (StarList!=null)
        {
            for (int i = 0; i < StarList.Count; i++)
            {
                Destroy(StarList[i]);
            }
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
    public void Inite(int pid,int type,MyCatteryItem MyCatteryItem)
    {
        this.MyCatteryItem = MyCatteryItem;
        CatId = pid;
        Type = type;
        string CatNameNumber = "cat" + pid;
        CatSprite.sprite= ResourceManager.Instance.GetUISprite("CatDecFoodIcon/"+ CatNameNumber);

        Price = GameDataMgr.Instance.table.GetCatAttributeById(pid).diamond_qty.ToString();
        PriceNum = GameDataMgr.Instance.table.GetCatAttributeById(pid).diamond_qty;

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
                        string naem=UserDataManager.Instance.Getuserhomepet.data.petarr.pet_name;

                        this.homepetpetarr = UserDataManager.Instance.Getuserhomepet.data.petarr;
                        CatInitInfo();
                    }
                }else if (jo.code==201)
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(122);                   
                    UITipsMgr.Instance.PopupTips(Localization, false);
                }

            }, null);
        }
    }

    private void CatInitInfo()
    {
        if (Name==null)
        {
            return;
        }

        Name.text = homepetpetarr.pet_name.ToString();
        PersonalityButtonText.text = homepetpetarr.personality.ToString();
        
        StarList = new List<GameObject>();
        StarList.Clear();

        for (int i=0;i< homepetpetarr.level;i++)
        {
            GameObject go = Instantiate(Star);
            go.transform.SetParent(TopStat.transform);
            go.transform.localScale = Vector3.one;
            go.SetActive(true);
            StarList.Add(go);
        }

        AppearanceButtonText.text = homepetpetarr.appearance.ToString();
        ContenText.text = homepetpetarr.remark.ToString();

        if (homepetpetarr.isfit == 1)
        {
            //满足收养条件
            oboption.GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("CatGiftFromAnimalForm/bg_btn1");
        }
        else
        {
            //没有满足收养条件
            //不可以用
            oboption.GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("CatGiftFromAnimalForm/bg_btn");
        }

        if (homepetpetarr.isadopt==1)
        {
            //已经收养了
            DifficultyPren.SetActive(false);
            ReleasePren.SetActive(true);
            oboptionText.text = GameDataMgr.Instance.table.GetLocalizationById(197) + " " + homepetpetarr.storys + "/" + homepetpetarr.max_story;

            
            if (homepetpetarr.storys_isused==1)
            {
                //可以用
                oboption.GetComponent<Image>().sprite= ResourceManager.Instance.GetUISprite("CatGiftFromAnimalForm/bg_btn1");

                //有新故事
                newtag.SetActive(true);
            }
            else
            {
                //不可以用
                oboption.GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("CatGiftFromAnimalForm/bg_btn");

                //没有新故事
                newtag.SetActive(false);
            }

            lntimacy.text = GameDataMgr.Instance.table.GetLocalizationById(106) /*"Intimacy"*/;
            lntimacyButtonText.text = homepetpetarr.intimacy.ToString();

        }
        else if (homepetpetarr.isadopt==0)
        {
            //没有收养
            ReleasePren.SetActive(false);
            DifficultyPren.SetActive(true);
            int henghshu = homepetpetarr.dif_level;
            for (int i=1;i<6;i++)
            {
                if (i-1< henghshu)
                {
                    //Debug.Log("数组个数："+ DifficultyImageList.Length+"--名称："+ DifficultyImageList[0].gameObject.name);
                    DifficultyImageList[i].gameObject.SetActive(true);
                }else
                {
                    DifficultyImageList[i].gameObject.SetActive(false);
                }
            }
            lntimacy.text = GameDataMgr.Instance.table.GetLocalizationById(196) /*"visits"*/;
            lntimacyButtonText.text = homepetpetarr.visits.ToString()/* + "/" + homepetpetarr.visit_min.ToString()*/;

        }

        if (!UserDataManager.Instance.CheckGameFunIsOpen(3))
        {
            //收养功能没有开启
            oboption.GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("CatGiftFromAnimalForm/bg_btn");
        } 
    }

    /// <summary>
    /// 放养按钮
    /// </summary>
    /// <param name="data"></param>

    private void ReleaseButtonOnclicke(PointerEventData data)
    {

        //if (Type == 2)
        //{
        //    return;
        //}

        if (homepetpetarr.isadopt == 1)
        {
            //已经收养了
            ButtonEvenType = 2;
            CUIManager.Instance.OpenForm(UIFormName.CatPublicForm);
            WindowInfo tmpWin = new WindowInfo(GameDataMgr.Instance.table.GetLocalizationById(198), GameDataMgr.Instance.table.GetLocalizationById(199), Price, "", PublicNoButtonOnClike, PublieUiYesButtonOnClike, 4, "", "", "");
            CUIManager.Instance.GetForm<CatPublicForm>(UIFormName.CatPublicForm).Inite(tmpWin);
        }
    }

    /// <summary>
    /// 这个是收养与打开故事界面
    /// </summary>
    /// <param name="data"></param>
    private void OboptionButtonOnclicke(PointerEventData data)
    {
        if (!UserDataManager.Instance.CheckGameFunIsOpen(3))
        {
            //收养功能没有开启
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(269);
            UITipsMgr.Instance.PopupTips(Localization, false);
            return;
        }
        //Debug.Log("dfafafafa");
        ButtonEvenType = 1;
        if (homepetpetarr.isadopt == 1)
        {
         
            //已经收养了
            if (homepetpetarr.storys_isused == 1)
            {

                //派发事件，关闭这个界面
                EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), UiFormIndex);

                //派发事件关闭收养界面
                EventDispatcher.Dispatch(EventEnum.CatteryFormClose);

                //可以用，弹出故事界面
                AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
                CUIManager.Instance.OpenForm(UIFormName.CatStoryDetails);
                CatStoryDetailsFrom catStoryDetailsFrom = CUIManager.Instance.GetForm<CatStoryDetailsFrom>(UIFormName.CatStoryDetails);
                if (catStoryDetailsFrom != null)
                    catStoryDetailsFrom.Inite(homepetpetarr.id, homepetpetarr.storys, homepetpetarr.max_story);
            }
            else
            {
                //不可以用

                string Localization = null;
                if (homepetpetarr.isfullpet==0)
                {
                    //院子已经满了
                    Localization = GameDataMgr.Instance.table.GetLocalizationById(125);

                }
                else
                {
                    //院子没有满
                    Localization = GameDataMgr.Instance.table.GetLocalizationById(124);

                }


                UITipsMgr.Instance.PopupTips(Localization, false);

                oboption.GetComponent<Image>().sprite = ResourceManager.Instance.GetUISprite("CatGiftFromAnimalForm/bg_btn");
            }
        }
        else if (homepetpetarr.isadopt == 0)
        {
            //没有收养

            if (homepetpetarr.isfullpet == 0)
            {
                //未解锁的位置
                if (UserDataManager.Instance.Getadoptpet==null)
                {
                    //UINetLoadingMgr.Instance.Show();
                    GameHttpNet.Instance.Getadoptpet(CallBacketadoptpet);

                }
                else
                {
                    int CatLock = UserDataManager.Instance.Getadoptpet.data.gold_pet - UserDataManager.Instance.Getadoptpet.data.max_pet;
                    string Localization = null;
                    if (CatLock > 0)
                    {
                        //位置可以扩建
                        Localization = GameDataMgr.Instance.table.GetLocalizationById(121);
                    }
                    else
                    {
                        Localization = GameDataMgr.Instance.table.GetLocalizationById(122);
                    }
                    UITipsMgr.Instance.PopupTips(Localization, false);
                }

                return;
            }

            if (homepetpetarr.isfit == 1)
            {
                //满足收养条件
                CUIManager.Instance.OpenForm(UIFormName.CatPublicForm);

                string St = string.Format(GameDataMgr.Instance.table.GetLocalizationById(115), homepetpetarr.dif_level);

                WindowInfo tmpWin = new WindowInfo(GameDataMgr.Instance.table.GetLocalizationById(200), St, Price, "", PublicNoButtonOnClike, PublieUiYesButtonOnClike, 3, "", "", "");
                CUIManager.Instance.GetForm<CatPublicForm>(UIFormName.CatPublicForm).Inite(tmpWin);
            }
            else 
            {
                string St = homepetpetarr.petlimit.ToString();
                string[] st = St.Split('#');

                int num =int.Parse(st[0]);
                //Debug.Log("num:" + num);

                if (homepetpetarr.nofitseason==9)
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(119);

                    UITipsMgr.Instance.PopupTips(Localization, false);
                    return;
                }

                if (num == 1)
                {
                    //性格1不满足
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(120);
                    string sT = string.Format(Localization, GameDataMgr.Instance.table.GetLocalizationById(202), st[1]); //"你的热情指数不足" + st[1] + "，它不太喜欢你";

                    UITipsMgr.Instance.PopupTips(sT, false);

                }
                else if (num == 2)
                {
                    //性格2不满足

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(120);
                    string sT = string.Format(Localization, GameDataMgr.Instance.table.GetLocalizationById(203), st[1]); //"你的热情指数不足" + st[1] + "，它不太喜欢你";

                    //string sT = "你的严格的指数不足" + st[1] + "，它不太喜欢你";
                    UITipsMgr.Instance.PopupTips(sT, false);
                }
                else if (num == 3)
                {
                    //性格3不满足

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(120);
                    string sT = string.Format(Localization, GameDataMgr.Instance.table.GetLocalizationById(204), st[1]); //"你的热情指数不足" + st[1] + "，它不太喜欢你";

                    // string sT = "你的优雅的指数不足" + st[1] + "，它不太喜欢你";
                    UITipsMgr.Instance.PopupTips(sT, false);
                }
                else if (num == 4)
                {
                    //性格4不满足

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(120);
                    string sT = string.Format(Localization, GameDataMgr.Instance.table.GetLocalizationById(205), st[1]); //"你的热情指数不足" + st[1] + "，它不太喜欢你";

                    //string sT = "你的好奇的指数不足" + st[1] + "，它不太喜欢你";
                    UITipsMgr.Instance.PopupTips(sT, false);
                }
                else if (num == 5)
                {
                    //性格5不满足
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(120);
                    string sT = string.Format(Localization, GameDataMgr.Instance.table.GetLocalizationById(206), st[1]); //"你的热情指数不足" + st[1] + "，它不太喜欢你";

                    //string sT = "你的充满深情的指数不足" + st[1] + "，它不太喜欢你";
                    UITipsMgr.Instance.PopupTips(sT, false);
                }
                else
                {
                    //来访次数不足5次
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(119);

                    UITipsMgr.Instance.PopupTips(Localization, false);
                }
                //没有满足收养条件
               
            }     
        }
    }


    private void CallBacketadoptpet(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----getadoptpet-CallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.Getadoptpet = JsonHelper.JsonToObject<HttpInfoReturn<Getadoptpet>>(result);

                    int CatLock = UserDataManager.Instance.Getadoptpet.data.gold_pet - UserDataManager.Instance.Getadoptpet.data.max_pet;
                    string Localization = null;
                    if (CatLock > 0)
                    {
                        //位置可以扩建
                        Localization = GameDataMgr.Instance.table.GetLocalizationById(121);
                    }
                    else
                    {
                        Localization = GameDataMgr.Instance.table.GetLocalizationById(122);
                    }
                    UITipsMgr.Instance.PopupTips(Localization, false);
                }

            }, null);
        }
    }

    private void PublieUiYesButtonOnClike(string st)
    {
        LOG.Info("Yes按钮点击了");

        if (ButtonEvenType==1)
        {
            //收养按钮事件处理
            if (homepetpetarr.isadopt == 1)
            {
                //已经收养的
            }
            else
            {
                //没有收养的

                //UINetLoadingMgr.Instance.Show();
                GameHttpNet.Instance.PostPetadopt(homepetpetarr.id, PetadoptCallBacke);
            }
        }else if (ButtonEvenType == 2)
        {
            //放养按钮事件处理
            //UINetLoadingMgr.Instance.Show();        
            GameHttpNet.Instance.Postpetbackout(homepetpetarr.adoptid, PostpetbackoutCallBacke);
        }

    }

    private void PublicNoButtonOnClike(string st)
    {
        LOG.Info("No按钮点击了");
    }


    private void PetadoptCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----PetadoptCallBacke---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {        
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();

                UserDataManager.Instance.Petadopt = JsonHelper.JsonToObject<HttpInfoReturn<Petadopt>>(result);
                var tmpForm = CUIManager.Instance.GetForm<CatTopMain>(UIFormName.CatTop);

                if (jo.code == 200)
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(116);
                    UITipsMgr.Instance.PopupTips(Localization, false);
                    if (tmpForm != null)
                    {
                        tmpForm.RefreshDiamond(2, UserDataManager.Instance.Petadopt.data.diamond);
                        UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.Petadopt.data.diamond);
                    }
                    EventDispatcher.Dispatch(EventEnum.CatAdoptionSucc, CatId);//收养成功

                    //收养成功
                    CloseUi(null);
                }
                else if (jo.code==201)
                {
                    //已超出当前院子可领养的宠物数

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(122);
                    UITipsMgr.Instance.PopupTips(Localization, false);


                    if (tmpForm != null)
                    {

                        tmpForm.RefreshDiamond(2, UserDataManager.Instance.Petadopt.data.diamond);
                        UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.Petadopt.data.diamond);
                    }
                }
                else if (jo.code == 202)
                {
                    //宠物ID不存在

                    if (tmpForm != null)
                    {

                        tmpForm.RefreshDiamond(2, UserDataManager.Instance.Petadopt.data.diamond);
                        UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.Petadopt.data.diamond);
                    }
                }
                else if (jo.code == 203)
                {
                    //宠物已经存在了,无需在重新收养

                    if (tmpForm != null)
                    {

                        tmpForm.RefreshDiamond(2, UserDataManager.Instance.Petadopt.data.diamond);
                        UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.Petadopt.data.diamond);
                    }
                }
                else if (jo.code == 204)
                {
                    // 钻石不足，无法收养
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(113);

                    UITipsMgr.Instance.PopupTips(Localization, false);

                    CUIManager.Instance.OpenForm(UIFormName.ChargeTipsForm);
                    ChargeTipsForm tipForm = CUIManager.Instance.GetForm<ChargeTipsForm>(UIFormName.ChargeTipsForm);

                    //CUIManager.Instance.OpenForm(UIFormName.NewChargeTips);
                    //NewChargeTips tipForm = CUIManager.Instance.GetForm<NewChargeTips>(UIFormName.NewChargeTips);


                    if (tipForm != null)
                    {
                        tipForm.Init(2, PriceNum, 1 * 0.99f);
                        tipForm.CatTopMainChange();
                    }

                    if (tmpForm != null)
                    {

                        tmpForm.RefreshDiamond(2, UserDataManager.Instance.Petadopt.data.diamond);
                        UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.Petadopt.data.diamond);
                    }
                }
                else if (jo.code == 205)
                {
                    //收养失败
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(117);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    if (tmpForm != null)
                    {

                        tmpForm.RefreshDiamond(2, UserDataManager.Instance.Petadopt.data.diamond);
                        UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.Petadopt.data.diamond);
                    }
                }
                else
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(117);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    if (tmpForm != null)
                    {

                        tmpForm.RefreshDiamond(2, UserDataManager.Instance.Petadopt.data.diamond);
                        UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.Petadopt.data.diamond);
                    }
                }

            }, null);
        }
    }

    private void PostpetbackoutCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----PostpetbackoutCallBacke---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    //放养成功
                    CloseUi(null);
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(126);

                    UITipsMgr.Instance.PopupTips(Localization, false);

                    if(MyCatteryItem!=null)
                       MyCatteryItem.HadGiveUp = true;
                }

            }, null);
        }
    }

}

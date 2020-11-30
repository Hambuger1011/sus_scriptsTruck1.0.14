using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatMainFormClasse;
using System;
using AB;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using pb;
using UGUI;
using System.Text;
using System.Linq;
using DG.Tweening;

/// <summary>
/// 猫场景(主要负责底图的展示、场景扩展，添加食物)
/// </summary>
public class CatSceneView : CatClase
{
    private RectTransform ContentRectTrans;
    private CatMainForm catForm;
    private Image Screnes, BgMask1, BgMask2, HouseImage, foodImg, StonTerracesImg;
    private GameObject CatPrefba, decorationPosPrefab, arrowPrefab,arrowGroup, arrowImage,decoration;
    private List<GameObject> mNewPosGoList;//扩展院子时，展示新的装饰物位置
    private List<GameObject> foodPosObjList;
    private List<GameObject> CreatFX;//生成的特效

    private CatSceneAnimView mSceneAnimView;

    private float SpriteWidth, SpriteHeight, SpriteWidthChange;//获得原来图片的长宽高
    private float SpriteWidthNext, SpriteHeightNext;//场景变化之后的大小
    private bool Mask1IsShow = true;
    private bool Mask2IsShow = true;
    private packarr food;
    private t_food tfood;

    private RectTransform Penquan;
    private GameObject CatFoodBowl, CatFoodBowlGo, CatFoodBowlImagGo;

    private GameObject NewPosPrompt;

    private t_shop t_shopGuid;
    public override void Bind(CatMainForm catmainform)
    {
        catmainform.catSceneView = this;
        catForm = catmainform;
        NewPosPrompt = catForm.NewPosPrompt;

        if (CreatFX == null)
            CreatFX = new List<GameObject>();

        EventDispatcher.AddMessageListener(EventEnum.SetGuidPos, SetGuidPos);

        Init();

        EventDispatcher.AddMessageListener(EventEnum.CatGuidRepair, CatGuidRepair);
    }


    private void Init()
    {
        InitTableData();
        InitSceneImg();
        InitFixBuild();
        InitEvent();
        InitCatDataInfo();
    }
    //初始化配置数据
    private void InitTableData()
    {
        GameDataMgr.Instance.table.GetMapData();
        GameDataMgr.Instance.table.catShopData();
        GameDataMgr.Instance.table.GetCatGifData();
        GameDataMgr.Instance.table.GetCatFoodData();
        GameDataMgr.Instance.table.GetCatStoryData();
        GameDataMgr.Instance.table.catDecorationData();
        GameDataMgr.Instance.table.GetCatAttributeData();
    }

    //初始化场景地图
    private void InitSceneImg()
    {
        ContentRectTrans = transform.Find("Viewport/Content").transform.rectTransform();
        Screnes = transform.Find("Viewport/Content/Scenes").GetComponent<Image>();
        BgMask1 = Screnes.transform.Find("Mask1").GetComponent<Image>();
        BgMask2 = Screnes.transform.Find("Mask2").GetComponent<Image>();
        CatPrefba = transform.Find("Viewport/Content/Scenes/CatCloth").gameObject;
        decorationPosPrefab = transform.Find("Viewport/Content/Scenes/decorationPosPrefab").gameObject;
        decoration = transform.Find("Viewport/Content/Scenes/decoration").gameObject;
        arrowPrefab = transform.Find("Viewport/Content/Scenes/arrowPrefab").gameObject;
        Screnes.sprite = ABSystem.ui.GetUITexture(AbTag.Global,"assets/Bundle/CatPreview/Screnes/1.png");
        Screnes.SetNativeSize();

        BgMask1.sprite = ABSystem.ui.GetUITexture(AbTag.Global,"assets/Bundle/CatPreview/Screnes/Mask1.png");
        BgMask1.SetNativeSize();

        BgMask2.sprite = ABSystem.ui.GetUITexture(AbTag.Global,"assets/Bundle/CatPreview/Screnes/Mask2.png");
        BgMask2.SetNativeSize();

        Penquan = transform.Find("Viewport/Content/Scenes/Penquan").GetComponent<RectTransform>();
        CatFoodBowl = transform.Find("Viewport/Content/Scenes/CatFoodBowlBg").gameObject;

        RectTransform Rect = Screnes.GetComponent<RectTransform>();
        SpriteWidth = Rect.rect.width;//原来背景图片的大小
        SpriteHeight = Rect.rect.height;//原来背景图片的大小


        SpriteWidthChange = SpriteWidth * catForm.ScreneHeight / SpriteHeight;
        float rate = catForm.ScreneHeight / SpriteHeight;

        SpriteWidthNext = SpriteWidth * rate;//缩放后背景图的大小
        SpriteHeightNext = catForm.ScreneHeight;//缩放后背景图的大小
        Rect.sizeDelta = new Vector2(SpriteWidthNext, catForm.ScreneHeight);


        mSceneAnimView = gameObject.GetComponent<CatSceneAnimView>();
        if (mSceneAnimView != null)
            mSceneAnimView.Init(decoration, CatPrefba, decorationPosPrefab, Rect, SpriteWidth, SpriteHeight, SpriteWidthNext, SpriteHeightNext);


        RectTransform mask1Trans = BgMask1.rectTransform();
        RectTransform mask2Trans = BgMask2.rectTransform();

        mask1Trans.sizeDelta = new Vector2(mask1Trans.rect.width * rate, mask1Trans.rect.height * rate);
        mask2Trans.sizeDelta = new Vector2(mask2Trans.rect.width * rate, mask2Trans.rect.height * rate);

        mask1Trans.anchoredPosition = new Vector2(mask1Trans.anchoredPosition.x * rate, mask1Trans.anchoredPosition.y * rate);
        mask2Trans.anchoredPosition = new Vector2(mask2Trans.anchoredPosition.x * rate, mask2Trans.anchoredPosition.y * rate);
        Mask1IsShow = true;
        Mask2IsShow = true;

        // 设置好喷泉的位置

        float PenquanX = (247.3F * SpriteWidthNext) / SpriteWidth;
        float PenquanY = (-412.4f * SpriteHeightNext) / SpriteHeight;
        Penquan.anchoredPosition = new Vector2(PenquanX, PenquanY);

        //Debug.Log("SpriteWidthNext:"+ SpriteWidthNext+ "--SpriteWidth:"+ SpriteWidth);
        //Debug.Log("SpriteHeightNext:" + SpriteHeightNext + "--SpriteHeight:" + SpriteHeight);
    }

    //初始化固定建筑
    private void InitFixBuild()
    {
        CreateCao();
        CreateArrow();
        CreateCatHouse();
        CreateStoneTerraces();
        CreateFoodBox();
    }

    private void CreateArrow()
    {
        arrowGroup = Instantiate(arrowPrefab);
        arrowGroup.transform.SetParent(Screnes.gameObject.transform);
        arrowGroup.name = "arrow";
        arrowGroup.SetActive(false);
        arrowGroup.transform.localScale = Vector3.one;
        arrowImage = arrowGroup.transform.Find("arrowImage").gameObject;
    }

    //创建猫屋实例
    private void CreateCatHouse()
    {
        GameObject go = Instantiate(decorationPosPrefab);
        go.transform.SetParent(Screnes.gameObject.transform);
        go.SetActive(true);
        go.transform.localScale = Vector3.one;
        go.name = "CatHouse";
        go.transform.rectTransform().anchoredPosition3D = new Vector3(403f * catForm.ScreneHeight / SpriteHeight, -232 * catForm.ScreneHeight / SpriteHeight, 0);
        HouseImage = go.GetComponent<Image>();
        HouseImage.sprite = ResourceManager.Instance.GetUISprite("CatSceneDecorations/house");
        HouseImage.SetNativeSize();

        //加载艹的特效
        GameObject EF = ResourceManager.Instance.LoadAssetBundleUI("UI/Resident/FX_UI/fx_cathouse_caodong");
        EF.transform.SetParent(go.gameObject.transform);
        EF.transform.localPosition = Vector3.zero;
        EF.transform.localScale = Vector3.one;
    }
    private void HouseButtonOnclick(PointerEventData data)
    {

        if (!UserDataManager.Instance.CheckGameFunIsOpen(3))
        {
            //收养功能没有开放
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(269);
            UITipsMgr.Instance.PopupTips(Localization, false);
            return;
        }
        if (UserDataManager.Instance.InPlaceCatThings != 0) return;
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CUIManager.Instance.OpenForm(UIFormName.Cattery);
    }

    //创建石台实例
    private void CreateStoneTerraces()
    {
        GameObject go = Instantiate(decorationPosPrefab);
        go.transform.SetParent(Screnes.gameObject.transform);
        go.SetActive(true);
        go.transform.localScale = Vector3.one;
        go.name = "FoodStonTerraces";
        go.transform.rectTransform().anchoredPosition3D = new Vector3(337f * catForm.ScreneHeight / SpriteHeight, -1087 * catForm.ScreneHeight / SpriteHeight, 0);
        StonTerracesImg = go.GetComponent<Image>();
        StonTerracesImg.sprite = ResourceManager.Instance.GetUISprite("CatSceneDecorations/foodstands");
        StonTerracesImg.SetNativeSize();

        //加载艹的特效
        GameObject EF = ResourceManager.Instance.LoadAssetBundleUI("UI/Resident/FX_UI/fx_food_caodong");
        EF.transform.SetParent(go.gameObject.transform);
        EF.transform.localPosition = Vector3.zero;
        EF.transform.localScale = Vector3.one;
        CreatFX.Add(EF);

    }

    //食盒
    private void CreateFoodBox()
    {
        GameObject go = Instantiate(CatFoodBowl);
        CatFoodBowlGo = go;
        go.transform.SetParent(Screnes.gameObject.transform);
        go.transform.localScale = new Vector3(1, 1, 1);
        //go.name = "CatFoodBowl";
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition3D = new Vector3(340f * catForm.ScreneHeight / SpriteHeight, -1047 * catForm.ScreneHeight / SpriteHeight, 0);
        foodImg = go.transform.Find("CatFoodBowl/foodImg").GetComponent<Image>();
        CatFoodBowlImagGo = foodImg.gameObject;
        go.SetActive(false);

       
    }

    //创建场景中的艹
    private void CreateCao()
    {
        //顶部的艹
        GameObject EF = ResourceManager.Instance.LoadAssetBundleUI("UI/Resident/FX_UI/UpsCao");
        EF.transform.SetParent(Screnes.gameObject.transform);
        EF.transform.localScale = new Vector3(1, 1, 1);
        RectTransform rect = EF.GetComponent<RectTransform>();
        rect.anchoredPosition3D = new Vector3(498.6f * catForm.ScreneHeight / SpriteHeight, -240 * catForm.ScreneHeight / SpriteHeight, 0);
        CreatFX.Add(EF);

        //中部的艹
        GameObject MEF = ResourceManager.Instance.LoadAssetBundleUI("UI/Resident/FX_UI/MiduleCao");
        MEF.transform.SetParent(Screnes.gameObject.transform);
        MEF.transform.localScale = new Vector3(1, 1, 1);
        RectTransform rectm = MEF.GetComponent<RectTransform>();
        rectm.anchoredPosition3D = new Vector3(1804.6f * catForm.ScreneHeight / SpriteHeight, -922.7f * catForm.ScreneHeight / SpriteHeight, 0);
        CreatFX.Add(MEF);


    }

    private void InitEvent()
    {
        UIEventListener.AddOnClickListener(arrowImage.gameObject, OnArrowClick);
        UIEventListener.AddOnClickListener(foodImg.gameObject, OnFoodImageClick);
        UIEventListener.AddOnClickListener(HouseImage.gameObject, HouseButtonOnclick);
        EventDispatcher.AddMessageListener(EventEnum.CatAdoptionSucc, CatAdoptionHandler);
    }

    /// <summary>
    /// 这个是收养成功后，屋子会闪动
    /// </summary>
    private void CatAdoptionHandler(Notification notification)
    {
        if (mSceneAnimView != null)
            mSceneAnimView.CatAdoptionSuccHandler((int)notification.Data);
        
        if (ContentRectTrans != null)
        {
            ContentRectTrans.DOAnchorPosX(0, 0.5f).SetDelay(1f).OnComplete(() =>
            {
                FlashCatHouse();
            }).Play();
        }
    }

    //猫屋闪动
    private void FlashCatHouse()
    {
        if (HouseImage == null) return;
        float time = 0.4f;
        HouseImage.DOFade(0, time).OnComplete(() =>
        {
            HouseImage.DOFade(1, time).OnComplete(() =>
            {
                HouseImage.DOFade(0, time).OnComplete(() =>
                {
                    HouseImage.DOFade(1, time);
                });
            });
        });
    }

    //获取猫场景数据
    private void InitCatDataInfo()
    {
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.PostGetSceneInfo(ProcessGetSceneInfo);
        GameHttpNet.Instance.PostGetYardInfo(ProcessGetYardInfo);
    }
    private void ProcessGetSceneInfo(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----ProcessGetSceneInfoCallback---->" + result);

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.SceneInfo = JsonHelper.JsonToObject<HttpInfoReturn<SceneInfo>>(result);
                    if (mSceneAnimView != null)
                        mSceneAnimView.InitSceneInfo();
                    UpdateFoodBox();
                    CheckAdoptCatInfo();

                    EventDispatcher.Dispatch(EventEnum.OpenCatGuid);//打开引导界面
                }
                else
                {
                    UITipsMgr.Instance.PopupTips(jo.msg, false);
                }

            }, null);
        }
    }

    private void ProcessGetYardInfo(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----ProcessGetYardInfo---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {

                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.YardInfo = JsonHelper.JsonToObject<HttpInfoReturn<YardInfo>>(result);
                    if(UserDataManager.Instance.YardInfo.data != null )
                    {
                        t_map tmpMap = GameDataMgr.Instance.table.GetCatInMapInfoBySort(UserDataManager.Instance.YardInfo.data.yard_data.id);
                        UpdateArrowPos(tmpMap);
                    }
                    UpdateBgMaskState(UserDataManager.Instance.YardInfo.data.yard_data.id);
                }
                else
                {
                    UITipsMgr.Instance.PopupTips(jo.msg, false);
                }

            }, null);
        }
    }

    private void UpdateBgMaskState(int vCurYardId)
    {
        if (vCurYardId >= 2 && Mask1IsShow)
        {
            BgMask1.DOColor(new Color(1, 1, 1, 0), 0.3f).Play();
            Mask1IsShow = false;
        }

        if (vCurYardId >= 3 && Mask2IsShow)
        {
            BgMask2.DOColor(new Color(1, 1, 1, 0), 0.3f).Play();
            Mask2IsShow = false;
        }
    }

    private void UpdateArrowPos(t_map tmpMap)
    {
        if (tmpMap == null|| arrowGroup==null) return;
        SetScrollRectSize(tmpMap);
        string x_pos = tmpMap.coordinates.Split(',')[0];
        string y_pos = tmpMap.coordinates.Split(',')[1];
        float xtmp = Convert.ToSingle(x_pos);
        float ytmp = Convert.ToSingle(y_pos);
        float x = xtmp * SpriteWidthChange / SpriteWidth;
        float y = ytmp * catForm.ScreneHeight / SpriteHeight;
        arrowGroup.transform.rectTransform().anchoredPosition3D = new Vector3(x, y, 0);
        arrowGroup.SetActive(true);
    }

    //检查，收养的猫，产出或状态，是否有更新
    private void CheckAdoptCatInfo()
    {
        if (UserDataManager.Instance.SceneInfo.data.adopt_change.Count > 0)
        {
            UserDataManager.Instance.GetChangValueList().Clear();
            List<adoptchange> tem = UserDataManager.Instance.SceneInfo.data.adopt_change;
            int len = tem.Count;

            for (int i = 0; i < len; i++)
            {
                int value = 0;
                adoptchange itemInfo = tem[i];
                if(itemInfo != null)
                    value = itemInfo.pid + itemInfo.change_status + itemInfo.intimacy + itemInfo.intimacy_new + itemInfo.story_new + itemInfo.love + itemInfo.diamond;

                UserDataManager.Instance.SaveCatValue(tem[i].pid, value);
            }

            if (UserDataManager.Instance.GetChangValueList().Count > 0)
            {
                CUIManager.Instance.OpenForm(UIFormName.CatWelcomBack);
            }
        }
    }

    private void UpdateFoodBox()
    {
        firstgift tmp = UserDataManager.Instance.SceneInfo.data.firstgift;
        CatFoodBowlGo.SetActive(true);
        //Debug.Log("------------------dfff11");
        if (tmp != null && tmp.shop_id != 0)
        {
            foodImg.sprite = ResourceManager.Instance.GetUISprite("CatSceneDecorations/" + tmp.shop_id);
        }

        if (UserDataManager.Instance.SceneInfo.data.usermoney.otherfood == 0)
        {
            foodImg.sprite = ResourceManager.Instance.GetUISprite("CatSceneDecorations/0");
            foodImg.SetNativeSize();
            return;
        }
        List<packarr> tmparr = UserDataManager.Instance.SceneInfo.data.packarr;
        for (int i = 0; i < tmparr.Count; i++)
        {
            if (tmparr[i].pay_type == 1)
            {
                food = tmparr[i];
                tfood = GameDataMgr.Instance.table.GetCatFoodById(tmparr[i].shop_id);
                break;
            }
        }
        if (tfood == null)
        {
            foodImg.sprite = ResourceManager.Instance.GetUISprite("CatSceneDecorations/0");
        }
        else
        {
            foodImg.sprite = ResourceManager.Instance.GetUISprite("CatSceneDecorations/" + tfood.id);
        }
        foodImg.SetNativeSize();
    }

    private void OnFoodImageClick(PointerEventData eventData)
    {
        if (UserDataManager.Instance.InPlaceCatThings != 0) return;
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.Getpetpackinfo(ProcessGetPackInfo);


        //这个是在引导点击食盒的步骤中，点击完食盒后进行下一步引导
        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.CatFoodOnGuid)
        {
            EventDispatcher.Dispatch(EventEnum.CatGuidUiClose);
        }
    }

    private void CatGuidRepair(Notification notification)
    {
        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.PlaceFoodYes)
        {
            if (UserDataManager.Instance.InPlaceCatThings != 0) return;
            GameHttpNet.Instance.Getpetpackinfo(ProcessGetPackInfo);

        }

        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.PlaceHuangyuandian)
        {
            
            t_shop shop = GameDataMgr.Instance.table.GetcatShopId(6);

            SpawnItemPos(shop);

            
        }
    }

    /// <summary>
    /// 获取背包数据的回调
    /// </summary>
    /// <param name="arg"></param>
    private void ProcessGetPackInfo(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----ProcessGetPackInfo---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.BagInfoContent = JsonHelper.JsonToObject<HttpInfoReturn<BagInfo>>(result);
                    if (UserDataManager.Instance.BagInfoContent.data.foodpack != null && UserDataManager.Instance.BagInfoContent.data.foodpack.Count > 0)
                    {
                        CUIManager.Instance.OpenForm(UIFormName.CatFoodSetForm);
                        CatFoodSetForm tmp = CUIManager.Instance.GetForm<CatFoodSetForm>(UIFormName.CatFoodSetForm);
                        if (tmp != null)
                        {
                            UserDataManager.Instance.BagInfoContent.data.foodpack.Sort(SortFoodItem);
                            tmp.SetFood(UserDataManager.Instance.BagInfoContent.data.foodpack);
                        }
                    }
                    else
                    {
                        CUIManager.Instance.OpenForm(UIFormName.CatPublicForm);
                        WindowInfo tmpWin = new WindowInfo("Tips", "There is No more food In your bag would you go to shop?", "", "Yes",
                            PublicNoButtonCallBacke, PublicYesButtonCallback, 1, null, null, null);
                        CUIManager.Instance.GetForm<CatPublicForm>(UIFormName.CatPublicForm).Inite(tmpWin);
                    }

                }
            }, null);
        }
    }

    private void PublicYesButtonCallback(string obj) { CUIManager.Instance.OpenForm(UIFormName.CatShop); }
    private void PublicNoButtonCallBacke(string obj) { }
    private void SetScrollRectSize(t_map tmpMap)
    {
        if (ContentRectTrans != null)
        {
            ContentRectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 750 * catForm.ScreneHeight / SpriteHeight - tmpMap.right * catForm.ScreneHeight / SpriteHeight);
        }
    }

    //扩展，场景
    private void OnArrowClick(PointerEventData eventData)
    {
        if (UserDataManager.Instance.InPlaceCatThings != 0) return;
        CUIManager.Instance.OpenForm(UIFormName.CatPublicForm);
        string Str = "Do you want to spend <color=#22f2ff>{0}</color> diamond to up grade the yard ?";

        string tmpContent;
        string tmpAnimalStr;
        string tmpSpaceStr;
        if (UserDataManager.Instance.YardInfo.data.yard_next_data != null && UserDataManager.Instance.YardInfo.data.yard_next_data.id != 0)
        {
            tmpContent = string.Format(Str, UserDataManager.Instance.YardInfo.data.yard_next_data.diamond_qty);
            tmpAnimalStr = string.Format("Cat +{0}", UserDataManager.Instance.YardInfo.data.yard_next_data.space_qty /*UserDataManager.Instance.YardInfo.data.yard_next_data.pet_qty*/);
            tmpSpaceStr = string.Format("Space +{0}", UserDataManager.Instance.YardInfo.data.yard_next_data.space_qty);
            WindowInfo tmpWin = new WindowInfo("Extend", "", "", "Yes", ExtendNoButtonCallBacke, ExtendYesButtonCallback, 2,
                tmpAnimalStr, tmpSpaceStr, UserDataManager.Instance.YardInfo.data.yard_next_data.diamond_qty.ToString(), tmpContent);
            CUIManager.Instance.GetForm<CatPublicForm>(UIFormName.CatPublicForm).Inite(tmpWin);
        }
        else
        {
            UITipsMgr.Instance.PopupTips("U R Upgraded !", false);
        }
    }
    private void ExtendNoButtonCallBacke(string obj) { }
    private void ExtendYesButtonCallback(string obj){GameHttpNet.Instance.PostUpgradeYard(ProcessExtend);}
    //扩展院子，信息返回
    private void ProcessExtend(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----ProcessExtend---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {

                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.goodinfo = JsonHelper.JsonToObject<HttpInfoReturn<GoodsInfo>>(result);
                    UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.goodinfo.data.diamond);
                    arrowImage.transform.DOLocalMoveY(-180, 0.4f).OnComplete(() =>
                    {
                        ReSpawnArrow(UserDataManager.Instance.YardInfo);
                    });

                    //院子扩充完毕，出现提示  上 176
                    NewPosPrompt.SetActive(true);
                    Text PromptCont = NewPosPrompt.transform.Find("PromptText").GetComponent<Text>();
                    PromptCont.text = GameDataMgr.Instance.table.GetLocalizationById(123);
                    NewPosPrompt.transform.DOLocalMoveY(-159,1.5f).SetEase(Ease.InOutBack).OnComplete(()=> {
                        Invoke("PosPromptReturn", 2.5f);
                    });
                }
                else if (jo.code == 204)
                {
                    CUIManager.Instance.OpenForm(UIFormName.ChargeTipsForm);
                    ChargeTipsForm tipForm = CUIManager.Instance.GetForm<ChargeTipsForm>(UIFormName.ChargeTipsForm);

                    //CUIManager.Instance.OpenForm(UIFormName.NewChargeTips);
                    //NewChargeTips tipForm = CUIManager.Instance.GetForm<NewChargeTips>(UIFormName.NewChargeTips);


                    if (tipForm != null)
                    {
                        tipForm.Init(2, UserDataManager.Instance.YardInfo.data.yard_next_data.diamond_qty, 1 * 0.99f);
                        tipForm.CatTopMainChange();
                    }
                }
                else
                {
                    UITipsMgr.Instance.PopupTips(jo.msg, false);
                }

            }, null);
        }
    }

    private void PosPromptReturn()
    {
        CancelInvoke("PosPromptReturn");
        NewPosPrompt.transform.DOLocalMoveY(176, 0.5f).OnComplete(()=> {
            NewPosPrompt.SetActive(false);
        });
    }

    // 展示扩展场景
    private void ReSpawnArrow(HttpInfoReturn<YardInfo> yardInfo)
    {
        if (yardInfo.data.yard_next_data == null && UserDataManager.Instance.YardInfo.data.yard_next_data.id != 0) return;
       
        t_map tmpMap = GameDataMgr.Instance.table.GetCatInMapInfoBySort(yardInfo.data.yard_next_data.id);
        if (tmpMap == null) return;
        SetScrollRectSize(tmpMap);
        if (ContentRectTrans != null)
        {
            float rectWidthDis = 750 - (750 * catForm.ScreneHeight / SpriteHeight - tmpMap.right * catForm.ScreneHeight / SpriteHeight);
            ContentRectTrans.DOAnchorPosX(rectWidthDis, 0.5f).OnComplete(() =>
            {
                UpdateArrowPos(tmpMap);
                ShowNewOpenPos(yardInfo.data.yard_next_data.id);
                UpdateBgMaskState(yardInfo.data.yard_next_data.id);
                arrowImage.transform.DOLocalMoveY(0, 0.4f);

            }).Play();
        }

        GameHttpNet.Instance.PostGetYardInfo(UpdataYardInfo);
    }

    //获得扩展后，猫的院子信息
    private void UpdataYardInfo(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----UpdataYardInfo---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                    UserDataManager.Instance.YardInfo = JsonHelper.JsonToObject<HttpInfoReturn<YardInfo>>(result);
                else
                    UITipsMgr.Instance.PopupTips(jo.msg, false);
            }, null);
        }
    }

    private void ShowNewOpenPos(int vCurYardId)
    {
        if (mNewPosGoList == null)
            mNewPosGoList = new List<GameObject>();

        List<t_map> tempMapList = GameDataMgr.Instance.table.GetCatMapPosListByLevel(vCurYardId);
        int len = tempMapList.Count;
        for (int i = 0; i < len; i++)
        {
            t_map itemMap = tempMapList[i];
            GameObject go;
            if (mNewPosGoList.Count > i)
            {
                go = mNewPosGoList[i];
                go.name = "newPos_" + i;
                go.transform.SetSiblingIndex(Screnes.gameObject.transform.childCount - 1);
            }
            else
            {
                go = Instantiate(decorationPosPrefab);
                go.transform.SetParent(Screnes.gameObject.transform);
                go.name = "newPos_" + i;
                mNewPosGoList.Add(go);
            }

            go.transform.localScale = new Vector3(GetDecorationObjScale(itemMap.type), GetDecorationObjScale(itemMap.type), GetDecorationObjScale(itemMap.type));
            RectTransform rect = go.GetComponent<RectTransform>();
            string x_pos = itemMap.coordinates.Split(',')[0];
            string y_pos = itemMap.coordinates.Split(',')[1];
            float xtmp = Convert.ToSingle(x_pos);
            float ytmp = Convert.ToSingle(y_pos);
            RectTransform Rect = Screnes.GetComponent<RectTransform>();
            rect.anchoredPosition3D = new Vector3(xtmp * catForm.ScreneHeight / SpriteHeight, ytmp * catForm.ScreneHeight / SpriteHeight, 0);
            Image posImage = go.GetComponent<Image>();
            posImage.color = new Color(1, 1, 1, 0);
            posImage.gameObject.SetActive(true);
            posImage.DOColor(new Color(1, 1, 1, 1), 0.5f).SetDelay(0.3f).OnComplete(() =>
            {
                posImage.DOColor(new Color(1, 1, 1, 0), 0.6f).SetDelay(0.7f).OnComplete(() =>
                {
                    posImage.gameObject.SetActive(false);
                    posImage.DOKill();
                }).Play();
            }).Play();
        }
    }

    /// <summary>
    /// 放置装饰物
    /// </summary>
    /// <param name="shop"></param>
    public void SpawnItemPos(t_shop shop)
    {
        t_shopGuid = shop;
        if (mSceneAnimView != null)
        {
            //UINetLoadingMgr.Instance.Show();
            GameHttpNet.Instance.PostGetYardInfo(ProcessGetYardInfoGuid);

        }

       
    }

    private void ProcessGetYardInfoGuid(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----ProcessGetYardInfo---->" + result);

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {

                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.YardInfo = JsonHelper.JsonToObject<HttpInfoReturn<YardInfo>>(result);

                    mSceneAnimView.SpawnItemPos(t_shopGuid);
                }
               
            }, null);
        }
    }
    /// <summary>
    /// 主动放置食物
    /// </summary>
    /// <param name="shop"></param>
    /// <param name="packid"></param>
    public void SetFood(t_shop shop, int packid)
    {
        if (foodPosObjList == null) foodPosObjList = new List<GameObject>();

         GameObject go;
        if (foodPosObjList.Count > 0)
        {
            go = foodPosObjList[0];
            go.gameObject.SetActive(true);
        }
        else
        {
            go = Instantiate(decorationPosPrefab);
            go.transform.SetParent(Screnes.gameObject.transform);
            go.gameObject.SetActive(true);
            UIEventListener.AddOnClickListener(go, OnPosItemFoodClick);
            foodPosObjList.Add(go);
        }

        go.SetActive(true);
        go.transform.localScale = new Vector3(1, 1, 1);
        RectTransform rect = go.GetComponent<RectTransform>();
        RectTransform Rect = Screnes.GetComponent<RectTransform>();
        rect.anchoredPosition3D = new Vector3(338f * catForm.ScreneHeight / SpriteHeight, -1066f * catForm.ScreneHeight / SpriteHeight, 0);
        tfood = GameDataMgr.Instance.table.GetCatFoodById(shop.shopid);
        catForm.SetBackBtnState(true);
        UserDataManager.Instance.InPlaceCatThings = 1;
    }

    private void OnPosItemFoodClick(PointerEventData eventData)
    {
        if (UserDataManager.Instance.InPlaceCatThings != 1) return;
        ResetdecorationPosPrefab();
        int id;
        if (food == null)
        {
            id = 0;
        }
        else
        {
            id = food.id;
        }
        GameHttpNet.Instance.PostPlaceItem("100", tfood.id.ToString(), ProcessPlaceFood);
    }
    /// <summary>
    /// 放置食物回调
    /// </summary>
    /// <param name="arg"></param>
    private void ProcessPlaceFood(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----ProcessPlaceFood---->" + result);

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                UserDataManager.Instance.InPlaceCatThings = 0;
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    catForm.SetBackBtnState(false);
                    CatFoodBowlGo.SetActive(false);
                    CatFoodBowlGo.SetActive(true);

                    //Debug.Log("------------------dfff11");
                    foodImg.sprite = ResourceManager.Instance.GetUISprite("CatSceneDecorations/" + tfood.id);
                    foodImg.SetNativeSize();
                    var tmpTopForm = CUIManager.Instance.GetForm<CatTopMain>(UIFormName.CatTop);
                    if (tmpTopForm)
                    {
                        tmpTopForm.RefreshDiamondAndHeart(null, null, "100");
                    }
                    GameHttpNet.Instance.PostGetSceneInfo(RefreshSceneData);
                }
                else
                {
                    UITipsMgr.Instance.PopupTips(jo.msg, false);

                }
            }, null);
        }
    }
    public void RefreshFoodImg(string id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            //Debug.Log("------------------dfff11");
            CatFoodBowlGo.SetActive(false);
            CatFoodBowlGo.SetActive(true);

            foodImg.sprite = ResourceManager.Instance.GetUISprite("CatSceneDecorations/" + id);
            GameHttpNet.Instance.PostGetSceneInfo(RefreshSceneData);
        }
    }

    /// <summary>
    /// 刷新场景数据
    /// </summary>
    /// <param name="arg"></param>
    private void RefreshSceneData(object arg)
    {
        string result = arg.ToString();

        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {

                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.SceneInfo = JsonHelper.JsonToObject<HttpInfoReturn<SceneInfo>>(result);
                }

            }, null);
        }
    }

    /// <summary>
    /// 根据size获取装饰物的缩放大小；
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    private float GetDecorationObjScale(int size)
    {
        float tmp = 1.0f;
        if (size == 1)
            tmp = 1.0f;
        else if (size == 2)
            tmp = 1.5f;
        return tmp;
    }

    //食物排序
    private int SortFoodItem(FoodBagItem x, FoodBagItem y)
    {
        if (x.count > 0 && y.count > 0)
        {
            if (x.shop_id > y.shop_id)
                return -1;
            else
                return 1;
        }
        else if (x.count > 0 && y.count == 0)
            return -1;
        else if (x.count == 0 && y.count > 0)
            return 1;
        else
        {
            if (x.shop_id > y.shop_id)
                return -1;
            else
                return 1;
        }
    }

    public void ResetdecorationPosPrefab()
    {
        UserDataManager.Instance.InPlaceCatThings = 0;
        if (foodPosObjList != null)
        {
            int len = foodPosObjList.Count;
            for (int i = 0; i < len; i++)
            {
                var tmpGo = foodPosObjList[i];
                if (tmpGo != null)
                    tmpGo.SetActive(false);
            }
        }
        if (mSceneAnimView != null)
            mSceneAnimView.ResetdecorationPosPrefab();
    }

    public override void CloseUi()
    {
        //界面关闭的时候释放内存
        Screnes.sprite = null;
        BgMask1.sprite = null;
        BgMask2.sprite = null;
        foodImg.sprite = null;
        HouseImage.sprite = null;
        StonTerracesImg.sprite = null;

        UIEventListener.RemoveOnClickListener(foodImg.gameObject, OnFoodImageClick);
        UIEventListener.RemoveOnClickListener(arrowImage.gameObject, OnArrowClick);
        UIEventListener.RemoveOnClickListener(HouseImage.gameObject, HouseButtonOnclick);
        EventDispatcher.RemoveMessageListener(EventEnum.CatAdoptionSucc, CatAdoptionHandler);

        EventDispatcher.RemoveMessageListener(EventEnum.SetGuidPos, SetGuidPos);

        EventDispatcher.RemoveMessageListener(EventEnum.CatGuidRepair, CatGuidRepair);

        DisposeOpenPosShowList();
        DisposeDecorationPosPrefab();

        if (CreatFX!=null)
        {
            for (int i = 0; i < CreatFX.Count; i++)
            {
               Destroy(CreatFX[i]);
            }
            CreatFX = null;
        }

        if (mSceneAnimView != null)
            mSceneAnimView.Dispose();
    }

    private void DisposeOpenPosShowList()
    {
        if (mNewPosGoList != null)
        {
            int len = mNewPosGoList.Count;
            for (int i = 0; i < len; i++)
            {
                GameObject.Destroy(mNewPosGoList[i]);
            }
        }
        mNewPosGoList = null;
    }

    private void DisposeDecorationPosPrefab()
    {
        if (foodPosObjList != null)
        {
            for (int i = 0; i < foodPosObjList.Count; i++)
            {
                var tmpGo = foodPosObjList[i];
                if (tmpGo)
                {
                    UIEventListener.RemoveOnClickListener(tmpGo, OnPosItemFoodClick);
                    Destroy(tmpGo);
                    tmpGo = null;
                }
            }
            foodPosObjList = null;
        }
    }

    /// <summary>
    /// 设置引导点击的位置
    /// </summary>
    /// <param name="notice"></param>
    public void SetGuidPos(Notification notice)
    {
        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.CatFoodOnGuid)
        {
            UserDataManager.Instance.GuidPos = new Vector3(340f * catForm.ScreneHeight / SpriteHeight, catForm.ScreneHeight - 1047 * catForm.ScreneHeight / SpriteHeight, 0);
            LOG.Info("得到食碗的坐标");
        }      
    }
}

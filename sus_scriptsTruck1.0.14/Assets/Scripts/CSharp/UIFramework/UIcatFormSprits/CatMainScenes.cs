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

public class CatMainScenes : CatClase
{

    private GameObject houseButton;
    private Image Screnes,BgMask1,BgMask2;
    private Image HouseImage;
    private Image foodImg;
    private Image StonTerracesImg;
    private GameObject CatPrefba;
    private List<GameObject> CatPrefbaList;
    private GameObject decorationPosPrefab;
    private List<GameObject> posObjList;
    private List<GameObject> foodPosObjList;
    private Dictionary<string, List<GameObject>> SetDecorationDic;//自己放置的装饰物字典
    private t_shop shopItem;
    private packarr food;
    private GameObject BackBtn;
    private Dictionary<int, int> PidShopIdPairDic;
    t_food tfood;
    Coroutine co = null;
    private CatMainForm catForm;
    private float SpriteWidth, SpriteHeight;//获得原来图片的长宽高
    string clickItem;
    string res;
    private Dictionary<int, t_map> mapData;//装饰物放置位置相关;
    private int catId;
    private Dictionary<int, GameObject> DecorationDic;//场景上已经放置的装饰物;
    private float SpriteWidthChange;//场景适配了屏幕后变成的宽度
    public GameObject content;
    private List<GameObject> HitDecoration;//保存已经屏蔽掉的装饰物
    private List<GameObject> mNewPosGoList;//扩展院子时，展示新的装饰物位置
    private int mTimeQueue = 0;
    private GameObject arrowPrefab, arrowImage;
    private bool Mask1IsShow = true;
    private bool Mask2IsShow = true;
    private GameObject HouseGame;
   
    public override void Bind(CatMainForm catmainform)
    {      
        //catmainform.catScren = this;
        catForm = catmainform;
        Inite();
       
        //存储猫装饰表中的数据入字典
        GameDataMgr.Instance.table.catShopData();

        GameDataMgr.Instance.table.catDecorationData();
        GameDataMgr.Instance.table.GetCatFoodData();
        GameDataMgr.Instance.table.GetCatGifData();
        GameDataMgr.Instance.table.GetCatStoryData();
        GameDataMgr.Instance.table.GetCatAttributeData();

        SpawnHouse();
        SpawnStonTerraces();
        if (posObjList == null)
        {
            posObjList = new List<GameObject>();
        }
        if (foodPosObjList == null)
            foodPosObjList = new List<GameObject>();

        HitDecoration = new List<GameObject>();
        HitDecoration.Clear();

       
        //if (mTimeQueue > 0)
        //    CTimerManager.Instance.RemoveTimer(mTimeQueue);
        //int flushTime = 10 * 60 * 1000;
        //flushTime = 5 * 1000;
        //mTimeQueue = CTimerManager.Instance.AddTimer(flushTime, -1, OnTimeUpHandler);
    }

    //private void OnTimeUpHandler(int vQueue)
    //{
    //    GameHttpNet.Instance.PostGetSceneInfo(ProcessGetSceneInfo);
    //}

    private void SpawnHouse()
    {
        GameObject go = Instantiate(decorationPosPrefab);
        go.transform.SetParent(Screnes.gameObject.transform);
        go.SetActive(true);

        go.transform.localScale = Vector3.one;
        go.name = "CatHouse";
        HouseGame = go;

        RectTransform rect = go.GetComponent<RectTransform>();
        RectTransform Rect = Screnes.GetComponent<RectTransform>();
        rect.anchoredPosition3D = new Vector3(403f * catForm.ScreneHeight / SpriteHeight, -232 * catForm.ScreneHeight / SpriteHeight, 0);
        HouseImage = go.GetComponent<Image>();
        HouseImage.sprite = ResourceManager.Instance.GetUISprite("CatSceneDecorations/house");
        HouseImage.SetNativeSize();

        houseButton = go;
        UIEventListener.AddOnClickListener(houseButton, HouseButtonOnclick);
    }
    
    private void HouseButtonOnclick(PointerEventData data)
    {
        if (UserDataManager.Instance.InPlaceCatThings != 0) return;
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CUIManager.Instance.OpenForm(UIFormName.Cattery);

        
    }

    /// <summary>
    /// 这个是收养成功后，屋子会闪动
    /// </summary>
    private void HouseAplaToChange(Notification notification)
    {
        float time = 0.3f;
        Image houseImage = HouseGame.GetComponent<Image>();

        houseImage.DOFade(0, time).OnComplete(()=>{
            houseImage.DOFade(1, time).OnComplete(()=> {
                houseImage.DOFade(0, time).OnComplete(()=> {
                    houseImage.DOFade(1, time);
                });
            });
        });

    }

    //石台(食物底座)
    private void SpawnStonTerraces()
    {
        GameObject go = Instantiate(decorationPosPrefab);
        go.transform.SetParent(Screnes.gameObject.transform);
        go.SetActive(true);

        go.transform.localScale = Vector3.one;
        go.name = "FoodStonTerraces";
        RectTransform rect = go.GetComponent<RectTransform>();
        RectTransform Rect = Screnes.GetComponent<RectTransform>();
        rect.anchoredPosition3D = new Vector3(337f * catForm.ScreneHeight / SpriteHeight, -1087 * catForm.ScreneHeight / SpriteHeight, 0);
        StonTerracesImg = go.GetComponent<Image>();
        StonTerracesImg.sprite = ResourceManager.Instance.GetUISprite("CatSceneDecorations/foodstands");
        StonTerracesImg.SetNativeSize();
    }

    //食物
    private void SpawnFood()
    {
        if (foodImg == null)
        {
            GameObject go = Instantiate(decorationPosPrefab);
            go.transform.SetParent(Screnes.gameObject.transform);
            go.SetActive(true);

            go.transform.localScale = new Vector3(1, 1, 1);
            go.name = "CatFoodBowl";
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchoredPosition3D = new Vector3(340f * catForm.ScreneHeight / SpriteHeight, -1047 * catForm.ScreneHeight / SpriteHeight, 0);
            foodImg = go.GetComponent<Image>();
            UIEventListener.AddOnClickListener(foodImg.gameObject, OnFoodImageClick);
        }

        firstgift tmp = UserDataManager.Instance.SceneInfo.data.firstgift;
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

    public override void CloseUi()
    {
        UserDataManager.Instance.InPlaceCatThings = 0;
        //界面关闭的时候释放内存
        Screnes.sprite = null;
        BgMask1.sprite = null;
        BgMask2.sprite = null;
        DisposeDecorationPosPrefab();
        DisposeDecorationPrefab();
        DisposeOpenPosShowList();
        if (mapData != null)
        {
            mapData.Clear();
        }
        if (posObjList != null)
        {
            posObjList.Clear();
        }
        if (DecorationDic != null)
        {
            DecorationDic.Clear();
        }
        if (SetDecorationDic != null)
        {
            SetDecorationDic.Clear();
        }

        if (PidShopIdPairDic != null)
        {
            PidShopIdPairDic.Clear();
        }

        if (CatPrefbaList!=null)
        {
            for (int i=0;i< CatPrefbaList.Count;i++)
            {
                CatPrefbaList[i].GetComponent<CatClothForm>().Dispose();

                Destroy(CatPrefbaList[i]);
            }
        }

        foodImg.sprite = null;
        HouseImage.sprite = null;
        StonTerracesImg.sprite = null;

        UIEventListener.RemoveOnClickListener(foodImg.gameObject, OnFoodImageClick);
        UIEventListener.RemoveOnClickListener(houseButton, HouseButtonOnclick);

        houseButton = null;
        if (co != null)
        {
            StopCoroutine(co);
            co = null;
        }
    }

    private void Inite()
    {
        
        Screnes = transform.Find("Viewport/Content/Scenes").GetComponent<Image>();
        BgMask1 = Screnes.transform.Find("Mask1").GetComponent<Image>();
        BgMask2 = Screnes.transform.Find("Mask2").GetComponent<Image>();
        CatPrefba = transform.Find("Viewport/Content/Scenes/CatCloth").gameObject;

        decorationPosPrefab = transform.Find("Viewport/Content/Scenes/decorationPosPrefab").gameObject;

        arrowPrefab = transform.Find("Viewport/Content/Scenes/arrowPrefab").gameObject;
       
        Screnes.sprite = ABSystem.ui.GetUITexture(AbTag.Global,"assets/Bundle/CatPreview/Screnes/1.png");
        Screnes.SetNativeSize();

        BgMask1.sprite = ABSystem.ui.GetUITexture(AbTag.Global,"assets/Bundle/CatPreview/Screnes/Mask1.png");
        BgMask1.SetNativeSize();

        BgMask2.sprite = ABSystem.ui.GetUITexture(AbTag.Global,"assets/Bundle/CatPreview/Screnes/Mask2.png");
        BgMask2.SetNativeSize();

        RectTransform Rect = Screnes.GetComponent<RectTransform>();
        SpriteWidth = Rect.rect.width;
        SpriteHeight = Rect.rect.height;
        GameDataMgr.Instance.table.GetMapData();
        //这里是使得不同图片的尺寸，以屏幕的高度为准做适应性。
        GameHttpNet.Instance.PostGetSceneInfo(ProcessGetSceneInfo);
        GameHttpNet.Instance.PostGetYardInfo(ProcessGetYardInfo);
        SpriteWidthChange = SpriteWidth * catForm.ScreneHeight / SpriteHeight;
        float rate = catForm.ScreneHeight / SpriteHeight;
        Rect.sizeDelta = new Vector2((SpriteWidth * rate), catForm.ScreneHeight);

        RectTransform mask1Trans = BgMask1.rectTransform();
        RectTransform mask2Trans = BgMask2.rectTransform();

        mask1Trans.sizeDelta = new Vector2(mask1Trans.rect.width * rate, mask1Trans.rect.height * rate);
        mask2Trans.sizeDelta = new Vector2(mask2Trans.rect.width * rate, mask2Trans.rect.height * rate);

        mask1Trans.anchoredPosition = new Vector2(mask1Trans.anchoredPosition.x * rate, mask1Trans.anchoredPosition.y * rate);
        mask2Trans.anchoredPosition = new Vector2(mask2Trans.anchoredPosition.x * rate, mask2Trans.anchoredPosition.y * rate);
        Mask1IsShow = true;
        Mask2IsShow = true;
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
                    SpawnArrow(UserDataManager.Instance.YardInfo);
                    UpdateBgMaskState(UserDataManager.Instance.YardInfo.data.yard_data.id);
                }
                else
                {
                    UITipsMgr.Instance.PopupTips(jo.msg, false);
                }

            }, null);
        }
    }

    private void SpawnArrow(HttpInfoReturn<YardInfo> yardInfo)
    {
        if (yardInfo.data.yard_next_data == null)
        {
            return;
        }
        t_map tmpMap = GameDataMgr.Instance.table.GetCatInMapInfoBySort(yardInfo.data.yard_data.id);
        if (tmpMap == null)
        {
            //LOG.Error("找不到装饰物");
           return;
        }
        SetScrollRectSize(tmpMap);
        GameObject go = Instantiate(arrowPrefab);
        go.transform.SetParent(Screnes.gameObject.transform);
        go.name = "arrow";
        go.transform.localScale = new Vector3(tmpMap.proportion, tmpMap.proportion, tmpMap.proportion);

        arrowImage = go.transform.Find("arrowImage").gameObject;


        //Image tmpImage = go.GetComponent<Image>();
        //tmpImage.sprite = ResourceManager.Instance.GetUISprite("CatSceneDecorations/arrow");//TDOD
        //tmpImage.SetNativeSize();

        //rect.anchoredPosition3D=new Vector3(520, -365,0);
        string x_pos = tmpMap.coordinates.Split(',')[0];
        string y_pos = tmpMap.coordinates.Split(',')[1];
        float xtmp = Convert.ToSingle(x_pos);
        float ytmp = Convert.ToSingle(y_pos);
        //LOG.Error("ScreneHeight / SpriteHeight=====" + catForm.ScreneHeight / SpriteHeight);
        float x = xtmp * SpriteWidthChange / SpriteWidth;
        float y = ytmp * catForm.ScreneHeight / SpriteHeight;
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition3D = new Vector3(x, y, 0);
        go.SetActive(true);
        if (DecorationDic == null)
        {
            DecorationDic = new Dictionary<int, GameObject>();
        }
        DecorationDic.Clear();
        DecorationDic.Add(10000, go);
        UIEventListener.AddOnClickListener(arrowImage.gameObject, OnArrowClick);
    }

    private void OnArrowClick(PointerEventData eventData)
    {
        if (UserDataManager.Instance.InPlaceCatThings != 0) return;
        CUIManager.Instance.OpenForm(UIFormName.CatPublicForm);
        string Str = "Do you want to spend <color=#22f2ff>{0}</color> diamond to up grade the yard ?";
        
        string tmpContent;
        string tmpAnimalStr;
        string tmpSpaceStr;
        if (UserDataManager.Instance.YardInfo.data.yard_next_data !=null && UserDataManager.Instance.YardInfo.data.yard_next_data.id != 0)
        {
            tmpContent = string.Format(Str, UserDataManager.Instance.YardInfo.data.yard_next_data.diamond_qty);
            tmpAnimalStr = string.Format("Animal +{0}", UserDataManager.Instance.YardInfo.data.yard_next_data.pet_qty);
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

    private void ExtendYesButtonCallback(string obj)
    {
        GameHttpNet.Instance.PostUpgradeYard(ProcessExtend);
    }

    private void ProcessExtend(object arg)
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
                    UserDataManager.Instance.goodinfo = JsonHelper.JsonToObject<HttpInfoReturn<GoodsInfo>>(result);
                    var tmpTopForm = CUIManager.Instance.GetForm<CatTopMain>(UIFormName.CatTop);
                    if (tmpTopForm)
                    {
                        //tmpTopForm.RefreshDiamondAndHeart(UserDataManager.Instance.goodinfo.data.diamond.ToString(), UserDataManager.Instance.goodinfo.data.love.ToString(), null);

                        //tmpTopForm.RefreshDiamond(2, UserDataManager.Instance.goodinfo.data.diamond);

                        UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.goodinfo.data.diamond);
                    }

                    arrowImage.transform.DOLocalMoveY(-180, 0.4f).OnComplete(()=>{
                        ReSpawnArrow(UserDataManager.Instance.YardInfo);
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
    private void ReSpawnArrow(HttpInfoReturn<YardInfo> yardInfo)
    {
        if (yardInfo.data.yard_next_data == null && UserDataManager.Instance.YardInfo.data.yard_next_data.id != 0)
        {
            return;
        }
        t_map tmpMap = GameDataMgr.Instance.table.GetCatInMapInfoBySort(yardInfo.data.yard_next_data.id);
        if (tmpMap == null)
        {
            //LOG.Error("找不到装饰物");
            return;
        }
        SetScrollRectSize(tmpMap);
        if(content != null)
        {
            RectTransform contRectTrans = content.transform.rectTransform();
            float rectWidthDis = 750 - (750 * catForm.ScreneHeight / SpriteHeight - tmpMap.right * catForm.ScreneHeight / SpriteHeight);
            //Debug.Log("------------->" + rectWidthDis);
            contRectTrans.DOAnchorPosX(rectWidthDis, 0.5f).OnComplete(() =>
            {
                ShowNewOpenPos(yardInfo.data.yard_next_data.id);
                UpdateBgMaskState(yardInfo.data.yard_next_data.id);
                arrowImage.transform.DOLocalMoveY(0, 0.4f);

            }).Play();
        }
        
        GameObject go;
        if (DecorationDic == null || DecorationDic.Count == 0)
        {
            go = Instantiate(arrowPrefab);
        }
        else
        {
            go = GetDecorationObj(10000);
            if (!go)
            {
                go = Instantiate(arrowPrefab);
            }
        }
        go.transform.SetParent(Screnes.gameObject.transform);
        go.name = "arrow";
        go.transform.localScale = new Vector3(tmpMap.proportion, tmpMap.proportion, tmpMap.proportion);
        //Image tmpImage = go.GetComponent<Image>();
        //tmpImage.sprite = ResourceManager.Instance.GetUISprite("CatSceneDecorations/arrow");//TDOD
        //tmpImage.SetNativeSize();

        //rect.anchoredPosition3D=new Vector3(520, -365,0);
        string x_pos = tmpMap.coordinates.Split(',')[0];
        string y_pos = tmpMap.coordinates.Split(',')[1];
        float xtmp = Convert.ToSingle(x_pos);
        float ytmp = Convert.ToSingle(y_pos);
        //LOG.Error("ScreneHeight / SpriteHeight=====" + catForm.ScreneHeight / SpriteHeight);
        float x = xtmp * SpriteWidthChange / SpriteWidth;
        float y = ytmp * catForm.ScreneHeight / SpriteHeight;
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition3D = new Vector3(x, y, 0);
        GameHttpNet.Instance.PostGetYardInfo(UpdataYardInfo);
        go.SetActive(true);
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

    private void ShowNewOpenPos(int vCurYardId)
    {
        if (mNewPosGoList == null)
            mNewPosGoList = new List<GameObject>();

        List<t_map> tempMapList = GameDataMgr.Instance.table.GetCatMapPosListByLevel(vCurYardId);
        int len = tempMapList.Count;
        for(int i = 0;i<len;i++)
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
                go.name = "newPos_"+i;
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
            posImage.DOColor(new Color(1,1,1,1), 0.5f).SetDelay(0.3f).OnComplete(()=> {
                posImage.DOColor(new Color(1, 1, 1, 0),0.6f).SetDelay(0.7f).OnComplete(() => {
                    posImage.gameObject.SetActive(false);
                    posImage.DOKill();
                }).Play();
            }).Play();
        }
    }

    private void SetScrollRectSize(t_map tmpMap)
    {
        if (content)
        {
            //LOG.Info("等级是======" + tmpMap.sort+"------>"+(750 * catForm.ScreneHeight / SpriteHeight - tmpMap.right * catForm.ScreneHeight / SpriteHeight));
            RectTransform tmptrans = content.GetComponent<RectTransform>();

            tmptrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 750* catForm.ScreneHeight / SpriteHeight - tmpMap.right* catForm.ScreneHeight / SpriteHeight);
        }
    }

    private void UpdataYardInfo(object arg)
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
                }
                else
                {
                    UITipsMgr.Instance.PopupTips(jo.msg, false);
                }

            }, null);
        }
    }

    private void ExtendNoButtonCallBacke(string obj)
    {
        
    }

    private void OnFoodImageClick(PointerEventData eventData)
    {
        if (UserDataManager.Instance.InPlaceCatThings != 0) return;
        GameHttpNet.Instance.Getpetpackinfo(ProcessGetPackInfo);
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

    private int SortFoodItem(FoodBagItem x, FoodBagItem y)
    {
        if (x.count > 0 && y.count > 0)
        {
            if (x.shop_id > y.shop_id)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
        else if (x.count > 0 && y.count == 0)
        {
            return -1;
        }
        else if (x.count == 0 && y.count > 0)
        {
            return 1;
        }
        else
        {
            if (x.shop_id > y.shop_id)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

    }

   
    private void PublicYesButtonCallback(string obj)
    {
        CUIManager.Instance.OpenForm(UIFormName.CatShop);
    }

    private void PublicNoButtonCallBacke(string obj)
    {

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
                    SpawnDecorations();
                    SpawnCat();
                    SpawnFood();

                    List<int> ListInt = new List<int>();
                    int ListAllNum = 0;

                    if (UserDataManager.Instance.SceneInfo.data.adopt_change.Count>0)
                    {

                            List<adoptchange> tem= UserDataManager.Instance.SceneInfo.data.adopt_change;

                            UserDataManager.Instance.GetChangValueList().Clear();


                            for (int i=0;i< tem.Count;i++)
                            {
                            int value = 0;    

                                ListInt.Add(tem[i].pid);
                                ListInt.Add(tem[i].change_status);
                                ListInt.Add(tem[i].intimacy);
                                ListInt.Add(tem[i].intimacy_new);
                                ListInt.Add(tem[i].story_new);
                                ListInt.Add(tem[i].love);
                                ListInt.Add(tem[i].diamond);

                            value = tem[i].pid + tem[i].change_status + tem[i].intimacy + tem[i].intimacy_new+ tem[i].story_new + tem[i].love + tem[i].diamond;

                            UserDataManager.Instance.SaveCatValue(tem[i].pid, value);
                           }
                      
                            if (UserDataManager.Instance.GetChangValueList().Count==0)
                            {
                                //这两list相同
                                //Debug.Log("2222");
                            }
                            else
                            {
                                //这两list不相同
                            
                                //打开动物结算界面
                                CUIManager.Instance.OpenForm(UIFormName.CatWelcomBack);
                           
                                ListInt = null;
                            }
                     
                    }               
                }
                else
                {
                    UITipsMgr.Instance.PopupTips(jo.msg, false);
                }

            }, null);
        }
    }

   

    private void SpawnDecorations()
    {
        if (UserDataManager.Instance.SceneInfo == null)
        {
            LOG.Error("无場景数据！");
        }
        if (DecorationDic == null)
        {
            DecorationDic = new Dictionary<int, GameObject>();
        }
        List<packarr> tmparr = UserDataManager.Instance.SceneInfo.data.packarr;
        for (int i = 0; i < tmparr.Count; i++)
        {
            int place;
            int.TryParse(tmparr[i].place, out place);
            t_map tmpMap = GameDataMgr.Instance.table.GetcatMapInfoById(place);
            if (tmpMap == null)
            {
                //LOG.Error("找不到装饰物");
                continue;
            }
            GameObject go;
            if(!DecorationDic.ContainsKey(tmparr[i].shop_id))
            {
                go = Instantiate(decorationPosPrefab);
                go.transform.SetParent(Screnes.gameObject.transform);
                go.name = tmparr[i].place;
                go.transform.localScale = new Vector3(tmpMap.proportion, tmpMap.proportion, tmpMap.proportion);
              
                //Debug.LogError("1tmpMap.proportion:" + tmpMap.proportion + "==tmpMap.proportion:" + tmpMap.proportion);

                go.transform.gameObject.SetActive(true);
            }else
            {
                go = DecorationDic[tmparr[i].shop_id];
                go.name = tmparr[i].place;

                //Debug.LogError("2tmpMap.proportion:"+ tmpMap.proportion+ "==tmpMap.proportion:"+ tmpMap.proportion);

                go.transform.localScale = new Vector3(tmpMap.proportion, tmpMap.proportion, tmpMap.proportion);
                go.transform.gameObject.SetActive(true);
            }

            Image tmpImage = go.GetComponent<Image>();
            tmpImage.sprite = ResourceManager.Instance.GetUISprite("CatSceneDecorations/" + GameDataMgr.Instance.table.GetcatDecorationResByShopId(tmparr[i].shop_id));//TDOD
            tmpImage.SetNativeSize();

            //rect.anchoredPosition3D=new Vector3(520, -365,0);
            string x_pos = tmpMap.coordinates.Split(',')[0];
            string y_pos = tmpMap.coordinates.Split(',')[1];
            float xtmp = Convert.ToSingle(x_pos);
            float ytmp = Convert.ToSingle(y_pos);
            //LOG.Error("ScreneHeight / SpriteHeight=====" + catForm.ScreneHeight / SpriteHeight);
            float x = xtmp * SpriteWidthChange / SpriteWidth;
            float y = ytmp * catForm.ScreneHeight / SpriteHeight;
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchoredPosition3D = new Vector3(x, y, 0);
            go.SetActive(true);
            if (!DecorationDic.ContainsKey(tmparr[i].shop_id))
            {
                DecorationDic.Add(tmparr[i].shop_id, go);
            }
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
        {
            tmp = 1.0f;
        }
        else if (size == 2)
        {
            tmp = 1.5f;
        }
        return tmp;
    }
    /// <summary>
    /// 根据装饰物ID获取场景上的装饰物
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private GameObject GetDecorationObj(int id)
    {
        GameObject go = null;
        if (DecorationDic == null || !DecorationDic.GetEnumerator().MoveNext())
        {
            LOG.Error("字典为空");
            go = null;
        }
        if (DecorationDic.ContainsKey(id))
        {
            go = DecorationDic[id];
        }
        else
        {
            LOG.Error("找不到该Id的装饰物");
            go = null;
        }
        return go;
    }

    /// <summary>
    /// 生成猫的位置
    /// </summary>
    private void SpawnCat()
    {
        if (UserDataManager.Instance.SceneInfo == null)
        {
            LOG.Error("无宠物数据！");
        }
        if (PidShopIdPairDic == null)
        {
            PidShopIdPairDic = new Dictionary<int, int>();
        }
        List<firstpetarr> tmparr = UserDataManager.Instance.SceneInfo.data.firstpetarr;

        if (CatPrefbaList == null)
            CatPrefbaList = new List<GameObject>();

        for (int i = 0; i < tmparr.Count; i++)
        {
            GameObject go;
            if(CatPrefbaList != null && CatPrefbaList.Count>i)
            {
                go = CatPrefbaList[i];
                go.gameObject.SetActive(true);
            }else
            {
                go = Instantiate(CatPrefba);
                go.gameObject.SetActive(true);
                CatPrefbaList.Add(go);
            }

            go.GetComponent<CatClothForm>().Inite(tmparr[i]);
            go.name = tmparr[i].pid + "_cat";
            catId = tmparr[i].pid+10086;
            GameObject tmpObj = GetDecorationObj(tmparr[i].shop_id);//一個裝飾ID只對應一個裝飾物
            if (tmpObj)
            {
                go.transform.SetParent(tmpObj.transform);
            }
            go.transform.localScale = new Vector3(1f, 1f, 1f);
            t_contrast tmpContent = GameDataMgr.Instance.table.GetcaIntMapInfoById(tmparr[i].pid, tmparr[i].decorations_id);
            if (tmpContent != null)
            {
                string tmpStr = tmpContent.param1.Replace("[", "");
                string tmpStr1 = tmpStr.Replace("]", "");
                float tmpX = Convert.ToSingle(tmpStr1.Split(',')[0]);
                float tmpY = Convert.ToSingle(tmpStr1.Split(',')[1]);
                RectTransform rect = go.GetComponent<RectTransform>();
                rect.anchoredPosition3D = new Vector3(tmpX, tmpY, 0);
                Image tmpImage = go.transform.GetComponent<Image>();
                PlayCatFrame(tmpImage, tmparr[i].pid, UserDataManager.Instance.GetCatAtion(tmparr[i].pid));
                if (tmpContent.display != 0)//屏蔽某些需要屏蔽的装饰物;
                {
                    GameObject decor = GetDecorationObj(tmpContent.display);
                    if (decor)
                    {
                        go.transform.SetParent(Screnes.transform);
                        decor.SetActive(false);

                        HitDecoration.Add(decor);
                    }
                }
                if (!DecorationDic.ContainsKey(catId))
                {
                    LOG.Info("存储的猫是" + tmparr[i].pid + "装饰物是" + tmparr[i].decorations_id);
                    DecorationDic.Add(catId, go);
                }
                if (!PidShopIdPairDic.ContainsKey(catId))
                    PidShopIdPairDic.Add(catId, tmparr[i].shop_id);
            }
            go.SetActive(true);
        }
    }
    
   

    /// <summary>
    /// 当成功收养了宠物之后，把已经隐藏的装饰物设置为可见
    /// </summary>
    /// <param name="notification"></param>
    private void HitDectionToShow(Notification notification)
    {
        for (int i = 0; i < HitDecoration.Count; i++)
        {
            if (HitDecoration[i]!=null)
            {
                HitDecoration[i].SetActive(true);
            }
        }
        HitDecoration.Clear();
    }

    /// <summary>
    /// 生成放置引导位置
    /// </summary>
    /// <param name="shop"></param>
    /// <param name="id"></param>
    public void SpawnItemPos(t_shop shop)
    {
        shopItem = shop;
        res = GameDataMgr.Instance.table.GetcatDecorationResByShopId(shopItem.shopid);
        mapData = GameDataMgr.Instance.table.GetCatMapDic();
        if (mapData == null || !mapData.GetEnumerator().MoveNext())
        {
            LOG.Error("地图资源为空");
            return;

        }
        UserDataManager.Instance.InPlaceCatThings = 2;
        var yardData = UserDataManager.Instance.YardInfo;
        catForm.SetBackBtnState(true);
        int tempIndex = 0;
        foreach (KeyValuePair<int, t_map> kvp in mapData)
        {
            GameObject go;
            if(posObjList.Count > tempIndex)
            {
                go = posObjList[tempIndex];
                go.name = kvp.Key.ToString();
                go.transform.SetSiblingIndex(Screnes.gameObject.transform.childCount - 1);
            }
            else
            {
                go = Instantiate(decorationPosPrefab);
                go.transform.SetParent(Screnes.gameObject.transform);
                go.name = kvp.Key.ToString();
                posObjList.Add(go);
                UIEventListener.AddOnClickListener(go, OnPosItemClick);
            }
            go.SetActive(false);
            if (kvp.Value.sort <= yardData.data.yard_data.id)//策划要求对应的屏蔽
            {
                if (shop.size != 2 )
                {

                    if (kvp.Value.type != 3)
                    {
                        go.SetActive(true);
                        go.transform.localScale = new Vector3(GetDecorationObjScale(kvp.Value.type), GetDecorationObjScale(kvp.Value.type), GetDecorationObjScale(kvp.Value.type));
                        RectTransform rect = go.GetComponent<RectTransform>();
                        string x_pos = kvp.Value.coordinates.Split(',')[0];
                        string y_pos = kvp.Value.coordinates.Split(',')[1];
                        float xtmp = Convert.ToSingle(x_pos);
                        float ytmp = Convert.ToSingle(y_pos);
                        RectTransform Rect = Screnes.GetComponent<RectTransform>();
                        rect.anchoredPosition3D = new Vector3(xtmp * catForm.ScreneHeight / SpriteHeight, ytmp * catForm.ScreneHeight / SpriteHeight, 0);
                        tempIndex++;
                    }
                }
                else
                {
                    if (kvp.Value.type != 1 && kvp.Value.type != 3)
                    {
                        go.SetActive(true);
                        go.transform.localScale = new Vector3(GetDecorationObjScale(kvp.Value.type), GetDecorationObjScale(kvp.Value.type), GetDecorationObjScale(kvp.Value.type));
                        RectTransform rect = go.GetComponent<RectTransform>();
                        string x_pos = kvp.Value.coordinates.Split(',')[0];
                        string y_pos = kvp.Value.coordinates.Split(',')[1];
                        float xtmp = Convert.ToSingle(x_pos);
                        float ytmp = Convert.ToSingle(y_pos);
                        RectTransform Rect = Screnes.GetComponent<RectTransform>();
                        rect.anchoredPosition3D = new Vector3(xtmp * catForm.ScreneHeight / SpriteHeight, ytmp * catForm.ScreneHeight / SpriteHeight, 0);
                        tempIndex++;
                    }
                }
            }
        }
    }

    public void SetFood(t_shop shop, int packid)
    {
        GameObject go;
        if(foodPosObjList.Count > 0)
        {
            go = foodPosObjList[0];
            go.gameObject.SetActive(true);
        }else
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
        GameHttpNet.Instance.PostPlaceItem( "100", tfood.id.ToString(), ProcessPlaceFood);
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
            foodImg.sprite = ResourceManager.Instance.GetUISprite("CatSceneDecorations/" + id);
            GameHttpNet.Instance.PostGetSceneInfo(RefreshSceneData);
        }
    }
    private void OnPosItemClick(PointerEventData eventData)
    {
        if (UserDataManager.Instance.InPlaceCatThings != 2) return;
        clickItem = eventData.pointerPress.name;
        LOG.Info("---------OnPosItemClick---------->" + clickItem);
        GameHttpNet.Instance.PostPlaceItem(clickItem,shopItem.shopid.ToString(),   ProcessPlaceItem);


    }
    /// <summary>
    /// 放置回调
    /// </summary>
    /// <param name="arg"></param>
    private void ProcessPlaceItem(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetpetgiftinfoCallbacke---->" + result);

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
                    GameObject go = Instantiate(decorationPosPrefab);
                    go.transform.SetParent(Screnes.gameObject.transform);
                    t_map tmp = GameDataMgr.Instance.table.GetcatMapInfoById(int.Parse(clickItem));
                    Image tmpImage = go.GetComponent<Image>();
                    if (tmpImage)
                    {
                        tmpImage.sprite = ResourceManager.Instance.GetUISprite("CatSceneDecorations/" + res);//TDOD
                        tmpImage.SetNativeSize();
                    }
                    if (tmp != null)
                    {
                        RectTransform rect = go.GetComponent<RectTransform>();
                        string x_pos = tmp.coordinates.Split(',')[0];
                        string y_pos = tmp.coordinates.Split(',')[1];
                        float xtmp = Convert.ToSingle(x_pos);
                        float ytmp = Convert.ToSingle(y_pos);
                        rect.anchoredPosition3D = new Vector3(xtmp * catForm.ScreneHeight / SpriteHeight, ytmp * catForm.ScreneHeight / SpriteHeight, 0);
                        go.SetActive(true);

                        go.transform.localScale = new Vector3(tmp.proportion, tmp.proportion, tmp.proportion);

                    }
                    if (SetDecorationDic== null)
                    {
                        SetDecorationDic = new Dictionary<string, List<GameObject>>();
                    }
                    if (!SetDecorationDic.ContainsKey(clickItem))//存放自己放置的物件;
                    {
                        List<GameObject> tmpList = new List<GameObject>();
                        tmpList.Add(go);
                        SetDecorationDic.Add(clickItem, tmpList);

                    }
                    else
                    {
                        SetDecorationDic[clickItem].Add(go);
                    }
                    ResetdecorationPosPrefab();
                    RefreshDecoration(go);
                    GameHttpNet.Instance.PostGetSceneInfo(RefreshSceneData);
                }
                else
                {
                    UITipsMgr.Instance.PopupTips(jo.msg, false);
                }


            }, null);
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
    /// s刷新场景上的物品显示
    /// </summary>
    /// <param name="go"></param>
    private void RefreshDecoration(GameObject go)
    {
        packarr tmp = GetDecorationPackageInfoByid(clickItem);
        if (tmp != null)
        {
            if (DecorationDic.ContainsKey(tmp.shop_id))
            {
                DecorationDic[tmp.shop_id].SetActive(false);
            }
            foreach (var item in PidShopIdPairDic)
            {
                if (item.Value == tmp.shop_id)
                {
                    DecorationDic[item.Key].SetActive(false);//根据对应关系将猫隐藏
                }
            }
        }
       
        

        if (SetDecorationDic == null)
        {
            return;
        }
        if (SetDecorationDic.ContainsKey(clickItem))
        {
            foreach (var item in SetDecorationDic[clickItem])
            {
                if (item.GetHashCode() != go.GetHashCode())
                {
                    item.SetActive(false);
                }
            }
        }

    }

    public void ResetdecorationPosPrefab()
    {
        UserDataManager.Instance.InPlaceCatThings = 0;
        if (posObjList != null)
        {
            int len = posObjList.Count;
            for (int i = 0; i < len; i++)
            {
                var tmpGo = posObjList[i];
                if (tmpGo != null)
                    tmpGo.SetActive(false);
            }
        }

        if(foodPosObjList != null)
        {
            int len = foodPosObjList.Count;
            for (int i = 0; i < len; i++)
            {
                var tmpGo = foodPosObjList[i];
                if (tmpGo != null)
                    tmpGo.SetActive(false);
            }
        }
        
    }
    private void DisposeDecorationPosPrefab()
    {
        if (posObjList != null)
        {
            for (int i = 0; i < posObjList.Count; i++)
            {
                var tmpGo = posObjList[i];
                if (tmpGo)
                {
                    Destroy(tmpGo);
                    tmpGo = null;
                }
            }
            posObjList = null;
        }

        if (foodPosObjList != null)
        {
            for (int i = 0; i < foodPosObjList.Count; i++)
            {
                var tmpGo = foodPosObjList[i];
                if (tmpGo)
                {
                    Destroy(tmpGo);
                    tmpGo = null;
                }
            }
            foodPosObjList = null;
        }
        
    }

    private void DisposeDecorationPrefab()
    {
        if (DecorationDic == null || DecorationDic.Count == 0)
        {
            return;
        }
        foreach (var item in DecorationDic)
        {
            if (item.Value)
            {
                Destroy(item.Value);
            }
        }
        DecorationDic = null;
        
    }

    private void DisposeOpenPosShowList()
    {
        if(mNewPosGoList!= null)
        {
            int len = mNewPosGoList.Count;
            for(int i = 0;i<len;i++)
            {
                GameObject.Destroy(mNewPosGoList[i]);
            }
        }
        mNewPosGoList = null;
    }

    /// <summary>
    /// 根据装饰物位置获取背包装饰物
    /// </summary>
    /// <param name="place_id">位置ID</param>
    /// <returns></returns>
    public packarr GetDecorationPackageInfoByid(string place_id)
    {
        packarr tmpInfo = null;
        List<packarr> arr = UserDataManager.Instance.SceneInfo.data.packarr;
        if (arr == null || arr.Count == 0)
        {
            LOG.Error("无装饰物数据");//表示沒有替換操作
            tmpInfo = null;
        }
        else
        {
            for (int i = 0; i < arr.Count; i++)
            {
                if (arr[i].place == place_id)
                {
                    tmpInfo = arr[i];
                }
            }
        }
        return tmpInfo;

    }
    /// <summary>
    /// 播放猫的序列帧
    /// </summary>
    /// <param name="image"></param>
    /// <param name="cid">猫宠物ID</param>
    /// <param name="aid">动作ID</param>
    /// <param name="frameCount">帧数</param>
    public void PlayCatFrame(Image image, int cid, int aid)
    {
        LOG.Error("猫："+cid+"--正在播放的动作："+aid);

        t_gif tmpGif = GameDataMgr.Instance.table.GetCatGifById(cid, aid);
        StringBuilder sb = new StringBuilder();
        sb.Append("assets/Bundle/Cat/");
        sb.Append(cid.ToString("D2"));
        sb.Append(aid.ToString("D2"));
        if (tmpGif == null)
        {
            LOG.Info("找不到Gif数据");//該動作只有一幀
            sb.Append("01");
            image.sprite = ABSystem.ui.GetUITexture(AbTag.Global,sb.ToString() + ".png");
            image.SetNativeSize();
            return;
        }
        int frameCount = 0;
        var tmp = tmpGif.frame.Split(',');
        frameCount = tmp.Length;

        if (frameCount == 1)
        {
            sb.Append(frameCount.ToString("D2"));
            image.sprite = ABSystem.ui.GetUITexture(AbTag.Global,sb.ToString() + ".png");
            image.SetNativeSize();
            return;
        }
        else
        {
            co = StartCoroutine(AnimationPlayThread(image, cid, aid, tmpGif, frameCount));

        }
    }



    /// <summary>
    /// 播放序列帧
    /// </summary>
    /// <param name="img">目标图片</param>
    /// <param name="cid">宠物ID</param>
    /// <param name="aid">动作ID</param>
    /// <param name="gif">动作数据</param>
    /// <param name="count">帧数</param>
    /// <returns></returns>
    IEnumerator AnimationPlayThread(Image img, int cid, int aid, t_gif gif, int count)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("assets/Bundle/Cat/");
        sb.Append(cid.ToString("D2"));
        sb.Append(aid.ToString("D2"));
        int i = 1;
        int tmploop = 0;
        int tmpmin = int.Parse(gif.num.Split(',')[0]);
        int tmpmax = int.Parse(gif.num.Split(',')[1]);
        int loop = GetRandom(tmpmin, tmpmax);//獲取循環次數
        int tmp_min = int.Parse(gif.frequency.Split(',')[0]);
        int tmp_max = int.Parse(gif.frequency.Split(',')[1]);
        int frequence = GetRandom(tmp_min, tmp_max);//跟据表里數據獲取播放頻率
        string[] id = gif.frame.Split(',');
        bool flag = true;
        while (flag)
        {
            if (i <=count)
            {
                int picid = int.Parse(id[i - 1]);
                if (img!=null)
                {
                    img.sprite = ABSystem.ui.GetUITexture(AbTag.Global,sb.ToString() + picid.ToString("D2") + ".png");
                    img.SetNativeSize();

                }
            }
            else
            {
                i = 1;
                int picid = int.Parse(id[i - 1]);
                if (img!=null)
                {
                    img.sprite = ABSystem.ui.GetUITexture(AbTag.Global,sb.ToString() + picid.ToString("D2") + ".png");
                    img.SetNativeSize();
                    tmploop += 1;
                    if (tmploop == loop)
                    {
                        tmploop = 0;
                        yield return new WaitForSeconds(frequence);
                    }
                }                     
            }

            string[] tmpsec = gif.time.Split(',');
            float tmpflo = Convert.ToSingle(tmpsec[i - 1]);//每一幀的控制時間
            i++;
            yield return new WaitForSeconds(tmpflo);
        }
    }
    /// <summary>
    /// 获取随机数
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns></returns>
    private int GetRandom(int min, int max)
    {

        return UnityEngine.Random.Range(min, max + 1);
    }

}

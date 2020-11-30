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
/// 场景动物展示（主要是放置装饰物、展示猫的动作）
/// </summary>
public class CatSceneAnimView : MonoBehaviour
{
    private GameObject mDecoratePrefab, mCatPrefab,mItemPosPrefab;
    private RectTransform mSceneTrans;

    private Dictionary<int, t_map> mapData;//装饰物放置位置相关;
    private Dictionary<int, GameObject> DecorateDic;
    private List<ScenePosItemInfoView> posObjList;
    private List<CatClothForm> CatPrefbaList;
    private float SpriteWidth, SpriteHeight;
    private float SpriteWidthNext, SpriteHeightNext;//缩放后背景的大小
    private int mMapId;

    private t_shop shopItem;
    private Queue<string> fx_18, fx_20, fx_CatFoodBowl, fx_23;
    private GameObject PlaceGuidGameobj;
    private bool OpenCatGuid = false;
    private bool FirstSpwanCat = false;

    private ScenePosItemInfoView catPosInfoVItem;

    public void Init(GameObject vDecoratePrefab,GameObject vCatPrefab,GameObject vItemPosPrefab,RectTransform vSceneTrans, float SpriteWidth, float SpriteHeight, float SpriteWidthNext, float SpriteHeightNext)
    {
        mDecoratePrefab = vDecoratePrefab;
        mCatPrefab = vCatPrefab;
        mItemPosPrefab = vItemPosPrefab;
        mSceneTrans = vSceneTrans;

        this.SpriteWidth = SpriteWidth /*vSceneTrans.rect.width*/;//原来背景的宽度
        this.SpriteHeight = SpriteHeight /*vSceneTrans.rect.height*/;//原来背景的高度

        this.SpriteWidthNext = SpriteWidthNext;//缩放后背景的宽度
        this.SpriteHeightNext = SpriteHeightNext;//缩放后背景的高度

        EventDispatcher.AddMessageListener(EventEnum.GuidSpwanCat, GuidSpwanCat);
        EventDispatcher.AddMessageListener(EventEnum.DestroyGuidCat, DestroyGuidCat);

        EventDispatcher.AddMessageListener(EventEnum.CatGuidRepair, CatGuidRepair);

       
    }

    //初始化，场景信息
    public void InitSceneInfo()
    {
        fx_18 = new Queue<string>();
        fx_18.Enqueue("18-a");
        fx_18.Enqueue("11-b");
        fx_18.Enqueue("6-a");
        fx_18.Enqueue("19-a");
        fx_18.Enqueue("24-a");
        fx_18.Enqueue("23-b");
        fx_18.Enqueue("15-a");
        fx_18.Enqueue("14-a");
        fx_18.Enqueue("21-a");
        fx_18.Enqueue("3-c");
        fx_18.Enqueue("3-b");
        fx_18.Enqueue("3-d");
        fx_18.Enqueue("1-a");
        fx_18.Enqueue("14-b");
        fx_18.Enqueue("24-b");
        fx_18.Enqueue("16-a");

        fx_20 = new Queue<string>();
        fx_20.Enqueue("20-a");
        fx_20.Enqueue("17-c");
        fx_20.Enqueue("17-a");
        fx_20.Enqueue("17-b");
        fx_20.Enqueue("7-a");

        fx_CatFoodBowl = new Queue<string>();
        fx_CatFoodBowl.Enqueue("10-a");
        fx_CatFoodBowl.Enqueue("11-a");
        fx_CatFoodBowl.Enqueue("3-a");
        fx_CatFoodBowl.Enqueue("2-a");
        fx_CatFoodBowl.Enqueue("1-b");
        fx_CatFoodBowl.Enqueue("6-b");
        fx_CatFoodBowl.Enqueue("9-a");
        fx_CatFoodBowl.Enqueue("6-c");
        fx_CatFoodBowl.Enqueue("13-a");
        fx_CatFoodBowl.Enqueue("16-b");

        fx_23 = new Queue<string>();
        fx_23.Enqueue("23-a");
        fx_23.Enqueue("12-a");
        fx_23.Enqueue("22-a");
        InitDecorate();
        InitCat();

        EventDispatcher.AddMessageListener(EventEnum.SetGuidPos, SetGuidPos);
       
    }

    //初始化装饰物
    public void InitDecorate()
    {
        if (UserDataManager.Instance.SceneInfo == null) return;
        if (DecorateDic == null) DecorateDic = new Dictionary<int, GameObject>();
        List<packarr> tmparr = UserDataManager.Instance.SceneInfo.data.packarr;
        for (int i = 0; i < tmparr.Count; i++)
        {
            int place;
            int.TryParse(tmparr[i].place, out place);
            t_map tmpMap = GameDataMgr.Instance.table.GetcatMapInfoById(place);
            if (tmpMap == null) continue;
            GameObject go;
            if (!DecorateDic.ContainsKey(tmparr[i].shop_id))
            {
                go = Instantiate(mDecoratePrefab);
                go.transform.SetParent(mSceneTrans.gameObject.transform);
            }
            else
            {
                go = DecorateDic[tmparr[i].shop_id];
            }

            go.name = tmparr[i].shop_id + "_Decorate";
            go.transform.localScale = new Vector3(tmpMap.proportion, tmpMap.proportion, tmpMap.proportion);
            go.SetActive(true);

            Image tmpImage = go.GetComponent<Image>();
            tmpImage.sprite = ResourceManager.Instance.GetUISprite("CatSceneDecorations/" + GameDataMgr.Instance.table.GetcatDecorationResByShopId(tmparr[i].shop_id));//TDOD
            tmpImage.SetNativeSize();
            go.transform.rectTransform().anchoredPosition3D = GetPosByStr(tmpMap.coordinates);


            if (!DecorateDic.ContainsKey(tmparr[i].shop_id))
            {
                DecorateDic.Add(tmparr[i].shop_id, go);
            }
        }
    }


    /// <summary>
    /// 生成猫的位置
    /// </summary>
    private void InitCat()
    {
        if (UserDataManager.Instance.SceneInfo == null) return;
        List<firstpetarr> tmparr = UserDataManager.Instance.SceneInfo.data.firstpetarr;
        if (CatPrefbaList == null)
            CatPrefbaList = new List<CatClothForm>();

        CatClothForm catItemInfoV;
        for (int i = 0; i < tmparr.Count; i++)
        {
            if (CatPrefbaList != null && CatPrefbaList.Count > i)
            {
                catItemInfoV = CatPrefbaList[i];
            }
            else
            {
                GameObject go = Instantiate(mCatPrefab);
                catItemInfoV = go.AddMissingComponent<CatClothForm>();
                CatPrefbaList.Add(catItemInfoV);
            }
            GameObject tmpObj = GetDecorateGo(tmparr[i].shop_id);//一個裝飾ID只對應一個裝飾物
            if (tmpObj)
            {
                catItemInfoV.transform.SetParent(tmpObj.transform);
            }

            catItemInfoV.gameObject.SetActive(true);
            catItemInfoV.Inite(tmparr[i]);
            catItemInfoV.transform.localScale = Vector3.one;

            t_contrast tmpContent = GameDataMgr.Instance.table.GetcaIntMapInfoById(tmparr[i].pid, tmparr[i].decorations_id);
            if (tmpContent != null)
            {
                string tmpStr = tmpContent.param1.Replace("[", "");
                string tmpStr1 = tmpStr.Replace("]", "");
                catItemInfoV.transform.rectTransform().anchoredPosition3D = GetPosByStr(tmpStr1);
                Image tmpImage = catItemInfoV.transform.GetComponent<Image>();
                if (tmpContent.display != 0)//一些猫是自带装饰物的,需要屏蔽原来的装饰物;
                {
                    GameObject decorateGo = GetDecorateGo(tmpContent.display);
                    if (decorateGo)
                    {
                        catItemInfoV.transform.SetParent(mSceneTrans.transform);
                        decorateGo.SetActive(false);
                    }
                }
            }
        }
    }

    private GameObject GuidCatSpwan;
    /// <summary>
    /// 引导阶段，调用这里来生成引导中默认的猫
    /// </summary>
    public void GuidSpwanCat(Notification notification)
    {
        if (FirstSpwanCat) return;

        LOG.Info("生成一只猫");
        GameObject go = Instantiate(mCatPrefab);
        go.gameObject.SetActive(true);
       
        GuidCatSpwan = go;
        FirstSpwanCat = true;

        GameObject tmpObj = GetDecorateGo(6);//一個裝飾ID只對應一個裝飾物
        if (tmpObj)
        {
            go.transform.SetParent(tmpObj.transform);
        }

        go.transform.localScale = new Vector3(1f, 1f, 1f);
        t_contrast tmpContent = GameDataMgr.Instance.table.GetcaIntMapInfoById(8, 1);
        if (tmpContent != null)
        {
            string tmpStr = tmpContent.param1.Replace("[", "");
            string tmpStr1 = tmpStr.Replace("]", "");
            go.transform.rectTransform().anchoredPosition3D = GetPosByStr(tmpStr1);
        }

        go.GetComponent<CatClothForm>().PlayCatFrame(8, UserDataManager.Instance.GetCatAtion(8));
    }

    /// <summary>
    /// 销毁新手引导的时候临时生成的猫
    /// </summary>
    /// <param name="notification"></param>
    private void DestroyGuidCat(Notification notification)
    {
        if (GuidCatSpwan == null) return;
        LOG.Info("销毁生成的猫:"+ GuidCatSpwan.name);
       
        Destroy(GuidCatSpwan);
    }

    /// <summary>
    /// 生成放置引导位置
    /// </summary>
    /// <param name="shop"></param>
    /// <param name="id"></param>
    public void SpawnItemPos(t_shop shop)
    {
        shopItem = shop;
        mapData = GameDataMgr.Instance.table.GetCatMapDic();
        if (mapData == null || !mapData.GetEnumerator().MoveNext())
        {
            LOG.Error("地图资源为空");
            return;
        }

        if (posObjList == null) posObjList = new List<ScenePosItemInfoView>();

        UserDataManager.Instance.InPlaceCatThings = 2;
        var yardData = UserDataManager.Instance.YardInfo;
        EventDispatcher.Dispatch(EventEnum.ChangeCatFormBackBtnState, true);
        int tempIndex = 0;
        catPosInfoVItem = null;
        foreach (KeyValuePair<int, t_map> kvp in mapData)
        {
            if (posObjList.Count > tempIndex)
            {
                catPosInfoVItem = posObjList[tempIndex];
                catPosInfoVItem.transform.SetSiblingIndex(mSceneTrans.transform.childCount - 1);
                catPosInfoVItem.MapInfo = kvp.Value;
                catPosInfoVItem.gameObject.name = catPosInfoVItem.MapInfo.id+ "_pos";
            }
            else
            {
                GameObject go = Instantiate(mItemPosPrefab);
                go.transform.SetParent(mSceneTrans.transform);
                catPosInfoVItem = go.AddMissingComponent<ScenePosItemInfoView>();
                catPosInfoVItem.MapInfo = kvp.Value;
                catPosInfoVItem.gameObject.name = catPosInfoVItem.MapInfo.id + "_pos";
                posObjList.Add(catPosInfoVItem);

                UIEventListener.AddOnClickListener(catPosInfoVItem.gameObject, OnPosItemClick);

            }
            catPosInfoVItem.gameObject.SetActive(false);

            if (kvp.Value.sort <= yardData.data.yard_data.id)//策划要求对应的屏蔽
            {
                if (shop.size != 2)
                {
                    if (kvp.Value.type != 3)
                    {
                        catPosInfoVItem.gameObject.SetActive(true);
                        catPosInfoVItem.transform.localScale = new Vector3(GetDecorationObjScale(kvp.Value.type), GetDecorationObjScale(kvp.Value.type), GetDecorationObjScale(kvp.Value.type));
                        Vector3 tempPos = GetPosByStr(kvp.Value.coordinates);
                        catPosInfoVItem.transform.rectTransform().anchoredPosition3D = new Vector3(tempPos.x,tempPos.y, 0);
                        tempIndex++;

                        OpenCatGuidPlaceHuangyuandian(catPosInfoVItem.gameObject);
                        //Debug.Log("dd11"+ "SpriteWidthNext:"+ SpriteWidthNext+ "--SpriteHeightNext:"+ SpriteHeightNext+ "--SpriteWidth:"+ SpriteWidth+ "--SpriteHeight:"+ SpriteHeight);
                    }
                }
                else
                {
                    if (kvp.Value.type != 1 && kvp.Value.type != 3)
                    {
                        catPosInfoVItem.gameObject.SetActive(true);
                        catPosInfoVItem.transform.localScale = new Vector3(GetDecorationObjScale(kvp.Value.type), GetDecorationObjScale(kvp.Value.type), GetDecorationObjScale(kvp.Value.type));
                        Vector3 tempPos = GetPosByStr(kvp.Value.coordinates);
                        catPosInfoVItem.transform.rectTransform().anchoredPosition3D = new Vector3(tempPos.x , tempPos.y, 0);
                        tempIndex++;
                        //Debug.Log("dd22");
                        OpenCatGuidPlaceHuangyuandian(catPosInfoVItem.gameObject);
                    }
                }
            }
        }
    }  
    private void OnPosItemClick(PointerEventData eventData)
    {
       
        if (UserDataManager.Instance.InPlaceCatThings != 2) return;
        ScenePosItemInfoView catPosInfoVItem = eventData.pointerPress.GetComponent<ScenePosItemInfoView>();
        if(catPosInfoVItem != null)
        {
            mMapId = catPosInfoVItem.MapInfo.id;

            packarr packItemInfo = GetDecorationPackageInfoByid(mMapId.ToString());
            GameObject go = null;
            Transform[] st = null;
            bool isHadCat = false;
            if (packItemInfo != null)
            {
                go = GetDecorateGo(packItemInfo.shop_id);
                st = go.GetComponentsInChildren<Transform>();

                for (int i=0;i< st.Length;i++)
                {
                    string Name = st[i].name;

                    string[] NameList = Name.Split('_');

                    if (NameList.Length>1&& NameList[1].Equals("cat"))
                    {
                        Debug.Log("有猫");
                        isHadCat = true;
                        break;
                    }
                }

            }
                     
            if (isHadCat && UserDataManager.Instance.CatPosReplace != DateTime.Now.DayOfYear)
            {
                isHadCat = false;
                //这个装饰物上有猫  打开提示界面，进行询问
                CUIManager.Instance.OpenForm(UIFormName.CatPublicForm);
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(118);
                string cont = Localization;//"确定要替换这个装饰物吗？上面的猫咪会被赶走哦！"   慎重考虑！
                WindowInfo tmpWin = new WindowInfo(GameDataMgr.Instance.table.GetLocalizationById(218), cont, GameDataMgr.Instance.table.GetLocalizationById(245), "Yes",
                          PublicNoButtonCallBacke, PublicYesButtonCallback, 5, null, null, null);
                CUIManager.Instance.GetForm<CatPublicForm>(UIFormName.CatPublicForm).Inite(tmpWin);

                st = null;
            }
            else
            {
                LOG.Info("---------OnPosItemClick---------->" + mMapId);
                GameHttpNet.Instance.PostPlaceItem(mMapId.ToString(), shopItem.shopid.ToString(), ProcessPlaceItemCallBack);
            }                  
        }

        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.PlaceHuangyuandian)
        {
            EventDispatcher.Dispatch(EventEnum.CatGuidUiClose);//关闭引导界面
        }
    }

    private void PublicYesButtonCallback(string ST)
    {
        if (ST.Equals("Yes"))
        {
            //勾选了不出现提示
            UserDataManager.Instance.CatPosReplace = DateTime.Now.DayOfYear;
        }

        //Debug.Log("共用界面YES按钮回调");
        LOG.Info("---------OnPosItemClick---------->" + mMapId);
        GameHttpNet.Instance.PostPlaceItem(mMapId.ToString(), shopItem.shopid.ToString(), ProcessPlaceItemCallBack);
    }
    private void PublicNoButtonCallBacke(string ST)
    {
        //Debug.Log("共用界面No按钮回调");

        if (ST.Equals("Yes"))
        {
            //勾选了不出现提示
            UserDataManager.Instance.CatPosReplace = DateTime.Now.DayOfYear;
        }     
    }

    /// <summary>
    /// 放置回调
    /// </summary>
    /// <param name="arg"></param>
    private void ProcessPlaceItemCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----ProcessPlaceItemCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                UserDataManager.Instance.InPlaceCatThings = 0;
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    EventDispatcher.Dispatch(EventEnum.ChangeCatFormBackBtnState, false);
                    packarr packItemInfo = GetDecorationPackageInfoByid(mMapId.ToString());
                    GameObject go = null;
                    if (packItemInfo != null)
                        go = GetDecorateGo(packItemInfo.shop_id);

                    
                    if(go == null)
                    {
                        go = Instantiate(mDecoratePrefab);
                        go.transform.SetParent(mSceneTrans.transform);
                    }
                    else
                    {
                        DecorateDic[packItemInfo.shop_id] = null;
                    }

                    if (DecorateDic.ContainsKey(shopItem.shopid))
                        DecorateDic[shopItem.shopid] = go;
                    else
                        DecorateDic.Add(shopItem.shopid, go);
                   
                    go.name = shopItem.shopid + "_Decorate";
                    Image tmpImage = go.GetComponent<Image>();
                    go.transform.localScale = Vector3.one;

                    GameObject ScaleObject = null;
                    if (tmpImage)
                    {

                       
                        string DecorationName = GameDataMgr.Instance.table.GetcatDecorationResByShopId(shopItem.shopid).ToString();

                        if (fx_18.Contains(DecorationName))
                        {
                            tmpImage.DOFade(0,0.1f);

                            go.transform.Find("fx_20").gameObject.SetActive(false);
                            go.transform.Find("fx_23").gameObject.SetActive(false);
                            go.transform.Find("fx_CatFoodBowl").gameObject.SetActive(false);

                            GameObject fxgo = go.transform.Find("fx_18").gameObject;
                            fxgo.SetActive(false);
                            fxgo.SetActive(true);
                           
                            Image Decorate = fxgo.transform.Find("Decorate").GetComponent<Image>();
                            Decorate.sprite= ResourceManager.Instance.GetUISprite("CatSceneDecorations/" + DecorationName);//TDOD
                            Decorate.SetNativeSize();
                            ScaleObject = Decorate.gameObject;
                            //Debug.Log("队列中包含了");                           
                        }
                        else if (fx_20.Contains(DecorationName))
                        {
                            tmpImage.DOFade(0, 0.1f);

                            go.transform.Find("fx_18").gameObject.SetActive(false);
                            go.transform.Find("fx_23").gameObject.SetActive(false);
                            go.transform.Find("fx_CatFoodBowl").gameObject.SetActive(false);

                            GameObject fxgo = go.transform.Find("fx_20").gameObject;
                            fxgo.SetActive(false);
                            fxgo.SetActive(true);
                           
                            Image Decorate = fxgo.transform.Find("Decorate").GetComponent<Image>();
                            Decorate.sprite = ResourceManager.Instance.GetUISprite("CatSceneDecorations/" + DecorationName);//TDOD
                            Decorate.SetNativeSize();
                            ScaleObject = Decorate.gameObject;
                            //Debug.Log("队列中包含了");
                        }
                        else if (fx_CatFoodBowl.Contains(DecorationName))
                        {
                            tmpImage.DOFade(0, 0.1f);

                            go.transform.Find("fx_18").gameObject.SetActive(false);
                            go.transform.Find("fx_23").gameObject.SetActive(false);
                            go.transform.Find("fx_20").gameObject.SetActive(false);

                            GameObject fxgo = go.transform.Find("fx_CatFoodBowl").gameObject;
                            fxgo.SetActive(false);
                            fxgo.SetActive(true);
                           
                            Image Decorate = fxgo.transform.Find("Decorate").GetComponent<Image>();
                            Decorate.sprite = ResourceManager.Instance.GetUISprite("CatSceneDecorations/" + DecorationName);//TDOD
                            Decorate.SetNativeSize();
                            ScaleObject = Decorate.gameObject;
                            //Debug.Log("队列中包含了");
                        }
                        else if (fx_23.Contains(DecorationName))
                        {
                            tmpImage.DOFade(0, 0.1f);
                            go.transform.Find("fx_18").gameObject.SetActive(false);
                            go.transform.Find("fx_CatFoodBowl").gameObject.SetActive(false);
                            go.transform.Find("fx_20").gameObject.SetActive(false);

                            GameObject fxgo = go.transform.Find("fx_23").gameObject;
                            fxgo.SetActive(false);
                            fxgo.SetActive(true);
                           
                            Image Decorate = fxgo.transform.Find("Decorate").GetComponent<Image>();
                            Decorate.sprite = ResourceManager.Instance.GetUISprite("CatSceneDecorations/" + DecorationName);//TDOD
                            Decorate.SetNativeSize();
                            ScaleObject = Decorate.gameObject;
                            //Debug.Log("队列中包含了");
                        }
                        else
                        {
                            tmpImage.DOFade(1, 0.1f);

                            go.transform.Find("fx_18").gameObject.SetActive(false);
                            go.transform.Find("fx_CatFoodBowl").gameObject.SetActive(false);
                            go.transform.Find("fx_20").gameObject.SetActive(false);
                            go.transform.Find("fx_23").gameObject.SetActive(false);

                          
                            tmpImage.sprite = ResourceManager.Instance.GetUISprite("CatSceneDecorations/" + GameDataMgr.Instance.table.GetcatDecorationResByShopId(shopItem.shopid));//TDOD
                            tmpImage.SetNativeSize();
                            ScaleObject = tmpImage.gameObject;
                        }                     
                    }
                    t_map tmp = GameDataMgr.Instance.table.GetcatMapInfoById(mMapId);
                    if (tmp != null)
                    {
                        Vector3 tempPos = GetPosByStr(tmp.coordinates);
                        go.transform.rectTransform().anchoredPosition3D = new Vector3(tempPos.x, tempPos.y, 0);
                        go.SetActive(true);

                        //Debug.Log("tmp.proportion:"+ tmp.proportion+"--Name:"+ ScaleObject.name);
                        ScaleObject.transform.localScale = new Vector3(tmp.proportion, tmp.proportion, tmp.proportion);
                    }
                    ResetdecorationPosPrefab();
                    RefreshDecoration();
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
        LOG.Info("----RefreshSceneData---->" + result);
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
    /// 猫收养成功
    /// </summary>
    /// <param name="CatId"></param>
    public void CatAdoptionSuccHandler(int vCatId)
    {
        if(CatPrefbaList != null)
        {
            int len = CatPrefbaList.Count;
            for (int i = 0; i < len; i++)
            {
                CatClothForm catItem = CatPrefbaList[i];
                if (catItem != null && catItem.CatId == vCatId)
                {
                    catItem.gameObject.SetActive(false);
                    if(DecorateDic.ContainsKey(catItem.ShopId))
                        DecorateDic[catItem.ShopId].SetActive(true);
                }
            }
        }
    }

     //隐藏放置未的显示
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
                    tmpGo.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// s刷新场景上的物品显示
    /// </summary>
    /// <param name="go"></param>
    private void RefreshDecoration()
    {
        packarr tmp = GetDecorationPackageInfoByid(mMapId.ToString());
        if (tmp != null)
        {
            int len = CatPrefbaList.Count;
            for(int i = 0;i<len;i++)
            {
                CatClothForm catItem = CatPrefbaList[i];
                if(catItem != null && catItem.ShopId == tmp.shop_id)
                {
                    catItem.gameObject.SetActive(false);
                }
            }
        }
    }

   

    /// <summary>
    /// 根据装饰物ID获取场景上的装饰物
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private GameObject GetDecorateGo(int id)
    {
        if (DecorateDic.ContainsKey(id))
        {
            return DecorateDic[id];
        }
        return null;
    }

    //返回坐标
    private Vector3 GetPosByStr(string vStr)
    {
        //tempPos.x* SpriteWidthNext / SpriteWidth, tempPos.y* SpriteHeightNext / SpriteHeight
        Vector3 pos = Vector3.zero;
        string[] strArr = vStr.Split(',');
        float xtmp = Convert.ToSingle(strArr[0]);
        float ytmp = Convert.ToSingle(strArr[1]);
        float x = xtmp*SpriteWidthNext /SpriteWidth;/*xtmp * SpriteWidthChange / SpriteWidth;*/
        float y = ytmp * SpriteHeightNext / SpriteHeight;/*ytmp * mCatFormSprHeight / SpriteHeight;*/
        pos = new Vector3(x, y, 0);
        return pos;
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


    public void Dispose()
    {
        EventDispatcher.RemoveMessageListener(EventEnum.SetGuidPos, SetGuidPos);
        
        EventDispatcher.RemoveMessageListener(EventEnum.GuidSpwanCat, GuidSpwanCat);

        EventDispatcher.RemoveMessageListener(EventEnum.DestroyGuidCat, DestroyGuidCat);

        EventDispatcher.RemoveMessageListener(EventEnum.CatGuidRepair, CatGuidRepair);

        DisposeDecorationPosPrefab();
        DisoseCat();
        DisposeDecorationPrefab();
       
        if (fx_18 != null)
            fx_18 = null;
        if (fx_20 != null)
            fx_20 = null;
        if (fx_23 != null)
            fx_23 = null;
        if (fx_CatFoodBowl != null)
            fx_CatFoodBowl = null;

    }

    private void DisposeDecorationPosPrefab()
    {
        if (posObjList != null)
        {
            int len = posObjList.Count;
            for (int i = 0; i < len; i++)
            {
                var tmpGo = posObjList[i];
                if (tmpGo)
                {
                    UIEventListener.RemoveOnClickListener(tmpGo.gameObject, OnPosItemClick);
                    Destroy(tmpGo);
                    tmpGo = null;
                }
            }
            posObjList = null;
        }
    }

    private void DisoseCat()
    {
        if (CatPrefbaList != null)
        {
            int len = CatPrefbaList.Count;
            for (int i = 0; i < len; i++)
            {
                var tmpGo = CatPrefbaList[i];
                if (tmpGo)
                {
                    tmpGo.Dispose();
                    tmpGo = null;
                }
            }
            CatPrefbaList = null;
        }
    }

    private void DisposeDecorationPrefab()
    {
        if (DecorateDic == null || DecorateDic.Count == 0)
        {
            return;
        }
        foreach (var item in DecorateDic)
        {
            if (item.Value)
            {
                Destroy(item.Value);
            }
        }
        DecorateDic = null;
    }

    /// <summary>
    /// 出现放置黄圆垫的引导界面
    /// </summary>
    private void OpenCatGuidPlaceHuangyuandian(GameObject go)
    {
        
        if (!OpenCatGuid && UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.PlaceHuangyuandian&& go.name.Equals("2_pos"))
        {
            OpenCatGuid = true;
            PlaceGuidGameobj = go;
            Invoke("OpenCatGuidInvoke",0.3f);
        }

        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.PlaceDecorationsTips && go.name.Equals("2_pos"))
        {
            PlaceGuidGameobj = go;
        }
    }

    private void OpenCatGuidInvoke()
    {
        CancelInvoke("OpenCatGuidInvoke");
        EventDispatcher.Dispatch(EventEnum.OpenCatGuid);//打开引导界面
    }

    /// <summary>
    /// 设置引导点击的位置
    /// </summary>
    /// <param name="notice"></param>
    public void SetGuidPos(Notification notice)
    {
        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.PlaceHuangyuandian|| UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.PlaceDecorationsTips || UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.SpwanCat || UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.CatCountdown)
        {
            if (PlaceGuidGameobj == null) return;
            RectTransform PlaceRect = PlaceGuidGameobj.GetComponent<RectTransform>();

            float Posx = PlaceRect.anchoredPosition.x;
            float Posy = SpriteHeightNext + PlaceRect.anchoredPosition.y;

            UserDataManager.Instance.GuidPos = new Vector3(Posx, Posy, 1);
            //LOG.Info("得到食物放置确定按钮的坐标");
        }
    }

    private void CatGuidRepair(Notification notification)
    {        
        if (UserDataManager.Instance.GuidFirstStupNum == (int)CatGuidEnum.PlaceDecorationsTips|| UserDataManager.Instance.GuidFirstStupNum == (int)CatGuidEnum.SpwanCat || UserDataManager.Instance.GuidFirstStupNum == (int)CatGuidEnum.CatCountdown || UserDataManager.Instance.GuidFirstStupNum == (int)CatGuidEnum.FeedbackTips)
        {
            GameObject go = transform.Find("Viewport/Content/Scenes/6_Decorate").gameObject;
            PlaceGuidGameobj = go;            
        }

        if (UserDataManager.Instance.GuidFirstStupNum == (int)CatGuidEnum.SpwanCat || UserDataManager.Instance.GuidFirstStupNum == (int)CatGuidEnum.CatCountdown)
        {
            if (!FirstSpwanCat)
            {
                LOG.Info("派发生成猫");
                EventDispatcher.Dispatch(EventEnum.GuidSpwanCat);//生成引导默认的猫
   
            }
             
        }
    }

}

public class ScenePosItemInfoView :MonoBehaviour
{
    public t_map MapInfo;
}
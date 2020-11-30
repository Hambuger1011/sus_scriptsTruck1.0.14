using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UGUI;

public class CatDecorationItem : MonoBehaviour {

    private Text Name, Count;
    private Image DecorationSprite;
    private GameObject Placed,Use;
    private Text PlacedText;
    t_shop ShopT;
    t_decorations decoration;
    private bool first = false;
    CatDecorationForm.TransitionItem transitionItem;
    int id;
    public GameObject SizeL, SizeS;
    private CatDecorationForm CatDecorationForm;
    public void Init(t_shop t_shop, CatDecorationForm.TransitionItem transitionItem, CatDecorationForm CatDecorationForm)
    {

        if (t_shop.shopid==6&&UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.DecorationsOnclick)
        {
            EventDispatcher.Dispatch(EventEnum.OpenCatGuid);
        }
       
        this.CatDecorationForm = CatDecorationForm;
        EventDispatcher.AddMessageListener(EventEnum.SetGuidPos, SetGuidPos);
        EventDispatcher.AddMessageListener(EventEnum.CatGuidRepair, CatGuidRepair);


        if (!first)
        {
            first = true;
            Name = transform.Find("Name").GetComponent<Text>();
            Count = transform.Find("Count").GetComponent<Text>();
            DecorationSprite = transform.Find("DecorationSprite").GetComponent<Image>();
            Placed = transform.Find("Placed").gameObject;
            Use= transform.Find("Use").gameObject;
            PlacedText = transform.Find("Placed/Text").GetComponent<Text>();
            //PlacedText.text = GameDataMgr.Instance.table.GetLocalizationById(208).text.ToString();

            UIEventListener.AddOnClickListener(gameObject,GameButtonOnclicke);
        }
        if (transitionItem != null)
        {
            if (transitionItem.isUsed != -1)
            {
                if (transitionItem.goods_type == 2)
                {
                    Placed.SetActive(transitionItem.isUsed == 1);

                    if (transitionItem.isUsed == 1)
                    {
                        Use.SetActive(false);
                    }else
                    {
                        Use.SetActive(true);
                    }
                }
                else
                {
                    Placed.SetActive( false);
                    //Use.SetActive(true);
                }
            }
        }
        SizeS.SetActive(false);
        SizeL.SetActive(false);
        Count.gameObject.SetActive(false);
        id = transitionItem.id;
        ShopT = t_shop;
        Name.text = t_shop.name.ToString();
        this.transitionItem = transitionItem;
        if (transitionItem.goods_type == 1 )
        {
            if (transitionItem.shopid != 1)
            {
                Count.gameObject.SetActive(true);
                Count.text = "x" + transitionItem.count.ToString();
            }

            
        }
        else
        {
            Count.gameObject.SetActive(false);
        }
        if (ShopT.size == 1 || ShopT.size == 2)
        {
            DecorationSprite.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/dec" + t_shop.shopid.ToString());
            DecorationSprite.material = null;
        }
        else
        {
            DecorationSprite.sprite = ResourceManager.Instance.GetUISprite("CatDecFoodIcon/" + t_shop.shopid.ToString());
            if (transitionItem.count == 0)
                DecorationSprite.material = ShaderUtil.GrayMaterial();
            else
                DecorationSprite.material = null;
        }

        SizeS.SetActive(t_shop.size == 1);
        SizeL.SetActive(t_shop.size == 2);

        //DecorationSprite.SetNativeSize();

        CatGuidRepair(null);
    }


    private void GameButtonOnclicke(PointerEventData data)
    {
        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.DecorationsOnclick)
        {
            EventDispatcher.Dispatch(EventEnum.CatGuidUiClose);
        }

        LOG.Info("CatDecorationItem被点击");
        if (this.transitionItem.isUsed == 1 && this.transitionItem.shopid>5)
        {
            return;
        }
        if (ShopT.size == 1 || ShopT.size == 2)
        {
            CUIManager.Instance.OpenForm(UIFormName.CatPublicForm);
            WindowInfo tmpWin = new WindowInfo(ShopT.name.ToString(), ShopT.describe, "Do you want to place this decoration?", "Set", PublicNoButtonCallBacke, PublicYesButtonCallback, 1, "", "", "");
            CUIManager.Instance.GetForm<CatPublicForm>(UIFormName.CatPublicForm).Inite(tmpWin);
        }
        else
        {
            if (transitionItem.count == 0)
            {
                CUIManager.Instance.OpenForm(UIFormName.CatShop);
            }
            else
            {
                //CUIManager.Instance.OpenForm(UIFormName.CatSetForm);
                //CUIManager.Instance.GetForm<CatSetForm>(UIFormName.CatSetForm).Inite("Do you want to place this food?", SetBtnButtonCallback);

                CUIManager.Instance.OpenForm(UIFormName.CatPublicForm);
                WindowInfo tmpWin = new WindowInfo(ShopT.name.ToString(), ShopT.describe, "Do you want to place this food?", "Set", PublicNoButtonCallBacke, SetBtnButtonCallback, 1, "", "", "");
                CUIManager.Instance.GetForm<CatPublicForm>(UIFormName.CatPublicForm).Inite(tmpWin);
            }
           
        }

    }

    private void SetBtnButtonCallback(string obj)
    {
        //LOG.Info("Set按钮点击了");
        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), (int)CatFormEnum.CAT_SET);
        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), (int)CatFormEnum.CAT_DECORATION);
        var tmpForm = CUIManager.Instance.GetForm<CatMainForm>(UIFormName.CatMain);
        CUIManager.Instance.OpenForm(UIFormName.CatMain);
        tmpForm.catSceneView.SetFood(ShopT, id);
    }

    private void PublicYesButtonCallback(string obj)
    {
        if (UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.PlaceDecorationsYes)
        {
            EventDispatcher.Dispatch(EventEnum.CatGuidUiClose);
        }


        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), (int)CatFormEnum.CAT_PUBLIC);
        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), (int)CatFormEnum.CAT_DECORATION);
        var tmpForm = CUIManager.Instance.GetForm<CatMainForm>(UIFormName.CatMain);
        CUIManager.Instance.OpenForm(UIFormName.CatMain);
        tmpForm.catSceneView.SpawnItemPos(ShopT);
    }

    

    private void PublicNoButtonCallBacke(string obj)
    {
        //Debug.Log("共用界面No按钮回调");
    }

    /// <summary>
    /// 移除物体释放内存
    /// </summary>
    public void Dispose()
    {
        if (first)
        {
            DecorationSprite.sprite = null;
            UIEventListener.RemoveOnClickListener(gameObject, GameButtonOnclicke);
        }

        EventDispatcher.RemoveMessageListener(EventEnum.SetGuidPos, SetGuidPos);
        EventDispatcher.RemoveMessageListener(EventEnum.CatGuidRepair, CatGuidRepair);

        Destroy(gameObject);
    }

    /// <summary>
    /// 设置引导点击的位置
    /// </summary>
    /// <param name="notice"></param>
    public void SetGuidPos(Notification notice)
    {
        if (ShopT.shopid==6&& UserDataManager.Instance.GuidStupNum == (int)CatGuidEnum.DecorationsOnclick)
        {
            RectTransform SelRect = transform.GetComponent<RectTransform>();
            RectTransform UseRect = Use.GetComponent<RectTransform>();

            float H = CatDecorationForm.UIMaskH;
            float W = CatDecorationForm.UIMaskW;

            float ScrollViewH = CatDecorationForm.ScrollViewH;

            float h1 = SelRect.anchoredPosition.y;
            float w1= SelRect.anchoredPosition.x;

            float h2 = UseRect.anchoredPosition.y;
            float w2 = UseRect.anchoredPosition.x;

            float Posx = w1+ w2;
            float Posy = H+ ScrollViewH + h1+ h2;
            UserDataManager.Instance.GuidPos = new Vector3(Posx,Posy, 1);
            //LOG.Info("得到食物放置确定按钮的坐标");
        }

    }

    private void CatGuidRepair(Notification notification)
    {
        if (ShopT.shopid == 6 && UserDataManager.Instance.GuidFirstStupNum == (int)CatGuidEnum.PlaceDecorationsYes)
        {
            CUIManager.Instance.OpenForm(UIFormName.CatPublicForm);
            WindowInfo tmpWin = new WindowInfo(ShopT.name.ToString(), ShopT.describe, "Do you want to place this decoration?", "Set", PublicNoButtonCallBacke, PublicYesButtonCallback, 1, "", "", "");
            CUIManager.Instance.GetForm<CatPublicForm>(UIFormName.CatPublicForm).Inite(tmpWin);
        }      
    }
}

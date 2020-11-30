using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatMainFormClasse;
using System;
using UnityEngine.EventSystems;
using UGUI;
using UnityEngine.UI;

public class CatMainTop : CatClase
{
    private GameObject HomeButton;
    private GameObject HeartBtn;
    private GameObject DiamondBtn;
    private GameObject CatFoodGo;
    private CatMainForm catForm;
    //private Image Passe;
    private Text FoodNum;
    public override void Bind(CatMainForm catmainform)
    {
        catmainform.CatMainTop = this;
        catForm= catmainform;

        HomeButton = transform.Find("BG/HomeButton").gameObject;

        Transform catFormTr = catForm.gameObject.transform;
        FoodNum = catFormTr.Find("ButtonListe/CatFood/CatFoodNumber").gameObject.GetComponent<Text>();
        CatFoodGo = catFormTr.Find("ButtonListe/CatFood").gameObject;


        UIEventListener.AddOnClickListener(HomeButton,HomeButtonOnclicke);

        UIEventListener.AddOnClickListener(CatFoodGo, CatFoodGoClickHandler);

        EventDispatcher.AddMessageListener(EventEnum.FoodNumberShow.ToString(), FoodNumberShow);

    }



    private void HomeButtonOnclicke(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);

        //派发事件，关闭猫的主界面
        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(),(int)CatFormEnum.CAT_MAIN);
    }

    private void FoodNumberShow(Notification notification)
    {
        int Number = (int)notification.Data;

        FoodNum.GetComponent<Text>().text = Number + "%";

        UserDataManager.Instance.FoodNum = Number;
    }

    private void CatFoodGoClickHandler(PointerEventData data)
    {
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
    private void PublicYesButtonCallback(string obj)
    {
        CUIManager.Instance.OpenForm(UIFormName.CatShop);
    }

    private void PublicNoButtonCallBacke(string obj)
    {

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
    public override void CloseUi()
    {
        UIEventListener.RemoveOnClickListener(HomeButton, HomeButtonOnclicke);

        EventDispatcher.RemoveMessageListener(EventEnum.FoodNumberShow.ToString(), FoodNumberShow);

        UIEventListener.RemoveOnClickListener(CatFoodGo, CatFoodGoClickHandler);
    }

}

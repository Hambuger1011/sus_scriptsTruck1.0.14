using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UGUI;

/// <summary>
/// 养猫的基础界面
/// </summary>
public class CatBaseForm :BaseUIForm 
{
    private int mUiFormId = 0;
    private bool mInitEvent = false;

    public int UiFormIndex
    {
        set
        {
            mUiFormId = value;
            UserDataManager.Instance.SetCatUiFormDic(mUiFormId);
            if (mUiFormId != (int)CatFormEnum.CAT_MAIN)
                CacularTime(false);

            if(!mInitEvent)
            {
                mInitEvent = true;
                addMessageListener(EventEnum.CloseUiFormDist.ToString(), CloseUiFormDist);
            }
        }
        get { return mUiFormId; }
    }


    public override void OnClose()
    {
        base.OnClose();
        mInitEvent = false;
    }

    /// <summary>
    /// 这个是统一调用，关闭界面的方法
    /// </summary>
    private void CloseUiFormDist(Notification notification)
    {
        int formIndex = (notification == null)?0:(int)notification.Data;
        int closeFormId = 0;
        if (formIndex <= 0)
            closeFormId = UserDataManager.Instance.GetCatFormListTop();
        else
            closeFormId = formIndex;

        if (closeFormId != UiFormIndex) return;

        if(UserDataManager.Instance.RemoveCatFormByIndex(closeFormId))
        {
            switch(closeFormId)
            {
                case (int)CatFormEnum.CAT_MAIN:
                    CUIManager.Instance.CloseForm(UIFormName.CatMain);
                    CUIManager.Instance.CloseForm(UIFormName.CatTop);
                    //退出清除猫的资源
                    CatResourcesSystem.Instance.ReleasePreLoadData();
                    break;
                case (int)CatFormEnum.CAT_ANIMAL:
                    CUIManager.Instance.CloseForm(UIFormName.CatAnimal);
                    break;
                case (int)CatFormEnum.CAT_SHOP:
                    CUIManager.Instance.CloseForm(UIFormName.CatShop);
                    break;
                case (int)CatFormEnum.CAT_DECORATION:
                    CUIManager.Instance.CloseForm(UIFormName.CatDecorations);
                    break;
                case (int)CatFormEnum.CAT_STORY:
                    CUIManager.Instance.CloseForm(UIFormName.CatStory);
                    break;
                case (int)CatFormEnum.CAT_STORY_DETAIL:
                    CUIManager.Instance.CloseForm(UIFormName.CatStoryDetails);
                    break;
                case (int)CatFormEnum.CAT_MY_CHART:
                    CUIManager.Instance.CloseForm(UIFormName.MyChart);
                    break;
                case (int)CatFormEnum.CAT_DETAIL:
                    CUIManager.Instance.CloseForm(UIFormName.CatDetails);
                    break;
                case (int)CatFormEnum.CAT_FOODSET:
                    CUIManager.Instance.CloseForm(UIFormName.CatFoodSetForm);
                    break;
                case (int)CatFormEnum.CAT_GIFT_FROM_ANIM:
                    CUIManager.Instance.CloseForm(UIFormName.CatGiftFromAnimalForm);
                    break;
                case (int)CatFormEnum.CAT_PUBLIC:
                    CUIManager.Instance.CloseForm(UIFormName.CatPublicForm);
                    break;
                case (int)CatFormEnum.CAT_SET:
                    CUIManager.Instance.CloseForm(UIFormName.CatSetForm);
                    break;
                case (int)CatFormEnum.CAT_COLLECT:
                    CUIManager.Instance.CloseForm(UIFormName.Cattery);
                    break;
                case (int)CatFormEnum.CAT_WELCOMEBACK:
                    CUIManager.Instance.CloseForm(UIFormName.CatWelcomBack);
                    break;
                case (int)CatFormEnum.CAT_DIAMONDEXCHANGE:
                    CUIManager.Instance.CloseForm(UIFormName.CatDiamondExchange);
                    break;
                case (int)CatFormEnum.CAT_ANIMALATTRIBUTE:
                    CUIManager.Instance.CloseForm(UIFormName.CatAnimalAttribute);
                    break;
            }
            AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);

            int curTopFormId = UserDataManager.Instance.GetCatFormListTop();
            if (curTopFormId == (int)CatFormEnum.CAT_MAIN)
                CacularTime(true);
            else
                CacularTime(false);
        }
    }

    private void CacularTime(bool isStart = true)
    {
        EventDispatcher.Dispatch(EventEnum.CacularCatFormStayTime, isStart);
    }
}

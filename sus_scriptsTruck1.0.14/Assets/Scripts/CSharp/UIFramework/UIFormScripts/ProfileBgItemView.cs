using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

/// <summary>
/// 个人中心，背景框的子项
/// </summary>
/// 
public class ProfileBgItemView : MonoBehaviour 
{
    public Image BgImage;
    public GameObject InUseFlag;
    public GameObject UseFlag;
    public GameObject LockIcon;

    private Action<int, String,bool> mClickCallBack;
    private ProfileItemInfo mItemInfo;

    void Start()
    {
        UIEventListener.AddOnClickListener(BgImage.gameObject, ItemClickHandler);
    }

    private void ItemClickHandler(PointerEventData vData)
    {
        if (mItemInfo == null) return;
        if(mClickCallBack != null)
        {
            bool canUse = false;
            if (mItemInfo.param != 1 || (mItemInfo.param == 1 && UserDataManager.Instance.userInfo.data.userinfo.is_vip == 1))
                canUse = true;

            mClickCallBack(mItemInfo.res, mItemInfo.remark, canUse);
        }
    }

    public void Init(ProfileItemInfo vItemInfo, Action<int, String,bool> vClickCallBack)
    {
        mItemInfo = vItemInfo;
        mClickCallBack = vClickCallBack;

        BgImage.sprite = ResourceManager.Instance.GetUISprite("ProfileForm/bg_img" + vItemInfo.res);
        LockIcon.gameObject.SetActive(mItemInfo.param == 1 && UserDataManager.Instance.userInfo.data.userinfo.is_vip != 1);
        if (vItemInfo.param == 0 || (vItemInfo.param == 1 && UserDataManager.Instance.userInfo.data.userinfo.is_vip == 1))
            BgImage.material = null;
        else
            BgImage.material = ShaderUtil.GrayMaterial();
        
        
        // InUseFlag.gameObject.SetActive(UserDataManager.Instance.userInfo.data.userinfo.background == vItemInfo.res);
        // UseFlag.gameObject.SetActive(UserDataManager.Instance.userInfo.data.userinfo.background == vItemInfo.res);
    }


    public void Dispose()
    {
        UIEventListener.RemoveOnClickListener(BgImage.gameObject, ItemClickHandler);
        BgImage.sprite = null;
    }
}

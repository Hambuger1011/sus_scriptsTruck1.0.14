using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

/// <summary>
/// 个人中心，头像子项
/// </summary>
public class ProfileHeadItemView : MonoBehaviour 
{
    public Image HeadIcon;
    public GameObject InUseFlag;
    public GameObject UseFlag;
    public GameObject LockIcon;

    private Action<int, String,bool> mClickCallBack;
    private ProfileItemInfo mItemInfo;

    void Start()
    {
        UIEventListener.AddOnClickListener(HeadIcon.gameObject, ItemClickHandler);
    }

    private void ItemClickHandler(PointerEventData vData)
    {
        // if (mItemInfo == null) return;
        // if (mClickCallBack != null)
        // {
        //     bool canUser = false;
        //     if (mItemInfo.param != 1 || (mItemInfo.param == 1 && UserDataManager.Instance.userInfo.data.userinfo.is_vip == 1))
        //         canUser =true;
        //
        //     mClickCallBack(mItemInfo.res ,mItemInfo.remark,canUser);
        //
        //     //保存当前使用的头像
        //     if (canUser)
        //         UserDataManager.Instance.userInfo.data.userinfo.avatar = mItemInfo.res;
        //        
        // }
    }


    public void Init(ProfileItemInfo vItemInfo, Action<int,String,bool> vClickCallBack)
    {
        // mItemInfo = vItemInfo;
        // mClickCallBack = vClickCallBack;
        //
        // HeadIcon.sprite = ResourceManager.Instance.GetUISprite("ProfileForm/img_renwu"+vItemInfo.res);
        // LockIcon.gameObject.SetActive(mItemInfo.param == 1 && UserDataManager.Instance.userInfo.data.userinfo.is_vip != 1);
        // if (vItemInfo.param == 0 || (vItemInfo.param == 1 && UserDataManager.Instance.userInfo.data.userinfo.is_vip == 1))
        //     HeadIcon.material = null;
        // else
        //     HeadIcon.material = ShaderUtil.GrayMaterial();
        //
        // InUseFlag.gameObject.SetActive(UserDataManager.Instance.userInfo.data.userinfo.avatar == vItemInfo.res);
        // UseFlag.gameObject.SetActive(UserDataManager.Instance.userInfo.data.userinfo.avatar == vItemInfo.res);
    }

    public void Dispose()
    {
        UIEventListener.RemoveOnClickListener(HeadIcon.gameObject, ItemClickHandler);
        HeadIcon.sprite = null;
    }
}

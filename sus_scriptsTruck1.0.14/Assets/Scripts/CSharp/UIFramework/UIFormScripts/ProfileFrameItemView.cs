using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

/// <summary>
/// 个人中心，头像框的子项
/// </summary>
public class ProfileFrameItemView : MonoBehaviour 
{
    public Image FrameIcon;
    public GameObject InUseFlag;
    public GameObject UseFlag;
    public GameObject LockIcon;

    private Action<int, String,bool> mClickCallBack;
    private ProfileItemInfo mItemInfo;

    void Start()
    {
        UIEventListener.AddOnClickListener(FrameIcon.gameObject, ItemClickHandler);
    }

    private void ItemClickHandler(PointerEventData vData)
    {
        if (mItemInfo == null) return;
        if (mClickCallBack != null)
        {
            bool canUse = false;
            if (mItemInfo.param != 1 || (mItemInfo.param == 1 && UserDataManager.Instance.userInfo.data.userinfo.is_vip == 1))
                canUse = true;

            mClickCallBack(mItemInfo.res,mItemInfo.remark,canUse);
        }
    }
    public void Init(ProfileItemInfo vItemInfo, Action<int, String ,bool> vClickCallBack)
    {
        mItemInfo = vItemInfo;
        mClickCallBack = vClickCallBack;

        FrameIcon.sprite = ResourceManager.Instance.GetUISprite("ProfileForm/bg_touxkuang" + vItemInfo.res);
        LockIcon.gameObject.SetActive(vItemInfo.param == 1 && UserDataManager.Instance.userInfo.data.userinfo.is_vip != 1);
        if (vItemInfo.param == 0 || (vItemInfo.param == 1 && UserDataManager.Instance.userInfo.data.userinfo.is_vip == 1))
            FrameIcon.material = null;
        else
            FrameIcon.material = ShaderUtil.GrayMaterial();

        InUseFlag.gameObject.SetActive(UserDataManager.Instance.userInfo.data.userinfo.avatar_frame == vItemInfo.res);
        UseFlag.gameObject.SetActive(UserDataManager.Instance.userInfo.data.userinfo.avatar_frame == vItemInfo.res);
    }

    public void Dispose()
    {
        UIEventListener.RemoveOnClickListener(FrameIcon.gameObject, ItemClickHandler);
        FrameIcon.sprite = null;
    }
}

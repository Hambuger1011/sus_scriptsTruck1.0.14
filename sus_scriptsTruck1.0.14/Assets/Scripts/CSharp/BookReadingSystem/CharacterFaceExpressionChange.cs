using DG.Tweening;
using AB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Spine.Unity;
using XLua;


[LuaCallCSharp]
public class CharacterFaceExpressionChange : MonoBehaviour
{
    public Image ColorBG_1;
    public Image ColorBG_2;

    public Image Appearance;
    public Image FacialExpression_1;
    public Image FacialExpression_2;

    [Header("-------------SheletonGraphic-------------")]
    public SkeletonGraphic RoleSkeGraphic;
    public BoneFollowerGraphic HeadBoneFolGraphic;
    public SkeletonGraphic ExpressionSkeGraphic;
    public SkeletonGraphic HairSkeGraphic;




#if false
    private int currentIndex = 0;
    private float delay = 0.3f;
    private float duration = 0.4f;
    private float facialDelay = 0.5f;

    private int appearanceIDCache = 0;
    private int expressionIdCache = -1;
    private int clothIdCache = -1;

    private int mRoleID;
    private int mAppearanceID;
    private int mFacialExpressionID;
    private int mFaciaOriginalID;
    private int mIconBGID;
    private int mOrientation;
    private int mLastSkinId;

    private Action mTurnCallBack;

    private int mCacheHairIndex = -1;
    private bool isSetPos = false;
    private int mAccount = 4;
    private int mClothId;

#region pic role
    /// <summary>
    /// 对话情节切换表情
    /// </summary>
    /// <param name="appearanceID"></param>
    /// <param name="facialExpressionID"></param>
    /// <param name="iconBGID"></param>
    public void Change(int appearanceID, int facialExpressionID, int faciaOriginalID, int iconBGID, int vOrientation)
    {
        delay = 0.3f;
        facialDelay = 0.5f;
        DoChange(appearanceID, facialExpressionID, faciaOriginalID, iconBGID, vOrientation, null);
    }

    /// <summary>
    /// 点击头像切换表情
    /// </summary>
    /// <param name="appearanceID"></param>
    /// <param name="facialExpressionID"></param>
    /// <param name="faciaOriginalID">原始表情的id（如果为0 ，则表示不需要表情）</param>
    /// <param name="iconBGID"></param>
    /// <param name="vOriginalExpression"></param>
    /// <param name="vNeedTurnBack"></param>
    public void Change(int appearanceID, int facialExpressionID, int faciaOriginalID, int iconBGID, int vTurnExpression, int vOrientation, int vNeedTurnBack = 1)
    {
        delay = 0f;
        facialDelay = 0f;
        mAppearanceID = appearanceID;
        mFacialExpressionID = facialExpressionID;
        mFaciaOriginalID = faciaOriginalID;
        mIconBGID = iconBGID;
        mOrientation = vOrientation;
        DoChange(appearanceID, vTurnExpression, faciaOriginalID, iconBGID, vOrientation, TurnBackHandler, 0);
    }

    private void TurnBackHandler()
    {
        delay = 0f;
        facialDelay = 0f;
        DoChange(mAppearanceID, mFacialExpressionID, mFaciaOriginalID, mIconBGID, mOrientation, null, 0);
    }

    private void DoChange(int appearanceID, int facialExpressionID, int faciaOriginalID, int iconBGID, int vOrientation, Action vTurnCallBack, int changeBg = 1)
    {
        ColorBG_1.DOKill();
        ColorBG_2.DOKill();
        FacialExpression_1.DOKill();
        FacialExpression_2.DOKill();

        mTurnCallBack = vTurnCallBack;

#if !NOT_USE_LUA
        var sptAppear = BookReadingWrapper.Instance.GetUITexture("RoleHead/" + appearanceID,false);
        //var sptBG = BookReadingWrapper.Instance.GetUITexture("UI/RoleHeadFacialExpression/ColorBG_" + iconBGID, true);

        var sptBG = ResourceManager.Instance.GetUISprite("BookReadingForm/ColorBG_" + iconBGID);
        Sprite sptFacial = null;
        if (faciaOriginalID != 0)
        {
            sptFacial = BookReadingWrapper.Instance.GetUITexture("UI/RoleHeadFacialExpression/" + facialExpressionID, true);
        }
#else
        var sptAppear = DialogDisplaySystem.Instance.GetUITexture("RoleHead/" + appearanceID, false);
        var sptBG = DialogDisplaySystem.Instance.GetUITexture("RoleHeadFacialExpression/ColorBG_" + iconBGID);
        Sprite sptFacial = null;
        if (faciaOriginalID != 0)
            sptFacial = DialogDisplaySystem.Instance.GetUITexture("RoleHeadFacialExpression/" + facialExpressionID);
#endif

        if (isChangeCharacter(appearanceID))
        {
            Appearance.sprite = sptAppear;
            ColorBG_1.sprite = sptBG;
            ColorBG_2.sprite = sptBG;
            if (faciaOriginalID != 0)
            {
                FacialExpression_1.sprite = sptFacial;
                FacialExpression_2.sprite = sptFacial;
            }
        }

        if (faciaOriginalID == 0)
        {
            FacialExpression_1.color = new Color(1, 1, 1, 0);
            FacialExpression_2.color = new Color(1, 1, 1, 0);
        }

        Vector3 dict = new Vector3(-1, 1, 1);
        if (vOrientation == 2)
            dict = new Vector3(1, 1, 1);
        Appearance.transform.localScale = dict;
        Appearance.sprite = sptAppear;
        if (currentIndex == 0)
        {
            if (changeBg == 1) imageChangeTween(ColorBG_1, ColorBG_2, sptBG, delay, null);
            if (faciaOriginalID != 0) imageChangeTween(FacialExpression_1, FacialExpression_2, sptFacial, facialDelay, mTurnCallBack);
            currentIndex = 1;
        }
        else
        {
            if (changeBg == 1) imageChangeTween(ColorBG_2, ColorBG_1, sptBG, delay, null);
            if (faciaOriginalID != 0) imageChangeTween(FacialExpression_2, FacialExpression_1, sptFacial, facialDelay, mTurnCallBack);
            currentIndex = 0;
        }
    }

    private void imageChangeTween(Image imageFirst, Image imageSceond, Sprite sprite, float vDelay, Action vTurnCallBack)
    {
        imageSceond.gameObject.SetActive(true);
        imageSceond.sprite = sprite;
        imageSceond.color = new Color(1, 1, 1, 0);
        imageSceond.DOColor(new Color(1, 1, 1, 1f), duration).SetDelay(vDelay)
            .OnComplete(
            () =>
            {
                imageSceond.transform.SetAsFirstSibling();
                imageFirst.gameObject.SetActive(false);
                imageSceond.gameObject.SetActive(true);
                EventDispatcher.Dispatch(UIEventMethodName.BookReadingForm_IsTweening.ToString(), false);
                if (vTurnCallBack != null && mTurnCallBack != null)
                    mTurnCallBack();
            });
    }
#endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="appearanceId">外貌id</param>
    /// <param name="clothId">衣服id</param>
    /// <param name="faciaOriginalID">原来的表情id</param>
    /// <param name="iconBGID">背景id</param>
    /// <param name="vOrientation">角色朝向</param>
    public void Change(int roleId, int appearanceId, int clothId, int faciaOriginalID, int iconBGID, int vOrientation)
    {
        delay = 0.3f;
        facialDelay = 0.5f;
        mRoleID = roleId;
        mAppearanceID = appearanceId;
        mClothId = clothId;
        mFaciaOriginalID = faciaOriginalID;
        mIconBGID = iconBGID;
        mOrientation = vOrientation;
        DoChange(appearanceId, clothId, faciaOriginalID, iconBGID, vOrientation);
    }


    private void DoChange(int appearanceId, int clothId, int faciaOriginalID, int iconBGID, int vOrientation)
    {
        ColorBG_1.DOKill();
        ColorBG_2.DOKill();
#if !NOT_USE_LUA
        //var sptBG = BookReadingWrapper.Instance.GetUITexture("UI/RoleHeadFacialExpression/ColorBG_" + iconBGID);
        var sptBG = ResourceManager.Instance.GetUISprite("BookReadingForm/ColorBG_" + iconBGID);
#else
        var sptBG = DialogDisplaySystem.Instance.GetUITexture("RoleHeadFacialExpression/ColorBG_" + iconBGID);
#endif

        
        if (isChangeCharacter(appearanceId))
        {
            isSetPos = false;
            ColorBG_1.sprite = sptBG;
            ColorBG_2.sprite = sptBG;
            UpdateAtlas();  //新的资源加载方式
            ResetPos();
        }
        else
        {
            if (isChangeClothes(mClothId))
                SetClothesIndex(mClothId);
            
            if (isChangeExpression(mFaciaOriginalID))
            {
                SetExpression(mFaciaOriginalID);
                var harIdx = (mFaciaOriginalID == 0) ? 0 : 1;
                if(mCacheHairIndex != harIdx)
                {
                    mCacheHairIndex = harIdx;
                    SetHair(harIdx);
                }
            }

            if (isChangeSkin())
                SetSkinName(mLastSkinId);
        }

        Vector3 dict = new Vector3(-1, 1, 1);
        if (vOrientation == 2)
            dict = new Vector3(1, 1, 1);
        Appearance.transform.localScale = dict;

        //ColorBG_1.gameObject.SetActive(false);
        //ColorBG_2.gameObject.SetActive(false);

        if (currentIndex == 0)
        {
            imageChangeTween(ColorBG_1, ColorBG_2, sptBG, delay, null);
            currentIndex = 1;
        }
        else
        {
            imageChangeTween(ColorBG_2, ColorBG_1, sptBG, delay, null);
            currentIndex = 0;
        }

        var t = RoleSkeGraphic.rectTransform();
        var bookDetial = JsonDTManager.Instance.GetJDTBookDetailInfo(BookReadingWrapper.Instance.BookID);
        if(bookDetial.RoleScale == 2)
        {
            t.localScale = new Vector3(0.85f,0.85f,1);
            t.anchoredPosition = new Vector2(0, -396f);
        }else
        {
            this.transform.localScale = new Vector3(1, 1, 1);
            t.anchoredPosition = new Vector2(0, -460f);
        }
    }

    public void UpdateColor(bool isLight)
    {
        Color targetColor = new Color(1, 1, 1, 1);
        if (!isLight)
            targetColor = new Color(0.6f, 0.6f, 0.6f, 1);

        RoleSkeGraphic.color = targetColor;
        ExpressionSkeGraphic.color = targetColor;
        HairSkeGraphic.color = targetColor;
    }

    private void UpdateAtlas()
    {
#if !NOT_USE_LUA
        SkeletonDataAsset skeData = BookReadingWrapper.Instance.GetSkeDataAsset("Role/" + mAppearanceID);
#else
        SkeletonDataAsset skeData = DialogDisplaySystem.Instance.GetSkeDataAsset("Role/" + mAppearanceID);
#endif
        if (skeData == null)
        {
            LOG.Error("---角色Spine SkeData 有误-->" + mAppearanceID);
            return;
        }
        RoleSkeGraphic.skeletonDataAsset = skeData;
        ExpressionSkeGraphic.skeletonDataAsset = skeData;
        HairSkeGraphic.skeletonDataAsset = skeData;

        HeadBoneFolGraphic.SkeletonGraphic = RoleSkeGraphic;
        ResetAvatarInfo();
    }

    private void ResetAvatarInfo()
    {
        int skinIndex = 1;
        int npcDetailId = 0;
        if (mRoleID == 1)
        {
#if !NOT_USE_LUA
            skinIndex = BookReadingWrapper.Instance.CurrentBookData.PlayerDetailsID;
#else
            skinIndex = DialogDisplaySystem.Instance.CurrentBookData.PlayerDetailsID;
#endif
        }
        else if (mRoleID > 0)
        {
            int recordeNpcId = 0;
#if !NOT_USE_LUA
            npcDetailId = BookReadingWrapper.Instance.CurrentBookData.NpcDetailId;
            recordeNpcId = BookReadingWrapper.Instance.CurrentBookData.NpcId;
#else
            npcDetailId = DialogDisplaySystem.Instance.CurrentBookData.NpcDetailId;
            recordeNpcId = DialogDisplaySystem.Instance.CurrentBookData.NpcId;
#endif
            if (npcDetailId > 0 && mRoleID == recordeNpcId)
            {
                int tempNpcDetail = (npcDetailId > 3) ? (npcDetailId - 3) : npcDetailId;
                skinIndex = tempNpcDetail;
            }
        }
        int hairId = 1;
        if (mFaciaOriginalID == 0)
            hairId = 0;
        SetSkinName(skinIndex);
        SetExpression(mFaciaOriginalID);
        SetHair(hairId);
        SetClothesIndex(mClothId);

        mLastSkinId = skinIndex;
    }

    private void ResetPos()
    {
        if (isSetPos) return;
        isSetPos = true;
        StartCoroutine(DoUpdatePos());
    }

    IEnumerator DoUpdatePos()
    {
        yield return new WaitForEndOfFrame();
        Vector3 targetPos = new Vector3(-HeadBoneFolGraphic.transform.localPosition.x, -HeadBoneFolGraphic.transform.localPosition.y);
        ExpressionSkeGraphic.transform.localPosition = targetPos;
        HairSkeGraphic.transform.localPosition = targetPos;
    }

    private void SetSkinName(int vSkinIndex)
    {
        string skinName = "skin" + vSkinIndex;
        RoleSkeGraphic.initialSkinName = skinName;
        ExpressionSkeGraphic.initialSkinName = skinName;
        HairSkeGraphic.initialSkinName = skinName;

        RoleSkeGraphic.Initialize(true);
        ExpressionSkeGraphic.Initialize(true);
        HairSkeGraphic.Initialize(true);

        ResetFloower();
    }

    private void SetClothesIndex(int vIndex)
    {
        string animName = (vIndex >= 10) ? "clothes" + vIndex : "clothes" + vIndex;
        //Debug.Log("===Index---->" + vIndex + "---animName---" + animName);
        RoleSkeGraphic.startingAnimation = animName;
        RoleSkeGraphic.startingLoop = true;
        RoleSkeGraphic.Initialize(true);
        clothIdCache = vIndex;
        ResetFloower();
    }

    private void SetExpression(int vIndex)
    {
        string animName = "expression" + vIndex;
        //Debug.Log("===Index---->" + vIndex + "---animName---" + animName);
        ExpressionSkeGraphic.startingAnimation = animName;
        ExpressionSkeGraphic.startingLoop = false;
        ExpressionSkeGraphic.Initialize(true);

        ResetFloower();
    }

    private void SetHair(int vIndex)
    {
        string animName = "hair" + vIndex;
        //Debug.Log("===Index---->" + vIndex + "---Hair---" + animName);
        HairSkeGraphic.startingAnimation = animName;
        HairSkeGraphic.startingLoop = true;
        HairSkeGraphic.Initialize(true);

        ResetFloower();
    }

    private void ResetFloower()
    {
        HeadBoneFolGraphic.boneName = "tou";
        HeadBoneFolGraphic.Initialize();
    }

    //是否角色有变化
    private bool isChangeCharacter(int appearanceID)
    {
        if (appearanceIDCache == appearanceID)
        {
            return false;
        }

        appearanceIDCache = appearanceID;
        return true;
    }

    //是否表情有变化
    private bool isChangeExpression(int curExpressionId)
    {
        if (expressionIdCache == curExpressionId)
        {
            return false;
        }
        expressionIdCache = curExpressionId;
        return true;
    }

    //服装是否有变化
    private bool isChangeClothes(int curClothId)
    {
        if (clothIdCache == curClothId)
        {
            return false;
        }

        clothIdCache = curClothId;
        return true;
    }

    //服装是否有变化
    private bool isChangeSkin()
    {
        int curSkin = 1;
        if (mRoleID == 1)
        {
#if !NOT_USE_LUA
            curSkin = BookReadingWrapper.Instance.CurrentBookData.PlayerDetailsID;
#else
            curSkin = DialogDisplaySystem.Instance.CurrentBookData.PlayerDetailsID;
#endif
        }
        else if (mRoleID > 0)
        {
            int recordeNpcId = 0;
            int npcDetailId = 0;
#if !NOT_USE_LUA
            npcDetailId = BookReadingWrapper.Instance.CurrentBookData.NpcDetailId;
            recordeNpcId = BookReadingWrapper.Instance.CurrentBookData.NpcId;
#else
            npcDetailId = DialogDisplaySystem.Instance.CurrentBookData.NpcDetailId;
            recordeNpcId = DialogDisplaySystem.Instance.CurrentBookData.NpcId;
#endif
            if (npcDetailId > 0 && mRoleID == recordeNpcId)
            {
                int tempNpcDetail = (npcDetailId > 3) ? (npcDetailId - 3) : npcDetailId;
                curSkin = tempNpcDetail;
            }
        }

        if (mLastSkinId == curSkin)
        {
            return false;
        }

        mLastSkinId = curSkin;
        return true;
    }
#endif

    public void Dispose()
    {
        //Appearance.sprite = null;
        ColorBG_1.sprite = null;
        ColorBG_2.sprite = null;
        FacialExpression_1.sprite = null;
        FacialExpression_2.sprite = null;
    }
}

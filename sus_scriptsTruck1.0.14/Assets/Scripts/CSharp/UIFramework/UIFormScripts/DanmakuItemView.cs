using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

/// <summary>
/// 表情互动项
/// </summary>
public class DanmakuItemView : MonoBehaviour 
{
    public Image HeadFrame;
    public Image HeadIcon;
    
    public Text DanmakuTxt;
    public CanvasGroup ItemCGroup;

    private bool mIsStart = false;
    private float mStartX = 0;
    private float mStartY = 0;
    private float mSpeedX = 0;
    private float mSpeedY = 0;
    private float mTargetX = -450f;
    private float mTargetY = 0;
    private float mLastItemX = 0;
    private float mLastItemY = 0;
    public int mIndex = 1;
    private int mType = 1;
    private float halfScreen = -1;
    private float itemAlpha = 1.0f;
    private float mStandingTime = 0;
    private float mInterval = 3;
    private Vector2 mRect;
    private bool mInitEvent;

    private int scrWidth = 750;

    private BookBarrageItemInfo mItemInfo;

    private Action<DanmakuItemView,bool> mCallBack;
    public void Init(int vType,int vTypeIndex,float vLastItemX,float vLastItemY,BookBarrageItemInfo vItemInfo, Vector2 vRect,Action<DanmakuItemView,bool> vCallBack)
    {
        mIndex = vTypeIndex;
        mType = vType;
        mCallBack = vCallBack;
        mRect = vRect;
        mItemInfo = vItemInfo;
        mLastItemX = vLastItemX;
        mLastItemY = vLastItemY;
        mIsStart = false;

        this.transform.DOKill();
        ItemCGroup.DOKill();
        itemAlpha = 1f;

        InitEvent();
        InitFaceIcon();

        if (halfScreen == -1)
            halfScreen = scrWidth * 0.5f;


        #region type1
        if (mType == 1)
        {
            mTargetX = -(GetItemWidth() * 0.5F + scrWidth * 0.5f);
            mStartY = (mIndex - 1) * -85;
            this.transform.localPosition = new Vector3(800, mStartY, 0);
            ItemCGroup.alpha = 0;
            this.transform.localScale = new Vector2(1, 1);
            ItemCGroup.DOFade(0, 0.1f).OnComplete(() =>
            {
                if ((mLastItemX + vRect.x * 0.5f)  < halfScreen)//上一个已经超过半场了
                {
                    mStartX = GetItemWidth() * 0.5f + halfScreen;
                }else
                {
                    mStartX = vLastItemX + (vRect.x + GetItemWidth()) * 0.5f;
                }
                ItemCGroup.alpha = 1;
                this.transform.localPosition = new Vector3(mStartX, mStartY, 0);
                StartPlay();
                //LOG.Info("--mStartX--->" + mStartX + "===tempTypeStart===>" + tempTypeStart);
            });
        }
        #endregion type1

        #region type2
        else if (mType == 2)
        {
            mStartX = 600;
            mStartY = -400;
            float tempStartX = 200;
            float showTime = 0.5f;
            mTargetY = 150;

            this.transform.localScale = new Vector2(1, 1);
            this.transform.localPosition = new Vector3(mStartX, mStartY, 0);
            ItemCGroup.DOFade(1, 0.1f).OnComplete(() =>
            {
                tempStartX = (750 - GetItemWidth()) * 0.5f- 10;
                //Debug.LogError("---scrWidth-->>"+scrWidth+"---GetItemWidth()-->"+GetItemWidth()+"---tempStartX->"+tempStartX);
                this.rectTransform().DOAnchorPosX(tempStartX, showTime).OnComplete(() =>
                {
                    mStartX = tempStartX;
                    StartPlay();
                }).Play();
            }).Play();
            
        }



        #endregion type2

        #region type3
        else if (mType == 3)
        {
            ItemCGroup.alpha = 0;
            float tempStartX = 200;
            mStandingTime = 0;
            mInterval = 3;
            ItemCGroup.DOFade(0, 0.1f).OnComplete(() =>
            {
                tempStartX = (scrWidth - GetItemWidth()) * 0.5f;
                mStartX = (float)UnityEngine.Random.Range(-tempStartX, tempStartX);
                mStartY = (float)UnityEngine.Random.Range(-400, -50);
                //LOG.Info("----startY1---->" + mStartY + "-vlastY->" + vLastItemY);
                if(Math.Abs(Math.Abs(mStartY) - Math.Abs(vLastItemY))<70)
                {
                    if (vLastItemY < -300)
                        mStartY = vLastItemY + UnityEngine.Random.Range(1, 4) * 50;
                    else if(vLastItemY>-50)
                        mStartY = vLastItemY - UnityEngine.Random.Range(1, 4) * 50;
                    else
                        mStartY = vLastItemY + UnityEngine.Random.Range(1, 3) * 50;
                    //LOG.Info("====----startY2---->" + mStartY + "-vlastY->" + vLastItemY);
                }
                this.transform.localPosition = new Vector3(mStartX, mStartY, 0);
                float tempScale = (float)(UnityEngine.Random.Range(3, 7) * 1.0f / 10f);
                this.transform.localScale = new Vector2(tempScale, tempScale);
                ItemCGroup.DOFade(1, 0.5f).Play();
                this.rectTransform().DOScale(1, 0.5f).OnComplete(() => { StartPlay(); }).Play();

            }).Play();
        }
        #endregion type3
       
        
    }

    private void InitEvent()
    {
        if(!mInitEvent)
        {
            mInitEvent = true;
            EventDispatcher.AddMessageListener(EventEnum.DanmakuChangeShowType, ChangeShowTypeHandler);
        }
    }

    #region 展示形式 01
    private void InitFaceIcon()
    {
        if (mItemInfo == null) return;
        if (mIndex == 0)
            mIndex = 1;

        string str = mItemInfo.content;
        if (str.Length > 30)
            str = str.Insert(30, "\n");

        if (mType == 1)
        {
            switch (mIndex)
            {
                case 1:
                    mSpeedX = 1.4f;
                    break;
                case 2:
                    mSpeedX = 2.2f;
                    break;
                case 3:
                    mSpeedX = 1.8f;
                    break;
                case 4:
                    mSpeedX = 2.5f;
                    break;
            }

        }
        else if (mType == 2)
        {
            mSpeedX = 0;
            mSpeedY = 0.8f;

        }
        else if (mType == 3)
        {
            mSpeedX = 0;
            mSpeedY = 0f;
        }




        if (DanmakuTxt != null)
        {
            DanmakuTxt.text = str;
        }


        if (HeadIcon != null)
        {
            //【调用lua公共方法 加载头像】   -1代码当前装扮的头像    
            XLuaManager.Instance.CallFunction("GameHelper", "ShowAvatar", mItemInfo.avatar, HeadIcon);
        }

        if (HeadFrame != null)
        {
            //【调用lua公共方法 加载头像框】   -1代码当前装扮的头像框   
            XLuaManager.Instance.CallFunction("GameHelper", "ShowAvatarframe", mItemInfo.avatar_frame, HeadFrame);
        }

        

        //FaceItem.sprite = ResourceManager.Instance.GetUISprite("MetaGames/face_" + mType + "_0" + mIndex);
    }
    private void StartPlay()
    {
        mIsStart = true;
    }

	void Update () 
    {
        if(mIsStart)
        {
            if(mType == 1)
            {
                if (mStartX >= 9999.0f)
                {
                    mStartX = mLastItemX + mRect.x + GetItemWidth() * 0.5f;
                }
                mStartX -= mSpeedX;
                if (mStartX < mTargetX)
                {
                    mIsStart = false;
                    if (mCallBack != null)
                        mCallBack(this,true);
                    mCallBack = null;
                }
                this.transform.localPosition = new Vector3(mStartX, mStartY, 0);
            }
            else if (mType == 2)
            {
                mStartY += mSpeedY;
                if (mStartY > -200)
                {
                    itemAlpha -= 0.004f;
                    if (itemAlpha <= 0f)
                        itemAlpha = 0f;
                }
                if (mStartY > mTargetY)
                {
                    mIsStart = false;
                    if (mCallBack != null)
                        mCallBack(this, true);
                    mCallBack = null;
                }

                ItemCGroup.alpha = itemAlpha;
                this.transform.localPosition = new Vector3(mStartX, mStartY, 0);
            }
            else if(mType == 3)
            {
                mStandingTime += Time.deltaTime;
                if(mStandingTime > mInterval)
                {
                    mIsStart = false;
                    ItemCGroup.DOFade(0, 0.5f).Play();
                    float tempScale = (float)(UnityEngine.Random.Range(3, 7) * 1.0f / 10f);
                    this.rectTransform().DOScale(tempScale, 0.5f).OnComplete(() => 
                    {
                        if (mCallBack != null)
                            mCallBack(this, true);
                        mCallBack = null;
                    }).Play();
                }
            }
            
            
        }
	}

    #endregion

    public int GetItemIndex()
    {
        return mIndex;
    }

    public float GetCurPosX()
    {
        return this.transform.localPosition.x ;
    }

    public float GetCurPosY()
    {
        return this.transform.localPosition.y;
    }

    public float GetItemWidth()
    {
        return (DanmakuTxt.rectTransform().rect.width + 100);
    }

    public float GetItemHeight()
    {
        return (DanmakuTxt.rectTransform().rect.height + 50);
    }

    public BookBarrageItemInfo GetItemInfo()
    {
        return mItemInfo;
    }

    public void ResetItem()
    {
        mIsStart = false;
        ItemCGroup.DOFade(0, 0.3f).OnComplete(()=> 
        {
            if (mCallBack != null)
                mCallBack(this, false);
            mCallBack = null;

        }).Play();
    }

    private void ChangeShowTypeHandler(Notification vNot)
    {
        ResetItem();
    }

    public void Dispose()
    {
        if(mInitEvent)
        {
            EventDispatcher.RemoveMessageListener(EventEnum.DanmakuChangeShowType, ChangeShowTypeHandler);
            mInitEvent = false;
        }
        //if (FaceItem != null)
        //    FaceItem.sprite = null;
    }
}

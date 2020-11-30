using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

/// <summary>
/// 表情互动项
/// </summary>
public class EmojiMsgItemForm : MonoBehaviour 
{
    public Image FaceItem;

    private bool mIsStart = false;
    private float mStartX = 0;
    private float mStartY = 0;
    private float mSpeedX = 0;
    private float mSpeedY = 0;
    private float mTargetX = -450f;
    private float mTargetY = 0;
    private float mIndex = 1;
    private int mType = 999;

    private Action<EmojiMsgItemForm> mCallBack;
    public void Init(int vType,int vIndex,Action<EmojiMsgItemForm> vCallBack)
    {
        mIndex = vIndex;
        mType = vType;
        mIsStart = false;
        mCallBack = vCallBack;
        mStartX = 450;
        mStartY = (float)UnityEngine.Random.Range(-150, 150);
        mTargetY = (mStartY >= 0) ? UnityEngine.Random.Range(-150, -10) : UnityEngine.Random.Range(10, 150);
        mSpeedX = (float)(UnityEngine.Random.Range(9, 25) * 1.0f / 10f);
        mSpeedY = (float)(UnityEngine.Random.Range(3, 20) * 1.0f / 10f);
        this.transform.localPosition = new Vector3(mStartX,mStartY,0);
        float tempScale = (float)(UnityEngine.Random.Range(3, 9)*1.0f/10f);
        float addScale = (float)(UnityEngine.Random.Range(1, 7) * 1.0f / 10f);
        this.transform.localScale = new Vector2(tempScale, tempScale);
        int tempStartX = UnityEngine.Random.Range(275, 300);
        float showTime = (float)(UnityEngine.Random.Range(6, 9) * 1.0f / 10f);
        this.rectTransform().DOAnchorPosX(tempStartX, showTime).Play();
        this.rectTransform().DOScale(tempScale + addScale, 0.5f).SetDelay(0.3f).OnComplete(() => { StartPlay(); }).Play();
        mStartX = tempStartX;
        InitFaceIcon();
    }

    private void InitFaceIcon()
    {
        if (mIndex == 0)
            mIndex = 1;

        if (mType == 0)
            mType = 999;
        FaceItem.sprite = ResourceManager.Instance.GetUISprite("MetaGames/face_" + mType + "_0" + mIndex);
    }
    private void StartPlay()
    {
        mIsStart = true;
    }

	void Update () 
    {
        if(mIsStart)
        {
            mStartX -= mSpeedX;
            if (mStartX < mTargetX)
            {
                mIsStart = false;
                if (mCallBack != null)
                    mCallBack(this);
                mCallBack = null;
            }
            if(mTargetY>0)
            {
                mStartY += mSpeedY;
                if(mStartY > mTargetY)
                    mTargetY = UnityEngine.Random.Range(-150, -10);
            }else
            {
                mStartY -= mSpeedY;
                if (mStartY < mTargetY)
                    mTargetY = UnityEngine.Random.Range(10,150);
            }

            this.transform.localPosition = new Vector3(mStartX, mStartY, 0);
        }
	}

    public void Dispose()
    {
        if (FaceItem != null)
            FaceItem.sprite = null;
    }
}

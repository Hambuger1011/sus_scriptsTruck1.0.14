using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using UGUI;

/// <summary>
/// 文字飘动
/// </summary>
public class PopTipsItemView : MonoBehaviour 
{
    public CanvasGroup CGroup;
    public Text TipsTxt;


    private Action<PopTipsItemView> mFinishCallBack;
    private Vector2 mStartPos;
    private RectTransform trans;

    private void Awake()
    {
        this.trans = this.rectTransform();
    }

    public void Init(string vMsg,Vector2 vStartPos, Action<PopTipsItemView> vFinishCallBack)
    {
        TipsTxt.text = vMsg;
        mFinishCallBack = vFinishCallBack;
        mStartPos = vStartPos;

        CGroup.DOKill();
        this.transform.DOKill();
        
        Show();
    }

    private void Show()
    {
        this.gameObject.SetActive(true);
        CGroup.alpha = 0;
        CGroup.DOFade(1, 0.5f).Play();
        this.trans.anchorMin = new Vector2(0.5f, 0.5f);
        this.trans.anchorMax = new Vector2(0.5f, 0.5f);
        this.trans.anchoredPosition = mStartPos;
        this.trans.localScale = new Vector3(0.5f,0.5f,0.5f);
        this.trans.DOScale(1, 0.5f).Play();
        this.trans.DOLocalMoveY(-100, 3f).OnComplete(() => 
        { 
            if(mFinishCallBack !=null)
            {
                this.gameObject.SetActive(false);
                mFinishCallBack(this);
            }
        
        }).Play();
    }
	
}

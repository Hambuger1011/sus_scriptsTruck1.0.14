using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System;
/// <summary>
/// 表情互动，选择项
/// </summary>
public class EmojiMsgSelectItemForm : MonoBehaviour 
{
    public Image IconImage;
    
    private int mIndex;
    private Action<int> mCallBack;
    private int mType;

    private bool mHasEvent = false;

    void Start()
    {
        
    }

    private void ClickItemHandler(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        if (mCallBack != null)
            mCallBack(mIndex);
    }


    public void Init(int vType,int vIndex,Action<int> vCallBack)
    {
        mType = vType;
        mIndex = vIndex;
        mCallBack = vCallBack;
        if (mType == 0)
            mType = 999;
        IconImage.sprite = ResourceManager.Instance.GetUISprite("MetaGames/face_"+mType+"_0" + (mIndex+1));

        if(!mHasEvent)
        {
            mHasEvent = true;
            UIEventListener.AddOnClickListener(IconImage.gameObject, ClickItemHandler);
        }
    }

    public void Dispose()
    {
        UIEventListener.RemoveOnClickListener(IconImage.gameObject, ClickItemHandler);
        IconImage.sprite = null;
        mHasEvent = false;
    }
	
}

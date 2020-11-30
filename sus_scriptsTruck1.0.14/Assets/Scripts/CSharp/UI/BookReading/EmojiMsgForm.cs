using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using UGUI;

/// <summary>
/// 表情互动界面
/// </summary>
public class EmojiMsgForm : BaseUIForm
{

    public GameObject UIMask;
    public RectTransform Content;
    public GameObject ItemFather;
    public GameObject ItemChild;
    public GameObject SelectItemFather;
    public GameObject SelectItemChild;
    public RectTransform SelectGroup;
    public Image ShowIcon;

    private List<EmojiMsgItemForm> mItemList;
    private List<EmojiMsgSelectItemForm> mSelectItemList;
    private float mInterval = 0.4f;
    private float mTimes = 0;
    private float mOpenFormTime = 0;
    private int mType = 0;

    private bool mIsSelected = false;
    private bool mCanCloseForm = false;



    private int mSelect1 ;
    private int mSelect2 ;
    private int mSelect3 ;
    private int mSelect4 ;


    public override void OnOpen()
    {
        base.OnOpen();
        mIsSelected = false;
        mCanCloseForm = false;

        mInterval = 0.2f;
        if (mItemList == null)
            mItemList = new List<EmojiMsgItemForm>();

        mSelect1 = 0;
        mSelect2 = 0;
        mSelect3 = 0;
        mSelect4 = 0;

        UIEventListener.AddOnClickListener(UIMask.gameObject, CloseFormHandler);

        GameHttpNet.Instance.GetGameEmojiMsgList(
#if NOT_USE_LUA
            DialogDisplaySystem.Instance.CurrentBookData.BookID, 
            DialogDisplaySystem.Instance.CurrentDialogData.dialogID, 
#else
            BookReadingWrapper.Instance.CurrentBookData.BookID,
            BookReadingWrapper.Instance.CurrentBookData.DialogueID,
#endif
            GetEmojiMsgListHandler
            );

#if ENABLE_DEBUG
        if (GameDataMgr.Instance.InAutoPlay)
        {
            Invoke("AutoDo", 1f);
        }
#endif
    }

    public void Init(int vType)
    {
        mType = vType;
        InitFaceSelectItem();
        ShowSelectGroup();
    }


    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(UIMask.gameObject, CloseFormHandler);
        if(mSelectItemList != null)
        {
            int len = mSelectItemList.Count;
            for (int i = 0; i < len; i++)
            {
                EmojiMsgSelectItemForm item = mSelectItemList[i];
                if (item != null)
                    item.Dispose();
            }
            mSelectItemList = null;
        }

        if (mItemList != null)
        {
            int len = mItemList.Count;
            for (int i = 0; i < len; i++)
            {
                EmojiMsgItemForm item = mItemList[i];
                if (item != null)
                    item.Dispose();
            }
            mItemList = null;
        }
    }

    private void AutoDo()
    {
        CancelInvoke("AutoDo");
        ItemClickHandler(0);
    }

    private void GetEmojiMsgListHandler(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetEmojiMsgListHandler---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.emojiMsgListInfo = JsonHelper.JsonToObject<HttpInfoReturn<EmojiMsgListInfo>>(result);
                    if (UserDataManager.Instance.emojiMsgListInfo != null && UserDataManager.Instance.emojiMsgListInfo.data != null)
                    {
                        EmojiMsgListInfo msgInfo = UserDataManager.Instance.emojiMsgListInfo.data;
                        mSelect1 = msgInfo.phiz1;
                        mSelect2 = msgInfo.phiz2;
                        mSelect3 = msgInfo.phiz3;
                        mSelect4 = msgInfo.phiz4;
                    }
                }
            }, null);
        }
    }
    private void InitFaceSelectItem()
    {
        if (mSelectItemList != null) return;
        mSelectItemList = new List<EmojiMsgSelectItemForm>();
        int len = 4;
        for (int i = 0; i < len; i++)
        {
            EmojiMsgSelectItemForm item = getSelectItem();
            item.rectTransform().anchoredPosition = new Vector3(-150 + (i * 130f), 0,0);
            item.Init(mType,i, ItemClickHandler);
            mSelectItemList.Add(item);
        }
    }

    private void ItemClickHandler(int vIndex)
    {
        if (mIsSelected) return;
        mIsSelected = true;
        CalularFaceNum((vIndex + 1), false);
        ShowIcon.sprite = ResourceManager.Instance.GetUISprite("MetaGames/face_" + mType + "_0" + (vIndex + 1));
        ShowIcon.gameObject.SetActive(true);
        ShowIcon.color = new Color(1, 1, 1, 1);
        ShowIcon.rectTransform().anchoredPosition = new Vector2(-300 + (vIndex * 130f), -575);
        ShowIcon.rectTransform().DOAnchorPos(new Vector2(0, 500), 0.6f).Play();
        ShowIcon.DOFade(0, 0.6f).SetDelay(0.3f).OnComplete(() => 
        { 
            HideSelectGroup();
        }).Play();

        GameHttpNet.Instance.SendEmojiMsgItem(
#if NOT_USE_LUA
            DialogDisplaySystem.Instance.CurrentBookData.BookID, 
            DialogDisplaySystem.Instance.CurrentDialogData.dialogID, 
#else
            BookReadingWrapper.Instance.CurrentBookData.BookID,
            BookReadingWrapper.Instance.CurrentBookData.DialogueID,
#endif
            (vIndex + 1), SendEmojiItemHandler);

        //LOG.Info("---ItemClickHandler---->" + vIndex);
    }

    private void SendEmojiItemHandler(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----SendEmojiItemHandler---->" + result);
    }
    public override void CustomUpdate()
    {
        base.CustomUpdate();
        mTimes += Time.deltaTime;
        if(mTimes >= mInterval)
        {
            mInterval =(float)(Random.Range(2, 15) * 1.0f / 10.0f);
            mTimes = 0;
            ShowOneFace();
        }

        mOpenFormTime += Time.deltaTime;
        if(mOpenFormTime >= 10)
        {
            mOpenFormTime = 0;
            if(!mIsSelected)
            {
                mIsSelected = true;
                HideSelectGroup();
            }
        }
    }

    private void ShowSelectGroup()
    {
        SelectGroup.rectTransform().anchoredPosition = new Vector2(-680, 200);
        SelectGroup.DOAnchorPosX(-110, 0.8f).SetDelay(0.5f).SetEase(Ease.OutBack).Play();
    }

    private void HideSelectGroup()
    {
        SelectGroup.rectTransform().anchoredPosition = new Vector2(-110, 200);
        SelectGroup.DOAnchorPosX(-680, 0.6f).SetEase(Ease.InBack).OnComplete(() => { mCanCloseForm = true; }).Play();
    }

    private void ShowOneFace()
    {
        if(GetLeftFaceNum()>0)
        {
            EmojiMsgItemForm item = GetItem();
            item.gameObject.SetActive(true);
            item.Init(mType,GetNextFaceIndex(), ItemFinishCallBack);
        }
        
    }

    private int GetNextFaceIndex()
    {
        List<int> leftFaceType = GetLeftFaceTypeList();
        int len = leftFaceType.Count;
        if(len>0)
        {
            int index = 0;
            if(len>1)
                index = Random.Range(0, len);
            int resultType = leftFaceType[index];
            CalularFaceNum(resultType);
            return resultType;
        }
        return 0;
    }
    private List<int> GetLeftFaceTypeList()
    {
        List<int> leftFaceType = new List<int>();
        if (mSelect1 > 0)
            leftFaceType.Add(1);
        if (mSelect2 > 0)
            leftFaceType.Add(2);
        if (mSelect3 > 0)
            leftFaceType.Add(3);
        if (mSelect4 > 0)
            leftFaceType.Add(4);
        return leftFaceType;
    }

    private void CalularFaceNum(int vType,bool vIsSub = true)
    {
        switch(vType)
        {
            case 1:
                mSelect1 = vIsSub ? (mSelect1 - 1) : (mSelect1 + 1);
                break;
            case 2:
                mSelect2 = vIsSub ? (mSelect2 - 1) : (mSelect2 + 1);
                break;
            case 3:
                mSelect3 = vIsSub ? (mSelect3 - 1) : (mSelect3 + 1);
                break;
            case 4:
                mSelect4 = vIsSub ? (mSelect4 - 1) : (mSelect4 + 1);
                break;
        }
    }

    private int GetLeftFaceNum()
    {
        return mSelect1 + mSelect2 + mSelect3 + mSelect4;
    }

    private void ItemFinishCallBack(EmojiMsgItemForm vItem)
    {
        vItem.gameObject.SetActive(false);
        if (mItemList != null)
            mItemList.Add(vItem);
    }

    private EmojiMsgItemForm GetItem()
    {
        EmojiMsgItemForm item;
        if (mItemList != null && mItemList.Count > 0)
       {
           item = mItemList[0];
           mItemList.RemoveAt(0);
       }
       else
       {
           GameObject go = GameObject.Instantiate(ItemChild, ItemFather.transform);
           var t = go.transform;
           t.localPosition = Vector3.zero;
           t.localScale = Vector3.one;
           t.localRotation = Quaternion.identity;
           item = go.GetComponent<EmojiMsgItemForm>();
           item.gameObject.SetActive(false);
       }
       return item;
    }

    private EmojiMsgSelectItemForm getSelectItem()
    {
        EmojiMsgSelectItemForm item;
        GameObject go = GameObject.Instantiate(SelectItemChild, SelectItemFather.transform);
        var t = go.transform;
        t.localPosition = Vector3.zero;
        t.localScale = Vector3.one;
        t.localRotation = Quaternion.identity;
        item = go.GetComponent<EmojiMsgSelectItemForm>();
        return item;
    }

    private void CloseFormHandler(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        if(mCanCloseForm)
        {
            CUIManager.Instance.CloseForm(UIFormName.EmojiMsgForm);
            EventDispatcher.Dispatch(EventEnum.ChangeBookReadingBgEnable, 1);
            EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, null);
        }
    }
}

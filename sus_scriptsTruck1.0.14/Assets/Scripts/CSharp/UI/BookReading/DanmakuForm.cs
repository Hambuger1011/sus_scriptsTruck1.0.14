using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using UGUI;
using SuperScrollView;

/// <summary>
/// 弹幕界面
/// </summary>
public class DanmakuForm : BaseUIForm
{

    public GameObject UIMask;
    public RectTransform Content;
    public GameObject ItemsPool;
    public GameObject ItemFather;
    public GameObject ItemChild;
    public RectTransform SelectGroup;
    public InputField InputTxt;
    public GameObject SendBtn,ReturnBtn;
    public Image Type1Btn, Type2Btn, Type3Btn;
    public CanvasGroup ShowTypeCanvas;


    Dictionary<int, List<DanmakuItemView>> typeItemList;
    Dictionary<int, DanmakuItemView> lastItemDic;
    Dictionary<int, int> curShowDanmakuDic;
    private List<DanmakuItemView> mItemList;
    private List<BookBarrageItemInfo> mBarrageInfoList;


    private float mInterval = 0.4f;
    private float mTimes = 0;
    private float mOpenFormTime = 0;
    private int mType = 2;
    private int onceShowNum = 4;
    private int typeIndexOffet = 0;

    private bool InClose = false;
    private bool mCanCloseForm = false;

    private int mLastSendMsgTime = 0;


    public override void OnOpen()
    {
        if (GameDataMgr.Instance.InAutoPlay)
        {
            GameDataMgr.Instance.AutoPlayPause = true;
            GameDataMgr.Instance.InAutoPlay = false;
        }
        base.OnOpen();
        InClose = false;
        mCanCloseForm = false;

        mInterval = 0.2f;
        if (mItemList == null)
            mItemList = new List<DanmakuItemView>();

        if (typeItemList == null)
            typeItemList = new Dictionary<int, List<DanmakuItemView>>();

        if (lastItemDic == null)
            lastItemDic = new Dictionary<int, DanmakuItemView>();

        if (curShowDanmakuDic == null)
            curShowDanmakuDic = new Dictionary<int, int>();

        mBarrageInfoList = new List<BookBarrageItemInfo>();

        for (int i = 0;i< onceShowNum; i++)
        {
            curShowDanmakuDic[i + 1] = 0;
        }

        InputTxt.shouldHideMobileInput = true;

        //UIEventListener.AddOnClickListener(UIMask.gameObject, CloseFormHandler);
        UIEventListener.AddOnClickListener(SendBtn, SendMsgHandler);
        UIEventListener.AddOnClickListener(ReturnBtn, CloseFormHandler);
        UIEventListener.AddOnClickListener(Type1Btn.gameObject, SelectType1Handler);
        UIEventListener.AddOnClickListener(Type2Btn.gameObject, SelectType2Handler);
        UIEventListener.AddOnClickListener(Type3Btn.gameObject, SelectType3Handler);


        Type2Btn.sprite = ResourceManager.Instance.GetUISprite("DanmakuForm/btn_list_selected_2");
        ShowTypeCanvas.DOKill();
        ShowTypeCanvas.alpha = 0;
        GetBarrageByPage(1);
        ShowSelectGroup();

#if ENABLE_DEBUG
        if (GameDataMgr.Instance.InAutoPlay)
        {
            Invoke("AutoDo", 1f);
        }
#endif
    }

    public void Init(int vType)
    {
        
    }


    public override void OnClose()
    {
        if (GameDataMgr.Instance.AutoPlayPause)
        {
            GameDataMgr.Instance.AutoPlayPause = false;
            GameDataMgr.Instance.InAutoPlay = true;
        }
        base.OnClose();
        UIEventListener.RemoveOnClickListener(UIMask.gameObject, CloseFormHandler);
        UIEventListener.RemoveOnClickListener(ReturnBtn, CloseFormHandler);
        UIEventListener.RemoveOnClickListener(Type1Btn.gameObject, SelectType1Handler);
        UIEventListener.RemoveOnClickListener(Type2Btn.gameObject, SelectType2Handler);
        UIEventListener.RemoveOnClickListener(Type3Btn.gameObject, SelectType3Handler);

        if(mItemList != null)
        {
            int len = mItemList.Count;
            for(int i = 0;i<len;i++)
            {
                DanmakuItemView tempItem = mItemList[i];
                if (tempItem != null)
                    tempItem.Dispose();
                tempItem = null;
            }
            mItemList.Clear();
        }
    }

    private void SelectType1Handler(PointerEventData data)
    {
        if (mType != 1)
            ResetType();
        typeIndexOffet = 0;
        mType = 1;
        ChangeTypeIconShow();
    }

    private void SelectType2Handler(PointerEventData data)
    {
        if (mType != 2)
            ResetType();
        mType = 2;
        ChangeTypeIconShow();
    }

    private void SelectType3Handler(PointerEventData data)
    {
        if (mType != 3)
            ResetType();
        mType = 3;
        ChangeTypeIconShow();
    }

    private void ChangeTypeIconShow()
    {
        switch (mType)
        {
            case 1:
                Type1Btn.sprite = ResourceManager.Instance.GetUISprite("DanmakuForm/btn_list_selected_1");
                Type2Btn.sprite = ResourceManager.Instance.GetUISprite("DanmakuForm/btn_list_normal_2");
                Type3Btn.sprite = ResourceManager.Instance.GetUISprite("DanmakuForm/btn_list_normal_3");
                break;
            case 2:
                Type1Btn.sprite = ResourceManager.Instance.GetUISprite("DanmakuForm/btn_list_normal_1");
                Type2Btn.sprite = ResourceManager.Instance.GetUISprite("DanmakuForm/btn_list_selected_2");
                Type3Btn.sprite = ResourceManager.Instance.GetUISprite("DanmakuForm/btn_list_normal_3");
                break;
            case 3:
                Type1Btn.sprite = ResourceManager.Instance.GetUISprite("DanmakuForm/btn_list_normal_1");
                Type2Btn.sprite = ResourceManager.Instance.GetUISprite("DanmakuForm/btn_list_normal_2");
                Type3Btn.sprite = ResourceManager.Instance.GetUISprite("DanmakuForm/btn_list_selected_3");
                break;
        }

        mTimes = 1;
    }


    private void AutoDo()
    {
        CancelInvoke("AutoDo");
        //ItemClickHandler(0);
    }

    private void GetBarrageByPage(int vPage)
    {
        GameHttpNet.Instance.GetBookBarrageInfoList(
            BookReadingWrapper.Instance.CurrentBookData.BookID,
            BookReadingWrapper.Instance.CurrentBookData.DialogueID,
            vPage,
            GetBookBarrageInfoHandler
            );
    }

    private void GetBookBarrageInfoHandler(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetBookBarrageInfoHandler---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.BookBarrageInfoList = JsonHelper.JsonToObject<HttpInfoReturn<BookBarrageInfoList>>(result);
                    if(UserDataManager.Instance.BookBarrageInfoList != null && UserDataManager.Instance.BookBarrageInfoList.data != null && UserDataManager.Instance.BookBarrageInfoList.data.data != null)
                    {
                        int len = UserDataManager.Instance.BookBarrageInfoList.data.data.Count;
                        for(int i = 0;i<len; i++)
                        {
                            mBarrageInfoList.Add(UserDataManager.Instance.BookBarrageInfoList.data.data[i]);
                        }
                    }
                }
            }, null);
        }
    }

    private void SendMsgHandler(PointerEventData data)
    {
        if(GameDataMgr.Instance.GetLocalUtcTimestamp() - mLastSendMsgTime < 3)
        {

            return;
        }

        string msgStr = InputTxt.text;
        if(string.IsNullOrEmpty(msgStr))
        {
            
            return;
        }

        
        GamePointManager.Instance.BuriedPoint(EventEnum.AddBarrage,"","",BookReadingWrapper.Instance.CurrentBookData.BookID.ToString(),BookReadingWrapper.Instance.CurrentBookData.DialogueID.ToString());
        GameHttpNet.Instance.CreateBookBarrage(BookReadingWrapper.Instance.CurrentBookData.BookID,
            BookReadingWrapper.Instance.CurrentBookData.DialogueID, msgStr, SendBarrageHandler);
        
        EventDispatcher.Dispatch(EventEnum.CreateBookBarrageSuccess);

        BookBarrageItemInfo tempItem = new BookBarrageItemInfo();
        tempItem.nickname = UserDataManager.Instance.userInfo.data.userinfo.nickname;
        tempItem.avatar = UserDataManager.Instance.userInfo.data.userinfo.avatar;
        tempItem.avatar_frame = UserDataManager.Instance.userInfo.data.userinfo.avatar_frame;
        tempItem.content = msgStr;
        tempItem.create_time = GameDataMgr.Instance.GetLocalUtcTimestamp();

        mBarrageInfoList.Add(tempItem);

        InputTxt.text = "";

        mLastSendMsgTime = GameDataMgr.Instance.GetLocalUtcTimestamp();
    }

    private void SendBarrageHandler(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----SendBarrageHandler---->" + result);
    }
    public override void CustomUpdate()
    {
        base.CustomUpdate();

        if (InClose) return;
        mTimes += Time.deltaTime;
        if(mTimes >= mInterval)
        {
            mTimes = 0;
            if (mType == 1)
            {
                mInterval = 0.2f;
                if(typeIndexOffet <onceShowNum)
                {
                    typeIndexOffet++;
                    ShowDanmaku(typeIndexOffet);
                }    
                else
                {
                    typeIndexOffet = 0;
                    mInterval = 3f;
                }
            }
            else if(mType == 2)
            {
                mInterval = 1.7f;
                ShowDanmaku(1);
            }
            else if (mType == 3)
            {
                mInterval = 0.5f;
                ShowDanmaku(1);
            }
            CheckLoadNextPageInfo();
        }
    }

    private void ShowSelectGroup()
    {
        SelectGroup.rectTransform().anchoredPosition = new Vector2(-800, 0);
        SelectGroup.DOAnchorPosX(0, 0.4f).SetDelay(0.1f).SetEase(Ease.Flash).OnComplete(() => {
            ShowTypeCanvas.DOFade(1, 0.3f).Play();
            mCanCloseForm = true; 
        }).Play();
    }

    private void HideSelectGroup()
    {
        SelectGroup.rectTransform().anchoredPosition = new Vector2(0, 0);
        ShowTypeCanvas.DOFade(0, 0.3f).OnComplete(() => {
            SelectGroup.DOAnchorPosX(-680, 0.3f).SetEase(Ease.Flash).OnComplete(() =>
            {
                if (mCanCloseForm)
                {
                    CUIManager.Instance.CloseForm(UIFormName.DanmakuForm);
                    //EventDispatcher.Dispatch(EventEnum.ChangeBookReadingBgEnable, 1);
                    //EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, null);
                }
            }).Play();
        }).Play();
        
    }

    private void ShowDanmaku(int typeIndex)
    {
        if(GetLeftBarrageNum()>0)
        {
            int cacheNum = 4;
            if (mType == 1)
                cacheNum = 4;
            else if (mType == 2)
                cacheNum = 8;
            else if (mType == 3)
                cacheNum = 6;
            if (curShowDanmakuDic.ContainsKey(typeIndex) &&  curShowDanmakuDic[typeIndex] > cacheNum) return;
            DanmakuItemView item = GetItem(typeIndex);
            item.transform.SetParent(ItemFather.transform);
            item.gameObject.SetActive(true);
            float lastItemX = Screen.width * 0.5f;
            float lastItemY= 0;
            float lastItemWidth = 100;
            float lastItemHeight = 80;
            if (lastItemDic.ContainsKey(typeIndex))//判断最后一个位置
            {
                lastItemX = lastItemDic[typeIndex].GetCurPosX();
                lastItemY = lastItemDic[typeIndex].GetCurPosY();
                lastItemWidth = lastItemDic[typeIndex].GetItemWidth();
                lastItemHeight = lastItemDic[typeIndex].GetItemHeight();
            }

            BookBarrageItemInfo tempItemInfo = mBarrageInfoList[0];
            mBarrageInfoList.RemoveAt(0);

            item.Init(mType, typeIndex,lastItemX,lastItemY,tempItemInfo, new Vector2(lastItemWidth,lastItemHeight) , ItemFinishCallBack);

            if (lastItemDic.ContainsKey(typeIndex))
                lastItemDic[typeIndex] = item;
            else
                lastItemDic.Add(typeIndex, item);

            if(curShowDanmakuDic.ContainsKey(typeIndex))
            {
                curShowDanmakuDic[typeIndex] += 1;
            }
        }
    }


    //检查是否需要加载下一页的内容
    private void CheckLoadNextPageInfo()
    {
        int leftNum = mBarrageInfoList.Count;
        if (UserDataManager.Instance.BookBarrageInfoList != null && UserDataManager.Instance.BookBarrageInfoList.data != null)
        {
            if(leftNum < onceShowNum && UserDataManager.Instance.BookBarrageInfoList.data.current_page < UserDataManager.Instance.BookBarrageInfoList.data.page_count)
            {
                GetBarrageByPage(UserDataManager.Instance.BookBarrageInfoList.data.current_page + 1);
            }
        }
    }

    private int GetLeftBarrageNum()
    {
        return mBarrageInfoList.Count;
    }

    private void ItemFinishCallBack(DanmakuItemView vItem,bool vIsFinish)
    {
        vItem.gameObject.SetActive(false);
        vItem.transform.SetParent(ItemsPool.transform);
        int itemIndex = vItem.GetItemIndex();
        if (curShowDanmakuDic.ContainsKey(itemIndex) && curShowDanmakuDic[itemIndex]>0)
            curShowDanmakuDic[itemIndex] = curShowDanmakuDic[itemIndex] - 1;
        if (typeItemList != null)
        {
            if(typeItemList.ContainsKey(itemIndex))
            {
                typeItemList[itemIndex].Add(vItem);
            }
            else
            {
                List<DanmakuItemView> tempList = new List<DanmakuItemView>();
                tempList.Add(vItem);
                typeItemList.Add(itemIndex, tempList);
            }
        }
        if(vItem!= null && vItem.GetItemInfo() != null)
        {
            mBarrageInfoList.Add(vItem.GetItemInfo());
        }
    }

    private DanmakuItemView GetItem(int vIndex)
    {
        DanmakuItemView item;
        List<DanmakuItemView> tempList;
        if(typeItemList != null && typeItemList.ContainsKey(vIndex) && typeItemList[vIndex].Count > 0)
        {
            tempList = typeItemList[vIndex];
            item = tempList[0];
            tempList.RemoveAt(0);
            item.transform.SetAsFirstSibling();
        }
       else
       {
           GameObject go = GameObject.Instantiate(ItemChild, ItemsPool.transform);
           var t = go.transform;
           t.localPosition = Vector3.zero;
           t.localScale = Vector3.one;
           t.localRotation = Quaternion.identity;
           item = go.GetComponent<DanmakuItemView>();
           item.gameObject.SetActive(false);
            mItemList.Add(item);
       }
       return item;
    }

    private void ResetType()
    {
        EventDispatcher.Dispatch(EventEnum.DanmakuChangeShowType);

        if (lastItemDic != null)
            lastItemDic.Clear();

        for (int i = 0; i < onceShowNum; i++)
        {
            curShowDanmakuDic[i + 1] = 0;
        }
    }

    private void CloseFormHandler(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        InClose = true;
        HideSelectGroup();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UGUI;
using AB;
using pb;
using System;
using Random = UnityEngine.Random;

#if !NOT_USE_LUA
using DialogDisplaySystem = BookReadingWrapper;
#endif

/// <summary>
/// 拼图界面
/// </summary>
[XLua.LuaCallCSharp]
public class PuzzleForm : BaseUIForm
{

    public GameObject UIMask;
    public RectTransform Content;
    public GameObject ItemFather;
    public GameObject ItemChild;

    public Image RoleImage;
    public Image NpcImage;


    public Image PuzzleBg;
    public CanvasGroup CanGroup;

    public Text DescText;


    private List<Vector2> itemPosList;
    private List<PuzzleItemForm> itemViewList;

    private List<Vector2> mItemFixPosList;
    private int mFixX = 250;
    private int mFixY = 180;

    private int mItemLen;
    private int mCurFinishNum;

    private MetaGameData mMetaGameData;
    public override void OnOpen()
    {
        base.OnOpen();
        mItemLen = 0;
        mCurFinishNum = 0;
    }


#if !NOT_USE_LUA
    Action onComplete;
    public void Init(int vMetaGameId, Action callback)
    {
        this.onComplete = callback;
#else
    public void Init(int vMetaGameId)
    {
#endif
        t_MetaGameDetails metaGameCfg = GameDataMgr.Instance.table.GetMetaGameById(vMetaGameId);
        if(metaGameCfg != null)
        {
            mMetaGameData = new MetaGameData(metaGameCfg);
            if(mMetaGameData != null)
                InitPuzzleItem();
        }
        else
        {
            LOG.Error("小游戏配置有错--->"+vMetaGameId);
        }
        
    }

    private void InitPuzzleItem()
    {
        InitRole();
        InitPost();

        int vType = mMetaGameData.ShapeDetails;
        DescText.text = mMetaGameData.Description;
        PuzzleBg.sprite = ABSystem.ui.GetUITexture(AbTag.Global,string.Concat("Assets/Bundle/Puzzle/" + vType + "_0", ".png"));
        PuzzleBg.rectTransform.anchoredPosition = Vector2.zero;
        PuzzleBg.rectTransform.localScale = Vector3.one;
        PuzzleBg.SetNativeSize();
        CanGroup.alpha = 1;
        itemViewList = new List<PuzzleItemForm>();
        int len = mMetaGameData.Pieces;
        List<Vector2> randPosList = mItemFixPosList.GetRange(0, len);
        for (int i = 0; i < len; i++)
        {
            PuzzleItemForm item = GetItem();
            if(item != null)
            {
                item.Init(itemPosList[i], vType, i + 1, ItemFinishHandler);
                itemViewList.Add(item);

                int tempRank = Random.Range(0, randPosList.Count);
                Vector2 RandPos = randPosList[tempRank];
                randPosList.RemoveAt(tempRank);

                int randX = (RandPos.x <= 0) ? Random.Range(-150, -80) : Random.Range(80, 150);
                int randY = (RandPos.y <= 0) ? Random.Range(-110, -40) : Random.Range(40, 110);
                item.transform.localPosition = new Vector3(RandPos.x * mFixX, RandPos.y * mFixY, 0f);
            }
        }
        mItemLen = itemViewList.Count;


        ShowRole();

#if ENABLE_DEBUG
        if (GameDataMgr.Instance.InAutoPlay)
        {
            Invoke("AutoDo", 1.5f);
        }
#endif
    }

    private void AutoDo()
    {
        CancelInvoke("AutoDo");
        mCurFinishNum = mItemLen;
        ItemFinishHandler();
    }
    private void InitPost()
    {
        mItemFixPosList = new List<Vector2>();
        mItemFixPosList.Add(new Vector2(-1, 1));
        mItemFixPosList.Add(new Vector2(1, 1));
        mItemFixPosList.Add(new Vector2(1, -1));
        mItemFixPosList.Add(new Vector2(-1, -1));
        mItemFixPosList.Add(new Vector2(-1, 0));
        mItemFixPosList.Add(new Vector2(1, 0));
        mItemFixPosList.Add(new Vector2(0, 1));
        mItemFixPosList.Add(new Vector2(0, -1));


        itemPosList = new List<Vector2>();
        int len = mMetaGameData.Pieces;
        for (int i = 0; i < len; i++)
        {
            Vector2 pos = ConvertPosVector2(mMetaGameData.GetPostList()[i]);
            itemPosList.Add(pos);
        }
    }

    private void InitRole()
    {
        int clothesID = 0;
        if (mMetaGameData.CharacterA == 0)
            clothesID = 0;
        else if(mMetaGameData.CharacterA > 0)
            clothesID = DialogDisplaySystem.Instance.CurrentBookData.PlayerClothes;

        if(mMetaGameData.CharacterA >=0)
        {
            int roleImageId = (100000 + (1 * 10000) + (1 * 100) + clothesID) * 10000;
            RoleImage.sprite = DialogDisplaySystem.Instance.GetUITexture("RoleHead/" + roleImageId, false);
            RoleImage.transform.localScale = new Vector3(-1, 1, 1);
        }
        
        if(mMetaGameData.CharacterB > 0)
        {
            int npcDetailId = 0;
            if (mMetaGameData.CharacterB == DialogDisplaySystem.Instance.CurrentBookData.NpcId)
                npcDetailId = DialogDisplaySystem.Instance.CurrentBookData.NpcDetailId;

            int npcImageId = (100000 + (mMetaGameData.CharacterB * 100) + mMetaGameData.CharacterBClothes) * 10000 + npcDetailId;
            NpcImage.sprite = DialogDisplaySystem.Instance.GetUITexture("RoleHead/" + npcImageId, false);
        }
    }

    private void ShowRole()
    {
        RoleImage.rectTransform.anchoredPosition = new Vector2(-600, 860);
        NpcImage.rectTransform.anchoredPosition = new Vector2(600, 860);

        if(mMetaGameData.CharacterA >=0)
            RoleImage.rectTransform.DOAnchorPosX(-160, 0.4f).SetDelay(1f).Play();
        
        if(mMetaGameData.CharacterB > 0)
            NpcImage.rectTransform.DOAnchorPosX(160, 0.4f).SetDelay(0.8f).Play();

        Content.anchoredPosition = new Vector2(0, -1334);
        Content.DOAnchorPosY(253, 0.8f).SetEase(Ease.InOutBack).Play();
    }

    private void HideRole()
    {
        if(mMetaGameData.CharacterA>=0)
        {
            RoleImage.rectTransform.anchoredPosition = new Vector2(-160, 860);
            RoleImage.rectTransform.DOAnchorPosX(-600, 0.4f).Play();
        }
        
        if(mMetaGameData.CharacterB > 0)
        {
            NpcImage.rectTransform.anchoredPosition = new Vector2(160, 860);
            NpcImage.rectTransform.DOAnchorPosX(600, 0.4f).SetDelay(0.2f).Play();
        }

        Content.anchoredPosition = new Vector2(0, 253);
        Content.DOAnchorPosY(-1334, 1f).SetDelay(0.3f).OnComplete(() =>
        {
            EventDispatcher.Dispatch(EventEnum.ChangeBookReadingBgEnable, 1);
            EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, null);
            CUIManager.Instance.CloseForm(UIFormName.PuzzleForm);

#if !NOT_USE_LUA
            this.onComplete();
#endif

        }).SetEase(Ease.InOutBack).Play();
    }

    private Vector2 ConvertPosVector2(string vPosStr)
    {
        if(!string.IsNullOrEmpty(vPosStr))
        {
            string[] tempList = vPosStr.Split(',');
            if (tempList.Length > 1)
                return new Vector2(float.Parse(tempList[0]), float.Parse(tempList[1]));
            else
                LOG.Error(" 小游戏 坐标配置有错 gameId:"+mMetaGameData.ID +"==str==>"+vPosStr);
        }
        return Vector2.zero;
    }

    private PuzzleItemForm GetItem()
    {
        GameObject go = GameObject.Instantiate(ItemChild, ItemFather.transform);
        var t = go.transform;
        t.localPosition = Vector3.zero;
        t.localScale = Vector3.one;
        t.localRotation = Quaternion.identity;
        PuzzleItemForm item = go.GetComponent<PuzzleItemForm>();
        return item;
    }

    private void ItemFinishHandler()
    {
        mCurFinishNum++;
        if(mCurFinishNum>=mItemLen)
        {
            CanGroup.DOFade(0, 0.8f).SetDelay(0.5f).OnComplete(() => { }).Play();
            PuzzleBg.rectTransform.DOAnchorPos(new Vector2(0, 600), 0.8f).SetDelay(0.3f).Play();
            PuzzleBg.rectTransform.DOScale(2f, 0.8f).SetDelay(0.3f).OnComplete(() =>
            {
                HideRole();
            }).Play();
        }
    }
}

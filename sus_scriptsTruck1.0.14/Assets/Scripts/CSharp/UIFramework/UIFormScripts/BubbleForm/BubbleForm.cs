using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;
using pb;
using UGUI;
using UI;

#if !NOT_USE_LUA
using DialogDisplaySystem = BookReadingWrapper;
#endif

/// <summary>
/// 气泡聊天界面
/// </summary>

[XLua.Hotfix, XLua.CSharpCallLua, XLua.LuaCallCSharp]

public class BubbleForm : MonoBehaviour
{
    public UIBubbleEvent bubbleEvt;
    public ScrollRect scrollRect;
    public UIBubbleList list;
    public GameObject[] itemPfbs;
    public Button btnShowImage;
    public Image picture;
    public RectTransform PhoneCallBG;
    public UIBubbleChoicesBtnGroup SelectionButtonGroup;

    private CUIForm uiform;
    int idx = 0;
    bool inOpenOrClose = false;
    bool inSelection = false;
    bool isTween = false;

    
    void Awake()
    {
        uiform = this.GetComponent<CUIForm>();
        UIBubbleFactory.itemPfbs = itemPfbs;
        list.OnItemClick = OnItemClick;
        scrollRect.gameObject.SetActive(false);

        btnShowImage.onClick.AddListener(OnHideImage);
        bubbleEvt.onClick.AddListener(OnClick);

        float mScale = 1f;
        if (GameUtility.IpadAspectRatio())
            mScale = 0.7f;

        PhoneCallBG.transform.localScale = Vector3.one * mScale;

        if (SelectionButtonGroup != null)
        {
            SelectionButtonGroup.Init();
            SelectionButtonGroup.gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        btnShowImage.onClick.RemoveListener(OnHideImage);
        bubbleEvt.onClick.RemoveListener(OnClick);
    }


    public void ShowOrHideView(bool value,Action callback)
    {
        if (inOpenOrClose) return;
        inOpenOrClose = true;

        int targetPosY = 0;
        if (GameUtility.IpadAspectRatio())
            targetPosY = -167;

        Vector2 startPos = value ? new Vector2(-750, -857) : new Vector2(0, targetPosY);
        Vector2 endPos = value ? new Vector2(0, targetPosY) : new Vector2(-750, -857);
        this.PhoneCallBG.anchoredPosition = startPos;
        this.PhoneCallBG.DOAnchorPos(endPos, 0.6f)
            .OnStart(() => { })
            .OnComplete(() =>
            {
                if (value)
                {
                    scrollRect.gameObject.SetActive(true);
                    //OnClick();
                    if (callback != null)
                    {
                        callback();
                    }
                }
                else
                {
                    if (SelectionButtonGroup != null)
                        SelectionButtonGroup.Dispose();

                    //CUIManager.Instance.CloseForm(UIFormName.BubbleForm);
                    EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, null);
                    EventDispatcher.Dispatch(EventEnum.ChangeBookReadingBgEnable, 1);

                    var bookData = BookReadingWrapper.Instance.CurrentBookData;

                    var cfg = JsonDTManager.Instance.GetDialogItem(DialogDisplaySystem.Instance.CurrentBookData.BookID,DialogDisplaySystem.Instance.CurrentBookData.ChapterID,DialogDisplaySystem.Instance.CurrentBookData.DialogueID);//BookReadingWrapper.Instance.GetDialogById(DialogDisplaySystem.Instance.CurrentBookData.DialogueID);
                    var data = new BaseDialogData(cfg);
                    if (data.selection_num == 0)
                    {
                        GameHttpNet.Instance.SendPlayerProgress(UserDataManager.Instance.UserData.CurSelectBookID,
                            DialogDisplaySystem.Instance.CurrentBookData.ChapterID, DialogDisplaySystem.Instance.CurrentBookData.DialogueID,RecordChapterProgress);
                    }
                    if(callback != null)
                    {
                        callback();
                    }
                }
                inOpenOrClose = false;
            });
    }

    public void DialogNextStepByDialogID(int id)
    {
        var cfg = JsonDTManager.Instance.GetDialogItem(DialogDisplaySystem.Instance.CurrentBookData.BookID,DialogDisplaySystem.Instance.CurrentBookData.ChapterID,id);//BookReadingWrapper.Instance.GetDialogById(DialogDisplaySystem.Instance.CurrentBookData.DialogueID);
        var data = new BaseDialogData(cfg);
        DialogNextStep(data);
    }

    public void DialogNextStep(BaseDialogData cfg)
    {
        if (cfg == null) return;
        UIBubbleData data = new UIBubbleData(cfg);
        list.AddItem(data);
        isTween = true;
        data.ui.isTween = true;
        data.ui.DOFade(1, 0.6f);
        scrollRect.DOVerticalNormalizedPos(0, 0.5f).OnComplete(() =>
        {
            data.ui.isTween = false;
            isTween = false;
        });

        if(cfg.trigger == 1 & cfg.selection_num > 0)
        {
            inSelection = true;

            if (SelectionButtonGroup != null)
            {
                SelectionButtonGroup.gameObject.SetActive(true);
                SelectionButtonGroup.choicesDialogInit(cfg);
            }
        }else
        {
            inSelection = false;
        }
    }

    
    public void OnClick()
    {
        if (isTween || inSelection)
        {
            return;
        }

        EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, null);

#if test
            list.AddItem(SpawnItem());
            isTween = true;
            var data = list.Items[list.Items.Count - 1];
            data.ui.isTween = true;
            data.ui.DOFade(1, 0.6f);
            scrollRect.DOVerticalNormalizedPos(0, 0.5f).OnComplete(()=>
            {
                data.ui.isTween = false;
                isTween = false;
            });

        if (idx >= cfgs.Count)
        {
            return;
        }
        var cfg = cfgs[idx++];
        UIBubbleData data = new UIBubbleData(cfg);
        list.AddItem(data);
        isTween = true;
        data.ui.isTween = true;
        data.ui.DOFade(1, 0.6f);
        scrollRect.DOVerticalNormalizedPos(0, 0.5f).OnComplete(() =>
        {
            data.ui.isTween = false;
            isTween = false;
        });
#endif
    }
    
    private void OnHideImage()
    {
        if (isTween)
        {
            return;
        }
        this.btnShowImage.gameObject.SetActiveEx(false);
    }

    private void OnItemClick(UIBubbleItem item)
    {
        if (item.boxType == EBubbleBoxType.Image)
        {
            if (isTween)
            {
                return;
            }

            var viewSize = uiform.GetViewSize();
            UIBubbleBox_Image img = (UIBubbleBox_Image)item.curBox;

           
            var size = img.imgPicture.rectTransform.rect.size; ;
            var pos = CUIUtility.World_To_UGUI_LocalPoint(uiform.GetCamera(), uiform.GetCamera(), img.imgPicture.transform.position, uiform.rectTransform());
            var offset = new Vector2(0.5f, 0.5f) - img.transform.pivot;
            pos.x += offset.x * size.x;
            pos.y += offset.y * size.y;


            picture.sprite = img.imgPicture.sprite;
            var maskTf = this.btnShowImage.rectTransform();

            maskTf.sizeDelta = size;
            picture.rectTransform.sizeDelta = size;
            maskTf.anchoredPosition = pos;
            maskTf.gameObject.SetActiveEx(true);
            maskTf.DOAnchorPos(new Vector2(0, 0), 0.25f).OnComplete(() =>
            {
                isTween = false;
            }).SetEase(Ease.Flash);
            maskTf.DOSizeDelta(viewSize, 0.25f).SetEase(Ease.Flash);
            picture.rectTransform.DOSizeDelta(new Vector2(viewSize.x, size.y / size.x * viewSize.x), 0.25f).SetEase(Ease.Flash);
        }
    }

    private void RecordChapterProgress(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----StartReadChapterCallBack---->" + result);
    }
}
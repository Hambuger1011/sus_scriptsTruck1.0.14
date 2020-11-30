
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIStoryItems : BaseUIForm
{
    public Button btnConfirm;
    public Image imgIcon;
    public RectTransform frame;
    public Text DescriptionTxt;

#if NOT_USE_LUA
    BaseDialogData mDialogData;
    protected override void Awake()
    {
        base.Awake();
        btnConfirm.onClick.AddListener(OnConfirmClick);
    }

    public void Show(BaseDialogData dialogData)
    {
        mDialogData = dialogData;
        imgIcon.sprite = DialogDisplaySystem.Instance.GetUITexture("StoryItems/icon" + dialogData.selection_1, false);
        DescriptionTxt.text = dialogData.dialog;
        frame.localScale = Vector3.zero;
        frame.DOScale(Vector3.one, 0.75f).SetEase(Ease.OutBack);
    }

    private void OnConfirmClick()
    {
        this.myForm.Close();

        EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, null);
    }
#endif
}
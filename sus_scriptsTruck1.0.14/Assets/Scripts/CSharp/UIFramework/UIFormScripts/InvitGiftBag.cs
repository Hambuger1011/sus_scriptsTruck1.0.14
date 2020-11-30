using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AB;
using UGUI;

public class InvitGiftBag : BaseUIForm {

    private Image BookSprite;
    private GameObject CloseButton;
    private void Awake()
    {
        BookSprite = transform.Find("Frame/Bg/BookSprite").GetComponent<Image>();
        CloseButton = transform.Find("Frame/CloseButton").gameObject;
    }
    public override void OnOpen()
    {
        base.OnOpen();

        UIEventListener.AddOnClickListener(CloseButton,CloseButtonOnclicke);
       
    }

    public override void OnClose()
    {
        base.OnClose();

        UIEventListener.RemoveOnClickListener(CloseButton, CloseButtonOnclicke);
    }

    public void Inite(int bookId)
    {
        BookSprite.sprite = ABSystem.ui.GetUITexture(AbTag.Global,string.Concat("Assets/Bundle/BookPreview/Icon/", bookId, ".png"));
    }
    private void CloseButtonOnclicke(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CUIManager.Instance.CloseForm(UIFormName.InviteGiftBag);
    }
}

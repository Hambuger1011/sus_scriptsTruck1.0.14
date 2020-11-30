using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// 邀请子项
/// </summary>
public class InviteItemView : BaseUIForm 
{
    public Image IconIamge;
    public Text NumText;
    public Text DescText;
    public Image ReceiveBtn;
    public Text BtnTxt;
    public Text ProgressText;
    public Image ProgressBar;

    private InviteUserInfo mInviteUserInfo;
    public InviteItemInfo InviteItemInfo;

    private Action<int> mReceiveCallBack;
    private bool isInitEvent = false;
    private bool canRecieve = false;
   
   
    public void Init(InviteUserInfo vInviteUserInfo,InviteItemInfo vItemInfo,Action<int>vReceiveCallBack = null)
    {
        mInviteUserInfo = vInviteUserInfo;
        InviteItemInfo = vItemInfo;
        mReceiveCallBack = vReceiveCallBack;
        if(!isInitEvent)
        {
            isInitEvent = true;
            UIEventListener.AddOnClickListener(ReceiveBtn.gameObject, ReceiveHandler);
        }

        if(vItemInfo != null && vInviteUserInfo != null)
        {
            IconIamge.sprite = ResourceManager.Instance.GetUISprite("InviteForm/icon_"+vItemInfo.icon);
            DescText.text = vItemInfo.taskddescribe;
            int rewardNum = 0;
            if (vItemInfo.rewarddiamonds>0)
                rewardNum = vItemInfo.rewarddiamonds;
            else if(vItemInfo.rewardkey>0)
                rewardNum = vItemInfo.rewardkey;
            else if (vItemInfo.rewardweek > 0)
                rewardNum = vItemInfo.rewardweek;

            NumText.text = "X" + rewardNum;

            int curValue = 0;
            if (vItemInfo.type == 1)
            {
                ProgressText.text = vInviteUserInfo.exchange_invite + "/" + vItemInfo.param;
                ProgressBar.fillAmount = vInviteUserInfo.exchange_invite / (vItemInfo.param * 1.0f);
                curValue = vInviteUserInfo.exchange_invite;
            }
            else if(vItemInfo.type == 2)
            {
                ProgressText.text = vInviteUserInfo.newuser_invite + "/" + vItemInfo.param;
                ProgressBar.fillAmount = vInviteUserInfo.newuser_invite / (vItemInfo.param * 1.0f);
                curValue = vInviteUserInfo.newuser_invite;
            }

            if (vItemInfo.is_receive == 1)
            {
                BtnTxt.text = "COMPLETE";
                ReceiveBtn.sprite = ResourceManager.Instance.GetUISprite("InviteForm/receiveEnableBtn");
                canRecieve = false;
            }
            else if (curValue >= vItemInfo.param)
            {
                BtnTxt.text = "GATHER";
                ReceiveBtn.sprite = ResourceManager.Instance.GetUISprite("InviteForm/receiveBtn");
                canRecieve = true;
            }
            else
            {
                BtnTxt.text = "GATHER";
                ReceiveBtn.sprite = ResourceManager.Instance.GetUISprite("InviteForm/receiveEnableBtn");
                canRecieve = false;
            }
        }   
    }

    private void ReceiveHandler(PointerEventData data)
    {
        if (!canRecieve) return;
        if(mReceiveCallBack != null)
        {
            mReceiveCallBack(InviteItemInfo.invite_id);
        }
    }

    public void Dispose()
    {
        ReceiveBtn.sprite = null;
        IconIamge.sprite = null;
        UIEventListener.RemoveOnClickListener(ReceiveBtn.gameObject, ReceiveHandler);
        isInitEvent = false;
    }
}

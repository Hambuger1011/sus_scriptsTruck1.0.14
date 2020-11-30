using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Framework;
using UGUI;

[DisallowMultipleComponent]
public class BaseUIForm : CUIComponent
{
    public override void Initialize(CUIForm formScript)
    {
        if (!this.m_isInitialized)
        {
            this.m_customUpdateFlags |= enCustomUpdateFlag.eUpdate;
            base.Initialize(formScript);
        }
    }
   
    public override void OnOpen()
    {
        base.OnOpen();
        m_messageListenerCache.Clear();
    }

    public override void OnClose()
    {
        base.OnClose();
        removeMessageListeners();
    }

    #region EventDispatch
    private Dictionary<string, Action<Notification>> m_messageListenerCache = new Dictionary<string, Action<Notification>>();
    private void InitMessageListenerCache() { m_messageListenerCache = new Dictionary<string, Action<Notification>>(); }
    protected void addMessageListener(string eventName, Action<Notification> call)
    {
        EventDispatcher.AddMessageListener(eventName, call);
        m_messageListenerCache.Add(eventName, call);
    }
    protected void removeMessageListeners()
    {
        foreach (var item in m_messageListenerCache)
        {
            EventDispatcher.RemoveMessageListener(item.Key, item.Value);
        }
        m_messageListenerCache.Clear();
    }
    #endregion
}
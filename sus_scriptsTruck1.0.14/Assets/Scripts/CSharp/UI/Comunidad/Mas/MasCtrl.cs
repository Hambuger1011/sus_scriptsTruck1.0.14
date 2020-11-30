using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MasCtrl : BaseUIForm
{
    [System.NonSerialized]
    public GameObject Close;

    [System.NonSerialized]
    public MasLogic MasLogic;
    [System.NonSerialized]
    public MasDB MasDB;
    [System.NonSerialized]
    public MasView MasView;

    protected override void Awake()
    {
        base.Awake();
        FindGameObject();

        MasLogic = new MasLogic(this);
        MasDB = new MasDB();
        MasView = new MasView(this);

        EventListen();
    }

    public override void OnOpen()
    {
        base.OnOpen();

        MasLogic.Init();
        EventDispatcher.Dispatch(EventEnum.MasOpen);
    }


    public override void OnClose()
    {
        base.OnClose();
        EvenlistenClean();
        MasView.Close();
        MasLogic.Close();

    }

    private void FindGameObject()
    {
        Close = DisplayUtil.GetChild(this.gameObject, "Close");
    }

    private void EventListen()
    {
        UIEventListener.AddOnClickListener(Close,MasLogic.CloseButtonOnclicke);

        addMessageListener(EventEnum.MasClose, MasClose);


    }
    private void EvenlistenClean()
    {
        UIEventListener.RemoveOnClickListener(Close, MasLogic.CloseButtonOnclicke);
    }

    private void MasClose(Notification notice)
    {
        MasLogic.CloseButtonOnclicke(null);
    }
}

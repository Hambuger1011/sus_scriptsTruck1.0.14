using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UGUI;

[XLua.Hotfix, XLua.LuaCallCSharp]
public class NetLoadingForm : BaseUIForm
{
    public GameObject mask;
    public Animation anima;

    private void Awake()
    {
        hide();
    }

    public void showMask()
    {
        mask.SetActive(true);
    }

    public void show()
    {
        anima.gameObject.SetActive(true);
        anima.Play();
    }

    public void hide()
    {
        mask.SetActive(false);
        anima.gameObject.SetActive(false);
        anima.Stop();
    }

    public override void OnOpen()
    {
        base.OnOpen();
    }


    public override void OnClose()
    {
        base.OnClose();
    }
}
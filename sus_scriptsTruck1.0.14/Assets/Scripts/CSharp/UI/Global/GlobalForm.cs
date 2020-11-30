//using HedgehogTeam.EasyTouch;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using Framework;
using HedgehogTeam.EasyTouch;

public class GlobalForm : CMonoSingleton<GlobalForm>
{
    public GameObject touchEffect;
    protected override void Init()
    {
        base.Init();
        EasyTouch.SetUICompatibily(false);
        EnableTouchEffect();
    }

    public void EnableTouchEffect()
    {
        touchEffect.SetActiveEx(true);
    }

    public void DisableTouchEffect()
    {
        touchEffect.SetActiveEx(false);
    }
}

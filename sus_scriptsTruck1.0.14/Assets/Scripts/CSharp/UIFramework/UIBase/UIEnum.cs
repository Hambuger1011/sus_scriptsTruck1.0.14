using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI窗体（位置）类型;普通窗体(共同层级，控件事件同层级);固定窗体（）;弹出窗体（）
/// </summary>
public enum UIFormType
{
    Normal,
    Fixed,
    PopUp
}

/// <summary>
/// UI窗体的显示类型;普通多窗体共存;栈式结构;隐藏其他窗口
/// </summary>
public enum UIFormShowMode
{
    Normal,
    ReverseChange,
    HideOther
}

/// <summary>
/// UI窗体透明度类型;完全透明，不能穿透;半透明，不能穿透;低透明度，不能穿透;可以穿透
/// </summary>
public enum UIFormLucencyType
{
    Lucency,
    Translucence,
    impenetrable,
    Penetrate
}


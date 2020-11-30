using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

[XLua.LuaCallCSharp,XLua.Hotfix]
public class LuaLayoutGroup : LayoutGroup
{
    Action onCalculateLayoutInputVertical;
    Action onSetLayoutHorizontal;
    Action onSetLayoutVertical;

    Action onEnable;
    Action onDisable;
    Action onDestory;

    public void Initialize(LuaTable obj)
    {
        onCalculateLayoutInputVertical = obj.Get<Action>("CalculateLayoutInputVertical");
        onSetLayoutHorizontal = obj.Get<Action>("SetLayoutHorizontal");
        onSetLayoutVertical = obj.Get<Action>("SetLayoutVertical");



        onEnable = obj.Get<Action>("onEnable");
        onDisable = obj.Get<Action>("onDisable");
        onDestory = obj.Get<Action>("onDestory");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (onEnable != null)
        {
            onEnable();
        }

    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (onDisable != null)
        {
            onDisable();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (onDestory != null)
        {
            onDestory();
        }
    }

    public override void CalculateLayoutInputVertical()
    {
        if(onCalculateLayoutInputVertical != null)
        {
            onCalculateLayoutInputVertical();
        }
    }

    public override void SetLayoutHorizontal()
    {
        if (onSetLayoutHorizontal != null)
        {
            onSetLayoutHorizontal();
        }
    }

    public override void SetLayoutVertical()
    {
        if (onSetLayoutVertical != null)
        {
            onSetLayoutVertical();
        }
    }


    [ContextMenu("Test")]
    public void Test()
    {
        var size = this.rectTransform().rect.size;
        size.y += 100;
        this.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
    }
}

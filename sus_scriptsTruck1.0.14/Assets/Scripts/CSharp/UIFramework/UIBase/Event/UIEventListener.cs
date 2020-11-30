using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UIEventTriggerBase;


public delegate void UIVoidPointerEvent(PointerEventData eventData);
public class UIEventListener
{

    //---------------------------------Add---------------------------------------------
    public static void AddOnClickListener(GameObject go, UIVoidPointerEvent func)
    {
        if (hasGraphic(go)) go.AddMissingComponent<UIEventTrigger>().OnClick += func;
        else go.AddMissingComponent<UIEventTrigger_WithoutGraphic>().OnClick += func;
    }
    public static void AddOnPointerEnterListener(GameObject go, UIVoidPointerEvent func)
    {
        if (hasGraphic(go)) go.AddMissingComponent<UIEventTrigger>().OnEnter += func;
        else go.AddMissingComponent<UIEventTrigger_WithoutGraphic>().OnEnter += func;
    }
    public static void AddOnPointerExitListener(GameObject go, UIVoidPointerEvent func)
    {
        if (hasGraphic(go)) go.AddMissingComponent<UIEventTrigger>().OnExit += func;
        else go.AddMissingComponent<UIEventTrigger_WithoutGraphic>().OnExit += func;
    }
    public static void AddOnPointerDownListener(GameObject go, UIVoidPointerEvent func)
    {
        if (hasGraphic(go)) go.AddMissingComponent<UIEventTrigger>().OnDown += func;
        else go.AddMissingComponent<UIEventTrigger_WithoutGraphic>().OnDown += func;
    }
    public static void AddOnPointerUpListener(GameObject go, UIVoidPointerEvent func)
    {
        if (hasGraphic(go)) go.AddMissingComponent<UIEventTrigger>().OnUp += func;
        else go.AddMissingComponent<UIEventTrigger_WithoutGraphic>().OnUp += func;
    }
    public static void AddOnItemClickListener(GameObject go, UIVoidPointerEvent func)
    {
        if (hasGraphic(go)) go.AddMissingComponent<UIEventTrigger>().OnUp += func;
        else go.AddMissingComponent<UIEventTrigger_WithoutGraphic>().OnUp += func;
    }


    //---------------------------------remove---------------------------------------------

    public static void RemoveOnClickListener(GameObject go, UIVoidPointerEvent func)
    {
        if (hasGraphic(go)) go.AddMissingComponent<UIEventTrigger>().OnClick -= func;
        else go.AddMissingComponent<UIEventTrigger_WithoutGraphic>().OnClick -= func;
    }
    public static void RemoveOnPointerEnterListener(GameObject go, UIVoidPointerEvent func)
    {
        if (hasGraphic(go)) go.AddMissingComponent<UIEventTrigger>().OnEnter -= func;
        else go.AddMissingComponent<UIEventTrigger_WithoutGraphic>().OnEnter -= func;
    }
    public static void RemoveOnPointerExitListener(GameObject go, UIVoidPointerEvent func)
    {
        if (hasGraphic(go)) go.AddMissingComponent<UIEventTrigger>().OnExit -= func;
        else go.AddMissingComponent<UIEventTrigger_WithoutGraphic>().OnExit -= func;
    }
    public static void RemoveOnPointerDownListener(GameObject go, UIVoidPointerEvent func)
    {
        if (hasGraphic(go)) go.AddMissingComponent<UIEventTrigger>().OnDown -= func;
        else go.AddMissingComponent<UIEventTrigger_WithoutGraphic>().OnDown -= func;
    }
    public static void RemoveOnPointerUpListener(GameObject go, UIVoidPointerEvent func)
    {
        if (hasGraphic(go)) go.AddMissingComponent<UIEventTrigger>().OnUp -= func;
        else go.AddMissingComponent<UIEventTrigger_WithoutGraphic>().OnUp -= func;
    }
    public static void RemoveOnItemClickListener(GameObject go, UIVoidPointerEvent func)
    {
        if (hasGraphic(go)) go.AddMissingComponent<UIEventTrigger>().OnUp -= func;
        else go.AddMissingComponent<UIEventTrigger_WithoutGraphic>().OnUp -= func;
    }

    private static bool hasGraphic(GameObject go)
    {
        return go.GetComponent<Graphic>() ? true : false;
    }
}
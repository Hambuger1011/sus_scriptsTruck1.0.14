using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 事件派发器，注意调用次序无序
/// </summary>
[XLua.Hotfix, XLua.LuaCallCSharp, XLua.CSharpCallLua]
public class Notification
{
    public object Data { get; set; }
    public string Type { get; set; }
    public Notification(object data, string type = "") { Data = data; Type = type; }
}

public class EventDispatcher
{
    private static Dictionary<string, IList<Action<Notification>>> observers = new Dictionary<string, IList<Action<Notification>>>();

    public static void AddMessageListener(string name, Action<Notification> call)
    {
        //DLOG.Info("EventDispatcher Adding MessageListener >>>>>>>>>> " + name);
        IList<Action<Notification>> funcs;
        if (!observers.TryGetValue(name, out funcs))
        {
            funcs = new List<Action<Notification>>();
            observers[name] = funcs;
        }
        if (funcs.IndexOf(call) == -1)
        {
            funcs.Add(call);
        }
    }

    public static void RemoveMessageListener(string name, Action<Notification> call)
    {
        IList<Action<Notification>> funcs;
        if(observers.TryGetValue(name, out funcs))
        {
            if (funcs != null)
            {
                int index = funcs.IndexOf(call);
                if (index != -1)
                {
                    funcs.RemoveAt(index);
                }
            }
        }
    }

    public static void Dispatch(string name, object data = null, string type = "")
    {
        
        Dispatch(name, new Notification(data, type));
    }

    public static void Dispatch(string name, Notification arg)
    {
        //DLOG.Info("EventDispatcher Dispatching MessageListener ---------- " + name);
        IList<Action<Notification>> funcs;
        if (observers.TryGetValue(name, out funcs))
        {
            int iCount = funcs.Count;
            for (int i = iCount - 1; i > -1; i--)
            {
                funcs[i].Invoke(arg);
            }
        }
    }
}
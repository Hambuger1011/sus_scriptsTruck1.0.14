using Framework;
using System;
using System.Collections.Generic;

public class EventRouter : Framework.CSingleton<EventRouter>
{
	public Dictionary<string, Delegate> m_eventTable = new Dictionary<string, Delegate>();
    bool GetDelegate<T>(ref string eventType, out T action) where T : class
    {
        Delegate d;
        if (m_eventTable.TryGetValue(eventType, out d))
        {
            action = d as T;
            if (action != null)
            {
                return true;
            }
            else
            {
                LOG.Error(string.Format("消息类型错误:base:{0},target{1}", d.GetType(), typeof(Action)));
            }
        }
        action = null;
        return false;
    }

    public void BroadCastEvent<T>(string oN_BATTLE_END, object onBattleEnd)
    {
        throw new NotImplementedException();
    }

    void OnHandlerAdding(ref string eventType, Delegate callback, Delegate handler)
    {
        if(callback != null)
        {
            this.m_eventTable[eventType] = Delegate.Combine(callback, handler);
        }
        else
        {
            this.m_eventTable.Add(eventType, handler);
        }
    }

    void OnHandlerRemoving(ref string eventType, Delegate callback, Delegate handler)
    {
        var d = Delegate.Remove(callback, handler);
        if (d != null)
        {
            this.m_eventTable[eventType] = d;
        }
        else
        {
            this.m_eventTable.Remove(eventType);
        }
    }

    #region 添加
    public void AddEventHandler(string eventType, Action handler)
	{
        Action callback;
        if(GetDelegate<Action>(ref eventType,out callback))
        {
            
        }
        OnHandlerAdding(ref eventType, callback, handler);
    }

	public void AddEventHandler<T1>(string eventType, Action<T1> handler)
	{
        Action<T1> callback;
        if (GetDelegate<Action<T1>>(ref eventType, out callback))
        {
        }
        OnHandlerAdding(ref eventType, callback, handler);
    }

	public void AddEventHandler<T1, T2>(string eventType, Action<T1, T2> handler)
	{
        Action<T1, T2> callback;
        if (GetDelegate<Action<T1, T2>>(ref eventType, out callback))
        {
        }
        OnHandlerAdding(ref eventType, callback, handler);
    }

	public void AddEventHandler<T1, T2, T3>(string eventType, Action<T1, T2, T3> handler)
    {
        Action<T1, T2, T3> callback;
        if (GetDelegate<Action<T1, T2, T3>>(ref eventType, out callback))
        {
        }
        OnHandlerAdding(ref eventType, callback, handler);
    }

	public void AddEventHandler<T1, T2, T3, T4>(string eventType, Action<T1, T2, T3, T4> handler)
    {
        Action<T1, T2, T3, T4> callback;
        if (GetDelegate<Action<T1, T2, T3, T4>>(ref eventType, out callback))
        {
        }
        OnHandlerAdding(ref eventType, callback, handler);
    }
    #endregion


    #region 移除
    public void RemoveEventHandler(string eventType, Action handler)
    {
        Action callback;
        if (GetDelegate<Action>(ref eventType, out callback))
        {
            OnHandlerRemoving(ref eventType, callback, handler);
        }
    }

	public void RemoveEventHandler<T1>(string eventType, Action<T1> handler)
    {
        Action<T1> callback;
        if (GetDelegate<Action<T1>>(ref eventType, out callback))
        {
            OnHandlerRemoving(ref eventType, callback, handler);
        }
    }

	public void RemoveEventHandler<T1, T2>(string eventType, Action<T1, T2> handler)
    {
        Action<T1, T2> callback;
        if (GetDelegate<Action<T1, T2>>(ref eventType, out callback))
        {
            OnHandlerRemoving(ref eventType, callback, handler);
        }
    }

	public void RemoveEventHandler<T1, T2, T3>(string eventType, Action<T1, T2, T3> handler)
    {
        Action<T1, T2, T3> callback;
        if (GetDelegate<Action<T1, T2, T3>>(ref eventType, out callback))
        {
            OnHandlerRemoving(ref eventType, callback, handler);
        }
    }

	public void RemoveEventHandler<T1, T2, T3, T4>(string eventType, Action<T1, T2, T3, T4> handler)
	{
        Action<T1, T2, T3, T4> callback;
        if (GetDelegate<Action<T1, T2, T3, T4>>(ref eventType, out callback))
        {
            OnHandlerRemoving(ref eventType, callback, handler);
        }
	}
    #endregion

    
    #region 广播
    public void BroadCastEvent(string eventType)
    {
        Action callback;
        if (GetDelegate<Action>(ref eventType, out callback))
        {
            callback.Invoke();
        }
    }

	public void BroadCastEvent<T1>(string eventType, T1 arg1)
    {
        Action<T1> callback;
        if (GetDelegate<Action<T1>>(ref eventType, out callback))
        {
            callback.Invoke(arg1);
        }
	}

	public void BroadCastEvent<T1, T2>(string eventType, T1 arg1, T2 arg2)
	{
        Action<T1, T2> callback;
        if (GetDelegate<Action<T1, T2>>(ref eventType, out callback))
        {
            callback.Invoke(arg1, arg2);
        }
	}

	public void BroadCastEvent<T1, T2, T3>(string eventType, T1 arg1, T2 arg2, T3 arg3)
    {
        Action<T1, T2, T3> callback;
        if (GetDelegate<Action<T1, T2, T3>>(ref eventType, out callback))
        {
            callback.Invoke(arg1, arg2, arg3);
        }
	}

	public void BroadCastEvent<T1, T2, T3, T4>(string eventType, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        Action<T1, T2, T3, T4> callback;
        if (GetDelegate<Action<T1, T2, T3, T4>>(ref eventType, out callback))
        {
            callback.Invoke(arg1, arg2, arg3, arg4);
        }
	}
    #endregion
    

    public void ClearAllEvents()
	{
		this.m_eventTable.Clear();
	}
}

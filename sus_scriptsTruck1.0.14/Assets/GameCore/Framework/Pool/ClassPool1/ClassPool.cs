using Framework;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassPool<T> : BasePool<T> where T : IPoolItem, new()
{
    private Queue<IPoolItem> m_objPool;

    protected override void OnCreate()
    {
        s_instance = this;
        m_objPool = new Queue<IPoolItem>(128);
    }

    protected override void OnDestroy()
    {
        s_instance = null;
        m_objPool = null;
    }

    public static int GetPoolCount()
    {
        if (s_instance == null)
        {
            return 0;
        }
        return s_instance.m_objPool.Count;
    }

    public static T Pop()
    {
        return (T)s_instance.PopItem();
    }

    public static void Push(T o)
    {
        s_instance.PushItem(o);
    }

    public override IPoolItem PopItem()
    {
        IPoolItem t;
        if (s_instance.m_objPool.Count == 0)
        {
            t = new T();
        }
        else
        {
            t = s_instance.m_objPool.Dequeue();
        }
        t.SetUsingSeq(++reqSeq);
        t.SetPoolHolder(s_instance);
        t.OnActive();
        return t;
    }

    public override void PushItem(IPoolItem t)
    {
        if (t.GetUsingSeq() == 0)
        {
            return;
        }
        s_instance.m_objPool.Enqueue(t);
        t.OnDeactive();
        t.SetUsingSeq(0);
        t.SetPoolHolder(null);
    }
}

using Framework;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPool
{
    IPoolItem PopItem();
    void PushItem(IPoolItem item);
}

public abstract class BasePool<T> : IPool where T : IPoolItem, new()
{
    protected static ClassPool<T> s_instance;
    protected HashSet<string> mReferenceSet = new HashSet<string>();
    protected static uint reqSeq = 0;

    public int retainCount
    {
        get
        {
            return mReferenceSet.Count;
        }
    }

    public static void Release(string tag)
    {
        if(s_instance == null)
        {
            return;
        }

        if (!s_instance.mReferenceSet.Contains(tag))
        {
            LOG.Error("未发现引用:" + tag);
        }
        else
        {
            s_instance.mReferenceSet.Remove(tag);
        }

        if (s_instance.mReferenceSet.Count <= 0)
        {
            s_instance.OnDestroy();
        }
    }

    public static void Retain(string tag)
    {
        if(s_instance == null)
        {
            s_instance = new ClassPool<T>();
        }
        s_instance.mReferenceSet.Add(tag);
    }

    protected BasePool() 
    {
        OnCreate();
    }

    protected abstract void OnCreate();
    protected abstract void OnDestroy();

    public abstract IPoolItem PopItem();
    public abstract void PushItem(IPoolItem item);
}

using Framework;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolItem : IPoolItem
{
    private uint m_UsingSeq;
    private IPool m_pool;

    public void SetPoolHolder(IPool holder)
    {
        m_pool = holder;
    }


    public uint GetUsingSeq()
    {
        return m_UsingSeq;
    }

    public void SetUsingSeq(uint seq)
    {
        m_UsingSeq = seq;
    }

    public abstract void OnActive();

    public abstract void OnDeactive();

    public void PushToPool()
    {
        m_pool.PushItem(this);
    }
}

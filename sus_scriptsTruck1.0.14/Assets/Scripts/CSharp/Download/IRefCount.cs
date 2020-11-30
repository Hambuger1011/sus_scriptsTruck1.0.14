/*
 * 引用计数接口
 */
using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRefCount
{
    /// <summary>
    /// 计数+
    /// </summary>
    void Retain();
    /// <summary>
    /// 计数-1
    /// </summary>
    void Release();

    /// <summary>
    /// 获取当前计数
    /// </summary>
    /// <returns></returns>
    int GetRefCount();

    /// <summary>
    /// 释放资源
    /// </summary>
    void OnDestroy();
}

public class RefCountMgr : CSingleton<RefCountMgr>
{
    List<IRefCount> m_waitForDestroy = new List<IRefCount>();
    public void AddToWaitForDestroy(IRefCount refCount)
    {
        m_waitForDestroy.Add(refCount);
    }

    public void Update()
    {
        for(int i = m_waitForDestroy.Count - 1; i >= 0; --i)
        {
            var item = m_waitForDestroy[i];
            m_waitForDestroy.RemoveAt(i);
            if(item.GetRefCount() > 0)
            {
                continue;
            }
            item.OnDestroy();
        }
    }
}

/*
 * unity object引用计数
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;


[XLua.LuaCallCSharp, XLua.Hotfix]
public class UnityObjectRefCount : IRefCount
{
    protected Object m_object = null;
    int m_refCount = 0;

    protected UnityObjectRefCount() { }

    public T Get<T>() where T : Object
    {
        return m_object as T;
    }

    public Object GetObject()
    {
        return m_object;
    }

    public int GetRefCount()
    {
        return m_refCount;
    }

    public void Release()
    {
        --m_refCount;
        if(m_refCount <= 0)
        {
            RefCountMgr.Instance.AddToWaitForDestroy(this);
        }
    }

    public void Retain()
    {
        ++m_refCount;
    }

    public virtual void OnDestroy()
    {
        if(m_object != null)
        {
            Object.Destroy(m_object);
        }
        m_object = null;
    }

    /// <summary>
    /// 默认创建时refCount=0,retain使计数+1
    /// </summary>
    public static UnityObjectRefCount Create(Object obj)
    {
        UnityObjectRefCount refCnt = new UnityObjectRefCount();
        refCnt.m_object = obj;
        refCnt.Retain();
        return refCnt;
    }
}





[XLua.LuaCallCSharp, XLua.Hotfix]
public class SpriteRefCount : UnityObjectRefCount
{
    public override void OnDestroy()
    {
        var sprite = m_object as Sprite;
        XLuaHelper.UnloadSprite(sprite);
        m_object = null;
    }

    /// <summary>
    /// 默认创建时refCount=0,retain使计数+1
    /// </summary>
    public new static SpriteRefCount Create(Object obj)
    {
        SpriteRefCount refCnt = new SpriteRefCount();
        refCnt.m_object = obj;
        refCnt.Retain();
        return refCnt;
    }
}
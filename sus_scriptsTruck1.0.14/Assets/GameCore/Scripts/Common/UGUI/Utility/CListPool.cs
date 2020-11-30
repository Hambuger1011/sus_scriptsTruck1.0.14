using Framework;

using System;
using System.Collections.Generic;
using UnityEngine;

internal static class CListPool<T>
{
    // Object pool to avoid allocations.
    private static readonly CObjectPool<List<T>> s_ListPool = new CObjectPool<List<T>>(null, l => l.Clear());

    public static List<T> Get()
    {
        return s_ListPool.Get();
    }

    public static void Release(List<T> toRelease)
    {
        s_ListPool.Release(toRelease);
    }
}

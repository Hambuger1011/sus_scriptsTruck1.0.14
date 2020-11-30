

using AB;
using Framework;
using GameLogic;
using System.Collections.Generic;
using UnityEngine;

public sealed class CGameObjectPool : Framework.CSingleton<CGameObjectPool>
{
    public bool UseLODObjects = false;

    private DictionaryView<int, Queue<CPooledGameObject>> m_pooledGameObjectMap = new DictionaryView<int, Queue<CPooledGameObject>>();

    private DictionaryView<int, Component> m_componentMap = new DictionaryView<int, Component>();

    private DictionaryView<int, CPooledGameObject> m_objectScriptMap = new DictionaryView<int, CPooledGameObject>();

    private LinkedList<stDelayRecycle> m_delayRecycle = new LinkedList<stDelayRecycle>();

    private GameObject m_poolRoot;

    private bool m_clearPooledObjects;

    private int m_clearPooledObjectsExecuteFrame;

    private static int s_frameCounter;

    protected override void Init()
    {
        m_poolRoot = new GameObject("Pool");
        GameObject gameObject = GameObject.Find("SceneRoot");
        if (gameObject != null)
        {
            m_poolRoot.transform.SetParent(gameObject.transform);
        }
    }

    protected override void UnInit()
    {
    }

    #region 维护对象回收
    public void Update()
    {
        s_frameCounter++;
        UpdateDelayRecycle();
        if (m_clearPooledObjects && m_clearPooledObjectsExecuteFrame == s_frameCounter)
        {
            ExecuteClearPooledObjects();
            m_clearPooledObjects = false;
        }
    }

    private void UpdateDelayRecycle()
    {
        LinkedListNode<stDelayRecycle> parentNode = m_delayRecycle.First;
        int delta = (int)(Time.time * 1000f);
        while (parentNode != null)
        {
            LinkedListNode<stDelayRecycle> nodePtr = parentNode;
            parentNode = parentNode.Next;
            if (nodePtr.Value.recycleObj == null)
            {
                m_delayRecycle.Remove(nodePtr);
                continue;
            }
            if (nodePtr.Value.recycleTime > delta)
            {
                break;
            }
            if (nodePtr.Value.callback != null)
            {
                nodePtr.Value.callback(nodePtr.Value.recycleObj, nodePtr.Value.objSize, nodePtr.Value.objScale);
            }
            RecycleGameObject(nodePtr.Value.recycleObj);
            m_delayRecycle.Remove(nodePtr);
        }
    }


    public void ExecuteClearPooledObjects()
    {
        for (LinkedListNode<stDelayRecycle> linkedListNode = m_delayRecycle.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
        {
            if (linkedListNode.Value.recycleObj != null)
            {
                RecycleGameObject(linkedListNode.Value.recycleObj);
            }
        }
        m_delayRecycle.Clear();
        m_componentMap.Clear();
        m_objectScriptMap.Clear();
        DictionaryView<int, Queue<CPooledGameObject>>.Enumerator enumerator = m_pooledGameObjectMap.GetEnumerator();
        while (enumerator.MoveNext())
        {
            Queue<CPooledGameObject> value = enumerator.Current.Value;
            while (value.Count > 0)
            {
                CPooledGameObject cPooledGameObjectScript = value.Dequeue();
                if (cPooledGameObjectScript != null && cPooledGameObjectScript.gameObject != null)
                {
                    Object.Destroy(cPooledGameObjectScript.gameObject);
                }
            }
        }
        m_pooledGameObjectMap.Clear();
    }
    #endregion


    #region 回收对象
    public void RecycleGameObject(CPooledGameObject pooledGameObjectScript)
    {
        _RecycleGameObject(pooledGameObjectScript, false);
    }

    public void RecyclePreparedGameObject(CPooledGameObject pooledGameObjectScript)
    {
        _RecycleGameObject(pooledGameObjectScript, true);
    }

    private void _RecycleGameObject(CPooledGameObject pooledGameObjectScript, bool setIsInit)
    {
        if (pooledGameObjectScript != null)
        {
            Queue<CPooledGameObject> queue = null;
            if (m_pooledGameObjectMap.TryGetValue(pooledGameObjectScript.m_prefabKey.JavaHashCodeIgnoreCase(), out queue))
            {
                queue.Enqueue(pooledGameObjectScript);
                pooledGameObjectScript.OnRecycle();
                pooledGameObjectScript.transform.SetParent(m_poolRoot.transform, true);
                pooledGameObjectScript.m_isInit = setIsInit;
            }
        }
    }

    /// <summary>
    /// 延迟回收对象
    /// </summary>
    public void RecycleGameObjectDelay(CPooledGameObject pooledGameObjectScript, int delayMillSeconds, OnDelayRecycleDelegate callback = null, float[] objSize = null, Vector3[] objScale = null)
    {
        stDelayRecycle stDelayRecycle = new stDelayRecycle();
        stDelayRecycle.recycleObj = pooledGameObjectScript;
        stDelayRecycle.recycleTime = (int)(Time.time * 1000f) + delayMillSeconds;
        stDelayRecycle.objSize = objSize;
        stDelayRecycle.objScale = objScale;
        stDelayRecycle.callback = callback;
        if (m_delayRecycle.Count == 0)
        {
            m_delayRecycle.AddLast(stDelayRecycle);
        }
        else
        {
            for (LinkedListNode<stDelayRecycle> linkedListNode = m_delayRecycle.Last; linkedListNode != null; linkedListNode = linkedListNode.Previous)
            {
                if (linkedListNode.Value.recycleTime < stDelayRecycle.recycleTime)
                {
                    m_delayRecycle.AddAfter(linkedListNode, stDelayRecycle);
                    return;
                }
            }
            m_delayRecycle.AddFirst(stDelayRecycle);
        }
    }

    #endregion

    /// <summary>
    /// 清除缓存的对象(下一帧)
    /// </summary>
    public void ClearPooledObjects()
    {
        m_clearPooledObjects = true;
        m_clearPooledObjectsExecuteFrame = s_frameCounter + 1;
    }

    /// <summary>
    /// 清除延迟队列中的
    /// </summary>
    public void ClearDelayRecycleObjectsSync()
    {
        for (LinkedListNode<stDelayRecycle> linkedListNode = m_delayRecycle.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
        {
            if (linkedListNode.Value.recycleObj != null)
            {
                RecycleGameObject(linkedListNode.Value.recycleObj);
            }
        }
        m_delayRecycle.Clear();
    }


    public void UpdateParticleChecker(int maxNum)
    {
    }


    #region 获取资源

    public CPooledGameObject GetGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, enResourceType resourceType, out bool isInit, string reservePrefabName = null)
    {
        return GetGameObject(prefabFullPath, pos, rot, true, resourceType, out isInit, reservePrefabName);
    }
    public CPooledGameObject GetGameObject(string prefabFullPath, Vector3 pos, enResourceType resourceType, out bool isInit, string reservePrefabName = null)
    {
        return GetGameObject(prefabFullPath, pos, Quaternion.identity, false, resourceType, out isInit, reservePrefabName);
    }

    public CPooledGameObject GetGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, enResourceType resourceType, string reservePrefabName = null)
    {
        bool isInit = false;
        return GetGameObject(prefabFullPath, pos, rot, true, resourceType, out isInit, reservePrefabName);
    }

    public CPooledGameObject GetGameObject(string prefabFullPath, Vector3 pos, enResourceType resourceType)
    {
        bool isInit = false;
        return GetGameObject(prefabFullPath, pos, Quaternion.identity, false, resourceType, out isInit, null);
    }


    public CPooledGameObject GetGameObject(string prefabFullPath, enResourceType resourceType, string reservePrefabName = null)
    {
        bool flag = false;
        return GetGameObject(prefabFullPath, Vector3.zero, Quaternion.identity, false, resourceType, out flag, reservePrefabName);
    }

    public CPooledGameObject GetGameObject(string prefabFullPath, enResourceType resourceType, out bool isInit, string reservePrefabFullPath = null)
    {
        return GetGameObject(prefabFullPath, Vector3.zero, Quaternion.identity, false, resourceType, out isInit, reservePrefabFullPath);
    }


    private CPooledGameObject GetGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, bool useRotation, enResourceType resourceType, out bool isInit, string reservePrefabFullPath)
    {
        if (string.IsNullOrEmpty(prefabFullPath))
        {
            isInit = false;
            return null;
        }
        string prefabKey = prefabFullPath;//CFileManager.EraseExtension(prefabFullPath);
        Queue<CPooledGameObject> queue = null;
        if (!m_pooledGameObjectMap.TryGetValue(prefabKey.JavaHashCodeIgnoreCase(), out queue))
        {
            queue = new Queue<CPooledGameObject>();
            m_pooledGameObjectMap.Add(prefabKey.JavaHashCodeIgnoreCase(), queue);
        }

#region 获取缓存对象
        CPooledGameObject cPooledGameObjectScript = null;
        while (queue.Count > 0)
        {
            cPooledGameObjectScript = queue.Dequeue();
            if (cPooledGameObjectScript != null && cPooledGameObjectScript.gameObject != null)
            {
                cPooledGameObjectScript.transform.SetParent(null, true);
                cPooledGameObjectScript.transform.position = pos;
                cPooledGameObjectScript.transform.rotation = rot;
                cPooledGameObjectScript.transform.localScale = cPooledGameObjectScript.m_defaultScale;
                break;
            }
            cPooledGameObjectScript = null;
        }
#endregion

        #region 使用Lod
        if (UseLODObjects && cPooledGameObjectScript == null && !string.IsNullOrEmpty(reservePrefabFullPath))
        {
            string s = reservePrefabFullPath;//CFileManager.EraseExtension(reservePrefabFullPath);
            queue = null;
            if (m_pooledGameObjectMap.TryGetValue(s.JavaHashCodeIgnoreCase(), out queue))
            {
                while (queue.Count > 0)
                {
                    cPooledGameObjectScript = queue.Dequeue();
                    if (cPooledGameObjectScript != null && cPooledGameObjectScript.gameObject != null)
                    {
                        cPooledGameObjectScript.transform.SetParent(null, true);
                        cPooledGameObjectScript.transform.position = pos;
                        cPooledGameObjectScript.transform.rotation = rot;
                        cPooledGameObjectScript.transform.localScale = cPooledGameObjectScript.m_defaultScale;
                        break;
                    }
                    cPooledGameObjectScript = null;
                }
            }
        }
        #endregion

        if (cPooledGameObjectScript == null)
        {
            cPooledGameObjectScript = CreateGameObject(prefabFullPath, pos, rot, useRotation, resourceType, prefabKey);
        }
        if (cPooledGameObjectScript == null)
        {
            isInit = false;
            return null;
        }
        isInit = cPooledGameObjectScript.m_isInit;
        cPooledGameObjectScript.OnGet();
        return cPooledGameObjectScript;
    }

    /// <summary>
    /// 加载资源
    /// </summary>
    private CPooledGameObject CreateGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, bool useRotation, enResourceType resourceType, string prefabKey)
    {
        bool needCached = resourceType == enResourceType.BattleScene;
        var bundle = ABMgr.Instance.LoadImme(null, enResType.ePrefab, prefabFullPath);
        if (bundle == null)
        {
            return null;
        }
        GameObject prefab = bundle.resPrefab;
        GameObject go = null;
        if (useRotation)
        {
            go = GameObject.Instantiate(prefab, pos, rot);
        }
        else
        {
            go = GameObject.Instantiate(prefab);
            go.transform.position = pos;
        }
        LOG.Assert(go != null);
        CPooledGameObject pGO = new CPooledGameObject();
        pGO.Initialize(prefabKey, go, bundle);
        pGO.OnCreate();
        m_objectScriptMap[go.GetInstanceID()] = pGO;
        return pGO;
    }

    #endregion


    /// <summary>
    /// 从CPooledGameObject对象获取一个类型为T的Component
    /// </summary>
    public T GetCachedComponent<T>(CPooledGameObject script, bool autoAdd = false) where T : Component
    {
        if (script == null)
        {
            return (T)null;
        }
        Component component = null;
        if (m_componentMap.TryGetValue(script.gameObject.GetInstanceID(), out component) && (!autoAdd || component != null))
        {
            return component as T;
        }
        component = script.gameObject.GetComponent<T>();
        if (autoAdd && component == null)
        {
            component = script.gameObject.AddComponent<T>();
        }
        m_componentMap[script.gameObject.GetInstanceID()] = component;
        if (null == component)
        {
            return null;
        }
        return component as T;
    }

    /// <summary>
    /// gameObject对象的CPooledGameObject
    /// </summary>
    public CPooledGameObject GetPooledGameScript(int instanceId, bool check = true)
    {
        CPooledGameObject result = null;
        m_objectScriptMap.TryGetValue(instanceId, out result);
        return result;
    }

    /// <summary>
    /// 预加载对象
    /// </summary>
    public void PrepareGameObject(string prefabFullPath, enResourceType resourceType, int amount, bool assertNull = true)
    {
        string text = prefabFullPath;//CFileManager.EraseExtension(prefabFullPath);
        Queue<CPooledGameObject> queue = null;
        if (!m_pooledGameObjectMap.TryGetValue(text.JavaHashCodeIgnoreCase(), out queue))
        {
            queue = new Queue<CPooledGameObject>();
            m_pooledGameObjectMap.Add(text.JavaHashCodeIgnoreCase(), queue);
        }
        if (queue.Count < amount)
        {
            amount -= queue.Count;
            for (int i = 0; i < amount; i++)
            {
                CPooledGameObject cPooledGameObjectScript = CreateGameObject(prefabFullPath, Vector3.zero, Quaternion.identity, false, resourceType, text);
                if (assertNull)
                {
                    LOG.Assert(cPooledGameObjectScript != null, "Failed Create Game object from \"{0}\"", prefabFullPath);
                }
                if (cPooledGameObjectScript != null)
                {
                    queue.Enqueue(cPooledGameObjectScript);
                    cPooledGameObjectScript.transform.SetParent(m_poolRoot.transform, true);
                    cPooledGameObjectScript.OnPrepare();
                }
            }
        }
    }


    #region 声明定义
    /// <summary>
    /// 延迟回收参数
    /// </summary>
    private class stDelayRecycle
    {
        public CPooledGameObject recycleObj;

        public int recycleTime;

        public float[] objSize;

        public Vector3[] objScale;

        public OnDelayRecycleDelegate callback;
    }

    public delegate void OnDelayRecycleDelegate(CPooledGameObject recycleObj, float[] objSize, Vector3[] scale);
    #endregion

}

public enum enResourceType
{
    BattleScene,
    Numeric,
    Sound,
    UIForm,
    UIPrefab,
    UI3DImage,
    UISprite
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


/// <summary>
/// 显示工具类
/// </summary>是
public sealed class DisplayUtil
{
    static public GameObject AddRootChild(string name)
    {
        GameObject go = new GameObject(name);
        Transform t = go.transform;
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
        //go.layer = parent.layer;
        return go;
    }
    static public GameObject AddChild(GameObject parent, string childname)
    {
        GameObject go = new GameObject(childname);

        if (parent != null)
        {
            Transform t = go.transform;
            t.parent = parent.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            go.layer = parent.layer;
        }
        return go;
    }
    static public GameObject AddChild(GameObject parent)
    {
        GameObject go = new GameObject();

        if (parent != null)
        {
            Transform t = go.transform;
            t.parent = parent.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            go.layer = parent.layer;
        }
        return go;
    }
    static public Transform AddChild(GameObject parent, Transform t)
    {
        if (parent != null)
        {
            t.parent = parent.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            t.gameObject.layer = parent.layer;
        }
        return t;
    }
    static public GameObject AddChild(GameObject parent, GameObject go)
    {
        GameObject child = GameObject.Instantiate(go) as GameObject;
        if (parent != null)
        {
            Transform t = child.transform;
            t.parent = parent.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            SetLayer(child, parent.layer);
        }
        return child;
    }

    static public GameObject AddChildNoIns(GameObject parent, GameObject child)
    {
        if (parent != null)
        {
            Transform t = child.transform;
            t.parent = parent.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            SetLayer(child, parent.layer);
        }
        return child;
    }
    static public void SetLayer(GameObject go, int layer)
    {
        go.layer = layer;

        Transform t = go.transform;

        for (int i = 0, imax = t.childCount; i < imax; ++i)
        {
            Transform child = t.GetChild(i);
            SetLayer(child.gameObject, layer);
        }
    }
    static public GameObject AddChild(GameObject parent, GameObject child, int layer)
    {
        if (parent != null)
        {
            Transform t = child.transform;
            t.parent = parent.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            child.layer = layer;
        }
        return child;
    }

    /// <summary>
    /// 添加子对象
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    static public GameObject FindChild(GameObject parent, string childName)
    {
        if (parent != null)
        {
            return parent.transform.Find(childName).gameObject;
        }
        return null;
    }


    /// <summary>
    /// Activate the specified object and all of its children.
    /// </summary>

    static void Activate(Transform t)
    {
        SetActiveSelf(t.gameObject, true);

        // Prior to Unity 4, active state was not nested. It was possible to have an enabled child of a disabled object.
        // Unity 4 onwards made it so that the state is nested, and a disabled parent results in a disabled child.
#if UNITY_3_5
		        for (int i = 0, imax = t.GetChildCount(); i < imax; ++i)
		        {
			        Transform child = t.GetChild(i);
			        Activate(child);
		        }
#else
        // If there is even a single enabled child, then we're using a Unity 4.0-based nested active state scheme.
        for (int i = 0, imax = t.childCount; i < imax; ++i)
        {
            Transform child = t.GetChild(i);
            if (child.gameObject.activeSelf) return;
        }

        // If this point is reached, then all the children are disabled, so we must be using a Unity 3.5-based active state scheme.
        for (int i = 0, imax = t.childCount; i < imax; ++i)
        {
            Transform child = t.GetChild(i);
            Activate(child);
        }
#endif
    }

    /// <summary>
    /// Deactivate the specified object and all of its children.
    /// </summary>

    static void Deactivate(Transform t)
    {
#if UNITY_3_5
		        for (int i = 0, imax = t.GetChildCount(); i < imax; ++i)
		        {
			        Transform child = t.GetChild(i);
			        Deactivate(child);
		        }
#endif
        SetActiveSelf(t.gameObject, false);
    }

    /// <summary>
    /// SetActiveRecursively enables children before parents. This is a problem when a widget gets re-enabled
    /// and it tries to find a panel on its parent.
    /// </summary>

    static public void SetActive(GameObject go, bool state)
    {
        if (state)
        {
            Activate(go.transform);
        }
        else
        {
            Deactivate(go.transform);
        }
    }

    /// <summary>
    /// 得到预设对象方法
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static GameObject GetPrefab(string name)
    {
        if (name != null)
        {
            return (GameObject)Resources.Load(name, typeof(GameObject));
        }
        return null;
    }

    /// <summary>
    /// Activate or deactivate children of the specified game object without changing the active state of the object itself.
    /// </summary>

    static public void SetActiveChildren(GameObject go, bool state)
    {
        Transform t = go.transform;

        if (state)
        {
            for (int i = 0, imax = t.childCount; i < imax; ++i)
            {
                Transform child = t.GetChild(i);
                Activate(child);
            }
        }
        else
        {
            for (int i = 0, imax = t.childCount; i < imax; ++i)
            {
                Transform child = t.GetChild(i);
                Deactivate(child);
            }
        }
    }

    /// <summary>
    /// Unity4 has changed GameObject.active to GameObject.activeself.
    /// </summary>

    static public bool GetActive(GameObject go)
    {
#if UNITY_3_5
		        return go && go.active;
#else
        return go && go.activeInHierarchy;
#endif
    }

    /// <summary>
    /// Unity4 has changed GameObject.active to GameObject.SetActive.
    /// </summary>

    static public void SetActiveSelf(GameObject go, bool state)
    {
#if UNITY_3_5
		        go.active = state;
#else
        go.SetActive(state);
#endif
    }


    public static GameObject GetChild(GameObject go, string name, bool includeSelf = true)
    {
        if ((go != null) && !string.IsNullOrEmpty(name))
        {
            if (includeSelf && (go.name == name))
            {
                return go;
            }
            Transform transform = go.transform;
            Transform transform2 = null;
            if (name.IndexOf('/') != -1)
            {
                transform2 = transform.Find(name);
                if (transform2 != null)
                {
                    return transform2.gameObject;
                }
                return null;
            }
            transform2 = transform.Find(name);
            if (transform2 != null)
            {
                return transform2.gameObject;
            }
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject obj2 = GetChild(transform.GetChild(i).gameObject, name, true);
                if (obj2 != null)
                {
                    return obj2;
                }
            }
        }
        return null;
    }
    public static T GetChildComponent<T>(GameObject go, string name) where T : Component
    {
        if ((go != null) && !string.IsNullOrEmpty(name))
        {
            GameObject obj2 = GetChild(go, name, true);
            if (obj2 != null)
            {
                return obj2.GetComponent<T>();
            }
        }
        return null;
    }
    public static GameObject GetDirectChild(GameObject go, string name)
    {
        if (go != null)
        {
            Transform transform = go.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject gameObject = transform.GetChild(i).gameObject;
                if (gameObject.name == name)
                {
                    return gameObject;
                }
            }
        }
        return null;
    }
}


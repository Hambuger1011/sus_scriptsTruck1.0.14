using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T : class, new()
{
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (syslock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                        (_instance as Singleton<T>).Init();
                    }
                }
            }
            return _instance;
        }
    }
    private static T _instance;
    private static readonly object syslock = new object();
    public static T GetInstance() { return Instance; }
    public static void DestroyInstance()
    {
        if (_instance != null)
        {
            (_instance as Singleton<T>).UnInit();
            _instance = (T)((object)null);
        }
    }
    protected Singleton() { }
    protected virtual void Init() { if (_instance != null) return; }
    protected virtual void UnInit() { if (_instance == null) return; }

}

[DisallowMultipleComponent]
public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (syslock)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        _instacneGameObject = GameObject.Find("SingletonManager") ?? new GameObject("SingletonManager");
                        _instance = _instacneGameObject.AddComponent<T>();
                    }
                    else
                    {
                        _instacneGameObject = _instance.gameObject;
                    }
                }
            }
            return _instance;
        }
    }

    private static T _instance;
    private static readonly object syslock = new object();
    private static GameObject _instacneGameObject;
    public static T GetInstance() { return Instance; }
    public bool HasInstance { get { return _instance != null; } }
    protected virtual void Awake()
    {
        Init();
        DontDestroyOnLoad(this);
    }
    public void DestroyInstance()
    {
        if (_instance != null) { UnInit(); Destroy(this); _instance = null; }
    }
    protected virtual void Init() { if (_instance != null) return; }
    protected virtual void UnInit() { if (_instance == null) return; }
}
using Framework;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AndroidHelper 
{
    /*
    http://www.lxway.com/946854656.htm
    类型
    符号
    boolean	Z
    byte	B
    char	C
    short	S
    int	I
    long	L
    float	F
    double	D
    void	V
    object对象	LClassName;      L类名;
    Arrays	
    [array-type        [数组类型
    methods方法	(argument-types)return-type     (参数类型)返回类型
    */
    [Conditional("UNITY_ANDROID")]
    public static void CallStaticMethod(string className, string methodName, params object[] args)
    {
        if(GameUtility.isEditorMode)
        {
            return;
        }
        using (AndroidJavaClass cls = new AndroidJavaClass(className))
        {
            if (cls != null)
            {
                cls.CallStatic(methodName, args);
            }
            else
            {
                LOG.Error("获取Android类失败:" + className);
            }
        }
    }

    public static T CallStaticMethod<T>(string className, string methodName, params object[] args)
    {
        if (GameUtility.isEditorMode)
        {
            return default(T);
        }
        using (AndroidJavaClass cls = new AndroidJavaClass(className))
        {
            if (cls != null)
            {
                return cls.CallStatic<T>(methodName, args);
            }
            else
            {
                LOG.Error("获取Android类失败:" + className);
            }
        }
        return default(T);
    }

    public static byte[] ReadAssetFileBytes(string filePath)
    {
        return
            null; //AndroidHelper.CallStaticMethod<byte[]>("com.game.gamelib.io.FileUtils", "ReadAssetFileBytes", filePath);
    }
}





#region 回调代理类(不支持数组)
public static class JAction
{
    public class Zero : AndroidJavaProxy
    {
        Action m_callback;
        public Zero(Action callback) : base("com.game.gamelib.event.IAction$Zero")
        {
            m_callback = callback;
        }

        public void Invoke()
        {
            m_callback();
        }
    }

    public class One<T> : AndroidJavaProxy
    {
        Action<T> m_callback;
        public One(Action<T> callback) : base("com.game.gamelib.event.IAction$One")
        {
            m_callback = callback;
        }
        public void Invoke(T t)
        {
            m_callback(t);
        }
    }

    public class Two<T1, T2> : AndroidJavaProxy
    {
        Action<T1, T2> m_callback;
        public Two(Action<T1, T2> callback) : base("com.game.gamelib.event.IAction$Two")
        {
            m_callback = callback;
        }
        public void Invoke(T1 t1, T2 t2)
        {
            m_callback(t1, t2);
        }
    }

    public class Three<T1, T2, T3> : AndroidJavaProxy
    {
        Action<T1, T2, T3> m_callback;
        public Three(Action<T1, T2, T3> callback) : base("com.game.gamelib.event.IAction$Three")
        {
            m_callback = callback;
        }
        public void Invoke(T1 t1, T2 t2, T3 t3)
        {
            m_callback(t1, t2, t3);
        }
    }
}


public static class JFunc
{
    public class Zero<TReturn> : AndroidJavaProxy
    {
        Func<TReturn> m_callback;
        public Zero(Func<TReturn> callback) : base("com.game.gamelib.event.IFunc$Zero")
        {
            m_callback = callback;
        }

        public TReturn Invoke()
        {
            return m_callback();
        }
    }

    public class One<TReturn, T> : AndroidJavaProxy
    {
        Func<T, TReturn> m_callback;
        public One(Func<T, TReturn> callback) : base("com.game.gamelib.event.IFunc$One")
        {
            m_callback = callback;
        }
        public TReturn Invoke(T t)
        {
            return m_callback(t);
        }
    }

    public class Two<TReturn,T1, T2> : AndroidJavaProxy
    {
        Func<T1, T2, TReturn> m_callback;
        public Two(Func<T1, T2, TReturn> callback) : base("com.game.gamelib.event.IFunc$Two")
        {
            m_callback = callback;
        }
        public TReturn Invoke(T1 t1, T2 t2)
        {
            return m_callback(t1, t2);
        }
    }

    public class Three<TReturn, T1, T2, T3> : AndroidJavaProxy
    {
        Func<T1, T2, T3, TReturn> m_callback;
        public Three(Func<T1, T2, T3, TReturn> callback) : base("com.game.gamelib.event.IFunc$Three")
        {
            m_callback = callback;
        }
        public TReturn Invoke(T1 t1, T2 t2, T3 t3)
        {
            return m_callback(t1, t2, t3);
        }
    }
}
#endregion




public sealed class PackageManager
{
    /**
     * Permission check result: this is returned by {@link #checkPermission}
     * if the permission has been granted to the given package.
     */
    public const int PERMISSION_GRANTED = 0;

    /**
     * Permission check result: this is returned by {@link #checkPermission}
     * if the permission has not been granted to the given package.
     */
    public const int PERMISSION_DENIED = -1;
}

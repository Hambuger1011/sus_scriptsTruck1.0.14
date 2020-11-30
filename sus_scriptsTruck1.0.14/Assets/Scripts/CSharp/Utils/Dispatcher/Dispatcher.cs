using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//namespace com.game.core.mvc
//{
/// <summary>
/// 基于委托的事件派发类
/// </summary>
public sealed class Dispatcher
{
    ///// <summary>
    ///// 触发监听，相当于as3的派发事件功能
    ///// </summary>
    ///// <param name="key"></param>
    ///// <param name="value"></param>
    ///// <returns></returns>
    //public static bool dispatchEvent(string key, string value)
    //{
    //    return command.call(key, value);
    //}

    public static void addEventListener(string key, Action action)
    {
        command.addCall(key, action);
    }
    public static void removeEventListener(string key, Action action)
    {
        command.removeCall(key, action);
    }

    public static bool dispatchEvent(string key)
    {
        return command.call(key);
    }

    /// <summary>
    /// 回调函数核心类
    /// </summary>
    private static Caller command = new Caller();
    /// <summary>
    /// 添加一个监听
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="action"></param>
    public static void addEventListener<T>(string key, Action<T> action)
    {
        command.addCall(key, action);
    }


    /// <summary>
    /// 移除一个监听
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="action"></param>
    public static void removeEventListener<T>(string key, Action<T> action)
    {
        command.removeCall(key, action);
    }



    public static bool dispatchEvent<T>(string key, T t)
    {
        return command.call<T>(key, t);
    }



    /// <summary>
    /// 添加一个监听
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="action"></param>
    public static void addEventListener<T, T1>(string key, Action<T, T1> action)
    {
        command.addCall(key, action);
    }
    /// <summary>
    /// 移除一个监听
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="action"></param>
    public static void removeEventListener<T, T1>(string key, Action<T, T1> action)
    {
        command.removeCall(key, action);
    }



    public static bool dispatchEvent<T, T1>(string key, T t, T1 t1)
    {
        return command.call<T, T1>(key, t, t1);
    }

    internal static void dispatchEvent<T1, T2, T3>(string chiCardRequest, object onChiCardRequest)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// 添加一个监听
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="action"></param>
    public static void addEventListener<T, T1, T2>(string key, Action<T, T1, T2> action)
    {
        command.addCall(key, action);
    }

    internal static void dispatchEvent<T>(string hupaiRequest, object onHupaiRequest)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 移除一个监听
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="action"></param>
    public static void removeEventListener<T, T1, T2>(string key, Action<T, T1, T2> action)
    {
        command.removeCall(key, action);
    }



    public static bool dispatchEvent<T, T1, T2>(string key, T t, T1 t1, T2 t2)
    {
        return command.call<T, T1, T2>(key, t, t1, t2);
    }


    /// <summary>
    /// 添加一个监听
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="action"></param>
    public static void addEventListener<T, T1, T2, T3>(string key, Action<T, T1, T2, T3> action)
    {
        command.addCall(key, action);
    }
    /// <summary>
    /// 移除一个监听
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="action"></param>
    public static void removeEventListener<T, T1, T2, T3>(string key, Action<T, T1, T2, T3> action)
    {
        command.removeCall(key, action);
    }



    public static bool dispatchEvent<T, T1, T2, T3>(string key, T t, T1 t1, T2 t2, T3 t3)
    {
        return command.call<T, T1, T2, T3>(key, t, t1, t2, t3);
    }
}
//}

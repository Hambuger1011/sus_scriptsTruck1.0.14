using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class StartCoroutineTool : SingletonMono<StartCoroutineTool> 
{
    //用以保存协程的引用
    private Dictionary<string, Coroutine> DispatcherDic = new Dictionary<string, Coroutine>();

    protected override void Init()
    {
        base.Init();
    }
    protected override void UnInit()
    {
        base.UnInit();
    }

    #region WaitToDo
    /// <summary>
    /// 延迟一定的时间间隔后执行
    /// </summary>
    /// <param name="delegateEvent"></param>
    /// <param name="time"></param>
    /// <param name="methodName"></param>
    public void WaitToDo(Action delegateEvent, float time, string methodName = null)
    {
        if (!string.IsNullOrEmpty(methodName))
        {
            if (!DispatcherDic.ContainsKey(methodName))
            {
                DispatcherDic.Add(methodName, StartCoroutine(WaitToDoCorutine(delegateEvent, time, methodName)));
            }
            else LOG.Warn("this same methoName already exist");
        }
        else StartCoroutine(WaitToDoCorutine(delegateEvent, time));
    }
    IEnumerator WaitToDoCorutine(Action delegateEvent, float time, string methodName = null)
    {
        yield return new WaitForSeconds(time);
        delegateEvent();
        OnDelegateEventFinish(methodName);
    }

    public void WaitToDo<T1>(Action<T1> delegateEvent, float time, T1 parm1, string methodName = null)
    {
        if (!string.IsNullOrEmpty(methodName))
        {
            if (!DispatcherDic.ContainsKey(methodName))
            {
                DispatcherDic.Add(methodName, StartCoroutine(WaitToDoCorutine(delegateEvent, time, parm1, methodName)));
            }
            else LOG.Warn("this same methoName already exist");
        }
        else StartCoroutine(WaitToDoCorutine(delegateEvent, time, parm1));
    }

    IEnumerator WaitToDoCorutine<T1>(Action<T1> delegateEvent, float time, T1 parm1, string methodName = null)
    {
        yield return new WaitForSeconds(time);
        delegateEvent(parm1);
        OnDelegateEventFinish(methodName);
    }
    #endregion

    #region RepeatDo
    /// <summary>
    /// 按一定的时间间隔重复执行数次
    /// </summary>
    /// <param name="delegateEvent"></param>
    /// <param name="times">当次数小于0时永不停止，直至主动Stop</param>
    /// <param name="interval"></param>
    public void RepeatDo(Action delegateEvent, int times, float interval, string methodName = null)
    {
        if (!string.IsNullOrEmpty(methodName))
        {
            if (!DispatcherDic.ContainsKey(methodName))
            {
                DispatcherDic.Add(methodName, StartCoroutine(RepeatDoCorutine(delegateEvent, times, interval, methodName)));
            }
            else LOG.Warn("this same methoName already exist");
        }
        else StartCoroutine(RepeatDoCorutine(delegateEvent, times, interval));
    }
    IEnumerator RepeatDoCorutine(Action delegateEvent, int times, float interval, string methodName = null)
    {
        WaitForSeconds wait = new WaitForSeconds(interval);
        bool isEndless = false;
        if (times < 0)
        {
            isEndless = true;
            times = int.MaxValue;
        }
        while (times > 0)
        {
            delegateEvent();
            if (!isEndless) times--;
            yield return wait;
        }
        OnDelegateEventFinish(methodName);
    }

    public void RepeatDo<T1>(Action<T1> delegateEvent, int times, float interval, T1 parm1, string methodName = null)
    {
        if (!string.IsNullOrEmpty(methodName))
        {
            if (!DispatcherDic.ContainsKey(methodName))
            {
                DispatcherDic.Add(methodName, StartCoroutine(RepeatDoCorutine(delegateEvent, times, interval, parm1, methodName)));
            }
            else LOG.Warn("this same methoName already exist");
        }
        else StartCoroutine(RepeatDoCorutine(delegateEvent, times, interval, parm1));
    }
    IEnumerator RepeatDoCorutine<T1>(Action<T1> delegateEvent, int times, float interval, T1 parm1, string methodName = null)
    {
        WaitForSeconds wait = new WaitForSeconds(interval);
        bool isEndless = false;
        if (times < 0)
        {
            isEndless = true;
            times = int.MaxValue;
        }
        while (times > 0)
        {
            delegateEvent(parm1);
            if (!isEndless) times--;
            yield return wait;
        }
        OnDelegateEventFinish(methodName);
    }
    #endregion

    #region RunCoroutine
    /// <summary>
    /// 使用此方法外部运行协程，完成后注意清除字典引用
    /// </summary>
    public void RunCoroutine(IEnumerator coroutine, string methodName = null)
    {
        if (!string.IsNullOrEmpty(methodName))
        {
            if (!DispatcherDic.ContainsKey(methodName))
            {
                DispatcherDic.Add(methodName, StartCoroutine(coroutine));
            }
            else LOG.Warn("this same methoName already exist");
        }
        else StartCoroutine(coroutine);
    }
    IEnumerator DoCoroutine(IEnumerator coroutine, string methodName = null)
    {
        yield return StartCoroutine(coroutine);
        OnDelegateEventFinish(methodName);
    }
    #endregion

    #region ToolMethods

    /// <summary>
    /// 协程结束时执行
    /// </summary>
    /// <param name="methodName"></param>
    private void OnDelegateEventFinish(string methodName)
    {
        if (!string.IsNullOrEmpty(methodName) && DispatcherDic.ContainsKey(methodName)) DispatcherDic.Remove(methodName);
    }

    /// <summary>
    /// 停止运行中的协程
    /// </summary>
    /// <param name="methodName"></param>
    public void StopRunningCoroutine(string methodName)
    {
        if (DispatcherDic.ContainsKey(methodName))
        {
            StopCoroutine(DispatcherDic[methodName]);
            DispatcherDic.Remove(methodName);
            LOG.Warn("Stop Coroutine : " + methodName + " Success!!!");
        }
        else
        {
            LOG.Warn("the coroutines name of " + methodName + " is not running.");
        }
    }

    /// <summary>
    /// 停止全部协程
    /// </summary>
    public void ClearAllCoroutine()
    {
        StopAllCoroutines();
        DispatcherDic.Clear();
    }
    #endregion
}

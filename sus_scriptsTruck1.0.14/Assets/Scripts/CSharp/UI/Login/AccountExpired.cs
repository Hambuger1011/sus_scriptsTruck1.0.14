
/*
   这里是当账号过期返回281时候的处理

 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountExpired
{
    private static AccountExpired instance;
    private List<Action> ActionList;

    public static AccountExpired Instance
    {
        get
        {
            if (instance==null)
            {
                instance = new AccountExpired();
            }

            return instance;
        }
    }

    /// <summary>
    /// 添加这条协议过期时候，调用这条协议的方法
    /// </summary>
    /// <param name="action"></param>

    public void AddAction(Action action)
    {
        if (ActionList==null)
        {
            ActionList = new List<Action>();
        }

        ActionList.Clear();

        ActionList.Add(action);
    }

    /// <summary>
    /// 执行协议过期后，调用这条过期协议的方法
    /// </summary>
    public void DoAction()
    {
        if (ActionList != null)
        {
            Action action = ActionList[0];
            action();
            ActionList.Clear();
        }
      
    }

    /// <summary>
    /// 清空方法
    /// </summary>
    public void ClearAction()
    {
        if (ActionList != null)
        {        
            ActionList.Clear();
        }
    }

}

using Framework;

using System;
using System.Collections;
using System.Collections.Generic;

public class CCoroutine
{
    private Stack<IEnumerator> mExcetionStack = new Stack<IEnumerator>();
    private int nMillSeconds = 0;
    public CCoroutine(IEnumerator itr)
    {
        this.mExcetionStack.Push(itr);
    }
    #region implement interface method
    public object Current
    {
        get
        {
            if (mExcetionStack.Count == 0) return null;
            return mExcetionStack.Peek().Current;
        }
    }
    public void Reset()
    {
        throw new System.NotSupportedException("This Operation Is Not Supported.");
    }
    /// <summary>
    /// 相当于执行计数器
    /// </summary>
    public bool MoveNext(int nDeltaTimeMS)
    {
        if (nMillSeconds > 0)
        {
            nMillSeconds -= nDeltaTimeMS;
            return true;
        }
        IEnumerator itr = this.mExcetionStack.Peek();
        if (itr.MoveNext())
        {
            object result = itr.Current;
            if (result != null)
            {
                //Debug.Log("计数器yield return 结果:" + result);
                if (result is IEnumerator)//计数器,放入执行栈
                {
                    this.mExcetionStack.Push(result as IEnumerator);
                }
                else if (result is CWaitForMillSeconds)//等待
                {
                    var wait = result as CWaitForMillSeconds;
                    nMillSeconds = wait.m_interval - nDeltaTimeMS;
                    //Debug.Log(mWaitSeconds);
                }else if(result is CWaitForNextFrame)
                {

                }
            }
            return true;
        }
        else if (mExcetionStack.Count > 1)//还有其它计数器
        {
            mExcetionStack.Pop();//移除空集
            return true;
        }
        return false;//已经是空集
    }
    #endregion
}

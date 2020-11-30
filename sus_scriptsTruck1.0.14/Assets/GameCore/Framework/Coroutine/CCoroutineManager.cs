using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CCoroutineManager : Framework.CSingleton<CCoroutineManager>
{
    LinkedList<CCoroutine> m_routineList = new LinkedList<CCoroutine>();
    

	public CCoroutine StartCoroutine(IEnumerator coroutine)
	{
		CCoroutine cCoroutine = new CCoroutine(coroutine);
        LinkedListNode<CCoroutine> node = new LinkedListNode<CCoroutine>(cCoroutine);
        m_routineList.AddLast(node);
        return cCoroutine;
	}

	public void StopCoroutine(CCoroutine c)
	{
        if(c == null)
        {
            return;
        }
        for (var itr = m_routineList.First;itr != null; itr = itr.Next)
        {
            if(itr.Value == c)
            {
                m_routineList.Remove(itr);
                break;
            }
        }
	}

    public void Update()
    {
        UpdateLogic(Mathf.RoundToInt(Time.deltaTime * 1000));
    }

    public void UpdateLogic(int nDeltaTimeMS)
    {
        LinkedListNode<CCoroutine> node = m_routineList.First;
        LinkedListNode<CCoroutine> tempNode = node;
        while (node != null)
        {
            if (!node.Value.MoveNext(nDeltaTimeMS))
            {
                tempNode = node;
                node = node.Next;
                //此处可以写删除节点的后续处理
                m_routineList.Remove(tempNode);
                tempNode.Value = null;
            }
            else
            {
                node = node.Next;
            }
        }
    }
}

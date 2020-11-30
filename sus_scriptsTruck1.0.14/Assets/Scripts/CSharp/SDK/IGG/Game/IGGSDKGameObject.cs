using System.Collections;
using System.Collections.Generic;
using IGG.SDK;
using IGG.SDK.Core.Unity;
using UnityEngine;

/// <summary>
/// IGGMonoBehaviour是USDK依赖的必要GameObject
/// </summary>
public class IGGSDKGameObject : IGGMonoBehaviour
{
    // Start is called before the first frame update
    public void Awake()
    {
        DontDestroyOnLoad(this);
    }

    protected override void Start()
    {
        base.Start();
        Debug.Log("IGGSDKGameObject Start");
        IGGSDKMain.sInitDelegate?.Invoke();
    }

    private Queue<System.Action> m_q = new Queue<System.Action>();

    protected override void Update()
    {
        base.Update();
        if (m_q.Count > 0)
        {
            var action = m_q.Dequeue();
            if (action != null)
            {
                action();
            }
        }
    }

    public void RunInMainThread(System.Action a)
    {
        m_q.Enqueue(a);
    }
}

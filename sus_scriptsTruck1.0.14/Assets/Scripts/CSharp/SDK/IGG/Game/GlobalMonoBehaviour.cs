using System.Collections;
using System.Collections.Generic;
using IGG.SDK.Foundation.Thread;
using IGGUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalMonoBehaviour : MonoBehaviour, ITaskManager
{
    private static GlobalMonoBehaviour instance;
    public static GlobalMonoBehaviour ShareInstance()
    {
        return instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("GlobalMonoBehaviour Start!");
        if (null == instance)
        {
            FacebookUtil.Init();
            instance = this;
        }
    }

    void OnDestroy()
    {
       Debug.Log("GlobalMonoBehaviour Destroy!");
    }

    public void ReloadGameScene()
    {
        DataCenter.OnDestory();
        StartCoroutine(EnterGameScene());
    }

    IEnumerator EnterGameScene()
    {
        Debug.Log("==================================重新进入游戏==================================");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        yield return new WaitForEndOfFrame();
    }

    private Queue<System.Action> queue = new Queue<System.Action>();
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (queue.Count > 0)
        {
            var action = queue.Dequeue();
            if (action != null)
            {
                action();
            }
        }
        
        if (runnables.Count > 0)
        {
            var dequeue = runnables.Dequeue();
            if (null != dequeue)
            {
                Debug.Log("null != action");
                dequeue?.Invoke();
            }
        }
    }
    
    void FixedUpdate()
    {
        if (timerTasks.Count > 0)
        {
            var timerTask = timerTasks.Dequeue();
            if (null != timerTask)
            {
                Debug.Log("null != timerTask");
                if (!timerTask.isScheduled && timerTask.nextExecutionTime < Time.realtimeSinceStartup)
                {
                    Debug.Log("timerTask.Runnable?.Invoke()");
                    timerTask.runnable?.Invoke();
                    timerTask.isScheduled = true;
                }
                else
                {
                    timerTasks.Enqueue(timerTask);
                }
            }
        }
    }

    private Queue<Runnable> runnables = new Queue<Runnable>();
    private Queue<TimerTask> timerTasks = new Queue<TimerTask>();
    public void AddRunnable(Runnable runnable)
    {
        Debug.Log("AddRunnable");
        runnables.Enqueue(runnable);
    }

    public void RemoveRunnable(Runnable runnable)
    {
        Debug.Log("RemoveRunnable");
        runnables.Dequeue();
    }

    public void AddTimerTask(Runnable runnable, float delay)
    {
        Debug.Log("addTimerTask");
        TimerTask timerTask = new TimerTask();
        timerTask.runnable = runnable;
        timerTask.isScheduled = false;
        timerTask.nextExecutionTime = Time.realtimeSinceStartup + delay;

        timerTasks.Enqueue(timerTask);
    }

    public void RemoveTimerTask(Runnable runnable)
    {
        foreach (var timerTask in timerTasks)
        {
            if (timerTask.runnable == runnable)
            {
                timerTasks.Dequeue();
                break;
            }
        }
    }

    public void RunInMainThread(System.Action a)
    {
        queue.Enqueue(a);
    }
}

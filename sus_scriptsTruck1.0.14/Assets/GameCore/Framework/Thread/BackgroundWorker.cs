using Framework;
using System;
using System.Collections.Generic;
using System.Threading;

public class BackgroundWorker : Framework.CSingleton<BackgroundWorker>
{
	public delegate void BackgroudDelegate();


	private bool bRequestExit;

	private Queue<BackgroundWorker.BackgroudDelegate> PendingWork = new Queue<BackgroundWorker.BackgroudDelegate>();

	//private List<BackgroundWorker.BackgroudDelegate> WorkingList = new List<BackgroundWorker.BackgroudDelegate>();

	//public int ThreadID;
    private Thread[] WorkingThread;
    private int threadNum = 8;

    protected override void Init()
	{
        this.WorkingThread = new Thread[threadNum];
        for (int i = 0; i < threadNum; ++i) {
            this.WorkingThread[i] = new Thread(BackgroundWorker.StaticEntry);
            this.WorkingThread[i].IsBackground = true;
        }
        //this.ThreadID = this.WorkingThread.ManagedThreadId;
        //this.WorkingThread.Start();
        for (int i = 0; i < threadNum; ++i)
        {
            this.WorkingThread[i].Start();
        }
    }

    protected override void UnInit()
	{
		this.bRequestExit = true;
        for (int i = 0; i < threadNum; ++i)
        {
            this.WorkingThread[i].Join();
        }
        WorkingThread = null;
    }

	protected static void StaticEntry()
	{
		BackgroundWorker.Instance.Entry();
	}

	private static void Swap<T>(ref T a, ref T b)
	{
		T t = a;
		a = b;
		b = t;
	}

	protected void Entry()
	{
		while (!this.bRequestExit)
		{
            BackgroudDelegate action = null;

            lock (this.PendingWork)
			{
                if(this.PendingWork.Count > 0)
                {
                    action = this.PendingWork.Dequeue();
                }

            }
            if(action != null)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    LOG.Error(ex);
                }
            }
            Thread.Sleep(60);
		}
	}

	public void AddBackgroudOperation(BackgroundWorker.BackgroudDelegate InDelegate)
	{
		lock (this.PendingWork)
		{
			this.PendingWork.Enqueue(InDelegate);
		}
	}
}

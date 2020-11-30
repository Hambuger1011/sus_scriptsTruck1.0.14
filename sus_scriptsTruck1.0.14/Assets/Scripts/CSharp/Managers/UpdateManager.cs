using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void UpdateHandler(float deltaTime);

public class UpdateManager
{
    private class TimerArgs
    {
        public int repeatCount;
        public float currentTime;
        public float delay;
        public VoidEventHandler handler;
    }

    private class DelayCallArgs
    {
        public float currentTime;
        public float delay;
        public object parameter;
        public EventHandler handler;
    }
        
    private UpdateHandler mUpdateHandler;
    private List<TimerArgs> mTimerHandlers;
    private List<TimerArgs> mTimerDeleteHandlers;
    private List<DelayCallArgs> mDelayCallHandlers;
    private List<DelayCallArgs> mDelayDeleteHandlers;
	private object mLocker;
        
    static private UpdateManager mInstance = null;
    static public UpdateManager GetInstance()
    {
        if (mInstance == null)
            mInstance = new UpdateManager();
        return mInstance;
    }

    static public void DisposeInstance()
    {
        if (mInstance != null)
        {
            mInstance.Dispose();
            mInstance = null;
        }
    }
        
    private UpdateManager()
    {
		mLocker = new object();
        mTimerHandlers = new List<TimerArgs>();
        mTimerDeleteHandlers = new List<TimerArgs>();
        mDelayCallHandlers = new List<DelayCallArgs>();
        mDelayDeleteHandlers = new List<DelayCallArgs>();
    }
        
    public void Update(float deltaTime)
    {
        if (mUpdateHandler != null)
            mUpdateHandler(deltaTime);
        
        if (mTimerDeleteHandlers.Count > 0)
        {
            int count = mTimerDeleteHandlers.Count;
            for (int i=0; i<count; i++)
                mTimerHandlers.Remove(mTimerDeleteHandlers[i]);
            
            mTimerDeleteHandlers.Clear();
        }

        if (mTimerHandlers.Count > 0)
        {
            int count = mTimerHandlers.Count;
            for (int i=0; i<count; i++)
            {
                TimerArgs args = mTimerHandlers[i];
                args.currentTime += deltaTime;
                if (args.currentTime >= args.delay)
                {
#if UNITY_EDITOR
                    args.handler();
#else
                    try
                    {
                        args.handler();
                    }
                    catch 
                    {
                        mTimerDeleteHandlers.Add(args);
                        continue ;
                    }
#endif

                    args.currentTime -= args.delay;
                    if (args.repeatCount != -1)
                    {
                        args.repeatCount --;
                        if (args.repeatCount <= 0) 
                            mTimerDeleteHandlers.Add(args);
                    }
                }
            }
        }
		
		lock (mLocker)
		{
			if (mDelayDeleteHandlers.Count > 0)
			{
				int count = mDelayDeleteHandlers.Count;
				for (int i=0; i<count; i++)
					mDelayCallHandlers.Remove(mDelayDeleteHandlers[i]);
				
				mDelayDeleteHandlers.Clear();
            }

            if (mDelayCallHandlers.Count > 0)
            {
                int count = mDelayCallHandlers.Count;
		        for (int i=0; i<count; i++)
		        {
		            DelayCallArgs args = mDelayCallHandlers[i];
		            args.currentTime += deltaTime;
		            if (args.currentTime >= args.delay)
		            {
						
#if UNITY_EDITOR
						args.handler(args.parameter);
#else
						try
						{
							args.handler(args.parameter);
						}
						catch 
						{
							mDelayDeleteHandlers.Add(args);
							continue ;
						}
#endif

		                args.currentTime -= args.delay;
		                mDelayDeleteHandlers.Add(args);
		            }
		        }
		    }
		}
    }

    /// 增加帧函数(每帧调用)
    public void AddUpdateHandler(UpdateHandler handler)
    {
        mUpdateHandler += handler;
    }
    /// 删除帧函数
    public void RemoveUpdateHandler(UpdateHandler handler)
    {
        mUpdateHandler -= handler;
    }
    /// 增加定时函数
    /// delay 调用间隔(单位:秒)
    /// repeatCount 重复次数
    public void AddTimerHandler(VoidEventHandler handler, float delay, int repeatCount=-1)
    {
        TimerArgs args = new TimerArgs();
        args.repeatCount = repeatCount;
        args.delay = delay;
        args.currentTime = 0f;
        args.handler = handler;
        mTimerHandlers.Add(args);
    }
    /// 删除定时函数
    public void RemoveTimerHandler(VoidEventHandler handler)
    {
        int count = mTimerHandlers.Count;
        for (int i=0; i<count; i++)
        {
            if (mTimerHandlers[i].handler == handler)
                mTimerDeleteHandlers.Add(mTimerHandlers[i]);
        }
    }
    /// 延迟调用
    /// delay 延迟时间(单位:秒)
    public void DelayCall(VoidEventHandler handler, float delay)
    {
        AddTimerHandler(handler, delay, 1);
    }
    public void RemoveDelayCall(VoidEventHandler handler)
    {
        RemoveTimerHandler(handler);
    }
    public void DelayCall(EventHandler handler, float delay, object parameter)
    {
        DelayCallArgs args = new DelayCallArgs();
        args.currentTime = 0f;
        args.delay = delay;
        args.handler = handler;
		args.parameter = parameter;
		lock (mLocker)
		{
        	mDelayCallHandlers.Add(args);
		}
    }
    public void DelayCallNextFrame(EventHandler handler, object parameter)
	{
		DelayCallArgs args = new DelayCallArgs();
		args.currentTime = 0f;
		args.delay = 0f;
		args.handler = handler;
		args.parameter = parameter;
		lock (mLocker)
		{
        	mDelayCallHandlers.Add(args);
		}
    }
    public void RemoveDelayCall(EventHandler handler)
    {
		lock (mLocker)
		{
        	int count = mDelayCallHandlers.Count;
        	for (int i=0; i<count; i++)
        	{
            	if (mDelayCallHandlers[i].handler == handler)
                	mDelayDeleteHandlers.Add(mDelayCallHandlers[i]);
        	}
		}
    }
    public void Dispose()
    {
        mUpdateHandler = null;
        mTimerHandlers = null;
        mTimerDeleteHandlers = null;
        mDelayCallHandlers = null;
        mDelayDeleteHandlers = null;
    }
}

using Framework;

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UberLogger
{
    public class ObjectPool<T> where T : class, new()
    {
        private readonly Stack<T> m_Stack = new Stack<T>();

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Stack.Count; } }

        Func<T> mSpawnFunc = null;
        Action<T> m_ActionOnRelease = null;

        public ObjectPool(Func<T> spawnFunc = null, Action<T> actionOnRelease = null)
        {
            mSpawnFunc = spawnFunc;
            m_ActionOnRelease = actionOnRelease;
        }

        public T Get()
        {
            T element = null;
            if(m_Stack.Count > 0) {
                element = m_Stack.Pop();
            }
            if(element == null) 
            {

                //UnityEngine.Profiling.Profiler.BeginSample(string.Format("new {0}",typeof(T)));
                if(mSpawnFunc != null) 
                {
                    element = mSpawnFunc();
                }
                else {
                    element = new T();
                }
                //UnityEngine.Profiling.Profiler.EndSample();
            }
            return element;
        }

        public void Release(T element)
        {
            if(element == null)
            {
                return;
            }
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                LOG.Error("Internal error. Trying to destroy object that is already released to pool.");
            if (m_ActionOnRelease != null)
                m_ActionOnRelease(element);
            m_Stack.Push(element);
        }
    }
}

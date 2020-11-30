using Framework;

using UnityEngine;
using System.Collections;

namespace Framework
{
    [DisallowMultipleComponent]
    public class CMonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static GameObject mGameMainObj = null;
        private static T mInstance = null;
        public static T Instance {
            get
            {
                if (mInstance == null)
                {
					LOG.Error ("GameObject未加载:"+typeof(T));
                }
                return mInstance;
            }
        }

        public static  bool HasInstance { get { return mInstance != null; } }

        void Awake()
        {
            if(mInstance == null)
            {
                mInstance = this as T;
                Init();
            }
            else
            {
                this.enabled = false;
            }
        }

        void OnDestroy()
        {
            if(mInstance != null)
            {
                UnInit();
                mInstance = null;
            }
        }

        void OnApplicationQuit()
        {
            OnDestroy();
        }

        protected virtual void Init()
        {
        }

        protected virtual void UnInit()
        {
        }
    }
}

using Framework;

using System;
using System.Collections.Generic;

namespace Framework
{

    public class CSingleton<T> where T : class, new()
    {
        private static readonly object _lock = new object();
        private static T s_instance;

        public static T Instance
        {
            get
            {
                if (CSingleton<T>.s_instance == null)
                {
#if ENABLE_DEBUG
                    LOG.Error("未创建实例:"+typeof(T));
#else
                    lock (_lock)
                    {
                        CSingleton<T>.CreateInstance();
                    }
#endif
                }
                return CSingleton<T>.s_instance;
            }
        }

        protected CSingleton()
        {
        }

        public static void CreateInstance()
        {
            if (CSingleton<T>.s_instance == null)
            {
                CSingleton<T>.s_instance = Activator.CreateInstance<T>();
                (CSingleton<T>.s_instance as CSingleton<T>).Init();
            }
        }

        public static void DestroyInstance()
        {
            if (CSingleton<T>.s_instance != null)
            {
                (CSingleton<T>.s_instance as CSingleton<T>).UnInit();
                CSingleton<T>.s_instance = (T)((object)null);
            }
        }

        public static T GetInstance()
        {
            return CSingleton<T>.Instance;
        }

        public static bool HasInstance()
        {
            return CSingleton<T>.s_instance != null;
        }

        protected virtual void Init()
        {
        }

        protected virtual void UnInit()
        {
        }


        public static void CheckDestroy()
        {
            if(s_instance != null)
            {
                LOG.Error("实例未销毁:"+typeof(T));
            }
        }
    }

}

#if NOT_USE_LUA
using AB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic
{
    public class ResPreLoader
    {
        List<CAsset> list = new List<CAsset>();
        public Dictionary<string,CAsset> lstTask = new Dictionary<string, CAsset>();
        public bool isPreLoad = false;
        [NonSerialized]
        public int hadLoaded = 0;

        public void BeginPreLoad()
        {
            isPreLoad = true;
        }

        public void EndPreLoad()
        {
            isPreLoad = false;
        }


        public float GetPreLoadProgress()
        {
            if (!isPreLoad)
            {
                return 0;
            }

            float p = 0;
            for(int i= 0; i < list.Count;++i)
            {
                var task = list[i];
                p += task.Progress();
            }
            p /= list.Count;
            return p;
        }

        public bool IsPreLoadDone()
        {
            if (!isPreLoad)
            {
                return false;
            }

            hadLoaded = 0;
            bool isDone = true;
            for (int i = 0; i < list.Count; ++i)
            {
                var task = list[i];
                if (!task.IsDone())
                {
                    isDone = false;
                    break;
                }
                task.DoneCallback();
                ++hadLoaded;
            }
            return isDone;
        }

        public CAsset PreLoad(enResType resType, string strAssetName, Action<CAsset> finishFunc = null)
        {
            CAsset task;
            //if (this.lstTask.TryGetValue(strAssetName, out task))
            //{
            //    if (task.IsDone())
            //    {
            //        if(task.m_resType == enResType.eObject && task.m_resType == resType)//Object默认加载出来的是Texture2D
            //        {
            //            if (finishFunc != null)
            //            {
            //                finishFunc((CAsset)task);
            //            }
            //            return task;
            //        }

            //    }
            //    else
            //    {
            //        //this.lstTask.Remove(strAssetName);//有可能是空的
            //    }
            //}

            task = ABSystem.Instance.LoadAsync(AbTag.Null, resType, strAssetName, finishFunc);
            if (task == null)
            {
                LOG.Error("预加载失败:type=" + resType + "," + strAssetName);
                return null;
            }
            task.Retain(this);
            this.lstTask[strAssetName] = task;
            list.Add(task);
            return task;
        }

        public void Clean()
        {
            for (int i = 0; i < list.Count; ++i)
            {
                var task = list[i];
                task.Release(this);
            }
            lstTask.Clear();
            list.Clear();
        }
    }
}
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AB
{
    public abstract class ABLoader
    {
        //public enAbLoadState loadState { get; protected set; }
        //public abstract void Load(AbResItem abConfigItem);
        public abstract float GetProgress();
        //public abstract bool IsDone();
        public abstract int GetAllSize();
        public abstract int GetCurSize();
        public abstract AssetBundle GetAssetBundle(ref string strError);
        public abstract IEnumerator DoUpdate();
    }
}

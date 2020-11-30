using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AB
{
    public class ABLoader_File : ABLoader
    {

        AbResItem abConfigItem;
        public float m_progress;

        AssetBundle assetbundle;
        string strError;

        ~ABLoader_File()
        {
            this.abConfigItem = null;
        }

        public ABLoader_File(AbResItem abConfigItem)
        {
            this.abConfigItem = abConfigItem;
        }

        public override IEnumerator DoUpdate()
        {
            var loadRequest = AssetBundle.LoadFromFileAsync(abConfigItem.abFilePath);
            while (!loadRequest.isDone)
            {
                this.m_progress = loadRequest.progress;
                yield return null;
            }
            this.m_progress = 1;
            this.assetbundle = loadRequest.assetBundle;
            yield break;
        }

        public override int GetAllSize()
        {
            if (abConfigItem != null)
            {
                return abConfigItem.size;
            }
            return 0;
        }

        public override int GetCurSize()
        {
            int curNum = 0;
            if (abConfigItem != null)
            {
                 curNum = (int)Math.Ceiling(this.m_progress * abConfigItem.size);
            }
            return curNum;
        }

        public override float GetProgress()
        {
            return m_progress;
        }


        public override AssetBundle GetAssetBundle(ref string strError)
        {
            if (assetbundle == null)
            {
                strError = this.strError;
                return null;
            }
            return assetbundle;
        }


        //public AssetBundleCreateRequest loadRequest = null;
        //public override void Load(AbResItem abConfigItem)
        //{
        //    loadState = enAbLoadState.eLoading;
        //    loadRequest = AssetBundle.LoadFromFileAsync(abConfigItem.abFilePath);
        //}

        //public override float GetProgress()
        //{
        //    if (loadRequest == null)
        //    {
        //        return 0;
        //    }
        //    return loadRequest.progress;
        //}

        //public override bool IsDone()
        //{
        //    if (loadRequest == null)
        //    {
        //        return false;
        //    }
        //    return loadRequest.isDone;
        //}

        //public override AssetBundle GetAssetBundle(ref string strError)
        //{
        //    if (loadRequest == null)
        //    {
        //        return null;
        //    }
        //    var assetbundle = loadRequest.assetBundle;
        //    if (assetbundle != null)
        //    {
        //        loadState = enAbLoadState.eLoadSuc;
        //    }
        //    else
        //    {
        //        loadState = enAbLoadState.eLoadFail;
        //        strError = "加载bundle为空";
        //    }
        //    return assetbundle;
        //}
    }
}

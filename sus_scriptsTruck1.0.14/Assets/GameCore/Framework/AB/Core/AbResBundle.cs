namespace AB
{
    using Framework;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UGUI;
    using UnityEngine;
    using UnityEngine.Networking;

    public class AbResBundle : AbWork
    {
        public AbResBundle(string rootPath) : base(rootPath)
        {
        }

        #region 下载配置
        public virtual IEnumerator DoLoadBundleConfig(string strAssetName, Action<bool> callback,Action<float> progressCallBack)
        {
            yield return null;
        }       

        public virtual IEnumerator DoDownload(string name, string strVer, Action<bool, string> callback, Action<float> progressCallBack)
        {
            yield return null;
        }
        
        public virtual IEnumerator DoDownload(string name, string strVer, Action<bool, string,string> callback, Action<float> progressCallBack)
        {
            yield return null;
        }



#endregion
    }
}
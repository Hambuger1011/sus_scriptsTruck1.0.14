using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace AB
{
    public class ABLoader_WebUnity : ABLoader
    {
        public static int TIME_OUT = 30;
        AbResItem abConfigItem;
        public float m_progress;
        private int curByteSize = 0;
        AssetBundle assetbundle;
        string strError;

        ~ABLoader_WebUnity()
        {
            this.abConfigItem = null;
        }

        public ABLoader_WebUnity(AbResItem abConfigItem)
        {
            this.abConfigItem = abConfigItem;
        }

        public override IEnumerator DoUpdate()
        {

            var resVersion = UserDataManager.Instance.ResVersion;//abConfigItem.crc.ToString();
            int tryCount = 0;

            int TIME = TIME_OUT;
            while (true)
            {
                yield return null;
                ++tryCount;
                //LOG.Error("abResConfig:" + url);

                float time = 0;
                bool isDone = false;
                bool isError = false;

                var url = this.abConfigItem.abFilePath;
                uint version = (uint)abConfigItem.crc;
#if UNITY_2018_1_OR_NEWER
                using (var req = UnityWebRequestAssetBundle.GetAssetBundle(url, version, 0))
#else
                using (var req = UnityWebRequest.GetAssetBundle(url,version,0))
#endif
                {
                    //Debug.Log("=====ConfigDownLoad=====>" + url);
                    req.timeout = TIME;
                    //req.useHttpContinue = true;

                    req.SendWebRequest();
                    while (!req.isDone)
                    {
                        this.m_progress = req.downloadProgress;
                        this.curByteSize = (int)req.downloadedBytes;

                        time += Time.deltaTime;
                        if (time > TIME)
                        {
                            //if (TIME < 60)
                            {
                                TIME += 10;
                            }
                            LOG.Error("下载超时:" + abConfigItem.abFilePath + "\n" + time);
                            isError = true;
                            req.Abort();
                            req.Dispose();
                            break;
                        }

                        yield return null;
                    }

                    if (isError)
                    {
                        continue;
                    }

                    if (req.isNetworkError || req.isHttpError || !string.IsNullOrEmpty(req.error))
                    {
                        LOG.Error("加载错误:" + req.error + ",url = " + url);
                        continue;
                    }


                    this.m_progress = 1;
                    this.assetbundle = (req.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
                    break;
                }
            }
            yield break;
        }



        public override float GetProgress()
        {
            return m_progress;
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
            return curByteSize;
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

    }
}






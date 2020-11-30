using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

namespace AB
{
    public class ABLoader_WebClient : ABLoader
    {
        public static int TIME_OUT = 10;
        AbResItem abConfigItem;
        public float m_progress;
        string filename;
        string version;
        string resVersion;
        AbWork context;
        AssetBundle assetbundle;
        string strError;

        ~ABLoader_WebClient()
        {
            this.abConfigItem = null;
        }

        public ABLoader_WebClient(AbWork context, AbResItem abConfigItem)
        {
            this.context = context;
            this.abConfigItem = abConfigItem;
            filename = GetPath(abConfigItem);
            version = CFileManager.ReadFileString(filename+".txt");
        }

        public static string GetPath(AbResItem abConfigItem)
        {
            return AbUtility.abWritablePath + "/ab_cache/" + abConfigItem.bundleContext.rootPath +"/" + abConfigItem.fileHashName;
        }

        bool IsDone()
        {
            return this.strError != null || this.assetbundle != null;
        }

        public override IEnumerator DoUpdate()
        {

            resVersion = abConfigItem.crc.ToString();
            while (true)
            {

                //下载ab
                var doDownload = DoDownload();
                while (doDownload.MoveNext())
                {
                    yield return null;
                }
                yield return null;
                var doLoad = DoLoadFile();
                while (doLoad.MoveNext())
                {
                    yield return null;
                }
                if (this.IsDone())
                {
                    break;//结束下载
                }
            }
        }

        private int m_cursize = 0;
        private int talsize = 0;
        IEnumerator DoDownload()
        {
            if (abConfigItem != null) { talsize = abConfigItem.size; }
            if (version == resVersion && File.Exists(filename))//已经下载过
            {
                this.m_progress = 1;
                m_cursize = talsize;
                yield break;
            }
            var info = DownloadMgr.Instance.Download(abConfigItem.abFilePath, filename, resVersion, 0, false);
            while (!info.isDone)
            {
                m_progress = info.Progress();
                m_cursize = info.downloadedBytes;
                yield return null;
            }
            m_cursize = info.size;
            this.m_progress = 1;
        }

        int loadErrorCount = 0;
        IEnumerator DoLoadFile()
        {
            //LOG.Error("下载:" + this.BytesReceived + "/" + this.TotalBytesToReceive + "," + this);
            if (File.Exists(filename))
            {
                var asyncOpt = AssetBundle.LoadFromFileAsync(filename);
                if(asyncOpt == null)
                {
                    strError = "LoadFromFileAsync失败:\n" + abConfigItem.abFilePath + "|" + abConfigItem.filename;
                    Debug.LogError(strError);
                    CFileManager.WriteFileString(filename + ".txt", "");
                    yield break;
                }
                while (!asyncOpt.isDone)
                {
                    yield return asyncOpt;
                }
                this.assetbundle = asyncOpt.assetBundle;
                if (assetbundle == null)
                {
                    ++loadErrorCount;
                    var fi = new FileInfo(filename);
                    var err = loadErrorCount+".加载bundle为空:\n" + abConfigItem.abFilePath + "|" + abConfigItem.filename+"\nsize="+fi.Length+"md5:"+CFileManager.GetFileMd5(filename);
                    if (loadErrorCount >= 3)
                    {
                        var b = CBundle.Get(context, this.abConfigItem, false);
                        strError = err + "\nother:" + b.assetbundle;
                    }
                    Debug.LogError(err);
                    CFileManager.WriteFileString(filename + ".txt", "");
                }
                else
                {
                    if (version != resVersion)
                    {
                        version = resVersion;
                        CFileManager.WriteFileString(filename + ".txt", version);
                        //LOG.Error("===>" + PlayerPrefs.GetInt(filename, 0));
                    }
                }
            }
            else
            {
                strError = "ab文件不存在:" + filename;
                LOG.Error(strError);
                CFileManager.WriteFileString(filename + ".txt", "");
            }
        }

        public bool IsDownload()
        {
            bool isBoo = false;

            if (version == resVersion && File.Exists(filename)) //已经下载过
            {
                isBoo = true;
            }

            return isBoo;
        }

        public override int GetAllSize()
        {
            if (abConfigItem != null)
            {
                if (version == resVersion && File.Exists(filename))//已经下载过
                {
                    return 0;
                }
                return abConfigItem.size;
            }
            return 0;
        }

        public override int GetCurSize()
        {
            if (abConfigItem != null)
            {
                if (version == resVersion && File.Exists(filename))//已经下载过
                {
                    return 0;
                }
                //int cursize=(int)(abConfigItem.size * m_progress);
                return m_cursize;
            }
            return 0;
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

    }
}

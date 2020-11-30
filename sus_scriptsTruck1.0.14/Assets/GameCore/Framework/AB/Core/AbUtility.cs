#define USE_WEB_ASSETBUNDLE
namespace AB
{
    using UnityEngine;
    using System.Collections;
    using System.IO;
    using System.Text.RegularExpressions;
    using System;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    /*
       android 的数据是区分大小写的文件系统(/data/file，/data/File会被视为两个文件(夹))，为避免大小写敏感，统一使用路径小写
    */
    public static class AbUtility
    {
        public static enLoadType loadType = enLoadType.eWebUnity;

#if true//UNITY_EDITOR
        /// <summary>
        /// 把资源路径名字整理
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static string NormalizerAbName(string assetName)
        {
            if (assetName == null)
            {
                return null;
            }
            return assetName.Replace('\\', '/').ToLower();
        }

        public static string NormalizerDir(string dir)
        {
            //dir = Regex.Replace(dir, @"\\+|/+", Path.AltDirectorySeparatorChar.ToString());
            if ('/' != Path.AltDirectorySeparatorChar)
            {
                return dir.Replace('/', Path.AltDirectorySeparatorChar);
            }
            else
            {
                return dir.Replace('\\', Path.AltDirectorySeparatorChar);
            }
        }
#endif

        /// <summary>
        /// 获取本地时间戳
        /// </summary>
        public static int GetLocalUtcTimestamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return (int)ts.TotalSeconds;//获取10位
                                        //return (int)ts.TotalMilliseconds; //获取13位
        }

        static string _abWritablePath = null;
        /// <summary>
        /// 本地存放打包的AssetBundle
        /// </summary>
        public static string abWritablePath
        {
            get
            {
                if (_abWritablePath == null)
                {
                    _abWritablePath = GameUtility.GetPath(string.Format("{0}ab/", GameUtility.WritablePath));
                }
                return _abWritablePath;
            }
        }
#if false



        public static string abZipPath
        {
            get
            {
#if false
                return string.Format("{0}ab/{1}/", AbUtility.ReadonlyPath, AbUtility.Platform);
#else
                return string.Concat(GameUtility.ReadonlyPath, "Res/");
#endif
            }
        }
#endif
        static string _abReadonlyPath = null;
        public static string abReadonlyPath
        {
            get
            {
                if (_abReadonlyPath == null)
                {
#if UNITY_EDITOR
                    _abReadonlyPath = string.Format("{0}ab/{1}/", GameUtility.WritablePath, GameUtility.Platform);
#else
                    _abReadonlyPath = string.Format("{0}ab/{1}/", GameUtility.ReadonlyPath, GameUtility.Platform);
#endif
                    if (GameUtility.isEditorMode)
                    {
                        GameUtility.GetPath(_abReadonlyPath);
                    }
                }
                return _abReadonlyPath;
            }
        }

        public static string AbBuildPath
        {
            get
            {
                return string.Format("{0}abbuild/{1}/", GameUtility.WritablePath, GameUtility.Platform);
            }
        }

        static string _abUri = null;
        public static string abUri
        {
            get
            {
                if (_abUri == null)
                {
                    _abUri = GameHttpNet.Instance.GetAssetBundleUrl();
                }
                return _abUri;
            }
        }

        static string _bookUri = null;
        public static string bookUri
        {
            get
            {
                if (_bookUri == null)
                {
                    _bookUri = GameHttpNet.Instance.GetBookABUrl();
                }
                return _bookUri;
            }
        }
    }

    public enum enLoadType
    {
        eFile = 0,
        eWebClient = 1,
        eWebUnity = 2,
    }
}

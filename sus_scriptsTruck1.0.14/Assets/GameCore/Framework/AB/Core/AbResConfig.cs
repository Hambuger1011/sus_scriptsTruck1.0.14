namespace AB
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using System.IO;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using System.IO;

    public class AbResConfig
    {
        [Header("最后修改时间")]
        public string lastModify = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");

        //[Header("AB文件名字")]
        //public string fileHashName;

        [Header("AssetBundle列表")]
        public List<AbResItem> abConfItems = new List<AbResItem>();

        public int assetCount = 0;

        [System.NonSerialized]
        public AbWork bundleContext;


        public void SaveInEditor(string savePath)
        {
            UpdateLastModify();
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            using(FileStream fs = File.Open(savePath, FileMode.OpenOrCreate))
            {
                using(BinaryWriter bw = new BinaryWriter(fs))
                {
                    this.Write(bw);
                }
            }
        }

        public void UpdateLastModify()
        {
            lastModify = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
        }


        #region map


        /// <summary>
        /// 资源名(assets/***.*)2AbConfigItem
        /// </summary>
        public Dictionary<string, AbResItem> assetsMap;


        /// <summary>
        /// ab文件名2AbConfigItem
        /// </summary>
        public Dictionary<string, AbResItem> abFileMap;

        /// <summary>
        /// assetname路径（全小写）
        /// </summary>
        public AbResItem GetConfigItemByAssetName(string assetHashCode)
        {
            AbResItem config = null;
            //assetName = AbUtility.NormalizerAbName(assetName);
            var configMap = this.assetsMap;
            if (configMap.TryGetValue(assetHashCode, out config))
            {
                return config;
            }
            Debug.LogError("asset资源未打进包:" + assetHashCode);
            return null;
        }

        public AbResItem GetConfigItemByAbName(string abFileName)
        {
            AbResItem config = null;
            var configMap = this.abFileMap;
            if (!configMap.TryGetValue(abFileName, out config))
            {
                Debug.LogError("ab资源包不存在:" + abFileName);
            }
            return config;
        }
        #endregion


        public void Init()
        {
           
        }

        public void Read(byte[] buff)
        {
            using (MemoryStream ms = new MemoryStream(buff))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    this.Read(br);
                }
            }
        }



        public void Write(BinaryWriter bw)
        {
            bw.WriteString(this.lastModify);
            //bw.WriteString(this.fileHashName);

            this.assetCount = 0;
            int cnt = this.abConfItems.Count;
            for (int i = 0; i < cnt; ++i)
            {
                var item = abConfItems[i];
                this.assetCount += item.assetNames.Length;
            }
            bw.Write(this.assetCount);
            bw.Write(cnt);
            for (int i = 0; i < cnt; ++i)
            {
                var item = abConfItems[i];
                item.Write(bw);
                this.assetCount += item.assetNames.Length;
            }
        }
        public void Read(BinaryReader br)
        {
            this.lastModify = br.ReadString();
            //this.fileHashName = br.ReadString();

            this.assetCount = br.ReadInt32();
            int cnt = br.ReadInt32();
            assetsMap = new Dictionary<string, AbResItem>(this.assetCount);
            abFileMap = new Dictionary<string, AbResItem>(cnt);
            this.abConfItems.Capacity = cnt;
            for (int i = 0; i < cnt; ++i)
            {
                AbResItem item = new AbResItem();
                item.bundleContext = this.bundleContext;
                item.Read(br);
                abFileMap.Add(item.filename, item);

                for (var j = 0; j < item.assetNames.Length; ++j)
                {
                    var assetName = item.assetNames[j];
                    //Debug.Log("---assetNames-->>" + assetName);
                    assetsMap.Add(assetName, item);
                }
#if UNITY_EDITOR
                abConfItems.Add(item);
#endif
            }
        }
    }



    [Serializable]
    public class AbResItem
    {
        public static readonly Hash128 NullHash = new Hash128(0, 0, 0, 0);
        public string filename;
        /// <summary>
        /// AB文件名字
        /// </summary>
        [Header("AB文件名字")]
        public string fileHashName;

        /// <summary>
        /// bundle类型
        /// </summary>
        [Header("bundle类型")]
        public enResType abType = enResType.eObject;

        //public bool isDependency = false;

        //public string hashValue;

        public uint crc;

        [Header("大小")]
        public int size;
        /// <summary>
        /// asset列表
        /// </summary>
        [Header("asset列表")]
        public string[] assetNames;

        // bundle依赖;
        [Header("依赖列表(ab文件名)")]
        public string[] dependencyList;

        //[Header("main asset列表")]
        //public string[] mainAssetNames;

        [NonSerialized]
        public AbWork bundleContext;

        [NonSerialized]
        string _abFilePath = null;
        public string abFilePath
        {
            get
            {
                if (_abFilePath == null)
                {
#if false
                    _abFilePath = string.Concat(AbUtility.abReadonlyPath, this.config.bundleContext.rootPath, this.fileHashName);
                    if (!File.Exists(_abFilePath))
                    {
                        _abFilePath = string.Concat(AbUtility.abReadonlyPath, this.fileHashName);
                    }
#elif ENABLE_DEBUG
                    switch (AbUtility.loadType)
                    {
                        case enLoadType.eFile:
                            _abFilePath = string.Concat(AbUtility.abReadonlyPath, this.bundleContext.rootPath, this.fileHashName);
                            return _abFilePath;
                        default: 
                            break;
                    }
#endif
                    if (this.bundleContext.rootPath.Equals("common/"))
                        _abFilePath = string.Concat(AbUtility.abUri, this.fileHashName, "?", UserDataManager.Instance.ResVersion, this.crc);
                    else
                    {
                        string[] sArray = this.bundleContext.rootPath.Split(new string[] { "book_" ,"/"}, StringSplitOptions.RemoveEmptyEntries); //进行字符串的截取
                        var version = UserDataManager.Instance.GetBookVersion(Convert.ToInt32(sArray[0]));
                        _abFilePath = string.Concat(AbUtility.bookUri, this.bundleContext.rootPath, this.fileHashName, "?", version, this.crc);
                    }
                }
                //Debug.LogError("===DownLoadFile===>>"+ filename);
                return _abFilePath;
            }
        }

        //Hash128 _Hash;
        ///// <summary>
        ///// 4个32位的uint组成
        ///// </summary>
        //public Hash128 Hash
        //{
        //    get
        //    {
        //        if (_Hash == AbResItem.NullHash)
        //        {
        //            _Hash = Hash128.Parse(this.hashValue);
        //        }
        //        return _Hash;
        //    }
        //}

        public AbResItem() { }

        public AbResItem(string filename, string name, enResType type, uint crc, int size, string[] assetNames, string[] deps)
        {
            this.filename = filename;
            this.fileHashName = name;//Path.GetFileName(name);
            this.abType = type;
            //this.isDependency = isDependency;
            //this.hashValue = hash;
            this.crc = crc;
            this.dependencyList = deps;
            this.assetNames = assetNames;
            this.size = size;
        }

        public override string ToString()
        {
            //return base.ToString();
            return filename;
        }


        public void Read(BinaryReader br)
        {
            this.filename = br.ReadString();
            this.fileHashName = br.ReadString();
            this.abType = (enResType)br.ReadInt32();
            this.crc = br.ReadUInt32();
            int cnt = br.ReadInt32();
            assetNames = new string[cnt];
            for (int i = 0; i < cnt; ++i)
            {
                var assetName = br.ReadString();
                assetNames[i] = assetName;
            }

            cnt = br.ReadInt32();
            dependencyList = new string[cnt];
            for (int i = 0; i < cnt; ++i)
            {
                var assetName = br.ReadString();
                dependencyList[i] = assetName;
            }
            this.size = br.ReadInt32();
        }

        public void Write(BinaryWriter bw)
        {
            bw.WriteString(this.filename);
            bw.WriteString(this.fileHashName);
            bw.Write((int)this.abType);
            bw.Write(this.crc);
            int cnt = this.assetNames.Length;
            bw.Write(cnt);
            for(int i = 0; i < cnt; ++i)
            {
                var assetName = assetNames[i];
                bw.WriteString(assetName);
            }


            cnt = this.dependencyList.Length;
            bw.Write(cnt);
            for (int i = 0; i < cnt; ++i)
            {
                var assetName = dependencyList[i];
                bw.WriteString(assetName);
            }
            bw.Write(this.size);
        }
    }

}
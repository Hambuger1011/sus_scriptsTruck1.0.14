

namespace AB
{
    using AB;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class AbBuilder
    {

        public const bool isDebug = false;
        public string buildPlatform { get; private set; }
        public BuildTarget buildTarget { get; private set; }
        public string outputPath { get; private set; }
        public bool success { get; private set; }
        public AbAnalyze analyzer { get; private set; }

        public AbstractAbOptions abOptions { get; private set; }



    public AbBuilder(AbstractAbOptions abBuilder)
        {
            buildTarget = EditorUserBuildSettings.activeBuildTarget;
            buildPlatform = GameUtility.Platform;
            abOptions = abBuilder;
            abOptions.builder = this;
            this.outputPath = AbUtility.AbBuildPath + abBuilder.SystemName + "/";
        }
        public void Begin()
        {
            Clear();
            success = false;
#if UNITY_2017_1_OR_NEWER
            Caching.ClearCache();
#else
            Caching.CleanCache();
#endif
            Directory.CreateDirectory(this.outputPath);
            if (EditorUtility.DisplayCancelableProgressBar("Loading", "Loading...", 0.1f))
            {
                throw new Exception("用户停止");
            }

            this.analyzer = new AbAnalyze();
            if (abOptions != null)
            {
                abOptions.OnBegin();
            }
            analyzer.OnBegin(this.outputPath, abOptions);
            
        }

        public void End()
        {
            analyzer.OnEnd();
            if (abOptions != null)
            {
                abOptions.OnEnd();
            }
            Clear();
            EditorUtility.ClearProgressBar();
        }

        void Clear()
        {
            this.analyzer = null;
        }

        public void Analyze()
        {
            this.analyzer.Analyze();
            //var bundles = analyzer.config.bundles;
            //LOG.Info("==================" + bundles.Count);

            //HashSet<string> useAbFilename = new HashSet<string>();
            //HashSet<string> useAbAssets = new HashSet<string>();
            //for (int i = 0; i < bundles.Count; i++)
            //{
            //    List<string> assetNames = new List<string>();
            //    AbPackage target = bundles[i];
            //    assetNames.AddRange(target.objs);
            //    assetNames.AddRange(target.deps);
            //    assetNames = assetNames.OrderBy(e => e).ToList();
            //    foreach (var it in assetNames)
            //    {
            //        if (!useAbAssets.Add(it))
            //        {
            //            throw new Exception("资源重复:" + it+", in " + target.abFileName);
            //        }
            //    }
            //    if (EditorUtility.DisplayCancelableProgressBar(string.Format("创建build({0}/{1})", i, bundles.Count), target.abFileName, (float)i / bundles.Count))
            //    {
            //        throw new Exception("用户停止");
            //    }
            //    if (assetNames.Count <= 0)
            //    {
            //        continue;
            //    }

            //    if (string.IsNullOrEmpty(target.abFileName))
            //    {
            //        //target.name = assetNames[0];
            //        throw new Exception("assetBundleName为空");
            //    }

            //    if (!useAbFilename.Add(target.abFileName))
            //    {
            //        throw new Exception("ab文件重名:" + target.abFileName);
            //    }
                
            //}
            //this.analyzer.Check(useAbFilename, useAbAssets);
        }

        public bool Build()
        {
            //开始打包
            try
            {
                this.analyzer.LoadConfig();


#if USE_Variant
                AssetBundleManifest assetBundleManifest =
                    BuildPipeline.BuildAssetBundles(
                        this.outputPath,
                        this.abOptions.GetBuildAssetBundleOptions(),
                        this.buildTarget
                        );
#else
                List<AssetBundleBuild> list = new List<AssetBundleBuild>();
                foreach(var itr in this.analyzer.config.abMap)
                {
                    AssetBundleBuild abb = new AssetBundleBuild();
                    abb.assetBundleName = itr.Key;
                    string[] assetNames = new string[itr.Value.Count];
                    for(int i=0,iMax = assetNames.Length; i < iMax; ++i)
                    {
                        assetNames[i] = itr.Value[i];
                    }
                    abb.assetNames = assetNames;
                    list.Add(abb);
                }
                AssetBundleManifest assetBundleManifest =
                    BuildPipeline.BuildAssetBundles(
                        this.outputPath,
                        list.ToArray(),
                        this.abOptions.GetBuildAssetBundleOptions(),//BuildAssetBundleOptions.DeterministicAssetBundle,
                        this.buildTarget
                        );
#endif

                if (assetBundleManifest == null)
                {
                    Debug.LogError("打bundle失败");
                    return false;
                }
                success = true;
                Debug.Log("=======================" + this.abOptions.SystemName + "打包bundle完成=======================");
                var isOK = UpdateConfig(this.outputPath, assetBundleManifest);
                if(!isOK)
                {
                    Debug.LogError("打bundle失败");
                    return false;
                }
                RemoveUnused(this.outputPath);
            }
            catch (Exception ex)
            {
                Debug.LogError(this.abOptions.SystemName + "-打bundle失败:\r\n" + ex);
            }
            finally
            {

                EditorUtility.ClearProgressBar();
            }
            return true;
        }
        

        

        bool UpdateConfig(string output, AssetBundleManifest assetBundleManifest)
        {
            Dictionary<string, string> pathDict = new Dictionary<string, string>();
            var editorAbConfig = new AbResConfig();

            //editorAbConfig.platform = this.buildPlatform;
            editorAbConfig.abConfItems.Clear();
            var list = assetBundleManifest.GetAllAssetBundles();
            abOptions.UpdateConfig(output, assetBundleManifest, editorAbConfig);

#region 写入配置
            int p = 0;
            foreach (var _abb in list)
            {
                ++p;
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("写入配置({0}/{1})", p, list.Length), _abb, (float)p / list.Length))
                {
                    throw new Exception("用户停止");
                }

                Hash128 hash = assetBundleManifest.GetAssetBundleHash(_abb);
                if (hash == AbResItem.NullHash)//跳过不合法资源
                {
                    Debug.LogError("非法资源:" + _abb);
                    continue;
                }
                FileInfo file = new FileInfo(this.outputPath + _abb);

                List<string> depList = new List<string>();
                foreach (var depFileName in assetBundleManifest.GetAllDependencies(_abb))
                {
                    if (string.IsNullOrEmpty(depFileName))
                    {
                        continue;
                    }
                    if (assetBundleManifest.GetAssetBundleHash(depFileName) == AbResItem.NullHash)//跳过不合法资源
                    {
                        continue;
                    }
                    depList.Add(depFileName);
                }

                var key = abOptions.GetHash(_abb);
                string s;
                if (pathDict.TryGetValue(key, out s))
                {
                    throw new Exception("路径重复:" + s + "<=>" + _abb + ",md5:" + key);
                    //return ;
                }
                pathDict.Add(key, _abb);
#if true
                uint crc;
                if (!BuildPipeline.GetCRCForAssetBundle(file.FullName, out crc))
                {
                    Debug.LogError("获取crc失败:" + _abb);
                }
                //if (crcDict.TryGetValue(crc, out s))
                //{
                //    Debug.LogError("crcMd5重复:" + s + "<=>" + _abb + ",crc:" + crc);
                //    //return ;
                //}
                //crcDict.Add(crc, _abb);
#else
                uint crc;
                ABVersion abVersion;
                if (!oldVerDict.TryGetValue(fileMd5,out abVersion))
                {
                    abVersion = new ABVersion(fileMd5,curVersion);
                }
                newVerDict.Add(fileMd5, abVersion);
                crc = abVersion.version;
#endif
                List<string> assetNames;
                if (!this.analyzer.config.abMap.TryGetValue(_abb,out assetNames))
                {
                    Debug.LogError(_abb);
                    continue;
                }
                this.analyzer.config.abMap.Remove(_abb);
                var item = new AbResItem(
                        _abb,
                        key,
                        0,
                        crc,
                        (int)file.Length,
                        assetNames.ToArray(),
                        depList.ToArray()
                        );

                //switch (bundle.abType)
                //{
                //    case enResType.eScene:
                //        //item.strSceneName = AssetDatabase.LoadAssetAtPath<Object>(_abb.assetNames[0]).name;
                //        break;
                //    case enResType.eShader:
                //        //editorAbConfig.AddShaders(_abb.assetNames);
                //        break;
                //}
                editorAbConfig.abConfItems.Add(item);
            }

            if(this.analyzer.config.abMap.Count > 0)
            {
                LOG.Error("fuck:" + this.analyzer.config.abMap.Count);
                foreach(var itr in this.analyzer.config.abMap)
                {
                    Debug.LogError(itr.Key);
                }
            }
            editorAbConfig.abConfItems.Sort((x, y) =>
            {
                if (x == null || x.filename == null)
                {
                    Debug.Log(x);
                    return 1;
                }
                return x.filename.CompareTo(y.filename);
            });

            //editorAbConfig.UpdateLastModify();
            editorAbConfig.SaveInEditor(output + "/Config/config" + ABMgr.const_extension);

            //verJson = JsonHelper.ObjectToJson(newVerDict);
            //EditorPrefs.SetString("ab_version", verJson);
#endregion

            return true;
        }

        public static AssetBundleManifest LoadAssetBundleManifest(string output)
        {
            AssetBundleManifest assetBundleManifest = null;
            DirectoryInfo di = new DirectoryInfo(output);
            var bundle = AssetBundle.LoadFromFile(output + di.Name);
            foreach (var key in bundle.GetAllAssetNames())
            {
                assetBundleManifest = bundle.LoadAsset<AssetBundleManifest>(key);
                if (assetBundleManifest != null)
                {
                    break;
                }
            }
            bundle.Unload(false);
            return assetBundleManifest;
        }

        public const string CONFIG_ASSET_PATH = "Config/config.ab";
        /// <summary>
        /// 删除未使用的AB，可能是上次打包出来的，而这一次没生成的
        /// </summary>
        /// <param name="all"></param>
        public void RemoveUnused(string abRootPath)
        {
            var editorAbConfig = new AbResConfig();
            var bytes = File.ReadAllBytes(abRootPath + "/"+ CONFIG_ASSET_PATH);
            editorAbConfig.Read(bytes);
           
            AssetBundleManifest assetBundleManifest = LoadAssetBundleManifest(abRootPath);
            HashSet<string> usedSet = new HashSet<string>();
            usedSet.Add(CONFIG_ASSET_PATH);
 
            for (int i = 0; i < editorAbConfig.abConfItems.Count; i++)
            {
                var bundleItem = editorAbConfig.abConfItems[i];
                usedSet.Add(bundleItem.filename);
            }

            long abFileTotalSize = 0;
            bool isRun = true;
            string strMsg = "";

            DirectoryInfo abAssetPath = new DirectoryInfo(abRootPath + "/assets/");
            #region 删除多余文件
           
            FileInfo[] abFiles = abAssetPath.GetFiles("*" + ABMgr.const_extension, SearchOption.AllDirectories);
            string rootPath = AbUtility.NormalizerAbName(new DirectoryInfo(abRootPath).FullName);
            for (var i = 0; i < abFiles.Length; i++)
            {
                FileInfo fi = abFiles[i];
                string assetFileName = AbUtility.NormalizerAbName(fi.FullName).Replace(rootPath, string.Empty);
                if (!assetFileName.StartsWith("assets/"))
                {
                    throw new System.Exception("路径转换错误:" + assetFileName);
                }
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("检查无用ab{0}({1}/{2})", assetFileName, i + 1, abFiles.Length), string.Format("大小={0}", GameUtility.FormatBytes(fi.Length)), (float)i / abFiles.Length))
                {
                    throw new Exception("用户停止");
                }
                if (assetFileName == CONFIG_ASSET_PATH)
                {
                    continue;
                }
                var hash = assetBundleManifest.GetAssetBundleHash(assetFileName);
                //Debug.Log(name);
                if (hash == AbResItem.NullHash || !usedSet.Contains(assetFileName))
                {
                    Debug.Log("Remove unused AB : " + fi.Name);
                    fi.Delete();
                    //for U5X
                    File.Delete(fi.FullName + ".manifest");
                }
            }
            #endregion

            #region 删除空文件夹
            {
                isRun = true;
                var dirs = abAssetPath.GetDirectories("*", SearchOption.AllDirectories);

                var totalCount = dirs.Length;
                int hadCopyCount = 0;
                object m_lock = new object();
                foreach (var itr in dirs)
                {
                    ThreadPool.QueueUserWorkItem((object obj) =>
                    {
                        try
                        {

                            if (!isRun)
                            {
                                return;
                            }
                            DirectoryInfo item = (DirectoryInfo)obj;
                            if (!Directory.Exists(item.FullName))
                            {
                                return;
                            }

                            if (item.GetFiles("*.*", SearchOption.AllDirectories).Length == 0)
                            {
                                //Directory.Delete(dir.FullName, true);
                                item.Delete(true);
                            }
                        }
                        catch(Exception ex)
                        {
                            Debug.LogError(ex);
                        }
                        finally
                        {
                            lock (m_lock)
                            {
                                ++hadCopyCount;
                            }
                        }
                    },itr);
                }

                while (hadCopyCount < totalCount)
                {
                    if (EditorUtility.DisplayCancelableProgressBar(string.Format("删除空目录({0}/{1})", hadCopyCount, totalCount), strMsg, (float)hadCopyCount / totalCount))
                    {
                        isRun = false;
                        throw new Exception("用户停止");
                    }
                    //Thread.Sleep(1000);
                }
            }
            #endregion
            EditorUtility.ClearProgressBar();
        }
    }


    public abstract class AbstractAbOptions
    {

        public HashSet<string> specialDependencyExt = new HashSet<string>()
        {
            ".unity",
            ".dll",
            ".pdb",
            ".mdb",
        };

        public AbBuilder builder;

        public abstract void OnBegin();

        public virtual void OnEnd()
        {
            if (!this.builder.success)
            {
                return;
            }
            CopyAbFiles();
        }

        public abstract void UpdateConfig(string output, AssetBundleManifest assetBundleManifest, AbResConfig editorAbConfig);

        public abstract string ConfigAssetName { get; }
        public abstract string SystemName { get; }
        public abstract void ScanResources();

        public virtual bool PackShaders()
        {
            return false;
        }

        public virtual BuildAssetBundleOptions GetBuildAssetBundleOptions()
        {
            return BuildAssetBundleOptions.None
                //| BuildAssetBundleOptions.DeterministicAssetBundle//编译资源包使用一个哈希表储存对象ID在资源包中
                | BuildAssetBundleOptions.IgnoreTypeTreeChanges//不使用unity版本兼容，体积会小些
                                                               //| BuildAssetBundleOptions.DisableLoadAssetByFileName
                                                               //| BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension
                ;
        }

        public static Type GetBuildType()
        {

            Type result = null;
            Assembly[] assemblies = new Assembly[]
            {
                Assembly.LoadFile(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Assembly-CSharp-Editor-firstpass.dll"),
                Assembly.LoadFile(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Assembly-CSharp-Editor.dll"),
            };
            if (assemblies != null)
            {
                for (int i = 0; i < assemblies.Length && result == null; i++)
                {
                    Assembly s = assemblies[i];
                    if (!s.ManifestModule.Name.Contains("-Editor"))
                    {
                        continue;
                    }
                    foreach (var t in s.GetTypes())
                    {
                        if (t.IsSubclassOf(typeof(AbstractAbOptions)))
                        {
                            result = t;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        public static AbstractAbOptions Create(AbBuilder context)
        {
            AbstractAbOptions abBuild = null;
            var t = GetBuildType();
            if (t != null)
            {
                abBuild = (AbstractAbOptions)System.Activator.CreateInstance(t, context);
            }
            else
            {
                throw new Exception("未找到IABBuild派生类");
            }
            return abBuild;
        }



        //public virtual AbResConfig GetAbConfigInEditor()
        //{
        //    AbResConfig config = AbResConfig.LoadInEditor(ConfigAssetName);
        //    return config;
        //}

        string GetCopyPath()
        {
            string outPath = String.Empty;
            if (SystemName.Equals("COMMON"))
                outPath = $"{GameUtility.WritablePath}ab/{SystemName.ToLower()}/{GameUtility.Platform}/{GameUtility.version}/{GameUtility.resVersion}/";
            else
                outPath = $"{GameUtility.WritablePath}ab/book/{GameUtility.Platform}/{SystemName.ToLower()}/";

            if (GameUtility.isEditorMode) GameUtility.GetPath(outPath);
            return outPath;
        }


        //复制文件
        public virtual void CopyAbFiles()
        {
            string inPath = this.builder.outputPath;
            string outPath = GetCopyPath();
            StringBuilder md5 = new StringBuilder();
            AssetBundleManifest assetBundleManifest = AbBuilder.LoadAssetBundleManifest(this.builder.outputPath);
            Debug.Log(assetBundleManifest.GetAllAssetBundles().Length);
            //return;

            Dictionary<string, string> hashDict = new Dictionary<string, string>();
            Directory.CreateDirectory(outPath);
            Debug.Log("复制ab文件:inFile=" + inPath + " outFile=" + outPath);

            DirectoryInfo resourceDir = new DirectoryInfo(inPath);
            DirectoryInfo targetDir = new DirectoryInfo(outPath);
            Directory.Delete(outPath, true);
            Directory.CreateDirectory(outPath);

            //var bundle = AssetBundle.LoadFromFile(inPath + "Config/config.ab");
            //var abConfig = bundle.LoadAsset<AbResConfig>(ConfigAssetName);
            //bundle.Unload(false);

            var abConfig = new AbResConfig();
            var bytes = File.ReadAllBytes(inPath + "/" + AbBuilder.CONFIG_ASSET_PATH);
            abConfig.Read(bytes);

            //File.WriteAllText(AbUtility.abReadonlyPath + "res_version.txt", abConfig.resVersionNumber.ToString());

            long abFileTotalSize = 0;
            int totalCount = abConfig.abConfItems.Count;
            int hadCopyCount = 0;
            bool isRun = true;
            string strMsg = "";

            object m_lock = new object();
            foreach (var itr in abConfig.abConfItems)
            {
                ThreadPool.QueueUserWorkItem((object obj) =>
                {
                    if (!isRun)
                    {
                        lock (m_lock)
                        {
                            ++hadCopyCount;
                        }
                        return;
                    }
                    AbResItem item = (AbResItem)obj;
                    FileInfo file = new FileInfo(inPath + item.filename);
                    long length = file.Length;
                    string path = AbUtility.NormalizerAbName(file.FullName);
                    string assetFileName = path.Replace(AbUtility.NormalizerAbName(new DirectoryInfo(AbUtility.AbBuildPath).FullName), "");
                    //string dir = Path.GetDirectoryName(outPath + assetFileName);
                    //Directory.CreateDirectory(dir);
                    string fileName = item.fileHashName;

                    lock (hashDict)
                    {
                        strMsg = string.Format("大小={0}", GameUtility.FormatBytes(length));
                        abFileTotalSize += length;
                        string file1;
                        if (hashDict.TryGetValue(fileName, out file1))
                        {
                            Debug.LogError("hash重复:hash=" + fileName + ",file1=" + file1 + ",file2" + assetFileName);

                            lock (m_lock)
                            {
                                ++hadCopyCount;
                            }
                            return;
                        }
                        hashDict.Add(fileName, assetFileName);
                        md5.AppendLine(assetFileName + "|" + fileName);
                    }
                    File.Copy(file.FullName, outPath + "/" + fileName);

                    lock (m_lock)
                    {
                        ++hadCopyCount;
                    }
                }, itr);
            }


            

            while(hadCopyCount < totalCount)
            {
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("复制ab文件({0}/{1})", hadCopyCount, totalCount), strMsg, (float)hadCopyCount / totalCount))
                {
                    isRun = false;
                    throw new Exception("用户停止");
                }
                //Thread.Sleep(1000);
            }

            File.Copy(inPath + "config/config.ab", outPath + "/config");

            File.WriteAllText(inPath + "md5.txt", md5.ToString());
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            Debug.Log("ab文件总大小:" + GameUtility.FormatBytes(abFileTotalSize));

        }

        public virtual string GetHash(string abFileName)
        {
            return CFileManager.GetStringMd5(abFileName.Substring(0, abFileName.Length - ABMgr.const_extension.Length));
        }

    }



    public class ABVersion
    {
        private string fileMd5;
        public uint version;

        public ABVersion(string fileMd5, uint version)
        {
            this.fileMd5 = fileMd5;
            this.version = version;
        }
    }
}



/*
文本格式
支持后缀：txt、xml；
打包后的类型：TextAsset，数据保存在TextAsset的text属性中。

二进制格式
支持后缀：bytes；
打包后的类型：TextAsset，数据保存在TextAsset的bytes属性中。


预制件格式
支持后缀：prefab；
打包后的类型：GameObject，加载后还需要调用Instantiate函数实例化才能使用。


FBX文件格式
支持后缀：fbx；
打包后的类型：添加了Animator（Mecanim动画系统）或者添加了Animation（Legacy动画系统）的GameObject，模型加载后还需要调用Instantiate函数实例化才能添加到场景，只包含动画的FBX文件动画剪辑的获取方法如下：

Mecanim动画
Mecanim中必须制作为预制件进行加载，所以加载后的人物都是配置好的，不存在需要加载Animation Clip的情况。

Legacy动画
1 private AnimationClip LoadAnimationClip(AssetBundle assetBundle, string name)
2 {
3     GameObject go = assetBundle.Load(name, typeof(GameObject)) as GameObject;
4     return go.animation.clip;
5 }


图片格式
支持后缀：bmp、jpg、png；
打包后的类型：Texture2D、Sprite。
默认Texture2D，比如使用AssetDatabase.LoadMainAssetAtPath方法加载是就是Texture2D的类型，如果希望打包后是Sprite类型（用在2D游戏上时）可以使用下面的方法加载：
AssetDatabase.LoadAssetAtPath("Assets/Image.png", typeof(Sprite));


音乐格式
支持后缀：aiff、wav、mp3、ogg；
打包后的类型：AudioClip。


ScriptableObject格式
支持后缀：asset；
打包后的类型：ScriptableObject的派生类。
 */

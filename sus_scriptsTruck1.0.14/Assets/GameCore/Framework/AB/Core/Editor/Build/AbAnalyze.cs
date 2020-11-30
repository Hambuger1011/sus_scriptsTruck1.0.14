

namespace AB
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class AbAnalyze
    {
        public class Config
        {
            public Dictionary<string, List<string>> abMap;

            public void Clear()
            {
                abMap = null;
            }
        }

        public static readonly string CONFIG_FILE = "ab_analyze.json";
        public static readonly string shader_pkg = "assets/bundle/shaders";
        public static readonly string shader_ab = "assets/bundle/shaders.ab";

        public Dictionary<string, AbPackage> file2PkgMap = new Dictionary<string, AbPackage>(StringComparer.CurrentCultureIgnoreCase);
        public Dictionary<string, AbPackage> ab2PkgMap = new Dictionary<string, AbPackage>(StringComparer.CurrentCultureIgnoreCase);
        
        AbstractAbOptions m_abOptions = null;
        AbPackage shaderBundle = null;
        StringBuilder logStr = new StringBuilder();
        public string m_output;

        public Config config { get; private set; }


        public void OnBegin(string output, AbstractAbOptions abOptions)
        {
            this.m_output = output;
            this.m_abOptions = abOptions;
        }

        public void OnEnd()
        {
            //LOG.Info(logStr);
            Clear();
        }

        void Clear()
        {
            logStr.Length = 0;
        }


        #region 扫描资源

        void AddShader()
        {
            //shaderBundle = new AbPackage();
            //shaderBundle.abType = enResType.eShader;
            //shaderBundle.SetAbFileName("assets/bundle/shaders");//输出文件名;
            //if (this.m_abOptions.PackShaders())
            //{

            //    var shaderVaraintsPath = "assets/bundle/shaders/ShaderVariants.shaderVariants";
            //    //shaderBundle.objs.Add(shaderVaraintsPath);
            //    var shaderVaraints = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(shaderVaraintsPath);
            //    var deps = GetDependencies(new[] { shaderVaraintsPath });
            //    for (int i = 0; i < deps.Length; ++i)
            //    {
            //        var assetsName = deps[i];
            //        if (!IsValidAsset(assetsName))
            //        {
            //            continue;
            //        }
            //        shaderBundle.objs.Add(assetsName);
            //    }
            //    shaderBundle.objs.Sort();
            //    config.bundles.Add(shaderBundle);
            //}
        }

        AbPackage AddShader(string file)
        {
            if (!this.m_abOptions.PackShaders())
            {
                return null;
            }
            if (file.EndsWith(".shader") || file.EndsWith(".cg"))
            {
                AbPackage pkg;
                if (!ab2PkgMap.TryGetValue(AbAnalyze.shader_pkg, out pkg))
                {
                    pkg = new AbPackage(AbAnalyze.shader_pkg);
                    ab2PkgMap.Add(AbAnalyze.shader_pkg, pkg);
                }
                pkg.isRoot = true;
                pkg.AddObject(file);
                file2PkgMap.Add(file, pkg);
                return pkg;
            }
            return null;
        }

        public AbPackage AddAsset( string file, string abName = null,bool isRoot = true)
        {
            AbPackage pkg;
            if(file2PkgMap.TryGetValue(file, out pkg))
            {
                return pkg;
            }
            pkg = AddShader(file);
            if(pkg != null)
            {
                return pkg;
            }

            if (isRoot)
            {
                if (abName == null)
                {
                    abName = file;
                }
                if (!ab2PkgMap.TryGetValue(abName, out pkg))
                {
                    pkg = new AbPackage(abName);
                    ab2PkgMap.Add(abName, pkg);
                }
            }
            else
            {
                pkg = new AbPackage(null);
            }
            pkg.isRoot = isRoot;
            pkg.AddObject(file);
            file2PkgMap.Add(file, pkg);
            return pkg;
        }

        public void Scan<T>(string path, enResType abType = enResType.eObject, Type componentType = null, string searchPattern = "*") where T : UnityEngine.Object
        {

            if (!AssetDatabase.IsValidFolder(path))
            {
                LOG.Error("路径不合法:" + path);
                return;
            }
            

            //var files = Directory.GetFiles(path, searchPattern, searchOption);
            //查找指定路径下指定类型的所有资源，返回的是资源GUID
            //string[] guids = AssetDatabase.FindAssets("t:GameObject", new string[] { "Assets/Resources/UI" });
            var fileGUIDs = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T).Name), new[] { path });
            //LOG.Info(string.Format("t:{0}", typeof(T).Name));
            int p = 0;
            foreach (string guid in fileGUIDs)
            {
                var file = AssetDatabase.GUIDToAssetPath(guid);
                ++p;
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("扫描({0}/{1})", p, fileGUIDs.Length), path, (float)p / fileGUIDs.Length))
                {
                    throw new Exception("用户停止");
                }

                if (!this.IsValidAsset(file))
                {
                    continue;
                }
                AddAsset(file);
            }
            //LOG.Info(typeof(T) + " " + fileGUIDs.Length);
        }


        /// <summary>
        /// 打成一个包
        /// </summary>
        public void ScanToSingle<T>(string path, enResType abType = enResType.ePrefab, string abName = null) where T : UnityEngine.Object
        {


            if (!AssetDatabase.IsValidFolder(path))
            {
                LOG.Error("路径不合法:" + path);
                return;
            }


            //var files = Directory.GetFiles(path, searchPattern, searchOption);
            //查找指定路径下指定类型的所有资源，返回的是资源GUID
            //string[] guids = AssetDatabase.FindAssets("t:GameObject", new string[] { "Assets/Resources/UI" });
            var fileGUIDs = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T).Name), new[] { path });
            //LOG.Info(string.Format("t:{0}", typeof(T).Name));
            int p = 0;
            foreach (string guid in fileGUIDs)
            {
                var file = AssetDatabase.GUIDToAssetPath(guid);
                ++p;
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("扫描({0}/{1})", p, fileGUIDs.Length), path, (float)p / fileGUIDs.Length))
                {
                    throw new Exception("用户停止");
                }

                if (!this.IsValidAsset(file))
                {
                    continue;
                }
                AddAsset(file, path);
            }
            //LOG.Info(typeof(T) + " " + fileGUIDs.Length);
        }
        
        #endregion



        #region  分析依赖

        /*
         *abb.assetBundleVariant设置时要调用这个函数，不然build会报错
         在资源属性窗口底部有一个选项，这个地方设置AssetBundle的名字。它会修改资源对应的.meta文件，记录这个名字。
         AssetBundle的名字固定为小写。另外，每个AssetBundle都可以设置一个Variant，其实就是一个后缀，实际AssetBundle的名字会添加这个后缀。
         如果有不同分辨率的同名资源，可以使用这个来做区分。
         */
        public static void SetVariant(string assetName, string abName, string abVariant = "")
        {
#if USE_Variant
            AssetImporter importer = AssetImporter.GetAtPath(assetName);
            if (importer != null)
            {
                importer.assetBundleName = abName;
                importer.assetBundleVariant = abVariant;
            }
#endif
        }

        public HashSet<string> GetDependencies(List<string> objs)
        {
            HashSet<string> deps = new HashSet<string>();
            var arr = AssetDatabase.GetDependencies(objs.ToArray(), true);
            HashSet<string> set = new HashSet<string>();
            for (int idx = 0; idx < arr.Length; ++idx)
            {
                var assetName = AbUtility.NormalizerAbName(arr[idx]);
                deps.Add(assetName);
            }
            foreach(var itr in objs)
            {
                deps.Remove(itr);
            }
            //var ex = Path.GetExtension(assetName);
            //if (this.m_abOptions.specialDependencyExt.Contains(ex))
            //{
            //    deps.Add(assetName);
            //}
            return deps;
        }

        public void Analyze()
        {
            try
            {

                this.config = new Config();
                this.AddShader();
                this.m_abOptions.ScanResources();
                Dictionary<string, AbPackage> rootAssets = new Dictionary<string, AbPackage>();


#region 寻找依赖
                int p = 0;
                int max = ab2PkgMap.Count;
                var pkgs = new AbPackage[file2PkgMap.Count];
                ab2PkgMap.Values.CopyTo(pkgs, 0);
                max = ab2PkgMap.Count;
                for (p = 0; p < max; ++p)
                {
                    var pkg = pkgs[p];
                    if (EditorUtility.DisplayCancelableProgressBar(string.Format("寻找依赖({0}/{1})", p, max), pkg.abFileName, (float)p / max))
                    {
                        throw new Exception("用户停止");
                    }
                    var deps = GetDependencies(pkg.Objects);
                    foreach (var assetName in deps)
                    {
                        if (!this.IsValidAsset(assetName))
                        {
                            continue;
                        }

                        AbPackage depPkg = this.AddAsset(assetName, null, false);
                        depPkg.AddParent(pkg);
                        pkg.AddChild(depPkg);
                    }
                }
                #endregion

                #region 分离资源，减少资源重复
                //解析依赖，去重复，取交集
                max = file2PkgMap.Count;
                pkgs = new AbPackage[max];
                file2PkgMap.Values.CopyTo(pkgs, 0);
                file2PkgMap = null;

                for (p=0;p<max;++p)
                {
                    var pkg = pkgs[p];
                    if (EditorUtility.DisplayCancelableProgressBar(string.Format("分析依赖({0}/{1})", p, max), pkg.abFileName, (float)p / max))
                    {
                        throw new Exception("用户停止");
                    }
                    pkg.Analyze();
                }
#endregion

#region 设置bundle Variant

                Dictionary<string, List<string>> abMap = new Dictionary<string, List<string>>(StringComparer.CurrentCultureIgnoreCase);
                HashSet<AbPackage> pkgMap = new HashSet<AbPackage>();
                for (p = 0; p < max; ++p)
                {
                    var pkg = pkgs[p];
                    if (EditorUtility.DisplayCancelableProgressBar(string.Format("设置Bundle Variant({0}/{1})", p, max), pkg.abFileName, (float)p / max))
                    {
                        throw new Exception("用户停止");
                    }
                    if (!pkgMap.Add(pkg))
                    {
                        continue;
                    }
                    if(string.IsNullOrEmpty(pkg.abFileName))
                    {
                        Debug.LogError(pkg.abFileName);
                        continue;
                    }

                    var abName = pkg.abFileName;
                    List<string> list;
                    if(!abMap.TryGetValue(abName, out list))
                    {
                        list = new List<string>();
                        abMap.Add(abName, list);
                    }
                    list.AddRange(pkg.Objects);
                    //SetVariant(pkg.assetName, key);
                    //foreach(var itr in list)
                    //{
                    //    if (file2PkgMap.ContainsKey(itr))
                    //    {
                    //        Debug.LogError(pkg.abFileName + " <=> " + file2PkgMap[itr]);
                    //        continue;
                    //    }
                    //    file2PkgMap.Add(itr, pkg);
                    //}
                }

#if USE_Variant
                var abNames = AssetDatabase.GetAllAssetBundleNames();
                foreach (var abName in abNames)
                {
                    if (abMap.ContainsKey(abName))
                    {
                    }
                    else
                    {
                        AssetDatabase.RemoveAssetBundleName(abName, true);
                    }
                }
#else
                //var abNames = AssetDatabase.GetAllAssetBundleNames();
                //foreach (var abName in abNames)
                //{
                //    AssetDatabase.RemoveAssetBundleName(abName, true);
                //}
#endif
                Debug.LogError("pkg=" + max + ",ab=" + abMap.Count);
                this.config.abMap = abMap;
#endregion
                this.SaveConfig();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        public void Check(HashSet<string> useAbFilename, HashSet<string> useAbAssets)
        {

#if SAFE_MODE
            logStr.Insert(0, "build:" + this.config.bundles.Count + System.Environment.NewLine);
            if (useAbAssets.Count != config.useDepAssets.Length)
            {
                StringBuilder sb = new StringBuilder();
#region 检测bundle
                foreach (var itr in config.useDepAssets)
                {
                    if (useAbAssets.Contains(itr))
                    {
                        continue;
                    }
                    sb.AppendLine(itr);
                }
                if (sb.Length > 0)
                {
                    LOG.Error("bundle没有[依赖]:" + sb);
                }
#endregion

#region 检查依赖
                sb.Length = 0;
                foreach (var itr in useAbAssets)
                {
                    if (config.useDepAssets.Contains(itr))
                    {
                        continue;
                    }
                    sb.AppendLine(itr);
                }
                if (sb.Length > 0)
                {
                    LOG.Error("依赖没有[bundle]:" + sb);
                }
#endregion
                throw new Exception("资源数量没有对齐:打包=" + useAbAssets.Count + ",实际" + config.useDepAssets.Length);
            }

            for (int i = 0; i < config.useDepAssets.Length; ++i)
            {
                var assetpath = config.useDepAssets[i];
                var assetname = AbUtility.NormalizerAbName(assetpath);
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("检查依赖({0}/{1})", i, config.bundles.Count, config.useDepAssets.Length), assetname, (float)i / config.useDepAssets.Length))
                {
                    throw new Exception("用户停止");
                }
                if (!useAbAssets.Contains(assetname))
                {
                    throw new Exception("依赖错误:" + assetname);
                }
            }
#endif
        }





        public bool IsValidAsset(string assetname)
        {
            if (assetname.EndsWith(".cs") || assetname.EndsWith(".js"))
            {
                return false;
            }

            if (!File.Exists(assetname) || !assetname.StartsWith("assets/", StringComparison.CurrentCultureIgnoreCase))
            {
                //logStr.AppendLine("Warning: 内置资源 = " + assetname);
                return false;
            }
            return true;
        }
#endregion


        public void SaveConfig()
        {
            if (this.config == null)
            {
                return;
            }
            var json = TinyJson.ToJson(this.config);
            string filename = this.m_output + "/" + CONFIG_FILE;
            File.WriteAllText(filename, json);
            LOG.Info("save config json:" + filename);
        }

        public void LoadConfig()
        {
            if(this.config != null)
            {
                return;
            }
            string filename = this.m_output + "/" + CONFIG_FILE;
            if(File.Exists(filename))
            {
                var json = File.ReadAllText(filename);
                this.config = TinyJson.ToObject<Config>(json);
                this.config.abMap = new Dictionary<string, List<string>>(this.config.abMap, StringComparer.CurrentCultureIgnoreCase);
            }
            else
            {
                this.Analyze();
            }
        }
    }
}

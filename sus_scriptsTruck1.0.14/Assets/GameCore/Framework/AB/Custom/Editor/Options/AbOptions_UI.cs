//#define NEW_BUILD
namespace AB
{
    using Spine.Unity;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class AbOptions_UI : AbstractAbOptions
    {

        public override string SystemName
        {
            get
            {
                return "COMMON";
            }
        }

        public override string ConfigAssetName
        {
            get
            {
                return "assets/bundle/config/ui.asset";
            }
        }

        public override bool PackShaders()
        {
            return true;
        }

        public override void OnBegin()
        {

        }

        public override void ScanResources()
        {
            var analyzer = this.builder.analyzer;

#if NEW_BUILD
            var iconPfb = ScanBookIcon();
            ScanBookRes();
#if CHANNEL_ONYX
            //ScanCatPreview();
            //ScanCatRes();
            //CatReSan();
#endif

            //主界面
            var lobbyAB = "Assets/Bundle/Lobby";
            analyzer.ScanToSingle<Object>("Assets/StoryEditor/Res", abName: lobbyAB);
            analyzer.ScanToSingle<Object>("Assets/Bundle/UI", abName: lobbyAB);
            analyzer.ScanToSingle<AudioClip>("Assets/Bundle/Music", abName: lobbyAB);
            analyzer.ScanToSingle<GameObject>("Assets/Bundle/ComEff", abName: lobbyAB);
            analyzer.ScanToSingle<Object>("Assets/Bundle/Puzzle", abName: lobbyAB);
            analyzer.AddAsset("assets/bundle/data/pb_define.txt", abName: lobbyAB);
            analyzer.AddAsset(iconPfb, abName: lobbyAB);
            analyzer.ScanToSingle<Object>("Assets/Bundle/BookPreview/icon", abName: lobbyAB);
            analyzer.ScanToSingle<TextAsset>("Assets/Bundle/Data/Common", abName: lobbyAB);//打包配置表

            //analyzer.ScanToSingle<Object>("Assets/Bundle/CatPreview", enResType.eObject);//常驻资源
            analyzer.ScanToSingle<GameObject>("assets/gamecore/bundle/debug/ui", abName: "Assets/Bundle/Debug");//Debug UI


            analyzer.Scan<TextAsset>("Assets/Bundle/Data/BookDialog");//打包书本配置表
            analyzer.Scan<Texture2D>("Assets/Bundle/BookPreview/banner");

            analyzer.AddAsset("assets/bundle/lua/lua.asset");
#else
            //analyzer.ScanToSingle<Object>("Assets/StoryEditor/Res");
            analyzer.Scan<GameObject>("Assets/Bundle/UI");// UGUI
            analyzer.ScanToSingle<Object>("Assets/Bundle/Puzzle");//常驻资源
            //analyzer.ScanToSingle<Object>("Assets/Bundle/CatPreview", enResType.eObject);//常驻资源
            analyzer.ScanToSingle<GameObject>("assets/gamecore/bundle/debug/ui");//Debug UI
            analyzer.ScanToSingle<AudioClip>("Assets/Bundle/Music");
            analyzer.ScanToSingle<GameObject>("Assets/Bundle/ComEff");


            //analyzer.AddAsset("assets/bundle/data/pb_define.txt");
            //analyzer.ScanToSingle<TextAsset>("Assets/Bundle/Data/Common");//打包配置表
            //analyzer.Scan<TextAsset>("Assets/Bundle/Data/BookDialog");//打包书本配置表
            //analyzer.Scan<Texture2D>("Assets/Bundle/BookPreview/banner");

            analyzer.AddAsset("assets/bundle/lua/lua.asset");
            //ScanBookIcon();
            //ScanBookRes();
#if CHANNEL_ONYX
            //ScanCatPreview();
            //ScanCatRes();
            //CatReSan();
#endif
#endif
            Debug.Log("asset bundle ouput:" + AbUtility.AbBuildPath);
        }


        public override void UpdateConfig(string output, AssetBundleManifest assetBundleManifest, AbResConfig editorAbConfig)
        {
            Debug.Log("PlayerSettings.bundleVersion:" + PlayerSettings.bundleVersion);
            //editorAbConfig.resVersionNumber = CommonUtil.GetServerCurrentTime();
            //File.WriteAllText(AbUtility.abReadonlyPath+"res_version.txt", editorAbConfig.resVersionNumber.ToString());
            //editorAbConfig.version = PlayerSettings.bundleVersion;
        }

        static HashSet<string> GetShaderVariantCollection()
        {
            return new HashSet<string>()
            {
                "Framework/UGUI/Image",
                "Framework/UI3D",
                "Framework/UGUI/Obj3D2UI",
                "UI/Default No-Alpha"
            };
        }





#if NEW_BUILD
        //检索book icon 内容
        string ScanBookIcon()
        {
            var savePath = "Assets/Bundle/BookPreview/";
            var data = AbBookResEditor.MakeBook("icon", "Assets/Bundle/BookPreview/icon", savePath);
            var gofileName = AbUtility.NormalizerAbName(savePath + "icon.prefab");

            //int p = 0;
            //foreach (var obj in data.objs)// AbBookRes表中的objs的所有物体
            //{
            //    ++p;
            //    if (EditorUtility.DisplayCancelableProgressBar(string.Format("扫描({0}/{1})", p, data.objs.Length), obj, (float)p / data.objs.Length))
            //    {
            //        throw new Exception("用户停止");
            //    }
            //    var fileName = AbUtility.NormalizerAbName(obj);
            //    if (!analyzer.IsValidAsset(fileName))
            //    {
            //        continue;
            //    }
            //    analyzer.AddAsset(fileName, fileName);//把这个保存好的 AbPackage 保存进list
            //}
            return gofileName;
        }


        //检索封面的内容
        void ScanCatPreview()
        {
            var analyzer = this.builder.analyzer;
            string childFolderDir = AbUtility.NormalizerDir("assets/bundle/catpreview".ToLower().Replace(Application.dataPath.ToLower(), "assets"));
            DirectoryInfo di = new DirectoryInfo(childFolderDir);//di.Name 获取目录的名称
            String pathnew = "Assets/Bundle/";
            var data = AbBookResEditor.MakeBook(di.Name, childFolderDir, Path.GetDirectoryName(childFolderDir));// 预制体生成目录
            string folderPath = di.Name;
            var gofileName = AbUtility.NormalizerAbName(childFolderDir + ".prefab");
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(gofileName);
            {
                analyzer.AddAsset(gofileName, gofileName);//把这个保存好的 AbPackage 保存进list
            }

            int p = 0;
            foreach (var obj in data.objs)
            {
                ++p;
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("扫描({0}/{1})", p, data.objs.Length), obj, (float)p / data.objs.Length))
                {
                    throw new Exception("用户停止");
                }
                var fileName = AbUtility.NormalizerAbName(obj);
                if (!analyzer.IsValidAsset(fileName))
                {
                    continue;
                }
                analyzer.AddAsset(fileName, fileName);//把这个保存好的 AbPackage 保存进list
            }
        }

        

        void ScanBookRes()
        {
            var analyzer = this.builder.analyzer;
            int bookId = 0;
            var books = Directory.GetDirectories("assets/bundle/book/", "*", SearchOption.TopDirectoryOnly);
            foreach(var bookPath in books)
            {
                string bookDir = AbUtility.NormalizerDir(bookPath.ToLower().Replace(Application.dataPath.ToLower(), "assets"));
                var fileGUIDs = UnityEditor.AssetDatabase.FindAssets(string.Format("t:{0}", typeof(Object).Name), new[] { bookDir });
                int p = 0;
                foreach (string guid in fileGUIDs)
                {
                    var file = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    ++p;
                    if (EditorUtility.DisplayCancelableProgressBar(string.Format("扫描({0}/{1})", p, fileGUIDs.Length), file, (float)p / fileGUIDs.Length))
                    {
                        throw new Exception("用户停止");
                    }
                    if (!analyzer.IsValidAsset(file))
                    {
                        continue;
                    }
                    analyzer.AddAsset(file, file);
                }           
            }
        }


        private void CatReSan()
        {
            var analyzer = this.builder.analyzer;
#if 打包文件夹
            var cats = Directory.GetDirectories("assets/bundle/CatRe/", "*", SearchOption.TopDirectoryOnly);
            foreach (var catPath in cats)
            {
                string catDir = AbUtility.NormalizerDir(catPath.ToLower().Replace(Application.dataPath.ToLower(), "assets"));
                analyzer.ScanToSingle<SkeletonDataAsset>(catDir, enResType.eScriptableObject);
                }                 
            }
#else
            analyzer.Scan<SkeletonDataAsset>("assets/bundle/CatRe");// UGUI
#endif
        }
#else
        //检索book icon 内容
        void ScanBookIcon()
        {

            var analyzer = this.builder.analyzer;
            var savePath = "Assets/Bundle/BookPreview/";
            var data = AbBookResEditor.MakeBook("icon", "Assets/Bundle/BookPreview/icon", savePath);
            var gofileName = AbUtility.NormalizerAbName(savePath + "icon.prefab");
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(gofileName);
            {
                analyzer.AddAsset(gofileName, gofileName);//把这个保存好的 AbPackage 保存进list
            }

            int p = 0;
            foreach (var obj in data.objs)// AbBookRes表中的objs的所有物体
            {
                ++p;
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("扫描({0}/{1})", p, data.objs.Length), obj, (float)p / data.objs.Length))
                {
                    throw new Exception("用户停止");
                }
                var fileName = AbUtility.NormalizerAbName(obj);
                if (!analyzer.IsValidAsset(fileName))
                {
                    continue;
                }
                analyzer.AddAsset(fileName, fileName);//把这个保存好的 AbPackage 保存进list
            }
        }


        //检索封面的内容
        void ScanCatPreview()
        {
            var analyzer = this.builder.analyzer;
            string childFolderDir = AbUtility.NormalizerDir("assets/bundle/catpreview".ToLower().Replace(Application.dataPath.ToLower(), "assets"));
            DirectoryInfo di = new DirectoryInfo(childFolderDir);//di.Name 获取目录的名称
            String pathnew = "Assets/Bundle/";
            var data = AbBookResEditor.MakeBook(di.Name, childFolderDir, Path.GetDirectoryName(childFolderDir));// 预制体生成目录
            string folderPath = di.Name;
            var gofileName = AbUtility.NormalizerAbName(childFolderDir + ".prefab");
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(gofileName);
            {
                analyzer.AddAsset(gofileName, gofileName);//把这个保存好的 AbPackage 保存进list
            }

            int p = 0;
            foreach (var obj in data.objs)
            {
                ++p;
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("扫描({0}/{1})", p, data.objs.Length), obj, (float)p / data.objs.Length))
                {
                    throw new Exception("用户停止");
                }
                var fileName = AbUtility.NormalizerAbName(obj);
                if (!analyzer.IsValidAsset(fileName))
                {
                    continue;
                }
                analyzer.AddAsset(fileName, fileName);//把这个保存好的 AbPackage 保存进list
            }
        }



        void ScanBookRes()
        {
            var analyzer = this.builder.analyzer;
            int bookId = 0;
            var books = Directory.GetDirectories("assets/bundle/book/", "*", SearchOption.TopDirectoryOnly);
            foreach (var bookPath in books)
            {
                string bookDir = AbUtility.NormalizerDir(bookPath.ToLower().Replace(Application.dataPath.ToLower(), "assets"));
                var fileGUIDs = UnityEditor.AssetDatabase.FindAssets(string.Format("t:{0}", typeof(Object).Name), new[] { bookDir });
                int p = 0;
                foreach (string guid in fileGUIDs)
                {
                    var file = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    ++p;
                    if (EditorUtility.DisplayCancelableProgressBar(string.Format("扫描({0}/{1})", p, fileGUIDs.Length), file, (float)p / fileGUIDs.Length))
                    {
                        throw new Exception("用户停止");
                    }
                    if (!analyzer.IsValidAsset(file))
                    {
                        continue;
                    }
                    analyzer.AddAsset(file, file);
                }
            }
        }


        private void CatReSan()
        {
            var analyzer = this.builder.analyzer;
#if 打包文件夹
            var cats = Directory.GetDirectories("assets/bundle/CatRe/", "*", SearchOption.TopDirectoryOnly);
            foreach (var catPath in cats)
            {
                string catDir = AbUtility.NormalizerDir(catPath.ToLower().Replace(Application.dataPath.ToLower(), "assets"));
                analyzer.ScanToSingle<SkeletonDataAsset>(catDir, enResType.eScriptableObject);
                }                 
            }
#else
            analyzer.Scan<SkeletonDataAsset>("assets/bundle/CatRe");// UGUI
#endif
        }
#endif

    }
}
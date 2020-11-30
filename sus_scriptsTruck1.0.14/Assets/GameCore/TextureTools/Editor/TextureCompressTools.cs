
//#define USE_RGBA32
/*
http://blog.csdn.net/goodai007/article/details/52679333
http://blog.csdn.net/qq_19929447/article/details/51803028
http://blog.csdn.net/u010153703/article/details/45502895
http://www.cnblogs.com/jietian331/p/5167422.html
https://blog.csdn.net/zw514159799/article/details/50282243

IOS平台：对于UI，优先使用RGB16+RGB PVRTC4，次选RGB PVRTC4+RGB PVRTC4。 
对于场景，根据精度要求使用RGB PVRTC4+RGB PVRTC4或RGBA PVRTC4，如果精度要求更低，可考虑PVRTC2
Android平台：如果想全平台通用，压缩方式没什么可选择的，对于UI和场景，最通用的方式是RGB ETC4+RGB ETC4
压缩质量：仅比较RGB这三个通道的质量，RGBA32 > RBG16 > RGBA16 > RGB PVRTC4/ETC4 > RGB PVRTC2

----------------------------------------------------------------------------------------------------------
图片占用内存计算
图像占用内存的公式是：numBytes = width * height * bitsPerPixel / 8


OpenGL ES  纹理的宽和高都要是2次幂数, 以刚才的例子来说, 假如 start.png 本身是 480x320, 但在载入内存後, 
它其实会被变成一张 512x512 的纹理, 而start.png 则由 101x131 变成 128x256, 默认情况下面RGBA32对于每一个
像素点使用４个byte来表示--１个byte（８位）代表red，另外３个byte分别代表green、blue和alpha透明通道，总共
4*8 = 32。这个就简称RGBA8888,这个颜色模式色彩最细腻，显示质量最高。但同样的，占用的内存也最大

图像宽度（width）×图像高度（height）×每一个像素的位数（bytes　per　pixel）　=　内存大小此时，如果你有
一张５１２×５１２的RGBA32图片，一个像素占用4个byte，那么将耗费:
５１２×５１２×(32/8) = ５１２×５１２×４=１MB
1024×1024×４= 4MB
1MB = 1024 KB= 1024*1024 B

Android:
RGB_ETC 
4bit: 1024 * 1024 * (4/8) = 0.5MB
ETC1把一个4x4的像素单元组压成一个64位的数据块。4x4的像素组先被水平或垂直分割成2个4x2的组，
每一半组有1个基础颜色（分别是RGB444/RGB444或RGB555/RGB333格式）、1个4位的亮度索引、8个2位
像素索引。每个像素的颜色等于基础颜色加上索引指向的亮度范围

ALPHA_8：每个像素占用1byte内存(此时图片只有alpha值，没有RGB值，一个像素占用一个字节)
RGB_565(RGB16):每个像素占用2byte内存(相对于ARGB_8888来说也能减少一半的内存开销。因此它是一个不错的选择),1024 * 1024 * (16/8) = 2MB
ARGB_4444(RGBA16):每个像素占用2byte内存(这种格式的图片，看起来质量太差，已经不推荐使用)
ARGB_8888(RGBA32):每个像素占用4byte内存,1024 * 1024 * (32/8) = 4MB

IOS:
RGB_PVRTC4 4Bit: 1024 * 1024 * (4/8) = 0.5MB
PVRTC4: Compressed format, 4 bits per pixel, ok image quality
PVRTC2: Compressed format, 2 bits per pixel, poor image quality

一般pvr格式文件的图像格式有：
RGBA8888: 32-bit texture with alpha channel, best image quality
RGBA4444: 16-bit texture with alpha channel, good image quality
RGB565: 16-bit texture without alpha channel, good image quality but no alpha (transparency)
*/


using GameCore;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using AB;
using TPImporter;



#region 菜单
public static class TextureCompressMenu
{
    public const string ATLAS_PATH = "Assets/Z_Res/Atlas";
    public const string DATA_PATH = "Assets/Resources/UI/Atlas";

    public const int compressionQuality = 50;

    static void OnEditorUpdate()
    {
        CTimerManager.Instance.Update();
    }

    public static void Begin()
    {
        BackgroundWorker.CreateInstance();
        CTimerManager.CreateInstance();
        EditorApplication.update += OnEditorUpdate;
        ImageHelper.Clear();
    }

    public static void End()
    {
        EditorApplication.update -= OnEditorUpdate;
        CTimerManager.DestroyInstance();
        BackgroundWorker.DestroyInstance();
        ImageHelper.Clear();
    }


#if TP
    /// <summary>
    /// 创建
    /// </summary>
    [MenuItem("Assets/GameTools/UITools/更新UI图集")]
    static void CreateAlphaMask()
    {
        try
        {
            ImageHelper.Clear();
            //foreach(var guid in Selection.assetGUIDs)
            //{
            //    var assetName = AssetDatabase.GUIDToAssetPath(guid);
            //    var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetName);
            //    if(!(obj is UnityEditor.DefaultAsset))
            //    {
            //        continue;
            //    }
            //    Debug.Log(obj);
            //}
            //return;
            int i = 0;
            var objs = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
            foreach (var obj in objs)
            {
                ++i;
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("更新图集({0}/{1})", i, objs.Length), obj.name, (float)i / objs.Length))
                {
                    throw new Exception("用户停止");
                }
                string texAssetPath = AssetDatabase.GetAssetPath(obj);
                if (Path.GetFileName(texAssetPath).EndsWith("_Alpha"))
                {
                    continue;
                }
#if false
            TextureAlphaMaskTools tools = new TextureAlphaMaskTools(texAssetPath);
            tools.Run();
            AssetDatabase.ImportAsset(texAssetPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
#else
                var tex2D = AssetDatabase.LoadAssetAtPath<Texture2D>(texAssetPath);
                TexturePackerImporter packer = new TexturePackerImporter();
                packer.PostprocessTexture(tex2D);

                string assetDir = Path.GetDirectoryName(texAssetPath);
                if (texAssetPath.StartsWith(ATLAS_PATH, StringComparison.CurrentCultureIgnoreCase))
                {
                    assetDir = DATA_PATH;//AbUtility.NormalizerDir(assetDir).Replace(ATLAS_PATH, DATA_PATH);
                }
                Directory.CreateDirectory(assetDir);
                string strFile = MakeAtlsPrefab(texAssetPath, AbUtility.NormalizerAbName(assetDir));
#endif
            }
        }
        finally
        {
            ImageHelper.Clear();
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
    }

    static string MakeAtlsPrefab(string file, string outpath)
    {
        var fileName = AbUtility.NormalizerAbName(file);
        var atlasName = Path.GetFileNameWithoutExtension(file);
        var extension = Path.GetExtension(file);
        //string sptPath = AssetDatabase.GetAssetPath(spt);

        //分离alpha

        var tools = TextureCompressTools.Process(file);

        //AssetDatabase.ImportAsset(file, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);

        //保存到prefab
        //string texMd5 = CFileManager.GetFileMd5(file + ".meta");

        string prefabPath = string.Format("{0}/{1}.prefab", outpath, atlasName);
        Directory.CreateDirectory(Path.GetDirectoryName(prefabPath));
#if UNITY_2018_3_OR_NEWER
        GameObject go = new GameObject();
        go.AddComponent<CAtlasData>();
#else
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (go == null)
        {
            GameObject tmpGo = new GameObject();
            tmpGo.AddComponent<AbAtlas>();
            go = PrefabUtility.CreatePrefab(prefabPath, tmpGo);
            GameObject.DestroyImmediate(tmpGo);
            AssetDatabase.Refresh();
            go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        }
#endif

        var data = go.GetComponent<AbAtlas>();
        data.spriteSheet.Clear();
        data._mainTex = AssetDatabase.LoadAssetAtPath<Texture2D>(tools.mainTexAssetName);
        data._alphaTex = AssetDatabase.LoadAssetAtPath<Texture2D>(tools.alphaTexAssetName);
        data.specialMaterial = AssetDatabase.LoadAssetAtPath<Material>(tools.matAssetName);

        //if (!data.md5.Equals(texMd5))
        {
            Debug.Log("更新图集:" + file);
            //data.md5 = texMd5;
            Sprite[] sprites = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(file).OfType<Sprite>().ToArray();
            foreach (var spt in sprites)
            {
                data.spriteSheet.Add(spt);
            }

#if UNITY_2018_3_OR_NEWER
            {
                PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
                GameObject.DestroyImmediate(go);
            }
#else
            EditorUtility.SetDirty(go);
#endif
            AssetDatabase.Refresh();
        }
        return AbUtility.NormalizerAbName(prefabPath);
    }
#endif





    //[MenuItem("GameTools/UITools/更新图集库", false, MenuPriority.UITools + 1000)]
    //public static void UpdateAtlas()
    //{
    //    try
    //    {
    //        Stopwatch sw = new Stopwatch();
    //        sw.Start();
    //        TextureCompressMenu.Begin();
    //        TextureCompressMenu.UpdateAtlas(ATLAS_PATH);
    //        TextureCompressMenu.End();
    //        sw.Stop();
    //        UnityEngine.Debug.LogError("更新图集库耗时:" + (sw.ElapsedMilliseconds / 1000.0f) + "s");
    //    }
    //    finally
    //    {
    //        EditorUtility.ClearProgressBar();
    //        AssetDatabase.Refresh();
    //    }
    //}





    //[MenuItem("GameTools/UITools/更新所有UI Prefab", false, MenuPriority.UITools + 1010)]
    //public static void RestoreAllAtlas()
    //{
    //    try
    //    {
    //        var fileGUIDs = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(GameObject).Name), new[] { "Assets/Bundle/Prefabs/UI" });
    //        Debug.Log(fileGUIDs.Length);
    //        int p = 0;
    //        foreach (var guid in fileGUIDs)
    //        {
    //            var file = AssetDatabase.GUIDToAssetPath(guid);
    //            ++p;
    //            EditorUtility.DisplayProgressBar(string.Format("SetDependance({0}/{1})", p, fileGUIDs.Length), file, (float)p / fileGUIDs.Length);
    //            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(file);
    //            RestoreAtlas(obj);
    //        }
    //    }
    //    finally
    //    {
    //        EditorUtility.ClearProgressBar();
    //        AssetDatabase.SaveAssets();
    //        AssetDatabase.Refresh();
    //    }
    //}

#if TP
    [MenuItem("GameObject/UITools/更新选中UI", priority = 0)]
    [MenuItem("GameTools/UITools/更新选中UI", false, MenuPriority.UITools + 2000)]
    [MenuItem("Assets/GameTools/UITools/更新选中UI", false, MenuPriority.UITools + 300)]
    public static void RestoreSingleAtlas()
    {
        try
        {
            GameObject obj = Selection.activeGameObject;
            RestoreAtlas(obj);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    //static void RemoveArialFont(GameObject obj)
    //{
    //    if (obj != null)
    //    {
    //        bool bDirty = false;
    //        Text[] texts = obj.GetComponentsInChildren<Text>(true);
    //        foreach (var text in texts)
    //        {
    //            if (text.font == null)
    //            {
    //                continue;
    //            }
    //            Font font = text.font;

    //            string strFontName = font.name;
    //            //if (strFontName != "MSYH" && strFontName != "MSYH_Bold")
    //            if (strFontName == "Arial")
    //            {
    //                Font newFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Z_Res/MSYH_Font/MSYH.TTF");
    //                text.font = newFont;
    //                bDirty = true;
    //            }
    //        }
    //        if (bDirty)
    //        {
    //            EditorUtility.SetDirty(obj);
    //        }
    //    }
    //}
    static void RestoreAtlas(GameObject obj)
    {

        if (obj != null)
        {
            Image[] imgs = obj.GetComponentsInChildren<Image>(true);
            foreach (var img in imgs)
            {
                if (img.sprite == null)
                {
                    continue;
                }
                string path = AbUtility.NormalizerAbName(AssetDatabase.GetAssetPath(img.sprite));
                if (!File.Exists(path))
                {
                    continue;
                }

                //AtlasHelper helper = img.GetComponent<AtlasHelper>();
                //if (helper == null)
                //{
                //    helper = img.gameObject.AddComponent<AtlasHelper>();
                //    helper.atlasName = path.Replace("assets/z_res/", "assets/bundle/") + ".prefab";
                //    helper.spriteName = img.sprite.name;
                //}

                AbAtlas data = AssetDatabase.LoadAssetAtPath<AbAtlas>(path.Replace("assets/z_res/", "assets/bundle/") + ".prefab");
                if (data != null)
                {
                    //img.sprite = data[helper.spriteName];
                    img.material = data.specialMaterial;
                    if (img.sprite == null)
                    {
                        LOG.Error(string.Format("{0}-{1}", obj.name, img.name));
                    }
                }

                //AtlasHelper helper = img.GetComponent<AtlasHelper>();
                //if (helper != null)
                //{
                //    Object.DestroyImmediate(helper,true);
                //}
            }
            EditorUtility.SetDirty(obj);
        }
    }
#endif


#if true//TP图集


    /// <summary>
    /// 图集制作
    /// </summary>
    /// <param name="path"></param>
    //public static void UpdateAtlas(string path)
    //{

    //    if (!AssetDatabase.IsValidFolder(path))
    //    {
    //        Debug.LogError("路径不合法:" + path);
    //        return;
    //    }

    //    HashSet<string> atlasSet = new HashSet<string>();
    //    var fileGUIDs = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(GameObject).Name), new[] { DATA_PATH });
    //    foreach (string guid in fileGUIDs)
    //    {
    //        var file = AbUtility.NormalizerAbName(AssetDatabase.GUIDToAssetPath(guid));
    //        atlasSet.Add(file);
    //    }

    //    fileGUIDs = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(Texture2D).Name), new[] { path });
    //    Debug.Log(string.Format("t:{0}", typeof(Texture2D).Name));
    //    int p = 0;

    //    foreach (string guid in fileGUIDs)
    //    {
    //        var file = AssetDatabase.GUIDToAssetPath(guid);
    //        ++p;
    //        //string fileName = AbUtility.NormalizerDir(file.ToLower().Replace(Application.dataPath.ToLower(), "assets"));
    //        EditorUtility.DisplayProgressBar(string.Format("多线程扫描{0}({1}/{2})", path, p, fileGUIDs.Length), file, (float)p / fileGUIDs.Length);
    //        if (Path.GetFileNameWithoutExtension(file).EndsWith("_Alpha"))
    //        {
    //            continue;
    //        }
    //        BackgroundWorker.Instance.AddBackgroudOperation(() =>
    //        {
    //            int nTexSize = 100;
    //            Color32[] pixels = null;
    //            TextureCompressTools.GetPixels(file, out nTexSize, ref pixels);//多线程缓冲图片像素
    //        });
    //    }
    //    p = 0;
    //    foreach (string guid in fileGUIDs)
    //    {
    //        var file = AssetDatabase.GUIDToAssetPath(guid);
    //        ++p;
    //        //string fileName = AbUtility.NormalizerDir(file.ToLower().Replace(Application.dataPath.ToLower(), "assets"));
    //        if (EditorUtility.DisplayCancelableProgressBar(string.Format("扫描{0}({1}/{2})", path, p, fileGUIDs.Length), file, (float)p / fileGUIDs.Length))
    //        {
    //            break;
    //        }
    //        if (Path.GetFileNameWithoutExtension(file).EndsWith("_Alpha"))
    //        {
    //            continue;
    //        }
    //        string assetDir = AbUtility.NormalizerDir(Path.GetDirectoryName(file)).Replace(ATLAS_PATH, DATA_PATH);
    //        Directory.CreateDirectory(assetDir);
    //        string strFile = MakeAtlsPrefab(file, AbUtility.NormalizerAbName(assetDir));
    //        atlasSet.Remove(strFile);
    //    }

    //    foreach (var strFile in atlasSet)
    //    {
    //        AssetDatabase.DeleteAsset(strFile);
    //    }

    //    AssetDatabase.SaveAssets();
    //    AssetDatabase.Refresh();
    //}

#endif
    //[MenuItem("GameTools/UITools/压缩场景纹理", false, MenuPriority.UITools + 3000)]
    //public static void SceneTexture2D_RGB_Compress()
    //{
    //    CommonTexture2D_Compress("Assets/Z_Res/Art/Sc_maps");
    //}

    //[MenuItem("GameTools/UITools/压缩模型纹理", false, MenuPriority.UITools + 3000)]
    //public static void ModelTexture2D_RGB_Compress()
    //{
    //    CommonTexture2D_Compress("Assets/Z_Res/Models");
    //}

    //[MenuItem("GameTools/UITools/压缩特效图片", false, MenuPriority.UITools + 4000)]
    //public static void Effect_Texture2D_Compress()
    //{
    //    CommonTexture2D_Compress("Assets/Z_Res/Effect");
    //}


    [MenuItem("Assets/GameTools/去掉图片空格", false, MenuPriority.Utils + 3001)]
    public static void Texture2D_Name_Trim()
    {
        try
        {
            ImageHelper.Clear();
            int p = 0;
            var objs = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
            foreach (var obj in objs)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ++p;
                //string fileName = AbUtility.NormalizerDir(file.ToLower().Replace(Application.dataPath.ToLower(), "assets"));
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("压缩{0}({1}/{2})", path, p, objs.Length), path, (float)p / objs.Length))
                {
                    return;
                }
                var name = obj.name;
                var realName = name.Trim();
                if (name != realName)
                {
                    var dir = Path.GetDirectoryName(path) + "/";
                    var ex = Path.GetExtension(path);
                    var newFile = dir + realName + ex;
                    if (File.Exists(newFile))
                    {
                        AssetDatabase.DeleteAsset(path);
                        continue;
                    }
                    realName = AssetDatabase.RenameAsset(path, realName + ex);
                    if (!string.IsNullOrEmpty(realName))
                    {
                        Debug.LogError(realName);
                    }
                }
            }
        }
        finally
        {
            ImageHelper.Clear();
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            //AssetDatabase.SaveAssets();
        }
    }

    #region 纹理压缩

    [MenuItem("Assets/GameTools/压缩纹理", false, MenuPriority.Utils + 3001)]
    public static void Texture2D_Select_Compress()
    {
        try
        {
            ImageHelper.Clear();
            int p = 0;
            var objs = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
            foreach (var obj in objs)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ++p;
                //string fileName = AbUtility.NormalizerDir(file.ToLower().Replace(Application.dataPath.ToLower(), "assets"));
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("压缩{0}({1}/{2})", path, p, objs.Length), path, (float)p / objs.Length))
                {
                    return;
                }

                if (path.Contains("/Role/"))
                {
                    //CommonSingleTexture2D_Compress(path, 2048, clearTag: true);
                }
                else if (path.Contains("/SceneBG/") || path.Contains("/Cover/"))
                {
                    CommonSingleTexture2D_Compress(path, 1024, true, clearTag: true);
                }
                else
                {

                    CommonSingleTexture2D_Compress(path, 512, clearTag: true);
                }
            }
        }
        finally
        {
            ImageHelper.Clear();
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            //AssetDatabase.SaveAssets();
        }
    }

    [MenuItem("Assets/GameTools/压缩图集", false, MenuPriority.Utils + 3002)]
    static void Texture2D_Select_Atlas()
    {
        try
        {
            ImageHelper.Clear();
            int p = 0;
            var objs = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
            foreach (var obj in objs)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ++p;
                //string fileName = AbUtility.NormalizerDir(file.ToLower().Replace(Application.dataPath.ToLower(), "assets"));
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("压缩{0}({1}/{2})", path, p, objs.Length), path, (float)p / objs.Length))
                {
                    return;
                }
                CommonSingleTexture2D_Compress(path);
            }
        }
        finally
        {
            ImageHelper.Clear();
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            //AssetDatabase.SaveAssets();
        }
    }
    static void CommonTexture2D_Compress(string path)
    {
        try
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                Debug.LogError("路径不合法:" + path);
                return;
            }

            var fileGUIDs = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(Texture2D).Name), new[] { path });
            Debug.Log(string.Format("t:{0}", typeof(Texture2D).Name));
            int p = 0;
            foreach (string guid in fileGUIDs)
            {
                var file = AssetDatabase.GUIDToAssetPath(guid);
                ++p;
                //string fileName = AbUtility.NormalizerDir(file.ToLower().Replace(Application.dataPath.ToLower(), "assets"));
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("扫描{0}({1}/{2})", path, p, fileGUIDs.Length), file, (float)p / fileGUIDs.Length))
                {
                    return;
                }
                if (Path.GetFileNameWithoutExtension(file).EndsWith("_Alpha"))
                {
                    continue;
                }

                //compress file
                CommonSingleTexture2D_Compress(file);
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    public static void CommonSingleTexture2D_Compress(string file, int maxSize = 1024, bool ignoreAlpha = false, bool clearTag = false)
    {
        #region 压缩

        try
        {
            TextureImporter importer = AssetImporter.GetAtPath(file) as TextureImporter;
            if (importer == null)
            {
                return;
            }
            int nTexSize;
            bool hasAlpha = ImageHelper.CheckPngAlpha(file, out nTexSize);
            if (nTexSize > maxSize)
            {
                nTexSize = maxSize;
            }
            if (ignoreAlpha)
            {
                hasAlpha = false;
            }
            TextureImporterSettings settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.textureType = TextureImporterType.Sprite;
            settings.readable = false;//关闭读写操作
            settings.alphaIsTransparency = true;
            settings.mipmapEnabled = false;
            importer.SetTextureSettings(settings);
            if (clearTag)
            {
                importer.spritePackingTag = string.Empty;
            }

            //=================pc
            {
                var setting = importer.GetPlatformTextureSettings("Standalone");
                setting.overridden = true;
                setting.maxTextureSize = nTexSize;
                if (hasAlpha)
                {
                    //setting.format = TextureImporterFormat.DXT5Crunched;
                    setting.format = TextureImporterFormat.RGBA32;
                }
                else
                {
                    //setting.format = TextureImporterFormat.DXT1Crunched;
                    setting.format = TextureImporterFormat.DXT1;
                }
                setting.allowsAlphaSplitting = false;
                setting.compressionQuality = TextureCompressMenu.compressionQuality;
                importer.SetPlatformTextureSettings(setting);
            }

            //=================android
            {
                var setting = importer.GetPlatformTextureSettings("Android");
                setting.overridden = true;
                setting.maxTextureSize = nTexSize;
                if (hasAlpha)
                {
                    setting.format = TextureImporterFormat.RGBA32;
                }
                else
                {
                    setting.format = TextureImporterFormat.ETC_RGB4;
                }
                setting.allowsAlphaSplitting = false;
                setting.compressionQuality = TextureCompressMenu.compressionQuality;
                importer.SetPlatformTextureSettings(setting);
            }

            //=================ios
            {
                var setting = importer.GetPlatformTextureSettings("iPhone");
                setting.overridden = true;
                setting.maxTextureSize = nTexSize;
                if (hasAlpha)
                {
                    setting.format = TextureImporterFormat.RGBA32;
                }
                else
                {
#if UNITY_2018_1_OR_NEWER
                    setting.format = TextureImporterFormat.PVRTC_RGB4;
#else
                    setting.format = TextureImporterFormat.PVRTC_RGB4;
#endif
                }
                setting.allowsAlphaSplitting = false;
                setting.compressionQuality = TextureCompressMenu.compressionQuality;
                importer.SetPlatformTextureSettings(setting);
            }

            AssetDatabase.WriteImportSettingsIfDirty(file);
            AssetDatabase.ImportAsset(file, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
            //AssetDatabase.Refresh();

            //Texture2D tex2D = AssetDatabase.LoadAssetAtPath<Texture2D>(file);
            //if (tex2D.width != tex2D.height)
            //{
            //    Debug.Log("非方形纹理:" + file);
            //}
        }
        catch (System.Exception ex)
        {
            Debug.LogError(file + "\n" + ex);
        }
        #endregion
    }

    #endregion



    [MenuItem("Assets/UITools/字体替换", false, MenuPriority.UITools + 1010)]
    public static void ReplaceFont()
    {
        try
        {
            var arial = AssetDatabase.LoadAssetAtPath<Font>("Assets/Plugins/Z_Share/GameCore/Z_Res/Fonts/Arial.ttf");
            var titleFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Z_Res/Font/FZCSJW.TTF");
            int p = 0;
            var objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets);
            foreach (GameObject obj in objs)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ++p;
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("扫描{0}({1}/{2})", path, p, objs.Length), path, (float)p / objs.Length))
                {
                    return;
                }
                var texts = obj.GetComponentsInChildren<Text>(true);
                bool isDirty = false;
                foreach (var t in texts)
                {
                    var f = t.font;
                    if (f == null)
                    {
                        t.font = titleFont;
                        isDirty = true;
                        continue;
                    }
                    string file = AssetDatabase.GetAssetPath(f);
                    if (!File.Exists(file))
                    {

                        t.font = titleFont;
                        isDirty = true;
                    }
                    if (f.name == "1")
                    {

                        t.font = arial;
                        isDirty = true;
                    }
                    else if (f.name == "2")
                    {

                        t.font = arial;
                        isDirty = true;
                    }
                }
                if (isDirty)
                {
                    EditorUtility.SetDirty(obj);
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endregion



#region class
#if TP
public class TextureCompressTools
{
    public static void CheckSpritesAlpha(string strPngFileName, out int nTexSize, ref Color32[] pixels, out bool enalbeAlpha, System.Action<Sprite, Color32[]> onSpriteProcess = null)
    {
        enalbeAlpha = false;
        var targetTexture2d = AssetDatabase.LoadAssetAtPath<Texture2D>(strPngFileName);
        ImageHelper.GetPixels(strPngFileName, out nTexSize, ref pixels);
        Sprite[] sprites = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(strPngFileName).OfType<Sprite>().ToArray();

        //扫描所有的sprite是否有alpha
        foreach (var spt in sprites)
        {
            //ui.uvTL = spt.uv[0];//0,1
            //ui.uvTR = spt.uv[1];//1,1
            //ui.uvBL = spt.uv[2];//0,0
            //ui.uvBR = spt.uv[3];//1,0

            int w = (int)spt.rect.size.x;
            int h = (int)spt.rect.size.y;

            int offsetx = (int)spt.rect.xMin;//(int)(spt.uv[2].x * targetTexture2d.width);
            int offsety = (int)spt.rect.yMax;//(int)(spt.uv[2].y * targetTexture2d.height);
                                             //Debug.Log(spt.name + " " + offsetx + " " + offsety);

            int minAlpha = int.MaxValue;
            Color32[] _pixels = new Color32[w * h];
            Color32 tmp = new Color32();
            for (int i = 0; i < w; ++i)
            {
                for (int j = 0; j < h; ++j)
                {
                    int idx = i + w * (h - j - 1);
                    int offsetIdx = (offsetx + i) + targetTexture2d.width * (offsety - j - 1);
                    _pixels[idx] = tmp = pixels[offsetIdx];
                    if (tmp.a < minAlpha)
                    {
                        minAlpha = tmp.a;
                    }
                    if (tmp.a < ImageHelper.MIN_ALHPA)
                    {
                        enalbeAlpha = true;
                        if (onSpriteProcess == null)
                        {
                            return;
                        }
                    }
                }
            }
            //Debug.LogError("minAlpha = " + minAlpha);
            if (onSpriteProcess != null)
            {
                onSpriteProcess(spt, _pixels);
            }
        }
    }


    public abstract class CompressBase
    {
        public string matAssetName;
        public string mainTexAssetName;
        public string alphaTexAssetName;

        public CompressBase(string file)
        {
            var prefix = file.Substring(0, file.LastIndexOf("."));
            this.mainTexAssetName = file;
            this.matAssetName = prefix + ".mat";
            this.alphaTexAssetName = prefix + "_Alpha.png";
        }

        public abstract void Run();
        public virtual void CreateMaterial(string matAssetName, string mainTexAssetName, string alphaTexAssetName)
        {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(matAssetName);
            if (mat == null)
            {
                mat = new Material(AbAtlas.shader);
                AssetDatabase.CreateAsset(mat, matAssetName);
                //AssetDatabase.SaveAssets();
            }
            Texture2D baseTex = AssetDatabase.LoadAssetAtPath<Texture2D>(mainTexAssetName);
            Texture2D alphaTex = AssetDatabase.LoadAssetAtPath<Texture2D>(alphaTexAssetName);

            bool isDrity = false;
            var _MainTex = mat.GetTexture("_MainTex");
            var _AlphaTex = mat.GetTexture("_AlphaTex");
            var _UseAlpha = mat.GetFloat("_UseAlpha");
            if (_MainTex != baseTex)
            {
                isDrity = true;
                mat.SetTexture("_MainTex", baseTex);
            }
            if (_AlphaTex != alphaTex)
            {
                isDrity = true;
                mat.SetTexture("_AlphaTex", alphaTex);
            }

            var useAlphaFlag = alphaTex ? 1 : 0;
            if (_UseAlpha != useAlphaFlag)
            {
                isDrity = true;
                mat.SetFloat("_UseAlpha", useAlphaFlag);
                if(useAlphaFlag > 0)
                {
                    mat.EnableKeyword("UI_ALPHA");
                }
                else
                {
                    mat.DisableKeyword("UI_ALPHA");
                }
            }
            if (isDrity)
            {
                EditorUtility.SetDirty(mat);
            }
        }

    }

    public class CompressETC1 : CompressBase
    {
        bool enalbeAlpha = false;
        int nTexSize;
        private string file;

        public CompressETC1(string file) : base(file)
        {
        }
        public override void Run()
        {
            CreateAlphaMask();
            CreateMaterial(matAssetName, mainTexAssetName, alphaTexAssetName);
            CompressTexture();
        }



        /// <summary>
        /// 创建alpha图
        /// </summary>
        public void CreateAlphaMask()
        {
            Color32[] pixels = null;
            TextureCompressTools.CheckSpritesAlpha(mainTexAssetName, out nTexSize, ref pixels, out enalbeAlpha);
            Texture2D baseTex = AssetDatabase.LoadAssetAtPath(mainTexAssetName, typeof(Texture2D)) as Texture2D;

            if (!enalbeAlpha)
            {
                Debug.Log("没有alpha通道:" + mainTexAssetName);
                AssetDatabase.DeleteAsset(this.alphaTexAssetName);
                AssetDatabase.DeleteAsset(this.matAssetName);
                this.alphaTexAssetName = null;
                //this.matAssetName = null;
                return;
            }

            bool bException = true;
            Color32 newColor = new Color32();
            for (int i = 0; i < pixels.Length; ++i)
            {
                newColor.r = pixels[i].a;
                newColor.g = 0;
                newColor.b = 0;
                //if (newColor.r < 1.0f - 0.001f)
                //{
                //    //enalbeAlpha = true;
                //}
                pixels[i] = newColor;
                if (newColor.r != 0)
                {
                    bException = false;
                }
            }

            if (bException)
            {
                Debug.LogError("读取图片像素失败:" + mainTexAssetName);
                AssetDatabase.DeleteAsset(this.alphaTexAssetName);
                AssetDatabase.DeleteAsset(this.matAssetName);
                this.alphaTexAssetName = null;
                this.matAssetName = null;
                return;
            }
            //Texture2D rgbTex = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
            Texture2D alphaTex = new Texture2D(baseTex.width, baseTex.height, TextureFormat.ARGB32, false);
            try
            {
                alphaTex.SetPixels32(pixels);
                alphaTex.Apply();
            }
            catch (Exception ex)
            {
                Debug.LogError("创建alpha失败:" + alphaTexAssetName + " " + baseTex.width + "*" + baseTex.height + ",len=" + pixels.Length + "\n" + ex);
            }

            byte[] bytes = alphaTex.EncodeToPNG();
            File.WriteAllBytes(this.alphaTexAssetName, bytes);
            bytes = null;
            //Object.DestroyImmediate(alphaTex);
            alphaTex = null;
            AssetDatabase.Refresh();

        }




        /// <summary>
        /// 压缩纹理
        /// </summary>
        public void CompressTexture()
        {
            TextureImporterFormat[] formats = new TextureImporterFormat[3]
           {
#if USE_RGBA32
                TextureImporterFormat.RGBA32,
                TextureImporterFormat.RGBA32,
                TextureImporterFormat.RGBA32
#else
                TextureImporterFormat.RGB16,
                TextureImporterFormat.RGB16,
                TextureImporterFormat.RGB16
#endif
           };
            string[] paths = { mainTexAssetName, alphaTexAssetName };
            foreach (var path in paths)
            {
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }
                bool isAlphaMask = path.EndsWith("_Alpha.png");

                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null)
                {
                    continue;
                }
                importer.spritePackingTag = "";//禁用自带图集打包
                TextureImporterSettings settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);

                //-----------------------normal/-----------------------
                settings.textureType = TextureImporterType.Sprite;//图集
                settings.spriteMode = (int)SpriteImportMode.Multiple;//多个sprite
                if (importer.spritesheet.Length <= 1)
                {
                    //None = 0, Single = 1, Manual = 2.
                    settings.spriteMode = (int)SpriteImportMode.Single;
                }
                else
                {
                    settings.spriteMode = (int)SpriteImportMode.Multiple;
                }
                settings.textureShape = TextureImporterShape.Texture2D;
                //settings.spritePixelsPerUnit = 100;//世界坐标的一个单位代表图片的多少个像素点
                settings.spriteMeshType = SpriteMeshType.Tight;//确定生成Mesh时，使用Full Rect直接一个矩形，还是Tight根据透明通道配置
                                                               //settings.spriteExtrude = 1;//在Mesh中会留下多少的透明度,设为1,这样就可以避免黑线问题

                ///-----------------------advanced/-----------------------
                settings.sRGBTexture = true;//非HDR的图片应该勾选该选项
                settings.alphaSource = TextureImporterAlphaSource.None;//Alpha通道值的来源。None，不需要。Input Texture Alpha，由图片的Alpha通道指定。From Gray Scale，RGB的平均值；
                settings.alphaIsTransparency = false;//Alpha是否代表透明通道

                settings.readable = false;//关闭读写操作,会消耗两倍的内存
                settings.mipmapEnabled = false;//关闭mipmap

                settings.wrapMode = TextureWrapMode.Clamp;//拉伸
                settings.filterMode = FilterMode.Bilinear;//纹理在近距离变模糊,Trilinear用于mipmap
                settings.aniso = 0;//各向异性级别,用于地板与地面纹理的
                settings.npotScale = TextureImporterNPOTScale.None;//保持非二次方纹理

                importer.SetTextureSettings(settings);

                //=================pc
                {
                    var setting = importer.GetPlatformTextureSettings("Standalone");
                    setting.overridden = true;
                    setting.maxTextureSize = nTexSize;
                    setting.format = formats[0];// TextureImporterFormat.DXT1Crunched;
                    setting.allowsAlphaSplitting = false;
                    setting.compressionQuality = TextureCompressMenu.compressionQuality;
                    importer.SetPlatformTextureSettings(setting);
                }

                //=================android
                {
                    var setting = importer.GetPlatformTextureSettings("Android");
                    setting.overridden = true;
                    setting.maxTextureSize = nTexSize;
                    setting.format = formats[1];// TextureImporterFormat.ETC_RGB4;
                    setting.allowsAlphaSplitting = false;
                    setting.compressionQuality = TextureCompressMenu.compressionQuality;
                    importer.SetPlatformTextureSettings(setting);
                }

                //=================ios
                {
                    var setting = importer.GetPlatformTextureSettings("iPhone");
                    setting.overridden = true;
                    setting.maxTextureSize = nTexSize;
                    setting.format = formats[2];// TextureImporterFormat.PVRTC_RGB4;
                    setting.allowsAlphaSplitting = false;
                    setting.compressionQuality = TextureCompressMenu.compressionQuality;
                    importer.SetPlatformTextureSettings(setting);
                }


                AssetDatabase.WriteImportSettingsIfDirty(path);
                //AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                //AssetDatabase.Refresh();
            }
        }
    }



    public class CompressETC2 : CompressBase
    {
        bool enalbeAlpha = false;
        int nTexSize;
        private string file;

        public CompressETC2(string file) : base(file)
        {
            this.alphaTexAssetName = null;
        }
        public override void Run()
        {
            CreateMaterial(matAssetName, mainTexAssetName, alphaTexAssetName);
            CompressTexture();
        }

        /// <summary>
        /// 压缩纹理
        /// </summary>
        public void CompressTexture()
        {
            Color32[] pixels = null;
            TextureCompressTools.CheckSpritesAlpha(mainTexAssetName, out nTexSize, ref pixels, out enalbeAlpha);
            TextureImporterFormat[] formats = new TextureImporterFormat[3]
           {
                TextureImporterFormat.DXT5,
                TextureImporterFormat.ETC2_RGBA8,
                TextureImporterFormat.ETC2_RGBA8
           };
            TextureImporterFormat[] noAlphaFormats = new TextureImporterFormat[3]
{
                TextureImporterFormat.DXT1,
                TextureImporterFormat.ETC2_RGB4,
                TextureImporterFormat.ETC2_RGB4
};
            string[] paths = { mainTexAssetName };
            foreach (var path in paths)
            {
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null)
                {
                    continue;
                }
                importer.spritePackingTag = "";//禁用自带图集打包
                TextureImporterSettings settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);

                //-----------------------normal/-----------------------
                settings.textureType = TextureImporterType.Sprite;//图集
                settings.spriteMode = (int)SpriteImportMode.Multiple;//多个sprite
                settings.spriteMode = (int)SpriteImportMode.Multiple;
                settings.textureShape = TextureImporterShape.Texture2D;
                //settings.spritePixelsPerUnit = 100;//世界坐标的一个单位代表图片的多少个像素点
                settings.spriteMeshType = SpriteMeshType.Tight;//确定生成Mesh时，使用Full Rect直接一个矩形，还是Tight根据透明通道配置
                settings.spriteExtrude = 1;//在Mesh中会留下多少的透明度,设为1,这样就可以避免黑线问题

                ///-----------------------advanced/-----------------------
                settings.sRGBTexture = true;//非HDR的图片应该勾选该选项
                settings.alphaSource = TextureImporterAlphaSource.FromInput;//Alpha通道值的来源。None，不需要。Input Texture Alpha，由图片的Alpha通道指定。From Gray Scale，RGB的平均值；
                settings.alphaIsTransparency = true;//Alpha是否代表透明通道

                settings.readable = false;//关闭读写操作,会消耗两倍的内存
                settings.mipmapEnabled = false;//关闭mipmap

                settings.wrapMode = TextureWrapMode.Clamp;//拉伸
                settings.filterMode = FilterMode.Bilinear;//纹理在近距离变模糊,Trilinear用于mipmap
                settings.aniso = 0;//各向异性级别,用于地板与地面纹理的
                settings.npotScale = TextureImporterNPOTScale.None;//保持非二次方纹理

                importer.SetTextureSettings(settings);

                //=================pc
                {
                    var setting = importer.GetPlatformTextureSettings("Standalone");
                    setting.overridden = true;
                    setting.maxTextureSize = nTexSize;
                    setting.format = enalbeAlpha ? formats[0] : noAlphaFormats[0];// TextureImporterFormat.DXT1Crunched;
                    setting.allowsAlphaSplitting = false;
                    setting.compressionQuality = TextureCompressMenu.compressionQuality;
                    importer.SetPlatformTextureSettings(setting);
                }

                //=================android
                {
                    var setting = importer.GetPlatformTextureSettings("Android");
                    setting.overridden = true;
                    setting.maxTextureSize = nTexSize;
                    setting.format = enalbeAlpha ? formats[1] : noAlphaFormats[1];// TextureImporterFormat.ETC_RGB4;
                    setting.allowsAlphaSplitting = false;
                    setting.compressionQuality = TextureCompressMenu.compressionQuality;
                    importer.SetPlatformTextureSettings(setting);
                }

                //=================ios
                {
                    var setting = importer.GetPlatformTextureSettings("iPhone");
                    setting.overridden = true;
                    setting.maxTextureSize = nTexSize;
                    setting.format = enalbeAlpha ? formats[2] : noAlphaFormats[2];// TextureImporterFormat.PVRTC_RGB4;
                    setting.allowsAlphaSplitting = false;
                    setting.compressionQuality = TextureCompressMenu.compressionQuality;
                    importer.SetPlatformTextureSettings(setting);
                }


                AssetDatabase.WriteImportSettingsIfDirty(path);
                //AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                //AssetDatabase.Refresh();
            }
        }
    }

    public static CompressBase Process(string file)
    {
        CompressBase c = new CompressETC2(file);
        c.Run();
        return c;
    }
}
#endif
#endregion

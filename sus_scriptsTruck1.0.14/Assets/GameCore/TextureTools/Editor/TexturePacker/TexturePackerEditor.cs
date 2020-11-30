using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using TPImporter;
using System.Reflection;
using System;

public class TexturePackerEditor : AssetPostprocessor
{
    /// <summary>
    /// 创建
    /// </summary>
    [MenuItem("Assets/GameTools/UITools/导入tpsheet")]
    static void CreateAtlas()
    {
        try
        {
            Texture2D tex2D = Selection.activeObject as Texture2D;
            TexturePackerImporter packer = new TexturePackerImporter();
            packer.PostprocessTexture(tex2D);
        }
        finally
        {
            //AssetDatabase.SaveAssets();
        }
    }
    //    [MenuItem("GameTools/TexturePacker/设置工程路径", false, MenuPriority.TexturePacker + 100)]
    //    static void SettingProjectPath()
    //    {
    //        IdentifyEncoding ifed = new IdentifyEncoding();
    //        var files = Directory.GetFiles("Assets/Z_Res/UIPack/Project", "*.tps", SearchOption.AllDirectories);
    //        int p = 0;
    //        foreach (var file in files)
    //        {
    //            ++p;
    //            Encoding encodeType = EncodingType.GetType(file);
    //            if (encodeType == null)
    //            {
    //                string encodingName = ifed.GetEncodingName(file);
    //                encodeType = Encoding.GetEncoding(encodingName);
    //                //LOG.Warn("文件编码:" + encodingName +" "+file);
    //                //continue;
    //            }
    //            if (encodeType != Encoding.UTF8)
    //            {
    //                //LOG.Info(encodeType+" "+file);
    //                //continue;
    //                CodeModify.ConvertToUTF8(file, encodeType);
    //            }

    //            EditorUtility.DisplayProgressBar("设置工程路径", string.Format("{0}({1}/{2})", file, p, files.Length), (float)p / files.Length);
    //            string content = System.Text.Encoding.UTF8.GetString(File.ReadAllBytes(file));
    //            content = content.Replace("<key type=\"filename\">../../UIResources/", "<key type=\"filename\">../../../../Z_Temp/UIResources/");
    //            content = content.Replace("<filename>../../UIResources/", "<filename>../../../../Z_Temp/UIResources/");
    //            File.WriteAllBytes(file, System.Text.Encoding.UTF8.GetBytes(content));
    //        }
    //        EditorUtility.ClearProgressBar();
    //    }

    //    //[MenuItem("GameTools/TexturePacker/Process to Sprites", false, MenuPriority.TexturePacker + 200)]
    //    //static void ProcessToSprite()
    //    //{
    //    //    var files = Directory.GetFiles(@"Assets\Z_Res\UIPack\Atlas", "*.txt", SearchOption.AllDirectories);
    //    //    foreach (var file in files)
    //    //    {
    //    //        string assetPath = AB.AbUtility.NormalizerAbName(file).Replace(Application.dataPath, "assets/");
    //    //        string content = File.ReadAllText(assetPath);
    //    //        string rootPath = Path.GetDirectoryName(assetPath);
    //    //        string fileName = Path.GetFileNameWithoutExtension(assetPath);

    //    //        TexturePacker.MetaData meta = TexturePacker.GetMetaData(content);

    //    //        List<SpriteMetaData> sprites = TexturePacker.ProcessToSprites(content);
    //    //        for (int i = 0; i < sprites.Count; ++i)
    //    //        {
    //    //            var spt = sprites[i];
    //    //            spt.name = Path.GetFileNameWithoutExtension(spt.name);
    //    //            sprites[i] = spt;
    //    //        }

    //    //        string path = string.Format("{0}/{1}.png", rootPath, fileName);
    //    //        TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter;
    //    //        texImp.spritesheet = sprites.ToArray();
    //    //        texImp.textureType = TextureImporterType.Sprite;
    //    //        texImp.spriteImportMode = SpriteImportMode.Multiple;

    //    //        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    //    //    }

    //    //}


    //    //纹理导入之前调用，针对入到的纹理进行设置  
    //    public void OnPreprocessTexture()
    //    {
    //        //Debug.Log("纹理导入之前调用=" + this.assetPath);

    //    }
    //    public void OnPostprocessTexture(Texture2D tex)
    //    {
    //        //Debug.Log("OnPostProcessTexture=" + this.assetPath);
    //        //ProcessToSprite();
    //    }


    //    //所有的资源的导入，删除，移动，都会调用此方法，注意，这个方法是static的  
    //    public static void OnPostprocessAllAssets(string[] importedAsset, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    //    {
    //        //Debug.Log("OnPostprocessAllAssets");
    //        //foreach (string str in importedAsset)
    //        //{
    //        //    Debug.Log("importedAsset = " + str);
    //        //}
    //        //foreach (string str in deletedAssets)
    //        //{
    //        //    Debug.Log("deletedAssets = " + str);
    //        //}
    //        //foreach (string str in movedAssets)
    //        //{
    //        //    Debug.Log("movedAssets = " + str);
    //        //}
    //        //foreach (string str in movedFromAssetPaths)
    //        //{
    //        //    Debug.Log("movedFromAssetPaths = " + str);
    //        //}
    //    }



    //    [MenuItem("GameTools/TexturePacker/批量导出图集", false, MenuPriority.TexturePacker + 300)]
    //    static void ProcessToProject()
    //    {
    //        Process genProc = new Process();
    //        ProcessStartInfo startInfo = genProc.StartInfo;//new ProcessStartInfo(Application.dataPath.Replace("/Assets", "/") + ".tools/protobuf-net/protogen/protogen.exe");
    //        //genProc.StartInfo = startInfo;
    //        startInfo.FileName = @"C:\Program Files\CodeAndWeb\TexturePacker\bin\TexturePacker.exe";
    //        startInfo.CreateNoWindow = true;//不创建窗口
    //        startInfo.UseShellExecute = false;//不使用系统外壳程序启动
    //        startInfo.RedirectStandardOutput = true;//重定向输出
    //        startInfo.RedirectStandardError = true;//重定向输出

    //        var dirs = Directory.GetDirectories(System.Environment.CurrentDirectory + "/Z_Temp/UIResources", "*", SearchOption.TopDirectoryOnly);
    //        int p = 0;
    //        foreach (var d in dirs)
    //        {
    //            ++p;
    //            EditorUtility.DisplayProgressBar("批量导出图集", string.Format("{0}({1}/{2})", d, p, dirs.Length), (float)p / dirs.Length);
    //            ProcessToProject(genProc, d);
    //        }
    //        EditorUtility.ClearProgressBar();
    //        AssetDatabase.Refresh();
    //    }

    //    //[HelpURL("https://www.codeandweb.com/texturepacker/documentation")]
    //    static void ProcessToProject(Process genProc, string path)
    //    {
    //        ProcessStartInfo startInfo = genProc.StartInfo;
    //        string dirName = Path.GetFileName(path);
    //        //var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
    //        //int fileNum = files.Length;
    //        //float p = 0;

    //        //string imgFiles = "";
    //        //foreach (var file in files)
    //        //{
    //        //    ++p;
    //        //    string fileName = Path.GetFileNameWithoutExtension(file);

    //        //    if (EditorUtility.DisplayCancelableProgressBar("ProcessToProject", fileName, p / fileNum))
    //        //    {
    //        //        break;
    //        //    }
    //        //    imgFiles += " "+fileName;
    //        //}
    //        //TexturePacker --format "json" --data "sheets/%%i.json" --sheet "sheets/%%i.png" "sprites/%%i"
    //        string genCmd = string.Format("{0} --format unity-texture2d --texture-format {1} --data  c://{2}.json --trim-sprite-names --multipack",
    //                                        path, //图集文件夹
    //                                        "png", //输出图集格式
    //                                        dirName//输出图集名
    //                                        );
    //        startInfo.Arguments = genCmd;
    //        if (!genProc.Start())
    //        {
    //            LOG.Error("启动失败:" + startInfo.FileName);
    //        }

    //        //genProc.WaitForExit();



    //        var result = genProc.StandardOutput.ReadToEnd();
    //        var errMsg = genProc.StandardError.ReadToEnd();
    //        //File.WriteAllText(logPath, result);
    //        LOG.Info("done");
    //        LOG.Info(result);
    //        if (!string.IsNullOrEmpty(errMsg))
    //        {
    //            UnityEngine.Debug.LogError(errMsg);
    //        }

    //    }
}
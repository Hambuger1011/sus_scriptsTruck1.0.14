using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SpineTextureImporter : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        //1 判断是它是不spine动画的图片
        // 判断依据是同目录下存在同名的后缀为“*.skel.bytes”或“*.atlas.txt”的文件
        //Debug.LogError(assetPath);
        string fullPath = Path.Combine(Application.dataPath, assetPath.Substring("Assets/".Length));
        //Debug.LogError(fullPath);
        string dir = Path.GetDirectoryName(fullPath);
        //Debug.LogError(dir);
        string nameWithExtension = Path.GetFileNameWithoutExtension(fullPath);
        //Debug.LogError(nameWithExtension);
        if(!File.Exists($"{dir}/{nameWithExtension}.skel.bytes") && !File.Exists($"{dir}/{nameWithExtension}.atlas.txt"))
        {
            return;
        }

        // 2 修改导入设置
        TextureImporter importer = (TextureImporter)assetImporter;
        TextureImporterPlatformSettings iosSetting = importer.GetPlatformTextureSettings("iPhone"); 
        iosSetting.overridden = true;
        iosSetting.maxTextureSize = 1024;
        iosSetting.format = TextureImporterFormat.ETC2_RGBA8;
        importer.SetPlatformTextureSettings(iosSetting);
    }
}

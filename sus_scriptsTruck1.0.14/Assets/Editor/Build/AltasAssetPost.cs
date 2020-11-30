using Framework;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

//http://www.xuanyusong.com/archives/3315
public class AltasAssetPost : AssetPostprocessor
{
    void OnPostprocessTexture(Texture2D texture)
    {
        //string AtlasName = new DirectoryInfo(Path.GetDirectoryName(assetPath)).Name;
        //TextureImporter textureImporter = assetImporter as TextureImporter;
        //textureImporter.textureType = TextureImporterType.Sprite;
        //textureImporter.spritePackingTag = AtlasName;
        //textureImporter.mipmapEnabled = false;
    }
}

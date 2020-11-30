using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public class MenuTools
{
    private static string UIAtlasSpritesPath = Application.dataPath + "/Resources/UI/UIAltasSprites/";

    [MenuItem("Tools/AtlasMaker")]
    private static void MakeUGUIAtlas()
    {
        if (!Directory.Exists(UIAtlasSpritesPath))
        {
            Directory.CreateDirectory(UIAtlasSpritesPath);
        }
        DirectoryInfo rootDirInfo = new DirectoryInfo(Application.dataPath + "/UIAtlsaCache/");
        foreach (DirectoryInfo dirInfo in rootDirInfo.GetDirectories())
        {
            string path = dirInfo.FullName.Substring(dirInfo.FullName.IndexOf("Assets"));
            string filePath = path.Split('\\')[2];
            filePath = UIAtlasSpritesPath + filePath;
            foreach (FileInfo pngFile in dirInfo.GetFiles("*.png", SearchOption.AllDirectories))
            {
                string allPath = pngFile.FullName;
                allPath = allPath.Substring(allPath.IndexOf("UIAtlsaCache"));
                string[] subAllPath = allPath.Split('\\');
                string directoryPath = "";
                for (int i = 1; i < subAllPath.Length - 1; i++)
                {
                    directoryPath += subAllPath[i] + '/';
                }
                string directoryPathFull = UIAtlasSpritesPath + directoryPath;
                if (!Directory.Exists(directoryPathFull))
                {
                    Directory.CreateDirectory(directoryPathFull);
                }
                string assetPath = pngFile.FullName.Substring(pngFile.FullName.IndexOf("Assets"));
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                if (sprite == null) { Debug.LogWarning("assetPath is Not Sprite!!!"); continue; }
                string saveAssetPath = directoryPathFull.Substring(directoryPathFull.IndexOf("Assets")) + sprite.name + ".prefab";
                GameObject go = new GameObject(sprite.name);
                go.AddComponent<SpriteRenderer>().sprite = sprite;
                PrefabUtility.CreatePrefab(saveAssetPath, go);
                GameObject.DestroyImmediate(go);
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/SceneSwitch/LaunghMainScene", false, 100)]
    private static void SceneSwitch()
    {
        EditorSceneManager.OpenScene("Assets/Scene/test.unity", OpenSceneMode.Single);
    }
}
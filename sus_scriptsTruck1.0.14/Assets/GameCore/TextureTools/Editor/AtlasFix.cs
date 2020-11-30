using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using GameCore;

public static class AtlasFix
{
    [MenuItem("GameTools/UITools/TexturePackerGUI", false, MenuPriority.UITools + 1100)]
    static void OpenGUI()
    {
        Application.OpenURL(System.Environment.CurrentDirectory + @"\Z_Work\Atlas\UITools\TexturePacker\bin\TexturePackerGUI.exe");
    }


    #if false
    [MenuItem("GameTools/UITools/图集替换散图", false, MenuPriority.UITools + 1000)]
    static void AtlasFix1()
    {
        try
        {
            var fileGUIDs = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(GameObject).Name), new[] { "Assets/Bundle/Prefabs/UGUI"});
            Debug.Log(fileGUIDs.Length);
            int p = 0;
            foreach (var guid in fileGUIDs)
            {
                var file = AssetDatabase.GUIDToAssetPath(guid);
                ++p;
                EditorUtility.DisplayProgressBar(string.Format("扫描UI({0}/{1})", p, fileGUIDs.Length), file, (float)p / fileGUIDs.Length);
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(file);
                var imgs = obj.GetComponentsInChildren<Image>(true);
                bool isDirty = false;
                foreach(var img in imgs)
                {
                    var spt = img.sprite;
                    var filename = AssetDatabase.GetAssetPath(spt);
                    if(string.IsNullOrEmpty(filename))
                    {
                        continue;
                    }
                    var fi = new FileInfo(filename);
                    var di = fi.Directory;

                    string atlasName = "";
                    if(filename.StartsWith("Assets/Z_Res/Texture/UI/"))
                    {
                        var tmp = filename.Replace("Assets/Z_Res/Texture/UI/",string.Empty);
                        int idx = tmp.IndexOf('/');
                        if(idx < 0)
                        {
                            Debug.LogError(filename);
                            continue;
                        }
                        atlasName = tmp.Substring(0,idx);
                    }else
                    {
                        continue;
                    }
                    for(int i=1; i< 10; ++i)
                    {
                        CAtlasData atlasData;
                        if(i == 1)
                        {
                            atlasData = AssetDatabase.LoadAssetAtPath<CAtlasData>("assets/bundle/atlas/ui/"+atlasName+".png.prefab");
                        }else
                        {
                            atlasData = AssetDatabase.LoadAssetAtPath<CAtlasData>("assets/bundle/atlas/ui/"+atlasName+i+".png.prefab");
                        }
                        if(atlasData == null)
                        {
                            continue;
                        }
                        spt = atlasData[fi.Name];
                        if(spt != null)
                        {
                            Debug.Log(di.Name+"|"+fi.Name);
                            img.sprite = spt;
                            img.material = atlasData.material;
                            isDirty = true;
                            break;
                        }
                    }
                }
                if(isDirty)
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
    #endif
}

using AB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using UnityEditor.iOS.Xcode.Custom;
using UnityEditor.iOS.Xcode.Custom.Extensions;
using System.Diagnostics;

public class ShaderRecord
{
    [Serializable]
    public class Atlas
    {
        public string path;
        public string spt;
    }


    [Serializable]
    public class Data
    {
        public string assetName;
        public List<Atlas> nodes = new List<Atlas>();
    }

    [MenuItem("GameTools/Shader/记录", false, 200)]
    static void Write()
    {
        try
        {
            List<Data> list = new List<Data>();
            var fileGUIDs = AssetDatabase.FindAssets("t:GameObject", new string[] { "Assets" });
            int p = 0;
            foreach (string guid in fileGUIDs)
            {
                var file = AssetDatabase.GUIDToAssetPath(guid);
                ++p;
                string fileName = AbUtility.NormalizerDir(file.ToLower().Replace(Application.dataPath.ToLower(), "assets"));
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("扫描{0}({1}/{2})", file, p, fileGUIDs.Length), fileName, (float)p / fileGUIDs.Length))
                {
                    throw new Exception("用户停止");
                }

                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(fileName);
                if (obj == null)
                {
                    continue;
                }

                Data data = new Data();
                data.assetName = fileName;
                var imgs = obj.GetComponentsInChildren<Text>(true);
                foreach (var img in imgs)
                {
                    var spt = img.font;
                    var path = AssetDatabase.GetAssetPath(spt);
                    if (!File.Exists(path))
                    {
                        continue;
                    }
                    if (!path.StartsWith("Assets/Z_Res"))
                    {
                        continue;
                    }
                    var atlas = new Atlas();
                    data.nodes.Add(atlas);
                    atlas.path = GetNode(obj.transform, img.transform);
                    //atlas.atlasName = Path.GetFileName(Path.GetDirectoryName(path));
                    atlas.spt = Path.GetFileName(path);
                }
                if (data.nodes.Count > 0)
                {
                    list.Add(data);
                }
            }//end for
            var json = JsonHelper.ObjectToJson(list);
            File.WriteAllText("shader.json", json);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    public static string GetNode(Transform root, Transform node)
    {
        if (root == node)
        {
            return "";
        }
        string path = node.name;
        node = node.parent;
        while (node != root)
        {
            path = node.name + "/" + path;
            node = node.parent;
        }
        return path;
    }

    [MenuItem("GameTools/Shader/恢复", false, 201)]
    static void Read()
    {
        try
        {
            var json = File.ReadAllText("shader.json");
            List<Data> list = JsonHelper.JsonToObject<List<Data>>(json);
            int p = 0;
            foreach (var itr in list)
            {
                ++p;
                string fileName = itr.assetName;
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("扫描{0}({1}/{2})", fileName, p, list.Count), fileName, (float)p / list.Count))
                {
                    throw new Exception("用户停止");
                }
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(fileName);
                if (obj == null)
                {
                    continue;
                }
                var tf = obj.transform;
                foreach(var node in itr.nodes)
                {
                    var ch = tf.Find(node.path);
                    if(ch == null)
                    {
                        LOG.Error("找不到节点"+node.path);
                        continue;
                    }
                    var uiImg = ch.GetComponent<Text>();
                    var atlas = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/"+node.spt);
                    if(atlas == null)
                    {
                        LOG.Error("找不到图集:" + node.spt);
                        continue;
                    }
                    uiImg.font = atlas;
                    //LOG.Info(uiImg.sprite);
                }
                EditorUtility.SetDirty(obj);
            }//end for
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
        }
    }
    

    

}

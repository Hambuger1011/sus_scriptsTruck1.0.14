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
using FreeImageAPI;
using System.Linq;

public class TpGen
{


    [MenuItem("GameTools/Atlas/生成工程", false, 100)]
    static void GenProject()
    {
        var dirs = Directory.GetDirectories("Z_Work/Atlas/UIProject/", "*.*", SearchOption.TopDirectoryOnly);
        foreach (var dir in dirs)
        {
            //var dataNode = doc.DocumentElement;
            var di = new DirectoryInfo(dir);
            LOG.Info(di.FullName);
            var imgs = di.GetFiles("*.png", SearchOption.AllDirectories);

            XmlDocument doc = new XmlDocument();
            doc.Load("Assets/Editor/TpGen/template.tps");
            //LOG.Error(doc.DocumentElement.Name);
            var dataNode = doc.DocumentElement;
            var structNode = dataNode.SelectSingleNode("struct");
            var childNodes = structNode.ChildNodes;
            for (int i = 0, iMax = childNodes.Count; i < iMax; i++)
            {
                var keyNode = childNodes[i];
                #region tps工程路径
                if (keyNode.InnerText == "fileName")
                {
                    var valueNode = childNodes[i + 1];
                    //LOG.Error(valueNode.InnerText);
                    valueNode.InnerText = di.FullName + "/" + di.Name + ".tps";
                }
                #endregion
                #region png输出路径
                else if (keyNode.InnerText == "textureFileName")
                {
                    var valueNode = childNodes[i + 1];
                    //LOG.Error(valueNode.InnerText);
                    valueNode.InnerText = "../../../../Assets/Z_Res/Atlas/" + di.Name + ".png";
                }
                #endregion
                #region tpsheet输出路径
                else if (keyNode.Name == "map")
                {
                    if (keyNode.Attributes["type"].Value == "GFileNameMap")
                    {
                        var valueNode = keyNode.SelectSingleNode("struct/filename");
                        //LOG.Error(valueNode.InnerText);
                        valueNode.InnerText = "../../../../Assets/Z_Res/Atlas/" + di.Name + ".tpsheet";
                    }
                }
                #endregion
                #region sprite列表
                else if (keyNode.InnerText == "fileList")
                {
                    var fileNode = childNodes[i + 1];
                    //LOG.Error("files:" + fileNode.ChildNodes.Count);
                    var list = new List<XmlNode>();
                    foreach (XmlNode ch in fileNode.ChildNodes)
                    {
                        list.Add(ch);
                    }
                    foreach (XmlNode ch in list)
                    {
                        fileNode.RemoveChild(ch);
                    }

                    foreach (var img in imgs)
                    {
                        var newNode = doc.CreateNode(XmlNodeType.Element, "filename", null);
                        newNode.InnerText = img.Name;
                        fileNode.AppendChild(newNode);
                    }
                }
                #endregion
                #region sprite设置
                else if (keyNode.InnerText == "individualSpriteSettings")
                {
                    var fileNode = childNodes[i + 1];
                    //LOG.Error("files:" + fileNode.ChildNodes.Count);
                    var list = new List<XmlNode>();
                    foreach (XmlNode ch in fileNode.ChildNodes)
                    {
                        list.Add(ch);
                    }
                    foreach (XmlNode ch in list)
                    {
                        fileNode.RemoveChild(ch);
                    }

                    foreach (var img in imgs)
                    {
                        FIBITMAP fib = FreeImage.LoadEx(img.FullName);
                        var width = (int)FreeImage.GetWidth(fib);
                        var height = (int)FreeImage.GetHeight(fib);

                        var newNode = doc.CreateNode(XmlNodeType.Element, "key", null);
                        var attr = doc.CreateAttribute("type");
                        attr.Value = "filename";
                        newNode.Attributes.Append(attr);
                        newNode.InnerText = img.Name;
                        fileNode.AppendChild(newNode);

                        bool scale9Enabled = false;
                        //L,T,W,H
                        Vector4 scale9Borders = new Vector4(0, 0, width, height);
                        Vector2 pivotPoint = new Vector2(0.5f, 0.5f);
                        var lines = File.ReadAllLines(img.FullName + ".meta");
                        foreach (var line in lines)
                        {
                            if (line.Contains("spriteBorder"))
                            {
                                //L,B,R,T
                                var vec = JsonHelper.JsonToObject<Vector4>(line.Substring(line.IndexOf(":") + 1));
                                scale9Borders = new Vector4(vec.x,vec.w,0,0);
                                scale9Borders.z = width - scale9Borders.x - vec.z;
                                scale9Borders.w = height - scale9Borders.y - vec.y;
                                if (scale9Borders.magnitude > 0)
                                {
                                    scale9Enabled = true;
                                }
                            }
                            else if (line.Contains("spritePivot"))
                            {
                                pivotPoint = JsonHelper.JsonToObject<Vector2>(line.Substring(line.IndexOf(":") + 1));
                            }
                        }
                        fileNode.AppendChild(GenStructNode(doc, scale9Enabled, scale9Borders, pivotPoint));
                    }
                }
                #endregion
            }
            using (var fs = new FileStream(di.FullName + "/" + di.Name + ".tps", FileMode.Create, FileAccess.ReadWrite))
            {
                fs.Seek(0, SeekOrigin.Begin);
                var utf8 = new System.Text.UTF8Encoding(false);
                TextWriter tw = new StreamWriter(fs, utf8);
                doc.Save(tw);
            }
            //break;
        }
        LOG.Info("Finish!!!");
    }

    [MenuItem("GameTools/Atlas/导出图集", false, 101)]
    static void PushProject()
    {
        string exe = @"Z_Work\Atlas\UITools\TexturePacker\bin\TexturePacker.exe";
        var dirs = Directory.GetDirectories("Z_Work/Atlas/UIProject/", "*.*", SearchOption.TopDirectoryOnly);
        foreach (var dir in dirs)
        {
            //var dataNode = doc.DocumentElement;
            var di = new DirectoryInfo(dir);
            var tps = di.FullName + "/" + di.Name + ".tps";
            Process.Start(exe, tps);
        }
        LOG.Info("Finish!!!");
    }


    [MenuItem("GameTools/Atlas/替换图集", false, 101)]
    static void SetAtlas()
    {

        try
        {
            Dictionary<string, Dictionary<string, Sprite>> atlas = new Dictionary<string, Dictionary<string, Sprite>>();
            var fileGUIDs = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(Texture2D).Name), new[] { @"Assets/Z_Res/Atlas" });
            int p = 0;
            foreach (string guid in fileGUIDs)
            {
                var file = AssetDatabase.GUIDToAssetPath(guid);
                ++p;

                var fileName = Path.GetFileNameWithoutExtension(file);
                if (EditorUtility.DisplayCancelableProgressBar(
                    string.Format("扫描({0}/{1})",
                                    p,
                                    fileGUIDs.Length
                                    ),
                    fileName,
                    (float)p / fileGUIDs.Length)
                    )
                {
                    throw new Exception("用户停止");
                }
                var spts = AssetDatabase.LoadAllAssetsAtPath(file).OfType<Sprite>();
                Dictionary<string, Sprite> map;
                if(!atlas.TryGetValue(fileName,out map))
                {
                    map = new Dictionary<string, Sprite>();
                    atlas.Add(fileName, map);
                }
                foreach(Sprite s in spts)
                {
                    map[s.name] = s;
                }
            }

            fileGUIDs = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(GameObject).Name), new[] { "Assets" });
            LOG.Error("count=" + fileGUIDs.Length);
            p = 0;
            foreach (string guid in fileGUIDs)
            {
                var file = AssetDatabase.GUIDToAssetPath(guid);
                ++p;

                var fileName = Path.GetFileName(file);
                if (EditorUtility.DisplayCancelableProgressBar(
                    string.Format("扫描({0}/{1})",
                                    p,
                                    fileGUIDs.Length
                                    ),
                    fileName,
                    (float)p / fileGUIDs.Length)
                    )
                {
                    throw new Exception("用户停止");
                }

                bool isDirty = false;
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(file);
                var uiImgs = go.GetComponentsInChildren<Image>(true);
                foreach(var img in uiImgs)
                {
                    if(img.sprite == null)
                    {
                        continue;
                    }

                    var f = AssetDatabase.GetAssetPath(img.sprite.texture);
                    var n = Path.GetFileName(Path.GetDirectoryName(f));
                    Dictionary<string, Sprite> map;
                    if (atlas.TryGetValue(n, out map))
                    {
                        img.sprite = map[img.sprite.name];
                        isDirty = true;
                    }
                }
                if (isDirty)
                {
                    PrefabUtility.SavePrefabAsset(go);
                }

            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
    }
    [Serializable]
    public class Atlas
    {
        public string path;
        public string atlasName;
        public string spt;
    }


    [Serializable]
    public class Data
    {
        public string assetName;
        public List<Atlas> nodes = new List<Atlas>();
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

    
    static XmlNode GenStructNode(XmlDocument doc, bool scale9Enabled, Vector4 scale9Borders, Vector2 pivotPoint)
    {
        //L,T,W,H
        var scale9BorderValue = scale9Borders.x + "," + scale9Borders.y + "," + scale9Borders.z + "," + scale9Borders.w;
        var root = doc.CreateNode(XmlNodeType.Element, "struct", null);
        var attr = doc.CreateAttribute("type");
        attr.Value = "IndividualSpriteSettings";
        root.Attributes.Append(attr);

        //pivotPoint
        var node = doc.CreateNode(XmlNodeType.Element, "key", null);
        node.InnerText = "pivotPoint";
        root.AppendChild(node);
        node = doc.CreateNode(XmlNodeType.Element, "point_f", null);
        node.InnerText = pivotPoint.x + "," + pivotPoint.y;
        root.AppendChild(node);

        //scale9
        node = doc.CreateNode(XmlNodeType.Element, "key", null);
        node.InnerText = "scale9Enabled";
        root.AppendChild(node);
        node = doc.CreateNode(XmlNodeType.Element, scale9Enabled?"true":"false", null);
        root.AppendChild(node);

        //scale9Borders
        node = doc.CreateNode(XmlNodeType.Element, "key", null);
        node.InnerText = "scale9Borders";
        root.AppendChild(node);
        node = doc.CreateNode(XmlNodeType.Element, "rect", null);
        node.InnerText = scale9BorderValue;
        root.AppendChild(node);


        //scale9Paddings
        node = doc.CreateNode(XmlNodeType.Element, "key", null);
        node.InnerText = "scale9Paddings";
        root.AppendChild(node);
        node = doc.CreateNode(XmlNodeType.Element, "rect", null);
        node.InnerText = scale9BorderValue;
        root.AppendChild(node);

        //scale9FromFile
        node = doc.CreateNode(XmlNodeType.Element, "key", null);
        node.InnerText = "scale9FromFile";
        root.AppendChild(node);
        node = doc.CreateNode(XmlNodeType.Element, "false", null);
        root.AppendChild(node);
        return root;
    }



}

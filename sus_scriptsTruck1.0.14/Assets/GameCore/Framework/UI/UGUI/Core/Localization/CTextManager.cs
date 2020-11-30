/*
 * 多语言
 */

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
#endif

using Framework;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using System.Reflection;
using System.Collections;
using UGUI;
using pb;

public class CTextManager : Framework.CSingleton<CTextManager>
{
	private const string c_localTextPath = "Text/LocalText";

	private DictionaryView<int, t_Localization> m_textMap = new DictionaryView<int, t_Localization>();

    protected override void Init()
    {
        base.Init();
        this.LoadBuiltin();
    }

    public void LoadBuiltin()
    {
        m_textMap.Clear();
        var asset = Resources.Load<TextAsset>("t_Localization");
        var list = XlsxData.Deserialize<List<pb.t_Localization>>(asset.bytes);
        foreach (var data in list)
        {
            m_textMap.Add(data.id, data);
        }
    }

    public void LoadAssetBundle()
    {
        m_textMap.Clear();
        var list = XlsxData.Deserialize<List<pb.t_Localization>>(string.Concat(XlsxData.CONF_DIR, "t_Localization.bytes"));
        foreach(var data in list)
        {
            m_textMap.Add(data.id, data);
        }
    }

	public bool IsTextLoaded()
	{
		return m_textMap.Count > 0;
	}
    

	public string GetText(int key)
	{
        string text = null;
        t_Localization data;
		if(m_textMap.TryGetValue(key, out data))
        {
            text = data.text;
        }
		if (string.IsNullOrEmpty(text))
		{
			text = "";
		}
		return text;
	}

	public string GetText(int key, params object[] args)
	{
		string text = CSingleton<CTextManager>.GetInstance().GetText(key);
		if (text == null)
		{
			return "text with tag [" + key + "] was not found!";
		}
		return string.Format(text, args);
	}

#if UNITY_EDITOR
    public class TextData
    {
        public int id;
        public string text;
        public string path;

        public TextData(int key, string text, string path)
        {
            this.id = key;
            this.text = text;
            this.path = path;
        }
    }

    static string GetPath(Transform t,Transform p)
    {
        string s = t.name;
        Transform q = t.parent;
        while(q != null)
        {
            s = q.name + "/" + s;
            q = q.parent;
        }
        return p.name+"/"+ s;
    }



    [MenuItem("Tools/导入多语言", false, 101)]
    static void Export2Excel()
    {
        var table = EPPlusHelper.LoadExcel("APP_OUT/Localization.xlsx");
        var sheet = table.Tables[0];
        //for(int i=1;i<= sheet.NumberOfRows; ++i)
        //{
        //    Debug.Log(sheet.GetCell(i, 1).Value);
        //}

        var json = File.ReadAllText("APP_OUT/text.json");
        Dictionary<string, TextData> map = JsonHelper.JsonToObject<Dictionary<string, TextData>>(json);
        SortedDictionary<int, TextData> dict = new SortedDictionary<int, TextData>();
        foreach(var itr in map)
        {
            dict.Add(itr.Value.id, itr.Value);
        }


        int row = 277;
        foreach(var itr in dict)
        {
            var data = itr.Value;
            sheet.SetValue(row, 1,data.id.ToString());
            sheet.SetValue(row, 2, "");
            sheet.SetValue(row, 3, data.text);
            sheet.SetValue(row, 4, data.path);
            ++row;
        }
        EPPlusHelper.WriteExcel(table, "APP_OUT/Localization.xlsx");
    }

    [MenuItem("Tools/多语言处理", false, 101)]
    static void SetAtlas()
    {

        try
        {
            int maxID = 273;
            Dictionary<string, TextData> map = new Dictionary<string, TextData>();
            var fileGUIDs = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(GameObject).Name), new[] { "Assets" });
            //LOG.Error("count=" + fileGUIDs.Length);
            var p = 0;

            Action<bool> func = (flag) =>
            {
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
                    if (flag)
                    {
                        go = PrefabUtility.InstantiatePrefab(go) as GameObject;
                    }
                    var uiTexts = go.GetComponentsInChildren<Text>(true);
                    foreach (var txt in uiTexts)
                    {
                        if (txt.font == null)
                        {
                            continue;
                        }
                        var str = txt.text;
                        float t;
                        if (float.TryParse(str, out t))
                        {
                            continue;
                        }
                        if (string.IsNullOrEmpty(str))
                        {
                            continue;
                        }
                        if (!flag && map.ContainsKey(str))
                        {
                            continue;
                        }
                        var c = txt.GetComponent<CUILocalization>();
                        if (c != null)
                        {
                            isDirty = true;
                            GameObject.DestroyImmediate(c);
                            continue;
                        }
                        if (!flag && (c == null || c.m_key == 0))
                        {
                            continue;
                        }
                        if (flag)
                        {
                            isDirty = true;
                            if (c == null)
                            {
                                c = txt.gameObject.AddComponent<CUILocalization>();
                            }
                            if (map.ContainsKey(str))
                            {
                                c.m_key = map[str].id;
                            }
                            else
                            {
                                var id = ++maxID;
                                map[str] = new TextData(id, str, GetPath(txt.transform, go.transform));
                                c.m_key = id;
                            }
                        }
                        else
                        {
                            if(c.m_key < 0)
                            {
                                continue;
                            }
                            maxID = Math.Max(maxID, c.m_key);
                            map[str] = new TextData(c.m_key, str, GetPath(txt.transform, go.transform));
                        }
                    }
                    if (flag)
                    {
                        if (isDirty)
                        {

                            //EditorUtility.SetDirty(go);
                            bool isSuccess;
                            PrefabUtility.SaveAsPrefabAsset(go, file, out isSuccess);
                            if (!isSuccess)
                            {
                                Debug.Log(file);
                            }
                            var prefabStage = PrefabStageUtility.GetPrefabStage(go);
                            if (prefabStage != null)
                            {
                                EditorSceneManager.MarkSceneDirty(prefabStage.scene);
                            }
                        }
                        GameObject.DestroyImmediate(go);
                    }

                }
            };

            //func(false);
            func(true);


            var json = JsonHelper.ObjectToJson(map);
            File.WriteAllText("APP_OUT/text.json",json);
            var buckets = map.GetType().GetField("table", BindingFlags.Instance | BindingFlags.NonPublic);
            var arr = (IList)buckets.GetValue(map);
            Debug.LogError(map.Count + " " + arr.Count);
            //var entries = map.GetType().GetField("entries", BindingFlags.Instance | BindingFlags.NonPublic);
            //arr = (IList)entries.GetValue(map);
            //Debug.LogError(map.Count + " " + arr.Count);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
    }
#endif
}

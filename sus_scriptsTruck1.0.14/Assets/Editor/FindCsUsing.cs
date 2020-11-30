using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using Debug = UnityEngine.Debug;
using Object= UnityEngine.Object;

public class FindCsUsing : MonoBehaviour
{
    [MenuItem("/Tools/FindCsUsing")]
    static void GetReference()
    {
        string target = "";
        if (Selection.activeObject != null)
            target = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(target))
            return;
        string[] files = Directory.GetFiles(Application.dataPath, "*.prefab", SearchOption.AllDirectories);
        string[] scene = Directory.GetFiles(Application.dataPath, "*.unity", SearchOption.AllDirectories);
         
        List<Object> filelst = new List<Object>();
        for (int i = 0; i < files.Length; i++)
        {
            string[] source = AssetDatabase.GetDependencies(new string[] { files[i].Replace(Application.dataPath, "Assets") });
            for (int j = 0; j < source.Length; j++)
            {
                if (source[j] == target)
                    filelst.Add(AssetDatabase.LoadMainAssetAtPath(files[i].Replace(Application.dataPath, "Assets")));
            }
        }
        for (int i = 0; i < scene.Length; i++)
        {
            string[] source = AssetDatabase.GetDependencies(new string[] { scene[i].Replace(Application.dataPath, "Assets") });
            for (int j = 0; j < source.Length; j++)
            {
                if (source[j] == target)
                    filelst.Add(AssetDatabase.LoadMainAssetAtPath(scene[i].Replace(Application.dataPath, "Assets")));
            }
        }
        for (int i = 0; i < filelst.Count; i++)
        {
            Debug.Log(filelst[i].name);
        }
        Selection.objects = filelst.ToArray();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

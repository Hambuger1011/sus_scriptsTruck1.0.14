//using AB;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Reflection;
//using UnityEditor;
//using UnityEditor.SceneManagement;
//using UnityEngine;

//public partial class AbResConfigWindows : EditorWindow
//{
//    public static AbResConfigEditor dataEditor = null;// Editor.CreateEditor(SailingData.Instance) as SailingDataEditor;
//    static Vector2 dataScrollPos = Vector2.zero;
//    AbResConfig abResCfg;

//    static AbResConfigWindows m_window = null;
//    //[MenuItem("GameTools/关卡编辑器/工具窗口", false, MenuPriority.Scene + 200)]
//    public static void ShowWindow()
//    {
//        bool immediate = true;
//        //创建窗口
//        if (m_window == null)
//        {
//            m_window = ScriptableObject.CreateInstance<AbResConfigWindows>();
//            m_window.Show(immediate);
//            m_window.Focus();
//        }
//        else
//        {
//            //m_window.Show(immediate);
//            m_window.Focus();
//        }
//    }

//    private void Awake()
//    {
//        var opt = new AbOptions_UI();
//        //abResCfg = AbResConfig.LoadInEditor(opt.ConfigAssetName);
//    }

//    void OnGUI()
//    {
//        if (abResCfg == null) return;
//        if (dataEditor == null || dataEditor.target != abResCfg)
//        {
//            dataEditor = Editor.CreateEditor(abResCfg) as AbResConfigEditor;
//            dataEditor.isCustom = true;
//        }

//        dataScrollPos = EditorGUILayout.BeginScrollView(dataScrollPos);
//        EditorGUILayout.BeginVertical();
//        dataEditor.SetWindow(this.position, dataScrollPos);
//        dataEditor.OnInspectorGUI();
//        EditorGUILayout.EndVertical();
//        EditorGUILayout.EndScrollView();
//    }
//}


using AB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ABBuildWindow : EditorWindow
{
    public static readonly string SystemName_Common = "COMMON";
    public static readonly string SystemName_DataTable = "DataTable";
    public static readonly string SystemName_ImageWall = "ImageWall";

    static ABBuildWindow m_instance = null;
    [MenuItem("GameTools/AssetBundle/打包", false, MenuPriority.AB + 100)]
    public static void ShowWindow()
    {
        bool immediate = true;
        //创建窗口
        if (m_instance == null)
        {
            m_instance = ScriptableObject.CreateInstance<ABBuildWindow>();
            m_instance.Show(immediate);
            m_instance.Focus();
            m_instance.position = AlignCenter(800, 600);

            //Texture iconTexture = Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Textures/Icon_Dark" : "Textures/Icon_Light");
            //m_instance.titleContent = new GUIContent("EUI Design", iconTexture);
        }
        else
        {
            //m_window.Show(immediate);
            m_instance.Focus();
        }
    }

    static Rect AlignCenter(float width, float height)
    {
        var type = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
        var type1 = typeof(Editor).Assembly.GetType("UnityEditor.View");
        var obj = type.GetField("get", BindingFlags.Static | BindingFlags.Public).GetValue(null);
        var parent = type.GetProperty("parent", BindingFlags.Instance | BindingFlags.Public).GetValue(obj, null);
        var rect = (Rect)type1.GetProperty("screenPosition", BindingFlags.Instance | BindingFlags.Public).GetValue(parent, null);



        rect.x = rect.x + rect.width * 0.5f - width * 0.5f;
        rect.y = rect.y + rect.height * 0.5f - height * 0.5f;
        rect.width = width;
        rect.height = height;
        return rect;
    }

    private void Awake()
    {
    }

    private void OnEnable()
    {
        bookMap.Clear();
        var books = Directory.GetDirectories("assets/bundle/book/", "*", SearchOption.TopDirectoryOnly);
        foreach (var bookPath in books)
        {
            var path = AbUtility.NormalizerAbName(bookPath);
            var name = Path.GetFileName(bookPath);
            int id = int.Parse(name);
            bookMap.Add(id, path);
        }
    }

    static Vector2 scrollPosition = Vector2.zero;
    SortedList<int, string> bookMap = new SortedList<int, string>(new BookCmp());

    class BookCmp : IEqualityComparer<int>, IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return x - y;
        }

        public bool Equals(int x, int y)
        {
            return x == y;
        }

        public int GetHashCode(int obj)
        {
            return obj;
        }
    }


    private void OnGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        if (GUILayout.Button("全部打包", GUILayout.Height(35)))
        {
            BuildCommon();
            BuildDataConfig();
            BuildImageWall();
            foreach (var itr in bookMap)
            {
                BuildBook(itr.Key, itr.Value);
            }
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("通用资源", GUILayout.Height(35)))
        {
            BuildCommon();
        }
        if (GUILayout.Button("形象墙资源", GUILayout.Height(35)))
        {
            BuildImageWall();
        }
        if (GUILayout.Button("DataTable", GUILayout.Height(35)))
        {
            BuildDataConfig();
        }
        GUILayout.EndHorizontal();
        int index = 0;
        if (bookMap.Count > 0) {
            GUILayout.BeginHorizontal();
        }
        foreach (var itr in bookMap)
        {
            if (GUILayout.Button("Book_" + itr.Key, GUILayout.Height(35)))
            {
                BuildBook(itr.Key, itr.Value);
            }
            if (index % 4 == 3)
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }
            index++;
        }
        if (bookMap.Count > 0)
        {
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }

    void BuildCommon()
    {
        try
        {
            var options = new AbOptions_UI();
            AbBuilder uiBuilder = new AbBuilder(options);
            uiBuilder.Begin();
            uiBuilder.Analyze();
            uiBuilder.Build();
            uiBuilder.End();
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    void BuildDataConfig()
    {
        try
        {
            var options = new AbOptions_DataTable();
            AbBuilder uiBuilder = new AbBuilder(options);
            uiBuilder.Begin();
            uiBuilder.Analyze();
            uiBuilder.Build();
            uiBuilder.End();
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    void BuildBook(int id, string path)
    {

        try
        {
            var options = new AbOptions_Book(id, path);
            AbBuilder uiBuilder = new AbBuilder(options);
            uiBuilder.Begin();
            uiBuilder.Analyze();
            uiBuilder.Build();
            uiBuilder.End();
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    void BuildImageWall()
    {
        try
        {
            var options = new AbOptions_ImageWall();
            AbBuilder uiBuilder = new AbBuilder(options);
            uiBuilder.Begin();
            uiBuilder.Analyze();
            uiBuilder.Build();
            uiBuilder.End();
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }
}

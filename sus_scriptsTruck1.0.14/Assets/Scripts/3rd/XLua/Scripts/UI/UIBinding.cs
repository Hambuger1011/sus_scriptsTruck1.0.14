using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[XLua.Hotfix, XLua.LuaCallCSharp,XLua.CSharpCallLua]
public class UIBinding : MonoBehaviour
{
    [System.Serializable]
    public class Meta
    {
        public string _name;
        public GameObject _object;
    }

    [SerializeField]
    public List<Meta> metas = new List<Meta>();

    private Dictionary<string, GameObject> map = null;

    public GameObject this[string name]
    {
        get
        {
            if(map == null)
            {
                map = new Dictionary<string, GameObject>();
                foreach(var itr in metas)
                {
                    map.Add(itr._name, itr._object);
                }
            }
            GameObject go;
            if(map.TryGetValue(name, out go))
            {

            }
            return go;
        }
    }


    public Component Get(string name, Type type)
    {
        var node = this[name];
        if (node == null)
        {
            LOG.Error("Not Found UI:" + name + " in " + this.name);
            return null;
        }
        var c = node.GetComponent(type);
        if (c == null)
        {
            LOG.Error("Not Found Component:" + type + " in " + name);
            return null;
        }
        return c;
    }

    public GameObject Get(string name)
    {
        return this[name];
    }

    public void CheckKeys()
    {
#if UNITY_EDITOR
        if (map == null)
        {
            map = new Dictionary<string, GameObject>();
        }
        map.Clear();
        foreach (var itr in metas)
        {
            if (map.ContainsKey(itr._name))
            {
                UnityEngine.Debug.LogError("key重复:" + itr._name);
                EditorUtility.DisplayDialog("key重复", itr._name, "确定");
                continue;
            }
            map.Add(itr._name, itr._object);
        }
#endif
    }
}


#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(UIBinding))]
public class UIBindingEditor : Editor
{
    public static GUIContent iconToolbarPlus;
    public static GUIContent iconToolbarMinus;
    public static GUIStyle preButton;

    void Init()
    {
        if (iconToolbarPlus != null)
        {
            return;
        }
        iconToolbarPlus = EditorGUIUtility.IconContent("Toolbar Plus", "Add to list");
        iconToolbarMinus = EditorGUIUtility.IconContent("Toolbar Minus", "Remove selection from list");
        preButton = "RL FooterButton";
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Init();
        EditorGUI.BeginChangeCheck();
        GUILayout.Space(50);
        EditorGUILayout.BeginHorizontal();
        //添加按钮
        if (GUILayout.Button(UIBindingEditor.iconToolbarPlus, GUILayout.Height(25)))
        {
            var o = this.target as UIBinding;
            o.metas.Add(new UIBinding.Meta());
        }

        ////删除按钮
        //if (GUILayout.Button(UIBingingEditor.iconToolbarMinus, UIBingingEditor.preButton, GUILayout.Height(25)))
        //{
        //    var o = this.target as UIBinging;
        //    o.metas.RemoveAt(o.metas.Count - 1);
        //}
        EditorGUILayout.EndHorizontal();
        if (EditorGUI.EndChangeCheck())
        {

            var o = this.target as UIBinding;
            o.CheckKeys();
        }
        GUILayout.Space(50);
        //添加按钮
        if (GUILayout.Button("检查Keys", GUILayout.Height(25)))
        {
            var o = this.target as UIBinding;
            o.CheckKeys();
        }
    }
}
#endif
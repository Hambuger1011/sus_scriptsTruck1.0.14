using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif
using Object = UnityEngine.Object;


public class UIConfigLegacy
{
    Transform m_rootTrans;
    string m_strPathPrefix = null;
    Dictionary<string, GameObject> m_uiObjDict = new Dictionary<string, GameObject>();

    public UIConfigLegacy(Transform trans, string strPathPrefix = null)
    {
        m_rootTrans = trans;
        m_strPathPrefix = strPathPrefix;
        if(m_strPathPrefix != null)
        {
            if(!m_strPathPrefix.EndsWith("/"))
            {
                m_strPathPrefix += "/";
            }
        }
    }
    public GameObject Get(string name, bool ignoreNull = false)
    {
        return this[name, ignoreNull];
    }

    public T Get<T>(string name, bool ignoreNull = false) where T : Component
    {
        T t = null;
        var go = this[name, ignoreNull];
        if(go != null)
        {
            t = go.GetComponent<T>();
        }
        return t;
    }

    public GameObject this[string name,bool ignoreNull = false]
    {
        get
        {
            GameObject go;
            if (m_uiObjDict.TryGetValue(name, out go))
            {
                return go;
            }
            else
            {
                Transform t = null;
                if(m_strPathPrefix != null)
                {
                    var path = name.Substring(name.IndexOf(m_strPathPrefix));
                    t = m_rootTrans.Find(path);
                }
                else
                {
                    t = m_rootTrans.Find(name);
                }
                if (t != null)
                {
                    go = t.gameObject;
                    m_uiObjDict.Add(name, go);
                }
                else if(!ignoreNull)
                {
                    LOG.Error("找不到UI控件:" + name);
                }
            }
            return go;
        }
    }
}


//#if UNITY_EDITOR
//[CustomEditor(typeof(UIConfig)), CanEditMultipleObjects]
//public class UIConfigEditor : Editor
//{
//    UIConfig m_uiConfig;
//    void OnEnable()
//    {
//        m_uiConfig = target as UIConfig;
//    }

//    public override void OnInspectorGUI()
//    {
//        /*
//        if (GUILayout.Button("记录UI控件", GUILayout.Height(30)))
//        {
//            m_uiConfig.SerializeUI();
//            EditorUtility.SetDirty(m_uiConfig);
//            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
//            serializedObject.Update();
//        }
//        GUILayout.Space(25);
//        */
//        if(m_uiConfig == null)
//        {
//            return;
//        }
//        base.OnInspectorGUI();
//        OnDragEventHandle();//拖拽事件
//    }
//    void OnDragEventHandle()
//    {
//        DropProc(obj =>
//        {
//            GameObject go = obj as GameObject;
//            if(go == null || m_uiConfig == null)
//            {
//                return;
//            }

//            if(go != m_uiConfig.gameObject /*|| go.transform.IsChildOf(m_uiConfig.transform) || m_uiConfig.transform.IsChildOf(go.transform)*/)
//            {
//                foreach(var otherGo in m_uiConfig.uiObjs)
//                {
//                    if(otherGo == go)
//                    {
//                        return;
//                    }
//                    if(otherGo.name == go.name)
//                    {
//                        Debug.LogError("存在同名UI控件:"+go.name);
//                        return;
//                    }
//                }
//                int s = m_uiConfig.uiObjs.Length;
//                Array.Resize<GameObject>(ref m_uiConfig.uiObjs, s + 1);
//                m_uiConfig.uiObjs[s] = go;
//                EditorUtility.SetDirty(m_uiConfig);
//                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
//                //serializedObject.Update();
//            }
//        });
//    }

//    void DropProc(System.Action<Object> OnDropped)
//    {
//        var evt = Event.current;

//        int id = GUIUtility.GetControlID(FocusType.Passive);
//        switch (evt.type)
//        {
//            case EventType.DragUpdated:
//            case EventType.DragPerform:
//                //if (!dropArea.Contains(evt.mousePosition)) break;

//                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
//                DragAndDrop.activeControlID = id;

//                if (evt.type == EventType.DragPerform)
//                {
//                    DragAndDrop.AcceptDrag();

//                    foreach (var draggedObject in DragAndDrop.objectReferences)
//                    {
//                        //Debug.Log("Drag Object:" + AssetDatabase.GetAssetPath(draggedObject));
//                        if (OnDropped != null)
//                            OnDropped(draggedObject);
//                    }
//                    DragAndDrop.activeControlID = 0;
//                }
//                Event.current.Use();
//                break;
//        }
//    }
//}
//#endif

//using AB;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Text;
//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(AbResConfig), true), CanEditMultipleObjects]
//public class AbResConfigEditor : Editor
//{
//    private Rect windowRect = new Rect(0, 0, 10000, 500);
//    private Vector2 scrollDelta;
//    private SerializedProperty m_Script;
//    private SerializedProperty lastModify, fileHashName;
//    private static bool mIsDirty = false;

//    private AbResConfig instance;
//    public bool isCustom = false;

//    private void OnEnable()
//    {
//        if (this.target == null)
//        {
//            return;
//        }
//        instance = (AbResConfig)target;
//        m_Script = this.serializedObject.FindProperty("m_Script");
//        lastModify = this.serializedObject.FindProperty("lastModify");
//        fileHashName = this.serializedObject.FindProperty("fileHashName");
//        Refresh();
//    }

//    public override void OnInspectorGUI()
//    {
//        if (m_dataList == null || instance == null)
//        {
//            base.OnInspectorGUI();
//        }
//        else
//        {
//            if(!isCustom)
//            {
//                if (GUILayout.Button("显示窗口", GUILayout.Height(25)))
//                {
//                    AbResConfigWindows.ShowWindow();
//                }
//            }
//            DrawCustomInspector();
//        }
//    }


//    public void DrawCustomInspector()
//    {
//        EditorGUI.BeginChangeCheck();
//        serializedObject.Update();

//        EditorGUILayout.PropertyField(m_Script, true);

//        if (m_dataProperty != null && instance != null)
//        {
//            EditorGUI.BeginDisabledGroup(true);
//            EditorGUILayout.PropertyField(lastModify);
//            EditorGUILayout.PropertyField(fileHashName);
//            EditorGUI.EndDisabledGroup();

//            m_dataList.DoLayoutList();//绘制
//        }
//        else
//        {
//            GUILayout.Label("没有数据");
//        }

//        if (EditorGUI.EndChangeCheck() || mIsDirty)
//        {
//            mIsDirty = false;
//            serializedObject.ApplyModifiedProperties();
//            EventRouter.CreateInstance();
//            EventRouter.Instance.BroadCastEvent("ELevelDataEditor.OnDirty");
//        }
//    }

//    public void SetWindow(Rect winRect, Vector2 dataScrollPos)
//    {
//        windowRect = winRect;
//        scrollDelta = dataScrollPos;
//    }
//    private void Apply()
//    {
//        serializedObject.ApplyModifiedProperties();
//        serializedObject.Update();
//        this.Repaint();
//    }

//    public void Refresh()
//    {
//        InitDataReorderableList();
//    }

//    #region 数据列表
//    private SerializedProperty m_dataProperty;
//    private EditorGUIList m_dataList;
//    void InitDataReorderableList()
//    {
//        m_dataProperty = serializedObject.FindProperty("abConfItems");
//        m_dataList = new EditorGUIList();
//        m_dataList.enableSearch = true;

//        #region 查找
//        m_dataList.onSearchCallback = (string searchKey, List<int> searchResult) =>
//        {
//            m_dataList.index = -1;
//            searchResult.Clear();
//            if (string.IsNullOrEmpty(searchKey)) return;

//            var key = searchKey.ToLower();
//            var items = instance.abConfItems;
//            for (int i = 0; i < items.Count; i++)
//            {
//                var item = items[i];
//                if (item.filename.ToLower().Contains(key) || item.fileHashName.ToLower().Contains(key))
//                {
//                    searchResult.Add(i);
//                    continue;
//                }
//                foreach (var itr in item.assetNames)
//                {
//                    if (itr.ToLower().Contains(key))
//                    {
//                        searchResult.Add(i);
//                        break;
//                    }
//                }
//            }
//        };
//        #endregion

//        #region 功能
//        m_dataList.onListElementsHeight = () =>
//        {
//            float height = 0;
//            if (m_dataProperty.isExpanded)
//            {
//                int count = m_dataList.onCount();//mInstance.groups.Count;
//                height = count * m_dataList.elementHeight + 7f;
//                if (m_dataList.index != -1)
//                {
//                    height += m_dataList.onElementExpandHeight(m_dataList.index);
//                }
//            }
//            return height;
//        };


//        m_dataList.onElementExpandHeight = (int index) =>
//        {
//            SerializedProperty element = m_dataProperty.GetArrayElementAtIndex(index);
//            return EditorGUI.GetPropertyHeight(element) - EditorGUIUtility.singleLineHeight;
//        };

//        m_dataList.onIsRootExpanded = () =>
//        {
//            return m_dataProperty.isExpanded;
//        };
//        m_dataList.onCount = () =>
//        {
//            if (m_dataList.searchResult.Count != 0)
//            {
//                return m_dataList.searchResult.Count;
//            }
//            return m_dataProperty.arraySize;
//        };
//        #endregion

//        #region 绘制
//        ///绘制头部
//        m_dataList.onDrawHeader = (Rect rect) =>
//        {
//            EditorGUIList.Defaults.DrawHeader(rect, new GUIContent(string.Format("AssetBundle({0})", m_dataProperty.arraySize)), m_dataProperty);
//        };

//        //绘制element背景
//        m_dataList.onDrawElementBackground = (Rect rect, int index, bool isActive, bool isFocused) =>
//        {
//            if (GUIExtensions.IsDraw)
//            {
//                Rect bgRect = rect;
//                bgRect.xMin += 2;
//                bgRect.xMax -= 5;
//                bgRect.yMax -= 5;
//                EditorGUI.DrawRect(bgRect, new Color(0.15f, 0.15f, 0.15f));

//                Rect handleRect = new Rect(rect.x + 5f, rect.y + 7f, 10f, rect.height - (rect.height - 7f));
//                EditorGUIList.Defaults.draggingHandle.Draw(handleRect, false, isActive, isActive, isFocused);
//            }
//        };
//        //绘制element
//        m_dataList.onDrawElements = (Rect rect) =>
//        {
//            Rect elementRect = rect;
//            elementRect.height = m_dataList.elementHeight;
//            int drawNum = 0;

//            var items = instance.abConfItems;
//            for (int i = 0; i < items.Count; i++)
//            {
//                if (m_dataList.searchResult.Count > 0)
//                {
//                    if (!m_dataList.searchResult.Contains(i))
//                    {
//                        continue;
//                    }
//                }
//                //超出边界不绘制
//                bool isSelected = (m_dataList.index == i);
//                if (windowRect.height <= 0 || isSelected || (elementRect.y - scrollDelta.y >= -100 && elementRect.y - scrollDelta.y <= windowRect.height))
//                {
//                    ++drawNum;
//                    var item = items[i];

//                    m_dataList.onDrawElementBackground(elementRect, i, isSelected, isSelected);
//                    Rect titleRect = GetElementRenderRect(elementRect);

//                    if (isSelected)
//                    {
//                        SerializedProperty element = m_dataProperty.GetArrayElementAtIndex(i);
//                        element.isExpanded = isSelected;
//                        EditorGUI.PropertyField(titleRect, element, new GUIContent(string.Format("{0}、{1}", i + 1, item.filename)), true);
//                    }
//                    else
//                    {
//                        if (GUI.Button(titleRect, ""))
//                        {
//                            isSelected = true;
//                            SerializedProperty element = m_dataProperty.GetArrayElementAtIndex(i);
//                            element.isExpanded = isSelected;
//                        }
//                        //EditorGUI.PropertyField(titleRect, element, new GUIContent(string.Format("{0}、[第{1}章]-第{0}关", i + 1, item.chapterID)), true);
//                        EditorGUI.Foldout(titleRect, false, new GUIContent(string.Format("{0}、{1}", i + 1, item.filename)));

//                    }
//                    if (isSelected)
//                    {
//                        m_dataList.index = i;
//                    }
//                    else
//                    {
//                        if (m_dataList.index == i)
//                        {
//                            m_dataList.index = -1;
//                        }
//                    }
//                }

//                elementRect.y += m_dataList.elementHeight;
//                if (isSelected)
//                {
//                    elementRect.y += m_dataList.onElementExpandHeight(i);
//                }
//            }
//            //LOG.Error(drawNum);
//        };
//        #endregion
//    }

//    /// <summary>
//    /// 获取item的rect
//    /// </summary>
//    public static Rect GetElementRenderRect(Rect elementRect)
//    {

//        float offset = 5;

//        Rect rect = elementRect;
//        rect.xMin += offset + 10;
//        rect.xMax -= 5;
//        rect.yMin += 1;
//        rect.yMax -= 1;

//        return rect;
//    }
//    #endregion

//}


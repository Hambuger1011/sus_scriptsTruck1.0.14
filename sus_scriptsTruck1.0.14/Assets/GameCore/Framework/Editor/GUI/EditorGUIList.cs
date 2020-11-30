#if !false
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections.Generic;

public static class GUIExtensions{
    public static bool IsDraw
    {
        get
        {
            return Event.current.type == EventType.Repaint;
        }
    }
}

public class EditorGUIList {

    public class Defaults
    {
        private const int buttonWidth = 25;

        public const int padding = 6;

        public const int dragHandleWidth = 20;

        public static GUIContent iconToolbarPlus = EditorGUIUtility.IconContent("Toolbar Plus", "Add to list");

        public static GUIContent iconToolbarPlusMore = EditorGUIUtility.IconContent("Toolbar Plus More", "Choose to add to list");

        public static GUIContent iconToolbarMinus = EditorGUIUtility.IconContent("Toolbar Minus", "Remove selection from list");

        public static readonly GUIStyle draggingHandle = "RL DragHandle";

        public static readonly GUIStyle headerBackground = "RL Header";

        public static readonly GUIStyle footerBackground = "RL Footer";

        public static readonly GUIStyle boxBackground = "RL Background";

        public static readonly GUIStyle preButton = "RL FooterButton";

        public static GUIStyle elementBackground = new GUIStyle("RL Element");
        public static GUIContent expandButton = EditorGUIUtility.IconContent("winbtn_win_max");
        public static GUIContent collapseButton = EditorGUIUtility.IconContent("winbtn_win_min");

        public static void DrawHeader(Rect rect,GUIContent label, SerializedProperty element)
        {
            bool isExpanded = element.isExpanded;

            Rect btnRect = rect;
            btnRect.xMax -= 45;
            if (GUI.Button(btnRect, string.Empty))
            {
                isExpanded = !isExpanded;
            }
            EditorGUILayout.Space();
            if (GUIExtensions.IsDraw)
            {
                Defaults.headerBackground.Draw(rect, false, false, false, false);
            }

            //HandleDragAndDrop(rect, Event.current);

            Rect titleRect = rect;
            titleRect.xMin += 6f;
            titleRect.xMax -= 55f;
            titleRect.height -= 2f;
            titleRect.y++;

            label = EditorGUI.BeginProperty(titleRect, label, element);
            titleRect.xMin += 10;
            isExpanded = EditorGUI.Foldout(titleRect, isExpanded, label, true);
            if (element.isExpanded != isExpanded)
            {
                element.isExpanded = isExpanded;
            }
            EditorGUI.EndProperty();

            //if (elementDisplayType != ElementDisplayType.SingleLine)
            {

                Rect bRect1 = rect;
                bRect1.xMin = rect.xMax - 25;
                bRect1.xMax = rect.xMax - 5;

                if (GUI.Button(bRect1, Defaults.expandButton, Defaults.preButton))
                {

                    ExpandElements(true, element);
                }

                Rect bRect2 = rect;
                bRect2.xMin = bRect1.xMin - 20;
                bRect2.xMax = bRect1.xMin;

                if (GUI.Button(bRect2, Defaults.collapseButton, Defaults.preButton))
                {

                    ExpandElements(false, element);
                }
            }
        }

        public static void DrawFooter(Rect rect, EditorGUIList list)
        {
            float xMax = rect.xMax;
            float num = xMax - 58f;
            rect = new Rect(num, rect.y, xMax - num, rect.height);
            if (GUIExtensions.IsDraw)
            {
                Defaults.footerBackground.Draw(rect, false, false, false, false);
            }
            EditorGUI.BeginDisabledGroup(list.searchResult.Count > 0);
            //添加按钮
            Rect rect2 = new Rect(num + 4f, rect.y - 3f, 25f, 13f);
            if (GUI.Button(rect2, Defaults.iconToolbarPlus , Defaults.preButton))
            {
                list.onAddCallback();

                if (list.onChangedCallback != null)
                {
                    list.onChangedCallback();
                }
            }
            //删除按钮
            Rect position = new Rect(xMax - 29f, rect.y - 3f, 25f, 13f);
            EditorGUI.BeginDisabledGroup(list.index < 0 || list.index >= list.onCount());
            if (GUI.Button(position, Defaults.iconToolbarMinus, Defaults.preButton))
            {
                list.onRemoveCallback();

                if (list.onChangedCallback != null)
                {
                    list.onChangedCallback();
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.EndDisabledGroup();
        }
    }


    private int m_ActiveElement = -1;
    private float m_DragOffset;
    private float m_DraggedY;

    public float elementHeight = 21f;
    public float footerHeight = 13f;
    public float headerHeight = 18f;

    public Func<bool> onIsRootExpanded;

    /// <summary>
    /// 获取总数量
    /// </summary>
    public Func<int> onCount;

    /// <summary>
    /// 获取列表总高度
    /// </summary>
    public Func<float> onListElementsHeight;

    /// <summary>
    /// 获取元素展开时高度
    /// </summary>
    public Func<int,float> onElementExpandHeight;

    public Action<Rect> onDrawHeader;
    public Action<Rect> onDrawFooter;
    public Action<Rect> onDrawElements;
    public Action<Rect,int,bool,bool> onDrawElementBackground;

    /// <summary>
    /// 添加回调
    /// </summary>
    public Action onAddCallback;
    public Action onChangedCallback;

    /// <summary>
    /// 删除回调
    /// </summary>
    public Action onRemoveCallback;

    public Action onInsertCallback;

    /// <summary>
    /// 右键
    /// </summary>
    public Action<int> onRightMouseClick;

    /// <summary>
    /// 查找回调
    /// </summary>
    public Action<string,List<int>> onSearchCallback;

    public int dragIndex = -1;

    public bool enableSearch = false;
    public List<int> searchResult = new List<int>();


    public int index
    {
        get
        {
            return this.m_ActiveElement;
        }
        set
        {
            this.m_ActiveElement = value;
        }
    }

    public EditorGUIList()
    {
        this.InitList();
    }
    private void InitList()
    {
    }

    public void DoLayoutList()
    {
#if true
        Rect searchRect = new Rect();
        Rect rect2 = new Rect();
        Rect rect3 = new Rect();

        Rect rect = GUILayoutUtility.GetRect(0f, this.headerHeight, GUILayout.ExpandWidth(true));
        if (onIsRootExpanded())
        {
            if (enableSearch)
            {
                searchRect = GUILayoutUtility.GetRect(0f, this.headerHeight, GUILayout.ExpandWidth(true));
            }
            rect2 = GUILayoutUtility.GetRect(100f, this.onListElementsHeight(), GUILayout.ExpandWidth(true));
            rect3 = GUILayoutUtility.GetRect(4f, this.footerHeight, GUILayout.ExpandWidth(true));
        }
        this.DoListHeader(rect);
        if (onIsRootExpanded())
        {
            if (enableSearch)
            {
                this.DoListSearch(searchRect);
            }
            this.DoListElements(rect2);
            this.DoListFooter(rect3);
        }

#else
        float height = this.headerHeight;
        if (serializedProperty.isExpanded)
        {
            height = this.headerHeight + this.GetListElementHeight() + this.footerHeight;
        }
        Rect position = EditorGUILayout.GetControlRect(false, height, EditorStyles.largeLabel);
        Rect rect = EditorGUI.IndentedRect(position);
        this.DoListHeader(rect);
#endif
    }

    string searchId = "";
    string searchId_tmp = "";

    private void DoListSearch(Rect searchRect)
    {
        Rect labelRect = searchRect;
        labelRect.width = 100;
        EditorGUI.LabelField(labelRect, "事件表ID搜索:");

        Rect idRect = searchRect;
        idRect.xMin = labelRect.xMax;
        idRect.xMax -= 200;
        searchId_tmp = EditorGUI.TextField(idRect, searchId_tmp);

        Rect btnRect = searchRect;
        btnRect.xMin = idRect.xMax;
        if (GUI.Button(btnRect, "搜索"))
        {
            searchId = searchId_tmp;
            onSearchCallback(searchId, searchResult);
        }
    }

    private void DoListHeader(Rect headerRect)
    {
        /*
        headerRect.xMin += 6f;
        headerRect.xMax -= 6f;
        headerRect.height -= 2f;
        headerRect.y += 1f;
        */
        this.onDrawHeader(headerRect);
    }

    private void DoListFooter(Rect footerRect)
    {
        if (this.onDrawFooter != null)
        {
            this.onDrawFooter(footerRect);
        }
        else
        {
            EditorGUIList.Defaults.DrawFooter(footerRect, this);
        }
    }

    private void DoListElements(Rect listRect)
    {
        if (GUIExtensions.IsDraw)
        {
            EditorGUIList.Defaults.boxBackground.Draw(listRect, false, false, false, false);
        }
        listRect.yMin += 2f;
        listRect.yMax -= 5f;
        Rect rect = listRect;
        rect.height = this.elementHeight;
        Rect rect2 = rect;

        int count = onCount();
        if (count > 0)
        {
            this.onDrawElements(listRect);
            this.DoDraggingAndSelection(listRect);
        }
        else
        {
            rect.y = listRect.y;
            if (this.onDrawElementBackground == null)
            {
                if (GUIExtensions.IsDraw)
                {
                    EditorGUIList.Defaults.elementBackground.Draw(rect, false, false, false, false);
                    EditorGUIList.Defaults.draggingHandle.Draw(new Rect(rect.x + 5f, rect.y + 7f, 10f, rect.height - (rect.height - 7f)), false, false, false, false);
                }
            }
            else
            {
                this.onDrawElementBackground(rect, -1, false, false);
            }
            rect2 = rect;
            rect2.xMin += 6f;
            rect2.xMax -= 6f;
            EditorGUI.LabelField(rect, "List is Empty");
        }
    }

    public float GetHeight()
    {
        float num = 0f;
        num += this.onListElementsHeight();
        num += this.headerHeight;
        return (num + this.footerHeight);
    }


    private static void ExpandElements(bool expand, SerializedProperty element)
    {

        if (!element.isExpanded && expand)
        {

            element.isExpanded = true;
        }

        for (int i = 0; i < element.arraySize; i++)
        {

            element.GetArrayElementAtIndex(i).isExpanded = expand;
        }
    }

    private void DoDraggingAndSelection(Rect listRect)
    {
        Event current = Event.current;
        int activeElement = this.m_ActiveElement;
        bool flag = false;
        switch (current.type)
        {
            case EventType.MouseDown:
                //Wizard.Debug.Info("MouseDown");
                dragIndex = -1;
                if (listRect.Contains(Event.current.mousePosition))
                {
                    //current.Use();
                    //Wizard.Debug.Info("down"+ index);
                    if(Event.current.button == 1)
                    {
                        /*
                        if (index == -1)
                        {
                            float offset = Event.current.mousePosition.y - listRect.y;
                            index = Mathf.Clamp(Mathf.FloorToInt(offset / this.elementHeight), 0, this.onCount() - 1);
                        }
                        */
                        if (index >= 0 && onRightMouseClick != null)
                        {
                            onRightMouseClick(index);
                        }
                    }
                    break;
                }
                break;
#if false
            case EventType.MouseDrag:
                if (dragIndex == -1 && listRect.Contains(Event.current.mousePosition) && (Event.current.button == 0))
                {
                    dragIndex = GetIndex(listRect, Event.current.mousePosition);
                    Wizard.Debug.Info("drag:"+ dragIndex);
                    if(this.onDragCallbackDelegate != null)
                    {
                        onDragCallbackDelegate(dragIndex);
                    }
                    current.Use();
                }
                break;
                /*
            case EventType.DragUpdated:
                if (dragIndex == -1)
                {
                    int id = GUIUtility.GetControlID(FocusType.Passive);
                    DragAndDrop.activeControlID = id;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                }
                //current.Use();
                break;
            case EventType.DragPerform:
                Wizard.Debug.Info("DragPerform");
                if (dragIndex == -1)
                {
                    int id = GUIUtility.GetControlID(FocusType.Passive);
                    DragAndDrop.activeControlID = id;
                    DragAndDrop.AcceptDrag();
                    foreach (var draggedObject in DragAndDrop.objectReferences)
                    {
                        //Debug.Log("Drag Object:" + AssetDatabase.GetAssetPath(draggedObject));
                        if (this.onDragFinishCallbackDelegate != null)
                        {
                            onDragFinishCallbackDelegate(draggedObject);
                        }
                    }
                    DragAndDrop.activeControlID = 0;
                    //current.Use();
                }
                break;
                */
            case EventType.DragExited:
                Wizard.Debug.Info("DragExited:"+ dragIndex);
                //dragIndex = -1;
                //current.Use();
                break;
#endif
            default:
                //dragIndex = -1;
                break;
        }
        /*
        if (((this.m_ActiveElement != activeElement) || flag) && (this.onSelectCallback != null))
        {
            this.onSelectCallback(this);
        }
        */
    }
}
#endif

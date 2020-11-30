using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Diagnostics;
using System.Collections;

public class EditorTree
{
    public delegate void OnDrawElement(int index, EditorTreeNode node, Rect rect, EditorTreeEventData eData, bool isActive, bool isFocused);
    EditorTreeNode mTreeRoot = null;
    EditorTreeNode mSearchTree = new EditorTreeNode("搜索结果");
    public OnDrawElement onDrawElement = null;


    Rect windowRect;
    Vector2 scrollDelta;

    public EditorTree()
    {
        mTreeRoot = new EditorTreeNode("事件库");
    }

    public void SetWindow(Rect winRect, Vector2 dataScrollPos)
    {
        windowRect = winRect;
        scrollDelta = dataScrollPos;
    }


    EditorTreeNode GetNode(EditorTreeNode parentNode,string name)
    {
        if (parentNode.name == name) return parentNode;
        for(int i = 0; i < parentNode.children.Count; ++i)
        {
            EditorTreeNode node = GetNode(parentNode.children[i], name);
            if(node != null)
            {
                return node;
            }
        }
        return null;
    }

    public void ClearNode()
    {
        mTreeRoot = null;
        mTreeRoot = new EditorTreeNode("事件库");
    }

    public void AddNode(string[] dirs, int index,int eventId)
    {
        EditorTreeNode treeNode = mTreeRoot;
        //添加节点
        for (int i=0;i<dirs.Length - 1; ++i)
        {
            EditorTreeNode node = GetNode(treeNode, dirs[i]);
            if(node == null)
            {
                node = new EditorTreeNode(dirs[i]);
                node.SetParent(treeNode);
            }
            treeNode = node;
        }

        //添加叶子
        if(treeNode.events.Find(e=>e.eventID == eventId) == null)
        {
            EditorTreeEventData eData = new EditorTreeEventData(index, eventId);

            treeNode.events.Add(eData);
            treeNode.events.Sort(eData);
        }
        treeNode.Refresh();
    }

    Stopwatch sw = new Stopwatch();
    public void Draw()
    {
        sw.Reset();
        sw.Start();

        GUILayout.Space(25);
        Rect searchRect = GUILayoutUtility.GetRect(0f, 25f, GUILayout.ExpandWidth(true));
        this.DoListSearch(searchRect);
        if (mSearchTree.count > 0)
        {
            DrawNode(mSearchTree);//绘制搜索的事件
        }
        else
        {
            DrawNode(mTreeRoot);//绘制所有的事件
        }

        sw.Stop();
        long elapsedSeconds = sw.ElapsedMilliseconds;
        if (elapsedSeconds > 0)
        {
            //Wizard.Debug.Info(string.Format("[EditorTree]耗时{0}ms",elapsedSeconds) );
        }
    }

    /// <summary>
    /// 绘制节点
    /// </summary>
    void DrawNode(EditorTreeNode node,float offset = 0)
    {
        string titleName = string.Format("{0}({1})", node.name, node.count);
        Rect rect = GUILayoutUtility.GetRect(0f, EditorGUIUtility.singleLineHeight * 2, GUILayout.ExpandWidth(true));

        ////超出边界不绘制
        bool isVisiable = (windowRect.height <= 0 || (rect.y - scrollDelta.y >= -100 && rect.y - scrollDelta.y <= windowRect.height));
        if (isVisiable)
        {
            DrawDirHead(rect, titleName, false, false);
        }
        rect.xMin += offset;
        node.isExpanded = EditorGUI.Foldout(rect, node.isExpanded, titleName, true);
        if (!node.isExpanded) return;
        foreach (var ch in node.children)
        {
            DrawNode(ch, offset + 10);
        }

        for (int i = 0; i < node.events.Count; ++i)
        {
            var e = node.events[i];
            DrawEvent(i,node, e, offset + 10);
        }
    }

    /// <summary>
    /// 绘制叶子
    /// </summary>
    void DrawEvent(int index,EditorTreeNode node, EditorTreeEventData eData, float offset = 0)
    {
        Rect rect = GUILayoutUtility.GetRect(0f, EditorGUIUtility.singleLineHeight * 1.5f, GUILayout.ExpandWidth(true));
        DrawFileHead(index,node, rect, eData, false, false, offset);
    }

    /// <summary>
    /// 节点名
    /// </summary>
    void DrawDirHead(Rect rect,string name,bool isActive, bool isFocused)
    {
        if (GUIExtensions.IsDraw)
        {
            Rect bgRect = rect;
            bgRect.xMin += 2;
            bgRect.xMax -= 5;
            bgRect.yMax -= 1;
            EditorGUI.DrawRect(bgRect, new Color(0.15f, 0.15f, 0.15f));

            Rect handleRect = new Rect(rect.x + 5f, rect.y + 7f, 10f, rect.height - (rect.height - 7f));
            //EditorGUIList.Defaults.draggingHandle.Draw(handleRect, false, isActive, isActive, isFocused);
        }
    }

    /// <summary>
    /// 文件名
    /// </summary>
    void DrawFileHead(int index, EditorTreeNode node, Rect rect, EditorTreeEventData eData, bool isActive, bool isFocused, float offset = 0)
    {
        if (GUIExtensions.IsDraw)
        {
            Rect bgRect = rect;
            bgRect.xMin += 2;
            bgRect.xMax -= 5;
            bgRect.yMax -= 1;
            EditorGUI.DrawRect(bgRect, new Color(0.15f, 0.15f, 0.15f));

            rect.xMin += offset;
            Rect handleRect = new Rect(rect.x + 5f, rect.y + 7f, 10f, rect.height - (rect.height - 7f));
            EditorGUIList.Defaults.draggingHandle.Draw(handleRect, false, isActive, isActive, isFocused);
        }
        rect.xMin += 15;
        if (onDrawElement != null)
        {
            onDrawElement(index,node,rect, eData,isActive,isFocused);
        }
    }


    public Action<int, EditorTreeNode> onSearchCallback;
    int searchId = -1;
    int searchId_tmp = -1;
    void DoListSearch(Rect searchRect)
    {
        Rect labelRect = searchRect;
        labelRect.width = 100;
        EditorGUI.LabelField(labelRect, "关卡ID搜索:");

        Rect idRect = searchRect;
        idRect.xMin = labelRect.xMax;
        idRect.xMax -= 200;
        searchId_tmp = EditorGUI.IntField(idRect, searchId_tmp);

        Rect btnRect = searchRect;
        btnRect.xMin = idRect.xMax;
        if (GUI.Button(btnRect, "搜索"))
        {
            searchId = searchId_tmp;
            onSearchCallback(searchId, mSearchTree);
        }
    }
}

[System.Serializable]
public class EditorTreeEventData :Comparer<EditorTreeEventData>
{
    public int eventID;
    public int index;

    public EditorTreeEventData(int idx,int id)
    {
        this.index = idx;
        this.eventID = id;
    }

    public override int Compare(EditorTreeEventData x, EditorTreeEventData y)
    {
        return x.eventID - y.eventID;
    }
}

[System.Serializable]
public class EditorTreeNode
{
    public string name;
    public EditorTreeNode parentNode = null;
    public List<EditorTreeNode> children = new List<EditorTreeNode>();
    public List<EditorTreeEventData> events = new List<EditorTreeEventData>();
    public bool isExpanded = false;
    public int count = 0;

    public EditorTreeNode(string name)
    {
        this.name = name;
    }

    public void SetParent(EditorTreeNode parent)
    {
        parentNode = parent;
        parentNode.children.Add(this);
        parent.count += this.count;
    }

    public void Refresh()
    {
        count = events.Count;
        foreach (var ch in children)
        {
            count += ch.count;
        }
        EditorTreeNode p = this.parentNode;
        while (p != null)
        {
            p.Refresh();
            p = p.parentNode;
        }
    }

    public void ReplaceID(int oldID, int newID)
    {
        var eData = events.Find(e => e.eventID == oldID);
        eData.eventID = newID;
    }

    public void Remove(EditorTreeEventData eData)
    {
        events.Remove(eData);

        EditorTreeNode node = this;
        while (node.children.Count == 0 && node.events.Count == 0)
        {
            node.parentNode.children.Remove(node);
            node.parentNode.Refresh();
            node = node.parentNode;
        }
    }
}

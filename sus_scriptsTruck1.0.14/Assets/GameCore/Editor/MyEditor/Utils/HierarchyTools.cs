using Framework;

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HierarchyTools
{
    [MenuItem("GameObject/复制UIFrame节点路径", priority = 0)]
    private static void NewMenuOption()
    {
        var go = Selection.activeGameObject;
        if(go == null)
        {
            return;
        }

        GameObject rootObj;
        if(PrefabUtility.GetPrefabType(go) == PrefabType.PrefabInstance)
        {
            rootObj = PrefabUtility.FindPrefabRoot(go);
        }else
        {
            //Debug.LogError("该方法只适用于prefab");
            //return;
            rootObj = go.GetComponentInParent<Canvas>().gameObject;
        }
        //var uiFrame = go.GetComponentInParent<UIFrame>();
        //if(uiFrame == null)
        //{
        //    return;
        //}
        Stack<string> pathStack = new Stack<string>();
        pathStack.Push(go.name);
        Transform parent = go.transform.parent;
        while(parent != rootObj.transform)
        {
            pathStack.Push(parent.name);
            parent = parent.parent;
        }
        string path = pathStack.Pop();
        while(pathStack.Count > 0)
        {
            path += "/"+pathStack.Pop();
        }
        //Debug.Log(path);
        EditorGUIUtility.systemCopyBuffer = path;
    }


    //[MenuItem("GameObject/拾取引导按钮信息", priority = 0)]
    //private static void PickButton()
    //{
    //    var go = Selection.activeGameObject;
    //    if (go == null)
    //    {
    //        return;
    //    }

    //    RectTransform canvasTrans = go.GetComponentInParent<Canvas>().transform as RectTransform;
    //    RectTransform trans = go.transform as RectTransform;

    //    var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(canvasTrans, trans);
    //    EditorGUIUtility.systemCopyBuffer = string.Format("{0}|{1:F1}|{2:F1}|{3:F1}|{4:F1}", go.name,bounds.center.x, bounds.center.y, bounds.size.x, bounds.size.y);
    //    Debug.Log(EditorGUIUtility.systemCopyBuffer);
    //}
}

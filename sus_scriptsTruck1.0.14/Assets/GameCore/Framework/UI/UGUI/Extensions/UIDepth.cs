using UnityEngine;
using System.Collections;
using Framework;
using UGUI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
public class UIDepth : CUIComponent
{
	public int order = 1;
    int addSortingOrder = 0;

    [Header("添加Canvas，特效请不要打勾")]
    public bool isUI = false;
    public bool autoSetOrder = false;
    private bool? isActive;

    void Start()
    {
        if(autoSetOrder)
        {
            ResetOrder();
        }
    }

    public override void OnOpen()
    {
        base.OnOpen();
        if(!isActive.HasValue){
            isActive = this.gameObject.activeSelf;
        }
        this.gameObject.SetActiveEx(isActive.Value);
    }

    public override void OnClose()
    {
        base.OnClose();
        this.gameObject.SetActiveEx(false);
    }


    void Init()
    {
        if (this.myForm == null)
        {
            var uiForm = this.GetComponentInParent<CUIForm>();
            this.Initialize(uiForm);
        }

        if (addSortingOrder == 0)
        {
            var canvas = this.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                addSortingOrder = canvas.sortingOrder;
            }
        }
    }

    public override void SetSortingOrder(int sortingOrder)
    {
        base.SetSortingOrder(sortingOrder);
        addSortingOrder = sortingOrder;
        ResetOrder();
    }

    [ContextMenu("ResetOrder")]
    public void ResetOrder()
    {
        Init();
        if (isUI)
        {
            Canvas canvas = GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = gameObject.AddComponent<Canvas>();
            }
            canvas.overrideSorting = true;
            canvas.sortingOrder = GetSortingOrder();
        }
        else
        {
            Renderer[] renders = GetComponentsInChildren<Renderer>(true);

            foreach (Renderer render in renders)
            {
                render.sortingOrder = GetSortingOrder();
            }
        }
#if UNITY_EDITOR
        sorderInLayer = GetSortingOrder();
#endif
    }

    public int GetSortingOrder()
    {
        return order + addSortingOrder;
    }

    public void SetActive(bool isOn)
    {
        this.isActive = isOn;
        this.gameObject.SetActiveEx(this.isActive.Value);
    }

#if UNITY_EDITOR
    [Header("调试信息")]
    public int sorderInLayer = 0;
#endif
}


#if UNITY_EDITOR

[CustomEditor(typeof(UIDepth), true)]
public class UIDepthEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(50);
        if (GUILayout.Button("设置层级", GUILayout.Height(25)))
        {
            var s = (UIDepth)this.target;
            s.ResetOrder();
        }
    }
}
#endif

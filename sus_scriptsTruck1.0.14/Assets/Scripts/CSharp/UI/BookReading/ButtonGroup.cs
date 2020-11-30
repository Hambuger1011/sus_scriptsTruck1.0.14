
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ButtonGroup : MonoBehaviour
{

    public GridLayoutGroup GridLayoutGroup { get { return m_gridLayoutGroup; } }
    public List<ButtonGroupSelection> UIObjectList { get { return m_uiObjectList; } }

    private RectTransform m_rectTransform;
    private CanvasGroup m_canvasGroup;
    private GridLayoutGroup m_gridLayoutGroup;

    private List<ButtonGroupSelection> m_uiObjectList;

    private Vector2 m_vCellSize;
    private Vector2 m_vSpacing;
#if NOT_USE_LUA
    public void Init(GameObject prefab, int count, Vector2 cellSize, Vector2 spacing)
    {
        m_rectTransform = this.gameObject.GetComponent<RectTransform>();
        m_uiObjectList = new List<ButtonGroupSelection>();
        for (int i = 0; i < count; i++)
        {
            RectTransform go = Instantiate(prefab, this.gameObject.transform).GetComponent<RectTransform>();
            go.name = prefab.name + "_" + i;
            go.ResetRectTransform();
            m_uiObjectList.Add(go.GetComponent<ButtonGroupSelection>());
        }
        m_canvasGroup = gameObject.AddMissingComponent<CanvasGroup>();
        m_gridLayoutGroup = gameObject.AddMissingComponent<GridLayoutGroup>();

        m_vCellSize = cellSize;
        m_vSpacing = spacing;
        m_gridLayoutGroup.cellSize = cellSize;
        m_gridLayoutGroup.spacing = spacing;
        m_gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
        m_gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
        m_gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
        m_gridLayoutGroup.constraint = GridLayoutGroup.Constraint.Flexible;
    }

    public void AddOnClickListener(UIVoidPointerEvent func)
    {
        int iCount = m_uiObjectList.Count;
        for (int i = 0; i < iCount; i++)
        {
            UIEventListener.AddOnClickListener(m_uiObjectList[i].gameObject, func);
        }
    }

    public void RemoveOnClickListener(UIVoidPointerEvent func)
    {
        if(m_uiObjectList != null)
        {
            int iCount = m_uiObjectList.Count;
            for (int i = 0; i < iCount; i++)
            {
                m_uiObjectList[i].Dispose();
                UIEventListener.RemoveOnClickListener(m_uiObjectList[i].gameObject, func);
                GameObject.Destroy(m_uiObjectList[i].gameObject);
                m_uiObjectList[i] = null;
            }
        }
        m_uiObjectList = null;
    }

    public void ChangeDialogDetails(int count, string[] selections, int[] cost,int[] hiddenEgg)
    {
        m_canvasGroup.alpha = 0;
        m_gridLayoutGroup.spacing = new Vector2(m_vSpacing.x, -100);
        int iCount = m_uiObjectList.Count;
        for (int i = 0; i < iCount; i++)
        {
            if (i < count)
            {
                m_uiObjectList[i].Init(selections[i], cost[i],hiddenEgg[i],i);
                m_uiObjectList[i].gameObject.SetActive(true);
            }
            else m_uiObjectList[i].gameObject.SetActive(false);
        }
    }

    public void ButtonGroupTween(float anchoredPosition_y, float duration)
    {
        //m_rectTransform.anchoredPosition = new Vector2(m_rectTransform.anchoredPosition.x, anchoredPosition_y);

        DOTween.To(() => 0f, (alpha) => m_canvasGroup.alpha = alpha, 1f, duration*0.8f).SetEase(Ease.Flash);
        DOTween.To(() => -100, (spacing_y) => m_gridLayoutGroup.spacing = new Vector2(m_vSpacing.x, spacing_y), m_vSpacing.y, duration).SetEase(Ease.Flash);
    }

    public void KillTween()
    {
        this.transform.DOKill();
    }
#endif
}
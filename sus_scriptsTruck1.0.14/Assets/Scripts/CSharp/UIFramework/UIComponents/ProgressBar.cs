using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProgressBar : UIBehaviour
{
    public Image ProgressBarImage;
    public float Value { get { return m_fValue; } set { m_fValue = Mathf.Clamp01(value); onValueChange(); } }
    private float m_fValue;

    public float HorizontalPadding = 1f;
    public float VerticalPadding = 1f;
    public bool FillFlag = false;   //是否用填充的形式
    private RectTransform m_rectTransform;

    protected override void Awake()
    {
        base.Awake();
        m_rectTransform = this.GetComponent<RectTransform>();
    }

    private void onValueChange()
    {
        if (ProgressBarImage == null) return;
        if (FillFlag)
            ProgressBarImage.fillAmount = m_fValue;
        else
            ProgressBarImage.rectTransform.offsetMax = new Vector2(MyUtils.RangeToRange(m_fValue, 0, 1, 0, m_rectTransform.rect.width - HorizontalPadding), (m_rectTransform.rect.height - VerticalPadding * 2) / 2);
    }

}

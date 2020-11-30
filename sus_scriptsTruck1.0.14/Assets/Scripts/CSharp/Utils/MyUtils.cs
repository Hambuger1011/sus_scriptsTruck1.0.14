using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyUtils
{
    public static T AddMissingComponent<T>(this GameObject go)where T: Component
    {
        //return go.GetComponent<T>() ?? go.AddComponent<T>();
        T compoent = go.GetComponent<T>();
        if (compoent == null) compoent = go.AddComponent<T>();
        return compoent;
    }

    public static void ResetRectTransform(this RectTransform rectTransform)
    {
        rectTransform.anchoredPosition3D = Vector3.zero;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one;
    }

    public static void ResetTransform(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public static void ClearAllChild(this Transform transform)
    {
        for (int i = 0,iCount = transform.childCount; i < iCount; i++)
        {
            Object.Destroy(transform.GetChild(i).gameObject);
        }
    }

    public static float RangeToRange(float value, float orignalMin, float orignalMax, float targetMin, float targetMax)
    {
        return (targetMax - targetMin) / (orignalMax - orignalMin) * (value - orignalMin) + targetMin;
    }
}
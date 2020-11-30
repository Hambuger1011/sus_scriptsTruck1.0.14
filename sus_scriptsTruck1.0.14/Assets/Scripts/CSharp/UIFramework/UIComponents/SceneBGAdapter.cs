using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneBGAdapter : MonoBehaviour {

    public const float width = 750f;
    public const float height = 1334f;

    public void ChangeBGSize(Image bg)
    {
        float screenX = System.Convert.ToSingle(Screen.width);
        float screenY = System.Convert.ToSingle(Screen.height);
        float scale = screenY / (screenX / width * height);
        float orignalW = bg.sprite.rect.width / bg.pixelsPerUnit;
        float orignalH = bg.sprite.rect.height / bg.pixelsPerUnit;
        bg.rectTransform.anchorMax = bg.rectTransform.anchorMin;
        bg.rectTransform.sizeDelta = new Vector2(orignalW, orignalH) * scale;
    }
}

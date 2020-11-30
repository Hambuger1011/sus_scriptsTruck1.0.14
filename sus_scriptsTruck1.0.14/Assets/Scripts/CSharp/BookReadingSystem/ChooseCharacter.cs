
using DG.Tweening;
using AB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseCharacter : MonoBehaviour
{
    public Image[] ImageGroup;
    public RectTransform ComfirmButton;
    public Image OnChoiceItem;

#if NOT_USE_LUA
    private void Start()
    {
        OnChoiceItem.DOColor(new Color(1, 1, 1, 0.5f), 0.8f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public void InitChooseCharacter(params int[] CharacterID)
    {
        for (int i = 0; i < CharacterID.Length; i++)
        {
            if (i < ImageGroup.Length)
            {
                int appearanceID = 100000 + (CharacterID[i] * 10000) + 100;
                ImageGroup[i].sprite = DialogDisplaySystem.Instance.GetUITexture("RoleHead/" + appearanceID, false);
                ImageGroup[i].transform.parent.gameObject.SetActive(true);
                //ImageGroup[i].rectTransform.parent.GetComponent<RectTransform>().anchoredPosition = getanchoredPosition(CharacterID.Length, i);
            }
            else
            {
                ImageGroup[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }

    //private Vector2 getanchoredPosition(int count, int index)
    //{
    //    switch (count)
    //    {
    //        case 3:
    //            return anchoredPosition_3[index];
    //        case 4:
    //            return anchoredPosition_4[index];
    //        case 5:
    //            return anchoredPosition_5[index];
    //        default:
    //            return Vector2.zero;
    //    }
    //}

    //private readonly Vector2[] anchoredPosition_3 = new Vector2[]
    //{
    //    new Vector2(-180,305),
    //    new Vector2(180,275),
    //    new Vector2(0,-18.5f),
    //    new Vector2(0,-290)
    //};
    //private readonly Vector2[] anchoredPosition_4 = new Vector2[]
    //{
    //    new Vector2(-180,305),
    //    new Vector2(180,275),
    //    new Vector2(-180,-25),
    //    new Vector2(180,-55),
    //    new Vector2(0,-290)
    //};
    //private readonly Vector2[] anchoredPosition_5 = new Vector2[]
    //{
    //    new Vector2(-180,305),
    //    new Vector2(180,275),
    //    new Vector2(0,20),
    //    new Vector2(-180,-240),
    //    new Vector2(-180,-270),
    //    new Vector2(0,-480)
    //};

#endif

}
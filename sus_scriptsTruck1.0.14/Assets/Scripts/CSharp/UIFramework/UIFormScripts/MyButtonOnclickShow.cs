using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyButtonOnclickShow : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{

    public GameObject ShowGame;
    public void OnPointerDown(PointerEventData eventData)
    {
        ShowGame.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
       
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ShowGame.SetActive(false);
    }
}

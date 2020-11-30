using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GeneroButtonItem : MonoBehaviour
{
    private Text OffButtonName, OnButtonName;
    private TypeSelectionFormLogic TypeSelectionFormLogic;
    private GameObject Off, On;
    private bool onClick;
    private int Index;
   public void Init(TypeSelectionFormLogic TypeSelectionFormLogic, int Index,string ButtonName)
    {
        this.Index = Index;
        OffButtonName = transform.Find("Off/Text").GetComponent<Text>();
        OnButtonName = transform.Find("ON/Text").GetComponent<Text>();

        Off = transform.Find("Off").gameObject;
        On = transform.Find("ON").gameObject;

        OffButtonName.text = ButtonName.ToString();
        OnButtonName.text= ButtonName.ToString();

        this.TypeSelectionFormLogic = TypeSelectionFormLogic;


        UIEventListener.AddOnClickListener(gameObject,ButtonOnclicke);
    }


    private void ButtonOnclicke(PointerEventData data)
    {
        if (BitUtils.GetBit64Count(TypeSelectionFormLogic.pickBookTag) >= 3 && !onClick)
        {
            return;
        }
        onClick = !onClick;

        if (onClick)
        {
            On.SetActive(true);
            Off.SetActive(false);
            TypeSelectionFormLogic.AddSelectionConditions(Index);
        }
        else
        {
            On.SetActive(false);
            Off.SetActive(true);
            TypeSelectionFormLogic.DeletSelectionConditions(Index);
        }
    }

    public void DestroyGameObject()
    {
        UIEventListener.RemoveOnClickListener(gameObject, ButtonOnclicke);
        if (gameObject!=null)
        {
            Destroy(gameObject);
        }
    }
}

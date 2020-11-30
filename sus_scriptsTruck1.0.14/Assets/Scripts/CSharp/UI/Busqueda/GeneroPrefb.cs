using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GeneroPrefb : MonoBehaviour
{

    private Text OnText, OffText;
    private BusquedaCtrl BusquedaCtrl;
    private GameObject Off,On;
    private bool Onclicke = false;
    private int Index;
    public void Init(string Name, BusquedaCtrl BusquedaCtrl,int index)
    {
        this.BusquedaCtrl = BusquedaCtrl;
        FindGameObject();

        Index = index;

        Onclicke = false;
        OnText.text = Name.ToString();
        OffText.text = Name.ToString();

        UIEventListener.AddOnClickListener(gameObject,ButtonOnclicke);
    }

    private void FindGameObject()
    {
        OnText = transform.Find("ON/Text").GetComponent<Text>();
        OffText = transform.Find("Off/Text").GetComponent<Text>();
        Off = transform.Find("Off").gameObject;
        On = transform.Find("ON").gameObject;
    }

    private void ButtonOnclicke(PointerEventData data)
    {
        if (BitUtils.GetBit64Count(BusquedaCtrl.BusquedaLogic.tagFlag) >= 3 && !Onclicke)
        {
            return;
        }
        Onclicke =! Onclicke;
        if (Onclicke)
        {
            On.SetActive(true);
            Off.SetActive(false);

            BusquedaCtrl.BusquedaLogic.GetBusquedaBooke(Index);

        }
        else
        {
            On.SetActive(false);
            Off.SetActive(true);

            BusquedaCtrl.BusquedaLogic.RemoveBookId(Index);

        }

    }

    public void DestryGameObject()
    {
        UIEventListener.RemoveOnClickListener(gameObject, ButtonOnclicke);


        if (gameObject != null)
            Destroy(gameObject);
    }
}

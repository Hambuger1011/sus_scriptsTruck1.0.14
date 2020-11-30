using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComuniadaModle
{
   public ComuniadaModle(ComuniadaCtrl ComuniadaCtrl)
    {
        topButtonOn = new List<GameObject>();
        topButtonOff = new List<GameObject>();
    }

    private List<GameObject> topButtonOn;
    private List<GameObject> topButtonOff;

    
    public List<GameObject> TopButtonOn
    {
        get
        {
            return topButtonOn != null? topButtonOn : null;
        }
    }

    public List<GameObject> TopButtonOff
    {
        get
        {
            return topButtonOff != null ? topButtonOff : null;
        }
    }

}

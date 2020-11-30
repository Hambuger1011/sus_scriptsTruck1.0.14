using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusquedaMange
{
    private static BusquedaMange instance;

    public static BusquedaMange Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new BusquedaMange();
            }
            return instance;
        }
    }
}

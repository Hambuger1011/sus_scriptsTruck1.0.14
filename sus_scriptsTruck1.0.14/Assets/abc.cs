using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine.U2D;

public class abc : MonoBehaviour{

    public SpriteAtlas atlas;

    [ContextMenu("Test")]
	void test()
    {
        Sprite sprite = atlas.GetSprite("0001");
        Debug.LogError(sprite);

    }
}

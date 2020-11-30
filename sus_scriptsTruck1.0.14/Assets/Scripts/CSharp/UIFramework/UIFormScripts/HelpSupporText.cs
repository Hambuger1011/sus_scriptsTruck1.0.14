using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpSupporText : MonoBehaviour {

    private ContentSizeFitter con;
	// Use this for initialization
	void Start () {
        con = transform.GetComponent<ContentSizeFitter>();
        
        Invoke("ConFals", 0.1f);
    }
	
    void ConFals()
    {
        CancelInvoke();
        con.enabled = false;
    }
	
}

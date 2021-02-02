using System.Collections;
using System.Collections.Generic;
using Candlelight.UI;
using UnityEngine;

public class JumpURL : MonoBehaviour
{
    public HyperText hypertext;
    public string _url = "";
    private void Start()
    {
        hypertext.ClickedLink.AddListener(OnClickLink);
    }
    public void OnClickLink(HyperText ht, HyperText.LinkInfo info)
    {
        Debug.Log("_url_url_url:" + _url);
        Application.OpenURL(_url);
    }

   
}

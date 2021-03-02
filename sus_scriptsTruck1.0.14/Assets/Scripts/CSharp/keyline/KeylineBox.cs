using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeylineBox : MonoBehaviour
{
    private Animator anima;

    public bool isPlay = false;
    
    public void Awake()
    {
        anima = this.GetComponent<Animator>();
    }

    // 开始接触
    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("开始接触");
        Debug.Log(collider.name);
        anima.enabled = true;
        if (isPlay)
        {
            anima.Play("Keyline", 0, 0);
        }
    }

}

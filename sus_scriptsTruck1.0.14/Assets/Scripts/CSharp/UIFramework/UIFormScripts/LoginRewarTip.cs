using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class LoginRewarTip : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Text AwardText;
    public void Inite(string awardName,int Number)
    {
        AwardText.text = "Get " + awardName + " " + Number;

        Tween t = transform.DOLocalMoveY(300, 2f);
        t.OnComplete(GameFalse);
    }

    private void GameFalse()
    {
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        GameFalse();
        //Debug.Log("关闭");
    }
}

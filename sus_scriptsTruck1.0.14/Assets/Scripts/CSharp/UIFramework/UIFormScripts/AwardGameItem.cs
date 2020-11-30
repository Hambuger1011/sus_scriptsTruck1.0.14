using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UGUI;

public class AwardGameItem : MonoBehaviour {

	// Use this for initialization
	void Start () {
        rect = transform.GetComponent<RectTransform>();

    }

    public Text Number;
    public Image SpriteImage;
    private int Type;
    private RectTransform rect;


    public void IniteGame(int vRewardType, int Number)
    {
        this.Number.text = "x" + Number;
        Type = vRewardType;
        if (vRewardType == 1)
        {
            string pathFront = "LoginToRewardUiForm/bg_kamex_04";
            SpriteImage.sprite = ResourceManager.Instance.GetUISprite(pathFront);
           
        }
        else
        {
            string pathFront = "LoginToRewardUiForm/bg_jlame_03";
            SpriteImage.sprite = ResourceManager.Instance.GetUISprite(pathFront);
        }

        //Invoke("GamoOverAwardGame", 2f);
    }
    private void GamoOverAwardGame()
    {
        CancelInvoke();

        if (Type==3)
        {
            //这个是阅读章节结束的时候获得钻石
#if !NOT_USE_LUA
            var Topbar = GameObject.FindObjectOfType<BookReadingFormTopBarController>();
            Topbar.touchAreaOnclick();
#else
            CUIManager.Instance.GetForm<BookReadingForm>(UIFormName.BookReadingForm).Topbar.touchAreaOnclick();
#endif
            rect.DOScale(new Vector3(0, 0, 0), 0.5f);
            rect.DOAnchorPos(new Vector3(268, 640, 0), 0.5f).OnComplete(() =>
            {

                UserDataManager.Instance.CalculateDiamondNum(1);
                Destroy(gameObject);
            });
        }
    }
}


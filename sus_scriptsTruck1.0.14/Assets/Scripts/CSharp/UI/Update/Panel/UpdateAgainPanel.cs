using System.Collections;using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateAgainPanel : MonoBehaviour
{
    public GameObject BgObj;
    private Text Title;
    private Text ContentText;
    private Button BtnCancel;
    private Button BtnAgain;

    private void Awake()
    {
        this.BgObj = DisplayUtil.GetChild(this.gameObject, "BgObj");
        this.Title = DisplayUtil.GetChildComponent<Text>(this.gameObject, "Title");
        this.ContentText = DisplayUtil.GetChildComponent<Text>(this.gameObject, "ContentText");
        this.BtnCancel = DisplayUtil.GetChildComponent<Button>(this.gameObject, "BtnCancel");
        this.BtnAgain = DisplayUtil.GetChildComponent<Button>(this.gameObject, "BtnAgain");


        this.BtnCancel.onClick.AddListener(OnBtnCancelClick);
        this.BtnAgain.onClick.AddListener(OnBtnAgainClick);
    }

    public void onClose()
    {
        this.BgObj.SetActiveEx(false);
    }


    private void OnBtnCancelClick()
    {

    }

    private void OnBtnAgainClick()
    {

    }
}

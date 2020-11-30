using System.Collections;using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateFailPanel : MonoBehaviour
{
    public GameObject BgObj;
    private Text Title;
    private Text ContentText;
    private Button BtnOk;

    private void Awake()
    {
        this.BgObj = DisplayUtil.GetChild(this.gameObject, "BgObj");
        this.Title = DisplayUtil.GetChildComponent<Text>(this.gameObject, "Title");
        this.ContentText = DisplayUtil.GetChildComponent<Text>(this.gameObject, "ContentText");
        this.BtnOk = DisplayUtil.GetChildComponent<Button>(this.gameObject, "BtnOk");


        this.BtnOk.onClick.AddListener(OnBtnOKClick);
    }

    private void OnBtnOKClick()
    {

    }

    public void onClose()
    {
        this.BgObj.SetActiveEx(false);
    }

}
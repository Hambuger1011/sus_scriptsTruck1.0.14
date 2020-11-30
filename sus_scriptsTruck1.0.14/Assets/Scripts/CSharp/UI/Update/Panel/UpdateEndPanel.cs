using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.UI;

public class UpdateEndPanel : MonoBehaviour
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

    private bool isBtn = false;

    private void OnBtnOKClick()
    {
        if (isBtn == true) return;
        isBtn = true;
        //-------------------------------------加载结束后的操作
        //存储所有需要到的文本，内容
        GameDataMgr.Instance.table.GetLocalizationData();

        SdkMgr.Instance.Start();

        UIUpdateModule updateUI = CUIManager.Instance.GetForm<UIUpdateModule>(UIFormName.UIUpdateModule);
        if (updateUI != null)
        {
            //回调lua 加载结束方法
            updateUI.LoadingonComplete();
        }
        //-------------------------------------加载结束后的操作
    }


    public void onClose()
    {
        this.BgObj.SetActiveEx(false);
    }

}

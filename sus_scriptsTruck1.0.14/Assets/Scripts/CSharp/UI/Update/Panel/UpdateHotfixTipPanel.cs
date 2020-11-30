using Framework;
using UGUI;
using UnityEngine;
using UnityEngine.UI;

public class UpdateHotfixTipPanel : MonoBehaviour
{
    public GameObject BgObj;
    private Text Title;
    private Text ContentText;
    private Button BtnUpdate;


    private void Awake()
    {
        this.BgObj = DisplayUtil.GetChild(this.gameObject, "BgObj");
        this.Title = DisplayUtil.GetChildComponent<Text>(this.gameObject, "Title");
        this.ContentText = DisplayUtil.GetChildComponent<Text>(this.gameObject, "ContentText");
        this.BtnUpdate = DisplayUtil.GetChildComponent<Button>(this.gameObject, "BtnUpdate");


        this.BtnUpdate.onClick.AddListener(OnBtnUpdateClick);
    }

    private void OnBtnUpdateClick()
    {
        //开始正常热更新
        XLuaHelper.isHotUpdate = false;

        //【==========================进入资源更新加载场景==========================】
        CSingleton<CGameStateMgr>.GetInstance().GotoState<CLoadingState>();
        AndroidUtils.CloseSplash();

        this.BgObj.SetActiveEx(false);
        CUIManager.Instance.CloseForm(UIFormName.UIUpdateModule);


        //当前版本号；
        string curResVersion = PlayerPrefs.GetString("CurResVersion");

        if (!string.IsNullOrEmpty(curResVersion))
        {
            PlayerPrefs.SetString("OldResVersion", curResVersion);
        }
    }

    public void onClose()
    {
        this.BgObj.SetActiveEx(false);
    }


}

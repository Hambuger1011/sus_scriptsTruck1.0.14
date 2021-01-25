using System;
using AB;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLua;


public enum EnumUpdate
{
    None,
    //系统公告     界面
    SystemNoticePanel,
    //系统维护中   界面
    MaintenancePanel,
    //维护完成需要更新 界面
    UpdateTipPanel,
    //强制提示需要热更
    UpdateHotfixTipPanel,
    //资源更新中   界面
    UpdateRunningPanel,
    //更新已完成   界面
    UpdateEndPanel,
    //更新失败   界面
    UpdateFailPanel,
    //继续下载   界面
    UpdateAgainPanel,
}


[LuaCallCSharp]
public class UIUpdateModule : BaseUIForm
{
    //系统公告 界面
    private SystemNoticePanel mSystemNoticePanel;
    //系统维护中 界面
    private MaintenancePanel mMaintenancePanel;
    //维护完成 需要更新 界面
    private UpdateTipPanel mUpdateTipPanel;
    //强制提示需要热更 界面
    private UpdateHotfixTipPanel mUpdateHotfixTipPanel;
    //资源更新中 界面
    private UpdateRunningPanel mUpdateRunningPanel;
    //更新已完成 界面
    private UpdateEndPanel mUpdateEndPanel;
    //更新失败 界面
    private UpdateFailPanel mUpdateFailPanel;
    //继续下载 界面
    private UpdateAgainPanel mUpdateAgainPanel;


    private EnumUpdate CurUI = EnumUpdate.None;

    private GameObject FeedbackButton;

    private Image loadImage;
    protected override void Awake()
    {
        base.Awake();



        this.FeedbackButton = DisplayUtil.GetChild(this.gameObject, "FeedbackButton");
        this.mSystemNoticePanel = DisplayUtil.GetChild(this.gameObject, "SystemNoticePanel").AddComponent<SystemNoticePanel>();
        this.mMaintenancePanel = DisplayUtil.GetChild(this.gameObject, "MaintenancePanel").AddComponent<MaintenancePanel>();
        this.mUpdateTipPanel = DisplayUtil.GetChild(this.gameObject, "UpdateTipPanel").AddComponent<UpdateTipPanel>();
        this.mUpdateHotfixTipPanel = DisplayUtil.GetChild(this.gameObject, "UpdateHotfixTipPanel").AddComponent<UpdateHotfixTipPanel>();
        this.mUpdateRunningPanel = DisplayUtil.GetChild(this.gameObject, "UpdateRunningPanel").AddComponent<UpdateRunningPanel>();
        this.mUpdateEndPanel = DisplayUtil.GetChild(this.gameObject, "UpdateEndPanel").AddComponent<UpdateEndPanel>();
        this.mUpdateFailPanel = DisplayUtil.GetChild(this.gameObject, "UpdateFailPanel").AddComponent<UpdateFailPanel>();
        this.mUpdateAgainPanel = DisplayUtil.GetChild(this.gameObject, "UpdateAgainPanel").AddComponent<UpdateAgainPanel>();
        this.loadImage = GameObject.Find("UIRect/BG").GetComponent<Image>();
        
        var imgVersion = PlayerPrefs.GetString("LoadImageVersion");
        if (!string.IsNullOrEmpty(imgVersion))
            this.loadImage.sprite = XLuaHelper.LoadSprite(string.Format("{0}cache/book/loading/0.jpg", GameUtility.WritablePath, 0));
    }

    public override void OnOpen()
    {
        base.OnOpen();

        UIEventListener.AddOnClickListener(FeedbackButton, FeedbackButtonOnClick);

    }

    public override void OnClose()
    {
        base.OnClose();
        CurUI = EnumUpdate.None;
        UIEventListener.RemoveOnClickListener(FeedbackButton, FeedbackButtonOnClick);
        this.hidePanelAll();
    }



    private void FeedbackButtonOnClick(PointerEventData data)
    {
        IGGSDKManager.Instance.OpenTSH();
    }



    public void SetPanel(EnumUpdate updatePanel,string content="",string _time="",int _isforceUpdate=0)
    {
        CurUI = updatePanel;
        hidePanelAll();
        switch (updatePanel)
        {
            case EnumUpdate.SystemNoticePanel:   //系统公告 界面
                this.mSystemNoticePanel.BgObj.SetActiveEx(true);
                this.mSystemNoticePanel.SetData(content);
                break;
            case EnumUpdate.MaintenancePanel:    //系统维护中 界面

                this.mMaintenancePanel.BgObj.SetActiveEx(true);
                this.mMaintenancePanel.SetData(content, _time);
                break;
            case EnumUpdate.UpdateTipPanel:     //维护完成 需要更新 界面

                this.mUpdateTipPanel.BgObj.SetActiveEx(true);
                this.mUpdateTipPanel.SetData(content, _isforceUpdate);
                break;
            case EnumUpdate.UpdateHotfixTipPanel:     //强制提示需要热更 界面
                this.mUpdateHotfixTipPanel.BgObj.SetActiveEx(true);
                break;
            case EnumUpdate.UpdateRunningPanel:   //资源更新中 界面
                this.mUpdateRunningPanel.BgObj.SetActiveEx(true);
                this.mUpdateRunningPanel.SetData();
                break;
            case EnumUpdate.UpdateEndPanel:     //更新已完成 界面
                this.mUpdateEndPanel.BgObj.SetActiveEx(true);
                break;
            case EnumUpdate.UpdateFailPanel:    //更新失败 界面
                this.mUpdateFailPanel.BgObj.SetActiveEx(true);
                break;
            case EnumUpdate.UpdateAgainPanel:    //继续下载 界面
                this.mUpdateAgainPanel.BgObj.SetActiveEx(true);
                break;
        }
    }

    public override void CustomUpdate()
    {
        base.CustomUpdate();

        if (CurUI == EnumUpdate.UpdateRunningPanel)
        {
            this.mUpdateRunningPanel.CustomUpdate();
        }


    }

    public void UpdateMaintinTime(int _time)
    {
        if (CurUI == EnumUpdate.MaintenancePanel)
        {
            this.mMaintenancePanel.BgObj.SetActiveEx(true);
            this.mMaintenancePanel.UpdateMaintinTime(_time);
        }
    }


    public void StartLoading2(Action<bool> callback)
    {
        if (CurUI == EnumUpdate.UpdateRunningPanel)
        {
            this.mUpdateRunningPanel.StartLoading2(callback);
        }
    }

    public void LoadingonComplete()
    {
        if (this.mUpdateRunningPanel.onComplete != null)
        {
            this.mUpdateRunningPanel.onComplete(true);
        }
    }

    public void hidePanelAll()
    {
        this.mSystemNoticePanel.onClose();
        this.mMaintenancePanel.onClose();
        this.mUpdateTipPanel.onClose();
        this.mUpdateRunningPanel.onClose();
        this.mUpdateEndPanel.onClose();
        this.mUpdateFailPanel.onClose();
        this.mUpdateAgainPanel.onClose();
    }

}

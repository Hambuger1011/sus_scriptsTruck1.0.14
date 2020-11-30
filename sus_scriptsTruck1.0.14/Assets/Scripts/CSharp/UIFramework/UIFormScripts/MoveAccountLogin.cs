using pb;
using System.Collections;
using System.Collections.Generic;
using GameCore.UGUI;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveAccountLogin : BaseUIForm
{
    public UITweenButton DeviceBtn;
    public UITweenButton FacebookBtn;
    public UITweenButton GoogleBtn;
    public Button UIMask;
    private GoogleSdk google = new GoogleSdk();
    public override void OnOpen()
    {
        base.OnOpen();
        UIMask.onClick.AddListener(UIMaskOnClick);
        switch (UserDataManager.Instance.logintType)
        {
           case 0:
               DeviceBtn.transform.Find("ToLogin").gameObject.SetActiveEx(false);
               DeviceBtn.transform.Find("Logged").gameObject.SetActiveEx(true);
               FacebookBtn.onClick.AddListener(FacebookBtnOnClick);
               GoogleBtn.onClick.AddListener(GoogleBtnOnClick);
               break;
           case 1:
               GoogleBtn.transform.Find("ToLogin").gameObject.SetActiveEx(false);
               GoogleBtn.transform.Find("Logged").gameObject.SetActiveEx(true);
               DeviceBtn.onClick.AddListener(DeviceBtnOnClick);
               FacebookBtn.onClick.AddListener(FacebookBtnOnClick);
               break;
           case 2:
               FacebookBtn.transform.Find("ToLogin").gameObject.SetActiveEx(false);
               FacebookBtn.transform.Find("Logged").gameObject.SetActiveEx(true);
               DeviceBtn.onClick.AddListener(DeviceBtnOnClick);
               GoogleBtn.onClick.AddListener(GoogleBtnOnClick);
               break;
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        UIMask.onClick.RemoveListener(UIMaskOnClick);
        DeviceBtn.onClick.RemoveListener(DeviceBtnOnClick);
        FacebookBtn.onClick.RemoveListener(FacebookBtnOnClick);
        GoogleBtn.onClick.RemoveListener(GoogleBtnOnClick);
    }

    private void UIMaskOnClick()
    {
        CUIManager.Instance.CloseForm(UIFormName.MoveAccountLogin);
    }

    private void GoogleBtnOnClick()
    {
        google.Start();
        SdkMgr.Instance.GoogleLogin(1);
        CUIManager.Instance.CloseForm(UIFormName.MoveAccountLogin);
    }

    private void FacebookBtnOnClick()
    {
        SdkMgr.Instance.FacebookLogin(1);
        CUIManager.Instance.CloseForm(UIFormName.MoveAccountLogin);
    }

    private void DeviceBtnOnClick()
    {
        if (SdkMgr.Instance.google.IsLogin())
            SdkMgr.Instance.google.Logout();
        if (SdkMgr.Instance.facebook.IsLogin())
            SdkMgr.Instance.facebook.Logout();
        GameHttpNet.Instance.TOKEN = string.Empty;
        GameHttpNet.Instance.TouristLogin(TouristLoginCallBacke);
        CUIManager.Instance.CloseForm(UIFormName.MoveAccountLogin);
    }
    
    private void TouristLoginCallBacke(object arg)
    {
        string result = arg.ToString();
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            if (jo.code == 200)
            {
                HttpInfoReturn<TouristLoginInfo> TouristLoginInfo = JsonHelper.JsonToObject<HttpInfoReturn<TouristLoginInfo>>(result);
                GameHttpNet.Instance.TOKEN = TouristLoginInfo.data.token;
                EventDispatcher.Dispatch(EventEnum.DeviceLoginSucc, TouristLoginInfo);
            }
        }
    }
    
}

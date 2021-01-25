using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using UGUI;
using UnityEngine.UI;

/// <summary>
/// 选择服务器的窗口
/// </summary>
public class ServiceSelectForm : BaseUIForm
{
    
    public RectTransform FrameTrans;
    public Button BtnOK;

  
    public InputField IdInput;
    public InputField TokenInput;

    public Dropdown ServicePath, ResourcePath;

    private List<string> ServicePathList, ResourcePathList;
    private Action mCallBack;

    public InputField mCustomSvr, mCustomRes;

    public Toggle UserLocalAddress,UserLocalVersion;
    public InputField VersionInputField;


    public override void OnOpen()
    {
        base.OnOpen();
        this.BtnOK.onClick.AddListener(OnOkClick);

        if (GameUtility.IpadAspectRatio() && FrameTrans != null)
            FrameTrans.localScale = Vector3.one * 0.7f;

#if CHANNEL_SPAIN
        SerTecentDebug.isOn = true;
        ResTecentDebug.isOn = true;
#endif
        IdInput.text = PlayerPrefs.GetString(GameHttpNet.UUID_LOCAL_FLAG, "");
        TokenInput.text = GameHttpNet.Instance.TOKEN;
        VersionInputField.text = GameDataMgr.Instance.LocalVersion;

        ServicePath.onValueChanged.AddListener(OnServiceChange);
        ResourcePath.onValueChanged.AddListener(OnResourcesChange);
        mCustomSvr.gameObject.CustomSetActive(false);
        mCustomRes.gameObject.CustomSetActive(false);

        GameDataMgr.Instance.UserLocalAddress = false;
        UserLocalAddress.isOn = false;
        UserLocalAddress.onValueChanged.AddListener(UserLocalAddressChanged);

        GameDataMgr.Instance.UserLocalVersion = false;
        UserLocalVersion.isOn = false;
        UserLocalVersion.onValueChanged.AddListener(UserLocalVersionChanged);

        ServicePathDropdownAdd();
        ResourcePathDropdownAdd();

    }
    
    private void UserLocalAddressChanged(bool isOn)
    {
        GameDataMgr.Instance.UserLocalAddress = isOn;
    }
    
    private void UserLocalVersionChanged(bool isOn)
    {
        GameDataMgr.Instance.UserLocalVersion = isOn;
    }

    private void OnServiceChange(int index)
    {
        if(index == 5)
        {
            mCustomSvr.gameObject.CustomSetActive(true);

            mCustomSvr.text = PlayerPrefs.GetString("mCustomSvr", "http://new.onyxgames1.com");
        }
        else
        {
            mCustomSvr.gameObject.CustomSetActive(false);
        }
    }
    private void OnResourcesChange(int index)
    {
        if (index == 5)
        {
            mCustomRes.gameObject.CustomSetActive(true);
            mCustomRes.text = PlayerPrefs.GetString("mCustomRes", "http://d3da55fv9mg213.cloudfront.net");

        }
        else
        {
            mCustomRes.gameObject.CustomSetActive(false);
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        this.BtnOK.onClick.RemoveListener(OnOkClick);
        UserLocalAddress.onValueChanged.RemoveListener(UserLocalAddressChanged);
        UserLocalVersion.onValueChanged.RemoveListener(UserLocalVersionChanged);
    }

    /// <summary>
    /// 数据服务器类型增减
    /// </summary>
    private void ServicePathDropdownAdd()
    {
        if (ServicePathList == null)
            ServicePathList = new List<string>();

        ServicePathList.Add("Developing (开发服)");
        ServicePathList.Add("TencentDeveloping (腾讯服)");
        ServicePathList.Add("Debug (测试服)");
        ServicePathList.Add("Release (正式服)");
        ServicePathList.Add("Alan (Alan本机服务器)");
        ServicePathList.Add("手动输入");

        ServicePath.AddOptions(ServicePathList);
        
        ServicePath.value = 0;
    }

    /// <summary>
    /// 资源服务器类型增减
    /// </summary>
    private void ResourcePathDropdownAdd()
    {
        if (ResourcePathList == null)
            ResourcePathList = new List<string>();

        ResourcePathList.Add("LocalRes(本地资源)");
        ResourcePathList.Add("DevelopingRes(开发服资源)");
        ResourcePathList.Add("TecentDevelopingRes(腾讯服资源)");
        ResourcePathList.Add("DebugRes(测试服资源)");
        ResourcePathList.Add("ReleaseRes(正式服资源)");
        ResourcePathList.Add("手动输入");

        ResourcePath.AddOptions(ResourcePathList);

        ResourcePath.value = 1;
    }

    private void OnOkClick()
    {
#if ENABLE_DEBUG

        switch (ServicePath.value)
        {
            case 0:
                GameDataMgr.Instance.ServiceType = 1;
                break;
            case 1:
                GameDataMgr.Instance.ServiceType = 2;
                break;
            case 2:
                GameDataMgr.Instance.ServiceType = 3;
                break;
            case 3:
                GameDataMgr.Instance.ServiceType = 4;
                break;
            case 4:
                GameDataMgr.Instance.ServiceType = 5;
                break;
            case 5:
                GameDataMgr.Instance.ServiceType = 6;
                var url = mCustomSvr.text.Trim();
                if(!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    url = "http://" + url;
                }
                PlayerPrefs.SetString("mCustomSvr", url);
                break;
        }

        switch (ResourcePath.value)
        {
            case 0:
                GameDataMgr.Instance.ResourceType = 0;
                break;
            case 1:
                GameDataMgr.Instance.ResourceType = 1;
                break;
            case 2:
                GameDataMgr.Instance.ResourceType = 2;
                break;
            case 3:
                GameDataMgr.Instance.ResourceType = 3;
                break;
            case 4:
                GameDataMgr.Instance.ResourceType = 4;
                break;
            case 5:
                GameDataMgr.Instance.ResourceType = 5;
                var url = mCustomRes.text.Trim();
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    url = "http://" + url;
                }
                PlayerPrefs.SetString("mCustomRes", url);
                break;

        }

       
        if (IdInput != null && !string.IsNullOrEmpty(IdInput.text) && IdInput.text.Length > 10 )
        {
            GameHttpNet.Instance.UUID = ReplaceSpaceTxt(IdInput.text);
            //PlayerPrefs.SetString("DEBUG_UUID", GameHttpNet.Instance.UUID);
        }
        else
        {
            PlayerPrefs.SetString(GameHttpNet.UUID_LOCAL_FLAG, "");
        }

        if (TokenInput != null && !string.IsNullOrEmpty(TokenInput.text) && TokenInput.text.Length > 5)
        {
            GameHttpNet.Instance.TOKEN = ReplaceSpaceTxt(TokenInput.text);
        }

        if (VersionInputField != null && !string.IsNullOrEmpty(VersionInputField.text))
        {
            GameDataMgr.Instance.LocalVersion = ReplaceSpaceTxt(VersionInputField.text);
        }
#endif
        DoResult(true);
    }

    private string ReplaceSpaceTxt(string value)
    {
        string result = value.Replace("\r\n","");
        result = value.Replace("\r", "");
        result = value.Replace("\n", "");
        result = result.Replace(" ", "");
        return result;
    }


    private void DoResult(bool value)
    {
        if (mCallBack != null)
            mCallBack();
        CUIManager.Instance.CloseForm(UIFormName.ServiceSelectForm);
    }

    public void SetCallBack(Action vCallBack)
    {
        mCallBack = vCallBack;
    }

}

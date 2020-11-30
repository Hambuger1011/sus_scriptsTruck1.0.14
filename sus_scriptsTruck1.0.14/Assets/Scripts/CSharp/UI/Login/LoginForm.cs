using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using Framework;
using Helper.Account;
using Helper.Login;
using IGG.SDK.Modules.Account;
using IGG.SDK.Modules.Account.VO;
using IGGUtils;

public class LoginForm : BaseUIForm
{
    public Button btnClose, btnFacebookLogin, btnGoogleLogin, btnIGGLogin, btnAppleLogin, btnDeviceLogin,Mask;

    private int mCurLoginChannel = 0;

    private bool IsTimeOutOpen = false; //是不是因为登录账号过期才打开的界面

    private IGGLoginType lastLoginType;

    public override void OnOpen()
    {
        base.OnOpen();
        IsTimeOutOpen = false;
        UserDataManager.Instance.SigningIn = true;

        btnClose.onClick.AddListener(OnCloseClick);
        btnFacebookLogin.onClick.AddListener(OnFacebookLoginClick);
        btnDeviceLogin.onClick.AddListener(btnDeviceLoginOnclicke);
        btnIGGLogin.onClick.AddListener(OnIGGLoginClick);
        btnGoogleLogin.gameObject.SetActive(false);
        btnAppleLogin.gameObject.SetActive(false);

        lastLoginType = AccountUtil.ReadLastLoginType(); // 获取上次登录方式（本接口不是USDK提供，由游戏自行实现，USDKDemo仅供参考）
        switch (lastLoginType)
        {
            case IGGLoginType.Guest:
                btnDeviceLogin.transform.Find("Image").gameObject.SetActiveEx(true);
                break;
            case IGGLoginType.IGGAccount:
                btnIGGLogin.transform.Find("Image").gameObject.SetActiveEx(true);
                break;
            case IGGLoginType.Facebook:
                btnFacebookLogin.transform.Find("Image").gameObject.SetActiveEx(true);
                break;
            case IGGLoginType.GooglePlus:
                btnGoogleLogin.transform.Find("Image").gameObject.SetActiveEx(true);
                break;
            case IGGLoginType.Apple:
                btnAppleLogin.transform.Find("Image").gameObject.SetActiveEx(true);
                break;
        }
#if UNITY_ANDROID
        // btnGoogleLogin.onClick.AddListener(OnGoogleLoginClick);
        // btnGoogleLogin.gameObject.SetActive(true);
#elif UNITY_IOS
        btnAppleLogin.onClick.AddListener(OnAppleLoginClick);
        btnAppleLogin.gameObject.SetActive(true);
#endif

        Transform bgTrans = this.gameObject.transform.Find("Frame");
        if (GameUtility.IpadAspectRatio() && bgTrans != null)
            bgTrans.localScale = Vector3.one * 0.7f;

        //关闭红点定时器
        if (XLuaManager.Instance != null)
        {
            if (XLuaManager.Instance.GetLuaEnv() != null)
            {
                XLuaManager.Instance.GetLuaEnv().DoString(@"GameController.MainFormControl:ClearTimer();");
            }
        }
       
        addMessageListener(EventEnum.SwitchLoginFailed, HideMask);
    }
    

    /// <summary>
    /// 这里标记，打开这个界面是因为登录账号过期了
    /// </summary>
    public void IsTimeOutOpenFanc()
    {
        LOG.Info("账号过期了");
        IGGSDKManager.Instance.isTokenExpired = true;
        IGGSession.currentSession.InvalidateCurrentSession();
        IsTimeOutOpen = true;
    }


    /// <summary>
    /// 这里是显示出界面关闭按钮
    /// </summary>
    public void btnCloseToTrue()
    {
        btnClose.gameObject.SetActive(true);
    }

    private void OnCloseClick()
    {
        if (this.myForm != null)
            this.myForm.Close();


        btnClose.onClick.RemoveListener(OnCloseClick);
        btnFacebookLogin.onClick.RemoveListener(OnFacebookLoginClick);
        btnGoogleLogin.onClick.RemoveListener(OnGoogleLoginClick);
        btnDeviceLogin.onClick.RemoveListener(btnDeviceLoginOnclicke);
    }


    private AccountHelper helper = new AccountHelper();

    
    private void ShowMask()
    {
        Mask.gameObject.SetActiveEx(true);
    }
    
    private void HideMask(Notification vNot)
    {
        if (null != this)
        {
            Mask.gameObject.SetActiveEx(false);
        }
    }
    
    private void OnGoogleLoginClick()
    {
        ShowMask();
        // CTimerManager.Instance.AddTimer(1000, 1, HideMask);
        if (lastLoginType == IGGLoginType.GooglePlus)
        {
            IGGSDKManager.Instance.OnLastLoginTypeClick();
            return;
        }

        IGGSDKManager.Instance.ExpiredGoogleLogin();
        // IGGNativeUtils.ShareInstance().FetchGooglePlayToken(((bool var1, string token) =>
        // {
        //     if (var1)
        //     {
        //         // Google账号登录（登录前会检测当前Google账号是否已经绑定了IGGID，根据绑定情况会做相应登录操作）。
        //         helper.CheckCandidateByGoogleAccount(token, IGGGoogleAccountTokenType.IdToken, new OnCheckCandidateByThirdAccountListenr(OnUnbindByThirdAccount, OnLoginSuccess, OnFailed));
        //     }
        //     else
        //     {
        //         UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(378));
        //     }
        //
        // }));
    }

    private void OnFacebookLoginClick()
    {
        ShowMask();
        if (lastLoginType == IGGLoginType.Facebook)
        {
            IGGSDKManager.Instance.OnLastLoginTypeClick();
            return;
        }

        IGGSDKManager.Instance.ExpiredFacebookLogin();
        // FacebookUtil.Login(delegate (string token)
        //     {
        //         // FB登录（登录前会检测当前FB账号是否已经绑定了IGGID，根据绑定情况会做相应登录操作）。
        //         helper.CheckCandidateByFacebookAccount(token, new OnCheckCandidateByThirdAccountListenr(OnUnbindByThirdAccount, OnLoginSuccess, OnFailed));
        //     },
        //     delegate ()
        //     {
        //         UITipsMgr.Instance.ShowTips(CTextManager.Instance.GetText(377));
        //     });
    }

    private void OnIGGLoginClick()
    {
        ShowMask();
        if (lastLoginType == IGGLoginType.IGGAccount)
        {
            IGGSDKManager.Instance.OnLastLoginTypeClick();
            return;
        }

        IGGSDKManager.Instance.ExpiredIGGAccountLogin();
        // // IGG通行证登录（登录前会检测当前IGG通行证账号是否已经绑定了IGGID，根据绑定情况会做相应登录操作）。
        // helper.CheckCandidateByIGGAccount(new OnCheckCandidateByIGGAccountListenr(OnUnbindByIGGAccount, OnLoginSuccess, OnFailed));
    }

    private void OnAppleLoginClick()
    {
        ShowMask();
        if (lastLoginType == IGGLoginType.Apple)
        {
            IGGSDKManager.Instance.OnLastLoginTypeClick();
            return;
        }

        IGGSDKManager.Instance.ExpiredAppleLogin();
        // //Apple登录（登录前会检测当前Apple账号是否已经绑定了IGGID，根据绑定情况会做相应登录操作）。
        // helper.CheckCandidateByApple(new OnCheckCandidateByAppleListenr(OnUnbindByApple, OnLoginSuccess, OnFailed));
    }

    public void btnDeviceLoginOnclicke()
    {
        ShowMask();
        if (lastLoginType == IGGLoginType.Guest)
        {
            IGGSDKManager.Instance.OnLastLoginTypeClick();
            return;
        }

        IGGSDKManager.Instance.ExpiredDeviceLogin();
        // // 设备登录（登录前会检测当前设备是否已经绑定了IGGID，根据绑定情况会做相应登录操作）。
        // helper.CheckCandidateByGuest(new OnCheckCandidateByGuestListenr(OnUnbindByGuest, OnLoginSuccess, OnFailed));
    }
}
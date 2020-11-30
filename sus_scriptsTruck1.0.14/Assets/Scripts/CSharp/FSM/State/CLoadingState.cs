using System;
using Framework;
using FSM;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using XLua;
using Random = UnityEngine.Random;

[CGameStateMgr.GameState]
public class CLoadingState : GameStateBase
{
    private bool resIsLoaded = false;
    private bool isEnter = false;
    private bool dataIsReturn = false;
    private string endtime;

    private int mUserInfoCheckStateSqu = 0;

    private int mUserInfoSqu = 0;
    private int mSelfBookSqu = 0;

    private bool mIsNewUser = false;


    public override void OnStateEnter()
    {
        base.OnStateEnter();
        isEnter = false;
        resIsLoaded = false;
        dataIsReturn = false;

        UserDataManager.Instance.GetLoginInfoByLocal();  //获得用户上次是使用哪个渠道登录  
        UserDataManager.Instance.GetVersion();
        GameDataMgr.Instance.BaseResLoadFinish = false;
        CUIManager.Instance.OpenForm(UIFormName.PopupTipsForm);


        Debug.LogError("CLoadingState :开始打开加载页 curtime：" + DateTime.Now);

        if (XLuaHelper.isHotUpdate)
        {
            Debug.Log("10........XLuaHelper.isHotUpdate");
            UIUpdateModule updateUI = CUIManager.Instance.GetForm<UIUpdateModule>(UIFormName.UIUpdateModule);
            updateUI.SetPanel(EnumUpdate.UpdateRunningPanel);
        }
        else
        {
            CUIManager.Instance.OpenForm(UIFormName.LoadingForm);
        }

        SetLoadingContInfo(CTextManager.Instance.GetText(274));
        StartLoadResource();
    }
    
#if false
    private void CheckVersion()
    {
        AccountExpired.Instance.AddAction(CheckVersion);

        GameHttpNet.Instance.GetGameVersion(CheckVersionCallBack);
    }

    private void CheckVersionCallBack(long responseCode, string result)
    {
        LOG.Info("----CheckVersion---->" + result);
        if (responseCode != 200)
        {
            TalkingDataManager.Instance.OpenApp("VersionProtocolReturn - " + result);
            return;
        }
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            if (jo.code == 200)
            {

                TalkingDataManager.Instance.OpenApp("VersionProtocolReturn - " + jo.code);

                GameHttpNet.Instance.SetUserLanguage(SdkMgr.Instance.GameVersion(), 3, SetVersionCallBackHandler);

                UserDataManager.Instance.gameVersionInfo = JsonHelper.JsonToObject<HttpInfoReturn<VersionCont<GameVersionInfo>>>(result);
                if (UserDataManager.Instance.gameVersionInfo != null && UserDataManager.Instance.gameVersionInfo.data != null)
                {
                    GameVersionInfo versionInfo = UserDataManager.Instance.gameVersionInfo.data.apparr;
                    if (!CheckNeedDownNewVersion(versionInfo.versionid))
                    {
                        StartLoadResource();
                    }
                    else
                    {
                        TalkingDataManager.Instance.OpenApp("TipsToDownloadNewVersionAppByGooglePlay");


                        //UserDataManager.Instance.Version = int.Parse(versionInfo.versionid);
                        //UserDataManager.Instance.SaveVersion();
                        string tempTips = "Please go to the Play Store and download the latest version!";
#if UNITY_IOS
           tempTips = "Please go to the AppStore and download the latest version!";
#endif

                        UIAlertMgr.Instance.Show("New update available!", tempTips, AlertType.Sure, (value) =>
                        {
                            string linkUrl = "";
#if CHANNEL_HUAWEI
                            SdkMgr.Instance.hwSDK.CheckUpdate();
                            linkUrl = "https://appgallery.cloud.huawei.com/marketshare/app/C" + HuaweiSdk.HUAWEI_APP_ID;
#else
#if UNITY_ANDROID
                            linkUrl = "https://play.google.com/store/apps/details?id=" + SdkMgr.packageName;
#endif
#if UNITY_IOS
                                linkUrl = "itms-apps://itunes.apple.com/cn/app/id" + SdkMgr.IosAppId;
#endif
                            Application.OpenURL(linkUrl);
#endif
                        });
                        return;
                    }
                }
            }
            else if (jo.code == 277)
            {
                UIAlertMgr.Instance.Show("TIPS", jo.msg);
                return;
            }
        }

    }

    private void SetVersionCallBackHandler(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----SetVersionCallBackHandler---->" + result);
    }
#endif


    private void StartLoadResource()
    {
#if false
        loadingForm = CUIManager.Instance.GetForm<LaunchLoadingForm>(UIFormName.LoadingForm);
        loadingForm.IsNewPlayer = mIsNewUser;
        loadingForm.StartLoading(OnLoadFinish);
        SetLoadingContInfo("Loading game resources...");
#else
        //XLuaManager.Instance.Startup();
        GetTonken();
#endif
    }

    //分析玩家的ip，用于区分他的国家
    private void AnalyzeUserIp()
    {
        string ipAdress = GameUtility.GetPlayerIPAdress();
        if (!string.IsNullOrEmpty(ipAdress))
        {
            GameHttpNet.Instance.GetPlayerIpAdressInfo(ipAdress, IpAddressCallBackHandler);
        }
    }

    private void GetTonken()
    {
        Debug.Log("11.CLoadingState: GetTonken curtime：" + DateTime.Now);

        AccountExpired.Instance.AddAction(GetTonken);
        if (!GameHttpNet.Instance.UUID.Equals(PlayerPrefs.GetString(GameHttpNet.UUID_LOCAL_FLAG)))
        {
            //进入游戏 UUid 被修改则换成一个新的账号
            PlayerPrefs.SetString(GameHttpNet.UUID_LOCAL_FLAG, GameHttpNet.Instance.UUID);
            // GameHttpNet.Instance.TouristLogin(TouristLoginCallBacke);
            
            TalkingDataManager.Instance.LoginAccount(EventEnum.LoginTouristResultSucc);
            XLuaManager.Instance.Startup();
        }
        else if (string.IsNullOrEmpty(PlayerPrefs.GetString(GameHttpNet.TOKEN_GUEST_LOCAL_FLAG))&& (UserDataManager.Instance.LoginInfo.LastLoginChannel== 0||UserDataManager.Instance.LoginInfo.LastLoginChannel == 4))
        {
            PlayerPrefs.SetString(GameHttpNet.UUID_LOCAL_FLAG, GameHttpNet.Instance.UUID);
            //当目前是新手账号或者上一次是游客登录的时候，先调用接口获得游客登录的Tonken
            // GameHttpNet.Instance.TouristLogin(TouristLoginCallBacke);
            
            TalkingDataManager.Instance.LoginAccount(EventEnum.LoginTouristResultSucc);
            XLuaManager.Instance.Startup();
        }
        else
        {
            PlayerPrefs.SetString(GameHttpNet.UUID_LOCAL_FLAG, GameHttpNet.Instance.UUID);
            XLuaManager.Instance.Startup();
        }

       
    }

    private void TouristLoginCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----TouristLoginGetToken---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            if (jo.code == 200)
            {
                TalkingDataManager.Instance.LoginAccount(EventEnum.LoginTouristResultSucc);
                HttpInfoReturn<TouristLoginInfo> TouristLoginInfo = JsonHelper.JsonToObject<HttpInfoReturn<TouristLoginInfo>>(result);
                UserDataManager.Instance.LoginInfo.LastLoginChannel = 4;//游客首次登录，获得token
                GameHttpNet.Instance.TOKEN = TouristLoginInfo.data.token;             
                XLuaManager.Instance.Startup();
            }else
            {
                TalkingDataManager.Instance.LoginAccount(EventEnum.LoginTouristResultFail);
            }
        }
    }

    private void IpAddressCallBackHandler(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----IpAddressCallBackHandler---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            if (jo.code == 200)
            {
                HttpInfoReturn < IpStateInfo > stateInfo = JsonHelper.JsonToObject<HttpInfoReturn<IpStateInfo>>(result);
                if (stateInfo != null)
                {
                    if (stateInfo.data.status.Equals("success"))
                    {
                        UserDataManager.Instance.userIpAddressInfo = JsonHelper.JsonToObject<HttpInfoReturn<IpAdressInfo>>(result);
                        if (UserDataManager.Instance.userIpAddressInfo != null)
                        {
                            //UserDataManager.Instance.UserCountry = UserDataManager.Instance.userIpAddressInfo.data.countryCode;
                            //LOG.Info("-----country===>" + UserDataManager.Instance.userIpAddressInfo.data.country + "==countryId===>" + UserDataManager.Instance.userIpAddressInfo.data.countryCode);
                        }
                    }
                }
            } 
        }
        
        UserDataManager.Instance.AnalyzeUserIp();
    }

    private void SetLoadingContInfo(string vInfo)
    {
        var loadingForm = CUIManager.Instance.GetForm<LaunchLoadingForm>(UIFormName.LoadingForm);
        // if (String.IsNullOrEmpty(vInfo))
        // {
        //     int loadingTipsCount = GameDataMgr.Instance.table.LoadingTipsCount();
        //     int randomNum = Random.Range(1, loadingTipsCount);
        //     var tips = GameDataMgr.Instance.table.GetLoadingTipsById(randomNum);
        //     loadingForm.CurLoadContInfoTxt.text = tips.LoadingTips;
        //     return;
        // }

        if (loadingForm)
        {
            loadingForm.CurLoadContInfoTxt.text = vInfo;
            loadingForm.VersionInfoTxt.text = "V "+SdkMgr.Instance.GameVersion();
        }
    }
}

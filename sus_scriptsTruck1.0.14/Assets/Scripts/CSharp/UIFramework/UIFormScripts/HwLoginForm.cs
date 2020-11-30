using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UGUI;
using DG.Tweening;
using pb;

public class HwLoginForm  : BaseUIForm
{
    public GameObject btnIcon1;
    public GameObject btnIcon2;
    public GameObject btnIcon3;

    public CanvasGroup Group1;
    public CanvasGroup Group2;
    public CanvasGroup Group3;


    private bool isSelect;
    private int mCurLoginChannel = 0;
    private HwUserInfo hwUserInfo;

    private List<RecommendABookList> recommendList;
    public override void OnOpen()
    {
        base.OnOpen();

        addMessageListener(EventEnum.FaceBookLoginSucc, FaceBookLoginSuccHandler);
        addMessageListener(EventEnum.HuaweiLoginInfo, HuaweiLoginHandler);

        UIEventListener.AddOnClickListener(btnIcon1.gameObject, HuaweiClickHandler);
        UIEventListener.AddOnClickListener(btnIcon2.gameObject, FaceBookClickHandler);
        UIEventListener.AddOnClickListener(btnIcon3.gameObject, GuestClickHandler);

        if (Group1 != null)
        {
            Group1.alpha = Group2.alpha = Group3.alpha = 0;
            Group1.DOFade(1, 1f).Play();
            Group2.DOFade(1, 1f).SetDelay(0.1f).Play();
            Group3.DOFade(1, 1f).SetDelay(0.2f).Play();
        }
    }

    public override void OnClose()
    {
        base.OnClose();

        UIEventListener.RemoveOnClickListener(btnIcon1.gameObject, HuaweiClickHandler);
        UIEventListener.RemoveOnClickListener(btnIcon2.gameObject, FaceBookClickHandler);
        UIEventListener.RemoveOnClickListener(btnIcon3.gameObject, GuestClickHandler);
    }

    public void HuaweiClickHandler(UnityEngine.EventSystems.PointerEventData data)
    {
        LOG.Info("---HuaweiClickHandler---->");
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        mCurLoginChannel = 0;
        //UINetLoadingMgr.Instance.Show();
#if CHANNEL_HUAWEI
        SdkMgr.Instance.hwSDK.Login(0);
#endif
    }

    public void FaceBookClickHandler(UnityEngine.EventSystems.PointerEventData data)
    {
        LOG.Info("---FaceBookClickHandler---->");
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        mCurLoginChannel = 0;
        //UINetLoadingMgr.Instance.Show();
        SdkMgr.Instance.FacebookLogin(0);
    }

    public void GuestClickHandler(UnityEngine.EventSystems.PointerEventData data)
    {
        LOG.Info("---GuestClickHandler---->");
        GameHttpNet.Instance.TOKEN = string.Empty;
        mCurLoginChannel = 4;
        if (UserDataManager.Instance.LoginInfo != null)
            UserDataManager.Instance.LoginInfo.LastLoginChannel = mCurLoginChannel;
        UserDataManager.Instance.SaveLoginInfo();
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.GetUserInfo(GetUserInfoCallBack);
    }

    private void FaceBookLoginSuccHandler(Notification vNot)
    {
        LoginDataInfo loginInfo = vNot.Data as LoginDataInfo;

        if (loginInfo != null)
        {
            string userId = loginInfo.UserId;
            string tokenStr = loginInfo.Token;

            if (loginInfo.OpenType != 0) return;

            UserDataManager.Instance.UserData.UserID = userId;
            UserDataManager.Instance.UserData.IdToken = tokenStr;

            LOG.Info("---FaceBookLoginSuccHandler --userId-->" + userId + "===token--->" + tokenStr);

            SdkMgr.Instance.facebook.GetMyInfo(ReturnSelfFBInfo);

        }
    }

    private void ReturnSelfFBInfo(string vMsg)
    {
        LOG.Info("---return Self FB Info ---->" + vMsg);

        Dictionary<string, string> selfInfo = JsonHelper.JsonToObject<Dictionary<string, string>>(vMsg);
        string userId = string.Empty;
        string userNick = string.Empty;
        string email = string.Empty;
        string faceUrl = string.Empty;
        if (selfInfo != null)
        {
            if (selfInfo.ContainsKey("id"))
                userId = selfInfo["id"];
            if (selfInfo.ContainsKey("name"))
                userNick = selfInfo["name"];

            UserDataManager.Instance.UserData.UserName = userNick;

            if (UserDataManager.Instance.LoginInfo == null)
                UserDataManager.Instance.LoginInfo = new ThirdLoginData();

            mCurLoginChannel = 1;
            LoginDataInfo fbInfo = new LoginDataInfo();
            fbInfo.UserId = userId;
            fbInfo.UserName = userNick;
            fbInfo.Email = email;
            fbInfo.Token = UserDataManager.Instance.UserData.IdToken;
            fbInfo.UserImageUrl = faceUrl;
            UserDataManager.Instance.LoginInfo.FaceBookLoginInfo = fbInfo;

            LOG.Info("--- Self FB  --id-->" + userId + " userNick:" + userNick + " --token->" + UserDataManager.Instance.UserData.IdToken);

            //UINetLoadingMgr.Instance.Show();

#if UNITY_ANDROID
#if CHANNEL_HUAWEI
            GameHttpNet.Instance.LoginByThirdParty(1, email, userNick, faceUrl, userId, UserDataManager.Instance.UserData.IdToken, "huawei", LoginByThirdPartyCallBack);
#else
            GameHttpNet.Instance.LoginByThirdParty(1,email,userNick,faceUrl,userId,UserDataManager.Instance.UserData.IdToken,"android",LoginByThirdPartyCallBack);
#endif
#else
            GameHttpNet.Instance.LoginByThirdParty(1,email,userNick,faceUrl,userId,UserDataManager.Instance.UserData.IdToken,"ios",LoginByThirdPartyCallBack);
#endif

        }
    }

    private void LoginByThirdPartyCallBack(object arg, EnumReLogin loginType)
    {
        string result = arg.ToString();
        LOG.Info("----LoginByThirdPartyCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                UserDataManager.Instance.LoginInfo.LastLoginChannel = mCurLoginChannel;
                if (jo.code == 200)
                {
                    UserDataManager.Instance.thirdPartyLoginInfo = JsonHelper.JsonToObject<HttpInfoReturn<ThirdPartyReturnInfo>>(result);
                    if (UserDataManager.Instance.thirdPartyLoginInfo != null && UserDataManager.Instance.thirdPartyLoginInfo.data != null)
                    {
                        GameHttpNet.Instance.TOKEN = UserDataManager.Instance.thirdPartyLoginInfo.data.token;
                    }

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(172);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Login Successful!", false);

                    GameHttpNet.Instance.GetUserInfo(GetUserInfoCallBack);
                }
                else if (jo.code == 201)
                {
                    //UINetLoadingMgr.Instance.Close();
                    LOG.Info("--LoginByThirdPartyCallBack--->参数不完整");

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(173);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("参数不完整");
                }
                else if (jo.code == 208)
                {
                    //UINetLoadingMgr.Instance.Close();
                    LOG.Info("--LoginByThirdPartyCallBack--->登录失败");

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(174);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Login failed.");
                }
                else if (jo.code == 277)
                {
                    UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, jo.msg);
                    return;
                }

                UserDataManager.Instance.SaveLoginInfo();

            }, null);
        }
    }


    private void HuaweiLoginHandler(Notification vData)
    {
        hwUserInfo = vData.Data as HwUserInfo;
        if (hwUserInfo != null && hwUserInfo.type == 0)
        {

#if UNITY_EDITOR
           mCurLoginChannel = 3;
                        UserDataManager.Instance.UserData.UserID = hwUserInfo.playerId;
                        UserDataManager.Instance.UserData.IdToken = hwUserInfo.gameAuthSign;
                        GameHttpNet.Instance.LoginByThirdParty(2, "", hwUserInfo.displayName, "", hwUserInfo.playerId, "huawei", "huawei", LoginByThirdPartyCallBack);
            return;
#endif
            GameHttpNet.Instance.CheckLoginInfo(1, hwUserInfo.playerId, hwUserInfo.playerLevel, hwUserInfo.gameAuthSign, hwUserInfo.ts, HwLoginCallBack);
        }
    }

    private void HwLoginCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----HwLoginCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    if (hwUserInfo != null)
                    {
                        mCurLoginChannel = 3;
                        UserDataManager.Instance.UserData.UserID = hwUserInfo.playerId;
                        UserDataManager.Instance.UserData.IdToken = hwUserInfo.gameAuthSign;
                        GameHttpNet.Instance.LoginByThirdParty(2, "", hwUserInfo.displayName, "", hwUserInfo.playerId, "huawei", "huawei", LoginByThirdPartyCallBack);
                    }
                }
            }, null);
        }
    }


    private void GetUserInfoCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetUserInfoCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.userInfo = JsonHelper.JsonToObject<HttpInfoReturn<UserInfoCont>>(arg.ToString());

                    if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data != null)
                    {
                        UserDataManager.Instance.UserData.UserID = UserDataManager.Instance.userInfo.data.userinfo.uid;
                        UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.userInfo.data.userinfo.bkey,false);
                        UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.userInfo.data.userinfo.diamond,false);
                        UserDataManager.Instance.UserData.bookList = new List<int>();
                        //UserDataManager.Instance.UserData.bookList.AddRange(UserDataManager.Instance.userInfo.data.userinfo.booklist);


                        GameDataMgr.Instance.SetServerTime(int.Parse(UserDataManager.Instance.userInfo.data.userinfo.current_time));


                        EventDispatcher.Dispatch(EventEnum.HwLoginFormUpdat);
                        //【初始化评星】
                        XLuaHelper.initAppRating();
                        //同步头像框缓存
                        XLuaManager.Instance.GetLuaEnv().DoString(@"Cache.DressUpCache:ResetLogin();");
                    }

                    GameHttpNet.Instance.GetSelfBookInfo(ToLoadSelfBookInfo);
                    //GameHttpNet.Instance.GetShopList(GetShopListCallBack);
                    //GameHttpNet.Instance.GetNewUserEggState(NewUserEggStateCallBack);
                }
                else if (jo.code == 277)
                {
                    //UINetLoadingMgr.Instance.Close();
                    UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, jo.msg);
                    return;
                }
                else
                {
                    //UINetLoadingMgr.Instance.Close();

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(175);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Your information is out of date. Please, log in again.", false);
                    LOG.Error(jo.msg);
                }
            }, null);

        }
    }
    

    private void ToLoadSelfBookInfo(object arg)
    {

        string result = arg.ToString();
        LOG.Info("----ToLoadSelfBookInfo---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);

        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.selfBookInfo = JsonHelper.JsonToObject<HttpInfoReturn<SelfBookInfo>>(arg.ToString());
                    UserDataManager.Instance.InitRecordServerBookData();
                    // EventDispatcher.Dispatch(EventEnum.BookJoinToShelfEvent);
                    //刷新我的书本
                    XLuaManager.Instance.GetLuaEnv().DoString(@"GameController.MainFormControl:ResetMyBookList()");
                    EventDispatcher.Dispatch(EventEnum.BookProgressUpdate);
                    DoEnter();
                }
                else if (jo.code == 277)
                {
                    UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, jo.msg);
                    return;
                }
                else
                {
                    LOG.Error(jo.msg);
                }
            }, null);

        }
    }

    private void GetShopListCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetShopListCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.shopList = JsonHelper.JsonToObject<HttpInfoReturn<ShopListCont>>(result);
                }
                else if (jo.code == 277)
                {
                    UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, jo.msg);
                    return;
                }
            }, null);
        }
    }

    private void DoEnter()
    {
        //UINetLoadingMgr.Instance.Close();
        CUIManager.Instance.CloseForm(UIFormName.HwLoginForm);
        if (UserDataManager.Instance.userInfo.data.userinfo.firstplay == 0)
        {
            CUIManager.Instance.OpenForm(UIFormName.GuideForm/*, useFormPool: true*/);
        }
        else
        {
            // CUIManager.Instance.OpenForm(UIFormName.MainForm/*, useFormPool: true*/);

            //打开主界面
            XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:Open(logic.uiid.UIMainForm);");
        }

        if (UserDataManager.Instance.thirdPartyLoginInfo != null && UserDataManager.Instance.thirdPartyLoginInfo.data != null &&
            UserDataManager.Instance.thirdPartyLoginInfo.data.isfirst == 1)
        {
            UserDataManager.Instance.thirdPartyLoginInfo.data.isfirst = 0;
            UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, GameDataMgr.Instance.table.GetLocalizationById(224)/*"First login Successful! You have already received 10 diamonds."*/, AlertType.Sure, null, "OK");
        }

    }
}
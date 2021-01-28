/*
 https://developers.facebook.com/docs/
 https://developers.facebook.com/docs/unity/gettingstarted
 */
//using Facebook.Unity;
using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Facebook.Unity;

public class FacebookSdk
{
    public delegate void OnGotFBMyInfo(string resultJsonStr);
    public delegate void OnFBShareLinkSucced(string postId);
    public delegate void OnFBShareLinkFaild(bool isCancel, string errorInfo);
    public delegate void OnGotFBFriendInGame(string resultJsonStr);
    public delegate void OnFBInvitedSucceed(string resultJsonStr);
    public delegate void OnTakeScreenshot(string resultJsonStr);
    private static string appLinkUrl;

    private int mType;  //从哪里来的登陆请求

    public void Init()
    {
        //if (!FB.IsInitialized)
        //{
        //    // Initialize the Facebook SDK
        //    FB.Init(InitCallback, OnHideUnity);
        //}
        //else
        //{
        //    // Already initialized, signal an app activation App Event
        //    FB.ActivateApp();
        //}
    }
    /// <summary>
    /// 初始化SDK
    /// </summary>
    private void InitCallback()
    {
        //if (FB.IsInitialized)
        //{
        //    // Signal an app activation App Event
        //    FB.ActivateApp();
        //    // Continue with Facebook SDK
        //    // ...
        //    LOG.Info("Facebook SDK Init");
        //}
        //else
        //{
        //    LOG.Error("Failed to Initialize the Facebook SDK");
        //}
    }

    /// <summary>
    /// 分享窗口隐藏
    /// </summary>
    /// <param name="isGameShown"></param>
    private void OnHideUnity(bool isGameShown)
    {
        LOG.Error("OnHideUnity isGameShown:" + isGameShown);
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    public void Login(int vType)
    {
        mType = vType;

        //LOG.Info("======facebook====Login=====state====>>>" + FB.IsLoggedIn);
        //if(FB.IsLoggedIn)
        //{
        //    return;
        //}
        /*
           使用FB进行第三方登录：Facebook Login 
           获取已登录FB账号的相关信息，权限：public_profile
           获取已登录FB账号的同应用好友信息，权限：user_friends
           使用FB应用邀请功能：App Invites
           使用FB分享存文本或链接：Sharing
         */

        //http://bbs.mob.com/thread-23674-1-1.html
        //FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email"}, AuthCallback);
    }

    public bool IsLogin()
    {
        return false;//FB.IsLoggedIn;
    }

    public void Logout()
    {
        //if (!FB.IsLoggedIn)
        //{
        //    return;
        //}
        //FB.LogOut();
    }

    /// <summary>
    /// 记录付费的事件
    /// </summary>
    /// <param name="priceAmount"></param>
    /// <param name="priceCurrency"></param>
    public void LogPurchase(float priceAmount,string priceCurrency="USD")
    {
#if !UNITY_EDITOR
        //string packageName = SdkMgr.packageName;
        //var iapParameters = new Dictionary<string, object>();
        //iapParameters["mygame_packagename"] = packageName;
        //FB.LogPurchase(priceAmount, priceCurrency, iapParameters);
#endif
    }

    /// <summary>
    /// 记录App相关的内容
    /// </summary>
    /// <param name="logEvent"></param>
    /// <param name="parameters"></param>
    public void LogAppEvent(string logEvent,Dictionary<string, object> parameters)
    {
#if !UNITY_EDITOR
        //FB.LogAppEvent(logEvent, 0f, parameters);
#endif
    }


    /// <summary>
    /// 登录回调
    /// </summary>
    /// <param name="result"></param>
    //private void AuthCallback(ILoginResult result)
    //{
    //    LOG.Error("login:" + result);
    //    if (FB.IsLoggedIn)
    //    {
    //        // AccessToken class will have session details
    //        var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
    //        // Print current access token's User ID
    //        //LOG.Info(aToken.UserId);
    //        // Print current access token's granted permissions
    //        foreach (string perm in aToken.Permissions)
    //        {
    //            LOG.Info("permissions:" + perm);
    //        }

    //        LoginDataInfo loginInfo = new LoginDataInfo();
            

    //        if (AccessToken.CurrentAccessToken != null)
    //        {
    //            LOG.Error("登录token:" + AccessToken.CurrentAccessToken.ToString());
    //            //AndroidUtils.ShowDialog("登录成功", AccessToken.CurrentAccessToken.TokenString, "", new JAction.Zero(() => { }));

    //            loginInfo.UserId = AccessToken.CurrentAccessToken.UserId;
    //            loginInfo.Token = AccessToken.CurrentAccessToken.TokenString;
    //            loginInfo.OpenType = mType;
    //        }
    //        else
    //        {
    //            LOG.Error("登录token为空!!!");
    //            //AndroidUtils.ShowDialog("登录成功", "登录token为空!!!", "", new JAction.Zero(() => { }));
    //        }

    //        TalkingDataManager.Instance.LoginAccount(EventEnum.Login3rdResultSucc,1);
    //        FBGetAPPLinkUrl();
    //        EventDispatcher.Dispatch(EventEnum.FaceBookLoginSucc, loginInfo);
    //    }
    //    else
    //    {
    //        LOG.Error("login failed:" + result.Error + " " + result.RawResult);
    //        TalkingDataManager.Instance.LoginAccount(EventEnum.Login3rdResultFail,1);
    //        //AndroidUtils.ShowDialog("登录失败", "login failed:" + result.Error + " " + result.RawResult, "", new JAction.Zero(() => { }));
    //    }
    //}

    /// <summary>
    /// 获取自己的信息
    /// </summary>
    /// <param name="onGotFBMyInfo"></param>
    public void GetMyInfo(OnGotFBMyInfo onGotFBMyInfo)
    {

#if UNITY_EDITOR
        onGotFBMyInfo("{id:'gjgfdqefqcwe4131efqw341dfasdfbvdc05',name:'facebookTest03'}");
        return;
#endif

        //if(FB.IsLoggedIn == false)
        //{

        //    LOG.Info("Not Login in");

        //    return;

        //}

        //FB.API("me?fields=id,name,email", HttpMethod.GET, (result) =>
        //{

        //    LOG.Info("---selfInfoStr--->" + result.RawResult);

        //    if (onGotFBMyInfo != null)
        //        onGotFBMyInfo(result.RawResult);

        //});

    }


    //分享, 例：

    //uri = "https://developers.facebook.com/";

    //contentTitle = "ShareLink";

    //contentDesc = "Look I'm sharing a link";

    //picUri = "https://ss1.bdstatic.com/5eN1bjq8AAUYm2zgoY3K/r/www/cache/holiday/habo/res/doodle/3.png";
    /// <summary>
    /// 分享
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="contentTitle"></param>
    /// <param name="contentDesc"></param>
    /// <param name="picUri"></param>
    /// <param name="onFBShareLinkSucced"></param>
    /// <param name="onFBShareLinkFaild"></param>
    public void FBShareLink(string uri, string contentTitle, string contentDesc, string picUri, OnFBShareLinkSucced onFBShareLinkSucced = null, OnFBShareLinkFaild onFBShareLinkFaild = null)
    {
        Uri linkUri = null;
        if (!string.IsNullOrEmpty(uri))
            linkUri = new Uri(uri);
        Uri mPicUri = null;
        if (!string.IsNullOrEmpty(picUri))
            mPicUri = new Uri(picUri);

        FBShareLink(linkUri, contentTitle, contentDesc, mPicUri, onFBShareLinkSucced, onFBShareLinkFaild);

    }
    /// <summary>
    /// 分享
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="contentTitle"></param>
    /// <param name="contentDesc"></param>
    /// <param name="picUri"></param>
    /// <param name="onFBShareLinkSucced"></param>
    /// <param name="onFBShareLinkFaild"></param>
    private void FBShareLink(Uri uri, string contentTitle, string contentDesc, Uri picUri, OnFBShareLinkSucced onFBShareLinkSucced = null, OnFBShareLinkFaild onFBShareLinkFaild = null)
    {

        FB.ShareLink(uri, contentTitle, contentDesc, picUri, (result) =>
        {

            if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
            {

                LOG.Info("ShareLink Faild");

                if (onFBShareLinkFaild != null)
                {

                    onFBShareLinkFaild(result.Cancelled, result.Error);

                }

            }

            else
            {

                LOG.Info("ShareLink success!");

                if (onFBShareLinkSucced != null)
                {

                    onFBShareLinkSucced(String.IsNullOrEmpty(result.PostId) ? "" : result.PostId);

                }

            }

        });

    }


    /// <summary>
    /// 获取游戏好友
    /// </summary>
    /// <param name="onGotFBFriendInGame"></param>
    public void GetFBFriendInGame(OnGotFBFriendInGame onGotFBFriendInGame = null)
    {

        LOG.Info("GetFBFriendInGame");

        //if (FB.IsLoggedIn == false)
        //{

        //    LOG.Info("Not Login in");

        //    return;

        //}

        //FB.API("me/friends?fields=id,name,picture", HttpMethod.GET, (result) =>
        //{

        //    LOG.Info(result.RawResult);

        //    if (onGotFBFriendInGame != null)
        //    {

        //        onGotFBFriendInGame(result.RawResult);

        //    }

        //});

    }

    //获取可邀请好友, 获取失败 TODO

    public void GetFBFriendInvitable()
    {

        //if (FB.IsLoggedIn == false)
        //{

        //    LOG.Info("Not Login in");

        //    return;

        //}

        //FB.API("/me/invitable_friends?fields=id,name,picture", HttpMethod.GET, (result) =>
        //{

        //    LOG.Info("result: ");

        //    LOG.Info(result.RawResult);

        //});

    }


    /// <summary>
    /// 邀请
    /// </summary>
    /// <param name="assignedLink"></param>
    /// <param name="previewImageUrl"></param>
    /// <param name="onFBInvitedSucceed"></param>
    public void FBInvite(string assignedLink, string previewImageUrl, OnFBInvitedSucceed onFBInvitedSucceed = null)
    {

        if (String.IsNullOrEmpty(assignedLink))
        {

            assignedLink = appLinkUrl;

        }

        LOG.Info("appLinkUrl: " + appLinkUrl);

        LOG.Info("assignedLink: " + assignedLink);

        FBInvite(new Uri(assignedLink), null, onFBInvitedSucceed);

    }

    private void FBInvite(Uri appLinkUrl, Uri previewImageUrl = null, OnFBInvitedSucceed onFBInvitedSucceed = null)
    {

        //FB.Mobile.AppInvite(appLinkUrl, previewImageUrl, (result) =>
        //{

        //    LOG.Info("rawResult: " + result.RawResult);
        //    if(onFBInvitedSucceed != null)
        //    {
        //        onFBInvitedSucceed(result.RawResult);
        //    }
        //});

    }

    //获取APPLink, 获取失败，TODO

    public void FBGetAPPLinkUrl()
    {

        //FB.GetAppLink((result) =>
        //{

        //    LOG.Info(result.RawResult);

        //    LOG.Info("Ref: " + result.Ref);

        //    LOG.Info("TargetUrl: " + result.TargetUrl);

        //    LOG.Info("Url: " + result.Url);

        //    appLinkUrl = result.Url;

        //});

    }
}
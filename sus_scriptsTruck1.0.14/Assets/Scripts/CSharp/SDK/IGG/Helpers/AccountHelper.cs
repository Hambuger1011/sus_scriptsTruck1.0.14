//#define IGGSDK_EDITOR_IOS // 为了编码的方便在这边定义了该宏
using System.Collections.Generic;
using IGG.SDK;
using IGG.SDK.Core.Error;
using IGG.SDK.Framework;
using IGG.SDK.Modules.Account;
using IGG.SDK.Modules.Account.Guideline;
using IGG.SDK.Modules.Account.Guideline.BindScene;
using IGG.SDK.Modules.Account.Guideline.LoginScene;
using IGG.SDK.Modules.Account.Guideline.VO;
using IGG.SDK.Modules.Account.IGGAccount.VO;
using IGG.SDK.Modules.Account.VO;
using Script.Game.Helpers;
using UnityEngine;
using IGGAccountLoginScene = IGG.SDK.Modules.Account.Guideline.LoginScene.IGGAccountLoginScene;

namespace Helper.Account
{
    /// <summary>
    /// 封装了一些会员相关的业务逻辑，简化研发接入会员。
    /// </summary>
    public class AccountHelper
    {
        

        #region 登录（会话过期）相关逻辑封装
        
        /// <summary>
        /// 设备登录(用设备登录前检测当前设备绑定情况，已绑定则直接进行登录操作，未绑定则进行相应提示后再执行相应操作。)
        /// </summary>
        /// <param name="listener"></param>
        public void CheckCandidateByGuest(OnCheckCandidateByGuestListenr listener)
        {
            
            IGGGuestLoginScene scene = KungfuInstance.Get().GetPreparedAccountManagementGuideline().GetAccountLoginScene().GetGuestLoginScene();
            scene.CheckCandidate(delegate (IGGError error, bool hasBound, string IGGId)
                {
                    if (error.IsOccurred()) //检测出错，直接当做登录失败。
                    {
                        listener?.onLoginFailed?.Invoke(error);
                        return;
                    }
                    
                    if (!hasBound)
                    {
                        //未绑定
                        listener?.onUnbind?.Invoke();
                    }
                    else
                    {
                        // 已绑定,直接登录
                        LoginByGuest(listener);
                    }
                }
            );
        }

        /// <summary>
        /// 用Google账号登录前检测当前谷歌账号的绑定情况（统一调用第三方平台账号绑定情况的检测接口）
        /// </summary>
        /// <param name="token">谷歌平台拿到的账号token</param>
        /// <param name="type">谷歌token的类型</param>
        /// <param name="listener"></param>
        public void CheckCandidateByGoogleAccount(string token, IGGGoogleAccountTokenType type, OnCheckCandidateByThirdAccountListenr listener)
        {
            // 构造Google账号凭证信息
            IGGGoogleAccountAuthenticationProfile profile = new IGGGoogleAccountAuthenticationProfile();
            profile.SetPlatform(IGGLoginType.GooglePlus);
            profile.SetToken(token);
            profile.SetTokenType(type);
            // 检测第三方平台账号绑定情况
            CheckCandidateByThirdAccount(profile, IGGLoginType.GooglePlus, listener);
        }

        /// <summary>
        /// 用GameCenter账号登录前检测当前GameCenter账号的绑定情况（统一调用第三方平台账号绑定情况的检测接口）
        /// </summary>
        /// <param name="token"></param>
        /// <param name="listener"></param>
        public void CheckCandidateByGameCenter(string token, OnCheckCandidateByThirdAccountListenr listener)
        {
            // 构造GameCenter账号凭证信息
            IGGGameCenterAccountAuthenticationProfile profile = new IGGGameCenterAccountAuthenticationProfile();
            profile.SetToken(token);
            // 检测第三方平台账号绑定情况
            CheckCandidateByThirdAccount(profile, IGGLoginType.GameCenter, listener);
        }

        /// <summary>
        /// 用FB账号登录前检测当前FB账号的绑定情况（统一调用第三方平台账号绑定情况的检测接口）
        /// </summary>
        /// <param name="token"></param>
        /// <param name="listener"></param>
        public void CheckCandidateByFacebookAccount(string token, OnCheckCandidateByThirdAccountListenr listener)
        {
            // 构造FB账号凭证信息
            IGGThirdPartyAuthorizationProfile profile = new IGGThirdPartyAuthorizationProfile();
            profile.SetPlatform(IGGLoginType.Facebook);
            profile.SetToken(token);
            // 检测第三方平台账号绑定情况
            CheckCandidateByThirdAccount(profile, IGGLoginType.Facebook, listener);
        }

        /// <summary>
        /// 第三方登录(用第三方登录前检测当前设备绑定情况，已绑定则直接进行登录操作，未绑定则进行相应提示后再执行相应操作。)
        /// </summary>
        /// <param name="profile">从第三方获取的账号信息</param>
        /// <param name="loginType">第三方账号类型</param>
        /// <param name="listener"></param>
        private void CheckCandidateByThirdAccount(IGGThirdPartyAuthorizationProfile profile, IGGLoginType loginType, OnCheckCandidateByThirdAccountListenr listener)
        {
            
            IGGThirdPartyAccountLoginScene scene = KungfuInstance.Get().GetPreparedAccountManagementGuideline().GetAccountLoginScene().GetThirdPartyAccountLoginScene();
            scene.CheckCandidate(profile, delegate (IGGError error, bool hasBound, string IGGId)
                {
                    if (error.IsOccurred()) //检测出错，直接当做登录失败。
                    {
                        listener?.onLoginFailed?.Invoke(error);
                        return;
                    }
                    
                    if (!hasBound)
                    {
                        //未绑定
                        listener?.onUnbind?.Invoke(profile);
                    }
                    else
                    {
                        // 已绑定,直接登录
                        LoginByThirdAccount(profile, listener);
                    }
                }
            );
        }

        /// <summary>
        /// IGG通行证登录(用IGG通行证登录前检测当前设备绑定情况，已绑定则直接进行登录操作，未绑定则进行相应提示后再执行相应操作。)
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="isForAudting"></param>
        public void CheckCandidateByIGGAccount(OnCheckCandidateByIGGAccountListenr listener, bool isForAudting = false)
        {
            
            IGGAccountLoginScene loginScene = KungfuInstance.Get().GetPreparedAccountManagementGuideline().GetAccountLoginScene().GetIGGAccountLoginScene();
            loginScene.SetBehavior(IGGAccountLoginBehavior.Browser); // 设置通过网页模式进行IGG通行证账号登录（目前暂时只支持网页版，手游助手版正在开发）
            loginScene.SetGameAudting(isForAudting); // 设置IGG通行证是否仅用于版号审核用（版号审核用的仅支持几个特殊的邮箱账号登录）
            loginScene.CheckCandidate(delegate (IGGError error, IGGAccountLoginResult result)  // 检测IGG通行证账号绑定情况
            {
                if (error.IsOccurred()) //检测出错，直接当做登录失败。
                {
                    listener?.onLoginFailed?.Invoke(error);
                    return;
                }

                bool hasBound = result.HasBound();
                IGGAccountLoginContext context = result.GetContext();
                if (!hasBound)
                {
                    //未绑定
                    listener?.onUnbind?.Invoke(context);
                }
                else
                {
                    // 已绑定,直接登录
                    LoginByIGGAccount(context, listener);
                }
            });
        }
        
        /// <summary>
        /// Apple登录(用Apple登录前检测当前设备绑定情况，已绑定则直接进行登录操作，未绑定则进行相应提示后再执行相应操作。)
        /// </summary>
        /// <param name="listener"></param>
        public void CheckCandidateByApple(OnCheckCandidateByAppleListenr listener)
        {
            
            IGGAppleLoginScene loginScene = KungfuInstance.Get().GetPreparedAccountManagementGuideline().GetAccountLoginScene().GetAppleLoginScene();
            loginScene.CheckCandidate(delegate (IGGError error, IGGAppleLoginResult result)  // 检测apple账号绑定情况
            {
                if (error.IsOccurred()) //检测出错，直接当做登录失败。
                {
                    listener?.onLoginFailed?.Invoke(error);
                    return;
                }

                bool hasBound = result.HasBound();
                IGGAppleLoginContext context = result.GetContext();
                if (!hasBound)
                {
                    //未绑定
                    listener?.onUnbind?.Invoke(context);
                }
                else
                {
                    // 已绑定,直接登录
                    LoginByApple(context, listener);
                }
            });
        }
        #endregion

        #region 绑定相关逻辑封装
        /// <summary>
        /// 绑定Google账号（统一调用第三方平台账号绑定）
        /// </summary>
        /// <param name="token"></param>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        /// <param name="loadUserListener"></param>
        public void BindByGoogleAccount(string token, IGGGoogleAccountTokenType type, OnBindListener listener, OnLoadUserListener loadUserListener)
        {
            // 构造Google账号凭证信息
            IGGGoogleAccountAuthenticationProfile profile = new IGGGoogleAccountAuthenticationProfile();
            profile.SetPlatform(IGGLoginType.GooglePlus);
            profile.SetToken(token);
            profile.SetTokenType(type);
            // 第三方平台账号绑定
            BindByThirdPartyAccount(profile, listener, loadUserListener);
        }

        /// <summary>
        /// 绑定FB账号（统一调用第三方平台账号绑定）
        /// </summary>
        /// <param name="token"></param>
        /// <param name="listener"></param>
        /// <param name="loadUserListener"></param>
        public void BindByFacebookAccount(string token, OnBindListener listener, OnLoadUserListener loadUserListener)
        {
            // 构造FB账号凭证信息
            IGGThirdPartyAuthorizationProfile profile = new IGGThirdPartyAuthorizationProfile();
            profile.SetPlatform(IGGLoginType.Facebook);
            profile.SetToken(token);
            // 第三方平台账号绑定
            BindByThirdPartyAccount(profile, listener, loadUserListener);
        }

        /// <summary>
        /// 绑定GameCenter账号（统一调用第三方平台账号绑定）
        /// </summary>
        /// <param name="token"></param>
        /// <param name="listener"></param>
        /// <param name="loadUserListener"></param>
        public void BindByGameCenter(string token, OnBindListener listener, OnLoadUserListener loadUserListener)
        {
            // 构造GameCenter账号凭证信息
            IGGGameCenterAccountAuthenticationProfile profile = new IGGGameCenterAccountAuthenticationProfile();
            profile.SetToken(token);
            // 第三方平台账号绑定
            BindByThirdPartyAccount(profile, listener, loadUserListener);
        }

        /// <summary>
        /// 第三方平台账号绑定
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="listener"></param>
        /// <param name="loadUserListener"></param>
        private void BindByThirdPartyAccount(IGGThirdPartyAuthorizationProfile profile, OnBindListener listener, OnLoadUserListener loadUserListener)
        {
            IGGThirdPartyAccountBindingScene scene = KungfuInstance.Get().GetPreparedAccountManagementGuideline().GetAccountBindScene().GetThirdPartyAccountBindingScene();
            scene.Bind(profile, delegate (IGGError error, string IGGID)
            {
                if (!error.IsNone() && (null != IGGID && "" != IGGID))
                {
                    // 提示用户已经绑定过 IGG ID
                    listener?.onBoundIGGID?.Invoke(IGGID);
                }
                else
                {
                    if (error.IsNone())
                    {
                        //提示用户绑定成功
                        listener?.onBindSuccess?.Invoke();
                        //重新加载用户信息
                        LoadUser(loadUserListener);
                    }
                    else
                    {
                        //提示用户绑定失败
                        listener?.onBindFailed?.Invoke(error);
                    }
                }
            });
        }

        /// <summary>
        /// IGG通行证账号绑定
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="loadUserListener"></param>
        public void BindByIGGAccount(OnBindListener listener, OnLoadUserListener loadUserListener)
        {
            var scene = KungfuInstance.Get().GetPreparedAccountManagementGuideline().GetAccountBindScene().GetIGGAccountBindingScene();
            scene.SetBehavior(IGGAccountLoginBehavior.Browser); // 设置通过网页模式进行IGG通行证账号登录（目前暂时只支持网页版，手游助手版正在开发）
            scene.Bind(delegate (IGGError error, string IGGID)
            {
                if (!error.IsNone() && (null != IGGID && "" != IGGID))
                {
                    // 提示用户已经绑定过 IGG ID
                    listener?.onBoundIGGID?.Invoke(IGGID);
                }
                else
                {
                    if (error.IsNone())
                    {
                        //提示用户绑定成功
                        listener?.onBindSuccess?.Invoke();
                        //重新加载用户信息
                        LoadUser(loadUserListener);
                    }
                    else
                    {
                        //提示用户绑定失败
                        listener?.onBindFailed?.Invoke(error);
                    }
                }
            });
        }
        
        /// <summary>
        /// IGG通行证账号绑定
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="loadUserListener"></param>
        public void BindByApple(OnBindListener listener, OnLoadUserListener loadUserListener)
        {
            var scene = KungfuInstance.Get().GetPreparedAccountManagementGuideline().GetAccountBindScene().GetAppleBindingScene();
            scene.Bind(delegate (IGGError error, string IGGID)
            {
                if (!error.IsNone() && (null != IGGID && "" != IGGID))
                {
                    // 提示用户已经绑定过 IGG ID
                    listener?.onBoundIGGID?.Invoke(IGGID);
                }
                else
                {
                    if (error.IsNone())
                    {
                        //提示用户绑定成功
                        listener?.onBindSuccess?.Invoke();
                        //重新加载用户信息
                        LoadUser(loadUserListener);
                    }
                    else
                    {
                        //提示用户绑定失败
                        listener?.onBindFailed?.Invoke(error);
                    }
                }
            });
        }
        #endregion

        #region 加载用户信息（绑定情况与实名认证情况），并对绑定信息进行相应处理
        
        /// <summary>
        /// 加载当前用户信息
        /// </summary>
        /// <param name="listener"></param>
        public void LoadUser(OnLoadUserListener listener)
        {
            
            var accountManagementGuideline = KungfuInstance.Get().GetPreparedAccountManagementGuideline();
            
            //Tips:接入 IGG 通行证时，必须调用 setCompatProxy 接口传入支持的登录类型
            accountManagementGuideline.SetCompatProxy(new IGGAccountManagerGuidelineMockCompatProxy());

            accountManagementGuideline.LoadUser(delegate (IGGError error, IGGUserProfile userProfile)
            {
                if (!error.IsNone()) // 加载失败，错误返回
                {
                    listener?.onLoadUserFailed?.Invoke(error);
                    return;
                }

                //在UI上显示用户的信息
                listener?.onLoadUserSuccess?.Invoke(userProfile);

                //获取设备绑定的状态
                GuestBindState guestBindState = GetGuestBindingStatus(userProfile);
                listener?.onGuestBindState?.Invoke(guestBindState);

                //Google Play 的绑定状态 
                List<IGGUserBindingProfile> userBindingProfiles = userProfile.GetBindingProfiles();
                IGGUserBindingProfile profile = null;

                //Facebook 的绑定状态
                profile = GetBindMessage(userBindingProfiles, IGGLoginType.Facebook);
                if (profile != null)
                {
                    listener?.onBindInfo?.Invoke(IGGLoginType.Facebook, profile);
                }
                else
                {
                    listener?.onUnbound?.Invoke(IGGLoginType.Facebook);
                }

                //IGG 通行证的绑定状态
                profile = GetBindMessage(userBindingProfiles, IGGLoginType.IGGAccount);
                if (profile != null)
                {
                    listener?.onBindInfo?.Invoke(IGGLoginType.IGGAccount, profile);
                }
                else
                {
                    listener?.onUnbound?.Invoke(IGGLoginType.IGGAccount);
                }

#if UNITY_IOS 
                //GameCenter
                profile = GetBindMessage(userBindingProfiles, IGGLoginType.GameCenter);
                if (profile != null)
                {
                    listener?.onBindInfo?.Invoke(IGGLoginType.GameCenter, profile);
                }
                else
                {
                    listener?.onUnbound?.Invoke(IGGLoginType.GameCenter);
                }
                //Apple
                profile = GetBindMessage(userBindingProfiles, IGGLoginType.Apple);
                if (profile != null)
                {
                    listener?.onBindInfo?.Invoke(IGGLoginType.Apple, profile);
                }
                else
                {
                    listener?.onUnbound?.Invoke(IGGLoginType.Apple);
                }
#elif UNITY_ANDROID 
                profile = GetBindMessage(userBindingProfiles, IGGLoginType.GooglePlus);
                if (profile != null)
                {
                    listener?.onBindInfo?.Invoke(IGGLoginType.GooglePlus, profile);
                }
                else
                {
                    listener?.onUnbound?.Invoke(IGGLoginType.GooglePlus);
                }
#endif
            });
        }

        /// <summary>
        /// 检测当前账号是否安全（IGGID有绑定了第三方平台的账号都认为这个IGGID是安全的）
        /// </summary>
        /// <param name="userProfile"></param>
        /// <returns></returns>
        public bool IsAccountSafety(IGGUserProfile userProfile)
        {
            if (userProfile == null)
            {
                return false;
            }
            //账号的安全等级，是否安全
            bool isAccountSafe = false;

            List<IGGUserBindingProfile> userBindingProfiles = userProfile.GetBindingProfiles();

            //Facebook 的绑定状态
            IGGUserBindingProfile profile = GetBindMessage(userBindingProfiles, IGGLoginType.Facebook);
            if (profile != null)
            {
                isAccountSafe = true;
            }

            //IGG Account的绑定状态
            profile = GetBindMessage(userBindingProfiles, IGGLoginType.IGGAccount);
            if (profile != null)
            {
                isAccountSafe = true;
            }

#if UNITY_IOS 

            profile = GetBindMessage(userBindingProfiles, IGGLoginType.GameCenter);
            if (profile != null)
            {
                isAccountSafe = true;
            }
            
            profile = GetBindMessage(userBindingProfiles, IGGLoginType.Apple);
            if (profile != null)
            {
                isAccountSafe = true;
            }

#elif UNITY_ANDROID 
            profile = GetBindMessage(userBindingProfiles, IGGLoginType.GooglePlus);
            if (profile != null)
            {
                isAccountSafe = true;
            }
#endif
            return isAccountSafe;
        }

        /// <summary>
        /// 获取当前账号某登录类型的绑定情况
        /// </summary>
        /// <param name="userBindingProfiles"></param>
        /// <param name="loginType"></param>
        /// <returns></returns>
        public IGGUserBindingProfile GetBindMessage(List<IGGUserBindingProfile> userBindingProfiles, IGGLoginType loginType)
        {
            if (userBindingProfiles == null)
            {
                return null;
            }

            IGGUserBindingProfile profile = null;
            for (int i = 0; i < userBindingProfiles.Count; i++)
            {
                IGGUserBindingProfile userBindingProfile = userBindingProfiles[i];
                if (userBindingProfile.GetTypeValue() == loginType)
                {
                    profile = userBindingProfile;
                }
            }

            return profile;
        }

        /// <summary>
        /// 获取当前账号设备登录类型的绑定情况，分别为：未绑定（直接通过第三方平台账号创建的IGGID）、已绑定并且是本机、已绑定不是本机
        /// </summary>
        /// <param name="userProfile"></param>
        /// <returns></returns>
        private GuestBindState GetGuestBindingStatus(IGGUserProfile userProfile)
        {
            if (null == userProfile)
            {
                return GuestBindState.NONE;
            }
            // 获取绑定类型信息
            List<IGGUserBindingProfile> userBindingProfiles = userProfile.GetBindingProfiles();
            GuestBindState guestBindState = GuestBindState.NONE;
            // guest 可能绑定多个设备ID，判断当前设备是否已经绑定
            for (int i = 0; i < userBindingProfiles.Count; i++)
            {
                IGGUserBindingProfile userBindingProfile = userBindingProfiles[i];
                if (userBindingProfile.GetTypeValue() == IGGLoginType.Guest) // 匹配到设备绑定信息
                {
                    string bindDeviceId = userBindingProfile.GetKey();
                    if (null != KungfuInstance.Get().UDID && KungfuInstance.Get().UDID == bindDeviceId) // 检测是不是本机设备
                    {
                        guestBindState = GuestBindState.BIND_CURRENT_DEVICE;
                    }
                }
            }

            // 不是本机设备时，检测当前账号是否有绑定其他设备
            if (guestBindState == GuestBindState.NONE)
            {
                for (int i = 0; i < userBindingProfiles.Count; i++)
                {
                    IGGUserBindingProfile userBindingProfile = userBindingProfiles[i];
                    if (userBindingProfile.GetTypeValue() == IGGLoginType.Guest)
                    {
                        guestBindState = GuestBindState.BIND_NO_CURRENT_DEVICE;
                    }
                }
            }

            return guestBindState;
        }
        #endregion

        #region 切换账号（账号管理页里的账号切换）
        /// <summary>
        /// 通过设备信息切换账号
        /// </summary>
        /// <param name="listener"></param>
        public void SwitchLoginByGuest(OnSwitchGuestLoginListener listener)
        {
            
            IGGGuestLoginScene scene = KungfuInstance.Get().GetPreparedAccountManagementGuideline().GetAccountLoginScene().GetGuestLoginScene();
            // 检测当前设备绑定情况
            scene.CheckCandidate(delegate (IGGError error, bool hasBound, string IGGId)
                {
                    if (error.IsOccurred()) // 检测出现错误，直接当做登录失败。
                    {
                        listener?.onLoginFailed?.Invoke(error);
                        return;
                    }

                    if (!hasBound)
                    {
                        //未绑定（具体处理请参考会员Demo）
                        listener?.onUnbind?.Invoke();
                    }
                    else
                    {
                        switch (CheckCandidateResult(IGGId, IGGLoginType.Guest))
                        {
                            case SwitchOperationType.DifIGGID:
                                listener?.onBindDifIGGID?.Invoke(IGGId);
                                break;
                            case SwitchOperationType.SameAsCurrent:
                                listener?.onIGGIDSameAsNow?.Invoke(IGGId);
                                break;
                            case SwitchOperationType.DifLoginType:
                                listener.onBindDifLoginType?.Invoke(IGGId);
                                break;
                        }
                    }
                }
            );
        }

        /// <summary>
        /// 通过设备信息创建新号并登录（一般当前设备未绑定某IGGID，然后执行登录的情况）
        /// </summary>
        /// <param name="listener"></param>
        public void CreateAndLoginByGuest(OnLoginListener listener)
        {
            IGGGuestLoginScene scene = KungfuInstance.Get().GetPreparedAccountManagementGuideline().GetAccountLoginScene().GetGuestLoginScene();
            // 通过设备信息创建新号并登录
            scene.CreateAndLogin(delegate (IGGError error, IGGSession session)
            {
                if (error.IsNone()) // 登录成功。
                {
                    listener?.onLoginSuccess?.Invoke(session);
                }
                else // 登录失败。
                {
                    listener?.onLoginFailed?.Invoke(error);
                }
            });
        }

        /// <summary>
        /// 通过设备信息登录（一般当前设备已绑定某IGGID，然后执行登录的情况）
        /// </summary>
        /// <param name="listener"></param>
        public void LoginByGuest(OnLoginListener listener)
        {
            IGGGuestLoginScene scene = KungfuInstance.Get().GetPreparedAccountManagementGuideline().GetAccountLoginScene().GetGuestLoginScene();
            // 通过设备信息登录
            scene.Login(delegate (IGGError error, IGGSession session)
            {
                if (error.IsNone()) // 登录成功。
                {
                    listener?.onLoginSuccess?.Invoke(session);
                }
                else // 登录失败。
                {
                    listener?.onLoginFailed?.Invoke(error);
                }
            });
        }

        /// <summary>
        /// 通过Google账号切换游戏账号（统一调用第三方平台账号切换）
        /// </summary>
        /// <param name="token"></param>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        public void SwitchLoginByGoogleAccount(string token, IGGGoogleAccountTokenType type, OnSwitchLoginByThirdAccountListener listener)
        {
            // 构造Google账号凭证信息
            IGGGoogleAccountAuthenticationProfile profile = new IGGGoogleAccountAuthenticationProfile();
            profile.SetPlatform(IGGLoginType.GooglePlus);
            profile.SetToken(token);
            profile.SetTokenType(type);
            // 第三方平台账号切换
            SwitchLoginByThirdAccount(profile, IGGLoginType.GooglePlus, listener);
        }

        /// <summary>
        /// 通过FB账号切换游戏账号（统一调用第三方平台账号切换）
        /// </summary>
        /// <param name="token"></param>
        /// <param name="listener"></param>
        public void SwitchLoginByFacebookAccount(string token, OnSwitchLoginByThirdAccountListener listener)
        {
            // 构造FB账号凭证信息
            IGGThirdPartyAuthorizationProfile profile = new IGGThirdPartyAuthorizationProfile();
            profile.SetPlatform(IGGLoginType.Facebook);
            profile.SetToken(token);
            // 第三方平台账号切换
            SwitchLoginByThirdAccount(profile, IGGLoginType.Facebook, listener);
        }

        /// <summary>
        /// 通过GameCenter账号切换游戏账号（统一调用第三方平台账号切换）
        /// </summary>
        /// <param name="token"></param>
        /// <param name="listener"></param>
        public void SwitchLoginByGameCenter(string token, OnSwitchLoginByThirdAccountListener listener)
        {
            // 构造GameCenter账号凭证信息
            IGGGameCenterAccountAuthenticationProfile profile = new IGGGameCenterAccountAuthenticationProfile();
            profile.SetPlatform(IGGLoginType.GameCenter);
            profile.SetToken(token);
            // 第三方平台账号切换
            SwitchLoginByThirdAccount(profile, IGGLoginType.GameCenter, listener);
        }

        /// <summary>
        /// 第三方平台账号切换
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="loginType"></param>
        /// <param name="listener"></param>
        private void SwitchLoginByThirdAccount(IGGThirdPartyAuthorizationProfile profile, IGGLoginType loginType, OnSwitchLoginByThirdAccountListener listener)
        {
            
            IGGThirdPartyAccountLoginScene scene = KungfuInstance.Get().GetPreparedAccountManagementGuideline().GetAccountLoginScene().GetThirdPartyAccountLoginScene();
            scene.CheckCandidate(profile, delegate (IGGError error, bool hasBound, string IGGId)
                {
                    if (error.IsOccurred()) // 检测出现错误，直接当做登录失败。
                    {
                        listener?.onLoginFailed?.Invoke(error);
                        return;
                    }
                    
                    if (!hasBound)
                    {
                        //未绑定（具体处理请参考会员Demo）
                        listener?.onUnbind?.Invoke(profile);
                    }
                    else
                    {
                        switch (CheckCandidateResult(IGGId, loginType))
                        {
                            case SwitchOperationType.DifIGGID:
                                listener?.onBindDifIGGID?.Invoke(profile, IGGId);
                                break;
                            case SwitchOperationType.SameAsCurrent:
                                listener?.onIGGIDSameAsNow?.Invoke(profile, IGGId);
                                break;
                            case SwitchOperationType.DifLoginType:
                                listener.onBindDifLoginType?.Invoke(profile, IGGId);
                                break;
                        }
                    }
                }
            );
        }

        /// <summary>
        /// 通过第三方平台账号创建新号并登录（一般当前第三方平台账号未绑定某IGGID，然后执行登录的情况）
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="listener"></param>
        public void CreateAndLoginByThirdAccount(IGGThirdPartyAuthorizationProfile profile, OnLoginListener listener)
        {
            IGGThirdPartyAccountLoginScene scene = KungfuInstance.Get().GetPreparedAccountManagementGuideline().GetAccountLoginScene().GetThirdPartyAccountLoginScene();
            scene.CreateAndLogin(profile, delegate (IGGError error, IGGSession session)
            {
                if (error.IsNone()) // 登录成功。
                {
                    listener?.onLoginSuccess?.Invoke(session);
                }
                else // 登录失败。
                { 
                    listener?.onLoginFailed?.Invoke(error);
                }
            });
        }

        /// <summary>
        /// 通过第三方平台账号登录（一般当前第三方平台账号已绑定某IGGID，然后执行登录的情况）
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="listener"></param>
        public void LoginByThirdAccount(IGGThirdPartyAuthorizationProfile profile, OnLoginListener listener)
        {
            IGGThirdPartyAccountLoginScene scene = KungfuInstance.Get().GetPreparedAccountManagementGuideline().GetAccountLoginScene().GetThirdPartyAccountLoginScene();
            scene.Login(profile, delegate (IGGError error, IGGSession session)
            {
                if (error.IsNone()) // 登录成功。
                {
                    listener?.onLoginSuccess?.Invoke(session);
                }
                else // 登录失败。
                {
                    listener?.onLoginFailed?.Invoke(error);
                }
            });
        }

        /// <summary>
        /// 通过IGG通行证切换账号。
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="isForAudting"></param>
        public void SwitchLoginByIGGAccount(OnSwitchLoginByIGGAccountListener listener, bool isForAudting = false)
        {
            
            IGGAccountLoginScene loginScene = KungfuInstance.Get().GetPreparedAccountManagementGuideline().GetAccountLoginScene().GetIGGAccountLoginScene();
            loginScene.SetBehavior(IGGAccountLoginBehavior.Browser); // 设置通过网页模式进行IGG通行证账号登录（目前暂时只支持网页版，手游助手版正在开发）
            loginScene.SetGameAudting(isForAudting); // 设置IGG通行证是否仅用于版号审核用（版号审核用的仅支持几个特殊的邮箱账号登录）
            loginScene.CheckCandidate(delegate (IGGError error, IGGAccountLoginResult result) // 检测IGG通行证账号绑定情况
            {
                if (error.IsOccurred()) // 检测出错当做登录失败处理。
                {
                    listener?.onLoginFailed?.Invoke(error);
                    return;
                }

                bool hasBound = result.HasBound();
                
                IGGAccountLoginContext context = result.GetContext();
                if (!hasBound)
                {
                    //未绑定（具体处理请参考会员Demo）
                    listener?.onUnbind?.Invoke(context);
                }
                else
                {
                    var IGGID = result.GetIGGId();
                    switch (CheckCandidateResult(result.GetIGGId(), IGGLoginType.IGGAccount))
                    {
                        case SwitchOperationType.DifIGGID:
                            listener?.onBindDifIGGID?.Invoke(context, IGGID);
                            break;
                        case SwitchOperationType.SameAsCurrent:
                            listener?.onIGGIDSameAsNow?.Invoke(context, IGGID);
                            break;
                        case SwitchOperationType.DifLoginType:
                            listener.onBindDifLoginType?.Invoke(context, IGGID);
                            break;
                    }
                }
            });
        }
        
        /// <summary>
        /// 通过Apple账号。
        /// </summary>
        /// <param name="listener"></param>
        public void SwitchLoginByApple(OnSwitchLoginByIGGAppleListener listener)
        {
            
            IGGAppleLoginScene loginScene = KungfuInstance.Get().GetPreparedAccountManagementGuideline().GetAccountLoginScene().GetAppleLoginScene();
            loginScene.CheckCandidate(delegate (IGGError error, IGGAppleLoginResult result) // 检测IGG通行证账号绑定情况
            {
                if (error.IsOccurred()) // 检测出错当做登录失败处理。
                {
                    listener?.onLoginFailed?.Invoke(error);
                    return;
                }

                bool hasBound = result.HasBound();
                
                IGGAppleLoginContext context = result.GetContext();
                if (!hasBound)
                {
                    //未绑定（具体处理请参考会员Demo）
                    listener?.onUnbind?.Invoke(context);
                }
                else
                {
                    var IGGID = result.GetIGGId();
                    switch (CheckCandidateResult(result.GetIGGId(), IGGLoginType.Apple))
                    {
                        case SwitchOperationType.DifIGGID:
                            listener?.onBindDifIGGID?.Invoke(context, IGGID);
                            break;
                        case SwitchOperationType.SameAsCurrent:
                            listener?.onIGGIDSameAsNow?.Invoke(context, IGGID);
                            break;
                        case SwitchOperationType.DifLoginType:
                            listener.onBindDifLoginType?.Invoke(context, IGGID);
                            break;
                    }
                }
            });
        }

        /// <summary>
        /// 通过IGG通行证账号创建新号并登录（一般当前IGG通行证账号未绑定某IGGID，然后执行登录的情况）
        /// </summary>
        /// <param name="context"></param>
        /// <param name="listener"></param>
        public void CreateAndLoginByIGGAccount(IGGAccountLoginContext context, OnLoginListener listener)
        {
            context.CreateAndLogin(delegate (IGGError error, IGGSession session)
            {
                if (error.IsNone()) // 登录成功。
                {
                    listener?.onLoginSuccess?.Invoke(session);
                }
                else // 登录失败。
                {
                    listener?.onLoginFailed?.Invoke(error);
                }
            });
        }
        
        /// <summary>
        /// 通过Apple账号创建新号并登录（一般当前Apple账号未绑定某IGGID，然后执行登录的情况）
        /// </summary>
        /// <param name="context"></param>
        /// <param name="listener"></param>
        public void CreateAndLoginByApple(IGGAppleLoginContext context, OnLoginListener listener)
        {
            context.CreateAndLogin(delegate (IGGError error, IGGSession session)
            {
                if (error.IsNone()) // 登录成功。
                {
                    listener?.onLoginSuccess?.Invoke(session);
                }
                else // 登录失败。
                {
                    listener?.onLoginFailed?.Invoke(error);
                }
            });
        }

        /// <summary>
        /// 通过IGG通行证账号登录（一般当前IGG通行证账号已绑定某IGGID，然后执行登录的情况）
        /// </summary>
        /// <param name="context"></param>
        /// 
        /// <param name="listener"></param>
        public void LoginByIGGAccount(IGGAccountLoginContext context, OnLoginListener listener)
        {
            context.Login(delegate (IGGError error, IGGSession session)
            {
                if (error.IsNone()) // 登录成功。
                {
                    listener?.onLoginSuccess?.Invoke(session);
                }
                else // 登录失败。
                {
                    listener?.onLoginFailed?.Invoke(error);
                }
            });
        }
        
        /// <summary>
        /// 通过Apple账号登录（一般当前APple账号已绑定某IGGID，然后执行登录的情况）
        /// </summary>
        /// <param name="context"></param>
        /// <param name="listener"></param>
        public void LoginByApple(IGGAppleLoginContext context, OnLoginListener listener)
        {
            context.Login(delegate (IGGError error, IGGSession session)
            {
                if (error.IsNone()) // 登录成功。
                {
                    listener?.onLoginSuccess?.Invoke(session);
                }
                else // 登录失败。
                {
                    listener?.onLoginFailed?.Invoke(error);
                }
            });
        }
        
        private SwitchOperationType CheckCandidateResult(string IGGID, IGGLoginType type)
        {
            IGGUserProfile profile = KungfuInstance.Get().GetPreparedAccountManagementGuideline().GetUserProfile();
            if (IGGID != profile.GetIGGId())
            {
                // 绑定的 IGGID 不是当前的 IGGID（具体处理请参考会员Demo）
                return SwitchOperationType.DifIGGID;
            }
            if (!IGGSession.currentSession.IsSessionExpired() && IGGID == profile.GetIGGId() && profile.GetLoginType() == type)
            {
                // 绑定的 IGGID 是当前的 IGGID 且登录方式一样（具体处理请参考会员Demo）
                return SwitchOperationType.SameAsCurrent;
            }

            if (IGGID == profile.GetIGGId() && profile.GetLoginType() != type)
            {
                // 绑定的 IGGID 是当前的 IGGID 但登录方式不一样（具体处理请参考会员Demo）
                return SwitchOperationType.DifLoginType;
            }

            return SwitchOperationType.Unknow;
        }
        #endregion
    }

    enum SwitchOperationType
    {
        DifIGGID, SameAsCurrent, DifLoginType, Unknow
    }

    /// <summary>
    /// 当有接入IGG通行证的游戏，在获取当前游戏账号绑定情况时，必须去设置这个Proxy，否则将获取不到当前游戏账号绑定的IGG通行证情况，另外设置的时候，必须把当前游戏支持的所有登录类型都写上去（getSupportedLoginTypes）。
    /// </summary>
    public class IGGAccountManagerGuidelineMockCompatProxy : IGGAccountManagerGuidelineCompatProxy
    {
        public IGGSession GetIGGSession()
        {
            return IGGSession.currentSession;
        }

        public List<IGGLoginType> GetSupportedLoginTypes()
        {
            
            return new List<IGGLoginType>() { IGGLoginType.Guest,
                                        IGGLoginType.Facebook,
                                        IGGLoginType.IGGAccount,  
#if UNITY_IOS 
                                        IGGLoginType.GameCenter,
                                        IGGLoginType.Apple
#elif UNITY_ANDROID 
                
                                        IGGLoginType.GooglePlus
#endif
            };
        }
    }

    /// <summary>
    /// 设备绑定的状态
    /// </summary>
    public enum GuestBindState
    {
        //未绑定
        NONE,
        //已绑定，绑定当前设备
        BIND_CURRENT_DEVICE,
        //已绑定，但不是当前设备
        BIND_NO_CURRENT_DEVICE
    }

    /// <summary>
    /// 检测设备绑定情况结果回调（封装了一层，更符合业务层面使用）
    /// </summary>
    public class OnCheckCandidateByGuestListenr : OnLoginListener
    {
        public delegate void OnUnbind();

        public OnUnbind onUnbind;

        public OnCheckCandidateByGuestListenr(OnUnbind onUnbind, OnLoginSuccess onLoginSuccess, OnLoginFailed onLoginFailed) : base(onLoginSuccess, onLoginFailed)
        {
            this.onUnbind = onUnbind;
        }
    }

    /// <summary>
    /// 检测第三方平台账号绑定情况结果回调（封装了一层，更符合业务层面使用）
    /// </summary>
    public class OnCheckCandidateByThirdAccountListenr : OnLoginListener
    {
        public delegate void OnUnbind(IGGThirdPartyAuthorizationProfile profile);

        public OnUnbind onUnbind;

        public OnCheckCandidateByThirdAccountListenr(OnUnbind onUnbind, OnLoginSuccess onLoginSuccess, OnLoginFailed onLoginFailed) : base(onLoginSuccess, onLoginFailed)
        {
            this.onUnbind = onUnbind;
        }
    }

    /// <summary>
    /// 检测IGG通行证账号绑定情况结果回调（封装了一层，更符合业务层面使用）
    /// </summary>
    public class OnCheckCandidateByIGGAccountListenr : OnLoginListener
    {
        public delegate void OnUnbind(IGGAccountLoginContext context);

        public OnUnbind onUnbind;

        public OnCheckCandidateByIGGAccountListenr(OnUnbind onUnbind, OnLoginSuccess onLoginSuccess, OnLoginFailed onLoginFailed) : base(onLoginSuccess, onLoginFailed)
        {
            this.onUnbind = onUnbind;
        }
    }
    
    /// <summary>
    /// 检测Apple账号绑定情况结果回调（封装了一层，更符合业务层面使用）
    /// </summary>
    public class OnCheckCandidateByAppleListenr : OnLoginListener
    {
        public delegate void OnUnbind(IGGAppleLoginContext context);

        public OnUnbind onUnbind;

        public OnCheckCandidateByAppleListenr(OnUnbind onUnbind, OnLoginSuccess onLoginSuccess, OnLoginFailed onLoginFailed) : base(onLoginSuccess, onLoginFailed)
        {
            this.onUnbind = onUnbind;
        }
    }

    /// <summary>
    /// 绑定结果回调（封装了一层，更符合业务层面使用）
    /// </summary>
    public class OnBindListener
    {
        public delegate void OnBindSuccess(); // 绑定成功
        public delegate void OnBoundIGGID(string IGGID); // 已绑定到某个IGGID
        public delegate void OnBindFailed(IGGError error); // 绑定出错

        public OnBindSuccess onBindSuccess;
        public OnBoundIGGID onBoundIGGID;
        public OnBindFailed onBindFailed;


        public OnBindListener(OnBindSuccess onBindSuccess, OnBindFailed onBindFailed, OnBoundIGGID onBoundIGGID)
        {
            this.onBindSuccess = onBindSuccess;
            this.onBoundIGGID = onBoundIGGID;
            this.onBindFailed = onBindFailed;
        }
    }

    /// <summary>
    /// 加载账号信息结果回调（封装了一层，更符合业务层面使用）
    /// </summary>
    public class OnLoadUserListener
    {
        public delegate void OnLoadUserSuccess(IGGUserProfile userProfile);  // 加载成功
        public delegate void OnLoadUserFailed(IGGError error); // 加载失败
        public delegate void OnGuestBindState(GuestBindState guestBindState); // 设备绑定情况
        public delegate void OnBindInfoGoogle(IGGUserBindingProfile profile); // Google账号已绑定
        public delegate void OnUnboundGoogle(); // Google账号未绑定
        public delegate void OnBindInfoFacebook(IGGUserBindingProfile profile); // FB账号已绑定
        public delegate void OnUnboundFacebook(); // FB账号未绑定
        public delegate void OnAccountSafetyStatus(bool isAccountSafe); // 当前账号是否安全
        public OnLoadUserSuccess onLoadUserSuccess;
        public OnLoadUserFailed onLoadUserFailed;
        public OnGuestBindState onGuestBindState;


        public delegate void OnBindInfo(IGGLoginType loginType, IGGUserBindingProfile profile);
        public delegate void OnUnbound(IGGLoginType loginType);
        public OnBindInfo onBindInfo;
        public OnUnbound onUnbound;

        public OnLoadUserListener(OnLoadUserSuccess onLoadUserSuccess, OnLoadUserFailed onLoadUserFailed)
        {
            this.onLoadUserSuccess = onLoadUserSuccess;
            this.onLoadUserFailed = onLoadUserFailed;
        }
        public OnLoadUserListener(OnLoadUserSuccess onLoadUserSuccess, OnLoadUserFailed onLoadUserFailed, OnGuestBindState onGuestBindState, OnBindInfo onBindInfo, OnUnbound onUnbound)
        {
            this.onLoadUserSuccess = onLoadUserSuccess;
            this.onLoadUserFailed = onLoadUserFailed;
            this.onGuestBindState = onGuestBindState;
            this.onBindInfo = onBindInfo;
            this.onUnbound = onUnbound;
        }
    }

    /// <summary>
    /// 通过设备切换账号结果回调（封装了一层，更符合业务层面使用）
    /// </summary>
    public class OnSwitchGuestLoginListener
    {
        public delegate void OnIGGIDSameAsNow(string IGGID); // 想切换的账号跟当前登录的账号一致（IGGID与登录类型都一样）
        public delegate void OnUnbind(); // 当前设备还未绑定一个IGGID，将会提示玩家是否已新的账号登录
        public delegate void OnBindDifIGGID(string IGGID); // 想切换的账号跟当前登录的账号不一致
        public delegate void OnBindDifLoginType(string IGGID); // 想切换的账号跟当前登录的账号一致，但登录类型不一致
        public delegate void OnLoginFailed(IGGError error); // 切换失败
        public OnIGGIDSameAsNow onIGGIDSameAsNow;
        public OnUnbind onUnbind;
        public OnBindDifIGGID onBindDifIGGID;
        public OnBindDifLoginType onBindDifLoginType;
        public OnLoginFailed onLoginFailed;

        public OnSwitchGuestLoginListener(OnIGGIDSameAsNow onIGGIDSameAsNow, OnUnbind onUnbind, OnBindDifIGGID onBindDifIGGID, OnBindDifLoginType onBindDifLoginType, OnLoginFailed onLoginFailed)
        {
            this.onIGGIDSameAsNow = onIGGIDSameAsNow;
            this.onUnbind = onUnbind;
            this.onBindDifIGGID = onBindDifIGGID;
            this.onBindDifLoginType = onBindDifLoginType;
            this.onLoginFailed = onLoginFailed;
        }
    }

    /// <summary>
    /// 通过第三方平台账号切换游戏账号结果回调（封装了一层，更符合业务层面使用）
    /// </summary>
    public class OnSwitchLoginByThirdAccountListener
    {
        public delegate void OnIGGIDSameAsNow(IGGThirdPartyAuthorizationProfile profile, string IGGID); // 想切换的账号跟当前登录的账号一致（IGGID与登录类型都一样）
        public delegate void OnUnbind(IGGThirdPartyAuthorizationProfile profile); // 当前第三方平台账号还未绑定一个IGGID，将会提示玩家是否已新的账号登录
        public delegate void OnBindDifIGGID(IGGThirdPartyAuthorizationProfile profile, string IGGID); // 想切换的账号跟当前登录的账号不一致
        public delegate void OnBindDifLoginType(IGGThirdPartyAuthorizationProfile profile, string IGGID); // 想切换的账号跟当前登录的账号一致，但登录类型不一致
        public delegate void OnLoginFailed(IGGError error); // 切换失败

        public OnIGGIDSameAsNow onIGGIDSameAsNow;
        public OnUnbind onUnbind;
        public OnBindDifIGGID onBindDifIGGID;
        public OnBindDifLoginType onBindDifLoginType;
        public OnLoginFailed onLoginFailed;

        public OnSwitchLoginByThirdAccountListener(OnIGGIDSameAsNow onIGGIDSameAsNow, OnUnbind onUnbind, OnBindDifIGGID onBindDifIGGID, OnBindDifLoginType onBindDifLoginType, OnLoginFailed onLoginFailed)
        {
            this.onIGGIDSameAsNow = onIGGIDSameAsNow;
            this.onUnbind = onUnbind;
            this.onBindDifIGGID = onBindDifIGGID;
            this.onBindDifLoginType = onBindDifLoginType;
            this.onLoginFailed = onLoginFailed;
        }
    }

    /// <summary>
    /// 通过IGG通行证账号切换游戏账号结果回调（封装了一层，更符合业务层面使用）
    /// </summary>
    public class OnSwitchLoginByIGGAccountListener
    {
        public delegate void OnIGGIDSameAsNow(IGGAccountLoginContext context, string IGGID); // 想切换的账号跟当前登录的账号一致（IGGID与登录类型都一样）
        public delegate void OnUnbind(IGGAccountLoginContext context); // 当前IGG通行证账号还未绑定一个IGGID，将会提示玩家是否已新的账号登录
        public delegate void OnBindDifIGGID(IGGAccountLoginContext context, string IGGID); // 想切换的账号跟当前登录的账号不一致
        public delegate void OnBindDifLoginType(IGGAccountLoginContext context, string IGGID); // 想切换的账号跟当前登录的账号一致，但登录类型不一致
        public delegate void OnLoginFailed(IGGError error); // 切换失败

        public OnIGGIDSameAsNow onIGGIDSameAsNow;
        public OnUnbind onUnbind;
        public OnBindDifIGGID onBindDifIGGID;
        public OnBindDifLoginType onBindDifLoginType;
        public OnLoginFailed onLoginFailed;

        public OnSwitchLoginByIGGAccountListener(OnIGGIDSameAsNow onIGGIDSameAsNow, OnUnbind onUnbind, OnBindDifIGGID onBindDifIGGID, OnBindDifLoginType onBindDifLoginType, OnLoginFailed onLoginFailed)
        {
            this.onIGGIDSameAsNow = onIGGIDSameAsNow;
            this.onUnbind = onUnbind;
            this.onBindDifIGGID = onBindDifIGGID;
            this.onBindDifLoginType = onBindDifLoginType;
            this.onLoginFailed = onLoginFailed;
        }
    }
    
    /// <summary>
    /// 通过Apple账号切换游戏账号结果回调（封装了一层，更符合业务层面使用）
    /// </summary>
    public class OnSwitchLoginByIGGAppleListener
    {
        public delegate void OnIGGIDSameAsNow(IGGAppleLoginContext context, string IGGID); // 想切换的账号跟当前登录的账号一致（IGGID与登录类型都一样）
        public delegate void OnUnbind(IGGAppleLoginContext context); // 当前Apple账号还未绑定一个IGGID，将会提示玩家是否已新的账号登录
        public delegate void OnBindDifIGGID(IGGAppleLoginContext context, string IGGID); // 想切换的账号跟当前登录的账号不一致
        public delegate void OnBindDifLoginType(IGGAppleLoginContext context, string IGGID); // 想切换的账号跟当前登录的账号一致，但登录类型不一致
        public delegate void OnLoginFailed(IGGError error); // 切换失败

        public OnIGGIDSameAsNow onIGGIDSameAsNow;
        public OnUnbind onUnbind;
        public OnBindDifIGGID onBindDifIGGID;
        public OnBindDifLoginType onBindDifLoginType;
        public OnLoginFailed onLoginFailed;

        public OnSwitchLoginByIGGAppleListener(OnIGGIDSameAsNow onIGGIDSameAsNow, OnUnbind onUnbind, OnBindDifIGGID onBindDifIGGID, OnBindDifLoginType onBindDifLoginType, OnLoginFailed onLoginFailed)
        {
            this.onIGGIDSameAsNow = onIGGIDSameAsNow;
            this.onUnbind = onUnbind;
            this.onBindDifIGGID = onBindDifIGGID;
            this.onBindDifLoginType = onBindDifLoginType;
            this.onLoginFailed = onLoginFailed;
        }
    }

    /// <summary>
    /// 登录结果回调（封装了一层，更符合业务层面使用）
    /// </summary>
    public class OnLoginListener
    {
        public delegate void OnLoginSuccess(IGGSession session);  // 登录成功

        public delegate void OnLoginFailed(IGGError error); // 登录失败
        public OnLoginSuccess onLoginSuccess; 

        public OnLoginFailed onLoginFailed;

        public OnLoginListener(OnLoginSuccess onLoginSuccess, OnLoginFailed onLoginFailed)
        {
            this.onLoginSuccess = onLoginSuccess;
            this.onLoginFailed = onLoginFailed;
        }
    }
    
    public delegate void OnConfirmed();
}
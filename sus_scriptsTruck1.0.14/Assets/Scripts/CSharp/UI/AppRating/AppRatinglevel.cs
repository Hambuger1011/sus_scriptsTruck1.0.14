using IGG.SDK.Core.Error;
using IGG.SDK.Modules.AppConf.VO;
using IGG.SDK.Modules.AppRating;
using IGG.SDK.Modules.AppRating.Listener;
using IGG.SDK.Modules.AppRating.VO;
using IGG.SDK.Utils.Common;
using Script.Game.Helpers;
using UnityEngine;
using UnityEngine.UI;
using XLua;


public class AppRatinglevel :Singleton<AppRatinglevel>
{
    private IGGAppRating appRating;

    public void Init()
    {
        // 游戏评分模块实例获取请通过KungfuInstance.Get().GetPreparedAppRating()拿到
        appRating = KungfuInstance.Get().GetPreparedAppRating();
        
        Debug.LogError("AppRating 初始化成功 "+appRating);
    }
  
    public AppRatinglevel() { }
    
    private bool IsValidScore(string score) {
        if (!StringHelper.IsEmpty(score) && (StringHelper.Equals(score, "0")
                                          || StringHelper.Equals(score, "1") || StringHelper.Equals(score, "2")
                                          || StringHelper.Equals(score, "3") || StringHelper.Equals(score, "4")
                                          || StringHelper.Equals(score, "5"))) {
            return true;
        } else {
            return false;
        }
    }


    /// <summary>
    /// 评分，需玩家确认之后才能跳转到应用商店
    /// </summary>
    /// <summary>
    /// 请求跳转平台评分页面
    /// </summary>
    public void RequestRating(int like,string content,string score)
    {
        _like = like;
        _content = content;
        _score = score;
        
        if (appRating == null)
        {
            Debug.LogError("apprating == null,评分接口获取失败，初始化失败;");
            return;
        }
        // 获取游戏评分情况，在不同的配置下（appconfig中的配置）会触发不同的回调。
        // onDisabled:当配置为关闭游戏评分功能时触发。
        // onStarndardModeEnabled:当配置为标准模式时触发，游戏研发根据IGGStarndardAppRating实例实现标准的评分流程。
        // onMinimizedModeEnabled:当配置为简化模式时触发，游戏研发根据IGGMinimizedAppRating实例实现简化的评分流程。
        // onError:发生错误，研发那边做相应处理。
        appRating.RequestReview(new AppRatingRequestReviewDemoListener(OnStarndardModeEnabledImpl, OnMinimizedModeEnabledImpl, OnDisabledImpl, OnErrorImpl));
    }



    private IGGStarndardAppRating _starndardAppRating = null;

    //1 喜欢  2不喜欢
    public int _like = 0;
    public string _content = "";
    public string _score = "";
    
    public void OnStarndardModeEnabledImpl(IGGStarndardAppRating starndardAppRating)
    {
        
        _starndardAppRating = starndardAppRating;
        
        // 标准模式：一般先让玩家选择是否喜欢我们的游戏，当玩家选择喜欢的话，则让玩家提示玩家进到应用商店进行评价，当
        // 玩家选择不喜欢的话，则提示玩家留下宝贵的建议。
        LOG.Log("标准版评分模式。");
        Debug.LogError("标准版评分模式。");

        if (_like == 1)
        {
            _starndardAppRating.Like(delegate(IGGError error)
            {
                // 触发这个回调之后，一般都会进到应用商店进行评分，如果没进入请查看手机环境
                LOG.Log("成功进入评分。");
                XLuaManager.Instance.GetLuaEnv().DoString(@"GameHelper.SetIGGPlatformComplet()");
                
            });
        }
        else if(_like==2)
        {
            LOG.Log("建议内容：建议内容：建议内容：建议内容："+_content);
            // 原生形式

           // UITipsMgr.Instance.PopupTips("Thank you very much！", false); 
 
            // 目前type只定义了1这个类型，所以研发那边可以写死这个值
            IGGAppRatingFeedback appRatingFeedback = new IGGAppRatingFeedback(int.Parse(_score), "1", _content);
            _starndardAppRating.Feedback(appRatingFeedback, delegate(IGGError error)
            {
                XLuaManager.Instance.GetLuaEnv().DoString(@"GameHelper.SetFeedbackSend()");
                //提交结果  error.GetReadableUniqueCode()
            });
        }



    }

    public void EnterLike()
    {
        _starndardAppRating.Like(delegate(IGGError error)
        {
            // 触发这个回调之后，一般都会进到应用商店进行评分，如果没进入请查看手机环境
            LOG.Log("成功进入评分。");
        });
    }

    public void feedBack(string content, string score)
    {
        // 这边分通过网页方式反馈与原生方式反馈，两种方式二选一，Demo只是为了演示，所以都实现了。
        // // 网页方式：
        // starndardAppRating.GetFeedbackWebPageURL(delegate(string url)
        // {
        //     IGGLog.Debug($"FeedbackWebPageURL:{url}");
        //     // 网页跳转由研发自己实现。
        //     // IGGNativeUtils.ShareInstance().OpenBrowser(url);
        // });
        
        Debug.LogError("建议内容：建议内容：建议内容：建议内容："+content);
        // 原生形式
        if (StringHelper.IsEmpty(content)) {
            LOG.Log( "反馈内容不能为空。" );
            return;
        }
                    
        if (!IsValidScore(score)) {
            LOG.Log( "分数请输入0-5之前的整数。" );
            return;
        }
                    
 
        // 目前type只定义了1这个类型，所以研发那边可以写死这个值
        IGGAppRatingFeedback appRatingFeedback = new IGGAppRatingFeedback(int.Parse(score), "1", content);
        _starndardAppRating.Feedback(appRatingFeedback, delegate(IGGError error)
        {
            
            //提交结果  error.GetReadableUniqueCode()
        });
    }


    /// <summary>
    /// 进入简化模式
    /// </summary>
    /// <param name="minimizedAppRating"></param>
    public void OnMinimizedModeEnabledImpl(IGGMinimizedAppRating minimizedAppRating)
    {
        // 简化模式：直接提示玩家是否要进到应用商店对我们的游戏进行评分
        //ViewUtil.ShowToast("简化版评分模式。");
        minimizedAppRating.GoRating(delegate (IGGError error)
        {
            if (error.IsNone())
            {
                // 触发这个回调之后，一般都会进到应用商店进行评分，如果没进入请查看手机环境
                UITipsMgr.Instance.PopupTips("Successfully entered Rating！", false);
            }
            else
            {
                // 触发这个回调之后，一般都会进到应用商店进行评分，如果没进入请查看手机环境
                UITipsMgr.Instance.PopupTips("[QA AppRating] User Cancel。", false);
                Debug.Log("[QA AppRating] User Cancel。");
            }
        });


    }

    public void OnDisabledImpl(IGGAppRatingStatus appRatingStatus)
    {
        // 评价关闭模式：不进行任何处理
        LOG.Log("评分功能被关闭，研发不需要处理。");
    }

    public void OnErrorImpl(IGGError error)
    {
        // 请求出错
        LOG.Log("请求评分失败:" + error.GetReadableUniqueCode());
    }
    
    private class AppRatingRequestReviewDemoListener : IIGGRequestReviewListener
    {
        private OnStarndardModeEnabled onStarndardModeEnabled;
        private OnMinimizedModeEnabled onMinimizedModeEnabled;
        private OnDisabled onDisabled;
        private OnError onError;


        public AppRatingRequestReviewDemoListener(OnStarndardModeEnabled onStarndardModeEnabled
            , OnMinimizedModeEnabled onMinimizedModeEnabled, OnDisabled onDisabled, OnError onError)
        {
            this.onStarndardModeEnabled = onStarndardModeEnabled;
            this.onMinimizedModeEnabled = onMinimizedModeEnabled;
            this.onDisabled = onDisabled;
            this.onError = onError;
        }
        
        public void OnStarndardModeEnabled(IGGStarndardAppRating starndardAppRating)
        {
            onStarndardModeEnabled?.Invoke(starndardAppRating);
        }

        public void OnMinimizedModeEnabled(IGGMinimizedAppRating minimizedAppRating)
        {
            onMinimizedModeEnabled?.Invoke(minimizedAppRating);
        }

        public void OnDisabled(IGGAppRatingStatus appRatingStatus)
        {
            onDisabled?.Invoke(appRatingStatus);
        }

        public void OnError(IGGError error)
        {
            onError?.Invoke(error);
        }
    }

    private delegate void OnStarndardModeEnabled(IGGStarndardAppRating starndardAppRating);

    private delegate void OnMinimizedModeEnabled(IGGMinimizedAppRating minimizedAppRating);

    private delegate void OnDisabled(IGGAppRatingStatus appRatingStatus);

    private delegate void OnError(IGGError error);
}

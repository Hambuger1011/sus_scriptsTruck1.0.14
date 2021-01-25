using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UGUI;
using AB;

public class NoticeTempletSprite : MonoBehaviour {

    public Image TempletImage;

    private int type, idtype;
    private string picture;

    UnityObjectRefCount m_cacheImage;
    public UnityObjectRefCount cacheImage
    {
        get
        {
            return m_cacheImage;
        }
        set
        {
            if (m_cacheImage != null)
            {
                m_cacheImage.Release();
            }
            m_cacheImage = value;
        }
    }


    private void OnDestroy()
    {
        cacheImage = null;
    }

    private void IniteGame(int type,int idtype,string picture)
    {
        this.type = type;
        this.idtype = idtype;
        this.picture = picture;
        UIEventListener.AddOnClickListener(TempletImage.gameObject, ImageButtonOnclick);

        if (string.IsNullOrEmpty(this.picture))
        {
            LOG.Info("公告表中没有图片路径");
            return;
        }
        
        //TempletImage.sprite= ResourceManager.Instance.GetUISprite(this.picture);
        string imgName = null;
        if(type == 1)
        {
            imgName = (int.Parse(this.picture)-1).ToString();
        }
        else
        {
            imgName = this.picture;
        }
        TempletImage.sprite = ResourceManager.Instance.GetUISprite("BookDisplayForm/bg_picture");
        var mBookId = int.Parse(this.picture);
        ABSystem.ui.DownloadBanner(mBookId, (id, refCount) =>
        {
            if (TempletImage == null)
            {
                refCount.Release();
                return;
            }
            if (mBookId != id)
            {
                refCount.Release();
                return;
            }
            cacheImage = refCount;
            TempletImage.sprite = refCount.Get<Sprite>();
        });

    }

    private void ImageButtonOnclick(PointerEventData data)
    {
        if (this.type == 1)
        {
            //跳转到书本
            LOG.Info("跳转到书本，书本id是："+ this.idtype);
            if (this.idtype==0)
            {
                LOG.Info("需要跳转的书本不存在");
                return;
            }

            UserDataManager.Instance.UserData.CurSelectBookID =this.idtype;
            AudioManager.Instance.PlayTones(AudioTones.book_click);
            CUIManager.Instance.OpenForm(UIFormName.BookDisplayForm);
            var view = CUIManager.Instance.GetForm<BookDisplayForm>(UIFormName.BookDisplayForm);
            view.InitByBookID(UserDataManager.Instance.UserData.CurSelectBookID);

        }
        else if (this.type == 2)
        {
            //跳转到活动
            //LOG.Info("跳转到活动，活动id是：" + this.idtype);

            switch (this.idtype)
            {
                case 100://邀请好友
                    InviteFriends();
                    break;
                case 1002://VIP界面
                    VipFunction();
                    break;
                case 1003://超值礼包
                    PremiumGiftBag();
                    break;
                case 1004://谷歌Face登录界面
                    GooldFaceUI();
                    break;
                case 1005://商城钻石界面
                    OpenShopUI();
                    break;
                case 1006://打开看视频的弹窗
                    VideoUI();
                    break;
                default:                   
                    LOG.Info("活动id有误是：" + this.idtype);
                    break;
            }
        }
    }


    #region  邀请好友
    private void InviteFriends()
    {

    }
    #endregion

    #region VIP
    private void VipFunction()
    {
        LOG.Info("vip按钮被点击了");

    }
    #endregion

    #region 超级礼包
    private void PremiumGiftBag()
    {
        LOG.Info("点击了超级礼包");

    }
    #endregion

    #region 谷歌Face登录界面
    private void GooldFaceUI()
    {
        CUIManager.Instance.OpenForm(UIFormName.LoginForm);
        CUIManager.Instance.GetForm<LoginForm>(UIFormName.LoginForm).btnCloseToTrue();
    }
    #endregion

    #region  商城钻石界面
    private void OpenShopUI()
    {
        CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop).OpenChargeMoney_Diamonds(null);
    }
    #endregion

    #region 打开看视频的弹窗
    private void VideoUI()
    {
        TalkingDataManager.Instance.WatchTheAds(1);
        SdkMgr.Instance.ShowAds(LookVideoComplete);
    }

    private void LookVideoComplete(bool value)
    {
        if (value)
            GameHttpNet.Instance.GetAdsReward(1, GetAdsRewardCallBack);
    }

    private void GetAdsRewardCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetOrderToSubmitCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    AudioManager.Instance.PlayTones(AudioTones.RewardWin);
                    UserDataManager.Instance.adsRewardResultInfo = JsonHelper.JsonToObject<HttpInfoReturn<GetAdsRewardResultInfo>>(result);

                    UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.adsRewardResultInfo.data.bkey);
                    UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.adsRewardResultInfo.data.diamond);

                    //Vector3 startPos = new Vector3(-188, -355);
                    //Vector3 targetPos = new Vector3(174, 720);
                    //RewardShowData rewardShowData = new RewardShowData();
                    //rewardShowData.StartPos = startPos;
                    //rewardShowData.TargetPos = targetPos;
                    //rewardShowData.IsInputPos = false;
                    //rewardShowData.KeyNum = UserDataManager.Instance.adsRewardResultInfo.data.bkey;
                    //rewardShowData.DiamondNum = UserDataManager.Instance.adsRewardResultInfo.data.diamond;
                    //rewardShowData.Type = 1;
                    //EventDispatcher.Dispatch(EventEnum.GetRewardShow, rewardShowData);
                    TalkingDataManager.Instance.WatchTheAds(2);
                }
                else if (jo.code == 202)
                {
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(191);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("You've reached the limit for today!", false);
                }
                else if (jo.code == 208)
                {
                    AudioManager.Instance.PlayTones(AudioTones.LoseFail);
                }
            }, null);
        }
    }
    #endregion

    private void OnDisable()
    {
        UIEventListener.RemoveOnClickListener(TempletImage.gameObject, ImageButtonOnclick);
    }
}

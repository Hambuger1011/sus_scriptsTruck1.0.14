using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UGUI;
using DG.Tweening;
using pb;

/// <summary>
/// 邀请界面
/// </summary>
public class InviteForm : BaseUIForm 
{

    public Transform Content;
    public GameObject InviteItemView;

    public Text CodeTxt;
    public GameObject CopyBtn;
    public GameObject ShareBtn;
    public GameObject CloseBtn;
    public RectTransform RewardEffect, Movepos;

    private GameObject UseBtn;
    private InputField CodeInputField;
    private InfinityGridLayoutGroup mInfinityGridLayoutGroup;
    private int mAmount = 20;//设定列表数据的总的数量

    private List<InviteItemView> mInviteItemList;
    private List<InviteItemInfo> mInviteInfoList;

    private Image mTopBg;
    private void Awake()
    {
        mTopBg = transform.Find("InviteBg/top").GetComponent<Image>();
    }
    public override void OnOpen()
    {
        base.OnOpen();

        UIEventListener.AddOnClickListener(CopyBtn, CopyHandler);
        UIEventListener.AddOnClickListener(ShareBtn, ShareHandler);
        UIEventListener.AddOnClickListener(CloseBtn, CloseHandler);

        CodeInputField= transform.Find("Canvas/UseInfo/InputField").GetComponent<InputField>();
        UseBtn = transform.Find("Canvas/UseInfo/UseBtn").gameObject;
        if (UseBtn != null)
            UIEventListener.AddOnClickListener(UseBtn.gameObject, UseCodeHandler);

        mInfinityGridLayoutGroup = Content.GetComponent<InfinityGridLayoutGroup>();
        //mInfinityGridLayoutGroup.GetComponent<GridLayoutGroup>().cellSize = new Vector2(620, 180);
        //mInfinityGridLayoutGroup.GetComponent<GridLayoutGroup>().spacing = new Vector2(0, 1);
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.GetInviteRewardList(InviteRewardListHandler);

        Transform bgTrans = this.gameObject.transform.Find("Canvas");
        if (GameUtility.IpadAspectRatio() && bgTrans != null)
            bgTrans.localScale = Vector3.one * 0.7f;


        //【屏幕适配】
        float offect = XLuaHelper.UnSafeAreaNotFit(this.myForm, mTopBg.rectTransform(), 750, 92);
        bgTrans.rectTransform().offsetMax = new Vector2(0, -offect);


    }

    public override void OnClose()
    {
        base.OnClose();

        UIEventListener.RemoveOnClickListener(CopyBtn, CopyHandler);
        UIEventListener.RemoveOnClickListener(ShareBtn, ShareHandler);
        UIEventListener.RemoveOnClickListener(CloseBtn, CloseHandler);

        if (mInviteItemList != null)
        {
            int len = mInviteItemList.Count;
            for (int i = 0; i < len; i++)
            {
                InviteItemView itemView = mInviteItemList[i];
                if(itemView != null)
                {
                    itemView.Dispose();
                    GameObject.Destroy(itemView);
                    itemView = null;
                }
            }
            mInviteItemList = null;
        }
    }

    private void UseCodeHandler(PointerEventData data)
    {
        if (string.IsNullOrEmpty(CodeInputField.text))
        {
            var Localization = GameDataMgr.Instance.table.GetLocalizationById(176);
            UITipsMgr.Instance.PopupTips(Localization, false);

            //UITipsMgr.Instance.PopupTips("Place enter  invitation codes", false);
            return;
        }
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.InviteExchange(CodeInputField.text, EnterCodeHandler);
    }

    private void EnterCodeHandler(object value)
    {
        string result = value.ToString();
        LOG.Info("-----EnterCodeHandler---->" + result);
        if (result.Equals("error"))
        {
            LOG.Info("---EnterCodeHandler--这条协议错误");
            return;
        }
        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo != null)
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.receiveInviteResult = JsonHelper.JsonToObject<HttpInfoReturn<ReceiveInviteResult>>(value.ToString());
                    if (UserDataManager.Instance.receiveInviteResult != null && UserDataManager.Instance.receiveInviteResult.data != null)
                    {
                        int getKey = UserDataManager.Instance.receiveInviteResult.data.bkey - UserDataManager.Instance.UserData.KeyNum;
                        int getDiamond = UserDataManager.Instance.receiveInviteResult.data.diamond - UserDataManager.Instance.UserData.DiamondNum;

                        UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.receiveInviteResult.data.bkey);
                        UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.receiveInviteResult.data.diamond);

                        string tips = "";
                        if(getKey > 0 && getDiamond > 0)
                        {
                            tips = "Congratulations on getting " + getDiamond + " diamonds and " + getKey + " keys";
                        }else if(getKey > 0)
                        {
                            tips = "Congratulations on getting "+ getKey + " keys";
                        }else if (getDiamond > 0)
                        {
                            tips = "Congratulations on getting " + getDiamond + " diamonds";
                        }

                        if(!string.IsNullOrEmpty(tips))
                            UITipsMgr.Instance.PopupTips(tips, false);

                        int bookId = UserDataManager.Instance.receiveInviteResult.data.bookid;
                        if (bookId > 0 && UserDataManager.Instance.UserData.bookList.IndexOf(bookId) == -1)
                        {
                            UserDataManager.Instance.UserData.bookList.Add(bookId);
                            CUIManager.Instance.OpenForm(UIFormName.InviteGiftBag);
                            InvitGiftBag tem = CUIManager.Instance.GetForm<InvitGiftBag>(UIFormName.InviteGiftBag);
                            if (tem != null)
                                tem.Inite(bookId);
                        }
                    }
                }
                else if (jo.code == 205)
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(177);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("You can't use your own invitation code.", false);
                }
                else if (jo.code == 203)
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(178);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("This invitation code has been used.", false);
                }
                else if (jo.code == 202)
                {

                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(179);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Maximum number of invitations used today.", false);
                }
                else if (jo.code == 201)
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(180);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("This invitation code does not exist.", false);
                }
            }

        }, null);
    }

    private void CloseHandler(PointerEventData data)
    {
        CUIManager.Instance.CloseForm(UIFormName.InviteForm);
    }

    private void CopyHandler(PointerEventData data)
    {
        if (UserDataManager.Instance.inviteListInfo != null && UserDataManager.Instance.inviteListInfo.data != null &&
            UserDataManager.Instance.inviteListInfo.data.user_info != null)
        {

#if UNITY_IOS
            SdkMgr.Instance.shareSDK.CopyToClipboard(UserDataManager.Instance.inviteListInfo.data.user_info.codes);
#elif UNITY_EDITOR
            TextEditor t = new TextEditor();
            t.content = new GUIContent(UserDataManager.Instance.inviteListInfo.data.user_info.codes);
            t.OnFocus();
            t.Copy();
#elif UNITY_ANDROID
            SdkMgr.Instance.shareSDK.CopyToClipboard(UserDataManager.Instance.inviteListInfo.data.user_info.codes);
#endif

            var Localization = GameDataMgr.Instance.table.GetLocalizationById(181);
            UITipsMgr.Instance.PopupTips(Localization, false);

            //UITipsMgr.Instance.PopupTips("Copy success", false);

        }
    }

    private void ShareHandler(PointerEventData data)
    {
        if (UserDataManager.Instance.inviteListInfo != null && UserDataManager.Instance.inviteListInfo.data != null &&
            UserDataManager.Instance.inviteListInfo.data.user_info != null)
        {
            string linkUrl = "";
#if UNITY_ANDROID
            linkUrl = "https://play.google.com/store/apps/details?id=" + SdkMgr.packageName;
#endif
#if UNITY_IOS
        linkUrl = "itms-apps://itunes.apple.com/cn/app/id" + SdkMgr.IosAppId;
#endif

            string invoteCode = UserDataManager.Instance.inviteListInfo.data.user_info.codes;

            string invoteUrl = "http://www.onyxgames1.com/Invitation.html?code=" + invoteCode;
#if CHANNEL_HUAWEI
            invoteUrl = "http://www.onyxgames1.com/Invitation_huawei.html?code=" + invoteCode;
#endif


            string shareTxt = "Join Secrets' community and enter this invitation code '" + invoteCode + "' to get free Diamonds! Choose your own adventure in the most thrilling interactive story game. Play Secrets now!\n" + invoteUrl;
#if UNITY_ANDROID && !UNITY_EDITOR
            SdkMgr.Instance.shareSDK.ShareMsg(shareTxt);
#elif UNITY_IOS
            SdkMgr.Instance.shareSDK.NativeShare(shareTxt);
#endif
        }

    }

    private void InviteRewardListHandler(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----InviteRewardListHandler---->" + result);
        if (result.Equals("error"))
        {
            LOG.Info("---VIPCallBacke--这条协议错误");
            return;
        }
        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo != null)
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.inviteListInfo = JsonHelper.JsonToObject < HttpInfoReturn<InviteListInfo>>(arg.ToString());
                    if(UserDataManager.Instance.inviteListInfo != null && UserDataManager.Instance.inviteListInfo.data != null)
                    {
                        mInviteInfoList = UserDataManager.Instance.inviteListInfo.data.inviteList;
                        mInviteInfoList.Sort(ToSortInvireList);
                        CodeTxt.text = UserDataManager.Instance.inviteListInfo.data.user_info.codes;
                        mInfinityGridLayoutGroup.SetAmount(mInviteInfoList.Count);
                        mInfinityGridLayoutGroup.updateChildrenCallback = UpdateChildrenCallback;
                        InitList(UserDataManager.Instance.inviteListInfo.data.user_info, mInviteInfoList);
                    }
                }
            }

        }, null);
    }

    private int ToSortInvireList(InviteItemInfo vInfo1, InviteItemInfo vInfo2)
    {
        InviteUserInfo vUser_info = UserDataManager.Instance.inviteListInfo.data.user_info;
        if(vInfo1.is_receive == 1 && vInfo2.is_receive != 1)
        {
            return 1;
        }
        else if(vInfo1.is_receive != 1 && vInfo2.is_receive == 1)
        {
            return -1;
        }
        else if (vInfo1.is_receive != 1 && vInfo2.is_receive != 1)
        {
            int info1State = 0;     //0还没完成，1已经完成
            int info2State = 0;  
            if (vInfo1.type == 1)
                info1State = (vUser_info.exchange_invite >= vInfo1.param) ? 1 : 0;
            else if (vInfo1.type == 2)
                info1State = (vUser_info.newuser_invite >= vInfo1.param) ? 1 : 0;

            if (vInfo2.type == 1)
                info2State = (vUser_info.exchange_invite >= vInfo2.param) ? 1 : 0;
            else if (vInfo1.type == 2)
                info2State = (vUser_info.newuser_invite >= vInfo2.param) ? 1 : 0;

            if (info1State == 0 && info2State == 1)
                return 1;
            else if (info1State == 1 && info2State == 0)
                return -1;
            else
            {
                if (vInfo1.sort > vInfo2.sort)
                    return 1;
                else if (vInfo1.sort < vInfo2.sort)
                    return -1;
                else
                    return 0;
            }
                
        }
        return 0;
    }

    private void InitList(InviteUserInfo vUserInfo,List<InviteItemInfo> vList)
    {
        if (vList != null)
        {
            if (mInviteItemList == null)
            {
                mInviteItemList = new List<InviteItemView>();
                int len = 8;
                for (int i = 0; i < len; i++)
                {
                    GameObject go = Instantiate(InviteItemView);
                    go.transform.SetParent(Content.transform);
                    InviteItemView itemView = go.GetComponent<InviteItemView>();
                    if (itemView != null)
                    {
                        if (vList.Count > i)
                        {
                            itemView.Init(vUserInfo, vList[i], ReceiveRewardHandler);
                            itemView.gameObject.SetActive(true);                           
                        } 
                        else
                            itemView.gameObject.SetActive(false);

                        mInviteItemList.Add(itemView);
                    }
                }
            }
            else
            {
                int len = 7;
                for (int i = 0; i < len; i++)
                {
                    InviteItemView itemView = mInviteItemList[i];
                    if (itemView != null)
                    {
                        if (vList.Count > i)
                        {
                            itemView.Init(vUserInfo, vList[i], ReceiveRewardHandler);
                            itemView.gameObject.SetActive(true);
                        } 
                        else
                            itemView.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    private void UpdateChildrenCallback(int index, Transform trans)
    {
        if (UserDataManager.Instance.inviteListInfo != null && UserDataManager.Instance.inviteListInfo.data != null &&
            UserDataManager.Instance.inviteListInfo.data.inviteList != null)
        {

            if (index < UserDataManager.Instance.inviteListInfo.data.inviteList.Count)
            {
                if (mInviteInfoList[index] != null)
                {
                    trans.GetComponent<InviteItemView>().Init(UserDataManager.Instance.inviteListInfo.data.user_info, mInviteInfoList[index], ReceiveRewardHandler);
                }
            }
        }

        //ArrayList arr = MyBooksDisINSTANCE.Instance.returNewGoList();
        
    }

    private int LastRecieveId = 0;
    private void ReceiveRewardHandler(int vId)
    {
        LastRecieveId = vId;
        GameHttpNet.Instance.ReceiveInviteReward(vId, ReceiveRewardCallBack);
    }

    private void ReceiveRewardCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----ReceiveRewardCallBack------>" + result);
        if (result.Equals("error"))
        {
            LOG.Info("---EnterCodeHandler--这条协议错误");
            return;
        }
        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo != null)
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.receiveInviteResult = JsonHelper.JsonToObject<HttpInfoReturn<ReceiveInviteResult>>(result);
                    if (UserDataManager.Instance.receiveInviteResult != null && UserDataManager.Instance.receiveInviteResult.data != null)
                    {
                        //UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.receiveInviteResult.data.bkey);
                        //UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.receiveInviteResult.data.diamond);

                        UpdateState();

                        AudioManager.Instance.PlayTones(AudioTones.RewardWin);
                        UserDataManager.Instance.adsRewardResultInfo = JsonHelper.JsonToObject<HttpInfoReturn<GetAdsRewardResultInfo>>(result);

                        UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.receiveInviteResult.data.bkey);
                        UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.receiveInviteResult.data.diamond);

                        //#region 领取特效出现
                        ////领取特效出现                      
                        //RewardEffect.gameObject.SetActive(true);
                        //Vector3 targetPos = Vector3.zero;
                        //RewardEffect.transform.localPosition = targetPos;

                        //targetPos = Movepos.transform.localPosition;                                              
                        //RewardEffect.transform.DOLocalMove(targetPos, 1.0f).OnComplete(() => {
                        //    RewardEffect.gameObject.SetActive(false);
                        //    RewardShowData rewardShowData = new RewardShowData();
                        //    rewardShowData.KeyNum = UserDataManager.Instance.receiveInviteResult.data.bkey;
                        //    rewardShowData.DiamondNum = UserDataManager.Instance.receiveInviteResult.data.diamond;
                        //    UserDataManager.Instance.ResetMoney(1, UserDataManager.Instance.receiveInviteResult.data.bkey);
                        //    UserDataManager.Instance.ResetMoney(2, UserDataManager.Instance.receiveInviteResult.data.diamond);
                        //});
                        //end
                        //#endregion                      
                    }
                }
                else if (jo.code == 203)
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(182);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("Not eligible for the current award", false);
                }
                else if (jo.code == 202)
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(183);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("The current award has been received", false);
                }
                else if (jo.code == 201)
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(184);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("The reward does not exist", false);
                }
            }

        }, null);
    }

    private void UpdateState()
    {
        if (UserDataManager.Instance.inviteListInfo != null && UserDataManager.Instance.inviteListInfo.data != null &&
            UserDataManager.Instance.inviteListInfo.data.inviteList != null && mInviteInfoList != null)
        {
            int len = mInviteInfoList.Count;
            for(int i = 0 ;i<len;i++)
            {
                InviteItemInfo itemInfo = mInviteInfoList[i];
                if(itemInfo != null && itemInfo.invite_id == LastRecieveId)
                {
                    itemInfo.is_receive = 1;
                    if (itemInfo.rewardweek == 1)
                    {
                        UserDataManager.Instance.userInfo.data.userinfo.is_vip = 1;

                        var Localization = GameDataMgr.Instance.table.GetLocalizationById(185);
                        UITipsMgr.Instance.PopupTips(Localization, false);

                        //UITipsMgr.Instance.PopupTips("Congratulations! You've got a Weekly VIP Card as reward!", false);
                    }
                    break;
                }
            }

            len = mInviteItemList.Count;
            for(int i = 0;i<len;i++)
            {
                InviteItemView itemView = mInviteItemList[i];
                if(itemView != null && itemView.InviteItemInfo != null && itemView.InviteItemInfo.invite_id == LastRecieveId)
                {
                    itemView.InviteItemInfo.is_receive = 1;
                    itemView.Init(UserDataManager.Instance.inviteListInfo.data.user_info, itemView.InviteItemInfo, ReceiveRewardHandler);
                    break;
                }
            }
        }
    }
}

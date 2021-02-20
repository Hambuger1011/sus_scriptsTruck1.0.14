using ProtoBuf;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using pb;
using System;
using AB;
using com.moblink.unity3d;


public class ThirdLoginData
{
    
    public LoginDataInfo FaceBookLoginInfo;
    public LoginDataInfo GoogleLoginInfo;
    public LoginDataInfo ThirdPartyLoginInfo;
    public int LastLoginChannel = 0;    //上一次登陆的渠道 1:facebook 2:google,3:huawei,4:游客
}


[ProtoContract]
public class UserData
{
    [ProtoMember(1)]
    public string UserID;
    [ProtoMember(2)]
    public int KeyNum;
    [ProtoMember(3)]
    public int DiamondNum;
    [ProtoMember(4)]
    public List<BookData> BookDataList;
    [ProtoMember(5)]
    public int CurSelectBookID;
    [ProtoMember(6)]
    public int BgMusicIsOn;
    [ProtoMember(7)]
    public int TonesIsOn;
    [ProtoMember(8)]
    public int IsFirstEnter;
    [ProtoMember(9)]
    public int IsFirstLogin;
    [ProtoMember(10)]
    public int IsSelectFirstBook;
    [ProtoMember(11)]
    public int IsStartReadBook;
    [ProtoMember(13)]
    public string IdToken;
    [ProtoMember(14)]
    public string UserName;
    [ProtoMember(15)]
    public int PlatformType;    //平台标示，1:google 2:facebook
    [ProtoMember(16)]
    public string JPushId;
    [ProtoMember(17)]
    public int TicketNum;
    [ProtoMember(18)]
    public List<int> bookList;
    [ProtoMember(19)]
    public string bookNickName;
    [ProtoMember(20)]
    public float AutoSpeed;
    public UserData()
    {
        UserID = "";
        KeyNum = 0;
        DiamondNum = 0;
        CurSelectBookID = 0;
        BgMusicIsOn = 1;
        TonesIsOn = 1;
        IsFirstEnter = 0;
        IsFirstLogin = 0;
        IsSelectFirstBook = 0;
        IsStartReadBook = 0;
        IdToken = "";
        UserName = "";
        PlatformType = 0;
        JPushId = "";
        TicketNum = 0;
        bookNickName = "";
        bookList = new List<int>();

        BookDataList = new List<BookData>();
        AutoSpeed = 0f;
    }

    public override string ToString()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var item in BookDataList)
        {
            sb.Append(item.ToString() + "\n");
        }
        LOG.Warn(sb.ToString());
        return string.Format("UserID : {0}---KeyNum : {1}---DiamondNum : {2}---BookDataList : {3}", UserID, KeyNum, DiamondNum, BookDataList.Count);
    }
}


public class BookData
{
    public int BookID;
    public string BookName;
    public int ChapterID;
    public int DialogueID;
    public string PlayerName;
    public int PlayerDetailsID;
    public int PlayerClothes;
    public bool IsPhoneCallMode;
    public int MaxChapterID;
    public int PhoneRoleID;
    public string ClothesStr;
    public List<int> ClothesList;
    public string ChapterPayStr;
    public List<int> ChapterPayList;
    public int NpcId;
    public string NpcName;
    public int NpcDetailId;     //记录npc长什么样子的ID
    public bool IsBubbleMode;   //气泡聊天的模式
    public int chaptersCount;

    public int character_id;//model选择形象id ,需要选择时必填
    public int outfit_id;//model选择服装id,需要选择时必填
    public int hair_id;//model选择头发id,需要选择时必填

    public BookData()
    {
        BookID = 0;
        BookName = "Player";
        ChapterID = 1;
        DialogueID = 1;
        PlayerName = "PLAYER";
        PlayerDetailsID = 1;
        PlayerClothes = 1;
        IsPhoneCallMode = false;
        MaxChapterID = 0;
        PhoneRoleID = 0;
        ClothesStr = "";
        ClothesList = new List<int>();
        ChapterPayStr = "";
        ChapterPayList = new List<int>();
        IsBubbleMode = false;
    }

    public override string ToString()
    {
        return string.Format(@"
BookID : {0}  BookName : {1}  ChapterID : {2}  DialogueID : {3}  PlayerName : {4}  PlayerDetailsID : {5}  PlayerClothes : {6}  IsPhoneCallMode : {7}
NpcDetailId:{8} NpcId:{9} NpcName:{10}
"
            , 
            BookID, 
            BookName, 
            ChapterID, 
            DialogueID, 
            PlayerName, 
            PlayerDetailsID, 
            PlayerClothes, 
            IsPhoneCallMode,
            NpcDetailId,
            this.NpcId,
            this.NpcName
            );
    }
}


public class LoginDataInfo
{
    public string UserId;
    public string Token;
    public string Email;
    public string UserName;
    public string UserImageUrl;
    public int OpenType;
}

public class LoopBroadcastItemInfo
{
    public int id;
    public string msg;
    public int begin;
    public int end;
    public int gap;
    public int curShowTime;
    public int type;
    public string color;
}

//屏蔽字的替换
public class BannedWordsItem
{
    public int Start;
    public int len;
}



[XLua.LuaCallCSharp, XLua.Hotfix]
public class UserDataManager : Singleton<UserDataManager> {

    public readonly string path = Application.persistentDataPath;

    public HttpInfoReturn<ThirdPartyReturnInfo> thirdPartyLoginInfo; //第三方登陆信息
    public HttpInfoReturn<UserInfoCont> userInfo;
    public HttpInfoReturn<MoveCodeInfoCont> moveCodeInfo;
    public HttpInfoReturn<SelfBookInfo> selfBookInfo;    //我阅读的书本
    public HttpInfoReturn<SwitchStatus> switchStatus;    //开关配置状态
    public HttpInfoReturn<BookItemsInfo> bookItemsInfo;   //书架上的书本
    public HttpInfoReturn<DayLoginInfo> dayLoginInfo;   // 领取7天登陆奖励，返回的结果
    public HttpInfoReturn<FreeKeyInfo> freeKeyInfo;   // 领取2小时免费钥匙，返回的结果
    //public HttpInfoReturn<UserOptionCostResultInfo> userOptionCostResultInfo;   // 玩家选择选项后的结果
    public HttpInfoReturn<BookOptionCostCont<BookOptionCostItemInfo>> bookChapterOptionCostList;   // 书本章节付费列表
    public HttpInfoReturn<HiddenEggInfo> hiddenEggInfo;   // 领取彩蛋，返回的结果

    public HttpInfoReturn<NewUserEggInfo> newUserEggInfo;     // 新用户，新手彩蛋的信息
    public HttpInfoReturn<ChoicesClothResultInfo> choicesClothResultInfo;   // 用户服装或者角色扣费 ,结果信息
    public HttpInfoReturn<BookDetailInfo> bookDetailInfo;   // 获取指定书本的详细信息
    public HttpInfoReturn<ResetBookOrChapterResultInfo> resetChapterOrBookInfo;   // 重置书本章节或者书本时，返回的结果
    public HttpInfoReturn<SetProgressResultInfo> setProgressResultInfo;   // 设置用户进度后的，返回的结果
    public HttpInfoReturn<BookCostChapterListCont<BookCostChapterItemInfo>> bookCostChapterList;   // 设置用户进度后的，返回的结果
    public HttpInfoReturn<ShopListCont> shopList;   // 商品列表信息，返回的结果
    public HttpInfoReturn<MallAwardStatus> mallAwardStatus;   // 商品列表信息，返回的结果
    public HttpInfoReturn<MallAward> mallAward;   // 商品列表信息，返回的结果
    public HttpInfoReturn<OrderFormInfo> orderFormInfo;   // 生成用户充值订单信息
    public HttpInfoReturn<OrderFormSubmitResultInfo> orderFormSubmitResultInfo;   // 充值订单提交结果信息
    public HttpInfoReturn<HwBuyResultInfo> hwBuyResultInfo;   // 充值订单提交结果信息
    public HttpInfoReturn<GetAdsRewardResultInfo> adsRewardResultInfo;   // 看广告后的，结果信息
    public HttpInfoReturn<ClearRecordResultInfo> clearRecordResultInfo;   // 清除用户游戏记录，结果信息


    HttpInfoReturn<BuyChapterResultInfo> _buyChapterResultInfo;
    public HttpInfoReturn<BuyChapterResultInfo> buyChapterResultInfo   // 购买章节后，结果信息
    {
        get{
            return _buyChapterResultInfo;
        }
    }


    public void SetBuyChapterResultInfo(int bookID, HttpInfoReturn<BuyChapterResultInfo> info)
    {
        _buyChapterResultInfo = info;
        var otherTrait = _buyChapterResultInfo.data.other_trait;
        UserDataManager.Instance.SetBookPropertyValue(bookID, otherTrait);


        if (info.data.npc_detail != null && info.data.npc_detail.Count > 0)
        {
            UserDataManager.Instance.RecordNpcInfo(bookID, info.data.npc_detail[0]);
        }else
        {
            UserDataManager.Instance.RecordNpcInfo(bookID, null);
        }
    }

    public HttpInfoReturn<RecommendABookListCont> recommendABookListCont;//这个是推荐书本
    public HttpInfoReturn<VersionCont<GameVersionInfo>> gameVersionInfo;//获得最新的版本号
    public HttpInfoReturn<AuditStatusInfo> auditStatusInfo;//获得版本审核状态
    public HttpInfoReturn<VersionInfo> versionInfo;//获取版本信息-新
    public HttpInfoReturn<DiscussCommentResultInfo> discussCommentResultInfo;//用戶留言結果信息
    public HttpInfoReturn<ChapterCommentCont<ChapterCommentItemInfo>> chapterCommentList;//用戶留言列表
    public HttpInfoReturn<ChapterCommentCont<CommentBackInfo>> chapterCommenBackInfo;//评论回复列表
    public HttpInfoReturn<SendCommentResultCont> sendCommentResultInfo;//用戶发送留言结果
    public HttpInfoReturn<GetUserTickInfo> userTickInfo;    //用户当前抽奖状态信息
    public HttpInfoReturn<LuckyDrawInfo> luckyDrawInfo;    //用户抽奖,返回的结果信息
    public HttpInfoReturn<EmojiMsgListInfo> emojiMsgListInfo;    //获取指定对话的表情列表
    public HttpInfoReturn<TicketCont<TicketItemInfo>> tickListInfo;    //抽奖选项
    public HttpInfoReturn<EmailListCont> EmailList;//这个是用户邮箱信息
    public HttpInfoReturn<ReadEmaillListCont<ReadEmaillList>> ReadEmaillList;//这个是读取用户的邮件信息
    public HttpInfoReturn<EmailGetAwardInfo> emailGetAwardInfo;
    public HttpInfoReturn<FreeKeyApply> FreeKeyApplyInfo;
    public HttpInfoReturn<GetNewRecommandbook> RecommandbookList;
    public HttpInfoReturn<SocketInfo> socketInfo;       //获取socket连接的一些信息
    public HttpInfoReturn<NewInfoCont> NewInfoCont;
    public HttpInfoReturn<ShareAwardInfo> shareAward;   //分享奖励
    public HttpInfoReturn<GetBookGiftBag> GetBookGiftBag;
    public HttpInfoReturn<ActiveInfo> lotteryDrawInfo;
    public HttpInfoReturn<Getuserpackage> Getuserpackage;
    public HttpInfoReturn<Getuserpaymallid> Getuserpaymallid;
    public HttpInfoReturn<GetBulletinBoard> GetBulletinBoard;
    public HttpInfoReturn<Getvipcard<vipinfo>> Getvipcard;
    public HttpInfoReturn<Getvipcardreceive> Getvipcardreceive;
    public HttpInfoReturn<Getusertask> Getusertask;
    public HttpInfoReturn<Achievetaskprice> Achievetaskprice;
    public HttpInfoReturn<Getboxlist> Getboxlist;
    public HttpInfoReturn<InviteListInfo> inviteListInfo;
    public HttpInfoReturn<ReceiveInviteResult> receiveInviteResult;
    public HttpInfoReturn<Achieveallmsgprice> achieveallmsgprice;
    public HttpInfoReturn<Getcomment> Getcomment;
    public HttpInfoReturn<Getcommentreplay> Getcommentreplay;
    public HttpInfoReturn<ProfileData> profileData;
    public HttpInfoReturn<ProfileConfigInfo> profileConfigInfo;
    public HttpInfoReturn<ProfileRefreshData> profileRefreshData;

   
    public HttpInfoReturn<BookOptionSelectInfo> BookOptSelInfo;
    public HttpInfoReturn<Getrecommandmall> recommandmallInfo;
    public HttpInfoReturn<IpAdressInfo> userIpAddressInfo;
    
    public HttpInfoReturn<BookJDTFormSever> bookJDTFormSever;
    public HttpInfoReturn<VersionBookDetailInfo> VersionBookDetailInfo;
    public HttpInfoReturn<VersionChapterInfo> VersionChapterInfo;
    public HttpInfoReturn<VersionChapterList> VersionChapterList;
    public HttpInfoReturn<VersionDialogList> VersionDialogList;
    public HttpInfoReturn<VersionSkinList> VersionSkinInfo;
    public HttpInfoReturn<VersionClothesPriceList> VersionClothesPriceInfo;
    public HttpInfoReturn<VersionRoleModelList> VersionRoleModelInfo;
    public HttpInfoReturn<VersionModelPriceList> VersionModelPriceInfo;
 

    public HttpInfoReturn<GameFunStateList> gameFunStateList;
    public HttpInfoReturn<SaveStep> SaveStepInfo; //进度保存数据
    public HttpInfoReturn<BooksUpdatedWeekly> BooksUpdatedWeekly;
    public HttpInfoReturn<BookNotUser> BookNotUserList;
    public HttpInfoReturn<WriterIndexInfo> WriterIndexInfo;
    public HttpInfoReturn<GetWriterHotBookList> GetWriterHotBookList;

    public HttpInfoReturn<BookBarrageCountList> BookBarrageCountList;
    public HttpInfoReturn<BookBarrageInfoList> BookBarrageInfoList;

    public bool is_use_prop = false; //是否使用钥匙优惠价道具: 1.使用道具 0不使用（非必传，默认不使用）
    public PropInfoItem propInfoItem; //当前使用的道具信息
    public PropInfo userPropInfo_Outfit;//装扮类型道具信息
    public PropInfo userPropInfo_Choice;//选项类型道具信息
    public PropInfo userPropInfo_Key;   //钥匙类型道具信息
    public int PropType = 0;
    public int SendSeq = 0;


    public void SetLuckyPropItem(bool isUse,int type,PropInfoItem item=null)
    {
        if (item != null)
        {
            is_use_prop = isUse;
            propInfoItem = item;
            PropType = type;
            SendSeq = GameHttpNet.Instance.SendSeq + 1;
        }
        else
        {
            is_use_prop = false;
            propInfoItem = null;
        }
        EventDispatcher.Dispatch(EventEnum.SetPropItem, propInfoItem);
    }
    //public void UpdatePropItem()
    public void UpdatePropItemWhenServerCallback()
    {
        if (is_use_prop)
        {
            if (propInfoItem == null)LOG.Warn("[recv]使用道具信息异常");
            else
            {
                propInfoItem.prop_num--;
                if (propInfoItem.prop_num == 0)
                {
                    switch (PropType)
                    {
                        case 1:
                            for (int i = userPropInfo_Outfit.discount_list.Count-1; i >= 0; i--)
                            {
                                if (userPropInfo_Outfit.discount_list[i].discount == propInfoItem.discount)
                                {
                                    userPropInfo_Outfit.discount_list.Remove(userPropInfo_Outfit.discount_list[i]);
                                }
                            }
                            break;
                        case 2:
                            for (int i = userPropInfo_Key.discount_list.Count-1; i >= 0; i--)
                            {
                                if (userPropInfo_Key.discount_list[i].discount == propInfoItem.discount)
                                {
                                    userPropInfo_Key.discount_list.Remove(userPropInfo_Key.discount_list[i]);
                                }
                            }
                            break;
                        case 3:
                            for (int i = userPropInfo_Choice.discount_list.Count-1; i >= 0; i--)
                            {
                                if (userPropInfo_Choice.discount_list[i].discount == propInfoItem.discount)
                                {
                                    userPropInfo_Choice.discount_list.Remove(userPropInfo_Choice.discount_list[i]);
                                }
                            }
                            break;
                    }
                }
            }
        }

        // if (GameHttpNet.Instance.SendSeq == SendSeq)
        // {
        is_use_prop = false;
        propInfoItem = null;
        // }
        EventDispatcher.Dispatch(EventEnum.SetPropItem, propInfoItem);
    }



    public AppconfInfo appconfinfo;

    public UserData UserData { get { return m_userData; } }
    private UserData m_userData;
    public ThirdLoginData LoginInfo { get { return mLoginInfo; } set { mLoginInfo = value; } }
    private ThirdLoginData mLoginInfo;
    
    //账号迁移时账号类型标识
    public int logintType = 0;

    public bool isReadNewerBook = false;

    /// <summary>
    /// 缓存广播列表
    /// </summary>
    public Queue<string> GMBroadcastQueue = new Queue<string>();
    public Queue<string> BroadcastQueue = new Queue<string>();
    public List<LoopBroadcastItemInfo> LoopBroadcastList = new List<LoopBroadcastItemInfo>();

    public HwUserInfo hwLoginInfo;

    private int mBroadcastIdIndex = 1;
    private int mBillingReqIndex = 0;
    private int mAdmobReqIndex = 0;

    public int Version = 0;     //客戶端版本号
    public string ResVersion = "";  //客户端资源版本号
    public string DataTableVersion = "";  //配置表资源版本号

    public string UserCountry = ""; //玩家所在的国家

    public bool ToPrintResPath = false;


    public bool FBHadInit = false;
    public bool LoginGetTrait = false;

    public int CatAction;//存储猫播放的动作

    public bool CatTimeDwonOpen = false;

    public int InPlaceCatThings = 0;    //当前猫的场景在放置的内容，1：食物，2：装饰物

    public bool TemListFirst = false;//是否只是初始一次temlist

    public bool IsCatUiOpent = false;

    public int GuidStupNum = 0;//保存猫的引导到达哪个引导号了
    public Vector3 GuidPos;   //保存需要引导点击的位置
    public Vector3 GuidFingerPos;   //保存引导提示框的位置
    public Vector3 GuidFingerEFPos; //保存手指特效的位置
    //public Vector3 CatFoodBowlPos;//保存猫的碗的位置
    public bool isFirstCatEnt = false;//是否是新进入猫的引导
    public bool HuangyuandianYesOnclickRepair = false; //黄圆垫点击确认购买是不是需要步骤修补
    public int GuidFirstStupNum = 0;//保存第一次获得的猫的引导步骤数
    public bool CatGuidIsCanTouch = false;//保存猫的引导是否可以执行下一步的点击
    public int FoodNum;
    public bool Nofistenter=false;//进入游戏第一次触发
    public int MasOpenType = 0;//这个是记录创作界面打开更多界面的时候，记录的类型  1 是最热 2是最新
    public float TopHight = 0;//记录顶部栏的高度

    private int nowBookID;
    public int NowBookID { get; set; }


    public string IGGid = "";
    public string Accesskey = "";
    public bool isMoveCode = false;//账号迁移标识

    public bool SigningIn = false;

    public string InviteCode = String.Empty;

    public UserDataManager() { }

    public void SetInviteCode()
    {
        InviteCode = MobLink.InviteCode;
    }

    public int GetBroadcastId()
    {
        return mBroadcastIdIndex += 1;
    }
    protected override void Init()
    {
        base.Init();
        m_userData = new UserData();
        mLoginInfo = new ThirdLoginData();
        appconfinfo = new AppconfInfo();
        ReadInfoByPlayPre();

        CTimerManager.Instance.AddTimer(5000, -1, OnTimeUpHandler);
    }

    private void OnTimeUpHandler(int vQueue)
    {
        //检查广播的内容
        CheckBroadcast();

        //给服务器发送心跳
        CheckUserHeartBeat();


#if UNITY_ANDROID
        //检查安卓付费初始化的状态
        //CheckBillIsInit();
#endif
        //检查视频初始化的状态
        CheckAdsIsInit();
    }

    #region 广播
    private void CheckBroadcast()
    {
        if (LoopBroadcastList.Count > 0)
        {
            List<LoopBroadcastItemInfo> removeList = new List<LoopBroadcastItemInfo>();
            int len = LoopBroadcastList.Count;
            for (int i = 0; i < len; i++)
            {
                LoopBroadcastItemInfo itemInfo = LoopBroadcastList[i];
                if (itemInfo != null)
                {
                    if (itemInfo.begin <= GameDataMgr.Instance.GetCurrentUTCTime() && itemInfo.end >= GameDataMgr.Instance.GetCurrentUTCTime())
                    {
                        if (itemInfo.curShowTime + itemInfo.gap <= GameDataMgr.Instance.GetCurrentUTCTime())
                        {
                            if (itemInfo.type == 1)
                                AddGMBroadcast((itemInfo.msg));
                            else
                                AddBroadcast(itemInfo.msg);
                            
                            UIAlertMgr.Instance.BroadcastShow();
                            LoopBroadcastList[i].curShowTime = GameDataMgr.Instance.GetCurrentUTCTime();
                        }
                    }
                    else if (itemInfo.end < GameDataMgr.Instance.GetCurrentUTCTime())
                    {
                        removeList.Add(itemInfo);
                    }
                }
            }

            if (removeList.Count > 0)
            {
                int reLen = removeList.Count;
                for (int i = 0; i < reLen; i++)
                {
                    LoopBroadcastItemInfo reItem = removeList[i];
                    int tempLen = LoopBroadcastList.Count;
                    for (int j = 0; j < tempLen; j++)
                    {
                        LoopBroadcastItemInfo tempItem = LoopBroadcastList[j];
                        if (tempItem != null && tempItem.id == reItem.id)
                        {
                            LoopBroadcastList.RemoveAt(j);
                            break;
                        }
                    }
                }
            }
        }
    }
    #endregion

    private int mHBTimesCount = 0;
    private void CheckUserHeartBeat()
    {
        if (userInfo == null || SigningIn) return;
        mHBTimesCount++;
        //60 = 60/5 * 5; 即5分钟累积的5秒的轮询检测的次数
        if (mHBTimesCount > 60)
        {
            mHBTimesCount = 0;
            GameHttpNet.Instance.SetUserHeartBeat((long responseCode, string result) => { LOG.Info("===CheckUserHeartBeat====>" + result); });
        }
    }

//    private void CheckBillIsInit()
//    {
//        mBillingReqIndex++;
//        if (!SdkMgr.Instance.BillingSysInit && mBillingReqIndex >= 2)
//        {
//            mBillingReqIndex = 0;
//#if UNITY_EDITOR
//            return;
//#endif
//            //LOG.Info("===============CheckBillIsInit==============>>");
//#if CHANNEL_ONYX || CHANNEL_SPAIN
//            SdkMgr.Instance.google.InitPay();
//#endif
//        }
//    }

    private void CheckAdsIsInit()
    {
        SdkMgr.Instance.CheckAdsIsInit();
    }
    
    protected override void UnInit()
    {
        base.UnInit();
        m_userData = null;
    }

    /// <summary>
    /// 计算钥匙数
    /// </summary>
    /// <param name="num"></param>
    public void CalculateKeyNum(int num)
    {
        m_userData.KeyNum += num;
        if (m_userData.KeyNum < 0)
            m_userData.KeyNum = 0;
        EventDispatcher.Dispatch(EventEnum.OnKeyNumChange, m_userData.KeyNum);
    }

    /// <summary>
    /// 计算钻石数
    /// </summary>
    /// <param name="num"></param>
    public void CalculateDiamondNum(int num)
    {
        m_userData.DiamondNum += num;
        if (m_userData.DiamondNum < 0)
            m_userData.DiamondNum = 0;
        EventDispatcher.Dispatch(EventEnum.OnDiamondNumChange, m_userData.DiamondNum);
    }

    public void CatResetMoney(int vNum)
    {
        m_userData.DiamondNum = vNum;
    }

    /// <summary>
    /// 重置玩家money的数量
    /// </summary>
    /// <param name="vType">1;钥匙，2：钻石</param>
    /// <param name="vNum"></param>
    public void ResetMoney(int vType, int vNum, bool vShow = true)
    {
        if (vType == 1)
        {
            m_userData.KeyNum = vNum;
            EventDispatcher.Dispatch(EventEnum.OnKeyNumChange, m_userData.KeyNum, (vShow) ? "" : EventEnum.LoginChangeMoney);
            CheckFreeKeyState();
            GamePointKeyNum(vNum);
        }
        else if (vType == 2)
        {
            m_userData.DiamondNum = vNum;
            EventDispatcher.Dispatch(EventEnum.OnDiamondNumChange, m_userData.DiamondNum, (vShow) ? "" : EventEnum.LoginChangeMoney);
            GamePointDiamondNum(vNum);
        }
        else if (vType == 3)
        {
            m_userData.TicketNum = vNum;
            EventDispatcher.Dispatch(EventEnum.OnTicketNumChange, m_userData.TicketNum, (vShow) ? "" : EventEnum.LoginChangeMoney);
        }
    }
    /// <summary>
    /// 钥匙埋点
    /// </summary>
    private int _keyNum = 0;
    private int _diamondNum = 0;
    private void GamePointKeyNum(int vNum)
    {
        if (_keyNum > 0)
        {
            if (_keyNum > vNum)
            {
                int _num = _keyNum - vNum;

                //埋点*钥匙消耗
                GamePointManager.Instance.BuriedPoint(EventEnum.UseKey, "", "", "", "", _num.ToString());
            }
            else if (_keyNum < vNum)
            {
                int _num = vNum - _keyNum;
                //埋点*钥匙获得
                GamePointManager.Instance.BuriedPoint(EventEnum.GetKey, "", "", "", "", _num.ToString());
            }
        }
        _keyNum = vNum;
    }

    /// <summary>
    /// 钻石埋点
    /// </summary>
    private void GamePointDiamondNum(int vNum)
    {
        if (_diamondNum > 0)
        {
            if (_diamondNum > vNum)
            {
                int _num = _diamondNum - vNum;

                //埋点*钻石消耗
                GamePointManager.Instance.BuriedPoint(EventEnum.UseDiamond, "", "", "", "", _num.ToString());
            }
            else if (_diamondNum < vNum)
            {
                int _num = vNum - _diamondNum;
                //埋点*钻石获得
                GamePointManager.Instance.BuriedPoint(EventEnum.GetDiamond, "", "", "", "", _num.ToString());
            }
        }
        _diamondNum = vNum;
    }

    //检查免费钥匙
    private void CheckFreeKeyState()
    {
        if (userInfo != null && userInfo.data != null && m_userData.KeyNum < 2 && userInfo.data.userinfo.end_time <= 0 && userInfo.data.userinfo.free_key > 0)
        {
            GameHttpNet.Instance.FreeKeyApply((object arg) =>
            {
                LoomUtil.QueueOnMainThread((param) =>
                {
                    JsonObject jo = JsonHelper.JsonToJObject(arg.ToString());
                    if (jo.code == 200)
                    {
                        FreeKeyApplyInfo = JsonHelper.JsonToObject<HttpInfoReturn<FreeKeyApply>>(arg.ToString());
                        if (FreeKeyApplyInfo != null)
                            userInfo.data.userinfo.end_time = FreeKeyApplyInfo.data.end_time;
                    }
                }, null);
            });
        }
    }

    /// <summary>
    /// 选择选项后重置玩家的货币
    /// </summary>
    public void OptionCostResultMoneyReset()
    {
        if (SaveStepInfo != null && SaveStepInfo.data != null)
        {
            ResetMoney(1, SaveStepInfo.data.user_key);
            ResetMoney(2, SaveStepInfo.data.user_diamond);
        }
    }

    
    public void AddProp(int vId,int vNum)
    {
        vNum = vNum > 99 ? 99 : vNum;
        var propInfo = vId + (float)vNum / 100;
        EventDispatcher.Dispatch(EventEnum.AddProp, propInfo);
    }

    /// <summary>
    /// 记录玩家书本阅读的进度
    /// </summary>
    public void InitRecordServerBookData()
    {
        if (UserDataManager.Instance.selfBookInfo != null && UserDataManager.Instance.selfBookInfo.data.favorite_book != null)
        {
            List<SelfBookShelfItemInfo> bookList = UserDataManager.Instance.selfBookInfo.data.favorite_book;
            List<BookData> BookDataList = new List<BookData>();
            m_userData.BookDataList = new List<BookData>();
            int len = bookList.Count;


            for (int i = 0; i < len; ++i)
            {
                SelfBookShelfItemInfo shelfItemInfo = bookList[i];
                if (shelfItemInfo != null)
                {
                    BookData bookData = new BookData();
                    bookData.BookID = shelfItemInfo.bookid;
                    bookData.BookName = shelfItemInfo.BookName;
                    bookData.DialogueID = shelfItemInfo.dialogid;
                    //bookData.ChapterID = shelfItemInfo.chapterid;
                    bookData.chaptersCount = shelfItemInfo.ChapterCount;
                    if (bookData.PlayerDetailsID == 0)
                        bookData.PlayerDetailsID = 1;

                    //t_BookDialog dialogData = GameDataMgr.Instance.table.GetDialogById(bookData.DialogueID);
                    //    bookData.ChapterID = (dialogData != null)?dialogData.chapterID:0;
                    //GameDataMgr.Instance.table.ChangeBookDialogPath(shelfItemInfo.bookid, bookData.ChapterID);

                    //已经收藏的书本，添加进列表
                    m_userData.BookDataList.Add(bookData);

                    if (shelfItemInfo.isfav == 1)
                    {
                        //添加书架上的书本
                        
                        //GameDataMgr.Instance.userData.AddMyBookId(bookData.BookID);
                        GameDataMgr.Instance.userData.saveAddMyBookId(bookData.BookID);
                        //LOG.Info("书架上有的书本id是：" + bookData.BookID);
                    }
                }
            }
            //如果一本书都没有，那么就推荐第一本
            //if(GameDataMgr.Instance.userData.GetMyBookIds().Count == 0)
                //GameDataMgr.Instance.userData.AddMyBookId(1);
        }
    }

    /// <summary>
    /// 记录一些书本的详细信息
    /// </summary>
    public void RecordBookDetailInfo()
    {
        //if (bookDetailInfo != null && bookDetailInfo.data != null && bookDetailInfo.data.userlog != null)
        //{
        //    BookUserLogInfo logInfo = bookDetailInfo.data.userlog;
        //    RecordPlayerName(logInfo.bookid, logInfo.role_name);
        //    RecordPhoneMode(logInfo.bookid, logInfo.phonesceneid, logInfo.phoneroleid);
        //    RecordPlayerRoleId(logInfo.bookid, logInfo.roleid);
        //    RecordPlayerCurClothId(logInfo.bookid, logInfo.clothid);
        //    if (logInfo.npc_detail != null && logInfo.npc_detail.Count > 0)
        //        RecordNpcInfo(logInfo.bookid, logInfo.npc_detail[0]);
        //}
    }

    /// <summary>
    /// 记录当前选择的角色id
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vRoleId"></param>
    public void RecordPlayerRoleId(int vBookId, int vRoleId)
    {
        if (m_userData.BookDataList != null)
        {
            BookData bookData = m_userData.BookDataList.Find((p) => p.BookID == vBookId);
            if (bookData != null)
            {
                bookData.PlayerDetailsID = vRoleId;
                if (bookData.PlayerDetailsID == 0)
                    bookData.PlayerDetailsID = 1;
            }
        }
    }

    /// <summary>
    /// 记录当前的衣服信息
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vClothId"></param>
    public void RecordPlayerCurClothId(int vBookId, int vClothId)
    {
        if (m_userData.BookDataList != null)
        {
            BookData bookData = m_userData.BookDataList.Find((p) => p.BookID == vBookId);
            if (bookData != null)
            {
                bookData.PlayerClothes = vClothId;
                if (bookData.PlayerClothes == 0)
                    bookData.PlayerClothes = 1;
            }
        }
    }

    public void RecordNpcInfo(int vBookId, BookNpcInfo vInfo)
    {
        BookData bookData = m_userData.BookDataList.Find((p) => p.BookID == vBookId);
        if(bookData == null)
        {
            return;
        }
        if (vInfo != null)
        {
            bookData.NpcId = vInfo.npc_id;
            bookData.NpcName = vInfo.npc_name;
            bookData.NpcDetailId = vInfo.npc_sex;
        }else
        {
            bookData.NpcId = 0;
            bookData.NpcName = string.Empty;
            bookData.NpcDetailId = 0;
        }
    }

    /// <summary>
    /// 记录这本书，玩家的名字
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vPlayerName"></param>
    public void RecordPlayerName(int vBookId, string vPlayerName)
    {
        if (m_userData.BookDataList != null)
        {
            BookData bookData = m_userData.BookDataList.Find((p) => p.BookID == vBookId);
            if (bookData != null)
            {
                bookData.PlayerName = vPlayerName;
            }
        }
    }

    /// <summary>
    /// 记录是否在电话场景中
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vInPhoneMode"></param>
    /// <param name="vPhoneRoleId"></param>
    public void RecordPhoneMode(int vBookId, int vInPhoneMode, int vPhoneRoleId)
    {
        if (m_userData.BookDataList != null)
        {
            BookData bookData = m_userData.BookDataList.Find((p) => p.BookID == vBookId);
            if (bookData != null)
            {
                bookData.IsPhoneCallMode = vInPhoneMode == 1;
                bookData.PhoneRoleID = vPhoneRoleId;
            }
        }
    }

    /// <summary>
    /// 检查对话选项是否已经付费
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vDialogId"></param>
    /// <param name="vIndex"></param>
    /// <returns></returns>
    public bool CheckDialogOptionHadCost(int vBookId, int vDialogId, int vIndex)
    {
        if (bookChapterOptionCostList != null && bookChapterOptionCostList.data != null)
        {
            List<BookOptionCostItemInfo> costList = bookChapterOptionCostList.data.costarr;
            if (costList != null)
            {
                int len = costList.Count;
                for (int i = 0; i < len; i++)
                {
                    BookOptionCostItemInfo costItem = costList[i];
                    if (costItem != null && costItem.bookid == vBookId && costItem.dialogid == vDialogId && costItem.option == vIndex)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 检查书本是否已经买了
    /// </summary>
    /// <param name="vBookId"></param>
    /// <returns></returns>
    public bool CheckBookHasBuy(int vBookId)
    {
        if (UserData != null && UserData.bookList != null)
        {
            if (UserData.bookList.IndexOf(vBookId) != -1)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 添加已经付费的选项进入选项记录列表
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vDialogId"></param>
    /// <param name="vIndex"></param>
    /// <returns></returns>
    public void AddDialogOptionHadCost(int vBookId, int vDialogId, int vIndex)
    {
        if (!CheckDialogOptionHadCost(vBookId, vDialogId, vIndex))
        {
            if (bookChapterOptionCostList == null)
                bookChapterOptionCostList = new HttpInfoReturn<BookOptionCostCont<BookOptionCostItemInfo>>();

            if (bookChapterOptionCostList.data == null)
                bookChapterOptionCostList.data = new BookOptionCostCont<BookOptionCostItemInfo>();

            if (bookChapterOptionCostList.data.costarr == null)
                bookChapterOptionCostList.data.costarr = new List<BookOptionCostItemInfo>();

            BookOptionCostItemInfo costItem = new BookOptionCostItemInfo();
            costItem.bookid = vBookId;
            costItem.dialogid = vDialogId;
            costItem.option = vIndex;

            bookChapterOptionCostList.data.costarr.Add(costItem);
        }
    }

    /// <summary>
    /// 判断当前这个衣服是否，已经消费过
    /// </summary>
    /// <param name="vBooId"></param>
    /// <param name="vClothId"></param>
    /// <returns></returns>
    public bool CheckClothHadCost(int vBookId, int vClothId)
    {
        //if (bookDetailInfo != null && bookDetailInfo.data != null && bookDetailInfo.data.clothearr != null)
        //{
        //    List<BookBuyClothItemInfo> clothList = bookDetailInfo.data.clothearr;
        //    int len = clothList.Count;
        //    for (int i = 0; i < len; i++)
        //    {
        //        BookBuyClothItemInfo clothItemInfo = clothList[i];
        //        if (clothItemInfo != null && clothItemInfo.bookid == vBookId && clothItemInfo.link_id == vClothId)
        //        {
        //            return true;
        //        }
        //    }
        //}
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vClothId"></param>
    /// <returns></returns>
    public void AddClothAfterPay(int vBookId, int vClothId)
    {
        //if (bookDetailInfo == null)
        //{
        //    bookDetailInfo = new HttpInfoReturn<BookDetailInfo>();
        //}
        //if (bookDetailInfo.data == null)
        //{
        //    bookDetailInfo.data = new BookDetailInfo();
        //}
        //if (bookDetailInfo.data.clothearr == null)
        //{
        //    bookDetailInfo.data.clothearr = new List<BookBuyClothItemInfo>();
        //}
        //if (!CheckClothHadCost(vBookId, vClothId))
        //{
        //    BookBuyClothItemInfo clothItemInfo = new BookBuyClothItemInfo();
        //    clothItemInfo.bookid = vBookId;
        //    clothItemInfo.link_id = vClothId;
        //    bookDetailInfo.data.clothearr.Add(clothItemInfo);
        //}
    }

    #region 记录那些衣服选项被消费过了
    
    private List<int> HadClothSt;
    public void SaveClothHadBuy(string vSelList)
    {
        if (string.IsNullOrEmpty(vSelList)) return;
    
        if(HadClothSt==null)
           HadClothSt = new List<int>();
    
        string[] strArr = vSelList.Split(',');
        for (int i = 0; i < strArr.Length; i++)
        {
            int hadBuyClothNumber = int.Parse(strArr[i]);
            if (!HadClothSt.Contains(hadBuyClothNumber))
            {
                HadClothSt.Add(int.Parse(strArr[i]));
            }
           
        }
    }
    public void ClothHadBuyClean()
    {
        if (HadClothSt!=null)
        {
            HadClothSt.Clear();
        }
    }
    
    public bool GetClothHadBuy(int ClothID)
    {
        if (HadClothSt !=null&& HadClothSt.Contains(ClothID))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
    #region 记录那些衣服选项被消费过了,新字段Outfit

    private List<int> HadOutfitSt;
    public void SaveOutfitHadBuy(string vSelList)
    {
        if (string.IsNullOrEmpty(vSelList)) return;

        if(HadOutfitSt==null)
            HadOutfitSt = new List<int>();

        string[] strArr = vSelList.Split(',');
        for (int i = 0; i < strArr.Length; i++)
        {
            int hadBuyHairNumber = int.Parse(strArr[i]);
            if (!HadOutfitSt.Contains(hadBuyHairNumber))
            {
                HadOutfitSt.Add(int.Parse(strArr[i]));
            }
           
        }
    }
    public void OutfitHadBuyClean()
    {
        if (HadOutfitSt!=null)
        {
            HadOutfitSt.Clear();
        }
    }

    public bool GetOutfitHadBuy(int OutfitID)
    {
        if (HadOutfitSt !=null&& HadOutfitSt.Contains(OutfitID))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
    #region 记录那些发型选项被消费过了
    
    private List<int> HadHairSt;
    public void SaveHairHadBuy(string vSelList)
    {
        if (string.IsNullOrEmpty(vSelList)) return;
    
        if(HadHairSt==null)
            HadHairSt = new List<int>();
    
        string[] strArr = vSelList.Split(',');
        for (int i = 0; i < strArr.Length; i++)
        {
            int hadBuyHairNumber = int.Parse(strArr[i]);
            if (!HadHairSt.Contains(hadBuyHairNumber))
            {
                HadHairSt.Add(int.Parse(strArr[i]));
            }
           
        }
    }
    public void HairHadBuyClean()
    {
        if (HadHairSt!=null)
        {
            HadHairSt.Clear();
        }
    }
    
    public bool GetHairHadBuy(int HairID)
    {
        if (HadHairSt !=null&& HadHairSt.Contains(HairID))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
    #region 记录那些皮肤选项被消费过了
    
    private List<int> HadCharacterStC;
    public void SaveCharacterHadBuy(string vSelList)
    {
        if (string.IsNullOrEmpty(vSelList)) return;
    
        if(HadCharacterStC==null)
            HadCharacterStC = new List<int>();
    
        string[] strArr = vSelList.Split(',');
        for (int i = 0; i < strArr.Length; i++)
        {
            int hadBuyCharacterNumber = int.Parse(strArr[i]);
            if (!HadCharacterStC.Contains(hadBuyCharacterNumber))
            {
                HadCharacterStC.Add(int.Parse(strArr[i]));
            }
           
        }
    }
    public void CharacterHadBuyClean()
    {
        if (HadCharacterStC!=null)
        {
            HadCharacterStC.Clear();
        }
    }
    
    public bool GetCharacterHadBuy(int CharacterID)
    {
        if (HadCharacterStC !=null&& HadCharacterStC.Contains(CharacterID))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
    #region 记录这个章节哪些选项已经购买过了

    private Dictionary<int,List<int>> hadBuySelection = new Dictionary<int, List<int>>();

    public void SaveCharpterSelectHadBuy(string vSelList)
    {
        if (string.IsNullOrEmpty(vSelList)) return;
        //  vSelList="10-1,10-2,11-1,12-1"
        string[] strArr = vSelList.Split(',');

        List<string> StrList = new List<string>();    
        List<int> DialogIdAll = new List<int>();
        hadBuySelection.Clear();

        for (int i=0;i< strArr.Length;i++)
        {
            StrList.Add(strArr[i]);
        }
        //  StrList="10-1 10-2 11-1 12-1"

        //这里获得所以的不重复 DialogId
        for (int i = 0; i < StrList.Count; i++)
        {
            string[] StrListArr = StrList[i].Split('-');
            int vDialogId = int.Parse(StrListArr[0]);
            if (!DialogIdAll.Contains(vDialogId))
            {
                DialogIdAll.Add(vDialogId);
            }
        }

        for (int i = 0; i < DialogIdAll.Count; i++)
        {
            List<int> SelectIdList = new List<int>();

            for (int j = 0; j < StrList.Count; j++)
            {
                string[] StrListArr = StrList[j].Split('-');
                int vDialogId = int.Parse(StrListArr[0]);
                if (vDialogId== DialogIdAll[i])
                {
                    //这里得到相同 DialogId  的选项号  （例如： 10-1  10-2 10-3）
                    int vSelectId = int.Parse(StrListArr[1]);
                    SelectIdList.Add(vSelectId); //存储好选项号 1 2 3 等
                }

                if (j == StrList.Count-1)
                {
                    //保存同一个DialogID 的所以 选项
                    hadBuySelection.Add(DialogIdAll[i], SelectIdList);
                }
            }
        }   
    }

    /// <summary>
    /// 得到这个DialogId里面已经购买过的选项
    /// </summary>
    /// <param name="DialogId"></param>
    /// <returns></returns>
    public List<int> GetHadBuySelectId(int DialogId)
    {
      
        if (hadBuySelection == null) return null;  
        if (hadBuySelection.ContainsKey(DialogId))
        {                   
            return hadBuySelection[DialogId];
        }else
        {           
            return null;
        }
    }

    #endregion


    private Dictionary<int, Dictionary<int,int>> mBookOptionSelectMap = new Dictionary<int, Dictionary<int,int>>();

    /// <summary>
    /// 记录书本的选项
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vSelList"></param>
    public void RecordBookOptionSelect(int vBookId, string[] vSelList)
    {
        if (vSelList == null || vSelList.Length == 0) return;
        mBookOptionSelectMap[vBookId] = GetOptionMapByList(vSelList);
    }

    private List<int> hadBuyselection = null;
    private Dictionary<int ,int> GetOptionMapByList(string[] vSelList)
    {
        if (vSelList == null || vSelList.Length == 0)
            return null;

        Dictionary<int, int> tempBookOptList = new Dictionary<int, int>();

        int len = vSelList.Length;
        for (int i = 0; i < len; i++)
        {
            string[] strArr = vSelList[i].Split('-');
            if (strArr != null && strArr.Length > 1)
            {
                int vDialogId = int.Parse(strArr[0]);
                int vSelectId = int.Parse(strArr[1]);
                if (tempBookOptList.ContainsKey(vDialogId))
                    tempBookOptList[vDialogId] = vSelectId;
                else
                    tempBookOptList.Add(vDialogId, vSelectId);
            }
        }

        return tempBookOptList;
    }

    /// <summary>
    /// 记录书本的选项
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vDialogId"></param>
    /// <param name="vSelectId"></param>
    public void RecordBookOptionSelect(int vBookId, int vDialogId, int vSelectId)
    {
        LOG.Error("--选项记录---vBookId--->" + vBookId + "---vDialogId--->" + vDialogId + "---vSelectId--->" + vSelectId);

        Dictionary<int, int> tempBookOptList;
        if (!mBookOptionSelectMap.ContainsKey(vBookId))
        {
            tempBookOptList = new Dictionary<int, int>();
            mBookOptionSelectMap.Add(vBookId, tempBookOptList);
        }
        else
        {
            tempBookOptList = mBookOptionSelectMap[vBookId];
        }

        if (tempBookOptList.ContainsKey(vDialogId))
            tempBookOptList[vDialogId] = vSelectId;
        else
            tempBookOptList.Add(vDialogId, vSelectId);
    }

    /// <summary>
    /// 获取书本指定对话ID，之前做过的选项
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vDialogId"></param>
    /// <returns></returns>
    public int GetBookOptionSelectIndex(int vBookId,int vDialogId)
    {
        if (mBookOptionSelectMap == null) return -1;
        if (!mBookOptionSelectMap.ContainsKey(vBookId)) return -1;
        Dictionary<int, int> tempBookOptList = mBookOptionSelectMap[vBookId];
        if (tempBookOptList == null) return -1;
        if (!tempBookOptList.ContainsKey(vDialogId)) return -1;
        int result = tempBookOptList[vDialogId];
        return result;
    }

    private Dictionary<int, Dictionary<int,int>> bookPropertyDic = new Dictionary<int, Dictionary<int, int>>();
    
    /// <summary>
    /// 设置书本的属性值
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vBookList"></param>
    public void SetBookPropertyValue(int vBookId,List<BookPropertyItemInfo> vBookList)
    {
        Dictionary<int, int> tempBookPropertyMap;
        if (!bookPropertyDic.ContainsKey(vBookId))
        {
            tempBookPropertyMap = new Dictionary<int, int>();
            bookPropertyDic.Add(vBookId, tempBookPropertyMap);
        }
        else
            tempBookPropertyMap = bookPropertyDic[vBookId];

        tempBookPropertyMap.Clear();

        if(vBookList != null)
        {
            int len = vBookList.Count;
            for(int i = 0;i<len;i++)
            {
                BookPropertyItemInfo pItemInfo = vBookList[i];
                if(pItemInfo != null)
                {
                    if (!tempBookPropertyMap.ContainsKey(pItemInfo.stamp_key))
                        tempBookPropertyMap.Add(pItemInfo.stamp_key, pItemInfo.stamp_val);
                    else
                        tempBookPropertyMap[pItemInfo.stamp_key] = pItemInfo.stamp_val;
                }
            }
        }
    }

    /// <summary>
    /// 设置书本的属性值
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vKey"></param>
    /// <param name="vValue"></param>
    public void SetBookPropertyValue(int vBookId, int vKey, int vValue)
    {
        LOG.Error("--设置属性-----vBookId--->" + vBookId + "---vDialogId--->" + vKey + "---vSelectId--->" + vValue);

        Dictionary<int, int> tempBookPropertyMap;
        if (!bookPropertyDic.ContainsKey(vBookId))
        {
            tempBookPropertyMap = new Dictionary<int, int>();
            bookPropertyDic.Add(vBookId, tempBookPropertyMap);
        }
        else
            tempBookPropertyMap = bookPropertyDic[vBookId];

        if (!tempBookPropertyMap.ContainsKey(vKey))
            tempBookPropertyMap.Add(vKey, vValue);
        else
        {
            int curValue = tempBookPropertyMap[vKey] + vValue;
            tempBookPropertyMap[vKey] = curValue;
        }
    }

    //获取当前书本某属性值的累积数值
    public int GetPropertyValueByType(int vBookId ,int vTpye)
    {
        if(bookPropertyDic != null && bookPropertyDic.ContainsKey(vBookId))
        {
            Dictionary<int, int> tempBookPropertyMap = bookPropertyDic[vBookId];
            if (tempBookPropertyMap.ContainsKey(vTpye))
                return tempBookPropertyMap[vTpye];
        }
        return 0;
    }


    public string CheckBannedWords(string vStr)
    {
        string result = vStr.ToLower();
        List<t_Banned_Words> bannedList = GameDataMgr.Instance.table.GetBannedWordsList();
        List<BannedWordsItem> recordList = new List<BannedWordsItem>();
        if(bannedList != null)
        {
            int len = bannedList.Count;
            
            for( int i = 0;i<len ; i++)
            {
                string tempWord = bannedList[i].BannedWord;
                result = RecursionReplaceWord(result, tempWord);
            }
        }
        return result;
    }

    /// <summary>
    /// 检查性格属性是否解锁
    /// </summary>
    /// <returns></returns>
    public bool CheckTraitIsUnlock()
    {
        if (UserDataManager.Instance.profileData != null && UserDataManager.Instance.profileData.data != null 
            && UserDataManager.Instance.profileData.data.info != null && UserDataManager.Instance.profileData.data.select != null)
        {
            return UserDataManager.Instance.profileData.data.info.option_num >= UserDataManager.Instance.profileData.data.select.Select1;
        }
        return false;
    }

    /// <summary>
    /// 判断功能是否开放
    /// </summary>
    /// <param name="funId"> 1 是任务 2是猫的入口按钮 3 猫的收养功能是否开放  </param>
    /// <returns></returns>
    public bool CheckGameFunIsOpen(int funId)
    {
        if(UserDataManager.Instance.gameFunStateList != null && UserDataManager.Instance.gameFunStateList.data != null &&
            UserDataManager.Instance.gameFunStateList.data.disjunctor != null)
        {
            int len = UserDataManager.Instance.gameFunStateList.data.disjunctor.Count;
            for(int i = 0;i<len;i++)
            {
                GameFunStateInfo itemInfo = UserDataManager.Instance.gameFunStateList.data.disjunctor[i];
                if (itemInfo != null && itemInfo.id == funId && itemInfo.state == 1)
                    return true;
            }
        }

        return false;
    }

    private string RecursionReplaceWord(string vStr,string vReplace)
    {
        int index = vStr.IndexOf(vReplace);
        string result = vStr;
        if(index != -1)
        {
            result = result.Remove(index, vReplace.Length);
            result = result.Insert(index, GetStarsByLen(vReplace.Length));
            result = RecursionReplaceWord(result, vReplace);
        }
        return result;
    }

    Dictionary<int, string> mStarMap = new Dictionary<int, string>();
    private string GetStarsByLen(int len)
    {
        if(!mStarMap.ContainsKey(len))
        {
            string tempStr = "";
            for(int i = 0 ;i<len;i++)
            {
                tempStr += "*";
            }
            mStarMap.Add(len, tempStr);
        }
        return mStarMap[len];
    }

    /// <summary>
    /// 更新用户留言信息
    /// </summary>
    /// <param name="vItemInfo"></param>
    public void UpdateCommentList(ChapterCommentItemInfo vItemInfo)
    {
        if(chapterCommentList != null &&  chapterCommentList.data != null && chapterCommentList.data.msgarr != null)
        {
            List<ChapterCommentItemInfo> tempList = chapterCommentList.data.msgarr;
            int len = tempList.Count;
            for(int i = 0;i<len;i++)
            {
                ChapterCommentItemInfo tempItemInfo = tempList[i];
                if(tempItemInfo != null && tempItemInfo.discussid == vItemInfo.discussid)
                {
                    tempList[i] = vItemInfo;
                    break;
                }
            }
        }
    }


    public void AddBroadcast(string vMsg)
    {
        BroadcastQueue.Enqueue(vMsg);
    }

    public void AddGMBroadcast(string vMsg)
    {
        GMBroadcastQueue.Enqueue(vMsg);
    }

    public void AddLoopBroadcast(LoopBroadcastItemInfo vMsgItem)
    {
        LoopBroadcastList.Add(vMsgItem);
    }


    public void OnReceiveBroadcast(string vInfo,string vMsg)
    {
        //vInfo = vInfo.Replace("'", "\"");
        Dictionary<string,string> resultDic = JsonHelper.JsonToObject<Dictionary<string,string>>(vInfo);
        int type = 0;
        string resultStr = String.Empty;
        int times = 1;
        if (resultDic != null)
        {
            if(resultDic.ContainsKey("type"))
                type =int.Parse(resultDic["type"]);
            
            if(resultDic.ContainsKey("times"))
                times = int.Parse(resultDic["times"]);
            
            if (resultDic.ContainsKey("color"))
            {
                resultStr = "<color='#"+resultDic["color"]+"'>"+vMsg+"</color>";
            }
            else
            {
                resultStr = vMsg;
            }
        }

        if (type == 1)
        {
            for (int i = 0; i < times; i++)
            {
                UserDataManager.Instance.AddGMBroadcast(resultStr);
            }
        }
        else
        {
            UserDataManager.Instance.AddBroadcast(resultStr);
        }
        UIAlertMgr.Instance.BroadcastShow();

        #region  hide
// Dictionary<string, object> recMsg = JsonHelper.JsonToObject<Dictionary<string, object>>(vMsg);
         //    if(recMsg.ContainsKey("extras"))
         //    {
         //        Dictionary<string, object> typeDic = JsonHelper.JsonToObject<Dictionary<string, object>>(recMsg["extras"].ToString());
         //        if (typeDic.ContainsKey("type"))
         //        {
         //            if (int.Parse(typeDic["type"].ToString()) == 1)
         //            {
         //                //android
         //                if (recMsg.ContainsKey("content"))
         //                {
         //                    AddBroadcast(recMsg["message"].ToString());
         //                    UIAlertMgr.Instance.BroadcastShow();
         //                }
         //            }
         //            else if (int.Parse(typeDic["type"].ToString()) == 2)
         //            {
         //                //android
         //                if (recMsg.ContainsKey("message"))
         //                {
         //                    LoopBroadcastItemInfo loopItemInfo = new LoopBroadcastItemInfo();
         //                    loopItemInfo.id = UserDataManager.Instance.GetBroadcastId();
         //                    loopItemInfo.msg = recMsg["message"].ToString();
         //                    if (typeDic.ContainsKey("begin"))
         //                        loopItemInfo.begin = int.Parse(typeDic["begin"].ToString());
         //                    if (typeDic.ContainsKey("end"))
         //                        loopItemInfo.end = int.Parse(typeDic["end"].ToString());
         //                    if (typeDic.ContainsKey("gap"))
         //                        loopItemInfo.gap = int.Parse(typeDic["gap"].ToString()) * 60;
         //
         //                    if (loopItemInfo.begin == 0 || loopItemInfo.end == 0 || loopItemInfo.gap == 0)
         //                    {
         //                        UserDataManager.Instance.AddBroadcast(recMsg["message"].ToString());
         //                        UIAlertMgr.Instance.BroadcastShow();
         //                    }
         //                    else
         //                    {
         //                        UserDataManager.Instance.AddLoopBroadcast(loopItemInfo);
         //                    }
         //                }
         //            }
         //        }
         //    }
        

        #endregion
         
    }


    #region LocalData
  
    public int AlertFormClosTip
    {
        get
        {          
            return PlayerPrefs.GetInt("AlertFormClosTip");
        }
        set
        {
            
            PlayerPrefs.SetInt("AlertFormClosTip", value);
        }
    }
  
    public int CatPosReplace
    {
        get
        {          
            return PlayerPrefs.GetInt("CatPosReplace");
        }
        set
        {
            PlayerPrefs.SetInt("CatPosReplace", value);
        }
    }

    /// <summary>
    /// 商店购买装饰物后，弹出的放置提示
    /// </summary>
    //public int CatShopBuyDesPlaceTips
    //{
    //    get
    //    {
    //        return PlayerPrefs.GetInt("CatShopBuyDesPlaceTips");
    //    }
    //    set
    //    {
    //        PlayerPrefs.SetInt("CatShopBuyDesPlaceTips", value);
    //    }
    //}


    /// <summary>
    /// 缓存本地的版本号
    /// </summary>
    public void SaveVersion()
    {
        PlayerPrefs.SetString("SecretsVersionId", Version.ToString());
    }

    /// <summary>
    /// 获取本地版本号
    /// </summary>
    public void GetVersion()
    {
        string tempVersionId = PlayerPrefs.GetString("SecretsVersionId");
        if (!string.IsNullOrEmpty(tempVersionId))
            Version = int.Parse(tempVersionId);
        else
            Version = 0;
    }

    //分析用户信息
    public void AnalyzeUserIp()
    {
        if(userInfo == null || userInfo.data.ipinfo == null)
        {
#if UNITY_EDITOR
            UserCountry = "CN";
#else
            UserCountry = "US";
#endif
        }
        else
            UserCountry = userInfo.data.ipinfo.country_code;

        GameHttpNet.Instance.LANG = UserCountry;
    }
    

    /// 1：表示英文国家  2：表示中文国家  3：西班牙语
    public int CountryId()
    {
        AnalyzeUserIp();
        switch (UserDataManager.Instance.UserCountry)
        {
            case "CN":  //中国
            case "TW":  //台湾
            case "HK":  //香港
            case "MO":  //澳门
            case "MY":  //马来西亚
            case "SG":  //新加坡
                return 2;
            //case "PT":  //葡萄牙
            //case "ES":  //西班牙
            //    return 3;
        }

        return 1;
    }

    //1：表示英文国家  2：表示中文国家  3：西班牙
    //根据书本的国家和区域，来判断是否对用户开放
    public bool BookOpenState(int bookAvailability)
    {
        int countryId = CountryId();
        if (countryId == 1 && bookAvailability != countryId) //英文国家，不开放其他语言的书本
            return false;

        return true;
    }

    
    

    

private string[] BookTypeNameArr;
public string GetBookTypeName(int vIndex)
{
    if(BookTypeNameArr == null)
        BookTypeNameArr = new string[12] {"Romance","LGBT","Action","Youth","Adventure","Drama","Comedy","Horror","18+","Fantasy","Suspense","Others"};
    if (vIndex < BookTypeNameArr.Length)
        return BookTypeNameArr[vIndex];
    return "";
}

    /// <summary>
    /// 保存第三方登陆信息
    ///<param name="curLoginChannel">当前登陆的渠道 1:facebook 2:google</param>
    /// </summary>
    public void SaveLoginInfo()
    {
        string localLoginInfo = string.Empty;
        if (mLoginInfo != null)
        {
            LoginDataInfo fb = mLoginInfo.FaceBookLoginInfo;
            if(fb != null)
            {
                localLoginInfo = fb.UserId + "_" + fb.Token + "_" + fb.Email + "_" + fb.UserName + "_" + fb.UserImageUrl;
            }
            localLoginInfo += "^";
            LoginDataInfo gl = mLoginInfo.GoogleLoginInfo;
            if (gl != null)
                localLoginInfo += gl.UserId + "_" + gl.Token + "_" + gl.Email + "_" + gl.UserName + "_" + gl.UserImageUrl;

            localLoginInfo += "^" + mLoginInfo.LastLoginChannel + "^" + GameHttpNet.Instance.TOKEN ;

            if(hwLoginInfo != null)
            {
                localLoginInfo += "^";
                localLoginInfo += hwLoginInfo.playerId + "_" + hwLoginInfo.playerLevel + "_" + hwLoginInfo.displayName + "_" + hwLoginInfo.gameAuthSign + "_" + hwLoginInfo.ts;
            }


            PlayerPrefs.SetString("SecretsThirdLoginInfo", localLoginInfo);
            PlayerPrefs.Save();
        }
    }


    /// <summary>
    /// 登出时，清理第三方的记录
    /// </summary>
    public void LogOutDelLocalInfo()
    {
        PlayerPrefs.SetString("SecretsThirdLoginInfo", string.Empty);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 获得本地的第三方登陆记录
    /// </summary>
    public void GetLoginInfoByLocal()
    {
        if (mLoginInfo == null)
            mLoginInfo = new ThirdLoginData();

        string localLoginInfo = PlayerPrefs.GetString("SecretsThirdLoginInfo");

        if(!string.IsNullOrEmpty(localLoginInfo))
        {
            string[] infoList = localLoginInfo.Split('^');

            if(infoList.Length > 0)
            {
                if (infoList.Length > 4)
                    hwLoginInfo = Str2HwUserInfo(infoList[4]);


                if (infoList.Length > 3)
                    GameHttpNet.Instance.TOKEN = infoList[3];

                if (infoList.Length > 2)
                    mLoginInfo.LastLoginChannel = int.Parse(infoList[2]);
                else
                    mLoginInfo.LastLoginChannel = 0;


                mLoginInfo.FaceBookLoginInfo = Str2LoginDadta(infoList[0]);

                if(infoList.Length>1)
                    mLoginInfo.GoogleLoginInfo = Str2LoginDadta(infoList[1]);
            }
        }
    }
    
    private LoginDataInfo Str2LoginDadta(string value)
    {
        LoginDataInfo info = null;
        string[] glInfoList = value.Split('_');
        if (glInfoList.Length > 4)
        {
            info  = new LoginDataInfo();
            info.UserId = glInfoList[0];
            info.Token = glInfoList[1];
            info.Email = glInfoList[2];
            info.UserName = glInfoList[3];
            info.UserImageUrl = glInfoList[4];
        }
        return info;
    }

    private HwUserInfo Str2HwUserInfo(string value)
    {
        HwUserInfo info = null;
        string[] glInfoList = value.Split('_');
        if (glInfoList.Length > 4)
        {
            info = new HwUserInfo();
            info.playerId = glInfoList[0];
            info.playerLevel = int.Parse(glInfoList[1]);
            info.displayName = glInfoList[2];
            info.gameAuthSign = glInfoList[3];
            info.ts = glInfoList[4];
        }
        return info;
    }

    public void SaveUserDataToLocal()
    {
        SaveInfoByPlayPre();
    }

    public void ReadBookData()
    {
        FileStream fileStream = null;
        try
        {
            fileStream = File.OpenRead(path + "/UserData");
            m_userData = Serializer.Deserialize<UserData>(fileStream);
        }
        catch (System.Exception e)
        {
            LOG.Error(e);
            if (IsExist(path + "/UserData"))
            {
                if (fileStream != null) fileStream.Close();
                File.Delete((path + "/UserData"));
            }
        }
        finally
        {
            if (fileStream != null) fileStream.Close();
        }
        //m_userData = ES3.Load<UserData>("UserData");
        LOG.Info(m_userData.ToString());
    }



    private bool IsExist(string path)
    {
        return File.Exists(path);
    }

    /// <summary>
    /// 是否为付费玩家
    /// </summary>
    /// <returns></returns>
    public bool IsPayUser()
    {
        if (userInfo != null && userInfo.data != null && userInfo.data.userinfo != null)
        {
            return userInfo.data.userinfo.pay_total > 0;
        }
        return false;
    }

    public void SetPayUser()
    {
        userInfo.data.userinfo.pay_total = 1;
    }

    /// <summary>
    /// 充值完后，设置付费状态
    /// </summary>
    /// <param name="value"></param>
    public void SetIsPayState(int value)
    {
        if (userInfo != null && userInfo.data != null && userInfo.data.userinfo != null)
        {
            userInfo.data.userinfo.pay_total = value;
        }
    }

    public void ClearCache()
    {
        PlayerPrefs.SetString("SecretsBaseInfo", string.Empty);
        PlayerPrefs.SetString("SecretsBookInfo", string.Empty);
    }

    private void SaveInfoByPlayPre()
    {
        if(m_userData != null)
        {
            string baseInfoStr = string.Empty;
            baseInfoStr =  m_userData.BgMusicIsOn + "_" + m_userData.TonesIsOn + "_" + m_userData.AutoSpeed;
            PlayerPrefs.SetString("SecretsBaseInfo", baseInfoStr);
        }
    }

    private void ReadInfoByPlayPre()
    {
        if (m_userData == null)
            m_userData = new UserData();

        string baseInfoStr = PlayerPrefs.GetString("SecretsBaseInfo");

        if(!string.IsNullOrEmpty(baseInfoStr))
        {
            string[] infoList = baseInfoStr.Split('_');
            int baseLen = infoList.Length;
            if (baseLen > 0)
                m_userData.BgMusicIsOn = int.Parse(infoList[0]);
            if (baseLen > 1)
                m_userData.TonesIsOn = int.Parse(infoList[1]);
            if (baseLen > 2)
                m_userData.AutoSpeed = float.Parse(infoList[2]);
        }
    }

    private List<int> ConvertStr2List(string value)
    {
        List<int> result = new List<int>();
        if(!string.IsNullOrEmpty(value))
        {
            string[] list = value.Split(',');
            int len = list.Length;
            for (int i = 0; i < len; i++)
            {
                int temp = int.Parse(list[i]);
                if (temp > 0)
                    result.Add(temp);
            }
        }
        return result;
    }

    private string ConvertList2Str(List<int> vList)
    {
        string result = string.Empty;
        if(vList != null)
        {
            int len = vList.Count;
            for(int i =0;i<len;i++)
            {
                if (i == 0)
                    result += vList[i];
                else
                    result += "," + vList[i];
            }
        }
        return result;
    }

    private int ConvertStr2Int(string value)
    {
        if(string.IsNullOrEmpty(value))
        {
            return 0;
        }
        return int.Parse(value);
    }

    private string mCoverFlag = "SecretsCoverPaths";
    public void AddCoverImage(CAsset asset)
    {
#if false //保存看过的
        var json = PlayerPrefs.GetString(mCoverFlag, "{}");
        var dict = JsonHelper.JsonToObject<Dictionary<string, uint>>(json);
#else
        var dict = new Dictionary<string, uint>();
#endif
        //if (ABMgr.Instance.isUseAssetBundle)
        //{
        //    switch (AbUtility.loadType)
        //    {
        //        case enLoadType.eWebUnity:
        //            dict[asset.abConfigItem.abFilePath] = asset.abConfigItem.crc;
        //            break;
        //        case enLoadType.eFile:
        //        case enLoadType.eWebClient:
        //            dict[ABLoader_WebClient.GetPath(asset.abConfigItem)] = asset.abConfigItem.crc;
        //            break;
        //    }
        //}
        //else
        //{
        //    dict[asset.assetName] = 0;
        //}
        var json = JsonHelper.ObjectToJson(dict);
        PlayerPrefs.SetString(mCoverFlag, json);
    }

    public Dictionary<string, uint> GetLoadedCoverImage()
    {
        var json = PlayerPrefs.GetString(mCoverFlag, "{}");
        var dict = JsonHelper.JsonToObject<Dictionary<string, uint>>(json);
        return dict;
    }

    public void clearLocalBookCoverInfo()
    {
        PlayerPrefs.DeleteKey(mCoverFlag);
        PlayerPrefs.Save();
    }


#endregion

  

#region 保存养猫游戏打开的所有界面顺序
    private ArrayList CatUiFormDic;
    private int UiNumber = 0;
    public void UiNumberInit()
    {
        UiNumber = 0;
    }
    public ArrayList GetCatUiFormDic()
    {
        if (CatUiFormDic == null)
        {
            CatUiFormDic = new ArrayList();
        }
        return CatUiFormDic;
    }

    /// <summary>
    /// 获取界面列表缓存的栈顶
    /// </summary>
    /// <returns></returns>
    public int GetCatFormListTop()
    {
        if (CatUiFormDic == null || CatUiFormDic.Count == 0)
            return -1;
        return (int)CatUiFormDic[CatUiFormDic.Count - 1];
    }


    /// <summary>
    /// 从列表里面移除
    /// </summary>
    /// <param name="vIndex"></param>
    public bool RemoveCatFormByIndex(int vFormId = -1)
    {
        if (CatUiFormDic != null && CatUiFormDic.Count >0)
        {
            int removeIndex = -1;
            if (vFormId == -1)
                removeIndex = CatUiFormDic.Count-1;
            else
                removeIndex = CatUiFormDic.IndexOf(vFormId);

            if (removeIndex != -1)
                CatUiFormDic.RemoveAt(removeIndex);

            return removeIndex != -1;
        }
        return false;
    }

    public void SetCatUiFormDic(int vFormIndex)
    {
        GetCatUiFormDic();
        int index = CatUiFormDic.IndexOf(vFormIndex);
        if (index != -1)
            CatUiFormDic.RemoveAt(index);
        CatUiFormDic.Add(vFormIndex);
    }
#endregion


#region 猫的动作获取与取值

    private Dictionary<int, int> CatAtionInfo;

    public int GetCatAtion(int CatID)
    {
        int Action = 1;
        if (CatAtionInfo.ContainsKey(CatID))
        {
            //Action = CatAtionInfo[CatID];

            CatAtionInfo.TryGetValue(CatID, out Action);
        }

        return Action;
    }

    public void CatAtionInfoSave(int CatID,int CatAction)
    {
        if (CatAtionInfo == null)
        {
            CatAtionInfo = new Dictionary<int, int>();
            CatAtionInfo.Clear();
        }

        if (CatAtionInfo.ContainsKey(CatID))
        {
           
        }
        else
        {
            CatAtionInfo.Add(CatID, CatAction);
           
        }     
    }

    public Dictionary<int, int> ReturnCatAtionInfoDic()
    {
        if (CatAtionInfo == null)
        {
            CatAtionInfo = new Dictionary<int, int>();
        }
        return CatAtionInfo;
    }
#endregion

#region 猫的界算界面数值统计，是否是有变化
    private Dictionary<int, int> OldCatValue;
    private List<int> ChangValueId;

    public List<int> GetChangValueList()
    {
        if (ChangValueId == null)
        {
            ChangValueId = new List<int>();
        }

        return ChangValueId;
    }

    public void SaveCatValue(int CatId,int CatValue)
    {
        if (OldCatValue==null)
            OldCatValue = new Dictionary<int, int>();

        if (ChangValueId==null)
            ChangValueId = new List<int>();

        if (OldCatValue.ContainsKey(CatId))
        {
            int valu = 0;
            OldCatValue.TryGetValue(CatId, out valu);
            if (valu!= CatValue)
            {
                //数据有变
                ChangValueId.Add(CatId);
                OldCatValue[CatId] = CatValue;
            }
        }
        else
        {
            //数据有变
            OldCatValue.Add(CatId, CatValue);
            ChangValueId.Add(CatId);
        }
    }

#endregion

    public void SetAuditStatusInfo(string json)
    {
        this.auditStatusInfo = JsonHelper.JsonToObject<HttpInfoReturn<AuditStatusInfo>>(json);
    }

    public void SetSetGameVersion(string json)
    {
        this.gameVersionInfo = JsonHelper.JsonToObject<HttpInfoReturn<VersionCont<GameVersionInfo>>>(json);
    }


    public void SetRecommandbookList(string json)
    {
        this.RecommandbookList = JsonHelper.JsonToObject<HttpInfoReturn<GetNewRecommandbook>>(json);
    }

    public void SetUserInfo(string json)
    {
        Debug.LogError("----GetUserInfoCallBack---->");
        userInfo = JsonHelper.JsonToObject<HttpInfoReturn<UserInfoCont>>(json);


        UserDataManager.Instance.UserData.UserID = UserDataManager.Instance.userInfo.data.userinfo.uid;
        UserDataManager.Instance.UserData.KeyNum = UserDataManager.Instance.userInfo.data.userinfo.bkey;
        UserDataManager.Instance.UserData.DiamondNum = UserDataManager.Instance.userInfo.data.userinfo.diamond;
        UserDataManager.Instance.UserData.bookList = new List<int>();
        //UserDataManager.Instance.UserData.bookList.AddRange(UserDataManager.Instance.userInfo.data.userinfo.booklist);
        UserDataManager.Instance.UserData.bookNickName = UserDataManager.Instance.userInfo.data.userinfo.nickname;


        GameDataMgr.Instance.SetServerTime(int.Parse(UserDataManager.Instance.userInfo.data.userinfo.current_time));

        if (UserDataManager.Instance.userInfo.data.userinfo.status != 0)
        {
            string msg = string.Empty;
            if (UserDataManager.Instance.userInfo.data.userinfo.status == 1)
            {
                msg = "Your account is being investigated due to unusual gameplay patterns.Please contact customer service for more information.";
            }
            else if (UserDataManager.Instance.userInfo.data.userinfo.status == 2)
            {
                msg = "Your account has been flagged as illegal. You've been banned from the servers in Secrets.Please contact with customer support for more information.";
            }
            if (!string.IsNullOrEmpty(msg))
            {
                UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, msg, AlertType.Sure, (value) =>
                {

#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                });
                return;
            }
        }
    }

    public void SetSelfBookInfo(string json)
    {
        selfBookInfo = JsonHelper.JsonToObject<HttpInfoReturn<SelfBookInfo>>(json);
    }
    
    public void SetSwitchStatus(string json)
    {
        switchStatus = JsonHelper.JsonToObject<HttpInfoReturn<SwitchStatus>>(json);
    }

    public void SetShopList(string json)
    {
        shopList = JsonHelper.JsonToObject<HttpInfoReturn<ShopListCont>>(json);
    }

    public void SetBookSelfInfo(string json)
    {
        bookItemsInfo = JsonHelper.JsonToObject<HttpInfoReturn<BookItemsInfo>>(json);
    }

    public BookItemInfo GetBookItemInfo(int bookId)
    {
        if (bookItemsInfo != null && bookItemsInfo.data != null && bookItemsInfo.data.book_list != null && bookItemsInfo.data.book_list.Count > 0)
        {
            var bookInfo = bookItemsInfo.data.book_list.Find(bookItemInfo => bookItemInfo.id == bookId);
            if (null != bookInfo) return bookInfo;
            LOG.Error($"服务器未配置书本{bookId}的版本号");
            return new BookItemInfo
            {
                id = 0, chaptercount = 0, chapteropen = 0, version = System.Environment.TickCount.ToString()
            };
        }
        GetBookVersionInfo();
        return new BookItemInfo
        {
            id = 0, chaptercount = 0, chapteropen = 0, version = System.Environment.TickCount.ToString()
        };
    }

    public void GetBookVersionInfo()
    {
        GameHttpNet.Instance.GetBookVersion(GetBookVersionCallBack);
    }
    

    private void GetBookVersionCallBack(long responseCode,string result)
    {
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            if (jo.code == 200)
            {
                SetBookSelfInfo(result);
            }
        }
    }

    public string GetBookVersion(int bookId)
    {
        var versionStr = GetBookItemInfo(bookId).version;
 #if ENABLE_DEBUG
         versionStr = $"{versionStr}@{System.Environment.TickCount.ToString()}";
 #endif
        return versionStr;
    }

    public int GetSeeVideoNumOfClothes(int bookID, int mChoicesClothId)
    {
        int mask = BitUtils.GetInt32Mask(bookID, mChoicesClothId);
        int num = PlayerPrefs.GetInt(GameHttpNet.Instance.UUID + "_SeeVideoNumOfClothes_"+ mask, 0);
        return num;
    }
    public void SetSeeVideoNumOfClothes(int bookID, int mChoicesClothId,int num)
    {
        int mask = BitUtils.GetInt32Mask(bookID, mChoicesClothId);
        PlayerPrefs.SetInt(GameHttpNet.Instance.UUID + "_SeeVideoNumOfClothes_" + mask, num);
    }



    public void SetChoicesClothResultInfo(string result)
    {
        choicesClothResultInfo = JsonHelper.JsonToObject<HttpInfoReturn<ChoicesClothResultInfo>>(result);
    }


    public void SetHiddenEggInfo(string result)
    {
        hiddenEggInfo = JsonHelper.JsonToObject<HttpInfoReturn<HiddenEggInfo>>(result);
    }

    public void SetNewUserEggInfo(string result)
    {
        newUserEggInfo = JsonHelper.JsonToObject<HttpInfoReturn<NewUserEggInfo>>(result);
    }

    public void Set_BookDetailInfo(string result)
    {
        this.bookDetailInfo = JsonHelper.JsonToObject<HttpInfoReturn<BookDetailInfo>>(result);
    }

    public void SetSendPlayerProgress(string result)
    {
        SaveStepInfo = JsonHelper.JsonToObject<HttpInfoReturn<SaveStep>>(result);
    }


}

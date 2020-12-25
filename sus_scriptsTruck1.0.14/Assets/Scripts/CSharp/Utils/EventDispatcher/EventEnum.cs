using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventEnum
{
    //------------------------------------------Onyx服务器埋点--------------------------------------------------

    #region 流程统计
    //游戏加载
    public static readonly string GetVersion = "get_version"; //加载界面开始
    public static readonly string GetVersionOk = "get_version_ok"; //加载成功
    public static readonly string LoadConfig = "load_config"; //开始加载配置
    public static readonly string LoadConfigSuccess = "load_config_success"; //加载资源成功
    public static readonly string LoadLua = "load_lua"; //开始加载lua
    public static readonly string LoadLuaSuccess = "load_lua_success"; //加载lua成功
    public static readonly string LoadResourceOk = "load_resouce_ok"; //加载资源成功
    public static readonly string LoadUserInfo = "load_userinfo"; //加载用户信息成功
    public static readonly string EnterGameOnyx = "enter_game"; //加载游戏完成

    //登陆
    public static readonly string Login = "login"; //登录开始
    public static readonly string LoginOk = "login_ok"; //登录成功

    //新手期引导
    public static readonly string NewerStart = "newer_start"; //新手节点步骤
    public static readonly string NewerNextBook = "newer_next_book"; //点击换一批
    public static readonly string NewerOut = "newer_out"; //退出新手推荐
    public static readonly string NewerSelectBook = "newer_select_book"; //选书
    public static readonly string NewerReadBook = "newer_read_book"; //确认书本
    public static readonly string NewerLeaveBook = "newer_leave_book"; //退出故事 todo

    //故事阅读
    public static readonly string SelectBook = "select_book"; //选择故事 //继续阅读模块|推荐模块|我的书库模块|选择故事类别|选择书本
    public static readonly string LoadBook = "load_book"; //加载故事
    public static readonly string ReadBook = "read_book"; //进入阅读
    public static readonly string SelectOption = "select_option"; //对话选项
    public static readonly string McSetName = "mc_set_name"; //主角输入名称
    public static readonly string McSelectCharacter = "mc_select_character"; //主角选择形象
    public static readonly string McSelectHair = "mc_select_hair"; //主角选择头发
    public static readonly string McSelectOutfit = "mc_select_outfit"; //主角选择服装
    public static readonly string NpcSetName = "npc_set_name"; //NPC输入名称
    public static readonly string NpcSelectCharacter = "npc_select_character"; //NPC选择形象
    public static readonly string NpcSelectHair = "npc_select_hair"; //NPC选择头发 todo
    public static readonly string NpcSelectOutfit = "npc_select_outfit"; //NPC选择服装
    public static readonly string LeaveBook = "leave_book"; //故事退出
    #endregion

    #region 操作行为

    //操作行为
    public static readonly string HomePage = "homepage";  //打开主界面
    public static readonly string SearchPage = "search_page";  //打开搜索界面
    public static readonly string SwitchingSort = "switching_sort";  //选择故事排序方式
    public static readonly string SelectCategory = "select_category";  //选择故事题材
    public static readonly string UgcPage = "ugc_page";  //打开创作界面
    public static readonly string UgcSearch = "ugc_search";  //点击搜索
    public static readonly string UgcSelectHotBook = "ugc_select_hot_book"; //点击最受欢迎书本
    public static readonly string UgcMoreHotBook = "ugc_more_hot_book";  //打开最受欢迎more
    public static readonly string UgcSelectNewBook = "ugc_select_new_book ";  //点击最新书本
    public static readonly string UgcMoreNewBook = "ugc_more_new_book";  //打开最新书本more
    public static readonly string UgcWriteBook = "ugc_write_book";  //点击快捷创作
    public static readonly string UgcMyBook = "ugc_mybook";  //点击我的创作
    public static readonly string UgcReadBook = "ugc_read_book";  //点击继续阅读
    public static readonly string ActivityPage = "activity_page";  //打开活动界面
    public static readonly string ActivityReceiveMonthSignItem = "activity_receive_month_sign_item";  //点击月签到领取
    public static readonly string ActivitySign = "activity_sign";  //点击签到
    public static readonly string ActivityReceiveSignItem = "activity_receive_sign_item";  //点击签到领取
    public static readonly string ActivityVideoAd = "activity_video_ad";  //点击看视频获得钻石
    public static readonly string ActivityClickTaskGo = "activity_click_task_go";  //点击任务前往
    public static readonly string ActivityClickTaskReward = "activity_click_task_reward";  //点击任务奖励领取
    public static readonly string UcenterPage = "ucenter_page";  //打开个人中心界面
    public static readonly string ChangeNickName = "change_nickname";  //改名
    public static readonly string BindAccount = "bind_account";  //绑定帐号
    public static readonly string ChangeAccount = "change_account";  //切换帐号
    public static readonly string MailBox = "mail_box";  //邮件
    public static readonly string PakageBox = "pakage_box";  //背包
    public static readonly string MyFavoriteBook = "my_favorite_book";  //我的书本
    public static readonly string SettingMusic = "setting_music";  //音乐
    public static readonly string SettingSound = "setting_sound";  //声音
    public static readonly string FaqList = "faq_list";  //FAQ
    public static readonly string ContactUs = "contact_us";  //联系我们
    public static readonly string PrivacyPolicy = "privacy_policy";  //隐私政策
    public static readonly string mService = "service";  //服务条款

    #endregion

    #region 数据统计

    //数据统计
    public static readonly string UseDiamond = "use_diamond";  //钻石消耗
    public static readonly string UseKey = "use_key";  //钥匙消耗
    public static readonly string GetDiamond = "get_diamond";  //钻石获得
    public static readonly string GetKey = "get_key";  //钥匙获得
    public static readonly string ReadBookCumulativeTime = "read_book_cumulative_time";  //阅读故事在线时长
    public static readonly string PageCumulativeTime = "page_cumulative_time";  //界面在线时长

    public static readonly string AddBookComment = "add_book_comment"; //发表书本评论
    public static readonly string ThumbUpRecord = "thumb_up_record"; //点赞记录
    public static readonly string CancelThumbUp = "cancel_thumb_up"; //取消点赞记录


    public static readonly string ResetChapter = "reset_chapter"; //章节重置
    public static readonly string ResetBook = "reset_book"; //书本重置
    public static readonly string AutoReadBookCumulativeTime = "auto_read_book_cumulative_time"; //自动阅读时长
    public static readonly string AutoReadBookCount = "auto_read_book_count"; //自动阅读书本数量
    public static readonly string ViewBarrage = "view_barrage"; //查看弹幕统计
    public static readonly string AddBarrage = "add_barrage"; //发送弹幕统计

    #endregion

    //------------------------------------------数据埋点--------------------------------------------------
    //游戏启动埋点
    public static readonly string GetVersionStart           = "1_01_##_get_version_start";
    public static readonly string GetVersionResultSucc      = "1_02_##_get_version_result_success";
    public static readonly string GetVersionResultFail      = "1_02_##_get_version_result_fail";
    public static readonly string GoToStoreUpdate           = "1_03_##_get_order_start";
    public static readonly string GetDomainStart            = "1_04_##_get_domain_start";
    public static readonly string GetDomainResultSucc       = "1_05_##_get_domain_result_success";
    public static readonly string GetDomainResultFail       = "1_05_##_get_domain_result_fail";
    public static readonly string LoadConfigStart           = "1_06_##_load_config_start";
    public static readonly string LoadConfigResultSucc      = "1_07_##_load_config_result_success";
    public static readonly string LoadConfigResultFail      = "1_07_##_load_config_result_fail";
    public static readonly string LoadLuaStart              = "1_08_##_load_lua_start";
    public static readonly string LoadLuaResultSucc         = "1_09_##_load_lua_result_success";
    public static readonly string LoadLuaResultFail         = "1_09_##_load_lua_result_fail";
    public static readonly string LoadResStart              = "1_10_##_load_res_start";
    public static readonly string LoadResResultSucc         = "1_11_##_load_res_result_success";
    public static readonly string LoadResResultFail         = "1_11_##_load_res_result_fail";
    public static readonly string GetUserInfoStart          = "1_12_##_get_userinfo_start";
    public static readonly string GetUserInfoResultSucc     = "1_13_##_get_userinfo_result_success";
    public static readonly string GetUserInfoResultFail     = "1_13_##_get_userinfo_result_fail";
    public static readonly string EnterGame                 = "1_14_##_enter_game";
    public static readonly string GetMallStart              = "1_15_##_get_mall_start";
    public static readonly string GetMallResultSucc         = "1_16_##_get_mall_result_success";
    public static readonly string GetMallResultFail         = "1_16_##_get_mall_result_fail";
    public static readonly string GetBookShelfStart         = "1_17_##_get_bookshelf_start";
    public static readonly string GetBookShelfResultSucc    = "1_18_##_get_bookshelf_result_success";
    public static readonly string GetBookShelfResultFail    = "1_18_##_get_bookshelf_result_fail";
    
    
    
    
    //账号登录埋点(游客)
    public static readonly string LoginTouristStart         = "2_##_tourist_login_start";
    public static readonly string LoginTouristResultSucc    = "2_##_tourist_login_success";
    public static readonly string LoginTouristResultFail    = "2_##_tourist_login_fail";

    //账号登录埋点(第三方：Facebook，Google)
    public static readonly string Login3rdStart             = "3_##_3rd_login_start";
    public static readonly string Login3rdResultSucc        = "3_##_3rd_login_success";
    public static readonly string Login3rdResultFail        = "3_##_3rd_login_fail";

    //支付埋点
    public static readonly string GetOrderStart             = "4_01_##_get_order_start";
    public static readonly string GetOrderResultSucc        = "4_02_##_get_order_success";
    public static readonly string GetOrderResultFail        = "4_02_##_get_order_fail";
    public static readonly string PlatformPayStart          = "4_03_##_pay_start";
    public static readonly string PlatformPayResultSucc     = "4_04_##_pay_success";
    public static readonly string PlatformPayResultFail     = "4_04_##_pay_fail";
    public static readonly string PlatformPayCancel         = "4_04_##_cancel_order";
    public static readonly string SubmitOrderStart          = "4_05_##_submit_order";
    public static readonly string SubmitOrderResultSucc     = "4_06_##_submit_order_success";
    public static readonly string SubmitOrderResultFail     = "4_06_##_submit_order_fail";
    public static readonly string SubmitRecoverStart        = "4_07_##_submit_recover_order";
    public static readonly string SubmitRecoverResultSucc   = "4_08_##_recover_order_success";
    public static readonly string SubmitRecoverResultFail   = "4_08_##_recover_order_fail";




    //--------------------------------------------------------------------------------------------

    public static readonly string OnKeyNumChange = "OnKeyNumChange";
    public static readonly string OnDiamondNumChange = "OnDiamondNumChange";
    public static readonly string OnTicketNumChange = "OnTicketNumChange";
    public static readonly string DialogDisplaySystem_PlayerMakeChoice = "DialogDisplaySystem_PlayerMakeChoice";
    public static readonly string BookJoinToShelfEvent = "BookJoinToShelfEvent";

    public static readonly string FaceBookLoginSucc = "FaceBookLoginSucc";
    public static readonly string GoogleLoginSucc = "GoogleLoginSucc";
    public static readonly string HideWebView = "HideWebView";
    public static readonly string PaySuccess = "PaySuccess";
    public static readonly string ThirdPartyLoginSucc = "ThirdPartyLoginSucc";
    public static readonly string SwitchLoginFailed = "SwitchLoginFailed";
    public static readonly string DeviceLoginSucc = "DeviceLoginSucc";
    public static readonly string GetRewardShow = "GetRewardShow";
    public static readonly string HiddenEggRewardShow = "HiddenEggRewardShow";
    public static readonly string GetDayLoginRewardSucc = "GetDayLoginRewardSucc";
    public static readonly string BookProgressUpdate = "BookProgressUpdate";
    public static readonly string GooglePayResultInfo = "GooglePayResultInfo";
    public static readonly string ChangeBookReadingBgEnable = "ChangeBookReadingBgEnable";
    public static readonly string ResidentMoneyInfo = "ResidentMoneyInfo";

    public static readonly string CheckIsCanCountDown = "CheckIsCanCountDown";
    public static readonly string GetEmailShowHint = "GetEmailShowHint";
    public static readonly string EmailAwartShowClose = "EmailAwartShowClose";
    public static readonly string CloseEamailNotice = "CloseEamailNotice";
    public static readonly string ShowMoneyTicketForm = "ShowMoneyTicketForm";
    public static readonly string ClosTimeReturn = "ClosTimeReturn";

    public static readonly string HuaweiLoginInfo = "HuaweiLoginInfo";

    public static readonly string ShowNewIcon = "ShowNewIcon";
    public static readonly string BookRecommendReturn = "BookRecommendReturn";
    public static readonly string nextChapterToShow = "nextChapterToShow";
    public static readonly string GetuserpaymallidStatChack = "GetuserpaymallidStatChack";
    public static readonly string ChargeTipsMoneyX2Value = "ChargeTipsMoneyX2Value";
    public static readonly string MainFormMove = "MainFormMove";
    public static readonly string NavigationClose = "NavigationClose";
    public static readonly string NoticeClose = "NoticeClose";
    public static readonly string VIPDayUp = "VIPDayUp";
    public static readonly string DayActivtyItemReviec = "DayActivtyItemReviec";
    public static readonly string MyBookDestry = "MyBookDestry";
    public static readonly string Achieveallmsgprice = "Achieveallmsgprice";


    public static readonly string AddOpenFormType = "AddOpenFormType";
    public static readonly string AddBarrageOnclike = "AddBarrageOnclike";
    public static readonly string CloseUiFormDist = "CloseUiFormDist";
    public static readonly string CacularCatFormStayTime = "CacularCatFormStayTime";
    public static readonly string CloseCoroutine = "CloseCoroutine";
    public static readonly string OnGiftReturnNumberStatistics = "OnGiftReturnNumberStatistics";
    public static readonly string isopenDiamondAddDith = "isopenDiamondAddDith";
    public static readonly string UpdateCatGoodList = "UpdateCatGoodList";
    public static readonly string CatRobotInfoUpdate = "CatRobotInfoUpdate";
    public static readonly string ChangeCatFormBackBtnState = "ChangeCatFormBackBtnState";
    public static readonly string CatAdoptionSucc = "CatAdoptionSucc";

    public static readonly string LoginChangeMoney = "LoginChangeMoney";
    public static readonly string RecordPremiumGiftBagBuy = "RecordPremiumGiftBagBuy";
    public static readonly string ItemListReceive = "ItemListReceive";
    public static readonly string TopMainFT = "TopMainFT";
    public static readonly string receiveButtonImageChange = "receiveButtonImageChange";
    public static readonly string FoodNumberShow = "FoodNumberShow";

    public static readonly string DoGuidStep = "DoGuidStep";

    public static readonly string HwLoginFormUpdat = "HwLoginFormUpdat";
    public static readonly string CatteryFormClose = "CatteryFormClose";
    public static readonly string SetTringerRange = "SetTringerRange";
    public static readonly string CatGuidUiClose = "CatGuidUiClose";
    public static readonly string OpenCatGuid = "OpenCatGuid";
    public static readonly string SetGuidPos = "SetGuidPos";
    public static readonly string GuidSpwanCat = "GuidSpwanCat";
    public static readonly string ClockTime = "ClockTime";
    public static readonly string CatGuidCanvasGroupOFF = "CatGuidCanvasGroupOFF";
    public static readonly string DestroyGuidCat = "DestroyGuidCat";
    public static readonly string CatGuidRepair = "CatGuidRepair";
    public static readonly string TouchAreaCanTouch = "TouchAreaCanTouch";

    public static readonly string EmailExpand = "EmailExpand";
    public static readonly string NewItemOpen = "NewItemOpen";

    public static readonly string ContenPostRest = "ContenPostRest";
    public static readonly string EmailNumberShow = "EmailNumberShow";
    public static readonly string PakageNumberShow = "PakageNumberShow";
    public static readonly string MasClose = "MasClose";
    public static readonly string MasOpen = "MasOpen";
    public static readonly string SwitchComuniada = "SwitchComuniada";
    public static readonly string GotoActivity = "GotoActivity";

    public static readonly string DanmakuChangeShowType = "DanmakuChangeShowType";
    public static readonly string DanmakuFlagTrigger = "DanmakuFlagTrigger";
    public static readonly string GotoMall = "GotoMall";
    public static readonly string GotoRead = "GotoRead";
    public static readonly string CloseDanmakuForm = "CloseDanmakuForm";
    public static readonly string CreateBookBarrageSuccess = "CreateBookBarrageSuccess";

    public static readonly string IGGSdkInit = "IGGSdkInit";

    public static readonly string AppConfComplete = "AppConfComplete";
    public static readonly string AppConfFail = "AppConfFail";

    public static readonly string AgainGetAppConf = "AgainGetAppConf";
    public static readonly string AgainGetAppConfIsMaintin = "AgainGetAppConfIsMaintin";
    public static readonly string MaintinWhileList = "MaintinWhileList";

    

}

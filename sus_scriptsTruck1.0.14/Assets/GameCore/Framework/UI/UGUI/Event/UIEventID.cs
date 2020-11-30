using Framework;

namespace UGUI
{
    public class UIEventID
    {
        public const string CREATE_ROLE_CHANGED = "CreateRoleChanged";

        public const string CREATE_ROLE_DONE = "CreateRoleDone";

        public const string CREATE_ROLE_ERROR = "CreateRoleError";

        public const string VIP_INFO_HAD_SET = "VipInfoHadSet";

        public const string MASTER_ATTRIBUTES_CHANGED = "MasterAttributesChanged";

        public const string MASTER_SYMBOLCOIN_CHANGED = "MasterSymbolCoinChanged";

        public const string MASTER_PVPLEVEL_CHANGED = "MasterPvpLevelChanged";

        public const string MASTER_JIFEN_CHANGED = "MasterJifenChanged";

        public const string MASTER_CURRENCY_CHANGED = "MasterCurrencyChanged";

        public const string TASK_UPDATED = "TaskUpdated";

        public const string TASK_SUBMIT = "TaskSubmit";

        public const string TASK_HUOYUEDU_Change = "TASK_HUOYUEDU_Change";

        public const string TASK_ON_GET_STUDENT_TASK = "TASK_ON_GET_STUDENT_TASK";

        public const string Rank_Friend_List = "Rank_Friend_List";

        public const string Rank_List = "Rank_List";

        public const string Rank_Arena_List = "Rank_Arena_List";

        public const string Arena_Fighter_Changed = "Arena_Fighter_Changed";

        public const string Arena_Record_List = "Arena_Record_List";

        public const string HERO_ADD = "HeroAdd";

        public const string HERO_EXPERIENCE_ADD = "HeroExperienceAdd";

        public const string HERO_LEVEL_CHANGE = "HeroLevelChange";

        public const string HERO_STAR_CHANGE = "HeroStarChange";

        public const string HERO_EXP_CHANGE = "HeroExpChange";

        public const string HERO_HP_CHANGE = "HeroHpChange";

        public const string HERO_QUALITY_CHANGE = "HeroQualityChange";

        public const string HERO_SKILL_LEVEL_UP = "HeroSkillLevelUp";

        public const string SKILL_POINT_CHANGE = "SkillPointChange";

        public const string HERO_SOUL_LEVEL_CHANGE = "HeroSoulLevelChange";

        public const string HERO_SOUL_EXP_CHANGE = "HeroSoulExpChange";

        public const string HERO_ENERGY_CHANGE = "HeroEnergyChange";

        public const string HERO_ENERGY_MAX = "HeroEnergyMax";

        public const string HERO_SKILL_LEVEL_CHANGE = "HeroSkillLevelChange";

        public const string HERO_EQUIP_CHANGE = "HeroEquipChange";

        public const string HERO_INIT_SUCCESS = "HeroInitSuccess";

        public const string HERO_UNLOCK_PVP = "HeroUnlockPvP";

        public const string HERO_LEARN_TALENT = "HeroLearnTalent";

        public const string HERO_SKIN_ADD = "HeroSkinAdd";

        public const string HERO_SYMBOL_PAGE_CHANGE = "HeroSymbolPageChange";

        public const string HERO_GOLD_COIN_IN_BATTLE_CHANGE = "HeroGoldCoinInBattleChange";

        public const string HERO_EQUIP_IN_BATTLE_CHANGE = "HeroEquipInBattleChange";

        public const string HERO_EXPERIENCE_TIME_UPDATE = "HeroExperienceTimeUpdate";

        public const string HOST_PLAYER_QUICKLY_BUY_EQUIP_LIST_CHANGED = "HostPlayerRecommendEquipListChanged";

        public const string HERO_SKILL_LEARN_BUTTON_STATE_CHANGE = "HeroSkillLearnButtonStateChange";

        public const string RESET_SKILL_BUTTON_MANAGER = "ResetSkillButtonManager";

        public const string HERO_RECOMMEND_EQUIP_INIT = "HeroRecommendEquipInit";

        public const string ACTOR_VISIBLE_TO_HOST_PLAYER_CHANGE = "ActorVisibleToHostPlayerChnage";

        public const string ShopBuy_Draw = "ShopBuyDraw";

        public const string ShopBuy_Purchase = "ShopBuyPurchase";

        public const string ShopBuy_LvlChallengeTime = "ShopBuyLvlChallengeTime";

        public const string MAIL_UNREAD_NUM_UPDATE = "MailUnReadNumUpdate";

        public const string MAIL_GUILD_MEM_UPDATE = "MAIL_GUILD_MEM_UPDATE";

        public const string MAIL_MSG_CENTER_CHANGE = "MAIL_MSG_CENTER_CHANGE";

        public const string NEWBIE_HOSTPLAYER_INANDHIT_BYTOWER = "NewbieHostPlayerInAndHitByTower";

        public const string EQUIP_LONGTIME_NOT_BUY_EQUIP = "EquipLongTimeNotBuyEquip";

        public const string NEWBIE_BITS_SYNC = "NEWBIE_BITS_SYNC";

        public const string NEWBIE_SKILLBTN_MANAGER_BTNUP = "NEWBIE_SKILLBTN_MANAGER_BTNUP";

        public const string IDIPNOTICE_UNREAD_NUM_UPDATE = "IDIPNOTICE_UNREAD_NUM_UPDATE";

        public const string FUC_UNLOCK_CONDITION_CHANGED = "FucUnlockConditionChanged";

        public const string FUNC_SWITCH_BITS_CHANGED = "FUNC_SWITCH_BITS_CHANGED";

        public static string DropTreasure = "OnDropTreasure";

        public static string AchievementRecorderEvent = "AchievementRecorderEvent";

        public static string StarSystemInitialized = "StarSystemInitialized";

        public static string PlayerRunAway = "PlayerRunAway";

        public static string HangupNtf = "HangupNtf";

        public static string DisConnectNtf = "DisConnectNtf";

        public static string FirstMoved = "FirstMoved";

        public static string PlayerReviveTime = "PlayerReviveTime";

        public static string CampTowerFirstAttackTime = "CampTowerFirstAttackTime";

        public static string CampStateChange = "CampStateChange";

        public static string BAG_ITEM_SALED = "BagItemSaled";

        public static string BATTLE_KDA_CHANGED = "BattleKdaChanged";

        public static string BATTLE_KDA_CHANGED_BY_ACTOR_DEAD = "BattleKdaChangedByActorDead";

        public static string BATTLE_HERO_PROPERTY_CHANGED = "BattleHeroPropertyChanged";

        public static string BATTLE_SHENFU_EFFECT_CHANGED = "BattleEffectChanged";

        public static string BATTLE_TOWER_DESTROY_CHANGED = "BattleTowerDestroyChanged";

        public static string BATTLE_DRAGON_KILL_CHANGED = "BattleDragonKillChanged";

        public static string Battle_ALL_PLAYER_KDA_NTFED = "BattleAllPlayerKDANTFED";

        public static string BATTLE_CAMPRANK_CHANGED = "BattleCampRankChanged";

        public static string Battle_TEAM_ORDER_CHANGED = "BattleTeamOrderChanged";

        public static string PVE_LEVEL_DETAIL_CHANGED = "Pve_Level_Detail_Changed";

        public static string BAG_ITEMS_UPDATE = "BagItemsUpdate";

        public static string EDITOR_REFRESH_GM_PANEL = "EDITOR_REFRESH_GM_PANEL";

        public static string EDITOR_CLEAR_PLAYER_PREFS = "EDITOR_CLEAR_PLAYER_PREFS";

        public static string Mall_Close_Mall = "MallCloseMall";

        public static string Mall_Change_Tab = "MallChangeTab";

        public static string Mall_Sub_Module_Loaded = "MallSubModuleLoaded";

        public static string Mall_Entry_Add_RedDotCheck = "MallEntryAddRedDotCheck";

        public static string Mall_Entry_Del_RedDotCheck = "MallEntryDelRedDotCheck";

        public static string Mall_Set_Free_Draw_Timer = "MallSetFreeDrawTimer";

        public static string Mall_Refresh_Tab_Red_Dot = "MallRefreshTabRedDot";

        public static string Mall_Receive_Roulette_Data = "MallReceiveRouletteData";

        public static string Mall_Get_Product_OK = "Mall_Get_Product_OK";

        public static string Mall_Recommend_Recommend_Data_Refresh = "MallRecommendRecommendDataRefresh";

        public static string Mall_Factory_Shop_Product_Bought_Success = "MallFactoryShopProductBoughtSuccess";

        public static string Mall_Factory_Shop_Product_Off_Sale = "MallFactoryShopProductOffSale";

        public static string Mall_Sort_Type_Changed = "MallSortTypeChanged";

        public static string Shop_Auto_Refresh_Shop_Items = "ShopAutoRefreshShopItems";

        public static string Shop_Receive_Shop_Items = "ShopReceiveShopItems";

        public static string Friend_Game_State_Change = "FriendGameStateChange";

        public static string Recent_Player_State_Change = "Recent_Player_State_Change";

        public static string ApolloHelper_Login_Success = "ApolloHelperLoginSuccess";

        public static string ApolloHelper_Login_Failed = "ApolloHelperLoginFailed";

        public static string ApolloHelper_Login_Need_Real_Name_Auth = "ApolloHelperLoginNeedRealNameAuth";

        public static string ApolloHelper_Platform_Not_Installed = "ApolloHelperPlatformNotInstalled";

        public static string ApolloHelper_Logout_Success = "ApolloHelperLogoutSuccess";

        public static string ApolloHelper_Logout_Failed = "ApolloHelperLogoutFailed";

        public static string ApolloHelper_Need_Login = "ApolloHelperNeed_Login";

        public static string ApolloHelper_Pay_Init_Success = "ApolloHelperPayInitSuccess";

        public static string ApolloHelper_Pay_Init_Failed = "ApolloHelperPayInitFailed";

        public static string ApolloHelper_Pay_Success = "ApolloHelperPaySuccess";

        public static string ApolloHelper_Pay_Failed = "ApolloHelperPayFailed";

        public static string ApolloHelper_Pay_Risk_Hit = "ApolloHelperPayRiskHit";

        public static string PlayerInfoSystem_Info_Received = "PlayerInfoSystemInfoReceived";

        public static string PlayerInfoSystem_Mini_Info_Closed = "PlayerInfoSystemMiniInfoClosed";

        public static string PlayerInfoSystem_Tab_Change = "PlayerInfoSystemTabChange";

        public static string PlayerInfoSystem_Form_Close = "PlayerInfoSystemTabClose";

        public static string PlayerBlock_Success = "PlayerBlock_Success";

        public static string SymbolEquipSuc = "SymbolEquipSuc";

        public static string SymbolFreeDrawChg = "SymbolFreeDrawChg";

        public static string SINGLEGAME_ERR_FREEHERO = "Single_Err_Free_Hero";

        public static string ACHIEVE_GET_RANKING_ACCOUNT_INFO = "ACHIEVE_GET_ACHIEVE_RANKING_ACCOUNT_INFO";

        public static string ACHIEVE_FILTER_MENU_CHANGE = "ACHIEVE_FILTER_MENU_CHANGE";

        public static string ACHIEVE_TROPHY_REWARD_INFO_STATE_CHANGE = "ACHIEVE_TROPHY_REWARD_INFO_STATE_CHANGE";

        public static string ACHIEVE_SERY_SELECT_DONE = "ACHIEVE_SERY_SELECT_DONE";

        public static string ACHIEVE_RECEIVE_PVP_BAN_MSG = "ACHIEVE_RECEIVE_PVP_BAN_MSG";

        public static string NAMECHANGE_PLAYER_NAME_CHANGE = "NAMECHANGE_PLAYER_NAME_CHANGE";

        public static string NAMECHANGE_GUILD_NAME_CHANGE = "NAMECHANGE_GUILD_NAME_CHANGE";

        public static string ROLLING_SYSTEM_CHAT_INFO_RECEIVED = "ROLLING_SYSTEM_CHAT_INFO_RECEIVED";

        public static string LOBBY_STATE_ENTER = "LOBBY_STATE_ENTER";

        public static string LOBBY_STATE_LEAVE = "LOBBY_STATE_LEAVE";

        public static string LOBBY_PURE_LOBBY_SHOW = "LOBBY_PURE_LOBBY_SHOW";

        public static string UI_FORM_CLOSED = "UI_FORM_CLOSED";

        public static string GLOBAL_REFRESH_TIME = "GLOBAL_REFRESH_TIME";

        public static string GLOBAL_SERVER_TO_CLIENT_CFG_READY = "GlobalServerToClientCfgReady";

        public static string ADVANCE_STOP_LOADING = "Advance_Stop_Loading";

        public static string NOBE_STATE_CHANGE = "NOBE_STATE_CHANGE";

        public static string NOBE_STATE_HEAD_CHANGE = "NOBE_STATE_HEAD_CHANGE";

        public static string SERVER_SKIN_DATABIN_READY = "Server_Skin_Databin_Ready";

        public static string ERRCODE_NTF = "Error_Code_Ntf";

        public static string INVITE_ROOM_ERRCODE_NTF = "Invite_Room_ErrCode_Ntf";

        public static string INVITE_TEAM_ERRCODE_NTF = "Invite_Team_ErrCode_Ntf";

        public static string NEWDAY_NTF = "NewDay_Ntf";

        public static string HEAD_IMAGE_FLAG_CHANGE = "HEAD_IMAGE_FLAG_CHANGE";

        public static string LOUDSPEAKER_MSG_RECEIVE = "LOUDSPEAKER_MSG_RECEIVE";

        public static string SHARE_PVP_SETTLEDATA_CLOSE = "SHARE_PVP_SETTLEDATA_CLOSE";

        public static string SHARE_TIMELINE_SUCC = "SHARE_TIMELINE_SUCC";

        public static string GAMER_REDDOT_CHANGE = "GAMER_REDDOT_CHANGE";

        public static string GPS_DATA_GOT = "GPS_DATA_GOT";

        public static string RECEIVE_RESERVE_DATA_CHANGE = "RECEIVE_RESERVE_DATA_CHANGE";

        public static string VERSION_UPDATE_FIN = "VERSION_UPDATE_FIN";

        public static string VOICE_RECEIVE_ENTER_ROOM = "VOICE_RECEIVE_ENTER_ROOM";

        public static string LBS_GOT_SOME_TITLEADDR = "LBS_GOT_SOME_TITLEADDR";

        public static string LBS_GOT_SOME_LBSADDR = "LBS_GOT_SOME_LBSADDR";

        public static string GAME_SETTING_SHOWEQUIPINFO_CHANGE = "Game_Setting_ShowEquipInfo_Change";

        public static string SETTING_FORM_CLOSE = "SETTING_FORM_CLOSE";

        public static string SETTING_TAB_CHANGE = "SETTING_TAB_CHANGE";

        public static string CUSTOM_EQUIP_RANK_LIST_GET = "Custom_Equip_Rank_List_Get";

        public static string GOD_EQUIP_RANK_LIST_GET = "God_Equip_Rank_List_Get";

        public static string REPLAY_KIT_STATUS_CHANGE = "ReplayKitStatusChange";

        public static string REPLAY_KIT_FINISH_PREVIEW = "ReplayKitFinishPreview";

        public static string VIDEOREPORT_STARTREPORT = "VideoReport_StartReport";

        public static string VIDEOREPORT_STARTRECORD = "VideoReport_StartRecord";

        public static string VIDEOREPORT_ENTERREPORT_RECORDSTEP = "VideoReport_EnterReport_RecordStep";

        public static string VIDEOREPORT_ENDRECORD = "VideoReport_EndRecord";

        public static string VIDEOREPORT_ENDREPORT = "VideoReport_EndReport";

        public static string VIDEOREPORT_CLOSED = "VideoReport_Closed";

        public static string VIDEOREPORT_APPBEGINPAUSED = "VideoReport_AppBeginPaused";

        public static string VIDEOREPORT_APPENDPAUSED = "VideoReport_AppEndPaused";

        public static string KILL_NOTIFY = "Kill_Notify";

        public static string SECURE_PWD_SET_RESULT = "SECURE_PWD_SET_RESULT";

        public static string SECURE_PWD_CLOSE_RESULT = "SECURE_PWD_CLOSE_RESULT";

        public static string SECURE_PWD_MODIFY_RESULT = "SECURE_PWD_MODIFY_RESULT";

        public static string SECURE_PWD_FORCE_CLOSE_RESULT = "SECURE_PWD_FORCE_CLOSE_RESULT";

        public static string SECURE_PWD_CANCEL_FORCE_CLOSE_RESULT = "SECURE_PWD_CANCEL_FORCE_CLOSE_RESULT";

        public static string SECURE_PWD_STATUS_CHANGE = "SECURE_PWD_STATUS_CHANGE";

        public static string SECURE_PWD_OP_CANCEL = "SECURE_PWD_OP_CANCEL";

        public static string SECURE_PWD_VALIDATE_RESULT = "SECURE_PWD_VALIDATE_RESULT";

        public static string LOTTERY_GET_NEW_SYMBOL = "LOTTERY_GET_NEW_SYMBOL";

        public static string BATTLE_DATAANL_DRAGON_KILLED = "Battle_DataAnl_SmallDragon_Killed";

        public static string BATTLE_TEAMFIGHT_DAMAGE_UPDATE = "Battle_TeamFight_DamageUpdate";

        public static string GUILD_PLATFORM_GROUP_STATUS_CHANGE = "GUILD_PLATFORM_GROUP_STATUS_CHANGE";

        public static string GUILD_PLATFORM_GROUP_OP_STATUS = "GUILD_PLATFORM_GROUP_OP_STATUS";

        public static string GUILD_GET_PLATFORM_GROUP_GUILD_ID_SUCCESS = "GUILD_GET_PLATFORM_GROUP_GUILD_ID_SUCCESS";

        public static string GUILD_GET_PLATFORM_GROUP_GUILD_ID_FAIL = "GUILD_GET_PLATFORM_GROUP_GUILD_ID_FAIL";

        public static string GUILD_SEND_INVITE_TO_PLATFORM_GROUP_SUCCESS = "GUILD_SEND_INVITE_TO_PLATFORM_GROUP_SUCCESS";

        public static string GUILD_SEND_REDENVELOPE_MSG_SUCCESS = "GUILD_SEND_REDENVELOPE_MSG_SUCCESS";

        public static string GUILD_PLATFORM_GROUP_SYNC_OPEN_ID = "GUILD_PLATFORM_GROUP_SYNC_OPEN_ID";

        public static string GUILD_SEASON_RESET = "GUILD_SEASON_RESET";

        public static string GUILD_ICON_USE = "GUILD_ICON_USE";

        public static string GUILD_RED_ENVELOPE_NEW_DATA = "GUILD_RED_ENVELOPE_NEW_DATA";

        public static string GUILD_RED_ENVELOPE_TAKEN_RECORD_NEW_DATA = "GUILD_RED_ENVELOPE_TAKEN_RECORD_NEW_DATA";

        public static string GUILD_RED_ENVELOPE_RANK_CHG = "GUILD_RED_ENVELOPE_RANK_CHG";

        public static string GUILD_RED_ENVELOPE_RAN_OUT = "GUILD_RED_ENVELOPE_RED_ENVELOPE_RAN_OUT";

        public static string MATCHING_SYSTEM_MATCHING_STATUS_CHANGE = "MATCHING_SYSTEM_MATCHING_STATUS_CHANGE";

        public static string PLAYER_JOIN_TEAM = "PLAYER_JOIN_TEAM";

        public static string PLAYER_JOIN_ROOM = "PLAYER_JOIN_ROOM";

        public static string PLAYER_JOIN_TEAM_ERR = "PLAYER_JOIN_TEAM_ERR";

        public static string PLAYER_JOIN_ROOM_ERR = "PLAYER_JOIN_ROOM_ERR";

        public static string GlobalNameVarChange = "GlobalNameVarChange";

        public static string KillSysBattleMsgBoardcast = "KillSysBattleMsgBoardcast";

        public static string CUSTOM_COMPETITION_NTF = "CUSTOM_COMPETITION_NTF";

        public static string CUSTOM_COMPETITION_GET_ERROR = "CUSTOM_COMPETITION_GET_ERROR";

        public static string CUSTOM_COMPETITION_STOP_NTF = "CUSTOM_COMPETITION_STOP_NTF";

        public static string CUSTOM_COMPETITION_PREPARE_NTF = "CUSTOM_COMPETITION_PREPARE_NTF";

        public static string CUSTOM_COMPETITION_ONCREATE_NTF = "CUSTOM_COMPETITION_ONCREATE_NTF";

        public static string CUSTOM_COMPETITION_OB_INFO_NTF = "CUSTOM_COMPETITION_OB_INFO_NTF";

        public static string CUSTOM_COMPETITION_INVITED_LIST_NTF = "CUSTOM_COMPETITION_INVITED_LIST_NTF";

        public static string CUSTOM_COMPETITION_TEAM_RECOMMEND_NTF = "CUSTOM_COMPETITION_TEAM_RECOMMEND_NTF";

        public static string CUSTOM_COMPETITION_APPLYTOME_LIST_NTF = "CUSTOM_COMPETITION_APPLYTOME_LIST_NTF";

        public static string CUSTOM_COMPETITION_TEAM_CHG_NTF = "CUSTOM_COMPETITION_TEAM_CHG_NTF";

        public static string CUSTOM_COMPETITION_FRIEND_LIST_NTF = "CUSTOM_COMPETITION_FRIEND_LIST_NTF";

        public static string CUSTOM_COMPETITION_SNS_FRIEND_LIST_NTF = "CUSTOM_COMPETITION_SNS_FRIEND_LIST_NTF";

        public static string CUSTOM_COMPETITION_FRIEND_LIST_ADD_NTF = "CUSTOM_COMPETITION_FRIEND_LIST_ADD_NTF";

        public static string CUSTOM_COMPETITION_FRIEND_LIST_DEL_NTF = "CUSTOM_COMPETITION_FRIEND_LIST_DEL_NTF";

        public static string CUSTOM_COMPETITION_TEAM_TIPS_NTF = "CUSTOM_COMPETITION_TEAM_TIPS_NTF";

        public static string CUSTOM_COMPETITION_FRIEND_LIST_CHG = "CUSTOM_COMPETITION_FRIEND_LIST_CHG";

        public static string CUSTOM_COMPETITION_APPLY_TO_ME_NTF = "CUSTOM_COMPETITION_APPLY_TO_ME_NTF";

        public static string CUSTOM_COMPETITION_ATTENTION_SUCCESS = "CUSTOM_COMPETITION_ATTENTION_SUCCESS";

        public static string CUSTOM_COMPETITION_NEW_ATTENTION_COUNT_NTF = "CUSTOM_COMPETITION_NEW_ATTENTION_COUNT_NTF";

        public static string CUSTOM_COMPETITION_CANCEL_ATTENTION = "CUSTOM_COMPETITION_CANCEL_ATTENTION";

        public static string CUSTOM_COMPETITION_SIGNUP_SUCCESS = "CUSTOM_COMPETITION_SIGNUP_SUCCESS";

        public static string CUSTOM_COMPETITION_CANCEL_SIGNUP_SUCCESS = "CUSTOM_COMPETITION_CANCEL_SIGNUP_SUCCESS";

        public static string CUSTOM_COMPETITION_ACNT_HONOR_RSP = "CUSTOM_COMPETITION_ACNT_HONOR_RSP";

        public static string CUSTOM_COMPETITION_HONOR_HISTORY_RSP = "CUSTOM_COMPETITION_HONOR_HISTORY_RSP";

        public static string CUSTOM_COMPETITION_PAUSE_CHG = "CUSTOM_COMPETITION_PAUSE_CHG";

        public static string CUSTOM_COMPETITION_DELAY_NTF = "CUSTOM_COMPETITION_DELAY_NTF";

        public static string CUSTOM_COMPETITION_VERSION_UPDATE_NTF = "CUSTOM_COMPETITION_VERSION_UPDATE_NTF";

        public static string CUSTOM_COMPETITION_TEAM_CHG = "CUSTOM_COMPETITION_TEAM_CHG";

        public static string CUSTOM_COMPETITION_OPPONET_ONLINE_NTF = "CUSTOM_COMPETITION_OPPONET_ONLINE_NTF";

        public static string CUSTOM_COMPETITION_ROOM_OPPOMEMBER_CHG_NTF = "CUSTOM_COMPETITION_ROOM_OPPOMEMBER_CHG_NTF";

        public static string CUSTOM_COMPETITION_ROOM_OUT_NTF = "CUSTOM_COMPETITION_ROOM_OUT_NTF";

        public static string CUSTOM_COMPETITION_GAME_RECORD_NTF = "CUSTOM_COMPETITION_GAME_RECORD_NTF";

        public static string CUSTOM_COMPETITION_RESULT_NTF = "CUSTOM_COMPETITION_RESULT_NTF";

        public static string CUSTOM_COMPETITION_DELAY_OR_PAUSE_REDHOT_NTF = "CUSTOM_COMPETITION_DELAY_OR_PAUSE_REDHOT_NTF";

        public static string CUSTOM_COMPETITION_PANDORA_RECLIST_NTF = "CUSTOM_COMPETITION_PANDORA_RECLIST_NTF";

        public static string CUSTOM_COMPETITION_TEAM_LOG_NTF = "CUSTOM_COMPETITION_TEAM_LOG_NTF";

        public static string CUSTOM_COMPETITION_TEAM_HISTORY_NTF = "CUSTOM_COMPETITION_TEAM_HISTORY_NTF";

        public static string CUSTOM_COMPETITION_TEAM_STATISTIC_NTF = "CUSTOM_COMPETITION_TEAM_STATISTIC_NTF";

        public static string CUSTOM_COMPETITION_COM_STATISTIC_NTF = "CUSTOM_COMPETITION_COM_STATISTIC_NTF";

        public static string CUSTOM_COMPETITION_TEAM_BE_KICKED_NTF = "CUSTOM_COMPETITION_TEAM_BE_KICKED_NTF";

        public static string CUSTOM_COMPETITION_VIEWTEAM_NTF = "CUSTOM_COMPETITION_VIEWTEAM_NTF";

        public static string CUSTOM_COMPETITION_JOIN_HISTORY_NTF = "CUSTOM_COMPETITION_JOIN_HISTORY_NTF";

        public static string CUSTOM_COMPETITION_CREATE_HISTORY_NTF = "CUSTOM_COMPETITION_CREATE_HISTORY_NTF";

        public static string CUSTOM_COMPETITION_PAUSE_DELAY_RED_NTF = "CUSTOM_COMPETITION_PAUSE_DELAY_RED_NTF";

        public static string CUSTOM_COMPETITION_FACE2FACE_SELECT_MAP_NTF = "CUSTOM_COMPETITION_FACE2FACE_SELECT_MAP_NTF";

        public static string CUSTOM_COMPETITION_FACE2FACE_WAIT_SELECT_MAP_NTF = "CUSTOM_COMPETITION_FACE2FACE_WAIT_SELECT_MAP_NTF";

        public static string CUSTOM_COMPETITION_FACE2FACE_CANCEL_ROOM_NTF = " CUSTOM_COMPETITION_FACE2FACE_CANCEL_ROOM_NTF";

        public static string SERVER_TIME_NTF = "SERVER_TIME_NTF";

        public static string CUSTOM_COMPETITION_FLEX_MODE_INFO_NTF = "CUSTOM_COMPETITION_RLEX_MODE_INFO_NTF";

        public static string CUSTOM_COMPETITION_FLEX_MODE_PREPARE = "CUSTOM_COMPETITION_RLEX_MODE_PREPARE";

        public static string GAME_PICK_START = "GAME_PICK_START";

        public static string CUSTOM_COMPETITION_PLATFORM_GROUP_STATUS_CHANGE = "CUSTOM_COMPETITION_PLATFORM_GROUP_STATUS_CHANGE";

        public static string CUSTOM_COMPETITION_PLATFORM_GROUP_OP_STATUS_CHANGE = "CUSTOM_COMPETITION_PLATFORM_GROUP_OP_STATUS_CHANGE";

        public static string MISHU_ROBOT_TTS_DOWNLOAD_SUC = "MISHU_ROBOT_TTS_DOWNLOAD_SUC";

        public static string MISHU_ROBOT_RSP_QUESTION = "MISHU_ROBOT_RSP_QUESTION";
    }
}

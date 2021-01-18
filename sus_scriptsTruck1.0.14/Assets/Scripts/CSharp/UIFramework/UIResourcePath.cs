using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFormName
{
    //Resources
    public readonly static string LoadingForm = "UI/UIFormPrefab/Canvas_Loading";// 3
    public readonly static string NetLoadingForm = "UI/UIFormPrefab/Canvas_NetLoading";// loading
    public readonly static string ServiceSelectForm = "UI/UIFormPrefab/Canvas_SelectServiceForm";// loading
    public readonly static string AlertUiForm = "UI/UIFormPrefab/Canvas_AlertForm";//9
    public readonly static string NetworkAlert = "UI/UIFormPrefab/Canvas_NetworkAlert";//9
    public readonly static string UIUpdateModule = "UI/UIFormPrefab/UIUpdateModule";//9
    public readonly static string MigrationAccountForm = "UI/UIFormPrefab/Canvas_MigrationAccountForm";//9
    public readonly static string MoveAccountLogin = "UI/UIFormPrefab/Canvas_MoveAccountLogin";//9

    //AB Resident
    public readonly static string Global = "UI/Resident/UI/Canvas_Global";//8
    public readonly static string BookDisplayForm = "UI/Resident/UI/Canvas_BookDisplay";//5
    public readonly static string BookLoadingForm = "UI/Resident/UI/Canvas_BookLoading";//8
    public readonly static string ChapterLoading = "UI/Resident/UI/Canvas_ChapterLoading";//8
    public readonly static string SettingForm = "UI/Resident/UI/Canvas_Setting";//3
    public readonly static string ChargeMoneyForm = "UI/Resident/UI/Canvas_ChargeMoney";//6
    public readonly static string ChargeTipsForm = "UI/Resident/UI/Canvas_ChargeTips";//5
    public readonly static string ComingSoonDialogForm = "UI/Resident/UI/Canvas_ComingSoonDialog";//9
    public static readonly string ChargeItemForm = "UI/Resident/UI/ChargeItemForm";
    public readonly static string BroadcastTipForm = "UI/Resident/UI/Canvas_BroadCastTipForm";//9
    public readonly static string ProfileForm = "UI/Resident/UI/Canvas_Profile";//9

    public static readonly string LoginForm = "UI/Resident/UI/Canvas_Login";//9
    public static readonly string PopupTipsForm = "UI/Resident/UI/PopupTips/Canvas_PopupTips";//9

    public readonly static string EmojiMsgForm = "UI/Resident/UI/Canvas_EmojiMessages";//3
    public readonly static string ChatForm = "UI/Resident/UI/Canvas_ChatForm";//3

    public readonly static string DanmakuForm = "UI/Resident/UI/Canvas_DanmakuForm";//4
    public static readonly string AwardGame = "UI/Resident/UI/AwardGame";


    public readonly static string MainFormTop = "UI/Resident/UI/Canvas_Main_top";//7
    public static readonly string ChargeMoneyKeyItem = "UI/Resident/UI/ChargeMoneyKeyItem";
    public static readonly string ChargeMoneyDiamondItem = "UI/Resident/UI/ChargeMoneyDiamondItem";
  
    public readonly static string Navigation = "UI/Resident/UI/Canvas_Navigation";//3
    public readonly static string Activity = "UI/Resident/UI/Canvas_Activity";//3
    public readonly static string EvreyDayActivty = "UI/Resident/UI/Canvas_EvreyDayActivty";//8
    public readonly static string RewardEffectInit = "UI/Resident/UI/RewardEffectInit";
    public static readonly string BarrageItem = "UI/Resident/UI/BarrageItem";

    

    //AB Dynamic
    public readonly static string BarrageForm = "UI/Resident/UI/Canvas_BarrageForm";//3
    public readonly static string NewChargeTips = "UI/Resident/UI/Canvas_NewChargeTips";//5
    public readonly static string ADSReward = "UI/Resident/UI/Canvas_ADSReward";
    public readonly static string FAQConten = "UI/Resident/UI/Canvas_FAQConten";//3
    public readonly static string FAQFeedBack = "UI/Resident/UI/Canvas_FAQFeedBack";//3
  
    public readonly static string UIBubbleSelectionItem = "Assets/Bundle/UI/BookReading/BubbleChat/UIBubbleSelectionItem.prefab";//3
    
    public readonly static string Announcement = "UI/Announcement/Canvas_Announcement";

}

public enum UIEventMethodName
{
    BookReadingForm_ShowDialogueType,
    BookReadingForm_IsTweening,
    DLogForm_AddLogMessage,
    ClickHeadToChangeFaceExpression,
}
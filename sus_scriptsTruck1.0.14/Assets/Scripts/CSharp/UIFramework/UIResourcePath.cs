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
    public readonly static string MainForm = "UI/Resident/UI/Canvas_Main";//3
    public readonly static string SettingForm = "UI/Resident/UI/Canvas_Setting";//3
    public readonly static string ChargeMoneyForm = "UI/Resident/UI/Canvas_ChargeMoney";//6
    public readonly static string ChargeTipsForm = "UI/Resident/UI/Canvas_ChargeTips";//5
    public readonly static string ComingSoonDialogForm = "UI/Resident/UI/Canvas_ComingSoonDialog";//9
    public static readonly string ChargeItemForm = "UI/Resident/UI/ChargeItemForm";
    public readonly static string LoginToRewardUiForm = "UI/Resident/UI/Canvas_LoginToRewardUiForm";//8
    public readonly static string BroadcastTipForm = "UI/Resident/UI/Canvas_BroadCastTipForm";//9
    public readonly static string ProfileForm = "UI/Resident/UI/Canvas_Profile";//9
    public readonly static string HwLoginForm = "UI/Resident/UI/Canvas_HwLoginForm";//9

    public readonly static string GamePlaySetting = "UI/Resident/UI/Canvas_GamePlaySetting";//3
    //public static readonly string LoginRewardItemForm = "UI/Resident/UI/LoginRewardItemForm";
    public static readonly string NewLoginRewardItemForm = "UI/Resident/UI/NewLoginRewardItemForm";
    public static readonly string LoginForm = "UI/Resident/UI/Canvas_Login";//9
    public static readonly string PopupTipsForm = "UI/Resident/UI/PopupTips/Canvas_PopupTips";//9
    public static readonly string InviteForm = "UI/Resident/UI/Canvas_Invite";//9

    public readonly static string SceneTapForm = "UI/Resident/UI/Canvas_SceneTap";//3
    public readonly static string PuzzleForm = "UI/Resident/UI/Canvas_Puzzle";//3
    public readonly static string LuckRollerForm = "UI/Resident/UI/Canvas_LuckRoller";//3
    public readonly static string EmojiMsgForm = "UI/Resident/UI/Canvas_EmojiMessages";//3
    public readonly static string ChatForm = "UI/Resident/UI/Canvas_ChatForm";//3
    public readonly static string ScreenShotForm = "UI/Resident/UI/Canvas_ScreenShotFrame";//3
    public readonly static string BookReadingBtnSelectItem = "UI/Resident/UI/ChoiceButtonSelection";//3

    public readonly static string DanmakuForm = "UI/Resident/UI/Canvas_DanmakuForm";//4

    public static readonly string AwardGame = "UI/Resident/UI/AwardGame";
    public static readonly string InviteGiftBag = "UI/Resident/UI/Canvas_InviteGiftBag.prefab";

    public readonly static string MainFormTop = "UI/Resident/UI/Canvas_Main_top";//7
    public static readonly string ChargeMoneyKeyItem = "UI/Resident/UI/ChargeMoneyKeyItem";
    public static readonly string ChargeMoneyDiamondItem = "UI/Resident/UI/ChargeMoneyDiamondItem";
    public static readonly string ChargeMoneyTicketItem = "UI/Resident/UI/ChargeMoneyTicketItem";
    public readonly static string GuideForm = "UI/Resident/UI/Canvas_GuideForm";//3
    public readonly static string NoviceGuide = "UI/Resident/UI/Canvas_noviceGuide";//3
    public readonly static string HelpSupportForm = "UI/Resident/UI/Canvas_HelpSupport";//3
    public readonly static string EmailForm = "UI/Resident/UI/Canvas_Email";//3
    public readonly static string EmailNotice = "UI/Resident/UI/Canvas_EmailNotice";//3
    public readonly static string GamePlayFeedback = "UI/Resident/UI/Canvas_GamePlayFeedback";//3
    public readonly static string ShareForm = "UI/Resident/UI/Canvas_Share";//6
    public readonly static string FeedbackForm = "UI/Resident/UI/Canvas_Feedback";//3
    public readonly static string BookActivities = "UI/Resident/UI/Canvas_BookActivities";//3
    public readonly static string ChristmasActivities = "UI/Resident/UI/Canvas_ChristmasActivities";//3
    public readonly static string Navigation = "UI/Resident/UI/Canvas_Navigation";//3
    public readonly static string FirstGigtGroup = "UI/Resident/UI/Canvas_FirstGigtGroup";//3
    public readonly static string Notice = "UI/Resident/UI/Canvas_Notice";//3
    public readonly static string Activity = "UI/Resident/UI/Canvas_Activity";//3
    public readonly static string VIP = "UI/Resident/UI/Canvas_VIP";//3
    public readonly static string VIPDay = "UI/Resident/UI/Canvas_VIPEveryDay";//9
    public readonly static string PublicNotice = "UI/Resident/UI/Canvas_PublicNotice";//9
    public readonly static string EvreyDayActivty = "UI/Resident/UI/Canvas_EvreyDayActivty";//8
    public readonly static string RewardEffectInit = "UI/Resident/UI/RewardEffectInit";
    public readonly static string ReplyForDetalls = "UI/Resident/UI/Canvas_ReplyForDetalls";//3
    public readonly static string newRCRform = "UI/Resident/UI/Canvas_newRCRform";//7
    public static readonly string BarrageItem = "UI/Resident/UI/BarrageItem";

    public static readonly string MainDown = "UI/Resident/UI/Canvas_Main_down";//7
    public readonly static string TypeSelection = "UI/Resident/UI/Canvas_TypeSelection";//3
    public static readonly string EmailAndNew = "UI/Resident/UI/Canvas_EmailAndNewForm";//3

    public readonly static string Ofertas = "UI/Resident/UI/Canvas_Ofertas";//3
    public readonly static string ActivityAndNews = "UI/Resident/UI/Canvas_ActivityAndNews";//3
    public readonly static string Comuniada = "UI/Resident/UI/Canvas_Comuniada";//3
    public readonly static string ComuniadaMas= "UI/Resident/UI/Canvas_Mas";//3
    public readonly static string Busqueda = "UI/Resident/UI/Canvas_Busqueda";//3
    

    //AB Dynamic
    public readonly static string BarrageForm = "UI/Resident/UI/Canvas_BarrageForm";//3
    public readonly static string CatMain = "UI/Resident/UI/Canvas_CatMain";//3
    public readonly static string MyChart = "UI/Resident/UI/Canvas_MyChart";//3
    public readonly static string CatShop = "UI/Resident/UI/Canvas_CatShop";//3
    public readonly static string CatDecorations = "UI/Resident/UI/Canvas_CatDecorations";//3
    public readonly static string CatAnimal = "UI/Resident/UI/Canvas_CatAnimal";//3
    public readonly static string CatStory = "UI/Resident/UI/Canvas_CatStory";//3
    public readonly static string CatGiftFromAnimalForm = "UI/Resident/UI/Canvas_CatGiftFromAnimalForm";//3
    public readonly static string CatPublicForm = "UI/Resident/UI/Canvas_CatPublicForm";//3
    public readonly static string CatLoading = "UI/Resident/UI/Canvas_CatLoading";//3
    public readonly static string CatSetForm = "UI/Resident/UI/Canvas_CatSetForm";//3
    public readonly static string CatFoodSetForm = "UI/Resident/UI/Canvas_CatFoodSetForm";//3
    public readonly static string CatTop = "UI/Resident/UI/Canvas_CatTop";//3
    public readonly static string Cattery = "UI/Resident/UI/Canvas_Cattery";//3
    public readonly static string CatDetails = "UI/Resident/UI/Canvas_CatDetails";//3
    public readonly static string CatStoryDetails = "UI/Resident/UI/Canvas_CatStoryDetails";//3
    public readonly static string SettingNav = "UI/Resident/UI/Canvas_SettingNav";//3
    public readonly static string FAQ = "UI/Resident/UI/Canvas_FAQ";//3
    public readonly static string CatWelcomBack = "UI/Resident/UI/Canvas_CatWelcomBack";//3
    public readonly static string CatDiamondExchange = "UI/Resident/UI/Canvas_CatDiamondExchange";//4
    public readonly static string StoryItems = "UI/Resident/UI/Canvas_StoryItems";//4
    public readonly static string NewChargeTips = "UI/Resident/UI/Canvas_NewChargeTips";//5
    public readonly static string EmailTextNotice = "UI/Resident/UI/Canvas_EmailTextNotice";//7
    public readonly static string LoadingFeedBack = "UI/Resident/UI/Canvas_LoadingFeedBack";//3
    public readonly static string CatGuid = "UI/Resident/UI/Canvas_CatGuid";//4
    public readonly static string ADSReward = "UI/Resident/UI/Canvas_ADSReward";
    public readonly static string FAQConten = "UI/Resident/UI/Canvas_FAQConten";//3
    public readonly static string EmailInfo = "UI/Resident/UI/Canvas_EmailInfo";//3
    public readonly static string FAQFeedBack = "UI/Resident/UI/Canvas_FAQFeedBack";//3
    
    public readonly static string Ratinglevel = "UI/Resident/UI/Canvas_Ratinglevel";//3

    public readonly static string CatAnimalAttribute = "UI/Resident/UI/Canvas_CatAnimalAttribute";//4

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
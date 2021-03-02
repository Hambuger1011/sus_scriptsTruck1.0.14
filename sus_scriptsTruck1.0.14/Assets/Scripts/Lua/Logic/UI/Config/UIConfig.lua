 --[[
-- added by wsh @ 2017-11-30
-- UI模块配置表，添加新UI模块时需要在此处加入
--]]

	-- 模块 = 模块配置表
local UIConfig = {
	require "Logic/UI/UI_Loading/UILoadingView",
	require "Logic/UI/UI_BookLoading/UIBookLoadingView",
	require "Logic/UI/UI_BookReading/UIBookReadingView",
	require "Logic/UI/UI_ValueChoice/UIValueChoiceView",
	require "Logic/UI/UI_GetADiamonds/UIGetADiamondsView",
	require "Logic/UI/UI_Guide/UIGuide",
	require "Logic/UI/UI_BookReading/UI_Comment",
	require "Logic/UI/UI_BookReading/UI_Reply",

	require "Logic/UI/UI_Account/UI_AccountInfo",
	require "Logic/UI/UI_Activity/Panel/Activity/InvitePanel",
	require "Logic/UI/UI_Account/UI_MoveWait",
	require "Logic/UI/UI_Account/UI_PlatformQuickLogin",


	require "Logic/UI/UI_Collect/UICollectForm",
	require "Logic/UI/UI_Rank/UIRankForm",
	require "Logic/UI/UI_Ratinglevel/UIRatinglevelForm",
	require "Logic/UI/UI_SignIn/UISignTipForm",
	require "Logic/UI/UI_FirstCharge/UIFirstChargeForm",
	require "Logic/UI/UI_NewBookTips/UINewBookTipsForm",
	require "Logic/UI/UI_DressUp/UIDressUpForm",
	require "Logic/UI/UI_Comuniada/UIMasForm",
	require "Logic/UI/UI_Busqueda/UIBusquedaForm",

	-- require "Logic/StoryEditor/UI/Editor/UIStoryListView",
	-- require "Logic/StoryEditor/UI/Editor/UIStoryCreateNewView",
	-- require "Logic/StoryEditor/UI/Editor/UIStoryEditorView",
	-- require "Logic/StoryEditor/UI/Editor/UIStoryRoleView",
	-- require "Logic/StoryEditor/UI/Editor/UIPictureView",
	-- require "Logic/StoryEditor/UI/Editor/UIAudioView",
	-- require "Logic/StoryEditor/UI/Preview/UIStoryPreview",

	require "Logic/StoryEditor/UI/Utils/UITexture2DClipper",
	require "Logic/StoryEditor/UI/NewBook/UIStory_NewBook",
	require "Logic/StoryEditor/UI/Detials/UIStory_Detials",
	require "Logic/StoryEditor/UI/Editor/UIStory_Editor",
	require "Logic/StoryEditor/UI/Preview/UIStory_Preview",
	require "Logic/StoryEditor/UI/Keyboard/UIStory_Keyboard",
	require "Logic/StoryEditor/UI/Reading/UIStoryChapterForm",
	require "Logic/StoryEditor/UI/Guide/UIStory_Guide",



	require "Logic/UI/UI_Main/UIMainForm",
	require "Logic/UI/UI_Main/UIMainDownForm",
	require "Logic/UI/UI_Search/UISearchForm",
	require "Logic/UI/UI_Activity/UIActivityForm",
	require "Logic/UI/UI_PersonalCenterForm/UIPersonalCenterForm",
	require "Logic/UI/UI_ActivityBanner/UIActivityBannerForm",
	require "Logic/UI/UI_Comuniada/UIComuniadaForm",
	require "Logic/UI/UI_Community/UICommunityForm",
	require "Logic/UI/UI_Email/UIEmailForm",
	require "Logic/UI/UI_Email/UIEmailInfoForm",
	require "Logic/UI/UI_Chat/UIChatForm",
	require "Logic/UI/UI_Pakage/UIPakageForm",
	require "Logic/UI/UI_DayPass/UIDayPassForm",
	require "Logic/UI/UI_Doves/UIDovesForm",
	require "Logic/UI/UI_Investment/UIInvestmentForm",
	require "Logic/UI/UI_Lottery/UILotteryForm",
	require "Logic/UI/UI_PrizeHistory/UIPrizeHistoryForm",
}
return UIConfig
--return ConstClass("UIConfig", UIConfig)
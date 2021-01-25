logic = {}




--cs
logic.cs = {}
logic.cs.Type = CS.System.Type
logic.cs.Application = CS.UnityEngine.Application

logic.cs.UserDataManager = CS.UserDataManager.Instance
logic.cs.JsonDTManager = CS.JsonDTManager.Instance
logic.cs.PluginTools = CS.PluginTools.Instance
logic.cs.GameDataMgr = CS.GameDataMgr.Instance
logic.cs.GameMain = CS.GameFrameworkImpl.Instance
logic.cs.GameHttpNet = CS.GameHttpNet.Instance
logic.cs.talkingdata = CS.TalkingDataManager.Instance
logic.cs.IGGSDKMrg = CS.IGGSDKManager.Instance
logic.cs.IGGAgreementMrg = CS.IGGAgreementManager.Instance
logic.cs.GamePointManager = CS.GamePointManager.Instance
logic.cs.UIAlertMgr = CS.UIAlertMgr.Instance
logic.cs.AlertType = CS.AlertType
logic.cs.SdkMgr = CS.SdkMgr.Instance
logic.cs.CUIManager = CS.UGUI.CUIManager.Instance
logic.cs.UIFormName = CS.UIFormName
logic.cs.CUIID = CS.CUIID
logic.cs.EventEnum = CS.EventEnum
logic.cs.PurchaseManager = CS.PurchaseManager.Instance
logic.cs.PurchaseRecordManager = CS.PurchaseRecordManager.Instance
logic.cs.UINetLoadingMgr = CS.UINetLoadingMgr.Instance
logic.cs.EventDispatcher = CS.EventDispatcher
logic.cs.BookReadingWrapper = CS.BookReadingWrapper.Instance
logic.cs.UnityWebRequest = CS.UnityEngine.UnityWebRequest
logic.cs.CanvasScaler= CS.UnityEngine.UI.CanvasScaler
logic.cs.LuaHelper= CS.XLuaHelper
logic.cs.Image= CS.UnityEngine.UI.Image
logic.cs.ScrollRect= CS.UnityEngine.UI.ScrollRect
logic.cs.Button= CS.UnityEngine.UI.Button
logic.cs.Text= CS.UnityEngine.UI.Text
logic.cs.UIToggle = CS.GameCore.UI.UIToggle
logic.cs.UIToggleGroup = CS.GameCore.UI.UIToggleGroup
logic.cs.Dropdown = CS.UnityEngine.UI.Dropdown
logic.cs.InputField = CS.UnityEngine.UI.InputField
logic.cs.TextTyperAnimation = CS.UnityEngine.UI.TextTyperAnimation
logic.cs.GameUtility = CS.GameUtility
logic.cs.GameObject = CS.UnityEngine.GameObject
logic.cs.Object = CS.UnityEngine.Object
logic.cs.StringUtils = CS.StringUtils
logic.cs.UIEventListener = CS.UIEventListener
logic.cs.MyBooksDisINSTANCE = CS.MyBooksDisINSTANCE.Instance
logic.cs.TalkingDataManager = CS.TalkingDataManager.Instance
logic.cs.JsonHelper = CS.JsonHelper
logic.cs.AudioTones =
{
    book_click = 0,             --点击书本
    dialog_click = 1,           --点击对话的音效
    diamond_click = 2,          --点击钻石的音效
    dialog_choice_click = 3,    --选择选项的音效
    RewardWin = 4,          --成功音效
    LoseFail = 5,           --失败音效
    --RouletteSpin = 6,       --转盘启动的音效
}
logic.cs.CanvasGroup = CS.UnityEngine.CanvasGroup
logic.cs.UIDepth = CS.UIDepth
logic.cs.Handheld = CS.UnityEngine.Handheld
logic.cs.GridLayoutGroup = CS.UnityEngine.UI.GridLayoutGroup
logic.cs.UITweenButton = CS.GameCore.UGUI.UITweenButton
logic.cs.ABSystem = CS.AB.ABSystem
logic.cs.Input = CS.UnityEngine.Input
logic.cs.Screen = CS.UnityEngine.Screen
logic.cs.Material = CS.UnityEngine.Material
logic.cs.Shader = CS.UnityEngine.Shader
logic.cs.PlayerPrefs = CS.UnityEngine.PlayerPrefs
logic.cs.Camera = CS.UnityEngine.Camera
logic.cs.Transform = CS.UnityEngine.Transform
logic.cs.RectTransform = CS.UnityEngine.RectTransform
logic.cs.AudioSource = CS.UnityEngine.AudioSource
logic.cs.SkeletonGraphic = CS.Spine.Unity.SkeletonGraphic
logic.cs.BoneFollowerGraphic = CS.Spine.Unity.BoneFollowerGraphic
logic.cs.ResourceManager = CS.ResourceManager.Instance
logic.cs.HyperText = CS.Candlelight.UI.HyperText
logic.cs.UIBinding = CS.UIBinding
logic.cs.CUIUtility = CS.UGUI.CUIUtility
logic.cs.AbAtlas = CS.AB.AbAtlas
logic.cs.VerticalLayoutGroup = CS.UnityEngine.UI.VerticalLayoutGroup
logic.cs.HorizontalLayoutGroup = CS.UnityEngine.UI.HorizontalLayoutGroup
logic.cs.GridLayoutGroup = CS.UnityEngine.UI.GridLayoutGroup
logic.cs.EventSystem = CS.UnityEngine.EventSystems.EventSystem
logic.cs.CFileManager = CS.CFileManager
logic.cs.NativeGallery = CS.NativeGallery
logic.cs.LuaLayoutGroup = CS.LuaLayoutGroup
logic.cs.HashUtil = CS.AB.HashUtil
logic.cs.ResolutionAdapter = CS.ResolutionAdapter

logic.IsNull = IsNull
logic.debug = core.debug
logic.ResMgr = core.ResMgr

logic.config = require("Logic/Config/Config")
logic.loadingTips = require("Logic/Config/LoadingTips")
logic.hotfix = require("Logic/Hotfix/HotfixMain")
--logic.LogicUpdater = require("GameLogic/Main/LogicUpdater")
logic.IsNull = IsNull

--net
--logic.http = require("Logic/Net/Http")
logic.gameHttp = require("Logic/Net/GameHttp")

--ui
logic.UIMgr = core.UIMgr
logic.uiid = require("Logic/UI/Config/UUID")
logic.UIConfig = require("Logic/UI/Config/UIConfig")
logic.SortingLayerNames = require "Logic/Config/SortingLayerNames"
core.SortingLayerNames = logic.SortingLayerNames

logic.DataManager = require("Logic/DataCenter/DataManager")
--logic.ClientData = require("Logic/DataCenter/ClientData/ClientData")
--logic.ServerData = require("Logic/DataCenter/ServerData/ServerData")

--scenes
logic.BaseScene = require("Logic/Scenes/Base/BaseScene")
logic.sceneMgr = require("Logic/Scenes/Base/SceneManager")
logic.SceneConfig = require("Logic/Scenes/Config/SceneConfig")

--book reading
logic.bookReadingMgr = require("Logic/BookReading/BookReadingMgr")
logic.pb = require('Logic/DataCenter/ClientData/pb_define')

--story editor
logic.StoryEditorMgr = require("Logic/StoryEditor/StoryEditorMgr")
logic.EventDispatcher = core.Messenger.New()
logic.EventName =require("Logic/Messenger/EventName")

require("Logic/Cache/Cache")
Cache.Init();

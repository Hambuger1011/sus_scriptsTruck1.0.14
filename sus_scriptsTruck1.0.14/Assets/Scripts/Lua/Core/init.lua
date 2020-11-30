core = core or {}

-- 加载全局模块
core.Class = require("Core/Class/BaseClass")
core.ConstClass = require("Core/Class/ConstClass")
core.DataClass = require("Core/Class/DataClass")

core.Singleton = require("Core/Class/Singleton")
core.UpdatableSingleton = require("Core/Class/UpdatableSingleton")

--System
core.Stack = require("Core/System/Collections/Stack")

-- Common
core.table = {}
require("Core/Tools/Table/tostring")
require("Core/Tools/Table/TableUtil")
require "Core/Tools/LuaUtil"
require "Core/Tools/StringUtil"
require "Core/Tools/string"
require("Core/Global/GameHelper")
require("Core/Global/Enum")

core.list = list
core.Mathf		= Mathf
core.Vector2		= Vector2
core.Vector3 	= Vector3
core.Vector4		= Vector4
core.Quaternion	= Quaternion
core.Color		= Color
core.Ray			= Ray
core.Bounds		= Bounds
core.RaycastHit	= RaycastHit
core.Touch		= Touch
core.LayerMask	= LayerMask
core.Plane		= Plane
core.Time		= Time
--core.Object		= Object
core.coroutine		= coroutine

--config
core.config = require("Core/Config/Config")

--log
core.debug = require("Core/Logger/Logger")

--Messenger
core.Messenger = require("Core/Messenger/Messenger")

--update
-- core.Timer = require("Core/Updater/Timer")
-- core.TimerManager = require("Core/Updater/TimerManager")
-- core.UpdateManager = require("Core/Updater/UpdateManager")
-- core.Updatable = require("Core/Updater/Updatable")
-- require("Core/Updater/Coroutine")


--res
core.ResMgr = require("Core/Res/ResMgr")
--core.atlasMgr = require("Core/Res/AtlasMgr")
--core.GameObjectPool = require("Core/Res/GameObjectPool")

--ui
core.UIView = require("Core/UI/UIView")
core.UIMgr = require("Core/UI/UIManager")


--dotween
core.tween = {}
core.tween.DoTween = CS.DG.Tweening.DOTween
core.tween.Ease = --CS.DG.Tweening.Ease
{
    Unset = 0,
    Linear = 1,
    InSine = 2,
    OutSine = 3,
    InOutSine = 4,
    InQuad = 5,
    OutQuad = 6,
    InOutQuad = 7,
    InCubic = 8,
    OutCubic = 9,
    InOutCubic = 10,
    InQuart = 11,
    OutQuart = 12,
    InOutQuart = 13,
    InQuint = 14,
    OutQuint = 15,
    InOutQuint = 16,
    InExpo = 17,
    OutExpo = 18,
    InOutExpo = 19,
    InCirc = 20,
    OutCirc = 21,
    InOutCirc = 22,
    InElastic = 23,
    OutElastic = 24,
    InOutElastic = 25,
    InBack = 26,
    OutBack = 27,
    InOutBack = 28,
    InBounce = 29,
    OutBounce = 30,
    InOutBounce = 31,
    Flash = 32,
    InFlash = 33,
    OutFlash = 34,
    InOutFlash = 35,
    INTERNAL_Zero = 36,
    INTERNAL_Custom = 37
}
core.tween.Ease = CS.DG.Tweening.Ease
require("Core/System/init")

require("Core/Contol/GameController")
GameController.Init();

require("Core/Data/DataConfig")
DataConfig.InitConfig();



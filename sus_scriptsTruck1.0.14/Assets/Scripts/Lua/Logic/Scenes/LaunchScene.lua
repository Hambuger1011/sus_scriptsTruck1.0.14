--[[
-- 启动场景
--]]

local Class = core.Class
local BaseScene = logic.BaseScene
local LaunchScene = Class("LaunchScene", BaseScene)
local base = BaseScene

-- 创建：准备预加载资源
function LaunchScene:OnEnter()
    base.OnEnter(self)

    
	logic.UIMgr:Startup(logic.UIConfig)
	--启动热更新补丁
    logic.hotfix.Start()
    logic.gameHttp:SetUrlHead()
    
    logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.PopupTipsForm)
    logic.cs.UITipsMgr = CS.UITipsMgr.Instance
    logic.sceneMgr:GotoScene(logic.SceneConfig.LoadingScene)
end

-- 离开场景
function LaunchScene:OnLeave()
	base.OnLeave(self)
end

return LaunchScene
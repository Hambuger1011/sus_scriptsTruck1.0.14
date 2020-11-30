--[[
-- Book加载场景
--]]

local Class = core.Class
local BaseScene = logic.BaseScene
local BookLoadingScene = Class("BookLoadingScene", BaseScene)
local base = BaseScene

-- 创建：准备预加载资源
function BookLoadingScene:OnEnter()
    base.OnEnter(self)
    --logic.sceneMgr:GotoScene(logic.SceneConfig.LoadingScene)
    logic.UIMgr:Open(logic.uiid.BookLoading)
end

-- 离开场景
function BookLoadingScene:OnLeave()
	base.OnLeave(self)
end

return BookLoadingScene
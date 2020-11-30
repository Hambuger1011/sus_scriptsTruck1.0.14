--[[
-- added by wsh 2017-12-29
-- 游戏热修复入口
--https://github.com/Tencent/xLua/blob/master/Assets/XLua/Doc/hotfix.md
--]]

local HotfixMain = {}
local this = HotfixMain

-- 需要被加载的热修复模块
HotfixMain.modules = {
	--"XLua.Hotfix.HotfixTest",
	--'Logic/Hotfix/UserDataManager',
	--'Logic/Hotfix/UI/MainForm',
	--'Logic/Hotfix/XLuaManager',
	-- 'Logic/Hotfix/Onyx/GameHttpNet',
	--'Logic/Hotfix/Onyx/UniHttp',
	--'Logic/Hotfix/Onyx/SdkMgr',
}

function HotfixMain.Start()
	if logic.config.isEditorMode then
		return
	end
	print("HotfixMain start...")
	for _,v in ipairs(this.modules) do
		local hotfix_module = reimport(v)
		hotfix_module.Register()
	end
	--CS.XLuaManager.Instance:TestHotfix()
end

function HotfixMain.Stop()
	print("HotfixMain stop...")
	for _,v in ipairs(this.modules) do
		local hotfix_module = require(v)
		hotfix_module.Unregister()
	end
end
return HotfixMain
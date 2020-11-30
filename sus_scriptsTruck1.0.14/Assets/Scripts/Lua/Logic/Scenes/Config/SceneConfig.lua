--[[
-- added by wsh @ 2017-11-15
-- 场景配置
-- 注意：
-- 1、level、name决定加载哪个物理场景
-- 2、Type决定加载哪个逻辑场景，多个逻辑场景可以使用同一个物理场景
--]]

local SceneConfig = {
	-- 启动场景
	LaunchScene = {
		Name = "LaunchScene",
		Type = require "Logic/Scenes/LaunchScene",
	},
	LoadingScene = {
		Name = "LoadingScene",
		Type = require "Logic/Scenes/LoadingScene",
	},
}

return SceneConfig--ConstClass("SceneConfig", SceneConfig)
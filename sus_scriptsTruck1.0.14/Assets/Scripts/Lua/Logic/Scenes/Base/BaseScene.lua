--[[
-- added by wsh @ 2017-12-15
-- 场景基类，各场景类从这里继承：提供统一的场景加载和初始化步骤，负责资源预加载
--]]
local BaseClass = core.Class
local BaseScene = BaseClass("BaseScene")

-- 构造函数，别重写，初始化放OnInit
function BaseScene:SetConfig(scene_config)
	-- 场景配置
	self.scene_config = scene_config
	self.isOverlap = false
end

-- 析构函数，别重写，资源释放放OnDispose
function BaseScene:__delete(self)
	self:OnDestroy()
end


-- 销毁：释放全局保存的状态
function BaseScene:OnDestroy(self)
	self.scene_config = nil
	self.preload_resources = nil
end

-- 加载前的初始化
function BaseScene:OnEnter(self)
end

-- 离开场景：清理场景资源
function BaseScene:OnLeave(self)
end

-- 场景被覆盖
function BaseScene:OnOverlap(self)
	self.isOverlap = true
end

-- 场景被置顶
function BaseScene:OnTop(self)
	self.isOverlap = false
end

return BaseScene
local Class = core.Class
local target = CS.SdkMgr
local SdkMgr = Class('SdkMgr')
local util = require("3rd/xlua/common/util")

function SdkMgr:Register()
	logic.cs.SdkMgr:Start()
	xlua.hotfix(target, "Start", SdkMgr.Start)
end

function SdkMgr:Unregister()
	xlua.hotfix(target, "Start", nil)
end

function SdkMgr.Start(self)
end

return SdkMgr
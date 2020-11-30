local Class = core.Class
local target = CS.OnyxAdsSdk
local OnyxAdsSdk = Class('OnyxAdsSdk')
local util = require("3rd/xlua/common/util")

function OnyxAdsSdk:Register()
	logic.cs.OnyxAdsSdk:Start()
	xlua.hotfix(target, "onRewardedVideoAdPlayClosed", OnyxAdsSdk.onRewardedVideoAdPlayClosed)
end

function OnyxAdsSdk:Unregister()
	xlua.hotfix(target, "onRewardedVideoAdPlayClosed", nil)
end

function OnyxAdsSdk.onRewardedVideoAdPlayClosed(self, unitId, isReward, callbackInfo)
    core.coroutine.start(function()
        if isReward then
            logic.debug.Log(string.format('onRewardedVideoAdPlayClosed:%s,%d',unitId, isReward))
        else
            self:CallRewardBasedVideoEvent(true)
        end
    end)
end

return OnyxAdsSdk
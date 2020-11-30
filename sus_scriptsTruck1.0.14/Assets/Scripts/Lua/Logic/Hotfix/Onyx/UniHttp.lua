local Class = core.Class
local target = CS.UniHttp
local UniHttp = Class('UniHttp')
local util = require("3rd/xlua/common/util")

xlua.private_accessible(target) --开启私有成员访问权限

function UniHttp:Register()
	util.hotfix_ex(target, "DoWebPost", UniHttp.DoWebPost)
	util.hotfix_ex(target, "DoWebGet", UniHttp.DoWebGet)
end

function UniHttp:Unregister()
	util.hotfix_ex(target, "DoWebPost", nil)
	util.hotfix_ex(target, "DoWebGet", nil)
end

local autoHideLoadingTimer = nil
local autoHideLoading = function(httpObject)
    if httpObject.isMask ~= nil and httpObject.isMask == false then
        httpObject.isMask = true
        logic.cs.UINetLoadingMgr:Show()
    end
    --logic.debug.LogError('get')
    if autoHideLoadingTimer then
        autoHideLoadingTimer:Stop()
    end
    autoHideLoadingTimer = core.Timer.New(function()
        logic.cs.CUIManager:CloseForm(logic.cs.UIFormName.NetLoadingForm)
    end,10)
    autoHideLoadingTimer:Start()
end

function UniHttp.DoWebPost(self,httpObject, tryCount, inThread)
    self:DoWebPost(httpObject, tryCount, inThread)
    autoHideLoading(httpObject)
end

function UniHttp.DoWebGet(self,httpObject, tryCount)
    self:DoWebGet(httpObject, tryCount)
    autoHideLoading(httpObject)
end

return UniHttp
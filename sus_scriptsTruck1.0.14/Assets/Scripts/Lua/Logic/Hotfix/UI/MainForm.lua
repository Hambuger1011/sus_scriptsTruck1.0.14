local Class = core.Class
local target = CS.MainForm
local MainForm = Class('MainForm')

xlua.private_accessible(target)

local OnOpen = function(this)
    logic.debug.LogError('fuck')
end

function MainForm:Register()
	xlua.hotfix(target, "OnOpen", OnOpen)
	--util.hotfix_ex(AssetBundleManager, "TestHotfix", AssetBundleManagerTestHotfix)
end

function MainForm:Unregister()
	xlua.hotfix(target, "OnOpen", nil)
	--xlua.hotfix(target, "TestHotfix", nil)
end




return MainForm
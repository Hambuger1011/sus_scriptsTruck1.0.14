--[[
    删除书本
]]
local BaseClass = core.Class

---@class UIStory_DeleteBookView
local UIDeleteBookView = BaseClass("UIDeleteBookView")

function UIDeleteBookView:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.uiBinding = gameObject:GetComponent(typeof(CS.UIBinding))

    self.btnCancel = self.uiBinding:Get('btnCancel', typeof(logic.cs.UITweenButton))
    self.btnSure = self.uiBinding:Get('btnSure', typeof(logic.cs.UITweenButton))

    self.btnCancel.onClick:AddListener(function()
        self:Hide()
        self.callback(false)
    end)
    self.btnSure.onClick:AddListener(function()
        self:Hide()
        self.callback(true)
    end)
end

function UIDeleteBookView:Show(callback)
    self.isActive = true
    self.gameObject:SetActiveEx(true)
    self.callback = callback
end
function UIDeleteBookView:Hide()
    self.isActive = false
    self.gameObject:SetActiveEx(false)
end

return UIDeleteBookView
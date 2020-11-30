--[[
    帮助菜单
]]
local BaseClass = core.Class

---@class UIStory_AddMenuView
local UIMoreMenuView = BaseClass("UIMoreMenuView")

function UIMoreMenuView:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.uiBinding = gameObject:GetComponent(typeof(CS.UIBinding))

    self.btnDelete = self.uiBinding:Get('btnDelete', typeof(logic.cs.UITweenButton))
    self.btnFaq = self.uiBinding:Get('btnFaq', typeof(logic.cs.UITweenButton))

    self.btnDelete.onClick:AddListener(function()
        self:OnDeleteClick()
    end)
    self.btnFaq.onClick:AddListener(function()
        self:OnFaqClick()
    end)

    
    local button = gameObject:GetComponent(typeof(logic.cs.Button))
    button.onClick:AddListener(function()
        self:Hide()
    end)
end

function UIMoreMenuView:Show()
    self.isActive = true
    self.gameObject:SetActiveEx(true)
    self.btnDelete.gameObject:SetActiveEx(not self.isReadonly)
end
function UIMoreMenuView:Hide()
    self.isActive = false
    self.gameObject:SetActiveEx(false)
end

function UIMoreMenuView:OnDeleteClick()
    self:Hide()
    logic.EventDispatcher:Broadcast(logic.EventName.on_story_delete_book)
end

function UIMoreMenuView:OnFaqClick()
    self:Hide()
    logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.FAQ);
end

return UIMoreMenuView
local BaseClass = core.Class

---@class UIStory_AddMenuView
local UIMoreMenuView = BaseClass("UIMoreMenuView")

function UIMoreMenuView:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.uiBinding = gameObject:GetComponent(typeof(CS.UIBinding))

    self.btnPreview = self.uiBinding:Get('btnPreview', typeof(logic.cs.UITweenButton))
    self.btnSave = self.uiBinding:Get('btnSave', typeof(logic.cs.UITweenButton))

    self.btnPreview.onClick:AddListener(function()
        self:OnPreviewClick()
    end)
    self.btnSave.onClick:AddListener(function()
        self:OnSaveClick()
    end)
    local button = gameObject:GetComponent(typeof(logic.cs.Button))
    button.onClick:AddListener(function()
        self:Hide()
    end)
end

function UIMoreMenuView:Show()
    self.isActive = true
    self.gameObject:SetActiveEx(true)
end
function UIMoreMenuView:Hide()
    self.isActive = false
    self.gameObject:SetActiveEx(false)
end

function UIMoreMenuView:OnPreviewClick()
    self:Hide()
    logic.EventDispatcher:Broadcast(logic.EventName.on_story_preview_click)
end

function UIMoreMenuView:OnSaveClick()
    self:Hide()
    logic.EventDispatcher:Broadcast(logic.EventName.on_story_chapter_save_click)
end

return UIMoreMenuView
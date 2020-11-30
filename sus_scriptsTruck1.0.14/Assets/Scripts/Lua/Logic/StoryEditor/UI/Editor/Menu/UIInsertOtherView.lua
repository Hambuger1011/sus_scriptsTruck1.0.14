local BaseClass = core.Class

---@class UIStory_InsertDialogView
local UIInsertOtherView = BaseClass("UIInsertOtherView")
UIInsertOtherView.ModifyType = {
    Modify = 0,
    Insert = 1,
    Append = 2,
}

function UIInsertOtherView:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.uiBinding = gameObject:GetComponent(typeof(CS.UIBinding))
    self.btnDialog = self.uiBinding:Get('btnDialog', typeof(logic.cs.UITweenButton))
    self.btnOption = self.uiBinding:Get('btnOption', typeof(logic.cs.UITweenButton))
    self.btnImage = self.uiBinding:Get('btnImage', typeof(logic.cs.UITweenButton))
    self.btnMenu = self.uiBinding:Get('panel_menu',typeof(logic.cs.Button))

    self.btnMenu.onClick:AddListener(function()
        self:Hide()
    end)
    self.btnDialog.onClick:AddListener(function()
        self:Hide()
        logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_dialog_append_modify, self.refItem, self.modifyType)
    end)
    self.btnOption.onClick:AddListener(function()
        self:Hide()
        local index = self.refItem.itemData.index + 1
        if self.modifyType == UIInsertOtherView.ModifyType.Insert then
            index = self.refItem.itemData.index
        end
        logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_dialog_append_option,index)
    end)
    self.btnImage.onClick:AddListener(function()
        self:Hide()
        local index = self.refItem.itemData.index + 1
        if self.modifyType == UIInsertOtherView.ModifyType.Insert then
            index = self.refItem.itemData.index
        end
        logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_pickimage, index)
    end)
    
    --logic.EventDispatcher:AddListener(logic.EventName.on_story_editor_dialog_modify,self.OnDialogModify,self)
    logic.EventDispatcher:AddListener(logic.EventName.on_story_editor_dialog_insert,self.OnDialogInsert,self)
    logic.EventDispatcher:AddListener(logic.EventName.on_story_editor_dialog_append,self.OnDialogAppend,self)
end

function UIInsertOtherView:__delete()
    --logic.EventDispatcher:RemoveListener(logic.EventName.on_story_editor_dialog_modify,self.OnDialogModify,self)
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_editor_dialog_insert,self.OnDialogInsert,self)
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_editor_dialog_append,self.OnDialogAppend,self)
end

function UIInsertOtherView:Show(modifyType)
    self.gameObject:SetActiveEx(true)
    self.deactiveObj:SetActiveEx(false)
    self.modifyType = modifyType
end
function UIInsertOtherView:Hide()
    self.gameObject:SetActiveEx(false)
    self.deactiveObj:SetActiveEx(true)
end

function UIInsertOtherView:SetData(deactiveObj)
    
    self.deactiveObj = deactiveObj
end

function UIInsertOtherView:RefreshUI()
end


---@param uiItem UIBubbleItem
function UIInsertOtherView:OnDialogModify(uiItem)
    self.refItem = uiItem
    self:Show(UIInsertOtherView.ModifyType.Modify)
end

---@param uiItem UIBubbleItem
function UIInsertOtherView:OnDialogInsert(uiItem)
    self.refItem = uiItem
    self:Show(UIInsertOtherView.ModifyType.Insert)
end

---@param uiItem UIBubbleItem
function UIInsertOtherView:OnDialogAppend(uiItem)
    self.refItem = uiItem
    self:Show(UIInsertOtherView.ModifyType.Append)
end

return UIInsertOtherView
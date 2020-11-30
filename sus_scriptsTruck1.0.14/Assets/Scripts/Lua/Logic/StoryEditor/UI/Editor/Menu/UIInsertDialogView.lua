local BaseClass = core.Class

---@class UIStory_InsertDialogView
local UIInsertDialogView = BaseClass("UIInsertDialogView")
local UIRoleItem = require('Logic/StoryEditor/UI/Editor/UIRoleItem')

local ModifyType = {
    Modify = 0,
    Insert = 1,
    Append = 2,
}

function UIInsertDialogView:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.uiBinding = gameObject:GetComponent(typeof(CS.UIBinding))
    self.btnClose = self.uiBinding:Get('btnClose', typeof(logic.cs.UITweenButton))
    self.btnSend = self.uiBinding:Get('btnSend', typeof(logic.cs.UITweenButton))
    self.inMsg = self.uiBinding:Get('inMsg', typeof(logic.cs.InputField))

    
    self.itemPfb = self.uiBinding:Get('itemPfb')
    self.itemPfb:SetActiveEx(false)
    self.itemRoot = self.uiBinding:Get('itemRoot').transform

    self.btnClose.onClick:AddListener(function()
        self:Hide()
    end)
    self.btnSend.onClick:AddListener(function()
        self:Hide()
        local roleData = self.activeRoleData
        if not roleData then
            logic.cs.UITipsMgr:PopupTips('Please Select Role', false)
            return
        end
        local strMsg = self.inMsg.text
        logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_dialog_modify, self.refItem,self.modifyType, strMsg, roleData)
    end)
    
    logic.EventDispatcher:AddListener(logic.EventName.on_story_editor_dialog_append_modify,self.OnDialogModify,self)
end

function UIInsertDialogView:__delete()
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_editor_dialog_append_modify,self.OnDialogModify,self)
end

function UIInsertDialogView:Show(modifyType)
    self.gameObject:SetActiveEx(true)
    self.modifyType = modifyType
    if modifyType == ModifyType.Modify then
        ---@type t_StoryItem
        local storyItem = self.refItem.bubbleData.storyNode.storyItem
        self.inMsg.text = storyItem.text
    else
        self.inMsg.text = ''
    end
    self:InitRoleList()
end
function UIInsertDialogView:Hide()
    self.gameObject:SetActiveEx(false)
end


---@param uiItem UIBubbleItem
function UIInsertDialogView:OnDialogModify(uiItem, modifyType)
    self.refItem = uiItem
    self:Show(modifyType)
end



function UIInsertDialogView:InitRoleList()
    self.uiRoleHeads = self.uiRoleHeads or {}
    local storyDetial = self.refItem.storyDetial
    local roles = storyDetial.roleTable.roles
    local storyItem = self.refItem.bubbleData.storyNode.storyItem
    self.activeRoleData = roles:Get(storyItem.roleID)
    local count = roles:Count()
    for i=#self.uiRoleHeads, count do
        local go = logic.cs.GameObject.Instantiate(self.itemPfb,self.itemRoot)
        go.name = tostring(i)
        --go:SetActiveEx(true)
        local uiItem = UIRoleItem.New(go)
        table.insert(self.uiRoleHeads, uiItem)
    end
    
    roles:Foreach(function(i,kv)
        local uiItem = self.uiRoleHeads[i]
        local isOn = (i <= count)
        if isOn then
            uiItem.gameObject:SetActiveEx(true)
            uiItem:SetData(kv.Value)
            uiItem:SetOn(self.activeRoleData == uiItem.roleData)
            uiItem.onValueChanged = function(item,isOn)
                self:OnRoleHeadSelect(item,isOn)
            end
        else
            uiItem.gameObject:SetActiveEx(false)
        end
    end)
end


---@param roleHeadItem UIRoleItem
function UIInsertDialogView:OnRoleHeadSelect(roleHeadItem, isOn)
    if isOn then
        self.activeRoleData = roleHeadItem.roleData
        logic.EventDispatcher:Broadcast(logic.EventName.on_story_role_select, roleHeadItem.roleData)
    end
    
    for i = 1, #self.uiRoleHeads do
        local uiItem = self.uiRoleHeads[i]
        uiItem:SetOn(uiItem == roleHeadItem)
    end
end

return UIInsertDialogView
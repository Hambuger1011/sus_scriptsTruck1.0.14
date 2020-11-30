local BaseClass = core.Class

---@class UIStory_RoleView
local UIRoleView = BaseClass("UIRoleView")
local UIRoleItem = require('Logic/StoryEditor/UI/Editor/UIRoleItem')

function UIRoleView:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.uiBinding = gameObject:GetComponent(typeof(CS.UIBinding))
    self.itemPfb = self.uiBinding:Get('itemPfb')
    self.itemPfb:SetActiveEx(false)
    self.itemRoot = self.uiBinding:Get('itemRoot').transform
    self.btnAddRole = self.uiBinding:Get('btnAddRole',typeof(logic.cs.UITweenButton))
    self.btnAddRole2 = self.uiBinding:Get('btnAddRole2',typeof(logic.cs.UITweenButton))

    self.btnAddRole.onClick:AddListener(function()
        self:OnAddRoleClick()
    end)
    self.btnAddRole2.onClick:AddListener(function()
        self:OnAddRoleClick()
    end)
    logic.EventDispatcher:AddListener(logic.EventName.on_story_role_change,self.OnRoleChange,self)
    logic.EventDispatcher:AddListener(logic.EventName.on_story_role_select,self.OnRoleSelect,self)
end

function UIRoleView:__delete()
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_role_change,self.OnRoleChange,self)
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_role_select,self.OnRoleSelect,self)
end


---@param storyDetial StoryEditor_BookDetials
---@param chapterData StoryEditor_ChapterDetial
---@param storyTable t_StoryTable
function UIRoleView:SetData(storyDetial, chapterData,storyTable)
    ---@type StoryEditor_BookDetials
    self.storyDetial = storyDetial
    ---@type StoryEditor_ChapterDetial
    self.chapterData = chapterData
    ---@type t_StoryTable
    self.storyTable = storyTable
    self.activeRoleData = nil
    self:InitRoleList()
end


function UIRoleView:InitRoleList()
    self.uiRoleHeads = self.uiRoleHeads or {}
    local roles = self.storyDetial.roleTable.roles
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
                self:OnRoleHeadSelect(item.roleData,isOn)
            end
        else
            uiItem.gameObject:SetActiveEx(false)
        end
    end)

    if self.activeRoleData == nil then
        self.activeRoleData = roles:Get(0)
    end

    if count <= 1 then
        self.itemRoot.gameObject:SetActiveEx(false)
        self.btnAddRole2.gameObject:SetActiveEx(true)
    else
        self.btnAddRole.transform:SetAsLastSibling()
        self.itemRoot.gameObject:SetActiveEx(true)
        self.btnAddRole2.gameObject:SetActiveEx(false)
        self:OnRoleSelect(self.activeRoleData,true)
    end
end

---@param roleHeadItem UIRoleItem
function UIRoleView:OnRoleHeadSelect(roleData, isOn)
    if isOn then
        self.activeRoleData = roleData
    end
    
    for i = 1, #self.uiRoleHeads do
        local uiItem = self.uiRoleHeads[i]
        uiItem:SetOn(uiItem.roleData == roleData)
    end
end

function UIRoleView:OnRoleSelect(roleData)
    self.activeRoleData = roleData
    for i,uiItem in pairs(self.uiRoleHeads) do
        local isOn = (roleData ~= nil and uiItem.roleData == roleData)
        uiItem:SetOn(isOn)
    end
end

function UIRoleView:OnAddRoleClick()
    logic.EventDispatcher:Broadcast(logic.EventName.on_story_role_new,false)
end

function UIRoleView:OnRoleChange()
    self:InitRoleList()
    logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_dialog_list_refresh)
end

return UIRoleView
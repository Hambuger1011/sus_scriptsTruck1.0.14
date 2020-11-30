local BaseClass = core.Class

---@class UIStory_AddRoleView
local UIAddRoleView = BaseClass("UIAddRoleView")
local UIAddRoleView_Item = require('Logic/StoryEditor/UI/Editor/UIAddRoleView_Item')

function UIAddRoleView:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.uiBinding = gameObject:GetComponent(typeof(CS.UIBinding))
    self.btnClose = self.uiBinding:Get('btnClose', typeof(logic.cs.UITweenButton))
    self.btnOK = self.uiBinding:Get('btnOK', typeof(logic.cs.UITweenButton))

    self.inName = self.uiBinding:Get('inName', typeof(logic.cs.InputField))
    self.lbRoleNumber = self.uiBinding:Get('lbRoleNumber', typeof(logic.cs.Text))
    self.inName.onValueChanged:AddListener(function(text)
        local value = string.trim(text)
        local len = string.GetUtf8Len(value)
        self.lbRoleNumber.text = string.format("%d/20",len)
    end)

    self.isLeadRole = self.uiBinding:Get('isLeadRole',typeof(logic.cs.UIToggle))
    self.isSupportingRole = self.uiBinding:Get('isSupportingRole',typeof(logic.cs.UIToggle))
    self.isLeadRole.isOn = true
    self.isSupportingRole.isOn = false

    self.isMale = self.uiBinding:Get('isMale',typeof(logic.cs.UIToggle))
    self.isFemale = self.uiBinding:Get('isFemale',typeof(logic.cs.UIToggle))
    self.isMale.isOn = false
    self.isFemale.isOn = true
    self.isMale.onValueChanged:AddListener(function()
        self:InitRoleList()
    end)

    self.roleHeadRoot = self.uiBinding:Get('roleHeadRoot').transform
    self.roleHeadPfb = self.uiBinding:Get('roleHeadPfb')
    self.roleHeadPfb:SetActiveEx(false)

    self.btnClose.onClick:AddListener(function()
        self:OnCloseClick()
    end)
    self.btnOK.onClick:AddListener(function()
        self:OnOKClick()
    end)
    self.root = self.uiBinding:Get('root').transform
    self.originRootPos = self.root.anchoredPosition
end

function UIAddRoleView:Show()
    self.isActive = true
    self.gameObject:SetActiveEx(true)

    core.tween.DoTween.Kill(self)
    local p = self.originRootPos
    self.root.anchoredPosition = core.Vector2.New(p.x + self.root.rect.size.x, p.y)
    self.root:DOAnchorPosX(p.x,0.5):SetEase(core.tween.Ease.Flash):SetId(self)
end
function UIAddRoleView:Hide(notUseTween)
    self.isActive = false
    
    if notUseTween then
        self.gameObject:SetActiveEx(false)
    else
        core.tween.DoTween.Kill(self)
        local p = self.originRootPos
        --self.root.anchoredPosition = core.Vector2.New(p.x, p.y - self.root.rect.size.y):SetId(self)
        self.root:DOAnchorPosX(p.y + self.root.rect.size.x,0.5):SetEase(core.tween.Ease.Flash):SetId(self):OnComplete(function()
            self.gameObject:SetActiveEx(false)
        end)
    end
end

---@param storyDetial StoryEditor_BookDetials
---@param storyTable t_StoryTable
---@param roleData t_Role
function UIAddRoleView:SetData(storyDetial, storyTable, roleData)
    if not roleData then
        ---@type t_Role
        roleData= logic.StoryEditorMgr.DataDefine.t_Role.New()
        roleData.id = 0
        roleData.name = ''
        self.type = logic.StoryEditorMgr.DataDefine.EBubbleType.SupportingRole
        -- storyDetial.desc = 'Test...'
        -- storyDetial.date = os.time()
    end
    self.storyDetial = storyDetial
    self.roleData = roleData
    self.storyTable = storyTable
    if roleData.icon < 1 then
        roleData.icon = 1
    end
    self.roleHeadIndex = roleData.icon
    self:RefreshUI()
end

function UIAddRoleView:RefreshUI()
    self.inName.text = self.roleData.name
    if self.roleData.type == logic.StoryEditorMgr.DataDefine.EBubbleType.LeadRole then
        self.isLeadRole.isOn = true
        self.isSupportingRole.isOn = false
    else
        self.isLeadRole.isOn = false
        self.isSupportingRole.isOn = true
    end
    self.isMale.isOn = self.roleData.isMale
    self.isFemale.isOn = not self.roleData.isMale
    self:InitRoleList()
end


function UIAddRoleView:InitRoleList()
    ---@type UIAddRoleView_Item[]
    self.uiRoleHeads = self.uiRoleHeads or {}
    if self.roleHeadData == nil then
        self.roleHeadData = {100,100}
    end
    local isMale = self.isMale.isOn
    local idx = (self.isMale.isOn and 2 or 1)
    local count = self.roleHeadData[idx]
    for i=#self.uiRoleHeads, count do
        local go = logic.cs.GameObject.Instantiate(self.roleHeadPfb, self.roleHeadRoot)
        --go:SetActiveEx(true)
        local uiItem = UIAddRoleView_Item.New(go)
        table.insert(self.uiRoleHeads, uiItem)
    end
    for i = 1, #self.uiRoleHeads do
        local uiItem = self.uiRoleHeads[i]
        local isOn = (i <= count)
        if isOn then
            uiItem.gameObject:SetActiveEx(true)
            uiItem:SetData(i,isMale)
            uiItem:SetOn(self.roleHeadIndex == i)
            uiItem.onValueChanged = function(item,isOn)
                self:OnRoleHeadSelect(item,isOn)
            end
        else
            uiItem.gameObject:SetActiveEx(false)
            uiItem.onValueChanged = nil
        end
    end
end


---@param roleHeadItem UIAddRoleView_Item
function UIAddRoleView:OnRoleHeadSelect(roleHeadItem, isOn)
    if isOn then
        self.roleHeadIndex = roleHeadItem.index
    end
    
    for i = 1, #self.uiRoleHeads do
        local uiItem = self.uiRoleHeads[i]
        uiItem:SetOn(uiItem == roleHeadItem)
    end
end

function UIAddRoleView:OnCloseClick()
    self:Hide()
end

function UIAddRoleView:OnOKClick()
    local len = 0
    local roleName = string.trim(self.inName.text)
    len = string.GetUtf8Len(roleName)
    if len < 1 or len > 20 then
        logic.debug.LogError(roleName)
        logic.cs.UITipsMgr:PopupTips("Enter a 1-20 words description.", false)
        return
    end
    self.roleData.name = roleName
    local roleTable = self.storyDetial.roleTable

    if self.isLeadRole.isOn then
        self.roleData.type = logic.StoryEditorMgr.DataDefine.EBubbleType.LeadRole
    else
        self.roleData.type = logic.StoryEditorMgr.DataDefine.EBubbleType.SupportingRole
    end
    self.roleData.isMale = self.isMale.isOn

    if self.roleData.id == 0 then
        roleTable:AddRole(self.roleData)
    end
    self.roleData.icon = self.roleHeadIndex

    --修改后乱序，重新插入
    local roles = roleTable.roles
    roles:Remove(self.roleData.id)
    roles:Set(self.roleData.id,self.roleData)

    logic.EventDispatcher:Broadcast(logic.EventName.on_story_role_change)
    self:Hide()
end


return UIAddRoleView
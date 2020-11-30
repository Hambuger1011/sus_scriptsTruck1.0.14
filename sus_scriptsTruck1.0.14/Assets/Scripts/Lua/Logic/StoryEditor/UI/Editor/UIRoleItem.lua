local BaseClass = core.Class

---@class UIStory_UIRoleItem
local UIRoleItem = BaseClass("UIRoleItem")

function UIRoleItem:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform

    self.button = gameObject:GetComponent(typeof(logic.cs.UITweenButton))

    self.uiBinding = self.gameObject:GetComponent(typeof(logic.cs.UIBinding))
    self.imgIcon = self.uiBinding:Get('imgIcon',typeof(logic.cs.Image))
    self.imgSelected = self.uiBinding:Get('imgSelected',typeof(logic.cs.Image))
    self.lbName = self.uiBinding:Get('lbName',typeof(logic.cs.Text))
    --self.goSelect = self.transform:Find('select').gameObject
    self.button.onClick:AddListener(function()
        self:SwitchOn(not self.isOn)
    end)
    self.isOn = false
    self.atlas = logic.cs.LuaHelper.GetAtlas(
        logic.ResMgr.tag.StoryEditor,
        logic.ResMgr.type.Atlas,
        'UI/StoryEditorRes/RoleHead'
        )
    self.oldNameColor = self.lbName.color
end

---@param roleData t_Role
function UIRoleItem:SetData(roleData)
    self.roleData = roleData
    self.lbName.text = roleData.name
    if roleData.id == 0 then
        self.imgIcon.sprite = self.atlas:GetSprite('btn_selected')
        --self.imgSelected.sprite = self.atlas:GetSprite('btn_selected')
    else
        local headName = roleData:GetIconName()
        self.imgIcon.sprite = self.atlas:GetSprite(headName)
        --self.imgSelected.sprite = self.atlas:GetSprite('bg_frame')
    end
end

function UIRoleItem:SetOn(isOn)
    self.isOn = isOn
    -- if self.roleData and self.roleData.id == 0 then
    --     self.goSelect:SetActiveEx(isOn)
    --     --self.imgIcon.gameObject:SetActiveEx(not isOn)
    -- else
    --     self.imgIcon.gameObject:SetActiveEx(true)
    --     --self.goSelect:SetActiveEx(isOn)
    -- end
    self.imgSelected.color = logic.cs.LuaHelper.ParseHtmlString((isOn and '#fa4764') or '#e6e6e6')
    self.lbName.color = ((isOn and logic.cs.LuaHelper.ParseHtmlString('#fa4764')) or self.oldNameColor)
end

function UIRoleItem:SwitchOn(isOn)
    if self.onValueChanged then
        self.onValueChanged(self,isOn)
    end
end

return UIRoleItem

local BaseClass = core.Class
---@class UIAddRoleView_Item
local UIAddRoleView_Item = BaseClass("UIAddRoleView_Item")

function UIAddRoleView_Item:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    
    self.uiBinding = self.gameObject:GetComponent(typeof(logic.cs.UIBinding))
    self.button = self.gameObject:GetComponent(typeof(logic.cs.UITweenButton))
    self.button.onClick:AddListener(function()
        self:SwitchOn(not self.isOn)
    end)
    self.isOn = false
    self.imgIcon = self.uiBinding:Get('imgIcon',typeof(logic.cs.Image))
    self.imgSelected = self.uiBinding:Get('selectedGo',typeof(logic.cs.Image))
    self.selectedGo = self.uiBinding:Get('selectedGo')
    --self.selectedGo:SetActiveEx(false)
    self.atlas = logic.cs.LuaHelper.GetAtlas(
        logic.ResMgr.tag.StoryEditor,
        logic.ResMgr.type.Atlas,
        'UI/StoryEditorRes/RoleHead'
        )
end

function UIAddRoleView_Item:__delete()
    if not logic.IsNull(self.gameObject) then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject = nil
    self.transform = nil
    self.button = nil
end

function UIAddRoleView_Item:SetData(index,isMale)
    self.index = index
    self.isMale = isMale
    self:RefresUI()
end

function UIAddRoleView_Item:RefresUI()
    local headName = string.format('bg_avatar_female_%.2d',self.index)
    if self.isMale then
        headName = string.format('bg_avatar_male_%.2d',self.index)
    end
    self.imgIcon.sprite = self.atlas:GetSprite(headName)
end


function UIAddRoleView_Item:SwitchOn(isOn)
    if self.onValueChanged then
        self.onValueChanged(self,isOn)
    end
end

function UIAddRoleView_Item:SetOn(isOn)
    self.isOn = isOn
    --self.selectedGo:SetActiveEx(isOn)
    self.imgSelected.color = logic.cs.LuaHelper.ParseHtmlString(isOn and '#fa4764' or '#e6e6e6')
end

return UIAddRoleView_Item
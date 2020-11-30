
local BaseClass = core.Class
---@class UICatagoryItem
local UICatagoryItem = BaseClass("UICatagoryItem")

function UICatagoryItem:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    
    self.uiBinding = self.gameObject:GetComponent(typeof(logic.cs.UIBinding))
    self.name1 = self.uiBinding:Get('name1',typeof(logic.cs.Text))
    self.name2 = self.uiBinding:Get('name2',typeof(logic.cs.Text))
    self.selectedGo = self.uiBinding:Get('selectedGo')
    self.button = self.gameObject:GetComponent(typeof(logic.cs.UITweenButton))
    self.button.onClick:AddListener(function()
        self:SwitchOn(not self.isOn)
    end)
    self.isOn = false
end

function UICatagoryItem:__delete()
    if not logic.IsNull(self.gameObject) then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject = nil
    self.transform = nil
    self.button = nil
end

function UICatagoryItem:SetData(index)
    self.index = index
    self:RefresUI()
end

function UICatagoryItem:RefresUI()
    local DataDefine = logic.StoryEditorMgr.DataDefine
    local name = DataDefine.BookTags[self.index]
    self.name1.text = name
    self.name2.text = name
end

function UICatagoryItem:SwitchOn(isOn)
    if self.onValueChanged then
        self.onValueChanged(self,isOn)
    end
end

function UICatagoryItem:SetOn(isOn)
    self.isOn = isOn
    self.selectedGo:SetActiveEx(isOn)
end

return UICatagoryItem
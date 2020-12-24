---@class PakageItem
local UIBubbleBox = require('Logic/StoryEditor/UI/Editor/BubbleItem/UIBubbleBox')
local PakageItem  = core.Class('PakageItem',UIBubbleBox)
local UIEmailInfoForm = require('Logic/UI/UI_Email/UIEmailInfoForm')

---@class PakageItem_Item
local Item = core.Class("PakageItem.Item")

function Item:__init(uiItem)
    self.transform = uiItem
    self.gameObject = uiItem.gameObject

    self.SetActive = function(self,isOn)
        self.gameObject:SetActiveEx(isOn)
    end
end

--region-------PakageItem

function PakageItem:__init(gameObject, parentUI)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.parentUI = parentUI
    self.objMask = CS.DisplayUtil.FindChild(self.gameObject, "mask")

    self.imgIcon = logic.cs.LuaHelper.GetComponent(self.transform, "icon",typeof(logic.cs.Image))
    self.txtTitle = logic.cs.LuaHelper.GetComponent(self.transform, "txtTitle",typeof(logic.cs.Text))
    self.txtCountDown = logic.cs.LuaHelper.GetComponent(self.transform, "countDowd/text",typeof(logic.cs.Text))
    self.txtDescript = logic.cs.LuaHelper.GetComponent(self.transform, "txtDesceription",typeof(logic.cs.Text))

    logic.cs.UIEventListener.AddOnClickListener(self.objMask, OnClose)
end



function PakageItem:SetData(Data)

end

function PakageItem:OnClose(eventData)
    self.gameObject:SetActive()
end

return PakageItem
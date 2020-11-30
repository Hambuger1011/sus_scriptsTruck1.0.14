
---@class UIBubbleBox
local UIBubbleBox = core.Class('UIBubbleBox')
-- local UIBubbleBox_Image = core.Class('UIBubbleBox_Image',UIBubbleBox)
-- local UIBubbleBox_Voice = core.Class('UIBubbleBox_Voice',UIBubbleBox)


--region------- UIBubbleBox

---@param uiItem UIBubbleItem
function UIBubbleBox:__init(uiItem)
    ---@type UIBubbleItem
    self.refUiItem = uiItem
end

function UIBubbleBox:GetSize()
    logic.debug.LogError(string.format('%s未实现SetSize',self.__cname))
end

function UIBubbleBox:SetSize()
    logic.debug.LogError(string.format('%s未实现SetSize',self.__cname))
    -- local size = self:GetSize()
    -- self.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Horizontal, size.x)
    -- self.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Vertical, size.y)
end

---@param bubbleData BubbleData
function UIBubbleBox:SetData(bubbleData)
    logic.debug.LogError(string.format('%s未实现SetData',self.__cname))
end

function UIBubbleBox:OnDeactive()
end


---@param uiItem UIBubbleItem
function UIBubbleBox.Create(uiItem,msgBoxType)
    --logic.debug.LogError('msgBoxType:'..msgBoxType)
    local clazz = nil
    local BoxType = logic.StoryEditorMgr.DataDefine.EBubbleBoxType
    if msgBoxType == BoxType.Text then
        clazz = require('Logic/StoryEditor/UI/Editor/BubbleItem/UIBubbleBox_Text')
    elseif msgBoxType == BoxType.Selection then
        clazz = require('Logic/StoryEditor/UI/Editor/BubbleItem/UIBubbleBox_Selection')
    elseif msgBoxType == BoxType.Voice then
        clazz = require('Logic/StoryEditor/UI/Editor/BubbleItem/UIBubbleBox_Audio')
    elseif msgBoxType == BoxType.Image then
        clazz = require('Logic/StoryEditor/UI/Editor/BubbleItem/UIBubbleBox_Picture')
    end
    if clazz == nil then
        logic.debug.LogError(string.format('Not found:%d',msgBoxType))
    end
    local obj = clazz.New(uiItem)
    return obj
end
--endregion


return UIBubbleBox
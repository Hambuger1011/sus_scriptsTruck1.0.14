---[[[
---可复用Item
---]]]
local Class = core.Class

---@class UIBubbleItem
local UIBubbleItem = Class('UIBubbleItem')
local UIBubbleBox = require('Logic/StoryEditor/UI/Preview/BubbleItem/UIBubbleBox')

function UIBubbleItem:__init(gameObject, msgBoxType)
    self.isEditMode = false
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.canvasGroup = gameObject:GetComponent(typeof(logic.cs.CanvasGroup))
    self.boxSize = core.Vector2.zero




    -- cs_itemScript._onActive = function()
    --     self.canvasGroup.blocksRaycasts = true
    --     self:DOFade(1, 0)
    -- end

    
    -- cs_itemScript._doFade = function(endValue, duration)
    --     self:DOFade(endValue, duration)
    -- end

    self.box = UIBubbleBox.Create(self, msgBoxType)
    
    ---@type t_StoryDetial
    self.storyDetial = nil

end

function UIBubbleItem:GetSize()
    return self.box:GetSize()
end

function UIBubbleItem:SetSize()
    self.box:SetSize()
end

function UIBubbleItem:DOFade(endValue, duration)
    if duration <= 0 then
        self.canvasGroup.alpha = endValue
    else
        local pEntity = self.itemData.pEntity
        pEntity.isTweening = true
        self.canvasGroup:DOFade(endValue, duration):SetId(self):SetEase(core.tween.Ease.Flash):OnComplete(function()
            pEntity.isTweening = false
        end)
    end
end

function UIBubbleItem:SetActive(isOn)
    self.gameObject:SetActiveEx(isOn)
end

---@param itemData UIDialogList.ItemData
function UIBubbleItem:OnLinkItem(itemData)
    self.itemData = itemData
    self.canvasGroup.blocksRaycasts = true
    local pEntity = self.itemData.pEntity
    if pEntity.isTweening then
        --self:DOFade(1, 1.75)
        pEntity.isTweening = false
        self:DOFade(1, 0)
    else
        self:DOFade(1, 0)
    end
end

function UIBubbleItem:OnUnLinkItem()
    self.box:OnDeactive()
    core.tween.DoTween.Kill(self)
    self.canvasGroup.blocksRaycasts = false
    self:DOFade(0, 0)
    --self.cs_itemScript.isTween = false
    if logic.config.isDebugMode then
        self.gameObject.name = "---"..self.gameObject.name;
    end
    --logic.debug.LogError('[-]'..tostring(self))
end


---绑定数据
function UIBubbleItem:BindData()
    ---@type BubbleData
    self.bubbleData = self.GetBubbleDataByIndex(self.itemData.index)
    if self.bubbleData == nil then
        logic.debug.LogError('Not Found:'..self.itemData.index)
    else
        self.box:SetData(self.bubbleData)
    end
    self:SetSize()

    if logic.config.isDebugMode then
        local size = self:GetSize()
        self.gameObject.name = string.format("[%d]-%d-(%.0f-%.0f)-(%.0f-%.0f)", 
        self.itemData.index, self.itemData.index, 
        self.itemData.leftTop, self.itemData.leftTop + self.itemData.size.y,
        size.x,
        size.y
        )
    end
    --logic.debug.LogError('[+]'..tostring(self))
end



return UIBubbleItem
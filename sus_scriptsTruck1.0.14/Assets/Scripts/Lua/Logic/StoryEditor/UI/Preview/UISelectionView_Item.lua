
local BaseClass = core.Class
---@class UISelectionView_Item
local UISelectionView_Item = BaseClass("UISelectionView_Item")

function UISelectionView_Item:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    
    self.uiBinding = self.gameObject:GetComponent(typeof(logic.cs.UIBinding))
    self.button = self.gameObject:GetComponent(typeof(logic.cs.UITweenButton))
    self.button.onClick:AddListener(function()
        --self:SwitchOn(not self.isOn)
        logic.EventDispatcher:Broadcast(logic.EventName.on_story_preview_selection_choice,self.index, self.storyNode)
    end)
    self.isOn = false
    self.lbContent = self.uiBinding:Get('lbContent',typeof(logic.cs.Text))
    -- self.selectedGo = self.uiBinding:Get('selectedGo')
    -- self.selectedGo:SetActiveEx(false)
end

function UISelectionView_Item:__delete()
    if not logic.IsNull(self.gameObject) then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject = nil
    self.transform = nil
    self.button = nil
end

---@param index number
---@param storyNode t_StoryNode
---@param subStoryNode t_StoryNode
function UISelectionView_Item:SetData(index, storyNode, subStoryNode)
    self.index = index
    self.storyNode = storyNode
    self.subStoryNode = subStoryNode
    self:RefresUI()
end

function UISelectionView_Item:RefresUI()
    self.lbContent.text = self.subStoryNode.name
end


function UISelectionView_Item:SwitchOn(isOn)
    if self.onValueChanged then
        self.onValueChanged(self,isOn)
    end
end

function UISelectionView_Item:SetOn(isOn)
    self.isOn = isOn
    --self.selectedGo:SetActiveEx(isOn)
end

return UISelectionView_Item
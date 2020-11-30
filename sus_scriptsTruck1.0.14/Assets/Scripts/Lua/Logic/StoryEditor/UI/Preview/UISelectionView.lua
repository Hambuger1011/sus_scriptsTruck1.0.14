local BaseClass = core.Class

---@class UIStory_SelectionView
local UISelectionView = BaseClass("UISelectionView")
local UISelectionView_Item = require('Logic/StoryEditor/UI/Preview/UISelectionView_Item')

function UISelectionView:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.uiBinding = gameObject:GetComponent(typeof(CS.UIBinding))

    self.selectionRoot = self.uiBinding:Get('selectionRoot').transform
    self.selectionPfb = self.uiBinding:Get('selectionPfb')
    self.selectionPfb:SetActiveEx(false)
end

function UISelectionView:Show()
    self.isActive = true
    self.gameObject:SetActiveEx(true)
end
function UISelectionView:Hide()
    self.isActive = false
    self.gameObject:SetActiveEx(false)
end

---@param storyNode t_StoryNode
function UISelectionView:SetData(storyNode)
    ---@type t_StoryDialog
    self.storyNode = storyNode
    self:InitSelectionList()
end

function UISelectionView:InitSelectionList()
    ---@type UISelectionView_Item[]
    self.uiSelectionItems = self.uiSelectionItems or {}

    local selectionDataList = self.storyNode.children
    local count = table.length(selectionDataList)
    for i=#self.uiSelectionItems, count do
        local go = logic.cs.GameObject.Instantiate(self.selectionPfb, self.selectionRoot)
        --go:SetActiveEx(true)
        local uiItem = UISelectionView_Item.New(go)
        table.insert(self.uiSelectionItems, uiItem)
    end
    for i = 1, #self.uiSelectionItems do
        local uiItem = self.uiSelectionItems[i]
        local isOn = (i <= count)
        if isOn then
            uiItem.gameObject:SetActiveEx(true)
            uiItem:SetData(i,self.storyNode, selectionDataList[i])
            uiItem:SetOn(self.roleHeadIndex == i)
            uiItem.onValueChanged = function(item,isOn)
                self:OnItemSelect(item,isOn)
            end
        else
            uiItem.gameObject:SetActiveEx(false)
            uiItem.onValueChanged = nil
        end
    end
end


---@param uiItem UISelectionView_Item
function UISelectionView:OnItemSelect(uiItem, isOn)
    if isOn then
        self.roleHeadIndex = uiItem.index
    end
    
    for i = 1, #self.uiSelectionItems do
        local uiItem = self.uiSelectionItems[i]
        uiItem:SetOn(uiItem == uiItem)
    end
end


return UISelectionView
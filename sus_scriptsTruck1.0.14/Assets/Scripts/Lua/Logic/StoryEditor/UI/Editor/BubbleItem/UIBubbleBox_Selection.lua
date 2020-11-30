---@class UIBubbleBox_Selection
local UIBubbleBox = require('Logic/StoryEditor/UI/Editor/BubbleItem/UIBubbleBox')
local UIBubbleBox_Selection  = core.Class('UIBubbleBox_Selection',UIBubbleBox)


--region-------UIBubbleBox_Selection

---@param uiItem UIBubbleItem
function UIBubbleBox_Selection:__init(uiItem)
    UIBubbleBox.__init(self, uiItem)
    self.transform = uiItem.transform
    
    self.uiBinding = self.transform:GetComponent(typeof(logic.cs.UIBinding))
    self.btnEditorMask = self.uiBinding:Get('btnEditor',typeof(logic.cs.UITweenButton))
    
    self.verticalLayout = self.uiBinding:Get('selectionRoot',typeof(logic.cs.VerticalLayoutGroup))
    self.selectionRoot = self.uiBinding:Get('selectionRoot').transform
    self.pfbSelection = self.uiBinding:Get('pfbSelection')
    self.pfbSelection:SetActiveEx(false)
    
    self.btnEditorMask.onClick:AddListener(function()
        self:OnEditorClick()
    end)
end

function UIBubbleBox_Selection:GetSize()
    return self.boxSize
end

function UIBubbleBox_Selection:SetSize()
    --self.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Horizontal, self.boxSize.x)
    self.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Vertical, self.boxSize.y)
end

function UIBubbleBox_Selection:OnEditorClick()
    logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_selection_click,self.refUiItem)
end

---@param dialogData BubbleData
function UIBubbleBox_Selection:SetData(bubbleData)
    self:RefreshSelections(bubbleData)

    -- local pos = self.activeBox.labelRoot.anchoredPosition
    -- local extends = core.Vector2.New(Mathf.Abs(pos.x), Mathf.Abs(pos.y))
    -- self.boxSize = core.Vector2.New(w, h + extends.y)
end


---@param dialogData BubbleData
function UIBubbleBox_Selection:RefreshSelections(bubbleData)
    local storyNode = bubbleData.storyNode
    local storyItem = storyNode.storyItem
    local selectionItems = storyNode.children
    ---@type t_StoryDetial
    local storyDetial = bubbleData.storyDetial
    
    local count = #selectionItems
    self.uiSections = self.uiSections or {}
    for i = table.length(self.uiSections) + 1, count do
        local idx = i
        local go = logic.cs.GameObject.Instantiate(self.pfbSelection,self.selectionRoot)
        local tf = go.transform

        local uiItem = {}
        self.uiSections[i] = uiItem
        uiItem.index = i
        uiItem.gameObject = go
        uiItem.transform = tf
        uiItem.isEditMode = self.refUiItem.isEditMode
        uiItem.InputField = tf:Find('InputField'):GetComponent(typeof(logic.cs.InputField))
        uiItem.InputField.onEndEdit:AddListener(function(val)
            uiItem.refSelectionItem.name = val
            --uiItem.InputField.interactable = false
            logic.debug.LogError('onEndEdit：'..val)
        end)
        uiItem.DoEditorSelf = function()
            local msg = uiItem.refSelectionItem.name or ''
            uiItem.InputField.interactable = true
            uiItem.InputField.text = msg
            uiItem.InputField:ActivateInputField()
            --uiItem.InputField.caretPosition = msg.Length
            --logic.cs.EventSystem.current:SetSelectedGameObject(uiItem.InputField.gameObject)
        end
        
        local btn = go:GetComponent(typeof(logic.cs.UITweenButton))
        btn.onClick:AddListener(function()
            logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_selection_item_click,self.refUiItem, uiItem)
        end)

        -- local btnModify = tf:Find('btnModify'):GetComponent(typeof(logic.cs.UITweenButton))
        -- btnModify.onClick:AddListener(function()
        --     self.curModifyItem = uiRole
        --     self.modify_nameInput.text = uiRole.refRole.name
        --     self.modifyPanel:SetActiveEx(true)
        -- end)
    end
    for i=1,table.length(self.uiSections) do
        local go = self.uiSections[i].gameObject
        go:SetActiveEx(i <= count)
    end

    local idx = 0
    ---@param item t_SelectionNode
    for _,item in pairs(selectionItems) do
        idx = idx + 1
        local uiItem = self.uiSections[idx]
        uiItem.rootItem = self.refUiItem
        uiItem.refSelectionItem = item
        uiItem.refDialog = storyNode
        local go = uiItem.gameObject
        local tf = go.transform
        local InputField = uiItem.InputField
        InputField.text = item.name
    end

    local itemSize = self.pfbSelection.transform.rect.size
    self.boxSize = core.Vector2.New(
        itemSize.x, 
        itemSize.y * count + self.verticalLayout.spacing * (count - 1) + self.verticalLayout.padding.vertical
        )
    self.boxSize.y = Mathf.Max(self.boxSize.y, 80)
end

--endregion



return UIBubbleBox_Selection
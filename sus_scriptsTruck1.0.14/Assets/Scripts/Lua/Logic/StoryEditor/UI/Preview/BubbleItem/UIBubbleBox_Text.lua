---@class UIBubbleBox_Text
local UIBubbleBox = require('Logic/StoryEditor/UI/Editor/BubbleItem/UIBubbleBox')
local UIBubbleBox_Text  = core.Class('UIBubbleBox_Text',UIBubbleBox)

---@class UIBubbleBox_Text_Item
local Item = core.Class("UIBubbleBox_Text.Item")

function Item:__init(transform)
    self.transform = transform
    self.gameObject = transform.gameObject

    self.uiBinding = transform:GetComponent(typeof(logic.cs.UIBinding))
    self.headIcon = self.uiBinding:Get('imgIcon', typeof(logic.cs.Image))
    self.lbMsg = self.uiBinding:Get('lbMsg',  typeof(logic.cs.HyperText))
    self.txtName = self.uiBinding:Get('lbName', typeof(logic.cs.Text))
    self.lbMsgRoot = self.uiBinding:Get('msgRoot').transform

    local rootSize = self.lbMsgRoot.rect.size
    self.lbMsgOriginSize = self.lbMsg.transform.rect.size
    self.lableMiniWidth = self.lbMsgOriginSize.x
    if self.lableMiniWidth < 100 then
        self.lableMiniWidth = 100
    end

    --Text extends
    self.extends = rootSize - self.lbMsgOriginSize
    self.SetActive = function(self,isOn)
        self.gameObject:SetActiveEx(isOn)
    end
end

--region-------UIBubbleBox_Text

---@param uiItem UIBubbleItem
function UIBubbleBox_Text:__init(uiItem)
    UIBubbleBox.__init(self, uiItem)
    local transform = uiItem.transform
    local EBubbleType = logic.StoryEditorMgr.DataDefine.EBubbleType
    self.boxes = {
        [EBubbleType.LeadRole] = Item.New(transform:Find("L")),
        [EBubbleType.SupportingRole] = Item.New(transform:Find("R")),
        [EBubbleType.Narration] = Item.New(transform:Find("M")),
    }
    
    self.uiBinding = transform:GetComponent(typeof(logic.cs.UIBinding))
    self.btnEditorMask = self.uiBinding:Get('EditorMask',typeof(logic.cs.UITweenButton))
    
    
    self.btnEditorMask.onClick:AddListener(function()
        self:OnEditorClick()
    end)

    self.roleHeadAtlas = logic.cs.LuaHelper.GetAtlas(
        logic.ResMgr.tag.StoryEditor,
        logic.ResMgr.type.Atlas,
        'UI/StoryEditorRes/RoleHead'
        )
end

function UIBubbleBox_Text:SetBoxType(boxType)
    ---@type UIBubbleBox_Text_Item
    self.activeBox = self.boxes[boxType]
    for _,box in pairs(self.boxes) do
        box:SetActive(self.activeBox == box)
    end
end

function UIBubbleBox_Text:GetSize()
    return self.boxSize
end

function UIBubbleBox_Text:SetSize()

    self.activeBox.lbMsgRoot:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Horizontal, self.lbMsgSize.x)
    self.activeBox.lbMsgRoot:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Vertical, self.lbMsgSize.y)

    self.activeBox.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Horizontal, self.boxSize.x)
    self.activeBox.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Vertical, self.boxSize.y)
    --self.refUiItem.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Vertical, self.boxSize.y)
end

---@param bubbleData BubbleData
function UIBubbleBox_Text:SetData(bubbleData)
    local EBubbleBoxType = logic.StoryEditorMgr.DataDefine.EBubbleBoxType
    local EBubbleType = logic.StoryEditorMgr.DataDefine.EBubbleType

    local storyNode = bubbleData.storyNode
    ---@type t_StoryItem
    local storyItem = storyNode.storyItem
    ---@type StoryEditor_BookDetials
    local storyDetial = bubbleData.storyDetial
    ---@type StoryEditor_Role
    local roleTable = storyDetial.roleTable

    --logic.debug.LogError(self.lbMsg)
    self:SetBoxType(roleTable:GetDialogType(storyItem.roleID))

    local box = self.activeBox
    box.lbMsg.text = storyItem.text

    ---@type t_Role
    local roleInfo = roleTable:GetRole(storyItem.roleID)
    if roleInfo then
        box.txtName.text = roleInfo.name
    else
        box.txtName.text = "???"
    end
    if roleInfo.icon ~= 0 then
        local headName = roleInfo:GetIconName()
        box.headIcon.sprite = self.roleHeadAtlas:GetSprite(headName)
    end

    --获取Text大小
    local txtPreferredSize = core.Vector2.New(
        Mathf.Min(box.lbMsg.preferredWidth, box.lableMiniWidth), 
        box.lbMsg:GetPreferredHeight(box.lbMsgOriginSize.x)
        )
    local w = txtPreferredSize.x + box.extends.x
    local h =txtPreferredSize.y + box.extends.y
    w = Mathf.Max(200,w)
    h = Mathf.Max(75,h)
    self.lbMsgSize = core.Vector2.New(w, h)

    local pos = self.activeBox.lbMsgRoot.anchoredPosition
    local extends = core.Vector2.New(Mathf.Abs(pos.x), Mathf.Abs(pos.y))
    self.boxSize = core.Vector2.New(w, h + extends.y)
end

--endregion


function UIBubbleBox_Text:OnEditorClick()
    logic.EventDispatcher:Broadcast(logic.EventName.on_story_preview_dialog_click,self.refUiItem)
end

return UIBubbleBox_Text
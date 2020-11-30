---@class UIBubbleBox_Audio
local UIBubbleBox = require('Logic/StoryEditor/UI/Editor/BubbleItem/UIBubbleBox')
local UIBubbleBox_Audio  = core.Class('UIBubbleBox_Audio',UIBubbleBox)

---@class UIBubbleBox_Audio_Item
local Item = core.Class("UIBubbleBox_Audio.Item")

function Item:__init(transform)
    self.transform = transform
    self.gameObject = transform.gameObject
    self.labelRoot = transform:Find("text")

    local get = logic.cs.LuaHelper.GetComponent
    self.txtName = get(transform, 'head/name',typeof(logic.cs.Text))
    self.label = get(transform, 'voice/hypertext', typeof(logic.cs.Text))

    self.SetActive = function(self,isOn)
        self.gameObject:SetActiveEx(isOn)
    end
end

--region-------UIBubbleBox_Audio

---@param uiItem UIBubbleItem
function UIBubbleBox_Audio:__init(uiItem)
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
end

function UIBubbleBox_Audio:SetBoxType(boxType)
    ---@type UIBubbleBox_Audio_Item
    self.activeBox = self.boxes[boxType]
    for _,box in pairs(self.boxes) do
        box:SetActive(self.activeBox == box)
    end
end

function UIBubbleBox_Audio:GetSize()
    return self.boxSize
end

function UIBubbleBox_Audio:SetSize()
    self.activeBox.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Horizontal, self.boxSize.x)
    self.activeBox.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Vertical, self.boxSize.y)
    --self.refUiItem.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Vertical, self.boxSize.y)
end

---@param dialogData t_StoryDialog
function UIBubbleBox_Audio:SetData(dialogData)
    --logic.debug.LogError(self.label)
    self:SetBoxType(dialogData:GetDialogType(bubbleData.storyDetial))

    local clip = dialogData.dialogData
    --test
    clip = logic.UIMgr:GetView(logic.uiid.Story_Editor).audioSources.clip
    local box = self.activeBox
    local length = clip.length
    box.label.text = string.format("%s:%.02s\"",60,5)

    ---@type t_StoryDetial
    local storyDetial = bubbleData.storyDetial
    local roleInfo = storyDetial:GetRole(dialogData.roleID)
    if roleInfo then
        box.txtName.text = roleInfo.name
    else
        box.txtName.text = "???"
    end

    self.boxSize = core.Vector2.New(200,40 + 80)
end

--endregion


function UIBubbleBox_Audio:OnEditorClick()
    logic.EventDispatcher:Broadcast(logic.EventName.ON_STORY_Dialog_Click,self.refUiItem)
end

return UIBubbleBox_Audio
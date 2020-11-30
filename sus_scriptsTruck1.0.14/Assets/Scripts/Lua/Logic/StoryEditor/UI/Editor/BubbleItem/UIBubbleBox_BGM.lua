---@class UIBubbleBox_BGM
local UIBubbleBox = require('Logic/StoryEditor/UI/Editor/BubbleItem/UIBubbleBox')
local UIBubbleBox_BGM  = core.Class('UIBubbleBox_BGM',UIBubbleBox)

---@param uiItem UIBubbleItem
function UIBubbleBox_BGM:__init(uiItem)
    UIBubbleBox.__init(self, uiItem)
    local transform = uiItem.transform
    
    self.uiBinding = transform:GetComponent(typeof(logic.cs.UIBinding))
    self.btnEditorMask = self.uiBinding:Get('EditorMask',typeof(logic.cs.UITweenButton))
    
    
    self.transform = transform
    self.gameObject = transform.gameObject

    local get = logic.cs.LuaHelper.GetComponent
    self.txtName = get(transform, 'Frame/head/name',typeof(logic.cs.Text))
    self.label = get(transform, 'Frame/voice/hypertext', typeof(logic.cs.Text))

    self.btnEditorMask.onClick:AddListener(function()
        self:OnEditorClick()
    end)
end

function UIBubbleBox_BGM:GetSize()
    return self.boxSize
end

function UIBubbleBox_BGM:SetSize()
    self.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Horizontal, self.boxSize.x)
    self.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Vertical, self.boxSize.y)
    --self.refUiItem.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Vertical, self.boxSize.y)
end

---@param dialogData BubbleData
function UIBubbleBox_BGM:SetData(bubbleData)

    ---@type t_StoryDialog
    local dialogData = bubbleData.dialogData

    local clip = dialogData.dialogData
    --test
    clip = logic.UIMgr:GetView(logic.uiid.Story_Editor).audioSources.clip

    local length = clip.length
    local min,sec = math.modf(length)
    self.label.text = string.format("%s:%.02s\"",min,sec)

    ---@type t_StoryDetial
    local storyDetial = bubbleData.storyDetial
    local roleInfo = storyDetial:GetRole(dialogData.roleID)
    if roleInfo then
        self.txtName.text = roleInfo.name
    else
        self.txtName.text = "???"
    end

    self.boxSize = core.Vector2.New(200,40 + 80)

    local uiView = logic.UIMgr:GetView(logic.uiid.Story_Preview)
    if uiView then
        uiView.audioSources:Play()
    end
end

--endregion


function UIBubbleBox_BGM:OnEditorClick()
    logic.EventDispatcher:Broadcast(logic.EventName.ON_STORY_Dialog_Click,self.refUiItem)
end

return UIBubbleBox_BGM
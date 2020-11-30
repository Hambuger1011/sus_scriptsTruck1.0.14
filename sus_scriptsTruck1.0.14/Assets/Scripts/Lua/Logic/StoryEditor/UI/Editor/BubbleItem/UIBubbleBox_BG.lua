---@class UIBubbleBox_BG
local UIBubbleBox = require('Logic/StoryEditor/UI/Editor/BubbleItem/UIBubbleBox')
local UIBubbleBox_BG  = core.Class('UIBubbleBox_BG',UIBubbleBox)

---@param uiItem UIBubbleItem
function UIBubbleBox_BG:__init(uiItem)
    UIBubbleBox.__init(self, uiItem)
    local transform = uiItem.transform
    
    
    self.uiBinding = transform:GetComponent(typeof(logic.cs.UIBinding))
    self.btnEditorMask = self.uiBinding:Get('EditorMask',typeof(logic.cs.UITweenButton))
    
    
    self.transform = transform
    self.gameObject = transform.gameObject

    local get = logic.cs.LuaHelper.GetComponent
    self.txtName = get(transform, 'Frame/head/name',typeof(logic.cs.Text))
    self.imgPicture = get(transform, 'Frame/image/picture', typeof(logic.cs.Image))

    self.btnEditorMask.onClick:AddListener(function()
        self:OnEditorClick()
    end)
end

function UIBubbleBox_BG:GetSize()
    return self.boxSize
end

function UIBubbleBox_BG:SetSize()
    self.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Horizontal, self.boxSize.x)
    self.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Vertical, self.boxSize.y)
    --self.refUiItem.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Vertical, self.boxSize.y)
end

---@param dialogData BubbleData
function UIBubbleBox_BG:SetData(bubbleData)
    ---@type t_StoryDialog
    local dialogData = bubbleData.dialogData
    --logic.debug.LogError(self.label)
    ---@type t_StoryDetial
    local storyDetial = bubbleData.storyDetial
    local roleInfo = storyDetial:GetRole(dialogData.roleID)
    if roleInfo then
        self.txtName.text = roleInfo.name
    else
        self.txtName.text = "---"
    end

    --获取image大小
    local spt = self.imgPicture.sprite
    if logic.IsNull(spt) then
        self.boxSize = core.Vector2.New(500,250)
    else
        local vec = spt.bounds.size * spt.pixelsPerUnit
        self.boxSize = core.Vector2.New(vec.x, vec.y)
        self.boxSize.y = self.boxSize.y + 46

        local uiView = logic.UIMgr:GetView(logic.uiid.Story_Preview)
        if uiView then
            uiView.imgBG.sprite = spt
            uiView.imgBG.color = core.Color.white
        end
    end

end

--endregion


function UIBubbleBox_BG:OnEditorClick()
    logic.EventDispatcher:Broadcast(logic.EventName.ON_STORY_Dialog_Click,self.refUiItem)
end

return UIBubbleBox_BG
local BaseClass = core.Class
local ModifyType = {
    Modify = 0,
    Insert = 1,
    Append = 2,
}

---@class UIStory_EditorMenu_Dialog
local UIEditorMenu_Dialog = BaseClass("UIEditorMenu_Dialog")

function UIEditorMenu_Dialog:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.uiBinding = gameObject:GetComponent(typeof(CS.UIBinding))

    local button = gameObject:GetComponent(typeof(logic.cs.Button))
    self.bodyTrans = self.uiBinding:Get('body').transform
    self.btnEditor = self.uiBinding:Get('btnEditor', typeof(logic.cs.UITweenButton))
    self.btnInsert = self.uiBinding:Get('btnInsert', typeof(logic.cs.UITweenButton))
    self.btnAdd = self.uiBinding:Get('btnAdd', typeof(logic.cs.UITweenButton))
    self.btnDelete = self.uiBinding:Get('btnDelete', typeof(logic.cs.UITweenButton))
    self.btnCancel = self.uiBinding:Get('btnCancel', typeof(logic.cs.UITweenButton))
    local DataDefine = logic.StoryEditorMgr.DataDefine
    self.arrows = {
        [DataDefine.EBubbleType.Narration] = self.uiBinding:Get('bg_arrow_mid'),
        [DataDefine.EBubbleType.SupportingRole] = self.uiBinding:Get('bg_arrow_right'),
        [DataDefine.EBubbleType.LeadRole] = self.uiBinding:Get('bg_arrow_left'),
    }

    button.onClick:AddListener(function()
        self:Hide()
    end)

    self.btnEditor.onClick:AddListener(function()
        self:Hide()
        local type = self.refItem.bubbleData.storyNode.storyItem.msgBoxType
        local DataDefine = logic.StoryEditorMgr.DataDefine.EBubbleBoxType
        if type == DataDefine.Text then
            logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_dialog_append_modify, self.refItem, 0)
        elseif type == DataDefine.Image then
            logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_dialog_modify, self.refItem, ModifyType.Modify)
        end
    end)
    self.btnInsert.onClick:AddListener(function()
        self:Hide()
        logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_dialog_insert, self.refItem)
    end)
    self.btnAdd.onClick:AddListener(function()
        self:Hide()
        logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_dialog_append, self.refItem)
    end)
    self.btnDelete.onClick:AddListener(function()
        self:Hide()
        logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_dialog_delete, self.refItem)
    end)
    self.btnCancel.onClick:AddListener(function()
        self:Hide()
    end)
    self.line_1 = self.uiBinding:Get('line_1')
end

function UIEditorMenu_Dialog:Show()
    self.isActive = true
    self.gameObject:SetActiveEx(true)
end


function UIEditorMenu_Dialog:SetPosition(tf)
    local uiView = logic.UIMgr:GetView(logic.uiid.Story_Editor)
    local uiform = uiView.uiform
    local viewSize = uiform:GetViewSize()

    local size = tf.rect.size

    local pos = logic.cs.CUIUtility.World_To_UGUI_LocalPoint(
        uiform:GetCamera(), 
        uiform:GetCamera(), 
        tf.position, 
        uiform.transform
    )
    local offset = core.Vector2.New(0.5,0.5) - tf.pivot
    pos.x = 0 --pos.x + offset.x * size.x
    pos.y = pos.y + offset.y * size.y
    pos.y = pos.y  - self.bodyTrans.rect.size.y * 0.5
    self.bodyTrans.anchoredPosition = pos
end


function UIEditorMenu_Dialog:Hide()
    self.isActive = false
    self.gameObject:SetActiveEx(false)
end

function UIEditorMenu_Dialog:OnPreviewClick()
    self:Hide()
    logic.EventDispatcher:Broadcast(logic.EventName.on_story_preview_click)
end

function UIEditorMenu_Dialog:OnSaveClick()
    self:Hide()
    logic.EventDispatcher:Broadcast(logic.EventName.on_story_chapter_save_click)
end


---@param uiItem UIBubbleItem
function UIEditorMenu_Dialog:SetData(uiItem)
    self.refItem = uiItem
    self:SetArrow()
    self:SetPosition(uiItem.transform)

    local type = uiItem.bubbleData.storyNode.storyItem.msgBoxType
    local DataDefine = logic.StoryEditorMgr.DataDefine.EBubbleBoxType
    local flag = type == DataDefine.Image or type == DataDefine.Text
    self.btnEditor.gameObject:SetActiveEx(flag)
    self.line_1:SetActiveEx(flag)
end

function UIEditorMenu_Dialog:SetArrow()
    local bubbleData = self.refItem.bubbleData
    local roleTable = bubbleData.storyDetial.roleTable
    local storyItem = bubbleData.storyNode.storyItem
    local type = roleTable:GetDialogType(storyItem.roleID)
    for k,go in pairs(self.arrows) do
        go:SetActiveEx(k == type)
    end
end

return UIEditorMenu_Dialog
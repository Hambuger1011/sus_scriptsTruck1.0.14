local BaseClass = core.Class

---@class UIStory_EditorMenu_Dialog
local UIEditorMenu_Selection = BaseClass("UIEditorMenu_Selection")

function UIEditorMenu_Selection:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.uiBinding = gameObject:GetComponent(typeof(CS.UIBinding))

    local button = gameObject:GetComponent(typeof(logic.cs.Button))
    self.bodyTrans = self.uiBinding:Get('body').transform
    self.btnEditor = self.uiBinding:Get('btnEditor', typeof(logic.cs.UITweenButton))
    self.btnInsert = self.uiBinding:Get('btnInsert', typeof(logic.cs.UITweenButton))
    self.btnAdd = self.uiBinding:Get('btnAdd', typeof(logic.cs.UITweenButton))
    self.btnDelete = self.uiBinding:Get('btnDelete', typeof(logic.cs.UITweenButton))
    self.btnEditorDlg = self.uiBinding:Get('btnEditorDlg', typeof(logic.cs.UITweenButton))
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
        self.refSubItem:DoEditorSelf()
        self:Hide()
    end)
    self.btnInsert.onClick:AddListener(function()
        ---@type t_StoryNode
        if table.count(self.refSubItem.refDialog.children)< 4 then
            local storyNode = self.refSubItem.refDialog
            storyNode:InsertSelection(self.refSubItem.index)
            self.refItem.itemData.isDirty = true --重新计算ui大小
            logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_dialog_list_refresh)
        end
        self:Hide()
    end)
    self.btnAdd.onClick:AddListener(function()
        ---@type t_StoryNode
        if table.count(self.refSubItem.refDialog.children)< 4 then
            local storyNode = self.refSubItem.refDialog
            storyNode:InsertSelection(self.refSubItem.index + 1)
            self.refItem.itemData.isDirty = true --重新计算ui大小
            logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_dialog_list_refresh)
        end
        self:Hide()
    end)
    self.btnDelete.onClick:AddListener(function()
        self:Hide()
        -- logic.cs.UIAlertMgr:Show(
        --     "删除对话", 
        --     "删除选中选项", 
        --     logic.cs.AlertType.SureOrCancel,
        --     function(isOK)
        --         if not isOK then
        --             return
        --         end
                
        --         ---@type t_StoryNode
        --         local storyNode = self.refSubItem.refDialog
        --         if #storyNode.children == 1 then
        --             logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_selection_dialog, self.refItem, self.refSubItem)
        --         else
        --             storyNode:RemoveSelection(self.refSubItem.index)
        --             self.refItem.itemData.isDirty = true --重新计算ui大小
        --             logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_dialog_list_refresh)
        --         end
        --     end
        -- )
        
        ---@type t_StoryNode
        local storyNode = self.refSubItem.refDialog
        if #storyNode.children == 1 then
            logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_selection_dialog, self.refItem, self.refSubItem)
        else
            storyNode:RemoveSelection(self.refSubItem.index)
            self.refItem.itemData.isDirty = true --重新计算ui大小
            logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_dialog_list_refresh)
        end
    end)
    self.btnEditorDlg.onClick:AddListener(function()
        self:Hide()
        logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_selection_dialog, self.refItem, self.refSubItem)
    end)
end

---@param uiItem UIBubbleItem
function UIEditorMenu_Selection:Show(uiItem, uiSubItem,posOffset)
    self.isActive = true
    self.gameObject:SetActiveEx(true)
    self:SetData(uiItem, uiSubItem)
    self:SetArrow()
    self:SetPosition(uiSubItem.transform,posOffset)
end

function UIEditorMenu_Selection:SetPosition(tf,posOffset)
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
    pos.x = pos.x + offset.x * size.x
    pos.y = pos.y + offset.y * size.y
    pos.y = pos.y  - self.bodyTrans.rect.size.y * 0.5
    pos = pos + posOffset
    self.bodyTrans.anchoredPosition = pos
end

function UIEditorMenu_Selection:Hide()
    self.isActive = false
    self.gameObject:SetActiveEx(false)
end

function UIEditorMenu_Selection:OnPreviewClick()
    self:Hide()
    logic.EventDispatcher:Broadcast(logic.EventName.on_story_preview_click)
end

function UIEditorMenu_Selection:OnSaveClick()
    self:Hide()
    logic.EventDispatcher:Broadcast(logic.EventName.on_story_chapter_save_click)
end


---@param uiItem UIBubbleItem
function UIEditorMenu_Selection:SetData(uiItem, uiSubItem)
    self.refItem = uiItem
    self.refSubItem = uiSubItem
end

function UIEditorMenu_Selection:SetArrow()
    local bubbleData = self.refItem.bubbleData
    local roleTable = bubbleData.storyDetial.roleTable
    local storyItem = bubbleData.storyNode.storyItem
    local type = roleTable:GetDialogType(storyItem.roleID)
    for k,go in pairs(self.arrows) do
        go:SetActiveEx(k == type)
    end
end

return UIEditorMenu_Selection
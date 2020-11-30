local BaseClass = core.Class
local UIView = core.UIView
---@class UIStory_Editor
local UIStory_Editor = BaseClass("UIStory_Editor", UIView)
local base = UIView

local uiid = logic.uiid

local BubbleData = require('Logic/StoryEditor/UI/Editor/BubbleItem/BubbleData')
local UIBubbleItem = require('Logic/StoryEditor/UI/Editor/BubbleItem/UIBubbleItem')
local UIDialogList = require('Logic/StoryEditor/UI/Utils/UIDialogList')

local DataDefine = require('Logic/StoryEditor/Data/DataDefine')
local StoryData = require('Logic/StoryEditor/Data/StoryData')
local EBubbleType = DataDefine.EBubbleType
local EBubbleBoxType = DataDefine.EBubbleBoxType

UIStory_Editor.config = {
	ID = uiid.Story_Editor,
	AssetName = 'UI/StoryEditorRes/UI/Canvas_Story_Editor'
}


function UIStory_Editor:OnInitView()
    UIView.OnInitView(self)
    local root = self.uiform.transform
    self.uiBinding = root:GetComponent(typeof(CS.UIBinding))
    self.itemPfbs = {
        [EBubbleBoxType.Text] = self.uiBinding:Get('Bubble_Text'),
        [EBubbleBoxType.Selection] = self.uiBinding:Get('Bubble_Selection'),
        [EBubbleBoxType.Image] = self.uiBinding:Get('Bubble_Image'),
        [EBubbleBoxType.Voice] = self.uiBinding:Get('Bubble_Text'),
    }
    for _,go in pairs(self.itemPfbs) do
        go:SetActiveEx(false)
    end
    self.roleView = require('Logic/StoryEditor/UI/Editor/UIRoleView').New(self.uiBinding:Get('roleView'))
    self.addRoleView = require('Logic/StoryEditor/UI/Editor/UIAddRoleView').New(self.uiBinding:Get('addRoleView'))
    self.addRoleView:Hide(true)

    self.addMenuView = require('Logic/StoryEditor/UI/Editor/Menu/UIAddMenuView').New(self.uiBinding:Get('addMenuView'))
    self.addMenuView:Hide(true)

    self.moreMenuView = require('Logic/StoryEditor/UI/Editor/Menu/UIMoreMenuView').New(self.uiBinding:Get('moreMenuView'))
    self.moreMenuView:Hide(true)

    self.editorMenu_Dialog = require('Logic/StoryEditor/UI/Editor/Menu/UIEditorMenu_Dialog').New(self.uiBinding:Get('editorMenu_dialog'))
    self.editorMenu_Dialog:Hide(true)

    self.editorMenu_Selection = require('Logic/StoryEditor/UI/Editor/Menu/UIEditorMenu_Selection').New(self.uiBinding:Get('editorMenu_selection'))
    self.editorMenu_Selection:Hide(true)

    self.uploadImageView = require('Logic/StoryEditor/UI/Editor/SubPanel/UIUploadImageView').New(self.uiBinding:Get('uploadImageView'))
    self.uploadImageView:Hide(true)

    self.body_bottom = self.uiBinding:Get('body_bottom')
    self.insertOtherView = require('Logic/StoryEditor/UI/Editor/Menu/UIInsertOtherView').New(self.uiBinding:Get('insertOtherView'))
    self.insertOtherView:SetData(self.body_bottom)
    self.insertOtherView:Hide(true)
    
    self.insertDialogView = require('Logic/StoryEditor/UI/Editor/Menu/UIInsertDialogView').New(self.uiBinding:Get('insertDialogView'))
    self.insertDialogView:Hide(true)
    
    self.btnClose = self.uiBinding:Get('btnClose', typeof(logic.cs.UITweenButton))
    self.btnClose.onClick:AddListener(function()
        self:OnExitClick()
    end)

    self.btnAddMenu = self.uiBinding:Get('btnAddMenu',typeof(logic.cs.UITweenButton))
    self.btnAddMenu.onClick:AddListener(function()
        if self.addMenuView.isActive then
            self.addMenuView:Hide()
        else
            self.addMenuView:Show()
        end
    end)

    
    self.btnMoreMenu = self.uiBinding:Get('btnMoreMenu',typeof(logic.cs.UITweenButton))
    self.btnMoreMenu.onClick:AddListener(function()
        -- if self.moreMenuView.isActive then
        --     self.moreMenuView:Hide()
        -- else
        --     self.moreMenuView:Show()
        -- end
        logic.EventDispatcher:Broadcast(logic.EventName.on_story_preview_click)
    end)


    self.btnSend = self.uiBinding:Get('btnSend',typeof(logic.cs.UITweenButton))
    self.btnSend.onClick:AddListener(function()
        self:OnSendMessageClick()
    end)
    self.inMsg = self.uiBinding:Get('inMsg',typeof(logic.cs.InputField))
    self.inMsg.text = ''

    ---@type BubbleData
    self.itemDataList = {}

    --self.titleLabel = self.uiBinding:Get('lbTitle',typeof(logic.cs.Text))
    self.subTitleLabel = self.uiBinding:Get('lbSubTitle',typeof(logic.cs.Text))
    self.btnSaveSubDialog = self.uiBinding:Get('btnSaveSubDialog', typeof(logic.cs.UITweenButton))
    self.btnSaveSubDialog.onClick:AddListener(function()
        self:OnExitClick()
    end)

    self.itemRoot = self.uiBinding:Get('Bubble_Root').transform
    ---@type UIDialogList
    self.itemListView = UIDialogList.New(self.uiBinding:Get('Bubble_Root'))
    self.itemListView.onCreateItem = function(index, data, reset)
        return self:OnCreateItem(index, data, reset)
    end

    self.msgPanel = self.uiBinding:Get('msgPanel').transform
    self.originMsgPos = self.msgPanel.anchoredPosition
    self.inMsg.onEndEdit:AddListener(function(val)
        self.msgPanel.anchoredPosition = self.originMsgPos
    end)
end


function UIStory_Editor:OnOpen()
    UIView.OnOpen(self)

    logic.EventDispatcher:AddListener(logic.EventName.on_story_role_new,self.OnRoleNewClick,self)
    logic.EventDispatcher:AddListener(logic.EventName.on_story_preview_click,self.OnPreviewClick,self)
    logic.EventDispatcher:AddListener(logic.EventName.on_story_chapter_save_click,self.OnSaveClick,self)
    logic.EventDispatcher:AddListener(logic.EventName.on_story_editor_dialog_click,self.OnDialogClick,self)
    logic.EventDispatcher:AddListener(logic.EventName.on_story_editor_dialog_delete,self.OnDialogDeleteClick,self)
    logic.EventDispatcher:AddListener(logic.EventName.on_story_editor_selection_add,self.OnAddSelectionClick,self)
    logic.EventDispatcher:AddListener(logic.EventName.on_story_editor_selection_click,self.OnSelectionClick,self)
    logic.EventDispatcher:AddListener(logic.EventName.on_story_editor_selection_item_click,self.OnSelectionItemClick,self)
    logic.EventDispatcher:AddListener(logic.EventName.on_story_editor_selection_dialog,self.OnSelectionDialogEditor,self)
    logic.EventDispatcher:AddListener(logic.EventName.on_story_editor_dialog_list_refresh,self.RefreshDialogList,self)
    logic.EventDispatcher:AddListener(logic.EventName.on_story_editor_dialog_append_option,self.OnOptionInsert,self)
    logic.EventDispatcher:AddListener(logic.EventName.on_story_editor_dialog_modify,self.OnDialogModify,self)
    logic.EventDispatcher:AddListener(logic.EventName.on_story_editor_image_add,self.OnImageInsert,self)
    logic.EventDispatcher:AddListener(logic.EventName.on_story_editor_pickimage,self.OnUploadImageClick,self)
    self:CheckGuide()
end

function UIStory_Editor:OnClose()
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_role_new,self.OnRoleNewClick,self)
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_preview_click,self.OnPreviewClick,self)
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_chapter_save_click,self.OnSaveClick,self)
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_editor_dialog_click,self.OnDialogClick,self)
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_editor_dialog_delete,self.OnDialogDeleteClick,self)
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_editor_selection_add,self.OnAddSelectionClick,self)
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_editor_selection_click,self.OnSelectionClick,self)
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_editor_selection_item_click,self.OnSelectionItemClick,self)
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_editor_selection_dialog,self.OnSelectionDialogEditor,self)
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_editor_dialog_list_refresh,self.RefreshDialogList,self)
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_editor_dialog_append_option,self.OnOptionInsert,self)
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_editor_dialog_modify,self.OnDialogModify,self)
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_editor_image_add,self.OnImageInsert,self)
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_editor_pickimage,self.OnUploadImageClick,self)

    self.roleView:Delete()
    self.addRoleView:Delete()
    self.addMenuView:Delete()
    self.moreMenuView:Delete()
    self.insertDialogView:Delete()
    self.insertOtherView:Delete()
    self.uploadImageView:Delete()
end

function UIStory_Editor:OnExit()
    UIView.__Close(self)
    self:OnSaveClick()
end


---@param storyDetial StoryEditor_BookDetials
---@param chapterData StoryEditor_ChapterDetial
---@param storyTable t_StoryTable
function UIStory_Editor:SetData(storyDetial, chapterData,storyTable)

    local roleTable = storyDetial.roleTable
    local roleJson = roleTable:ToJson()
    local roleMd5 = string.getMd5(roleJson)
    roleTable.md5 = roleMd5

    ---@type StoryEditor_BookDetials
    self.storyDetial = storyDetial

    ---@type StoryEditor_ChapterDetial
    self.chapterData = chapterData
    
    ---@type t_StoryTable
    self.storyTable = storyTable

    self.roleView:SetData(self.storyDetial,self.chapterData,storyTable)
    self.dialogGroupStack = {}

    if storyTable then
        local bookJson = storyTable:ToJson()
        local bookMd5 = string.getMd5(bookJson)
        storyTable.md5 = bookMd5
        self.storyNodeRoot = DataDefine.t_StoryNode.Create(storyTable)
    else
        self.storyNodeRoot = DataDefine.t_StoryNode.New()
    end
    self.storyNodeRoot.name = storyDetial.title
    self:PushDialogGroup(self.storyNodeRoot)
end

---@param storyNode t_StoryNode
function UIStory_Editor:PushDialogGroup(storyNode)
    table.insert(self.dialogGroupStack, storyNode)
    self:SetDialogGroup(storyNode)
end

function UIStory_Editor:PopDialogData()
    if #self.dialogGroupStack == 0 then
        self:OnExit()
        return
    end
    table.remove(self.dialogGroupStack,#self.dialogGroupStack)
    if #self.dialogGroupStack == 0 then
        self:OnExit()
    else
        self:SetDialogGroup(self.dialogGroupStack[#self.dialogGroupStack])
    end
end

---@param storyNode t_StoryNode
function UIStory_Editor:SetDialogGroup(storyNode)
    self.itemListView:ClearItem()
    self.itemDataList = {}

    ---@param type t_StoryNode
    self.curStoryNodeRoot = storyNode

    local height = 0
    ---@param item t_StoryNode
    for i,node in pairs(storyNode.children) do
        ---@type t_StoryItem
        local storyItem = node.storyItem
        local msgBoxType = storyItem.msgBoxType
        local bubbleData = BubbleData.New(self.storyDetial, node)
        table.insert(self.itemDataList, bubbleData)

        height = self.itemListView:AddVirtualItem(msgBoxType)
        --logic.debug.Log('type='..tostring(msgBoxType)..',text='..storyItem.text)
    end
    if storyNode.position then
        self.itemListView:SetHeight(height)
        self.itemListView:MoveToIndex(storyNode.position)
        storyNode.position = nil
    else
        self.itemListView:SetHeight(height, true)
    end
    self:RefreshUI()
end


function UIStory_Editor:OnCreateItem(index, itemData)
    local msgBoxType = itemData.msgBoxType
    local pfb = self.itemPfbs[msgBoxType]
    if logic.IsNull(pfb) then
        logic.debug.LogError('type='..tostring(msgBoxType)..tostring(pfb))
        pfb = self.itemPfb[DataDefine.EBubbleBoxType.Text]
    end
    local go = logic.cs.GameObject.Instantiate(pfb,self.itemListView.transform)
    go:SetActiveEx(true)

    local item = UIBubbleItem.New(go,msgBoxType)
    item.storyDetial = self.storyDetial
    item.GetBubbleDataByIndex = function(index)
        return self.itemDataList[index]
    end
    --local data = self.storyData.items[index + 1]
    --item:SetData(data)

    --[[
        item必须实现函数:
        OnLinkItem/OnUnLinkItem/BindData/GetSize
    ]]
    return item
end

---@param storyNode t_StoryNode
function UIStory_Editor:AddUIItem(storyNode,pos)
    local bubbleData = BubbleData.New(self.storyDetial, storyNode)
    if pos then
        table.insert(self.itemDataList, pos, bubbleData)
    else
        table.insert(self.itemDataList, bubbleData)
    end
    local storyItem = storyNode.storyItem
    local msgBoxType = storyItem.msgBoxType
    
    local height
    if pos then
        height = self.itemListView:InsertVirtualItem(msgBoxType, pos, true)
    else
        height = self.itemListView:AddVirtualItem(msgBoxType, true)
    end
    self:RefreshDialogList()
    self.itemListView:SetHeight(height, true, true)
    -- itemEntity:DOFade(0)
    -- itemEntity:DOFade(1,0.6)
end

--创建角色
function UIStory_Editor:OnRoleNewClick(isEditor)
    local roleData = self.roleView.activeRoleData
    if isEditor then
        if roleData == nil then
            return
        end
        if roleData.id == 0 then    --narration
            return
        end
        self.addRoleView:Show()
        self.addRoleView:SetData(self.storyDetial, self.storyTable, roleData)
    else
        self.addRoleView:Show()
        self.addRoleView:SetData(self.storyDetial, self.storyTable, nil)
    end
end

--点击退出
function UIStory_Editor:OnExitClick()
    self:PopDialogData()
end

--发送消息
function UIStory_Editor:OnSendMessageClick()
    local roleData = self.roleView.activeRoleData
    if not roleData then
        logic.cs.UITipsMgr:PopupTips('Please Select Role', false)
        return
    end
    local strMsg = self.inMsg.text
    if string.IsNullOrEmpty(strMsg) then
        logic.cs.UITipsMgr:PopupTips("Send Content Can't Empty", false)
        return
    end
    self.inMsg.text = ''

    ---@type t_StoryItem
    local storyItem = DataDefine.t_StoryItem.New()
    storyItem.msgBoxType = EBubbleBoxType.Text
    storyItem.text = strMsg
    storyItem.roleID = roleData.id
    
    local node = self.curStoryNodeRoot:CreateChild(self.storyTable,storyItem)
    self:AddUIItem(node)
end

--添加选项
function UIStory_Editor:OnAddSelectionClick()
    self:OnOptionInsert()
end

--插入选项
function UIStory_Editor:OnOptionInsert(index)
    ---@type t_StoryItem
    local storyItem = DataDefine.t_StoryItem.New()
    storyItem.msgBoxType = EBubbleBoxType.Selection
    ---@type t_StoryNode
    local node = self.curStoryNodeRoot:CreateChild(self.storyTable,storyItem,index)
    node.isRoot = true
    node:AddEmptySelection()--添加默认选项1
    node:AddEmptySelection()--添加默认选项2
    self:AddUIItem(node,index)
    
    --item.rootItem.itemData.isDirty = true --重新计算ui大小
    self:RefreshDialogList()
end

--插入图片
function UIStory_Editor:OnImageInsert(filemd5, imgUrl,index)
    local roleData = self.roleView.activeRoleData
    ---@type t_StoryItem
    local storyItem = DataDefine.t_StoryItem.New()
    storyItem.msgBoxType = EBubbleBoxType.Image
    storyItem.roleID = roleData and roleData.id or 0
    storyItem.image = imgUrl
    storyItem.imageMd5 = filemd5
    ---@type t_StoryNode
    local node = self.curStoryNodeRoot:CreateChild(self.storyTable,storyItem,index)
    self:AddUIItem(node,index)
    
    --item.rootItem.itemData.isDirty = true --重新计算ui大小
    self:RefreshDialogList()
end

--对话修改
---@param uiItem UIBubbleItem
function UIStory_Editor:OnDialogModify(uiItem, modifyType, strMsg,roleData)
    
    if modifyType == self.insertOtherView.ModifyType.Modify then    --修改内容
        ---@type t_StoryItem
        local storyItem = uiItem.bubbleData.storyNode.storyItem
        if storyItem.msgBoxType == DataDefine.EBubbleBoxType.Text then
            if string.IsNullOrEmpty(strMsg) then
                logic.cs.UITipsMgr:PopupTips("Send Content Can't Empty", false)
                return
            end
            storyItem.text = strMsg
            storyItem.roleID = roleData.id
            uiItem.itemData.isDirty = true --重新计算ui大小
            self:RefreshDialogList()
        else
            roleData = self.roleView.activeRoleData
            if not roleData then
                logic.cs.UITipsMgr:PopupTips('Please Select Role', false)
                return
            end
            self.uploadImageView:Show(self.storyDetial, self.chapterData)
            self.uploadImageView:SetData(
                self.storyDetial, self.chapterData,self.storyNodeRoot,
                storyItem.image, storyItem.imageMd5,
                function(imgUrl,filemd5)
                    storyItem.roleID = roleData.id
                    storyItem.image = imgUrl
                    storyItem.imageMd5 = filemd5
                    uiItem.itemData.isDirty = true --重新计算ui大小
                    self:RefreshDialogList()
            end)
        end
    else    --插入
        if string.IsNullOrEmpty(strMsg) then
            logic.cs.UITipsMgr:PopupTips("Send Content Can't Empty", false)
            return
        end
        local index = uiItem.itemData.index + 1
        if modifyType == self.insertOtherView.ModifyType.Insert then
            index = uiItem.itemData.index
        end
        ---@type t_StoryItem
        local storyItem = DataDefine.t_StoryItem.New()
        storyItem.msgBoxType = EBubbleBoxType.Text
        storyItem.text = strMsg
        storyItem.roleID = roleData.id
        
        local node = self.curStoryNodeRoot:CreateChild(self.storyTable,storyItem, index)
        self:AddUIItem(node, index)
    end
end

--选项编辑点击
---@param uiItem UIBubbleItem
function UIStory_Editor:OnSelectionClick(uiItem)
    ---@type BubbleData
    local bubbleData = uiItem.bubbleData
    self.editorMenu_Dialog:Show()
    self.editorMenu_Dialog:SetData(uiItem,nil,Vector2.New(0,-11))
end

--选项子分支编辑点击
---@param uiItem UIBubbleItem
function UIStory_Editor:OnSelectionItemClick(uiItem, uiSubItem)
    self.editorMenu_Selection:Show(uiItem, uiSubItem,Vector2.New(0,-10))
end

---编辑选项剧情
---@param uiItem UIBubbleItem
function UIStory_Editor:OnSelectionDialogEditor(uiItem, uiSubItem)
    local index = uiSubItem.index
    local bubbleData = uiItem.bubbleData
    local storyNode = bubbleData.storyNode
    local selectionNode = storyNode.children[index]
    self.curStoryNodeRoot.position = uiItem.itemData.index
    self:PushDialogGroup(selectionNode)
end

--预览
function UIStory_Editor:OnPreviewClick()
    local uiView = logic.UIMgr:Open(logic.uiid.Story_Preview)
    uiView:SetData(self.storyDetial, self.chapterData.chapter_number,self.storyNodeRoot)
end

--保存
function UIStory_Editor:OnSaveClick()
    local storyTable = self.storyNodeRoot:ToStoryTable()
    --不管有没网络，先保存
    logic.StoryEditorMgr:SaveStoryEditorData(self.storyDetial, self.chapterData.chapter_number,storyTable, self.chapterData.update_version)
    
    ----logic.cs.UINetLoadingMgr:Show()
    logic.StoryEditorMgr:UploadChapter(self.storyDetial, self.chapterData.chapter_number,storyTable, function()
        ----logic.cs.UINetLoadingMgr:Close()
        logic.StoryEditorMgr:SaveStoryEditorData(self.storyDetial, self.chapterData.chapter_number,storyTable, self.chapterData.update_version)
    end)
    -- local roleJson = self.storyDetial.roleTable:ToJson()
    -- local bookJson = self.storyNodeRoot:ToJson()
    -- local roleMd5 = logic.cs.CFileManager.GetMd5(roleJson)
    -- local bookMd5 = logic.cs.CFileManager.GetMd5(bookJson)

    -- local isRoleDirty = self.storyDetial.roleTable.md5 ~= roleMd5
    -- local isBookDirty = self.storyNodeRoot.refStoryTable.md5 ~= bookMd5

    -- if isRoleDirty then
    --     logic.gameHttp:StoryEditor_SaveRoleList(self.chapterData.book_id,roleJson, function(result)
    --         local json = core.json.Derialize(result)
    --         local code = tonumber(json.code)
    --         if code == 200 then
    --             self.storyDetial.roleTable.md5 = roleMd5
    --         else
    --             logic.cs.UIAlertMgr:Show("TIPS",json.msg)
    --         end
    --     end)
    -- end
    
    -- if isBookDirty then
    --     logic.StoryEditorMgr:SaveStoryTable(self.chapterData.book_id, self.chapterData.chapter_number, bookJson,function(isOK,jsonData)
    --         if isOK then
    --             self.storyNodeRoot.refStoryTable.md5 = bookMd5
    --             logic.cs.UITipsMgr:PopupTips('Save Success', false)
    --         else
    --             logic.cs.UIAlertMgr:Show("TIPS",jsonData.msg)
    --         end
    --     end)
    -- end
end

--点击对话
---@param uiItem UIBubbleItem
function UIStory_Editor:OnDialogClick(uiItem)
    ---@type BubbleData
    local bubbleData = uiItem.bubbleData
    self.editorMenu_Dialog:Show()
    self.editorMenu_Dialog:SetData(uiItem)
end

--删除对话
---@param uiItem UIBubbleItem
function UIStory_Editor:OnDialogDeleteClick(uiItem)
    local index = uiItem.itemData.index
    logic.debug.LogError('remove:'..index)
    self.itemListView:RemoveItemAt(index)
    table.remove(self.itemDataList, index)
    self.curStoryNodeRoot:RemoveChild(index)
end


--刷新界面
function UIStory_Editor:RefreshUI()
    local hasMutilDialogs = table.length(self.dialogGroupStack) > 1
    --self.titleLabel.gameObject:SetActiveEx(not hasMutilDialogs)
    self.btnMoreMenu.gameObject:SetActiveEx(not hasMutilDialogs)
    self.btnClose.gameObject:SetActiveEx(not hasMutilDialogs)

    self.subTitleLabel.gameObject:SetActiveEx(hasMutilDialogs)
    self.btnSaveSubDialog.gameObject:SetActiveEx(hasMutilDialogs)
    if hasMutilDialogs then
        self.subTitleLabel.text = self.curStoryNodeRoot.name
    else
        --self.titleLabel.text = self.curStoryNodeRoot.name
    end
end

--更新对话列表
function UIStory_Editor:RefreshDialogList()
    self.itemListView:MarkDirty()
    --self.list:ReCalculateLayout()
end


--上传图片
function UIStory_Editor:OnUploadImageClick(insertPosition)
    local roleData = self.roleView.activeRoleData
    if not roleData then
        logic.cs.UITipsMgr:PopupTips('Please Select Role', false)
        return
    end
    self.uploadImageView:Show(self.storyDetial, self.chapterData)
    self.uploadImageView:SetData(
        self.storyDetial, self.chapterData,self.storyNodeRoot,
        nil, nil,
        function(imgUrl,filemd5)
            logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_image_add,filemd5, imgUrl, insertPosition)
    end)
end

--检测是否需要引导
function UIStory_Editor:CheckGuide()
    if logic.cs.UserDataManager.userInfo.data.userinfo.writer_guide ~= 0 then
        return
    end
    logic.UIMgr:Open(logic.uiid.Story_Guide)
end

return UIStory_Editor
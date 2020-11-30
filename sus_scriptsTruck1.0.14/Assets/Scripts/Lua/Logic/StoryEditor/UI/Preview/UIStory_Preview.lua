local BaseClass = core.Class
local UIView = core.UIView
---@class UIStory_Preview
local UIStory_Preview = BaseClass("UIStory_Preview", UIView)
local base = UIView

local uiid = logic.uiid
local UIBubbleEvent = CS.UI.UIBubbleEvent

local BubbleData = require('Logic/StoryEditor/UI/Preview/BubbleItem/BubbleData')
local UIBubbleItem = require('Logic/StoryEditor/UI/Preview/BubbleItem/UIBubbleItem')
local UIDialogList = require('Logic/StoryEditor/UI/Utils/UIDialogList')

local DataDefine = require('Logic/StoryEditor/Data/DataDefine')
local StoryData = require('Logic/StoryEditor/Data/StoryData')
local EBubbleType = DataDefine.EBubbleType
local EBubbleBoxType = DataDefine.EBubbleBoxType

UIStory_Preview.config = {
	ID = uiid.Story_Preview,
	AssetName = 'UI/StoryEditorRes/UI/Canvas_Story_Preview'
}


function UIStory_Preview:OnInitView()
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
    self.itemRoot = self.uiBinding:Get('Bubble_Root').transform
    
    ---@type UIDialogList
    self.itemListView = UIDialogList.New(self.uiBinding:Get('Bubble_Root'))
    self.paddingBottom = self.itemListView:GetPaddingBottom()
    self.itemListView.onCreateItem = function(index, data, reset)
        return self:OnCreateItem(index, data, reset)
    end

    self.btnListView = self.uiBinding:Get('btnStoryListView',typeof(UIBubbleEvent))
    self.btnListView.onClick:AddListener(function()
        self:ShowNextDialog()   --listview click
    end)

    self.touchMask = self.uiBinding:Get('touchMask',typeof(logic.cs.Button))
    self.touchMask.onClick:AddListener(function()
        self:ShowNextDialog()   --mask click
    end)
    
    self.btnClose = self.uiBinding:Get('btnClose', typeof(logic.cs.UITweenButton))
    self.btnClose.onClick:AddListener(function()
        self:OnExitClick()
    end)
    
    -- self.btnMoreMenu = self.uiBinding:Get('btnMoreMenu', typeof(logic.cs.UITweenButton))
    -- self.btnMoreMenu.onClick:AddListener(function()
    -- end)
    
    
    self.selectionView = require('Logic/StoryEditor/UI/Preview/UISelectionView').New(self.uiBinding:Get('selectionView'))
    self.selectionView:Hide(true)

    self.showPicktureView = require('Logic/StoryEditor/UI/Preview/SubPanel/UIShowPictureView').New(self.uiBinding:Get('pictureShowView'))
    self.showPicktureView:Hide(true)


    self.nextChapterView = require('Logic/StoryEditor/UI/Preview/SubPanel/UINextChapterView').New(self.uiBinding:Get('nextChapterView'))
    self.nextChapterView:Hide(true)

    ---@type BubbleData
    self.itemDataList = {}
    self.titleLabel = self.uiBinding:Get('lbTitle',typeof(logic.cs.Text))
    self.objEnd = self.uiBinding:Get('objEnd')
    self.objEnd:SetActiveEx(false)
    self.lbChapterEnd = self.uiBinding:Get('lbChapterEnd', typeof(logic.cs.Text))

    self.btnTop = self.uiBinding:Get('btnTop', typeof(logic.cs.Button))
    self.topPanel = self.uiBinding:Get('topPanel').transform
    self.centerPanel = self.uiBinding:Get('centerPanel').transform
    local safeArea = logic.cs.ResolutionAdapter:GetSafeArea()
    self.safeAreaHeight = self.uiform:yPixel2View(safeArea.y)
    local t = self.btnTop.transform
    local size = t.rect.size
    t:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Vertical, size.y + self.safeAreaHeight)

    t = self.topPanel
    local pos = t.anchoredPosition
    pos.y = pos.y - self.safeAreaHeight
    t.anchoredPosition = pos
    self.topPanelPos = pos
    
    -- t = self.centerPanel
    -- local offsetMax = t.offsetMax
    -- offsetMax.y = offsetMax.y - self.safeAreaHeight
    -- t.offsetMax = offsetMax
    local padding = self.itemListView:GetPadding()
    padding.top = padding.top + Mathf.Ceil(self.safeAreaHeight)
    self:RefreshDialogList()
    self.btnTop.onClick:AddListener(function()
        self:ShowTop()
    end)
end


function UIStory_Preview:OnOpen()
    UIView.OnOpen(self)
    logic.EventDispatcher:AddListener(logic.EventName.on_story_preview_dialog_click,self.OnDialogClick,self)
    logic.EventDispatcher:AddListener(logic.EventName.on_story_preview_selection_click,self.OnSelectionClick,self)
    logic.EventDispatcher:AddListener(logic.EventName.on_story_preview_selection_item_click,self.OnSelectionItemClick,self)
    logic.EventDispatcher:AddListener(logic.EventName.on_story_preview_selection_choice,self.OnSelectionChoice,self)
    
    self:ShowTop()
end

function UIStory_Preview:OnClose()
    UIView.OnClose(self)
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_preview_dialog_click,self.OnDialogClick,self)
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_preview_selection_click,self.OnSelectionClick,self)
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_preview_selection_item_click,self.OnSelectionItemClick,self)
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_preview_selection_choice,self.OnSelectionChoice,self)

    self.showPicktureView:Delete()
    self.selectionView:Delete()
    self.nextChapterView:Delete()
    if self.isReadonly then
        logic.StoryEditorMgr.data:SaveBookReadRecord(self.storyDetial.id,self.chapterID,self.readRecord)
    end
end

function UIStory_Preview:AutoHideTop()
    if self.hideTopTimer then
        self.hideTopTimer:Stop()
    end
    --定时器test
    self.hideTopTimer = core.Timer.New(function()
        self.hideTopTimer = nil
        if logic.IsNull(self.topPanel) then
            return
        end
        local y = self.safeAreaHeight + self.topPanel.rect.size.y + 10
        core.tween.DoTween.Kill('topPanel')
        self.topPanel:DOAnchorPosY(y, 0.75):SetEase(core.tween.Ease.Flash):SetId('topPanel')
    end,10)
    self.hideTopTimer:Start()
end

function UIStory_Preview:ShowTop()
    self:AutoHideTop()
    core.tween.DoTween.Kill('topPanel')
    self.topPanel:DOAnchorPosY(self.topPanelPos.y, 0.75):SetEase(core.tween.Ease.Flash):SetId('topPanel')
end

---@param storyDetial StoryEditor_BookDetials
---@param storyNodeRoot t_StoryNode
function UIStory_Preview:SetData(storyDetial, chapterID,storyNodeRoot, isReadonly)
    logic.debug.Log('[+]阅读书本:'..storyDetial.id..',章节:'..chapterID)
    isReadonly = isReadonly or false
    self.isReadonly = isReadonly
    --self.btnMoreMenu.gameObject:SetActiveEx(not isReadonly)

    ---@type StoryEditor_BookDetials
    self.storyDetial = storyDetial

    self.chapterID = chapterID

    ---@type t_StoryTable
    self.storyNodeRoot = storyNodeRoot
    self.storyNodeRoot.name = storyDetial.title

    self.isPlayEnd = false
    self.dialogIndex = 0

    self:ClearItem()
    self.dialogGroupStack = {}


    self.isReading = false
    ---@type t_ReadingRecord
    self.readRecord = DataDefine.t_ReadingRecord.New()
    if isReadonly then
        --读取记录
        local recordData = logic.StoryEditorMgr.data:LoadBookReadRecord(storyDetial.id,chapterID)
        self:PushDialogGroup(storyNodeRoot)
        local selectionTime = 0
        while recordData.dialogID ~= 0 do
            local node = self.curStoryNodeRoot.children[self.dialogIndex]
            if node == nil then
                logic.debug.LogError("not found node:"..self.dialogIndex)
                break
            end
            if(recordData.dialogID == node.storyItem.id) then
                logic.debug.Log("last reading id:"..node.storyItem.id)
                break
            end

            local isSelection = node:IsSelectionNode()
            if isSelection then
                selectionTime = selectionTime + 1
                local index = recordData.selectionHistory[selectionTime]
                -- local subStoryNode = node.children[index]
                -- subStoryNode.readingPos = 0
                -- self:PushDialogGroup(subStoryNode)
                
                logic.EventDispatcher:Broadcast(logic.EventName.on_story_preview_selection_choice,index, node)
            else
                self:ShowNextDialog()   --auto play
            end
        end
        if recordData.isReadEnd then
            self:ShowNextDialog()
        end
        self.itemListView:MoveToIndex(#self.itemDataList)
    else
        self:PushDialogGroup(storyNodeRoot)
    end
    self.isReading = true
end

---@param storyNode t_StoryNode
function UIStory_Preview:PushDialogGroup(storyNode)
    if self.curStoryNodeRoot then
        self.curStoryNodeRoot.readingPos = self.dialogIndex   --记录该剧情读的进度
    end
    table.insert(self.dialogGroupStack, storyNode)
    storyNode.readingPos = 0
    self:SetDialogGroup(storyNode)
end

function UIStory_Preview:PopDialogData()
    if #self.dialogGroupStack == 0 then
        return
    end
    table.remove(self.dialogGroupStack,#self.dialogGroupStack)
    if #self.dialogGroupStack == 0 then
        --self:OnExit()
        self:PlayEnd()
        --logic.debug.LogError('---Play End---')
    else
        self:SetDialogGroup(self.dialogGroupStack[#self.dialogGroupStack])
    end
end

---@param storyNode t_StoryNode
function UIStory_Preview:SetDialogGroup(storyNode)
    --self.itemListView:ClearItem()
    --self.itemDataList = {}
    self.curStoryNodeRoot = storyNode
    --self.titleLabel.text = dialogs.name
    self.dialogIndex = storyNode.readingPos
    self:RefreshUI()
    
    self:ShowNextDialog()   --auto play
end



function UIStory_Preview:RefreshUI()
    -- local hasMutilDialogs = table.length(self.dialogGroupStack) > 1
    
    -- if hasMutilDialogs then
    --     self.titleLabel.text = self.curStoryNodeRoot.name
    -- else
    --     self.titleLabel.text = self.curStoryNodeRoot.name
    -- end
    self.titleLabel.text = self.storyNodeRoot.name
end

function UIStory_Preview:ClearItem()
    self.itemListView:ClearItem()
    self.itemListView:SetPaddingBottom(self.paddingBottom)
    self.itemDataList = {}
end



function UIStory_Preview:OnCreateItem(index, itemData)
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
    return item
end

---@param storyNode t_StoryNode
function UIStory_Preview:AddUIItem(storyNode, newInstance)
    local bubbleData = BubbleData.New(self.storyDetial, storyNode)
    table.insert(self.itemDataList, bubbleData)
    local storyItem = storyNode.storyItem
    local msgBoxType = storyItem.msgBoxType
    local height = self.itemListView:AddVirtualItem(msgBoxType, newInstance)

    local useTween = self.isReading
    local scrollToBottom = self.isReading
    self.itemListView:SetHeight(height, scrollToBottom, useTween)
    self:RefreshDialogList()
    -- cs_item:DOFade(0)
    -- cs_item:DOFade(1,0.6)
end

function UIStory_Preview:RefreshDialogList()
    self.itemListView:MarkDirty()
    --self.list:ReCalculateLayout()
end

function UIStory_Preview:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end

function UIStory_Preview:ShowNextDialog(force)
    if self.isPlayEnd then
        --logic.cs.UITipsMgr:PopupTips('---Play End---', false)
        return
    end
    --已经读完
    if self.dialogIndex >= table.length(self.curStoryNodeRoot.children) then
        self:PopDialogData()
        --当前是选项
        local subStoryNode = self.curStoryNodeRoot.children[self.dialogIndex]
        local isSelection = subStoryNode and subStoryNode.IsSelectionNode()
        if isSelection and table.length(self.curStoryNodeRoot.children) > self.dialogIndex then
            self:ShowNextDialog(true)   --selection
        end
		return
    end
	self.dialogIndex = self.dialogIndex + 1

    ---@type t_StoryNode
    local subStoryNode = self.curStoryNodeRoot.children[self.dialogIndex]
    if subStoryNode ~= nil then
        self.readRecord.dialogID = subStoryNode.storyItem.id
        local isSelection = subStoryNode:IsSelectionNode()
        if isSelection then
            --self.dialogIndex = self.dialogIndex + 1
            --logic.cs.UITipsMgr:PopupTips("请选择一个选项", false);
            self.selectionView:Show()
            self.selectionView:SetData(subStoryNode)
            return
        end
		self:AddUIItem(subStoryNode, self.isReading)
	else
		logic.debug.LogError('not found index:'..self.dialogIndex)
	end
end




---@param uiItem UIBubbleItem
function UIStory_Preview:OnDialogClick(uiItem)
    if uiItem.isEditMode then
        return
    end

    
    ---@type BubbleData
    local bubbleData = uiItem.bubbleData
    local storyNode = bubbleData.storyNode
    local storyItem = storyNode.storyItem
    if storyItem.msgBoxType == DataDefine.EBubbleBoxType.Image then
        self.showPicktureView:Show(self.uiform, uiItem)
    elseif storyItem.msgBoxType == DataDefine.EBubbleBoxType.Voice then
        --self.audioSources.clip = nil
        self.audioSources:Play()
    else
        self:ShowNextDialog()   --dialog click
    end
end

--点击选项对话（整个选项）
---@param uiItem UIBubbleItem
function UIStory_Preview:OnSelectionClick(uiItem)
    ---@type BubbleData
    local bubbleData = uiItem.bubbleData
    logic.debug.Log('OnSelectionClick')
end

--点击选项分支
---@param uiItem UIBubbleItem
function UIStory_Preview:OnSelectionItemClick(uiItem, uiSubItem)
    logic.debug.Log('OnSelectionItemClick')
end

--点击选择对话分支
---@param index number
---@param storyNode t_StoryNode
function UIStory_Preview:OnSelectionChoice(index, storyNode)
    self.selectionView:Hide()
    local subStoryNode = storyNode.children[index]
    subStoryNode.readingPos = 0
    self:PushDialogGroup(subStoryNode)
    self.readRecord.selectionHistory[#self.readRecord.selectionHistory + 1] = index
end

function UIStory_Preview:SetReadingCoolDown(time)
    if self.chapterID == 1 then
        return
    end
    if time <= 0 then
        return
    end
    --self.nextChapterView:Show()
    --self.nextChapterView:SetData(self.storyDetial,self.chapterID,logic.cs.GameDataMgr:GetCurrentUTCTime() - time)
end

function UIStory_Preview:PlayEnd()
    self.isPlayEnd = true
    if self.isReadonly then
        self.readRecord.isReadEnd = true
        
        --logic.cs.UINetLoadingMgr:Show()
        logic.StoryEditorMgr:FinishReadingChapter(
            self.storyDetial.id,
            self.chapterID,
            function(finishTime)
                --logic.cs.UINetLoadingMgr:Close()
                if self.nextChapterView:SetData(self.storyDetial,self.chapterID, finishTime) then
                    self.nextChapterView:Show()
                else
                    self.objEnd:SetActiveEx(true)
                    self.lbChapterEnd.text = string.format('CH.%d END', self.chapterID)
                end
                local h = self.objEnd.transform.rect.height
                if h >self.paddingBottom then
                    self.itemListView:SetPaddingBottom(h)
                end
        end)
    else
        self.objEnd:SetActiveEx(true)
        self.lbChapterEnd.text = string.format('CH.%d END', self.chapterID)
        
        local h = self.objEnd.transform.rect.height
        if h >self.paddingBottom then
            self.itemListView:SetPaddingBottom(h)
        end
    end
end

return UIStory_Preview
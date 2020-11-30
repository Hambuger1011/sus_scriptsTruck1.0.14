--[[
    章节菜单
]]
local BaseClass = core.Class

---@class UIStory_Menu_Chapter
local UIMenu_Chapter = BaseClass("UIMenu_Chapter")

function UIMenu_Chapter:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.uiBinding = gameObject:GetComponent(typeof(CS.UIBinding))

    self.btnEditDialog = self.uiBinding:Get('btnEditDialog', typeof(logic.cs.UITweenButton))
    self.btnEditDetails = self.uiBinding:Get('btnEditDetails', typeof(logic.cs.UITweenButton))
    self.btnUpload = self.uiBinding:Get('btnUpload', typeof(logic.cs.UITweenButton))
    self.btnPreview = self.uiBinding:Get('btnPreview', typeof(logic.cs.UITweenButton))
    self.btnCancel = self.uiBinding:Get('btnCancel', typeof(logic.cs.UITweenButton))

    self.btnEditDetails.onClick:AddListener(function()
        self:OnEditDetialsClick()
    end)
    self.btnEditDialog.onClick:AddListener(function()
        self:OnEditContentClick()
    end)
    self.btnUpload.onClick:AddListener(function()
        self:OnUploadClick()
    end)
    self.btnPreview.onClick:AddListener(function()
        self:OnPreViewClick()
    end)
    self.btnCancel.onClick:AddListener(function()
        self:Hide()
    end)
    self.root = self.uiBinding:Get('root').transform
    self.originRootPos = self.root.anchoredPosition
    self.mask = self.gameObject:GetComponent(typeof(logic.cs.Image))
end

function UIMenu_Chapter:Show()
    self.isActive = true
    self.gameObject:SetActiveEx(true)

    core.tween.DoTween.Kill(self)
    local p = self.originRootPos
    self.root.anchoredPosition = core.Vector2.New(p.x, p.y - self.root.rect.size.y)
    self.root:DOAnchorPosY(p.y,0.3):SetEase(core.tween.Ease.Flash):SetId(self)
    self.mask:DOFade(0,0):SetEase(core.tween.Ease.Flash):SetId(self)
    self.mask:DOFade(0.5, 0.2):SetEase(core.tween.Ease.Flash):SetId(self)
end
function UIMenu_Chapter:Hide(notUseTween)
    self.isActive = false
    
    if notUseTween then
        self.gameObject:SetActiveEx(false)
    else
        core.tween.DoTween.Kill(self)
        local p = self.originRootPos
        --self.root.anchoredPosition = core.Vector2.New(p.x, p.y - self.root.rect.size.y):SetId(self)
        self.root:DOAnchorPosY(p.y - self.root.rect.size.y,0.2):SetEase(core.tween.Ease.Flash):SetId(self):OnComplete(function()
            self.gameObject:SetActiveEx(false)
        end)
        --self.mask:DOFade(0,0):SetEase(core.tween.Ease.Flash):SetId(self)
        self.mask:DOFade(0, 0.3):SetEase(core.tween.Ease.Flash):SetId(self)
    end
end

---@param uiItem UIChapterItem
function UIMenu_Chapter:SetData(uiItem)
    self.index = uiItem.index
    ---@type StoryEditor_BookDetials
    self.storyDetial = uiItem.storyDetial
    ---@type StoryEditor_ChapterDetial
    self.chapterData = uiItem.chapterData
end

--编辑decription
function UIMenu_Chapter:OnEditDetialsClick()
    self:Hide()
    
    logic.EventDispatcher:Broadcast(logic.EventName.on_story_chapter_editor, self.index)
end

--编辑对话
function UIMenu_Chapter:OnEditContentClick()
    self:Hide()
    logic.EventDispatcher:Broadcast(logic.EventName.on_story_chapter_content_editor, self.chapterData)
end

--预览
function UIMenu_Chapter:OnPreViewClick()
    self:Hide()
    
    local DataDefine = logic.StoryEditorMgr.DataDefine
    local chapterData = self.chapterData
    local chapterID = chapterData.chapter_number
    --logic.cs.UINetLoadingMgr:Show()
    logic.StoryEditorMgr:LoadStoryEditorData(self.storyDetial,chapterID,chapterData.update_version,function(storyTable)
        --logic.cs.UINetLoadingMgr:Close()
        local storyNodeRoot = DataDefine.t_StoryNode.Create(storyTable)
        --storyNodeRoot.name = self.storyDetial.title
        local uiView = logic.UIMgr:Open(logic.uiid.Story_Preview)
        uiView:SetData(self.storyDetial, chapterID, storyNodeRoot, false)
    end)
end

--上传
function UIMenu_Chapter:OnUploadClick()
    local DataDefine = logic.StoryEditorMgr.DataDefine
    local chapterID = self.chapterData.chapter_number
    --logic.cs.UINetLoadingMgr:Show()
    logic.StoryEditorMgr:SumbitChapter(self.storyDetial,chapterID,function()
        --logic.cs.UINetLoadingMgr:Close()
        self:Hide()
        logic.EventDispatcher:Broadcast(logic.EventName.on_story_chapter_refresh)
    end)

    --【AF事件记录*发送 完成1个UGC章节创作】
    CS.AppsFlyerManager.Instance:CREATE_UGC_CHAPTER();

end

return UIMenu_Chapter
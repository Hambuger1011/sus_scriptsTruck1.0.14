--[[
    主面板
]]
local BaseClass = core.Class
local UIView = core.UIView
local UIStory_Chapter = BaseClass("UIStory_Chapter", UIView)
local base = UIView
local uiid = logic.uiid

UIStory_Chapter.config = {
	ID = uiid.Story_Chapter,
	AssetName = 'UI/StoryEditorRes/UI/Canvas_StoryChapter'
}

function UIStory_Chapter:OnInitView()
    UIView.OnInitView(self)
    local root = self.uiform.transform
    self.uiBinding = root:GetComponent(typeof(CS.UIBinding))

    self.btnClose = self.uiBinding:Get('btnClose',typeof(logic.cs.UITweenButton))
    self.btnCloseImg = self.uiBinding:Get('btnClose',typeof(logic.cs.Image))
    self.bodyImage = self.uiBinding:Get('BodyImage',typeof(logic.cs.Image))
    self.btnClose.onClick:AddListener(function()
        self:__Close()
    end)
    self.uiBinding = self.uiBinding:Get('chapterRoot',typeof(logic.cs.UIBinding))
    self.lbBookName = self.uiBinding:Get('lbBookName', typeof(logic.cs.Text))
    self.lbChapterName = self.uiBinding:Get('lbChapterName',  typeof(logic.cs.Text))
    self.lbChapterDesc = self.uiBinding:Get('lbChapterDesc',  typeof(logic.cs.Text))
    self.lbAuthor = self.uiBinding:Get('lbAuthor',  typeof(logic.cs.Text))
    self.lbLectoresCount = self.uiBinding:Get('lbLectoresCount', typeof(logic.cs.Text))

    self.imgCover = self.uiBinding:Get('imgCover', typeof(logic.cs.Image))
    self.defaultCoverSize = self.imgCover.transform.rect.size
    self.defaultSprite = self.imgCover.sprite

    self.btnPlay = self.uiBinding:Get('btnPlay', typeof(logic.cs.UITweenButton))
    self.btnPlay.onClick:AddListener(function()
        self:OnPlayClick()
    end)

    self.bodyImage:DOFade(0.6,0.3):SetEase(core.tween.Ease.Flash):Play()
    self.uiBinding.transform.anchoredPosition = core.Vector2.New(1200, 32)
    self.uiBinding.transform:DOAnchorPosX(0,0.2):SetEase(core.tween.Ease.OutFlash):Play()

    self.btnClose.transform.anchoredPosition = core.Vector2.New(0, -210)
    self.btnCloseImg.color =  core.Color.New(1,1,1,0)
    self.btnClose.transform:DOAnchorPosY(-120,0.3):SetDelay(0.2):SetEase(core.tween.Ease.OutBack):Play()
    self.btnCloseImg:DOFade(1,0.3):SetDelay(0.2):SetEase(core.tween.Ease.Flash):Play()

    self.btnStar = self.uiBinding:Get('btnStar', typeof(logic.cs.UIToggle))
end

function UIStory_Chapter:OnOpen()
    UIView.OnOpen(self)
end


---@param storyDetial StoryEditor_BookDetials
function UIStory_Chapter:SetData(storyDetial)
    ---@type StoryEditor_BookDetials
    self.storyDetial = storyDetial
    self:RefresUI()
    self.isReady = false
    self.btnStar.onValueChanged:RemoveAllListeners()
    self.btnStar.isOn = (storyDetial.is_fav == 1)
    self.btnStar.onValueChanged:AddListener(function(isOn)
        logic.StoryEditorMgr:SaveFavorite(self.storyDetial.id,isOn,function(isOK, isFav)
            if not isOK then
                return
            end
            self.storyDetial.is_fav = isFav
        end)
    end)
    self.lbChapterName.text = 'Chapter.-- of '..self.storyDetial.total_chapter_count
    logic.gameHttp:StoryEditor_GetReadingRecord(storyDetial.id, function(result)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 200 then
            self.isReady = true
            local data = json.data
            self.chapterIndex = data.chapter_number
            if self.chapterIndex > self.storyDetial.total_chapter_count then
                self.chapterIndex = self.storyDetial.total_chapter_count
            end
            self.pay_countdown = data.pay_countdown
            self.lbChapterName.text = 'Chapter.'..self.chapterIndex..' of '..self.storyDetial.total_chapter_count
        else
            logic.cs.UIAlertMgr:Show("TIPS",json.msg)
        end
    end)
end

function UIStory_Chapter:RefresUI()
    self:SetBookCover(nil)
    local cover_image = self.storyDetial.cover_image
    if not string.IsNullOrEmpty(cover_image) then
        local url,md5 = logic.StoryEditorMgr:ParseImageUrl(cover_image)
        local filename = logic.config.WritablePath .. '/cache/story_image/'..md5
        logic.StoryEditorMgr.data:LoadSprite(filename, md5, url, function(sprite)
            self:SetBookCover(sprite)
        end)
    end

    self.lbBookName.text = self.storyDetial.title
    self.lbChapterDesc.text = self.storyDetial.description
    self.lbAuthor.text = 'Author: '..(self.storyDetial.writer_name or '')
    self.lbLectoresCount.text = tostring(self.storyDetial.read_count)
    self:InitCatagoryList()
end

function UIStory_Chapter:SetBookCover(sprite)
    local uiImage = self.imgCover
    if logic.IsNull(uiImage) then
        return
    end
    if logic.IsNull(sprite) then
        sprite = self.defaultSprite
    end
    logic.StoryEditorMgr:SetCover(uiImage,sprite,self.defaultCoverSize)
end


function UIStory_Chapter:InitCatagoryList()
    self.selectCatagorys = self.selectCatagorys or {}
    ---@type UICatagoryItem[]
    self.uiCatagorys = self.uiCatagorys or {}
    
    if self.catagoryRoot == nil then
        self.catagoryRoot = self.uiBinding:Get('catagoryRoot').transform
        local getComponent = logic.cs.LuaHelper.GetComponent
        for i=0, 2 do
            local tf = self.catagoryRoot:GetChild(i)
            --go:SetActiveEx(true)
            local uiItem = {
                transform = tf,
                gameObject = tf.gameObject,
                lbName = getComponent(tf,'Text',typeof(logic.cs.Text))
            }
            uiItem.SetData = function(self,index)
                local DataDefine = logic.StoryEditorMgr.DataDefine
                local name = DataDefine.BookTags[index]
                self.lbName.text = name
            end
            table.insert(self.uiCatagorys, uiItem)
        end
    end

    for i = 1, #self.uiCatagorys do
        local uiItem = self.uiCatagorys[i]
        local kv = self.storyDetial.tagArray:GetKVByIndex(i)
        local isOn = (kv and kv.Value)
        if isOn then
            uiItem.gameObject:SetActiveEx(true)
            uiItem:SetData(kv.Key)
        else
            uiItem.gameObject:SetActiveEx(false)
        end
    end
end


function UIStory_Chapter:OnPlayClick()
    if not self.isReady then
        logic.debug.LogError('服务端没返回进度数据，你读个p?>')
        return
    end
    if not self.chapterIndex then
    end
    local DataDefine = logic.StoryEditorMgr.DataDefine
    local chapterID = self.chapterIndex
    --logic.cs.UINetLoadingMgr:Show()
    logic.StoryEditorMgr:GetDialogList(self.storyDetial.id,chapterID,function(storyTable)
        --logic.cs.UINetLoadingMgr:Close()
        local storyNodeRoot = DataDefine.t_StoryNode.Create(storyTable)
        --storyNodeRoot.name = self.storyDetial.title
        
        logic.StoryEditorMgr:EnterStoryEditorMode()
        --self.uiform:Hide()
        self:__Close()
        local uiView = logic.UIMgr:Open(logic.uiid.Story_Preview)
        uiView:SetData(self.storyDetial, chapterID, storyNodeRoot, true)
        uiView:SetReadingCoolDown(self.pay_countdown)
        uiView.onClose = function()
            logic.StoryEditorMgr:BackToMainClick()
            --self.uiform:Appear()
        end
    end)
end

return UIStory_Chapter
--[[
    主面板
]]
local BaseClass = core.Class
local UIView = core.UIView
local UIStoryChapterForm = BaseClass("UIStoryChapterForm", UIView)
local base = UIView
local uiid = logic.uiid

UIStoryChapterForm.config = {
	ID = uiid.UIStoryChapterForm,
	AssetName = 'UI/StoryEditorRes/UI/UIStoryChapterForm'
}


local this=nil;
function UIStoryChapterForm:OnInitView()
    UIView.OnInitView(self)
    this=self.uiform;

    self.CloseBtn = CS.DisplayUtil.GetChild(this.gameObject, "CloseBtn");


    --self.HeadIcon =CS.DisplayUtil.GetChild(gameObject, "HeadIcon"):GetComponent("Image");
    --self.State =CS.DisplayUtil.GetChild(gameObject, "State");
    self.BookName =CS.DisplayUtil.GetChild(this.gameObject, "BookName"):GetComponent("Text");
    self.Chapter =CS.DisplayUtil.GetChild(this.gameObject, "Chapter"):GetComponent("Text");
    self.Content =CS.DisplayUtil.GetChild(this.gameObject, "Content"):GetComponent("Text");
    self.Author = CS.DisplayUtil.GetChild(this.gameObject, "Author");
    self.HeadIcon =CS.DisplayUtil.GetChild(self.Author, "HeadIcon"):GetComponent("Image");
    self.HeadFrame =CS.DisplayUtil.GetChild(self.Author, "HeadFrame"):GetComponent("Image");
    self.AuthorName =CS.DisplayUtil.GetChild(self.Author, "AuthorName"):GetComponent("Text");

    self.LookNumberText =CS.DisplayUtil.GetChild(this.gameObject, "LookNumberText"):GetComponent("Text");
    self.imgCover =CS.DisplayUtil.GetChild(this.gameObject, "imgCover"):GetComponent("Image");
    self.ReadBtn = CS.DisplayUtil.GetChild(this.gameObject, "ReadBtn");
    self.btnStar =CS.DisplayUtil.GetChild(this.gameObject, "LikeToogle"):GetComponent("UIToggle");
    self.catagoryRoot = CS.DisplayUtil.GetChild(this.gameObject, "catagoryRoot");

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.CloseBtn,function(data) self:OnExitClick() end);
    logic.cs.UIEventListener.AddOnClickListener(self.ReadBtn,function(data) self:OnPlayClick() end);
    logic.cs.UIEventListener.AddOnClickListener(self.HeadFrame.gameObject,function(data) self:HeadIconClick() end);

    --self.bodyImage:DOFade(0.6,0.3):SetEase(core.tween.Ease.Flash):Play()
    --self.uiBinding.transform.anchoredPosition = core.Vector2.New(1200, 32)
    --self.uiBinding.transform:DOAnchorPosX(0,0.2):SetEase(core.tween.Ease.OutFlash):Play()
    --self.btnClose.transform.anchoredPosition = core.Vector2.New(0, -210)
    --self.btnCloseImg.color =  core.Color.New(1,1,1,0)
    --self.btnClose.transform:DOAnchorPosY(-120,0.3):SetDelay(0.2):SetEase(core.tween.Ease.OutBack):Play()
    --self.btnCloseImg:DOFade(1,0.3):SetDelay(0.2):SetEase(core.tween.Ease.Flash):Play()
end

function UIStoryChapterForm:OnOpen()
    UIView.OnOpen(self)
end
--region 【OnClose】

function UIStoryChapterForm:OnClose()
    UIView.OnClose(self)
    if(self.CloseBtn)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.CloseBtn,function(data) self:OnExitClick() end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.ReadBtn,function(data) self:OnPlayClick() end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.HeadFrame.gameObject,function(data) self:HeadIconClick() end);
    end

end
--endregion

---@param storyDetial StoryEditor_BookDetials
function UIStoryChapterForm:SetData(storyDetial)
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
    self.Chapter.text = 'Chapter.-- of '..self.storyDetial.total_chapter_count
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
            self.Chapter.text = 'Chapter.'..self.chapterIndex..' of '..self.storyDetial.total_chapter_count
        else
            logic.cs.UIAlertMgr:Show("TIPS",json.msg)
        end
    end)
end

function UIStoryChapterForm:RefresUI()
    self:SetBookCover(nil)
    local cover_image = self.storyDetial.cover_image
    if not string.IsNullOrEmpty(cover_image) then
        local url,md5 = logic.StoryEditorMgr:ParseImageUrl(cover_image)
        local filename = logic.config.WritablePath .. '/cache/story_image/'..md5
        logic.StoryEditorMgr.data:LoadSprite(filename, md5, url, function(sprite)
            self:SetBookCover(sprite)
        end)
    end

    self.BookName.text = self.storyDetial.title;
    self.Content.text = self.storyDetial.description;
    self.AuthorName.text = self.storyDetial.user_info.nickname;
    self.LookNumberText.text = tostring(self.storyDetial.read_count)
    self:InitCatagoryList()

    if(self.storyDetial and  self.storyDetial.user_info)then
        --显示头像
        GameHelper.luaShowDressUpForm(self.storyDetial.user_info.avatar,self.HeadIcon,DressUp.Avatar,1001);
        --加载头像框
        GameHelper.luaShowDressUpForm(self.storyDetial.user_info.avatar_frame,self.HeadFrame,DressUp.AvatarFrame,2001);
    else
        --显示头像
        GameHelper.luaShowDressUpForm(-1,self.HeadIcon,DressUp.Avatar,1001);
        --加载头像框
        GameHelper.luaShowDressUpForm(-1,self.HeadFrame,DressUp.AvatarFrame,2001);
    end
end

function UIStoryChapterForm:SetBookCover(sprite)
    if logic.IsNull(self.imgCover) then
        return
    end
    if logic.IsNull(sprite) then
        sprite = self.imgCover.sprite
    end
    logic.StoryEditorMgr:SetCover(self.imgCover,sprite,self.imgCover.transform.rect.size)
end


function UIStoryChapterForm:InitCatagoryList()
    self.selectCatagorys = self.selectCatagorys or {}
    ---@type UICatagoryItem[]
    self.uiCatagorys = self.uiCatagorys or {}
    
    if(self.catagoryRoot)then

        local getComponent = logic.cs.LuaHelper.GetComponent
        for i=0, 2 do
            local tf = self.catagoryRoot.transform:GetChild(i)
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


function UIStoryChapterForm:OnPlayClick()
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


function UIStoryChapterForm:HeadIconClick()
    if(self.storyDetial and self.storyDetial.writer_id)then
        self:OnExitClick();
      GameController.CommunityControl:GetWriterInfoRequest(self.storyDetial.writer_id);
    end

end

--region 【界面关闭】
function UIStoryChapterForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end
--endregion


return UIStoryChapterForm
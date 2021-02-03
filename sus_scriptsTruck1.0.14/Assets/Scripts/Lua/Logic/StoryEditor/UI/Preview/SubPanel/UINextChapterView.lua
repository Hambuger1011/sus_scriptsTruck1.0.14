local BaseClass = core.Class

---@class UIStory_UINextChapterView
local UINextChapterView = BaseClass("UINextChapterView")

function UINextChapterView:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.uiBinding = gameObject:GetComponent(typeof(CS.UIBinding))

    self.lbChapter = self.uiBinding:Get('lbChapter', typeof(logic.cs.Text))

    -- self.imgCover = self.uiBinding:Get('imgCover', typeof(logic.cs.Image))
    -- self.defaultCoverSize = self.imgCover.transform.rect.size
    -- self.defaultSprite = self.imgCover.sprite

    self.objUnlock = self.uiBinding:Get('objUnlock')
    self.objLock = self.uiBinding:Get('objLock')

    self.btnReadNext = self.uiBinding:Get('btnReadNext',typeof(logic.cs.UITweenButton))
    self.btnAds = self.uiBinding:Get('btnAds',typeof(logic.cs.UITweenButton))
    self.btnBuy = self.uiBinding:Get('btnBuy',typeof(logic.cs.UITweenButton))
    self.lbTimer = self.uiBinding:Get('lbTimer', typeof(logic.cs.Text))
    self.lbCostNum = self.uiBinding:Get('lbCostNum', typeof(logic.cs.Text))
    self.lbChapterEnd = self.uiBinding:Get('lbChapterEnd', typeof(logic.cs.Text))
    self.lbChapterDesc = self.uiBinding:Get('lbChapterDesc', typeof(logic.cs.Text))

    self.btnReadNext.onClick:AddListener(function()
        self:OnReadNextClick()
    end)
    self.btnAds.onClick:AddListener(function()
        self:OnAdsClick()
    end)
    self.btnBuy.onClick:AddListener(function()
        self:OnBuyClick()
    end)
end

function UINextChapterView:__delete()
    self:Hide()
end

function UINextChapterView:Hide()
    self.isActive = false
    self.gameObject:SetActiveEx(false)
    self:StopTimer()
end

function UINextChapterView:StopTimer()
    if self.timer then
        self.timer:Stop()
        self.timer = nil
    end
end

function UINextChapterView:Show()
    self.isActive = true
    self.gameObject:SetActiveEx(true)


end

---@param storyDetial StoryEditor_BookDetials
function UINextChapterView:SetData(storyDetial, chapterID, finishTime)
    ---@type StoryEditor_BookDetials
    self.storyDetial = storyDetial
    self.chapterID = chapterID
    if chapterID < storyDetial.update_chapter_count then
        self.nextChapterID = chapterID + 1
    else
        self:Hide()
        return false
    end
    self.finishTime = finishTime

    self.lbChapter.text = 'Chapter '..self.nextChapterID

    self.lbChapterEnd.text = string.format('CH.%d END', self.chapterID)
    self.lbChapterDesc.text = ''--self.chapterData.description

    --【AF事件记录* 读完1个UGC章节】
    CS.AppsFlyerManager.Instance:FINISH_UGC_CHAPTER();

    --logic.cs.UINetLoadingMgr:Show()
    logic.gameHttp:StoryEditor_GetChapterDetail(storyDetial.id, self.nextChapterID,function(result)

        --logic.cs.UINetLoadingMgr:Close()
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 200 then
            ---@param chapterData StoryEditor_ChapterDetial
            self.chapterData = json.data
            self.lbChapterDesc.text = self.chapterData.description
        else
            logic.cs.UIAlertMgr:Show("TIPS",json.msg)
        end
    end)
    -- self:SetBookCover(nil)
    -- local cover_image = self.storyDetial.cover_image
    -- if not string.IsNullOrEmpty(cover_image) then
    --     local url,md5 = logic.StoryEditorMgr:ParseImageUrl(cover_image)
    --     local filename = logic.config.WritablePath .. '/cache/story_image/'..md5
    --     logic.StoryEditorMgr.data:LoadSprite(filename, md5, url, function(sprite)
    --         self:SetBookCover(sprite)
    --     end)
    -- end

    local delta = self:GetRemainTime()
    self:SetLock(delta > 0)
    return true
end

function UINextChapterView:GetRemainTime()
    local curTime = logic.cs.GameDataMgr:GetCurrentUTCTime()
    local delta = curTime - self.finishTime
    delta = 60 * 10 - delta
    return delta
end

function UINextChapterView:SetLock(isLock)
    logic.debug.LogError('chapter is lock:'..tostring(isLock))
    self.objLock:SetActiveEx(isLock)
    self.objUnlock:SetActiveEx(not isLock)

    self:StopTimer()
    if isLock then
        --定时器test
        self.timer = core.Timer.New(function()
            --剩余时间
            local delta = self:GetRemainTime()
            if delta <= 0 then	--时间到
                self:SetLock(false)
            else
                local s = math.fmod(delta, 60)
                local m = math.fmod(math.floor(delta/60), 60)
                local h = math.floor(delta/3600)
                self.lbTimer.text = string.format("%.2d:%.2d:%.2d",h,m,s)
            end
        end,0.5,-1)
        self.timer:Start()
    end
end


function UINextChapterView:SetBookCover(sprite)
    local uiImage = self.imgCover
    if logic.IsNull(uiImage) then
        return
    end
    if logic.IsNull(sprite) then
        sprite = self.defaultSprite
    end
    logic.StoryEditorMgr:SetCover(uiImage,sprite,self.defaultCoverSize)
end

function UINextChapterView:OnReadNextClick()
    local DataDefine = logic.StoryEditorMgr.DataDefine
    local chapterID = self.nextChapterID
    --logic.cs.UINetLoadingMgr:Show()
    self:SaveReadingRecord(3)
end

function UINextChapterView:OnAdsClick()
    local seeVideoNum = 1
    logic.cs.SdkMgr.ads:ShowRewardBasedVideo("Buy-Clothes",function(success)
        if not success then
            return
        end
        self:SaveReadingRecord(2)
    end)
end

function UINextChapterView:OnBuyClick()
    local cost = 1
    if logic.cs.UserDataManager.UserData.KeyNum < cost then
        self:ShowChargeTips(cost)
    else
        self:SaveReadingRecord(1)
    end
end

function UINextChapterView:SaveReadingRecord(payType)
    local chapterID = self.nextChapterID
    local KeyNum = logic.cs.UserDataManager.UserData.KeyNum
    logic.StoryEditorMgr:SaveReadingRecord(self.storyDetial.id,chapterID,payType,function(isOK)
        if not isOK then
            return
        end
        if payType == 1 then
            KeyNum = KeyNum - 1
            logic.cs.UserDataManager.UserData.KeyNum = KeyNum
            logic.cs.UserDataManager:ResetMoney(1, KeyNum)
            -- logic.cs.UserDataManager:ResetMoney(2, logic.cs.UserDataManager.choicesClothResultInfo.data.user_diamond)
        end

        logic.StoryEditorMgr:GetDialogList(self.storyDetial.id, chapterID,function(storyTable)
            --logic.cs.UINetLoadingMgr:Close()
            local DataDefine = logic.StoryEditorMgr.DataDefine
            local storyNodeRoot = DataDefine.t_StoryNode.Create(storyTable)
            --storyNodeRoot.name = self.storyDetial.title
            local uiView = logic.UIMgr:Open(logic.uiid.Story_Preview)
            uiView:SetData(self.storyDetial, chapterID, storyNodeRoot, true)
            self:Hide()
        end)
    end)
end

function UINextChapterView:ShowChargeTips(cost)
    local type = logic.cs.MyBooksDisINSTANCE:GameOpenUItype()
    if type == 1 then
        local userInfo = logic.cs.UserDataManager.userInfo
        if userInfo and userInfo.data and userInfo.data.userinfo.newpackage_status == 1 then
            local uiform = logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.FirstGigtGroup);
            local c = uiform:GetComponent(typeof(CS.FirstGigtGroup))
            c:GetType(1);
        end
    elseif type == 2 then
        logic.cs.MyBooksDisINSTANCE:VideoUI()
    else
        if logic.config.channel == Channel.Spain then
            local uiform = logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.ChargeTipsForm);
            local tipForm = uiform:GetComponent(typeof(CS.ChargeTipsForm))
            tipForm:Init(2, cost, cost * 0.99)
        else
            local uiform = logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.NewChargeTips);
            local tipForm = uiform:GetComponent(typeof(CS.NewChargeTips))
            tipForm:Init(2, cost, cost * 0.99)
        end
    end
end

return UINextChapterView
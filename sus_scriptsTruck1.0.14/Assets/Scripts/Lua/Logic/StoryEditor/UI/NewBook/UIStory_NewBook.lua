--[[
    主面板
]]
local BaseClass = core.Class
local UIView = core.UIView
local UIStory_NewBook = BaseClass("UIStory_NewBook", UIView)
local base = UIView
local uiid = logic.uiid

UIStory_NewBook.config = {
	ID = uiid.Story_NewBook,
	AssetName = 'UI/StoryEditorRes/UI/Canvas_Story_NewBook'
}

local UICatagoryItem = require('Logic/StoryEditor/UI/NewBook/UICatagoryItem')

function UIStory_NewBook:OnInitView()
    UIView.OnInitView(self)
    local root = self.uiform.transform
    self.uiBinding = root:GetComponent(typeof(CS.UIBinding))

    self.step1_gameObject = self.uiBinding:Get('step1_gameObject')
    self.step1_btnClose = self.uiBinding:Get('step1_btnClose',typeof(logic.cs.UITweenButton))
    self.step1_btnOK = self.uiBinding:Get('step1_btnOK',typeof(logic.cs.UITweenButton))
    self.step1_catagoryRoot = self.uiBinding:Get('catagoryRoot').transform
    self.step1_catagoryPfb = self.uiBinding:Get('catagoryPfb')
    self.step1_catagoryPfb:SetActiveEx(false)

    self.lbPanNameNumber = self.uiBinding:Get('lbPanNameNumber', typeof(logic.cs.Text))
    self.lbBookNameNumber = self.uiBinding:Get('lbBookNameNumber', typeof(logic.cs.Text))
    self.lbBookDescNumber = self.uiBinding:Get('lbBookDescNumber', typeof(logic.cs.Text))
    
    self.step1_penName = self.uiBinding:Get('step1_penName',typeof(logic.cs.InputField))
    self.step1_bookName = self.uiBinding:Get('step1_bookName',typeof(logic.cs.InputField))
    
    self.step1_bookDesc = self.uiBinding:Get('step1_bookDesc',typeof(logic.cs.InputField))
    self.btnClose = self.uiBinding:Get('btnClose',typeof(logic.cs.UITweenButton))
    

    self.step1_penName.onValueChanged:AddListener(function(text)
        local value = string.trim(text)
        local len = string.GetUtf8Len(value)
        self.lbPanNameNumber.text = string.format("%d/20",len)
    end)
    self.step1_bookName.onValueChanged:AddListener(function(text)
        local value = string.trim(text)
        local len = string.GetUtf8Len(value)
         self.lbBookNameNumber.text = string.format("%d/20",len)
    end)
    self.step1_bookDesc.onValueChanged:AddListener(function(text)
        local value = string.trim(text)
        local len = string.GetUtf8Len(value)
        self.lbBookDescNumber.text = string.format("%d/150",len)
    end)

    self.step1_btnClose.onClick:AddListener(function()
        self:OnExitClick()
    end)
    self.btnClose.onClick:AddListener(function()
        self:OnExitClick()
    end)
    self.step1_btnOK.onClick:AddListener(function()
        local len = 0
        local penName = string.trim(self.step1_penName.text)
        len = string.GetUtf8Len(penName)
        if len < 1 or len > 20 then
            logic.debug.LogError(penName)
            logic.cs.UITipsMgr:PopupTips("Enter a 1-20 words description.", false)
            return
        end

        local title = string.trim(self.step1_bookName.text)
        len = string.GetUtf8Len(title)
        if len < 1 or len > 20 then
            logic.debug.LogError(title)
            logic.cs.UITipsMgr:PopupTips("Enter a 1-20 words description.", false)
            return
        end
        
        local description = string.trim(self.step1_bookDesc.text)
        len = string.GetUtf8Len(description)
        if len < 1 or len > 150 then
            logic.debug.LogError(description)
            logic.cs.UITipsMgr:PopupTips("Enter a 1-150 words description.", false)
            return
        end

        if table.count(self.selectCatagorys) == 0 then
            logic.cs.UITipsMgr:PopupTips("Genres 1-3.", false)
            return
        end
        
        --if self.isNewBook then
            self:SendCreateBook(function(isNewBook, bookDetials)
                self.isNewBook = false
                self:Finish()
            end)
        --else
            --self:GotoStep2()
        --end
    end)
    
    self.step_privacy = self.uiBinding:Get('step_privacy')
    self.btnPrivacySure = self.uiBinding:Get('btnPrivacySure', typeof(logic.cs.UITweenButton))
    self.btnPrivacy = self.uiBinding:Get('btnPrivacy', typeof(logic.cs.UIToggle))
    self.btnPrivacySure.onClick:AddListener(function()
        if not self.btnPrivacy.isOn then
            return
        end
        logic.cs.PlayerPrefs.SetInt(logic.cs.UserDataManager.userInfo.data.userinfo.phoneimei..'_story_privacy',1)
        self.step_privacy:SetActiveEx(false)
    end)
end

function UIStory_NewBook:OnOpen()
    UIView.OnOpen(self)

    self.firstCreate = false
    if logic.cs.UserDataManager.userInfo.data.userinfo.writer_guide == 0 then
        local flag = logic.cs.PlayerPrefs.GetInt(logic.cs.UserDataManager.userInfo.data.userinfo.phoneimei..'_story_privacy',0)
        self.firstCreate = (flag == 0)
    end
    self.step_privacy:SetActiveEx(false)
    self:GotoStep1()
end

function UIStory_NewBook:OnExitClick()
    self:Finish()
end

function UIStory_NewBook:GotoStep1()
    self.step1_gameObject:SetActiveEx(true)
end

function UIStory_NewBook:GotoStep2()
    self.step1_gameObject:SetActiveEx(false)
end

---@param storyDetial StoryEditor_BookDetials
---@param onFinishCallback function(newDetials)
function UIStory_NewBook:SetData(storyDetial,onFinishCallback)
    self.onFinishCallback = onFinishCallback
    if storyDetial then
        self.isNewBook = false
        self.selectCatagorys = {}
        local array = string.split(storyDetial.tag,',')
        for i,v in pairs(array) do
            local val = tonumber(v)
            self.selectCatagorys[val] = true
        end
    else
        self.isNewBook = true
        ---@type StoryEditor_BookDetials
        storyDetial= {}
        storyDetial.id = 0
        storyDetial.title = ''
        storyDetial.writer_name = ''
        storyDetial.description = ''
        storyDetial.tag = ''
        storyDetial.read_count = 0
        storyDetial.favorite_count = 0
        storyDetial.word_count = 0
    end
    ---@type StoryEditor_BookDetials
    self.storyDetial = storyDetial
    self:RefreshUI()
    self.step_privacy:SetActiveEx(false)
    logic.cs.IGGAgreementMrg:OnRequestStatusCustomClick()
end

function UIStory_NewBook:RefreshUI()
    if not string.IsNullOrEmpty(self.storyDetial.title) then
        self.step1_bookName.text = self.storyDetial.title
    end
    if not string.IsNullOrEmpty(self.storyDetial.description) then
        self.step1_bookDesc.text = self.storyDetial.description
    end
    self.step1_penName.text = self.storyDetial.writer_name
    self:InitCatagoryList()
end


function UIStory_NewBook:Finish()
    self:__Close()
    if self.onFinishCallback then
        self.onFinishCallback(true,self.storyDetial)
    else
        if self.storyDetial and self.storyDetial.id ~= 0 then
            logic.StoryEditorMgr:EnterStoryEditorMode()
            logic.StoryEditorMgr:EnterBookDetials(self.storyDetial.id,function(storyDetial)
                local uiView = logic.UIMgr:Open(logic.uiid.Story_Detials)
                uiView:SetData(storyDetial)
            end)
        end
    end
end

function UIStory_NewBook:SendCreateBook(callback)
    local title = string.trim(self.step1_bookName.text)
    local description = self.step1_bookDesc.text
    local penName = self.step1_penName.text
    local tag = {}
    local cover_image = self.cover_image
    if string.IsNullOrEmpty(cover_image) then
        cover_image = self.storyDetial.cover_image
    end
    
    for k,v in pairs(self.selectCatagorys) do
        table.insert(tag,k);
    end
    local isNewBook = self.isNewBook
    local onSendBookCallback = function(result)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 200 then
            if isNewBook then
                self.storyDetial.id = json.data.book_id
                logic.debug.LogError('创建:'..self.storyDetial.title..',id='..json.data.book_id)
            end
            self.storyDetial.title = title
            self.storyDetial.description = description
            self.storyDetial.writer_name = penName
            self.storyDetial.tagArray = System.Collections.SortedDictionary.New()
            for i,item in pairs(tag) do
                self.storyDetial.tagArray:Set(item,true)
            end
            self.storyDetial.tag = string.join(tag,',')
            self.storyDetial.cover_image = cover_image
            callback(isNewBook, self.storyDetial)
        else
            logic.cs.UIAlertMgr:Show("TIPS",json.msg)
        end
    end
    if isNewBook then
        logic.gameHttp:StoryEditor_NewBook(penName, title,description,tag,cover_image,onSendBookCallback)
    else
        logic.gameHttp:StoryEditor_SaveBook(penName, self.storyDetial.id,title,description,tag,cover_image,onSendBookCallback)
    end
end



function UIStory_NewBook:InitCatagoryList()
    self.selectCatagorys = self.selectCatagorys or {}
    -------@type UICatagoryItem[]
    self.uiCatagorys = self.uiCatagorys or {}

    local DataDefine = logic.StoryEditorMgr.DataDefine
    local count = #DataDefine.BookTags
    for i=#self.uiCatagorys, count do
        local go = logic.cs.GameObject.Instantiate(self.step1_catagoryPfb,self.step1_catagoryRoot)
        --go:SetActiveEx(true)
        local uiItem = UICatagoryItem.New(go)
        table.insert(self.uiCatagorys, uiItem)
    end
    for i = 1, #self.uiCatagorys do
        local uiItem = self.uiCatagorys[i]
        local isOn = (i <= count)
        if isOn then
            uiItem.gameObject:SetActiveEx(true)
            uiItem:SetData(i)
            uiItem:SetOn(self.selectCatagorys[i] or false)
            uiItem.onValueChanged = function(item,isOn)
                self:OnCatagorySelect(item,isOn)
            end
        else
            uiItem.gameObject:SetActiveEx(false)
            uiItem.onValueChanged = nil
        end
    end
end

---@param catagoryItem UICatagoryItem
function UIStory_NewBook:OnCatagorySelect(catagoryItem,isOn)
    if isOn then
        if table.count(self.selectCatagorys) == 3 then
            catagoryItem:SetOn(false)
            return
        end
        self.selectCatagorys[catagoryItem.index] = true
        catagoryItem:SetOn(true)
    else
        self.selectCatagorys[catagoryItem.index] = nil
        catagoryItem:SetOn(false)
    end
end


return UIStory_NewBook
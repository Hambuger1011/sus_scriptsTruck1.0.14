--[[
    UIFavoritosForm
]]
local BaseClass = core.Class
local UIFavoritosForm = BaseClass("UIFavoritosForm")
local UIBookItem= require('Logic/StoryEditor/UI/Favoritos/UIBookItem')


function UIFavoritosForm:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self:InitView()
end

function UIFavoritosForm:IsDestroyed()
    return logic.IsNull(self.gameObject)
end

function UIFavoritosForm:InitView()
    self.uiBinding =  self.gameObject:GetComponent(typeof(logic.cs.UIBinding))
    self.myBookRoot = self.uiBinding:Get('myBookRoot').transform
    self.myBookItemPfb = self.uiBinding:Get('myBookItemPfb')
    self.myBookItemPfb:SetActiveEx(false)

    
    self.myReadBookRoot = self.uiBinding:Get('myReadBookRoot').transform
    self.myReadBookItemPfb = self.uiBinding:Get('myReadBookItemPfb')
    self.myReadBookItemPfb:SetActiveEx(false)

    
    local get = logic.cs.LuaHelper.GetComponent
    
    self.BookNewTf = self.uiBinding:Get('newBookItem').transform
    self.btnBookNew = get(self.BookNewTf, 'Body', typeof(logic.cs.UITweenButton))
    self.btnBookNew.onClick:AddListener(function()
        logic.cs.AudioManager:PlayTones(logic.cs.AudioTones.dialog_choice_click)
        self:OnNewBookClick()
    end)

    self.btnCreate = self.transform.parent:Find('ComunidadScrollView/AnchorGo/btnCreate'):GetComponent(typeof(logic.cs.UITweenButton))
    self.btnCreate.onClick:AddListener(function()
        logic.cs.EventDispatcher.Dispatch(logic.cs.EventEnum.SwitchComuniada, 1)
        local uiView = logic.UIMgr:Open(logic.uiid.Story_NewBook)
        uiView:SetData(self.storyDetial,nil,true)
    end)

    ---@type StoryEditor_BookDetial[]
    self.myBookList = {}

    ---@type StoryEditor_BookDetial[]
    self.myReadBookList = {}
end

function UIFavoritosForm:SetActive(isOn)
    self.gameObject:SetActiveEx(isOn)
    if isOn then
        self:RefreshBookList()
    end
end

function UIFavoritosForm:RefreshBookList()
    if self:IsDestroyed() then
        return
    end
    logic.gameHttp:StoryEditor_GetMyBookList(nil,function(result)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 200 then
            self.myBookList = json.data
            self:InitMyBookList()
        else
            logic.cs.UIAlertMgr:Show("TIPS",json.msg)
        end
    end)


    logic.gameHttp:StoryEditor_GetMyReadBookList(function(result)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 200 then
            self.myReadBookList = json.data
            self:InitMyReadBookList()
        else
            logic.cs.UIAlertMgr:Show("TIPS",json.msg)
        end
    end)
end

function UIFavoritosForm:InitMyBookList()
    if self:IsDestroyed() then
        return
    end
    local bookDataList = self.myBookList
    self.uiMyBooks = self.uiMyBooks or {}
    local count = table.length(bookDataList)
    for i=#self.uiMyBooks, count do
        local go = logic.cs.GameObject.Instantiate(self.myBookItemPfb,self.myBookRoot)
        --go:SetActiveEx(true)
        local uiItem = UIBookItem.New(go)
        table.insert(self.uiMyBooks, uiItem)
        uiItem.onClick = function()
            logic.StoryEditorMgr:EnterBookDetials(uiItem.storyDetial.id, function(storyDetial)
                local uiView = logic.UIMgr:Open(logic.uiid.Story_Detials)
                uiView:SetData(storyDetial)
            end)
        end
    end
    for i = 1, #self.uiMyBooks do
        local uiItem = self.uiMyBooks[i]
        local isOn = (i <= count)
        if isOn then
            uiItem.gameObject:SetActiveEx(true)
            uiItem:SetData(bookDataList[i])
        else
            uiItem.gameObject:SetActiveEx(false)
        end
    end
    self.BookNewTf:SetAsLastSibling()
end

function UIFavoritosForm:OnNewBookClick()
    local uiView = logic.UIMgr:Open(logic.uiid.Story_NewBook)
    uiView:SetData(self.storyDetial)
end




function UIFavoritosForm:InitMyReadBookList()
    if self:IsDestroyed() then
        return
    end
    local bookDataList = self.myReadBookList
    self.uiMyReadBooks = self.uiMyReadBooks or {}
    local count = table.length(bookDataList)
    for i=#self.uiMyReadBooks, count do
        local go = logic.cs.GameObject.Instantiate(self.myReadBookItemPfb,self.myReadBookRoot)
        --go:SetActiveEx(true)
        local uiItem = UIBookItem.New(go)
        table.insert(self.uiMyReadBooks, uiItem)
        uiItem.onClick = function()
            --logic.StoryEditorMgr:EnterBookDetials(uiItem.storyDetial.book_id, false)
            logic.StoryEditorMgr:ReadingOtherChapter(uiItem.storyDetial.book_id, uiItem.storyDetial.chapter_number,function()
                logic.StoryEditorMgr:BackToMainClick()
            end)
        end
    end
    for i = 1, #self.uiMyReadBooks do
        local uiItem = self.uiMyReadBooks[i]
        local isOn = (i <= count)
        if isOn then
            uiItem.gameObject:SetActiveEx(true)
            uiItem:SetData(bookDataList[i])
        else
            uiItem.gameObject:SetActiveEx(false)
        end
    end
    self.BookNewTf:SetAsLastSibling()
end

return UIFavoritosForm
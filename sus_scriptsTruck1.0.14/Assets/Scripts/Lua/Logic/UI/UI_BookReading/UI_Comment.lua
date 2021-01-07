local BaseClass = core.Class
local UIView = core.UIView
---@class UI_Comment
local UI_Comment = BaseClass("UI_Comment", UIView)

local uiid = logic.uiid

local UIBubbleItem = require('Logic/StoryEditor/UI/Preview/BubbleItem/UIBubbleItem')
local UIDialogList = require('Logic/StoryEditor/UI/Utils/UIDialogList')
local UIBubbleBox = require('Logic/StoryEditor/UI/Preview/BubbleItem/UIBubbleBox')

UI_Comment.config = {
    ID = uiid.Comment,
    AssetName = 'UI/BookReading/Canvas_Comment'
}

local TopSelectType = {
    Newest = 0, -- 最新
    Hot = 1, -- 热门
}

local TopSelect = -1
local pageNum = 1

function UI_Comment:BackToMainClick()
    local hideFormUI = {
        logic.cs.UIFormName.MainFormTop,
        logic.cs.UIFormName.ComuniadaMas,
        logic.cs.UIFormName.Busqueda,
    }
    for i, uuid in pairs(hideFormUI) do
        local uiForm = logic.cs.CUIManager:GetForm(uuid)
        if not logic.IsNull(uiForm) then
            uiForm:Appear()
        end
    end

    --界面
    local uiform = logic.UIMgr:GetView(logic.uiid.UIMainDownForm);
    if (uiform) then
        uiform.uiform:Appear();
    end

end

function UI_Comment:EnterStoryComment()
    local hideFormUI = {
        logic.cs.UIFormName.MainFormTop,
        logic.cs.UIFormName.ComuniadaMas,
        logic.cs.UIFormName.Busqueda,
    }
    for i, uuid in pairs(hideFormUI) do
        local uiForm = logic.cs.CUIManager:GetForm(uuid)
        if not logic.IsNull(uiForm) then
            uiForm:Hide()
        end
    end

    --隐藏界面
    local uiform = logic.UIMgr:GetView(logic.uiid.UIMainDownForm);
    if (uiform) then
        uiform.uiform:Hide();
    end
end

function UI_Comment:OnInitView()
    self.bookId = logic.bookReadingMgr.selectBookId
    UIView.OnInitView(self)
    local root = self.uiform.transform
    self.uiBinding = root:GetComponent(typeof(CS.UIBinding))
    self.CommentItem = self.uiBinding:Get('CommentItem')
    self.CommentItem:SetActiveEx(false)
    self.itemListView = UIDialogList.New(self.uiBinding:Get('Layout'), 0)
    self.paddingBottom = self.itemListView:GetPaddingBottom()
    self.itemListView.onCreateItem = function(index, data, reset)
        return self:OnCreateItem(index, data, reset)
    end
    self.btnClose = self.uiBinding:Get('btnClose', typeof(logic.cs.UITweenButton))
    self.btnClose.onClick:RemoveAllListeners()
    self.btnClose.onClick:AddListener(function()
        self:OnExitClick()
    end)

    self.hotButton = self.uiBinding:Get('HotButton', typeof(logic.cs.UITweenButton))
    self.newestButton = self.uiBinding:Get('NewestButton', typeof(logic.cs.UITweenButton))
    self.hotCheckmark = self.hotButton.transform:Find('Text/Checkmark').gameObject
    self.newestCheckmark = self.newestButton.transform:Find('Text/Checkmark').gameObject

    self.hotButton.onClick:RemoveAllListeners()
    self.hotButton.onClick:AddListener(function()
        if TopSelect == TopSelectType.Hot then
            return
        end
        TopSelect = TopSelectType.Hot
        pageNum = 1
        self:UpdateComment(pageNum)
    end)

    self.newestButton.onClick:RemoveAllListeners()
    self.newestButton.onClick:AddListener(function()
        if TopSelect == TopSelectType.Newest then
            return
        end
        TopSelect = TopSelectType.Newest
        pageNum = 1
        self:UpdateComment(pageNum)
    end)

    self.inputField = self.uiBinding:Get('InputField', typeof(logic.cs.InputField))
    self.inputField.shouldHideMobileInput = true
    --self.input.onEndEdit:AddListener(function(val)
    --    logic.debug.Log('end:'..val)
    --end)

    self.headImage = self.uiBinding:Get('HeadImage', typeof(logic.cs.Image))
    self.HeadFrame = self.uiBinding:Get('HeadFrame', typeof(logic.cs.Image))


    --显示头像
    GameHelper.luaShowDressUpForm(-1, self.headImage, DressUp.Avatar, 1001);
    --加载头像框
    GameHelper.luaShowDressUpForm(-1, self.HeadFrame, DressUp.AvatarFrame, 2001);

    self.submitBtn = self.uiBinding:Get('SubmitBtn', typeof(logic.cs.UITweenButton))
    self.submitBtn.onClick:RemoveAllListeners()
    self.submitBtn.onClick:AddListener(function()
        local content = self.inputField.text
        if #content == 0 then
            return
        end
        logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.AddBookComment, "", "", tostring(self.bookId))
        logic.gameHttp:CreateBookComment(tonumber(self.bookId), content, function(result)
            local json = core.json.Derialize(result)
            local code = tonumber(json.code)
            if code == 200 then
                self.inputField.text = ""
                pageNum = 1
                self:UpdateComment(pageNum)
            else
                logic.cs.UIAlertMgr:Show("TIPS", json.msg)
            end
        end)
    end)

    self.ScrollView = self.uiBinding:Get('ScrollView', typeof(logic.cs.ScrollRect))
    self.BookBannerBG = self.uiBinding:Get('BookBannerBG', typeof(logic.cs.Image))
    logic.cs.ABSystem.ui:DownloadBanner(self.bookId, function(id, refCount)
        if (self.BookBannerBG == nil) then
            refCount:Release();
            return ;
        end
        if (self.bookId ~= id) then
            refCount:Release();
            return ;
        end
        self.BookBannerBG.sprite = refCount:GetObject();
        self.BookBannerBG:SetNativeSize()
        local cs_layout = self.uiBinding:Get('Layout').gameObject:GetComponent(typeof(logic.cs.LuaLayoutGroup))
        cs_layout.padding.top = self.BookBannerBG.transform.rect.height
        cs_layout:SetLayoutVertical()
    end)

    TopSelect = TopSelectType.Hot
    pageNum = 1
    self:UpdateComment(pageNum)

    local uiform1 = root:GetComponent("CUIForm")
    local safeArea = logic.cs.ResolutionAdapter:GetSafeArea()
    local safeAreaHeight = uiform1:yPixel2View(safeArea.y)
    if safeAreaHeight and safeAreaHeight > 0 then
        local TopTile = root:Find('body/Top').transform
        local Center = root:Find('body/Center').transform

        local pos = TopTile.anchoredPosition
        pos.y = pos.y - safeAreaHeight
        TopTile.anchoredPosition = pos

        local w = Center.rect.width
        local h = Center.rect.height - safeAreaHeight
        Center.anchorMax = core.Vector2.New(0.5, 0.5);
        Center.anchorMin = core.Vector2.New(0.5, 0.5);
        Center.pivot = core.Vector2.New(0.5, 0.5);
        Center.sizeDelta = { x = w, y = h }
        local pos1 = Center.anchoredPosition
        pos1.y = pos1.y - safeAreaHeight * 0.5
        Center.anchoredPosition = pos1
    end

end

function UI_Comment:UpdateComment(_pageNum)
    logic.gameHttp:GetBookCommentList(tonumber(self.bookId), _pageNum, TopSelect, function(result)
        logic.debug.Log("----GetBookCommentList---->" .. result)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 200 then
            self.newestCheckmark:SetActiveEx(TopSelect == TopSelectType.Newest)
            self.hotCheckmark:SetActiveEx(TopSelect == TopSelectType.Hot)
            if _pageNum == 1 then
                self:ClearItem()
                self.itemDataList = json.data.data
                for i = 1, #self.itemDataList do
                    if i == #self.itemDataList then
                        self.itemDataList[i].pageNum = pageNum
                        self.itemDataList[i].GetNextPage = function()
                            pageNum = pageNum + 1
                            self:UpdateComment(pageNum)
                        end
                    end
                    self:AddUIItem(false, false, false)
                end
                self.itemListView:MoveToIndex(1)
                self.ScrollView.verticalNormalizedPosition = 1;
            else
                local newData = json.data.data
                for i = 1, #newData do
                    if i == #newData then
                        newData[i].pageNum = pageNum
                        newData[i].GetNextPage = function()
                            pageNum = pageNum + 1
                            self:UpdateComment(pageNum)
                        end
                    end
                    table.insert(self.itemDataList, newData[i])
                    self:AddUIItem(true, false, false)
                end
            end

        else
            logic.cs.UIAlertMgr:Show("TIPS", json.msg)
            self:OnExitClick()
        end
    end)
end

function UI_Comment:AddUIItem(newInstance, useTween, scrollToBottom)
    local height = self.itemListView:AddVirtualItem(UIBubbleBox.BoxType.CommentItem, newInstance)
    self.itemListView:SetHeight(height, scrollToBottom, useTween)
    self:RefreshDialogList()
end

function UI_Comment:RefreshDialogList()
    self.itemListView:MarkDirty()
    --self.list:ReCalculateLayout()
end

function UI_Comment:OnCreateItem(index, itemData)
    local go = logic.cs.GameObject.Instantiate(self.CommentItem, self.itemListView.transform)
    go:SetActiveEx(true)

    local item = UIBubbleItem.New(go, UIBubbleBox.BoxType.CommentItem)
    item.GetBubbleDataByIndex = function(index)
        return self.itemDataList[index]
    end
    return item
end

function UI_Comment:ClearItem()
    self.itemListView:ClearItem()
    self.itemListView:SetPaddingBottom(self.paddingBottom)
    self.itemDataList = {}
end

function UI_Comment:OnOpen()
    self:EnterStoryComment()
    UIView.OnOpen(self)
end

function UI_Comment:OnClose()
    local uiform = logic.UIMgr:GetView2(logic.uiid.UIActivityForm);
    if(uiform)then
        uiform:UpdateTasks();
    end
    self:BackToMainClick()
    UIView.OnClose(self)
end

function UI_Comment:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end

return UI_Comment
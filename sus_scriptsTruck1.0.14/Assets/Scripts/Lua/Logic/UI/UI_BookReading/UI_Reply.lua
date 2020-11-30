local BaseClass = core.Class
local UIView = core.UIView
---@class UI_Reply
local UI_Reply = BaseClass("UI_Reply", UIView)
local base = UIView

local uiid = logic.uiid
local UIBubbleEvent = CS.UI.UIBubbleEvent

local BubbleData = require('Logic/StoryEditor/UI/Preview/BubbleItem/BubbleData')
local UIBubbleItem = require('Logic/StoryEditor/UI/Preview/BubbleItem/UIBubbleItem')
local UIDialogList = require('Logic/StoryEditor/UI/Utils/UIDialogList')
local UIBubbleBox = require('Logic/StoryEditor/UI/Preview/BubbleItem/UIBubbleBox')

local DataDefine = require('Logic/StoryEditor/Data/DataDefine')
local StoryData = require('Logic/StoryEditor/Data/StoryData')
local EBubbleType = DataDefine.EBubbleType
local EBubbleBoxType = DataDefine.EBubbleBoxType

local Item = core.Class("ReplyItem.Item")

UI_Reply.config = {
	ID = uiid.Reply,
	AssetName = 'UI/BookReading/Canvas_Reply'
}

local pageNum = 1

function UI_Reply:OnInitView()
    self.bookId = logic.bookReadingMgr.selectBookId
    self.toReplyId = 0
    UIView.OnInitView(self)
    local root = self.uiform.transform
    self.uiBinding = root:GetComponent(typeof(CS.UIBinding))
    self.ReplyItem = self.uiBinding:Get('ReplyItem')
    self.replyBg = self.uiBinding:Get('ReplyBg').transform
    self.submitBtn = self.uiBinding:Get('SubmitBtn', typeof(logic.cs.UITweenButton))
    self.inputField = self.uiBinding:Get('InputField', typeof(logic.cs.InputField))
    self.btnClose = self.uiBinding:Get('btnClose', typeof(logic.cs.UITweenButton))

    self.headImage = self.uiBinding:Get('HeadImage', typeof(logic.cs.Image))
    self.HeadFrame = self.uiBinding:Get('HeadFrame', typeof(logic.cs.Image))

    self.center = self.uiBinding:Get('Center').transform
    self.itemListView = UIDialogList.New(self.uiBinding:Get('Layout'),0)
    
    self.ReplyItem:SetActiveEx(false)
    
    self.paddingBottom = self.itemListView:GetPaddingBottom()
    self.itemListView.onCreateItem = function(index, data, reset)
        return self:OnCreateItem(index, data, reset)
    end
    
    self.btnClose.onClick:RemoveAllListeners()
    self.btnClose.onClick:AddListener(function()
        self:OnExitClick()
    end)

    --加载头像
    GameHelper.luaShowDressUpForm(-1,self.headImage,DressUp.Avatar,1001);
    --加载头像框
    GameHelper.luaShowDressUpForm(-1,self.HeadFrame,DressUp.AvatarFrame,2001);



    self.inputField.shouldHideMobileInput = true
    
    --self.listenerCallback = function(nickName)
    --    logic.cs.EventDispatcher.RemoveMessageListener(logic.cs.EventEnum.DialogDisplaySystem_PlayerMakeChoice,self.callback)
    --    if nickName then
    --        
    --    end
    --end
    --logic.cs.EventDispatcher.AddMessageListener("send to someBody", self.listenerCallback)
    self.submitBtn.onClick:RemoveAllListeners()
    self.submitBtn.onClick:AddListener(function()
        local content = self.inputField.text
        if #content == 0 then
            return
        end
        logic.gameHttp:CreateBookCommentReply(self.commentData.id,content,self.toReplyId,function(result)
            local json = core.json.Derialize(result)
            local code = tonumber(json.code)
            if code == 200 then
                self.inputField.text = ""
                pageNum = 1
                self:UpdateReply(pageNum)
            else
                logic.cs.UIAlertMgr:Show("TIPS",json.msg)
            end
        end)
    end)

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
        local h = Center.rect.height-safeAreaHeight
        Center.anchorMax = core.Vector2.New(0.5, 0.5);
        Center.anchorMin = core.Vector2.New(0.5, 0.5);
        Center.pivot = core.Vector2.New(0.5, 0.5);
        Center.sizeDelta =  {x=w,y=h}
        local pos1 = Center.anchoredPosition
        pos1.y = pos1.y - safeAreaHeight*0.5
        Center.anchoredPosition = pos1
    end
    
    self:InitComment(function()
        pageNum = 1
        self:UpdateReply(pageNum)
    end)
end

function UI_Reply:UpdateReply(_pageNum)
    logic.gameHttp:GetBookCommentReplyList(self.commentData.id, _pageNum,function(result)
        logic.debug.Log("----GetBookCommentReplyList---->" .. result)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 200 then
            if _pageNum == 1 then
                self:ClearItem()
                self.itemDataList =  json.data.data
                for i = 1 , #self.itemDataList do
                    if i == #self.itemDataList then
                        self.itemDataList[i].pageNum = pageNum
                        self.itemDataList[i].GetNextPage = function()
                            pageNum = pageNum + 1
                            self:UpdateReply(pageNum)
                        end
                    end
                    self.itemDataList[i].replyBtnClick = function(id,name)
                        logic.cs.EventSystem.current:SetSelectedGameObject(self.inputField.gameObject)
                        if self.toReplyId == id then
                            return
                        end
                        self.toReplyId = id
                        self.inputField.text = "@"..name..":"
                    end
                    self:AddUIItem(false,false,false)
                end
                self.itemListView:MoveToIndex(1)
            else
                local newData = json.data.data
                for i = 1 , #newData do
                    if i == #newData then
                        newData[i].pageNum = pageNum
                        newData[i].GetNextPage = function()
                            pageNum = pageNum + 1
                            self:UpdateReply(pageNum)
                        end
                    end
                    newData[i].replyBtnClick = function(id,name)
                        logic.cs.EventSystem.current:SetSelectedGameObject(self.inputField.gameObject)
                        if self.toReplyId == id then
                            return
                        end
                        self.toReplyId = id
                        self.inputField.text = "@"..name..":"
                    end
                    table.insert(self.itemDataList,newData[i])
                    self:AddUIItem(true,false,false)
                end
            end
        else
            logic.cs.UIAlertMgr:Show("TIPS",json.msg)
            self:OnExitClick()
        end
    end)
end

function UI_Reply:FormatNum(_num)
    _num = tonumber(_num)
    if _num >= 1000000 then
        _num = string.format("%0.1f", tonumber(_num) / 1000000) .. "m"
    elseif _num >= 1000 then
        _num = string.format("%0.1f", tonumber(_num) / 1000) .. "k"
    end
    return _num
end

function UI_Reply:InitComment(callback)
    local Data = self.commentData
    local comment = self.uiBinding:Get('Comment').transform
    local uiBinding = comment:GetComponent(typeof(CS.UIBinding))
    local line = uiBinding:Get('Line').transform
    local HeadImage = uiBinding:Get('HeadImage', typeof(logic.cs.Image))
    local HeadFrame = uiBinding:Get('HeadFrame', typeof(logic.cs.Image))
    local nameText = uiBinding:Get('Name',  typeof(logic.cs.Text))
    local contentText = uiBinding:Get('ContentText',  typeof(logic.cs.Text))
    local dateText = uiBinding:Get('Date',  typeof(logic.cs.Text))
    local likeText = uiBinding:Get('LikeText',  typeof(logic.cs.Text))
    local unLikeText = uiBinding:Get('UnLikeText',  typeof(logic.cs.Text))
    local replyText = uiBinding:Get('ReplyText',  typeof(logic.cs.Text))
    local likeBtn = uiBinding:Get('LikeBtn',  typeof(logic.cs.UITweenButton))
    local unLikeBtn = uiBinding:Get('UnLikeBtn',  typeof(logic.cs.UITweenButton))
    local replyBtn = uiBinding:Get('ReplyBtn',  typeof(logic.cs.UITweenButton))

    --headImage.sprite = CS.ResourceManager.Instance:GetUISprite("ProfileForm/img_renwu"..Data.face_icon);
    nameText.text = Data.nickname
    local contentTxt = Data.content
    if Data.is_self == 0 then
        contentTxt = logic.cs.PluginTools:ReplaceBannedWords(contentTxt)
    end
    contentText.text = contentTxt
    contentText.transform:GetComponent("ContentSizeFitter"):SetLayoutVertical()
    dateText.text = os.date("%m-%d %H:%M", Data.create_time)
    likeText.text = self:FormatNum(Data.agree_count)
    unLikeText.text = self:FormatNum(Data.disagree_count)
    replyText.text = Data.reply_count

    --加载头像
    GameHelper.luaShowDressUpForm(Data.avatar,HeadImage,DressUp.Avatar,1001);
    --加载头像框
    GameHelper.luaShowDressUpForm(Data.avatar_frame,HeadFrame,DressUp.AvatarFrame,2001);



    local rootSize = contentText.transform.rect.size
    local pos = contentText.transform.anchoredPosition
    local linePos = line.anchoredPosition
    local height = Mathf.Abs(rootSize.y) + Mathf.Abs(pos.y) + Mathf.Abs(linePos.y)
    self.boxSize = core.Vector2.New(750, height)
    local centerSize = self.center.rect.size
    self.replyBg.transform:SetInsetAndSizeFromParentEdge(logic.cs.RectTransform.Edge.Top, height , centerSize.y - height)


    likeBtn.onClick:RemoveAllListeners()
    likeBtn.onClick:AddListener(function()
        logic.gameHttp:BookCommentSetAgree(Data.id,function(result)
            local json = core.json.Derialize(result)
            local code = tonumber(json.code)
            if code == 200 then
                logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.ThumbUpRecord,"","",tostring(logic.bookReadingMgr.selectBookId))
                Data.agree_count = json.data.agree_count
                likeText.text = self:FormatNum(Data.agree_count)
                Data.disagree_count = json.data.disagree_count
                unLikeText.text = self:FormatNum(Data.disagree_count)
            else
                logic.cs.UIAlertMgr:Show("TIPS",json.msg)
            end
        end)
    end)

    unLikeBtn.onClick:RemoveAllListeners()
    unLikeBtn.onClick:AddListener(function()
        logic.gameHttp:BookCommentSetDisagree(Data.id,function(result)
            local json = core.json.Derialize(result)
            local code = tonumber(json.code)
            if code == 200 then
                logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.CancelThumbUp,"","",tostring(logic.bookReadingMgr.selectBookId))
                Data.agree_count = json.data.agree_count
                Data.disagree_count = json.data.disagree_count
                likeText.text = self:FormatNum(Data.agree_count)
                unLikeText.text = self:FormatNum(Data.disagree_count)
            else
                logic.cs.UIAlertMgr:Show("TIPS",json.msg)
            end
        end)
    end)

    replyBtn.onClick:RemoveAllListeners()
    replyBtn.onClick:AddListener(function()
        logic.cs.EventSystem.current:SetSelectedGameObject(self.inputField.gameObject)
        if self.toReplyId == 0 then
            return
        end
        self.toReplyId = 0
        self.inputField.text = ""
    end)
    
    if callback then
        callback()
    end
end

function UI_Reply:SetCommentData(data)
    self.commentData = data
end

function UI_Reply:AddUIItem(newInstance,useTween,scrollToBottom)
    local height = self.itemListView:AddVirtualItem(UIBubbleBox.BoxType.ReplyItem,newInstance)
    self.itemListView:SetHeight(height, scrollToBottom, useTween)
    self:RefreshDialogList()
end

function UI_Reply:RefreshDialogList()
    self.itemListView:MarkDirty()
end

function UI_Reply:OnCreateItem(index, itemData)
    local go = logic.cs.GameObject.Instantiate(self.ReplyItem,self.itemListView.transform)
    go:SetActiveEx(true)

    local item = UIBubbleItem.New(go,UIBubbleBox.BoxType.ReplyItem)
    item.GetBubbleDataByIndex = function(index)
        return self.itemDataList[index]
    end
    return item
end

function UI_Reply:ClearItem()
    self.itemListView:ClearItem()
    self.itemListView:SetPaddingBottom(self.paddingBottom)
    self.itemDataList = {}
end

function UI_Reply:OnOpen()
    UIView.OnOpen(self)
end

function UI_Reply:OnClose()
    UIView.OnClose(self)
end

function UI_Reply:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end

return UI_Reply
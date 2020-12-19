local BaseClass = core.Class
local UIView = core.UIView
---@class UIEmailInfoForm
local UIEmailInfoForm = BaseClass("UIEmailInfoForm", UIView)
local base = UIView

local uiid = logic.uiid

UIEmailInfoForm.config = {
    ID = uiid.UIEmailInfoForm,
    AssetName = 'UI/Resident/UI/UIEmailInfoForm'
}
local hit = nil;

local function SetSafeAreaHeight(Center, safeAreaHeight)
    local w = Center.rect.width
    local h = Center.rect.height - safeAreaHeight
    Center.anchorMax = core.Vector2.New(0.5, 1);
    Center.anchorMin = core.Vector2.New(0.5, 1);
    Center.pivot = core.Vector2.New(0.5, 1);
    Center.sizeDelta = { x = w, y = h }
    local pos1 = Center.anchoredPosition
    pos1.y = pos1.y - safeAreaHeight * 1.5
    Center.anchoredPosition = pos1
end

function UIEmailInfoForm:OnInitView()
    self.bookId = logic.bookReadingMgr.selectBookId
    UIView.OnInitView(self)
    local root = self.uiform.transform
    self.uiBinding = root:GetComponent(typeof(CS.UIBinding))
    self.btnClose = self.uiBinding:Get('BtnClose', typeof(logic.cs.Button))
    self.receiveButton = self.uiBinding:Get('ReceiveButton', typeof(logic.cs.Button))
    self.time = self.uiBinding:Get('Time', typeof(logic.cs.Text))
    self.title = self.uiBinding:Get('Title', typeof(logic.cs.Text))
    self.content = self.uiBinding:Get('Content', typeof(logic.cs.Text))
    self.gitBg = self.uiBinding:Get('GitBg').gameObject
    self.keyText = self.uiBinding:Get('KeyText', typeof(logic.cs.Text))
    self.dimandText = self.uiBinding:Get('DimandText', typeof(logic.cs.Text))
    self.btnText = self.uiBinding:Get('BtnText', typeof(logic.cs.Text))
    self.picture = self.uiBinding:Get('Picture', typeof(logic.cs.Image))
    self.pictureBtn = self.picture.transform:GetComponent(typeof(logic.cs.Button))
    self.type4 = self.uiBinding:Get('Type4').gameObject
    self.type2 = self.uiBinding:Get('Type2').gameObject
    self.gitItemBg = self.uiBinding:Get('GitItemBg').gameObject
    self.timeText = self.uiBinding:Get('TimeText', typeof(logic.cs.Text))
    self.bookText = self.uiBinding:Get('BookText', typeof(logic.cs.Text))
    self.bodyText = self.uiBinding:Get('BodyText', typeof(logic.cs.Text))
    self.contentText = self.uiBinding:Get('ContentText', typeof(logic.cs.Text))
    self.nameText = self.uiBinding:Get('NameText', typeof(logic.cs.Text))
    self.headImage = self.uiBinding:Get('HeadImage', typeof(logic.cs.Image))
    self.HeadFrame = self.uiBinding:Get('HeadFrame', typeof(logic.cs.Image))
    self.replyButton = self.uiBinding:Get('ReplyButton', typeof(logic.cs.Button))
    self.submitBtn = self.uiBinding:Get('SubmitBtn', typeof(logic.cs.Button))
    self.inputMask = self.uiBinding:Get('InputMask', typeof(logic.cs.Button))
    self.inputField = self.uiBinding:Get('InputField', typeof(logic.cs.InputField))
    self.keyItem = self.uiBinding:Get('KeyItem').gameObject
    self.dimandItem = self.uiBinding:Get('DimandItem').gameObject

    local Data = self.emailData

    if Data.msg_type == 1 then
        self.time.text = Data.createtime
        self.title.text = Data.title
        self.content.text = string.gsub(Data.content, '\\n', '\n')

        if Data.email_pic and #Data.email_pic > 0 then
            logic.cs.ABSystem.ui:DownloadImage(Data.email_pic, function(url, refCount)
                if Data.email_pic ~= url then
                    refCount:Release()
                    return
                end
                local sprite = refCount:GetObject()
                self.picture.sprite = sprite
                self.picture:SetNativeSize()
            end)
            self.picture.gameObject:SetActiveEx(true)
            if Data.email_url and #Data.email_url > 0 then
                self.pictureBtn.onClick:RemoveAllListeners()
                self.pictureBtn.onClick:AddListener(function()
                    logic.cs.Application.OpenURL(Data.email_url);
                end)
            end
        else
            self.picture.gameObject:SetActiveEx(false)
        end

        self.type2:SetActiveEx(true)
        self.type4:SetActiveEx(false)
        self.gitBg:SetActiveEx(false)
    end

    if Data.msg_type == 2 then
        self.time.text = Data.createtime
        self.title.text = Data.title
        self.content.text = string.gsub(Data.content, '\\n', '\n')

        if Data.price_bkey and tonumber(Data.price_bkey) > 0 then
            self.keyText.text = "x" .. Data.price_bkey
        else
            self.keyItem:SetActiveEx(false)
        end
        if Data.price_diamond and tonumber(Data.price_diamond) > 0 then
            self.dimandText.text = "x" .. Data.price_diamond
        else
            self.dimandItem:SetActiveEx(false)
        end
        self.btnText.text = "RECEIVE"

        if Data.price_status == 1 then
            self.receiveButton.gameObject:SetActiveEx(false)
        end

        if Data.email_pic and #Data.email_pic > 0 then
            logic.cs.ABSystem.ui:DownloadImage(Data.email_pic, function(url, refCount)
                if Data.email_pic ~= url then
                    refCount:Release()
                    return
                end
                local sprite = refCount:GetObject()
                self.picture.sprite = sprite
                self.picture:SetNativeSize()
            end)
            self.picture.gameObject:SetActiveEx(true)
            if Data.email_url and #Data.email_url > 0 then
                self.pictureBtn.onClick:RemoveAllListeners()
                self.pictureBtn.onClick:AddListener(function()
                    logic.cs.Application.OpenURL(Data.email_url);
                end)
            end
        else
            self.picture.gameObject:SetActiveEx(false)
        end

        self.receiveButton.onClick:RemoveAllListeners()
        self.receiveButton.onClick:AddListener(function()
            logic.gameHttp:AchieveMsgPrice(Data.msgid, function(result)
                local json = core.json.Derialize(result)
                local code = tonumber(json.code)
                if code == 200 then
                    logic.cs.UserDataManager:ResetMoney(1, tonumber(json.data.bkey))
                    logic.cs.UserDataManager:ResetMoney(2, tonumber(json.data.diamond))
                    self.receiveButton.gameObject:SetActiveEx(false)

                    hit:SetActiveEx(false)
                    --CS.Dispatcher.dispatchEvent(CS.EventEnum.RequestRedPoint);
                    GameController.MainFormControl:RedPointRequest();
                    if(self.emailItem)then
                        Data.price_status = 1
                        Data.status=1;
                        self.emailItem:SetData(Data);
                    end
                else
                    logic.cs.UIAlertMgr:Show("TIPS", json.msg)
                end
            end)
        end)

        self.gitItemBg:SetActiveEx(true)
        self.gitBg:SetActiveEx(true)

        self.type2:SetActiveEx(true)
        self.type4:SetActiveEx(false)
    else
        self.gitBg:SetActiveEx(false)
    end

    if Data.msg_type == 3 then
        self.time.text = Data.createtime
        self.title.text = Data.title
        self.content.text = string.gsub(Data.content, '\\n', '\n')

        if Data.button_name and #Data.button_name > 0 then
            self.btnText.text = Data.button_name
        else
            self.btnText.text = "RECEIVE"
        end

        if Data.email_pic and #Data.email_pic > 0 then
            logic.cs.ABSystem.ui:DownloadImage(Data.email_pic, function(url, refCount)
                if Data.email_pic ~= url then
                    refCount:Release()
                    return
                end
                local sprite = refCount:GetObject()
                self.picture.sprite = sprite
                self.picture:SetNativeSize()
            end)
            self.picture.gameObject:SetActiveEx(true)
            if Data.email_url and #Data.email_url > 0 then
                self.pictureBtn.onClick:RemoveAllListeners()
                self.pictureBtn.onClick:AddListener(function()
                    logic.cs.Application.OpenURL(Data.email_url);
                end)
            end
        else
            self.picture.gameObject:SetActiveEx(false)
        end

        self.receiveButton.onClick:RemoveAllListeners()
        self.receiveButton.onClick:AddListener(function()
            logic.cs.Application.OpenURL(Data.button_link);
        end)

        self.gitItemBg:SetActiveEx(false)
        self.gitBg:SetActiveEx(true)

        self.type2:SetActiveEx(true)
        self.type4:SetActiveEx(false)
    end

    if Data.msg_type == 4 then
        self.timeText.text = Data.createtime
        self.bookText.text = Data.book_name
        self.bodyText.text = "<b><color=#101010>" .. Data.replied_nickname .. ": </color></b>" .. Data.replied_content
        local contentTxt = Data.comment_content
        if Data.comment_is_self == 0 then
            contentTxt = logic.cs.PluginTools:ReplaceBannedWords(contentTxt)
        end
        self.contentText.text = contentTxt

        local co = coroutine.start(function()
            coroutine.wait(0.1)
            logic.cs.PluginTools:ContentSizeFitterRefresh(self.bodyText.transform);
            logic.cs.PluginTools:ContentSizeFitterRefresh(self.contentText.transform);
        end)
        self.nameText.text = Data.comment_nickname

        --加载头像
        GameHelper.luaShowDressUpForm(Data.comment_avatar, self.headImage, DressUp.Avatar, 1001);
        --加载头像框
        GameHelper.luaShowDressUpForm(Data.comment_avatar_frame, self.HeadFrame, DressUp.AvatarFrame, 2001);

        self.replyButton.onClick:RemoveAllListeners()
        self.replyButton.onClick:AddListener(function()
            self.inputMask.gameObject:SetActiveEx(true)
            logic.cs.EventSystem.current:SetSelectedGameObject(self.inputField.gameObject)
        end)
        self.inputMask.onClick:RemoveAllListeners()
        self.inputMask.onClick:AddListener(function()
            self.inputMask.gameObject:SetActiveEx(false)
        end)

        self.submitBtn.onClick:RemoveAllListeners()
        self.submitBtn.onClick:AddListener(function()
            local content = self.inputField.text
            if #content == 0 then
                return
            end
            logic.gameHttp:CreateBookCommentReply(Data.comment_id, content, Data.reply_id, function(result)
                local json = core.json.Derialize(result)
                local code = tonumber(json.code)
                if code == 200 then
                    self.inputField.text = ""
                    self.inputMask.gameObject:SetActiveEx(false)
                else
                    logic.cs.UIAlertMgr:Show("TIPS", json.msg)
                end
            end)
        end)

        self.type4:SetActiveEx(true)
        self.type2:SetActiveEx(false)
    end

    self.btnClose.onClick:RemoveAllListeners()
    self.btnClose.onClick:AddListener(function()
        self:OnExitClick()
    end)

    local uiform1 = root:GetComponent("CUIForm")
    local safeArea = logic.cs.ResolutionAdapter:GetSafeArea()
    local safeAreaHeight = uiform1:yPixel2View(safeArea.y)
    if safeAreaHeight and safeAreaHeight > 0 then
        local TopTile = root:Find('Bg/TopBg').transform
        local Type2 = root:Find('Bg/Type2').transform
        local Type4 = root:Find('Bg/Type4').transform
        local InputMask = root:Find('Bg/InputMask').transform

        local pos = TopTile.anchoredPosition
        pos.y = pos.y - safeAreaHeight
        TopTile.anchoredPosition = pos

        SetSafeAreaHeight(Type2, safeAreaHeight)
        SetSafeAreaHeight(Type4, safeAreaHeight)
        SetSafeAreaHeight(InputMask, safeAreaHeight)
    end
end

function UIEmailInfoForm:OnOpen()
    UIView.OnOpen(self)
end

function UIEmailInfoForm:OnClose()
    UIView.OnClose(self)
end

function UIEmailInfoForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end

function UIEmailInfoForm:SetEmailData(Data, _Hit,item)
    self.emailData = Data
    hit = _Hit;
    self.emailItem=item;
end

return UIEmailInfoForm
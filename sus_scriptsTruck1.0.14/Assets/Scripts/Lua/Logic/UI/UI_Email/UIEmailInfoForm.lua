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
    self.btnText = self.uiBinding:Get('BtnText', typeof(logic.cs.Text))
    self.picture = self.uiBinding:Get('Picture', typeof(logic.cs.Image))
    self.pictureBtn = self.picture.transform:GetComponent(typeof(logic.cs.Button))
    self.type4 = self.uiBinding:Get('Type4').gameObject
    self.type2 = self.uiBinding:Get('Type2').gameObject
    self.gitItemBg = self.uiBinding:Get('GitItemBg').gameObject
    self.gitItemBg2 = self.uiBinding:Get('GitItemBg2').gameObject
    self.scrollView = self.uiBinding:Get('ScrollView').gameObject
    self.scrollView2 = self.uiBinding:Get('ScrollView2').gameObject
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
    self.Item = self.uiBinding:Get('Item')

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.receiveButton.gameObject,function(data) self:ReceiveButtonClick() end)
    logic.cs.UIEventListener.AddOnClickListener(self.btnClose.gameObject,function(data) self:OnExitClick() end)
    logic.cs.UIEventListener.AddOnClickListener(self.replyButton.gameObject,function(data) self:ReplyButtonClick() end)
    logic.cs.UIEventListener.AddOnClickListener(self.inputMask.gameObject,function(data) self:InputMaskClick() end)
    logic.cs.UIEventListener.AddOnClickListener(self.submitBtn.gameObject,function(data) self:SubmitBtnClick() end)

    self.TopBg = CS.DisplayUtil.GetChild(self.uiform.gameObject, "TopBg"):GetComponent("RectTransform");

    --【屏幕适配】
    local offect = CS.XLuaHelper.UnSafeAreaNotFit(self.uiform, nil, 750, 120);
    local size = self.TopBg.sizeDelta;
    size.y = size.y + offect;
    self.TopBg.sizeDelta = size;


    self.EmailInfo=nil;
end


function UIEmailInfoForm:OnOpen()
    UIView.OnOpen(self)

    --EmailControl 赋值本界面
    GameController.EmailControl:SetEmailInfo(self);
end


function UIEmailInfoForm:OnClose()
    UIView.OnClose(self)

    --EmailControl 赋值本界面
    GameController.EmailControl:SetEmailInfo(nil);
end


function UIEmailInfoForm:SetEmailData(id)

    local Info=Cache.EmailCache:GetInfoById(id);
    if(Info==nil)then return; end
    self.EmailInfo=Info;

    if(Info.msg_type == 1)then  --1系统消息

        self.type2:SetActiveEx(true)
        self.type4:SetActiveEx(false)
        self.gitBg:SetActiveEx(false)
    elseif(Info.msg_type == 2)then  --2奖励信息
        local ItemNum = #Info.item_list
        if (Info.price_bkey and tonumber(Info.price_bkey) > 0) then
            ItemNum = ItemNum + 1
        end
        if (Info.price_diamond and tonumber(Info.price_diamond) > 0) then
            ItemNum = ItemNum + 1
        end
        local gitItemBg = self.gitItemBg
        if ItemNum > 5 then
            gitItemBg = self.gitItemBg2
        end
        self.scrollView:SetActiveEx(not (ItemNum > 5));
        self.scrollView2:SetActiveEx(ItemNum > 5);
        if (Info.price_bkey and tonumber(Info.price_bkey) > 0) then
            local item = logic.cs.GameObject.Instantiate(self.Item,gitItemBg.transform,false)
            local Text = CS.DisplayUtil.GetChild(item, "Text"):GetComponent(typeof(logic.cs.Text))
            local Icon = item:GetComponent(typeof(logic.cs.Image))
            Text.text = "x".. Info.price_bkey;
            Icon.sprite = Cache.PropCache.SpriteData[2]
            item:SetActiveEx(true)
        end
        if (Info.price_diamond and tonumber(Info.price_diamond) > 0) then
            local item = logic.cs.GameObject.Instantiate(self.Item,gitItemBg.transform,false)
            local Text = CS.DisplayUtil.GetChild(item, "Text"):GetComponent(typeof(logic.cs.Text))
            local Icon = item:GetComponent(typeof(logic.cs.Image))
            Text.text = "x".. Info.price_diamond;
            Icon.sprite = Cache.PropCache.SpriteData[1]
            item:SetActiveEx(true)
        end

        if Info.item_list then
            for k, v in pairs(Info.item_list) do
                local item = logic.cs.GameObject.Instantiate(self.Item,gitItemBg.transform,false)
                local Text = CS.DisplayUtil.GetChild(item, "Text"):GetComponent(typeof(logic.cs.Text))
                local Icon = item:GetComponent(typeof(logic.cs.Image))
                Text.text = "x".. v.num;
                if 1000<tonumber(v.id) and tonumber(v.id)<10000 then
                    local sprite=DataConfig.Q_DressUpData:GetSprite(v.id)
                    Icon.sprite = sprite
                else
                    local sprite = Cache.PropCache.SpriteData[tonumber(v.id)]
                    Icon.sprite = sprite
                end
                item:SetActive(true)
            end
        end

        self.btnText.text = "RECEIVE";

        if(Info.price_status == 1)then
            self.receiveButton.gameObject:SetActiveEx(false);
        end

        self.gitBg:SetActiveEx(true)

        self.type2:SetActiveEx(true)
        self.type4:SetActiveEx(false)

    elseif(Info.msg_type == 3)then  --3问卷调查
        if(Info.button_name and #Info.button_name > 0)then
            self.btnText.text = Info.button_name;
        else
            self.btnText.text = "RECEIVE";
        end
        self.scrollView:SetActiveEx(false);
        self.scrollView2:SetActiveEx(false);
        self.gitBg:SetActiveEx(true);
        self.type2:SetActiveEx(true);
        self.type4:SetActiveEx(false);
    elseif(Info.msg_type == 4)then  --4书本用户回复
        self.timeText.text = Info.createtime
        self.bookText.text = Info.book_name
        self.bodyText.text = "<b><color=#101010>" .. Info.replied_nickname .. ": </color></b>" .. Info.replied_content
        local contentTxt = Info.comment_content
        if(Info.comment_is_self == 0)then
            contentTxt = logic.cs.PluginTools:ReplaceBannedWords(contentTxt)
        end
        self.contentText.text = contentTxt

        local co = coroutine.start(function()
            coroutine.wait(0.1)
            logic.cs.PluginTools:ContentSizeFitterRefresh(self.bodyText.transform);
            logic.cs.PluginTools:ContentSizeFitterRefresh(self.contentText.transform);
        end)
        self.nameText.text = Info.comment_nickname

        --加载头像
        GameHelper.luaShowDressUpForm(Info.comment_avatar, self.headImage, DressUp.Avatar, 1001);
        --加载头像框
        GameHelper.luaShowDressUpForm(Info.comment_avatar_frame, self.HeadFrame, DressUp.AvatarFrame, 2001);

        self.type4:SetActiveEx(true)
        self.type2:SetActiveEx(false)
    end

    if(Info.msg_type ~= 4)then
        self.time.text = Info.createtime
        self.title.text = Info.title
        self.content.text = string.gsub(Info.content, '\\n', '\n')


        if(Info.email_pic and #Info.email_pic > 0)then
            logic.cs.ABSystem.ui:DownloadImage(Info.email_pic, function(url, refCount)
                if(Info.email_pic ~= url)then
                    refCount:Release();
                    return
                end
                local sprite = refCount:GetObject()
                self.picture.sprite = sprite
                self.picture:SetNativeSize()
            end)
            self.picture.gameObject:SetActiveEx(true)
            if(Info.email_url and #Info.email_url > 0)then
                self.pictureBtn.onClick:RemoveAllListeners()
                self.pictureBtn.onClick:AddListener(function()
                    logic.cs.Application.OpenURL(Info.email_url);
                end)
            end
        else
            self.picture.gameObject:SetActiveEx(false);
        end
    end
end


function UIEmailInfoForm:ReplyButtonClick()
    self.inputMask.gameObject:SetActiveEx(true);
    logic.cs.EventSystem.current:SetSelectedGameObject(self.inputField.gameObject);
end

function UIEmailInfoForm:InputMaskClick()
    self.inputMask.gameObject:SetActiveEx(false);
end


function UIEmailInfoForm:SubmitBtnClick()
    local content = self.inputField.text;
    if(#content == 0)then
        return;
    end

    if(self.EmailInfo)then
        GameController.EmailControl:CreateBookCommentReplyRequest(self.EmailInfo.comment_id, content, self.EmailInfo.reply_id);
    end
end

function UIEmailInfoForm:ReceiveButtonClick()
    if(self.EmailInfo)then
        if(self.EmailInfo.msg_type == 2)then
            GameController.EmailControl:AchieveMsgPriceRequest(self.EmailInfo.msgid);
        elseif(self.EmailInfo.msg_type == 3)then
            logic.cs.Application.OpenURL(self.EmailInfo.button_link);
        end
    end
end


function UIEmailInfoForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose();
    end
end

return UIEmailInfoForm
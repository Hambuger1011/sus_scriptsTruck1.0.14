local UIView = core.UIView

---@class UIEmailForm
local UIEmailForm = core.Class("UIEmailForm", UIView)

local uiid = logic.uiid;
UIEmailForm.config = {
    ID = uiid.UIEmailForm,
    AssetName = 'UI/Resident/UI/UIEmailForm'
}


--region【Awake】

local this=nil;
function UIEmailForm:OnInitView()
    UIView.OnInitView(self)
    this=self.uiform;

    --Toogle 标签按钮
    self.TabScrollRect = CS.DisplayUtil.GetChild(this.gameObject, "TabScrollRect");
    self.MailboxTab = CS.DisplayUtil.GetChild(self.TabScrollRect, "MailboxTab"):GetComponent("UIToggle");
    self.PrivateLetterTab = CS.DisplayUtil.GetChild(self.TabScrollRect, "PrivateLetterTab"):GetComponent("UIToggle");

    --邮件
    self.mEmailPanel =CS.DisplayUtil.GetChild(this.gameObject, "EmailPanel");
    self.EmailPanel = require('Logic/UI/UI_Email/Panel/EmailPanel').New(self.mEmailPanel);

    --个人私信
    self.mPrivateLetterPanel =CS.DisplayUtil.GetChild(this.gameObject, "PrivateLetterPanel");
    self.PrivateLetterPanel = require('Logic/UI/UI_Email/Panel/PrivateLetterPanel').New(self.mPrivateLetterPanel);

    self.TopTile =CS.DisplayUtil.GetChild(this.gameObject, "TopTile");
    self.TopRect = self.TopTile:GetComponent("RectTransform");
    self.CloseBtn =CS.DisplayUtil.GetChild(self.TopTile, "CloseBtn");
    self.ContactUsButton =CS.DisplayUtil.GetChild(self.TopTile, "ContactUsButton");

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.CloseBtn,function(data) self:OnExitClick(); end)
    logic.cs.UIEventListener.AddOnClickListener(self.ContactUsButton,function(data) logic.cs.IGGSDKMrg:OpenTSH(); end)

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.MailboxTab.gameObject,function(data) self:MailboxTabClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.PrivateLetterTab.gameObject,function(data) self:PrivateLetterTabClick(data) end);

    self.Bottom =CS.DisplayUtil.GetChild(this.gameObject, "Bottom");
    self.BottomRect = self.Bottom:GetComponent("RectTransform");
    self.BatchBtn =CS.DisplayUtil.GetChild(self.Bottom, "BatchBtn");
    self.CollectBtn =CS.DisplayUtil.GetChild(self.Bottom, "CollectBtn");
    self.DeleteBtn =CS.DisplayUtil.GetChild(self.Bottom, "DeleteBtn");
    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.BatchBtn,function(data) self:BatchBtnClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.DeleteBtn,function(data) self:DeleteBtnClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.CollectBtn,function(data) self:CollectBtnClick(data) end);



end

--endregion


--region【OnOpen】

function UIEmailForm:OnOpen()
    UIView.OnOpen(self)
    GameController.EmailControl:SetData(self);
    --请求获取邮箱信息
    GameController.EmailControl:GetPrivateLetterTeamPageRequest(1);
    --请求获取邮箱信息
    GameController.EmailControl:GetSystemMsgRequest(1);

    self:MailboxTabClick(nil);

    --【屏幕适配】
    local offect = CS.XLuaHelper.UnSafeAreaNotFit(self.uiform, nil, 750, 120);
    local size = self.TopRect.sizeDelta;
    size.y = size.y + offect;
    self.TopRect.sizeDelta = size;

    local size2 = self.BottomRect.sizeDelta;
    size2.y = size2.y + offect;
    self.BottomRect.sizeDelta=size2;
end

--endregion


--region 【OnClose】

function UIEmailForm:OnClose()
    UIView.OnClose(self)

    GameController.EmailControl:SetData(nil);
    GameController.EmailControl:EmailReset();
    GameController.EmailControl:Reset();
    if(self.CommunityTab)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.MailboxTab.gameObject,function(data) self:MailboxTabClick(data) end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.PrivateLetterTab.gameObject,function(data) self:PrivateLetterTabClick(data) end);

        logic.cs.UIEventListener.RemoveOnClickListener(self.CloseBtn,function(data) self:OnExitClick(); end)
        logic.cs.UIEventListener.RemoveOnClickListener(self.ContactUsButton,function(data) logic.cs.IGGSDKMrg:OpenTSH(); end)
    end


    --Toogle 标签按钮
    self.TabScrollRect = nil;
    self.MailboxTab = nil;
    self.PrivateLetterTab = nil;

    --关闭销毁 【邮件】
    self.EmailPanel:Delete();
    --关闭销毁 【个人私信】
    self.PrivateLetterPanel:Delete();

    self.TopTile = nil;
    self.CloseBtn = nil;
    self.ContactUsButton = nil;

    self.Bottom = nil;
    self.BatchBtn = nil;
    self.CollectBtn = nil;
    self.DeleteBtn = nil;
end

--endregion


--region 【刷新数据】【邮箱】

function UIEmailForm:UpdateEmail(page)
    if(self.EmailPanel)then
        self.EmailPanel:UpdateEmail(page);

        if(self.MailboxTab.isOn==true)then
            self.EmailPanel:SetBatchBtn(self.BatchBtn);
        end
    end
end

--endregion


--region 【Toogle 切换显示状态】

function UIEmailForm:ChangeToggle(name)
    --关闭旧的
    self:HideOld(self.oldToggleName);
    if(name~="" and self.oldToggleName~=name)then
        self.oldToggleName=name;
    end
end

--【关闭旧界面】
function UIEmailForm:HideOld(name)
    if(name==nil)then return; end

    if(name=="MailboxTab")then
        self.mEmailPanel:SetActive(false);
    elseif(name=="PrivateLetterTab")then
        self.mPrivateLetterPanel:SetActive(false);
    end
end
--endregion


--region 【点击Mailbox】
function UIEmailForm:MailboxTabClick(data)
    if(self.oldToggleName=="MailboxTab")then return; end
    self:ChangeToggle("MailboxTab");

    --打开邮件面板
    self.mEmailPanel:SetActive(true);

    if(self.EmailPanel)then
        self.EmailPanel:SetBatchBtn(self.BatchBtn);
    end
end
--endregion


--region 【点击PrivateLetter】
function UIEmailForm:PrivateLetterTabClick(data)
    if(self.oldToggleName=="PrivateLetterTab")then return; end
    self:ChangeToggle("PrivateLetterTab");

    --打开个人私信面板
    self.mPrivateLetterPanel:SetActive(true);

    if(self.PrivateLetterPanel)then
        self.PrivateLetterPanel:SetBatchBtn(self.BatchBtn);
    end
end
--endregion


--region【批量检测】
function UIEmailForm:BatchTest(msgid,_type,len,isAdd)

    self.DeleteBtn:SetActive(false);
    self.BatchBtn:SetActive(false);
    self.CollectBtn:SetActive(false);

    if(len>0)then
        if(_type==1)then
            if(Cache.EmailCache:IsUnread(msgid,isAdd)==true)then --是否有未读
                self.CollectBtn:SetActive(true);
            else
                self.DeleteBtn:SetActive(true);
            end
        elseif(_type==2)then
            if(Cache.EmailCache:IsUnreadPrivate(msgid,isAdd)==true)then --是否有未读
                self.CollectBtn:SetActive(true);
            else
                self.DeleteBtn:SetActive(true);
            end
        end
    else
        self.BatchBtn:SetActive(true);
    end
end
--endregion


--region 【点击*批量管理】
local numb=0;
function UIEmailForm:BatchBtnClick()
    numb=numb+1;
    if(self.mEmailPanel.activeSelf==true)then    --邮件
        if(self.EmailPanel)then
            if(numb%2==0)then
                self.EmailPanel:SelectAll(false,false);
            else
                self.EmailPanel:SelectAll(true,false);
            end
        end
    elseif(self.mPrivateLetterPanel.activeSelf==true)then      --个人私信
        if(self.PrivateLetterPanel)then
            if(numb%2==0)then
                self.PrivateLetterPanel:SelectAll(false,false);
            else
                self.PrivateLetterPanel:SelectAll(true,false);
            end
        end
    end
end
--endregion


--region 【点击*收取】
function UIEmailForm:CollectBtnClick(data)
    if(self.mEmailPanel.activeSelf==true)then    --邮件
        GameController.EmailControl:ReadSelected(1);
    elseif(self.mPrivateLetterPanel.activeSelf==true)then      --个人私信
        GameController.EmailControl:ReadSelected(2);
    end
end
--endregion


--region 【点击*删除】
function UIEmailForm:DeleteBtnClick(data)
    if(self.mEmailPanel.activeSelf==true)then    --邮件
        GameController.EmailControl:DeleteSelected(1);
    elseif(self.mPrivateLetterPanel.activeSelf==true)then      --个人私信
        GameController.EmailControl:DeleteSelected(2);
    end
end
--endregion


--region 【单个、批量删除邮件】
function UIEmailForm:DelMail(_type)
    self.DeleteBtn:SetActive(false);
    self.CollectBtn:SetActive(false);
    numb=numb+1;
    if(_type==1)then
        self.EmailPanel:SelectAll(false,true);
    elseif(_type==2)then
        self.PrivateLetterPanel:SelectAll(false,true);
    end
end
--endregion


--region【领取奖励】
function UIEmailForm:AchieveMsgPrice(msgid)

    if(self.EmailPanel)then
        self.EmailPanel:AchieveMsgPrice(msgid);
    end

end
--endregion


--region【刷新信鸽列表】
function UIEmailForm:UpdateGetPrivateLetterBoxList(page)
    if(self.PrivateLetterPanel)then
        self.PrivateLetterPanel:UpdateGetPrivateLetterBoxList(page);
        if(self.PrivateLetterTab.isOn==true)then
            self.PrivateLetterPanel:SetBatchBtn(self.BatchBtn);
        end
    end
end
--endregion


--region 【界面关闭】
function UIEmailForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end
--endregion



return UIEmailForm
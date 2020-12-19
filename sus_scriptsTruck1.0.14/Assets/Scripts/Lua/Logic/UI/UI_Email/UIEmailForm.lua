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
    self.CloseBtn =CS.DisplayUtil.GetChild(self.TopTile, "CloseBtn");
    self.ContactUsButton =CS.DisplayUtil.GetChild(self.TopTile, "ContactUsButton");

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.CloseBtn,function(data) self:OnExitClick(); end)
    logic.cs.UIEventListener.AddOnClickListener(self.ContactUsButton,function(data) logic.cs.IGGSDKMrg:OpenTSH(); end)

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.MailboxTab.gameObject,function(data) self:MailboxTabClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.PrivateLetterTab.gameObject,function(data) self:PrivateLetterTabClick(data) end);

    self.Bottom =CS.DisplayUtil.GetChild(this.gameObject, "Bottom");
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
    self:MailboxTabClick(nil);
end

--endregion


--region 【OnClose】

function UIEmailForm:OnClose()
    UIView.OnClose(self)

    GameController.EmailControl:SetData(nil);





    if(self.CommunityTab)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.MailboxTab.gameObject,function(data) self:MailboxTabClick(data) end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.PrivateLetterTab.gameObject,function(data) self:PrivateLetterTabClick(data) end);

        logic.cs.UIEventListener.RemoveOnClickListener(self.CloseBtn,function(data) self:OnExitClick(); end)
        logic.cs.UIEventListener.RemoveOnClickListener(self.ContactUsButton,function(data) logic.cs.IGGSDKMrg:OpenTSH(); end)
    end
end

--endregion


--region 【刷新数据】【邮箱】

function UIEmailForm:UpdateEmail(page)
    if(self.EmailPanel)then
        self.EmailPanel:UpdateEmail(page);
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
end
--endregion


--region 【点击PrivateLetter】
function UIEmailForm:PrivateLetterTabClick(data)
    if(self.oldToggleName=="PrivateLetterTab")then return; end
    self:ChangeToggle("PrivateLetterTab");

    --打开个人私信面板
    self.mPrivateLetterPanel:SetActive(true);

end
--endregion


--region 【点击*批量管理】
function UIEmailForm:BatchBtnClick(data)


end
--endregion

--region 【点击*收取】
function UIEmailForm:CollectBtnClick(data)


end
--endregion


--region 【点击*删除】
function UIEmailForm:BatchBtnClick(data)


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
local UIView = core.UIView

local UIActivityForm = core.Class("UIActivityForm", UIView)


local uiid = logic.uiid
UIActivityForm.config = {
    ID = uiid.UIActivityForm,
    AssetName = 'UI/Resident/UI/UIActivityForm'
}


--region【Awake】

local this=nil;
function UIActivityForm:OnInitView()
    UIView.OnInitView(self)
    this=self.uiform;

    self.Tabs = CS.DisplayUtil.GetChild(this.gameObject, "Tabs");
    self.LimitedTimeTab = CS.DisplayUtil.GetChild(self.Tabs, "LimitedTimeTab"):GetComponent("UIToggle");
    self.ActivityTab = CS.DisplayUtil.GetChild(self.Tabs, "ActivityTab"):GetComponent("UIToggle");
    self.NewsTab = CS.DisplayUtil.GetChild(self.Tabs, "NewsTab"):GetComponent("UIToggle");

    self.LimitedTimeRedPoint = CS.DisplayUtil.GetChild(self.LimitedTimeTab.gameObject, "RedPoint");
    self.ActivityRedPoint = CS.DisplayUtil.GetChild(self.ActivityTab.gameObject, "RedPoint");
    self.NewsRedPoint = CS.DisplayUtil.GetChild(self.NewsTab.gameObject, "RedPoint");


    --【限时活动】【界面】
    self.mLimitedTimePanel =CS.DisplayUtil.GetChild(this.gameObject, "LimitedTimePanel");
    self.LimitedTimePanel = require('Logic/UI/UI_Activity/Panel/LimitedTimePanel').New(self.mLimitedTimePanel);

    --【常规活动】【界面】
    self.mActivityPanel =CS.DisplayUtil.GetChild(this.gameObject, "ActivityPanel");
    self.ActivityPanel = require('Logic/UI/UI_Activity/Panel/ActivityPanel').New(self.mActivityPanel);

    --【新闻外部活动】【界面】
    self.mNewsPanel =CS.DisplayUtil.GetChild(this.gameObject, "NewsPanel");
    self.SafeBG =this.transform:Find('BG')
    self.NewsPanel = require('Logic/UI/UI_Activity/Panel/NewsPanel').New(self.mNewsPanel,self.SafeBG.transform);

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.LimitedTimeTab.gameObject,function(data) self:LimitedTimeTabClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.ActivityTab.gameObject,function(data) self:ActivityTabClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.NewsTab.gameObject,function(data) self:NewsTabClick(data) end);
    
    self.MsgListener = function() self:LimitedTimeTabClick() end
    logic.cs.EventDispatcher.AddMessageListener(logic.cs.EventEnum.HideWebView, self.MsgListener)

    self.WebViewHide=false;
end
--endregion


--region【OnOpen】

function UIActivityForm:OnOpen()
    UIView.OnOpen(self)

    GameController.ActivityControl:SetData(self);

    self:LimitedTimeTabClick(nil);

    self.LimitedTimePanel:OnOpen();

    --刷新红点
    self:RefreshRed()
end

--endregion


--region 【OnClose】

function UIActivityForm:OnClose()
    UIView.OnClose(self)
    GameController.ActivityControl:SetData(nil);
    
    logic.cs.EventDispatcher.RemoveMessageListener(logic.cs.EventEnum.HideWebView, self.MsgListener)

    if(self.LimitedTimeTab)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.LimitedTimeTab.gameObject,function(data) self:LimitedTimeTabClick(data) end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.ActivityTab.gameObject,function(data) self:ActivityTabClick(data) end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.NewsTab.gameObject,function(data) self:NewsTabClick(data) end);
    end

    self.LimitedTimePanel:OnClose();
    --关闭销毁 【LimitedTimePanel】lua脚本
    self.LimitedTimePanel:Delete();
    --关闭销毁 【ActivityPanel】lua脚本
    self.ActivityPanel:Delete();
    --关闭销毁 【NewsPanel】lua脚本
    self.NewsPanel:Delete();


    self.Tabs = nil;
    self.LimitedTimeTab = nil;
    self.ActivityTab = nil;
    self.NewsTab = nil;
    self.mLimitedTimePanel = nil;
    self.LimitedTimePanel = nil;
    self.mActivityPanel = nil;
    self.ActivityPanel = nil;
    self.mNewsPanel = nil;
    self.NewsPanel = nil;
    self.WebViewHide=false;


end

--endregion


--region【刷新红点】

function UIActivityForm:RefreshRed()
    if(Cache.RedDotCache.ActivityPanelRed==true)then
        self.ActivityRedPoint:SetActive(true);
    else
        self.ActivityRedPoint:SetActive(false);
    end

    if(Cache.RedDotCache.LimitedTimePanelRed==true)then
        self.LimitedTimeRedPoint:SetActive(true);
    else
        self.LimitedTimeRedPoint:SetActive(false);
    end

    if(Cache.RedDotCache.NewsPanelRed==true)then
        self.NewsRedPoint:SetActive(true);
    else
        self.NewsRedPoint:SetActive(false);
    end

    if(self.LimitedTimePanel)then
        self.LimitedTimePanel:RedPointShow();
    end
end

--endregion


--region 【点击LimitedTime】

function UIActivityForm:LimitedTimeTabClick(data)
    if(self.oldToggleName=="LimitedTimeTab")then return; end
    self:ChangeToggle("LimitedTimeTab");
    self.LimitedTimeTab.isOn=true;
    self.mLimitedTimePanel:SetActive(true);
end

--endregion


--region 【点击Activity】

function UIActivityForm:ActivityTabClick(data)
    if(self.oldToggleName=="ActivityTab")then return; end
    self:ChangeToggle("ActivityTab");
    self.ActivityTab.isOn=true;

    --【获取登录活动奖励内容】【签到】
    GameController.ActivityControl:GetActivityRewardContentRequest();
    --【获取任务列表】【每日任务】
    GameController.ActivityControl:GetMyTaskListRequest();

    if(self.mActivityPanel)then
        self.mActivityPanel:SetActive(true);
    end
end

--endregion


--region 【点击News】

function UIActivityForm:NewsTabClick(data)
    if(self.oldToggleName=="NewsTab")then return; end
    self:ChangeToggle("NewsTab");
    self.NewsTab.isOn=true;

    self.NewsPanel:ShowWebView()


    if(Cache.RedDotCache.NewsPanelRed==true)then
        Cache.RedDotCache.NewsPanelRed=false;
        self.NewsRedPoint:SetActive(false);
        logic.cs.PlayerPrefs.SetInt("NewsPanel", 2);
    end
end

--endregion


--region 【Toogle 切换显示状态】

function UIActivityForm:ChangeToggle(name)
    --关闭旧的
    self:HideOld(self.oldToggleName);
    if(name~="" and self.oldToggleName~=name)then
        self.oldToggleName=name;
    end
end

--endregion


--region【关闭旧界面】

function UIActivityForm:HideOld(name)
    if(name==nil)then return; end

    if(name=="LimitedTimeTab")then
        self.LimitedTimeTab.isOn=false;
        self.mLimitedTimePanel:SetActive(false);
    elseif(name=="ActivityTab")then
        self.ActivityTab.isOn=false;
        self.mActivityPanel:SetActive(false);

        --停止倒计时 计时器
        GameHelper.StopFR_Timer();
    elseif(name=="NewsTab")then
        self.NewsTab.isOn=false;
        --关闭
        self.NewsPanel:DestroyWebView();
    end
end

--endregion


--region【刷新常规活动 签到面板】
function UIActivityForm:UpdateSignInPanel()
    if(self.ActivityPanel)then
        self.ActivityPanel:UpdateSignInPanel();
    end
end
--endregion


--region【刷新常规活动 签到领取奖励】
function UIActivityForm:SignInReceiveReward()
    if(self.ActivityPanel)then
        self.ActivityPanel:SignInReceiveReward();
    end
end
--endregion


--region 【刷新常规活动 每日任务】

function UIActivityForm:UpdateTasks(data)
    --【获取任务列表】【每日任务】
    GameController.ActivityControl:GetMyTaskListRequest();
end

--endregion


--region【刷新常规活动 每日任务】
function UIActivityForm:UpdateDailyTasksPanel()
    if(self.ActivityPanel)then
        self.ActivityPanel:UpdateDailyTasksPanel();
    end
end
--endregion


--region【刷新常规活动 在线阅读时长】
function UIActivityForm:UpdateReadTimePanel()
    if(self.ActivityPanel)then
        self.ActivityPanel:UpdateReadTimePanel();
    end
end
--endregion


--region【刷新常规活动 刷新看广告】
function UIActivityForm:UpdateFreeRewardPanel()
    if(self.ActivityPanel)then
        self.ActivityPanel:UpdateFreeRewardPanel();
    end
end
--endregion


--region【刷新常规活动 领取活动广告奖励】
function UIActivityForm:ReceiveDailyAdAward()
    if(self.ActivityPanel)then
        self.ActivityPanel:ReceiveDailyAdAward();
    end
end
--endregion


--region【领取用户迁移的奖励*响应】---【限时活动】【限时活动*账号迁移奖励】
function UIActivityForm:ReceiveUserMoveAward_Response()
    if(self.LimitedTimePanel)then
        self.LimitedTimePanel:ReceiveUserMoveAward_Response();
    end
end
--endregion


--region【领取首充奖励*响应】---【限时活动】【限时活动*账号迁移奖励】
function UIActivityForm:ReceiveFirstRechargeAward_Response()
    if(self.LimitedTimePanel)then
        self.LimitedTimePanel:ReceiveFirstRechargeAward_Response();
    end
end
--endregion


--region【领取第三方登录绑定的奖励*响应】---【限时活动】【账号绑定奖励】
function UIActivityForm:ReceiveThirdPartyAward_Response()
    if(self.LimitedTimePanel)then
        self.LimitedTimePanel:ReceiveThirdPartyAward_Response();
    end
end
--endregion


--region【领取关注社媒奖励*响应】---【限时活动】【关注社媒奖励】
function UIActivityForm:ReceiveAttentionMediaReward_Response()
    if(self.LimitedTimePanel)then
        self.LimitedTimePanel:ReceiveAttentionMediaReward_Response();
    end
end
--endregion


--region【更新社媒状态奖励为可领取*响应】---【限时活动】【关注社媒】
function UIActivityForm:UpdateAttentionMedia_Response()
    if(self.LimitedTimePanel)then
        self.LimitedTimePanel:UpdateAttentionMedia_Response();
    end
end
--endregion


--region【获取通用奖励配置*响应】---【限时活动】【通用奖励】
function UIActivityForm:GetRewardConfig_Response()
    if(self.LimitedTimePanel)then
        self.LimitedTimePanel:GetRewardConfig_Response();
    end
end
--endregion


--region【常规活动 重置排序】

function UIActivityForm:PanelSort()
    if(self.ActivityPanel)then
        self.ActivityPanel:PanelSort();
    end
end

--endregion


--region【限时活动】【刷新】【绑定有礼】

function UIActivityForm:SetBindStatus()
    if(self.LimitedTimePanel)then
        self.LimitedTimePanel:SetBindStatus();
    end
end

--endregion


--region【限时活动】【刷新】【关注有礼】

function UIActivityForm:SetFollowStatus()
    if(self.LimitedTimePanel)then
        self.LimitedTimePanel:SetFollowStatus();
    end
end

--endregion


--region【限时活动】【刷新】【迁移奖励】

function UIActivityForm:SetMoveRewardStatus()
    if(self.LimitedTimePanel)then
        self.LimitedTimePanel:SetMoveRewardStatus();
    end
end

--endregion


--region【限时活动】【刷新】【全书免费】

function UIActivityForm:SetFreeBG()
    if(self.LimitedTimePanel)then
        self.LimitedTimePanel:SetFreeBG();
    end
end

--endregion

--region 【刷新常规活动】【广告CD开始】
function UIActivityForm:StartCD()
    if(self.ActivityPanel)then
        self.ActivityPanel:StartCD();
    end
end

--endregion


--region 【刷新常规活动】【广告CD结束】
function UIActivityForm:EndCD()
    if(self.ActivityPanel)then
        self.ActivityPanel:EndCD();
    end
end

--endregion


--region 【刷新常规活动】【广告CD】【展示文本】

function UIActivityForm:ShowCD(txt)
    if(self.ActivityPanel)then
        self.ActivityPanel:ShowCD(txt);
    end
end

--endregion


--region【UIActivityForm设置ScrollRect】---【常规活动】

function UIActivityForm:SetVerticalNormalizedPosition()
    if(self.ActivityPanel)then
        self.ActivityPanel:SetVerticalNormalizedPosition();
    end
end

--endregion


--region【投资活动关闭】---【限时活动】

function UIActivityForm:InvestmentIsEnd()
    if(self.LimitedTimePanel)then
        self.LimitedTimePanel:InvestmentIsEnd();
    end
end

--endregion


--region【投资活动刷新】---【限时活动】

function UIActivityForm:UpdateInvestment()
    if(self.LimitedTimePanel)then
        self.LimitedTimePanel:UpdateInvestment();
    end
end

--endregion


--region 【界面关闭】
function UIActivityForm:OnExitClick()
    UIView.__Close(self)
    if(self.onClose)then
        self.onClose()
    end
end
--endregion



return UIActivityForm
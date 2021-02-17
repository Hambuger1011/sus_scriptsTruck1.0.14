local UIView = core.UIView

local UIMainDownForm = core.Class("UIMainDownForm", UIView)

local uiid = logic.uiid
UIMainDownForm.config = {
    ID = uiid.UIMainDownForm,
    AssetName = 'UI/Resident/UI/UIMainDownForm'
}

--region【Awake】

local this=nil;
function UIMainDownForm:OnInitView()
    UIView.OnInitView(self)
    this=self.uiform;

    self.Down = CS.DisplayUtil.GetChild(this.gameObject, "Down");
    self.DownRect=self.Down:GetComponent(typeof(logic.cs.RectTransform));

    --主界面 Toogle
    self.HomeToggle = CS.DisplayUtil.GetChild(self.Down, "HomeToggle"):GetComponent("UIToggle");
    --搜索界面 Toogle
    self.SearchToggle = CS.DisplayUtil.GetChild(self.Down, "SearchToggle"):GetComponent("UIToggle");
    --创作界面 Toogle
    self.CommunityToggle = CS.DisplayUtil.GetChild(self.Down, "CommunityToggle"):GetComponent("UIToggle");
    --活动奖励 Toogle
    self.RwardToggle = CS.DisplayUtil.GetChild(self.Down, "RwardToggle"):GetComponent("UIToggle");
    --个人中心 Toogle
    self.ProfileToggle =CS.DisplayUtil.GetChild(self.Down, "ProfileToggle"):GetComponent("UIToggle");

    self.Rward_RedImg=CS.DisplayUtil.GetChild(self.RwardToggle.gameObject, "Rward_RedImg");
    self.Profile_RedImg=CS.DisplayUtil.GetChild(self.ProfileToggle.gameObject, "Profile_RedImg");
    --默认主界面
    self:HomeToggleClick(nil)
end
--endregion


--region【OnOpen 加载完成 打开后执行】

function UIMainDownForm:OnOpen()
    UIView.OnOpen(self)
    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.HomeToggle.gameObject,function(data) self:HomeToggleClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.SearchToggle.gameObject,function(data) self:SearchToggleClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.CommunityToggle.gameObject,function(data) self:CommunityToggleClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.RwardToggle.gameObject,function(data) self:RwardToggleClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.ProfileToggle.gameObject,function(data) self:ProfileToggleClick(data) end);
    --红点请求
    self:RedPointRequest();
    --定时红点请求
    self:TimerRedPointRequest();

    --【本界面自适应】
    --获取偏移量；
    local offectY= CS.XLuaHelper.isHasUnSafeArea(self.uiform);
    local curnum=offectY/2+100;
    self.DownRect.sizeDelta={x=750,y=curnum};
end

--endregion


--region 【OnClose】

function UIMainDownForm:OnClose()
    UIView.OnClose(self)

    GameController.MainFormControl:ClearTimer()

    --【销毁红点定时器】
    GameController.MainFormControl:ClearTimer();


    --按钮监听
    logic.cs.UIEventListener.RemoveOnClickListener(self.HomeToggle.gameObject,function(data) self:HomeToggleClick(data) end);
    logic.cs.UIEventListener.RemoveOnClickListener(self.SearchToggle.gameObject,function(data) self:SearchToggleClick(data) end);
    logic.cs.UIEventListener.RemoveOnClickListener(self.CommunityToggle.gameObject,function(data) self:CommunityToggleClick(data) end);
    logic.cs.UIEventListener.RemoveOnClickListener(self.RwardToggle.gameObject,function(data) self:RwardToggleClick(data) end);
    logic.cs.UIEventListener.RemoveOnClickListener(self.ProfileToggle.gameObject,function(data) self:ProfileToggleClick(data) end);


    --如果本界面都关闭了   五个界面也关闭销毁；
    local uimain = logic.UIMgr:GetView2(logic.uiid.UIMainForm);
    if(uimain)then
        logic.UIMgr:Close(logic.uiid.UIMainForm);
    end

    local uisearch = logic.UIMgr:GetView2(logic.uiid.UISearchForm);
    if(uisearch)then
        logic.UIMgr:Close(logic.uiid.UISearchForm);
    end

    local uicomuniada = logic.UIMgr:GetView2(logic.uiid.UIComuniadaForm);
    if(uicomuniada)then
        logic.UIMgr:Close(logic.uiid.UIComuniadaForm);
    end

    local uiactivity = logic.UIMgr:GetView2(logic.uiid.UIActivityForm);
    if(uiactivity)then
        logic.UIMgr:Close(logic.uiid.UIActivityForm);
    end


    local uiform=logic.cs.CUIManager:GetForm(logic.cs.UIFormName.ProfileForm)
    if(uiform or CS.XLuaHelper.is_Null(uiform)==false)then
        logic.cs.CUIManager:CloseForm(logic.cs.UIFormName.ProfileForm)
    end

    local uiactivitybanner = logic.UIMgr:GetView2(logic.uiid.UIActivityBannerForm);
    if(uiactivitybanner)then
        logic.UIMgr:Close(logic.uiid.UIActivityBannerForm);
    end


    self.Down =nil;
    self.HomeToggle  =nil;
    self.SearchToggle =nil;
    self.CommunityToggle  =nil;
    self.RwardToggle =nil;
    self.ProfileToggle  =nil;
    self.Rward_RedImg =nil;
    self.Profile_RedImg =nil;

end

--endregion


--region 【点击Home标签】
function UIMainDownForm:HomeToggleClick(data)
    if(self.oldToggleName=="HomeToggle")then return; end
    self:ChangeToggle("HomeToggle");
    if(data)then

        local uiform = logic.UIMgr:GetView2(logic.uiid.UIMainForm);
        if(uiform==nil)then
            --打开主界面
            logic.UIMgr:Open(logic.uiid.UIMainForm);
        else
            uiform.uiform:Appear();
        end

        --刷新倒计时
        GameController.ActivityBannerControl:DefaultCountDown();
    end
end
--endregion


--region 【点击搜索按钮】

function UIMainDownForm:SearchToggleClick(data,_index)
    if(self.oldToggleName=="SearchToggle")then return; end
    self:ChangeToggle("SearchToggle");
    --埋点*打开搜索界面
    logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.SearchPage);

    --搜索界面
    local uiform = logic.UIMgr:GetView2(logic.uiid.UISearchForm);
    if(uiform==nil)then
        --打开主界面
        uiform = logic.UIMgr:Open(logic.uiid.UISearchForm);
        uiform:OpenBookType(_index)
    else
        uiform.uiform:Appear();
        uiform:OpenBookType(_index)
    end

end
--endregion


--region 【点击创作按钮】

function UIMainDownForm:CommunityToggleClick(data)
    if(self.oldToggleName=="CommunityToggle")then return; end
    self:ChangeToggle("CommunityToggle");

    --埋点*打开创作界面
    logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.UgcPage);

    --创作界面
    local uiform = logic.UIMgr:GetView2(logic.uiid.UIComuniadaForm);
    if(uiform==nil)then
        --打开创作界面
        uiform = logic.UIMgr:Open(logic.uiid.UIComuniadaForm);
    else
        uiform.uiform:Appear();
    end
end

--endregion


--region 【点击活动奖励按钮】
function UIMainDownForm:RwardToggleClick(data,data1)
    if(self.oldToggleName=="RwardToggle")then return; end
    self:ChangeToggle("RwardToggle");
    --埋点*打开活动界面
    logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.ActivityPage);

    --每天第一次登陆有任务未完成时显示红点，1.今天已提示过 0.未提示过
    local first_task_notice = Cache.RedDotCache.first_task_notice;
    if(first_task_notice==0)then
        GameController.MainFormControl:FirstTaskRequest();
    end

    local uiform = logic.UIMgr:GetView2(logic.uiid.UIActivityForm);
    if(uiform==nil)then
        uiform = logic.UIMgr:Open(logic.uiid.UIActivityForm);
    else
        uiform.uiform:Appear();
        uiform:SetBindStatus();
        uiform:UpdateInvestment();
    end

    if(uiform and data1 and data1==3)then
        uiform:NewsTabClick(nil);
    end

end
--endregion


--region【点击个人中心按钮】

function UIMainDownForm:ProfileToggleClick(data)
    if(self.oldToggleName=="ProfileToggle")then return; end
    self:ChangeToggle("ProfileToggle");
    --埋点*打开个人中心界面
    logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.UcenterPage);

     --个人中心
    local uiform=logic.cs.CUIManager:GetForm(logic.cs.UIFormName.ProfileForm)
    if(uiform==nil or CS.XLuaHelper.is_Null(uiform)==true)then
        logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.ProfileForm);
    end
    if(uiform and CS.XLuaHelper.is_Null(uiform)==false)then
        --展示界面
        uiform:Appear();
    end
end

--endregion


--region 【Toogle 切换显示状态】

function UIMainDownForm:ChangeToggle(name)
    --延时红点请求
    GameController.MainFormControl:DelayRedPointRequest();
    --关闭旧的
    self:HideOld(self.oldToggleName,false);
    if(self.oldToggleName~=name)then
        self.oldToggleName=name;
    end
end

--【关闭旧界面】
function UIMainDownForm:HideOld(name)
    if(name==nil)then return; end
    if(name=="HomeToggle")then
        local uimain = logic.UIMgr:GetView2(logic.uiid.UIMainForm);
        if(uimain)then
            uimain.uiform:Hide();
        end
    elseif(name=="SearchToggle")then
        local uisearch = logic.UIMgr:GetView2(logic.uiid.UISearchForm);
        if(uisearch)then
            uisearch.uiform:Hide();
        end
    elseif(name=="CommunityToggle")then
        local uicomuniada = logic.UIMgr:GetView2(logic.uiid.UIComuniadaForm);
        if(uicomuniada)then
            uicomuniada.uiform:Hide();
        end
    elseif(name=="RwardToggle")then
        local uiactivity = logic.UIMgr:GetView2(logic.uiid.UIActivityForm);
        if(uiactivity)then
            uiactivity.uiform:Hide();
            uiactivity:LimitedTimeTabClick(nil);
        end
    elseif(name=="ProfileToggle")then
        local uiform=logic.cs.CUIManager:GetForm(logic.cs.UIFormName.ProfileForm)
        if(uiform or CS.XLuaHelper.is_Null(uiform)==false)then
            uiform:Hide();
        end
        if(GameController.DressUpControl.isShow==true)then
            local uidressup = logic.UIMgr:GetView2(logic.uiid.UIDressUpForm);
            if(uidressup)then
                uidressup:OnExitClick();
            end
        end

    end
end

--endregion


--region 【红点请求】

function UIMainDownForm:RedPointRequest()
    GameController.MainFormControl:RedPointRequest();
end

function UIMainDownForm:TimerRedPointRequest()
    GameController.MainFormControl:TimerRedPointRequest();
end

--endregion


--region 【红点显示】

function UIMainDownForm:Profile_RedImg_show(isShow)
    if (isShow==true)then
        self.Profile_RedImg:SetActive(true);
    else
        self.Profile_RedImg:SetActive(false);
    end
end



function UIMainDownForm:Rward_RedImg_show(isShow)
    if (isShow==true)then
        self.Rward_RedImg:SetActive(true);
    else
        self.Rward_RedImg:SetActive(false);
    end
end

--endregion


--region 【主界面移入动画】
function UIMainDownForm:MainFormMove(data)
    local _type = data;
    self.MainBG.anchoredPosition ={x=0,y=0};
    if(_type==1)then
        --主界面移出
        self.MainBG:DOAnchorPosX(380, 0.5):Play();
    else
        --主界面移入
        self.MainBG.anchoredPosition ={x=0,y=0};
    end
end
--endregion


--region 【界面关闭】
function UIMainDownForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end
--endregion



return UIMainDownForm
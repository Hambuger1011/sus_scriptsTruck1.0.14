local WindowConfig = core.Class("WindowConfig", core.Singleton)

local this = WindowConfig
local FirstChargeNeedShown = true
local BookPopupNeedShown = true
local WindowIndex = 0

--region【构造函数】
function WindowConfig:__init()
end
--endregion

function WindowConfig:SetWindowsList()
    WindowIndex = 0
    this.NeedShowNextWindow = false
end

function WindowConfig:ResetStatus()
    FirstChargeNeedShown = true
    BookPopupNeedShown = true
end

function WindowConfig:ShowFirstCharge()
    if tonumber(logic.cs.UserDataManager.selfBookInfo.data.first_recharge_switch) ~= 0
            and not logic.cs.IGGSDKMrg.isNewUser and not FirstChargeNeedShown then

        --请求【获取通用奖励配置】---【通用奖励】
        GameController.FirstChargeControl:GetRewardConfigRequest();

        FirstChargeNeedShown = false
    else
        this:ShowNextWindow()
    end
end

function WindowConfig:ShowNewBookTips()
    if BookPopupNeedShown then
        BookPopupNeedShown = false
        local uiView = logic.UIMgr:Open(logic.uiid.UINewBookTipsForm);
    else
        this:ShowNextWindow()
    end
end


function WindowConfig:ShowDayPass()
    local info2=Cache.PopWindowCache:GetInfoById(2);
    if(info2)then
        local daypassInfo= info2:GetDayPassShow()
        if(daypassInfo)then
            local uiform = logic.UIMgr:GetView2(logic.uiid.UIDayPassForm);
            if(Cache.PopWindowCache:IsHaveDayPass2(daypassInfo.book_id)==false and daypassInfo.is_show==0)then
                if(uiform==nil)then
                    uiform = logic.UIMgr:Open(logic.uiid.UIDayPassForm);
                    uiform:SetInfo(daypassInfo)
                else
                    uiform.uiform:Appear();
                    uiform:SetInfo(daypassInfo)
                end
            else
                this:ShowNextWindow()
            end
            CS.AppsFlyerManager.Instance:GetFirstActionLog();
        else
            logic.UIMgr:Close(logic.uiid.UIDayPassForm);
            this:ShowNextWindow()
        end
    else
        logic.UIMgr:Close(logic.uiid.UIDayPassForm);
        this:ShowNextWindow()
    end
end

function WindowConfig:ShowSignTip()
    if(Cache.SignInCache.activity_login.is_receive==0)then
        logic.UIMgr:Open(logic.uiid.UISignTipForm);
        CS.AppsFlyerManager.Instance:GetFirstActionLog();
    else
        this:ShowNextWindow()
    end
end

function WindowConfig:ShowAgreement()
    if not logic.cs.IGGSDKMrg.isNewUser then
        logic.cs.IGGAgreementMrg:OnRequestStatusCustomClick()
    else
        this:ShowNextWindow()
    end
    
end

function WindowConfig:ShowNextWindow()
    WindowIndex = WindowIndex + 1;
    local windowlist=Cache.PopWindowCache.window_list;
    if(windowlist)then
        local len=table.length(windowlist);
        if(len>0 and WindowIndex<=len)then
            if(windowlist[WindowIndex].id==1)then
                self:ShowFirstCharge();
            elseif(windowlist[WindowIndex].id==2)then
                self:ShowDayPass();
            elseif(windowlist[WindowIndex].id==3)then
                self:ShowNewBookTips();
            elseif(windowlist[WindowIndex].id==4)then
                self:ShowSignTip();
            elseif(windowlist[WindowIndex].id==5)then
                self:ShowAgreement();
            end
        end
    end
end



--析构函数
function WindowConfig:__delete()
    
end


WindowConfig = WindowConfig:GetInstance()
return WindowConfig
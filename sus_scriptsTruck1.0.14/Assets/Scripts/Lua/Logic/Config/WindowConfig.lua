local BaseClass = core.Class
local WindowConfig = BaseClass("WindowConfig", core.Singleton)
local this = WindowConfig
local FirstChargeNeedShown = true
local BookPopupNeedShown = true
local WindowIndex = 0
local WindowsList = {}

--region【构造函数】
function WindowConfig:__init()
end
--endregion

function WindowConfig:SetWindowsList(data)
    WindowsList = {}
    WindowIndex = 0
    this.NeedShowNextWindow = false
    for k, v in pairs(data) do
        if tonumber(v.id) == 1 then
            table.insert(WindowsList,this.ShowFirstCharge)
        elseif tonumber(v.id) == 3 then
            table.insert(WindowsList,this.ShowNewBookTips)
        elseif tonumber(v.id) == 4 then
            table.insert(WindowsList,this.ShowSignTip)
        elseif tonumber(v.id) == 5 then
            table.insert(WindowsList,this.ShowAgreement)
        end
    end
end

function WindowConfig:ShowFirstCharge()
    if tonumber(logic.cs.UserDataManager.selfBookInfo.data.first_recharge_switch) == 1
            and not logic.cs.IGGSDKMrg.isNewUser and FirstChargeNeedShown then
        logic.gameHttp:GetRewardConfig(function(result)
            if(string.IsNullOrEmpty(result))then return; end
            logic.debug.Log("----GetRewardConfig---->" .. result)
            local json = core.json.Derialize(result);
            local code = tonumber(json.code);
            if(code == 200)then
                Cache.ActivityCache:UpdatedRewardConfig(json.data);
                FirstChargeNeedShown = false
                logic.UIMgr:Open(logic.uiid.UIFirstChargeForm);
            end
        end)
    else
        this:ShowNextWindow()
    end
end

function WindowConfig:ShowNewBookTips()
    if BookPopupNeedShown then
        logic.gameHttp:GetRecommendBookPopup(function(result1)
            logic.debug.Log("----GetRecommendBookPopup---->" .. result1);
            local json = core.json.Derialize(result1);
            local code = tonumber(json.code)
            if(code == 200)then
                BookPopupNeedShown = false
                local uiView = logic.UIMgr:Open(logic.uiid.UINewBookTipsForm);
                uiView:SetData(json.data.book_list)
            end
        end)
    else
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
    WindowIndex = WindowIndex + 1
    local nextWindow = WindowsList[WindowIndex]
    if nextWindow then
        nextWindow()
    end
end

--析构函数
function WindowConfig:__delete()
    
end


WindowConfig = WindowConfig:GetInstance()
return WindowConfig
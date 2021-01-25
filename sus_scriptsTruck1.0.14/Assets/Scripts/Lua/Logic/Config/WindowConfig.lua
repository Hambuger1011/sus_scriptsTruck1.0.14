local BaseClass = core.Class
local WindowConfig = BaseClass("WindowConfig", core.Singleton)
local FirstChargeNeedShown = true
local BookPopupNeedShown = true

--region【构造函数】
function WindowConfig:__init()
    self.m_curPage=0;
    self.m_maxPage=1;
end
--endregion

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
    end
end

function WindowConfig:ShowNewBookTips()
    if BookPopupNeedShown then
        logic.gameHttp:GetRecommendBookPopup(function(result1)
            logic.debug.Log("----GetRecommendBookPopup---->" .. result1);
            local json1 = core.json.Derialize(result1);
            local code1 = tonumber(json1.code)
            if(code1 == 200)then
                BookPopupNeedShown = false
                local uiView = logic.UIMgr:Open(logic.uiid.UINewBookTipsForm);
                uiView:SetData(json1.data.book_list)
            end
        end)
    end
end

function WindowConfig:ShowSignTip()
    if(Cache.SignInCache.activity_login.is_receive==0)then
        logic.UIMgr:Open(logic.uiid.UISignTipForm);
        CS.AppsFlyerManager.Instance:GetFirstActionLog();
    end
end

--析构函数
function WindowConfig:__delete()
end


WindowConfig = WindowConfig:GetInstance()
return WindowConfig
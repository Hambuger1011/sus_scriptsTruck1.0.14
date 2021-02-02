local FirstChargeControl = core.Class("FirstChargeControl", core.Singleton)

local UIFirstChargeForm=nil;
-- 构造函数
function FirstChargeControl:__init()
end



--region【SetData】
function FirstChargeControl:SetData(firstCharge)
    UIFirstChargeForm=firstCharge;
end
--endregion


--region【获取通用奖励配置】---【通用奖励】
function FirstChargeControl:GetRewardConfigRequest()
    logic.gameHttp:GetRewardConfig(function(result) self:GetRewardConfig(result); end)
end
--endregion


--region【获取通用奖励配置*响应】---【通用奖励】
function FirstChargeControl:GetRewardConfig(result)
    if(string.IsNullOrEmpty(result))then return; end
    logic.debug.Log("----GetRewardConfig---->" .. result)
    local json = core.json.Derialize(result);
    local code = tonumber(json.code);
    if(code == 200)then
        --缓存奖励数据
        Cache.ActivityCache:UpdatedRewardConfig(json.data);
        logic.UIMgr:Open(logic.uiid.UIFirstChargeForm);
    end

end
--endregion




--析构函数
function FirstChargeControl:__delete()

end


FirstChargeControl = FirstChargeControl:GetInstance()
return FirstChargeControl
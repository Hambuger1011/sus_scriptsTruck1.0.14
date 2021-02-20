local InvestmentControl = core.Class("InvestmentControl", core.Singleton)

local UIInvestmentForm=nil;
--region【构造函数】
function InvestmentControl:__init()
    self.isEndTime=false;
end
--endregion


--region【设置界面】
function InvestmentControl:SetData(investmentform)
    UIInvestmentForm=investmentform;
    self.isEndTime=false;
end
--endregion


--region 【获取投资活动计划列表  请求请求】
function InvestmentControl:PopUpDayPassBookRequest(activity_id)
    logic.gameHttp:GetInvestPlanList(activity_id,function(result) self:GetInvestPlanList(result); end)
end

--endregion


--region 【获取投资活动计划列表  响应响应】
function InvestmentControl:GetInvestPlanList(result)
    logic.debug.Log("----GetInvestPlanList---->" .. result);
    local json = core.json.Derialize(result);
    local code = tonumber(json.code)
    if(code == 200)then
        --【缓存】
        Cache.InvestmentCache:UpdateData(json.data)

        if(Cache.InvestmentCache.Investment1.is_join==1 or Cache.InvestmentCache.Investment2.is_join==1)then
            -- 【获取投资活动计划状态  请求请求】
            self:GetInvestPlanStatusRequest(Cache.InvestmentCache.Investment1.activity_id);
        else
            if(UIInvestmentForm)then
                UIInvestmentForm:SetInfo();
            else
                local uiform = logic.UIMgr:Open(logic.uiid.UIInvestmentForm);
                if(uiform)then
                    --刷新
                    uiform:SetInfo();
                end
            end
        end
    end
end

--endregion


--region 【参加投资活动计划  请求请求】
function InvestmentControl:JoinInvestPlanRequest(activity_id,number)
    logic.gameHttp:JoinInvestPlan(activity_id,number,function(result) self:JoinInvestPlan(activity_id,result); end)
end

--endregion


--region 【参加投资活动计划  响应响应】
function InvestmentControl:JoinInvestPlan(activity_id,result)
    logic.debug.Log("----JoinInvestPlan---->" .. result);
    local json = core.json.Derialize(result);
    local code = tonumber(json.code)
    if(code == 200)then
        logic.cs.UserDataManager:ResetMoney(1, tonumber(json.data.bkey))
        logic.cs.UserDataManager:ResetMoney(2, tonumber(json.data.diamond))

        -- 【获取投资活动计划状态  请求请求】
        self:GetInvestPlanStatusRequest(activity_id);
    end
end

--endregion


--region 【获取投资活动计划状态  请求请求】
function InvestmentControl:GetInvestPlanStatusRequest(activity_id)
    logic.gameHttp:GetInvestPlanStatus(activity_id,function(result) self:GetInvestPlanStatus(result); end)
end

--endregion


--region 【获取投资活动计划状态  响应响应】
function InvestmentControl:GetInvestPlanStatus(result)
    logic.debug.Log("----GetInvestPlanStatus---->" .. result);
    local json = core.json.Derialize(result);
    local code = tonumber(json.code)
    if(code == 200)then
        --【缓存】
        Cache.InvestmentCache:UpdateJoinInfo(json.data)

        if(UIInvestmentForm)then
            UIInvestmentForm:SetInfo2();
        else
            local uiform = logic.UIMgr:Open(logic.uiid.UIInvestmentForm);
            if(uiform)then
                --刷新
                uiform:SetInfo2();
            end
        end

        if(json.data.join_info.countdown<=0)then
            self.isEndTime=true;
            if(UIInvestmentForm)then
                UIInvestmentForm:EnterCollect();
            else
                local uiform = logic.UIMgr:Open(logic.uiid.UIInvestmentForm);
                if(uiform)then
                    --刷新
                    uiform:EnterCollect();
                end
            end
        end

    end
end

--endregion



--region 【领取投资活动计划奖励  请求请求】
function InvestmentControl:ReceiveInvestPlanRequest(activity_id)
    logic.gameHttp:ReceiveInvestPlan(activity_id,function(result) self:ReceiveInvestPlan(result); end)
end

--endregion


--region 【领取投资活动计划奖励  响应响应】
function InvestmentControl:ReceiveInvestPlan(result)
    logic.debug.Log("----GetInvestPlanStatus---->" .. result);
    local json = core.json.Derialize(result);
    local code = tonumber(json.code)
    if(code == 200)then
        logic.cs.UserDataManager:ResetMoney(1, tonumber(json.data.bkey))
        logic.cs.UserDataManager:ResetMoney(2, tonumber(json.data.diamond))


        if(UIInvestmentForm)then
            UIInvestmentForm:EnterCollect2(Cache.InvestmentCache.plan_info);
        end
    end
end

--endregion



--region 【获取投资活动计划状态  请求请求2】
function InvestmentControl:GetInvestPlanStatusRequest2(activity_id)
    logic.gameHttp:GetInvestPlanStatus(activity_id,function(result) self:GetInvestPlanStatus2(result); end)
end

--endregion


--region 【获取投资活动计划状态  响应响应2】
function InvestmentControl:GetInvestPlanStatus2(result)
    logic.debug.Log("----GetInvestPlanStatus2---->" .. result);
    local json = core.json.Derialize(result);
    local code = tonumber(json.code)
    if(code == 200)then
        Cache.InvestmentCache.join_info.countdown=json.data.join_info.countdown;
        local _time=json.data.join_info.countdown;
        if(_time<=0)then
            self.isEndTime=true;
        end

        if(_time and _time>0)then
            --刷新倒计时
            if(UIInvestmentForm)then
                UIInvestmentForm:ShowTime();
            end
        else

            local diamond_count=0;
            local key_count=0;
            local itemArr={};

            local itemlist=json.data.plan_info.prize2;
            local len=table.length(itemlist);
            if(itemlist and len)then
                for i = 1, len do
                    if(itemlist[i].id==1)then
                        diamond_count=itemlist[i].num;
                    elseif(itemlist[i].id==2)then
                        key_count=itemlist[i].num;
                    else
                        table.insert(itemArr,itemlist[i]);
                    end
                end
            end
            GameHelper.ShowCollectItem(diamond_count,key_count,"CLAIM",
                    function()  self:ReceiveInvestPlanRequest(json.data.plan_info.activity_id); end,itemArr);
        end
    end
end

--endregion


--region【刷新显示】
local isFirst=false;
function InvestmentControl:UpdateCountdown()
    if(self.isEndTime==true)then return; end

    local _time= Cache.InvestmentCache.join_info.countdown;
    --剩余倒计时大于0
    if(_time and _time>0)then
        if(isFirst==true)then
            Cache.InvestmentCache.join_info.countdown=Cache.InvestmentCache.join_info.countdown-120;
        end
        if(Cache.InvestmentCache.join_info.countdown<0)then
            Cache.InvestmentCache.join_info.countdown=0;
            if(UIInvestmentForm)then
                UIInvestmentForm:EnterCollect();
                return;
            end
        end

        --刷新倒计时
        if(_time and _time>0)then
            if(UIInvestmentForm)then
                UIInvestmentForm:ShowTime();
            end
        end

    end
    isFirst=true;
end
--endregion




--析构函数
function InvestmentControl:__delete()
end


InvestmentControl = InvestmentControl:GetInstance()
return InvestmentControl
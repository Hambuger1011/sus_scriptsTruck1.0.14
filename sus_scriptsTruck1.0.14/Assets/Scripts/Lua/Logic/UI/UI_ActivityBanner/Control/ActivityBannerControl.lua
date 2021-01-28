local ActivityBannerControl = core.Class("ActivityBannerControl", core.Singleton)

--【UI界面】
local UIActivityBannerForm=nil;
-- 构造函数
function ActivityBannerControl:__init()
    self.TimerList={};
end

function ActivityBannerControl:SetData(uiactivitybanner)
    UIActivityBannerForm=uiactivitybanner;

    if(uiactivitybanner==nil)then
        self:ClearTimer();--【销毁所有计时器】
    end
end


--region【刷新计时器】
local isFirst=false;
function ActivityBannerControl:UpdateCountdown()
    if(GameHelper.islistHave(Cache.LimitTimeActivityCache.ActivityList)==true)then
        local len=table.length(Cache.LimitTimeActivityCache.ActivityList);
        for i = 1, len do
            --如果本活动开启
            if(Cache.LimitTimeActivityCache.ActivityList[i].is_open==1)then
                --剩余倒计时大于0
                if(Cache.LimitTimeActivityCache.ActivityList[i].countdown>0)then
                    if(isFirst==true)then
                        Cache.LimitTimeActivityCache.ActivityList[i].countdown=Cache.LimitTimeActivityCache.ActivityList[i].countdown-120;
                    end

                    if(Cache.LimitTimeActivityCache.ActivityList[i].countdown<0)then
                        Cache.LimitTimeActivityCache.ActivityList[i].countdown=0;
                    end
                end
            end
        end
    end
    self:DefaultCountDown();
    isFirst=true;
end

--【刷新展示】【刷新计时器】
function ActivityBannerControl:UpdateShow()
    --if(Cache.LimitTimeActivityCache:IsOpen()==true or Cache.MainCache.migration.migration_web_switch==1)then  --如果有活动
    if(Cache.LimitTimeActivityCache:IsOpen()==true)then  --如果有活动
        --【打开活动Banner】
        GameController.ActivityControl:UpdateActivityBanner();
        --【刷新展示计时器】
        self:DefaultCountDown()
    else
        self:CloseUI();
    end
end


--刷新展示计时器
function ActivityBannerControl:DefaultCountDown()
    if(UIActivityBannerForm)then
        UIActivityBannerForm:DefaultCountDown();
    end
end


function ActivityBannerControl:CloseUI()
    --【关闭计时器】
    self:ClearTimer()
    --【关闭销毁界面】
    if(UIActivityBannerForm)then
        UIActivityBannerForm:OnExitClick();
    end
    --【主界面banner 位置刷新】
    GameController.MainFormControl:MoveBanner();
end



--endregion


--region 【定时请求】【活动】【彩蛋】
function ActivityBannerControl:TimerRequest()
    --【销毁计时器】
    self:ClearTimer();

    if(GameHelper.islistHave(Cache.LimitTimeActivityCache.ActivityList)==true)then
        local len=table.length(Cache.LimitTimeActivityCache.ActivityList);
        for i = 1, len do
            --是否添加
            local isAdd=true;
            --如果本活动开启
            if(Cache.LimitTimeActivityCache.ActivityList[i].is_open==1)then
                --剩余倒计时大于0
                if(Cache.LimitTimeActivityCache.ActivityList[i].countdown>0)then
                    --延迟10秒
                    local countdown=Cache.LimitTimeActivityCache.ActivityList[i].countdown+10;
                    if(countdown>3600)then  --如果剩余时间大于 1小时   不开启定时器
                        isAdd=false;
                    end

                    if(isAdd==true)then
                        local _id=Cache.LimitTimeActivityCache.ActivityList[i].id;
                        --【取出定时器】
                        local _Timer = table.trygetvalue(self.TimerList,_id);
                        if(_Timer==nil)then --【如果没有缓存】
                            _Timer = core.Timer.New(function()

                                --【销毁计时器】
                                _Timer:Stop();
                                self.TimerList[_id]=nil;
                                --【请求数据】【刷新计时】
                                GameController.ActivityControl:GetActivityInfoRequest(_id);

                            end,countdown,-1);   --【生成一个新的】

                            self.TimerList[_id]=_Timer; --【缓存定时器】
                        end
                        if(_Timer)then
                            _Timer:Start();  --【开启定时器】
                        end
                    end
                end
            end
        end
    end
end
--endregion


--region【销毁计时器】
function ActivityBannerControl:ClearTimer()
    if(self.TimerList)then
        for i, v in pairs(self.TimerList) do
            if(v)then
                v:Stop();
                v=nil;
            end
        end
        self.TimerList={};
    end
end
--endregion


--析构函数
function ActivityBannerControl:__delete()
end


ActivityBannerControl = ActivityBannerControl:GetInstance()
return ActivityBannerControl
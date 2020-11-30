local BaseClass = core.Class
local ActivityBannerControl = BaseClass("ActivityBannerControl", core.Singleton)

local UIActivityBannerForm=nil;
local ColoredEgg_Timer=nil;
local SpinDraw_Timer=nil;
local FreeKey_Timer=nil;

-- 构造函数
function ActivityBannerControl:__init()
end

function ActivityBannerControl:SetData(uiactivitybanner)
    UIActivityBannerForm=uiactivitybanner;

    if(uiactivitybanner==nil)then
        if(ColoredEgg_Timer)then ColoredEgg_Timer:Stop(); ColoredEgg_Timer = nil end
        if(SpinDraw_Timer)then SpinDraw_Timer:Stop(); SpinDraw_Timer = nil end
        if(FreeKey_Timer)then FreeKey_Timer:Stop(); FreeKey_Timer = nil end
    end
end


--region【刷新计时器】
local isFirst=false;
function ActivityBannerControl:UpdateCountdown()

    --local num=0;

    --【彩蛋 活动开启】【刷新展示Banner 倒计时】
    if(Cache.LimitTimeActivityCache.ColoredEgg.is_open==1)then
        --剩余倒计时大于0
        if(Cache.LimitTimeActivityCache.ColoredEgg.countdown>0)then

            if(isFirst==true)then
                Cache.LimitTimeActivityCache.ColoredEgg.countdown=Cache.LimitTimeActivityCache.ColoredEgg.countdown-120;
            end

            if(Cache.LimitTimeActivityCache.ColoredEgg.countdown<0)then
                Cache.LimitTimeActivityCache.ColoredEgg.countdown=0;
            end
        end
        --num=num+1;
    end

    --【旋转抽奖 活动开启】【刷新展示Banner 倒计时】
    if(Cache.LimitTimeActivityCache.SpinDraw.is_open==1)then
        --剩余倒计时大于0
        if(Cache.LimitTimeActivityCache.SpinDraw.countdown>0)then

            if(isFirst==true)then
                Cache.LimitTimeActivityCache.SpinDraw.countdown=Cache.LimitTimeActivityCache.SpinDraw.countdown-120;
            end
            if(Cache.LimitTimeActivityCache.SpinDraw.countdown<0)then
                Cache.LimitTimeActivityCache.SpinDraw.countdown=0;
            end
        end
        --num=num+1;
    end

    --【免费钥匙 活动开启】【刷新展示Banner 倒计时】
    if(Cache.LimitTimeActivityCache.FreeKey.is_open==1)then
        --剩余倒计时大于0
        if(Cache.LimitTimeActivityCache.FreeKey.countdown>0)then

            if(isFirst==true)then
                Cache.LimitTimeActivityCache.FreeKey.countdown=Cache.LimitTimeActivityCache.FreeKey.countdown-120;
            end

            if(Cache.LimitTimeActivityCache.FreeKey.countdown<0)then
                Cache.LimitTimeActivityCache.FreeKey.countdown=0;
            end
        end
        --num=num+1;
    end

    self:DefaultCountDown();

    isFirst=true;
end


--刷新展示计时器
function ActivityBannerControl:DefaultCountDown()
    if(UIActivityBannerForm)then
        UIActivityBannerForm:DefaultCountDown();
    end
end



--endregion


--region 【定时请求】【活动】【彩蛋】

function ActivityBannerControl:Timer_ColoredEggRequest()
    --【销毁计时器】
    self:Clear_ColoredEggTimer();
    --【彩蛋 活动开启】
    if(Cache.LimitTimeActivityCache.ColoredEgg.is_open==1)then
        --剩余倒计时大于0
        if(Cache.LimitTimeActivityCache.ColoredEgg.countdown>0)then
            --延迟10秒
            local countdown=Cache.LimitTimeActivityCache.ColoredEgg.countdown+10;
            if(countdown>3600)then  --如果剩余时间大于 1小时   不开启定时器
                return;
            end
            ColoredEgg_Timer = core.Timer.New(function() self:Timer_ColoredEggCallBack(); end,countdown,-1);
            ColoredEgg_Timer:Start();
        end
    end
end

function ActivityBannerControl:Timer_ColoredEggCallBack()
    --【销毁计时器】
    self:Clear_ColoredEggTimer();
   --【请求彩蛋】
   GameController.ActivityControl:GetActivityInfoRequest(EnumActivity.ColoredEgg);
end

--endregion


--region 【定时请求】【活动】【转盘抽奖】

function ActivityBannerControl:Timer_SpinDrawRequest()
    --【销毁计时器】
    self:Clear_SpinDrawTimer();
    --【旋转抽奖 活动开启】【刷新展示Banner 倒计时】
    if(Cache.LimitTimeActivityCache.SpinDraw.is_open==1)then
        --剩余倒计时大于0
        if(Cache.LimitTimeActivityCache.SpinDraw.countdown>0)then
            --延迟10秒
            local countdown=Cache.LimitTimeActivityCache.SpinDraw.countdown+10;
            if(countdown>3600)then  --如果剩余时间大于 1小时   不开启定时器
                return;
            end
            SpinDraw_Timer = core.Timer.New(function() self:Timer_SpinDrawCallBack(); end,countdown,-1);
            SpinDraw_Timer:Start();
        end
    end
end

function ActivityBannerControl:Timer_SpinDrawCallBack()
    --【销毁计时器】
    self:Clear_SpinDrawTimer();
    --【请求彩蛋】
    GameController.ActivityControl:GetActivityInfoRequest(EnumActivity.SpinDraw);
end

--endregion


--region 【定时请求】【活动】【转盘抽奖】【全书免费】

function ActivityBannerControl:Timer_FreeKeyRequest()
    --【销毁计时器】
    self:Clear_FreeKeyTimer();
    --【免费钥匙 活动开启】【刷新展示Banner 倒计时】
    if(Cache.LimitTimeActivityCache.FreeKey.is_open==1)then
        --剩余倒计时大于0
        if(Cache.LimitTimeActivityCache.FreeKey.countdown>0)then
            --延迟10秒
            local countdown=Cache.LimitTimeActivityCache.FreeKey.countdown+10;
            if(countdown>3600)then  --如果剩余时间大于 1小时   不开启定时器
                return;
            end
            FreeKey_Timer = core.Timer.New(function() self:Timer_FreeKeyCallBack(); end,countdown,-1);
            FreeKey_Timer:Start();
        end
    end
end

function ActivityBannerControl:Timer_FreeKeyCallBack()
    --【销毁计时器】
    self:Clear_FreeKeyTimer();
    --【请求彩蛋】
    GameController.ActivityControl:GetActivityInfoRequest(EnumActivity.FreeKey);
end

--endregion


--region【销毁计时器】【彩蛋】
function ActivityBannerControl:Clear_ColoredEggTimer()
    if(ColoredEgg_Timer)then
        ColoredEgg_Timer:Stop();
        ColoredEgg_Timer = nil
    end
end
--endregion


--region【销毁计时器】【彩蛋】
function ActivityBannerControl:Clear_SpinDrawTimer()
    if(SpinDraw_Timer)then
        SpinDraw_Timer:Stop();
        SpinDraw_Timer = nil
    end
end
--endregion


--region【销毁计时器】【彩蛋】
function ActivityBannerControl:Clear_FreeKeyTimer()
    if(FreeKey_Timer)then
        FreeKey_Timer:Stop();
        FreeKey_Timer = nil
    end
end
--endregion

--析构函数
function ActivityBannerControl:__delete()
end


ActivityBannerControl = ActivityBannerControl:GetInstance()
return ActivityBannerControl
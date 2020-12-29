local BaseClass = core.Class
local FreeRewardPanel = BaseClass("FreeRewardPanel")



--region【init】
function FreeRewardPanel:__init(gameObject)

    self.gameObject=gameObject;
    self.TimeText =CS.DisplayUtil.GetChild(gameObject, "TimeText"):GetComponent("Text");
    self.WatchBtn =CS.DisplayUtil.GetChild(gameObject, "WatchBtn");
    self.WatchBtnText =CS.DisplayUtil.GetChild(gameObject, "WatchBtnText"):GetComponent("Text");
    self.WatchBtnMask =CS.DisplayUtil.GetChild(gameObject, "WatchBtnMask");
    self.WatchBtnMaskText =CS.DisplayUtil.GetChild(gameObject, "WatchBtnMaskText"):GetComponent("Text");
    self.CDText =CS.DisplayUtil.GetChild(gameObject, "CDText"):GetComponent("Text");


    logic.cs.UIEventListener.AddOnClickListener(self.WatchBtn,function(data) self:WatchBtnClick() end)

    --广告剩余领取此时
    self.ads_number = 0;
end
--endregion


--region 【刷新观看广告】
function FreeRewardPanel:UpdateFreeRewardPanel()
    --开启倒计时 计时器
    GameHelper.FRPanel_CountDown(self.TimeText,self.CDText);
    self.ads_number = Cache.ActivityCache.ad_number;

    self.WatchBtnText.text = string.format("WATCH (%s)",self.ads_number)
    self.WatchBtnMaskText.text = string.format("WATCH (%s)",self.ads_number)
    if(self.ads_number == 0)then
        self.WatchBtnMask.gameObject:SetActiveEx(true);
        self.WatchBtn.gameObject:SetActiveEx(false);
    else
        self.WatchBtnMask.gameObject:SetActiveEx(false);
        self.WatchBtn.gameObject:SetActiveEx(true);
    end
end
--endregion


function FreeRewardPanel:WatchBtnClick()

    if(self.ads_number and self.ads_number > 0)then
        if(CS.XLuaHelper.GetPlatFormType()==0)then
            self:AdAwardRequest();
        else
            --播放活动页面 激励视频广告
            CS.GoogleAdmobAds.Instance.acitityRewardedAd:ShowRewardedAd_Activity(function() self:AdAwardRequest(); end);
        end
    end
end

--看广告完成后请求奖励
function FreeRewardPanel:AdAwardRequest()
    logic.debug.Log("广告观看结果")
    --领取活动广告奖励 请求
    GameController.ActivityControl:ReceiveDailyAdAwardRequest();
end

function FreeRewardPanel:ReceiveDailyAdAward()
    Cache.ActivityCache.ad_number = Cache.ActivityCache.ad_number - 1

    if(Cache.ActivityCache.ad_number == 0)then
        self.WatchBtnMask.gameObject:SetActiveEx(true)
        self.WatchBtn.gameObject:SetActiveEx(false)
    end
end


function FreeRewardPanel:StartCD()
    self.CDText.gameObject:SetActiveEx(true);
    self.WatchBtnMask.gameObject:SetActiveEx(true)
    self.WatchBtn.gameObject:SetActiveEx(false)

    GameHelper.FR_CDNum=60;
    GameHelper.StartFRTimer();
end

function FreeRewardPanel:EndCD()
    self.CDText.gameObject:SetActiveEx(false);
    self.WatchBtnMask.gameObject:SetActiveEx(false);
    self.WatchBtn.gameObject:SetActiveEx(true);
end


--region【销毁】
function FreeRewardPanel:__delete()

    if(self.WatchBtn)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.WatchBtn,function(data) self:WatchBtnClick() end);
    end

    --销毁计时器
    GameHelper.CloseFR_Timer();
    self.TimeText = nil;
    self.WatchBtn = nil;
    self.WatchBtnText = nil;
    self.WatchBtnMask = nil;
    self.WatchBtnMaskText = nil;
    self.ads_number = nil;

    self.gameObject=nil;
end
--endregion


return FreeRewardPanel

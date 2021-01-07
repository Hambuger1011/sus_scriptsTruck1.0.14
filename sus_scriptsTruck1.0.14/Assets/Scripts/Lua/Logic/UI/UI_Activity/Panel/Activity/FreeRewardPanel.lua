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

end
--endregion


--region 【刷新观看广告】
function FreeRewardPanel:UpdateFreeRewardPanel()
    --开启倒计时 计时器
    GameHelper.FRPanel_CountDown(self.TimeText);

    self.WatchBtnText.text = string.format("WATCH (%s)",Cache.ActivityCache.ad_number);
    self.WatchBtnMaskText.text = string.format("WATCH (%s)",Cache.ActivityCache.ad_number);

    --【设置按钮状态】
    self:SetButtonState();
end
--endregion


function FreeRewardPanel:WatchBtnClick()
    if(Cache.ActivityCache.ad_number and Cache.ActivityCache.ad_number > 0)then
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
    --【广告剩余次数-1】
    Cache.ActivityCache.ad_number = Cache.ActivityCache.ad_number - 1
    --【设置按钮状态】
    self:SetButtonState();
end


function FreeRewardPanel:ShowCD(txt)
    if(txt)then
        self.CDText.text=txt;
    end
end


function FreeRewardPanel:StartCD()
    --【设置CD】【开始计时】
    GameHelper.FR_CDNum=60;
    GameHelper.FRCD_CountDown();

    --【设置按钮状态】
    self:SetButtonState();
end

function FreeRewardPanel:EndCD()
    --【设置按钮状态】
    self:SetButtonState();
end


function FreeRewardPanel:SetButtonState()
    if(Cache.ActivityCache.ad_number <= 0)then  --【次数为0】
        self.WatchBtnMask.gameObject:SetActiveEx(true);  --【灰色按钮】
        self.WatchBtn.gameObject:SetActiveEx(false);    --【亮色按钮】
        self.CDText.gameObject:SetActiveEx(false);    --【CD文字】

    else
        self.WatchBtnMask.gameObject:SetActiveEx(false); --【灰色按钮】
        self.WatchBtn.gameObject:SetActiveEx(true);   --【亮色按钮】

        if(GameHelper.FR_CDNum<=0)then
            self.CDText.gameObject:SetActiveEx(false); --【CD文字】
        else
            self.CDText.gameObject:SetActiveEx(true);  --【CD文字】
            self.WatchBtnMask.gameObject:SetActiveEx(true);  --【灰色按钮】
            self.WatchBtn.gameObject:SetActiveEx(false); --【亮色按钮】
        end
    end

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
    self.CDText = nil;

    self.gameObject=nil;
end
--endregion


return FreeRewardPanel

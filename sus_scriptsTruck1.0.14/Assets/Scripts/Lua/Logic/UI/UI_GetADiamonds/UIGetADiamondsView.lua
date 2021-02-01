local BaseClass = core.Class
local UIView = core.UIView
local UIGetADiamondsView = BaseClass("UIGetADiamondsView", UIView)
local base = UIView

local uiid = logic.uiid
UIGetADiamondsView.config = {
	ID = uiid.GetADiamonds,
	AssetName = 'UI/Resident/UI/Canvas_GetADiamonds'
}

function UIGetADiamondsView:OnInitView()
    UIView.OnInitView(self)
    local get = logic.cs.LuaHelper.GetComponent
    local root = self.uiform.transform
    self.btnWatchAds = get(root,'Canvas/Mask/watchButton',typeof(logic.cs.UITweenButton))
    self.btnClose = get(root,'Canvas/Mask/CancelButton',typeof(logic.cs.UITweenButton))

  
    self.btnWatchAds.onClick:AddListener(function()
        self:OnWatchAdsClick()
    end)
    self.btnClose.onClick:AddListener(function()
        self:__Close()
    end)
end

function UIGetADiamondsView:OnOpen()
    UIView.OnOpen(self)
end

function UIGetADiamondsView:OnClose()
    UIView.OnClose(self)
end


function UIGetADiamondsView:Complete()
    self:__Close()
end

function UIGetADiamondsView:OnWatchAdsClick()
    if not logic.cs.UserDataManager.userInfo or logic.cs.UserDataManager.lotteryDrawInfo.data.bookadcount <= 0 then
        local cfg = logic.cs.GameDataMgr.table:GetLocalizationById(150)
        logic.cs.UITipsMgr:PopupTips(cfg, false)
        return
    end
    logic.debug.Log('可看广告次数:'..logic.cs.UserDataManager.lotteryDrawInfo.data.bookadcount)

    local bookData = logic.bookReadingMgr.bookData
    logic.cs.talkingdata:WatchTheAds(1)
    logic.cs.SdkMgr.ads:ShowRewardBasedVideo("chapter-completed", function(isOK)
        if not isOK then
            return
        end
        logic.gameHttp:GetAdsReward(bookData.BookID,2,function(result)
            local json = core.json.Derialize(result)
            local code = tonumber(json.code)
            if code == 200 then
                logic.cs.UserDataManager:Set_adsRewardResultInfo(result)
                logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.RewardWin)
                logic.cs.UserDataManager:ResetMoney(1, json.data.bkey)
                logic.cs.UserDataManager:ResetMoney(2, json.data.diamond)
                --logic.cs.UserDataManager.lotteryDrawInfo.data.bookadcount = json.data.bookadcount
                logic.cs.UserDataManager.lotteryDrawInfo.data.bookadcount = logic.cs.UserDataManager.lotteryDrawInfo.data.bookadcount - 1
                logic.cs.talkingdata:WatchTheAds(2)

                --AF事件记录*  用户点击广告进行商店跳转
                CS.AppsFlyerManager.Instance:ADS_CLICK();

                self:Complete()
            elseif code == 201 then --付费用户无法领取奖励
                logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.LoseFail)
            elseif code == 202 then --你的选项上一次已扣费
                logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.LoseFail)
                local cfg = logic.cs.GameDataMgr.table:GetLocalizationById(191)
                logic.cs.UITipsMgr:PopupTips(cfg, false)
            elseif code == 203 then --章节免费
                logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.LoseFail)
            elseif code == 204 then --对话不存在
                logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.LoseFail)
            elseif code == 205 then --你的钻石不足，请先充值
                logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.LoseFail)
            elseif code == 208 then --扣费失败
                logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.LoseFail)
                logic.debug.LogError("--BookDialogOptionCallBack--扣费失败,BookId:" .. bookData.BookID .. " DialogId:" .. self.component.cfg.dialogid);
            end
            if code ~= 200 then
                logic.cs.UIAlertMgr:Show('TIPS', json.msg)
            end
        end)
    end)


end


return UIGetADiamondsView
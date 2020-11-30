local Class = core.Class
local AdsTicker = Class("AdsTicker")

function AdsTicker:__init(root)
	self.AdsInterval = 60 * 3
	self.lastTime = 0
	self.transform = root
	self.gameObject = root.gameObject
	self.gameObject:SetActiveEx(false)
	self.txtTick = logic.cs.LuaHelper.GetComponent(root, 'timer',typeof(logic.cs.Text))
end

function AdsTicker:__delete()
	if self.timer then
		self.timer:Stop()
		self.timer = nil
	end
end

function AdsTicker:IsAdsEnabled()
	return logic.cs.UserDataManager.userInfo.data.userinfo.isscreenad == 0
end

function AdsTicker:Start()
	if not self:IsAdsEnabled() then
		return
	end
	if self.timer then
		return
	end
	self.lastTime = core.Time.time
	--定时器test
	 self.timer = core.Timer.New(function()
		if not logic.bookReadingMgr.isReading then
			logic.debug.LogError('fuck....')
			if self.timer then
				self.timer:Stop()
				self.timer = nil
			end
			return
		end

		if not self:IsAdsEnabled() then
			self.timer:Stop()
			self.timer = nil
			return
		end

		--剩余时间
	     local delta = self.AdsInterval - (core.Time.time - self.lastTime)
		if delta <= 0 then	--时间到，播放广告
			self.gameObject:SetActiveEx(false)
			self.lastTime = core.Time.time
			logic.cs.SdkMgr.ads:ShowInterstitial(DisjunctorType.Ads_Book,"book reading")
		elseif delta <= 10 then	--10秒内倒计时
			self.gameObject:SetActiveEx(true)
			self.txtTick.text = string.format('%.0f',delta)
		else		--倒计时中
			self.gameObject:SetActiveEx(false)
			if logic.config.isDebugMode then
				self.txtTick.text = string.format('%.0f',delta)
			end
		end
	 end,0.5,-1)
	 self.timer:Start()
end


return AdsTicker
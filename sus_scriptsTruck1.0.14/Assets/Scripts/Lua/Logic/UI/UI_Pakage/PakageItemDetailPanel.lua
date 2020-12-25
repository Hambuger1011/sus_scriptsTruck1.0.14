---@class PakageItemDetailPanel
local PakageItemDetailPanel  = core.Class('PakageItemDetailPanel')



function PakageItemDetailPanel:__init(gameObject, parentUI)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.parentUI = parentUI
    self.btnMask = logic.cs.LuaHelper.GetComponent(self.transform, "bntMask",typeof(logic.cs.Button))
    self.imgIcon = logic.cs.LuaHelper.GetComponent(self.transform, "frame/icon",typeof(logic.cs.Image))
    self.txtTitle = logic.cs.LuaHelper.GetComponent(self.transform, "frame/txtTitle",typeof(logic.cs.Text))
    self.txtCountDown = logic.cs.LuaHelper.GetComponent(self.transform, "frame/countDowd/text",typeof(logic.cs.Text))
    self.txtDescript = logic.cs.LuaHelper.GetComponent(self.transform, "frame/txtDesceription",typeof(logic.cs.Text))
    self.btnMask.onClick:AddListener(function()
        self:OnClose()
    end)
end



function PakageItemDetailPanel:SetData(itemData, deltaTime)
    self.itemData = itemData
    self.txtTitle.text = itemData.name or ""
    self.txtDescript.text = itemData.describe or ""
    local sprite = self.parentUI:GetIconSprite(itemData.resources)
    if sprite~=nil then
        self.imgIcon.sprite = sprite
    end
    if itemData.expire_time~=-1 then -- 道具过期时间：秒数，-1为永久
        self.txtCountDown.transform.parent.gameObject:SetActive(true)
        self:StartCoundown(deltaTime)
    else
        self.txtCountDown.transform.parent.gameObject:SetActive(true)
    end
end

function PakageItemDetailPanel:ShowDeltaTime(delta)
    local hourSeconds = 3600 --一个小时的秒数
    local minuteSeconds = 60
    if delta<0 then delta = 0 end
    if delta >= hourSeconds then
        local hour = math.floor(delta/hourSeconds)
        self.txtCountDown.text = string.format("Expiress in %d hours",hour)
    elseif delta >= minuteSeconds then
        local minute = math.floor(delta/minuteSeconds)
        self.txtCountDown.text = string.format("Expiress in %d mins",minute)
    else
        self.txtCountDown.text = string.format("Expiress in %d sec",delta)
    end
end

function PakageItemDetailPanel:StartCoundown(expire_time)
    local hourSeconds = 3600 --一个小时的秒数
    local minuteSeconds = 60
    self:StopTimer()
    local delta = expire_time or 0
    self:ShowDeltaTime(delta)
    self.timer = core.Timer.New(function()
        self:ShowDeltaTime(delta)
        if delta <= 0 then	--时间到
            self:StopTimer()
            self.parentUI:RemoveItem(self.itemData)
        end
        if delta> 0 then
            delta = delta - 1
        end
    end,1,-1)
    self.timer:Start()
end

function PakageItemDetailPanel:StopTimer()
	if self.timer then
		self.timer:Stop()
		self.timer = nil
    end
end

function PakageItemDetailPanel:Dispose()
    self:StopTimer()
end

function PakageItemDetailPanel:OnClose(eventData)
    self:StopTimer()
    self.gameObject:SetActive(false)
end

return PakageItemDetailPanel
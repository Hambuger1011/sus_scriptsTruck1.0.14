---@class PakageItem
local PakageItem  = core.Class('PakageItem')



function PakageItem:__init(gameObject,parentUI)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.parentUI = parentUI
    self.button = logic.cs.LuaHelper.GetComponent(self.transform, "BGButton",typeof(logic.cs.Button))
    self.imgIcon = logic.cs.LuaHelper.GetComponent(self.transform, "icon",typeof(logic.cs.Image))
    self.objHit = CS.DisplayUtil.FindChild(self.gameObject, "Hit")
    self.txtNum = logic.cs.LuaHelper.GetComponent(self.transform, "txtNum",typeof(logic.cs.Text))
    self.txtCountDown = logic.cs.LuaHelper.GetComponent(self.transform, "countDowd/text",typeof(logic.cs.Text))
    self.txtName = logic.cs.LuaHelper.GetComponent(self.transform, "txtName",typeof(logic.cs.Text))

    self.itemData = nil
    self.deltaTime = -1
    self.button.onClick:AddListener(function()
        self:OnClickItem()
    end)
end


function PakageItem:SetData(itemData)
    self.itemData = itemData
    self.deltaTime = itemData.expire_time
    self:SetUnReadFlag(itemData.is_read~=1) --道具是否已读 1：已读 0未读
    self.imgIcon.sprite = self.parentUI:GetIconSprite(itemData.resources)
    self.txtNum.text = itemData.prop_num or "0"
    self.txtName.text = itemData.name or ""
    if itemData.expire_time~=-1 then -- 道具过期时间：秒数，-1为永久
        self.txtCountDown.transform.parent.gameObject:SetActive(true)
        self:StartCoundown(itemData.expire_time)
    else
        self.txtCountDown.transform.parent.gameObject:SetActive(false)
        self:StopTimer()
    end
end

function PakageItem:OnClickItem()
    if self.itemData.is_read==0 then
        self.parentUI:SetPropReadRequest(self.itemData.id, function()
            self.itemData.is_read=1 --道具是否已读 1：已读 0未读
            self:SetUnReadFlag(false)--设置成已读
        end)
    end
    self.parentUI:ShowItemDetail(self.itemData, self.deltaTime)

end

function PakageItem:SetUnReadFlag(bUnReadFlag)
    self.objHit:SetActive(bUnReadFlag)
end

function PakageItem:ShowDeltaTime(delta)
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

function PakageItem:StartCoundown(expire_time)
    self:StopTimer()
    self.deltaTime = expire_time or 0
    self:ShowDeltaTime(self.deltaTime)
    self.timer = core.Timer.New(function()
        self:ShowDeltaTime(self.deltaTime)
        if self.deltaTime <= 0 then	--时间到
            self.parentUI:RemoveItem(self.itemData)
            self:StopTimer()
        end
        if self.deltaTime> 0 then
            self.deltaTime = self.deltaTime - 1
        end
    end,1,-1)
    self.timer:Start()
end

function PakageItem:StopTimer()
	if self.timer then
		self.timer:Stop()
		self.timer = nil
    end
end

function PakageItem:Dispose()
    self:StopTimer()
    logic.cs.GameObject.Destroy(self.gameObject)
end

return PakageItem
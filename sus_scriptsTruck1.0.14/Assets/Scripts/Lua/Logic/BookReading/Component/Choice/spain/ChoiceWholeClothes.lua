local Class = core.Class

local Base = require("Logic/BookReading/Component/Choice/spain/BaseChoice")
local UICtrl = require("Logic/BookReading/Component/Choice/spain/UIChoiceCtrl")
local ChoiceWholeClothes = Class('ChoiceWholeClothes',Base)



local BuyType =
{
    Free = 0,
    Diamonds = 1,
    Video = 2,
}

local ChoiceType = 
{
	Player = 1,
	Clothes = 2,
	NPC = 3
}

function ChoiceWholeClothes:CollectRes(resTable)
	Base.CollectRes(self,resTable)
	
	local bookData = logic.bookReadingMgr.bookData
	
	--收集npc头像
	if self.cfg.trigger == ChoiceType.NPC then

        local NpcIDs = logic.bookReadingMgr.context.NpcIDs
		local NpcDetailIDs = logic.bookReadingMgr.context.NpcDetailIDs
		
        local NpcID = self.cfg.role_id
        NpcIDs[NpcID] = 1
		NpcDetailIDs[NpcID] = (NpcDetailIDs[NpcID] and NpcDetailIDs[NpcID]) or {}
		
        for i=1,self.cfg.selection_num do
            local NpcDetailID = tonumber(self.cfg['selection_'..i])
            if not NpcDetailID then
                self.cfg['selection_'..i] = '0'
                NpcDetailID = 0
            end
            NpcDetailIDs[NpcID][NpcDetailID] = 1 --收集所有NpcDetailID

            local appearanceID = logic.bookReadingMgr:GetAppearanceID(0, NpcID, self.cfg.icon) + NpcDetailID
            resTable[logic.bookReadingMgr.Res.bookFolderPath..'RoleHead/'..appearanceID..'.png'] = BookResType.BookRes
		end
		
	--收集角色头像
	elseif self.cfg.trigger == ChoiceType.Player then
		local roleIDs = logic.bookReadingMgr.context.PlayerIDs
		for i=1,self.cfg.selection_num do
			local roleID = tonumber(self.cfg['selection_'..i])
			if not roleID then
				self.cfg['selection_'..i] = '0'
				roleID = 0
			end
			roleIDs[roleID] = 1
			
			local appearanceID = logic.bookReadingMgr:GetAppearanceID(roleID, 1, 0)
			resTable[logic.bookReadingMgr.Res.bookFolderPath..'RoleHead/'..appearanceID..'.png'] = BookResType.BookRes
		end
	--收集衣服
	else
		local IDs = logic.bookReadingMgr.context.ClothesIDs
		local roleIDs = {}
		for id,_ in pairs(logic.bookReadingMgr.context.PlayerIDs) do
			roleIDs[id] = 1
		end
		if bookData.PlayerDetailsID and bookData.PlayerDetailsID ~= 0 then
			roleIDs[bookData.PlayerDetailsID] = 1
		end
		for id,_ in pairs(roleIDs) do
			IDs[id] = (IDs[id] and IDs[id] or {})
			for i=1,self.cfg.selection_num do
				local clothesID = tonumber(self.cfg['selection_'..i])
				local appearanceID = logic.bookReadingMgr:GetAppearanceID(id, 1, clothesID)
				resTable[logic.bookReadingMgr.Res.bookFolderPath..'RoleHead/'..appearanceID..'.png'] = BookResType.BookRes
				IDs[id][clothesID] = 1
			end
		end
	end
end


function ChoiceWholeClothes:GetNextDialogID()
	local id = 0
	if self.cfg.trigger == ChoiceType.Player or self.cfg.trigger == ChoiceType.NPC then
		id = Base.GetNextDialogID(self)
	else
		id = self:GetNextDialogIDBySelection(self.selectIdx - 1);
	end
    return id
end

function ChoiceWholeClothes:Play()
    logic.cs.EventDispatcher.Dispatch(logic.cs.EventEnum.ResidentMoneyInfo, 1)
    local bookData = logic.bookReadingMgr.bookData
	self.imgs = {}
	self.IDs = {}
	self.clothesIDs = {}

	if self.cfg.trigger == ChoiceType.NPC then
		for i=1,self.cfg.selection_num do
			local clothesID = tonumber(self.cfg['selection_'..i])
			self.clothesIDs[i] = clothesID
			local appearanceID = logic.bookReadingMgr:GetAppearanceID(0, self.cfg.role_id, self.cfg.icon) + clothesID
			local sprite = logic.bookReadingMgr.Res:GetSprite('RoleHead/'..appearanceID)
			table.insert(self.imgs, sprite)
		end
	elseif self.cfg.trigger == ChoiceType.Player then
		for i=1,self.cfg.selection_num do
			local clothesID = tonumber(self.cfg['selection_'..i])
			self.clothesIDs[i] = clothesID
			local appearanceID = logic.bookReadingMgr:GetAppearanceID(clothesID, 1, 0)
			local sprite = logic.bookReadingMgr.Res:GetSprite('RoleHead/'..appearanceID)
			table.insert(self.imgs, sprite)
		end
	else
		for i=1,self.cfg.selection_num do
			local clothesID = tonumber(self.cfg['selection_'..i])
			self.clothesIDs[i] = clothesID
			local appearanceID = logic.bookReadingMgr:GetAppearanceID(bookData.PlayerDetailsID, 1, clothesID)
			local sprite = logic.bookReadingMgr.Res:GetSprite('RoleHead/'..appearanceID)
			table.insert(self.imgs, sprite)
		end
	end

    ---@type UICtrl
    self.uiCtrl = self:getUI()
    self.uiCtrl:show(self)
end

function ChoiceWholeClothes:GetItems()
    return self.imgs
end

function ChoiceWholeClothes:OnChoiceIndex(idx)
    self.selectIdx = idx
    self:updateSelectCloths()
end




function ChoiceWholeClothes:updateSelectCloths()
    local bookData = logic.bookReadingMgr.bookData
    local clothesID = self.clothesIDs[self.selectIdx]
	self.buyType = BuyType.Free
	self.cost = 0
	if self.cfg.trigger == ChoiceType.Clothes then
		if not logic.cs.UserDataManager:CheckClothHadCost(bookData.BookID, clothesID) then
			local priceCfg = logic.cs.JsonDTManager:GetJDTClothesPrice(bookData.BookID, clothesID)
			if priceCfg and priceCfg.clotheprice > 0 then
				self.buyType = priceCfg.pricetype
				self.cost = priceCfg.clotheprice
	
				if self.buyType == BuyType.Video then
					local seeVideoNum = logic.cs.UserDataManager:GetSeeVideoNumOfClothes(bookData.BookID, clothesID)
					self.cost = self.cost - seeVideoNum
					if(self.cost < 0) then
						self.cost = 0
					end
				end
			end
		end
		if logic.cs.UserDataManager:CheckBookHasBuy(bookData.BookID) then
			self.buyType = BuyType.Free
			self.cost = 0
		end
	end
    self:updateView()
end

function ChoiceWholeClothes:updateView()
    local isNotFree = (self.cost > 0 and self.buyType ~= BuyType.Free)
    if isNotFree then
        self.uiCtrl:SetComfirmType(2)
    else
        self.uiCtrl:SetComfirmType(1)
    end
    if self.buyType == BuyType.Diamonds then
        logic.cs.ABSystem.ui:SetAtlasSprite(self.uiCtrl.imgConfirmCost2, "BookReadingForm", "icon_dimon")
    elseif self.buyType == BuyType.Video then
        logic.cs.ABSystem.ui:SetAtlasSprite(self.uiCtrl.imgConfirmCost2, "BookReadingForm", "icon_video")
    end
    self.uiCtrl.txtConfirmCost2.text = tostring(self.cost)
end


--region 选择

function ChoiceWholeClothes:OnConfirm(idx)
	--print("333")
	self.ChoiceRoleGo=idx
	if self.ChoiceRoleGo.ConfirmMask.gameObject~=nil then			
		self.ChoiceRoleGo.ConfirmMask.gameObject:SetActiveEx(true)
	end
	
	
    logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.dialog_choice_click)
    local bookData = logic.bookReadingMgr.bookData
    local clothesID = self.clothesIDs[self.selectIdx]
    self.needPay = 0
	local doBuyCloth = function(buyType)
		if (not buyType) or logic.cs.UserDataManager:CheckClothHadCost(bookData.BookID, clothesID) then
			self:DoChoicesCloth()
		else
			self.needPay = 1
			--logic.cs.UINetLoadingMgr:Show()
			logic.gameHttp:BookChoicesClothCost(logic.bookReadingMgr.bookData.BookID, clothesID, buyType, function(result)
				self:OnChoicesClothCostNotify(result)
			end)
		end
	end

	if self.buyType == BuyType.Diamonds then
		if self.cost <= 0 or logic.cs.UserDataManager:CheckClothHadCost(bookData.BookID, clothesID) then
			self:DoChoicesCloth()
		else
			self.needPay = 1
			---@type UIValueChoice
			local uiValueChoice = logic.UIMgr:Open(logic.uiid.ValueChoice)
			uiValueChoice:ChoiceRoleGo(self.ChoiceRoleGo)
			uiValueChoice:SetData(1,"Buy-Clothes",self.cost, function(buyType)
				doBuyCloth(buyType)
			end)
		end
	elseif self.buyType == BuyType.Video then
		if self.cost > 0 then
			local seeVideoNum = logic.cs.UserDataManager:GetSeeVideoNumOfClothes(logic.bookReadingMgr.bookData.BookID, clothesID)
			logic.cs.SdkMgr.ads:ShowRewardBasedVideo("Buy-Clothes",function(success)
				if not success then
					return
				end
				seeVideoNum = seeVideoNum + 1
				logic.cs.UserDataManager:SetSeeVideoNumOfClothes(logic.bookReadingMgr.bookData.BookID, clothesID, seeVideoNum)
				self:updateSelectCloths()
				if self.cost <= 0 then
					doBuyCloth(2)
				end

				if idx.ConfirmMask.gameObject~=nil then			
					idx.ConfirmMask.gameObject:SetActiveEx(true)
				end
			end)
		else
			doBuyCloth(2)
		end
	else		
		doBuyCloth()
	end
end


function ChoiceWholeClothes:DoChoicesCloth()

    local bookData = logic.bookReadingMgr.bookData
	local clothesID = self.clothesIDs[self.selectIdx]
	if self.cfg.trigger == ChoiceType.Clothes then	--选衣服
		bookData.PlayerClothes = clothesID
		logic.bookReadingMgr:SavePlayerClothes(clothesID)
	else
		if self.cfg.trigger == ChoiceType.NPC then	--选npc
			local npcId = self.cfg.role_id
			local npcCharacterId = tonumber(self.cfg['selection_'..self.selectIdx])
			bookData.NpcId = npcId
			bookData.NpcDetailId = npcCharacterId
			logic.cs.TalkingDataManager:SelectNpc(bookData.BookID, npcCharacterId)
			logic.bookReadingMgr:SaveNpc(npcId,npcCharacterId)
		else	--选角色
			bookData.PlayerDetailsID = tonumber(self.cfg['selection_'..self.selectIdx])
			logic.cs.TalkingDataManager:SelectPlayer(bookData.BookID, bookData.PlayerDetailsID)
			logic.bookReadingMgr:SavePlayerDetailsID(bookData.PlayerDetailsID)
		end
	end
	logic.bookReadingMgr:SaveOption(self.selectIdx)
	logic.bookReadingMgr:SaveProgress(function(result)
		self:SetProgressHandler(result)
	end)
	logic.cs.UserDataManager:RecordBookOptionSelect(logic.bookReadingMgr.bookData.BookID, self.cfg.dialogID, self.selectIdx);
end

function ChoiceWholeClothes:SetProgressHandler(result)
	local clothesID = self.clothesIDs[self.selectIdx]
	local bookData = logic.bookReadingMgr.bookData
	logic.debug.Log("----SetProgressHandler---->" .. result)
	local json = core.json.Derialize(result)
	--logic.debug.LogError(core.table.tostring(json))
	local code = tonumber(json.code) --坑，返回来的code是字符串
	if code == 200 then
		if self.cfg.trigger == ChoiceType.Clothes then
			logic.bookReadingMgr.bookData.PlayerClothes = clothesID;
			logic.cs.TalkingDataManager:SelectCloths(bookData.BookID, self.cfg.dialogID,clothesID,self.needPay,self.cost)
		else
			if self.cfg.trigger == ChoiceType.Player then
				bookData.PlayerDetailsID = self.selectIdx
				logic.cs.TalkingDataManager:SelectPlayer(bookData.BookID, bookData.PlayerDetailsID)
			else
				local npcId = self.cfg.role_id
				local npcCharacterId = tonumber(self.cfg['selection_'..self.selectIdx])
				bookData.NpcId = npcId
				bookData.NpcDetailId = npcCharacterId
				logic.cs.TalkingDataManager:SelectNpc(bookData.BookID, npcCharacterId)
			end
		end
		self.uiCtrl.gameObject:SetActiveEx(false);
		self:ShowNextDialog()
	else
		if not string.IsNullOrEmpty(json.msg) then
			logic.cs.UITipsMgr:PopupTips(json.msg, false);
		end
	end
end

function ChoiceWholeClothes:OnChoicesClothCostNotify(result)
	--logic.cs.UINetLoadingMgr:Close()
	logic.debug.Log("----ChoicesClothCostCallBack---->" .. result)
	local clothesID = self.clothesIDs[self.selectIdx]
	local json = core.json.Derialize(result)
	local code = tonumber(json.code)
	
	if json and json.data and json.data.videocount then
		logic.cs.UserDataManager.userInfo.data.userinfo.videocount = json.data.videocount
	end
	
	if code == 200 then

		if self.ChoiceRoleGo.ConfirmMask.gameObject~=nil then			
			self.ChoiceRoleGo.ConfirmMask.gameObject:SetActiveEx(true)
		end

		logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.RewardWin)
		logic.cs.UserDataManager:SetChoicesClothResultInfo(result)
		if logic.cs.UserDataManager.choicesClothResultInfo and logic.cs.UserDataManager.choicesClothResultInfo.data then
			local purchase = logic.cs.UserDataManager.UserData.DiamondNum - logic.cs.UserDataManager.choicesClothResultInfo.data.diamond
			if purchase > 0 then
				logic.cs.TalkingDataManager:OnPurchase("ChoicesCloth cost diamond", purchase, 1)
			end
			logic.cs.UserDataManager:ResetMoney(1, logic.cs.UserDataManager.choicesClothResultInfo.data.bkey)
            logic.cs.UserDataManager:ResetMoney(2, logic.cs.UserDataManager.choicesClothResultInfo.data.diamond)
		end
		logic.bookReadingMgr.bookData.PlayerClothes = clothesID
		local appearanceID = logic.bookReadingMgr:GetAppearanceID(logic.bookReadingMgr.bookData.PlayerDetailsID,1,clothesID)
		logic.cs.UserDataManager:AddClothAfterPay(logic.bookReadingMgr.bookData.BookID, clothesID)
		self:DoChoicesCloth()

	elseif code == 202 or code == 203 then
		logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.LoseFail)
		logic.bookReadingMgr.view:ShowChargeTips(self.cost)

		if self.ChoiceRoleGo.ConfirmMask.gameObject~=nil then			
			self.ChoiceRoleGo.ConfirmMask.gameObject:SetActiveEx(false)
		end
	else
		if not string.IsNullOrEmpty(json.msg) then
			logic.cs.UITipsMgr:PopupTips(json.msg, false);
		end

		if self.ChoiceRoleGo.ConfirmMask.gameObject~=nil then			
			self.ChoiceRoleGo.ConfirmMask.gameObject:SetActiveEx(false)
		end
	end
end
--endregion

return ChoiceWholeClothes
local Class = core.Class

local Base = require("Logic/BookReading/Component/Choice/spain/BaseChoice")
local UICtrl = require("Logic/BookReading/Component/Choice/spain/UIChoiceCtrl")
local ChoiceClothes = Class('ChoiceClothes',Base)


local BuyType =
{
    Free = 0,
    Diamonds = 1,
    Video = 2,
}

function ChoiceClothes:CollectRes(resTable)
    Base.CollectRes(self,resTable)

	local bookData = logic.bookReadingMgr.bookData
	----收集所有角色衣服
	local closthesIDs = logic.bookReadingMgr.context.ClothesIDs
	local roleIDs = {}
	if self.isNpc then --收集所有的npc衣服
		for id,_ in pairs(logic.bookReadingMgr.context.NpcIDs) do
			roleIDs[id] = 1
		end
		if bookData.NpcId and bookData.NpcId ~= 0 then
			roleIDs[bookData.NpcId] = 1
		end
	else --收集所有的player衣服
		for id,_ in pairs(logic.bookReadingMgr.context.PlayerIDs) do
			roleIDs[id] = 1
		end
		if bookData.PlayerDetailsID and bookData.PlayerDetailsID ~= 0 then
			roleIDs[bookData.PlayerDetailsID] = 1
		end
	end
	
	for id,_ in pairs(roleIDs) do
		closthesIDs[id] = (closthesIDs[id] and closthesIDs[id] or {})
		for i=1,self.cfg.selection_num do
			local clothesID = tonumber(self.cfg['selection_'..i])
			if not clothesID then
				self.cfg['selection_'..i] = '0'
				clothesID = 0
			end
			local appearanceID = logic.bookReadingMgr:GetAppearanceID(id, 1, clothesID)
			resTable[logic.bookReadingMgr.Res.bookFolderPath..'RoleHead/'..appearanceID..'.png'] = BookResType.BookRes
			closthesIDs[id][clothesID] = 1
		end
	end
end


function ChoiceClothes:GetNextDialogID()
    local id = self:GetNextDialogIDBySelection(self.selectIdx - 1);
    return id
end


function ChoiceClothes:Play()
    logic.cs.EventDispatcher.Dispatch(logic.cs.EventEnum.ResidentMoneyInfo, 1)
    self.imgs = {}
    self.clothesIDs = {}
    for i=1,self.cfg.selection_num do
        self.clothesIDs[i] = tonumber(self.cfg['selection_'..i])
        local appearanceID = logic.bookReadingMgr:GetAppearanceID(logic.bookReadingMgr.bookData.PlayerDetailsID, 1, self.clothesIDs[i])
        local sprite = logic.bookReadingMgr.Res:GetSprite('RoleHead/'..appearanceID)
        table.insert(self.imgs, sprite)
    end

    ---@type UICtrl
    self.uiCtrl = self:getUI()
    self.uiCtrl:show(self)
end

function ChoiceClothes:GetItems()
    return self.imgs
end

function ChoiceClothes:OnChoiceIndex(idx)
    self.selectIdx = idx
    self:updateSelectCloths()
end


function ChoiceClothes:OnConfirm(idx)
	--print("2222")

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
			uiValueChoice:SetData(1,"Buy-Clothes",self.cost, function(result)
				doBuyCloth(result)
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
				if self.cost == 0 then
					doBuyCloth(2)
				end
			end)
		else
			doBuyCloth(2)
		end
	else
		doBuyCloth()
	end
end



function ChoiceClothes:updateSelectCloths()
    local bookData = logic.bookReadingMgr.bookData
    local clothesID = self.clothesIDs[self.selectIdx]
    self.buyType = BuyType.Free
	self.cost = 0
    if logic.cs.UserDataManager:CheckClothHadCost(bookData.BookID, clothesID) then
		self.cost = 0
		self.buyType = BuyType.Free
		logic.debug.Log('已经购买过:BookID='..bookData.BookID..',clothesID='..clothesID)
    else
        local priceCfg = logic.cs.JsonDTManager:GetJDTClothesPrice(bookData.BookID, clothesID)
		if priceCfg then
			self.buyType = priceCfg.pricetype
			self.cost = priceCfg.clotheprice

			if self.buyType == BuyType.Video then
				local seeVideoNum = logic.cs.UserDataManager:GetSeeVideoNumOfClothes(bookData.BookID, clothesID)
				self.cost = self.cost - seeVideoNum
				if(self.cost < 0) then
					self.cost = 0
				end
			end
		else
			self.buyType = BuyType.Free
			self.cost = 0
		end
		if logic.cs.UserDataManager:CheckBookHasBuy(bookData.BookID) then
			self.buyType = BuyType.Free
			self.cost = 0
		end
		logic.debug.Log('购买:BookID='..bookData.BookID..',clothesID='..clothesID..',cost='..self.cost)
    end
    self:updateView()
end

function ChoiceClothes:updateView()
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
function ChoiceClothes:DoChoicesCloth()

	local clothesID = self.clothesIDs[self.selectIdx]
	local bookData = logic.bookReadingMgr.bookData
	bookData.PlayerClothes = clothesID
	logic.bookReadingMgr:SavePlayerClothes(clothesID)
	logic.bookReadingMgr:SaveOption(self.selectIdx)
	logic.bookReadingMgr:SaveProgress(function(result)
		self:SetProgressHandler(result)
	end)
end

function ChoiceClothes:SetProgressHandler(result)
    local clothesID = self.clothesIDs[self.selectIdx]
	logic.debug.Log("----SetProgressHandler---->" .. result)
	local json = core.json.Derialize(result)
	--logic.debug.LogError(core.table.tostring(json))
	local code = tonumber(json.code) --坑，返回来的code是字符串
	if code == 200 then
		
		logic.bookReadingMgr.bookData.PlayerClothes = clothesID;
		self.uiCtrl.gameObject:SetActiveEx(false);
		logic.cs.UserDataManager:RecordBookOptionSelect(logic.bookReadingMgr.bookData.BookID, self.cfg.dialogID, self.selectIdx);
		--_form.BgAddBlurEffect(false);
		logic.cs.EventDispatcher.Dispatch(logic.cs.EventEnum.ResidentMoneyInfo, 0);
		--logic.cs.EventDispatcher.Dispatch(logic.cs.EventEnum.DialogDisplaySystem_PlayerMakeChoice, self.curClothesIndex);
		--_form.setBGOnClickListenerActive(true);
		--_form.ResetOperationTime();
		logic.cs.TalkingDataManager:SelectCloths(logic.bookReadingMgr.bookData.BookID, self.cfg.dialogID, clothesID, self.needPay, self.cost);
		self:ShowNextDialog()
	else
		if not string.IsNullOrEmpty(json.msg) then
			logic.cs.UITipsMgr:PopupTips(json.msg, false);
		end
	end
end

function ChoiceClothes:OnChoicesClothCostNotify(result)
    local clothesID = self.clothesIDs[self.selectIdx]
	--logic.cs.UINetLoadingMgr:Close()
	logic.debug.Log("----ChoicesClothCostCallBack---->" .. result)
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

return ChoiceClothes
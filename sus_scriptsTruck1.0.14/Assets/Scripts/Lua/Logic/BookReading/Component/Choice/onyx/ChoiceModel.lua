--[[
    选择形象
]]
local Class = core.Class
local Base = logic.BookReading.BaseComponent

local ChoiceModel = Class("ChoiceModel", Base)


local BuyType =
{
    Free = 0,
    Diamonds = 1,
    Video = 2,
}

local ChoiceType = 
{
	Skin = 1,
	Hair = 2,
	Clothes = 3,
}

local uiView = nil

local NeedShowBtn = {}

local skinIsSelected = false; --皮肤确定选择前无法选择其他按钮

local isFree = false --当前选择是否免费或已付费

local delayMove = false

local canChoiceSkin = true

local skinTable = {}

local InstallUI = function()
    if not uiView then
		local prefab = logic.bookReadingMgr.Res:GetPrefab("Assets/Bundle/UI/BookReading/ChoiceModel.prefab")
		uiView = {}
		uiView.gameObject = logic.cs.GameObject.Instantiate(prefab,logic.bookReadingMgr.view.transComponent,false)
		uiView.transform = uiView.gameObject.transform
		uiView.Reset = function()
			--print("选衣服的显示出来")
			uiView.imgLeftArrow.raycastTarget=true		
		    uiView.imgRightArrow.raycastTarget=true	

			uiView.gameObject:SetActiveEx(true)
			uiView.fxSelectClothes:SetActiveEx(false)
			uiView.imgScreenFlashImage.gameObject:SetActiveEx(false)
			uiView.imgScreenFlashImage.color = core.Color.New(1,1,1,0)
		end

		uiView.Close = function(self)
			uiView.gameObject:SetActiveEx(false)
		end

        local get = logic.cs.LuaHelper.GetComponent
        uiView.uiBinding = uiView.gameObject:GetComponent(typeof(logic.cs.UIBinding))
		uiView.fxSelectClothes = uiView.uiBinding:Get('fxSelectClothes')
		uiView.ClothesTF = uiView.uiBinding:Get('spineRoot').transform
        uiView.roleSpine = logic.SpineCharacter.New(uiView.uiBinding:Get('spine'))
        uiView.imgLeftArrow = uiView.uiBinding:Get('btnPre',  typeof(logic.cs.Image))
        uiView.imgRightArrow = uiView.uiBinding:Get('btnNext', typeof(logic.cs.Image))
		uiView.ClothNumber = uiView.uiBinding:Get('lbTotal', typeof(logic.cs.Text))
		uiView.detailsTF = uiView.uiBinding:Get('detailsTF').transform
		uiView.Detials = uiView.uiBinding:Get('Detials').transform
		uiView.Toggles = uiView.uiBinding:Get('Toggles').transform
		uiView.rotateMask1 = uiView.uiBinding:Get('rotateMask1').transform
		uiView.rotateMask2 = uiView.uiBinding:Get('rotateMask2').transform
		uiView.rotateMask3 = uiView.uiBinding:Get('rotateMask3').transform
		uiView.btnShowHideDetails = uiView.uiBinding:Get('btnShowHide', typeof(logic.cs.UITweenButton))
		uiView.imgScreenFlashImage = uiView.uiBinding:Get('ScreenFlashImage',typeof(logic.cs.Image))
		uiView.CharageEffectGo = uiView.uiBinding:Get('CharageEffectGo')
		uiView.freePanel = uiView.uiBinding:Get('freePanel')
		uiView.payPanel = uiView.uiBinding:Get('payPanel')

		uiView.toggleSkin = uiView.uiBinding:Get('toggleSkin', typeof(logic.cs.UIToggle))
		uiView.toggleClothing = uiView.uiBinding:Get('toggleClothing', typeof(logic.cs.UIToggle))
		uiView.toggleHair = uiView.uiBinding:Get('toggleHair', typeof(logic.cs.UIToggle))

		uiView.lbCost = uiView.uiBinding:Get('lbCost', typeof(logic.cs.Text))
		uiView.DetailText = uiView.uiBinding:Get('DetailText', typeof(logic.cs.Text))

		
		uiView.btnPayConfirm = uiView.uiBinding:Get('btnPayConfirm')
		uiView.btnFreeConfirm = uiView.uiBinding:Get('btnFreeConfirm')
		
		uiView.Flash = uiView.uiBinding:Get('Flash',typeof(logic.cs.Image))
		
		-- uiView.imgLeftArrow:SetNativeSize()
		-- uiView.imgRightArrow:SetNativeSize()
		-- uiView.imgDown:SetNativeSize()
	end
end

local UninstallUI = function()
    if not uiView then
        return
	end
    uiView = nil
end

local RotateInit = function()
	for k,v in pairs(NeedShowBtn) do
		v.localRotation = core.Quaternion.Euler(0,0,-90)
	end
end

local function SetToggleState (choice)
	if choice == ChoiceType.Skin and uiView.toggleSkin.isOn == false then
		uiView.toggleSkin.isOn = true
		uiView.toggleClothing.isOn = false
		uiView.toggleHair.isOn = false
	elseif choice == ChoiceType.Clothes and uiView.toggleClothing.isOn == false then
		uiView.toggleClothing.isOn = true
		uiView.toggleSkin.isOn = false
		uiView.toggleHair.isOn = false
	elseif choice == ChoiceType.Hair and uiView.toggleHair.isOn == false then
		uiView.toggleHair.isOn = true
		uiView.toggleClothing.isOn = false
		uiView.toggleSkin.isOn = false
	end
end

local function ShowNeedShowBtn()
	local size = table.length(NeedShowBtn);
	if size == 3 then
		for i = 1,size do
			local rotZ = 28-(14*i)
			NeedShowBtn[i]:DORotate( core.Vector3(0,0,rotZ),0.2*i)
			NeedShowBtn[i]:Find("btn").transform.localRotation = core.Quaternion.Euler(0,0,-rotZ)
		end
	elseif size == 2 then
		for i = 1,size do
			local rotZ = 21-(14*i)
			NeedShowBtn[i]:DORotate( core.Vector3(0,0,rotZ),0.2*i)
			NeedShowBtn[i]:Find("btn").transform.localRotation = core.Quaternion.Euler(0,0,-rotZ)
		end
	elseif size == 1 then
		NeedShowBtn[1]:DORotate( core.Vector3(0,0,0),0.2)
		NeedShowBtn[1]:Find("btn").transform.localRotation = core.Quaternion.Euler(0,0,0)
	end
end

---region ModelView
local ModelView = Class('ChoiceModel.ModelView')
---endregion

function ChoiceModel:__init(index,cfg)
	Base.__init(self, index, cfg)
	self.cfg.trigger = 1
end

---@overrider
--收集所用到的资源
function ChoiceModel:CollectRes(resTable)
    Base.CollectRes(self,resTable)
	resTable["Assets/Bundle/UI/BookReading/ChoiceModel.prefab"] = BookResType.UIRes

	local bookFolderPath = logic.bookReadingMgr.Res.bookFolderPath
	local roleModel = logic.bookReadingMgr.Res:GetRoleMode(self.cfg.modelid)
	local clothesIDs = roleModel.outfit_type3
	for k , v in pairs(clothesIDs) do
		--local clothGroupId = math.ceil( v / 4 )
		--if clothGroupId == 0 then
		--	clothGroupId = 1
		--end
		local clothGroupId = 1
		local appearanceID = logic.bookReadingMgr:GetAppearanceID(1,0, clothGroupId)
		resTable[bookFolderPath..'Role/'..appearanceID..'_SkeletonData.asset'] = BookResType.BookRes
	end
end

---@overrider
function ChoiceModel:Clean()
	Base.Clean(self)
    UninstallUI()
end

---@overrider
function ChoiceModel:OnPlayEnd()
	self:removeListener()
	logic.bookReadingMgr.view:SetBottomActive(true)
    if uiView then
		uiView:Close()
	end
end

function ChoiceModel:ChocieAnim()
	delayMove = true
	uiView.Flash:DOColor(core.Color.New(1,1,1,0.5), 0.1)
		  :OnComplete(function()
		uiView.Flash:DOColor(core.Color.New(1,1,1,0), 0.1)
			  :OnComplete(function()
			self:MovePeople();
			delayMove = false
		end):Play()
	end):Play()

	if not isFree then
		coroutine.start(function()
			uiView.CharageEffectGo:SetActiveEx(true)
			coroutine.wait(3)
			uiView.CharageEffectGo:SetActiveEx(false)
		end)
	end
end

---@overrider
function ChoiceModel:Play()
	InstallUI()
	uiView.Reset()
	self.cost = 0

	logic.bookReadingMgr.view:SetBottomActive(false)

	local modelID = self.cfg.modelid
	self.roleModel = logic.bookReadingMgr.Res:GetRoleMode(modelID)
	skinTable={}
	if self.roleModel and self.roleModel.character_type1 then
		skinTable = self.roleModel.character_type1
	end
	canChoiceSkin = true
	skinIsSelected = false
	if #skinTable > 0 then
		
	else
		local bookData = logic.bookReadingMgr.bookData
		local characterId = bookData.character_id
		if characterId and tonumber(characterId) > 0 then
			skinTable = {characterId}
		else
			skinTable = {1}
		end
		canChoiceSkin = false
	end

	local resetBtn = function()
		NeedShowBtn = {}
		if math.max(1, table.length(self.roleModel.hair_type2)) > 0 then	--头发
			table.insert(NeedShowBtn,uiView.rotateMask1)
		else
			uiView.rotateMask1.gameObject:SetActiveEx(false)
		end
		if table.length(self.roleModel.outfit_type3) > 0 then	--衣服
			table.insert(NeedShowBtn,uiView.rotateMask2)
		else
			uiView.rotateMask2.gameObject:SetActiveEx(false)
		end
		if table.length(self.roleModel.character_type1) > 0 then	--皮肤
			table.insert(NeedShowBtn,uiView.rotateMask3)
		else
			uiView.rotateMask3.gameObject:SetActiveEx(false)
		end
		ShowNeedShowBtn()
	end
		--region 添加事件

		local onScreenFlashClick = function(data)
		self:OnScreenFlashClick()
		end
		local onLeftArrowClick = function(data)
		self:ChangeClothesMove(true)
		end
		local onRightArrowClick = function(data)
		self:ChangeClothesMove(false)
		end
		local onConfirmClick = function(data)
		self:OnComfirmClick()
		end

		logic.cs.UIEventListener.AddOnClickListener(uiView.imgScreenFlashImage.gameObject,onScreenFlashClick)
		uiView.btnShowHideDetails.onClick:AddListener(function()
		self:HideDetials()
		end)
		logic.cs.UIEventListener.AddOnClickListener(uiView.imgLeftArrow.gameObject,onLeftArrowClick)
		logic.cs.UIEventListener.AddOnClickListener(uiView.imgRightArrow.gameObject,onRightArrowClick)
		logic.cs.UIEventListener.AddOnClickListener(uiView.btnPayConfirm,onConfirmClick)
		logic.cs.UIEventListener.AddOnClickListener(uiView.btnFreeConfirm,onConfirmClick)

		uiView.toggleSkin.onValueChanged:AddListener(function()
			self:OnSelectModeType(ChoiceType.Skin)
		end)
		uiView.toggleClothing.onValueChanged:AddListener(function()
			if skinIsSelected then
				self:OnSelectModeType(ChoiceType.Clothes)
			else
				--图标震动无法选择
				SetToggleState(ChoiceType.Skin)
				uiView.btnFreeConfirm.transform:DOShakePosition(0.5, core.Vector3.New(10, 0, 0),200)
				uiView.btnPayConfirm.transform:DOShakePosition(0.5, core.Vector3.New(10, 0, 0),200)
			end
		end)
		uiView.toggleHair.onValueChanged:AddListener(function()
			if skinIsSelected then
				self:OnSelectModeType(ChoiceType.Hair)
			else
				--图标震动无法选择
				SetToggleState(ChoiceType.Skin)
				uiView.btnFreeConfirm.transform:DOShakePosition(0.5, core.Vector3.New(10, 0, 0),200)
				uiView.btnPayConfirm.transform:DOShakePosition(0.5, core.Vector3.New(10, 0, 0),200)
			end
		end)

		self.removeListener = function()
		uiView.btnShowHideDetails.onClick:RemoveAllListeners()
		uiView.toggleSkin.onValueChanged:RemoveAllListeners()
		uiView.toggleClothing.onValueChanged:RemoveAllListeners()
		uiView.toggleHair.onValueChanged:RemoveAllListeners()

		logic.cs.UIEventListener.RemoveOnClickListener(uiView.imgScreenFlashImage.gameObject,onScreenFlashClick)
		logic.cs.UIEventListener.RemoveOnClickListener(uiView.imgLeftArrow.gameObject,onLeftArrowClick)
		logic.cs.UIEventListener.RemoveOnClickListener(uiView.imgRightArrow.gameObject,onRightArrowClick)
		logic.cs.UIEventListener.RemoveOnClickListener(uiView.btnPayConfirm,onConfirmClick)
		logic.cs.UIEventListener.RemoveOnClickListener(uiView.btnFreeConfirm,onConfirmClick)

		end
		--endregion
	
	if canChoiceSkin then
		self:OnSelectModeType(ChoiceType.Skin,true)
		SetToggleState(ChoiceType.Skin)
	else
		skinIsSelected = true
		self:OnSelectModeType(ChoiceType.Hair,true)
		SetToggleState(ChoiceType.Hair)
	end
		resetBtn();
end

---@overrider
function ChoiceModel:OnScreenFlashClick()
	logic.bookReadingMgr.view:ResetOperationTips()

	self:ShowDetials()
end

function ChoiceModel:ShowDetials()
	if self.mInHideState then
		self.mInHideState = false
		self:MoveLeftOrRightBtn(false)
		self:MoveDetailFrame(true, false)

		
		uiView.imgScreenFlashImage.gameObject:SetActiveEx(false)
		uiView.imgScreenFlashImage.raycastTarget = false

		--[[
				local downTF = ui.imgDown.transform
		downTF.localRotation = core.Quaternion.Euler(0,0,0)
		downTF:DOAnchorPos(self.mDownPosUp,0.4)
			:SetEase(core.tween.Ease.OutQuint)
			:Play()
		--]]
	
	end
end

function ChoiceModel:HideDetials()
	self:MoveLeftOrRightBtn(true)
	local Detials = uiView.Detials
	local Toggles = uiView.Toggles
	--local downTF = uiView.imgDown.transform

	--[[
		downTF.anchoredPosition = self.mDownPosUp
	downTF:DOAnchorPos(self.mDownPowDo,0.4)
		:SetEase(core.tween.Ease.InQuart)
		:OnComplete(function()
			downTF.localRotation = core.Quaternion.Euler(0,0,180)
		end):Play()
	--]]
	local viewSize = logic.bookReadingMgr.view.viewSize
	local moveTargetY = viewSize.y
	Toggles:DOLocalMoveY(-moveTargetY, 0.4):OnComplete(function()
		RotateInit()
	end)   :Play()
	Detials :DOLocalMoveY(-moveTargetY,0.4)
			:SetEase(core.tween.Ease.InQuart)
			:OnComplete(function()
			self.mInHideState = true

			uiView.imgScreenFlashImage.color = core.Color.New(1,1,1,0)
			uiView.imgScreenFlashImage.raycastTarget = true
			uiView.imgScreenFlashImage.gameObject:SetActiveEx(true)
		end):Play()
end


function ChoiceModel:MoveDetailFrame(isRight,isOut)
	if self.modelType == ChoiceType.Skin then --修改皮肤后重新锁定其他两个选项按钮
		skinIsSelected = false
	end
	local viewSize = logic.bookReadingMgr.view.viewSize
	local Detials = uiView.Detials
	local Toggles = uiView.Toggles
	local moveTargetX = (isRight and -viewSize.x) or viewSize.x
	local moveTargetY = viewSize.y
	if isOut then
		Detials:DOLocalMoveX(moveTargetX, 0.3):OnComplete(function()
			Detials.gameObject:SetActiveEx(false)
		end)   :Play()
		Toggles:DOLocalMoveY(-moveTargetY, 0.3):OnComplete(function()
			Toggles.gameObject:SetActiveEx(false)
			RotateInit()
		end)   :Play()
	else
		Toggles.gameObject:SetActiveEx(true)
		Toggles:DOLocalMoveY(-312.5, 0.4):OnComplete(function()
			ShowNeedShowBtn()
		end)   :Play()
		local pos = Detials.anchoredPosition
		pos.x = 0
		pos.y = -moveTargetY
		Detials.anchoredPosition = pos
		--Detials.localScale = core.Vector3(1,0,1)
		--uiView.txtDesc.color = core.Color.New(0.2,0.2,0.2,0)
		Detials.gameObject:SetActiveEx(true)
		Detials:DOLocalMoveY(0, 0.4)
			   :SetEase(core.tween.Ease.OutBack)
			   :OnComplete(function()
				--downTF.anchoredPosition = self.mDownPosUp
				--downTF.gameObject:SetActiveEx(true)
				--downTF:DOScaleY(1,0.4):Play()
				--uiView.txtDesc:DOColor(core.Color.New(0.2,0.2,0.2,1),0.4):Play()
			end)
		
		--local isNotFree = (self.cost > 0 and self.buyType ~= BuyType.Free)
		--uiView.CharageEffectGo:SetActiveEx(isNotFree)
	end
end

function ChoiceModel:MoveLeftOrRightBtn(isOut)
	uiView.CharageEffectGo:SetActiveEx(false)
	local viewSize = logic.bookReadingMgr.view.viewSize
	if isOut then
		uiView.imgLeftArrow.transform:DOLocalMoveX(-viewSize.x,0.3):SetEase(core.tween.Ease.InQuint):Play()
		uiView.imgRightArrow.transform:DOLocalMoveX(viewSize.x,0.3):SetEase(core.tween.Ease.InQuint):Play()
	else
		uiView.imgLeftArrow.transform.localScale = core.Vector3.New(0,0,0)
		uiView.imgRightArrow.transform.localScale = core.Vector3.New(0,0,0)

		uiView.imgLeftArrow.transform.anchoredPosition = core.Vector2.New(-300, -15)
		uiView.imgRightArrow.transform.anchoredPosition = core.Vector2.New(300, -15)

		uiView.imgLeftArrow.transform:DOLocalMoveX(-320,0.3):SetDelay(0.15):SetEase(core.tween.Ease.OutBack):Play()
		uiView.imgRightArrow.transform:DOLocalMoveX(320,0.3):SetDelay(0.15):SetEase(core.tween.Ease.OutBack):Play()

		uiView.imgLeftArrow.transform:DOScale(1,0.3):Play()
		uiView.imgRightArrow.transform:DOScale(1,0.3):Play()
	end
end

function ChoiceModel:SetSpineData(selectIdx)

	-- local clothesID = self.clothesIDs[selectIdx]
	-- local bookData = logic.bookReadingMgr.bookData
	-- local skinName = 'skin1'
	-- if self.cfg.trigger == ChoiceType.Player then
	-- 	skinName = 'skin'..selectIdx
	-- elseif self.cfg.trigger ==  ChoiceType.Clothes then
	-- 	skinName = 'skin'..bookData.PlayerDetailsID
	-- elseif self.cfg.trigger == ChoiceType.NPC then
	-- 	local tempClothesID = clothesID;
	-- 	if tempClothesID > 3 then
	-- 		tempClothesID = tempClothesID - 3
	-- 	end
	-- 	local skinIndex = tempClothesID 
	-- 	skinName = 'skin'.. skinIndex
	-- 	clothesID = 1
	-- end
	

	-- local clothesName = "clothes" .. clothesID
	-- local expressionName = "expression1"
	-- local hair1Name	= "hair1"
	-- local hair2Name	= "hair1_1"

	
	
	-- uiView.roleSpine:SetData(skinName,clothesName,expressionName,hair1Name,hair2Name)
end



function ChoiceModel:ChangeClothesMove(isLeftArrow)
	if logic.cs.BookReadingWrapper.IsTextTween then
		return
	end
	logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.dialog_choice_click)
	uiView.CharageEffectGo:SetActiveEx(false)
	local lastIndex = self.selectIdx[self.modelType]
	if isLeftArrow then
		self.selectIdx[self.modelType] = self.selectIdx[self.modelType] - 1
	else
		self.selectIdx[self.modelType] = self.selectIdx[self.modelType] + 1
	end
	if self.selectIdx[self.modelType] > self.numOfChoice[self.modelType] then
		self.selectIdx[self.modelType] = 1
	elseif self.selectIdx[self.modelType] < 1 then
		self.selectIdx[self.modelType] = self.numOfChoice[self.modelType]
	end
	
	--print("当前衣服的编号是："..self.selectIdx[self.modelType].."--衣服总共的编号是："..self.numOfChoice[self.modelType])


	self:MoveLeftOrRightBtn(true)
	self:MoveDetailFrame(not isLeftArrow,true)

	local viewSize = logic.bookReadingMgr.view.viewSize
	local moveTargetX = (isLeftArrow and viewSize.x) or -viewSize.x
	uiView.ClothesTF:DOLocalMoveX(moveTargetX, 0.3)
		:SetEase(core.tween.Ease.InQuad)
		:OnComplete(function()
			local startX = (isLeftArrow and -viewSize.x) or viewSize.x
			local pos = uiView.ClothesTF.anchoredPosition
			pos.x = startX
			uiView.ClothesTF.anchoredPosition = pos
			uiView.ClothesTF:DOLocalMoveX(0, 0.3)
				:SetEase(core.tween.Ease.OutQuint)
				:OnComplete(function()
					self:MoveDetailFrame(not isLeftArrow,false)
				end):Play()
			self:MoveLeftOrRightBtn(false)
			self:UpdateDetialsView()
		end)
	logic.bookReadingMgr.view:ResetOperationTips()
end

function ChoiceModel:OnSelectModeType(nType,isInit)
	logic.bookReadingMgr.view:ResetOperationTips()
	if self.modelType == nType then
		return
	end
	self.modelType = nType
	local modelID = self.cfg.modelid
	---@type t_RoleModel
	self.roleModel = logic.bookReadingMgr.Res:GetRoleMode(modelID)
	if self.roleModel == nil then
		return
	end

	self.selectIdx = self.selectIdx or {1,1,1}
	self.numOfChoice = self.numOfChoice or {0,0,0}
	if nType == ChoiceType.Skin then	--皮肤
		self.numOfChoice[nType] = table.length(skinTable)
	elseif nType == ChoiceType.Clothes then	--衣服
		self.numOfChoice[nType] = table.length(self.roleModel.outfit_type3)
	elseif nType == ChoiceType.Hair then	--头发
		self.numOfChoice[nType] = math.max(1, table.length(self.roleModel.hair_type2))
	else
		logic.debug.LogError("not found modelType:"..tostring(nType))
	end
	self:UpdateDetialsView(isInit)
end

function ChoiceModel:MovePeople()
	if self.modelType == ChoiceType.Skin then
		uiView.ClothesTF:DOAnchorPosY(770, 0.5)
		uiView.ClothesTF:DOScale(core.Vector3.New(1.3,1.3,1),0.5)
	elseif self.modelType == ChoiceType.Clothes then
		uiView.ClothesTF:DOAnchorPosY(923, 0.5)
		uiView.ClothesTF:DOScale(core.Vector3.New(0.9,0.9,1),0.5)
	else
		uiView.ClothesTF:DOAnchorPosY(620, 0.5)
		uiView.ClothesTF:DOScale(core.Vector3.New(1.4,1.4,1),0.5)
	end
end

function ChoiceModel:UpdateDetialsView(isInit)
	local skinID = skinTable[self.selectIdx[ChoiceType.Skin]]
	local clothesID = self.roleModel.outfit_type3[self.selectIdx[ChoiceType.Clothes]]
	-- if isInit and self.modelType ~= ChoiceType.Clothes then
	-- 	local bookData = logic.bookReadingMgr.bookData
	-- 	local outfitId = bookData.outfit_id
	-- 	clothesID = outfitId
	-- 	if clothesID == 0 then
	-- 		clothesID = 1
	-- 	end
	-- end
	local hairID = self.roleModel.hair_type2[self.selectIdx[ChoiceType.Hair]]

	local skinCfg = logic.bookReadingMgr.Res:GetRoleModelData(ChoiceType.Skin,skinID)
	local clothesCfg = logic.bookReadingMgr.Res:GetRoleModelData(ChoiceType.Clothes,clothesID)
	local hairCfg = logic.bookReadingMgr.Res:GetRoleModelData(ChoiceType.Hair,hairID)
	self.cost = skinCfg.price + clothesCfg.price + hairCfg.price

	local choiceCost
	local descriptionText
	if self.modelType == ChoiceType.Skin then
		choiceCost = skinCfg.price
		descriptionText = skinCfg.description
		if choiceCost <= 0 or logic.cs.UserDataManager:GetCharacterHadBuy(skinID) then
			isFree = true
		else
			isFree = false
		end
	elseif self.modelType == ChoiceType.Clothes then

		hairID = 1
		local bookData = logic.bookReadingMgr.bookData
		if bookData.hair_id and bookData.hair_id ~= 0 then
			hairID = bookData.hair_id
		end
		
		choiceCost = clothesCfg.price
		descriptionText = clothesCfg.description
		if choiceCost <= 0 or logic.cs.UserDataManager:GetOutfitHadBuy(clothesID) then
			isFree = true
		else
			isFree = false
		end
	else
		choiceCost = hairCfg.price
		descriptionText = hairCfg.description
		if choiceCost <= 0 or logic.cs.UserDataManager:GetHairHadBuy(hairID) then
			isFree = true
		else
			isFree = false
		end
	end
	uiView.DetailText.text = tostring(descriptionText)
	uiView.lbCost.text = tostring(choiceCost)
	uiView.freePanel:SetActiveEx(isFree)
	uiView.payPanel:SetActiveEx(not isFree)
	uiView.fxSelectClothes:SetActiveEx(not isFree)
	

	uiView.ClothNumber.text= string.format("CHOICE[%d/%d]",self.selectIdx[self.modelType],self.numOfChoice[self.modelType]);
    --self:ShowPiontType(self.selectIdx[self.modelType])


	if not delayMove then
		self:MovePeople();
	end
	
	--local id = 
	--local roleModelData = logic.bookReadingMgr.Res:GetRoleModelData(roleModel.book_id,nType,)
	
	local skinName = 'skin'..skinID
	local clothesName = "clothes" .. clothesID
	local expressionName = "expression1"
	local hair1Name	= "hair1"
	local hair2Name	= "hair1_1"
	if hairID > 1 then
		hair1Name	= "hair"..hairID
		hair2Name	= "hair"..hairID.."_1"
	end
	
	--local clothGroupId = math.ceil( clothesID/4 )
	--if clothGroupId == 0 then
	--	clothGroupId = 1
	--end
	local clothGroupId = 1
	local appearanceID = logic.bookReadingMgr:GetAppearanceID(1,0, clothGroupId)
	local spine = logic.bookReadingMgr.Res:GetSkeDataAsset('Role/'..appearanceID)
	uiView.roleSpine:SetSpine(spine)
	
	uiView.roleSpine:SetData(skinName,clothesName,expressionName,hair1Name,hair2Name)

end


function ChoiceModel:OnComfirmClick()
	local skinID = skinTable[self.selectIdx[ChoiceType.Skin]]
	local clothesID = self.roleModel.outfit_type3[self.selectIdx[ChoiceType.Clothes]]
	local hairID = self.roleModel.hair_type2[self.selectIdx[ChoiceType.Hair]]

	local skinCfg = logic.bookReadingMgr.Res:GetRoleModelData(ChoiceType.Skin,skinID)
	local clothesCfg = logic.bookReadingMgr.Res:GetRoleModelData(ChoiceType.Clothes,clothesID)
	local hairCfg = logic.bookReadingMgr.Res:GetRoleModelData(ChoiceType.Hair,hairID)
	

	if self.modelType == ChoiceType.Skin then
		if logic.cs.UserDataManager.UserData.DiamondNum < skinCfg.price and not isFree then
			logic.bookReadingMgr.view:ShowChargeTips(skinCfg.price)
			return
		end
		logic.bookReadingMgr:SaveModel(skinID,0,0)
		logic.bookReadingMgr:SaveProgress(function(result)
			local json = core.json.Derialize(result)
			local code = tonumber(json.code)
			if code == 200 then
				logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.McSelectCharacter,"","",tostring(logic.bookReadingMgr.bookData.BookID),tostring(self.cfg.dialogID),tostring(self.selectIdx[self.modelType]))
				self:OnChoicesModelNotify(result,skinID,nil,nil)
				self:ChocieAnim()
				skinIsSelected = true
				self:OnSelectModeType(ChoiceType.Hair)
				SetToggleState(ChoiceType.Hair)
				logic.cs.UserDataManager:SaveCharacterHadBuy(tostring(skinID))
			else
				logic.cs.UIAlertMgr:Show("TIPS",json.msg)
				return
			end
		end)
	elseif self.modelType == ChoiceType.Hair then
		if logic.cs.UserDataManager.UserData.DiamondNum < hairCfg.price and not isFree then
			logic.bookReadingMgr.view:ShowChargeTips(hairCfg.price)
			return
		end
		logic.bookReadingMgr:SaveModel(0,0,hairID)
		logic.bookReadingMgr:SaveProgress(function(result)
			local json = core.json.Derialize(result)
			local code = tonumber(json.code)
			if code == 200 then
				logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.McSelectHair,"","",tostring(logic.bookReadingMgr.bookData.BookID),tostring(self.cfg.dialogID),tostring(self.selectIdx[self.modelType]))
				self:OnChoicesModelNotify(result,nil,nil,hairID)
				self:ChocieAnim()
				self:OnSelectModeType(ChoiceType.Clothes)
				SetToggleState(ChoiceType.Clothes)
				logic.cs.UserDataManager:SaveHairHadBuy(tostring(hairID))
			else
				logic.cs.UIAlertMgr:Show("TIPS",json.msg)
				return
			end
		end)
	elseif self.modelType == ChoiceType.Clothes then
		if logic.cs.UserDataManager.UserData.DiamondNum < clothesCfg.price and not isFree then
			logic.bookReadingMgr.view:ShowChargeTips(clothesCfg.price)
			return
		end
		local hairSelect = 1
		local bookData = logic.bookReadingMgr.bookData
		if bookData.hair_id and bookData.hair_id ~= 0 then
			hairSelect = bookData.hair_id
		end
		
		local characterSelect = 1
		if bookData.character_id and bookData.character_id ~= 0 then
			characterSelect = bookData.character_id
		end
		
		local optionList = tostring(self.selectIdx[1])
		for i = 2, #self.selectIdx do
			optionList = optionList .. "," .. tostring(self.selectIdx[i])
		end
		logic.bookReadingMgr:SaveOptionList(optionList)
		logic.bookReadingMgr:SaveModel(characterSelect,clothesID,hairSelect)
		logic.bookReadingMgr:SaveProgress(function(result)
			local json = core.json.Derialize(result)
			local code = tonumber(json.code)
			if code == 200 then
				logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.McSelectOutfit,"","",tostring(logic.bookReadingMgr.bookData.BookID),tostring(self.cfg.dialogID),tostring(self.selectIdx[self.modelType]))
				logic.bookReadingMgr.bookData.PlayerClothes = clothesID
				self:ChocieAnim()
				
				logic.bookReadingMgr.view:ResetOperationTips()
				logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.dialog_choice_click)

				uiView.imgLeftArrow.raycastTarget = false
				uiView.imgRightArrow.raycastTarget = false
				
				logic.cs.UserDataManager:SaveOutfitHadBuy(tostring(clothesID))
				skinIsSelected = false
				self:OnChoicesModelNotify(result,nil,clothesID,hairSelect)
			else
				logic.cs.UIAlertMgr:Show("TIPS",json.msg)
				return
			end
		end)
	end
end


function ChoiceModel:OnChoicesModelNotify(result,skinID,clothesID,hairID)
	
	--logic.cs.UINetLoadingMgr:Close()
	logic.debug.Log("----ChoicesClothCostCallBack---->" .. result)
	local bookData = logic.bookReadingMgr.bookData
	local isOK = false
	local json = core.json.Derialize(result)
	local code = tonumber(json.code)
	if code == 200 then
		isOK = true
		logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.RewardWin)
	elseif code == 202 or code == 203 or code == 204 then --免费衣服
		logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.LoseFail)
		logic.bookReadingMgr.view:ShowChargeTips(self.cost)
	else
		if not string.IsNullOrEmpty(json.msg) then
			logic.cs.UITipsMgr:PopupTips(json.msg, false);
		end
	end
	
	uiView.imgLeftArrow.raycastTarget = true		
	uiView.imgRightArrow.raycastTarget = true
	if isOK then
		if skinID then
			bookData.character_id = skinID
		end
		if clothesID then
			bookData.outfit_id = clothesID
			self:ShowNextDialog()
		end
		if hairID then
			bookData.hair_id = hairID
		end
	end
end

return ChoiceModel
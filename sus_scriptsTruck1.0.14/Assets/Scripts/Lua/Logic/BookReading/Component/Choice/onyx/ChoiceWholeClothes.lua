local Class = core.Class

local Base = require("Logic/BookReading/Component/Choice/spain/BaseChoice")
local UICtrl = require("Logic/BookReading/Component/Choice/spain/UIChoiceCtrl")
local ChoiceWholeClothes = Class('ChoiceWholeClothes',Base)
local uiView = nil
local SaveProResult;

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
    resTable["Assets/Bundle/UI/BookReading/ChangeWholeClothes.prefab"] = BookResType.UIRes
    resTable[logic.bookReadingMgr.Res.bookCommonPath.."Atlas/ChangeClothes/bg.png"] = BookResType.BookRes
    resTable[logic.bookReadingMgr.Res.bookCommonPath.."Atlas/ChangeClothes/btn_last.png"] = BookResType.BookRes
    resTable[logic.bookReadingMgr.Res.bookCommonPath.."Atlas/ChangeClothes/btn_next.png"] = BookResType.BookRes
    resTable[logic.bookReadingMgr.Res.bookCommonPath.."Atlas/ChangeClothes/bg_btn.png"] = BookResType.BookRes
	resTable[logic.bookReadingMgr.Res.bookCommonPath.."Atlas/ChangeClothes/bg_btn2.png"] = BookResType.BookRes
	
	local bookData = logic.bookReadingMgr.bookData
	
	
	local bookFolderPath = logic.bookReadingMgr.Res.bookFolderPath
	local clothGroupId = 0
	--收集角色头像
	if self.cfg.trigger == ChoiceType.Player then
		local roleIDs = logic.bookReadingMgr.context.PlayerIDs
		for i=1,self.cfg.selection_num do
			local roleID = tonumber(self.cfg['selection_'..i])
			if not roleID then
				self.cfg['selection_'..i] = '0'
				roleID = 0
			end
			roleIDs[roleID] = 1
			
			local appearanceID = logic.bookReadingMgr:GetAppearanceID(1, clothGroupId, 1)
			resTable[bookFolderPath..'Role/'..appearanceID..'_SkeletonData.asset'] = BookResType.BookRes
		end
	--收集npc头像
	elseif self.cfg.trigger == ChoiceType.NPC then

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
			local sex = core.Mathf.Ceil(NpcDetailID/3)
			local appearanceID = logic.bookReadingMgr:GetNPCAppearanceID(sex, self.cfg.role_id, 1)
			resTable[bookFolderPath..'Role/'..appearanceID..'_SkeletonData.asset'] = BookResType.BookRes
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

		local appearanceID = logic.bookReadingMgr:GetAppearanceID(1, 0, 1)
		resTable[bookFolderPath..'Role/'..appearanceID..'_SkeletonData.asset'] = BookResType.BookRes
		--for id,_ in pairs(roleIDs) do
		--	IDs[id] = (IDs[id] and IDs[id] or {})
		--	for i=1,self.cfg.selection_num do
		--		local clothesID = tonumber(self.cfg['selection_'..i])
		--		if clothesID == nil then
		--			logic.debug.LogError(string.format('bookID=%d,chapter=%d,dialogID=%d,selection_%d填0做什么？',bookData.BookID,bookData.ChapterID,self.cfg.dialogID,i))
		--			goto continue
		--		end
		--		IDs[id][clothesID] = 1
		--		clothGroupId = core.Mathf.Ceil(clothesID/4)
		--		if clothGroupId == 0 then
		--			clothGroupId = 1
		--		end
		--		local appearanceID = logic.bookReadingMgr:GetAppearanceID(1, 0, clothGroupId)
		--		resTable[bookFolderPath..'Role/'..appearanceID..'_SkeletonData.asset'] = BookResType.BookRes
		--		::continue::
		--	end
		--end
	end

end

function ChoiceWholeClothes.InstallUI()
	if not uiView then
		local prefab = logic.bookReadingMgr.Res:GetPrefab("Assets/Bundle/UI/BookReading/ChangeWholeClothes.prefab")
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

		local get = logic.cs.LuaHelper.GetComponent

		uiView.fxSelectClothes = uiView.transform:Find('Dressing_ef').gameObject
		uiView.ClothesTF = uiView.transform:Find('ClothesGroup/Clothes_1')
		uiView.roleSpine = logic.SpineCharacter.New(uiView.ClothesTF:Find('spine').gameObject)
		
		uiView.imgClotheDetails = logic.cs.LuaHelper.GetComponent(uiView.transform,"ClotheDetails",typeof(logic.cs.Image))
		uiView.txtDesc = logic.cs.LuaHelper.GetComponent(uiView.transform,"ClotheDetails/ClothDescText",typeof(logic.cs.Text))
		uiView.imgLeftArrow = logic.cs.LuaHelper.GetComponent(uiView.transform,'LeftButton',typeof(logic.cs.Image))
		uiView.imgRightArrow = logic.cs.LuaHelper.GetComponent(uiView.transform,'RightButton',typeof(logic.cs.Image))
		uiView.imgDown = logic.cs.LuaHelper.GetComponent(uiView.transform,'ClotheDetails/DownButton',typeof(logic.cs.Image))

		uiView.btnConfirm1 = logic.cs.LuaHelper.GetComponent(uiView.transform,"ClotheDetails/ConfirmButton1",typeof(logic.cs.Button))
		uiView.btnDetails1 = logic.cs.LuaHelper.GetComponent(uiView.transform,'ClotheDetails/ConfirmButton1/DescText',typeof(logic.cs.Text))

		uiView.btnConfirm2 = logic.cs.LuaHelper.GetComponent(uiView.transform,"ClotheDetails/ConfirmButton2",typeof(logic.cs.Button))
		uiView.btnDetails2 = logic.cs.LuaHelper.GetComponent(uiView.transform,'ClotheDetails/ConfirmButton2/DescText',typeof(logic.cs.Text))

		uiView.imgConfirmCost2 = logic.cs.LuaHelper.GetComponent(uiView.transform,'ClotheDetails/ConfirmButton2/DiamondImage',typeof(logic.cs.Image))
		uiView.txtConfirmCost2 = logic.cs.LuaHelper.GetComponent(uiView.transform,'ClotheDetails/ConfirmButton2/DiamondImage/CostText',typeof(logic.cs.Text))

		uiView.imgScreenFlashImage = logic.cs.LuaHelper.GetComponent(uiView.transform,'ScreenFlashImage',typeof(logic.cs.Image))
		uiView.CharageEffectGo = uiView.transform:Find('ClothesGroup/fx_huanzhuan_shoufei').gameObject	
		uiView.ClothNumber=logic.cs.LuaHelper.GetComponent(uiView.transform,"ClotheDetails/ClothNumber",typeof(logic.cs.Text))	
		uiView.NumberList=uiView.transform:Find("NumberList").gameObject
		uiView.Point=uiView.transform:Find("NumberList/Point").gameObject
		uiView.Point:SetActiveEx(false)
		uiView.pointList = {}

		--set skin
		--logic.cs.ABSystem.ui:SetAtlasSprite(ui.imgClotheDetails, "BookReadingForm", "bg_smg")
		--logic.cs.ABSystem.ui:SetAtlasSprite(ui.imgLeftArrow, "BookReadingForm", "btn_left")
		--logic.cs.ABSystem.ui:SetAtlasSprite(ui.imgRightArrow, "BookReadingForm", "btn_right")
		--logic.cs.ABSystem.ui:SetAtlasSprite(ui.imgDown, "BookReadingForm", "btn_back_n")
		uiView.imgLeftArrow:SetNativeSize();
		uiView.imgRightArrow:SetNativeSize();
		uiView.imgDown:SetNativeSize();

		
		logic.cs.ABSystem.ui:SetAtlasSprite(uiView.btnConfirm1.image, "BookReadingForm", "btn_me_s")
		logic.cs.ABSystem.ui:SetAtlasSprite(uiView.btnConfirm2.image, "BookReadingForm", "btn_me_s1")


		-- 道具相关
		uiView.btnKeyProp = CS.DisplayUtil.GetChild(uiView.gameObject, "btnKeyProp"):GetComponent(typeof(logic.cs.Button))
		uiView.objBtnKeyProp = uiView.btnKeyProp.gameObject
		uiView.textKeyProp = CS.DisplayUtil.GetChild(uiView.gameObject, "textKeyProp"):GetComponent(typeof(logic.cs.Text))
		uiView.objItemParentProp = CS.DisplayUtil.GetChild(uiView.gameObject, "transItemParentProp")
		uiView.transItemParentProp = uiView.objItemParentProp.transform
		uiView.itemPrefabProp = CS.DisplayUtil.GetChild(uiView.gameObject, "itemPrefabProp")
		uiView.DiscountText = CS.DisplayUtil.GetChild(uiView.gameObject, "DiscountText"):GetComponent(typeof(logic.cs.Text))
		uiView.objBtnKeyProp:SetActive(false)
		uiView.itemPrefabProp:SetActive(false)
		uiView.textKeyProp.gameObject:SetActive(false)
		uiView.itemPropList = {}
		uiView.luckItemProp = nil
	end
end


function ChoiceWholeClothes:Clean()
	Base.Clean(self)
	uiView = nil
	print("界面关闭了")
	self:DestryPiont()
	
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


function ChoiceWholeClothes:OnPlayEnd(nextComponent)
	Base.OnPlayEnd(self,nextComponent)
	self.removeListener()
end

function ChoiceWholeClothes:Play()
	self.InstallUI()
	uiView.Reset()
	self.cost = 0
	uiView.btnKeyProp.onClick:RemoveAllListeners()
	uiView.btnKeyProp.onClick:AddListener(function()
		self:ShowPropList()
	end)

	self.isComplete = false
	logic.bookReadingMgr.view:BgAddBlurEffect(true)
	local onLeftArrowClick = function(data)
		self:ChangeClothesMove(true)
	end
	local onRightArrowClick = function(data)
		self:ChangeClothesMove(false)
	end
	local onDownClick = function(data)
		self:HideDetailInfoHandler()
	end
	local onConfirmClick = function(data)
		--print("确定按钮被点击了")
		uiView.imgLeftArrow.raycastTarget = false		
		uiView.imgRightArrow.raycastTarget = false

		self:OnComfirmClick()
	end
	local onScreenFlashClick = function(data)
		self:OnScreenFlashClick()
	end
	logic.cs.UIEventListener.AddOnClickListener(uiView.imgLeftArrow.gameObject,onLeftArrowClick)
	logic.cs.UIEventListener.AddOnClickListener(uiView.imgRightArrow.gameObject,onRightArrowClick)
	logic.cs.UIEventListener.AddOnClickListener(uiView.imgDown.gameObject,onDownClick)
	logic.cs.UIEventListener.AddOnClickListener(uiView.btnConfirm1.gameObject,onConfirmClick)
	logic.cs.UIEventListener.AddOnClickListener(uiView.btnConfirm2.gameObject,onConfirmClick)
	logic.cs.UIEventListener.AddOnClickListener(uiView.imgScreenFlashImage.gameObject,onScreenFlashClick)

	self.removeListener = function(self)
		logic.cs.UIEventListener.RemoveOnClickListener(uiView.imgLeftArrow.gameObject,onLeftArrowClick)
		logic.cs.UIEventListener.RemoveOnClickListener(uiView.imgRightArrow.gameObject,onRightArrowClick)
		logic.cs.UIEventListener.RemoveOnClickListener(uiView.imgDown.gameObject,onRightArrowClick)
		logic.cs.UIEventListener.RemoveOnClickListener(uiView.btnConfirm1.gameObject,onConfirmClick)
		logic.cs.UIEventListener.RemoveOnClickListener(uiView.btnConfirm2.gameObject,onConfirmClick)
		logic.cs.UIEventListener.RemoveOnClickListener(uiView.imgScreenFlashImage.gameObject,onScreenFlashClick)
	end


    logic.cs.EventDispatcher.Dispatch(logic.cs.EventEnum.ResidentMoneyInfo, 1)
    local bookData = logic.bookReadingMgr.bookData
	self.numOfClothes = self.cfg.selection_num

	self.spines = {}
	self.clothesIDs = {}

	local clothGroupId = 0
	if self.cfg.trigger == ChoiceType.Player then
		for i=1,self.cfg.selection_num do
			self.clothesIDs[i] = 1
			local appearanceID = logic.bookReadingMgr:GetAppearanceID(1,0, 1)
			local spine = logic.bookReadingMgr.Res:GetSkeDataAsset('Role/'..appearanceID)
			table.insert(self.spines, spine)
		end
	elseif self.cfg.trigger == ChoiceType.NPC then
		for i=1,self.cfg.selection_num do
			local clothesID = tonumber(self.cfg['selection_'..i])
			self.clothesIDs[i] = clothesID
			local sex = core.Mathf.Ceil(clothesID/3)
			local appearanceID = logic.bookReadingMgr:GetNPCAppearanceID(sex, self.cfg.role_id, 1)
			local spine = logic.bookReadingMgr.Res:GetSkeDataAsset('Role/'..appearanceID)
			table.insert(self.spines, spine)
		end
	else
		for i=1,self.cfg.selection_num do
			clothesID = tonumber(self.cfg['selection_'..i])
			self.clothesIDs[i] = clothesID
			local appearanceID = logic.bookReadingMgr:GetAppearanceID(1, 0, 1)
			local spine = logic.bookReadingMgr.Res:GetSkeDataAsset('Role/'..appearanceID)
			table.insert(self.spines, spine)
		end
	end

	for k,v in pairs(self.spines) do
		if v == nil then
			logic.debug.LogError('spine为空:index = '..k)
		end
	end



	self.mDownPosUp = core.Vector3.New(270, 295)
    self.mDownPowDo = core.Vector3.New(332, 115)

	uiView.imgLeftArrow.gameObject:SetActiveEx(self.numOfClothes > 1)
	uiView.imgRightArrow.gameObject:SetActiveEx(self.numOfClothes > 1)
	
	self.cost = 0
	self.selectIdx = 1
	self:SetSpineData(self.selectIdx)
	uiView.imgClotheDetails.gameObject:SetActiveEx(false)
	self:UpdateDetialsView()
	self:MoveDetailFrame(true,false)
	self:MoveLeftOrRightBtn(false)

	uiView.ClothNumber.text=CS.System.String.Format("CHOICE[{0}/{1}]",1,self.numOfClothes);
	self:SpwanClothPiont(self.numOfClothes)
	self:ShowPiontType(1)

	local bookDetails = logic.cs.GameDataMgr.table:GetBookDetailsById(logic.bookReadingMgr.bookID)


	--（RoleScale == 1）是 坐标  scaleX = 1.15   scaleY = 1,15
	--（RoleScale == 2）是 坐标  scaleX = 1.05   scaleY = 1,05
	local localScale = Vector3.New(1.15, 1.15, 1)
	local localSpinePos = Vector3.New(uiView.roleSpine.transform.localPosition.x,-308,0)
	if bookDetails.RoleScale == 2 then
		localScale = Vector3.New(1.05, 1.05, 1)
		localSpinePos = Vector3.New(uiView.roleSpine.transform.localPosition.x,-20,0)
	end
	uiView.roleSpine:SetScale(localScale)
	uiView.roleSpine.transform.localPosition = localSpinePos
end

---overrider
function ChoiceWholeClothes:OnScreenFlashClick()
	logic.bookReadingMgr.view:ResetOperationTips()

	self:ShowDetailInfoHandler()
end


function ChoiceWholeClothes:SetSpineData(selectIdx)

	local clothesID = self.clothesIDs[selectIdx]
	uiView.roleSpine:SetSpine(self.spines[selectIdx])


	local bookData = logic.bookReadingMgr.bookData
	local skinName = 'skin1'
	if self.cfg.trigger == ChoiceType.Player then
		skinName = 'skin'..selectIdx
	elseif self.cfg.trigger ==  ChoiceType.Clothes then
		skinName = 'skin'..bookData.PlayerDetailsID
	elseif self.cfg.trigger == ChoiceType.NPC then
		local tempClothesID = clothesID;
		if tempClothesID > 3 then
			tempClothesID = tempClothesID - 3
		end
		local skinIndex = tempClothesID 
		skinName = 'skin'.. skinIndex
		clothesID = 1
	end

	local clothesName = "clothes" .. clothesID
	local expressionName = "expression0"
	local hair1Name	= "hair1"
	local hair2Name	= "hair1_1"

	uiView.roleSpine:SetData(skinName,clothesName,expressionName,hair1Name,hair2Name)
end


function ChoiceWholeClothes:UpdateDetialsView()
    local bookData = logic.bookReadingMgr.bookData
    local clothesID = self.clothesIDs[self.selectIdx]
	self.buyType = BuyType.Free
	self.cost = 0
	
	if self.cfg.trigger == ChoiceType.Clothes then

		local appearanceID = logic.bookReadingMgr:GetAppearanceID(bookData.PlayerDetailsID, 1, clothesID)
		local skinCfg = logic.cs.GameDataMgr.table:GetSkinById(bookData.BookID,appearanceID)
		print("BookID:"..bookData.BookID.."==appearanceID:"..appearanceID)
		if skinCfg==nil then
			print("skinCfg为空")
		end
		if skinCfg then
			uiView.txtDesc.text = skinCfg.dec
		end
		
		if not logic.cs.UserDataManager:CheckClothHadCost(bookData.BookID, clothesID) then
			local priceCfg = logic.cs.GameDataMgr.table:GetClothePriceById(bookData.BookID, clothesID)
			if priceCfg and priceCfg.ClothePrice > 0 then
				self.buyType = priceCfg.PriceType
				self.cost = priceCfg.ClothePrice
	
				if self.buyType == BuyType.Video then
					local seeVideoNum = logic.cs.UserDataManager:GetSeeVideoNumOfClothes(bookData.BookID, clothesID)
					self.cost = self.cost - seeVideoNum
					if(self.cost < 0) then
						self.cost = 0
						self.buyType = BuyType.Free
					end
				end
			end
		end
		print("====>>"..clothesID.."==tye:"..type(clothesID))
		if logic.cs.UserDataManager:GetClothHadBuy(clothesID) then
			self.buyType = BuyType.Free
			self.cost = 0
		end
	elseif self.cfg.trigger == ChoiceType.NPC then
		uiView.txtDesc.text = "Tap left and right to choose your love interest."
	elseif self.cfg.trigger == ChoiceType.Player then
		uiView.txtDesc.text = "Tap left and right to choose your appereance."
	end
    self:updateView()
end


function ChoiceWholeClothes:SetComfirmType(type)
    uiView.btnConfirm1.gameObject:SetActiveEx(type == 1)
    uiView.btnConfirm2.gameObject:SetActiveEx(type == 2)
end

function ChoiceWholeClothes:updateView()
	local isNotFree = (self.cost > 0 and self.buyType ~= BuyType.Free)
	print("========衣服选项")
    if isNotFree then
        self:SetComfirmType(2)
    else
        self:SetComfirmType(1)
    end
    if self.buyType == BuyType.Diamonds then
        logic.cs.ABSystem.ui:SetAtlasSprite(uiView.imgConfirmCost2, "BookReadingForm", "icon_dimon")
    elseif self.buyType == BuyType.Video then
        logic.cs.ABSystem.ui:SetAtlasSprite(uiView.imgConfirmCost2, "BookReadingForm", "icon_video")
    end
    uiView.txtConfirmCost2.text ="<color=#AF691EFF>" .. tostring(self.cost) .. "</color>"
	uiView.textKeyProp.text = tostring(self.cost)
	if isNotFree then
		self:ShowPropBtn()
	end
end

function ChoiceWholeClothes:ShowPropBtn()
	local userPropInfo = logic.cs.UserDataManager.userPropInfo_Outfit
	if not userPropInfo or not userPropInfo.discount_list or userPropInfo.discount_list.Count<=0 then
		uiView.objBtnKeyProp:SetActive(false)
		return
	end
	uiView.objBtnKeyProp:SetActive(true)
	local discount_list = userPropInfo.discount_list
	uiView.transItemParentProp:ClearAllChild()
	uiView.itemPropList = {}
	local firstItem = nil
	for i=0,discount_list.Count-1,1 do
		local itemProp = self:AddItemProp(discount_list[i])
		if firstItem == nil then
			firstItem = itemProp
		end
	end
	local lastItem = self:AddItemProp(nil)
	if firstItem ~= nil then
		firstItem:fucOnClick()
	else
		lastItem:fucOnClick()
	end
end
function ChoiceWholeClothes:ShowPropList()
	local isShow = not uiView.objItemParentProp.activeSelf
	uiView.objItemParentProp.gameObject:SetActive(isShow)
	local isUse = uiView.txtConfirmCost2.text ~= uiView.textKeyProp.text
	uiView.textKeyProp.gameObject:SetActive(isUse)
end
function ChoiceWholeClothes:AddItemProp(data)
	local go = logic.cs.GameObject.Instantiate(uiView.itemPrefabProp,uiView.transItemParentProp,false)
	go:SetActive(true)
	local trans = go.transform
	local button = logic.cs.LuaHelper.GetComponent(trans,'Button',typeof(logic.cs.Button))
	local objCheck = trans:Find('checked').gameObject
	local txtNum = logic.cs.LuaHelper.GetComponent(trans,'Text',typeof(logic.cs.Text))
	if data~= nil then
		txtNum.text = data.discount_string or "No Discount"
	else
		txtNum.text = "No Discount"
	end
	local fucShowCheck = function(_self,isShow)
		_self.objCheck:SetActive(isShow)
	end
	local fucOnClick = function(_self)
		if uiView.luckItemProp ~= _self then
			self:OnClcikPropItem(_self)
		end
		uiView.objItemParentProp.gameObject:SetActive(false)
	end
	local item ={
		data = data,
		gameObject = go,
		transform = trans,
		button = button,
		objCheck = objCheck,
		txtNum = txtNum,
		fucShowCheck = fucShowCheck,
		fucOnClick = fucOnClick,
	}
	item.button.onClick:AddListener(function()
		item:fucOnClick()
	end)
	table.insert(uiView.itemPropList, item)
	return item
end
function ChoiceWholeClothes:OnClcikPropItem(propItem)
	for i,v in ipairs(uiView.itemPropList) do
		v:fucShowCheck(v==propItem)
	end
	uiView.luckItemProp = propItem
	local isUser = true
	if propItem.data==nil then
		isUser = false
	end
	logic.cs.UserDataManager:SetLuckyPropItem(isUser, propItem.data)

	--refresh ui
	uiView.textKeyProp.text = tostring(self.cost)
	uiView.textKeyProp.gameObject:SetActive(isUser)
	if isUser then
		local newCost = tonumber(self.cost)-tonumber(self.cost)*tonumber(propItem.data.discount)
		newCost = math.floor(newCost)
		uiView.txtConfirmCost2.text = tostring(newCost)
		uiView.DiscountText.text = propItem.data.discount_string
	else
		uiView.txtConfirmCost2.text = tostring(self.cost)
		uiView.DiscountText.text = ""
	end
end

function ChoiceWholeClothes:ChangeClothesMove(isLeftArrow)
	if logic.cs.BookReadingWrapper.IsTextTween then
		return
	end
	logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.dialog_choice_click)
	uiView.CharageEffectGo:SetActiveEx(false)
	local lastIndex = self.selectIdx
	if isLeftArrow then
		self.selectIdx = self.selectIdx - 1
	else
		self.selectIdx = self.selectIdx + 1
	end
	if self.selectIdx > self.numOfClothes then
		self.selectIdx = 1
	elseif self.selectIdx < 1 then
		self.selectIdx = self.numOfClothes
	end
	
	--print("当前衣服的编号是："..self.selectIdx.."--衣服总共的编号是："..self.numOfClothes)

	uiView.ClothNumber.text=CS.System.String.Format("CHOICE[{0}/{1}]",self.selectIdx,self.numOfClothes);
    self:ShowPiontType(self.selectIdx)

	self:MoveLeftOrRightBtn(true)
	self:MoveDetailFrame(not isLeftArrow,true)

	local viewSize = logic.bookReadingMgr.view.viewSize
	local moveTargetX = (isLeftArrow and viewSize.x) or -viewSize.x
	uiView.ClothesTF:DOLocalMoveX(moveTargetX, 0.6)
		:SetEase(core.tween.Ease.InQuad)
		:OnComplete(function()

			self:SetSpineData(self.selectIdx)


			local startX = (isLeftArrow and -viewSize.x) or viewSize.x
			local pos = uiView.ClothesTF.anchoredPosition
			pos.x = startX
			uiView.ClothesTF.anchoredPosition = pos
			uiView.ClothesTF:DOLocalMoveX(0, 0.6)
				:SetEase(core.tween.Ease.OutQuint)
				:OnComplete(function()
					self:MoveDetailFrame(not isLeftArrow,false)
				end):Play()
			self:MoveLeftOrRightBtn(false)
			self:UpdateDetialsView()
		end)
	logic.bookReadingMgr.view:ResetOperationTips()
end

function ChoiceWholeClothes:MoveDetailFrame(isRight,isOut)
	local viewSize = logic.bookReadingMgr.view.viewSize

	local detailsTF = uiView.imgClotheDetails.transform
	local downTF = uiView.imgDown.transform
	local imgConfirm = (self.buyType ~= BuyType.Free and uiView.btnConfirm2) or uiView.btnConfirm1
	local confirmTF = imgConfirm.transform

	if isOut then
		
		local moveTargetX = (isRight and -viewSize.x) or viewSize.x
		detailsTF:DOLocalMoveX(moveTargetX, 0.6):OnComplete(function()
			detailsTF.gameObject:SetActiveEx(false)
		end):Play()

		--[[
			ui.imgDown.transform.anchoredPosition = self.mDownPosUp
		ui.imgDown.transform:DOLocalMoveX(moveTargetX, 0.6):OnComplete(function()
			ui.imgDown.gameObject:SetActiveEx(false)
		end):Play()
		--]]
		
	else

		local pos = detailsTF.anchoredPosition
		pos.x = 0
		detailsTF.anchoredPosition = pos
		detailsTF.localScale = core.Vector3(1,0,1)
		--downTF.localScale = core.Vector3(1,0,1)
		confirmTF.localScale = core.Vector3(0,0,0)

		uiView.txtDesc.color = core.Color.New(0.2,0.2,0.2,0)
		detailsTF.gameObject:SetActiveEx(true)
		detailsTF:DOScaleY(1, 0.4)
			:SetEase(core.tween.Ease.OutBack)
			:OnComplete(function()
				--downTF.anchoredPosition = self.mDownPosUp
				--downTF.gameObject:SetActiveEx(true)
				--downTF:DOScaleY(1,0.4):Play()
				uiView.txtDesc:DOColor(core.Color.New(0.2,0.2,0.2,1),0.4):Play()
				confirmTF:DOScale(1,0.3):SetEase(core.tween.Ease.OutQuint):Play()
			end)
		
		local isNotFree = (self.cost > 0 and self.buyType ~= BuyType.Free)
		uiView.CharageEffectGo:SetActiveEx(isNotFree)
	end
end

function ChoiceWholeClothes:MoveLeftOrRightBtn(isOut)
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

function ChoiceWholeClothes:HideDetailInfoHandler()
	if self.mInHideState then
		self:ShowDetailInfoHandler()
	else
		self:MoveLeftOrRightBtn(true)
		local detailsTF = uiView.imgClotheDetails.transform
		local downTF = uiView.imgDown.transform

		--[[
			downTF.anchoredPosition = self.mDownPosUp
		downTF:DOAnchorPos(self.mDownPowDo,0.4)
			:SetEase(core.tween.Ease.InQuart)
			:OnComplete(function()
				downTF.localRotation = core.Quaternion.Euler(0,0,180)
			end):Play()
		--]]
		
		detailsTF:DOScaleY(0,0.4)
			:SetEase(core.tween.Ease.InQuart)
			:OnComplete(function()
				self.mInHideState = true

				uiView.imgScreenFlashImage.color = core.Color.New(1,1,1,0)
				uiView.imgScreenFlashImage.raycastTarget = true
				uiView.imgScreenFlashImage.gameObject:SetActiveEx(true)
			end):Play()
	end
end

function ChoiceWholeClothes:ShowDetailInfoHandler()
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

--region 选择


function ChoiceWholeClothes:OnComfirmClick()
	logic.bookReadingMgr.view:ResetOperationTips()
    logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.dialog_choice_click)
    local bookData = logic.bookReadingMgr.bookData
    local clothesID = self.clothesIDs[self.selectIdx]
	self.needPay = 0
	SaveProResult=""
	--print("==================selectIdx:"..self.selectIdx)
	local doChoice = function(needBuy)
	
		if (not needBuy) or logic.cs.UserDataManager:CheckClothHadCost(bookData.BookID, clothesID) then
			self:DoChoicesCloth()
			
		else
			self.needPay = 1
			--logic.cs.UINetLoadingMgr:Show()
		
			if self.buyType == BuyType.Diamonds then
				logic.bookReadingMgr:SaveOption(self.selectIdx)
				if self.cfg.trigger == ChoiceType.NPC then
					if self.cfg.trigger == ChoiceType.Clothes then
						logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.NpcSelectOutfit,"","",tostring(bookData.BookID),tostring(self.cfg.dialogID),tostring(self.selectIdx))
					else
						logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.NpcSelectCharacter,"","",tostring(bookData.BookID),tostring(self.cfg.dialogID),tostring(self.selectIdx))
					end
				else
					if self.cfg.trigger == ChoiceType.Clothes then
						logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.McSelectOutfit,"","",tostring(bookData.BookID),tostring(self.cfg.dialogID),tostring(self.selectIdx))
					else
						logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.McSelectCharacter,"","",tostring(bookData.BookID),tostring(self.cfg.dialogID),tostring(self.selectIdx))
					end
				end					logic.bookReadingMgr:SaveProgress(function(result)
				logic.cs.UserDataManager:SaveClothHadBuy(tostring(clothesID))
				SaveProResult=result
				self:OnChoicesClothCostNotify(result)
			end)
			end

			
		end
	end

	if self.buyType == BuyType.Diamonds then
		doChoice(true)
	elseif self.buyType == BuyType.Video then
		if self.cost > 0 then
			local seeVideoNum = logic.cs.UserDataManager:GetSeeVideoNumOfClothes(logic.bookReadingMgr.bookData.BookID, clothesID)
			logic.cs.SdkMgr.ads:ShowRewardBasedVideo("Buy-Clothes",function(success)
				uiView.imgLeftArrow.raycastTarget = true		
				uiView.imgRightArrow.raycastTarget = true
				if not success then
					return
				end
				seeVideoNum = seeVideoNum + 1
				logic.cs.UserDataManager:SetSeeVideoNumOfClothes(logic.bookReadingMgr.bookData.BookID, clothesID, seeVideoNum)
				self:UpdateDetialsView()
				if self.cost <= 0 then
					doChoice(true)
				end
			end)
		else
			doChoice(true)
		end
	else
		doChoice(false)
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


	if self.needPay==0 then
if self.cfg.trigger == ChoiceType.NPC then
			if self.cfg.trigger == ChoiceType.Clothes then
				logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.NpcSelectOutfit,"","",tostring(bookData.BookID),tostring(self.cfg.dialogID))
			else
				logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.NpcSelectCharacter,"","",tostring(bookData.BookID),tostring(self.cfg.dialogID))
			end
		else
			if self.cfg.trigger == ChoiceType.Clothes then
				logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.McSelectOutfit,"","",tostring(bookData.BookID),tostring(self.cfg.dialogID))
			else
				logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.McSelectCharacter,"","",tostring(bookData.BookID),tostring(self.cfg.dialogID))
			end
		end		logic.bookReadingMgr:SaveProgress(function(result)
			self:SetProgressHandler(result)
		end)
	elseif self.needPay==1 then
		self:SetProgressHandler(SaveProResult)
	end
	
	logic.cs.UserDataManager:RecordBookOptionSelect(logic.bookReadingMgr.bookData.BookID, self.cfg.dialogID, self.selectIdx);
end

function ChoiceWholeClothes:SetProgressHandler(result)
	if self.isComplete then
		return
	end
	local clothesID = self.clothesIDs[self.selectIdx]
	local bookData = logic.bookReadingMgr.bookData
	logic.debug.Log("----SetProgressHandler---->" .. result)
	local json = core.json.Derialize(result)
	--logic.debug.LogError(core.table.tostring(json))
	local code = tonumber(json.code) --坑，返回来的code是字符串
	if code == 200 then
		self.isComplete = true
		local func = function()
			uiView.fxSelectClothes:SetActiveEx(false)
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
			logic.cs.EventDispatcher.Dispatch(logic.cs.EventEnum.ResidentMoneyInfo, 0)
			logic.cs.EventDispatcher.Dispatch(logic.cs.EventEnum.ChangeBookReadingBgEnable, 1)
			logic.bookReadingMgr.view:BgAddBlurEffect(false)
			uiView.gameObject:SetActiveEx(false);
			self:ShowNextDialog()
		end
		uiView.fxSelectClothes.gameObject:SetActiveEx(true)
		uiView.imgScreenFlashImage.gameObject:SetActiveEx(true)
		uiView.imgScreenFlashImage:DOColor(core.Color.New(1,1,1,1), 0.05)
			:OnComplete(function()
				uiView.imgScreenFlashImage:DOColor(core.Color.New(1,1,1,0), 0.1)
					:OnComplete(function()
						uiView.imgScreenFlashImage:DOColor(core.Color.New(0,0,0,0.8), 0.4)
							:SetDelay(1.5)
							:OnComplete(function()
								func()
							end):Play()
					end):Play()
			end):Play()
		
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
	if code == 200 then

		logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.RewardWin)
		logic.cs.UserDataManager:SetChoicesClothResultInfo(result)
		if logic.cs.UserDataManager.choicesClothResultInfo and logic.cs.UserDataManager.choicesClothResultInfo.data then
			local purchase = logic.cs.UserDataManager.UserData.DiamondNum - logic.cs.UserDataManager.choicesClothResultInfo.data.user_diamond
			if purchase > 0 then
				logic.cs.TalkingDataManager:OnPurchase("ChoicesCloth cost diamond", purchase, 1)
			end
			logic.cs.UserDataManager:ResetMoney(1, logic.cs.UserDataManager.choicesClothResultInfo.data.user_key)
            logic.cs.UserDataManager:ResetMoney(2, logic.cs.UserDataManager.choicesClothResultInfo.data.user_diamond)
		end
		logic.bookReadingMgr.bookData.PlayerClothes = clothesID
		local appearanceID = logic.bookReadingMgr:GetAppearanceID(logic.bookReadingMgr.bookData.PlayerDetailsID,1,clothesID)
		logic.cs.UserDataManager:AddClothAfterPay(logic.bookReadingMgr.bookData.BookID, clothesID)
		self:DoChoicesCloth()

	elseif code == 202 or code == 203 then
		logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.LoseFail)
		logic.bookReadingMgr.view:ShowChargeTips(self.cost)
	else
		if not string.IsNullOrEmpty(json.msg) then
			logic.cs.UITipsMgr:PopupTips(json.msg, false);
		end
	end
	
	uiView.imgLeftArrow.raycastTarget = true		
	uiView.imgRightArrow.raycastTarget = true
end
--endregion



function ChoiceWholeClothes:SpwanClothPiont(num)
	if num > #uiView.pointList then
		for i = #uiView.pointList + 1, num do
			local go=logic.cs.Object.Instantiate(uiView.Point)
			go:SetActive(true)
			go.transform:SetParent(uiView.NumberList.transform)
			go.transform.localPosition=CS.UnityEngine.Vector3.zero
			go.transform.localScale=CS.UnityEngine.Vector3.one
			local target = go:GetComponent(typeof(logic.cs.UIToggle))
			table.insert(uiView.pointList, target)
		end
	elseif num == #uiView.pointList then
		for i = 1 , #uiView.pointList do
			local target = uiView.pointList[i]
			target.gameObject:SetActiveEx(true)
		end
	else
		--隐藏多余的点
		for i = num + 1, #uiView.pointList do
			local target = uiView.pointList[i]
			target.gameObject:SetActiveEx(false)
		end
	end
end

function ChoiceWholeClothes:ShowPiontType(index)
	for i = 1, #uiView.pointList do
		if i== index then
			uiView.pointList[i].isOn = true
			
		else
			uiView.pointList[i].isOn = false
			
		end
	end
end

function ChoiceWholeClothes:DestryPiont()

end

return ChoiceWholeClothes
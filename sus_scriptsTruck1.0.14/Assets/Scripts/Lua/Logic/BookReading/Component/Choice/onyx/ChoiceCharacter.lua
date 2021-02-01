local Class = core.Class
local Base = logic.BookReading.BaseComponent

local ChoiceCharacter = Class("ChoiceCharacter", Base)
local ui = nil

--收集所用到的资源
function ChoiceCharacter:CollectRes(resTable)
	Base.CollectRes(self,resTable)
    resTable["Assets/Bundle/UI/BookReading/ChoiceCharacter.prefab"] = BookResType.UIRes
    resTable[logic.bookReadingMgr.Res.bookCommonPath.."Atlas/EnterName/bg.png"] = BookResType.BookRes
	resTable[logic.bookReadingMgr.Res.bookCommonPath.."Atlas/EnterName/bg_input.png"] = BookResType.BookRes
	

	--收集所有角色半身像
    if self.isNpc then
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

            local appearanceID = logic.bookReadingMgr:GetAppearanceID(0,NpcID,self.cfg.icon) + NpcDetailID
            resTable[logic.bookReadingMgr.Res.bookFolderPath..'RoleClothes/'..appearanceID..'.png'] = BookResType.BookRes
        end
    else
        local roleIDs = logic.bookReadingMgr.context.PlayerIDs
        for i=1,self.cfg.selection_num do
            local roleID = tonumber(self.cfg['selection_'..i])
            if not roleID then
                self.cfg['selection_'..i] = '0'
                roleID = 0
            end
            roleIDs[roleID] = 1
            
            local appearanceID = logic.bookReadingMgr:GetAppearanceID(roleID,1,0)
            --resTable[logic.bookReadingMgr.Res.bookFolderPath..'RoleClothes/'..appearanceID..'.png'] = BookResType.BookRes
        end
	end
	
end


function ChoiceCharacter.InstallUI()
	if not ui then
		local prefab = logic.bookReadingMgr.Res:GetPrefab("Assets/Bundle/UI/BookReading/ChoiceCharacter.prefab")
		ui = {}
		ui.gameObject = logic.cs.GameObject.Instantiate(prefab,logic.bookReadingMgr.view.transComponent,false)
		ui.transform = ui.gameObject.transform
		
		ui.Reset = function()
			ui.gameObject:SetActiveEx(true)
			ui.touchMask:SetActiveEx(false)
		end

		ui.fxSelectClothes = ui.transform:Find('Dressing_ef').gameObject
		ui.imgClotheDetails = logic.cs.LuaHelper.GetComponent(ui.transform,"ClotheDetails",typeof(logic.cs.Image))
		ui.btnComfirm = logic.cs.LuaHelper.GetComponent(ui.transform,'ComfirmButton',typeof(logic.cs.Button))
		ui.imgComfirm = logic.cs.LuaHelper.GetComponent(ui.transform,'ComfirmButton',typeof(logic.cs.Image))
		ui.imgDiamond = logic.cs.LuaHelper.GetComponent(ui.transform,"ComfirmButton/DiamondImage",typeof(logic.cs.Image))
		ui.DescText = logic.cs.LuaHelper.GetComponent(ui.transform,"ComfirmButton/DescText",typeof(logic.cs.Text))

		-- ui.btnLeftArrow = logic.cs.LuaHelper.GetComponent(ui.transform,'LeftButton',typeof(logic.cs.Button))
		-- ui.btnRightArrow = logic.cs.LuaHelper.GetComponent(ui.transform,'RightButton',typeof(logic.cs.Button))

		ui.imgLeftArrow = logic.cs.LuaHelper.GetComponent(ui.transform,'LeftButton',typeof(logic.cs.Image))
		ui.imgRightArrow = logic.cs.LuaHelper.GetComponent(ui.transform,'RightButton',typeof(logic.cs.Image))

		--set skin
		--ui.imgClotheDetails.sprite = logic.bookReadingMgr.Res:GetSprite('Atlas/ChangeClothes/bg',true)
		--ui.imgLeftArrow.sprite = logic.bookReadingMgr.Res:GetSprite('Atlas/ChangeClothes/btn_last',true)
		--ui.imgRightArrow.sprite = logic.bookReadingMgr.Res:GetSprite('Atlas/ChangeClothes/btn_next',true)
		ui.imgLeftArrow:SetNativeSize()
		ui.imgRightArrow:SetNativeSize()
		ui.touchMask = ui.transform:Find('TouchMask').gameObject

		ui.ClothesGroup = ui.transform:Find('ClothesGroup')
		ui.clothes = {}
		local arr = ui.ClothesGroup:GetComponentsInChildren(typeof(logic.cs.Image),true)
		for i = 1, arr.Length do
			ui.clothes[i] = arr[i-1]
		end
	end
end


---overrider
-- function ChoiceCharacter:GetNextDialogID()
--     local id = self:GetNextDialogIDBySelection(self.selectIdx - 1);
--     return id
-- end


function ChoiceCharacter:OnPlayEnd(nextComponent)
	Base.OnPlayEnd(self,nextComponent)
	self.removeListener()
end


function ChoiceCharacter:Play()
	self.InstallUI()
	ui.Reset()

	
	logic.bookReadingMgr.view:BgAddBlurEffect(true)
	local onLeftArrowClick = function(data)
		self:ChangeClothesMove(true)
	end
	local onRightArrowClick = function(data)
		self:ChangeClothesMove(false)
	end
	local onConfirmClick = function(data)
		self:OnComfirmClick()
	end
	logic.cs.UIEventListener.AddOnClickListener(ui.imgLeftArrow.gameObject,onLeftArrowClick)
	logic.cs.UIEventListener.AddOnClickListener(ui.imgRightArrow.gameObject,onRightArrowClick)
	logic.cs.UIEventListener.AddOnClickListener(ui.btnComfirm.gameObject,onConfirmClick)

	self.removeListener = function()
		logic.cs.UIEventListener.RemoveOnClickListener(ui.imgLeftArrow.gameObject,onLeftArrowClick)
		logic.cs.UIEventListener.RemoveOnClickListener(ui.imgRightArrow.gameObject,onRightArrowClick)
		logic.cs.UIEventListener.RemoveOnClickListener(ui.btnComfirm.gameObject,onConfirmClick)
	end



	ui.ClothesGroup.anchoredPosition = core.Vector3.zero
	ui.gameObject:SetActiveEx(true)


	local detailCfg = logic.bookReadingMgr.Res.bookDetailCfg
	self:SetClothesDetails()
	self.cost = 0
end


function ChoiceCharacter:SetClothesDetails()
	self.numOfClothes = self.cfg.selection_num
	self.characterIDs = {}
	for i = 1, #ui.clothes do
		ui.clothes[i].gameObject:SetActiveEx(false)
	end
	for i=1,self.numOfClothes do
		local roleID = tonumber(self.cfg['selection_'..i])
		self.characterIDs[i] = roleID
		local appearanceID = 0
        if self.isNpc then
            appearanceID = logic.bookReadingMgr:GetAppearanceID(0,self.cfg.role_id,self.cfg.icon) + roleID
        else
            appearanceID = logic.bookReadingMgr:GetAppearanceID(roleID, 1, 0)
        end
		ui.clothes[i].sprite = logic.bookReadingMgr.Res:GetSprite("RoleClothes/" .. appearanceID, false)
		ui.clothes[i].color = Color.white
	end
	ui.imgLeftArrow.gameObject:SetActiveEx(self.numOfClothes > 1)
	ui.imgRightArrow.gameObject:SetActiveEx(self.numOfClothes > 1)
	ui.clothes[1].gameObject:SetActiveEx(true)
	self.selectIdx = 1
	self.cost = 0
	self:UpdateSelect()
end

--切换人物
function ChoiceCharacter:ChangeClothesMove(isLeftArrow)
	if logic.cs.BookReadingWrapper.IsTextTween then
		return
	end
	logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.dialog_choice_click)
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
	local duration = 0.6
	ui.clothes[lastIndex].color = core.Color.New(1,1,1,1)
	ui.clothes[self.selectIdx].color = core.Color.New(1,1,1,0)

	ui.clothes[lastIndex]:DOFade(0,duration)
	:SetEase(core.tween.Ease.Flash)
	:OnStart(function()
		logic.cs.BookReadingWrapper.IsTextTween = true
	end)

	ui.clothes[self.selectIdx]:DOFade(1,duration)
	:SetEase(core.tween.Ease.Flash)
	:OnStart(function()
		ui.clothes[self.selectIdx].gameObject:SetActiveEx(true)
	end)
	:OnComplete(function()
		logic.cs.BookReadingWrapper.IsTextTween = false
		ui.clothes[lastIndex].gameObject:SetActiveEx(false)
	end)
	ui.fxSelectClothes:SetActiveEx(false)
	ui.fxSelectClothes:SetActiveEx(true)
	logic.bookReadingMgr.view:ResetOperationTips()
	self:UpdateSelect()
end

function ChoiceCharacter:UpdateSelect()
	local needPay = self.cost > 0
	ui.imgDiamond.gameObject:SetActiveEx(needPay)
	if needPay then
		ui.imgComfirm.sprite = logic.bookReadingMgr.Res:GetSprite("Atlas/ChangeClothes/bg_btn", true)
		ui.DescText.transform.anchoredPosition = core.Vector2.New(-52, 4)
		ui.DescText.text = tostring(self.cost)
	else
		ui.imgComfirm.sprite = logic.bookReadingMgr.Res:GetSprite("Atlas/ChangeClothes/bg_btn2", true)
		ui.DescText.transform.anchoredPosition = core.Vector2.New(0, 4)
	end
end

function ChoiceCharacter:OnComfirmClick()
	ui.touchMask:SetActiveEx(true)
	logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.dialog_choice_click)
	

	local bookData = logic.bookReadingMgr.bookData
    local npcId = 0
    local npcCharacterId = 0
    if self.isNpc then
        npcId = self.cfg.role_id
        npcCharacterId = tonumber(self.cfg['selection_'..self.selectIdx])
        bookData.NpcId = npcId
		bookData.NpcDetailId = npcCharacterId
		logic.bookReadingMgr:SaveNpc(npcId,npcCharacterId)
    else
        bookData.PlayerDetailsID = tonumber(self.cfg['selection_'..self.selectIdx])
		logic.bookReadingMgr:SavePlayerDetailsID(bookData.PlayerDetailsID)
    end

	logic.bookReadingMgr:SaveOption(self.selectIdx)
    logic.bookReadingMgr:SaveProgress()
    logic.cs.UserDataManager:RecordBookOptionSelect(bookData.BookID, self.cfg.dialogid, self.selectIdx);



    if self.isNpc then
        logic.cs.TalkingDataManager:SelectNpc(bookData.BookID, npcCharacterId)
    else
        logic.cs.TalkingDataManager:SelectPlayer(bookData.BookID, bookData.PlayerDetailsID)
	end
	logic.bookReadingMgr.view:BgAddBlurEffect(false)
	ui.gameObject:SetActiveEx(false)
    self:ShowNextDialog()
end


function ChoiceCharacter:Clean()
	Base.Clean(self)
	ui = nil
end


return ChoiceCharacter
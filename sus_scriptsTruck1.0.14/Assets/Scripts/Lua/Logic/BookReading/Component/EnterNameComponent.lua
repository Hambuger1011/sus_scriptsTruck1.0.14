local Class = core.Class
local Base = logic.BookReading.BaseComponent

local EnterNameComponent = Class("EnterNameComponent", Base)
local ui = nil


function EnterNameComponent:__init(index,cfg)
	Base.__init(self,index,cfg)
end

--收集所用到的资源
function EnterNameComponent:CollectRes(resTable)
	Base.CollectRes(self,resTable)
    resTable["Assets/Bundle/UI/BookReading/EnterName.prefab"] = BookResType.UIRes
    resTable[logic.bookReadingMgr.Res.bookCommonPath.."Atlas/EnterName/bg.png"] = BookResType.BookRes
    resTable[logic.bookReadingMgr.Res.bookCommonPath.."Atlas/EnterName/bg_input.png"] = BookResType.BookRes
    resTable[logic.bookReadingMgr.Res.bookCommonPath.."Atlas/EnterName/btn_prey_64.png"] = BookResType.BookRes
    resTable[logic.bookReadingMgr.Res.bookCommonPath.."Atlas/EnterName/btn_purple_64.png"] = BookResType.BookRes
end



function EnterNameComponent:OnPlayEnd(nextComponent)
	Base.OnPlayEnd(self,nextComponent)
	self.removeListener()
end

function EnterNameComponent:Play()
	self.InstallUI()
	ui.Reset()
	local detailCfg = logic.bookReadingMgr.Res.bookDetailCfg
	local leadingSex = detailCfg.Gender or 0



	ui.inName.onValueChanged:RemoveAllListeners()
	ui.inName.onValueChanged:AddListener(function(value)
		self:OnEnterNameInputValueChange(value)
	end)


	local onConfirmClick = function(data)
		local haveBanWords = ui.bannedWords:haveBannedWords()
		if haveBanWords then
			logic.cs.UITipsMgr:ShowTips(CS.CTextManager.Instance:GetText(440));
		else
			self:OnSetPlayerName()
		end
	end
	logic.cs.UIEventListener.AddOnClickListener(ui.btnComfirm.gameObject,onConfirmClick)

	self.removeListener = function()
		logic.cs.UIEventListener.RemoveOnClickListener(ui.btnComfirm.gameObject,onConfirmClick)
	end


	local color = logic.cs.LuaHelper.ParseHtmlString('#'..detailCfg.EnterNameColor)
	ui.inName.textComponent.color = color
	ui.txtDesc.color = color
	ui.gameObject:SetActiveEx(true)
	ui.inName.text = ""
	self:OnEnterNameInputValueChange('')

	print("是否为中文："..detailCfg.Availability)
	if self.isNpc then
        if detailCfg.Availability==2 then
			ui.txtDesc.text = "输入你的伴侣的名字"
			ui.ComfirmButtonText.text="确定"
		else
			ui.txtDesc.text = "Enter your partner's name"
			ui.ComfirmButtonText.text="Confirm"
		end

		
		local randomName = ''
		if self.cfg.NpcDetailId and self.cfg.NpcDetailId > 3 then
			--女生
			randomName = logic.cs.GameDataMgr.table:GetNameByType(1)
		else
			--男生
			randomName = logic.cs.GameDataMgr.table:GetNameByType(0)
		end
		ui.inName.text = randomName
	else
		if detailCfg.Availability==2 then
			ui.txtDesc.text = "输入你的名字"
			ui.ComfirmButtonText.text="确定"
		else
			ui.txtDesc.text = "Enter your name"
			ui.ComfirmButtonText.text="Confirm"
		end
		
		--if string.IsNullOrEmpty(logic.cs.UserDataManager.UserData.bookNickName) then
			local randomName = logic.cs.GameDataMgr.table:GetNameByType(leadingSex);
			ui.inName.text = randomName
		--else
		--	ui.inName.text = logic.cs.UserDataManager.UserData.bookNickName
		--end
	end
end


function EnterNameComponent.InstallUI()
	if not ui then
		local prefab = logic.bookReadingMgr.Res:GetPrefab("Assets/Bundle/UI/BookReading/EnterName.prefab")
		ui = {}
		ui.gameObject = logic.cs.GameObject.Instantiate(prefab,logic.bookReadingMgr.view.transComponent,false)
		ui.transform = ui.gameObject.transform
		ui.inName = logic.cs.LuaHelper.GetComponent(ui.transform,"InputField",typeof(logic.cs.InputField))
		ui.btnComfirm = logic.cs.LuaHelper.GetComponent(ui.transform,"ComfirmButton",typeof(logic.cs.Button))
		ui.txtDesc = logic.cs.LuaHelper.GetComponent(ui.transform,"DescTxt",typeof(logic.cs.Text))
		ui.imgBG = logic.cs.LuaHelper.GetComponent(ui.transform,"BG",typeof(logic.cs.Image))
		ui.imgInputField= logic.cs.LuaHelper.GetComponent(ui.transform,"InputField",typeof(logic.cs.Image))
		ui.bannedWords = ui.imgInputField.gameObject:GetComponent("BannedWords")
		ui.ComfirmButtonText = logic.cs.LuaHelper.GetComponent(ui.transform,"ComfirmButton/Text",typeof(logic.cs.Text))
		ui.Reset = function()
			ui.gameObject:SetActiveEx(true)
		end

		--ui.imgBG.sprite = logic.bookReadingMgr.Res:GetSprite("Atlas/EnterName/bg",true)
		--ui.imgInputField.sprite = logic.bookReadingMgr.Res:GetSprite("Atlas/EnterName/bg_input",true)
	end
end


function EnterNameComponent:Clean()
	Base.Clean(self)
	ui = nil
end

function EnterNameComponent:OnEnterNameInputValueChange(value)
	if #value < 3 or #value > 12 then
		--ui.btnComfirm:GetComponent(typeof(logic.cs.Image)).sprite = logic.bookReadingMgr.Res:GetSprite('Atlas/EnterName/btn_prey_64',true)
	else
		--ui.btnComfirm:GetComponent(typeof(logic.cs.Image)).sprite = logic.bookReadingMgr.Res:GetSprite('Atlas/EnterName/btn_purple_64',true)
	end
end

function EnterNameComponent:OnSetPlayerName()
	logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.dialog_choice_click)
	local name = ui.inName.text
	if #name < 3 or #name > 12 then
		return
	end
	local npcId = 0
	local npcName = ''
	if self.isNpc then
		logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.NpcSetName,"","",tostring(logic.bookReadingMgr.bookData.BookID),tostring(self.cfg.dialogID))
		npcId = self.cfg.role_id
		npcName = name
		logic.bookReadingMgr.bookData.NpcName = name
		logic.debug.Log('npc name:'..name)
		logic.bookReadingMgr:SaveNpcName(npcId,npcName)
	else
		logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.McSetName,"","",tostring(logic.bookReadingMgr.bookData.BookID),tostring(self.cfg.dialogID))
		logic.debug.Log('player name:'..name)
		logic.bookReadingMgr.bookData.PlayerName = name
		logic.bookReadingMgr:SavePlayerName(name)

		if logic.cs.UserDataManager.UserData.bookNickName ~=  name then
			logic.cs.UserDataManager.UserData.bookNickName =  name
			if string.IsNullOrEmpty(logic.cs.UserDataManager.userInfo.data.userinfo.nickname) and 
			not string.IsNullOrEmpty(name) then
				logic.cs.GameHttpNet:SetUserLanguage(name,2,function(arg)
				end)
			end
		end
	end
	ui.gameObject:SetActiveEx(false)
	--保存进度
	logic.bookReadingMgr:SaveProgress()
	self:ShowNextDialog()
end

return EnterNameComponent
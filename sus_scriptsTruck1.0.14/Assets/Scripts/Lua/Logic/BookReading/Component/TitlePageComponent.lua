local Class = core.Class
local Base = logic.BookReading.BaseComponent

local TitlePageComponent = Class("TitlePageComponent", Base)
local ui = nil


function TitlePageComponent:__init(index,cfg)
	Base.__init(self,index,cfg)
end

--收集所用到的资源
function TitlePageComponent:CollectRes(resTable)
	Base.CollectRes(self,resTable)
    resTable["Assets/Bundle/UI/BookReading/TitlePage.prefab"] = BookResType.UIRes
end



function TitlePageComponent:OnPlayEnd(nextComponent)
	Base.OnPlayEnd(self,nextComponent)
	self.removeListener()
end

function TitlePageComponent:Play()
	self.InstallUI()
	ui.Reset()
	local startIndex , endIndex = string.find(self.cfg.dialog, "@Signature:", 1)
	if startIndex and endIndex then
		ui.signatureText.text = string.sub(self.cfg.dialog, endIndex + 1, #self.cfg.dialog)
		self.cfg.dialog = string.sub(self.cfg.dialog, 1, startIndex - 1)
		ui.bodyText.text = self.cfg.dialog
	else
		ui.signatureText.text = ""
		ui.bodyText.text = self.cfg.dialog
	end
	
	--local TitlePageData = core.json.Derialize(self.cfg.dialog)
	--logic.debug.PrintTable(TitlePageData,"TitlePageData")

	local bgButtonClick = function(data)
		self:AfterReading()
	end
	logic.cs.UIEventListener.AddOnClickListener(ui.bgButton.gameObject, bgButtonClick)

	self.removeListener = function()
		logic.cs.UIEventListener.RemoveOnClickListener(ui.bgButton.gameObject, bgButtonClick)
	end


	logic.cs.BookReadingWrapper.IsTextTween = true
	local showText = logic.bookReadingMgr:ReplaceChar(self.cfg.dialog)
	--播放显示动画
	logic.bookReadingMgr.view:textDialogTween(self,ui.Box,ui.bodyText,0,showText, function()
		logic.cs.BookReadingWrapper.IsTextTween = false
	end)
end


function TitlePageComponent.InstallUI()
	if not ui then
		local prefab = logic.bookReadingMgr.Res:GetPrefab("Assets/Bundle/UI/BookReading/TitlePage.prefab")
		ui = {}
		ui.gameObject = logic.cs.GameObject.Instantiate(prefab,logic.bookReadingMgr.view.transComponent,false)
		ui.transform = ui.gameObject.transform
		ui.bgButton = logic.cs.LuaHelper.GetComponent(ui.transform,"BgButton",typeof(logic.cs.Button))
		ui.Box = ui.transform:Find('Box').transform
		ui.bodyText = logic.cs.LuaHelper.GetComponent(ui.transform,"Text",typeof(logic.cs.TextTyperAnimation))
		ui.signatureText = logic.cs.LuaHelper.GetComponent(ui.transform,"Signature",typeof(logic.cs.Text))
		ui.Reset = function()
			ui.gameObject:SetActiveEx(true)
		end

	end
end

function TitlePageComponent:Clean()
	Base.Clean(self)
	ui = nil
end

function TitlePageComponent:AfterReading()
	if logic.cs.BookReadingWrapper.IsTextTween then
		ui.bodyText:StopTyperTween()
		logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.dialog_click)
	else
		logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.dialog_click)
		ui.gameObject:SetActiveEx(false)
		--保存进度
		logic.bookReadingMgr:SaveProgress()
		self:ShowNextDialog()
	end
end

return TitlePageComponent
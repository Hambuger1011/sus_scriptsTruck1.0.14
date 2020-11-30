local Class = core.Class
local Base = logic.BookReading.BaseComponent

local StoryItemsComponent = Class("StoryItemsComponent", Base)
local ui = nil


--收集所用到的资源
function StoryItemsComponent:CollectRes(resTable)
	Base.CollectRes(self,resTable)
	if self.cfg.selection_1  then
		resTable[logic.bookReadingMgr.Res.bookFolderPath.."StoryItems/icon" .. self.cfg.selection_1..'.png'] = BookResType.BookRes
	end
end


function StoryItemsComponent:Clean()
	Base.Clean(self)
	ui = nil
end


function StoryItemsComponent:Play()
	local uiform = logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.StoryItems)
	local root = uiform.transform
	local get = logic.cs.LuaHelper.GetComponent

	local frame = root:Find('Frame')
	local btnConfirm = get(root,'Frame/Button',typeof(logic.cs.Button))
	local imgIcon = get(root,'Frame/AdvImageBg/TypeImage',typeof(logic.cs.Image))
	local txtDesc = get(root,'Frame/TilteText',typeof(logic.cs.Text))

	btnConfirm.onClick:RemoveAllListeners()
	btnConfirm.onClick:AddListener(function()
		uiform:Close()
        logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.dialog_click)
        self:ShowNextDialog()
	end)

	imgIcon.sprite = logic.bookReadingMgr.Res:GetSprite("StoryItems/icon" .. self.cfg.selection_1)
	txtDesc.text = self.cfg.dialog
	frame.localScale = core.Vector3.New(0,0,1)
	frame:DOScale(core.Vector3.New(1,1,1),0.75):SetEase(core.tween.Ease.OutBack)
end

return StoryItemsComponent
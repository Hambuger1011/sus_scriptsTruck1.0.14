local Class = core.Class
local Base = logic.BookReading.BaseComponent
local UICtrl = require("Logic/BookReading/Component/Choice/spain/UIChoiceCtrl")
local BaseChoice = Class('BaseChoice',Base)
local ui = nil

--收集所用到的资源
function BaseChoice:CollectRes(resTable)
	Base.CollectRes(self,resTable)
    resTable["Assets/Bundle/UI/BookReading/ChoiceRole.prefab"] = BookResType.UIRes
end

function BaseChoice:getUI()
    if (not ui) or IsNull(ui.gameObject) then
        ui = UICtrl.New(logic.bookReadingMgr.UI:GetChoiceRole())
    end
    return ui
end

function BaseChoice:GetItems()
    error('未实现方法:GetItems')
end

function BaseChoice:OnChoiceIndex(idx)
    error('未实现方法:OnChoiceIndex')
end


function BaseChoice:OnConfirm(idx)
    error('未实现方法:OnConfirm')
end

return BaseChoice
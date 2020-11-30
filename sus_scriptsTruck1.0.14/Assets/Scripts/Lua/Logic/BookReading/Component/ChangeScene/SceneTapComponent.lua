local Class = core.Class
local Base = logic.BookReading.BaseComponent

local SceneTapComponent = Class("SceneTapComponent", Base)
local ui = nil



--收集所用到的资源
function SceneTapComponent:CollectRes(resTable)
    Base.CollectRes(self,resTable)
    --resTable["Assets/Bundle/UI/BookReading/ChangeSceneMask.prefab"] = BookResType.UIRes
end


function SceneTapComponent:Clean()
	Base.Clean(self)
end


function SceneTapComponent:GetNextDialogID()
	local id = self:GetNextDialogIDBySelection(self.selectIdx - 1)
    return id
end

function SceneTapComponent:Play()
    local data = CS.BaseDialogData()
    data.dialogID = self.cfg.dialogID
    data.selection_num = self.cfg.selection_num

    data.selection_1 = self.cfg.selection_1
    data.selection_2 = self.cfg.selection_2
    data.selection_3 = self.cfg.selection_3
    data.selection_4 = self.cfg.selection_4
    
    data.hidden_egg1 = self.cfg.hidden_egg1
    data.hidden_egg2 = self.cfg.hidden_egg2
    data.hidden_egg3 = self.cfg.hidden_egg3
    data.hidden_egg4 = self.cfg.hidden_egg4

    local uiform = logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.SceneTapForm)
    local tapForm = uiform:GetComponent(typeof(CS.SceneTapForm))
    tapForm:Init(self.cfg.trigger,data,function(selectIdx)
        self.selectIdx = selectIdx
        self:ShowNextDialog()
    end)
end

return SceneTapComponent
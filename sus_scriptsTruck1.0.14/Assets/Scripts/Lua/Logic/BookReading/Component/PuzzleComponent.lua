local Class = core.Class
local Base = logic.BookReading.BaseComponent

local PuzzleComponent = Class("PuzzleComponent", Base)



--收集所用到的资源
function PuzzleComponent:CollectRes(resTable)
    Base.CollectRes(self,resTable)
	-- local bookFolderPath = logic.bookReadingMgr.Res.bookFolderPath
    resTable["Assets/Bundle/Puzzle/1_0.png"] = BookResType.BookRes
end


function PuzzleComponent:Clean()
	Base.Clean(self)

end



function PuzzleComponent:Play()
	local uiform = logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.PuzzleForm)
    local puzzForm = uiform:GetComponent(typeof(CS.PuzzleForm))
    puzzForm:Init(self.cfg.trigger,function()
        self:ShowNextDialog()
    end)
end

return PuzzleComponent
local Class = core.Class
local Base = logic.BookReading.BaseComponent

local SplitviewComponent = Class("SplitviewComponent", Base)

local ViewType = 
{
	Center = 0,
	Left = 1,
	Right = 2,
}

--收集所用到的资源
function SplitviewComponent:CollectRes(resTable)
    Base.CollectRes(self,resTable)
end


function SplitviewComponent:Clean()
	Base.Clean(self)
end

function SplitviewComponent:OnSceneClick()
    --Base.OnSceneClick(self)
end

function SplitviewComponent:Play()
    self.viewType = self.cfg.trigger
    local curSceneBG = logic.bookReadingMgr.view.curSceneBG
    local offset = core.Vector2.zero
    local viewSize = logic.bookReadingMgr.view.viewSize
    local imgSize = curSceneBG.size
    if self.viewType == ViewType.Center then
        offset.x = 0
    elseif self.viewType == ViewType.Left then
        offset.x = -imgSize.x * 0.25 --width * 1/4
    elseif self.viewType == ViewType.Right then
        offset.x = imgSize.x * 0.25 --width * 1/4
    end
    logic.bookReadingMgr.view:sceneBGMove(offset,function()
        curSceneBG.offset = offset
        self:ShowNextDialog()
    end)
end

return SplitviewComponent
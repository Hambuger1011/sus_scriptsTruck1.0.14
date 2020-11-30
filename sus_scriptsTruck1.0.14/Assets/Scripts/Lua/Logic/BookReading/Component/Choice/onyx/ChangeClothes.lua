local Class = core.Class
local Base = require("Logic/BookReading/Component/Choice/onyx/ChoiceWholeClothes")

local ChangeClothes = Class("ChangeClothes", Base)
local ui = nil
local BuyType = {
	Free = 0,
	Diamonds = 1,
	Video = 2,
}


function ChangeClothes:__init(index,cfg)
	Base.__init(self, index, cfg)
	self.cfg.trigger = 2
end

--收集所用到的资源
function ChangeClothes:CollectRes(resTable)
    Base.CollectRes(self,resTable)

end

function ChangeClothes:Play()
	logic.debug.LogWarning('表类型填错了，类型=5的作废了，换衣服都用20')
	Base:Play()
end


return ChangeClothes
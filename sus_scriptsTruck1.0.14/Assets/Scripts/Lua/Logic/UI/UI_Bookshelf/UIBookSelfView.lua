--[[

--]]
local BaseClass = core.Class
local UIView = core.UIView
local V = BaseClass("UIBookShelfView", UIView)
local base = UIView

local uiid = logic.uiid
V.config = {
	ID = uiid.BookSelf,
	AssetName = "UI/Resident/Canvas_main",
}

function V:OnInitView()
    UIView.OnInitView(self)

end

function V:OnOpen()
    UIView.OnOpen(self)

end

function V:OnClose()
    UIView.OnOpen(self)
end

return V
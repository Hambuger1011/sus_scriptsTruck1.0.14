--[[
    View层，只负责界面相关
]]
local Class = core.Class
local cs_UIMgr = CS.UGUI.CUIManager.Instance

local UIView = Class("UIView")

---@class UIViewConfig
local Config = {
	ID = 0,
	AssetName = '',
}
---@type UIViewConfig
UIView.config = nil

function UIView:__init()
    --base.__init()
    self.isOpen = false
    self.uiform = nil
end

function UIView:__delete()
    self.isOpen = false
    self.uiform = nil
end


function UIView:__Open()
    self.isOpen = true
    if self.config.AssetName ~= nil and IsNull(self.uiform) then
        logic.debug.Log('open view:'..self.config.AssetName)
        self.uiform = cs_UIMgr:OpenForm(self.config.AssetName)
        self:OnInitView()
    else
        logic.debug.Log('open view:'..self.config.ID)
    end
    self:OnOpen()
end

function UIView:__Close()
    if self.isOpen then
        self.isOpen = false
        self:OnClose()
    end
    if not IsNull(self.uiform) then
        self.uiform:Close()     --调用这里关闭界面
        self.uiform = nil
    end
    core.UIMgr:RemoveView(self.config.ID)
end

function UIView:OnInitView()
end

function UIView:OnOpen()
end

function UIView:OnClose()
end

return UIView
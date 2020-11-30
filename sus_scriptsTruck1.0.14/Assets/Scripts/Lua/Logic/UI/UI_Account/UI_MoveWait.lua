local BaseClass = core.Class
local UIView = core.UIView
---@class UI_MoveWait
local UI_MoveWait = BaseClass("UI_MoveWait", UIView)
local uiid = logic.uiid

UI_MoveWait.config = {
    ID = uiid.MoveWait,
    AssetName = 'UI/Resident/UI/Canvas_MoveWait'
}

function UI_MoveWait:OnInitView()
    UIView.OnInitView(self)
    local root = self.uiform.transform

    self.KEEPWAITING =CS.DisplayUtil.GetChild(root.gameObject, "KEEPWAITING"):GetComponent(typeof(logic.cs.Button));

    --self.KEEPWAITING.onClick:RemoveAllListeners()
    --self.KEEPWAITING.onClick:AddListener(function()
    --    self:OnExitClick()
    --end)
end

function UI_MoveWait:OnOpen()
    UIView.OnOpen(self)
end

function UI_MoveWait:OnClose()
    UIView.OnClose(self)
end

function UI_MoveWait:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end

return UI_MoveWait
local BaseClass = core.Class
local UIView = core.UIView
---@class InvitePanel
local InvitePanel = BaseClass("InvitePanel", UIView)
local uiid = logic.uiid


InvitePanel.config = {
    ID = uiid.InvitePanel,
    AssetName = 'UI/Resident/UI/Canvas_InvitePanel'
}

function InvitePanel:OnInitView()
    UIView.OnInitView(self)
    local this=self.uiform
    self.closeButton =CS.DisplayUtil.GetChild(this.gameObject, "CloseButton")

    logic.cs.UIEventListener.AddOnClickListener(self.closeButton,function(data) self:OnExitClick() end)

end

function InvitePanel:OnOpen()
    UIView.OnOpen(self)
end

function InvitePanel:OnClose()
    UIView.OnClose(self)
end

function InvitePanel:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end

return InvitePanel
local UIView = core.UIView
local UIDovesForm = core.Class("UIDovesForm", UIView)


UIDovesForm.config = {
    ID = logic.uiid.UIDovesForm,
    AssetName = 'UI/Resident/UI/UIDovesForm'
}

--region【Awake】

local this=nil;
function UIDovesForm:OnInitView()
    UIView.OnInitView(self)
    this=self.uiform;

    self.CancelBtn = CS.DisplayUtil.GetChild(this.gameObject, "CancelBtn");
    self.BuyBtn = CS.DisplayUtil.GetChild(this.gameObject, "BuyBtn");

    logic.cs.UIEventListener.AddOnClickListener(self.BuyBtn,function(data) self:BuyBtnClick(); end)
    logic.cs.UIEventListener.AddOnClickListener(self.CancelBtn,function(data) self:OnExitClick(); end)
end
--endregion


--region【OnOpen】

function UIDovesForm:OnOpen()
    UIView.OnOpen(self)

end

--endregion


--region 【OnClose】

function UIDovesForm:OnClose()
    UIView.OnClose(self)
    if(self.BuyBtn)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.BuyBtn,function(data) self:BuyBtnClick(); end)
        logic.cs.UIEventListener.RemoveOnClickListener(self.CancelBtn,function(data) self:OnExitClick(); end)
    end
end

--endregion


function UIDovesForm:BuyBtnClick()
    GameController.ChatControl:BuyPrivateLetterRequest()
    self:OnExitClick();
end



--region 【界面关闭】
function UIDovesForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end
--endregion


return UIDovesForm
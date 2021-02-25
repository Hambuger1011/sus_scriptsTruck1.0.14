local UIView = core.UIView
local UIPersonalCenterForm = core.Class("UIPersonalCenterForm", UIView)
local this=UIPersonalCenterForm;

local uiid = logic.uiid
UIPersonalCenterForm.config = {
    ID = uiid.UIPersonalCenterForm,
    AssetName = 'UI/Resident/UI/UIPersonalCenterForm'
}

--region【Awake】

function UIPersonalCenterForm:OnInitView()
    UIView.OnInitView(self)
    
    local gameObject = self.uiform.gameObject

end
--endregion

--region【OnOpen】

function UIPersonalCenterForm:OnOpen()
    UIView.OnOpen(self)
    
    --按钮监听
    --logic.cs.UIEventListener.AddOnClickListener(self.RomanceTab.gameObject,function(data) self:RomanceTabClick(data) end);

end

--endregion

--region 【OnClose】

function UIPersonalCenterForm:OnClose()
    UIView.OnClose(self)

    --logic.cs.UIEventListener.RemoveOnClickListener(self.RomanceTab.gameObject,function(data) self:RomanceTabClick(data) end);

end

--endregion

--region 【界面关闭】
function UIPersonalCenterForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end
--endregion



return UIPersonalCenterForm
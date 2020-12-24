local UIView = core.UIView

---@class UIPakageForm
local UIPakageForm = core.Class("UIPakageForm", UIView)

local uiid = logic.uiid;
UIPakageForm.config = {
    ID = uiid.UIPakageForm,
    AssetName = 'UI/Resident/UI/UIPakageForm'
}



local this=nil;
function UIPakageForm:OnInitView()
    UIView.OnInitView(self)
    this=self.uiform;
    self.transform = this.gameObject

    --load other lua model
    self.objItemDetailPanel = CS.DisplayUtil.FindChild(self.gameObject, "BG/Panel/PropDetail")
    self.mItemDetailPanel = require('Logic/UI/UI_Pakage/PakageItemDetailPanel').New(self.objItemDetailPanel);
    self.objItem =CS.DisplayUtil.FindChild(self.gameObject, "BG/Panel/PropsList/Viewport/Layout/Item")
    self.mItem = require('Logic/UI/UI_Pakage/PakageItem').New(self.objItem);
    self.IconCahes = self:InitIconCahes()
    self.objNoProp = CS.DisplayUtil.FindChild(self.gameObject, "BG/Panel/NoProp")
    self.objPropsList = CS.DisplayUtil.FindChild(self.gameObject, "BG/Panel/PropsList")


    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.MailboxTab.gameObject,function(data) self:MailboxTabClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.PrivateLetterTab.gameObject,function(data) self:PrivateLetterTabClick(data) end);

    self.Bottom =CS.DisplayUtil.GetChild(this.gameObject, "Bottom");
    self.BatchBtn =CS.DisplayUtil.GetChild(self.Bottom, "BatchBtn");
    self.CollectBtn =CS.DisplayUtil.GetChild(self.Bottom, "CollectBtn");
    self.DeleteBtn =CS.DisplayUtil.GetChild(self.Bottom, "DeleteBtn");
    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.BatchBtn,function(data) self:BatchBtnClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.DeleteBtn,function(data) self:DeleteBtnClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.CollectBtn,function(data) self:CollectBtnClick(data) end);

end


function UIPakageForm:InitIconCahes()
    local go =CS.DisplayUtil.FindChild(self.gameObject, "BG/Panel/icon_caches")
    go:SetActive(false)
    local trans = go.transform
    local caches = {}
    -- local img = nil
    -- img = logic.cs.LuaHelper.GetComponent(trans, "icon_10",typeof(logic.cs.Image))
    -- caches.icon_10 = img
    -- img = logic.cs.LuaHelper.GetComponent(trans, "icon_10",typeof(logic.cs.Image))
    -- caches.icon_10 = img
    -- img = logic.cs.LuaHelper.GetComponent(trans, "icon_10",typeof(logic.cs.Image))
    -- caches.icon_10 = img
    -- img = logic.cs.LuaHelper.GetComponent(trans, "icon_10",typeof(logic.cs.Image))
    -- caches.icon_10 = img

    return caches
end


function UIPakageForm:OnOpen()
    UIView.OnOpen(self)
    GameController.EmailControl:SetData(self);
    self:MailboxTabClick(nil);
end

function UIPakageForm:OnClose()
    UIView.OnClose(self)

end


function UIPakageForm:SetData()
    self:RefreshUI()
end


function UIPakageForm:RefreshUI()
end


return UIPakageForm
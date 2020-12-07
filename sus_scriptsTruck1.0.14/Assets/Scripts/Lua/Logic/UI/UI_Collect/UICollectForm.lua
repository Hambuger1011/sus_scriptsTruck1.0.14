local BaseClass = core.Class
local UIView = core.UIView
---@class UICollectForm
local UICollectForm = BaseClass("UICollectForm", UIView)
local uiid = logic.uiid
local diamondsNum,keysNum
local needX2 = false
local CLAIMCallback


UICollectForm.config = {
    ID = uiid.UICollectForm,
    AssetName = 'UI/Resident/UI/UICollectForm'
}

function UICollectForm:OnInitView()
    UIView.OnInitView(self)
    local this=self.uiform
    self.Diamonds =CS.DisplayUtil.GetChild(this.gameObject, "Diamonds")
    self.Keys =CS.DisplayUtil.GetChild(this.gameObject, "Keys")
    self.ClothesCoupon =CS.DisplayUtil.GetChild(this.gameObject, "ClothesCoupon")
    self.OptionCoupon =CS.DisplayUtil.GetChild(this.gameObject, "OptionCoupon")
    self.DiamondsNum =CS.DisplayUtil.GetChild(this.gameObject, "DiamondsNum"):GetComponent(typeof(logic.cs.Text))
    self.KeysNum =CS.DisplayUtil.GetChild(this.gameObject, "KeysNum"):GetComponent(typeof(logic.cs.Text))
    self.ClothesCouponNum =CS.DisplayUtil.GetChild(this.gameObject, "ClothesCouponNum"):GetComponent(typeof(logic.cs.Text))
    self.OptionCouponNum =CS.DisplayUtil.GetChild(this.gameObject, "OptionCouponNum"):GetComponent(typeof(logic.cs.Text))
    self.CLAIM =CS.DisplayUtil.GetChild(this.gameObject, "CLAIM")
    self.CLAIMx2 =CS.DisplayUtil.GetChild(this.gameObject, "CLAIMx2")
    logic.cs.UIEventListener.AddOnClickListener(self.CLAIM,function(data) self:CLAIMOnClick() end)
end

local CLAIMCallback=nil;
function UICollectForm:SetData(_diamondsNum,_keysNum,_needX2,_CLAIMCallback,_clothesCouponNum,_optionCouponNum)
    if(_diamondsNum and tonumber(_diamondsNum) > 0)then
        self.DiamondsNum.text = "x".._diamondsNum;
        self.Diamonds.gameObject:SetActiveEx(true);
    else
        self.Diamonds.gameObject:SetActiveEx(false);
    end
    if(_keysNum and tonumber(_keysNum) > 0)then
        self.KeysNum.text = "x".._keysNum;
        self.Keys.gameObject:SetActiveEx(true);
    else
        self.Keys.gameObject:SetActiveEx(false);
    end
    if(_clothesCouponNum and tonumber(_clothesCouponNum) > 0)then
        self.ClothesCouponNum.text = "x".._clothesCouponNum;
        self.ClothesCoupon.gameObject:SetActiveEx(true);
    else
        self.ClothesCoupon.gameObject:SetActiveEx(false);
    end
    if(_optionCouponNum and tonumber(_optionCouponNum) > 0)then
        self.OptionCouponNum.text = "x".._optionCouponNum;
        self.OptionCoupon.gameObject:SetActiveEx(true);
    else
        self.OptionCoupon.gameObject:SetActiveEx(false);
    end
    self.CLAIMx2.gameObject:SetActiveEx(_needX2);
    CLAIMCallback = _CLAIMCallback
end

function UICollectForm:CLAIMOnClick()
    CLAIMCallback();
    self:OnExitClick();
end

function UICollectForm:OnOpen()
    UIView.OnOpen(self)
end

function UICollectForm:OnClose()
    UIView.OnClose(self)
end

function UICollectForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end

return UICollectForm
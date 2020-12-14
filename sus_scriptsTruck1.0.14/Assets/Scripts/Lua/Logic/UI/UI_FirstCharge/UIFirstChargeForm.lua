local BaseClass = core.Class
local UIView = core.UIView

local UIFirstChargeForm = BaseClass("UIFirstChargeForm", UIView)
local base = UIView

local uiid = logic.uiid

local PosList = {
    core.Vector2.New(232,150),core.Vector2.New(83,280),
    core.Vector2.New(-220,150),core.Vector2.New(-83,280),
}
local RewardTrans = {}

UIFirstChargeForm.config = {
    ID = uiid.UIFirstChargeForm,
    AssetName = 'UI/Resident/UI/UIFirstChargeForm'
}


function UIFirstChargeForm:OnInitView()

    UIView.OnInitView(self)
    local get = logic.cs.LuaHelper.GetComponent
    local root = self.uiform.transform
    RewardTrans = {}

    self.Reward =CS.DisplayUtil.GetChild(root.gameObject, "Reward"):GetComponent(typeof(logic.cs.CanvasGroup))
    self.TreasureBox =CS.DisplayUtil.GetChild(root.gameObject, "TreasureBox")
    self.Keys =CS.DisplayUtil.GetChild(root.gameObject, "Keys")
    self.Diamonds =CS.DisplayUtil.GetChild(root.gameObject, "Diamonds")
    self.ClothesCoupon =CS.DisplayUtil.GetChild(root.gameObject, "ClothesCoupon")
    self.OptionCoupon =CS.DisplayUtil.GetChild(root.gameObject, "OptionCoupon")
    RewardTrans = {self.Keys,self.Diamonds,self.ClothesCoupon,self.OptionCoupon}
    
    self.GoBtn = get(root,'Bg/GoBtn',typeof(logic.cs.Button))
    self.Close = get(root,'Bg/Close',typeof(logic.cs.Button))
    self.Tile = get(root,'Bg/Tile',typeof(logic.cs.Text))
    
    self.GoBtn.onClick:RemoveAllListeners()
    self.GoBtn.onClick:AddListener(function()
        self:OnGoBtn()
    end)

    self.Close.onClick:RemoveAllListeners()
    self.Close.onClick:AddListener(function()
        self:OnExitClick()
    end)

    self:PlayAnim();
end

function UIFirstChargeForm:OnOpen()
    UIView.OnOpen(self)
end

function UIFirstChargeForm:OnClose()
    UIView.OnClose(self)
    self.GoBtn.onClick:RemoveAllListeners()
    self.Close.onClick:RemoveAllListeners()

end

function UIFirstChargeForm:PlayAnim()
    local duration = 1
    self.Reward.alpha = 0
    self.Reward:DOFade(1,duration)

    for i = 1, #RewardTrans do
        RewardTrans[i].transform:DOAnchorPos(PosList[i],duration):SetEase(core.tween.Ease.Flash):OnComplete(function()  end)
    end

    self.TreasureBox.transform.localScale = core.Vector3.New(0.5,0.5,1)
    self.TreasureBox.transform:DOScale(core.Vector3.New(1,1,1),duration):SetEase(core.tween.Ease.OutBack)
end


function UIFirstChargeForm:OnGoBtn()
    self:OnExitClick()
    local uiform = logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.ChargeMoneyForm)
    local tapForm = uiform:GetComponent(typeof(CS.ChargeMoneyForm))
    tapForm:SetFormStyle(2);
    uiform = logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.MainFormTop)
    tapForm = uiform:GetComponent(typeof(CS.MainTopSprite))
    tapForm:GamePlayTopOpen("UIChargeMoneyForm1");
end




function UIFirstChargeForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end

return UIFirstChargeForm
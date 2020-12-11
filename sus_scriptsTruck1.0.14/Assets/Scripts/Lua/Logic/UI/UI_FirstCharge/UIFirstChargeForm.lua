local BaseClass = core.Class
local UIView = core.UIView

local UIFirstChargeForm = BaseClass("UIFirstChargeForm", UIView)
local base = UIView

local uiid = logic.uiid

UIFirstChargeForm.config = {
    ID = uiid.UIFirstChargeForm,
    AssetName = 'UI/Resident/UI/UIFirstChargeForm'
}


function UIFirstChargeForm:OnInitView()

    UIView.OnInitView(self)
    local get = logic.cs.LuaHelper.GetComponent
    local root = self.uiform.transform

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

    --刷新展示
    --self:UpdateShow(get,root);
end

function UIFirstChargeForm:OnOpen()
    UIView.OnOpen(self)
end

function UIFirstChargeForm:OnClose()
    UIView.OnClose(self)
    self.GoBtn.onClick:RemoveAllListeners()
    self.Close.onClick:RemoveAllListeners()

end

--刷新展示
function UIFirstChargeForm:UpdateShow(get,root)

    self.Tile.text=CS.CTextManager.Instance:GetText(343);  -- "自动签到标题"
    self.SignInBtnText.text=CS.CTextManager.Instance:GetText(344);  -- "签到"

    --id; --奖励id
    --type; --奖励类型 1钥匙 2钻石 4组合包
    --bkey_qty; --奖励钥匙
    --diamond_qty;  --奖励钻石
    --is_receive; --是否已签到领取 1:是 0否

    if(Cache.SignInCache.activity_login.type==1)then    --1钥匙

        self.Type1:SetActiveEx(true);
        self.RewardImg.sprite = CS.ResourceManager.Instance:GetUISprite("ADSReward/qd-yaoshi");
        self.KeyText = get(root,'Canvas/Bg/Type1/KeyImg/KeyText',typeof(logic.cs.Text))
        self.KeyText.text="X"..Cache.SignInCache.activity_login.bkey_qty;

    elseif(Cache.SignInCache.activity_login.type==2)then  -- 2钻石

        self.Type2:SetActiveEx(true);
        self.RewardImg.sprite = CS.ResourceManager.Instance:GetUISprite("ADSReward/qd-zuanshi3");
        self.DiamondText = get(root,'Canvas/Bg/Type2/DiamondImg/DiamondText',typeof(logic.cs.Text))
        self.DiamondText.text="X"..Cache.SignInCache.activity_login.diamond_qty;


    elseif(Cache.SignInCache.activity_login.type==4)then   -- 4组合包

        self.Type3:SetActiveEx(true);
        self.RewardImg.sprite = CS.ResourceManager.Instance:GetUISprite("ADSReward/qd-zuanshi_yaoshi");
        self.KeyText = get(root,'Canvas/Bg/Type3/KeyImg/KeyText',typeof(logic.cs.Text))
        self.DiamondText = get(root,'Canvas/Bg/Type3/DiamondImg/DiamondText',typeof(logic.cs.Text))
        self.KeyText.text="X"..Cache.SignInCache.activity_login.bkey_qty;
        self.DiamondText.text="X"..Cache.SignInCache.activity_login.diamond_qty;


    end

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
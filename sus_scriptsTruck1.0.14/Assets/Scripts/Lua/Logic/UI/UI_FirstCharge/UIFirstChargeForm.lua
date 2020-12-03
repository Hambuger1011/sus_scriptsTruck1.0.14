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

    self.RewardImg= get(root,'Canvas/Bg/RewardImg',typeof(logic.cs.Image))
    self.SignInBtn = get(root,'Canvas/Bg/SignInBtn',typeof(logic.cs.Button))
    self.Type1=root:Find('Canvas/Bg/Type1').gameObject
    self.Type2=root:Find('Canvas/Bg/Type2').gameObject
    self.Type3=root:Find('Canvas/Bg/Type3').gameObject
    self.Close = get(root,'Canvas/Bg/Close',typeof(logic.cs.Button))
    self.Tile = get(root,'Canvas/Bg/Tile',typeof(logic.cs.Text))
    self.SignInBtnText = get(root,'Canvas/Bg/SignInBtn/SignInBtnText',typeof(logic.cs.Text))
    self.UIMask = root:Find('Canvas/UIMask').gameObject
    self.SignInBtn.onClick:RemoveAllListeners()
    self.SignInBtn.onClick:AddListener(function()
        self:OnSignBtn()
    end)

    self.Close.onClick:AddListener(function()
        self:OnExitClick()
    end)
    logic.cs.UIEventListener.AddOnClickListener(self.UIMask,function(data) self:OnExitClick() end)

    --刷新展示
    self:UpdateShow(get,root);
end

function UIFirstChargeForm:OnOpen()
    UIView.OnOpen(self)

    --【每日一次】
    --【AF事件记录*发送 每日登录事件】
    --CS.AppsFlyerManager.Instance:DailyLogin();

end

function UIFirstChargeForm:OnClose()
    UIView.OnClose(self)
    logic.cs.UIEventListener.RemoveOnClickListener(self.UIMask,function(data) self:OnExitClick() end)
    self.SignInBtn.onClick:RemoveAllListeners()
    self.Close.onClick:RemoveAllListeners()

    self.RewardImg = nil;
    self.Type1 = nil;
    self.Type2 = nil;
    self.Type3 = nil;
    self.Close = nil;
    self.Tile = nil;
    self.SignInBtnText = nil;
    self.UIMask = nil;
    self.SignInBtn = nil;
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


function UIFirstChargeForm:OnSignBtn()

    if(Cache.SignInCache.activity_login.is_receive==1)then
    return;
    end


    logic.gameHttp:ReceiveDailyLoginAward(function(result)
        logic.debug.Log("----ReceiveDailyLoginAward---->" .. result)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code ==200 then
            logic.cs.UserDataManager:ResetMoney(1, tonumber(json.data.bkey))
            logic.cs.UserDataManager:ResetMoney(2, tonumber(json.data.diamond))
            self.SignInBtn.gameObject:SetActiveEx(false)
            Cache.SignInCache.activity_login.is_receive=1;
            --CS.Dispatcher.dispatchEvent(CS.EventEnum.RequestRedPoint);
            GameController.MainFormControl:RedPointRequest();

            --【AF事件记录*发送 签到1次】
            CS.AppsFlyerManager.Instance:SIGN();
           --关闭界面
            self:OnExitClick();
        end
    end)

end




function UIFirstChargeForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end

return UIFirstChargeForm
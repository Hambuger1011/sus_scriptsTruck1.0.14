local BaseClass = core.Class
local UIView = core.UIView

local UISignTipForm = BaseClass("UISignTipForm", UIView)
local base = UIView

local uiid = logic.uiid

UISignTipForm.config = {
    ID = uiid.UISignTipForm,
    AssetName = 'UI/Resident/UI/UISignTipForm'
}


function UISignTipForm:OnInitView()

    UIView.OnInitView(self)
    local get = logic.cs.LuaHelper.GetComponent
    local root = self.uiform.transform

    self.SignInBtn = get(root,'Canvas/Bg/SignInBtn',typeof(logic.cs.Button))
    self.ItemImg= get(root,'Canvas/Bg/RewardImg/ItemImg',typeof(logic.cs.Image))
    self.ItemName = get(root,'Canvas/Bg/ItemName',typeof(logic.cs.Text))
    self.ItemNum = get(root,'Canvas/Bg/ItemNum',typeof(logic.cs.Text))
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
        GameController.WindowConfig:ShowNextWindow()
    end)
    logic.cs.UIEventListener.AddOnClickListener(self.UIMask,function(data) 
        self:OnExitClick()
        GameController.WindowConfig:ShowNextWindow()
    end)

    --刷新展示
    self:UpdateShow(get,root);
end

function UISignTipForm:OnOpen()
    UIView.OnOpen(self)

    --【每日一次】
    --【AF事件记录*发送 每日登录事件】
    --CS.AppsFlyerManager.Instance:DailyLogin();

end

function UISignTipForm:OnClose()
    UIView.OnClose(self)
    logic.cs.UIEventListener.RemoveOnClickListener(self.UIMask,function(data)
        self:OnExitClick()
        GameController.WindowConfig:ShowNextWindow()
    end)
    self.SignInBtn.onClick:RemoveAllListeners()
    self.Close.onClick:RemoveAllListeners()

    self.Close = nil;
    self.Tile = nil;
    self.SignInBtnText = nil;
    self.UIMask = nil;
    self.SignInBtn = nil;
end

--刷新展示
function UISignTipForm:UpdateShow()

    self.Tile.text=CS.CTextManager.Instance:GetText(343);  -- "自动签到标题"
    self.SignInBtnText.text=CS.CTextManager.Instance:GetText(344);  -- "签到"

    --id; --奖励id
    --type; --奖励类型 1钥匙 2钻石 4组合包
    --bkey_qty; --奖励钥匙
    --diamond_qty;  --奖励钻石
    --is_receive; --是否已签到领取 1:是 0否

    local numText
    local nameText
    local sprite
    local size
    if Cache.SignInCache.activity_login.diamond_qty and Cache.SignInCache.activity_login.diamond_qty > 0 then
        numText = Cache.SignInCache.activity_login.diamond_qty
        sprite = Cache.PropCache.SpriteData[1]
        size = core.Vector3.New(1.5,1.5,1)
        nameText = "Diamond"
    elseif Cache.SignInCache.activity_login.bkey_qty and Cache.SignInCache.activity_login.bkey_qty > 0 then
        numText = Cache.SignInCache.activity_login.bkey_qty
        sprite = Cache.PropCache.SpriteData[2]
        size = core.Vector3.New(1.5,1.5,1)
        nameText = "Key"
    elseif Cache.SignInCache.activity_login.item_list and #Cache.SignInCache.activity_login.item_list > 0 then
        for k, v in pairs(Cache.SignInCache.activity_login.item_list) do
            numText = v.num
            nameText = v.name
            if 1000<tonumber(v.id) and tonumber(v.id)<10000 then
                sprite=DataConfig.Q_DressUpData:GetSprite(v.id)
            else
                sprite = Cache.PropCache.SpriteData[v.id]
            end
        end
        size = core.Vector3.New(1,1,1)
    end
    self.ItemName.text = nameText
    self.ItemNum.text = "×"..numText
    self.ItemImg.sprite = sprite
    self.ItemImg:SetNativeSize()
    self.ItemImg.transform.localScale = size

end


function UISignTipForm:OnSignBtn()

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
            GameController.WindowConfig:ShowNextWindow()
        end
    end)

end




function UISignTipForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end

return UISignTipForm
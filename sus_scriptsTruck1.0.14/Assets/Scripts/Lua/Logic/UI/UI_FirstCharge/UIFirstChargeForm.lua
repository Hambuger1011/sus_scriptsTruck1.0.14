local BaseClass = core.Class
local UIView = core.UIView

local UIFirstChargeForm = BaseClass("UIFirstChargeForm", UIView)
local base = UIView

local uiid = logic.uiid

local StartPosList =
{
    {core.Vector2.New(3,197)},

    {core.Vector2.New(-84,159),core.Vector2.New(92,159)},

    {core.Vector2.New(122,120),core.Vector2.New(-115,120),
     core.Vector2.New(3,197)},

    {core.Vector2.New(149,88),core.Vector2.New(64,175),
     core.Vector2.New(-122,84),core.Vector2.New(-45,171)},

    {core.Vector2.New(129,88),core.Vector2.New(87,171),
     core.Vector2.New(-123,71),core.Vector2.New(-90,167),
     core.Vector2.New(3,197),}
}
local PosList =
{
    {core.Vector2.New(3,317)},

    {core.Vector2.New(-134,259),core.Vector2.New(142,259)},

    {core.Vector2.New(190,215),core.Vector2.New(-186,215),
     core.Vector2.New(3,317)},

    {core.Vector2.New(241,132),core.Vector2.New(102,259),
     core.Vector2.New(-239,130),core.Vector2.New(-94,259)},

    {core.Vector2.New(241,132),core.Vector2.New(142,259),
     core.Vector2.New(-239,130),core.Vector2.New(-134,259),
     core.Vector2.New(3,317),}
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
    self.BoxUp =CS.DisplayUtil.GetChild(root.gameObject, "BoxUp")

    local firstRecharge = Cache.ActivityCache.first_recharge;
    self.Item =CS.DisplayUtil.GetChild(root.gameObject, "Item")


    for k, v in pairs(firstRecharge.item_list) do
        local item = logic.cs.GameObject.Instantiate(self.Item,self.Reward.transform,false)
        local Num = CS.DisplayUtil.GetChild(item, "Num"):GetComponent(typeof(logic.cs.Text))
        local Icon = CS.DisplayUtil.GetChild(item, "Icon"):GetComponent(typeof(logic.cs.Image))
        Num.text = "x".. v.num;
        if 1000<tonumber(v.id) and tonumber(v.id)<10000 then
            Icon.sprite = Cache.PropCache.SpriteData[3]
        else
            Icon.sprite = Cache.PropCache.SpriteData[v.id]
        end
        --Icon:SetNativeSize()
        --Icon.transform.localScale = core.Vector3.New(0.5,0.5,1)
        item:SetActive(true)
        table.insert(RewardTrans,item)
    end
    
    if tonumber(firstRecharge.diamond_count) > 0 then
        local item = logic.cs.GameObject.Instantiate(self.Item,self.Reward.transform,false)
        local Num = CS.DisplayUtil.GetChild(item, "Num"):GetComponent(typeof(logic.cs.Text))
        local Icon = CS.DisplayUtil.GetChild(item, "Icon"):GetComponent(typeof(logic.cs.Image))
        Num.text = "x".. firstRecharge.diamond_count;
        Icon.sprite = Cache.PropCache.SpriteData[1]
        table.insert(RewardTrans,item)
    end
    if tonumber(firstRecharge.key_count) > 0 then
        local item = logic.cs.GameObject.Instantiate(self.Item,self.Reward.transform,false)
        local Num = CS.DisplayUtil.GetChild(item, "Num"):GetComponent(typeof(logic.cs.Text))
        local Icon = CS.DisplayUtil.GetChild(item, "Icon"):GetComponent(typeof(logic.cs.Image))
        Num.text = "x".. firstRecharge.key_count;
        Icon.sprite = Cache.PropCache.SpriteData[2]
        table.insert(RewardTrans,item)
    end

    
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

    self.BoxUp.transform:DORotate( core.Vector3(0,0,0),duration)

    local size = #RewardTrans > #PosList and #PosList or #RewardTrans
    for i = 1, size do
        RewardTrans[i].transform.anchoredPosition =StartPosList[size][i]
        RewardTrans[i]:SetActive(true)
        RewardTrans[i].transform:DOAnchorPos(PosList[size][i],duration):SetEase(core.tween.Ease.Flash):OnComplete(function()  end)
    end

    self.TreasureBox.transform.localScale = core.Vector3.New(0.5,0.5,duration)
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
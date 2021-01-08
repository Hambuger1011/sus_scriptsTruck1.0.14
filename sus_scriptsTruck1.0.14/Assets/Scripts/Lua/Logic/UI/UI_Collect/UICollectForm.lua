local BaseClass = core.Class
local UIView = core.UIView
---@class UICollectForm
local UICollectForm = BaseClass("UICollectForm", UIView)
local uiid = logic.uiid
local diamondsNum,keysNum
local needX2 = false
local CLAIMCallback
local PosList = {
    {core.Vector2.New(0,50)},
    {core.Vector2.New(-150,50),core.Vector2.New(150,50)},
    {core.Vector2.New(-150,50),core.Vector2.New(150,50),core.Vector2.New(0,200)},
    {core.Vector2.New(-150,50),core.Vector2.New(150,50),core.Vector2.New(0,200),core.Vector2.New(0,-100)},
}
local RewardTrans = {}


UICollectForm.config = {
    ID = uiid.UICollectForm,
    AssetName = 'UI/Resident/UI/UICollectForm'
}

function UICollectForm:OnInitView()
    UIView.OnInitView(self)
    local this=self.uiform
    self.Item =CS.DisplayUtil.GetChild(this.gameObject, "Item")
    self.LayoutGroup =CS.DisplayUtil.GetChild(this.gameObject, "Layout Group").transform
    self.CLAIM =CS.DisplayUtil.GetChild(this.gameObject, "CLAIM")
    self.CLAIMx2 =CS.DisplayUtil.GetChild(this.gameObject, "CLAIMx2")
    self.Title =CS.DisplayUtil.GetChild(this.gameObject, "Title")
end

local CLAIMCallback=nil;
function UICollectForm:SetData(_diamondsNum,_keysNum,_needX2,_CLAIMCallback,
                               _itemData)
    RewardTrans = {}
    if(_diamondsNum and tonumber(_diamondsNum) > 0)then
        local item = logic.cs.GameObject.Instantiate(self.Item,self.LayoutGroup,false)
        local Num = CS.DisplayUtil.GetChild(item, "Num"):GetComponent(typeof(logic.cs.Text))
        local Icon = CS.DisplayUtil.GetChild(item, "Icon"):GetComponent(typeof(logic.cs.Image))
        Num.text = "x".._diamondsNum;
        Icon.sprite = Cache.PropCache.SpriteData[1]
        table.insert(RewardTrans,item)
    end
    if(_keysNum and tonumber(_keysNum) > 0)then
        local item = logic.cs.GameObject.Instantiate(self.Item,self.LayoutGroup,false)
        local Num = CS.DisplayUtil.GetChild(item, "Num"):GetComponent(typeof(logic.cs.Text))
        local Icon = CS.DisplayUtil.GetChild(item, "Icon"):GetComponent(typeof(logic.cs.Image))
        Num.text = "x".._keysNum;
        Icon.sprite = Cache.PropCache.SpriteData[2]
        table.insert(RewardTrans,item)
    end
    if _itemData then
        for k, v in pairs(_itemData) do
            local item = logic.cs.GameObject.Instantiate(self.Item,self.LayoutGroup,false)
            local Num = CS.DisplayUtil.GetChild(item, "Num"):GetComponent(typeof(logic.cs.Text))
            local Icon = CS.DisplayUtil.GetChild(item, "Icon"):GetComponent(typeof(logic.cs.Image))
            Num.text = "x".. v.num;
            if 1000<tonumber(v.id) and tonumber(v.id)<10000 then
                Icon.sprite = Cache.PropCache.SpriteData[3]
            else
                Icon.sprite = Cache.PropCache.SpriteData[v.id]
            end
            item:SetActive(true)
            table.insert(RewardTrans,item)
        end
    end
    self.CLAIMx2.gameObject:SetActiveEx(_needX2);
    CLAIMCallback = _CLAIMCallback
    self:PlayAnim()
end

function UICollectForm:PlayAnim()
    for i = 1, #RewardTrans do
        RewardTrans[i]:SetActiveEx(true)
        --RewardTrans[i].transform:DOAnchorPos(PosList[#RewardTrans][i],1):SetEase(core.tween.Ease.Flash):OnComplete(function()  end)
    end
    --coroutine.start(function()
    --    coroutine.wait(0.5) 
    --    self.Title.transform:DORotate( core.Vector3(0,0,0),1)
        logic.cs.UIEventListener.AddOnClickListener(self.CLAIM,function(data) self:CLAIMOnClick() end)
    --end)
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
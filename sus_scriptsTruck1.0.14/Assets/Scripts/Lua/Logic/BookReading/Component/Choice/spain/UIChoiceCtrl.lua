local Class = core.Class
local UIChoiceCtrl = Class('UIChoiceCtrl')

local Item = Class("UIChoiceCtrl.Item")
Item.__init = function(this,root,selectRoot,unSelectRoot)

    this.UpdateUI = function(this)
        if this.isOn then
            this.trans:SetParent(this.selectRoot,false)
        else
            this.trans:SetParent(this.unSelectRoot,false)
        end
    end

    this.Select = function(this,isOn)
        if this.isOn == isOn then
            return
        end
        this.isOn = isOn
        this:UpdateUI()
    end

    this.SetData = function(this,idx,pos,sprite)
        this.index = idx
        this.trans.anchoredPosition = pos
        this.imgRole.sprite = sprite
    end

    this.trans = root
    this.gameObject = root.gameObject
    this.selectRoot = selectRoot
    this.unSelectRoot = unSelectRoot
    this.imgRole = this.trans:Find("imgRole"):GetComponent(typeof(logic.cs.Image))
    this.isOn = false
    this:UpdateUI()
end

function UIChoiceCtrl:__init(ui)
    self.itemPosArr = {}
    self.itemPosArr[0] = {
        core.Vector2.New(0,0)
    }
    --2个
    self.itemPosArr[1] = {
        core.Vector2.New(-170,280),
        core.Vector2.New(145,0)
    }
    --3个
    self.itemPosArr[2] = {
        core.Vector2.New(-180,100),
        core.Vector2.New(135,355),
        core.Vector2.New(160,-90),
    }
    --4个
    self.itemPosArr[3] = {
        core.Vector2.New(-165,380),
        core.Vector2.New(180,250),
        core.Vector2.New(-200,-45),
        core.Vector2.New(155,-175),
    }
    self.itemList = {}

    self.gameObject = ui.gameObject
    self.transform = ui.transform
    self.pfbItem = ui.pfbItem
    self.selectRoot = ui.selectRoot
    self.unSelectRoot = ui.unSelectRoot
    self.btnConfirm1 = ui.btnConfirm1
    self.btnConfirm2 = ui.btnConfirm2
    self.txtConfirmCost2 = ui.txtConfirmCost2
    self.imgConfirmCost2 = ui.imgConfirmCost2

    self.ConfirmMask=ui.ConfirmMask

    self.pfbItem:SetActiveEx(false)
    self.btnConfirm1.onClick:RemoveAllListeners()
    self.btnConfirm2.onClick:RemoveAllListeners()
    self.btnConfirm1.onClick:AddListener(function()
        self:OnConfirmClick()
    end)
    self.btnConfirm2.onClick:AddListener(function()
        self:OnConfirmClick()
    end)
end

---@param choiceImpl BaseChoice
function UIChoiceCtrl:show(choiceImpl)
  
    self.choiceImpl = choiceImpl
    self.gameObject:SetActiveEx(true)
    self.ConfirmMask.gameObject:SetActiveEx(false)

    self:setChangeClothesDetails(choiceImpl.cfg)
    self:OnChoiceEvent(1)
end

function UIChoiceCtrl:hide()
    self.gameObject:SetActiveEx(false)
end

function UIChoiceCtrl:setChangeClothesDetails(cfg)
    self.mMaxNum = cfg.selection_num
    for i = #self.itemList+1, self.mMaxNum do
        self:createItem(i)
    end
    local imgs = self.choiceImpl:GetItems()
    for i=1,#self.itemList do
        local isOn = (i<=self.mMaxNum)
        local item = self.itemList[i]
        item.gameObject:SetActiveEx(isOn)
        if isOn then
            item:SetData(i,self.itemPosArr[self.mMaxNum - 1][i],imgs[i])
        end
    end
end

function UIChoiceCtrl:createItem(idx)
    local go = logic.cs.GameObject.Instantiate(self.pfbItem, self.unSelectRoot);
    local trans = go.transform
    local item = Item.New(trans, self.selectRoot, self.unSelectRoot)
    table.insert(self.itemList,item)
    local btn = go:GetComponent(typeof(logic.cs.Button))
    btn.onClick:AddListener(function()
        logic.cs.AudioManager:PlayTones(logic.bookReadingMgr.Res.AudioTones.dialog_choice_click)
        self:OnChoiceEvent(item.index)
    end)
end

function UIChoiceCtrl:OnChoiceEvent(idx)
    logic.bookReadingMgr.view:ResetOperationTips()
    self.mIndex = idx
    if self.mIndex > self.mMaxNum then
        self.mIndex = 1
    end
    
    for i=1,#self.itemList do
        local isOn = (i==self.mIndex)
        local item = self.itemList[i]
        item:Select(isOn)
    end
    self.choiceImpl:OnChoiceIndex(idx)
end

function UIChoiceCtrl:OnConfirmClick()

    --self.ConfirmMask.gameObject:SetActiveEx(true)

    logic.bookReadingMgr.view:ResetOperationTips()
    self.choiceImpl:OnConfirm(self)
end

function UIChoiceCtrl:SetComfirmType(type)
    self.btnConfirm1.gameObject:SetActiveEx(type == 1)
    self.btnConfirm2.gameObject:SetActiveEx(type == 2)
end

return UIChoiceCtrl
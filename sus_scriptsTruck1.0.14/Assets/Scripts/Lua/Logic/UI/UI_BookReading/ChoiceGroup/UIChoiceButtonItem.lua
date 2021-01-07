local Class = core.Class
---@class UIChoiceButtonItem
local UIChoiceButtonItem = Class("UIChoiceButtonItem")

function UIChoiceButtonItem:__init(gameObject)
    self.transform = gameObject.transform
    self.gameObject = gameObject

    self.imgBg = self.gameObject:GetComponent(typeof(logic.cs.Image))
    self.button = self.gameObject:GetComponent(typeof(logic.cs.UITweenButton))

    local get = logic.cs.LuaHelper.GetComponent
    self.goDiamond = self.transform:Find('DiamondIcon').gameObject
    self.txtCost = get(self.transform,'DiamondIcon/DiamondNumText',typeof(logic.cs.Text))
    self.txtInfo = get(self.transform,'InfoTxt',typeof(logic.cs.Text))
    self.fxSelect = self.transform:Find('SelectEffect').gameObject

    --道具相关
    self.btnKeyProp = get(self.transform,'DiamondIcon/btnKeyProp',typeof(logic.cs.Button))
    self.objBtnKeyProp = self.btnKeyProp.gameObject
    self.textKeyProp = get(self.transform,'DiamondIcon/btnKeyProp/Text',typeof(logic.cs.Text))
    self.transItemParentProp = self.transform:Find('DiamondIcon/btnKeyProp/list')
    self.objItemParentProp = self.transItemParentProp.gameObject
    self.itemPrefabProp = self.transform:Find('DiamondIcon/btnKeyProp/item').gameObject
    self.btnKeyProp.onClick:AddListener(function()
        self:ShowPropList()
    end)
    self.objBtnKeyProp:SetActive(false)
    self.itemPrefabProp:SetActive(false)    
	self.textKeyProp.gameObject:SetActive(false)
    self.itemPropList = {}
    self.luckItemProp = nil

end

function UIChoiceButtonItem:__delete()
end

function UIChoiceButtonItem:SetActive(isActive)
    self.transform.localScale = core.Vector3.New(1,1,1)
    self.gameObject:SetActiveEx(isActive)
end

function UIChoiceButtonItem:init(index,callback)
    self.button.onClick:AddListener(callback)
end

function UIChoiceButtonItem:initData(index,selection,cost,hiddenEgg,dialogID)
    if not selection then
        selection = ''
    end
    if not cost then
        cost = 0
    end
    if not hiddenEgg then
        hiddenEgg = 0
    end
    self.index = index
    self.selection = selection
    self.cost = cost
    self.hiddenEgg = hiddenEgg
    self.textKeyProp.text = tostring(self.cost)
    self.txtInfo.text = logic.bookReadingMgr:ReplaceChar(selection)
    self.isHadBuy = false

    local GetHadBuySelectId=logic.cs.UserDataManager:GetHadBuySelectId(dialogID);
    print("lua====dialogID:"..dialogID)  

    if GetHadBuySelectId~=nil then

        if cost > 0 then
            --需要花钱购买的

            self.goDiamond:SetActiveEx(true)
            self.fxSelect:SetActiveEx(true)
            --self.imgBg.sprite = logic.bookReadingMgr.Res:GetSprite("Atlas/Choice/bg_chat_choice2",true)
            self.imgBg.sprite = CS.ResourceManager.Instance:GetUISprite("BookReadingForm/bg_chat_choice2")

            self.txtCost.text = tostring(cost)

            for k,v in pairs(GetHadBuySelectId) do
                print("lua==已经买的选项值："..v.."--index:"..index.."--长度：")   
                if v==index then
                    -- 这个选项已经买过了
                    self.isHadBuy = true
                    self.goDiamond:SetActiveEx(false)
                    self.fxSelect:SetActiveEx(false)
                    --self.imgBg.sprite = logic.bookReadingMgr.Res:GetSprite("Atlas/Choice/bg_chat_choice",true)
                    self.imgBg.sprite=CS.ResourceManager.Instance:GetUISprite("BookReadingForm/bg_chat_choice")
                end
            end

            if not self.isHadBuy then
                self:ShowPropBtn()
            end
        else
            --不需要花钱购买的
            self.goDiamond:SetActiveEx(false)
            self.fxSelect:SetActiveEx(false)
            --self.imgBg.sprite = logic.bookReadingMgr.Res:GetSprite("Atlas/Choice/bg_chat_choice",true)
            self.imgBg.sprite=CS.ResourceManager.Instance:GetUISprite("BookReadingForm/bg_chat_choice")
        end
    else
        if cost > 0 then
            self.goDiamond:SetActiveEx(true)
            self.fxSelect:SetActiveEx(true)
            --self.imgBg.sprite = logic.bookReadingMgr.Res:GetSprite("Atlas/Choice/bg_chat_choice2",true)
            self.imgBg.sprite=CS.ResourceManager.Instance:GetUISprite("BookReadingForm/bg_chat_choice2")
            self.txtCost.text = tostring(cost)
            self:ShowPropBtn()
        else
            self.goDiamond:SetActiveEx(false)
            self.fxSelect:SetActiveEx(false)
            --self.imgBg.sprite = logic.bookReadingMgr.Res:GetSprite("Atlas/Choice/bg_chat_choice",true)
            self.imgBg.sprite=CS.ResourceManager.Instance:GetUISprite("BookReadingForm/bg_chat_choice")
        end
    end



    --[[
 for i = 1,#GetHadBuySelectId do
        print("GetHadBuySelectId:"..GetHadBuySelectId[i])

        if GetHadBuySelectId[i]==index then
            self.goDiamond:SetActiveEx(true)
            self.fxSelect:SetActiveEx(true)
            self.imgBg.sprite = logic.bookReadingMgr.Res:GetSprite("Atlas/Choice/bg_chat_choice2",true)
            self.txtCost.text = tostring(cost)
        else
            self.goDiamond:SetActiveEx(false)
            self.fxSelect:SetActiveEx(false)
            self.imgBg.sprite = logic.bookReadingMgr.Res:GetSprite("Atlas/Choice/bg_chat_choice",true)
        end
    end 

    --]]
   
--[[
if cost > 0 then
        self.goDiamond:SetActiveEx(true)
        self.fxSelect:SetActiveEx(true)
        self.imgBg.sprite = logic.bookReadingMgr.Res:GetSprite("Atlas/Choice/bg_chat_choice2",true)
        self.txtCost.text = tostring(cost)
    else
        self.goDiamond:SetActiveEx(false)
        self.fxSelect:SetActiveEx(false)
        self.imgBg.sprite = logic.bookReadingMgr.Res:GetSprite("Atlas/Choice/bg_chat_choice",true)
    end
--]]
    
end

function UIChoiceButtonItem:ShowPropBtn()
    local userPropInfo = logic.cs.UserDataManager.userPropInfo_Choice
    if not userPropInfo or not userPropInfo.discount_list or userPropInfo.discount_list.Count<=0 then
        self.objBtnKeyProp:SetActive(false)
        return
    end
    self.objBtnKeyProp:SetActive(true)
    local discount_list = userPropInfo.discount_list
    self.transItemParentProp:ClearAllChild()
    self.itemPropList = {}
    for i=0,discount_list.Count-1,1 do
        self:AddItemProp(discount_list[i])
    end
    local lastItem = self:AddItemProp(nil)
    lastItem:fucOnClick()
end
function UIChoiceButtonItem:ShowPropList()
    local isShow = not self.objItemParentProp.activeSelf
    self.objItemParentProp.gameObject:SetActive(isShow)
    self.textKeyProp.gameObject:SetActive(isShow)
end
function UIChoiceButtonItem:AddItemProp(data)
    local go = logic.cs.GameObject.Instantiate(self.itemPrefabProp,self.transItemParentProp,false)
    go:SetActive(true)
    local trans = go.transform
    local button = logic.cs.LuaHelper.GetComponent(trans,'Button',typeof(logic.cs.Button))
    local objCheck = trans:Find('checked').gameObject
    local txtNum = logic.cs.LuaHelper.GetComponent(trans,'Text',typeof(logic.cs.Text))
    if data~= nil then
        txtNum.text = data.discount_string or "No Discount"
    else
        txtNum.text = "No Discount"
    end
    local fucShowCheck = function(_self,isShow)
        _self.objCheck:SetActive(isShow)
    end
    local fucOnClick = function(_self)
		if self.luckItemProp ~= _self then
			self:OnClcikPropItem(_self)
		end
    end
    local item ={
        data = data,
        gameObject = go,
        transform = trans,
		button = button,
        objCheck = objCheck,
        txtNum = txtNum,
        fucShowCheck = fucShowCheck,
        fucOnClick = fucOnClick
    }
    item.button.onClick:AddListener(function()
        item:fucOnClick()
    end)
    table.insert(self.itemPropList, item)
    return item
end
function UIChoiceButtonItem:OnClcikPropItem(propItem)
	for i,v in ipairs(self.itemPropList) do
		v:fucShowCheck(v==propItem)
	end
	self.luckItemProp = propItem
	local isUser = true
	if propItem.data==nil then
		isUser = false
	end
    logic.cs.UserDataManager:SetLuckyPropItem(isUser, propItem.data)

    --refresh ui
    self.textKeyProp.gameObject:SetActive(isUser)
    if isUser then
        self.textKeyProp.text = tostring(self.cost)
        local newCost = tonumber(self.cost) - tonumber(self.cost)*tonumber(propItem.data.discount)
        newCost = math.floor(newCost)
        self.txtCost.text = tostring(newCost)
    else
        self.txtCost.text = tostring(self.cost)
    end
end

return UIChoiceButtonItem
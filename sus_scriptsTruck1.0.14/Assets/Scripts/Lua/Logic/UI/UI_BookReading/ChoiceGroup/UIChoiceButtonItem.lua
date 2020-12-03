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
    self.txtInfo.text = logic.bookReadingMgr:ReplaceChar(selection)
    self.isHadBuy = false

    local GetHadBuySelectId=logic.cs.UserDataManager:GetHadBuySelectId(dialogID);
    print("lua====dialogID:"..dialogID)  
    
    if GetHadBuySelectId~=nil then

        if cost > 0 then
            --需要花钱购买的

            self.goDiamond:SetActiveEx(true)
            self.fxSelect:SetActiveEx(true)
            self.imgBg.sprite = logic.bookReadingMgr.Res:GetCommonSprite("bg_chat_choice2")
            --self.imgBg.sprite = CS.ResourceManager.Instance:GetUISprite("BookReadingForm/bg_chat_choice2")
            
            self.txtCost.text = tostring(cost)

            for k,v in pairs(GetHadBuySelectId) do
                print("lua==已经买的选项值："..v.."--index:"..index.."--长度：")   
                if v==index then
                    -- 这个选项已经买过了
                    self.isHadBuy = true
                    self.goDiamond:SetActiveEx(false)
                    self.fxSelect:SetActiveEx(false)
                    self.imgBg.sprite = logic.bookReadingMgr.Res:GetCommonSprite("bg_chat_choice")
                    --self.imgBg.sprite=CS.ResourceManager.Instance:GetUISprite("BookReadingForm/bg_chat_choice")
                end              
            end          
        else
            --不需要花钱购买的
            self.goDiamond:SetActiveEx(false)
            self.fxSelect:SetActiveEx(false)
            self.imgBg.sprite = logic.bookReadingMgr.Res:GetCommonSprite("bg_chat_choice")
            --self.imgBg.sprite=CS.ResourceManager.Instance:GetUISprite("BookReadingForm/bg_chat_choice")
        end
    else
        if cost > 0 then
            self.goDiamond:SetActiveEx(true)
            self.fxSelect:SetActiveEx(true)
            self.imgBg.sprite = logic.bookReadingMgr.Res:GetCommonSprite("bg_chat_choice2")
            --self.imgBg.sprite=CS.ResourceManager.Instance:GetUISprite("BookReadingForm/bg_chat_choice2")
            self.txtCost.text = tostring(cost)
        else
            self.goDiamond:SetActiveEx(false)
            self.fxSelect:SetActiveEx(false)
            self.imgBg.sprite = logic.bookReadingMgr.Res:GetCommonSprite("bg_chat_choice")
            --self.imgBg.sprite=CS.ResourceManager.Instance:GetUISprite("BookReadingForm/bg_chat_choice")
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

return UIChoiceButtonItem
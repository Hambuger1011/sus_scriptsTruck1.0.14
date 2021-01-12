local Class = core.Class
local UIChoiceButtonGroup = Class("UIChoiceButtonGroup")
local UIChoiceButtonItem = require('Logic/UI/UI_BookReading/ChoiceGroup/UIChoiceButtonItem')

function UIChoiceButtonGroup:__init(root)
    self.transform = root
    self.gameObject = root.gameObject
    self.group = root:Find('group')
    self.pfbItem = self.group:Find('item').gameObject
    self.pfbItem:SetActiveEx(false)
    self.canvasGroup = self.group:GetComponent(typeof(logic.cs.CanvasGroup))
    self.gridLayout = self.group:GetComponent(typeof(logic.cs.GridLayoutGroup))
    self.spaceing = core.Vector2.New(0,3)
    self.cellSize = core.Vector2.New(590, 98)
    self.items = {}
    self.selectIdx = 1
    self.gameObject:SetActiveEx(false)
end

function UIChoiceButtonGroup:__delete()
end

---@param component BaseComponent
---@param vSideType number (0:Left, 1:Right,2:Middle, )
function UIChoiceButtonGroup:choicesDialogInit(component,vSideType)
    self.canvasGroup.alpha = 0
    self.component = component
    self.maxNum = component.cfg.selection_num
    self.gridLayout.spacing = core.Vector2(self.spaceing.x,-100)
    self:initItems(
        {
            component.cfg.selection_1,
            component.cfg.selection_2,
            component.cfg.selection_3,
            component.cfg.selection_4
        },{
            component.cfg.requirement1,
            component.cfg.requirement2,
            component.cfg.requirement3,
            component.cfg.requirement4
        },{
            component.cfg.hidden_egg1,
            component.cfg.hidden_egg2,
            component.cfg.hidden_egg3,
            component.cfg.hidden_egg4
        }
        )

        local yNum = -275
        local xNum = 50
        if vSideType == 0 then
            self.group.anchoredPosition = Vector2.New(-1 * xNum, yNum)
        elseif vSideType == 1 then
            self.group.anchoredPosition = Vector2.New(xNum, yNum)
        else
            self.group.anchoredPosition = Vector2.New(0, yNum)
        end
end


function UIChoiceButtonGroup:initItems(selections,cost,hiddenEgg)
    self:CheckNeedPay(cost)
    local bookData = logic.bookReadingMgr.bookData
    for i=#self.items,self.maxNum do
        table.insert(self.items,self:createItem(i))
    end
    for i=1,self.maxNum do
        ---@type UIChoiceButtonItem
        local item = self.items[i]
        item:SetActive(true)
        item:initData(i,selections[i],cost[i],hiddenEgg[i], self.component.cfg.dialogID,self.items)
    end
    for i=self.maxNum + 1,#self.items do
        ---@type UIChoiceButtonItem
        local item = self.items[i]
        item:SetActive(false)
    end
end

function UIChoiceButtonGroup:createItem(index)
    local go = logic.cs.GameObject.Instantiate(self.pfbItem,self.group,false)
    local item = UIChoiceButtonItem.New(go)
    item:init(index,function()
        self:onItemClick(item)
    end)
    return item
end

function UIChoiceButtonGroup:show()
    self.gameObject:SetActiveEx(true)
	logic.bookReadingMgr.view:SetBottomActive(false)
    local duration = 0.6
    core.tween.DoTween.To(function()
            return 0
        end,function(alpha)
            self.canvasGroup.alpha = alpha
        end,1,duration*0.8):SetEase(core.tween.Ease.Flash):SetId(self)


    core.tween.DoTween.To(function()
            return -100
        end,function(y)
            self.gridLayout.spacing = core.Vector2(self.spaceing.x,y)
        end,self.spaceing.y,duration):SetEase(core.tween.Ease.Flash):SetId(self)
end

function UIChoiceButtonGroup:hide()
    core.tween.DoTween.Kill(self)
    self.gameObject:SetActiveEx(false)
	logic.bookReadingMgr.view:SetBottomActive(true)
end

function UIChoiceButtonGroup:CheckNeedPay(cost)
    local bookData = logic.bookReadingMgr.bookData
    self.mNeedPay = false
    for i = 1,self.component.cfg.selection_num do
        if cost[i] ~= 0 then
            if logic.cs.UserDataManager:CheckDialogOptionHadCost(bookData.BookID,self.component.cfg.dialogID,i) then
                cost[i] = 0
            end
            if logic.cs.UserDataManager:CheckBookHasBuy(bookData.BookID) then
                cost[i] = 0
            end
        end
        if cost[i] ~= 0 then
            self.mNeedPay = true
        end
    end
    if self.mNeedPay then
        logic.cs.EventDispatcher.Dispatch(logic.cs.EventEnum.ResidentMoneyInfo, 1)
    end
end

---@param item UIChoiceButtonItem
function UIChoiceButtonGroup:onItemClick(item)
    local bookData = logic.bookReadingMgr.bookData
    logic.bookReadingMgr.view:ResetOperationTips()
    self.selectIdx = item.index
    logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.SelectOption,"","",tostring(bookData.BookID),tostring(self.component.cfg.dialogID),tostring(self.selectIdx))
    if logic.config.channel == Channel.Spain then
        ---@type UIValueChoice
        local uiValueChoice = logic.UIMgr:Open(logic.uiid.ValueChoice)
        uiValueChoice:SetData(1,"Buy-Choice",item.cost, function(buyType)
            --logic.cs.UINetLoadingMgr:Show()
            logic.gameHttp:BookDialogOptionCost(
                bookData.BookID,
                bookData.ChapterID,
                self.component.cfg.dialogID,
                item.index,
                buyType,
                function(result)
                    logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.RewardWin)
                    self:BookDialogOptionCallBack(result)
                end
            )
        end)
    else
        logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.RewardWin)
        --logic.cs.UINetLoadingMgr:Show()

        logic.bookReadingMgr:SaveOption(item.index)
        logic.bookReadingMgr:SaveProgress(function(result)
            self:BookDialogOptionCallBack(result)
        end)
    end
    
end

function UIChoiceButtonGroup:BookDialogOptionCallBack(result)
    local bookData = logic.bookReadingMgr.bookData
    logic.debug.Log("----SendPlayerProgress---->" .. result)
    --local json = core.json.Derialize(result)
    --local code = tonumber(json.code)
    --local msg=tonumber(json.msg)
    --logic.cs.UINetLoadingMgr:Close()

    local JsonObject=CS.JsonObject
    JsonObject=CS.JsonHelper.JsonToJObject(result);  
    local code =JsonObject.code
    local msg=JsonObject.msg

    if code == 200 then
        --logic.cs.UserDataManager:SetUserOptionCostResultInfo(result)
        if logic.cs.UserDataManager.userOptionCostResultInfo ~= nil then
            local purchase = logic.cs.UserDataManager.UserData.DiamondNum - logic.cs.UserDataManager.userOptionCostResultInfo.data.diamond
            if purchase > 0 then
                logic.cs.TalkingDataManager:OnPurchase("Choices Options cost diamond", purchase, 1)
            end
        end
        logic.cs.UserDataManager:OptionCostResultMoneyReset()
        self:TurnToOption(code,msg)
    elseif code == 204 then --本章节未购买
        self:TurnToOption(code,msg)
    -- elseif code == 201 then --一般错误，超出章节
    --     self:TurnToOption(code,msg)
    -- elseif code == 202 then --砖石不够
    --     --self:TurnToOption(code,msg)
    -- elseif code == 203 then --钥匙不够
    --     --self:TurnToOption(code,msg)
    elseif code == 202 or code == 203 then
        logic.debug.LogError("--BookDialogOptionCallBack--扣费失败,BookId:" .. bookData.BookID .. " DialogId:" .. self.component.cfg.dialogID);
        
        local item = self.items[self.selectIdx]
        logic.bookReadingMgr.view:ShowChargeTips(item.cost)
    else
        self:TurnToOption(code,msg)
    end
end


function UIChoiceButtonGroup:TurnToOption(code,msg)
    local bookData = logic.bookReadingMgr.bookData
    ----logic.cs.UINetLoadingMgr:Show()
    
    print("code:"..code.."=msg:"..msg)
    if code == 200 then
        self:hide()
        self:CheckAddPersonalist(self.component)
        self.component:ShowNextDialog()
        local item = self.items[self.selectIdx]
        if item.cost > 0 then
            logic.cs.UserDataManager:AddDialogOptionHadCost(bookData.BookID,self.component.cfg.dialogID,self.selectIdx)
        end
        logic.cs.UserDataManager:RecordBookOptionSelect(bookData.BookID,self.component.cfg.dialogID,self.selectIdx)

        self:HandleHiddentEgg(item)
        if self.mNeedPay then
            logic.cs.EventDispatcher.Dispatch(logic.cs.EventEnum.ResidentMoneyInfo, 0)
        end

        --【AF事件记录*发送 选择1次钻石选项】
        CS.AppsFlyerManager.Instance:CHOOSE_DIAMOND_SELECTION();
    else
        if (not string.IsNullOrEmpty(msg)) then
            logic.cs.UITipsMgr:PopupTips(msg, false)
        end
    end

end


---@param item UIChoiceButtonItem
function UIChoiceButtonGroup:HandleHiddentEgg(item)
    local bookData = logic.bookReadingMgr.bookData
    
--[[
     if item.hiddenEgg > 0 then
        logic.cs.GameHttpNet:BookGetHiddenEgg(bookData.BookID,bookData.ChapterID,self.component.cfg.dialogID,self.selectIdx, function(arg)
            local result = tostring(arg)
            logic.debug.Log("----BookDialogOptionCallBack---->" .. result)
            coroutine.start(function()
                local json = core.json.Derialize(result)
                local code = tonumber(json.code)
                if code == 200 then
                    logic.cs.UserDataManager:SetHiddenEggInfo(result)
                    if logic.cs.UserDataManager.hiddenEggInfo ~= nil then
                        self:DoGetHiddenEgg(logic.cs.UserDataManager.hiddenEggInfo.data.bkey, logic.cs.UserDataManager.hiddenEggInfo.data.diamond)
                    end
                elseif code == 203 then
                    logic.debug.LogError("--GetHiddenEggCallBack--此对话没有彩蛋 DialogId" .. self.component.cfg.dialogID)
                end
            end)
        end)
    end
--]]

   
end

function UIChoiceButtonGroup:DoGetHiddenEgg(vKeyNum,vDiamondNum)
    local mHiddenEggStartPos = logic.cs.Camera.main:WorldToScreenPoint(logic.cs.Input.mousePosition)
    local targetPos = core.Vector3.New(306, 625)
    local rewardShowData = CS.RewardShowData()
    rewardShowData.StartPos = mHiddenEggStartPos
    rewardShowData.TargetPos = targetPos
    rewardShowData.KeyNum = vKeyNum
    rewardShowData.DiamondNum = vDiamondNum
    logic.cs.EventDispatcher.Dispatch(logic.cs.EventEnum.HiddenEggRewardShow, rewardShowData)
end


---@param component BaseComponent
function UIChoiceButtonGroup:CheckAddPersonalist(component)
    local bookData = logic.bookReadingMgr.bookData
    local personalistStr = component.cfg['Personalit_'..self.selectIdx]
    if string.IsNullOrEmpty(personalistStr) then
        return
    end
    personalistStr = string.gsub(personalistStr,'##','_')
    local infoList = string.split(personalistStr, '_')
    for i = 1, #infoList do
        local str = infoList[i]
        if string.IsNullOrEmpty(str) then
            goto continue
        end
        local keyValue = string.split(str,':')
        if #keyValue < 2 then
            goto continue
        end
        local key = tonumber(keyValue[1])
        local value = tonumber(keyValue[2])
        local resultValue = 0
        local msg = string.Empty

        local arr = {
            {ratio = 0.34, key = key, color = '#41b1ff'},
            {ratio = 0.21, key = key, color = '#54e778'},
            {ratio = 0.05, key = key, color = '#f75acd'},
            {ratio = 0.25, key = key, color = '#b555ff'},
            {ratio = 0.15, key = key, color = '#f78686'},
        }
        if key > 5 then
            logic.cs.UserDataManager:SetBookPropertyValue(bookData.BookID,key,value)
        else
            local data = arr[key]
            resultValue = core.Mathf.Round(value / data.ratio)
            msg = logic.cs.GameDataMgr.table:GetPersonalityTxtById(data.key) .. " <color=" .. data.color .. ">+" .. resultValue .. "</color>"
        end
        if not string.IsNullOrEmpty(msg) then
            --logic.cs.UITipsMgr:ShowPopTips(msg, logic.cs.Input.mousePosition)
            if (logic.cs.UserDataManager.profileData ~= nil and logic.cs.UserDataManager.profileData.data ~= nil and logic.cs.UserDataManager.profileData.data.info ~= nil) then-- 客户端自己统计性格选项的个数
                logic.cs.UserDataManager.profileData.data.info.option_num = logic.cs.UserDataManager.profileData.data.info.option_num + 1
            end
        end
        ::continue::
    end
end

return UIChoiceButtonGroup
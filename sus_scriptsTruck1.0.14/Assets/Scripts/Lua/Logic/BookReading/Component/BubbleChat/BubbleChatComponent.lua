local Class = core.Class
local Base = logic.BookReading.BaseComponent

local BubbleChatComponent = Class("BubbleChatComponent", Base)
local uiBubble = nil

--收集所用到的资源
function BubbleChatComponent:CollectRes(resTable)
	Base.CollectRes(self,resTable)
    resTable['Assets/Bundle/UI/BookReading/BubbleChat/Canvas_Bubble.prefab'] = BookResType.UIRes
    resTable['Assets/Bundle/UI/BookReading/BubbleChat/UIBubbleSelectionItem.prefab'] = BookResType.UIRes
    if self.cfg.dialog_type ~= logic.DialogType.BubbleChat and tonumber(self.cfg.role_id) > 0 then
        resTable[logic.bookReadingMgr.Res.bookCommonPath.."UI/PhoneCallHeadIcon/"..self.cfg.role_id..".png"] = BookResType.BookRes
    end
end


function BubbleChatComponent:Clean()
	Base.Clean(self)
end

function BubbleChatComponent:GetNextDialogID()
    local id = self.cfg.next
    if self.selectIdx then
        local idx = self.selectIdx - 1
        id = self:GetNextDialogIDBySelection(idx);
    else
        id = Base.GetNextDialogID(self)
    end
    return id
end

function BubbleChatComponent:OpenUI(callback)
    logic.bookReadingMgr.view:StopOperationTips()
    if IsNull(uiBubble) then
        local prefab = logic.bookReadingMgr.Res:GetPrefab("Assets/Bundle/UI/BookReading/BubbleChat/Canvas_Bubble.prefab")
        local gameObject = logic.cs.GameObject.Instantiate(prefab,logic.bookReadingMgr.view.transComponent,false)
        gameObject:SetActiveEx(true)
        uiBubble = gameObject:GetComponent(typeof(CS.BubbleForm))
        uiBubble:ShowOrHideView(true,callback)
    end
end

function BubbleChatComponent:CloseUI()
    logic.bookReadingMgr.view:ResetOperationTips()
    if not IsNull(uiBubble) then
        uiBubble:ShowOrHideView(false,function()
            logic.cs.GameObject.Destroy(uiBubble.gameObject)
            uiBubble = nil
            if logic.bookReadingMgr.playingComponent.cfg and logic.bookReadingMgr.playingComponent.cfg.selection_num == 0 then
                logic.bookReadingMgr:SaveProgress()
            end
        end)
    end
end


--- overrider
--- @param component BaseComponent @将要播放的component
function BubbleChatComponent:OnPlayEnd(nextComponent)
    logic.cs.EventDispatcher.RemoveMessageListener(logic.cs.EventEnum.DialogDisplaySystem_PlayerMakeChoice,self.callback)
    self.callback = nil
    --下一个对话不是气泡对话
    if nextComponent.__cname ~= self.__cname and nextComponent.cfg.dialog_type ~= logic.DialogType.BubbleChat then
        --self:CloseUI()
    end
end

function BubbleChatComponent:Play()
    self.callback = function(notification)
        logic.cs.EventDispatcher.RemoveMessageListener(logic.cs.EventEnum.DialogDisplaySystem_PlayerMakeChoice,self.callback)
        if logic.bookReadingMgr.playingComponent ~= self then
            logic.debug.LogError(self.__cname)
            return
        end
        if notification == nil then
            --logic.debug.LogError(self.cfg.dialogid)
        else
            self.selectIdx = tonumber(notification.Data) + 1
            logic.debug.LogError('选项index:'..self.selectIdx)
        end
        self:ShowNextDialog()
    end
    logic.cs.EventDispatcher.AddMessageListener(logic.cs.EventEnum.DialogDisplaySystem_PlayerMakeChoice, self.callback)

    self.isPhoneCallMode = true
    if self.cfg.dialog_type == logic.DialogType.BubbleChat then

        local nextComponent = logic.bookReadingMgr.components[self:GetNextDialogID()]
        if nextComponent ~= nil and nextComponent.cfg.phonecall ~= 2 then
            self.isPhoneCallMode = false
        else
            self.isPhoneCallMode = true
        end
        if self.isPhoneCallMode then
            self:OpenUI(function()
                logic.bookReadingMgr:SaveProgress()
                self:ShowNextDialog()
            end)
        else
            self:CloseUI()
        end
    else
        self:OpenUI()
        uiBubble:DialogNextStepByDialogID(self.cfg.dialogid)
    end
end

return BubbleChatComponent
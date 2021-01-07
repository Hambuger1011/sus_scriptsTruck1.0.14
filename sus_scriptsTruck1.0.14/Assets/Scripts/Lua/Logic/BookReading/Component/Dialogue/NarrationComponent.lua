--[[
    旁白
]]
local Class = core.Class
local Base = logic.BookReading.BaseDialogueComponent

local NarrationComponent = Class("NarrationComponent", Base)


--收集所用到的资源
function NarrationComponent:CollectRes(resTable)
    Base.CollectRes(self,resTable)
    resTable["Assets/Bundle/UI/BookReading/Dialog_Narration.prefab"] = BookResType.UIRes
    resTable[logic.bookReadingMgr.Res.bookCommonPath.."Atlas/Dialog/bg_chat.png"] = BookResType.BookRes
end

function NarrationComponent:Clean()
	Base.Clean(self)
    self.ui = nil
end

--- overrider
--- @param component BaseComponent @将要播放的component
function NarrationComponent:OnPlayEnd(nextComponent)
    if nextComponent.__cname ~= self.__cname
        or nextComponent.isPhoneCallMode ~= self.isPhoneCallMode 
    then
        self:dialogOutTween(self.ui.gameObject)
    end
end

function NarrationComponent:Play()
    Base.Play(self)
    --local isPhoneCallMode = logic.bookReadingMgr.bookData.IsPhoneCallMode
    if self.isPhoneCallMode then
        self.ui = logic.bookReadingMgr.UI:GetPhone_NarrationDialogue()
    else
        self.ui = logic.bookReadingMgr.UI:GetNarrationDialogue()
    end

    self.ui.Reset()
    
    self:ShowDetails(
        self.ui.gameObject,
        self.ui.DialogBox.transform,
        self.ui.DialogText,
        self.isPhoneCallMode
    )
end

---@param Dialogue logic.cs.GameObject
---@param DialogBox logic.cs.Transform
---@param DialogText logic.cs.Text
---@param isPhoneCallMode bool
function NarrationComponent:ShowDetails(
    Dialogue,
    DialogBox,
    DialogText,
    isPhoneCallMode
    )
    logic.cs.BookReadingWrapper.IsTextTween = true
    
    local showText = logic.bookReadingMgr:ReplaceChar(self.cfg.dialog)
    if not self:IsSameDialogType(self) then
        self:ResetDialogPos(Dialogue)
    end
    --移动背景
    logic.bookReadingMgr.view:sceneBGMove(logic.bookReadingMgr.view:GetPiexlX(self.cfg.Scenes_X),function()
        Dialogue:SetActiveEx(true)
        self.IsPlayTween = false
        --播放显示动画
        logic.bookReadingMgr.view:textDialogTween(self,DialogBox,DialogText,0,showText, function()
            if self.cfg.trigger == 1 then
                logic.bookReadingMgr.view.choiceGroup:choicesDialogInit(self,2)
                logic.bookReadingMgr.view.choiceGroup:show()
            else
                if logic.cs.GameDataMgr.InAutoPlay and self.cfg.dialogID ~= logic.cs.BookReadingWrapper.EndDialogID then
                    local autoPlayTimer
                    autoPlayTimer = core.Timer.New(function()
                        if logic.bookReadingMgr.playingComponent == self then
                            if logic.cs.GameDataMgr.InAutoPlay then
                                self:OnSceneClick(true)
                                autoPlayTimer:Stop()
                                autoPlayTimer = nil
                            end
                        else
                            if autoPlayTimer then
                                autoPlayTimer:Stop()
                            end
                            autoPlayTimer = nil
                        end
                    end,4 - tonumber(logic.cs.UserDataManager.UserData.AutoSpeed),-1)
                    autoPlayTimer:Start()
                end
                logic.cs.GameDataMgr.AutoPlayOpen = function()
                    self:OnSceneClick()
                    logic.cs.GameDataMgr.AutoPlayOpen = nil
                end
                logic.cs.GameDataMgr.AutoPlayClose = function()
                    --core.coroutine.step(nil,co)
                end
            end
            logic.cs.BookReadingWrapper.IsTextTween = false
        end)
        --播放tips
        logic.bookReadingMgr.view.tipsImage:ShowTips(self.cfg.tips);
        if not self:IsSameDialogType(self) then
            self:dialogEnterTween(Dialogue.transform)
        end
    end)
end



function NarrationComponent:ResetDialogPos(dialogBox)
    local t = dialogBox.transform
    local canvasGroup = self.ui.canvasGroup --logic.cs.LuaHelper.AddMissingComponent(dialogBox,typeof(logic.cs.CanvasGroup))

    t:DOKill()
    canvasGroup:DOKill()
    local duration = 0.18
    canvasGroup.alpha = 0
    canvasGroup:DOFade(1,duration)

    t.localScale = core.Vector3.zero
    t.anchoredPosition = core.Vector2.New(0,100)
end


function NarrationComponent:dialogEnterTween(dialogBox,callback)
    local t = dialogBox.transform
    local canvasGroup = self.ui.canvasGroup --logic.cs.LuaHelper.AddMissingComponent(dialogBox.gameObject,typeof(logic.cs.CanvasGroup))

    t:DOKill()
    canvasGroup:DOKill()
    local duration = 0.18
    canvasGroup.alpha = 0
    canvasGroup:DOFade(1,duration)

    t.localScale = core.Vector3.one
    t:DOAnchorPos(core.Vector2.New(0,100),duration):SetEase(core.tween.Ease.Flash):OnComplete(callback)
end


function NarrationComponent:dialogOutTween(gameObject)
    local t = gameObject.transform
    local canvasGroup = self.ui.canvasGroup --logic.cs.LuaHelper.AddMissingComponent(gameObject,typeof(logic.cs.CanvasGroup))

    t:DOKill()
    canvasGroup:DOKill()
    local duration = 0.2
    canvasGroup.alpha = 1
    canvasGroup:DOFade(0,duration)

    t.localScale = core.Vector3.one
    t:DOAnchorPos(core.Vector2.New(0,100),duration):SetEase(core.tween.Ease.Flash):OnComplete(function()
        self.ui.gameObject:SetActiveEx(false)
    end)
end

return NarrationComponent
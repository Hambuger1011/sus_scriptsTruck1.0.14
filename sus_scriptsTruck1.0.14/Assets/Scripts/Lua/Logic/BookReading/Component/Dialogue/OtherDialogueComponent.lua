--[[
    其它人对白
]]
local Class = core.Class
local Base = logic.BookReading.BaseDialogueComponent

local OtherDialogueComponent = Class("OtherDialogueComponent", Base)

function OtherDialogueComponent:__init(index,cfg)
    Base.__init(self,index,cfg)
    self.isNpc = true
end

--收集所用到的资源
function OtherDialogueComponent:CollectRes(resTable)
	Base.CollectRes(self,resTable)
    resTable["Assets/Bundle/UI/BookReading/Dialog_OthersDialogue.prefab"] = BookResType.UIRes
    resTable[logic.bookReadingMgr.Res.bookCommonPath.."Atlas/Dialog/bg_chat_right.png"] = BookResType.BookRes
    resTable[logic.bookReadingMgr.Res.bookCommonPath.."Atlas/Dialog/bg_chat_2.png"] = BookResType.BookRes
    resTable[logic.bookReadingMgr.Res.bookCommonPath.."Atlas/Dialog/bg_think.png"] = BookResType.BookRes
end

function OtherDialogueComponent:Clean()
	Base.Clean(self)
	self.ui = nil
end


--- overrider
--- @param component BaseComponent @将要播放的component
function OtherDialogueComponent:OnPlayEnd(nextComponent)
    if nextComponent.__cname ~= self.__cname
        or nextComponent.isPhoneCallMode ~= self.isPhoneCallMode 
    then
        self:dialogOutTween(self.ui.gameObject)
    end
end

function OtherDialogueComponent:Play()
    Base.Play(self)
    --logic.bookReadingMgr.bookData.IsPhoneCallMode = true
    --local isPhoneCallMode = logic.bookReadingMgr.bookData.IsPhoneCallMode
    if self.isPhoneCallMode then
        self.ui = logic.bookReadingMgr.UI:GetPhone_OtherDialogue()
    else
        self.ui = logic.bookReadingMgr.UI:GetOtherDialogue()
        if self.type == 0 then
            --self.ui.imgBgDot.sprite = logic.bookReadingMgr.Res:GetSprite("Atlas/Dialog/bg_chat_2",true)
            self.ui.imgBgDot.sprite = CS.ResourceManager.Instance:GetUISprite("BookReadingForm/bg_chat_2");
        else
            --self.ui.imgBgDot.sprite = logic.bookReadingMgr.Res:GetSprite("Atlas/Dialog/bg_think",true)
            self.ui.imgBgDot.sprite = CS.ResourceManager.Instance:GetUISprite("BookReadingForm/bg_think");
        end
        
        local t = self.ui.DialogBoxContent.transform
        local bookDetial = logic.cs.JsonDTManager:GetJDTBookDetailInfo(logic.bookReadingMgr.bookID)
        local pos = t.anchoredPosition
        if(bookDetial.rolescale == 2) then
            pos.y = -2
        else
            pos.y = -0
        end
        t.anchoredPosition = pos
    end
    self.ui.Reset()
    self:ShowDetails(
        self.ui.gameObject,
        self.ui.DialogText,
        self.ui.FaceExpr,
        self.ui.PlayerName,
        self.ui.DialogBox.transform,
        self.ui.DialogBoxContent,
        self.isPhoneCallMode,
        1,
            nil
        )
end

function OtherDialogueComponent:ResetDialogPos(dialogBox)
    local t = dialogBox.transform
    local canvasGroup = self.ui.canvasGroup --logic.cs.LuaHelper.AddMissingComponent(dialogBox,typeof(logic.cs.CanvasGroup))

    t:DOKill()
    canvasGroup:DOKill()
    local duration = 0.18
    canvasGroup.alpha = 0
    canvasGroup:DOFade(1,duration)

    t.localScale = core.Vector3.zero
    t.anchoredPosition = core.Vector2.New(200,-150)
end


function OtherDialogueComponent:dialogEnterTween(dialogBox,callback)
    local t = dialogBox.transform
    local canvasGroup = self.ui.canvasGroup --logic.cs.LuaHelper.AddMissingComponent(dialogBox,typeof(logic.cs.CanvasGroup))

    t:DOKill()
    canvasGroup:DOKill()
    local duration = 0.18
    canvasGroup.alpha = 0
    canvasGroup:DOFade(1,duration)

    t.localScale = core.Vector3.one
    t.anchoredPosition = core.Vector2.New(200,-150)
    local IpadAspectRatio = CS.GameUtility:IpadAspectRatio()
    local x = 18
    --if IpadAspectRatio then
    --    x = -180
    --end
    t:DOAnchorPos(core.Vector2.New(x,-70),duration):SetEase(core.tween.Ease.Flash):OnComplete(callback)
end


function OtherDialogueComponent:dialogOutTween(gameObject)
    local t = gameObject.transform
    local canvasGroup = self.ui.canvasGroup --logic.cs.LuaHelper.AddMissingComponent(gameObject,typeof(logic.cs.CanvasGroup))

    t:DOKill()
    canvasGroup:DOKill()
    local duration = 0.2
    local scale = 0.6
    canvasGroup.alpha = 1
    --canvasGroup:DOFade(0,duration)

    t.localScale = core.Vector3.one
    t:DOScale(core.Vector2.New(scale,scale,1),duration):SetEase(core.tween.Ease.Flash)
    t:DOAnchorPos(core.Vector2.New(350,-100),duration):SetEase(core.tween.Ease.Flash):OnComplete(function()
        self.ui.gameObject:SetActiveEx(false)
    end)
end

return OtherDialogueComponent
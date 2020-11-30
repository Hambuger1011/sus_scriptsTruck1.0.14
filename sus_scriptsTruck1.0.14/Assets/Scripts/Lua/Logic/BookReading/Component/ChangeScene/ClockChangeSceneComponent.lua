local Class = core.Class
local Base = logic.BookReading.BaseComponent

local ClockChangeSceneComponent = Class("ClockChangeSceneComponent", Base)
local ui = nil



--收集所用到的资源
function ClockChangeSceneComponent:CollectRes(resTable)
    Base.CollectRes(self,resTable)
    resTable["Assets/Bundle/UI/BookReading/ClockChangeListener.prefab"] = BookResType.UIRes
end


function ClockChangeSceneComponent:Clean()
    Base.Clean(self)
end


function ClockChangeSceneComponent:Play()

    local prefab = logic.bookReadingMgr.Res:GetPrefab("Assets/Bundle/UI/BookReading/ClockChangeListener.prefab")
    local maskObj = logic.cs.GameObject.Instantiate(prefab,logic.bookReadingMgr.view.transComponent,false)
    local maskTransform = maskObj.transform
    maskTransform:SetAsLastSibling()
    maskObj:SetActiveEx(true)

    local uiWatch = maskObj:GetComponent(typeof(CS.Watch))
    self:ClockChangeSceneTween(uiWatch,function()
        logic.cs.GameObject.Destroy(maskObj)
        self:ShowNextDialog()
    end)
end

function ClockChangeSceneComponent:ClockChangeSceneTween(uiWatch,callback)
    local lastSceneBG = logic.bookReadingMgr.view:ExchangeSceneBG()
    local curSceneBG = logic.bookReadingMgr.view.curSceneBG
    lastSceneBG.canvasGroup.alpha = 1
    curSceneBG.canvasGroup.alpha = 0

    logic.bookReadingMgr.view:SetSceneBG(self,curSceneBG)

    --赋值，指定这个时钟需要转多少圈数
    uiWatch:GetTurns(7,function(alpha)
        logic.bookReadingMgr.view:ResetOperationTips()
        if alpha >= 1 then
            lastSceneBG.canvasGroup.alpha = 0
            curSceneBG.canvasGroup.alpha = 1
            lastSceneBG:Clear()
            callback()
        else
            lastSceneBG.canvasGroup.alpha = 1 - alpha
            curSceneBG.canvasGroup.alpha = alpha
        end
    end) 
end

return ClockChangeSceneComponent
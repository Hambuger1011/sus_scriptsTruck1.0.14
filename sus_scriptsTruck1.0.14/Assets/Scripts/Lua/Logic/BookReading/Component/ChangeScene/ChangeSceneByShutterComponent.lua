local Class = core.Class
local Base = logic.BookReading.BaseComponent

local ChangeSceneByShutterComponent = Class("ChangeSceneByShutterComponent", Base)
local ui = nil



--收集所用到的资源
function ChangeSceneByShutterComponent:CollectRes(resTable)
    Base.CollectRes(self,resTable)
end


function ChangeSceneByShutterComponent:Clean()
	Base.Clean(self)
end


function ChangeSceneByShutterComponent:Play()
    local lastSceneBG = logic.bookReadingMgr.view:ExchangeSceneBG()
    local curSceneBG = logic.bookReadingMgr.view.curSceneBG
    lastSceneBG.canvasGroup.alpha = 1
    curSceneBG.canvasGroup.alpha = 1
    --lastSceneBG.transform:SetAsLastSibling()

    logic.bookReadingMgr.view:SetSceneBG(self,curSceneBG)

    --lastSceneBG.image.sprite = curSceneBG.image.sprite
    local oldMat = curSceneBG.image.material
    local effectMat = logic.cs.Material(logic.cs.Shader.Find("Game/UI/Effect/Shutter"))
    curSceneBG.image.material = effectMat
    effectMat:SetFloat("_Count", 10)
    effectMat:SetFloat("_Range",0)

    effectMat:DOFloat(1, "_Range", 1.2)
    :SetEase(core.tween.Ease.Flash)
    :SetDelay(0.3)
    :SetEase(core.tween.Ease.Flash)
    :OnUpdate(function()
        logic.cs.BookReadingWrapper.IsTextTween = true
    end)
    :OnComplete(function()
        lastSceneBG.canvasGroup.alpha = 0
        --lastSceneBG.transform:SetAsFirstSibling()
        curSceneBG.image.material = oldMat
        logic.cs.GameObject.Destroy(effectMat)
        --local width = logic.cs.Screen.width*1334/logic.cs.Screen.height
        local Vector2 = core.Vector2.New(375 - curSceneBG.transform.rect.width / 2, 0)
        curSceneBG.transform:DOAnchorPos(Vector2, 1.5)
        :OnComplete(function()
            lastSceneBG:Clear()
            logic.cs.BookReadingWrapper.IsTextTween = false
            self:ShowNextDialog()
        end)
    end)
end

return ChangeSceneByShutterComponent
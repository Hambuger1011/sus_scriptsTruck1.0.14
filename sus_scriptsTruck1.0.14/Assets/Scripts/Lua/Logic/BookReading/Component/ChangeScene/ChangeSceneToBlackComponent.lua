local Class = core.Class
local Base = logic.BookReading.BaseComponent

local ChangeSceneToBlackComponent = Class("ChangeSceneToBlackComponent", Base)


--收集所用到的资源
function ChangeSceneToBlackComponent:CollectRes(resTable)
    Base.CollectRes(self,resTable)
    resTable["Assets/Bundle/UI/BookReading/ChangeSceneMask.prefab"] = BookResType.UIRes
end


function ChangeSceneToBlackComponent:Clean()
	Base.Clean(self)
end


function ChangeSceneToBlackComponent:Play()
    local lastSceneBG = logic.bookReadingMgr.view:ExchangeSceneBG()
    local curSceneBG = logic.bookReadingMgr.view.curSceneBG
    lastSceneBG.canvasGroup.alpha = 1
    curSceneBG.canvasGroup.alpha = 0

    local prefab = logic.bookReadingMgr.Res:GetPrefab("Assets/Bundle/UI/BookReading/ChangeSceneMask.prefab")
    local maskObj = logic.cs.GameObject.Instantiate(prefab,logic.bookReadingMgr.view.transLayer,false)
    local maskTransform = maskObj.transform
    maskTransform:SetAsLastSibling()
    local maskImage = maskObj:GetComponent(typeof(logic.cs.Image))
    maskImage.color = core.Color.New(0,0,0,0)
    
    local fadeIn = nil
    local fadeOut = nil

    fadeIn = function()
        maskImage:DOFade(1,0.6):SetEase(core.tween.Ease.Flash):OnUpdate(function()
            logic.cs.BookReadingWrapper.IsTextTween = true
        end):OnComplete(function()
            lastSceneBG:Clear()
            curSceneBG.canvasGroup.alpha = 1
            logic.bookReadingMgr.view:SetSceneBG(self,curSceneBG)   --显示当前图片
            fadeOut()
        end)
    end

    fadeOut = function()
        maskImage:DOFade(0,0.6):SetEase(core.tween.Ease.Flash):SetDelay(0.3):OnUpdate(function()
            logic.cs.BookReadingWrapper.IsTextTween = true
        end):OnComplete(function()
                logic.cs.BookReadingWrapper.IsTextTween = false
                logic.cs.GameObject.Destroy(maskObj)
                self:ShowNextDialog()
        end)
    end
    fadeIn()
end

return ChangeSceneToBlackComponent
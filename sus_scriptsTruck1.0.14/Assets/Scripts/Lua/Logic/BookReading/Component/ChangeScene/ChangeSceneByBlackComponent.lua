local Class = core.Class
local Base = logic.BookReading.BaseComponent

local ChangeSceneByBlackComponent = Class("ChangeSceneByBlackComponent", Base)


--收集所用到的资源
function ChangeSceneByBlackComponent:CollectRes(resTable)
    Base.CollectRes(self,resTable)
    resTable["Assets/Bundle/UI/BookReading/ChangeSceneMask.prefab"] = BookResType.UIRes
end


function ChangeSceneByBlackComponent:Clean()
	Base.Clean(self)
end


function ChangeSceneByBlackComponent:Play()
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
            local width = logic.cs.Screen.width*1334/logic.cs.Screen.height
            local Vector2 = core.Vector2.New(width / 2 - curSceneBG.transform.rect.width / 2, 0)
            curSceneBG.transform:DOAnchorPos(Vector2, 1.5)
            :OnComplete(function()
                logic.cs.BookReadingWrapper.IsTextTween = false
                logic.cs.GameObject.Destroy(maskObj)
                self:ShowNextDialog()
            end)
        end)
    end
    fadeIn()
end

return ChangeSceneByBlackComponent
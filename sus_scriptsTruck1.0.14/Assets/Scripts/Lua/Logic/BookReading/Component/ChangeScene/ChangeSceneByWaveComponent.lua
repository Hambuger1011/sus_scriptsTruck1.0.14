local Class = core.Class
local Base = logic.BookReading.BaseComponent

local ChangeSceneByWaveComponent = Class("ChangeSceneByWaveComponent", Base)
local ui = nil



--收集所用到的资源
function ChangeSceneByWaveComponent:CollectRes(resTable)
    Base.CollectRes(self,resTable)
    --resTable["Assets/Bundle/UI/BookReading/ChangeSceneMask.prefab"] = BookResType.UIRes
end


function ChangeSceneByWaveComponent:Clean()
	Base.Clean(self)
end


function ChangeSceneByWaveComponent:Play()
    local lastSceneBG = logic.bookReadingMgr.view:ExchangeSceneBG()
    local curSceneBG = logic.bookReadingMgr.view.curSceneBG
    lastSceneBG.canvasGroup.alpha = 1
    curSceneBG.canvasGroup.alpha = 0

    logic.bookReadingMgr.view:SetSceneBG(self,curSceneBG)
    local effectCtrl = logic.bookReadingMgr.view.uiform:GetCamera().gameObject:AddComponent(typeof(CS.WaveEffect))
    effectCtrl:Play(core.Mathf.Max(logic.cs.Screen.width,logic.cs.Screen.height)/ 1200)
    logic.cs.BookReadingWrapper.IsTextTween = true

    local duration = 1.2
    lastSceneBG.canvasGroup:DOFade(0,duration)
    :SetEase(core.tween.Ease.Flash)
    :OnUpdate(function()
        logic.cs.BookReadingWrapper.IsTextTween = true
        curSceneBG.canvasGroup.alpha = 1 - lastSceneBG.canvasGroup.alpha
    end)
    :OnComplete(function()
        lastSceneBG.transform:ClearAllChild()
        logic.cs.GameObject.Destroy(effectCtrl)
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

return ChangeSceneByWaveComponent
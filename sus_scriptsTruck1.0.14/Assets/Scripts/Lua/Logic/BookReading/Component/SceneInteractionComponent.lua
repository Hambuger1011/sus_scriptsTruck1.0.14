local Class = core.Class
local Base = logic.BookReading.BaseComponent

local SceneInteractionComponent = Class("SceneInteractionComponent", Base)



--收集所用到的资源
function SceneInteractionComponent:CollectRes(resTable)
    Base.CollectRes(self,resTable)
    resTable["Assets/Bundle/UI/BookReading/SceneInteraction.prefab"] = BookResType.UIRes
end


function SceneInteractionComponent:Clean()
	Base.Clean(self)

end



function SceneInteractionComponent:Play()
    local lastSceneBG = logic.bookReadingMgr.view:ExchangeSceneBG()
    local curSceneBG = logic.bookReadingMgr.view.curSceneBG
    lastSceneBG.canvasGroup.alpha = 1
    curSceneBG.canvasGroup.alpha = 1
    lastSceneBG.transform:ClearAllChild()

    local prefab = logic.bookReadingMgr.Res:GetPrefab("Assets/Bundle/UI/BookReading/SceneInteraction.prefab")
    local maskObj = logic.cs.GameObject.Instantiate(prefab,logic.bookReadingMgr.view.transComponent,false)
    local maskTransform = maskObj.transform
    maskTransform:SetAsLastSibling()
    maskObj:SetActiveEx(true)

    local uictrl = maskObj:GetComponent(typeof(CS.BookReading.UISceneInteraction))
    uictrl:SetPos(
        logic.bookReadingMgr.view:GetPiexlX(self.cfg.sceneActionX), 
        logic.bookReadingMgr.view:GetPiexlX(self.cfg.sceneActionY)
    )
    uictrl:Show(function()
        self:ShowNextDialog()
    end)
end


return SceneInteractionComponent
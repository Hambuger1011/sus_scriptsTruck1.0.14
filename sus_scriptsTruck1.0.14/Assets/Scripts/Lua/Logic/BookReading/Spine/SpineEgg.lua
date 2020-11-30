local class = core.Class
local SpineEgg = class('SpineEgg')

function SpineEgg:__init(gameObject)
    self.gameObject = CS.DisplayUtil.GetChild(gameObject, "Egg")
    self.transform = self.gameObject.transform
    self.skeletonGraphic = self.gameObject:GetComponent(typeof(logic.cs.SkeletonGraphic))
    
    self.starGameObject = CS.DisplayUtil.GetChild(gameObject, "Star")
    self.starGameObject:SetActiveEx(false)
    --self.starTransform = self.starGameObject.transform
    --self.starSkeletonGraphic = self.starGameObject:GetComponent(typeof(logic.cs.SkeletonGraphic))
end

function SpineEgg:SetScale(localScale)
    self.transform.localScale = localScale
    --self.starTransform.localScale = localScale
end

function SpineEgg:SetPosition(anchoredPosition)
    self.transform.anchoredPosition = anchoredPosition
    --self.starTransform.anchoredPosition = anchoredPosition
end

function SpineEgg:SetSpine(spineData)
    if self.spineData == spineData then
        return
    end
    --self.starSkeletonGraphic:Initialize(true)
    
	self.skeletonGraphic.skeletonDataAsset = spineData
	self.skeletonGraphic:Initialize(true)
    self.animName = nil
end

function SpineEgg:SetData(animName,loop)
    
    loop = loop or false
    
    logic.debug.Log("-SetData -animName-->"..tostring(animName).."-looploop-->"..tostring(loop))

    if self.animName ~= animName then
        self.animName = animName
        self.skeletonGraphic.startingAnimation = animName
        self.skeletonGraphic.startingLoop = loop
        self.skeletonGraphic:Initialize(true)
    end
end

return SpineEgg
local class = core.Class
local SpineCharacter = class('SpineCharacter')

function SpineCharacter:__init(gameObject)
    self.transform = gameObject.transform
    self.gameObject = gameObject
    self.uiBinding = self.gameObject:GetComponent(typeof(logic.cs.UIBinding))

    self.follow_back = self.uiBinding:Get('follow_back',typeof(logic.cs.BoneFollowerGraphic))
    self.follow_front = self.uiBinding:Get('follow_front',typeof(logic.cs.BoneFollowerGraphic))

    self.RoleSkeGraphic = self.uiBinding:Get('body',typeof(logic.cs.SkeletonGraphic))
    self.ExpressionSkeGraphic = self.uiBinding:Get('expression',typeof(logic.cs.SkeletonGraphic))
    self.hair_front = self.uiBinding:Get('hair_front',typeof(logic.cs.SkeletonGraphic))
    self.hair_back = self.uiBinding:Get('hair_back',typeof(logic.cs.SkeletonGraphic))
end

function SpineCharacter:SetScale(localScale)
    self.transform.localScale = localScale
end

function SpineCharacter:SetPosition(anchoredPosition)
    self.transform.anchoredPosition = anchoredPosition
end

function SpineCharacter:SetSpine(spineData)
    if self.spineData == spineData then
        return
    end
	self.RoleSkeGraphic.skeletonDataAsset = spineData
	self.ExpressionSkeGraphic.skeletonDataAsset = spineData
	self.hair_front.skeletonDataAsset = spineData
	self.hair_back.skeletonDataAsset = spineData

    --reload data
    local skinName = "skin1"
    self.RoleSkeGraphic.initialSkinName = skinName
    self.ExpressionSkeGraphic.initialSkinName = skinName
    self.hair_back.initialSkinName = skinName
    self.hair_front.initialSkinName = skinName

	self.RoleSkeGraphic:Initialize(true)
	self.ExpressionSkeGraphic:Initialize(true)
	self.hair_front:Initialize(true)
    self.hair_back:Initialize(true)
    
    
    self.follow_back.SkeletonGraphic = self.RoleSkeGraphic
    self.follow_front.SkeletonGraphic = self.RoleSkeGraphic

    self.skinName = nil
    self.clothesName = nil
    self.expressionName = nil
    self.hair1Name = nil
    self.hair2Name = nil
end

function SpineCharacter:SetData(
    skinName,clothesName,expressionName,hair1Name,hair2Name
    )

    --logic.debug.Log("-SetData -skinName-->"..skinName.."-clothesName-->"..
            --clothesName.."-expressionName-->"..expressionName.."-hair1Name-->"..hair1Name.."-hair2Name-->"..hair2Name)

    local dirty = false
    if self.skinName ~= skinName then
        dirty = true
        self.skinName = skinName
        self.RoleSkeGraphic.initialSkinName = skinName
        self.ExpressionSkeGraphic.initialSkinName = skinName
        self.hair_back.initialSkinName = skinName
        self.hair_front.initialSkinName = skinName

        self.RoleSkeGraphic:Initialize(true)
        self.ExpressionSkeGraphic:Initialize(true)
        self.hair_back:Initialize(true)
        self.hair_front:Initialize(true)
    end

    --set clothes
    if self.clothesName ~= clothesName then
        dirty = true
        self.clothesName = clothesName
        self.RoleSkeGraphic.startingAnimation = clothesName
        self.RoleSkeGraphic.startingLoop = true
        self.RoleSkeGraphic:Initialize(true)
    end

    --set ExpressionSkeGraphic
    if self.expressionName ~= expressionName then
        dirty = true
        self.expressionName = expressionName
        self.ExpressionSkeGraphic.startingAnimation = expressionName
        self.ExpressionSkeGraphic.startingLoop = false
        self.ExpressionSkeGraphic:Initialize(true)
    end

    --set Hair
    if self.hair1Name ~= hair1Name then
        dirty = true
        self.hair1Name = hair1Name
        self.hair_front.startingAnimation = hair1Name
        self.hair_front.startingLoop = true
        self.hair_front:Initialize(true)
    end
    
    if self.hair2Name ~= hair2Name then
        dirty = true
        self.hair2Name = hair2Name
        self.hair_back.startingAnimation = hair2Name
        self.hair_back.startingLoop = true
        self.hair_back:Initialize(true)
    end

	core.coroutine.start(function()
        --只有一帧手机上出现无法跟随，改成3帧
        for i=1,3 do
            self.follow_back.SkeletonGraphic = self.RoleSkeGraphic
            self.follow_back.boneName = "tou"
            self.follow_back.followBoneRotation = false
            self.follow_back.followZPosition = true
            self.follow_back.followLocalScale = true
            self.follow_back.followSkeletonFlip = true
            self.follow_back.enabled = true
            self.follow_back:Initialize()
            local pos = self.follow_back.transform.anchoredPosition
            local targetPos = core.Vector3.New(-pos.x, -pos.y)
            self.hair_back.transform.anchoredPosition = targetPos
            
            
            self.follow_front.SkeletonGraphic = self.RoleSkeGraphic
            self.follow_front.boneName = "tou"
            self.follow_front.followBoneRotation = false
            self.follow_front.followZPosition = true
            self.follow_front.followLocalScale = true
            self.follow_front.followSkeletonFlip = true
            self.follow_front.enabled = true
            self.follow_front:Initialize()
            local pos = self.follow_front.transform.anchoredPosition
            local targetPos = core.Vector3.New(-pos.x, -pos.y)
            self.ExpressionSkeGraphic.transform.anchoredPosition = targetPos
            self.hair_front.transform.anchoredPosition = targetPos
            core.coroutine.step(1)
        end
	end)
end

return SpineCharacter
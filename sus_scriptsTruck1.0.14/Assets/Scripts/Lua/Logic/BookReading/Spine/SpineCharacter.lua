local class = core.Class
local SpineCharacter = class('SpineCharacter')

function SpineCharacter:__init(gameObject)
    self.transform = gameObject.transform
    self.gameObject = gameObject
    self.uiBinding = self.gameObject:GetComponent(typeof(logic.cs.UIBinding))

    self.follow_backObj = CS.DisplayUtil.GetChild(gameObject, "follow_back");
    self.follow_back=CS.XLuaHelper.AddBoneFollowerGraphic(self.follow_backObj);

    self.hair_backObj = CS.DisplayUtil.GetChild(self.follow_backObj, "hair_back");
    self.hair_back=CS.XLuaHelper.AddSkeletonGraphic(self.hair_backObj);


    self.RoleSkeletonGraphicObj = CS.DisplayUtil.GetChild(gameObject, "RoleSkeletonGraphic");
    self.RoleSkeletonGraphic=CS.XLuaHelper.AddSkeletonGraphic(self.RoleSkeletonGraphicObj);


    self.follow_frontObj = CS.DisplayUtil.GetChild(self.RoleSkeletonGraphicObj, "follow_front");
    self.follow_front=CS.XLuaHelper.AddBoneFollowerGraphic(self.follow_frontObj);


    self.ExpressionSkeletonGraphicObj = CS.DisplayUtil.GetChild(self.follow_frontObj, "ExpressionSkeletonGraphic");
    self.ExpressionSkeletonGraphic=CS.XLuaHelper.AddSkeletonGraphic(self.ExpressionSkeletonGraphicObj);


    self.hair_frontObj = CS.DisplayUtil.GetChild(self.follow_frontObj, "hair_front");
    self.hair_front=CS.XLuaHelper.AddSkeletonGraphic(self.hair_frontObj);


    self.hair_back.material=CS.XLuaHelper.SpineMaterial;
    self.RoleSkeletonGraphic.material=CS.XLuaHelper.SpineMaterial;
    self.ExpressionSkeletonGraphic.material=CS.XLuaHelper.SpineMaterial;
    self.hair_front.material=CS.XLuaHelper.SpineMaterial;
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
	self.RoleSkeletonGraphic.skeletonDataAsset = spineData
	self.ExpressionSkeletonGraphic.skeletonDataAsset = spineData
	self.hair_front.skeletonDataAsset = spineData
	self.hair_back.skeletonDataAsset = spineData

    --reload data
    local skinName = "skin1"
    self.RoleSkeletonGraphic.initialSkinName = skinName
    self.ExpressionSkeletonGraphic.initialSkinName = skinName
    self.hair_back.initialSkinName = skinName
    self.hair_front.initialSkinName = skinName

	self.RoleSkeletonGraphic:Initialize(true)
	self.ExpressionSkeletonGraphic:Initialize(true)
	self.hair_front:Initialize(true)
    self.hair_back:Initialize(true)
    
    
    self.follow_back.SkeletonGraphic = self.RoleSkeletonGraphic
    self.follow_front.SkeletonGraphic = self.RoleSkeletonGraphic

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
        self.RoleSkeletonGraphic.initialSkinName = skinName
        self.ExpressionSkeletonGraphic.initialSkinName = skinName
        self.hair_back.initialSkinName = skinName
        self.hair_front.initialSkinName = skinName

        self.RoleSkeletonGraphic:Initialize(true)
        self.ExpressionSkeletonGraphic:Initialize(true)
        self.hair_back:Initialize(true)
        self.hair_front:Initialize(true)
    end

    --set clothes
    if self.clothesName ~= clothesName then
        dirty = true
        self.clothesName = clothesName
        self.RoleSkeletonGraphic.startingAnimation = clothesName
        self.RoleSkeletonGraphic.startingLoop = true
        self.RoleSkeletonGraphic:Initialize(true)
    end

    --set ExpressionSkeletonGraphic
    if self.expressionName ~= expressionName then
        dirty = true
        self.expressionName = expressionName
        self.ExpressionSkeletonGraphic.startingAnimation = expressionName
        self.ExpressionSkeletonGraphic.startingLoop = false
        self.ExpressionSkeletonGraphic:Initialize(true)
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
            self.follow_back.SkeletonGraphic = self.RoleSkeletonGraphic
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
            
            
            self.follow_front.SkeletonGraphic = self.RoleSkeletonGraphic
            self.follow_front.boneName = "tou"
            self.follow_front.followBoneRotation = false
            self.follow_front.followZPosition = true
            self.follow_front.followLocalScale = true
            self.follow_front.followSkeletonFlip = true
            self.follow_front.enabled = true
            self.follow_front:Initialize()
            local pos = self.follow_front.transform.anchoredPosition
            local targetPos = core.Vector3.New(-pos.x, -pos.y)
            self.ExpressionSkeletonGraphic.transform.anchoredPosition = targetPos
            self.hair_front.transform.anchoredPosition = targetPos
            core.coroutine.step(1)
        end
	end)
end

return SpineCharacter
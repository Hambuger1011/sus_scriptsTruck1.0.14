local BaseClass = core.Class
local ImageWall = BaseClass("ImageWall")
local ChoiceEnum = {
    Sex = {Male = 1, Female = 0},
    Top = {Character = 1, Hairstyle = 2, Outfit = 3, Background = 4}
}

function ImageWall:__init(gameObject)
    self.gameObject=gameObject;
    
    self.HideBtn = CS.DisplayUtil.GetChild(gameObject, "HideBtn"):GetComponent(typeof(logic.cs.Button));
    
    self.spine = CS.DisplayUtil.GetChild(gameObject, "spine");

    self.follow_backObj = CS.DisplayUtil.GetChild(gameObject, "follow_back");
    self.follow_back=CS.XLuaHelper.AddBoneFollowerGraphic(self.follow_backObj);

    self.hair_backObj = CS.DisplayUtil.GetChild(gameObject, "hair_back");
    self.hair_back=CS.XLuaHelper.AddSkeletonGraphic(self.hair_backObj);

    self.BodySkeletonGraphicObj = CS.DisplayUtil.GetChild(gameObject, "BodySkeletonGraphic");
    self.BodySkeletonGraphic=CS.XLuaHelper.AddSkeletonGraphic(self.BodySkeletonGraphicObj);

    self.ClothesSkeletonGraphicObj = CS.DisplayUtil.GetChild(gameObject, "ClothesSkeletonGraphic");
    self.ClothesSkeletonGraphic=CS.XLuaHelper.AddSkeletonGraphic(self.ClothesSkeletonGraphicObj);

    self.follow_frontObj = CS.DisplayUtil.GetChild(gameObject, "follow_front");
    self.follow_front=CS.XLuaHelper.AddBoneFollowerGraphic(self.follow_frontObj);

    self.ExpressionSkeletonGraphicObj = CS.DisplayUtil.GetChild(gameObject, "ExpressionSkeletonGraphic");
    self.ExpressionSkeletonGraphic=CS.XLuaHelper.AddSkeletonGraphic(self.ExpressionSkeletonGraphicObj);

    self.hair_frontObj = CS.DisplayUtil.GetChild(gameObject, "hair_front");
    self.hair_front=CS.XLuaHelper.AddSkeletonGraphic(self.hair_frontObj);

    self.hair_back.material=CS.XLuaHelper.SpineMaterial;
    self.BodySkeletonGraphic.material=CS.XLuaHelper.SpineMaterial;
    self.ClothesSkeletonGraphic.material=CS.XLuaHelper.SpineMaterial;
    self.ExpressionSkeletonGraphic.material=CS.XLuaHelper.SpineMaterial;
    self.hair_front.material=CS.XLuaHelper.SpineMaterial;

    local bodySpineData = self:GetSkeDataAsset("Assets/Bundle/ImageWall/male/body/body_SkeletonData.asset")
    local clothesSpineData = self:GetSkeDataAsset("Assets/Bundle/ImageWall/male/clothes/clothes1_SkeletonData.asset")
    local expressionSpineData = self:GetSkeDataAsset("Assets/Bundle/ImageWall/male/head/b/head1_SkeletonData.asset")
    local hairSpineData = self:GetSkeDataAsset("Assets/Bundle/ImageWall/male/hair/hair1_SkeletonData.asset")

    self:SetSpine(bodySpineData,clothesSpineData,expressionSpineData,hairSpineData)

    local skinName = "skin1"
    local bodyName = "h_body"
    local clothesName = "clothes1"
    local expressionName = "expression1_6"
    local hair1Name	= "hair1"
    local hair2Name	= "hair1_1"
    self:SetData(skinName,bodyName,clothesName,expressionName,hair1Name,hair2Name)

    self.ChoiceBG = CS.DisplayUtil.GetChild(gameObject, "ChoiceBG")
    self.ShowChoiceBtn = CS.DisplayUtil.GetChild(gameObject, "ShowChoiceBtn"):GetComponent(typeof(logic.cs.Button))
    self.HideChoiceBtn = CS.DisplayUtil.GetChild(gameObject, "HideChoiceBtn"):GetComponent(typeof(logic.cs.Button))

    self.ShowChoiceBtn.onClick:RemoveAllListeners()
    self.ShowChoiceBtn.onClick:AddListener(function() self:SetChoiceBGShow(true) end)
    self.HideChoiceBtn.onClick:RemoveAllListeners()
    self.HideChoiceBtn.onClick:AddListener(function() self:SetChoiceBGShow(false) end)
    
    self.SexChoice = CS.DisplayUtil.GetChild(gameObject, "SexChoice")
    self.MaleChoice = CS.DisplayUtil.GetChild(gameObject, "MaleChoice"):GetComponent(typeof(logic.cs.Button))
    self.MaleChoiceMake = CS.DisplayUtil.GetChild(gameObject, "MaleChoiceMake")
    self.FemaleChoice = CS.DisplayUtil.GetChild(gameObject, "FemaleChoice"):GetComponent(typeof(logic.cs.Button))
    self.FemaleChoiceMake = CS.DisplayUtil.GetChild(gameObject, "FemaleChoiceMake")

    self.MaleChoice.onClick:RemoveAllListeners()
    self.MaleChoice.onClick:AddListener(function() self:SetSex(ChoiceEnum.Sex.Male) end)
    self.FemaleChoice.onClick:RemoveAllListeners()
    self.FemaleChoice.onClick:AddListener(function() self:SetSex(ChoiceEnum.Sex.Female) end)

    self.CharacterSwitch = CS.DisplayUtil.GetChild(gameObject, "CharacterSwitch"):GetComponent(typeof(logic.cs.Button))
    self.CharacterSwitchMake = CS.DisplayUtil.GetChild(gameObject, "CharacterSwitchMake")
    self.CharacterSwitchLine = CS.DisplayUtil.GetChild(gameObject, "CharacterSwitchLine")
    self.HairstyleSwitch = CS.DisplayUtil.GetChild(gameObject, "HairstyleSwitch"):GetComponent(typeof(logic.cs.Button))
    self.HairstyleSwitchMake = CS.DisplayUtil.GetChild(gameObject, "HairstyleSwitchMake")
    self.HairstyleSwitchLine = CS.DisplayUtil.GetChild(gameObject, "HairstyleSwitchLine")
    self.OutfitSwitch = CS.DisplayUtil.GetChild(gameObject, "OutfitSwitch"):GetComponent(typeof(logic.cs.Button))
    self.OutfitSwitchMake = CS.DisplayUtil.GetChild(gameObject, "OutfitSwitchMake")
    self.OutfitSwitchLine = CS.DisplayUtil.GetChild(gameObject, "OutfitSwitchLine")
    self.BackgroundSwitch = CS.DisplayUtil.GetChild(gameObject, "BackgroundSwitch"):GetComponent(typeof(logic.cs.Button))
    self.BackgroundSwitchMake = CS.DisplayUtil.GetChild(gameObject, "BackgroundSwitchMake")
    self.BackgroundSwitchLine = CS.DisplayUtil.GetChild(gameObject, "BackgroundSwitchLine")
    
    self.CharacterSwitch.onClick:RemoveAllListeners()
    self.CharacterSwitch.onClick:AddListener(function() self:SetTopSwitch(ChoiceEnum.Top.Character) end)
    self.HairstyleSwitch.onClick:RemoveAllListeners()
    self.HairstyleSwitch.onClick:AddListener(function() self:SetTopSwitch(ChoiceEnum.Top.Hairstyle) end)
    self.OutfitSwitch.onClick:RemoveAllListeners()
    self.OutfitSwitch.onClick:AddListener(function() self:SetTopSwitch(ChoiceEnum.Top.Outfit) end)
    self.BackgroundSwitch.onClick:RemoveAllListeners()
    self.BackgroundSwitch.onClick:AddListener(function() self:SetTopSwitch(ChoiceEnum.Top.Background) end)
    
    self.SkinColourMakeList = {}
    local SkinColourSwitch
    local skinNum = 4
    for i = 1, skinNum do
        SkinColourSwitch = CS.DisplayUtil.GetChild(gameObject, "SkinColourSwitch"..i):GetComponent(typeof(logic.cs.Button))
        SkinColourSwitch.onClick:RemoveAllListeners()
        SkinColourSwitch.onClick:AddListener(function() self:SetSkinSwitch(i) end)
        self.SkinColourMakeList[i] = CS.DisplayUtil.GetChild(SkinColourSwitch.gameObject, "SkinColourMake")
    end

    self.ModelSwitchText = CS.DisplayUtil.GetChild(gameObject, "ModelSwitchText"):GetComponent(typeof(logic.cs.Text))
    self:SetChoiceBGShow(nil)
end

--region【展示or隐藏选项】
function ImageWall:SetChoiceBGShow(_show)
    if self.ChoiceBGIsShow ~= nil and self.ChoiceBGIsShow == _show then
        return
    end
    self.ChoiceBGIsShow = _show
    self.SexChoice:SetActive(_show)
    if self.ChoiceBGPosY == nil then
        self.ChoiceBGPosY = self.ChoiceBG.transform.localPosition.y
    end
    self.ChoiceBGPosY = _show and (self.ChoiceBGPosY + 700) or (self.ChoiceBGPosY - 700)
    if _show then
        self.ShowChoiceBtn.gameObject:SetActive(false)
        self.ChoiceBG.transform:DOLocalMoveY(self.ChoiceBGPosY, 0.5):OnComplete(function()  end):Play()
    else
        self.ChoiceBG.transform:DOLocalMoveY(self.ChoiceBGPosY, 0.5):OnComplete(function() self.ShowChoiceBtn.gameObject:SetActive(true) end):Play()
    end
end
--endregion

--region【性别选择】
function ImageWall:SetSex(sex)
    self.SexSwitchIndex = sex
    local isMale = self.SexSwitchIndex == ChoiceEnum.Sex.Male
    self.MaleChoiceMake:SetActive(isMale)
    self.FemaleChoiceMake:SetActive(not isMale)
    self:UpdateChoiceList()
end
--endregion

--region【Top类型选择】
function ImageWall:SetTopSwitch(_index)
    self.TopSwitchIndex = _index
    self.CharacterSwitchMake:SetActive(_index == ChoiceEnum.Top.Character)
    self.CharacterSwitchLine:SetActive(_index == ChoiceEnum.Top.Character)
    self.HairstyleSwitchMake:SetActive(_index == ChoiceEnum.Top.Hairstyle)
    self.HairstyleSwitchLine:SetActive(_index == ChoiceEnum.Top.Hairstyle)
    self.OutfitSwitchMake:SetActive(_index == ChoiceEnum.Top.Outfit)
    self.OutfitSwitchLine:SetActive(_index == ChoiceEnum.Top.Outfit)
    self.BackgroundSwitchMake:SetActive(_index == ChoiceEnum.Top.Background)
    self.BackgroundSwitchLine:SetActive(_index == ChoiceEnum.Top.Background)
    self:UpdateChoiceList()
end
--endregion

--region【肤色选择】
function ImageWall:SetSkinSwitch(_index)
    self.SkinSwitchIndex = _index
    for k, v in pairs(self.SkinColourMakeList) do
        v:SetActive(_index == k)
    end
    self:UpdateChoiceList()
end
--endregion

--region【更新选项列表】
function ImageWall:UpdateChoiceList()
    self.ModelSwitchText.text = "Sex:"..tostring(self.SexSwitchIndex).."\nTop:"..tostring(self.TopSwitchIndex).."\nSkin:"..tostring(self.SkinSwitchIndex)
end
--endregion

--region【spine】
--region【获取spine文件】
function ImageWall:GetSkeDataAsset(url)
    --local asset = logic.ResMgr.LoadImme(logic.ResMgr.tag.Null,logic.ResMgr.type.ScriptableObject,url)
    local asset = logic.ResMgr.LoadImme(logic.ResMgr.tag.Null,logic.ResMgr.type.Object,url)
    if not asset or not asset.resObject then
        logic.debug.LogError("资源未预加载:"..url)
        return nil
    end
    return asset.resObject
end
--endregion

--region【设置spine大小】
function ImageWall:SetScale(localScale)
    self.spine.localScale = localScale
end
--endregion

--region【设置spine位置】
function ImageWall:SetPosition(anchoredPosition)
    self.spine.anchoredPosition = anchoredPosition
end
--endregion

--region【设置spine文件】
function ImageWall:SetSpine(bodySpineData,clothesSpineData,expressionSpineData,hairSpineData)
    local skinName = "skin1"

    if self.bodySpineData ~= bodySpineData then
        self.BodySkeletonGraphic.skeletonDataAsset = bodySpineData
        self.BodySkeletonGraphic.initialSkinName = skinName
        self.BodySkeletonGraphic:Initialize(true)
        self.bodyName = nil
        self.follow_back.SkeletonGraphic = self.BodySkeletonGraphic
        self.follow_front.SkeletonGraphic = self.BodySkeletonGraphic
        self.skinName = nil
    end

    if self.clothesSpineData ~= clothesSpineData then
        self.ClothesSkeletonGraphic.skeletonDataAsset = clothesSpineData
        self.ClothesSkeletonGraphic.initialSkinName = skinName
        self.ClothesSkeletonGraphic:Initialize(true)
        self.clothesName = nil
    end

    if self.expressionSpineData ~= expressionSpineData then
        self.ExpressionSkeletonGraphic.skeletonDataAsset = expressionSpineData
        self.ExpressionSkeletonGraphic.initialSkinName = skinName
        self.ExpressionSkeletonGraphic:Initialize(true)
        self.expressionName = nil
    end
    
    if self.hairSpineData ~= hairSpineData then
        self.hairSpineData = hairSpineData
        self.hair_front.skeletonDataAsset = hairSpineData
        self.hair_back.skeletonDataAsset = hairSpineData
        self.hair_back.initialSkinName = skinName
        self.hair_front.initialSkinName = skinName
        self.hair_front:Initialize(true)
        self.hair_back:Initialize(true)
        self.hair1Name = nil
        self.hair2Name = nil
    end
end
--endregion

--region【设置spine人物】
function ImageWall:SetData(skinName,bodyName,clothesName,expressionName,hair1Name,hair2Name)
    local dirty = false
    if self.skinName ~= skinName then
        dirty = true
        self.skinName = skinName
        self.BodySkeletonGraphic.initialSkinName = skinName
        self.ClothesSkeletonGraphic.initialSkinName = skinName
        self.ExpressionSkeletonGraphic.initialSkinName = skinName
        self.hair_back.initialSkinName = skinName
        self.hair_front.initialSkinName = skinName

        self.BodySkeletonGraphic:Initialize(true)
        self.ClothesSkeletonGraphic:Initialize(true)
        self.ExpressionSkeletonGraphic:Initialize(true)
        self.hair_back:Initialize(true)
        self.hair_front:Initialize(true)
    end

    --set clothes
    if self.clothesName ~= clothesName then
        dirty = true
        self.clothesName = clothesName
        self.ClothesSkeletonGraphic.startingAnimation = clothesName
        self.ClothesSkeletonGraphic.startingLoop = true
        self.ClothesSkeletonGraphic:Initialize(true)
    end

    --set clothes
    if self.bodyName ~= bodyName then
        dirty = true
        self.bodyName = bodyName
        self.BodySkeletonGraphic.startingAnimation = bodyName
        self.BodySkeletonGraphic.startingLoop = true
        self.BodySkeletonGraphic:Initialize(true)
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
            self.follow_back.SkeletonGraphic = self.BodySkeletonGraphic
            self.follow_back.boneName = "bone"
            self.follow_back.followBoneRotation = false
            self.follow_back.followZPosition = true
            self.follow_back.followLocalScale = true
            self.follow_back.followSkeletonFlip = true
            self.follow_back.enabled = true
            self.follow_back:Initialize()
            local pos = self.follow_back.transform.anchoredPosition
            local targetPos = core.Vector3.New(-pos.x, -pos.y)
            self.hair_back.transform.anchoredPosition = targetPos


            self.follow_front.SkeletonGraphic = self.BodySkeletonGraphic
            self.follow_front.boneName = "bone"
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
--endregion

--region【设置Hide点击事件】
function ImageWall:SetHideOnClick(HideOnClick)
    self.HideBtn.onClick:RemoveAllListeners()
    self.HideBtn.onClick:AddListener(HideOnClick)
end
--endregion
--endregion

--销毁
function ImageWall:__delete()
end

return ImageWall

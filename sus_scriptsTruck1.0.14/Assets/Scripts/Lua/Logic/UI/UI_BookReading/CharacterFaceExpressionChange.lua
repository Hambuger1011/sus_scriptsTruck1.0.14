local CharacterFaceExpressionChange = core.Class('CharacterFaceExpressionChange')
local cs_CharacterFaceExpressionChange = CS.CharacterFaceExpressionChange

function CharacterFaceExpressionChange:__init(trans)
    self.transform = trans
    self.gameObject = trans.gameObject
    self.uiBinding = self.gameObject:GetComponent(typeof(logic.cs.UIBinding))
    self.component = self.gameObject:GetComponent(typeof(cs_CharacterFaceExpressionChange))
    self.roleSpine = logic.SpineCharacter.New(self.uiBinding:Get('spine'))

    -- self.component.loadSprite = function(name, isCommon)
    --     return logic.bookReadingMgr.Res:GetSprite(name, isCommon)
    -- end
    -- self.Change = function(role_id, appearanceID, clothesID, phiz_id, iconBGId, vOrientation)
    --     --self.component:Change(role_id, appearanceID, clothesID, phiz_id, iconBGId, vOrientation)
    -- end
    self.currentIndex = 0
    self.duration = 0.4
end

function CharacterFaceExpressionChange:Change(roleId, appearanceId, clothId, faciaOriginalID, iconBGID, vOrientation)
    self.delay = 0.3
    self.facialDelay = 0.5
    self.mRoleID = roleId
    self.mAppearanceID = appearanceId
    self.mClothId = clothId
    self.mFaciaOriginalID = faciaOriginalID
    self.mIconBGID = iconBGID
    self.mOrientation = vOrientation
    self:DoChange(appearanceId, clothId, faciaOriginalID, iconBGID, vOrientation)
end

function CharacterFaceExpressionChange:DoChange(appearanceId, clothId, faciaOriginalID, iconBGID, vOrientation)
    self.component.ColorBG_1:DOKill()
    self.component.ColorBG_2:DOKill()

    local sptBG = logic.cs.ResourceManager:GetUISprite("BookReadingForm/ColorBG_" .. iconBGID)
    if self:isChangeCharacter(appearanceId) then
        self.isSetPos = false
        self.component.ColorBG_1.sprite = sptBG
        self.component.ColorBG_2.sprite = sptBG
        self:UpdateAtlas()  --新的资源加载方式
        self:ResetPos()
    else
        -- if (self:isChangeClothes(self.mClothId)) then
        --     self:SetClothesIndex(self.mClothId)
        -- end
            
        -- if (self:isChangeExpression(self.mFaciaOriginalID)) then
        --     self:SetExpression(self.mFaciaOriginalID)
        --     local harIdx = (self.mFaciaOriginalID == 0) and 0 or 1
        --     if(self.mCacheHairIndex ~= harIdx) then
        --         self.mCacheHairIndex = harIdx
        --         self:SetHair(harIdx)
        --     end
        -- end

        -- if (self:isChangeSkin()) then
        --     self:SetSkinName(self.mLastSkinId)
        -- end
        
        self:UpdateAtlas()  --新的资源加载方式
    end
    
    local dict = core.Vector3.New(-1,1,1)
    if vOrientation == 2 then
        dict = core.Vector3.New(1,1,1)
    end
    self.component.Appearance.transform.localScale = dict
 
    if (self.currentIndex == 0) then
        self:imageChangeTween(
            self.component.ColorBG_1,
            self.component.ColorBG_2,
            sptBG, self.delay, nil
            )
        self.currentIndex = 1
    else
        self:imageChangeTween(
            self.component.ColorBG_2, 
            self.component.ColorBG_1, 
            sptBG, self.delay, nil
            )
        self.currentIndex = 0
    end


    local localScale = Vector3.New(1, 1, 1)
    local anchoredPosition = Vector2.New(0, -460)
    local bookDetial = logic.cs.JsonDTManager:GetJDTBookDetailInfo(logic.bookReadingMgr.bookID);
    if(bookDetial.rolescale == 2) then
        localScale = Vector3.New(0.85,0.85,1)
        anchoredPosition = Vector2.New(30, -175)
    end
    self.roleSpine:SetScale(localScale)
    self.roleSpine:SetPosition(anchoredPosition)
end

--是否角色有变化
function CharacterFaceExpressionChange:isChangeCharacter(appearanceID)
    if self.appearanceIDCache == appearanceID then
        return false
    end
    self.appearanceIDCache = appearanceID
    return true
end


function CharacterFaceExpressionChange:imageChangeTween(imageFirst, imageSceond, sprite, vDelay, vTurnCallBack)
    imageSceond.gameObject:SetActive(true);
    imageSceond.sprite = sprite;
    imageSceond.color = core.Color.New(1, 1, 1, 0)
    imageSceond:DOColor(core.Color.New(1, 1, 1, 1), self.duration):SetDelay(vDelay)
        :OnComplete(function()
            imageSceond.transform:SetAsFirstSibling()
            imageFirst.gameObject:SetActive(false)
            imageSceond.gameObject:SetActive(true)
            logic.cs.EventDispatcher.Dispatch(tostring(CS.UIEventMethodName.BookReadingForm_IsTweening), false)
            if (vTurnCallBack ~= nil and self.mTurnCallBack ~= nil) then
                self.mTurnCallBack()
            end
        end);
end

function CharacterFaceExpressionChange:UpdateAtlas()
    local skeData = logic.bookReadingMgr.Res:GetSkeDataAsset("Role/".. self.mAppearanceID)
    if (skeData == nil) then
        logic.debug.LogError("---角色Spine SkeData 有误-->" .. self.mAppearanceID)
        return
    end
    self.roleSpine:SetSpine(skeData)
    self:ResetAvatarInfo()
end

function CharacterFaceExpressionChange:ResetAvatarInfo()
    local skinIndex = 1
    local npcDetailId = 0
    local bookData = logic.bookReadingMgr.bookData
    if self.mRoleID == 1 then
        skinIndex = bookData.PlayerDetailsID
    elseif self.mRoleID > 0 then
        local recordeNpcId = 0
        local npcDetailId = 0
        npcDetailId = bookData.NpcDetailId
        recordeNpcId = bookData.NpcId
        if (npcDetailId > 0 and self.mRoleID == recordeNpcId) then
            local tempNpcDetail = (npcDetailId > 3) and (npcDetailId - 3) or npcDetailId
            skinIndex = tempNpcDetail
        end
    end
    local hairId = 1
    if (self.mFaciaOriginalID == 0) then
        hairId = 0
    end

    if self.mRoleID == 1 then
        if bookData.hair_id and bookData.hair_id ~=0 then
            hairId = bookData.hair_id
        end
        if bookData.character_id and bookData.character_id ~=0 then
            skinIndex = bookData.character_id
        end
        if self.mClothId and self.mClothId ~= 0 and bookData.outfit_id and bookData.outfit_id ~= 0 then
            self.mClothId = bookData.outfit_id
        end
    end

    self.mLastSkinId = skinIndex

    local skinName = "skin" .. skinIndex
	local clothesName = "clothes" .. self.mClothId
	local expressionName = "expression" .. self.mFaciaOriginalID
	local hair1Name	= "hair" .. hairId
    local hair2Name	= "hair" .. hairId.."_1"

    if self.mRoleID == 1 then
        --local clothGroupId = math.ceil( self.mClothId/4 )
        --if clothGroupId == 0 then
        --    clothGroupId = 1
        --end
        local skeID = logic.bookReadingMgr:GetAppearanceID(1,0, 1)
        local skeData = logic.bookReadingMgr.Res:GetSkeDataAsset("Role/".. skeID)
        if (skeData == nil) then
            logic.debug.LogError("---角色Spine SkeData 有误-->" .. skeID)
        else
            --isfalse
            self.roleSpine:SetSpine(skeData)
        end
    end
	self.roleSpine:SetData(skinName,clothesName,expressionName,hair1Name,hair2Name)
end

function CharacterFaceExpressionChange:ResetPos()
    if self.isSetPos then
        return
    end
    self.isSetPos = true
    core.coroutine.start(function()
        core.coroutine.step(1)
        local targetPos = Vector3.New(
            -self.component.HeadBoneFolGraphic.transform.localPosition.x, 
            -self.component.HeadBoneFolGraphic.transform.localPosition.y
        )
        self.component.ExpressionSkeGraphic.transform.localPosition = targetPos
        self.component.HairSkeGraphic.transform.localPosition = targetPos
    end)
end

return CharacterFaceExpressionChange
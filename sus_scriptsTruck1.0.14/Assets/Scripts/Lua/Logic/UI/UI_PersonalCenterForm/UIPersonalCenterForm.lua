local UIView = core.UIView
local UIPersonalCenterForm = core.Class("UIPersonalCenterForm", UIView)
local this=UIPersonalCenterForm;

local uiid = logic.uiid
UIPersonalCenterForm.config = {
    ID = uiid.UIPersonalCenterForm,
    AssetName = 'UI/Resident/UI/UIPersonalCenterForm'
}

--region【Awake】

function UIPersonalCenterForm:OnInitView()
    UIView.OnInitView(self)
    
    local gameObject = self.uiform.gameObject

    self.spine = CS.DisplayUtil.GetChild(gameObject, "spine");

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

    local spine = self:GetSkeDataAsset(80,1100010000)

    self:SetSpine(spine)

    local skinName = "skin1"
    local clothesName = "clothes1"
    local expressionName = "expression0"
    local hair1Name	= "hair1"
    local hair2Name	= "hair1_1"
    self:SetData(skinName,clothesName,expressionName,hair1Name,hair2Name)
end
--endregion


function UIPersonalCenterForm:GetSkeDataAsset(bookId,key)
    local AbBookSystem = CS.AB.AbBookSystem.Create(bookId)
    local asset = AbBookSystem:LoadImme(logic.ResMgr.tag.DialogDisplay,logic.ResMgr.type.ScriptableObject,"Assets/Bundle/Book/"..bookId.."/Role/"..key.."_SkeletonData.asset")
    if not asset or not asset.resObject then
        logic.debug.LogError("资源未预加载:"..key)
        return nil
    end
    return asset.resObject
end

--region【设置spine大小】
function UIPersonalCenterForm:SetScale(localScale)
    self.spine.localScale = localScale
end
--endregion

--region【设置spine位置】
function UIPersonalCenterForm:SetPosition(anchoredPosition)
    self.spine.anchoredPosition = anchoredPosition
end
--endregion

--region【设置spine文件】
function UIPersonalCenterForm:SetSpine(spineData)
    if self.spineData == spineData then
        return
    end
    self.RoleSkeletonGraphic.skeletonDataAsset = spineData
    self.ExpressionSkeletonGraphic.skeletonDataAsset = spineData
    self.hair_front.skeletonDataAsset = spineData
    self.hair_back.skeletonDataAsset = spineData

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

    self.spineData = spineData
    self.skinName = nil
    self.clothesName = nil
    self.expressionName = nil
    self.hair1Name = nil
    self.hair2Name = nil
end
--endregion

--region【设置spine人物】
function UIPersonalCenterForm:SetData(
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
--endregion


--region【OnOpen】

function UIPersonalCenterForm:OnOpen()
    UIView.OnOpen(self)
    
    --按钮监听
    --logic.cs.UIEventListener.AddOnClickListener(self.RomanceTab.gameObject,function(data) self:RomanceTabClick(data) end);

end

--endregion


--region 【OnClose】

function UIPersonalCenterForm:OnClose()
    UIView.OnClose(self)

    --logic.cs.UIEventListener.RemoveOnClickListener(self.RomanceTab.gameObject,function(data) self:RomanceTabClick(data) end);

end

--endregion

--region 【点击Others】
function UIPersonalCenterForm:RomanceTabClick(data)
    
end
--endregion


--region 【界面关闭】
function UIPersonalCenterForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end
--endregion



return UIPersonalCenterForm
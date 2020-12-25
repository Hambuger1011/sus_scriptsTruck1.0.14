local UIView = core.UIView


---@class UIPakageForm
local UIPakageForm = core.Class("UIPakageForm", UIView)

local uiid = logic.uiid;
UIPakageForm.config = {
    ID = uiid.UIPakageForm,
    AssetName = 'UI/Resident/UI/UIPakageForm'
}

local PakageItem = require('Logic/UI/UI_Pakage/PakageItem')
local PakageItemDetailPanel = require('Logic/UI/UI_Pakage/PakageItemDetailPanel')

-- local ServerIcon_LocalIcon_Map={
--     Outfit_Discount_10 = "PakageForm/props_icon_10_outfit_discount",
--     Outfit_Discount_25 = "PakageForm/props_icon_25_outfit_discount",
--     Choice_Discount_10 = "PakageForm/props_icon_10_choice_discount",
--     Choice_Discount_25 = "PakageForm/props_icon_25_choice_discount",
--     Outfit_Coupon = "pakageForm/props_icon_outfit_coupon",
--     Choice_Coupon = "PakageForm/props_icon_choice_coupon",
--     Key_Coupon = "PakageForm/props_icon_key_oupon",
--     Messenger_Dove = "PakageForm/com_icon_messenger_dove",
-- }

function UIPakageForm:OnInitView()

    UIView.OnInitView(self)
    self.gameObject = self.uiform.gameObject
    self.transform = self.uiform.transform

    self.objItemDetailPanel = CS.DisplayUtil.FindChild(self.gameObject, "BG/Panel/PropDetail")
    self.mItemDetailPanel = PakageItemDetailPanel.New(self.objItemDetailPanel,self);
    self.objItemDetailPanel:SetActive(false)
    self.transParent = CS.DisplayUtil.FindChild(self.gameObject, "BG/Panel/PropsList/Viewport/Layout").transform
    self.prefabItem = CS.DisplayUtil.FindChild(self.gameObject, "BG/Panel/PropsList/Viewport/Layout/Item")
    self.prefabItem:SetActive(false)
    self.objNoProp = CS.DisplayUtil.FindChild(self.gameObject, "BG/Panel/NoProp")
    self.objNoProp:SetActive(false)
    self.objPropsList = CS.DisplayUtil.FindChild(self.gameObject, "BG/Panel/PropsList")

    self.btnClose = logic.cs.LuaHelper.GetComponent(self.transform, "BG/TopTile/CloseBtn",typeof(logic.cs.Button))
    self.btnClose.onClick:AddListener(function()
        self:OnExitClick()
    end)

    self.propListData = {}
    self.itemList = {}
    self.iconCaches = self:InitIconCaches()

    self:GetBackpackPropRequest()
    self.id = 0
end

function UIPakageForm:InitIconCaches()
    local go = CS.DisplayUtil.FindChild(self.gameObject, "BG/Panel/iconcaches/")
    local trans = go.transform
    local get = logic.cs.LuaHelper.GetComponent
    local map={}
    map.Outfit_Discount_10 = get(trans,"10_outfit_discount",typeof(logic.cs.Image)).sprite
    map.Outfit_Discount_25 = get(trans,"25_outfit_discount",typeof(logic.cs.Image)).sprite
    map.Choice_Discount_10 = get(trans,"10_choice_discount",typeof(logic.cs.Image)).sprite
    map.Choice_Discount_25 = get(trans,"25_choice_discount",typeof(logic.cs.Image)).sprite
    map.Outfit_Coupon = get(trans,"outfit_coupon",typeof(logic.cs.Image)).sprite
    map.Choice_Coupon = get(trans,"choice_coupon",typeof(logic.cs.Image)).sprite
    map.Key_Coupon = get(trans,"key_oupon",typeof(logic.cs.Image)).sprite
    map.Messenger_Dove = get(trans,"messenger_dove",typeof(logic.cs.Image)).sprite
    go:SetActive(false)
    return map
end

function UIPakageForm:OnOpen()
    UIView.OnOpen(self)
end

function UIPakageForm:OnClose()
    UIView.OnClose(self)
end

function UIPakageForm:OnExitClick()
    for k,v in pairs(self.itemList) do
        v:Dispose()
    end
    self.itemList={}
    self.mItemDetailPanel:Dispose()

    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end

function UIPakageForm:SetData(proplist)
    self.propListData = proplist or {}
    self:RefreshUI()
    self:RefreshEnterRead()
end

function UIPakageForm:RefreshUI()
    if #self.propListData == 0 then
        self.objNoProp:SetActive(true)
        self.objPropsList:SetActive(false)
        return
    end
    self.objNoProp:SetActive(false)
    self.objPropsList:SetActive(true)

    --refresh list
    for k,v in pairs(self.itemList) do
        v:Dispose()
    end
    self.itemList = {}
    for i,v in ipairs(self.propListData) do
        self:AddItem(v)
    end
end

function UIPakageForm:GetIconSprite(serverIconName)
    local sprite = self.iconCaches[serverIconName]
    if not sprite then
        logic.debug.LogError("lzh ===> 未配置的资源名:" .. tostring(serverIconName))
        return nil
    end
    return sprite
end

function UIPakageForm:AddItem(itemData)
    local go = logic.cs.GameObject.Instantiate(self.prefabItem,self.transParent,false)
    go:SetActive(true)
    local item = PakageItem.New(go, self)
    item:SetData(itemData)
    self.itemList[itemData.id] = item
end

function UIPakageForm:ShowItemDetail(itemData, deltaTime)
    if itemData == nil then
        logic.debug.LogError("UIPakageForm:ShowItemDetail " .. tostring(itemData))
        return
    end
    self.mItemDetailPanel.gameObject:SetActive(true)
    self.mItemDetailPanel:SetData(itemData, deltaTime)

end


function UIPakageForm:RemoveItem(itemData)
    if itemData == nil then
        return
    end
    local item = self.itemList[itemData.id]
    self.itemList[itemData.id] = nil
    if item then
        item:Dispose()
    end
    -- for i,v in ipairs(self.propListData) do
    --     if v.id == itemData.id then
    --         table.remove(self.propListData, i)
    --         break
    --     end
    -- end
    -- self:RefreshUI()
end

function UIPakageForm:GetBackpackPropRequest()
    logic.gameHttp:GetBackpackProp(
        function(result)
            -- logic.debug.LogError(result)
            local info = core.json.Derialize(result) or {}
            local data = info.data or {}
            local prop_list = data.prop_list or {}
            self:SetData(prop_list)
        end)
end
function UIPakageForm:SetPropReadRequest(id, callback)
    logic.gameHttp:SetPropRead(id,
        function(result)
            -- logic.debug.LogError(result)
            local info = core.json.Derialize(result) or {}
            if info.code == 200 then
                callback()
            end
        end)
end
function UIPakageForm:SetPropByTypeRequest()
    logic.gameHttp:SetPropByType(
        function(result)
            -- logic.debug.LogError(result)
        end)
end

--刷新入口红点
function UIPakageForm:RefreshEnterRead()
    local isShow = 0
    for i,v in ipairs(self.propListData or {}) do
        if v.is_read == 0 then--道具是否已读 1：已读 0未读
            isShow = 1
            break
        end
    end
    logic.cs.EventDispatcher.Dispatch(logic.cs.EventEnum.PakageNumberShow, isShow);
end

return UIPakageForm
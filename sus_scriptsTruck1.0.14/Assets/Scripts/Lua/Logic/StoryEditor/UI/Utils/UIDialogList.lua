local class = core.Class

---@class UIDialogList
local UIDialogList = class('UIDialogList')

---@class UIDialogList.ItemEntity
UIDialogList.ItemEntity = class('UIDialogList.ItemEntity')
function UIDialogList.ItemEntity:__init(msgBoxType, viewCtrl)
    self.gameObject = viewCtrl.gameObject
    self.transform = viewCtrl.transform
    self.msgBoxType = msgBoxType
    self.viewCtrl = viewCtrl
    self.isDirty = false   --布局是否有变化
    self.pData = nil
    self.isTweening = false
end

---@class UIDialogList.ItemData
UIDialogList.ItemData = class('UIDialogList.ItemData')
function UIDialogList.ItemData:__init(msgBoxType)
    self.msgBoxType = msgBoxType
    self.isDirty = true   --数据是否有变化
    self.leftTop = 0
    self.size = core.Vector2.New(750,100)
    self.pEntity = nil
    self.isTweening = false
end


function UIDialogList:__init(gameObject,spacing)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.cs_layout = gameObject:GetComponent(typeof(logic.cs.LuaLayoutGroup))
    --inspector里的transform属性置灰
    --self.m_Tracker = CS.UnityEngine.DrivenRectTransformTracker()

    self.CalculateLayoutInputVertical = function()
        --self.m_Tracker:Clear()
    end
    self.SetLayoutHorizontal = function()
    end
    self.SetLayoutVertical = function()
    end
    self.onDestory = function()
        self:Delete()
    end
    self.onEnable = function()
        self.activeInHierarchy = true
    end
    self.onDisable = function()
        self.activeInHierarchy = false
        --self.m_Tracker:Clear()
    end
    self.cs_layout:Initialize(self)
    
    self.activeInHierarchy = gameObject.activeInHierarchy
    self.m_isDirty = true
    
    self.m_lastOffset = nil

    ---@type UIDialogList.ItemEntity[]
    self.m_useItems = {}

    ---@type UIDialogList.ItemEntity[]
    self.m_unuseItems = {}

    ---@type UIDialogList.ItemData[]
    self.itemDataList = {}

    if spacing then
        self.spacing = spacing
    else
        self.spacing = 40
    end
    self.scrollView = self.transform:GetComponentInParent(typeof(logic.cs.ScrollRect))
    -- self.scrollView.onValueChanged:AddListener(function(normalizedPosition)
    --     self:Update()
    -- end)
    self.updateHandle = LateUpdateBeat:Add(self.Update,self)
end

function UIDialogList:__delete()
    if self.updateHandle ~= nil then
        core.UpdateBeat:Remove(self.updateHandle)
        self.updateHandle = nil
    end
end

function UIDialogList:MarkDirty()
    self.m_isDirty = true
end

function UIDialogList:Update()
    if not self.activeInHierarchy then
        return
    end
    local offset = self:GetContentOffset()
    if self.m_lastOffset ~= offset then
        self.m_lastOffset = offset
        self:MarkDirty()
    end
    if self.m_isDirty then
        self.m_isDirty = false
        self:Refresh(offset)
    end
end

function UIDialogList:GetViewSize()
    local viewSize = self.scrollView.viewport.rect.size
    return viewSize
end

function UIDialogList:GetContentOffset()
    local offset = self.transform.anchoredPosition
    return offset.y
end

function UIDialogList:SetContentOffset(y)
    local offset = core.Vector2.New(0,y)
    self.transform.anchoredPosition = offset
    --logic.debug.LogError('y = '..y)
end

function UIDialogList:Refresh(offset)
    -- if offset.y < 0 then
    --     return
    -- end
    local maxIndex = -1
    local minIndex = #self.itemDataList

    local isDirty = false

    --region 计算可见item
    local viewSize = self:GetViewSize()
    for i = 1, #self.itemDataList do
        local data = self.itemDataList[i]
        if self:IsVisiable(data,viewSize,offset) then
            minIndex = math.min(minIndex, i)
            maxIndex = math.max(maxIndex, i)
        end
    end

    --回收不可见、布局变化的item
    for i = #self.m_useItems, 1, -1 do
        local itemEntity = self.m_useItems[i]
        local index = itemEntity.pData.index
        if itemEntity.isTweening then
            goto continue
        end
        --数据有刷新或不可见，进行回收
        if itemEntity.isDirty or (index < minIndex or index > maxIndex) then
            self:Recycle(i)
            isDirty = true
        end
        ::continue::
    end

    --生成可见的item
    for i = minIndex,maxIndex do
        for _,item in pairs(self.m_useItems) do
            if item.pData.index == i then
                goto continue
            end
        end

        local itemData = self.itemDataList[i]
        local msgBoxType = itemData.msgBoxType
        ---@type UIDialogList.ItemEntity
        local itemEntity = self:PopPool(itemData)
        if itemEntity == nil then
            local viewCtrl = self.onCreateItem(i,itemData)
            itemEntity = UIDialogList.ItemEntity.New(msgBoxType, viewCtrl)
        end
        
        itemEntity.pData = itemData
        itemData.pEntity = itemEntity
        itemEntity.isDirty = true
        itemData.isDirty = true
        table.insert(self.m_useItems, itemEntity)
        itemEntity.viewCtrl:OnLinkItem(itemData)--刷新数据到UI
        --itemEntity.viewCtrl:BindData()
        isDirty = true
        ::continue::
    end

    --endregion

    --计算item rect
    local reLayoutIndex = -1
    for i = #self.m_useItems, 1, -1 do
        ---@type UIDialogList.ItemEntity
        local itemEntity = self.m_useItems[i]
        ---@type UIDialogList.ItemData
        local itemData = itemEntity.pData
        local idx = itemData.index
        logic.debug.Asset(itemData~=nil,'itemdata is nil:'..idx)
        if itemEntity.isDirty or itemData.isDirty then
            itemEntity.isDirty = false
            itemEntity.viewCtrl:BindData()
        end

        if itemData.isDirty then
            itemData.isDirty = false
            isDirty = true
            local size = itemEntity.viewCtrl:GetSize()
            itemData.size = size
            if reLayoutIndex == -1 then
                reLayoutIndex = idx
            else
                reLayoutIndex = math.min(reLayoutIndex, idx)
            end
        end
    end

    --调整content
    if reLayoutIndex > 0 then
        local height, diff = self:RelayoutAt(reLayoutIndex)
        self:SetHeight(height)
        if not self.isTweening then
            local pos = self:GetContentOffset()
            pos = pos + diff
            self:SetContentOffset(pos)
            --logic.debug.LogError(diff)
        end
    end

    --设置item 位置
    if isDirty then
        for i = 1, #self.m_useItems do
            local itemEntity = self.m_useItems[i]
            local itemData = itemEntity.pData
            -- self.m_Tracker:Add(
            --         self.cs_layout, 
            --         itemEntity.transform.transform,
            --         CS.UnityEngine.DrivenTransformProperties.Anchors |
            --         CS.UnityEngine.DrivenTransformProperties.AnchoredPositionY
            --         )
            --itemEntity.transform:SetAsLastSibling()
            --设置leftop、size
            itemEntity.transform:SetInsetAndSizeFromParentEdge(logic.cs.RectTransform.Edge.Top, itemData.leftTop, itemData.size.y)
            --itemEntity.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Vertical, itemData.size.y)
        end
        CS.UnityEngine.UI.LayoutRebuilder.MarkLayoutForRebuild(self.transform)
    end
    return isDirty
end

---@param itemData UIDialogList.ItemData
function UIDialogList:IsVisiable(itemData,viewSize,viewOffset)
    local leftTop = itemData.leftTop
    leftTop = viewOffset - leftTop
    
    local rightBottom = leftTop - itemData.size.y

    if (rightBottom <= 0 and leftTop >= -viewSize.y) then
        return true
    end
    return false
end

function UIDialogList:Recycle(index)
    local item = self.m_useItems[index]
    item.viewCtrl:OnUnLinkItem()
    table.remove(self.m_useItems, index)
    table.insert(self.m_unuseItems, item)
    local itemData = item.pData
    item.pData = nil
    itemData.pEntity = nil
    --item.viewCtrl = nil
end

function UIDialogList:PopPool(itemData)
    local msgBoxType = itemData.msgBoxType
    local itemEntity = nil
    for i = #self.m_unuseItems, 1, -1 do
        ---@type UIDialogList.ItemEntity
        local item = self.m_unuseItems[i]
        if item.msgBoxType == msgBoxType then
            table.remove(self.m_unuseItems, i)
            itemEntity = item
            break
        end
    end
    return itemEntity
end

function UIDialogList:SetHeight(height, scrollToBottom, useTween)
    self.contentHeight = height
    self.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Vertical, height)
    if scrollToBottom then
        if useTween then
            core.tween.DoTween.Kill(self.scrollView)
            self.isTweening = true
            self.scrollView:DOVerticalNormalizedPos(0, 0.5):SetEase(core.tween.Ease.Flash):OnComplete(function()
                self.isTweening = false
            end):SetId(self.scrollView)
        else
            self:MoveToIndex(#self.itemDataList)
        end
    end
end

---@param itemData UIDialogList.ItemData
local NewItem = function(self,itemData,index)
    local msgBoxType = itemData.msgBoxType
    ---@type UIDialogList.ItemEntity
    local itemEntity = self:PopPool(itemData)
    if itemEntity == nil then
        local viewCtrl = self.onCreateItem(index,itemData)
        itemEntity = UIDialogList.ItemEntity.New(msgBoxType, viewCtrl)
    end
    
    itemEntity.pData = itemData
    itemData.pEntity = itemEntity
    itemEntity.isDirty = false
    itemEntity.isTweening = true
    itemData.isDirty = false
    table.insert(self.m_useItems, itemEntity)
    itemEntity.viewCtrl:OnLinkItem(itemData)
    itemEntity.viewCtrl:BindData()

    local size = itemEntity.viewCtrl:GetSize()
    itemData.size = size
    return itemEntity
end

---添加虚拟条目，newInstance=false 不会实例化, newInstance=true立即创建实例
function UIDialogList:AddVirtualItem(msgBoxType, newInstance)
    local itemData = UIDialogList.ItemData.New(msgBoxType)
    local i = #self.itemDataList
    itemData.index = i + 1
    table.insert(self.itemDataList, itemData)

    ---@type UIDialogList.ItemEntity
    local itemEntity = nil
    if newInstance then
        itemEntity = NewItem(self, itemData, i)
    end
    local height = self:RelayoutAt(itemData.index)
    if itemEntity then
        itemEntity.transform:SetInsetAndSizeFromParentEdge(logic.cs.RectTransform.Edge.Top, itemData.leftTop, itemData.size.y)
    end
    return height
end


---插入虚拟条目，并不会实例化
function UIDialogList:InsertVirtualItem(msgBoxType, position, newInstance)
    local itemData = UIDialogList.ItemData.New(msgBoxType)
    itemData.index = position
    local i = #self.itemDataList
    --data.index = position
    table.insert(self.itemDataList, position, itemData)

    ---@type UIDialogList.ItemEntity
    local itemEntity = nil
    if newInstance then
        itemEntity = NewItem(self, itemData, i)
    end
    local height = self:RelayoutAt(position)
    if itemEntity then
        itemEntity.transform:SetInsetAndSizeFromParentEdge(logic.cs.RectTransform.Edge.Top, itemData.leftTop, itemData.size.y)
    end
    itemData.isDirty = true
    --itemEntity.isDirty = true
    return height
end

---删除指定item
function UIDialogList:RemoveItemAt(index)
    local itemData = self.itemDataList[index]
    if itemData == nil then
        return
    end
    table.remove(self.itemDataList, index)
    if itemData.pEntity then
        itemData.pEntity.isDirty = true
    end
    local height = self:RelayoutAt(index,true)
    return height
end

function UIDialogList:RelayoutAt(index, markDirty)
    local padding = self.cs_layout.padding
    local spacing = self.spacing
    local  y = padding.top
    local height = 0
    local diff = 0
    if index > 1 then
        local data = self.itemDataList[index - 1]
        y = data.leftTop + data.size.y + spacing
        height = data.leftTop + data.size.y
    end
    for i = index, #self.itemDataList do
        local data = self.itemDataList[i]
        if data == nil then
            logic.debug.LogError('error index:'..index)
            return
        end
        diff = y - data.leftTop
        data.index = i
        data.leftTop = y
        y = y + data.size.y + spacing
        height = data.leftTop + data.size.y
    end
    height = height + padding.bottom
    if markDirty then
        self:SetHeight(height)
        self:MarkDirty()
    end
    return height, diff
end

function UIDialogList:ClearItem()
    for i = #self.m_useItems, 1, -1 do
        self:Recycle(i)
    end
    self.itemDataList = {}
    self:SetHeight(0)
    self:MarkDirty()
end

function UIDialogList:MoveToIndex(index)
    
    self.scrollView:StopMovement()
    local viewSize = self:GetViewSize()

    
    self.isTweening = true
    local count = 100
    for i = 0, count do
        local maxOffset = self.contentHeight - viewSize.y
        if maxOffset < 0 then
            maxOffset = 0
        end
        -- local offset = self:GetContentOffset()
        -- local pos = core.Vector2.New(0,0)
        -- pos.y = offset.y
        -- if pos.y < 0 then
        --     pos.y = 0
        -- end
        ---@type UIDialogList.ItemData
        local itemData = self.itemDataList[index]
        if itemData == nil then
            break
        end
        local offset = math.min(maxOffset, itemData.leftTop)
        self:SetContentOffset(offset)
        --local offset = self:GetContentOffset()
        self.m_lastOffset = offset
        self:MarkDirty()
        if not self:Refresh(offset) then
            break
        end
    end
    self.isTweening = false
end

function UIDialogList:GetPadding()
    local padding = self.cs_layout.padding
    return padding
end

function UIDialogList:GetPaddingBottom()
    local padding = self.cs_layout.padding
    return padding.bottom
end

function UIDialogList:SetPaddingBottom(y)
    local padding = self.cs_layout.padding
    padding.bottom = y
    local index = #self.itemDataList
    if index == 0 then
        return
    end
    local height = self:RelayoutAt(index,true)
    self.scrollView:StopMovement()
    local viewSize = self:GetViewSize()

    local count = 100
    for i = 0, count do
        local maxOffset = self.contentHeight - viewSize.y
        if maxOffset < 0 then
            maxOffset = 0
        end
        self:SetContentOffset(maxOffset)
        --local offset = self:GetContentOffset()
        self.m_lastOffset = maxOffset
        self:MarkDirty()
        if not self:Refresh(maxOffset) then
            break
        end
    end
end

return UIDialogList
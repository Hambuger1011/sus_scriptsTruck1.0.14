local class = core.Class

---@class UIList
local UIList = class('UIList')

---@class UIList.Item
UIList.Item = class('UIList.Item')
function UIList.Item:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
end

function UIList:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.cs_layout = gameObject:AddComponent(typeof(logic.cs.LuaLayoutGroup))

    self.CalculateLayoutInputVertical = function()
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
    end
    self.cs_layout:Initialize(self)
    
    self.activeInHierarchy = gameObject.activeInHierarchy
    self.m_isDirty = true
    self.updateHandle = core.UpdateBeat:Add(self.Update,self)
    self.itemSize = core.Vector2.New(100,100)
    self.m_lastOffset = nil
    self.m_isVertical = true
    self.m_useItems = {}
    self.m_unuseItems = {}

    self.onLinkItem = function(index,item)
        error(self.gameObject.name..".onLinkItem")
    end
    self.onUnLinkItem = function(index,item)
        error(self.gameObject.name..".onUnLinkItem")
    end
    self.scrollView = nil
end

function UIList:__delete()
    core.UpdateBeat:Remove(self.updateHandle)
end

function UIList:MarkDirty()
    self.m_isDirty = true
end

function UIList:Update()
    if not self.activeInHierarchy then
        return
    end
    local offset = self.transform.anchoredPosition
    if self.m_lastOffset ~= offset then
        self.m_lastOffset = offset
        self:MarkDirty()
    end
    if self.m_isDirty then
        self:Refresh(offset)
        self.m_isDirty = false
    end
end

function UIList:Refresh(offset)
    local maxIndex = -1
    local minIndex = -1
    if self.m_isVertical then
    else
    end

    for i = #self.m_useItems, 1, -1 do
        if i < minIndex or i > maxIndex then
            self:Recycle(i)
        end
    end
    for i = minIndex,maxIndex do
        for _,item in pairs(self.m_useItems) do
            if item.index == i then
                goto continue
            end
        end
        local item = self:PopPool()
        if item == nil then
            item = self:CreateItem()
        end
        table.insert(self.m_useItems, item)
        self.onLinkItem(i,item)
        ::continue::
    end
end

function UIList:Recycle(index)
    local item = self.m_useItems[index]
    table.remove(self.m_useItems, index)
    table.insert(self.m_unuseItems, item)
    self.onUnLinkItem(index,item)
end

function UIList:PopPool()
    local index = #self.m_unuseItems
    if index == 0 then
        return
    end
    local item = self.m_unuseItems[index]
    table.remove(self.m_unuseItems, index)
    return item
end

function UIList:CreateItem()
    local go = logic.cs.GameObject.Instantiate(self.itemPfb,self.transform)
    local item = UIList.Item.New(go)
    return go
end

return UIList
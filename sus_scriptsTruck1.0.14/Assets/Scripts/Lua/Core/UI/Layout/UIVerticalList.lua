local class = core.Class

---@class UIVerticalList
local UIVerticalList = class('UIVerticalList')

function UIVerticalList:__init(gameObject)
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
end

function UIVerticalList:__delete()
    core.UpdateBeat:Remove(self.updateHandle)
end

function UIVerticalList:MarkDirty()
    self.m_isDirty = true
end

function UIVerticalList:Update()
    if not self.activeInHierarchy then
        return
    end
    if self.m_isDirty then
        self:Refresh()
        self.m_isDirty = false
    end
end

function UIVerticalList:Refresh()
end

return UIVerticalList
local BaseClass = core.Class
System.Collections.Generic.Dictionary = BaseClass("System.Collections.Generic.Dictionary")
local Dictionary = System.Collections.Generic.Dictionary
function Dictionary:__init()
    System.Collections.ICollection.implements(self)
    self._items = {}
    self._count = 0
end

function Dictionary:__delete()
    self._items = nil
end

function List:Count()
    return self._count
end

function Dictionary:CopyTo(array,startIndex)
end

function Dictionary:Get(key)
    return self._items[key]
end

function Dictionary:Keys()
    local keys = {}
    for key,value in pairs(self._items) do
        table.insert(keys,key)
    end
    return keys
end


function Dictionary:Values()
    local values = {}
    for key,value in pairs(self._items) do
        table.insert(values,value)
    end
    return values
end

function Dictionary:Add(key,value)
    self._count = self._count + 1
    self._items[key] = value
end


function Dictionary:Clear()
    self._items = {}
    self._count = 0
end

function Dictionary:ContainsKey(key)
    local isContains = (self._items[key] ~= nil)
    return isContains
end


function Dictionary:ContainsValue(value)
    local isContains = false
    for key,value in pairs(self._items) do
        if value == value then
            isContains = true
            break
        end
    end
    return isContains
end

function Dictionary:GetEnumerator()
    return pairs(self._items)
end

function Dictionary:Remove(key)
    if self._items[key] == nil then
        return false
    end
    self._items[key] = nil
    return true
end
return Dictionary
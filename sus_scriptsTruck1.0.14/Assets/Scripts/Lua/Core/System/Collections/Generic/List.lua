local BaseClass = core.Class
System.Collections.Generic.List = BaseClass("List")
local List = System.Collections.Generic.List
function List:__init()
    System.Collections.ICollection.implements(self)
    System.Collections.IList.implements(self)
    self._items = {}
    self._count = 0
end

function List:__delete()
    self._items = nil
end

function List:Count()
    return self._count
end

function List:CopyTo(array,startIndex)
end

function List:Get(index)
    if index < 0 or index >= self._count then
        error(string.format('数字index超出范围:[%d,%d),index=%d',0,self._count,index))
        return nil
    end
    index = index + 1
    return self._items[index]
end

function List:Add(item)
    self._count = self._count + 1
    table.insert(self._items,item)
end

function List:AddRange(o,type)
    if type == 'table' then
        for i,item in pairs(o) do
            self:Add(item)
        end
    else
        error('未知类型:'..type)
    end
end

function List:Clear()
    self._items = {}
    self._count = 0
end

function List:Contains(item)
    local isContains = (self:IndexOf(item) ~= -1)
    return isContains
end

function List:IndexOf(item)
    local index = -1
    for i,v in pairs(self._items) do
        if v == item then
            index = i - 1
            return
        end
    end
    return index
end

function List:Insert(index,item)
    table.insert(self._items,index,item)
end

function List:Remove(item)
    local index = self:IndexOf(item)
    if index ~= -1 then
        table.remove(self._items, index + 1)
        return true
    end
    return false
end


function List:RemoveAt(index)
    if index < 0 or index >= self._count then
        error(string.format('数字index超出范围:[%d,%d),index=%d',0,self._count,index))
        return nil
    end
    index = index + 1
    table.remove(self._items,index)
end

function List:GetEnumerator()
    return pairs(self._items)
end

return List
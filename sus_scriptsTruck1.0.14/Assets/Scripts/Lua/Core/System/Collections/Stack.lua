local Class = core.Class
local Stack = Class("Stack")

function Stack:__init()
    self._array = {}
end

function Stack:Clear()
    self._array = {}
end

function Stack:Contains(t)
    for k,v in pairs(self._array) do
        if v == t then
            return true
        end
    end
    return false
end

function Stack:Peek()
    local len = #self._array
    if len == 0 then
        return nil
    end
    return self._array[len]
end


function Stack:Pop()
    local t = self:Peek()
    if t then
        table.remove(self._array, #self._array)
    end
end

function Stack:Push(t)
    table.insert(self._array, t)
end

function Stack:Count()
    return #self._array
end

return Stack
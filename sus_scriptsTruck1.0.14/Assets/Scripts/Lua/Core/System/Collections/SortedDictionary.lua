local Class = core.Class
local SortedDictionary = Class("SortedDictionary")

---@class SortedDictionary
function SortedDictionary:__init(comparer)
    --System.Collections.ICollection.implements(self)
    if comparer == nil then
        comparer = function(x,y)
            return x.Key - y.Key
        end
    end
    self.comparer = comparer
    self.array = {}
    self.dicationary = {}
end

function SortedDictionary:__delete()
    self.comparer = nil
    self.array = nil
    self.dicationary = nil
end

local Find = function(this, kv)
    local table = this.array;
    local len = #table;

    if (len == 0) then
        return 1
    end

    local left = 1;
    local right = len;

    while (left <= right) do
        local guess = math.floor((left + right) / 2)

        local cmp = this.comparer(table[guess], kv)
        if (cmp == 0) then
            return guess
        end

        if (cmp <  0) then 
            left = guess+1
        else 
            right = guess-1
        end
    end
    return left
end

function SortedDictionary:Count()
    return #self.array
end

function SortedDictionary:CopyTo(array, index)
    error('SortedDictionary:CopyTo')
end

function SortedDictionary:Add(key, value)
    if not key then
        error('key is nill')
        return
    end
    self:Set(key,value)
end

function SortedDictionary:Set(key,value)
    if not key then
        error('key is nill')
        return
    end
    local index = self.dicationary[key]
    if index == nil then
        local keyValuePair = {
            Key = key,
            Value = value
        }
        local pos = Find(self, keyValuePair)
        --logic.debug.Log('pos:'..pos)
        table.insert(self.array, pos, keyValuePair)
        for i = pos, #self.array do
            local item = self.array[i]
            self.dicationary[item.Key] = i
        end
    else
        self.array[index].Value = value
    end
end

function SortedDictionary:Get(key)
    if not key then
        error('key is nill')
        return
    end
    local index = self.dicationary[key]
    if index == nil then
        return nil
    end
    return self:GetByIndex(index)
end

function SortedDictionary:GetByIndex(index)
    local kv = self:GetKVByIndex(index)
    if kv then
        return kv.Value
    end
    return nil
end

function SortedDictionary:GetKVByIndex(index)
    local kv = self.array[index]
    return kv
end

function SortedDictionary:Remove(key)
    if not key then
        error('key is nill')
        return
    end
    local index = self.dicationary[key]
    if index == nil then
        return nil
    end
    local keyValuePair = self.array[index]
    table.remove(self.array,index)
    self.dicationary[key] = nil
    return keyValuePair
end

function SortedDictionary:Clear()
    self.array = {}
    self.dicationary = {}
end

function SortedDictionary:ContainsKey(key)
    if not key then
        error('key is nill')
        return
    end
    local index = self.dicationary[key]
    if index == nil then
        return false
    end
    return true
end

function SortedDictionary:Foreach(func)
    for i,kv in pairs(self.array) do
        func(i,kv)
    end
end

function SortedDictionary:DumpKeys()
    local ret = ''
    for i = 1,#self.array do
        ret = ret ..self.array[i].Key..','
    end
    logic.debug.Log(ret)
end

function SortedDictionary.UnitTest()
	local map = System.Collections.SortedDictionary.New(function(a,b)
		return b.Key - a.Key
	end)
	for i = 1,10 do
		map:Add(i,i)
		map:DumpKeys()
    end
    map:Clear()
    map:DumpKeys()
    for i = 1,10 do
        local v = math.random()
		map:Add(v,v)
		map:DumpKeys()
    end
    -- local itr  = map:GetEnumerator()
    -- logic.debug.Log(itr())
    -- for i,kv in itr() do
    --     logic.debug.Log(i)
    -- end
end

return SortedDictionary
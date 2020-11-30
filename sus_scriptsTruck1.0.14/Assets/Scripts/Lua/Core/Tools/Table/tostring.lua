
local _list_table = nil
_list_table = function(tb, table_list, level)
    local ret = ""
    local indent = string.rep(" ", level*4)

    for k, v in pairs(tb) do
        local quo = type(k) == "string" and "\"" or ""
        ret = ret .. indent .. "[" .. quo .. tostring(k) .. quo .. "] = "

        if type(v) == "table" then
            local t_name = table_list[v]
            if t_name then
                ret = ret .. tostring(v) .. " -- > [\"" .. t_name .. "\"]\n"
            else
                table_list[v] = tostring(k)
                ret = ret .. "{\n"
                ret = ret .. _list_table(v, table_list, level+1)
                ret = ret .. indent .. "},\n"
            end
        elseif type(v) == "string" then
            ret = ret .. "\"" .. tostring(v) .. "\",\n"
        else
            ret = ret .. tostring(v) .. ",\n"
        end
    end

    local mt = getmetatable(tb)
    if mt then 
        ret = ret .. "\n"
        local t_name = table_list[mt]
        ret = ret .. indent .. "<metatable> = "

        if t_name then
            ret = ret .. tostring(mt) .. " -- > [\"" .. t_name .. "\"]\n"
        else
            ret = ret .. "{\n"
            ret = ret .. _list_table(mt, table_list, level+1)
            ret = ret .. indent .. "},\n"
        end
        
    end

   return ret
end

-------------------------------------------------------------------
-- Public functions
-------------------------------------------------------------------

core.table.tostring = function(tb)
    if type(tb) ~= "table" then
        error("Sorry, it's not table, it is " .. type(tb) .. ".")
    end

    local ret = "{\n"
    local table_list = {}
    table_list[tb] = "root table"
    ret = ret .. _list_table(tb, table_list, 1)
    ret = ret .. "}"
    return ret
end

core.table.print = function(name, tb)
    return(name.."["..tostring(tb).."]" .. core.table.tostring(tb))
end
--[[
-------------------------------------------------------------------
-- For test
-------------------------------------------------------------------

local test_table_2 = {
    print,
}

local test_table_1 = {
    12, 22.5, true, 
    info = {
        name = "Jack", age = 26,
        lifeexp = {
            ["1986"] = "Both",
            ["2013"] = "Work in Tencent",
            ["2015"] = "Get married"
        }, 
        wife = "Lucy"
    },
    "Hello test",
    recu_table = test_table_2,
    ["2"] = 13
}

test_table_2.recu_table = test_table_1

local metatable = {
    __index = test_table_2,    
    __add = function(a, b) return 0 end
}

setmetatable(test_table_1, metatable)

function table_lib_test()
    table_print(test_table_1)
end

]]
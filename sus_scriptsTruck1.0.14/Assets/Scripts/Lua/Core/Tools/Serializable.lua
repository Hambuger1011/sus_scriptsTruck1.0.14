local BaseClass = core.Class
local Serializable = BaseClass("Serializable")

function Serializable:__init()
end

local SerializeTable = nil

SerializeTable = function(tb, table_list, level)
    local ret = ""
    local indent = string.rep(" ", level*4)

    ret = ret .. indent .. "__id = \"" .. tostring(tb) .. "\",\n"
    for k, v in pairs(tb) do

        if k == "__class" then
            ret = ret .. indent .. "__class = \"" .. v.__cname .. "\",\n"
            goto continue
        elseif type(v) == "function" then
            --ret = ret .. "func,\n"
            goto continue
        end

        if type(k) == 'number' then
            ret = ret .. indent .. "[" .. tostring(k) .. "] = "
        else
            ret = ret .. indent .. tostring(k) .. " = "
        end

        if type(v) == "table" then
            local t_name = nil--table_list[v]
            if t_name then
                ret = ret .. tostring(v) .. " -- > [\"" .. t_name .. "\"]\n"
            else
                table_list[v] = tostring(k)
                ret = ret .. "{\n"
                ret = ret .. SerializeTable(v, table_list, level+1)
                ret = ret .. indent .. "},\n"
            end
        elseif type(v) == "string" then
            ret = ret .. "\"" .. tostring(v) .. "\",\n"
        else
            ret = ret .. tostring(v) .. ",\n"
        end
        ::continue::
    end

    -- local mt = getmetatable(tb)
    -- if mt then 
    --     ret = ret .. "\n"
    --     local t_name = table_list[mt]
    --     ret = ret .. indent .. "<metatable> = "

    --     if t_name then
    --         ret = ret .. tostring(mt) .. " -- > [\"" .. t_name .. "\"]\n"
    --     else
    --         ret = ret .. "{\n"
    --         ret = ret .. SerializeTable(mt, table_list, level+1)
    --         ret = ret .. indent .. "},\n"
    --     end
        
    -- end

   return ret
end


local DeserializeTable = nil
DeserializeTable = function(tb,table_list,newObjFunc)
    table_list["__id"] = tb.__id
    
    local obj = nil
    if tb.__class then
        obj = newObjFunc(tb.__class)
    else
        obj = {}
    end
    for k, v in pairs(tb) do
        if k == "__id" or k == "__class" then
            goto continue
        end
        if type(v) == "table" then
            obj[k] = DeserializeTable(v,table_list,newObjFunc)
        else
            obj[k] = v
        end
        ::continue::
    end
    return obj
end

function Serializable:Serialize(tb)

    local ret = "{\n"
    local table_list = {}
    table_list[tb] = "root table"
    ret = ret .. SerializeTable(tb, table_list, 1)
    ret = ret .. "}"
    return ret
end

function Serializable:Deserialize(data,newObjFunc)
    local tb = load("return "..data)()
    local table_list = {}
    return DeserializeTable(tb,table_list,newObjFunc)
end

return Serializable.New()
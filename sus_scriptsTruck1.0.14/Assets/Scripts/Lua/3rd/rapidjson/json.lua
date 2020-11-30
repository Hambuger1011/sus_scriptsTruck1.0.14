--local rapidjson = require 'rapidjson'
local jsonlib = require('3rd/json/Json4Lua')
local M = {}

function M.Serialize(obj)
    --local str = rapidjson.encode(obj)
    local str = jsonlib.encode(obj)
    return str
end


function M.Derialize(json)
    if string.IsNullOrEmpty(json) then
        return nil
    end
    --local obj = rapidjson.decode(json)
    local obj = jsonlib.decode(json)
    return obj
end

return M
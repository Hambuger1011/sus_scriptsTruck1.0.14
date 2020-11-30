--[[
-- 服务器数据
--]]

local BaseClass = core.Class
local ServerData = BaseClass("ServerData")

function ServerData:GetDisjunctor(callback)
    -- logic.gameHttp.GetDisjunctor(function(result)
    --     logic.debug.LogError(result)
    --     callback()
    -- end)
    callback()
end

return ServerData.New()

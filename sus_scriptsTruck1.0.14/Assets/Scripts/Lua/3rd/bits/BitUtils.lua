local bit = require("3rd/bits/numberlua")
local M = {}
M.bit = bit

function M.GetInt32Mask(x,y)
    assert(x < 65536 and y < 65536)
    local high = bit.lshift(x,15)
    local ret = bit.bor(high,y)
    return ret
end

function M.PaseInt32Mask(i)
    local x = bit.rshift(i,15)
    local high = bit.lshift(x,15)
    local y = bit.bxor(i,high)
    return {x=x,y=y}
end

return M
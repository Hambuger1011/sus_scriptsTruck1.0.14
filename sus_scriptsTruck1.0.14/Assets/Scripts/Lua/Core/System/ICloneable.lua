System.ICloneable = {}
function System.ICloneable.implements(o)
    if o.__cname == nil then
        core.debug.LogError(string.format('%s不是class',o))
    end
    local functions = {
        'Clone',
    }
    for _,name in pairs(functions) do
        if type(o[name]) ~= 'function' then
            core.debug.LogError(string.format('%s未实现接口:%s',o.__cname,name))
        end
    end
end
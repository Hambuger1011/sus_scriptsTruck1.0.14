--[[
-- 客户端数据
--]]
local BaseClass = core.Class
local ClientData = BaseClass("ClientData")

function ClientData:__init()
    self.CONF_DIR = "Assets/Bundle/Data/Common/"
end

function ClientData:GetCfgList(name)
    local list = self['list_' .. name]
    if list then
        return list
    end

    local asset = logic.ResMgr.LoadImme(logic.ResMgr.tag.Null,logic.ResMgr.type.Text,self.CONF_DIR ..name..".bytes")
	local bytes = asset.resTextAsset.bytes
	asset:Release()
    local cfg = core.Protobuf.pb.decode('pb.'..name..'_List',bytes)
    list = cfg.items
    self['list_' .. name] = list
    return list
end

function ClientData:GetCfgMap(name,keyFunc)
    local map = self['map_' .. name]
    if map then
        return map
    end

    local list = self:GetCfgList(name)
    map = {}
    for i,v in pairs(list) do
        map[keyFunc(v)] = v
    end
    self['map_' .. name] = map
    return map
end

function ClientData:GetBookDetailByID(id)
    local map = self:GetCfgMap('t_BookDetails',function(cfg)
        return cfg.id
    end)
    local cfg = map[id]
    if not cfg then
        logic.debug.LogError('获取t_BookDetails失败:ID = '..id)
    end
    if not cfg.strCharacterNames then
        local json = string.gsub(cfg.BookCharacterName,'\'','\"')
        cfg.strCharacterNames = core.json.Derialize(json)
        if not cfg.strCharacterNames then
            logic.debug.LogError(string.format('json解析错误:%s',json))
        end
    end
    if not cfg.intChapterDivisionArray then
        local json = string.gsub(cfg.ChapterDivision,'\'','\"')
        cfg.intChapterDivisionArray = core.json.Derialize(json)
        if not cfg.intChapterDivisionArray then
            logic.debug.LogError(string.format('json解析错误:%s',json))
        end
    end
    return cfg
end

return ClientData.New()
require "Data/Config/Q_DressUpDataCfg"
-- 单例模式
local BaseClass = core.Class
local Q_DressUpData = BaseClass("Q_DressUpData", core.Singleton)

-- 构造函数
function Q_DressUpData:__init()
    self.dataCacheMap = {} -- 集合

    self.typelist={};
end


--加载数据
function Q_DressUpData:Load()
    local data =  GameData.Q_DressUpDataCfg;
    for key, value in ipairs(data) do
        self.dataCacheMap[value.Id]=value;
    end
end

--返回一组 信息
function Q_DressUpData:GetMapData(_id)
    local mapData = nil;
    for i, v in pairs(self.dataCacheMap) do
        if(v.Id == _id) then
            mapData=v;
            break;
        end
    end
    return mapData;
end


--通过类型 获取 一组集合 数据
function Q_DressUpData:GetInfoByType(_type)
    self.typelist={};
    for i, v in pairs(self.dataCacheMap) do
        if(v.frame_type == _type) then
            table.insert(self.typelist,v);
        end
    end
    return self.typelist;
end





-- 析构函数
function Q_DressUpData:__delete()

end


Q_DressUpData = Q_DressUpData:GetInstance()
return Q_DressUpData







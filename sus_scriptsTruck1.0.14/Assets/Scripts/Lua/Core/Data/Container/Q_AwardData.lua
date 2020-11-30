require "Data/Config/Q_AwardDataCfg"
-- 单例模式
local BaseClass = core.Class
local Q_AwardData = BaseClass("Q_AwardData", core.Singleton)

-- 构造函数
function Q_AwardData:__init()
    self.dataCacheMap = {} -- 集合
end


--加载数据
function Q_AwardData:Load()
    local data =  GameData.Q_AwardDataCfg;
    for key, value in ipairs(data) do
        self.dataCacheMap[value.Id]=value;
    end
end

--返回一组 信息
function Q_AwardData:GetMapData(_id)
    local mapData = nil;
    for i, v in pairs(self.dataCacheMap) do
        if(v.Id == _id) then
            mapData=v;
            break;
        end
    end
    --table.sort(guideData,function(a,b) return a.GuideID<b.GuideID end)
    return mapData;
end

-- 析构函数
function Q_AwardData:__delete()

end


Q_AwardData = Q_AwardData:GetInstance()
return Q_AwardData







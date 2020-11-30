DataConfig={};

local isInit=false;
-- 构造函数
function DataConfig.InitConfig()
    --判断是否初始化
    if(isInit)then
        return;
    end
    DataConfig.Q_AwardData=require("Core/Data/Container/Q_AwardData");
    DataConfig.Q_DressUpData=require("Core/Data/Container/Q_DressUpData");
    isInit = true
    DataConfig.LoadConfig();
end


function DataConfig.LoadConfig()
    DataConfig.Q_AwardData:Load();
    DataConfig.Q_DressUpData:Load();
end
Cache={};

local isInit=false;
-- 构造函数
function Cache.Init()
    --判断是否初始化
    if(isInit)then
        return;
    end
    Cache.SignInCache=require("Logic/Cache/SignInCache");
    Cache.PropCache=require("Logic/Cache/PropCache");
    Cache.MainCache=require("Logic/Cache/MainCache");
    Cache.RedDotCache=require("Logic/Cache/RedDotCache");
    Cache.SearchCache=require("Logic/Cache/SearchCache");
    Cache.ReadTimeCache=require("Logic/Cache/ReadTimeCache");
    Cache.DressUpCache=require("Logic/Cache/DressUpCache");
    Cache.LimitTimeActivityCache=require("Logic/Cache/LimitTimeActivityCache");
    Cache.ActivityCache=require("Logic/Cache/ActivityCache");
    Cache.ComuniadaCache=require("Logic/Cache/ComuniadaCache");
    Cache.BusquedaCache=require("Logic/Cache/BusquedaCache");
    Cache.EmailCache=require("Logic/Cache/EmailCache");
    Cache.ChatCache=require("Logic/Cache/ChatCache");
    Cache.PopWindowCache=require("Logic/Cache/PopWindowCache");
    Cache.InvestmentCache=require("Logic/Cache/InvestmentCache");
    Cache.LotteryCache=require("Logic/Cache/LotteryCache");

    isInit = true
end

function Cache.ClearList(_list)
    if(_list)then
        local len=table.length(_list);
        if(len>0)then
            for i = 1, len do
                --销毁这个类
                if(_list[i])then
                    _list[i]:Delete();
                    _list[i]=nil;
                end
            end
            _list={};
        end
    end
end



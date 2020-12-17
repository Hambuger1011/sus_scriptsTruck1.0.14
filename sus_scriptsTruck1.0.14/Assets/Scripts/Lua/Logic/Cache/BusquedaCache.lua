-- 单例模式
local BaseClass = core.Class
local BusquedaCache = BaseClass("BusquedaCache", core.Singleton)


-- 构造函数
function BusquedaCache:__init()
    ---------------------【更多书本】
    --书本总数
    self.total=0;
    --总页数
    self.per_page=0;
    --当前页
    self.current_page=0;
    --搜索结果 列表
    self.BusquedaList= {};
    ---------------------【更多书本】
end


function BusquedaCache:UpdateList(datas)
    self.total=datas.total;
    self.per_page=datas.per_page;
    self.current_page=datas.current_page;

    local busquedaList=datas.data;
    local len=table.length(busquedaList);
    if(busquedaList and len>0)then
        for i = 1,len do
            local info =require("Logic/Cache/ComuniadaInfo/StoryInfo2").New();
            info:UpdateData(busquedaList[i]);
            table.insert(self.BusquedaList,info);
        end
    end
end

function BusquedaCache:GetInfoByIndex(Index)
    if(self.BusquedaList)then
        local len=table.length(self.BusquedaList);
        if(len>0 and Index)then
            if(len>=Index)then
                return self.BusquedaList[Index];
            else
                logic.debug.LogError("GetHotInfoByIndex Index :"..Index);
            end
        end
    end
    return nil;
end

function BusquedaCache:UpdateTypeList(datas)
    self.total=datas.total;
    self.per_page=datas.per_page;
    self.current_page=datas.current_page;

    Cache.ClearList(self.BusquedaList);
    local busquedaList=datas.data;
    local len=table.length(busquedaList);
    if(busquedaList and len>0)then
        for i = 1,len do
            local info =require("Logic/Cache/ComuniadaInfo/StoryInfo2").New();
            info:UpdateData(busquedaList[i]);
            table.insert(self.BusquedaList,info);
        end
    end
end



function BusquedaCache:ClearMas()
    if(GameHelper.islistHave(self.BusquedaList)==true)then
        Cache.ClearList(self.BusquedaList);
    end
end


-- 析构函数
function BusquedaCache:__delete()
end


BusquedaCache = BusquedaCache:GetInstance()
return BusquedaCache
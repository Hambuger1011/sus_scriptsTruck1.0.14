-- 单例模式
local BaseClass = core.Class
local ComuniadaCache = BaseClass("ComuniadaCache", core.Singleton)


-- 构造函数
function ComuniadaCache:__init()
    --最受欢迎书本
    self.mostPopularList= {};
    --最新更新
    self.latestReleaseList= {};


    ------------------------------------------【更多书本】
    --书本总数
    self.total=0;
    --总页数
    self.per_page=0;
    --当前页
    self.current_page=0;
    --最火
    self.allHotList= {};
    --最新更新
    self.allNewList= {};
    ------------------------------------------【更多书本】
    --自己创作故事列表
    self.myWriterList={};
    --历史看过的故事列表
    self.historyList={};
    ---------------------------------------------------
    --【火爆作者排名】
    self.HotWriterList={};
    ---------------------------------------------------
    --【作者详情】
    self.WriterInfo={};
    --【动态列表】
    self.DynamicList={};

    --作者创作故事列表
    self.WriterList={};
    --作者历史看过的故事列表
    self.WriterhistoryList={};
    ----------------------------------------------------

end

--最受欢迎书本
function ComuniadaCache:UpdateMostPopularList(datas)
    if(GameHelper.islistHave(self.mostPopularList)==true)then
        Cache.ClearList(self.mostPopularList);
    end
    local hotList=datas.hotList;
    local len=table.length(hotList);
    if(hotList and len>0)then
        for i = 1,len do
            local hotinfo =require("Logic/Cache/ComuniadaInfo/StoryInfo").New();
            hotinfo:UpdateData(hotList[i]);
            table.insert(self.mostPopularList,hotinfo);
        end
    end
end

--最新更新
function ComuniadaCache:UpdateLatestReleaseList(datas)
    if(GameHelper.islistHave(self.latestReleaseList)==true)then
        Cache.ClearList(self.latestReleaseList);
    end
    local newList=datas.newList;
    local len=table.length(newList);
    if(newList and len>0)then
        for i = 1,len do
            local newinfo =require("Logic/Cache/ComuniadaInfo/StoryInfo").New();
            newinfo:UpdateData(newList[i]);
            table.insert(self.latestReleaseList,newinfo);
        end
    end
end




function ComuniadaCache:UpdateHotList(datas)
    self.total=datas.total;
    self.per_page=datas.per_page;
    self.current_page=datas.current_page;

    local hotList=datas.data;
    local len=table.length(hotList);
    if(hotList and len>0)then
        for i = 1,len do
            local info =require("Logic/Cache/ComuniadaInfo/StoryInfo").New();
            info:UpdateData(hotList[i]);
            table.insert(self.allHotList,info);
        end
    end
end


function ComuniadaCache:UpdateNewList(datas)
    self.total=datas.total;
    self.per_page=datas.per_page;
    self.current_page=datas.current_page;

    local newList=datas.data;
    local len=table.length(newList);
    if(newList and len>0)then
        for i = 1,len do
            local info =require("Logic/Cache/ComuniadaInfo/StoryInfo").New();
            info:UpdateData(newList[i]);
            table.insert(self.allNewList,info);
        end
    end
end

function ComuniadaCache:GetHotInfoByIndex(Index)
    if(self.allHotList)then
        local len=table.length(self.allHotList);
        if(len>0 and Index)then
            if(len>=Index)then
                return self.allHotList[Index];
            else
                logic.debug.LogError("GetHotInfoByIndex Index :"..Index);
            end
        end
    end
    return nil;
end

function ComuniadaCache:GetNewInfoByIndex(Index)
    if(self.allNewList)then
        local len=table.length(self.allNewList);
        if(len>0 and Index)then
            if(len>=Index)then
                return self.allNewList[Index];
            else
                logic.debug.LogError("GetNewInfoByIndex Index :"..Index);
            end
        end
    end
    return nil;
end


function ComuniadaCache:ClearMas()
    if(GameHelper.islistHave(self.allHotList)==true)then
        Cache.ClearList(self.allHotList);
    end

    if(GameHelper.islistHave(self.allNewList)==true)then
        Cache.ClearList(self.allNewList);
    end
end



--自己创作故事列表
function ComuniadaCache:UpdateMyWriterList(datas)
    if(GameHelper.islistHave(self.myWriterList)==true)then
        Cache.ClearList(self.myWriterList);
    end
    local List=datas.data;
    local len=table.length(List);
    if(List and len>0)then
        for i = 1,len do
            local info =require("Logic/Cache/ComuniadaInfo/StoryInfo").New();
            info:UpdateData(List[i]);
            table.insert(self.myWriterList,info);
        end
    end
end

--历史看过的故事列表
function ComuniadaCache:UpdateHistoryList(datas)
    if(GameHelper.islistHave(self.historyList)==true)then
        Cache.ClearList(self.historyList);
    end
    local List=datas.data;
    local len=table.length(List);
    if(List and len>0)then
        for i = 1,len do
            local info =require("Logic/Cache/ComuniadaInfo/StoryInfo").New();
            info:UpdateData(List[i]);
            table.insert(self.historyList,info);
        end
    end
end


--最受欢迎作者
function ComuniadaCache:UpdateHotWriter(datas)
    if(GameHelper.islistHave(self.HotWriterList)==true)then
        Cache.ClearList(self.HotWriterList);
    end
    local List=datas.data;
    local len=table.length(List);
    if(List and len>0)then
        for i = 1,len do
            local info =require("Logic/Cache/ComuniadaInfo/AuthorInfo").New();
            info:UpdateData(List[i]);
            table.insert(self.HotWriterList,info);
        end
    end
end



--作者创作故事列表
function ComuniadaCache:UpdateWriterList(datas)
    if(GameHelper.islistHave(self.WriterList)==true)then
        Cache.ClearList(self.WriterList);
    end
    local List=datas.new_list;
    local len=table.length(List);
    if(List and len>0)then
        for i = 1,len do
            local info =require("Logic/Cache/ComuniadaInfo/StoryInfo").New();
            info:UpdateData(List[i]);
            table.insert(self.WriterList,info);
        end
    end
end

--作者历史看过的故事列表
function ComuniadaCache:UpdateWriterhistoryList(datas)
    if(GameHelper.islistHave(self.WriterhistoryList)==true)then
        Cache.ClearList(self.WriterhistoryList);
    end
    local List=datas.reading_list;
    local len=table.length(List);
    if(List and len>0)then
        for i = 1,len do
            local info =require("Logic/Cache/ComuniadaInfo/StoryInfo").New();
            info:UpdateData(List[i]);
            table.insert(self.WriterhistoryList,info);
        end
    end
end




-- 析构函数
function ComuniadaCache:__delete()
end


ComuniadaCache = ComuniadaCache:GetInstance()
return ComuniadaCache
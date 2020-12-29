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
    self.WriterInfo = {};
    --uid
    self.WriterInfo.uid = 0;
    --是否关注
    self.WriterInfo.is_follow = 0;
    --是否点赞
    self.WriterInfo.is_agree = 0;
    --个性签名
    self.WriterInfo.writer_sign = 0;
    --点赞人数
    self.WriterInfo.agree_count = 0;
    --粉丝数量
    self.WriterInfo.fans_count = 0;
    --最近更新书本时间
    self.WriterInfo.last_update_book_time = 0;
    --最近更新的创作书本id
    self.WriterInfo.last_update_book_id = 0;
    --作者创建时间
    self.WriterInfo.create_time = 0;
    --作者昵称
    self.WriterInfo.nickname = "";
    --头像
    self.WriterInfo.avatar = 0;
    --头像框
    self.WriterInfo.avatar_frame = 0;
    --评论框
    self.WriterInfo.comment_frame = 0;
    --评论框
    self.WriterInfo.barrage_frame = 0;
    --设备. 1安卓 2苹果
    self.WriterInfo.system_type = 0;
    --是否在线, 0否,1是
    self.WriterInfo.is_online = 0;
    --书本数量
    self.WriterInfo.book_count = 0;


    --【动态列表】
    self.DynamicList={};
    self.DynamicList_Count=0;
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


--region【刷新作者详情】
function ComuniadaCache:UpdateWriterInfo(datas)
    self.WriterInfo.is_follow = datas.is_follow;
    self.WriterInfo.is_agree =  datas.is_agree;
    self.WriterInfo.writer_sign =  datas.writer_sign;
    self.WriterInfo.agree_count =  datas.agree_count;
    self.WriterInfo.fans_count =  datas.fans_count;
    self.WriterInfo.last_update_book_time =  datas.last_update_book_time;
    self.WriterInfo.last_update_book_id =  datas.last_update_book_id;
    self.WriterInfo.create_time = datas.create_time;
    self.WriterInfo.nickname = datas.nickname;
    self.WriterInfo.avatar =  datas.avatar;
    self.WriterInfo.avatar_frame =  datas.avatar_frame;
    self.WriterInfo.comment_frame =  datas.comment_frame;
    self.WriterInfo.barrage_frame =  datas.barrage_frame;
    self.WriterInfo.system_type =  datas.system_type;
    self.WriterInfo.is_online = datas.is_online;
    self.WriterInfo.book_count =  datas.book_count;
end
--endregion


--作者历史看过的故事列表
function ComuniadaCache:UpdateDynamicList(datas)
    self.DynamicList_Count=datas.count;
    local List=datas.data;
    local len=table.length(List);
    if(List and len>0)then
        for i = 1,len do
            if(List[i])then
                table.insert(self.DynamicList,List[i]);
            end
        end
    end
end




--作者历史看过的故事列表
function ComuniadaCache:GetDynamicList(_index)
    if(GameHelper.islistHave(self.DynamicList)==true)then
        local len=table.length(self.DynamicList);
        if(len>=_index)then
            return self.DynamicList[_index];
        end
    end
    return nil;
end




-- 析构函数
function ComuniadaCache:__delete()
end


ComuniadaCache = ComuniadaCache:GetInstance()
return ComuniadaCache
-- 单例模式
local BaseClass = core.Class
local MainCache = BaseClass("MainCache", core.Singleton)


-- 构造函数
function MainCache:__init()
    --消息码
    self.code=0;

    --【周更新的书本】
    self.weekly_list={};
    --【首页推荐的书本】
    self.recommend_list={};
    --【我的书本】（即自己阅读过的书）
    self.mybook_list={};
    --【LGBT 列表】
    self.LGBT_list={};
    --【Romance 列表】
    self.Romance_list={};
    --【Suspense 列表】
    self.Suspense_list={};
    --最后阅读数本的节点
    self.final_book_info=nil;



    --【平台排行榜列表】
    self.platform_ranklist={};
    --【新书排行榜列表】
    self.newbook_ranklist={};
    --【人气排行榜列表】
    self.popularity_ranklist={};
end

--更新周更 和 推荐列表
function MainCache:UpdatedWeekly(datas)
    Cache.ClearList(self.weekly_list);
    Cache.ClearList(self.recommend_list);

    self.code=datas.code;
    --【周列表】
    local weeklyList=datas.data.updated_weekly_book
    local _len=table.length(weeklyList);
    if(weeklyList and _len>0)then
        for i = 1, _len do
            local weeklyinfo =require("Logic/Cache/MainInfo/WeeklyInfo").New();
            weeklyinfo:UpdateData(weeklyList[i]);
            table.insert(self.weekly_list,weeklyinfo)
        end
    end
    print(table.length(self.weekly_list))

    --【推荐列表】
    local recommendList=datas.data.recommend_book
    local len=table.length(recommendList);
    if(recommendList and len>0)then
        for i = 1, len do
            local recommendinfo =require("Logic/Cache/MainInfo/RecommendInfo").New();
            recommendinfo:UpdateData(recommendList[i]);
            table.insert(self.recommend_list,recommendinfo)
        end
    end
    print(table.length(self.recommend_list))

end


--更新书本排行数据 列表
function MainCache:UpdatedRankList(datas)

    --【平台排行榜列表】
    if(GameHelper.islistHave(self.platform_ranklist)==false)then
        local rankList=datas.data.platform_list
        local len=table.length(rankList);
        if(rankList and len>0)then
            for i = 1,len do
                local rankinfo =require("Logic/Cache/MainInfo/RankInfo").New();
                rankinfo:UpdateData(rankList[i]);
                table.insert(self.platform_ranklist,rankinfo)
            end
        end
    end
    --【新书排行榜列表】
    if(GameHelper.islistHave(self.newbook_ranklist)==false)then
        local rankList=datas.data.new_list
        local len=table.length(rankList);
        if(rankList and len>0)then
            for i = 1,len do
                local rankinfo =require("Logic/Cache/MainInfo/RankInfo").New();
                rankinfo:UpdateData(rankList[i]);
                table.insert(self.newbook_ranklist,rankinfo)
            end
        end
    end
    --【人气排行榜列表】
    if(GameHelper.islistHave(self.popularity_ranklist)==false)then
        local rankList=datas.data.popularity_list
        local len=table.length(rankList);
        if(rankList and len>0)then
            for i = 1,len do
                local rankinfo =require("Logic/Cache/MainInfo/RankInfo").New();
                rankinfo:UpdateData(rankList[i]);
                table.insert(self.popularity_ranklist,rankinfo)
            end
        end
    end

end

--更新我的书本列表
function MainCache:UpdatedMyBooks(datas)
    Cache.ClearList(self.mybook_list);

    self.code=datas.code;
    self.final_book_info=datas.data.final_book_info;
    --【我的书本】
    local mybookList=datas.data.favorite_book
    local len=table.length(mybookList);
    if(mybookList and len>0)then
        for i = 1, len do
            local mybookinfo =require("Logic/Cache/MainInfo/MyBookInfo").New();
            mybookinfo:UpdateData(mybookList[i]);
            table.insert(self.mybook_list,mybookinfo)
        end
    end
    print(table.length(self.mybook_list))
end


--更新LGBT 列表
function MainCache:UpdatedLGBTList(booklist)
    Cache.ClearList(self.LGBT_list);
    self.code=200;
    --【LGBT】
    local LGBTList=booklist;
    local len=table.length(LGBTList);
    if(LGBTList and len>0)then
        for i = 1,len do
            local bookinfo =require("Logic/Cache/MainInfo/BookTypeInfo").New();
            bookinfo:UpdateData(LGBTList[i]);
            table.insert(self.LGBT_list,bookinfo)
        end
    end
    print(table.length(self.LGBT_list))
end


--更新Romance 列表
function MainCache:UpdatedRomanceList(booklist)
    Cache.ClearList(self.Romance_list);

    self.code=200;
    --【Romance】
    local RomanceList=booklist;
    local len=table.length(RomanceList);
    if(RomanceList and len>0)then
        for i = 1,len do
            local bookinfo =require("Logic/Cache/MainInfo/BookTypeInfo").New();
            bookinfo:UpdateData(RomanceList[i]);
            table.insert(self.Romance_list,bookinfo)
        end
    end
    print(table.length(self.Romance_list))
end

--更新Suspense 列表
function MainCache:UpdatedSuspenseList(booklist)
    Cache.ClearList(self.Suspense_list);

    self.code=200;
    --【Suspense】
    local SuspenseList=booklist;
    local len=table.length(SuspenseList);
    if(SuspenseList and len>0)then
        for i = 1,len do
            local bookinfo =require("Logic/Cache/MainInfo/BookTypeInfo").New();
            bookinfo:UpdateData(SuspenseList[i]);
            table.insert(self.Suspense_list,bookinfo)
        end
    end
    print(table.length(self.Suspense_list))
end

--获取排行第一的书本
function MainCache:GetRankFirst()
    local len= table.length(self.popularity_ranklist);
    for i = 1, len do
        if(self.popularity_ranklist[i].ranking==1)then
            return self.popularity_ranklist[i];
        end
    end
    return nil;
end


--获取
function MainCache:GetRankByIndex(list,index)
    local len= table.length(list);
    for i = 1, len do
        if(list[i].ranking==index)then
            return list[i];
        end
    end
    return nil;
end


-- 析构函数
function MainCache:__delete()
    Cache.ClearList(self.mybook_list);
    Cache.ClearList(self.popularity_ranklist);
    Cache.ClearList(self.newbook_ranklist);
    Cache.ClearList(self.platform_ranklist);
    Cache.ClearList(self.weekly_list);
    Cache.ClearList(self.recommend_list);
    Cache.ClearList(self.LGBT_list);
    Cache.ClearList(self.Romance_list);
    Cache.ClearList(self.Suspense_list);
end


MainCache = MainCache:GetInstance()
return MainCache
-- 单例模式
local BaseClass = core.Class
local SearchCache = BaseClass("SearchCache", core.Singleton)


-- 构造函数
function SearchCache:__init()
    --消息码
    self.code=0;

    --【Romance 列表】
    self.Romance_list={};
    --【LGBT 列表】
    self.LGBT_list={};
    --【Action 列表】
    self.Action_list={};
    --【Youth 列表】
    self.Youth_list={};
    --【Adventure 列表】
    self.Adventure_list={};
    --【Drama 列表】
    self.Drama_list={};
    --【Comedy 列表】
    self.Comedy_list={};
    --【Horror 列表】
    self.Horror_list={};
    --【18+ 列表】
    self.Eighteen_list={};
    --【Fantasy 列表】
    self.Fantasy_list={};
    --【Suspense 列表】
    self.Suspense_list={};
    --【Others 列表】
    self.Others_list={};
end

--更新 列表
function SearchCache:UpdateList(_index,datas)

    self.code=datas.code;

    local curlist=self:GetListByIndex(_index);
    if(GameHelper.islistHave(curlist)==true)then
        return;
    end

    local mylist=datas.data.book_list;
    if(mylist)then
        local len=table.length(mylist);
        if(len>0)then
            for i = 1,len do
                local bookinfo =require("Logic/Cache/MainInfo/BookTypeInfo").New();
                bookinfo:UpdateData(mylist[i]);

                if(_index==BookType.Romance)then
                    table.insert(self.Romance_list,bookinfo)
                elseif(_index==BookType.LGBT)then
                    table.insert(self.LGBT_list,bookinfo)
                elseif(_index==BookType.Action)then
                    table.insert(self.Action_list,bookinfo)
                elseif(_index==BookType.Youth)then
                    table.insert(self.Youth_list,bookinfo)
                elseif(_index==BookType.Adventure)then
                    table.insert(self.Adventure_list,bookinfo)
                elseif(_index==BookType.Drama)then
                    table.insert(self.Drama_list,bookinfo)
                elseif(_index==BookType.Comedy)then
                    table.insert(self.Comedy_list,bookinfo)
                elseif(_index==BookType.Horror)then
                    table.insert(self.Horror_list,bookinfo)
                elseif(_index==BookType.Eighteen)then
                    table.insert(self.Eighteen_list,bookinfo)
                elseif(_index==BookType.Fantasy)then
                    table.insert(self.Fantasy_list,bookinfo)
                elseif(_index==BookType.Suspense)then
                    table.insert(self.Suspense_list,bookinfo)
                elseif(_index==BookType.Others)then
                    table.insert(self.Others_list,bookinfo)
                end
            end
        end
    end
end

--【通过编号获取列表】
function SearchCache:GetListByIndex(_index)
    if(_index==BookType.Romance)then
        return self.Romance_list;
    elseif(_index==BookType.LGBT)then
        return self.LGBT_list;
    elseif(_index==BookType.Action)then
        return self.Action_list;
    elseif(_index==BookType.Youth)then
        return self.Youth_list;
    elseif(_index==BookType.Adventure)then
        return self.Adventure_list;
    elseif(_index==BookType.Drama)then
        return self.Drama_list;
    elseif(_index==BookType.Comedy)then
        return self.Comedy_list;
    elseif(_index==BookType.Horror)then
        return self.Horror_list;
    elseif(_index==BookType.Eighteen)then
        return self.Eighteen_list;
    elseif(_index==BookType.Fantasy)then
        return self.Fantasy_list;
    elseif(_index==BookType.Suspense)then
        return self.Suspense_list;
    elseif(_index==BookType.Others)then
        return self.Others_list;
    end
end




-- 析构函数
function SearchCache:__delete()
    Cache.ClearList(self.Romance_list);
    Cache.ClearList(self.LGBT_list);
    Cache.ClearList(self.Action_list);
    Cache.ClearList(self.Youth_list);
    Cache.ClearList(self.Adventure_list);
    Cache.ClearList(self.Drama_list);
    Cache.ClearList(self.Comedy_list);
    Cache.ClearList(self.Horror_list);
    Cache.ClearList(self.Eighteen_list);
    Cache.ClearList(self.Fantasy_list);
    Cache.ClearList(self.Suspense_list);
    Cache.ClearList(self.Others_list);
end


SearchCache = SearchCache:GetInstance()
return SearchCache
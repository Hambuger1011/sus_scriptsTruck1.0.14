-- 单例模式
local PopWindowCache = core.Class("PopWindowCache", core.Singleton)

-- 构造函数
function PopWindowCache:__init()
     --弹窗列表，数组顺序为弹窗顺序
    self.window_list= {};

    --[临时缓存]
    self.daypassList={};

    --[临时缓存]
    self.daypassList2={};
end

--更新 列表
function PopWindowCache:UpdateList(datas)
    if(GameHelper.islistHave(self.window_list)==true)then
        Cache.ClearList(self.window_list);
    end
    local wList=datas.window_list;
    local len=table.length(wList);
    if(wList and len>0)then
        for i = 1,len do
            local info =require("Logic/Cache/PopWindowInfo/PopWindowInfo").New();
            info:UpdateData(wList[i]);
            table.insert(self.window_list,info);
        end
    end
end


function PopWindowCache:GetInfoById(id)
    if(GameHelper.islistHave(self.window_list)==true)then
        local len=table.length(self.window_list);
        for i = 1, len do
            if(self.window_list[i].id==id)then
                return self.window_list[i];
            end
        end
    end
    return nil;
end
function PopWindowCache:IsDayPassShow(bookId)
    if(GameHelper.islistHave(self.window_list)==true)then
        local len=table.length(self.window_list);
        for i = 1, len do
            if(self.window_list[i].id==2)then
                local isHave=self.window_list[i]:IsDayPassShow(bookId);
                return isHave;
            end
        end
    end
    return false;
end


function PopWindowCache:AddDayPass(bookId)
    if(GameHelper.islistHave(self.daypassList)==false)then
        table.insert(self.daypassList,bookId);
        return;
    end
    if(GameHelper.islistHave(self.daypassList)==true)then
        local len=table.length(self.daypassList)
        for i = 1, len do
            if(self.daypassList[i]==bookId)then
                return;
            end
        end
        table.insert(self.daypassList,bookId);
        return;
    end
end

function PopWindowCache:AddDayPass2(bookId)
    if(GameHelper.islistHave(self.daypassList2)==false)then
        table.insert(self.daypassList2,bookId);
        return;
    end
    if(GameHelper.islistHave(self.daypassList2)==true)then
        local len=table.length(self.daypassList2)
        for i = 1, len do
            if(self.daypassList2[i]==bookId)then
                return;
            end
        end
        table.insert(self.daypassList2,bookId);
        return;
    end
end

function PopWindowCache:IsHaveDayPass2(bookId)
    if(GameHelper.islistHave(self.daypassList2)==true)then
        local len=table.length(self.daypassList2)
        for i = 1, len do
            if(self.daypassList2[i]==bookId)then
                return true;
            end
        end
    end
    return false;
end

function PopWindowCache:__delete()
    self.window_list=nil;
end


PopWindowCache = PopWindowCache:GetInstance()
return PopWindowCache
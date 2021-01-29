local BaseClass = core.Class
local PopWindowInfo = BaseClass("PopWindowInfo")

function PopWindowInfo:__init()
    --功能的弹窗id，固定不可变，之后新功能只会增加
    self.id=0;
    --功能弹窗名称备注
    self.name="";
    --只有day pass弹窗会返回此字段
    self.book_list= nil;
end

function PopWindowInfo:UpdateData(data)
    self.id=data.id;
    self.name=data.name;


    if(GameHelper.islistHave(self.book_list)==true)then
        Cache.ClearList(self.book_list);
    end
    self.book_list= {};
    local booklist=data.book_list;
    local len=table.length(booklist);
    if(booklist and len>0)then
        for i = 1,len do
            local info =require("Logic/Cache/PopWindowInfo/PopBookInfo").New();
            info:UpdateData(booklist[i]);
            table.insert(self.book_list,info);
        end
    end
end

function PopWindowInfo:GetDayPassShow()
    if(GameHelper.islistHave(self.book_list)==true)then
        local len=table.length(self.book_list);
        for i = 1,len do
            if(self.book_list[i].isOpened==false and self.book_list[i].is_show==0)then
                return self.book_list[i];
            end
        end
    end
    return nil;
end

function PopWindowInfo:IsDayPassShow(bookId)
    if(GameHelper.islistHave(self.book_list)==true)then
        local len=table.length(self.book_list);
        for i = 1,len do
            if(self.book_list[i].book_id==bookId)then
                return true;
            end
        end
    end
    return false;
end

--销毁
function PopWindowInfo:__delete()
    self.id=nil;
    self.name=nil;
    self.book_list=nil;
end


return PopWindowInfo
